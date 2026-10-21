const http = require('http');
const fs = require('fs');
const path = require('path');
const { exec } = require('child_process');

const PORT = parseInt(process.env.LIAM_MCP_PORT || '17420', 10);
const ROOT_DIR = __dirname;
const HTML_PATH = path.join(ROOT_DIR, 'index.html');

let commandSeq = 1;
const commandQueue = [];
const pendingResolvers = new Map();
const waitingPolls = [];
let lastBrowserSeen = 0;

function enqueueBrowserCommand(action, params = {}, timeoutMs = 15000) {
    return new Promise(async (resolve) => {
        if (Date.now() - lastBrowserSeen > 6000) {
            openBrowserIfNeeded();
            await waitForBrowserConnection(8000);
        }

        const id = 'cmd_' + (commandSeq++) + '_' + Date.now();
        const item = { id, command: { action, params } };

        const timer = setTimeout(() => {
            pendingResolvers.delete(id);
            resolve({
                ok: false,
                error: 'Timeout esperando respuesta de la ventana de LiamToolAvatar2Unity en http://127.0.0.1:' + PORT
            });
        }, timeoutMs);

        pendingResolvers.set(id, (result) => {
            clearTimeout(timer);
            resolve(result);
        });

        if (waitingPolls.length > 0) {
            const pollRes = waitingPolls.shift();
            sendJson(pollRes, 200, item);
        } else {
            commandQueue.push(item);
        }
    });
}

function waitForBrowserConnection(maxWaitMs) {
    return new Promise((resolve) => {
        const start = Date.now();
        const check = () => {
            if (Date.now() - lastBrowserSeen < 4000) return resolve(true);
            if (Date.now() - start >= maxWaitMs) return resolve(false);
            setTimeout(check, 250);
        };
        check();
    });
}

let browserLaunchAttempted = 0;
function openBrowserIfNeeded() {
    if (Date.now() - browserLaunchAttempted < 10000) return;
    browserLaunchAttempted = Date.now();
    const url = `http://127.0.0.1:${PORT}/`;
    if (process.platform === 'win32') {
        exec(`start "" "${url}"`);
    } else if (process.platform === 'darwin') {
        exec(`open "${url}"`);
    } else {
        exec(`xdg-open "${url}"`);
    }
}

function sendJson(res, status, obj) {
    const body = JSON.stringify(obj);
    res.writeHead(status, {
        'Content-Type': 'application/json; charset=utf-8',
        'Access-Control-Allow-Origin': '*',
        'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
        'Access-Control-Allow-Headers': 'Content-Type',
        'Access-Control-Allow-Private-Network': 'true'
    });
    res.end(body);
}

const MCP_TOOLS = [
    {
        name: 'avatar_capture_screenshot',
        description: 'Toma una captura PNG en tiempo real del viewport del avatar (en modo setup o animate) o del Atlas de texturas empaquetado para que la IA pueda inspeccionar visualmente el rig, las mallas y las costuras.',
        inputSchema: {
            type: 'object',
            properties: {
                target: { type: 'string', enum: ['viewport', 'atlas'], default: 'viewport' },
                systemState: { type: 'string', enum: ['setup', 'animate'] },
                showGrid: { type: 'boolean' },
                showWire: { type: 'boolean' },
                showBones: { type: 'boolean' },
                showAnchors: { type: 'boolean' },
                showWeights: { type: 'boolean' },
                fitCamera: { type: 'boolean' }
            }
        }
    },
    {
        name: 'avatar_get_state',
        description: 'Obtiene el estado completo de LiamToolAvatar2Unity: resolucion del lienzo, huesos (coordenadas normalizadas nx1, ny1, nx2, ny2 y roles), partes en orden Z, puntos de anclaje anti-dislocacion y parametros de animacion.',
        inputSchema: {
            type: 'object',
            properties: {}
        }
    },
    {
        name: 'avatar_update_bones',
        description: 'Ajusta de forma inteligente las posiciones, roles o envoltura de uno o varios huesos del esqueleto, sincroniza los anclajes y recalcula los pesos de skinning.',
        inputSchema: {
            type: 'object',
            properties: {
                bones: {
                    type: 'array',
                    items: {
                        type: 'object',
                        properties: {
                            id: { type: 'string' },
                            label: { type: 'string' },
                            role: { type: 'string', enum: ['breath', 'head', 'armR', 'armL', 'pendulum', 'legR', 'legL'] },
                            nx1: { type: 'number' },
                            ny1: { type: 'number' },
                            nx2: { type: 'number' },
                            ny2: { type: 'number' },
                            envelope: { type: 'number' }
                        },
                        required: ['id']
                    }
                },
                syncAnchors: { type: 'boolean', default: true },
                includeScreenshot: { type: 'boolean', default: true }
            },
            required: ['bones']
        }
    },
    {
        name: 'avatar_update_part',
        description: 'Modifica una parte o capa del avatar: posicion, escala, rotacion, visibilidad, orden Z, huesos permitidos o punto de anclaje anti-dislocacion (parentBone, nx, ny, radiusPct, strength).',
        inputSchema: {
            type: 'object',
            properties: {
                id: { type: 'string' },
                name: { type: 'string' },
                visible: { type: 'boolean' },
                zOrderIndex: { type: 'number' },
                x: { type: 'number' },
                y: { type: 'number' },
                scaleX: { type: 'number' },
                scaleY: { type: 'number' },
                rot: { type: 'number' },
                allowedBones: { type: 'array', items: { type: 'string' } },
                anchor: {
                    type: 'object',
                    properties: {
                        parentBone: { type: 'string' },
                        nx: { type: 'number' },
                        ny: { type: 'number' },
                        radiusPct: { type: 'number' },
                        strength: { type: 'number' }
                    }
                },
                includeScreenshot: { type: 'boolean', default: true }
            },
            required: ['id']
        }
    },
    {
        name: 'avatar_smart_auto_adjust',
        description: 'Ejecuta el auto-ajuste inteligente de silueta: detecta automaticamente la pose sobre las partes visibles, suelda los anclajes anti-dislocacion, suaviza fronteras de pesos y empaqueta el Atlas.',
        inputSchema: {
            type: 'object',
            properties: {
                poseTemplate: { type: 'string', enum: ['dynamic', 'casual', 'apose', 'compact'] },
                autoDetectSilhouette: { type: 'boolean', default: true },
                relaxIterations: { type: 'number', default: 3 }
            }
        }
    },
    {
        name: 'avatar_configure_mesh_and_anim',
        description: 'Ajusta la densidad y margen de las mallas anti-recorte o los parametros procedurales de animacion (velocidad, respiracion, cadera, brazos, cabeza, IK de pies y clip).',
        inputSchema: {
            type: 'object',
            properties: {
                cols: { type: 'number' },
                paddingPct: { type: 'number' },
                fullRect: { type: 'boolean' },
                systemState: { type: 'string', enum: ['setup', 'animate'] },
                clip: { type: 'string', enum: ['idle', 'walk', 'jump', 'wave'] },
                playing: { type: 'boolean' },
                speed: { type: 'number' },
                breath: { type: 'number' },
                hipX: { type: 'number' },
                hipY: { type: 'number' },
                armSwing: { type: 'number' },
                forearmSwing: { type: 'number' },
                pendulumSwing: { type: 'number' },
                headTilt: { type: 'number' },
                footIK: { type: 'boolean' },
                includeScreenshot: { type: 'boolean', default: true }
            }
        }
    },
    {
        name: 'avatar_load_l2u_project',
        description: 'Carga un archivo de proyecto .l2u desde el disco hacia la herramienta activa.',
        inputSchema: {
            type: 'object',
            properties: {
                filePath: { type: 'string' }
            },
            required: ['filePath']
        }
    },
    {
        name: 'avatar_export_unity_bundle',
        description: 'Genera y guarda en disco el paquete completo para Unity: LiamAvatar_Atlas.png, LiamAvatar.json, LiamAvatar2UnityRuntime.cs y Proyecto_LiamAvatar2Unity.l2u.',
        inputSchema: {
            type: 'object',
            properties: {
                outputDir: { type: 'string' }
            }
        }
    }
];

function buildToolResponseContent(res, extraText) {
    const content = [];
    if (extraText) {
        content.push({ type: 'text', text: extraText });
    }
    if (res && res.screenshot && res.screenshot.dataUrl) {
        const base64Data = res.screenshot.dataUrl.replace(/^data:image\/png;base64,/, '');
        const capturePath = path.join(ROOT_DIR, 'mcp_last_capture.png');
        try {
            fs.writeFileSync(capturePath, Buffer.from(base64Data, 'base64'));
        } catch (e) {}
        content.push({
            type: 'image',
            data: base64Data,
            mimeType: 'image/png'
        });
    }
    const cleanCopy = { ...res };
    if (cleanCopy.screenshot) {
        cleanCopy.screenshot = {
            target: res.screenshot.target,
            width: res.screenshot.width,
            height: res.screenshot.height,
            savedToFile: path.join(ROOT_DIR, 'mcp_last_capture.png')
        };
    }
    content.push({ type: 'text', text: JSON.stringify(cleanCopy, null, 2) });
    return content;
}

async function handleMcpToolCall(name, args = {}) {
    if (name === 'avatar_capture_screenshot') {
        const res = await enqueueBrowserCommand('capture_screenshot', args);
        return { content: buildToolResponseContent(res, 'Captura realizada.') };
    }
    if (name === 'avatar_get_state') {
        const res = await enqueueBrowserCommand('get_state', {});
        return { content: buildToolResponseContent(res, 'Estado actual del estudio.') };
    }
    if (name === 'avatar_update_bones') {
        const res = await enqueueBrowserCommand('update_bones', args);
        return { content: buildToolResponseContent(res, 'Huesos y pesos actualizados.') };
    }
    if (name === 'avatar_update_part') {
        const res = await enqueueBrowserCommand('update_part', args);
        return { content: buildToolResponseContent(res, 'Parte actualizada.') };
    }
    if (name === 'avatar_smart_auto_adjust') {
        const res = await enqueueBrowserCommand('smart_auto_adjust', args);
        return { content: buildToolResponseContent(res, 'Auto-ajuste inteligente completado.') };
    }
    if (name === 'avatar_configure_mesh_and_anim') {
        const res = await enqueueBrowserCommand('configure_mesh_and_anim', args);
        return { content: buildToolResponseContent(res, 'Configuracion de malla y animacion actualizada.') };
    }
    if (name === 'avatar_load_l2u_project') {
        const targetFile = path.resolve(args.filePath);
        if (!fs.existsSync(targetFile)) {
            return { isError: true, content: [{ type: 'text', text: 'No existe el archivo: ' + targetFile }] };
        }
        const projectData = JSON.parse(fs.readFileSync(targetFile, 'utf8'));
        const res = await enqueueBrowserCommand('load_l2u_json', { projectData }, 20000);
        return { content: buildToolResponseContent(res, 'Proyecto .L2U cargado desde ' + targetFile) };
    }
    if (name === 'avatar_export_unity_bundle') {
        const outDir = args.outputDir ? path.resolve(args.outputDir) : ROOT_DIR;
        fs.mkdirSync(outDir, { recursive: true });
        const res = await enqueueBrowserCommand('export_unity_bundle', {}, 20000);
        if (!res || !res.ok) {
            return { isError: true, content: [{ type: 'text', text: JSON.stringify(res) }] };
        }
        const atlasPath = path.join(outDir, 'LiamAvatar_Atlas.png');
        const jsonPath = path.join(outDir, 'LiamAvatar.json');
        const csPath = path.join(outDir, 'LiamAvatar2UnityRuntime.cs');
        const l2uPath = path.join(outDir, 'Proyecto_LiamAvatar2Unity.l2u');

        const pngBase64 = res.atlasPngDataUrl.replace(/^data:image\/png;base64,/, '');
        fs.writeFileSync(atlasPath, Buffer.from(pngBase64, 'base64'));
        fs.writeFileSync(jsonPath, res.jsonContent, 'utf8');
        fs.writeFileSync(csPath, res.csContent, 'utf8');
        fs.writeFileSync(l2uPath, res.l2uContent, 'utf8');

        return {
            content: [{
                type: 'text',
                text: JSON.stringify({
                    ok: true,
                    atlasMeta: res.atlasMeta,
                    filesWritten: { atlasPath, jsonPath, csPath, l2uPath }
                }, null, 2)
            }]
        };
    }

    return {
        isError: true,
        content: [{ type: 'text', text: 'Herramienta MCP desconocida: ' + name }]
    };
}

async function processJsonRpcMessage(msg) {
    if (!msg || typeof msg !== 'object') return null;

    if (msg.method === 'initialize') {
        return {
            jsonrpc: '2.0',
            id: msg.id,
            result: {
                protocolVersion: (msg.params && msg.params.protocolVersion) || '2024-11-05',
                capabilities: {
                    tools: {}
                },
                serverInfo: {
                    name: 'liam-avatar2unity-mcp',
                    version: '3.0.0'
                }
            }
        };
    }

    if (msg.method === 'notifications/initialized') {
        return null;
    }

    if (msg.method === 'ping') {
        return { jsonrpc: '2.0', id: msg.id, result: {} };
    }

    if (msg.method === 'tools/list') {
        return {
            jsonrpc: '2.0',
            id: msg.id,
            result: { tools: MCP_TOOLS }
        };
    }

    if (msg.method === 'tools/call') {
        try {
            const p = msg.params || {};
            const toolResult = await handleMcpToolCall(p.name, p.arguments || {});
            return {
                jsonrpc: '2.0',
                id: msg.id,
                result: toolResult
            };
        } catch (err) {
            return {
                jsonrpc: '2.0',
                id: msg.id,
                result: {
                    isError: true,
                    content: [{ type: 'text', text: String(err && err.message ? err.message : err) }]
                }
            };
        }
    }

    if (msg.id !== undefined) {
        return {
            jsonrpc: '2.0',
            id: msg.id,
            error: { code: -32601, message: 'Method not found: ' + msg.method }
        };
    }

    return null;
}

const server = http.createServer((req, res) => {
    if (req.method === 'OPTIONS') {
        res.writeHead(204, {
            'Access-Control-Allow-Origin': '*',
            'Access-Control-Allow-Methods': 'GET, POST, OPTIONS',
            'Access-Control-Allow-Headers': 'Content-Type',
            'Access-Control-Allow-Private-Network': 'true'
        });
        return res.end();
    }

    const urlObj = new URL(req.url, `http://127.0.0.1:${PORT}`);

    if (req.method === 'GET' && (urlObj.pathname === '/' || urlObj.pathname === '/index.html')) {
        if (fs.existsSync(HTML_PATH)) {
            res.writeHead(200, { 'Content-Type': 'text/html; charset=utf-8' });
            return res.end(fs.readFileSync(HTML_PATH, 'utf8'));
        }
        res.writeHead(404);
        return res.end('index.html not found');
    }

    if (req.method === 'GET' && urlObj.pathname === '/bridge/poll') {
        lastBrowserSeen = Date.now();
        if (commandQueue.length > 0) {
            const nextCmd = commandQueue.shift();
            return sendJson(res, 200, nextCmd);
        }
        const pollTimer = setTimeout(() => {
            const idx = waitingPolls.indexOf(res);
            if (idx !== -1) waitingPolls.splice(idx, 1);
            sendJson(res, 200, { id: null });
        }, 8000);

        req.on('close', () => {
            clearTimeout(pollTimer);
            const idx = waitingPolls.indexOf(res);
            if (idx !== -1) waitingPolls.splice(idx, 1);
        });

        waitingPolls.push(res);
        return;
    }

    if (req.method === 'POST' && urlObj.pathname === '/bridge/result') {
        lastBrowserSeen = Date.now();
        let body = '';
        req.on('data', chunk => { body += chunk; });
        req.on('end', () => {
            try {
                const parsed = JSON.parse(body);
                if (parsed && parsed.id && pendingResolvers.has(parsed.id)) {
                    const resolve = pendingResolvers.get(parsed.id);
                    pendingResolvers.delete(parsed.id);
                    resolve(parsed.result);
                }
                sendJson(res, 200, { ok: true });
            } catch (e) {
                sendJson(res, 400, { ok: false });
            }
        });
        return;
    }

    if (req.method === 'POST' && urlObj.pathname === '/mcp') {
        let body = '';
        req.on('data', chunk => { body += chunk; });
        req.on('end', async () => {
            try {
                const msg = JSON.parse(body);
                const reply = await processJsonRpcMessage(msg);
                if (reply) sendJson(res, 200, reply);
                else {
                    res.writeHead(202, { 'Access-Control-Allow-Origin': '*' });
                    res.end();
                }
            } catch (e) {
                sendJson(res, 400, { jsonrpc: '2.0', id: null, error: { code: -32700, message: 'Parse error' } });
            }
        });
        return;
    }

    sendJson(res, 404, { error: 'Not found' });
});

server.on('error', () => {});
server.listen(PORT, '127.0.0.1');

let stdioBuffer = Buffer.alloc(0);
let useHeaderFraming = false;

function writeStdioResponse(obj) {
    if (!obj) return;
    const json = JSON.stringify(obj);
    if (useHeaderFraming) {
        const byteLen = Buffer.byteLength(json, 'utf8');
        process.stdout.write(`Content-Length: ${byteLen}\r\n\r\n${json}`);
    } else {
        process.stdout.write(json + '\n');
    }
}

process.stdin.on('data', async (chunk) => {
    stdioBuffer = Buffer.concat([stdioBuffer, chunk]);

    while (stdioBuffer.length > 0) {
        const str = stdioBuffer.toString('utf8');
        if (str.startsWith('Content-Length:')) {
            useHeaderFraming = true;
            const headerEnd = stdioBuffer.indexOf('\r\n\r\n');
            if (headerEnd === -1) break;
            const headerStr = stdioBuffer.slice(0, headerEnd).toString('utf8');
            const m = headerStr.match(/Content-Length:\s*(\d+)/i);
            const len = m ? parseInt(m[1], 10) : 0;
            const totalNeeded = headerEnd + 4 + len;
            if (stdioBuffer.length < totalNeeded) break;
            const bodyBuf = stdioBuffer.slice(headerEnd + 4, totalNeeded);
            stdioBuffer = stdioBuffer.slice(totalNeeded);
            try {
                const msg = JSON.parse(bodyBuf.toString('utf8'));
                const reply = await processJsonRpcMessage(msg);
                writeStdioResponse(reply);
            } catch (e) {}
        } else {
            const nlIdx = stdioBuffer.indexOf('\n');
            if (nlIdx === -1) break;
            const line = stdioBuffer.slice(0, nlIdx).toString('utf8').trim();
            stdioBuffer = stdioBuffer.slice(nlIdx + 1);
            if (!line) continue;
            try {
                const msg = JSON.parse(line);
                const reply = await processJsonRpcMessage(msg);
                writeStdioResponse(reply);
            } catch (e) {}
        }
    }
});

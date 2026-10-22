# LiamToolAvatar2Unity

Herramienta web (single-file HTML) para crear, riggear y animar **avatares 2D en alta resolución (1536x2752)** estilo Just Dance y exportarlos listos para Unity con empaquetador de **Atlas de Texturas Inteligente**.

- **Google Sites**: https://sites.google.com/view/liamtoolavatar2unity/full-page-embed
- **GitHub Pages (Auto-actualizable)**: https://ey-liam.github.io/LiamToolAvatar2Unity/

## Características Principales

- **Lienzo Nativo en Alta Resolución (1536 x 2752)**: Importación 100% fiel de partes PNG sin recortes ni reducciones de resolución.
- **Smart Texture Atlas Packer (`LiamAvatar_Atlas.png`)**: Escanea los píxeles visibles de todas las capas, elimina el espacio transparente vacío y empaqueta todas las piezas en un solo Atlas PNG compacto.
- **Exportador Directo a Unity (`LiamAvatar.json` + `LiamAvatar2UnityRuntime.cs`)**: Exporta coordenadas `canvasRect`, `atlasRect`, puntos de anclaje anti-dislocación (`anchorWeld`) y el script de Unity libre de errores de compilación.
- **Rigging, Puntos de Anclaje y Pincel de Pesos**: Presets de 6 y 15 capas, variantes de piernas, plantillas de pose, auto-detección por silueta, mallas anti-recorte y línea de tiempo con física.
- **Formato de Proyecto `.L2U`**: Guardado y carga de proyectos editables completos con retrocompatibilidad automática.
- **Servidor MCP Integrado (`mcp-server.js`)**: Permite que agentes de IA tomen capturas en tiempo real del viewport o del Atlas, inspeccionen el estado del rig, ajusten huesos/anclajes/pesos de forma inteligente y exporten los paquetes de Unity automáticamente.

## Licencia y Creditos
- Si usas la Tool dame los créditos necesarios en tu proyecto
- No robes/copies/clones nada del codigo fuente

## Estructura del Repositorio

- `index.html` — Aplicación completa auto-contenida (lista para abrir en navegador o incrustar en Google Sites).
- `LiamAvatar2UnityRuntime.cs` — Script Runtime de Unity para renderizar y animar el Atlas y el JSON.
- `mcp_config.json` — Configuración de conexión MCP.

## Licencia

Proyecto personal de Liam. Uso libre para aprendizaje; contactar al autor para otros usos.


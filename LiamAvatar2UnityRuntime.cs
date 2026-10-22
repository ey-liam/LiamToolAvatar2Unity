using UnityEngine;
using System.Collections.Generic;
using System.Globalization;
using System.Text.RegularExpressions;
#if UNITY_EDITOR
using UnityEditor;
#endif

public class LiamAvatar2UnityRuntime : MonoBehaviour
{
    [System.Serializable]
    public class AvatarPartLayer
    {
        public string partName;
        public bool visible = true;
        public Texture2D partTexture;
        public Vector2 anchorUV = new Vector2(0.49f, 0.72f);
        [Range(0f, 1f)] public float seamWeldStrength = 0.85f;
        public Vector4 canvasRect = new Vector4(0f, 0f, 1f, 1f);
        public Vector4 atlasRect = new Vector4(0f, 0f, 1f, 1f);

        [HideInInspector] public Vector2[] customUVs;
        [HideInInspector] public int[] customTriangles;
        [HideInInspector] public Mesh runtimeMesh;
        [HideInInspector] public Vector3[] restVerts;
        [HideInInspector] public Vector3[] deformedVerts;
        [HideInInspector] public Vector2[] canvasUVs;
        [HideInInspector] public Vector2[] meshUVs;
    }

    [Header("1. Atlas Unico y Archivo JSON (Alta Resolucion 1536x2752)")]
    public TextAsset jsonFile;
    public Texture2D sharedAtlasTexture;
    public float canvasWidth = 1536f;
    public float canvasHeight = 2752f;
    public bool autoDetectResolutionFromTexture = true;

    [Header("2. Capas en Orden Z (Cabeza, Brazo 1 (Izq Pantalla), Torso, Brazo 2 (Der Pantalla), PiernasConCalcetasYTenis, Falda, Pierna1, Pierna2)")]
    public List<AvatarPartLayer> avatarParts = new List<AvatarPartLayer>();

    [Header("3. Parametros de Malla y Animacion")]
    public int gridColumns = 22;
    public int gridRows = 30;
    public float worldScale = 5.0f;
    public float cycleSpeed = 0.80f;
    public float breathAmount = 0.020f;
    public float hipSwayX = 0.0100f;
    public float hipBounceY = 0.0100f;
    public float armSwingDeg = 2.20f;
    public float headTiltDeg = 5.00f;
    public float legSwingAmount = 0.015f;
    public bool lockFeetIK = true;

    void Reset()
    {
        AutoBindEditorAssets();
        if (jsonFile != null) ParseJsonData(jsonFile.text);
        else EnsureDefaultLayers();
    }

    void OnValidate()
    {
        AutoBindEditorAssets();
        if (avatarParts == null || avatarParts.Count == 0)
        {
            if (jsonFile != null) ParseJsonData(jsonFile.text);
            else EnsureDefaultLayers();
        }
    }

    private void AutoBindEditorAssets()
    {
#if UNITY_EDITOR
        if (sharedAtlasTexture == null)
        {
            string[] guids = AssetDatabase.FindAssets("LiamAvatar_Atlas t:Texture2D");
            if (guids != null && guids.Length > 0)
            {
                string path = AssetDatabase.GUIDToAssetPath(guids[0]);
                TextureImporter ti = AssetImporter.GetAtPath(path) as TextureImporter;
                if (ti != null && (ti.maxTextureSize < 4096 || !ti.alphaIsTransparency))
                {
                    ti.maxTextureSize = 4096;
                    ti.alphaIsTransparency = true;
                    ti.textureCompression = TextureImporterCompression.Uncompressed;
                    ti.SaveAndReimport();
                }
                sharedAtlasTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(path);
            }
        }
        if (jsonFile == null)
        {
            string[] jGuids = AssetDatabase.FindAssets("LiamAvatar t:TextAsset");
            if (jGuids != null && jGuids.Length > 0)
            {
                string jPath = AssetDatabase.GUIDToAssetPath(jGuids[0]);
                jsonFile = AssetDatabase.LoadAssetAtPath<TextAsset>(jPath);
            }
        }
#endif
    }

    [ContextMenu("Cargar Datos desde JSON")]
    public void LoadFromJsonInEditor()
    {
        AutoBindEditorAssets();
        if (jsonFile == null)
        {
            EnsureDefaultLayers();
            return;
        }
        ParseJsonData(jsonFile.text);
    }

    private void EnsureDefaultLayers()
    {
        if (avatarParts != null && avatarParts.Count > 0) return;
        avatarParts = new List<AvatarPartLayer>
        {
            new AvatarPartLayer { partName = "Cabeza", visible = true, anchorUV = new Vector2(0.5348f, 0.7100f), seamWeldStrength = 0.850f, canvasRect = new Vector4(0.34245f, 0.70567f, 0.62435f, 0.88663f), atlasRect = new Vector4(0.63031f, 0.27342f, 0.87248f, 0.47190f) },
            new AvatarPartLayer { partName = "Brazo 1 (Izq Pantalla)", visible = true, anchorUV = new Vector2(0.4283f, 0.7200f), seamWeldStrength = 0.900f, canvasRect = new Vector4(0.03841f, 0.47347f, 0.42253f, 0.70930f), atlasRect = new Vector4(0.29698f, 0.21323f, 0.62696f, 0.47190f) },
            new AvatarPartLayer { partName = "Torso", visible = true, anchorUV = new Vector2(0.4867f, 0.5200f), seamWeldStrength = 0.000f, canvasRect = new Vector4(0.33529f, 0.49746f, 0.67318f, 0.76672f), atlasRect = new Vector4(0.00336f, 0.17656f, 0.29362f, 0.47190f) },
            new AvatarPartLayer { partName = "Brazo 2 (Der Pantalla)", visible = true, anchorUV = new Vector2(0.6009f, 0.7244f), seamWeldStrength = 0.900f, canvasRect = new Vector4(0.56901f, 0.59375f, 0.94206f, 0.74273f), atlasRect = new Vector4(0.37248f, 0.01076f, 0.69295f, 0.17417f) },
            new AvatarPartLayer { partName = "PiernasConCalcetasYTenis", visible = false, anchorUV = new Vector2(0.5655f, 0.4990f), seamWeldStrength = 0.850f, canvasRect = new Vector4(0.28841f, 0.05305f, 0.81901f, 0.53016f), atlasRect = new Vector4(0.00336f, 0.47429f, 0.45917f, 0.99761f) },
            new AvatarPartLayer { partName = "Falda", visible = true, anchorUV = new Vector2(0.5655f, 0.4990f), seamWeldStrength = 0.850f, canvasRect = new Vector4(0.34701f, 0.38263f, 0.77279f, 0.53924f), atlasRect = new Vector4(0.00336f, 0.00239f, 0.36913f, 0.17417f) },
            new AvatarPartLayer { partName = "Pierna1", visible = true, anchorUV = new Vector2(0.4867f, 0.6500f), seamWeldStrength = 0.800f, canvasRect = new Vector4(0.28776f, 0.05305f, 0.57487f, 0.43060f), atlasRect = new Vector4(0.46253f, 0.58350f, 0.70917f, 0.99761f) },
            new AvatarPartLayer { partName = "Pierna2", visible = true, anchorUV = new Vector2(0.4867f, 0.6500f), seamWeldStrength = 0.800f, canvasRect = new Vector4(0.48698f, 0.15225f, 0.81380f, 0.46039f), atlasRect = new Vector4(0.71253f, 0.65963f, 0.99329f, 0.99761f) }
        };
    }

    void Start()
    {
        AutoBindEditorAssets();
        if (jsonFile != null)
        {
            ParseJsonData(jsonFile.text);
        }
        else
        {
            EnsureDefaultLayers();
        }

        if (autoDetectResolutionFromTexture && sharedAtlasTexture == null)
        {
            foreach (AvatarPartLayer layer in avatarParts)
            {
                if (layer.partTexture != null)
                {
                    canvasWidth = layer.partTexture.width;
                    canvasHeight = layer.partTexture.height;
                    break;
                }
            }
        }

        float aspect = canvasWidth / Mathf.Max(1f, canvasHeight);

        for (int pIdx = 0; pIdx < avatarParts.Count; pIdx++)
        {
            AvatarPartLayer layer = avatarParts[pIdx];
            GameObject child = new GameObject("Part_" + layer.partName);
            child.transform.SetParent(transform, false);
            child.transform.localPosition = new Vector3(0f, 0f, pIdx * 0.005f);
            child.SetActive(layer.visible);

            MeshFilter mf = child.AddComponent<MeshFilter>();
            MeshRenderer mr = child.AddComponent<MeshRenderer>();

            Shader spriteShader = Shader.Find("Sprites/Default");
            if (spriteShader == null) spriteShader = Shader.Find("Universal Render Pipeline/2D/Sprite-Unlit-Default");
            if (spriteShader == null) spriteShader = Shader.Find("Unlit/Transparent");
            mr.material = new Material(spriteShader);
            mr.sortingOrder = (avatarParts.Count - pIdx) * 10;

            bool useAtlas = (layer.partTexture == null && sharedAtlasTexture != null);
            if (layer.partTexture != null)
            {
                mr.material.mainTexture = layer.partTexture;
            }
            else if (sharedAtlasTexture != null)
            {
                mr.material.mainTexture = sharedAtlasTexture;
            }

            layer.runtimeMesh = new Mesh { name = "Mesh_" + layer.partName };
            mf.mesh = layer.runtimeMesh;

            if (!useAtlas && layer.customUVs != null && layer.customUVs.Length > 0 && layer.customTriangles != null && layer.customTriangles.Length > 0)
            {
                int vCount = layer.customUVs.Length;
                layer.canvasUVs = new Vector2[vCount];
                layer.meshUVs = new Vector2[vCount];
                layer.restVerts = new Vector3[vCount];
                layer.deformedVerts = new Vector3[vCount];

                for (int i = 0; i < vCount; i++)
                {
                    Vector2 uv = layer.customUVs[i];
                    layer.canvasUVs[i] = uv;
                    layer.meshUVs[i] = uv;
                    layer.restVerts[i] = new Vector3((uv.x - 0.5f) * worldScale * aspect, (uv.y - 0.5f) * worldScale, 0f);
                }

                layer.runtimeMesh.vertices = layer.restVerts;
                layer.runtimeMesh.uv = layer.meshUVs;
                layer.runtimeMesh.triangles = layer.customTriangles;
            }
            else
            {
                int vCount = (gridColumns + 1) * (gridRows + 1);
                layer.restVerts = new Vector3[vCount];
                layer.deformedVerts = new Vector3[vCount];
                layer.canvasUVs = new Vector2[vCount];
                layer.meshUVs = new Vector2[vCount];

                bool hasValidRect = (layer.canvasRect.z > layer.canvasRect.x && layer.canvasRect.w > layer.canvasRect.y);
                Vector4 cRect = (useAtlas && hasValidRect) ? layer.canvasRect : new Vector4(0f, 0f, 1f, 1f);
                Vector4 aRect = (useAtlas && hasValidRect) ? layer.atlasRect : new Vector4(0f, 0f, 1f, 1f);

                int idx = 0;
                for (int r = 0; r <= gridRows; r++)
                {
                    float ty = (float)r / gridRows;
                    float cy = Mathf.Lerp(cRect.y, cRect.w, ty);
                    float vy = Mathf.Lerp(aRect.y, aRect.w, ty);

                    for (int c = 0; c <= gridColumns; c++)
                    {
                        float tx = (float)c / gridColumns;
                        float cx = Mathf.Lerp(cRect.x, cRect.z, tx);
                        float ux = Mathf.Lerp(aRect.x, aRect.z, tx);

                        layer.canvasUVs[idx] = new Vector2(cx, cy);
                        layer.meshUVs[idx] = new Vector2(ux, vy);
                        layer.restVerts[idx] = new Vector3((cx - 0.5f) * worldScale * aspect, (cy - 0.5f) * worldScale, 0f);
                        idx++;
                    }
                }

                int[] tris = new int[gridColumns * gridRows * 6];
                int t = 0;
                for (int r = 0; r < gridRows; r++)
                {
                    for (int c = 0; c < gridColumns; c++)
                    {
                        int i0 = r * (gridColumns + 1) + c;
                        int i1 = i0 + 1;
                        int i2 = i0 + (gridColumns + 1);
                        int i3 = i2 + 1;
                        tris[t++] = i0; tris[t++] = i2; tris[t++] = i1;
                        tris[t++] = i1; tris[t++] = i2; tris[t++] = i3;
                    }
                }

                layer.runtimeMesh.vertices = layer.restVerts;
                layer.runtimeMesh.uv = layer.meshUVs;
                layer.runtimeMesh.triangles = tris;
            }

            layer.runtimeMesh.RecalculateBounds();
        }
    }

    void Update()
    {
        float phase = Time.time * cycleSpeed;
        float sinP = Mathf.Sin(phase);
        float cosP = Mathf.Cos(phase);
        float aspect = canvasWidth / Mathf.Max(1f, canvasHeight);

        foreach (AvatarPartLayer layer in avatarParts)
        {
            if (layer.runtimeMesh == null || !layer.visible) continue;
            string n = (layer.partName ?? "").ToLower();

            Vector3 anchorWorld = new Vector3(
                (layer.anchorUV.x - 0.5f) * worldScale * aspect,
                (layer.anchorUV.y - 0.5f) * worldScale,
                0f
            );

            for (int i = 0; i < layer.restVerts.Length; i++)
            {
                Vector3 p = layer.restVerts[i];
                Vector2 cuv = layer.canvasUVs[i];

                float groundLock = lockFeetIK ? Mathf.Clamp01((cuv.y - 0.05f) * 2.2f) : 1.0f;
                float distToAnchor = Vector2.Distance(cuv, layer.anchorUV);
                float mobileFactor = Mathf.Clamp01(distToAnchor / 0.12f) * (1f - layer.seamWeldStrength * Mathf.Exp(-distToAnchor * 15f));

                if (n.Contains("cabeza") || n.Contains("head") || n.Contains("cabello") || n.Contains("lentes") || n.Contains("ojos"))
                {
                    float rad = Mathf.Sin(phase - 0.5f) * headTiltDeg * Mathf.Deg2Rad * mobileFactor;
                    p = RotateAround2D(p, anchorWorld, rad);
                }
                else if (n.Contains("brazo") || n.Contains("arm"))
                {
                    float dir = (n.Contains("2") || n.Contains("der")) ? -1f : 1f;
                    float rad = -Mathf.Sin(phase - 0.35f) * dir * armSwingDeg * Mathf.Deg2Rad * mobileFactor;
                    p = RotateAround2D(p, anchorWorld, rad);
                }
                else if (n.Contains("falda") || n.Contains("skirt") || n.Contains("correa"))
                {
                    p.x += Mathf.Sin(phase - 0.4f) * (hipSwayX * 1.1f) * mobileFactor;
                }
                else if (n.Contains("pierna") || n.Contains("leg") || n.Contains("muslo") || n.Contains("calceta") || n.Contains("pantalon"))
                {
                    p.x += cosP * legSwingAmount * mobileFactor * groundLock;
                }

                p.x += cosP * hipSwayX * groundLock;
                p.y -= Mathf.Abs(sinP) * hipBounceY * groundLock;
                p.y += sinP * breathAmount * Mathf.Clamp01(cuv.y - 0.48f);

                layer.deformedVerts[i] = p;
            }

            layer.runtimeMesh.vertices = layer.deformedVerts;
        }
    }

    private Vector3 RotateAround2D(Vector3 pt, Vector3 pivot, float rad)
    {
        float c = Mathf.Cos(rad);
        float s = Mathf.Sin(rad);
        float dx = pt.x - pivot.x;
        float dy = pt.y - pivot.y;
        return new Vector3(pivot.x + dx * c - dy * s, pivot.y + dx * s + dy * c, pt.z);
    }

    public void ParseJsonData(string jsonText)
    {
        if (string.IsNullOrEmpty(jsonText)) return;

        Match wMatch = Regex.Match(jsonText, @"""width""\s*:\s*(\d+)");
        Match hMatch = Regex.Match(jsonText, @"""height""\s*:\s*(\d+)");
        if (wMatch.Success) canvasWidth = float.Parse(wMatch.Groups[1].Value, CultureInfo.InvariantCulture);
        if (hMatch.Success) canvasHeight = float.Parse(hMatch.Groups[1].Value, CultureInfo.InvariantCulture);

        cycleSpeed = ParseFloatKey(jsonText, "cycleSpeed", cycleSpeed);
        breathAmount = ParseFloatKey(jsonText, "breathAmount", breathAmount);
        hipSwayX = ParseFloatKey(jsonText, "hipSwayX", hipSwayX);
        hipBounceY = ParseFloatKey(jsonText, "hipBounceY", hipBounceY);
        armSwingDeg = ParseFloatKey(jsonText, "armSwingDeg", armSwingDeg);
        headTiltDeg = ParseFloatKey(jsonText, "headTiltDeg", headTiltDeg);

        Dictionary<string, Texture2D> savedTextures = new Dictionary<string, Texture2D>();
        if (avatarParts != null)
        {
            foreach (AvatarPartLayer existing in avatarParts)
            {
                if (!string.IsNullOrEmpty(existing.partName) && existing.partTexture != null)
                {
                    savedTextures[existing.partName] = existing.partTexture;
                }
            }
        }

        List<AvatarPartLayer> parsed = new List<AvatarPartLayer>();
        string[] blocks = jsonText.Split(new string[] { "\"slotName\"" }, System.StringSplitOptions.RemoveEmptyEntries);

        for (int s = 1; s < blocks.Length; s++)
        {
            string b = blocks[s];
            AvatarPartLayer layer = new AvatarPartLayer();

            Match mName = Regex.Match(b, @"^\s*:\s*""([^""]+)""");
            if (mName.Success) layer.partName = mName.Groups[1].Value;

            Match mVis = Regex.Match(b, @"""visible""\s*:\s*(true|false)");
            if (mVis.Success) layer.visible = (mVis.Groups[1].Value == "true");

            Match mNx = Regex.Match(b, @"""nx""\s*:\s*([-+]?[0-9]*\.?[0-9]+)");
            Match mNy = Regex.Match(b, @"""ny""\s*:\s*([-+]?[0-9]*\.?[0-9]+)");
            if (mNx.Success && mNy.Success)
            {
                float nx = float.Parse(mNx.Groups[1].Value, CultureInfo.InvariantCulture);
                float ny = float.Parse(mNy.Groups[1].Value, CultureInfo.InvariantCulture);
                layer.anchorUV = new Vector2(nx, 1f - ny);
            }

            Match mStr = Regex.Match(b, @"""strength""\s*:\s*([-+]?[0-9]*\.?[0-9]+)");
            if (mStr.Success) layer.seamWeldStrength = float.Parse(mStr.Groups[1].Value, CultureInfo.InvariantCulture);

            layer.canvasRect = new Vector4(
                ParseFloatKey(b, "xMin", 0f),
                ParseFloatKey(b, "yMin", 0f),
                ParseFloatKey(b, "xMax", 1f),
                ParseFloatKey(b, "yMax", 1f)
            );

            layer.atlasRect = new Vector4(
                ParseFloatKey(b, "uMin", 0f),
                ParseFloatKey(b, "vMin", 0f),
                ParseFloatKey(b, "uMax", 1f),
                ParseFloatKey(b, "vMax", 1f)
            );

            int uvsStart = b.IndexOf("\"uvs\"");
            int trisStart = b.IndexOf("\"triangles\"");
            if (uvsStart != -1 && trisStart != -1)
            {
                string uvsSection = b.Substring(uvsStart, trisStart - uvsStart);
                MatchCollection uvMatches = Regex.Matches(uvsSection, @"\[\s*([-+]?[0-9]*\.?[0-9]+)\s*,\s*([-+]?[0-9]*\.?[0-9]+)\s*\]");
                layer.customUVs = new Vector2[uvMatches.Count];
                for (int i = 0; i < uvMatches.Count; i++)
                {
                    float u = float.Parse(uvMatches[i].Groups[1].Value, CultureInfo.InvariantCulture);
                    float v = float.Parse(uvMatches[i].Groups[2].Value, CultureInfo.InvariantCulture);
                    layer.customUVs[i] = new Vector2(u, v);
                }

                string trisSection = b.Substring(trisStart);
                int endMesh = trisSection.IndexOf("}");
                if (endMesh != -1) trisSection = trisSection.Substring(0, endMesh);

                MatchCollection triMatches = Regex.Matches(trisSection, @"\[\s*(\d+)\s*,\s*(\d+)\s*,\s*(\d+)\s*\]");
                layer.customTriangles = new int[triMatches.Count * 3];
                for (int i = 0; i < triMatches.Count; i++)
                {
                    int i0 = int.Parse(triMatches[i].Groups[1].Value, CultureInfo.InvariantCulture);
                    int i1 = int.Parse(triMatches[i].Groups[2].Value, CultureInfo.InvariantCulture);
                    int i2 = int.Parse(triMatches[i].Groups[3].Value, CultureInfo.InvariantCulture);
                    layer.customTriangles[i * 3] = i0;
                    layer.customTriangles[i * 3 + 1] = i2;
                    layer.customTriangles[i * 3 + 2] = i1;
                }
            }

            if (!string.IsNullOrEmpty(layer.partName) && savedTextures.TryGetValue(layer.partName, out Texture2D tex))
            {
                layer.partTexture = tex;
            }
            else if (avatarParts != null && s - 1 < avatarParts.Count && avatarParts[s - 1].partTexture != null)
            {
                layer.partTexture = avatarParts[s - 1].partTexture;
            }

            parsed.Add(layer);
        }

        if (parsed.Count > 0) avatarParts = parsed;
    }

    private float ParseFloatKey(string text, string key, float fallback)
    {
        Match m = Regex.Match(text, "\"" + key + "\"\\s*:\\s*([-+]?[0-9]*\\.?[0-9]+)");
        if (m.Success) return float.Parse(m.Groups[1].Value, CultureInfo.InvariantCulture);
        return fallback;
    }
}

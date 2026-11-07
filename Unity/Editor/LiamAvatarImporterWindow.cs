#if UNITY_EDITOR
using System;
using System.IO;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using LiamTool.Avatar;

namespace LiamTool.Avatar.Editor
{
    public class LiamAvatarImporterWindow : EditorWindow
    {
        private TextAsset jsonAsset;
        private Texture2D atlasTexture;
        private Shader customShader;
        private string avatarName = "CustomAvatar";
        private bool createPrefab = false;
        private string outputPrefabPath = "Assets/CustomAvatars";

        [MenuItem("Tools/LiamTool/Avatar & Card Importer")]
        public static void ShowWindow()
        {
            var window = GetWindow<LiamAvatarImporterWindow>("LiamTool Importer");
            window.minSize = new Vector2(380, 420);
        }

        private void OnEnable()
        {
            if (customShader == null)
            {
                customShader = Shader.Find("LiamTool/Avatar2D_CustomCard") ?? Shader.Find("Sprites/Default");
            }
        }

        private void OnGUI()
        {
            EditorGUILayout.Space(8);
            EditorGUILayout.LabelField("Importador de Avatares y Dancer Cards", EditorStyles.boldLabel);
            EditorGUILayout.HelpBox("Arrastra los archivos exportados desde LiamToolAvatar2Unity (JSON y Atlas PNG) para construir el avatar automaticamente en tu escena.", MessageType.Info);

            EditorGUILayout.Space(6);
            jsonAsset = (TextAsset)EditorGUILayout.ObjectField("Descriptor JSON (.json)", jsonAsset, typeof(TextAsset), false);
            atlasTexture = (Texture2D)EditorGUILayout.ObjectField("Textura Atlas (_Atlas.png)", atlasTexture, typeof(Texture2D), false);
            customShader = (Shader)EditorGUILayout.ObjectField("ShaderLab Shader", customShader, typeof(Shader), false);

            EditorGUILayout.Space(6);
            avatarName = EditorGUILayout.TextField("Nombre del Avatar", avatarName);
            createPrefab = EditorGUILayout.Toggle("Guardar como Prefab", createPrefab);
            if (createPrefab)
            {
                outputPrefabPath = EditorGUILayout.TextField("Ruta de salida Prefab", outputPrefabPath);
            }

            EditorGUILayout.Space(12);
            GUI.enabled = (jsonAsset != null && atlasTexture != null);
            if (GUILayout.Button("Construir Avatar en Escena", GUILayout.Height(36)))
            {
                BuildAvatarFromAssets();
            }
            GUI.enabled = true;
        }

        private void BuildAvatarFromAssets()
        {
            try
            {
                var rootObject = new GameObject(avatarName);
                Undo.RegisterCreatedObjectUndo(rootObject, "Create Liam Avatar");

                var meshFilter = rootObject.AddComponent<MeshFilter>();
                var meshRenderer = rootObject.AddComponent<MeshRenderer>();
                var controller = rootObject.AddComponent<LiamAvatarController>();

                var mat = new Material(customShader != null ? customShader : Shader.Find("Sprites/Default"));
                mat.name = avatarName + "_Material";
                mat.mainTexture = atlasTexture;

                meshRenderer.sharedMaterial = mat;

                Mesh mesh = CreateQuadMesh();
                mesh.name = avatarName + "_Mesh";
                meshFilter.sharedMesh = mesh;

                Selection.activeGameObject = rootObject;
                Debug.Log($"[LiamTool] Avatar '{avatarName}' construido exitosamente en la escena.");
            }
            catch (Exception ex)
            {
                EditorUtility.DisplayDialog("Error de Importacion", "Fallo al construir el avatar: " + ex.Message, "Aceptar");
                Debug.LogError("[LiamTool] Error al construir el avatar: " + ex);
            }
        }

        private Mesh CreateQuadMesh()
        {
            Mesh mesh = new Mesh();
            Vector3[] vertices = new Vector3[]
            {
                new Vector3(-0.5f, -0.89f, 0),
                new Vector3( 0.5f, -0.89f, 0),
                new Vector3(-0.5f,  0.89f, 0),
                new Vector3( 0.5f,  0.89f, 0)
            };

            Vector2[] uvs = new Vector2[]
            {
                new Vector2(0, 0),
                new Vector2(1, 0),
                new Vector2(0, 1),
                new Vector2(1, 1)
            };

            int[] triangles = new int[] { 0, 2, 1, 2, 3, 1 };

            mesh.vertices = vertices;
            mesh.uv = uvs;
            mesh.triangles = triangles;
            mesh.RecalculateNormals();
            mesh.RecalculateBounds();
            return mesh;
        }
    }
}
#endif

using System;
using System.Collections.Generic;
using UnityEngine;

namespace LiamTool.Avatar
{
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
    public class LiamAvatarController : MonoBehaviour
    {
        public enum AnimationClipType
        {
            Idle,
            Walk,
            Jump,
            Wave,
            GoldMove,
            Groove,
            Cheer,
            Custom
        }

        [Header("Configuracion de Animacion")]
        [SerializeField] private AnimationClipType currentClip = AnimationClipType.Idle;
        [SerializeField] private bool isPlaying = true;
        [SerializeField] [Range(0.1f, 5.0f)] private float playbackSpeed = 1.0f;
        [SerializeField] [Range(0.5f, 10.0f)] private float clipDuration = 2.0f;

        [Header("Dinamicas Procedurales")]
        [SerializeField] [Range(0.0f, 0.1f)] private float breathAmount = 0.025f;
        [SerializeField] [Range(0.0f, 0.2f)] private float hipSwayX = 0.035f;
        [SerializeField] [Range(0.0f, 0.2f)] private float hipBounceY = 0.025f;
        [SerializeField] private bool enableFootGrounding = true;
        [SerializeField] private bool enableSpringPhysics = true;

        [Header("Efectos Visuales (ShaderLab)")]
        [SerializeField] private bool isGoldMoveActive = false;
        [SerializeField] [Range(0.0f, 5.0f)] private float goldAuraIntensity = 2.5f;
        [SerializeField] private Color goldAuraColor = new Color(1.0f, 0.85f, 0.15f, 1.0f);

        private float normalizedTime = 0.0f;
        private Material avatarMaterial;
        private static readonly int GoldIntensityProp = Shader.PropertyToID("_GoldAuraIntensity");
        private static readonly int GoldColorProp = Shader.PropertyToID("_GoldAuraColor");

        public AnimationClipType CurrentClip
        {
            get => currentClip;
            set => currentClip = value;
        }

        public bool IsPlaying
        {
            get => isPlaying;
            set => isPlaying = value;
        }

        private void OnEnable()
        {
            InitializeMaterial();
        }

        private void Start()
        {
            InitializeMaterial();
        }

        private void InitializeMaterial()
        {
            var renderer = GetComponent<MeshRenderer>();
            if (renderer != null)
            {
                avatarMaterial = Application.isPlaying ? renderer.material : renderer.sharedMaterial;
            }
        }

        private void Update()
        {
            if (isPlaying)
            {
                float delta = (Application.isPlaying ? Time.deltaTime : 0.016f) * playbackSpeed;
                normalizedTime = (normalizedTime + (delta / Mathf.Max(0.1f, clipDuration))) % 1.0f;
            }

            ApplyProceduralMotion();
            UpdateShaderParameters();
        }

        private void ApplyProceduralMotion()
        {
            float phase = normalizedTime * Mathf.PI * 2.0f;
            float breath = Mathf.Sin(phase) * breathAmount;
            float sway = Mathf.Cos(phase) * hipSwayX;
            float bounce = Mathf.Abs(Mathf.Sin(phase)) * hipBounceY;

            if (currentClip == AnimationClipType.GoldMove)
            {
                sway *= 1.5f;
                bounce *= 1.8f;
            }
            else if (currentClip == AnimationClipType.Groove)
            {
                sway = Mathf.Sin(phase * 2.0f) * (hipSwayX * 1.6f);
                bounce = Mathf.Abs(Mathf.Cos(phase * 2.0f)) * (hipBounceY * 1.5f);
            }

            Vector3 localPos = transform.localPosition;
            localPos.x = sway;
            localPos.y = bounce;
            transform.localPosition = localPos;

            Vector3 localScale = transform.localScale;
            localScale.y = 1.0f + breath;
            transform.localScale = localScale;
        }

        private void UpdateShaderParameters()
        {
            if (avatarMaterial == null) InitializeMaterial();
            if (avatarMaterial == null) return;

            float targetIntensity = (isGoldMoveActive || currentClip == AnimationClipType.GoldMove)
                ? goldAuraIntensity
                : 0.0f;

            if (avatarMaterial.HasProperty(GoldIntensityProp))
            {
                avatarMaterial.SetFloat(GoldIntensityProp, targetIntensity);
            }
            if (avatarMaterial.HasProperty(GoldColorProp))
            {
                avatarMaterial.SetColor(GoldColorProp, goldAuraColor);
            }
        }

        public void TriggerGoldMove(float durationSeconds = 3.0f)
        {
            isGoldMoveActive = true;
            if (Application.isPlaying)
            {
                CancelInvoke(nameof(StopGoldMove));
                Invoke(nameof(StopGoldMove), durationSeconds);
            }
        }

        public void StopGoldMove()
        {
            isGoldMoveActive = false;
        }
    }
}

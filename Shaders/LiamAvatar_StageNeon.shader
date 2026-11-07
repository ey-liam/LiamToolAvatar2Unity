Shader "LiamTool/Avatar_StageNeon"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _NeonColor ("Neon Color", Color) = (0.9, 0.2, 0.8, 1.0)
        _GlowIntensity ("Glow Intensity", Range(0.0, 4.0)) = 1.5
        _PulseFrequency ("Pulse Frequency", Range(0.1, 5.0)) = 1.2
        _ScanlineDensity ("Scanline Density", Range(0.0, 50.0)) = 18.0
        _ChromaticAberration ("Chromatic Aberration", Range(0.0, 0.05)) = 0.008
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "IgnoreProjector" = "True"
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _NeonColor;
            float _GlowIntensity;
            float _PulseFrequency;
            float _ScanlineDensity;
            float _ChromaticAberration;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float ca = _ChromaticAberration;
                float r = tex2D(_MainTex, i.uv + float2(ca, 0)).r;
                float g = tex2D(_MainTex, i.uv).g;
                float b = tex2D(_MainTex, i.uv - float2(ca, 0)).b;
                float a = tex2D(_MainTex, i.uv).a;

                float pulse = 0.85 + 0.15 * sin(_Time.y * _PulseFrequency * 6.28318);
                float scanline = 0.95 + 0.05 * sin(i.uv.y * _ScanlineDensity * 6.28318);

                float3 baseCol = float3(r, g, b);
                float3 neonGlow = _NeonColor.rgb * _GlowIntensity * pulse;

                fixed4 finalCol;
                finalCol.rgb = (baseCol * neonGlow) * scanline;
                finalCol.a = a;

                return finalCol;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Cutout/VertexLit"
}

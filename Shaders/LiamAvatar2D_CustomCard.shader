Shader "LiamTool/Avatar2D_CustomCard"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Atlas / Texture", 2D) = "white" {}
        _Color ("Main Tint", Color) = (1, 1, 1, 1)

        [Header(Gold Move Aura)]
        _GoldAuraColor ("Gold Move Color", Color) = (1.0, 0.84, 0.15, 1.0)
        _GoldAuraIntensity ("Gold Move Intensity", Range(0.0, 5.0)) = 0.0
        _GoldPulseSpeed ("Gold Pulse Speed", Range(0.0, 10.0)) = 3.0

        [Header(Coach Outline)]
        _OutlineColor ("Coach Outline Color", Color) = (0.2, 0.85, 1.0, 1.0)
        _OutlineWidth ("Outline Width", Range(0.0, 0.04)) = 0.0

        [Header(Color Grading)]
        _HueShift ("Hue Shift (Degrees)", Range(0.0, 360.0)) = 0.0
        _Brightness ("Brightness", Range(0.0, 3.0)) = 1.0
        _Contrast ("Contrast", Range(0.1, 3.0)) = 1.0
        _Saturation ("Saturation", Range(0.0, 3.0)) = 1.0

        [Header(Alpha and Blending)]
        _Cutoff ("Alpha Cutoff", Range(0.0, 1.0)) = 0.01
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
            fixed4 _Color;
            fixed4 _GoldAuraColor;
            float _GoldAuraIntensity;
            float _GoldPulseSpeed;
            fixed4 _OutlineColor;
            float _OutlineWidth;
            float _HueShift;
            float _Brightness;
            float _Contrast;
            float _Saturation;
            float _Cutoff;

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = TRANSFORM_TEX(IN.texcoord, _MainTex);
                OUT.color = IN.color * _Color;
                OUT.screenPos = ComputeScreenPos(OUT.vertex);
                return OUT;
            }

            float3 rgbToHsv(float3 c)
            {
                float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
                float4 p = lerp(float4(c.bg, K.wz), float4(c.gb, K.xy), step(c.b, c.g));
                float4 q = lerp(float4(p.xyw, c.r), float4(c.r, p.yzx), step(p.x, c.r));
                float d = q.x - min(q.w, q.y);
                float e = 1.0e-10;
                return float3(abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
            }

            float3 hsvToRgb(float3 c)
            {
                float4 K = float4(1.0, 2.0 / 3.0, 1.0 / 3.0, 3.0);
                float3 p = abs(frac(c.xxx + K.xyz) * 6.0 - K.www);
                return c.z * lerp(K.xxx, saturate(p - K.xxx), c.y);
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, IN.texcoord) * IN.color;

                if (col.a <= _Cutoff)
                {
                    if (_OutlineWidth > 0.001)
                    {
                        float2 offsets[8] = {
                            float2(-1,  0), float2(1,  0), float2(0, -1), float2(0,  1),
                            float2(-1, -1), float2(1, -1), float2(-1,  1), float2(1,  1)
                        };

                        float outlineAlpha = 0.0;
                        for (int i = 0; i < 8; i++)
                        {
                            float2 sampleUV = IN.texcoord + offsets[i] * _MainTex_TexelSize.xy * (_OutlineWidth * 100.0);
                            float a = tex2D(_MainTex, sampleUV).a;
                            outlineAlpha = max(outlineAlpha, a);
                        }

                        if (outlineAlpha > 0.15)
                        {
                            return fixed4(_OutlineColor.rgb, outlineAlpha * _OutlineColor.a);
                        }
                    }
                    discard;
                }

                if (_HueShift > 0.001 || _Saturation != 1.0 || _Brightness != 1.0 || _Contrast != 1.0)
                {
                    float3 hsv = rgbToHsv(col.rgb);
                    hsv.x = frac(hsv.x + _HueShift / 360.0);
                    hsv.y = saturate(hsv.y * _Saturation);
                    hsv.z = saturate(hsv.z * _Brightness);
                    col.rgb = hsvToRgb(hsv);

                    col.rgb = saturate(((col.rgb - 0.5) * _Contrast) + 0.5);
                }

                if (_GoldAuraIntensity > 0.01)
                {
                    float pulse = 0.75 + 0.25 * sin(_Time.y * _GoldPulseSpeed * 3.14159);
                    float edge = 1.0 - saturate(col.a);
                    float3 goldEffect = _GoldAuraColor.rgb * _GoldAuraIntensity * pulse;
                    col.rgb += goldEffect * (1.0 - col.r * 0.3);
                }

                return col;
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}

Shader "Custom/UI_ScanReveal"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _ScanColor ("Scan Color", Color) = (1,1,1,1)
        _ScanProgress ("Scan Progress", Range(0, 1)) = 0.0
        _LineWidth ("Line Width", Range(0, 0.5)) = 0.05
    }

    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        
        // UI perlukan Blend begini supaya nampak lutsinar
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex    : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f {
                float4 vertex   : SV_POSITION;
                fixed4 color    : COLOR;
                float2 texcoord : TEXCOORD0;
                float2 localPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed4 _ScanColor;
            float _ScanProgress;
            float _LineWidth;

            v2f vert(appdata_t v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = v.texcoord;
                o.color = v.color * _Color;
                // Simpan posisi local (0 hingga 1) untuk scan dari bawah ke atas
                o.localPos = v.texcoord; 
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.texcoord) * i.color;
                
                // Guna koordinat Y (0 = bawah, 1 = atas)
                float y = i.localPos.y;
                
                // 1. Bahagian yang belum kena scan (Transparent)
                if (y > _ScanProgress) {
                    col.a = 0; 
                }

                // 2. Garisan Scan (Glowing Line)
                float dist = abs(y - _ScanProgress);
                if (dist < _LineWidth && _ScanProgress > 0 && _ScanProgress < 1) {
                    float glow = 1.0 - (dist / _LineWidth);
                    col.rgb += _ScanColor.rgb * glow;
                    col.a = max(col.a, glow); // Biar garisan nampak walaupun badan belum ada
                }

                return col;
            }
            ENDCG
        }
    }
}
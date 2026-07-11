Shader "Custom/FelineScanReveal_Solid"
{
    Properties
    {
        _BaseMap ("Cat Texture", 2D) = "white" {} 
        _Color ("Scan Color", Color) = (1, 1, 1, 1)
        _PointSize ("Point Size", Float) = 0.005
        _ScanProgress ("Scan Progress", Range(0, 1)) = 0.0
        _LineWidth ("Scan Width", Float) = 0.05
        _MinY ("Min Y", Float) = -0.5
        _MaxY ("Max Y", Float) = 0.5
    }
    SubShader
    {
        // KUNCI: Kita guna Opaque balik, tapi kita paksa ZWrite On supaya dia tak nampak tembus
        Tags { "RenderType"="Opaque" "Queue"="Geometry" "RenderPipeline" = "UniversalPipeline" }
        
        Cull Off    // Supaya telinga/ekor tak nampak berlubang
        ZWrite On   // Kunci kedudukan supaya tak nampak 'dalam badan'
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma geometry geom
            #pragma target 4.0

            #include "UnityCG.cginc"

            struct v2g {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
                float yPos : TEXCOORD1;
            };

            struct g2f {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float isPoint : TEXCOORD1; 
            };

            sampler2D _BaseMap;
            float4 _BaseMap_ST;
            float _ScanProgress;
            float _LineWidth;
            float _MinY;
            float _MaxY;
            float _PointSize;
            float4 _Color;

            v2g vert (appdata_full v) {
                v2g o;
                o.pos = v.vertex;
                o.uv = TRANSFORM_TEX(v.texcoord, _BaseMap);
                o.yPos = v.vertex.y; 
                return o;
            }

            [maxvertexcount(15)]
            void geom (triangle v2g IN[3], inout TriangleStream<g2f> triStream)
            {
                float currentScanY = lerp(_MinY, _MaxY, _ScanProgress);

                // 1. LUKIS BADAN SOLID (Dah reveal)
                for (int i = 0; i < 3; i++) {
                    if (IN[i].yPos < currentScanY) {
                        g2f o;
                        o.vertex = UnityObjectToClipPos(IN[i].pos);
                        o.uv = IN[i].uv;
                        o.color = float4(1,1,1,1);
                        o.isPoint = 0.0;
                        triStream.Append(o);
                    }
                }
                triStream.RestartStrip();

                // 2. LUKIS ZARAH (Kat garisan scan)
                for (int j = 0; j < 3; j++) {
                    float dist = abs(IN[j].yPos - currentScanY);
                    if (dist < _LineWidth) {
                        float3 right = UNITY_MATRIX_V[0].xyz;
                        float3 up = UNITY_MATRIX_V[1].xyz;
                        float3 p = IN[j].pos.xyz;
                        
                        float s = _PointSize;
                        float4 corners[4];
                        corners[0] = float4(p + (right - up) * s, 1);
                        corners[1] = float4(p + (-right - up) * s, 1);
                        corners[2] = float4(p + (right + up) * s, 1);
                        corners[3] = float4(p + (-right + up) * s, 1);

                        for(int k=0; k<4; k++) {
                            g2f o;
                            o.vertex = UnityObjectToClipPos(corners[k]);
                            o.uv = float2(0,0);
                            o.color = _Color;
                            o.isPoint = 1.0;
                            triStream.Append(o);
                        }
                        triStream.RestartStrip();
                    }
                }
            }

            fixed4 frag (g2f i) : SV_Target {
                if (i.isPoint > 0.5) return i.color;
                return tex2D(_BaseMap, i.uv);
            }
            ENDCG
        }
    }
}
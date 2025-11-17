Shader "Custom/StencilMasked"
{
    Properties {
        _Color("Tint Color", Color) = (0.5, 0.5, 0.5, 0.1)   // grey, 50% transparent
    }

    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }

        ZWrite Off                // typical for transparent objects
        Blend SrcAlpha OneMinusSrcAlpha

        Stencil {
            Ref 1
            Comp NotEqual          // don't draw where mask wrote
        }

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            sampler2D _MainTex;
            fixed4 _Color;

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                fixed4 texCol = tex2D(_MainTex, i.uv);
                return texCol * _Color;  // apply tint + transparency
            }
            ENDCG
        }
    }
}
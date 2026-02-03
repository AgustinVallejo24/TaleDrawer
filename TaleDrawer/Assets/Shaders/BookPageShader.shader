Shader "Unlit/BookPage_BackToBasics"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 0
    }
        SubShader
        {
            Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}

            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            Cull[_Cull]

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma target 3.0
                #include "UnityCG.cginc"

                struct appdata {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                    fixed4 color : COLOR;
                };

                struct v2f {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    fixed4 color : COLOR;
                };

                sampler2D _MainTex;
                float4 _MainTex_ST;

                v2f vert(appdata v) {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                    o.color = v.color;
                    return o;
                }

                fixed4 frag(v2f i, float facing : VFACE) : SV_Target {
                    float2 uv = i.uv;

                    // Solo invertimos si detectamos CLARAMENTE que es el dorso (valor negativo)
                    // El frente (valor positivo) se queda con la UV original de Unity
                    if (facing < 0) {
                        uv.x = 1.0 - uv.x;
                    }

                    return tex2D(_MainTex, uv) * i.color;
                }
                ENDCG
            }
        }
}
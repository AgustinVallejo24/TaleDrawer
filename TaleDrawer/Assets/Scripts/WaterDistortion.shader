Shader "Custom/SpriteBackgroundBlurURP"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (1,1,1,1)

            // Blur
            _BlurSize("Blur Size", Range(0, 8)) = 1.5
            _BlurAmount("Blur Amount", Range(0,1)) = 0.9

            [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
            [HideInInspector] _RendererColor("RendererColor", Color) = (1,1,1,1)
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
                "RenderPipeline" = "UniversalPipeline"
            }

            Cull Off
            Lighting Off
            ZWrite Off
            Blend One OneMinusSrcAlpha

            Pass
            {
                HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ PIXELSNAP_ON
                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                struct Attributes
                {
                    float4 positionOS   : POSITION;
                    float4 color        : COLOR;
                    float2 uv           : TEXCOORD0;
                };

                struct Varyings
                {
                    float4 positionCS   : SV_POSITION;
                    float4 color        : COLOR;
                    float2 uv           : TEXCOORD0;
                    float4 screenPos    : TEXCOORD1;
                };

                TEXTURE2D(_MainTex);
                SAMPLER(sampler_MainTex);

                // ¡Esto es lo que faltaba!
                TEXTURE2D_X(_CameraOpaqueTexture);
                SAMPLER(sampler_CameraOpaqueTexture);   // Declara el sampler explícitamente

                float4 _MainTex_ST;
                float4 _Color;
                float _BlurSize;
                float _BlurAmount;
                float4 _RendererColor;

                Varyings vert(Attributes IN)
                {
                    Varyings OUT;
                    OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                    OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);
                    OUT.color = IN.color * _Color * _RendererColor;

                    OUT.screenPos = ComputeScreenPos(OUT.positionCS);

                    #ifdef PIXELSNAP_ON
                    OUT.positionCS.xy = UnityPixelSnap(OUT.positionCS.xy);
                    #endif

                    return OUT;
                }

                // Box blur simple y barato (3x3)
                half4 Blur(float2 uv)
                {
                    float2 texelSize = 1.0 / _ScreenParams.xy;
                    half4 sum = 0;

                    // 3x3 box (9 muestras)
                    for (int x = -1; x <= 1; x++)
                    {
                        for (int y = -1; y <= 1; y++)
                        {
                            float2 offset = float2(x, y) * texelSize * _BlurSize;
                            // Usa SAMPLE_TEXTURE2D_X para compatibilidad URP
                            half4 col = SAMPLE_TEXTURE2D_X(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, UnityStereoTransformScreenSpaceTex(uv + offset));
                            sum += col;
                        }
                    }

                    return sum / 9.0;
                }

                half4 frag(Varyings IN) : SV_Target
                {
                    // UV de pantalla (ya viene preparado por ComputeScreenPos)
                    float2 screenUV = IN.screenPos.xy / IN.screenPos.w;

                    // Fondo borroso
                    half4 blurredBG = Blur(screenUV);

                    // Sprite normal
                    half4 sprite = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv) * IN.color;

                    // Mezcla: más sprite donde hay opacidad, más blur donde es transparente
                    half4 finalColor = lerp(blurredBG, sprite, sprite.a * _BlurAmount);

                    // Premultiplied alpha (estándar para sprites transparentes)
                    finalColor.rgb *= finalColor.a;

                    return finalColor;
                }
                ENDHLSL
            }
        }
}
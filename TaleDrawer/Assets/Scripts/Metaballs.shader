Shader "Custom/MetaballsLiquidURP"
{
    Properties
    {
        _Threshold("Threshold", Float) = 1
        _Color("Color", Color) = (0,0.5,1,1)
        _Alpha("Alpha", Range(0,1)) = 1

        _LiquidTex("Liquid Texture", 2D) = "white" {}

        _NoiseStrength("Noise Strength", Float) = 0.02
        _WaveStrength("Wave Strength", Float) = 0.01
        _SurfaceTension("Surface Tension", Float) = 0.1
        _FlattenStrength("Flatten Strength", Float) = 0.15

        _RefractionStrength("Refraction Strength", Range(0,0.2)) = 0.05
    }

        SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
                "RenderPipeline" = "UniversalPipeline"
            }

            Pass
            {
                Blend SrcAlpha OneMinusSrcAlpha
                ZWrite Off

                HLSLPROGRAM

                #pragma vertex vert
                #pragma fragment frag

                #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

                #define MAX_BALLS 512

                struct Attributes
                {
                    float4 vertex : POSITION;
                };

                struct Varyings
                {
                    float4 pos : SV_POSITION;
                    float2 worldPos : TEXCOORD0;
                };

                int _BallCount;
                float4 _Balls[MAX_BALLS];

                float _Threshold;
                float4 _Color;
                float _Alpha;

                float _NoiseStrength;
                float _WaveStrength;
                float _SurfaceTension;
                float _FlattenStrength;
                float _RefractionStrength;

                TEXTURE2D(_LiquidTex);
                SAMPLER(sampler_LiquidTex);

                TEXTURE2D(_SceneColorTexture);
                SAMPLER(sampler_SceneColorTexture);

                float hash(float2 p)
                {
                    return frac(sin(dot(p,float2(127.1,311.7))) * 43758.5453);
                }

                float noise(float2 p)
                {
                    float2 i = floor(p);
                    float2 f = frac(p);

                    float a = hash(i);
                    float b = hash(i + float2(1,0));
                    float c = hash(i + float2(0,1));
                    float d = hash(i + float2(1,1));

                    float2 u = f * f * (3 - 2 * f);

                    return lerp(a,b,u.x)
                         + (c - a) * u.y * (1 - u.x)
                         + (d - b) * u.x * u.y;
                }

                Varyings vert(Attributes v)
                {
                    Varyings o;

                    float4 world = mul(UNITY_MATRIX_M, v.vertex);

                    o.pos = TransformObjectToHClip(v.vertex.xyz);
                    o.worldPos = world.xy;

                    return o;
                }

                float metaball(float2 p)
                {
                    float sum = 0;

                    for (int i = 0; i < _BallCount; i++)
                    {
                        float2 bpos = _Balls[i].xy;
                        float r = _Balls[i].z * 1.2;

                        float2 diff = p - bpos;
                        float d = length(diff);

                        sum += pow(r,4) / (pow(d,4) + 0.0001);
                    }

                    return sum;
                }

                half4 frag(Varyings i) : SV_Target
                {
                    float2 p = i.worldPos;

                    float n = noise(p * 3 + _Time.y);
                    if (abs(p.y) > 0.5) n *= 0.5;

                    p += (n - 0.5) * _NoiseStrength;

                    float v = metaball(p);

                    float wave =
                    sin(p.x * 4 + _Time.y * 3)
                    * _WaveStrength
                    * (1 - smoothstep(0.5,1.0,p.y));

                    v += wave;
                    v -= p.y * _FlattenStrength;

                    float edge =
                    smoothstep(
                        _Threshold - _SurfaceTension * 0.5,
                        _Threshold + _SurfaceTension * 0.5,
                        v
                    );

                    if (edge <= 0) discard;

                    // --------- calcular gradiente ---------

                    float eps = 0.01;

                    float dx =
                    metaball(p + float2(eps,0))
                    - metaball(p - float2(eps,0));

                    float dy =
                    metaball(p + float2(0,eps))
                    - metaball(p - float2(0,eps));

                    float2 grad = float2(dx,dy);

                    float2 refraction =
                    grad * _RefractionStrength * edge;

                    // --------- UV de pantalla correctos ---------

                    float2 screenUV =
                    i.pos.xy / _ScreenParams.xy;

                    screenUV.y = 1 - screenUV.y;

                    // --------- sample fondo ---------

                    float3 background =
                    SAMPLE_TEXTURE2D(
                        _SceneColorTexture,
                        sampler_SceneColorTexture,
                        screenUV + refraction
                    ).rgb;

                    // --------- textura liquido ---------



                    //float3 finalColor =
                    //lerp(background, liquidTex * _Color.rgb, edge);

                    float t = _Time.y * 2;
                    float value = lerp(.003, .005, (sin(t) * 0.5 + 0.5));

                    screenUV.y = (1 + value) - screenUV.y;
                    screenUV.x = screenUV.x + value;
                    float3 liquidTex =
                        SAMPLE_TEXTURE2D(
                            _LiquidTex,
                            sampler_LiquidTex,
                            p * 0.2 + _Time.y * 0.02
                        ).rgb;
                    return float4(
                        SAMPLE_TEXTURE2D(
                            _SceneColorTexture,
                            sampler_SceneColorTexture,
                            screenUV
                        ).rgb + liquidTex,
                        _Alpha);
                }

                ENDHLSL
            }
        }
}
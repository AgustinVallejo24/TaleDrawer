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

        _RefractionStrength("Refraction Strength", Range(0,1)) = 0.05
        _TextureMovement("Texture Movement", Range(0,1)) = 1

            // -------- OVERLAY --------
            _OverlayTex("Overlay Texture", 2D) = "black" {}
            _OverlayFill("Overlay Fill", Range(0,1)) = 0

                // -------- WORLD HEIGHT --------
                _LiquidMinY("Liquid Min Y", Float) = -1
                _LiquidMaxY("Liquid Max Y", Float) = 1

                // -------- EDGE NOISE --------
                _NoiseScaleEdge("Noise Scale Edge", Float) = 3
                _NoiseStrengthEdge("Noise Strength Edge", Float) = 0.15
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
                    float _TextureMovement;

                    TEXTURE2D(_LiquidTex);
                    SAMPLER(sampler_LiquidTex);

                    TEXTURE2D(_SceneColorTexture);
                    SAMPLER(sampler_SceneColorTexture);

                    TEXTURE2D(_OverlayTex);
                    SAMPLER(sampler_OverlayTex);

                    float _OverlayFill;

                    float _LiquidMinY;
                    float _LiquidMaxY;

                    float _NoiseScaleEdge;
                    float _NoiseStrengthEdge;

                    float4 _LiquidTex_ST;
                    float4 _OverlayTex_ST;

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
                        float2 pStatic = p;
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

                        float2 screenUV =
                        i.pos.xy / _ScreenParams.xy;

                        screenUV.y = 1 - screenUV.y;

                        float t = _Time.y * 2;
                        float value = lerp(.003, .005, (sin(t) * 0.5 + 0.5));

                        screenUV.y = (1 + value) - screenUV.y;
                        screenUV.x = screenUV.x + value;

                        float2 liquidUV = p * _LiquidTex_ST.xy + _LiquidTex_ST.zw;
                        liquidUV += _Time.y * 0.02 * _TextureMovement;

                        float3 liquidTex =
                            SAMPLE_TEXTURE2D(
                                _LiquidTex,
                                sampler_LiquidTex,
                                liquidUV
                            ).rgb;

                        float3 finalCol =
                            SAMPLE_TEXTURE2D(
                                _SceneColorTexture,
                                sampler_SceneColorTexture,
                                screenUV
                            ).rgb * _RefractionStrength + liquidTex;

                        // -------- FILL + NOISE --------

                        float height01 = saturate(
                            (p.y - _LiquidMinY) / (_LiquidMaxY - _LiquidMinY)
                        );

                        float noiseEdge = noise(p * _NoiseScaleEdge + _Time.y/2);
                        noiseEdge = (noiseEdge - 0.5) * _NoiseStrengthEdge;

                        height01 += noiseEdge;

                        float mask = step(1.0 - _OverlayFill, height01);

                        float2 overlayUV = pStatic * _OverlayTex_ST.xy + _OverlayTex_ST.zw;

                        float4 overlaySample =
                            SAMPLE_TEXTURE2D(
                                _OverlayTex,
                                sampler_OverlayTex,
                                overlayUV
                            );

                        finalCol = lerp(finalCol, overlaySample.rgb, overlaySample.a * mask);

                        return float4(finalCol, _Alpha);
                    }

                    ENDHLSL
                }
            }
}
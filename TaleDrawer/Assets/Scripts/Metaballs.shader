Shader "Custom/Metaballs"
{
    Properties
    {
        _Threshold("Threshold", Float) = 1
        _Color("Color", Color) = (0,0.5,1,1)
        _Alpha("Alpha", Range(0,1)) = 1
        _NoiseStrength("Noise Strength", Float) = 0.02  // Reduced for flatter surface
        _WaveStrength("Wave Strength", Float) = 0.01   // Reduced for less waviness
        _SurfaceTension("Surface Tension", Float) = 0.1 // Tightened for sharper edges
        _FlattenStrength("Flatten Strength", Float) = 0.15 // New: Strength of vertical flattening bias
    }
        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
            #define MAX_BALLS 512
            struct appdata
            {
                float4 vertex : POSITION;
            };
            struct v2f
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
            float _FlattenStrength; // New property

            float hash(float2 p)
            {
                return frac(sin(dot(p, float2(127.1,311.7))) * 43758.5453);
            }
            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);
                float a = hash(i);
                float b = hash(i + float2(1,0));
                float c = hash(i + float2(0,1));
                float d = hash(i + float2(1,1));
                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a,b,u.x) + (c - a) * u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }
            v2f vert(appdata v)
            {
                v2f o;
                float4 world = mul(unity_ObjectToWorld, v.vertex);
                o.pos = UnityObjectToClipPos(v.vertex);
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
                    // Modified: Higher power falloff (d^4) for sharper, less rounded blending -> flatter surfaces
                    sum += pow(r, 4) / (pow(d, 4) + 0.0001);
                }
                return sum;
            }
            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = i.worldPos;

                // Apply noise only subtly and conditionally (e.g., stronger near edges, less on flat areas)
                float n = noise(p * 3 + _Time.y);
                if (abs(p.y) > 0.5) { // Example condition: Reduce noise at higher y (top surface)
                    n *= 0.5;
                }
                p += (n - 0.5) * _NoiseStrength;

                float v = metaball(p);

                // Modified wave: Reduced and applied only horizontally with damping at top
                float wave = sin(p.x * 4 + _Time.y * 3) * _WaveStrength * (1 - smoothstep(0.5, 1.0, p.y)); // Dampen wave at higher y
                v += wave;

                // New: Add vertical flattening bias (negative gradient to pull top surface down/flat)
                v -= p.y * _FlattenStrength; // Tune _FlattenStrength; positive y assumes up is positive

                // Tightened smoothstep for crisper, flatter edges
                float edge = smoothstep(_Threshold - _SurfaceTension * 0.5,
                                        _Threshold + _SurfaceTension * 0.5,
                                        v);
                if (edge <= 0)
                    discard;

                return float4(_Color.rgb, edge * _Alpha);
            }
            ENDCG
        }
    }
}
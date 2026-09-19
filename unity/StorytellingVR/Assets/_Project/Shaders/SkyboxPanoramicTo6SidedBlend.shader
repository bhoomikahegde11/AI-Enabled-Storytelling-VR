Shader "Custom/Skybox/PanoramicTo6SidedBlend"
{
    Properties
    {
        [NoScaleOffset] _DayTex ("Day Panoramic Texture", 2D) = "grey" {}
        _DayTint ("Day Tint", Color) = (0.49826992, 0.5139482, 0.5294118, 1)
        _DayExposure ("Day Exposure", Range(0, 8)) = 0.95
        _DayRotation ("Day Rotation", Range(0, 360)) = 0

        [NoScaleOffset] _EveningFrontTex ("Evening Front (+Z)", 2D) = "grey" {}
        [NoScaleOffset] _EveningBackTex ("Evening Back (-Z)", 2D) = "grey" {}
        [NoScaleOffset] _EveningLeftTex ("Evening Left (-X)", 2D) = "grey" {}
        [NoScaleOffset] _EveningRightTex ("Evening Right (+X)", 2D) = "grey" {}
        [NoScaleOffset] _EveningUpTex ("Evening Up (+Y)", 2D) = "grey" {}
        [NoScaleOffset] _EveningDownTex ("Evening Down (-Y)", 2D) = "grey" {}
        _EveningTint ("Evening Tint", Color) = (0.5, 0.5, 0.5, 1)
        _EveningExposure ("Evening Exposure", Range(0, 8)) = 0.95
        _EveningRotation ("Evening Rotation", Range(0, 360)) = 0

        _Blend ("Blend", Range(0, 1)) = 0
    }

    SubShader
    {
        Tags { "Queue" = "Background" "RenderType" = "Background" "PreviewType" = "Skybox" "RenderPipeline" = "UniversalPipeline" }
        Cull Off
        ZWrite Off
        ZTest LEqual

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _DayTex;
            float4 _DayTex_ST;
            float4 _DayTint;
            float _DayExposure;
            float _DayRotation;

            sampler2D _EveningFrontTex;
            sampler2D _EveningBackTex;
            sampler2D _EveningLeftTex;
            sampler2D _EveningRightTex;
            sampler2D _EveningUpTex;
            sampler2D _EveningDownTex;
            float4 _EveningTint;
            float _EveningExposure;
            float _EveningRotation;

            float _Blend;

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 viewDir : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.vertex.z = o.vertex.w;
                o.viewDir = v.vertex.xyz;
                return o;
            }

            float3 RotateAroundY(float3 dir, float degrees)
            {
                float radians = degrees * UNITY_PI / 180.0;
                float s = sin(radians);
                float c = cos(radians);
                return float3(
                    c * dir.x + s * dir.z,
                    dir.y,
                    -s * dir.x + c * dir.z
                );
            }

            float2 DirectionToPanoramicUV(float3 dir)
            {
                dir = normalize(dir);
                float2 sampleUV;
                sampleUV.x = atan2(dir.x, dir.z) * (0.5 / UNITY_PI) + 0.5;
                sampleUV.y = 0.5 - (asin(dir.y) / UNITY_PI);
                return sampleUV;
            }

            float4 SamplePanoramicSky(float3 dir)
            {
                float3 rotatedDir = RotateAroundY(normalize(dir), _DayRotation);
                float2 uv = DirectionToPanoramicUV(rotatedDir);
                float3 color = tex2D(_DayTex, uv).rgb * _DayTint.rgb * _DayExposure;
                return float4(color, 1.0);
            }

            float4 SampleSixSidedSky(float3 dir)
            {
                float3 rotatedDir = RotateAroundY(normalize(dir), _EveningRotation);
                float3 absDir = abs(rotatedDir);
                float2 uv;
                float3 color;

                if (absDir.x >= absDir.y && absDir.x >= absDir.z)
                {
                    float inv = 0.5 / absDir.x;
                    if (rotatedDir.x >= 0.0)
                    {
                        uv = float2(-rotatedDir.z, rotatedDir.y) * inv + 0.5;
                        color = tex2D(_EveningRightTex, uv).rgb;
                    }
                    else
                    {
                        uv = float2(rotatedDir.z, rotatedDir.y) * inv + 0.5;
                        color = tex2D(_EveningLeftTex, uv).rgb;
                    }
                }
                else if (absDir.y >= absDir.x && absDir.y >= absDir.z)
                {
                    float inv = 0.5 / absDir.y;
                    if (rotatedDir.y >= 0.0)
                    {
                        uv = float2(rotatedDir.x, -rotatedDir.z) * inv + 0.5;
                        color = tex2D(_EveningUpTex, uv).rgb;
                    }
                    else
                    {
                        uv = float2(rotatedDir.x, rotatedDir.z) * inv + 0.5;
                        color = tex2D(_EveningDownTex, uv).rgb;
                    }
                }
                else
                {
                    float inv = 0.5 / absDir.z;
                    if (rotatedDir.z >= 0.0)
                    {
                        uv = float2(rotatedDir.x, rotatedDir.y) * inv + 0.5;
                        color = tex2D(_EveningFrontTex, uv).rgb;
                    }
                    else
                    {
                        uv = float2(-rotatedDir.x, rotatedDir.y) * inv + 0.5;
                        color = tex2D(_EveningBackTex, uv).rgb;
                    }
                }

                color *= _EveningTint.rgb * _EveningExposure;
                return float4(color, 1.0);
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float3 dir = normalize(i.viewDir);
                float3 dayColor = SamplePanoramicSky(dir).rgb;
                float3 eveningColor = SampleSixSidedSky(dir).rgb;
                return float4(lerp(dayColor, eveningColor, saturate(_Blend)), 1.0);
            }
            ENDCG
        }
    }

    FallBack Off
}

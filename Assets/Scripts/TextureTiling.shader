Shader "Custom/TriplanarPBR"
{
    Properties
    {
        _BaseMap ("Base Map", 2D) = "white" {}
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _MetallicMap ("Metallic (R)", 2D) = "black" {}
        _OcclusionMap ("Occlusion (G)", 2D) = "white" {}
        _HeightMap ("Height (R)", 2D) = "black" {}

        _Tiling ("Tiling", Float) = 1
        _NormalStrength ("Normal Strength", Float) = 1
        _HeightStrength ("Height Strength", Float) = 0.1
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityStandardUtils.cginc"

            sampler2D _BaseMap;
            sampler2D _NormalMap;
            sampler2D _MetallicMap;
            sampler2D _OcclusionMap;
            sampler2D _HeightMap;

            float _Tiling;
            float _NormalStrength;
            float _HeightStrength;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                UNITY_FOG_COORDS(2)
            };

            v2f vert (appdata v)
            {
                v2f o;

                o.pos = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);

                UNITY_TRANSFER_FOG(o, o.pos);

                return o;
            }

            float4 SampleTriplanar(sampler2D tex, float3 worldPos, float3 normal)
            {
                float3 blend = abs(normal);
                blend = normalize(max(blend, 0.0001));

                float2 xUV = worldPos.zy * _Tiling;
                float2 yUV = worldPos.xz * _Tiling;
                float2 zUV = worldPos.xy * _Tiling;

                float4 x = tex2D(tex, xUV);
                float4 y = tex2D(tex, yUV);
                float4 z = tex2D(tex, zUV);

                return x * blend.x + y * blend.y + z * blend.z;
            }

            float3 SampleNormalTriplanar(sampler2D tex, float3 worldPos, float3 normal)
            {
                float3 blend = abs(normal);
                blend = normalize(max(blend, 0.0001));

                float2 xUV = worldPos.zy * _Tiling;
                float2 yUV = worldPos.xz * _Tiling;
                float2 zUV = worldPos.xy * _Tiling;

                float3 nx = UnpackNormal(tex2D(tex, xUV));
                float3 ny = UnpackNormal(tex2D(tex, yUV));
                float3 nz = UnpackNormal(tex2D(tex, zUV));

                float3 n = nx * blend.x + ny * blend.y + nz * blend.z;

                return normalize(n);
            }

            float4 frag (v2f i) : SV_Target
            {
                float3 worldPos = i.worldPos;
                float3 worldNormal = normalize(i.worldNormal);

                float4 albedo = SampleTriplanar(_BaseMap, worldPos, worldNormal);

                float3 normal = SampleNormalTriplanar(_NormalMap, worldPos, worldNormal);
                normal = lerp(float3(0,0,1), normal, _NormalStrength);

                float ao = SampleTriplanar(_OcclusionMap, worldPos, worldNormal).g;

                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                float NdotL = saturate(dot(normal, lightDir));

                float3 color = albedo.rgb * (NdotL * ao + 0.2);

                float4 finalColor = float4(color, albedo.a);

                UNITY_APPLY_FOG(i.fogCoord, finalColor);

                return finalColor;
            }

            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags { "LightMode" = "ShadowCaster" }

            HLSLPROGRAM
            #pragma vertex vertShadow
            #pragma fragment fragShadow

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            v2f vertShadow(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            float4 fragShadow(v2f i) : SV_Target
            {
                return 0;
            }

            ENDHLSL
        }
    }
}
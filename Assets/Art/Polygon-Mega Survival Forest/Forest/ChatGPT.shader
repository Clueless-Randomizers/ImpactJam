Shader "Custom/SurfaceShaderFoliageURP"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Lambert90deg("Lambert90deg", Range(0,1)) = 0.6
        _Lambert180deg("Lambert180deg", Range(0,1)) = 0.8
        _AmbientAmount("AmbientAmount", Range(0,1)) = 0.5
        _Ambient0deg("Ambient0deg", Range(0,1)) = 0.0
        _Ambient90deg("Ambient90deg", Range(0,1)) = 0.5
        _Ambient180deg("Ambient180deg", Range(0,1)) = 1.0
        _OffsetAmbient("OffsetAmbient", Range(-1,1)) = -0.1
        _Cutoff("Transparency (Light transmission)", Range(0.01,1)) = 0.5
        _AlphaOffset("Alpha offset", Range(-1,1)) = 0.1
        _WindSpeed("WindSpeed", Range(0,1)) = 1.0
        _WindStrength("WindStrength", Range(0,1)) = 1.0
    }
    SubShader
    {
        Tags {
            "RenderType" = "TransparentCutout"
            "Queue" = "AlphaTest"
            "IgnoreProjector" = "True"
            "RenderPipeline" = "UniversalPipeline"
        }

        LOD 200
        AlphaToMask On
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include <UnityCG.cginc>
            #include <UnityShaderUtilities.cginc>

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)
            float4 _Color;
            float4 _ColorLighted;
            float _Lambert90deg;
            float _Lambert180deg;
            float _OffsetAmbient;
            float _AmbientAmount;
            float _Ambient0deg;
            float _Ambient90deg;
            float _Ambient180deg;
            float _AlphaOffset;
            float _Glossiness;
            sampler2D _MainTex;
            float4 windAnimation;
            float _WindSpeed;
            float _WindStrength;
            CBUFFER_END

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                float3 normal : NORMAL;
            };

            v2f vert(appdata_full v)
            {
                v2f o;
                float4 modelPos = UnityObjectToClipPos(v.vertex);
                windAnimation = UnityObjectToClipPos(v.vertex + v.normal * _WindSpeed * _Time.y + v.tangent * _WindSpeed * _Time.x);
                o.vertex = ComputeGrabScreenPos(modelPos);
                o.uv = v.texcoord;
                o.uv2 = v.texcoord2;
                o.normal = mul(unity_WorldToObject, v.normal);
                return o;
            }

            vector<double, 3> frag(v2f IN) : SV_Target
            {
                float4 c = tex2D(_MainTex, IN.uv);
                float4 c = tex2D(_MainTex, IN.uv + windAnimation.xy * _WindStrength);
                vector<double, 3> output = c.rgb * _Color.rgb * (1.0 / 8.0);
                output.a = c.a * _Color.a + _AlphaOffset;
                return output;
            }
            ENDHLSL
        }
    }
    FallBack "Universal Render Pipeline/Lit"
}
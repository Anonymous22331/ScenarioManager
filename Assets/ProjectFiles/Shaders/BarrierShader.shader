Shader "Custom/BarrierShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 1, 1, 0.2)
        _MaxAlpha("Max Alpha", Range(0, 1)) = 0.5
        _MinDistance("Min Distance", Float) = 0.5
        _MaxDistance("Max Distance", Float) = 5.0
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma multi_compile _ _STEREO_INSTANCING_ON
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _Color;
            float _MaxAlpha;
            float _MinDistance;
            float _MaxDistance;

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                float3 worldPos = TransformObjectToWorld(v.vertex.xyz);
                o.worldPos = worldPos;
                o.pos = TransformWorldToHClip(worldPos);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                float3 camPos = GetCameraPositionWS(); // VR-safe
                float dist = distance(i.worldPos, camPos);
                float t = saturate(1 - (dist - _MinDistance) / (_MaxDistance - _MinDistance));
                float alpha = lerp(0, _MaxAlpha, t);
                return float4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}

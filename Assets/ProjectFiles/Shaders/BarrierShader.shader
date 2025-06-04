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
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float3 worldPos : TEXCOORD0;
            };

            float4 _Color;
            float _MaxAlpha;
            float _MinDistance;
            float _MaxDistance;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex.xyz);
                o.worldPos = TransformObjectToWorld(v.vertex.xyz);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                float dist = distance(i.worldPos, _WorldSpaceCameraPos);
                float t = saturate(1 - (dist - _MinDistance) / (_MaxDistance - _MinDistance));
                float alpha = lerp(0, _MaxAlpha, t);
                return float4(_Color.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
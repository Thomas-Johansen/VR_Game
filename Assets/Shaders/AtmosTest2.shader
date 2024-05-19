Shader "Custom/AtmosTest2"
{
    Properties
    {
        _AtmosphereColor ("Atmosphere Color", Color) = (0.5,0.5,1.0,0.5)
        _AtmosphereDensity ("Atmosphere Density", Range (0.0, 1.0)) = 0.1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front // Cull front faces to render the inside
        Lighting Off
        ZWrite Off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            float4 _AtmosphereColor;
            float _AtmosphereDensity;
            //float3 _WorldSpaceCameraPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(worldPos - _WorldSpaceCameraPos);

                float distance = length(worldPos - _WorldSpaceCameraPos);
                float intensity = exp(-_AtmosphereDensity * distance);

                o.color = _AtmosphereColor * intensity;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return i.color;
            }
            ENDCG
        }
    }
    FallBack "Transparent/Diffuse"
}
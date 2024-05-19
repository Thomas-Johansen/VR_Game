Shader "Custom/AtmosTest"
{
    Properties
    {
        _RimColor ("Rim Color", Color) = (0.5,0.5,1.0,0.5)
        _RimPower ("Rim Power", Range (0.5, 8.0)) = 3.0
        _AtmosphereIntensity ("Atmosphere Intensity", Range(0.0, 1.0)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        Cull Front
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float4 color : COLOR;
            };

            float4 _RimColor;
            float _RimPower;
            float _AtmosphereIntensity;
            //float3 _WorldSpaceCameraPos;

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);

                float3 worldNormal = normalize(mul((float3x3)unity_ObjectToWorld, v.normal));
                float3 worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

                float rim = 1.0 - saturate(dot(worldNormal, viewDir));
                float atmosphere = pow(rim, _RimPower);
                float4 rimColor = _RimColor * atmosphere * _AtmosphereIntensity;

                o.color = rimColor;

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

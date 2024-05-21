Shader "MyShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    	_planetCentre ("centre", Vector) = (0,0,0)
    	_planetRadius ("planet radius", Float) = 100.0
    	_atmosphereRadius ("atmo radius", Float) = 100.0
    	_numInScatteringPoints ("scatter points", Int) = 1
    	_numOpticalDepthPoints ("optical depth points", Int) = 1
    	_lightDirection ("Light Direction", Vector) = (0, -1, 0)
    	_densityFalloff ("Density Falloff", Float) = 1.0
    }
    SubShader
    {
    	Cull Off ZWrite Off ZTest Always
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            float3 _planetCentre;
            float _planetRadius;
            float _atmosphereRadius;
            int _numInScatteringPoints;
            int _numOpticalDepthPoints;
            float3 _lightDirection;
            float _densityFalloff;

            #include "UnityCG.cginc"
            #include "Math.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };
            
            sampler2D _BlueNoise;
			sampler2D _MainTex;
			sampler2D _BakedOpticalDepth;
			sampler2D _CameraDepthTexture;
            float4 _MainTex_ST;

            struct v2f {
					float4 pos : SV_POSITION;
					float2 uv : TEXCOORD0;
					float3 viewVector : TEXCOORD1;
			};

			v2f vert (appdata v) {
					v2f output;
					output.pos = UnityObjectToClipPos(v.vertex);
					output.uv = v.uv;
					// Camera space matches OpenGL convention where cam forward is -z. In unity forward is positive z.
					// (https://docs.unity3d.com/ScriptReference/Camera-cameraToWorldMatrix.html)
					float3 viewVector = mul(unity_CameraInvProjection, float4(v.uv.xy * 2 - 1, 0, -1));
					output.viewVector = mul(unity_CameraToWorld, float4(viewVector,0));
					return output;
			}

            float densityAtPoint(float3 densitySamplePoint) {
				float heightAboveSurface = length(densitySamplePoint - _planetCentre) - _planetRadius;
				float height01 = heightAboveSurface / (_atmosphereRadius - _planetRadius);
				float localDensity = exp(-height01 * _densityFalloff) * (1 - height01);
				return localDensity;
			}

            float opticalDepth(float3 rayOrigin, float3 rayDir, float rayLength) {
				float3 densitySamplePoint = rayOrigin;
				float stepSize = rayLength / (_numOpticalDepthPoints - 1);
				float opticalDepth = 0;

				for (int i = 0; i < _numOpticalDepthPoints; i ++) {
					float localDensity = densityAtPoint(densitySamplePoint);
					opticalDepth += localDensity * stepSize;
					densitySamplePoint += rayDir * stepSize;
				}
				return opticalDepth;
			}


            float calculateLight(float3 rayOrigin, float3 rayDir, float rayLength)
			{
				float3 inScatterPoint = rayOrigin;
				float stepSize = rayLength / (_numInScatteringPoints - 1);
				float inScatteredLight = 0;
				
				for (int i = 0; i < _numInScatteringPoints; i++)
				{
					float sunRayLength = raySphere(_planetCentre, _atmosphereRadius, inScatterPoint, _lightDirection).y;
					float sunRayOpticalDepth = opticalDepth(inScatterPoint, _lightDirection, sunRayLength);
					float viewRayOpticalDepth = opticalDepth(inScatterPoint, -rayDir, stepSize * i);
					float transmittance = exp(-(sunRayOpticalDepth + viewRayOpticalDepth));
					float localDensity = densityAtPoint(inScatterPoint);

					inScatteredLight += localDensity * transmittance * stepSize;
					inScatterPoint += rayDir * stepSize;
				}
				return inScatteredLight;
			}

            fixed4 frag (v2f i) : SV_Target
            {
            	float3 planetCentre = _planetCentre;
            	float atmosphereRadius = _atmosphereRadius;
                // sample the texture
                float4 col = tex2D(_MainTex, i.uv);
				float sceneDepthNonLinear = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);
            	float sceneDepth = LinearEyeDepth(sceneDepthNonLinear) * length(i.viewVector);
            	

                float3 rayOrigin = _WorldSpaceCameraPos;
                float3 rayDir = normalize(i.viewVector);

				float2 hitInfo = raySphere(planetCentre, atmosphereRadius, rayOrigin, rayDir);
            	float dstToAtmosphere = hitInfo.x;
            	float dstThroughAtmosphere = min(hitInfo.y, sceneDepth - dstToAtmosphere);

				//return dstThroughAtmosphere / (atmosphereRadius * 2) * float4(rayDir.rgb * 0.5 + 0.5, 0);
            	
                if (dstThroughAtmosphere > 0)
                {
                	const float epsilon = 0.0001;
	                float3 pointInAtmosphere = rayOrigin + rayDir * (dstToAtmosphere + epsilon);
                	float light = calculateLight(pointInAtmosphere, rayDir, dstThroughAtmosphere - epsilon * 2);
                	return col * (1 - light) + light;
                }
            	return col;
            }
            ENDCG
        }
    }
}

Shader "Highlighters_BuiltIn/MeshOutlineObjects"
{
 CGINCLUDE
        #include "UnityCG.cginc"
		
		sampler2D _SceneDepthMask;

		float _AdaptiveThickness;
		float _Thickness;
		float4 _ColorFront;
		float4 _ColorBack;

		struct Attributes
            {
                float4 position     : POSITION;
                float3 normal        : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

        struct Varyings
        {
            float2 uv           : TEXCOORD0;
            float4 positionCS   : SV_POSITION;
            float4 screenPos    : TEXCOORD1;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

         Varyings vert(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

				float3 positionWS = mul (unity_ObjectToWorld, input.position).xyz;
				//float3 ase_worldPos = mul(GetObjectToWorldMatrix(), input.position).xyz;
				float lerpResult59 = lerp( 1.0 , distance( _WorldSpaceCameraPos , positionWS ) , _AdaptiveThickness);

				float4 positionCS = UnityObjectToClipPos(input.position.xyz + _Thickness * input.normal * lerpResult59);

                output.positionCS = positionCS;
                output.screenPos = ComputeScreenPos(positionCS);

                //VertexPositionInputs positionInputs = GetVertexPositionInputs(input.position.xyz + _Thickness * input.normal * lerpResult59);
				//
                //output.positionCS = positionInputs.positionCS;
				//output.screenPos = positionInputs.positionNDC;

                return output;
            }

		float4 fragBack(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

			    //Behind R channel
			    //Front G channel

                float4 mask = tex2D(_SceneDepthMask, input.screenPos.xy / input.screenPos.w);

				if (mask.r > input.positionCS.z)
					{
						return _ColorBack;
					}
                
                return float4(0,0,0,0);
             }

		float4 fragFront(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

			    //Behind R channel
			    //Front G channel

                float4 mask = tex2D(_SceneDepthMask, input.screenPos.xy / input.screenPos.w);

				if (mask.r < input.positionCS.z)
					{
						return _ColorFront;
					}
                
				return float4(0,0,0,0);
             }

		float4 fragBoth(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

			    //Behind R channel
			    //Front G channel

				// This needs to be reversed when using WebGL

                float4 mask = tex2D(_SceneDepthMask, input.screenPos.xy / input.screenPos.w);

				#ifdef UNITY_REVERSED_Z 
				if (mask.r > input.positionCS.z)
					{
						return _ColorBack;
					}
				#else
				if (mask.r <= input.positionCS.z)
					{
						return _ColorBack;
					}
				#endif

				return _ColorFront;

				
             }
	ENDCG

	SubShader
	{
		ZWrite On
		ZTest LEqual
		Lighting Off

		Pass
		{
			Name "MeshOutlineFront"
			Cull Off

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragFront

            ENDCG
		}

		Pass
		{
			Name "MeshOutlineBack"
			Cull Off

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBack

            ENDCG
		}

		Pass
		{
			Name "MeshOutlineBoth"
			Cull Off

			CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBoth

            ENDCG
		}
	}
}

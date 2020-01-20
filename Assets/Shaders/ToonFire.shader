// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "ToonFire"
{
	Properties
	{
		_Noise2Speed("Noise 2 Speed", Float) = 0
		_Noise2Scale("Noise 2 Scale", Float) = 0
		_Noise1speed("Noise 1 speed", Float) = 0
		_Noise1scale("Noise 1 scale", Float) = 0
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_OpacityStep("OpacityStep", Range( 0 , 1)) = 0
		_InnerFlameStep("InnerFlameStep", Range( 0 , 1)) = 0
		_OuterColour("OuterColour", Color) = (1,0.7241379,0,0)
		_InnerColor("InnerColor", Color) = (1,0.9482759,0.25,0)
		_OuterColorBlend("OuterColorBlend", Range( 0 , 1)) = 0
		_DepthFade("DepthFade", Float) = 0
		_OuterColorTop("OuterColorTop", Color) = (1,0,0,0)
		_PushForward("PushForward", Range( 0 , 0.5)) = 0
		_Brightness("Brightness", Float) = 1
		_FlameMask("Flame Mask", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _PushForward;
		uniform sampler2D _NoiseTexture;
		uniform float _Noise1speed;
		uniform float _Noise1scale;
		uniform float _Noise2Scale;
		uniform float _Noise2Speed;
		uniform sampler2D _FlameMask;
		uniform float4 _FlameMask_ST;
		uniform float4 _InnerColor;
		uniform float _InnerFlameStep;
		uniform float _OpacityStep;
		uniform float _OuterColorBlend;
		uniform float4 _OuterColour;
		uniform float4 _OuterColorTop;
		uniform float _Brightness;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFade;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix ));
			//This unfortunately must be made to take non-uniform scaling into account;
			//Transform to world coords, apply rotation and transform back to local;
			v.vertex = mul( v.vertex , unity_ObjectToWorld );
			v.vertex = mul( v.vertex , rotationCamMatrix );
			v.vertex = mul( v.vertex , unity_WorldToObject );
			float4 transform62 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float4 normalizeResult61 = normalize( ( float4( _WorldSpaceCameraPos , 0.0 ) - transform62 ) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 BillboardPush71 = ( ( _PushForward * normalizeResult61 ) + float4( ( 0.1 * ( 0 + ase_vertex3Pos ) ) , 0.0 ) );
			v.vertex.xyz += BillboardPush71.xyz;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float4 appendResult3 = (float4(0.0 , _Noise1speed , 0.0 , 0.0));
			float2 panner11 = ( 1.0 * _Time.y * appendResult3.xy + ( _Noise1scale * i.uv_texcoord ));
			float4 appendResult9 = (float4(0.0 , _Noise2Scale , 0.0 , 0.0));
			float2 panner10 = ( 1.0 * _Time.y * appendResult9.xy + ( i.uv_texcoord * _Noise2Speed ));
			float2 uv_FlameMask = i.uv_texcoord * _FlameMask_ST.xy + _FlameMask_ST.zw;
			float4 tex2DNode85 = tex2D( _FlameMask, uv_FlameMask );
			float ScrollingNoise17 = ( ( ( tex2D( _NoiseTexture, panner11 ).r * tex2D( _NoiseTexture, panner10 ).r ) + tex2DNode85.r ) * tex2DNode85.r );
			float temp_output_31_0 = step( _InnerFlameStep , ScrollingNoise17 );
			float smoothstepResult45 = smoothstep( 0.0 , _OuterColorBlend , i.uv_texcoord.y);
			float4 StepCutoffColoring40 = ( saturate( ( ( 1.0 - 0.62 ) + ScrollingNoise17 ) ) * ( ( _InnerColor * temp_output_31_0 ) + ( ( step( _OpacityStep , ScrollingNoise17 ) - temp_output_31_0 ) * ( ( ( 1.0 - smoothstepResult45 ) * _OuterColour ) + ( smoothstepResult45 * _OuterColorTop ) ) ) ) * _Brightness );
			o.Emission = StepCutoffColoring40.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth53 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth53 = abs( ( screenDepth53 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float depthFade55 = saturate( distanceDepth53 );
			o.Alpha = ( tex2DNode85 * depthFade55 ).r;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf Standard alpha:fade keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.screenPos = IN.screenPos;
				SurfaceOutputStandard o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputStandard, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7;29;1522;788;655.2578;-871.2039;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;18;-2670.391,-670.1609;Float;False;1990.54;817.3841;Comment;21;17;12;15;16;10;14;11;4;3;8;9;1;6;2;7;5;83;84;85;86;87;Scrolling Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-2546.45,-166.2689;Float;False;Property;_Noise2Speed;Noise 2 Speed;0;0;Create;True;0;0;False;0;0;0.54;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;1;-2563.312,-601.1121;Float;False;Property;_Noise1speed;Noise 1 speed;2;0;Create;True;0;0;False;0;0;-1.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;2;-2575.034,-450.1854;Float;False;Property;_Noise1scale;Noise 1 scale;3;0;Create;True;0;0;False;0;0;1.22;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-2620.391,-316.1151;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-2558.172,-15.34209;Float;False;Property;_Noise2Scale;Noise 2 Scale;1;0;Create;True;0;0;False;0;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;3;-2325.931,-620.1609;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;4;-2313.664,-418.8628;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;9;-2281.359,-45.77704;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-2308.359,-192.7772;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;10;-2041.259,-121.6284;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;14;-2080.259,-373.8282;Float;True;Property;_NoiseTexture;Noise Texture;4;0;Create;True;0;0;False;0;None;cae6203344b65f44b805e6c60bd57a9f;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.PannerNode;11;-2071.158,-557.1283;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;15;-1809.859,-516.5282;Float;True;Property;_TextureSample0;Texture Sample 0;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;16;-1809.859,-273.7285;Float;True;Property;_TextureSample1;Texture Sample 1;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-1499.159,-355.6282;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;85;-1452.957,-144.2565;Float;True;Property;_FlameMask;Flame Mask;15;0;Create;True;0;0;False;0;None;b8c1c0128084cae49a0194b41cf85512;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;41;-3214.813,1388.248;Float;False;2022.737;1727.69;Comment;12;36;38;37;34;40;35;39;50;33;79;80;81;Step Cutoff Coloring;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleAddOpNode;83;-1248.957,-357.2565;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;84;-1052.957,-359.2565;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;50;-2987.213,2452.978;Float;False;1211.787;572.1818;Comment;8;46;47;48;49;45;42;43;44;Outer Color Vertical Blend;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;17;-890.848,-360.6003;Float;False;ScrollingNoise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;33;-3108.339,1888.058;Float;False;634.7317;407.3641;Comment;6;28;31;32;29;30;51;Step Cutoff;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;43;-2937.213,2672.159;Float;False;Property;_OuterColorBlend;OuterColorBlend;10;0;Create;True;0;0;False;0;0;0.701;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;42;-2878.248,2502.978;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SmoothstepOpNode;45;-2606.172,2636.755;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;51;-3089.214,2064.753;Float;False;17;ScrollingNoise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;70;-275.7885,1528.874;Float;False;1057.177;580.4827;Comment;6;57;58;59;60;61;62;PushForward;1,1,1,1;0;0
Node;AmplifyShaderEditor.RelayNode;28;-2867.57,2060.44;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;46;-2364.241,2623.755;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;30;-3058.339,2180.421;Float;False;Property;_OpacityStep;OpacityStep;6;0;Create;True;0;0;False;0;0;0.423;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;29;-3053.54,1938.058;Float;False;Property;_InnerFlameStep;InnerFlameStep;7;0;Create;True;0;0;False;0;0;0.897;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;81;-3053.108,1488.107;Float;False;1053.53;283.8441;Comment;6;78;74;73;77;76;75;Shading;1,1,1,1;0;0
Node;AmplifyShaderEditor.ColorNode;35;-2311.216,2264.571;Float;False;Property;_OuterColour;OuterColour;8;0;Create;True;0;0;False;0;1,0.7241379,0,0;1,0.8873225,0.2573529,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;44;-2890.213,2818.159;Float;False;Property;_OuterColorTop;OuterColorTop;12;0;Create;True;0;0;False;0;1,0,0,0;1,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.StepOpNode;31;-2628.808,1962.055;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-2365.242,2790.744;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;48;-2107.525,2612.643;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.CommentaryNode;56;-315.7039,1139.609;Float;False;1170.224;201.4963;Comment;4;52;53;54;55;Depth Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;69;-190.7242,2168.206;Float;False;884.6553;421.3557;Comment;5;64;65;66;67;68;BillBoard;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;74;-3003.108,1655.754;Float;False;17;ScrollingNoise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;62;-216.7885,1907.357;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;75;-2922.895,1538.107;Float;False;Constant;_ShadingAmount;Shading Amount;11;0;Create;True;0;0;False;0;0.62;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;32;-2627.608,2155.225;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceCameraPos;59;-225.7885,1706.357;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RelayNode;73;-2740.906,1661.952;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BillboardNode;64;-140.7242,2218.206;Float;False;Spherical;False;0;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-265.7037,1226.105;Float;False;Property;_DepthFade;DepthFade;11;0;Create;True;0;0;False;0;0;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;76;-2604.713,1544.792;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;65;-115.0687,2410.562;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;49;-1929.425,2730.239;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ColorNode;34;-2275.909,1807.371;Float;False;Property;_InnerColor;InnerColor;9;0;Create;True;0;0;False;0;1,0.9482759,0.25,0;1,0.5586207,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;60;118.2115,1803.357;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;38;-2246.883,2092.749;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;37;-1968.477,2111.796;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;77;-2424.233,1633.027;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;53;-6.009264,1189.609;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;61;334.2115,1816.357;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-222.9507,1578.874;Float;False;Property;_PushForward;PushForward;13;0;Create;True;0;0;False;0;0;0.263;0;0.5;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;326.9315,2410.562;Float;False;Constant;_Float1;Float 1;11;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;36;-1968.477,1919.845;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;66;202.9313,2256.562;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SaturateNode;54;352.522,1214.225;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;68;524.931,2254.562;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;80;-1768.764,2158.662;Float;False;Property;_Brightness;Brightness;14;0;Create;True;0;0;False;0;1;1.82;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;39;-1773.418,2010.339;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;58;612.3881,1596.796;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SaturateNode;78;-2174.578,1631.023;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;55;611.5203,1218.506;Float;False;depthFade;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;63;1012.101,1979.168;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;79;-1586.559,1981.695;Float;False;3;3;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;86;-1050.309,38.067;Float;False;55;depthFade;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;25;-2673.514,340.2745;Float;False;1602.291;576.0146;Comment;8;23;24;21;22;20;19;27;26;Gradient Coloring;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;40;-1429.709,1991.342;Float;False;StepCutoffColoring;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;71;1209.94,1987.975;Float;False;BillboardPush;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;26;-1318.296,442.6971;Float;False;GradientRGBA;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;24;-1756.774,421.1186;Float;True;Property;_TextureSample2;Texture Sample 2;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;21;-2280.858,497.0754;Float;False;Constant;_Float0;Float 0;5;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;87;-846.0352,-64.97869;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;72;-325.4106,338.7397;Float;False;71;BillboardPush;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;82;-323.4106,22.20697;Float;False;40;StepCutoffColoring;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-2623.514,390.2745;Float;False;17;ScrollingNoise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;20;-2262.858,396.0758;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;22;-2010.855,394.0758;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;27;-1318.296,555.8919;Float;False;GradientOpacity;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;23;-2080.96,599.728;Float;True;Property;_Gradient;Gradient;5;0;Create;True;0;0;False;0;None;None;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-1,-1;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;ToonFire;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;True;0;False;Transparent;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;3;1;1;0
WireConnection;4;0;2;0
WireConnection;4;1;5;0
WireConnection;9;1;7;0
WireConnection;8;0;5;0
WireConnection;8;1;6;0
WireConnection;10;0;8;0
WireConnection;10;2;9;0
WireConnection;11;0;4;0
WireConnection;11;2;3;0
WireConnection;15;0;14;0
WireConnection;15;1;11;0
WireConnection;16;0;14;0
WireConnection;16;1;10;0
WireConnection;12;0;15;1
WireConnection;12;1;16;1
WireConnection;83;0;12;0
WireConnection;83;1;85;1
WireConnection;84;0;83;0
WireConnection;84;1;85;1
WireConnection;17;0;84;0
WireConnection;45;0;42;2
WireConnection;45;2;43;0
WireConnection;28;0;51;0
WireConnection;46;0;45;0
WireConnection;31;0;29;0
WireConnection;31;1;28;0
WireConnection;47;0;45;0
WireConnection;47;1;44;0
WireConnection;48;0;46;0
WireConnection;48;1;35;0
WireConnection;32;0;30;0
WireConnection;32;1;28;0
WireConnection;73;0;74;0
WireConnection;76;0;75;0
WireConnection;49;0;48;0
WireConnection;49;1;47;0
WireConnection;60;0;59;0
WireConnection;60;1;62;0
WireConnection;38;0;32;0
WireConnection;38;1;31;0
WireConnection;37;0;38;0
WireConnection;37;1;49;0
WireConnection;77;0;76;0
WireConnection;77;1;73;0
WireConnection;53;0;52;0
WireConnection;61;0;60;0
WireConnection;36;0;34;0
WireConnection;36;1;31;0
WireConnection;66;0;64;0
WireConnection;66;1;65;0
WireConnection;54;0;53;0
WireConnection;68;0;67;0
WireConnection;68;1;66;0
WireConnection;39;0;36;0
WireConnection;39;1;37;0
WireConnection;58;0;57;0
WireConnection;58;1;61;0
WireConnection;78;0;77;0
WireConnection;55;0;54;0
WireConnection;63;0;58;0
WireConnection;63;1;68;0
WireConnection;79;0;78;0
WireConnection;79;1;39;0
WireConnection;79;2;80;0
WireConnection;40;0;79;0
WireConnection;71;0;63;0
WireConnection;26;0;24;0
WireConnection;24;0;23;0
WireConnection;24;1;22;0
WireConnection;87;0;85;0
WireConnection;87;1;86;0
WireConnection;20;0;19;0
WireConnection;22;0;20;0
WireConnection;22;1;21;0
WireConnection;27;0;24;4
WireConnection;0;2;82;0
WireConnection;0;9;87;0
WireConnection;0;11;72;0
ASEEND*/
//CHKSM=582E5FB125D0A925C10ED5E1A8A104636437BADC
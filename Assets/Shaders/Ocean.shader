// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Ocean"
{
	Properties
	{
		_WaveSpeed("WaveSpeed", Float) = 0
		_WaveTile("WaveTile", Float) = 1
		_WaveHeight("Wave Height", Float) = 0
		_Tessellation("Tessellation", Float) = 8
		_WaveSize("WaveSize", Vector) = (1,1,0,0)
		_Smoothness("Smoothness", Float) = 0
		_TopColor("Top Color", Color) = (0,0,0,0)
		_WaterColor("WaterColor", Color) = (0,0,0,0)
		_Normal_Map("Normal_Map", 2D) = "white" {}
		_NormalTile("NormalTile", Float) = 1
		_NormalSpeed("Normal Speed", Float) = 1
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 1
		_EdgeDistance("Edge Distance", Float) = 0
		_EdgePower("EdgePower", Range( 0 , 1)) = 0
		_SeaFoam("Sea Foam", 2D) = "white" {}
		_SeaFoamTile("SeaFoam Tile", Float) = 1
		_EdgeFoamTile("Edge Foam Tile", Float) = 1
		_RefractAmount("Refract Amount", Float) = 0.1
		_Depth("Depth", Float) = -4
		_MaxTes("Max Tes", Float) = 80
		_MinTes("Min Tes", Float) = 10
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityStandardUtils.cginc"
		#include "UnityCG.cginc"
		#include "Tessellation.cginc"
		#pragma target 4.6
		#pragma surface surf Standard keepalpha noshadow vertex:vertexDataFunc tessellate:tessFunction 
		struct Input
		{
			float3 worldPos;
			float4 screenPos;
		};

		uniform float _WaveHeight;
		uniform float _WaveSpeed;
		uniform float2 _WaveSize;
		uniform float _WaveTile;
		uniform sampler2D _Normal_Map;
		uniform float _NormalStrength;
		uniform float _NormalSpeed;
		uniform float _NormalTile;
		uniform float4 _WaterColor;
		uniform float4 _TopColor;
		uniform sampler2D _SeaFoam;
		uniform float _SeaFoamTile;
		uniform sampler2D _GrabTexture;
		uniform float _RefractAmount;
		uniform sampler2D _CameraDepthTexture;
		uniform float _Depth;
		uniform float _EdgePower;
		uniform float _EdgeDistance;
		uniform float _EdgeFoamTile;
		uniform float _Smoothness;
		uniform float _MinTes;
		uniform float _MaxTes;
		uniform float _Tessellation;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 Tessellation125 = UnityDistanceBasedTess( v0.vertex, v1.vertex, v2.vertex, _MinTes,_MaxTes,( _WaveHeight * _Tessellation ));
			return Tessellation125;
		}

		void vertexDataFunc( inout appdata_full v )
		{
			float temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			float4 appendResult9 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * float4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			float2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			float simplePerlin2D2 = snoise( panner3 );
			float2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0.1,0,0 ) ).xy);
			float simplePerlin2D23 = snoise( panner24 );
			float WavePattern28 = ( simplePerlin2D2 + simplePerlin2D23 );
			float3 WaveHeight34 = ( ( float3(0,1,0) * _WaveHeight ) * WavePattern28 );
			v.vertex.xyz += WaveHeight34;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float3 ase_worldPos = i.worldPos;
			float4 appendResult9 = (float4(ase_worldPos.x , ase_worldPos.z , 0.0 , 0.0));
			float4 WorldSpaceTile10 = appendResult9;
			float4 temp_output_62_0 = ( WorldSpaceTile10 * _NormalTile );
			float2 panner65 = ( 1.0 * _Time.y * ( float2( 1,0 ) * _NormalSpeed ) + temp_output_62_0.xy);
			float2 panner66 = ( 1.0 * _Time.y * ( float2( -1,0 ) * ( _NormalSpeed * 3.0 ) ) + ( temp_output_62_0 * ( _NormalTile * 5.0 ) ).xy);
			float3 Normals75 = BlendNormals( UnpackScaleNormal( tex2D( _Normal_Map, panner65 ), _NormalStrength ) , UnpackScaleNormal( tex2D( _Normal_Map, panner66 ), _NormalStrength ) );
			o.Normal = Normals75;
			float2 panner98 = ( 1.0 * _Time.y * float2( 0.2,0.2 ) + ( WorldSpaceTile10 * 0.03 ).xy);
			float simplePerlin2D97 = snoise( panner98 );
			float clampResult102 = clamp( ( tex2D( _SeaFoam, ( ( WorldSpaceTile10 / 10.0 ) * _SeaFoamTile ).xy ).r * simplePerlin2D97 ) , 0.0 , 1.0 );
			float SeaFoam93 = clampResult102;
			float temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * float4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			float2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			float simplePerlin2D2 = snoise( panner3 );
			float2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0.1,0,0 ) ).xy);
			float simplePerlin2D23 = snoise( panner24 );
			float WavePattern28 = ( simplePerlin2D2 + simplePerlin2D23 );
			float clampResult44 = clamp( WavePattern28 , 0.0 , 1.0 );
			float4 lerpResult41 = lerp( _WaterColor , ( _TopColor + SeaFoam93 ) , clampResult44);
			float4 WaterColor45 = lerpResult41;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor109 = tex2D( _GrabTexture, ( float3( (ase_grabScreenPosNorm).xy ,  0.0 ) + ( _RefractAmount * Normals75 ) ).xy );
			float4 clampResult110 = clamp( screenColor109 , float4( 0,0,0,0 ) , float4( 1,1,1,0 ) );
			float4 Refraction111 = clampResult110;
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth113 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth113 = abs( ( screenDepth113 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float clampResult115 = clamp( ( 1.0 - distanceDepth113 ) , 0.0 , 1.0 );
			float Depth116 = clampResult115;
			float4 lerpResult117 = lerp( WaterColor45 , Refraction111 , Depth116);
			o.Albedo = lerpResult117.rgb;
			float screenDepth48 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth48 = abs( ( screenDepth48 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float4 clampResult56 = clamp( ( _EdgePower * ( ( 1.0 - distanceDepth48 ) + tex2D( _SeaFoam, ( ( WorldSpaceTile10 / 10.0 ) * _EdgeFoamTile ).xy ) ) ) , float4( 0,0,0,0 ) , float4( 1,1,1,1 ) );
			float4 Edge53 = clampResult56;
			o.Emission = Edge53.rgb;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
7;29;1522;788;2723.731;511.5389;1;True;False
Node;AmplifyShaderEditor.CommentaryNode;11;-5142.05,-1086.919;Float;False;784.8464;240.4145;;3;8;9;10;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-5092.05,-1036.919;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;9;-4856.621,-1025.505;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-4611.204,-994.1138;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;21;-5418.107,-580.1932;Float;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;76;-1946.688,-1731.625;Float;False;2747.751;836.5029;Comment;19;60;62;61;59;38;66;65;67;69;57;63;64;68;72;71;70;73;74;75;NormalMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.Vector2Node;14;-5348.134,-391.7898;Float;False;Property;_WaveSize;WaveSize;4;0;Create;True;0;0;False;0;1,1;0.1,0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.GetLocalVarNode;12;-5368.107,-530.1931;Float;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1039.84,-1371.462;Float;False;Property;_NormalSpeed;Normal Speed;10;0;Create;True;0;0;False;0;1;0.01;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-5109.852,-483.1077;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;16;-4995.756,-328.9074;Float;False;Property;_WaveTile;WaveTile;1;0;Create;True;0;0;False;0;1;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1678.698,-1464.515;Float;False;Property;_NormalTile;NormalTile;9;0;Create;True;0;0;False;0;1;0.025;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-1723.212,-1681.625;Float;True;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-4830.063,-496.9503;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;68;-1035.566,-1068.523;Float;False;Constant;_PanD2;PanD2;12;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-1590.485,-1136.278;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-844.1078,-1232.211;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;67;-1041.161,-1539.724;Float;False;Constant;_PanDirection;PanDirection;12;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1411.934,-1568.418;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;96;-2108.62,735.262;Float;False;1668.536;780.4965;Comment;13;93;102;101;100;99;98;97;92;91;90;89;88;87;Sea Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;87;-1945.638,991.7604;Float;False;Constant;_Float3;Float 3;15;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-4637.876,-502.565;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;88;-2058.62,787.1517;Float;True;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;29;-5661.762,-32.73029;Float;False;1852.234;881.3187;Comment;13;27;6;5;22;25;7;24;23;26;28;4;3;2;Wave Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;100;-1943.317,1302.311;Float;False;Constant;_FoamMask;Foam Mask;17;0;Create;True;0;0;False;0;0.03;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1363.149,-1148.122;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-765.1539,-1059.121;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-798.9095,-1522.865;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;90;-1696.639,838.7603;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;89;-1681.597,997.3303;Float;False;Property;_SeaFoamTile;SeaFoam Tile;15;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;57;-1896.688,-1239.553;Float;True;Property;_Normal_Map;Normal_Map;8;0;Create;True;0;0;False;0;None;4c332ad77dd917b48bad9255508ff48c;True;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;99;-1741.671,1197.726;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;54;-2414.151,-418.9287;Float;False;1982.619;909.7831;Comment;15;53;56;51;86;52;80;78;82;48;84;49;79;83;85;81;Edge & Foam;1,1,1,1;0;0
Node;AmplifyShaderEditor.PannerNode;66;-594.7749,-1122.907;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;65;-635.7458,-1597.184;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-660.3539,-1382.03;Float;False;Property;_NormalStrength;Normal Strength;11;0;Create;True;0;0;False;0;1;0.26;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-5487.836,733.5884;Float;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-5542.072,490.7596;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-5508.265,603.5818;Float;False;Property;_WaveSpeed;WaveSpeed;0;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-5611.762,17.26974;Float;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;4;-5496.214,278.8184;Float;False;Constant;_WaveDirection;WaveDirection;0;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;98;-1544.719,1163.137;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0.2,0.2;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;38;-271.4618,-1588.374;Float;True;Property;_NormalMap;NormalMap;6;0;Create;True;0;0;False;0;None;4c332ad77dd917b48bad9255508ff48c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-265.2299,-1223.483;Float;True;Property;_TextureSample0;Texture Sample 0;12;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-5243.696,519.8863;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-5209.801,700.8604;Float;False;2;2;0;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0.1,0.1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;91;-1491.598,847.33;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.TexturePropertyNode;79;-2041.458,-77.46824;Float;True;Property;_SeaFoam;Sea Foam;14;0;Create;True;0;0;False;0;None;90cf1cd325ccb2a4e8d5ff5ac5167929;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.BlendNormalsNode;73;205.7039,-1316.774;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.PannerNode;24;-4942.731,502.285;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;92;-1288.526,785.262;Float;True;Property;_TextureSample2;Texture Sample 2;17;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.NoiseGeneratorNode;97;-1248.153,1079.785;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;3;-5009.715,171.8761;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;558.0623,-1304.877;Float;False;Normals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-2282.563,335.8652;Float;False;Constant;_Float0;Float 0;15;0;Create;True;0;0;False;0;10;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;81;-2395.545,131.2563;Float;True;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;112;-3911.249,-2310.31;Float;False;1585.792;512.9757;Comment;9;103;104;105;107;106;108;109;110;111;Refraction;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-4651.014,463.1583;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;101;-984.0433,1011.078;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-4709.463,122.0772;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2328.225,-258.564;Float;False;Property;_EdgeDistance;Edge Distance;12;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;84;-2033.564,182.8651;Float;False;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;83;-2018.522,341.435;Float;False;Property;_EdgeFoamTile;Edge Foam Tile;16;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;102;-829.5152,989.5159;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-4268.959,362.3141;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;121;-3728.436,-2653.674;Float;False;1196.501;216.6802;Comment;5;114;113;120;115;116;Depth;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;105;-3730.119,-2044.569;Float;False;Property;_RefractAmount;Refract Amount;17;0;Create;True;0;0;False;0;0.1;0.03;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GrabScreenPosition;103;-3861.249,-2260.31;Float;False;0;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;106;-3736.254,-1912.334;Float;False;75;Normals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;107;-3454.153,-1989.034;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;82;-1828.522,191.4349;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.DepthFade;48;-2126.9,-290.1482;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;93;-674.2852,1003.734;Float;False;SeaFoam;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;114;-3678.436,-2603.394;Float;False;Property;_Depth;Depth;18;0;Create;True;0;0;False;0;-4;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;46;-3858.743,-1533.971;Float;False;1131.484;741.8878;Comment;8;45;41;39;44;40;42;94;95;Water Color (Albedo);1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-4052.53,345.6291;Float;False;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ComponentMaskNode;104;-3487.951,-2221.736;Float;False;True;True;False;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;80;-1667.199,-41.39867;Float;True;Property;_TextureSample1;Texture Sample 1;15;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CommentaryNode;128;-3483.785,19.24007;Float;False;949.9185;463.7804;Comment;6;124;123;122;125;127;17;Tessellation;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;35;-4200.097,-656.0023;Float;False;1141.994;525.1011;Comment;6;19;18;31;32;33;34;Wave Height;1,1,1,1;0;0
Node;AmplifyShaderEditor.DepthFade;113;-3490.034,-2603.674;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;108;-3248.754,-2109.937;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.OneMinusNode;78;-1748.003,-259.4017;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-3778.755,-914.0261;Float;False;28;WavePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;95;-3749.358,-1078.397;Float;False;93;SeaFoam;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-3808.743,-1295.36;Float;False;Property;_TopColor;Top Color;6;0;Create;True;0;0;False;0;0,0,0,0;0.2647059,0.84787,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;31;-4150.097,-386.5335;Float;False;Property;_WaveHeight;Wave Height;2;0;Create;True;0;0;False;0;0;2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;86;-1359.034,-130.9676;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.ScreenColorNode;109;-3051.198,-2110.661;Float;False;Global;_GrabScreen1;Grab Screen 1;18;0;Create;True;0;0;False;0;Object;-1;False;False;1;0;FLOAT2;0,0;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;44;-3528.671,-1007.314;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;120;-3223.771,-2576.14;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;94;-3543.05,-1235.482;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-1362.079,-301.0467;Float;False;Property;_EdgePower;EdgePower;13;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;18;-4126.275,-606.0023;Float;False;Constant;_WaveUp;WaveUp;3;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.ColorNode;39;-3806.83,-1483.971;Float;False;Property;_WaterColor;WaterColor;7;0;Create;True;0;0;False;0;0,0,0,0;0.2185337,0.5933882,0.6911765,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;17;-3433.785,151.5142;Float;False;Property;_Tessellation;Tessellation;3;0;Create;True;0;0;False;0;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1049.055,-138.9047;Float;False;2;2;0;FLOAT;0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;127;-3218.243,69.24007;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-3857.037,-529.1097;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-3868.124,-245.9013;Float;False;28;WavePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;115;-3028.435,-2592.994;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;110;-2822.399,-2093.063;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;123;-3408.248,272.0201;Float;False;Property;_MinTes;Min Tes;20;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;41;-3343.014,-1366.302;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;124;-3393.248,368.0205;Float;False;Property;_MaxTes;Max Tes;19;0;Create;True;0;0;False;0;80;80;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-3527.06,-348.0035;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;56;-848.8501,-109.1869;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;1,1,1,1;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-2989.354,-1319.524;Float;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.DistanceBasedTessNode;122;-3159.248,223.0201;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;111;-2568.457,-2087.664;Float;False;Refraction;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;116;-2774.935,-2573.494;Float;False;Depth;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-683.9311,-105.3814;Float;False;Edge;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;125;-2776.867,237.5264;Float;False;Tessellation;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;118;-95.73212,-448.4709;Float;False;111;Refraction;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-3301.104,-328.0185;Float;False;WaveHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;-103.7889,-537.5857;Float;False;45;WaterColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;119;-102.4417,-351.523;Float;False;116;Depth;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;12.55198,-62.57859;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;117;216.2679,-482.4709;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;14.05048,66.37958;Float;False;34;WaveHeight;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;126;32.28403,173.9811;Float;False;125;Tessellation;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;67.0506,-196.7179;Float;False;75;Normals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-254.0374,-142.8138;Float;False;53;Edge;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;577.405,-342.2245;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;Ocean;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;False;0;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;8;1
WireConnection;9;1;8;3
WireConnection;10;0;9;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;63;0;61;0
WireConnection;71;0;70;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;20;0;15;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;72;0;68;0
WireConnection;72;1;71;0
WireConnection;69;0;67;0
WireConnection;69;1;70;0
WireConnection;90;0;88;0
WireConnection;90;1;87;0
WireConnection;99;0;88;0
WireConnection;99;1;100;0
WireConnection;66;0;64;0
WireConnection;66;2;72;0
WireConnection;65;0;62;0
WireConnection;65;2;69;0
WireConnection;98;0;99;0
WireConnection;38;0;57;0
WireConnection;38;1;65;0
WireConnection;38;5;74;0
WireConnection;59;0;57;0
WireConnection;59;1;66;0
WireConnection;59;5;74;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;25;0;27;0
WireConnection;91;0;90;0
WireConnection;91;1;89;0
WireConnection;73;0;38;0
WireConnection;73;1;59;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;92;0;79;0
WireConnection;92;1;91;0
WireConnection;97;0;98;0
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;75;0;73;0
WireConnection;23;0;24;0
WireConnection;101;0;92;1
WireConnection;101;1;97;0
WireConnection;2;0;3;0
WireConnection;84;0;81;0
WireConnection;84;1;85;0
WireConnection;102;0;101;0
WireConnection;26;0;2;0
WireConnection;26;1;23;0
WireConnection;107;0;105;0
WireConnection;107;1;106;0
WireConnection;82;0;84;0
WireConnection;82;1;83;0
WireConnection;48;0;49;0
WireConnection;93;0;102;0
WireConnection;28;0;26;0
WireConnection;104;0;103;0
WireConnection;80;0;79;0
WireConnection;80;1;82;0
WireConnection;113;0;114;0
WireConnection;108;0;104;0
WireConnection;108;1;107;0
WireConnection;78;0;48;0
WireConnection;86;0;78;0
WireConnection;86;1;80;0
WireConnection;109;0;108;0
WireConnection;44;0;42;0
WireConnection;120;0;113;0
WireConnection;94;0;40;0
WireConnection;94;1;95;0
WireConnection;51;0;52;0
WireConnection;51;1;86;0
WireConnection;127;0;31;0
WireConnection;127;1;17;0
WireConnection;19;0;18;0
WireConnection;19;1;31;0
WireConnection;115;0;120;0
WireConnection;110;0;109;0
WireConnection;41;0;39;0
WireConnection;41;1;94;0
WireConnection;41;2;44;0
WireConnection;32;0;19;0
WireConnection;32;1;33;0
WireConnection;56;0;51;0
WireConnection;45;0;41;0
WireConnection;122;0;127;0
WireConnection;122;1;123;0
WireConnection;122;2;124;0
WireConnection;111;0;110;0
WireConnection;116;0;115;0
WireConnection;53;0;56;0
WireConnection;125;0;122;0
WireConnection;34;0;32;0
WireConnection;117;0;47;0
WireConnection;117;1;118;0
WireConnection;117;2;119;0
WireConnection;0;0;117;0
WireConnection;0;1;77;0
WireConnection;0;2;55;0
WireConnection;0;4;37;0
WireConnection;0;11;36;0
WireConnection;0;14;126;0
ASEEND*/
//CHKSM=E30F6B4D23E25B82DBE3F5F46C6C239848AAFE58
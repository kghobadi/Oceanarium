// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "TrippyWater"
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
		_EdgeDistance("Edge Distance", Float) = 0
		_EdgePower("EdgePower", Range( 0 , 1)) = 0
		_Normal_Map("Normal_Map", 2D) = "white" {}
		_NormalTile("NormalTile", Float) = 1
		_NormalSpeed("Normal Speed", Float) = 1
		_NormalStrength("Normal Strength", Range( 0 , 1)) = 1
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
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
		uniform sampler2D _CameraDepthTexture;
		uniform float _EdgeDistance;
		uniform float _EdgePower;
		uniform float _Smoothness;
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


		float4 tessFunction( appdata_full v0, appdata_full v1, appdata_full v2 )
		{
			float4 temp_cast_3 = (_Tessellation).xxxx;
			return temp_cast_3;
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
			float temp_output_7_0 = ( _Time.y * _WaveSpeed );
			float2 _WaveDirection = float2(1,0);
			float4 WaveTileUV20 = ( ( WorldSpaceTile10 * float4( _WaveSize, 0.0 , 0.0 ) ) * _WaveTile );
			float2 panner3 = ( temp_output_7_0 * _WaveDirection + WaveTileUV20.xy);
			float simplePerlin2D2 = snoise( panner3 );
			float2 panner24 = ( temp_output_7_0 * _WaveDirection + ( WaveTileUV20 * float4( 0.1,0.1,0,0 ) ).xy);
			float simplePerlin2D23 = snoise( panner24 );
			float WavePattern28 = ( simplePerlin2D2 + simplePerlin2D23 );
			float clampResult44 = clamp( WavePattern28 , 0.0 , 1.0 );
			float4 lerpResult41 = lerp( _WaterColor , _TopColor , clampResult44);
			float4 WaterColor45 = lerpResult41;
			o.Albedo = WaterColor45.rgb;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth48 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth48 = abs( ( screenDepth48 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _EdgeDistance ) );
			float clampResult56 = clamp( ( ( 1.0 - distanceDepth48 ) * _EdgePower ) , 0.0 , 1.0 );
			float Edge53 = clampResult56;
			float3 temp_cast_6 = (Edge53).xxx;
			o.Emission = temp_cast_6;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
175;375;1276;442;1785.612;1586.495;1.434108;True;True
Node;AmplifyShaderEditor.CommentaryNode;11;-5142.05,-1086.919;Float;False;784.8464;240.4145;;3;8;9;10;WorldSpaceTile;1,1,1,1;0;0
Node;AmplifyShaderEditor.WorldPosInputsNode;8;-5092.05,-1036.919;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;9;-4856.621,-1025.505;Float;False;FLOAT4;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;21;-5418.107,-580.1932;Float;False;1023.231;366.2856;Comment;6;12;14;13;16;15;20;Wave Tile UV;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;10;-4611.204,-994.1138;Float;False;WorldSpaceTile;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;12;-5368.107,-530.1931;Float;False;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;14;-5348.134,-391.7898;Float;False;Property;_WaveSize;WaveSize;4;0;Create;True;0;0;False;0;1,1;0.15,0.02;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.RangedFloatNode;16;-4995.756,-328.9074;Float;False;Property;_WaveTile;WaveTile;1;0;Create;True;0;0;False;0;1;0.44;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-5109.852,-483.1077;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT2;0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;15;-4830.063,-496.9503;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;29;-5273.777,69.75669;Float;False;1852.234;881.3187;Comment;13;27;6;5;22;25;7;24;23;26;28;4;3;2;Wave Pattern;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;20;-4637.876,-502.565;Float;False;WaveTileUV;-1;True;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-5099.851,836.0753;Float;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleTimeNode;5;-5154.087,593.2465;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;6;-5120.28,706.0688;Float;False;Property;_WaveSpeed;WaveSpeed;0;0;Create;True;0;0;False;0;0;0.2;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;7;-4855.711,622.3732;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;22;-5223.777,119.7567;Float;False;20;WaveTileUV;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;25;-4821.816,803.3473;Float;False;2;2;0;FLOAT4;0.1,0,0,0;False;1;FLOAT4;0.1,0.1,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.Vector2Node;4;-5108.229,381.3053;Float;False;Constant;_WaveDirection;WaveDirection;0;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;24;-4554.746,604.772;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;3;-4621.73,274.363;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CommentaryNode;76;-1946.688,-1731.625;Float;False;2747.751;836.5029;Comment;19;60;62;61;59;38;66;65;67;69;57;63;64;68;72;71;70;73;74;75;NormalMap;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;60;-1723.212,-1681.625;Float;True;10;WorldSpaceTile;1;0;OBJECT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;23;-4263.029,565.6452;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;54;-2691.176,-436.0188;Float;False;1336.239;330.689;Comment;7;48;50;51;52;53;49;56;Edge;1,1,1,1;0;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-4321.478,224.5642;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;61;-1678.698,-1464.515;Float;False;Property;_NormalTile;NormalTile;11;0;Create;True;0;0;False;0;1;0.07;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-1039.84,-1371.462;Float;False;Property;_NormalSpeed;Normal Speed;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;68;-1035.566,-1068.523;Float;False;Constant;_PanD2;PanD2;12;0;Create;True;0;0;False;0;-1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;67;-1041.161,-1539.724;Float;False;Constant;_PanDirection;PanDirection;12;0;Create;True;0;0;False;0;1,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1411.934,-1568.418;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;71;-844.1078,-1232.211;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;3;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;26;-3880.973,464.801;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;49;-2641.176,-314.1212;Float;False;Property;_EdgeDistance;Edge Distance;8;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;63;-1590.485,-1136.278;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;35;-4200.097,-656.0023;Float;False;1141.994;525.1011;Comment;6;19;18;31;32;33;34;Wave Height;1,1,1,1;0;0
Node;AmplifyShaderEditor.DepthFade;48;-2388.071,-386.0188;Float;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;69;-798.9095,-1522.865;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1363.149,-1148.122;Float;True;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT;0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.CommentaryNode;46;-3858.743,-1533.971;Float;False;1112.389;594.3395;Comment;6;42;40;39;44;41;45;Water Color;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-765.1539,-1059.121;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-3664.542,448.116;Float;False;WavePattern;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;50;-2129.017,-340.6666;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;52;-2232.018,-227.6667;Float;False;Property;_EdgePower;EdgePower;9;0;Create;True;0;0;False;0;0;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector3Node;18;-4126.275,-606.0023;Float;False;Constant;_WaveUp;WaveUp;3;0;Create;True;0;0;False;0;0,1,0;0,0,0;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.PannerNode;65;-635.7458,-1597.184;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;66;-594.7749,-1122.907;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;74;-660.3539,-1382.03;Float;False;Property;_NormalStrength;Normal Strength;13;0;Create;True;0;0;False;0;1;0.61;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;31;-4150.097,-386.5335;Float;False;Property;_WaveHeight;Wave Height;2;0;Create;True;0;0;False;0;0;3.35;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TexturePropertyNode;57;-1896.688,-1239.553;Float;True;Property;_Normal_Map;Normal_Map;10;0;Create;True;0;0;False;0;None;4c332ad77dd917b48bad9255508ff48c;True;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.GetLocalVarNode;42;-3789.17,-1054.631;Float;False;28;WavePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;-1928.29,-302.8588;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;38;-271.4618,-1588.374;Float;True;Property;_NormalMap;NormalMap;6;0;Create;True;0;0;False;0;None;4c332ad77dd917b48bad9255508ff48c;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ClampOpNode;44;-3539.086,-1147.919;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;33;-3868.124,-245.9013;Float;False;28;WavePattern;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;40;-3808.743,-1295.36;Float;False;Property;_TopColor;Top Color;6;0;Create;True;0;0;False;0;0,0,0,0;0.2783304,0.7398877,0.8602941,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;39;-3806.83,-1483.971;Float;False;Property;_WaterColor;WaterColor;7;0;Create;True;0;0;False;0;0,0,0,0;0.2185337,0.5933882,0.6911765,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-265.2299,-1223.483;Float;True;Property;_TextureSample0;Texture Sample 0;12;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;19;-3857.037,-529.1097;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.LerpOp;41;-3343.014,-1366.302;Float;True;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.BlendNormalsNode;73;205.7039,-1316.774;Float;False;0;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;32;-3527.06,-348.0035;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.ClampOpNode;56;-1770.386,-307.9575;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;45;-2989.354,-1319.524;Float;False;WaterColor;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;53;-1591.007,-309.5983;Float;False;Edge;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;34;-3301.104,-328.0185;Float;False;WaveHeight;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;75;558.0623,-1304.877;Float;False;Normals;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;77;67.0506,-196.7179;Float;False;75;Normals;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;14.05048,66.37958;Float;False;34;WaveHeight;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;37;12.55198,-62.57859;Float;False;Property;_Smoothness;Smoothness;5;0;Create;True;0;0;False;0;0;0.9;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;50.73405,190.0613;Float;False;Property;_Tessellation;Tessellation;3;0;Create;True;0;0;False;0;8;8;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;47;100.2111,-397.5857;Float;False;45;WaterColor;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;55;-253.0374,-142.8138;Float;False;53;Edge;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;297.405,-189.2245;Float;False;True;6;Float;ASEMaterialInspector;0;0;Standard;TrippyWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;False;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;True;2;15;10;25;False;0.5;False;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;8;1
WireConnection;9;1;8;3
WireConnection;10;0;9;0
WireConnection;13;0;12;0
WireConnection;13;1;14;0
WireConnection;15;0;13;0
WireConnection;15;1;16;0
WireConnection;20;0;15;0
WireConnection;7;0;5;0
WireConnection;7;1;6;0
WireConnection;25;0;27;0
WireConnection;24;0;25;0
WireConnection;24;2;4;0
WireConnection;24;1;7;0
WireConnection;3;0;22;0
WireConnection;3;2;4;0
WireConnection;3;1;7;0
WireConnection;23;0;24;0
WireConnection;2;0;3;0
WireConnection;62;0;60;0
WireConnection;62;1;61;0
WireConnection;71;0;70;0
WireConnection;26;0;2;0
WireConnection;26;1;23;0
WireConnection;63;0;61;0
WireConnection;48;0;49;0
WireConnection;69;0;67;0
WireConnection;69;1;70;0
WireConnection;64;0;62;0
WireConnection;64;1;63;0
WireConnection;72;0;68;0
WireConnection;72;1;71;0
WireConnection;28;0;26;0
WireConnection;50;0;48;0
WireConnection;65;0;62;0
WireConnection;65;2;69;0
WireConnection;66;0;64;0
WireConnection;66;2;72;0
WireConnection;51;0;50;0
WireConnection;51;1;52;0
WireConnection;38;0;57;0
WireConnection;38;1;65;0
WireConnection;38;5;74;0
WireConnection;44;0;42;0
WireConnection;59;0;57;0
WireConnection;59;1;66;0
WireConnection;59;5;74;0
WireConnection;19;0;18;0
WireConnection;19;1;31;0
WireConnection;41;0;39;0
WireConnection;41;1;40;0
WireConnection;41;2;44;0
WireConnection;73;0;38;0
WireConnection;73;1;59;0
WireConnection;32;0;19;0
WireConnection;32;1;33;0
WireConnection;56;0;51;0
WireConnection;45;0;41;0
WireConnection;53;0;56;0
WireConnection;34;0;32;0
WireConnection;75;0;73;0
WireConnection;0;0;47;0
WireConnection;0;1;77;0
WireConnection;0;2;55;0
WireConnection;0;4;37;0
WireConnection;0;11;36;0
WireConnection;0;14;17;0
ASEEND*/
//CHKSM=5796BDEF43E83C41D2F015C821B62676FEB7F26A
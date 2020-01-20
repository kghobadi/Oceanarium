// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Teleport/Teleport"
{
	Properties
	{
		_Tiling("Tiling", Vector) = (5,5,0,0)
		_Speed("Speed", Float) = 0
		_Teleport("Teleport", Range( -20 , 20)) = 2.033981
		_Range("Range", Range( -10 , 10)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha addshadow fullforwardshadows noforwardadd 
		struct Input
		{
			float3 worldPos;
			float2 uv_texcoord;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _Teleport;
		uniform float _Range;
		uniform float2 _Tiling;
		uniform float _Speed;


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


		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			c.rgb = 0;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform20 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Ygradient18 = saturate( ( ( transform20.y + _Teleport ) / _Range ) );
			float mulTime6 = _Time.y * _Speed;
			float2 temp_cast_1 = (mulTime6).xx;
			float2 panner8 = ( mulTime6 * float2( 0,0 ) + temp_cast_1);
			float2 uv_TexCoord1 = i.uv_texcoord * _Tiling + panner8;
			float simplePerlin2D2 = snoise( uv_TexCoord1 );
			float Noise12 = ( simplePerlin2D2 + 1.0 );
			float Opacity_Mask28 = ( ( 1.0 - Ygradient18 ) * Noise12 );
			float3 temp_cast_2 = (Opacity_Mask28).xxx;
			o.Emission = temp_cast_2;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
171;271;1276;501;3442.464;876.1357;4.258062;True;True
Node;AmplifyShaderEditor.CommentaryNode;14;-3779.667,-1085.469;Float;False;1832.791;665.4265;Noise;9;7;6;4;8;1;9;2;10;12;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;24;-3711.398,-126.3049;Float;False;1742.208;762.4413;Comment;8;15;20;17;22;16;21;23;18;Y_Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-3661.398,-72.63852;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;7;-3729.667,-686.4238;Float;True;Property;_Speed;Speed;2;0;Create;True;0;0;False;0;0;5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleTimeNode;6;-3476.217,-673.0421;Float;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-3544.583,160.9197;Float;False;Property;_Teleport;Teleport;3;0;Create;True;0;0;False;0;2.033981;4;-20;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;20;-3421.721,-76.30489;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;22;-3081.381,378.1365;Float;True;Property;_Range;Range;4;0;Create;True;0;0;False;0;0;6;-10;10;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;4;-3289.554,-932.2168;Float;False;Property;_Tiling;Tiling;1;0;Create;True;0;0;False;0;5,5;5,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;8;-3242.551,-705.3568;Float;True;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-3141.903,95.37923;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleDivideOpNode;21;-2769.487,166.0044;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-2951.296,-952.2312;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;23;-2444.75,180.5933;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;9;-2585.226,-711.9702;Float;True;Constant;_Booster;Booster;3;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NoiseGeneratorNode;2;-2594.181,-1035.469;Float;True;Simplex2D;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;10;-2348.888,-855.8224;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2212.19,132.4777;Float;False;Ygradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-1564.351,55.41296;Float;True;18;Ygradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2189.875,-873.3031;Float;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-1239.086,62.20836;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-1257.91,392.1168;Float;False;12;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-882.0198,133.2031;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-491.5807,195.8372;Float;False;Opacity_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-330.2155,-89.6116;Float;False;12;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-189.4827,232.2052;Float;True;28;Opacity_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;47.40138,-7.900229;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Teleport/Teleport;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;6;0;7;0
WireConnection;20;0;15;0
WireConnection;8;0;6;0
WireConnection;8;1;6;0
WireConnection;16;0;20;2
WireConnection;16;1;17;0
WireConnection;21;0;16;0
WireConnection;21;1;22;0
WireConnection;1;0;4;0
WireConnection;1;1;8;0
WireConnection;23;0;21;0
WireConnection;2;0;1;0
WireConnection;10;0;2;0
WireConnection;10;1;9;0
WireConnection;18;0;23;0
WireConnection;12;0;10;0
WireConnection;25;0;19;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;28;0;26;0
WireConnection;0;2;13;0
WireConnection;0;15;29;0
ASEEND*/
//CHKSM=AEB251E9F2C822331428B4480D18CFF707FA764E
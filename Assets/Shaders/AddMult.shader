// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "MathOps/AddmultiplyTexture"
{
	Properties
	{
		_Texture1("Texture 1", 2D) = "bump" {}
		_Texture1Tiling("Texture 1 Tiling", Vector) = (0,0,0,0)
		_Texture2("Texture 2", 2D) = "white" {}
		_Texture2Tiling("Texture 2 Tiling", Vector) = (0,0,0,0)
		_Speed("Speed", Float) = 0
		_Direction("Direction", Vector) = (0.5,0.5,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _Texture1;
		uniform float2 _Texture1Tiling;
		uniform sampler2D _Texture2;
		uniform float _Speed;
		uniform float2 _Direction;
		uniform float2 _Texture2Tiling;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_TexCoord11 = i.uv_texcoord * _Texture1Tiling;
			float2 uv_TexCoord4 = i.uv_texcoord * _Texture2Tiling;
			float2 panner5 = ( ( _Time.y * _Speed ) * _Direction + uv_TexCoord4);
			o.Albedo = ( float4( UnpackNormal( tex2D( _Texture1, uv_TexCoord11 ) ) , 0.0 ) + tex2D( _Texture2, panner5 ) ).rgb;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
175;375;1276;442;2834.517;825.4653;3.275328;True;True
Node;AmplifyShaderEditor.TimeNode;7;-1505.457,96.67754;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;8;-1434.019,275.2726;Float;False;Property;_Speed;Speed;4;0;Create;True;0;0;False;0;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;10;-1864.853,-311.6069;Float;False;Property;_Texture2Tiling;Texture 2 Tiling;3;0;Create;True;0;0;False;0;0,0;25,5;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;-1264.353,123.4668;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;13;-1301.695,-573.3876;Float;False;Property;_Texture1Tiling;Texture 1 Tiling;1;0;Create;True;0;0;False;0;0,0;0,0;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.Vector2Node;6;-1434.02,-57.36047;Float;False;Property;_Direction;Direction;5;0;Create;True;0;0;False;0;0.5,0.5;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.TextureCoordinatesNode;4;-1524.402,-340.9242;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;2,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;11;-1037.667,-546.1716;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.PannerNode;5;-1038.88,-104.2415;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-694.0304,-557.326;Float;True;Property;_Texture1;Texture 1;0;0;Create;True;0;0;False;0;None;24e31ecbf813d9e49bf7a1e0d4034916;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;2;-733.3445,-114.9191;Float;True;Property;_Texture2;Texture 2;2;0;Create;True;0;0;False;0;None;60eb4ceee01d39540b9d3da69930715e;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;3;-318.3422,-212.741;Float;False;2;2;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;-116,-291;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;MathOps/AddmultiplyTexture;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;9;0;7;2
WireConnection;9;1;8;0
WireConnection;4;0;10;0
WireConnection;11;0;13;0
WireConnection;5;0;4;0
WireConnection;5;2;6;0
WireConnection;5;1;9;0
WireConnection;1;1;11;0
WireConnection;2;1;5;0
WireConnection;3;0;1;0
WireConnection;3;1;2;0
WireConnection;0;0;3;0
ASEEND*/
//CHKSM=BE8180D0E27FA3B9B08E25B3AEE84D088B0C7FAB
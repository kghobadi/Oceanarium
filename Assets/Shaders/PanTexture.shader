// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PanTexture"
{
	Properties
	{
		_ColorMultiplier("Color Multiplier", Vector) = (0,0,0,0)
		_PanningSpeed("Panning Speed", Vector) = (0,0,0,0)
		_MainTexture("Main Texture", 2D) = "bump" {}
		_Smoothness("Smoothness", Float) = 0
		_Opacity("Opacity", Float) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_Emission("Emission", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred 
		struct Input
		{
			float2 uv_texcoord;
		};

		uniform sampler2D _MainTexture;
		uniform float2 _PanningSpeed;
		uniform float3 _ColorMultiplier;
		uniform float _Emission;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Opacity;

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 panner4 = ( _Time.y * _PanningSpeed + i.uv_texcoord);
			o.Albedo = ( UnpackNormal( tex2D( _MainTexture, panner4 ) ) * _ColorMultiplier );
			float3 temp_cast_0 = (_Emission).xxx;
			o.Emission = temp_cast_0;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = _Opacity;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
313.5;645.5;948;512;864.9896;1052.577;3.96278;False;True
Node;AmplifyShaderEditor.TimeNode;5;-405.8676,-274.1556;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.TextureCoordinatesNode;8;-418.7355,-589.5642;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.Vector2Node;1;-474.7843,-423.8922;Float;False;Property;_PanningSpeed;Panning Speed;1;0;Create;True;0;0;False;0;0,0;0.1,0.1;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.PannerNode;4;-202.6364,-415.8016;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,0;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.Vector3Node;2;-33.51504,-112.5131;Float;False;Property;_ColorMultiplier;Color Multiplier;0;0;Create;True;0;0;False;0;0,0,0;-2.5,12.7,-6.39;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.SamplerNode;7;-15.88657,-426.0016;Float;True;Property;_MainTexture;Main Texture;2;0;Create;True;0;0;False;0;None;f53512d44b91e954dae7bf028209df1a;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;11;173,-3;Float;False;Property;_Metallic;Metallic;5;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;10;238.3354,-82.75433;Float;False;Property;_Emission;Emission;6;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;13;259.3354,227.2457;Float;False;Property;_Opacity;Opacity;4;0;Create;True;0;0;False;0;0;0.46;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;6;289.3207,-218.579;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;12;172.7,95.7;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0;3.11;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;520.03,-250.4041;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;PanTexture;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;4;0;8;0
WireConnection;4;2;1;0
WireConnection;4;1;5;2
WireConnection;7;1;4;0
WireConnection;6;0;7;0
WireConnection;6;1;2;0
WireConnection;0;0;6;0
WireConnection;0;2;10;0
WireConnection;0;3;11;0
WireConnection;0;4;12;0
WireConnection;0;9;13;0
ASEEND*/
//CHKSM=BEE503B9249082C48CB8A3D30CEE534950A9177E
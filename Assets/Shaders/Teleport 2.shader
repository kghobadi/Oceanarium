// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Teleport/Teleport 2"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_Tint("Tint", Color) = (0,0,0,0)
		_Albedo("Albedo", 2D) = "white" {}
		_Smoothness("Smoothness", Range( 0 , 1)) = 0
		_Metallic("Metallic", Range( 0 , 1)) = 0
		_NormalMap("NormalMap", 2D) = "white" {}
		_AmbientOcclusion("Ambient Occlusion", 2D) = "white" {}
		[HDR]_GlowColour("Glow Colour", Color) = (0.4558824,1,1,0)
		_Teleport("Teleport", Range( -20 , 20)) = 2.033981
		[Toggle]_Reverse("Reverse", Float) = 0
		_NoiseScale("NoiseScale", Float) = 0
		_Thickness("Thickness", Range( 0 , 1)) = 0
		_Speed("Speed", Float) = 1
		_VertOffsetStrength("VertOffsetStrength", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Standard keepalpha addshadow fullforwardshadows vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
		};

		uniform float _Teleport;
		uniform float _Reverse;
		uniform float _VertOffsetStrength;
		uniform float _NoiseScale;
		uniform float _Speed;
		uniform float _Thickness;
		uniform sampler2D _NormalMap;
		uniform float4 _NormalMap_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _AmbientOcclusion;
		uniform float4 _AmbientOcclusion_ST;
		uniform float4 _Tint;
		uniform float4 _GlowColour;
		uniform float _Metallic;
		uniform float _Smoothness;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertex3Pos = v.vertex.xyz;
			float4 transform20 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Ygradient18 = saturate( ( ( transform20.y + _Teleport ) / lerp(-10.0,10.0,_Reverse) ) );
			float2 temp_cast_1 = (_NoiseScale).xx;
			float2 panner73 = ( ( _Time.y * _Speed ) * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord1 = v.texcoord.xy * temp_cast_1 + panner73;
			float Noise12 = step( frac( uv_TexCoord1.y ) , _Thickness );
			float3 VertOffset60 = ( ( ( ase_vertex3Pos * Ygradient18 ) * _VertOffsetStrength ) * Noise12 );
			v.vertex.xyz += VertOffset60;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float2 uv_NormalMap = i.uv_texcoord * _NormalMap_ST.xy + _NormalMap_ST.zw;
			float4 NormalMap52 = tex2D( _NormalMap, uv_NormalMap );
			o.Normal = NormalMap52.rgb;
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_AmbientOcclusion = i.uv_texcoord * _AmbientOcclusion_ST.xy + _AmbientOcclusion_ST.zw;
			float4 Albedo48 = ( tex2D( _Albedo, uv_Albedo ) * tex2D( _AmbientOcclusion, uv_AmbientOcclusion ) * _Tint );
			o.Albedo = Albedo48.rgb;
			float2 temp_cast_2 = (_NoiseScale).xx;
			float2 panner73 = ( ( _Time.y * _Speed ) * float2( 0,-1 ) + float2( 0,0 ));
			float2 uv_TexCoord1 = i.uv_texcoord * temp_cast_2 + panner73;
			float Noise12 = step( frac( uv_TexCoord1.y ) , _Thickness );
			float3 ase_vertex3Pos = mul( unity_WorldToObject, float4( i.worldPos , 1 ) );
			float4 transform20 = mul(unity_ObjectToWorld,float4( ase_vertex3Pos , 0.0 ));
			float Ygradient18 = saturate( ( ( transform20.y + _Teleport ) / lerp(-10.0,10.0,_Reverse) ) );
			float4 Emission39 = ( _GlowColour * ( Noise12 * Ygradient18 ) );
			o.Emission = Emission39.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1;
			float temp_output_31_0 = ( Ygradient18 * 1.0 );
			float Opacity_Mask28 = ( ( ( ( 1.0 - Ygradient18 ) * Noise12 ) - temp_output_31_0 ) + ( 1.0 - temp_output_31_0 ) );
			clip( Opacity_Mask28 - _Cutoff );
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16200
171;271;1276;501;4661.549;875.6361;1;True;True
Node;AmplifyShaderEditor.CommentaryNode;24;-4337.113,-210.9457;Float;False;1742.208;762.4413;Comment;10;15;20;17;22;16;21;23;18;67;68;Y_Gradient;1,1,1,1;0;0
Node;AmplifyShaderEditor.CommentaryNode;14;-4302.858,-1098.884;Float;False;1832.791;665.4265;Noise;10;1;12;69;70;71;72;73;74;75;76;Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;75;-4253.649,-666.8931;Float;True;Property;_Speed;Speed;12;0;Create;True;0;0;False;0;1;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PosVertexDataNode;15;-4287.114,-157.2794;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleTimeNode;74;-4268.669,-903.7402;Float;False;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;17;-4170.299,76.27884;Float;False;Property;_Teleport;Teleport;8;0;Create;True;0;0;False;0;2.033981;-2;-20;20;0;1;FLOAT;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;20;-4047.436,-160.9458;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;-4094.723,-724.3969;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;67;-3781.346,409.9423;Float;False;Constant;_Positive;Positive;4;0;Create;True;0;0;False;0;10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;22;-3781.791,280.6023;Float;False;Constant;_Negative;Negative;4;0;Create;True;0;0;False;0;-10;10;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ToggleSwitchNode;68;-3599.129,345.9468;Float;False;Property;_Reverse;Reverse;9;0;Create;True;0;0;False;0;0;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;73;-3885.817,-738.882;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleAddOpNode;16;-3767.617,10.7383;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;70;-4016.829,-962.6779;Float;True;Property;_NoiseScale;NoiseScale;10;0;Create;True;0;0;False;0;0;15;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;1;-3648.521,-926.718;Float;True;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleDivideOpNode;21;-3353.758,59.55109;Float;True;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.FractNode;69;-3269.425,-928.7455;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;-3360.456,-695.0837;Float;False;Property;_Thickness;Thickness;11;0;Create;True;0;0;False;0;0;0.5;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;23;-3070.463,95.95244;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;18;-2837.903,47.83679;Float;False;Ygradient;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StepOpNode;71;-2995.403,-893.1048;Float;True;2;0;FLOAT;0;False;1;FLOAT;0.5;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;35;-4297.715,775.6197;Float;False;1830.896;777.8452;Comment;10;19;30;25;27;31;26;33;34;28;32;Opacity_Mask;1,1,1,1;0;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;12;-2713.067,-886.7184;Float;True;Noise;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;19;-4247.715,825.6198;Float;True;18;Ygradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;66;-2495.559,-1916.695;Float;False;1545.265;501.0595;Comment;8;58;57;59;63;62;65;64;60;Vert Offset;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;58;-2445.559,-1663.332;Float;False;18;Ygradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;42;-4197.699,-2010.607;Float;False;1392.357;560.6863;Comment;6;37;36;38;39;41;40;Emission;1,1,1,1;0;0
Node;AmplifyShaderEditor.PosVertexDataNode;57;-2437.504,-1866.695;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;30;-4161.33,1258.292;Float;True;18;Ygradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;25;-3922.448,832.4151;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;27;-3941.272,1162.324;Float;False;12;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;36;-4142.248,-1889.139;Float;True;12;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;59;-2077.088,-1778.101;Float;True;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;31;-3729.052,1288.198;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;53;-2080.663,-1021.676;Float;False;1086.764;729.8159;Comment;5;46;44;45;47;48;Albedo;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;37;-4147.699,-1679.921;Float;True;18;Ygradient;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;63;-2081.598,-1530.635;Float;False;Property;_VertOffsetStrength;VertOffsetStrength;13;0;Create;True;0;0;False;0;0;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;26;-3565.381,903.4099;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;32;-3168.182,1118.32;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;62;-1748.727,-1739.387;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SamplerNode;46;-2009.207,-761.3665;Float;True;Property;_AmbientOcclusion;Ambient Occlusion;6;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;33;-3178.091,1300.465;Float;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;45;-2030.663,-971.6756;Float;True;Property;_Albedo;Albedo;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.GetLocalVarNode;65;-1633.123,-1545.558;Float;False;12;Noise;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;38;-3778.148,-1738.365;Float;True;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;44;-1912.388,-498.8597;Float;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;40;-3610.848,-1960.608;Float;False;Property;_GlowColour;Glow Colour;7;1;[HDR];Create;True;0;0;False;0;0.4558824,1,1,0;0.4558824,1,1,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;64;-1413.423,-1753.558;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;41;-3338.212,-1707.267;Float;True;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;47;-1531.279,-779.3552;Float;False;3;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;34;-2974.808,1178.915;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;51;-789.3439,-966.092;Float;True;Property;_NormalMap;NormalMap;5;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RegisterLocalVarNode;52;-329.8356,-886.2859;Float;True;NormalMap;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;48;-1236.898,-771.1417;Float;False;Albedo;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;39;-3048.344,-1738.821;Float;True;Emission;-1;True;1;0;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;60;-1193.294,-1786.083;Float;True;VertOffset;-1;True;1;0;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;28;-2709.815,1022.633;Float;True;Opacity_Mask;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;56;-517.886,134.0329;Float;False;Property;_Smoothness;Smoothness;3;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;13;-222.0522,37.31019;Float;False;39;Emission;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;55;-515.7559,62.51602;Float;False;Property;_Metallic;Metallic;4;0;Create;True;0;0;False;0;0;0;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;50;-219.9753,-114.0796;Float;False;48;Albedo;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.GetLocalVarNode;61;-242.6459,521.9119;Float;False;60;VertOffset;1;0;OBJECT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.GetLocalVarNode;29;-297.0686,242.8914;Float;True;28;Opacity_Mask;1;0;OBJECT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.GetLocalVarNode;54;-223.9805,-29.93287;Float;False;52;NormalMap;1;0;OBJECT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;47.40138,-7.900229;Float;False;True;2;Float;ASEMaterialInspector;0;0;Standard;Teleport/Teleport 2;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;True;0;True;Transparent;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;20;0;15;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;68;0;22;0
WireConnection;68;1;67;0
WireConnection;73;1;76;0
WireConnection;16;0;20;2
WireConnection;16;1;17;0
WireConnection;1;0;70;0
WireConnection;1;1;73;0
WireConnection;21;0;16;0
WireConnection;21;1;68;0
WireConnection;69;0;1;2
WireConnection;23;0;21;0
WireConnection;18;0;23;0
WireConnection;71;0;69;0
WireConnection;71;1;72;0
WireConnection;12;0;71;0
WireConnection;25;0;19;0
WireConnection;59;0;57;0
WireConnection;59;1;58;0
WireConnection;31;0;30;0
WireConnection;26;0;25;0
WireConnection;26;1;27;0
WireConnection;32;0;26;0
WireConnection;32;1;31;0
WireConnection;62;0;59;0
WireConnection;62;1;63;0
WireConnection;33;0;31;0
WireConnection;38;0;36;0
WireConnection;38;1;37;0
WireConnection;64;0;62;0
WireConnection;64;1;65;0
WireConnection;41;0;40;0
WireConnection;41;1;38;0
WireConnection;47;0;45;0
WireConnection;47;1;46;0
WireConnection;47;2;44;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;52;0;51;0
WireConnection;48;0;47;0
WireConnection;39;0;41;0
WireConnection;60;0;64;0
WireConnection;28;0;34;0
WireConnection;0;0;50;0
WireConnection;0;1;54;0
WireConnection;0;2;13;0
WireConnection;0;3;55;0
WireConnection;0;4;56;0
WireConnection;0;10;29;0
WireConnection;0;11;61;0
ASEEND*/
//CHKSM=B7A92F2DD75916138EFE9596F96694BEB4CEEAE6
Shader "Unlit/RainbowAlienBeam"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
        _Transparency("Transparency", Range(0,1)) = 0.5
        _FlowMap("Flow Map",2D) = "white"{}
        _Speed("Speed",Range(0,20)) = 1
       _VerticalStep("Vertical Step",Int) = 1
       _FresnelPower("Fresnel Power", Range(1,10)) = 5
       [HDR]_FresnelColor("Fresnel Power", Color) = (1,1,1,1)
       [HDR]_MainColor("Main Color", Color) = (1,1,1,1)
       _EmissionPower("Emission Power", Range(1,100)) = 1
       _Distortion("Distortion", Range(0,1)) = 0

	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100
        Cull Off
     
      Pass{
        ZWrite On
        ColorMask 0
      }
		Pass
		{
             ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
	
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
                float2 uv1 : TEXCOORD3;
				float4 vertex : SV_POSITION;
                float3 worldVertex : TEXCOORD1;
                float3 worldNormal:TEXCOORD2;
			};

			sampler2D _MainTex;
            sampler2D _FlowMap;
			float4 _MainTex_ST;
            float4 _FlowMap_ST;
            float _Transparency;
            float _Speed;
            float _VerticalStep;
            float _FresnelPower;
            float4 _FresnelColor;
            float4 _MainColor;
            float _EmissionPower;
            float _Distortion;


            half3 HueToRGB(half h)
            {
                h = frac(h);
                half r = abs(h * 6 - 3) - 1;
                half g = 2 - abs(h * 6 - 2);
                half b = 2 - abs(h * 6 - 4);
                half3 rgb = saturate(half3(r, g, b));
#if UNITY_COLORSPACE_GAMMA
                return rgb;
#else
                return GammaToLinearSpace(rgb);
#endif
            }

			v2f vert (appdata v)
			{
				v2f o;
           

                o.worldVertex = mul(unity_ObjectToWorld,v.vertex).xyz;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.uv1 = TRANSFORM_TEX(v.uv,_FlowMap);
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
			
				return o;
			}
            float2 FlowUV(float2 flowVal, float2 uv, float time){
                return uv-flowVal*time;
            }

			fixed4 frag (v2f i, float facing :VFACE) : SV_Target
			{
				// sample the texture
                float t = _Time.y*_Speed;
                float3 eyeDir = normalize(_WorldSpaceCameraPos - i.worldVertex);
                float EDotN = max(dot(eyeDir,i.worldNormal),0);
                float fresnel = pow(1.0-EDotN,_FresnelPower);
				fixed4 col;
                float moveDir;
                if(facing>0){
                    moveDir = atan2(i.worldVertex.z,i.worldVertex.x);
                }else{
                    moveDir =-i.worldVertex.y;
                }
                float vStep = frac(_VerticalStep* (atan2(i.worldVertex.z+i.worldVertex.y,i.worldVertex.x+i.worldVertex.y)+t));
               
                col.rgb = (_MainColor.rgb*_EmissionPower)+(fresnel*_FresnelColor);
                if(facing<0){
                    //col.rgb= (tex2D(_MainTex,float2(i.uv.x,i.uv.y+_Time.y)).rgb);
                     //float u = cos(i.uv.x+t)*.5+.5;
                     //float v = sin(i.uv.y+t)*.5+.5;
                     //col.rgb = saturate(HueToRGB(sin(vStep)));
                     float2 uvVec = tex2D(_FlowMap,float2(i.uv1.x,i.uv1.y+t))*2-1;
                     uvVec = i.uv+uvVec*_Distortion;
                     uvVec.y+=t;
                     col.rgb = saturate(HueToRGB(sin(vStep)))*tex2D(_MainTex,uvVec);
                 }
               
				// apply fog
	
      
                col.a = _Transparency;
				return col;
			}
			ENDCG
		}
        
	}
}

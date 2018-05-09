Shader "EffectL/SphereModify"
{
	Properties
	{
		_MainColor("Main Color",Color)=(1,1,1,1)
		_Force("Force",Float)=0.1
	}
	SubShader 
	{
		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		float4 _MainColor;
		float4 _WorldForcePos;
		float _Force;
		float _MaxForce;
		float _Spring;
		float _Damping;
		float _StartTime;
		ENDCG
		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			CGPROGRAM
			#pragma multi_compile_fwdbase
			struct VInput
			{
				float4 pos:POSITION;
				float3 nor:NORMAL;
				float3 LightDir:TEXCOORD1;
				float4 texcoord:TEXCOORD0;  
				float4 wpos:TEXCOORD2;
			};
			struct FInput
			{
				float4 texcoord :TEXCOORD0;
				float3 nor:NORMAL;
				float3 LightDir:TEXCOORD1;
				float4 pos:SV_POSITION;
			};

			float4 ModifyPos(float4 pos,in float3 normal)
			{
				float3 dir=pos.xyz-_WorldForcePos.xyz;
				float dis=dot(dir,dir);
				float singleForce=_Force/(1+dis);
				dir=normalize(dir);
				float time=_Time.y-_StartTime;
				float offset=sin(_Spring*time)*max(0,(singleForce-_Damping*time));
				normal=lerp(normal,-dir,saturate(offset/_Force));
				return half4(pos.xyz+dir*offset,1);
			}

			FInput vert(VInput v)
			{
				FInput o;
				o.texcoord=v.texcoord;
				o.LightDir=WorldSpaceLightDir(v.pos);
				v.wpos=mul(unity_ObjectToWorld,v.pos);
			    _WorldForcePos=mul(unity_ObjectToWorld,float4(0,0,-1,1));
				float3 normal=mul((float3x3)unity_ObjectToWorld,v.nor);
			    float4 w= ModifyPos(v.wpos,normal);
			    w=UnityObjectToClipPos(mul(unity_WorldToObject,w));
			    o.pos=w;
			    o.nor=normal;
			    return o;
			}
			float4 frag(FInput i):COLOR 
			{
				float3 albedo=_MainColor.rgb;
				float3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				float3 L=normalize(i.LightDir);
				float3 diffuse=_LightColor0.rgb*albedo*max(0,dot(i.nor,L));
				float3 finalcolor=diffuse+ambient;
				return float4(finalcolor,1);
			}
			ENDCG
		}
	}
}
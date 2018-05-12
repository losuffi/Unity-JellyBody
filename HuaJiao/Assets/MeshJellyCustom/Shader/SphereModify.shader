// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

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
		#pragma multi_compile _IsWave _Explore
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		int _Count;
		float4 _pAfs[10];
		float4 _dAts[10];
		float4 _MainColor;
		float _MaxForce;
		float _Spring;
		float _Damping;
		float _Namida;
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
			struct result
			{
				float4 resPos;
				float3 resNormal;
			};
			struct dataBuff
			{
				float _StartTime;
				float _Force;
				float _Namida;
				float3 _ForceDir;
				float4 _WorldForcePos;
			};
			dataBuff getData(int i)
			{
				dataBuff o;
				o._StartTime=_dAts[i].w;
				o._Force=_pAfs[i].w;
				
				o._ForceDir=mul((float3x3)unity_WorldToObject,_dAts[i].xyz);
				o._WorldForcePos=mul(unity_WorldToObject,float4(_pAfs[i].xyz,1));
				o._Namida=_Namida;
				return o;
			}
			result ModifyPos(float4 pos,in float3 normal)
			{
				result r;
				float3 v;
				float3 n=normal;
				for(int i=0;i<_Count;i++)
				{
					dataBuff data=getData(i);
					float3 dir=pos.xyz-data._WorldForcePos.xyz;
					float3 wdir=mul((float3x3)unity_ObjectToWorld,dir);
					float distance=dot(dir,dir);
					float time=_Time.y-data._StartTime;
					float singleForce=data._Force/(1+distance*5);
					float A=lerp(singleForce,0,saturate((_Damping*time)/abs(singleForce)));
					//float speed=data._Namida* (4*3.14*3.14)/_Spring;
					//A=(speed*speed*time*time)>distance?A:0;	
					float offset=(cos(_Spring*time))*-A;			 	
					v+=dir*offset;
					float3 binormal=cross(normal,wdir);
					float3 tangent=cross(binormal,normal);
					n+=offset*tangent*5;
				}
				r.resPos=half4(pos.xyz+v,1);
				r.resNormal=normalize(n);
				return r;
			}

			result WaveModify(float4 pos,in float3 normal)
			{
				result r;
				float3 v;
				float3 n=normal;
				for(int i=0;i<_Count;i++){
					dataBuff data=getData(i);
					float3 dir=pos.xyz-data._WorldForcePos.xyz;
					float3 wdir=mul((float3x3)unity_ObjectToWorld,dir);
					float distance=dot(dir,dir);
					float time=_Time.y-data._StartTime;
					float singleForce=data._Force/(1+distance*5);
					float A=lerp(singleForce,0,saturate(((_Damping)*time)/abs(singleForce)));
					float x=time-distance/data._Namida;
					//x=x<0?0:x;
					float speed=data._Namida* (4*3.14*3.14)/_Spring;
					A=(speed*speed*time*time)>distance?A:0;	
					float h=-A*(cos(_Spring*(x)))*0.1;
					v+=h*normal;
					float3 binormal=cross(normal,wdir);
					float3 tangent=cross(binormal,normal);
					n+=h*tangent*20;
				}
				n=normalize(n);
				r.resPos=float4(pos.xyz+v,1);
				r.resNormal=n;
				return r;
			}


			FInput vert(VInput v)
			{
				FInput o;
				o.texcoord=v.texcoord;
				o.LightDir=WorldSpaceLightDir(v.pos);
				v.wpos=mul(unity_ObjectToWorld,v.pos);
				float3 normal=mul((float3x3)unity_ObjectToWorld,v.nor);
				#ifdef _IsWave
			    result w= WaveModify(v.pos,normal);
				#elif _Explore
				result w= ModifyPos(v.pos,normal);
				#else
				result w= ModifyPos(v.pos,normal);
				#endif
			    o.pos=UnityObjectToClipPos(w.resPos);
			    o.nor=w.resNormal;
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
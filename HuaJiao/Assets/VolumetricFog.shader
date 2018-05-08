// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "EffectL/VolumeFog"
{
	Properties
	{
		_BurnAmount("Burn Amount",Range(0.0,1.0))=0.0
		_LineWidth("Burn Line Width",Range(0.0,0.2))=0.1
		_MainColor("Main Color",Color)=(1,1,1,1)
		_BurnMap("Burn Map",2D)="white"{}
	}
	SubShader
	{
		CGINCLUDE
		#pragma vertex vert
		#pragma fragment frag
		#include "Lighting.cginc"
		#include "AutoLight.cginc"
		float _BurnAmount;
		float _LineWidth;
		float4 _MainColor;
		sampler2D _BurnMap;
		float4 _BurnMap_ST;
		ENDCG
		Pass
		{
			Tags{"LightMode"="ForwardBase"}
			Cull off
            Blend SrcAlpha OneMinusSrcAlpha
			CGPROGRAM
			#pragma multi_compile_fwdbase
			struct a2v
			{
				float4 vertex:POSITION;
				float4 texcoord:TEXCOORD0;
				float4 tangent:TANGENT;
				float3 normal:NORMAL;
			};
			struct v2f
			{
				float4 pos:SV_POSITION;
				float2 uvBurnMap:TEXCOORD3;
				float3 lightDir:TEXCOORD1;
				float3 worldPos:TEXCOORD2;
				float3 normal:NORMAL;
				SHADOW_COORDS(5)
			};
			v2f vert(a2v v)
			{
				v2f o;
				o.pos=UnityObjectToClipPos(v.vertex);
				o.uvBurnMap=TRANSFORM_TEX(v.texcoord,_BurnMap);
				TANGENT_SPACE_ROTATION;
				o.lightDir=mul(rotation,ObjSpaceLightDir(v.vertex)).xyz;
				o.normal=mul(rotation,v.normal);
				o.worldPos=mul(unity_ObjectToWorld,v.vertex).xyz;
				TRANSFER_SHADOW(o);
				return o;
			}
			fixed4 frag(v2f i):SV_Target
			{
				fixed3 burn=tex2D(_BurnMap,i.uvBurnMap).rgb;
				clip(burn.r-_BurnAmount);
				float3 tangentLightDir=normalize(i.lightDir);
				float3 tangentNormal=normalize(i.normal);
				float3 albedo=_MainColor.rgb;
				float3 ambient=UNITY_LIGHTMODEL_AMBIENT.xyz*albedo;
				float3 diffuse=_LightColor0.rgb*albedo*max(0,dot(tangentNormal,tangentLightDir));
				float t=1-smoothstep(0.0,_LineWidth,burn.r-_BurnAmount);
				UNITY_LIGHT_ATTENUATION(atten,i,i.worldPos);
				return fixed4(ambient+diffuse*atten,0.4);
				
			}
			ENDCG
		}
		Pass
		{
			Tags{"LightMode"="ShadowCaster"}
			CGPROGRAM
			#pragma multi_compile_shadowcaster
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 uvBurnMap:TEXCOORD1;
			};
			v2f vert(appdata_base v)
			{
				v2f o;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET(o)
				o.uvBurnMap=TRANSFORM_TEX(v.texcoord,_BurnMap);
				return o;
			}
			float4 frag(v2f i):SV_Target
			{
				float3 burn=tex2D(_BurnMap,i.uvBurnMap).rgb;
				clip(burn.r-_BurnAmount);
				SHADOW_CASTER_FRAGMENT(i)
			}
			ENDCG
		}	
	}
}

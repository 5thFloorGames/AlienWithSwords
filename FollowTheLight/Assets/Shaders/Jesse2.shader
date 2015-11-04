// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33433,y:32699,varname:node_9361,prsc:2|custl-9459-OUT;n:type:ShaderForge.SFN_Color,id:1686,x:31673,y:32135,ptovrint:False,ptlb:Shadow Color,ptin:_ShadowColor,varname:node_1686,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.2426471,c2:0.2426471,c3:0.2426471,c4:1;n:type:ShaderForge.SFN_Color,id:9965,x:31538,y:33016,ptovrint:False,ptlb:Diffuse Color,ptin:_DiffuseColor,varname:node_9965,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.9350913,c3:0.7647059,c4:1;n:type:ShaderForge.SFN_Blend,id:638,x:31875,y:32764,varname:node_638,prsc:2,blmd:10,clmp:True|SRC-1686-RGB,DST-9965-RGB;n:type:ShaderForge.SFN_LightAttenuation,id:8614,x:31538,y:32547,varname:node_8614,prsc:2;n:type:ShaderForge.SFN_Multiply,id:910,x:32203,y:32611,varname:node_910,prsc:2|A-8614-OUT,B-638-OUT,C-3365-RGB;n:type:ShaderForge.SFN_LightColor,id:3365,x:31564,y:32731,varname:node_3365,prsc:2;n:type:ShaderForge.SFN_OneMinus,id:9014,x:31812,y:32315,varname:node_9014,prsc:2|IN-8614-OUT;n:type:ShaderForge.SFN_ValueProperty,id:1241,x:31812,y:32482,ptovrint:False,ptlb:Value,ptin:_Value,varname:node_1241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:10;n:type:ShaderForge.SFN_Multiply,id:5600,x:32004,y:32361,varname:node_5600,prsc:2|A-9014-OUT,B-1241-OUT;n:type:ShaderForge.SFN_OneMinus,id:4293,x:32203,y:32361,varname:node_4293,prsc:2|IN-5600-OUT;n:type:ShaderForge.SFN_Vector4,id:993,x:32213,y:32257,varname:node_993,prsc:2,v1:0.5,v2:0.5,v3:0.5,v4:1;n:type:ShaderForge.SFN_Lerp,id:7420,x:32443,y:32183,varname:node_7420,prsc:2|A-1686-RGB,B-993-OUT,T-4293-OUT;n:type:ShaderForge.SFN_Blend,id:9091,x:32678,y:32462,varname:node_9091,prsc:2,blmd:10,clmp:True|SRC-7420-OUT,DST-910-OUT;n:type:ShaderForge.SFN_Dot,id:3831,x:32789,y:32901,varname:node_3831,prsc:2,dt:0|A-7400-OUT,B-2209-OUT;n:type:ShaderForge.SFN_LightVector,id:7400,x:32511,y:32857,varname:node_7400,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:2209,x:32496,y:33003,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:9459,x:32949,y:32742,varname:node_9459,prsc:2|A-9091-OUT,B-3831-OUT;proporder:1686-9965-1241;pass:END;sub:END;*/

Shader "Shader Forge/Jesse2" {
    Properties {
        _ShadowColor ("Shadow Color", Color) = (0.2426471,0.2426471,0.2426471,1)
        _DiffuseColor ("Diffuse Color", Color) = (1,0.9350913,0.7647059,1)
        _Value ("Value", Float ) = 10
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _ShadowColor;
            uniform float4 _DiffuseColor;
            uniform float _Value;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = (saturate(( (attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb) > 0.5 ? (1.0-(1.0-2.0*((attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb)-0.5))*(1.0-lerp(float4(_ShadowColor.rgb,0.0),float4(0.5,0.5,0.5,1),(1.0 - ((1.0 - attenuation)*_Value))))) : (2.0*(attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb)*lerp(float4(_ShadowColor.rgb,0.0),float4(0.5,0.5,0.5,1),(1.0 - ((1.0 - attenuation)*_Value)))) ))*dot(lightDirection,i.normalDir)).rgb;
                fixed4 finalRGBA = fixed4(finalColor,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma multi_compile_fog
            #pragma exclude_renderers gles3 metal d3d11_9x xbox360 xboxone ps3 ps4 psp2 
            #pragma target 3.0
            uniform float4 _ShadowColor;
            uniform float4 _DiffuseColor;
            uniform float _Value;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 posWorld : TEXCOORD0;
                float3 normalDir : TEXCOORD1;
                LIGHTING_COORDS(2,3)
                UNITY_FOG_COORDS(4)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.posWorld = mul(_Object2World, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                UNITY_TRANSFER_FOG(o,o.pos);
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
/////// Vectors:
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 finalColor = (saturate(( (attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb) > 0.5 ? (1.0-(1.0-2.0*((attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb)-0.5))*(1.0-lerp(float4(_ShadowColor.rgb,0.0),float4(0.5,0.5,0.5,1),(1.0 - ((1.0 - attenuation)*_Value))))) : (2.0*(attenuation*saturate(( _DiffuseColor.rgb > 0.5 ? (1.0-(1.0-2.0*(_DiffuseColor.rgb-0.5))*(1.0-_ShadowColor.rgb)) : (2.0*_DiffuseColor.rgb*_ShadowColor.rgb) ))*_LightColor0.rgb)*lerp(float4(_ShadowColor.rgb,0.0),float4(0.5,0.5,0.5,1),(1.0 - ((1.0 - attenuation)*_Value)))) ))*dot(lightDirection,i.normalDir)).rgb;
                fixed4 finalRGBA = fixed4(finalColor * 1,0);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}

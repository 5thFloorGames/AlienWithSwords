// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33669,y:32572,varname:node_9361,prsc:2|custl-3130-OUT;n:type:ShaderForge.SFN_Dot,id:7483,x:32287,y:32943,varname:node_7483,prsc:2,dt:0|A-3516-OUT,B-6676-OUT;n:type:ShaderForge.SFN_LightVector,id:3516,x:32110,y:32865,varname:node_3516,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:6676,x:32110,y:33070,prsc:2,pt:False;n:type:ShaderForge.SFN_Color,id:5312,x:32778,y:32449,ptovrint:False,ptlb:Base Color,ptin:_BaseColor,varname:node_5312,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_LightAttenuation,id:2798,x:32135,y:32452,varname:node_2798,prsc:2;n:type:ShaderForge.SFN_Multiply,id:566,x:32472,y:32776,varname:node_566,prsc:2|A-2798-OUT,B-7483-OUT,C-600-RGB;n:type:ShaderForge.SFN_LightColor,id:600,x:32146,y:32616,varname:node_600,prsc:2;n:type:ShaderForge.SFN_Vector1,id:4692,x:32616,y:32681,varname:node_4692,prsc:2,v1:0;n:type:ShaderForge.SFN_Step,id:1427,x:32783,y:32793,varname:node_1427,prsc:2|A-4692-OUT,B-566-OUT;n:type:ShaderForge.SFN_Noise,id:4343,x:32528,y:31927,varname:node_4343,prsc:2|XY-138-OUT;n:type:ShaderForge.SFN_Time,id:4175,x:32034,y:32020,varname:node_4175,prsc:2;n:type:ShaderForge.SFN_Append,id:138,x:32325,y:32023,varname:node_138,prsc:2|A-4175-T,B-8138-X;n:type:ShaderForge.SFN_FragmentPosition,id:8138,x:32204,y:31851,varname:node_8138,prsc:2;n:type:ShaderForge.SFN_Color,id:7703,x:32549,y:31750,ptovrint:False,ptlb:Gray,ptin:_Gray,varname:node_7703,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.8088235,c2:0.8088235,c3:0.8088235,c4:1;n:type:ShaderForge.SFN_Add,id:7061,x:32733,y:31915,varname:node_7061,prsc:2|A-7703-RGB,B-4343-OUT;n:type:ShaderForge.SFN_Step,id:7779,x:32776,y:33057,varname:node_7779,prsc:2|A-2819-OUT,B-566-OUT;n:type:ShaderForge.SFN_Vector1,id:2819,x:32511,y:33013,varname:node_2819,prsc:2,v1:0.2;n:type:ShaderForge.SFN_Color,id:8363,x:32864,y:33296,ptovrint:False,ptlb:Light Color,ptin:_LightColor,varname:node_8363,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.8615619,c3:0.7132353,c4:1;n:type:ShaderForge.SFN_Multiply,id:4009,x:33011,y:33057,varname:node_4009,prsc:2|A-7779-OUT,B-8363-RGB;n:type:ShaderForge.SFN_Multiply,id:5840,x:33036,y:32598,varname:node_5840,prsc:2|A-5312-RGB,B-1427-OUT;n:type:ShaderForge.SFN_Add,id:1791,x:33255,y:32737,varname:node_1791,prsc:2|A-5840-OUT,B-4009-OUT;n:type:ShaderForge.SFN_Multiply,id:3130,x:33453,y:32555,varname:node_3130,prsc:2|A-7061-OUT,B-1791-OUT;proporder:5312-7703-8363;pass:END;sub:END;*/

Shader "Shader Forge/Jesse3" {
    Properties {
        _BaseColor ("Base Color", Color) = (1,0,0,1)
        _Gray ("Gray", Color) = (0.8088235,0.8088235,0.8088235,1)
        _LightColor ("Light Color", Color) = (1,0.8615619,0.7132353,1)
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
            uniform float4 _TimeEditor;
            uniform float4 _BaseColor;
            uniform float4 _Gray;
            uniform float4 _LightColor;
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
                float4 node_4175 = _Time + _TimeEditor;
                float2 node_138 = float2(node_4175.g,i.posWorld.r);
                float2 node_4343_skew = node_138 + 0.2127+node_138.x*0.3713*node_138.y;
                float2 node_4343_rnd = 4.789*sin(489.123*(node_4343_skew));
                float node_4343 = frac(node_4343_rnd.x*node_4343_rnd.y*(1+node_4343_skew.x));
                float3 node_566 = (attenuation*dot(lightDirection,i.normalDir)*_LightColor0.rgb);
                float3 finalColor = ((_Gray.rgb+node_4343)*((_BaseColor.rgb*step(0.0,node_566))+(step(0.2,node_566)*_LightColor.rgb)));
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
            uniform float4 _TimeEditor;
            uniform float4 _BaseColor;
            uniform float4 _Gray;
            uniform float4 _LightColor;
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
                float4 node_4175 = _Time + _TimeEditor;
                float2 node_138 = float2(node_4175.g,i.posWorld.r);
                float2 node_4343_skew = node_138 + 0.2127+node_138.x*0.3713*node_138.y;
                float2 node_4343_rnd = 4.789*sin(489.123*(node_4343_skew));
                float node_4343 = frac(node_4343_rnd.x*node_4343_rnd.y*(1+node_4343_skew.x));
                float3 node_566 = (attenuation*dot(lightDirection,i.normalDir)*_LightColor0.rgb);
                float3 finalColor = ((_Gray.rgb+node_4343)*((_BaseColor.rgb*step(0.0,node_566))+(step(0.2,node_566)*_LightColor.rgb)));
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

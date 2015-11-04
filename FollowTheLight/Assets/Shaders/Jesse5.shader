// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33209,y:32712,varname:node_9361,prsc:2|custl-6192-OUT;n:type:ShaderForge.SFN_LightVector,id:9569,x:31753,y:32999,varname:node_9569,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:4435,x:31765,y:33156,prsc:2,pt:False;n:type:ShaderForge.SFN_LightAttenuation,id:6973,x:31898,y:32701,varname:node_6973,prsc:2;n:type:ShaderForge.SFN_LightColor,id:2998,x:31898,y:32568,varname:node_2998,prsc:2;n:type:ShaderForge.SFN_Multiply,id:842,x:32224,y:32675,varname:node_842,prsc:2|A-1310-RGB,B-2998-RGB,C-6973-OUT;n:type:ShaderForge.SFN_Dot,id:3029,x:32062,y:32972,varname:node_3029,prsc:2,dt:1|A-9569-OUT,B-4435-OUT;n:type:ShaderForge.SFN_Color,id:1310,x:31898,y:32395,ptovrint:False,ptlb:Light color,ptin:_Lightcolor,varname:node_1310,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Color,id:9474,x:32374,y:32490,ptovrint:False,ptlb:Shadow color,ptin:_Shadowcolor,varname:node_9474,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:1,c4:1;n:type:ShaderForge.SFN_Lerp,id:6192,x:32615,y:32768,varname:node_6192,prsc:2|A-9474-RGB,B-842-OUT,T-3029-OUT;n:type:ShaderForge.SFN_Tex2d,id:5624,x:31991,y:33254,ptovrint:False,ptlb:Grunge,ptin:_Grunge,varname:node_5624,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:2ce04796d78bfa64b92a68735afb4be5,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Add,id:5327,x:32262,y:33151,varname:node_5327,prsc:2|A-3029-OUT,B-5624-RGB;n:type:ShaderForge.SFN_Multiply,id:8878,x:32807,y:32981,varname:node_8878,prsc:2|A-6192-OUT,B-3524-OUT;n:type:ShaderForge.SFN_Vector1,id:3629,x:32205,y:33504,varname:node_3629,prsc:2,v1:5;n:type:ShaderForge.SFN_Blend,id:3524,x:32518,y:33256,varname:node_3524,prsc:2,blmd:15,clmp:True|SRC-5327-OUT,DST-3029-OUT;n:type:ShaderForge.SFN_Color,id:6711,x:32262,y:33341,ptovrint:False,ptlb:node_6711,ptin:_node_6711,varname:node_6711,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.5073529,c2:0.5073529,c3:0.5073529,c4:1;proporder:1310-9474-5624-6711;pass:END;sub:END;*/

Shader "Shader Forge/Jesse4" {
    Properties {
        _Lightcolor ("Light color", Color) = (1,0,0,1)
        _Shadowcolor ("Shadow color", Color) = (0,0,1,1)
        _Grunge ("Grunge", 2D) = "white" {}
        _node_6711 ("node_6711", Color) = (0.5073529,0.5073529,0.5073529,1)
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
            uniform float4 _Lightcolor;
            uniform float4 _Shadowcolor;
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
                float node_3029 = max(0,dot(lightDirection,i.normalDir));
                float3 node_6192 = lerp(_Shadowcolor.rgb,(_Lightcolor.rgb*_LightColor0.rgb*attenuation),node_3029);
                float3 finalColor = node_6192;
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
            uniform float4 _Lightcolor;
            uniform float4 _Shadowcolor;
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
                float node_3029 = max(0,dot(lightDirection,i.normalDir));
                float3 node_6192 = lerp(_Shadowcolor.rgb,(_Lightcolor.rgb*_LightColor0.rgb*attenuation),node_3029);
                float3 finalColor = node_6192;
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

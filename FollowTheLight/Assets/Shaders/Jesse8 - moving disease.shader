// Shader created with Shader Forge v1.18 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.18;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,rpth:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:9361,x:33418,y:32828,varname:node_9361,prsc:2|emission-7950-OUT,custl-7095-OUT;n:type:ShaderForge.SFN_Tex2d,id:438,x:32470,y:32622,ptovrint:False,ptlb:Diffuse Texture,ptin:_DiffuseTexture,varname:node_438,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:36598527bc646ef4898830c5369799e4,ntxv:0,isnm:False;n:type:ShaderForge.SFN_NormalVector,id:1814,x:32011,y:32855,prsc:2,pt:False;n:type:ShaderForge.SFN_Dot,id:3902,x:32200,y:32941,varname:node_3902,prsc:2,dt:1|A-1814-OUT,B-4880-OUT;n:type:ShaderForge.SFN_LightVector,id:4880,x:32011,y:33042,varname:node_4880,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:2998,x:32200,y:33106,varname:node_2998,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:9897,x:32282,y:33628,ptovrint:False,ptlb:Shadow Noise,ptin:_ShadowNoise,varname:node_9897,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,tex:488386bd12e89f448bd2c1fdfbbab08c,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:753,x:31952,y:33569,ptovrint:False,ptlb:Shadow intensity,ptin:_Shadowintensity,varname:node_753,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.4188034,max:1;n:type:ShaderForge.SFN_Multiply,id:5493,x:32633,y:33377,varname:node_5493,prsc:2|A-2492-OUT,B-9897-RGB;n:type:ShaderForge.SFN_OneMinus,id:2492,x:32633,y:33203,varname:node_2492,prsc:2|IN-3970-OUT;n:type:ShaderForge.SFN_Step,id:7436,x:33091,y:33364,varname:node_7436,prsc:2|A-7420-OUT,B-3553-OUT;n:type:ShaderForge.SFN_Slider,id:3553,x:32566,y:33784,ptovrint:False,ptlb:Shadow cutoff,ptin:_Shadowcutoff,varname:node_3553,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.7932656,max:1;n:type:ShaderForge.SFN_Clamp01,id:7420,x:32885,y:33475,varname:node_7420,prsc:2|IN-5018-OUT;n:type:ShaderForge.SFN_Color,id:456,x:32470,y:32809,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_456,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:0.4852941,c3:0.9858013,c4:1;n:type:ShaderForge.SFN_Add,id:5018,x:32644,y:33569,varname:node_5018,prsc:2|A-5493-OUT,B-753-OUT;n:type:ShaderForge.SFN_LightColor,id:5284,x:32200,y:33232,varname:node_5284,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7095,x:32902,y:32851,varname:node_7095,prsc:2|A-438-RGB,B-6568-OUT,C-7436-OUT;n:type:ShaderForge.SFN_Multiply,id:6568,x:32442,y:32972,varname:node_6568,prsc:2|A-3902-OUT,B-2998-OUT,C-5284-RGB;n:type:ShaderForge.SFN_Desaturate,id:3970,x:32459,y:33158,varname:node_3970,prsc:2|COL-6568-OUT,DES-2532-OUT;n:type:ShaderForge.SFN_Vector1,id:2532,x:32281,y:33402,varname:node_2532,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:7950,x:32877,y:32651,varname:node_7950,prsc:2|A-8993-RGB,B-438-RGB;n:type:ShaderForge.SFN_AmbientLight,id:8993,x:32694,y:32406,varname:node_8993,prsc:2;proporder:438-9897-753-3553-456;pass:END;sub:END;*/

Shader "Shader Forge/Jesse6" {
    Properties {
        _DiffuseTexture ("Diffuse Texture", 2D) = "white" {}
        _ShadowNoise ("Shadow Noise", 2D) = "white" {}
        _Shadowintensity ("Shadow intensity", Range(0, 1)) = 0.4188034
        _Shadowcutoff ("Shadow cutoff", Range(0, 1)) = 0.7932656
        _Color ("Color", Color) = (1,0.4852941,0.9858013,1)
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
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform sampler2D _ShadowNoise; uniform float4 _ShadowNoise_ST;
            uniform float _Shadowintensity;
            uniform float _Shadowcutoff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
////// Emissive:
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float3 emissive = (UNITY_LIGHTMODEL_AMBIENT.rgb*_DiffuseTexture_var.rgb);
                float3 node_6568 = (max(0,dot(i.normalDir,lightDirection))*attenuation*_LightColor0.rgb);
                float4 _ShadowNoise_var = tex2D(_ShadowNoise,TRANSFORM_TEX(i.uv0, _ShadowNoise));
                float3 finalColor = emissive + (_DiffuseTexture_var.rgb*node_6568*step(saturate((((1.0 - lerp(node_6568,dot(node_6568,float3(0.3,0.59,0.11)),1.0))*_ShadowNoise_var.rgb)+_Shadowintensity)),_Shadowcutoff));
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
            uniform sampler2D _DiffuseTexture; uniform float4 _DiffuseTexture_ST;
            uniform sampler2D _ShadowNoise; uniform float4 _ShadowNoise_ST;
            uniform float _Shadowintensity;
            uniform float _Shadowcutoff;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                LIGHTING_COORDS(3,4)
                UNITY_FOG_COORDS(5)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
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
                float4 _DiffuseTexture_var = tex2D(_DiffuseTexture,TRANSFORM_TEX(i.uv0, _DiffuseTexture));
                float3 node_6568 = (max(0,dot(i.normalDir,lightDirection))*attenuation*_LightColor0.rgb);
                float4 _ShadowNoise_var = tex2D(_ShadowNoise,TRANSFORM_TEX(i.uv0, _ShadowNoise));
                float3 finalColor = (_DiffuseTexture_var.rgb*node_6568*step(saturate((((1.0 - lerp(node_6568,dot(node_6568,float3(0.3,0.59,0.11)),1.0))*_ShadowNoise_var.rgb)+_Shadowintensity)),_Shadowcutoff));
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

Shader "Rogue/Environment/StylizedBGShader"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _BaseMap ("Main Texture (RGB : BaseColor Texture, A : AlphaCutout) ", 2D) = "white" {}
        
        [Header(Shadow)]
        [Space(10)]
        _ReceiveShadowAmount ("Receive Shadow Amount", Range(0,1)) = 1
        //_ReceiveShadowDarkness ("Receive Shadow Darkness", Range(0,1)) = 0
        _ShadowThreshold("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmooth ("Shadow Smooth", Range(0,0.5)) = 0.5
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)

        [Header(Normal)]
        [Space(10)]
        [Toggle]_UseNormalTex ("Use Normal Texture", float) = 0
        [NoScaleOffset]_BumpMap ("Normal Texture", 2D) = "bump" {}
        _BumpScale ("Normal Strength", float) = 1

        [Space(20)]
        
        _MaskTex ("Mask Texture (R:Metallic, G:Roughness, B:Occlusion)", 2D) = "white" {}


        [Header(Stylized PBR)]
        [Space(10)]
        _RoughnessValue ("Roughness", Range(0,1)) = 1
        _MetallicValue ("Metallic ", Range(0,1)) = 1
        _AoStrength("AO Strength", Range(0, 2)) = 1.0
        //[Toggle]_VertexColorAO ("Vertex Color AO", float) = 0
        
        [Space(10)]
        _FresnelThreshold("Fresnel Threshold", Range(0,1)) = 0.8
        _FresnelSmooth("Fresnel Smooth", Range(0,0.5)) = 0.25
        _FresnelIntensity("Fresnel Intensity", Range(0,2)) = 1
        [Space(10)]
        _CubemapIntensity ("Cubemap Intensity", Range(0,10)) = 1
        _CubemapBlurAmount ("Cubemap Blur Amount", Range(0,1)) = 1
        [Space(10)]
        _SpecularIntensity("Specular Intensity", Range(0,10)) = 1
        [Space(10)]
        _GIIntensity("GI Intensity", Range(0,2)) = 1

        
        [Header(Emissive)]
        [Space(10)]
        [Toggle] _USEEMISSIVE ("Use Emissive", float) = 0
        _EmissiveMap ("Emissive Color Texture (RGB:Color, A:Mask)", 2D) = "black"{}
        _EmissionColor ("Emissive Color", Color) = (1,1,1,1)
        _EmissiveEdgeSmooth ("Emissive Mask Edge Smooth", float) = 1
        _EmissiveIntensity ("Emissive Intensity", float) = 1

        [Header(Height)]
        _HeightTex ("Height Texture", 2D) = "gray"{}
        _ParallaxAmplitude ("Parallax Amplitude", Range(0,2)) = 0
        _VertexOffset ("Vertex Offset", float) = 0
        
        
        [Header(Alpha Culling)]
        [Space(10)]
        [Toggle] _AlphaTest ("AlphaTest", float) = 0
        _AlphaCutout ("Alpha Cutout", Range(0,1)) = 0
        [Space(10)]
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Culling", float) = 2

        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}




    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "Queue"="Geometry+0"
        }
        LOD 100

        Pass
        {
            Name "Universal Forward"
            Tags 
            { 
                "LightMode" = "UniversalForward"
            }
            Cull [_Cull]
            ZTest LEqual


            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"
            
             // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile_fog

            // lightmap
            #pragma multi_compile _ DIRLIGHTMAP_COMBINED
            #pragma multi_compile _ LIGHTMAP_ON
            #pragma multi_compile _ LIGHTMAP_SHADOW_MIXING
            #pragma multi_compile _ SHADOWS_SHADOWMASK

            // Recieve Shadow
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            //#pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS 
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma multi_compile _ _USENORMALTEX_ON
            #pragma multi_compile _ _USEEMISSIVE_ON

            #pragma multi_compile _ _FOG_HEIGHT

            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   
            TEXTURE2D(_BumpMap);   SAMPLER(sampler_BumpMap);  
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex); 
            TEXTURE2D(_EmissiveMap);   SAMPLER(sampler_EmissiveMap); 
            TEXTURE2D(_HeightTex);  SAMPLER(sampler_HeightTex);
            CBUFFER_START(UnityPerMaterial)    

            
            half4 _MaskTex_ST;
            half4 _BaseMap_ST;
            half4 _EmissiveMap_ST;
            
            float4 _Color, _EmissionColor;
            float4 _ShadowColor;
            
            
            
            float _ReceiveShadowAmount;
            float _ShadowThreshold, _ShadowSmooth;
            float _BumpScale;
            float _EmissiveEdgeSmooth, _EmissiveIntensity;
            float _MetallicValue;
            float _RoughnessValue;
            float _AoStrength;
            float _GIIntensity;
            float _CubemapIntensity, _CubemapBlurAmount;
            float _FresnelThreshold, _FresnelSmooth, _FresnelIntensity;
            float _SpecularIntensity;
            
            float _AlphaCutout;
            //float _VertexColorAO;

            //float _ReceiveShadowDarkness;

            float _ParallaxAmplitude;
            float _VertexOffset;


            CBUFFER_END

            half _FogOffset;
            half _FogHeight;
            half _HeightFogAmount;
            half _FogThreshold;
            half _FogSmooth;
            TEXTURE2D(_GradientTex); SAMPLER(sampler_GradientTex);

            

            

            struct appdata
            {
                float4 color : COLOR0;
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID                                
            };


            struct v2f
            {
                float4 color : COLOR0;
                float4 positionCS : SV_POSITION;
                
                float2 uv : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
                
                float4 tangentWS : TEXCOORD2;
                float4 bitangentWS : TEXCOORD3;
                float4 normalWS : TEXCOORD4;  
           
                //float3 viewDirWS : TEXCOORD4;
                float3 positionWS : TEXCOORD5;

                float4 fogFactorAndVertexLight  : TEXCOORD6;
                float4 shadowCoord : TEXCOORD7;
                float3 viewDirTS : TEXCOORD8;
                
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO

            };



            


            void InitializeInputData(v2f input, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;

            #if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
                inputData.positionWS = input.positionWS;
            #endif

        
                half3 viewDirWS = half3(input.normalWS.w, input.tangentWS.w, input.bitangentWS.w);
                inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz));
            
                inputData.normalWS = NormalizeNormalPerPixel(inputData.normalWS);
                viewDirWS = SafeNormalize(viewDirWS);
                inputData.viewDirectionWS = viewDirWS;
            
                inputData.shadowCoord = float4(0, 0, 0, 0);
            

                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
            }

            half3 DirectBDRFCustom(BRDFData brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS, half metallic)
            {

                float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));

                float NoH = saturate(dot(normalWS, halfDir));
                half LoH = saturate(dot(lightDirectionWS, halfDir));

                float d = NoH * NoH * brdfData.roughness2MinusOne + 1.00001f;

                half LoH2 = LoH * LoH;
                half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);

                
            #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
                specularTerm *= lerp(1,3.5,metallic);
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
                
            #endif
                half3 color = specularTerm * _SpecularIntensity * brdfData.specular + brdfData.diffuse;
                return color;

                //return brdfData.diffuse;

            }



            half3 LightingPhysicallyBasedCustom(BRDFData brdfData, half3 lightColor, half3 lightDirectionWS, half lightAttenuation, half3 normalWS, half3 viewDirectionWS, half metallic)
            {
                half NdotL = saturate(dot(normalWS, lightDirectionWS));
                half3 radiance;
                half3 shadowColor = lerp( _ShadowColor.rgb, 1 , smoothstep( _ShadowThreshold - _ShadowSmooth, _ShadowThreshold + _ShadowSmooth + 0.001,  lightAttenuation * NdotL ) );

                radiance = lightColor * shadowColor;
                    
                return DirectBDRFCustom(brdfData, normalWS, lightDirectionWS, viewDirectionWS, metallic) * radiance;
            }

            half3 LightingPhysicallyBasedCustom(BRDFData brdfData, Light light, half3 normalWS, half3 viewDirectionWS, half metallic)
            {
                return LightingPhysicallyBasedCustom(brdfData, light.color, light.direction, lerp(1,light.distanceAttenuation * light.shadowAttenuation, _ReceiveShadowAmount), normalWS, viewDirectionWS, metallic);
            }

            half3 GlossyEnvironmentReflectionCustom(half3 reflectVector, half perceptualRoughness, half occlusion)
            {
            
                half mip = PerceptualRoughnessToMipmapLevel(perceptualRoughness);
                half4 encodedIrradiance = SAMPLE_TEXTURECUBE_LOD(unity_SpecCube0, samplerunity_SpecCube0, reflectVector, mip * _CubemapBlurAmount);
            
                half3 irradiance = DecodeHDREnvironment(encodedIrradiance, unity_SpecCube0_HDR);
            

                return irradiance * occlusion;
            

                //return _GlossyEnvironmentColor.rgb * occlusion;
            }



            half3 GlobalIlluminationCustom(BRDFData brdfData, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS)
            {
                half3 reflectVector = reflect(-viewDirectionWS, normalWS);
                half fresnelTerm = smoothstep(_FresnelThreshold - _FresnelSmooth, _FresnelThreshold + _FresnelSmooth,  1.0 - saturate(dot(normalWS, viewDirectionWS))) * _FresnelIntensity;

                half3 indirectDiffuse = bakedGI * occlusion * _GIIntensity;
                half3 indirectSpecular = GlossyEnvironmentReflectionCustom(reflectVector, brdfData.perceptualRoughness, occlusion) * _CubemapIntensity;

                return EnvironmentBRDF(brdfData, indirectDiffuse, indirectSpecular, fresnelTerm);
            }



            half4 UniversalFragmentPBRCustom(InputData inputData, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half3 emission, half alpha)
            {
                BRDFData brdfData;
                InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);
                
                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                Light mainLight = GetMainLight(inputData.shadowCoord);
                MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

                half3 color = GlobalIlluminationCustom(brdfData, inputData.bakedGI, occlusion, inputData.normalWS, inputData.viewDirectionWS) ;
                color += LightingPhysicallyBasedCustom(brdfData, mainLight, inputData.normalWS, inputData.viewDirectionWS, metallic);

            #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
                    color += LightingPhysicallyBased(brdfData, light, inputData.normalWS, inputData.viewDirectionWS) ;
                }
            #endif


            #ifdef _ADDITIONAL_LIGHTS_VERTEX
                color += inputData.vertexLighting * brdfData.diffuse;
            #endif

                color += emission;
                return half4(color, alpha);
            }

            half3 MixFogCustom(half3 fragColor, half fogFactor, half3 worldPos)
            {
                
                #if _FOG_HEIGHT
                    float heightFogFactor;
                    float heightFog;
                    float3 GradientFog;
                    #if defined(FOG_LINEAR)
                        heightFogFactor = smoothstep( _FogThreshold - _FogSmooth, _FogThreshold + _FogSmooth, saturate( (worldPos.y - _FogOffset) / max(0,_FogHeight) ) );
                        GradientFog = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, heightFogFactor).xyz;
                        heightFog = 1-saturate( (1-fogFactor) * (1-heightFogFactor) );
                    #elif defined(FOG_EXP)
                        GradientFog = SAMPLE_TEXTURE2D(_GradientTex, sampler_GradientTex, 1-fogFactor).xyz;
                        heightFog = 1-saturate( (1-fogFactor) * (heightFogFactor) );
                    #endif
                    return MixFogColor(fragColor, GradientFog, heightFog);

                #else
                    return MixFogColor(fragColor, unity_FogColor.rgb, fogFactor);
                #endif

            }


            v2f vert (appdata v)
            {
                v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_TRANSFER_INSTANCE_ID(v, o);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                    VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
                    VertexNormalInputs normalInput = GetVertexNormalInputs(v.normal, v.tangent);
                    half3 viewDirWS = GetCameraPositionWS() - vertexInput.positionWS;
                    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                    half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                    o.uv = TRANSFORM_TEX(v.texcoord, _BaseMap);
                    o.color = v.color;
                
                    o.normalWS = half4(normalInput.normalWS, viewDirWS.x);
                    o.tangentWS = half4(normalInput.tangentWS, viewDirWS.y);
                    o.bitangentWS = half4(normalInput.bitangentWS, viewDirWS.z);
                

                    OUTPUT_LIGHTMAP_UV(v.lightmapUV, unity_LightmapST, o.lightmapUV);
                    OUTPUT_SH(o.normalWS.xyz, o.vertexSH);

                    //o.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                    o.positionWS = vertexInput.positionWS;
                    o.shadowCoord = GetShadowCoord(vertexInput);

                    float height = SAMPLE_TEXTURE2D_LOD(_HeightTex, sampler_HeightTex, o.uv, 0);
                    o.positionCS = TransformObjectToHClip(v.vertex.xyz + float3(0,0,(height-0.5) * _VertexOffset) );

                    float3x3 tangentToWorld = float3x3(o.tangentWS.xyz, o.bitangentWS.xyz, o.normalWS.xyz);
                    o.viewDirTS = TransformWorldToTangent(viewDirWS, tangentToWorld);
                    
                
                    o.fogFactorAndVertexLight = half4(ComputeFogFactor(vertexInput.positionCS.z), vertexLight);
                
                


                return o;
            }

            half4 frag (v2f i) : SV_Target
            {

                UNITY_SETUP_INSTANCE_ID(i);

                i.uv += ParallaxMapping(_HeightTex, sampler_HeightTex, i.viewDirTS, _ParallaxAmplitude * 0.01, i.uv );
                 
                
                #if _USENORMALTEX_ON
                    float3 NormalTS = UnpackNormal( SAMPLE_TEXTURE2D(_BumpMap, sampler_BumpMap, i.uv) ) * float3(_BumpScale, _BumpScale, 1);
                #else
                    float3 NormalTS = float3(0,0,1);
                #endif
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _Color;

                float4 col;

                InputData inputData;
                InitializeInputData(i, NormalTS, inputData);
                i.shadowCoord = TransformWorldToShadowCoord(i.positionWS);

                float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);
                
                float metallic = mask.r * _MetallicValue;
                float roughness = mask.g * _RoughnessValue;
                float occlusion =  lerp( 1 , mask.b , _AoStrength);
                


                col = UniversalFragmentPBRCustom(inputData, albedo.rgb, metallic, 0.5 , 1 - roughness , occlusion, 0, albedo.a);
                
                
                //emissive
                #if _USEEMISSIVE_ON
                    float4 emissiveTex = SAMPLE_TEXTURE2D(_EmissiveMap, sampler_EmissiveMap, i.uv);
                    float3 emissive = pow( abs(emissiveTex.a), _EmissiveEdgeSmooth) * emissiveTex.rgb * max(0,_EmissiveIntensity) * _EmissionColor.rgb;
                    col.rgb += emissive;
                #endif
                

                //col.rgb *= lerp( 1, i.color.rgb, i.color.a * _VertexColorAO  );
                    
                col.rgb = MixFogCustom(col.rgb, inputData.fogCoord, i.positionWS);


                #if _ALPHATEST_ON
                    clip(col.a - _AlphaCutout);
                #endif

                

                return col;    
            }

            ENDHLSL
        }

                

        Pass
    {
        Name "ShadowCaster"

        Tags{"LightMode" = "ShadowCaster"}

            Cull Back
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0

            #pragma vertex ShadowPassVertex
            #pragma fragment ShadowPassFragment

           // GPU Instancing
            #pragma multi_compile_instancing

            #pragma multi_compile_local _ _ALPHATEST_ON
          
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   
            TEXTURE2D(_BumpMap);   SAMPLER(sampler_BumpMap);  
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex); 
            //TEXTURE2D(_EmissiveMap);   SAMPLER(sampler_EmissiveMap); 
            CBUFFER_START(UnityPerMaterial)    

            
            half4 _MaskTex_ST;
            half4 _BaseMap_ST;
            half4 _EmissiveMap_ST;
            
            float4 _Color, _EmissiveColor;
            float4 _ShadowColor;
            
            
            
            float _ReceiveShadowAmount;
            float _ShadowThreshold, _ShadowSmooth;
            float _BumpScale;
            float _EmissiveEdgeSmooth, _EmissionColor;
            float _MetallicValue;
            float _RoughnessValue;
            float _AoStrength;
            float _GIIntensity;
            float _CubemapIntensity;
            float _FresnelThreshold, _FresnelSmooth, _FresnelIntensity;
            float _SpecularIntensity;
            
            float _AlphaCutout;
            //float _VertexColorAO;
            
            //float _ReceiveShadowDarkness;

            CBUFFER_END


            struct VertexInput
            {          
                float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
                float4 normal : NORMAL;

                UNITY_VERTEX_INPUT_INSTANCE_ID  
            };
          
            struct VertexOutput
            {          
                float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID          
                UNITY_VERTEX_OUTPUT_STEREO
  
            };


            VertexOutput ShadowPassVertex(VertexInput v)
            {
               VertexOutput o;
               UNITY_SETUP_INSTANCE_ID(v);
               UNITY_TRANSFER_INSTANCE_ID(v, o);                        
           
              float3 positionWS = TransformObjectToWorld(v.vertex.xyz);
              float3 normalWS   = TransformObjectToWorldNormal(v.normal.xyz);
              float4 positionCS = TransformWorldToHClip(  ApplyShadowBias( positionWS , normalWS, _MainLightPosition.xyz)  );
              
              o.vertex = positionCS;
			  o.texcoord = v.texcoord;
             
              return o;
            }

            half4 ShadowPassFragment(VertexOutput i) : SV_TARGET
            {  
                UNITY_SETUP_INSTANCE_ID(i);
                float4 diffuse = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.texcoord) ;
            #if _ALPHATEST_ON
                clip(diffuse.a - _AlphaCutout);
            #endif
                return 0;
            }

            ENDHLSL
        }

        Pass
        {
            Name "DepthOnly"
            Tags{"LightMode" = "DepthOnly"}

            ZWrite On
            ColorMask 0

            Cull Back

            HLSLPROGRAM
          
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
  
            // GPU Instancing
            #pragma multi_compile_instancing

            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_local _ _ALPHATEST_ON
              
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
              
            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   
            TEXTURE2D(_BumpMap);   SAMPLER(sampler_BumpMap);  
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex); 
            //TEXTURE2D(_EmissiveMap);   SAMPLER(sampler_EmissiveMap); 
            CBUFFER_START(UnityPerMaterial)    

            
            half4 _MaskTex_ST;
            half4 _BaseMap_ST;
            half4 _EmissiveMap_ST;
            
            float4 _Color, _EmissionColor;
            float4 _ShadowColor;
            
            
            
            float _ReceiveShadowAmount;
            float _ShadowThreshold, _ShadowSmooth;
            float _BumpScale;
            float _EmissiveEdgeSmooth, _EmissiveIntensity;
            float _MetallicValue;
            float _RoughnessValue;
            float _AoStrength;
            float _GIIntensity;
            float _CubemapIntensity, _CubemapBlurAmount;
            float _FresnelThreshold, _FresnelSmooth, _FresnelIntensity;
            float _SpecularIntensity;
            
            float _AlphaCutout;
            //float _VertexColorAO;

            //float _ReceiveShadowDarkness;


            CBUFFER_END


              
            struct VertexInput
            {
                float4 vertex : POSITION;        
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput
            {          
                float4 vertex : SV_POSITION;
				float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID          
            };

            


            

            VertexOutput vert(VertexInput v)
            {
                VertexOutput o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                o.vertex = TransformObjectToHClip(v.vertex.xyz);
				o.texcoord = v.texcoord;
                return o;
            }

            half4 frag(VertexOutput IN) : SV_TARGET
            {     
                float4 diffuse = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.texcoord);
            #if _ALPHATEST_ON
                clip(diffuse.a - _AlphaCutout);
            #endif             
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "DepthNormals"
            Tags{"LightMode" = "DepthNormals"}

            ZWrite On
            Cull[_Cull]

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex DepthNormalsVertex
            #pragma fragment DepthNormalsFragment

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma shader_feature_local_fragment _RoughnessValue_TEXTURE_ALBEDO_CHANNEL_A

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/CommonMaterial.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"
            //#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/ParallaxMapping.hlsl"

            //#include "Packages/com.unity.render-pipelines.universal/Shaders/LitInput.hlsl"
            //TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap); 
            CBUFFER_START(UnityPerMaterial)    
            
            half4 _MaskTex_ST;
            half4 _BaseMap_ST;
            half4 _EmissiveMap_ST;
            
            float4 _Color, _EmissionColor;
            float4 _ShadowColor;
            
            
            
            float _ReceiveShadowAmount;
            float _ShadowThreshold, _ShadowSmooth;
            float _BumpScale;
            float _EmissiveEdgeSmooth, _EmissiveIntensity;
            float _MetallicValue;
            float _RoughnessValue;
            float _AoStrength;
            float _GIIntensity;
            float _CubemapIntensity, _CubemapBlurAmount;
            float _FresnelThreshold, _FresnelSmooth, _FresnelIntensity;
            float _SpecularIntensity;
            
            float _AlphaCutout;
            //float _VertexColorAO;

            //float _ReceiveShadowDarkness;
            
            CBUFFER_END

            #ifndef UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED
            #define UNIVERSAL_DEPTH_ONLY_PASS_INCLUDED

            

            struct Attributes
            {
                float4 positionOS     : POSITION;
                float4 tangentOS      : TANGENT;
                float2 texcoord     : TEXCOORD0;
                float3 normal       : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD1;
                float3 normalWS                 : TEXCOORD2;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings DepthNormalsVertex(Attributes input)
            {
                Varyings output = (Varyings)0;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv         = TRANSFORM_TEX(input.texcoord, _BaseMap);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);

                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normal, input.tangentOS);
                output.normalWS = NormalizeNormalPerVertex(normalInput.normalWS);

                return output;
            }

            float4 DepthNormalsFragment(Varyings input) : SV_TARGET
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                Alpha(SampleAlbedoAlpha(input.uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap)).a, _Color, _AlphaCutout);
                return float4(PackNormalOctRectEncode(TransformWorldToViewDir(input.normalWS, true)), 0.0, 0.0);
            }
            #endif

            ENDHLSL
        }


        Pass
        {
            Name "Meta"
            Tags{"LightMode" = "Meta"}

            Cull Off

            HLSLPROGRAM
            // Required to compile gles 2.0 with standard srp library
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex UniversalVertexMeta
            #pragma fragment UniversalFragmentMeta

            #pragma shader_feature_local_fragment _SPECULAR_SETUP
            //#pragma shader_feature_local_fragment _EMISSION
            #pragma shader_feature_local_fragment _MetallicValueSPECGLOSSMAP
            #pragma multi_compile_local _ _ALPHATEST_ON
            #pragma shader_feature_local_fragment _ _RoughnessValue_TEXTURE_ALBEDO_CHANNEL_A
            //#pragma shader_feature_local _ _DETAIL_MULX2 _DETAIL_SCALED

            #pragma shader_feature_local_fragment _SPECGLOSSMAP


            #ifndef UNIVERSAL_LIT_META_PASS_INCLUDED
            #define UNIVERSAL_LIT_META_PASS_INCLUDED

            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/MetaInput.hlsl"

            #ifndef UNIVERSAL_META_PASS_INCLUDED
            #define UNIVERSAL_META_PASS_INCLUDED

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/SurfaceInput.hlsl"


            //TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   
            //TEXTURE2D(_NormalTex);   SAMPLER(sampler_NormalTex);  
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex); 
            //TEXTURE2D(_EmissiveMap);   SAMPLER(sampler_EmissiveMap); 
            CBUFFER_START(UnityPerMaterial)    

            
            half4 _MaskTex_ST;
            half4 _BaseMap_ST;
            half4 _EmissiveMap_ST;
            
            float4 _Color, _EmissionColor;
            float4 _ShadowColor;
            
            
            
            float _ReceiveShadowAmount;
            float _ShadowThreshold, _ShadowSmooth;
            float _BumpScale;
            float _EmissiveEdgeSmooth, _EmissiveIntensity;
            float _MetallicValue;
            float _RoughnessValue;
            float _AoStrength;
            float _GIIntensity;
            float _CubemapIntensity, _CubemapBlurAmount;
            float _FresnelThreshold, _FresnelSmooth, _FresnelIntensity;
            float _SpecularIntensity;
            
            float _AlphaCutout;
            //float _VertexColorAO;

            //float _ReceiveShadowDarkness;


            CBUFFER_END

            
            //half4 _BaseColor;
            
            half _Cutoff;
            //half _BumpScale;

            // x = use uv1 as raster position
            // y = use uv2 as raster position
            bool4 unity_MetaVertexControl;

            // x = return albedo
            // y = return normal
            bool4 unity_MetaFragmentControl;

            float unity_OneOverOutputBoost;
            float unity_MaxOutputValue;
            float unity_UseLinearSpace;

            struct MetaInput
            {
                half3 Albedo;
                half3 Emission;
                half3 SpecularColor;
            };

            float4 MetaVertexPosition(float4 positionOS, float2 uv1, float2 uv2, float4 uv1ST, float4 uv2ST)
            {
                if (unity_MetaVertexControl.x)
                {
                    positionOS.xy = uv1 * uv1ST.xy + uv1ST.zw;
                    // OpenGL right now needs to actually use incoming vertex position,
                    // so use it in a very dummy way
                    positionOS.z = positionOS.z > 0 ? REAL_MIN : 0.0f;
                }
                if (unity_MetaVertexControl.y)
                {
                    positionOS.xy = uv2 * uv2ST.xy + uv2ST.zw;
                    // OpenGL right now needs to actually use incoming vertex position,
                    // so use it in a very dummy way
                    positionOS.z = positionOS.z > 0 ? REAL_MIN : 0.0f;
                }
                return TransformWorldToHClip(positionOS.xyz);
            }

            half4 MetaFragment(MetaInput input)
            {
                half4 res = 0;
                if (unity_MetaFragmentControl.x)
                {
                    res = half4(input.Albedo, 1.0);

                    // Apply Albedo Boost from LightmapSettings.
                    res.rgb = clamp(PositivePow(res.rgb, saturate(unity_OneOverOutputBoost)), 0, unity_MaxOutputValue);
                }
                if (unity_MetaFragmentControl.y)
                {
                    half3 emission;
                    if (unity_UseLinearSpace)
                        emission = input.Emission;
                    else
                        emission = LinearToSRGB(input.Emission);

                    res = half4(emission, 1.0);
                }
                return res;
            }

            #endif

            half4 SampleMetallicSpecGloss(float2 uv, half albedoAlpha)
            {
                half4 specGloss;

            #ifdef _MetallicValueSPECGLOSSMAP
                specGloss = SAMPLE_MetallicValueSPECULAR(uv);
                #ifdef _SmoothnessValue_TEXTURE_ALBEDO_CHANNEL_A
                    specGloss.a = albedoAlpha * (1 - _RoughnessValue);
                #else
                    specGloss.a *= (1 - _RoughnessValue);
                #endif
            #else // _MetallicValueSPECGLOSSMAP
                #if _SPECULAR_SETUP
                    specGloss.rgb = _SpecColor.rgb;
                #else
                    specGloss.rgb = _MetallicValue.rrr;
                #endif

                #ifdef _SmoothnessValue_TEXTURE_ALBEDO_CHANNEL_A
                    specGloss.a = albedoAlpha * (1 - _RoughnessValue);
                #else
                    specGloss.a = (1 - _RoughnessValue);
                #endif
            #endif

                return specGloss;
            }

            half SampleOcclusion(float2 uv)
            {
            #ifdef _OCCLUSIONMAP
            // TODO: Controls things like these by exposing SHADER_QUALITY levels (low, medium, high)
            #if defined(SHADER_API_GLES)
                return SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
            #else
                half occ = SAMPLE_TEXTURE2D(_OcclusionMap, sampler_OcclusionMap, uv).g;
                return LerpWhiteTo(occ, _OcclusionStrength);
            #endif
            #else
                return 1.0;
            #endif
            }



            inline void InitializeStandardLitSurfaceData(float2 uv, out SurfaceData outSurfaceData)
            {
                half4 albedoAlpha = SampleAlbedoAlpha(uv, TEXTURE2D_ARGS(_BaseMap, sampler_BaseMap));
                outSurfaceData.alpha = Alpha(albedoAlpha.a, _Color, _Cutoff);

                half4 specGloss = SampleMetallicSpecGloss(uv, albedoAlpha.a);
                outSurfaceData.albedo = albedoAlpha.rgb * _Color.rgb;

                float4 MaskTex = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv);

            #if _SPECULAR_SETUP
                outSurfaceData.metallic = 1.0h;
                outSurfaceData.specular = specGloss.rgb;
            #else
                outSurfaceData.metallic = MaskTex.r * _MetallicValue;//specGloss.r;
                outSurfaceData.specular = half3(0.0h, 0.0h, 0.0h);
            #endif

                

                outSurfaceData.smoothness = MaskTex.g * _RoughnessValue;//specGloss.a;
                outSurfaceData.normalTS = SampleNormal(uv, TEXTURE2D_ARGS(_BumpMap, sampler_BumpMap), _BumpScale);
                outSurfaceData.occlusion = MaskTex.b;//SampleOcclusion(uv);
                outSurfaceData.emission = SampleEmission(uv, _EmissionColor.rgb, TEXTURE2D_ARGS(_EmissionMap, sampler_EmissionMap));
                
            #if defined(_CLEARCOAT) || defined(_CLEARCOATMAP)
                half2 clearCoat = SampleClearCoat(uv);
                outSurfaceData.clearCoatMask       = clearCoat.r;
                outSurfaceData.clearCoatSmoothness = clearCoat.g;
            #else
                outSurfaceData.clearCoatMask       = 0.0h;
                outSurfaceData.clearCoatSmoothness = 0.0h;
            #endif

            #if defined(_DETAIL)
                half detailMask = SAMPLE_TEXTURE2D(_DetailMask, sampler_DetailMask, uv).a;
                float2 detailUv = uv * _DetailAlbedoMap_ST.xy + _DetailAlbedoMap_ST.zw;
                outSurfaceData.albedo = ApplyDetailAlbedo(detailUv, outSurfaceData.albedo, detailMask);
                outSurfaceData.normalTS = ApplyDetailNormal(detailUv, outSurfaceData.normalTS, detailMask);

            #endif

            }

            struct Attributes
            {
                float4 positionOS   : POSITION;
                float3 normalOS     : NORMAL;
                float2 uv0          : TEXCOORD0;
                float2 uv1          : TEXCOORD1;
                float2 uv2          : TEXCOORD2;
            #ifdef _TANGENT_TO_WORLD
                float4 tangentOS     : TANGENT;
            #endif
            };

            struct Varyings
            {
                float4 positionCS   : SV_POSITION;
                float2 uv           : TEXCOORD0;
            };

            Varyings UniversalVertexMeta(Attributes input)
            {
                Varyings output;
                output.positionCS = MetaVertexPosition(input.positionOS, input.uv1, input.uv2, unity_LightmapST, unity_DynamicLightmapST);
                output.uv = TRANSFORM_TEX(input.uv0, _BaseMap);
                return output;
            }

            half4 UniversalFragmentMeta(Varyings input) : SV_Target
            {
                SurfaceData surfaceData;
                InitializeStandardLitSurfaceData(input.uv, surfaceData);

                BRDFData brdfData;
                InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

                MetaInput metaInput;
                metaInput.Albedo = brdfData.diffuse + brdfData.specular * brdfData.roughness * 0.5;
                metaInput.SpecularColor = surfaceData.specular;
                metaInput.Emission = surfaceData.emission;

                return MetaFragment(metaInput);
            }


            //LWRP -> Universal Backwards Compatibility
            Varyings LightweightVertexMeta(Attributes input)
            {
                return UniversalVertexMeta(input);
            }

            half4 LightweightFragmentMeta(Varyings input) : SV_Target
            {
                return UniversalFragmentMeta(input);
            }

            #endif




            ENDHLSL
        }



            
    }
}







Shader "Rogue/Character/StylizedCharacterShader"
{
    Properties
    {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _BaseMap ("Main Texture (RGB : Albedo Texture, A : Alpha) ", 2D) = "white" {}
        
        _ReceiveShadowAmount ("Receive Shadow Amount", Range(0,1)) = 1

        [Header(Normal)]
        [Space(10)]
        [NoScaleOffset]_NormalTex ("Normal Texture", 2D) = "bump" {}
        
        _NormalStrength ("Normal Strength", float) = 1

        [Space(20)]
        [Header(Stylized Diffuse)]
        [Space(10)]
        _MidColor("Mid Tone Color", Color) = (1,1,1,1)
        _MidThreshold ("Mid Tone Threshold", Range(0,1)) = 1
        _MidSmooth ("Mid Tone Smooth", Range(0,0.5)) = 0.25
        
        [Space(10)]
        _ShadowColor ("Shadow Color", Color) = (0,0,0,1)
        _ShadowThreshold ("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmooth ("Shadow Smooth", Range(0,0.5)) = 0.25
        
        [Space(10)]
        _ReflectColor ("Reflect Color", Color) = (0,0,0,1)
        _ReflectThreshold ("Reflect Threshold", Range(0,1)) = 0
        _ReflectSmooth ("Reflect Smooth", Range(0,0.5)) = 0.25
        [Space(10)]
        
        _GIIntensity("GI Intensity", Range(0,2)) = 1
    
        [Space(20)]
        [Header(Stylized Reflection)]
        [Space(10)]
        [NoScaleOffset] _MaskTex ("Mask Texture ( R:Metallic, G:Roughness, B:AO, A:Emission", 2D) = "white" {}
        _Metallic ("Metallic", Range(0,1)) = 1
        _Roughness ("Roughness", Range(0,1)) = 1
        _AoStrength("AO Strength", Range(0, 1)) = 1.0

        [Space(10)]
        _EmissionColor ("Emission Color", Color) = (1,1,1,1)
        _EmissionEdgeSmooth ("Emission Edge Smooth ", float) = 1
        _EmissionIntensity ("Emission Intensity", float) = 1

        
        [Space(10)]
        [Toggle] _UseGGX ("Use GGX", float) = 0
        _SpecularNumStep("Specular Num Step", Range(0,10)) = 1
        _SpecularColor("Specular Color", Color) = (0.5, 0.5, 0.5)
        _SpecularIntensity("Specular Intensity", Range(0,30)) = 1
        [Space(10)]
        [Toggle] _DirectionalFresnel ("Directional Fresnel", float) = 0
        
        [Toggle] _SingleColorFresnel ("Single Color Fresnel", float) = 0
        _FresnelColor("Fresnel Color", Color) = (1,1,1,1)
        _FresnelThreshold("Fresnel Threshold", Range(0,1)) = 0.8
        _FresnelSmooth("Fresnel Smooth", Range(0,0.5)) = 0.25
        _FresnelIntensity ("Non Metal Fresnel Intensity", float) = 1
        _MetalFresnelIntensity("Metal Fresnel Intensity", float) = 1

        [Space(10)]
        _ReflProbeColor ("Reflection Probe Color", Color) = (1,1,1,1)
        _ReflProbeIntensity("Non Metal Reflection Probe Intensity", float) = 1
        _MetalReflProbeIntensity ("Metal Reflection Probe Intensity", float) = 1
        _ReflProbeBlurAmount("Reflection Probe Blur Amount", Range(0,2)) = 1

   
        [Space(20)]
        [Header(Alpha Culling)]
        [Space(10)]
        [Toggle] _AlphaTest ("AlphaTest", float) = 0
        _AlphaCutout ("Alpha Cutout", Range(0,1)) = 0
        [Enum(UnityEngine.Rendering.CullMode)]_Cull ("Culling", float) = 2

        [Space(20)]
        [Header(Outline)]
        [Space(10)]
        [Toggle]_UseTangentOutline("Use Tangent Outline", float) = 0
        _OutlineWidth("Outline Width", Float) = 1
        _OutlineColor("Outline Color", Color) = (0, 0, 0, 0)
        _ZOffset("Outline Z Offset", Range(-3,3)) = 0
        _OutlineMainTexAmount("Outline Main Tex Color Amount", Range(0,1)) = 0
        _PerspectiveCorrect("Perspective Correct", Range(0,1)) = 0.5
        

        
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}

        //------------------------------------------------------
        
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
            
             // GPU Instancing
            #pragma multi_compile_instancing
            #pragma multi_compile_fog


            // Recieve Shadow
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_VERTEX _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHT_SHADOWS
            #pragma multi_compile _ _SHADOWS_SOFT

            #pragma multi_compile _ _ALPHATEST_ON
                        
            
            

            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   
            TEXTURE2D(_NormalTex);   SAMPLER(sampler_NormalTex);  
            TEXTURE2D(_MaskTex);   SAMPLER(sampler_MaskTex); 

            CBUFFER_START(UnityPerMaterial)    

            float4 _BaseMap_ST;
            half4 _MaskTex_ST;
            
            float4 _Color,_FresnelColor,_SpecularColor;
            
            float _ReceiveShadowAmount;
            float _NormalStrength;
            
            float _Metallic;
            float _Roughness;
            float _AoStrength;


            float _AlphaCutout;

            float4 _EmissionColor;
            float _EmissionEdgeSmooth;
            float _EmissionIntensity;
            float _EmissionMode;


            float4 _MidColor, _ShadowColor, _ReflectColor, _ReflProbeColor;
            float _MidThreshold, _MidSmooth, _ShadowThreshold, _ShadowSmooth, _ReflectThreshold, _ReflectSmooth;
            float _SpecularNumStep, _SpecularIntensity ,_FresnelIntensity, _FresnelThreshold, _FresnelSmooth, _DirectionalFresnel, _MetalFresnelIntensity, _SingleColorFresnel;
            float _ReflProbeIntensity, _MetalReflProbeIntensity, _ReflProbeBlurAmount;
            float _GIIntensity;

            float _UseGGX;

            float4 _OutlineColor;
            float _OutlineWidth, _ZOffset, _OutlineMainTexAmount,_PerspectiveCorrect, _UseTangentOutline;


            CBUFFER_END

            

            struct appdata
            {
                float4 color : COLOR0;
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 lightmapUV : TEXCOORD1;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID                                
            };


            struct v2f
            {
                float4 color : COLOR0;
                float4 positionCS : SV_POSITION;
                
                float2 uv                       : TEXCOORD0;
                DECLARE_LIGHTMAP_OR_SH(lightmapUV, vertexSH, 1);
                float3 positionWS               : TEXCOORD2;
                float3 normalWS                 : TEXCOORD3;
                float4 tangentWS                : TEXCOORD4;    // xyz: tangent, w: sign
                float3 viewDirWS                : TEXCOORD5;
                half4 fogFactorAndVertexLight   : TEXCOORD6; // x: fogFactor, yzw: vertex light
                float4 shadowCoord              : TEXCOORD7;
                //float3 viewDirTS                : TEXCOORD8;

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
  
            };

            float4 RotateX(float4 localRotation, float angle)
            {
                float angleX = radians(angle);
                float c = cos(angleX);
                float s = sin(angleX);
                float4x4 rotateXMatrix  = float4x4( 1,  0,  0,  0,
                                                    0,  c,  -s, 0,
                                                    0,  s,  c,  0,
                                                    0,  0,  0,  1);
                return mul(localRotation, rotateXMatrix);
            }

            float4 RotateY(float4 localRotation, float angle)
            {
                float angleY = radians(angle);
                float c = cos(angleY);
                float s = sin(angleY);
                float4x4 rotateYMatrix  = float4x4( c,  0,  s,  0,
                                                    0,  1,  0,  0,
                                                    -s, 0,  c,  0,
                                                    0,  0,  0,  1);
                return mul(localRotation, rotateYMatrix);
            }

            float4 RotateZ(float4 localRotation, float angle)
            {
                float angleZ = radians(angle);
                float c = cos(angleZ);
                float s = sin(angleZ);
                float4x4 rotateZMatrix  = float4x4( c,  -s, 0,  0,
                                                    s,  c,  0,  0,
                                                    0,  0,  1,  0,
                                                    0,  0,  0,  1);
                return mul(localRotation, rotateZMatrix);
            }



            half LinearStep(half minValue, half maxValue, half In)
            {
                return saturate((In-minValue) / (maxValue - minValue));
            }

            

            
            void InitializeInputData(v2f input, half3 normalTS, out InputData inputData)
            {
                inputData = (InputData)0;

                inputData.positionWS = input.positionWS;
        
                half3 viewDirWS = SafeNormalize(input.viewDirWS);

                float sgn = input.tangentWS.w;      // should be either +1 or -1
                float3 bitangent = sgn * cross(input.normalWS.xyz, input.tangentWS.xyz);
                inputData.normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS.xyz, bitangent.xyz, input.normalWS.xyz));
            
                inputData.normalWS = normalize(inputData.normalWS);
                inputData.viewDirectionWS = viewDirWS;

            #if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
                inputData.shadowCoord = input.shadowCoord;
            #elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
            #else
                inputData.shadowCoord = float4(0, 0, 0, 0);
            #endif

                inputData.fogCoord = input.fogFactorAndVertexLight.x;
                inputData.vertexLighting = input.fogFactorAndVertexLight.yzw;
                inputData.bakedGI = SAMPLE_GI(input.lightmapUV, input.vertexSH, inputData.normalWS);
                inputData.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(input.positionCS);
                inputData.shadowMask = SAMPLE_SHADOWMASK(input.lightmapUV);
            }

            half3 DirectBDRFCustom(BRDFData brdfData, half3 normalWS, half3 lightDirectionWS, half3 viewDirectionWS)
            {

                float3 halfDir = SafeNormalize(float3(lightDirectionWS) + float3(viewDirectionWS));

                float NoH = saturate(dot(normalWS, halfDir));
                half LoH = saturate(dot(lightDirectionWS, halfDir));

                float d = NoH * lerp(1,NoH, _UseGGX) * brdfData.roughness2MinusOne + 1.00001f;

                half LoH2 = LoH * LoH;
                half specularTerm = brdfData.roughness2 / ((d * d) * max(0.1h, LoH2) * brdfData.normalizationTerm);

                
            #if defined (SHADER_API_MOBILE) || defined (SHADER_API_SWITCH)
                specularTerm = specularTerm - HALF_MIN;
                specularTerm = clamp(specularTerm, 0.0, 100.0); // Prevent FP16 overflow on mobiles
            #endif
                half3 color =  lerp(  floor( saturate(NoH) * _SpecularNumStep) / (_SpecularNumStep+1)   , specularTerm , _UseGGX) * brdfData.specular * _SpecularColor.rgb * max(0,_SpecularIntensity) + brdfData.diffuse; // * brdfData.specular * max(0,_SpecularIntensity) + brdfData.diffuse;
                return color;

                

            }

            half3 CalculateRadiance(Light light, half3 normalWS, half3 brushStrengthRGB)
            {
                half NdotL = dot(normalWS, light.direction);

                half halfLambert = NdotL * 0.5 + 0.5;


                half smoothMidTone = LinearStep( _MidThreshold - _MidSmooth, _MidThreshold + _MidSmooth, halfLambert);
                half3 MidToneColor = lerp( _MidColor.rgb , 1 , smoothMidTone);
                
                half smoothShadow = LinearStep ( _ShadowThreshold - _ShadowSmooth, _ShadowThreshold + _ShadowSmooth, halfLambert * lerp(1,light.distanceAttenuation * light.shadowAttenuation,_ReceiveShadowAmount) ) ;
                half3 ShadowColor = lerp( _ShadowColor.rgb , MidToneColor, smoothShadow );   
                half smoothReflect = LinearStep( _ReflectThreshold - _ReflectSmooth, _ReflectThreshold + _ReflectSmooth, halfLambert);
                half3 ReflectColor = lerp(_ReflectColor.rgb , ShadowColor , smoothReflect);
                half3 radiance = light.color * ReflectColor;
                return radiance;
            }



            half3 LightingPhysicallyBasedCustom(BRDFData brdfData, half3 radiance, half3 lightDirectionWS, half3 normalWS, half3 viewDirectionWS, half3 positionWS)
            {             
            
                return DirectBDRFCustom(brdfData, normalWS, lightDirectionWS, viewDirectionWS) * radiance;
            }

            half3 LightingPhysicallyBasedCustom(BRDFData brdfData, half3 radiance, Light light, half3 normalWS, half3 viewDirectionWS, half3 positionWS)
            {
                return LightingPhysicallyBasedCustom(brdfData, radiance, light.direction, normalWS, viewDirectionWS, positionWS );
            }

            half3 EnvironmentBRDFCustom(BRDFData brdfData, half3 radiance, half3 indirectDiffuse, half3 indirectSpecular, half fresnelTerm)  
            {
                half3 c = indirectDiffuse * brdfData.diffuse * _GIIntensity;
                float surfaceReduction = 1.0 / (brdfData.roughness2 + 1.0);
                c += surfaceReduction * lerp( indirectSpecular * lerp(brdfData.specular * radiance, brdfData.grazingTerm * _FresnelColor.rgb, fresnelTerm), 0 , _SingleColorFresnel);   
                return c;
            }



            half3 GlobalIlluminationCustom(BRDFData brdfData, half3 radiance, half3 bakedGI, half occlusion, half3 normalWS, half3 viewDirectionWS, half metallic, half ndotl, half fresnelTerm)
            {
                half3 reflectVector = reflect(-viewDirectionWS, normalWS);
                

                half3 indirectDiffuse = bakedGI * occlusion;
                half3 indirectSpecular = GlossyEnvironmentReflection(reflectVector, brdfData.perceptualRoughness * _ReflProbeBlurAmount, occlusion) * lerp(max(0,_ReflProbeIntensity), max(0,_MetalReflProbeIntensity), step(0.5,metallic) ) * _ReflProbeColor.rgb;

                return EnvironmentBRDFCustom(brdfData, radiance, indirectDiffuse, indirectSpecular, fresnelTerm);
            }



            half4 UniversalFragmentPBRCustom(InputData inputData, half3 albedo, half metallic, half3 specular, half smoothness, half occlusion, half3 emission, half alpha)
            {
                BRDFData brdfData;
                InitializeBRDFData(albedo, metallic, specular, smoothness, alpha, brdfData);
                
                inputData.shadowCoord = TransformWorldToShadowCoord(inputData.positionWS);
                Light mainLight = GetMainLight(inputData.shadowCoord);

                float3 radiance = CalculateRadiance(mainLight, inputData.normalWS, float3(0, 0, 0));

                MixRealtimeAndBakedGI(mainLight, inputData.normalWS, inputData.bakedGI, half4(0, 0, 0, 0));

                float ndotl = lerp(1, saturate(dot(mainLight.direction, inputData.normalWS) ), _DirectionalFresnel);

                half fresnelTerm = smoothstep(_FresnelThreshold - _FresnelSmooth, _FresnelThreshold + _FresnelSmooth,  (1.0 - saturate(dot(inputData.normalWS, inputData.viewDirectionWS))) * ndotl ) * max(0,lerp(_FresnelIntensity, _MetalFresnelIntensity, step(0.5,metallic) ) ) ;
                half3 color = GlobalIlluminationCustom(brdfData, radiance, inputData.bakedGI, occlusion, inputData.normalWS, inputData.viewDirectionWS, metallic, ndotl, fresnelTerm);
                color += LightingPhysicallyBasedCustom(brdfData, radiance, mainLight, inputData.normalWS, inputData.viewDirectionWS, inputData.positionWS);

                

            #ifdef _ADDITIONAL_LIGHTS
                uint pixelLightCount = GetAdditionalLightsCount();
                for (uint lightIndex = 0u; lightIndex < pixelLightCount; ++lightIndex)
                {
                    Light light = GetAdditionalLight(lightIndex, inputData.positionWS);
                    color += LightingPhysicallyBased(brdfData, light, inputData.normalWS, inputData.viewDirectionWS);
                }
            #endif

            #ifdef _ADDITIONAL_LIGHTS_VERTEX
                color += inputData.vertexLighting * brdfData.diffuse;
            #endif

                color += emission;
                color += lerp(0, fresnelTerm * _FresnelColor.rgb, _SingleColorFresnel);
                return half4(color, alpha);
            }






            v2f vert (appdata input)
            {
                v2f output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);

                output.uv = TRANSFORM_TEX(input.texcoord, _BaseMap); 
                //float normalExpandMask = SAMPLE_TEXTURE2D_LOD(_NormalExpandMask, sampler_NormalExpandMask, output.uv, 0).x;
                

                VertexPositionInputs vertexInput = GetVertexPositionInputs(input.vertex.xyz );
                VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);
                half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);
                //half fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

                
                output.color = input.color;

                // already normalized from normal transform to WS.
                output.normalWS = normalInput.normalWS;
                output.viewDirWS = viewDirWS;
            
                real sign = input.tangentOS.w * GetOddNegativeScale();
                half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
           
            
                output.tangentWS = tangentWS;
            
                OUTPUT_LIGHTMAP_UV(input.lightmapUV, unity_LightmapST, output.lightmapUV);
                OUTPUT_SH(output.normalWS.xyz, output.vertexSH);

                //output.fogFactorAndVertexLight = half4(fogFactor, vertexLight);

                output.positionWS = vertexInput.positionWS;

                
                        
                output.shadowCoord = GetShadowCoord(vertexInput);
                
                output.positionCS = TransformWViewToHClip(vertexInput.positionVS );
                
                output.fogFactorAndVertexLight = half4(ComputeFogFactor(vertexInput.positionCS.z), vertexLight);
                
                


                return output;
            }

            half4 frag (v2f i) : SV_Target
            {

                UNITY_SETUP_INSTANCE_ID(i);
                
               
                float3 NormalTS = UnpackNormal( SAMPLE_TEXTURE2D(_NormalTex, sampler_NormalTex, i.uv) ) * float3(_NormalStrength, _NormalStrength , 1);
                
                float4 albedo = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.uv) * _Color;

                Light mainLight = GetMainLight();

                float4 col;

                InputData inputData;
                InitializeInputData(i, NormalTS, inputData);
                i.shadowCoord = TransformWorldToShadowCoord(i.positionWS);

                float4 mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv);

                float metallic = mask.r * _Metallic;
                float roughness = mask.g * _Roughness;
                float occlusion =  lerp( 1 , mask.b , _AoStrength);
                
                

                float3 emissive =  pow( abs(mask.a), _EmissionEdgeSmooth) * _EmissionIntensity * _EmissionColor.rgb;

                float smoothShadow;
                col = UniversalFragmentPBRCustom(inputData, albedo.rgb, metallic, 0.5 , 1-roughness , occlusion, emissive.rgb, albedo.a);
                col.rgb = MixFog(col.rgb, inputData.fogCoord);
                

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

            #pragma shader_feature _ALPHATEST_ON
          
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            
           TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap);   


            CBUFFER_START(UnityPerMaterial)    

            float4 _BaseMap_ST;
            half4 _MaskTex_ST;
            
            float4 _Color,_FresnelColor,_SpecularColor ;
            
            float _ReceiveShadowAmount;
            float _NormalStrength;
            
            float _Metallic;
            float _Roughness;
            float _AoStrength;


            float _AlphaCutout;

            float4 _EmissionColor;
            float _EmissionEdgeSmooth;
            float _EmissionIntensity;
            float _EmissionMode;


            float4 _MidColor, _ShadowColor, _ReflectColor, _ReflProbeColor;
            float _MidThreshold, _MidSmooth, _ShadowThreshold, _ShadowSmooth, _ReflectThreshold, _ReflectSmooth;
            float _SpecularNumStep, _SpecularIntensity ,_FresnelIntensity, _FresnelThreshold, _FresnelSmooth, _DirectionalFresnel, _MetalFresnelIntensity, _SingleColorFresnel;
            float _ReflProbeIntensity, _MetalReflProbeIntensity, _ReflProbeBlurAmount;
            float _GIIntensity;

            float _UseGGX;

            float4 _OutlineColor;
            float _OutlineWidth, _ZOffset, _OutlineMainTexAmount, _PerspectiveCorrect, _UseTangentOutline;


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

                //float normalExpandMask = SAMPLE_TEXTURE2D_LOD(_NormalExpandMask, sampler_NormalExpandMask, v.texcoord, 0).x;               
            
                float3 positionWS = TransformObjectToWorld(v.vertex.xyz );
                float3 normalWS   = TransformObjectToWorldNormal(v.normal.xyz);
                float3 positionVS = TransformWorldToView( ApplyShadowBias(positionWS, normalWS, _MainLightPosition.xyz));
                float4 positionCS = TransformWViewToHClip(positionVS);


                
                o.vertex = positionCS;
                
                o.texcoord = v.texcoord;
             
              return o;
            }

            half4 ShadowPassFragment(VertexOutput i) : SV_TARGET
            {  
                UNITY_SETUP_INSTANCE_ID(i);
                
                #if _ALPHATEST_ON
                    float4 diffuse = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.texcoord) ;
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

            #pragma shader_feature _ALPHATEST_ON
              
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
              
            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap); 

            CBUFFER_START(UnityPerMaterial)    

            float4 _BaseMap_ST;
            half4 _MaskTex_ST;
            
            float4 _Color, _FresnelColor,_SpecularColor;
            
            float _ReceiveShadowAmount;
            float _NormalStrength;
            
            float _Metallic;
            float _Roughness;
            float _AoStrength;


            float _AlphaCutout;

            float4 _EmissionColor;
            float _EmissionEdgeSmooth;
            float _EmissionIntensity;
            float _EmissionMode;


            float4 _MidColor, _ShadowColor, _ReflectColor, _ReflProbeColor;
            float _MidThreshold, _MidSmooth, _ShadowThreshold, _ShadowSmooth, _ReflectThreshold, _ReflectSmooth;
            float _SpecularNumStep, _SpecularIntensity ,_FresnelIntensity, _FresnelThreshold, _FresnelSmooth, _DirectionalFresnel, _MetalFresnelIntensity, _SingleColorFresnel;
            float _ReflProbeIntensity, _MetalReflProbeIntensity, _ReflProbeBlurAmount;
            float _GIIntensity;

            float _UseGGX;

            float4 _OutlineColor;
            float _OutlineWidth, _ZOffset, _OutlineMainTexAmount, _PerspectiveCorrect, _UseTangentOutline;

            CBUFFER_END


              
            struct VertexInput
            {
                float4 vertex : POSITION;        
				float2 texcoord : TEXCOORD0;
                float3 normalOS : NORMAL;
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
                //float normalExpandMask = SAMPLE_TEXTURE2D_LOD(_NormalExpandMask, sampler_NormalExpandMask, v.texcoord, 0).x;

                float3 positionWS = TransformObjectToWorld(v.vertex.xyz );
                float3 positionVS = TransformWorldToView(positionWS) ;
                o.vertex = TransformWViewToHClip(positionVS);
				o.texcoord = v.texcoord;
                return o;
            }

            half4 frag(VertexOutput IN) : SV_TARGET
            {     
                
                #if _ALPHATEST_ON
                    float4 diffuse = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, IN.texcoord);
                    clip(diffuse.a - _AlphaCutout);
                #endif      
      
                return 0;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Outline"
            Tags{"LightMode" = "Outline" "Queue" = "Geometry -1" }

           
            // Render State
            Blend One Zero, One Zero
            Cull Front
            //ZTest LEqual
            ZWrite On
            // ColorMask: <None>
            
        
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 2.0
            #pragma multi_compile_instancing
        
            
        
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            TEXTURE2D(_BaseMap);   SAMPLER(sampler_BaseMap); 

            CBUFFER_START(UnityPerMaterial)    

            float4 _BaseMap_ST;
            half4 _MaskTex_ST;
            
            float4 _Color, _FresnelColor,_SpecularColor;
            
            float _ReceiveShadowAmount;
            float _NormalStrength;
            
            float _Metallic;
            float _Roughness;
            float _AoStrength;


            float _AlphaCutout;

            float4 _EmissionColor;
            float _EmissionEdgeSmooth;
            float _EmissionIntensity;
            float _EmissionMode;


            float4 _MidColor, _ShadowColor, _ReflectColor, _ReflProbeColor;
            float _MidThreshold, _MidSmooth, _ShadowThreshold, _ShadowSmooth, _ReflectThreshold, _ReflectSmooth;
            float _SpecularNumStep, _SpecularIntensity ,_FresnelIntensity, _FresnelThreshold, _FresnelSmooth, _DirectionalFresnel, _MetalFresnelIntensity, _SingleColorFresnel;
            float _ReflProbeIntensity, _MetalReflProbeIntensity, _ReflProbeBlurAmount;
            float _GIIntensity;

            float _UseGGX;

            float4 _OutlineColor;
            float _OutlineWidth, _ZOffset, _OutlineMainTexAmount, _PerspectiveCorrect, _UseTangentOutline;

            CBUFFER_END

            struct appdata
            {
                float4 color : COLOR0;
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID                                
            };


            struct v2f
            {
                float4 color : COLOR0;
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                float fogCoord  : TEXCOORD1;
                
                float3 viewDir : TEXCOORD2;
                float3 normal : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            v2f vert (appdata v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_TRANSFER_INSTANCE_ID(v, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.color = v.color;
               
                float4 positionCS = TransformObjectToHClip(v.vertex.xyz);
                float3 worldPos = TransformObjectToWorld(v.vertex.xyz + ( lerp(v.normal, v.tangent.xyz, _UseTangentOutline) * 0.01 * _OutlineWidth * lerp(1,positionCS.a * 0.01,_PerspectiveCorrect ) ))  ;
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - worldPos );

                o.vertex = TransformWorldToHClip(worldPos.xyz - normalize(o.viewDir) * _ZOffset );

                o.texcoord = TRANSFORM_TEX(v.texcoord, _BaseMap);
                o.fogCoord = ComputeFogFactor(o.vertex.z);     
                
                o.normal = TransformObjectToWorldNormal(v.normal);

                return o;
            }

            half4 frag (v2f i) : SV_Target
            {
                float4 col;

                float4 MainTex = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, i.texcoord);

                col.rgb = lerp(_OutlineColor.rgb, MainTex.rgb, _OutlineMainTexAmount);

                col.rgb = MixFog(col.rgb, i.fogCoord);

                col.a = MainTex.a;

                #if _ALPHATEST_ON
                    clip(col.a - _AlphaCutout);
                #endif  

                return col;

            }


            ENDHLSL

        }

        

            
    }
}







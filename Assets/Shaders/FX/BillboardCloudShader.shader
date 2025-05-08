Shader "Rogue/FX/BillboardCloudShader"
{
    Properties
    {

        [Toggle]_UseBillboard("Use Billboard", float) = 0
        [Space(10)]
		[HDR]_Color ("Color", Color) = (0.5,0.5,0.5,1.0)
        _MainTex ("Main Texture ", 2D) = "white" {}
        _Rotate( "Rotate" , Vector) = (0,0,0,0)
        


        [Header(Distortion)]
        _DistortionTex ("Distortion Texture", 2D) = "gray"{}
        _DistortAmount ("Distortion Amount", Range(0,0.5)) = 0.1
        _DistortScrollSpeed ("Distortion Scroll Speed (XY)", Vector ) = (0,0,0,0)
        


        [Header(Depth Fade)]
        [Toggle]_UseDepthFade ("Use Depth Fade", float) = 1
        _DepthFadeFactor ("Depth Fade Factor", Range(0.01,5)) = 2

        [Space(20)]
        _MaskTex ("Mask Texture", 2D) = "white" {}
        _MaskPow ("Mask Pow", Range (0.01,1)) = 1
        _MaskDistortAmount ("Mask Distortion Amount", Range(0,0.1)) = 0.01



        [HideInInspector] _ConvertToBlack("Convert To Black", float ) = 0
        [HideInInspector] _TimeScale("Time Scale", float ) = 1

    }
    SubShader
    {
        Tags 
		{
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline" = "UniversalPipeline"
        }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            Name "ForwardLit"
			Tags { "LightMode" = "UniversalForward" }

			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite Off
			Cull Off

            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"


            #pragma multi_compile _ _USEDEPTHFADE_ON
            #pragma multi_compile _ _USEBILLBOARD_ON



            TEXTURE2D(_MainTex);    SAMPLER(sampler_MainTex);   
            TEXTURE2D(_MaskTex);    SAMPLER(sampler_MaskTex);   
            TEXTURE2D(_DistortionTex);    SAMPLER(sampler_DistortionTex);   
            #if _USEDEPTHFADE_ON
            TEXTURE2D(_CameraDepthTexture); SAMPLER(sampler_CameraDepthTexture);
            #endif

            CBUFFER_START(UnityPerMaterial)  
            half4 _Color;


            half4 _MainTex_ST;
            half4 _MaskTex_ST;
            half4 _DistortionTex_ST;
            half4 _DepthColor;

            half _DistortAmount;
            half4 _DistortScrollSpeed;
            half _DepthFadeFactor;
            float _ConvertToBlack;
            float _TimeScale;
            float _MaskDistortAmount;

            float4 _Rotate;
            float _MaskPow;


            CBUFFER_END

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float3 normal : NORMAL;
                float2 texcoord : TEXCOORD0;
                float4 worldPos : TEXCOORD1; 
                half3 viewDir : TEXCOORD2;
                float4 sPos : TEXCOORD3;
                
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


            v2f vert (appdata v)
            {
                v2f o;
                
                v.vertex = RotateZ( RotateX( RotateY(v.vertex, _Rotate.y), _Rotate.x), _Rotate.z);

                #if _USEBILLBOARD_ON
                    // apply object scale
                    v.vertex.xy *= float2(length(GetObjectToWorldMatrix()._m00_m10_m20), length(GetObjectToWorldMatrix()._m01_m11_m21));

        
                    // get the camera basis vectors
                    float3 forward = -normalize(GetWorldToViewMatrix()._m20_m21_m22);
                    float3 up = normalize(GetWorldToViewMatrix()._m10_m11_m12);
                    float3 right = normalize(GetWorldToViewMatrix()._m00_m01_m02);
        
                    // rotate to face camera
                    float4x4 rotationMatrix = float4x4(right, 0,
                        up, 0,
                        forward, 0,
                        0, 0, 0, 1);

                    v.vertex = mul(v.vertex, rotationMatrix);
        
                    // undo object to world transform surface shader will apply
                    v.vertex.xyz = mul((float3x3)GetWorldToObjectMatrix(), v.vertex.xyz);
                #endif
                o.vertex = TransformObjectToHClip(v.vertex.xyz );


                o.worldPos.xyz = TransformObjectToWorld(v.vertex.xyz);
                o.worldPos.w = ComputeFogFactor(o.vertex.z);
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);
                
                o.texcoord = v.texcoord;
                o.normal = TransformObjectToWorldNormal(v.normal);
                
                o.sPos = ComputeScreenPos(o.vertex);
                o.sPos.z = -TransformWorldToView(o.worldPos.xyz).z;


                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                Light mainLight = GetMainLight();

                float distortTex = SAMPLE_TEXTURE2D(_DistortionTex, sampler_DistortionTex, i.texcoord.xy * _DistortionTex_ST.xy - _Time.y * _DistortScrollSpeed.xy + _DistortionTex_ST.zw ).x;
                half4 mainTex = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord.xy * _MainTex_ST.xy + (distortTex * 2 - 1) * _DistortAmount + _MainTex_ST.zw ) * _Color;
                float3 bakedGI = SampleSH(i.normal);

        
                
            #if _USEDEPTHFADE_ON
                float sceneZ = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, sampler_CameraDepthTexture, i.sPos.xy / i.sPos.w ).x;
                if(unity_OrthoParams.w > 0)     //orthographic camera
                {
                    #if defined(UNITY_REVERSED_Z)
                    sceneZ = 1.0f - sceneZ;
                    #endif
                    sceneZ = (sceneZ * _ProjectionParams.z) + _ProjectionParams.y;
                }
                else    //perspective camera
                {
                    sceneZ = 1.0 / (_ZBufferParams.z * sceneZ + _ZBufferParams.w);  //LinearEyeDepth(sceneZ)
                }
                float pixelZ = i.sPos.z;
                float depthDiff = abs(sceneZ - pixelZ);

                float depthFade = saturate(_DepthFadeFactor * depthDiff);
                mainTex.a *= depthFade;
            
            #endif


                float mask = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.texcoord.xy * _MaskTex_ST.xy + _MaskTex_ST.zw + (distortTex * 2 - 1) * _MaskDistortAmount).x;
                mask = pow(abs(mask), 1 / _MaskPow );
                float4 c = float4(0,0,0,0);	

                c.rgb = mainTex.rgb;	
                c.a = mainTex.a * mask;

                c.rgb = MixFog(c.rgb, i.worldPos.w);
                c.rgb = lerp(c.rgb, 0, _ConvertToBlack);

                return c;

            }
            ENDHLSL
        }
    }
}

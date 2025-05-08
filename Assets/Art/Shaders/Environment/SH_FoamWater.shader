Shader "Environment/SH_FoamWater"
{
    Properties
    {
        _Color ("Color" , Color) = (1,1,1,1)
        _ShadowColor ("Shadow Color", Color) = (0.5,0.5,0.5,1)
        //_MainTex ("Texture", 2D) = "white" {}

        [Header(Distortion)]
        [NoScaleOffset]_NormalTex1 ("Normal Tex 1", 2D) = "bump"{}
        _NormalTex1ScaleOffset("Normal Tex 1 Scale Offset", Vector) = (1,1,0,0)
        _NormalStrength1 ("Normal Strength 1", float) = 1
        [NoScaleOffset]_NormalTex2 ("Normal Tex 2", 2D) = "bump"{}
        _NormalTex2ScaleOffset("Normal Tex 2 Scale Offset", Vector) = (1,1,0,0)
        _NormalStrength2 ("Normal Strength 2", float) = 1

        
        _FlowSpeed ("Flow Speed", Range(0,1)) = 0.15

        

        [Header(Specular)]
        _LightOffset ("Light Offset", Vector) = (0,0,0,0)
        _SpecularColor("Specular Color", Color) = (1,1,1,1)
        _SpecularPower ("Specular Power", float) = 32
        _SpecularThreshold("Specular Threshold", Range(0,1)) = 1
        _SpecularSmooth("Specular Smooth", Range(0,0.5)) = 0.25
        _SpecularIntensity("Specular Intensity", float) = 1
        

        [Header(Foam)]
        [Toggle]_UseFoam ("Use Foam", float) = 1
        _DepthColor ("Depth Color", Color) = (0.5,0.5,0.5,1.0)
        [PowerSlider(5.0)] _DepthDistance ("Depth Distance", Range(0.01,3)) = 0.5
        _DepthAlpha ("Depth Alpha", Range(0,1)) = 0.8
        [Space(10)]
		_FoamColor ("Foam Color (RGB) Opacity (A)", Color) = (0.9,0.9,0.9,1.0)
        _FoamTex ("Foam Texture", 2D) = "white" {}
        _FoamDistortAmount ("Foam Distortion Amount", Range(0,0.5)) = 0.1
        _FoamSpread ("Foam Spread", Range(0.01,5)) = 2
		_FoamStrength ("Foam Strength", Range(0.01,1)) = 0.8
		
		_FoamSmooth ("Foam Smoothness", Range(0,0.5)) = 0.02
		

        [Header(Waves Animation)]
		_WaveSpeed ("Speed", Float) = 1
		_WaveAmplitude ("Amplitude", Range(0.001,0.01)) = 0.005
		_WaveFrequency ("Frequency", Range(0,2)) = 1


    }
    SubShader
    {
        Tags { "Queue"="Transparent"  "RenderType"="Transparent" }
        LOD 100
        Blend SrcAlpha OneminusSrcAlpha
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #pragma multi_compile _ _USEFOAM_ON

            #include "UnityCG.cginc"

            //sampler2D _MainTex;
            sampler2D _NormalTex1;
            sampler2D _NormalTex2;
            sampler2D _FoamTex;

            //float4 _MainTex_ST;
            float4 _NormalTex1_ST;
            float4 _NormalTex2_ST;
            float4 _FoamTex_ST;

            sampler2D _CameraDepthTexture;
            
            float4 _Color, _ShadowColor, _DepthColor;
            float4 _FoamColor, _SpecularColor;
            float4 _NormalTex1ScaleOffset,_NormalTex2ScaleOffset, _LightOffset;
            float _FoamSmooth, _FoamSpread, _FoamStrength;
            float _NormalStrength1, _NormalStrength2;
            float _SpecularSmooth, _SpecularThreshold, _SpecularIntensity, _SpecularPower;
            float _DepthDistance, _DepthAlpha;
            float _FoamDistortAmount, _FlowSpeed;

            float _WaveAmplitude, _WaveFrequency, _WaveSpeed;



            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float3 normal : NORMAL;
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD2;  //alpha : float fogCoord  : TEXCOORD1;
                half3 viewDir : TEXCOORD3;
                float2 sinAnim : TEXCOORD4;
                float4 screenPos : TEXCOORD5;
                float3 tangent      : TEXCOORD6;
                float3 biTangent    : TEXCOORD7;
                
            };

            

            v2f vert (appdata v)
            {
                v2f o;
                
                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                
                


                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                o.viewDir = normalize(_WorldSpaceCameraPos.xyz - o.worldPos.xyz);

                float yOffset = sin( (o.worldPos.x + o.worldPos.z) * _WaveFrequency + _Time.y * _WaveSpeed) * _WaveAmplitude;
                o.vertex = UnityObjectToClipPos(v.vertex + float3(0,yOffset,0));


                float2 mainTexcoords = o.worldPos.xz * 0.1;
                o.uv = mainTexcoords;//TRANSFORM_TEX(mainTexcoords.xy, _MainTex);
                o.sinAnim = ((v.vertex.xy + v.vertex.yz) * _WaveFrequency) + (_Time.y * _WaveSpeed);
                
                o.normal = UnityObjectToWorldNormal(v.normal);
                
                
                o.screenPos = ComputeScreenPos(o.vertex);
                o.screenPos.z = -mul( UNITY_MATRIX_MV, v.vertex ).z ;//COMPUTE_EYEDEPTH(o.sPos.z);

                o.tangent = UnityObjectToWorldNormal(v.tangent.xyz);
                o.biTangent = cross(o.normal, o.tangent) * v.tangent.w;

                UNITY_TRANSFER_FOG(o,o.vertex);

                return o;


            }

            fixed4 frag (v2f i) : SV_Target
            {
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz);
                half4 col = 1;//tex2D(_MainTex, i.uv) ;
                half3 normal1 = UnpackNormal(tex2D(_NormalTex1, i.uv * _NormalTex1ScaleOffset.xy + _NormalTex1ScaleOffset.zw * _Time.y * _FlowSpeed)) * float3(_NormalStrength1, _NormalStrength1,1);
                half3 normal2 = UnpackNormal(tex2D(_NormalTex2, i.uv * _NormalTex2ScaleOffset.xy + _NormalTex2ScaleOffset.zw * _Time.y * _FlowSpeed)) * float3(_NormalStrength2, _NormalStrength2,1);
                half3 normalTSCombine = normalize(normal1 + normal2);
                float3x3 tangentToWorldMatrix = float3x3( i.tangent, i.biTangent, i.normal);
                float3 normalWS = normalize(mul(normalTSCombine, tangentToWorldMatrix));

                //shadow
                float halfLambert = max(0,dot(normalWS, lightDir));
                float3 shadowColor = lerp(_ShadowColor.rgb, 1, halfLambert);
                col.rgb *= shadowColor;
                
                

                //foam
                float ndotv = dot(normalWS, i.viewDir);
                #if _USEFOAM_ON
                    float sceneZ = tex2D(_CameraDepthTexture, i.screenPos.xy / i.screenPos.w );

                    half2 uvDistort = ((sin(0.9*i.sinAnim.xy) + sin(1.33*i.sinAnim.xy+3.14) + sin(2.4*i.sinAnim.xy+5.3))/3) * _WaveAmplitude;
                    half2 calculatedUv = i.uv.xy + uvDistort.xy + normalTSCombine.xy * _FoamDistortAmount;

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
                    float pixelZ = i.screenPos.z;
                    float depthDiff = abs(sceneZ - pixelZ) ;//* ndotv * 2;
                    half4 foam = tex2D(_FoamTex,  calculatedUv * _FoamTex_ST.xy );
                    half4 foam2 = tex2D(_FoamTex, calculatedUv * _FoamTex_ST.xy ) ;
                    half4 averageFoam = (foam + foam2) / 2;
                    float foamDepth = saturate(_FoamSpread * depthDiff);
                    half foamTerm = (smoothstep(foam.r - _FoamSmooth, averageFoam.r + _FoamSmooth, saturate(_FoamStrength - foamDepth)) * saturate(1 - foamDepth)) * _FoamColor.a;

                    col.rgb = lerp(_DepthColor.rgb, col.rgb , saturate(_DepthDistance * depthDiff + 1-_DepthAlpha) ) ;
                    
                    col.rgb = lerp(col.rgb * _Color.rgb, _FoamColor.rgb*2, foamTerm);
                    col.a = lerp(col.a * _Color.a, _FoamColor.a, foamTerm);   

                #else
                    col *= _Color;
                
                #endif

                //specular
                float3 halfVector = normalize(i.viewDir + normalize(lightDir + _LightOffset.xyz) );
                float ndoth = dot(normalWS, halfVector);
                float3 specular = smoothstep(_SpecularThreshold - _SpecularSmooth, _SpecularThreshold + _SpecularSmooth, pow(max(ndoth,0),_SpecularPower) ) * _SpecularIntensity * _SpecularColor.rgb;
                col.rgb += specular;



                
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
    
}

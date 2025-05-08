// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable

// Upgrade NOTE: commented out 'float4 unity_LightmapST', a built-in variable
// Upgrade NOTE: commented out 'sampler2D unity_Lightmap', a built-in variable
// Upgrade NOTE: replaced tex2D unity_Lightmap with UNITY_SAMPLE_TEX2D

        float4 _Color;

    #ifdef ENABLE_ARRAY
        UNITY_DECLARE_TEX2DARRAY(_MainTex);
        UNITY_DECLARE_TEX2DARRAY(_DataTex);
        UNITY_DECLARE_TEX2DARRAY(_DecalTex);
        //UNITY_DECLARE_TEX2DARRAY(_Lightmap);
        //UNITY_DECLARE_TEX2DARRAY(_ShadowMask);
    #else
        sampler2D _MainTex;
        sampler2D _DataTex;
        sampler2D _DecalTex;
        //sampler2D _Lightmap;
        //sampler2D _ShadowMask;
    #endif

    #ifdef _USENORMAL_ON
        sampler2D _NormalTex;
        float4 _NormalTex_ST;
        float _NormalStrength;
    #endif

        float4 _MainTex_ST;

        float _RealtimeShadowAmount;
        float4 _MedToneColor;
        float _MedToneSmooth;
        float _MedToneThreshold;
        float4 _ShadowColor;
        float _ShadowSmooth;
        float _ShadowThreshold;


        fixed _EmissiveMultiplier;
        fixed3 _EmissiveColor;
        

        
        fixed4 _AdditiveColor;
        half _Smoothness;
        half _Metallic;
        half _DiffuseMode;
        half _ClipYPosition;

        float _GIIntensity;
        float _Radiance;
        
        Texture2D _VariableTex;

        //matcap
        fixed _AddAmount;
        Texture2D _AddMatcapTex;
        SamplerState sampler_AddMatcapTex;
        
        fixed _UseMatcapOnly;
        fixed _MulAmount;
        Texture2D _MulMatcapTex;
        SamplerState sampler_MulMatcapTex;
        

        // sampler2D unity_Lightmap;
        // float4 unity_LightmapST;

        
        //line fx
        fixed3 _LinePos;
        fixed _LineWidth;
        fixed3 _LineColor;
        Texture2D _LinePattern;
        fixed4 _LinePattern_ST;
        SamplerState sampler_LinePattern;
        
        fixed _SecondLineWidth;
        fixed3 _SecondLineColor;

        //PBR
        fixed _SpecularIntensity;
        half _SpecularPower;
        fixed _RimSize;
        half _RimEnabled;
        fixed4 _RimColor;
        fixed _RimIntensity;
        fixed _RimGradation;
        half _ApplyDirectionalRim;
        half _CubemapBlurAmount;
        half _CubemapBlurSeperate;
        half _CubemapIntensity;

        //Outline
        half _Satuation;

        //UV Ani
        sampler2D _ScrollMaskTex;
        half4 _ScrollVector;
        half _ScrollSpeed;

        //hair
        Texture2D _AnisoTex;
        SamplerState sampler_AnisoTex;
        fixed4 _AnisoTex_ST;
        float4 _AnisoSpecularColor;
        float _AnisoSpecularIntensity;
        float _AnisoSpecularPower;
        float _AnisoSpecularSmooth;
        float _AnisoSpecularPosition;
        float _ShiftMax;
        float _ShiftMin;
        float4 _RotateTangent;

        
        sampler2D _NoiseTex;
        fixed4 _WindDirection;
        fixed4 _WaveControl;

        
        sampler2D _VCEffectTexR;
        fixed4 _VCEffectTexR_ST;
        
        sampler2D _VCEffectTexG;
        fixed4 _VCEffectTexG_ST;
        


        //SamplerState sampler_Lightmap;


        fixed _ShadowGradation;
        float _ShadowAttenuation;

        #ifdef ENABLE_SHOWDOWNFX
            uniform float4 _CircleLineColor;
            uniform float4 _SquareLineColor;
            uniform float _ShowDownLineWidth;
            uniform float4 _OutLineColor;
            uniform float4 _RedZoneColor;
            fixed3 _Center;
            fixed3 _RedZonePoint;
        #endif

        #ifdef ENABLE_CLOUD
            Texture2D _CloudTex;
            SamplerState sampler_CloudTex;
            fixed4 _CloudProp;
        #endif

        #ifdef ENABLE_COLORVAR
            float4 _MaskThreshold;
            float4 _CoverColor;
            float _MaskSmooth;
            float _MaskViewMode;

        #endif


        struct appdata
        {
            float4 vertex: POSITION;
            float2 uv: TEXCOORD0;
            float3 normal: NORMAL;
            #ifdef ENABLE_LIGHTMAP
                float2 lm: TEXCOORD1;
                #ifdef ENABLE_DECAL
                    float2 uv2: TEXCOORD2;
                #endif
            #else
                #ifdef ENABLE_DECAL
                    float2 uv2: TEXCOORD1;
                #endif
            #endif
            
            #ifdef ENABLE_WIND
                float3 color: COLOR;
            #endif
            float4 tangent: TANGENT;
        };

          
        
        struct v2f
        {
            float2 uv: TEXCOORD0;
            UNITY_FOG_COORDS(1)
            SHADOW_COORDS(2)
            float4 pos: SV_POSITION;
            float3 normal: NORMAL;
            float4 worldPos: TEXCOORD3;
            float3 viewDir: TEXCOORD4;
            float3 reflectVector: TEXCOORD5;
            float3 capUV: TEXCOORD6;

            #ifdef HAIR_ON
                float3 tangent: TANGENT;
                float3 bitangent: TEXCOORD7;
                float2 shiftUV: TEXCOORD8;
            #endif

            #ifdef _USENORMAL_ON
                float3 tangent : TEXCOORD7;
                float3 bitangent : TEXCOORD8;
                float2 normalUV : TEXCOORD9;
            #endif

            #ifdef ENABLE_SCANLINE
                fixed2 scanUV: TEXCOORD9;
            #endif

            #ifdef ENABLE_LIGHTMAP
                fixed2 lm: TEXCOORD10;
            #endif

             #ifdef ENABLE_DECAL
                float2 uv2: TEXCOORD11;
            #endif

            #ifdef ENABLE_CLOUD
                float3 cloudPos: TEXCOORD12;
            #endif

            #ifdef ENABLE_WIND
                float3 color: COLOR;
            #endif
        };



        UnityLight MainLight()
        {
            UnityLight light;
            
            light.color = _LightColor0.rgb;
            light.dir = _WorldSpaceLightPos0.xyz;
            
            return light;
        }



        half PerceptualRoughnessToMipmapLevel(half perceptualRoughness, float SpeccubeLOD)
        {
            perceptualRoughness = perceptualRoughness * (1.7 - 0.7 * perceptualRoughness);
            return perceptualRoughness * SpeccubeLOD;
        }

        half3 GlossyEnvironmentReflection(half3 reflectVector, half roughness, half occlusion)
        {
            half mip = PerceptualRoughnessToMipmapLevel( sqrt(roughness) , UNITY_SPECCUBE_LOD_STEPS );
            half4 encodedIrradiance = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, reflectVector, mip * lerp( roughness, _CubemapBlurAmount, _CubemapBlurSeperate) );
            half3 irradiance = DecodeHDR(encodedIrradiance, unity_SpecCube0_HDR);
            return irradiance * occlusion;
        }

        half3 EnvironmentBRDF(float roughness, float3 diffuse, float3 specular, half grazingTerm, half3 indirectDiffuse, half3 indirectSpecular, half fresnelTerm)
        {
            half3 c = indirectDiffuse * diffuse;
            float surfaceReduction = 1.0 / ( roughness * roughness + 1.0);
            c += surfaceReduction * indirectSpecular * lerp(specular, grazingTerm * _RimColor.rgb, fresnelTerm);
            return c;
        }



        half3 CalculateFresnel(float3 normal, float3 viewDir, float NdotL, float3 rimColor)
        {
            //Fresnel
            half NdotV = dot(normal, viewDir);
            half rimTerm = 1 - NdotV;
            half rimMode = NdotL;//1 - NdotL;
            
            rimTerm = smoothstep((1 - _RimSize*0.5) - _RimGradation, (1 - _RimSize*0.5) + _RimGradation, rimTerm) * _RimIntensity * rimMode * 0.5;

            
            return rimTerm * rimColor.rgb;
        }

        




        half3 PBR_BRDF_GI(float roughness, float metallic, float3 albedo, half occlusion, half3 normalWS, half3 lightDir, half3 lightColor, half3 viewDir, half3 indirectDiffuse, half3 ReflectVector, half3 smoothedShadowColor, half smoothedNdotL)
        {
            //diffuse & specularGGX
            float3 halfDir = normalize(float3(lightDir) + float3(viewDir));

            float oneMinusDielectricSpec = 0.96;
            float oneMinusReflectivityMetallic = oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
            half reflectivity = 1.0 - oneMinusReflectivityMetallic;
            float grazingTerm = saturate( (1-roughness) + reflectivity);

            float3 diffuse = albedo * oneMinusReflectivityMetallic;
            float3 specular = lerp( float3(0.04,0.04,0.04) , albedo, metallic);

            float NdotH = saturate(dot(normalWS, halfDir));
            half LdotH = saturate(dot(lightDir, halfDir));

            float roughness2MinusOne = roughness * roughness -1;
            float normalizationTerm = roughness * 4 + 2;

            float d = NdotH * NdotH * roughness2MinusOne + 1.00001f;

            half LdotH2 = LdotH * LdotH;
            half specularTerm = roughness * roughness / ((d * d) * max(0.1h, LdotH2) * normalizationTerm);

            specularTerm = lerp(0,clamp(specularTerm, 0.0, 100.0), _SpecularIntensity); 

            half3 pbr = (specularTerm * specular + diffuse) * smoothedShadowColor * lightColor;

            //GI
            half3 indirectSpecular = GlossyEnvironmentReflection(ReflectVector, roughness, occlusion) * _CubemapIntensity;

            //Fresnel
            //fixed NdotV = dot(normalWS, viewDir);
            //fixed rimTerm = 1 - NdotV;
            //float rimMode = 1 - smoothedNdotL;
            //rimTerm = smoothstep((1 - _RimSize) - _RimGradation, (1 - _RimSize) + _RimGradation, rimTerm) * _RimIntensity * rimMode ;

            half fresnelTerm = (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir)));

            half3 gi = EnvironmentBRDF ( roughness,  diffuse, specular, grazingTerm, indirectDiffuse, indirectSpecular, fresnelTerm);


            return pbr + gi;

        }

        half3 PBR_BRDF_GROUND_GI(float roughness, float metallic, float3 albedo, half occlusion, half3 normalWS, half3 lightDir, half3 lightColor, half3 viewDir, half3 indirectDiffuse, half3 ReflectVector, half3 smoothedShadowColor, half smoothedNdotL, float atten)
        {
            //diffuse & specularGGX
            float3 halfDir = normalize(float3(lightDir) + float3(viewDir));

            float oneMinusDielectricSpec = 0.96;
            float oneMinusReflectivityMetallic = oneMinusDielectricSpec - metallic * oneMinusDielectricSpec;
            half reflectivity = 1.0 - oneMinusReflectivityMetallic;
            float grazingTerm = saturate( (1-roughness) + reflectivity);

            //half grazingTerm = saturate( (1-roughness) + (metallic));

            float3 diffuse = albedo * oneMinusReflectivityMetallic;
            float3 specular = lerp( float3(0.04,0.04,0.04) , albedo, metallic);

            float NdotH = saturate(dot(normalWS, halfDir));
            half LdotH = saturate(dot(lightDir, halfDir));
            half NdotV = abs(dot(normalWS, viewDir));
            half NdotL = smoothedNdotL;

            float roughness2MinusOne = roughness * roughness -1;
            float normalizationTerm = roughness * 4 + 2;

            float d = NdotH * NdotH * roughness2MinusOne + 1.00001f;

            half LdotH2 = LdotH * LdotH;
            half specularTerm = roughness * roughness / ((d * d) * max(0.1h, LdotH2) * normalizationTerm);
            specularTerm = lerp(0,clamp(specularTerm, 0.0, 100.0), _SpecularIntensity); 

            
            half3 radiance = lightColor * (atten );

            half3 pbr = ((specularTerm * specular + diffuse) * smoothedShadowColor * lightColor) * radiance * (1+_Radiance);

            //atten = atten - lerp( 0, (1-atten), _ShadowAttenuation);

            //GI
            half3 indirectSpecular = GlossyEnvironmentReflection(ReflectVector, roughness, occlusion) * _CubemapIntensity;
            half fresnelTerm = (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir))) * (1.0 - saturate(dot(normalWS, viewDir)));
            half3 gi = EnvironmentBRDF ( roughness,  diffuse, specular, grazingTerm, indirectDiffuse, indirectSpecular, fresnelTerm);

            //half4 skyData = UNITY_SAMPLE_TEXCUBE_LOD(unity_SpecCube0, ReflectVector, lerp( roughness * 8, _CubemapBlurAmount * 8, _CubemapBlurSeperate) ) * _CubemapIntensity;
            //half3 skyColor = DecodeHDR(skyData, unity_SpecCube0_HDR);
            
            //pbr = ( smoothedNdotL * (lightColor * albedo)) * (1 - metallic) + specularTerm * lightColor;// * FresnelTerm(1, dotLH);

            //fixed surfaceReduction = 1.0 / (roughness * roughness + 1.0);
            //fixed3 sky = FresnelLerp(1, grazingTerm, NdotV) * lerp(0.2, 4, Pow4(1 - NdotV)) * skyColor * surfaceReduction;
            //ret = saturate(ret) * occlusion * atten + lerp(diffuseGI, sky, metallic + 0.2);
            pbr = saturate(pbr) * occlusion * atten;// + lerp(indirectDiffuse, indirectSpecular, metallic + 0.2)) ;

            //fixed rimTerm = 1 - NdotV;
            //rimTerm = smoothstep((1 - rimSize) - rimGradation, (1 - rimSize) + rimGradation, rimTerm) * rimIntensity * (1 - dotNL);
            //pbr += rimTerm * rimColor;

            return pbr + gi * (1 - _GIIntensity);

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

        float StrandSpecular(float4 T, float3 V, float3 L, float3 N, float shift, float exponent, float strength)
        {
            float3 Tan = RotateZ(float4(T.xyz,1), -90).xyz; 
            float3 Nor = N.xyz;
            float3 H = normalize(L+V);
            float3 B = -normalize(cross( Tan ,Nor) * T.w);
            B = normalize(B + Nor * shift );

            float dotBH =  dot(B, H);
            float sinBH = sqrt(1.0 - dotBH*dotBH );
            float dirAtten = smoothstep(-1, 0 , dotBH);
            return dirAtten * pow(sinBH, 1 / (exponent)+00001 ) * strength;
        }

        half3 PBR_Anisotrophy_GI(float4 T, float3 V, float3 L, float3 N , float Exponent, float Shift, float intensity,  float3 SpecularColor, float SpecularSmooth, float Position)
        {
            
            float3 lightPosition = L + float3(0,Position ,0);
            float3 specular = StrandSpecular( T, V, lightPosition , N, Shift, Exponent, intensity) ;
            float3 specularColor = smoothstep(0.5- SpecularSmooth, 0.5 + SpecularSmooth, specular) * SpecularColor;
            return specularColor;

        }



        v2f vert(appdata v)
        {
            v2f o;
            o.pos = UnityObjectToClipPos(v.vertex);
            o.uv = TRANSFORM_TEX(v.uv, _MainTex);
            o.normal = UnityObjectToWorldNormal(v.normal);
            o.worldPos = mul(unity_ObjectToWorld, v.vertex);
            o.viewDir = normalize(_WorldSpaceCameraPos - o.worldPos );
            o.reflectVector = reflect(o.viewDir, o.normal);//mul(unity_ObjectToWorld, o.viewDir);
            
            fixed3 viewNormal = mul((float3x3)UNITY_MATRIX_V, o.normal);
            o.capUV = viewNormal * 0.5 + 0.5;
            
            #ifdef ENABLE_SCANLINE
                o.scanUV = TRANSFORM_TEX(o.worldPos.xy, _LinePattern);
            #endif

            #ifdef ENABLE_LIGHTMAP
                o.lm = v.lm * unity_LightmapST.xy + unity_LightmapST.zw;
            #endif
            
            #ifdef ENABLE_DECAL
                o.uv2 = v.uv2;
            #endif

            #ifdef ENABLE_WIND
                o.color = v.color;

                fixed2 samplePos = o.worldPos.xz / _WaveControl.w;
                samplePos += _Time.x * _WaveControl.xz;
                fixed waveSample = tex2Dlod(_NoiseTex, fixed4(samplePos.xy, 0, 0)).r;
                
                o.worldPos.xyz += sin(waveSample * _WindDirection.xyz) * _WaveControl.xyz * _WindDirection.w * v.color.b;
                o.pos = UnityWorldToClipPos(o.worldPos);
            #endif

            #ifdef ENABLE_CLOUD
                o.cloudPos = o.worldPos;
                o.cloudPos.xz *= _CloudProp.xy * 0.001;
                o.cloudPos.xz += frac(fixed2(-_CloudProp.z, -_CloudProp.w) * _Time.y);
            #endif

            #ifdef _USENORMAL_ON
                o.tangent = UnityObjectToWorldNormal(v.tangent.xyz);
                o.bitangent = cross(o.normal, o.tangent) * v.tangent.w;
                o.normalUV = v.uv * _NormalTex_ST.xy + _NormalTex_ST.zw;

            #endif

            #ifdef HAIR_ON
                o.tangent = v.tangent;
                o.tangent.rgb = normalize(mul(unity_ObjectToWorld, float4(v.tangent.xyz, 0)));
                o.bitangent = cross(o.normal, o.tangent);
                o.shiftUV = TRANSFORM_TEX(v.uv, _AnisoTex);
            #endif

            TRANSFER_SHADOW(o);
            UNITY_TRANSFER_FOG(o, o.pos);
            
            return o;
        }

        
        

        
        
        fixed4 frag(v2f i): SV_Target
        {
            // sample the texture
            fixed3 uvw = fixed3(i.uv, floor(i.uv.x));

            float3 emissiveColor = float3(1,1,1);
            float shadowGradation = 0.5;
            float4 rimColor = float4(1,1,1,1);
            float rimIntensity = 1;

            #ifdef ENABLE_ARRAY
                fixed4 col = _MainTex.Sample(sampler_MainTex, uvw);
                fixed4 data = _DataTex.Sample(sampler_DataTex, uvw);

                emissiveColor = _VariableTex.Load(int3(0, uvw.z, 0));
                shadowGradation = _VariableTex.Load(int3(1, uvw.z, 0)).r;
                rimColor = _VariableTex.Load(int3(2, uvw.z, 0));
                rimIntensity = _VariableTex.Load(int3(3, uvw.z, 0)).r;

            #else
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 data = tex2D(_DataTex, i.uv);
                
            #endif
            
            fixed3 normal;
            #ifdef _USENORMAL_ON
                float3 normalTS = UnpackNormal(tex2D(_NormalTex, i.normalUV)).xyz * float3(_NormalStrength, _NormalStrength,1);
                float3x3 tangentToWorld = float3x3(i.tangent, i.bitangent, i.normal);
                normal = normalize(mul( normalTS, tangentToWorld));
            #else
                normal = normalize(i.normal);
            #endif

            #ifdef UVANIMATION_ON
                half uvMaskTex = tex2D(_ScrollMaskTex, i.uv); 
                fixed4 uvAnimatedCol = tex2D(_MainTex, i.uv + _ScrollVector.xy * _Time.y * _ScrollSpeed);
                col = lerp(col, uvAnimatedCol, uvMaskTex);
            #endif
            
            #ifdef ENABLE_DECAL
                fixed4 decal = tex2D(_DecalTex, i.uv2);
                #ifdef UVANIMATION_ON    
                    fixed4 uvAnimatedDecal = tex2D(_DecalTex, i.uv2 + _ScrollVector.xy * _Time.y * _ScrollSpeed);
                    decal = lerp(decal, uvAnimatedDecal, uvMaskTex );
                #endif
                col.rgb = lerp(col.rgb, decal.rgb, decal.a);
            #endif

            
            UnityLight light = MainLight();
            //float3 reflectVector = reflect(i.viewDir, normal);
            UNITY_LIGHT_ATTENUATION(atten, i, i.worldPos);
            
            #ifdef ENABLE_CLOUD
                // 윗면만 나오게 처리할지, 일정 높이 이상은 나오게 할 것인지 결정하면 될 듯.
                fixed2 cloudUV = fixed2(i.cloudPos.x, i.cloudPos.z);
                fixed cloud = _CloudTex.Sample(sampler_CloudTex, cloudUV);
                atten = min(atten, cloud);
            #endif
            float3 sh;
            #ifdef ENABLE_LIGHTMAP
                sh = DecodeLightmap(UNITY_SAMPLE_TEX2D(unity_Lightmap, i.lm.xy));
                fixed shadowMask = UNITY_SAMPLE_TEX2D(unity_ShadowMask, i.lm.xy).r;
                atten = min(atten, shadowMask);
            #else
                sh = ShadeSH9(fixed4(normal, 1));
            #endif
            
            //atten = atten - lerp( 0, (1-atten), _ShadowAttenuation);

            //Diffuse Light
            half ndotl = dot(normal, light.dir);
            float halfLambert = ndotl * 0.5 + 0.5;
            float shadow = halfLambert * lerp(1, atten ,_RealtimeShadowAmount);
            float smoothShadow = smoothstep(_ShadowThreshold - _ShadowSmooth, _ShadowThreshold + _ShadowSmooth, shadow);
            float medTone = smoothstep(_MedToneThreshold - _MedToneSmooth, _MedToneThreshold + _MedToneSmooth, shadow);
            float3 ShadowColor = lerp( lerp(_ShadowColor.rgb, _MedToneColor.rgb, medTone), float3(1,1,1), smoothShadow);

        #ifdef HAIR_ON
            //diffuse
            col.rgb *= (ShadowColor + sh);
            
            float ao = 1;//data.b;

            //aniso Specular
            float anisoOffset = data.r*4-2;
            float shiftTex = _AnisoTex.Sample(sampler_AnisoTex, i.uv * _AnisoTex_ST.xy + _AnisoTex_ST.zw).r;
            float Shift = lerp(_ShiftMin, _ShiftMax, shiftTex);
            col.rgb += PBR_Anisotrophy_GI( float4(normalize(i.tangent),1) , i.viewDir, light.dir, normal , _AnisoSpecularPower, Shift, _AnisoSpecularIntensity, _AnisoSpecularColor.rgb, _AnisoSpecularSmooth, _AnisoSpecularPosition + anisoOffset) * smoothShadow * ao;

        #else

            #if _DIFFUSEMODE_ON
                
                #ifdef ENABLE_GROUND
                    col.rgb *= ( saturate(ndotl) * atten + sh);
                #else
                    col.rgb *= (ShadowColor + sh );
                #endif

                //디퓨즈 모드에 프레넬을 넣을지 말지 고민..

            #else
                //PBR MODE
                //fixed4 data = tex2D(_DataTex, i.uv);
                fixed smoothness = data.r * _Smoothness;
                fixed roughness = (1 - smoothness);
                fixed metallic = data.g * _Metallic;
                fixed ao = data.a;
                fixed emissive = data.b;
                
                #ifdef UVANIMATION_ON
                    fixed4 uvAnimatedData = tex2D(_DataTex, i.uv + _ScrollVector.xy * _Time.y * _ScrollSpeed);
                    data = lerp(data, uvAnimatedData, uvMaskTex);
                #endif
            
                #ifdef ENABLE_GROUND
                    smoothShadow = saturate(ndotl);
                    float3 pbnpr = PBR_BRDF_GROUND_GI(roughness, metallic, col.rgb, ao, normal,  light.dir , light.color, i.viewDir, sh, -i.reflectVector, smoothShadow, smoothShadow, atten);
                    pbnpr += lerp(half3(0,0,0),CalculateFresnel(normal, i.viewDir, 1-smoothShadow, _RimColor.rgb * rimColor.rgb * rimIntensity),_RimEnabled);
                #else
                    float3 pbnpr = PBR_BRDF_GI( roughness, metallic, col.rgb, ao, normal,  light.dir , light.color, i.viewDir, sh, -i.reflectVector, ShadowColor, smoothShadow);
                    pbnpr += lerp(half3(0,0,0),CalculateFresnel(normal, i.viewDir, smoothShadow, _RimColor.rgb),_RimEnabled);
                #endif

                col.rgb = pbnpr;
                col.rgb += emissive * emissive * _EmissiveColor * _EmissiveMultiplier * emissiveColor;

            #endif

            #ifdef ENABLE_COLORVAR
                    float mask = (smoothstep(col.r-_MaskSmooth, col.r+_MaskSmooth, _MaskThreshold.r) * smoothstep(col.g-_MaskSmooth ,col.g +_MaskSmooth, _MaskThreshold.g)) * (1-smoothstep(col.b-_MaskSmooth, col.b+_MaskSmooth,1-_MaskThreshold.b)) ;
                    col.rgb = lerp(col.rgb, _CoverColor.rgb, mask );
                    //return mask;   
            #endif
            
        #endif

            //col.rgb += lerp(0,CalculateFresnel(normal, i.viewDir, smoothShadow),_RimEnabled);

            // FX
            // Additive
        #ifdef ENABLE_ADD
            fixed3 addMatcap = _AddMatcapTex.Sample(sampler_AddMatcapTex, i.capUV) * _AddAmount;
            col.rgb += addMatcap;
        #endif
        
            // Multiply
        #ifdef ENABLE_MUL
            fixed3 mulMatcap = _MulMatcapTex.Sample(sampler_MulMatcapTex, i.capUV) * _MulAmount;
            col.rgb = lerp(col.rgb * mulMatcap, mulMatcap * 2, _UseMatcapOnly);
        #endif
        
            // ScanLine
        #ifdef ENABLE_SCANLINE
            // 특정 위치에서만 라인이 나올 수 있게, 마스크가 되는 그래프 생성
            fixed a = ceil(1 - saturate(i.worldPos.z + 1 - _LinePos.z));
            // 실제로 스캔라인 생기는 공식
            fixed b = saturate(i.worldPos.z + 1 - _LinePos.z);
            fixed lineBase = saturate(a * b); // 대략 __/‾‾ 이런 식의 그래프
            
            fixed lineAmount = pow(lineBase, _LineWidth);
            
            fixed linePattern = _LinePattern.Sample(sampler_LinePattern, i.scanUV);
            fixed3 scanLine = _LineColor * lineAmount * linePattern;
            
            fixed secondLineAmount = pow(lineBase, _SecondLineWidth);
            fixed3 secondScanLine = _SecondLineColor * secondLineAmount;
            
            col.rgb += max(scanLine.rgb, secondScanLine);
        #endif
            
            col += _AdditiveColor;

        #ifdef ENABLE_CLIP
            clip(i.worldPos.y - _ClipYPosition);
        #endif

        #ifdef ENABLE_GROUND
            col *= 1;
        #else
            col *= _Color;
        #endif
            
            
        #ifdef SATUATION_ON
            col.rgb = lerp( col.r *0.29 + col.g * 0.59 + col.b * 0.12, col.rgb, float3(1,1,1)*_Satuation);
        #endif

        #ifdef ENABLE_SHOWDOWNFX
            //SquareFX
            fixed2 localPos = i.worldPos.xz - _Center.xz;

            float XPos = abs((localPos.x - localPos.y) * sqrt(2));
            float ZPos = abs((localPos.x + localPos.y) * sqrt(2));

            float SquareLine = (_Center.y-_ShowDownLineWidth<XPos || _Center.y-_ShowDownLineWidth<ZPos)? 1 : 0;
            float SquareOutLine = (XPos<_Center.y && ZPos<_Center.y)? 1 : 0;

            col.rgb = col.rgb * (1 - SquareLine) + SquareLine * SquareOutLine * _SquareLineColor.rgb + col.rgb * (1 - SquareOutLine) * _OutLineColor;

            //RedZone CircleFX
            float redZoneDistance = distance(i.worldPos.xz, _RedZonePoint.xz);
            float RedZoneLine = (_RedZonePoint.y-_ShowDownLineWidth*0.5<redZoneDistance)? 1 : 0;
            float RedZoneOutLine = (redZoneDistance<_RedZonePoint.y)? 1 : 0;

            col.rgb = col.rgb * (1 - RedZoneOutLine) + RedZoneLine * RedZoneOutLine * _CircleLineColor.rgb + col.rgb * (1 - RedZoneLine) * _RedZoneColor;
        #endif

        #ifdef ENABLE_COLORVAR
            col.rgb = lerp(col.rgb, mask, _MaskViewMode);
        #endif
            // apply fog
            UNITY_APPLY_FOG(i.fogCoord, col);

            
            return col;
        }
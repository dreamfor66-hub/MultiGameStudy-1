Shader "Mobile/Particles/Master"
{
    Properties
    {
        _MainTex ("Main Texture", 2D) = "white" { }
        _PanDir ("MainTex Panning Direction (XY)", Vector) = (0,0,0,0)
        [Toggle] _HDRParticle("HDR Particle Mode", float) = 0
        [Toggle] _RGBMode("RGB Mode", float) = 0
        [Toggle] _IsTrail("UV Scrolling ( Trail Mode )", float) = 0
        
        
        [Space(10)]
        
        

        [Header(Dissolve)]
        [KeywordEnum(None, Hard, Soft)] _DissolveMode ("Dissolve Mode", float) = 0
        [Toggle] _MaskToggle ("Mask Sampling : None - R, Soft/Hard - G Channel", float) = 0
        _DissolveTex ("Dissolve Texture", 2D) = "white" { }
        _DissolvePanDir ("Dissolve Panning Direction (XY)", Vector) = (0,0,0,0)
        [Header(Dissolve Outline)]
        [Toggle] _EnableDissolveOutline("Enable Dissolve Outline (Hard Only)", float) = 0
        [HDR] _DissolveOutlineColor("Dissolve Outline Color ", Color) = (1,1,1,1)
        _DissolveOutlineWidth ("Dissolve Outline Width ", float) = 0

        [Space(20)]
        [Header(Distortion Tex)]
        [Toggle] _SecondTexMode ("Enable Distortion Texture", float) = 0
        _DistortTex ("Distortion Texture", 2D) = "gray" {}
        
        _DistortionPanDir("Distortion Panning Direction (XY)", Vector) = (0,0,0,0)
        _DistortAmount ("Distortion Amount", float) = 0


        [Space(20)]
        [KeywordEnum(Add, AlphaBlend)] _BlendMod("Blend Mode", float) = 0
        [Enum(Add,1,AlphaBlend,10)] _DestBlend ("DestBlend", float) = 10

        [Space(10)]

        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", float) = 0
        [Toggle] _ZWrite ("ZWrite", float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Background+2000" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
        
        Pass
        {
            Blend SrcAlpha [_DestBlend]
            Cull [_Cull] Lighting Off ZWrite [_ZWrite] ZTest [_ZTest] Fog
            {
                Mode Off
            }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            #pragma multi_compile _DISSOLVEMODE_NONE _DISSOLVEMODE_HARD _DISSOLVEMODE_SOFT
            #pragma multi_compile _ _SECONDTEXMODE_ON
            #pragma multi_compile _BLENDMOD_ADD _BLENDMOD_ALPHABLEND
            #pragma multi_compile _ _ENABLEDISSOLVEOUTLINE_ON
            
            #include "UnityCG.cginc"
            
            
            struct appdata
            {
                float4 vertex: POSITION;
                float4 uv: TEXCOORD0;
                float2 uv2: TEXCOORD1;
                float4 customData2: TEXCOORD2;
                fixed4 color: COLOR;
            };
            
            struct v2f
            {
                float3 uv: TEXCOORD0;
                fixed hdr: TEXCOORD1;
                float4 customData2 : TEXCOORD2;
                float4 uv_dissolve : TEXCOORD3;
                float2 uv_distort : TEXCOORD4;
                float4 vertex: SV_POSITION;
                fixed4 color: COLOR;
            };
            
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            fixed4 _MainTex_ST;
            uniform float _IsEnemy;
            float _RGBMode;

            Texture2D _DissolveTex;
            SamplerState sampler_DissolveTex;
            fixed4 _DissolveTex_ST;
            
            Texture2D _DistortTex;
            SamplerState sampler_DistortTex;
            fixed4 _DistortTex_ST;

            float4 _Color, _EnemyColor;
            float3 _PanDir;
            float3 _DistortionPanDir, _DissolvePanDir;
            half _DistortAmount;
            float _MaskToggle;
            half _DistortFactor, _DistortionScroll;
            half _HDRParticle, _IsTrail;
            half4 _DissolveOutlineColor;
            half _DissolveOutlineWidth;

            float Average(float3 color)
            {
                return color.r*0.33 + color.g * 0.33 + color.b*0.33;
            }
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.customData2 = v.customData2;

                #if _DISSOLVEMODE_NONE
                    float dissolveData = 0;
                    float panningData = lerp(v.uv.z, v.uv.w, _HDRParticle);

                #else
                    float dissolveData = lerp(v.uv.z, v.uv.w, _HDRParticle);
                    float panningData = lerp(v.uv.w, v.customData2.x, _HDRParticle);

                #endif
                    float hdrData = lerp(0, v.uv.z, _HDRParticle);

                float2 offset = float2(_PanDir.x, _PanDir.y) * lerp( panningData, _Time.y, _IsTrail);


                o.uv = fixed3(TRANSFORM_TEX(v.uv.xy, _MainTex), dissolveData );
                o.uv.xy += offset;
                o.hdr = hdrData;

                o.uv_dissolve.xy = v.uv.xy * _DissolveTex_ST.xy  + _DissolveTex_ST.zw + _DissolvePanDir.xy * v.customData2.y;
                o.uv_dissolve.zw = v.uv.xy;
                o.uv_distort = v.uv.xy * _DistortTex_ST.xy + _DistortTex_ST.zw;
                
                o.color = v.color;
                o.color.rgb += o.color.rgb * hdrData;
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                // sample the texture
                fixed4 col;
                
                float2 calculatedUV = i.uv.xy;
                float PanningMaskTex = 1;

                #if _SECONDTEXMODE_ON
                    float2 noiseUV = float2(_DistortionPanDir.x, _DistortionPanDir.y) * i.customData2.z;
                    calculatedUV = calculatedUV + (_DistortTex.Sample(sampler_DistortTex, i.uv_distort + noiseUV).rgb - 0.5) * _DistortAmount;
                #endif
                
                
                col = _MainTex.Sample(sampler_MainTex, calculatedUV) * PanningMaskTex;

                float dissolveCalculate;
                float3 dissolveInv;

                #if _DISSOLVEMODE_NONE
                    dissolveCalculate = 1;

                    fixed mask = _DissolveTex.Sample(sampler_DissolveTex, i.uv_dissolve.zw).g;
                    mask = lerp(1,mask,_MaskToggle);

                #elif _DISSOLVEMODE_HARD

                    fixed dis = _DissolveTex.Sample(sampler_DissolveTex, i.uv_dissolve.xy).r;
                    float dissolveHard = saturate(dis > i.uv.z);
                    #if _ENABLEDISSOLVEOUTLINE_ON
                        float outlineWidth = _DissolveOutlineWidth;
                    #else
                        float outlineWidth = 0;
                    #endif
                    float outlineHard = saturate(dis + outlineWidth > i.uv.z);

                    dissolveInv = saturate(outlineHard - dissolveHard);

                    dissolveCalculate = outlineHard;

                    fixed mask = _DissolveTex.Sample(sampler_DissolveTex, i.uv_dissolve.zw).g;
                    mask = lerp(1,mask, _MaskToggle);

                #elif _DISSOLVEMODE_SOFT

                    fixed dis = _DissolveTex.Sample(sampler_DissolveTex, i.uv_dissolve.xy).r;
                    float dissolveSoft = saturate(dis - (i.uv.z * 2 - 1));
                    dissolveCalculate = dissolveSoft;

                    fixed mask = _DissolveTex.Sample(sampler_DissolveTex, i.uv_dissolve.zw).g;
                    mask = lerp(1,mask, _MaskToggle);

                #endif

                col.rgb = lerp(col.rrr, col.rgb, _RGBMode * (1-_IsEnemy) );

                #if _BLENDMOD_ADD
                    #if _DISSOLVEMODE_NONE
                        col.rgb = lerp(saturate(col.rgb),col.rgb,_HDRParticle );
                        col.a *= dissolveCalculate * mask;
                    #else   //Soft & Hard
                        col = lerp(saturate(col),col, _HDRParticle) * mask;
                        col *= dissolveCalculate;
                    #endif
                #else   //AlphaBlend
                    col.rgb = lerp(saturate(col.rgb),col.rgb,_HDRParticle);
                    col.a *= dissolveCalculate * mask;
                #endif

                float3 enemyVertexColor = lerp(float3(0,0,0), float3(1, 0.09, 0.1), saturate(Average(i.color.rgb)*2) ) * lerp(1, 1+i.hdr, _HDRParticle);
                col.rgb *= lerp(i.color.rgb, enemyVertexColor, _IsEnemy);

                #if _DISSOLVEMODE_HARD
                    col.rgb = lerp(col.rgb, _DissolveOutlineColor.rgb, dissolveInv);
                #endif

                col.a *= i.color.a;

                return col ;
            }
            ENDCG
            
        }
    }
}

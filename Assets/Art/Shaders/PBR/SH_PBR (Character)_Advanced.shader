Shader "Custom/PBR (Character)_Advanced"
{
    Properties
    {
        [Header(Basic PBR Term)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture", 2D) = "white" { }
        _DataTex ("Data Texture (R: Smoothness, G: Metallic, B: Emissive, A: AO)", 2D) = "white" { }
        _Smoothness("Smoothness", Range(0,1)) = 1
        _Metallic("Metallic", Range(0,1)) = 1

        [Haeder(Shadow)]

        _RealtimeShadowAmount("Realtime Shadow Amount", Range(0,1)) = 1
        _MedToneColor ("Med Tone Color", Color) = (0.6,0.6,0.6,1)
        _MedToneThreshold("Med Tone Threshold", Range(0,1)) = 0.5
        _MedToneSmooth("Med Tone Smooth", Range(0,0.5)) = 0.05
        _ShadowColor("Shadow Color", Color) = (0.3,0.3,0.3,1)
        _ShadowThreshold("Shadow Threshold", Range(0,1)) = 0.5
        _ShadowSmooth("Shadow Smooth", Range(0,0.5)) = 0.05

        [Space(20)]
        _EmissiveMultiplier ("Emissive Multiplier", float) = 1
        [HDR] _EmissiveColor ("Emissive Color", Color) = (0, 0, 0, 1)
        
        [Header(Reflection)]
        [Toggle]_DiffuseMode ("Diffuse Mode", float) = 0
        _Metallic ("Metallic", Range(0,1)) = 1
        _Smoothness ("Smoothness", Range(0,1)) = 1
        [Space(10)]
        _SpecularIntensity ("Specular Intensity", Range(0, 1)) = 1
        [Toggle]_CubemapBlurSeperate ("Cubemap Blur Seperate", float) = 0
        _CubemapBlurAmount ("Cubemap Blur Amount", Range(0,1)) = 0
        _CubemapIntensity( "Cubemap Intensity", Range(0,2)) = 1

        [Header(RimLight)]
        [Toggle]_RimEnabled ("Enable RimLight", float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimSize ("Rim Size", Range(0, 1)) = 1
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 0
        _RimGradation ("Rim Gradation", Range(0, 1)) = 1
        
        [Header(Decal)]
        [Toggle(ENABLE_DECAL)] _DecalEnabled ("Enable Decal", float) = 0
        _DecalTex ("Decal Texture", 2D) = "black" { }
        
        
        [Header(FX Matcap Additive)]
        [Toggle(ENABLE_ADD)] _AddEnabled ("Enable Additive Matcap", float) = 0
        [NoScaleOffset] _AddMatcapTex ("Additive Matcap: RGB", 2D) = "black" { }
        _AddAmount ("Additive Amount", Range(0, 5)) = 1
        
        [Header(FX Matcap Multiplicative)]
        [Toggle(ENABLE_MUL)] _MulEnabled ("Enable Multiplicative Matcap", float) = 0
        [Toggle(ENABLE_MATCAP_ONLY)] _UseMatcapOnly ("Use Matcap Only", float) = 0
        [NoScaleOffset] _MulMatcapTex ("Multiplitive Matcap: RGB", 2D) = "white" { }
        _MulAmount ("Multiplicative amount", Range(0, 5)) = 1
        

        [Header(Scan Line)]
        [Toggle(ENABLE_SCANLINE)] _ScanLineEnabled ("Enable Scan Line", float) = 0
        _LinePos ("Line Position", Vector) = (0, 0, 0, 0)
        _LineWidth ("Line Width", float) = 1
        [HDR] _LineColor ("Line Color", Color) = (1, 1, 1, 1)
        _LinePattern ("Line Pattern", 2D) = "white" { }
        
        _SecondLineWidth ("Second Line Width", float) = 1
        [HDR] _SecondLineColor ("Second Line Color", Color) = (1, 1, 1, 1)

        [Header(Position Clipping)]
        [Toggle(ENABLE_CLIP)] _EnableClip ("Enable Clipping", float) = 0
        _ClipYPosition("Clip Y Position", float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        
        Pass
        {
            Lighting On
            Tags { "LightMode" = "ForwardBase" }
            CGPROGRAM
            
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            #pragma multi_compile_fog
            
            #pragma multi_compile _ ENABLE_MUL
            #pragma multi_compile _ ENABLE_ADD
            #pragma multi_compile _ ENABLE_SCANLINE
            #pragma multi_compile _ ENABLE_DECAL
            #pragma multi_compile _ ENABLE_CLIP
            #pragma multi_compile _ _DIFFUSEMODE_ON
            
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "PBR_Advance.cginc"
            
            

            ENDCG
            
        }
    }
    Fallback "Diffuse"
}
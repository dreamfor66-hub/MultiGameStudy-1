Shader "Custom/PBR (Hair)_Advanced"
{
    Properties
    {
        [Header(Basic Term)]
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Base Texture", 2D) = "white" { }
        _DataTex ("Data Texture (R: AnisoOffset)", 2D) = "white" { }
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



        [Header(RimLight)]
        [Toggle]_RimEnabled ("Enable RimLight", float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimSize ("Rim Size", Range(0, 1)) = 1
        _RimIntensity ("Rim Intensity", Range(0, 5)) = 0
        _RimGradation ("Rim Gradation", Range(0, 1)) = 1
        
        [Header(Anisotropy)]
        //_RotateTangent("Rotate Tangent", vector) = (0,0,0,0)
        _AnisoTex ("Anisotropic Direction", 2D) = "white" { }
        _AnisoSpecularColor("Specular Color", Color) = (0.5,0.5,0.5,1)
        _AnisoSpecularPosition("Specular Position Offset", Range(-4,4) ) = 0
        _AnisoSpecularSmooth ("Specular Smooth", Range(0,0.5)) = 0.5 
        _AnisoSpecularPower ("Specular Power", Range(0,0.05)) = 0.02
        _AnisoSpecularIntensity ("Specular Intensity", float) = 1
        _ShiftMin("Shift Min", float) = -0.9
        _ShiftMax("Shift Max", float) = 0.9

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

  
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry+1" }
        
        Pass
        {
            Stencil
            {
                Ref 2
                
            }
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
            #pragma multi_compile _ HAIR_ON
            #define HAIR_ON 1
    
            
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "PBR_Advance.cginc"

            
            
            
            ENDCG
            
        }
    }
    Fallback "Diffuse"
}
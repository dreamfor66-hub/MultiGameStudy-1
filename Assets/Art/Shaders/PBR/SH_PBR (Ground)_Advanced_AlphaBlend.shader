Shader "Custom/PBR (Ground)_Advanced_AlphaBlend"
{
    Properties
    {
        [Header(Basic PBR Term)]
        
        _MainTex ("Base Texture(RGB) Alpha(A)", 2D) = "white" { }
        _DataTex ("R: Smoothness, G: Metallic, B: Emissive, A: AO", 2D) = "white" { }
        _Smoothness("Smoothness", Range(0,1)) = 1
        _Metallic("Metallic", Range(0,1)) = 1

        [Header(Normal)]
        [Toggle]_UseNormal ("Use Normal Texture", float) = 0
        _NormalTex("Normal Texture", 2D) = "bump"{}
        _NormalStrength("Normal Strength", float) = 1

        [Space(20)]
        _EmissiveMultiplier ("Emissive Multiplier", float) = 1
        [HDR] _EmissiveColor ("Emissive Color", Color) = (0, 0, 0, 1)

        [Header(Reflection)]
        [Toggle]_DiffuseMode ("Diffuse Mode", float) = 0
        [Toggle]_CubemapBlurSeperate ("Cubemap Blur Seperate", float) = 0
        _CubemapBlurAmount ("Cubemap Blur Amount", Range(0,1)) = 0
        _CubemapIntensity( "Cubemap Intensity", Range(0,2)) = 1

        [Header(Wind)]
        [Toggle(ENABLE_WIND)] _LeafMoveEnabled ("Enable Wind", float) = 0
        _NoiseTex ("NoiseTex", 2D) = "white" { }
        _WindDirection ("Wind Direction (xyz, w: Strength)", Vector) = (0, 0, 0, 1)
        _WaveControl ("Wave Control", Vector) = (0, 0, 0, 1)

        [Header(Cloud)]
        [MaterialToggle(ENABLE_CLOUD)] _ENABLE_CLOUD ("Enable Cloud", Range(0, 1)) = 0
        _CloudTex ("Cloud Texture", 2D) = "black" { }
        _CloudProp ("Cloud Properties X, Y: Tiling, Z, W: Speed", Vector) = (0, 0, 0, 0)
        
        [Header(Lightmap)]
        [Toggle(ENABLE_LIGHTMAP)] _LightmapEnable ("Enable Lightmap", float) = 0
        _Lightmap ("Lightmap", 2D) = "gray" { }
        _ShadowMask ("Shadow Mask", 2D) = "white" { }
        
        [Header(Magic Number(Careful))]
        _SpecularIntensity ("Specular Intensity", Range(0, 1)) = 1
        
        //[Header(Decal)]
        //[Toggle(ENABLE_DECAL)] _DecalEnabled ("Enable Decal", float) = 0
        //_DecalTex ("Decal Texture", 2D) = "black" { }

        [Header(Shadow)]
        
        _ShadowGradation ("Shadow Gradation", Range(0, 1)) = 0.5
        
        [Header(Rim)]
        [Toggle]_RimEnabled ("Enable RimLight", float) = 1
        _RimColor ("Rim Color", Color) = (1, 1, 1, 1)
        _RimSize ("Rim Size", Range(0, 1)) = 1
        _RimIntensity ("Rim Intensity", Range(0, 1)) = 0
        _RimGradation ("Rim Gradation", Range(0, 1)) = 1
        

        //[Space(10)]
        //[Header(ShowdownFX)]
        //[Toggle(ENABLE_SHOWDOWNFX)] _ShowdownFx ("Enable ShowdownFx", float) = 0
        //[HDR]_CircleLineColor ("Circle Line Color", Color) = (0,0,0,0)
        //[HDR]_SquareLineColor ("Square Line Color", Color) = (0,0,0,0)
        //_LineWidth ("Line Width", Range(0,1)) = 0.1
        //[HDR]_OutLineColor ("OutZone Color", Color) = (0,0,0,0)
        //[HDR]_RedZoneColor ("RedZone Color", Color) = (0,0,0,0)

        [Header(Depth)]
        [Toggle] _ZWrite ("Depth Write ", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull("Cull Mode", Float) = 2


        
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }
        
        Pass
        {

            Lighting On
            Tags { "LightMode" = "ForwardBase" "RenderType" = "Transparent" "Queue" = "Transparent"}
            Blend SrcAlpha OneMinusSrcAlpha
            Cull [_Cull]
            Zwrite [_Zwite]
            CGPROGRAM
            
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fwdbase
            #pragma target 3.0
            #pragma multi_compile_fog
            
            #pragma multi_compile _ ENABLE_CLOUD

            //#pragma multi_compile _ ENABLE_DECAL
            #pragma multi_compile _ ENABLE_LIGHTMAP
            #pragma multi_compile _ ENABLE_WIND
            //#pragma multi_compile _ ENABLE_SHOWDOWNFX
            #pragma multi_compile _ _DIFFUSEMODE_ON
            #pragma multi_compile _ _USENORMAL_ON

            #define ENABLE_GROUND 1
            
            #include "UnityCG.cginc"
            #include "Lighting.cginc"
            #include "AutoLight.cginc"
            #include "PBR_Advance.cginc"
            
            
            ENDCG
            
        }
    } 
    
}
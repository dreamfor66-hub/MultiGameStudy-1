Shader "Mobile/Particles/Alpha Blended"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        [Toggle] _IsEnemy("Is Enemy", float) = 0
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", float) = 0
        [Toggle] _ZWrite ("ZWrite", float) = 0
    }
    CGINCLUDE
    
    #include "UnityCG.cginc"
    
    sampler2D _MainTex;
    fixed4 _MainTex_ST;
    uniform float _IsEnemy;
    
    struct appdata_t
    {
        fixed4 position: POSITION;
        fixed4 texcoord: TEXCOORD0;
        fixed4 color: COLOR;
    };
    
    struct v2f
    {
        fixed4 position: SV_POSITION;
        fixed2 texcoord: TEXCOORD0;
        fixed4 color: COLOR;
    };
    
    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
        o.color = v.color;
        return o;
    }
    
    fixed4 frag(v2f i): SV_Target
    {
        fixed4 col = tex2D(_MainTex, i.texcoord) ;
        col.rgb *= lerp(i.color.rgb, float3(1,0.22,0.26) , _IsEnemy);
        col.a *= i.color.a;
        return col;
    }
    
    ENDCG
    
    SubShader
    {
        Tags { "Queue" = "Background+2000" "IgnoreProjector" = "True" "RenderType" = "Transparent" "PreviewType" = "Plane" }
        
        Blend SrcAlpha OneMinusSrcAlpha
        Cull [_Cull] Lighting Off ZWrite [_ZWrite] ZTest [_ZTest] Fog
        {
            Mode Off
        }
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            ENDCG
            
        }
    }
}
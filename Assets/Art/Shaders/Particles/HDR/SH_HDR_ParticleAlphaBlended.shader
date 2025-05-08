Shader "Mobile/Particles/Alpha Blended-HDR"
{
    Properties
    {
        _MainTex ("Particle Texture", 2D) = "white" { }
        [Toggle] _IsEnemy("Is Enemy", float) = 0
        //[Header(Test)]
        //_EnemyColor ("Enemy Color", Color) = (1,0,0,1)


        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", float) = 0
        [Toggle] _ZWrite ("ZWrite", float) = 0
    }
    
    CGINCLUDE
    
    #include "UnityCG.cginc"
    
    sampler2D _MainTex;
    float4 _MainTex_ST;
    uniform float _IsEnemy;
    half4 _EnemyColor;
    
    struct appdata_t
    {
        float4 position: POSITION;
        float3 texcoord: TEXCOORD0;
        fixed4 color: COLOR;
    };
    
    struct v2f
    {
        float4 position: SV_POSITION;
        float2 texcoord: TEXCOORD0;
        float hdr : TEXCOORD1;
        fixed4 color: COLOR;
    };
    
    v2f vert(appdata_t v)
    {
        v2f o;
        o.position = UnityObjectToClipPos(v.position);
        o.texcoord = TRANSFORM_TEX(v.texcoord.xy, _MainTex);
        v.color.rgb += v.color.rgb * v.texcoord.z;
        o.hdr = v.texcoord.z;
        o.color = v.color;
        return o;
    }
    
    fixed4 frag(v2f i): SV_Target
    {
        fixed4 col = tex2D(_MainTex, i.texcoord) ;
        col.rgb *= lerp(i.color.rgb, float3(1,0.22,0.26) * (1 + i.hdr), _IsEnemy);
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
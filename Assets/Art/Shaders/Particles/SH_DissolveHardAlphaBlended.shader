Shader "Mobile/Particles/Dissolve Hard (Alpha Blended)"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _DissolveTex ("Dissolve Texture", 2D) = "white" { }
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull Mode", float) = 0
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", float) = 0
        [Toggle] _ZWrite ("ZWrite", float) = 0
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Background+2000" "IgnoreProjector" = "True" "PreviewType" = "Plane" }
        
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            Cull [_Cull] Lighting Off ZWrite [_ZWrite] ZTest [_ZTest] Fog
            {
                Mode Off
            }
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_particles
            
            #include "UnityCG.cginc"
            
            
            struct appdata
            {
                float4 vertex: POSITION;
                float3 uv: TEXCOORD0;
                fixed4 color: COLOR;
            };
            
            struct v2f
            {
                float3 uv: TEXCOORD0;
                fixed2 uv2: TEXCOORD1;
                float4 vertex: SV_POSITION;
                fixed4 color: COLOR;
            };
            
            Texture2D _MainTex;
            SamplerState sampler_MainTex;
            fixed4 _MainTex_ST;
            uniform float _IsEnemy;
            
            Texture2D _DissolveTex;
            fixed4 _DissolveTex_ST;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = fixed3(TRANSFORM_TEX(v.uv.xy, _MainTex), v.uv.z);
                o.uv2 = TRANSFORM_TEX(v.uv.xy, _DissolveTex);
                o.color = v.color;
                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                // sample the texture
                fixed4 col = _MainTex.Sample(sampler_MainTex, i.uv.xy);
                fixed dis = _DissolveTex.Sample(sampler_MainTex, i.uv2).r;
                
                col.a *= dis.r > i.uv.z;

                
                col.rgb *= lerp(i.color.rgb, float3(1,0.22,0.26) , _IsEnemy);
                col.a *= i.color.a;
                return col;
                
                return col ;
            }
            ENDCG
            
        }
    }
}

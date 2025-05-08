Shader "StencilTest"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
        _Color ("Color", Color) = (0, 0, 0, 1)
        
        [IntRange] _Ref ("Ref", Range(0, 255)) = 0
        [IntRange] _ReadMask ("ReadMask", Range(0, 255)) = 255
        [IntRange] _WriteMask ("WriteMask", Range(0, 255)) = 255
        [Enum(UnityEngine.Rendering.CompareFunction)] _Comp ("Comp", float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _Pass ("Pass", float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _Fail ("Fail", float) = 0
        [Enum(UnityEngine.Rendering.StencilOp)] _ZFail ("ZFail", float) = 0
        
        [Toggle] _ZWrite ("ZWrite", Range(0, 1)) = 1
        [Enum(UnityEngine.Rendering.CullMode)] _Cull ("Cull", float) = 2
        [Enum(UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", float) = 0
    }
    SubShader
    {
        Blend SrcAlpha OneMinusSrcAlpha
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
        Stencil
        {
            Ref [_Ref]
            ReadMask [_ReadMask]
            WriteMask [_WriteMask]
            Comp [_Comp]
            Pass [_Pass]
            Fail [_Fail]
            ZFail [_ZFail]
        }
        ZWrite [_ZWrite]
        ZTest [_ZTest]
        Cull [_Cull]
        
        Pass
        {
            CGPROGRAM
            
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            
            #include "UnityCG.cginc"
            
            struct appdata
            {
                float4 vertex: POSITION;
                float2 uv: TEXCOORD0;
            };
            
            struct v2f
            {
                float2 uv: TEXCOORD0;
                float4 vertex: SV_POSITION;
            };
            fixed4 _Color;
            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);

                return o;
            }
            
            fixed4 frag(v2f i): SV_Target
            {
                // sample the texture
                fixed4 col = _Color * tex2D(_MainTex, i.uv);

                return col;
            }
            ENDCG
            
        }
    }
}

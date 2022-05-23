Shader "Screen/OutlineStencilShader"
{
    Properties
    {
        _OutlineValue ("OutlineValue", Integer) = 0
    }
    SubShader
    {
        Tags {
            "RenderType" = "Opaque"
            "RenderedByReplacementCamera" = "True"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
            };

            int _OutlineValue;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return _OutlineValue;
            }
            ENDCG
        }        
    }
}

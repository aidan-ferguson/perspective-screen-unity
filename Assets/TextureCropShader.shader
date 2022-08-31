Shader "Custom/TextureCropShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _corrected_uvs[4]; 

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                // Very hacky, find a way to fix at some point
                if(v.uv.x == 0.0f && v.uv.y == 0.0f){
                    // Bottom left
                    o.uv = float2(_corrected_uvs[0].x, _corrected_uvs[0].y);
                } else if(v.uv.x == 0.0f && v.uv.y == 1.0f) {
                    // Top left
                    o.uv = float2(_corrected_uvs[1].x, _corrected_uvs[1].y);
                } else if(v.uv.x == 1.0f && v.uv.y == 0.0f) {
                    // Bottom right
                    o.uv = float2(_corrected_uvs[2].x, _corrected_uvs[2].y);
                } else if(v.uv.x == 1.0f && v.uv.y == 1.0f) {
                    // Top right
                    o.uv = float2(_corrected_uvs[3].x, _corrected_uvs[3].y);
                }
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                return col;
            }
            ENDCG
        }
    }
}

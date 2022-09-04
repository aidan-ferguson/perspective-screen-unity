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

            float4 _perspective_screen_corners[4]; 
            float4x4 _screen_distortion_matrix;

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
            
            v2f vert(appdata v)
            {
                v2f o;
                o.uv = v.uv;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float3x3 dist_mat = (float3x3)_screen_distortion_matrix;
                float3 normal_coordinate = float3(i.uv.x, i.uv.y, 1.0f);
                float3 distored_coordinate = mul(normal_coordinate, dist_mat);
                fixed4 col = tex2D(_MainTex, float2(distored_coordinate.x/distored_coordinate.z, distored_coordinate.y/distored_coordinate.z));
                return fixed4(col);
            }
            ENDCG
        }
    }
}

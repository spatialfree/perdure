// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/HolyGrail"
{
    Properties
    {
        [NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset] _Gradient ("Gradient", 2D) = "white" {}
    }
    SubShader
    {
        Fog { Mode Off }
        Pass
        {
            Tags { "LightMode" = "ForwardBase" } 

            CGPROGRAM
            // use "vert" function as the vertex shader
            #pragma vertex vert
            // use "frag" function as the pixel (fragment) shader
            #pragma fragment frag

            #include "UnityCG.cginc"

            // vertex shader inputs
            struct appdata
            {
                float4 vertex : POSITION;
                float4 normal : NORMAL;
            };

            // vertex shader outputs ("vertex to fragment")
            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };


            sampler2D _Gradient;

            // vertex shader
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = dot( v.normal, _WorldSpaceLightPos0.xyz );
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_Gradient, i.color.rg);
            }
            ENDCG
        }
    }
}
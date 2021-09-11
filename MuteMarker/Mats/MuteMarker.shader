Shader "MuteMarker/Ink" {
Properties {
    _MainTex ("Texture", 2D) = "white" {}
    _EmissionPower ("EmissionPower", Range(0,10)) = 0
}

Category {
    Tags { "Queue" = "AlphaTest" "IgnoreProjector" = "True" "RenderType" = "TransparentCutout" "PreviewType" = "Plane" }
    ColorMask RGB
    Cull Off Lighting Off

    SubShader
    {
        Pass {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_particles

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            fixed4 _TintColor;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 _MainTex_ST;
            float _EmissionPower;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
                return o;
            }

            UNITY_DECLARE_DEPTH_TEXTURE(_CameraDepthTexture);

            fixed4 frag (v2f i) : SV_Target
            { 
                half4 col = tex2D(_MainTex, i.texcoord);
                clip(col.r - 0.99999f);

                fixed4 finalCol = (col * i.color) + (i.color * _EmissionPower);
                return finalCol;
            }
            ENDCG
        }
    }
}
}

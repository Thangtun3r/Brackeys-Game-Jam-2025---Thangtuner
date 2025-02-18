Shader "Custom/2DGlowOutline"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (1,1,0,1)
        _GlowSize ("Glow Size", Range(0,0.1)) = 0.05
        _Threshold ("Alpha Threshold", Range(0,1)) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Pass 1: Render glow (outline) around the sprite.
        Pass
        {
            Name "Glow"
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragGlow
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GlowColor;
            float _GlowSize;
            float _Threshold;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 fragGlow(v2f i) : SV_Target
            {
                float glowFactor = 0.0;
                float2 uv = i.uv;
                int sampleCount = 8;

                // Sample around the pixel in a circle.
                for (int j = 0; j < sampleCount; j++)
                {
                    float angle = 6.2831853 * j / sampleCount;
                    float2 offset = float2(cos(angle), sin(angle)) * _GlowSize;
                    fixed4 sample = tex2D(_MainTex, uv + offset);
                    glowFactor += sample.a;
                }
                glowFactor /= sampleCount;

                // If the current pixel is transparent but its surroundings are opaque,
                // output the glow color with an intensity proportional to the neighbor alpha.
                fixed4 mainCol = tex2D(_MainTex, uv);
                if (mainCol.a < _Threshold && glowFactor > _Threshold)
                {
                    return fixed4(_GlowColor.rgb, glowFactor);
                }
                return fixed4(0, 0, 0, 0);
            }
            ENDCG
        }

        // Pass 2: Render the main sprite.
        Pass
        {
            Name "Sprite"
            Tags { "LightMode"="Always" }
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragSprite
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                return o;
            }

            fixed4 fragSprite(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }
    }
    FallBack "Sprites/Default"
}

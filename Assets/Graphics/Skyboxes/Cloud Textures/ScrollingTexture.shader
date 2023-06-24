Shader "Custom/ScrollingTexture" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _ScrollSpeed ("Scroll Speed", Range(0, 1)) = 0.5
        _ScrollDirection ("Scroll Direction", Range(0, 1)) = 0.0
    }
 
    SubShader {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200
 
        CGPROGRAM
        #pragma surface surf Lambert
 
        sampler2D _MainTex;
        float _ScrollSpeed;
        float _ScrollDirection;
 
        struct Input {
            float2 uv_MainTex;
        };
 
        void surf (Input IN, inout SurfaceOutput o) {
            fixed2 scrollOffset = fixed2(_ScrollDirection, 1 - _ScrollDirection) * _ScrollSpeed * _Time.y;
            fixed2 uv = IN.uv_MainTex + scrollOffset;
            fixed4 c = tex2D(_MainTex, uv);
            
            // Apply transparency if the color is black
            if (c.r == 0 && c.g == 0 && c.b == 0) {
                discard;
            }
            
            o.Albedo = c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}

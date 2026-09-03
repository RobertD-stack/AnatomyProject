Shader "getReal3D/ChaperoneShader"
{
    Properties
    {
        _MainTex("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,0,1,1)
        gridSize("GridSize", Float) = 0.1
        gridBarSize("GridBarSize", Float) = 0.02
    }
        SubShader
    {
        Tags {"Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent"}
        LOD 100

        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha
        Cull off

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
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float gridSize;
            float gridBarSize;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                /*
                float x = abs(fmod(i.uv.x, gridSize) / gridSize - 0.5) * 2;
                float y = abs(fmod(i.uv.y, gridSize) / gridSize - 0.5) * 2;

                return fixed4(x, 0, 0, 1);

                float f = max(tex2D(_MainTex, x).x, tex2D(_MainTex, y).x);

                return fixed4(f * mainColor.xyz, f);
                */

                float x = 1 - abs(fmod(i.uv.x + gridSize/2, gridSize) / gridSize - 0.5) / gridBarSize * gridSize;
                float y = 1 - abs(fmod(i.uv.y + gridSize / 2, gridSize) / gridSize - 0.5) / gridBarSize * gridSize;

                //float f = tex2D(_MainTex, x).x;
                float f = max(tex2D(_MainTex, x).x, tex2D(_MainTex, y).x);
                //return fixed4(f, 0, 0, 1);


                //float f = max(tex2D(_MainTex, x).x, tex2D(_MainTex, y).x);
                //
                return f * _Color;
        }
        ENDCG
    }
    }
}

Shader "Unlit/Window"
{
    Properties
    {
        _Surface("__surface", Float) = 0.0
        _MainTex ("Texture", 2D) = "white" {}
        _Size ("Size", float) = 1
        _T("Time", float) = 1
        _Disortion("Disortion", range(-5, 5)) = 0
        _Blur("Blur", range(0, 1)) = 0
        _Clean("Clean", float) = 1
        _Rain("Rain", float) = 0

        _Xspeed("Xspeed", float) = 0
        _Yspeed("Yspeed", float) = 0

        _ChunkSizeX("ChunkSizeX", float) = 1
        _ChunkSizeY("ChunkSizeY", float) = 1

        _CleanerCos("CleanerCos", float) = 0
        _TimeSin("SinTime", float) = 0

        _Mark("Mark", 2D) = "white" {}
        _MarkScale("MarkScale", float) = 0
        _MarkMult("MarkMult", float) = 0
        _MarkRise("MarkRise", float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Transparent"}
        LOD 100

        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog
            #define S(a,b,t) smoothstep(a,b,t)
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float4 uv : TEXCOORD0;
                float4 mark : TEXCOORD2;
            };

            struct v2f
            {
                float4 uv : TEXCOORD0;
                float2 uvv : TEXCOORD1;
                float2 mark : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex, _GrabTexture, _Mark;
            float4 _MainTex_ST, _Mark_ST;
            float _Size;
            float _T, _Disortion, _Blur, _Xspeed, _Yspeed, _Clean, _Rain, _ChunkSizeX, _ChunkSizeY, _MarkScale, _CleanerCos, _TimeSin, _MarkRise;
            float _MarkMult, _OldA;

            v2f vert (appdata v)
            {
                v2f o;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uvv = TRANSFORM_TEX(v.uv.xy, _MainTex);
                o.mark = TRANSFORM_TEX(v.mark.xy, _Mark);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                o.vertex = UnityObjectToClipPos(v.vertex.xyz);
                o.uv = ComputeScreenPos(o.vertex);
                return o;
            }

            float N21(float2 p)
            {
                p = frac(p * float2(169.12, 256.10));
                p += dot(p, p + 31.272);
                return frac(p.x * p.y);
            }

            float3 Layer(float2 UV, float t)
            {
                float2 aspect = float2(2,2);
                float2 uv = UV*8*aspect;
                uv.y += t *.25;
                float2 gv = frac(uv + float2(_Xspeed * t, _Yspeed * t))-.5;
                float2 id = floor(uv);

                float n = N21(id);
                t += n * 6.2831;

                float w = UV.y * 10; 
                float x = (n - .5) * .8;
                x += ((.4 - abs(x)) * sin(3 * w) * pow(sin(w), 6)) * .45;

                float y = -sin(t+sin(t+sin(t)*.5)) * .45;
                y -= (gv.x-x) * (gv.x-x);

                float2 dropPos =  (gv - float2(x, y))/aspect;
                float drop = S(.05, .03, length(dropPos));

                float2 trailPos =  (gv - float2(x, t * .25))/aspect;
                trailPos.y = (frac(trailPos.y * 8)-.5) / 8;
                float trail = S(.03, .01, length(trailPos));
                float fogTrail = S(-.05, .05, dropPos.y);
                fogTrail *= S(.5, y, gv.y);
                trail *= fogTrail;
                fogTrail *= S(.05, .04, abs(dropPos.x));

                float2 offset = drop * dropPos + trail * trailPos;
                return float3(offset, fogTrail);
            }

            float3 Layer2(float2 UV, float t)
            {
                float2 aspect = float2(2,2);
                float2 uv = UV*8*aspect;
                float2 gv = frac(uv + float2(_Xspeed * t, _Yspeed * t))-.5;
                float2 id = floor(uv);

                float n = N21(id);
                t += n * 6.2831;

                float alpha = 1 - fmod(t, 1);

                float xn = N21(float2(n, t - frac(t)));
                float yn = N21(float2(n, 1.28 + t - frac(t)));
                float x = (xn - .5) * .8;
                float y = (yn - .5) * .8;

                float2 dropPos =  (gv - float2(x, y))/aspect;
                float drop = S(.05 * alpha, .03 * alpha, length(dropPos));

                float2 offset = drop * dropPos * alpha;
                return float3(offset, 0);
            }

            float2 CleanLayer(float2 UV, float t)
            {
                float2 uv = UV;
                float r = uv.x - 0.5;
                float a0 = _CleanerCos;
                float a1 = r / sqrt(r * r * 1.5 + uv.y * uv.y * .5);
                float d = abs((_TimeSin >= 0 ? 1:-1) - a1) / 2;
                float dif = (_TimeSin > 0) ^ (a0 - a1) < 0 ? min(d + abs(_TimeSin / 2), 1) : abs(a0 - a1) / 2;
                float cleanAmount = (uv.y * uv.y * .5 + r*r * 1.5) > .25 ? 1: dif;
                return float2(cleanAmount, 0);
            }
            float2 CleanMark(float2 UV, float t)
            {
                float2 uv = UV;
                float r = uv.x - 0.5;
                float a0 = _CleanerCos;
                float a1 = r / sqrt(r * r * 1.5 + uv.y * uv.y * .5);
                float d = abs((_TimeSin >= 0 ? 1:-1) - a1) / 2;
                float dif = (_TimeSin > 0) ^ (a0 - a1) < 0 ? _MarkMult : max(_MarkMult - 0.1, 0);
                float cleanAmount = (uv.y * uv.y * .5 + r*r * 1.5) > .25 ? 1: dif;
                float silecek = (a1 > a0 - 0.001) & (a1 < a0 + 0.001) & (uv.y * uv.y * .5 + r*r * 1.5) < .25 ? 1:0; 
                return float2(cleanAmount, silecek);
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float t = fmod(_Time.y +_T, 6000);
                float4 col = 0;
                float2 _uv = i.uv.xy / i.uv.w;
                float2 pixel = floor(i.uvv * 500) / 500;
                float2 pixelMark = floor(i.mark * 500) / 500;
                float3 drops = float3(0,0,0);
                for(int i = 0; i < _Rain; i += 1)
                {
                    //drops += Layer2(pixel * (N21(i + 0.1) + 1) + N21(i * i - 0.14), t) * min(_Rain - i, 1);
                    if(i % 2 == 0)
                    {
                        drops += Layer(pixel * (N21(i + 0.2) + 1) + N21(i * i - 0.2), t) * min(_Rain - i, 1);
                    }
                    
                }
                float2 clean = CleanLayer(pixel, _Clean > 0 ? 0: t);
                //clean.x *= _Clean;
                //clean.y *= _Clean;

                float2 cleanM = CleanMark(pixel,  _Clean > 0 ? 0: t);
                //cleanM.x *= _Clean;
                //cleanM.y *= _Clean;
                drops *= _Clean <= 0 ? 1 : clean.x;

                
                float4 mark = tex2Dlod(_Mark, float4(pixelMark, 0, 0)) * _MarkScale * cleanM.x;
                

                float blur = _Blur * 7 * (1 - drops.z);
                col += tex2Dlod(_MainTex, float4( _uv + drops.xy * -4,0,blur));

                col += pixel.y > _MarkRise ? 0 : mark * _MarkRise;
                
                col = clean.y > 0 ? float4(0,0,0,1):col;

                return col;
            }
            ENDCG
        }
    }
}

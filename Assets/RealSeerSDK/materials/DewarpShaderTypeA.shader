Shader "Custom/DewarpShaderTypeA" { // defines the name of the shader 
	Properties {
		_MainTex("_MainTex", 2D) = "" {}
		screenInfo("screenInfo", Vector) = (0,0,0,0)
		eyeBox("eyeBox", Vector) = (0,0,0,0)
	}

	//the CG subshader
	SubShader {
		//only pass
		Pass {
			ZTest Always
			Cull Off
			ZWrite Off
			Fog { Mode off }
			
			CGPROGRAM

			//tell the compiler about our vert and frag functions
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 screenInfo;
			fixed4 eyeBox;

			struct vertfrag {
				float4 position : POSITION;
				float4 worldpos : TEXCOORD1;
			};

			//passthrough vert shader (why even bother multiplying by mvp?)
			vertfrag vert(appdata_base v) {
				vertfrag o;
				o.position = mul(UNITY_MATRIX_MVP, v.vertex);
				o.worldpos = o.position; //because glsl is better
				return o;
			}

			float2 map00(float x, float x2, float x3, float x4, float y, float y2, float y3, float y4) { return float2(x, y); } //bottom left
			float2 map10(float x, float x2, float x3, float x4, float y, float y2, float y3, float y4) {
				//these are the old coefficients!
				return float2(0.022162 +
					x	  *  1.165375 +
					y	  *  0.052539 +
					x2	  * -0.034822 +
					x*y	  *  0.283082 +
					y2	  *  0.040379 +
					x3	  * -0.054210 +
					x2*y  *  0.174904 +
					x*y2  *  0.157498 +
					y3	  * -0.012528 +
					x4	  *  0.215194 +
					x3*y  *  0.308627 +
					x2*y2 * -0.049195 +
					x*y3  *  0.123411 +
					y4	  * -0.032656
					,		-0.141015 +
					x	  * -0.221409 +
					y	  *  1.088636 +
					x2	  * -1.012132 +
					x*y	  *  0.018144 +
					y2	  *  0.423666 +
					x3	  * -0.001848 +
					x2*y  * -1.467441 +
					x*y2  * -0.052869 +
					y3	  * -0.137657 +
					x4	  *  0.492221 +
					x3*y  * -0.933112 +
					x2*y2 * -0.287140 +
					x*y3  * -0.234555 +
					y4	  * -0.060633
				);
			} //top left
			float2 map01(float x, float x2, float x3, float x4, float y, float y2, float y3, float y4) {
				return float2(
							-0.005732 +
					x	  * 1.233685 +
					y	  * 0.039561 +
					x2	  * -0.000649 +
					x*y	  * -0.073908 +
					y2	  * 0.008677 +
					x3	  * -0.128713 +
					x2*y  * 0.019731 +
					x*y2  * -0.051050 +
					y3	  * 0.001961 +
					x4	  * 0.081437 +
					x3*y  * 0.255228 +
					x2*y2 * 0.021673 +
					x*y3  * 0.148230 +
					y4	  * 0.000520,

							0.186200 +
					x	  * -0.077624 +
					y	  * 1.161327 +
					x2	  * -0.922708 +
					x*y	  * -0.073886 +
					y2	  * -0.073107 +
					x3	  *  0.040573 +
					x2*y  * -0.393019 +
					x*y2  * -0.005763 +
					y3	  * -0.063284 +
					x4	  * 0.181403 +
					x3*y  * 0.000397 +
					x2*y2 * 0.092176 +
					x*y3  * 0.000769 +
					y4	  * -0.006216
				);
			} //bottom right
			float2 map11(float x, float x2, float x3, float x4, float y, float y2, float y3, float y4) { return float2(x, y); } //top right

			float2 getDistortion(float2 smp) {
				//mirror eye
				smp *= float2(eyeBox.w, 1.0);
				float x = smp.x, y = smp.y, x2 = x*x,
					  y2 = y*y, x3 = x2*x, y3 = y2*y, x4 = x3*x, y4 = y3*y;
				float2	d1 = smp - map00(x, x2, x3, x4, y, y2, y3, y4),
						d2 = smp - map01(x, x2, x3, x4, y, y2, y3, y4),
						d3 = smp - map10(x, x2, x3, x4, y, y2, y3, y4),
						d4 = smp - map11(x, x2, x3, x4, y, y2, y3, y4),
					//create uniform mix
					accum = lerp(lerp(d1, d2, eyeBox.x), lerp(d3, d4, eyeBox.x), eyeBox.y);
				//add distortion and unmirror eye
				return (smp - accum) * float2(eyeBox.w, 1.0);
			}

			//if unity complains about SV_POSITION just ignore it
			fixed4 frag(vertfrag inp) : SV_Target {
				float2 smp = inp.worldpos.xy;

				//new displacement with eyebox support
				float2 disp = smp + (smp - getDistortion(smp));
			
				//if distored outside defined area return black
				if (disp.x < -1.0 || disp.x > 1.0 || disp.y < -1.0 || disp.y > 1.0) return fixed4(0., 0., 0., 1.);

				//the one texture sample
				return tex2D(_MainTex, disp * float2(-0.5, 0.5) + 0.5);
			}

			ENDCG
		}
	}

	SubShader { // Unity chooses the subshader that fits the GPU best
		Pass { // some shaders require multiple passes
			GLSLPROGRAM // here begins the part in Unity's GLSL

#ifdef VERTEX // here begins the vertex shader

			uniform sampler2D _MainTex;
			uniform vec4 screenInfo;
			uniform vec4 eyeBox;
	
			void main() {
				gl_Position = gl_ModelViewProjectionMatrix * gl_Vertex;
			}

#endif // here ends the definition of the vertex shader
#ifdef FRAGMENT // here begins the fragment shader

			uniform sampler2D _MainTex;
			uniform vec4 screenInfo;
			uniform vec4 eyeBox;

			vec2 map(float x, float x2, float x3, float x4, float y, float y2, float y3, float y4) {
				return vec2(
					/*x*/	-0.005732 +
					x	  * 1.233685 +
					y	  * 0.039561 +
					x2	  * -0.000649 +
					x*y	  * -0.073908 +
					y2	  * 0.008677 +
					x3	  * -0.128713 +
					x2*y  * 0.019731 +
					x*y2  * -0.051050 +
					y3	  * 0.001961 +
					x4	  * 0.081437 +
					x3*y  * 0.255228 +
					x2*y2 * 0.021673 +
					x*y3  * 0.148230 +
					y4	  * 0.000520,

					/*y*/	0.186200 +
					x	  * -0.077624 +
					y	  * 1.161327 +
					x2	  * -0.922708 +
					x*y	  * -0.073886 +
					y2	  * -0.073107 +
					x3	  *  0.040573 +
					x2*y  * -0.393019 +
					x*y2  * -0.005763 +
					y3	  * -0.063284 +
					x4	  * 0.181403 +
					x3*y  * 0.000397 +
					x2*y2 * 0.092176 +
					x*y3  * 0.000769 +
					y4	  * -0.006216
				);
			}

			vec2 getDistortion(vec2 smp) {
				//mirror eye
				smp *= vec2(-eyeBox.w, 1.0);
				float x = smp.x, y = smp.y, x2 = x*x,
					y2 = y*y, x3 = x2*x, y3 = y2*y, x4 = x3*x, y4 = y3*y;
				vec2 d1 = smp - map(x, x2, x3, x4, y, y2, y3, y4);
				//add distortion and unmirror eye
				return (smp - d1) * vec2(-eyeBox.w, 1.0);
			}

			void main() {
				vec2 smp = (gl_FragCoord.xy * screenInfo.zw - vec2(eyeBox.w * -0.5 + 0.5, 0.)) * 2.0 - 1.0;

				//new displacement with eyebox support
				vec2 disp = smp + (smp - getDistortion(smp));
				
				//if distored outside defined area return black
				if (disp.x < -1.0 || disp.x > 1.0 || disp.y < -1.0 || disp.y > 1.0) {
					gl_FragColor = vec4(0., 0., 0., 1.);
					return;
				}

				//the one texture sample
				gl_FragColor = texture2D(_MainTex, disp * vec2(-0.5, 0.5) + 0.5);
			}
#endif // here ends the definition of the fragment shader

			ENDGLSL // here ends the part in GLSL
		}
	}
}
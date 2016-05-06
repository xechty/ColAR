Shader "Custom/Set_Matrix_To_Material" {

    Properties {

        _MainTex("Texture", 2D) = "white" {}
        _BackTex("Texture", 2D) = "white" {}
		_Alpha ("AlphaFront", Range(0,1)) = 1
		_AlphaBack ("AlphaBack", Range(0,1)) = 0
    }

    SubShader{  
	Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

			Pass{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			fixed _AlphaBack;

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _BackTex;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 col = tex2D(_BackTex, i.uv);
				col.a = _AlphaBack;
				return col;
			}
			ENDCG
		}


    Pass{

    Cull Back
	ZWrite Off
	Blend SrcAlpha OneMinusSrcAlpha
	
    CGPROGRAM

    #pragma vertex vert
    #pragma fragment frag
    #include "UnityCG.cginc"

    sampler2D _MainTex;
    float4x4 _MATRIX_MVP;
    
    float _KX;
    float _KY;
    
   int _KR = 1;
   int _KG = 1;
      
	fixed _Alpha;
	
    struct v2f{

        float4  pos : SV_POSITION;
        float2  uv : TEXCOORD0;

    };

    v2f vert(appdata_base v){
       
        v2f o;
        float2 screenSpacePos;
        float4 clipPos;

        //Convert position from world space to clip space.
        //Only the UV coordinates should be frozen, so use a different matrix

        clipPos = mul(_MATRIX_MVP, v.vertex);

        
        //Convert position from clip space to screen space.
        //Screen space has range x=-1 to x=1

        screenSpacePos.x = clipPos.x / clipPos.w;
        screenSpacePos.y = clipPos.y / clipPos.w;

        //the screen space range (-1 to 1) has to be converted to
        //the UV range 0 to 1 

        o.uv.x = (_KX*screenSpacePos.x) + _KX;
        o.uv.y = (_KY*screenSpacePos.y) + _KY;
        
        //The position of the vertex should not be frozen, so use 
        //the standard UNITY_MATRIX_MVP matrix
        o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
        return o;
    }

    half4 frag(v2f i) : COLOR{
        fixed4 col = tex2D(_MainTex, i.uv);     

        if (i.uv.x < 0 || i.uv.x > _KX*2 || i.uv.y < 0 || i.uv.y > _KY*2) {col.rgb = 1; col.a = 0;}
        else col.a = _Alpha;

        col.r *= _KR + 1;
        col.g *= _KG + 1;
        
        return col;
    }

    ENDCG

    	}
    }
}
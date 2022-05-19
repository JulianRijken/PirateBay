// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "PirateWater"
{
	Properties
	{
		_Smoothness("Smoothness", Float) = 0
		_Metallic("Metallic", Float) = 0
		_WaterColor("WaterColor", Color) = (0,0,0,0)
		_RedWaterColor("RedWaterColor", Color) = (0,0,0,0)
		_Blend("Blend", Vector) = (0,0,0,0)
		_WaterClarity("WaterClarity", Float) = 0
		_Power("Power", Float) = 0
		_RedSeaFadeLength("RedSeaFadeLength", Float) = 0
		_RedSeaFadeOffset("RedSeaFadeOffset", Float) = 0
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Standard alpha:fade keepalpha noshadow exclude_path:deferred vertex:vertexDataFunc 
		struct Input
		{
			float eyeDepth;
			float3 worldPos;
			float4 screenPos;
		};

		uniform float4 _WaterColor;
		uniform float4 _RedWaterColor;
		uniform float _RedSeaFadeLength;
		uniform float _RedSeaFadeOffset;
		uniform float2 _Blend;
		uniform float _Metallic;
		uniform float _Smoothness;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _WaterClarity;
		uniform float _Power;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			o.eyeDepth = -UnityObjectToViewPos( v.vertex.xyz ).z;
		}

		void surf( Input i , inout SurfaceOutputStandard o )
		{
			float cameraDepthFade96 = (( i.eyeDepth -_ProjectionParams.y - _RedSeaFadeOffset ) / _RedSeaFadeLength);
			float4 lerpResult85 = lerp( _WaterColor , _RedWaterColor , saturate( ( 1.0 - cameraDepthFade96 ) ));
			float3 ase_worldPos = i.worldPos;
			float2 appendResult16 = (float2(ase_worldPos.x , ase_worldPos.z));
			float4 lerpResult24 = lerp( _WaterColor , lerpResult85 , saturate( (0.0 + (length( ( appendResult16 - float2( 0,0 ) ) ) - _Blend.x) * (1.0 - 0.0) / (_Blend.y - _Blend.x)) ));
			o.Albedo = lerpResult24.rgb;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth77 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm.xy ));
			float distanceDepth77 = abs( ( screenDepth77 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _WaterClarity ) );
			o.Alpha = pow( saturate( distanceDepth77 ) , _Power );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=18935
-1901;28;1920;921;1887.797;789.2112;1.3;True;False
Node;AmplifyShaderEditor.WorldPosInputsNode;14;-1570.084,-600.9633;Inherit;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.DynamicAppendNode;16;-1345.638,-564.3017;Inherit;False;FLOAT2;4;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.RangedFloatNode;98;-892.7575,-10.70022;Inherit;False;Property;_RedSeaFadeOffset;RedSeaFadeOffset;8;0;Create;True;0;0;0;False;0;False;0;65.4;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-895.5577,-108.1003;Inherit;False;Property;_RedSeaFadeLength;RedSeaFadeLength;7;0;Create;True;0;0;0;False;0;False;0;255.31;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;17;-1164.259,-541.408;Inherit;False;2;0;FLOAT2;0,0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.CameraDepthFade;96;-656.4632,-86.53059;Inherit;False;3;2;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Vector2Node;23;-796.5094,-416.1162;Inherit;False;Property;_Blend;Blend;4;0;Create;True;0;0;0;False;0;False;0,0;400,409.7;0;3;FLOAT2;0;FLOAT;1;FLOAT;2
Node;AmplifyShaderEditor.OneMinusNode;99;-400.5962,-92.41114;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LengthOpNode;15;-994.7385,-536.9017;Inherit;True;1;0;FLOAT2;0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;76;-662.1768,305.7355;Inherit;False;Property;_WaterClarity;WaterClarity;5;0;Create;True;0;0;0;False;0;False;0;25;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;5;-365.5123,-892.4332;Inherit;False;Property;_WaterColor;WaterColor;2;0;Create;True;0;0;0;False;0;False;0,0,0,0;0.5801887,0.814182,1,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.DepthFade;77;-451.0566,287.8126;Inherit;False;True;False;True;2;1;FLOAT3;0,0,0;False;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TFHCRemapNode;20;-595.9508,-471.7467;Inherit;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;0;False;4;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;25;-363.4188,-695.8231;Inherit;False;Property;_RedWaterColor;RedWaterColor;3;0;Create;True;0;0;0;False;0;False;0,0,0,0;1,0,0,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SaturateNode;87;-244.7391,-127.0559;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;78;-171.957,443.2701;Inherit;False;Property;_Power;Power;6;0;Create;True;0;0;0;False;0;False;0;0.5;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;21;-376.9507,-441.2467;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;79;-165.9543,294.8802;Inherit;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;85;49.80298,-179.2097;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;24;140.6418,-442.0247;Inherit;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;3;100.2887,134.1862;Inherit;False;Property;_Smoothness;Smoothness;0;0;Create;True;0;0;0;False;0;False;0;0.36;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;130.2887,54.18628;Inherit;False;Property;_Metallic;Metallic;1;0;Create;True;0;0;0;False;0;False;0;1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;80;19.60345,292.9459;Inherit;True;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;357,-6;Float;False;True;-1;2;ASEMaterialInspector;0;0;Standard;PirateWater;False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Transparent;0.5;True;False;0;False;Transparent;;Transparent;ForwardOnly;18;all;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;2;5;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;True;Relative;0;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;False;0.1;False;-1;0;False;-1;False;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;16;0;14;1
WireConnection;16;1;14;3
WireConnection;17;0;16;0
WireConnection;96;0;97;0
WireConnection;96;1;98;0
WireConnection;99;0;96;0
WireConnection;15;0;17;0
WireConnection;77;0;76;0
WireConnection;20;0;15;0
WireConnection;20;1;23;1
WireConnection;20;2;23;2
WireConnection;87;0;99;0
WireConnection;21;0;20;0
WireConnection;79;0;77;0
WireConnection;85;0;5;0
WireConnection;85;1;25;0
WireConnection;85;2;87;0
WireConnection;24;0;5;0
WireConnection;24;1;85;0
WireConnection;24;2;21;0
WireConnection;80;0;79;0
WireConnection;80;1;78;0
WireConnection;0;0;24;0
WireConnection;0;3;4;0
WireConnection;0;4;3;0
WireConnection;0;9;80;0
ASEEND*/
//CHKSM=7FB7A9BB73736BCDE37AD03DD80E11ABC5DA46DE
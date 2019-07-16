#include "StandardBase.fx"
#include "../Common/ShadowMap.fxh"

// Variables
float3 DiffuseColor;
float3 Features;
float2 TextureTiling;
float TotalTime;
float Alpha;

// Textures
DECLARE_TEXTURE(AlbedoMap, 1);
DECLARE_TEXTURE(NormalMap, 2);
DECLARE_TEXTURE(SpecularMap, 3);
DECLARE_TEXTURE(ReflectionMap, 4);

VertexShaderOutput VertexShaderWaterFunction(VertexShaderInput input)
{
	input.Position.z += sin((TotalTime * 16.0) + (input.Position.y / 1.0)) / 16.0;
	input.Position.y += sin(1.0 * input.Position.y + (TotalTime * 5.0)) * 0.25;

	return VertexShaderFunction(input);
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// Custom UV
	float2 uv = input.UV;
	uv.x = uv.x * 20.0 + sin(TotalTime * 3.0 + 10.0) / 256.0;
	uv.y = uv.y * 20.0;

	float2 scaledUV = uv * TextureTiling;

	// Albedo
	float3 albedo = SAMPLE_TEXTURE(AlbedoMap, scaledUV).xyz;

	// Normal
	float3 normal = input.WorldNormal;
	if (Features.x > 0)
	{
		normal = (2.0 * (SAMPLE_TEXTURE(NormalMap, scaledUV).xyz)) - 1.0;
		normal = normalize(mul(normal, input.WorldToTangentSpace));
	}

	// Specular
	float specularTerm = 0.5;
	if (Features.y > 0)
		specularTerm = SAMPLE_TEXTURE(SpecularMap, scaledUV).r;

	// Shadows
	float shadowTerm = CalcShadow(input.WorldPosition);

	// Reflection
	float4 reflection = float4(0, 0, 0, 0);

	if (Features.z > 0)
	{
		float2 projectedUV = float2(input.Reflection.x / input.Reflection.w / 2.0 + 0.5, -input.Reflection.y / input.Reflection.w / 2.0 + 0.5);
		float3 reflectionColor = SAMPLE_TEXTURE(ReflectionMap, projectedUV).xyz;
		reflection = float4(reflectionColor, Features.z);
	}

	// Base Pixel Shader
	return float4(StandardPixelShader(input.WorldPosition, normal, specularTerm, input.FogDistance, albedo.rgb * DiffuseColor, FLOAT3(0), shadowTerm, reflection), Alpha);
}

technique Water
{
	pass WaterPass
	{
		AlphaBlendEnable = TRUE;
		DestBlend = INVSRCALPHA;
		SrcBlend = SRCALPHA;
#if SM4
		VertexShader = compile vs_4_0 VertexShaderWaterFunction();
		PixelShader = compile ps_4_0 PixelShaderFunction();
#else
		VertexShader = compile vs_3_0 VertexShaderWaterFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
#endif
	}
}

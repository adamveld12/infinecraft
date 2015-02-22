float4x4 View;
float4x4 Projection;

float3 LightDirection = normalize(float3(0, 30, 15));
float DiffuseLight = .8;
float AmbientLight = 0.45;

texture Texture;

sampler Sampler = sampler_state
{
	Texture = (Texture);
	MinFilter = POINT;
    MagFilter = POINT;
    MipFilter = POINT;
    AddressU = Wrap;
    AddressV = Wrap;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float4 Normal : NORMAL0;
	float2 TextureCoordinate : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION;
	float3 Normal : TEXCOORD1;
	float2 TextureCoordinate : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output = (VertexShaderOutput)0;
	output.Position = mul(mul(input.Position, View),Projection);
	output.Normal = normalize(input.Normal);
	output.TextureCoordinate = input.TextureCoordinate;
	output.Normal = input.Normal;
    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float diffuseLightingFactor = dot(LightDirection, input.Normal);
	diffuseLightingFactor = saturate(diffuseLightingFactor);
	diffuseLightingFactor *= DiffuseLight;
	
 	return float4((tex2D(Sampler, input.TextureCoordinate) * (diffuseLightingFactor + AmbientLight)).rgb, 1);
}

technique StaticVertexBufferRendering
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}

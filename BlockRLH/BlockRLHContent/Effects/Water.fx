float4x4 View;
float4x4 Projection;
float4x4 World;
float waveLength;
float waveHeight;
float time;
Texture waterBumpMap;

sampler WaterBumpMapSampler = sampler_state { texture = <waterBumpMap> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

Texture texture1;

sampler TextureSampler = sampler_state { texture = <texture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = MIRROR; AddressV = MIRROR;};


//------- Technique: Water --------

struct WVertexToPixel
{
    float4 Position                 : POSITION;
	float2 TextureSamplingPos		: TEXCOORD0;
    float2 BumpMapSamplingPos        : TEXCOORD1;
 };
 
 struct WPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 WVertexToPixel WaterVS(float4 inPos : POSITION, float2 inTex: TEXCOORD)
 {    
     WVertexToPixel Output = (WVertexToPixel)0;

	 float4x4 preViewProjection = mul (View, Projection);
     float4x4 preWorldViewProjection = mul (World, preViewProjection);

	 Output.Position = mul(inPos, preWorldViewProjection);
	 Output.TextureSamplingPos = inTex;
     Output.BumpMapSamplingPos = inTex/waveLength;
 
     return Output;
 }
 
 WPixelToFrame WaterPS(WVertexToPixel PSIn)
 {
     WPixelToFrame Output = (WPixelToFrame)0;         

     float4 bumpColor = tex2D(WaterBumpMapSampler, mul(PSIn.BumpMapSamplingPos,time/1000));
     float2 perturbation = waveHeight*(bumpColor.rg - 0.5f)*2.0f;
     float2 perturbatedTexCoords = PSIn.TextureSamplingPos + perturbation;
 
     Output.Color = tex2D(TextureSampler, perturbatedTexCoords);    
	 Output.Color.a = .2;
     
     return Output;
 }
 
 technique Water
 {
     pass Pass0
     {
         VertexShader = compile vs_1_1 WaterVS();
         PixelShader = compile ps_2_0 WaterPS();
     }
 }
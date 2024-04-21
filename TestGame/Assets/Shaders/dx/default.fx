cbuffer data :register(b0)
{
    float4x4 mvp;
};

Texture2D myTexture : register(t0); // Declare the texture
SamplerState mySampler : register(s0); // Declare the sampler

struct VS_IN
{
    float4 pos : POSITION;
    float2 texcoord : TEXCOORD;
};

struct PS_IN
{
    float4 pos : SV_POSITION;
    float2 texcoord : TEXCOORD;
};

PS_IN VS(VS_IN input)
{
    PS_IN output = (PS_IN)0;
    
    output.pos = mul(mvp, input.pos);
    output.texcoord = input.texcoord;
    
    return output;
}

float4 PS(PS_IN input) : SV_Target
{
    // Sample the texture using the texture coordinates and the sampler
    float4 color = myTexture.Sample(mySampler, input.texcoord);
    return color;
}

technique10 Render
{
    pass P0
    {
        SetGeometryShader(0);
        SetVertexShader(CompileShader(vs_4_0, VS()));
        SetPixelShader(CompileShader(ps_4_0, PS()));
    }
}

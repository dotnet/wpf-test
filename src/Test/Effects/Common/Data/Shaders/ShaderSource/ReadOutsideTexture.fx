//Samplinhg outside the 0-1 range. 

SamplerState  ImageSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS( float2 uv  : TEXCOORD0) : SV_Target
{
    return tex2D(ImageSampler, float2(uv.x+0.5, uv.y));
}

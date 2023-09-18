//average got the color for current point to be average of 10 points toward the an EndPoint. 

// Object Declarations
float2 endPoint : register(C0);

SamplerState  ImageSampler : register(S0);

//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
    float4 Diffuse  : COLOR0;
    float2 UV0      : TEXCOORD0;
    float2 UV1      : TEXCOORD1;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION;
    float4 Color     : COLOR0;
    float2 UV        : TEXCOORD0;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.UV = input.UV0;   
    return output;
}

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS( VS_OUTPUT input ) : SV_Target
{
   float4 result = (tex2D( ImageSampler, input.UV)
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.01))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.02))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.03))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.04))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.05))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.06))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.07))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.08))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.09))
           + tex2D( ImageSampler, lerp(input.UV, endPoint, 0.1)))/10.0;

   return result;
}



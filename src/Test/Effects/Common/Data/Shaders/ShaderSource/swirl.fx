// Swirl shader -- Greg Schechter, February 2008

// Object Declarations
float2 center : register(C0);
float spiralStrength : register(C1);
float angleFrequency : register(C2);

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
   float2 dir = input.UV - center;
   float l = length(dir);
   float angle = atan2(dir.y, dir.x);
   
   float newAng = angle + spiralStrength * l;
   float newAng2 = angleFrequency * newAng;
   float sinVal, cosVal;
   sincos(newAng2, sinVal, cosVal);
   float xAmt = cosVal * l;
   float yAmt = sinVal * l;
   
   float2 newCoord = center + float2(xAmt, yAmt);
   float2 saturatedNewCoord = saturate(newCoord);
   
   float4 result;
   if (distance(saturatedNewCoord, newCoord) != 0)
   {
       result = float4(0,0,0,0);
   }
   else
   {
       result = tex2D( ImageSampler, newCoord );
   }
   
   return result;
}



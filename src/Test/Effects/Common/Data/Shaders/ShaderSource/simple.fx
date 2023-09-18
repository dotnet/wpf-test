float4x4 DeviceTransform : register(c0);


//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float4 Position : POSITION;
};

struct VS_OUTPUT
{
    float4 Position  : POSITION;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.Position = mul(input.Position, DeviceTransform);    
    return output;
}

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS( VS_OUTPUT input ) : COLOR0
{
   return float4(1, 0.5, 0, 0);
}



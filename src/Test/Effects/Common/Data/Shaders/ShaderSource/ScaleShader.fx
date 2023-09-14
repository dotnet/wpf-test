
// Object Declarations
float scale : register(C0);
float2 center : register(C1);
sampler2D g_samSrcColor : register(s0);

//--------------------------------------------------------------------------------------
struct VS_INPUT
{
    float2 UV      : TEXCOORD0;
};

struct VS_OUTPUT
{
    float2 UV        : TEXCOORD0;
};

//--------------------------------------------------------------------------------------
// Vertex Shader
//--------------------------------------------------------------------------------------
VS_OUTPUT VS(VS_INPUT input)
{
    VS_OUTPUT output = (VS_OUTPUT)0;
    output.UV = input.UV;   
    return output;
}

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS( VS_OUTPUT input ) : COLOR0
{
   float2 texuv = input.UV;
   
   float2 ray = texuv - center;
   float2 newTextuv =  ray / scale  + center;

   if(newTextuv.x < 0 || newTextuv.y < 0 
       || newTextuv.x > 1 || newTextuv.y > 1)
       return float4(0,0,0,0);
   
   return tex2D(g_samSrcColor, newTextuv);
}



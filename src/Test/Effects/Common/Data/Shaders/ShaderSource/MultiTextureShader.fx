// Shader take texture from all sampler register and draw them in different rows. 

// Object Declarations
SamplerState ImageSample0 : register(S0);
SamplerState ImageSample1 : register(S1);
SamplerState ImageSample2 : register(S2);
SamplerState ImageSample3 : register(S3);


float4 shaderCount : register(C0);
            

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

float4 PS( VS_OUTPUT input ) : COLOR0
{
   float2 texuv = input.UV;
   
   int rowIndex = floor(4.0 * texuv.g);
   float4 color = float4(0, 0, 0, 1);
   //First row uses texture from S0
   if(rowIndex  == 0)
   {
      color = tex2D(ImageSample0, texuv); 
   }
   else if(rowIndex == 1)
   {       
      color = tex2D(ImageSample1, texuv); 
   }
   else if(rowIndex == 2)
   {       
      color = tex2D(ImageSample2, texuv); 
   }
   else if(rowIndex == 3)
   {       
      color = tex2D(ImageSample3, texuv); 
   }
    
   return color;
}



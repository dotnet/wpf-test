
// This shader take texture from S0 adn S1. The rendering is splitted into 3 rows
// The first row uses texture from S0, third row uses texutre from S1, and the second
// is mixed of the two. 
SamplerState ImageSample0 : register(S0);
SamplerState ImageSample1 : register(S1);
            

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
   
   int rowIndex = floor(3.0 * texuv.g);
   float4 color = float4(0, 0, 0, 1);
   //First row uses texture from S0
   if(rowIndex  == 0)
   {
      color = tex2D(ImageSample0, texuv); 
   }
   //The second row gets color mixed from S0 and S1
   else if(rowIndex == 1)
   {       
      color = (tex2D(ImageSample0, texuv) + tex2D(ImageSample1, texuv)) / 2; 
   }
   //The third row uses texture from S1
   else if(rowIndex == 2)
   {       
      color = tex2D(ImageSample1, texuv); 
   }
    
   return color;
}



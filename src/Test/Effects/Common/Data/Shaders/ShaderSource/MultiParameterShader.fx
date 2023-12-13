
// Object Declarations
float4 floatValue : register(C0);
float4 doubleValue : register(C1);
float4 pointValue : register(C2);
float4 point3DValue : register(C3);
float4 point4DValue : register(C4);
float4 vector3DValue : register(C5);
float4 sizeValue : register(C6);
float4 vectorValue : register(C7);
float4 colorValue : register(C8);
float4 destinationSize : register(C9);
            

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
   
   int columnIndex = floor(9.0 * texuv.r);
   float4 color = float4(0, 0, 0, 1);
   if(columnIndex == 0)
   {
      color = float4(floatValue.rgb / 1000, 1); 
   }
   else if(columnIndex == 1)
   {       
      color = float4(doubleValue.rgb / 1000, 1);
   }
   else if(columnIndex == 2)
   {       
      color.rg = pointValue / 100; 
   }
   else if(columnIndex == 3)
   {       
      color.rgb = point3DValue / 100;
   }
   else if(columnIndex == 4)
   {       
      color = point4DValue / 100;
   }
   else if(columnIndex == 5)
   {       
      color.rgb = vector3DValue / 100;
   }
   else if(columnIndex == 6)
   {       
      color.rg = sizeValue / 100;
   }
   else if(columnIndex == 7)
   {       
      color.rg = vectorValue / 100;
   }
   else if(columnIndex == 8)
   {       
      color = colorValue;
   }
   
   //Boundaries. 
   if(abs(9.0 * texuv.r - columnIndex) < 0.1)
   {
      color = float4(0, 0, 0, 1);
   }  
   return color;
}



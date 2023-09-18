// Return color based on ddxuvddyuv value passed in through register 1. 
// The Rectangle on which effect is applied has the size of (200, 100). 
// ddxvuddyub passed in should be (1/200, 1/100). This shader return a color 
// (1, 1, 1, 0) if the value has been passed in correctly. 

// Object Declarations
float4 ddxuvddyuv : register(c1);

SamplerState  ImageSampler : register(S0);


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS() : SV_Target
{  
   float4 result = float4(1,0,0,0);
   result.r = 200 * ddxuvddyuv.a;
   result.g = 100 * ddxuvddyuv.r;
   result.a=1;
   return result;
}



// Shader take texture from all sampler register and draw them in different rows. 

// Object Declarations
sampler2D ImageSample0 : register(S0);
sampler2D ImageSample1 : register(S1);
sampler2D ImageSample2 : register(S2);
sampler2D ImageSample3 : register(S3);
sampler2D ImageSample4 : register(S4);
sampler2D ImageSample5 : register(S5);
sampler2D ImageSample6 : register(S6);
sampler2D ImageSample7 : register(S7);
            


//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------

float4 PS( float2 texuv : TEXCOORD0) : COLOR0
{
   
   int rowIndex = floor(8.0 * texuv.g);
   float4 color = float4(0, 0, 0, 1);
   //First row uses texture from S0...
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
   else if(rowIndex == 4)
   {       
      color = tex2D(ImageSample4, texuv); 
   }    
   else if(rowIndex == 5)
   {       
      color = tex2D(ImageSample5, texuv); 
   }    
   else if(rowIndex == 6)
   {       
      color = tex2D(ImageSample6, texuv); 
   }    
   else if(rowIndex == 7)
   {       
      color = tex2D(ImageSample7, texuv); 
   }    
   return color;
}

sampler2D implicitInput : register(s0);
float2 uv : TEXCOORD;


// INT
int intVar0  : register(i0);
int intVar1  : register(i1);
int intVar2  : register(i2);
int intVar3  : register(i3);
int intVar4  : register(i4);
int intVar5  : register(i5);
int intVar6  : register(i6);
int intVar7  : register(i7);
int intVar8  : register(i8);
int intVar9  : register(i9);
int intVar10  : register(i10);
int intVar11  : register(i11);
int intVar12  : register(i12);
int intVar13  : register(i13);
int intVar14  : register(i14);
int intVar15 : register(i15);

 
//With Dx10 compiler, value passed through i* can only use in loop control. 
float GetFloatFromInt(int intval)
{
    float val = 0.0;
    for(int i = 0; i< intval; i++)
    {
       val = val + 1.0 / 256.0;
    }
    return val;
}

float4 GetColorFromInt(int intval)
{
    float4 color = float4(0, 0, 0, 1);
    
    float floatvar = GetFloatFromInt(intval);

    color.r = frac(floatvar);
    
    floatvar = (floatvar - frac(floatvar)) / 256.0;
    color.g = frac(floatvar);

    floatvar = (floatvar - frac(floatvar)) / 256.0;
    color.b = frac(floatvar);
    
    return color;
}

float4 GetColorBasedOnIntValue(int columnIndex)
{
   float4 color = float4(0, 0, 0, 1);
   
   if(columnIndex == 0)
   {
      color = GetColorFromInt(intVar0);
   }
   
   else if(columnIndex == 1)
   {
      color = GetColorFromInt(intVar1);
   }
   else if(columnIndex == 2)
   {
      color = GetColorFromInt(intVar2);
   }
   else if(columnIndex == 3)
   {
      color = GetColorFromInt(intVar3);
   }
   else if(columnIndex == 4)
   {
      color = GetColorFromInt(intVar4);
   }
   else if(columnIndex == 5)
   {
      color = GetColorFromInt(intVar5);
   }
   else if(columnIndex == 6)
   {
      color = GetColorFromInt(intVar6);
   }
   else if(columnIndex == 7)
   {
      color = GetColorFromInt(intVar7);
   }
   else if(columnIndex == 8)
   {
      color = GetColorFromInt(intVar8);
   }
   else if(columnIndex == 9)
   {
      color = GetColorFromInt(intVar9);
   }
   else if(columnIndex == 10)
   {
      color = GetColorFromInt(intVar10);
   }
   else if(columnIndex == 11)
   {
      color = GetColorFromInt(intVar11);
   }
   else if(columnIndex == 12)
   {
      color = GetColorFromInt(intVar12);
   }
   else if(columnIndex == 13)
   {
      color = GetColorFromInt(intVar13);
   }
   else if(columnIndex == 14)
   {
      color = GetColorFromInt(intVar14);
   }
   else if(columnIndex == 15)
   {
      color = GetColorFromInt(intVar15);
   }
   return color;
}
 

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS( float2 texuv : TEXCOORD ) : COLOR
{  
   int columnIndex = floor(16 * texuv.r);
   
   float4 color = GetColorBasedOnIntValue(columnIndex);
      
   return color;
}

  



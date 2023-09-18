sampler2D implicitInput : register(s0);
float2 uv : TEXCOORD;
// BOOL
bool boolVar0 : register(b0);
bool boolVar1 : register(b1);
bool boolVar2 : register(b2);
bool boolVar3 : register(b3);
bool boolVar4 : register(b4);
bool boolVar5 : register(b5);
bool boolVar6 : register(b6);
bool boolVar7 : register(b7);
bool boolVar8 : register(b8);
bool boolVar9 : register(b9);
bool boolVar10 : register(b10);
bool boolVar11 : register(b11);
bool boolVar12 : register(b12);
bool boolVar13 : register(b13);
bool boolVar14 : register(b14);
bool boolVar15 : register(b15);

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
//double
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
float4 double10 : register(C10);
float4 double50 : register(C50);
float4 double125 : register(C125);
float4 double180 : register(C180);
float4 double200 : register(C200);
float4 double220 : register(C220);
float4 double223 : register(C223);

float4 GetColorBasedonBool(bool boolv)
{
    float4 color = tex2D(implicitInput, uv);
    
    if(boolv) 
    {
        color = float4(0, 0, 1, 1);
    }  		
	
    return color;
}
  
float4 GetColorBasedOnBoolValue (int columnIndex)
{	  

   float4 color;

   if(columnIndex == 0)
   {
      color = GetColorBasedonBool(boolVar0);
   }
   else if(columnIndex == 1)
   {
      color = GetColorBasedonBool(boolVar1);
   }
   else if(columnIndex == 2)
   {
      color = GetColorBasedonBool(boolVar2);
   }
   else if(columnIndex == 3)
   {
      color = GetColorBasedonBool(boolVar3);
   }
   else if(columnIndex == 4)
   {
      color = GetColorBasedonBool(boolVar4);
   }
   else if(columnIndex == 5)
   {
      color = GetColorBasedonBool(boolVar5);
   }
   else if(columnIndex == 6)
   {
      color = GetColorBasedonBool(boolVar6);
   }
   else if(columnIndex == 7)
   {
      color = GetColorBasedonBool(boolVar7);
   }
   else if(columnIndex == 8)
   {
      color = GetColorBasedonBool(boolVar8);
   }
   else if(columnIndex == 9)
   {
      color = GetColorBasedonBool(boolVar9);
   }
   else if(columnIndex == 10)
   {
      color = GetColorBasedonBool(boolVar10);
   }
   else if(columnIndex == 11)
   {
      color = GetColorBasedonBool(boolVar11);
   }
   else if(columnIndex == 12)
   {
      color = GetColorBasedonBool(boolVar12);
   }
   else if(columnIndex == 13)
   {
      color = GetColorBasedonBool(boolVar13);
   }
   else if(columnIndex == 14)
   {
      color = GetColorBasedonBool(boolVar14);
   }
   else if(columnIndex == 15)
   {
      color = GetColorBasedonBool(boolVar15);
   }
   return color;
 }
 
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
   // code handling int 6 - 12 has been moved MultiIntParameterShader3.fx. Original shader is too complicated. 
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
 
float4 GetColorBasedOnDouble(int columnIndex)
{
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
   else if(columnIndex == 9)
   {
       color = double10;
   }
   else if(columnIndex == 10)
   {
       color = double50;
   }
   else if(columnIndex == 11)
   {
       color = double125;
   }
   else if(columnIndex == 12)
   {
       color = double180;
   }
   else if(columnIndex == 13)
   {
       color = double200;
   }
   else if(columnIndex == 14)
   {
       color = double220;
   }
   else if(columnIndex == 15)
   {
       color = double223;
   }
   color.a = 1;
   return color;
}
float4 GetColor (int columnIndex, int rowIndex)
{
    float4 color = float4(1, 1, 0, 1);
    if(rowIndex == 0)
    {
        color = GetColorBasedOnBoolValue(columnIndex);
    }
    else if(rowIndex == 1)
    {
         color = GetColorBasedOnIntValue(columnIndex);
    }
    else
    {
       color = GetColorBasedOnDouble(columnIndex);
    }
    return color;
}

//--------------------------------------------------------------------------------------
// Pixel Shader
//--------------------------------------------------------------------------------------
float4 PS( float2 texuv : TEXCOORD ) : COLOR
{  
   int columnIndex = floor(16 * texuv.r);
   int rowIndex = floor(3 * texuv.g);
   
   float4 color = GetColor(columnIndex, rowIndex);
   
   return color;
}

  





//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

//
// Samplers...
//

//
// Vertex Shader Stage Data
//

struct VS_STAGE_DATA
{
   float2 UV;
   float4 Color;
};

//
// Vertex Fragment Data...
//

struct Get3DTransforms_VS0_VS_ConstData
{
    float4x4 mat4x4WorldViewTransform;
    float4x4 mat4x4WorldViewProjTransform;
    float4x4 mat4x4WorldViewAdjTransTransform;
};

struct Ambient_Lighting_VS3_VS_ConstData
{
    float4 Color;
};

//
// Vertex Shader Constant Data
//

struct VertexShaderConstantData
{
    Get3DTransforms_VS0_VS_ConstData Get3DTransforms_VS0_ConstantTable;
    Ambient_Lighting_VS3_VS_ConstData Ambient_Lighting_VS3_ConstantTable;
};

cbuffer cbVSUpdateEveryCall
{
    VertexShaderConstantData Data_VS;
};

//
// Pixel Shader Stage Data
//

struct PS_STAGE_DATA
{
   float4 Color;
};

//
// Pixel Fragment Data...
//

struct SetColorValue_PS1_PS_ConstData
{
    float4 color;

};

//
// Pixel Shader Constant Data
//

struct PixelShaderConstantData
{
    SetColorValue_PS1_PS_ConstData SetColorValue_PS1_ConstantTable;
};

cbuffer cbPSUpdateEveryCall
{
    PixelShaderConstantData Data_PS;
};


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Diffuse  : TEXCOORD0;
    float4 UV_0 : TEXCOORD1;
};

//
// Fragment Vertex Shader functions...
//
void
Get3DTransforms_VS0(
    in Get3DTransforms_VS0_VS_ConstData Data,
    out float4x4 mat4x4WorldViewTransform,
    out float4x4 mat4x4WorldViewProjTransform,
    out float4x4 mat4x4WorldViewAdjTransTransform
    )
{
    mat4x4WorldViewTransform         = Data.mat4x4WorldViewTransform;
    mat4x4WorldViewProjTransform     = Data.mat4x4WorldViewProjTransform;
    mat4x4WorldViewAdjTransTransform = Data.mat4x4WorldViewAdjTransTransform;
}
void
SetVertexStageColorToInputDiffuse_VS2(
    float4 Diffuse,
    inout VS_STAGE_DATA vsStageData
    )
{
    vsStageData.Color = Diffuse;
}
void
MoveVertexStageColorToInterpolator_VS2(
    VS_STAGE_DATA vsStageData,
    inout float4 BlendColor
    )
{
    BlendColor = vsStageData.Color;
}
void
Ambient_Lighting_VS3(
    Ambient_Lighting_VS3_VS_ConstData Data,
    inout VertexShaderOutput Output
    )
{
    Output.Diffuse = Data.Color;
}


//
// Main Vertex Shader
//



VertexShaderOutput
VS(
    float3 Position : POSITION,
    float3 Normal : TEXCOORD0,
    float2 UV_0 : TEXCOORD1
    )
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

    // These will be optimized away when not in use
    float4x4      View, WorldView, WorldViewProj, WorldViewAdjTrans;
    float         SpecularPower;
    VS_STAGE_DATA vsStageData;


    float4 Diffuse = {1.0f, 1.0f, 1.0f, 1.0f};
    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    Get3DTransforms_VS0(
        Data_VS.Get3DTransforms_VS0_ConstantTable,
        WorldView,
        WorldViewProj,
        WorldViewAdjTrans
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS2(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS2(
        vsStageData,
        Output.UV_0
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    Ambient_Lighting_VS3(
        Data_VS.Ambient_Lighting_VS3_ConstantTable,
        Output
        );

    Output.Diffuse.rgb = min(Output.Diffuse.rgb, 1.0);

    return Output;
};

//
// Fragment Pixel Shader fragments...
//
void
SetColorValue_PS1(
    SetColorValue_PS1_PS_ConstData data,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = data.color;
}
void
Multiply_Premultiplied_Color_PS1(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor *= psStageData.Color;
}
void
MoveInterpolatorToPixelStageColor_PS2(
    float4 BlendColor,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = BlendColor;
}
void
Multiply_NonPremultiplied_Color_PS2(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor.a *= psStageData.Color.a;
}


//
// Main Pixel Shader
//


float4
PS(
    float4 Position : SV_POSITION,
    float4 Diffuse  : TEXCOORD0,
    float4 UV_0 : TEXCOORD1
    ) : SV_Target
{
    float4        curColor = Diffuse;
    PS_STAGE_DATA psStageData;

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    SetColorValue_PS1(
        Data_PS.SetColorValue_PS1_ConstantTable,
        psStageData        );

    Multiply_Premultiplied_Color_PS1(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS2(
        UV_0,
        psStageData        );

    Multiply_NonPremultiplied_Color_PS2(
        psStageData,
        curColor
        );

    return curColor;
};

//
// Technique
//

technique T0
{
    pass P0
    {
        SetVertexShader( CompileShader( vs_4_0, VS() ) );
        SetGeometryShader( NULL );
        SetPixelShader( CompileShader( ps_4_0, PS() ) );
    }
}

//
// End of Dynamic Shader Code
//

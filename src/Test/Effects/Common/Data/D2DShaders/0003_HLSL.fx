

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

struct Transform_World2D_By_Matrix4x4_VS0_VS_ConstData
{
    float4x4 mat4x4WorldToProjection;
};

//
// Vertex Shader Constant Data
//

struct VertexShaderConstantData
{
    Transform_World2D_By_Matrix4x4_VS0_VS_ConstData Transform_World2D_By_Matrix4x4_VS0_ConstantTable;
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


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Diffuse  : TEXCOORD0;
    float4 UV_0 : TEXCOORD1;
    float4 UV_1 : TEXCOORD2;
};

//
// Fragment Vertex Shader functions...
//
void
Transform_World2D_By_Matrix4x4_VS0(
    float3 WorldPos2D,
    Transform_World2D_By_Matrix4x4_VS0_VS_ConstData Data,
    inout VertexShaderOutput Output
    )
{
    Output.Diffuse = float4(1.0, 1.0, 1.0, 1.0);
    Output.Position = mul(float4(WorldPos2D, 1.0f), Data.mat4x4WorldToProjection);
}
void
SetVertexStageColorToInputDiffuse_VS1(
    float4 Diffuse,
    inout VS_STAGE_DATA vsStageData
    )
{
    vsStageData.Color = Diffuse;
}
void
MoveVertexStageColorToInterpolator_VS1(
    VS_STAGE_DATA vsStageData,
    inout float4 BlendColor
    )
{
    BlendColor = vsStageData.Color;
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


//
// Main Vertex Shader
//



VertexShaderOutput
VS(
    float3 Position : POSITION,
    float4 Diffuse : TEXCOORD0,
    float2 UV_0 : TEXCOORD1,
    float2 UV_1 : TEXCOORD2
    )
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

    // These will be optimized away when not in use
    float4x4      View, WorldView, WorldViewProj, WorldViewAdjTrans;
    float         SpecularPower;
    VS_STAGE_DATA vsStageData;


    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    Transform_World2D_By_Matrix4x4_VS0(
        Position,
        Data_VS.Transform_World2D_By_Matrix4x4_VS0_ConstantTable,
        Output
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS1(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS1(
        vsStageData,
        Output.UV_0
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS2(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS2(
        vsStageData,
        Output.UV_1
        );

    Output.Diffuse.rgb = min(Output.Diffuse.rgb, 1.0);

    return Output;
};

//
// Fragment Pixel Shader fragments...
//
void
MoveInterpolatorToPixelStageColor_PS1(
    float4 BlendColor,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = BlendColor;
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
Multiply_Premultiplied_Color_PS2(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor *= psStageData.Color;
}


//
// Main Pixel Shader
//


float4
PS(
    float4 Position : SV_POSITION,
    float4 Diffuse  : TEXCOORD0,
    float4 UV_0 : TEXCOORD1,
    float4 UV_1 : TEXCOORD2
    ) : SV_Target
{
    float4        curColor = Diffuse;
    PS_STAGE_DATA psStageData;

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS1(
        UV_0,
        psStageData        );

    Multiply_Premultiplied_Color_PS1(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS2(
        UV_1,
        psStageData        );

    Multiply_Premultiplied_Color_PS2(
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

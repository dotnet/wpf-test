

//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

//
// Samplers...
//

sampler RadialCenteredSampleFromInterpolatorUV1_Sampler;

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

struct RadialCenteredSampleFromInterpolatorUV_PS1_PS_ConstData
{
    float flHalfTexelSizeNormalized;

};

struct SetColorValue_PS2_PS_ConstData
{
    float4 color;

};

//
// Pixel Shader Constant Data
//

struct PixelShaderConstantData
{
    RadialCenteredSampleFromInterpolatorUV_PS1_PS_ConstData RadialCenteredSampleFromInterpolatorUV_PS1_ConstantTable;
    SetColorValue_PS2_PS_ConstData SetColorValue_PS2_ConstantTable;
};

cbuffer cbPSUpdateEveryCall
{
    PixelShaderConstantData Data_PS;
};


struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Diffuse  : TEXCOORD0;
    float2 UV_0 : TEXCOORD1;
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
SetVertexStageUVToInputUV_VS1(
    float2 UV,
    inout VS_STAGE_DATA vsStageData
    )
{
    vsStageData.UV = UV;
}
void
MoveVertexStageUVToInterpolator_VS1(
    VS_STAGE_DATA vsStageData,
    inout float2 uv
    )
{
    uv = vsStageData.UV;
}
void
SetVertexStageColorToInputDiffuse_VS3(
    float4 Diffuse,
    inout VS_STAGE_DATA vsStageData
    )
{
    vsStageData.Color = Diffuse;
}
void
MoveVertexStageColorToInterpolator_VS3(
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

    SetVertexStageUVToInputUV_VS1(
        UV_0,
        vsStageData
        );

    MoveVertexStageUVToInterpolator_VS1(
        vsStageData,
        Output.UV_0
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS3(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS3(
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
RadialCenteredSampleFromInterpolatorUV_PS1(
    sampler TextureSampler,
    float2 samplePos,
    RadialCenteredSampleFromInterpolatorUV_PS1_PS_ConstData GradInfoParams,
    inout PS_STAGE_DATA psStageData
    )
{
    // Get distance (in unit circle) from sample point to the gradient origin:
    float uc_dx = samplePos.x;
    float uc_dy = samplePos.y;

    float distToOriginSqr = uc_dx*uc_dx + uc_dy*uc_dy;

    // Simple radial gradient
    float sampleGradientTexCoord = sqrt(distToOriginSqr);

    // Ensure that the gradient space does not wrap around,
    // interpolating with the last stop at the center point.
    if (sampleGradientTexCoord < GradInfoParams.flHalfTexelSizeNormalized)
    {
        sampleGradientTexCoord = GradInfoParams.flHalfTexelSizeNormalized;
    }

    psStageData.Color = tex1D(TextureSampler, sampleGradientTexCoord);
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
SetColorValue_PS2(
    SetColorValue_PS2_PS_ConstData data,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = data.color;
}
void
Multiply_AlphaOnly_Premultiplied_PS2(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor *= psStageData.Color.a;
}
void
MoveInterpolatorToPixelStageColor_PS3(
    float4 BlendColor,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = BlendColor;
}
void
Multiply_Premultiplied_Color_PS3(
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
    float2 UV_0 : TEXCOORD1,
    float4 UV_1 : TEXCOORD2
    ) : SV_Target
{
    float4        curColor = Diffuse;
    PS_STAGE_DATA psStageData;

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    RadialCenteredSampleFromInterpolatorUV_PS1(
        RadialCenteredSampleFromInterpolatorUV1_Sampler,
        UV_0,
        Data_PS.RadialCenteredSampleFromInterpolatorUV_PS1_ConstantTable,
        psStageData        );

    Multiply_Premultiplied_Color_PS1(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    SetColorValue_PS2(
        Data_PS.SetColorValue_PS2_ConstantTable,
        psStageData        );

    Multiply_AlphaOnly_Premultiplied_PS2(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS3(
        UV_1,
        psStageData        );

    Multiply_Premultiplied_Color_PS3(
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



//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

//
// Samplers...
//

sampler SuperSampleTextureFromInterpolatorUV1_Sampler;

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

struct ExpandUVToSuperSampleUVInterpolator_VS1_VS_ConstData
{
    float4 ddxyEstimated;
};

//
// Vertex Shader Constant Data
//

struct VertexShaderConstantData
{
    Transform_World2D_By_Matrix4x4_VS0_VS_ConstData Transform_World2D_By_Matrix4x4_VS0_ConstantTable;
    ExpandUVToSuperSampleUVInterpolator_VS1_VS_ConstData ExpandUVToSuperSampleUVInterpolator_VS1_ConstantTable;
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
    float4 UV_2 : TEXCOORD3;
    float4 UV_3 : TEXCOORD4;
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
ExpandUVToSuperSampleUVInterpolator_VS1(
    VS_STAGE_DATA vsStageData,
    ExpandUVToSuperSampleUVInterpolator_VS1_VS_ConstData data,
    inout float4 outputUV01,
    inout float4 outputUV23
    )
{
    float2 unitUMinor = float2(data.ddxyEstimated.x / 8, 0);
    float2 unitVMinor = float2(0, data.ddxyEstimated.y / 8);
    float2 unitUMajor = unitUMinor * 3;
    float2 unitVMajor = unitVMinor * 3;
    outputUV01.xy = vsStageData.UV + unitUMajor - unitVMinor;
    outputUV01.zw = vsStageData.UV - unitUMinor - unitVMajor;
    outputUV23.xy = vsStageData.UV - unitUMajor + unitVMinor;
    outputUV23.zw = vsStageData.UV + unitUMinor + unitVMajor;
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

    ExpandUVToSuperSampleUVInterpolator_VS1(
        vsStageData,
        Data_VS.ExpandUVToSuperSampleUVInterpolator_VS1_ConstantTable,
        Output.UV_0,
        Output.UV_1
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS2(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS2(
        vsStageData,
        Output.UV_2
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    SetVertexStageColorToInputDiffuse_VS3(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS3(
        vsStageData,
        Output.UV_3
        );

    Output.Diffuse.rgb = min(Output.Diffuse.rgb, 1.0);

    return Output;
};

//
// Fragment Pixel Shader fragments...
//
void
SuperSampleTextureFromInterpolatorUV_PS1(
    sampler TextureSampler,
    float4 uv01,
    float4 uv23,
    inout PS_STAGE_DATA psStageData
    )
{
    float4 sampledColor0 = tex2D(TextureSampler, uv01.xy);
    float4 sampledColor1 = tex2D(TextureSampler, uv01.zw);
    float4 sampledColor2 = tex2D(TextureSampler, uv23.xy);
    float4 sampledColor3 = tex2D(TextureSampler, uv23.zw);

    psStageData.Color = (sampledColor0 + sampledColor1 + sampledColor2 + sampledColor3) / 4;
}
void
MakeOpaque_PS1(
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color.a = 1.0;
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
    float4 UV_0 : TEXCOORD1,
    float4 UV_1 : TEXCOORD2,
    float4 UV_2 : TEXCOORD3,
    float4 UV_3 : TEXCOORD4
    ) : SV_Target
{
    float4        curColor = Diffuse;
    PS_STAGE_DATA psStageData;

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    SuperSampleTextureFromInterpolatorUV_PS1(
        SuperSampleTextureFromInterpolatorUV1_Sampler,
        UV_0,
        UV_1,
        psStageData        );

    MakeOpaque_PS1(
        psStageData        );

    Multiply_Premultiplied_Color_PS1(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS2(
        UV_2,
        psStageData        );

    Multiply_Premultiplied_Color_PS2(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS3(
        UV_3,
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

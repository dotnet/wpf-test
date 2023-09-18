

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

struct Get3DTransforms_VS0_VS_ConstData
{
    float4x4 mat4x4WorldViewTransform;
    float4x4 mat4x4WorldViewProjTransform;
    float4x4 mat4x4WorldViewAdjTransTransform;
};

struct TransformVertexStageUV_VS1_VS_ConstData
{
    float4x2 mat3x2TextureTransform;
};

struct ExpandUVToSuperSampleUVInterpolator_VS1_VS_ConstData
{
    float4 ddxyEstimated;
};

struct Ambient_Lighting_VS4_VS_ConstData
{
    float4 Color;
};

//
// Vertex Shader Constant Data
//

struct VertexShaderConstantData
{
    Get3DTransforms_VS0_VS_ConstData Get3DTransforms_VS0_ConstantTable;
    TransformVertexStageUV_VS1_VS_ConstData TransformVertexStageUV_VS1_ConstantTable;
    ExpandUVToSuperSampleUVInterpolator_VS1_VS_ConstData ExpandUVToSuperSampleUVInterpolator_VS1_ConstantTable;
    Ambient_Lighting_VS4_VS_ConstData Ambient_Lighting_VS4_ConstantTable;
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

struct TransformColor_PS2_PS_ConstData
{
    float4x4 mat4x4;
    float4 matRow4;

};

//
// Pixel Shader Constant Data
//

struct PixelShaderConstantData
{
    TransformColor_PS2_PS_ConstData TransformColor_PS2_ConstantTable;
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
    float4 UV_1 : TEXCOORD2;
    float4 UV_2 : TEXCOORD3;
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
SetVertexStageUVToInputUV_VS1(
    float2 UV,
    inout VS_STAGE_DATA vsStageData
    )
{
    vsStageData.UV = UV;
}
void
TransformVertexStageUV_VS1(
    TransformVertexStageUV_VS1_VS_ConstData data,
    inout VS_STAGE_DATA vsStageData
    )
{
    float2 inputUV = vsStageData.UV;
    vsStageData.UV.x = inputUV.x*data.mat3x2TextureTransform[0][0] + inputUV.y*data.mat3x2TextureTransform[1][0] + data.mat3x2TextureTransform[2][0];
    vsStageData.UV.y = inputUV.x*data.mat3x2TextureTransform[0][1] + inputUV.y*data.mat3x2TextureTransform[1][1] + data.mat3x2TextureTransform[2][1];
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
void
Ambient_Lighting_VS4(
    Ambient_Lighting_VS4_VS_ConstData Data,
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
    float4 Diffuse : TEXCOORD1,
    float2 UV_0 : TEXCOORD2
    )
{
    VertexShaderOutput Output = (VertexShaderOutput)0;

    // These will be optimized away when not in use
    float4x4      View, WorldView, WorldViewProj, WorldViewAdjTrans;
    float         SpecularPower;
    VS_STAGE_DATA vsStageData;


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

    SetVertexStageUVToInputUV_VS1(
        UV_0,
        vsStageData
        );

    TransformVertexStageUV_VS1(
        Data_VS.TransformVertexStageUV_VS1_ConstantTable,
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

    SetVertexStageColorToInputDiffuse_VS3(
        Diffuse,
        vsStageData
        );

    MoveVertexStageColorToInterpolator_VS3(
        vsStageData,
        Output.UV_2
        );

    vsStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);
    vsStageData.UV = float2(0.0, 0.0);

    Ambient_Lighting_VS4(
        Data_VS.Ambient_Lighting_VS4_ConstantTable,
        Output
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
Multiply_Premultiplied_Color_PS1(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor *= psStageData.Color;
}
void
SetColorValue_PS2(
    float4 color,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = color;
}
void
TransformColor_PS2(
    TransformColor_PS2_PS_ConstData data,
    inout PS_STAGE_DATA psStageData
    )
{
    psStageData.Color = mul(psStageData.Color,data.mat4x4) + data.matRow4;
}
void
SetCurrentPipelineColorToPipelineColor_PS2(
    PS_STAGE_DATA psStageData,
    inout float4 curPixelColor
    )
{
    curPixelColor = psStageData.Color;
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
Multiply_NonPremultiplied_Color_PS3(
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
    float4 UV_0 : TEXCOORD1,
    float4 UV_1 : TEXCOORD2,
    float4 UV_2 : TEXCOORD3
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

    Multiply_Premultiplied_Color_PS1(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    SetColorValue_PS2(
        curColor
,
        psStageData        );

    TransformColor_PS2(
        Data_PS.TransformColor_PS2_ConstantTable,
        psStageData        );

    SetCurrentPipelineColorToPipelineColor_PS2(
        psStageData,
        curColor
        );

    psStageData.Color = float4(1.0 , 1.0, 1.0, 1.0);

    MoveInterpolatorToPixelStageColor_PS3(
        UV_2,
        psStageData        );

    Multiply_NonPremultiplied_Color_PS3(
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

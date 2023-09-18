

//
// Copyright (c) Microsoft Corporation.  All rights reserved.
//

//
// Samplers...
//

sampler RadialNonCenteredSampleFromInterpolatorUV1_Sampler;

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

struct RadialNonCenteredSampleFromInterpolatorUV_PS1_PS_ConstData
{
    float2 ptGradOrigin;
    float2 ptFirstTexelRegionCenter;
    float gradientSpanNormalized;
    float flHalfTexelSizeNormalized;

};

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
    RadialNonCenteredSampleFromInterpolatorUV_PS1_PS_ConstData RadialNonCenteredSampleFromInterpolatorUV_PS1_ConstantTable;
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
RadialNonCenteredSampleFromInterpolatorUV_PS1(
    sampler TextureSampler,
    float2 samplePos,
    RadialNonCenteredSampleFromInterpolatorUV_PS1_PS_ConstData GradInfoParams,
    inout PS_STAGE_DATA psStageData
    )
{
    //
    // There are overflow issues in refrast and hw implementation of clamping.
    // Therefore we need to clamp ourselves in areas of the shader that have a
    // high risk of overflowing.
    //
    // We will go with 32768 as the maximum number of wraps that's support in 
    // supported since that's what refrast has.
    //
    #define MAX_RELIABLE_WRAP_VALUE 32768

    float u;
    
    float2 sampleToFirstTexelRegionCenter = samplePos - GradInfoParams.ptFirstTexelRegionCenter;
    float firstTexelRegionRadiusSquared = GradInfoParams.flHalfTexelSizeNormalized * GradInfoParams.flHalfTexelSizeNormalized;
    
    if (dot(sampleToFirstTexelRegionCenter, sampleToFirstTexelRegionCenter) <
        firstTexelRegionRadiusSquared)
    {
        u = GradInfoParams.flHalfTexelSizeNormalized;
    }
    else
    {
        // Get distance (in unit circle) from sample point to the gradient origin:
        float2 sampleToOrigin = samplePos - GradInfoParams.ptGradOrigin;
    
        float A = dot(sampleToOrigin, sampleToOrigin);
        
        float B = 2.0f * dot(GradInfoParams.ptGradOrigin, sampleToOrigin);

        float2 ptGradOriginPerp = {GradInfoParams.ptGradOrigin.y, -GradInfoParams.ptGradOrigin.x};
        float sampleToOriginCrossOriginNorm = dot(sampleToOrigin, ptGradOriginPerp);

        // see brushspan.cpp for an explanation of why the determinant is calculated this way.
        float determinant = 
            4.0f * (  GradInfoParams.gradientSpanNormalized * GradInfoParams.gradientSpanNormalized * A
                    - sampleToOriginCrossOriginNorm * sampleToOriginCrossOriginNorm);
        
        if (0.0f > determinant)
        {
            // This complex region appears when the gradient origin is outside the
            // ellipse defining the end of the gradient. When rendering this region
            // we choose the last texel color.
            u = 1.0f - GradInfoParams.flHalfTexelSizeNormalized;
        }
        else
        {
            u = (2 * A * GradInfoParams.gradientSpanNormalized) / (sqrt(determinant) - B);
            
            if (u < GradInfoParams.flHalfTexelSizeNormalized)
            {
                if (u < 0.0)
                {
                    // This negative region appears when the gradient origin is outside the
                    // ellipse defining the end of the gradient. When rendering this region
                    // we choose the last texel color.
                    u = 1.0f - GradInfoParams.flHalfTexelSizeNormalized;
                }
                else                                                         
                {
                    // Ensure that the gradient space does not wrap around,
                    // interpolating with the last stop at the center point.
                    // This value for u picks the first texel in the texture.
                    
                    // Given an infinite precicision machine, we'd never get to this case since
                    // we should have skipped the quadratic equation up top. Nevertheless,
                    // we do not have an infinite precision machine, so we may still get here.
                    u = GradInfoParams.flHalfTexelSizeNormalized;
                }
            }   
            else
            {
                //
                // Refrast & probably hw implement wrapping/clamping logic by first casting
                // the float to an integer and then doing integer math.  They are not robust
                // against integer overflow, so we need to do the check manually.
                //

                if (u > MAX_RELIABLE_WRAP_VALUE)
                {
                    u = 1.0f;
                }
            }
        }
    }    

    psStageData.Color = tex2D(TextureSampler, u);
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

    RadialNonCenteredSampleFromInterpolatorUV_PS1(
        RadialNonCenteredSampleFromInterpolatorUV1_Sampler,
        UV_0,
        Data_PS.RadialNonCenteredSampleFromInterpolatorUV_PS1_ConstantTable,
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

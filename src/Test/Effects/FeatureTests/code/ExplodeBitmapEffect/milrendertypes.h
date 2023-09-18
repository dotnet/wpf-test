// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*=========================================================================*\

    Copyright (c) Microsoft Corporation.  All right reserved.

    Module Name: MILRender

\*=========================================================================*/

#pragma once

/*=========================================================================*\
    Forward D3D type declarations

      To use these D3D types include the appropriate d3d header file(s).

\*=========================================================================*/

#define MIL_MAX_NUM_LIGHTS 8

#define MIL_FORCE_DWORD 0x7FFFFFFF


struct MILPointF
{
    FLOAT X;
    FLOAT Y;
};


struct MILPoint
{
    INT X;
    INT Y;
};


struct MILRectF_WH
{
    FLOAT X;
    FLOAT Y;
    FLOAT Width;
    FLOAT Height;
};

typedef MILRectF_WH MILRectF;

struct MILRectF_RB
{
    FLOAT left;
    FLOAT top;
    FLOAT right;
    FLOAT bottom;
};


/*
// MILRect is already defined by WinCodec as WICRect
//  See WinCodec_private.h.  Below is the layout
//  we expect to see.  IF we can break the dependence
//  then we can use the defines list here, which match
//  the FLOAT version defines.
//
struct MILRect_WH
{
    INT X;
    INT Y;
    INT Width;
    INT Height;
};

typedef MILRect_WH MILRect;

*/

typedef WICRect MILRect_WH;

typedef RECT MILRect_RB;

struct MILRectU_RB
{
    UINT left;
    UINT top;
    UINT right;
    UINT bottom;
};


struct MILBoxF
{
    FLOAT X;
    FLOAT Y;
    FLOAT Z;
    FLOAT LengthX;
    FLOAT LengthY;
    FLOAT LengthZ;
};

struct MILPointD
{
    double X;
    double Y;
};

struct MILRectD
{
    double X;
    double Y;
    double Width;
    double Height;
};

const MILRectD EMPTY_MILRectD = { 0.0, 0.0, -1.0, -1.0 };

/*=========================================================================*\
    Color types
\*=========================================================================*/

typedef UINT64 MILColor64;

const INT MIL_ALPHA_SHIFT = 24;
const INT MIL_RED_SHIFT   = 16;
const INT MIL_GREEN_SHIFT =  8;
const INT MIL_BLUE_SHIFT  =  0;

#define MIL_ALPHA_MASK  ((MILColor) 0xff << MIL_ALPHA_SHIFT)
#define MIL_RED_MASK    ((MILColor) 0xff << MIL_RED_SHIFT)
#define MIL_GREEN_MASK  ((MILColor) 0xff << MIL_GREEN_SHIFT)
#define MIL_BLUE_MASK   ((MILColor) 0xff << MIL_BLUE_SHIFT)

#define MIL_COLOR(a, r, g, b) \
    (MILColor)(((a) & 0xFF) << MIL_ALPHA_SHIFT | \
     ((r) & 0xFF) << MIL_RED_SHIFT | \
     ((g) & 0xFF) << MIL_GREEN_SHIFT | \
     ((b) & 0xFF) << MIL_BLUE_SHIFT)

#define MIL_COLOR_GET_ALPHA(c) (((c) & MIL_ALPHA_MASK) >> MIL_ALPHA_SHIFT)
#define MIL_COLOR_GET_RED(c)   (((c) & MIL_RED_MASK)   >> MIL_RED_SHIFT)
#define MIL_COLOR_GET_GREEN(c) (((c) & MIL_GREEN_MASK) >> MIL_GREEN_SHIFT)
#define MIL_COLOR_GET_BLUE(c)  (((c) & MIL_BLUE_MASK)  >> MIL_BLUE_SHIFT)

typedef D3DCOLORVALUE MILColorF;

struct MILICCProfile
{
    DWORD   dwType;             // profile type
    PVOID   pProfileData;       // filename or buffer containing profile
    DWORD   cbDataSize;         // size of profile data
};

struct MILICCProfileHeader
{
    DWORD   phSize;             // profile size in bytes
    DWORD   phCMMType;          // CMM for this profile
    DWORD   phVersion;          // profile format version number
    DWORD   phClass;            // type of profile
    DWORD   phDataColorSpace;   // color space of data
    DWORD   phConnectionSpace;  // PCS
    DWORD   phDateTime[3];      // date profile was created
    DWORD   phSignature;        // magic number
    DWORD   phPlatform;         // primary platform
    DWORD   phProfileFlags;     // various bit settings
    DWORD   phManufacturer;     // device manufacturer
    DWORD   phModel;            // device model number
    DWORD   phAttributes[2];    // device attributes
    DWORD   phRenderingIntent;  // rendering intent
    CIEXYZ  phIlluminant;       // profile illuminant
    DWORD   phCreator;          // profile creator
    BYTE    phReserved[44];     // reserved for future use
};

/*=========================================================================*\
    Vertex types
\*=========================================================================*/

/* Type definition and flags for MIL Vertex Formats */

// These are different from D3DFVF flags because we use different types for
// color data.

typedef DWORD MILVertexFormat;

enum MILVertexFormatAttribute
{
    MILVFAttrNone        = 0x0,
    MILVFAttrXY          = 0x1,
    MILVFAttrXYZ         = 0x3,
    MILVFAttrNormal      = 0x4,
    MILVFAttrDiffuse     = 0x8,
    MILVFAttrSpecular    = 0x10,
    MILVFAttrUV1         = 0x100,
    MILVFAttrUV2         = 0x300,
    MILVFAttrUV3         = 0x700,
    MILVFAttrUV4         = 0xf00,
    MILVFAttrUV5         = 0x1f00,
    MILVFAttrUV6         = 0x3f00,
    MILVFAttrUV7         = 0x7f00,
    MILVFAttrUV8         = 0xff00,

    MILVERTEXFORMATATTRIBUTE_FORCE_DWORD = MIL_FORCE_DWORD
};


struct MILVertexXYD
{
    MILPointF Pos;
    MILColorF Diffuse;

    enum {Format = MILVFAttrXY | MILVFAttrDiffuse};
};

struct MILVertexXYUV
{
    MILPointF Pos;
    MILPointF Tex;

    enum {Format = MILVFAttrXY | MILVFAttrUV1};
};

struct MILVertexXYDUV
{
    MILPointF Pos;
    MILColorF Diffuse;
    MILPointF Tex;

    enum {Format = MILVFAttrXY | MILVFAttrDiffuse | MILVFAttrUV1};
};

struct MILVertexXYZNDSUV4
{
    FLOAT X;
    FLOAT Y;
    FLOAT Z;
    FLOAT Nx;
    FLOAT Ny;
    FLOAT Nz;
    MILColorF Diffuse;
    MILColorF Specular;
    MILPointF Tex1;
    MILPointF Tex2;
    MILPointF Tex3;
    MILPointF Tex4;

    enum {Format = MILVFAttrXYZ | MILVFAttrNormal | MILVFAttrDiffuse | MILVFAttrSpecular | MILVFAttrUV4};
};

/*=========================================================================*\
    Shader Types
\*=========================================================================*/

typedef UINT MILSPHandle;

#define MILSP_INVALID_HANDLE 0xffffffff

/*=========================================================================*\
    MILFillMode
\*=========================================================================*/

enum MILFillMode
{
    MILFillModeAlternate = 0,
    MILFillModeWinding   = 1,
    MILFILLMODE_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILUnit constants
\*=========================================================================*/

enum MILUnit
{
    MILUnitPixel        = 2, /* Each unit is one device pixel. */
    MILUnitPoint        = 3, /* Each unit is a printer's point, or 1/72 inch. */
    MILUnitInch         = 4, /* Each unit is 1 inch. */
    MILUnitDocument     = 5, /* Each unit is 1/300 inch. */
    MILUnitMillimeter   = 6, /* Each unit is 1 millimeter. */
    MILUnitMetafile     = 7, /* Each unit is 1/100 millimeter */
    MILUnitWVG          = 8, /* Each unit is 1/96 inch */
    MILUNIT_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    Various wrap modes for brushes
\*=========================================================================*/

enum MILGradientWrapMode
{
    MILGradientWrapModeExtend  = 0,
    MILGradientWrapModeFlip    = 1,
    MILGradientWrapModeTile    = 2,
    MILGRADIENTWRAPMODE_FORCE_DWORD = MIL_FORCE_DWORD
};


/*=========================================================================*\
    MILLineShape
\*=========================================================================*/

enum MILLineShape
{
    MILLineShapeNone       = 0,
    MILLineShapeSquare     = 1,
    MILLineShapeRound      = 2,
    MILLineShapeDiamond    = 3,
    MILLineShapeArrow      = 4,
    MILLineShapeCustom     = 5,
    MILLINESHAPE_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILDashStyle
\*=========================================================================*/

enum MILDashStyle
{
    MILDashStyleSolid      = 0,
    MILDashStyleDash       = 1,
    MILDashStyleDot        = 2,
    MILDashStyleDashDot    = 3,
    MILDashStyleDashDotDot = 4,
    MILDashStyleCustom     = 5,
    MILDASHSTYLE_FORCE_DWORD = MIL_FORCE_DWORD
};


#ifdef COMPOUND_PEN_IMPLEMENTED
/*=========================================================================*\
    MILCompoundStyle
\*=========================================================================*/

enum MILCompoundStyle
{
    MILCompoundStyleSingle = 0,
    MILCompoundStyleDouble = 1,
    MILCompoundStyleTriple = 2,
    MILCompoundStyleCustom = 3,
    MILCOMPOUNDSTYLE_FORCE_DWORD = MIL_FORCE_DWORD
};
#endif // COMPOUND_PEN_IMPLEMENTED

/*=========================================================================*\
    MILLineJoin
\*=========================================================================*/

enum MILLineJoin
{
    MILLineJoinMiter        = 0,
    MILLineJoinBevel        = 1,
    MILLineJoinRound        = 2,
    MILLineJoinMiterClipped = 3,
    MILLINEJOIN_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILClipCombineMode
\*=========================================================================*/

enum MILClipCombineMode
{
    MILClipCombineModeAnd          = 0,
    // We may want to add an exclude combine mode later
    MILCLIPCOMBINEMODE_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILPathsRelation
\*=========================================================================*/

enum MILPathsRelation
{
    MILPathsUnknown = 0,
    MILPathsDisjoint = 1,
    MILPathsIsContained = 2,
    MILPathsContains = 3,
    MILPathsOverlap = 4,
    MILPATHSRELATION_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILCompositingMode
\*=========================================================================*/

enum MILCompositingMode
{
    MILCompositingModeSourceOver = 0,
    MILCompositingModeSourceCopy = 1,
    MILCompositingModeSourceAdd  = 2,
    MILCompositingModeDestCopy   = 3,
    MILCompositingModeSourceInverseAlphaMultiply = 4,
    MILCompositingModeSourceUnder                = 5,
    MILCOMPOSITINGMODE_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILCompositingQuality
\*=========================================================================*/

enum MILCompositingQuality
{
    MILCompositingQualityInvalid          = 0,
    MILCompositingQualityDefault          = 1,
    MILCompositingQualityHighSpeed        = 2,
    MILCompositingQualityHighQuality      = 3,
    MILCompositingQualityGammaCorrected   = 4,
    MILCompositingQualityAssumeLinear     = 5,
    MILCOMPOSITINGQUALITY_FORCE_DWORD = MIL_FORCE_DWORD
};

/*=========================================================================*\
    MILAntiAliasMode
\*=========================================================================*/

enum MILAntiAliasMode
{
    MILAntiAliasModeNone    = 0,
    MILAntiAliasMode8x8     = 1,
    MILANTIALIASMODE_FORCE_DWORD = MIL_FORCE_DWORD
};




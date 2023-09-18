// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//+--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//     Header for the CPartition class
//
//----------------------------------------------------------------------------

#pragma once

//+--------------------------------------------------------------------------
//
//  Abstract:
//     CShard is a helper class for CPartition.
//
//----------------------------------------------------------------------------
struct CShard
{
    double dX, dY;    //initial location of the shard (normalized from 0 to 1)
    int fixX, fixY;    //initial location of the shard (in fixed-point space)
    int iPixelOffsetX, iPixelOffsetY;    //x,y offsets for pixels that belong to this shard
};

//+--------------------------------------------------------------------------
//
//  Abstract:
//     CPartition provides means to forward-transform and inverse-transform
//                points in pixel space. The forward-transform creates the
//                explosion effect when applied to all pixels of an image.
//
//  Notes:
//      In order to forward transform and inverse transform points that belong
//        to a source bitmap, we need to know the width and height of the source
//        bitmap. The *best-guess* as to this value is to use the width/height from
//        the last call to bitmapeffect::GetOutput.
//
//        This means that the bitmap-effect must call SetBounds on CPartition
//        before it can inverse or forward transform points. To maintain a valid
//        object state, we assume that the width/height of the source is '1'/'1',
//        and that the radius is '1.0' at the time of object creation. This changes
//        after the first call to 'SetBounds', which should occur the first time
//        the explode effect calls 'GetOutput'.
//
//
//----------------------------------------------------------------------------
class CPartition
{

public:
    static const ULONG NUM_SHARDS = 10;
    static const ULONG DEFAULT_SEED = 0x29A;

public:

    ULONG GetSourceWidth();
    ULONG GetSourceHeight();
    ULONG GetDestWidth();
    ULONG GetDestHeight();
    double CalcDestXOffset_Points( double dpiX );
    double CalcDestYOffset_Points( double dpiY );

    CPartition();
    virtual ~CPartition();

    void SetSeed( ULONG uiSeed );

    HRESULT SetBounds( double dRadius, ULONG uiSrcWidth, ULONG uiSrcHeight, 
                                        ULONG *puiDstWidth, ULONG *puiDstHeight);

    HRESULT Transform( ULONG uiSrcX, ULONG uiSrcY, ULONG *puiDstX, ULONG *puiDestY );

    BOOL InverseTransform( ULONG uiDstX, ULONG uiDstY, ULONG *puiSrcX, ULONG *puiSrcY );

protected:
    ULONG Lookup( ULONG uiSrcX, ULONG uiSrcY );
    void SetBounds_Internal();

protected:
    CNoise m_clsNoise;
    CShard m_shards[ NUM_SHARDS ];
    ULONG m_uiSrcWidth, m_uiDstWidth, m_uiSrcHeight, m_uiDstHeight;
    int m_fixWidthRatio, m_fixHeightRatio;
    double m_dRadius;
};




//+--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//     Implementation of the CNoise class
// 
//  Notes:
//
//
//----------------------------------------------------------------------------

#include "stdafx.h"
#include "Fixed.h"
#include "Noise.h"

//+---------------------------------------------------------------------------
//
//  Member:
//      CNoise::DiscreteSample
//
//  Synopsis:
//      Returns a fixed-point value, over the range -1 to 1.
//        This value is hashed to by the lower 16-bits of uiX, 
//        the lower 16-bits of uiY, and the lower 8-bits of uiChoose.
//
//  Returns:
//      The value of the sample, in fixed-point format (defined in Fixed.h).
//
//----------------------------------------------------------------------------
int CNoise::DiscreteSample(ULONG uiX, ULONG uiY, ULONG uiChoose)
{
    ULONG uiHash0 = uiX & 0xFF;                ULONG uiHash1 = uiY & 0xFF;
    ULONG uiHash2 = (uiX>>8) & 0xFF;        ULONG uiHash3 = (uiY>>8) & 0xFF;
    ULONG uiHash4 = uiChoose & 0xFF;

    uiHash0 = hash[0][uiHash0];
    uiHash1 = hash[1][uiHash1];
    uiHash2 = hash[2][uiHash2];
    uiHash3 = hash[3][uiHash3];
    uiHash4 = hash[4][uiHash4];

    return lookup[ (uiHash0 ^ uiHash1 ^ uiHash2 ^ uiHash3 ^ uiHash4) & LOOKUP_MASK ];
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CNoise::SetSeed
//
//  Synopsis:
//      Initializes internal random numbers. Each seed value corresponds
//        to a unique set of return vaules for SmoothSample. This function
//        should be called before any calls to 'SmoothSample'.
//
//----------------------------------------------------------------------------
void CNoise::SetSeed(ULONG uiSeed)
{
    srand( uiSeed );
    for(int i=0;i<INDICES;i++)
    {
        for(int j=0;j<5;j++)
            hash[j][i] = rand() & LOOKUP_MASK;

        lookup[i] = CFixed::NextRandomFixed_Normalized();
    }
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CNoise::SmoothSample
//
//    Inputs:
//        fixX, fixY must be in fixed-point format.
//
//  Synopsis:
//        Output is smooth 2-d noise:
//        Vaules are equivalent to DiscreteNoise at integer values,
//        and are interpolated for in-between points.
//
//        Each lower 8-bit value of uiChoose (from 0 to 255) maps to a unique
//        SmoothSample function.
//
//  Returns:
//      The value of the sample, in fixed-point format.
//
//----------------------------------------------------------------------------
int CNoise::SmoothSample(ULONG fixX, ULONG fixY, ULONG uiChoose)
{
    ULONG uiX = fixX>>CFixed::FIX_SHIFT,
          uiY = fixY>>CFixed::FIX_SHIFT;

    ULONG uiX2 = uiX + 1,
          uiY2 = uiY + 1;

    int    iDecx = (int)(fixX & CFixed::FIX_MASK),
        iDecy = (int)(fixY & CFixed::FIX_MASK);

    int iA = DiscreteSample( uiX, uiY, uiChoose),
        iB = DiscreteSample( uiX2, uiY, uiChoose),
        iC = DiscreteSample( uiX, uiY2, uiChoose),
        iD = DiscreteSample( uiX2, uiY2, uiChoose );
    
    int iLeft = (iA<<CFixed::FIX_SHIFT) + (iC-iA)*iDecy;
    int iRight = (iB<<CFixed::FIX_SHIFT) + (iD-iB)*iDecy;

    int iResult = iLeft + ((iRight-iLeft)>>CFixed::FIX_SHIFT)*iDecx;

    /*
        This code produces an identical result to the above. However,
        the above version can be used for future optimizations
            (since iLeft and iRight are dependent on Y instead
            of X, which means more coherenecy between pixels on the same
            row)
    int iTop = (iA<<FIX_SHIFT) + (iB-iA)*iDecx;
    int iBottom = (iC<<FIX_SHIFT) + (iD-iC)*iDecx;

    int iResult = iTop + ((iBottom-iTop)>>FIX_SHIFT)*iDecy;
    */

    iResult>>=CFixed::FIX_SHIFT;

    return iResult;
}

CNoise::CNoise(void)
{
}

CNoise::~CNoise(void)
{
}
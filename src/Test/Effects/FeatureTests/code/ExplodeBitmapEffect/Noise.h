//+--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//     Header for the CNoise class
//
//----------------------------------------------------------------------------

#pragma once


//+--------------------------------------------------------------------------
//
//  Abstract:
//     CNoise provides a smooth two-dimensional noise function.
// 
//----------------------------------------------------------------------------

class CNoise
{

protected:
    static const ULONG INDICES = 256;
    static const ULONG LOOKUP_MASK = 0xFF;

protected:
    int lookup[ INDICES ], hash[5][INDICES];

    int DiscreteSample(ULONG ix, ULONG iy, ULONG choose);

public:
    CNoise(void);
    virtual ~CNoise(void);
    
    void SetSeed(ULONG uiSeed);

    int SmoothSample(ULONG fixX, ULONG fixY, ULONG choose);
};

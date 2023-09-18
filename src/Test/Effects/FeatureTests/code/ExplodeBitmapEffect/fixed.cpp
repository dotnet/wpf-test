// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "stdafx.h"
#include "Fixed.h"


//+---------------------------------------------------------------------------
//
//  Member:
//      CFxied::DoubleToFixed
//
//  Returns:
//      Fixed-point representation of 'dValue'.
//
int CFixed::DoubleToFixed( double dValue )
{
    return static_cast<int>(dValue * (double)( 1 << FIX_SHIFT ) );
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CFxied::NextRandomFixed_Normalized
//
//  Returns:
//      Fixed-point number from -1.0 to 1.0
//
//    Notes:
//        Uses rand() function, thus value
//        depends on this function's internal state
//
//    Implementation details:
//        We compute number over the range 2^10 rather
//        than 2^FIX_SHIFT to maintain compatibility
//        with the old version of explode (this way
//        we're producing the same sequence of random
//        numbers as the old version, which used 10-bits
//        for the decimal part)
//
int CFixed::NextRandomFixed_Normalized()
{
    int x;

    x = (rand() & 0x3FF*2) - 0x3FF;
    x<<=FIX_SHIFT - 10;

    return x;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CFxied::DoubleToFixed
//
//  Returns:
//      Fixed-point number from 0.0 to 1.0
//
//    Implementation details:
//        See NextRandomFixed_Normalized
//
int CFixed::NextRandomFixed_Positive_Normalized()
{
    int x;

    x = rand() & 0x3FF;
    x<<=FIX_SHIFT - 10;

    return x;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CFxied::DoubleToFixed
//
//  Returns:
//      Double float point representation of the fixed point value 'fixX'
//
double CFixed::FixedToDouble( int fixX )
{
    return (double)fixX / (double)(FIX_ONE);
}

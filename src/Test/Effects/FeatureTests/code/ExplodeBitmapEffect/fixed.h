#pragma once


//+--------------------------------------------------------------------------
//
//  Abstract:
//     CFixed defines our fixed point format. (10 decimal bits, 1 sign bit, 21 whole part)
// 
//
//    Notes:
//        A fix_shift of 14 is the highest amount of fixed-decimal space
//        we can have without causing overflows in our calculations.
//        (2^14 ==> 16,384 pixels in a normalized space)
//        I.E. the explode effect allows 'explosion details' to emerge pixel-perfect
//        over an image that is about 13 times the size of a normal-resolution monitor.
//        Anything higher than this will snape to the nearest pixel, but
//        the image will not look any different.
//
//
//
//----------------------------------------------------------------------------
struct CFixed
{
    static const int FIX_SHIFT = 14;    //the number of decimal bits
    static const int FIX_MASK = ((1<<FIX_SHIFT)-1);    //mask for just the decimal part
    static const int FIX_ONE = (FIX_MASK+1);    //the value one in our fixed point format

    static int DoubleToFixed( double dX );

    static int NextRandomFixed_Normalized();

    static int NextRandomFixed_Positive_Normalized();

    static double FixedToDouble( int fixX );
};


//+--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//     Implementation of the CPartition class
// 
//  Notes:
//
//
//----------------------------------------------------------------------------

#include "stdafx.h"
#include "Fixed.h"
#include "Noise.h"
#include "Partition.h"
//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::InverseTransform
//
//  Synopsis:
//      Inverse-transforms point from the destination image back to the source image.
//        This is done in pixel-space, and all calculations are based on the values
//        from the last call to 'SetBounds'.
//
//  Returns:
//      FALSE if puiSrcX or puiSrcY are null
//        FALSE if uiDstX or uiDstY do not have a known inverse
//        TRUE otherwise
//
//----------------------------------------------------------------------------
BOOL CPartition::InverseTransform( ULONG uiDstX, ULONG uiDstY, ULONG *puiSrcX, ULONG *puiSrcY )
{
    assert( puiSrcX);
    assert( puiSrcY);

    if( (NULL == puiSrcX) || (NULL == puiSrcY) )    return FALSE;

    int iTestX, iTestY;
    ULONG uiTestShard;
    *puiSrcX = 0;
    *puiSrcY = 0;
    BOOL result = FALSE;

    for(ULONG i=0;i<NUM_SHARDS;i++)
    {
        iTestX = (int)uiDstX - m_shards[ i ].iPixelOffsetX;
        iTestY = (int)uiDstY - m_shards[ i ].iPixelOffsetY;

        //do bounds checking
        if( (iTestX <0) || (iTestY<0) )    continue;
        if( (iTestX >= (int)m_uiSrcWidth) || (iTestY >= (int)m_uiSrcHeight) ) continue;

        //make sure that the test position maps to the test shard
        uiTestShard = Lookup( (ULONG)iTestX, (ULONG)iTestY );

        if( uiTestShard == i )    {
            //finally, take the shard that's in the farthest
            //bottom right (since that establishes the z-order)
            if( ((ULONG)iTestY >= *puiSrcY ) ||
                (((ULONG)iTestX >= *puiSrcX ) && ((ULONG)iTestY >= *puiSrcY )) )
            {
                *puiSrcX = (ULONG)iTestX;
                *puiSrcY = (ULONG)iTestY;
                result = TRUE;
            }
        }
    }
    return result;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::CPartition
//
//  Synopsis:
//      Creates a fresh partition object.
//
//    Notes:
//        At creation time, we do not know the input/output width and height,
//        so we assume that they're of size '1' (the minimum valid size) --> this
//        is just so that 'CPartition' maintains a valid state, and doesn't fail
//        on calls to transform/inverse-transform. These values are corrected
//        on the first call to SetBounds, which is called by the exposion
//        effect during GetOutput
//
//
//----------------------------------------------------------------------------
CPartition::CPartition()
{
    m_uiSrcWidth = 1;
    m_uiSrcHeight = 1;
    m_uiDstWidth = 1;
    m_uiDstHeight = 1;
    m_dRadius = 1.0;
    SetSeed( DEFAULT_SEED );
}


//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::SetSeed
//
//  Synopsis:
//      Changes the seed number for the random number generator.
//        Each seed number maps to a unique explosion effect.
//
//
//----------------------------------------------------------------------------
void CPartition::SetSeed( ULONG uiSeed )
{
    m_clsNoise.SetSeed( uiSeed );
    for(int i=0;i<NUM_SHARDS;i++)
    {
        m_shards[i].fixX = CFixed::NextRandomFixed_Positive_Normalized();
        m_shards[i].fixY = CFixed::NextRandomFixed_Positive_Normalized();

        m_shards[i].dX = CFixed::FixedToDouble( m_shards[i].fixX );
        m_shards[i].dY = CFixed::FixedToDouble( m_shards[i].fixY );
    }

    SetBounds_Internal();    //avoid invalidating CPartition's state
}

CPartition::~CPartition()
{
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::CalcDestXOffset_Points
//
//  Return:
//        X Offset for the destination bitmap, in terms
//        of resolution-independent points (rather than pixels)
//
//
//----------------------------------------------------------------------------
double CPartition::CalcDestXOffset_Points( double dpiX )
{
    double xOffset_pixels = (double)( GetDestWidth() - GetSourceWidth() ) / 2.0;

    return xOffset_pixels * 96.0 / dpiX;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::CalcDestYOffset_Points
//
//  Return:
//        Y Offset for the destination bitmap, in terms
//        of resolution-independent points (rather than pixels)
//
//
//----------------------------------------------------------------------------
double CPartition::CalcDestYOffset_Points( double dpiY )
{
    double yOffset_pixels = (double)( GetDestHeight() - GetSourceHeight() ) / 2.0;

    return yOffset_pixels * 96.0 / dpiY;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::GetSourceWidth
//
//  Return:
//        Width of the source image, as specified by the last call to SetBounds
//
//
//----------------------------------------------------------------------------
ULONG CPartition::GetSourceWidth()
{
    return m_uiSrcWidth;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::GetSourceHeight
//
//  Return:
//        Height of the source image, as specified by the last call to SetBounds
//
//
//----------------------------------------------------------------------------
ULONG CPartition::GetSourceHeight()
{
    return m_uiSrcHeight;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::GetDestWidth
//
//  Return:
//        Width of the destination image, as specified by the last call to SetBounds
//
//
//----------------------------------------------------------------------------
ULONG CPartition::GetDestWidth()
{
    return m_uiDstWidth;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::GetDestHeight
//
//  Return:
//        Height of the destination image, as specified by the last call to SetBounds
//
//
//----------------------------------------------------------------------------
ULONG CPartition::GetDestHeight()
{
    return m_uiDstHeight;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::SetBounds_Internal
//
//  Synopsis:
//        Calls 'SetBounds' with the last specified source image width/height.
//        This causes SetBounds to re-compute internal variables. It should
//        be called internally if the seed number changes.
//
//
//----------------------------------------------------------------------------
void CPartition::SetBounds_Internal()
{
    ULONG uiDummy1, uiDummy2;
    SetBounds( m_dRadius, m_uiSrcWidth, m_uiSrcHeight, &uiDummy1, &uiDummy2 );
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::SetBounds
//
//  Synopsis:
//      Computes the destination image width and height based on the explosion radius.
//
//  Returns:
//      E_INVALIDARG if dRadius < 1.0, uiSrcWidth is 0, uiSrcHeiht is 0,
//                    puiDstWidth is null, or puiDstHeight is null
//
//----------------------------------------------------------------------------
HRESULT CPartition::SetBounds( double dRadius,
                           ULONG uiSrcWidth, ULONG uiSrcHeight, 
                           ULONG *puiDstWidth, ULONG *puiDstHeight )
{
    assert( dRadius >= 1.0);
    assert( puiDstWidth);
    assert( puiDstHeight);
    assert( uiSrcWidth > 0);
    assert( uiSrcHeight > 0);

    if( (dRadius < 1.0) || (NULL==puiDstWidth) || (NULL==puiDstHeight) || 
        (uiSrcWidth==0) || (uiSrcHeight==0) )
        return(E_INVALIDARG);

    //calc output width and height
    *puiDstWidth = (ULONG)((double)uiSrcWidth * dRadius);
    *puiDstHeight = (ULONG)((double)uiSrcHeight * dRadius);

    //set member variables
    m_dRadius = dRadius;
    m_uiSrcWidth = uiSrcWidth;
    m_uiDstWidth = *puiDstWidth;
    m_uiSrcHeight = uiSrcHeight;
    m_uiDstHeight = *puiDstHeight;
    //int fixRadius  = CFixed::DoubleToFixed( dRadius );
    m_fixWidthRatio =  CFixed::FIX_ONE / uiSrcWidth;
    m_fixHeightRatio = CFixed::FIX_ONE / uiSrcHeight;

    //now, modify the pixel offsets for each shard
    double dOffsetX, dOffsetY, dSizeInc = dRadius - 1.0;
    int iImageOffsetX = (*puiDstWidth - uiSrcWidth) / 2;
    int iImageOffsetY = (*puiDstHeight - uiSrcHeight) / 2;
    for(ULONG i=0;i<NUM_SHARDS;i++)
    {
        dOffsetX = (m_shards[i].dX - 0.5) * dSizeInc;
        dOffsetY = (m_shards[i].dY - 0.5) * dSizeInc;

        /*
        Might want to adjust the speed of each shard here...
        Perhaps involve acceleration as well.
        This is where we get realistic animation.
        */

        m_shards[i].iPixelOffsetX = (int)( dOffsetX * ((double)uiSrcWidth) );
        m_shards[i].iPixelOffsetY = (int)( dOffsetY * ((double)uiSrcHeight) );

        m_shards[i].iPixelOffsetX+=iImageOffsetX;
        m_shards[i].iPixelOffsetY+=iImageOffsetY;
    }

    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::Lookup
//
//  Synopsis:
//      Finds the shard that the input point belongs to.
//        The input point is in the pixel space of the source image.
//
//  Returns:
//      The index of the shard 
//
//    Perf: Approximately 40 adds and 20 mults per lookup, and less than
//                one noise computation on average
//
//----------------------------------------------------------------------------

ULONG CPartition::Lookup( ULONG uiSrcX, ULONG uiSrcY )
{
    int fixX = (int)uiSrcX * m_fixWidthRatio,
        fixY = (int)uiSrcY * m_fixHeightRatio;

    int fixWinnerDst;    ULONG uiShardWinnerIdx = 0;
    BOOL bWinnerComputeNoise = FALSE;
    int fixDx, fixDy, fixTestDst, fixNoise;

    fixDx = m_shards[0].fixX - fixX;
    fixDy = m_shards[0].fixY - fixY;

    fixWinnerDst = fixDx*fixDx + fixDy*fixDy;

    for(ULONG uiShardTestIdx=1;uiShardTestIdx<NUM_SHARDS;uiShardTestIdx++)
    {
        fixDx = m_shards[ uiShardTestIdx ].fixX - fixX;
        fixDy = m_shards[ uiShardTestIdx ].fixY - fixY;

        fixTestDst = fixDx*fixDx + fixDy*fixDy;

        //NOISE_CUTOFF value is semi-arbitrary
        //FIX_ONE squared minus four, technically, is the largest possible
        //value for fixNoise
        //however, set the cutoff lower to filter out high amp noise
        //
        const static int NOISE_CUTOFF = CFixed::FIX_ONE*CFixed::FIX_ONE - 4 - 2;

        if( !bWinnerComputeNoise )
        {
            //case 1: noise is not computed for winner or test
            //          see if winner wins regardless of actual noise values
            if( (fixTestDst+NOISE_CUTOFF) < (fixWinnerDst-NOISE_CUTOFF) )
            {
                fixWinnerDst = fixTestDst;
                uiShardWinnerIdx = uiShardTestIdx;
                bWinnerComputeNoise = FALSE;
                continue;
            }
            //case 2: noise is not computed for winner or test
            //          see if test shard cannot win
            else if( (fixTestDst-NOISE_CUTOFF) > (fixWinnerDst+NOISE_CUTOFF) )
            {
                continue;
            }
            //case 3: must compute noise for winner and drop down to
            //          next set of test cases
            else
            {
                fixNoise = m_clsNoise.SmoothSample( fixX<<4, fixY<<4, uiShardTestIdx );

                fixNoise>>=2;

                if( fixNoise < 0 )    fixNoise = (-(fixNoise*fixNoise));
                else                fixNoise = (fixNoise*fixNoise);

                fixWinnerDst+=fixNoise;
                bWinnerComputeNoise = TRUE;
            }
        }
        
        if( bWinnerComputeNoise )
        {
            //case 4: see if test shard must win
            if( (fixTestDst+NOISE_CUTOFF) < fixWinnerDst )
            {
                fixWinnerDst = fixTestDst;
                uiShardWinnerIdx = uiShardTestIdx;
                bWinnerComputeNoise = FALSE;
                continue;
            }
            //case 5: see if test shard cannot win
            else if( (fixTestDst-NOISE_CUTOFF) > fixWinnerDst )
            {
                continue;
            }
            //case 6: unsure, must compute noise value for test shard
            else
            {
                fixNoise = m_clsNoise.SmoothSample( fixX<<4, fixY<<4, uiShardTestIdx );

                fixNoise>>=2;

                if( fixNoise < 0 )    fixNoise = (-(fixNoise*fixNoise));
                else                fixNoise = (fixNoise*fixNoise);

                //now, we should have the same value..., in theory...
                fixTestDst+=fixNoise;
                
                if( fixTestDst < fixWinnerDst )
                {
                    fixWinnerDst = fixTestDst;
                    uiShardWinnerIdx = uiShardTestIdx;
                    bWinnerComputeNoise = TRUE;
                    continue;
                }
            }
        }

    }//for
    return uiShardWinnerIdx;
}


//+---------------------------------------------------------------------------
//
//  Member:
//      CPartition::Transform
//
//  Synopsis:
//      Transforms point from the source image to the destination image.
//        This is done in pixel-space, and all calculations are based on the values
//        from the last call to 'SetBounds'.
//
//  Returns:
//      E_INVALIDARG if puiDstX or puiDstY are null
//
//----------------------------------------------------------------------------
HRESULT CPartition::Transform( ULONG uiSrcX, ULONG uiSrcY, ULONG *puiDstX, ULONG *puiDstY )
{
    assert( puiDstX);
    assert( puiDstY);

    if( (NULL == puiDstX) || (NULL == puiDstY) )    return(E_INVALIDARG);

    ULONG uiShardIdx = Lookup( uiSrcX, uiSrcY );
    
    *puiDstX = m_shards[ uiShardIdx ].iPixelOffsetX + uiSrcX;
    *puiDstY = m_shards[ uiShardIdx ].iPixelOffsetY + uiSrcY;

    return S_OK;
}







//UN-OPTIMZED VERSION OF LOOKUP
//KEEP FOR REFERENCE
/*
ULONG CPartition::Lookup( ULONG uiSrcX, ULONG uiSrcY )
{
    int fixX = (int)uiSrcX * m_fixWidthRatio,
        fixY = (int)uiSrcY * m_fixHeightRatio;

    int fixSmallDist = 0x7FFFFFFF;    ULONG uiShardIdx = 0;
    int fixDx, fixDy, fixDistSqr, fixNoise;

    for(ULONG i=0;i<NUM_SHARDS;i++)
    {
        fixDx = m_shards[i].fixX - fixX;
        fixDy = m_shards[i].fixY - fixY;
        fixDistSqr = fixDx*fixDx + fixDy*fixDy;

        fixNoise = m_clsNoise.SmoothSample( fixX<<4, fixY<<4, i )>>2;

        if( fixNoise < 0 )    fixNoise = (-(fixNoise*fixNoise));
        else                fixNoise = (fixNoise*fixNoise);

        fixDistSqr+= fixNoise;

        if( fixDistSqr < fixSmallDist )
        {
            fixSmallDist = fixDistSqr;
            uiShardIdx = i;
        }

    }
    return uiShardIdx;
}
*/
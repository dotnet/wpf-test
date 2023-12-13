// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#include "stdafx.h"
#include "Explode.h"
#include "ImageAccessor.h"

const WCHAR *c_pwcsRadius = L"Radius";
const WCHAR *c_pwcsSeedNumber = L"SeedNumber";

const ULONG c_ulNumberProperties = 2; // Number of properties this effect exposes

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::CExplode
//
//  Synopsis:
//      Ctor
//
//----------------------------------------------------------------------------
CExplode::CExplode() :         m_dRadius(1.5),
        m_uiSeedNumber( CPartition::DEFAULT_SEED )

{
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::~CExplode
//
//  Synopsis:
//      dtor
//
//----------------------------------------------------------------------------
CExplode::~CExplode()
{
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::FinalConstruct
//
//  Synopsis:
//
//----------------------------------------------------------------------------
HRESULT CExplode::FinalConstruct()
{
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetOutput
//
//  Synopsis:
//      This method is what does the actual pixel processing for the effect.
//      The caller will be requesting the output for some output pin on
//      the effect (uiIndex, which is normally == 0 for an effect with 1 output).
//      The passed in render context contains information about how the effect
//      should render (see interface for details). pfModifyInPlace will indicate
//      whether the effect should attempt to modify the input image in place.
//      If this is VARIANT_TRUE then you can just write the output pixels over
//      the input image and return the input image. If your effect can't operate
//      in-place then return VARIANT_FALSE through this parameter to indicate
//      that a new image was created.
//
// Arguments:
//      uiIndex - zero-based index of output pin whose output is desired
//      pRenderContext - the render context to use
//      pfModifyInPlace - On input, indicates whether the effect should operate
//          in place. On output, indicates whether the effect did the operation
//          in place.
//      ppBitmapSource - returns the output from the effect.
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetOutput(ULONG uiIndex,
                                      IMILBitmapEffectRenderContext *pRenderContext,
                                      VARIANT_BOOL *pfModifyInPlace,
                                      IWICBitmapSource **ppBitmapSource)
{
      assert(pRenderContext);
    assert(ppBitmapSource);
    if (pRenderContext == NULL || ppBitmapSource == NULL || pfModifyInPlace==NULL)
    {
        return(E_INVALIDARG);
    }
    *pfModifyInPlace = VARIANT_FALSE;
    assert(uiIndex == 0);
    if (uiIndex != 0)
    {
        return(E_INVALIDARG);
    }
    
        //**********************************************************************
    // Ask the aggregator to get our input source
    //**********************************************************************
    HRESULT hr = S_OK;
    
    CComPtr<IMILBitmapEffect> spOuterEffect;
    if (SUCCEEDED(hr))
    {
        hr = QueryInterface(__uuidof(IMILBitmapEffect), reinterpret_cast<void**>(&spOuterEffect));
    }
    
    CComPtr<IMILBitmapEffectImpl> spOuterEffectImpl;
    if (SUCCEEDED(hr))
    {
        hr = QueryInterface(__uuidof(IMILBitmapEffectImpl), reinterpret_cast<void**>(&spOuterEffectImpl));
    }


    CComPtr<IWICBitmapSource> spBitmap;
    VARIANT_BOOL vbModifyInPlace = *pfModifyInPlace;
    hr = spOuterEffectImpl->GetInputBitmapSource(0, pRenderContext, &vbModifyInPlace, &spBitmap);
    if (FAILED(hr))
    {
        return(hr);
    }

        //**********************************************************************
    // Create our imaging factory
    //**********************************************************************
    CComPtr<IWICImagingFactory> spImageFactory;
    if (SUCCEEDED(hr))
    {
        hr = CoCreateInstance(CLSID_WICImagingFactory, NULL, CLSCTX_INPROC_SERVER, __uuidof(IWICImagingFactory),
                              reinterpret_cast<void**>(&spImageFactory));
    }


    if( FAILED( hr ) )    return(hr);

    CComPtr<IWICBitmap> spSrcBitmap;
    hr=spBitmap->QueryInterface(__uuidof(IWICBitmap), reinterpret_cast<void**>(&spSrcBitmap));
    if (FAILED(hr))
    {
        if (hr != E_NOINTERFACE)
        {
            return(hr);
        }
        hr=spImageFactory->CreateBitmapFromSource(spBitmap, WICBitmapNoCache, &spSrcBitmap);
        if (FAILED(hr))
        {
            return(hr);
        }
    }

    // Create our output image.
    UINT uiSrcWidth = 0;
    UINT uiSrcHeight = 0;
    if (SUCCEEDED(hr))
    {
        hr=spSrcBitmap->GetSize(&uiSrcWidth, &uiSrcHeight);
    }
 
    //change uiWidth and uiHeight appropriately for the output format
    WICPixelFormatGUID pixelFormat = GUID_WICPixelFormatUndefined;
    if (SUCCEEDED(hr))
    {
        hr=spSrcBitmap->GetPixelFormat(&pixelFormat);
    }

    //calculate destination width and height
    ULONG uiDstWidth,
          uiDstHeight;

    hr= m_clsPartition.SetBounds( m_dRadius, uiSrcWidth, uiSrcHeight, &uiDstWidth, &uiDstHeight ) ;
    if( FAILED( hr ) )    return( hr );

    //ensure that partition calculated bounds properly
    assert( uiDstWidth >= uiSrcWidth );
    assert( uiDstHeight >= uiSrcHeight );

    //create the destination bitmap
    CComPtr<IWICBitmap> spDstBitmap;
    
    hr=spImageFactory->CreateBitmap(uiDstWidth, uiDstHeight, pixelFormat, WICBitmapCacheOnLoad, &spDstBitmap);
    if (FAILED(hr))
    {
        return(hr);
    }

    assert( GUID_WICPixelFormat32bppBGRA == pixelFormat );

    if( GUID_WICPixelFormat32bppBGRA != pixelFormat)
        return( E_INVALIDARG );

    /*
    assert( GUID_WICPixelFormat32bppBGRA == pixelFormat );

    if( GUID_WICPixelFormat32bppBGRA != pixelFormat)
        return( E_INVALIDARG );

    IWICBitmapLock *pSrcLock, *pDstLock;
    MILRect srcRect = {0,0,uiSrcWidth,uiSrcHeight};
    MILRect dstRect = {0,0,uiDstWidth,uiDstHeight};
    hr= spSrcBitmap->Lock( &srcRect, WICBitmapLockRead, &pSrcLock ) ;
    if( FAILED( hr ) )    return( hr );
    hr= spDstBitmap->Lock( &dstRect, WICBitmapLockWrite, &pDstLock ) ;
    if( FAILED( hr ) )
    {
        pSrcBitmap->Unlock();
        return( hr );
    }

    hr= pSrcLock->GetDataPointer( &uiSrcBufferSize, &pbSrcData ) ;
    */
        


    CPixelIterator_32bppBGRA_Clamp iterSrcRow = CPixelIterator_32bppBGRA_Clamp();
    iterSrcRow.Initialize(spSrcBitmap, 0,0,false );
    CPixelIterator_32bppBGRA_Clamp iterDst = CPixelIterator_32bppBGRA_Clamp();
    iterDst.Initialize(spDstBitmap, 0,0,false );
    CPixelIterator_32bppBGRA_Clamp iterSrcCol;



    ULONG uiDestX, uiDestY;
    WICColor  pixel = {0};
    for(ULONG uiY = 0;    uiY < uiSrcHeight;    uiY++)
    {
        iterSrcCol = iterSrcRow;

        for(ULONG uiX = 0;    uiX < uiSrcWidth;    uiX++)
        {
            pixel = iterSrcCol.GetPixel();

            m_clsPartition.Transform( uiX, uiY, &uiDestX, &uiDestY );

            iterDst.SetCoordinates( uiDestX, uiDestY );

            iterDst.SetPixel( pixel );

            iterSrcCol.StepRight();
        }
        iterSrcRow.StepDown();
    }

    hr=spDstBitmap->QueryInterface(__uuidof(IWICBitmapSource),
                                        reinterpret_cast<void**>(ppBitmapSource));

    
    MILMatrixF matrix;

    //**********************************************************************
    // Get our render context's implementation interface
    //**********************************************************************
    CComPtr<IMILBitmapEffectRenderContextImpl> spRenderContextImpl;
    if (SUCCEEDED(hr))
    {
        hr = pRenderContext->QueryInterface(__uuidof(IMILBitmapEffectRenderContextImpl),
                                            reinterpret_cast<void**>(&spRenderContextImpl));
    }

    hr= spRenderContextImpl->GetTransform( &matrix ) ;

    
    CComPtr< IMILBitmapEffectRenderContext > spRenderContextNonImpl;
    if( SUCCEEDED(hr ) )
    {
        hr= pRenderContext->QueryInterface( __uuidof( IMILBitmapEffectRenderContext),
                                                reinterpret_cast<void**>( &spRenderContextNonImpl ) );
    }

    double dblDpiX = 0.0, dblDpiY = 0.0;

    if( SUCCEEDED(hr ) )
    {
        hr= spRenderContextNonImpl->GetOutputDPI( &dblDpiX, &dblDpiY ) ;
    }
    

    if( SUCCEEDED( hr ) )
    {
        double x_off = m_clsPartition.CalcDestXOffset_Points( dblDpiX );
        double y_off = m_clsPartition.CalcDestYOffset_Points( dblDpiY );

        matrix._31 -= x_off;
        matrix._32 -= y_off;

        hr=spRenderContextImpl->UpdateTransform(&matrix);
    }
    

    return(hr);
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::TransformPoint
//
//  Synopsis:
//      










STDMETHODIMP
    CExplode::TransformPoint(ULONG uiIndex, MIL_2DPOINTD *p,
    VARIANT_BOOL fForwardTransform, IMILBitmapEffectRenderContext *pContext,
    VARIANT_BOOL *pfPointTransformed)
{
    HRESULT hr = S_OK;
      assert( p );
    assert( pfPointTransformed );
    assert( pContext );

    if( (NULL == p) || (NULL == pfPointTransformed) || (NULL == pContext) )
        return( E_INVALIDARG );

    CComPtr<IMILBitmapEffectOutputConnector> spOutputConnector;
    
    //**********************************************************************
    // Get the connections object for current effect
    //**********************************************************************
    CComPtr<IMILBitmapEffectConnections> spConnections;
    hr = this->QueryInterface(__uuidof(IMILBitmapEffectConnections),
                                         reinterpret_cast<void**>(&spConnections));

    //**********************************************************************
    // Find the input connector on the effect
    //**********************************************************************
    CComPtr<IMILBitmapEffectInputConnector> spInputConnector;
    if (SUCCEEDED(hr))
    {
        hr = spConnections->GetInputConnector(0, &spInputConnector);
    }
    
    VARIANT_BOOL vbConnected = VARIANT_FALSE;
    while (true)
    {
        if (SUCCEEDED(hr))
        {
            hr = spInputConnector->IsConnected(&vbConnected);
        }
        if (vbConnected == VARIANT_FALSE)
        {
            break;
        }
        //**********************************************************************
        // Find the output connector the input connector is connected to
        //**********************************************************************
        if (SUCCEEDED(hr))
        {
            spOutputConnector.Release();
            hr = spInputConnector->GetConnection(&spOutputConnector);
        }
        
        //**********************************************************************
        // Check if the output connector is an interior connector. In this case
        // it is interior portion of a group effect which means we need to step
        // outside the group.
        //**********************************************************************
        CComPtr<IMILBitmapEffectInteriorInputConnector> spInteriorInput;
        if (SUCCEEDED(hr))
        {
            hr = spOutputConnector->QueryInterface(__uuidof(IMILBitmapEffectInteriorInputConnector),
                                                   reinterpret_cast<void**>(&spInteriorInput));
        }
        
        //**********************************************************************
        // If it isn't an interior connector we are done
        //**********************************************************************
        if (FAILED(hr))
        {
            if (hr == E_NOINTERFACE)
            {
                hr = S_OK;
            }
            break;
        }
        
        //**********************************************************************
        // Otherwise, get the input connector that is associated with the
        // interior connector (i.e. step outside the current group effect)
        // Loop back up and see if we are inside yet another group object.
        //**********************************************************************
        spInputConnector.Release();
        hr = spInteriorInput->GetInputConnector(&spInputConnector);
        if (FAILED(hr))
        {
            break;
        }
    }
    if (SUCCEEDED(hr))
    {
        if ( ! fForwardTransform)
        {
            if (m_dRadius > 1)  // Radius 1 = degenerate case => Nothing explodes. No need for complex math. 
            {
                //get the horizontal and vertical DPI
                CComPtr< IMILBitmapEffectRenderContext > spRenderContextNonImpl;
                hr= pContext->QueryInterface( __uuidof( IMILBitmapEffectRenderContext),
                    reinterpret_cast<void**>( &spRenderContextNonImpl) );
                if( FAILED( hr) )    return( hr );
                double dblDpiX = 0.0, dblDpiY = 0.0;
                hr= spRenderContextNonImpl->GetOutputDPI( &dblDpiX, &dblDpiY ) ;
                if( FAILED( hr) )    return( hr );

                //calculate offset to convert to pixel-space 
                MIL_RECTD rect = {0};
                MIL_RECTD *pRect = &rect;
                TransformRect( 0, pRect, VARIANT_TRUE, pContext );
                p->X-=pRect->X;
                p->Y-=pRect->Y;

                //convert from point space to pixel space [pixel space is (0,0)-->(souceWidthPixels,sourceHeightPixels) ]
                ULONG uiDstX = (ULONG)( p->X * dblDpiX / 96.0 );
                ULONG uiDstY = (ULONG)( p->Y * dblDpiY / 96.0 );

                //inverse-transform in pixel space
                ULONG uiSrcX, uiSrcY;
                if( m_clsPartition.InverseTransform( uiDstX, uiDstY,
                    &uiSrcX, &uiSrcY ) )
                {
                    *pfPointTransformed = VARIANT_TRUE;

                    //convert back to point space
                    p->X = (double)(uiSrcX) * 96.0 / dblDpiX;
                    p->Y = (double)(uiSrcY) * 96.0 / dblDpiY;

                    p->X+=pRect->X;
                    p->Y+=pRect->Y;

                    //add x,y point offset values (since the image expanded from the center)
                    double dXOffset = m_clsPartition.CalcDestXOffset_Points( dblDpiX );
                    double dYOffset = m_clsPartition.CalcDestYOffset_Points( dblDpiY );

                    p->X+= dXOffset;
                    p->Y+= dYOffset;
                    hr = S_OK;
                }
                else
                {
                    hr = E_FAIL;
                }
            }

        }
        if (vbConnected == VARIANT_TRUE)
        {
            CComPtr<IMILBitmapEffect> spPreviousEffect;
            hr = spOutputConnector->GetBitmapEffect(&spPreviousEffect);
            CComPtr<IMILBitmapEffectPrimitive> spPrimitive;
            if (SUCCEEDED(hr))
            {
                hr = spPreviousEffect->QueryInterface(__uuidof(IMILBitmapEffectPrimitive),
                                                      reinterpret_cast<void**>(&spPrimitive));
            }
            if (SUCCEEDED(hr))
            {
                hr = spPrimitive->TransformPoint(0, p, fForwardTransform, pContext, pfPointTransformed);
            }
        }
        if (SUCCEEDED(hr) && fForwardTransform)
        {
            hr = E_NOTIMPL ;
        }
    }
    if (SUCCEEDED(hr))
    {
        *pfPointTransformed = VARIANT_TRUE;
    }
    else
    {
        *pfPointTransformed = VARIANT_FALSE;
    }

    return S_OK; //Always return S_OK from here. False/True should handle the cases
}



//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::TransformRect
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::TransformRect(ULONG uiIndex, MIL_RECTD *pRect,
                                          VARIANT_BOOL fForwardTransform,
                                          IMILBitmapEffectRenderContext *pRenderContext)
{
    assert(pRect);
    if (pRect == NULL)
    {
        return(E_INVALIDARG);
    }

    HRESULT hr = S_OK;
    
    //**********************************************************************
    // Get the connections object for current effect
    //**********************************************************************
    CComPtr<IMILBitmapEffectConnections> spConnections;
    hr = this->QueryInterface(__uuidof(IMILBitmapEffectConnections),
                              reinterpret_cast<void**>(&spConnections));

    //**********************************************************************
    // Find the input connector on the effect
    //**********************************************************************
    CComPtr<IMILBitmapEffectInputConnector> spInputConnector;
    if (SUCCEEDED(hr))
    {
        hr = spConnections->GetInputConnector(0, &spInputConnector);
    }
    
    CComPtr<IMILBitmapEffectOutputConnector> spOutputConnector;
    VARIANT_BOOL vbConnected = VARIANT_FALSE;
    while (true)
    {
        if (SUCCEEDED(hr))
        {
            hr = spInputConnector->IsConnected(&vbConnected);
        }
        if (vbConnected == VARIANT_FALSE)
        {
            break;
        }
        //**********************************************************************
        // Find the output connector the input connector is connected to
        //**********************************************************************
        if (SUCCEEDED(hr))
        {
            spOutputConnector.Release();
            hr = spInputConnector->GetConnection(&spOutputConnector);
        }
        
        //**********************************************************************
        // Check if the output connector is an interior connector. In this case
        // it is interior portion of a group effect which means we need to step
        // outside the group.
        //**********************************************************************
        CComPtr<IMILBitmapEffectInteriorInputConnector> spInteriorInput;
        if (SUCCEEDED(hr))
        {
            hr = spOutputConnector->QueryInterface(__uuidof(IMILBitmapEffectInteriorInputConnector),
                                                   reinterpret_cast<void**>(&spInteriorInput));
        }
        
        //**********************************************************************
        // If it isn't an interior connector we are done
        //**********************************************************************
        if (FAILED(hr))
        {
            if (hr == E_NOINTERFACE)
            {
                hr = S_OK;
            }
            break;
        }
        
        //**********************************************************************
        // Otherwise, get the input connector that is associated with the
        // interior connector (i.e. step outside the current group effect)
        // Loop back up and see if we are inside yet another group object.
        //**********************************************************************
        spInputConnector.Release();
        hr = spInteriorInput->GetInputConnector(&spInputConnector);
        if (FAILED(hr))
        {
            break;
        }
    }
    if (SUCCEEDED(hr))
    {
        if ( ! fForwardTransform)
        {
            ;
        }
        if (vbConnected == VARIANT_TRUE)
        {
            CComPtr<IMILBitmapEffect> spPreviousEffect;
            if (SUCCEEDED(hr))
            {
                hr = spOutputConnector->GetBitmapEffect(&spPreviousEffect);
            }
            CComPtr<IMILBitmapEffectPrimitive> spPrimitive;
            if (SUCCEEDED(hr))
            {
                hr = spPreviousEffect->QueryInterface(__uuidof(IMILBitmapEffectPrimitive),
                                                      reinterpret_cast<void**>(&spPrimitive));
            }
            if (SUCCEEDED(hr))
            {
                hr = spPrimitive->TransformRect(uiIndex, pRect, fForwardTransform, pRenderContext);
            }
        }
        if (fForwardTransform)
        {
        HRESULT hr = S_OK;

        CComPtr< IMILBitmapEffectRenderContext > spRenderContextNonImpl;
        if( SUCCEEDED(hr ) )
        {
            hr= pRenderContext->QueryInterface( __uuidof( IMILBitmapEffectRenderContext),
                                                    reinterpret_cast<void**>( &spRenderContextNonImpl ) );
        }

        double dblDpiX = 0.0, dblDpiY = 0.0;

        if( SUCCEEDED(hr ) )
        {
            hr= spRenderContextNonImpl->GetOutputDPI( &dblDpiX, &dblDpiY ) ;
        }

        double dXOffset = m_clsPartition.CalcDestXOffset_Points( dblDpiX );
        double dYOffset = m_clsPartition.CalcDestYOffset_Points( dblDpiY );

        double newWidth = m_dRadius * pRect->Width;
        double newHeight = m_dRadius * pRect->Height;

        pRect->Width = newWidth;
        pRect->Height = newHeight;
        pRect->X-= dXOffset;
        pRect->Y-= dYOffset;
        }
    }

    return hr; //Always return S_OK from here. False/True should handle the cases}
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::HasAffineTransform
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::HasAffineTransform(ULONG uiIndex, VARIANT_BOOL *pfAffine)
{
    assert(pfAffine);
    if (pfAffine == NULL)
    {
        return(E_INVALIDARG);
    }
    
    *pfAffine = VARIANT_FALSE;

    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::HasInverseTransform
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::HasInverseTransform(ULONG uiIndex, VARIANT_BOOL *pfHasInverse)
{
    HRESULT hr = S_OK;
    *pfHasInverse = VARIANT_FALSE;
    return hr;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetAffineMatrix
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::GetAffineMatrix(ULONG uiIndex, MIL_MATRIX3X2D *pMatrix)
{
    HRESULT hr = S_OK;
    pMatrix->S_11 = 1.0f;
    pMatrix->S_12 = 0.0f;
    pMatrix->S_21 = 0.0f;
    pMatrix->S_22 = 1.0f;
    pMatrix->DX = 0.0f;
    pMatrix->DY = 0.0f;
    return hr;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::IsDirty
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::IsDirty(ULONG uiOutputIndex, VARIANT_BOOL *pfDirty)
{
    HRESULT hr = S_OK;
    *pfDirty = VARIANT_TRUE;
    return hr;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::IsVolatile
//
//  Synopsis:
//      
STDMETHODIMP
CExplode::IsVolatile(ULONG uiOutputIndex, VARIANT_BOOL *pfVolatile)
{
    HRESULT hr = S_OK;
    *pfVolatile = VARIANT_TRUE;
    return hr;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::CountProperties
//
//  Synopsis:
//      IPropertyBag2 method. See MSDN for details.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::CountProperties(ULONG *pcProperties)
{
     assert(pcProperties);
    if (pcProperties == NULL)
    {
        return(E_INVALIDARG);
    }
    *pcProperties = 2;
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetPropertyInfo
//
//  Synopsis:
//      IPropertyBag2 method. See MSDN for details.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetPropertyInfo(ULONG iProperty, ULONG cProperties, PROPBAG2 *pPropBag, ULONG *pcProperties)
{
    assert(pPropBag);
    assert(pcProperties);
    if (pPropBag == NULL || pcProperties == NULL)
    {
        return(E_INVALIDARG);
    }
    
    assert( iProperty + cProperties < 3 );
    if( iProperty + cProperties > 2)
    {
        return( E_INVALIDARG );
    }

    ZeroMemory(pPropBag, sizeof(PROPBAG2) * cProperties);
    *pcProperties = 0;
    HRESULT hr = S_OK;
    int j = 0;
    for (ULONG i = iProperty; i < iProperty + cProperties; i++)
    {
        switch (i)
        {
        case 0:
            {
                pPropBag[j].dwType = PROPBAG2_TYPE_DATA;
                pPropBag[j].vt = VT_UI4;
                int iNumBytes = static_cast<int>((wcslen(c_pwcsSeedNumber) + 1) * sizeof(WCHAR));
                LPVOID pvData = CoTaskMemAlloc(iNumBytes);
                if (pvData == NULL)
                {
                    hr=E_OUTOFMEMORY;
                    break;
                }
                memcpy(pvData, c_pwcsSeedNumber, iNumBytes);
                pPropBag[j].pstrName = reinterpret_cast<LPOLESTR>(pvData);
                (*pcProperties)++;
            }
            break;
        case 1:
            {
                pPropBag[j].dwType = PROPBAG2_TYPE_DATA;
                pPropBag[j].vt = VT_R8;
                int iNumBytes = static_cast<int>((wcslen(c_pwcsRadius) + 1) * sizeof(WCHAR));
                LPVOID pvData = CoTaskMemAlloc(iNumBytes);
                if (pvData == NULL)
                {
                    hr=E_OUTOFMEMORY;
                    break;
                }
                memcpy(pvData, c_pwcsRadius, iNumBytes);
                pPropBag[j].pstrName = reinterpret_cast<LPOLESTR>(pvData);
                (*pcProperties)++;
            }
            break;
        }
        if (FAILED(hr))
        {
            break;
        }
        j++;
    }

    if (FAILED(hr))
    {
        int j = 0;
        for (ULONG i = iProperty; i < cProperties; i++)
        {
            if (pPropBag[j].pstrName)
            {
                CoTaskMemFree(reinterpret_cast<LPVOID>(pPropBag[j].pstrName));
                pPropBag[j].pstrName = NULL;
            }
            j++;
        }
    }

    return(hr);
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::LoadObject
//
//  Synopsis:
//      IPropertyBag2 method. See MSDN for details.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::LoadObject(LPCOLESTR pstrName, DWORD dwHint, IUnknown *pUnkObject, IErrorLog *pErrLog)
{
    HRESULT hr = S_OK;
    return hr;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::Read
//
//  Synopsis:
//      IPropertyBag2 method. See MSDN for details.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::Read(ULONG cProperties, PROPBAG2 *pPropBag, IErrorLog *pErrLog, VARIANT *pvarValue, HRESULT *phrError)
{
     HRESULT hr = S_OK;
    assert(pPropBag);
    assert(pvarValue);
    if (pPropBag == NULL || pvarValue == NULL)
    {
        return(E_INVALIDARG);
    }
    
    for (ULONG i = 0; i < cProperties; i++)
    {
        if (_wcsicmp(pPropBag[i].pstrName, c_pwcsSeedNumber) == 0)
        {
            pvarValue[i].vt = VT_UI4;
            pvarValue[i].ulVal = m_uiSeedNumber;
            phrError[i] = S_OK;
        }
        else if (_wcsicmp(pPropBag[i].pstrName, c_pwcsRadius) == 0)
        {
            pvarValue[i].vt = VT_R8;
            pvarValue[i].dblVal = m_dRadius;
            phrError[i] = S_OK;
        }
        else
        {
            phrError[i] = E_FAIL;
            hr=E_FAIL; // a property was requested that we don't have
        }
    }
    return(hr);
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::Write
//
//  Synopsis:
//      IPropertyBag2 method. See MSDN for details.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::Write(ULONG cProperties, PROPBAG2 *pPropBag, VARIANT *pvarValue)
{
    HRESULT hrRes = S_OK;
    HRESULT hr;
    assert(pPropBag);
    assert(pvarValue);
    if (pPropBag == NULL || pvarValue == NULL)
    {
        return(E_INVALIDARG);
    }
    
    for (ULONG i = 0; i < cProperties; i++)
    {
        if (_wcsicmp(pPropBag[i].pstrName, c_pwcsSeedNumber) == 0)
        {
            VARIANTARG varResult;
            VariantInit(&varResult);
            hr=VariantChangeType(&varResult, &pvarValue[i], 0, VT_UI4);
            if (SUCCEEDED(hr))
            {
                if (varResult.ulVal > 0)
                {
                    m_uiSeedNumber = varResult.ulVal;
                    m_clsPartition.SetSeed( m_uiSeedNumber );
                }
                else
                {
                    hrRes = E_INVALIDARG;
                }                
            }
            else
            {
                hrRes = hr;
            }
        }
        else if (_wcsicmp(pPropBag[i].pstrName, c_pwcsRadius) == 0)
        {
            VARIANTARG varResult;
            VariantInit(&varResult);
            hr=VariantChangeType(&varResult, &pvarValue[i], 0, VT_R8);
            if (SUCCEEDED(hr))
            {
                if( m_dRadius >= 1.0)
                    m_dRadius = varResult.dblVal;
                else
                    hrRes = E_INVALIDARG;
            }
            else
            {
                hrRes = hr;
            }
        }
        else
        {
            hrRes = E_FAIL;
            hr=hrRes;
        }
    }
    return(hrRes);
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetNumberInputs
//
//  Synopsis:
//      IMILBitmapEffectConnectionsInfo method.
//      Returns the number of input pins exposed by this effect.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetNumberInputs(ULONG *puiNumInputs)
{
    *puiNumInputs = 1;
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetNumberOutputs
//
//  Synopsis:
//      IMILBitmapEffectConnectionsInfo method.
//      Returns the number of output pins exposed by this effect.
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetNumberOutputs(ULONG *puiNumOutputs)
{
    *puiNumOutputs = 1;
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetInputConnectorInfo
//
//  Synopsis:
//      IMILBitmapEffectConnectionsInfo method.
//      Returns the description for the specified input pin
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetInputConnectorInfo(ULONG uiIndex, IMILBitmapEffectConnectorInfo **ppConnectorInfo)
{
    *ppConnectorInfo = this;
    AddRef();
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetOutputConnectorInfo
//
//  Synopsis:
//      IMILBitmapEffectConnectionsInfo method.
//      Returns the description for the specified output pin
//
//----------------------------------------------------------------------------
STDMETHODIMP
CExplode::GetOutputConnectorInfo(ULONG uiIndex, IMILBitmapEffectConnectorInfo **ppConnectorInfo)
{
    *ppConnectorInfo = this;
    AddRef();
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetIndex
//
//  Synopsis:
//      IMILBitmapEffectConnectorInfo method.
//      Returns the index for the pin.
//
//----------------------------------------------------------------------------
STDMETHODIMP 
CExplode::GetIndex(ULONG *puiIndex)
{
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetOptimalFormat
//
//  Synopsis:
//      IMILBitmapEffectConnectorInfo method.
//      Returns the optimal image format for this pin
//
//----------------------------------------------------------------------------
STDMETHODIMP 
CExplode::GetOptimalFormat(WICPixelFormatGUID *pFormat)
{
    *pFormat = GUID_WICPixelFormat32bppBGRA;
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetNumberFormats
//
//  Synopsis:
//      IMILBitmapEffectConnectorInfo method.
//      Returns the number of formats supported by this pin
//
//----------------------------------------------------------------------------
STDMETHODIMP 
CExplode::GetNumberFormats(ULONG *pulNumberFormats)
{
    *pulNumberFormats = 1;
    return S_OK;
}

//+---------------------------------------------------------------------------
//
//  Member:
//      CExplode::GetFormat
//
//  Synopsis:
//      IMILBitmapEffectConnectorInfo method.
//      Returns the requested format
//
//----------------------------------------------------------------------------
STDMETHODIMP 
CExplode::GetFormat(ULONG ulIndex, WICPixelFormatGUID *pFormat)
{
    *pFormat = GUID_WICPixelFormat32bppBGRA;
    return S_OK;
}

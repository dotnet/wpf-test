//+--------------------------------------------------------------------------
//
//  Copyright (c) Microsoft Corporation.  All rights reserved.
//
//  Abstract:
//     Header for classes used for manipulate bits in an image
//
//----------------------------------------------------------------------------

#pragma once
#define MIL_COLOR(a, r, g, b) \
    (WICColor)(((a) & 0xFF) << MIL_ALPHA_SHIFT | \
     ((r) & 0xFF) << MIL_RED_SHIFT | \
     ((g) & 0xFF) << MIL_GREEN_SHIFT | \
     ((b) & 0xFF) << MIL_BLUE_SHIFT)
typedef UINT64 MILColor64;

const INT MIL_ALPHA_SHIFT = 24;
const INT MIL_RED_SHIFT   = 16;
const INT MIL_GREEN_SHIFT =  8;
const INT MIL_BLUE_SHIFT  =  0;

#define MIL_ALPHA_MASK  ((WICColor) 0xff << MIL_ALPHA_SHIFT)
#define MIL_RED_MASK    ((WICColor) 0xff << MIL_RED_SHIFT)
#define MIL_GREEN_MASK  ((WICColor) 0xff << MIL_GREEN_SHIFT)
#define MIL_BLUE_MASK   ((WICColor) 0xff << MIL_BLUE_SHIFT)

#define MIL_COLOR_GET_ALPHA(c) (((c) & MIL_ALPHA_MASK) >> MIL_ALPHA_SHIFT)
#define MIL_COLOR_GET_RED(c)   (((c) & MIL_RED_MASK)   >> MIL_RED_SHIFT)
#define MIL_COLOR_GET_GREEN(c) (((c) & MIL_GREEN_MASK) >> MIL_GREEN_SHIFT)
#define MIL_COLOR_GET_BLUE(c)  (((c) & MIL_BLUE_MASK)  >> MIL_BLUE_SHIFT)


#define ReleaseInterface(x) do{if (x) {(x)->Release(); (x) = NULL; }}while(0);

//**********************************************************************
// Abstract:
//**********************************************************************
namespace PixelWrap
{
    enum Enum
    {
        None,
        Clamp,
        Wrap
    };
}

//+--------------------------------------------------------------------------
//
//  Abstract:
//     CPixelIteratorBase is our base class for all the special case iterators
//     used for walking a specific image type.
//
//----------------------------------------------------------------------------
class CPixelIteratorBase
{
public:
    CPixelIteratorBase() 
    {
        m_pixelFormat = GUID_WICPixelFormatDontCare;
    }
    
    HRESULT Initialize(IWICBitmap *pBitmap, int iX, int iY, WICPixelFormatGUID pixelFormat, PixelWrap::Enum enumClampMethod)
    {
        m_spBitmap = pBitmap;
        HRESULT hr = S_OK;
        (pBitmap->GetSize(&m_uiWidth, &m_uiHeight));
        if (FAILED(hr))
        {
            return(hr);
        }
        m_iX = iX;
        m_iY = iY;
        m_enumClampMethod = enumClampMethod;
        m_pixelFormat = pixelFormat;
        if (m_pixelFormat == GUID_WICPixelFormatDontCare)
        {
            (pBitmap->GetPixelFormat(&m_pixelFormat));
            if (FAILED(hr))
            {
                return(hr);
            }
        }
        if (m_pixelFormat == GUID_WICPixelFormat24bppBGR ||
            m_pixelFormat == GUID_WICPixelFormat24bppRGB)
        {
            m_uiBytesPerPixel = 3;
        }
        else if (m_pixelFormat == GUID_WICPixelFormat32bppBGR ||
                 m_pixelFormat == GUID_WICPixelFormat32bppBGRA ||
                 m_pixelFormat == GUID_WICPixelFormat32bppPBGRA)
        {
            m_uiBytesPerPixel = 4;
        }
        else
        {
            return(E_INVALIDARG);
        }
        m_uiBytesPerScanline = m_uiWidth * m_uiBytesPerPixel;

        WICRect rcLock = {0, 0, m_uiWidth, m_uiHeight};
        (pBitmap->Lock(&rcLock, WICBitmapLockWrite, &m_spLock));
        if (SUCCEEDED(hr) && m_spLock)
        {
            (m_spLock->GetDataPointer(&m_uiBufferSize, (BYTE**)&m_pbData));
            m_pbOrigData = m_pbData;
            if (SUCCEEDED(hr))
            {
                UINT uiStride;
                (m_spLock->GetStride(&uiStride));
                if (SUCCEEDED(hr))
                {
                    m_pbData += static_cast<INT_PTR>(uiStride * static_cast<INT_PTR>(iY) +
                                                     static_cast<INT_PTR>(iX) * static_cast<INT_PTR>(m_uiBytesPerPixel));
                }
            }
        }
        return(hr);
    }
    
    ~CPixelIteratorBase()
    {
    }
    
    CPixelIteratorBase operator=(CPixelIteratorBase &rPixelIterator)
    {
        m_pbData = rPixelIterator.m_pbData;
        m_pbOrigData = rPixelIterator.m_pbOrigData;
        m_spBitmap = rPixelIterator.m_spBitmap;
        m_iX = rPixelIterator.m_iX;
        m_iY = rPixelIterator.m_iY;
        m_enumClampMethod = rPixelIterator.m_enumClampMethod;
        m_uiBytesPerScanline = rPixelIterator.m_uiBytesPerScanline;
        m_uiBytesPerPixel = rPixelIterator.m_uiBytesPerPixel;
        m_pixelFormat = rPixelIterator.m_pixelFormat;
        m_uiWidth = rPixelIterator.m_uiWidth;
        m_uiHeight = rPixelIterator.m_uiHeight;
        m_spLock = NULL;
        return *this;
    }
    void SetCoordinates(int iX, int iY)
    {
        m_iX = iX;
        m_iY = iY;
        m_pbData = &m_pbOrigData[iY * m_uiBytesPerScanline + iX * m_uiBytesPerPixel];        
    }
    CPixelIteratorBase(CPixelIteratorBase &rPixelIterator)
    {
        m_pbData = rPixelIterator.m_pbData;
        m_pbOrigData = rPixelIterator.m_pbOrigData;
        m_spBitmap = rPixelIterator.m_spBitmap;
        m_iX = rPixelIterator.m_iX;
        m_iY = rPixelIterator.m_iY;
        m_enumClampMethod = rPixelIterator.m_enumClampMethod;
        m_uiBytesPerScanline = rPixelIterator.m_uiBytesPerScanline;
        m_uiBytesPerPixel = rPixelIterator.m_uiBytesPerPixel;
        m_pixelFormat = rPixelIterator.m_pixelFormat;
        m_uiWidth = rPixelIterator.m_uiWidth;
        m_uiHeight = rPixelIterator.m_uiHeight;
        m_spLock = NULL;
    }

    void StepLeft()
    {
        m_iX--;
        m_pbData -= static_cast<INT_PTR>(m_uiBytesPerPixel);
    }
  
    void StepRight()
    {
        m_iX++;
        m_pbData += static_cast<INT_PTR>(m_uiBytesPerPixel);
    }
  
    void StepUp()
    {
        m_iY--;
        m_pbData -= static_cast<INT_PTR>(m_uiBytesPerScanline);
    }
  
    void StepDown()
    {
        m_iY++;
        m_pbData += static_cast<INT_PTR>(m_uiBytesPerScanline);
    }
    
protected:
    UINT m_uiBufferSize;
    BYTE *m_pbData;
    BYTE *m_pbOrigData;
    CComPtr<IWICBitmap> m_spBitmap;
    int m_iX;
    int m_iY;
    UINT m_uiWidth;
    UINT m_uiHeight;
    PixelWrap::Enum m_enumClampMethod;
    int m_uiBytesPerScanline;
    int m_uiBytesPerPixel;
    WICPixelFormatGUID m_pixelFormat;
    CComPtr<IWICBitmapLock> m_spLock;    // NOTE: Only the original iterator can lock/unlock this.
                                // Be careful not to copy the iterator and then destroy the original
};

//+--------------------------------------------------------------------------
//
//  Abstract:
//     CPixelIterator_32bppBGRA_Clamp is used to walk a 32bppBGRA image using
//     clamping as the wrap mode (i.e. if the iterator steps off the image
//     it will pull pixels from the nearest image edge).
//
//----------------------------------------------------------------------------
class CPixelIterator_32bppBGRA_Clamp : public CPixelIteratorBase
{
    bool m_fZeroAlpha;
public:
    CPixelIterator_32bppBGRA_Clamp()
    {
        m_fZeroAlpha = false;
    }
    
    ~CPixelIterator_32bppBGRA_Clamp()
    {
    }
    
    HRESULT Initialize(IWICBitmap *pBitmap, int iX, int iY, bool fZeroAlpha = false)
    {
        HRESULT hr = __super::Initialize(pBitmap, iX, iY, GUID_WICPixelFormatDontCare, PixelWrap::Clamp);
        m_fZeroAlpha = fZeroAlpha;
        return(hr);
    }
    
    WICColor GetPixel()    
    {
        BYTE *pbData = NULL;
        int iAlpha;
        if (m_iX < 0 || m_iY < 0 || m_iX >= static_cast<int>(m_uiWidth) || m_iY >= static_cast<int>(m_uiHeight))
        {
            int iX = m_iX;
            int iY = m_iY;
            if (m_iX < 0)
            {
                iX = 0;
            }
            else if (m_iX >= static_cast<int>(m_uiWidth))
            {
                iX = m_uiWidth - 1;
            }
            if (m_iY < 0)
            {
                iY = 0;
            }
            else if (m_iY >= static_cast<int>(m_uiHeight))
            {
                iY = m_uiHeight - 1;
            }
            pbData = &m_pbOrigData[iY * m_uiBytesPerScanline + iX * m_uiBytesPerPixel];
            iAlpha = (m_fZeroAlpha) ? 0 : pbData[3];
        }
        else
        {
            pbData = m_pbData;
            iAlpha = pbData[3];
        }
        return MIL_COLOR(iAlpha, pbData[2], pbData[1], pbData[0]);
    }

    void SetPixel(WICColor clr)
    {
        if (m_iX < 0 || m_iY < 0 || m_iX >= static_cast<int>(m_uiWidth) || m_iY >= static_cast<int>(m_uiHeight))
        {
            return;
        }
        m_pbData[3] = MIL_COLOR_GET_ALPHA(clr);
        m_pbData[2] = MIL_COLOR_GET_RED(clr);
        m_pbData[1] = MIL_COLOR_GET_GREEN(clr);
        m_pbData[0] = MIL_COLOR_GET_BLUE(clr);
        return;
    }
};

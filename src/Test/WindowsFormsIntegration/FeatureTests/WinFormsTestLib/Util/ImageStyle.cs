using System;
    
namespace WFCTestLib.Util
{
    // <doc>
    // <desc>
    //  This enumeration is used by the RandomUtil class to choose an image type.
    // </desc>
    // <seealso class="RandomUtil" member="ImageNames"/>
    // <seealso class="RandomUtil" member="GetImage"/>    
    // </doc>
    public enum ImageStyle
    {
    
        // <doc>
        // <desc>
        //  A Bitmap (BEANY.BMP)
        // </desc>
        // </doc>
        Bitmap  = 0,
       
        // <doc>
        // <desc>
        //  Enhanced Metafile (MS.EMF)
        // </desc>
        // </doc>
        EMF     = 1,
    
        // <doc>
        // <desc>
        //  GIF (mts.gif)
        // </desc>
        // </doc>
        GIF     = 2,
    
        // <doc>
        // <desc>
        //  JPG (rock.jpg)
        // </desc>
        // </doc>
        JPG     = 3,
    
        // <doc>
        // <desc>
        //  Icon (FIRE.ICO)
        // </desc>
        // </doc>
        Icon     = 4,
    
        // <doc>
        // <desc>
        //  Windows Metafile (FLOWER.WMF)
        // </desc>
        // </doc>
        WMF     = 5,
    
        // <doc>
        // <desc>
        //  Animated GIF (mts.gif)
        // </desc>
        // </doc>
        ANIGIF     = 6,

        // <doc>
        // <desc>
        //  A Random bmp, emf, gif, jpg, ico, or wmf object
        // </desc>
        // </doc>
        Random  = 7,

        
    }
}

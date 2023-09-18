// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;

namespace Microsoft.Test.Graphics.CachedComposition
{
    [Test(1, "CachedCompositionFunctional", "Full Vector Coverage",
     Area = "2D",
     Description = "CachedComposition extended functional tests - non exhaustive",
     SupportFiles = @"FeatureTests\2D\Part1\Masters\red2SecondsGreen30.wmv",
     SecurityLevel = TestCaseSecurityLevel.FullTrust)
    ]
    public class CachedCompositionFunctionalExtended : CachedCompositionFunctional
    {

        #region Extended Functional variations

        //all invalidating properties
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.TileBrush, Detector.Visual,   true, 0,false,false,false,-1)]
        
        ////////////////////////////////////////////////////////////////////////////////////////////        
        // DISABLEDUNSTABLETEST:
        // TestName:Full Vector Coverage(Normal\,Normal\,Theme\,False\,TileBrush\,ETW\,True\,1\,False\,False\,False\,-1)
        // Area: 2D   SubArea: CachedCompositionFunctional
        // Disable this case due to high fail rate, will enable after fix it.
        // to find all disabled tests in test tree, use: “findstr /snip DISABLEDUNSTABLETEST” 
        ////////////////////////////////////////////////////////////////////////////////////////////
        //[Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Theme, false, Context.TileBrush, Detector.ETW,            true, 1, false, false, false, -1)]
        
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.RenderScale, false, Context.TileBrush, Detector.ETW,      true, 1,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, false, Context.TileBrush, Detector.ETW,  true, 1,false,false,false,-1)]

        //All Renderscales
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,          true, 0,false,false,false,-1)]
        [Variation(RenderScale.One, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,             true, 0,false,false,false,-1)]
        //DISABLEUNSTABLETEST [Variation(RenderScale.OnePixelElement, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.ETW,    true, 1, false, false, false, -1)]
        [Variation(RenderScale.TooLarge, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,        true, 0,false,false,false,-1)]
       
        //All UIElementSizes
        [Variation(RenderScale.Normal, UIElementSize.FullScreen, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,      true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,          true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,          true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxHwTexSize, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,    true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxSoftTexSize, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,  true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.OverMaxHwTexSize, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual,true, 0,false,false,false,-1)]
        //DISABLEUNSTABLETEST [Variation(RenderScale.Normal, UIElementSize.Thin, InvalidatingProperty.Code, false, Context.TileBrush, Detector.ETW,               true, 1, false, false, false, -1)]
        #endregion

        // sole purpose of the class is to hold the variations under a unique priority and name
        public CachedCompositionFunctionalExtended(
                                RenderScale renderScale,
                                UIElementSize uiElementSize,
                                InvalidatingProperty invalidatingProperty,
                                bool snapsToDevicePixels,
                                Context context,
                                Detector detectorType,
                                bool successExpected,
                                int cacheRegensExpected,
                                bool cacheDisabled,
                                bool BCBPresent,
                                bool cacheOnBCB,
                                float screenAreaUpdateExpected
                             )
                             : base(
                                renderScale,
                                uiElementSize,
                                invalidatingProperty,
                                snapsToDevicePixels,
                                context,
                                detectorType,
                                successExpected,
                                cacheRegensExpected,
                                cacheDisabled,
                                BCBPresent,
                                cacheOnBCB,
                                screenAreaUpdateExpected
                             )
        {
        }

    }
}
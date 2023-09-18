// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;

namespace Microsoft.Test.Graphics.CachedComposition
{
    [Test(0, "CachedCompositionPerf", "Functional",
     Area = "2D",
     Description = "CachedComposition perf functional tests",
     SupportFiles = @"FeatureTests\2D\Part1\Masters\red2SecondsGreen30.wmv",
     SecurityLevel=TestCaseSecurityLevel.FullTrust)
    ]
    public class CachedCompositionPerf : CachedCompositionFunctional
    {
        #region variations

        //Event-based checking. This will grow to cover all of the invisible update cases as well as the negative tests for perf.
        //positive cases - where we expect this many events or more
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.ETW, true, 1,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.RenderScale, false, Context.TileBrush, Detector.ETW, true, 1,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, false, Context.TileBrush, Detector.ETW, true, 1,false,false,false,-1)]
        //DISABLEUNSTABLETEST [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, true, Context.TileBrush, Detector.ETW, true, 1,false,false,false,-1)]

        //negative cases - where we expect no regeneration events, or less than a certain number of events
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.AnimatedTransformedBrush, Detector.ETW, true, 0,false,false,false,-1)]
        [Variation(RenderScale.FillMemory, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.AnimatedTransformedBrush, Detector.ETW, true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.FullScreen, InvalidatingProperty.Animation, false, Context.AnimatedTransformedBrush, Detector.ETW, true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxSoftTexSize, InvalidatingProperty.Animation, false, Context.AnimatedTransformedBrush, Detector.ETW, true, 0,false,false,false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.AnimatedTransformedBrush, Detector.ETW, true, 0,false,false,false,-1)]

        //screen area update cases - these tests test how much of the screen we,re updating
        // note that the screenAreaUpdateExpected variation element is set for these.
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.VP2DV3D, Detector.ETW, true, 0, false, false, false, 2)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code,         false,  Context.TileBrush,  Detector.ETW,   true, 0, false, false, false, 2)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, false, Context.TileBrush, Detector.ETW,  true, 0, false, false, false, 2)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, true, Context.TileBrush, Detector.ETW,   true, 0, false, false, false, 2)]

        

        #endregion

        // sole purpose of the class is to hold the variations under a unique priority and name
        public CachedCompositionPerf(
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
                             ): base(
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
                                screenAreaUpdateExpected) { }
    }
}
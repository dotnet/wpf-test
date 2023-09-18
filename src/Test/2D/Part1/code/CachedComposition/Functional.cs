// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;

namespace Microsoft.Test.Graphics.CachedComposition
{
    [Test(0, "CachedCompositionBasic", "Functional",
     Area = "2D",
     Description = "CachedComposition functional tests",
     SupportFiles = @"FeatureTests\2D\Part1\Masters\red2SecondsGreen30.wmv",
     SecurityLevel=TestCaseSecurityLevel.FullTrust
    ) ]
    public class CachedCompositionFunctional : WindowTest
    {
        #region Functional variations

        ////each type of content, changed through the visible change methods
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.VP2DV3D, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TransformedBrush, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.OpacityClippedOutput, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.LayeredWindows, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.RenderTargetBitmap, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxHwTexSize, InvalidatingProperty.Code, false, Context.MultipleMonitor, Detector.Visual, true, 0, false, false, false, -1)]
        //this causes an EVR assert to fire when rendered in software. Workaround is, don't run it in software!
        //tracked by 'future release' Regression_Bug75
        //[Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, true, Context.MediaElement, Detector.Visual, false, 0, false, false, false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, true, Context.TileBrush, Detector.Visual, true, 0, false, false, false, -1)]

        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, true, Context.TileBrush, Detector.Visual, true, 0, false, false, false, -1)]
        //these just don't work on win 7 - something about our process monitoring story is busted.
        //[Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Theme, true, Context.TileBrush, Detector.Visual, true, 0, false, false, false,-1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.TileBrush, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.VP2DV3D, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.TransformedBrush, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.OpacityClippedOutput, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.LayeredWindows, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Animation, false, Context.RenderTargetBitmap, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxHwTexSize, InvalidatingProperty.Animation, false, Context.MultipleMonitor, Detector.Visual, true, 0, false, false, false, -1)]

        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.TileBrush, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.TransformedBrush, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.OpacityClippedOutput, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.LayeredWindows, Detector.Visual, true, 0, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxHwTexSize, InvalidatingProperty.DataBinding, false, Context.MultipleMonitor, Detector.Visual, true, 0, false, false, false, -1)]

        ////Event-based checking. This will grow to cover all of the invisible update cases as well as the negative tests for perf.
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.ETW, true, 1, false, false, false, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, false, Context.TileBrush, Detector.ETW, true, 1, false, false, false, -1)]//new
        // DISABLEUNSTABLETEST [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.EnableClearType, true, Context.TileBrush, Detector.ETW, true, 1, false, false, false, -1)]//new

        //BitmapCacheBrush testing. Works with VisualBrush as a stand-in for BCBs
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.VP2DV3D, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TransformedBrush, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.OpacityClippedOutput, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.LayeredWindows, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.RenderTargetBitmap, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.MaxHwTexSize, InvalidatingProperty.Code, false, Context.MultipleMonitor, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.MediaElement, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.Code, false, Context.TileBrush, Detector.Visual, true, 0, false, true, true, -1)]
        [Variation(RenderScale.Normal, UIElementSize.Normal, InvalidatingProperty.DataBinding, false, Context.TileBrush, Detector.Visual, true, 0, false, true, true, -1)]

        #endregion


        #region Functional Test Method

        public CachedCompositionFunctional(
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
            : base(context == Context.LayeredWindows)
        {
            _req.renderScale = renderScale;
            _req.uIElementSize = uiElementSize;
            _req.invalidatingProperty = invalidatingProperty;
            _req.snapsToDevicePixels = snapsToDevicePixels;
            _req.detectorType = detectorType;
            _req.context = context;
            _req.successExpected = successExpected;
            _req.cacheRegensExpected = cacheRegensExpected;
            _req.cacheDisabled = cacheDisabled;
            _req.bitmapCacheBrush = BCBPresent;
            _req.cacheOnBitmapCacheBrush = cacheOnBCB;
            _req.screenUpdateAreaExpected = screenAreaUpdateExpected;

            _factory = new Factory();
            _content = _factory.CreateContent(_req);
            _detector = _factory.CreateDetector(_req);
            _changer = _factory.CreateChanger(_req);
            _changer.SetContent(_content);
            //InitializeSteps += new TestStep(StoreState);
            RunSteps += new TestStep(Execute);
            //CleanUpSteps += new TestStep(RevertState);
        }

        TestResult StoreState()
        {
            _theme = Microsoft.Test.Display.DisplayConfiguration.GetTheme();
            return TestResult.Pass;
        }

        TestResult RevertState()
        {
            if(_theme != Microsoft.Test.Display.DisplayConfiguration.GetTheme())
             Microsoft.Test.Display.DisplayConfiguration.SetTheme(_theme);
            return TestResult.Pass;
        }

        TestResult Execute()
        {
            Status("Constructing content");
            //now set the RenderScale
            _content.RenderAtScale = s_scaleLookupTable[_req.renderScale];
            this.Window.Content = _content.Construct(_req);
            //now that the content and the window are constructed, size the content to the requested size
            SetSizeBasedOnRequirments();
            _content.Display();
            Window.Activate();
            Window.Show();
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the first render.
            DispatcherHelper.DoEvents(40);//let a couple frames render to settle

            Status("Capturing pre-change state");
            _detector.DetectBefore(this.Window);

            Status("Changing content");
            _changer.Change();
            DispatcherHelper.DoEvents(System.Windows.Threading.DispatcherPriority.Render);//push the second render
            DispatcherHelper.DoEvents(40);//let a couple frames render to settle

            Status("Copturing post-change state");
            _detector.DetectAfter();

            Status("Verifying changes");                   
                             
            if (_detector.VerifyChanges(_req, this.Log))
            {
                return TestResult.Pass;
            }
            return TestResult.Fail;
        }



        private void SetSizeBasedOnRequirments()
        {
            if (_req.uIElementSize == UIElementSize.FullScreen)
            {
                Window.WindowState = WindowState.Maximized;
            }
            else
            {
                setSize(s_windowSizeLookupTable[_req.uIElementSize]);
            }
        }

        private static Dictionary<UIElementSize, Size> MakeWindowSizeLookupTable()
        {
            Dictionary<UIElementSize, Size> table = new Dictionary<UIElementSize, Size>();
            table.Add(UIElementSize.MaxHwTexSize, new Size(2048, 2048));
            table.Add(UIElementSize.MaxSoftTexSize, new Size(2048, 2048));
            table.Add(UIElementSize.Normal, new Size(200, 200));
            table.Add(UIElementSize.OnePixel, new Size(1, 1));
            table.Add(UIElementSize.OverMaxHwTexSize, new Size(100000, 100000));
            table.Add(UIElementSize.Thin, new Size(1, 100));
            table.Add(UIElementSize.Zero, new Size(0, 0));
            return table;
        }
        private static Dictionary<RenderScale, float> MakeRenderScaleLookupTable()
        {
            Dictionary<RenderScale, float> table = new Dictionary<RenderScale, float>();
            table.Add(RenderScale.FillMemory, 20);
            table.Add(RenderScale.Neg, -1);
            table.Add(RenderScale.Normal, 1);
            table.Add(RenderScale.One, 1);
            table.Add(RenderScale.OnePixelElement, 0.01f);
            table.Add(RenderScale.TooLarge, 1000);
            table.Add(RenderScale.Zero, 0);
            return table;
        }

        /// <summary>
        /// Set the size of the host window
        /// </summary>
        /// <param name="s">New size</param>
        private void setSize(Size s)
        {
            Window.Width = s.Width;
            Window.Height = s.Height;
        }

        #endregion

        #region members

        private static Dictionary<RenderScale, float> s_scaleLookupTable = MakeRenderScaleLookupTable();
        private static Dictionary<UIElementSize, Size> s_windowSizeLookupTable = MakeWindowSizeLookupTable();
        
        private Requirements _req;
        private Factory _factory;  // the factory which will make our content, changer, and detector
        private ChangeableContent _content;          // the content that we're testing. Built based on the 'Context' element type
        private Changer _changer;                    // changes the desired 'InvalidatingProperty' of the content. 
        private ChangeDetector _detector;            // detects the desired change - currently, we're just looking at if the rendered output changed. 
        //later on we'll get ETW tracing working and will detect cache regeneration through events as well.
        private string _theme;                       // store the theme so that we can revert it.

        #endregion
    }
}
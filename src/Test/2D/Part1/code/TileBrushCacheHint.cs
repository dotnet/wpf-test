// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

///
/// Purpose: Tests for tilebrush's cache hint
///

using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Tests for tilebrush's cache hint. 
    /// </summary>
    [Test(1, "TileBrushCacheHint",
        Area = "2D",
        Description = @"Tests for tilebrush's cache hint. ",
        SupportFiles = @"Infra\TestApiCore.dll,FeatureTests\2D\Part1\Masters\smallJelly.png,FeatureTests\2D\Part1\Masters\CacheHintMaster*.png"
        )
    ]
    public class TileBrushCacheHint : WindowTest
    {

        /// <summary>
        /// Constructor
        /// </summary>
        [Variation("smallJelly.png", "CacheHintMasterAl.png", "CacheHintMasterAm.png", "CacheHintMasterAs.png", 0.1, 1.0)]
        [Variation("smallJelly.png", "CacheHintMasterBl.png", "CacheHintMasterBm.png", "CacheHintMasterBs.png", 0.1, 1.1)]
        [Variation("smallJelly.png", "CacheHintMasterCl.png", "CacheHintMasterCm.png", "CacheHintMasterCs.png", 0.1, 2.0)]
        [Variation("smallJelly.png", "CacheHintMasterDl.png", "CacheHintMasterDm.png", "CacheHintMasterDs.png", 0.5, 1.0)]
        [Variation("smallJelly.png", "CacheHintMasterEl.png", "CacheHintMasterEm.png", "CacheHintMasterEs.png", 0.5, 1.1)]
        [Variation("smallJelly.png", "CacheHintMasterFl.png", "CacheHintMasterFm.png", "CacheHintMasterFs.png", 0.5, 2.0)]
        [Variation("smallJelly.png", "CacheHintMasterGl.png", "CacheHintMasterGm.png", "CacheHintMasterGs.png", 0.9, 1.0)]
        [Variation("smallJelly.png", "CacheHintMasterHl.png", "CacheHintMasterHm.png", "CacheHintMasterHs.png", 0.9, 1.1)]
        [Variation("smallJelly.png", "CacheHintMasterIl.png", "CacheHintMasterIm.png", "CacheHintMasterIs.png", 0.9, 2.0)]
        public TileBrushCacheHint(string imageFileName,
                                                string expectedLargeFilename,
                                                string expectedMiddleFilename,
                                                string expectedSmallFilename,
                                                double minValue,
                                                double maxValue
            )
            : base(true)
        {
            _imageFile = imageFileName;
            _expectedLargeFile = expectedLargeFilename;
            _expectedMiddleFile = expectedMiddleFilename;
            _expectedSmallFile = expectedSmallFilename;
            _thresholdMinValue = minValue;
            _thresholdMaxValue = maxValue;
            RunSteps += new TestStep(LoadReferenceContent);
            RunSteps += new TestStep(CreateWindowContent);
            RunSteps += new TestStep(Capture);
            RunSteps += new TestStep(VerifyValues);
            RunSteps += new TestStep(VerifyVisuals);
        }

        /// <summary>
        /// Load reference image
        /// </summary>
        /// <returns>success of image loading</returns>
        private TestResult LoadReferenceContent()
        {
            LogComment("Loading reference images [" + _expectedLargeFile + "] and [" + _expectedMiddleFile + "] and ["+ _expectedSmallFile +"]");
            _expectedLarge = new Bitmap(_expectedLargeFile);
            _expectedMiddle = new Bitmap(_expectedMiddleFile);
            _expectedSmall = new Bitmap(_expectedSmallFile);
            if (null == _expectedLarge || null == _expectedMiddle || null == _expectedSmall)
            {
                LogComment("reference images failed to load!");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        #region Test Methods

        /// <summary>
        /// Create window content.
        /// </summary>
        private TestResult CreateWindowContent()
        {   
            Window.WindowStyle = WindowStyle.None;
            Window.ResizeMode = ResizeMode.NoResize;
            ImageBrush ib = new ImageBrush(new BitmapImage(new Uri(_imageFile, UriKind.RelativeOrAbsolute)));
            Window.Background = ib;            
            RenderOptions.SetCachingHint(Window.Background, CachingHint.Cache);
            RenderOptions.SetCacheInvalidationThresholdMinimum(Window.Background, _thresholdMinValue);
            RenderOptions.SetCacheInvalidationThresholdMaximum(Window.Background, _thresholdMaxValue);            
            Window.Height = 100;
            Window.Width = 100;
            return TestResult.Pass;
        }

        /// <summary>
        /// Capture image of content
        /// </summary>
        /// <returns></returns>
        private TestResult Capture()
        {
            _capturedSmall = ScaleAndCapture(_thresholdMinValue - 0.001);
            _capturedMiddle = ScaleAndCapture((_thresholdMinValue + _thresholdMaxValue)/2);
            _capturedLarge = ScaleAndCapture(_thresholdMaxValue + 0.001);
            
            return TestResult.Pass;
        }

        /// <summary>
        /// Verify that the assigned values are correct
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyValues()
        {
            double minimum;
            double maximum;

            TestResult result = TestResult.Pass;
            
            CachingHint cachingHint = RenderOptions.GetCachingHint(Window.Background);

            if (cachingHint != CachingHint.Cache)
            {
                Log.LogEvidence("expected value of CachingHint.Cache, but we got " + cachingHint);
                result = TestResult.Fail;
            }
            else
            {
                // Get the minimum and maximum relative sizes for regenerating the tiled brush.
                minimum = RenderOptions.GetCacheInvalidationThresholdMinimum(Window.Background);
                maximum = RenderOptions.GetCacheInvalidationThresholdMaximum(Window.Background);

                if (minimum != _thresholdMinValue || maximum != _thresholdMaxValue)
                {
                    // save out our data
                    Log.LogEvidence("expected minimum was " + _thresholdMinValue + " we got " + minimum);
                    Log.LogEvidence("expected maximum was " + _thresholdMaxValue + " we got " + maximum);
                    result = TestResult.Fail;
                }
            }

            return result;
        }

        /// <summary>
        /// Verify Rendering against known good render
        /// </summary>
        /// <returns>whether the render was good or wrong</returns>
        private TestResult VerifyVisuals()
        {
            ImageAdapter capturedSmallAdapter = new ImageAdapter(_capturedSmall);
            ImageAdapter capturedMiddleAdapter = new ImageAdapter(_capturedMiddle);
            ImageAdapter capturedLargeAdapter = new ImageAdapter(_capturedLarge);
            ImageAdapter expectedSmallAdapter = new ImageAdapter(_expectedSmall);
            ImageAdapter expectedMiddleAdapter = new ImageAdapter(_expectedMiddle);
            ImageAdapter expectedLargeAdapter = new ImageAdapter(_expectedLarge);

            TestResult resultSmall = CompareAndLogImages(capturedSmallAdapter, expectedSmallAdapter, _expectedSmallFile);
            TestResult resultMiddle = CompareAndLogImages(capturedMiddleAdapter, expectedMiddleAdapter, _expectedMiddleFile);
            TestResult resultLarge = CompareAndLogImages(capturedLargeAdapter, expectedLargeAdapter,_expectedLargeFile);

            if (resultLarge == TestResult.Pass && resultMiddle == TestResult.Pass && resultSmall == TestResult.Pass)
            {
                return TestResult.Pass;
            }

            return TestResult.Fail;
        }

        private TestResult CompareAndLogImages(ImageAdapter capturedAdapter, ImageAdapter expectedAdapter, string fileToLog)
        {
            ImageComparator comparator = new ImageComparator();
            bool imagesMatch = comparator.Compare(capturedAdapter, expectedAdapter, true);

            //if we fail, write out the before and after shots.            
            if (imagesMatch)
            {
                LogComment("Images match");
            }
            else
            {
                LogComment("Images do not match");
            }

            //unexpected results? log everything and fail.
            if (!imagesMatch)
            {
                string loggedCaptureFile = fileToLog + "-captured.png";

                ImageUtility.ToImageFile(capturedAdapter, loggedCaptureFile);

                Log.LogFile(loggedCaptureFile);
                Log.LogFile(fileToLog);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
        #endregion
        
        #region Helper Methods
        
        /// <summary>
        /// scale the window and capture it
        /// </summary>
        /// <returns></returns>
        private Bitmap ScaleAndCapture(double scale)
        {
            LogComment("Adjusting scale by factor of " + scale);
            Window.Height *= scale;
            Window.Width *= scale;

            LogComment("Waiting for render");
            WaitForRender();

            //capture the image
            LogComment("Capturing image");
            return ImageUtility.CaptureElement(Window);
        }

        /// <summary>
        /// Wait for window to render
        /// </summary>
        private void WaitForRender()
        {
            WaitFor(500); //half a second should keep us safe through heavily instrumented runs.
        }

        #endregion

        #region Members
        private string _imageFile;
        private Bitmap _capturedLarge;
        private Bitmap _capturedMiddle;
        private Bitmap _capturedSmall;
        private Bitmap _expectedLarge;
        private Bitmap _expectedMiddle;
        private Bitmap _expectedSmall;
        private string _expectedLargeFile;
        private string _expectedMiddleFile;
        private string _expectedSmallFile;
        private double _thresholdMinValue;
        private double _thresholdMaxValue;
        #endregion
    }
}


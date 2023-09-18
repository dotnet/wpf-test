// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: ClearTypeHint testing
 ********************************************************************/
using System.Drawing;
using System.IO;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Verification for ClearTypeHint, which allows us to force Cleartype rendering when the 
    ///  framework would normally disallow it.
    /// </summary>
    [Test(2, "ClearType", "ClearTypeHintDev10", // Positional variables for the subarea and name.
        Area = "2D", // Named variable for the area.
        SupportFiles = @"FeatureTests\Text\Data\times.ttf",
        Description = "Verification for ClearTypeHint")] // Named variable for the desc.
    public class ClearTypeHintDev10 : WindowTest
    {
        /// <summary>
        /// Constructor
        /// Takes these inputs:
        /// 1. The full path and name of the xaml file to load and render.
        /// 2. Whether we expect the text to render with cleartype.
        /// 3. The row in the image to scan to detect cleartype - this is unfortunately different for each case
        /// 
        /// </summary>
        #region variations
        [Variation("ClearTypeHint-01.xaml", true, true, 40, false, SupportFiles=@"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-01.xaml")]
        [Variation("ClearTypeHint-02.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-02.xaml")]
        [Variation("ClearTypeHint-03.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-03.xaml")]
        [Variation("ClearTypeHint-04.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-04.xaml")]
        [Variation("ClearTypeHint-05.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-05.xaml")]
        [Variation("ClearTypeHint-06.xaml", true, true, 40, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-06.xaml")]
        [Variation("ClearTypeHint-07.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-07.xaml")]
        [Variation("ClearTypeHint-08.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-08.xaml")]
        [Variation("ClearTypeHint-09.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-09.xaml")]
        [Variation("ClearTypeHint-10.xaml", true, true, 8, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-10.xaml")]
        [Variation("ClearTypeHint-11.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-11.xaml")]
        [Variation("ClearTypeHint-12.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-12.xaml")]
        [Variation("ClearTypeHint-13.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-13.xaml")]
        [Variation("ClearTypeHint-14.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-14.xaml")]
        [Variation("ClearTypeHint-15.xaml", true, true, 40, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-15.xaml")]
        [Variation("ClearTypeHint-16.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-16.xaml")]
        [Variation("ClearTypeHint-17.xaml", true, true, 50, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-17.xaml")]
        [Variation("ClearTypeHint-18.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-18.xaml")]
        [Variation("ClearTypeHint-19.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-19.xaml")]
        [Variation("ClearTypeHint-20.xaml", true, true, 50, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-20.xaml")]
        [Variation("ClearTypeHint-21.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-21.xaml")]
        [Variation("ClearTypeHint-22.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-22.xaml")]
        //[Variation("ClearTypeHint-23.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-23.xaml")]
        [Variation("ClearTypeHint-24.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-24.xaml")]
        [Variation("ClearTypeHint-25.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-25.xaml")]
        [Variation("ClearTypeHint-26.xaml", true, true, 30, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-26.xaml")]
        [Variation("ClearTypeHint-27.xaml", true, true, 5, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-27.xaml")]
        [Variation("ClearTypeHint-28.xaml", true, true, 5, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-28.xaml")]
        [Variation("ClearTypeHint-29.xaml", true, true, 5, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-29.xaml")]
        [Variation("ClearTypeHint-30.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-30.xaml")]
        [Variation("ClearTypeHint-31.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-31.xaml")]
        [Variation("ClearTypeHint-32.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-32.xaml")]
        [Variation("ClearTypeHint-33.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-33.xaml")]
        [Variation("ClearTypeHint-34.xaml", true, true, 7, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-34.xaml")]
        [Variation("ClearTypeHint-35.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-35.xaml")]
        [Variation("ClearTypeHint-36.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-36.xaml")]
        [Variation("ClearTypeHint-37.xaml", true, true, 10, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-37.xaml")]
        [Variation("ClearTypeHint-38.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-38.xaml")]
        [Variation("ClearTypeHint-39.xaml", true, true, 40, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-39.xaml")]
        [Variation("ClearTypeHint-40.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-40.xaml")]
        [Variation("ClearTypeHint-41.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-41.xaml")]
        [Variation("ClearTypeHint-42.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-42.xaml")]
        [Variation("ClearTypeHint-43.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-43.xaml")]
        [Variation("ClearTypeHint-44.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-44.xaml")]
        [Variation("ClearTypeHint-45.xaml", true, true, 50, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-45.xaml")]
        [Variation("ClearTypeHint-46.xaml", true, true, 40, true, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-46.xaml")]
        [Variation("ClearTypeHint-47.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-47.xaml")]
        [Variation("ClearTypeHint-48.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-48.xaml")]
        [Variation("ClearTypeHint-49.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-49.xaml")]
        [Variation("ClearTypeHint-50.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-50.xaml")]
        [Variation("ClearTypeHint-51.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-51.xaml")]
        [Variation("ClearTypeHint-52.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-52.xaml")]
        [Variation("ClearTypeHint-53.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-53.xaml")]
        [Variation("ClearTypeHint-54.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-54.xaml")]
        [Variation("ClearTypeHint-55.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-55.xaml")]
        [Variation("ClearTypeHint-56.xaml", true, true, 32, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-56.xaml")]
        [Variation("ClearTypeHint-57.xaml", true, true, 35, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-57.xaml")]
        [Variation("ClearTypeHint-58.xaml", true, true, 50, false, SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ClearTypeHint-58.xaml")]
        #endregion
        public ClearTypeHintDev10(  string  xamlFile,
                                    bool    clearTypeExpected,
                                    bool    transparentWindow,
                                    int     rowToScan,
                                    bool    RTBRequested
                                    )
            : base(transparentWindow) // construct the base class (WindowTest) with a transparent window
        {
            this._xamlFile = xamlFile;
            this._clearTypeExpected = clearTypeExpected;
            this._transparentWindow = transparentWindow;
            this._rowToScan = rowToScan;
            this._RTBRequested = RTBRequested;

            InitializeSteps += new TestStep(LoadXaml);

            if (RTBRequested)
            {
                RunSteps += new TestStep(RenderContent);
            }
            else
            {   
                RunSteps += new TestStep(RenderContentToRTB);
            }

            RunSteps += new TestStep(CaptureTarget);

            RunSteps += new TestStep(Verification);
        }

        #region test steps
        /// <returns></returns>
        private TestResult LoadXaml()
        {
            XmlDocument xamldoc = new XmlDocument();
            Status("Loading xaml from " + _xamlFile);
            xamldoc.Load(_xamlFile);
            Status("xaml loaded;");
            _xaml = xamldoc.OuterXml;
            Status(_xaml);
            return TestResult.Pass;
        }
        
        /// <returns></returns>
        private TestResult RenderContent()
        {
            Status("Rendering ...");
            Stream stream = IOHelper.ConvertTextToStream(_xaml);
            Window.Content = XamlReader.Load(stream) as FrameworkElement;
            //transparency is set in the base class ctor
            Window.Height = 100;
            Window.Width = 200;
            WaitFor(100);
            return TestResult.Pass;
        }
        /// <returns></returns>
        private TestResult RenderContentToRTB()
        {
            Status("Rendering ...");
            Stream stream = IOHelper.ConvertTextToStream(_xaml);
            Window.Content = XamlReader.Load(stream) as FrameworkElement;
            //transparency is set in the base class ctor
            Window.Height = 100;
            Window.Width = 200;
            WaitFor(100);

            //render to an RTB if requested
            double DPI = Microsoft.Test.Display.Monitor.Dpi.x;//x and y will always have same DPI - ask Gerhard
            RenderTargetBitmap RTB = new RenderTargetBitmap((int)Window.Width, (int)Window.Height, DPI, DPI, PixelFormats.Default);
            RTB.Render(Window);
            //now that we've rendered the Window contents into the render target bitmap,
            //use those as the contents of the window so that we can capture it.
            System.Windows.Controls.Image img = new System.Windows.Controls.Image();
            img.Source = RTB;
            Window.Content = img;
            WaitFor(100);//wait again for the render.
            
            return TestResult.Pass;
        }

        /// <returns>TestResult</returns>
        private TestResult CaptureTarget()
        {
            Status("Capturing target element...");
            _capture = ImageUtility.CaptureElement(Window.Content as UIElement);
            return TestResult.Pass;
        }

        /// <summary>
        /// Detect cleartype and compare to expected result.
        /// Saves the rendered image and xaml if there is an error.
        /// </summary>
        /// <returns>TestResult</returns>
        public TestResult Verification()
        {
            bool clearTypeDetected = false;
            //walk through the bitmap and detect blue-red shift (or just non greyscale)
            Status("Checking all pixels on midline in the rendered text for red-blue shift.");
            clearTypeDetected = ClearTypeDetector.CheckForRedBlueShift(_capture, this._rowToScan);

            TestResult result = TestResult.Unknown;

            if (clearTypeDetected == _clearTypeExpected)
            {
                Status("Results match expected behaviour - PASS");
                result = TestResult.Pass;
            }
            if (clearTypeDetected != _clearTypeExpected)
            {
                Status("Results do not match expected behaviour - FAIL");
                //log the image  
                //we can open up the xaml file in xamlpad!
                string saveFile = _xamlFile + ".bmp";
                Status("Saving captured image to " + saveFile);
                ImageAdapter img = new ImageAdapter(_capture);
                ImageUtility.ToImageFile(img, saveFile);
                Log.LogFile(saveFile);
                Log.LogFile(_xamlFile);
                result = TestResult.Fail;
            }
            Status("Cleartype detected: [" + clearTypeDetected + "] Cleartype expected: [" + _clearTypeExpected + "]");
            return result;
        }
        #endregion

        #region member variables
        private Bitmap _capture;                     // captured image of our rendered text
        private string _xaml;                        // string of our factory-constructed xaml.
        private string _xamlFile;                    // our test collateral

        //Factors from test spec
        private bool _transparentWindow;             // should the root window be transparent
        private bool _clearTypeExpected;             // do we expect CT to be present
        private int _rowToScan;                      // the row of the rendered image to scan for CT
        private bool _RTBRequested;                  // should this test perform a RenderTargetBitmap step
        #endregion
    }

}


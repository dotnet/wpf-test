// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//****************************************************************** 
//* Purpose: Test TS Scroll Acceleration feature
//******************************************************************
using System;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Threading;

namespace Microsoft.Test.TS
{
    /// <summary>
    /// This class is the base class for any class to be used for testing the Scroll Acceleration feature.
    /// </summary>
    public class ScrollAccelerationTestBase
    {
        #region Constructor

        /// <summary>
        /// The constructor.
        /// </summary>
        /// <param name="log">The test log to be used for any logging.</param>
        /// <param name="parameters">The test parameters.</param>
        public ScrollAccelerationTestBase(TestLog log, PropertyBag parameters)
        {
            this.log = log;
            this.testParameters = parameters;

            SetParameters(testParameters);
        }

        #endregion

        #region Methods

        /// <summary>
        /// Performs any necessary steps (if any) on a test window before starting the test.
        /// </summary>
        /// <param name="window">The test window.</param>
        public virtual void PreTestStep(Window window) { }

        /// <summary>
        /// For a test case type, it returns the name of the canvas that has scroll acceleration.
        /// </summary>
        /// <returns>The name of the canvas with scroll acceleration.</returns>
        public virtual string GetAcceleratedCanvasName()
        {
            return acceleratedCanvasName;
        }

        /// <summary>
        /// Given a test window, this method will capture the test result. Depending on the type of the test, this may be taking the snapshot of the window.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <returns>The captured test result.</returns>
        public virtual object GetTestResult(Window window)
        {
            return null;
        }

        /// <summary>
        /// Compares the results of two tests and throws a TestValidationException if the results are not the same.
        /// Depending on the type of the test, this may be an image comparison step.
        /// </summary>
        /// <param name="referenceTestResult">The captured test result of the reference window.</param>
        /// <param name="renderedTestResult">The captured test result of the test window.</param>
        public virtual void CompareTestResults(object referenceTestResult, object renderedTestResult) { }

        /// <summary>
        /// Runs a type of a test case -- implemented by the sub-classes
        /// </summary>
        public void RunTest()
        {
            // Run the test on a window that is rendered in HW.
            // It will be considered as the reference window.
            hwRenderedWindow = CreateWindow();
            log.LogStatus("Reference window created.");

            PreTestStep(hwRenderedWindow);

            string canvasName = GetAcceleratedCanvasName();

            SetXamlContent(hwRenderedWindow, false, canvasName);

            PerformScrollAction(hwRenderedWindow, canvasName);
            log.LogStatus("Scroll action performed on reference window.");

            referenceTestResult = GetTestResult(hwRenderedWindow);

            CloseWindow(hwRenderedWindow);
            log.LogStatus("Reference window closed.");

            // Run the test on a window that is rendered in SW.
            // It will be compared against the reference window above.
            swRenderedWindow = CreateWindow();
            log.LogStatus("Opted-In window created.");

            PreTestStep(swRenderedWindow);

            SetXamlContent(swRenderedWindow, true, canvasName);

            //
            // 




            PerformScrollAction(swRenderedWindow, canvasName);
            log.LogStatus("Scroll action performed on Opted-In window.");

            //
            // 







            renderedTestResult = GetTestResult(swRenderedWindow);

            CloseWindow(swRenderedWindow);
            log.LogStatus("Opted-In window closed.");

            //
            // 








            CompareTestResults(referenceTestResult, renderedTestResult);
        }

        /// <summary>
        /// Creates a window for a test. The size of the window depends on the test parameters. If no
        /// size parameters are provided, a default size is used. The window will automatically have 
        /// a content which is composed of a regular canvas having another canvas as its child.
        /// The child canvas will be used to turn scroll acceleration feature on or off when scrolling.
        /// </summary>
        /// <returns>A window object.</returns>
        protected Window CreateWindow()
        {
            Window window = new Window();
            window.Width = windowWidth;
            window.Height = windowHeight;
            window.Top = 10;
            window.Left = 10;
            window.Topmost = true;
            window.SnapsToDevicePixels = true;
            RenderOptions.SetEdgeMode(window, EdgeMode.Aliased);
            if (isLayeredWindow)
            {
                window.AllowsTransparency = true;
                window.WindowStyle = WindowStyle.None;
                window.Background = Brushes.Transparent;
            }
            window.Show();
            return window;
        }

        /// <summary>
        /// Sets the content of a test window.
        /// </summary>
        /// <param name="window">The test window.</param>
        /// <param name="SWRendering">Indicates whether the rendering mode of the input window is to be set to SW rendering.</param>
        /// <param name="scrollAcceleratedCanvasName">The name of the canvas that has scroll-acceleration on.</param>
        protected void SetXamlContent(Window window, bool SWRendering, string scrollAcceleratedCanvasName)
        {
            window.Content = LoadScrollableContent(xamlFilePath);
            ScrollAcceleratedCanvas acceleratedCanvas = (ScrollAcceleratedCanvas)GetElementByName(window, scrollAcceleratedCanvasName);

            if (scrollableArea != null)
            {
                // Override the default ScrollableArea (if any) with the one provided in the test argument (if any)
                acceleratedCanvas.SetVisualScrollableAreaClip(Rect.Parse(scrollableArea));
            }

            if (SWRendering == true)
            {
                // Use software rendering mode to emulate running under terminal services.
                RenderingModeHelper.SetRenderingMode(window, RenderingMode.Software);
            }
            else
            {
                // Use default rendering mode
                RenderingModeHelper.SetRenderingMode(window, RenderingMode.Default);
            }

            // Wait a moment so that the window redraws its new content and also switches its rendering mode 
            DispatcherHelper.DoEvents(intervalForRender);
        }

        /// <summary>
        /// Loads the content of the test from a provided xaml file.
        /// </summary>
        /// <returns>A FrameworkElement which is loaded from the xaml file.</returns>
        protected FrameworkElement LoadScrollableContent(string filePath)
        {
            FileStream xamlStream = File.OpenRead(filePath);
            FrameworkElement element = XamlReader.Load(xamlStream) as FrameworkElement;
            return element;
        }

        /// <summary>
        /// Given a test window, it will scroll its content Up, Down, Left, Right, Up-Left, Up-Right, Down-Left, or Down-Right.
        /// </summary>
        /// <param name="window">The test window to perform the action on.</param>
        protected void PerformScrollAction(Window window, string scrollAcceleratedCanvasName)
        {
            ScrollAcceleratedCanvas acceleratedCanvas = (ScrollAcceleratedCanvas)GetElementByName(window, scrollAcceleratedCanvasName);

            switch (testAction)
            {
                case "SCROLLDOWN":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) + scrollAmount);
                    break;

                case "SCROLLUP":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) - scrollAmount);
                    break;

                case "SCROLLLEFT":
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) - scrollAmount);
                    break;

                case "SCROLLRIGHT":
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) + scrollAmount);
                    break;

                case "SCROLLDOWNLEFT":
                case "SCROLLLEFTDOWN":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) + scrollAmount);
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) - scrollAmount);
                    break;

                case "SCROLLDOWNRIGHT":
                case "SCROLLRIGHTDOWN":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) + scrollAmount);
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) + scrollAmount);
                    break;

                case "SCROLLUPLEFT":
                case "SCROLLLEFTUP":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) - scrollAmount);
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) - scrollAmount);
                    break;

                case "SCROLLUPRIGHT":
                case "SCROLLRIGHTUP":
                    Canvas.SetTop(acceleratedCanvas, Canvas.GetTop(acceleratedCanvas) - scrollAmount);
                    Canvas.SetLeft(acceleratedCanvas, Canvas.GetLeft(acceleratedCanvas) + scrollAmount);
                    break;

                default:
                    throw new TestValidationException(string.Format("Unrecognized Test Action: {0}.", testAction));
            }

            DispatcherHelper.DoEvents(intervalForRender);
        }

        /// <summary>
        /// Given a window object and an element name, this method traverses its logical view tree and tries to find the element by that name.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <param name="elementName">An element name.</param>
        /// <returns>The element by the given name (or null if none exists).</returns>
        protected FrameworkElement GetElementByName(Window window, string elementName)
        {
            FrameworkElement element = (FrameworkElement)window.Content;
            return (FrameworkElement)element.FindName(elementName);
        }

        /// <summary>
        /// Given a test window, it will take the snapshot of its content.
        /// </summary>
        /// <param name="window">A test window.</param>
        /// <returns>The snapshot of the window content.</returns>
        protected ImageAdapter TakeSnapshot(Window window)
        {
            ImageAdapter adapter = null;

            if (window.Content != null)
            {
                System.Drawing.Bitmap capture = ImageUtility.CaptureElement((UIElement)window.Content);
                adapter = new ImageAdapter(capture);
            }

            return adapter;
        }




        protected void TurnDirtyRegionTintingOn(Window window)
        {
        }




        protected void TurnDirtyRegionTintingOff(Window window)
        {
        }

        /// <summary>
        /// Given a reference and a rendered image, it will compare the two for any mismatches. It will save the
        /// images andtheir difference, should there be any mismatches or if the user forces them to be saved.
        /// </summary>
        /// <param name="referenceImg">The reference image.</param>
        /// <param name="referenceImgFileName">The file name to be used for saving the reference image.</param>
        /// <param name="renderedImg">The rendered image.</param>
        /// <param name="renderedImgFileName">The file name to be used for saving the rendered image.</param>
        /// <param name="diffImgFileName">The file name to be used for saving the difference image.</param>
        protected void CompareSnapshots(ImageAdapter referenceImg, string referenceImgFileName, ImageAdapter renderedImg, string renderedImgFileName, string diffImgFileName)
        {
            ImageComparator comparator = CreateImageComparator(toleranceFilePath);
            bool isIdentical = comparator.Compare(renderedImg, referenceImg);

            //Save files for analysis if specified, or if image comparison failed. 
            if (forceSaveFiles || !isIdentical)
            {
                ImageAdapter diffImg = new ImageAdapter(comparator.GetErrorDifference(ErrorDifferenceType.IgnoreAlpha));

                // Set the alpha value to completely opaque
                for (int x = 0; x < diffImg.Width; x++)
                {
                    for (int y = 0; y < diffImg.Height; y++)
                    {
                        diffImg[x, y].A = byte.MaxValue;
                    }
                }

                ImageUtility.ToImageFile(renderedImg, renderedImgFileName);
                log.LogFile(renderedImgFileName);

                ImageUtility.ToImageFile(referenceImg, referenceImgFileName);
                log.LogFile(referenceImgFileName);

                ImageUtility.ToImageFile(diffImg, diffImgFileName);
                log.LogFile(diffImgFileName);

                diffImg = null;
            }

            if (!isIdentical)
            {
                int numMismatches = comparator.MismatchingPoints.NumMismatchesAboveLevel(1);
                throw new TestValidationException(string.Format("Image comparison failed: {0} mismatches.", numMismatches));
            }
        }

        /// <summary>
        /// Clean up after ourselves before exiting.
        /// </summary>
        protected void Cleanup()
        {
            CloseWindow(swRenderedWindow);
            CloseWindow(hwRenderedWindow);
            swRenderedWindow = null;
            hwRenderedWindow = null;
            referenceTestResult = null;
            renderedTestResult = null;
            referenceDirtyRegionImg = null;
            renderedDirtyRegionImg = null;
        }

        /// <summary>
        /// Closes a test window and sets its content to null.
        /// </summary>
        /// <param name="window">A test window.</param>
        protected void CloseWindow(Window window)
        {
            if (window != null)
            {
                window.Content = null;
                window.Close();
            }
        }

        /// <summary>
        /// Loads the test parameters that the user has provided and initializes appropriate fields.
        /// </summary>
        /// <param name="parameters">The test parameters.</param>
        private void SetParameters(PropertyBag parameters)
        {
            if (!string.IsNullOrEmpty(parameters["XamlFilePath"]))
            {
                xamlFilePath = parameters["XamlFilePath"];
                log.LogStatus(string.Format("Xaml file: {0}.", xamlFilePath));
            }
            else
            {
                throw new TestValidationException("Must specify XamlFilePath.");
            }

            if (!string.IsNullOrEmpty(parameters["TestAction"]))
            {
                testAction = parameters["TestAction"].Trim().ToUpper(System.Globalization.CultureInfo.InvariantCulture);
                log.LogStatus(string.Format("Test Action: {0}.", testAction));
            }
            else
            {
                throw new TestValidationException("Must specify TestAction.");
            }

            if (!string.IsNullOrEmpty(parameters["ToleranceFilePath"]))
            {
                toleranceFilePath = parameters["ToleranceFilePath"];
                log.LogStatus(string.Format("Tolerance profile file: {0}.", toleranceFilePath));
            }

            if (!string.IsNullOrEmpty(parameters["ScrollableArea"]))
            {
                scrollableArea = parameters["ScrollableArea"];
                log.LogStatus(string.Format("ScrollableArea: {0}.", scrollableArea));
            }

            windowWidth = defaultWidth;
            windowHeight = defaultHeight;
            if (!string.IsNullOrEmpty(parameters["WindowWidth"]))
            {
                double.TryParse(parameters["WindowWidth"], out windowWidth);
            }
            if (!string.IsNullOrEmpty(parameters["WindowHeight"]))
            {
                double.TryParse(parameters["WindowHeight"], out windowHeight);
            }
            log.LogStatus(string.Format("Window Width,Height: {0},{1}", windowWidth, windowHeight));

            scrollAmount = defaultScrollAmount;
            if (!string.IsNullOrEmpty(parameters["ScrollAmount"]))
            {
                double.TryParse(parameters["ScrollAmount"], out scrollAmount);
            }
            log.LogStatus(string.Format("ScrollAmount: {0}", scrollAmount));

            bool.TryParse(parameters["ForceSaveFiles"], out forceSaveFiles);
            bool.TryParse(parameters["LayeredWindow"], out isLayeredWindow);
        }

        /// <summary>
        /// Given a tolerance file, it will create an image comparator.
        /// </summary>
        /// <param name="toleranceFile">The path to a tolerance file.</param>
        /// <returns>An image comparator.</returns>
        private ImageComparator CreateImageComparator(string toleranceFile)
        {
            ImageComparator comparator;
            if (File.Exists(toleranceFile))
            {
                CurveTolerance tolerance = new CurveTolerance();
                tolerance.LoadTolerance(toleranceFile);
                comparator = new ImageComparator(tolerance);
                log.LogStatus("Using custom tolerance (" + toleranceFile + ")");
            }
            else
            {
                comparator = new ImageComparator();
                log.LogStatus("Using default tolerance");
            }
            return comparator;
        }

        #endregion

        #region Data

        protected const string parentCanvasName = "PART_ParentCanvas";
        protected const string acceleratedCanvasName = "PART_ScrollAcceleratedCanvas";
        private const double defaultWidth = 300, defaultHeight = 300; // The default window size
        private const double defaultScrollAmount = 50; // By default scroll by 50 pixels
        protected const int intervalForRender = 500; // Time to wait for window rendering to be completed, in ms. 
        protected double windowWidth, windowHeight; // The actual window size (might be different than default size)
        protected bool isLayeredWindow;
        protected double scrollAmount; // The amount (in pixels) by which the content should be scrolled
        protected bool forceSaveFiles; // Indicates whether to force saving the snapshots and diff images
        protected TestLog log;
        protected PropertyBag testParameters;
        protected string scrollableArea; // The scrollable area clip
        protected string toleranceFilePath; // The path to a tolerance profile file
        protected string xamlFilePath; // The path to a Xaml file containing the content of a window
        protected string testAction;   // The type of action to be perfomed for a test
        protected Window swRenderedWindow;  // The window whose rendering mode is set to SW
        protected Window hwRenderedWindow;  // The window whose rendering mode is set to HW
        protected object referenceTestResult; // Holds the test result of the window without scroll acceleration
        protected object renderedTestResult;  // Holds the test result of the window with scroll acceleration
        protected ImageAdapter referenceDirtyRegionImg; // Holds the reference dirty region
        protected ImageAdapter renderedDirtyRegionImg;  // Holds the rendered dirty region
        
        #endregion
    }

    /// <summary>
    /// Implements a canvas where the scroll-acceleration feature can be
    /// turned on or off by setting its VisualScrollableAreaClip.
    /// </summary>
    public class ScrollAcceleratedCanvas : Canvas
    {
        #region Constructor
        
        public ScrollAcceleratedCanvas()
        {
            // No scrollable area is provided, so disable Scroll Acceleration by default.
            SetVisualScrollableAreaClip(null);
        }

        public ScrollAcceleratedCanvas(Rect? scrollableArea)
        {
            SetVisualScrollableAreaClip(scrollableArea);
        }

        #endregion

        #region Methods

        public void SetVisualScrollableAreaClip(Rect? scrollableArea)
        {
            VisualScrollableAreaClip = scrollableArea;
        }

        private static void ScrollableAreaChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ScrollAcceleratedCanvas)d).VisualScrollableAreaClip = (Rect?)e.NewValue;
        }

        #endregion

        #region Properties

        public Rect? ScrollableArea
        {
            get { return (Rect?)GetValue(ScrollableAreaProperty); }
            set { SetValue(ScrollableAreaProperty, value); }
        }

        public static readonly DependencyProperty ScrollableAreaProperty = DependencyProperty.Register("ScrollableArea", typeof(Rect?), typeof(ScrollAcceleratedCanvas),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.AffectsRender, new PropertyChangedCallback(ScrollableAreaChanged)));

        #endregion
    }
}

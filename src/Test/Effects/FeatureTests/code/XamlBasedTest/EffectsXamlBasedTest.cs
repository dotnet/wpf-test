// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test hardware accelerated effects with xaml files.
 * Owner: Microsoft 
 * Contributors: Microsoft
 ********************************************************************/
using System;
using System.Xml;
using System.Windows;
using Microsoft.Test.Logging;
using System.Drawing;
using Microsoft.Test.Display;
using Microsoft.Test.Discovery;
using System.IO;
using System.Windows.Interop;
using Microsoft.Test.Serialization;
using System.Collections.Generic;
using Microsoft.Test.Threading;
using System.Windows.Markup;
using Microsoft.Test.Win32;
using System.Windows.Media;
using Microsoft.Test.Graphics;
using Microsoft.Test.VisualVerification;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// xaml base test for Effects. 
    /// verify with visual validation, move window to the second window and 
    /// verify, and test hittesting and serialization.
    /// </summary>
    public class EffectsXamlBasedTest
    {
        #region Methods
        
        /// <summary>
        /// create TestLog and run RunTest. 
        /// This is the entry point for xtc adaptor. 
        /// </summary>
        public static void Run()
        {
            EffectsXamlBasedTest test = new EffectsXamlBasedTest();
            using (test.log = new TestLog("Effects Xamlbase Test"))
            {
                try
                {
                    test.RunTest(DriverState.DriverParameters);
                    test.log.Result = TestResult.Pass;
                }
                catch (Exception e)
                {
                    test.log.LogEvidence(string.Format("Got an exception: \n{0}", e.ToString()));
                    test.log.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Run a test with parameters. Result would be logged 
        /// to the testLog.
        /// </summary>
        /// <param name="testLog"></param>
        /// <param name="parameters"></param>
        protected internal void RunTest(PropertyBag parameters)
        {
            try
            {
                log = TestLog.Current;

                //In the case environment variable NewMasterImagesPath has been defined as a non-empty string, 
                //just generate master image to that folder
                _locationOfMasterImages = System.Environment.GetEnvironmentVariable("NewMasterImagesPath");
                createMasterImagesOnly = !string.IsNullOrEmpty(_locationOfMasterImages);
                SetParameters(parameters);
                Initialize();

                // parse xaml file name
                FileStream xamlStream = File.OpenRead(_xamlFileName);
                FrameworkElement content = XamlReader.Load(xamlStream) as FrameworkElement;
                log.LogStatus("Content loaded from xaml.");

                //Create a Window
                CreateWindow(content);
                log.LogStatus("Window Created.");
                
                //Validate rendered image with master image.
                ValidateWindow();
                log.LogStatus("First Visual Validation passed.");

                if (!createMasterImagesOnly)
                {
                    //Test on multi-monitor if available. 
                    if (!_disableMultiMonitorTesting && Monitor.IsMultiMonAvailable())
                    {
                        TestMultiMonitor();
                    }
                    else
                    {
                        log.LogStatus("MultiMonitor Testing not run.");
                    }

                    if (!_disableSerializationTesting)
                    {
                        TestSerialization();                        
                    }
                    else
                    {
                        log.LogStatus("Disabled Serialization Testing.");
                    }

                    //Test in alternative renderingMode. 
                    if (!_disableAlternativeRenderingModeTesting)
                    {
                        TestAlternativeRenderingMode();
                    }
                    else
                    {
                        log.LogStatus("Disabled AlternativeRenderingMode Testing.");
                    }

                    //Test HitTesting, should alway be the last one, since hittesting test changes the content of the window. 
                    if (!_disableHitTestingTesting)
                    {
                        TestHitTesting();
                    }
                    else
                    {
                        log.LogStatus("Disabled Hittesting Testing.");
                    }
                }
                else
                {
                    log.LogStatus("Just create master images.");
                }
            }
            finally
            {
                Cleanup();
            }
        }

        /// <summary>
        /// Initialization, log initialzation information, create an ImageComparator
        /// and ImageAdapter for master image. 
        /// </summary>
        protected virtual void Initialize()
        {
            log.Result = TestResult.Unknown;

            log.LogStatus("Xaml: " + _xamlFileName);
            log.LogStatus("Master: " + masterImageFile);
            log.LogStatus("RenderingMode: " + _initialRenderingMode);

            if (String.Compare(_toleranceFilePath, "NoVisualValidation", StringComparison.InvariantCultureIgnoreCase) != 0)
            {
                _verifier = new SnapshotHistogramVerifier(Histogram.FromFile(_toleranceFilePath));
            }

            if (!createMasterImagesOnly)
            {
                if (File.Exists(masterImageFile))
                {
                    masterImageSnapshot = Snapshot.FromFile(masterImageFile);
                }
                else
                {
                    throw new TestSetupException(string.Format("master image file: {0} not found.", masterImageFile));
                }
            }
        }

        /// <summary>
        /// Render window in alternative RenderingMode 
        /// and do visual validation. 
        /// </summary>
        private void TestAlternativeRenderingMode()
        {
            currentRenderingMode = _initialRenderingMode == RenderingMode.Default ? RenderingMode.Software : RenderingMode.Default;

            RenderMode renderMode = currentRenderingMode == RenderingMode.Software ? RenderMode.SoftwareOnly : RenderMode.Default;
            
            // HACK: SetRenderingMode doesn't work stably. 
            EffectsTestHelper.SetRenderMode(_window, renderMode);
            log.LogStatus("Wait till SystemIdle for Render Mode to be applied." +
            "Note: This test used to fail because Application had not finished rendering when a snapshot was taken." +
            "We now wait for Dispatcher to be at DispatcherPriority.SystemIdle before we take the snapshot. If this test continues to fail " +
            "If this test fails again because the snapshots did not match, there is some other issue with it.");
            
            DispatcherHelper.DoEvents(0); 
            ValidateWindow();
            log.LogStatus("Alternative RenderingMode test passed.");
        }
        /// <summary>
        /// Test that hittesting result is the same for w / o bitmapeffect.
        /// </summary>
        private void TestHitTesting()
        {
            FrameworkElement mainElement = GetElementForHitTesting();
            
            int[,] result1 = GetHitTestingResult(mainElement);
            log.LogStatus("Removing Effect...");
            ShowWindowWithoutEffects();

            mainElement = GetElementForHitTesting();
            int[,] result2 = GetHitTestingResult(mainElement as FrameworkElement);

            CompareArray(result1, result2);
            log.LogStatus("Hittesting passed.");
        }

        /// <summary>
        /// Get the element on which to do the hittesting. 
        /// </summary>
        /// <returns></returns>
        private FrameworkElement GetElementForHitTesting()
        {
            //For model base test, there is an element with name MainElement we could 
            //usd for hittesting.
            FrameworkElement mainElement = _window.FindName("MainElement") as FrameworkElement;
            if (mainElement != null)
            {
                return mainElement;
            }

            //otherwise, just test all the content of current window. 

            mainElement = _window.Content as FrameworkElement;

            return mainElement;
        }

        /// <summary>
        /// Create the window based on the same xaml file, but BitmapEffect properties have
        /// been removed. 
        /// </summary>
        private void ShowWindowWithoutEffects()
        {
            //close original window
            if (_window != null)
            {
                _window.Close();
            }
            
            //remove the BitmapEffect properties
            XmlDocument document = new XmlDocument();
            document.Load(_xamlFileName);
            XmlNodeList Nodes = document.SelectNodes("//*");
            foreach (XmlElement node in Nodes)
            {
                if(node != null && node.Name.Contains(".BitmapEffect"))
                {
                    node.ParentNode.RemoveChild(node);
                }
            }
            string noEffectXamlFileName = "NoEffects.xaml";
         
            document.Save(noEffectXamlFileName);
            FileStream stream = File.OpenRead(noEffectXamlFileName);
            FrameworkElement content = XamlReader.Load(stream) as FrameworkElement;
            
            //clean up temporary file.
            File.Delete(noEffectXamlFileName);

            CreateWindow(content);
        }

        /// <summary>
        /// Do Hittesting on each point in the element, and record the result in
        /// a two demension array. 
        /// </summary>
        /// <param name="element"></param>
        /// <returns></returns>
        private int[,] GetHitTestingResult(FrameworkElement element)
        {         
            int x = (int)element.Width;
            int y = (int)element.Height;

            if (x < 0 || y < 0) return null;

            int[,] visualArray = new int[x, y];

            for (int i = 0; i < x; ++i)
            {
                for (int j = 0; j < y; ++j)
                {
                    System.Windows.Media.HitTestResult hit =
                        System.Windows.Media.VisualTreeHelper.HitTest(element, new System.Windows.Point(i, j));
                    if (hit == null)
                    {
                        visualArray[i, j] = 0;  
                    }
                    else if (hit.VisualHit == element)
                    {
                        visualArray[i, j] = 1; 
                    }
                    else
                    {
                        visualArray[i, j] = 2; 
                    }
                }
            }
            return visualArray;
        }

        private void CompareArray(int[,] array1, int[,] array2)
        {
            if (array1 == null && array2 == null)
            {
                return;
            }

            if ((array1.GetLength(0) != array2.GetLength(0)) ||
                (array1.GetLength(1) != array2.GetLength(1)))
            {
                throw new TestValidationException("The array dimensions for analysis does not match up");
            }

            for (int i = 0; i < array1.GetLength(0); ++i)
            {
                for (int j = 0; j < array1.GetLength(1); ++j)
                {
                    if (array1[i, j] != array2[i, j])
                    {
                        throw new TestValidationException(string.Format("Hittest different at point {0}, {1}.", i, j));
                    }
                }
            }
        }

        /// <summary>
        /// Test serialization by serialize window content, 
        /// load the serialized into the a new window and do
        /// visual validation. 
        /// </summary>
        private void TestSerialization()
        {
            //close previous window
            _window.Close();                      

            //Serializing
            string serialized = SerializationHelper.SerializeObjectTree(_window.Content);
            log.LogStatus("Serialized: " + serialized);

            //Parsing
            FrameworkElement content = SerializationHelper.ParseXaml(serialized) as FrameworkElement;
            
            // Revert transform added in CreateWindow. To avoid rendering different image with different Dpi,
            // We have add a ScaleTransform before. However, this is not part of the content 
            // and need to be removed.
            content.LayoutTransform = Transform.Identity;

            //Validation
            CreateWindow(content);
            ValidateWindow();

            log.LogStatus("Serialization test passed. ");
        }

        /// <summary>
        /// Move the window to the second monitor and test. 
        /// </summary>
        private void TestMultiMonitor()
        {
            log.LogStatus("Start multi-monitor testing.");

            log.LogStatus("Get all monitors");
            Monitor[] monitors = Monitor.GetAllEnabled();

            log.LogStatus("Number of monitors: {0}, For each monitor:", monitors.Length);

            foreach (Monitor monitor in monitors)
            {
                //Don't need for primary monitor.
                if (monitor.IsPrimary) continue;

                log.LogStatus("Move the window onto non-primary monitor.");
                _window.Left = monitor.WorkingArea.left + EffectsTestHelper.LeftWindowOffset;
                _window.Top = monitor.WorkingArea.top + EffectsTestHelper.TopWindowOffset;

                log.LogStatus("Validate on non-primary monitor.");
                DispatcherHelper.DoEvents(_intervalForWindowMoving);
                ValidateWindow();

                // Get a straddling position

                Monitor primaryMonitor = Monitor.GetPrimary();

                double left = _window.Left;
                double top = _window.Top;

                if (monitor.Area.left == primaryMonitor.Area.right)
                {
                    left = primaryMonitor.Area.right - _window.Width / 2;
                }
                if (monitor.Area.right == primaryMonitor.Area.left - 1)
                {
                    left = monitor.Area.right - _window.Width / 2;
                }
                if (monitor.Area.top == primaryMonitor.Area.bottom)
                {
                    top = primaryMonitor.Area.bottom - _window.Height / 2;
                }
                if (monitor.Area.bottom == primaryMonitor.Area.top - 1)
                {
                    top = monitor.Area.bottom - _window.Height / 2;
                }

                if ((left != _window.Left) || (top != _window.Top))
                {
                    log.LogStatus("Validate a window spanning multi-monitor.");
                    _window.Left = left;
                    _window.Top = top;
                    DispatcherHelper.DoEvents(_intervalForWindowMoving);
                    ValidateWindow();
                }
                else
                {
                    log.LogStatus("Don't verify on this monitor since it is not adjacent to primary.");
                }
            }

            log.LogStatus("Multi-monitor test passed.");
        }

        /// <summary>
        /// Create a window with certain content.
        /// </summary>
        /// <param name="content"></param>
        private void CreateWindow(FrameworkElement content)
        {
            _window = EffectsTestHelper.CreateWindow(_windowWidth, _windowHeight, _isLayeredWindow, content);
            _window.Show();

            //assign RenderingMode
            currentRenderingMode = _initialRenderingMode;
            RenderingModeHelper.SetRenderingMode(_window, currentRenderingMode);
            // Wait for RenderMode to be applied.
            log.LogStatus("Wait till SystemIdle for Render Mode to be applied." +
                "Note: This test used to fail because Application had not finished rendering when a snapshot was taken." +
                "We now wait for Dispatcher to be at DispatcherPriority.SystemIdle before we take the snapshot. If this test continues to fail " +
                "If this test fails again because the snapshots did not match, there is some other issue with it.");

            DispatcherHelper.DoEvents(500);
        }

        /// <summary>
        /// Compare capured window with master image.
        /// </summary>
        /// <returns></returns>
        protected virtual void ValidateWindow()
        {
            ValidateWindow(masterImageSnapshot, masterImageFile);
        }

        protected void ValidateWindow(Snapshot master, string masterImageFileName)
        {
            Snapshot captureSnapshot = SnapshotHelper.SnapshotFromWindow(_window, WindowSnapshotMode.IncludeWindowBorder);
            
            //In the case to create master image, just save the rendered 
            //image, if not there yet.
            if (createMasterImagesOnly)
            {
                string masterImageFileFullPath = Path.Combine(_locationOfMasterImages, masterImageFileName);
                EffectsTestHelper.LogPngSnapshot(captureSnapshot, masterImageFileFullPath);
            }
            else
            {
                Snapshot diff = master.CompareTo(captureSnapshot);

                if ((_verifier != null) && (_verifier.Verify(diff) != VerificationResult.Pass))
                {
                    EffectsTestHelper.LogPngSnapshot(master, masterImageFileName);
                    
                    string renderedFileName = RenderedPrefix + masterImageFileName;

                    EffectsTestHelper.LogPngSnapshot(captureSnapshot, renderedFileName);
                    
                    throw new TestValidationException("Image comparison failed.");
                }
            }
        }

        /// <summary>
        /// Clean Window Comparator, and master image adaptor, and clean the window.
        /// </summary>
        protected virtual void Cleanup()
        {
            _verifier = null;
            masterImageSnapshot = null;
            if (_window != null)
            {
                _window.Close();
                _window = null;
            }
        }

        /// <summary>
        /// method to set parameters to the test.
        /// </summary>
        /// <param name="parameters"></param>
        protected virtual void SetParameters(PropertyBag parameters)
        {
            // parse xaml file name
            if(string.IsNullOrEmpty(parameters["Xaml"]) || string.IsNullOrEmpty(parameters["Master"]))
            {
                throw new TestValidationException("Must Specify Xaml and Master for xaml based tests.");
            }
            _xamlFileName = parameters["Xaml"];
            masterImageFile = parameters["Master"];

            if (!string.IsNullOrEmpty(parameters["IsLayeredWindow"]))
            {
                _isLayeredWindow = bool.Parse(parameters["IsLayeredWindow"]);
            }

            _initialRenderingMode = RenderingMode.Default;
            if(!string.IsNullOrEmpty(parameters["RenderingMode"]))
            {
                _initialRenderingMode = (RenderingMode)Enum.Parse(typeof(RenderingMode), parameters["RenderingMode"]);
            }

            //set window size if specified. 
            _windowHeight = defaultWindowHeight;
            if (!string.IsNullOrEmpty(parameters["WindowHeight"]))
            {
                double.TryParse(parameters["WindowHeight"], out _windowHeight);
            }

            _windowWidth = defaultWindowWidth;
            if (!string.IsNullOrEmpty(parameters["WindowWidth"]))
            {
                double.TryParse(parameters["WindowWidth"], out _windowWidth);
            }

            _disableAlternativeRenderingModeTesting = false;
            if (!string.IsNullOrEmpty(parameters["DisableAlternativeRenderingModeTesting"]))
            {
                bool.TryParse(parameters["DisableAlternativeRenderingModeTesting"], out _disableAlternativeRenderingModeTesting);
            }

            _disableSerializationTesting = false;
            if (!string.IsNullOrEmpty(parameters["DisableSerializationTesting"]))
            {
                bool.TryParse(parameters["DisableSerializationTesting"], out _disableSerializationTesting);
            }

            _disableMultiMonitorTesting = false;
            if (!string.IsNullOrEmpty(parameters["DisableMultiMonitorTesting"]))
            {
                bool.TryParse(parameters["DisableMultiMonitorTesting"], out _disableMultiMonitorTesting);
            }

            _disableHitTestingTesting = false;
            if (!string.IsNullOrEmpty(parameters["DisableHitTestingTesting"]))
            {
                bool.TryParse(parameters["DisableHitTestingTesting"], out _disableHitTestingTesting);
            }


            if (!string.IsNullOrEmpty(parameters["ToleranceFilePath"]))
            {
                _toleranceFilePath = parameters["ToleranceFilePath"];
                log.LogStatus(string.Format("Tolerance profile file: {0}.", _toleranceFilePath));
            }
        }

        #endregion

        #region Private Data

        private string _xamlFileName = null;
        protected string masterImageFile = string.Empty;
        private bool _isLayeredWindow = false;
        private RenderingMode _initialRenderingMode = RenderingMode.Default;
        protected RenderingMode currentRenderingMode = RenderingMode.Default;
        private const double defaultWindowWidth = 300;
        private const double defaultWindowHeight = 300;
        private SnapshotHistogramVerifier _verifier = null;
        protected TestLog log = null;
        protected Snapshot masterImageSnapshot;
        private Window _window = null;
        protected bool createMasterImagesOnly = false;
        private const string RenderedPrefix = "Rendered";
        private string _locationOfMasterImages = string.Empty;
        private bool _disableAlternativeRenderingModeTesting = false;
        private double _windowWidth;
        private double _windowHeight;
        private bool _disableMultiMonitorTesting = false;
        private bool _disableHitTestingTesting = false;
        private bool _disableSerializationTesting = false;
        private string _toleranceFilePath = "TestProfile.xml";
        private int _intervalForWindowMoving = 500; //time to wait for window moving to be completed, in ms. 
        #endregion
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test UIElements in FlowDocumentScrollViewer (covers PS Task2 and regression test for Regression_Bug37
//              
//              1.  Render some content that is UIElement rich (BlockUIContainer and InlineUIContainer)
//              2.  Resize the Window several times.
//              3.  Visually verify that elements are arranged correctly.
//				                                
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Interop;

using Microsoft.Test.Layout;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{   
    [Test(3, "Bottomless", "UIElementsInBottomless", Variables = "VscanMasterPath=FeatureTests\\FlowLayout\\Masters\\VSCAN")]
    class UIElementsInBottomless: AvalonTest
    {      
        private Window _testWin;
        private string _content = "UIElementsInBottomless.xaml";
        private int _count;
        private int[] _resizeValue;
        private Bitmap _initialScreen = null;
        private Bitmap _endScreen = null;
      
        public UIElementsInBottomless()
            : base()
        {
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Initialize: setup tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _resizeValue = new int[4];
            _resizeValue[0] = 100;
            _resizeValue[1] = 2000;
            _resizeValue[2] = 1;
            _resizeValue[3] = 600;
            
            _testWin = (Window)TestWin.Launch(typeof(Window), 600, 600, 0, 0, _content, true, "LayoutTestWindow");            
            _testWin.Show();
            
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _testWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: Run tests
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTest()
        {                                    
            //Capture initial screen
            _initialScreen = CaptureScreen();

            while (_count < _resizeValue.Length)
            {                
                ResizeWindow();
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }
           
            return TestResult.Pass;
        }

        private TestResult VerifyTest()
        {
            Status("Verifying layout...");

            _endScreen = CaptureScreen();

            IImageAdapter masterAdapter = new ImageAdapter(_initialScreen);
            IImageAdapter captureAdapter = new ImageAdapter(_endScreen);
            ImageComparator comparator = new ImageComparator();
            comparator.ChannelsInUse = ChannelCompareMode.ARGB;

            if (!comparator.Compare(masterAdapter, captureAdapter, false))
            {
                TestLog.Current.LogEvidence("Comparison was not successful!");
                TestLog.Current.Result = TestResult.Fail;
                
                //Dump captured bitmaps to log
                _initialScreen.Save("FirstCapture.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("FirstCapture.tif");
                _endScreen.Save("SecondCapture.tif", System.Drawing.Imaging.ImageFormat.Tiff);
                GlobalLog.LogFile("SecondCapture.tif");              
            }
            return TestResult.Pass;
        }       
       
        private Bitmap CaptureScreen()
        {
            WaitFor(1000);

            WindowInteropHelper iwh = new WindowInteropHelper(_testWin);
            IntPtr hwnd = iwh.Handle;

            Bitmap screen = null;
            screen = ImageUtility.CaptureScreen(hwnd, true);

            return screen;
        }

        private void ResizeWindow()
        {
            Status("Resize Window");            
            //Resize window to trigger reflowing of Content
            _testWin.Width = _resizeValue[_count];
            _testWin.Height = _resizeValue[_count];
            _count++;
            WaitFor(1500);
        }        
    }
}

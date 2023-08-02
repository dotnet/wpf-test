// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Runtime.InteropServices;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Win32;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;


namespace Avalon.Test.CoreUI.Dwm
{
    /******************************************************************************
    * CLASS:          BlurBehindAvalonClientAreaTest
    ******************************************************************************/
    [Test(1, "Integration.DwmApiIntegration", "BlurBehindAvalonClientAreaTest", Disabled=true)]
    public class BlurBehindAvalonClientAreaTest : DwmApiIntegrationTestBase
    {
        #region Private Data
        private Window _behindWindow;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          BlurBehindAvalonClientAreaTest Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public BlurBehindAvalonClientAreaTest() :base()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            TestContainer = new ExeStubContainerCore();
            Dispatcher.CurrentDispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, new DispatcherOperationCallback(app_Startup), null);
            TestContainer.RequestStartDispatcher();

            //Any test failures will be caught elsewhere.
            return TestResult.Pass;
        }
        #endregion

        #region Private Members
        /******************************************************************************
        * Function:          app_Startup
        ******************************************************************************/
        // <summary>
        // </summary>        
        object app_Startup(object args)
        {
            Panel panel = GetAvalonTree();
            GlobalLog.LogStatus("Calling TestContainer.DisplayObject");
            panel.Loaded += new RoutedEventHandler(panel_Loaded);
            surface = TestContainer.DisplayObject(panel, 10, 10, 500, 500);

            return null;
        }

        /******************************************************************************
        * Function:          panel_Loaded
        ******************************************************************************/
        private void panel_Loaded(object sender, RoutedEventArgs args)
        {
            GlobalLog.LogStatus("panel_Loaded"); 
            //Workaround for Window tranparent
            DoMinimizeRestore(surface.Handle);
            DispatcherHelper.DoEvents(1000);
            TestBlurBehindAvalonClientArea();
        }

        /******************************************************************************
        * Function:          TestBlurBehindAvalonClientArea
        ******************************************************************************/
        private void TestBlurBehindAvalonClientArea()
        {
            bool testResult = true;

            CaptureUIElement(rectangle, elmImgBeforeGlassEffectApplied);
            DispatcherHelper.DoEvents(1000);

            helper.StartDWMWindowMessageCollect(surface.SurfaceObject);
            dwmAPISuccess = helper.BlurBehindEntireWindow(surface.SurfaceObject);
            
            if (dwmAPISuccess)
            {
                //Workaround for existing 
                DoMinimizeRestore(surface.Handle);
            }
            else
            {
                GlobalLog.LogStatus("BlurBehindWindow call failed");
                RegisterResult(false);
                return;
            }

            DispatcherHelper.DoEvents(1000);

            _behindWindow = CreateNewWindow();

            DispatcherHelper.DoEvents(1000);

            testResult&= VerifyBlurBehindAvalonClient();

            DoResize();

            DispatcherHelper.DoEvents(1000);
            DispatcherHelper.DoEvents(1000);

            testResult&= VerifyBlurBehindAvalonClient();

            MouseHelper.Move(button, MouseLocation.Center);
            MouseHelper.PressButton(MouseButton.Right);
            MouseHelper.ReleaseButton(MouseButton.Right);

            DispatcherHelper.DoEvents(100);

            DispatcherHelper.DoEvents(100);

            if (contextMenuEvent != 1)
            {
                testResult = false;
                GlobalLog.LogStatus("FAIL - ContextMenuOpenedEvent fired : " + contextMenuEvent.ToString() + " times, expected 1");
            }

            if (testResult)
            {
                RegisterResult(true);
            }
            else
            {
                RegisterResult(false);
            }
        }

        /******************************************************************************
        * Function:          VerifyBlurBehindAvalonClient
        ******************************************************************************/
        private bool VerifyBlurBehindAvalonClient()
        {
            GlobalLog.LogStatus("Verify BlurBehindAvalonClient");

            bool result = true;

            CaptureUIElement(rectangle, elmImgAfterGlassEffectApplied);

            result &= VerifyElementsAreSame();

            result &= VerifyGlassEffectAppliedToAvalonClientArea();

            return result;
        }

        /******************************************************************************
        * Function:          CreateNewWindow
        ******************************************************************************/
        private Window CreateNewWindow()
        {
            GlobalLog.LogStatus("CreateNewWindow");

            _behindWindow = new Window();
            NativeStructs.RECT windowRect = NativeStructs.RECT.Empty;
            NativeMethods.GetWindowRect(new HandleRef(null, surface.Handle), ref windowRect);

            _behindWindow.Width = windowRect.Width;
            _behindWindow.Height = windowRect.Height;
            _behindWindow.Left = windowRect.left;
            _behindWindow.Top = windowRect.top;

            _behindWindow.Topmost = false;

            _behindWindow.Show();

            NativeMethods.SetWindowPos(surface.Handle
                , NativeConstants.HWND_TOP, windowRect.left, windowRect.top, windowRect.Width, windowRect.Height, NativeConstants.SWP_SHOWWINDOW);

            return _behindWindow;
        }

        /******************************************************************************
        * Function:          VerifyGlassEffectAppliedToAvalonClientArea
        ******************************************************************************/
        private bool VerifyGlassEffectAppliedToAvalonClientArea()
        {
            GlobalLog.LogStatus("VerifyGlassEffectAppliedToAvalonClientArea");

            bool retVal = true;

            IImageAdapter img1 = VScanHelper.Capture(surface.Handle, true);

            _behindWindow.Background = Brushes.Blue;

            DispatcherHelper.DoEvents(1000);

            IImageAdapter img2 = VScanHelper.Capture(surface.Handle, true);

            bool result = VScanHelper.Compare(img1, img2, VScanProfile.Profile.Poor);

            if (!result)
            {
                GlobalLog.LogStatus("PASS - images are different");
            }
            else
            {
                GlobalLog.LogStatus("FAIL - images are same");
                retVal = false;
            }

            _behindWindow.Background = Brushes.Red;

            DispatcherHelper.DoEvents(1000);

            IImageAdapter img3 = VScanHelper.Capture(surface.Handle, true);

            result = VScanHelper.Compare(img2, img3, VScanProfile.Profile.Poor);

            if (!result)
            {
                GlobalLog.LogStatus("PASS - images are different");
            }
            else
            {
                GlobalLog.LogStatus("FAIL - images are same");
                retVal = false;
            }

            return retVal;
        }

        /******************************************************************************
        * Function:          DoResize
        ******************************************************************************/
        private void DoResize()
        {
            GlobalLog.LogStatus("DoResize");

            NativeStructs.RECT windowRect = NativeStructs.RECT.Empty;
            NativeMethods.GetWindowRect(new HandleRef(null, surface.Handle), ref windowRect);

            NativeMethods.SetWindowPos(surface.Handle
                , NativeConstants.HWND_TOP, windowRect.left, windowRect.top, windowRect.Width + 100, windowRect.Height + 100, NativeConstants.SWP_SHOWWINDOW);
        }


        /******************************************************************************
        * Function:          ResizeVerification
        ******************************************************************************/
        private bool ResizeVerification()
        {
            bool result = true;

            GlobalLog.LogStatus("ResizeVerification");

            elmImgAfterGlassEffectApplied.Clear();

            CaptureUIElement(rectangle, elmImgAfterGlassEffectApplied);

            result &= VerifyElementsAreSame();

            result &= VerifyGlassEffectAppliedToAvalonClientArea();

            return result;
        }
        #endregion
    }
}


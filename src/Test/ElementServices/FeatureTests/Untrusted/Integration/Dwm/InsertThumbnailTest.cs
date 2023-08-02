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
using Microsoft.Test.Threading;
using System.Runtime.InteropServices;
using Microsoft.Test.Win32;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Shapes;
using Microsoft.Win32;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Dwm
{
    /******************************************************************************
    * CLASS:          InsertThumbnailTest
    ******************************************************************************/
    [Test(1, "Integration.DwmApiIntegration", "InsertThumbnailTest", Disabled=true)]
    public class InsertThumbnailTest : DwmApiIntegrationTestBase
    {
        #region Private Data
        private IntPtr _destinationWindowHandle;
        private Panel _topElement;
        private System.Drawing.Rectangle _boundingBoxForColorInSourceImage;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          InsertThumbnailTest Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public InsertThumbnailTest() :base()
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
        * Function:          StartTest
        ******************************************************************************/
        // <summary>
        // </summary>        
        object app_Startup(object args)
        {
            Panel panel = GetAvalonTree();
            CoreLogger.LogStatus("Calling TestContainer.DisplayObject");
            panel.Loaded += new RoutedEventHandler(panel_Loaded);
            surface = TestContainer.DisplayObject(panel, 0, 0, 400, 400);
            return null;
        }

        /******************************************************************************
        * Function:          panel_Loaded
        ******************************************************************************/
        private void panel_Loaded(object sender, RoutedEventArgs args)
        {
            CoreLogger.LogStatus("panel_Loaded");
            //Workaround for Window tranparent
            DoMinimizeRestore(surface.Handle);
            DispatcherHelper.DoEvents(1000);

            bool testResult = TestInsertThumbnail();

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
        * Function:          TestInsertThumbnail
        ******************************************************************************/
        private bool TestInsertThumbnail()
        {
            CoreLogger.LogStatus("TestInsertThumbnail");

            System.Drawing.Bitmap sourceImage = ImageUtility.CaptureElement(_topElement);
            _boundingBoxForColorInSourceImage = ImageUtility.GetBoundingBoxForColor(sourceImage, System.Drawing.Color.Blue);

            Window destinationWindow = new Window();
            destinationWindow.Show();
            _destinationWindowHandle = helper.GetHwnd(destinationWindow);
            DispatcherHelper.DoEvents(1000);

            Win32.RECT destinationRectangle = new Win32.RECT();
            destinationRectangle.top = 0;
            destinationRectangle.left = 0;
            destinationRectangle.right = 400;
            destinationRectangle.bottom = 400;

            helper.InsertAvalonWindowsAsThumbNailToAvalonWindow(_destinationWindowHandle, surface.Handle, destinationRectangle, true);

            DispatcherHelper.DoEvents(1000);

            NativeMethods.SetForegroundWindow(new HandleRef(null, _destinationWindowHandle));

            DispatcherHelper.DoEvents(1000);

            return Verify();
        }

        /******************************************************************************
        * Function:          GetAvalonTree
        ******************************************************************************/
        new private Panel GetAvalonTree()
        {
            CoreLogger.LogStatus("GetAvalonTree"); 

            _topElement = new StackPanel();
            _topElement.Height = 400.0;
            _topElement.Width = 400.0;
            _topElement.Background = Brushes.Blue;

            return _topElement;
        }

        /******************************************************************************
        * Function:          Verify
        ******************************************************************************/
        private bool Verify()
        {
            CoreLogger.LogStatus("Verify");

            IImageAdapter destinationImage = VScanHelper.Capture(_destinationWindowHandle, true);
            System.Drawing.Rectangle boundingBoxForColorInDestinationImage = ImageUtility.GetBoundingBoxForColor(ImageUtility.ToBitmap(destinationImage), System.Drawing.Color.Blue);

            if (_boundingBoxForColorInSourceImage.Equals(boundingBoxForColorInDestinationImage))
            {
                CoreLogger.LogStatus("PASS: Inserting thumb nail to destination window");
                return true;
            }
            else
            {
                CoreLogger.LogStatus("FAIL: Inserting thumb nail to destination window");
                return false;
            }

        }
        #endregion
    }
}


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

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Dwm
{
    /******************************************************************************
    * CLASS:          EnableDisableDWMTest
    ******************************************************************************/
    [Test(1, "Integration.DwmApiIntegration", "EnableDisableDWMTest", Disabled=true)]
    public class EnableDisableDWMTest : DwmApiIntegrationTestBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          EnableDisableDWMTest Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public EnableDisableDWMTest() :base()
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
        private object app_Startup(object args)
        {

            Panel panel = GetAvalonTree();

            CoreLogger.LogStatus("Calling TestContainer.DisplayObject");
            panel.Loaded+=new RoutedEventHandler(panel_Loaded);
            surface = TestContainer.DisplayObject(panel, 10, 10, 500, 500);

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
            TestEnableDisableDwm();
        }

        /******************************************************************************
        * Function:          TestEnableDisableDwm
        ******************************************************************************/
        private void TestEnableDisableDwm()
        {
            CoreLogger.LogStatus("EnableDisableDwmTest");

            bool testResult = true;

            IImageAdapter img1 = VScanHelper.Capture(surface.Handle, true);
            DispatcherHelper.DoEvents(3000);
            dwmAPISuccess = helper.DisableDWM();
            DispatcherHelper.DoEvents(2000);

            if (!dwmAPISuccess)
            {
                CoreLogger.LogStatus("DisableDWM call failed");
                RegisterResult(false);
                return;
            }

            DispatcherHelper.DoEvents(1000);

            IImageAdapter img2 = VScanHelper.Capture(surface.Handle, true);

            testResult &= VScanHelper.Compare(img1, img2, VScanProfile.Profile.QualityVisualTransform);

            dwmAPISuccess = helper.EnableDWM();

            DispatcherHelper.DoEvents(1000);

            if (!dwmAPISuccess)
            {
                CoreLogger.LogStatus("Enable DWM call failed");
                RegisterResult(false);
                return;
            }

            IImageAdapter img3 = VScanHelper.Capture(surface.Handle, true);

            testResult &= VScanHelper.Compare(img1, img3, VScanProfile.Profile.QualityVisualTransform);

            if (testResult)
            {
                RegisterResult(true);
            }
            else
            {
                RegisterResult(false);
            }
        }
        #endregion
    }
}


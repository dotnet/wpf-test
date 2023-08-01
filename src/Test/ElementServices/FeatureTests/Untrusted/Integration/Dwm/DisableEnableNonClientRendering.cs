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

using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;

using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;

using Microsoft.Test.Logging;
using Microsoft.Test;
using Microsoft.Test.Discovery;

namespace Avalon.Test.CoreUI.Dwm
{
    /******************************************************************************
    * CLASS:          DisableEnableNonClientRendering
    ******************************************************************************/
    [Test(1, "Integration.DwmApiIntegration", "DisableEnableNonClientRendering", Disabled=true)]
    public class DisableEnableNonClientRendering : DwmApiIntegrationTestBase
    {
        #region Constructor
        /******************************************************************************
        * Function:          DisableEnableNonClientRendering Constructor
        ******************************************************************************/
        /// <summary>
        /// Constructor
        /// </summary>
        public DisableEnableNonClientRendering() :base()
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
            TestDisableEnableNonClientRendering();
        }

        /******************************************************************************
        * Function:          TestDisableEnableNonClientRendering
        ******************************************************************************/
        private void TestDisableEnableNonClientRendering()
        {
            CoreLogger.LogStatus("DisableEnableNonClientRendering");

            bool testResult = DisableNonClientRenderingTest();
            testResult &= EnableNonClientRenderingTest();

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
        * Function:          DisableNonClientRenderingTest
        ******************************************************************************/
        private bool DisableNonClientRenderingTest()
        {
            bool testResult = true;

            CoreLogger.LogStatus("DisableNonClientRenderingTest");

            IImageAdapter img1 = VScanHelper.Capture(surface.Handle, true);

            bool dwmAPISuccess = helper.DisableNonClientRendering(surface.SurfaceObject);

            if (dwmAPISuccess)
            {
                DoMinimizeRestore(surface.Handle);
            }
            else
            {
                CoreLogger.LogStatus("DisableNonClientRendering call failed");
                return false;
            }

            DispatcherHelper.DoEvents(2000);

            IImageAdapter img2 = VScanHelper.Capture(surface.Handle, true);

            testResult &= VScanHelper.Compare(img1, img2, VScanProfile.Profile.QualityVisualTransform);

            return testResult;
        }

        /******************************************************************************
        * Function:          EnableNonClientRenderingTest
        ******************************************************************************/
        private bool EnableNonClientRenderingTest()
        {
            bool testResult = true;

            CoreLogger.LogStatus("EnableNonClientRenderingTest");

            IImageAdapter img1 = VScanHelper.Capture(surface.Handle, true);

            bool dwmAPISuccess = helper.EnableNonClientRendering(surface.SurfaceObject);

            if (dwmAPISuccess)
            {
                DoMinimizeRestore(surface.Handle);
            }
            else
            {
                CoreLogger.LogStatus("EnableNonClientRendering call failed");
                return false;
            }

            DispatcherHelper.DoEvents(2000);

            IImageAdapter img2 = VScanHelper.Capture(surface.Handle, true);

            testResult &= VScanHelper.Compare(img1, img2, VScanProfile.Profile.QualityVisualTransform);

            return testResult;
        }
        #endregion
    }
}


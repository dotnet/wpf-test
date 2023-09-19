// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Win32;
using System.Xml;
using System.Configuration;
using System.Reflection;
using System.Security;
using System.Security.Permissions;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using System.Windows.Interop;
using System.Windows.Controls;
using Microsoft.Test.Configuration;
using Microsoft.Test.Logging;
using Microsoft.Test.Graphics;
using Microsoft.Test.RenderingVerification;
using System.Threading;
using Microsoft.Test.Threading;


namespace Microsoft.Test.Graphics.Regression
{
    /// <summary>
    /// Regression Test for Regression_Bug241 - DCR
    /// (Expose a APIs which let developers choose hardware or software rendering modes.)
    /// This test contains multiple variations.
    /// </summary>
    public partial class Regression_Bug23: Application
    {
        /// <summary>
        /// Application.OnStartup.
        ///     - Start test based on variation name.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnStartup(StartupEventArgs e)
        {
            if (IsHWAccelerationDisabled())
            {
                CommonLib.Log.Result = TestResult.Ignore;
                CommonLib.Log.LogEvidence("WPF HW Acceleration is turned off in registry. Please enable HW acceleration at " + _disableHWAccKeyName + "\\" + _disableHWAccKeyValueName + " and try rerun this test");
                Shutdown();
                return;
            }

            if (string.IsNullOrEmpty(DriverState.DriverParameters["XamlTestVar"]))
            {
                CommonLib.Log.Result = TestResult.Fail;
                CommonLib.LogFail("Variation name not specified. Test Exists.");
                this.Shutdown();
                return;
            }
            string varName = DriverState.DriverParameters["XamlTestVar"] as string;
            CommonLib.LogStatus("Variation Name: " + varName);

            // Check to make sure DebugControlKey for WPF is set.
            // DebugControlKey controls the memory section created by WPF
            // for debugging purpose. In this test, it is being used as a
            // way to validate number of hardware and software rendering targets
            CheckAndEnableDebugControlKey();

            //Run the variation synch
            TestVariationCallback callback = (TestVariationCallback)Delegate.CreateDelegate(typeof(TestVariationCallback), this, varName + "Test", true, true);
            callback();

            //flush the Dispatcher queue before shutting down
            DispatcherHelper.DoEvents();
            CommonLib.Log.LogStatus("Shutting down app");
            Shutdown();
        }

        #region Variation Methods
        /// <summary>
        /// Variation:  SetRenderModeToSoftwareBeforeShowTest
        /// Test Logic: Creates a HwndSource with a border as its root visual.
        ///             Sets RenderMode on HwndTarget to be SoftwareOnly
        ///             Validate Number of Software Render Target is 1
        ///             Validate Number of Hardware REnder Target is 0
        /// </summary>
        private void SetRenderModeToSoftwareBeforeShowTest()
        {
            HwndSourceParameters param = new HwndSourceParameters("SetRenderModeToSoftwareBeforeShow", 200, 200);
            HwndSource hwndSource = new HwndSource(param);
            Border border = new Border();
            border.Background = Brushes.Blue;
            hwndSource.RootVisual = border;
            hwndSource.CompositionTarget.RenderMode = RenderMode.SoftwareOnly;
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
            ValidateRenderTargetCount(RenderMode.Default, 0);
        }

        /// <summary>
        /// Variation:  VerifyDefaultRenderTargetTest
        /// Test Logic: Creates and Shows a default WPF Window (Hardware Rendering)
        ///             Validate to ensure Hardware Render Target is 1
        ///             Validate to ensure Software REnder Target is 0
        /// </summary>
        private void VerifyDefaultRenderTargetTest()
        {
            Window win = CreateTestWindow(false);
            ValidateRenderTargetCount(RenderMode.Default, 1);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 0);
        }

        /// <summary>
        /// Variation:  ChangeRenderModeAfterShowTest
        /// Test Logic: Creates and Shows a default WPF window
        ///             Changes RenderMode to SoftwareOnly
        ///             Validate HardwareRenderTargets=0 and SoftwareRenderTargets=1
        /// </summary>
        private void ChangeRenderModeAfterShowTest()
        {
            Window win = CreateTestWindow(false);
            CommonLib.LogStatus("Change to software rendering");
            ChangeRenderMode(win, RenderMode.SoftwareOnly);
            ValidateRenderTargetCount(RenderMode.Default, 0);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
        }

        /// <summary>
        /// Variation:  ChangeRenderModeInLayeredWindowTest
        /// Test Logic: Creats and Shows a transparent WPF window
        ///             Changes RenderMode to SoftwareOnly
        ///             Validate HardwareRenderTargets=0 and SoftwareRenderTargets=1
        /// </summary>
        private void ChangeRenderModeInLayeredWindowTest()
        {
            Window win = CreateTestWindow(true);
            ChangeRenderMode(win, RenderMode.SoftwareOnly);
            ValidateRenderTargetCount(RenderMode.Default, 0);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
        }

        /// <summary>
        /// Variation:  DefaultRenderModeInChildWindowTest
        /// Test Logic: Creates and shows a default, transparent WPF window called parentWindow
        ///             Creates and shows a default, transparent WPF window called childWindow
        ///             Sets childWindow's owner property to parentWindow object
        ///             Validate hardware render targets to be 2 and software as 0
        /// 
        /// </summary>
        private void DefaultRenderModeInChildWindowTest()
        {
            Window parentWindow = CreateTestWindow(true);
            Window childWindow = CreateTestWindow(true);
            childWindow.Owner = parentWindow;

            ValidateRenderTargetCount(RenderMode.Default, 2);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 0);
        }

        /// <summary>
        /// Variation:  SoftwareRenderModeInParentWindowTest
        /// Test Logic: Creates and shows a default, transparent WPF window called parentWindow
        ///             Creates and shows a default, transparent WPF window called childWindow
        ///             Establish child-parent relationship
        ///             Change ParentWindow's rendermode to SoftwareOnly
        ///             Validate there is 1 Software and 1 Hardware render target
        /// </summary>
        private void SoftwareRenderModeInParentWindowTest()
        {
            Window parentWindow = CreateTestWindow(true);
            Window childWindow = CreateTestWindow(true);
            childWindow.Owner = parentWindow;
            ChangeRenderMode(parentWindow, RenderMode.SoftwareOnly);

            ValidateRenderTargetCount(RenderMode.Default, 1);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
        }

        /// <summary>
        /// Variation:  SoftwareRenderModeInChildWindowTest
        /// Test Logic: Creates and shows a default, transparent WPF window called parentWindow
        ///             Creates and shows a default, transparent WPF window called childWindow
        ///             Establishes child and parent relationship
        ///             Change ChildWindow's rendermode to SoftwareOnly
        ///             Validate there is 1 Software and 1 Hardware render target
        /// </summary>
        private void SoftwareRenderModeInChildWindowTest()
        {
            Window parentWindow = CreateTestWindow(true);
            Window childWindow = CreateTestWindow(true);
            childWindow.Owner = parentWindow;
            ChangeRenderMode(childWindow, RenderMode.SoftwareOnly);

            ValidateRenderTargetCount(RenderMode.Default, 1);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
        }

        /// <summary>
        /// Variation:  InvalidRenderModeEnumTest
        /// Test Logic: Create a default WPF window
        ///             Sets RenderMode to value 2
        ///             Expects InvalidEnumArgumentException to be caught
        ///             Validates RenderMode property did not get changed. (Default)
        /// </summary>
        private void InvalidRenderModeEnumTest()
        {
            Window win = CreateTestWindow(false);
            WindowInteropHelper winHelper = new WindowInteropHelper(win);
            HwndSource source = HwndSource.FromHwnd(winHelper.Handle);
            try
            {
                source.CompositionTarget.RenderMode = (RenderMode)(2);
                CommonLib.Log.LogEvidence("InvalidEnumArgumentException not caught. Test Failed");
                CommonLib.Log.Result = TestResult.Fail;
            }
            catch (System.ComponentModel.InvalidEnumArgumentException)
            {
                CommonLib.Log.LogEvidence("Test Passed. InvalidEnumArgumentException caught as expected.");
            }

            if (source.CompositionTarget.RenderMode != RenderMode.Default)
            {
                CommonLib.Log.Result = TestResult.Fail;
                CommonLib.Log.LogEvidence("RenderMode should be Default, but it is " + source.CompositionTarget.RenderMode.ToString());
            }
            else
            {
                CommonLib.Log.LogEvidence("Test Passed. RenderMode appers as expected");
            }
        }

        /// <summary>
        /// Variation:  ChangeWindowContentAndSizeAfterChangeRenderModeTest
        /// Test Logic: Creats and shows a Window called MasterWindow
        ///             Create and shows a Window called CompareWindow
        ///             Sets MasterWIndow background color to be green
        ///             Sets CompareWindow's content to be a Page contains
        ///                 a Video on 3D surface. The video will change color
        ///                 every 5 seconds. (6-10 seconds shows green color)
        ///             Changes CompareWindow RenderMode to SoftwareOnly.
        ///             Starts a 8 second timer.
        ///             When timer ticks. Changes CompareWindow's WindowState property
        ///                 so window size changes
        ///             Visually compare MasterWindow and CompareWindow.
        /// </summary>
        private void ChangeWindowContentAndSizeAfterChangeRenderModeTest()
        {

            Regression_Bug23_Content content = new Regression_Bug23_Content();
            content.InitializeComponent();

            _masterWindow = CreateTestWindow(false);
            _masterWindow.Content = null;
            _masterWindow.Background = new SolidColorBrush(Color.FromRgb(61, 170, 66)); // Video frame color between 6-10th seconds

            _compareWindow = CreateTestWindow(false);
            ChangeRenderMode(_compareWindow, RenderMode.SoftwareOnly);
            _compareWindow.Content = content;
            _compareWindow.Left = _masterWindow.Left + _masterWindow.ActualWidth;

            ValidateRenderTargetCount(RenderMode.Default, 1);
            ValidateRenderTargetCount(RenderMode.SoftwareOnly, 1);
            
            _compareWindow.WindowState = WindowState.Maximized;
            _compareWindow.WindowState = WindowState.Normal;

            //Wait for application idle
            DispatcherHelper.DoEvents(5000);

            //System.Threading.Thread.Sleep(1000);
            VisualCompare();
            CommonLib.Log.LogStatus("Shutting down app");
        }

        /// <summary>
        /// Variation:  ChangeRenderModeInPartialTrustTest
        /// Test Logic: Reflect into RenderMode property on HwndTarget
        ///             Verify it contains UIPermissionAttribute and it is AllWindows
        /// </summary>
        private void ChangeRenderModeInPartialTrustTest()
        {
            Type t = typeof(HwndTarget);
            MethodInfo mi = t.GetProperty("RenderMode").GetAccessors(true)[1] as MethodInfo;
            object[] attributes = mi.GetCustomAttributes(false);
            foreach (object att in attributes)
            {
                if (att is UIPermissionAttribute && ((UIPermissionAttribute)att).Window == UIPermissionWindow.AllWindows)
                {
                    CommonLib.Log.LogEvidence("Test Passed. AllWindow Security attribute found.");
                    return;
                }
            }

            CommonLib.Log.Result = TestResult.Fail;
            CommonLib.Log.LogEvidence("Test Failed. AllWindow Security Attribute was not found");
        }
        #endregion

        #region Helper and Validation Methods
        /// <summary>
        /// Creates and Shows a default WPF Window
        /// </summary>
        /// <param name="AllowsTransparency">Whether to create a transprent window</param>
        /// <returns>window created</returns>
        private Window CreateTestWindow(bool allowsTransparency)
        {
            CommonLib.LogStatus("Creating Test Window");
            Window win = new Window();
            win.Width = 200;
            win.Height = 200;
            win.Left = 0;
            win.Top= 0;
            win.Topmost = true;
            win.WindowStyle = WindowStyle.None;
            win.ResizeMode = ResizeMode.NoResize;
            win.Background = Brushes.Blue;
            win.AllowsTransparency = allowsTransparency;
            win.Content = new Button();
            win.Show();
            return win;
        }

        /// <summary>
        /// Changes RenderMode to a window
        /// </summary>
        /// <param name="window">window object to change render mode</param>
        /// <param name="renderMode">RenderMode to be changed to</param>
        private void ChangeRenderMode(Window window, RenderMode renderMode)
        {
            WindowInteropHelper winHelper = new WindowInteropHelper(window);
            HwndSource source = HwndSource.FromHwnd(winHelper.Handle);
            source.CompositionTarget.RenderMode = renderMode;
        }

        /// <summary>
        /// Validates number of hardware or software render targets in this app instance.
        /// </summary>
        /// <param name="renderMode">the rendering mode to be validated against</param>
        /// <param name="ExpectedCount">expected number of render target</param>
        private void ValidateRenderTargetCount(RenderMode renderMode, uint expectedCount)
        {
            //Wait for applicaiton Idle
            DispatcherHelper.DoEvents();

            System.Threading.Thread.Sleep(1000);
            int ActualCount = -1;
            string ActualRenderMode = "Hardware";
            // RenderCapability.Tier==0 --> No graphics hardware acceleration available for the application on the device.
            if (renderMode == RenderMode.Default && RenderCapability.Tier > 0)
            {
                ActualCount = RenderModeValidationHelper.GetRenderTargetNumber(RenderMode.Default);
            }
            else
            {
                ActualCount = RenderModeValidationHelper.GetRenderTargetNumber(RenderMode.SoftwareOnly);
                ActualRenderMode = "Software";
            }

            CommonLib.Log.LogEvidence(ActualRenderMode + " Render Targets: Expected=" + expectedCount.ToString() + " Actual=" + ActualCount.ToString());
            if (expectedCount == ActualCount)
            {
                CommonLib.Log.LogEvidence("VALIDATION PASSED: Render Target Count Matched.");
            }
            else
            {
                CommonLib.Log.Result = TestResult.Fail;
                CommonLib.Log.LogEvidence("VALIDATION FAILED!");
            }

        }

        /// <summary>
        /// Checks and Sets DebugControlKey
        /// </summary>
        private void CheckAndEnableDebugControlKey()
        {
            int currentDebugKey = -1;
            try
            {
                currentDebugKey = (int)Registry.GetValue(_debugControlRegKeyName, _debugControlRegKeyValueName, -1);
            }
            catch
            {
                CommonLib.LogStatus("DebugControlKey does not exist or cannot be accessed.");
            }
            finally
            {
                if (currentDebugKey != 1)
                {
                    CommonLib.LogStatus("Enabing DebugControlKey");
                    MachineStateManager.SetRegistryValue(_debugControlRegKeyName, _debugControlRegKeyValueName, 1);
                }
                else
                {
                    CommonLib.LogStatus("DebugControlKey already enabled");
                }
            }
        }

        private bool IsHWAccelerationDisabled()
        {
            object keyValue = Registry.GetValue(_disableHWAccKeyName, _disableHWAccKeyValueName, -1);
            return ((keyValue != null && (int)keyValue == 1) ? true : false);
        }

        /// <summary>
        /// Performs visual validation on MasterWindow and CompareWindow.
        /// </summary>
        private void VisualCompare()
        {
            ImageAdapter CompareImageAdapter = new ImageAdapter(new System.Drawing.Rectangle((int)_compareWindow.Left, (int)_compareWindow.Top, (int)_compareWindow.ActualWidth, (int)_compareWindow.ActualHeight));
            ImageAdapter MasterImageAdapter = new ImageAdapter(new System.Drawing.Rectangle((int)_masterWindow.Left, (int)_masterWindow.Top, (int)_masterWindow.ActualWidth, (int)_masterWindow.ActualHeight));

            ImageComparator comparator = new ImageComparator();
            bool TestPassed = comparator.Compare(CompareImageAdapter, MasterImageAdapter, true);
    
            if (!TestPassed)
            {
                CommonLib.Log.Result = TestResult.Fail;
                CommonLib.Log.LogEvidence("Visual Validation Failed. Saving Failure analysis package");
                Package package = Package.Create(".\\FailurePackage.vscan",
                                                  ImageUtility.ToBitmap(CompareImageAdapter),
                                                  ImageUtility.ToBitmap(MasterImageAdapter),
                                                  comparator.Curve.CurveTolerance.WriteToleranceToNode());
                package.Save();
                CommonLib.Log.LogFile(package.PackageName);
            }
            else
            {
                CommonLib.Log.Result = TestResult.Pass;
                CommonLib.Log.LogEvidence("Test Passed");
            }
        }
        #endregion

        #region Private Members
        private readonly string _debugControlRegKeyName = "HKEY_LOCAL_MACHINE\\SOFTWARE\\Microsoft\\Avalon.Graphics";
        private readonly string _debugControlRegKeyValueName = "EnableDebugControl";
        private readonly string _disableHWAccKeyName = "HKEY_CURRENT_USER\\SOFTWARE\\Microsoft\\Avalon.Graphics";
        private readonly string _disableHWAccKeyValueName = "DisableHWAcceleration";
        private Window _masterWindow,_compareWindow;
        #endregion


        private delegate void TestVariationCallback();


    }
}

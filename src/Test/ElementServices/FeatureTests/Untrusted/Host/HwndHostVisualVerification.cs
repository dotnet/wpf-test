// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Visual verification of HwndHosts
 * 
 * Contributor: Microsoft
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/

using System;
using System.Collections;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Source;
using Avalon.Test.CoreUI.Threading;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Trusted.Controls;
//using Avalon.Test.Framework.Dispatchers;
//using Avalon.Test.Host.Controls;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.Win32;

namespace Avalon.Test.CoreUI.Hosting
{


    ///<summary>
    /// VScan based tests for HnwdHost.
    ///</summary>
    //[CoreTestsLoader(CoreTestsTestType.MethodBase)]
    [TestDefaults]
    public class HwndHostRightToLeftVScan
    {
        /// <summary>
        /// Test RTL property's affect on an HwndHost using VScan.
        /// </summary>
        [Test(0, @"Hosting\VisualVerification\RightToLeft", TestCaseSecurityLevel.FullTrust, "HwndHost RightToLeft vscan test", TestParameters = "MasterImage=HwndHostRightToLeft.png", SupportFiles=@"FeatureTests\ElementServices\HwndHostRightToLeft.png", Area = "AppModel", Disabled = true)]
        public void Run()
        {
            CoreLogger.BeginVariation();
            // Parse params.
            // TestCaseInfo testCaseInfo = TestCaseInfo.GetCurrentInfo();
            string masterImage = DriverState.DriverParameters["MasterImage"];

            using (CoreLogger.AutoStatus("Test has started"))
            {
                // Create simple window with canvas and HwndHost.
                CreateWindow();

                if (VScanHelper.Compare(masterImage, _win))
                {
                    CoreLogger.LogTestResult(true, "Pass");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Fail, images did not match.");
                }
            }
            CoreLogger.EndVariation();
        }

        Window _win = null;
        StackPanel _panel = null;
        Win32ButtonCtrl _host = null;

        /// <summary>
        /// Create a Window with Canvas and HwndHost. Several unsupported properties on the HwndHost
        /// are set including Clip, RenderTransform and Opacity. These properties should not change the
        /// appearance of the HwndHost. 
        /// </summary>
        private void CreateWindow()
        {
            _win = new Window();
            _win.Width = 400;
            _win.Height = 400;

            _panel = new StackPanel();

            _panel.Background = System.Windows.Media.Brushes.Green;
            _panel.Orientation = Orientation.Horizontal;
            _panel.FlowDirection = FlowDirection.RightToLeft;

            // Create hwndHost
            _host = new Win32ButtonCtrl();
            _host.Width = 200;
            _host.Height = 200;
            Canvas.SetTop(_host, 30);
            Canvas.SetLeft(_host, 30);

            //
            // Setting these properties on the HnwdHost should have no effect.
            //

            // Opacity.
            _host.Opacity = 0.1;

            // Clip geometry, clip to ellipse centered in HwndHost.
            EllipseGeometry myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new System.Windows.Point(100, 100);
            myEllipseGeometry.RadiusX = 25;
            myEllipseGeometry.RadiusY = 50;
            _host.Clip = myEllipseGeometry;

            // RenderTransform, rotate about HwndHost center.
            _host.RenderTransform = new RotateTransform(0, 100, 100);

            _panel.Children.Add(_host);
            _win.Content = _panel;

            _win.Show();

            Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
        }
    }

    ///<summary>
    /// VScan based tests for HnwdHost.
    ///</summary>
    [CoreTestsLoader(CoreTestsTestType.MethodBase)]
    public class HwndHostUnsupportedPropertiesVScan
    {
        /// <summary>
        /// Test setting unsupported properties on an HwndHost and verify that its visual appearance doesn't change
        /// using VScan. The properties include Clip, RenderTransform, and Opacity.
        /// </summary>
        [TestAttribute(0, @"Hosting\VisualVerification\UnsupportedProperties", TestCaseSecurityLevel.FullTrust, "HwndHost unsupported properties vscan test", TestParameters = "MasterImage=HwndHostUnsupportedProperties.png", Area = "AppModel")]
        public void Run()
        {
            CoreLogger.BeginVariation();
            // Parse params.
            // TestCaseInfo testCaseInfo = TestCaseInfo.GetCurrentInfo();
            string masterImage = DriverState.DriverParameters["MasterImage"];

            using (CoreLogger.AutoStatus("Test has started"))
            {
                // Create simple window with canvas and HwndHost.
                CreateWindow();

                if (VScanHelper.Compare(masterImage, _win))
                {
                    CoreLogger.LogTestResult(true, "Pass");
                }
                else
                {
                    CoreLogger.LogTestResult(false, "Fail, images did not match.");
                }

            }
            CoreLogger.EndVariation();
        }

        Window _win = null;
        Canvas _canvas = null;
        Win32ButtonCtrl _host = null;
        
        /// <summary>
        /// Create a Window with Canvas and HwndHost. Several unsupported properties on the HwndHost
        /// are set including Clip, RenderTransform and Opacity. These properties should not change the
        /// appearance of the HwndHost. 
        /// </summary>
        private void CreateWindow()
        {
            _win = new Window();
            _win.Width = 400;
            _win.Height = 400;

            _canvas = new Canvas();
            
            // Create hwndHost
            _host = new Win32ButtonCtrl();
            _host.Width = 200;
            _host.Height = 200;
            Canvas.SetTop(_host, 30);
            Canvas.SetLeft(_host, 30);

            //
            // Setting these properties on the HnwdHost should have no effect.
            //
            
            // Opacity.
            _host.Opacity = 0.1;

            // Clip geometry, clip to ellipse centered in HwndHost.
            EllipseGeometry myEllipseGeometry = new EllipseGeometry();
            myEllipseGeometry.Center = new System.Windows.Point(100, 100);
            myEllipseGeometry.RadiusX = 25;
            myEllipseGeometry.RadiusY = 50;
            _host.Clip = myEllipseGeometry;

            // RenderTransform, rotate about HwndHost center.
            _host.RenderTransform = new RotateTransform(0, 100,100);

            _canvas.Children.Add(_host);
            _win.Content = _canvas;

            _win.Show();

            Microsoft.Test.Threading.DispatcherHelper.DoEvents(2000);
        }   
    }
 }











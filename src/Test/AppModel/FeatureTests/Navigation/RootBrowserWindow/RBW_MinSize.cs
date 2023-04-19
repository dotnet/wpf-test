// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Net;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Loaders;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // MinSize
    public partial class NavigationTests : Application
    {
        void RBWWindowMinSize_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWMinSize");
        }

        void RBWWindowMinSize_LoadCompleted(object sender, NavigationEventArgs e)
        {
            // RBW's allowable minimum dimensions (min size = 200x200)
            double ExpectedMinWidth = 200;
            double ExpectedMinHeight = 200;

            // IE's allowable minimum width (smallest allowable IE window dimensions = 250x150)
            if (ApplicationDeploymentHelper.GetIEVersion() > 6)
                ExpectedMinWidth = 250;

            _rbwTest.SetupTest();

            _rbwTest.Output("Expected Dimension: " + ExpectedMinWidth + "x" + ExpectedMinHeight);
            if (_rbwTest.NavWin.ActualWidth != ExpectedMinWidth || _rbwTest.NavWin.ActualHeight != ExpectedMinHeight)
                _rbwTest.Fail("Actual Dimension: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            else
                _rbwTest.Output("[VALIDATION PASSED] Actual Dimension: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);

            _rbwTest.Output("[SET] Window.W/H = 0");
            _rbwTest.NavWin.Width = 0;
            _rbwTest.NavWin.Height = 0;
            if (_rbwTest.NavWin.ActualWidth != ExpectedMinWidth || _rbwTest.NavWin.ActualHeight != ExpectedMinHeight)
                _rbwTest.Fail("Actual Dimension: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            else
                _rbwTest.Output("[VALIDATION PASSED] Actual Dimension: " + _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);

            /***** BVT BLOCKER:  ******
             * rbwTest.Output("[SET] Page.W/H = 0");
             * this.Width = 0;
             * this.Height = 0;
             * 
             * rbwTest.Output("[SET] Window.SizeToContent = WidthAndHeight");
             * rbwTest.NavWin.SizeToContent = SizeToContent.WidthAndHeight;
             * 
             * if (rbwTest.NavWin.ActualWidth != ExpectedMinWidth || rbwTest.NavWin.ActualHeight != ExpectedMinHeight)
             *     rbwTest.Fail("Actual Dimension: " + rbwTest.NavWin.ActualWidth + "x" + rbwTest.NavWin.ActualHeight);
             * else
             *     rbwTest.Output("[VALIDATION PASSED] Actual Dimension: " + rbwTest.NavWin.ActualWidth + "x" + rbwTest.NavWin.ActualHeight);
             *************/

            _rbwTest.Pass("MinSize correctly clipped");
        }
    }
}

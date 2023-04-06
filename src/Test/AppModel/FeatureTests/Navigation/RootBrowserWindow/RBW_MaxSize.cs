// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;
using System.Windows.Controls;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // MaxSize
    public partial class NavigationTests : Application
    {
        double _rbwMaxWidth = 800,_rbwMaxHeight = 600;

        void RBWWindowMaxSize_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWMaxSize");
        }

        void RBWWindowMaxSize_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            // Setting MaxWidth = Desktop.Res + 8 there are total 4 pixels rendered outside the screen
            // to mimic full screen
            // On Vista, there are 8 pixels rendered outside the screen on each edge, so add 16.
            int offset = Const.IsVistaOrNewer ? 16 : 8;
            _rbwMaxWidth = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width + offset;
            _rbwMaxHeight = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height + offset;
            _rbwTest.Output("Max Resolution: " + _rbwMaxWidth + "x" + _rbwMaxHeight);

            RBWWindowMaxSize_Validate();

            _rbwTest.Output("[SET] Page.Width/Height = MaxWidth/Height + 5");
            Page currRBWPage = LogicalTreeHelper.FindLogicalNode(_rbwTest.NavWin.Content as DependencyObject, "rbwTestPage") as Page;
            currRBWPage.Width = _rbwMaxWidth + 5;
            currRBWPage.Height = _rbwMaxHeight + 5;

            RBWWindowMaxSize_Validate();

            _rbwTest.Output("[SET] Window.Width/Height = MaxWidth/Height + 5");
            _rbwTest.NavWin.Width = _rbwMaxWidth + 5;
            _rbwTest.NavWin.Height = _rbwMaxHeight + 5;

            RBWWindowMaxSize_Validate();

            _rbwTest.Pass("MaxSize correctly clipped");
        }

        void RBWWindowMaxSize_Validate()
        {
            if (_rbwTest.NavWin.ActualWidth > _rbwMaxWidth || _rbwTest.NavWin.ActualHeight > _rbwMaxHeight)
            {
                _rbwTest.Fail("Actual Dimension: " + 
                    _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            }
            else
            {
                _rbwTest.Output("[VALIDATION PASSED] Actual Dimension: " +
                    _rbwTest.NavWin.ActualWidth + "x" + _rbwTest.NavWin.ActualHeight);
            }

        }
    }
}

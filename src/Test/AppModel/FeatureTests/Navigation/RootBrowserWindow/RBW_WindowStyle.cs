// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // WindowStyle
    public partial class NavigationTests : Application
    {
        void RBWWindowStyle_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWWindowStyle");
        }

        void RBWWindowStyle_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [GET] Default RBW.WindowStyle");
            try
            {
                WindowStyle ws = _rbwTest.NavWin.WindowStyle;
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] RBW.WindowStyle = None");
            try
            {
                _rbwTest.NavWin.WindowStyle = WindowStyle.None;
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] RBW.WindowStyle = ThreeDBorderWindow");
            try
            {
                _rbwTest.NavWin.WindowStyle = WindowStyle.ThreeDBorderWindow;
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }
            catch (System.Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] RBW.WindowStyle = ToolWindow");
            try
            {
                _rbwTest.NavWin.WindowStyle = WindowStyle.ToolWindow;
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Pass("InvalidOperationException caught as expected");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }
        }
    }
}

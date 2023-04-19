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
    // Topmost
    public partial class NavigationTests : Application
    {
        void RBWWindowTopmost_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWTopmost");
        }

        void RBWWindowTopmost_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [GET] Default RBW.Topmost");
            try
            {
                bool test = _rbwTest.NavWin.Topmost;
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }

            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should not have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] RBW.Topmost = false");
            try
            {
                _rbwTest.NavWin.Topmost = false;
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

            _rbwTest.Output("  [SET] RBW.Topmost = true");
            try
            {
                _rbwTest.NavWin.Topmost = true;
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Pass("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

        }
    }
}

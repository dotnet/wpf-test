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
    // Owner
    public partial class NavigationTests : Application
    {
        void RBWWindowOwner_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWOwner");
        }

        void RBWWindowOwner_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [GET] Default RBW.Owner");
            try
            {
                Window win = _rbwTest.NavWin.Owner;
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

            _rbwTest.Output("  [SET] RBW.Owner = null");
            try
            {
                _rbwTest.NavWin.Owner = null;
                _rbwTest.Fail("InvalidOperationException was not thrown");
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

            _rbwTest.Output("  [SET] RBW.Owner = Application.Current.MainWindow");
            try
            {
                _rbwTest.NavWin.Owner = Application.Current.MainWindow;
                _rbwTest.Fail("InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Pass("  [VALIDATION PASSED] InvalidOperationException caught as expected");
            }
            catch (System.Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown. Actual exception: "+ ex.ToString());
            }
        }
    }
}

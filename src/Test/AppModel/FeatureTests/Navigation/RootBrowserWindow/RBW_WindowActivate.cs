// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Navigation;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // WindowActivate
    public partial class NavigationTests : Application
    {
        void RBWWindowActivate_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("WindowActivate");
        }

        void RBWWindowActivate_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            // try the MainWindow cast to a NavigationWindow
            try
            {
                _rbwTest.NavWin.Activate();
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("InvalidOperationException successfully caught");
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown. Actual exception: " + exp.ToString());
            }

            // try the MainWindow itself
            try
            {
                Application.Current.MainWindow.Activate();
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Pass("InvalidOperationException successfully caught");
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown. Actual exception: " + exp.ToString());
            }
        }
    }
}

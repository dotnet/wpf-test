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
    // Location
    public partial class NavigationTests : Application
    {
        void RBWWindowLocation_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWLocation");
        }

        void RBWWindowLocation_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [SET] Window.Top = 100");
            try
            {
                _rbwTest.NavWin.Top = 100;
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Output("InvalidOperationException successfully caught");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown", 
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] Window.Left = 100");
            try
            {
                _rbwTest.NavWin.Left = 100;
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown");
            }
            catch (InvalidOperationException)
            {
                _rbwTest.Pass("InvalidOperationException successfully caught");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("InvalidOperationException should have been thrown",
                    "InvalidOperationException was not thrown. Actual exception: " + ex.ToString());
            }
        }
    }
}

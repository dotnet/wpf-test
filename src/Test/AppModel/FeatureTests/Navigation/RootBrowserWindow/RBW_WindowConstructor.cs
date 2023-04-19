// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Security;
using System.Windows;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // WindowConstructor
    public partial class NavigationTests : Application
    {
        void RBWWindowConstructor_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("WindowConstructor");
        }

        void RBWWindowConstructor_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            // Window
            try
            {
                new Window();
                _rbwTest.Fail("SecurityException should have been thrown",
                    "SecurityException was not thrown");
            }
            catch (SecurityException)
            {
                _rbwTest.Output("SecurityException successfully caught");
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("SecurityException should have been thrown", 
                    "SecurityException was not thrown. Actual exception: " + exp.ToString());
            }

            // NavigationWindow
            try
            {
                new NavigationWindow();
                _rbwTest.Fail("SecurityException should have been thrown",
                    "SecurityException was not thrown");
            }
            catch (SecurityException)
            {
                _rbwTest.Pass("SecurityException successfully caught");
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("SecurityException should have been thrown",
                    "SecurityException was not thrown. Actual exception: " + exp.ToString());
            }
        }
    }
}

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
    // RestoreBounds
    public partial class NavigationTests : Application
    {
        void RBWWindowRestoreBounds_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWRestoreBounds");
        }

        void RBWWindowRestoreBounds_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [GET] Default RBW.RestoreBounds");
            try
            {
                Rect nwinBounds = _rbwTest.NavWin.RestoreBounds;
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

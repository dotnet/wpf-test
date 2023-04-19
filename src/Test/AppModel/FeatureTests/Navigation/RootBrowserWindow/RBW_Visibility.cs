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
    // Visibility
    public partial class NavigationTests : Application
    {
        void RBWWindowVisibility_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWVisibility");
        }

        void RBWWindowVisibility_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            _rbwTest.Output("  [GET] Default RBW.Visibility");
            try
            {
                RBWWindowVisibility_EnsureVisibility(Visibility.Visible);
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("Unexpected exception caught! Actual exception: " + ex.ToString());
            }

            _rbwTest.Output("  [SET] RBW.Visibility = Visibility.Visible");
            try
            {
                _rbwTest.NavWin.Visibility = Visibility.Visible;
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("Unexpected exception caught! Actual exception: " + ex.ToString());
            }
            finally
            {
                RBWWindowVisibility_EnsureVisibility(Visibility.Visible);
            }

            _rbwTest.Output("  [SET] RBW.Visibility = Visibility.Hidden");
            try
            {
                _rbwTest.NavWin.Visibility = Visibility.Hidden;
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("Unexpected exception caught! Actual exception: " + ex.ToString());
            }
            finally
            {
                RBWWindowVisibility_EnsureVisibility(Visibility.Hidden);
            }

            _rbwTest.Output("  [SET] RBW.Visibility = Visibility.Collapsed");

            try
            {
                _rbwTest.NavWin.Visibility = Visibility.Collapsed;
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("Unexpected exception caught! Actual exception: " + ex.ToString());
            }
            finally
            {
                RBWWindowVisibility_EnsureVisibility(Visibility.Collapsed);
            }

            try
            {
                _rbwTest.NavWin.ShowDialog();
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
            finally
            {
                RBWWindowVisibility_EnsureVisibility(Visibility.Collapsed);
            }
        }

        void RBWWindowVisibility_EnsureVisibility(Visibility vis)
        {
            _rbwTest.Output("  [EXPECTING] RBW.Visibility == " + vis.ToString());
            if (Application.Current.MainWindow.Visibility != vis)
            {
                _rbwTest.Fail("RBW.Visibility == " + Application.Current.MainWindow.Visibility.ToString());
            }
            else
            {
                _rbwTest.Output("  [VALIDATION PASSED] RBW.Visibility == " + vis);
            }

            // TO BE IMPLEMENTED: Visual Validation
        }
    }
}

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
    // WindowHide
    public partial class NavigationTests : Application
    {
        void RBWWindowHide_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("WindowHide");
            //RootBrowserWindowTestPage rbwtp = new RootBrowserWindowTestPage();
            //Activator.CreateInstance(typeof(RootBrowserWindowTestPage));
            //Application.Current.StartupUri = new Uri("RBW_WindowTestClass.xaml", UriKind.RelativeOrAbsolute);

        }

        void RBWWindowHide_LoadCompleted(object sender, NavigationEventArgs e)
        {
            _rbwTest.SetupTest();

            // try MainWindow cast to a NavigationWindow
            try
            {
                _rbwTest.NavWin.Hide();
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("SecurityException was not thrown. Actual exception: " + exp.ToString());
            }
            finally
            {
                RBWWindowHide_EnsureHidden();
            }

            // try MainWindow itself
            try
            {
                Application.Current.MainWindow.Hide();
            }
            catch (Exception exp)
            {
                _rbwTest.Fail("SecurityException was not thrown. Actual exception: " + exp.ToString());
            }
            finally
            {
                RBWWindowHide_EnsureHidden();
            }

            _rbwTest.Pass("RootBrowserWindow Hide tests passed.");
        }

        void RBWWindowHide_EnsureHidden()
        {
            _rbwTest.Output("   [EXPECTED] RBW.Visibility == Hidden");

            if (Application.Current.MainWindow.Visibility != Visibility.Hidden)
            {
                _rbwTest.CacheFail("RBW.Visibility == Hidden", "RBW.Visibility == " + Application.Current.MainWindow.Visibility.ToString());
            }
            else
            {
                _rbwTest.CachePass("   [VALIDATION PASSED] RBW.Visibility == Hidden");
            }
        }

        // TO BE IMPLEMENTED: Visual Validation
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Navigation;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    // IsActive
    public partial class NavigationTests : Application
    {
        void RBWWindowIsActive_Startup(object sender, StartupEventArgs e)
        {
            _rbwTest = new RootBrowserWindowTestClass("RBWIsActive");
            
	    // validate the IsActive Property after ContentRendered event fired instead of GotKeyBoardFocus Event
            // make sure the window Isactive after page loaded.
            
            this.MainWindow.ContentRendered += new EventHandler(RBWWindowIsActive_ContentRendered);
        }

        void RBWWindowIsActive_ContentRendered(object sender, EventArgs e)
        {
            _rbwTest.Output("  [GET] Window.IsActive, after page is loaded");
            try
            {
                if (Application.Current.MainWindow.IsActive == true)
                    _rbwTest.Output("correct!!! IsActive = true, after page is loaded");
                else
                    _rbwTest.Fail("IsActive = true, after page is loaded", "IsActive = false");
            }
            catch (Exception ex)
            {
                _rbwTest.Fail("Unexpected exception: " + ex.ToString());
            }


            _rbwTest.Pass("IsActive == true after page is loaded");
        }

    }
}

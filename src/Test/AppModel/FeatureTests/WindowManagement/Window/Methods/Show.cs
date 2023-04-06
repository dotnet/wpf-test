// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;
using System.Windows.Controls;
using Microsoft.Test.Input;

namespace WindowTest
{

    /// <summary>
    /// 
    /// Test for calling Window.Show() method
    ///
    /// </summary>
    public partial class Show
    {                                                    
        void OnContentRendered(object sender, EventArgs e)
        {
            Visibility expectedVisibility = Visibility.Visible;
            
            Logger.Status("Calling Show to a visible window");
            this.Show();
            ValidateVisibility(this, expectedVisibility);

            Logger.Status("Calling Show() to never been shown Window");
            Window win = new Window();
            win.Title = "Second Window";
            win.Show();
            ValidateVisibility(win, expectedVisibility);

            Logger.Status("Calling Show() to hidden Window");
            win.Hide();
            win.Show();
            ValidateVisibility(win, expectedVisibility);

		    TestHelper.Current.TestCleanup();

        }

        void ValidateVisibility(Window targetWindow, Visibility expectedVisibility)
        {
            Logger.Status("[EXPECTED] Visibility = " + expectedVisibility.ToString());
            if (targetWindow.Visibility != expectedVisibility)
            {
                Logger.LogFail("[ACTUAL] Visibility = " + targetWindow.Visibility.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] Visibility = " + targetWindow.Visibility.ToString());
            }

            if (!WindowValidator.ValidateVisibility(targetWindow.Title, expectedVisibility))
            {
                Logger.LogFail("Win32 Validation Failed");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
            }
            
        }
    }

}

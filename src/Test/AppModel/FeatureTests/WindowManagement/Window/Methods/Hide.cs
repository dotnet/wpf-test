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
    /// Test for calling Window.Hide() method
    ///
    /// </summary>
    public partial class Hide
    {                              
        void OnContentRendered(object sender, EventArgs e)
        {
            Visibility expectedVisibility = Visibility.Hidden;
            
            Logger.Status("Calling Hide() to a visible window");
            this.Hide();
            PropertyValidation(this, expectedVisibility);
            Win32Validation(this.Title, expectedVisibility);

            Logger.Status("Calling Hide() to Hidden Window");
            this.Hide();
            PropertyValidation(this, expectedVisibility);
            Win32Validation(this.Title, expectedVisibility);

            Logger.Status("Calling Hide() on never-been-shown Window should not throw exception");
            Window win = new Window();
            win.Title = "Second Window";
            try
            {
                win.Hide();
                PropertyValidation(win, expectedVisibility);
            }
            catch(System.Exception ex)
            {
                Logger.LogFail("Unexpected Exception caugh!!\nException Text: " + ex.ToString());
            }

		    TestHelper.Current.TestCleanup();

        }

        void PropertyValidation(Window targetWindow, Visibility expectedVisibility)
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
        }

        void Win32Validation(string windowTitle, Visibility expectedVisibility)
        {
            if (!WindowValidator.ValidateVisibility(windowTitle, expectedVisibility))
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

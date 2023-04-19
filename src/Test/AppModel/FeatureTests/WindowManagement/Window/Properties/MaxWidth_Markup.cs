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
    /// Test for setting Window.MaxWidth property in Markup
    ///
    /// </summary>
    public partial class MaxWidth_Markup
    {                                                                                                                                                                                                                 
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedWidth = 300;
            double expectedMaxWidth = 300;
            Validate(expectedWidth, expectedMaxWidth);
            
            // Expected Value
            expectedWidth = 200;
            expectedMaxWidth = 300;
            Logger.Status("[SET] Window.Width = " + expectedWidth.ToString());
            this.Width = expectedWidth;
            Validate(expectedWidth, expectedMaxWidth);

            Logger.Status("[SET] Window.Width = 400");
            this.Width = 400;
            this.MaxWidth = 300;
            expectedWidth = 300;
            Validate(expectedWidth, expectedMaxWidth);

    //                expectedMaxWidth = 500;
    //                Logger.Status("[SET] Window.MaxWidth = " + expectedMaxWidth.ToString());
    //                this.MaxWidth = expectedMaxWidth;
    //                expectedWidth = 400;
    //                Validate(expectedWidth, expectedMaxWidth);

            Logger.Status("[SET] WindowState=Maximized");
            this.WindowState = WindowState.Maximized;
            expectedWidth = expectedMaxWidth;
            Validate(expectedWidth, expectedMaxWidth);

            Logger.Status("[SET] WindowState=Minimized");
            this.WindowState = WindowState.Minimized;
            expectedWidth = 300;
            Validate(expectedWidth, expectedMaxWidth);
            
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            expectedWidth = 300;
            Validate(expectedWidth, expectedMaxWidth);
            

		    TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedWidth, double expectedMaxWidth)
        {
            Logger.Status("[EXPECTED VALUES] Window.MaxWidth=" + expectedMaxWidth.ToString() + " Window.ActualWidth=" + expectedWidth.ToString());
            // Validate Property Value
            if ((this.MaxWidth != expectedMaxWidth) || (this.ActualWidth != expectedWidth))
            {
                Logger.LogFail("Window.MaxWidth=" + this.MaxWidth.ToString() + " Window.ActualWidth=" + this.ActualWidth.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
            
            // Win32 Validation
            // Call Win32 validation only when Window is not minimized. This is because 
            // when Window is minimized, win32 reports 160 for width. Window.ActualWidth
            // property reports the actual restored width, which is better.
            if (this.WindowState != WindowState.Minimized)
            {
                if (WindowValidator.ValidateWidth(this.Title, expectedWidth))
                {
                    Logger.Status("[VALIDATION PASSED] Win32 Validation Passed");
                }
                else
                {
                    Logger.LogFail("Win32 Validation Failed!");
                }
            }

        }
    }

}

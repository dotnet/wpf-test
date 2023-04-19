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
    /// Test for setting Window.MinWidth property in Markup
    ///
    /// </summary>
    public partial class MinWidth_Markup
    {                                                                                                                                                                                                                                                                                                         
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedWidth = 300;
            double expectedMinWidth = 300;
            Validate(expectedWidth, expectedMinWidth);

            expectedMinWidth = 350;
            Logger.Status("[SET] Window.MinWidth = " + expectedMinWidth.ToString());
            this.MinWidth = expectedMinWidth;
            expectedWidth = expectedMinWidth;
            Validate(expectedWidth, expectedMinWidth);

    //                expectedMinWidth = 500;
    //                Logger.Status("[SET] Window.MinWidth = " + expectedMinWidth.ToString());
    //                this.MinWidth = expectedMinWidth;
    //                expectedWidth = 400;
    //                Validate(expectedWidth, expectedMinWidth);

            Logger.Status("[SET] WindowState=Maximized");
            this.WindowState = WindowState.Maximized;
            
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            Validate(expectedWidth, expectedMinWidth);
           

            TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedWidth, double expectedMinWidth)
        {
            Logger.Status("[EXPECTED VALUES] Window.MinWidth=" + expectedMinWidth.ToString() + " Window.ActualWidth=" + expectedWidth.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.MinWidth, expectedMinWidth)
                || !TestUtil.IsEqual(this.ActualWidth, expectedWidth))
            {
                Logger.LogFail("Window.MinWidth=" + this.MinWidth.ToString() + " Window.ActualWidth=" + this.ActualWidth.ToString());
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

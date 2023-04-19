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
    /// Test for setting Window.MaxHeight property in Markup
    ///
    /// </summary>
    public partial class MaxHeight_Markup
    {                                                                                                                                                                         
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedHeight = 300;
            double expectedMaxHeight = 300;
            Validate(expectedHeight, expectedMaxHeight);
            
            // Expected Value
            expectedHeight = 200;
            expectedMaxHeight = 300;
            Logger.Status("[SET] Window.Height = " + expectedHeight.ToString());
            this.Height = expectedHeight;
            Validate(expectedHeight, expectedMaxHeight);

            Logger.Status("[SET] Window.Height = 400");
            this.Height = 400;
            this.MaxHeight = 300;
            expectedHeight = 300;
            Validate(expectedHeight, expectedMaxHeight);

    //                expectedMaxHeight = 500;
    //                Logger.Status("[SET] Window.MaxHeight = " + expectedMaxHeight.ToString());
    //                this.MaxHeight = expectedMaxHeight;
    //                expectedHeight = 400;
    //                Validate(expectedHeight, expectedMaxHeight);

            Logger.Status("[SET] WindowState=Maximized");
            this.WindowState = WindowState.Maximized;
            expectedHeight = expectedMaxHeight;
            Validate(expectedHeight, expectedMaxHeight);

            Logger.Status("[SET] WindowState=Minimized");
            this.WindowState = WindowState.Minimized;
            expectedHeight = 300;
            Validate(expectedHeight, expectedMaxHeight);
            
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            expectedHeight = 300;
            Validate(expectedHeight, expectedMaxHeight);
            

		    TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedHeight, double expectedMaxHeight)
        {
            Logger.Status("[EXPECTED VALUES] Window.MaxHeight=" + expectedMaxHeight.ToString() + " Window.ActualHeight=" + expectedHeight.ToString());
            // Validate Property Value
            if ((this.MaxHeight != expectedMaxHeight) || (this.ActualHeight != expectedHeight))
            {
                Logger.LogFail("Window.MaxHeight=" + this.MaxHeight.ToString() + " Window.ActualHeight=" + this.ActualHeight.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Property Value Validated");
            }
            
            // Win32 Validation
            // Call Win32 validation only when Window is not minimized. This is because 
            // when Window is minimized, win32 reports 160 for width. Window.ActualHeight
            // property reports the actual restored width, which is better.
            if (this.WindowState != WindowState.Minimized)
            {
                if (WindowValidator.ValidateHeight(this.Title, expectedHeight))
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

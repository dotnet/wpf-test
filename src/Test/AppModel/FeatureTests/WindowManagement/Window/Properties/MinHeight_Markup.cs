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
    /// Test for setting Window.MinHeight property in Markup
    ///
    /// </summary>
    public partial class MinHeight_Markup
    {                                                                                                                                                                                                                                                              
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedHeight = 300;
            double expectedMinHeight = 300;
            Validate(expectedHeight, expectedMinHeight);
            
            Logger.Status("[SET] Window.Height = 400");
            this.MinHeight = 350;
            expectedHeight = 350;
            expectedMinHeight = 350;
            Validate(expectedHeight, expectedMinHeight);


    //                expectedMinHeight = 500;
    //                Logger.Status("[SET] Window.MinHeight = " + expectedMinHeight.ToString());
    //                this.MinHeight = expectedMinHeight;
    //                expectedHeight = 400;
    //                Validate(expectedHeight, expectedMinHeight);

            Logger.Status("[SET] WindowState=Maximized");
            this.WindowState = WindowState.Maximized;
            
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            Validate(expectedHeight, expectedMinHeight);

            Logger.Status("[SET] WindowState=Minimized");
            this.WindowState = WindowState.Maximized;
            
            Logger.Status("[SET] WindowState=Normal");
            this.WindowState = WindowState.Normal;
            Validate(expectedHeight, expectedMinHeight);
            

            TestHelper.Current.TestCleanup();

        }

        void Validate(double expectedHeight, double expectedMinHeight)
        {
            Logger.Status("[EXPECTED VALUES] Window.MinHeight=" + expectedMinHeight.ToString() + " Window.ActualHeight=" + expectedHeight.ToString());
            // Validate Property Value
            if (!TestUtil.IsEqual(this.MinHeight, expectedMinHeight)
                || !TestUtil.IsEqual(this.ActualHeight, expectedHeight))
            {
                Logger.LogFail("Window.MinHeight=" + this.MinHeight.ToString() + " Window.ActualHeight=" + this.ActualHeight.ToString());
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

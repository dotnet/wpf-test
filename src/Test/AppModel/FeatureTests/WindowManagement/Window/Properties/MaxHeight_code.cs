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
    /// Test for setting Window.MaxHeight property in code
    ///
    /// </summary>
    public partial class MaxHeight_code
    {                                                                                                                                                   
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedHeight = 300;
            double expectedMaxHeight = 300;
            this.MaxHeight = expectedMaxHeight;
            Validate(expectedHeight, expectedMaxHeight);

            Logger.Status("[SET] Height=400");
            this.Height = 400;
            Validate(expectedHeight, expectedMaxHeight);

            Logger.Status("[SET] SizeToContent=Height --> <Button Height=500>");
            this.SizeToContent = SizeToContent.Height;
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

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
    /// Test for setting Window.MaxWidth property in code
    ///
    /// </summary>
    public partial class MaxWidth_Code
    {                                                                                                                                                                                              
        void OnContentRendered(object sender, EventArgs e)
        {            
            // Expected Value
            double expectedWidth = 300;
            double expectedMaxWidth = 300;
            this.MaxWidth = expectedMaxWidth;
            Validate(expectedWidth, expectedMaxWidth);

            Logger.Status("[SET] Width=400");
            this.Width = 400;
            Validate(expectedWidth, expectedMaxWidth);

            Logger.Status("[SET] SizeToContent=WidthAndhHeight --> <Button Height=500 Width=500>");
            this.SizeToContent = SizeToContent.WidthAndHeight;
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

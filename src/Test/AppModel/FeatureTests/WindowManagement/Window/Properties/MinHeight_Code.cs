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
    /// Test for setting Window.MinHeight property in code
    ///
    /// </summary>
    public partial class MinHeight_Code
    {                                                                                                                                                                                                                                       
        void OnContentRendered(object sender, EventArgs e)
        {       
            this.SizeChanged += OnSizeChanged;
            // Expected Value
            int expectedHitCount = 3;
            
            double expectedHeight = 300;
            double expectedMinHeight = 300;
            Logger.Status("[SET] MinHeight=" + expectedMinHeight.ToString() + " --> Expecting Hit!");
            this.MinHeight = expectedMinHeight;
            Validate(expectedHeight, expectedMinHeight);

            Logger.Status("[SET] Height=400 --> Expecting Hit");
            expectedHeight = 400;
            this.Height = expectedHeight;
            Validate(expectedHeight, expectedMinHeight);

            Logger.Status("[SET] SizeToContent=Height --> <Button Height=250> --> Expecting Hit");
            expectedHeight = 300;
            this.SizeToContent = SizeToContent.Height;
            Validate(expectedHeight, expectedMinHeight);

            Logger.Status("Expected Hitcount == " + expectedHitCount.ToString());
            if (((int)Logger.GetHitCount("OnSizeChanged")) != expectedHitCount)
            {
                Logger.LogFail("Hitcount mismatch. Actual Hitcount=" + ((int)Logger.GetHitCount("OnSizeChanged")).ToString());
            }
            else
            {
                Logger.Status("Hitcount Validated");
            }
            
		    TestHelper.Current.TestCleanup();

        }

        void OnSizeChanged(object sender, EventArgs e)
        {
           Microsoft.Test.Logging.LogManager.LogMessageDangerously("this.Width=" + this.ActualWidth.ToString() + " this.Height=" + this.ActualHeight.ToString());
           Logger.RecordHit("OnSizeChanged");
        }

        
        void Validate(double expectedHeight, double expectedMinHeight)
        {
            Logger.Status("[EXPECTED VALUES] Window.MinHeight=" + expectedMinHeight.ToString() + " Window.ActualHeight=" + expectedHeight.ToString());
            // Validate Property Value
            if ((this.MinHeight != expectedMinHeight) || (this.ActualHeight != expectedHeight))
            {
                Logger.LogFail("Window.MinHeight=" + this.MinHeight.ToString() + " Window.ActualHeight=" + this.ActualHeight.ToString());
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

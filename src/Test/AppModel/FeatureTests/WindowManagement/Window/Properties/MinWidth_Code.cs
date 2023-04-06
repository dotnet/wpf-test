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
    /// Test for setting Window.MinWidth property in code
    /// </summary>
    public partial class MinWidth_Code
    {
        void OnContentRendered(object sender, EventArgs e)
        {
            this.SizeChanged += OnSizeChanged;
            // Expected Value
            int expectedHitCount = 3;

            double expectedWidth = 300;
            double expectedMinWidth = 300;
            Logger.Status("[SET] MinWidth=" + expectedMinWidth.ToString() + " --> Expecting Hit!");
            this.MinWidth = expectedMinWidth;
            Validate(expectedWidth, expectedMinWidth);

            Logger.Status("[SET] Width=400 --> Expecting Hit");
            expectedWidth = 400;
            this.Width = expectedWidth;
            Validate(expectedWidth, expectedMinWidth);

            Logger.Status("[SET] SizeToContent=Width --> <Button Width=250> --> Expecting Hit");
            expectedWidth = 300;
            this.SizeToContent = SizeToContent.Width;
            Validate(expectedWidth, expectedMinWidth);

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
            Microsoft.Test.Logging.LogManager.LogMessageDangerously("this.Width=" + this.ActualWidth.ToString() + " this.Width=" + this.ActualWidth.ToString());
            Logger.RecordHit("OnSizeChanged");
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

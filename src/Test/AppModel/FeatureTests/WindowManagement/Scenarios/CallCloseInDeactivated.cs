// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Automation;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Windows.Test.Client.AppSec.BVT.Window;
using Microsoft.Test.Win32;

namespace WindowTest
{

    /// <summary>
    /// Test for calling Window.Close() method in Deactivated Event
    /// </summary>
    public partial class CallCloseInDeactivated
    {
        private AutomationHelper _AH = new AutomationHelper();

        void OnDeactivated(object sender, EventArgs e)
        {
            Logger.Status("[OnDeactivated] Calling Close()");
            this.Close();
        }

        void OnContentRendered(object sender, EventArgs e)
        {
            // Needed because the standard exit logs a result to make up for the new Infra's different behavior.
            Application.Current.Exit -= WindowTestApp.StandardExit;
            Application.Current.Exit += OnExit;
            Logger.Status("Deactivating Window by clicking outside ClientArea");
            //the mouse on  position y(0)  will get Cursor, not a arrow, click doesnt work,
            //adjust y from 0 to 5 to get it work.
            _AH.MoveToAndClick(new Point(this.Left + this.ActualWidth + 2, 5));

        }

        void OnExit(object sender, EventArgs e)
        {
            Logger.LogPass("Closed Event Fired - Test case passes if Window was not forced to close due to Timeout.");
            TestHelper.CloseAndSetVariationResults();
        }
    }

}

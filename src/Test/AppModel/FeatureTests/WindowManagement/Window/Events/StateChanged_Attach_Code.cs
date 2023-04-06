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
    /// Test for Attaching and firing StateChanged Event in Code
    ///
    /// </summary>
    public partial class StateChanged_Attach_Code
    {
        int _expectedHitCount = 0;
        WindowState _expectedValue;

        void OnStateChanged(object sender, EventArgs e)
        {
            Logger.RecordHit("OnStateChanged");
        }

        void OnContentRendered(object sender, EventArgs e)
        {

            Logger.Status("Attaching Window.StateChanged Event Handler");
            this.StateChanged += new EventHandler(OnStateChanged);

            _expectedValue = WindowState.Normal;
            ValidateState();

            Logger.Status("[SET] Window.WindowState=Maximized - Expect HitCount");
            _expectedValue = WindowState.Maximized;
            this.WindowState = WindowState.Maximized;
            _expectedHitCount++;
            ValidateState();

            Logger.Status("[SET] Window.WindowState=Minimized - Expect HitCount");
            _expectedValue = WindowState.Minimized;
            this.WindowState = WindowState.Minimized;
            _expectedHitCount++;
            ValidateState();

            // BVT BLOCKER #1230907: Setting Window.WindowState from Maximized to Minimized then to Normal does not work
            /*
            Logger.Status("[SET] Window.WindowState=Minimized - Expect HitCount");
            ExpectedValue=WindowState.Normal;
            this.WindowState = WindowState.Normal;
            ExpectedHitCount++;
            ValidateState();                
            */

            Logger.Status("[EXPECTED] HitCount = " + _expectedHitCount.ToString());
            int ActualHitCount = Logger.GetHitCount("OnStateChanged");
            if (ActualHitCount != _expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] HitCount = " + ActualHitCount.ToString());
            }

            TestHelper.Current.TestCleanup();


        }

        void ValidateState()
        {
            Logger.Status("VALIDATE PROPERTY");
            Logger.Status("[EXPECTED] State = " + _expectedValue.ToString());
            if (this.WindowState != _expectedValue)
            {
                Logger.LogFail("[ACTUAL] WindowState = " + this.WindowState.ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] WindowState = " + this.WindowState.ToString());
            }
        }
    }

}

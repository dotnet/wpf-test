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
    /// Test for Child windows minimize and closes when parent window minimizes and closes
    ///
    /// </summary>
    public partial class OwnerMinimizeAndCloseChildWindows
    {                   
        void OnContentRendered(object sender, EventArgs e)
        {   
            WindowState expectedWindowState;
            int expectedHitCount=4;
            
            Window Child1 = new Window();
            Child1.Title = "Child1";
            Child1.Show();
            Child1.Closed+=OnClosed;
            Logger.Status("Adding Child1");
            Child1.Owner = this;
            
            Window Child2 = new Window();
            Logger.Status("Adding Child2");
            Child2.Owner = this;
            Child2.Title = "Child2";
            Child2.Show();
            Child1.Closed+=OnClosed;

            expectedWindowState = WindowState.Minimized;
            Logger.Status("[SET] Parent.WindowState=" + expectedWindowState.ToString());
            this.WindowState = expectedWindowState;
            Validate(expectedWindowState, 2);

            expectedWindowState = WindowState.Maximized;
            Logger.Status("[SET] Parent.WindowState=" + expectedWindowState.ToString());
            this.WindowState = expectedWindowState;
            
            Window Child3 = new Window();
            Logger.Status("Adding Child3");
            Child3.Owner = this;
            Child3.Title = "Child3";
            Child3.Show();
            Child3.Closed+=OnClosed;
            Validate(expectedWindowState, 3);

            expectedWindowState = WindowState.Minimized;
            Logger.Status("[SET] Parent.WindowState=" + expectedWindowState.ToString());
            this.WindowState = expectedWindowState;

            expectedWindowState = WindowState.Normal;
            Logger.Status("[SET] Parent.WindowState=" + expectedWindowState.ToString());
            this.WindowState = expectedWindowState;
            Validate(expectedWindowState, 3);

            Logger.Status("Closing Parent Window");
            this.Close();

            Logger.Status("[EXPECTED] Window.Closed expectedHitCount == " + expectedHitCount.ToString());
            if (((int)Logger.GetHitCount("OnClosed")) != expectedHitCount)
            {
                Logger.LogFail("[ACTUAL] " + ((int)Logger.GetHitCount("OnClosed")).ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] " + ((int)Logger.GetHitCount("OnClosed")).ToString());
            }

            Logger.LogPass("");

        }

        void OnClosed(object sender, EventArgs e)
        {
            Logger.Status("Sender==" + ((Window)sender).Title);
            Logger.RecordHit("OnClosed");
        }
        
        void Validate(WindowState ExpectedWindowState, int OwnedWindowCount)
        {
            Visibility ExpectedVisibility = Visibility.Visible;
            
            if (ExpectedWindowState == WindowState.Minimized)
            {
                ExpectedVisibility = Visibility.Hidden;
            }
            
            Logger.Status("[EXPECTED] OwnedWindowCount=" + OwnedWindowCount.ToString());
            Logger.Status("[EXPECTED] ExpectedWindowState=" + ExpectedWindowState.ToString());
            Logger.Status("[EXPECTED] ExpectedVisibility=" + ExpectedVisibility.ToString());
            if (this.OwnedWindows.Count != OwnedWindowCount)
            {
                Logger.LogFail("[ACTUAL] OwnedWindows.Count == " + this.OwnedWindows.Count.ToString());
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] OwnedWindows.Count == " + this.OwnedWindows.Count.ToString());
            }

            for(int i=0; i<this.OwnedWindows.Count; i++)
            {
                if (this.OwnedWindows[i].Visibility != ExpectedVisibility)
                {
                    Logger.LogFail(this.OwnedWindows[i].Title.ToString() + " != " + ExpectedVisibility.ToString());
                }
                else
                {
                    Logger.Status("[VALIDATION PASSED] " + this.OwnedWindows[i].Title.ToString() + " == " + ExpectedVisibility.ToString());
                }
            }
        }
    }

}

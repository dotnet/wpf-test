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
    ///  Description: Test for Topmost Window
    ///  Test Logic:   (1) Create a parent window
    ///                (2) Create a TopMost child window
    ///                (3) Set child window to be inside parent window
    ///                (4) Click outside child window, but inside parent window
    ///                (8) Click inside Child window
    ///                (9) If child window is topmost, then it shoud receive Activated event
    ///
    /// </summary>
    public partial class TopmostWindowClass
    {                                       
        AutomationHelper _AH = new AutomationHelper();
        Window _topmostWindow;
        Point _insideNormalWindow,_insideTopmostWindow;
        int _expectedDeactivatedHitCount = 1;
        
        void OnContentRendered(object sender, EventArgs e)
        {
            _topmostWindow = new Window();
            _topmostWindow.Title = "Avalon.Window.Test - TopmostWindow";
            _topmostWindow.Width = 200;
            _topmostWindow.Height = 200;
            _topmostWindow.Top = 100;
            _topmostWindow.Left = 100;
            _topmostWindow.Topmost = true;
            _topmostWindow.Content = new Button();
            _topmostWindow.ContentRendered += OnTopmostWindowContentRendered;
            _topmostWindow.Show();
        }

        void OnTopmostWindowContentRendered(object sender, EventArgs e)
        {
            Point insideNormalWindowLogicalUnits = new Point(this.Left + (_topmostWindow.Left - this.Left) / 2, this.Top + (_topmostWindow.Top - this.Top) / 2);
            Point insideTopmostWindowLogicalUnits = new Point(_topmostWindow.Left + 10, _topmostWindow.Top + 10);

            _insideNormalWindow = TestUtil.LogicalToDeviceUnits(insideNormalWindowLogicalUnits, this);
            _insideTopmostWindow = TestUtil.LogicalToDeviceUnits(insideTopmostWindowLogicalUnits, this);
            
            _topmostWindow.Activated+=OnTopmostWindowActivated;
            _topmostWindow.Deactivated+=OnTopmostWindowDeactivated;
            _AH.MoveToAndClick(_insideNormalWindow);
        }

        void OnTopmostWindowDeactivated(object sender, EventArgs e)
        {
            Logger.RecordHit("OnTopmostWindowDeactivated");
            _AH.MoveToAndClick(_insideTopmostWindow);
        }
        
        void OnTopmostWindowActivated(object sender, EventArgs e)
        {
            _topmostWindow.Deactivated-=OnTopmostWindowDeactivated;
            Logger.Status("[EXPECTED] ExpectedDeactivatedHitCount = " + _expectedDeactivatedHitCount.ToString());
            if ((int)Logger.GetHitCount("OnTopmostWindowDeactivated") != _expectedDeactivatedHitCount)
            {
                Logger.LogFail("[ACTUAL] ExpectedDeactivatedHitCount = " + Logger.GetHitCount("OnTopmostWindowDeactivated").ToString());
            }
            else
            {
                Logger.Status("[ACTUAL] ExpectedDeactivatedHitCount = " + Logger.GetHitCount("OnTopmostWindowDeactivated").ToString());
                Logger.Status("[VALIDATION PASSED]");
            }
            
            TestHelper.Current.TestCleanup();
        }
    }

}

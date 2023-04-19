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
    ///  Description: Test for ESC key can close Dialog window if Cancel button present
    ///  Test Logic:   (1) Create two windows.. parentWin and dialogWin
    ///                (2) Create Cancel button for dialogWin
    ///                (3) Add Closed event handler for dialogWin
    ///                (4) Send keyboard ESC key to dialogWin
    ///                (5) Verify dialogWin closes
    ///                (6) Verify return value of ShowDialog
    /// </summary>
    public partial class ESCClosesDialogWindow
    {                                                                                                                          
        AutomationHelper _AH = new AutomationHelper();

        Window _dialogWindow;
        Button _cancelButton;
        bool _cancelClosing = true;

        void OnContentRendered(object sender, EventArgs e)
        {               
            // Create a cancel button
            _cancelButton = new Button();
            _cancelButton.Content = "Cancel";
            _cancelButton.Width = 100;
            _cancelButton.Height = 50;
            _cancelButton.IsCancel = true;
            _cancelButton.IsDefault = false;
            _cancelButton.Click += OnBtnClick;
            
            // Create a dialog window
            _dialogWindow = new Window();
            _dialogWindow.Width = this.Width;
            _dialogWindow.Height = this.Height;
            _dialogWindow.Title = "DialogWindow";
            _dialogWindow.Closed += dialogWindowClosed;
            _dialogWindow.Closing += dialogWindowClosing;
            _dialogWindow.Activated += new EventHandler(dialogActivated);
            
            Logger.Status("[SET] DialogWindow.Content = Default Button");
            _dialogWindow.Content = _cancelButton;

            Logger.Status("Show Dialog Window");
            _dialogWindow.ShowDialog();
            
        }

        void dialogActivated(object sender, EventArgs e)
        {   
            Logger.Status("Sending ESC key to dialogWindow, which should sending Closing event (to be canceled)");
            Input.SendKeyboardInput(System.Windows.Input.Key.Escape, true);
        }

        void OnBtnClick(object sender, RoutedEventArgs e)
        {
            Logger.RecordHit("OnBtnClick");
        }
        
        void dialogWindowClosed(object sender, EventArgs e)
        {
            Logger.Status("[EXPECTED] Closing HitCount = 2");
            Logger.Status("[EXPECTED] Button.CLick HitCount = 2");

            if ((int)Logger.GetHitCount("dialogWindowClosing") != 2 || (int)Logger.GetHitCount("OnBtnClick") != 2)
            {
                Logger.LogFail("[ACTUAL] Closing HitCount = " + Logger.GetHitCount("dialogWindowClosing").ToString());
                Logger.LogFail("[ACTUAL] Button.CLick HitCount = " + Logger.GetHitCount("OnBtnClick").ToString());
            }
            else
            {
                Logger.LogPass("[VALIDATION PASSED]");
            }

            this.Close();

        }

        void dialogWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.RecordHit("dialogWindowClosing");
            if (_cancelClosing)
            {
                _cancelClosing = false;
                Logger.Status("[SET] Cancel button the default button");
                _cancelButton.IsDefault = true;
                e.Cancel = true;
                Logger.Status("Sending ENTER key to dialogWindow");
                Input.SendKeyboardInput(System.Windows.Input.Key.Enter, true);
            }
        }

    }

}

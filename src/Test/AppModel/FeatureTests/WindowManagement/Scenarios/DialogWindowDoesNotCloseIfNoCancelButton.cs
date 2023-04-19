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
    /// est for ESC key does not close Dialog window if there is no cancel button present
    /// </summary>
    public partial class DialogWindowDoesNotCloseIfNoCancelButton
    {                                                                                                                 
        AutomationHelper _AH = new AutomationHelper();

        Window _dialogWindow;
        Button _cancelButton;
        System.Timers.Timer _timer;

        void OnContentRendered(object sender, EventArgs e)
        {               

            _timer = new System.Timers.Timer(3000);
            _timer.Elapsed += new System.Timers.ElapsedEventHandler(timerElapsed);

            _cancelButton = new Button();
            _cancelButton.Content = "Cancel";
            _cancelButton.Width = 100;
            _cancelButton.Height = 50;
            _cancelButton.IsCancel = false;
            _cancelButton.IsDefault = true;
            
            // Create a dialog window
            _dialogWindow = new Window();
            _dialogWindow.Width = this.Width;
            _dialogWindow.Height = this.Height;
            _dialogWindow.Title = "DialogWindow";
            _dialogWindow.Closed += dialogWindowClosed;
            _dialogWindow.Closing += dialogWindowClosing;
            _dialogWindow.ContentRendered += new EventHandler(dialogContentRendered);
            
            Logger.Status("[SET] DialogWindow.Content = Default Button");
            _dialogWindow.Content = _cancelButton;

            Logger.Status("Show Dialog Window");
            _dialogWindow.ShowDialog();
            
        }

        void dialogContentRendered(object sender, EventArgs e)
        {            
            _timer.Start();
            Logger.Status("Sending ESC key to dialogWindow, which should sending Closing event (to be canceled)");
            Input.SendKeyboardInput(System.Windows.Input.Key.Escape, true);
            
        }

        void timerElapsed(object sender, System.Timers.ElapsedEventArgs e)
        {            
            _timer.Stop();
            Logger.Status("Detaching Closing and Closed event handlers on Dialog Window");
            _dialogWindow.Closed -= dialogWindowClosed;
            _dialogWindow.Closing -= dialogWindowClosing;
            TestHelper.Current.TestCleanup();
        }


        void dialogWindowClosed(object sender, EventArgs e)
        {
            Logger.LogFail("Dialog Closed Event Caught!");
        }

        void dialogWindowClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.LogFail("Dialog Closing Event Caught!");
        }
    }

}

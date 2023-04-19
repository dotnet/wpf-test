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
    ///       window can as show threadModel dialog
    ///
    ///
    /// Test Logic:   (1) Create a normal window of size 500x500
    ///                (2) Create a dialogWin of size 200x200
    ///                (3) Place dialogWin inside Normal Window
    ///                (4) Click point X at (50,50) which is gurrenteed 
    ///                    to be outside DialogWin but inside NormalWin
    ///                (5) Start timer for 3 seconds. If Normal win did
    ///                    not receive activated event, log test pass.
    ///
    ///  TEST CASE Logic Diagram
    ///                  *******************************
    ///                  |NormalWindow                 |
    ///                  |                             |
    ///                  |    x (50,50)                |
    ///                  |                             |
    ///                  |                             |
    ///                  |    ****************         |
    ///                  |    |DialogWindow  |         |
    ///                  |    |              |         |500px
    ///                  |    |              |200px    |
    ///                  |    |              |         |
    ///                  |    |              |         |
    ///                  |    ******200px*****         |
    ///                  |                             |
    ///                  |                             |
    ///                  |                             |
    ///                  |                             |
    ///                  *************500px*************
    ///
    /// </summary>

    public partial class ModalDialogWindow
    {                                                                                                                                              
        AutomationHelper _AH = new AutomationHelper();
        Window _dialogWindow;
        System.Timers.Timer _t1, _t2;
        
        void OnContentRendered(object sender, EventArgs e)
        {
             _t1 = new System.Timers.Timer(2000);
             _t2 = new System.Timers.Timer(2000);

             _t1.Elapsed += new System.Timers.ElapsedEventHandler(t1_Elapsed);
             _t2.Elapsed += new System.Timers.ElapsedEventHandler(t2_Elapsed);

            // Set dialog window to be 200x200, starting from (100,100)
            _dialogWindow = new Window();
            _dialogWindow.Left = 100;
            _dialogWindow.Top = 100;
            _dialogWindow.Width = 200;
            _dialogWindow.Height = 200;
            _dialogWindow.Title = "DialogWindow";
            
            Logger.Status("Setting _dialogWindow.Content to a new docPanel");
            _dialogWindow.Content = new DockPanel();
            
            Logger.Status("Creating ContentRendered event handler for DialogWindow");
            _dialogWindow.ContentRendered += new EventHandler(DialogWindow_ContentRendered);            

            Logger.Status("Start timer1, which triggers MouseMoveAndClick()");
            _t1.Start();

            // ContentRendered event will only trigger when a window contains UI element, such as a docPanel.
            _dialogWindow.ShowDialog();


        }

        private void DialogWindow_ContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Adding Activated event handler for NormalWindow");
            this.Activated += new EventHandler(NormalWindow_Activated);
        }

        private void NormalWindow_Activated(object sender, EventArgs e)
        {
            Logger.Status("NormalWindow has gained Focus");
            Logger.LogFail("NormalWindow should never gain forcus. DialogWindow was not thread modal");
            Application.Current.Shutdown();
        }

        private void t1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("Timer1 has elapsed. Stop timer1");
            _t1.Stop();

            // Click on NormalWindow, Point X on Diagram above
            Logger.Status("MoveAndClick on NormalWindow at 50,50");
            _AH.MoveToAndClick(new Point(50,50));

            Logger.Status("Start timer2, which Close App and Log Results");
            _t2.Start();
        }

        private void t2_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            Logger.Status("Timer2 has elapsed. Stop timer2");
            _t2.Stop();
            
            Logger.LogPass("NormalWindow did not get Activated event. i.e. DialogWindow is modal");

            // Remove NormalWindow Activated event, prior to shutdown()
            this.Activated -= NormalWindow_Activated;
            Logger.Status("Shutting down app");
            this.Dispatcher.BeginInvoke(System.Windows.Threading.DispatcherPriority.Send, (System.Windows.Threading.DispatcherOperationCallback) delegate (object o)
                {
                    Application.Current.Shutdown();
                    return null;
                },
                null);
        }  
    }

}

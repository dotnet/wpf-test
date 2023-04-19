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
    /// Test for Detach StateChanged Event
    ///
    /// </summary>
    public partial class StateChanged_Detach
    {         
        void OnStateChanged(object sender, EventArgs e)
        {
            Logger.LogFail("StateChanged Should not have been caught!");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
      
            Logger.Status("Attaching Window.StateChanged Event Handler");
            this.StateChanged += new EventHandler(OnStateChanged);

            Logger.Status("Detaching Window.StateChanged Event Handler");
            this.StateChanged -= OnStateChanged;

            Logger.Status("[SET] Window.WindowState=Maximized - Expect HitCount");
            this.WindowState=WindowState.Maximized;

            Logger.Status("[SET] Window.WindowState=Minimized - Expect HitCount");
            this.WindowState=WindowState.Minimized;

            Logger.Status("[SET] Window.WindowState=Minimized - Expect HitCount");
            this.WindowState=WindowState.Normal;
            
            TestHelper.Current.TestCleanup();


        }
    }

}

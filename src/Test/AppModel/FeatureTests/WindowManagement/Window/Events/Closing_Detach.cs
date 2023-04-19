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
    /// Test for Detaching Closing Event
    ///
    /// </summary>
    public partial class Closing_Detach
    {                                                                                                                                                     
        void OnClosed(object sender, EventArgs e)
        {
            TestHelper.Current.TestCleanup();
        }
        void OnClosing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Logger.LogFail("This event should not fire");
        }   
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.Closing Event Handler");
            this.Closing += new System.ComponentModel.CancelEventHandler(OnClosing);

            Logger.Status("Detaching Closed Event Handler");
            this.Closing -= OnClosing;

            Logger.Status("Closing Window");
            this.Close();
        }
    }

}

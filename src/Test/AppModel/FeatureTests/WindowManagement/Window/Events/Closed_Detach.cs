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
    /// Test for Detaching Closed Event
    ///
    /// </summary>
    public partial class Closed_Detach
    {                                                                                    
        void OnClosed(object sender, EventArgs e)
        {
		    Logger.LogFail("This event should not fire");
        }
        
        void OnContentRendered(object sender, EventArgs e)
        {
            Logger.Status("Attaching Window.Closed Event Handler");
            this.Closed += new EventHandler(OnClosed);
            
            Logger.Status("Detaching Window.Closed Event Handler");
            this.Closed -= OnClosed;
            
            Logger.Status("Closing Window");
            this.Close();
            
		    TestHelper.Current.TestCleanup();
            
        }
    }

}

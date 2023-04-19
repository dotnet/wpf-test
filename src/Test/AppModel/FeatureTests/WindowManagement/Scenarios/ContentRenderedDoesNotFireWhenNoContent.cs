// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Automation;
using Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon;
using Microsoft.Test.Win32;

namespace WindowTest
{

    /// <summary>
    /// Test to ensure ContentRendered event does not fire when there is no content in the window
    /// </summary>
    public partial class ContentRenderedDoesNotFireWhenNoContent
    {                                                   
        System.Timers.Timer _t1;
        bool _expectToFire = false;
        
        void OnActivated(object sender, EventArgs e)
        {
             _t1 = new System.Timers.Timer(5000);
             _t1.Elapsed += new System.Timers.ElapsedEventHandler(t1_Elapsed);

            Logger.Status("Start timer1, which triggers MouseMoveAndClick()");
            _t1.Start();
        }

        private void t1_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
    	    Logger.Status("Timer elapsed!");
            _expectToFire = true;
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.Send, (DispatcherOperationCallback) delegate (object o)
            {
        	    Logger.Status("[SET]Window.Content = new FrameworkElement()");
			    this.Content = new FrameworkElement();
                return null;
            },
            null);
        }

        private void OnContentRendered(object sender, EventArgs e)
        {
    	    if (!_expectToFire)
			    Logger.LogFail("ContentRendered Event Caught! -> Should not have fired.");
		    else
			    TestHelper.Current.TestCleanup();	
        }  
    }

}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
*************************************************************************************
*                                                                                   *
*  Title:                                                                           *
*      The Base App for Markup PageFunction Tests                                   *
*                                                                                   *
*  Description:                                                                     *
*      Base app for the markup testcases for PageFunctions. Basically the same      *
*      as for the base app for code pagefunction testing, but has less features     *
*                                                                                   *
*                                                       *
*                                                                                   *
*                                                                                   *
*                                                                                   *
*                                                                                   * 
*************************************************************************************
*/

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using System.Timers;
using System.Threading;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation 
{
    public class BasePFNavApp 
    {
        private string _desc;
        
        public static Application MainApp 
        {
            get 
            {
                if (Application.Current == null) 
                {
                    //NavigationHelper.Output("Full type info of app not stored in the exec app assembly");
                    NavigationHelper.Fail("Application.Current is null");
                } 
                return Application.Current;
            }
        }
        
        
        public BasePFNavApp() 
        {
        }
                
        public string TestCaseDesc {
            set 
            {
                _desc = value;
                NavigationHelper.Output("Testcase Description: " + _desc);
            }
            
            get 
            {
                if (_desc == String.Empty) 
                {
                    return "";
                } 
                else 
                {
                    return _desc;
                }
            }
        }
        
        protected void ShutdownIn(int milliseconds) 
        {
            NavigationHelper.Output("Shutting down after a delay of: " + milliseconds);
            _timershut = new System.Timers.Timer(milliseconds);
            _timershut.Elapsed += new ElapsedEventHandler(DoShutdownIn);
            _timershut.Start();
        }
        
        private void DoShutdownIn(object sender, ElapsedEventArgs e) 
        {
            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Cleanup);
            _timershut.Dispose();
            Application.Current.Dispatcher.BeginInvoke(
                 DispatcherPriority.Send,
                 (DispatcherOperationCallback) delegate (object notused) 
                 {
                     Application.Current.Shutdown(0);
                     return null;
                 },
                 null);
        }
        
        private System.Timers.Timer _timershut;

    }
}

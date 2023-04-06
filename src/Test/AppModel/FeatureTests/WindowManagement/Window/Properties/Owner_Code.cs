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
    /// Test for setting Window.Owner property in Code
    ///
    /// </summary>
    public partial class Owner_Code
    {                                                                                                                                                                                                                                                                                                                                                                       
        Window _expectedValue = null;
        void OnContentRendered(object sender, EventArgs e)
        {   
            Logger.Status("[SET] this.Owner = this (After show) --> Expect ArgumentException");
            try
            {
                this.Owner = this;
            }
            catch(System.ArgumentException ae)
            {
                Logger.Status("[VALIDATION PASSED] ArgumentException caught as expected\n" + ae.ToString());
            }
            catch(System.Exception ex2)
            {
                Logger.LogFail("Unexpected exception caught!\n + " + ex2.ToString());
            }

            Logger.Status("[SET] this.Owner = this (Before show) --> Expect ArgumentException");
            try
            {
                Window win = new Window();
                win.Owner = win;
                win.Show();
            }
            catch(System.ArgumentException ae)
            {
                Logger.Status("[VALIDATION PASSED] ArgumentException caught as expected\n" + ae.ToString());
            }
            catch(System.Exception ex2)
            {
                Logger.LogFail("Unexpected exception caught!\n + " + ex2.ToString());
            }
            
            
            Window ParentWindow1 = new Window();
            ParentWindow1.Title = "ParentWindow1";
            
            Window ParentWindow2 = new Window();
            ParentWindow2.Title = "ParentWindow2";

            try
            {
                Logger.Status("[SET] Owner to UnShown window of = " + ParentWindow1.Title + " --> Expect InvalidOperationException");
                this.Owner = ParentWindow1;
            }
            catch(InvalidOperationException ex)
            {
                Logger.Status("[VALIDATION PASSED] InvalidOperationException caught as expected\n" + ex.ToString());
            }
            catch(Exception ex2)
            {
                Logger.LogFail("Unexpected exception caught!\n + " + ex2.ToString());
            }
            
            Validate();

            Logger.Status("Showing " + ParentWindow1.Title);
            ParentWindow1.Show();
            Validate();

            ParentWindow2.Show();
            Logger.Status("[SET] Owner to shown window of = " + ParentWindow2.Title);
            _expectedValue = ParentWindow2;
            this.Owner = _expectedValue;
            Validate();

            Logger.Status("[SET] Owner to null");
            _expectedValue = null;
            this.Owner = null;
            if (this.Owner != null)
            {
                Logger.LogFail("Owner != null");
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Owner == null");
            }

            ParentWindow2.Close();
            ParentWindow1.Close();
            Logger.Status("[DETACHING] Closing event handler");
            this.Closing -= OnClosing;                

            TestHelper.Current.TestCleanup();

        }

        void OnClosing(object sender, EventArgs e)
        {
            Logger.LogFail("Closing event shouldn't have fired!");
        }
        
        void Validate()
        {
            if (this.Owner != _expectedValue)
            {
                Logger.LogFail("Owner != " + _expectedValue.Title);
            }
            else
            {
                Logger.Status("[VALIDATION PASSED] Owner == " + (_expectedValue==null ? "null" : _expectedValue.Title));
            }
        }
    }

}

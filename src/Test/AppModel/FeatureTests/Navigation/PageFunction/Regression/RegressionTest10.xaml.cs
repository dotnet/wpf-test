// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Windows.Test.Client.AppSec.Navigation
{
    /// <summary>
    /// Interaction logic for RegressionTest10.xaml
    /// </summary>
    public partial class Regression10 : Page
    {
        public Regression10() { }

        public static void Regression10_Startup(object sender, StartupEventArgs e)
        {
            NavigationHelper.CreateLog("Regression Test 10");
            NavigationHelper.Output("Testing correct exception thrown (Invalid Operation Exception versus Invariant Assert) on invalid PF scenario");

            NavigationHelper.Output("App starting up");
            Application.Current.StartupUri = new Uri("RegressionTest10.xaml" , UriKind.RelativeOrAbsolute);

            NavigationHelper.SetStage(Microsoft.Test.Logging.TestStage.Run);
        }

        public static void Regression10_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            e.Handled = true;
            NavigationHelper.Output("DispatcherUnhandledException handler: ");

            if (e.Exception is System.ExecutionEngineException)
            {
                NavigationHelper.Fail("FAIL: ExecutionEngineException thrown (really Invariant Assert). Dev10 10 may have regressed.");                
            }
            else if (e.Exception is System.NotSupportedException)
            {
                NavigationHelper.PassTest("Got expected NotSupported Exception when using PageFunction w/ return event handler not declared on calling class (D10 10)");
                e.Handled = true;
            }
            else 
            {
                NavigationHelper.Fail("Hit unexpected " + e.Exception.GetType().ToString() + " : " + "\n" + e.Exception.StackTrace.ToString());
            }
        }
    }
}

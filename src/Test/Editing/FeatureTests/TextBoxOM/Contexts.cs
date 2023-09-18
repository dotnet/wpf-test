// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for verfiying TextBox behavior when used
//  in multiple Dispatchers.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;
    using System.Threading; 

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Input;    
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that Text Controls instances can be created in different
    /// Dispatcher instances.
    /// </summary>
    [Test(0, "TextBox", "TextControlContextCreation", MethodName = "DriverEntryPoint", MethodParameters = "/TestCaseType=TextControlContextCreation", TestParameters = "Class=EntryPointType")]
    [TestOwner("Microsoft"), TestTactics("647"), TestBugs("667")]
    public class TextControlContextCreation
    {
        #region Main flow.
        
        private Window _window;

        /// <summary>Runs the test case.</summary>
        [TestEntryPoint]
        public void RunTestCase()
        {
            for (int i = 0; i < 10; i++)
            {
                Logger.Current.Log("Iteration: " + i);
                Logger.Current.Log("Creating a dispatcher and a context...");
                try
                {
                    QueueHelper queueHelper = new QueueHelper(Dispatcher.CurrentDispatcher);
                    Control textcontrol;
                    if (i < 5)
                    {
                        Logger.Current.Log("Creating a TextBox...");
                        textcontrol = new TextBox();
                    }
                    else
                    {
                        Logger.Current.Log("Creating a RichTextBox...");
                        textcontrol = new RichTextBox();
                    }
                    _window = new Window();
                    _window.Content = textcontrol;

                    Logger.Current.Log("Showing the window...");
                    queueHelper.QueueDelegate(new SimpleHandler(CloseWindow));
                    _window.ShowDialog();
                    Logger.Current.Log("Dialog shown and closed");
                }
                finally
                {                    
                }
            }
            Logger.Current.ReportSuccess();
            if (Logger.Current.TestLog != null)
            {
                Logger.Current.TestLog.Close();
            }
        }

        private void CloseWindow()
        {
            Logger.Current.Log("Dispatch queue has gone idle...");
            _window.Close();
        }

        #endregion Main flow.
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Wpf.AppModel
{
    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Interop;
    using System.Windows.Threading;
    using System.Xml;
    using Microsoft.Test.Logging;
    
    public partial class TestHelper
    {
        // Private properties
        private string[] _appArgs = null;

        // Static properties
        static private TestHelper s_currentInstance = null;

        public TestHelper()
        {
            Initialize(null);
        }
        
        public TestHelper(string[] appArgs)
        {
            Initialize(appArgs);
        }

        private void Initialize(string[] args)
        {            
            s_currentInstance = this;
            if (args != null && args.Length != 0)
            {
                _appArgs = args;
            }
            else
            {
                //appArgs = new String[] {Harness.Current["TestName"], Harness.Current["Param1"], Harness.Current["Param2"], Harness.Current["Param3"]};
                throw new NotImplementedException("Test is trying to access the harness. Figure out what scenario this is for and what it needs.");
            }
        }

        public void TestCleanup()
        {
            Logger.LogPass();
            
            // cleanup!
            Logger.LogStatus("Shutting down application");
            Application.Current.Dispatcher.BeginInvoke(DispatcherPriority.ApplicationIdle, (DispatcherOperationCallback) delegate (object o)
            {
                try
                {
                    Application.Current.Shutdown();
                }
                catch(System.Security.SecurityException ex)
                {
                    Logger.LogStatus("SecurityException caught. " + ex.ToString());
                }
                catch(System.Exception ex)
                {
                    Logger.LogFail("Application Exit threw exception. " + ex.ToString());
                }
                return null;
            },
            null);
        }

        static public TestHelper Current
        {
            get
            {
                return s_currentInstance;
            }
        }

        public string[] AppArgs
        {
            set
            {
                _appArgs = value;
            }
            get
            {
                return _appArgs;
            }
        }
    }
}

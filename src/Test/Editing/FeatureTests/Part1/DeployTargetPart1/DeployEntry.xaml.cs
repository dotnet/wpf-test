// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Editing
{   
    using System;
    using System.Windows;
    using System.Windows.Threading;
    using Microsoft.Test.Loaders;
    using Microsoft.Test.Logging;
    using Test.Uis.Loggers;

    public partial class EditingTestApplicationPart1 : Application
    {
        protected override void OnStartup(StartupEventArgs args)
        {
            string[] arguments;

            // The *.xbap must be loaded by AppMonitor.exe to get the arguments.
            arguments = ApplicationMonitor.GetArguments();
            Application.Current.DispatcherUnhandledException += new DispatcherUnhandledExceptionEventHandler(Current_DispatcherUnhandledException);
            // ApplicationMonitor.GetArguments() return new string[0] if no argument is specified.
            if (arguments.Length > 0 && !(arguments[0] == null || arguments[0] == string.Empty))
            {
                Test.Uis.Management.TestRunner.DoMain(arguments);
            }
        }

        delegate void DumpExceptionDelegate(Exception e, ExceptionDumpKinds kinds);
        void Current_DispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            Dispatcher.CurrentDispatcher.Invoke(DispatcherPriority.ApplicationIdle, new DumpExceptionDelegate(Logger.Current.DumpException), e.Exception, ExceptionDumpKinds.Default);
            if (Logger.Current.TestLog != null)
            {
                Logger.Current.TestLog.Result = TestResult.Fail;
                Logger.Current.TestLog.Close();

            }
            ApplicationMonitor.NotifyStopMonitoring();
        }
    }
}


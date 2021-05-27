// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// System
using System;
using System.Xml;
using System.Threading;
using System.Security.Permissions;
using System.IO;

// Avalon
using System.Windows;
using System.Windows.Threading;
using System.Windows.Markup;

// Test
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.CrossProcess;

namespace Microsoft.Test.Integration.Windows
{
    /// <summary>
    /// Base type of Controllers, which handle directions from the ControllerProxy.
    /// </summary>
    /// <remarks>
    /// A Controller creates the ActionFactory and runs variations. If necessary,
    /// it runs a Dispatcher.
    /// 
    /// The Dispatcher thread is kept seperate from the VariationPerformer thread
    /// so nested loop actions, such as opening dialogs and drag/drop, may run.
    /// </remarks>
    [UIPermission(SecurityAction.Assert, Unrestricted = true)]
    public abstract class Controller
    {
        /// <summary>
        /// 
        /// </summary>
        protected Dispatcher TestDispatcher = null;

        /// <summary>
        /// Controls how the variation loop runs.
        /// </summary>
        /// <remarks>
        /// A Controller implements this to ensure the variation loop is invoked
        /// in the right way.</remarks>
        public abstract void RunVariationLoop();

        /// <summary>
        /// Initiates shutdown of the Controller's process or thread.
        /// </summary>
        /// <remarks>
        /// A Controller implements this to ensure its process or thread
        /// exists gracefully.
        /// </remarks>
        public abstract void EndTest();

        // Since we're running in a different process than the loader, we need
        // to log a failure if there is an unhandled exception.
        private void _OnDispatcherException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            if (TestLog.Current != null)
            {
                TestLog.Current.LogEvidence("Unhandled exception occurred in dispatcher.");
                TestLog.Current.LogEvidence(e.Exception.ToString());
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("Unhandled exception occurred in dispatcher.");
                GlobalLog.LogEvidence(e.Exception.ToString());
            }

            e.Handled = true;
            this.EndTest();
        }

        /// <summary>
        /// Starts the variation loop on a new thread.
        /// </summary>
        /// <remarks>
        /// The Dispatcher thread is kept seperate from the VariationPerformer thread
        /// so nested loop actions, such as opening dialogs and drag/drop, may run.
        /// </remarks>
        protected void StartLoop()
        {
            CommonStorage.Current.Store("Controller", this);

            //
            // Ensure we log any unhandled exceptions.
            //
            Dispatcher.CurrentDispatcher.UnhandledException +=
                new DispatcherUnhandledExceptionEventHandler(_OnDispatcherException);


            //
            string execDir = DriverState.ExecutionDirectory;//Harness.Current.RemoteSite["STI_ExecutionDirectory"];
            string currentDir = Environment.CurrentDirectory;

            //
            // If the app is a new process, the current directory will likely be
            // different than the original execution directory.  Copy files
            // from the original directory to the current one.
            //
            if (!String.Equals(currentDir, execDir))
            {
                GlobalLog.LogStatus("Current directory is different than original execution directory.");
                GlobalLog.LogStatus("Execution directory: " + execDir);
                GlobalLog.LogStatus("Current directory: " + currentDir);
                
                GlobalLog.LogStatus("Changing current directory to original execution directory...");
                Environment.CurrentDirectory = execDir;
            }

            this.TestDispatcher = Dispatcher.CurrentDispatcher;

            Thread thread = new Thread(new ThreadStart(_RunLoop));
            thread.Start();
        }

        /// <summary>
        /// Loop function that runs on its own thread.
        /// It continuously waits for new variations from the 
        /// controller proxy, and performs them here.  The loop terminates
        /// when the proxy signals that it is done (disposed).
        /// </summary>
        private void _RunLoop()
        {
            GlobalLog.LogStatus("Variation loop beginning...");

            // These are used to synchronize the main controller with this proxy.
            // For each variation, the main controller sets the xml in storage, signals 
            // the proxy to run it, and waits for it to finish.            
            EventWaitHandle controllerInitializedSignal = EventWaitHandle.OpenExisting("Controller_InitializedSignal");
            EventWaitHandle variationStartSignal = EventWaitHandle.OpenExisting("VariationStartSignal");
            EventWaitHandle variationDoneSignal = EventWaitHandle.OpenExisting("VariationDoneSignal");

            
            // Signal to the main controller that our initialization is done.
            _SetEvent(controllerInitializedSignal);

            try
            {
                //
                // Continuously wait for new variations to perform.
                // Exit when the controller proxy says we're done.
                //
                while (_WaitForSignal(variationStartSignal))
                {
                    // Run the current variation item.
                    VariationItem item = _CurrentVariation;

                    DispatcherOperation op = this.TestDispatcher.BeginInvoke(
                            DispatcherPriority.SystemIdle,
                            (DispatcherOperationCallback)delegate(object obj)
                            {
                                item.Execute();
                                return null;
                            },
                            null);

                    _WaitForOperation(op);

                    // Let the controller proxy know that the variation
                    // is has completed.
                    _SetEvent(variationDoneSignal);
                }

                this.EndTest();
            }
            catch (Exception exception)
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.LogEvidence("Unexpected exception:\r\n" + exception.ToString());
                    TestLog.Current.Result = TestResult.Fail;
                }
                else
                {
                    GlobalLog.LogStatus("Unexpected exception:\r\n" + exception.ToString());
                }
            }
            finally
            {
                GlobalLog.LogStatus("Variation loop exiting...");
                _IsVariationLoopDone = true;
            }
        }

        // Waits for event signal from the controller proxy.
        // Stops waiting if the proxy is done (disposed or error).
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        private bool _WaitForSignal(EventWaitHandle signal)
        {
            while (!signal.WaitOne(500, false))
            {
                if (this.IsControllerProxyDone)
                {
                    return false;
                }
            }

            return true;
        }

        // Waits for a DispatcherOperation to complete.
        // Stops waiting if the proxy is done (disposed or error).
        private bool _WaitForOperation(DispatcherOperation op)
        {
            while (DispatcherOperationStatus.Completed != op.Wait(TimeSpan.FromMilliseconds(500)))
            {
                if (this.IsControllerProxyDone)
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Helper method that asserts around AutoResetEvent.Set().
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        private void _SetEvent(EventWaitHandle eventHandle)
        {
            eventHandle.Set();
        }

        /// <summary>
        /// Set by the controller proxy to let us know when we're done.
        /// </summary>
        protected bool IsControllerProxyDone
        {
            get
            {
                //No known default to use in a TryParse... Better to ---- up than silently wait.
                return bool.Parse(DictionaryStore.Current["Controller_IsControllerProxyDone"]); 
            }
        }

        // True if the variation loop has ended.
        private bool _IsVariationLoopDone
        {
            set
            {                
                DictionaryStore.Current["Controller_IsVariationLoopDone"] = value.ToString();
            }
        }

        // Set by the controller proxy right before it signals us 
        // to perform another variation.
        private VariationItem _CurrentVariation
        {
            [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
            get
            {
                //
                string xaml = null;// (string)Harness.Current.RemoteSite["Controller_CurrentVariation"];

                return (VariationItem)XamlReader.Load(IOHelper.ConvertTextToStream(xaml));
            }
        }
    }
}


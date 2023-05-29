// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// System
using System;
using System.Diagnostics;
using System.IO;
using System.Security.Permissions;
using System.Threading;
using System.Xml;

// Avalon
using System.Windows;
using System.Windows.Markup;
using System.Windows.Threading;

// Test
using Microsoft.Test;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
using Microsoft.Test.CrossProcess;

using System.Security.AccessControl;

namespace Avalon.Test.CoreUI.Common
{
    /// <summary>
    /// Communicates with a Controller that runs variations on a single 
    /// Dispatcher thread of various kinds of applications.
    /// </summary>
    /// <remarks>
    /// Controller supports ApplicationType, so variations may run in different 
    /// kinds of applications.
    /// 
    /// For example, a single variation may run inside a ".Application"
    /// as well as inside a Browser window in a ".Xbap" or Window in a 
    /// non-Avalon CLR executable.
    /// </remarks>
    public class ControllerProxy : IDisposable
    {
        private ApplicationType _appType = ApplicationType.ClrExe;
        private HostType _hostType;
        private string _testSuite;
        private string _testName;
        private Thread _thread = null;
        private EventWaitHandle _variationStartSignal = null;
        private EventWaitHandle _variationDoneSignal = null;

        /// <summary>
        /// </summary>
        [UIPermission(SecurityAction.Assert, Unrestricted = true)]
        public ControllerProxy(ApplicationType applicationType, HostType hostType, string testSuite, string testName)
        {

            //
            // Initialize control variables.
            //
            _appType = applicationType;
            _hostType = hostType;
            _testSuite = testSuite;
            _testName = testName;


            // Add a rule that grants the current user the 
            // right to wait on or signal the event.  Needed for Xbaps calling NotifyStopMonitoring from ACL-stripped PH.exe
            EventWaitHandleSecurity abortSignalHandleSecurity = new EventWaitHandleSecurity();
            string user = Environment.UserDomainName + "\\" + Environment.UserName;

            bool success;
            SecurityPermission fullTrust = new SecurityPermission(PermissionState.Unrestricted);
            fullTrust.Assert();
            {
                abortSignalHandleSecurity.AddAccessRule(new EventWaitHandleAccessRule(user, EventWaitHandleRights.FullControl, AccessControlType.Allow));
                _variationStartSignal = new EventWaitHandle(false, EventResetMode.AutoReset, "Controller_VariationStartSignal", out success);
                _variationDoneSignal = new EventWaitHandle(false, EventResetMode.AutoReset, "Controller_VariationDoneSignal", out success);
            }
            SecurityPermission.RevertAssert();
            
            _IsControllerProxyDone = false;

            //
            // Start controller thread/process.
            //
            _StartController();
        }

        /// <summary>
        /// 
        /// </summary>
        public void WaitUntilDone()
        {
            // Wait for Controller thread to quit.
            while (_thread.IsAlive && !_IsControllerProxyDone)
            {
                Thread.Sleep(500);
            }
        }

        /// <summary>
        /// Signals to the controller that we're done, and waits for it to quit.
        /// </summary>
        [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
        public void Dispose()
        {
            // Let the Controller know that we're done.
            _IsControllerProxyDone = true;

            // Wait for Controller thread to quit.
            while (_thread.IsAlive && !_IsVariationLoopDone)
            {
                Thread.Sleep(500);
            }
        }

        // Starts a new thread with a controller that will
        // actually perform the variations.
        private void _StartController()
        {
            // Add a rule that grants the current user the 
            // right to wait on or signal the event.  Needed for Xbaps calling NotifyStopMonitoring from ACL-stripped PH.exe
            EventWaitHandleSecurity abortSignalHandleSecurity = new EventWaitHandleSecurity();
            string user = Environment.UserDomainName + "\\" + Environment.UserName;
            bool success = false;
            abortSignalHandleSecurity.AddAccessRule(new EventWaitHandleAccessRule(user, EventWaitHandleRights.FullControl, AccessControlType.Allow));
            EventWaitHandle controllerInitializedSignal = new EventWaitHandle(false, EventResetMode.AutoReset, "Controller_InitializedSignal", out success);

            // Start thread and wait until controller is initialized.
            _thread = new Thread(new ThreadStart(_ControllerThreadStart));
            _thread.SetApartmentState(ApartmentState.STA);
            _thread.Start();

            _WaitForSignal(controllerInitializedSignal);
        }

        // Start function for the controller thread.
        // Depending on the ApplicationType, runs the controller directly or starts its process.
        private void _ControllerThreadStart()
        {
            DictionaryStore.Current["HostType"] = _hostType.ToString();
            DictionaryStore.Current["TestSuite"] = _testSuite;
            DictionaryStore.Current["TestName"] = _testName;

            try
            {
                // Run controller directly if ApplicationType is normal clr exe.
                if (_appType == ApplicationType.ClrExe)
                {
                    ThreadController controller = new ThreadController();
                    controller.RunVariationLoop();
                }
                // Run controller directly if ApplicationType is WinForms application.
                else if (_appType == ApplicationType.WinFormsApplication)
                {
                    WinFormsController controller = new WinFormsController();
                    controller.RunVariationLoop();
                }
                // Run controller on separate process via ActivationStep if
                // if ApplicationType is an Avalon application.
                else
                {
                    ActivationStep activationStep = new ActivationStep();

                    if (_appType == ApplicationType.Xbap)
                        activationStep.FileName = "ControllerBrowserApp.xbap";
                    else
                        activationStep.FileName = "ControllerWpfApp.exe";

                    activationStep.DoStep();
                }
            }
            catch (Exception exception)
            {
                GlobalLog.LogEvidence("Unexpected exception:\r\n" + exception.ToString());
                throw new Microsoft.Test.TestSetupException("Unexpected Exception.");
            }
            finally
            {
                // Let the Controller know that we're done.
                _IsControllerProxyDone = true;
            }
        }

        // Waits for a signal from the controller, and checks if the controller's thread is 
        // alive to avoid waiting forever.
        private void _WaitForSignal(EventWaitHandle signal)
        {
            while (!signal.WaitOne(5000, false))
            {
                // If the controller thread is gone, we need to throw because
                // we can't run any future variations.
                if (!_thread.IsAlive)
                {
                    throw new TestSetupException("The controller thread ended unexpectedly.");
                }
            }
        }

        /// <summary>
        /// Performs variations via the controller on another thread or process.
        /// </summary>
        /// <remarks>
        /// Variations may be executed on separate threads or processes
        /// depending on the ApplicationType specified in &lt;Dimensions /&gt;.
        /// </remarks>
        public void Perform()
        {
            _variationStartSignal.Set();

            _WaitForSignal(_variationDoneSignal);
        }

        // True if the controller is disposed, usually as a result of all 
        // variations completed, or an error occurred.
        private bool _IsControllerProxyDone
        {
            get
            {
                return bool.Parse(DictionaryStore.Current["Controller_IsControllerProxyDone"]);
            }
            set
            {
                DictionaryStore.Current["Controller_IsControllerProxyDone"] = value.ToString();
            }
        }

        // Set by the controller to let us know when the variation loop has ended.
        private bool _IsVariationLoopDone
        {
            get
            {
                bool result;
                return bool.TryParse(DictionaryStore.Current["Controller_IsVariationLoopDone"], out result) ? result : false;
            }
        }
    }
}


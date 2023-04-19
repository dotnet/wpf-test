// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Test.Loaders;
using Microsoft.Test.Loaders.Steps;
using Microsoft.Test.Logging;
using Microsoft.Win32;
using System.IO;
using System.Threading;
using System.Windows.Automation;
using System.Diagnostics;
using MTI = Microsoft.Test.Input.Input;
using System.Windows.Input;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Test.CrossProcess;

namespace Microsoft.Test.Deployment
{
    /// <summary>
    /// "Evil Twin" of regular ActivationStep, this one is designed to handle intentional, custom, misbehaved activations
    /// </summary>
    class InvalidActivationStep : ActivationStep
    {
        public enum InvalidActivationScheme
        {
            MissingRegistryACL,
            HTTPSMismatchedCertificate,
            InvalidPathLocal
        }
        
        #region Private Members

        // Class used to upload files to the 
        FileHost _fileHost;
        // Class used to monitor creation of processes / UI
        ApplicationMonitor _appMonitor;
        // Handlers to be executed when specific UI is seen
        UIHandler[] _uiHandlers = new UIHandler[0];

        private static void DismissIE7HTTPSCertPage(AutomationElement IEWindow)
        {
            try { IEWindow.SetFocus(); }
            catch { } // Do nothing if this fails.  Fails on IE9 sporadically.
            ParameterizedThreadStart workerThread = new ParameterizedThreadStart(HandleCertPageNewThread);
            Thread thread = new Thread(workerThread);
            thread.SetApartmentState(ApartmentState.STA);
            thread.Start((object)IEWindow);
            thread.Join();
        }

        private static void HandleCertPageNewThread(object theWindow)
        {
            PropertyCondition pc = new PropertyCondition(AutomationElement.ClassNameProperty, "Internet Explorer_Server");
            PropertyCondition isInvokeAble = new PropertyCondition(AutomationElement.IsInvokePatternAvailableProperty, true);

            AutomationElement IEWindow = (AutomationElement)theWindow;
            AutomationElement IEServer = IEAutomationHelper.WaitForElementByPropertyCondition(IEWindow, pc, 20);
            AutomationElementCollection invokeables = IEServer.FindAll(TreeScope.Descendants, isInvokeAble);
            AutomationElement continueToWebPageLink = invokeables[3];
            InvokePattern toInvoke = (InvokePattern)continueToWebPageLink.GetCurrentPattern(InvokePattern.Pattern);
            toInvoke.Invoke();
        }

        private string InvalidatePath(string param, InvalidActivationScheme invalidActivationScheme)
        {
            switch (invalidActivationScheme)
            {
                // Since we use the same server for HTTPS testing, there's a certificate on this one.
                // Simply turning the non-HTTPS url into HTTPS will give the desired cert mismatch.
                case InvalidActivationScheme.HTTPSMismatchedCertificate:
                    param = param.Replace("http://", "https://");
                    break;
                default:
                    break;
            }
            return param;
        }

        #endregion

        #region Public Members
        /// <summary>
        /// Gets or sets the Scheme to when activating the application
        /// </summary>
        public InvalidActivationScheme InvalidateScheme = InvalidActivationScheme.HTTPSMismatchedCertificate;

        #endregion

        #region Step Implementation

        /// <summary>
        /// Performs the Activation step
        /// </summary>
        /// <returns>returns true if the rest of the steps should be executed, otherwise, false</returns>
        protected override bool BeginStep()
        {
            //Create ApplicationMonitor
            _appMonitor = new ApplicationMonitor();

            FileHostUriScheme hostScheme = FileHostUriScheme.Local;
            switch (this.InvalidateScheme)
            {
                case InvalidActivationScheme.HTTPSMismatchedCertificate:
                    GlobalLog.LogEvidence("Invalid Activation Step: HTTPS Certificate Mismatch (Expected to cause Watsoning)");
                    hostScheme = FileHostUriScheme.HttpIntranet;
                    // This IE UI only comes up for HTTPS cert mismatch.  The change doesnt need to be reverted because this value is the default 
                    // It just helps test stability to enforce this particular default.
                    Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings", "WarnonBadCertRecving", 1);
                    break;
                case InvalidActivationScheme.MissingRegistryACL:
                    string currentUser = Environment.UserDomainName + "\\" + Environment.UserName;
                    ApplicationDeploymentHelper.CleanClickOnceCache();

                    RegistryKey deployKey = Registry.CurrentUser.OpenSubKey(@"Software\Classes\Software\Microsoft\Windows\CurrentVersion", true);
                    try
                    {
                        deployKey.DeleteSubKeyTree("Deployment");
                    }
                    catch { } // Do nothing (In case it's already missing)

                    deployKey = deployKey.CreateSubKey("Deployment");
                    RegistrySecurity security = deployKey.GetAccessControl(AccessControlSections.All);
                    RegistrySecurity security2 = new RegistrySecurity();
                    security2.SetAccessRuleProtection(true, false);

                    foreach (AccessRule ar in security.GetAccessRules(true, true, typeof(NTAccount)))
                    {
                        if (string.Compare(ar.IdentityReference.Value, currentUser, StringComparison.OrdinalIgnoreCase) == 0 && ar.AccessControlType == AccessControlType.Allow)
                        {
                            security.RemoveAccessRule((RegistryAccessRule)ar);
                            GlobalLog.LogDebug("Removed access rule: " + ar.ToString());
                        }
                        else
                        {
                            security2.AddAccessRule((RegistryAccessRule)ar);
                        }
                    }
                    deployKey.SetAccessControl(security2);
                    deployKey.Flush();
                    deployKey.Close();

                    break;
                default:
                    break;
            }

            if (SupportFiles.Length > 0)
            {
                // Create host to copy files to...
                _fileHost = new FileHost(null, false);
                // Upload each file
                foreach (SupportFile suppFile in SupportFiles)
                {
                    // Whether to copy foo\bar\baz.xbap to the foo\bar created on the remote machine or just flattened
                    _fileHost.PreserveDirectoryStructure = suppFile.PreserveDirectoryStructure;

                    if (suppFile.IncludeDependencies)
                    {
                        _fileHost.UploadFileWithDependencies(suppFile.Name);
                    }
                    else
                    {
                        _fileHost.UploadFile(suppFile.Name, suppFile.TargetDirectory);
                    }
                }
            }

            // register UIHandlers
            foreach (UIHandler handler in UIHandlers)
            {
                if (handler.NamedRegistration != null)
                    _appMonitor.RegisterUIHandler(handler, handler.NamedRegistration, handler.Notification);
                else
                    _appMonitor.RegisterUIHandler(handler, handler.ProcessName, handler.WindowTitle, handler.Notification);
            }

            string param = FileName;

            param = _fileHost.GetUri(FileName, hostScheme).ToString();

            param = InvalidatePath(param, this.InvalidateScheme);

            // Clear the fusion cache by default.  Can be disabled for custom ClickOnce scenarios
            if (ClearFusionCache)
                ApplicationDeploymentHelper.CleanClickOnceCache();

            // Clear IE History but only if specified (defaults to false).  Only matters for history-based navigation
            if (ClearIEHistory)
                ApplicationDeploymentHelper.ClearIEHistory();

            // Launch the appropriate handler...
            switch (Method)
            {
                case ActivationMethod.Launch:
                    {
                        // This only works for local paths for security reasons.  
                        if (PresentationHostDebugMode)
                        {
                            param = Path.GetFullPath(param);
                            // Workaround ... for some reason on XP64 there's no guarantee that it will actually find PresHost
                            // Even though it verily is in the SysWOW64 directory.  Solution... find the right one before we try
                            string presHostPath = "presentationhost.exe";
                            if ((Environment.OSVersion.Version.Major == 5))
                            {
                                presHostPath = (Directory.GetFiles(Environment.GetEnvironmentVariable("SystemRoot"), "PresentationHost.exe", SearchOption.AllDirectories))[0];
                            }
                            _appMonitor.StartProcess(presHostPath, " -debug \"" + param + "\"");
                        }
                        else
                        {
                            // Launch process with specified arguments.  If shell: specified, then start that way.
                            // If the arguments are for the URL, directly concatenate them.
                            if ((Arguments.Length > 6) && (Arguments.ToLowerInvariant().StartsWith("shell:")))
                                _appMonitor.StartProcess(param, Arguments.Substring(6));
                            else
                                _appMonitor.StartProcess(param + Arguments);
                        }
                        break;
                    }
                default:
                    throw new InvalidOperationException("InvalidActivationStep currently only supports Launch scenario!");                       
            }

            switch (this.InvalidateScheme)
            {
                case InvalidActivationScheme.MissingRegistryACL:
                    // Do nothing currently... 
                    break;
                case InvalidActivationScheme.HTTPSMismatchedCertificate:
                    {
                        int expectedIEProcessCount = (ApplicationDeploymentHelper.GetIEVersion() >= 8) ? 2 : 1;
                        int countdown = 15;
                        Process [] IEProcesses = Process.GetProcessesByName("iexplore");
                        
                        while ((countdown > 0) && IEProcesses.Length == 0)
                        {
                            Thread.Sleep(1000);
                            IEProcesses = Process.GetProcessesByName("iexplore");
                        }

                        if (IEProcesses.Length > expectedIEProcessCount)
                        {
                            GlobalLog.LogStatus("Error! - Saw " + IEProcesses.Length + " IE Processes where there should have been 1.  Failing test, machine is in a bad state.");
                            TestLog.Current.Result = TestResult.Fail;
                            return false;
                        }

                        foreach (Process ieProc in IEProcesses)
                        {
                            ieProc.WaitForInputIdle();
                        }
                        Thread.Sleep(2000);

                        switch (ApplicationDeploymentHelper.GetIEVersion())
                        {
                            case 7:
                                {
                                    // Deals with LCIE issue where some IExplore processes never have window handles (no windows)
                                    IntPtr currentIEHandle= IntPtr.Zero;
                                    int index = 0;
                                    while (currentIEHandle == IntPtr.Zero && index < IEProcesses.Length)
                                    {
                                        currentIEHandle = IEProcesses[index].MainWindowHandle;
                                        index ++;
                                    }
                                    DismissIE7HTTPSCertPage(AutomationElement.FromHandle(currentIEHandle));
                                    break;
                                }
                            case 6:
                                {
                                    AndCondition isChildDialog = new AndCondition(new PropertyCondition(AutomationElement.ClassNameProperty, "#32770"),
                                        new PropertyCondition(AutomationElement.ControlTypeProperty, ControlType.Window));
                                    IEAutomationHelper.WaitForElementByAndCondition(AutomationElement.FromHandle(IEProcesses[0].MainWindowHandle), isChildDialog, 15);

                                    MTI.SendKeyboardInput(Key.Left, true);
                                    MTI.SendKeyboardInput(Key.Left, false);
                                    Thread.Sleep(100);
                                    MTI.SendKeyboardInput(Key.Enter, true);
                                    MTI.SendKeyboardInput(Key.Enter, false);

                                    break;
                                }
                            // There's a definitely trend towards every new version working like IE7...
                            default:
                                goto case 7;
                        }
                        break;
                    }
            }

            // Store the activation path into the cross process dictionary.  This way apps or child steps can directly figure out the deployment URI
            DictionaryStore.Current["ActivationStepUri"] = param;

            return true;
        }

        /// <summary>
        /// Waits for the ApplicationMonitor to Abort and Closes any remaining
        /// processes
        /// </summary>
        /// <returns>true</returns>
        protected override bool EndStep()
        {
            //Wait for the application to be done
            _appMonitor.WaitForUIHandlerAbort();
            _appMonitor.Close();

            // This dialog only comes up for HTTPS cert mismatch.  The change doesnt need to be reverted but is in the EndStep part.
            Registry.SetValue(@"HKEY_CURRENT_USER\Software\Microsoft\Windows\CurrentVersion\Internet Settings", "WarnonBadCertRecving", 1);

            // close the fileHost if one was created within ActivationStep. 
            // Don't close if the filehost is in the context of a FileHostStep
            if ((_fileHost != null) && (SupportFiles.Length > 0))
                _fileHost.Close();

            return true;
        }
        #endregion
    }
}

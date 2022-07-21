// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace WPF.Printing.Common.Helpers
{
    using System;
    using System.Diagnostics;
    using System.Management;
    using System.Threading;
    using Microsoft.Test.Logging;

    public delegate void ProcessWatcherAction();

    /// <summary>
    /// Class designed to wait for a Process (iexplore.exe, notepad.exe, etc..)
    /// that start as a result of a test process or some test actions.
    /// </summary>
    public class ProcessWatcher
    {
        /// <summary>
        /// Set the maximum amount of time to wait for process to start
        /// </summary>
        /// <param name="seconds">Wait time in seconds</param>
        public void SetTimeout(double seconds)
        {
            timeout = TimeSpan.FromSeconds(seconds);
        }

        /// <summary>
        /// Start a thread to wait for a process to start
        /// </summary>
        /// <param name="processname">Name of the process expected to start</param>
        public void CreateWaitForProcess(string processname)
        {
            resetEvent = new ManualResetEvent(false);
            targetProcessName = processname;

            ThreadPool.QueueUserWorkItem(new WaitCallback(ExecuteWaitForProcess));

            // Allow one second for wait event to get started.
            Thread.Sleep(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Wait for the Process to started and found
        /// </summary>
        /// <returns>The expected process</returns>
        public Process WaitForProcess()
        {
            resetEvent.WaitOne();
            resetEvent.Reset();
            return targetProcess;
        }

        /// <summary>
        /// Exit code of target process.
        /// </summary>
        public int ExitCode { get { return targetProcess.ExitCode; } }

        /// <summary>
        /// Start a Process and wait for it to exit.
        /// </summary>
        /// <param name="filename">Name of file to start with process</param>
        /// <param name="cmdargs">Command line arguments for the process</param>
        public static void ExecuteProcess(string filename, string cmdargs)
        {
            ProcessStartInfo processInfo = new ProcessStartInfo();
            processInfo.UseShellExecute = true;
            processInfo.FileName = filename;
            processInfo.Arguments = cmdargs;

            Process process = new Process();
            process.StartInfo = processInfo;
            process.Start();
            process.WaitForExit();
        }

        /// <summary>
        /// Thread pool callback that waits for desired process to start
        /// </summary>
        /// <param name="o">Info to be used by callback</param>
        private void ExecuteWaitForProcess(object o)
        {
            string querystring = string.Format("TargetInstance ISA 'Win32_Process' AND TargetInstance.Name = '{0}'", targetProcessName);

            WqlEventQuery query = new WqlEventQuery("__InstanceCreationEvent", TimeSpan.FromSeconds(5), querystring);

            ManagementBaseObject mbObject = null;

            ManagementEventWatcher meWatcher = new ManagementEventWatcher();
            meWatcher.Query = query;
            meWatcher.Options.Timeout = timeout;

            GlobalLog.LogStatus("Waiting for next instance of '{0}'", targetProcessName);

            try
            {
                mbObject = meWatcher.WaitForNextEvent();
            }
            catch (ManagementException me)
            {
                if (me.ErrorCode == ManagementStatus.Timedout)
                {
                    GlobalLog.LogStatus("ManagementEventWatcher timed out after {0} seconds.", timeout);
                }
                else
                {
                    GlobalLog.LogStatus(me.Message);
                }
                resetEvent.Set();
            }

            if (mbObject != null)
            {
                GlobalLog.LogStatus("Process {0} has been created, path is: {1}",
                    ((ManagementBaseObject)mbObject["TargetInstance"])["Name"],
                    ((ManagementBaseObject)mbObject["TargetInstance"])["ExecutablePath"]);

                targetProcess = Process.GetProcessById(Convert.ToInt32(((ManagementBaseObject)mbObject["TargetInstance"])["ProcessId"]));
            }

            resetEvent.Set();
        }
        
        private ManualResetEvent resetEvent = null;
        private Process targetProcess = null;
        private string targetProcessName = string.Empty;
        private TimeSpan timeout = TimeSpan.FromSeconds(15);
    }
}

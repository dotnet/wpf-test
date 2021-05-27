// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// <description>

//   Will managed the lifetime of processes, including watching for failure.

// </description>




//              processes.


//              also terminated.

using System;
using System.Diagnostics;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.ComponentModel;
using System.Threading;
using System.IO;

namespace DRT
{
    /// <summary>
    /// Will managed the lifetime of processes, including watching for failure.
    /// </summary>
    public sealed class ProcessManager : IDisposable
    {
        #region Constructors
        //----------------------------------------------------------------------
        // Constructors
        //----------------------------------------------------------------------

        /// <summary>
        /// Constructs a ProcessManager with no associated processes.
        /// </summary>
        public ProcessManager()
        {
            _watchForFailures = new Timer(
                delegate { _trackedProcesses.ForEach(WatchForFailure); },
                null,
                Interval,
                Interval);

            _job = NativeMethods.CreateJobObject(IntPtr.Zero, "ProcessManager#" + this.GetHashCode());

            if (_job == IntPtr.Zero)
            {
                TestServices.Warning(
                    "JobObject could not be created; clean up will be on a best effort basis.");
            }

            // Add a handler for the DebugOutput event to trap debug output from
            // all the processes associated with this ProcessManager
            DebugOutputPublisher.DebugOutput +=
                new EventHandler<DebugOutputEventArgs>(OnDebugOutput);
        }

        #endregion Constructors

        #region Public Methods
        //----------------------------------------------------------------------
        // Public Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Create a process and manage its' lifetime.
        /// </summary>
        /// <param name="fileName">Full filename to the executable</param>
        /// <param name="arguments">Arguments to provide to the executable.
        /// </param>
        /// <param name="assertOnFailure">If true ProcessManager will 
        /// TestServices.Assert on a non-zero exit code.</param>
        /// <returns>A new Process</returns>
        public Process Create(
            string fileName, string arguments, bool assertOnFailure)
        {
            return Create(fileName, arguments, true, assertOnFailure);
        }

        /// <summary>
        /// Create a process and manage its' lifetime.
        /// </summary>
        /// <param name="fileName">Full filename to the executable</param>
        /// <param name="arguments">Arguments to provide to the executable.
        /// </param>
        /// <param name="showWindow">False hides the process window.</param>
        /// <param name="assertOnFailure">If true ProcessManager will 
        /// TestServices.Assert on a non-zero exit code.</param>
        /// <returns>A new Process</returns>
        public Process Create(
            string fileName, string arguments, bool showWindow, bool assertOnFailure)
        {
            ProcessStartInfo info = new ProcessStartInfo(
                fileName, arguments);

            if (!showWindow)
            {
                info.WindowStyle = ProcessWindowStyle.Hidden;
            }
            
            return Create(info, assertOnFailure);
        }

        /// <summary>
        /// Create a process and manage its' lifetime.
        /// </summary>
        /// <param name="startInfo">Start-up information for the process.</param>
        /// <param name="assertOnFailure">If true ProcessManager will 
        /// TestServices.Assert on a non-zero exit code.</param>
        /// <returns>A new Process</returns>
        public Process Create(ProcessStartInfo startInfo, bool assertOnFailure)
        {
            Process p = Process.Start(startInfo);

            ProcessInfo info = new ProcessInfo(
                p, Path.GetFileName(startInfo.FileName), assertOnFailure);

            if (p != null)
            {
                AddToJob(info);

                SafeAdd(info);

                TestServices.Trace(
                    "Process: {0}[{1}] {2} created.",
                    info.Name,
                    info.Process.Id,
                    startInfo.Arguments);
            }
            else
            {
                if (assertOnFailure)
                {
                    TestServices.Assert(
                        true,
                        "Process: {0}[n/a] {1} could not be created.",
                        info.Name,
                        startInfo.Arguments);
                }
                else
                {
                    TestServices.Warning(
                        "Process: {0}[n/a] {1} could not be created.",
                        info.Name,
                        startInfo.Arguments);
                }
            }

            return p;
        }

        /// <summary>
        /// Add all existing processes that match the name provied
        /// to the list of processes being managed.
        /// </summary>
        /// <param name="name">Name of process. 
        /// (Typically the image name without extension.)</param>
        /// <returns>A list of the processes that were found</returns>
        public List<Process> Add(string name, bool assertOnFailure)
        {
            Process[] matches = Process.GetProcessesByName(name);

            foreach (Process p in matches)
            {
                ProcessInfo info = new ProcessInfo(
                    p, p.MainModule.FileName, assertOnFailure);

                AddToJob(info);

                SafeAdd(info);

                TestServices.Trace(
                    "Process: {0}[1] added to watch list.",
                    info.Name,
                    info.Process.Id);
            }

            if (matches.Length == 0)
            {
                TestServices.Trace("Process: {0} was not found.", name);
            }

            return new List<Process>(matches);
        }

        /// <summary>
        /// Close all managed processes.
        /// </summary>
        /// <param name="timeout">Amount of time to wait in milliseconds before 
        /// killing.</param>
        public void CloseAll(int timeout)
        {
            // remove anyone already closed
            _trackedProcesses.ForEach(RemoveExited);

            // ask the rest to close nicely
            _trackedProcesses.ForEach(Close);

            // wait for them to all close
            DateTime end = DateTime.Now.AddMilliseconds(timeout);
            while ((_trackedProcesses.Count != 0) && (DateTime.Now < end))
            {
                _trackedProcesses.ForEach(RemoveExited);
                System.Threading.Thread.Sleep(Interval);
            }

            // kill whoever did not close nicely
            _trackedProcesses.ForEach(Terminate);
            _trackedProcesses.Clear();
        }
        #endregion Public Methods

        #region IDisposable Members
        //----------------------------------------------------------------------
        // IDisposable Members
        //----------------------------------------------------------------------

        /// <summary>
        /// Disposes (close's) all processes.
        /// </summary>
        public void Dispose()
        {
            _watchForFailures.Dispose();

            int nMaxTries = 3;
            while (--nMaxTries > 0)
            {
                try
                {
                    CloseAll(Timeout);
                    break;
                }
                catch (InvalidOperationException)
                {
                    // Collection was modified; enumeration operation may not execute
                }
            }

            // kill the job object this will close whatever assigned processes
            // are left
            if (_job != IntPtr.Zero)
            {
                NativeMethods.TerminateJobObject(_job, 0);
                NativeMethods.CloseHandle(_job);
                _job = IntPtr.Zero;
            }

            DebugOutputPublisher.DebugOutput -=
                new EventHandler<DebugOutputEventArgs>(OnDebugOutput);
        }
        #endregion IDisposable Members

        #region Private Methods
        //----------------------------------------------------------------------
        // Private Methods
        //----------------------------------------------------------------------

        /// <summary>
        /// Adds a Process to the Job object.
        /// </summary>
        /// <param name="info"></param>
        private void AddToJob(ProcessInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException("info");
            }

            // short-circuted if statement ensures _job is valid before call
            if (_job == IntPtr.Zero ||
                !NativeMethods.AssignProcessToJobObject(_job, info.Process.Handle))
            {
                TestServices.Warning(
                    "Process: {0}[{1}] was not assigned to job; cleanup will be best effort.",
                    info.Name,
                    info.Process.Id);
            }
        }

        /// <summary>
        /// Adds a Process to the watch list.
        /// 
        /// ThreadSafe
        /// </summary>
        /// <param name="info">Process information.</param>
        private void SafeAdd(ProcessInfo info)
        {
            lock (info) // Lock: Scope=Class Order=1
            {
                lock (this) // Lock: Scope=Class Order=2
                {
                    _trackedProcesses.Add(info);
                }
            }
        }

        /// <summary>
        /// Removes a Process from the watch list.
        /// 
        /// ThreadSafe
        /// </summary>
        /// <param name="info">Process information.</param>
        private void SafeRemove(ProcessInfo info)
        {
            lock (info) // Lock: Scope=Class Order=1
            {
                lock (this) // Lock: Scope=Class Order=2
                {
                    _trackedProcesses.Remove(info);
                }
            }
        }

        /// <summary>
        /// Watches a Process for failure; a non-zero exit code.
        /// 
        /// Will TestServices.Assert on failure.
        /// 
        /// ThreadSafe
        /// </summary>
        /// <param name="info">Process information.</param>
        private void WatchForFailure(ProcessInfo info)
        {
            lock (info) // Lock: Scope=Class Order=1
            {
                // validate it is supposed to be watched
                if (!info.AssertOnFailure)
                {
                    return;
                }

                // validate that it has exited
                if (!info.Process.HasExited)
                {
                    return;
                }

                // we don't want to handle this process agian
                info.AssertOnFailure = false;
            }

            Process process = info.Process;

            // assert on failure codes for exited processes
            try
            {
                if (process.ExitCode != 0)
                {
                    TestServices.Assert(
                        true,
                        "Process: {0}[{1}] exited with failure code: {2}.",
                        info.Name,
                        process.Id,
                        process.ExitCode);
                }
                else
                {
                    TestServices.Log(
                        "Process: {0}[{1}] exited with code: {2}.",
                        info.Name,
                        process.Id,
                        process.ExitCode);
                }
            }
            catch(InvalidOperationException)
            {
                TestServices.Log(
                    "Process: {0}[{1}] exited, but the exit code could not be determined, possibly because we didn't start it.",
                    info.Name,
                    process.Id);
            }
        }

        /// <summary>
        /// Remove a process from the managed list if it has exited.
        /// </summary>
        /// <param name="info">A running Process</param>
        private void RemoveExited(ProcessInfo info)
        {
            Process process = info.Process;
            try
            {
                if (process.HasExited)
                {
                    SafeRemove(info);
                    TestServices.Trace(
                        "Process: {0}[{1}] has already closed.",
                        info.Name,
                        process.Id);
                }
            }
            catch (Win32Exception win32)
            {
                if (win32.NativeErrorCode == 0x00000005)
                {
                    SafeRemove(info);
                    TestServices.Trace(
                        "Process: {0}[{1}] removed due to Access Denied error.",
                        info.Name,
                        process.Id);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Ask a process to close.
        /// </summary>
        /// <param name="info">A running Process</param>
        private void Close(ProcessInfo info)
        {
            Process process = info.Process;
            try
            {
                process.CloseMainWindow();
            }
            catch (InvalidOperationException)
            {
                // if we did not recieve the exception because the process
                // has exited throw
                if (!process.HasExited)
                {
                    throw;
                }
                // otherwise we can safely ignore the exception
            }
        }

        /// <summary>
        /// Kill a process and remove it from the managed list.
        /// </summary>
        /// <param name="info">A running Process</param>
        private void Terminate(ProcessInfo info)
        {
            Process process = info.Process;
            try
            {
                NativeMethods.TerminateProcess(process.Handle, 0);
            }
            catch (Win32Exception win32)
            {
                if (win32.NativeErrorCode == 0x00000005)
                {
                    TestServices.Trace(
                        "Process: {0} removed due to Access Denied.",
                        info.Name);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Prints the debug output contained in the EventArgs if the associated
        /// process ID matches any of the tracked processes.
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="args">The event arguments</param>
        private void OnDebugOutput(object sender, DebugOutputEventArgs args)
        {
            foreach (ProcessInfo processInfo in _trackedProcesses)
            {
                if (processInfo.Process.Id == args.ProcessId)
                {
                    TestServices.Trace(args.Output);
                    break;
                }
            }
        }
        #endregion Private Methods

        #region Private Fields
        //----------------------------------------------------------------------
        // Private Fields
        //----------------------------------------------------------------------

        /// <summary>
        /// Interval used by _watchForFailures timer.
        /// </summary>
        /// <remarks>
        /// A value that is too small will tax the system, a value to large will
        /// result in a test waiting too long to detect failure.
        /// 
        /// It's better to favor the later as failures are not the norm.
        /// </remarks>
        private const int Interval = 200;

        /// <summary>
        /// How long we will wait before terminating a process vs closing it.
        /// </summary>
        /// <remarks>
        /// Forcing a process to terminate is a last resort, a value as
        /// large as possible with out hindering automated test runs is best.
        /// </remarks>
        private const int Timeout = 5000;

        /// <summary>
        /// The job object that represents this instance of ProcessManager.
        /// </summary>
        IntPtr _job = IntPtr.Zero;

        /// <summary>
        /// Processes we are watching.
        /// </summary>
        List<ProcessInfo> _trackedProcesses = new List<ProcessInfo>();

        /// <summary>
        /// Timer which invokes the WatchForFailure on each process.
        /// </summary>
        Timer _watchForFailures;
        #endregion Private Fields

        /// <summary>
        /// Information on a process we are managing.
        /// </summary>
        private class ProcessInfo
        {
            #region Constructors
            //------------------------------------------------------------------
            // Constructors
            //------------------------------------------------------------------

            /// <summary>
            /// Constructs the ProcessInfo for a given Process.
            /// </summary>
            /// <param name="process">An existing process.</param>
            /// <param name="name">Name of the process.</param>
            /// <param name="assert">True to TestServices.Assert on failure
            /// </param>
            public ProcessInfo(Process process, string name, bool assert)
            {
                _assertOnFailure = assert;
                _name = name;
                _process = process;
            }
            #endregion Constructors

            #region Public Properties
            //------------------------------------------------------------------
            // Public Properties
            //------------------------------------------------------------------

            /// <summary>
            /// If true a TestServices.Assert should be raised if the process
            /// exit code is non-zero.
            /// </summary>
            public bool AssertOnFailure
            {
                get 
                {
                    lock (this) // Lock: Scope=Class Order=1
                    {
                        return _assertOnFailure;
                    }
                }
                set
                {
                    lock (this) // Lock: Scope=Class Order=1
                    {
                        _assertOnFailure = value;
                    }
                }
            }

            /// <summary>
            /// The name of the process.
            /// </summary>
            public string Name
            {
                get { return _name; }
            }

            /// <summary>
            /// The process that is being managed.
            /// </summary>
            public Process Process
            {
                get { return _process; }
            }
            #endregion Public Properties

            #region Private Fields
            //------------------------------------------------------------------
            // Private Fields
            //------------------------------------------------------------------

            private bool _assertOnFailure;
            private string _name;
            private Process _process;
            #endregion Private Fields
        }

        /// <summary>
        /// Native methods.
        /// </summary>
        private static class NativeMethods
        {
            #region Internal Methods
            //------------------------------------------------------------------
            // Internal Methods
            //------------------------------------------------------------------

            /// <summary>
            /// http://msdn.microsoft.com/library/en-us/dllproc/base/assignprocesstojobobject.asp
            /// </summary>
            [DllImport("kernel32.dll")]
            internal static extern bool AssignProcessToJobObject(
                IntPtr hJob, IntPtr hProcess);

            /// <summary>
            /// http://msdn.microsoft.com/library/en-us/sysinfo/base/closehandle.asp
            /// </summary>
            [DllImport("kernel32.dll")]
            internal static extern bool CloseHandle(
                IntPtr handle);

            /// <summary>
            /// http://msdn.microsoft.com/library/en-us/dllproc/base/createjobobject.asp
            /// </summary>
            [DllImport("kernel32.dll")]
            internal static extern IntPtr CreateJobObject(
                IntPtr lpJobAttributes, string lpName);

            /// <summary>
            /// http://msdn.microsoft.com/library/en-us/dllproc/base/terminatejobobject.asp
            /// </summary>
            [DllImport("kernel32.dll")]
            internal static extern IntPtr TerminateJobObject(
                IntPtr hJob, uint exitCode);

            /// <summary>
            /// http://msdn.microsoft.com/library/en-us/dllproc/base/terminateprocess.asp
            /// </summary>
            [DllImport("kernel32.dll")]
            internal static extern IntPtr TerminateProcess(
                IntPtr hProcess, uint exitCode);

            #endregion Internal Methods
        }
    }
}

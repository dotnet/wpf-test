// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides process and module utilities to test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author$ $Change$ $Date$ $Revision$ $Source$")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Threading;

    #endregion Namespaces.

    /// <summary>
    /// This class contains helper methods for process / module enumeration
    /// </summary>
    public static class ProcessUtils
    {

        #region Public methods.

        /// <summary>
        /// This method returns true if the modeulName specified is found
        /// to be loaded in the process identified by processId, false
        /// otherwise
        /// </summary>
        /// <param name="processId">Process Id for the process to be looked into</param>
        /// <param name="moduleName">Module name to be looked for</param>
        /// <returns>true if the module is loaded, false otherwise</returns>
        public static bool CheckLoadedModule(int processId, string moduleName)
        {
            Process process;

            new SecurityPermission(PermissionState.Unrestricted).Assert();

            process = Process.GetProcessById(processId);
            foreach (ProcessModule moduleInfo in process.Modules)
            {
                if (moduleInfo.ModuleName.ToLower(CultureInfo.InvariantCulture) == moduleName.ToLower(CultureInfo.InvariantCulture))
                    return true;
            }

            return false;
        }

        /// <summary>
        /// This method is the override. It assumes that it is checking
        /// current process
        /// </summary>
        /// <param name="moduleName">Module name to be looked for</param>
        /// <returns>true if the module is loaded, false otherwise</returns>
        public static bool CheckLoadedModule(string moduleName)
        {
            new SecurityPermission(PermissionState.Unrestricted).Assert();

            return ProcessUtils.CheckLoadedModule(Process.GetCurrentProcess().Id, moduleName);
        }

        /// <summary>
        /// Runs the specified process and returns the exit code, and the output
        /// and error results.
        /// </summary>
        /// <param name="fileName">Name of file to execute.</param>
        /// <param name="arguments">Arguments for executable, possibly null.</param>
        /// <param name="timeoutMilliseconds">Milliseconds to wait for process to finish.</param>
        /// <param name="standardOutput">On return, text produced on the standard output pipe.</param>
        /// <param name="standardError">On return, text produced on the standard error pipe.</param>
        /// <returns>The exit code of the process.</returns>
        /// <remarks>
        /// The first implementation of this method used WaitHandle.WaitAll.
        /// Unfortunately, this throws a NotSupportedException when run
        /// from the main thread, so instead we wait for each handle in
        /// turn. Bleargh. Note that the method may actually wait up to
        /// timeoutMilliseconds * 3 in the current implementation (but most
        /// blocked scenarios will actually timeout in this time).
        /// </remarks>
        public static int RunProcess(string fileName, string arguments,
            double timeoutMilliseconds, out string standardOutput,
            out string standardError)
        {
            Process process;                    // Process begin run.
            ProcessStartInfo startInfo;         // Process startup configuration.
            ProcessStreamReader outputReader;   // Reader for output pipe.
            ProcessStreamReader errorReader;    // Reader for standard pipe.
            DateTime timeoutMoment;             // Moment at which we time out.
            int result;                         // Exit code of the process.

            if (fileName == null)
            {
                throw new ArgumentNullException("fileName");
            }

            new SecurityPermission(PermissionState.Unrestricted).Assert();

            // Setup process startup configuration.
            startInfo = new ProcessStartInfo(fileName);
            if (arguments != null)
            {
                startInfo.Arguments = arguments;
            }
            startInfo.UseShellExecute = false;
            startInfo.RedirectStandardOutput = true;
            startInfo.RedirectStandardError = true;

            process = Process.Start(startInfo);
            try
            {
                AttachListeners(process, out outputReader, out errorReader);
				timeoutMoment = DateTime.Now.AddMilliseconds(timeoutMilliseconds);
                if (!outputReader.WaitHandle.WaitOne(timeoutMoment - DateTime.Now, false))
                {
                    throw new Exception("Output reader timed out after " + timeoutMilliseconds +
                        "ms. : " + fileName);
                }
                if (!errorReader.WaitHandle.WaitOne(timeoutMoment - DateTime.Now, false))
                {
                    throw new Exception("Error reader timed out after " + timeoutMilliseconds +
                        "ms. : " + fileName);
                }
                if (!new ProcessWaitHandle(process).WaitOne(timeoutMoment - DateTime.Now, false))
                {
                    throw new Exception("Process timed out after " + timeoutMilliseconds +
                        "ms. : " + fileName);
                }
                standardOutput = outputReader.Output;
                standardError = errorReader.Output;
                outputReader = null;
                errorReader = null;
                result = process.ExitCode;
                process.Close();
                process = null;
                return result;
            }
            finally
            {
                CleanupProcess(process);
            }
        }

        #endregion Public methods.


        #region Private methods.

        /// <summary>
        /// Attaches listeners to the standard output and error pipes
        /// of the specified process.
        /// </summary>
        /// <param name="process">Process to attach to.</param>
        /// <param name="outputReader">On return, a new reader listening on the output pipe.</param>
        /// <param name="errorReader">On return, a new reader listening on the error pipe.</param>
        private static void AttachListeners(Process process,
            out ProcessStreamReader outputReader, out ProcessStreamReader errorReader)
        {
            if (process == null)
            {
                throw new ArgumentNullException("process");
            }

            outputReader = new ProcessStreamReader(process.StandardOutput);
            outputReader.StartListening();

            errorReader = new ProcessStreamReader(process.StandardError);
            errorReader.StartListening();
        }

        /// <summary>
        /// Cleans up after the specified (possibly null) process, by
        /// killing it if it's still running and closing resources.
        /// </summary>
        /// <param name="process">Process to clean up after.</param>
        private static void CleanupProcess(Process process)
        {
            if (process != null)
            {
                if (!process.HasExited)
                {
                    process.Kill();
                }
                process.Close();
            }
        }

        #endregion Private methods.


        #region Inner types.

        /// <summary>
        /// Reads from a process standard output or error stream.
        /// </summary>
        class ProcessStreamReader
        {
            /// <summary>
            /// Initializes a new ProcessStreamReader associated with the
            /// specified reader stream.
            /// </summary>
            /// <param name="reader">Reader to read from.</param>
            internal ProcessStreamReader(StreamReader reader)
            {
                if (reader == null)
                {
                    throw new ArgumentNullException("reader");
                }
                this._event = new AutoResetEvent(false);
                this._reader = reader;
            }

            /// <summary>
            /// Starts listening to the stream reader on a new thread.
            /// </summary>
            internal void StartListening()
            {
                Thread thread;
                thread = new Thread(ThreadCallback);
                thread.Name = "Process stream listener";
                thread.IsBackground = true; // Do not block process termination.
                thread.Start();
            }

            /// <summary>
            /// Output produced, valid only after event has been signaled.
            /// </summary>
            internal string Output
            {
                get { return _output; }
            }

            /// <summary>
            /// WaitHandle that is signaled when reading has finished.
            /// </summary>
            internal WaitHandle WaitHandle
            {
                get { return _event; }
            }

            /// <summary>
            /// Reads all content from stream reader on a separate thread.
            /// </summary>
            private void ThreadCallback()
            {
                System.Diagnostics.Debug.Assert(_reader != null);

                try
                {
                    try
                    {
                        this._output = this._reader.ReadToEnd();
                    }
                    catch (System.IO.IOException ioException)
                    {
                        this._output = "IOException when reading from process handle: " +
                            ioException.Message;
                    }
                    catch (System.Exception exception)
                    {
                        this._output = "Exception when reading from process handle: " + exception.ToString();
                    }
                }
                finally
                {
                    _event.Set();
                }
            }

            /// <summary>Output from reader.</summary>
            private string _output;
            /// <summary>Event to signal that reading has completed.</summary>
            private AutoResetEvent _event;
            /// <summary>Stream reader to get information from.</summary>
            private StreamReader _reader;
        }

        /// <summary>
        /// Encapsulates a process handle in a WaitHandle subclass.
        /// </summary>
        class ProcessWaitHandle: WaitHandle
        {
            /// <summary>
            /// Initializes a new ProcessWaitHandle instance.
            /// </summary>
            /// <param name="process">Process to encapsulate.</param>
            public ProcessWaitHandle(Process process)
            {
                if (process == null)
                {
                    throw new ArgumentNullException("process");
                }
                this.SafeWaitHandle = new Microsoft.Win32.SafeHandles.SafeWaitHandle(process.Handle, false);
            }
        }

        #endregion Inner types.
    }
}

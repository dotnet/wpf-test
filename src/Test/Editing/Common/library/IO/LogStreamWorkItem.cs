// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides types to log the contents of a stream in a schedulable work item.

namespace Test.Uis.IO
{
    #region Namespaces.

    using System;
    using System.IO;
    using System.Threading; using System.Windows.Threading;
    

    using Test.Uis.Loggers;

    #endregion Namespaces.

    /// <summary>
    /// Use this class to create a work item that logs the content
    /// of a stream through the given StreamReader.
    /// </summary>
    public class LogStreamWorkItem
    {
        #region Constructors.

        /// <summary>
        /// Creates a new Test.Uis.IO.LogStreamWork instance.
        /// </summary>
        /// <param name='streamReader'>StreamReader to log from.</param>
        public LogStreamWorkItem(StreamReader streamReader)
            : this(streamReader, String.Empty, false)
        {
        }

        /// <summary>
        /// Creates a new Test.Uis.IO.LogStreamWork instance.
        /// </summary>
        /// <param name='streamReader'>StreamReader to log from.</param>
        /// <param name='prefix'>Prefix to add to log lines.</param>
        public LogStreamWorkItem(StreamReader streamReader, string prefix)
            : this(streamReader, prefix, false)
        {
        }

        /// <summary>
        /// Creates a new Test.Uis.IO.LogStreamWork instance, optionally
        /// queueing it as a work item in the system thread pool.
        /// </summary>
        /// <param name='streamReader'>StreamReader to log from.</param>
        /// <param name='prefix'>Prefix to add to log lines.</param>
        /// <param name='autoQueue'>
        /// Whether to queue this item in the ThreadPool.
        /// </param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void RunApp(string exe) {
        ///   System.Diagnostics.ProcessStartInfo startInfo;
        ///   startInfo = new System.Diagnostics.ProcessStartInfo();
        ///   startInfo.FileName = exe;
        ///   startInfo.UseShellExecute = false;
        ///   startInfo.RedirectStandardOutput = true;
        ///
        ///   System.Diagnostics.Process process;
        ///   process = System.Diagnostics.Process.Start(startInfo);
        ///
        ///   LogStreamWorkItem item = LogStreamWorkItem(
        ///     process.StandardOutput, "StdOut Log: ", true);
        /// }</code></example>
        public LogStreamWorkItem(StreamReader streamReader, string prefix, bool autoQueue)
        {
            if (streamReader == null)
            {
                throw new ArgumentNullException("streamReader");
            }
            if (prefix == null)
            {
                prefix = String.Empty;
            }

            _streamReader = streamReader;
            _prefix = prefix;
            if (autoQueue)
            {
                ThreadPool.QueueUserWorkItem(new WaitCallback(LogCallback));
            }
        }

        #endregion Constructors.

        #region Public methods.

        /// <summary>
        /// Callback to read and log stream data.
        /// </summary>
        /// <param name='o'>Data to work on, possibly null.</param>
        /// <remarks>
        /// Clients that do not auto-queue this work item can use
        /// this method to create and queue a delegate.
        /// </remarks>
        public void LogCallback(object o)
        {
            System.Diagnostics.Debug.Assert(_streamReader != null);

            // StreamReader.ReadLine will return null to signal EOF.
            string s = _streamReader.ReadLine();
            while (s != null)
            {
                Logger.Current.Log(_prefix + s);
                s = _streamReader.ReadLine();
            }
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>
        /// Prefix to prepend to each line logged.
        /// </summary>
        public string Prefix
        {
            get { return _prefix; }
            set
            {
                _prefix = (value == null)? String.Empty : value;
            }
        }

        /// <summary>
        /// StreamReader being used to log stream contents.
        /// </summary>
        public StreamReader StreamReader
        {
            get { return _streamReader; }
        }

        #endregion Public properties.


        #region Private fields.

        private StreamReader _streamReader;

        private string _prefix;

        #endregion Private fields.
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a wrapper to interact with the CLR Profiler.

namespace Test.Uis.Loggers
{
    #region Namespaces.

    using System;
    using System.Runtime.InteropServices;

    #endregion Namespaces.

    /// <summary>
    /// Provides control over the CLR Profiler and adds the ability to
    /// log comments to it.
    /// </summary>
    public sealed class ClrProfilerLogger
    {
        #region Imported methods.

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern void LogComment(string comment);

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern bool GetAllocationLoggingActive();

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern void SetAllocationLoggingActive(bool active);

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern bool GetCallLoggingActive();

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern void SetCallLoggingActive(bool active);

        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern bool DumpHeap(uint timeOut);

        #endregion Imported methods.

        #region Constructors.

        /// <summary>Class constructor.</summary>
        static ClrProfilerLogger()
        {
            try
            {
                // Try changing the active flag. If this works, then
                // we are hooked up to profilerOBJ.dll.
                bool originalActive = GetAllocationLoggingActive();
                SetAllocationLoggingActive(!originalActive);
                s_processIsUnderProfiler =
                    originalActive != GetAllocationLoggingActive();
                SetAllocationLoggingActive(originalActive);
            }
            catch(DllNotFoundException)
            {
                s_processIsUnderProfiler = false;
            }
        }

        /// <summary>Hide the constructor.</summary>
        private ClrProfilerLogger() { }

        #endregion Constructors.

        #region Public methods.

        /// <summary>Dumps the heap in CLRProfiler.</summary>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   ClrProfilerLogger.Log("Initial heap dump...");
        ///   ClrProfilerLogger.DumpHeap();
        ///   // Do something interesting...
        ///   ClrProfilerLogger.Log("Another heap dump to look at the delta");
        ///   ClrProfilerLogger.DumpHeap();
        /// }</code></example>
        public static void DumpHeap()
        {
            if (s_processIsUnderProfiler)
            {
                if (!DumpHeap(60 * 1000))
                {
                    throw new Exception("Failure to dump heap.");
                }
            }
        }

        /// <summary>Logs the specified comment.</summary>
        /// <param name='comment'>Text to log.</param>
        /// <example>The following sample shows how to use this method.<code>...
        /// private void MyMethod() {
        ///   ClrProfilerLogger.Log("Running MyMethod...");
        ///   // Do something interesting...
        ///   ClrProfilerLogger.Log("MyMethod run.");
        /// }</code></example>
        public static void Log(string comment)
        {
            if (s_processIsUnderProfiler)
            {
                LogComment(comment);
            }
        }

        /// <summary>Logs the specified comment with formatting.</summary>
        /// <param name="format">A String containing zero or more format specifications.</param>
        /// <param name="args">An Object array containing zero or more objects to be formatted.</param>
        public static void Log(string format, params object[] args)
        {
            if (s_processIsUnderProfiler)
            {
                LogComment(String.Format(format, args));
            }
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Whether allocations are being logged.</summary>
        public static bool AllocationLoggingActive
        {
            get { return (s_processIsUnderProfiler)? GetAllocationLoggingActive() : false; }
            set { if (s_processIsUnderProfiler) SetAllocationLoggingActive(value); }
        }

        /// <summary>Whether calls are being logged.</summary>
        public static bool CallLoggingActive
        {
            get { return (s_processIsUnderProfiler)? GetCallLoggingActive() : false; }
            set { if (s_processIsUnderProfiler) SetCallLoggingActive(value); }
        }

        /// <summary>Whether the process is being run in the profiler.</summary>
        public static bool ProcessIsUnderProfiler
        {
            get { return (s_processIsUnderProfiler)? GetCallLoggingActive() : false; }
            set { if (s_processIsUnderProfiler) SetCallLoggingActive(value); }
        }

        #endregion Public properties.

        #region Private fields.

        private static bool s_processIsUnderProfiler;

        #endregion Private fields.
    }
}
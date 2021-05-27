// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Microsoft.Test.Performance
{
    public class CLRProfilerControl
    {
        [DllImport("ProfilerOBJ.dll", CharSet=CharSet.Unicode)]
        private static extern void LogComment(string comment);

        [DllImport("ProfilerOBJ.dll")]
        private static extern bool GetAllocationLoggingActive();

        [DllImport("ProfilerOBJ.dll")]
        private static extern void SetAllocationLoggingActive(bool active);

        [DllImport("ProfilerOBJ.dll")]
        private static extern bool GetCallLoggingActive();

        [DllImport("ProfilerOBJ.dll")]
        private static extern void SetCallLoggingActive(bool active);

        [DllImport("ProfilerOBJ.dll")]
        private static extern bool DumpHeap(uint timeOut);

        private static bool isProcessIsUnderProfiler;

        public static void LogWriteLine(string comment)
        {
            if (isProcessIsUnderProfiler)
            {
                LogComment(comment);
                if (comment == killProcessMarker)
                    Process.GetCurrentProcess().Kill();
            }
        }

        public static void LogWriteLine(string format, params object[] args)
        {
            if (isProcessIsUnderProfiler)
            {
                LogComment(string.Format(format, args));
            }
        }

        public static bool AllocationLoggingActive
        {
            get
            {
                if (isProcessIsUnderProfiler)
                    return GetAllocationLoggingActive();
                else
                    return false;
            }
            set
            {
                if (isProcessIsUnderProfiler)
                    SetAllocationLoggingActive(value);
            }
        }

        public static bool CallLoggingActive
        {
            get
            {
                if (isProcessIsUnderProfiler)
                    return GetCallLoggingActive();
                else
                    return false;
            }
            set
            {
                if (isProcessIsUnderProfiler)
                    SetCallLoggingActive(value);
            }
        }

        public static void DumpHeap()
        {
            if (isProcessIsUnderProfiler)
            {
                if (!DumpHeap(60*1000))
                    throw new Exception("Failure to dump heap");
            }
        }

        public static bool ProcessIsUnderProfiler
        {
            get { return isProcessIsUnderProfiler; }
        }

        static string killProcessMarker;

        static CLRProfilerControl()
        {
            try
            {
                // if AllocationLoggingActive does something, this implies profilerOBJ.dll is attached
                // and initialized properly
                bool isActive = GetAllocationLoggingActive();
                SetAllocationLoggingActive(!isActive);
                isProcessIsUnderProfiler = isActive != GetAllocationLoggingActive();
                SetAllocationLoggingActive(isActive);
                killProcessMarker = Environment.GetEnvironmentVariable("OMV_KILLPROCESS_MARKER");
            }
            catch (DllNotFoundException)
            {
                throw new Exception("profilerObj.dll was not found");
            }
        }
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Diagnostics;
using System.Runtime.InteropServices;

using System;

namespace MS.Internal
{
    ///
    public class Monitors
    {
        [Conditional("DEBUG")]
        static public void TraceTagGUI(bool wait)
        {
            DbgExDoTracePointsDialog(wait);
        }

        [Conditional("DEBUG")]
        static public void MeterGUI()
        {
            DbgExMtOpenMonitor();
        }

        [Conditional("DEBUG")]
        static public void MemMonitorGUI()
        {
            DbgExOpenMemoryMonitor();
        }

        [DllImport("PresentationDebug.dll")]
        private static extern bool DbgExDisplayStackTrace(
                string title, string filename, int line, string message, string stackTrace);

        [DllImport("PresentationDebug.dll")]
        private static extern bool DbgExDoTracePointsDialog(bool wait);

        [DllImport("PresentationDebug.dll")]
        private static extern bool DbgExMtOpenMonitor();

        [DllImport("PresentationDebug.dll")]
        private static extern bool DbgExOpenMemoryMonitor();
    }
}


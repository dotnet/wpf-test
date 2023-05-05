// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    using System;
    using System.Globalization;
    using Microsoft.Test.Logging;
    using Microsoft.Test.Xaml.Common;

    /// <summary>
    /// Common interface for logging
    /// </summary>
    public class Tracer
    {
        /// <summary>
        /// Trace out to the logs
        /// </summary>
        /// <param name="source">test source identifier</param>
        /// <param name="format">format string</param>
        /// <param name="args">arguments for string.format</param>
        public static void Trace(string source, string format, params object[] args)
        {
            Trace(source, String.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Trace out to the logs and optionally to the console
        /// </summary>
        /// <param name="source">identifier to use</param>
        /// <param name="message">message to log</param>
        public static void Trace(string source, string message)
        {
            string messageToTrace = String.Format(CultureInfo.InvariantCulture, "[{0}] {1}", source, message);
            LogTrace(messageToTrace);
        }

        /// <summary>
        /// Log a trace
        /// </summary>
        /// <param name="format">format string for string.format</param>
        /// <param name="args">arguments for the string.format</param>
        public static void LogTrace(string format, params object[] args)
        {
            LogTrace(String.Format(CultureInfo.InvariantCulture, format, args));
        }

        /// <summary>
        /// Log a trace message
        /// </summary>
        /// <param name="message">string message to trace</param>
        public static void LogTrace(string message)
        {
            if (Environment.GetEnvironmentVariable(Global.ToConsoleEnvironmentVariable) != null)
            {
                Console.WriteLine(message);
            }
            else
            {
                if (TestLog.Current != null)
                {
                    TestLog.Current.LogDebug(message);
                }
                else
                {
                    Console.WriteLine(message);
                }
            }
        }
    }
}

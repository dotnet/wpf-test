// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Microsoft.Build.Utilities;
using Microsoft.Build.Framework;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Summary description for IMSBuildLogger.
    /// </summary>
    interface IMSBuildLogger : Microsoft.Build.Framework.ILogger
    {
        /// <summary>
        /// Build Log file.
        /// </summary>
        /// <value></value>
        string BuildLogFileName
        {
            set;
        }

        /// <summary>
        /// Build Error Log file.
        /// </summary>
        /// <value></value>
        string BuildLogErrorFileName
        {
            set;
        }

        /// <summary>
        /// Build warning log file.
        /// </summary>
        /// <value></value>
        string BuildLogWarningFileName
        {
            set;
        }
    }
}

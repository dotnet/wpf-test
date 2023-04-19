using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Logging
{
    /// <summary>
    /// Compiler services analog to the MSBuild LoggerVerbosity
    /// Used to prevent the handling of MSBuild Types
    /// </summary>
    public enum LogVerbosity
    {
        Quiet,
        Minimal,
        Normal,
        Detailed,
        Diagnostic
    }
}

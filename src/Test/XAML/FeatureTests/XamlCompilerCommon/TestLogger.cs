// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace XamlCompilerCommon
{
    /// <summary>
    /// Abstract logger interface so that the same 
    /// tests can be run standalone or in different
    /// test infrastructures
    /// </summary>
    public abstract class TestLogger
    {
        public abstract void LogStatus(string status);

        public abstract void LogPass();

        public abstract void LogFail();

        public abstract void LogEvidence(string message);

        public abstract void LogFile(string fileName);
    }
}

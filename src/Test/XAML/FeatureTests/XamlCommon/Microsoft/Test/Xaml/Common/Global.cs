// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Common
{
    /// <summary>
    /// Global constants used in tests
    /// * Not sure if we need these to be global 
    /// </summary>
    public static class Global
    {
        /// <summary>
        /// Serialization schema file name
        /// </summary>
        public const string SerializationSchemaFileName = "schemas.microsoft.com.2003.10.Serialization.xsd";

        /// <summary>
        /// To Console environment variable
        /// </summary>
        public const string ToConsoleEnvironmentVariable = "ToConsole";

        /// <summary>
        /// To file environment variable
        /// </summary>
        public const string ToFileEnvironmentVariable = "ToFile";

        /// <summary>
        /// GetTestCases method's name
        /// </summary>
        public const string GetTestCasesMethodName = "GetTestCases";

        /// <summary>
        /// SyncRoot to lock on 
        /// </summary>
        private static readonly object s_syncRoot = new object();

        /// <summary>
        /// Default result file name prefix
        /// </summary>
        private const string DefaltResultFileNamePrefix = "out";

        /// <summary>
        /// Result file count
        /// </summary>
        private static int s_resultFileCount = 0;

        /// <summary>
        /// Gets a unique results file name
        /// </summary>
        public static string UniqueResultFileName
        {
            get
            {
                lock (Global.s_syncRoot)
                {
                    return Global.DefaltResultFileNamePrefix + Global.s_resultFileCount++;
                }
            }
        }
    }
}

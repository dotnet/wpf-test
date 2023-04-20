// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Diagnostics;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.NonParserMethodTests
{
    /// <summary>
    /// BamlWriter BasicTests 
    /// </summary>
    public static class BamlWriterBasicTests
    {
        /// <summary>
        /// BamlWriter Basic Test
        /// </summary>
        public static void BamlWriterBasicTest()
        {
            string bamlFileName = DriverState.DriverParameters["BamlFileName"];
            string args = bamlFileName + " r";
            ProcessStartInfo startInfo = new ProcessStartInfo("Bamldump.exe", args);
            startInfo.UseShellExecute = false;
            startInfo.ErrorDialog = false;
            startInfo.CreateNoWindow = true;
            startInfo.RedirectStandardOutput = true;

            try
            {
                Process bamlDumpProc = Process.Start(startInfo);
                if (bamlDumpProc.WaitForExit(5000) == false)
                {
                    throw new TestValidationException("BamlDump is taking longer than the allocated time");
                }

                System.IO.StreamReader streamReader = bamlDumpProc.StandardOutput;
                string processOutput = streamReader.ReadToEnd();
                streamReader.Close();
                if (processOutput.Length > 0)
                {
                    GlobalLog.LogStatus(processOutput);
                    throw new TestValidationException("------------- BamlDump Failed ------------- ");
                }
            }
            catch (Exception ex)
            {
                throw new TestValidationException(ex.Message);
            }
        }
    }
}

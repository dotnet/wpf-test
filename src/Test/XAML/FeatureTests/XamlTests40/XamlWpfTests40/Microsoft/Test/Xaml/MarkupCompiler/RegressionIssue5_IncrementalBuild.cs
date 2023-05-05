// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Resources;
using Microsoft.Test.CompilerServices;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.MarkupCompiler
{
    /// <summary>
    /// Regression test
    /// </summary>
    public class RegressionIssue5_IncrementalBuild
    {
        /// <summary>
        /// Test method
        /// </summary>
        public void RunTest()
        {
            Compiler compiler = new Compiler();
            BuildLogger logger = null;
            Dictionary<string, string> properties = null;

            // Rebuild with ErrorTask in AfterMarkupCompilePass1 target
            // This will cause MarkupCompilePass1 to run, but error and return before 
            // MarkupCompilePass2 is run
            logger = new BuildLogger();
            logger.BuildLogPath = "_Phase1BuildLog.txt";
            properties = new Dictionary<string, string>() { { "ENABLEFORCEDERROR", "true" } };
            if (compiler.CompileProject("RegressionIssue5.csproj", logger, "v4.0", properties, "Rebuild"))
            {
                GlobalLog.LogEvidence("Phase1: Rebuild passed. Expected it to fail.");
                GlobalLog.LogEvidence("See the file " + logger.BuildLogPath + " for details");
                GlobalLog.LogFile(logger.BuildLogPath);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            // Incremental build without the AfterMarkupCompilerPass1 target
            logger = new BuildLogger();
            logger.BuildLogPath = "_Phase2BuildLog.txt";
            properties = new Dictionary<string, string>() { { "ENABLEFORCEDERROR", "false" } };
            if (!compiler.CompileProject("RegressionIssue5.csproj", logger, "v4.0", properties, "Build"))
            {
                GlobalLog.LogEvidence("Phase2: Incremental build failed. Expected it to pass.");
                GlobalLog.LogEvidence("See the file " + logger.BuildLogPath + " for details");
                GlobalLog.LogFile(logger.BuildLogPath);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            string assemblyPath = compiler.BinDir + compiler.AssemblyName;
            if (!File.Exists(assemblyPath))
            {
                GlobalLog.LogEvidence("Assembly " + assemblyPath + " not found in OutputPath");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            // Check whether the BAML resource is present in the assembly
            Assembly assembly = Assembly.Load(File.ReadAllBytes(assemblyPath));
            ResourceManager resourceManager = new ResourceManager(new AssemblyName(assembly.FullName).Name + ".g", assembly);
            resourceManager.GetStream("themes/generic.baml");
        }
    }
}

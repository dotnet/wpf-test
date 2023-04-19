using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;
using Microsoft.Build.BuildEngine;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Deployment;
using Microsoft.Test.Logging;

namespace Microsoft.Test.CompilerServices
{
    /// <summary>
    /// Class to wrap the MSBuild APIs for compilation
    /// </summary>
    public class Compiler
    {
        private string binDir;
        private string assemblyName;
        private Engine msBuildEngine;
        private Project currentProj;

        public string BinDir
        {
            get { return binDir; }
        }

        public string AssemblyName
        {
            get { return assemblyName; }
        }

        /// <summary>
        /// Compiles a project without a logger
        /// The only result available is pass/fail
        /// </summary>
        /// <param name="projectFileName">Project to compile</param>
        /// <returns>true if compilation succeeded, else false</returns>
        public bool CompileProject(string projectFileName)
        {
            return CompileProject(projectFileName, null);
        }

        // <summary>
        /// Compiles a project with a logger
        /// Build errors/warnings will be present in the logger on return
        /// </summary>
        /// <param name="projectFileName">Project to compile</param>
        /// <param name="buildLogger">The BuildLogger to log to</param>
        /// <returns>true if compilation succeeded, else false</returns>
        public bool CompileProject(string projectFileName, BuildLogger buildLogger)
        {
            return CompileProject(projectFileName, buildLogger, String.Empty);
        }

        /// <summary>
        /// Compiles a project with a logger
        /// Build errors/warnings will be present in the logger on return
        /// The values for ToolsVersion and TargetFrameworkVersion are overridden based on the
        /// version of MSBuild linked to the tests as told by Engine.Version property
        /// </summary>
        /// <param name="projectFileName">Project to compile</param>
        /// <param name="buildLogger">The BuildLogger to log to</param>
        /// <param name="targetFrameworkVersion">The target framework to build for</param>
        /// <returns>true if compilation succeeded, else false</returns>
        public bool CompileProject(string projectFileName, BuildLogger buildLogger, string targetFrameworkVersion)
        {
            string selectedToolsVersion = CompilationHelper.InferToolsVersion();

            string selectedTargetFrameworkVersion = null;

            if (String.IsNullOrEmpty(targetFrameworkVersion))
            {
                // ToolsVersions 3.5 and 4.0 support TargetFrameworkVersions v3.5 and v4.0 respectively
                selectedTargetFrameworkVersion = "v" + selectedToolsVersion;
            }
            else
            {
                selectedTargetFrameworkVersion = targetFrameworkVersion;
            }

            msBuildEngine = new Engine();
            if (buildLogger != null)
            {
                msBuildEngine.RegisterLogger((BuildLoggerInternal)buildLogger.InnerObject);
            }
            msBuildEngine.DefaultToolsVersion = selectedToolsVersion;

            currentProj = new Project(msBuildEngine, selectedToolsVersion);
            currentProj.Load(projectFileName);
            SetBinData();
            SetupAssemblyLinker();
            currentProj.DefaultToolsVersion = selectedToolsVersion;
            currentProj.SetProperty("TargetFrameworkVersion", selectedTargetFrameworkVersion);

            GlobalLog.LogDebug("Project.ToolsVersion: " + currentProj.ToolsVersion);
            GlobalLog.LogDebug("Project.TargetFrameworkVersion: " + currentProj.GetEvaluatedProperty("TargetFrameworkVersion"));

            string directoryPath = Path.GetDirectoryName(projectFileName);
            if (directoryPath == String.Empty)
            {
                directoryPath = ".";
            }

            string savedProjectName = String.Format(CultureInfo.InvariantCulture, @"{0}\{1}{2}.csproj",
                directoryPath, Path.GetFileNameWithoutExtension(projectFileName), selectedTargetFrameworkVersion);
            currentProj.Save(savedProjectName, Encoding.Unicode);
            GlobalLog.LogDebug("Project file saved to " + savedProjectName);

            bool compilationResult = msBuildEngine.BuildProject(currentProj);
            msBuildEngine.UnregisterAllLoggers();

            return compilationResult;
        }

        private void SetBinData()
        {
            binDir = currentProj.GetEvaluatedProperty("OutputPath");
            assemblyName = currentProj.GetEvaluatedProperty("AssemblyName");
            string outputType = currentProj.GetEvaluatedProperty("OutputType");
            // OutputType of "exe" allows the program to write to standard output
            if (String.Compare(outputType, "winexe", true) == 0 || String.Compare(outputType, "exe", true) == 0)
            {
                assemblyName += ".exe";
            }
            else if (String.Compare(outputType, "library", true) == 0)
            {
                assemblyName += ".dll";
            }
        }

        /// <summary>
        /// Sets up the environment variables needed to use AL.exe.  This is only
        /// performed if the assembly linker deployment is installed.
        /// </summary>
        private void SetupAssemblyLinker()
        {
            string installRoot = RuntimeDeploymentHelper.GetDeploymentInstallLocation("AssemblyLinker");
            if (installRoot != null)
            {
                currentProj.GlobalProperties["AlToolPath"] = new BuildProperty("AlToolPath", installRoot);
            }
        }
    }
}

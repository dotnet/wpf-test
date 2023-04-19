using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Loader;
using System.Text;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;
using Microsoft.Build.Locator;
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
        private Project currentProj;
        private static string msBuildPath;

        /// <summary>
        /// In .NET Core we need to locate dotnet's install msbuild. MSBuildLocator achieves this
        /// but it requires to run RegisterDefaults before any Microsoft.Build assembly is loaded.
        /// </summary>
        private static void InitializeBuildLocator()
        {
            VisualStudioInstance instance = MSBuildLocator.RegisterDefaults();
            msBuildPath = instance.MSBuildPath;
        }

        /// <summary>
        /// Once we have our msbuild instance located, we need to make sure assemblies required for 
        /// compilation can be located. Sti will only look in the running directory, so we add the sdk 
        /// folder to the search path as well.
        /// </summary>
        public static Assembly OnAssemblyResolve(AssemblyLoadContext assemblyLoadContext, AssemblyName assemblyName)
        {
            try
            {
                return assemblyLoadContext.LoadFromAssemblyPath(msBuildPath + assemblyName.Name + ".dll");
            }
            catch
            {
                return null;
            }
        }

        /// <summary>
        /// Call InitializeBuildLocator before Microsoft.Build loads any assemblies.
        /// </summary>
        static Compiler()
        {
            InitializeBuildLocator();
        }

        /// <summary>
        /// Helper function to ensure MSBuildLocator runs before any calls into Microsoft.Build.
        /// </summary>
        public static void EnsureMSBuildPath()
        {
            if (String.IsNullOrEmpty(msBuildPath))
            {
                throw new Exception("Could not load MSBuild");
            }
        }

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
            return CompileProject(projectFileName, buildLogger, targetFrameworkVersion, null, String.Empty);
        }

        public bool CompileProject(string projectFileName, BuildLogger buildLogger, string targetFrameworkVersion, IDictionary<string, string> properties, string target)
        {
            bool result = false;

            AssemblyLoadContext.Default.Resolving += OnAssemblyResolve;

            try
            {
                result = CompileProjectImpl(projectFileName, buildLogger, targetFrameworkVersion, properties, target);
            }
            finally
            {
                AssemblyLoadContext.Default.Resolving -= OnAssemblyResolve;
            }
            return result;
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
        /// <param name="properties">Global properties to set</param>
        /// <param name="target">The target to build. eg. Clean, Rebuild etc. 
        ///   To use DefaultTargets in project file, pass null. 
        ///   To run specific targets, pass a list of targets separated by semi-colon.
        /// </param>
        /// <returns>true if compilation succeeded, else false</returns>
        public bool CompileProjectImpl(string projectFileName, BuildLogger buildLogger, string targetFrameworkVersion, IDictionary<string, string> properties, string target)
        {
            if (String.IsNullOrEmpty(projectFileName))
            {
                throw new TestValidationException("Project filename is null or empty");
            }

            string commandLineOptions = null;

            string selectedToolsVersion = "Current";
            commandLineOptions += " /toolsversion:" + selectedToolsVersion;
            GlobalLog.LogDebug("ToolsVersion: " + selectedToolsVersion);

            string selectedTargetFrameworkVersion = null;
            if (String.IsNullOrEmpty(targetFrameworkVersion))
            {
                selectedTargetFrameworkVersion = "netcoreapp3.0";
            }
            else
            {
                selectedTargetFrameworkVersion = targetFrameworkVersion;
            }

            Dictionary<string, string> globalProperties = new Dictionary<string, string>();

            // Set TargetFramework
            globalProperties.Add("TargetFramework", selectedTargetFrameworkVersion);
            commandLineOptions += " /p:TargetFramework=" + selectedTargetFrameworkVersion;
            GlobalLog.LogDebug("TargetFramework: " + selectedTargetFrameworkVersion);

            // Set other properties, including AlToolPath, if specified.
            if (properties != null)
            {
                foreach (KeyValuePair<string, string> item in properties)
                {
                    globalProperties.Add(item.Key, item.Value);
                    commandLineOptions += " /p:" + item.Key + "=" + item.Value;
                    GlobalLog.LogDebug(item.Key + ": " + item.Value);
                }
            }

            // Set PlatformTarget, to be the same as that of TestRuntime
            string platformTarget = CompilationHelper.InferPlatformTarget();
            globalProperties.Add("PlatformTarget", platformTarget);
            commandLineOptions += " /p:PlatformTarget=" + platformTarget;
            GlobalLog.LogDebug("PlatformTarget: " + platformTarget);

            currentProj = new Project(projectFileName, globalProperties, selectedToolsVersion);
            SetBinData();

            BuildParameters parameters = new BuildParameters();
            parameters.DefaultToolsVersion = selectedToolsVersion;
            if (buildLogger != null)
            {
                parameters.Loggers = new List<Microsoft.Build.Framework.ILogger>() { (Microsoft.Build.Framework.ILogger)buildLogger.InnerObject };
                commandLineOptions += " /v:diag";
            }

            BuildRequestData requestData = null;
            string[] targetsToBuild = null;
            if (String.IsNullOrEmpty(target))
            {
                // if no targets are specified, append Clean target to the front of default targets from csproj
                string[] targetsInProject = currentProj.Xml.DefaultTargets.Split(';');
                targetsToBuild = new string[targetsInProject.Length + 1];
                targetsToBuild[0] = "Clean";
                targetsInProject.CopyTo(targetsToBuild, 1);
            }
            else
            {
                // if targets are specified, just override the default targets from csproj
                targetsToBuild = target.Split(';');
                commandLineOptions += " /target:" + target;
                GlobalLog.LogDebug("Target: " + target);
            }

            requestData = new BuildRequestData(BuildManager.DefaultBuildManager.GetProjectInstanceForBuild(currentProj), targetsToBuild);

            BuildManager.DefaultBuildManager.BeginBuild(parameters);

            BuildSubmission submission = BuildManager.DefaultBuildManager.PendBuildRequest(requestData);
            submission.ExecuteAsync(null, null);

            if (!submission.IsCompleted)
            {
                submission.WaitHandle.WaitOne();
            }

            BuildResult buildResult = submission.BuildResult;

            BuildManager.DefaultBuildManager.EndBuild();

            ProjectCollection.GlobalProjectCollection.UnloadProject(currentProj);
            BuildManager.DefaultBuildManager.ResetCaches();

            GlobalLog.LogDebug("Repro command: msbuild.exe " + projectFileName + commandLineOptions);
            if (buildResult.OverallResult == BuildResultCode.Success)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetBinData()
        {
            binDir = currentProj.GetPropertyValue("OutputPath");
            assemblyName = currentProj.GetPropertyValue("AssemblyName");
            string outputType = currentProj.GetPropertyValue("OutputType");
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
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Test.Deployment;
using Microsoft.Test.Security.Wrappers;
using Microsoft.Build.BuildEngine;
using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;
using Microsoft.Win32;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// Executes a project file.
    /// Provides the ability to Create a new project file in memory and build it.
    /// Enables build event logging.
    /// </summary>
    public class MSBuildProjExecutor : IDisposable
    {
        #region Local Variables

        string projectFileName = null;

        string msbuildcommandlineoptions = null;

        /// <summary>
        /// ErrorWarningParser object that stores all the error+warning information in a hashtable.
        /// </summary> 
        ErrorWarningParser errorparser;

        /// <summary>
        /// Properties passed in through the commandline using MSBuildProjFileInfo struct that get
        /// converted into actual MSBuild BuildPropertyGroup objects.
        /// </summary>
        BuildPropertyGroup globalproperties;

        /// <summary>
        /// Reference to Logger object.
        /// </summary>
        MSBuildProjLogger _logger;

        /// <summary>
        /// Perf logger.
        /// </summary>
        MSBuildPerfLogger _perflogger;

        /// <summary>
        /// Error file that gets passed into the constructor.
        /// </summary>
        string errorfile;

        /// <summary>
        /// List of target specified from commandline.
        /// </summary>
        string[] targets;

        /// <summary>
        /// List of error+warnings that need to be ignored.
        /// </summary>
        string[] ignoreableerrorwarnings;

        /// <summary>
        /// MSBuild project.
        /// </summary>
        Project currentproj;

        /// <summary>
        /// MSBuild Engine
        /// </summary>
        Engine msbuildengine;

        /// <summary>
        /// MSBuild BuildPropertyGroup
        /// </summary>
        BuildPropertyGroup propertygroup;

        /// <summary>
        /// MSBuild BuildItemGroup.
        /// </summary>
        BuildItemGroup itemgroup;

        /// <summary>
        /// Generated assembly output Path.
        /// </summary>
        string generatedassemblypath = null;

        /// <summary>
        /// Generated assembly name.
        /// </summary>
        string generatedassembly = null;

        /// <summary>
        /// Target Type for the assembly generated.
        /// </summary>
        string targettype = null;

        bool targetcleanup = false;

        string stagingdirectory = null;
        string outputdirectory = null;

        bool bhostinbrowser = false;

        bool _btimestamp = false;

        #endregion Local Variables

        #region Static Methods
        /// <summary>
        /// Static Constructor to enable Redirection for MSBuild assemblies at runtime.
        /// </summary>
        static MSBuildProjExecutor()
        {
            MSBuildEngineCommonHelper.urtpath = PathSW.GetDirectoryName((typeof(object).Assembly).Location);
            MSBuildEngineCommonHelper.urtversion = (typeof(object).Assembly).ImageRuntimeVersion;

#if (!STRESS_RUNTIME)
            string temp = MSBuildEngineCommonHelper.PresentationFrameworkFullName;
            temp = null;
#endif
        }

        #endregion Static Methods

        #region Public Methods

        /// <summary>
        /// 
        /// </summary>
        public MSBuildProjExecutor()
        {
            Initialize(null);
        }

        /// <summary>
        /// Constructor that initializes the Perf logger.
        /// </summary>
        /// <param name="bdoperfmeasurement"></param>
        public MSBuildProjExecutor(bool bdoperfmeasurement)
        {
            _btimestamp = bdoperfmeasurement;
            Initialize(null);
        }

        /// <summary>
        /// Initialize and load error file information.
        /// </summary>
        /// <param name="errorfilename"></param>
        public MSBuildProjExecutor(string errorfilename)
        {
            Initialize(errorfilename);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="errorfilename"></param>
        /// <param name="bdoperfmeasurement"></param>
        public MSBuildProjExecutor(string errorfilename, bool bdoperfmeasurement)
        {
            _btimestamp = bdoperfmeasurement;
            Initialize(errorfilename);
        }

        /// <summary>
        /// Dispose all References to MSBuild objects.
        /// </summary>
        public void Dispose()
        {
            this.itemgroup = null;
            this.propertygroup = null;
            this.currentproj = null;
            this.msbuildengine = null;

            System.GC.WaitForPendingFinalizers();
            System.GC.Collect();
        }

        /// <summary>
        /// Create an Empty Project.
        /// </summary>
        public void CreateProject()
        {
            if (currentproj == null)
            {
                msbuildengine.BinPath = MSBuildEngineCommonHelper.urtpath;
                currentproj = msbuildengine.CreateNewProject();
            }
        }

        /// <summary>
        /// Create Property groups with or without conditions.
        /// </summary>
        /// <param name="condition"></param>
        public void CreateBuildPropertyGroup(string condition)
        {
            if (currentproj == null)
            {
                throw new ApplicationException("A new project file needs to be created.");
            }

            propertygroup = currentproj.AddNewPropertyGroup(false);

            if (String.IsNullOrEmpty(condition) == false)
            {
                propertygroup.Condition = condition;
            }

        }

        /// <summary>
        /// Create BuildItem groups with or without conditions
        /// </summary>
        /// <param name="condition"></param>
        public void CreateBuildItemGroup(string condition)
        {
            if (currentproj == null)
            {
                throw new ApplicationException("A new project file needs to be created.");
            }

            itemgroup = currentproj.AddNewItemGroup();

            if (String.IsNullOrEmpty(condition) == false)
            {
                itemgroup.Condition = condition;
            }
        }

        /// <summary>
        /// Add a new Property to the current project file.
        /// </summary>
        /// <param name="propertyname">Name of the poperty to Add</param>
        /// <param name="propertyvalue">Value of the property</param>
        public void AddProperty(string propertyname, string propertyvalue)
        {
            AddProperty(propertyname, propertyvalue, null);
        }

        /// <summary>
        /// Add a new Property to the current project file with a Condition.
        /// </summary>
        /// <param name="propertyname">Name of the poperty to Add</param>
        /// <param name="propertyvalue">Value of the property</param>
        /// <param name="condition">Condition to consider when setting the property</param>
        public void AddProperty(string propertyname, string propertyvalue, string condition)
        {
            if (String.IsNullOrEmpty(propertyname) || String.IsNullOrEmpty(propertyvalue))
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Failed to AddProperty as one of the input parameters was null";
                return;
            }

            BuildProperty prop = propertygroup.AddNewProperty(propertyname, propertyvalue);

            if (prop == null)
            {
                throw new Exception("Failed to create new property.");
            }

            if (String.IsNullOrEmpty(condition) == false)
            {
                prop.Condition = condition;
            }
        }

        /// <summary>
        /// Add an BuildItem to the current project file
        /// </summary>
        /// <param name="itemtype">The Type attribute value</param>
        /// <param name="includevalue">The Include attribute value</param>
        public BuildItem AddItem(string itemtype, string includevalue)
        {
            return AddItem(itemtype, includevalue, null);
        }

        /// <summary>
        /// Add an BuildItem to the current project file with an Exclude.
        /// </summary>
        /// <param name="itemtype">The Type attribute value</param>
        /// <param name="includevalue">The Include attribute value</param>
        /// <param name="exclude">The Exclude attribute value</param>
        /// <remarks>
        /// If Reference is specified, special handling is done to seperate the include value to 
        ///     either become a ReferencePath or
        ///     split into Include, Name and HintPath.
        /// </remarks>
        /// <returns>Item</returns>
        public BuildItem AddItem(string itemtype, string includevalue, string exclude)
        {
            return AddItem(itemtype, includevalue, exclude, null);
        }

        /// <summary>
        /// Add an BuildItem to the current project file with an Exclude.
        /// </summary>
        /// <param name="itemtype">The Type attribute value</param>
        /// <param name="includevalue">The Include attribute value</param>
        /// <param name="exclude">The Exclude attribute value</param>
        /// <param name="condition">The Condition attribute value</param>
        /// <remarks>
        /// If Reference is specified, special handling is done to seperate the include value to 
        ///     either become a ReferencePath or
        ///     split into Include, Name and HintPath.
        /// </remarks>
        /// <returns>Item</returns>
        public BuildItem AddItem(string itemtype, string includevalue, string exclude, string condition)
        {
            if (String.IsNullOrEmpty(itemtype) || String.IsNullOrEmpty(includevalue))
            {
                throw new ArgumentException("Failed to AddItem as one of the input parameters was null.");
            }

            BuildItem item = null;
            ProjRefFileInfo refinfo = GetIncludeFileInfo(includevalue);

            //            // Reference special processing.
            //            if (itemtype.ToLower() == "reference")
            //            {
            //                if (String.IsNullOrEmpty(refinfo.includefilename) || String.IsNullOrEmpty(refinfo.includefilepath))
            //                {
            //                    item = itemgroup.AddNewItem("ReferencePath", includevalue);
            //                }
            //                else
            //                {
            //                    item = itemgroup.AddNewItem(itemtype, refinfo.includefilename);
            //                    item.SetMetadata("Name", refinfo.includefilename);
            //                    item.SetMetadata("HintPath", refinfo.includefilepath + refinfo.includefilename + refinfo.includefileextension);
            //                }
            //            }
            //            else
            //            {
            //            }

            item = itemgroup.AddNewItem(itemtype, includevalue);

            if (item == null)
            {
                throw new Exception("Failed to create Item");
            }

            if (String.IsNullOrEmpty(exclude) == false)
            {
                item.Exclude = exclude;
            }

            if (String.IsNullOrEmpty(condition) == false)
            {
                item.Condition = condition;
            }

            return item;
        }

        /// <summary>
        /// Add a reference to a project.
        /// </summary>
        /// <param name="includevalue"></param>
        /// <param name="name"></param>
        /// <param name="hintpath"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public BuildItem AddReference(string includevalue, string name, string hintpath, string condition)
        {
            if (String.IsNullOrEmpty(includevalue))
            {
                throw new ArgumentException("Failed to AddItem as one of the input parameters was null.");
            }

            BuildItem item = itemgroup.AddNewItem("Reference", includevalue);
            if (item == null)
            {
                throw new Exception("Failed to create Item");
            }

            if (String.IsNullOrEmpty(hintpath) == false)
            {
                item.SetMetadata("HintPath", hintpath);
            }

            if (String.IsNullOrEmpty(name) == false)
            {
                item.SetMetadata("Name", name);
            }

            if (String.IsNullOrEmpty(condition) == false)
            {
                item.Condition = condition;
            }

            return item;
        }

        /// <summary>
        /// Add a reference to a project with Private flag set.
        /// </summary>
        /// <param name="includevalue"></param>
        /// <param name="name"></param>
        /// <param name="hintpath"></param>
        /// <param name="condition"></param>
        /// <param name="privatereference"></param>
        /// <returns></returns>
        public BuildItem AddReference(string includevalue, string name, string hintpath, string condition, bool privatereference)
        {
            BuildItem item = AddReference(includevalue, name, hintpath, condition);
            if (item == null)
            {
                throw new Exception("Failed to create Item");
            }

            if (privatereference == false)
            {
                item.SetMetadata("Private", "False");
            }

            return item;
        }
        /// <summary>
        /// Add a resource to a project.
        /// </summary>
        /// <param name="includevalue"></param>
        /// <param name="fileStorage"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public BuildItem AddResource(string includevalue, string fileStorage, string condition)
        {
            if (String.IsNullOrEmpty(includevalue))
            {
                throw new ArgumentException("Failed to AddItem as one of the input parameters was null.");
            }

            BuildItem item = itemgroup.AddNewItem("Resource", includevalue);
            if (item == null)
            {
                throw new Exception("Failed to create Item");
            }

            if (String.IsNullOrEmpty(fileStorage) == false)
            {
                item.SetMetadata("FileStorage", fileStorage);
            }

            if (String.IsNullOrEmpty(condition) == false)
            {
                item.Condition = condition;
            }

            return item;
        }

        /// <summary>
        /// Add a content resource to a project.
        /// </summary>
        /// <param name="includevalue"></param>
        /// <param name="copyToOutputDirectory"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public BuildItem AddContent(string includevalue, string copyToOutputDirectory, string condition)
        {
            if (String.IsNullOrEmpty(includevalue))
            {
                throw new ArgumentException("Failed to AddContent as one of the input parameters was null.");
            }

            BuildItem item = itemgroup.AddNewItem("Content", includevalue);
            if (item == null)
            {
                throw new Exception("Failed to create Item");
            }

            if (String.IsNullOrEmpty(copyToOutputDirectory) == false)
            {
                item.SetMetadata("CopyToOutputDirectory", copyToOutputDirectory);
            }

            if (String.IsNullOrEmpty(condition) == false)
            {
                item.Condition = condition;
            }

            return item;
        }

        /// <summary>
        /// Add Import to the current project file.
        /// </summary>
        /// <param name="includevalue">The target file to be included.</param>
        public void AddImport(string includevalue)
        {
            if (String.IsNullOrEmpty(includevalue))
            {
                return;
            }

            currentproj.AddNewImport(includevalue, null);
        }

        /// <summary>
        /// Build the current project file.
        /// </summary>
        /// <returns></returns>
        public bool Build()
        {
            return Build(null);
        }

        /// <summary>
        /// Initializes the MSBuild Engine, Initializes the MSBuild Project object.
        /// Passes in the properties overrides specified in commandline options to the Project object.
        /// Runs the Project and checks for BuildResult and checks the result expected by the user.
        /// </summary>
        public bool Build(string projectfilename)
        {            
            // Register MSBuild Logger.
            MSBuildEngineCommonHelper.LogDiagnostic = "Registering the MSBuildProjLogger";
            if (_btimestamp == false)
            {
                msbuildengine.RegisterLogger(_logger);

                if (_logger._listofhandlederrors != null)
                {
                    _logger._listofhandlederrors = null;
                }
            }
            else
            {
                msbuildengine.RegisterLogger(_perflogger);
            }

            // Initialize a MSBuild project.
            MSBuildEngineCommonHelper.LogDiagnostic = "Initializing a MSBuild Project class";

            // Set BinPath to the current MSBuild.exe directory.
            if (String.IsNullOrEmpty(msbuildengine.BinPath))
            {
                msbuildengine.BinPath = MSBuildEngineCommonHelper.urtpath;
            }

            if (String.IsNullOrEmpty(projectfilename) == false && currentproj == null)
            {
                currentproj = msbuildengine.CreateNewProject();
            }

            if (currentproj == null)
            {
                throw new Exception("Failed to Initialize a msbuild Project");
            }
            
            // Check if error parser is Initialized.
            // This is possible if the Property ErrorFile was set.
            if (_btimestamp == false)
            {
                if (errorparser == null)
                {
                    MSBuildEngineCommonHelper.LogDiagnostic = "No error file specified or found in Executing directory";
                }
                else
                {
                    _logger.ExpectedErrors = errorparser.ListofErrors;

                    // If a list of errorwarnings were added to be ignored for a particular run
                    // from commandline or a specific run then set ignoreable to true on these.
                    // If there is no error parser that means there is nothing to set as ignoreable.
                    for (int i = 0; i < ignoreableerrorwarnings.Length; i++)
                    {
                        // Todo: See if this is required.
                        _logger.SetErrorWarningAsIgnoreable(ignoreableerrorwarnings[i], true);
                    }
                }
            }

            if (String.IsNullOrEmpty(projectfilename) == false)
            {
                projectFileName = projectfilename;

                // Load the project file in memory
                MSBuildEngineCommonHelper.LogDiagnostic = "Loading " + projectFileName + " project file";

                try
                {
                    currentproj.Load(projectFileName);
                }
                catch (InvalidProjectFileException ipfex)
                {
                    Console.WriteLine(currentproj.IsValidated);
                    if (this.UnhandledErrorsandWarningsList.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(ipfex.Message);
                        Console.WriteLine(ipfex.StackTrace.ToString());
                        return false;
                    }
                }
                catch (InternalLoggerException ilex)
                {
                    Console.WriteLine(currentproj.IsValidated);
                    if (this.UnhandledErrorsandWarningsList.Count == 0)
                    {
                        return true;
                    }
                    else
                    {
                        Console.WriteLine(ilex.Message);
                        Console.WriteLine(ilex.StackTrace.ToString());
                        return false;
                    }
                }
            }

            if (currentproj.EvaluatedProperties["HostinBrowser"] != null)
            {
                bhostinbrowser = Convert.ToBoolean(currentproj.EvaluatedProperties["HostinBrowser"].FinalValue);
            }

            // For WOW testing setting PlatformTarget
            if (Microsoft.Test.Diagnostics.SystemInformation.Current.IsWow64Process && bhostinbrowser == false)
            {
                Console.WriteLine("Compiling for WOW by adding command line property.");
                this.AddGlobalProperty("PlatformTarget", "x86");
            }

            // for AL
            SetupAssemblyLinker();

            SetupToolsVersion();

            // If properties were set from the commandline using /p: then 
            // set GlobalProperties property on the project file.
            MSBuildEngineCommonHelper.LogDiagnostic = "Setting global properties";
            if (globalproperties != null)
            {
                msbuildengine.GlobalProperties = globalproperties;
                currentproj.GlobalProperties = globalproperties;
            }

#if (!STRESS_RUNTIME)

            // For Side-by-Side scenario the project file needs to be updated
            // to have fully qualified version numbers.

            if (Microsoft.Test.Logging.Harness.Current != null)
            {
                if (string.IsNullOrEmpty(Microsoft.Test.Logging.Harness.Current["SxSRun"]) == false)
                {
                    string versioninfo = MSBuildEngineCommonHelper.PresentationVersionInformation;
                    //string versioninfo = ", Version=3.0.51116.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL";

                    if (string.IsNullOrEmpty(versioninfo) == false)
                    {
                        BuildItemGroup builditemgroup = currentproj.GetEvaluatedItemsByName("Reference");

                        IEnumerator enumerator = builditemgroup.GetEnumerator();
                        string[] winfxassemblies = new string[] {
                                "PresentationFramework",
                                "PresentationCore",
                                "WindowsBase",
                                "UIAutomationProvider",
                                "UIAutomationTypes"
                                };

                        while (enumerator.MoveNext())
                        {
                            object temp = enumerator.Current;

                            BuildItem referenceItem = (BuildItem)temp;
                            if (temp == null)
                            {
                                continue;
                            }

                            bool referencematched = false;
                            for (int i = 0; i < winfxassemblies.Length; i++)
                            {
                                if (referenceItem.Include.ToLowerInvariant() == winfxassemblies[i].ToLowerInvariant())
                                {
                                    referencematched = true;
                                    break;
                                }
                            }

                            if (referencematched)
                            {
                                referenceItem.Include = referenceItem.Include + versioninfo;
                            }
                        }
                    }
                }
            }


#endif
            // Problem : Think when errorparser is not initialized. 
            // Solution - Currently we'll just ignore and there is not much that can be done without a list
            // of errors supplied to us.
            //MSBuildEngineCommonHelper.Log = "Buidling the current project";

            Console.WriteLine("Starting Compilation");

            bool breturnvalue = false;

            if (targets != null)
            {
                Console.Write("Targets to run - ");
                for (int i = 0; i < targets.Length; i++)
                {
                    if (i == 0)
                    {
                        Console.Write(targets[i]);
                    }
                    else
                    {
                        Console.Write("," + targets[i]);
                    }
                }
            }

            Console.WriteLine();

            string signkeyfile = null;
            if (currentproj.EvaluatedProperties["SignManifests"] != null)
            {
                bool bresult;
                Boolean.TryParse(currentproj.EvaluatedProperties["SignManifests"].FinalValue, out bresult);

                if (bresult)
                {
                    if (Convert.ToBoolean(currentproj.EvaluatedProperties["SignManifests"].FinalValue))
                    {
                        if (String.IsNullOrEmpty(this.projectFileName))
                        {
                            Console.WriteLine("Writing Key File to current directory {0}.", DirectorySW.GetCurrentDirectory());
                            signkeyfile = Microsoft.Test.Loaders.ApplicationDeploymentHelper.WriteKeyFile(DirectorySW.GetCurrentDirectory());
                        }
                        else
                        {
                            Console.WriteLine("Writing Key File to project directory {0}.", PathSW.GetDirectoryName(this.projectFileName));
                            signkeyfile = Microsoft.Test.Loaders.ApplicationDeploymentHelper.WriteKeyFile(PathSW.GetDirectoryName(this.projectFileName));
                        }
                    }
                }
            }

            //Console.WriteLine(currentproj.IsValidated);

            // Actual build occurs here, the buildresult property are very important.
            // Check and if it is false or not 
            breturnvalue = msbuildengine.BuildProject(currentproj, targets, null);

            if (String.IsNullOrEmpty(signkeyfile) == false)
            {
                if (FileSW.Exists(signkeyfile) && _cleanSignFile)
                {
                    FileSW.Delete(signkeyfile);
                }
            }

            if (_btimestamp == false)
            {
                MSBuildEngineCommonHelper.WritetoStreamandClose();

                if (breturnvalue == false)
                {
                    MSBuildEngineCommonHelper.Log = "Compilation Failed.";
                }
                else
                {
                    MSBuildEngineCommonHelper.Log = "Build done as expected";
                    string generatedassemblyproperty = null;
                    if (currentproj.EvaluatedProperties["OutputType"] != null)
                    {
                        targettype = currentproj.EvaluatedProperties["OutputType"].FinalValue;
                        if (targettype.ToLowerInvariant() == "exe" || targettype.ToLowerInvariant() == "winexe" || targettype.ToLowerInvariant() == "library")
                        {
                            if (bhostinbrowser)
                            {
                                generatedassemblyproperty = @"DeployManifest";
                            }
                            else
                            {
                                generatedassemblyproperty = @"IntermediateAssembly";
                            }
                        }

                    }

                    if (currentproj.EvaluatedProperties["IntermediateOutputPath"] != null)
                    {
                        stagingdirectory = currentproj.EvaluatedProperties["IntermediateOutputPath"].FinalValue;
                    }

                    if (currentproj.EvaluatedProperties["OutDir"] != null)
                    {
                        outputdirectory = currentproj.EvaluatedProperties["OutDir"].FinalValue;
                    }

                    if (currentproj.EvaluatedProperties["HostinBrowser"] != null)
                    {
                        bhostinbrowser = Convert.ToBoolean(currentproj.EvaluatedProperties["HostinBrowser"].FinalValue);
                    }

                    if (String.IsNullOrEmpty(generatedassemblyproperty) == false)
                    {
                        if (currentproj.EvaluatedItems != null)
                        {
                            BuildItemGroup evaluateditems = currentproj.EvaluatedItems;

                            IEnumerator enumerator = evaluateditems.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                object current = enumerator.Current;
                                if (current != null)
                                {
                                    BuildItem item = (BuildItem)current;
                                    //MSBuildEngineCommonHelper.Log = "Item name = " + item.FinalItemSpec;

                                    if (String.Compare(item.Name, generatedassemblyproperty, true, System.Globalization.CultureInfo.InvariantCulture) == 0)
                                    {
                                        generatedassembly = item.FinalItemSpec.ToString();
                                        break;
                                    }
                                }
                            }

                            //MSBuildEngineCommonHelper.Log = "Generated assembly = " + generatedassembly;

                        }
                    }
                }
            }

            msbuildengine.UnregisterAllLoggers();

            if (currentproj.EvaluatedProperties["OutputPath"] != null)
            {
                generatedassemblypath = currentproj.EvaluatedProperties["OutDir"].FinalValue;
            }

            MSBuildEngineCommonHelper.Log = "Ignored Return value " + breturnvalue;
            //MSBuildEngineCommonHelper.Log = "Executing MSBuild done";

            return breturnvalue;
        }

        /// <summary>
        /// ParseCommandline given a array of strings
        /// MSBuild commandline options are the only once considered.
        /// </summary>
        /// <param name="args"></param>
        /// <returns></returns>
        public bool ParseComandlineArgs(string[] args)
        {
            if (args == null)
            {
                Console.WriteLine("No arguments specific to MSBuild recognized.");
                return false;
            }

            if (args.Length == 0)
            {
                Console.WriteLine("No arguments specific to MSBuild recognized.");
                return false;
            }

            char[] seperator = { '/', '-' };

            for (int i = 0; i < args.Length; i++)
            {
                string arguement = args[i];

                if (String.IsNullOrEmpty(arguement))
                {
                    continue;
                }

                int index = arguement.IndexOfAny(seperator, 0);
                if (index > 0)
                {
                    arguement = arguement.Substring(index + 1);
                }
                else
                {
                    index = 0;
                    arguement = arguement.Trim();
                }

                char[] split = { ':' };
                char[] delimiter = { ',', ';' };
                string[] splitstring = new string[2];

                index = arguement.IndexOf(":", index);
                if (index < 0)
                {
                    splitstring[0] = arguement;
                }
                else
                {
                    splitstring = arguement.Split(split, 2);
                }

                switch (splitstring[0].ToLower())
                {
                    case "t":
                    case "target":
                        if (String.IsNullOrEmpty(splitstring[1]))
                        {
                            continue;
                        }

                        if (splitstring[1].ToLowerInvariant().Contains("clean"))
                        {
                            string[] targetstringarray = splitstring[1].Split(new char[] { ',', ';' });
                            for (int j = 0; j < targetstringarray.Length; j++)
                            {
                                if (targetstringarray[j].ToLowerInvariant() == "clean" || targetstringarray[j].ToLowerInvariant() == "testcleanup")
                                {
                                    targetcleanup = true;
                                }
                                else if (targetstringarray[j].ToLowerInvariant().Contains("build"))
                                {
                                    targetcleanup = false;
                                }
                            }

                            targetstringarray = null;
                        }

                        targets = splitstring[1].Split(delimiter);
                        break;

                    case "v":
                    case "verbose":
                        if (String.IsNullOrEmpty(splitstring[1]))
                        {
                            continue;
                        }

                        ILogger ilogger;
                        if (_btimestamp && _perflogger != null)
                        {
                            ilogger = (ILogger)_perflogger;
                        }
                        else
                        {
                            ilogger = (ILogger)_logger;
                        }

                        switch (splitstring[1])
                        {
                            case "q":
                            case "quiet":
                                ilogger.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Quiet;
                                break;

                            case "m":
                            case "minimal":
                                ilogger.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Minimal;
                                break;

                            case "n":
                            case "normal":
                                ilogger.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Normal;
                                break;

                            case "d":
                            case "detailed":
                                ilogger.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Detailed;
                                break;

                            case "diag":
                            case "diagnostic":
                                ilogger.Verbosity = Microsoft.Build.Framework.LoggerVerbosity.Diagnostic;
                                break;
                        }
                        break;

                    case "p":
                    case "property":
                        string[] splitpropertynamevalue;

                        //                      List<string> propertynamevalue = new List<string>();
                        char[] semicolonseperator = { ';' };
                        char[] equalsseperator = { '=' };

                        if (splitstring[1].IndexOf(';') > 0)
                        {
                            splitpropertynamevalue = splitstring[1].Split(semicolonseperator, StringSplitOptions.RemoveEmptyEntries);

                            // Most cases the split for '=' should be twice the actual set of strings.
                            //                          propertynamevalue.Capacity = splitpropertynamevalue.Length;
                            for (int j = 0; j < splitpropertynamevalue.Length; j++)
                            {
                                string[] temp = new string[2];

                                // Could cause an outof range exception.
                                temp = splitpropertynamevalue[j].Split(equalsseperator, StringSplitOptions.RemoveEmptyEntries);
                                if (temp.Length > 0 && temp.Length <= 2)
                                {
                                    //globalproperties.SetProperty(temp[0], temp[1]);
                                    AddGlobalProperty(temp[0], temp[1]);
                                }
                            }
                        }
                        else
                        {
                            splitpropertynamevalue = splitstring[1].Split(equalsseperator, StringSplitOptions.RemoveEmptyEntries);
                            if (splitpropertynamevalue.Length > 0 && splitpropertynamevalue.Length <= 2)
                            {
                                //globalproperties.SetProperty(splitpropertynamevalue[0].Trim(), splitpropertynamevalue[1].Trim());
                                if (splitpropertynamevalue.Length > 1)
                                {
                                    AddGlobalProperty(splitpropertynamevalue[0].Trim(), splitpropertynamevalue[1].Trim());
                                }
                                else
                                {
                                    AddGlobalProperty(splitpropertynamevalue[0].Trim(), null);
                                }
                            }
                        }

                        break;

                    default:
                        msbuildcommandlineoptions += splitstring[1];
                        break;
                }
            }

            return true;
        }

        /// <summary>
        /// List of Errors or Warnings listed by Number which exist in the 
        /// Error xml file that need to be ignored for the end result.
        /// Use ',' for delimiting the error and warnings numbers.
        /// </summary>
        /// <value></value>
        public void ErrorWarningsToIgnore(string listoferrorsandwarnings)
        {
            if (String.IsNullOrEmpty(listoferrorsandwarnings))
            {
                // Todo : Display explicit log info that list is null
                return;
            }

            char[] seperator = { ',' };
            ignoreableerrorwarnings = listoferrorsandwarnings.Split(seperator, StringSplitOptions.RemoveEmptyEntries);
        }

        /// <summary>
        /// Pass to ErrorWarning parser if additional errors and warnings
        /// need to be ignored other than specified in ErrorWarningCodes.xml
        /// </summary>
        /// <param name="additionaerrorfile"></param>
        public void AdditionalExpectedErrorsandWarnings(string additionaerrorfile)
        {
            if (errorparser != null)
            {
                errorparser.AdditionalErrorWarningsFile = additionaerrorfile;
            }
        }

        #endregion Public Methods

        #region Public Properties

        /// <summary>
        /// Add Global Properties to current project file.
        /// </summary>
        /// <param name="propertyname">Property Name</param>
        /// <returns>ReadOnly</returns>
        public string this[string propertyname]
        {
            set
            {
                if (String.IsNullOrEmpty(propertyname))
                {
                    return;
                }

                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                string[] args = new string[1];
                args[0] = "/p:" + propertyname + "=" + value;
                AddGlobalProperty(propertyname, value);
            }
        }

        /// <summary>
        /// String that contains all Commandline arguements to be passed to MSBuild.
        /// </summary>
        /// <value></value>
        public string CommandLineArguements
        {
            set
            {
                ParseCommandline(value);
            }
        }

        /// <summary>
        /// Commandline arguements taken using a String array.
        /// </summary>
        /// <value></value>
        [CLSCompliant(false)]
        public string[] CommandlineArguements
        {
            set
            {
                ParseComandlineArgs(value);
            }
        }


        internal static bool CleanSignFile
        {
            set
            {
                _cleanSignFile = value;
            }
        }

        private static bool _cleanSignFile = true;

        /// <summary>
        /// Save current project to a file.
        /// </summary>
        /// <value></value>
        /// <remarks>
        /// This is applicable only when a new Project is created and built.
        /// Build with an existing proj file doesn't have to save an existing file.
        /// </remarks>
        public string SaveProjectFile
        {
            set
            {
                if (String.IsNullOrEmpty(value))
                {
                    return;
                }

                if (currentproj == null)
                {
                    Console.WriteLine("Initialize current project before Saving to file");
                    return;
                }

                if (String.IsNullOrEmpty(projectFileName) == false)
                {
                    return;
                }

                currentproj.Save(value);
            }
        }

        /// <summary>
        /// Debug mode for Engine.
        /// </summary>
        /// <value></value>
        public bool DebugMode
        {
            set
            {
                if (value)
                {
                    MSBuildEngineCommonHelper.Debug = Microsoft.Test.MSBuildEngine.DebugMode.Diagnoistic;
                }
                else
                {
                    MSBuildEngineCommonHelper.Debug = Microsoft.Test.MSBuildEngine.DebugMode.Quiet;
                }
            }
        }

        /// <summary>
        /// Error File name property
        /// </summary>
        /// <value></value>
        /// <exception cref="System.Xml.XmlException"></exception>
        public string ErrorFile
        {
            // Verify if File exists and then pass the errorfile as input to the error parser
            // for the parser to parse the errorfile contents.
            set
            {
                Initialize(value);
            }
        }

        /// <summary>
        /// A XmlElement that represents a list of Expected Errors and Warnings 
        /// not specified in ErrorCodes.xml.
        /// </summary>
        public XmlElement ExpectedMSBuildErrors
        {
            set
            {
                // Do additional parsing of Error/Warning XmlNodes 
                // specified outside the ErrorCodes xml file.
                errorparser.Parse(value);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public List<ErrorWarningCode> UnhandledErrorsandWarningsList
        {
            get
            {
                if (_logger == null)
                {
                    return null;
                }

                return _logger._listofhandlederrors;
            }
        }

        ///// <summary>
        ///// List of expected errors and warnings that were explicitly specified as to be handled by
        ///// the user.
        ///// </summary>
        //public List<string> ListofExpectedErrorsandWarningsHandled
        //{
        //    get
        //    {
        //        if (logger == null)
        //        {
        //            return null;
        //        }

        //        return logger.listofhandlederrors;
        //    }
        //}

        /// <summary>
        /// Build log file name.
        /// </summary>
        /// <value></value>
        public string BuildLogFileName
        {
            set
            {
                _logger.BuildLogFileName = value;
            }
            get
            {
                return _logger.BuildLogFileName;
            }
        }

        /// <summary>
        /// Build Error log file name.
        /// </summary>
        /// <value></value>
        public string BuildLogErrorFileName
        {
            set
            {
                _logger.BuildLogErrorFileName = value;
            }
            get
            {
                return _logger.BuildLogErrorFileName;
            }
        }

        /// <summary>
        /// Build Warning Log file name.
        /// </summary>
        /// <value></value>
        public string BuildLogWarningFileName
        {
            set
            {
                _logger.BuildLogWarningFileName = value;
            }
            get
            {
                return _logger.BuildLogWarningFileName;
            }
        }

        /// <summary>
        /// Set File logging for debug info.
        /// </summary>
        /// <value></value>
        public void LHCompilerLog(string write)
        {
            MSBuildEngineCommonHelper.LogToFile(write);
        }

        /// <summary>
        /// Target cleanup option flag.
        /// </summary>
        /// <value></value>
        public bool TargetCleanup
        {
            get
            {
                return targetcleanup;
            }
            set
            {
                targetcleanup = value;
            }
        }

        /// <summary>
        /// Generated assembly Path.
        /// </summary>
        /// <value></value>
        public string GeneratedAssembly
        {
            get
            {
                if (String.IsNullOrEmpty(generatedassembly))
                {
                    return null;
                }

                return generatedassemblypath + PathSW.GetFileName(generatedassembly);
            }
        }

        /// <summary>
        /// Data structure that represents the current projects perf data.
        /// </summary>
        public List<MSBuildProjectPerf> ProjectFilesPerfList
        {
            get
            {
                if (_perflogger == null)
                {
                    return null;
                }

                return _perflogger.projectfilesperflist;
            }
        }

        /// <summary>
        /// Generated assembly Path.
        /// </summary>
        public string OutputDirectory
        {
            get
            {
                //return currentproj.EvaluatedProperties["OutDir"].FinalValue;
                return outputdirectory;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <value></value>
        public string GeneratedAssemblyType
        {
            get
            {
                return targettype;
            }
        }

        /// <summary>
        /// Flag set if the application is hosted in browser.
        /// </summary>
        public bool HostinBrowser
        {
            get
            {
                return bhostinbrowser;
            }
        }

        /// <summary>
        /// Build result property based on errors and warnings ignored.
        /// </summary>
        /// <value></value>
        public bool BuiltwithErrors
        {
            get
            {
                if (_logger == null)
                {
                    throw new Exception("Logger not initialized");
                }

                return _logger._bbuiltwitherrors;
            }
        }

        /// <summary>
        /// Build result property based on errors and warnings ignored.
        /// </summary>
        /// <value></value>
        public bool BuiltwithWarnings
        {
            get
            {
                if (_logger == null)
                {
                    throw new Exception("Logger not initialized");
                }

                return _logger._bbuiltwithwarnings;
            }
        }

        /// <summary>
        /// Flag to ignore MSBuild runtime error/warning handling.
        /// set to false if it has to be ignored.
        /// </summary>
        public bool IgnoreErrorWarningHandling
        {
            set
            {
                if (_logger == null)
                {
                    throw new Exception("Logger not initialized");
                }

                _logger._berrorhandling = value;
            }
        }
        #endregion Public Properties

        #region Private Methods

        /// <summary>
        /// Add Global properties to the current project.
        /// </summary>
        /// <param name="propertyname"></param>
        /// <param name="propertyvalue"></param>
        private void AddGlobalProperty(string propertyname, string propertyvalue)
        {
            if (String.IsNullOrEmpty(propertyname))
            {
                return;
            }

            //propertycommandline += arguement;
            if (globalproperties == null)
            {
                globalproperties = new BuildPropertyGroup();
            }

            globalproperties.SetProperty(propertyname, propertyvalue);
        }

        /// <summary>
        /// Split up a Reference include file name into,
        /// Name, Path and Extension.
        /// </summary>
        /// <param name="file">Include file name</param>        
        private ProjRefFileInfo GetIncludeFileInfo(string file)
        {
            // Method breaks out a giving filename into its fullpath, name and extension
            // and populates the global refinfo structure.

            MSBuildEngineCommonHelper.LogDiagnostic = "Getting Include File info ..";
            ProjRefFileInfo refinfo = new ProjRefFileInfo();
            if (String.IsNullOrEmpty(file) == true)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "input to GetIncludeFileInfo is null";
                return refinfo;
            }

            // First get the last '\'
            int index = file.LastIndexOf(@"\");

            if (index < 0)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Could not find '\\' in the string " + file;
                return refinfo;
            }

            MSBuildEngineCommonHelper.LogDiagnostic = "Populating the ProjRefFileInfo struct";
            refinfo.includefilepath = file.Substring(0, index + 1);

            string filewithoutpath = file.Substring(index + 1);

            // Check for wildcards in the current string.
            char[] wildcards = { '*', '?' };

            refinfo.includefilename = PathSW.GetFileNameWithoutExtension(filewithoutpath);
            if (refinfo.includefilename.IndexOfAny(wildcards) >= 0)
            {
                refinfo.includefilename = null;
                MSBuildEngineCommonHelper.LogDiagnostic = "The file " + refinfo.includefilename + " is specified with wildcards";
                return refinfo;
            }

            refinfo.includefileextension = PathSW.GetExtension(filewithoutpath);
            return refinfo;
        }

        /// <summary>
        /// Commandline parser that accepts a string.
        /// </summary>
        /// <param name="commandline"></param>
        /// <returns></returns>
        private bool ParseCommandline(string commandline)
        {
            MSBuildEngineCommonHelper.LogDiagnostic = "Executing - ParseCommandline(string commandline)";
            if (String.IsNullOrEmpty(commandline) == true)
            {
                MSBuildEngineCommonHelper.LogDiagnostic = "Input paramter was null";
                return false;
            }

            // There are many ways a split can be done.
            // 1. Split against a ; when string contains ;
            // 1. Split against a /p: for all properties - Should be done if string contains 
            // 2. Split against a blank 
            // Problem: Issue where path includes blanks
            //  Resoloved : Example "/p:WINDOWS_TST_CLIENT_ROOT="e:\Visual Studio Projects\LHCompiler\bin\debug\"
            // Solution -
            //      First trim string
            //      Then find the first instance of '/' or '-' and then the second instance of the 
            //       same set of characters.
            //      The firstindex to the second index should gives us the paramter.
            //      Store that in a string list.
            commandline.Trim();

            List<string> commandlineargs = new List<string>();
            char[] seperator = { '/', '-' };
            int firstindex, secondindex;

            firstindex = secondindex = 0;

            do
            {
                // Get first index of seperators
                firstindex = commandline.IndexOfAny(seperator);
                if (firstindex >= 0)
                {
                    // One of the seperators was found get the second one.
                    secondindex = commandline.IndexOfAny(seperator, firstindex + 1);
                    if (secondindex > firstindex)
                    {
                        // A second occurance of the seperators was found, get the commandline option and store it.
                        commandlineargs.Add(commandline.Substring(firstindex, secondindex));
                        commandline = commandline.Substring(secondindex);
                        firstindex = secondindex;
                    }
                    else
                    {
                        // A second occurance of the seperators was not found.
                        if (firstindex < commandline.Length)
                        {
                            // if the length of the actual string is greater than the first index 
                            // and second index greater than zero get the option and store else
                            // its the end of the line, just get whatever remains and store.
                            if (secondindex > 0)
                            {
                                commandlineargs.Add(commandline.Substring(firstindex, secondindex));
                                commandline = commandline.Substring(secondindex);
                                firstindex = secondindex;
                            }
                            else
                            {
                                commandlineargs.Add(commandline.Substring(firstindex));
                                firstindex = -1;
                            }
                        }
                    }
                }

            } while (firstindex > 0);

            string[] args = new string[commandlineargs.Count];
            commandlineargs.CopyTo(args);

            // Call overloaded function which takes array of strings.
            return ParseComandlineArgs(args);
        }

        /// <summary>
        /// Initialize errorparser with input error file.
        /// Also initialize the MSBuildProjLogger class
        /// </summary>
        /// <param name="errorfilename">Error XML file</param>
        private void Initialize(string errorfilename)
        {
            if (String.IsNullOrEmpty(errorfilename) == false)
            {
                // Check if error file exists.
                errorfile = MSBuildEngineCommonHelper.VerifyFileExists(errorfilename);
                if (errorfile == null)
                {
                    throw new System.IO.FileNotFoundException(errorfile + " does not exist");
                }

                // Read the Error file and initialize the list of errors and warnings.
                try
                {
                    errorparser = new ErrorWarningParser();
                    if (errorparser.Parse(errorfile) == false)
                    {
                        MSBuildEngineCommonHelper.LogError = "Error parsing error codes file";
                    }
                }
                catch (System.Xml.XmlException xex)
                {
                    throw xex;
                }
            }
            else
            {
                errorparser = new ErrorWarningParser();
            }

            // Don't want to create a logger again.
            if (_btimestamp)
            {
                if (_perflogger == null)
                {
                    _perflogger = new MSBuildPerfLogger();
                }
            }
            else
            {
                if (_logger == null)
                {
                    _logger = new MSBuildProjLogger();
                }
            }

            // Initialize the MSBuild engine
            if (msbuildengine == null)
            {
                msbuildengine = new Engine();
                if (msbuildengine == null)
                {
                    throw new Exception("Failed to initialize MSBuild engine");
                }
            }

            if (_btimestamp == false)
            {
                // When currentFileInfo.ErrorCodes.Length is not 0 have to initialize list.
                ignoreableerrorwarnings = new string[0];
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
                AddGlobalProperty("AlToolPath", installRoot);                                                    
            }
        }
        /// <summary>
        /// If ToolsVersion is present add the global property(MSBuild version 3.5)
        /// </summary>
        private void SetupToolsVersion()
        {
            if (Microsoft.Test.Logging.TestDefinition.Current != null)
            {
                if (Microsoft.Test.Logging.DriverState.DriverParameters["ToolsVersion"] != null)
                {
                    AddGlobalProperty("ToolsVersion", Microsoft.Test.Logging.DriverState.DriverParameters["ToolsVersion"]);
                }
            }
        }

        #endregion Private Methods

    }

}


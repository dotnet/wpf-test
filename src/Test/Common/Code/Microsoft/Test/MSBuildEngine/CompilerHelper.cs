// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Used for running msbuild to compile Avalon 
 *          applications at test run time.
********************************************************************/
using Microsoft.Test.Security.Wrappers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Printing;
using System.IO;
using System.Reflection;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using System.Threading;
using System.Text;


namespace Microsoft.Test.MSBuildEngine
{   


    
    /// <summary>
    /// Creates and compiles a new Avalon application project based on
    /// a various parameters.  Provides default app name and error logging.
    /// </summary>
    public sealed class CompilerHelper
    {
        #region public Constructor

        /// <summary>
        /// Creates and compiles a new Avalon application project based on
        /// a various parameters.  Provides default app name and error logging.
        /// </summary>
        public void AddDefaults()
        {
            string currentDirectory = EnvironmentSW.CurrentDirectory;
            References.Add(new Reference("CoreTestsUntrusted", Path.Combine(currentDirectory, "CoreTestsUntrusted.dll")));
            References.Add(new Reference("CoreTestsTrusted", Path.Combine(currentDirectory, "CoreTestsTrusted.dll")));
        }



        /// <summary>
        /// Constructor
        /// </summary>
        public CompilerHelper()
        {
        }

        #endregion public Constructor

        #region Define member

        string _assemblyName = "ParserTestApp";
        string _compiledPath = DirectorySW.GetCurrentDirectory() + "\\bin\\release\\ParserTestApp.exe";
        string _objPath = DirectorySW.GetCurrentDirectory() + "\\obj\\release";
        // __XamlTestRunnerTempFile.xaml is the temporary file created after we add x:Class to 
        // the original Xaml. Hence the name of the Baml.
        string _bamlPath = DirectorySW.GetCurrentDirectory() + "\\obj\\release\\__XamlTestRunnerTempFile.baml";
        string _appDefFileName = "ParserTestApp.xaml";
        bool _autoCloseWindow = false;
        bool _addClassAttribute = true;
        #endregion Define member

        #region Public members



        /// <summary>
        /// Set if you want to a x:Class attribute on root element
        /// </summary>
        public bool AddClassAttribute
        {
            get
            {
                return _addClassAttribute;
            }
            set
            {
                _addClassAttribute = value;
            }
        }

        /// <summary>
        /// The path of the compiled executable.
        /// </summary>
        public string CompiledExecutablePath
        {
            get
            {
                return _compiledPath;
            }
        }

        /// <summary>
        /// The path of the Baml file created after compilation.
        /// </summary>
        public string BamlPath
        {
            get
            {
                return _bamlPath;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool AutoCloseWindow
        {
            get
            {
                return _autoCloseWindow;
            }
            set
            {
                _autoCloseWindow = value;
            }
        }


        /// <summary>
        /// </summary>
        public List<Reference> References 
        {
            get
            {
                return _references;
            }
        }


        List<Reference> _references = new List<Reference>();
        
        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFile"></param>
        public void CompileApp(string xamlFile)
        {
            this.CompileApp(xamlFile, "Application");
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType">Container type. Could be Application</param>
        public void CompileApp(string xamlFile, string hostType)
        {
            this.CompileApp(xamlFile, hostType, null, null);
        }

        /// <summary>
        /// Generates and compiles the application.
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        public void CompileApp(string xamlFile, string hostType, string uiCulture, List<string> extraFiles)
        {
            this.CompileApp(xamlFile, hostType, uiCulture, extraFiles, null);
        }

        /// <summary>
        /// Generates and compiles the application.
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType">Host type. Could be Application or NavigationApplication</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        public void CompileApp(string xamlFile, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies)
        {
            this.CompileApp(xamlFile, hostType, uiCulture, extraFiles, supportingAssemblies, Languages.CSharp);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        public void CompileApp(string xamlFile, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language)
        {
            string[] xamlFiles = { xamlFile };
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, null);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param> 
        public void CompileApp(string xamlFile, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language, string additionalAppMarkup)
        {
            string[] xamlFiles = { xamlFile };
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, additionalAppMarkup);
        }


        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFiles">The first in this array is main page.</param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        public void CompileApp(string[] xamlFiles, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language)
        {
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, null);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFiles">The first in this array is main page.</param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param>
        public void CompileApp(string[] xamlFiles, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language, string additionalAppMarkup)
        {
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, additionalAppMarkup, null);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFiles">The first in this array is main page.</param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param>
        /// <param name="resources"></param>
        public void CompileApp(string[] xamlFiles, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language, string additionalAppMarkup, List<Resource> resources)
        {
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, additionalAppMarkup, resources, null);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFiles">The first in this array is main page.</param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param>
        /// <param name="resources"></param>
        /// <param name="contents"></param>
        public void CompileApp(string[] xamlFiles, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language, string additionalAppMarkup, List<Resource> resources, List<Content> contents)
        {
            CompileApp(xamlFiles, hostType, uiCulture, extraFiles, supportingAssemblies, language, additionalAppMarkup, resources, contents, false);
        }

        /// <summary>
        /// Generates and compiles the application. 
        /// </summary>
        /// <param name="xamlFiles">The first in this array is main page.</param>
        /// <param name="hostType">Host type. Could be Application</param>
        /// <param name="uiCulture"></param>
        /// <param name="extraFiles"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param>
        /// <param name="resources"></param>
        /// <param name="contents"></param>
        /// <param name="debugBaml"></param>
        public void CompileApp(string[] xamlFiles, string hostType, string uiCulture, List<string> extraFiles, List<string> supportingAssemblies, Languages language, string additionalAppMarkup, List<Resource> resources, List<Content> contents, bool debugBaml)
        {
            // Cleanup temp. Xaml file, if necessary.
            string _tempXamlFile = "__XamlTestRunnerTempFile.xaml";
            if (FileSW.Exists(_tempXamlFile))
            {
                FileSW.Delete(_tempXamlFile);
            }



            // Some Xaml files (e.g. those which contain events, <x:Code>, x:Name attributes etc.)
            // need to specify an x:Class attribute on the root tag. We add that here.
            string xClassName = "MySubclass";

            // Load the original Xaml file into a DOM tree, add x:Class attribute to the root element,
            // and then save the DOM tree into a temporary Xaml file
            XmlDocumentSW doc = new XmlDocumentSW();
            doc.PreserveWhitespace = true;
            doc.Load(xamlFiles[0]);


            XmlElement rootElement = doc.DocumentElement;

            if (AddClassAttribute)
            {
                rootElement.SetAttribute("Class", "http://schemas.microsoft.com/winfx/2006/xaml", xClassName);
            }

            if (rootElement.NamespaceURI.IndexOf("schemas.microsoft.com") != -1) //&& !verifierFound)
            {
                AutoCloseWindow = true;
            }

            doc.Save(_tempXamlFile);

            xamlFiles[0] = _tempXamlFile;

            GlobalLog.LogStatus("Start compilation...");

            // Generate app definition file.
            GlobalLog.LogStatus("Generate app definition file...");
            GenerateAppdef(_tempXamlFile, hostType, "winexe", language, additionalAppMarkup);


            CompilerParams compilerParams = new CompilerParams(true);

            //
            // Generate the 'proj' file.
            //
            GlobalLog.LogStatus("Generate project file...");
            foreach (string xamlFile in xamlFiles)
            {
                compilerParams.XamlPages.Add(xamlFile);
            }

            compilerParams.OutputType = "winexe";
            compilerParams.ApplicationDefinition = _appDefFileName;
            compilerParams.AssemblyName = _assemblyName;
            compilerParams.RootNamespace = "Avalon.Test.CoreUI.Parser.MyName";
            compilerParams.Language = language;

            // Put debugging info (line,position) in Baml.
            if (debugBaml)
            {
                compilerParams.XamlDebuggingInformation = true;
            }

            if (uiCulture != null)
                compilerParams.UICulture = uiCulture;

            // Add extraFiles.  They should be either xaml files or code files.
            for (int i = 0; extraFiles != null && i < extraFiles.Count; i++)
            {
                string fileName = extraFiles[i];

                if (PathSW.GetExtension(fileName) == "xaml")
                    compilerParams.XamlPages.Add(fileName);
                else
                    compilerParams.CompileFiles.Add(fileName);
            }

            // Add code-behind files if they exist and they weren't in the extraFiles.
            // Always assume they should be added.
            for (int i = 0; i < compilerParams.XamlPages.Count; i++)
            {
                string fileName = compilerParams.XamlPages[i] + ".cs";

                if (FileSW.Exists(fileName) && !compilerParams.CompileFiles.Contains(fileName))
                {
                    compilerParams.CompileFiles.Add(fileName);
                }
            }

            // Add supporting assemblies, if any, as references
            if (null != supportingAssemblies)
            {
                string currentDirectory = EnvironmentSW.CurrentDirectory;
                for (int i = 0; i < supportingAssemblies.Count; i++)
                {
                    string assemblyName = supportingAssemblies[i];
                    compilerParams.References.Add(new Reference(assemblyName, currentDirectory + "\\" + assemblyName + ".dll"));
                }
            }


            if (References.Count > 0)
            {
               compilerParams.References.AddRange(References);
            }

            // Add Resources, if any
            if (null != resources)
            {
                foreach (Resource r in resources)
                {
                    compilerParams.Resources.Add(r);
                }
            }

            // Add Contents, if any
            if (null != contents)
            {
                foreach (Content c in contents)
                {
                    compilerParams.Contents.Add(c);                    
                }
            }

            // Also add any loose files specified by TestCaseInfo.SupportFiles
            // 




            //TestDefinition td = TestDefinition.Current;
            //td.
            //if (currentTestCaseInfo != null)
            //{
            //    string[] currentSupportFiles = currentTestCaseInfo.SupportFiles;
            //    if (currentSupportFiles != null)
            //    {
            //        foreach (string fileName in currentSupportFiles)
            //        {
            //            compilerParams.Contents.Add(new Content(fileName, "Always"));
            //        }
            //    }
            //}

            //
            // Compile project.
            //
            GlobalLog.LogStatus("Compiling project...");
            Compiler compiler = new Compiler(compilerParams);
            List<ErrorWarningCode> buildErrorsAndWarnings = compiler.Compile(true);

            //
            // Check if Compiling project is successful by verifying that compiled app exists.
            //
            GlobalLog.LogStatus("Check if compiling project was successful...");
            GlobalLog.LogStatus("Check if the BAML and the compiled app both exist.");
            string bamlPath = _objPath + PathSW.DirectorySeparatorChar + PathSW.ChangeExtension(xamlFiles[0], "baml");
            bool compilerFailure = false;
            GlobalLog.LogStatus("Baml Path: " + bamlPath);
            if (!FileSW.Exists(bamlPath))
            {
                GlobalLog.LogEvidence("Baml file did not exist");
                compilerFailure = true;
            }
            GlobalLog.LogStatus("Compiled path: " + _compiledPath);
            if (!FileSW.Exists(_compiledPath))
            {
                GlobalLog.LogEvidence("Compiled path did not exist");
                compilerFailure = true;
            }
           
            if (compilerFailure)
            {
                //Save files to be used for debugging
                GlobalLog.LogStatus("Saving files used in compilation...");
                GlobalLog.LogFile("__CompilerServicesSave.proj");
                foreach (string file in compilerParams.XamlPages)
                {
                    GlobalLog.LogFile(file);
                }
                foreach (string file in compilerParams.CompileFiles)
                {
                    GlobalLog.LogFile(file);
                }
                // Get compilation error message
                string compileErrors = "Errors: \n";
                string compileWarnings = "Warnings \n";
                if ((null != buildErrorsAndWarnings) && (buildErrorsAndWarnings.Count > 0))
                {
                    // The list contains both errors and warnings. 
                    // Get the first error and get its description.
                    foreach (ErrorWarningCode errAndWarn in buildErrorsAndWarnings)
                    {
                        if (errAndWarn.Type == ErrorType.Error)
                        {
                            compileErrors += errAndWarn.Description + "\n";
                            GlobalLog.LogStatus("\nBuild Error Found - " + errAndWarn.Description + "\n");
                            break;
                        }
                        else
                        {
                            compileWarnings += errAndWarn.Description + "\n";
                            GlobalLog.LogStatus("\nWarning - " + errAndWarn.Description + "\n");
                        }
                    }
                }

                TestSetupException setupException = new TestSetupException("Compilation failed: " + compileErrors + compileWarnings);
                // Add the list of build errors and warnings as custom Exception data.
                // This can be used by callers to retrive the errors and warnings list.
                setupException.Data.Add("buildErrorsAndWarnings", buildErrorsAndWarnings);
                throw setupException;
            }
        }


        /// <summary>
        /// Extracts error data from an ErrorWarningCode object and
        /// puts it into a Hashtable as a set of key-value pairs.
        /// ErrorWarningCode object represents an error or warning from MSBuild.
        /// </summary>
        /// <param name="e"></param>
        /// <returns>Hashtable containing the error data as a set of key-value pairs</returns>
        public static Hashtable ExtractErrData(ErrorWarningCode e)
        {
            Hashtable errData = new Hashtable();
            errData["ErrorMessage"] = e.Description;
            errData["Line"] = e.LineNumber.ToString();
            errData["Position"] = e.ColumnNumber.ToString();
            return errData;
        }

        /// <summary>
        /// Runs the compiled application as a separate process.
        /// </summary>
        public void RunCompiledApp()
        {
            String execName = _assemblyName + ".exe";
            String execPath = DirectorySW.GetCurrentDirectory() + "\\bin\\Release\\" + execName;

            GlobalLog.LogStatus("Running Avalon app...");

            TimeSpan timeOutSpan = new TimeSpan(0, 0, 180);
            ProcessSW p = new ProcessSW();
            ProcessStartInfoSW info = new ProcessStartInfoSW();
            info.FileName = execPath;
            p.StartInfo = info;

            p.Start();

            bool exited = p.WaitForExit((int)timeOutSpan.TotalMilliseconds);

            if (!exited)
            {
                while (!p.HasExited)
                {
                    p.Kill();
                }
            }
                        

            // If an exception happened within the Avalon app,
            // forward that exception now.
            Exception testResultEx = SerializationHelper.RetrieveException();

            if (testResultEx != null)
            {
                SerializationHelper.StoreException(null);
                throw new TestValidationException("Exception was logged while running compiled app.\r\n\r\n" + testResultEx, testResultEx);
            }

            GlobalLog.LogStatus("Avalon app compiled and ran without error.");
        }

        /// <summary>
        /// Generate the application definition file.
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType"></param>
        /// <param name="targetType"></param>
        /// <param name="language"></param>
        private void GenerateAppdef(string xamlFile, string hostType, string targetType, Languages language)
        {
            GenerateAppdef(xamlFile, hostType, targetType, language, null);
        }

        /// <summary>
        /// Generate the application definition file.
        /// </summary>
        /// <param name="xamlFile"></param>
        /// <param name="hostType"></param>
        /// <param name="targetType"></param>
        /// <param name="language"></param>
        /// <param name="additionalAppMarkup"></param>
        private void GenerateAppdef(string xamlFile, string hostType, string targetType, Languages language, string additionalAppMarkup)
        {
            TextWriterSW appdefFile = new StreamWriterSW(_appDefFileName);

            const string sysXmlns = "clr-namespace:System;assembly=mscorlib";
            switch (hostType)
            {
                case "Application":
                    if (targetType != "Container")
                        if (!String.IsNullOrEmpty(additionalAppMarkup))
                        {
                            appdefFile.WriteLine("<Application x:Class=\"Application__\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" StartupUri=\"" + PathSW.GetFileName(xamlFile) + "\" DispatcherUnhandledException=\"HandleException\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:sys=\"" + sysXmlns + "\">");
                        }
                        else
                        {
                            appdefFile.WriteLine("<Application x:Class=\"Application__\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" StartupUri=\"" + PathSW.GetFileName(xamlFile) + "\" DispatcherUnhandledException=\"HandleException\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
                        }
                    else
                        if (!String.IsNullOrEmpty(additionalAppMarkup))
                        {
                            appdefFile.WriteLine("<Application x:Class=\"Application__\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" DispatcherUnhandledException=\"HandleException\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" xmlns:sys=\"" + sysXmlns + "\">");
                        }
                        else
                        {
                            appdefFile.WriteLine("<Application x:Class=\"Application__\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" DispatcherUnhandledException=\"HandleException\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\">");
                        }
                    break;

                default:
                    throw new TestSetupException("Parameter hostType has invalid value of " + hostType
                        + ". The only valid value is Application");
            }

            if (!String.IsNullOrEmpty(additionalAppMarkup))
            {
                appdefFile.WriteLine(additionalAppMarkup);
            }


            appdefFile.WriteLine("<x:Code>");
            appdefFile.WriteLine("    <![CDATA[");

            if (language == Languages.CSharp)
            {

                if (AutoCloseWindow)
                {

                    appdefFile.WriteLine("    protected override void OnStartup(System.Windows.StartupEventArgs e)");
                    appdefFile.WriteLine("    {");
                    appdefFile.WriteLine("        Microsoft.Test.Logging.GlobalLog.LogStatus(\"In TestParserApp.OnStartup()...\");");
                    appdefFile.WriteLine("        Microsoft.Test.Logging.GlobalLog.LogStatus(\"Current directory: \" + Microsoft.Test.Security.Wrappers.EnvironmentSW.CurrentDirectory);");

                    appdefFile.WriteLine("        System.Windows.Threading.DispatcherTimer timer = new System.Windows.Threading.DispatcherTimer();");
                    appdefFile.WriteLine("        timer.Interval = TimeSpan.FromSeconds(10);");
                    appdefFile.WriteLine("        timer.Tick += delegate (object o, EventArgs args) {((System.Windows.Threading.DispatcherTimer)o).Stop(); System.Windows.Application.Current.Shutdown();};");
                    appdefFile.WriteLine("        timer.Start();");
                    appdefFile.WriteLine("    }");

                }

                appdefFile.WriteLine("    public void HandleException(object sender, System.Windows.Threading.DispatcherUnhandledExceptionEventArgs e)");
                appdefFile.WriteLine("    {");
                appdefFile.WriteLine("        if(null != e.Exception)");
                appdefFile.WriteLine("        {");
                appdefFile.WriteLine("            e.Handled = true;");
                appdefFile.WriteLine("            Microsoft.Test.Serialization.SerializationHelper.StoreException(e.Exception);");
                appdefFile.WriteLine("            System.Windows.Application.Current.Shutdown();");
                appdefFile.WriteLine("        }");
                appdefFile.WriteLine("    }");
            }
            else // Languages.VisualBasic
            {

                if (AutoCloseWindow)
                {

                    appdefFile.WriteLine("    Protected Overrides Sub OnStartup(ByVal e As System.Windows.StartupEventArgs)");

                    appdefFile.WriteLine("        Dim timer As New System.Windows.Threading.DispatcherTimer()");
                    appdefFile.WriteLine("        timer.Interval = TimeSpan.FromSeconds(5)");
                    appdefFile.WriteLine("        AddHandler timer.Tick, AddressOf TimerHandler");
                    appdefFile.WriteLine("        timer.Start()");


                    appdefFile.WriteLine("    End Sub");

                    appdefFile.WriteLine("    Private Sub TimerHandler(ByVal o As Object, ByVal args As EventArgs)");

                    appdefFile.WriteLine("        Dim timer As System.Windows.Threading.DispatcherTimer");
                    appdefFile.WriteLine("        timer = o");
                    appdefFile.WriteLine("        timer.Stop()");

                    appdefFile.WriteLine("        System.Windows.Application.Current.Shutdown()");

                    appdefFile.WriteLine("    End Sub");


                }

                appdefFile.WriteLine("        Sub HandleException(ByVal sender As Object, ByVal e As System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)");
                appdefFile.WriteLine("            If Not (e.Exception is Nothing)");
                appdefFile.WriteLine("                e.Handled = True");
                appdefFile.WriteLine("                Microsoft.Test.Serialization.SerializationHelper.StoreException(e.Exception)");
                appdefFile.WriteLine("                System.Windows.Application.Current.Shutdown()");
                appdefFile.WriteLine("            End If");
                appdefFile.WriteLine("        End Sub");
            }


            appdefFile.WriteLine("    ]]>");
            appdefFile.WriteLine("</x:Code>");

            // Write end tags.
            switch (hostType)
            {
                case "Application":
                    appdefFile.Write("</Application>");
                    break;
            }

            appdefFile.Close();
        }

        /// <summary>
        /// Deletes old app definition files, project files, and built components
        /// left over from previous runs.
        /// 
        /// When we run in an automated test run, the previous test process may not have
        /// died completely, and it may still hold access
        /// to the files/directories we are trying to delete. Thus, we make multiple 
        /// attempts to cleanup, sleeping for some time between attempts.        
        /// </summary>
        public void CleanUpCompilation()
        {
            GlobalLog.LogStatus("Cleaning up compilation directory, if there is any....");

            // Initialization for the multi-attempt logic
            bool cleanupDone = false;
            int maxAttempts = 5;
            int timeBetweenAttempts = 1000; // sleep time, in milliseconds

            int attempt = 0;
            while (!cleanupDone && attempt < maxAttempts)
            {
                attempt++;
                try
                {
                    // Delete appdef file.
                    if (FileSW.Exists(_appDefFileName))
                    {
                        GlobalLog.LogStatus("Found appdef file " + _appDefFileName + ". Deleting...");
                        FileSW.Delete(_appDefFileName);
                    }

                    string currentFolder = DirectorySW.GetCurrentDirectory();

                    // Delete bin directory.
                    string binPath = PathSW.Combine(currentFolder, "bin");
                    if (DirectorySW.Exists(binPath))
                    {
                        GlobalLog.LogStatus("Found bin folder " + binPath + ". Deleting...");
                        DirectorySW.Delete(binPath, true);
                        GlobalLog.LogStatus("Bin folder deleted.");
                    }

                    // Delete obj directory.
                    string objPath = PathSW.Combine(currentFolder, "obj");
                    if (DirectorySW.Exists(objPath))
                    {
                        GlobalLog.LogStatus("Found obj folder " + objPath + ". Deleting...");
                        DirectorySW.Delete(objPath, true);
                        GlobalLog.LogStatus("Obj folder deleted.");
                    }

                    // Cleanup done!
                    cleanupDone = true;
                }
                catch (Exception e)
                {
                    // We catch only IOException or UnauthorizedAccessException
                    // since those are thrown if some other process is using the 
                    // files and directories we are trying to delete.
                    if (e is IOException || e is UnauthorizedAccessException)
                    {
                        GlobalLog.LogStatus("Cleanup attempt #" + attempt + " failed.");
                        if ((1 == attempt) || (maxAttempts == attempt))
                        {
                            GlobalLog.LogStatus("Here are the active processes on the system.");
                            LogProcessesInfo();
                        }
                        if (maxAttempts == attempt)
                        {
                            GlobalLog.LogStatus("Maximum no. of cleanup attempts reached. Bailing out....");
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }

                Thread.Sleep(timeBetweenAttempts);
            }
        }
        #endregion Public members

        #region Private Methods
        /// <summary>
        /// For all processes running on the system, show the process name 
        /// and the name of the file that was used to create the process
        /// </summary>
        private void LogProcessesInfo()
        {
            ProcessSW[] processes = ProcessSW.GetProcesses();
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < processes.Length; i++)
            {
                ProcessSW process = processes[i];
                sb.Append(process.ProcessName);
                sb.Append(" ");
            }
            GlobalLog.LogDebug(sb.ToString());
        }
        #endregion Private Methods
    }
}

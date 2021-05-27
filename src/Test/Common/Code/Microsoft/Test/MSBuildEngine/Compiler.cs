// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Threading;
//using Microsoft.Test.Loaders;
#if (!STRESS_RUNTIME)
using Microsoft.Test.Logging;
#endif
using Microsoft.Test.MSBuildEngine;
using System.Collections.Generic;
using Microsoft.Test.Security.Wrappers;
using System.Security;
using System.Security.Permissions;
using Microsoft.Test.Loaders;

namespace Microsoft.Test.MSBuildEngine
{
    /// <summary>
    /// A reference used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Reference
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference(string fileName)
            : this(fileName, "")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Reference(string fileName, string hintPath)
        {
            FileName = fileName;
            HintPath = hintPath;
        }

        /// <summary>
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }

        /// <summary>
        /// </summary>
        public string HintPath
        {
            get
            {
                return _hintPath;
            }
            set
            {
                _hintPath = value;
            }
        }

        private string _fileName;
        private string _hintPath;
    }

    /// <summary>
    /// A resource used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Resource
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName)
            : this(fileName, "embedded")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName, string fileStorage)
            : this(fileName, fileStorage, false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Resource(string fileName, string fileStorage, bool localizable)
        {
            FileName = fileName;
            FileStorage = fileStorage;
            Localizable = localizable;
        }

        /// <summary>
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        private string _fileName;

        /// <summary>
        /// </summary>
        public string FileStorage
        {
            get
            {
                return _fileStorage;
            }
            set
            {
                _fileStorage = value;
            }
        }
        private string _fileStorage;

        /// <summary>
        /// </summary>
        public bool Localizable
        {
            get
            {
                return _localizable;
            }
            set
            {
                _localizable = value;
            }
        }
        private bool _localizable;
    }

    /// <summary>
    /// A content resource used in the Proj File.
    /// </summary>
    [Serializable()]
    public class Content
    {

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content()
            : this("")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content(string fileName)
            : this(fileName, "Always")
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary>
        public Content(string fileName, string copyToOutputDirectory)
        {
            FileName = fileName;
            CopyToOutputDirectory = copyToOutputDirectory;
        }

        /// <summary>
        /// </summary>
        public string FileName
        {
            get
            {
                return _fileName;
            }
            set
            {
                _fileName = value;
            }
        }
        private string _fileName;

        /// <summary>
        /// </summary>
        public string CopyToOutputDirectory
        {
            get
            {
                return _copyToOutputDirectory;
            }
            set
            {
                _copyToOutputDirectory = value;
            }
        }
        private string _copyToOutputDirectory;
    }


    /// <summary>
    /// </summary>
    public enum Languages
    {
        /// <summary>
        /// </summary>
        CSharp,

        /// <summary>
        /// </summary>
        VisualBasic
    }


    /// <summary>
    /// This is equivalent to the Proj File, a runtime version for setting up
    /// all the values for the Proj File.
    /// </summary> 
    [Serializable()]
    public class CompilerParams
    {
        /// <summary>
        /// Constructor.
        /// </summary> 
        public CompilerParams()
            : this(false)
        {
        }

        /// <summary>
        /// Constructor.
        /// </summary> 
        public CompilerParams(bool defaultReferences)
        {
            if (defaultReferences)
            {
                References.Add(new Reference("TestRuntime", ""));
                References.Add(new Reference("System", ""));
                References.Add(new Reference("System.Xml", ""));
                References.Add(new Reference("System.Data", ""));
                References.Add(new Reference("WindowsBase", ""));
                References.Add(new Reference("PresentationCore", ""));
                References.Add(new Reference("PresentationFramework", ""));
                References.Add(new Reference("UIAutomationTypes", ""));
                References.Add(new Reference("UIAutomationProvider", ""));
            }
        }


        /// <summary>
        /// List of all the references.
        /// </summary>        
        public List<Reference> References
        {
            get
            {
                return _references;
            }
        }

        /// <summary>
        /// List with all the Xaml Pages that are going to be compiled on the Proj File.
        /// </summary>    
        public List<string> XamlPages
        {
            get
            {
                return _xamlPages;
            }
        }


        /// <summary>
        /// List with all the Resources that are going to be compiled on the Proj File.
        /// </summary>    
        public List<Resource> Resources
        {
            get
            {
                return _resources;
            }
        }

        /// <summary>
        /// List with all the Loose Content Resources that are going to be compiled on the Proj File.
        /// </summary>    
        public List<Content> Contents
        {
            get
            {
                return _contents;
            }
        }

        /// <summary>
        /// List of all the code pages that are going to be part of the Proj File.
        /// </summary>    
        public List<string> CompileFiles
        {
            get
            {
                return _compileFiles;
            }
        }

        /// <summary>
        /// Set the Configuration type.
        /// </summary>    
        public string Configuration
        {
            set
            {
                _configuration = value;
            }
            get
            {
                return _configuration;
            }
        }

        /// <summary>
        /// Set to true if you want Baml to have debugging 
        /// info such as line numbers and positions. 
        /// </summary>    
        public bool XamlDebuggingInformation
        {
            set
            {
                _xamlDebuggingInformation = value;
            }
            get
            {
                return _xamlDebuggingInformation;
            }
        }

        /// <summary>
        /// Set the language 
        /// </summary>      
        public Languages Language
        {
            set
            {
                _language = value;
            }
            get
            {
                return _language;
            }
        }

        /// <summary>
        /// Set true if the project will be run hosted in the browser.
        /// </summary>    

        public bool HostInBrowser
        {
            set
            {
                _hostInBrowser = value;
            }
            get
            {
                return _hostInBrowser;
            }
        }

        /// <summary>
        /// Set Type of compilation
        /// </summary>    
        public string OutputType
        {
            set
            {
                _outputType = value;
            }
            get
            {
                return _outputType;
            }
        }

        /// <summary>
        /// Set to create .application manifest file.
        /// </summary>
        public bool GenerateManifests
        {
            set
            {
                _bgenmanifest = value;
            }
            get
            {
                return _bgenmanifest;
            }
        }

        /// <summary>
        /// Set Name of the Assemble generated by the Proj File.
        /// </summary>    
        public string AssemblyName
        {
            set
            {
                _assemblyName = value;
            }
            get
            {
                return _assemblyName;
            }
        }

        /// <summary>
        /// Set default Namespace.
        /// </summary>    
        public string RootNamespace
        {
            set
            {
                _rootNamespace = value;
            }
            get
            {
                return _rootNamespace;
            }
        }

        /// <summary>
        /// Set UICulture.
        /// </summary>    
        public string UICulture
        {
            set
            {
                _uiCulture = value;
            }
            get
            {
                return _uiCulture;
            }
        }

        /// <summary>
        /// Set name for the ApplicationDefinition on the Proj File.
        /// </summary>   
        public string ApplicationDefinition
        {
            set
            {
                _applicationDefinition = value;
            }
            get
            {
                return _applicationDefinition;
            }
        }

        /// <summary>
        /// Set the directory for the output of the build process.
        /// </summary>   
        public string OutputPath
        {
            set
            {
                _outputPath = value;
            }
            get
            {
                return _outputPath;
            }
        }

        /// <summary>
        /// Set the filename that contains the security permission requested.
        /// The default is FullTrust. If TargetZone is "" it won't be set it
        /// on the Proj File.
        /// </summary>   
        public string TargetZone
        {
            set
            {
                _targetZone = value;
            }
            get
            {
                return _targetZone;
            }
        }


        /// <summary>
        /// If TargetZone is "" (FullTrust), then we need to generate a manifiest.
        /// This property tell us a full path to generate it.
        /// If empty it will use CurrentDirectory.
        /// </summary>
        public string LocalPath
        {
            set
            {
                _localPath = value;
            }
            get
            {
                return _localPath;
            }
        }

        private string _localPath = "";
        private List<Content> _contents = new List<Content>();
        private List<Resource> _resources = new List<Resource>();
        private List<string> _xamlPages = new List<string>();
        private List<Reference> _references = new List<Reference>();
        private List<string> _compileFiles = new List<string>();
        private string _outputPath = "bin\\$(Configuration)\\";
        private string _configuration = "Release";
        private bool _xamlDebuggingInformation = false;
        private bool _hostInBrowser = false;
        private string _outputType = "winexe";
        private string _assemblyName = "";
        private string _rootNamespace = "";
        private string _uiCulture = "";
        private string _applicationDefinition = "";
        private Languages _language = Languages.CSharp;
        private string _targetZone = "";
        private bool _bgenmanifest = true;

    }

    /// <summary>
    /// This class encapsulate the usage of the MSBuildProjExecutor class to create and
    /// compile Proj Files.
    /// </summary>
    [Serializable()]
    [System.Security.Permissions.PermissionSet(System.Security.Permissions.SecurityAction.Assert, Name = "FullTrust")]
    public class Compiler
    {
        static Compiler()
        {
            msbuildDirectory = PathSW.GetDirectoryName(AssemblySW.Wrap(typeof(object).Assembly).Location);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="setupParams"></param>
        public Compiler(CompilerParams setupParams)
        {
            SetupParams = setupParams;
        }

        /// <summary>
        /// Compile the values passed on the constructor
        /// </summary>
        public virtual void Compile()
        {
            Compile(true);
        }

        /// <summary>
        /// Compile the values passed on the constructor
        /// </summary>     
        /// <returns>A list of compilation errors and warnings.</returns>
        public virtual List<ErrorWarningCode> Compile(bool saveProjFile)
        {
            //if (_testDomain == null)
            //{
            //    _testDomain = AppDomain.CreateDomain("CompilerAppDomain");
            //}
            
            //List<ErrorWarningCode> buildErrorsAndWarnings = null;

            //_testDomain.SetData("saveProjFile", saveProjFile);

            //try
            //{
            //    //_testDomain.DoCallBack(new CrossAppDomainDelegate(CompileCrossDomain));
            //}
            //finally
            //{
            //    if (_testDomain != null)
            //    {
            //        _testDomain.SetData("saveProjFile", null);                  
            //    }
            //}

            //buildErrorsAndWarnings = (List<ErrorWarningCode>)_testDomain.GetData("buildErrorsAndWarnings");

            List<ErrorWarningCode> buildErrorsAndWarnings = CompileInternal(saveProjFile);

            return buildErrorsAndWarnings;
        }

        //AppDomain _testDomain = null;


        /// Compile the values passed on the constructor
        // Callback that executes in a new AppDomain.
        private void CompileCrossDomain()
        {

            AppDomainSW currentDomain = AppDomainSW.Wrap(AppDomain.CurrentDomain);
            bool saveProjFile = (bool)currentDomain.GetData("saveProjFile");

            List<ErrorWarningCode> errors = CompileInternal(saveProjFile);

            currentDomain.SetData("buildErrorsAndWarnings", errors);

        }

        private List<ErrorWarningCode> CompileInternal(bool saveProjFile)
        {
            if (FileSW.Exists("ClickOnceTest.pfx"))
            {
                MSBuildProjExecutor.CleanSignFile = false;
            }


            MSBuildProjExecutor build = new MSBuildProjExecutor();
            build.CreateProject();
            build.CreateBuildPropertyGroup("");
            build.CreateBuildItemGroup("");
            build.AddProperty("Configuration", SetupParams.Configuration);
            build.AddProperty("Platform", "AnyCPU");
            build.AddProperty("XamlDebuggingInformation", Convert.ToString(SetupParams.XamlDebuggingInformation));
            build.AddProperty("OutputType", SetupParams.OutputType);

            if (!IsLibrary() && SetupParams.GenerateManifests)
            {
                build.AddProperty("GenerateManifests", Convert.ToString(SetupParams.GenerateManifests));
            }

            string hostInBrowser = "false";
            if (SetupParams.HostInBrowser)
            {
                hostInBrowser = "true";
            }

            build.AddProperty("HostinBrowser", hostInBrowser);

            string localPath = SetupParams.LocalPath;

            if (String.IsNullOrEmpty(localPath))
            {
                localPath = DirectorySW.GetCurrentDirectory();
            }

            if (!IsLibrary() && SetupParams.TargetZone == "")
            {
                // This is for Full Trust Compilation
                string appManifest = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
                "<asmv1:assembly manifestVersion=\"1.0\" xmlns=\"urn:schemas-microsoft-com:asm.v1\" xmlns:asmv1=\"urn:schemas-microsoft-com:asm.v1\" xmlns:asmv2=\"urn:schemas-microsoft-com:asm.v2\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\">" +
                "  <trustInfo xmlns=\"urn:schemas-microsoft-com:asm.v2\">" +
                "    <security>" +
                "      <applicationRequestMinimum>" +
                "        <PermissionSet class=\"System.Security.PermissionSet\" version=\"1\" Unrestricted=\"true\" ID=\"Custom\" SameSite=\"site\" />" +
                "        <defaultAssemblyRequest permissionSetReference=\"Custom\" />" +
                "      </applicationRequestMinimum>" +
                "    </security>" +
                "  </trustInfo>" +
                "</asmv1:assembly>";

                string securityManifestFullTrust = "<Security>" +
                "    <ApplicationRequestMinimum>" +
                "        <PermissionSet ID=\"FT\" temp:Unrestricted=\"true\" />" +
                "        <DefaultAssemblyRequest PermissionSetReference=\"FT\" />" +
                "      </ApplicationRequestMinimum>" +
                "</Security>";


                FileStreamSW fs = null;
                StreamWriterSW writer = null;

                if (!FileSW.Exists(app_manifest))
                {
                    try
                    {
                        string path = PathSW.Combine(localPath, app_manifest);
                        fs = new FileStreamSW(path, FileMode.Create, FileAccess.Write);
                        writer = new StreamWriterSW((Stream)fs.InnerObject);
                        writer.Write(appManifest);
                    }
                    finally
                    {

                        if (writer != null)
                        {
                            writer.Close();
                        }
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }

                if (!FileSW.Exists(SecurityManifestFullTrust))
                {
                    try
                    {
                        string path = PathSW.Combine(localPath, SecurityManifestFullTrust);
                        fs = new FileStreamSW(path, FileMode.Create, FileAccess.Write);
                        writer = new StreamWriterSW((Stream)fs.InnerObject);
                        writer.Write(securityManifestFullTrust);
                    }
                    finally
                    {

                        if (writer != null)
                        {
                            writer.Close();
                        }
                        if (fs != null)
                        {
                            fs.Close();
                        }
                    }
                }


                build.AddProperty("TargetZone", "Custom");
                build.AddItem("None", app_manifest);
                build.AddItem("None", SecurityManifestFullTrust);
            }

            build.AddProperty("AssemblyName", SetupParams.AssemblyName);

            build.AddProperty("OutputPath", SetupParams.OutputPath);

            build.AddProperty("RootNamespace", SetupParams.RootNamespace);

            build.AddProperty("UICulture", SetupParams.UICulture);

            if (!IsLibrary())
            {
                build.AddItem("None", "ClickOnceTest.pfx");

                build.AddProperty("SignManifests", "true");
                build.AddProperty("ManifestKeyFile", "ClickOnceTest.pfx");
                build.AddProperty("ManifestCertificateThumbprint", "cd582af19e477ae94a53102e0453e71b3c592a80");
            }

            if (SetupParams.Language == Languages.CSharp)
            {
                build.AddImport(MSBuildDirectory + @"\Microsoft.CSharp.targets");
            }
            else
            {
                build.AddImport(MSBuildDirectory + @"\Microsoft.VisualBasic.targets");
            }

            build.AddImport(MSBuildDirectory + @"\Microsoft.WinFx.targets");

            if (!String.IsNullOrEmpty(SetupParams.ApplicationDefinition))
            {
                build.AddItem("ApplicationDefinition", SetupParams.ApplicationDefinition);
            }

            for (int i = 0; i < SetupParams.XamlPages.Count; i++)
            {
                build.AddItem("Page", SetupParams.XamlPages[i]);
            }

            for (int i = 0; i < SetupParams.CompileFiles.Count; i++)
            {
                build.AddItem("Compile", SetupParams.CompileFiles[i]);
            }

            for (int i = 0; i < SetupParams.Resources.Count; i++)
            {
                build.AddResource(SetupParams.Resources[i].FileName, SetupParams.Resources[i].FileStorage, "");
            }

            for (int i = 0; i < SetupParams.Contents.Count; i++)
            {
                build.AddContent(SetupParams.Contents[i].FileName, SetupParams.Contents[i].CopyToOutputDirectory, "");
            }

            for (int i = 0; i < SetupParams.References.Count; i++)
            {
                build.AddReference(SetupParams.References[i].FileName, "", SetupParams.References[i].HintPath, "");
            }

            //HACK
            build.CommandLineArguements = "/t:Build";

            if (saveProjFile)
            {
                build.SaveProjectFile = Path.Combine(localPath, "__CompilerServicesSave.proj");
            }

            build.Build();

            if (saveProjFile)
            {
                build.SaveProjectFile = Path.Combine(localPath, "__CompilerServicesSave.proj");
            }

            if (build.OutputDirectory != null && !IsLibrary())
            {
                string compiledExtension;
                if (SetupParams.HostInBrowser)
                {                    
                    compiledExtension = ApplicationDeploymentHelper.BROWSER_APPLICATION_EXTENSION;
                }
                else
                {
                    
                    compiledExtension = ApplicationDeploymentHelper.STANDALONE_APPLICATION_EXTENSION;
                }

                if (SetupParams.GenerateManifests)
                {
                    string fileToSign = PathSW.Combine(build.OutputDirectory, SetupParams.AssemblyName + compiledExtension);
#if (!STRESS_RUNTIME)
                    Microsoft.Test.Logging.GlobalLog.LogStatus("Signing file: " + fileToSign);
#endif
//MQ                    AvalonDeploymentHelper.SignManifest(fileToSign);
                }
            }

            return build.UnhandledErrorsandWarningsList;
        }


        private bool IsLibrary()
        {
            if (SetupParams != null)
            {
                return (String.Compare(SetupParams.OutputType, "Library", true) == 0);
            }

            return false;
        }

        /// <summary>
        /// The directory containing MSBUILD.
        /// </summary>
        /// <returns>Fully qualified path to MSBUILD.</returns>
        public static string MSBuildDirectory
        {
            get
            {
                return msbuildDirectory;
            }
        }

        private static string msbuildDirectory = String.Empty;

        /// <summary>
        /// </summary>
        protected CompilerParams SetupParams = null;

        string SecurityManifestFullTrust = "SecurityManifestFullTrust.xml";
        string app_manifest = "app.manifest";
    }
}



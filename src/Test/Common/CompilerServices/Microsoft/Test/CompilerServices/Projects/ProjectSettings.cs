using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.Test.CompilerServices.Projects
{
    /// <summary>
    /// This is equivalent to the Proj File, a runtime version for setting up
    /// all the values for the Proj File.
    /// </summary> 
    [Serializable()]
    public class ProjectSettings
    {
        #region Private Fields
        private string localPath = "";
        private string applicationDefinition = "";
        private List<Content> contents = new List<Content>();
        private List<Resource> resources = new List<Resource>();
        private List<string> xamlPages = new List<string>();
        private List<Reference> references = new List<Reference>();
        private List<string> compileFiles = new List<string>();
        private Languages language = Languages.CSharp;
        private PropertyBag properties;
        #endregion

        /// <summary>
        /// Constructor.
        /// </summary> 
        public ProjectSettings()            
        {
            properties = new PropertyBag();
        }

        /// <summary>
        /// Returns a ProjectSettings with the approprite values for a wpf project
        /// </summary>
        /// <returns></returns>
        public static ProjectSettings CreateDefaultWPFProject()
        {
            ProjectSettings settings = new ProjectSettings();
            settings.References.Add(new Reference("TestRuntime", ""));
            settings.References.Add(new Reference("System", ""));
            settings.References.Add(new Reference("System.Xml", ""));
            settings.References.Add(new Reference("System.Data", ""));
            settings.References.Add(new Reference("WindowsBase", ""));
            settings.References.Add(new Reference("PresentationCore", ""));
            settings.References.Add(new Reference("PresentationFramework", ""));
            settings.References.Add(new Reference("UIAutomationTypes", ""));
            settings.References.Add(new Reference("UIAutomationProvider", ""));

            settings.ProjectProperties["Platform"] = "AnyCPU";
            settings.ProjectProperties["Configuration"] = "Release";
            settings.ProjectProperties["AssemblyName"] = "TestApp";
            settings.ProjectProperties["RootNamespace"] = "TestApp";
            settings.ProjectProperties["OutputType"] = "winexe";
            settings.ProjectProperties["OutputPath"] = "bin\\$(Configuration)\\";
            settings.ProjectProperties["HostInBrowser"] = "false";
            settings.ProjectProperties["XamlDebuggingInformation"] = "false";

            return settings;
        }

 

        #region Public Properties
        /// <summary>
        /// List of all the references.
        /// </summary>        
        public List<Reference> References
        {
            get
            {
                return references;
            }
        }

        /// <summary>
        /// List with all the Xaml Pages that are going to be compiled on the Proj File.
        /// </summary>    
        public List<string> XamlPages
        {
            get
            {
                return xamlPages;
            }
        }


        /// <summary>
        /// List with all the Resources that are going to be compiled on the Proj File.
        /// </summary>    
        public List<Resource> Resources
        {
            get
            {
                return resources;
            }
        }

        /// <summary>
        /// List with all the Loose Content Resources that are going to be compiled on the Proj File.
        /// </summary>    
        public List<Content> Contents
        {
            get
            {
                return contents;
            }
        }

        /// <summary>
        /// List of all the code pages that are going to be part of the Proj File.
        /// </summary>    
        public List<string> CompileFiles
        {
            get
            {
                return compileFiles;
            }
        }

        /// <summary>
        /// Set the language 
        /// </summary>      
        public Languages Language
        {
            set
            {
                language = value;
            }
            get
            {
                return language;
            }
        }

        /// <summary>
        /// Set name for the ApplicationDefinition on the Proj File.
        /// </summary>   
        public string ApplicationDefinition
        {
            set
            {
                applicationDefinition = value;
            }
            get
            {
                return applicationDefinition;
            }
        }

        /// <summary>
        /// If TargetZone is "" (FullTrust), then we need to generate a manifest.
        /// This property tell us a full path to generate it.
        /// If empty it will use CurrentDirectory.
        /// </summary>
        public string LocalPath
        {
            set
            {
                localPath = value;
            }
            get
            {
                return localPath;
            }
        }

        /// <summary>
        /// PropertyBag for storing all of the properties needs for creating an MSBuild project
        /// 

        public PropertyBag ProjectProperties
        {
            get
            {
                return properties;
            }
        }

       

        #endregion

    }
}

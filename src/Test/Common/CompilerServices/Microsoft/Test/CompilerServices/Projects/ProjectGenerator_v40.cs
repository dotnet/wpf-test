using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Build.Construction;
using Microsoft.Build.Evaluation;
using Microsoft.Build.Execution;

namespace Microsoft.Test.CompilerServices.Projects
{
    public class ProjectGenerator
    {
        #region Private Fields
        private ProjectSettings projectSettings;
        private string msBuildDirectory;

        //Build object caches.  This is implemented in this way to 
        //avoid having MSBuild types as parameters or return types
        private Project currentProject;
        private ProjectRootElement currentProjectRoot;
        private ProjectItemElement currentProjectItem;
        private ProjectItemGroupElement currentProjectItemGroup;
        private ProjectPropertyGroupElement currentProjectPropertyGroup;

        private const string securityManifestFullTrust = "SecurityManifestFullTrust.xml";
        private const string appManifest = "app.manifest";
        #endregion

        /// <summary>
        /// Handles MSBuild project generation
        /// </summary>
        /// <param name="ProjectSettings"></param>
        public ProjectGenerator(ProjectSettings ProjectSettings)
        {
            projectSettings = ProjectSettings;
            msBuildDirectory = Path.GetDirectoryName((typeof(object).Assembly).Location);
        }

        /// <summary>
        /// Generates a project from the ProjectSetting passed in via the constructor
        /// Saves project to a file with the provided name
        /// </summary>
        /// <param name="filename"></param>
        public void GenerateProjectFile(string filename)
        {
            CreateProject();
            CreateBuildPropertyGroup("");
            CreateBuildItemGroup("");

            IEnumerable<KeyValuePair<string, string>> properties = projectSettings.ProjectProperties as IEnumerable<KeyValuePair<string, string>>;
            foreach (KeyValuePair<string, string> property in properties)
            {
                AddProperty(property.Key, property.Value);
            }
            
            SetApplicationSettings();
            SetTargetSettings();
            AddIncludeFiles();

            currentProject.Save(filename);
        }

        #region Private Methods

        private void CreateProject()
        {
            if (currentProject == null)
            {
                currentProject = new Project();
                currentProjectRoot = currentProject.Xml;
            }
        }

        private void CreateBuildPropertyGroup(string condition)
        {
            if (currentProject == null)
            {
                throw new ApplicationException("A new project file needs to be created.");
            }

            currentProjectPropertyGroup = currentProjectRoot.AddPropertyGroup();

            if (String.IsNullOrEmpty(condition) == false)
            {
                currentProjectPropertyGroup.Condition = condition;
            }

        }

        private void CreateBuildItemGroup(string condition)
        {
            if (currentProject == null)
            {
                throw new ApplicationException("A new project file needs to be created.");
            }

            currentProjectItemGroup = currentProjectRoot.AddItemGroup();

            if (String.IsNullOrEmpty(condition) == false)
            {
                currentProjectItemGroup.Condition = condition;
            }
        }

        /// <summary>
        /// Add a new Property to the current project file.
        /// </summary>
        private void AddProperty(string propertyname, string propertyvalue)
        {
            AddProperty(propertyname, propertyvalue, null);
        }

        /// <summary>
        /// Add a new Property to the current project file with a Condition.
        /// </summary>
        private void AddProperty(string propertyName, string propertyValue, string condition)
        {
            if (String.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            if (String.IsNullOrEmpty(propertyValue))
            {
                throw new ArgumentNullException("propertyValue");
            }

            ProjectPropertyElement prop = currentProjectPropertyGroup.AddProperty(propertyName, propertyValue);

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
        /// removes a property from the current BuildPropertyGroup
        /// If property doesn't exists it does nothing
        /// </summary>
        private void RemoveProperty(string propertyName)
        {
            currentProject.RemoveProperty(currentProject.GetProperty(propertyName));
        }

        /// <summary>
        /// Add a BuildItem to the current project file
        /// </summary>
        private void AddItem(string itemtype, string includevalue)
        {
            AddItem(itemtype, includevalue, null);
        }

        /// <summary>
        /// Add a BuildItem to the current project file with an Exclude.
        /// </summary>
        private void AddItem(string itemtype, string includevalue, string exclude)
        {
            AddItem(itemtype, includevalue, exclude, null);
        }

        /// <summary>
        /// Add a BuildItem to the current project file with an Exclude and condition.
        /// </summary> 
        private void AddItem(string itemType, string includeValue, string exclude, string condition)
        {
            AddItem(itemType, includeValue, exclude, condition, null, null, null, null);
        }

        /// <summary>
        /// Add a BuildItem to the current project.  Master method that supports all types of Items.
        /// </summary>
        private void AddItem(string itemType, string includeValue, string exclude, string condition, string name, string hintpath, string fileStorage, string copyToOutputDirectory)
        {
            if (String.IsNullOrEmpty(itemType))
            {
                throw new ArgumentNullException("itemType");
            }
            if (String.IsNullOrEmpty(includeValue))
            {
                throw new ArgumentNullException("includeValue");
            }

            ProjectItemElement item = null;
            item = currentProjectItemGroup.AddItem(itemType, includeValue);

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
            //Reference specific
            if (String.IsNullOrEmpty(hintpath) == false)
            {
                item.AddMetadata("HintPath", hintpath);
            }

            if (String.IsNullOrEmpty(name) == false)
            {
                item.AddMetadata("Name", name);
            }
            //Resource specific
            if (String.IsNullOrEmpty(fileStorage) == false)
            {
                item.AddMetadata("FileStorage", fileStorage);
            }
            //Content specific
            if (String.IsNullOrEmpty(copyToOutputDirectory) == false)
            {
                item.AddMetadata("CopyToOutputDirectory", copyToOutputDirectory);
            }

            currentProjectItem = item;
        }

        /// <summary>
        /// Add a reference to the current project.
        /// </summary>
        private void AddReference(string includeValue, string name, string hintpath, string condition)
        {
            if (String.IsNullOrEmpty(includeValue))
            {
                throw new ArgumentNullException("includeValue");
            }
            AddItem("Reference", includeValue, null, condition, name, hintpath, null, null);
        }

        /// <summary>
        /// Add a reference to the current project with Private flag set.
        /// </summary>
        private void AddReference(string includeValue, string name, string hintpath, string condition, bool privatereference)
        {
            AddReference(includeValue, name, hintpath, condition);

            if (privatereference)
            {
                currentProjectItem.AddMetadata("Private", "True");
            }
            else
            {
                currentProjectItem.AddMetadata("Private", "False");
            }
        }

        /// <summary>
        /// Add a resource to the current project.
        /// </summary>
        private void AddResource(string includeValue, string fileStorage, string condition)
        {
            if (String.IsNullOrEmpty(includeValue))
            {
                throw new ArgumentNullException("includeValue");
            }
            AddItem("Resource", includeValue, null, condition, null, null, fileStorage, null);
        }

        /// <summary>
        /// Add a content resource to the current project.
        /// </summary>
        private void AddContent(string includeValue, string copyToOutputDirectory, string condition)
        {
            if (String.IsNullOrEmpty(includeValue))
            {
                throw new ArgumentNullException("includeValue");
            }
            AddItem("Content", includeValue, null, condition, null, null, null, copyToOutputDirectory);
        }

        /// <summary>
        /// Add Import to the current project file.
        /// </summary>
        private void AddImport(string includeValue)
        {
            if (String.IsNullOrEmpty(includeValue))
            {
                throw new ArgumentNullException("includeValue");
            }
            currentProjectRoot.AddImport(includeValue);
        }

        /// <summary>
        /// Saves an Application Manifest file to the local path directory
        /// </summary>
        private void GenerateAppManifest()
        {
            string appManifestString = "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
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

            string localPath = projectSettings.LocalPath;
            if (String.IsNullOrEmpty(localPath))
            {
                localPath = Directory.GetCurrentDirectory();
            }

            if (!File.Exists(appManifest))
            {
                WriteStringToFile(Path.Combine(localPath, appManifest), appManifestString);
            }
        }        

        private void SetApplicationSettings()
        {
            if (!String.IsNullOrEmpty(projectSettings.ApplicationDefinition))
            {
                AddItem("ApplicationDefinition", projectSettings.ApplicationDefinition);
            }

            if (!IsLibrary)
            {
                //when the project is not a library we will generate the manifests, not MSBuild
                if (projectSettings.ProjectProperties["GenerateManifests"] != null)
                {
                    RemoveProperty("GenerateManifests");
                }

                //if the TargetZone is not set, set it to Custom
                //and add the App and Security Manifests
                if (String.IsNullOrEmpty(projectSettings.ProjectProperties["TargetZone"]))
                {
                    GenerateAppManifest();

                    AddProperty("TargetZone", "Custom");
                    AddItem("None", appManifest);
                    AddItem("None", securityManifestFullTrust);
                }

                //when the project is not a library we will sign the output
                //this code allow the user to override the default signing behavior
                if (projectSettings.ProjectProperties["SignManifests"] == null)
                {

                    AddProperty("SignManifests", "true");
                }
                if (projectSettings.ProjectProperties["ManifestKeyFile"] == null
                    && projectSettings.ProjectProperties["ManifestCertificateThumbprint"] == null)
                {
                    AddProperty("ManifestKeyFile", "ClickOnceTest.pfx");
                    AddItem("None", "ClickOnceTest.pfx");
                    AddProperty("ManifestCertificateThumbprint", "cd582af19e477ae94a53102e0453e71b3c592a80");
                }                

            }
        }

        private void SetTargetSettings()
        {
            if (projectSettings.Language == Languages.CSharp)
            {
                AddImport(msBuildDirectory + @"\Microsoft.CSharp.targets");
            }
            else
            {
                AddImport(msBuildDirectory + @"\Microsoft.VisualBasic.targets");
            }

            AddImport(msBuildDirectory + @"\Microsoft.WinFx.targets");
        }

        private void AddIncludeFiles()
        {
            if (projectSettings.XamlPages.Count != 0)
            {
                CreateBuildItemGroup("");
                for (int i = 0; i < projectSettings.XamlPages.Count; i++)
                {
                    AddItem("Page", projectSettings.XamlPages[i]);
                }
            }

            if (projectSettings.CompileFiles.Count != 0)
            {
                CreateBuildItemGroup("");
                for (int i = 0; i < projectSettings.CompileFiles.Count; i++)
                {
                    AddItem("Compile", projectSettings.CompileFiles[i]);
                }
            }

            if (projectSettings.Resources.Count != 0)
            {
                CreateBuildItemGroup("");
                for (int i = 0; i < projectSettings.Resources.Count; i++)
                {
                    AddResource(projectSettings.Resources[i].FileName, projectSettings.Resources[i].FileStorage, "");
                }
            }

            if (projectSettings.Contents.Count != 0)
            {
                CreateBuildItemGroup("");
                for (int i = 0; i < projectSettings.Contents.Count; i++)
                {
                    AddContent(projectSettings.Contents[i].FileName, projectSettings.Contents[i].CopyToOutputDirectory, "");
                }
            }

            if (projectSettings.References.Count != 0)
            {
                CreateBuildItemGroup("");
                for (int i = 0; i < projectSettings.References.Count; i++)
                {
                    AddReference(projectSettings.References[i].FileName, "", projectSettings.References[i].HintPath, "");
                }
            }
        }

        private void WriteStringToFile(string path, string stringToWrite)
        {
            FileStream fs = null;
            StreamWriter writer = null;
            try
            {
                fs = new FileStream(path, FileMode.Create, FileAccess.Write);
                writer = new StreamWriter((Stream)fs);
                writer.Write(stringToWrite);
                writer.Flush();
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
        #endregion

        #region Private Properties

        /// <summary>
        /// Check if the current project builds a library
        /// </summary>
        private bool IsLibrary
        {
            get
            {
                if (projectSettings != null)
                {
                    return (String.Compare(projectSettings.ProjectProperties["OutputType"], "Library", true) == 0);
                }
                return false;
            }
        }

        #endregion
    }
}

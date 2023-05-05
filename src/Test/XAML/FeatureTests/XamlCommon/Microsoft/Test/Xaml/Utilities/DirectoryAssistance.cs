// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Xaml.Utilities
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Reflection;
    using Microsoft.Win32;

    /// <summary>
    /// Directory structure related helper class
    /// </summary>
    public static class DirectoryAssistance
    {
        /// <summary>
        /// Get temporary folder. 
        /// Folder is created if it doesn't exist
        /// </summary>
        /// <returns>temp folder</returns>
        public static string GetTestDataDirectory()
        {
            string dir = Environment.GetEnvironmentVariable("systemdrive") + @"\eth\cdf\TestData";
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        /// <summary>
        /// Get temporary folder. 
        /// Folder is created if it doesn't exist
        /// </summary>
        /// <param name="subpath">sub path contents</param>
        /// <returns>the full path to test data directory</returns>
        public static string GetTestDataDirectory(string subpath)
        {
            string path = Path.Combine(
                GetTestDataDirectory(),
                subpath);

            return path;
        }

        /// <summary>
        /// Returns new temp file name in temporary folder. 
        /// </summary>
        /// <returns>temp file name</returns>
        public static string GetTempFile()
        {
            return GetTempFileWithGuid("{0}");
        }

        /// <summary>
        /// Returns new temp file name in temporary folder. 
        /// </summary>
        /// <param name="nameFormat">Format for file name with place for guid. Example "myTempFile{0}.cs"</param>
        /// <returns>temp folder</returns>
        public static string GetTempFileWithGuid(string nameFormat)
        {
            string filePath = Path.Combine(
                GetArtifactDirectory(),
                String.Format(CultureInfo.InvariantCulture, nameFormat, Guid.NewGuid()));

            return filePath;
        }

        /// <summary>
        /// Get TestData\Artifacts temporary folder. 
        /// Folder is created if it doesn't exist
        /// </summary>
        /// <returns>artifacts folder</returns>
        public static string GetArtifactDirectory()
        {
            string dir = GetTestDataDirectory("Artifacts");
            if (!Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            return dir;
        }

        /// <summary>
        /// Get TestData\Artifacts temporary folder. 
        /// Folder is created if it doesn't exist
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>artifacts folder</returns>
        public static string GetArtifactDirectory(string subpath)
        {
            string path = Path.Combine(
                GetArtifactDirectory(),
                subpath);

            return path;
        }

        /// <summary>
        /// GetTestRootDirectory retreives the test root installation folder
        /// </summary>
        /// <returns>test root folder</returns>
        public static string GetTestBinsDirectory()
        {
            return Assembly.GetExecutingAssembly().Location;
        }

        /// <summary>
        /// GetTestRootDirectory retreives the test root installation folder
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>test root folder</returns>
        public static string GetTestBinsDirectory(string subpath)
        {
            string path = Path.Combine(
                GetTestBinsDirectory(),
                subpath);

            return path;
        }

        /// <summary>
        /// Gets the .Net framework folder
        /// </summary>
        /// <returns>the framework folder</returns>
        public static string GetDotNetFrameworkFolder()
        {
            string path = Path.GetDirectoryName(typeof(int).Assembly.Location);

            return path;
        }

        /// <summary>
        /// Gets the Dot Net framework folder
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>Gets the dot net framework folder</returns>
        public static string GetDotNetFrameworkFolder(string subpath)
        {
            string path = Path.Combine(
                GetDotNetFrameworkFolder(),
                subpath);

            return path;
        }

        /// <summary>
        /// Gets the folder for the Orcas (v3.5) version of the .NET Framework
        /// </summary>
        /// <returns>v 3.5 framework folder path</returns>
        public static string GetDotNetFramework35Folder()
        {
            try
            {
                string keyName = @"SOFTWARE\Microsoft\MSBuild\ToolsVersions\3.5";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
                {
                    if (key == null || key.GetValue("MsBuildToolsPath") == null)
                    {
                        throw new Exception(String.Format(CultureInfo.InvariantCulture, "Registry key for {0} not found", keyName));
                    }

                    return key.GetValue("MsBuildToolsPath") as string;
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Exception while getting dotNet35 folder. See inner exception for details", exc);
            }
        }

        /// <summary>
        /// Gets the Dot Net framework 3.5 folder
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>Gets the dot net framework 3.5 folder</returns>
        public static string GetDotNetFramework35Folder(string subpath)
        {
            string path = Path.Combine(
                GetDotNetFramework35Folder(),
                subpath);

            return path;
        }

        /// <summary>
        /// Gets the Dot Net framework 4.0 folder
        /// </summary>
        /// <returns>Gets the dot net framework 4.0 folder</returns>
        public static string GetDotNetFramework40Folder()
        {
            try
            {
                string keyName = @"SOFTWARE\Microsoft\MSBuild\ToolsVersions\4.0";
                using (RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName))
                {
                    if (key == null || key.GetValue("MsBuildToolsPath") == null)
                    {
                        throw new Exception(String.Format(CultureInfo.InvariantCulture, "Registry key for {0} not found", keyName));
                    }

                    return key.GetValue("MsBuildToolsPath") as string;
                }
            }
            catch (Exception exc)
            {
                throw new Exception("Exception while getting dotNet40 folder. See inner exception for details", exc);
            }
        }

        /// <summary>
        /// Gets the Dot Net framework 4.0 folder with subpath appended
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>Gets the dot net framework 4.0 folder with subpath appended</returns>
        public static string GetDotNetFramework40Folder(string subpath)
        {
            string path = Path.Combine(
                GetDotNetFramework40Folder(),
                subpath);

            return path;
        }

        /// <summary>
        /// GetVSCommon7IDEDirectory retreives the VS Common7 IDE 1003 Directory
        /// </summary>
        /// <returns>VS Common7 IDE 1003 Directory</returns>
        public static string GetVSCommon7IdeDirectory()
        {
            // we now work on Orcas RTM 
            string tools = Environment.GetEnvironmentVariable("VS90COMNTOOLS");
            if (String.IsNullOrEmpty(tools))
            {
                throw new InvalidOperationException("Visual Studio is required but is not present on this machine.");
            }

            string path = Path.Combine(
                tools,
                @"..\IDE");

            return path;
        }

        /// <summary>
        /// GetVSCommon7IDEDirectory retreives the VS Common7 IDE 1003 Directory
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>VS Common7 IDE 1003 Directory</returns>
        public static string GetVSCommon7IdeDirectory(string subpath)
        {
            string path = Path.Combine(
                GetVSCommon7IdeDirectory(),
                subpath);

            return path;
        }

        /// <summary>
        /// GetProductRootDirectory retreives the product root installation 
        /// folder
        /// </summary>
        /// <returns>product root folder</returns>
        public static string GetProductRootDirectory()
        {
            return GetDotNetFrameworkFolder();
        }

        /// <summary>
        /// Get the 3.5 framework reference assemblies folder
        /// </summary>
        /// <returns>reference assembly folder path</returns>
        public static string GetReferenceAssemblies35Folder()
        {
            try
            {
                string keyName = @"SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\v3.5";

                RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);

                if (key == null || key.GetValue("All Assemblies In") == null)
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, "Registry key for {0} not found", keyName));
                }

                return key.GetValue("All Assemblies In") as string;
            }
            catch (Exception exc)
            {
                throw new Exception("Exception while getting Reference Assemblies 3.5 folder. See inner exception for details", exc);
            }
        }

        /// <summary>
        /// Get the 3.5 reference assmblies folder
        /// </summary>
        /// <param name="subpath">subpath to use</param>
        /// <returns>the reference assembly path</returns>
        public static string GetReferenceAssemblies35Folder(string subpath)
        {
            string path = Path.Combine(
                GetReferenceAssemblies35Folder(),
                subpath);

            return path;
        }

        /// <summary>
        /// Get the 3.0 reference assemblies folder
        /// </summary>
        /// <returns>ref assembly folder</returns>
        public static string GetReferenceAssemblies30Folder()
        {
            try
            {
                string keyName = @"SOFTWARE\Microsoft\.NETFramework\AssemblyFolders\v3.0";

                RegistryKey key = Registry.LocalMachine.OpenSubKey(keyName);

                if (key == null || key.GetValue("All Assemblies In") == null)
                {
                    throw new Exception(String.Format(CultureInfo.InvariantCulture, "Registry key for {0} not found", keyName));
                }

                return key.GetValue("All Assemblies In") as string;
            }
            catch (Exception exc)
            {
                throw new Exception("Exception while getting Reference Assemblies 3.0 folder. See inner exception for details", exc);
            }
        }

        /// <summary>
        /// Get the 3.0 reference assembly folder
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>reference path</returns>
        public static string GetReferenceAssemblies30Folder(string subpath)
        {
            string path = Path.Combine(
                GetReferenceAssemblies30Folder(),
                subpath);

            return path;
        }

        /// <summary>
        /// Get the 4.0 reference assembly folder
        /// </summary>
        /// <param name="subpath">sub path to use</param>
        /// <returns>reference assembly path</returns>
        public static string GetReferenceAssemblies40Folder(string subpath)
        {
            string path = Path.Combine(
                GetDotNetFrameworkFolder(),
                subpath);

            return path;
        }
    }
}

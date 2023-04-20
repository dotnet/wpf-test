// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Windows.Baml2006;
using System.Xaml;
using System.Xml;
using Microsoft.Test.CompilerServices;
using Microsoft.Test.CompilerServices.Logging;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Utilities
{
    /******************************************************************************
    * CLASS:          BamlFactory
    ******************************************************************************/

    /// <summary>
    /// Class for creating a .baml file.
    /// </summary>
    public class BamlFactory
    {
        #region Public and Protected Members

        /// <summary> Path to which the project will be compiled. </summary>
        private static readonly string s_bamlDirectoryPath = Environment.CurrentDirectory + "\\obj\\Release\\";

        /// <summary> The name of the .csproj file to be compiled. </summary>
        private static readonly string s_projectFileName = "XamlToBaml.csproj";

        /// <summary> The name of the .xaml file in the to-be-compiled project. </summary>
        private static string s_projectXamlFileName = string.Empty;

        /******************************************************************************
        * Function:          CompileXamlToBaml
        ******************************************************************************/

        /// <summary>
        /// Compiles a .xaml file to a .baml file.  The incoming .xaml file is specified by the test case.  It is given
        /// a standard name and  copied to a standard projectXamlFileName that is included in the project file to be
        /// dynamically compiled. After setting up the project, then calls a function to compile it.
        /// </summary>
        /// <param name="xamlFileName">The name of the .xaml file to be tested.</param>
        /// <param name="globalProperties">Properties passed on to the build engine via CompilerServices.</param>
        /// <returns>The name of the .baml file that was created.</returns>
        public static string CompileXamlToBaml(string xamlFileName, Dictionary<string, string> globalProperties)
        {
            s_projectXamlFileName = Path.GetFileNameWithoutExtension(s_projectFileName) + ".xaml";

            // Copy the .xaml file to a standard file name.
            File.Copy(xamlFileName, s_projectXamlFileName, true);

            int delayCount = 3; // delay being introduced since IO operation from the previous test may be holding onto the binaries
            if (Directory.Exists(s_bamlDirectoryPath) && (delayCount-- > 0))
            {
                System.Threading.Thread.Sleep(new TimeSpan(0, 0, 1));
            }

            FileInfo projectFileInfo = new FileInfo(Environment.CurrentDirectory + "\\" + s_projectFileName);

            string bamlFileName = GenerateBamlFromProject(string.Empty, projectFileInfo, globalProperties);

            // Copy the .baml file from the project directory to the current directory where it is later referenced.
            File.Copy(s_bamlDirectoryPath + "\\" + bamlFileName, bamlFileName, true);

            return bamlFileName;
        }

        /******************************************************************************
        * Function:          GenerateBamlFromProject
        ******************************************************************************/

        /// <summary>
        /// Compiles a WPF project using the .xaml file passed in.
        /// </summary>
        /// <param name="targetFrameworkVersion">The target framework where the application will be run, Pass in String.Empty for default</param>
        /// <param name="projectFileInfo">The location and name of the project to be compiled.</param>
        /// <param name="globalProperties">Properties passed on to the build engine via CompilerServices.</param>
        /// <returns>The name of the .baml file that was created.</returns>
        public static string GenerateBamlFromProject(string targetFrameworkVersion, FileInfo projectFileInfo, Dictionary<string, string> globalProperties)
        {
            bool compileResult = false;
            string bamlFileName = Path.GetFileNameWithoutExtension(projectFileInfo.Name) + ".baml";

            // Compile the specified .xaml file, thereby creating a .baml file.
            Compiler compiler = new Compiler();

            if (compiler == null)
            {
                throw new TestSetupException("ERROR: the Compiler object returned null.");
            }

            BuildLogger logger = new BuildLogger();

            compileResult = compiler.CompileProject(projectFileInfo.Name, logger, targetFrameworkVersion, globalProperties, String.Empty);

            if (!compileResult)
            {
                GlobalLog.LogEvidence("Build errors :");
                foreach (BuildError error in logger.BuildErrors)
                {
                    GlobalLog.LogEvidence(error.Message);
                }

                throw new TestSetupException("ERROR: building " + projectFileInfo + "  failed.");
            }

            return bamlFileName;
        }

        /******************************************************************************
        * Function:          CleanUpBamlCompilation
        ******************************************************************************/

        /// <summary>
        /// Removes the .xaml file and the directory where it was compiled.
        /// </summary>
        /// <param name="bamlFileName">The name of a .baml file.</param>
        public static void CleanUpBamlCompilation(string bamlFileName)
        {
            if (!string.IsNullOrEmpty(s_projectXamlFileName) && File.Exists(s_projectXamlFileName))
            {
                File.Delete(s_projectXamlFileName);
            }

            if (!string.IsNullOrEmpty(bamlFileName) && File.Exists(bamlFileName))
            {
                File.Delete(bamlFileName);
            }

            if (Directory.Exists(s_bamlDirectoryPath))
            {
                Directory.Delete(s_bamlDirectoryPath, true);
            }
        }

        /******************************************************************************
        * Function:          GenerateBamlFromInfoset
        ******************************************************************************/

        /// <summary>
        /// Generates a .baml file from an infoset string using a Baml2006Writer.
        /// </summary>
        /// <param name="infosetString">The infoset to be transformed.</param>
        /// <returns>The name of the generated .baml file</returns>
        public static string GenerateBamlFromInfoset(string infosetString)
        {
            string bamlFileName = string.Empty;

            //// TO_DO: Add conditional compilation logic for this function.  Baml2006Writer will be implemented post-4.0.
            //// TO-DO: Call Baml2006Writer here.

            return bamlFileName;
        }

        #endregion
    }
}

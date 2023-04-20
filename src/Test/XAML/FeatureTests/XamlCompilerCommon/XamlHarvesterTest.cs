// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xaml;
using System.Xml;
using CodeGenInspect.Proxies;
using Microsoft.Xaml.Tools;

namespace XamlCompilerCommon
{
    /// <summary>
    ///  Test type for testing the XamlHarvester feature
    /// </summary>
    public class XamlHarvesterTest
    {
        /// <summary>
        /// Execute the test type.
        /// 
        /// The test type does the following
        ///  - Load the SilverlightXamlSchemaContext with the given local and 
        ///    reference assemblies
        ///  - Create a DOMRoot from the provided xaml file and the silverlight
        ///    schema context
        ///  - Pass the DOM to the harvester to get the XamlClassCodeInfo
        ///  - Serialize the XamlClassCodeInfo and compare it against a baseline
        ///  - It an exception occurred during the process, compare the exception with 
        ///    the baseline.
        ///    
        /// Please consult the Harvester spec for more details.
        /// </summary>
        public void RunTest(string xamlFilePath, string localAssemblyPath, string expectedClassCodeInfoPath, TestLogger logger)
        {
            this._logger = logger;
            this.ReferenceAssemblyPaths = new List<string>
            {
                @"mscorlib.dll",
                @"System.Windows.dll",
                @"System.dll",
                @"System.Core.dll",
                @"System.Net.dll",
                @"System.Xml.dll",
                @"System.Windows.Browser.dll",
            };

            string expectedClassCodeInfo = string.Empty;

            try
            {
                if (File.Exists(expectedClassCodeInfoPath))
                {
                    expectedClassCodeInfo = File.ReadAllText(expectedClassCodeInfoPath);
                }

                XamlClassCodeInfo xamlClassCodeInfo = GetXamlClassCodeInfo(ReferenceAssemblyPaths, xamlFilePath);

                string actualClassCodeInfo = xamlClassCodeInfo.ToString();
                logger.LogStatus("Actual XamlClassCodeInfo:");
                logger.LogStatus(actualClassCodeInfo);

                if (!string.Equals(expectedClassCodeInfo, actualClassCodeInfo, StringComparison.InvariantCultureIgnoreCase))
                {
                    logger.LogEvidence("Expected and actual class code info did not match");

                    LogFile(Path.GetFileName(expectedClassCodeInfoPath), expectedClassCodeInfo);
                    LogFile(Path.GetFileName(expectedClassCodeInfoPath) + ".actual", actualClassCodeInfo);
                    logger.LogFail();
                }
                else
                {
                    logger.LogStatus("Harvested information matched baseline");
                    logger.LogPass();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
                bool match = TryMatchExpectedException(ex, expectedClassCodeInfo);

                logger.LogEvidence("Exception during harvester test" + ex.ToString());


                if (match == true)
                {
                    logger.LogStatus("Expected and actual exception matched");
                    logger.LogPass();
                }
                else
                {
                    logger.LogFail();
                    logger.LogEvidence("Unexpected Exception - logging expected and actual information to files");
                    LogFile(expectedClassCodeInfoPath, expectedClassCodeInfo);
                    this._logger.LogEvidence(ex.ToString());
                }
            }
        }

        private bool TryMatchExpectedException(Exception actualException, string expectedClassCodeInfo)
        {
            try
            {
                ExpectedException expectedException = new ExpectedException();
                expectedException.ExceptionType = expectedClassCodeInfo;
                if (string.Equals(expectedException.ExceptionType, actualException.InnerException.GetType().ToString()))
                {
                    return true;
                }
                else
                {
                    this._logger.LogEvidence("Actual Exception Type: " + actualException.InnerException.GetType().ToString());
                    this._logger.LogEvidence("Expected: " + expectedException.ExceptionType);

                    return false;
                }
            }
            catch (Exception ex)
            {
                this._logger.LogEvidence("Error trying to look for expected exception information: " + ex.ToString());
                return false;
            }
        }

        private void LogFile(string fileName, string contents)
        {
            string filePath = Path.Combine(Path.GetTempPath(), fileName);
            File.WriteAllText(filePath,
               contents);
            this._logger.LogFile(filePath);
        }

        public static XamlClassCodeInfo GetXamlClassCodeInfo(List<string> referenceAssemblyPaths, string xamlFileName)
        {
            XamlSchemaContext schema = LoadSchemaContext(null, referenceAssemblyPaths);

            XamlReader xamlReader = GetXamlReader(File.ReadAllText(xamlFileName), schema, null);
            XamlCompilerReflectionHelper helper = new XamlCompilerReflectionHelper();
            CompilerDomRoot domToken = helper.CreateCompilerDomRoot(xamlReader);

            XamlHarvester harvester = new XamlHarvester();
            XamlClassCodeInfo xamlClassCodeInfo = harvester.Harvest(domToken, xamlFileName);

            return xamlClassCodeInfo;
        }

        private static XamlReader GetXamlReader(String xamlText, XamlSchemaContext schema, Assembly localAssembly)
        {
            TextReader textReader = new StringReader(xamlText);
            XmlReader xmlReader = XmlReader.Create(textReader);
            XamlXmlReaderSettings settings = new XamlXmlReaderSettings();
            settings.LocalAssembly = localAssembly;
            settings.AllowProtectedMembersOnRoot = true;
            XamlXmlReader xamlReader = new XamlXmlReader(xmlReader, schema, settings);

            return xamlReader;
        }

        private static XamlSchemaContext LoadSchemaContext(string localAssemblyPath, List<string> referenceAssemblyPaths)
        {
            AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(LoadResolveHander);

            Assembly localAssembly = null;
            List<Assembly> referenceAssemblies = null;
            if (localAssemblyPath != null)
            {
                try
                {
                    localAssembly = LoadAssembly(localAssemblyPath);
                }
                catch (FileNotFoundException)
                {
                }
            }

            if (referenceAssemblyPaths != null)
            {
                referenceAssemblies = LoadAssemblyList(referenceAssemblyPaths);
            }

            XamlSchemaContext schemaContext = null;

            // Creation of a Jupiter Schema context will be a little different
            // but similer to this.
            Assembly testAssembly = localAssembly;
            if (testAssembly == null)
            {
                testAssembly = GetSilverlightSystemWindows(referenceAssemblies);
            }
            if (testAssembly != null) //&& SilverlightAssemblyHelper.IsSilverlightAssembly(testAssembly))
            {
                // .NET Core 3.0
                // We don't have the code for XamlToolsSilverlight to port it, and we probably wouldn't be able to just load Silverlight assemblies, 
                // I suspect this code is not exercised anyway, adding an exception here that should bubble up in case we do.
                throw new NotImplementedException();
                // List<Assembly> localAssemblies = new List<Assembly>();
                // localAssemblies.Add(testAssembly);
                // schemaContext = SilverlightAssemblyHelper.CreateSilverlightSchemaContext(localAssemblies);
            }
            // else
            // {
                // throw new ArgumentException("Was not able to find a Silverlight reference assembly");
            // }
            return schemaContext;
        }

        static Assembly LoadResolveHander(object sender, ResolveEventArgs args)
        {
            string name = args.Name;
            Assembly[] loadedAsms = AppDomain.CurrentDomain.GetAssemblies();
            Assembly[] reflectionOnlyLoadedAsms = AppDomain.CurrentDomain.ReflectionOnlyGetAssemblies();

            AssemblyName asmName = new AssemblyName(name);
            if (asmName.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(Object).Assembly;
            }

            foreach (Assembly asm in loadedAsms)
            {
                if (asm.FullName == name)
                    return asm;
            }

            return null;
        }

        private static Assembly LoadAssembly(String path)
        {
            String fullPath = Path.GetFullPath(path);

            if (fullPath.EndsWith("mscorlib.dll", StringComparison.OrdinalIgnoreCase))
            {
                return typeof(Object).Assembly;
            }

            Assembly asm = Assembly.LoadFrom(path);
            return asm;
        }

        private static List<Assembly> LoadAssemblyList(List<String> pathList)
        {
            List<Assembly> asmList = new List<Assembly>();

            foreach (String path in pathList)
            {
                Assembly asm = LoadAssembly(path);
                asmList.Add(asm);
            }
            return asmList;
        }

        private static Assembly GetSilverlightSystemWindows(IEnumerable<Assembly> referenceAssemblies)
        {
            foreach (Assembly asm in referenceAssemblies)
            {
                AssemblyName asmName = new AssemblyName(asm.FullName);

                if (asmName.Name.Equals("system.windows", System.StringComparison.OrdinalIgnoreCase))
                {
                    return asm;
                }
            }
            return null;
        }

        private XamlSchemaContext SchemaContext { get; set; }
        private Assembly LocalAssembly { get; set; }
        private string LocalAssemblyPath { get; set; }
        private List<string> ReferenceAssemblyPaths { get; set; }
        private List<Assembly> ReferenceAssemblies { get; set; }
        private TestLogger _logger;
    }
}

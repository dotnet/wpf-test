// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Please see comments on class declaration below.
 *          
 * Contributor: 
 *
 
  
 * Revision:         $Revision: 1 $
 
********************************************************************/


using Microsoft.Test.MSBuildEngine;
using Microsoft.Test.Serialization;
using Microsoft.Test.Logging;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Markup;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using System.Security;
using System;
using Avalon.Test.CoreUI.Parser.Error;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Methods in this class test various aspects of Xmlns cache.
    /// 
    /// We load/compile a Xaml that uses types contained in a custom-created assembly. 
    /// We refer to the types using Xml namespaces, which are mapped to Clr namespaces 
    /// through mapping attributes exposed by the assembly. Depending on the test 
    /// parameters, we decide whether parser should be able to resolve a particular 
    /// type or not. We then verify the test result accordingly.
    /// </summary>
    [TestDefaults(SupportFiles= new String[] {@"FeatureTests\ElementServices\ParserTestControlsV1.dll",@"FeatureTests\ElementServices\ParserTestControlsFixedSize.dll",@"FeatureTests\ElementServices\XmlnsCacheTest.xaml",@"FeatureTests\ElementServices\Errors_XmlnsCacheTest.xml",@"FeatureTests\ElementServices\Errors_XmlnsCacheTest1.xml"})]
    public class XmlnsCacheTest
    {
        #region Public Methods
        #region TestCorruptAssembly
        /// <summary>
        /// This method tests a Compile time scenario.
        /// By design, at compile time parser gathers Xmlns attributes only from
        /// the assemblies that are passed as references in the project file.
        /// 
        /// In this method, we do the following:
        /// 1. Corrupt the custom assembly containing types used in the test Xaml. 
        /// 2. Compile the test Xaml, with the custom assembly (and an assembly it depends on) 
        ///    passed as references. 
        /// 3. Verify that compilation fails, saying it could not resolve the custom types.
        /// 
        /// This verifies that if any of the reference assemblies is corrupt, 
        /// parser ignores it and moves on. 
        /// </summary>
        [TestAttribute(1, @"Parser\XmlnsCache", TestCaseSecurityLevel.FullTrust, "TestCorruptAssembly", Area="XAML", Disabled=true)]
        public void TestCorruptAssembly()
        {
            List<string> supportingAssemblies = new List<string>(2);
            supportingAssemblies.Add(_assembly1);
            supportingAssemblies.Add(_assembly2);
            // Corrupt the assembly
            CorruptFile(Path.Combine(Environment.CurrentDirectory, _assembly2 + ".dll"));

            // Now compile the xaml file, with the assemblies as references.
            CompileAndVerifyError(_xamlFile, supportingAssemblies, _errorDatafile);
        }
        #endregion TestCorruptAssembly

        #region TestNonExistentAssembly
        /// <summary>
        /// This method tests a Compile time scenario.
        /// By design, at compile time parser gathers Xmlns attributes only from
        /// the assemblies that are passed as references in the project file.
        /// 
        /// In this method, we do the following:
        /// 1. Delete the custom assembly containing types used in the test Xaml. 
        /// 2. Compile the test Xaml, with the custom assembly (and an assembly it depends on) 
        ///    passed as references. 
        /// 3. Verify that compilation fails, saying it could not resolve the custom types.
        /// 
        /// This verifies that if any of the reference assemblies is not found, 
        /// parser ignores it and moves on. 
        /// </summary>
        [TestAttribute(1, @"Parser\XmlnsCache", TestCaseSecurityLevel.FullTrust, "TestNonExistentAssembly", Area = "XAML", Disabled = true)]
        public void TestNonExistentAssembly()
        {
            List<string> supportingAssemblies = new List<string>(2);
            supportingAssemblies.Add(_assembly1);
            supportingAssemblies.Add(_assembly2);
            // Delete the assembly, to make sure it's non-existent.
            // The HintPath provided to the compiler is current directory,
            // so we delete it from the current directory.
            File.Delete(Path.Combine(Environment.CurrentDirectory, _assembly2 + ".dll"));

            // Now compile the xaml file, with the assemblies as references.
            CompileAndVerifyError(_xamlFile, supportingAssemblies, _errorDatafile);
        }
        #endregion TestNonExistentAssembly

        #region TestAssemblyWithMissingDependency
        /// <summary>
        /// This method tests a Compile time scenario.
        /// By design, at compile time parser gathers Xmlns attributes only from
        /// the assemblies that are passed as references in the project file.
        /// 
        /// In this method, we do the following:
        /// 1. The custom assembly containing types used in the test Xaml depends on an assembly.
        ///    We delete that assembly.
        /// 2. Compile the test Xaml, with the custom assembly (and an assembly it depends on) 
        ///    passed as references. 
        /// 3. Verify that compilation fails, saying it could not resolve the custom types.
        /// 
        /// This verifies that if any of the reference assemblies cannot be reflected upon 
        /// (fails Assembly.ReflectionOnlyLoad() because its dependencies are missing), 
        /// parser ignores it and moves on. 
        /// </summary>
        [TestAttribute(1, @"Parser\XmlnsCache", TestCaseSecurityLevel.FullTrust, "TestAssemblyWithMissingDependency", Area = "XAML", Disabled = true)]        
        public void TestAssemblyWithMissingDependency()
        {
            List<string> supportingAssemblies = new List<string>(2);
            supportingAssemblies.Add(_assembly1);
            supportingAssemblies.Add(_assembly2);
            // Delete the dependency assembly.
            // The HintPath provided to the compiler is current directory,
            // so we delete it from the current directory.
            File.Delete(Path.Combine(Environment.CurrentDirectory, _assembly1 + ".dll"));

            // Now compile the xaml file, with the assemblies as references.
            CompileAndVerifyError(_xamlFile, supportingAssemblies, _errorDatafile1);
        }
        #endregion TestAssemblyWithMissingDependency
        #endregion Public Methods

        #region Private Methods
        /// <summary>
        /// This method compiles the given Xaml file, using the given assemblies 
        /// as references.
        /// It catches the compilation error and verifies that the error is same as the 
        /// expected error, as specified in the error data file proided.
        /// </summary>
        /// <param name="xamlfile"></param>
        /// <param name="supportingAssemblies"></param>
        /// <param name="errorfile"></param>
        private static void CompileAndVerifyError(string xamlfile, List<string> supportingAssemblies, string errorfile)
        {
            CompilerHelper compiler = new CompilerHelper();
            compiler.CleanUpCompilation();
            try
            {
                compiler.AddDefaults();
                compiler.CompileApp(xamlfile, "Application", null, null, supportingAssemblies, Languages.CSharp);
                throw new Microsoft.Test.TestValidationException("Compilation errors expected for " + xamlfile + ", but didn't occur."); 
            }
            catch (Microsoft.Test.TestSetupException e)
            {
                // Build errors and warnings are populated in e.Data
                List<ErrorWarningCode> buildErrorsAndWarnings = (List<ErrorWarningCode>)e.Data["buildErrorsAndWarnings"];
                ErrorWarningCode firstError = ExtractFirstError(buildErrorsAndWarnings);
                Hashtable actualErrData = CompilerHelper.ExtractErrData(firstError);

                // Get expected error data.
                XamlCompileDataManager dataManager = new XamlCompileDataManager(errorfile);
                Hashtable expectedErrData = dataManager.GetRecord(xamlfile);

                ErrComparer errComparer = new ReportingErrComparer();
                bool errorsSame = false;
                errComparer.Run(expectedErrData, actualErrData, null, ref errorsSame);

                if (!errorsSame)
                {
                    throw new Microsoft.Test.TestValidationException("Expected error is not same as the actual error");
                }
            }
        }

        /// <summary>
        /// Corrupt the given file.
        /// We do it by writing junk (asterix) in half of the file.
        /// </summary>
        /// <param name="filePath"></param>
        private void CorruptFile(string filePath)
        {
            long fileSize = new FileInfo(filePath).Length;
            FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            fs.Seek(fileSize / 4, SeekOrigin.Begin);
            byte[] junk = new byte[fileSize / 2];
            for (int i = 0; i < fileSize / 2; i++)
            {
                junk[i] = (byte)'*';
            }
            fs.Write(junk, 0, (int)fileSize / 2);
            fs.Close();
        }

        /// <summary>
        /// Returns the first error from a list of MSBuild errors and warnings.
        /// Returns null if the list is null/empty, or if there are all warnings and no error.
        /// </summary>
        /// <param name="buildErrorsAndWarnings"></param>
        /// <returns></returns>
        private static ErrorWarningCode ExtractFirstError(List<ErrorWarningCode> buildErrorsAndWarnings)
        {
            if ((null != buildErrorsAndWarnings) && (buildErrorsAndWarnings.Count > 0))
            {
                // Get the first error and return it.
                foreach (ErrorWarningCode errAndWarn in buildErrorsAndWarnings)
                {
                    if (ErrorType.Error == errAndWarn.Type)
                    {
                        return errAndWarn;
                    }
                }
            }
            return null;
        }
        #endregion Private Methods

        #region Private Variables
        private string _xamlFile = "XmlnsCacheTest.xaml";
        private string _errorDatafile = "Errors_XmlnsCacheTest.xml";
        private string _errorDatafile1 = "Errors_XmlnsCacheTest1.xml";
        private string _assembly1 = "ParserTestControlsV1";
        private string _assembly2 = "ParserTestControlsFixedSize";
        #endregion Private Variables
    }   
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Regression test
    /// </summary>
    public static class UnresolvableTypeInAssemblyTest
    {
        /// <summary>
        /// Core assembly 
        /// </summary>
        private static Assembly s_coreAssembly = null;

        /// <summary>
        /// Dependent assembly
        /// </summary>
        private static Assembly s_dependentAssembly;

        /// <summary>
        /// If the AppDomain contains a assembly with an unresolved assembly 
        /// attribute (for instance, the attribute references a type from another 
        /// assembly not loaded in the AppDomain), XamlReader.Load throws an 
        /// exception even if the markup does not reference that assembly.
        /// </summary>
        public static void RunTest()
        {
            bool testPassed = false;
            AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
            CompileAndLoadAssemblies(false, true);
            string markup = "<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'/>";
            object value = LoadMarkup(markup);

            if (value != null)
            {
                CompileAndLoadAssemblies(true, false);
                value = LoadMarkup(markup);

                if (value != null)
                {
                    testPassed = true;
                }
            }          

            if (testPassed)
            {
                GlobalLog.LogEvidence("Obtained null for XamlType as expected");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("Obtained non null XamlType. Expected null");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Assembly resolve handler for the appdomain
        /// </summary>
        /// <param name="sender">the sender</param>
        /// <param name="args">resolve arguments</param>
        /// <returns>the resolved assembly</returns>
        private static Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
        {
            if ((s_coreAssembly != null) &&
                (args.Name == s_coreAssembly.FullName))
            {
                return s_coreAssembly;
            }
            else if (args.Name == s_dependentAssembly.FullName)
            {
                return s_dependentAssembly;
            }

            return null;
        }

        /// <summary>
        /// Compile and optionally load the assembly
        /// </summary>
        /// <param name="deleteCoreAssembly">delete the core assembly</param>
        /// <param name="loadAssembly">load the dependent assembly</param>
        private static void CompileAndLoadAssemblies(bool deleteCoreAssembly, bool loadAssembly)
        {
            string coreAssemblyName = CompileAssembly(
                new string[] { "mscorlib.dll", "System.dll" },
                "CoreAssembly.dll",
                "public class LabelAttribute : System.Attribute {}");
           
            string dependentAssemblyName = CompileAssembly(
                new string[] { "mscorlib.dll", "System.dll", coreAssemblyName },
                "DependentAssembly.dll",
                "[assembly: Label]");

            GlobalLog.LogStatus("depenedent assembly is : " + dependentAssemblyName);
            if (loadAssembly)
            {
                s_dependentAssembly = Assembly.LoadFile(dependentAssemblyName);
            }

            if (deleteCoreAssembly)
            {
                File.Delete(coreAssemblyName);
            }
        }

        /// <summary>
        /// Compile the given code into an assembly
        /// </summary>
        /// <param name="assemblyNames">reference assemblies</param>
        /// <param name="assemblyName">target assembly name</param>
        /// <param name="code">the actual code to compile</param>
        /// <returns>compiled file name</returns>
        private static string CompileAssembly(string[] assemblyNames, string assemblyName, string code)
        {
            string fileName = Path.Combine(Path.GetTempPath(), assemblyName);
            using (CodeDomProvider codeProvider = new Microsoft.CSharp.CSharpCodeProvider())
            {
                CompilerParameters compilerParameters = new CompilerParameters(assemblyNames, fileName);
                CompilerResults results = codeProvider.CompileAssemblyFromSource(compilerParameters, code);
                if (results.Errors.Count > 0)
                {
                    foreach (CompilerError error in results.Errors)
                    {
                        GlobalLog.LogEvidence("{0}", error.ErrorText);
                    }

                    return null;
                }
            }

            return fileName;
        }

        /// <summary>
        /// Load the provided Xaml 
        /// </summary>
        /// <param name="markup">the xaml to load</param>
        /// <returns>loaded object</returns>
        private static object LoadMarkup(string markup)
        {
            using (StringReader stringReader = new StringReader(markup))
            {
                using (XmlReader xmlReader = XmlReader.Create(stringReader))
                {
                    object value = XamlReader.Load(xmlReader);
                    return value;
                }
            }         
        }
    }
}

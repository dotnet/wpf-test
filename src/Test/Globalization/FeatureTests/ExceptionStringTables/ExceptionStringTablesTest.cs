// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
using System.Resources;
using System.Windows;
using System.Windows.Markup;
using System.IO;
using System.Xml;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Globalization
{
    [Test(0, "ExceptionStringTables", "ExceptionStringTablesTest", SupportFiles = @"FeatureTests\Globalization\Data\ExceptionStringTablesFileList.xaml", SecurityLevel = TestCaseSecurityLevel.FullTrust, Keywords = "Localization_Suite")]
    public class ExceptionStringTablesTest : StepsTest
    {
        int _indentLevel = 0;
        
        public ExceptionStringTablesTest()
        {
            RunSteps += new TestStep(RunTest);
        }
        
        TestResult RunTest()
        {
            Log.Result = TestResult.Pass;
            bool passed = true;
            AssemblyResourceInfoList resourceList = (AssemblyResourceInfoList)XamlReader.Load(new FileStream("ExceptionStringTablesFileList.xaml", FileMode.Open));
            _indentLevel++;
            foreach (AssemblyResourceInfo ari in resourceList)
            {                
                Log.LogStatus(Indent() + "Checking: " + ari.AssemblyName);
                bool thisAssemblyPassed = true;
                _indentLevel++;
                try
                {
                    thisAssemblyPassed = VerifyAssembly(ari);
                }
                catch (Exception e)
                {
                    thisAssemblyPassed = false;
                    passed = false;
                    Log.LogStatus(e.ToString());
                }
                _indentLevel--;
                if (thisAssemblyPassed)
                {
                    Log.LogEvidence(Indent() + ari.AssemblyName + "  Passed");
                }
                else
                {
                    Log.LogEvidence(Indent() + ari.AssemblyName + "  Failed");
                }
            }
            _indentLevel--;
            
            if (passed)
            {
                Log.LogStatus("Test Succeeded");
            }
            else
            {
                Log.LogStatus("Test Failed");
            }
            return Log.Result;
        }

        //Loads the assembly with the appropriate method
        private Assembly LoadAssembly(AssemblyResourceInfo ari)
        {
            if (!String.IsNullOrEmpty(ari.AssemblyFullName))
            {
                return Assembly.Load(ari.AssemblyFullName);
            }
            else if (!String.IsNullOrEmpty(ari.AssemblyPath))
            {
                return Assembly.LoadFile(ari.AssemblyPath);
            }
            else
            {
                throw new Exception("Either AssemblyFullName or Assembly Path must be set");
            }
        }

        //Loads the assembly and a ResourceManager for each string table
        //Verifies each SRID entry has a non-empty string
        private bool VerifyAssembly(AssemblyResourceInfo ari)
        {
            Assembly asm = LoadAssembly(ari);
            Type[] types = asm.GetTypes();
            List<ResourceManager> rmList = new List<ResourceManager>();
            foreach (object tableObj in ari.ResourceTables)
            {
                string tableName = (string)tableObj;
                rmList.Add(new ResourceManager(tableName, asm));
            }
            bool passed = true;
            bool foundSRID = false;
            foreach (Type t in types)
            {
                if (t.Name.ToUpper().Contains("SR"))
                {
                    Log.LogStatus(Indent() + "Found SR type: " + t.Name);
                    foundSRID = true;
                    PropertyInfo[] pis = t.GetProperties();
                    foreach (PropertyInfo pi in pis)
                    {
                        if (pi.PropertyType.Name.ToUpper() == "SR")
                        {
                            object mySrid = pi.GetValue(null, null);
                            PropertyInfo stringProp = mySrid.GetType().GetProperty("String");
                            string val = stringProp.GetValue(mySrid, null) as string;
                            if (!string.IsNullOrEmpty(val))
                            {
                                string exceptionStr = null;
                                foreach (ResourceManager rm in rmList)
                                {
                                    exceptionStr = rm.GetString(val);
                                    if (!String.IsNullOrEmpty(exceptionStr))
                                        break;
                                }
                                if (String.IsNullOrEmpty(exceptionStr))
                                {
                                    Log.LogEvidence(Indent() + val + " was null");
                                    Log.Result = TestResult.Fail;
                                    passed = false;
                                }
                            }
                            break;
                        }                        
                    }
                    break;
                }                
            }
            if (!foundSRID)
            {
                Log.LogEvidence(Indent() + "SR type was not found");
                Log.Result = TestResult.Fail;
                passed = false;
            }
            return passed;
        }

        private string Indent()
        {
            return new string('\t', _indentLevel);
        }
    }
}

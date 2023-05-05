// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Markup;
using Microsoft.Test.Integration;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for the ParserLoadTest
    /// </summary>
    public class ParserLoadTest : XamlTestType
    {
        /// <summary>
        ///  Runs a ParserLoadTest which consists of:
        ///     Loading the xaml file and it's associated verifier
        ///     Verifying the object tree
        ///     Reloading the xaml for each mode and comparing the object trees
        /// Each test produces multiple variations, the number of which is determined by the number of ways
        /// to load a xaml file for the parser being tested
        /// </summary>
        public override void Run()
        {
            //// This causes the WPF assemblies to load, this is a hack that will be removed later
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;
            string xamlFile = DriverState.DriverParameters["File"];
            string verifierName = DriverState.DriverParameters["Verifier"];
            string testAssembly = DriverState.DriverParameters["TestAssembly"];
            string parseModeString = DriverState.DriverParameters["ParseMode"];

            if (String.IsNullOrEmpty(xamlFile))
            {
                throw new Exception("xamlFile cannot be null");
            }

            if (String.IsNullOrEmpty(parseModeString))
            {
                parseModeString = "allModes";
            }

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assemblies = DriverState.DriverParameters["SupportingAssemblies"];
                GlobalLog.LogStatus("Loading Assemblies: " + assemblies);
                FrameworkHelper.LoadSupportingAssemblies(assemblies);
            }

            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);
            object rootElement = null;
            object rootParseMode = null;
            IXamlTestParser parser = XamlTestParserFactory.Create();

            foreach (object parseMode in parser.ParseModes(parseModeString))
            {
                TestLog log = new TestLog(DriverState.TestName + "_" + parseMode.ToString());
                if (rootElement == null) // Initial load and verify
                {
                    //// For the initial load, we load the xaml, call the verifier and upon success
                    //// store the verified object tree for comparison
                    try
                    {
                        GlobalLog.LogStatus("Loading Xaml");
                        rootParseMode = parseMode;
                        rootElement = parser.LoadXaml(xamlFile, parseMode);
                    }
                    catch (Exception e)
                    {
                        GlobalLog.LogEvidence("Parsing failed with exception:\r\n" + e.Message);
                        GlobalLog.LogEvidence(e.ToString());
                        GlobalLog.LogStatus("Aborting test");
                        TestLog.Current.Result = TestResult.Fail;
                        log.Close();
                        return;
                    }

                    if (rootElement == null)
                    {
                        GlobalLog.LogEvidence("parser.LoadXaml did not return anything");
                        TestLog.Current.Result = TestResult.Fail;
                        log.Close();
                        return;
                    }
                    else
                    {
                        bool testResult = false;
                        try
                        {
                            //// Load and invoke the verifier
                            if (!String.IsNullOrEmpty(verifierName))
                            {
                                GlobalLog.LogStatus("Loading Verifier");
                                MethodInfo verifier = FrameworkHelper.LoadVerifier(testAssembly, verifierName);
                                GlobalLog.LogStatus("Verifying");
                                testResult = (bool) verifier.Invoke(null, new object[] { rootElement });
                            }
                            else
                            {
                                GlobalLog.LogStatus("No verifier specified for this file");
                                testResult = !(rootElement == null); // 
                            }
                        }
                        catch (Exception e)
                        {
                            GlobalLog.LogEvidence("Object tree verification failed with exception:\r\n" + e.Message);
                            GlobalLog.LogEvidence(e.ToString());
                            TestLog.Current.Result = TestResult.Fail;
                            log.Close();
                            return;
                        }

                        if (!testResult)
                        {
                            GlobalLog.LogEvidence("Object tree verification failed, aborting test");
                            TestLog.Current.Result = TestResult.Fail;
                            log.Close();
                            return;
                        }
                        else
                        {
                            GlobalLog.LogStatus("Object tree verification successful..");
                            TestLog.Current.Result = TestResult.Pass;
                        }
                    }
                }
                else
                {
                    //// For subsequent modes, we load the xaml and compare the tree produced with the reloaded verified tree
                    //// The original tree must be reloaded since some properties may have changed due to the previous comparision.
                    rootElement = parser.LoadXaml(xamlFile, rootParseMode);
                    try
                    {
                        if (parser.CompareXamlTrees(xamlFile, rootElement, parseMode))
                        {
                            TestLog.Current.Result = TestResult.Pass;
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                        }
                    }
                    catch (Exception e)
                    {
                        TestLog.Current.Result = TestResult.Fail;
                        GlobalLog.LogEvidence("Tree comparision failed for: " + parseMode.ToString());
                        GlobalLog.LogEvidence("With exception:\r\n" + e.Message);
                        GlobalLog.LogEvidence(e.ToString());
                    }
                }

                log.Close();
            }            
        }
    }
}

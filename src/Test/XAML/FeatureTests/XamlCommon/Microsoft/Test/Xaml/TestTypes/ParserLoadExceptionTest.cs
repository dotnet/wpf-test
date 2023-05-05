// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Parser;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for the ParserLoadExceptionTest
    /// </summary>
    public class ParserLoadExceptionTest : XamlTestType
    {
        /// <summary>
        /// Runs a ParserLoadExceptionTest which consists of:
        ///     Loading a bad xaml file and catching the exception
        ///     extracting the culture correct exception message from the parser's dll
        ///     comparing the messages
        /// </summary>
        public override void Run()
        {
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            string xamlFile = DriverState.DriverParameters["File"];
            string exceptionType = DriverState.DriverParameters["ExceptionType"];
            string sridName = DriverState.DriverParameters["SRIDname"];
            string parseModeString = DriverState.DriverParameters["ParseMode"];

            GlobalLog.LogStatus("File:     " + xamlFile);
            GlobalLog.LogStatus("SRIDname: " + sridName);
            GlobalLog.LogStatus("ExceptionType: " + exceptionType);
            
            if (String.IsNullOrEmpty(xamlFile))
            {
                throw new Exception("xamlFile cannot be null");
            }

            if (String.IsNullOrEmpty(exceptionType) && String.IsNullOrEmpty(sridName))
            {
                throw new Exception("exceptionType and sridName cannot both be null");
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

            IXamlTestParser parser = XamlTestParserFactory.Create();

            Regex exceptionRegex = null;
            string exceptionMessage = String.Empty;
            if (!String.IsNullOrEmpty(sridName))
            {
                exceptionMessage = parser.ExtractExceptionMessage(sridName);
                if (String.IsNullOrEmpty(exceptionMessage))
                {
                    GlobalLog.LogEvidence(String.Format("A matching exception message for SRID: {0} was not found.", sridName));
                    TestLog.Current.Result = TestResult.Fail;
                    return;
                }

                // Format the string and create a Regex, the * is used in generation of the regex
                exceptionMessage = String.Format(exceptionMessage, new object[] { ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*", ".*" });
               
                // for the specified Culture , make sure the regex pattern to be the right meaning
                if (System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-TW" || System.Globalization.CultureInfo.InstalledUICulture.Name == "zh-CN")
                {
                    exceptionMessage = EscapeRegexPattern(exceptionMessage);
                }
                
                exceptionRegex = new Regex(exceptionMessage);
            }

            foreach (object parseMode in parser.ParseModes(parseModeString))
            {
                TestLog log = new TestLog(DriverState.TestName + "_" + parseMode.ToString());
                try
                {
                    parser.LoadXaml(xamlFile, parseMode);

                    // Test fails if no exception is caught
                    GlobalLog.LogEvidence("No exception caught..");
                    TestLog.Current.Result = TestResult.Fail;
                }
                catch (Exception e)
                {
                    GlobalLog.LogStatus("Exception caught :" + e.ToString());
                    if (!String.IsNullOrEmpty(exceptionType))
                    {
                        if (!String.Equals(e.GetType().ToString(), exceptionType, StringComparison.InvariantCulture))
                        {
                            TestLog.Current.Result = TestResult.Fail;
                            GlobalLog.LogStatus("Exception type did not match. Actual[" + e.GetType().ToString() + "] Expected[" + exceptionType + "]");
                        }
                    }

                    if (null != exceptionRegex)
                    {
                        GlobalLog.LogStatus("Exception caught, attempting to match with: " + exceptionMessage);
                        if (exceptionRegex.IsMatch(e.Message))
                        {
                            TestLog.Current.Result = TestResult.Pass;
                            GlobalLog.LogStatus("Exception matched");
                        }
                        else
                        {
                            TestLog.Current.Result = TestResult.Fail;
                            GlobalLog.LogEvidence("Exception did not match.  Exception message:");
                            GlobalLog.LogEvidence(e.Message);
                        }
                    }
                    else
                    {
                        TestLog.Current.Result = TestResult.Pass;
                        GlobalLog.LogStatus("Exception type matched");
                    }
                }

                log.Close();
            }            
        }

        /// <summary>
        /// This method make sure the '(' ')' in Regex pattern string represent the right meaning
        /// But did not escape '*' for Regex pattern
        /// </summary>
        /// <param name="pattern"> regex string ready to transform </param>
        /// <returns> the transfromed regex string </returns>
        private string EscapeRegexPattern(string pattern)
        {
            StringBuilder sb = new StringBuilder(pattern);
            sb.Replace("(", Regex.Escape("("));
            sb.Replace(")", Regex.Escape(")"));
            return sb.ToString();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.XamlTextReader
{
    /// <summary>
    /// Parser Test
    /// </summary>
    /// <remarks>
    /// Verify passing an empty file to XamlTextReader
    /// </remarks>
    public class EmptyFileTest
    {
        #region RunTest

        /// <summary>
        /// Test case Entry point
        /// </summary>
        public void RunTest()
        {
            // This causes the WPF assemblies to load, this is a hack that will be removed later
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            string xamlPath = Assembly.GetAssembly(GetType()).Location;
            xamlPath = Path.GetDirectoryName(xamlPath) + "\\EmptyFile.xaml";

            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);

            try
            {
                var reader = new System.Xaml.XamlXmlReader(XmlReader.Create(xamlPath));
                TextWriter textWriter = new StringWriter();
                DiagnosticWriter diagWriter = new DiagnosticWriter(textWriter, reader.SchemaContext);
            
                XamlServices.Transform(reader, diagWriter);
                TestLog.Current.Result = TestResult.Fail; // Fail the test if no Exception occurs.
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("Exception caught while attempting load Xaml...");

                Type expectedException = typeof(XmlException);
                Type actualException = ex.GetType();

                if (actualException == expectedException)
                {
                    GlobalLog.LogEvidence("---PASS: Expected Exception was thrown.: " + actualException);
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("---FAIL: Incorrect Exception was thrown.\nExpected = " + expectedException + "\nActual = " + actualException);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        #endregion RunTest
    }
}

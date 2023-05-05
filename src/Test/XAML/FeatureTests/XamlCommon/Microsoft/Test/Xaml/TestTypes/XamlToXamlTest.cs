// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows;
using System.Xaml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Parser;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// XamlTestType for XamlRoundTripTest
    /// </summary>
    public class XamlToXamlTest : XamlTestType
    {
        #region XamlTestType Members

        /// <summary>
        /// Tests XAML to XAML scenarios by calling XamlServices.Transform
        /// with a XamlXmlReader and a XamlXmlWriter.
        /// Runs a XamlToXamlTest, which consists of:
        ///     Transforming the original xaml to a new xaml file
        ///     compare diagnostic traces of each
        /// </summary>
        public override void Run()
        {
            string originalFile = DriverState.DriverParameters["File"];
            XamlSchemaContext schemaContext = DriverState.DriverParameters["Mode"].ToLowerInvariant() == "wpf" ?
                System.Windows.Markup.XamlReader.GetWpfSchemaContext() : new XamlSchemaContext();
            string originalXaml = null;
            string transformedXaml = null;
            string originalTrace = null;
            string transformedTrace = null;
            string newSerializedFile = "_TransformedXaml_.xaml";

            if (String.IsNullOrEmpty(originalFile))
            {
                throw new Exception("originalFile cannot be null");
            }

            TestLog log = new TestLog(DriverState.TestName);

            try
            {
                originalXaml = File.ReadAllText(originalFile);

                // Transform xaml into new xaml
                XamlXmlReader reader = new XamlXmlReader(new StringReader(originalXaml), schemaContext);
                StringWriter transformedXamlWriter = new StringWriter();
                XamlXmlWriter writer = new XamlXmlWriter(transformedXamlWriter, schemaContext);

                XamlServices.Transform(reader, writer);
                transformedXaml = transformedXamlWriter.ToString();
                File.WriteAllText(newSerializedFile, transformedXaml, Encoding.Unicode);

                // Get diagnostic traces
                originalTrace = GetDiagnosticTrace(originalXaml, schemaContext);
                transformedTrace = GetDiagnosticTrace(transformedXaml, schemaContext);

                // compare traces
                if (originalTrace.Equals(transformedTrace))
                {
                    GlobalLog.LogStatus("Diagnostic traces match");
                    log.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("The actual Infoset failed to match the expected master.\nGo to the 'Logs location' shown below to compare the Infoset (.diagxaml) files.");
                    GlobalLog.LogStatus("Writing diagOutput to disk...");
                    File.WriteAllText("Original.diagxaml", originalTrace);
                    File.WriteAllText("Transformed.diagxaml", transformedTrace);
                    GlobalLog.LogStatus("Logging files...");
                    GlobalLog.LogFile(originalFile);
                    GlobalLog.LogFile(newSerializedFile);
                    GlobalLog.LogFile("Original.diagxaml");
                    GlobalLog.LogFile("Transformed.diagxaml");
                    log.Result = TestResult.Fail;
                }

                log.Close();
            }
            catch (Exception e)
            {
                log.LogEvidence(e);
                log.Result = TestResult.Fail;
                log.Close();
            }
        }

        #endregion

        /// <summary>
        /// Processes a diagnostic trace into canonical form
        /// </summary>
        /// <param name="diagTrace">the trace to process</param>
        /// <returns>canonical form</returns>
        private string ProcessTrace(string diagTrace)
        {
            StringReader reader = new StringReader(diagTrace);
            StringBuilder builder = new StringBuilder();
            string line = reader.ReadLine();

            // these namespaces can appear out of order
            string[] outOfOrderNamespaces = 
            {
                @"NS  xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""",
                @"NS  xmlns:mc=""http://schemas.openxmlformats.org/markup-compatibility/2006"""
            };

            while (line != null)
            {
                if (!outOfOrderNamespaces.Any((s) => line.Contains(s)))
                {
                    builder.AppendLine(line);
                }

                line = reader.ReadLine();
            }            

            return builder.ToString();
        }

        /// <summary>
        /// Get diagnostic trace for given xaml
        /// </summary>
        /// <param name="xaml">the xaml to process</param>
        /// <param name="schemaContext">schema context for transformation</param>
        /// <returns>the diagnostic trace</returns>
        private string GetDiagnosticTrace(string xaml, XamlSchemaContext schemaContext)
        {
            StringWriter diagText = new StringWriter();
            DiagnosticWriter diagWriter = new DiagnosticWriter(diagText, schemaContext);
            XamlXmlReader xamlReader = new XamlXmlReader(new StringReader(xaml), schemaContext);

            XamlServices.Transform(xamlReader, diagWriter);
            return ProcessTrace(diagText.ToString());
        }
    }
}

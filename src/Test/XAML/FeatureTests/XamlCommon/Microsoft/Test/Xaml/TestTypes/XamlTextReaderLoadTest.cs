// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Framework;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.TestTypes
{
    /// <summary>
    /// Class for XamlTextReaderLoad testing
    /// </summary>
    public class XamlTextReaderLoadTest : XamlTestType
    {
        /// <summary>
        /// Runs a XamlTextReaderLoadTest
        /// Load a xaml file, produces DiagXaml
        /// Compares DiagXaml to master file.
        /// </summary>
        public override void Run()
        {
            //// This causes the WPF assemblies to load, this is a hack that will be removed later
            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;
            string xamlFile = DriverState.DriverParameters["XamlFile"];
            string masterFile = DriverState.DriverParameters["MasterFile"];
            bool useWpfSchemaContext = false;

            if (String.IsNullOrEmpty(xamlFile))
            {
                throw new Exception("xamlFile cannot be null");
            }

            if (String.IsNullOrEmpty(masterFile))
            {
                throw new Exception("masterFile cannot be null");
            }

            if (!String.IsNullOrEmpty(DriverState.DriverParameters["UseWpfSchemaContext"]))
            {
                useWpfSchemaContext = bool.Parse(DriverState.DriverParameters["UseWpfSchemaContext"]);
            }

            // Load any supporting assemblies
            if (!String.IsNullOrEmpty(DriverState.DriverParameters["SupportingAssemblies"]))
            {
                string assemblies = DriverState.DriverParameters["SupportingAssemblies"];
                GlobalLog.LogStatus("Loading Assemblies: " + assemblies);
                FrameworkHelper.LoadSupportingAssemblies(assemblies);
            }

            TestLog log = new TestLog(DriverState.TestName);
            GlobalLog.LogStatus("DisplayName: " + DriverState.TestName);

            TextWriter textWriter = new StringWriter();
            DiagnosticWriter diagWriter;

            // Load Xaml and produce a DiagXaml string
            try
            {
                if (useWpfSchemaContext)
                {
                    XamlSchemaContext schemaContext = System.Windows.Markup.XamlReader.GetWpfSchemaContext();
                    diagWriter = new DiagnosticWriter(textWriter, schemaContext);
                    diagWriter.FromMemberText = "Retrieved";
                    XamlServices.Transform(new XamlXmlReader(XmlReader.Create(xamlFile), schemaContext), diagWriter);
                }
                else
                {
                    var reader = new XamlXmlReader(XmlReader.Create(xamlFile));
                    diagWriter = new DiagnosticWriter(textWriter, reader.SchemaContext);
                    diagWriter.FromMemberText = "Retrieved";
                    XamlServices.Transform(reader, diagWriter);
                }
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("Exception caught while attempting load Xaml...");
                GlobalLog.LogEvidence(ex);
                TestLog.Current.Result = TestResult.Fail;
                log.Close();
                return;
            }

            string diagOutput = textWriter.ToString();

            // Verify string exists
            if (String.IsNullOrEmpty(diagOutput))
            {
                GlobalLog.LogEvidence("No DiagXaml returned, test failed");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            // Special case:  xml:base
            diagOutput = HandleXmlBase(diagOutput);

            // Load the master file and retrieve its contents
            string masterString;
            try
            {
                StreamReader streamReader = new StreamReader(masterFile);
                masterString = streamReader.ReadToEnd();
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("Reading master file failed..");
                GlobalLog.LogEvidence(ex);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            // Compare the master to the created DiagXaml
            if (masterString.Equals(diagOutput))
            {
                GlobalLog.LogStatus("Strings matched, test sucessful");
                TestLog.Current.Result = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogEvidence("The actual Infoset failed to match the expected master.\nGo to the 'Logs location' shown below to compare the Infoset (.diagxaml) files:\n\tExpected:  " + DriverState.TestName + ".diagxaml\n\tActual:    DiagOutput.DiagXaml\n\tOriginal Markup:  " + DriverState.TestName + ".xaml");
                GlobalLog.LogStatus("Writing diagOutput to disk...");
                StreamWriter writer = new StreamWriter("DiagOutput.DiagXaml");
                writer.Write(diagOutput);
                writer.Flush();
                writer.Close();
                GlobalLog.LogStatus("Logging files...");
                GlobalLog.LogFile(masterFile);
                GlobalLog.LogFile(xamlFile);
                GlobalLog.LogFile("DiagOutput.DiagXaml");
                TestLog.Current.Result = TestResult.Fail;
            }

            log.Close();
        }

        /// <summary>
        /// An SP node that contain "xml:base" is followed by a Text (T) node with a local url.
        /// Each such T node is replaced by a constant string, so that the result string will always match the master.
        /// </summary>
        /// <param name="diagXamlString">The diag xaml string.</param>
        /// <returns>string value</returns>
        private string HandleXmlBase(string diagXamlString)
        {
            string newString = "  V \"FILE LOCATION STRING\"";

            StringReader reader = new StringReader(diagXamlString);
            StringBuilder builder = new StringBuilder();
            string line = reader.ReadLine();

            while (line != null)
            {
                if (line.Contains("SM"))
                {
                    if (line.Contains("xml:base"))
                    {
                        builder.AppendLine(line); // Append the xml:base line
                        reader.ReadLine(); // Skip the next line (T)
                        builder.AppendLine(newString); // Replace the line 
                        break; // xml:base is only valid once on the root element
                    } // so after we've seen it, we're done
                }

                builder.AppendLine(line);
                line = reader.ReadLine();
            }

            line = reader.ReadToEnd(); // if we didn't encounter xml:base this will be empty
            if (!String.IsNullOrEmpty(line))
            {
                builder.Append(line);
            }

            builder.AppendLine();
            return builder.ToString();
        }
    }
}

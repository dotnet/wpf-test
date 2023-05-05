// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// Regression test
    /// </summary>
    public class UnknownCollectionInXxrXxwRoundtrip
    {
        /// <summary>
        /// XXR -> XXW roundtrip fails for a Xaml with a collection of an unknown type containing 2 or more elements
        /// </summary>
        public static void RegressionIssue140()
        {
            string xaml = @"
<Window x:Class='LocallyDefinedPanel.MainWindow'
        xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'
        xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml'
        xmlns:l='clr-namespace:LocallyDefinedPanel'
        Title='MainWindow' Height='240' Width='500'>
    <l:RobPanel>
        <Button>Button1</Button>
        <Button>Button2</Button>
    </l:RobPanel>
</Window>";

            string expectedOutput = @"<Window x:Class=""LocallyDefinedPanel.MainWindow"" Title=""MainWindow"" Height=""240"" Width=""500"" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation"" xmlns:l=""clr-namespace:LocallyDefinedPanel"" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml"">
  <l:RobPanel>
    <Button>Button1</Button> <Button>Button2</Button></l:RobPanel>
</Window>";

            XamlXmlReader xamlXmlReader = new XamlXmlReader(new StringReader(xaml));

            TextWriter textWriter = new StringWriter(CultureInfo.InvariantCulture);
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings();
            xmlWriterSettings.Indent = true;
            xmlWriterSettings.OmitXmlDeclaration = true;
            xmlWriterSettings.CloseOutput = true;
            XmlWriter xmlWriter = XmlWriter.Create(textWriter, xmlWriterSettings);
            XamlXmlWriter xamlXmlWriter = new XamlXmlWriter(xmlWriter, xamlXmlReader.SchemaContext);

            XamlServices.Transform(xamlXmlReader, xamlXmlWriter);

            if (textWriter.ToString() != expectedOutput)
            {
                GlobalLog.LogDebug("Expected output: " + expectedOutput);
                GlobalLog.LogDebug("Actual output: " + textWriter.ToString());
                GlobalLog.LogEvidence("Actual output didn't match expected output");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("Actual output matched expected output");
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }
}

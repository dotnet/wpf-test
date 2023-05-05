// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.XamlTextReader
{
    /// <summary>
    ///  Regression test
    /// </summary>
    public class XamlSubReaderLineInfoTest
    {
        /// <summary>
        /// Verify that a SubReader's LineNumber and LinePosition are correct.
        /// </summary>
        public void RunTest()
        {
            TestResult testResult = TestResult.Fail;

            FrameworkElement frameworkElement = new FrameworkElement();
            frameworkElement = null;

            try
            {
string xamlString = @"
<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>
    <Button Name='Button1'>
        <Button.Content>
            <StackPanel Background='DodgerBlue'>
                <TextBox Text='textbox1' />
                <Unknown>
                    <Unknown.Prop>abc</Unknown.Prop>
                </Unknown>
            </StackPanel>
        </Button.Content>
    </Button>
</Canvas>";

                var reader = new XamlXmlReader(XmlReader.Create(new StringReader(xamlString)), new XamlXmlReaderSettings { ProvideLineInfo = true });

                while (reader.Read())
                {
                    if (reader.NodeType == XamlNodeType.StartObject)
                    {
                        XamlServices.Load(reader.ReadSubtree());
                    }
                }
            }
            catch (XamlObjectWriterException ex)
            {
                int expectedLineNumber = 7;
                int expectedLinePosition = 18;

                if (ex.LineNumber == expectedLineNumber && ex.LinePosition == expectedLinePosition)
                {
                    GlobalLog.LogEvidence("PASS: LineNumber (" + ex.LineNumber.ToString() + ") and LinePosition (" + ex.LinePosition.ToString() + ") are correct.");
                    testResult = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect LineNumber and/or LinePosition");
                    GlobalLog.LogEvidence("Expected LineNumber:   " + expectedLineNumber.ToString() + " / Actual LineNumber:   " + ex.LineNumber.ToString());
                    GlobalLog.LogEvidence("Expected LinePosition: " + expectedLinePosition.ToString() + " / Actual LinePosition: " + ex.LinePosition.ToString());
                }
            }

            TestLog.Current.Result = testResult;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.TestTypes;

namespace Microsoft.Test.Xaml.Parser.MethodTests
{
    /******************************************************************************
    * CLASS:          XamlBackgroundReaderDefaults
    ******************************************************************************/

    /// <summary>
    /// Class for XamlBackgroundReaderDefaults
    /// </summary>
    public class XamlBackgroundReaderDefaults : XamlTestType
    {
        /******************************************************************************
        * Function:          Run
        ******************************************************************************/

        /// <summary>
        /// Verifies XamlBackgroundReader features.
        /// </summary>
        public override void Run()
        {
            bool test1Passed = true;

            FrameworkElement fe = new FrameworkElement();
            fe = null;

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();

            XamlXmlReaderSettings xxrSettings = new XamlXmlReaderSettings();
            xxrSettings.ProvideLineInfo = true;

            string xamlFile = DriverState.DriverParameters["XamlFile"] + ".xaml";
            XamlXmlReader xamlXmlReader = new XamlXmlReader(XmlReader.Create(xamlFile), xamlSchemaContext, xxrSettings);

            using (XamlBackgroundReader xbr = new XamlBackgroundReader(xamlXmlReader))
            {
                if (xbr.LineNumber != 0)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect LineNumber");
                    GlobalLog.LogEvidence("Expected LineNumber:   0 / Actual LineNumber:   " + xbr.LineNumber);
                    test1Passed = false;
                }

                if (xbr.LinePosition != 0)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect LinePosition");
                    GlobalLog.LogEvidence("Expected LinePosition:   0 / Actual LinePosition:   " + xbr.LinePosition);
                    test1Passed = false;
                }

                if (xbr.HasLineInfo == false)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect HasLineInfo");
                    GlobalLog.LogEvidence("Expected HasLineInfo:   true / Actual HasLineInfo:   " + xbr.HasLineInfo);
                    test1Passed = false;
                }

                if (xbr.IsEof == true)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect IsEof");
                    GlobalLog.LogEvidence("Expected IsEof:   false / Actual IsEof:   " + xbr.IsEof);
                    test1Passed = false;
                }

                if (xbr.Member != null)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect Member");
                    GlobalLog.LogEvidence("Expected Member:   null / Actual Member:   " + xbr.Member.ToString());
                    test1Passed = false;
                }

                if (xbr.Namespace != null)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect Namespace");
                    GlobalLog.LogEvidence("Expected Namespace:   null / Actual Namespace:   " + xbr.Namespace);
                    test1Passed = false;
                }

                if (xbr.NodeType != XamlNodeType.None)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect NodeType");
                    GlobalLog.LogEvidence("Expected NodeType:   None / Actual NodeType:   " + xbr.NodeType);
                    test1Passed = false;
                }

                if (xbr.SchemaContext != xamlSchemaContext)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect SchemaContext");
                    GlobalLog.LogEvidence("Expected SchemaContext: " + xamlSchemaContext + " / Actual SchemaContext:   " + xbr.SchemaContext);
                    test1Passed = false;
                }

                if (xbr.Type != null)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect Type");
                    GlobalLog.LogEvidence("Expected Type:   null / Actual Type:   " + xbr.Type);
                    test1Passed = false;
                }

                if (xbr.Value != null)
                {
                    GlobalLog.LogEvidence("FAIL: Incorrect Value");
                    GlobalLog.LogEvidence("Expected Value:   null / Actual Value:   " + xbr.Value);
                    test1Passed = false;
                }

                xbr.StartThread();
                XamlObjectWriter xow = new XamlObjectWriter(xbr.SchemaContext);
                XamlServices.Transform(xbr, xow);
                object root = xow.Result;

                if (root == null)
                {
                    GlobalLog.LogEvidence("A null object tree was returned.");
                    test1Passed = false;
                }

                if (!test1Passed)
                {
                    throw new TestValidationException("FAIL: Test failed.");
                }
            }

            TestLog.Current.Result = TestResult.Pass;
        }
    }
}

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
    * CLASS:          XamlBackgroundReaderTest
    ******************************************************************************/

    /// <summary>
    /// Class for XamlBackgroundReaderTest
    /// </summary>
    public class XamlBackgroundReaderTest : XamlTestType
    {
        /// <summary>
        /// Verifies that XamlBackgroundReader can be used to successfully create an object.
        /// </summary>
        public override void Run()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            string xamlFile = DriverState.DriverParameters["XamlFile"] + ".xaml";

            XamlSchemaContext xamlSchemaContext = new XamlSchemaContext();
            XamlXmlReader xamlXmlReader = new XamlXmlReader(XmlReader.Create(xamlFile), xamlSchemaContext);

            XamlBackgroundReader xbr = new XamlBackgroundReader(xamlXmlReader);
            GlobalLog.LogStatus("Starting the XamlBackgroundReader thread.");
            xbr.StartThread();

            XamlObjectWriter xow = new XamlObjectWriter(xbr.SchemaContext);

            XamlServices.Transform(xbr, xow);

            object root = xow.Result;

            if (root == null)
            {
                GlobalLog.LogEvidence("A null object tree was returned.");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogEvidence("A non-null object tree was returned.");
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }
}

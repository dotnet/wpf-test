// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.RegressionTests
{
    /// <summary>
    /// WriteOnlyAttachedProperty Test
    /// </summary>
    public static class WriteOnlyAttachedPropertyTest
    {
        /// <summary>
        /// Scenario:  an Attached property that has no getter.
        /// </summary>
        public static void RunTest()
        {
            bool valueFound = false;
            string xamlString = @"<cmn:CustomRoot
                xmlns:cmn='http://XamlTestTypes'>
                <cmn:CustomItem cmn:CustomAttached.AttachedProp='123'></cmn:CustomItem>
                </cmn:CustomRoot>";

            XmlReader xmlReader = XmlReader.Create(new StringReader(xamlString));
            XamlSchemaContext xsc = new XamlSchemaContext();
            System.Xaml.XamlXmlReader xamlTextReader = new System.Xaml.XamlXmlReader(xmlReader, xsc);
            XamlObjectWriter objWriter = new XamlObjectWriter(xamlTextReader.SchemaContext);

            while (xamlTextReader.Read())
            {
                objWriter.WriteNode(xamlTextReader);

                if ((string) xamlTextReader.Value == "123")
                {
                    valueFound = true;
                }
            }

            if (objWriter.Result == null)
            {
                GlobalLog.LogEvidence("The ObjectWriter returned a null root.");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                if (valueFound)
                {
                    GlobalLog.LogEvidence("The Custom Attached Property value was found.");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("The Custom Attached Property value was found.");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }
    }
}

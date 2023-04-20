// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;

namespace Microsoft.Test.Xaml.Parser.MethodTests.IXSCP
{
    /// <summary>
    /// IXamlSchemaContextProvider Test
    /// </summary>
    public static class IXamlSchemaContextProviderTest
    {
        /// <summary>
        /// Verifies that the context returned by IXamlSchemaContextProvider is 
        /// reference equals to the one passed in.  Verifiers both the MarkupExtension 
        /// and TypeConverter scenarios.
        /// </summary>
        public static void RunTest()
        {
            int expectedChildrenCount = 2;
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            XmlReader xmlReader = XmlReader.Create("IXSCP.xaml");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);
            XamlObjectWriter objWriter = new XamlObjectWriter(xtr.SchemaContext);
            XamlServices.Transform(xtr, objWriter);
            object root = objWriter.Result;
            CustomRootWithCollection rootElement = (CustomRootWithCollection) root;
            int count = 0;
            foreach (object obj in rootElement.Content)
            {
                count++;
                Custom_IXSCPObject item = (Custom_IXSCPObject) obj;
                if (!Object.ReferenceEquals(xtr.SchemaContext, item.Content.SchemaContext))
                {
                    GlobalLog.LogEvidence("The contexts were not reference equals. Item: " + count);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }

            if (count != expectedChildrenCount)
            {
                GlobalLog.LogEvidence("Encountered " + count + " children, expected 2");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                TestLog.Current.Result = TestResult.Pass;
            }
        }
    }
}

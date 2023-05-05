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
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.SdxCore
{
    /// <summary>
    /// Class for XamlTypeTest
    /// </summary>
    public static class XamlTypeTest
    {
        /// <summary>
        /// Test method for XamlType
        /// Load XamlType, serializes, and compares with pre-serialized file;
        /// Requirements: TestParams format: prefix,FullTypeName
        ///                 FullTypeName.xml must be part of the support files
        /// </summary>
        public static void RunTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            Dictionary<string, string> prefixToNSMap = SetupPrefixToNamespaceMap();
            string[] args = ParseArgs();
            XamlObjectSerializer xos = new XamlObjectSerializer();
            XamlSchemaContext xsc = new XamlSchemaContext();
            XamlType xamlType = xsc.GetXamlType(prefixToNSMap[args[0]], args[1]);
            if (xamlType == null)
            {
                GlobalLog.LogEvidence("XamlType creation failed for: " + args[1]);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }
            //// Serialize the XamlType
            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlWriter = new XmlTextWriter(stringWriter);
            xmlWriter.Formatting = Formatting.Indented;
            xos.Serialize(xmlWriter, xamlType, true);
            xmlWriter.Flush();
            stringWriter.Flush();
            string resultString = stringWriter.ToString();
            resultString = resultString.Replace('`', '_'); // Generic types are returned with "`1" as part of their name, this is illegal in XML
            //// Load the output into an XmlDocument
            XmlDocument resultDoc = new XmlDocument();
            resultDoc.LoadXml(resultString);
            //// Load the pre-serialized XamlType
            XmlDocument originalDoc = new XmlDocument();
            originalDoc.Load(args[2]);

            XmlCompareResult result = XamlComparer.Compare(originalDoc, resultDoc, null);
            if (result.Result == CompareResult.Different)
            {
                GlobalLog.LogEvidence("The Generated xml tree did not match the loaded one. Saving file");
                XmlTextWriter writer = new XmlTextWriter("Output_" + args[2], Encoding.UTF8);
                writer.Formatting = Formatting.Indented;
                resultDoc.Save(writer);
                writer.Flush();
                writer.Close();
                GlobalLog.LogFile("Output_" + args[2]);
                GlobalLog.LogFile(args[2]);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogStatus("Trees matched");
                TestLog.Current.Result = TestResult.Pass;
            }
        }

        /// <summary>
        /// Sets up the prefix to namespace map.
        /// </summary>
        /// <returns> Dictionary prefixToNS</returns>
        private static Dictionary<string, string> SetupPrefixToNamespaceMap()
        {
            Dictionary<string, string> prefixToNSMap = new Dictionary<string, string>();

            string xNS = "clr-namespace:System;assembly=mscorlib";
            prefixToNSMap.Add("ms", xNS);
            xNS = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
            prefixToNSMap.Add(string.Empty, xNS);
            xNS = "http://schemas.microsoft.com/winfx/2006/xaml";
            prefixToNSMap.Add("x", xNS);
            xNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";
            prefixToNSMap.Add("xc", xNS);
            xNS = "clr-namespace:Microsoft.Test.Xaml.Types.Attributes;assembly=XamlClrTypes";
            prefixToNSMap.Add("xca", xNS);
            xNS = "clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlWpfTypes";
            prefixToNSMap.Add("xw", xNS);
            xNS = "clr-namespace:Microsoft.Test.Xaml.Types.Attributes;assembly=XamlWpfTypes";
            prefixToNSMap.Add("xwa", xNS);
            return prefixToNSMap;
        }

        /// <summary>
        /// Parses the args.
        /// </summary>
        /// <returns> string arr of arguments </returns>
        private static string[] ParseArgs()
        {
            string input = DriverState.DriverParameters["TestParams"];
            string[] array1 = input.Split(new string[1] { "," }, StringSplitOptions.None);
            if (array1.Length < 2)
            {
                throw new Exception("Invalid TestParams.  Must be: prefix,fullTypeName");
            }

            string[] array2 = array1[1].Split(new string[1] { "." }, StringSplitOptions.None);
            string suffix = array1.Length == 3 ? "." + array1[2] + ".xml" : ".xml";
            return new string[3] { array1[0], array2[array2.Length - 1], array1[1] + suffix };
        }
    }
}

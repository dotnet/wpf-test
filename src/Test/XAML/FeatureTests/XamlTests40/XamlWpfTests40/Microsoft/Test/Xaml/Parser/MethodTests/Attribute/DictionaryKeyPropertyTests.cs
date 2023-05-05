// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Tests for DictionaryKeyProperty
    /// </summary>
    public static class DictionaryKeyPropertyTests
    {
        /// <summary> current Type </summary>
        private static Type s_currentTypeVar;

        /// <summary> entry Count </summary>
        private static int s_entryCount;

        /// <summary> Dictionary of Types </summary>
        private static Dictionary<object, Type> s_typeDic;

        /// <summary>
        /// Runs the test.
        /// </summary>
        public static void RunTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;
            s_currentTypeVar = null;
            string xamlFile = DriverState.DriverParameters["TestParams"];

            XmlReader xmlReader = XmlReader.Create(xamlFile);
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(ip.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteMemberDelegate = CheckWriteMember;
            ip.WriteValueDelegate = CheckWriteValue;
            s_typeDic = new Dictionary<object, Type>();
            s_entryCount = 0;

            XamlServices.Transform(xtr, ip);
            object root = ip.Result;
            Custom_IDictionaryHost host = (Custom_IDictionaryHost) root;
            if (s_entryCount != host.Dictionary.Count)
            {
                GlobalLog.LogEvidence("_entryCount != host.Dictionary.Count");
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                GlobalLog.LogStatus("Entry count matched");
            }

            // This loop verifies that the value of the DKPA returns the correct object, only works for strings
            foreach (KeyValuePair<object, Type> kvp in s_typeDic) 
            {
                GlobalLog.LogStatus(kvp.Key + " - " + kvp.Value);
                if (host.Dictionary[kvp.Key].GetType().Equals(kvp.Value))
                {
                    GlobalLog.LogStatus("Confirmed..");
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("Failed:");
                    GlobalLog.LogEvidence("host.Dictionary[kvp.Key] = " + host.Dictionary[kvp.Key]);
                    GlobalLog.LogEvidence("kvp.Value = " + kvp.Value);
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
        }

        /// <summary>
        /// Checks the write member.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteMember(XamlMember property, XamlSchemaContext context)
        {
            GlobalLog.LogStatus("Checking: " + property.Name);
            if (property.IsDirective)
            {
                return;
            }

            if (!property.DeclaringType.Name.StartsWith("DKPA"))
            {
                return;
            }

            s_entryCount++;
            Type currentType = property.DeclaringType.UnderlyingType;
            object[] objAttrs = currentType.GetCustomAttributes(typeof(DictionaryKeyPropertyAttribute), true);

            if (objAttrs.Length == 0)
            {
                GlobalLog.LogEvidence("No DictionaryKeyProperty was found for type: " + currentType.Name);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            DictionaryKeyPropertyAttribute dkpa = objAttrs[0] as DictionaryKeyPropertyAttribute;
            if (dkpa.Name == property.Name)
            {
                s_currentTypeVar = currentType;
            }
        }

        /// <summary>
        /// Checks the write value.
        /// </summary>
        /// <param name="value">The value.</param>
        public static void CheckWriteValue(object value)
        {
            if (s_currentTypeVar != null)
            {
                s_typeDic.Add(value, s_currentTypeVar);
            }

            s_currentTypeVar = null;
        }
    }
}

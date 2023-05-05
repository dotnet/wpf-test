// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Reflection;
using System.Windows;
using System.Windows.Markup;
using System.Xaml;
using System.Xml;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Tests for AmbientAttribute
    /// </summary>
    public static class AmbientAttributeTests
    {
        /// <summary> ambient Count </summary>
        private static int s_ambientCount;

        /// <summary>Xaml Reader </summary>
        private static System.Xaml.XamlReader s_reader;

        /// <summary> List of Types</summary>
        private static List<string> s_typeList;

        /// <summary>
        /// Entry point for AmbientPropertyAttribute tests
        /// </summary>
        public static void RunTest()
        {
            s_ambientCount = 0;
            FrameworkElement fe = new FrameworkElement();
            string xamlFile = DriverState.DriverParameters["TestParams"];
            s_typeList = new List<string>();

            XmlReader xmlReader = XmlReader.Create(xamlFile);
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);
            s_reader = xtr;
            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            ip.Writer = new XamlObjectWriter(ip.SchemaContext);

            fe = null;
            ip.WriteMemberDelegate = CheckWriteMember;
            GlobalLog.LogStatus("Loading: " + xamlFile);
            XamlServices.Transform(xtr, ip);
            if (s_ambientCount == 0 && DriverState.TestName != "AmbientAttachedTest")
            {
                GlobalLog.LogEvidence("No Ambient Properties were detected, failing test.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Runs the exception test.
        /// </summary>
        public static void RunExceptionTest()
        {
            s_ambientCount = 0;
            FrameworkElement fe = new FrameworkElement();
            string xamlFile = DriverState.DriverParameters["TestParams"];
            s_typeList = new List<string>();

            XmlReader xmlReader = XmlReader.Create(xamlFile);
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);
            s_reader = xtr;
            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            ip.Writer = new XamlObjectWriter(ip.SchemaContext);

            fe = null;
            ip.WriteMemberDelegate = CheckWriteMember;
            GlobalLog.LogStatus("Loading: " + xamlFile);
            GlobalLog.LogStatus("Expecting exception...");
            try
            {
                XamlServices.Transform(xtr, ip);
            }
            catch (XamlSchemaException)
            {
                GlobalLog.LogStatus("Exception caught..");
                TestLog.Current.Result = TestResult.Pass;
                return;
            }

            GlobalLog.LogEvidence("No exception caught, failing test.");
            TestLog.Current.Result = TestResult.Fail;
        }

        /// <summary>
        /// Method for checking each StartProperty for ambient
        /// </summary>
        /// <param name="property">The property.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteMember(XamlMember property, XamlSchemaContext context)
        {
            if (property == null)
            {
                throw new ArgumentNullException("Property was null");
            }

            if (context == null)
            {
                throw new ArgumentNullException("Context was null");
            }

            if (XamlTestHelper.IsImplicit(property))
            {
                GlobalLog.LogStatus("Ignoring implicit property: " + property.Name);
                return;
            }

            if (property.IsDirective)
            {
                GlobalLog.LogStatus("Ignoring directive: " + property.Name);
                return;
            }
            else if (property.IsUnknown)
            {
                GlobalLog.LogStatus("Ignoring unknown property: " + property.Name);
                return;
            }

            Type currentType = property.DeclaringType.UnderlyingType;
            if (currentType == null)
            {
                GlobalLog.LogEvidence("UnderlyingType returned null for XamlType: " + property.DeclaringType.Name);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            PropertyInfo currentProp = currentType.GetProperty(property.Name);
            if (currentProp == null)
            {
                GlobalLog.LogEvidence(string.Format("Unable to find property: {0} on type: {1}", property.Name, currentType.Name));
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            object[] objAttrs = currentProp.GetCustomAttributes(typeof(AmbientAttribute), true);
            s_ambientCount += objAttrs.Length;
            if (objAttrs.Length != 0)
            {
                GlobalLog.LogStatus("Verifying Ambient Property:{0}.{1} ", property.DeclaringType, property.Name);
                if (IsAmbient(property))
                {
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    TestLog.Current.Result = TestResult.Fail;
                    GlobalLog.LogEvidence("Property: {0} was not Ambient", property.Name);
                }
            }
        }

        /// <summary>
        /// Determines whether the specified property is ambient.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns>
        /// <c>true</c> if the specified property is ambient; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsAmbient(XamlMember property)
        {
            return property.IsAmbient;
        }
    }
}

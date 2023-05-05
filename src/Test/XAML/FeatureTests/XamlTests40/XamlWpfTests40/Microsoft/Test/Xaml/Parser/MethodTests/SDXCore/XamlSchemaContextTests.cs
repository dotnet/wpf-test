// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Xaml;
using System.Xaml.Schema;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.SdxCore
{
    /// <summary>
    /// API tests targeting the System.Xaml.XamlSchemaContext class
    /// </summary>
    public static class XamlSchemaContextTests
    {
        /// <summary>
        /// Scenario: clr-ns no Ref assemblies  Verification: XamlNS created, retrieved custom type
        /// </summary>
        public static void GetXamlNamespaceTestNoRA()
        {
            XamlSchemaContext xSC = new XamlSchemaContext();
            string xNS = @"clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlWpfTypes";

            XamlType xamlType = xSC.GetXamlType(xNS, "Custom_Clr");
            if (xamlType != null)
            {
                if (xamlType.Name == "Custom_Clr")
                {
                    GlobalLog.LogStatus("Custom_Clr type found, test successful");
                    TestLog.Current.Result = TestResult.Pass;
                    return;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Custom_Clr was not found..");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Scenario: WPF Uri no Ref assemblies  Verification: XamlNS created, retrieved custom type
        /// </summary>
        public static void GetXamlNamespaceTestNoRA_URI()
        {
            FrameworkElement fe = new FrameworkElement(); // Hack: loads WPF assemblies
            fe = null;
            XamlSchemaContext xSC = new XamlSchemaContext();
            string xNS = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            XamlType xamlType = xSC.GetXamlType(xNS, "Button");
            if (xamlType != null)
            {
                if (xamlType.Name == "Button")
                {
                    GlobalLog.LogStatus("Button type found, test successful");
                    TestLog.Current.Result = TestResult.Pass;
                    return;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Button was not found..");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Scenario: clr-ns with Ref assemblies  Verification: XamlNS created, retrieved custom type
        /// </summary>
        public static void GetXamlNamespaceTestWithRA()
        {
            List<Assembly> refAssemblies = new List<Assembly>();
            refAssemblies.Add(Assembly.Load(@"XamlWpfTypes"));

            XamlSchemaContext xSC = new XamlSchemaContext(refAssemblies);
            string xNS = @"clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlWpfTypes";

            XamlType xamlType = xSC.GetXamlType(xNS, "Custom_Clr");
            if (xamlType != null)
            {
                if (xamlType.Name == "Custom_Clr")
                {
                    GlobalLog.LogStatus("Custom_Clr type found, test successful");
                    TestLog.Current.Result = TestResult.Pass;
                    return;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Custom_Clr was not found..");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Scenario: WPF Uri with Ref assemblies  Verification: XamlNS created, retrieved custom type
        /// </summary>
        public static void GetXamlNamespaceTestWithRA_URI()
        {
            List<Assembly> refAssemblies = new List<Assembly>();
            refAssemblies.Add(Assembly.Load(@"PresentationCore, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35"));
            refAssemblies.Add(Assembly.Load(@"PresentationFramework, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL"));
            refAssemblies.Add(Assembly.Load(@"WindowsBase, Version=3.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35, processorArchitecture=MSIL"));

            XamlSchemaContext xSC = new XamlSchemaContext(refAssemblies);
            string xNS = @"http://schemas.microsoft.com/winfx/2006/xaml/presentation";

            XamlType xamlType = xSC.GetXamlType(xNS, "Button"); // System.Windows.Controls.
            if (xamlType != null)
            {
                if (xamlType.Name == "Button")
                {
                    GlobalLog.LogStatus("Button type found, test successful");
                    TestLog.Current.Result = TestResult.Pass;
                    return;
                }
            }
            else
            {
                GlobalLog.LogEvidence("Button was not found..");
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Scenario: Custom type  Verification: Custom type retrieved, refrence equal to type retrieved from Xaml NS
        /// </summary>
        public static void GetXamlTypeTest()
        {
            List<Assembly> refAssemblies = new List<Assembly>();
            refAssemblies.Add(Assembly.Load(@"XamlWpfTypes"));
            XamlSchemaContext xSC = new XamlSchemaContext(refAssemblies);
            string xNS = @"clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlWpfTypes";

            XamlType getXamlType = xSC.GetXamlType(typeof(Custom_Clr));
            XamlType findXamlType = xSC.GetXamlType(xNS, "Custom_Clr");

            if (getXamlType == null)
            {
                GlobalLog.LogEvidence("XamlSchemaContext.GetXamlType failed to return a XamlType for Custom_Clr");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            if (getXamlType.Name != "Custom_Clr")
            {
                GlobalLog.LogEvidence("GetXamlType did not return the proper XamlType.  Name was: " + getXamlType.Name);
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            if (!Object.ReferenceEquals(getXamlType, findXamlType))
            {
                GlobalLog.LogEvidence("The XamlType returned by GetXamlType was not reference equals to the XamlType returned by FindXamlType");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Scenario: Type  Verification: proper type returned when clr type exisits, null when it doesn't
        /// </summary>
        public static void GetClrTypeTest()
        {
            List<Assembly> refAssemblies = new List<Assembly>();
            refAssemblies.Add(Assembly.Load(@"XamlWpfTypes"));
            XamlSchemaContext xSC = new XamlSchemaContext(refAssemblies);
            string xNS = @"clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlWpfTypes";

            XamlType xamlType = xSC.GetXamlType(xNS, "Custom_Clr");
            Type type = xamlType.UnderlyingType;

            if (type != typeof(Custom_Clr))
            {
                GlobalLog.LogEvidence("GetClrType did not return the proper type for Custom_Clr");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Scenario: TextSytnax  Verification: proper type returned when clr type exisits, null when it doesn't
        /// </summary>
        public static void GetClrTypeTextSyntaxTest()
        {
            List<Assembly> refAssemblies = new List<Assembly>();
            refAssemblies.Add(Assembly.Load(@"XamlClrTypes"));
            XamlSchemaContext xSC = new XamlSchemaContext(refAssemblies);
            string xNS = @"clr-namespace:Microsoft.Test.Xaml.Types;assembly=XamlClrTypes";

            XamlType findXamlType = xSC.GetXamlType(xNS, "CustomPropertyType");
            XamlValueConverter<TypeConverter> textSyntax = findXamlType.TypeConverter;
            Type type = textSyntax.ConverterType;
            if (type != typeof(CustomPropertyTypeConverter))
            {
                GlobalLog.LogEvidence("XamlValueConverter.ConverterType did not return the proper type for CustomPropertyTypeConverter");
                TestLog.Current.Result = TestResult.Fail;
                return;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Scenario: pass null to XamlSchemaContext, then call TryGetCompatibleXamlNamespace on the context
        /// Regression test (1st scenario)
        /// </summary>
        public static void TryGetCompatibleXamlNamespaceWithNull()
        {
            try
            {
                string s;
                XamlSchemaContext context = new XamlSchemaContext((XamlSchemaContextSettings)null);
                context.TryGetCompatibleXamlNamespace(null, out s);
            }
            catch (ArgumentNullException argNullEx)
            {
                Assert.IsTrue(argNullEx.ParamName.Contains("xamlNamespace"));
            }
            catch (Exception ex)
            {
                GlobalLog.LogEvidence("FAIL: The wrong exception was thrown.\nExpected: ArgumentNullException / Actual: " + ex);
                TestLog.Current.Result = TestResult.Fail;
            }
        }

        /// <summary>
        /// Scenario: pass null to XamlSchemaContextSettings constructor
        /// Regression test (2nd scenario)
        /// </summary>
        public static void XamlSchemaContextSettingsWithNull()
        {
            XamlSchemaContextSettings xscs = new XamlSchemaContextSettings(null);
            
            if (xscs.FullyQualifyAssemblyNamesInClrNamespaces == false)
            {
                if (xscs.SupportMarkupExtensionsWithDuplicateArity == false)
                {
                    TestLog.Current.Result = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogEvidence("FAIL: SupportMarkupExtensionsWithDuplicateArity returned true.");
                    TestLog.Current.Result = TestResult.Fail;
                }
            }
            else
            {
                GlobalLog.LogEvidence("FAIL: FullyQualifyAssemblyNamesInClrNamespaces returned true.");
                TestLog.Current.Result = TestResult.Fail;
            }
        }
    }
}

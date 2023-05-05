// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Reflection;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Tests for UsableDuringInitialization Attribute
    /// </summary>
    public static class UsableDuringInitializationAttributeTests
    {
        /// <summary>
        /// Class Constructor 
        /// </summary>
        public static void UsableDuringInitializationObjectTest()
        {
            XmlReader xmlReader = XmlReader.Create("UsableDuringInitialization_Objects.xaml");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(ip.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UsableDuringInitialization_Objects.xaml");
            XamlServices.Transform(xtr, ip);
        }

        /// <summary>
        /// UsableDuringInitializationFE Test .
        /// </summary>
        public static void UsableDuringInitializationFETest()
        {
            XmlReader xmlReader = XmlReader.Create("UsableDuringInitialization_FrameworkElements.xaml");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(ip.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UsableDuringInitialization_FrameworkElements.xaml");
            XamlServices.Transform(xtr, ip);
        }

        /// <summary>
        /// UsableDuringInitializationObjectWCollection Test
        /// </summary>
        public static void UsableDuringInitializationObjectWCollectionTest()
        {
            XmlReader xmlReader = XmlReader.Create("UsableDuringInitialization_ObjectsWithCollection.xaml");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(ip.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UsableDuringInitialization_ObjectsWithCollection.xaml");
            XamlServices.Transform(xtr, ip);
        }

        /// <summary>
        /// UsableDuringInitializationFewCollection Test
        /// </summary>
        public static void UsableDuringInitializationFewCollectionTest()
        {
            XmlReader xmlReader = XmlReader.Create("UsableDuringInitialization_FrameworkElementsWithCollection.xaml");
            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(ip.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UsableDuringInitialization_FrameworkElementsWithCollection.xaml");
            XamlServices.Transform(xtr, ip);
        }

        /// <summary>
        /// Checks the write object.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="context">The context.</param>
        public static void CheckWriteObject(XamlType xamlType, XamlSchemaContext context)
        {
            switch (xamlType.Name)
            {
                // CustomBTDObject
                case "CustomBTDObject":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObject was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObject_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObject_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObject_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDObject_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObject_SubBTDfalse_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObject_SubBTDfalse_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObject_SubBTD_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDObject_SubBTD_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;

                // CustomBTDFE
                case "CustomBTDFE":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFE was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFE_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFE_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFE_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDFE_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFE_SubBTDfalse_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFE_SubBTDfalse_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFE_SubBTD_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDFE_SubBTD_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;

                // CustomBTDObjectWCollection
                case "CustomBTDObjectWCollection":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObjectWCollection was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObjectWCollection_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObjectWCollection_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObjectWCollection_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDObjectWCollection_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObjectWCollection_SubBTDfalse_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDObjectWCollection_SubBTDfalse_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDObjectWCollection_SubBTD_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDObjectWCollection_SubBTD_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;

                // CustomBTDFEWCollection
                case "CustomBTDFEWCollection":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFEWCollection was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFEWCollection_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFEWCollection_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFEWCollection_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDFEWCollection_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFEWCollection_SubBTDfalse_BTD":
                    if (IsUsableDuringInitialization(xamlType) == false)
                    {
                        GlobalLog.LogEvidence("CustomBTDFEWCollection_SubBTDfalse_BTD was not UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "CustomBTDFEWCollection_SubBTD_BTDfalse":
                    if (IsUsableDuringInitialization(xamlType))
                    {
                        GlobalLog.LogEvidence("CustomBTDFEWCollection_SubBTD_BTDfalse was UsableDuringInitialization");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Determines whether xamlType is usable during initialization.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <returns>
        /// <c>true</c> if [is usable during initialization] [the specified xaml type]; otherwise, <c>false</c>.
        /// </returns>
        private static bool IsUsableDuringInitialization(XamlType xamlType)
        {
            object result = typeof(XamlType).GetProperty("IsUsableDuringInitialization", BindingFlags.Instance | BindingFlags.NonPublic).GetValue(xamlType, null);
            return (bool)result;
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;
using System.Xaml;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Xaml.Types.Attributes;
using Microsoft.Test.Xaml.Utilities;

namespace Microsoft.Test.Xaml.Parser.MethodTests.Attribute
{
    /// <summary>
    /// Tests for UidProperty Attribute
    /// </summary>
    public static class UidPropertyAttributeTests
    {
        /// <summary>
        /// Uids property object test.
        /// </summary>
        public static void UidPropertyObjectTest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            XmlReader xmlReader = XmlReader.Create("UidProperty_Objects.xaml");

            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(xtr.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UidProperty_Objects.xaml");
            XamlServices.Transform(xtr, ip);

            CustomUDIObjectWCollection rootObject = ((CustomUDIObjectWCollection) objWriter.Result);

            UPAObject upaObject = ((UPAObject) rootObject.Children[0]);
            if (((string) upaObject.ObjectProperty) != "u1")
            {
                GlobalLog.LogEvidence("UPAObject.ObjectProperty was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            UPAString upaString = ((UPAString) rootObject.Children[1]);
            if (upaString.Name != "u2")
            {
                GlobalLog.LogEvidence("UPAString.Name was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            // 
            /*
            UPACollectionRO upaCollectionRO = ((UPACollectionRO)rootObject.Children[2]);
            if (upaCollectionRO.ObjectCollection != "u3")
            {
                GlobalLog.LogEvidence("UPACollectionRO.ObjectCollection was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            UPACollectionRW upaCollectionRW = ((UPACollectionRW)rootObject.Children[3]);
            if (upaCollectionRW.ObjectCollection != "u4")
            {
                GlobalLog.LogEvidence("UPACollectionRW.ObjectCollection was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }
            */ 
        }

        /// <summary>
        /// Uids property FE test.
        /// </summary>
        public static void UidPropertyFETest()
        {
            FrameworkElement fe = new FrameworkElement();
            fe = null;

            XmlReader xmlReader = XmlReader.Create("UidProperty_FrameworkElements.xaml");

            System.Xaml.XamlXmlReader xtr = new System.Xaml.XamlXmlReader(xmlReader);

            InfosetProcessor ip = new InfosetProcessor(xtr.SchemaContext);
            XamlObjectWriter objWriter = new XamlObjectWriter(xtr.SchemaContext);
            ip.Writer = objWriter;

            ip.WriteObjectDelegate = CheckWriteObject;
            GlobalLog.LogStatus("Loading: UidProperty_FrameworkElements.xaml");
            XamlServices.Transform(xtr, ip);

            Page page = ((Page) objWriter.Result);
            if (page.Uid != "page")
            {
                GlobalLog.LogEvidence("page.Uid was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            StackPanel stackPanel = ((StackPanel) page.Content);
            if (stackPanel.Uid != "stackpanel")
            {
                GlobalLog.LogEvidence("stackpanel.Uid was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            UIElement uielement = stackPanel.Children[0];
            if (uielement.Uid != "uielement")
            {
                GlobalLog.LogEvidence("uielement.Uid was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }

            Button button = ((Button) stackPanel.Children[1]);
            if (button.Uid != "button")
            {
                GlobalLog.LogEvidence("button.Uid was not set to the content of x:Uid");
                TestLog.Current.Result = TestResult.Fail;
            }
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
                case "UPAObject":
                    if (GetUidProperty(xamlType, context).Name != "ObjectProperty")
                    {
                        GlobalLog.LogEvidence("UPAObject did not have a UidProperty of ObjectProperty");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "UPAString":
                    if (GetUidProperty(xamlType, context).Name != "Name")
                    {
                        GlobalLog.LogEvidence("UPAObject did not have a UidProperty of Name");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "UPACollectionRO":
                    if (GetUidProperty(xamlType, context).Name != "ObjectCollection")
                    {
                        GlobalLog.LogEvidence("UPACollectionRO did not have a UidProperty of ObjectCollection");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "UPACollectionRW":
                    if (GetUidProperty(xamlType, context).Name != "ObjectCollection")
                    {
                        GlobalLog.LogEvidence("UPACollectionRW did not have a UidProperty of ObjectCollection");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "UIElement":
                    if (GetUidProperty(xamlType, context).Name != "Uid")
                    {
                        GlobalLog.LogEvidence("UIElement did not have a UidProperty of Uid");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "Button":
                    if (GetUidProperty(xamlType, context).Name != "Uid")
                    {
                        GlobalLog.LogEvidence("Button did not have a UidProperty of Uid");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
                case "DependencyObject":
                    if (GetUidProperty(xamlType, context) != null)
                    {
                        GlobalLog.LogEvidence("DependencyObject did not have a UidProperty of null");
                        TestLog.Current.Result = TestResult.Fail;
                    }

                    break;
            }

            TestLog.Current.Result = TestResult.Pass;
        }

        /// <summary>
        /// Gets the uid property.
        /// </summary>
        /// <param name="xamlType">Type of the xaml.</param>
        /// <param name="schemaContext">The schema context.</param>
        /// <returns> XamlMember object </returns>
        private static XamlMember GetUidProperty(XamlType xamlType, XamlSchemaContext schemaContext)
        {
            XamlDirective uidDirective = schemaContext.GetXamlDirective("http://schemas.microsoft.com/winfx/2006/xaml", "Uid");
            return xamlType.GetAliasedProperty(uidDirective);
        }
    }
}

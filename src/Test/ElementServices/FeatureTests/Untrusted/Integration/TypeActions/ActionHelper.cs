// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Xml;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Windows;


namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /// <summary>
    /// Implements various routines for testing Avalon types.
    ///</summary>
    public class ActionHelper
    {
        #region Private Data
        private static object                               s_syncObject                  = new object();
        private static XmlNamespaceManager                  s_nsmgr                       = null;
        private static Assembly[]                           s_assemblies                  = null;
        private static Dictionary<string, PropertyToIgnore> s_propertiesToSkip            = null;
        private static readonly string                      s_avalonUri                   = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private static XamlTestDocument                     s_testDoc                     = null;
        private static string                               s_emptyFileName               = null;
        private static string                               s_xamlFileName                = "__ActionForType.xaml";
        public  static string                               snippetsFileName            = "IntegrationXamlSnippets.xaml";
        public  static string                               propertiesToSkipFileName    = "ActionForType_PropertiesToSkip.xml";
        public  static string                               baseElementName             = "elementWithInitialValues";

        /// <summary>
        /// Name of xaml file to be generated.
        /// </summary>
        public static string XamlFileName
        {
            get
            {
                return s_xamlFileName;
            }
            set
            {
                s_xamlFileName = value;
            }
        }

        /// <summary>
        /// Name of xaml file to be generated.
        /// </summary>
        public static string PropertiesToSkipFileName
        {
            get
            {
                return propertiesToSkipFileName;
            }
            set
            {
                propertiesToSkipFileName = value;
            }
        }

        /// <summary>
        /// Name of snippets file used for xaml generation.
        /// </summary>
        public static string SnippetsFileName
        {
            set
            {
                if (!String.Equals(snippetsFileName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    snippetsFileName = value;
                    s_testDoc = null;
                }
            }
        }

        /// <summary>
        /// Name of template file used for xaml generation.
        /// </summary>
        public static string EmptyFileName
        {
            set
            {
                if (!String.Equals(s_emptyFileName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    s_emptyFileName = value;
                    s_testDoc = null;
                }
            }
        }

        /// <summary>
        /// Returns the XamlTestDocument for the xaml template and snippets.
        /// </summary>
        public static XamlTestDocument TestDocument
        {
            get
            {
                if (s_testDoc == null)
                {
                    s_testDoc = new XamlTestDocument(s_emptyFileName, snippetsFileName);
                }

                return s_testDoc;
            }
        }
       #endregion


        #region Static Members
        /******************************************************************************
        * Function:          GetType
        ******************************************************************************/
        /// <summary>
        /// Finds and returns the Type for the given type name.
        /// Throws if type isn't found.
        /// </summary>
        public static Type GetType(string element, bool throwOnError)
        {
            if (s_assemblies == null)
            {
                s_assemblies = new Assembly[]
                {
                    typeof(FrameworkElement).Assembly,
                    typeof(UIElement).Assembly,
                    typeof(DependencyObject).Assembly
                };
            }

            Type type = null;

            for (int i = 0; type == null && i < s_assemblies.Length; i++)
            {
                type = s_assemblies[i].GetType(element, false, false);
            }

            if (type == null && throwOnError)
            {
                throw new Microsoft.Test.TestValidationException("Could not find type '" + element + "'.");
            }

            return type;
        }

        /******************************************************************************
        * Function:          GetFullName
        ******************************************************************************/
        /// <summary>
        /// Obtains the full name for the given element.
        /// </summary>
        /// <param name="typeName">The type to be checked.</param>
        ///<returns>A full name</returns>
        public static string GetFullName(string elementName)
        {
            string[] nameSpaces = new string[] {"System.Windows", "System.Windows.Controls", "System.Windows.Controls.Primitives", "System.Windows.Documents", "System.Windows.Shapes"};
            string fullName;
            Type type = null;

            foreach (string prefix in nameSpaces)
            {
                fullName = prefix + "." + elementName;
                type = ActionHelper.GetType(fullName, false);
                if (type != null)
                    break;
            }

            if (type == null)
            {
                throw new Microsoft.Test.TestValidationException("ERROR: Type not found: " + elementName);
            }

            return type.FullName;
        }
        
        /******************************************************************************
        * Function:          GetPropertyValue
        ******************************************************************************/
        /// <summary>
        /// Returns a value to use for the property in xaml. 
        /// Custom values may be defined in a xaml snippets file. 
        /// The value returned is the first Value in xaml snippets file. 
        /// The property's default value will be returned if no custom value is available.
        /// </summary>
        public static object GetPropertyValue(XamlTestDocument doc, string element, DependencyPropertyDescriptor dpd)
        {
            object value = GetPropertyValue(doc, element, dpd, 0);

            // Use default value if a match wasn't found in the snippets file.
            // Otherwise, when a match is found, return the xml snippet root if
            // it's a complex value.  Return the trimmed string if it's a simple value.
            if (value == null)
            {
                value = dpd.Metadata.DefaultValue;
            }

            return value;
        }

        /******************************************************************************
        * Function:          GetPropertyValue
        ******************************************************************************/
        /// <summary>
        /// Returns a value to use for the property in xaml. 
        /// Custom values may be defined in a xaml snippets file.
        /// Return null if the property has not been defined. 
        /// </summary>
        public static object GetPropertyValue(XamlTestDocument doc, string element, DependencyPropertyDescriptor dpd, int index)
        {
            object value = null;
            DependencyProperty dp = dpd.DependencyProperty;
            string propName = dp.Name;

            // ValueType and ValueOwnerType and PropertyName
            XmlElement propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and @ValueOwnerType='" + element + "' and @PropertyName='" + propName + "']", false);

            // ValueType and PropertyName
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and not(@ValueOwnerType) and @PropertyName='" + propName + "']", false);

            // ValueType and ValueOwnerType
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and @ValueOwnerType='" + element + "' and not(@PropertyName)]", false);

            // ValueType
            if (propertyTestValueElement == null)
                propertyTestValueElement = doc.GetSnippetByXPath("//av:PropertyTestValue[@ValueType='" + dp.PropertyType.Name + "' and not(@ValueOwnerType) and not(@PropertyName)]", false);

            // Use default value if a match wasn't found in the snippets file.
            // Otherwise, when a match is found, return the xml snippet root if
            // it's a complex value.  Return the trimmed string if it's a simple value.
            if (propertyTestValueElement != null)
            {
                XmlNodeList valueNodes = propertyTestValueElement.ChildNodes;
                if (valueNodes.Count < index + 1) return null;
                XmlElement valueElement = valueNodes[index] as XmlElement;
                if (String.Equals(propertyTestValueElement.GetAttribute("ValueFormat"), "Simple", StringComparison.InvariantCulture))
                {
                    value = valueElement.InnerText.Trim();
                }
                else
                {
                    value = valueElement.FirstChild;
                }
            }
            return value;
        }

        /******************************************************************************
        * Function:          PropertiesToSkip
        ******************************************************************************/
        /// <summary>
        /// Properties to ignore in xaml generation and verification.
        /// </summary>
        /// <param name="fileName">The name of the file containing properties to skip.</param>
        /// <returns>Dictionary of PropertyToIgnore items indexed by property name.</returns>
        public static Dictionary<string, PropertyToIgnore> PropertiesToSkip
        {
            get
            {
                if (s_propertiesToSkip == null)
                {
                    s_propertiesToSkip = TreeComparer.ReadSkipProperties(propertiesToSkipFileName);
                }

                return s_propertiesToSkip;
            }
        }

        /******************************************************************************
        * Function:          GetNamespace
        ******************************************************************************/
        /// <summary>
        /// Gets the valid xml namespace for the given type.
        /// </summary>
        /// <param name="type">The type to be checked.</param>
        ///<returns>A Namespace string</returns>
        public static string GetNamespace(Type type)
        {
            string xmlns = s_avalonUri;

            if (!IsInAvalonNamespace(type))
            {
                xmlns =
                    "clr-namespace:" +
                    type.Namespace +
                    ";assembly=" +
                    type.Assembly.GetName().Name;
            }

            return xmlns;
        }

        /******************************************************************************
        * Function:          XmlNamespaceManager
        ******************************************************************************/
        /// <summary>
        /// Returns the common xmlns manager used for xaml generation in action handlers.
        /// </summary>
        public static XmlNamespaceManager NamespaceManager
        {
            get
            {
                if (s_nsmgr != null)
                {
                    return s_nsmgr;
                }

                lock (s_syncObject)
                {
                    if (s_nsmgr == null)
                    {
                        // Construct the XmlNamespaceManager used for xpath queries later.
                        NameTable ntable = new NameTable();

                        s_nsmgr = new XmlNamespaceManager(ntable);
                        s_nsmgr.AddNamespace("av", s_avalonUri);
                        s_nsmgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
                        s_nsmgr.AddNamespace("cmn", "clr-namespace:Microsoft.Test.Serialization.CustomElements;assembly=TestRuntime");
                    }
                }

                return s_nsmgr;
            }
        }

        /******************************************************************************
        * Function:          IsStylableProperty
        ******************************************************************************/
        // Checks if a property is stylable.
        // Includes filters for certain properties, e.g. Name.
        public static bool IsStylableProperty(DependencyPropertyDescriptor dpd)
        {
            bool isStylable = dpd != null 
                              && !dpd.IsReadOnly
                              && dpd.DependencyProperty != FrameworkElement.StyleProperty
                              && dpd.DependencyProperty != FrameworkElement.NameProperty;

            return isStylable;
        }

        /******************************************************************************
        * Function:          InsertStyleTriggersNode
        ******************************************************************************/
        /// <summary>
        /// Insert two layers like
        /// <Element.Style>
        ///     <Style>
        ///     </Style>
        /// </Element.Style>
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementNode">The node under which to insert this.</param>
        /// <returns>inner Style node</returns>
        public static XmlElement InsertStyleTriggersNode(XmlDocument doc, XmlElement elementNode)
        {
            XmlElement styleComplexProp = doc.CreateElement("", elementNode.Name + ".Style", elementNode.NamespaceURI);
            elementNode.InsertAfter(styleComplexProp, null);

            XmlElement styleElement = doc.CreateElement("Style", XamlGenerator.AvalonXmlns);
            // Set TargetType attribute on Style.
            styleElement.SetAttribute("TargetType", "{x:Type " + elementNode.Name + "}");
            styleComplexProp.InsertAfter(styleElement, null);

            XmlElement triggersElement = doc.CreateElement("Style.Triggers", XamlGenerator.AvalonXmlns);
            styleElement.InsertAfter(triggersElement, null);
            return triggersElement;
        }

        /******************************************************************************
        * Function:          GetPropertyTableElementBinding
        ******************************************************************************/
        /// <summary>
        /// Returns list of properties that can be used for ElementBinding on the given type.
        /// </summary>
        public static List<DependencyPropertyDescriptor> GetPropertyTableElementBinding(string typeName)
        {
            Type type = ActionHelper.GetType(typeName, true);

            List<DependencyPropertyDescriptor> properties = new List<DependencyPropertyDescriptor>();

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (String.Equals(prop.Name, "IsItemsHost", StringComparison.InvariantCulture))
                    continue;

                if (ActionHelper.IsStylableProperty(dpd))
                {
                    properties.Add(dpd);
                }
            }

            return properties;
        }

        /******************************************************************************
        * Function:          InsertBindings
        ******************************************************************************/
        public static void InsertBindings(XmlDocument doc, XmlElement element, List<DependencyPropertyDescriptor> properties, bool IsComplexSyntax, string baseElementName)
        {
            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                if (IsComplexSyntax)
                {
                    XmlElement dotProperty = doc.CreateElement("", element.Name + "." + dpd.Name,
                              element.NamespaceURI);
                    element.InsertAfter(dotProperty, null);
                    InsertBindingNode(doc, dotProperty, dpd, baseElementName);
                }
                else
                element.SetAttribute(dpd.Name, "{Binding Path=" + dpd.Name + ",Mode=TwoWay,ElementName=elementWithInitialValues}");
            }
        }

        /******************************************************************************
        * Function:          InsertBindingNode
        ******************************************************************************/
        public static void InsertBindingNode(XmlDocument doc, XmlElement parent, DependencyPropertyDescriptor dpd, string baseElementName)
        {
            XmlElement bindingNode = doc.CreateElement("Binding", XamlGenerator.AvalonXmlns);
            bindingNode.SetAttribute("Path", dpd.Name);
            bindingNode.SetAttribute("Mode", "TwoWay");
            bindingNode.SetAttribute("ElementName", baseElementName);
            parent.InsertAfter(bindingNode, null);
        }

        /******************************************************************************
        * Function:          CompareElements
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        public static void CompareElements(List<DependencyPropertyDescriptor> properties, DependencyObject compareWith, DependencyObject toVerify, Dictionary<string, PropertyToIgnore> skipProps)
        {
            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                if (null != toVerify)
                {
                    GlobalLog.LogStatus("Check property: " + dpd.Name);
                    ActionForTypeHelper.CheckExpectedPropertyValue(toVerify, compareWith, dpd.DependencyProperty, skipProps);
                }
            }
        }

        /******************************************************************************
        * Function:          GetElementNode
        ******************************************************************************/
        /// <summary>
        /// Returns the xml node that contains XAML for the type
        /// </summary>
        public static XmlElement GetElementNode(Type type, XamlTestDocument doc)
        {
            XmlElement elementNode = null;

            elementNode = (XmlElement)doc.SelectSingleNode("//av:Element[@Type='" + type.Name + "']", NamespaceManager);

            return elementNode;
        }

        /******************************************************************************
        * Function:          InsertTemplateTriggersNode
        ******************************************************************************/
        /// <summary>
        /// Insert two layers like
        /// <!-- <Element.Template>
        ///     <ControlTemplate>
        ///     </ControlTemplate>
        /// </Element.Template> -->
        /// </summary>
        /// <param name="doc"></param>
        /// <param name="elementNode">The node under which to insert this.</param>
        /// <returns>inner Template node</returns>
        public static XmlElement InsertTemplateTriggersNode(XmlDocument doc, XmlElement elementNode)
        {
            XmlElement templateComplexProp = doc.CreateElement("", elementNode.Name + ".Template", elementNode.NamespaceURI);
            elementNode.InsertAfter(templateComplexProp, null);

            XmlElement templateElement = doc.CreateElement("ControlTemplate", XamlGenerator.AvalonXmlns);
            // Set TargetType attribute on Template.
            templateElement.SetAttribute("TargetType", "{x:Type " + elementNode.Name + "}");
            templateComplexProp.InsertAfter(templateElement, null);

            XmlElement triggersElement = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
            templateElement.InsertAfter(triggersElement, null);
            return triggersElement;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          IsInAvalonNamespace
        ******************************************************************************/
        private static bool IsInAvalonNamespace(Type type)
        {
            string ns = type.Namespace;

            object[] oArray = type.Assembly.GetCustomAttributes(typeof(XmlnsDefinitionAttribute), false);

            bool expectingException = false;

            foreach (XmlnsDefinitionAttribute attribute in oArray)
            {
                if (String.Compare(attribute.XmlNamespace, s_avalonUri, true) == 0 &&
                    String.Compare(attribute.ClrNamespace, ns, true) == 0)
                {
                    expectingException = true;
                    break;
                }
            }

            return expectingException;
        }
        #endregion
    }
}

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
using System.Threading;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using System.Xml;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Markup;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;
using Microsoft.Test.Windows;

namespace Avalon.Test.CoreUI.Integration.TypeActions
{
    /******************************************************************************
    * CLASS:          ControlTemplateActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing ControlTemplate, all types and properties.
    /// </summary>
    [Test(1, "Integration.ControlTemplate", "ControlTemplateActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
    public class ControlTemplateActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor

        [Variation("Button")]
        [Variation("CheckBox")]
        [Variation("ComboBox")]
        [Variation("ComboBoxItem")]
        [Variation("ContentControl")]
        [Variation("Control")]
        [Variation("Expander")]
        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
        [Variation("FlowDocumentScrollViewer")]
        [Variation("Frame")]
        [Variation("GridSplitter")]
        [Variation("GridViewColumnHeader")]
        [Variation("GroupBox")]
        [Variation("GroupItem")]
        [Variation("HeaderedContentControl")]
        [Variation("HeaderedItemsControl")]
        [Variation("ItemsControl")]
        [Variation("Label")]
        [Variation("ListBox")]
        [Variation("ListBoxItem")]
        [Variation("ListView")]
        [Variation("ListViewItem")]
        // [DISABLE WHILE PORTING]
        // [Variation("Menu")]
        // [Variation("MenuItem")]
        [Variation("PasswordBox")]
        [Variation("ProgressBar")]
        [Variation("RepeatButton")]
        [Variation("ResizeGrip")]
        [Variation("RadioButton")]
        [Variation("RichTextBox")]
        [Variation("ScrollBar")]
        [Variation("ScrollViewer")]
        [Variation("Separator")]
        // [DISABLE WHILE PORTING]
        // [Variation("Slider", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disable there. 
        // [Variation("StatusBar")]
        [Variation("StatusBarItem")]
        [Variation("TabControl")]
        [Variation("TabItem")]
        [Variation("TextBox")]
        [Variation("Thumb")]
        [Variation("ToggleButton")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
        [Variation("UserControl")]

        /******************************************************************************
        * Function:          ControlTemplateActionRunner Constructor
        ******************************************************************************/
        public ControlTemplateActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoControlTemplateTest);
        }
        public ControlTemplateActionRunner() { }

        #endregion

        #region Test Steps
        /******************************************************************************
        * Function:          Initialize
        ******************************************************************************/
        /// <summary>
        /// Initialize.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult Initialize()
        {
            s_typeName = ActionHelper.GetFullName(s_typeName);
            ActionHelper.XamlFileName = "__ControlTemplateActionForType.xaml";
            ActionHelper.EmptyFileName= "ControlTemplateActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoControlTemplateTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements, loads it,
        /// and verifies that the instance's property values.
        /// </summary>
        TestResult DoControlTemplateTest()
        {
            // Make xaml.
            BuildControlTemplateActionXaml(s_typeName, s_xamlTestDocument);
            GlobalLog.LogFile(ActionHelper.XamlFileName);

            // Parse xaml.
            object root = SerializationHelper.ParseXamlFile(ActionHelper.XamlFileName);

            // Diplay tree.
            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root, "ControlTemplate", true);

            // Verification routine is called automatically when the tree
            // is rendered.

            GlobalLog.LogStatus("Xaml loaded. ");

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          BuildControlTemplateActionXaml
        ******************************************************************************/
        /// <summary>
        /// Builds a new xaml file for the type to use for loading and verification.
        /// </summary>
        private void BuildControlTemplateActionXaml(string typeName, XamlTestDocument testDoc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            XmlElement testRootNode = testDoc.TestRoot;

            // Get test roots.
            XmlElement typeInTemplateRoot = (XmlElement)testRootNode.SelectSingleNode("//*[@Name='TypeInTemplateRoot']", testDoc.Nsmgr);
            XmlElement templatedTypeRoot = (XmlElement)testRootNode.SelectSingleNode("//*[@Name='TemplatedTypeRoot']", testDoc.Nsmgr);
            XmlElement templateBindingTemplatedTypeRoot = (XmlElement)testRootNode.SelectSingleNode("//*[@Name='TemplateBindingTemplatedTypeRoot']", testDoc.Nsmgr);
            XmlElement typeRoot = (XmlElement)testRootNode.SelectSingleNode("//*[@Name='TypeRoot']", testDoc.Nsmgr);

            // Build xaml for each root
            BuildTypeInTemplateXaml(typeName, typeInTemplateRoot, testDoc);
            BuildTemplatedTypeXaml(typeName, templatedTypeRoot, testDoc);
            BuildTemplateBindingTemplatedTypeXaml(typeName, templateBindingTemplatedTypeRoot, testDoc);
            XmlElement untemplatedTypeElement = BuildTypeAllPropertiesSetXaml(typeName, typeRoot, testDoc);
            testDoc.AddAttribute(untemplatedTypeElement, "Name", "UntemplatedType");

            // Fix Verifier property
            testDoc.SetVerifierRoutine(this.GetType().GetMethod("VerifyTemplate"));
            
            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(testDoc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName);
        }

        /******************************************************************************
        * Function:          BuildTypeInTemplateXaml
        ******************************************************************************/
        /// <summary>
        /// This adds some xaml under root that creates a Control element and
        /// templates it with the type, typeName, and sets all valid properties on that type. 
        /// </summary>
        /// <remarks>
        /// This should perhaps template a Button instead of Control. Control is always a base of the
        /// type, we may see different problems for Button because it is in a different branch. 
        /// </remarks>
        private void BuildTypeInTemplateXaml(string typeName, XmlElement root, XamlTestDocument testDoc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            // Add control.
            XmlElement controlElement = testDoc.AddElement(root, "Control");
            testDoc.AddAttribute(controlElement, "Name", "TypeInTemplateControl");

            // Add template property element.
            XmlElement templateParent = testDoc.AddElement(controlElement, "Control.Template");

            // Add template.
            XmlElement template = testDoc.AddElement(templateParent, "ControlTemplate");

            // Place an instance of the type with all properties set in the ControlTemplate.
            XmlElement typeElement = BuildTypeAllPropertiesSetXaml(typeName, template, testDoc);
            testDoc.AddAttribute(typeElement, "Name", "TypeInTemplate");
        }

        /******************************************************************************
        * Function:          BuildTemplatedTypeXaml
        ******************************************************************************/
        /// <summary>
        /// Add some Xaml to the root that creates the type then templates it with a template
        /// that contains the same type with all valid properties set.
        /// </summary>
        /// <remarks>
        /// todo: 'Valid properties' is currently the same as Styled properties. This excludes, at least,
        /// Name and Resources, which should be added soon.
        /// </remarks>
        private void BuildTemplatedTypeXaml(string typeName, XmlElement root, XamlTestDocument testDoc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            // Add type container.
            XmlElement container = (XmlElement)testDoc.GetContainerNode(type);
            if (container != null)
            {
                container = testDoc.ImportAndAppendElement(root, container);

                // Get Element in container where we may add a child.
                container = (XmlElement)container.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
                container.RemoveAttribute("Name");
            }
            else
            {
                container = root;
            }
            
            // Add type.
            XmlElement typeElement = testDoc.AddElement(container, type.Name);
            testDoc.AddAttribute(typeElement, "Name", "TemplatedType");

            // Add template property element.
            XmlElement templateParent = testDoc.AddElement(typeElement, type.Name + ".Template");

            // Add template
            XmlElement template = testDoc.AddElement(templateParent, "ControlTemplate");
            testDoc.AddAttribute(template, "TargetType", "{x:Type " + type.Name + "}");

            // Place an instance of the type with all properties set in the ControlTemplate.
            XmlElement typeInTemplate = BuildTypeAllPropertiesSetXaml(typeName, template, testDoc);
            testDoc.AddAttribute(typeInTemplate, "Name", "TemplatedTypeInTemplate");
        }


        /******************************************************************************
        * Function:          BuildTemplateBindingTemplatedTypeXaml
        ******************************************************************************/
        /// <summary>
        /// Add some xaml to the root that creates the type, typeName, sets all valid properties on it, and 
        /// adds a template. The template contains the same type with TemplateBindings on all valid properties.
        /// </summary>
        private void BuildTemplateBindingTemplatedTypeXaml(string typeName, XmlElement root, XamlTestDocument testDoc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            // Add type and set every property.
            XmlElement typeElement = BuildTypeAllPropertiesSetXaml(typeName, root, testDoc);
            testDoc.AddAttribute(typeElement, "Name", "TemplateBindingTypeInTemplateParent");

            // Get or add the template property element and remove any children it may already have.
            XmlElement templateParent = testDoc.AddElement(typeElement, type.Name + ".Template");
            templateParent.RemoveAll();

            // Add template
            XmlElement template = testDoc.AddElement(templateParent, "ControlTemplate");
            testDoc.AddAttribute(template, "TargetType", "{x:Type " + type.Name + "}");

            // Add type container to template.
            XmlElement container = (XmlElement)testDoc.GetContainerNode(type);
            if (container != null)
            {
                container = testDoc.ImportAndAppendElement(template, container);

                // Get Element in container where we may add a child.
                container = (XmlElement)container.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
                container.RemoveAttribute("Name");
            }
            else
            {
                container = template;
            }

            // Add type to template.
            typeElement = testDoc.AddElement(container, type.Name);
            testDoc.AddAttribute(typeElement, "Name", "TemplateBindingTypeInTemplate");

            //
            // Template bind every property.
            //

            // Get all the valid inside template properties.
            List<DependencyPropertyDescriptor> properties = GetTemplateProperties(typeName);

            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                // Property name.
                string propName = dpd.Name;
                if (dpd.IsAttached)
                {
                    propName = dpd.DependencyProperty.OwnerType.Name + "." + dpd.Name;
                }

                if (propName != "Template")
                    testDoc.AddAttribute(typeElement, propName, "{TemplateBinding " + propName + "}");
            }
        }

        /******************************************************************************
        * Function:          BuildTypeAllPropertiesSetXaml
        ******************************************************************************/
        /// <summary>
        /// This adds some xaml to the root that creates the type and sets all properties on it. 
        /// This is used as a baseline for verification.
        /// </summary>
        /// <returns>
        /// Type's XmlElement
        /// </returns>
        /// <remarks>
        /// Returned XmlElement might not be the first child of root if the type requires a container.
        /// </remarks>
        private XmlElement BuildTypeAllPropertiesSetXaml(string typeName, XmlElement root, XamlTestDocument testDoc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            // Add type container.
            XmlElement container = (XmlElement)testDoc.GetContainerNode(type);
            if (container != null)
            {
                container = testDoc.ImportAndAppendElement(root, container);

                // Get Element in container where we may add a child.
                container = (XmlElement)container.SelectSingleNode("//*[@Name='ContainerElement']", ActionHelper.NamespaceManager);
                container.RemoveAttribute("Name");
            }
            else
            {
                container = root;
            }

            // Create element of type.
            XmlElement typeElement = testDoc.AddElement(container, type.Name);

            // Get all the valid inside template properties.
            List<DependencyPropertyDescriptor> properties = GetTemplateProperties(typeName);

            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                // Property name.
                string propName = dpd.Name;
                if (dpd.IsAttached)
                {
                    propName = dpd.DependencyProperty.OwnerType.Name + "." + dpd.Name;
                }

                // Property value.
                object value = ActionHelper.GetPropertyValue(testDoc, type.Name, dpd);

                // Assign property as attribute or property element.
                if (value == null)
                {
                    testDoc.AddAttribute(typeElement, propName, "{x:Null}");
                }
                else if (value is string || value.GetType().IsValueType)
                {
                    testDoc.AddAttribute(typeElement, propName, value.ToString());
                }
                else
                {
                    XmlElement valueElement = testDoc.AddElement(typeElement, type.Name + "." + propName);
                    if (value is XmlNode)
                    {
                        testDoc.ImportAndPrependElement(valueElement, (XmlElement)value);
                    }
                    else
                    {
                        valueElement.InnerXml = SerializationHelper.SerializeObjectTree(value);
                    }
                }
            }

            return typeElement;
        }

        /******************************************************************************
        * Function:          GetTemplateProperties
        ******************************************************************************/
        /// <summary>
        /// Returns list of properties that can be set in a template. 
        /// </summary>
        private static List<DependencyPropertyDescriptor> GetTemplateProperties(string typeName)
        {
            Type type = ActionHelper.GetType(typeName, true);

            List<DependencyPropertyDescriptor> properties = new List<DependencyPropertyDescriptor>();

            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);

                if (IsTemplateProperty(dpd))
                {
                    properties.Add(dpd);
                }
            }

            return properties;
        }

        /******************************************************************************
        * Function:          IsTemplateProperty
        ******************************************************************************/
        /// <summary>
        /// Checks if a property can be set in a template.
        /// Includes filters for certain properties, e.g. Name.
        /// </summary>
        private static bool IsTemplateProperty(DependencyPropertyDescriptor dpd)
        {
            bool isTemplateProperty = dpd != null
                    && !dpd.IsReadOnly
                    && dpd.DependencyProperty != FrameworkElement.NameProperty;

            return isTemplateProperty;
        }

        #endregion


        #region Verification
        /******************************************************************************
        * Function:          VerifyTemplate
        ******************************************************************************/
        /// <summary>
        /// Verifies templating after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyTemplate(UIElement uielement)
        {
            FrameworkElement root = (FrameworkElement)uielement;

            Control untemplatedTypeInstance = (Control)root.FindName("UntemplatedType");

            VerifyTypeInTemplate(root, untemplatedTypeInstance);
            VerifyTemplatedType(root, untemplatedTypeInstance);
            VerifyTemplateBindingTemplatedType(root, untemplatedTypeInstance);
        }

        /******************************************************************************
        * Function:          VerifyTypeInTemplate
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        private void VerifyTypeInTemplate(FrameworkElement root, Control untemplatedTypeInstance)
        {
            GlobalLog.LogStatus("Verifying type instance in template.", ConsoleColor.Yellow);

            // Get the type instance in the template on a Control.
            Control testControl = (Control)root.FindName("TypeInTemplateControl");
            Control typeInTemplateInstance = (Control)testControl.Template.FindName("TypeInTemplate", testControl);

            if (typeInTemplateInstance == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find type instance in template");
            }

            VerifyTemplateProperties(typeInTemplateInstance, untemplatedTypeInstance, false);
        }

        /******************************************************************************
        * Function:          VerifyTemplatedType
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        private void VerifyTemplatedType(FrameworkElement root, Control untemplatedTypeInstance)
        {
            GlobalLog.LogStatus("Verifying type instance in template on type instance.", ConsoleColor.Yellow);
            
            // Get the type instance in a template on an instance of the type.
            Control templatedType = (Control)root.FindName("TemplatedType");
            if (templatedType is ContextMenu)
            {
                ContextMenu cm = templatedType as ContextMenu;
                cm.IsOpen = true;
                DispatcherHelper.DoEvents(2000);
            }
            else if (templatedType is ToolTip)
            {
                ToolTip t = templatedType as ToolTip;
                t.IsOpen = true;
                DispatcherHelper.DoEvents(2000);
            }

            Control typeInTemplateInstance = (Control)templatedType.Template.FindName("TemplatedTypeInTemplate", templatedType);

            if (typeInTemplateInstance == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find templated type instance in template");
            }

            VerifyTemplateProperties(typeInTemplateInstance, untemplatedTypeInstance, false);
        }

        /******************************************************************************
        * Function:          VerifyTemplateBindingTemplatedType
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        private void VerifyTemplateBindingTemplatedType(FrameworkElement root, Control untemplatedTypeInstance)
        {
            GlobalLog.LogStatus("Verifying template binding type instance.", ConsoleColor.Yellow);

            // Get the type instance 
            Control templateBindingTemplatedType = (Control)root.FindName("TemplateBindingTypeInTemplateParent");
            if (templateBindingTemplatedType is ContextMenu)
            {
                ContextMenu cm = templateBindingTemplatedType as ContextMenu;
                cm.IsOpen = true;
                DispatcherHelper.DoEvents(2000);
            }
            else if (templateBindingTemplatedType is ToolTip)
            {
                ToolTip t = templateBindingTemplatedType as ToolTip;
                t.IsOpen = true;
                DispatcherHelper.DoEvents(2000);
            }

            Control templateBindingTypeInstance = (Control)templateBindingTemplatedType.Template.FindName("TemplateBindingTypeInTemplate", templateBindingTemplatedType);

            if (templateBindingTypeInstance == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find template binding type instance in template");
            }

            VerifyTemplateProperties(templateBindingTypeInstance, untemplatedTypeInstance, true);
        }

        /******************************************************************************
        * Function:          VerifyTemplateProperties
        ******************************************************************************/
        /// <summary>
        /// Compare properties on an untemplated instance of the type against one that is in a template.
        /// </summary>
        private void VerifyTemplateProperties(Control templatedTypeInstance, Control untemplatedTypeInstance, bool ignoreTemplateProperty)
        {
            Type type = templatedTypeInstance.GetType();

            //
            // Verify properties on instance of type in template.
            //

            List<DependencyPropertyDescriptor> properties = GetTemplateProperties(type.FullName);

            // Add BaseUri and BaseUriHelper.BaseUri skip properties.
            PropertyToIgnore ignoreProp = new PropertyToIgnore();
            ignoreProp.WhatToIgnore = IgnoreProperty.IgnoreNameAndValue;
            AddPropertyToSkip("BaseUri", ignoreProp);
            AddPropertyToSkip("BaseUriHelper.BaseUri", ignoreProp);
            if (ignoreTemplateProperty) AddPropertyToSkip("Template", ignoreProp); 

            // Check each property.
            foreach (DependencyPropertyDescriptor dpd in properties)
            {
                if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, ActionHelper.PropertiesToSkip))
                    continue;

                GlobalLog.LogStatus("Check " + dpd.Name);

                // Check property is set from template parent.
                ValueSource valueSource = DependencyPropertyHelper.GetValueSource(templatedTypeInstance, dpd.DependencyProperty);
                if (valueSource.BaseValueSource != BaseValueSource.ParentTemplate)
                {
                    throw new Microsoft.Test.TestValidationException("'" + type.Name + "' element's '" + dpd.Name + "' property value does not come from the template parent.");
                }

                // Check property value is equivalent to locally-set property on untemplated element.
                ActionForTypeHelper.CheckExpectedPropertyValue(templatedTypeInstance, untemplatedTypeInstance, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
            }
        }

        /******************************************************************************
        * Function:          AddPropertyToSkip
        ******************************************************************************/
        private void AddPropertyToSkip(string name, PropertyToIgnore propertyToIgnore)
        {
            if (ActionHelper.PropertiesToSkip.ContainsKey(name))
                return;

            ActionHelper.PropertiesToSkip.Add(name, propertyToIgnore);
        }
        #endregion
    }

}

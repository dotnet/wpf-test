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

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Windows;


namespace Avalon.Test.CoreUI.Integration.TypeActions
{

    /// <summary>
    /// Implements various routines for testing Avalon types.
    ///</summary>
    [TestDefaults(DefaultPriority=1, DefaultMethodName="RunTest", SupportFiles=new string[]{@"FeatureTests\ElementServices\ActionForTypeExclude.xml",@"Common\PropertiesToSkip.xml",@"FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml",@"FeatureTests\ElementServices\IntegrationXamlSnippets.xaml",@"FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml",@"FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml"})]
    public class ActionRunner
    {

        #region Private Data
        /// <summary>
        /// Represents an action that may be applied to types.
        /// </summary>
        public enum ActionForType
        {        
            /// <summary>
            /// Verifies each type that may have a ControlTemplate
            /// </summary>        
            ControlTemplate,

            /// <summary>
            /// Verifies that each type can be in DataTemplate and all properties 
            /// Bind to an element in logical tree, and that the DataTemplate
            /// can be used in all scenarios
            /// </summary>        
            DataTemplate,

            /// <summary>
            /// Verifies that all properties can be source or source of Binding
            /// and all properties may be set via Binding
            /// </summary>
            ElementBinding,

            /// <summary>
            /// Verifies that each routed event can be used as source of EventTrigger
            /// and all properties may be changed by EventTrigger
            /// </summary>
            EventTrigger,

            /// <summary>
            /// Verifies mouse interaction with each types.
            /// </summary>        
            MouseAction,

            /// <summary>
            /// Verifies that each type and its properties may have a Style
            /// </summary>        
            Style,

            /// <summary>
            /// Verifies that each type and its properties may have a PropertyTrigger in a Style
            /// </summary>        
            StylePropertyTrigger,

            /// <summary>
            /// Verifies that each type and its properties may be PropertyTrigger/Storyboard in Style
            /// </summary>        
            StylePropertyTriggerStoryboard,

            /// <summary>
            /// Verifies that each type and its properties may have a PropertyTrigger in a Template
            /// </summary>        
            TemplatePropertyTrigger,

            /// <summary>
            /// Verifies that each type and its properties may be PropertyTrigger in Template
            /// </summary>        
            TemplatePropertyTriggerStoryboard
        }

        private static object                           s_syncObject              = new object();
        private static Assembly[]                       s_assemblies              = null;
        private static XmlNamespaceManager              s_nsmgr                   = null;
        private Dictionary<string, PropertyToIgnore>    _propertiesToSkip        = null;
        private static XamlTestDocument                 s_testDoc                 = null;
        private static string                           s_snippetsFileName        = "IntegrationXamlSnippets.xaml";
        private static string                           s_emptyFileName           = null;
        private static readonly string                  s_avalonUri               = "http://schemas.microsoft.com/winfx/2006/xaml/presentation";
        private ActionForType                           _actionType;
        #endregion

        #region Constructors
        /// <summary>
        /// Constructor.
        /// </summary>
        public ActionRunner() { }

        // Initialize the Avalon assemblies.
        static ActionRunner()
        {
            s_assemblies = new Assembly[]
            {
                typeof(FrameworkElement).Assembly,
                typeof(UIElement).Assembly,
                typeof(DependencyObject).Assembly
            };
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          RunTest
        ******************************************************************************/
        /// <summary>
        /// Launch the test.
        /// </summary>
        /// <param name="action">The Action requested by the test case.</param>
        /// <param name="actionSubGroupTotal">The number of subgroups for the Action.</param>
        /// <param name="actionSubGroupIndex">The subgroup requested.</param>
        [Test(@"Integration\ControlTemplate\Group1",            TestCaseSecurityLevel.FullTrust, @"ControlTemplate1",                   MethodParams="ControlTemplate,4,0",                         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\ControlTemplate\Group2",            TestCaseSecurityLevel.FullTrust, @"ControlTemplate2",                   MethodParams="ControlTemplate,4,1",                         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\ControlTemplate\Group3",            TestCaseSecurityLevel.FullTrust, @"ControlTemplate3",                   MethodParams="ControlTemplate,4,2",                         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\ControlTemplate\Group4",            TestCaseSecurityLevel.FullTrust, @"ControlTemplate4",                   MethodParams="ControlTemplate,4,3",                         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group1",               TestCaseSecurityLevel.FullTrust, @"DataTemplate1",                      MethodParams="DataTemplate,10,0",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group2",               TestCaseSecurityLevel.FullTrust, @"DataTemplate2",                      MethodParams="DataTemplate,10,1",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group3",               TestCaseSecurityLevel.FullTrust, @"DataTemplate3",                      MethodParams="DataTemplate,10,2",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group4",               TestCaseSecurityLevel.FullTrust, @"DataTemplate4",                      MethodParams="DataTemplate,10,3",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group5",               TestCaseSecurityLevel.FullTrust, @"DataTemplate5",                      MethodParams="DataTemplate,10,4",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group6",               TestCaseSecurityLevel.FullTrust, @"DataTemplate6",                      MethodParams="DataTemplate,10,5",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group7",               TestCaseSecurityLevel.FullTrust, @"DataTemplate7",                      MethodParams="DataTemplate,10,6",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group8",               TestCaseSecurityLevel.FullTrust, @"DataTemplate8",                      MethodParams="DataTemplate,10,7",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\Group9",               TestCaseSecurityLevel.FullTrust, @"DataTemplate9",                      MethodParams="DataTemplate,10,8",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\DataTemplate\GroupA",               TestCaseSecurityLevel.FullTrust, @"DataTemplateA",                      MethodParams="DataTemplate,10,9",                           Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\DataTemplateActionForType_empty.xaml")]
        [Test(@"Integration\ElementBinding\Group1",             TestCaseSecurityLevel.FullTrust, @"ElementBinding1",                    MethodParams="TemplatePropertyTrigger,3,0",                 Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ElementBindingActionForType_empty.xaml")]
        [Test(@"Integration\ElementBinding\Group2",             TestCaseSecurityLevel.FullTrust, @"ElementBinding2",                    MethodParams="TemplatePropertyTrigger,3,1",                 Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ElementBindingActionForType_empty.xaml")]
        [Test(@"Integration\ElementBinding\Group3",             TestCaseSecurityLevel.FullTrust, @"ElementBinding3",                    MethodParams="TemplatePropertyTrigger,3,2",                 Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ElementBindingActionForType_empty.xaml")]
        [Test(@"Integration\EventTrigger\Group1",               TestCaseSecurityLevel.FullTrust, @"EventTrigger1",                      MethodParams="EventTrigger,5,0",                            Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
        [Test(@"Integration\EventTrigger\Group2",               TestCaseSecurityLevel.FullTrust, @"EventTrigger2",                      MethodParams="EventTrigger,5,1",                            Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
        [Test(@"Integration\EventTrigger\Group3",               TestCaseSecurityLevel.FullTrust, @"EventTrigger3",                      MethodParams="EventTrigger,5,2",                            Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
        [Test(@"Integration\EventTrigger\Group4",               TestCaseSecurityLevel.FullTrust, @"EventTrigger4",                      MethodParams="EventTrigger,5,3",                            Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
        [Test(@"Integration\EventTrigger\Group5",               TestCaseSecurityLevel.FullTrust, @"EventTrigger5",                      MethodParams="EventTrigger,5,4",                            Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group1",                TestCaseSecurityLevel.FullTrust, @"MouseAction1",                       MethodParams="MouseAction,8,0",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group2",                TestCaseSecurityLevel.FullTrust, @"MouseAction2",                       MethodParams="MouseAction,8,1",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group3",                TestCaseSecurityLevel.FullTrust, @"MouseAction3",                       MethodParams="MouseAction,8,2",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group4",                TestCaseSecurityLevel.FullTrust, @"MouseAction4",                       MethodParams="MouseAction,8,3",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group5",                TestCaseSecurityLevel.FullTrust, @"MouseAction5",                       MethodParams="MouseAction,8,4",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group6",                TestCaseSecurityLevel.FullTrust, @"MouseAction6",                       MethodParams="MouseAction,8,5",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group7",                TestCaseSecurityLevel.FullTrust, @"MouseAction7",                       MethodParams="MouseAction,8,6",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\MouseAction\Group8",                TestCaseSecurityLevel.FullTrust, @"MouseAction8",                       MethodParams="MouseAction,8,7",                             Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\MouseActionForType_empty.xaml")]
        [Test(@"Integration\Style\Group1",                      TestCaseSecurityLevel.FullTrust, @"Style1",                             MethodParams="Style,3,0",                                   Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StyleActionForType_empty.xaml")]
        [Test(@"Integration\Style\Group2",                      TestCaseSecurityLevel.FullTrust, @"Style2",                             MethodParams="Style,3,1",                                   Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StyleActionForType_empty.xaml")]
        [Test(@"Integration\Style\Group3",                      TestCaseSecurityLevel.FullTrust, @"Style3",                             MethodParams="Style,3,2",                                   Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StyleActionForType_empty.xaml")]
        [Test(@"Integration\StylePropertyTrigger\Group1",       TestCaseSecurityLevel.FullTrust, @"StylePropertyTrigger1",              MethodParams="StylePropertyTrigger,4,0",                    Timeout=360)]
        [Test(@"Integration\StylePropertyTrigger\Group2",       TestCaseSecurityLevel.FullTrust, @"StylePropertyTrigger2",              MethodParams="StylePropertyTrigger,4,1",                    Timeout=360)]
        [Test(@"Integration\StylePropertyTrigger\Group3",       TestCaseSecurityLevel.FullTrust, @"StylePropertyTrigger3",              MethodParams="StylePropertyTrigger,4,2",                    Timeout=360)]
        [Test(@"Integration\StylePropertyTrigger\Group4",       TestCaseSecurityLevel.FullTrust, @"StylePropertyTrigger4",              MethodParams="StylePropertyTrigger,4,3",                    Timeout=360)]
        [Test(@"Integration\StylePropertyTrigger\FCE\Storyboard\Group1", TestCaseSecurityLevel.FullTrust, @"StylePropertyTriggerFCEStoryboard1", MethodParams="StylePropertyTriggerStoryboard,5,0", Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StylePropertyTriggerActionForType_PropertiesToSkip.xml")]
        [Test(@"Integration\StylePropertyTrigger\FCE\Storyboard\Group2", TestCaseSecurityLevel.FullTrust, @"StylePropertyTriggerFCEStoryboard2", MethodParams="StylePropertyTriggerStoryboard,5,1", Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StylePropertyTriggerActionForType_PropertiesToSkip.xml")]
        [Test(@"Integration\StylePropertyTrigger\FCE\Storyboard\Group3", TestCaseSecurityLevel.FullTrust, @"StylePropertyTriggerFCEStoryboard3", MethodParams="StylePropertyTriggerStoryboard,5,2", Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StylePropertyTriggerActionForType_PropertiesToSkip.xml")]
        [Test(@"Integration\StylePropertyTrigger\FCE\Storyboard\Group4", TestCaseSecurityLevel.FullTrust, @"StylePropertyTriggerFCEStoryboard4", MethodParams="StylePropertyTriggerStoryboard,5,3", Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StylePropertyTriggerActionForType_PropertiesToSkip.xml")]
        [Test(@"Integration\StylePropertyTrigger\FCE\Storyboard\Group5", TestCaseSecurityLevel.FullTrust, @"StylePropertyTriggerFCEStoryboard5", MethodParams="StylePropertyTriggerStoryboard,5,4", Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\StylePropertyTriggerActionForType_PropertiesToSkip.xml")]
        [Test(@"Integration\TemplatePropertyTrigger\Basic\Group1",     TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTrigger1",            MethodParams="TemplatePropertyTrigger,3,0",         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\TemplatePropertyTrigger\Basic\Group2",     TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTrigger2",            MethodParams="TemplatePropertyTrigger,3,1",         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\TemplatePropertyTrigger\Basic\Group3",     TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTrigger3",            MethodParams="TemplatePropertyTrigger,3,2",         Timeout=360,    SupportFiles=@"FeatureTests\ElementServices\ControlTemplateActionForType_empty.xaml")]
        [Test(@"Integration\TemplatePropertyTrigger\Control\Storyboard\Group1",TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTriggerStoryboard1",  MethodParams="TemplatePropertyTriggerStoryboard,3,0", Timeout=360)]
        [Test(@"Integration\TemplatePropertyTrigger\Control\Storyboard\Group2",TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTriggerStoryboard2",  MethodParams="TemplatePropertyTriggerStoryboard,3,1", Timeout=360)]
        [Test(@"Integration\TemplatePropertyTrigger\Control\Storyboard\Group3",TestCaseSecurityLevel.FullTrust, @"TemplatePropertyTriggerStoryboard3",  MethodParams="TemplatePropertyTriggerStoryboard,3,2", Timeout=360)]
        public void RunTest(string action, int actionSubGroupTotal, int actionSubGroupIndex)
        {

            GlobalLog.LogStatus("*******************************************");
            GlobalLog.LogStatus("Action: " + action + " " + actionSubGroupIndex);
            GlobalLog.LogStatus("*******************************************");

            _actionType = (ActionForType)Enum.Parse(typeof(ActionForType), action, true);

            List<string> elementList = GenerateElementList(_actionType, actionSubGroupTotal, actionSubGroupIndex);

            if (elementList.Count == 0)
            {
                GlobalLog.LogEvidence("ERROR: no entries were found in the Control List."); 
                TestLog log = new TestLog("ElementServices Integration Testing");
                log.Result = TestResult.Fail;                    
                log.Close();
            }

            //Main loop: test each Element in the elementList.
            foreach (string element in elementList)
            {
                TestLog log = new TestLog(element);

                StartTest(element, _actionType);

                log.Close();
            }
        }

        /******************************************************************************
        * Function:          GetType
        ******************************************************************************/
        /// <summary>
        /// Finds and returns the Type for the given type name.
        /// Throws if type isn't found.
        /// </summary>
        protected static Type GetType(string element)
        {
            Type type = null;

            for (int i = 0; type == null && i < s_assemblies.Length; i++)
            {
                type = s_assemblies[i].GetType(element, false, false);
            }

            if (type == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find type '" + element + "'.");
            }

            return type;
        }

        /// <summary>
        /// Name of snippets file used for xaml generation.
        /// </summary>
        protected string SnippetsFileName
        {
            set
            {
                if (!String.Equals(s_snippetsFileName, value, StringComparison.InvariantCultureIgnoreCase))
                {
                    s_snippetsFileName = value;
                    s_testDoc = null;
                }
            }
        }

        /// <summary>
        /// Name of template file used for xaml generation.
        /// </summary>
        protected string EmptyFileName
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
        protected XamlTestDocument TestDocument
        {
            get
            {
                if (s_testDoc == null)
                {
                    s_testDoc = new XamlTestDocument(s_emptyFileName, s_snippetsFileName);
                }

                return s_testDoc;
            }
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
        protected static object GetPropertyValue(XamlTestDocument doc, string element, DependencyPropertyDescriptor dpd)
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
        protected static object GetPropertyValue(XamlTestDocument doc, string element, DependencyPropertyDescriptor dpd, int index)
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
        /// <returns>Dictionary of PropertyToIgnore items indexed by property name.</returns>
        protected Dictionary<string, PropertyToIgnore> PropertiesToSkip
        {
            get
            {
                if (_propertiesToSkip == null)
                {
                    _propertiesToSkip = TreeComparer.ReadSkipProperties(PropertiesToSkipFileName);
                }

                return _propertiesToSkip;
            }
        }

        /// <summary>
        /// File to use for directing properties to ignore in xaml generation and verification.
        /// </summary>
        protected string PropertiesToSkipFileName = "ActionForType_PropertiesToSkip.xml";

        /// <summary>
        /// Name of xaml file that is generated.
        /// </summary>
        protected string XamlFileName = "__ActionForType.xaml";

        /******************************************************************************
        * Function:          XmlNamespaceManager
        ******************************************************************************/
        /// <summary>
        /// Returns the common xmlns manager used for xaml generation in action handlers.
        /// </summary>
        protected static XmlNamespaceManager NamespaceManager
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
        #endregion


        #region Private Members

        /******************************************************************************
        * Function:          GenerateElementList
        ******************************************************************************/
        /// <summary>
        /// Uses reflection to create a list of Controls.
        /// </summary>
        /// <param name="actionType">The Acton requested by the test case, e.g., EventTrigger.</param>
        /// <param name="actionSubGroupTotal">The number of subgroups for a given ActionForType.</param>
        /// <param name="actionSubGroupIndex">The requested subgroup for a given ActionForType.</param>
        ///<returns>An ArrayList containing the Controls to be tested.</returns>
        private List<string> GenerateElementList(ActionForType actionType, int actionSubGroupTotal, int actionSubGroupIndex)
        {
            //First, obtain a list of types to be excluded from the test.
            List<string> exclusions = GetExclusionList(actionType.ToString());
            //Use reflection to create a list of type to test.
            List<string> typeList = new List<string>();

            Assembly assembly = typeof(FrameworkElement).Assembly;
            Type feType  = assembly.GetType("System.Windows.FrameworkElement", true);
            Type fceType = assembly.GetType("System.Windows.FrameworkContentElement", true);
            Type control = assembly.GetType("System.Windows.Controls.Control", true);

            Type[] allTypes = assembly.GetTypes();

            switch (actionType)
            {
                case ActionForType.ControlTemplate:
                case ActionForType.TemplatePropertyTrigger:
                case ActionForType.TemplatePropertyTriggerStoryboard:
                    AddToTypeList(ref typeList, actionSubGroupTotal, actionSubGroupIndex, allTypes, exclusions, control);
                    break;

                case ActionForType.MouseAction:
                case ActionForType.Style:
                case ActionForType.StylePropertyTrigger:
                case ActionForType.StylePropertyTriggerStoryboard:
                case ActionForType.EventTrigger:
                case ActionForType.ElementBinding:
                    AddToTypeList(ref typeList, actionSubGroupTotal, actionSubGroupIndex, allTypes, exclusions, feType);
                    AddToTypeList(ref typeList, actionSubGroupTotal, actionSubGroupIndex, allTypes, exclusions, fceType);
                    break;

                case ActionForType.DataTemplate:
                    AddToTypeList(ref typeList, actionSubGroupTotal, actionSubGroupIndex, allTypes, exclusions, feType);
                    break;
            }
            
            return typeList;
        }

        /******************************************************************************
        * Function:          AddToTypeList
        ******************************************************************************/
        /// <summary>
        /// Determines which types will be tested for a given ActionForType.  Each type in 'allTypes' (in PresentationFramework) is compared to the 'requestedType',
        /// which is the specific type requested by the test case, e.g., FrameworkContentElement.  If the type's InnerObject matches the requested type's
        /// InnerObject, and also satisfies some additional criteria, the type is then added to the list to be tested.
        /// </summary>
        /// <param name="listOfTypes">The list of Types that will actually be tested.</param>
        /// <param name="actionSubGroups">The number of subgroups for the specified ActionForType request.</param>
        /// <param name="actionSubGroup">The specific subgroup for an ActionForType request.</param>
        /// <param name="allTypes">An array of all Types matching those needed by the text case, e.g., all Controls.</param>
        /// <param name="exclusionList">A list of elements to be excluded from the elements to be tested.</param>
        /// <param name="requestedType">The Type requested by the test case.</param>
        ///<returns>An updated list of Types.</returns>
        private void AddToTypeList(ref List<string> listOfTypes, int actionSubGroupTotal, int actionSubGroupIndex, Type[] allTypes, List<string> exclusionList, Type requestedType)
        {
            int typeCount = 0;

            foreach (Type currentType in allTypes)
            {
                if (MatchTypes(requestedType, currentType))
                {
                    string element = currentType.FullName;
                    if (!exclusionList.Contains(element))
                    {
                        typeCount++;

                        if (AddToPartition(typeCount, actionSubGroupTotal, actionSubGroupIndex))
                        {
                            GlobalLog.LogStatus("  " + element);
                            listOfTypes.Add(element); 
                        }
                    }
                }
            }
        }

        /******************************************************************************
        * Function:          MatchTypes
        ******************************************************************************/
        /// <summary>
        /// Determines whether or not a Type matches certain criteria.
        /// </summary>
        /// <param name="baseType">The requested Type to be compared.</param>
        /// <param name="currentType">A Type from the list of Types to be checked against a set of criteria.</param>
        ///<returns>Returns true if the types match.</returns>
        private bool MatchTypes(Type baseType, Type currentType)
        {
            Type[] emptyArray = new Type[0];

            bool isMatching =
                (currentType.IsValueType || currentType.GetConstructor(emptyArray) != null) // Value types always have empty ctor.
                && currentType.IsPublic
                && !currentType.IsAbstract
                && !currentType.IsGenericType
                && !currentType.IsEnum
                && (currentType == baseType || currentType.IsSubclassOf(baseType));

            return (isMatching);
        }

        /******************************************************************************
        * Function:          AddToPartition
        ******************************************************************************/
        /// <summary>
        /// Determines whether or not an eligible type should be added to the requested ActionForType subgroup.  
        /// An ActionForType (e.g., DataTemplate) is split into separate subgroups of elements to be tested.
        /// This is done to reduce the total run-time for a given ActionForType test group.
        /// </summary>
        /// <param name="typeCount">The cumulative count of types to-be-tested.</param>
        /// <param name="actionSubGroupTotal">The number of subgroups for an ActionForType.</param>
        /// <param name="actionSubGroupIndex">The specified subgroup for an ActionForType.</param>
        ///<returns>Returns true if the element is assigned to the requested subgroup.</returns>
        private bool AddToPartition(int typeCount, int actionSubGroupTotal, int actionSubGroupIndex)
        {
            if ((typeCount % actionSubGroupTotal) == actionSubGroupIndex)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /******************************************************************************
        * Function:          GetExclusionList
        ******************************************************************************/
        /// <summary>
        /// 
        /// </summary>
        /// <param name="actionName">The name of an action specified by the test case.</param>
        ///<returns></returns>
        private List<string> GetExclusionList(string actionName)
        {
            List<string> exclusionList = new List<string>();

            //Retrieve Exclusions from an .xml file.
            XmlDocument xmlDoc = new XmlDocument();
            xmlDoc.Load("ActionForTypeExclude.xml");

            // Check section for the current action.
            XmlNodeList nodeList = xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='" + actionName.ToString() + "']/ExcludeTypes/ExcludeType");

            foreach (XmlNode node in nodeList)
            {
                exclusionList.Add(node.Attributes["Name"].Value);
            }

            // Check section for all actions.
            nodeList = xmlDoc.SelectNodes("SetupActionForTypes/SetupActionForType[@Name='All']/ExcludeTypes/ExcludeType");

            foreach (XmlNode node in nodeList)
            {
                exclusionList.Add(node.Attributes["Name"].Value);
            }
            
            return exclusionList;
        }


        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Selects which test to run, based on the specified Action.
        /// </summary>
        /// <param name="element">The element to be tested.</param>
        /// <param name="actionType">The type of action specified by the test case.</param>
        ///<returns></returns>
        private void StartTest(string element, ActionForType actionType)
        {

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            switch (actionType)
            {
                case ActionForType.ControlTemplate:

                    ControlTemplateActionRunner runner1 = new ControlTemplateActionRunner();
                    runner1.DoControlTemplateTest(element, actionType);
                    break;

                case ActionForType.DataTemplate:

                    DataTemplateActionRunner runner2 = new DataTemplateActionRunner();
                    runner2.DoDataTemplateTest(element, actionType);
                    break;

                case ActionForType.ElementBinding:

                    ElementBindingActionRunner runner3 = new ElementBindingActionRunner();
                    runner3.DoElementBindingTest(element, actionType);
                    break;

                case ActionForType.EventTrigger:

                    EventTriggerActionRunner runner4 = new EventTriggerActionRunner();
                    runner4.DoEventTriggerTest(element, actionType);
                    break;

                case ActionForType.MouseAction:
                    MouseActionRunner runner5 = new MouseActionRunner();
                    runner5.DoMouseActionTest(element, actionType);
                    break;

                case ActionForType.Style:

                    StyleActionRunner runner6 = new StyleActionRunner();
                    runner6.DoStyleTest(element, actionType);
                    break;

                case ActionForType.StylePropertyTrigger:

                    StylePropertyTriggerActionRunner runner7 = new StylePropertyTriggerActionRunner();
                    runner7.DoStylePropertyTriggerTest(element, actionType);
                    break;

                case ActionForType.StylePropertyTriggerStoryboard:

                    StylePropertyTriggerStoryboardActionRunner runner8 = new StylePropertyTriggerStoryboardActionRunner();
                    runner8.DoStylePropertyTriggerStoryboardTest(element, actionType);
                    break;

                case ActionForType.TemplatePropertyTrigger:

                    TemplatePropertyTriggerActionRunner runner9 = new TemplatePropertyTriggerActionRunner();
                    runner9.DoTemplatePropertyTriggerTest(element, actionType);
                    break;

                case ActionForType.TemplatePropertyTriggerStoryboard:

                    TemplatePropertyTriggerStoryboardActionRunner runner10 = new TemplatePropertyTriggerStoryboardActionRunner();
                    runner10.DoTemplatePropertyTriggerTest(element, actionType);
                    break;

                default:
                    throw new NotImplementedException(actionType.ToString());
            }

            s_testDoc = null;

            //Any test failures will be caught by throwing an Exception (or, in the case of MouseActionRunner, calling CheckExpectedPropertyValue).
            if (TestLog.Current != null)
            {
                TestLog.Current.Result = TestResult.Pass;
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
        private string GetNamespace(Type type)
        {
            string xmlns = s_avalonUri;

            if (!IsInAvalonNamespace(type))
            {
                xmlns =
                    "clr-namespace:" +
                    type.Namespace +
                    ";assembly=" +
                    type).Assembly.GetName(.Name;
            }

            return xmlns;
        }

        private bool IsInAvalonNamespace(Type type)
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

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
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Windows.Media;
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
    * CLASS:          EventTriggerActionRunner
    ******************************************************************************/
    /// <summary>
    /// Implements routines for testing Avalon EventTrigger in Style, all types and properties.
    ///</summary>
    [Test(1, "Integration.EventTrigger", "EventTriggerActionRunner", SecurityLevel=TestCaseSecurityLevel.FullTrust, SupportFiles=@"FeatureTests\ElementServices\ActionForTypeExclude.xml,Common\PropertiesToSkip.xml,FeatureTests\ElementServices\PropertyTriggerActionForType_empty.xaml,FeatureTests\ElementServices\IntegrationXamlSnippets.xaml,FeatureTests\ElementServices\MouseActionIntegrationXamlSnippets.xaml,FeatureTests\ElementServices\ActionForType_PropertiesToSkip.xml,FeatureTests\ElementServices\EventTriggerActionForType_empty.xaml")]
    public class EventTriggerActionRunner : WindowTest
    {
        #region Private Data
        private static XamlTestDocument                 s_xamlTestDocument        = null;
        private static string                           s_typeName                = "";
        #endregion


        #region Constructor

//        [Variation("BlockUIContainer")]
        [Variation("Bold")]
        [Variation("Border")]
        [Variation("Button")]
        [Variation("Canvas")]
        [Variation("CheckBox")]
        [Variation("ComboBox", Versions = "3.0SP1,3.0SP2,AH")]  // Failing regularly on only 4.X, disabled there. 
        // [DISABLE WHILE PORTING]
        // [Variation("ComboBoxItem")]
        [Variation("ContentPresenter")]
        // [Variation("ContextMenu")] // [DISABLE WHILE PORTING]
        [Variation("Decorator")]
        [Variation("DockPanel")]
        [Variation("DocumentViewer")]
        [Variation("Ellipse")]
        [Variation("Expander")]
        [Variation("Figure")]
//        [Variation("Floater")]
//        [Variation("FlowDocumentPageViewer")]
        [Variation("FlowDocumentReader")]
//        [Variation("FlowDocumentScrollViewer")]
//        [Variation("Frame")]
        [Variation("FrameworkContentElement")]
        [Variation("FrameworkElement")]
        [Variation("Grid")]
        [Variation("GridSplitter")]
//        [Variation("GroupBox")]
//        [Variation("HeaderedItemsControl")]
        [Variation("Hyperlink")]
        [Variation("Image")]
        [Variation("InkCanvas")]
//        [Variation("InlineUIContainer")]
        [Variation("ItemsControl")]
        [Variation("Label")]
//        [Variation("List")]
        [Variation("ListBox")]
//        [Variation("ListItem")]
        [Variation("ListView")]
//        [Variation("ListViewItem")]
        // [Variation("Menu")] // [DISABLE WHILE PORTING]
        // [Variation("MenuItem")] // [DISABLE WHILE PORTING]
//        [Variation("Paragraph")]
        [Variation("PasswordBox")]
        [Variation("Path")]
        [Variation("RepeatButton")]
//        [Variation("ResizeGrip")]
        [Variation("RichTextBox")]
//        [Variation("Separator")]
        [Variation("ScrollBar")]
//        [Variation("ScrollViewer")]
//        [Variation("Table")]
//        [Variation("TableCell")]
        [Variation("TextBlock")]
        [Variation("TextBox")]
        [Variation("Thumb")]
        [Variation("TickBar")]
        [Variation("TreeView")]
        [Variation("TreeViewItem")]
//        [Variation("UserControl")]
        [Variation("Viewport3D")]
        [Variation("VirtualizingStackPanel")]
        [Variation("WebBrowser")]

        /******************************************************************************
        * Function:          EventTriggerActionRunner Constructor
        ******************************************************************************/
        public EventTriggerActionRunner(string arg)
        {
            s_typeName = arg;

            InitializeSteps += new TestStep(Initialize);
            RunSteps += new TestStep(DoEventTriggerTest);
        }
        public EventTriggerActionRunner() { }
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
            ActionHelper.XamlFileName = "__EventTriggerActionForType.xaml";
            ActionHelper.EmptyFileName= "EventTriggerActionForType_empty.xaml";
            s_xamlTestDocument = ActionHelper.TestDocument;

            // Set culture to 'en-us' so round-tripping will work correctly.
            Thread.CurrentThread.CurrentCulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfoByIetfLanguageTag("en-us");

            return TestResult.Pass;
        }

        /******************************************************************************
        * Function:          DoEventTriggerTest
        ******************************************************************************/
        /// <summary>
        /// Builds xaml with elements with EventTrigger in Template, loads it,
        /// and verifies that the instance's property values were triggered correctly.
        /// </summary>
        /// <returns>A TestResult</returns>
        TestResult DoEventTriggerTest()
        {
            BuildEventTriggerXaml(s_typeName, s_xamlTestDocument);
            GlobalLog.LogFile(ActionHelper.XamlFileName);

            //
            // Parse xaml.
            // Diplay tree.
            // Verify tree. 
            //
            // Verification routine is called automatically when the tree
            // is rendered.
            //
            object root = SerializationHelper.ParseXamlFile(ActionHelper.XamlFileName);

            GlobalLog.LogStatus("xaml loaded. ");

            SerializationHelper helper = new SerializationHelper();
            helper.DisplayTree(root, "EventTrigger", true);

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        /******************************************************************************
        * Function:          GetPropertyTableEventTrigger
        ******************************************************************************/
        // Returns list of properties that can be used for EventTrigger on the given type.
        private Hashtable GetPropertyTableEventTrigger(string typeName, XamlTestDocument testDocument)
        {
            Type type = ActionHelper.GetType(typeName, true);
            Hashtable properties = new Hashtable();
            
            foreach (PropertyDescriptor prop in TypeDescriptor.GetProperties(type))
            {
                DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(prop);
                if (ActionHelper.IsStylableProperty(dpd))
                {
                    //Usually, the value those property depends on some other factor, not always the value set, thus
                    //cannot be used for trigger purpose. 
                    if (ActionForTypeHelper.ShouldIgnoreProperty(dpd.Name, type, ActionHelper.PropertiesToSkip))
                        continue;

                    object[] value = new object[2];
                    value[0] = ActionHelper.GetPropertyValue(testDocument, typeName, dpd);
                    if (value[0] == null)
                    {
                        GlobalLog.LogStatus("Value for: " + dpd.Name + ", which is of type : " + dpd.PropertyType.Name + ", is null. ");
                        //continue;
                    }

                    //Cannot set Template in Template. 
                    if (String.Equals(dpd.Name, "Template", StringComparison.InvariantCulture))
                        continue;
                    //Cannot set OverridesDefaultStyle in Template. 
                    if (String.Equals(dpd.Name, "OverridesDefaultStyle", StringComparison.InvariantCulture))
                        continue;
                    properties.Add(dpd, value);
                }
            }
            return properties;
        }
            
        /******************************************************************************
        * Function:          BuildEventTriggerXaml
        ******************************************************************************/
        // Builds a new xaml file for the type to use for loading and verification.
        // The xaml file contains 5 instance declarations: 
        //      - StyledElement - has a Style set via property element syntax, and EventTriggers are under the style.
        //      - StyledElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - TemplatedElement - has a template set via property element syntax, and EventTriggers are under the template.
        //      - TemplatedElementMultiTrigger - same as above except the triggers are MultiTrigger.
        //      - elementWithInitialValues - has locally-set properties for comparison purpose.
        
        private void BuildEventTriggerXaml(string typeName, XamlTestDocument doc)
        {
            Type type = ActionHelper.GetType(typeName, true);

            //XmlNode testRootNode = null;

            Hashtable properties = TriggerActionHelper.TrimPropertyTableForStoryboard(GetPropertyTableEventTrigger(typeName, doc));
            ArrayList keysForProperty = new ArrayList();
            // Insert styled element with locally set value. 
            XmlElement containerNode = doc.GetContainerNode(type);
            XmlElement elementWithInitialValues = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithInitialValues", "elementWithInitialValues", type);
            TriggerActionHelper.SetInitialValues(doc, elementWithInitialValues, properties, keysForProperty, 0);
            GlobalLog.LogStatus("elementWithInitialValues generated.");

            // Insert element with of EventTrigger for FrameworkElement. 
            if (type.IsSubclassOf(typeof(FrameworkElement)) || type.Equals(typeof(FrameworkElement)))
            {
                XmlElement element = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithEventTrigger", "ElementWithEventTrigger", type);
                XmlElement triggersNode = doc.CreateElement("", type.Name + ".Triggers", element.NamespaceURI);
                element.InsertAfter(triggersNode, null);
                InsertEventTriggers(doc, triggersNode, properties, type, keysForProperty);
            }
            // Insert styled element with style composed of EventTrigger. 
            XmlElement styledElement = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithEventTriggerInStyle", "ElementWithEventTriggerInStyle", type);
            XmlElement styleTriggersNode = ActionHelper.InsertStyleTriggersNode(doc, styledElement);
            InsertEventTriggers(doc, styleTriggersNode, properties, type, keysForProperty);
            GlobalLog.LogStatus("templatedElement set.");

            // Insert Templated element with Template composed of EventTrigger for Control;
            if (type.IsSubclassOf(typeof(Control)) || type.Equals(typeof(Control)))
            {
                XmlElement templatedElement = TriggerActionHelper.InsertMainElement(doc, containerNode, "RootForElementWithEventTriggerInTemplate", "ElementWithEventTriggerInTemplate", type);

                XmlElement templateTriggersNode = ActionHelper.InsertTemplateTriggersNode(doc, templatedElement);
                InsertEventTriggers(doc, templateTriggersNode, properties, type, keysForProperty);
                GlobalLog.LogStatus("templatedElement set.");
            }

            //set verification routine. 
            doc.SetVerifierRoutine(this.GetType().GetMethod("VerifyEventTrigger"));

            // Save final xml to file.
            IOHelper.SaveTextToFile(IOHelper.ConvertXmlDocumentToStream(doc), ActionHelper.XamlFileName);
            GlobalLog.LogStatus("Xaml Generated : " + ActionHelper.XamlFileName + ".");
        }

        /******************************************************************************
        * Function:          TriggerAllEvents
        ******************************************************************************/
        private void TriggerAllEvents(FrameworkElement root)
        {
            //Trigger event on element with EventTrigger
            DependencyObject element = root.FindName("ElementWithEventTrigger") as DependencyObject;
            RaiseEvents(element);

            //Trigger event on element with EventTrigger in Style
            element = root.FindName("ElementWithEventTriggerInStyle") as DependencyObject; 
            RaiseEvents(element);

            //Trigger event on element with EventTrigger in Template
            element = root.FindName("ElementWithEventTriggerInTemplate") as DependencyObject;
            RaiseEvents(element);
        }

        /******************************************************************************
        * Function:          TriggerAllEvents
        ******************************************************************************/
        private void RaiseEvents(DependencyObject element)
        {
            UIElement ue = element as UIElement;
            ContentElement ce = element as ContentElement;
            RoutedEvent[] events = GetRoutedEvents();
            foreach (RoutedEvent current in events)
            {
                GlobalLog.LogStatus("Raise Event: " + current.OwnerType.Name + "." + current.Name + ".");
                Type handlerType = current.HandlerType;

                Type argsType = GetEventArgsType(handlerType.FullName);
                RoutedEventArgs args;
                try
                {
                    args = Activator.CreateInstance(argsType) as RoutedEventArgs;
                }
                catch (MissingMethodException)
                {
                    args = Activator.CreateInstance(argsType, new object[1] { current }) as RoutedEventArgs;
                }
                args.RoutedEvent = current;

                if (null != ue)
                {
                    ue.RaiseEvent(args);
                }
                else if (null != ce)
                {
                    ce.RaiseEvent(args);
                }
            }
        }

        /******************************************************************************
        * Function:          InsertEventTriggers
        ******************************************************************************/
        // Every property serves as Source for a EventTrigger to trig something to change the value of next available 
        // property in the list. In the case actionsName is null, a Setter will be used. Otherwise, storyboard created. 
        // actionsName can be EnterActions and ExitActions
        private void InsertEventTriggers(XamlTestDocument doc, XmlElement triggersNode, Hashtable properties, Type ownerType, ArrayList keysForProperty)
        {
            XmlElement lastTriggerNode = null;
            RoutedEvent[] events = GetRoutedEvents();
            int numberOfEvent = events.Length;

            //For storyboard, there is no circle. Otherwise, the value cannot be re-set thus
            //Triggers are alway active.
            for (int eventIndex = 0; eventIndex < numberOfEvent; eventIndex++)
            {
                RoutedEvent routedEvent = events[eventIndex];
                XmlElement triggerNode = doc.CreateElement("EventTrigger", XamlGenerator.AvalonXmlns);
                triggersNode.InsertAfter(triggerNode, lastTriggerNode);
                lastTriggerNode = triggerNode;
                
                triggerNode.SetAttribute("RoutedEvent", routedEvent.OwnerType.Name + "." + routedEvent.Name);
                
                XmlElement triggerActions = doc.CreateElement("EventTrigger.Actions", XamlGenerator.AvalonXmlns);
                triggerNode.InsertAfter(triggerActions, null);
                GenerateEventTriggerActions(doc, triggerActions, events, properties, keysForProperty, eventIndex, ownerType);
            }
        }

        /******************************************************************************
        * Function:          TrimEvents
        ******************************************************************************/
        //Cannot use a event whose owner is not public.
        private RoutedEvent[] TrimEvents(RoutedEvent[] original)
        {
            ArrayList Trimed = new ArrayList();
            foreach (RoutedEvent current in original)
            {
                if (!(current.OwnerType.IsPublic))
                {
                    continue;
                }
                Type handlerType = current.HandlerType;

                Type argsType = GetEventArgsType(handlerType.FullName);
                if(null == argsType ||!argsType.IsPublic)
                {
                    continue;
                }
                //skip the event whose args doesn't have a public constructor. 
                ConstructorInfo constructor = argsType.GetConstructor(new Type[0]);
                if (null == constructor || !constructor.IsPublic)
                {
                    constructor = argsType.GetConstructor(new Type[1]{typeof(RoutedEvent)});
                    if (null == constructor || !constructor.IsPublic)
                        continue;
                }
                if(string.Equals(current.Name, "AccessKeyPressed", StringComparison.InvariantCulture)
                    && current.OwnerType.Equals(typeof(System.Windows.Input.AccessKeyManager))
                    )
                    continue;
                Trimed.Add(current);
            }
            return Trimed.ToArray(typeof(RoutedEvent)) as RoutedEvent[];
        }

        /******************************************************************************
        * Function:          GenerateEventTriggerActions
        ******************************************************************************/
        private void GenerateEventTriggerActions(XamlTestDocument doc, XmlElement actionsNode, RoutedEvent[] events, Hashtable properties, ArrayList keysForProperty, int eventIndex, Type ownerType)
        {
            int numberOfProperties = properties.Count;

            for (int propertyIndex = 0; propertyIndex < numberOfProperties; propertyIndex++)
            {
                if (propertyIndex % (events.Length) == eventIndex)
                {
                    InsertsBeginStoryboard(doc, actionsNode, properties, ownerType, keysForProperty, propertyIndex);
                }
            }
            if(eventIndex >= numberOfProperties)
            {
                InsertsBeginStoryboard(doc, actionsNode, properties, ownerType, keysForProperty, eventIndex % numberOfProperties);
            }
        }

        /******************************************************************************
        * Function:          InsertsBeginStoryboard
        ******************************************************************************/
        private void InsertsBeginStoryboard(XamlTestDocument doc, XmlElement actionsNode, Hashtable properties, Type ownerType, ArrayList keysForProperty, int propertyIndex)
        {
            DependencyPropertyDescriptor[] dpds = new DependencyPropertyDescriptor[properties.Count];
            properties.Keys.CopyTo(dpds, 0);
            DependencyPropertyDescriptor property = dpds[propertyIndex] as DependencyPropertyDescriptor;
            TriggerActionHelper.InsertsBeginStoryboard(doc, actionsNode, property, properties, ownerType, keysForProperty, 0);
        }
        
        /******************************************************************************
        * Function:          GetRoutedEvents
        ******************************************************************************/
        private RoutedEvent[] GetRoutedEvents()
        {
            Type globalEventManager = ActionHelper.GetType("System.Windows.GlobalEventManager", true);
            MethodInfo getRoutedEvents = globalEventManager.GetMethod("GetRoutedEvents", BindingFlags.Static | BindingFlags.NonPublic);
            return TrimEvents(getRoutedEvents.Invoke(null, null) as RoutedEvent[]);
        }

        /******************************************************************************
        * Function:          GetEventArgsType
        ******************************************************************************/
        private Type GetEventArgsType(string handlerName)
        {
            string argsName =string.Empty;
            Type argsType = null;

            if(handlerName.EndsWith("Handler"))
                argsName = handlerName.Substring(0, handlerName.Length - 7) + "Args";
            
            if(0!= argsName.Length)argsType= ActionHelper.GetType(argsName, true);
            if (null == argsType)
            {
                GlobalLog.LogStatus("Not found type: " + argsName + " for handler: " + handlerName + ".");
            }
            return argsType;
        }
        #endregion
            
        #region Verification
        /******************************************************************************
        * Function:          VerifyEventTrigger
        ******************************************************************************/
        /// <summary>
        /// Verifies preperty trigger after the first render.
        /// </summary>
        /// <param name="uielement"></param>
        public void VerifyEventTrigger(UIElement uielement)
        {
            GlobalLog.LogStatus("Verify EventTriggers...");
            FrameworkElement root = (FrameworkElement)uielement;
            DependencyObject elementWithInitialValues = root.FindName("elementWithInitialValues") as DependencyObject;
            DependencyObject firstElement = root.FindName("ElementWithEventTrigger") as DependencyObject;
            DependencyObject secondElement = root.FindName("ElementWithEventTriggerInStyle") as DependencyObject;
            DependencyObject thirdElement = root.FindName("ElementWithEventTriggerInTemplate") as DependencyObject;
            
            //Trigger all events
            TriggerAllEvents(root);

            //DispatcherHelper.DoEvents(0,DispatcherPriority.ApplicationIdle);
            DispatcherHelper.DoEvents(500);

            Type type = root.FindName("elementWithInitialValues").GetType();
            Hashtable properties = TriggerActionHelper.TrimPropertyTableForStoryboard(GetPropertyTableEventTrigger(type.FullName, s_xamlTestDocument));

            DependencyPropertyDescriptor[] propertyArray = new DependencyPropertyDescriptor[properties.Count];
            properties.Keys.CopyTo(propertyArray, 0);
            foreach (DependencyPropertyDescriptor dpd in propertyArray)
            {

                // Check property value is equivalent to locally-set property
                GlobalLog.LogStatus("Check property: " + dpd.Name + " on the first element.");
                if (null != firstElement)
                {
                    ActionForTypeHelper.CheckExpectedPropertyValue(firstElement, elementWithInitialValues, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
                }
                GlobalLog.LogStatus("On the second element.");
                ActionForTypeHelper.CheckExpectedPropertyValue(secondElement, elementWithInitialValues, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
                if (null != thirdElement)
                {
                    GlobalLog.LogStatus("On the third element.");
                    ActionForTypeHelper.CheckExpectedPropertyValue(thirdElement, elementWithInitialValues, dpd.DependencyProperty, ActionHelper.PropertiesToSkip);
                }
            }
        }
        #endregion
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model LocallyDefined.
 *          Construct trees, serialize them and verify.
 *
 
  
 * Revision:         $Revision:$
 
 * Filename:         $Source:$
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Threading;
using System.Windows.Markup;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Microsoft.Test.Threading;
using Avalon.Test.CoreUI.Parser;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Threading;
using Microsoft.Test.Logging;
using System.Windows.Controls;
using System.Reflection;
using Microsoft.Test.Markup;
using System.Xml;

using Avalon.Test.CoreUI.Serialization;
using Microsoft.Test;

namespace Avalon.Test.CoreUI.Parser.LocallyDefinedNamespace
{
    /// <summary>
    /// LocallyDefined code-based test suit with a Model. 
    /// </summary>
    [Model(@"FeatureTests\ElementServices\LocallyDefinedCases.xtc", 1, @"Parser\LocallyDefined", TestCaseSecurityLevel.FullTrust, "LocallyDefined model pairwise", SupportFiles=@"FeatureTests\ElementServices\LocallyDefinedTypes.vb,FeatureTests\ElementServices\LocallyDefinedTypes.cs,FeatureTests\ElementServices\locallyDefinedButtonXamlBasedCS.xaml,FeatureTests\ElementServices\locallyDefinedButtonXamlBasedVB.xaml,FeatureTests\ElementServices\TestPageCS.xaml,FeatureTests\ElementServices\TestPageVB.xaml")]
    public class LocallyDefinedModel : Model
    {

        /// <summary>
        /// Construct new instance of the model.
        /// </summary>
        public LocallyDefinedModel()
            : base()
        {
            Name = "untitled";
            Description = "LocallyDefined Model";

            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Single action for this model.  Creates Compile xaml and run the 
        /// application. Verify property and event
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunTest(State endState, State inParams, State outParams)
        {
            ModelState = new LocallyDefinedModelState(inParams);
            ModelState.LogState();
            XmlDocument newXaml = CreateXaml();
            string tempXamlFileName="___LocallyDefinedComponentXaml.xaml";
            CoreLogger.LogStatus("Saving xaml to : " + tempXamlFileName);
            newXaml.Save(tempXamlFileName);

            CompilerHelper compiler = new CompilerHelper();

            compiler.CleanUpCompilation();
            compiler.AddDefaults();

            List<string> extraFiles = new List<string>();

            extraFiles.Add("LocallyDefinedTypes." + "cs"/*ModelState.Language*/);
            
            string[] pages = { tempXamlFileName, "LocallyDefinedButtonXamlBased" +"CS" /*ModelState.Language*/ + ".xaml" };
            Languages language;
            //if (String.Equals(ModelState.Language, "VB")) language = Languages.VisualBasic;
            //else
                language = Languages.CSharp;
            compiler.CompileApp(pages, "Application", null, extraFiles, null, language);
             
            compiler.RunCompiledApp();

            return true;
        }

        /// <summary>
        /// Create the xaml according to the parameter read from xtc.
        /// </summary>
        private XmlDocument CreateXaml()
        {

            string elementPrefix = "l";
            string xmlns = "clr-namespace:LocallyDefined";
            //






            XmlDocument newXaml = new XmlDocument();
            string baseXamlName = "TestPage" + "CS"/*ModelState.Language*/ + ".xaml";

            newXaml.Load(baseXamlName);
            string elementName = "LocallyDefinedButton";
            if (String.Equals(ModelState.Defined, "xaml", StringComparison.InvariantCulture))
            {
                elementName += "XamlBased";
            }
            
            XmlElement rootNode = newXaml.DocumentElement;
            XmlElement fullElementNode = CreateElementNode(newXaml, elementPrefix, elementName, xmlns);
            XmlElement newNode = newXaml.CreateElement(elementPrefix, elementName, xmlns);
            XmlElement resourcesNode = newXaml.DocumentElement.FirstChild as XmlElement;
            XmlElement setterNode = null;
            XmlElement templateNode = null;
            XmlElement dotTemplateNode = null;
            XmlElement dotStyleNode = null;
            XmlElement styleNode = null;
            switch (ModelState.Location)
            {
                case "MainTree":
                    rootNode.InsertAfter(fullElementNode, resourcesNode);

                    if (String.Equals(ModelState.MultipleOccurrences, "Yes", StringComparison.InvariantCulture))
                    {
                        XmlElement colon = fullElementNode.Clone() as XmlElement;
                        rootNode.InsertAfter(colon, resourcesNode);
                    }
                    // We use LocallyDefinedButtonBase.Identifier property, instead of Name
                    // or x:Name, since x:Name doesn't always work for locally defined components
                    // See 
                    fullElementNode.SetAttribute("Identifier", "elementName");
                    rootNode.InsertAfter(fullElementNode, resourcesNode);
                    break;

                case "Resources":
                    resourcesNode.InsertAfter(fullElementNode, null);
                    fullElementNode.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, "elementInResource");
                    rootNode.InsertAfter(newNode, resourcesNode);
                    newNode.SetAttribute("Identifier", "elementHost");
                    newNode.SetAttribute("Content", "{" + ModelState.ResourceReference +" elementInResource}");
                    break;

                case "InlineTemplate":
                    templateNode = CreateTemplateNode(newXaml, elementPrefix, elementName, xmlns);
                    dotTemplateNode = newXaml.CreateElement(elementPrefix, elementName + ".Template", xmlns);
                    dotTemplateNode.InsertAfter(templateNode, null);
                    rootNode.InsertAfter(newNode, resourcesNode);
                    newNode.InsertAfter(dotTemplateNode, null);
                    newNode.SetAttribute("Identifier", "elementWithTemplate");
                    break;

                case "TemplateReferenceInResource":
                    templateNode = CreateTemplateNode(newXaml, elementPrefix, elementName, xmlns);
                    resourcesNode.InsertAfter(templateNode, null);
                    templateNode.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, "TemplateInResource");
                    newNode.SetAttribute("Identifier", "elementWithTemplate");
                    newNode.SetAttribute("Template", "{" + ModelState.ResourceReference +" TemplateInResource}");
                    rootNode.InsertAfter(newNode, resourcesNode);
                    break;

                case "TemplateInsideStyle":
                    templateNode = CreateTemplateNode(newXaml, elementPrefix, elementName, xmlns);
                    dotStyleNode = newXaml.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                    styleNode = CreateStyleNode(newXaml, elementPrefix, elementName);
                    dotStyleNode.InsertAfter(styleNode, null);
                    setterNode = newXaml.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    styleNode.InsertAfter(setterNode, null);
                    setterNode.SetAttribute("Property", "Template");
                    XmlNode setterValueNode = newXaml.CreateElement("Setter.Value", XamlGenerator.AvalonXmlns);
                    setterNode.InsertAfter(setterValueNode, null);
                    setterValueNode.InsertAfter(templateNode, null);

                    newNode.SetAttribute("Identifier", "elementWithTemplate");
                    newNode.InsertAfter(dotStyleNode, null);
                    rootNode.InsertAfter(newNode, resourcesNode);
                    break;

                case "ImplicitContentSyntax":
                    rootNode.InsertAfter(newNode, resourcesNode);
                    newNode.InsertAfter(fullElementNode, null);
                    fullElementNode.SetAttribute("Identifier", "elementName");
                    break;

                case "ExplicitComplexPropertySyntax":
                    rootNode.InsertAfter(newNode, resourcesNode);
                    XmlElement dotContentNode = newXaml.CreateElement("ContentControl.Content", XamlGenerator.AvalonXmlns);
                    newNode.InsertAfter(dotContentNode, null);
                    dotContentNode.InsertAfter(fullElementNode, null);
                    fullElementNode.SetAttribute("Identifier", "elementName");
                    break;

                default:
                    throw new Microsoft.Test.TestSetupException("Not implement Location: " + ModelState.Location + ".");
            }
            //Select verification routine.
            string verifier = "CoreTestsUntrusted.dll#Avalon.Test.CoreUI.Serialization.LocallyDefinedComponents.ModelVerifier";
            //should verify Background (effect of setter) besides locally defined property
            if (ModelState.PropertySet.Contains("Trigger"))
            {
                verifier += "PropertyTrigger";
            }
            //What to verify for event?
            if (ModelState.SetEventHandler.Contains("EventTrigger"))
            {
                verifier += "EventTrigger";
            }
            else if (ModelState.SetEventHandler.Contains("EventInTemplate"))
            {
                verifier += "EventHandler";
            }
            rootNode.SetAttribute("Verifier", verifier);
            return newXaml;
        }

        XmlElement CreateTemplateNode(XmlDocument doc, string elementPrefix, string elementName, string xmlns)
        {
            XmlElement templateNode = doc.CreateElement("ControlTemplate", XamlGenerator.AvalonXmlns);
            templateNode.SetAttribute("TargetType", "{x:Type " + elementPrefix + ":" + elementName + "}");
            
            XmlElement elementNode = CreateElementNode(doc, elementPrefix, elementName, xmlns);
            elementNode.SetAttribute("Name", XamlGenerator.AvalonXmlnsX, "elementInTemplate");
            templateNode.InsertAfter(elementNode, null);
            if (String.Equals(ModelState.MultipleOccurrences, "Yes", StringComparison.InvariantCulture))
            {
                XmlElement colon = elementNode.Clone() as XmlElement;
                colon.SetAttribute("Name", XamlGenerator.AvalonXmlnsX, "anotherElementInTemplate");
                elementNode.InsertAfter(colon, null);
            }
            return templateNode;
        }
        XmlElement CreateStyleNode(XmlDocument doc, string elementPrefix, string elementName)
        {
            XmlElement styleNode = doc.CreateElement("Style", XamlGenerator.AvalonXmlns);
            styleNode.SetAttribute("TargetType", "{x:Type " + elementPrefix + ":" + elementName + "}");
            return styleNode;
        }

        XmlElement CreateTemplateNode(XmlDocument doc, string elementPrefix, string elementName)
        {
            XmlElement templateNode = doc.CreateElement("ControlTemplate", XamlGenerator.AvalonXmlns);
            templateNode.SetAttribute("TargetType", "{x:Type " + elementPrefix + ":" + elementName + "}");
            return templateNode;
        }
        XmlElement CreateElementNode(XmlDocument doc, string elementPrefix, string elementName, string xmlns)
        {
            XmlElement elementNode = doc.CreateElement(elementPrefix, elementName, xmlns);
            XmlElement styleNode = null;
            XmlElement templateNode = null;
            XmlElement styleTriggersNode = null;
            XmlElement templateTriggersNode = null;
            XmlElement triggerNode = null;
            XmlElement setterNode = null;
            XmlElement dotStyleNode = null;
            XmlElement dotTemplateNode = null;
            XmlElement eventTriggerNode = null;
            XmlElement styleSetterNode = null;
            //Set property
            switch (ModelState.PropertySet)
            {
                case "LocalAttribute":
                    elementNode.SetAttribute("LocallyDefinedProperty", "LocallyDefinedValue");
                    break;
                case "LocalComplex":
                    XmlElement propertyNode = doc.CreateElement(elementPrefix, elementName+ ".LocallyDefinedProperty", xmlns);
                    propertyNode.InnerText = "LocallyDefinedValue";
                    elementNode.InsertAfter(propertyNode, null);
                    break;
                case "StyleSetter":
                    styleNode = CreateStyleNode(doc, elementPrefix, elementName);                    
                    styleSetterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    styleSetterNode.SetAttribute("Property", "LocallyDefinedProperty");
                    styleSetterNode.SetAttribute("Value", "LocallyDefinedValue");
                    styleNode.InsertAfter(styleSetterNode, null);                    
                    dotStyleNode = doc.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                    dotStyleNode.InsertAfter(styleNode, null);
                    elementNode.InsertAfter(dotStyleNode, null);
                    break;
                case "StyleTriggerSetter":
                    styleNode = CreateStyleNode(doc, elementPrefix, elementName);

                    //set property value to trigger 
                    styleSetterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    styleSetterNode.SetAttribute("Property", "Background");
                    styleSetterNode.SetAttribute("Value", "Red");
                    styleNode.InsertAfter(styleSetterNode, null);                    

                    styleTriggersNode = doc.CreateElement("Style.Triggers", XamlGenerator.AvalonXmlns);
                    styleNode.InsertAfter(styleTriggersNode, null);

                    triggerNode = doc.CreateElement("Trigger", XamlGenerator.AvalonXmlns);
                    triggerNode.SetAttribute("Property", "Background");
                    triggerNode.SetAttribute("Value", "Red");

                    setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    setterNode.SetAttribute("Property", "LocallyDefinedProperty");
                    setterNode.SetAttribute("Value", "LocallyDefinedValue");
                    triggerNode.InsertAfter(setterNode, null);

                    styleTriggersNode.InsertAfter(triggerNode, null);
                    
                    dotStyleNode = doc.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                    dotStyleNode.InsertAfter(styleNode, null);
                    elementNode.InsertAfter(dotStyleNode, null);
                    break;
                case "StyleTriggerCondition":
                    styleNode = CreateStyleNode(doc, elementPrefix, elementName);

                    //set property value to trigger 
                    styleSetterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    styleSetterNode.SetAttribute("Property", "LocallyDefinedProperty");
                    styleSetterNode.SetAttribute("Value", "LocallyDefinedValue");
                    styleNode.InsertAfter(styleSetterNode, null); 

                    styleTriggersNode = doc.CreateElement("Style.Triggers", XamlGenerator.AvalonXmlns);
                    styleNode.InsertAfter(styleTriggersNode, null);

                    triggerNode = doc.CreateElement("Trigger", XamlGenerator.AvalonXmlns);
                    triggerNode.SetAttribute("Property", "LocallyDefinedProperty");
                    triggerNode.SetAttribute("Value", "LocallyDefinedValue");

                    setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    setterNode.SetAttribute("Property", "Background");
                    setterNode.SetAttribute("Value", "Red");
                    triggerNode.InsertAfter(setterNode, null);
                    styleTriggersNode.InsertAfter(triggerNode, null);
                    dotStyleNode = doc.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                    dotStyleNode.InsertAfter(styleNode, null);
                    elementNode.InsertAfter(dotStyleNode, null);
                    break;
                case "TemplateTriggerCondition":
                    templateNode = CreateTemplateNode(doc, elementPrefix, elementName);

                    templateTriggersNode = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
                    templateNode.InsertAfter(templateTriggersNode, null);

                    triggerNode = doc.CreateElement("Trigger", XamlGenerator.AvalonXmlns);
                    triggerNode.SetAttribute("Property",  "LocallyDefinedProperty");
                    triggerNode.SetAttribute("Value", "LocallyDefinedValue");

                    setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);
                    setterNode.SetAttribute("Property", "Background");
                    setterNode.SetAttribute("Value", "Red");
                    triggerNode.InsertAfter(setterNode, null);
                    templateTriggersNode.InsertAfter(triggerNode, null);
                    
                    dotTemplateNode = doc.CreateElement(elementPrefix, elementName + ".Template", xmlns);
                    dotTemplateNode.InsertAfter(templateNode, null);
                    elementNode.InsertAfter(dotTemplateNode, null);
                    //set property to trigger 
                    elementNode.SetAttribute("LocallyDefinedProperty", "LocallyDefinedValue");
                    break;
                case "TemplateTriggerSetter":
                    templateNode = CreateTemplateNode(doc, elementPrefix, elementName);

                    templateTriggersNode = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
                    templateNode.InsertAfter(templateTriggersNode, null);

                    triggerNode = doc.CreateElement("Trigger", XamlGenerator.AvalonXmlns);

                    triggerNode.SetAttribute("Property", "Background");
                    triggerNode.SetAttribute("Value", "Red");
                    setterNode = doc.CreateElement("Setter", XamlGenerator.AvalonXmlns);

                    setterNode.SetAttribute("Property", "LocallyDefinedProperty");
                    setterNode.SetAttribute("Value", "LocallyDefinedValue");
                    triggerNode.InsertAfter(setterNode, null);
                    templateTriggersNode.InsertAfter(triggerNode, null);

                    dotTemplateNode = doc.CreateElement(elementPrefix, elementName + ".Template", xmlns);
                    dotTemplateNode.InsertAfter(templateNode, null);
                    elementNode.InsertAfter(dotTemplateNode, null);
                    //set property to trigger 
                    elementNode.SetAttribute("Background", "Red");
                    break;                    
                default:
                    throw new NotImplementedException(ModelState.PropertySet);
            }
            bool isInTemplate = false;
            //no event is allowed in a template. 
            if (ModelState.Location.Contains("Template"))
            {
                isInTemplate = true;
            }

            //Set EventHandler
            switch (ModelState.SetEventHandler)
            {
                case "LocalHandler":
                    elementNode.SetAttribute("LocallyDefinedEvent", "OnLocallyDefinedEvent");
                    break;
                case "EventInTemplate":
                    if (null == templateNode)
                    {
                        templateNode = CreateTemplateNode(doc, elementPrefix, elementName);
                        dotTemplateNode = doc.CreateElement(elementPrefix, elementName + ".Template", xmlns);
                        dotTemplateNode.InsertAfter(templateNode, null);
                        elementNode.InsertAfter(dotTemplateNode, null);
                    }
                    XmlElement innerElement = doc.CreateElement(elementPrefix, elementName , xmlns);
                    innerElement.SetAttribute("LocallyDefinedEvent", "OnLocallyDefinedEvent");
                    templateNode.InsertAfter(innerElement, null);
                    break;
                case "StyleEventSetter":
                    if (null == styleNode)
                    {
                        styleNode = CreateStyleNode(doc, elementPrefix, elementName);
                        dotStyleNode = doc.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                        dotStyleNode.InsertAfter(styleNode, null);
                        elementNode.InsertAfter(dotStyleNode, null);
                    }
                    XmlElement eventSetterNode = doc.CreateElement("EventSetter", XamlGenerator.AvalonXmlns);
                    eventSetterNode.SetAttribute("Event", "LocallyDefinedEvent");
                    eventSetterNode.SetAttribute("Handler", "OnLocallyDefinedEvent");
                    styleNode.InsertAfter(eventSetterNode, null);
                    break;
                case "EventTriggerInElementTrigger":
                    if (isInTemplate)
                    {
                        break;
                    }
                    eventTriggerNode = GetEventTrigger(doc, elementPrefix, elementName, xmlns);
                    XmlElement dotTriggersNode = doc.CreateElement(elementPrefix, elementName + ".Triggers", xmlns);
                    dotTriggersNode.InsertAfter(eventTriggerNode, null);
                    elementNode.InsertAfter(dotTriggersNode, null);
                    break;
                case "EventTriggerInStyle":
                    if (null == styleNode)
                    {
                        styleNode = CreateStyleNode(doc, elementPrefix, elementName);
                        dotStyleNode = doc.CreateElement(elementPrefix, elementName + ".Style", xmlns);
                        dotStyleNode.InsertAfter(styleNode, null);
                        elementNode.InsertAfter(dotStyleNode, null);
                    }
                    if (null == styleTriggersNode)
                    {
                        styleTriggersNode = doc.CreateElement("Style.Triggers", XamlGenerator.AvalonXmlns);
                        styleNode.InsertAfter(styleTriggersNode, null);
                    }
                    eventTriggerNode = GetEventTrigger(doc, elementPrefix, elementName, xmlns);
                    styleTriggersNode.InsertAfter(eventTriggerNode, null);
                    break;
                case "EventTriggerInTemplate":
                    if(null == templateNode)
                    {
                        templateNode = CreateTemplateNode(doc, elementPrefix, elementName);
                        dotTemplateNode = doc.CreateElement(elementPrefix, elementName + ".Template", xmlns);
                        dotTemplateNode.InsertAfter(templateNode, null);
                        elementNode.InsertAfter(dotTemplateNode, null);
                    }
                    if (null == templateTriggersNode)
                    {
                        templateTriggersNode = doc.CreateElement("ControlTemplate.Triggers", XamlGenerator.AvalonXmlns);
                        templateNode.InsertAfter(templateTriggersNode, null);
                    }
                    eventTriggerNode = GetEventTrigger(doc, elementPrefix, elementName, xmlns);
                    templateTriggersNode.InsertAfter(eventTriggerNode, null);
                    break;
                
                default:
                    throw new NotImplementedException(ModelState.SetEventHandler);
            }
            return elementNode;
        }

        //Create an EventTrigger with storyboard. 
        XmlElement GetEventTrigger(XmlDocument doc, string elementPrefix, string elementName, string xmlns)
        {
            XmlElement eventTriggerNode=doc.CreateElement("EventTrigger", XamlGenerator.AvalonXmlns);
            eventTriggerNode.SetAttribute("RoutedEvent",  elementPrefix + ":LocallyDefinedButton.LocallyDefinedEvent");
            XmlElement actionsNode=doc.CreateElement("EventTrigger.Actions", XamlGenerator.AvalonXmlns);
            eventTriggerNode.InsertAfter(actionsNode, null);

            XmlElement bStoryNode=doc.CreateElement("BeginStoryboard", XamlGenerator.AvalonXmlns);
            actionsNode.InsertAfter(bStoryNode, null);

            XmlElement storyNode=doc.CreateElement("Storyboard", XamlGenerator.AvalonXmlns);
            bStoryNode.InsertAfter(storyNode, null);
            
            XmlElement ptl=doc.CreateElement("ParallelTimeline", XamlGenerator.AvalonXmlns);
            ptl.SetAttribute("FillBehavior", "HoldEnd");
            ptl.SetAttribute("BeginTime", "0:0:0");
            storyNode.InsertAfter(ptl, null);
             
            XmlElement da = doc.CreateElement("DoubleAnimation", XamlGenerator.AvalonXmlns);
            da.SetAttribute("From", "300");
            da.SetAttribute("To", "300");
            da.SetAttribute("Storyboard.TargetProperty", "Width");
            da.SetAttribute("Duration", "0:0:0");
            ptl.InsertAfter(da, null);

            return eventTriggerNode;
        }
        /// <summary>
        /// ModelState.
        /// </summary>
        LocallyDefinedModelState ModelState
        {
            get
            {
                return _modelState;
            }
            set
            {
                _modelState = value;
            }
        }

        LocallyDefinedModelState _modelState = null;       
    }

    /// <summary>
    /// LocallyDefinedModelState inherits CoreModelState and 
    /// holds the parameters from the Model, as well as a LogState function 
    /// which print out the information about the correct state. 
    /// </summary>
    [Serializable()]
    class LocallyDefinedModelState : CoreModelState
    {
        public LocallyDefinedModelState(State state)
        {
            Location = state["Location"];
            Defined = state["Defined"];
            Consumed = state["Consumed"];
            PropertySet = state["PropertySet"];
            MultipleOccurrences = state["MultipleOccurrences"];
            SetEventHandler = state["EventHandler"];
            Language = state["Language"];
            NameSpaceMapping = state["NameSpaceMapping"];
            ResourceReference = state["ResourceReference"];
        }

        public override void LogState()
        {

            CoreLogger.LogStatus("  Location: " + Location +
                           "\r\n  Defined: " + Defined +
                           "\r\n  Consumed: " + Consumed +
                           "\r\n  PropertySet: " + PropertySet +
                           "\r\n  MultipleOccurrences: " + MultipleOccurrences +
                           "\r\n  SetEventHandler: " + SetEventHandler +
                           "\r\n  Language: " + Language +
                           "\r\n  NameSpaceMapping: " + NameSpaceMapping +
                           "\r\n  ResourceReference: " + ResourceReference
                           );
        }

        public string Location;
        public string Defined;
        public string Consumed;
        public string PropertySet;
        public string MultipleOccurrences;
        public string SetEventHandler;
        public string Language;
        public string NameSpaceMapping;
        public string ResourceReference;
    }

}

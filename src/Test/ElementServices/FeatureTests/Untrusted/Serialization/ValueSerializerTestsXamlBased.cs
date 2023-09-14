// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless MDE model ValueSerializer.
 *          Construct trees, serialize them and verify.
 *
 
  
 * Revision:         $Revision: 1 $
 
 *********************************************************************/
using System;
using System.Windows;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.IO;
using System.Collections;
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
using System.Xml;

namespace Avalon.Test.CoreUI.Serialization.Converter
{
    /// <summary>
    /// ValueSerializer code-based test suit with a Model. 
    /// </summary>
    /// 
    [TestCaseModel("ValueSerializerXamlBasedCases.xtc", "1", @"Serialization\ValueSerializer\XamlBased", TestCaseSecurityLevel.FullTrust, "ValueSerializer model pairwise: xaml-based tests")]
    [TestCaseSupportFile("ValueSerializerModel_empty.xaml")]
    public class ValueSerializerXamlBasedModel : Model 
    {

        /// <summary>
        /// Construct new instance of the model.
        /// </summary>
        public ValueSerializerXamlBasedModel() : base()
        {
            Name = "ValueSerializerXamlBasedModel";
            Description = "ValueSerializerXamlBased Model";

            //Add Action Handlers
            AddAction("RunTest", new ActionHandler(RunTest));
        }

        /// <summary>
        /// Single action for this model.  Constructs a tree based on
        /// the parameter combination; loads the tree, serialize the tree,
        /// and verify.
        /// </summary>
        /// <remarks>Handler for RunTest</remarks>
        /// <param name="endState">Expected end State object</param>
        /// <param name="inParams">Input action parameters object</param>
        /// <param name="outParams">Output action parameters object</param>
        /// <returns>false if errors</returns>
        private bool RunTest(State endState, State inParams, State outParams)
        {
            ModelState = new ValueSerializerXamlBasedModelState(inParams);
            ModelState.LogState();

            RunXamlBasedTest();
            return true;
        }

        // Handles xaml-based variations.
        private void RunXamlBasedTest()
        {
            // Xaml-based variations always go through round-tripping
            // or compile-and-run.
            // The trees and verification routines should be identical
            // to the code-only variations.
            Stream stream = CreateXaml();
            // Save the xml to a temporary file.

            string tempFileName = ".\\___TempXamlForValueSerializer.xaml";
            IOHelper.SaveTextToFile(stream, tempFileName);
            CoreLogger.LogStatus("Saved generated xaml to " + tempFileName);
            //throw new Exception();
            // Move the mouse out of the way before running the test so it doesn't trip any events unexpectedly.
            MouseHelper.MoveOnVirtualScreenMonitor();
            DispatcherHelper.DoEvents(2000);
            
            try
            {
                // round-tripping.
                SerializationHelper helper = new SerializationHelper();
                helper.XamlSerialized += new XamlSerializedEventHandler(_OnXamlSerialized);
                helper.RoundTripTest(stream, XamlWriterMode.Expression, true /*display*/);

                // Delete the temp xaml file since the test passed.
                File.Delete(tempFileName);
            }
            catch
            {
                // Save the xaml file for future analysis.
                TestLog.Current.LogFile(tempFileName);
                throw;
            }
            finally
            {
                stream.Close();
            }
        }
        /// <summary>
        /// Logs round trip status messages to CoreLogger.
        /// </summary>
        private static void _OnXamlSerialized(object sender, XamlSerializedEventArgs args)
        {
            // Save xaml to file for potential debugging.
            if (File.Exists(s_tempXamlFile))
            {
                File.Copy(s_tempXamlFile, s_tempXamlFile2, true);
            }

            IOHelper.SaveTextToFile(args.Xaml, s_tempXamlFile);
        }

        /// <summary>
        /// Create the object tree according to the parameter read from xtc.
        /// </summary>
        private Stream CreateXaml()
        {
            // Create main XmlDocument.
            XmlDocument basicXaml = new XmlDocument();
            basicXaml.Load("ValueSerializerModel_empty.xaml");

            //Get the nodes. 
            XmlElement outerContentControl = basicXaml.DocumentElement.FirstChild as XmlElement;
            XmlElement resourcesNode = outerContentControl.FirstChild as XmlElement;
            XmlElement innerContentControl = outerContentControl.LastChild as XmlElement;
            string typeWithValueSerializerName = null;

            //Need framework in Logical tree or Visual tree.
            if (String.Equals(ModelState.Location, "Normal", StringComparison.InvariantCulture)
               || ModelState.Location.EndsWith("VisualTree"))
            {
                typeWithValueSerializerName = "CustomFrameworkElement" + ModelState.DeclarationLocation;
            }
            else
            {
                typeWithValueSerializerName = "CustomElement" + ModelState.DeclarationLocation;
            }

            
            if(String.Equals(ModelState.Location, "Normal", StringComparison.InvariantCulture))
            {
                XmlNode newNode = basicXaml.CreateElement("Con", typeWithValueSerializerName, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                innerContentControl.InsertAfter(newNode, null);
            }
            else if(String.Equals(ModelState.Location, "NormalProperty", StringComparison.InvariantCulture))
            {
                SetProperty(typeWithValueSerializerName, innerContentControl, resourcesNode, basicXaml);
            }
            else if (ModelState.Location.StartsWith("Style"))
            {
                SetStyle(typeWithValueSerializerName, innerContentControl, resourcesNode, basicXaml);
            }
            else if (-1 != ModelState.Location.IndexOf("Template"))
            {
                SetTemplate(typeWithValueSerializerName, innerContentControl, resourcesNode, basicXaml);
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("Not implemented Location: " + ModelState.Location);
            }

            return IOHelper.ConvertXmlDocumentToStream(basicXaml);
            
        }

        void SetStyle(string elementName, XmlElement element, XmlElement resourcesNode, XmlDocument basicXaml)
        {
            XmlElement styleNode = basicXaml.CreateElement("Style", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            styleNode.SetAttribute("TargetType", "{x:Type Con:CustomElementWithCustomTypeProperties}");
            XmlElement triggersNode = null;
            XmlElement triggerNode = null;
            XmlElement triggerSetterNode = null;
           
            switch(ModelState.Location)
            {
                case "StyleSetterValue":
                    XmlElement styleSetterNode = basicXaml.CreateElement("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    styleSetterNode.SetAttribute("Property", elementName + ModelState.PropertyType);
                    styleNode.InsertAfter(styleSetterNode, null);

                    XmlElement styleSetterValueNode = basicXaml.CreateElement("Setter.Value", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    styleSetterNode.InsertAfter(styleSetterValueNode, null);

                    XmlElement newNode = basicXaml.CreateElement("Con", elementName, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                    newNode.SetAttribute("Value", "ValueString");
                    styleSetterValueNode.InsertAfter(newNode, null);
                    break;
                case "StyleTriggerSetterValue":
                    //adding Style.Triggers layer
                    triggersNode = basicXaml.CreateElement("Style.Triggers", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    styleNode.InsertAfter(triggersNode, null);

                    triggerNode = basicXaml.CreateElement("Trigger", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    triggerNode.SetAttribute("Property", "StringProperty");
                    triggerNode.SetAttribute("Value", "ValueString");
                    triggersNode.InsertAfter(triggerNode, null);

                    triggerSetterNode = basicXaml.CreateElement("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    triggerSetterNode.SetAttribute("Property", elementName + ModelState.PropertyType);
                    triggerSetterNode.SetAttribute("Value", "ValueString");
                    triggerNode.InsertAfter(triggerSetterNode, null);
                    break;
                case "StyleTriggerCondition":
                    //adding Style.Triggers layer
                    triggersNode = basicXaml.CreateElement("Style.Triggers", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    styleNode.InsertAfter(triggersNode, null);

                    triggerNode = basicXaml.CreateElement("Trigger", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    triggerNode.SetAttribute("Property", elementName + ModelState.PropertyType);
                    triggerNode.SetAttribute("Value", "ValueString");
                    triggersNode.InsertAfter(triggerNode, null);

                    triggerSetterNode = basicXaml.CreateElement("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                    triggerSetterNode.SetAttribute("Property", "StringProperty");
                    triggerSetterNode.SetAttribute("Value", "ValueString");
                    triggerNode.InsertAfter(triggerSetterNode, null);
                    break;

                default: break;
            }
            if (String.Equals(ModelState.IsResource, "Yes", StringComparison.InvariantCulture))
            {
                styleNode.SetAttribute("Key", "http://schemas.microsoft.com/winfx/2006/xaml", "style");
                resourcesNode.InsertAfter(styleNode, null);
                element.SetAttribute("Style", "{DynamicResource style}");
            }
            else if (String.Equals(ModelState.IsResource, "No", StringComparison.InvariantCulture))
            {

                XmlElement dotStyleNode = basicXaml.CreateElement("Con", "CustomElementWithCustomTypeProperties.Style", "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                element.InsertAfter(dotStyleNode, null);
                dotStyleNode.InsertAfter(styleNode, null);
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("ModelState.IsResource: " + ModelState.IsResource + " not implemented.");
            }
        }

        void SetTemplate(string elementName, XmlElement element, XmlElement resourcesNode, XmlDocument basicXaml)
        {
            XmlElement templateNode = basicXaml.CreateElement("ControlTemplate", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
            templateNode.SetAttribute("TargetType", "{x:Type Con:CustomElementWithCustomTypeProperties}");

            XmlElement triggerNode = null;
            XmlElement triggersNode = null;

            if(ModelState.Location.EndsWith("VisualTree"))
            {
                XmlElement newNode = basicXaml.CreateElement("Con", elementName, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                newNode.SetAttribute("Value", "ValueString");
                templateNode.InsertAfter(newNode, null);
            }
            else if(ModelState.Location.EndsWith("TriggerSetterValue"))
            {
                triggersNode = basicXaml.CreateElement("ControlTemplate" + ".Triggers", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                templateNode.InsertAfter(triggersNode, null);

                triggerNode = basicXaml.CreateElement("Trigger", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                triggerNode.SetAttribute("Property", "StringProperty");
                triggerNode.SetAttribute("Value", "ValueString");
                triggersNode.InsertAfter(triggerNode, null);

                XmlElement setterNode = basicXaml.CreateElement("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                setterNode.SetAttribute("Property", elementName + ModelState.PropertyType);
                triggerNode.InsertAfter(setterNode, null);

                XmlElement setterValueNode = basicXaml.CreateElement("Setter.Value", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                setterNode.InsertAfter(setterValueNode, null);

                XmlElement newNode = basicXaml.CreateElement("Con", elementName, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                newNode.SetAttribute("Value", "ValueString");
                setterValueNode.InsertAfter(newNode, null);
            }
            else if (ModelState.Location.EndsWith("TriggerCondition"))
            {
                triggersNode = basicXaml.CreateElement("ControlTemplate" + ".Triggers", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                templateNode.InsertAfter(triggersNode, null);

                triggerNode = basicXaml.CreateElement("Trigger", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                triggersNode.InsertAfter(triggerNode, null);
    
                triggerNode.SetAttribute("Property", elementName + ModelState.PropertyType);
                triggerNode.SetAttribute("Value", "ValueString");
                XmlElement triggerSetterNode = basicXaml.CreateElement("Setter", "http://schemas.microsoft.com/winfx/2006/xaml/presentation");
                triggerSetterNode.SetAttribute("Property", "StringProperty");
                triggerSetterNode.SetAttribute("Value", "ValueString");
                triggerNode.InsertAfter(triggerSetterNode, null);
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("Not implemented: " + ModelState.Location);
            }

            if (String.Equals(ModelState.IsResource, "Yes", StringComparison.InvariantCulture))
            {
                templateNode.SetAttribute("Key", "http://schemas.microsoft.com/winfx/2006/xaml", "template");
                resourcesNode.InsertAfter(templateNode, null);
                element.SetAttribute("Template", "{DynamicResource template}");
            }
            else if (String.Equals(ModelState.IsResource, "No", StringComparison.InvariantCulture))
            {

                XmlElement dotTemplateNode = basicXaml.CreateElement("Con", "CustomElementWithCustomTypeProperties.Template", "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
                element.InsertAfter(dotTemplateNode, null);
                dotTemplateNode.InsertAfter(templateNode, null);
            }
            else
            {
                throw new Microsoft.Test.TestSetupException("ModelState.IsResource: " + ModelState.IsResource + " not implemented.");
            }
        }
        
        /// <summary>
        /// Set property 
        /// </summary>
        /// <param name="elementName"></param>
        /// <param name="element"></param>
        /// <param name="resourcesNode"></param>
        /// <param name="basicXaml"></param>
        void SetProperty(string elementName, XmlElement element, XmlElement resourcesNode, XmlDocument basicXaml)
        {
            element.SetAttribute(elementName + ModelState.PropertyType, "ValueString");
            //XmlElement propertyNode = basicXaml.CreateElement(
            //    "Con", "CustomElementWithCustomTypeProperties." 
            //    + elementName 
            //    + ModelState.PropertyType, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
            //element.InsertAfter(propertyNode, null);
            //XmlElement valueNode = basicXaml.CreateElement(
            //    "Con", elementName, "clr-namespace:Avalon.Test.CoreUI.Serialization.Converter;assembly=CoreTestsUntrusted");
            //propertyNode.InsertAfter(valueNode, null);

            //valueNode.InnerText = "ValueString";
        }

        /// <summary>
        /// ModelState.
        /// </summary>
        ValueSerializerXamlBasedModelState ModelState
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

        ValueSerializerXamlBasedModelState _modelState = null;
        static string s_tempXamlFile = "___SerializationTempFile.xaml";
        static string s_tempXamlFile2 = "___SerializationTempFile2.xaml";
    }

    /// <summary>
    /// ValueSerializerXamlBasedModelState inherits CoreModelState and 
    /// holds the parameters from the Model, as well as a LogState function 
    /// which print out the information about the correct state. 
    /// </summary>
    [Serializable()]
    class ValueSerializerXamlBasedModelState : CoreModelState
    {
        public ValueSerializerXamlBasedModelState(State state)
        {
            Location = state["Location"];
            PropertyType = state["PropertyType"];
            DeclarationLocation = state["DeclarationLocation"];
            IsResource = state["IsResource"];
        }

        public override void LogState()
        {
            CoreLogger.LogStatus("  Location: " + Location +
                           "\r\n  PropertyType: " + PropertyType +
                           "\r\n  DeclarationLocation: " + DeclarationLocation +
                           "\r\n  IsResource: " + IsResource);            
        }

        public string Location;
        public string PropertyType;
        public string DeclarationLocation;
        public string IsResource;
    }

}

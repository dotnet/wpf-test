// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Test hardware accelerated effects with xaml files.
 * Owner: Microsoft 
 * Contributors: Microsoft
 ********************************************************************/
using System;
using System.Windows;
using Microsoft.Test.Logging;
using Microsoft.Test.Modeling;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Discovery;
using System.Xml;
using System.Collections.Generic;
using Microsoft.Test.Markup;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Pict.MatrixOutput;
using System.IO;

namespace Microsoft.Test.Effects
{
    /// <summary>
    /// Model base test for Effects. Generate test cases with PICT and 
    /// route each test to EffectsXamlBasedTest.
    /// </summary>
    public class EffectsModel : PictModel
    {
        #region Private Data

        private const string xamlTemplateName = "XamlTemplate.xaml";
        private string _xamlFileName;
        
        #endregion


        #region Methods

        public override void RunVariationCollection(PairwiseTestCase[] variations, int firstVariationIndex, int lastVariationIndex)
        {
            if (firstVariationIndex != lastVariationIndex)
            {
                throw new NotSupportedException("Cannot run multiple test variations in one pass for Effects tests.");
            }

            PairwiseTestCase test = variations[firstVariationIndex];

            //Clone current properties.
            PropertyBag parameters = (PropertyBag)DriverState.DriverParameters.Clone();

            foreach (PairwiseTestParameter parameter in test.Parameters)
            {
                //Add parameters from test case into the property bag. 
                parameters[parameter.Name] = parameter.Value;
            }

            _xamlFileName = CreateXaml(parameters);

            //Add Xaml and Master properties. 
            parameters["Xaml"] = _xamlFileName;
            parameters["Master"] = _xamlFileName + ".png";

            EffectsXamlBasedTest effectTest = new EffectsXamlBasedTest();

            effectTest.RunTest(parameters);
        }

        /// <summary>
        /// If test case failed, log the xaml file. 
        /// </summary>
        public override void OnTestFailed()
        {
            if (File.Exists(_xamlFileName))
            {
                log.LogFile(_xamlFileName);
            }
        }

        /// <summary>
        /// Build xaml file based parameters for Effects and Elements. 
        /// </summary>
        /// <param name="parameters">parameters</param>
        /// <returns>file name of the xaml generated.</returns>
        protected virtual string CreateXaml(PropertyBag parameters)
        {
            XmlNode gridElement = null;

            string xamlFileName = ModelFileName + (VariationIndex) + ".xaml";
            XmlDocument document = new XmlDocument();

            document.Load(xamlTemplateName);

            gridElement = document.GetElementsByTagName("Grid")[0];

            //parentNode is the node under which new element is added. 
            XmlNode parentNode = gridElement;

            bool isElementInViewport2DVisual3D = false;
            bool.TryParse(parameters["ElementInViewport2DVisual3D"], out isElementInViewport2DVisual3D);
            if (isElementInViewport2DVisual3D)
            {
                XmlElement viewport3dNode = document.GetElementsByTagName("Viewport3D")[0].Clone() as XmlElement;

                parentNode.AppendChild(viewport3dNode);
                XmlNode viewport2DVisual3DNode = viewport3dNode.GetElementsByTagName("Viewport2DVisual3D")[0];
                parentNode = viewport2DVisual3DNode;
            }

            // Add elements
            AddElements(document, parameters, parentNode);

            //clean helper segments
            XmlNode helpers = document.GetElementsByTagName("HelperXamlSegments")[0];
            helpers.ParentNode.RemoveChild(helpers);

            document.Save(xamlFileName);
            return xamlFileName;
        }

        /// <summary>
        /// Add elements to the object tree. 
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="parameters">parameters</param>
        /// <param name="parentNode">the element under which the elements are going to add</param>
        private void AddElements(XmlDocument document, PropertyBag parameters, XmlNode parentNode)
        {
            XmlElement mainElement = null;
            XmlElement nestedElement = null;

            mainElement = CreateElement(document, parameters, true);
            mainElement.SetAttribute("Name", "MainElement");

            AddEffect(document, mainElement, parameters, true);

            ConditionallyAddProperty(document, mainElement, "RenderTransform", parameters);
            ConditionallyAddProperty(document, mainElement, "OpacityMask", parameters);
            ConditionallyAddProperty(document, mainElement, "Clip", parameters);
            ConditionallyAddProperty(document, mainElement, "BitmapEffectInput", parameters);

            parentNode.AppendChild(mainElement);

            //Add second element if WithNesting is specified to be true.
            if (bool.Parse(parameters["UsingNesting"]))
            {
                nestedElement = CreateElement(document, parameters, false);
                nestedElement.SetAttribute("Name", "ChildElement");
                AddEffect(document, nestedElement, parameters, false);

                mainElement.AppendChild(nestedElement);
            }
        }

        /// <summary>
        /// Add property only when "Using{Proptery}" paramter from the model is True.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="element"></param>
        /// <param name="propertyName"></param>
        /// <param name="parameters"></param>
        /// <returns>value node added</returns>
        protected XmlElement ConditionallyAddProperty(XmlDocument document, XmlElement element, string propertyName, PropertyBag parameters)
        {
            bool hasProperty = false;
            bool.TryParse(parameters["Using" + propertyName], out hasProperty);
            if (hasProperty)
            {
                return AddProperty(document, element, propertyName, parameters);
            }
            return null;
        }


        /// <summary>
        /// Add a complex property.
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="element">element to add the property</param>
        /// <param name="propertyName"></param>
        /// <param name="parameters"></param>
        /// <returns>value node added. </returns>
        protected XmlElement AddProperty(XmlDocument document, XmlElement element, string propertyName, PropertyBag parameters)
        {
            XmlElement propertyElement = null;
            XmlElement valueElement = null;

            propertyElement = document.CreateElement(string.Format("{0}.{1}", element.Name, propertyName), XamlGenerator.AvalonXmlns);
            element.AppendChild(propertyElement);

            XmlNodeList nodes = document.GetElementsByTagName(string.Format("{0}PropertyValues", propertyName));
            if (nodes == null || nodes.Count < 1)
            {
                throw new TestValidationException(string.Format("{0}PropertyValues not found.", propertyName));
            }

            XmlElement valuesElement = nodes[0] as XmlElement;

            //in the case property has not been specified, use the first value.
            string valueStr = parameters[propertyName];
            if (string.IsNullOrEmpty(valueStr))
            {
                valueElement = valuesElement.ChildNodes[0] as XmlElement;
            }
            else
            {
                nodes = valuesElement.GetElementsByTagName(valueStr);

                if (nodes == null || nodes.Count < 1)
                {
                    throw new TestValidationException(string.Format("value: {0} for {1} not found.", valueStr, propertyName));
                }

                valueElement = nodes[0] as XmlElement;
            }
            //add propertyvalue element under the property element
            XmlElement elementToAdd = valueElement.CloneNode(true) as XmlElement;
            propertyElement.AppendChild(elementToAdd);
            return elementToAdd;
        }

        /// <summary>
        /// Add Effects as specified in the State. For main element and child 
        /// element, different element is added. 
        /// </summary>
        /// <param name="document">document of main xaml</param>
        /// <param name="element">element to add the effects</param>
        /// <param name="parameters">State from model</param>
        /// <param name="isMainElement">whether this is the main element</param>
        private void AddEffect(XmlDocument document, XmlElement element, PropertyBag parameters, bool isMainElement)
        {
            XmlElement effectValueElement = CreateEffectValueElement(document, parameters, isMainElement);
            XmlElement resourcesElement = document.GetElementsByTagName("ResourceDictionary")[0] as XmlElement;

            //effect property name : BitmapEffect for v1, and Effect for hw accelerated. 
            string effectPropertyName = "BitmapEffect";
            if (!string.IsNullOrEmpty(parameters["EffectPropertyName"]))
            {
                effectPropertyName = parameters["EffectPropertyName"];
            }

            string effectDefinitionLocation = parameters["EffectDefinitionLocation"];
            if (string.Compare(effectDefinitionLocation, "ResourceDictionary", StringComparison.InvariantCultureIgnoreCase) == 0)
            {
                string key = (isMainElement ? "Main" : "Second") + "Effect";
                element.SetAttribute(effectPropertyName, "{StaticResource " + key + "}");
                resourcesElement.AppendChild(effectValueElement);
                effectValueElement.SetAttribute("Key", XamlGenerator.AvalonXmlnsX, key);
            }
            else
            {
                XmlElement effectPropertyNode = document.CreateElement(element.Name + "." + effectPropertyName, XamlGenerator.AvalonXmlns);
                element.AppendChild(effectPropertyNode);
                effectPropertyNode.AppendChild(effectValueElement);
            }
        }

        /// <summary>
        /// Created an XmlElement representing the value of the Effect. The effect could be a complex one 
        /// in a BitmapEffectGroup.
        /// </summary>
        /// <param name="document"></param>
        /// <param name="parameters"></param>
        /// <param name="isMainElement"></param>
        /// <returns></returns>
        private XmlElement CreateEffectValueElement(XmlDocument document, PropertyBag parameters, bool isMainElement)
        {
            XmlElement effectValueElement = null;
            string effectType = isMainElement ? parameters["EffectType"] : parameters["SecondEffectType"];
            int numberOfEffects = int.Parse(parameters["NumberOfEffectsInGroup"]);
            bool inGroup = bool.Parse(parameters["InGroup"]);

            if (!inGroup)
            {
                effectValueElement = CreateSingleEffectValue(document, effectType, parameters);
            }
            else
            {
                effectValueElement = document.CreateElement("BitmapEffectGroup", XamlGenerator.AvalonXmlns);

                for (int i = 0; i < 1; i++)
                {
                    effectValueElement.AppendChild(CreateSingleEffectValue(document, effectType, parameters));
                }
            }
            return effectValueElement;
        }

        /// <summary>
        /// Create a XmlElement represent a simple effect value. 
        /// </summary>
        /// <param name="document"></param>
        /// <param name="effectType"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        private XmlElement CreateSingleEffectValue(XmlDocument document, string effectType, PropertyBag parameters)
        {
            XmlElement effectElement = document.CreateElement(effectType, XamlGenerator.AvalonXmlns);

            //add parameters for effect. 
            switch (effectType)
            {
                case "BlurEffect":
                    if (parameters.ContainsProperty("UsingRenderingBias") && bool.Parse(parameters["UsingRenderingBias"]))
                    {
                        effectElement.SetAttribute("RenderingBias", parameters["RenderingBias"]);
                    }
                    SetCommonBlurAttribute(effectElement, parameters);
                    break;

                case "BlurBitmapEffect":
                    SetCommonBlurAttribute(effectElement, parameters);
                    break;

                case "DropShadowBitmapEffect":

                    if (parameters.ContainsProperty("UsingDropShadowSoftness") && bool.Parse(parameters["UsingDropShadowSoftness"]))
                    {
                        effectElement.SetAttribute("Softness", parameters["DropShadowSoftness"]);
                    }
                    SetCommonDropShadowAttribute(effectElement, parameters);
                    break;

                case "DropShadowEffect":

                    if (parameters.ContainsProperty("UsingDropShadowBlurRadius") && bool.Parse(parameters["UsingDropShadowBlurRadius"]))
                    {
                        effectElement.SetAttribute("BlurRadius", parameters["DropShadowBlurRadius"]);
                    }
                    if (parameters.ContainsProperty("UsingRenderingBias") && bool.Parse(parameters["UsingRenderingBias"]))
                    {
                        effectElement.SetAttribute("RenderingBias", parameters["RenderingBias"]);
                    }
                    SetCommonDropShadowAttribute(effectElement, parameters);
             
                    break;
                //just default
                case "EmbossBitmapEffect":
                    break;

                default:
                    throw new TestValidationException("Not implemented type: " + parameters["EffectType"]);
            }
            return effectElement;
        }

        /// <summary>
        /// Set attributes that are common for DropShadowEffect and DropShadowBitmapEffect
        /// </summary>
        /// <param name="effectElement"></param>
        /// <param name="parameters"></param>
        private void SetCommonDropShadowAttribute(XmlElement effectElement, PropertyBag parameters)
        {
            if (parameters.ContainsProperty("UsingDropShadowColor") && bool.Parse(parameters["UsingDropShadowColor"]))
            {
                effectElement.SetAttribute("Color", parameters["DropShadowColor"]);
            }

            if (parameters.ContainsProperty("UsingDropShadowDirection") && bool.Parse(parameters["UsingDropShadowDirection"]))
            {
                effectElement.SetAttribute("Direction", parameters["DropShadowDirection"]);
            }
            if (parameters.ContainsProperty("UsingDropShadowShadowDepth") && bool.Parse(parameters["UsingDropShadowShadowDepth"]))
            {
                effectElement.SetAttribute("ShadowDepth", parameters["DropShadowShadowDepth"]);
            }

            if (parameters.ContainsProperty("UsingDropShadowOpacity") && bool.Parse(parameters["UsingDropShadowOpacity"]))
            {
                effectElement.SetAttribute("Opacity", parameters["DropShadowOpacity"]);
            }
        }

        /// <summary>
        /// Set Radius and KernelType properties, which are common for BlurEffect and BlurBitmapEffect
        /// </summary>
        /// <param name="effectElement"></param>
        /// <param name="parameters"></param>
        private void SetCommonBlurAttribute(XmlElement effectElement, PropertyBag parameters)
        {
            if (parameters.ContainsProperty("UsingRadius") && bool.Parse(parameters["UsingRadius"]))
            {
                effectElement.SetAttribute("Radius", parameters["Radius"]);
            }
            if (parameters.ContainsProperty("UsingKernelType") && bool.Parse(parameters["UsingKernelType"]))
            {
                effectElement.SetAttribute("KernelType", parameters["KernelType"]);
            }
        }


        /// <summary>
        /// Create a XmlElement with attributes. 
        /// </summary>
        /// <param name="document">document</param>
        /// <param name="parameters">parameters</param>
        /// <param name="isMainElement"></param>
        /// <returns></returns>
        private XmlElement CreateElement(XmlDocument document, PropertyBag parameters, bool isMainElement)
        {
            //inner element is smaller and with different color
            double sizeFactor = isMainElement ? 1 : 0.5;
            string elementBackgroundColor = isMainElement ? "Blue" : "Red";

            XmlElement element = document.CreateElement(parameters["ElementType"], XamlGenerator.AvalonXmlns);
            double size = (double.Parse(parameters["ElementSize"])) * sizeFactor;
            element.SetAttribute("Width", size.ToString());
            element.SetAttribute("Height", size.ToString());
            element.SetAttribute("Opacity", parameters["Opacity"]);

            //Add Background only if specified. 
            if (parameters.ContainsProperty("ElementWithBackground") && bool.Parse(parameters["ElementWithBackground"]))
            {
                element.SetAttribute("Background", elementBackgroundColor);
            }
            return element;
        }

        #endregion
    }
}
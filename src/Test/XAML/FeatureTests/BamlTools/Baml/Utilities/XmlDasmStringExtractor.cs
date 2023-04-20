// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Linq;

namespace Microsoft.Test.Baml.Utilities
{
    public class XmlDasmStringExtractor
    {
        string _xmlFilePath;
        List<BamlString> _extractedStrings;
        string _locAttributeId = "DUMMYVALUE"; //Prevent false positives

        public XmlDasmStringExtractor(string xmlFilePath)
        {
            this._xmlFilePath = xmlFilePath;
            _extractedStrings = new List<BamlString>();
        }

        public List<BamlString> ExtractedStrings
        {
            get { return _extractedStrings; }
        }

        public void ExtractStrings()
        {
            XElement bamlElement = XElement.Load(_xmlFilePath);
            XElement docElement = bamlElement.Element("Document");

            HandleElement(docElement.Element("Element"));
        }

        private void HandleElement(XElement element)
        {
            string uid = "";
            bool hasDefAttribute = false;
            Dictionary<string, string> locAttributes = null;

            //Check for DefAttribute (x:Uid)
            XElement defAttribute = element.Element("DefAttribute");
            if (defAttribute != null)
            {
                hasDefAttribute = true;
                uid = (string)defAttribute.Attribute("Value");
            }

            var attributeInfos = from item in element.Elements("AttributeInfo") //Allows for discovering LocAttributes
                                  where (string)item.Attribute("OwnerTypeId")
                                  == "(-379):System.Windows.Localization"
                                  && ((string)item.Attribute("NewAttributeValue"))
                                  == "Attributes"
                                  select item;
            foreach (XElement attributeInfoElement in attributeInfos)
            {
                _locAttributeId = String.Format("({0}):Attributes", attributeInfoElement.Attribute("NewAttributeId").Value);
            }

            var locAttributeSet = from item in element.Elements("PropertyWithConverter") 
                                  where (string)item.Attribute("ConverterTypeId")
                                  == "(-615):System.ComponentModel.StringConverter"
                                  && ((string)item.Attribute("AttributeId")).Contains(_locAttributeId)
                                  select item;

            foreach (XElement attributeElement in locAttributeSet)
            {
                if (locAttributes == null)
                {
                    locAttributes = ProcessLocalizationAttributes(attributeElement);
                }
            }

            //Get properies of type string from attributes
            IEnumerable<XElement> attributeStringElements = from item in element.Elements("PropertyWithConverter")
                                 where (string)item.Attribute("ConverterTypeId")
                                 == "(-615):System.ComponentModel.StringConverter"
                                 && !((string)item.Attribute("AttributeId")).Contains(_locAttributeId) //Filter out localization attributes
                                 && !((string)item.Attribute("AttributeId")).Contains("Comments") //Filter out Comments
                                 select item;

            //Property Element Values are stored as Text elements
            IEnumerable<XElement> textElements = from item in element.Elements("Text")
                                                 select item;

            List<XElement> stringElements = new List<XElement>(attributeStringElements);
            stringElements.AddRange(textElements);

            foreach (XElement stringElement in stringElements)
            {
                //Determine localizability
                bool isLocalizable = hasDefAttribute;
                string attributeId = ""; // attributeId will remain "" for PE syntax
                if (stringElement.Name == "PropertyWithConverter") // For attributes only
                {
                    attributeId = stringElement.Attribute("AttributeId").Value;
                    string propName = attributeId.Substring(attributeId.IndexOf(':') + 1);
                    if (locAttributes != null)
                    {
                        if (locAttributes.ContainsKey(propName))
                        {
                            isLocalizable &= EvaluateLocalizability(locAttributes[propName]);
                        }
                    }
                }

                _extractedStrings.Add(new BamlString(isLocalizable,
                                                    attributeId,
                                                    uid,
                                                    stringElement.Attribute("Value").Value));
            }

            //Handle complex properties
            var propertyElements = element.Elements("PropertyComplex");
            if (propertyElements != null)
            {
                foreach (XElement child in propertyElements)
                {
                    HandleElement(child);
                }
            }

            //Handle DeferableContent
            var defContentElements = element.Elements("DeferableContent");
            if (defContentElements != null)
            {
                foreach (XElement child in defContentElements)
                {
                    HandleElement(child);
                }
            }

            //Handle IDictionary
            var dictionaryElements = element.Elements("PropertyIDictionary");
            if (dictionaryElements != null)
            {
                foreach (XElement child in dictionaryElements)
                {
                    HandleElement(child);
                }
            }

            //Handle child elements
            var childElements = element.Elements("Element");
            if (childElements != null)
            {
                foreach (XElement child in childElements)
                {
                    HandleElement(child);
                }
            }

        }

        private bool EvaluateLocalizability(string attributeValue)
        {
            attributeValue = attributeValue.ToUpperInvariant();
            return !(attributeValue.Contains("UNREADABLE") || attributeValue.Contains("UNMODIFIABLE"));
        }

        private Dictionary<string, string> ProcessLocalizationAttributes(XElement locAttributeSet)
        {
            Dictionary<string, string> locAttributes = new Dictionary<string, string>();
            string pattern = @"([a-zA-Z]+)\s*\(?([a-zA-Z]+)\s([a-zA-Z]+)\)?"; //works with pattern PropName(Value Value)
            Regex regex = new Regex(pattern);
            MatchCollection matches = regex.Matches((string)locAttributeSet.Attribute("Value"));

            foreach (Match m in matches)
            {
                string attribute = m.Value;
                string propertyName = attribute.Substring(0, attribute.IndexOf('(')).Trim();
                string attributeValue = attribute.Substring(attribute.IndexOf('('));
                locAttributes.Add(propertyName, attributeValue);
            }
            return locAttributes;
        }
    }
}

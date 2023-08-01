// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#region Using directives
using System;
using System.Windows;
using System.Threading;
using System.Windows.Media;
using System.Windows.Controls;
using System.Windows.Markup;
using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
//using Avalon.Test.CoreUI.PropertyEngine;
using Microsoft.Test.Modeling;
using System.Windows.Media.Animation;
using System.Diagnostics;
using System.Windows.Input;
using System.Xml;
using System.IO;
using System.Text;
using Microsoft.Test.Serialization;
#endregion

namespace Avalon.Test.CoreUI.Resources
{
    static class ResourcesModelXamlHelper
    {
        public static Stream GenerateXaml(ResourcesModelState modelState)
        {
            s_state = modelState;
            s_resolutionStepCount = s_remainingCount = s_state.ResolutionSteps;

            XmlDocument tempDoc = new XmlDocument();
            tempDoc.Load("ResourcesModel_empty.xaml");
            s_mainDoc = tempDoc;
            tempDoc = new XmlDocument();
            tempDoc.Load("ResourcesModel_elements.xaml");
            s_elementsDoc = tempDoc;

            // Grab Root.
            s_rootRDNode = s_mainDoc.SelectSingleNode("//*[@Name='TestRoot']");
            Debug.Assert(s_rootRDNode != null, "rootRDNode cannot be null");

            // Add Three Stack Panels.
            XmlNode innermostNode = AddThreeStackPanel();

            // Get First Node that uses Resource.
            XmlNode newNode = GetUniqueNameNode(GetElementName());

            // Get Prefix for resource key to be used.
            s_resourceKeyPrefix = FindResourceKeyPrefix(newNode);

            // Adjust the references Depending on what the reference type and markup syntax for the element is.
            AdjustReferenceTypeAndName(
                newNode,
                s_state.ReferenceType == "AllDynamic" || s_state.ReferenceType == "AlternateDynamicFirst",
                s_state.MarkupSyntax == "PropertyElement",
                s_state.ResolutionSteps);

            // Add node with resource in deepest nested test panel.
            ImportAndInsertNode(innermostNode, newNode, false);

            // Set Ancestor Nodes.
            ValidatePresentparentAndGrandParentRDNodes(false);
            SetPresentparentAndGrandParentRDNodes(s_rootRDNode);
            ValidatePresentparentAndGrandParentRDNodes(true);
            SetApplicationRDNode();

            SetResourcesIntoRDs();

            // Add Application Resource Dictionary if required.
            if (s_applicationRDNode.ChildNodes.Count > 0)
            {
                s_additionalAppMarkup = FormatElement(s_applicationRDNode.FirstChild, 4);
            }

            // Save the XAML.
            return IOHelper.ConvertXmlDocumentToStream(s_mainDoc);
        }

        private static string GetElementName()
        {
            if (s_state.ResourceUseLocation == "Inline")
            {
                return "Inline_" + s_state.InlineOwnerAndResourceType;
            }
            else
            {
                return s_state.ResourceUseLocation;
            }
        }


        private static XmlNode GetUniqueResourceNode(string keyName)
        {
            XmlDocument sourceDocument = s_elementsDoc;
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
            nsMgr.AddNamespace("x", "http://schemas.microsoft.com/winfx/2006/xaml");
            Debug.Assert(sourceDocument.SelectNodes(string.Format("//*[@x:Key='{0}']", keyName), nsMgr).Count == 1, "Check the source for Key of " + keyName);
            return sourceDocument.SelectSingleNode(string.Format("//*[@x:Key='{0}']", keyName), nsMgr);
        }

        private static XmlNode GetUniqueNameNode(string name)
        {
            XmlDocument sourceDocument = s_elementsDoc;
            XmlNamespaceManager nsMgr = new XmlNamespaceManager(new NameTable());
            Debug.Assert(sourceDocument.SelectNodes(string.Format("//*[@Name='{0}']", name), nsMgr).Count == 1, "Check the source for Name of " + name);
            return sourceDocument.SelectSingleNode(string.Format("//*[@Name='{0}']", name), nsMgr);
        }

        // Clones a sub-tree from one XmlDocument instance
        // to another.
        private static XmlNode ImportAndInsertNode(XmlNode firstNode, XmlNode secondNode, bool usePrepend)
        {
            Debug.Assert(firstNode != null, "firstNode must be non-null");
            Debug.Assert(secondNode != null, "secondNode must be non-null");

            // Import second node to first node's document.
            XmlNode newNode = firstNode.OwnerDocument.ImportNode(secondNode, true);

            // Insert newly-imported node under the first node.
            if (usePrepend)
            {
                firstNode.PrependChild(newNode);
            }
            else
            {
                firstNode.AppendChild(newNode);
            }

            return newNode;
        }

        private static string FindResourceKeyPrefix(XmlNode node)
        {
            s_resourceKeyPrefix = null;
            foreach (XmlAttribute attribute in node.Attributes)
            {
                if (attribute.Name == "Tag")
                {
                    s_resourceKeyPrefix = "TestKey_" + attribute.Value;
                    break;
                }
            }
            Debug.Assert(s_resourceKeyPrefix != null, "Must find resourceKeyPrefix");
            return s_resourceKeyPrefix;
        }

        /// <summary>
        /// Starting from rootRDNode, Add three StackPanels so that
        /// we have sufficient Resources sections for Present, parent
        /// and GrandParent. It may be more than necessary, but that is fine.
        /// </summary>
        /// <returns>the lowest StackPanel node</returns>
        private static XmlNode AddThreeStackPanel()
        {
            Debug.Assert(s_rootRDNode != null, "AddThreeStackPanel cannot be called when rootRDNode is NULL");
            XmlNode parentNode = s_rootRDNode;            

            for (int i = 1; i <= 3; i++)
            {
                XmlNode dpNode = GetUniqueNameNode("SP" + i);
                ImportAndInsertNode(parentNode, dpNode, false);
                parentNode = parentNode.LastChild;
            }

            return parentNode;
        }

        private static void AdjustReferenceTypeAndName(XmlNode element, bool isDynamicReference, bool isPropertyElementSyntax, int keyBasedOnStep)
        {
            Debug.Assert(keyBasedOnStep == -1 || keyBasedOnStep == 1 || keyBasedOnStep == 2 || keyBasedOnStep == 3,
                "Only valid values for keyBasedOnStep are -1, 1, 2, 3");

            //if keyBasedOnStep is -1, no need to adjust it. 

            if (element != null)
            {
                XmlAttributeCollection attributes = element.Attributes;

                for (int i = 0; attributes != null && i < attributes.Count; i++)
                {
                    XmlAttribute attribute = attributes[i];
                    string attName = attribute.Name;
                    string attValue = attribute.Value;
                    if (attValue.StartsWith("{StaticResource ") || attValue.StartsWith("{ StaticResource "))
                    {
                        if (attValue.StartsWith("{ StaticResource "))
                        {
                            if (isDynamicReference)
                            {
                                isDynamicReference = false;
                                Console.WriteLine("To Remove: Force StaticResource.");
                            }
                        }

                        //Adjust it based on parameter info
                        if (keyBasedOnStep != -1)
                        {
                            Debug.Assert(attValue.Contains("1"), "Must Contains 1. Check source for misspelling.");

                            attValue = attValue.Replace("1", keyBasedOnStep.ToString());
                        }
                        if (isDynamicReference)
                        {
                            attValue = attValue.Replace("StaticResource", "DynamicResource");
                        }
                        attribute.Value = attValue;

                        if (isPropertyElementSyntax)
                        {
                            /*The goal is to change
                <Setter Property="Background" Value="{StatucResource TestKey1_gscb}">
                            
                <Setter Property="Background">
                    <Setter.Value>
                        <StaticResource ResourceKey="TestKey1_gscb"/>
                    </Setter.Value>
                </Setter>
                             * */

                            XmlDocument sourceDocument = s_elementsDoc;

                            element.Attributes.Remove(attribute);
                            XmlElement newElement = sourceDocument.CreateElement(element.Name + "." + attribute.Name, element.NamespaceURI);
                            element.AppendChild(newElement);

                            XmlElement resourceElement = null;
                            if (isDynamicReference)
                            {
                                resourceElement = sourceDocument.CreateElement("DynamicResource", element.NamespaceURI);
                            }
                            else
                            {
                                resourceElement = sourceDocument.CreateElement("StaticResource", element.NamespaceURI);
                            }
                            XmlAttribute resourceAttribute = sourceDocument.CreateAttribute("ResourceKey");
                            string attValueString = attribute.Value;
                            if (attValueString.StartsWith("{ "))
                            {
                                attValueString = attValueString.Remove(1, 1);
                            }
                            string[] breakupStrings = attValueString.Split(' ', '}');
                            Debug.Assert(breakupStrings.Length == 3, "Should be divided into 3");
                            resourceAttribute.Value = breakupStrings[1];
                            resourceElement.Attributes.Append(resourceAttribute);
                            newElement.AppendChild(resourceElement);
                        }
                        break;
                    }
                }

                if (element.ChildNodes.Count != 0)
                {
                    foreach (XmlNode childNode in element.ChildNodes)
                    {
                        AdjustReferenceTypeAndName(childNode, isDynamicReference, isPropertyElementSyntax, keyBasedOnStep);
                    }

                }
            }
        }

        private static void ValidatePresentparentAndGrandParentRDNodes(bool afterSet)
        {
            Debug.Assert(s_rootRDNode != null, "");

            if (!afterSet)
            {
                Debug.Assert(s_presentRDNode == null, "");
                Debug.Assert(s_parentRDNode == null, "");
                Debug.Assert(s_grandParentRDNode == null, "");
            }
            else
            {
                Debug.Assert(s_presentRDNode != null, "");
                Debug.Assert(s_parentRDNode != null, "");
                Debug.Assert(s_grandParentRDNode != null, "");
                Debug.Assert(s_grandParentRDNode != s_rootRDNode, "");
            }
        }

        private static void SetPresentparentAndGrandParentRDNodes(XmlNode current)
        {
            Debug.Assert(current != null, "current node cannot be null");
            if (current.ChildNodes.Count > 0)
            {
                SetPresentparentAndGrandParentRDNodes(current.FirstChild);
            }
            //Debug.Assert(current.ChildNodes.Count == 1, "In set up, current only contains one child");

            if (s_grandParentRDNode == null)
            {
                if (CanContainRD(current.Name))
                {
                    if (s_presentRDNode == null)
                    {
                        s_presentRDNode = current;
                    }
                    else if (s_parentRDNode == null)
                    {
                        s_parentRDNode = current;
                    }
                    else if (s_grandParentRDNode == null)
                    {
                        s_grandParentRDNode = current;
                    }
                }
            }
        }

        private static bool CanContainRD(string name)
        {
            return ((name == "StackPanel" || name == "Button" || name == "Control" || name == "FlowDocumentReader"
                || name == "FlowDocument" || name == "UserControl" || name == "Rectangle"
                || name == "Application"
                || name == "Style" || name == "ControlTemplate"));
        }

        private static void SetApplicationRDNode()
        {
            XmlElement e1 = s_mainDoc.CreateElement("Application", s_mainDoc.NamespaceURI);
            s_applicationRDNode = e1;
        }

        private static void SetResourcesIntoRDs()
        {
            bool isDynamicReference;
            bool isPropertyElementSyntax;

            string resourcekey = null;

            resourcekey = CalculateResourceKey(RdLocatiion.PresentRD, out isDynamicReference, out isPropertyElementSyntax);
            while (!string.IsNullOrEmpty(resourcekey))
            {
                AddResourceIntoRD(s_presentRDNode, resourcekey, isDynamicReference, isPropertyElementSyntax);
                resourcekey = CalculateResourceKey(RdLocatiion.PresentRD, out isDynamicReference, out isPropertyElementSyntax);
            }

            resourcekey = CalculateResourceKey(RdLocatiion.ParentRD, out isDynamicReference, out isPropertyElementSyntax);
            while (!string.IsNullOrEmpty(resourcekey))
            {
                AddResourceIntoRD(s_parentRDNode, resourcekey, isDynamicReference, isPropertyElementSyntax);
                resourcekey = CalculateResourceKey(RdLocatiion.ParentRD, out isDynamicReference, out isPropertyElementSyntax);
            }

            resourcekey = CalculateResourceKey(RdLocatiion.GrandParentRD, out isDynamicReference, out isPropertyElementSyntax);
            while (!string.IsNullOrEmpty(resourcekey))
            {
                AddResourceIntoRD(s_grandParentRDNode, resourcekey, isDynamicReference, isPropertyElementSyntax);
                resourcekey = CalculateResourceKey(RdLocatiion.GrandParentRD, out isDynamicReference, out isPropertyElementSyntax);
            }

            resourcekey = CalculateResourceKey(RdLocatiion.RootRD, out isDynamicReference, out isPropertyElementSyntax);
            while (!string.IsNullOrEmpty(resourcekey))
            {
                AddResourceIntoRD(s_rootRDNode, resourcekey, isDynamicReference, isPropertyElementSyntax);
                resourcekey = CalculateResourceKey(RdLocatiion.RootRD, out isDynamicReference, out isPropertyElementSyntax);
            }

            resourcekey = CalculateResourceKey(RdLocatiion.ApplicationRD, out isDynamicReference, out isPropertyElementSyntax);
            while (!string.IsNullOrEmpty(resourcekey))
            {
                AddResourceIntoRD(s_applicationRDNode, resourcekey, isDynamicReference, isPropertyElementSyntax);
                resourcekey = CalculateResourceKey(RdLocatiion.ApplicationRD, out isDynamicReference, out isPropertyElementSyntax);

                if (s_remainingCount == 0)
                {
                    Console.WriteLine("________________");
                    Console.WriteLine(FormatElement(s_applicationRDNode.FirstChild, 4));
                    Console.WriteLine("================");
                }
            }
        }

        private static void AddResourceIntoRD(XmlNode rdNode, string resourceKey, bool isDynamicReference, bool isPropertyElementSyntax)
        {
            Debug.Assert(rdNode != null, "rdNode cannot be Null.");
            Debug.Assert(rdNode == s_applicationRDNode || rdNode == s_presentRDNode
                || rdNode == s_rootRDNode || rdNode == s_grandParentRDNode || rdNode == s_parentRDNode,
                "Only selected RDNode can call AddResourceIntoRD.");
            Debug.Assert(CanContainRD(rdNode.Name), "Double Check");

            XmlDocument sourceDocument = s_elementsDoc;
            //Find Resources section, if not found, create one
            XmlNode resources = null;

            foreach (XmlNode checkNode in rdNode.ChildNodes)
            {
                if (checkNode.Name == rdNode.Name + ".Resources")
                {
                    resources = checkNode;
                    break;
                }
            }

            if (resources == null)
            {
                resources = sourceDocument.CreateElement(rdNode.Name + ".Resources", rdNode.NamespaceURI);
                resources = ImportAndInsertNode(rdNode, resources, true);

            }

            XmlNode resource = GetUniqueResourceNode(resourceKey);
            if (isDynamicReference != false || isPropertyElementSyntax != false)
            {
                AdjustReferenceTypeAndName(resource, isDynamicReference, isPropertyElementSyntax, -1);
            }

            ImportAndInsertNode(resources, resource, true);
        }


        internal static string FormatElement(XmlNode element, int leadingBlankSpaces)
        {
            string spaces = new string(' ', leadingBlankSpaces);

            StringWriter sw = new StringWriter();

            XmlTextWriter writer = new XmlTextWriter(sw);
            writer.Formatting = Formatting.Indented;
            writer.Indentation = 4;
            element.WriteTo(writer);

            string original = sw.ToString();

            string[] lines = original.Split(new string[] { System.Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            StringBuilder sb1 = new StringBuilder();
            foreach (string line in lines)
            {
                sb1.Append(spaces);
                sb1.Append(line.Replace(@" xmlns:x=""http://schemas.microsoft.com/winfx/2006/xaml""", "").Replace(@" xmlns=""http://schemas.microsoft.com/winfx/2006/xaml/presentation""", ""));
                sb1.Append(System.Environment.NewLine);
            }
            sb1.Remove(sb1.Length - 1, 1); //cosmetic: The last NewLine is not needed 
            return sb1.ToString();
        }

        private static string CalculateResourceKey(RdLocatiion location, out bool isDynamicReference, out bool isPropertyElementSyntax)
        {
            isDynamicReference = false;
            isPropertyElementSyntax = s_state.MarkupSyntax == "PropertyElement";

            RdLocatiion stateRdLocation = (RdLocatiion)Enum.Parse(typeof(RdLocatiion), s_state.ResourceDefinitionLocation);

            if (s_remainingCount > 0)
            {
                if (s_state.ReferenceType == "AllDynamic")
                {
                    isDynamicReference = true;
                }
                else if (s_state.ReferenceType != "AllStatic")
                {
                    int rank = s_resolutionStepCount - s_remainingCount;
                    if (s_state.ReferenceType == "AlternateDynamicFirst")
                    {
                        rank++;
                    }
                    if (rank % 2 == 0)
                    {
                        isDynamicReference = true;
                    }
                }

                if (location == RdLocatiion.ApplicationRD)
                {
                    return GetNewKey();
                }

                //Now location is not ApplicationRd
                if (s_state.ApplicationResourcePreferred == "true" && s_remainingCount == 1)
                {
                    return null; //Make sure we satisfy ApplicationResourcePreferred
                }
                if (s_state.ApplicationResourcePreferred != "true" && location == RdLocatiion.RootRD)
                {
                    return GetNewKey();
                }

                if (s_state.SameRDPreferred == "true")
                {
                    if (stateRdLocation == location)
                    {
                        return GetNewKey();
                    }
                }
                else
                {
                    if (s_lastRdLocation != location && location >= stateRdLocation)
                    {
                        s_lastRdLocation = location;
                        return GetNewKey();
                    }
                }
            }

            return null;
      
        }

        private static string GetNewKey()
        {
            string key;
            Debug.Assert(s_remainingCount > 0, "GetNewKey cannot be called when there is no remainging key expected.");
            if (s_resolutionStepCount == s_remainingCount)
            {
                key = s_resourceKeyPrefix + "_" + s_resolutionStepCount;
            }
            else
            {
                key = s_resourceKeyPrefix + "_" + s_resolutionStepCount + s_remainingCount;
            }
            s_remainingCount--;
            return key;

        }

        internal static string AdditionalAppMarkup
        {
            get
            {
                return s_additionalAppMarkup;
            }
            private set
            {
                s_additionalAppMarkup = value;
            }
        }
        private static string s_additionalAppMarkup;

        private static XmlDocument s_mainDoc;
        private static XmlDocument s_elementsDoc;

        private static XmlNode s_presentRDNode;
        private static XmlNode s_parentRDNode;
        private static XmlNode s_grandParentRDNode;
        private static XmlNode s_rootRDNode;
        private static XmlNode s_applicationRDNode;

        private static string s_resourceKeyPrefix;
        private static int s_resolutionStepCount;
        private static int s_remainingCount;
        private static RdLocatiion s_lastRdLocation = RdLocatiion.ApplicationRD;

        private static ResourcesModelState s_state;

        private enum RdLocatiion
        {
            PresentRD,
            ParentRD,
            GrandParentRD,
            RootRD,
            ApplicationRD
        }

    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*******************************************************************
 * Purpose: Automation for the stateless DisjointTree MDE model.
 *  
 * Contributors: Microsoft
 *
 
  
 * Revision:         $Revision: 14 $
 
 * Filename:         $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/Core/Framework/BVT/PropertyEngine/DisjointTree/DisjointTreeModel.cs $
********************************************************************/
using System;
using Avalon.Test.CoreUI.Trusted;
using Microsoft.Test;
using Avalon.Test.CoreUI;
using System.IO;
using System.Xml;
using Microsoft.Test.Modeling;
using Microsoft.Test.Serialization;
using Avalon.Test.CoreUI.Common;


namespace Avalon.Test.CoreUI.PropertyEngine.DisjointTree
{
    /// <summary>
    /// DisjointTreeXamlHelper class
    /// </summary>  
    internal class DisjointTreeXamlHelper
    {
        /// <summary>
        /// Create an DisjointTreeXamlHelper instance.
        /// </summary>
        public DisjointTreeXamlHelper(DisjointTreeModelState state)
        {
            _state = state;
        }

        /// <summary>
        /// Constructs xaml according to the model param values.
        /// </summary>
        /// <returns>A stream of the constructed xaml.</returns>
        public Stream GenerateXaml()
        {
            _xamlDoc = new XamlTestDocument(_emptyXamlFilename, _elementsXamlFilename);

            _testRootNode = _xamlDoc.TestRoot;

            _InsertMentorParents();
            _InsertMentors();
            _InsertLinkParents();
            _InsertLinks();
            _InsertMentees();

            _InsertEventHandler();
            _InsertDataProvider();
            _InsertImplicitStyle();

            _xamlDoc.RemoveTestIds();

            //return _xamlDoc.FinishDocument();
            return IOHelper.ConvertXmlDocumentToStream(_xamlDoc);
        }

        /// <summary>
        /// Insert parents of the mentoring element. 
        /// </summary>
        private void _InsertMentorParents()
        {
            // Insert first mentor parent
            _firstMentorParent = _xamlDoc.ImportAndAppendElement(_testRootNode, "MentorParent");
            
            // Insert second mentor parent, location depends on model parameters.
            XmlElement secondParent = _xamlDoc.GetSnippet("MentorParent");
            if (_state.ContextOrientation == "Sibling")
            {
                _secondMentorParent = _xamlDoc.ImportAndAppendElement(_testRootNode, secondParent);
            }
            else if (_state.ContextOrientation == "Nested")
            {
                _secondMentorParent = _xamlDoc.ImportAndAppendElement(_firstMentorParent, secondParent);
            }
            else
            {
                throw new NotSupportedException(_state.ContextOrientation);
            }

            _xamlDoc.AddAttribute(_firstMentorParent, "Name", _state.FirstParentName);
            _xamlDoc.AddAttribute(_secondMentorParent, "Name", _state.SecondParentName);

            // 
            _xamlDoc.AddAttribute(_firstMentorParent, "Background", "Orange");
            _xamlDoc.AddAttribute(_secondMentorParent, "Background", "Red");

            if (_state.Service == "DynamicResource")
            {
                // Mentee contains reference to SolidColorBrush, add resources section and brush to mentor parents.

                XmlElement parentResources;
                //XmlElement brush;

                // Add to first parent.
                parentResources = _xamlDoc.AddElement(_firstMentorParent, "StackPanel.Resources");
                _xamlDoc.ImportAndAppendElement(parentResources, "firstBrush");

                // Add to second parent.
                parentResources = _xamlDoc.AddElement(_secondMentorParent, "StackPanel.Resources");
                _xamlDoc.ImportAndAppendElement(parentResources, "secondBrush");
            }

            if (_state.Service == "Inheritance")
            {
                _xamlDoc.AddAttribute(_firstMentorParent, "TextBlock.FontStyle", "Italic");
                _xamlDoc.AddAttribute(_secondMentorParent, "TextBlock.FontStyle", "Oblique");
            }
        }


        /// <summary>
        /// Insert mentors that will be IC values.
        /// </summary>
        private void _InsertMentors()
        {
            _firstMentor = _xamlDoc.ImportAndAppendElement(_firstMentorParent, _state.MentorItem + "_Mentor");

            _secondMentor = _xamlDoc.ImportAndAppendElement(_secondMentorParent, _state.MentorItem + "_Mentor");

            _xamlDoc.AddAttribute(_firstMentor, "Name", _state.FirstMentorName);
            _xamlDoc.AddAttribute(_secondMentor, "Name", _state.SecondMentorName);

            if (_state.Service == "BindingDataContext")
            {
                // Add DataContext reference.
                _xamlDoc.AddAttribute(_firstMentor, "DataContext", "{Binding Source={StaticResource XmlDataProvider1}}");
                _xamlDoc.AddAttribute(_secondMentor, "DataContext", "{Binding Source={StaticResource XmlDataProvider2}}");

                if (_state.HowLinked == "DataTemplate")
                {
                    // Add Content binding.
                    _xamlDoc.AddAttribute(_firstMentor, "Content", "{Binding}");
                    _xamlDoc.AddAttribute(_secondMentor, "Content", "{Binding}");
                }
            }

        }

        /// <summary>
        /// Get or create parents of the IC link. The link may be in resources
        /// or a setter or ...
        /// </summary>
        private void _InsertLinkParents()
        {
            string propertyName;
            switch (_state.Link)
            {
                case "VisualBrush":
                    propertyName = "Background";
                    break;
                case "ContextMenu":
                    propertyName = "ContextMenu";
                    break;
                case "ToolTip":
                    propertyName = "ToolTip";
                    break;
                default:
                    propertyName = "";                    
                    break;
            }

            if (propertyName == "")
            {
                throw new NotSupportedException(_state.Link);
            }


            if (_state.HowLinked == "Locally")
            {
                // Link parent is a property on the mentor (Button.Background for VB, Button.ContextMenu for ContextMenu, etc).
                _firstLinkParent = _xamlDoc.AddElement(_firstMentor, _state.MentorItem + "." + propertyName);

                _secondLinkParent = _xamlDoc.AddElement(_secondMentor, _state.MentorItem + "." + propertyName);
            }
            else if ((_state.HowLinked == "StaticResource") || (_state.HowLinked == "DynamicResource"))
            {
                //
                // Link parent is resource section in the mentor's parent.
                //

                _firstLinkParent = _xamlDoc.AddElement(_firstMentorParent, "StackPanel.Resources");

                _secondLinkParent = _xamlDoc.AddElement(_secondMentorParent, "StackPanel.Resources");

                _xamlDoc.AddAttribute(_firstMentor, propertyName, "{" + _state.HowLinked + " firstLink}");
                _xamlDoc.AddAttribute(_secondMentor, propertyName, "{" + _state.HowLinked + " secondLink}");

            }
            else if (_state.HowLinked == "Style")
            {
                _firstLinkParent = _InsertStyle(_firstMentor, propertyName);
                _secondLinkParent = _InsertStyle(_secondMentor, propertyName);
            }
            else if ((_state.HowLinked == "ControlTemplate") || (_state.HowLinked == "DataTemplate"))
            {
                _firstLinkParent = InsertTemplate(_firstMentor, propertyName);
                _secondLinkParent = InsertTemplate(_secondMentor, propertyName);
            }
            else
            {
                throw new NotSupportedException(_state.HowLinked);
            }
        }

        /// <summary>
        /// Styles are always inline. Link parent is Style's Setter.Value
        /// </summary>
        /// <returns>Parent for hybrid tree link.</returns>
        private XmlElement _InsertStyle(XmlElement mentor, string propertyName)
        {
            XmlElement styleProperty;
            XmlElement style;
            XmlElement setter;
            //XmlElement setterValue;

            styleProperty = _xamlDoc.AddElement(mentor, _state.MentorItem + ".Style");
            style = _xamlDoc.ImportAndAppendElement(styleProperty, "HowLinked_Style");
            setter = (XmlElement)style.FirstChild;
            _xamlDoc.ReplaceAttributeSubstring(setter, "Property", "FOO", propertyName);

            // Setter's first child is Setter.Value.
            return (XmlElement)setter.FirstChild;
        }

        /// <summary>
        /// Templates are always inline. Link parent is in template's button's first child.
        /// </summary>
        /// <param name="mentor"></param>
        /// <param name="propertyName">Name of property element to add to button in template</param>
        /// <returns>Parent for hybrid tree link.</returns>
        private XmlElement InsertTemplate(XmlElement mentor, string propertyName)
        {
            string templatePropertyName;
            if (_state.HowLinked == "ControlTemplate")
            {
                templatePropertyName = "Template";
            }
            else if (_state.HowLinked == "DataTemplate")
            {
                templatePropertyName = "ContentTemplate";
            }
            else
            {
                throw new NotSupportedException(_state.HowLinked);
            }

            XmlElement templateProperty = _xamlDoc.AddElement(mentor, _state.MentorItem + "." + templatePropertyName);
            XmlElement template = _xamlDoc.ImportAndAppendElement(templateProperty, "HowLinked_" + _state.HowLinked);
            XmlElement templateContent = (XmlElement)template.FirstChild;

            return _xamlDoc.AddElement(templateContent, "Button." + propertyName);
        }

        /// <summary>
        /// Insert mentor mentee link, this may be either "FE to FE" or "FE to Freezable to FE"
        /// </summary>
        private void _InsertLinks()
        {
            _firstLink = _xamlDoc.ImportAndAppendElement(_firstLinkParent, _state.Link + "_Link");

            _secondLink = _xamlDoc.ImportAndAppendElement(_secondLinkParent, _state.Link + "_Link"); 

            // todo: make this a switch
            if (_state.Link == "VisualBrush")
            {
                // First mentee parent is VisualBrush.Visual under VisualBrush.
                _firstMenteeParent = (XmlElement)_firstLink.FirstChild;
                _secondMenteeParent = (XmlElement)_secondLink.FirstChild;
            }
            else if (_state.Link == "ContextMenu")
            {
                // Mentee parent is just ContextMenu element.
                _firstMenteeParent = _firstLink;
                _secondMenteeParent = _secondLink;
            }
            else if (_state.Link == "ToolTip")
            {
                // Mentee parent is just ToolTip element.
                _firstMenteeParent = _firstLink;
                _secondMenteeParent = _secondLink;
            }
            else
            {
                throw new NotSupportedException(_state.Link);
            }

            // If link is in a resource dictionary add resource key.
            if ((_state.HowLinked == "StaticResource") || (_state.HowLinked == "DynamicResource"))
            {
                _xamlDoc.ReplaceAttributeSubstring(_firstLink, "x:Key", "FOO", "firstLink");
                _xamlDoc.ReplaceAttributeSubstring(_secondLink, "x:Key", "FOO", "secondLink");
            }
            else
            {
                _xamlDoc.RemoveAttribute(_firstLink, "x:Key");
                _xamlDoc.RemoveAttribute(_secondLink, "x:Key");
            }
        }

        /// <summary>
        /// Insert mentee
        /// </summary>
        private void _InsertMentees()
        {
            // Should be inserting rectangles and things.
            _firstMentee = _xamlDoc.ImportAndAppendElement(_firstMenteeParent, _state.Service + "_Mentee");

            // Perform same op on second mentee.
            _secondMentee = _xamlDoc.ImportAndAppendElement(_secondMenteeParent, _state.Service + "_Mentee");

            _xamlDoc.AddAttribute(_firstMentee, "Name", _state.FirstMenteeName);
            _xamlDoc.AddAttribute(_secondMentee, "Name", _state.SecondMenteeName);

            if (_state.Service == "BindingElementName")
            {
                // Set binding ElementName property
                _xamlDoc.ReplaceAttributeSubstring(_firstMentee, "Fill", "FOO", _state.FirstParentName);
                _xamlDoc.ReplaceAttributeSubstring(_secondMentee, "Fill", "FOO", _state.SecondParentName);
            }

            if (_state.Service == "DynamicResource")
            {
                _xamlDoc.ReplaceAttributeSubstring(_firstMentee, "Fill", "FOO", _state.DynamicResourceKey);
                _xamlDoc.ReplaceAttributeSubstring(_secondMentee, "Fill", "FOO", _state.DynamicResourceKey);
            }

            // Remove name from mentee if link is in a static resource, verifier will have to find it another way.
            if ((_state.HowLinked == "StaticResource") || (_state.HowLinked == "DynamicResource"))
            {
                _xamlDoc.RemoveAttribute(_firstMentee, "Name");
                _xamlDoc.RemoveAttribute(_secondMentee, "Name");
            }
        }

        private void _InsertEventHandler()
        {
            if ((_state.Service == "InitializationEvent") || (_state.Service == "LoadedEvent"))
            {
                _xamlDoc.ImportAndAppendElement(_testRootNode, "EventCode");
            }
        }

        private void _InsertDataProvider()
        {
            if (_state.Service.Contains("Binding") == false) 
                return;

            XmlElement resources = _xamlDoc.AddElement(_testRootNode, "StackPanel.Resources");

            _xamlDoc.ImportAndAppendElement(resources, "XmlDataProvider1");
            _xamlDoc.ImportAndAppendElement(resources, "XmlDataProvider2");
        }

        private void _InsertImplicitStyle()
        {
            if (_state.Service != "ImplicitStyle")
                return;

            XmlElement resources = _xamlDoc.AddElement(_testRootNode, "StackPanel.Resources");

            _xamlDoc.ImportAndAppendElement(resources, "ImplicitStyle");
        }

        //
        // Private fields
        //

        private DisjointTreeModelState _state = null;

        string _emptyXamlFilename = "DisjointTree_empty.xaml";
        string _elementsXamlFilename = "DisjointTree_Elements.xaml";

        XamlTestDocument _xamlDoc;

        // Convenient references to xml nodes used throughout...
        private XmlElement _testRootNode = null;

        private XmlElement _firstMentorParent = null;
        private XmlElement _secondMentorParent = null;

        private XmlElement _firstMentor = null;
        private XmlElement _secondMentor = null;

        private XmlElement _firstLinkParent = null;
        private XmlElement _secondLinkParent = null;

        private XmlElement _firstLink = null;
        private XmlElement _secondLink = null;

        private XmlElement _firstMenteeParent = null;
        private XmlElement _secondMenteeParent = null;

        private XmlElement _firstMentee = null;
        private XmlElement _secondMentee = null;
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides custom XPathNavigator classes to navigate different tree structures.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Utils/CustomXPathNavigators.cs $")]

namespace Test.Uis.Utils
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    #endregion Namespaces.

    /// <summary>
    /// Provides convenience methods to use other navigator types.
    /// </summary>
    public abstract class XPathNavigatorUtils
    {
        /// <summary>
        /// Lists all the AutomationElements that match the specified
        /// XPath query for the given root in the raw tree.
        /// </summary>
        /// <param name='root'>Root to start query from.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <returns>An array of matches.</returns>
        /// <example>The following sample shows how to use this method.<code>...
        /// private string ListTopLevelWindowTitles() {
        ///   AutomationElement rootElement = (AutomationElement)AutomationElement.RootElement;
        ///   StringBuilder sb = new StringBuilder(1024);
        ///   AutomationElement[] topLevelWindows =
        ///     XPathNavigatorUtils.ListAutomationElements("./*", rootElement);
        ///   foreach(AutomationElement e in topLevelWindows)
        ///     sb.Append(e.GetCurrentPropertyValue(AutomationElement.NameProperty);
        ///   }
        ///   return sb.ToString();
        /// }</code></example>
        public static AutomationElement[] ListAutomationElements(
            AutomationElement root, string xpathQuery)
        {
            return ListAutomationElements(root, xpathQuery, TreeWalker.RawViewWalker);
        }

        /// <summary>
        /// Lists all the AutomationElements that match the specified
        /// XPath query for the given root in the raw tree.
        /// </summary>
        /// <param name='root'>Root to start query from.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <param name='walker'>Walker used to control how the UIAutomation tree is walked.</param>
        /// <returns>An array of matches.</returns>
        /// <remarks>
        /// The tree walker is typically one of the following static fields:
        /// System.Windows.Automation.TreeWalker.RawViewWalker, or
        /// System.Windows.Automation.TreeWalker.ControlViewWalker.
        /// </remarks>
        public static AutomationElement[] ListAutomationElements(
            AutomationElement root, string xpathQuery, TreeWalker walker)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (xpathQuery == null)
            {
                throw new ArgumentNullException("xpathQuery");
            }
            if (walker == null)
            {
                throw new ArgumentNullException("walker");
            }
            ArrayList list = new ArrayList();
            AutomationElementNavigator navigator =
                new AutomationElementNavigator(root, walker);
            XPathNodeIterator i = navigator.Select(xpathQuery);
            while (i.MoveNext())
            {
                navigator = (AutomationElementNavigator) i.Current;
                list.Add(navigator.CurrentElement);
            }
            return (AutomationElement[]) list.ToArray(typeof(AutomationElement));
        }

        /// <summary>
        /// Lists all the elements that match 
        /// the specified XPath query for the given TextContainer.
        /// </summary>
        /// <param name='textRange'>TextRange to query information.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <returns>An array of matches.</returns>
        public static TextRange[] ListElements(TextRange textRange, string xpathQuery)
        {
            ArrayList list;                         // List of matches.
            TextContainerXPathNavigator navigator;  // Document navigator.
            XPathNodeIterator i;                    // Results enumerator.
            
            list = new ArrayList();
            navigator = new TextContainerXPathNavigator(textRange);
            
            i = navigator.Select(xpathQuery);
            while (i.MoveNext())
            {
                navigator = (TextContainerXPathNavigator) i.Current;
                list.Add(navigator.CurrentRange);
            }

            return (TextRange[])list.ToArray(typeof(TextRange));
        }

        /// <summary>
        /// Lists all the text elements that match 
        /// the specified XPath query for the given TextContainer.
        /// </summary>
        /// <param name='root'>Root element to query.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <returns>An array of matches.</returns>
        public static TextElement[] ListTextElements(FrameworkContentElement root, string xpathQuery)
        {
            ArrayList list;                         // List of matches.
            TextElementXPathNavigator navigator;    // Document navigator.
            XPathNodeIterator i;                    // Results enumerator.

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (xpathQuery == null)
            {
                throw new ArgumentNullException("xpathQuery");
            }

            list = new ArrayList();
            navigator = new TextElementXPathNavigator(root);

            i = navigator.Select(xpathQuery);
            while (i.MoveNext())
            {
                navigator = (TextElementXPathNavigator)i.Current;
                list.Add(navigator.CurrentElement);
            }

            return (TextElement[])list.ToArray(typeof(FrameworkContentElement));
        }

        /// <summary>
        /// Lists all the text elements that match 
        /// the specified XPath query for the given TextContainer.
        /// </summary>
        /// <param name='root'>Root element to query.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <returns>An array of matches.</returns>
        public static TextElement[] ListTextElements(FlowDocument root, string xpathQuery)
        {
            ArrayList list;                         // List of matches.
            TextElementXPathNavigator navigator;    // Document navigator.
            XPathNodeIterator i;                    // Results enumerator.

            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (xpathQuery == null)
            {
                throw new ArgumentNullException("xpathQuery");
            }

            list = new ArrayList();
            navigator = new TextElementXPathNavigator(root);

            i = navigator.Select(xpathQuery);
            while (i.MoveNext())
            {
                navigator = (TextElementXPathNavigator)i.Current;
                list.Add(navigator.CurrentElement);
            }

            return (TextElement[])list.ToArray(typeof(TextElement));
        }

        /// <summary>
        /// Lists all the IVisuals that match the specified
        /// XPath query for the given root.
        /// </summary>
        /// <param name='root'>Root to start query from.</param>
        /// <param name='xpathQuery'>XPath query specifying elements to look for.</param>
        /// <returns>An array of matches.</returns>
        public static Visual[] ListVisuals(Visual root, string xpathQuery)
        {
            ArrayList list = new ArrayList();
            VisualXPathNavigator navigator =
                new VisualXPathNavigator(root);
            XPathNodeIterator i = navigator.Select(xpathQuery);
            while (i.MoveNext())
            {
                navigator = (VisualXPathNavigator) i.Current;
                list.Add(navigator.CurrentVisual);
            }
            return (Visual [])list.ToArray(typeof(Visual));
        }
    }

    #region Navigator implementations.

    /// <summary>
    /// Implements an XPathNavigator type that navigates the Automation tree.
    /// </summary>
    /// <remarks>
    /// Most of the properties declared on AutomationElement are supported
    /// as attributes. The XML nodes are represented with the value of
    /// the ClassName automation property.
    /// </remarks>
    /// <example>The following sample shows how to use this class.<code>...
    /// private string ListTopLevelWindowTitles() {
    ///   StringBuilder sb = new StringBuilder(1024);
    ///   AutomationElementNavigator navigator =
    ///     new AutomationElementNavigator((AutomationElement)AutomationElement.RootElement);
    ///   XPathNodeIterator i = navigator.Select("./*");
    ///   while (i.MoveNext()) {
    ///     navigator = (AutomationElementNavigator) i.Current;
    ///     AutomationElement e = navigator.CurrentElement;
    ///     sb.Append(e.GetCurrentPropertyValue(AutomationElement.NameProperty));
    ///   }
    ///   return sb.ToString();
    /// }</code></example>
    class AutomationElementNavigator: XPathNavigator
    {
        #region Constructors.

        /// <summary>
        /// Creates a new AutomationElementNavigator instance rooted at the
        /// specified AutomationElement element and walking with the
        /// given TreeWalker.
        /// </summary>
        /// <param name='root'>Navigation root.</param>
        /// <param name='walker'>TreeWalker to navigate with..</param>
        public AutomationElementNavigator(AutomationElement root, TreeWalker walker)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }
            if (walker == null)
            {
                throw new ArgumentNullException("walker");
            }

            _root = root;
            _current = root;
            _attributeIndex = -1;
            _nametable = new NameTable();
            _walker = walker;
        }

        /// <summary>Private copy constructor.</summary>
        private AutomationElementNavigator(AutomationElementNavigator other)
        {
            System.Diagnostics.Debug.Assert(other != null);

            _root = other._root;
            _current = other._current;
            _attributeIndex = other._attributeIndex;
            _nametable = other._nametable;
            _properties = other._properties;
            _walker = other._walker;

            _nametable.Add(String.Empty);
        }

        #endregion Constructors.

        #region Public methods.

        #region XPathNavigator abstract implementations.

        public override XPathNavigator Clone()
        {
            return new AutomationElementNavigator(this);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified LocalName and
        /// NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// </returns>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            if (_current == null) return String.Empty;

            AutomationProperty property = StringToAutomationProperty(localName);
            if (property == null) return String.Empty;

            return _current.GetCurrentPropertyValue(property).ToString();
        }

        /// <summary>
        /// Returns the value of the namespace node corresponding to the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// The value of the namespace node; String.Empty if a matching
        /// namespace node is not found or if the navigator is not
        /// positioned on an element node.
        /// </returns>
        public override string GetNamespace(string name)
        {
            return String.Empty;
        }

        /// <summary>
        /// Determines whether the current XPathNavigator is at the same
        /// position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>The XPathNavigator that you want to compare against.</param>
        /// <returns>true if the two navigators have the same position; otherwise, false.</returns>
        public override bool IsSamePosition(XPathNavigator other)
        {
            AutomationElementNavigator o = other as AutomationElementNavigator;
            return (other != null) &&
                (o._current == this._current) &&
                (o._attributeIndex == this._attributeIndex);
        }

        /// <summary>
        /// Moves to the same position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>
        /// The XPathNavigator positioned on the node that you want to move to.
        /// </param>
        /// <returns>
        /// true if successful; otherwise false. If false, the position
        /// of the navigator is unchanged.
        /// </returns>
        public override bool MoveTo(XPathNavigator other)
        {
            AutomationElementNavigator o = other as AutomationElementNavigator;
            if (o != null)
            {
                this._current = o._current;
                this._properties = o._properties;
                this._attributeIndex = o._attributeIndex;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the attribute with matching LocalName and NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false,
        /// the position of the navigator does not change.
        /// </returns>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            return false;
        }

        /// <summary>
        /// Moves to the first sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first sibling node;
        /// false if there is no first sibling or if the navigator is currently
        /// positioned on an attribute node.
        /// </returns>
        public override bool MoveToFirst()
        {
            if (_current == null) return false;
            if (_current == _root) return false;
            AutomationElement v = GetNthElement(_walker.GetParent(_current), 0);
            if (v == null) return false;
            _current = v;
            _properties = null;
            _attributeIndex = -1;
            _indexInParent = 0;
            return true;
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first attribute;
        /// otherwise, false.
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            EnsureProperties();
            if (_properties.Length == 0)
            {
                return false;
            }
            _attributeIndex = 0;
            return true;
        }

        /// <summary>
        /// Moves to the first child of the current node.
        /// </summary>
        /// <returns>
        /// true if there is a first child node; otherwise false.
        /// </returns>
        public override bool MoveToFirstChild()
        {
            if (_current == null) return false;
            AutomationElement e = _walker.GetFirstChild(_current);
            if (e == null) return false;
            _current = e;
            _properties = null;
            _attributeIndex = -1;
            _indexInParent = 0;
            return true;
        }

        /// <summary>
        /// Moves the XPathNavigator to the first namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the first
        /// namespace node; otherwise, false.
        /// </returns>
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the node that has an attribute of type ID whose
        /// value matches the specified string.
        /// </summary>
        /// <param name='id'>
        /// A string representing the ID value of the node to which you want to move.
        /// </param>
        /// <returns>
        /// true if the move was successful; otherwise false. If false, the
        /// position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the namespace node with the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// true if the move was successful; false if a matching
        /// namespace node was not found or if the navigator is
        /// not positioned on an element node
        /// </returns>
        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        /// <summary>
        /// Moves to the next sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no more siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToNext()
        {
            if (_current == _root) return false;
            AutomationElement next = _walker.GetNextSibling(_current);
            if (next == null) return false;
            _current = next;
            _properties = null;
            _attributeIndex = -1;
            _indexInParent++;
            return true;
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            EnsureProperties();
            if (_properties.Length == 0) return false;
            if (_attributeIndex + 1 >= _properties.Length) return false;
            _attributeIndex++;
            return true;
        }

        /// <summary>
        /// Moves the XPathNavigator to the next namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// namespace node; false if there are no more namespace nodes.
        /// </returns>
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the parent of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the parent node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToParent()
        {
            if (_current == _root) return false;
            _current = _walker.GetParent(_current);
            _properties = null;
            _attributeIndex = -1;
            _indexInParent = FindElementIndex(_current);
            return true;
        }

        /// <summary>
        /// Moves to the previous sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no previous siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToPrevious()
        {
            if (_current == _root) return false;
            if (_indexInParent == 0) return false;
            AutomationElement prev = _walker.GetPreviousSibling(_current);
            if (prev == null) return false;
            _current = prev;
            _properties = null;
            _attributeIndex = -1;
            _indexInParent--;
            return true;
        }

        /// <summary>
        /// Moves to the root node to which the current node belongs.
        /// </summary>
        public override void MoveToRoot()
        {
            _current = _root;
            _properties = null;
            _attributeIndex = -1;
        }

        #endregion XPathNavigator abstract implementations.

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// AutomationElement the navigator is placed on.
        /// </summary>
        public AutomationElement CurrentElement
        {
            get { return _current; }
        }

        #region Node properties.

        public override string BaseURI
        {
            get { return String.Empty; }
        }

        public override bool HasAttributes
        {
            get { return true; }
        }

        public override bool HasChildren
        {
            get { return (_current != null) && (_walker.GetFirstChild(_current) != null); }
        }

        public override bool IsEmptyElement
        {
            get { return !HasChildren; }
        }

        public override string LocalName
        {
            get
            {
                return Name;
            }
        }

        public override string Name
        {
            get
            {
                if (_current == null) return String.Empty;
                string name;
                EnsureProperties();
                if (_attributeIndex == -1)
                {
                    name = _current.GetCurrentPropertyValue(
                        AutomationElement.ClassNameProperty).ToString();
                }
                else
                {
                    name = AutomationPropertyToString(_properties[_attributeIndex]);
                    if (name == null) name = "UnknownPropertyName";
                }
                _nametable.Add(name);
                return _nametable.Get(name);
            }
        }

        public override string NamespaceURI
        {
            get { return String.Empty; }
        }

        public override XmlNameTable NameTable
        {
            get { return _nametable; }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                if (_attributeIndex != -1)
                {
                    return XPathNodeType.Attribute;
                }
                else if (_current == _root)
                {
                    return XPathNodeType.Root;
                }
                else
                {
                    return XPathNodeType.Element;
                }
            }
        }

        public override string Prefix
        {
            get { return String.Empty; }
        }

        public override string Value
        {
            get
            {
                if (_attributeIndex == -1) return String.Empty;
                if (_current == null) return String.Empty;
                EnsureProperties();
                if (_attributeIndex >= _properties.Length)
                {
                    return String.Empty;
                }
                else
                {
                    AutomationProperty p;
                    object result;

                    p = _properties[_attributeIndex];
                    result = _current.GetCurrentPropertyValue(p);
                    return (result == null) ? String.Empty : result.ToString();
                }

            }
        }

        public override string XmlLang
        {
            get { return "en-us"; }
        }

        #endregion Node properties.

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Ensures that the _properties field is initialized
        /// to a valid value.
        /// </summary>
        private void EnsureProperties()
        {
            if (_properties != null)
            {
                return;
            }

            if (_current == null)
            {
                _properties = new AutomationProperty[0];
            }
            else
            {
                _properties = _current.GetSupportedProperties();
            }
        }

        /// <summary>
        /// Gets the nth child element from the specified element.
        /// </summary>
        private AutomationElement GetNthElement(AutomationElement parent, int index)
        {
            int enumIndex = 0;
            AutomationElement child = _walker.GetFirstChild(parent);
            while (enumIndex != index && child != null)
            {
                enumIndex++;
                child = _walker.GetNextSibling(child);
            }
            return child;
        }

        private int FindElementIndex(AutomationElement element)
        {
            if (element == null) return 0;

            AutomationElement parent = _walker.GetParent(element);
            if (parent == null) return 0;

            AutomationElement child = _walker.GetFirstChild(element);
            int enumIndex = 0;
            while (child != element && child != null)
            {
                enumIndex++;
                child = _walker.GetNextSibling(child);
            }
            System.Diagnostics.Debug.Assert(false, "Unable to find AutomationElement in parent.");
            return 0;
        }

        /// <summary>
        /// Maps a string to an AutomationProperty instance.
        /// </summary>
        /// <param name="value">String to map.</param>
        /// <returns>
        /// The corrisponding AutomationProperty if available, null otherwise.
        /// </returns>
        private static AutomationProperty StringToAutomationProperty(string value)
        {
            if (value == null) return null;
            for (int i = 0; i < s_propertyNameMap.Length; i++)
            {
                if (s_propertyNameMap[i].Name == value)
                    return s_propertyNameMap[i].Property;
            }
            return null;
        }

        private static string AutomationPropertyToString(AutomationProperty property)
        {
            if (property == null) return null;
            for (int i = 0; i < s_propertyNameMap.Length; i++)
            {
                if (s_propertyNameMap[i].Property == property)
                    return s_propertyNameMap[i].Name;
            }
            return null;
        }

        #endregion Private methods.

        #region Private fields.

        #region Property-to-name mapping.

        struct PropertyNameMapEntry
        {
            public readonly AutomationProperty Property;
            public readonly string Name;

            internal PropertyNameMapEntry(AutomationProperty property, string name)
            {
                this.Property = property;
                this.Name = name;
            }
        }

        private static PropertyNameMapEntry[] s_propertyNameMap = {
            new PropertyNameMapEntry(AutomationElement.AcceleratorKeyProperty, "AcceleratorKey"),
            new PropertyNameMapEntry(AutomationElement.AccessKeyProperty, "AccessKey"),
            new PropertyNameMapEntry(AutomationElement.BoundingRectangleProperty, "BoundingRectangle"),
            new PropertyNameMapEntry(AutomationElement.ClassNameProperty, "ClassName"),
            new PropertyNameMapEntry(AutomationElement.ControlTypeProperty, "ControlType"),
            new PropertyNameMapEntry(AutomationElement.LocalizedControlTypeProperty, "LocalizedControlType"),
            new PropertyNameMapEntry(AutomationElement.CultureProperty, "Culture"),
            new PropertyNameMapEntry(AutomationElement.IsEnabledProperty, "IsEnabled"),
            new PropertyNameMapEntry(AutomationElement.HasKeyboardFocusProperty, "HasKeyboardFocus"),
            new PropertyNameMapEntry(AutomationElement.NameProperty, "Name"),
            new PropertyNameMapEntry(AutomationElement.ProcessIdProperty, "ProcessId"),
            new PropertyNameMapEntry(AutomationElement.RuntimeIdProperty, "RuntimeId"),
            new PropertyNameMapEntry(AutomationElement.HelpTextProperty, "HelpText"),
        };

        #endregion Property-to-name mapping.

        private AutomationElement _current;
        private AutomationElement _root;
        private int _indexInParent;

        /// <summary>
        /// Index of attribute. Properties are exposed as attributes.
        /// Set to -1 every time the _current element changes.
        /// </summary>
        private int _attributeIndex;

        /// <summary>
        /// Properties of the _current element. Set to null every
        /// time it changes.
        /// </summary>
        private AutomationProperty[] _properties;

        /// <summary>Name table for names. Allows object-based comparisons for performance.</summary>
        private NameTable _nametable;

        /// <summary>TreeWalker used to navigate the automation tree.</summary>
        private TreeWalker _walker;

        #endregion Private fields.
    }

    /// <summary>
    /// Implements an XPathNavigator type that navigates a visual subtree.
    /// </summary>
    class VisualXPathNavigator: XPathNavigator
    {
        #region Constructors.

        /// <summary>
        /// Creates a new VisualXPathNavigator instance rooted at the
        /// specified IVisual element.
        /// </summary>
        /// <param name='root'>Navigation root.</param>
        public VisualXPathNavigator(Visual root)
        {
            if (root == null)
            {
                throw new ArgumentNullException("root");
            }

            _root = root;
            _current = _root;
            _nametable = new NameTable();
        }

        /// <summary>Private copy constructor.</summary>
        private VisualXPathNavigator(VisualXPathNavigator other)
        {
            System.Diagnostics.Debug.Assert(other != null);

            _root = other._root;
            _current = other._current;
            _nametable = other._nametable;
            _indexInParent = other._indexInParent;

            _nametable.Add(String.Empty);
        }

        #endregion Constructors.

        #region Public methods.

        #region XPathNavigator abstract implementations.

        public override XPathNavigator Clone()
        {
            return new VisualXPathNavigator(this);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified LocalName and
        /// NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// </returns>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the value of the namespace node corresponding to the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// The value of the namespace node; String.Empty if a matching
        /// namespace node is not found or if the navigator is not
        /// positioned on an element node.
        /// </returns>
        public override string GetNamespace(string name)
        {
            return String.Empty;
        }

        /// <summary>
        /// Determines whether the current XPathNavigator is at the same
        /// position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>The XPathNavigator that you want to compare against.</param>
        /// <returns>true if the two navigators have the same position; otherwise, false.</returns>
        public override bool IsSamePosition(XPathNavigator other)
        {
            VisualXPathNavigator o = other as VisualXPathNavigator;
            return (o != null) && (o._current == this._current);
        }

        /// <summary>
        /// Moves to the same position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>
        /// The XPathNavigator positioned on the node that you want to move to.
        /// </param>
        /// <returns>
        /// true if successful; otherwise false. If false, the position
        /// of the navigator is unchanged.
        /// </returns>
        public override bool MoveTo(XPathNavigator other)
        {
            VisualXPathNavigator o = other as VisualXPathNavigator;
            if (o != null)
            {
                this._current = o._current;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the attribute with matching LocalName and NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false,
        /// the position of the navigator does not change.
        /// </returns>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            return false;
        }

        /// <summary>
        /// Moves to the first sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first sibling node;
        /// false if there is no first sibling or if the navigator is currently
        /// positioned on an attribute node.
        /// </returns>
        public override bool MoveToFirst()
        {
            if (_current == null) return false;
            if (_current == _root) return false;
            DependencyObject v = GetNthVisual(VisualTreeHelper.GetParent(_current), 0);
            if (v == null) return false;
            _current = v;
            _indexInParent = 0;
            return true;
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first attribute;
        /// otherwise, false.
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves to the first child of the current node.
        /// </summary>
        /// <returns>
        /// true if there is a first child node; otherwise false.
        /// </returns>
        public override bool MoveToFirstChild()
        {
            if (_current == null) return false;
            DependencyObject v = GetNthVisual(_current, 0);
            if (v == null) return false;
            _current = v;
            _indexInParent = 0;
            return true;
        }

        /// <summary>
        /// Moves the XPathNavigator to the first namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the first
        /// namespace node; otherwise, false.
        /// </returns>
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the node that has an attribute of type ID whose
        /// value matches the specified string.
        /// </summary>
        /// <param name='id'>
        /// A string representing the ID value of the node to which you want to move.
        /// </param>
        /// <returns>
        /// true if the move was successful; otherwise false. If false, the
        /// position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the namespace node with the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// true if the move was successful; false if a matching
        /// namespace node was not found or if the navigator is
        /// not positioned on an element node
        /// </returns>
        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        /// <summary>
        /// Moves to the next sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no more siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToNext()
        {
            if (_current == _root) return false;
            DependencyObject next = GetNthVisual(VisualTreeHelper.GetParent(_current), _indexInParent + 1);
            if (next == null) return false;
            _current = next;
            _indexInParent++;
            return true;
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the next namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// namespace node; false if there are no more namespace nodes.
        /// </returns>
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the parent of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the parent node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToParent()
        {
            if (_current == null) return false;
            if (_current == _root) return false;
            if (VisualTreeHelper.GetParent(_current) == null) return false;
            _current = VisualTreeHelper.GetParent(_current);
            _indexInParent = FindVisualIndex(_current);
            return true;
        }

        /// <summary>
        /// Moves to the previous sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no previous siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToPrevious()
        {
            if (_current == _root) return false;
            if (_indexInParent == 0) return false;
            DependencyObject prev = GetNthVisual(VisualTreeHelper.GetParent(_current), _indexInParent - 1);
            if (prev == null) return false;
            _current = prev;
            _indexInParent--;
            return true;
        }

        /// <summary>
        /// Moves to the root node to which the current node belongs.
        /// </summary>
        public override void MoveToRoot()
        {
            _current = _root;
        }

        #endregion XPathNavigator abstract implementations.

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// Visual the navigator is placed on.
        /// </summary>
        public DependencyObject CurrentVisual
        {
            get { return _current; }
        }

        #region Node properties.

        public override string BaseURI
        {
            get { return String.Empty; }
        }

        public override bool HasAttributes
        {
            get { return false; }
        }

        public override bool HasChildren
        {
            get { return (_current != null) && (VisualTreeHelper.GetChildrenCount(_current)>0); }
        }

        public override bool IsEmptyElement
        {
            get { return !HasChildren; }
        }

        public override string LocalName
        {
            get
            {
                return Name;
            }
        }

        public override string Name
        {
            get
            {
                string name = _current.GetType().Name;
                _nametable.Add(name);
                return _nametable.Get(name);
            }
        }

        public override string NamespaceURI
        {
            get { return String.Empty; }
        }

        public override XmlNameTable NameTable
        {
            get { return _nametable; }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                if (_current == _root)
                {
                    return XPathNodeType.Root;
                }
                else
                {
                    return XPathNodeType.Element;
                }
            }
        }

        public override string Prefix
        {
            get { return String.Empty; }
        }

        public override string Value
        {
            get { return String.Empty; }
        }

        public override string XmlLang
        {
            get { return "en-us"; }
        }

        #endregion Node properties.

        #endregion Public properties.

        #region Private methods.

        /// <summary>
        /// Gets the nth child visual from the specified visual.
        /// </summary>
        private DependencyObject GetNthVisual(DependencyObject parent, int index)
        {
            // Common base class for Visual and Visual3D is DependencyObject

            int enumIndex = 0;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for(int i = 0; i < count; i++)
            {
                if (index == enumIndex)
                {
                    return VisualTreeHelper.GetChild(parent, i);
                }
                enumIndex++;
            }
            return null;
        }

        private int FindVisualIndex(DependencyObject visual)
        {
            // Common base class for Visual and Visual3D is DependencyObject

            DependencyObject parent = VisualTreeHelper.GetParent(visual);
            if (parent == null) return 0;
            int enumIndex = 0;
            int count = VisualTreeHelper.GetChildrenCount(parent);
            for(int i = 0; i < count; i++)
            {
                DependencyObject v = VisualTreeHelper.GetChild(parent, i);
                if (v == visual) return enumIndex;
                enumIndex++;
            }
            System.Diagnostics.Debug.Assert(false, "Unable to find visual in parent.");
            return 0;
        }

        #endregion Private methods.

        #region Private fields.

        // Common base class for Visual and Visual3D is DependencyObject
        private DependencyObject _current;
        private DependencyObject _root;
        private int _indexInParent;

        /// <summary>Name table for names. Allows object-based comparisons for performance.</summary>
        private NameTable _nametable;

        #endregion Private fields.
    }
    
    /// <summary>
    /// Implements an XPathNavigator type that navigates a TextContainer.
    /// </summary>
    /// <remarks>
    /// Positions in the document are tracked by a TextPointer placed
    /// right after the start tag of an element.
    /// </remarks>
    class TextContainerXPathNavigator: XPathNavigator
    {
        #region Constructors.

        /// <summary>
        /// Creates a new VisualXPathNavigator instance rooted at the
        /// specified IVisual element.
        /// </summary>
        /// <param name='textRange'>TextRange for navigation.</param>
        public TextContainerXPathNavigator(TextRange textRange)
        {
            if (textRange == null)
            {
                throw new ArgumentNullException("textRange");
            }

            _textRange = textRange;
            _current = textRange.Start;
            _nametable = new NameTable();
            
            CheckInvariants();
        }

        /// <summary>Private copy constructor.</summary>
        private TextContainerXPathNavigator(TextContainerXPathNavigator other)
        {
            System.Diagnostics.Debug.Assert(other != null);

            _textRange = new TextRange(other._textRange.Start, other._textRange.End);
            _current = other._current;
            _nametable = other._nametable;

            _nametable.Add(String.Empty);
            
            CheckInvariants();
        }

        #endregion Constructors.

        #region Public methods.

        #region XPathNavigator abstract implementations.

        public override XPathNavigator Clone()
        {
            return new TextContainerXPathNavigator(this);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified LocalName and
        /// NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// </returns>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the value of the namespace node corresponding to the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// The value of the namespace node; String.Empty if a matching
        /// namespace node is not found or if the navigator is not
        /// positioned on an element node.
        /// </returns>
        public override string GetNamespace(string name)
        {
            return String.Empty;
        }

        /// <summary>
        /// Determines whether the current XPathNavigator is at the same
        /// position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>The XPathNavigator that you want to compare against.</param>
        /// <returns>true if the two navigators have the same position; otherwise, false.</returns>
        public override bool IsSamePosition(XPathNavigator other)
        {
            TextContainerXPathNavigator o;
            
            o = other as TextContainerXPathNavigator;
            return (o != null) && (o._current.CompareTo(this._current) == 0);
        }

        /// <summary>
        /// Moves to the same position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>
        /// The XPathNavigator positioned on the node that you want to move to.
        /// </param>
        /// <returns>
        /// true if successful; otherwise false. If false, the position
        /// of the navigator is unchanged.
        /// </returns>
        public override bool MoveTo(XPathNavigator other)
        {
            TextContainerXPathNavigator o;
            
            o = other as TextContainerXPathNavigator;
            if (o != null)
            {
                this._current = o._current;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the attribute with matching LocalName and NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false,
        /// the position of the navigator does not change.
        /// </returns>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            return false;
        }

        /// <summary>
        /// Moves to the first sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first sibling node;
        /// false if there is no first sibling or if the navigator is currently
        /// positioned on an attribute node.
        /// </returns>
        public override bool MoveToFirst()
        {
            bool result;
            
            CheckInvariants();

            if (_current.CompareTo(_textRange.Start) == 0) return false;
            
            // Move before our own element and into our parent,
            // then move to the first child.
            _current = ((TextElement)_current.Parent).ElementStart;
            _current = ((TextElement)_current.Parent).ContentStart;
            result = MoveToFirstChild();
            
            System.Diagnostics.Debug.Assert(result);
            
            return true;
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first attribute;
        /// otherwise, false.
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves to the first child of the current node.
        /// </summary>
        /// <returns>
        /// true if there is a first child node; otherwise false.
        /// </returns>
        public override bool MoveToFirstChild()
        {
            TextPointer initialPosition;   // Position to return to if not moved.

            CheckInvariants();
            
            initialPosition = _current;
            
            if (MoveIntoChild(_current, LogicalDirection.Forward))
            {
                return true;
            }
            else
            {
                _current = initialPosition;            
                return false;
            }
        }

        /// <summary>
        /// Moves the XPathNavigator to the first namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the first
        /// namespace node; otherwise, false.
        /// </returns>
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the node that has an attribute of type ID whose
        /// value matches the specified string.
        /// </summary>
        /// <param name='id'>
        /// A string representing the ID value of the node to which you want to move.
        /// </param>
        /// <returns>
        /// true if the move was successful; otherwise false. If false, the
        /// position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the namespace node with the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// true if the move was successful; false if a matching
        /// namespace node was not found or if the navigator is
        /// not positioned on an element node
        /// </returns>
        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        /// <summary>
        /// Moves to the next sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no more siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToNext()
        {
            TextPointer initialPosition;   // Position to return to if not moved.

            CheckInvariants();
            
            if (_current.CompareTo(_textRange.Start) == 0)
            {
                return false;
            }
            
            initialPosition = _current;
            
            // Move into parent context, outside of the current element.
            _current = ((TextElement)_current.Parent).ElementEnd;

            // Move into the next child of our parent == our sibling.
            if (MoveIntoChild(_current, LogicalDirection.Forward))
            {
                return true;
            }
            else
            {
                _current = initialPosition;            
                return false;
            }
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the next namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// namespace node; false if there are no more namespace nodes.
        /// </returns>
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the parent sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the parent
        /// sibling node
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToParent()
        {
            CheckInvariants();
            
            if (_current.CompareTo(_textRange.Start) == 0) return false;

            _current = ((TextElement)_current.Parent).ElementStart;
            _current = ((TextElement)_current.Parent).ContentStart;

            return true;
        }

        /// <summary>
        /// Moves to the previous sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no previous siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToPrevious()
        {
            TextPointer initialPosition;   // Position to return to if not moved.

            CheckInvariants();

            if (_current.CompareTo(_textRange.Start) == 0)
            {
                return false;
            }

            initialPosition = _current;

            // Move into parent context, outside of the current element.
            _current = ((TextElement)_current.Parent).ElementStart;

            // Move into the next child of our parent == our sibling.
            if (MoveIntoChild(_current, LogicalDirection.Backward))
            {
                return true;
            }
            else
            {
                _current = initialPosition;            
                return false;
            }
        }

        /// <summary>
        /// Moves to the root node to which the current node belongs.
        /// </summary>
        public override void MoveToRoot()
        {
            _current = _textRange.Start;
        }

        #endregion XPathNavigator abstract implementations.

        #endregion Public methods.

        #region Public properties.

        /// <summary>
        /// TextRange that encompasses the element contents (from 
        /// AfterStart to BeforeEnd), possibly all text range if the
        /// position is at the root level.
        /// </summary>
        public TextRange CurrentRange
        {
            get
            {
                TextPointer elementStart;
                TextPointer elementEnd;
                
                CheckInvariants();

                if (_current.CompareTo(_textRange.Start) == 0)
                {
                    return new TextRange(_textRange.Start, _textRange.End);
                }
                else
                {
                    elementStart = _current;
                    elementEnd = _current;
                    elementEnd = ((TextElement)_current.Parent).ContentEnd;
                    return new TextRange(elementStart, elementEnd);
                }
            }
        }

        /// <summary>TextPointer the navigator is placed on, null for root.</summary>
        public TextPointer CurrentPosition
        {
            get { return _current; }
        }

        #region Node properties.

        public override string BaseURI
        {
            get { return String.Empty; }
        }

        public override bool HasAttributes
        {
            get { return false; }
        }

        public override bool HasChildren
        {
            get
            {
                TextPointer navigator;
                
                CheckInvariants();
                
                navigator = _current;
                return MoveIntoChild(navigator, LogicalDirection.Forward);
            }
        }

        public override bool IsEmptyElement
        {
            get { return !HasChildren; }
        }

        public override string LocalName
        {
            get
            {
                return Name;
            }
        }

        public override string Name
        {
            get
            {
                string name;
                
                if (_current.CompareTo(_textRange.Start) == 0)
                {
                    name = "Root";
                }
                else
                {
                    name = _current.GetAdjacentElement(LogicalDirection.Forward).GetType().Name;
                }
                _nametable.Add(name);
                return _nametable.Get(name);
            }
        }

        public override string NamespaceURI
        {
            get { return String.Empty; }
        }

        public override XmlNameTable NameTable
        {
            get { return _nametable; }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                if (_current.CompareTo(_textRange.Start) == 0)
                {
                    return XPathNodeType.Root;
                }
                else
                {
                    return XPathNodeType.Element;
                }
            }
        }

        public override string Prefix
        {
            get { return String.Empty; }
        }

        public override string Value
        {
            get { return String.Empty; }
        }

        public override string XmlLang
        {
            get { return "en-us"; }
        }

        #endregion Node properties.

        #endregion Public properties.
        
        #region Private methods.

        /// <summary>Verifies that object invariants hold.</summary>
        private void CheckInvariants()
        {
            System.Diagnostics.Debug.Assert(_nametable != null);
            System.Diagnostics.Debug.Assert(_textRange != null);
            System.Diagnostics.Debug.Assert(_current != null);
            System.Diagnostics.Debug.Assert(_current.CompareTo(_textRange.Start) == 0 ||
                _current.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart);
        }

        /// <summary>
        /// Moves the specified navigator into the first child element
        /// found at the current scope in the given direction.
        /// </summary>
        /// <param name='navigator'>Navigator to move.</param>
        /// <param name='direction'>Direction in which to move.</param>
        /// <returns>true if a child was found, false otherwise.</returns>
        /// <remarks>
        /// If the method returns false, the navigator position is undefined.
        /// The navigator will be at the position immediately following 
        /// the opening tag of the child.
        /// </remarks>
        private static bool MoveIntoChild(TextPointer navigator, 
            LogicalDirection direction)
        {
            bool found;                             // Whether the child was found.
            TextPointerContext symbolSought;            // Symbol that indicates match.
            TextPointerContext oppositeSymbolSought;    // Opposite of symbolSought.
            TextPointerContext nextSymbol;              // Next symbol in direction.
            
            System.Diagnostics.Debug.Assert(navigator != null);
            
            symbolSought = (direction == LogicalDirection.Forward)?
                TextPointerContext.ElementStart : TextPointerContext.ElementEnd;
            oppositeSymbolSought = (direction == LogicalDirection.Forward)?
                TextPointerContext.ElementEnd : TextPointerContext.ElementStart;
            
            found = false;
            do
            {
                nextSymbol = navigator.GetPointerContext(direction);
                if (nextSymbol == symbolSought)
                {
                    found = true;
                }
                else if (nextSymbol == TextPointerContext.None || nextSymbol == oppositeSymbolSought)
                {
                    return false;
                }
                navigator = navigator.GetNextContextPosition(direction);
            } while (!found);
            
            if (direction == LogicalDirection.Backward)
            {
                navigator = ((TextElement)navigator.Parent).ContentStart;
            }

            System.Diagnostics.Debug.Assert(
                navigator.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart);

            return true;
        }
        
        #endregion Private methods.

        #region Private fields.

        /// <summary>Current navigator position.</summary>
        private TextPointer _current;
        
        /// <summary>Root of tree (container).</summary>
        private TextRange _textRange;

        /// <summary>Name table for names. Allows object-based comparisons for performance.</summary>
        private NameTable _nametable;

        #endregion Private fields.
    }

    /// <summary>
    /// Implements an XPathNavigator type that navigates a TextElement tree.
    /// </summary>
    class TextElementXPathNavigator : XPathNavigator
    {
        #region Constructors.

        /// <summary>
        /// Creates a new TextElementXPathNavigator instance rooted at the
        /// specified text element.
        /// </summary>
        /// <param name='element'>Element for navigation.</param>
        public TextElementXPathNavigator(FrameworkContentElement element)
        {
            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            _current = element;
            _root = element;
            _nametable = new NameTable();

            CheckInvariants();
        }

        /// <summary>Private copy constructor.</summary>
        private TextElementXPathNavigator(TextElementXPathNavigator other)
        {
            System.Diagnostics.Debug.Assert(other != null);

            _current = other._current;
            _root = other._root;
            _nametable = other._nametable;

            _nametable.Add(String.Empty);

            CheckInvariants();
        }

        #endregion Constructors.

        #region Public methods.

        #region XPathNavigator abstract implementations.

        public override XPathNavigator Clone()
        {
            return new TextElementXPathNavigator(this);
        }

        /// <summary>
        /// Gets the value of the attribute with the specified LocalName and
        /// NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// </returns>
        public override string GetAttribute(string localName, string namespaceUri)
        {
            return String.Empty;
        }

        /// <summary>
        /// Returns the value of the namespace node corresponding to the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// The value of the namespace node; String.Empty if a matching
        /// namespace node is not found or if the navigator is not
        /// positioned on an element node.
        /// </returns>
        public override string GetNamespace(string name)
        {
            return String.Empty;
        }

        /// <summary>
        /// Determines whether the current XPathNavigator is at the same
        /// position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>The XPathNavigator that you want to compare against.</param>
        /// <returns>true if the two navigators have the same position; otherwise, false.</returns>
        public override bool IsSamePosition(XPathNavigator other)
        {
            TextElementXPathNavigator o;

            o = other as TextElementXPathNavigator;
            return (o != null) && (o._current == this._current);
        }

        /// <summary>
        /// Moves to the same position as the specified XPathNavigator.
        /// </summary>
        /// <param name='other'>
        /// The XPathNavigator positioned on the node that you want to move to.
        /// </param>
        /// <returns>
        /// true if successful; otherwise false. If false, the position
        /// of the navigator is unchanged.
        /// </returns>
        public override bool MoveTo(XPathNavigator other)
        {
            TextElementXPathNavigator o;

            o = other as TextElementXPathNavigator;
            if (o != null)
            {
                this._current = o._current;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the attribute with matching LocalName and NamespaceURI.
        /// </summary>
        /// <param name='localName'>The local name of the attribute.</param>
        /// <param name='namespaceUri'>The namespace URI of the attribute.</param>
        /// <returns>
        /// true if the attribute is found; otherwise, false. If false,
        /// the position of the navigator does not change.
        /// </returns>
        public override bool MoveToAttribute(string localName, string namespaceUri)
        {
            return false;
        }

        /// <summary>
        /// Moves to the first sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first sibling node;
        /// false if there is no first sibling or if the navigator is currently
        /// positioned on an attribute node.
        /// </returns>
        public override bool MoveToFirst()
        {
            TextElement parent;

            CheckInvariants();

            if (_current == _root) return false;

            parent = (TextElement)_current.Parent;
            if (((TextElement)parent).ContentStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                _current = (TextElement)parent.ContentStart.GetAdjacentElement(LogicalDirection.Forward);
            }
            else
            {
                return false;
            }

            return true;
        }

        /// <summary>
        /// Moves to the first attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the first attribute;
        /// otherwise, false.
        /// </returns>
        public override bool MoveToFirstAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves to the first child of the current node.
        /// </summary>
        /// <returns>
        /// true if there is a first child node; otherwise false.
        /// </returns>
        public override bool MoveToFirstChild()
        {
            TextElement child =null;

            CheckInvariants();

            if (ContentStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                child = ContentStart.GetAdjacentElement(LogicalDirection.Forward) as TextElement;
            }

            if (child != null)
            {
                _current = child;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves the XPathNavigator to the first namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the first
        /// namespace node; otherwise, false.
        /// </returns>
        public override bool MoveToFirstNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the node that has an attribute of type ID whose
        /// value matches the specified string.
        /// </summary>
        /// <param name='id'>
        /// A string representing the ID value of the node to which you want to move.
        /// </param>
        /// <returns>
        /// true if the move was successful; otherwise false. If false, the
        /// position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToId(string id)
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the namespace node with the
        /// specified local name.
        /// </summary>
        /// <param name='name'>The local name of the namespace node.</param>
        /// <returns>
        /// true if the move was successful; false if a matching
        /// namespace node was not found or if the navigator is
        /// not positioned on an element node
        /// </returns>
        public override bool MoveToNamespace(string name)
        {
            return false;
        }

        /// <summary>
        /// Moves to the next sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no more siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToNext()
        {
            CheckInvariants();

            if (_current == _root)
            {
                return false;
            }
            else if (ElementEnd.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
            {
                _current = (FrameworkContentElement)ElementEnd.GetAdjacentElement(LogicalDirection.Forward);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the next attribute.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// attribute; false if there are no more attributes.
        /// </returns>
        public override bool MoveToNextAttribute()
        {
            return false;
        }

        /// <summary>
        /// Moves the XPathNavigator to the next namespace node matching
        /// the XPathNamespaceScope specified.
        /// </summary>
        /// <param name='namespaceScope'>
        /// An XPathNamespaceScope value describing the namespace scope.
        /// </param>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// namespace node; false if there are no more namespace nodes.
        /// </returns>
        public override bool MoveToNextNamespace(XPathNamespaceScope namespaceScope)
        {
            return false;
        }

        /// <summary>
        /// Moves to the parent sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the parent
        /// sibling node
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToParent()
        {
            CheckInvariants();

            if (_current == _root) return false;

            _current = (FrameworkContentElement)_current.Parent;

            return true;
        }

        /// <summary>
        /// Moves to the previous sibling of the current node.
        /// </summary>
        /// <returns>
        /// true if the navigator is successful moving to the next
        /// sibling node; false if there are no previous siblings or if
        /// the navigator is currently positioned on an attribute node.
        /// If false, the position of the navigator is unchanged.
        /// </returns>
        public override bool MoveToPrevious()
        {
            CheckInvariants();

            if (ElementStart.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementEnd)
            {
                _current = (FrameworkContentElement)ElementStart.GetAdjacentElement(LogicalDirection.Backward);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Moves to the root node to which the current node belongs.
        /// </summary>
        public override void MoveToRoot()
        {
            _current = _root;
        }

        #endregion XPathNavigator abstract implementations.

        #endregion Public methods.

        #region Public properties.

        /// <summary>Current TextElement the navigator is on.</summary>
        public FrameworkContentElement CurrentElement
        {
            get
            {
                return _current;
            }
        }

        /// <summary>Gets the ContentEnd value for the CurrentElement.</summary>
        public TextPointer ContentEnd
        {
            get
            {
                if (_current is TextElement)
                {
                    return ((TextElement)_current).ContentEnd;
                }
                else if (_current is FlowDocument)
                {
                    return ((FlowDocument)_current).ContentEnd;
                }
                else
                {
                    throw new NotSupportedException("ContentEnd not supported for " + _current.GetType());
                }
            }
        }

        /// <summary>Gets the ContentStart value for the CurrentElement.</summary>
        public TextPointer ContentStart
        {
            get
            {
                if (_current is TextElement)
                {
                    return ((TextElement)_current).ContentStart;
                }
                else if (_current is FlowDocument)
                {
                    return ((FlowDocument)_current).ContentStart;
                }
                else
                {
                    throw new NotSupportedException("ContentStart not supported for " + _current.GetType());
                }
            }
        }

        /// <summary>Gets the ElementEnd value for the CurrentElement.</summary>
        public TextPointer ElementEnd
        {
            get
            {
                if (_current is TextElement)
                {
                    return ((TextElement)_current).ElementEnd;
                }
                else
                {
                    throw new NotSupportedException("ElementEnd not supported for " + _current.GetType());
                }
            }
        }

        /// <summary>Gets the ElementStart value for the CurrentElement.</summary>
        public TextPointer ElementStart
        {
            get
            {
                if (_current is TextElement)
                {
                    return ((TextElement)_current).ElementStart;
                }
                else
                {
                    throw new NotSupportedException("ElementStart not supported for " + _current.GetType());
                }
            }
        }

        #region Node properties.

        public override string BaseURI
        {
            get { return String.Empty; }
        }

        public override bool HasAttributes
        {
            get { return false; }
        }

        public override bool HasChildren
        {
            get
            {
                CheckInvariants();

                return this.ContentStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart;
            }
        }

        public override bool IsEmptyElement
        {
            get { return !HasChildren; }
        }

        public override string LocalName
        {
            get
            {
                return Name;
            }
        }

        public override string Name
        {
            get
            {
                string name;

                name = _current.GetType().Name;
                _nametable.Add(name);
                return _nametable.Get(name);
            }
        }

        public override string NamespaceURI
        {
            get { return String.Empty; }
        }

        public override XmlNameTable NameTable
        {
            get { return _nametable; }
        }

        public override XPathNodeType NodeType
        {
            get
            {
                if (_current == _root)
                {
                    return XPathNodeType.Root;
                }
                else
                {
                    return XPathNodeType.Element;
                }
            }
        }

        public override string Prefix
        {
            get { return String.Empty; }
        }

        public override string Value
        {
            get { return String.Empty; }
        }

        public override string XmlLang
        {
            get { return "en-us"; }
        }

        #endregion Node properties.

        #endregion Public properties.

        #region Private methods.

        /// <summary>Verifies that object invariants hold.</summary>
        private void CheckInvariants()
        {
            System.Diagnostics.Debug.Assert(_nametable != null);
            System.Diagnostics.Debug.Assert(_root != null);
            System.Diagnostics.Debug.Assert(_current != null);
        }

        #endregion Private methods.

        #region Private fields.

        /// <summary>Current navigator position.</summary>
        private FrameworkContentElement _current;

        /// <summary>Root of tree (container).</summary>
        private FrameworkContentElement _root;

        /// <summary>Name table for names. Allows object-based comparisons for performance.</summary>
        private NameTable _nametable;

        #endregion Private fields.
    }

    #endregion Navigator implementations.
}
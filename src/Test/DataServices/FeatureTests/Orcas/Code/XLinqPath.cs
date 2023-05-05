// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Codeplex;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using System.Xml.Linq;

namespace Microsoft.Test.DataServices
{
	// These test cases should have always run in Full Trust.
    [Test(2, "Xml", "XLinqPathTest", SecurityLevel=TestCaseSecurityLevel.FullTrust)]
    public class XLinqPath : XamlTest
    {
        #region Private Data

        private ListBox _listBox;
        private XElement _xElement;
        private string _listBoxName;
        private TextBlock _textBlock;

        #endregion

        #region Constructors

        [Variation("NamespaceMultipleElements")]
        [Variation("MultipleElements")]
        [Variation("NamespaceAttributeElement")]
        [Variation("AttributeElement")]
        [Variation("XmlElement")]
        [Variation("AllElements")]
        [Variation("NamespaceDescendants")]
        [Variation("Descendants")]
        [Variation("AllDescendants")]
        public XLinqPath(string listBoxName)
            : base(@"Markup\XLinqPath.xaml")
        {
            this._listBoxName = listBoxName;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(InitialUI);
            RunSteps += new TestStep(ChangeUI);
            RunSteps += new TestStep(ChangeXElement);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            // The XAML contains a set of listboxes, each named according to
            // which synthetic property it uses in its path. We can therefore
            // get a handle to the appropriate ListBox via LogicalTreeHelper.
            _listBox = (ListBox)LogicalTreeHelper.FindLogicalNode(RootElement, _listBoxName);

            // The XLinqDataProvider is an DataSourceProvider that allows us to
            // easily bind to XLinq. The provider returns a collection of
            // XElements corresponding to the top level of the XML. Since well
            // formed XML has a single root, we grab the first element of the
            // collection.
            XLinqDataProvider xLinqDataProvider = RootElement.FindResource("XLinqPath") as XLinqDataProvider;
            _xElement = ((ObservableCollection<XElement>)DataSourceHelper.WaitForData(xLinqDataProvider))[0];

            return TestResult.Pass;
        }

        /// <summary>
        /// Verify the initial UI of the ListBox reflects the appropriate
        /// binding.
        /// </summary>
        /// <returns>Result.</returns>
        private TestResult InitialUI()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            // The DataTemplate for each data item is TextBlock bound to the
            // relevant synthetic property.
            _textBlock = (TextBlock)Util.FindDataVisual(_listBox, _listBox.Items[0]);

            bool passes = false;

            // For each synthetic property we validate that the first item
            // returned was the correct one by looking at the text of the
            // TextBlock which is the result of the DataTemplate applied to
            // the first item. We also verify that the correct number of items
            // were returned by looking at the number of items in the ListBox.
            // For example, when the ListBox bound using the Elements synthetic
            // property, we expect to see 7 ListBox items because that is how
            // many children the root element had, and the text should be
            // 'Namespace Multiple Elements 1' because that is the result of
            // the DataTemplate for the first item.
            switch (_listBoxName)
            {
                case "NamespaceMultipleElements":
                    passes = (_textBlock.Text == "Namespace Multiple Elements 1") && (_listBox.Items.Count == 2);
                    break;
                case "MultipleElements":
                    passes = (_textBlock.Text == "Multiple Elements 1") && (_listBox.Items.Count == 2);
                    break;
                case "NamespaceAttributeElement":
                    passes = (_textBlock.Text == "Namespace Attribute") && (_listBox.Items.Count == 1);
                    break;
                case "AttributeElement":
                    passes = (_textBlock.Text == "Attribute") && (_listBox.Items.Count == 1);
                    break;
                case "XmlElement":
                    passes = (_textBlock.Text == @"<XmlElement><XmlElementValue>XmlElement Text</XmlElementValue></XmlElement>") && (_listBox.Items.Count == 1);
                    break;
                case "AllElements":
                    passes = (_textBlock.Text == "Namespace Multiple Elements 1") && (_listBox.Items.Count == 7);
                    break;
                case "NamespaceDescendants":
                    passes = (_textBlock.Text == "Namespace Multiple Elements 1") && (_listBox.Items.Count == 2);
                    break;
                case "Descendants":
                    passes = (_textBlock.Text == "Multiple Elements 1") && (_listBox.Items.Count == 2);
                    break;
                case "AllDescendants":
                    passes = (_textBlock.Text == "Namespace Multiple Elements 1") && (_listBox.Items.Count == 12);
                    break;
                default:
                    TestLog.Current.LogEvidence(_listBoxName + " was an unexpected case.");
                    break;
            }

            if (passes)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /// <summary>
        /// Change the value of the UI and verify the change is propogated back
        /// to the underlying XElement.
        /// </summary>
        /// <returns>Result.</returns>
        private TestResult ChangeUI()
        {
            _textBlock.Text = "New Value";
            WaitForPriority(DispatcherPriority.SystemIdle);

            bool passes = false;

            XNamespace ns = "http://namespace";

            // Similar to verifying the initial UI, now we are verifying that
            // the part of the XElement that our TextBlock is bound to now has
            // the new value.
            switch (_listBoxName)
            {
                case "NamespaceMultipleElements":
                    passes = _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value == "New Value";
                    break;
                case "MultipleElements":
                    passes = _xElement.Element("MultipleElements").Element("ElementValue").Value == "New Value";
                    break;
                case "NamespaceAttributeElement":
                    passes = _xElement.Element("NamespaceAttributeElement").Attribute(ns + "AttributeValue").Value == "New Value";
                    break;
                case "AttributeElement":
                    passes = _xElement.Element("AttributeElement").Attribute("AttributeValue").Value == "New Value";
                    break;
                case "XmlElement":
                    // The Xml synthetic property is read-only, so the binding was only one-way, so the value shouldn't change.
                    passes = _xElement.Element("XmlElement").ToString(SaveOptions.DisableFormatting) == @"<XmlElement><XmlElementValue>XmlElement Text</XmlElementValue></XmlElement>";
                    break;
                case "AllElements":
                    passes = _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value == "New Value";
                    break;
                case "NamespaceDescendants":
                    passes = _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value == "New Value";
                    break;
                case "Descendants":
                    passes = _xElement.Element("MultipleElements").Element("ElementValue").Value == "New Value";
                    break;
                case "AllDescendants":
                    passes = _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value == "New Value";
                    break;
                default:
                    TestLog.Current.LogEvidence(_listBoxName + " was an unexpected case.");
                    break;
            }

            if (passes)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        /// <summary>
        /// Change the value of the underlying XElement and verify the change
        /// is propogated to the UI.
        /// </summary>
        /// <returns>Result.</returns>
        private TestResult ChangeXElement()
        {
            XNamespace ns = "http://namespace";

            // This time the switch statement isn't validation, but is to set
            // the value in the part of the XElement that the TextBlock is
            // bound to.
            switch (_listBoxName)
            {
                case "NamespaceMultipleElements":
                    _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value = "Changed XElement";
                    break;
                case "MultipleElements":
                    _xElement.Element("MultipleElements").Element("ElementValue").Value = "Changed XElement";
                    break;
                case "NamespaceAttributeElement":
                    _xElement.Element("NamespaceAttributeElement").Attribute(ns + "AttributeValue").Value = "Changed XElement";
                    break;
                case "AttributeElement":
                    _xElement.Element("AttributeElement").Attribute("AttributeValue").Value = "Changed XElement";
                    break;
                case "XmlElement":
                    _xElement.Element("XmlElement").Value = "Changed XElement";
                    break;
                case "AllElements":
                    _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value = "Changed XElement";
                    break;
                case "NamespaceDescendants":
                    _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value = "Changed XElement";
                    break;
                case "Descendants":
                    _xElement.Element("MultipleElements").Element("ElementValue").Value = "Changed XElement";
                    break;
                case "AllDescendants":
                    _xElement.Element(ns + "MultipleElements").Element(ns + "ElementValue").Value = "Changed XElement";
                    break;
                default:
                    TestLog.Current.LogEvidence(_listBoxName + " was an unexpected case.");
                    break;
            }

            // Validation is the simple part this time.
            if (_textBlock.Text == "Changed XElement")
            {
                return TestResult.Pass;
            }
            // Since XmlElement synthetic property is read only the binding was one-way, so when I set it earlier, it clubbed the binding.
            else if (_listBoxName == "XmlElement" && _textBlock.Text == "New Value")
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        #endregion
    }
}

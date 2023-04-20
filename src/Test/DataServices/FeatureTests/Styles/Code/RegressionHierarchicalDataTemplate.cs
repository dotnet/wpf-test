// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Coverage for HierarchicalDataTemplate bugs
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(2, "Styles", "RegressionHierarchicalDataTemplate")]
    public class RegressionHierarchicalDataTemplate : XamlTest
    {
        #region Constructors

        public RegressionHierarchicalDataTemplate()
            : base(@"RegressionHierarchicalDataTemplate.xaml")
        {
            RunSteps += new TestStep(ItemContainerStyleandSelector);
            RunSteps += new TestStep(ItemStringFormatParseException);
        }

        #endregion

        #region Private Members

        // HierarchicalDataTemplate should set ItemContainerStyle[Selector]
        private TestResult ItemContainerStyleandSelector()
        {
            // ItemContainerStyle and Selector defined in XAML. I'm verifying basic forwarding
            // of these properties onto the TreeViewItem.
            TreeView tv = (TreeView)RootElement.FindName("ItemContainerStyleTreeView");
            TreeViewItem tvi = (TreeViewItem)tv.ItemContainerGenerator.ContainerFromItem(tv.Items[0]);
            if (tvi.ItemContainerStyle != (Style)RootElement.FindResource("ItemContainerStyleStyle"))
            {
                return TestResult.Fail;
            }

            tv = (TreeView)RootElement.FindName("ItemContainerStyleSelectorTreeView");
            tvi = (TreeViewItem)tv.ItemContainerGenerator.ContainerFromItem(tv.Items[0]);
            if (tvi.ItemContainerStyleSelector != (StyleSelector)RootElement.FindResource("ItemContainerStyleSelector"))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // HierarchicalDataTemplate.ItemStringFormat causing XamlParseException in TemplateXamlParser
        // The offending xaml snippet is part of RegressionHierarchicalDataTemplate.xaml, so being able to navigate
        // to that indicates the exception is gone.
        private TestResult ItemStringFormatParseException()
        {
            // Also test parsing the text via XamlReader.
            object parsedXaml = XamlReader.Load(new XmlTextReader(new StringReader("<HierarchicalDataTemplate ItemsSource=\"{x:Null}\" ItemTemplate=\"{x:Null}\" ItemTemplateSelector=\"{x:Null}\" ItemStringFormat=\"{x:Null}\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:x=\"http://schemas.microsoft.com/winfx/2006/xaml\" />")));
            if (parsedXaml == null) return TestResult.Fail;

            // Also look in the tree to see that the HDT got applied
            TreeView tv = (TreeView)RootElement.FindName("ItemStringFormatParseExceptionTreeView");
            if (tv.ItemTemplate != (DataTemplate)RootElement.FindResource("ItemStringFormatParseExceptionHDT"))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
    }

    // Skeleton StyleSelector whose SelectTemplate method just returns the stored Style.
    public class FixedStyleSelector : StyleSelector
    {
        #region Private Data

        private Style _style;

        #endregion

        #region Public and Protected Members

        public Style Style
        {
            get { return _style; }
            set { _style = value; }
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            return Style;
        }

        #endregion
    }
}


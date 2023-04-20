// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Xml;
using System.Threading;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests the order of precedence of different ways of applying HierarchicalDataTemplates to a TreeView.
    /// 
    /// Matrix:
    /// Data sources: ADO, Object, XML
    /// Template type: Explicit (x:Key in DataTemplate), Implicit (DataType in DataTemplate) and ItemTemplateSelector
    /// Forwarding: No forwarding (meaning no properties set on Hemisphere level), ItemTemplate forwarded to next level (this happens by default when setting the ItemTemplate at Hemisphere level), ItemTemplateSelector forwarded to next level, forwarded ItemTemplate is cancelled (done by setting ItemTemplate to a template at TreeView level and setting ItemTemplate to {x:Null} at Hemisphere level), forwarded ItemTemplateSelector is cancelled.
    /// 
    /// ItemTemplate/ItemTemplateSelector Properties are set at different levels of the hierarchy according to the following:
    /// TreeView level � set when forwarding cancelled
    /// Hemisphere level � set depending on Forwarding type
    /// Region level � set according to Template type
    /// Country � verification happens only at this level
    /// </description>
    /// <relatedBugs>



    /// </relatedBugs>
    /// </summary>
    [Test(2, "Styles", "HierarchicalDataTemplateForwarding")]
    public class HierarchicalDataTemplateForwarding : XamlTest
    {
        private DataSource _dataSource;
        private TemplateType _templateType;
        private Forwarding _forwarding;

        private enum DataSource { ADO, Object, XML, XLinq };
        private enum TemplateType { ExplicitItemTemplate, ImplicitItemTemplate, ItemTemplateSelector };
        private enum Forwarding { NoForwarding, ItemTemplateForwarded, ItemTemplateSelectorForwarded, ItemTemplateForwardingCancelled, ItemTemplateSelectorForwardingCancelled };


        // ItemTemplate/ItemTemplateSelector Forwarding Cancelling doesn't work because of Implicit Templating since
        // in ADO DataSet every level is same type. So if I make DataRowView template work for Regions then it won't
        // work for Countries, and vice versa.
        [Variation("ADO", "ExplicitItemTemplate", "NoForwarding")]
        [Variation("ADO", "ExplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("ADO", "ExplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        //[Variation("ADO", "ExplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        //[Variation("ADO", "ExplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("ADO", "ImplicitItemTemplate", "NoForwarding")]
        [Variation("ADO", "ImplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("ADO", "ImplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        //[Variation("ADO", "ImplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        //[Variation("ADO", "ImplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("ADO", "ItemTemplateSelector", "NoForwarding")]
        [Variation("ADO", "ItemTemplateSelector", "ItemTemplateForwarded")]
        [Variation("ADO", "ItemTemplateSelector", "ItemTemplateSelectorForwarded")]
        //[Variation("ADO", "ItemTemplateSelector", "ItemTemplateForwardingCancelled")]
        //[Variation("ADO", "ItemTemplateSelector", "ItemTemplateSelectorForwardingCancelled")]

        [Variation("Object", "ExplicitItemTemplate", "NoForwarding")]
        [Variation("Object", "ExplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("Object", "ExplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("Object", "ExplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("Object", "ExplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("Object", "ImplicitItemTemplate", "NoForwarding")]
        [Variation("Object", "ImplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("Object", "ImplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("Object", "ImplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("Object", "ImplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("Object", "ItemTemplateSelector", "NoForwarding")]
        [Variation("Object", "ItemTemplateSelector", "ItemTemplateForwarded")]
        [Variation("Object", "ItemTemplateSelector", "ItemTemplateSelectorForwarded")]
        [Variation("Object", "ItemTemplateSelector", "ItemTemplateForwardingCancelled")]
        [Variation("Object", "ItemTemplateSelector", "ItemTemplateSelectorForwardingCancelled")]

        [Variation("XML", "ExplicitItemTemplate", "NoForwarding")]
        [Variation("XML", "ExplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("XML", "ExplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("XML", "ExplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("XML", "ExplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("XML", "ImplicitItemTemplate", "NoForwarding")]
        [Variation("XML", "ImplicitItemTemplate", "ItemTemplateForwarded")]
        [Variation("XML", "ImplicitItemTemplate", "ItemTemplateSelectorForwarded")]
        [Variation("XML", "ImplicitItemTemplate", "ItemTemplateForwardingCancelled")]
        [Variation("XML", "ImplicitItemTemplate", "ItemTemplateSelectorForwardingCancelled")]
        [Variation("XML", "ItemTemplateSelector", "NoForwarding")]
        [Variation("XML", "ItemTemplateSelector", "ItemTemplateForwarded")]
        [Variation("XML", "ItemTemplateSelector", "ItemTemplateSelectorForwarded")]
        [Variation("XML", "ItemTemplateSelector", "ItemTemplateForwardingCancelled")]
        [Variation("XML", "ItemTemplateSelector", "ItemTemplateSelectorForwardingCancelled")]
        public HierarchicalDataTemplateForwarding(string ds, string tT, string fwd)
            : this(ds, tT, fwd, @"HierarchicalDataTemplateForwarding.xaml")
        {
        }

        public HierarchicalDataTemplateForwarding(string ds, string tT, string fwd, string xamlFile)
            : base(xamlFile)
        {
            // set private values since attribute can't handle enums
            SetPrivateValues(ds, tT, fwd);

            InitializeSteps += new TestStep(Setup);

            RunSteps += new TestStep(TestTemplate);
        }

        private void SetPrivateValues(string ds, string tT, string fwd)
        {
            switch (ds)
            {
                case "ADO":
                    _dataSource = DataSource.ADO;
                    break;
                case "Object":
                    _dataSource = DataSource.Object;
                    break;
                case "XML":
                    _dataSource = DataSource.XML;
                    break;
                case "XLinq":
                    _dataSource = DataSource.XLinq;
                    break;
            }

            switch (tT)
            {
                case "ExplicitItemTemplate":
                    _templateType = TemplateType.ExplicitItemTemplate;
                    break;
                case "ImplicitItemTemplate":
                    _templateType = TemplateType.ImplicitItemTemplate;
                    break;
                case "ItemTemplateSelector":
                    _templateType = TemplateType.ItemTemplateSelector;
                    break;
            }

            switch (fwd)
            {
                case "NoForwarding":
                    _forwarding = Forwarding.NoForwarding;
                    break;
                case "ItemTemplateForwarded":
                    _forwarding = Forwarding.ItemTemplateForwarded;
                    break;
                case "ItemTemplateSelectorForwarded":
                    _forwarding = Forwarding.ItemTemplateSelectorForwarded;
                    break;
                case "ItemTemplateForwardingCancelled":
                    _forwarding = Forwarding.ItemTemplateForwardingCancelled;
                    break;
                case "ItemTemplateSelectorForwardingCancelled":
                    _forwarding = Forwarding.ItemTemplateSelectorForwardingCancelled;
                    break;
            }
        }

        private TestResult Setup()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            return TestResult.Pass;
        }

        private TestResult TestTemplate()
        {
            Status("Begin Test");
            // For ADO.NET, since each level is a DataSet, implicit templating doesn't work quite as well.
            // So we have some special casing to get the appropriate behavior for testing what we want to

            TreeView myTreeView = (TreeView)LogicalTreeHelper.FindLogicalNode(RootElement, "My" + _dataSource.ToString() + "TreeView");

            // In ADO.NET, we need to set the TreeView's template explicitly. We'll use ItemTemplateSelector so it can be easily wiped out
            if (_dataSource == DataSource.ADO)
            {
                myTreeView.ItemTemplateSelector = (DataTemplateSelector)RootElement.Resources["ExplicitADOHemisphereSelector"];
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            // Get a handle to the TreeViewItem for the first hemisphere. We will regrab handles to TreeViewItems throughout the test
            // whenever we do something that can cause the Tree to be regenerated (such as setting an ItemTemplate), because that
            // TreeViewItem is then a dead reference.
            TreeViewItem hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);

            // In ADO.NET, we need to set the TreeViewItem's template explicitly. We'll use ItemTemplateSelector so it can be easily wiped out
            if (_dataSource == DataSource.ADO)
            {
                hemisphereTreeViewItem.ItemTemplateSelector = (DataTemplateSelector)RootElement.Resources["ExplicitADORegionSelector"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
            }

            // If we are testing Template Forwarding, we need to set the some parental templates explicitly
            if (_forwarding == Forwarding.ItemTemplateForwarded)
            {
                hemisphereTreeViewItem.ItemTemplate = (DataTemplate)RootElement.Resources["Explicit" + _dataSource.ToString() + "Region"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
            }
            else if (_forwarding == Forwarding.ItemTemplateSelectorForwarded)
            {
                hemisphereTreeViewItem.ItemTemplateSelector = (DataTemplateSelector)RootElement.Resources["Explicit" + _dataSource.ToString() + "RegionSelector"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
            }
            else if (_forwarding == Forwarding.ItemTemplateForwardingCancelled)
            {
                myTreeView.ItemTemplate = (DataTemplate)RootElement.Resources["Explicit" + _dataSource.ToString() + "Hemisphere"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
                hemisphereTreeViewItem.ItemTemplate = null;
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
            }
            else if (_forwarding == Forwarding.ItemTemplateSelectorForwardingCancelled)
            {
                myTreeView.ItemTemplateSelector = (DataTemplateSelector)RootElement.Resources["Explicit" + _dataSource.ToString() + "HemisphereSelector"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
                hemisphereTreeViewItem.ItemTemplateSelector = null;
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
            }

            // Open up the first TreeViewItem
            hemisphereTreeViewItem.IsExpanded = true;
            WaitForPriority(DispatcherPriority.SystemIdle);
            TreeViewItem regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);

            // Specify explicit template if we are testing that
            // We'll then grab new handles to Hemisphere and Region TreeViewItems since template change regenerated tree.
            if (_templateType == TemplateType.ExplicitItemTemplate)
            {
                regionTreeViewItem.ItemTemplate = (DataTemplate)RootElement.Resources["Explicit" + _dataSource.ToString() + "Country"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
                // Open up the first TreeViewItem
                hemisphereTreeViewItem.IsExpanded = true;
                WaitForPriority(DispatcherPriority.SystemIdle);
                regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);
            }
            else if (_templateType == TemplateType.ItemTemplateSelector)
            {
                regionTreeViewItem.ItemTemplateSelector = (DataTemplateSelector)RootElement.Resources["Explicit" + _dataSource.ToString() + "CountrySelector"];
                WaitForPriority(DispatcherPriority.SystemIdle);
                hemisphereTreeViewItem = (TreeViewItem)myTreeView.ItemContainerGenerator.ContainerFromItem(myTreeView.Items[0]);
                // Open up the first TreeViewItem
                hemisphereTreeViewItem.IsExpanded = true;
                WaitForPriority(DispatcherPriority.SystemIdle);
                regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);
            }

            // In ADO.NET, if we want to test direct/forwarding cancelled implicit item templates, we need to turn off the ItemTemplate and ItemTemplateSelector on the regionTreeViewItem
            if (_dataSource == DataSource.ADO && _templateType == TemplateType.ImplicitItemTemplate && _forwarding != Forwarding.ItemTemplateForwarded && _forwarding != Forwarding.ItemTemplateSelectorForwarded)
            {
                regionTreeViewItem.ItemTemplate = null;
                WaitForPriority(DispatcherPriority.SystemIdle);
                regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);
                regionTreeViewItem.ItemTemplateSelector = null;
                WaitForPriority(DispatcherPriority.SystemIdle);
                regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);
                // Open up the first TreeViewItem
                hemisphereTreeViewItem.IsExpanded = true;
                WaitForPriority(DispatcherPriority.SystemIdle);
                regionTreeViewItem = (TreeViewItem)hemisphereTreeViewItem.ItemContainerGenerator.ContainerFromItem(hemisphereTreeViewItem.Items[0]);
            }

            // Open up the second TreeViewItem
            regionTreeViewItem.IsExpanded = true;
            WaitForPriority(DispatcherPriority.SystemIdle);

            TreeViewItem countryTreeViewItem = (TreeViewItem)regionTreeViewItem.ItemContainerGenerator.ContainerFromItem(regionTreeViewItem.Items[0]);

            StackPanel countryStackPanel = (StackPanel)Util.FindDataVisual(countryTreeViewItem, regionTreeViewItem.Items[0]);

            TextBlock countryTextBlockOne = (TextBlock)countryStackPanel.Children[0];
            TextBlock countryTextBlockTwo = (TextBlock)countryStackPanel.Children[1];

            String expectedTextBlockOne = "unset";
            String expectedTextBlockTwo = "unset";
            // If we are forwarding an ItemTemplate, it overrides unless we are specifying an ItemTemplate on our TreeViewItem
            if (_forwarding == Forwarding.ItemTemplateForwarded && _templateType != TemplateType.ExplicitItemTemplate)
            {
                expectedTextBlockOne = "";
                expectedTextBlockTwo = "Explicit" + _dataSource.ToString() + "Region";
            }
            // If we are forwarding an ItemTemplateSelector, it overrides unless we are specifying either an ItemTemplate or ItemTemplateSelector on our TreeViewItem
            else if (_forwarding == Forwarding.ItemTemplateSelectorForwarded && _templateType != TemplateType.ExplicitItemTemplate && _templateType != TemplateType.ItemTemplateSelector)
            {
                expectedTextBlockOne = "";
                expectedTextBlockTwo = "Explicit" + _dataSource.ToString() + "RegionSelector";
            }
            else if (_templateType == TemplateType.ExplicitItemTemplate)
            {
                expectedTextBlockOne = "Saint Lucia";
                expectedTextBlockTwo = "Explicit" + _dataSource.ToString() + "Country";
            }
            else if (_templateType == TemplateType.ImplicitItemTemplate)
            {
                expectedTextBlockOne = "Saint Lucia";
                expectedTextBlockTwo = "Implicit" + _dataSource.ToString() + "Country";
            }
            else if (_templateType == TemplateType.ItemTemplateSelector)
            {
                expectedTextBlockOne = "Saint Lucia";
                expectedTextBlockTwo = "Explicit" + _dataSource.ToString() + "CountrySelector";
            }

            if (countryTextBlockOne.Text != expectedTextBlockOne)
            {
                LogComment("Text of first TextBlock was expected to be: " + expectedTextBlockOne + ", was actually: " + countryTextBlockOne.Text);
                return TestResult.Fail;
            }

            if (countryTextBlockTwo.Text != expectedTextBlockTwo)
            {
                LogComment("Text of second TextBlock was expected to be: " + expectedTextBlockTwo + ", was actually: " + countryTextBlockTwo.Text);
                return TestResult.Fail;
            }

            Status("End Test");
            return TestResult.Pass;
        }
    }

    // Skeleton DataTemplateSelector whose SelectTemplate method just returns the stored DataTemplate.
    public class HDTSelector : DataTemplateSelector
    {
        private DataTemplate _hdt;

        public DataTemplate HDT
        {
            get { return _hdt; }
            set { _hdt = value; }
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return HDT;
        }
    }

    // Ended up not using this helper function, but it might be useful later on.
    //public static class DanielHelper
    //{
    //        // Version of ItemContainerGenerator.ContainerFromItem that performs a nested search
    //    public static DependencyObject ContainerFromItem(ItemsControl ic, object o)
    //    {
    //        DependencyObject item;
            
    //        // We first look to find the container for our object in the current ItemsControl
    //        // using the existing ItemContainerGenerator.ContainerFromItem method, which
    //        // does not perform a recursive search.
    //        item = ic.ItemContainerGenerator.ContainerFromItem(o);
    //        if (item != null)
    //            return item;

    //        // Since ICG.ContainerFromItem didn't work, we'll do a recursive search through the
    //        // ItemControl's items collection.
    //        for (int i = 0; i < ic.Items.Count; i++)
    //        {
    //            // We get the container corresponding to each item, and call out recursively if that
    //            // container was itself an ItemsControl
    //            ItemsControl nestedic = ic.ItemContainerGenerator.ContainerFromIndex(i) as ItemsControl;
    //            if (nestedic != null)
    //                item = ContainerFromItem(nestedic, o);
    //            if (item != null)
    //                return item;
    //        }

    //        return null;
    //    }
    //}
}
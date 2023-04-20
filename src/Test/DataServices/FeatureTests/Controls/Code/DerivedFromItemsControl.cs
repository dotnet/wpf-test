// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Uses a control derived from ItemsControl to test the "On...()" methods
    /// of ItemsControl such as OnItemsChanged()
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Controls", "DerivedFromItemsControl")]
    public class DerivedFromItemsControl : XamlTest
    {
        MyListBox _myRBL;
        DockPanel _myDockPanel;
        XmlDataProvider _xds;
        public static SolidColorBrush EvenColor = Brushes.AntiqueWhite;
        public static SolidColorBrush OddColor = Brushes.LightBlue;

        public DerivedFromItemsControl()
            : base(@"DerivedFromItemsControl.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestOnItemsChanged);
            RunSteps += new TestStep(TestOnStyleChanged);
            RunSteps += new TestStep(SetUpBinding);
            RunSteps += new TestStep(TestOnItemTemplateChanged);
            RunSteps += new TestStep(TestOnItemUIStyleChanged);
            RunSteps += new TestStep(TestOnItemTemplateSelectorChanged);
            RunSteps += new TestStep(TestOnItemUIStyleSelectorChanged);
        }

        #region Setup
        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);
            _myRBL = Util.FindElement(RootElement, "myRBL") as MyListBox;
            if (_myRBL == null)
            {
                LogComment("Unable to reference the ListBox.");
                return TestResult.Fail;
            }
            _myDockPanel = Util.FindElement(RootElement, "myDockPanel") as DockPanel;
            if (_myDockPanel == null)
            {
                LogComment("Unable to reference the DockPanel.");
                return TestResult.Fail;
            }
            _xds = RootElement.Resources["xds"] as XmlDataProvider;
            if (_xds == null)
            {
                LogComment("Unable to reference the XmlDataSource.");
                return TestResult.Fail;
            }
            LogComment("Setup successful");
            return TestResult.Pass;
        }
        #endregion Setup

        #region TestOnItemsChanged
        private TestResult TestOnItemsChanged()
        {
            Status("TestOnItemsChanged");
            WaitForPriority(DispatcherPriority.Render);

            _myRBL.ItemsChanged += new EventHandler(ItemsChangedHandler);

            // add an item to the ListBox
            _myRBL.Items.Add("item 1");

            // verify event handler is called
            TestResult resItems = WaitForSignal("ItemsChanged");
            if (resItems != TestResult.Pass)
            {
                LogComment("Fail - failed waiting the the ItemsChanged event");
                return TestResult.Fail;
            }

            // verify item was added
            int numItemsExpected = 1;
            int numItemsActual = _myRBL.Items.Count;
            if (numItemsActual != 1)
            {
                LogComment("Fail - number of items in ListBox is not as expected. Actual: " + numItemsActual + ", Expected: " + numItemsExpected);
                return TestResult.Fail;
            }

            string itemAdded = _myRBL.Items[0] as String;
            if (itemAdded != "item 1")
            {
                LogComment("Fail - item added is not as expected.");
                return TestResult.Fail;
            }

            _myRBL.ItemsChanged -= new EventHandler(ItemsChangedHandler);

            LogComment("TestOnItemsChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnItemsChanged

        #region TestOnStyleChanged
        private TestResult TestOnStyleChanged()
        {
            Status("TestOnStyleChanged");
            WaitForPriority(DispatcherPriority.Render);

            Style rblStyle2 = new Style(typeof(ListBox));
            rblStyle2.Setters.Add(new Setter(ListBox.BackgroundProperty, Brushes.LightBlue));
            _myDockPanel.Resources.Add("rblStyle2", rblStyle2);

            _myRBL.StyleChanged += new EventHandler(StyleChangedHandler);

            // set up Style
            Style stl = _myDockPanel.Resources["rblStyle1"] as Style;
            if (stl == null)
            {
                LogComment("Fail - Style rblStyle1 not found in DockPanel's resources");
                return TestResult.Fail;
            }
            _myRBL.Style = stl;

            // verify event handler is called
            TestResult resStyle1 = WaitForSignal("StyleChanged");
            if (resStyle1 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the StyleChanged event");
                return TestResult.Fail;
            }

            // verify style was added
            if (_myRBL.Background != Brushes.Silver)
            {
                LogComment("Fail - Style was not applied to dockpanel. Actual background: " + _myDockPanel.Background);
                return TestResult.Fail;
            }

            // change style
            _myRBL.Style = rblStyle2;

            // verify event handler is called
            TestResult resStyle2 = WaitForSignal("StyleChanged");
            if (resStyle2 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the StyleChanged event");
                return TestResult.Fail;
            }

            // verify style was added
            if (_myRBL.Background != Brushes.LightBlue)
            {
                LogComment("Fail - ListBox's style was not changed correctly");
                return TestResult.Fail;
            }

            _myRBL.StyleChanged -= new EventHandler(StyleChangedHandler);

            LogComment("TestOnStyleChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnStyleChanged

        #region SetUpBinding
        private TestResult SetUpBinding()
        {
            Status("SetUpBinding");
            WaitForPriority(DispatcherPriority.Render);

            // remove all items from radio button list
            _myRBL.Items.Clear();

            // set up data binding for radio button list
            Binding bind = new Binding();
            bind.Source = _xds;
            bind.XPath = "Book";
            bind.NotifyOnTargetUpdated = true;

            _myRBL.TargetUpdated += new EventHandler<DataTransferEventArgs>(DataTransferHandler);

            _myRBL.SetBinding(ListBox.ItemsSourceProperty, bind);
            BindingExpression binding = _myRBL.GetBindingExpression(ListBox.ItemsSourceProperty);
            if (binding == null)
            {
                LogComment("Fail - BindingExpression in ListBox's ItemSource is null");
                return TestResult.Fail;
            }

            // verify event handler is called
            TestResult resDataTransfer = WaitForSignal("DataTransfer");
            if (resDataTransfer != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the DataTransfer event");
                return TestResult.Fail;
            }

            _myRBL.TargetUpdated -= new EventHandler<DataTransferEventArgs>(DataTransferHandler);

            LogComment("SetUpBinding was successful");
            return TestResult.Pass;
        }
        #endregion SetUpBinding

        #region TestOnItemTemplateChanged
        private TestResult TestOnItemTemplateChanged()
        {
            Status("TestItemTemplateChanged");
            WaitForPriority(DispatcherPriority.Background);

            DataTemplate rblItemTemplate2 = new DataTemplate();
            FrameworkElementFactory fee = new FrameworkElementFactory(typeof(TextBlock));
            Binding bind1 = new Binding();
            bind1.XPath = "@ISBN";
            bind1.NotifyOnTargetUpdated = true;
            fee.SetBinding(TextBlock.TextProperty, bind1);
            fee.SetValue(TextBlock.NameProperty, "txt4");
            rblItemTemplate2.VisualTree = fee;
            _myDockPanel.Resources.Add("rblItemTemplate2", rblItemTemplate2);

            _myRBL.ItemTemplateChanged += new EventHandler(ItemTemplateChangedHandler);

            // set up ItemTemplate
            DataTemplate template = _myDockPanel.Resources["rblItemTemplate1"] as DataTemplate;
            if (template == null)
            {
                LogComment("Fail - DataTemplate rblItemTemplate1 not found in DockPanel's resources");
                return TestResult.Fail;
            }

            // change ItemTemplate
            _myRBL.ItemTemplate = template;

            // verify event handler is called
            TestResult resItemTemplate1 = WaitForSignal("ItemTemplateChanged");
            if (resItemTemplate1 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemTemplateChanged event");
                return TestResult.Fail;
            }

            // verify ItemTemplate was applied correctly
            TestResult resVerifyStyle1 = VerifyItemTemplate("txt3", "Microsoft C# Language Specification", "Inside C#");
            if (resVerifyStyle1 != TestResult.Pass)
            {
                LogComment("Fail - failure in VerifyItemTemplate");
                return TestResult.Fail;
            }

            // change ItemTemplate
            _myRBL.ItemTemplate = rblItemTemplate2;

            // verify event handler is called
            TestResult resItemTemplate2 = WaitForSignal("ItemTemplateChanged");
            if (resItemTemplate2 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemTemplateChanged event");
                return TestResult.Fail;
            }

            // verify ItemTemplate was applied correctly
            TestResult resVerifyStyle2 = VerifyItemTemplate("txt4", "0-7356-1448-2", "0-7356-1288-9");
            if (resVerifyStyle2 != TestResult.Pass)
            {
                LogComment("Fail - failure in VerifyItemTemplate");
                return TestResult.Fail;
            }

            _myRBL.ItemTemplateChanged -= new EventHandler(ItemTemplateChangedHandler);

            LogComment("TestItemTemplateChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnItemTemplateChanged

        #region TestOnItemUIStyleChanged
        private TestResult TestOnItemUIStyleChanged()
        {
            Status("TestOnItemUIStyleChanged");
            WaitForPriority(DispatcherPriority.Render);

            Style rblItemUIStyle2 = new Style(typeof(ListBoxItem));
            rblItemUIStyle2.Setters.Add(new Setter(ListBoxItem.HeightProperty, 50d));
            _myDockPanel.Resources.Add("rblItemUIStyle2", rblItemUIStyle2);

            _myRBL.ItemUIStyleChanged += new EventHandler(ItemUIStyleChangedHandler);

            // set up ItemContainerStyle
            Style stl = _myDockPanel.Resources["rblItemUIStyle1"] as Style;
            if (stl == null)
            {
                LogComment("Fail - Style rblItemTemplate2 not found in DockPanel's resources");
                return TestResult.Fail;
            }
            _myRBL.ItemContainerStyle = stl;

            // verify event handler is called
            TestResult resItemUIStyle1 = WaitForSignal("ItemUIStyleChanged");
            if (resItemUIStyle1 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemUIStyleChanged event");
                return TestResult.Fail;
            }

             

            TestResult resVerifyUIStyle1 = VerifyUIStyle(40, "TestOnItemUIStyleChanged - before change");
            if (resVerifyUIStyle1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            // change ItemContainerStyle
            _myRBL.ItemContainerStyle = rblItemUIStyle2;

            // verify event handler is called
            TestResult resItemUIStyle2 = WaitForSignal("ItemUIStyleChanged");
            if (resItemUIStyle2 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemUIStyleChanged event");
                return TestResult.Fail;
            }

             

            TestResult resVerifyUIStyle2 = VerifyUIStyle(50, "TestOnItemUIStyleChanged - after change");
            if (resVerifyUIStyle2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            _myRBL.ItemUIStyleChanged -= new EventHandler(ItemUIStyleChangedHandler);

            LogComment("TestOnItemUIStyleChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnItemUIStyleChanged

        #region TestOnItemTemplateSelectorChanged
        private TestResult TestOnItemTemplateSelectorChanged()
        {
            Status("TestOnItemTemplateSelectorChanged");
            WaitForPriority(DispatcherPriority.Render);

            _myRBL.ItemTemplate = null;

            _myRBL.ItemTemplateSelectorChanged += new EventHandler(ItemTemplateSelectorChangedHandler);

            // assign a StyleSelector
            MyDataTemplateSelector1 mss1 = new MyDataTemplateSelector1();
            _myRBL.ItemTemplateSelector = mss1;

            // verify event handler is called
            TestResult resItemTemplateSelector1 = WaitForSignal("ItemTemplateSelectorChanged");
            if (resItemTemplateSelector1 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemTemplateSelectorChanged event");
                return TestResult.Fail;
            }

            // verify StyleSelector applied the correct styles
            TestResult resStyleSelector1 = VerifyStyleSelector("txt1", FontWeights.Normal, FontWeights.Bold);
            if (resItemTemplateSelector1 != TestResult.Pass)
            {
                LogComment("Fail - failed in VerifyStyleSelector");
                return TestResult.Fail;
            }

            // change the StyleSelector
            MyDataTemplateSelector2 mss2 = new MyDataTemplateSelector2();
            _myRBL.ItemTemplateSelector = mss2;

            // verify event handler is called
            TestResult resItemTemplateSelector2 = WaitForSignal("ItemTemplateSelectorChanged");
            if (resItemTemplateSelector2 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemTemplateSelectorChanged event");
                return TestResult.Fail;
            }

            // verify StyleSelector applied the correct styles
            TestResult resStyleSelector2 = VerifyStyleSelector("txt2", FontWeights.Bold, FontWeights.Normal);
            if (resItemTemplateSelector2 != TestResult.Pass)
            {
                LogComment("Fail - failed in VerifyStyleSelector");
                return TestResult.Fail;
            }

            _myRBL.ItemTemplateSelectorChanged -= new EventHandler(ItemTemplateSelectorChangedHandler);

            LogComment("TestOnItemTemplateSelectorChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnItemTemplateSelectorChanged

        #region TestOnItemUIStyleSelectorChanged
        private TestResult TestOnItemUIStyleSelectorChanged()
        {
            Status("TestOnItemUIStyleSelectorChanged");
            WaitForPriority(DispatcherPriority.Render);

            _myRBL.ItemContainerStyle = null;

            _myRBL.ItemUIStyleSelectorChanged += new EventHandler(ItemUIStyleSelectorChangedHandler);

            // assign a UIStyleSelector
            MyUIStyleSelector1 mss1 = new MyUIStyleSelector1();
            _myRBL.ItemContainerStyleSelector = mss1;

            // verify event handler is called
            TestResult resItemUIStyleSelector1 = WaitForSignal("ItemUIStyleSelectorChanged");
            if (resItemUIStyleSelector1 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemUIStyleSelectorChanged event");
                return TestResult.Fail;
            }

             
            TestResult resVerifyUIStyleSelector1 =  VerifyUIStyleSelector(EvenColor, OddColor, "TestOnItemUIStyleSelectorChanged - before change");
            if (resVerifyUIStyleSelector1 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            // change the UIStyleSelector
            MyUIStyleSelector2 mss2 = new MyUIStyleSelector2();
            _myRBL.ItemContainerStyleSelector = mss2;

            // verify event handler is called
            TestResult resItemUIStyleSelector2 = WaitForSignal("ItemUIStyleSelectorChanged");
            if (resItemUIStyleSelector2 != TestResult.Pass)
            {
                LogComment("Fail - failed waiting for the ItemUIStyleSelectorChanged event");
                return TestResult.Fail;
            }

             
            TestResult resVerifyUIStyleSelector2 = VerifyUIStyleSelector(OddColor, EvenColor, "TestOnItemUIStyleSelectorChanged - after change");
            if (resVerifyUIStyleSelector2 != TestResult.Pass)
            {
                return TestResult.Fail;
            }

            _myRBL.ItemUIStyleSelectorChanged -= new EventHandler(ItemUIStyleSelectorChangedHandler);

            LogComment("TestOnItemUIStyleSelectorChanged was successful");
            return TestResult.Pass;
        }
        #endregion TestOnItemUIStyleSelectorChanged

        #region auxMethods
        private void DataTransferHandler(object sender, DataTransferEventArgs args)
        {
            Status("EventHandler - DataTransferHandler");
            Signal("DataTransfer", TestResult.Pass);
        }

        private void ItemsChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - ItemsChangedHandler");
            Signal("ItemsChanged", TestResult.Pass);
        }

        private void StyleChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - StyleChangedHandler");
            Signal("StyleChanged", TestResult.Pass);
        }

        private void ItemTemplateChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - ItemTemplateChangedHandler");
            Signal("ItemTemplateChanged", TestResult.Pass);
        }

        private void ItemUIStyleChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - ItemUIStyleChangedHandler");
            Signal("ItemUIStyleChanged", TestResult.Pass);
        }

        private void ItemTemplateSelectorChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - ItemTemplateSelectorChangedHandler");
            Signal("ItemTemplateSelectorChanged", TestResult.Pass);
        }

        private void ItemUIStyleSelectorChangedHandler(Object sender, EventArgs args)
        {
            Status("EventHandler - ItemUIStyleSelectorChangedHandler");
            Signal("ItemUIStyleSelectorChanged", TestResult.Pass);
        }

        // Verify the style selector applied the correct style
        // Verify first and second items
        private TestResult VerifyStyleSelector(string id, FontWeight firstItem, FontWeight secondItem)
        {
            Status("VerifyStyleSelector");
            WaitForPriority(DispatcherPriority.Background);
            FrameworkElement[] feList1 = Util.FindElements(_myRBL, id);
            int expectedNumberItems = 4;
            if (feList1.Length != expectedNumberItems)
            {
                LogComment("Fail - Number of elements in myRBL with Name " + id + " is not as expected. Actual: " + feList1.Length + ", Expected: " + expectedNumberItems);
                return TestResult.Fail;
            }
            TextBlock txt1 = feList1[0] as TextBlock;
            if (txt1 == null)
            {
                LogComment("Fail - feList1[0] could not be converted to a TextBlock element");
                return TestResult.Fail;
            }
            if (txt1.FontWeight != firstItem)
            {
                LogComment("Fail - ItemTemplateSelector failed selecting bold items - the first item does not have " + firstItem.ToString() + " weight");
                return TestResult.Fail;
            }
            TextBlock txt2 = feList1[1] as TextBlock;
            if (txt2 == null)
            {
                LogComment("Fail - feList1[1] could not be converted to a TextBlock element");
                return TestResult.Fail;
            }
            if (txt2.FontWeight != secondItem)
            {
                LogComment("Fail - ItemTemplateSelector failed selecting bold items - the second item does not have " + secondItem.ToString() + " weight");
                return TestResult.Fail;
            }
            LogComment("VerifyStyleSelector was successful");
            return TestResult.Pass;
        }

        // Verify ItemTemplate was applied correctly
        // Verify first and second items
        private TestResult VerifyItemTemplate(string id, string firstItem, string secondItem)
        {
            Status("VerifyItemTemplate");
            WaitForPriority(DispatcherPriority.SystemIdle);
            FrameworkElement[] feList1 = Util.FindElements(_myRBL, id);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && feList1.Length == 0)
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                feList1 = Util.FindElements(_myRBL, id);
            }
            int expectedNumberItems = 4;
            if (feList1.Length != expectedNumberItems)
            {
                LogComment("Fail - Number of elements in myRBL with Name " + id + " is not as expected. Actual: " + feList1.Length + ", Expected: " + expectedNumberItems);
                return TestResult.Fail;
            }
            TextBlock txt1 = feList1[0] as TextBlock;
            if (txt1 == null)
            {
                LogComment("Fail - feList1[0] could not be converted to a TextBlock element");
                return TestResult.Fail;
            }
            if (txt1.Text != firstItem)
            {
                LogComment("Fail - First item. Expected text: " + firstItem + ", Actual: " + txt1.Text);
                return TestResult.Fail;
            }
            TextBlock txt2 = feList1[1] as TextBlock;
            if (txt2 == null)
            {
                LogComment("Fail - feList1[1] could not be converted to a TextBlock element");
                return TestResult.Fail;
            }
            if (txt2.Text != secondItem)
            {
                LogComment("Fail - Second item. Expected text: " + secondItem + ", Actual: " + txt2.Text);
                return TestResult.Fail;
            }
            LogComment("VerifyItemTemplate was successful");
            return TestResult.Pass;
        }

        private TestResult VerifyUIStyleSelector(SolidColorBrush color1, SolidColorBrush color2, string step)
        {
            Status("VerifyUIStyleSelector");
            WaitForPriority(DispatcherPriority.Background);
             

            ListBoxItem rb1 = _myRBL.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            if (rb1 == null)
            {
                LogComment("Fail - failed retrieving the ListBoxItem in index 0 - " + step);
                return TestResult.Fail;
            }
            if (rb1.Background != color1)
            {
                LogComment("Fail - background of ListBoxItem in index 0 not as expected. Actual: " + rb1.Background + ", Expected: " + color1.ToString() + " - " + step);
                return TestResult.Fail;
            }
            ListBoxItem rb2 = _myRBL.ItemContainerGenerator.ContainerFromIndex(1) as ListBoxItem;
            if (rb2 == null)
            {
                LogComment("Fail - failed retrieving the ListBoxItem in index 1 - " + step);
                return TestResult.Fail;
            }
            if (rb2.Background != color2)
            {
                LogComment("Fail - background of ListBoxItem in index 1 not as expected. Actual: " + rb2.Background + ", Expected: " + color2.ToString() + " - " + step);
                return TestResult.Fail;
            }

            LogComment("VerifyUIStyleSelector was successful");
            return TestResult.Pass;
        }

        private TestResult VerifyUIStyle(int height, string step)
        {
            Status("VerifyUIStyle");
            WaitForPriority(DispatcherPriority.Background);
             


            double expectedHeightFirst = height;
            ListBoxItem rbFirst = _myRBL.ItemContainerGenerator.ContainerFromIndex(0) as ListBoxItem;
            if (rbFirst == null)
            {
                LogComment("Fail - ListBoxItem in index 0 is null - " + step);
                return TestResult.Fail;

            }
            if (rbFirst.Height != expectedHeightFirst)
            {
                LogComment("Fail - item UI style was not applied. Actual height: " + rbFirst.Height + ", Expected height: " + expectedHeightFirst + " - " + step);
                return TestResult.Fail;
            }
            LogComment("VerifyUIStyle was successful");
            return TestResult.Pass;
        }

        #endregion auxMethods
    }

    #region class-MyListBox
    public class MyListBox : ListBox
    {
        public event EventHandler ItemsChanged;
        override protected void OnItemsChanged(NotifyCollectionChangedEventArgs args)
        {
            if (ItemsChanged != null)
            {
                ItemsChanged(this, null);
            }
            base.OnItemsChanged(args);
        }

        public event EventHandler StyleChanged;
        protected override void OnStyleChanged(Style oldStyle, Style newStyle)
        {
            if (StyleChanged != null)
            {
                StyleChanged(this, null);
            }
            base.OnStyleChanged(oldStyle, newStyle);
        }

        public event EventHandler ItemTemplateChanged;
        protected override void OnItemTemplateChanged(DataTemplate oldTemplate, DataTemplate newTemplate)
        {
            if (ItemTemplateChanged != null)
            {
                ItemTemplateChanged(this, null);
            }
            base.OnItemTemplateChanged(oldTemplate, newTemplate);
        }

        public event EventHandler ItemUIStyleChanged;
        protected override void OnItemContainerStyleChanged(Style oldStyle, Style newStyle)
        {
            if (ItemUIStyleChanged != null)
            {
                ItemUIStyleChanged(this, null);
            }
            base.OnItemContainerStyleChanged(oldStyle, newStyle);
        }

        public event EventHandler ItemTemplateSelectorChanged;
        protected override void OnItemTemplateSelectorChanged(DataTemplateSelector oldTemplateSelector, DataTemplateSelector newTemplateSelector)
        {
            if (ItemTemplateSelectorChanged != null)
            {
                ItemTemplateSelectorChanged(this, null);
            }
            base.OnItemTemplateSelectorChanged(oldTemplateSelector, newTemplateSelector);
        }

        public event EventHandler ItemUIStyleSelectorChanged;
        protected override void OnItemContainerStyleSelectorChanged(StyleSelector oldStyleSelector, StyleSelector newStyleSelector)
        {
            if (ItemUIStyleSelectorChanged != null)
            {
                ItemUIStyleSelectorChanged(this, null);
            }
            base.OnItemContainerStyleSelectorChanged(oldStyleSelector, newStyleSelector);
        }
    }
    #endregion class-MyListBox

    #region StyleSelectors
    public class MyDataTemplateSelector1 : DataTemplateSelector
    {
        DataTemplate _bookTemplate1;
        DataTemplate _bookTemplate2;

        public MyDataTemplateSelector1()
        {
            _bookTemplate1 = new DataTemplate();
            _bookTemplate2 = new DataTemplate();

            FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(TextBlock));

            Binding b1 = new Binding();
            b1.XPath = "Title";
            template1.SetBinding(TextBlock.TextProperty, b1);
            template1.SetValue(TextBlock.NameProperty, "txt1");
            _bookTemplate1.VisualTree = template1;

            FrameworkElementFactory template2 = new FrameworkElementFactory(typeof(TextBlock));

            Binding b2 = new Binding();
            b2.XPath = "Title";
            template2.SetBinding(TextBlock.TextProperty, b2);
            template2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template2.SetValue(TextBlock.NameProperty, "txt1");
            _bookTemplate2.VisualTree = template2;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is XmlElement)
            {
                XmlElement book = item as XmlElement;
                int price = Int32.Parse((book.ChildNodes[1]).InnerText);

                if (price > 35)
                    return _bookTemplate2;
                else
                    return _bookTemplate1;
            }

            return base.SelectTemplate(item, container);
        }
    }

    public class MyDataTemplateSelector2 : DataTemplateSelector
    {
        DataTemplate _bookTemplate1;
        DataTemplate _bookTemplate2;

        public MyDataTemplateSelector2()
        {
            _bookTemplate1 = new DataTemplate();
            _bookTemplate2 = new DataTemplate();

            FrameworkElementFactory template1 = new FrameworkElementFactory(typeof(TextBlock));

            Binding b1 = new Binding();
            b1.XPath = "Title";
            template1.SetBinding(TextBlock.TextProperty, b1);
            template1.SetValue(TextBlock.NameProperty, "txt2");
            _bookTemplate1.VisualTree = template1;

            FrameworkElementFactory template2 = new FrameworkElementFactory(typeof(TextBlock));

            Binding b2 = new Binding();
            b2.XPath = "Title";
            template2.SetBinding(TextBlock.TextProperty, b2);
            template2.SetValue(TextBlock.FontWeightProperty, FontWeights.Bold);
            template2.SetValue(TextBlock.NameProperty, "txt2");
            _bookTemplate2.VisualTree = template2;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is XmlElement)
            {
                XmlElement book = item as XmlElement;
                int price = Int32.Parse((book.ChildNodes[1]).InnerText);

                if (price <= 35)
                    return _bookTemplate2;
                else
                    return _bookTemplate1;
            }

            return base.SelectTemplate(item, container);
        }
    }
    #endregion StyleSelectors

    #region UIStyleSelectors

    public class MyUIStyleSelector1 : StyleSelector
    {
        Style _color1Style;
        Style _color2Style;
        private int _i;

        public MyUIStyleSelector1()
        {
            _i = -1;
            _color1Style = new Style(typeof(ListBoxItem));
            _color1Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, DerivedFromItemsControl.EvenColor));
            _color1Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));

            _color2Style = new Style(typeof(ListBoxItem));
            _color2Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, DerivedFromItemsControl.OddColor));
            _color2Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));
        }

        /// <summary>
        /// Even rows are white, odd rows are light blue
        /// </summary>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            _i++;
            if (_i % 2 == 0)
            {
                return _color1Style;
            }
            else
            {
                return _color2Style;
            }
        }
    }

    public class MyUIStyleSelector2 : StyleSelector
    {
        Style _color1Style;
        Style _color2Style;
        private int _i;

        public MyUIStyleSelector2()
        {
            _i = -1;
            _color1Style = new Style(typeof(ListBoxItem));
            _color1Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, DerivedFromItemsControl.OddColor));
            _color1Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));

            _color2Style = new Style(typeof(ListBoxItem));
            _color2Style.Setters.Add(new Setter(ListBoxItem.BackgroundProperty, DerivedFromItemsControl.EvenColor));
            _color2Style.Setters.Add(new Setter(ListBoxItem.HeightProperty, 20d));
        }

        /// <summary>
        /// Even rows are light blue, odd rows are white
        /// </summary>
        public override Style SelectStyle(object item, DependencyObject container)
        {
            _i++;
            if (_i % 2 == 0)
            {
                return _color1Style;
            }
            else
            {
                return _color2Style;
            }
        }
    }
    #endregion UIStyleSelectors

}

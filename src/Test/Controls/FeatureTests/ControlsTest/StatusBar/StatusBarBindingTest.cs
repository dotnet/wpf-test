//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.ComponentModel;
using System.Xml;
using System.Windows;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Data;
using System.Globalization;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Actions;

namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// this class is used to test DataBinding, and the data source is a Dependency Property
    /// of TabItem (TabItem.HeaderProperty)
    /// To run this test case, one parameter should be assigned: xtc file name
    /// for example : CMLoader.exe StatusBarBindTabControl.xtc
    /// </summary>
    class StatusBarBindingTabControl : IIntegrationTest
    {
        /// <summary>
        /// test counts: default is 5
        /// </summary>
        private const int c_TestCount = 5;

        /// <summary>
        /// Do test
        /// </summary>
        /// <param name="testObject">test object</param>
        /// <param name="variation">null</param>
        /// <returns></returns>
        public TestResult Perform(object testObject, XmlElement variation)
        {
            GlobalLog.LogStatus("Starting StatusBarBindingTabControl test...");

            StackPanel stackpanel = testObject as StackPanel;
            TabControl tabctrl = stackpanel.Children[0] as TabControl;
            StatusBar sb = stackpanel.Children[1] as StatusBar;
            StatusBarItem item = sb.Items[1] as StatusBarItem;

            //create data binding
            Binding bind = new Binding();
            bind.Mode = BindingMode.OneWay;
            bind.Source = tabctrl;
            bind.Path = new PropertyPath(TabControl.SelectedItemProperty);
            bind.Converter = new StatusBarItemContentConvert();
            item.SetBinding(StatusBarItem.ContentProperty, bind);

            //do test
            for (int i = 0; i < c_TestCount; i++)
            {
                SelectTabItemAction action = new SelectTabItemAction();
                action.Do(tabctrl);

                TabItem tabItem = tabctrl.SelectedItem as TabItem;
                //verify
                if (tabItem.Header != item.Content)
                {
                    GlobalLog.LogEvidence("Bind error" + tabItem.Header.ToString() + "->" + item.Content.ToString());
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }
    }


    /// <summary>
    /// this class is used to get Header info from a TabItem.
    /// </summary>
    internal sealed class StatusBarItemContentConvert : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is TabItem)
            {
                TabItem item = value as TabItem;
                return item.Header;
            }

            return null;

        }

        //Not support ConvertBack
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            GlobalLog.LogEvidence("Not Support ConverBack in StatusBarItemContentConvert");
            return null;
        }
    }

    /// <summary>
    /// this class is used to test databind for ItemsSource property of StatusBar
    /// we think this is a key senario for statusbar to get items from list through DataBinding.
    /// To run this test case, one parameter should be assigned: xtc file name
    /// for example : CMLoader.exe StatusBarItemsSourceBindingTest.xtc
    /// </summary>
    class StatusBarItemsSourceBinding : IIntegrationTest
    {
        /// <summary>
        /// Do test
        /// </summary>
        /// <param name="testObject">test object</param>
        /// <param name="variation">method name(see .xtc file)</param>
        /// <returns></returns>
        public TestResult Perform(object testObject, XmlElement variation)
        {
            GlobalLog.LogStatus("Starting StatusBarItemsSourceBinding test...");

            StatusBar statBar = testObject as StatusBar;
            CustomItemsSource dataSource = new CustomItemsSource();

            //create databinding
            Binding bind = new Binding();
            bind.Source = dataSource;
            statBar.SetBinding(StatusBar.ItemsSourceProperty, bind);

            string strValue = ((XmlElement)variation["Parameters"]).GetAttribute("Method");
            GlobalLog.LogStatus("Process parameter : " + strValue);

            //first binding, the StatusInfo should be 3, because the DataSource
            //has 3 init value
            GlobalLog.LogStatus("StatusBar.Items.Count=" + statBar.Items.Count.ToString());
            GlobalLog.LogStatus("DataSource.Count=" + dataSource.Count.ToString());

            //change DataSource, so the binding will work
            if (strValue == "Add")
            {
                dataSource.AddData();
            }
            else if (strValue == "Delete")
            {
                dataSource.DelData();
            }
            else
            {
                GlobalLog.LogEvidence("Not Support this parameter : " + strValue);
                return TestResult.Unknown;
            }
            QueueHelper.WaitTillQueueItemsProcessed();

            //Now we can verify the result after binding
            //first the Count should be equal
            if (dataSource.Count != statBar.Items.Count)
            {
                GlobalLog.LogStatus("StatusBar.Items.Count=" + statBar.Items.Count.ToString());
                GlobalLog.LogStatus("DataSource.Count=" + dataSource.Count.ToString());

                GlobalLog.LogEvidence("Fail: itemsSource.Count != statBar.Items.Count");
                return TestResult.Fail;
            }

            //second the Content should be equal.
            for (int i = 0; i < statBar.Items.Count; i++)
            {
                string str1 = (string)(dataSource[i]);
                string str2 = (string)(statBar.Items[i]);
                GlobalLog.LogStatus("DataSource: " + str1);
                GlobalLog.LogStatus("StatusBar.Items: " + str2);

                if (str1 != str2)
                {
                    GlobalLog.LogEvidence("Fail: Results mismatch <" + str1 + "," + str2 + ">");
                    return TestResult.Fail;
                }
            }

            GlobalLog.LogEvidence("StatusBar.ItemsSource binding success");
            return TestResult.Pass;

        }
    }

    /// <summary>
    /// customized data source, it will provide a list data source
    /// </summary>
    internal sealed class CustomItemsSource : ObservableCollection<string>
    {
        public CustomItemsSource()
        {
            //init
            this.Add("StatusBar....");
            this.Add("Item-1");
            this.Add("Item-2");
        }

        /// <summary>
        /// Add new data for ItmesSource
        /// </summary>
        public void AddData()
        {
            this.Add("Item-" + this.Count.ToString());
        }

        /// <summary>
        /// delete ths last data from current data list.
        /// </summary>
        public void DelData()
        {
            this.RemoveAt(this.Count - 1);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;

namespace PropertyOrderWithTemplatesAndStyles
{
    public class TestComboBox : ComboBox
    {
        static List<TestComboBoxData> s_testComboBoxDataList = new List<TestComboBoxData>();
        TestComboBoxData _tcbData;

        public TestComboBox()
            : base()
        {
            _tcbData = new TestComboBoxData();
            _tcbData.PropertySetOrder = String.Empty;
            _tcbData.TestID = String.Empty;
            TestComboBox.TestComboBoxDataList.Add(_tcbData);
        }

        public static List<TestComboBoxData> TestComboBoxDataList
        {
            get { return s_testComboBoxDataList; }
        }

        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            switch (e.Property.Name)
            {
                case "ItemsSource":
                    _tcbData.PropertySetOrder += "ItemsSource, ";
                    break;
                case "SelectedIndex":
                    _tcbData.PropertySetOrder += "SelectedIndex, ";
                    break;
                case "IsSynchronizedWithCurrentItem":
                    _tcbData.PropertySetOrder += "IsSynchronizedWithCurrentItem, ";
                    break;
                case "TestID":
                    _tcbData.TestID = e.NewValue as string;
                    break;
            }
        }

        public string TestID
        {
            get { return (string)GetValue(TestIDProperty); }
            set { SetValue(TestIDProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TestID.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty TestIDProperty =
            DependencyProperty.Register("TestID", typeof(string), typeof(TestComboBox));       

    }

    public class TestComboBoxData
    {
        public string TestID;
        public string PropertySetOrder;
    }
}

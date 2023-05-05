// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;

namespace PropertyOrderWithTemplatesAndStyles
{
    /// <summary>
    /// Markup compiler test : Template properties are set in a different order
    /// Uses a custom combo box that records the order in which properties are set
    /// There are 14 different varitions on how the properties are set(Inline vs Resource, Style vs. Template, etc.)
    /// </summary>
    public partial class Window1 : Window
    {
        static string[] s_residents = { "Not Resident", "Resident", "Checked-Out"};
        static string[] s_tariffs = { "25.00   Single", "45.00   Double", "75.00   Illuminations" };

        List<TestComboBoxData> _verificationData;

        public Window1()
        {
            InitializeComponent();

            CollectionViewSource cvs;
            cvs = (CollectionViewSource)Resources["cvsResidents"];
            cvs.Source = s_residents;
            cvs = (CollectionViewSource)Resources["cvsTariffs"];
            cvs.Source = s_tariffs;

            InitializeVerificationData();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            if (TestComboBox.TestComboBoxDataList.Count != _verificationData.Count)
            {
                TestLog.Current.LogEvidence("Property set counts did not match.  Expected: {0}, Actual: {1}", _verificationData.Count, TestComboBox.TestComboBoxDataList.Count);
                TestLog.Current.Result = TestResult.Fail;
            }
            else
            {
                for (int i = 0; i < _verificationData.Count; i++)
                {
                    if (!CompareTCBDItems(_verificationData[i], TestComboBox.TestComboBoxDataList[i]))
                    {
                        TestLog.Current.LogEvidence("Item index {0} failed.", i.ToString());
                    }
                }
            }
            TestLog.Current.Result = TestResult.Pass;
            Application.Current.Shutdown(0);
        }

        private void InitializeVerificationData()
        {
            _verificationData = new List<TestComboBoxData>();

            _verificationData.Add(new TestComboBoxData() { TestID = "Resource1", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Resource Template
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline1", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Inline Template
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource1", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Nested Resource Templates
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource1", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Inline Nested Resource Template
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline2", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Nested Inline Templates
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource2", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Resource With Nested Inline Template
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline3", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Inline Template within an Inline Style
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource1", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Resource Template within an Inline Style pointing to resource template, prop set via template
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource3", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Inline Template within a Resource Style
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline4", PropertySetOrder = "ItemsSource, SelectedIndex, IsSynchronizedWithCurrentItem," }); // Inline Template with  inline Style, prop set via style
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline5", PropertySetOrder = "ItemsSource, SelectedIndex, IsSynchronizedWithCurrentItem," }); // Inline Template with  resource Style, prop set via style
            _verificationData.Add(new TestComboBoxData() { TestID = "Resource3", PropertySetOrder = "IsSynchronizedWithCurrentItem, SelectedIndex, ItemsSource," }); // Nested resource Styles, prop set via second style
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline6", PropertySetOrder = "ItemsSource, SelectedIndex, IsSynchronizedWithCurrentItem," }); // Inline Style with nested resource style
            _verificationData.Add(new TestComboBoxData() { TestID = "Inline7", PropertySetOrder = "ItemsSource, SelectedIndex, IsSynchronizedWithCurrentItem," }); // Nested inline styles
        }        

        private bool CompareTCBDItems(TestComboBoxData testComboBoxData, TestComboBoxData testComboBoxData_2)
        {
            bool passed = true;
            passed &= testComboBoxData.TestID.Trim() == testComboBoxData_2.TestID.Trim();
            passed &= testComboBoxData.PropertySetOrder.Trim() == testComboBoxData_2.PropertySetOrder.Trim();

            if (!passed)
            {
                TestLog.Current.LogEvidence("TestComboBoxData item did not match");
                TestLog.Current.LogEvidence("\tExpected: {0}: {1}", testComboBoxData.TestID, testComboBoxData.PropertySetOrder);
                TestLog.Current.LogEvidence("\t  Actual: {0}: {1}", testComboBoxData_2.TestID, testComboBoxData_2.PropertySetOrder);
                TestLog.Current.Result = TestResult.Fail;
            }
            return passed;

        }
    }

    public class Booking : INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Members

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        short _resident;
        public short resident
        {
            get { return _resident; }
            set { _resident = value; OnPropertyChanged("resident"); }
        }

        Decimal _st_rate;
        public Decimal st_rate
        {
            get { return _st_rate; }
            set { _st_rate = value; OnPropertyChanged("st_rate"); }
        }
    }

    public class Bookings : ObservableCollection<Booking>
    {
        public Bookings()
        {
            Add(new Booking { resident = 1, st_rate = 45.00M });
        }
    }

    public class MyConverter : IValueConverter
    {
        #region IValueConverter Members

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
           // if (value is int)
                return value;
           // else
           //     return Binding.DoNothing;
        }

        #endregion
    }
}

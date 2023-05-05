// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using Microsoft.Test.DataServices.RegressionTest4;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage - Binding that worked in 3.51 does not work in 4
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest4BindingValidation", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionTest4BindingValidation : XamlTest
    {
        static string[] s_residents = { "Not Resident", "Resident", "Checked-Out" };
        static string[] s_restaurants = { "Not Set", "Connaught", "Washington" };
        static string[] s_boards = { "Room Only", "B and B", "Half Board", "Full Board" };
        static string[] s_tariffs = { "25.00     Single", "45.00     Double", "75.00     Illuminations" };

        #region Private Data

        private ListBox _myListBox;

        #endregion

        #region Constructors

        public RegressionTest4BindingValidation() : base(@"RegressionTest4BindingValidation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            StackPanel myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            
            CollectionViewSource cvs;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsResidents"];
            cvs.Source = s_residents;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsRestaurants"];
            cvs.Source = s_restaurants;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsBoards"];
            cvs.Source = s_boards;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsTariffs"];
            cvs.Source = s_tariffs;

            _myListBox = (ListBox)RootElement.FindName("myListBox");

            if (_myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            LogComment("Beginning validation: Ensuring that default validation does not show error in initial state.");

            // Getting the currently selected ListBoxItem
            // Note that the ListBox must have
            // IsSynchronizedWithCurrentItem set to True for this to work
            ListBoxItem myListBoxItem = (ListBoxItem)(_myListBox.ItemContainerGenerator.ContainerFromItem(_myListBox.Items.CurrentItem));

            // Getting the ContentPresenter of myListBoxItem
            ContentPresenter myContentPresenter = Util.FindVisualChild<ContentPresenter>(myListBoxItem);

            // Finding textBlock from the DataTemplate that is set on that ContentPresenter
            DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
            ComboBox myComboBox = (ComboBox)myDataTemplate.FindName("testComboBox", myContentPresenter);            

            ReadOnlyObservableCollection<ValidationError> errors = Validation.GetErrors(myComboBox);

            if (errors.Count == 0)
            {
                LogComment("Success, initial validation did not report error; bug has not regressed.");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Error, initial validation reported error (regression)");
                return TestResult.Fail;
            }
        }
        #endregion
    }

    /// <summary>
    /// <description>
    ///  Regression coverage for issue Selector's SynchronizeWithCurrentItem behavior depends on property order
    /// </description>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest12SelectionIndex", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionTest12SelectionIndex : XamlTest
    {
        static string[] s_residents = { "Not Resident", "Resident", "Checked-Out" };
        static string[] s_restaurants = { "Not Set", "Connaught", "Washington" };
        static string[] s_boards = { "Room Only", "B and B", "Half Board", "Full Board" };
        static string[] s_tariffs = { "25.00     Single", "45.00     Double", "75.00     Illuminations" };

        #region Private Data

        private ListBox _myListBox;

        #endregion

        #region Constructors
        // Use the same Xaml for a different test... 
        public RegressionTest12SelectionIndex() : base(@"RegressionTest4BindingValidation.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            StackPanel myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");

            CollectionViewSource cvs;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsResidents"];
            cvs.Source = s_residents;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsRestaurants"];
            cvs.Source = s_restaurants;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsBoards"];
            cvs.Source = s_boards;
            cvs = (CollectionViewSource)myStackPanel.Resources["cvsTariffs"];
            cvs.Source = s_tariffs;

            _myListBox = (ListBox)RootElement.FindName("myListBox");

            if (_myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            LogComment("Beginning validation: Ensuring that templated combo box SelectedIndex is correct on initial render");

            int selectedIndexSum = 0;
            foreach (Booking book in _myListBox.Items)
            {
                ListBoxItem myListBoxItem = (ListBoxItem)(_myListBox.ItemContainerGenerator.ContainerFromItem(book));

                // Getting the ContentPresenter of myListBoxItem
                ContentPresenter myContentPresenter = Util.FindVisualChild<ContentPresenter>(myListBoxItem);

                // Finding textBlock from the DataTemplate that is set on that ContentPresenter
                DataTemplate myDataTemplate = myContentPresenter.ContentTemplate;
                ComboBox myComboBox = (ComboBox)myDataTemplate.FindName("testComboBox", myContentPresenter);

                selectedIndexSum += myComboBox.SelectedIndex;
            }

            if (selectedIndexSum  > 0)
            {
                LogComment("Success, selected index of even ComboBoxes was 1 (would be 0 if regressed)");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Failure, selected index of all ComboBoxes was 0 (regression)");
                return TestResult.Fail;
            }
        }
        #endregion
    }    
}

namespace Microsoft.Test.DataServices.RegressionTest4
{
    #region Helper Classes


    public class Booking : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(name));
        }

        short _resident;
        public short resident
        {
            get { return _resident; }
            set { _resident = value; OnPropertyChanged("resident"); }
        }

        short _restaurant;
        public short restaurant
        {
            get { return _restaurant; }
            set { _restaurant = value; OnPropertyChanged("restaurant"); }
        }

        short _board;
        public short board
        {
            get { return _board; }
            set { _board = value; OnPropertyChanged("board"); }
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
            Add(new Booking());
            Add(new Booking { resident = 1, restaurant = 1, board = 1, st_rate = 45.00M });
        }
    }

    #endregion
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where No feedback when user-supplied converter fails to convert
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "FailedConverterFeedback")]
    public class FailedConverterFeedback : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListBox _myListBox;
        private TextBox _airportTextBox;
        private TextBox _myTextBox;
        
        #endregion

        #region Constructors

        public FailedConverterFeedback()
            : base(@"FailedConverterFeedback.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListBox = (ListBox)RootElement.FindName("myListBox");
            _airportTextBox = (TextBox)RootElement.FindName("airportTextBox");
            _myTextBox = (TextBox)RootElement.FindName("myTextBox");

            if (_myStackPanel == null || _myListBox == null || _airportTextBox == null || _myTextBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Select a leg.
            _myListBox.SelectedItem = _myListBox.Items[1]; 
          
            // Focus on airport field and type invalid entry.
            _airportTextBox.Focus();
            _airportTextBox.Text = "Some invalid Airport.";

            // Focus away from the Airport field.
            _myTextBox.Focus();
            WaitForPriority(DispatcherPriority.Render);         

            // Grab the bindingExpression on the TextBox
            BindingExpression bindingExpression = _airportTextBox.GetBindingExpression(TextBox.TextProperty);

            // Verify 
            if (bindingExpression.ValidationError == null)
            {
                LogComment("Failed to create a validation error");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class AirportConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Airport airport = (Airport)value;
            return airport.City + "/" + airport.Code;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Airport airport = Airport.Find((string)value);

            if (airport != null)
                return airport;
            else
                return DependencyProperty.UnsetValue;   // failure to convert
        }
    }

    public class Airport
    {
        static Airport()
        {
            s_list = new List<Airport>();
            s_list.Add(new Airport("Boston", "BOS"));
            s_list.Add(new Airport("Los Angeles", "LAX"));
            s_list.Add(new Airport("Seattle", "SEA"));
        }

        public Airport(string city, string code)
        {
            _cityName = city;
            _cityCode = code;
        }

        public string City { get { return _cityName; } }
        public string Code { get { return _cityCode; } }

        static public Airport Find(string s)
        {
            foreach (Airport airport in s_list)
            {
                if (airport.City.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0 ||
                    airport.Code.IndexOf(s, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return airport;
                }
            }

            return null;
        }

        string _cityName,_cityCode;
        static List<Airport> s_list;
    }

    public class Leg : INotifyPropertyChanged
    {
        public Leg(Airport depart, DateTime departTime, Airport arrive, DateTime arriveTime)
        {
            _departCity = depart;
            _departCityTime = departTime;
            _arriveCity = arrive;
            _arriveCityTime = arriveTime;
        }

        public Airport Depart
        {
            get { return _departCity; }
            set { _departCity = value; OnPropertyChanged("Depart"); }
        }
        Airport _departCity;

        public Airport Arrive
        {
            get { return _arriveCity; }
            set { _arriveCity = value; OnPropertyChanged("Arrive"); }
        }
        Airport _arriveCity;

        public DateTime DepartTime
        {
            get { return _departCityTime; }
            set { _departCityTime = value; OnPropertyChanged("DepartTime"); }
        }
        DateTime _departCityTime;

        public DateTime ArriveTime
        {
            get { return _arriveCityTime; }
            set { _arriveCityTime = value; OnPropertyChanged("ArriveTime"); }
        }
        DateTime _arriveCityTime;

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged
    }

    public class Itinerary : ObservableCollection<Leg>
    {
        public Itinerary()
        {
            Airport bos = Airport.Find("Boston");
            Airport lax = Airport.Find("LAX");
            Airport seatac = Airport.Find("Seattle");

            Add(new Leg(bos, DateTime.Parse("4/11/2007 10:20:00"),
                        lax, DateTime.Parse("4/11/2007 12:58:00")));
            Add(new Leg(lax, DateTime.Parse("4/11/2007 14:35:00"),
                        seatac, DateTime.Parse("4/11/2007 16:18:00")));
        }
    }

    #endregion
}

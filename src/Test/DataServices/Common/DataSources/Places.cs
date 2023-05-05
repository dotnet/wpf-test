// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows.Data;
using System.Collections.Generic;

namespace Microsoft.Test.DataServices
{
    public class Places : ObservableCollection<Place>
    {
        public Places()
        {
            Add(new Place("Seattle", "WA"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("Bellevue", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Place("Portland", "OR"));
            Add(new Place("San Francisco", "CA"));
            Add(new Place("Los Angeles", "CA"));
            Add(new Place("San Diego", "CA"));
            Add(new Place("San Jose", "CA"));
            Add(new Place("Santa Ana", "CA"));
            Add(new Place("Bellingham", "WA"));
        }
    }

    public class PlacesBindingListT : FullBindingList<Place>
    {
        public PlacesBindingListT()
        {
            Add(new Place("Seattle", "WA"));
            Add(new Place("Redmond", "WA"));
            Add(new Place("Bellevue", "WA"));
            Add(new Place("Kirkland", "WA"));
            Add(new Place("Portland", "OR"));
            Add(new Place("San Francisco", "CA"));
            Add(new Place("Los Angeles", "CA"));
            Add(new Place("San Diego", "CA"));
            Add(new Place("San Jose", "CA"));
            Add(new Place("Santa Ana", "CA"));
            Add(new Place("Bellingham", "WA"));
        }
    }

    public class Place : INotifyPropertyChanged, IDataErrorInfo, ISupportInitialize, IEditableObject
    {
        private string _name;

        private string _state;

        [ComparableByValue()]
        public string Name
        {
            get { return _name; }
            set 
            { 
                _name = value;
                RaisePropertyChangedEvent("Name");

                if (_name != null)
                {
                    foreach (char c in _name)
                    {
                        if (!(char.IsLetter(c)) && c != ' ')
                        {
                            throw new ApplicationException("Name must be alpha only.");
                        }
                    }
                }
            }
        }

        [ComparableByValue()]
        public string State
        {
            get { return _state; }
            set 
            { 
                _state = value;
                RaisePropertyChangedEvent("State");

                foreach (char c in _state)
                {
                    if (!(char.IsLetter(c)) && c != ' ')
                    {
                        throw new ApplicationException("State must be alpha only.");
                    }
                }
            }
        }

        public Place()
        {
            this._name = "";
            this._state = "";
        }

        public Place(string name, string state)
        {
            this._name = name;
            this._state = state;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void RaisePropertyChangedEvent(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public string Error
        {
            get
            {
                return this[string.Empty];
            }
        }

        public string this[string propertyName]
        {
            get
            {
                string result = string.Empty;
                if (propertyName == "Name" && string.IsNullOrEmpty(this.Name))
                {
                    result = "Name cannot be blank!";
                }
                else if (propertyName == "State" && string.IsNullOrEmpty(this.State))
                {
                    result = "State cannot be blank!";
                }
                return result;
            }
        }

        public enum InterfaceAPI { BeginInit, EndInit, BeginEdit, CancelEdit, EndEdit, None };

        private List<InterfaceAPI> _calledInterfaceAPI = new List<InterfaceAPI>();

        public List<InterfaceAPI> CalledInterfaceAPI
        {
            get { return _calledInterfaceAPI; }
        }

        #region ISupportInitialize Members

        public void BeginInit()
        {
            CalledInterfaceAPI.Add(InterfaceAPI.BeginInit);
        }

        public void EndInit()
        {
            CalledInterfaceAPI.Add(InterfaceAPI.EndInit);
        }

        #endregion

        #region IEditableObject Members

        public void BeginEdit()
        {
            CalledInterfaceAPI.Add(InterfaceAPI.BeginEdit);
        }

        public void CancelEdit()
        {
            CalledInterfaceAPI.Add(InterfaceAPI.CancelEdit);
        }

        public void EndEdit()
        {
            CalledInterfaceAPI.Add(InterfaceAPI.EndEdit);
        }

        #endregion
    }

    public class PlaceConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return (values[0] + ", " + values[1]);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            return ((string)value).Split(new string[] { ", " }, StringSplitOptions.None);
        }
    }

    public class PlacesDataTable : DataTable
    {
        public PlacesDataTable()
        {
            Places places = new Places();
            this.TableName = "Places";
            this.Columns.Add("Name");
            this.Columns.Add("State");
            this.Rows.Add(places[0].Name, places[0].State);
            this.Rows.Add(places[1].Name, places[1].State);
            this.Rows.Add(places[2].Name, places[2].State);
            this.Rows.Add(places[3].Name, places[3].State);
            this.Rows.Add(places[4].Name, places[4].State);
            this.Rows.Add(places[5].Name, places[5].State);
            this.Rows.Add(places[6].Name, places[6].State);
            this.Rows.Add(places[7].Name, places[7].State);
            this.Rows.Add(places[8].Name, places[8].State);
            this.Rows.Add(places[9].Name, places[9].State);
            this.Rows.Add(places[10].Name, places[10].State);
        }
    }
}

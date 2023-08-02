using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Avalon.Test.ComponentModel
{

    public class Region : INotifyPropertyChanged
    {
        private string regionName;
        private ObservableCollection<Country> countries;

        #region Properties
        public string RegionName
        {
            get { return regionName; }
            set
            {
                regionName = value;
                OnPropertyChanged("RegionName");
            }
        }

        public ObservableCollection<Country> Countries
        {
            get { return countries; }
            set
            {
                countries = value;
                OnPropertyChanged("Countries");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Region(string regionName, ObservableCollection<Country> countries)
        {
            this.regionName = regionName;
            this.countries = countries;
        }

        public Region()
        {
        }
    }
}
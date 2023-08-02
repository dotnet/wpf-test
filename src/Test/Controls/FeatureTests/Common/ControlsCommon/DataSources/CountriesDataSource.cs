using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Avalon.Test.ComponentModel
{
    // For a hierarchical object data source: EarthDataSource + Hemisphere + Region + Country
    // For a flat object data source with potential to be turned into hierarhcical through grouping: CountriesDataSource + CountryWithExtraInfo
    // For ADO.NET hierarhical data: EarthDataSet

    public enum GovernmentType
    {
        Republic,
        Monarchy,
        Democracy
    }   

    public class Country : INotifyPropertyChanged
    {
        private string countryName;
        private string capital;
        private DateTime independenceDay;
        private int population;
        private GovernmentType government;

        #region Properties
        public string CountryName
        {
            get { return countryName; }
            set
            {
                countryName = value;
                OnPropertyChanged("CountryName");
            }
        }

        public string Capital
        {
            get { return capital; }
            set
            {
                capital = value;
                OnPropertyChanged("Capital");
            }
        }

        public DateTime IndependenceDay
        {
            get { return independenceDay; }
            set
            {
                independenceDay = value;
                OnPropertyChanged("IndependenceDay");
            }
        }

        public int Population
        {
            get { return population; }
            set
            {
                population = value;
                OnPropertyChanged("Population");
            }
        }

        public GovernmentType Government
        {
            get { return government; }
            set
            {
                government = value;
                OnPropertyChanged("Government");
            }
        }
        #endregion

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        public Country(string countryName, string capital, DateTime independenceDay, int population, GovernmentType government)
        {
            this.countryName = countryName;
            this.capital = capital;
            this.independenceDay = independenceDay;
            this.population = population;
            this.government = government;
        }

        public Country()
        {
        }
    }
    
    #region Object CountriesDataSource
    public class CountriesDataSource
    {
        private ObservableCollection<CountryWithExtraInfo> countries;

        public ObservableCollection<CountryWithExtraInfo> Countries
        {
            get { return countries; }
            set { countries = value; }
        }

        public CountriesDataSource()
        {
            this.countries = new ObservableCollection<CountryWithExtraInfo>();

            this.countries.Add(new CountryWithExtraInfo("Yemen", "Sanaa", new DateTime(1990, 5, 22), 20024867, GovernmentType.Republic, "Middle East", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Qatar", "Doha", new DateTime(1971, 9, 3), 840290, GovernmentType.Monarchy, "Middle East", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Saint Lucia", "Castries", new DateTime(1979, 2, 22), 164213, GovernmentType.Democracy, "The Caribbean", "Western Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("San Marino", "San Marino", new DateTime(301, 9, 3), 28503, GovernmentType.Republic, "Europe", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Turkmenistan", "Ashgabat", new DateTime(1991, 10, 27), 4863169, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Moldova", "Chisinau", new DateTime(1991, 8, 27), 4446455, GovernmentType.Republic, "Europe", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Nauru", null, new DateTime(1968, 1, 31), 12809, GovernmentType.Republic, "Oceania", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Sierra Leone", "Freetown", new DateTime(1961, 4, 27), 5883889, GovernmentType.Democracy, "Africa", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Kyrgyzstan", "Bishkek", new DateTime(1991, 8, 31), 5081429, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Malawi", "Lilongwe", new DateTime(1964, 7, 6), 11906855, GovernmentType.Democracy, "Africa", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Djibouti", "Djibouti", new DateTime(1977, 6, 27), 466900, GovernmentType.Republic, "Africa", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("East Timor", "Dili", new DateTime(1975, 11, 28), 1019252, GovernmentType.Republic, "Southeast Asia", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Armenia", "Yerevan", new DateTime(1991, 9, 21), 2991360, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("El Salvador", "San Salvador", new DateTime(1821, 9, 15), 6587541, GovernmentType.Republic, "Central America", "Western Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Guyana", "Georgetown", new DateTime(1966, 5, 26), 705803, GovernmentType.Republic, "South America", "Western Hemisphere"));
            this.countries.Add(new CountryWithExtraInfo("Mexico", "Mexico", new DateTime(1810, 9, 16), 104959594, GovernmentType.Republic, "North America", "Western Hemisphere"));
        }
    }

    public class CountryWithExtraInfo : Country
    {
        private string region;
        private string hemisphere;

        #region Properties
        public string Region
        {
            get { return region; }
            set
            {
                region = value;
                OnPropertyChanged("Region");
            }
        }

        public string Hemisphere
        {
            get { return hemisphere; }
            set { hemisphere = value; }
        }
        #endregion

        public CountryWithExtraInfo(string countryName, string capital, DateTime independenceDay, int population, GovernmentType government, string region, string hemisphere)
            : base(countryName, capital, independenceDay, population, government)
        {
            this.region = region;
            this.hemisphere = hemisphere;
        }
    }
    #endregion    
}
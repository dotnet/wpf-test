// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Microsoft.Test.DataServices
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

    public enum BeenThere
    {
        BeenThereIn2000s,
        BeenThereIn1990s,
        BeenThereIn1980s,
        NeverBeenThere
    }

    #region Object EarthDataSource
    public class EarthDataSource
    {
        private ObservableCollection<Hemisphere> _hemispheres;

        public ObservableCollection<Hemisphere> Hemispheres
        {
            get { return _hemispheres; }
            set { _hemispheres = value; }
        }

        public EarthDataSource()
        {
            // Countries 
            Country yemen = new Country("Yemen", "Sanaa", new DateTime(1990, 5, 22), 20024867, GovernmentType.Republic);
            Country qatar = new Country("Qatar", "Doha", new DateTime(1971, 9, 3), 840290, GovernmentType.Monarchy);
            Country saintLucia = new Country("Saint Lucia", "Castries", new DateTime(1979, 2, 22), 164213, GovernmentType.Democracy);
            Country sanMarino = new Country("San Marino", "San Marino", new DateTime(301, 9, 3), 28503, GovernmentType.Republic);
            Country turkmenistan = new Country("Turkmenistan", "Ashgabat", new DateTime(1991, 10, 27), 4863169, GovernmentType.Republic);
            Country moldova = new Country("Moldova", "Chisinau", new DateTime(1991, 8, 27), 4446455, GovernmentType.Republic);
            Country nauru = new Country("Nauru", null, new DateTime(1968, 1, 31), 12809, GovernmentType.Republic);
            Country sierraLeone = new Country("Sierra Leone", "Freetown", new DateTime(1961, 4, 27), 5883889, GovernmentType.Democracy);
            Country kyrgyzstan = new Country("Kyrgyzstan", "Bishkek", new DateTime(1991, 8, 31), 5081429, GovernmentType.Republic);
            Country malawi = new Country("Malawi", "Lilongwe", new DateTime(1964, 7, 6), 11906855, GovernmentType.Democracy);
            Country djibouti = new Country("Djibouti", "Djibouti", new DateTime(1977, 6, 27), 466900, GovernmentType.Republic);
            Country eastTimor = new Country("East Timor", "Dili", new DateTime(1975, 11, 28), 1019252, GovernmentType.Republic);
            Country armenia = new Country("Armenia", "Yerevan", new DateTime(1991, 9, 21), 2991360, GovernmentType.Republic);
            Country elSalvador = new Country("El Salvador", "San Salvador", new DateTime(1821, 9, 15), 6587541, GovernmentType.Republic);
            Country guyana = new Country("Guyana", "Georgetown", new DateTime(1966, 5, 26), 705803, GovernmentType.Republic);
            Country mexico = new Country("Mexico", "Mexico", new DateTime(1810, 9, 16), 104959594, GovernmentType.Republic);

            // Regions
            ObservableCollection<Country> countriesInMiddleEast = new ObservableCollection<Country>();
            countriesInMiddleEast.Add(yemen);
            countriesInMiddleEast.Add(qatar);
            Region middleEast = new Region("Middle East", countriesInMiddleEast);

            ObservableCollection<Country> countriesInTheCaribbean = new ObservableCollection<Country>();
            countriesInTheCaribbean.Add(saintLucia);
            Region theCaribbean = new Region("The Caribbean", countriesInTheCaribbean);

            ObservableCollection<Country> countriesInEurope = new ObservableCollection<Country>();
            countriesInEurope.Add(sanMarino);
            countriesInEurope.Add(moldova);
            Region europe = new Region("Europe", countriesInEurope);

            ObservableCollection<Country> countriesInAsia = new ObservableCollection<Country>();
            countriesInAsia.Add(turkmenistan);
            countriesInAsia.Add(kyrgyzstan);
            countriesInAsia.Add(armenia);
            Region asia = new Region("Asia", countriesInAsia);

            ObservableCollection<Country> countriesInOceania = new ObservableCollection<Country>();
            countriesInOceania.Add(nauru);
            Region oceania = new Region("Oceania", countriesInOceania);

            ObservableCollection<Country> countriesInAfrica = new ObservableCollection<Country>();
            countriesInAfrica.Add(sierraLeone);
            countriesInAfrica.Add(malawi);
            countriesInAfrica.Add(djibouti);
            Region africa = new Region("Africa", countriesInAfrica);

            ObservableCollection<Country> countriesInSoutheastAsia = new ObservableCollection<Country>();
            countriesInSoutheastAsia.Add(eastTimor);
            Region southeastAsia = new Region("Southeast Asia", countriesInSoutheastAsia);

            ObservableCollection<Country> countriesInCentralAmerica = new ObservableCollection<Country>();
            countriesInCentralAmerica.Add(elSalvador);
            Region centralAmerica = new Region("Central America", countriesInCentralAmerica);

            ObservableCollection<Country> countriesInSouthAmerica = new ObservableCollection<Country>();
            countriesInSouthAmerica.Add(guyana);
            Region southAmerica = new Region("South America", countriesInSouthAmerica);

            ObservableCollection<Country> countriesInNorthAmerica = new ObservableCollection<Country>();
            countriesInNorthAmerica.Add(mexico);
            Region northAmerica = new Region("North America", countriesInNorthAmerica);

            // Hemispheres
            ObservableCollection<Region> westernRegions = new ObservableCollection<Region>();
            westernRegions.Add(theCaribbean);
            westernRegions.Add(centralAmerica);
            westernRegions.Add(southAmerica);
            westernRegions.Add(northAmerica);
            Hemisphere westernHemisphere = new Hemisphere("Western Hemisphere", westernRegions);

            ObservableCollection<Region> easternRegions = new ObservableCollection<Region>();
            easternRegions.Add(europe);
            easternRegions.Add(asia);
            easternRegions.Add(oceania);
            easternRegions.Add(middleEast);
            easternRegions.Add(africa);
            easternRegions.Add(southeastAsia);
            Hemisphere easternHemisphere = new Hemisphere("Eastern Hemisphere", easternRegions);

            _hemispheres = new ObservableCollection<Hemisphere>();
            _hemispheres.Add(westernHemisphere);
            _hemispheres.Add(easternHemisphere);
        }
    }

    public class Hemisphere : INotifyPropertyChanged
    {
        private string _hemisphereName;
        private ObservableCollection<Region> _regions;

        #region Properies
        public string HemisphereName
        {
            get { return _hemisphereName; }
            set
            {
                _hemisphereName = value;
                OnPropertyChanged("PlanetName");
            }
        }

        public ObservableCollection<Region> Regions
        {
            get { return _regions; }
            set
            {
                _regions = value;
                OnPropertyChanged("Regions");
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

        public Hemisphere(string hemisphereName, ObservableCollection<Region> regions)
        {
            this._hemisphereName = hemisphereName;
            this._regions = regions;
        }
    }

    public class Region : INotifyPropertyChanged
    {
        private string _regionName;
        private ObservableCollection<Country> _countries;

        #region Properties
        public string RegionName
        {
            get { return _regionName; }
            set
            {
                _regionName = value;
                OnPropertyChanged("RegionName");
            }
        }

        public ObservableCollection<Country> Countries
        {
            get { return _countries; }
            set
            {
                _countries = value;
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
            this._regionName = regionName;
            this._countries = countries;
        }

        public Region()
        {
        }
    }

    public class Country : INotifyPropertyChanged
    {
        private string _countryName;
        private string _capital;
        private DateTime _independenceDay;
        private int _population;
        private GovernmentType _government;

        #region Properties
        public string CountryName
        {
            get { return _countryName; }
            set
            {
                _countryName = value;
                OnPropertyChanged("CountryName");
            }
        }

        public string Capital
        {
            get { return _capital; }
            set
            {
                _capital = value;
                OnPropertyChanged("Capital");
            }
        }

        public DateTime IndependenceDay
        {
            get { return _independenceDay; }
            set
            {
                _independenceDay = value;
                OnPropertyChanged("IndependenceDay");
            }
        }

        public int Population
        {
            get { return _population; }
            set
            {
                _population = value;
                OnPropertyChanged("Population");
            }
        }

        public GovernmentType Government
        {
            get { return _government; }
            set
            {
                _government = value;
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
            this._countryName = countryName;
            this._capital = capital;
            this._independenceDay = independenceDay;
            this._population = population;
            this._government = government;
        }

        public Country()
        {
        }
    }
    #endregion

    #region Object CountriesDataSource
    public class CountriesDataSource
    {
        private ObservableCollection<CountryWithExtraInfo> _countries;

        public ObservableCollection<CountryWithExtraInfo> Countries
        {
            get { return _countries; }
            set { _countries = value; }
        }

        public CountriesDataSource()
        {
            this._countries = new ObservableCollection<CountryWithExtraInfo>();

            this._countries.Add(new CountryWithExtraInfo("Yemen", "Sanaa", new DateTime(1990, 5, 22), 20024867, GovernmentType.Republic, "Middle East", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Qatar", "Doha", new DateTime(1971, 9, 3), 840290, GovernmentType.Monarchy, "Middle East", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Saint Lucia", "Castries", new DateTime(1979, 2, 22), 164213, GovernmentType.Democracy, "The Caribbean", "Western Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("San Marino", "San Marino", new DateTime(301, 9, 3), 28503, GovernmentType.Republic, "Europe", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Turkmenistan", "Ashgabat", new DateTime(1991, 10, 27), 4863169, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Moldova", "Chisinau", new DateTime(1991, 8, 27), 4446455, GovernmentType.Republic, "Europe", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Nauru", null, new DateTime(1968, 1, 31), 12809, GovernmentType.Republic, "Oceania", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Sierra Leone", "Freetown", new DateTime(1961, 4, 27), 5883889, GovernmentType.Democracy, "Africa", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Kyrgyzstan", "Bishkek", new DateTime(1991, 8, 31), 5081429, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Malawi", "Lilongwe", new DateTime(1964, 7, 6), 11906855, GovernmentType.Democracy, "Africa", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Djibouti", "Djibouti", new DateTime(1977, 6, 27), 466900, GovernmentType.Republic, "Africa", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("East Timor", "Dili", new DateTime(1975, 11, 28), 1019252, GovernmentType.Republic, "Southeast Asia", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Armenia", "Yerevan", new DateTime(1991, 9, 21), 2991360, GovernmentType.Republic, "Asia", "Eastern Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("El Salvador", "San Salvador", new DateTime(1821, 9, 15), 6587541, GovernmentType.Republic, "Central America", "Western Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Guyana", "Georgetown", new DateTime(1966, 5, 26), 705803, GovernmentType.Republic, "South America", "Western Hemisphere"));
            this._countries.Add(new CountryWithExtraInfo("Mexico", "Mexico", new DateTime(1810, 9, 16), 104959594, GovernmentType.Republic, "North America", "Western Hemisphere"));
        }
    }

    public class CountryWithExtraInfo : Country
    {
        private string _region;
        private string _hemisphere;

        #region Properties
        public string Region
        {
            get { return _region; }
            set
            {
                _region = value;
                OnPropertyChanged("Region");
            }
        }

        public string Hemisphere
        {
            get { return _hemisphere; }
            set { _hemisphere = value; }
        }
        #endregion

        public CountryWithExtraInfo(string countryName, string capital, DateTime independenceDay, int population, GovernmentType government, string region, string hemisphere)
            : base(countryName, capital, independenceDay, population, government)
        {
            this._region = region;
            this._hemisphere = hemisphere;
        }
    }
    #endregion

    #region ADO.NET EarthDataSet
    public class EarthDataSet : DataSet
    {
        public EarthDataSet()
        {
            this.Tables.Add(new HemispheresDataTable());
            this.Tables.Add(new RegionsDataTable());
            this.Tables.Add(new CountriesDataTable());

            // foreign keys
            this.Relations.Add(new DataRelation("HemispheresRegions", this.Tables["Hemispheres"].Columns["HemisphereID"],
                this.Tables["Regions"].Columns["HemisphereID"]));

            this.Relations.Add(new DataRelation("RegionsCountries", this.Tables["Regions"].Columns["RegionID"],
                this.Tables["Countries"].Columns["RegionID"]));
        }
    }

    public class HemispheresDataTable : DataTable
    {
        public HemispheresDataTable()
            : base("Hemispheres")
        {
            this.Columns.Add("HemisphereID", typeof(int));
            this.Columns["HemisphereID"].Unique = true;
            this.Columns.Add("HemisphereName", typeof(string));

            // Western Hemisphere
            DataRow westernHemisphere = this.NewRow();
            westernHemisphere["HemisphereID"] = 1;
            westernHemisphere["HemisphereName"] = "Western Hemisphere";
            this.Rows.Add(westernHemisphere);

            // Eastern Hemisphere
            DataRow easternHemisphere = this.NewRow();
            easternHemisphere["HemisphereID"] = 2;
            easternHemisphere["HemisphereName"] = "Eastern Hemisphere";
            this.Rows.Add(easternHemisphere);

            // HemisphereID is the primary key
            //this.Constraints.Add(new UniqueConstraint(this.Columns["HemisphereID"], true));
        }
    }

    public class RegionsDataTable : DataTable
    {
        public RegionsDataTable()
            : base("Regions")
        {
            this.Columns.Add("HemisphereID", typeof(int));
            this.Columns.Add("RegionID", typeof(int));
            this.Columns.Add("RegionName", typeof(string));

            // Middle East
            DataRow middleEast = this.NewRow();
            middleEast["RegionID"] = 1;
            middleEast["HemisphereID"] = 2;
            middleEast["RegionName"] = "Middle East";
            this.Rows.Add(middleEast);

            // The Caribbean
            DataRow theCaribbean = this.NewRow();
            theCaribbean["RegionID"] = 2;
            theCaribbean["HemisphereID"] = 1;
            theCaribbean["RegionName"] = "The Caribbean";
            this.Rows.Add(theCaribbean);

            // Europe
            DataRow europe = this.NewRow();
            europe["RegionID"] = 3;
            europe["HemisphereID"] = 2;
            europe["RegionName"] = "Europe";
            this.Rows.Add(europe);

            // Asia
            DataRow asia = this.NewRow();
            asia["RegionID"] = 4;
            asia["HemisphereID"] = 2;
            asia["RegionName"] = "Asia";
            this.Rows.Add(asia);

            // Oceania
            DataRow oceania = this.NewRow();
            oceania["RegionID"] = 5;
            oceania["HemisphereID"] = 2;
            oceania["RegionName"] = "Asia";
            this.Rows.Add(oceania);

            // Africa
            DataRow africa = this.NewRow();
            africa["RegionID"] = 6;
            africa["HemisphereID"] = 2;
            africa["RegionName"] = "Africa";
            this.Rows.Add(africa);

            // Southeast Asia
            DataRow southeastAsia = this.NewRow();
            southeastAsia["RegionID"] = 7;
            southeastAsia["HemisphereID"] = 2;
            southeastAsia["RegionName"] = "Southeast Asia";
            this.Rows.Add(southeastAsia);

            // Central America
            DataRow centralAmerica = this.NewRow();
            centralAmerica["RegionID"] = 8;
            centralAmerica["HemisphereID"] = 1;
            centralAmerica["RegionName"] = "Central America";
            this.Rows.Add(centralAmerica);

            // South America
            DataRow southAmerica = this.NewRow();
            southAmerica["RegionID"] = 9;
            southAmerica["HemisphereID"] = 1;
            southAmerica["RegionName"] = "South America";
            this.Rows.Add(southAmerica);

            // North America
            DataRow northAmerica = this.NewRow();
            northAmerica["RegionID"] = 10;
            northAmerica["HemisphereID"] = 1;
            northAmerica["RegionName"] = "North America";
            this.Rows.Add(northAmerica);

            // RegionID is the primary key
            //this.Constraints.Add(new UniqueConstraint(this.Columns["RegionID"], true));
            // HemisphereID is a foreign key, but this is set in the DataSet's Relations
        }
    }

    public class CountriesDataTable : DataTable
    {
        public CountriesDataTable()
            : base("Countries")
        {
            this.Columns.Add("RegionID", typeof(int));
            this.Columns.Add("CountryID", typeof(int));
            this.Columns.Add("CountryName", typeof(string));
            this.Columns.Add("Capital", typeof(string));
            this.Columns.Add("IndependenceDay", typeof(DateTime));
            this.Columns.Add("Population", typeof(int));
            this.Columns.Add("Government", typeof(GovernmentType));

            // Yemen
            DataRow yemen = this.NewRow();
            yemen["RegionID"] = 1;
            yemen["CountryID"] = 1;
            yemen["CountryName"] = "Yemen";
            yemen["Capital"] = "Sanaa";
            yemen["IndependenceDay"] = new DateTime(1990, 5, 22);
            yemen["Population"] = 20024867;
            yemen["Government"] = GovernmentType.Republic;
            this.Rows.Add(yemen);

            // Qatar
            DataRow qatar = this.NewRow();
            qatar["RegionID"] = 1;
            qatar["CountryID"] = 2;
            qatar["CountryName"] = "Qatar";
            qatar["Capital"] = "Doha";
            qatar["IndependenceDay"] = new DateTime(1971, 9, 3);
            qatar["Population"] = 840290;
            qatar["Government"] = GovernmentType.Monarchy;
            this.Rows.Add(qatar);

            // Saint Lucia
            DataRow saintLucia = this.NewRow();
            saintLucia["RegionID"] = 2;
            saintLucia["CountryID"] = 3;
            saintLucia["CountryName"] = "Saint Lucia";
            saintLucia["Capital"] = "Castries";
            saintLucia["IndependenceDay"] = new DateTime(1979, 2, 22);
            saintLucia["Population"] = 164213;
            saintLucia["Government"] = GovernmentType.Democracy;
            this.Rows.Add(saintLucia);

            // San Marino
            DataRow sanMarino = this.NewRow();
            sanMarino["RegionID"] = 3;
            sanMarino["CountryID"] = 4;
            sanMarino["CountryName"] = "San Marino";
            sanMarino["Capital"] = "San Marino";
            sanMarino["IndependenceDay"] = new DateTime(301, 9, 3);
            sanMarino["Population"] = 28503;
            sanMarino["Government"] = GovernmentType.Republic;
            this.Rows.Add(sanMarino);

            // Turkmenistan
            DataRow turkmenistan = this.NewRow();
            turkmenistan["RegionID"] = 4;
            turkmenistan["CountryID"] = 5;
            turkmenistan["CountryName"] = "Turkmenistan";
            turkmenistan["Capital"] = "Ashgabat";
            turkmenistan["IndependenceDay"] = new DateTime(1991, 10, 27);
            turkmenistan["Population"] = 4863169;
            turkmenistan["Government"] = GovernmentType.Republic;
            this.Rows.Add(turkmenistan);

            // Moldova
            DataRow moldova = this.NewRow();
            moldova["RegionID"] = 3;
            moldova["CountryID"] = 6;
            moldova["CountryName"] = "Moldova";
            moldova["Capital"] = "Chisinau";
            moldova["IndependenceDay"] = new DateTime(1991, 8, 27);
            moldova["Population"] = 4446455;
            moldova["Government"] = GovernmentType.Republic;
            this.Rows.Add(moldova);

            // Nauru
            DataRow nauru = this.NewRow();
            nauru["RegionID"] = 5;
            nauru["CountryID"] = 7;
            nauru["CountryName"] = "Nauru";
            nauru["Capital"] = null;
            nauru["IndependenceDay"] = new DateTime(1968, 1, 31);
            nauru["Population"] = 12809;
            nauru["Government"] = GovernmentType.Republic;
            this.Rows.Add(nauru);

            // Sierra Leone
            DataRow sierraLeone = this.NewRow();
            sierraLeone["RegionID"] = 6;
            sierraLeone["CountryID"] = 8;
            sierraLeone["CountryName"] = "Sierra Leone";
            sierraLeone["Capital"] = "Freetown";
            sierraLeone["IndependenceDay"] = new DateTime(1961, 4, 27);
            sierraLeone["Population"] = 5883889;
            sierraLeone["Government"] = GovernmentType.Democracy;
            this.Rows.Add(sierraLeone);

            // Kyrgyzstan
            DataRow kyrgyzstan = this.NewRow();
            kyrgyzstan["RegionID"] = 4;
            kyrgyzstan["CountryID"] = 9;
            kyrgyzstan["CountryName"] = "Kyrgyzstan";
            kyrgyzstan["Capital"] = "Bishkek";
            kyrgyzstan["IndependenceDay"] = new DateTime(1991, 8, 31);
            kyrgyzstan["Population"] = 5081429;
            kyrgyzstan["Government"] = GovernmentType.Republic;
            this.Rows.Add(kyrgyzstan);

            // Malawi
            DataRow malawi = this.NewRow();
            malawi["RegionID"] = 6;
            malawi["CountryID"] = 10;
            malawi["CountryName"] = "Malawi";
            malawi["Capital"] = "Lilongwe";
            malawi["IndependenceDay"] = new DateTime(1964, 7, 6);
            malawi["Population"] = 11906855;
            malawi["Government"] = GovernmentType.Democracy;
            this.Rows.Add(malawi);

            // Djibouti
            DataRow djibouti = this.NewRow();
            djibouti["RegionID"] = 6;
            djibouti["CountryID"] = 11;
            djibouti["CountryName"] = "Djibouti";
            djibouti["Capital"] = "Djibouti";
            djibouti["IndependenceDay"] = new DateTime(1977, 6, 27);
            djibouti["Population"] = 466900;
            djibouti["Government"] = GovernmentType.Republic;
            this.Rows.Add(djibouti);

            // East Timor
            DataRow eastTimor = this.NewRow();
            eastTimor["RegionID"] = 7;
            eastTimor["CountryID"] = 12;
            eastTimor["CountryName"] = "East Timor";
            eastTimor["Capital"] = "Dili";
            eastTimor["IndependenceDay"] = new DateTime(1975, 11, 28);
            eastTimor["Population"] = 1019252;
            eastTimor["Government"] = GovernmentType.Republic;
            this.Rows.Add(eastTimor);

            // Armenia
            DataRow armenia = this.NewRow();
            armenia["RegionID"] = 4;
            armenia["CountryID"] = 13;
            armenia["CountryName"] = "Armenia";
            armenia["Capital"] = "Yerevan";
            armenia["IndependenceDay"] = new DateTime(1991, 9, 21);
            armenia["Population"] = 2991360;
            armenia["Government"] = GovernmentType.Republic;
            this.Rows.Add(armenia);

            // El Salvador
            DataRow elSalvador = this.NewRow();
            elSalvador["RegionID"] = 8;
            elSalvador["CountryID"] = 14;
            elSalvador["CountryName"] = "El Salvador";
            elSalvador["Capital"] = "San Salvador";
            elSalvador["IndependenceDay"] = new DateTime(1821, 9, 15);
            elSalvador["Population"] = 6587541;
            elSalvador["Government"] = GovernmentType.Republic;
            this.Rows.Add(elSalvador);

            // Guyana
            DataRow guyana = this.NewRow();
            guyana["RegionID"] = 9;
            guyana["CountryID"] = 15;
            guyana["CountryName"] = "Guyana";
            guyana["Capital"] = "Georgetown";
            guyana["IndependenceDay"] = new DateTime(1966, 5, 26);
            guyana["Population"] = 705803;
            guyana["Government"] = GovernmentType.Republic;
            this.Rows.Add(guyana);

            // Mexico
            DataRow mexico = this.NewRow();
            mexico["RegionID"] = 10;
            mexico["CountryID"] = 16;
            mexico["CountryName"] = "Mexico";
            mexico["Capital"] = "Mexico";
            mexico["IndependenceDay"] = new DateTime(1810, 9, 16);
            mexico["Population"] = 104959594;
            mexico["Government"] = GovernmentType.Republic;
            this.Rows.Add(mexico);

            // CountryID is the primary key
            //this.Constraints.Add(new UniqueConstraint(this.Columns["CountryID"], true));
            // RegionID is a foreign key, but this is set in the DataSet's Relations
        }
    }
    #endregion
}
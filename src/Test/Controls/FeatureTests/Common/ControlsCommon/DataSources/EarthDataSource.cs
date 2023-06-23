using System;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Data;

namespace Avalon.Test.ComponentModel
{
    // For a hierarchical object data source: EarthDataSource + Hemisphere + Region + Country
    // For a flat object data source with potential to be turned into hierarhcical through grouping: CountriesDataSource + CountryWithExtraInfo
    // For ADO.NET hierarhical data: EarthDataSet


    #region Object EarthDataSource
    public class EarthDataSource
    {
        private ObservableCollection<Hemisphere> hemispheres;

        public ObservableCollection<Hemisphere> Hemispheres
        {
            get { return hemispheres; }
            set { hemispheres = value; }
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

            hemispheres = new ObservableCollection<Hemisphere>();
            hemispheres.Add(westernHemisphere);
            hemispheres.Add(easternHemisphere);
        }
    }

    #endregion
}
using System;
using System.Collections.ObjectModel;
using System.Data;

namespace Avalon.Test.ComponentModel.DataSources
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
            Add(new Place("Dover", "NJ"));
            Add(new Place("Morristown", "NJ"));
            Add(new Place("Pinebrook", "NJ"));
            Add(new Place("Cincinnati", "OH"));
            Add(new Place("Dayton", "OH"));
            Add(new Place("Carrollton ", "GA"));
            Add(new Place("Portland", "ME"));
            Add(new Place("Boston", "MA"));
            Add(new Place("Trenton", "NJ"));
            Add(new Place("Chicago", "IL"));
            Add(new Place("Houston", "TX"));
            Add(new Place("Austin", "TX"));
            Add(new Place("Baltimore", "MD"));
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
            Add(new Place("Dover", "NJ"));
            Add(new Place("Morristown", "NJ"));
            Add(new Place("Pinebrook", "NJ"));
            Add(new Place("Cincinnati", "OH"));
            Add(new Place("Dayton", "OH"));
            Add(new Place("Carrollton", "GA"));
            Add(new Place("Portland", "ME"));
            Add(new Place("Boston", "MA"));
            Add(new Place("Trenton", "NJ"));
            Add(new Place("Chicago", "IL"));
            Add(new Place("Houston", "TX"));
            Add(new Place("Austin", "TX"));
            Add(new Place("Baltimore", "MD"));

            Add(new Place("Seattle1", "WA"));
            Add(new Place("Redmond1", "WA"));
            Add(new Place("Bellevue1", "WA"));
            Add(new Place("Kirkland1", "WA"));
            Add(new Place("Portland1", "OR"));
            Add(new Place("San Francisco1", "CA"));
            Add(new Place("Los Angeles1", "CA"));
            Add(new Place("San Diego1", "CA"));
            Add(new Place("San Jose1", "CA"));
            Add(new Place("Santa Ana1", "CA"));
            Add(new Place("Bellingham1", "WA"));
            Add(new Place("Dover1", "NJ"));
            Add(new Place("Morristown1", "NJ"));
            Add(new Place("Pinebrook1", "NJ"));
            Add(new Place("Cincinnati1", "OH"));
            Add(new Place("Dayton1", "OH"));
            Add(new Place("Carrollton1", "GA"));
            Add(new Place("Portland1", "ME"));
            Add(new Place("Boston1", "MA"));
            Add(new Place("Trenton1", "NJ"));
            Add(new Place("Chicago1", "IL"));
            Add(new Place("Houston1", "TX"));
            Add(new Place("Austin1", "TX"));
            Add(new Place("Baltimore1", "MD"));

            Add(new Place("Seattle2", "WA"));
            Add(new Place("Redmond2", "WA"));
            Add(new Place("Bellevue2", "WA"));
            Add(new Place("Kirkland2", "WA"));
            Add(new Place("Portland2", "OR"));
            Add(new Place("San Francisco2", "CA"));
            Add(new Place("Los Angeles2", "CA"));
            Add(new Place("San Diego2", "CA"));
            Add(new Place("San Jose2", "CA"));
            Add(new Place("Santa Ana2", "CA"));
            Add(new Place("Bellingham2", "WA"));
            Add(new Place("Dover2", "NJ"));
            Add(new Place("Morristown2", "NJ"));
            Add(new Place("Pinebrook2", "NJ"));
            Add(new Place("Cincinnati2", "OH"));
            Add(new Place("Dayton2", "OH"));
            Add(new Place("Carrollton2", "GA"));
            Add(new Place("Portland2", "ME"));
            Add(new Place("Boston2", "MA"));
            Add(new Place("Trenton2", "NJ"));
            Add(new Place("Chicago2", "IL"));
            Add(new Place("Houston2", "TX"));
            Add(new Place("Austin2", "TX"));
            Add(new Place("Baltimore2", "MD"));

            Add(new Place("Seattle3", "WA"));
            Add(new Place("Redmond3", "WA"));
            Add(new Place("Bellevue3", "WA"));
            Add(new Place("Kirkland3", "WA"));
            Add(new Place("Portland3", "OR"));
            Add(new Place("San Francisco3", "CA"));
            Add(new Place("Los Angeles3", "CA"));
            Add(new Place("San Diego3", "CA"));
            Add(new Place("San Jose3", "CA"));
            Add(new Place("Santa Ana3", "CA"));
            Add(new Place("Bellingham3", "WA"));
            Add(new Place("Dover3", "NJ"));
            Add(new Place("Morristown3", "NJ"));
            Add(new Place("Pinebrook3", "NJ"));
            Add(new Place("Cincinnati3", "OH"));
            Add(new Place("Dayton3", "OH"));
            Add(new Place("Carrollton3", "GA"));
            Add(new Place("Portland3", "ME"));
            Add(new Place("Boston3", "MA"));
            Add(new Place("Trenton3", "NJ"));
            Add(new Place("Chicago3", "IL"));
            Add(new Place("Houston3", "TX"));
            Add(new Place("Austin3", "TX"));
            Add(new Place("Baltimore3", "MD"));

        }
    }

    public class Place
    {
        private string name;

        private string state;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public string State
        {
            get { return state; }
            set { state = value; }
        }

        public Place()
        {
            this.name = "";
            this.state = "";
        }

        public Place(string name, string state)
        {
            this.name = name;
            this.state = state;
        }
    }
}

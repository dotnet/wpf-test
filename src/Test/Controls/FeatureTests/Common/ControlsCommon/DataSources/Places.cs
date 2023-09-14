using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    public class Places : ObservableCollection<USPlace>
    {
        public Places()
        {
            #region default data

            Add(new USPlace("Seattle", "WA"));
            Add(new USPlace("Redmond", "WA"));
            Add(new USPlace("Bellevue", "WA"));
            Add(new USPlace("Kirkland", "WA"));
            Add(new USPlace("Portland", "OR"));
            Add(new USPlace("San Francisco", "CA"));
            Add(new USPlace("Los Angeles", "CA"));
            Add(new USPlace("San Diego", "CA"));
            Add(new USPlace("San Jose", "CA"));
            Add(new USPlace("Santa Ana", "CA"));
            Add(new USPlace("Bellingham", "WA"));
            Add(new USPlace("Dover", "NJ"));
            Add(new USPlace("Morristown", "NJ"));
            Add(new USPlace("Pinebrook", "NJ"));
            Add(new USPlace("Cincinnati", "OH"));
            Add(new USPlace("Dayton", "OH"));
            Add(new USPlace("Carrollton ", "GA"));
            Add(new USPlace("Portland", "ME"));
            Add(new USPlace("Boston", "MA"));
            Add(new USPlace("Trenton", "NJ"));
            Add(new USPlace("Chicago", "IL"));
            Add(new USPlace("Houston", "TX"));
            Add(new USPlace("Austin", "TX"));
            Add(new USPlace("Baltimore", "MD"));
            Add(new USPlace("Seattle", "WA"));
            Add(new USPlace("Redmond", "WA"));
            Add(new USPlace("Bellevue", "WA"));
            Add(new USPlace("Kirkland", "WA"));
            Add(new USPlace("Portland", "OR"));
            Add(new USPlace("San Francisco", "CA"));
            Add(new USPlace("Los Angeles", "CA"));
            Add(new USPlace("San Diego", "CA"));
            Add(new USPlace("San Jose", "CA"));
            Add(new USPlace("Santa Ana", "CA"));
            Add(new USPlace("Bellingham", "WA"));
            Add(new USPlace("Dover", "NJ"));
            Add(new USPlace("Morristown", "NJ"));
            Add(new USPlace("Pinebrook", "NJ"));
            Add(new USPlace("Cincinnati", "OH"));
            Add(new USPlace("Dayton", "OH"));
            Add(new USPlace("Carrollton", "GA"));
            Add(new USPlace("Portland", "ME"));
            Add(new USPlace("Boston", "MA"));
            Add(new USPlace("Trenton", "NJ"));
            Add(new USPlace("Chicago", "IL"));
            Add(new USPlace("Houston", "TX"));
            Add(new USPlace("Austin", "TX"));
            Add(new USPlace("Baltimore", "MD"));

            Add(new USPlace("Seattle1", "WA"));
            Add(new USPlace("Redmond1", "WA"));
            Add(new USPlace("Bellevue1", "WA"));
            Add(new USPlace("Kirkland1", "WA"));
            Add(new USPlace("Portland1", "OR"));
            Add(new USPlace("San Francisco1", "CA"));
            Add(new USPlace("Los Angeles1", "CA"));
            Add(new USPlace("San Diego1", "CA"));
            Add(new USPlace("San Jose1", "CA"));
            Add(new USPlace("Santa Ana1", "CA"));
            Add(new USPlace("Bellingham1", "WA"));
            Add(new USPlace("Dover1", "NJ"));
            Add(new USPlace("Morristown1", "NJ"));
            Add(new USPlace("Pinebrook1", "NJ"));
            Add(new USPlace("Cincinnati1", "OH"));
            Add(new USPlace("Dayton1", "OH"));
            Add(new USPlace("Carrollton1", "GA"));
            Add(new USPlace("Portland1", "ME"));
            Add(new USPlace("Boston1", "MA"));
            Add(new USPlace("Trenton1", "NJ"));
            Add(new USPlace("Chicago1", "IL"));
            Add(new USPlace("Houston1", "TX"));
            Add(new USPlace("Austin1", "TX"));
            Add(new USPlace("Baltimore1", "MD"));

            Add(new USPlace("Seattle2", "WA"));
            Add(new USPlace("Redmond2", "WA"));
            Add(new USPlace("Bellevue2", "WA"));
            Add(new USPlace("Kirkland2", "WA"));
            Add(new USPlace("Portland2", "OR"));
            Add(new USPlace("San Francisco2", "CA"));
            Add(new USPlace("Los Angeles2", "CA"));
            Add(new USPlace("San Diego2", "CA"));
            Add(new USPlace("San Jose2", "CA"));
            Add(new USPlace("Santa Ana2", "CA"));
            Add(new USPlace("Bellingham2", "WA"));
            Add(new USPlace("Dover2", "NJ"));
            Add(new USPlace("Morristown2", "NJ"));
            Add(new USPlace("Pinebrook2", "NJ"));
            Add(new USPlace("Cincinnati2", "OH"));
            Add(new USPlace("Dayton2", "OH"));
            Add(new USPlace("Carrollton2", "GA"));
            Add(new USPlace("Portland2", "ME"));
            Add(new USPlace("Boston2", "MA"));
            Add(new USPlace("Trenton2", "NJ"));
            Add(new USPlace("Chicago2", "IL"));
            Add(new USPlace("Houston2", "TX"));
            Add(new USPlace("Austin2", "TX"));
            Add(new USPlace("Baltimore2", "MD"));

            Add(new USPlace("Seattle3", "WA"));
            Add(new USPlace("Redmond3", "WA"));
            Add(new USPlace("Bellevue3", "WA"));
            Add(new USPlace("Kirkland3", "WA"));
            Add(new USPlace("Portland3", "OR"));
            Add(new USPlace("San Francisco3", "CA"));
            Add(new USPlace("Los Angeles3", "CA"));
            Add(new USPlace("San Diego3", "CA"));
            Add(new USPlace("San Jose3", "CA"));
            Add(new USPlace("Santa Ana3", "CA"));
            Add(new USPlace("Bellingham3", "WA"));
            Add(new USPlace("Dover3", "NJ"));
            Add(new USPlace("Morristown3", "NJ"));
            Add(new USPlace("Pinebrook3", "NJ"));
            Add(new USPlace("Cincinnati3", "OH"));
            Add(new USPlace("Dayton3", "OH"));
            Add(new USPlace("Carrollton3", "GA"));
            Add(new USPlace("Portland3", "ME"));
            Add(new USPlace("Boston3", "MA"));
            Add(new USPlace("Trenton3", "NJ"));
            Add(new USPlace("Chicago3", "IL"));
            Add(new USPlace("Houston3", "TX"));
            Add(new USPlace("Austin3", "TX"));
            Add(new USPlace("Baltimore3", "MD"));

            #endregion
        }

        /// <summary>
        /// Support for data coming from outside
        /// </summary>
        /// <param name="places"></param>
        public Places(List<USPlace> places)
        {
            foreach(USPlace place in places)
            {
                Add(place);
            }
        }
    }
}

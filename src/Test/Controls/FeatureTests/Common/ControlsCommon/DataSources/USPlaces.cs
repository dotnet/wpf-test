using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.Test.Controls
{
    public class USPlaces : ObservableCollection<USPlace>
    {
        public USPlaces()
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

            #endregion
        }

        /// <summary>
        /// Support for data coming from outside
        /// </summary>
        /// <param name="places"></param>
        public USPlaces(List<USPlace> places)
        {
            foreach(USPlace place in places)
            {
                Add(place);
            }
        }
    }
}

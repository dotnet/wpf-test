// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Data;

namespace Microsoft.Test.DataServices
{
    public class MyFancyIEnumerable : IEnumerable
    {
        private ArrayList _myList;

        public MyFancyIEnumerable()
        {
            Place place = new Place();
            place.State = "VT";
            place.Name = "Burlington";

            _myList = new ArrayList();
            _myList.Add("a");
            _myList.Add(10);
            _myList.Add(DateTime.Now);
            _myList.Add(place);
        }

        public IEnumerator GetEnumerator()
        {
            return _myList.GetEnumerator();
        }

        public int Count
        {
            get { return _myList.Count; }
        }

        public int IndexOf(object item)
        {
            return _myList.IndexOf(item);
        }

        // The two IndexOf methods below are here to make sure that IndexerEnumerable does not get confused 
        public int IndexOf(object item, int startIndex)
        {
            return _myList.IndexOf(item, startIndex);
        }

        public int IndexOf(object item, int startIndex, int count)
        {
            return _myList.IndexOf(item, startIndex, count);
        }
        
        public object this[int index]
        {
            get { return _myList[index]; }
        }

        // The next 2 indexers are to make sure IndexerEnumerable does not get confused
        public object this[double index]
        {
            get { return _myList[0]; }
        }

        public object this[int index1, int index2]
        {
            get { return _myList[index1]; }
        }
    }
}

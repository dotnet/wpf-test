// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Data;

namespace Microsoft.Test.DataServices
{
    public class MyIEnumerable : IEnumerable
    {
        private ArrayList _myList;
        private int _numGetEnumeratorCalls;

        public MyIEnumerable()
        {
            NumGetEnumeratorCalls = 0;
            _myList = new ArrayList();
            _myList.Add("a");
            _myList.Add("b");
            _myList.Add("c");
            _myList.Add("d");
        }

        public IEnumerator GetEnumerator()
        {
            NumGetEnumeratorCalls++;
            return _myList.GetEnumerator();
        }

        public int NumGetEnumeratorCalls
        {
            get { return _numGetEnumeratorCalls; }
            set 
            { 
                _numGetEnumeratorCalls = value;
            }
        }
    }
}

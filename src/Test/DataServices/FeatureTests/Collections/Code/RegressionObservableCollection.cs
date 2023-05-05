// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for ObservableCollection Bug Fixes
    /// </description>
    /// </summary>
    [Test(2, "Collections", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionObservableCollection : WindowTest
    {
        #region Constructors

        public RegressionObservableCollection()
        {
            RunSteps += new TestStep(IEnumerableOfTConstructor);
            RunSteps += new TestStep(CollectionChangedNotSerializable);
            RunSteps += new TestStep(ReadOnlyObservableCollectionSerializable);
        }

        #endregion

        #region Private Members

        // Suggest having a constructor for ObservableCollection that receives an IEnumerable<T> as a parameter
        private TestResult IEnumerableOfTConstructor()
        {
            IEnumerableOfPlaceDataSource places = new IEnumerableOfPlaceDataSource();
            ObservableCollection<Place> placesOC = new ObservableCollection<Place>(new IEnumerableOfPlaceDataSource());

            // The Places collection used as a source has 11 items, so we'll validate expected count.
            // We can't do item by item comparison since we are cloning and they therefore aren't reference equal.
            if (placesOC.Count != 11) return TestResult.Fail;

            return TestResult.Pass;
        }

        // ObservableCollection is marked as serializable, but CollectionChanged is not marked as 'Not for Serialization'
        private TestResult CollectionChangedNotSerializable()
        {
            ObservableCollection<string> strings = new ObservableCollection<string>();
            NonSerializableObserver nonSerializableObserver = new NonSerializableObserver();
            BinaryFormatter formatter = new BinaryFormatter();

            strings.CollectionChanged += nonSerializableObserver.CollectionChanged;

            using (MemoryStream stream = new MemoryStream())
            {
                // Causes a SerializationException before fix.
                formatter.Serialize(stream, strings);
            }

            return TestResult.Pass;
        }

        // System.Collections.ObjectModel.ReadOnlyObservableCollection<T> not marked serializable
        private TestResult ReadOnlyObservableCollectionSerializable()
        {
            ObservableCollection<string> stringsOC = new ObservableCollection<string>();
            NonSerializableObserver nonSerializableObserver = new NonSerializableObserver();
        
            stringsOC.CollectionChanged += nonSerializableObserver.CollectionChanged;
            ReadOnlyObservableCollection<string> strings = new ReadOnlyObservableCollection<string>(stringsOC);
            BinaryFormatter formatter = new BinaryFormatter();

            using (MemoryStream stream = new MemoryStream())
            {
                // Causes a SerializationException before fix.
                formatter.Serialize(stream, strings);
            }

            return TestResult.Pass;
        }

        private class NonSerializableObserver
        {
            public void CollectionChanged(object sender, EventArgs e) { }
        }

        private class IEnumerableOfPlaceDataSource : IEnumerable<Place>
        {
            private Places _data = new Places();

            #region IEnumerable<Place> Members

            public IEnumerator<Place> GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            #endregion

            #region IEnumerable Members

            System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
            {
                return _data.GetEnumerator();
            }

            #endregion
        }

        #endregion
    }
}

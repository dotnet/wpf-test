// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Add and remove items to a collection that implements IEnumerable and
    /// INotifyPropertyChanged, verifying that CollectionView property values
    /// are as expected.
    /// </description>
    /// <relatedBugs>


    /// </relatedBugs>
    /// </summary>
    [Test(0, "Views", "CollectionChangedSelectionCurrency")]
    public class CollectionChangedSelectionCurrency : WindowTest
    {
        private ListBox _lb;
        private MyLibraryEnumerable _mle;
        private CollectionView _cv;

        public CollectionChangedSelectionCurrency()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AddRemoveItems);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _mle = new MyLibraryEnumerable(0);
            _lb = new ListBox();
            _cv = (CollectionView)CollectionViewSource.GetDefaultView(_mle);
            _lb.ItemsSource = _cv;

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        // A simplistic BVT that is effectively a random walk through a state-
        // based model. Long term, a P1 that either uses modeling or attribute
        // variations would be nice. Even better if we better unify our data
        // sources where this test could be run with various types of data
        // sources backing the collection view.
        private TestResult AddRemoveItems()
        {
            Status("Add and Remove Items");

            Book book1 = new Book();
            Book book2 = new Book();
            Book book3 = new Book();
            Book book4 = new Book();

            if (!Verify(true, -1, null))
                return TestResult.Fail;

            // What is this you ask? Good question! MyLibraryEnumerable will
            // not raise events unless you set this property. The logic of
            // the testcase this source was originally created for seems to
            // have perhaps permeated the data source.
            _mle.ActionToBePassed = NotifyCollectionChangedAction.Add;
            _mle.ChangedItemToBePassed = book1;

            _mle.Add(book1);

            if (!Verify(false, -1, null))
                return TestResult.Fail;

            _cv.MoveCurrentToFirst();

            if (!Verify(false, 0, book1))
                return TestResult.Fail;

            // See earlier comment about weird event raising on the data source.
            _mle.ActionToBePassed = NotifyCollectionChangedAction.Add;
            _mle.ChangedItemToBePassed = book2;

            _mle.Add(book2);


            // See earlier comment about weird event raising on the data source.
            _mle.ActionToBePassed = NotifyCollectionChangedAction.Add;
            _mle.ChangedItemToBePassed = book3;

            _mle.Add(book3);

            if (!Verify(false, 0, book1))
                return TestResult.Fail;

            _cv.MoveCurrentToNext();

            if (!Verify(false, 1, book2))
                return TestResult.Fail;

            // See earlier comment about weird event raising on the data source.
            _mle.ActionToBePassed = NotifyCollectionChangedAction.Add;
            _mle.ChangedItemToBePassed = book4;

            _mle.Add(book4);

            if (!Verify(false, 1, book2))
                return TestResult.Fail;

            // See earlier comment about weird event raising on the data source.
            _mle.ActionToBePassed = NotifyCollectionChangedAction.Remove;
            _mle.ChangedItemToBePassed = book1;
            _mle.IndexToBePassed = 0;

            _mle.Remove(book1);

            if (!Verify(false, 0, book2))
                return TestResult.Fail;

            if (_mle.NumGetEnumeratorCalls != 1)
                return TestResult.Fail;

            LogComment("Add and Remove Items passed.");
            return TestResult.Pass;
        }

        // Simplistic verifier. There are three or four IVerifiers that all
        // test related aspects of selection, currency, and collection views,
        // but none of them match this cases needs. For BVT coverage I'll just
        // use this. Long term, it would be great to investigate merging those
        // IVerifiers to some degree, along with incorporating these needs.
        private bool Verify(bool isEmpty, int currentPosition, object currentItem)
        {
            if (isEmpty == _cv.IsEmpty && currentPosition == _cv.CurrentPosition && currentItem == _cv.CurrentItem)
                return true;
            else
                return false;
        }
    }
}


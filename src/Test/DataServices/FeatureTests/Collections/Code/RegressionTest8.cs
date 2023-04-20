// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Data;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Test user defined Collection inherit from CollectionView to support range action on different verion of .NET.
    /// Range actions are supported at .NET4.0, if app build to .NET4.0 range action will be all right.
    /// Range actions are not supported above .NET4.5, if app build to .NET4.5,or .NET4.5.1, a NotSupportedException will throw by .NET.
    /// </description>
    /// </summary>

    // [DISABLED_WHILE_PORTING]
    // [Test(2, "Collections", "RegressionTest8")]
    public class RegressionTest8 : AvalonTest
    {
        #region Private field

        private MyRangeCollectionView<int> _rangeCollection;
        private ItemsControl _itemContainer;

        #endregion

        #region Constructor

        public RegressionTest8()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Verify);
            RunSteps += new TestStep(CleanUp);
        }
        #endregion

        #region Test Steps

        TestResult Setup()
        {
            Status("Setup");
            _rangeCollection = new MyRangeCollectionView<int>(Enumerable.Range(0, 10).ToList());
            _itemContainer = new ItemsControl();
            _itemContainer.ItemsSource = _rangeCollection;
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _rangeCollection = null;
            _itemContainer = null;
            return TestResult.Pass;
        }

        TestResult Verify()
        {
            Status("Verify Range Actions");

            try
            {
                LogComment("Raise CollectionChanged event with a range action!");
                _rangeCollection.RaiseCollectionChangedEvent();
            }
            catch (NotSupportedException notSupport)
            {
#if TESTBUILD_NET_ATLEAST_45 // NotSupportedExpction is expected to throw at .NET4.5+
                LogComment(string.Format("Expected Exception is caught:\n,Details:{0} ", notSupport));
                return TestResult.Pass;
#else  // NotSupportedExpction is not expected to throw bellow .NET4.0-

                LogComment(string.Format("Unexpected Exception is caught:\nThis exception should just throw on .net 4.5 or .net4.5+\nDetails:{0} ", notSupport));
                return TestResult.Fail;
#endif
            }
            catch (Exception unexcept)
            {
                LogComment(string.Format("Unexpected Exception is caught:\n,Details:{0} ", unexcept));
                return TestResult.Fail;
            }

#if TESTBUILD_NET_ATLEAST_45
            LogComment("Range action should not supported on .NET 4.5+, but it appeared supported.");
            return TestResult.Fail;
#else
            return TestResult.Pass;
#endif
        }

        #endregion
    }

    #region Helper Class

    /// <summary>
    /// This class is for RegressionTest8 as the ItemSource of a ItemsControl and change this collection will raise a CollectionChanged Event for ItemsControl to handle.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class MyRangeCollectionView<T> : CollectionView
    {

        private readonly List<int> _mapping;

        public MyRangeCollectionView(IList<T> collection)
            : base(collection)
        {
            _mapping = Enumerable.Range(0, collection.Count).ToList();
        }

        public override object GetItemAt(int index)
        {
            return base.GetItemAt(_mapping[index]);
        }

        // Raise event with range action.
        public void RaiseCollectionChangedEvent()
        {
            var movedItems = new List<object>(2);

            for (int i = 2 - 1; i >= 0; i--)
            {
                var lastIndex = _mapping.Count - 1;
                var value = _mapping[lastIndex];

                movedItems.Insert(0, GetItemAt(value));

                _mapping.RemoveAt(lastIndex);
                _mapping.Insert(0, value);
            }

            this.OnCollectionChanged(new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Move, movedItems, 0, this.Count - 2));
        }

    }

    #endregion
}

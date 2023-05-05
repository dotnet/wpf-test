// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug where a move is reported as a replace to the same location.
    /// </description>
    /// <relatedBugs>


    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ListCollectionViewMoveReplace")]
    public class ListCollectionViewMoveReplace : AvalonTest
    {
        #region Private Data

        ObservableCollection<CollectionItem> _items = new ObservableCollection<CollectionItem>();
        ICollectionView _view;

        const int TargetItemNumber = 0;
        bool _viewChanged = false;
        bool _itemsChanged = false;

        #endregion

        #region Constructors

        public ListCollectionViewMoveReplace()
        {
            InitializeSteps += new TestStep(PopulateCollection);
            RunSteps += new TestStep(CreateView);
            RunSteps += new TestStep(ApplyFilter);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult PopulateCollection()
        {
            for (int itemNumber = 0; itemNumber < 10; itemNumber++)
            {
                for (int subItemNumber = 0; subItemNumber < 3; subItemNumber++)
                {
                    var item = new CollectionItem(itemNumber, subItemNumber);
                    this._items.Add(item);
                }
            }

            return TestResult.Pass;
        }

        private TestResult CreateView()
        {
            CollectionViewSource cvs = new CollectionViewSource();
            cvs.Source = this._items;

            this._view = cvs.View;

            return TestResult.Pass;
        }

        private TestResult ApplyFilter()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(this._view);
            view.Filter = delegate(object obj)
            {
                var item = (CollectionItem)obj;
                return item.Number.Equals(TargetItemNumber);
            };

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            this._view.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(view_CollectionChanged);
            this._items.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(items_CollectionChanged);

            
            int foundCount = 0;

            this._viewChanged = false;
            this._itemsChanged = false;

            for (int index = 0; index < this._items.Count; index++)
            {
                if (this._items[index].Number == TargetItemNumber)
                {
                    foundCount++;

                    if (foundCount == 2)
                    {
                        this._items.Move(index, 0);
                        break;
                    }
                }
            }

            Debug.Assert(this._itemsChanged == true && (this._itemsChanged == this._viewChanged));
            return TestResult.Pass;
            
        }

        #endregion


        #region Helper Methods

        void items_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this._itemsChanged = true;
        }

        void view_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            this._viewChanged = true;

            if (e.NewStartingIndex == e.OldStartingIndex &&
                Object.ReferenceEquals(e.NewItems[0], e.OldItems[0]))
            {
                throw new TestValidationException("Item reported as moved to same place");
            }
        }

        #endregion
    }

    #region Helper Class

    class CollectionItem
    {
        int _subNumber;

        public CollectionItem(int number, int subNumber)
        {
            this.Number = number;
            this._subNumber = subNumber;
        }

        public int Number
        {
            get;
            private set;
        }
    }

    #endregion

}

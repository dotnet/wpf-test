// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using System;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Diagnostics;

namespace Microsoft.Test.DataServices
{

    #region CollectionViewCCTestBase

    public abstract class CollectionViewCCTestBase : XamlTest
    {

        protected ListBox lb;
        protected CollectionView cv;
        protected IDisposable disp;
        protected Predicate<object> cfc;
        protected object item1;
        protected object item2;

        public CollectionViewCCTestBase(string filename) : base(filename)
        {
            InitializeSteps += new TestStep(SetUp);
            InitializeSteps += new TestStep(SetUpVerify);
            RunSteps += new TestStep(MoveFirst);
            RunSteps += new TestStep(CurrentPositionVerify);
            RunSteps += new TestStep(Filter);
            RunSteps += new TestStep(FilterVerify);
            RunSteps += new TestStep(SortVerify);

            #region Throwing for access during DeferRefresh

            RunSteps += new TestStep(ThrowOnCurrentPosition);
			RunSteps += new TestStep(ThrowOnCount);
			RunSteps += new TestStep(ThrowOnContains);
			RunSteps += new TestStep(ThrowOnCurrentItem);
			RunSteps += new TestStep(ThrowOnMoveLast);
			RunSteps += new TestStep(ThrowOnMovePrevious);
			RunSteps += new TestStep(ThrowOnMoveNext);
			RunSteps += new TestStep(ThrowOnMoveTo);
			RunSteps += new TestStep(ThrowOnMoveFirst);
			RunSteps += new TestStep(ThrowOnMoveToPosition);
			RunSteps += new TestStep(ThrowOnIsCurrentAfterLast);
			RunSteps += new TestStep(ThrowOnIsCurrentBeforeFirst);

            #endregion

        }

        #region virtual protected functions

        virtual protected TestResult SetUp()
        {
            LogComment("SetUp was not overridden");
            return TestResult.Fail;
        }


        protected virtual TestResult ICollectionVerify()
        {
            Status("Check ICollection properties");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            ICollection coll = cv as ICollection;
            if (coll == null)
            {
                LogComment("Could not cast to an ICollection");
                return TestResult.Fail;
            }

            if (coll.IsSynchronized)
            {
                LogComment("The collection is syncronized, it should not be");
                return TestResult.Fail;
            }

            if (coll.SyncRoot == null)
            {
                LogComment("The collection SyncRoot is null, it should not be");
                return TestResult.Fail;
            }

            LogComment("Checked ICollection properties");

            return TestResult.Pass;
        }

        protected virtual TestResult IListVerify()
        {
            Status("Check IList properties");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            IList list = cv as IList;
            if (list == null)
            {
                LogComment("Could not cast to an IList");
                return TestResult.Fail;
            }

            if (list.IsFixedSize)
            {
                LogComment("The List is a fixed size, it should not be");
                return TestResult.Fail;
            }

            if (list.IsReadOnly)
            {
                LogComment("The List is readonly, it should not be");
                return TestResult.Fail;
            }

            LogComment("Checked IList properties");

            return TestResult.Pass;
        }

        virtual protected TestResult SetUpVerify()
        {
            Status("Verifying setup");

            if (lb == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }

            if (cv == null)
            {
                LogComment("ItemCollection is null");
                return TestResult.Fail;
            }

            if (item1 == null)
            {
                LogComment("item1 is null");
                return TestResult.Fail;
            }

            if (item2 == null)
            {
                LogComment("item2 is null");
                return TestResult.Fail;
            }

            if (cfc == null)
            {
                LogComment("CollectionFilterCallback is null");
                return TestResult.Fail;
            }

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            return TestResult.Pass;
        }

        virtual protected TestResult MoveFirst()
        {
            Status("Move First");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            cv.MoveCurrentToFirst();

            return TestResult.Pass;
        }

        virtual protected TestResult CurrentPositionVerify()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            if (cv.CurrentPosition != 0)
            {
                LogComment("Current Postition was " + cv.CurrentPosition + " expected 0");
                return TestResult.Fail;
            }

            LogComment("The CurrentPosition was at the correct position");
            return TestResult.Pass;
        }

        virtual protected TestResult Filter()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            cv.Filter = cfc;
            LogComment("Filter applied");

            return TestResult.Pass;
        }

        virtual protected TestResult FilterVerify()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            if (cfc != cv.Filter)
            {
                LogComment("Expected filter was not applied");
                return TestResult.Fail;
            }

            LogComment("Filters match");
            return TestResult.Pass;
        }

        virtual protected TestResult SortVerify()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            IList<SortDescription> sd = cv.SortDescriptions;

            if (sd.Count != 0)
            {
                LogComment("Expected 0 sort desriptions for the Sort");
                return TestResult.Fail;
            }

            LogComment("Correct number of sort descriptions");
            return TestResult.Pass;
        }

        #region Throwing for DeferRefresh

        virtual protected TestResult ThrowOnCurrentPosition()
        {
            LogComment("Attempting to access CurrentPosition");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				int i = cv.CurrentPosition;
			}
			LogComment("Current Postition did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnContains()
        {
            LogComment("Attempting to access Contains");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				bool inIt = cv.Contains(item1);
			}
            LogComment("Contains did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnCount()
        {
			LogComment("Attempting to access Count");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				int i = cv.Count;
			}
            LogComment("Count did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnCurrentItem()
        {
			LogComment("Attempting to access CurrentItem");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				object o = cv.CurrentItem;
			}
            LogComment("CurrentItem did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMoveLast()
        {
            LogComment("Attempting to access MoveCurrentToLast");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentToLast();
			}
            LogComment("MoveCurrentToLast did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMovePrevious()
        {
            LogComment("Attempting to access MoveCurrentToPrevious");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentToPrevious();
			}
            LogComment("MoveCurrentToPrevious did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMoveNext()
        {
            LogComment("Attempting to access MoveCurrentToNext");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentToNext();
			}
			 LogComment("MoveCurrentToNext did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMoveTo()
        {
			LogComment("Attempting to access MoveCurrentTo");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentTo(item2);
			}
            LogComment("MoveCurrentTo did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMoveFirst()
        {
            LogComment("Attempting to access MoveCurrentToFirst");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentToFirst();
			}
            LogComment("MoveCurrentToFirst did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnMoveToPosition()
        {
            LogComment("Attempting to access MoveCurrentToPosition");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				cv.MoveCurrentToPosition(0);
			}
            LogComment("MoveCurrentToPosition did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnIsCurrentAfterLast()
        {
            LogComment("Attempting to access IsCurrentAfterLast");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				bool b = cv.IsCurrentAfterLast;
			}
            LogComment("IsCurrentAfterLast did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        virtual protected TestResult ThrowOnIsCurrentBeforeFirst()
        {
            LogComment("Attempting to access IsCurrentBeforeFirst");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
			using (cv.DeferRefresh())
			{
				bool b = cv.IsCurrentBeforeFirst;
			}
            LogComment("IsCurrentBeforeFirst did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        #endregion

        #endregion

    }

    #endregion

    #region CollectionViewCCTest

    /// <summary>
    /// <description>
    /// Helps code coverage for CollectionView. Verifies it throws when doing certain operation inside a
    /// DeferRefresh.
    /// </description>
    /// </summary>

    [Test(2, "Views", "CollectionViewCCTest")]
    public class CollectionViewCCTest : CollectionViewCCTestBase
    {
        public CollectionViewCCTest() : base("CollectionIEnumerableViews.xaml") { }

        protected override TestResult SetUp()
        {
            Status("Referencing Listbox");

            lb = (ListBox)Util.FindElement(RootElement, "testListBox");
            if (lb == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }

            cv = CollectionViewSource.GetDefaultView(lb.ItemsSource) as CollectionView;

            if (cv == null)
            {
                LogComment("ItemCollection is null");
                return TestResult.Fail;
            }

            if (SystemInformation.WpfVersion == WpfVersions.Wpf30)
            {
                if (cv.GetType() != typeof(System.Windows.Data.CollectionView))
                {
                    LogComment("Expected type CollectionView, but actual was " + cv.GetType().FullName);
                    return TestResult.Fail;
                }
            }
            else
            {
                if (cv.GetType().ToString() != "MS.Internal.Data.EnumerableCollectionView")
                {
                    LogComment("Expected type EnumerableCollectionView, but actual was " + cv.GetType().FullName);
                    return TestResult.Fail;
                }
            }

            cfc = new Predicate<object>(MyFilter);

            ObjectDataProvider dso = RootElement.Resources["DSO"] as ObjectDataProvider;
            if (dso == null)
            {
                LogComment("Datasource was null");
                return TestResult.Fail;
            }

            LibraryEnumerable l = dso.Data as LibraryEnumerable;
            IEnumerator ie = l.GetEnumerator();
            while (ie.MoveNext())
            {
                if (item1 != null && item2 == null)
                    item2 = ie.Current;
                if (item1 == null)
                    item1 = ie.Current;
                if (item1 != null && item2 != null)
                    break;
            }

            ie.Reset();

            return TestResult.Pass;
        }

        bool MyFilter(object item)
        {
            Book b = item as Book;
            if (b != null)
            {
                return (b.Price > 30.0);
            }

            return false;

        }
    }

    #endregion

    #region ListCollectionViewCCTest

    /// <summary>
    /// <description>
    /// Helps code coverage for ListCollectionView. Verifies it throws when doing certain operation inside a
    /// DeferRefresh.
    /// </description>
    /// </summary>

    [Test(2, "Views", "ListCollectionViewCCTest")]
    public class ListCollectionViewCCTest : CollectionViewCCTestBase
    {
        ListCollectionView _lcv;

        public ListCollectionViewCCTest()
            : base("CollectionALDCViews.xaml")
        {
//            RunSteps += new TestStep(ICollectionVerify);
//            RunSteps += new TestStep(IListVerify);
        }

        protected override TestResult SetUp()
        {
            Status("Referencing Listbox");

            lb = (ListBox)Util.FindElement(RootElement, "testListBox");
            if (lb == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }

            cv = CollectionViewSource.GetDefaultView(lb.ItemsSource) as CollectionView;

            if (cv == null)
            {
                LogComment("ItemCollection is null");
                return TestResult.Fail;
            }

            if (cv.GetType() != typeof(System.Windows.Data.ListCollectionView))
            {
                LogComment("Expected type ListCollectionView, but actual was " + cv.GetType().FullName);
                return TestResult.Fail;
            }
            _lcv = cv as ListCollectionView;
            if (_lcv == null)
            {
                LogComment("Could not cast CollectionView to ListCollectionView");
                return TestResult.Fail;
            }

            cfc = new Predicate<object>(MyFilter);

            ObjectDataProvider dso = RootElement.Resources["DSO"] as ObjectDataProvider;
            if (dso == null)
            {
                LogComment("Datasource was null");
                return TestResult.Fail;
            }

            Library l = dso.Data as Library;
            item1 = l[0];
            item2 = l[1];

            return TestResult.Pass;
        }

        protected override TestResult IListVerify()
        {
            Status("Check IList properties");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            IList list = _lcv as IList;
            if (list == null)
            {
                LogComment("Could not cast to an IList");
                return TestResult.Fail;
            }

            if (!list.IsFixedSize)
            {
                LogComment("The List is not a fixed size, it should be");
                return TestResult.Fail;
            }

            if (!list.IsReadOnly)
            {
                LogComment("The List is not readonly, it should be");
                return TestResult.Fail;
            }

            LogComment("Checked IList properties");

            return TestResult.Pass;
        }


        TestResult CopyTo()
        {
            Status("Testing CopyTo");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            IList list = _lcv as IList;
            if (list == null)
            {
                LogComment("Could not cast to IList");
                return TestResult.Fail;
            }
            Array mylist = Array.CreateInstance(typeof(object), list.Count);
            list.CopyTo(mylist, 0);

            return TestResult.Pass;
        }

        TestResult ThrowOnGetItem()
        {
            LogComment("Attempting to access getitem using index ");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            object o = _lcv.GetItemAt(0);
            LogComment("getitem using index did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnClear()
        {
            LogComment("Attempting to Clear");
            IList list = _lcv as IList;
            if (list == null)
            {
                LogComment("Could not cast to IList");
                return TestResult.Fail;
            }

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            list.Clear();

            LogComment("Clear did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        bool MyFilter(object item)
        {
            Book b = item as Book;
            if (b != null)
            {
                return (b.Price > 30.0);
            }

            return false;

        }
    }

    #endregion

    #region ItemCollectionCCTest

    /// <summary>
    /// <description>
    /// Helps code coverage for ItemCollection. Verifies it throws when doing certain operation inside a
    /// DeferRefresh.
    /// </description>
    /// </summary>

    [Test(2, "Views", "ItemCollectionCCTest")]
    public class ItemCollectionCCTest : CollectionViewCCTestBase
    {
        ItemCollection _ic;

        public ItemCollectionCCTest()
            : base("BlankViews.xaml")
        {
            RunSteps += new TestStep(ICollectionVerify);
            RunSteps += new TestStep(IListVerify);
        }

        protected override TestResult SetUp()
        {

            lb = new ListBox();
            Panel panel = RootElement as Panel;

            panel.Children.Add(lb);

            cv = lb.Items as ItemCollection;

            if (cv == null)
            {
                LogComment("ItemCollection is null");
                return TestResult.Fail;
            }

            TextBox tbx = new TextBox();
            tbx.Text = "Hello World";

            item1 = tbx;

            TextBlock tbl = new TextBlock();
            tbl.Text = "From Mars";

            item2 = tbl;

            if (cv.GetType() != typeof(System.Windows.Controls.ItemCollection))
            {
                LogComment("Expected type ItemCollection, but actual was " + cv.GetType().FullName);
                return TestResult.Fail;
            }
            else
            {
                _ic = cv as ItemCollection;
                if (_ic == null)
                {
                    LogComment("Could not cast CollectionView to an ItemCollection");
                }
                _ic.Add(item1);
                _ic.Add(item2);

                cfc = new Predicate<object>(MyFilter);

                return TestResult.Pass;
            }

        }

        protected override TestResult ICollectionVerify()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            ICollection coll = cv as ICollection;
            if (coll == null)
            {
                LogComment("Could not cast to an ICollection");
                return TestResult.Fail;
            }

            if (coll.IsSynchronized)
            {
                LogComment("The collection is syncronized, it should not be");
                return TestResult.Fail;
            }

            LogComment("ICollection.IsSynchronized is correct");

            Status("Attempting to access the SyncRoot");
            object syncroot = coll.SyncRoot;

            return TestResult.Pass;
        }

        protected override TestResult IListVerify()
        {
            Status("Check IList properties");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            IList list = _ic as IList;
            if (list == null)
            {
                LogComment("Could not cast to an IList");
                return TestResult.Fail;
            }
            // FixedSize and ReadOnly should be false with filter
            if (list.IsFixedSize)
            {
                LogComment("The List should not be FixedSize, but it is!");
                return TestResult.Fail;
            }

            if (list.IsReadOnly)
            {
                LogComment("The List should not be ReadOnly, but it is!");
                return TestResult.Fail;
            }

            LogComment("Checked IList properties");

            return TestResult.Pass;
        }


        TestResult CopyTo()
        {
            Status("Testing CopyTo");
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            Array mylist = Array.CreateInstance(typeof(object), _ic.Count);

            _ic.CopyTo(mylist, 0);

            return TestResult.Pass;
        }

        TestResult ThrowOnGetItem()
        {
            LogComment("Attempting to access getitem using index ");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            object o = _ic[0];
            LogComment("getitem using index did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnClear()
        {
            LogComment("Attempting to Clear");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _ic.Clear();
            LogComment("Clear did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        public bool MyFilter(object item)
        {
            return (item is TextBox);
        }

    }

    #endregion

}

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
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Test ItemCollection collection PME to improve code coverage
    /// </description>
    /// </summary>
    [Test(3, "Views", "ItemCollectionTest")]
    public class ItemCollectionTest : WindowTest
    {

        ListBox _lb;
        ItemCollection _ic;
        Array _mylist;
        TextBox _tb;
        TextBlock _tb2;
        Predicate<object> _cfc;

        public ItemCollectionTest()
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(AddingItems);
            RunSteps += new TestStep(CopyTo);
            RunSteps += new TestStep(CopyToVerify);
            RunSteps += new TestStep(MoveFirst);
            RunSteps += new TestStep(InterfaceProperties);
            RunSteps += new TestStep(CurrentPositionVerify);
            RunSteps += new TestStep(Filter);
            RunSteps += new TestStep(FilterVerify);
            RunSteps += new TestStep(SortVerify);
           // This is now supported
           // RunSteps += new TestStep(SyncRootThrow);

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
            RunSteps += new TestStep(ThrowOnGetItem);
            RunSteps += new TestStep(ThrowOnClear);


            #endregion

        }

        TestResult SetUp()
        {
            Status("Creating Listbox");
            _lb = new ListBox();
            Page p = new Page();
            DockPanel panel = new DockPanel();

            p.Content = panel;
            panel.Children.Add(_lb);
            Window.Content = p;

            _ic = _lb.Items as ItemCollection;

            if (_ic == null)
            {
                LogComment("ItemCollection is null");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult AddingItems()
        {
            _tb = new TextBox();
            _tb.Text = "Hello World";

            _ic.Add(_tb);

            _tb2 = new TextBlock();
            _tb2.Text = "From Mars";

            _ic.Add(_tb2);

            return TestResult.Pass;
        }

        TestResult CopyTo()
        {
            Status("Testing CopyTo");
            _mylist = Array.CreateInstance(typeof(object), _ic.Count);

            _ic.CopyTo(_mylist, 0);

            return TestResult.Pass;
        }

        TestResult CopyToVerify()
        {
            if (_mylist.Length != 2)
            {
                LogComment("Expected 2 items in the copied array, acutal " + _mylist.Length.ToString());
            }
            LogComment("Correct number of items in the copied array");
            return TestResult.Pass;
        }

        TestResult MoveFirst()
        {
            Status("Move First");

            _ic.MoveCurrentToFirst();

            return TestResult.Pass;
        }

        TestResult InterfaceProperties()
        {
            Status("Check ICollection properties, IList properties, and remove handler");

            ICollection coll = _ic as ICollection;
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

            IList list = _ic as IList;
            if (list == null)
            {
                LogComment("Could not cast to an IList");
                return TestResult.Fail;
            }

            if (list.IsFixedSize)
            {
                LogComment("The Collection is a fixed size, it should not be");
                return TestResult.Fail;
            }

            if (list.IsReadOnly)
            {
                LogComment("The Collection is readonly, it should not be");
                return TestResult.Fail;
            }

            LogComment("Checked ICollection properties, IList properties, removed handler");

            return TestResult.Pass;
        }

        TestResult CurrentPositionVerify()
        {
            if (_ic.CurrentPosition != 0)
            {
                LogComment("Current Postition was " + _ic.CurrentPosition + " expected 0");
                return TestResult.Fail;
            }

            LogComment("The CurrentPosition was at the correct position");
            return TestResult.Pass;
        }

        TestResult Filter()
        {
            _cfc = new Predicate<object>(MyFilter);
            _ic.Filter = _cfc;
            LogComment("Filter applied");

            return TestResult.Pass;
        }

        TestResult FilterVerify()
        {
            if (_cfc != _ic.Filter)
            {
                LogComment("Expected filter was not applied");
                return TestResult.Fail;
            }

            LogComment("Filters match");
            return TestResult.Pass;
        }

        TestResult SortVerify()
        {
            IList<SortDescription> sd = _ic.SortDescriptions;

            if (sd.Count != 0)
            {
                LogComment("Expected 0 sort desriptions for the Sort");
                return TestResult.Fail;
            }

            LogComment("Correct number of sort descriptions");
            return TestResult.Pass;
        }


        TestResult SyncRootThrow()
        {
            ICollection coll = _ic as ICollection;
            if (coll == null)
            {
                LogComment("Could not cast to an ICollection");
                return TestResult.Fail;
            }

            SetExpectedErrorTypeInStep(typeof(NotSupportedException));

            object syncroot = coll.SyncRoot;

            LogComment("Expected NotSupportedException when accessing the SyncRoot");
            return TestResult.Fail;
        }

        #region Throwing for DeferRefresh


        TestResult ThrowOnCurrentPosition()
        {
            LogComment("Attempting to access CurrentPosition");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
            	int i = _ic.CurrentPosition;
            }
            	LogComment("Current Postition did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnContains()
        {
            LogComment("Attempting to access Contains");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                bool inIt = _ic.Contains(_tb);
            }
            LogComment("Contains did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnCount()
        {
            LogComment("Attempting to access Contains");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                int i = _ic.Count;
            }
            LogComment("Count did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnCurrentItem()
        {
            LogComment("Attempting to access Contains");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                object o = _ic.CurrentItem;
            }
            LogComment("CurrentItem did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMoveLast()
        {
            LogComment("Attempting to access MoveCurrentToLast");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentToLast();
            }
            LogComment("MoveCurrentToLast did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMovePrevious()
        {
            LogComment("Attempting to access MoveCurrentToPrevious");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentToPrevious();
            }
            LogComment("MoveCurrentToPrevious did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMoveNext()
        {
            LogComment("Attempting to access MoveCurrentToNext");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentToNext();
            }
            LogComment("MoveCurrentToNext did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMoveTo()
        {
            LogComment("Attempting to access MoveCurrentTo");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentTo(_tb2);
            }
            LogComment("MoveCurrentTo did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMoveFirst()
        {
            LogComment("Attempting to access MoveCurrentToFirst");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentToFirst();
            }
            LogComment("MoveCurrentToFirst did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnMoveToPosition()
        {
            LogComment("Attempting to access MoveCurrentToPosition");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                _ic.MoveCurrentToPosition(0);
            }
            LogComment("MoveCurrentToPosition did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnIsCurrentAfterLast()
        {
            LogComment("Attempting to access IsCurrentAfterLast");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                bool b = _ic.IsCurrentAfterLast;
            }
            LogComment("IsCurrentAfterLast did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnIsCurrentBeforeFirst()
        {
            LogComment("Attempting to access IsCurrentBeforeFirst");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                bool b = _ic.IsCurrentBeforeFirst;
            }
            LogComment("IsCurrentBeforeFirst did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnGetItem()
        {
            LogComment("Attempting to access getitem using index ");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
                object o = _ic[0];
            }
            LogComment("getitem using index did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        TestResult ThrowOnClear()
        {
            LogComment("Attempting to Clear");
            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
	    using (_ic.DeferRefresh())
            {
               _ic.Clear();
            }
            LogComment("Clear did not throw InvalidOperationException");
            return TestResult.Fail;
        }

        #endregion


        #region helpers

        public bool MyFilter(object item)
        {
            return (item is TextBox);
        }

        #endregion

    }
}

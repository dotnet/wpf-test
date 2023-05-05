// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This tests sorting, currency and filtering using a ListCollectionView. The data source is
    /// an ArrayList.
    /// </description>
    /// </summary>
    [Test(0, "Views", "ListCollectionViewTest")]
    public class ListCollectionViewTest : AvalonTest
    {
        private ListCollectionView _lcv;
        private ArrayList _ar;
        private SuperFilter _filter;
        private CurrentChangingVerifier _ccgv;
        private CurrentChangedVerifier _ccdv;

        public ListCollectionViewTest ()
        {
            InitializeSteps += new TestStep (Init);
            RunSteps += new TestStep (lvcSortTest);
            RunSteps += new TestStep (lvcRemoveEventHandler);
            RunSteps +=new TestStep(lvcFilterTest);
            RunSteps += new TestStep (lcvEnumerate);
        }

        private TestResult Init()
        {
            Status("Init");
            _ar = new ArrayList ();
            _lcv = (ListCollectionView) CollectionViewSource.GetDefaultView(_ar);
            _ar.Add ("First");
            _ar.Add ("Second");
            _ar.Add ("Third");
            _ar.Add ("Fourth");
            _ar.Add ("Fifth");
            return TestResult.Pass;
        }

        private TestResult lvcSortTest ()
        {
            Status("lvcSortTest");

            CollectionViewOrderVerifier cvov = new CollectionViewOrderVerifier(_lcv);

            //Verifying Event handlers are hooked up
            _ccgv = new CurrentChangingVerifier(_lcv);
            _ccdv = new CurrentChangedVerifier(_lcv);
            // Getting current item before refresh

            _lcv.CustomSort = new SuperSort (ListSortDirection.Ascending);

            //Validating Event Handlers
            IVerifyResult ivr1 = _ccgv.Verify(_lcv, 1, _lcv.CurrentItem);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                return ivr1.Result;
            }
			IVerifyResult ivr2 = _ccdv.Verify(_lcv, 1, _lcv.CurrentItem);
			if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                return ivr2.Result;
            }
            //checking to see that everything sorted correctly
            IVerifyResult ivr3 = cvov.Verify(4, 0, 3, 1, 2);
            if (ivr3.Result != TestResult.Pass)
            {
                LogComment(ivr3.Message);
                return ivr3.Result;
            }

            LogComment("lvcSortTest was successful");
            return TestResult.Pass;
        }

        private TestResult lvcRemoveEventHandler()
        {
            Status("lvcRemoveEventHandler");

			string current = _lcv.CurrentItem as string;
			_ccgv.RemoveEventHandler(_lcv);
            _ccdv.RemoveEventHandler(_lcv);
            _lcv.MoveCurrentToFirst();

			IVerifyResult ivr1 = _ccgv.Verify(_lcv, 0, current);
			if (ivr1.Result != TestResult.Pass)
			{
				LogComment(ivr1.Message);
				return ivr1.Result;
			}
			IVerifyResult ivr2 = _ccdv.Verify(_lcv, 0, current);
			if (ivr2.Result != TestResult.Pass)
			{
				LogComment(ivr2.Message);
				return ivr2.Result;
			}

			LogComment("lvcRemoveEventHandler was successful");
            return TestResult.Pass;
        }

        private TestResult lvcFilterTest()
        {
            Status("lvcFilterTest");
            //Making CurrentItem the item that will be filtered out
            _lcv.MoveCurrentTo ("Third");
            //Verifying Navigation
            if (_lcv.IndexOf (_lcv.CurrentItem) != 4)
            {
                LogComment ("Fail - Wasn't able to MoveTo() object ");
                return TestResult.Fail;
            }
            CollectionViewOrderVerifier cvov = new CollectionViewOrderVerifier(_lcv);
            this._filter = new SuperFilter("<", "Third");
            _lcv.Filter = new Predicate<object>(this._filter.Contains);
            //CurrentItem should be filtered out, first item in the collection should be selected
            if (_lcv.IndexOf(_lcv.CurrentItem) != 0)
            {
                LogComment ("Fail - CurrentItem wasn't set to the first item in the collection");
                return TestResult.Fail;
            }
            if (_lcv.Contains ("Third"))
            {
                LogComment ("Fail - ListViewCollection contains the Object that should have been filtered out");
                return TestResult.Fail;
            }
            //Verifying the filtering
            IVerifyResult ivr = cvov.Verify(4, 0, 3, 1);
            if (ivr.Result != TestResult.Pass)
            {
                LogComment(ivr.Message);
                return ivr.Result;
            }
            LogComment("lvcFilterTest was successful");
            return TestResult.Pass;
        }

        private TestResult lcvEnumerate()
        {
            Status("lcvEnumerate");
            int counter = 0;
            int[] i = { 4, 0, 3, 1 };

            foreach (string a in _lcv)
            {
                if (a != (string)_ar[i[counter++]])
                {
                    LogComment("Fail - There is something wrong with GetEnumerator");
                    return TestResult.Fail;
                }
            }

            LogComment("lcvEnumerate was successful");
            return TestResult.Pass;
        }
    }
}

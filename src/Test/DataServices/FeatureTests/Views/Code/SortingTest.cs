// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This tests sorting an ObservableCollection using collectionView.Sort
	/// and listCollectionView.CustomSort.
	/// </description>
	/// </summary>
    [Test(0, "Views", "SortingTest")]
	public class SortingTest : AvalonTest
	{
        ObservableCollection<CLRBook> _books;
        CLRBook _item;
		ICollectionView _cv;
		CollectionViewOrderVerifier _cvov;
		CurrentChangingVerifier _ccv;
		CurrentChangedVerifier _ccdv;

        public SortingTest()
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(Sort);
            RunSteps += new TestStep(CustomSort);
        }

        private TestResult Init()
        {
            Status ("Initilizing my Collection");
            _books = new ObservableCollection<CLRBook>();
            _item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.92, BookType.Novel);
            _books.Add (_item);
            _item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.91, BookType.Novel);
            _books.Add (_item);
            _item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
            _books.Add (_item);
            _item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
            _books.Add (_item);
            _cv = CollectionViewSource.GetDefaultView(_books);
            _cvov = new CollectionViewOrderVerifier(_cv);
            return TestResult.Pass;
        }

		private TestResult Sort()
		{
			Status("Sort");
			if (_cv.CanSort)
			{
                CLRBook book = _cv.CurrentItem as CLRBook; // this is Homo Faber, Max Frish
				_ccv = new CurrentChangingVerifier(_cv);
				_ccdv = new CurrentChangedVerifier(_cv);

                using(_cv.DeferRefresh())
                {
                    _cv.SortDescriptions.Clear();
                    _cv.SortDescriptions.Add(new SortDescription ("BookType", ListSortDirection.Ascending));
                    _cv.SortDescriptions.Add(new SortDescription ("Price", ListSortDirection.Ascending));
                }

				if (_cv.SortDescriptions.Count != 2)
				{
					LogComment("Fail - SortDescriptions not set correctly");
					return TestResult.Fail;
				}

				IVerifyResult ivr1 = _ccv.Verify(_cv, 1, book); // it fails here, the local sender is null
				if (ivr1.Result != TestResult.Pass)
				{
					LogComment(ivr1.Message);
					return ivr1.Result;
				}

				IVerifyResult ivr2 = _ccdv.Verify(_cv, 1, book);
				if (ivr2.Result != TestResult.Pass)
				{
					LogComment(ivr2.Message);
					return ivr2.Result;
				}

				IVerifyResult ivr3 = _cvov.Verify(3, 1, 0, 2);
				if (ivr3.Result != TestResult.Pass)
				{
					LogComment(ivr3.Message);
					return ivr3.Result;
				}
				// Should the current item be the one in the third position after the sort? Or
				// should it go back to the first?
			}
			LogComment("Sort was successful");
			return TestResult.Pass;
		}

        private TestResult CustomSort()
        {
            Status("CustomSort");
            ListCollectionView lcv = _cv as ListCollectionView;
            MySort ms = new MySort(typeof(CLRBook), "Title");

            lcv.CustomSort = ms;

			if (lcv.CustomSort != ms)
			{
				LogComment("Fail - CustomSort was not properly set");
				return TestResult.Fail;
			}

			IVerifyResult ivr = _cvov.Verify(3, 0, 2, 1);
			if (ivr.Result != TestResult.Pass)
			{
				LogComment(ivr.Message);
				return ivr.Result;
			}

			LogComment("CustomSort was successful");
			return TestResult.Pass;
		}
	}
}



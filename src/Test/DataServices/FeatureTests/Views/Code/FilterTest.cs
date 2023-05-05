// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using System;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This tests basic filtering.
	/// </description>
	/// </summary>
    [Test(0, "Views", "FilterTest")]
	public class FilterTest : AvalonTest
	{
        ObservableCollection<CLRBook> _books;
        CLRBook _item;
		ICollectionView _cv;
		CollectionViewOrderVerifier _cvov;
		private MyFilter _myFilter;

        public FilterTest ()
        {
            InitializeSteps += new TestStep (Init);
            RunSteps += new TestStep (CustomFilter);
        }

        private TestResult Init ()
        {
            Status ("Initilizing my Collection");
            _books = new ObservableCollection<CLRBook>();
            _item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.99, BookType.Novel);
            _books.Add (_item);
            _item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.95, BookType.Novel);
            _books.Add (_item);
            _item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
            _books.Add (_item);
            _item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
            _books.Add (_item);
            _cv = CollectionViewSource.GetDefaultView(_books);
            _cvov = new CollectionViewOrderVerifier(_cv);
            return TestResult.Pass;
        }

        TestResult CustomFilter ()
        {
            Status("CustomFilter");

			ListCollectionView lcv = _cv as ListCollectionView;

			if (lcv.CanFilter)
			{
				this._myFilter = new MyFilter(BookType.Novel);
				Predicate<object> filterCallback = new Predicate<object>(_myFilter.Contains);
				lcv.Filter = filterCallback;

				if (lcv.Filter != filterCallback)
				{
					LogComment("Fail - Filter was not properly set");
					return TestResult.Fail;
				}

				IVerifyResult ivr = _cvov.Verify(0, 1, 3);
				if (ivr.Result != TestResult.Pass)
				{
					LogComment(ivr.Message);
					return ivr.Result;
				}
			}
			LogComment("CustomFilter was successful");
			return TestResult.Pass;
		}
	}
}



// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Threading;
using System.Globalization;
using System.Collections;
using System.Windows.Controls;
using Microsoft.Test;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This is a get/set test for System.Collections.Generic.IList&lt;BindingExpression&gt;
	/// </description>
	/// </summary>
    [Test(1, "Binding", "BindingListTest")]
	public class BindingListTest : WindowTest
	{
        ObservableCollection<CLRBook> _verifyArrayList;
        Binding _bAuthor;
		System.Collections.Generic.IList<BindingExpressionBase> _pBingList;
		BindingExpressionBase[] _bindingArray;
		public BindingListTest()
		{
		//	RunSteps += new TestStep(Test);

			InitializeSteps += new TestStep(CreateTest);
			RunSteps += new TestStep(CopyToList);
			RunSteps += new TestStep(AddToList);
			RunSteps += new TestStep(RemoveFromList);
//			RunSteps += new TestStep(IListRemoveFromList);
			RunSteps += new TestStep(IndexOfObjInList);
			RunSteps += new TestStep(ContainsInList);
			RunSteps += new TestStep(InsertInToList);
			RunSteps += new TestStep(CheckProperties);
			RunSteps += new TestStep(AssignTo);
			RunSteps += new TestStep(ClearList);

		}
        private ObservableCollection<CLRBook> CreateData()
        {
            ObservableCollection<CLRBook> books = new ObservableCollection<CLRBook>();
            CLRBook item = new CLRBook("Homo Faber", "Max Frisch", 1957, 14.92, BookType.Novel);
			books.Add(item);
			item = new CLRBook("The Fourth Hand", "John Irving", 2001, 14.91, BookType.Novel);
			books.Add(item);
			item = new CLRBook("Inside C#", "Tom Archer e.a.", 2002, 49.99, BookType.Reference);
			books.Add(item);
			item = new CLRBook("A Man in Full", "Tom Wolfe", 1998, 8.95, BookType.Novel);
			books.Add(item);


			return books;
		}
		private TestResult CreateTest()
		{
			_verifyArrayList = CreateData();

			//creating a prioritybind
			TextBox tb1 = new TextBox();
			System.Windows.Data.PriorityBinding pb = new System.Windows.Data.PriorityBinding();
			Binding bBadBind = new Binding("badbind");
			pb.Bindings.Add(bBadBind);
			_bAuthor = new Binding("Author");

			pb.Bindings.Add(_bAuthor);
			tb1.SetBinding(TextBox.TextProperty, pb);
			PriorityBindingExpression pBing = BindingOperations.GetPriorityBindingExpression(tb1, TextBox.TextProperty) as PriorityBindingExpression;
			_pBingList = pBing.BindingExpressions;


			DockPanel dp = new DockPanel();
			dp.DataContext = CreateData();
			dp.Children.Add(tb1);
			Window.Content = dp;
			return TestResult.Pass;
		}
		private TestResult CopyToList()
		{
			Status("CopyToList");
			_bindingArray = new BindingExpression[4];
			((IList)_pBingList).CopyTo(_bindingArray, 1);
			if (_bindingArray[1] != _pBingList[0] || _bindingArray[2] != _pBingList[1] || _bindingArray[0] != null)
			{
				LogComment("CopyTo didn't copy correctly");
				return TestResult.Fail;
			}

			_pBingList.CopyTo(_bindingArray, 0);
			if (_bindingArray[0] != _pBingList[0] || _bindingArray[1] != _pBingList[1] || _bindingArray[2] != _pBingList[1])
			{
				LogComment("CopyTo didn't copy correctly");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult AddToList()
		{
			try
			{
				((IList)_pBingList).Add(_bindingArray[0]);
				LogComment("((IList)bindlist).Add didn't throw");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected error: " + e.Message);
			}

			return TestResult.Pass;
		}
		private TestResult RemoveFromList()
		{
			try
			{
				((IList)_pBingList).Remove(_bindingArray[0]);
				LogComment("((IList)bindlist).Remove didn't throw");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected error: " + e.Message);
			}

			try
			{
				((IList)_pBingList).RemoveAt(0);
				LogComment("((IList)bindlist).RemoveAt didn't throw");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected error: " + e.Message);
			}

			return TestResult.Pass;
		}

		private TestResult IndexOfObjInList()
		{
			if (((IList)_pBingList).IndexOf(null) >= 0)
			{
				LogComment("IndexOf(null) didn't return -1");
				return TestResult.Fail;
			}
			if (((IList)_pBingList).IndexOf(_bindingArray[1]) != 1)
			{
				LogComment("IndexOf(binding) didn't return correctly");
				return TestResult.Fail;
			}
			return TestResult.Pass;
		}
		private TestResult ContainsInList()
		{
			if (((IList)_pBingList).Contains(null))
			{
				LogComment("Binds.Contains() couldn't handle a null value!");
				return TestResult.Fail;
			}


			if (!((IList)_pBingList).Contains(_bindingArray[1]))
			{
				LogComment("Binds.Contains() doesn't contains a value, it should!");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}

		private TestResult InsertInToList()
		{
			try
			{
				((IList)_pBingList).Insert(0, _bindingArray[0]);
				LogComment("Insert should have thrown an error");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected exception: " + e.Message);
			}

			return TestResult.Pass;
		}
		private TestResult CheckProperties()
		{
			if (!((IList)_pBingList).IsFixedSize)
			{
				return TestResult.Fail;
			}
			if (!((IList)_pBingList).IsReadOnly)
			{
				return TestResult.Fail;
			}
			if (((ICollection)_pBingList).IsSynchronized)
			{
				return TestResult.Fail;
			}
			IEnumerator _enumerator = ((ICollection)_pBingList).GetEnumerator() as IEnumerator;
			if (_enumerator == null)
			{
				LogComment("Couldn't GetEnumerator()");
				return TestResult.Fail;
			}
			_enumerator.MoveNext();

			if (((BindingExpression)_enumerator.Current) != _bindingArray[0])
			{
				LogComment("Current Item in enumerator was incorrect");
				return TestResult.Fail;
			}
			return TestResult.Pass;

		}
		private TestResult AssignTo()
		{

			try
			{
				((IList)_pBingList)[0] = null;
				LogComment("Assigning null failed to throw");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected Error message: " + e.Message);
			}

			BindingExpression bb = ((IList)_pBingList)[0] as BindingExpression;
			if (bb != _bindingArray[0])
			{
				LogComment("GetBinding didn't successed");
				return TestResult.Fail;
			}

			return TestResult.Pass;
		}
		private TestResult ClearList()
		{
			try
			{
				((IList)_pBingList).Clear();
				LogComment("Clear should have thrown an error");
				return TestResult.Fail;
			}
			catch (Exception e)
			{
				Status("Expected error: " + e.Message);
			}
			return TestResult.Pass;
		}
	}

}




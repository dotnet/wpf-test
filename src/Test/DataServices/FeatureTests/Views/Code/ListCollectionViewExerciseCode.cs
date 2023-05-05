// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// This test exercises code from ListCollectionView, no real user scenarios.
	/// </description>
	/// </summary>
    [Test(3, "Views", "ListCollectionViewExerciseCode")]
	public class ListCollectionViewExerciseCode : AvalonTest
	{
		private ListCollectionView _lcv;
		private ArrayList _ar;

		public ListCollectionViewExerciseCode()
		{
			InitializeSteps += new TestStep(Setup);
//			RunSteps += new TestStep(TestIList);
//			RunSteps += new TestStep(TestICollection);
			RunSteps += new TestStep(TestIComparer);
			RunSteps += new TestStep(TestContains);
			RunSteps += new TestStep(TestIndexOf);
		}

		private TestResult Setup()
		{
			Status("Setup");
			_ar = new ArrayList();
			_lcv = (ListCollectionView) CollectionViewSource.GetDefaultView(_ar);
			_ar.Add ("First");
			_ar.Add ("Second");
			_ar.Add ("Third");
			_ar.Add ("Fourth");
			_ar.Add ("Fifth");
			LogComment("Setup was successful");
			return TestResult.Pass;
		}

#if COLLVIEW_IS_ILIST   // but it isn't anymore :-)
		private TestResult TestIList()
		{
			Status("TestIList");

			// Add
			bool throwsException1 = false;
			try
			{
				((IList)lcv).Add("Sixth");
			}
			catch (NotSupportedException nse)
			{
				Status("Expected exception:" + nse.Message + " - " + nse.GetType());
				throwsException1 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException1)
			{
				LogComment("Fail - No exception was thrown. Expected a NotSupportedException.");
				return TestResult.Fail;
			}

			// Clear
			bool throwsException2 = false;
			try
			{
				((IList)lcv).Clear();
			}
			catch (NotSupportedException nse)
			{
				Status("Expected exception:" + nse.Message + " - " + nse.GetType());
				throwsException2 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException2)
			{
				LogComment("Fail - No exception was thrown. Expected a NotSupportedException.");
				return TestResult.Fail;
			}

			// Insert
			bool throwsException3 = false;
			try
			{
				((IList)lcv).Insert(2, "Seventh");
			}
			catch (NotSupportedException nse)
			{
				Status("Expected exception:" + nse.Message + " - " + nse.GetType());
				throwsException3 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException3)
			{
				LogComment("Fail - No exception was thrown. Expected a NotSupportedException.");
				return TestResult.Fail;
			}

			// Remove
			bool throwsException4 = false;
			try
			{
				((IList)lcv).Remove("First");
			}
			catch (NotSupportedException nse)
			{
				Status("Expected exception:" + nse.Message + " - " + nse.GetType());
				throwsException4 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException4)
			{
				LogComment("Fail - No exception was thrown. Expected a NotSupportedException.");
				return TestResult.Fail;
			}

			// RemoveAt
			bool throwsException5 = false;
			try
			{
				((IList)lcv).RemoveAt(0);
			}
			catch (NotSupportedException nse)
			{
				Status("Expected exception:" + nse.Message + " - " + nse.GetType());
				throwsException5 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException5)
			{
				LogComment("Fail - No exception was thrown. Expected a NotSupportedException.");
				return TestResult.Fail;
			}

			// IsReadOnly getter
			if (((IList)lcv).IsReadOnly != true)
			{
				LogComment("Fail - IsReadOnly should be true but is false");
				return TestResult.Fail;
			}

			// IsFixedSize getter
			if (((IList)lcv).IsFixedSize != true)
			{
				LogComment("Fail - IsFixedSize should be true but is false");
				return TestResult.Fail;
			}

			LogComment("TestIList was successful");
			return TestResult.Pass;
		}

		private TestResult TestICollection()
		{
			Status("TestICollection");
			string[] stringArray = new string[10];

			// CopyTo - exception
			bool throwsException1 = false;
			try
			{
				((ICollection)lcv).CopyTo(stringArray, -1);
			}
			catch (ArgumentOutOfRangeException aoore)
			{
				Status("Expected exception:" + aoore.Message + " - " + aoore.GetType());
				throwsException1 = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Exception does not have expected type. Actual:" + e.Message + " - " + e.GetType());
				return TestResult.Fail;
			}
			if (!throwsException1)
			{
				LogComment("Fail - No exception was thrown. Expected a ArgumentOutOfRangeException.");
				return TestResult.Fail;
			}

			// CopyTo - correct behavior
			((ICollection)lcv).CopyTo(stringArray, 1);

			if (stringArray[0] != null)
			{
				LogComment("Fail - stringArray[0] should be null");
				return TestResult.Fail;
			}
			if (stringArray[1] != "First")
			{
				LogComment("Fail - stringArray[1] should be 'First'");
				return TestResult.Fail;
			}
			if (stringArray[5] != "Fifth")
			{
				LogComment("Fail - stringArray[5] should be 'Fifth'");
				return TestResult.Fail;
			}
			if (stringArray[6] != null)
			{
				LogComment("Fail - stringArray[6] should be null");
				return TestResult.Fail;
			}

			// IsSynchronized - thread-safe
			if (((ICollection)lcv).IsSynchronized != false)
			{
				LogComment("Fail - IsSynchronized should be false but is true");
				return TestResult.Fail;
			}

			// SyncRoot is used to synchronize access to ICollection
			if (((ICollection)lcv).SyncRoot == null)
			{
				LogComment("Fail - SyncRoot is null");
				return TestResult.Fail;
			}

			LogComment("TestICollection was successful");
			return TestResult.Pass;
		}
#endif // COLLVIEW_IS_ILIST 


		public TestResult TestIComparer()
		{
			Status("TestIComparer");

			int res1 = ((IComparer)_lcv).Compare("First", "Second");
			if (res1 != -1)
			{
				LogComment("Fail - TestResult of Compare should be -1, instead it is " + res1);
				return TestResult.Fail;
			}

			int res2 = ((IComparer)_lcv).Compare("Second", "First");
			if (res2 != 1)
			{
				LogComment("Fail - TestResult of Compare should be 1, instead it is " + res2);
				return TestResult.Fail;
			}

			int res3 = ((IComparer)_lcv).Compare("First", "First");
			if (res3 != 0)
			{
				LogComment("Fail - TestResult of Compare should be 0, instead it is " + res3);
				return TestResult.Fail;
			}

			int res4 = ((IComparer)_lcv).Compare(null, "First");
			if (res4 != -1)
			{
				LogComment("Fail - TestResult of Compare should be -1, instead it is " + res4);
				return TestResult.Fail;
			}

			int res5 = ((IComparer)_lcv).Compare("First", null);
			if (res5 != 1)
			{
				LogComment("Fail - TestResult of Compare should be 1, instead it is " + res5);
				return TestResult.Fail;
			}

			int res6 = ((IComparer)_lcv).Compare("Seventh", "Nineth");
			if (res6 != 0)
			{
				LogComment("Fail - TestResult of Compare should be 0, instead it is " + res6);
				return TestResult.Fail;
			}

			LogComment("TestIComparer was successful");
			return TestResult.Pass;
		}

		private TestResult TestContains()
		{
			Status("TestContains");

			if (_lcv.Contains("hello"))
			{
				LogComment("Fail - ListCollectionView should not contain 'hello'");
				return TestResult.Fail;
			}
			if (_lcv.Contains(null))
			{
				LogComment("Fail - ListCollectionView should not contains null");
				return TestResult.Fail;
			}
			if (!_lcv.Contains("First"))
			{
				LogComment("Fail - ListCollectionView should contain 'First'");
				return TestResult.Fail;
			}

			LogComment("TestContains was successful");
			return TestResult.Pass;
		}

		private TestResult TestIndexOf()
		{
			Status("TestIndexOf");

			if (_lcv.IndexOf("hello") != -1)
			{
				LogComment("Fail - IndexOf 'hello' should be -1");
				return TestResult.Fail;
			}

			LogComment("TestIndexOf was successful");
			return TestResult.Pass;
		}
	}
}

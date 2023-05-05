// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Threading; 
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Makes sure that we can assign an IEnumerable directly to the ItemSource of a ListBox
	/// </description>
	/// <relatedBugs>

	/// </relatedBugs>
	/// </summary>
    [Test(1, "Controls", "CCIEnumerable")]
	public class CCIEnumerable : WindowTest
	{
		private DockPanel _dp;
		private ListBox _lb;

		public CCIEnumerable()
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(AssignIEnumerable);
			RunSteps += new TestStep(VerifyItems);
		}

		private TestResult Setup()
		{
			Status("Setup");
			_dp = new DockPanel();
			_lb = new ListBox();

			_dp.Children.Add(_lb);
			Window.Content = _dp;

			WaitForPriority(DispatcherPriority.Render);

			LogComment("Setup was successful");
			return TestResult.Pass;
		}

		private TestResult AssignIEnumerable()
		{
			Status("AssignIEnumerable");
			IEnumerable coll = PublicTypes(typeof(String).Assembly.GetTypes());
			_lb.ItemsSource = coll;
			LogComment("AssignIEnumerable was successful");
			return TestResult.Pass;
		}

		private TestResult VerifyItems()
		{
			Status("VerifyItems");
			IEnumerable ieActual = _lb.ItemsSource as IEnumerable;
			if (ieActual == null)
			{
				LogComment("Fail - IEnumerable ieActual is null");
				return TestResult.Fail;
			}
			IEnumerable ieExpected = PublicTypes(typeof(String).Assembly.GetTypes());
			if (ieExpected == null)
			{
				LogComment("Fail - IEnumerable ieExpected is null");
				return TestResult.Fail;
			}

			IEnumerator enumeratorExpected = ieExpected.GetEnumerator();
			foreach (Type type1 in ieActual)
			{
				enumeratorExpected.MoveNext();
				Type type2 = enumeratorExpected.Current as Type;
				if (type1 != type2)
				{
					LogComment("Fail - Expected type:" + type2.ToString() + " Actual:" + type1.ToString());
					return TestResult.Fail;
				}
			}
			LogComment("VerifyItems was successful");
			return TestResult.Pass;
		}

		private IEnumerable PublicTypes(Type[] types)
		{
			for (int i = 0; i < types.Length; i++)
				if (types[i].IsPublic)
					yield return types[i];
		}

	}
}





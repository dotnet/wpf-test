// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Globalization;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the frameworkPropertyMetadata.IsDataBindingAllowed and 
	/// frameworkPropertyMetadata.IsNotDataBindable values for 3 dps: one that is 
	/// not readonly and databinding is allowed, one that is read-only and one that 
	/// is not readonly and databinding is not allowed.
	/// </description>
	/// </summary>
    [Test(3, "Binding", "MetadataFlagsTest")]
	public class MetadataFlagsTest : XamlTest
	{
		private MyButton _btn1;
		private MyDS _myDS;

		public MetadataFlagsTest() : base(@"MetadataFlagsTest.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestMyDpProperty);
			RunSteps += new TestStep(TestMyReadOnlyDpProperty);
			RunSteps += new TestStep(TestMyNotDataBindableDpProperty);
		}

		private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);
			
			_btn1 = LogicalTreeHelper.FindLogicalNode(RootElement, "btn1") as MyButton;
			if (_btn1 == null)
			{
				LogComment("Fail - Unable to reference btn1");
				return TestResult.Fail;
			}

			_myDS = RootElement.Resources["myDS"] as MyDS;
			if (_myDS == null)
			{
				LogComment("Fail - Unable to reference myDS");
				return TestResult.Fail;
			}

			LogComment("Setup was successful");
			return TestResult.Pass;
		}

		// Tests that I can bind to a dp that is not readonly and that doesn't
		// have the FrameworkPropertyMetadataOptions.NotDataBindable flag
		// It tests that IsDataBindingAllowed=true and IsNotDataBindable=false
		private TestResult TestMyDpProperty()
		{
			Status("TestMyDpProperty");
			
			if (_btn1.MyDp != _myDS.IntProp)
			{
				LogComment("Fail - Expected MyDp:" + _myDS.IntProp + " Actual:" + _btn1.MyDp);
				return TestResult.Pass;
			}

			FrameworkPropertyMetadata fpm = MyButton.MyDpProperty.GetMetadata(_btn1) as FrameworkPropertyMetadata;
			if (fpm.IsDataBindingAllowed != true)
			{
				LogComment("Fail - MyDp's IsDataBindingAllowed should be true");
				return TestResult.Fail;
			}
			if (fpm.IsNotDataBindable != false)
			{
				LogComment("Fail - MyDp's IsNotDataBindable should be false");
				return TestResult.Fail;
			}

			LogComment("TestMyDpProperty was successful");
			return TestResult.Pass;
		}

		// Tests that IsDataBindingAllowed=false and IsNotDataBindable=false for a read only
		// property. (I get a compile error when trying to assign to it (or data bind to it))
		private TestResult TestMyReadOnlyDpProperty()
		{
			Status("TestMyReadOnlyDpProperty");

			if (_btn1.MyReadOnlyDp != 6)
			{
				LogComment("Fail - MyReadOnlyDp's expected value:6 Actual:" + _btn1.MyReadOnlyDp);
				return TestResult.Fail;
			}

			FrameworkPropertyMetadata fpm = MyButton.MyReadOnlyDpProperty.GetMetadata(_btn1) as FrameworkPropertyMetadata;
			if (fpm.IsDataBindingAllowed != false)
			{
				LogComment("Fail - MyReadOnlyDp's IsDataBindingAllowed should be false");
				return TestResult.Fail;
			}
			if (fpm.IsNotDataBindable != false)
			{
				LogComment("Fail - MyReadOnlyDp's IsNotDataBindable should be false");
				return TestResult.Fail;
			}

			// I get a compile error with the following code, as expected
//			btn1.MyReadOnlyDp = 3;

			LogComment("TestMyReadOnlyDpProperty was successful");
			return TestResult.Pass;
		}

		// Tests that if I pass FrameworkPropertyMetadataOptions.NotDataBindable when registering a dp,
		// IsDataBindingAllowed=false and IsNotDataBindable=true
		// Tests that binding to that dp throws an ArgumentException
		private TestResult TestMyNotDataBindableDpProperty()
		{
			Status("TestMyNotDataBindableDpProperty");

			if (_btn1.MyNotDataBindableDp != 9)
			{
				LogComment("Fail - MyNotDataBindableDp's expected value:9 Actual:" + _btn1.MyNotDataBindableDp);
				return TestResult.Fail;
			}

			FrameworkPropertyMetadata fpm = MyButton.MyNotDataBindableDpProperty.GetMetadata(_btn1) as FrameworkPropertyMetadata;
			if (fpm.IsDataBindingAllowed != false)
			{
				LogComment("Fail - MyNotDataBindableDp's IsDataBindingAllowed should be false");
				return TestResult.Fail;
			}
			if (fpm.IsNotDataBindable != true)
			{
				LogComment("Fail - MyNotDataBindableDp's IsNotDataBindable should be true");
				return TestResult.Fail;
			}

			bool argumentExceptionThrown = false;
			try
			{
				Binding bind = new Binding("IntProp");
				bind.Source = _myDS;
				_btn1.SetBinding(MyButton.MyNotDataBindableDpProperty, bind);
			}
			catch (ArgumentException ae)
			{
				Status("Expected exception: " + ae.GetType() + " - " + ae.Message);
				argumentExceptionThrown = true;
			}
			catch (Exception e)
			{
				LogComment("Fail - Expected exception:ArgumentException. Actual:"
					+ e.GetType() + " - " + e.Message);
				return TestResult.Fail;
			}
			if (!argumentExceptionThrown)
			{
				LogComment("Fail - No exception was thrown. Expected ArgumentException.");
				return TestResult.Fail;
			}

			LogComment("TestMyNotDataBindableDpProperty was successful");
			return TestResult.Pass;
		}
	}

	#region MyDependencyObject
	public class MyButton : Button
	{
		public static readonly DependencyProperty MyDpProperty =
			DependencyProperty.Register("MyDp", typeof(int), typeof(MyButton),
			new FrameworkPropertyMetadata(0));

		public int MyDp
		{
			get
			{
				return (int)GetValue(MyDpProperty);
			}
			set
			{
				SetValue(MyDpProperty, value);
			}
		}

		private static readonly DependencyPropertyKey s_myReadOnlyDpPropertyKey =
			DependencyProperty.RegisterReadOnly("MyReadOnlyDp", typeof(int), typeof(MyButton),
			new FrameworkPropertyMetadata(6));
		public static readonly DependencyProperty MyReadOnlyDpProperty =
			s_myReadOnlyDpPropertyKey.DependencyProperty;

		public int MyReadOnlyDp
		{
			get
			{
				return (int)GetValue(MyReadOnlyDpProperty);
			}
		}

		public static readonly DependencyProperty MyNotDataBindableDpProperty =
			DependencyProperty.Register("MyNotDataBindableDp", typeof(int), typeof(MyButton),
			new FrameworkPropertyMetadata(3, FrameworkPropertyMetadataOptions.NotDataBindable));

		public int MyNotDataBindableDp
		{
			get
			{
				return (int)GetValue(MyNotDataBindableDpProperty);
			}
			set
			{
				SetValue(MyNotDataBindableDpProperty, value);
			}
		}
	}
	#endregion

	public class MyDS : INotifyPropertyChanged
	{
		private int _intProp;
		public int IntProp
		{
			get { return _intProp; }
			set 
			{ 
				_intProp = value;
				RaisePropertyChangedEvent("IntProp");
			}
		}

		public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

		private void RaisePropertyChangedEvent(string name)
		{
			if (PropertyChanged != null)
			{
				PropertyChanged(this, new System.ComponentModel.PropertyChangedEventArgs(name));
			}
		}

		public MyDS()
		{
			IntProp = 0;
		}
	}
}


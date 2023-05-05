// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Collections;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This tests setting SelectedIndex / SelectedItem / IsSelectionRequired / SelectedValue 
    /// in a ListBox and setting the ItemsSource to a data source that takes a while 
	/// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Controls", "DeferredSelection")]
	public class DeferredSelection : XamlTest
	{
		private ListBox _lb;
		private FastDataItem _expectedSelectedItem;
        private SelectedItemsVerifier _siv;

        public DeferredSelection()
            : base(@"DeferredSelection.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(TestSelectedIndex);
            RunSteps += new TestStep(TestSelectedValue);
            RunSteps += new TestStep(ChangeSource);
            RunSteps += new TestStep(TestInvalidSelection);
        }

        private TestResult Setup()
		{
			Status("Setup");

			_lb = Util.FindElement(RootElement, "lb") as ListBox;
			if (_lb == null)
			{
				LogComment("Fail - Unable to reference lb element (ListBox)");
				return TestResult.Fail;
			}

            _siv = new SelectedItemsVerifier(_lb);

            LogComment("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult TestSelectedIndex()
		{
			Status("TestSelectedIndex");
			Reset();
            int expectedIndexSelectedItem = 4;

            MySlowDataSource msds = SetupBinding();
            _lb.SelectedIndex = expectedIndexSelectedItem;
            if (!VerifyNothingIsSelected()) { return TestResult.Fail; }
            WaitForPriority(DispatcherPriority.SystemIdle);
            ObservableCollection<FastDataItem> aldc = msds.Data as ObservableCollection<FastDataItem>;
            _expectedSelectedItem = aldc[expectedIndexSelectedItem] as FastDataItem;
            if (!VerifyExpectedItemIsSelected()) { return TestResult.Fail; }

            LogComment("TestSelectedIndex was successful");
			return TestResult.Pass;
		}

        private TestResult TestSelectedValue()
        {
            Status("TestSelectedValue");
            Reset();

            MySlowDataSource msds = SetupBinding();
            _lb.SelectedValue = "FastValue5";
            _lb.SelectedValuePath = "FastValue";
            if (!VerifyNothingIsSelected()) { return TestResult.Fail; }
            WaitForPriority(DispatcherPriority.SystemIdle);
            ObservableCollection<FastDataItem> aldc = msds.Data as ObservableCollection<FastDataItem>;
            _expectedSelectedItem = aldc[4] as FastDataItem;
            if (!VerifyExpectedItemIsSelected()) { return TestResult.Fail; }

            LogComment("TestSelectedValue was successful");
            return TestResult.Pass;
        }

        // Setting the selected index and changing the source makes the listbox loose selection
        // This is by design.
        private TestResult ChangeSource()
        {
            Status("ChangeSource");

            MySlowDataSource msds = SetupBinding();
            int validSelectedIndex = 3;
            if (!VerifyNothingIsSelected()) { return TestResult.Fail; }
            WaitForPriority(DispatcherPriority.SystemIdle);
            ObservableCollection<FastDataItem> aldc = msds.Data as ObservableCollection<FastDataItem>;
            aldc[validSelectedIndex] = new FastDataItem("FastValueChanged");

            int selectedIndexAfter = _lb.SelectedIndex;
            if (selectedIndexAfter != -1)
            {
                LogComment("Fail - The item should remain selected after being modified. Before:" + validSelectedIndex + " After:" + selectedIndexAfter);
                return TestResult.Fail;
            }

            LogComment("ChangeSource was successful");
            return TestResult.Pass;
        }

        private TestResult TestInvalidSelection()
        {
            Status("TestInvalidSelection");
            Reset();

            MySlowDataSource msds = SetupBinding();
            _lb.SelectedIndex = 12;
            if (!VerifyNothingIsSelected()) { return TestResult.Fail; }
            WaitForPriority(DispatcherPriority.SystemIdle);
            _expectedSelectedItem = null;
            if (!VerifyExpectedItemIsSelected()) { return TestResult.Fail; }

            LogComment("TestInvalidSelection was successful");
            return TestResult.Pass;
        }

		#region AuxMethods

        private MySlowDataSource SetupBinding()
        {
            Binding bind = new Binding();
            bind.NotifyOnTargetUpdated = true;
            bind.IsAsync = true;
            MySlowDataSource slowSource = new MySlowDataSource();
            bind.Source = slowSource;
            BindingOperations.SetBinding(_lb, ListBox.ItemsSourceProperty, bind);
            return slowSource;
        }

        private bool VerifyNothingIsSelected()
        {
            //selectedItem is null at this point although we might have set it to something else
            VerifyResult result = _siv.Verify(null) as VerifyResult;
            LogComment(result.Message);
            if (result.Result == TestResult.Fail) { return false; }
            return true;
        }

        private bool VerifyExpectedItemIsSelected()
        {
            VerifyResult result = _siv.Verify(_expectedSelectedItem) as VerifyResult;
            LogComment(result.Message);
            if (result.Result == TestResult.Fail) { return false; }
            return true;
        }

		private void Reset()
		{
			_lb.ItemsSource = null;
			_lb.Items.Clear();
		}
		#endregion
	}
}


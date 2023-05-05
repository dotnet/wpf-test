// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the SelectedValuePath and SelectedPath properties of ItemsControl.
	/// Data source of the items control is a CLR object.
	/// This tests assings values to these properties in 2 ways: hardcoded explictly and using 
	/// data binding. 
	/// </description>
    /// <relatedBugs>





    /// </relatedBugs>
	/// </summary>
    [Test(1, "Controls", "SelectedValuePath")]
    public class SelectedValuePath : XamlTest
    {
		private ListBox _myListBox1;
		private ListBox _myListBox2;
		private ObjectDataProvider _ods;
        private SelectedValueVerifier _svv1;
        private SelectedValueVerifier _svv2;

        public SelectedValuePath()
            : base(@"SelectedValuePath.xaml")
		{
			InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyNotBoundListBox);
            RunSteps += new TestStep(VerifyBoundListBox);
            //RunSteps += new TestStep(ChangeNotBoundListBox);
            RunSteps += new TestStep(ChangeBoundListBox);
            RunSteps += new TestStep(SelectedValuePathNull);
        }

        private TestResult Setup()
		{
			Status("Setup");
			WaitForPriority(DispatcherPriority.Render);

			_myListBox1 = Util.FindElement(RootElement, "myListBox1") as ListBox;
			if (_myListBox1 == null)
			{
				LogComment("Fail - Unable to reference myListBox1.");
				return TestResult.Fail;
			}

			_myListBox2 = Util.FindElement(RootElement, "myListBox2") as ListBox;
			if (_myListBox2 == null)
			{
				LogComment("Fail - Unable to reference myListBox2.");
				return TestResult.Fail;
			}

			_ods = RootElement.Resources["ods"] as ObjectDataProvider;
			if (_ods == null)
			{
				LogComment("Fail - Unable to reference the ObjectDataProvider ods.");
				return TestResult.Fail;
			}

            _svv1 = new SelectedValueVerifier(_myListBox1);
            _svv2 = new SelectedValueVerifier(_myListBox2);

            Status("Setup was successful");
			return TestResult.Pass;
		}

        private TestResult VerifyNotBoundListBox()
		{
			Status("VerifyNotBoundListBox");
			WaitForPriority(DispatcherPriority.Render);

			// expected1 values
			int expectedIndex = 1;
			string expectedSelectedValue = "Redmond";
			string expectedSelectedValuePath = "Name";

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = _svv1.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

        private TestResult VerifyBoundListBox()
		{
			Status("VerifyBoundListBox");
			WaitForPriority(DispatcherPriority.Render);

			// expected1 values
			int expectedIndex = 5;
			string expectedSelectedValue = "CA";
			string expectedSelectedValuePath = "State";

            WaitForPriority(DispatcherPriority.SystemIdle);
            VerifyResult result = _svv2.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

        private TestResult ChangeNotBoundListBox()
		{
			Status("ChangeNotBoundListBox");
			WaitForPriority(DispatcherPriority.Render);

			_myListBox1.SelectedValue = "WA";
			_myListBox1.SelectedValuePath = "State";

			// expected1 values
			int expectedIndex = 0;
			string expectedSelectedValue = "WA";
			string expectedSelectedValuePath = "State";

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = _svv1.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

        private TestResult ChangeBoundListBox()
		{
			Status("ChangeBoundListBox");
			WaitForPriority(DispatcherPriority.Render);

            // new Binding for SelectedValuePath
            Binding bindValuePath = new Binding();
            bindValuePath.ElementName = "txt4";
            bindValuePath.Path = new PropertyPath(TextBlock.TextProperty);
            _myListBox2.SetBinding(ListBox.SelectedValuePathProperty, bindValuePath);
            
            // new Binding for SelectedValue
            Binding bindValue = new Binding();
			bindValue.ElementName = "txt3";
            bindValue.Path = new PropertyPath(TextBlock.TextProperty);
            _myListBox2.SetBinding(ListBox.SelectedValueProperty, bindValue);

			// expected1 values
			int expectedIndex = 10;
            string expectedSelectedValuePath = "Name";
            string expectedSelectedValue = "Bellingham";

            WaitForPriority(DispatcherPriority.Background);
            VerifyResult result = _svv2.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

        // Regression Bug - SelectedValue in ComboBox doesn't get updated
        private TestResult SelectedValuePathNull()
        {
            ListBox lb = new ListBox();
            ((Panel)RootElement).Children.Add(lb);

            Places p = new Places();
            p[0].Name = null;

            // If I set this to "Bellevue" instead of null it works fine
            lb.SelectedValue = null;
            lb.SelectedValuePath = "Name";

            lb.ItemsSource = p;

            lb.SelectedItem = p[1];

            // The bug was that while SelectedItem moved to Redmond, WA SelectedValue remained null.
            if (!Object.Equals(lb.SelectedValue, "Redmond")) return TestResult.Fail;

            lb.SelectedValue = null;


            if (!Object.Equals(lb.SelectedItem, p[0])) return TestResult.Fail;

            return TestResult.Pass;
        }
	}
}

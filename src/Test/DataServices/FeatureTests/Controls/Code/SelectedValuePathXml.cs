// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; using System.Windows.Threading;
using System.Windows;
using System.Xml;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using System.Collections;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests the SelectedValuePath and SelectedPath properties of ItemsControl.
	/// Data source of the items control is XML.
	/// There are 3 list boxes in the xaml file of this test. The first one has the SelectedValue
	/// and SelectedValuePath properties hardcoded explicitly, the second one is bound
	/// to an XML source and the third one is bound to a value non existent in the list.
	/// </description>
    /// <relatedBugs>






    /// </relatedBugs>
	/// </summary>
    [Test(1, "Controls", "SelectedValuePathXml")]
    public class SelectedValuePathXml : XamlTest
    {
        private ListBox _myListBox1;
        private ListBox _myListBox2;
        private ListBox _myListBox3;
        private XmlDataProvider _xds;
        private SelectedValueVerifier _svv1;
        private SelectedValueVerifier _svv2;
        private SelectedValueVerifier _svv3;

        public SelectedValuePathXml() : base(@"SelectedValuePathXml.xaml")
		{
			InitializeSteps += new TestStep(Setup);
			RunSteps += new TestStep(VerifyListBox1);
             
            RunSteps += new TestStep(VerifyListBox2);
             
            RunSteps += new TestStep(VerifyListBox3);

            //RunSteps += new TestStep(VerifyChangeListBox1);
            RunSteps += new TestStep(VerifyChangeListBox2);
             
            RunSteps += new TestStep(VerifyChangeListBox3);
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
				LogComment("Fail - Unable to reference myListBox2");
				return TestResult.Fail;
			}

			_myListBox3 = Util.FindElement(RootElement, "myListBox3") as ListBox;
			if (_myListBox3 == null)
			{
				LogComment("Fail - Unable to reference myListBox3");
				return TestResult.Fail;
			}

			_xds = RootElement.Resources["xds"] as XmlDataProvider;
			if (_xds == null)
			{
				LogComment("Fail - Unable to reference the XmlDataProvider xds");
				return TestResult.Fail;
			}

            _svv1 = new SelectedValueVerifier(_myListBox1);
            _svv2 = new SelectedValueVerifier(_myListBox2);
            _svv3 = new SelectedValueVerifier(_myListBox3);

            // 30 seconds is overkill but if it really takes that long we ought to fail.
            Util.WaitForItemsControlPopulation(_myListBox1, 30);
            Util.WaitForItemsControlPopulation(_myListBox2, 30);
            Util.WaitForItemsControlPopulation(_myListBox3, 30);

            Status("Setup was successful");
			return TestResult.Pass;
		}


		private TestResult VerifyListBox1()
		{
			Status("VerifyListBox1");
			WaitForPriority(DispatcherPriority.Render);

			// expected values
			int expectedIndex = 3; // notice that this index is 0 based (XPath syntax is not)
			string expectedSelectedValue = "0-321-15491-6";
			string expectedSelectedValuePath = "@ISBN";

            VerifyResult result = _svv1.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

		private TestResult VerifyListBox2()
		{
			Status("VerifyListBox2");
			WaitForPriority(DispatcherPriority.Render);

			// expected values
			int expectedIndex = 0; // Notice that there's 2 books with price 30. By default the first one is chosen.
            string expectedSelectedValuePath = "Price";
            string expectedSelectedValue = "30";

            VerifyResult result = _svv2.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

		private TestResult VerifyListBox3()
		{
			Status("VerifyListBox3");
			WaitForPriority(DispatcherPriority.Render);

			// expected values
			int expectedIndex = -1;
            string expectedSelectedValuePath = "Price";
            string expectedSelectedValue = null;

            VerifyResult result = _svv3.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

		private TestResult VerifyChangeListBox1()
		{
			Status("VerifyChangeListBox1");
			WaitForPriority(DispatcherPriority.Render);

			// new SelectedValue and SelectedValuePath
            _myListBox1.SelectedValuePath = "Title";
            _myListBox1.SelectedValue = "Xml in Action";

            // expected values
			int expectedIndex = 2;
            string expectedSelectedValuePath = "Title";
            string expectedSelectedValue = "Xml in Action";

            VerifyResult result = _svv1.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

		private TestResult VerifyChangeListBox2()
		{
			Status("VerifyChangeListBox2");
			WaitForPriority(DispatcherPriority.Render);

            // new Binding for SelectedValuePath
            Binding bindValuePath = new Binding();
            bindValuePath.Source = _xds;
            bindValuePath.XPath = "BindTo/Element";
            _myListBox2.SetBinding(ListBox.SelectedValuePathProperty, bindValuePath);
            
            // new Binding for SelectedValue
            Binding bindValue = new Binding();
			bindValue.Source = _xds;
			bindValue.XPath = "BindTo/Price2";
			_myListBox2.SetBinding(ListBox.SelectedValueProperty, bindValue);

			// expected values
			int expectedIndex = 1;
            string expectedSelectedValuePath = "Price";
            string expectedSelectedValue = "50";

            VerifyResult result = _svv2.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}

		private TestResult VerifyChangeListBox3()
		{
			Status("VerifyChangeListBox3");
			WaitForPriority(DispatcherPriority.Render);

            // new value for SelectedValuePath
            _myListBox3.SelectedValuePath = "@ISBN";
            
            // new Binding for SelectedValue
            Binding bindValue = new Binding();
			bindValue.Source = _xds;
			bindValue.XPath = "Book[2]/@ISBN"; // XPath indices are 0 based
			_myListBox3.SetBinding(ListBox.SelectedValueProperty, bindValue);

			// expected values
			int expectedIndex = 1;
            string expectedSelectedValuePath = "@ISBN";
            string expectedSelectedValue = "0-7356-1288-9";

            VerifyResult result = _svv3.Verify(expectedIndex, expectedSelectedValue, expectedSelectedValuePath) as VerifyResult;
            LogComment(result.Message);
            return result.Result;
		}
	}
}

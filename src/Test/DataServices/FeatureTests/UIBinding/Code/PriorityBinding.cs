// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading; 
using System.Windows.Threading;
using System.Collections;
using System.Windows;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests Priority Bindings
	/// </description>
	/// </summary>
    [Test(0, "Binding", "PriorityBinding")]
	public class PriorityBinding : XamlTest
	{
		Button _btn1,_btn2,_btn3;

		public PriorityBinding() : base(@"PriorityBinding.xaml")
		{
			InitializeSteps += new TestStep(initializeTest);

			RunSteps += new TestStep(validProperty);
			RunSteps += new TestStep(invalidProperty);
			RunSteps += new TestStep(nonValidProperties);

		}

		private TestResult initializeTest()
		{
			Status("Finding root elements.");
			_btn1 = LogicalTreeHelper.FindLogicalNode(RootElement, "btn1") as Button;
			_btn2 = LogicalTreeHelper.FindLogicalNode(RootElement, "btn2") as Button;
			_btn3 = LogicalTreeHelper.FindLogicalNode(RootElement, "btn3") as Button;
			
			return TestResult.Pass;
		}

		/// <summary>
		/// Validate binding for valid property
		/// </summary>
		/// <returns></returns>
		private TestResult validProperty()
		{
			Status("Validate correct binding for valid property with 'non-null' value");
			if(!(ValidateContent("Oddysseus", _btn1.Content.ToString())))
				return TestResult.Fail;

			return TestResult.Pass;
		}
		private TestResult invalidProperty()
		{
			Status("Validate element contents for binding that has a 'invalid property'. ");
			if (!(ValidateContent("Oddysseus", _btn2.Content.ToString())))
				return TestResult.Fail;

			return TestResult.Pass;
		}
		private TestResult nonValidProperties()
		{
			Status("Validate element contents for binding that only has 'invalid properties'.");
			if (!(ValidateContent("Fallback Value!", _btn3.Content.ToString())))
				return TestResult.Fail;

			return TestResult.Pass;

		}

		private bool ValidateContent(string expected, string actual)
		{
			if (expected != actual)
			{
				LogComment("The binding is incorrect.  Expected element content:  " + expected + "Actual element content:  " + actual);
				return false;
			}
			return true;
		}

	}

	public class GreekKing : Dwarf
	{
		public GreekKing()
		{
			this.Name = "Oddysseus";
		}
	}

}

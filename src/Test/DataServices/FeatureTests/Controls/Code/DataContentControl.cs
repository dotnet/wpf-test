// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
	/// Tests binding to content controls (several variations).
	/// </description>
	/// </summary>
    [Test(3, "Controls", "ContentControlTest")]
	public class ContentControlTest: XamlTest
	{

		FrameworkElement[] _visualelement;

		public ContentControlTest(): this(@"Button.xaml")
		{
		}
		
		[Variation(@"CheckBox.xaml")]
		[Variation(@"ComboBoxItem.xaml")]
		[Variation(@"Label.xaml")]
		[Variation(@"ListItem.xaml")]
		[Variation(@"RadioButton.xaml")]
		public ContentControlTest(string filename): base(filename)
		{
			InitializeSteps += new TestStep(InitializeTestCase);
			RunSteps += new TestStep(VisualElementType);
			RunSteps += new TestStep(ElementContent);
			RunSteps += new TestStep(ElementFontSize);
			RunSteps += new TestStep(ElementForeGround);
			RunSteps += new TestStep(ElementFontWeight);
			RunSteps += new TestStep(ElementFontStyle);
		}

        private TestResult InitializeTestCase()
		{
			Status("InitializeTestCase");
			DockPanel dp = LogicalTreeHelper.FindLogicalNode(RootElement, "dp") as DockPanel;
			_visualelement = Util.FindDataVisuals(dp, new object[] { dp.DataContext });

			if (dp.Children.Count < 0)
			{
				LogComment("There are no visual elements in tree.");
				return TestResult.Fail;
			}

			LogComment("InitializeTestCase was successful");
			return TestResult.Pass;
		}

        private TestResult VisualElementType()
		{
			Status("VisualElementType");
			if (!(ValidateElementType(typeof(TextBlock), _visualelement[0].GetType())))
				return TestResult.Fail;

			LogComment("VisualElementType was successful");
			return TestResult.Pass;
		}

        private TestResult ElementContent()
		{
			Status("ElementContent");
			if (!(ValidateContent("Oddysseus", ((TextBlock)_visualelement[0]).Text)))
				return TestResult.Fail;

			LogComment("ElementContent was successful");
			return TestResult.Pass;
		}

        private TestResult ElementFontSize()
		{
			Status("ElementFontSize");
			if (!(ValidateFontSize("16", ((TextBlock)_visualelement[0]).FontSize.ToString())))
				return TestResult.Fail;

			LogComment("ElementFontSize was successful");
			return TestResult.Pass;
		}

        private TestResult ElementForeGround()
		{
			Status("ElementForeGround");
			if (!(ValidateForeground(Brushes.Red, ((TextBlock)_visualelement[0]).Foreground)))
				return TestResult.Fail;

			LogComment("ElementForeGround was successful");
			return TestResult.Pass;
		}

        private TestResult ElementFontWeight()
		{
			Status("ElementFontWeight");
			if (!(ValidateFontWeight(FontWeights.Bold, ((TextBlock)_visualelement[0]).FontWeight)))
				return TestResult.Fail;

			LogComment("ElementFontWeight was successful");
			return TestResult.Pass;
		}

        private TestResult ElementFontStyle()
		{
			Status("ElementFontStyle");
			if (!(ValidateFontStyle(FontStyles.Italic, ((TextBlock)_visualelement[0]).FontStyle)))
				return TestResult.Fail;

			LogComment("ElementFontStyle was successful");
			return TestResult.Pass;
		}

		#region AuxMethods
		bool ValidateElementType(Type expected, Type actual)
		{
			if (expected != actual)
			{
				LogComment("Object type is incorrect.  Expected:  " + expected.ToString() + "  Actual:  " + actual.ToString());
				return false;
			}

			return true;
		}


		bool ValidateContent(string expected, string actual)
		{
			if (expected != actual)
			{
				LogComment("Content is incorrect.  Expected:  " + expected + "  Actual:  " + actual);
				return false;
			}

			return true;
		}


		bool ValidateFontSize(string expected, string actual)
		{
			if (expected != actual)
			{
				LogComment("FontSize is incorrect.  Expected:  " + expected + " Actual:  " + actual);
				return false;
			}

			return true;
		}


		bool ValidateForeground(Brush expected, Brush actual)
		{
			if (expected != actual)
			{
				LogComment("Foreground type is incorrect.  Expected:  " + expected + "  Actual:  " + actual);
				return false;
			}

			return true;
		}


		bool ValidateFontWeight(FontWeight expected, FontWeight actual)
		{
			if (expected != actual)
			{
				LogComment("FontWeight type is incorrect.  Expected:  " + expected + "  Actual:  " + actual);
				return false;
			}

			return true;
		}


		bool ValidateFontStyle(FontStyle expected, FontStyle actual)
		{
			if (expected != actual)
			{
				LogComment("FontStyle type is incorrect.  Expected:  " + expected + "  Actual:  " + actual);
				return false;
			}

			return true;
		}
		#endregion
	}
}


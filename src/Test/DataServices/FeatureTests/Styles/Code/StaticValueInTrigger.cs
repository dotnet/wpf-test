// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Media;
using System.Windows;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using System.Windows.Markup;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// This test verifies that we can set static values to the "Value" property of a DataTrigger and Trigger.
	/// </description>
	/// <relatedBugs>


    /// </relatedBugs>
	/// </summary>
    [Test(2, "Styles", "StaticValueInTrigger")]
	public class StaticValueInTrigger : XamlTest
	{
        // DataTriggers
        private ContentControl _cc1;
        private ContentControl _cc2;
        private ContentControl _cc3;

        // Triggers
        private CheckBox _cb1;
        private CheckBox _cb2;
        private CheckBox _cb3;

        public StaticValueInTrigger()
            : base(@"StaticValueInTrigger.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyDataTriggers);
            RunSteps += new TestStep(VerifyTriggers);
        }

        TestResult Setup()
        {
            Status("Setup");

            _cc1 = (ContentControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cc1"));
            _cc2 = (ContentControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cc2"));
            _cc3 = (ContentControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cc3"));

            _cb1 = (CheckBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cb1"));
            _cb2 = (CheckBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cb2"));
            _cb3 = (CheckBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "cb3"));

            return TestResult.Pass;
        }

        TestResult VerifyDataTriggers()
        {
            Status("VerifyDataTriggers");

            CheckBox checkBox1 = (CheckBox)(Util.FindDataVisual(_cc1, _cc1.Content));
            if (!VerifyIsCheckedAndFontWeight(checkBox1, false, "Normal"))
            {
                return TestResult.Fail;
            }

            CheckBox checkBox2 = (CheckBox)(Util.FindDataVisual(_cc2, _cc2.Content));
            if (!VerifyIsCheckedAndFontWeight(checkBox2, true, "Normal"))
            {
                return TestResult.Fail;
            }

            CheckBox checkBox3 = (CheckBox)(Util.FindDataVisual(_cc3, _cc3.Content));
            if (!VerifyIsCheckedAndFontWeight(checkBox3, false, "Bold"))
            {
                return TestResult.Fail;
            }
            
            return TestResult.Pass;
        }

        TestResult VerifyTriggers()
        {
            Status("VerifyTriggers");

            if (!VerifyIsCheckedAndFontWeight(_cb1, false, "Normal"))
            {
                return TestResult.Fail;
            }

            if (!VerifyIsCheckedAndFontWeight(_cb2, true, "Normal"))
            {
                return TestResult.Fail;
            }

            if (!VerifyIsCheckedAndFontWeight(_cb3, false, "Bold"))
            {
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        bool VerifyIsCheckedAndFontWeight(CheckBox cb, bool isChecked, string fontWeight)
        {
            if (cb.IsChecked != isChecked)
            {
                LogComment("Fail - Expected IsChecked: " + isChecked + ". Actual: " + cb.IsChecked);
                return false;
            }

            if (cb.FontWeight.ToString() != fontWeight)
            {
                LogComment("Fail - Expected FontWeight: " + fontWeight + ". Actual: " + cb.FontWeight);
                return false;
            }
            return true;
        }
    }
}


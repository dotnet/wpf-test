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
    /// Verifies when DisplayMemberPath is set, we throw when setting ItemTemplateSelector.  
	/// </description>
    /// <activeBugs>

    /// </activeBugs>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>

    [Test(2, "Controls", "DisplayMemberPathIgnored")]
	public class DisplayMemberPathIgnored : XamlTest
	{
        private ListBox _listBox;

        public DisplayMemberPathIgnored()
            : base(@"DisplayMemberPathIgnored.xaml")
		{
            InitializeSteps +=new TestStep(Setup);
            RunSteps += new TestStep(VerifyDisplayMemberPath);
            RunSteps += new TestStep(SetItemTemplateSelector);
            
            //RunSteps += new TestStep(SetItemTemplate);
        }

        TestResult Setup()
        {
            Status("Setup");
            _listBox = (ListBox)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb"));
            return TestResult.Pass;
        }

        TestResult VerifyDisplayMemberPath()
        {
            Status("VerifyDisplayMemberPath");

            TextBlock tb = (TextBlock)(Util.FindDataVisual(_listBox, _listBox.Items[0]));

            if (tb.Text != ((DateTime)(_listBox.Items[0])).Month.ToString())
            {
                LogComment("Fail - Expected text: " + ((DateTime)(_listBox.Items[0])).Month.ToString() + 
                    ". Actual: " + tb.Text);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult SetItemTemplateSelector()
        {
            Status("SetItemTemplateSelector");

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _listBox.ItemTemplateSelector = null;

            return TestResult.Fail;
        }

        TestResult SetItemTemplate()
        {
            Status("SetItemTemplate");

            SetExpectedErrorTypeInStep(typeof(InvalidOperationException));
            _listBox.ItemTemplate = null;

            return TestResult.Fail;
        }
    }
}


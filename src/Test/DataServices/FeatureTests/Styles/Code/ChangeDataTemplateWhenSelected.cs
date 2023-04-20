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
    /// When a ListBoxItem is selected, the colors of elements inside the DataTemplate change.
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(2, "Styles", "ChangeDataTemplateWhenSelected")]
	public class ChangeDataTemplateWhenSelected : XamlTest
	{
        private ListBox _lb1;

        public ChangeDataTemplateWhenSelected()
            : base(@"ChangeDataTemplateWhenSelected.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(ChangeTemplateWhenIsSelected);
        }

		private TestResult Setup()
		{
            Status("Setup");

            _lb1 = LogicalTreeHelper.FindLogicalNode(RootElement, "lb1") as ListBox;
            if (_lb1 == null)
            {
                LogComment("Fail - Not able to reference ListBox lb1");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // When a ListBoxItem is selected, the colors of elements
        // inside the DataTemplate change.
        private TestResult ChangeTemplateWhenIsSelected()
        {
            Status("ChangeTemplateWhenIsSelected");

            WaitForPriority(DispatcherPriority.Render);

            int selectedIndex = 1;
            _lb1.SelectedIndex = selectedIndex;

            int count = _lb1.Items.Count;
            for (int i = 0; i < count; i++)
            {
                StackPanel sp = Util.FindDataVisual(_lb1, _lb1.Items[i]) as StackPanel;
                TextBlock tb = sp.Children[0] as TextBlock;
                Button btn = sp.Children[1] as Button;

                if (i == selectedIndex)
                {
                    if (tb.Background != Brushes.Red)
                    {
                        LogComment("Fail - TextBlock's background should be red");
                        return TestResult.Fail;
                    }
                    if (btn.Background != Brushes.Cyan)
                    {
                        LogComment("Fail - Button's background should be cyan");
                        return TestResult.Fail;
                    }
                }
                else
                {
                }
            }

            return TestResult.Pass;
        }
    }
}


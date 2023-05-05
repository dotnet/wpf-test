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
    /// Tests using the value of an enumeration in the Value of a DataTrigger.
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>
    [Test(2, "Styles", "EnumerationInDataTrigger")]
	public class EnumerationInDataTrigger : XamlTest
	{
        private ListBox _lb;

        public EnumerationInDataTrigger()
            : base(@"EnumerationInDataTrigger.xaml")
		{
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(DataTriggersEnumeration);
        }

		private TestResult Setup()
		{
            Status("Setup");

            _lb = LogicalTreeHelper.FindLogicalNode(RootElement, "lb") as ListBox;
            if (_lb == null)
            {
                LogComment("Fail - Not able to reference ListBox lb");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        // Tests using the value of an enumeration in the Value of
        // a DataTrigger.
        private TestResult DataTriggersEnumeration()
        {
            Status("DataTriggersEnumeration");

            int count = _lb.Items.Count;
            for (int i = 0; i < count; i++)
            {
                SimpleBook simpleBook = _lb.Items[i] as SimpleBook;
                ListBoxItem lbi = _lb.ItemContainerGenerator.ContainerFromIndex(i) as ListBoxItem;
                if (simpleBook.Status == SimpleBookStatus.OutOfPrint)
                {
                    if (lbi.Foreground != Brushes.Red)
                    {
                        LogComment("Fail - ListBoxItem's foreground should be red - it is out of print");
                        return TestResult.Fail;
                    }
                }
                else
                {
                    if (lbi.Foreground == Brushes.Red)
                    {
                        LogComment("Fail - ListBoxItem's foreground should not be red - it is not out of print");
                        return TestResult.Fail;
                    }
                }
            }
            return TestResult.Pass;
        }
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
using System.Collections;
using Microsoft.Test;
using System.ComponentModel;
using System.Windows;
using System.Windows.Threading;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
	/// <summary>
	/// <description>
    /// Coverage for bug - Suggest coming up with a design for sorting a list of strings through markup
    /// After this fix, if PropertyName is not specified in the SortDescription, we compare the actual
    /// objects in the list if they implement IComparable (which strings do)
	/// </description>
	/// <relatedBugs>

    /// </relatedBugs>
	/// </summary>







    [Test(2, "Views", "SortIComparable")]
    public class SortIComparable : XamlTest
    {
        public SortIComparable()
            : base(@"SortIComparable.xaml")
        {
            RunSteps += new TestStep(VerifyOrderElements);
        }

        private TestResult VerifyOrderElements()
        {
            Status("VerifyOrderElements");

            ItemsControl itemsControl = (ItemsControl)(LogicalTreeHelper.FindLogicalNode(this.RootElement, "itemsControl"));
            GreekGods greekGods = (GreekGods)(this.RootElement.Resources["greekGods"]);

            // verify count
            int count = itemsControl.Items.Count;
            int expectedCount = 12;
            if (count != expectedCount)
            {
                LogComment("Fail - There should be " + expectedCount + "elements, instead there are " + count);
                return TestResult.Fail;
            }

            // verify order of items
            int j = 0;
            for (int i = count - 1; i >= 0; i--)
            {
                if (greekGods[j].ToString() != itemsControl.Items[i].ToString())
                {
                    LogComment("Fail - Expected item in index " + i + ": " + greekGods[j] + ". Actual: " + itemsControl.Items[i]);
                    return TestResult.Fail;
                }
                j++;
            }

            return TestResult.Pass;
        }
    }
}

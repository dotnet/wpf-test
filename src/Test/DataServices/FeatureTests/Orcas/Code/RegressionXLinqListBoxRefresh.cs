// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.Xml.Linq;
using Codeplex;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Adding item to XElement collection that listbox is bound to causes all listboxitems to be regenerated
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary> 
    [Test(2, "Xml", "RegressionXLinqListBoxRefresh", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionXLinqListBoxRefresh : XamlTest
    {
        #region Constructors

        public RegressionXLinqListBoxRefresh()
            // We can reuse the xaml file created for the XLinqPath testcase.
            : base(@"Markup\XLinqPath.xaml")
        {
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Verify()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            ListBox multipleElementsListBox = (ListBox)RootElement.FindName("MultipleElements");
            ListBoxItem originalListBoxItem = (ListBoxItem)multipleElementsListBox.ItemContainerGenerator.ContainerFromIndex(0);

            // Needed for slower machines (like VMs with low memory)
            int retryCount = 0;
            while (retryCount < 5 && (originalListBoxItem == null))
            {
                retryCount++;
                LogComment("Pausing then retrying originalListBoxItem (slow machine)... #" + retryCount + "/5");
                System.Threading.Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                originalListBoxItem = (ListBoxItem)multipleElementsListBox.ItemContainerGenerator.ContainerFromIndex(0);
            }
            XLinqDataProvider xldp = RootElement.FindResource("XLinqPath") as XLinqDataProvider;

            XElement xe = ((ObservableCollection<XElement>)xldp.Data)[0];
            xe.Add(new XElement("MultipleElements", new XElement("ElementValue", "C")));

            // This waits until the UI is updated after the add.
            WaitForPriority(DispatcherPriority.SystemIdle);

            object afterAddListBoxItem = multipleElementsListBox.ItemContainerGenerator.ContainerFromIndex(0);

            // Needed for slower machines (like VMs with low memory)
            retryCount = 0;
            while (retryCount < 5 && afterAddListBoxItem == null)
            {
                retryCount++;
                LogComment("Pausing then retrying (slow machine)... #" + retryCount + "/5");
                System.Threading.Thread.Sleep(1000);
                WaitForPriority(DispatcherPriority.SystemIdle);
                afterAddListBoxItem = multipleElementsListBox.ItemContainerGenerator.ContainerFromIndex(0);
            }

            if (!originalListBoxItem.Equals(afterAddListBoxItem))
            {
                TestLog.Current.LogEvidence("ListBoxItem for the item was regenerated.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
    }
}

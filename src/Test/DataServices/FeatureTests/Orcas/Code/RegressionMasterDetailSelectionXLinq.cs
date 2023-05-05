// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression Test - Selection not retained in master-detail over XLinq
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>   
    [Test(2, "Xml", "RegressionMasterDetailSelectionXLinq", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionMasterDetailSelectionXLinq : XamlTest
    {
        #region Constructors

        public RegressionMasterDetailSelectionXLinq()
            : base(@"Markup\RegressionMasterDetailSelectionXLinq.xaml")
        {
            RunSteps += new TestStep(Verify);
        }

        #endregion

        #region Private Members

        private TestResult Verify()
        {
            WaitForPriority(DispatcherPriority.Background);
            Util.WaitForXLinqDataProviderReady("XLinqPath", RootElement, 30);

            ListBox masterListBox = (ListBox)RootElement.FindName("MasterListBox");
            ListBox detailListBox = (ListBox)RootElement.FindName("DetailListBox");

            // Want this to fail if the listbox never populates, so let it time out.
            while (masterListBox.Items.Count <= 0)
            {
                TestLog.Current.LogEvidence("Waiting for Data Provider...");
                WaitForPriority(DispatcherPriority.SystemIdle);
                Thread.Sleep(1000);
            }

            // For indices 0 and 1 in the Master ListBox, set the Detail
            // ListBox's indices to 2 and 3 respectively. When we toggle
            // between the first and second index in the Master ListBox,
            // the Detail ListBox's SelectedIndex should retain memory.
            // Wait for everything to complete between actions since this is all async.
            masterListBox.SelectedIndex = 0;
            detailListBox.SelectedIndex = 2;
            masterListBox.SelectedIndex = 1;
            detailListBox.SelectedIndex = 3;
            masterListBox.SelectedIndex = 0;

            if (detailListBox.SelectedIndex != 2)
            {
                TestLog.Current.LogEvidence("Detail ListBox selected index was expected to be 2, actual was: " + detailListBox.SelectedIndex + ".", detailListBox.SelectedIndex);
                return TestResult.Fail;
            }

            masterListBox.SelectedIndex = 1;

            WaitForPriority(DispatcherPriority.SystemIdle);

            if (detailListBox.SelectedIndex != 3)
            {
                TestLog.Current.LogEvidence("Detail ListBox selected index was expected to be 3, actual was: " + detailListBox.SelectedIndex + ".", detailListBox.SelectedIndex);
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
    }
}

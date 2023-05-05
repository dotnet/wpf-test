// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using LocalClasses = Microsoft.Test.DataServices.RegressionTest2;
using Microsoft.Test.DataServices.RegressionTest3; // Useful datagrid manipulation helpers here
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Controls.Primitives;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.ObjectModel;
using System.Threading;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage - DataGrid crash when applying ScaleTransform to DataGridCell
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest5TargetElementNullRef", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.0GDR+")]
    public class RegressionTest5TargetElementNullRef : XamlTest
    {
        ObservableCollection<string> _strings;

        #region Private Data
        private ListBox _myListBox;
        #endregion

        #region Constructors

        public RegressionTest5TargetElementNullRef()
            : base(@"RegressionTest5TargetElementNullRef.xaml")
        {
            _strings = new ObservableCollection<string>();
            _strings.Add("first");
            _strings.Add("second");
            _strings.Add("third");

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myListBox = (ListBox)RootElement.FindName("listBox1");

            if (_myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }
            _myListBox.ItemsSource = _strings;

            while (_myListBox.Items.Count <= 0)
            {
                Thread.Sleep(500);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);

            // add a new string to the collection.  This will create a new container, including a binding
            // that uses ElementName to refer to something outside the ListBox.
            _strings.Add("added1");
            _strings.Add("added2");

            // now delete the new string.  This will remove the container.
            _strings.Remove("added1");
            _strings.Remove("added2");

            // force a GC, to reclaim the binding's target element before the VerifySourceReference task has run.
            // If the bug regresses this will throw.
            GC.GetTotalMemory(true);
            WaitForPriority(DispatcherPriority.SystemIdle);

            LogComment("Success, no exception thrown immediately removing added elements (regression test) ");
            return TestResult.Pass;
        }

        #endregion
    }
}


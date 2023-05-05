// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests master-detail scenario where one of the collections in the CompositeCollection is ADO.NET.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>



    [Test(1, "Collections", "CompositeCollectionADO")]
    public class CompositeCollectionADO : XamlTest
    {
        private ListBox _lb;
        private TextBlock _tb;

        public CompositeCollectionADO()
            : base("CompositeCollectionADO.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestMasterDetail);
        }

        TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.SystemIdle);

            _lb = LogicalTreeHelper.FindLogicalNode(this.RootElement, "lb") as ListBox;
            _tb = LogicalTreeHelper.FindLogicalNode(this.RootElement, "tb") as TextBlock;

            return TestResult.Pass;
        }

        TestResult TestMasterDetail()
        {
            Status("TestMasterDetail");
            _lb.SelectedIndex = 1;
            Util.AssertEquals(_tb.Text, "Redmond");
            // the following used to throw
            _lb.SelectedIndex = 11;
            Util.AssertEquals(_tb.Text, "Seattle");

            return TestResult.Pass;
        }
   }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Xml;
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
    /// Regression test for grouping to Test 
    /// that it possible to bind to the actual Groups
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>



    [Test(1, "Grouping", "BindToGroup")]
    public class BindToGroup : XamlTest 
    {
        TreeView _treeview;
        XmlDataProvider _ods;

        public BindToGroup()
            : base(@"BindToGroup.xaml")
        {
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(Step1);
        }
        TestResult Init()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            _treeview = LogicalTreeHelper.FindLogicalNode(RootElement, "treeview") as TreeView;

            if (_treeview == null)
            {
                LogComment("Couldn't find TreeView");
                return TestResult.Fail;
            }

            StackPanel _stackpanel = LogicalTreeHelper.FindLogicalNode(RootElement, "stackpanel") as StackPanel;

            _ods = _stackpanel.FindResource("DSO") as XmlDataProvider;
            DataSourceHelper.WaitForData(_ods);

            return TestResult.Pass;
        }

        TestResult Step1()
        {

            if (_treeview.Items.Count != 4)
            {
                LogComment("Wrong number of Groups!  Expected: 4 Actual: " + _treeview.Items.Count.ToString());
                return TestResult.Fail;
            }

            ArrayList ar = new ArrayList();
            ar.Add(1);
            ar.Add(2);
            ar.Add(3);
            ar.Add(4);

            for (int i = 0; i < ar.Count; i++)
            {
                if (((CollectionViewGroup)_treeview.Items[i]).Name.ToString() != ar[i].ToString())
                {
                    LogComment("Didn't Find expected Group " + ar[i].ToString() + "  Actual " + ((CollectionViewGroup)_treeview.Items[i]).Name.ToString());
                    return TestResult.Fail;
                }
            }
            
            return TestResult.Pass;
        }
    }    
}

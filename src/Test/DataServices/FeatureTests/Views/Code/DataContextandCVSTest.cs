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
	/// Testing to see that Setting DataContext with a CVS gets inherit
	/// Inital Sort applied in Markup with IsSynchronizedWithCurrentItem="true" 
    /// should alwasy select the first item.
	/// </description>
	/// <relatedBugs>


    /// </relatedBugs>
	/// </summary>



    [Test(1, "Views", "DataContextandCVSTest")]

    public class DataContextandCVSTest : XamlTest 
    {
        ListBox _listbox;
        ObjectDataProvider _odp;
    public DataContextandCVSTest()
            : base(@"DataContextandCVSTest.xaml")
        {

            InitializeSteps += new TestStep(Init);                
             RunSteps += new TestStep(Verify);
         }

        TestResult Init()
        {
            WaitForPriority(DispatcherPriority.SystemIdle);
            _listbox = LogicalTreeHelper.FindLogicalNode(RootElement, "lb1") as ListBox;
            if (_listbox == null)
            {
                LogComment("ListBox is null!");
                return TestResult.Fail;
            }

            _odp = RootElement.FindResource("DSO") as ObjectDataProvider;

            return TestResult.Pass;
        }

        TestResult Verify()
        {


            Status("Forcing FallbackValue to be applied");
            // The first item in the ListBox should be selected after a sort has been applied.
            if (_listbox.SelectedIndex != 0)
            {
                LogComment("First Item isn't selected!");
                return TestResult.Fail;
            }
            //LastRow in the collection should equal the Row in the ListBox
            // which should be selected
            if (((DataTable)_odp.Data).Rows[9] != ((DataRowView)_listbox.SelectedItem).Row)
            {
                LogComment("Sort wasn't applied!");
                return TestResult.Fail;
            }

            
            return TestResult.Pass;
        }



    }    

}

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

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for  - WPF DataGrid leaks memory when sorting in grouping mode
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "RegressionTest2DataGridRowLeaking", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class RegressionTest2DataGridRowLeaking : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private DataGrid _myDataGrid;

        #endregion

        #region Constructors

        public RegressionTest2DataGridRowLeaking() : base(@"RegressionTest2DataGridRowLeaking.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(DoAction);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel)RootElement.FindName("myStackPanel");
            _myDataGrid = (DataGrid)RootElement.FindName("myDataGrid");

            if (_myDataGrid == null || _myStackPanel == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private TestResult DoAction()
        {
            LogComment("Beginning validation: Ensuring that DataGridRows do not leak after grouping ()");

            _weakRef = new WeakReference((DataGridRow)_myDataGrid.ItemContainerGenerator.ContainerFromIndex(0), true);

            GroupStyle defaultGroupStyle = (GroupStyle)_myDataGrid.FindResource("gs_Default");
            _myDataGrid.GroupStyle.Add(defaultGroupStyle);

            _myDataGrid.Items.GroupDescriptions.Add(new PropertyGroupDescription("Band"));
            _myDataGrid.Items.SortDescriptions.Add(new SortDescription("Band", ListSortDirection.Ascending));

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            GC.Collect(3, GCCollectionMode.Forced);
            DataGridHelpers.WaitTillQueueItemsProcessed();

            GC.Collect(3, GCCollectionMode.Forced);
            DataGridHelpers.WaitTillQueueItemsProcessed();

            LogComment("WeakRef is " + (_weakRef.IsAlive ? "alive" : "dead") + " (should be dead)" );
            if (_weakRef.IsAlive == false)
            {
                LogComment("Success, reference to DataGridRow was cut on GC collect after sort ( regression test) ");
                return TestResult.Pass;
            }
            else
            {
                LogComment("Error, reference to DataGridRow was not cut on GC collect after sort ( regression test) ");
                return TestResult.Fail;
            }
        }

        WeakReference _weakRef;
        #endregion
    }
}

// Useful classes copied almost verbatim from the repro
// Scenario sorts a list of musicians by band...
namespace Microsoft.Test.DataServices.RegressionTest2
{
    public class Musician
    {
        public string Name { get; set; }

        public string Band { get; set; }

        public string this[long index]
        {
            get
            {
                return Name;
            }
            set
            {
                Name = value;
            }
        }
    }

    class MusicianRepository
    {
        private List<Musician> _persons;

        public List<Musician> Persons
        {
            get
            {
                return _persons;
            }

            set
            {
                this._persons = value;
            }
        }

        public MusicianRepository()
        {
            List<Musician> returnValue = new List<Musician>();
            returnValue.Add(new Musician() { Name = "Mark", Band = "Blink" });
            returnValue.Add(new Musician() { Name = "Tom", Band = "Blink" });
            returnValue.Add(new Musician() { Name = "Travis", Band = "Blink" });
            returnValue.Add(new Musician() { Name = "John", Band = "Beatles" });
            returnValue.Add(new Musician() { Name = "Paul", Band = "Beatles" });
            returnValue.Add(new Musician() { Name = "George", Band = "Beatles" });
            returnValue.Add(new Musician() { Name = "Ringo", Band = "Beatles" });
            returnValue.Add(new Musician() { Name = "Pete", Band = "Ex Beatles" });
            _persons = returnValue;
        }
    }
}

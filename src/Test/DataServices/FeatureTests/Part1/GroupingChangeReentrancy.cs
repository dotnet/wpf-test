// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Handle re-entrant access to grouped collection view during refresh
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Binding", SecurityLevel=TestCaseSecurityLevel.FullTrust, Versions="4.8+")]
    public class GroupingChangeReentrancy : XamlTest
    {
        #region Private Data

        private CollectionViewSource _cvs;

        #endregion

        #region Constructors

        public GroupingChangeReentrancy()
            : base(@"GroupingChangeReentrancy.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(InvokeReentrantAccess);
            RunSteps += new TestStep(RemoveItems);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _cvs = (CollectionViewSource)RootElement.FindResource("cvs");
            _cvs.Source = RepertoireRegressionTest15.Create();
            DataGrid dataGrid = (DataGrid)RootElement.FindName("dataGrid");
            dataGrid.SelectedIndex = 1;
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        private TestResult InvokeReentrantAccess()
        {
            // refresh the collection view (e.g. by changing shaping).  This
            // rebuilds the view.  While it's rebuilding, any re-entrant access
            // should give consistent results (in particular, it shouldn't crash).
            // Regression bug exposed such a re-entrant access - a DataGrid with
            // grouped data and a non-empty selection checks to see if cells
            // are still selected.
            _cvs.SortDescriptions.Add(new SortDescription("Composer", ListSortDirection.Ascending));

            // let WPF react
            WaitForPriority(DispatcherPriority.SystemIdle);

            // no crash => success
            return TestResult.Pass;
        }

        private TestResult RemoveItems()
        {
            // Regression bug found a similar reentrant crash when removing an item that
            // causes one or more groups to become empty.

            // add another layer of grouping, to test the case when multiple groups
            // become empty
            _cvs.GroupDescriptions.Add(new PropertyGroupDescription("Form"));
            WaitForPriority(DispatcherPriority.SystemIdle);

            // now remove the items one by one.  Some of these will cause
            // groups to become empty.
            RepertoireRegressionTest15 items = (RepertoireRegressionTest15)_cvs.Source;
            for (int i=items.Count-1; i>=0; --i)
            {
                items.RemoveAt(i);
                WaitForPriority(DispatcherPriority.SystemIdle);
            }

            // no crash => success
            return TestResult.Pass;
        }

        #endregion
    }

    #region Data

    public class OpusRegressionTest15
    {
        public string Title { get; set; }
        public string Composer { get; set; }
        public string Form { get; set; }
        public string Period { get; set; }

        public OpusRegressionTest15(string title, string composer, string form, string period)
        {
            Title = title;
            Composer = composer;
            Form = form;
            Period = period;
        }
    }

    public class RepertoireRegressionTest15 : ObservableCollection<OpusRegressionTest15>
    {
        public static RepertoireRegressionTest15 Create()
        {
            RepertoireRegressionTest15 list = new RepertoireRegressionTest15();
            list.Add(new OpusRegressionTest15("Bluebeard's Castle", "Bartok", "Opera", "Modern"));
            list.Add(new OpusRegressionTest15("Firebird", "Stravinsky", "Ballet", "Modern"));
            list.Add(new OpusRegressionTest15("Lulu", "Berg", "Opera", "Modern"));
            list.Add(new OpusRegressionTest15("Nutcracker", "Tchaikowsky", "Ballet", "Romantic"));
            list.Add(new OpusRegressionTest15("Rigoletto", "Verdi", "Opera", "Romantic"));
            list.Add(new OpusRegressionTest15("Rodeo", "Copland", "Ballet", "Modern"));
            list.Add(new OpusRegressionTest15("Tales of Hoffman", "Offenbach", "Ballet", "Romantic"));
            list.Add(new OpusRegressionTest15("Tosca", "Puccini", "Opera", "Romantic"));
            return list;
        }
    }

    #endregion
}


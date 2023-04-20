// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Data;
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
    /// Test lifetime of values returned from DataSet collection properties
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(1, "Binding", SecurityLevel=TestCaseSecurityLevel.FullTrust, Versions="4.8+")]
    public class DataSetCollectionPropertyLifetime : XamlTest
    {
        #region Private Data

        private Panel _panel;
        private TextBlock _parentCount,_childCount;
        private WeakReference _wrDataSet;
        private WeakReference _wrDataView;
        private WeakReference _wrRelatedView;

        #endregion

        #region Constructors

        public DataSetCollectionPropertyLifetime()
            : base(@"DataSetCollectionPropertyLifetime.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(VerifyLive);
            RunSteps += new TestStep(RemovePropertyDescriptorBindings);
            RunSteps += new TestStep(VerifyLive);
            RunSteps += new TestStep(ReplaceDataSet);
            RunSteps += new TestStep(VerifyReclaimed);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _panel       =     (Panel)RootElement.FindName("panel");
            _parentCount = (TextBlock)RootElement.FindName("parentCount");
            _childCount  = (TextBlock)RootElement.FindName("childCount");

            ReplaceDataSet();
            SaveWeakRefs();
            return TestResult.Pass;
        }

        private TestResult ReplaceDataSet()
        {
            // replace the DataSet with a new one
            _panel.DataContext = NewDataSet();

            // let WPF react
            WaitForPriority(DispatcherPriority.SystemIdle);

            return TestResult.Pass;
        }

        private void SaveWeakRefs()
        {
            // save weak refs to objects of interest
            _wrDataSet = new WeakReference(_panel.DataContext);
            _wrDataView = new WeakReference(BindingOperations.GetBindingExpression(_parentCount, TextBlock.TextProperty).ResolvedSource);
            _wrRelatedView = new WeakReference(BindingOperations.GetBindingExpression(_childCount, TextBlock.TextProperty).ResolvedSource);
        }

        private TestResult VerifyLive()
        {
            // after initial setup, ADO objects should be alive
            return Verify(true);
        }

        private TestResult VerifyReclaimed()
        {
            // after replacing the DataSet, ADO objects (from the first DataSet) should be GC'd
            return Verify(false);
        }

        private TestResult Verify(bool live)
        {
            TestResult result = TestResult.Pass;
            string comment = String.Format(" has {0}been GCd", live ? String.Empty : "not ");

            // run aggressive GC (twice, to be overly aggressive)
            Util.GetMemory();
            Util.GetMemory();

            // check whether objects are alive
            if (live == (_wrDataSet.Target == null))
            {
                LogComment("DataSet" + comment);
                result = TestResult.Fail;
            }
            if (live == (_wrDataView.Target == null))
            {
                LogComment("DataView" + comment);
                result = TestResult.Fail;
            }
            if (live == (_wrRelatedView.Target == null))
            {
                LogComment("RelatedView" + comment);
                result = TestResult.Fail;
            }

            return result;
        }

        private TestResult RemovePropertyDescriptorBindings()
        {
            // the original markup has bindings to the Count property of
            // a DataView and a RelatedView - two of the objects whose lifetime
            // we're testing.   We need those bindings in order to obtain the
            // objects (can't get them directly, since ADO creates new objects
            // every time someone asks).   But the objects are kept alive
            // by the PropertyDescriptor as long as those bindings are active.
            // To test the premature GC of regression test, remove those
            // bindings.
            BindingOperations.ClearBinding(_parentCount, TextBlock.TextProperty);
            BindingOperations.ClearBinding(_childCount, TextBlock.TextProperty);
            return TestResult.Pass;
        }

        private DataSet NewDataSet()
        {
            var dataSet = new DataSet();
            var parent = dataSet.Tables.Add("Parent");
            var parentId = parent.Columns.Add("ParentId", typeof(int));
            parent.Columns.Add("ParentName");
            parent.PrimaryKey = new[] {parentId};

            var child = dataSet.Tables.Add("Child");
            var childId = child.Columns.Add("ChildId", typeof(int));
            child.Columns.Add("ChildName");
            var childParentId = child.Columns.Add("ParentId", typeof(int));
            child.PrimaryKey = new[] {childId};

            var relation = parent.ChildRelations.Add(parentId, childParentId);
            relation.RelationName = "Children";

            parent.Rows.Add(1, "MyParent");
            child.Rows.Add(1, "MyChild", 1);

            return dataSet;
        }

        #endregion
    }
}


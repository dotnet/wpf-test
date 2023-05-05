// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// This test exercises code in the CollectionViewProxy.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>


    [Test(3, "Views", "ProxyViewTest")]
    public class ProxyViewTest : WindowTest
    {
        MyCollView _customview;
        int _collectionchangecount;
        int _currencychanging;
        int _currencychanged;

        public ProxyViewTest()
        {
            InitializeSteps += new TestStep(ProxyViewTest_InitializeSteps);
            RunSteps += new TestStep(CursorStep);
            RunSteps += new TestStep(AddCurrencyEvents);
            RunSteps += new TestStep(TestCurrencyEvents);
            RunSteps += new TestStep(CheckCurrencyEvents);
            RunSteps += new TestStep(RemoveCurrencyEvents);
            RunSteps += new TestStep(SortStep);
            RunSteps += new TestStep(FilterStep);
            RunSteps += new TestStep(MiscTest);
            RunSteps += new TestStep(CultureStep);
            RunSteps += new TestStep(DeferRefresh);
        }

        TestResult ProxyViewTest_InitializeSteps()
        {
            CollectionViewSource cvs = new CollectionViewSource();
            ((ISupportInitialize)cvs).BeginInit();
            cvs.CollectionViewType = typeof(MyCollView);
            cvs.Source = CreateDataTable();
            ((ISupportInitialize)cvs).EndInit();
            _customview = cvs.View as MyCollView;
            if (_customview == null)
            {
                LogComment("GetCustomView failed!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult CursorStep()
        {
            ArrayList ar = new ArrayList();
            IEnumerator e = (IEnumerator)((ICollectionView)_customview).GetEnumerator();
            while (e.MoveNext())
            {
                ar.Add(e.Current);
            }

            Status("Cursor should be on the first item in the collection");
            if (!(_customview.CurrentPosition == 0 ))
            {
                LogComment("Expecting Currency to be set on the first item. ");
                return TestResult.Fail;
            }

            Status("MoveCurrentToNext()");
            _customview.MoveCurrentToNext();
            if (_customview.CurrentPosition != 1)
            {
                LogComment("Expected CurrentPosition to be 0");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());

                return TestResult.Fail;
            }

            Status("MoveCurrentToLast()");
            _customview.MoveCurrentToLast();
            if (_customview.CurrentPosition != 10)
            {
                LogComment("Expected CurrentPosition to be 10");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());

                return TestResult.Fail;
            }

            Status("MoveCurrentToPrevious()");
            _customview.MoveCurrentToPrevious();
            if (_customview.CurrentPosition != 9)
            {
                LogComment("Expected CurrentPosition to be 9");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());
                return TestResult.Fail;
            }

            Status("MoveCurrentToFirst()");
            _customview.MoveCurrentToFirst();
            if (_customview.CurrentPosition != 0)
            {
                LogComment("Expected CurrentPosition to be 0");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());

                return TestResult.Fail;
            }


            Status("MoveCurrentTo(object)");
            _customview.MoveCurrentTo(ar[2]);
            if (_customview.CurrentPosition != 2)
            {
                LogComment("Expected CurrentPosition to be 2");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());

                return TestResult.Fail;
            }

            Status("MoveCurrentToPosition(int)");
            _customview.MoveCurrentToPosition(5);
            if (_customview.CurrentPosition != 5)
            {
                LogComment("Expected CurrentPosition to be 5");
                LogComment("Actual CurrentPosition: " + _customview.CurrentPosition.ToString());

                return TestResult.Fail;
            }


            Status("Contains");
            if (!_customview.Contains(ar[3]))
            {
                LogComment("Contains or CurrentItem failed");
                return TestResult.Fail;
            }


            Status("SourceCollection");
            if (((DataView)_customview.SourceCollection)[10] == ar[9])
            {
                LogComment("SourceCollection Failed!!");
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult AddCurrencyEvents()
        {
            Status("Adding Currency Events");
            _customview.CurrentChanged += new EventHandler(_customview_CurrentChanged);
            _customview.CurrentChanging += new CurrentChangingEventHandler(_customview_CurrentChanging);
            return TestResult.Pass;
        }

        TestResult TestCurrencyEvents()
        {
            _currencychanging = 0;
            _currencychanged = 0;

            _customview.MoveCurrentToNext();

            return TestResult.Pass;
        }

        TestResult CheckCurrencyEvents()
        {
            if (_currencychanging == 0 || _currencychanged == 0)
            {
                LogComment("Currency Events were not fired!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult RemoveCurrencyEvents()
        {
            Status("Removing Currency Events");
            _customview.CurrentChanged -= new EventHandler(_customview_CurrentChanged);
            _customview.CurrentChanging -= new CurrentChangingEventHandler(_customview_CurrentChanging);
            return TestResult.Pass;
        }

        TestResult SortStep()
        {
            Status("Getting Sort");

            IList<SortDescription> sort = _customview.SortDescriptions;
            if (sort == null)
            {
                LogComment("Sort must not return null");
                return TestResult.Fail;
            }
            Status("Setting Sort");
            // This collection view only works with a DataSet so it should DataSet sorting
            SetExpectedErrorTypeInStep(typeof(NotSupportedException));
            _customview.SortDescriptions.Clear();
            _customview.SortDescriptions.Add(new SortDescription());
            return TestResult.Fail;
        }

        TestResult FilterStep()
        {
            Status("Setting Filter");

            BogusFilter mvf = new BogusFilter("2", "=");
            _customview.Filter = new Predicate<object>(mvf.Contains);

            Status("Refresh View");
            _customview.Refresh();

            Status("Get Filter");
            Predicate<object> cfc = _customview.Filter;
            if (cfc == null)
            {

                LogComment("Filter Getter didn't work!");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult MiscTest()
        {
            Status("Checking IndexOf on CollectionView");
            int i = _customview.IndexOf(((DataView)((ICollectionView)_customview).SourceCollection)[2]);
            if (i != 2)
            {
                LogComment("IndexOf() did not return the correct value");
                return TestResult.Fail;
            }
            Status("Checking Count on CollectionView");
            if (_customview.Count != 11)
            {
                LogComment("Count did not return the correct value");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult CultureStep()
        {
            Status("Getting CultureInfo");
            CultureInfo _cultureinfo = _customview.Culture;

            if (_cultureinfo == null)
            {
                LogComment("Culture wasn't set to English");
                return TestResult.Fail;
            }

            Status("Setting CultureInfo");
            _customview.Culture = new CultureInfo(1041);

            if (_customview.Culture.EnglishName != "Japanese (Japan)")
            {
                LogComment("Culture wasn't set to Japanese");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        TestResult DeferRefresh()
        {
            // There is not logic written for DeferRefresh()
            // This is called purely for code coverage.
            _customview.DeferRefresh();
            return TestResult.Pass;
        }

        #region Events
        void _customview_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _collectionchangecount++;
        }
        void _customview_CurrentChanged(object sender, EventArgs e)
        {
            _currencychanged++;
        }

        void _customview_CurrentChanging(object sender, CurrentChangingEventArgs e)
        {
            _currencychanging++;
        }
        #endregion

        #region data
        private DataView CreateDataTable()
        {
            DataSet data = new DataSet();
            DataTable table = data.Tables.Add("Test");
            table.Columns.Add("Name", typeof(System.String));
            table.Columns.Add("Value", typeof(System.Int32));
            table.Columns.Add("Comment", typeof(System.String));

            for (int i = 0; i < 10; ++i)
            {
                DataRow row = table.NewRow();
                row["Name"] = "TableData " + i.ToString();
                row["Value"] = i;
                row["Comment"] = "Item " + i.ToString() + " created at " + DateTime.Now.ToShortTimeString();
                table.Rows.Add(row);
            }
            DataRow row1 = table.NewRow();
            row1["Name"] = "TableData differentdata";
            row1["Value"] = 1;
            row1["Comment"] = "Item stuff";
            table.Rows.Add(row1);
            return data.Tables[0].DefaultView;
        }

        #endregion
    }

    public class BogusFilter
    {
        public BogusFilter(string compareType, object FilterValue)
        {
            _compareType = compareType;
            _FilterValue = FilterValue;
        }

        // delegate DataListFilterCallback
        public bool Contains(object item)
        {
            GlobalLog.LogStatus("filtered Called!");

            if (_compareType == "=")
                return Comparer.Default.Compare((int)((DataRowView)item)[1], int.Parse((string)_FilterValue)) == 0;
            else if (_compareType == ">")
                return Comparer.Default.Compare((int)((DataRowView)item)[1], int.Parse((string)_FilterValue)) > 0;
            else if (_compareType == "<")
                return Comparer.Default.Compare((int)((DataRowView)item)[1], int.Parse((string)_FilterValue)) < 0;
            else
                return false;
        }

        object _FilterValue;
        string _compareType;
    }
}

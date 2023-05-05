// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Controls;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using System.Collections;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices {

    /// <summary>
    /// <description>
    /// Test Adding, Inserting, Removing from ItemCollection after
    /// a sort of filter has been appiled.
    /// </description>
    /// <relatedBugs>

    /// </relatedBugs>
    /// </summary>
    [Test(2, "Views", "ItemsCollectionSortFilterTest")]
    public class ItemsCollectionSortFilterTest : WindowTest
    {
        ListBox _lb;
        FileItem _removableobj;
        FileItem _addfileitemobj;
        FileItem _insertfileitemobj;
        object _beforeMoveObj = null;
        object _afterMoveObj = null;
        CurrentChangingVerifier _ccgv;
        CurrentChangedVerifier _ccdv;


        public ItemsCollectionSortFilterTest()
        {
            InitializeSteps += new TestStep(init);
            RunSteps += new TestStep(addaftersort);
            RunSteps += new TestStep(addafterfilter);
            RunSteps += new TestStep(clear);
            RunSteps += new TestStep(addnewitems);
            RunSteps += new TestStep(currency);
        }

        private TestResult init()
        {
            DockPanel dp = new DockPanel();

            _lb = new ListBox();
            _lb.Items.Add(new FileItem("Afile.cxx", 2175, "12/7/03"));
            _lb.Items.Add(new FileItem("Cfile.hxx", 2175, "12/9/03"));
            _lb.Items.Add(new FileItem("Zfile.cs", 2175, "12/8/03"));
            _lb.Items.Add(new FileItem("Xile2.cs", 2175, "12/6/03"));
            _lb.Items.Add(new FileItem("IFile5.xml", 2175, "12/4/03"));
            //To be used throught the test
            _removableobj = new FileItem("Removefile.xml", 2173, "12/4/03");
            _addfileitemobj = new FileItem("Addfile.xml", 2173, "12/4/03");
            _insertfileitemobj = new FileItem("Inserterfile.xml", 2173, "4/12/05");
            _lb.Items.Add(_removableobj);
            //Events
            _ccgv = new CurrentChangingVerifier((CollectionView)_lb.Items.SourceCollection);
            _ccdv = new CurrentChangedVerifier((CollectionView)_lb.Items.SourceCollection);

            // Template
            DataTemplate dt = new DataTemplate();
            FrameworkElementFactory panel = new FrameworkElementFactory(typeof(DockPanel));

            FrameworkElementFactory text = new FrameworkElementFactory(typeof(TextBlock));
            text.SetValue(TextBlock.MarginProperty, new Thickness(2));
            text.SetBinding(TextBlock.TextProperty, new Binding("DateModified"));
            panel.AppendChild(text);

            text = new FrameworkElementFactory(typeof(TextBlock));
            text.SetValue(TextBlock.MarginProperty, new Thickness(2));
            text.SetValue(TextBlock.ForegroundProperty, Brushes.Blue);
            text.SetBinding(TextBlock.TextProperty, new Binding("Name"));
            panel.AppendChild(text);

            text = new FrameworkElementFactory(typeof(TextBlock));
            text.SetValue(TextBlock.MarginProperty, new Thickness(2));
            text.SetBinding(TextBlock.TextProperty, new Binding("Size"));
            panel.AppendChild(text);

            dt.VisualTree = panel;

            _lb.ItemTemplate = dt;
            dp.Children.Add(_lb);
            Window.Content = dp;

            _afterMoveObj = _lb.Items[0];
            _lb.Items.MoveCurrentToFirst();

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);
            return TestResult.Pass;
        }
        private TestResult addaftersort()
        {

            Status("checking sort");

            sort();
            if (_lb.Items.NeedsRefresh)
            {
                LogComment("NeedsRefresh was set to true after sort!");
                return TestResult.Fail;
            }

            Status("adding new item after sort");
            // These items shouldn't sort until Refresh is called
            if (_lb.Items.Add(_addfileitemobj) != 6)
            {
                LogComment("Adding Item after sort wasn't added to the end of the list");
                return TestResult.Fail;
            }
            if (!_lb.Items.NeedsRefresh)
            {
                LogComment("NeedsRefresh wasn't set to true after adding new item!");
                return TestResult.Fail;
            }

            if (!currencycheck(5))
                return TestResult.Fail;

            _lb.Items.Insert(0, _insertfileitemobj);

            // Sort caused the cursor change events to fire a second time
            // resetting the curor to the same item.
            _beforeMoveObj = _afterMoveObj;
            if (!checkchangedevents(2))
                return TestResult.Fail;

            if (!currencycheck(6))
                return TestResult.Fail;

            if (!verify(7, 0, 3))
                return TestResult.Fail;

            Status("Removing Sort then Refreshing ItemCollection");
            _lb.Items.SortDescriptions.RemoveAt(0);

            if (!checkchangedevents(1))
                return TestResult.Fail;

            if (!currencycheck(0))
                return TestResult.Fail;

            if (!verify(6, 7, 5))
                return TestResult.Fail;
            _lb.Items.Refresh();

            return TestResult.Pass;
        }
        private TestResult addafterfilter()
        {
            Status("checking filter");

            filter();
            if (_lb.Items.NeedsRefresh)
            {
                LogComment("NeedsRefresh was set to true after filter");
                return TestResult.Fail;
            }

            Status("adding new item after filter");
            // These items shouldn't be filtered until Refresh is called
            if (_lb.Items.Add(_addfileitemobj) != 5)
            {
                LogComment("Adding Item after filter wasn't added to the end of the list");
                return TestResult.Fail;
            }
            if (!_lb.Items.NeedsRefresh)
            {
                LogComment("NeedsRefresh wasn't set to true after adding new item after filtering!");
                return TestResult.Fail;
            }

            if (!currencycheck(0))
                return TestResult.Fail;

            _lb.Items.Insert(0, _insertfileitemobj);

            if (!checkchangedevents(1))
                return TestResult.Fail;

            if (!currencycheck(1))
                return TestResult.Fail;

            if (!verify(6, 0, -1))
                return TestResult.Fail;

            //Remove() should remove from the underling collection.
            _lb.Items.Remove(_removableobj);
            //RemoveAt should remove from the view ( (0) was the insertfileobj)
            _lb.Items.RemoveAt(0);

            Status("Removing Filter then Refreshing ItemCollection");
            _lb.Items.Filter = null;
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);

            if (!checkchangedevents(1))
                return TestResult.Fail;
            if (!currencycheck(0))
                return TestResult.Fail;

            if (!verify(6, 7, -1))
                return TestResult.Fail;
            _lb.Items.Refresh();
            return TestResult.Pass;
        }
        private TestResult clear()
        {
            Status("Clearing to check Sort & Filter are retained");
            // After doing Clear() the Sort and Filter should still exist.
            sort();
            filter();
            _lb.Items.Clear();
            if (!_lb.Items.IsEmpty)
            {
                LogComment("Items.IsEmpty is false after Clear()!");
                return TestResult.Fail;
            }
            if (_lb.Items.Count != 0)
            {
                LogComment("Count after Clear() isn't 0!");
                return TestResult.Fail;
            }
            _afterMoveObj = null;
            if (!checkchangedevents(3))
                return TestResult.Fail;
            if (!currencycheck(-1))
                return TestResult.Fail;
            return TestResult.Pass;
        }
        private TestResult addnewitems()
        {
            // Adding new items to validate the Sort and Fitler are still applied
            _lb.Items.Add(_removableobj);
            _lb.Items.Add(new FileItem("Afile.cxx", 2173, "12/7/03"));
            _lb.Items.Add(new FileItem("Xile2.cs", 2173, "12/6/03"));

            _lb.Items.Refresh();
            if (!_lb.Items.IsEmpty)
            {
                LogComment("Items.IsEmpty is false after Filter out all values!");
                return TestResult.Fail;
            }


            _lb.Items.Add(new FileItem("Cfile.hxx", 2175, "12/9/03"));
            _lb.Items.Add(new FileItem("Zfile.cs", 2175, "12/8/03"));
            _lb.Items.Add(new FileItem("IFile5.xml", 2175, "12/4/03"));
            _lb.Items.Refresh();
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);


            FrameworkElement[] fwe = Util.FindDataVisuals(_lb, _lb.Items.SourceCollection);
            ArrayList al = new ArrayList();
            al.Add("Zfile");
            al.Add("IFile5");
            al.Add("Cfile");
            for (int i = 0;i < fwe.Length; i++)
            {
                if (al[i].ToString() != ((FileItem)fwe[i].DataContext).Name)
                {
                    LogComment("Sort was lost after Clearing the ItemCollection");
                    return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }
        private TestResult currency()
        {

             _lb.Items.MoveCurrentToLast();
            //
             if (!currencycheck(2))
                 return TestResult.Fail;
            _lb.Items.RemoveAt(2);


            _lb.Items.MoveCurrentTo(_removableobj);
            if (!currencycheck(-1))
                return TestResult.Fail;

            _lb.Items.Add(_addfileitemobj);
            _lb.Items.MoveCurrentToPosition(3);
            if (!currencycheck(3))
                return TestResult.Fail;

            _lb.Items.Insert(1, _insertfileitemobj);

            if (!currencycheck(4))
                return TestResult.Fail;

            _lb.Items.MoveCurrentTo(_insertfileitemobj);

            if (!currencycheck(1))
                return TestResult.Fail;

            _lb.Items.Remove(_removableobj);

            using (_lb.Items.DeferRefresh())
            {
                _lb.Items.Filter = null;
            }

            if (!currencycheck(2))
                return TestResult.Fail;
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);

            return TestResult.Pass;
        }

#region Helpers
        private void sort()
        {
            _lb.Items.SortDescriptions.Add(new SortDescription("Name", ListSortDirection.Descending));
        }
        private void filter()
        {
            FileItemFilter fileitemfilter = new FileItemFilter(2175);
            _lb.Items.Filter = new Predicate<object>(fileitemfilter.Contains);
        }
        private bool verify(int add, int insert, int remove)
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Render);
            int _add = -1;
            int _insert = -1;
            int _remove = -1;
            int position = 0;
            FrameworkElement[] fwe = Util.FindDataVisuals(_lb, _lb.Items.SourceCollection);
            foreach (FrameworkElement e in fwe)
            {
                if (((FileItem)e.DataContext) == _addfileitemobj)
                    _add = position;
                if (((FileItem)e.DataContext) == _insertfileitemobj)
                    _insert = position;
                if (((FileItem)e.DataContext) == _removableobj)
                    _remove = position;
                position++;
            }

            if (add != _add || insert != _insert || remove != _remove)
            {
                GlobalLog.LogEvidence("Object not in expected position!");
                GlobalLog.LogEvidence("AddFileObj index " + _add.ToString());
                GlobalLog.LogEvidence("insertFileObj index " + _insert.ToString());
                GlobalLog.LogEvidence("RemoveFileObj index " + _remove.ToString());
                return false;
            }
            return true;
        }
        private bool checkchangedevents(int timesfired)
        {
            bool returnvalue = true;
            IVerifyResult ivr1 = _ccgv.Verify((CollectionView)_lb.Items.SourceCollection, timesfired, _beforeMoveObj);
            if (ivr1.Result != TestResult.Pass)
            {
                LogComment(ivr1.Message);
                returnvalue = false;
            }

            IVerifyResult ivr2 = _ccdv.Verify((CollectionView)_lb.Items.SourceCollection, timesfired, _afterMoveObj);
            if (ivr2.Result != TestResult.Pass)
            {
                LogComment(ivr2.Message);
                returnvalue = false;
            }
            return returnvalue;

        }
        private bool currencycheck(int i)
        {

            if (_lb.Items.CurrentPosition != i)
            {
                GlobalLog.LogEvidence("Cursor index in unexpectect position!  Expected: " + i.ToString() + " Actual: " + _lb.Items.CurrentPosition.ToString());
                return false;
            }
            else
            {
                return true;
            }
        }
#endregion
    }

      public class FileItemFilter
    {
        public FileItemFilter(int size)
        {
            _size = size;
        }

        // delegate DataListFilterCallback
        public bool Contains(object item)
        {
            FileItem file = (FileItem)item;
            return (file.Size == _size);
        }

        int _size;
    }


}

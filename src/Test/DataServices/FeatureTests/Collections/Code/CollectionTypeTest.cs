// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
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
using Microsoft.Test.Diagnostics;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Collection tests for different collection stores
    /// </description>
    /// </summary>

    [Test(2, "Collections", "CollectionTypeTest")]
    public class CollectionTypeTest : XamlTest
    {

        #region Test case members

        private ArrayList _datasources;
        private Library _library = null;
        private LibraryEnumerable _libEnumer = null;
        private LibraryGenericList _libGen = null;
        private LibraryQueue _libQue = null;
        private LibraryStack _libStack = null;
        private LibraryEnumerableNotify _libEnumerNotify = null;
        private ItemsControl _itemsControl = null;

        #endregion

        #region accessors

        internal ArrayList DataSources
        {
            get { return _datasources; }
        }

        internal Library LibraryData
        {
            get { return _library; }
            set { _library = value; }
        }

        internal LibraryEnumerable LibraryEnumerableData
        {
            get { return _libEnumer; }
            set { _libEnumer = value; }
        }

        internal LibraryGenericList LibraryGenericListData
        {
            get { return _libGen; }
            set { _libGen = value; }
        }

        internal LibraryQueue LibraryQueueData
        {
            get { return _libQue; }
            set { _libQue = value; }
        }

        internal LibraryStack LibraryStackData
        {
            get { return _libStack; }
            set { _libStack = value; }
        }

        internal LibraryEnumerableNotify LibraryEnumerNotifyData
        {
            get { return _libEnumerNotify; }
            set { _libEnumerNotify = value; }
        }

        internal ItemsControl ItemsControlElement
        {
            get { return _itemsControl; }
            set { _itemsControl = value; }
        }

        #endregion

        #region Constructor

        public CollectionTypeTest() : this("CollectionALDC.xaml") { }

        [Variation("CollectionALDC.xaml")]
        [Variation("CollectionGenericList.xaml")]
        [Variation("CollectionIEnumerable.xaml")]
        [Variation("CollectionIEnumerableNotify.xaml")]
        [Variation("CollectionMixed.xaml")]
        [Variation("CollectionQueue.xaml")]
        [Variation("CollectionStack.xaml")]
        public CollectionTypeTest(string fileName) : base(fileName)
        {
            bool regressionBugFlag = (SystemInformation.WpfVersion == WpfVersions.Wpf30);
            InitializeSteps += new TestStep(FindDataSources);
            RunSteps += new TestStep(Verify);

            if (regressionBugFlag && (fileName == "CollectionMixed.xaml"))
            {
                Console.WriteLine("**\nRegression Bug prevents composite collections from testing Add & Remove\n**");
            }
            else
            {
                RunSteps += new TestStep(AddToEach);
                RunSteps += new TestStep(Verify);
                RunSteps += new TestStep(RemoveOneFromEach);
                RunSteps += new TestStep(Verify);
                //RunSteps += new TestStep(SortThem);
                //RunSteps += new TestStep(ThreadTest);
            }
        }


        #endregion

        #region Test Steps

        TestResult FindDataSources()
        {
            Status("Setting up for the test");
            _datasources = new ArrayList();

            Style style = null;

            DictionaryEntry entry;
            IDictionaryEnumerator resources = RootElement.Resources.GetEnumerator();

            while (resources.MoveNext())
            {
                entry = (DictionaryEntry)resources.Current;
                if (entry.Value is ObjectDataProvider)
                {
                    DataSources.Add(entry.Value);
                }
                if (entry.Value is Style)
                {
                    style = (Style)entry.Value;
                }
            }

            foreach (ObjectDataProvider dso in DataSources)
            {
                LogComment(dso.Data.ToString());
                if (dso.Data != null)
                {
                    if (dso.Data is Library)
                    {
                        LibraryData = (Library)dso.Data;
                    }
                    if (dso.Data is LibraryEnumerable)
                    {
                        LibraryEnumerableData = (LibraryEnumerable)dso.Data;
                    }
                    if (dso.Data is LibraryGenericList)
                    {
                        LibraryGenericListData = (LibraryGenericList)dso.Data;
                    }
                    if (dso.Data is LibraryQueue)
                    {
                        LibraryQueueData = (LibraryQueue)dso.Data;
                    }
                    if (dso.Data is LibraryStack)
                    {
                        LibraryStackData = (LibraryStack)dso.Data;
                    }
                    if (dso.Data is LibraryEnumerableNotify)
                    {
                        LibraryEnumerNotifyData = (LibraryEnumerableNotify)dso.Data;
                    }
                }
            }

            ItemsControlElement = (ItemsControl)Util.FindElement(RootElement, "testListBox");
            if (ItemsControlElement == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }
            LogComment("Setup was completed successfully");
            return TestResult.Pass;
        }


        TestResult Verify()
        {
            LogComment("Validating the items in the ItemsControl");
            return ValidateItems();
        }

        TestResult AddToEach()
        {
            Status("Adding an item to each datasource");
            if (LibraryData != null)
            {
                Status("added 1 to the library");
                LibraryData.Add(new Book("New Library Book", "1111", "Library Author", "Library Publisher", 22.22, Book.BookGenre.Comic));
            }
            if (LibraryEnumerableData != null)
            {
                Status("Will not add to IEnumerable");

            }
            if (LibraryGenericListData != null)
            {
                Status("added 1 to the generic list");
                LibraryGenericListData.Add(new Book("New LibraryGeneric Book", "3333", "LibraryGeneric Author", "LibraryGeneric Publisher", 11.11, Book.BookGenre.Mystery));
            }
            if (LibraryQueueData != null)
            {
                Status("added 1 to the queue");
                LibraryQueueData.Enqueue(new Book("New LibraryQueue Book", "5555", "LibraryQueue Author", "LibraryQueue Publisher", 33.33, Book.BookGenre.Romance));
            }
            if (LibraryStackData != null)
            {
                Status("added 1 to the stack");
                LibraryStackData.Push(new Book("New LibraryStack Book", "7777", "LibraryStack Author", "LibraryStack Publisher", 55.55, Book.BookGenre.SelfHelp));
            }
            if (LibraryEnumerNotifyData != null)
            {
                Status("added 1 to the notify enumerable");
                LibraryEnumerNotifyData.Add(new Book("New LibraryEnumerNotifyData Book", "8888", "LibraryEnumerNotifyData Author", "LibraryEnumerNotifyData Publisher", 66.66, Book.BookGenre.Reference));
            }

            LogComment("Added 1 new book to each datasource");
            return TestResult.Pass;
        }

        TestResult RemoveOneFromEach()
        {
            Status("Removing an item from each datasource");
            if (LibraryData != null)
            {
                Status("removed 1 from the library");
                LibraryData.RemoveAt(0);
            }
            if (LibraryEnumerableData != null)
            {
                Status("Will not remove from IEnumerable");
                //Status("removed 1 from the enumerable");
                //LibraryEnumerableData.RemoveAt(0);
            }
            if (LibraryGenericListData != null)
            {
                Status("removed 1 from the generic list");
                LibraryGenericListData.RemoveAt(0);
            }
            if (LibraryQueueData != null)
            {
                Status("removed 1 from the queue");
                LibraryQueueData.Dequeue();
            }
            if (LibraryStackData != null)
            {
                Status("removed 1 from the stack");
                LibraryStackData.Pop();
            }
            if (LibraryEnumerNotifyData != null)
            {
                Status("removed 1 from the notify enumerable");
                LibraryEnumerNotifyData.RemoveAt(0);
            }

            LogComment("Removed 1 book from each datasource");
            return TestResult.Pass;
        }

        TestResult SortThem()
        {

            ICollectionView cv = CollectionViewSource.GetDefaultView(ItemsControlElement.Items);
            if (cv == null)
            {
                LogComment("Could not reference CollectionView");
                return TestResult.Fail;
            }

            using (cv.DeferRefresh())
            {
                //cv.SortDescriptions.Clear();
                cv.SortDescriptions.Add(new SortDescription("Price", System.ComponentModel.ListSortDirection.Ascending));
            }

            return TestResult.Pass;
        }

        TestResult ThreadTest()
        {
            WaitFor(2000);

            Thread testThread = new Thread(new ThreadStart(OtherThread));

            testThread.Start();

            WaitFor(2000);


            return TestResult.Pass;
        }

        public void OtherThread()
        {
            Status("Adding an item to each datasource");
            if (LibraryData != null)
            {
                Status("added 1 to the library");
                LibraryData.Add(new Book("Other Thread", "1111", "Library Author", "Library Publisher", 22.22, Book.BookGenre.Comic));
            }
            if (LibraryEnumerableData != null)
            {
                Status("Can not add to the enumerable");
            }
            if (LibraryGenericListData != null)
            {
                Status("added 1 to the generic list");
                LibraryGenericListData.Add(new Book("Other Thread", "3333", "LibraryGeneric Author", "LibraryGeneric Publisher", 11.11, Book.BookGenre.Mystery));
            }
            if (LibraryQueueData != null)
            {
                Status("added 1 to the queue");
                LibraryQueueData.Enqueue(new Book("Other Thread", "5555", "LibraryQueue Author", "LibraryQueue Publisher", 33.33, Book.BookGenre.Romance));
            }
            if (LibraryStackData != null)
            {
                Status("added 1 to the stack");
                LibraryStackData.Push(new Book("Other Thread", "7777", "LibraryStack Author", "LibraryStack Publisher", 55.55, Book.BookGenre.SelfHelp));
            }
            LogComment("Added 1 new book to each datasource");
        }


        private TestResult ValidateItems()
        {
            if (LibraryEnumerableData != null)
            {
                //LibraryEnumerable does not fire collection changed event, so force the update
                ItemsControlElement.Items.Refresh();
            }
            WaitForPriority(DispatcherPriority.Background);
            WaitFor(100);

            ArrayList dataContexts = new ArrayList();
            foreach (object o in ItemsControlElement.Items)
            {
                dataContexts.Add(o);
            }

            FrameworkElement[] items = Util.FindElements(RootElement, "BookItem");

            if (items == null)
            {
                LogComment("Items are null");
                return TestResult.Fail;
            }
            if (dataContexts.Count != items.Length)
            {
                LogComment("Number of objects and number of items are different. Expected: " + dataContexts.Count.ToString() + " Actual: " + items.Length.ToString());
                return TestResult.Fail;
            }

            bool pass = true;
            for (int i = 0; i < dataContexts.Count; i++)
            {
                if (!Util.CompareObjects(items[i].DataContext, dataContexts[i]))
                {
                    pass = false;
                    LogComment("Item" + i.ToString() + " had the incorrect datacontext. Expected: " + dataContexts[i].ToString() + " Actual: " + items[i].DataContext.ToString());
                }
                else
                {
                    Status("Item " + i.ToString() + " was ok");
                }
            }
            if (pass)
            {
                LogComment("Items had the correct datascontext");
                return TestResult.Pass;
            }
            else
                return TestResult.Fail;
        }

        #endregion



    }
}

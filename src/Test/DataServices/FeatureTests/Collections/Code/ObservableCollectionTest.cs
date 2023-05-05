// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
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
    /// Test ObservableCollection API to improve code coverage.
    /// </description>
    /// </summary>

    [Test(2, "Collections", "ObservableCollectionTest")]
    public class ObservableCollectionTest : XamlTest
    {

        Library _library = null;
        ObservableCollection<Book> _oc;
        NotifyCollectionChangedAction _lastAction;
        ListBox _lb = null;
        ICollectionView _lvs = null;


        public ObservableCollectionTest()
            : base("CollectionALDC.xaml")
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(SetUpVerify);
            RunSteps += new TestStep(CopyClear);
            RunSteps += new TestStep(CopyClearVerify);
            RunSteps += new TestStep(CopyBack);
            RunSteps += new TestStep(CopyBackVerify);
            RunSteps += new TestStep(SortIt);
            RunSteps += new TestStep(SortItVerify);
            RunSteps += new TestStep(TrimIt);

            RunSteps += new TestStep(MoveItem);
            RunSteps += new TestStep(ReplaceItem);

            RunSteps += new TestStep(SortView);
            RunSteps += new TestStep(MoveItemSort);
            RunSteps += new TestStep(ReplaceItemSort);

            RunSteps += new TestStep(GroupView);
            RunSteps += new TestStep(MoveItemGroup);
            RunSteps += new TestStep(ReplaceItemGroup);


        }


        TestResult SetUp()
        {
            ObjectDataProvider dso = RootElement.Resources["DSO"] as ObjectDataProvider;
            if (dso == null)
            {
                LogComment("Could not reference ObjectDataSource");
                return TestResult.Fail;
            }

            _library = (Library)dso.Data;
            if (_library == null)
            {
                LogComment("Library was null");
                return TestResult.Fail;
            }

            _lb = (ListBox)Util.FindElement(RootElement, "testListBox");
            if (_lb == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }

            _library.CollectionChanged += new NotifyCollectionChangedEventHandler(library_CollectionChanged);

            return TestResult.Pass;

        }
        TestResult SetUpVerify()
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 0",
                                            "Title of book 1",
                                            "Title of book 2",
                                            "Title of book 3",
                                            "Title of book 4",
                                            "Title of book 5",
                                            "Title of book 6",
                                            "Title of book 7",
                                            "Title of book 8",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Setup");
        }

        TestResult CopyClear()
        {
            Status("Creating a copy of the ObservableCollection using the constructor");

            List<Book> list = new List<Book>();

            foreach (Book book in _library)
            {
                list.Add(book);
            }

            _oc = new ObservableCollection<Book>(list);

            if (_oc.Count != 10)
            {
                LogComment("Copied ObservableCollection did not contain 10 items, it had " + _oc.Count.ToString());
                return TestResult.Fail;
            }
            LogComment("Created a copy of the ObservableCollection using the constructor");

            Status("Clearing the Library");

            _library.Clear();

            string[] expected = new string[] { };

            LogComment("Library is cleared");

            return VerifyContent(expected, "Verify Cleared");
        }

        TestResult CopyClearVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            if (_lastAction != NotifyCollectionChangedAction.Reset)
            {
                LogComment("Expected Reset for last action, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            if (_lb.Items.Count != 0)
            {
                LogComment("ListBox did not have 0 items, it had " + _lb.Items.Count.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult CopyBack()
        {
            Status("Copy back the items from the ObservableList to the Library");

            foreach (Book book in _oc)
            {
                _library.Add(book);
            }

            if (_library.Count != 10)
            {
                LogComment("Library did not contain 10 items, it had " + _library.Count.ToString());
                return TestResult.Fail;
            }

            LogComment("Copied back the items from the ObservableList to the Library");
            return TestResult.Pass;
        }

        TestResult CopyBackVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            if (_lastAction != NotifyCollectionChangedAction.Add)
            {
                LogComment("Expected Add for last action, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 0",
                                            "Title of book 1",
                                            "Title of book 2",
                                            "Title of book 3",
                                            "Title of book 4",
                                            "Title of book 5",
                                            "Title of book 6",
                                            "Title of book 7",
                                            "Title of book 8",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Setup");
        }

        TestResult SortIt()
        {
            _library.Sort();
            return TestResult.Pass;
        }

        TestResult SortItVerify()
        {
            WaitForPriority(DispatcherPriority.Background);
            if (_lastAction != NotifyCollectionChangedAction.Reset)
            {
                LogComment("Expected Reset for last action, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 8",
                                           "Title of book 6",
                                            "Title of book 4",
                                            "Title of book 2",
                                            "Title of book 0",
                                            "Title of book 1",
                                            "Title of book 3",
                                            "Title of book 5",
                                            "Title of book 7",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Sort on Library");
        }

        TestResult TrimIt()
        {
            _library.TrimExcess();

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 8",
                                           "Title of book 6",
                                            "Title of book 4",
                                            "Title of book 2",
                                            "Title of book 0",
                                            "Title of book 1",
                                            "Title of book 3",
                                            "Title of book 5",
                                            "Title of book 7",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Trim");
        }

        TestResult MoveItem()
        {
            _library.Move(2, 7);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 8",
                                           "Title of book 6",
                                            "Title of book 2",
                                            "Title of book 0",
                                            "Title of book 1",
                                            "Title of book 3",
                                            "Title of book 5",
                                            "Title of book 4",
                                            "Title of book 7",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Move");
        }

        TestResult ReplaceItem()
        {
            Book b = new Book("Replaced Book", "2303", "Replace Author", "Replace Publisher", 11.55, Book.BookGenre.Romance);
            
            _library[3] = b;

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 8",
                                           "Title of book 6",
                                            "Title of book 2",
                                            "Replaced Book",
                                            "Title of book 1",
                                            "Title of book 3",
                                            "Title of book 5",
                                            "Title of book 4",
                                            "Title of book 7",
                                            "Title of book 9"};
            return VerifyContent(expected, "Verify Replace");
        }

        //Change the view 

        TestResult SortView()
        {

            _lvs = (ICollectionView)_lb.Items;
            
            if (_lvs == null)
            {
                LogComment("Could not find reference to the CollectionViewSource");
                return TestResult.Fail;
            }

            using (_lvs.DeferRefresh())
            {
                _lvs.SortDescriptions.Clear();
                _lvs.SortDescriptions.Add(new SortDescription("Genre", ListSortDirection.Descending));
                _lvs.SortDescriptions.Add(new SortDescription("Title", ListSortDirection.Descending));
            }

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 4",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Title of book 7",
                                            "Title of book 1",
                                            "Title of book 6"};
            return VerifyContent(expected, "Verify SortView");
        }

        TestResult MoveItemSort()
        {
            _library.Move(0, 7);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 4",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Title of book 7",
                                            "Title of book 1",
                                            "Title of book 6"};
            return VerifyContent(expected, "Verify MoveItemSort");
        }

        TestResult ReplaceItemSort()
        {
            Book b = new Book("Replaced Book for Sort", "2303", "Replace Author Sort", "Replace Publisher Sort", 33.55, Book.BookGenre.SciFi);

            _library[6] = b;

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Replaced Book for Sort",
                                            "Title of book 7",
                                            "Title of book 1",
                                            "Title of book 6"};
            return VerifyContent(expected, "Verify ReplaceItemSort");
        }

        TestResult GroupView()
        {
            PropertyGroupDescription pgd = new PropertyGroupDescription();
            pgd.PropertyName = "Genre";
            _lvs.GroupDescriptions.Add(pgd);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Replaced Book for Sort",
                                            "Title of book 7",
                                            "Title of book 1",
                                            "Title of book 6"};
            return VerifyContent(expected, "Verify GroupView");
        }
        TestResult MoveItemGroup()
        {
            _library.Move(_library.Count - 1, 3);

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Replaced Book for Sort",
                                            "Title of book 7",
                                            "Title of book 1",
                                            "Title of book 6"};
            return VerifyContent(expected, "MoveItemGroup");
        }

        TestResult ReplaceItemGroup()
        {
            Book b = new Book("Replaced Book for Group", "9900", "Replace Author Group", "Replace Publisher Group", 50.24, Book.BookGenre.Mystery);

            _library[_library.Count - 1] = b;

            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);
            string[] expected = new string[] { "Title of book 5",
                                            "Title of book 9",
                                            "Title of book 3",
                                            "Replaced Book",
                                            "Title of book 8",
                                            "Title of book 2",
                                            "Replaced Book for Sort",
                                            "Title of book 1",
                                            "Title of book 6",
                                            "Replaced Book for Group",};

            return VerifyContent(expected, "ReplaceItemGroup");
        }

        TestResult VerifyContent(string[] expected, string stepName)
        {
            WaitForPriority(System.Windows.Threading.DispatcherPriority.Background);

            FrameworkElement[] titles = Util.FindElements(_lb, "Title");

            if (expected.Length != titles.Length)
            {
                LogComment("Expected " + expected.Length + " Titles, but actual " + titles.Length);
                return TestResult.Fail;
            }

            TextBlock title;
            bool allCorrect = true;

            for (int i = 0; i < expected.Length; i++)
            {
                title = (TextBlock)titles[i];
                if (title != null)
                {
                    if (title.Text == expected[i])
                    {
                        Status("Correct value for Title" + i);
                    }
                    else
                    {
                        if (allCorrect)
                        {
                            LogComment("Incorrect value found in step " + stepName);
                        }
                        LogComment("Expected \"" + expected[i] + "\" for Item " + i + " actual \"" + title.Text + "\" ");
                        allCorrect = false;
                    }
                }
                else
                {
                    if (allCorrect)
                    {
                        LogComment("Incorrect value found in step " + stepName);
                    }
                    LogComment("Title " + i + " could not cast to TextBlock");
                    allCorrect = false;
                }
            }

            if (allCorrect)
            {
                LogComment("All values were correct for " + stepName);
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }



        void library_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            Status("Collection changed");
            _lastAction = e.Action;
        }
    }
}

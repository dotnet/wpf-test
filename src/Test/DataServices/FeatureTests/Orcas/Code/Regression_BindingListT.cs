// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*
 * 
 * Desc: Verify that added fixes for BindingLOist<T> implementation dont regress.
 * 
 * Use a BindingList<T> Data source
 * 1. Remove Item (check for no reset event, currency and selected item removed correctly)
 * 2. Sort, then add/remove
 * 3. Filter, then Add/remove
 * 4. Change value of an items properties
*/

using System;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Windows.Media;
using System.ComponentModel;
using System.Windows.Controls;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Input;
using Microsoft.Test.DataServices;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Verification;

namespace Microsoft.Test.DataServices
{    
    /// <summary>
    /// <description>
    /// Add and remove items to a collection that implements BindingList<T>,
    /// verifying that CollectionView property values are as expected.
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(0, "Views", "BindingListTRegression")]
    public class BindingListRegressionTest : WindowTest
    {
        private ListBox _lBox;              
        private FullBindingList<Book> _fbList;
        private BindingListCollectionView _fblView;
        private ArrayList _oracle;

        public BindingListRegressionTest()
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(AddRemoveItems);
        }

        private TestResult Setup()
        {
            Status("Setup");
            WaitForPriority(DispatcherPriority.Render);

            _fbList = new FullBindingList<Book>();
            _lBox = new ListBox();
            _fblView = (BindingListCollectionView)CollectionViewSource.GetDefaultView(_fbList);
            _lBox.ItemsSource = _fblView;

            // Setup oracle
            _oracle = new ArrayList();

            LogComment("Setup was successful");


            return TestResult.Pass;
        }       

        // A simplistic BVT that is effectively a random walk through a state-
        // based model. Long term, a P1 that either uses modeling or attribute
        // variations would be nice. Even better if we better unify our data
        // sources where this test could be run with various types of data
        // sources backing the collection view.
        private TestResult AddRemoveItems()
        {
            Status("Add and Remove Items");

            
            Book book1 = new Book("title1", "", "Aman", "", 0.0, Book.BookGenre.Comic);
            Book book2 = new Book("title2", "", "Bman", "", 0.0, Book.BookGenre.Comic);
            Book book3 = new Book("title3", "", "Cman", "", 0.0, Book.BookGenre.Comic);
            Book book4 = new Book("title4", "", "Dman", "", 0.0, Book.BookGenre.Comic);
            Book book5 = new Book("title4", "", "Eman", "", 0.0, Book.BookGenre.Comic);
            
            // 1. Adding Items.
            _fbList.Add(book1);
            _lBox.SelectedItem = _lBox.Items[0];
            int origIndex = _lBox.SelectedIndex;
            _fbList.Add(book2);
            _fbList.Add(book3);
            _fbList.Add(book4);
            _fbList.Insert(origIndex, book5);

            // Update oracle ...           
            _oracle.Add(book1);
            _oracle.Add(book2);
            _oracle.Add(book3);
            _oracle.Add(book4);
            _oracle.Insert(origIndex, book5);

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView)) 
            {
                LogComment("Add didnt work properly.");
                return TestResult.Fail;
            }

            // verify seleteditem not changed.

            Book selectedItem = (Book) _lBox.SelectedItem;            
            
            // Update oracle ...           
            _oracle.RemoveAt(_lBox.SelectedIndex);

            // 2. Remove Item.
            _fbList.RemoveAt(_lBox.SelectedIndex);            

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Remove didnt work properly.");
                return TestResult.Fail;
            }

            // Check to see if selecteditem was removed ...
            if (_lBox.SelectedItem == selectedItem)
            {
                LogComment("SelectedItem not removed.");
                return TestResult.Fail;
            }           

            // ... and if selected index was updated correctly.
            if (!Verify(false, -1, null))
                return TestResult.Fail;

            _fblView.MoveCurrentToFirst();
            
            if (!Verify(false, 0, book5))
                return TestResult.Fail;
            
            // 3. Sort View.
            _fblView.SortDescriptions.Add(new SortDescription("Author", ListSortDirection.Ascending));
            
            // Update oracle (first sort call causes ascend sort)
            _oracle.Sort();

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("sort didnt work properly.");
                return TestResult.Fail;
            }

            // Add Items after sort.
            _fbList.Add(book5);

            // Update oracle ...           
            _oracle.Add(book5);

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Add after sort didnt work properly.");
                return TestResult.Fail;
            }

            // Verify currency.
            if (!Verify(false, 4, book5))
                return TestResult.Fail;

            // Update oracle ...           
            _oracle.RemoveAt(0);

            // Remove Items after sort.
            _fbList.RemoveAt(0);            

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Remove didnt work properly.");
                return TestResult.Fail;
            }

            // Verify currency.
            if (!Verify(false, 3, book5))
                return TestResult.Fail;

            // 4. Filter View
            _fblView.CustomFilter = "Author <> 'C'";

            // Update oracle ...           
            _oracle.Clear();
            _oracle.Add(book3);
            _oracle.Add(book4);
            _oracle.Add(book5);
            _oracle.Add(book5);            

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Filter didnt work properly.");
                return TestResult.Fail;
            }

            // Add Items After Filter.
            _fbList.Add(book5);

            // Update oracle ...           
            _oracle.Add(book5);

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Add after filter didnt work properly.");
                return TestResult.Fail;
            }

            // Verify currency.
            _fblView.MoveCurrentToNext();
            
            if (!Verify(false, 4, book5))
                return TestResult.Fail;

            // Update oracle ...           
            _oracle.RemoveAt(0);

            // Remove Items After Filter.
            _fbList.RemoveAt(0);            

            // Verify content.
            if (!CheckItemsInView(_oracle, _fblView))
            {
                LogComment("Remove didnt work properly.");
                return TestResult.Fail;
            }
            

            // Verify currency.
            _fblView.MoveCurrentToNext();

            if (!Verify(false, 4, null))
                return TestResult.Fail;

            LogComment("Add and Remove Items passed.");
            return TestResult.Pass;
        }

        // Simplistic verifier.
        private bool Verify(bool isEmpty, int currentPosition, object currentItem)
        {            
            if (isEmpty == _fblView.IsEmpty && currentPosition == _fblView.CurrentPosition && currentItem == _fblView.CurrentItem)
                return true;
            else
                return false;
        }

        // Compare Oracle to actual list.
        bool CheckItemsInView(ArrayList expectedItems, BindingListCollectionView fblv)
        {
            int listCount = expectedItems.Count;
            int viewCount = 0;
            foreach (object item in fblv)
            {
                if (!expectedItems.Contains(item))
                {
                    LogComment("Fail - View should not contain item " + item);
                    return false;
                }
                viewCount++;
            }
            if (listCount != viewCount)
            {
                LogComment("Fail - Fail - There should be " + listCount + " element in the view, instead there are " + viewCount);
                return false;
            }
            return true;
        }

        
    }
}




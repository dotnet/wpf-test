// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where BindingListCollectionView doesn't sort if ITypedList is not present
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "BindingListCollectionViewSort")]
    public class BindingListCollectionViewSort : XamlTest
    {
        #region Private Data

        private StackPanel _myStackPanel;
        private ListBox _myListBox;
        private MyList _myList;
        
        #endregion

        #region Constructors

        public BindingListCollectionViewSort()
            : base(@"BindingListCollectionViewSort.xaml")
        {
            InitializeSteps += new TestStep(Setup);            
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        private TestResult Setup()
        {
            _myStackPanel = (StackPanel) RootElement.FindName("myStackPanel");
            _myListBox = (ListBox)RootElement.FindName("myListBox");
            _myList = (MyList)RootElement.FindResource("MyList");

            if (_myStackPanel == null || _myList == null || _myListBox == null)
            {
                LogComment("Failed to load Xaml correctly");
                return TestResult.Fail;
            }                        

            return TestResult.Pass;
        }
                
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Add the Sort descriptions
            ICollectionView collectionView = CollectionViewSource.GetDefaultView(_myList);
            SortDescriptionCollection sortDescriptionCollection = collectionView.SortDescriptions;

            if (sortDescriptionCollection.Count == 0)
            {
                using (collectionView.DeferRefresh())
                {
                    sortDescriptionCollection.Add(new SortDescription("Author", ListSortDirection.Ascending));
                    sortDescriptionCollection.Add(new SortDescription("Title", ListSortDirection.Ascending));
                }
            }

            // Verify the sort order
            CustomBook customBook = (CustomBook)_myListBox.Items[0];

            if (customBook.Author != "Doyle" || customBook.Title != "Hound of the Baskervilles")
            {
                LogComment("The Listbox was not sorted correctly");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class CustomBook
    {
        public CustomBook(string authorString, string titleString)
        {
            _author = authorString;
            _title = titleString;
        }

        public string Author
        {
            get { return _author; }
            set { _author = value; }
        }

        public string Title
        {
            get { return _title; }
            set { _title = value; }
        }

        string _author,_title;
    }

    public class MyList : FullBindingList<CustomBook>
    {
        public MyList()
        {
            Add(new CustomBook("Shakespeare", "Macbeth"));
            Add(new CustomBook("Doyle", "Study in Scarlet"));
            Add(new CustomBook("Shakespeare", "Hamlet"));
            Add(new CustomBook("Eco", "Name of the Rose"));
            Add(new CustomBook("Doyle", "Valley of Fear"));
            Add(new CustomBook("Russo", "Empire Falls"));
            Add(new CustomBook("Shakespeare", "Comedy of Errors"));
            Add(new CustomBook("Doyle", "Hound of the Baskervilles"));
            Add(new CustomBook("Eco", "Baudalino"));
            Add(new CustomBook("Shakespeare", "Tempest"));
        }
    }

    #endregion
}

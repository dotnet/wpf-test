// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Data;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where Inconsistency in type of datasource and NewItemPlaceholder
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "NewItemPlaceHolder")]
    public class NewItemPlaceHolder : AvalonTest
    {
        #region Constructors

        public NewItemPlaceHolder()            
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members

        private TestResult Validate()
        {            
            // Set the ItemsSource to be an empty Collection.            
            ListBox myListBox = new ListBox();
            myListBox.ItemsSource = new EditablePlaces();

            // Grab a reference to the collection view
            IEditableCollectionView collectionView = (IEditableCollectionView) CollectionViewSource.GetDefaultView(myListBox.ItemsSource);
            
            // Verify 
            if (!collectionView.CanAddNew)
            {
                LogComment("CanAddNew is set to false, whereas it should be true.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class EditablePlaces: ObservableCollection<Place>
    {
        public EditablePlaces()
        {            
        }
    }

    #endregion
}
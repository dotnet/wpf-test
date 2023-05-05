// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.ObjectModel;
using System.ComponentModel;
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
    ///  Regression coverage for bug where Can't set ItemsCollection.ItemsSource to TypeDescriptor.GetProperties()
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "ItemSourceTypeDescriptor")]
    public class ItemSourceTypeDescriptor : AvalonTest
    {
        #region Constructors

        public ItemSourceTypeDescriptor()
        {
            InitializeSteps += new TestStep(Validate);                        
        }

        #endregion

        #region Private Members
        
        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Render);         
            
            // Do some actions            
            ObservableCollection<string> observableCollection = new ObservableCollection<string>();
            observableCollection.Add("AliceBlue");
            observableCollection.Add("AntiqueWhite");
            observableCollection.Add("Aqua");
            observableCollection.Add("Azure");
            observableCollection.Add("Beige");
            observableCollection.Add("Bisque");
            CollectionViewSource collectionViewSource = new CollectionViewSource();
            collectionViewSource.Source = observableCollection;

            ListBox listBox = new ListBox();

            // If the bug regresses, then we will get a ArgumentNullException here.
            listBox.ItemsSource = TypeDescriptor.GetProperties(collectionViewSource);             

            return TestResult.Pass;
        }

        #endregion
        
    }    
}

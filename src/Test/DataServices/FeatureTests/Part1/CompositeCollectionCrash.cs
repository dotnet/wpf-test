// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.ComponentModel;
using System.Globalization;
using System.Collections;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where WPF:  Nested CompositeCollection throw ArgumentOutOfRangeException when adding element to inner ObservableCollection
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "CompositCollectionCrash")]
    public class CompositCollectionCrash : AvalonTest
    {
        #region Constructors

        public CompositCollectionCrash()            
        {
            InitializeSteps += new TestStep(Validate);            
        }

        #endregion

        #region Private Members
                
        private TestResult Validate()
        {
            //construct the following object hierarchy:
            // composite_2
            //  compositeContainer_1
            //    composite_1
            //      compositeContainer_0
            //        composite_0
            //          collectionContainer
            //            observableCollection             
            ObservableCollection<Object> observableCollection = new ObservableCollection<Object>();
            CollectionContainer collectionContainer = new CollectionContainer();
            collectionContainer.Collection = observableCollection;

            CompositeCollection composite_0 = new CompositeCollection();
            composite_0.Add(collectionContainer);

            CollectionContainer compositeContainer_0 = new CollectionContainer();
            compositeContainer_0.Collection = composite_0;

            CompositeCollection composite_1 = new CompositeCollection();
            composite_1.Add(compositeContainer_0);

            CollectionContainer compositeContainer_1 = new CollectionContainer();
            compositeContainer_1.Collection = composite_1;

            CompositeCollection composite_2 = new CompositeCollection();
            composite_2.Add(compositeContainer_1);
            
            // At this point, all collections are empty.
            // We now add an element to the observableCollection object.
            //  => unhandled exception.            
            observableCollection.Add(new Object());

            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes
    
    #endregion
}

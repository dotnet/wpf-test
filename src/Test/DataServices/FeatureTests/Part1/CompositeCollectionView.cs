// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Windows.Data;
using System.Collections.Specialized;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    ///  Regression coverage for bug where CompositeCollectionView doesn't deal well with a Remove CollectionChanged event where index is -1
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>
    [Test(1, "Regressions.Part1", "CompositeCollectionView")]
    public class CompositeCollectionView : AvalonTest
    {
        #region Private Data

        
        
        #endregion

        #region Constructors

        public CompositeCollectionView()
            : base()
        {
            InitializeSteps += new TestStep(Validate);            
            
        }

        #endregion

        #region Private Members

                
        private TestResult Validate()
        {
            // Do some actions            
            CompositeCollection compositeCollection = new CompositeCollection();
            MyColl<int> myCollection = new MyColl<int>();
            myCollection.Add(1);
            myCollection.Add(2);
            myCollection.Add(3);
            CollectionContainer collectionContainer = new CollectionContainer();
            collectionContainer.Collection = myCollection;
            compositeCollection.Add(collectionContainer);

            object o = CollectionViewSource.GetDefaultView(compositeCollection);
            myCollection.PerformRemove(0);
            myCollection.PerformRemove(0);

            // In case of a regression the previous code will cause an invariant assert.            
            return TestResult.Pass;
        }

        #endregion
        
    }
    
    #region Helper Classes

    public class MyColl<T> : List<T>, INotifyCollectionChanged
    {

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion

        public void PerformRemove(int index)
        {
            object item = base[index];

            base.RemoveAt(index);

            CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, item, -1));
        }
    }

    #endregion
}

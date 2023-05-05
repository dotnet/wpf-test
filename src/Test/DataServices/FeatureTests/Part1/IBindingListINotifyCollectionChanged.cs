// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.Specialized;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Regression coverage for bug where if a collection implements both IBindingList and INotifyCollectionChanged, then the collection view listens to both.
    /// </description>

    /// </summary>
    [Test(1, "Regressions.Part1", "IBindingListINotifyCollectionChanged")]
    public class IBindingListINotifyCollectionChanged: XamlTest
    {
        #region Private Data

        private ListView _listView;
        private MyColl _collection;        
	 
        #endregion
	
        #region Constructors

        public IBindingListINotifyCollectionChanged() :
            base(@"IBindingListINotifyCollectionChanged.xaml")
        {
            RunSteps += new TestStep(Setup);
            RunSteps += new TestStep(Validate);
        }

        #endregion

        #region Private Members

        // Grab ListView and add our custome collection as the ItemSource
        private TestResult Setup()
        {
            //Grabbing ListView
            _listView = (ListView)RootElement.FindName("listView");            
            
            _collection = new MyColl();
            _collection.Add(new object());
            
            // Adding ItemSource
            _listView.ItemsSource = _collection;

            return TestResult.Pass;
        }

        private TestResult Validate()
        {
            WaitForPriority(DispatcherPriority.Background);
					
            // Validation #1: If Item Containers are not empty
            if (_listView.ItemContainerGenerator.ContainerFromIndex(0) == null)
            {
                LogComment("All item containers were removed");
                return TestResult.Fail;
            }
            
            if (_listView.ItemContainerGenerator.ContainerFromIndex(1) != null)
            {
                LogComment("We still have 2 item containers");
                return TestResult.Fail;
            }

            // Validation #2: Check number of child elements for dockpanel using visual tree (should be 1)
            if (VisualTreeHelper.GetChildrenCount(_listView) != 1)
            {
                LogComment("We should only have 1 item. There are currently " + VisualTreeHelper.GetChildrenCount(_listView).ToString() + " items in the DockPanel");
                return TestResult.Fail;
            }
            

            return TestResult.Pass;
        }      
	#endregion               
	
    }

    #region Helper Class

    public class MyColl : BindingList<object>, INotifyCollectionChanged
    {
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            base.OnListChanged(e);
            if (e.ListChangedType == ListChangedType.ItemAdded && CollectionChanged != null)
            {
                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, this[e.NewIndex], e.NewIndex));
            }
        }

        #region INotifyCollectionChanged Members

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        #endregion
    }

    #endregion

}

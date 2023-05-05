// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Threading;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Verification;
using Microsoft.Test.Verifiers;
using System.Collections.Specialized;
using System.ComponentModel;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// - Verify that PropertyChanged and CollectionChanged events are raised correctly on FreezableCollection<T>
    /// - Verify that re-entrancy is correctly blocked on said Event Handlers   
    /// </description>
    /// <relatedBugs>
    
    /// </relatedBugs>
    /// </summary>                
    [Test(3, "Collections", "FreezableCollectionINotifyCollectionChange")]
    public class FreezableCollectionINotifyCollectionChange : AvalonTest
    {
        #region Private Data

        private FreezableCollection<DependencyObject> _myCollection;
        private DependencyObject _myDependencyObject;
        private DependencyObject _myReplaceDependencyObject;

        #endregion


        #region Public Members

        public FreezableCollectionINotifyCollectionChange()
        {
            InitializeSteps += new TestStep(SetUp);
            RunSteps += new TestStep(VerifyActionNotifications);
            RunSteps += new TestStep(VerifyReentrancy);
        }

        public TestResult SetUp()
        {
            Status("Setup");

            // Create a Freezable Collection.
            _myCollection = new FreezableCollection<DependencyObject>();
            _myDependencyObject = new DependencyObject();
            _myReplaceDependencyObject = new DependencyObject();

            LogComment("Setup was successful");
            return TestResult.Pass;
        }


        public TestResult VerifyActionNotifications()
        {
            Status("VerifyActionNotifications");

            // Verify Add.
            NotifyCollectionChangedEventArgs addEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _myDependencyObject, _myCollection.Count);
            NotificationEventsVerifier.VerifyNotificationEvents(Add, (INotifyCollectionChanged)_myCollection, addEventArgs, "Count", "Item[]");

            // Verify Remove.
            NotifyCollectionChangedEventArgs removeEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, _myDependencyObject, 0);
            NotificationEventsVerifier.VerifyNotificationEvents(Remove, (INotifyCollectionChanged)_myCollection, removeEventArgs, "Count", "Item[]");

            // Verify Insert.
            NotifyCollectionChangedEventArgs insertEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, _myDependencyObject, 0);
            NotificationEventsVerifier.VerifyNotificationEvents(Insert, (INotifyCollectionChanged)_myCollection, insertEventArgs, "Count", "Item[]");

            // Verify Replace.
            NotifyCollectionChangedEventArgs replaceEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Replace, _myReplaceDependencyObject, _myDependencyObject, 0);
            NotificationEventsVerifier.VerifyNotificationEvents(Replace, (INotifyCollectionChanged)_myCollection, replaceEventArgs, "Item[]");

            // Verify Clear.
            NotifyCollectionChangedEventArgs clearEventArgs = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            NotificationEventsVerifier.VerifyNotificationEvents(Clear, (INotifyCollectionChanged)_myCollection, clearEventArgs, "Count", "Item[]");

            LogComment("Notifications Verified Successfully.");
            return TestResult.Pass;
        }

        // Verify that the re-entrancy is blocked.
        public TestResult VerifyReentrancy()
        {
            Status("VerifyReentrancy");

            // Attach an event listener.
            ((INotifyCollectionChanged)_myCollection).CollectionChanged += new NotifyCollectionChangedEventHandler(CollectionChanged);
            ((INotifyPropertyChanged)_myCollection).PropertyChanged += new PropertyChangedEventHandler(PropertyChanged);

            // This action should raise an event.
            Add();

            LogComment("Blocked Reentrancy Verified Successfully.");
            return TestResult.Pass;
        }

        #endregion


        #region Private Members

        // EventHandlers for myCollection.
        private void PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Verify correct exceptions are thrown on attempts to modify collection.
            NotificationEventsVerifier.VerifyNotificationEvents(Add, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Remove, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Insert, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Replace, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Clear, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
        }

        private void CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Varify correct exceptions are thrown on attempts to modify collection.
            NotificationEventsVerifier.VerifyNotificationEvents(Add, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Remove, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Insert, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Replace, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
            NotificationEventsVerifier.VerifyNotificationEvents(Clear, (INotifyCollectionChanged)_myCollection, typeof(InvalidOperationException));
        }        

        // Actions for myCollection.
        private void Add()
        {
            _myCollection.Add(_myDependencyObject);
        }
        private void Remove()
        {
            _myCollection.RemoveAt(0);
        }

        private void Clear()
        {
            _myCollection.Clear();
        }

        private void Replace()
        {
            _myCollection[0] = _myReplaceDependencyObject;
        }

        private void Insert()
        {
            _myCollection.Insert(0, _myDependencyObject);
        }

        #endregion
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Specialized;
using Microsoft.Test;
using System.Windows.Controls;
using System.ComponentModel;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests that the CollectionChanged event will be fired with the correct arguments when
    /// the ItemsCollection changes.
    /// </description>
    /// </summary>

    [Test(0, "Collections", "CollectionEvent")]
    public class CollectionEvent : AvalonTest 
    {
        ItemCollection _items;
        NotifyCollectionChangedEventArgs _expectedArg;
        int _eventCalled = 0;
        bool _argIsValid = true;

        public CollectionEvent(){
            InitializeSteps += new TestStep(Init);
            RunSteps += new TestStep(CollectionAdd);
            RunSteps += new TestStep(CollectionRemove);
            RunSteps += new TestStep(CollectionInsert);
            RunSteps += new TestStep(CollectionClear);
        }

        private TestResult Init()
        {
            Status("Init");
            ItemsControl  ctrl = new ItemsControl();
            _items = ctrl.Items;
            _items.Add("foo");
            _items.Add("bar");
            _items.Add("moo");
            if (!(_items.Count==3))
                return TestResult.Fail;

            ((INotifyCollectionChanged)_items).CollectionChanged += new NotifyCollectionChangedEventHandler(OnCollChanged);

            return TestResult.Pass;
        }

        private TestResult CollectionAdd()
        {
            Status("Adding to Collection");
            _expectedArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, "jar");
            _items.Add("jar");
            return ValidateCollectionEvent();
        }
        private TestResult CollectionRemove()
        {
            Status("Removing to Collection");
            _expectedArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Remove, "foo");
            _items.Remove("foo");
            return ValidateCollectionEvent();
        }
        private TestResult CollectionInsert()
        {
            Status("Inserting to Collection");
            _expectedArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, "tar");
            _items.Insert(2,"tar");
            return ValidateCollectionEvent();
        }
        private TestResult CollectionClear()
        {
            Status("Clearing to Collection");
            _expectedArg = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            _items.Clear();
            return ValidateCollectionEvent();
        }

        #region AuxMethods
        private TestResult ValidateCollectionEvent()
        {
            TestResult returnValue = TestResult.Pass;
            if (_eventCalled != 1) // Event was not fired
            {
                LogComment("Event Fired an unexecpted amount of times, Expected == 1: Actual == "+_eventCalled);
                returnValue = TestResult.Fail;
            }
            if (!(_argIsValid)) //Something didn't validate correctly
            {
                returnValue = TestResult.Fail;
            }

            //resetting global variables
            _eventCalled = 0;
            _argIsValid = true;
            return returnValue;
        }
        private void OnCollChanged(object sender, NotifyCollectionChangedEventArgs args)
        {
            _eventCalled++;
            if (!(_items==sender))
            {
                LogComment("The collection didn't match the sender");
                _argIsValid = false;
            }
            if (!(args.Action == _expectedArg.Action))
            {
                LogComment("args.Action '"+args.Action+"' doesn't match expectedArg.Action '"+_expectedArg.Action+"'!");
                _argIsValid = false;
            }
            if (args.Action != NotifyCollectionChangedAction.Reset)
            {
                object expectedItem = null;
                object actualItem = null;
                if (args.Action == NotifyCollectionChangedAction.Add)
                {
                    expectedItem = _expectedArg.NewItems[0];
                    actualItem = args.NewItems[0];
                }
                else if (args.Action == NotifyCollectionChangedAction.Remove)
                {
                    expectedItem = _expectedArg.OldItems[0];
                    actualItem = args.OldItems[0];
                }

                if (actualItem != expectedItem)
                {
                    LogComment("Actual item '"+actualItem+"' doesn't match expected item '"+expectedItem+"'!");
                    _argIsValid = false;
                }
            }
        }
        #endregion
    }

}


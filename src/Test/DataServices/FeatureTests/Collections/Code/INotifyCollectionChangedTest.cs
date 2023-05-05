// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using Microsoft.Test;
using System.ComponentModel;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Collections;
using System.Collections.Specialized;
using System;
using System.Windows.Controls;
using System.Windows;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Tests raising the CollectionChanged event in an IEnumerable data source by passing
    /// different combinations of arguments to the NotifyCollectionChangedArgs constructor.
    /// </description>
    /// <relatedBugs>



    /// </relatedBugs>
    /// </summary>
    [Test(3, "Collections", "INotifyCollectionChangedTest")]
    public class INotifyCollectionChangedTest : XamlTest
    {
        private string _action;
        private string _changedItemToBePassed;
        private string _indexToBePassed;
        private string _resultChangedItem;
        private string _resultIndex;
        private string _exception;
        private ListBox _lb;
        private MyLibraryEnumerable _mle;

        private Book _bookAdd;
        private Book _bookRemove;
        private int _indexAdd;
        private int _indexRemove;

        public INotifyCollectionChangedTest()
            : this("Add", "Correct", "Correct", "Correct", "Correct", "")
        {
        }

        [Variation("Reset", "", "", "Null", "DontKnow", "")]
        [Variation("Reset", "Null", "IncorrectNotInCollection", "", "", "ArgumentException")]
        [Variation("Add", "", "", "", "", "ArgumentException")]
        [Variation("Add", "Correct", "Correct", "Correct", "Correct", "")]
        [Variation("Remove", "Correct", "", "", "", "InvalidOperationException")]
        [Variation("Remove", "IncorrectNotInCollection", "", "", "", "InvalidOperationException")]
        [Variation("Remove", "Correct", "Correct", "Correct", "Correct", "")]
        [Variation("Remove", "Correct", "DontKnow", "", "", "InvalidOperationException")]
        [Variation("Remove", "IncorrectInCollection", "DontKnow", "", "", "InvalidOperationException")]
        [Variation("Add", "Correct", "", "Correct", "Correct", "")]
        [Variation("Add", "IncorrectNotInCollection", "", "IncorrectPassed", "Correct", "")]
        [Variation("Add", "IncorrectInCollection", "", "IncorrectPassed", "Correct", "")]
        [Variation("Add", "Null", "", "Null", "Correct", "")]
        [Variation("Add", "Correct", "DontKnow", "Correct", "Correct", "")]
        [Variation("Add", "IncorrectInCollection", "DontKnow", "IncorrectPassed", "Correct", "")]
        [Variation("Add", "Null", "DontKnow", "Null", "Correct", "")]
        [Variation("Remove", "Correct", "IncorrectInCollection", "Correct", "IncorrectPassed", "ArgumentOutOfRangeException")]
        [Variation("Remove", "Correct", "IncorrectNotInCollection", "Correct", "IncorrectPassed", "ArgumentOutOfRangeException")]
        [Variation("Remove", "IncorrectNotInCollection", "Correct", "IncorrectPassed", "Correct", "InvalidOperationException")]
        [Variation("Remove", "IncorrectInCollection", "Correct", "IncorrectPassed", "Correct", "InvalidOperationException")]
        [Variation("Remove", "IncorrectInCollection", "IncorrectNotInCollection", "IncorrectPassed", "IncorrectPassed", "ArgumentOutOfRangeException")]
        public INotifyCollectionChangedTest(string action, string changedItemToBePassed,
            string indexToBePassed, string resultChangedItem, string resultIndex, string exception)
            : base(@"INotifyCollectionChangedTest.xaml")
        {
            this._action = action;
            this._changedItemToBePassed = changedItemToBePassed;
            this._indexToBePassed = indexToBePassed;
            this._resultChangedItem = resultChangedItem;
            this._resultIndex = resultIndex;
            this._exception = exception;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestNotify);
        }

        private TestResult Setup()
        {
            Status("Setup");

            _lb = Util.FindElement(RootElement, "lb") as ListBox;
            if (_lb == null)
            {
                LogComment("Fail - Unable to reference lb element (ListBox).");
                return TestResult.Fail;
            }

            _mle = new MyLibraryEnumerable(30);
            _lb.ItemsSource = _mle;
            ((INotifyCollectionChanged)_lb.Items).CollectionChanged += new NotifyCollectionChangedEventHandler(collectionChangedHandler);
            return TestResult.Pass;
        }

        private void collectionChangedHandler(object sender, NotifyCollectionChangedEventArgs nccea)
        {
            IList changedItems = null;
            int startingIndex = -1;
            if (nccea.Action == NotifyCollectionChangedAction.Add)
            {
                changedItems = nccea.NewItems;
                startingIndex = nccea.NewStartingIndex;
            }
            else if (nccea.Action == NotifyCollectionChangedAction.Remove)
            {
                changedItems = nccea.OldItems;
                startingIndex = nccea.OldStartingIndex;
            }
            
            if (nccea.Action != GetAction(_action))
            {
                LogComment("Fail - Action is incorrect.");
                Signal("CollectionChanged", TestResult.Fail);
            }
            else if ((nccea.Action != NotifyCollectionChangedAction.Reset) && 
                (changedItems[0] != GetResultChangedItem()))
            {
                LogComment("Fail - ChangedItem in collection changed handler is incorrect (action != Reset).");
                Signal("CollectionChanged", TestResult.Fail);
            }
            else if ((nccea.Action == NotifyCollectionChangedAction.Reset) &&
                (changedItems != GetResultChangedItem()))
            {
                LogComment("Fail - ChangedItem in collection changed handler is incorrect (action == Reset).");
                Signal("CollectionChanged", TestResult.Fail);
            }
            else if (startingIndex != GetResultIndex())
            {
                LogComment("Fail - StartingIndex in collection changed handler is incorrect");
                Signal("CollectionChanged", TestResult.Fail);
            }
            else
            {
                Signal("CollectionChanged", TestResult.Pass);
            }
        }

        TestResult TestNotify()
        {
            Status("TestNotify");

            if (this._exception != "")
            {
                Type exceptionType = Type.GetType("System." + this._exception, true, true);
                Type baseExceptionType = Type.GetType("System.Exception");
                if (exceptionType.Equals(baseExceptionType) || exceptionType.IsSubclassOf(baseExceptionType))
                {
                    this.SetExpectedErrorTypeInStep(exceptionType);
                }
            }
            switch (_action)
            {
                case "Add":
                    _bookAdd = new Book("99");
                    _indexAdd = _mle.Count;
                    if (_action != "") { _mle.ActionToBePassed = GetAction(_action); }
                    if (_changedItemToBePassed != "") { _mle.ChangedItemToBePassed = GetChangedItemToBePassed(_bookAdd); }
                    if (_indexToBePassed != "") { _mle.IndexToBePassed = GetIndexToBePassed(_indexAdd); }
                    _mle.Add(_bookAdd);
                    TestResult resultAdd = WaitForSignal("CollectionChanged");
                    if (resultAdd == TestResult.Unknown)
                    {
                        LogComment("Wait for collection changed event handler timed out.");
                    }
                    return resultAdd;
                case "Remove":
                    IEnumerator ie = _mle.GetEnumerator();
                    ie.MoveNext();
                    _bookRemove = ie.Current as Book;
                    _indexRemove = 0;
                    if (_action != "") { _mle.ActionToBePassed = GetAction(_action); }
                    if (_changedItemToBePassed != "") { _mle.ChangedItemToBePassed = GetChangedItemToBePassed(_bookRemove); }
                    if (_indexToBePassed != "") { _mle.IndexToBePassed = GetIndexToBePassed(_indexRemove); }
                    _mle.Remove(_bookRemove);
                    TestResult resultRemove = WaitForSignal("CollectionChanged");
                    if (resultRemove == TestResult.Unknown)
                    {
                        LogComment("Wait for collection changed event handler timed out.");
                    }
                    return resultRemove;
                case "Reset":
                    if (_action != "") { _mle.ActionToBePassed = GetAction(_action); }
                    if (_changedItemToBePassed != "") { _mle.ChangedItemToBePassed = GetChangedItemToBePassed(_bookRemove); }
                    if (_indexToBePassed != "") { _mle.IndexToBePassed = GetIndexToBePassed(_indexRemove); }
                    _mle.Clear();
                    TestResult resultClear = WaitForSignal("CollectionChanged");
                    if (resultClear == TestResult.Unknown)
                    {
                        LogComment("Wait for collection changed event handler timed out.");
                    }
                    return resultClear;
            }
            return TestResult.Pass;
        }

        private NotifyCollectionChangedAction GetAction(string action)
        {
            switch (action)
            {
                case "Add":
                    return NotifyCollectionChangedAction.Add;
                case "Remove":
                    return NotifyCollectionChangedAction.Remove;
                case "Reset":
                    return NotifyCollectionChangedAction.Reset;
                default:
                    throw new Exception("Action is not valid.");
            }
        }

        private object GetChangedItemToBePassed(object correctItem)
        {
            switch (_changedItemToBePassed)
            {
                case "Correct":
                    return correctItem;
                case "IncorrectNotInCollection":
                    return new Book("1000");
                case "IncorrectInCollection":
                    foreach (object obj in _mle)
                    {
                        if (obj != correctItem)
                        {
                            return obj;
                        }
                    }
                    throw new Exception("Could not pick an incorrect item in collection");
                case "Null":
                    return null;
                default:
                    throw new Exception("ChangedItemToBePassed is not valid.");
            }
        }

        private int GetIndexToBePassed(int correctIndex)
        {
            switch (_indexToBePassed)
            {
                case "Correct":
                    return correctIndex;
                case "IncorrectNotInCollection":
                    return (this._mle.Count + 2);
                case "IncorrectInCollection":
                    for (int i = _mle.Count; i > 0; i--)
                    {
                        if (i != correctIndex)
                        {
                            return i;
                        }
                    }
                    throw new Exception("Could not pick an incorrect index in collection");
                case "DontKnow":
                    return -1;
                default:
                    throw new Exception("IndexToBePassed is not valid.");
            }
        }

        private object GetResultChangedItem()
        {
            switch (_resultChangedItem)
            {
                case "Correct":
                    if (_action == "Add")
                    {
                        return _bookAdd;
                    }
                    else if (_action == "Remove")
                    {
                        return _bookRemove;
                    }
                    else
                    {
                        throw new Exception("Action in GetResultChangedItem is not valid");
                    }
                case "IncorrectPassed":
                    return _mle.ChangedItemToBePassed;
                case "Null":
                    return null;
                default:
                    throw new Exception("ResultChangedItem is not valid.");
            }
        }

        private int GetResultIndex()
        {
            switch (_resultIndex)
            {
                case "Correct":
                    if (_action == "Add")
                    {
                        return _indexAdd;
                    }
                    else if(_action == "Remove")
                    {
                        return _indexRemove;
                    }
                    else
                    {
                        throw new Exception("Action in GetResultIndex is not valid");
                    }
                case "IncorrectPassed":
                    return _mle.IndexToBePassed;
                case "DontKnow":
                    return -1;
                default:
                    throw new Exception("ResultIndex is not valid");
            }
        }
    }
}


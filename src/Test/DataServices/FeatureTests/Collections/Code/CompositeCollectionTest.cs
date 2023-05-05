// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.ComponentModel;
using System.Collections;
using System.Collections.Specialized;
using System.Threading;
using System.Windows.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Collections.ObjectModel;
using System.Windows.Navigation;
using System.Windows.Media;
using Microsoft.Test;
using Microsoft.Test.DataServices;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.DataServices
{
    /// <summary>
    /// <description>
    /// Test CompositeCollection API to improve code coverage
    /// </description>
    /// </summary>
    [Test(1, "Collections", "CompositeCollectionTest")]
    public class CompositeCollectionTest : XamlTest
    {

        ListBox _lb = null;
        CompositeCollection _cc = null;
        NotifyCollectionChangedAction _lastAction = NotifyCollectionChangedAction.Reset;
        CollectionContainer _container = null;
        Array _mylist = null;
        NotifyCollectionChangedEventHandler _myhandler;

        public CompositeCollectionTest()
            : base("CollectionMixed.xaml")
        {
            InitializeSteps += new TestStep(FindListBox);
            RunSteps += new TestStep(TestAdd);
            RunSteps += new TestStep(TestAddVerify);
            RunSteps += new TestStep(TestRemove);
            RunSteps += new TestStep(TestRemoveVerify);
            RunSteps += new TestStep(CopyTo);
            RunSteps += new TestStep(CopyToVerify);
            RunSteps += new TestStep(Clear);
            RunSteps += new TestStep(ClearVerify);
            RunSteps += new TestStep(Insert);
            RunSteps += new TestStep(InsertVerify);
            RunSteps += new TestStep(InterfaceProperties);
            RunSteps += new TestStep(ClearNoHandler);
            RunSteps += new TestStep(VerifyHandlerRemoved);
        }

        TestResult FindListBox()
        {
            Status("Setting up for the test");

            _lb = (ListBox)Util.FindElement(RootElement, "testListBox");
            if (_lb == null)
            {
                LogComment("testListBox is null");
                return TestResult.Fail;
            }

            _cc = _lb.ItemsSource as CompositeCollection;
            if (_cc == null)
            {
                LogComment("CompositeCollection is null");
                return TestResult.Fail;
            }

            LogComment("Setup was completed successfully");
            return TestResult.Pass;
        }


        TestResult TestAdd()
        {
            Status("Testing eventhandler, and add");
            _myhandler = new NotifyCollectionChangedEventHandler(cc_CollectionChanged);
            ((INotifyCollectionChanged)_cc).CollectionChanged += _myhandler;

            _container = new CollectionContainer();
            Binding b = new Binding();
            ObjectDataProvider dso_genlist = RootElement.Resources["DSO_GenList"] as ObjectDataProvider;
            if (dso_genlist == null)
            {
                LogComment("Could not reference the DSO_GenList");
                return TestResult.Fail;
            }

            b.Source = dso_genlist;
            BindingOperations.SetBinding(_container, CollectionContainer.CollectionProperty, b);

            _cc.Add(_container);

            LogComment("Add and eventhandler complete");
            return TestResult.Pass;
        }

        TestResult TestAddVerify()
        {
            Status("Check add, contains, indexof");
            WaitForPriority(DispatcherPriority.Background);

            if (_lastAction != NotifyCollectionChangedAction.Add)
            {
                LogComment("Expected Add, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            if (!_cc.Contains(_container))
            {
                LogComment("CompositeCollection does not contain the CollectionContainer");
                return TestResult.Fail;
            }

            if (_cc.IndexOf(_container) != 6)
            {
                LogComment("CollectionContainer is not the correct index, expected 6, actual " + _cc.IndexOf(_container).ToString());
                return TestResult.Fail;
            }
            LogComment("Checked add, contains, indexof");
            return TestResult.Pass;
        }

        TestResult TestRemove()
        {
            Status("Testing Remove ");

            _cc.Remove(_container);

            LogComment("Checked remove");

            return TestResult.Pass;
        }

        TestResult TestRemoveVerify()
        {
            Status("Check remove");
            WaitForPriority(DispatcherPriority.Background);

            if (_lastAction != NotifyCollectionChangedAction.Remove)
            {
                LogComment("Expected Remove, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }
            return TestResult.Pass;
        }

        TestResult CopyTo()
        {
            Status("Testing CopyTo");
            _mylist = Array.CreateInstance( typeof(CollectionContainer), _cc.Count );

            _cc.CopyTo(_mylist, 0);
            return TestResult.Pass;
        }

        TestResult CopyToVerify()
        {
            if (_mylist.Length != 6)
            {
                LogComment("The copied array is does not have 6 items, it has " + _mylist.Length.ToString());
                return TestResult.Fail;
            }
            LogComment("Checked copyto");
            return TestResult.Pass;
        }

        TestResult Clear()
        {
            Status("Testing Clear");

            _cc.Clear();

            return TestResult.Pass;
        }

        TestResult ClearVerify()
        {
            WaitForPriority(DispatcherPriority.Background);

            if (_lastAction != NotifyCollectionChangedAction.Reset)
            {
                LogComment("Expected Reset, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            if (_cc.Count > 0)
            {
                LogComment("CollectionContainer has " + _cc.Count.ToString() + " expected 0");
                return TestResult.Fail;
            }

            if (_lb.Items.Count > 0)
            {
                LogComment("ListBox has " + _lb.Items.Count.ToString() + " items, expected 0");
                return TestResult.Fail;
            }
            LogComment("Checked clear");

            return TestResult.Pass;
        }

        TestResult Insert()
        {
            Status("Check insert");
            IEnumerator myEnumerator = _mylist.GetEnumerator();
            int i = 0;
            int cols = _mylist.GetLength(_mylist.Rank - 1);
            while (myEnumerator.MoveNext())
            {
                _cc.Insert(i, myEnumerator.Current);
                i++;
            }


            return TestResult.Pass;
        }

        TestResult InsertVerify()
        {
            WaitForPriority(DispatcherPriority.Background);

            if (_lastAction != NotifyCollectionChangedAction.Add)
            {
                LogComment("Expected Add, actual " + _lastAction.ToString());
                return TestResult.Fail;
            }

            if (_cc.Count != 6)
            {
                LogComment("CollectionContainer has " + _cc.Count.ToString() + " expected 6");
                return TestResult.Fail;
            }

            if (_lb.Items.Count != 12)
            {
                LogComment("ListBox has " + _lb.Items.Count.ToString() + " items, expected 12");
                return TestResult.Fail;
            }

            LogComment("Checked insert");
            return TestResult.Pass;
        }
        TestResult InterfaceProperties()
        {
            Status("Check ICollection properties, IList properties, and remove handler");

            ICollection coll = _cc as ICollection;
            if (coll == null)
            {
                LogComment("Could not cast to an ICollection");
                return TestResult.Fail;
            }

            if (coll.IsSynchronized)
            {
                LogComment("The collection is syncronized, it should not be");
                return TestResult.Fail;
            }

            if (coll.SyncRoot == null)
            {
                LogComment("The collection SyncRoot is null, it should not be.");
                return TestResult.Fail;
            }

            IList list = _cc as IList;
            if (list == null)
            {
                LogComment("Could not cast to an IList");
                return TestResult.Fail;
            }

            if (list.IsFixedSize)
            {
                LogComment("The Collection is a fixed size, it should not be");
                return TestResult.Fail;
            }

            if (list.IsReadOnly)
            {
                LogComment("The Collection is readonly, it should not be");
                return TestResult.Fail;
            }

            ((INotifyCollectionChanged)_cc).CollectionChanged -= _myhandler;

            LogComment("Checked ICollection properties, IList properties, removed handler");

            return TestResult.Pass;
        }
        TestResult ClearNoHandler()
        {
            Status("Calling Clear to verify that the handler was removed");

            _cc.Clear();
            return TestResult.Pass;
        }

        TestResult VerifyHandlerRemoved()
        {
            Status("Check handler removed");
            WaitForPriority(DispatcherPriority.Background);

            if (_lastAction != NotifyCollectionChangedAction.Add)
            {
                LogComment("Expected Add, actual " + _lastAction.ToString() + " because the CollectionChanged handler was removed");
                return TestResult.Fail;
            }

            if (_cc.Count > 0)
            {
                LogComment("CollectionContainer has " + _cc.Count.ToString() + " expected 0");
                return TestResult.Fail;
            }

            if (_lb.Items.Count > 0)
            {
                LogComment("ListBox has " + _lb.Items.Count.ToString() + " items, expected 0");
                return TestResult.Fail;
            }

            LogComment("Checked handler removed");
            return TestResult.Pass;
        }


        private void cc_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _lastAction = e.Action;
            Status("Collection Changed " + e.Action.ToString());
        }


    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   Base class for Freezable Collection tests
 
 *
 ************************************************************/

using System;
using System.Threading;
using System.Windows;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using Microsoft.Test;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{
    /**********************************************************************************
    * CLASS:          FreezableCollectionTest<T>
    **********************************************************************************/
    internal abstract class FreezableCollectionTest<T> where T: DependencyObject
    {
        internal FreezableCollectionTest()
        {
            result = new Microsoft.Test.ElementServices.Freezables.Utils.Result();
            collection = new FreezableCollection<T>();
        }
  
        internal FreezableCollection<T> Make()
        {
            return new FreezableCollection<T>();
        }
        //---------------------------------------------------------
        internal abstract Microsoft.Test.ElementServices.Freezables.Utils.Result Perform();
        internal abstract FreezableCollection<T> PopulateCollection();
        //---------------------------------------------------------
        
        internal void TestDefaultConstructor()
        {
            FreezableCollection<T> empty = new FreezableCollection<T>();
            if (empty.Count != 0)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + empty.ToString() + ": TestDefaultConstructor - Expected count = 0, actual count = " + empty.Count);
                GlobalLog.LogStatus("FAIL: " + empty.ToString() + ": TestDefaultConstructor - Expected count = 0, actual count = " + empty.Count);
            }
        }
 
        //---------------------------------------------------------
        internal void TestConstructorWith(int capacity)
        {
            FreezableCollection<T> collection = new FreezableCollection<T>(capacity);
            if (collection.Count != 0)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = 0, actual count = " + collection.Count);
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = 0, actual count = " + collection.Count);
            }
        }
        //---------------------------------------------------------
        internal void TestConstructorWith(FreezableCollection<T> freezables)
        {
            FreezableCollection<T> collection = new FreezableCollection<T>(freezables);
            if (collection.Count != freezables.Count)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = " + freezables.Count + ", actual count = " + collection.Count);
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = " + freezables.Count + ", actual count = " + collection.Count);
            }
        }
        //---------------------------------------------------------
        internal void TestConstructorWith( IEnumerable<T> freezables)
        {
            FreezableCollection<T> collection = new FreezableCollection<T>(freezables);
            if (collection.Count != 0)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = 0, actual count = " + collection.Count);
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestConstructorWith - Expected count = 0, actual count = " + collection.Count);
            }
        }
        //---------------------------------------------------------
        internal void TestAdd(List<T> objects)
        {
            foreach (object o in objects)
            {
                int oldCount = collection.Count;
                collection.Add(o as T);
                if (oldCount + 1 != collection.Count || !collection[oldCount].Equals(o))
                {
                    result.passed = false;
                    result.failures.Add("FAIL: " + collection.ToString() + ": TestAdd - Expected count = " + (oldCount+1) + ", Object added = " + o 
                        + " -Actual count = " + collection.Count + ", Object added = " + collection[oldCount]);
                    GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestAdd - Expected count = " + (oldCount + 1) + ", Object added = " + o
                        + " -Actual count = " + collection.Count + ", Object added = " + collection[oldCount]);
                }
            }
        }
        //---------------------------------------------------------
        internal void TestAddBadType(List<object> bogusCollection)
        {
            foreach (object o in bogusCollection)
            {
                try
                {
                    collection.Add(o as T);
                    result.passed = false;
                    result.failures.Add("FAIL: " + collection.ToString() + ": TestAddBadType - Expected System.ArgumentExeption thrown");   
                    GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestAddBadType - Expected System.ArgumentExeption thrown");      
                 
                }
                catch (System.ArgumentException e)
                {
                    //Console.WriteLine(e.ToString());
                    string s = e.ToString();
                }
            }
        }
        //----------------------------------------------
        internal void TestAddFromAnotherThread()
        {
            GlobalLog.LogStatus("Testing AddFromAnotherThread....");
            bool threwInvalidOparationException = false; 
            collection = PopulateCollection();
            T objectFromOtherThread = default(T);
            T item = collection[0];

            Thread thread = new Thread((ThreadStart)
                delegate
                {
                    try
                    {
                        collection.Add(item);
                    }
                    catch (InvalidOperationException)
                    {
                        threwInvalidOparationException = true;
                    }
                    if (!threwInvalidOparationException)
                    {
                        result.passed = false;
                        result.failures.Add("FAIL: " + collection.ToString() + ": TestAddFromAnotherThread - case#1 :Expected InvalidOperationException thrown");
                        GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestAddFromAnotherThread - case#1 :Expected InvalidOperationException thrown");
                    }
                    collection = PopulateCollection();
                    objectFromOtherThread = collection[0];
                }
            );

            thread.Start();
            thread.Join();
            if (objectFromOtherThread == null)
            {
                throw new ApplicationException("objectFromAnohterThread should not be null");
            }
            threwInvalidOparationException = false;
            collection = PopulateCollection();
            try
            {
                collection.Add(objectFromOtherThread);
            }
            catch (InvalidOperationException)
            {
                threwInvalidOparationException = true;
            }
            if (!threwInvalidOparationException)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestAddFromAnotherThread - case#2 :Expected InvalidOperationException thrown");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestAddFromAnotherThread - case#2 :Expected InvalidOperationException thrown");
            }
        }
   
        //----------------------------------------------
        internal void TestAddToFrozenCollection()
        {
            GlobalLog.LogStatus("Testing TestAddToFrozenCollection...");
            bool invalidOperationException = false;
            collection = PopulateCollection();
            T item = collection[0];
            collection.Freeze();
            try
            {
                collection.Add(item);
            }
            catch (System.InvalidOperationException)
            {
                invalidOperationException = true;
            }
            if (!invalidOperationException)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestAddToFrozenCollection - Expected System.InvalidOperationException thrown");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestAddToFrozenCollection - Expected SSystem.InvalidOperationException");
            }

        }
        //---------------------------------------------------------
        internal void TestChangedEvent(List<T> objects)
        {
            GlobalLog.LogStatus("Testing changed event...");

            TestChangedForAdd(objects);
            TestChangedForInsert(objects);
            TestChangedForRemove();
            TestChangedForRemoveAt();
            TestChangedForClear();
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForAdd(List<T> objects)
        {
            collection = new FreezableCollection<T>();
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = 0;

            foreach (object o in objects)
            {
                _callbacksExpected++;
                _objectsExpected++;
                collection.Add(o as T);
            }
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;

            foreach (object o in objects)
            {
                // No more callbacks expected
                _objectsExpected++;
                collection.Add(o as T);
            }
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForClear()
        {
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = 0;
            collection.Clear();

            _callbacksExpected++;
            collection.Clear();
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;
            // No more callbacks expected
            collection.Clear();
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForInsert(List<T> objects)
        {
            collection = new FreezableCollection<T>();
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 0;
            _objectsExpected = 0;

            foreach (object obj in objects)
            {
                _callbacksExpected++;
                _objectsExpected++;
                collection.Insert(0, obj as T);
            }
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;

            foreach (object obj in objects)
            {
                // No more callbacks expected
                _objectsExpected++;
                collection.Insert(0, obj as T);
            }
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForItem(List<T> objects)
        {
            collection = PopulateCollection();
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = objects.Count;

            collection[0] = objects[objects.Count - 1];
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;

            // No more callbacks expected
            collection[0] = objects[objects.Count - 1];
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForRemove()
        {
            collection = PopulateCollection();
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = collection.Count -1 ;

            // This should not invoke the changed handler
            collection.Remove(null);
            collection.Remove(collection[0]);
       
            // This should not invoke the changed handler
            collection.Remove(null);
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;
            collection = PopulateCollection();
            _objectsExpected = collection.Count;

            // This should not invoke the changed handler
            collection.Remove(null);
            collection.Remove(collection[0]);
            VerifyCallbacksProcessed();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void TestChangedForRemoveAt()
        {
            collection = PopulateCollection();
            collection.Changed += ChangedHandler;

            _callbacksProcessed = 0;
            _callbacksExpected = 1;
            _objectsExpected = collection.Count - 1;

            collection.RemoveAt(0);
            VerifyCallbacksProcessed();

            collection.Changed -= ChangedHandler;
            collection = PopulateCollection();
            collection.RemoveAt(0);
            VerifyCallbacksProcessed();
        }
        private void ChangedHandler(object sender, EventArgs args)
        {
            _callbacksProcessed++;

            // Do this partially because I want to verify "sender"
            //  and partially so I don't have to create a new collection variable
            //  and cast from "sender"
            if (!object.ReferenceEquals(sender, collection))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestChangedEvent - Expected = " + collection + ", Actual = " + sender);
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestChangedEvent - Expected = " + collection + ", Actual = " + sender);
            }

            if (collection.Count != _objectsExpected)
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + ": TestChangedEven # objects in collection is not many as we expected");
                result.failures.Add("Expected: " + _objectsExpected + ", Actual: "+ collection.Count);
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + ": TestChangedEven # objects in collection is not many as we expected");
                GlobalLog.LogStatus("Expected: " + _objectsExpected + ", Actual: " + collection.Count);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void VerifyCallbacksProcessed()
        {
            if (_callbacksProcessed != _callbacksExpected)
            {
                result.passed = false;
                result.failures.Add("Callbacks processed and callbacks expected do not match");
                result.failures.Add("Expected: " + _callbacksExpected + ", Actual: " + _callbacksProcessed);
                GlobalLog.LogStatus("Callbacks processed and callbacks expected do not match");
                GlobalLog.LogStatus("Expected: " + _callbacksExpected + ", Actual: " + _callbacksProcessed);
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        internal void TestClear()
        {
            GlobalLog.LogStatus("Testing Clear...");

            // Call Clear on a list with items in it
            collection = PopulateCollection();
            collection.Clear();
            if (collection.Count != 0)
            {
                result.passed = false;
                result.failures.Add("FAIL: TestClear() for " + collection.ToString());
                result.failures.Add("Expected count = 0, actual count = " + collection.Count);
                GlobalLog.LogStatus("FAIL: TestClear() for " + collection.ToString());
                GlobalLog.LogStatus("Expected count = 0, actual count = " + collection.Count);
            }
            foreach (object o in collection)
            {
                result.passed = false;
                result.failures.Add("There shouldn't be any objects in the collection despite what \"Count\" says");
                GlobalLog.LogStatus("There shouldn't be any objects in the collection despite what \"Count\" says");
                break;
            }

            // Call clear on an empty list
            collection = new FreezableCollection<T>();
            collection.Clear();
            if (collection.Count != 0)
            {
                result.passed = false;
                result.failures.Add("FAIL: TestClear() for " + collection.ToString());
                result.failures.Add("Expected count = 0, actual count = " + collection.Count);
                GlobalLog.LogStatus("FAIL: TestClear() for " + collection.ToString());
                GlobalLog.LogStatus("Expected count = 0, actual count = " + collection.Count);
          }
            foreach (object o in collection)
            {
                result.passed = false;
                result.failures.Add("There shouldn't be any objects in the collection despite what \"Count\" says");
                GlobalLog.LogStatus("There shouldn't be any objects in the collection despite what \"Count\" says");
                break;
            }
        }
        //---------------------------------------------
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        internal void TestContains()
        {
            GlobalLog.LogStatus("Testing Contains...");
            collection = PopulateCollection();

            foreach (object obj in collection)
            {
                if (!collection.Contains(obj as T))
                {
                    result.passed = false;
                    result.failures.Add("Contains() failed, Could not locate " + obj + "in my collection");
                    GlobalLog.LogStatus("Contains() failed, Could not locate " + obj + "in my collection");
                }
            }
            T temp = collection[0];
            collection.Remove(collection[0]);
            if (collection.Contains(temp))
            {
                result.passed = false;
                result.failures.Add("Contains() failed, should not have located " + temp + "in my collection");
                GlobalLog.LogStatus("Contains() failed, should not have located " + temp + "in my collection");
            }

        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

       
        internal void TestCopyTo()
        {
            GlobalLog.LogStatus("Testing CopyTo()...");
            collection = PopulateCollection();

            T[] array = new T[collection.Count];
            collection.CopyTo(array, 0);

            for (int i = 0; i < collection.Count; i++)
            {
                if (!collection[i].Equals(array[i]))
                {
                    result.passed = false;
                    result.failures.Add("CopyTo() failed, Expected: " + collection[i] + " Actual: " + array[i]);
                    GlobalLog.LogStatus("CopyTo() failed, Expected: " + collection[i] + " Actual: " + array[i]);
                    break;
                }
            }

        }

        //-------------------------------------------------
        internal void TestCount()
        {
            GlobalLog.LogStatus("Testing Count...");
            collection = PopulateCollection();

            int count = 0;
            int theirAnswer = collection.Count;

            foreach (object o in collection)
            {
                count++;
            }
            if (count != theirAnswer )
            {
                result.passed = false;
                result.failures.Add("Count failed, Expected count: " + count + " Actual: " + theirAnswer);
                GlobalLog.LogStatus("Count failed, Expected count: " + count + " Actual: " + theirAnswer);
            }

            // Make sure list really is cleared and that Count is updated
            collection.Clear();
            count = 0;
            theirAnswer = collection.Count;

            foreach (object o in collection)
            {
                count++;
            }
            if (count != theirAnswer)
            {
                result.passed = false;
                result.failures.Add("Count failed, Expected count: " + count + " Actual: " + theirAnswer);
                GlobalLog.LogStatus("Count failed, Expected count: " + count + " Actual: " + theirAnswer);
            }
        }
        //---------------------------------------
        internal virtual void TestDataBinding()
        {
            throw new ApplicationException("TestDataBinding() - derived class needs to implement this method");        
        }

        //---------------------------------------
        internal void TestEnumerator()
        {
            GlobalLog.LogStatus("Testing Enumerator...");
            collection = PopulateCollection();
            int i = 0;
            foreach (T coll in collection)
            {
                if (!object.ReferenceEquals(coll, collection[i++]))
                {
                    result.passed = false;
                    result.failures.Add("FAIL: Enumerator for " + collection.ToString());
                    result.failures.Add("Expected: " + collection[i++] + "Actual: " + coll);
                    GlobalLog.LogStatus("FAIL: Enumerator for " + collection.ToString());
                    GlobalLog.LogStatus("Expected: " + collection[i++] + "Actual: " + coll);
                }
            }
        }
      
        //---------------------------------
        internal void TestGetAsFrozen()
        {
            GlobalLog.LogStatus("Testing GetAsFrozen...");

            collection = PopulateCollection();

            // We clone the collection first so that the test objects are not permanently frozen
            FreezableCollection<T> copy = collection.Clone();

            if (object.ReferenceEquals(copy, collection) ||
                 !ObjectUtils.DeepEqualsToAnimatable(copy, collection))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + " Clone should always return a deep copy");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + " Clone should always return a deep copy");
            }

            Freezable frozen = copy.GetAsFrozen();
            if (object.ReferenceEquals(frozen, copy) ||
                 !ObjectUtils.DeepEqualsToAnimatable(frozen, copy))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + "GetAsFrozen should return a deep copy if the collection is not frozen");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + "GetAsFrozen should return a deep copy if the collection is not frozen");
            }

            copy.Freeze();
            frozen = copy.GetAsFrozen();
            if (!object.ReferenceEquals(frozen, copy))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + "GetAsFrozen should return a shallow copy if the collection is frozen");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + "GetAsFrozen should return a shallow copy if the collection is frozen");
            }

        }
        //--------------------------------------------
        internal void TestGetCurrentValueAsFrozen()
        {
            GlobalLog.LogStatus("Testing GetCurrentValueAsFrozen...");

            collection = PopulateCollection();
            // We clone the collection first so that the test objects are not permanently frozen
            FreezableCollection<T> copy = collection.Clone();

            if (object.ReferenceEquals(copy, collection) ||
                 !ObjectUtils.DeepEqualsToAnimatable(copy, collection))
                 
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + "Clone should always return a deep copy");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + "Clone should always return a deep copy");
            }

            Freezable frozen = copy.GetCurrentValueAsFrozen();
            if (object.ReferenceEquals(frozen, copy) ||
                 !ObjectUtils.DeepEqualsToAnimatable(frozen, copy))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + "GetCurrentValueAsFrozen should return a deep copy if the collection is not frozen");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + "GetCurrentValueAsFrozen should return a deep copy if the collection is not frozen");
            }

            copy.Freeze();
            frozen = copy.GetCurrentValueAsFrozen();
            if (!object.ReferenceEquals(frozen, copy))
            {
                result.passed = false;
                result.failures.Add("FAIL: " + collection.ToString() + "GetCurrentValueAsFrozen should return a shallow copy if the collection is frozen");
                GlobalLog.LogStatus("FAIL: " + collection.ToString() + "GetCurrentValueAsFrozen should return a shallow copy if the collection is frozen");
            }
        }
        //-------------------------------------
        internal void TestIndexOf()
        {
            GlobalLog.LogStatus("Testing IndexOf( )...");
            collection = PopulateCollection();
            T [] objects = new T[collection.Count];
            for (int index = 0; index < collection.Count; index++)
            {
                objects[index] = collection[index];
            }
            for (int i = 0; i < collection.Count; i++)
            {
                int position = collection.IndexOf(objects[i]);
                if (position != i )
                {
                    result.passed = false;
                    result.failures.Add("FAIL: IndexOf for " + collection.ToString());
                    result.failures.Add("Expected: " + objects[i] + " at postion " + i);
                    result.failures.Add("Actual: " + objects[i] + " at postion " + position);
                    GlobalLog.LogStatus("FAIL: IndexOf for " + collection.ToString());
                    GlobalLog.LogStatus("Expected: " + objects[i] + " at postion " + i);
                    GlobalLog.LogStatus("Actual: " + objects[i] + " at postion " + position);
                }
            }
        }
        //-------------------------------------
        internal void TestInsert()
        {
            GlobalLog.LogStatus("Testing Insert...");
            collection = PopulateCollection();
            FreezableCollection<T> cloned = collection.Clone();
            int count = collection.Count;
            collection.Insert(0, cloned[cloned.Count - 1]);

            if (count + 1 != collection.Count ||
                !collection[0].Equals(cloned[cloned.Count - 1]))
            {
                result.passed = false;
                result.failures.Add("FAIL: Insert for " + collection.ToString());
                result.failures.Add("Expected count: " + (count+1) + "first object = " + cloned[cloned.Count - 1]);
                result.failures.Add("Actual count: " + collection.Count + "first object = " + collection[0]);
                GlobalLog.LogStatus("FAIL: Insert for " + collection.ToString());
                GlobalLog.LogStatus("Expected count: " + (count + 1) + "first object = " + cloned[cloned.Count - 1]);
                GlobalLog.LogStatus("Actual count: " + collection.Count + "first object = " + collection[0]);
            }

            collection = PopulateCollection();
            cloned = collection.Clone();
            collection.Insert(count - 1, cloned[0]);

            if (count + 1 != collection.Count ||
                !collection[collection.Count - 2].Equals(cloned[0]))
            {
                result.passed = false;
                result.failures.Add("FAIL: Insert for " + collection.ToString());
                result.failures.Add("Expected count: " + (count + 1) + "2nd to last object = " + cloned[0]);
                result.failures.Add("Actual count: " + collection.Count + "first object = " + collection[collection.Count -2]);
                GlobalLog.LogStatus("FAIL: Insert for " + collection.ToString());
                GlobalLog.LogStatus("Expected count: " + (count + 1) + "2nd to last object = " + cloned[0]);
                GlobalLog.LogStatus("Actual count: " + collection.Count + "first object = " + collection[collection.Count - 2]);
            }

        }
        /*
        internal void TestIsFixedSize()
        {
            GlobalLog.LogStatus("Testing IsFixedSize...");
            collection = PopulateCollection();

            bool theirAnswer = ((ICollection<T>)collection).IsFixedSize;
            if (theirAnswer)
            {
                result.passed = false;
                result.failures.Add("FAIL: IsFixedSize for " + collection.ToString());
                result.failures.Add("A growing collection should not have a fixed size");
                GlobalLog.LogStatus("FAIL: Insert for " + collection.ToString());
                GlobalLog.LogStatus("A growing collection should not have a fixed size");
            }

            collection.Freeze();
            theirAnswer = ((ICollection<T>)collection).IsFixedSize;
            if (!theirAnswer)
            {
                result.passed = false;
                result.failures.Add("FAIL: IsFixedSize for " + collection.ToString());
                result.failures.Add("A frozen collection should be fixed in size");
                GlobalLog.LogStatus("FAIL: Insert for " + collection.ToString());
                GlobalLog.LogStatus("A frozen collection should be fixed in size");
            }
        }
        */
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        internal void TestIsReadOnly()
        {
            GlobalLog.LogStatus("Testing IsReadOnly...");
            collection = PopulateCollection();
            bool theirAnswer = ((ICollection<T>)collection).IsReadOnly;
            if (theirAnswer)
            {
                result.passed = false;
                result.failures.Add("FAIL: IsFixedSize for " + collection.ToString());
                result.failures.Add("A growing collection should not be read-only");
                GlobalLog.LogStatus("FAIL: IsFixedSize for " + collection.ToString());
                GlobalLog.LogStatus("A growing collection should not be read-only");
            }

            collection.Freeze();
            theirAnswer = ((ICollection<T>)collection).IsReadOnly;
            if (!theirAnswer)
            {
                result.passed = false;
                result.failures.Add("FAIL: IsFixedSize for " + collection.ToString());
                result.failures.Add("A frozen collection should be read-only");
                GlobalLog.LogStatus("FAIL: IsFixedSize for " + collection.ToString());
                GlobalLog.LogStatus("A frozen collection should be read-only");
            }
        }

        //-------------------------------------
        internal void TestItem()
        {
            GlobalLog.LogStatus("Testing IList.get/set_Item...");
            collection = PopulateCollection();
            FreezableCollection<T> objects = collection.Clone();
            collection[0] = objects[objects.Count-1];
            if (!collection[0].Equals(objects[objects.Count - 1]))
            {
                result.passed = false;
                result.failures.Add("IList.get/set_Item failed for " + collection.ToString());
                result.failures.Add("Expected: item[0] = " + objects[objects.Count -1]);
                result.failures.Add("Actual: item[0] = " + collection[0]);
                GlobalLog.LogStatus("IList.get/set_Item failed for " + collection.ToString());
                GlobalLog.LogStatus("Expected: item[0] = " + objects[objects.Count - 1]);
                GlobalLog.LogStatus("Actual: item[0] = " + collection[0]);
            }

        }
        //-------------------------------------

        internal void TestRemove()
        {
            GlobalLog.LogStatus("Testing Remove...");
            collection = PopulateCollection();
            T temp = collection[0];
            int count = collection.Count;
            collection.Remove(temp);
            if (count - 1 != collection.Count || collection.IndexOf(temp) != -1)
            {
                result.passed = false;
                result.failures.Add("Remove failed for " + collection.ToString());
                result.failures.Add("Expected: item[0] = " + collection[collection.Count - 1]);
                result.failures.Add("Actual: item[0] = " + collection[0]);
                result.failures.Add("Expected: count = "+ (count-1) + " and object should not be found");
                if (count - 1 == collection.Count)
                {
                    result.failures.Add("Actual: count = " + collection.Count + " and object was not removed");
                }
                else
                {
                    result.failures.Add("Actual: count = " + collection.Count);
                }
            }
        }

        internal void TestRemoveAt()
        {
            GlobalLog.LogStatus("Testing RemoveAt...");
            collection = PopulateCollection();

            T[] myCollection = new T[collection.Count];
            collection.CopyTo(myCollection, 0);

            collection.RemoveAt(0);
            myCollection[0] = null;

            VerifyRemoveAt(myCollection);

            collection = PopulateCollection();
            collection.CopyTo(myCollection, 0);
            collection.RemoveAt(collection.Count - 1);
            myCollection[myCollection.Length - 1] = null;

            VerifyRemoveAt(myCollection);
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        internal void VerifyRemoveAt(T [] myList)
        {
            int myIterator = 0;
            int theirIterator = 0;
            while (theirIterator < collection.Count)
            {
                if (myList[myIterator] == null)
                {
                    myIterator++;
                }
                if (!collection[theirIterator].Equals(myList[myIterator]))
                {
                    result.passed = false;
                    result.failures.Add("RemoveAt failed for " + collection.ToString());
                    result.failures.Add("Expected: object # " + theirIterator + " == " + myList[myIterator]);
                    result.failures.Add("Expected: object # " + theirIterator + " == " + collection[myIterator]);
                    GlobalLog.LogStatus("RemoveAt failed for " + collection.ToString());
                    GlobalLog.LogStatus("Expected: object # " + theirIterator + " == " + myList[myIterator]);
                    GlobalLog.LogStatus("Expected: object # " + theirIterator + " == " + collection[myIterator]);
                    break;
                }
                myIterator++;
                theirIterator++;
            }
        }
        private int _callbacksProcessed;
        private int _objectsExpected;
        private int _callbacksExpected;
        protected Microsoft.Test.ElementServices.Freezables.Utils.Result result;
        protected FreezableCollection<T> collection;
    }
}

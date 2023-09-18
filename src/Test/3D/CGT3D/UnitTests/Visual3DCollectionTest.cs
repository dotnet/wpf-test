// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{    
    /// <summary>
    /// Test Visual3DCollection (based on tests from CodeGen)
    /// </summary>
    public class Visual3DCollectionTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            _collection = NewVisual3DCollection();
            _list = NewVisual3DCollection();
            _objects = new List<Visual3D>();
            _extraObjects = new List<Visual3D>();
            _bogusCollection = new ArrayList();

            _objects.Add(new ModelVisual3D());
            _objects.Add(Const.mesh);
            _objects.Add(Const.light);
#if SSL
            objects.Add( Const.lines );
#endif
            _objects.Add(Const.group);
            _objects.Add(Const.children);

            _extraObjects.Add(new ModelVisual3D());
            _extraObjects.Add(Const.mesh);
            _extraObjects.Add(Const.light);
#if SSL
            extraObjects.Add( Const.lines );
#endif
            _extraObjects.Add(Const.group);
            _extraObjects.Add(Const.children);

            _bogusCollection.Add(10);
            _bogusCollection.Add(new TranslateTransform());
            _bogusCollection.Add(new Point3D());
            _bogusCollection.Add(null);
            _bogusCollection.Add(new Point());
            _bogusCollection.Add(new Vector3D());
            _bogusCollection.Add(new Matrix3D());
            _bogusCollection.Add(true);
        }

        /// <summary/>
        public override void RunTheTest()
        {
            if (priority == 0)
            {
                TestAdd();
                TestClear();
                TestContains();
                TestCopyTo();
                TestCount();
                TestEnumerator();
                TestIndexOf();
                TestInsert();
                TestIsFixedSize();
                TestIsReadOnly();
                TestIsSynchronized();
                TestItem();
                TestRemove();
                TestRemoveAt();
                TestSyncRoot();
            }
            else // priority > 0
            {
                TestAdd2();
                TestContains2();
                TestCopyTo2();
                TestEnumerator2();
                TestIndexOf2();
                TestInsert2();
                TestItem2();
                TestRemove2();
                TestRemoveAt2();
            }
        }

        private Visual3DCollection NewVisual3DCollection()
        {
            if (_list != null)
            {
                _list.Clear();
            }
            if (_collection != null)
            {
                _collection.Clear();
            }
            ModelVisual3D visual = new ModelVisual3D();
            return visual.Children;
        }

        private void PopulateCollection()
        {
            if (_collection != null)
            {
                _collection.Clear();
            }
            _collection = NewVisual3DCollection();
            foreach (Visual3D obj in _objects)
            {
                _collection.Add(obj);
            }
        }

        private void PopulateList()
        {
            _list = NewVisual3DCollection();
            foreach (Visual3D obj in _objects)
            {
                _list.Add(obj);
            }
        }

        private void TestAdd()
        {
            Log("Testing Add...");
            _list = NewVisual3DCollection();

            foreach (object o in _objects)
            {
                TestAddWith(o);
            }

            // Remove references to added visuals
            _list = NewVisual3DCollection();

            _collection = NewVisual3DCollection();
            foreach (Visual3D v in _objects)
            {
                TestAddWith(v);
            }
        }

        private void TestAddWith(object o)
        {
            int oldCount = _list.Count;
            int position = _list.Add(o);

            if (oldCount + 1 != _list.Count || position != oldCount ||
                    !_list[position].Equals(o) || failOnPurpose)
            {
                AddFailure("IList.Add failed");
                Log("*** Expected: count = {0}, position = {1}, object added = {2}", oldCount + 1, oldCount, o);
                Log("***   Actual: count = {0}, position = {1}, object added = {2}", _list.Count, position, _list[position]);
            }
        }

        private void TestAddWith(Visual3D v)
        {
            int oldCount = _collection.Count;
            _collection.Add(v);

            if (oldCount + 1 != _collection.Count || !_collection[oldCount].Equals(v) || failOnPurpose)
            {
                AddFailure("Visual3DCollection.Add failed");
                Log("*** Expected: count = {0}, Visual3D added = {1}", oldCount + 1, v);
                Log("***   Actual: count = {0}, Visual3D added = {1}", _collection.Count, _collection[oldCount]);
            }
        }

        private void TestAdd2()
        {
            Log("Testing IList.Add( object ) with bad parameters...");

            foreach (object o in _bogusCollection)
            {
                Type itemType = (o == null) ? null : o.GetType();
                Type addType = typeof(Visual3D);
                try
                {
                    _list.Add(o);
                    if (!itemType.IsSubclassOf(addType) && !itemType.Equals(addType))
                    {
                        AddFailure("Should not be able to add object of incorrect type to this collection");
                    }
                }
                catch (ArgumentException)
                {
                    if (itemType == null || !itemType.IsSubclassOf(addType))
                    {
                        Log("  Good! - Invalid cast detected when adding object of incorrect type");
                    }
                    else
                    {
                        AddFailure("Invalid cast detected when adding object of correct type");
                    }
                }
            }

            Try(AddNullToList, typeof(ArgumentException));

            Log("Testing Visual3DCollection.Add with bad parameters...");

            Try(AddNullToCollection, typeof(ArgumentException));
        }

        #region ExceptionThrowers for Add

        private void AddNullToList()
        {
            _list = NewVisual3DCollection();
            _list.Add(null);
        }

        private void AddNullToCollection()
        {
            _collection = NewVisual3DCollection();
            _collection.Add(null);
        }

        #endregion

        private void TestClear()
        {
            Log("Testing Clear...");

            // Call Clear on a list with items in it
            PopulateList();
            _list.Clear();
            if (_list.Count != 0 || failOnPurpose)
            {
                AddFailure("Clear failed");
                Log("*** Expected: count = 0");
                Log("***   Actual: count = {0}", _list.Count);
            }
            foreach (object o in _list)
            {
                AddFailure("There shouldn't be any objects in the list despite what \"Count\" says");
                break;
            }

            // Call clear on an empty list
            _list = NewVisual3DCollection();
            _list.Clear();
            if (_list.Count != 0 || failOnPurpose)
            {
                AddFailure("Clear failed");
                Log("*** Expected: count = 0");
                Log("***   Actual: count = {0}", _list.Count);
            }
            foreach (object o in _list)
            {
                AddFailure("There shouldn't be any objects in the list despite what \"Count\" says");
                break;
            }
        }

        private void TestContains()
        {
            Log("Testing IList.Contains...");
            PopulateList();

            foreach (Visual3D obj in _objects)
            {
                if (!_list.Contains(obj) || failOnPurpose)
                {
                    AddFailure("IList.Contains failed");
                    Log("Could not locate {0} in my collection", obj);
                }
            }

            _list.Remove(_objects[0]);
            if (_list.Contains(_objects[0]) || failOnPurpose)
            {
                AddFailure("IList.Contains failed");
                Log("Should not have located {0} in my collection", _objects[0]);
            }

            Log("Testing Visual3DCollection.Contains...");
            PopulateCollection();

            foreach (Visual3D obj in _objects)
            {
                if (!_collection.Contains(obj) || failOnPurpose)
                {
                    AddFailure("Visual3DCollection.Contains failed");
                    Log("Could not locate {0} in my collection", obj);
                }
            }

            _collection.Remove(_objects[0]);
            if (_collection.Contains(_objects[0]) || failOnPurpose)
            {
                AddFailure("Visual3DCollection.Contains failed");
                Log("Should not have located {0} in my collection", _objects[0]);
            }
        }

        private void TestContains2()
        {
            Log("Testing IList.Contains( object ) with bad parameters...");

            foreach (object o in _bogusCollection)
            {
                Type itemType = (o == null) ? null : o.GetType();
                Type listType = typeof(Visual3D);
                if (listType.Equals(itemType))
                {
                    // We're not looking for items of the correct type here
                    continue;
                }
                try
                {
                    bool theirAnswer = _list.Contains(o);
                    if (theirAnswer != false || failOnPurpose)
                    {
                        AddFailure("Found item in collection that should not be there");
                        Log("*** Found: " + o);
                    }
                }
                catch (Exception)
                {
                    AddFailure("Should just return false when asking if collection contains wrong type");
                }
            }
        }

        private void TestCopyTo()
        {
            Log("Testing ICollection.CopyTo...");
            PopulateList();

            object[] array = new object[_objects.Count];
            _list.CopyTo(array, 0);

            for (int i = 0; i < _list.Count; i++)
            {
                if (!_list[i].Equals(array[i]) || failOnPurpose)
                {
                    AddFailure("ICollection.CopyTo( Array,int ) failed");
                    Log("*** Expected: {0}", _list[i]);
                    Log("***   Actual: {0}", array[i]);
                    break;
                }
            }

            Log("Testing Visual3DCollection.CopyTo...");
            PopulateCollection();
            Visual3D[] typedArray = new Visual3D[_objects.Count];
            _collection.CopyTo(typedArray, 0);

            for (int i = 0; i < _collection.Count; i++)
            {
                if (!_collection[i].Equals(typedArray[i]) || failOnPurpose)
                {
                    AddFailure("CopyTo( Visual3D[],int ) failed");
                    Log("*** Expected: {0}", _collection[i]);
                    Log("***   Actual: {0}", typedArray[i]);
                    break;
                }
            }
        }

        private void TestCopyTo2()
        {
            Log("Testing ICollection.CopyTo with bad parameters...");

            Try(CopyToNullArray, typeof(ArgumentException));
            Try(CopyToMultidimensionalArray, typeof(ArgumentException));
            Try(CopyToWrongTypeArray, typeof(ArgumentException));
            Try(CopyToSmallerArray, typeof(ArgumentOutOfRangeException));
            Try(CopyToNegativeStartPosition, typeof(ArgumentOutOfRangeException));
            Try(CopyToNotEnoughRoomPosition, typeof(ArgumentOutOfRangeException));

            Log("Testing Visual3DCollection.CopyTo with bad parameters...");

            Try(TypedCopyToNullArray, typeof(ArgumentException));
            Try(TypedCopyToMultidimensionalArray, typeof(ArgumentException));
            Try(TypedCopyToSmallerArray, typeof(ArgumentOutOfRangeException));
            Try(TypedCopyToNegativeStartPosition, typeof(ArgumentOutOfRangeException));
            Try(TypedCopyToNotEnoughRoomPosition, typeof(ArgumentOutOfRangeException));
        }

        #region ExceptionThrowers for CopyTo

        private void CopyToNullArray()
        {
            PopulateCollection();
            object[] array = null;
            ((ICollection)_collection).CopyTo(array, 0);
        }

        private void CopyToMultidimensionalArray()
        {
            PopulateCollection();
            object[,] array = new object[_objects.Count, _objects.Count];
            ((ICollection)_collection).CopyTo(array, 0);
        }

        private void CopyToWrongTypeArray()
        {
            PopulateCollection();
            bool[] array = new bool[_objects.Count];
            ((ICollection)_collection).CopyTo(array, 0);
        }

        private void CopyToSmallerArray()
        {
            PopulateCollection();
            object[] array = new object[_objects.Count - 1];
            ((ICollection)_collection).CopyTo(array, 0);
        }

        private void CopyToNegativeStartPosition()
        {
            PopulateCollection();
            object[] array = new object[_objects.Count];
            ((ICollection)_collection).CopyTo(array, -2);
        }

        private void CopyToNotEnoughRoomPosition()
        {
            PopulateCollection();
            object[] array = new object[_objects.Count];
            ((ICollection)_collection).CopyTo(array, 1);
        }

        private void TypedCopyToNullArray()
        {
            PopulateCollection();
            Visual3D[] array = null;
            _collection.CopyTo(array, 0);
        }

        private void TypedCopyToMultidimensionalArray()
        {
            PopulateList();
            Visual3D[,] array = new Visual3D[_objects.Count, _objects.Count];
            _list.CopyTo(array, 0);
        }

        private void TypedCopyToSmallerArray()
        {
            PopulateCollection();
            Visual3D[] array = new Visual3D[_objects.Count - 1];
            _collection.CopyTo(array, 0);
        }

        private void TypedCopyToNegativeStartPosition()
        {
            PopulateCollection();
            Visual3D[] array = new Visual3D[_objects.Count];
            _collection.CopyTo(array, -2);
        }

        private void TypedCopyToNotEnoughRoomPosition()
        {
            PopulateCollection();
            Visual3D[] array = new Visual3D[_objects.Count];
            _collection.CopyTo(array, 1);
        }

        #endregion

        private void TestCount()
        {
            Log("Testing Count...");
            PopulateList();

            int count = 0;
            int theirAnswer = _list.Count;

            foreach (object o in _list)
            {
                count++;
            }
            if (count != theirAnswer || failOnPurpose)
            {
                AddFailure("Count failed");
                Log("*** Expected: {0}", count);
                Log("***   Actual: {0}", theirAnswer);
            }

            // Make sure list really is cleared and that Count is updated
            _list.Clear();
            count = 0;
            theirAnswer = _list.Count;

            foreach (object o in _list)
            {
                count++;
            }
            if (count != theirAnswer || failOnPurpose)
            {
                AddFailure("Count failed");
                Log("*** Expected: {0}", count);
                Log("***   Actual: {0}", theirAnswer);
            }
        }

        private void TestEnumerator()
        {
            Log("Testing Visual3DEnumerator...");
            PopulateCollection();

            TestEnumeratorWith(NewVisual3DCollection());
            TestEnumeratorWith(_collection);
        }

        private void TestEnumeratorWith(Visual3DCollection collection)
        {
            IEnumerator enumerator = ((IEnumerable)collection).GetEnumerator();

            Log("Testing MoveNext and get_Current...");
            TestMoveNextWith(enumerator, collection);

            Log("Testing Reset...");
            enumerator.Reset();
            int currentFailures = Failures;
            TestMoveNextWith(enumerator, collection);
            if (currentFailures < Failures)
            {
                Log("The above MoveNext failures were actually caused by a failing Reset function");
            }
        }

        private void TestMoveNextWith(IEnumerator enumerator, Visual3DCollection collection)
        {
            int count = 0;
            while (enumerator.MoveNext())
            {
                Visual3D current = (Visual3D)enumerator.Current;
                if (!collection[count].Equals(current) || failOnPurpose)
                {
                    AddFailure("Enumerator failed on object {0}", count);
                    Log("*** Expected: Current = {0}", current);
                    Log("***   Actual: Current = {0}", collection[count]);
                }
                count++;
            }

            if (collection.Count != count || failOnPurpose)
            {
                AddFailure("MoveNext failed");
                Log("*** Expected: should be called exactly {0} times", collection.Count + 1);
                Log("***   Actual: was called {0} times", count + 1);
            }
        }

        private void TestEnumerator2()
        {
            Log("Testing Enumerator with bad parameters...");
            PopulateCollection();

            IEnumerator enumerator = ((IEnumerable)_collection).GetEnumerator();
            enumerator.Reset();

            try
            {
                object foo = enumerator.Current;
                AddFailure("Current shouldn't be accessible until MoveNext is called");
            }
            catch (InvalidOperationException)
            {
                Log("  Good!  Invalid operation exception caught for accessing Current before MoveNext is called");
            }

            try
            {
                while (enumerator.MoveNext()) ;

                object foo = enumerator.Current;
                AddFailure("Current shouldn't be accessible after MoveNext returns false");
            }
            catch (InvalidOperationException)
            {
                Log("  Good!  Invalid operation exception caught for accessing Current after MoveNext returns false");
            }

            try
            {
                foreach (Visual3D o in _collection)
                {
                    _collection.Remove(o);
                }
                AddFailure("Should not be able to modify collection and enumerate at the same time");
            }
            catch (InvalidOperationException)
            {
                Log("  Good!  Invalid Operation exception caught for modify + enumerate collection at the same time");
            }

            try
            {
                PopulateCollection();
                enumerator = _collection.GetEnumerator();
                enumerator.MoveNext();
                _collection.Remove(_objects[0]);
                enumerator.Reset();
                AddFailure("Should not be able to modify collection and reset enumerator");
            }
            catch (InvalidOperationException)
            {
                Log("  Good!  Invalid Operation exception caught for modify + reset enumerator");
            }

            try
            {
                PopulateCollection();
                enumerator = ((IEnumerable)new Model3DCollection()).GetEnumerator();
                object foo = enumerator.Current;
                AddFailure("Empty collections cannot have Current elements");
            }
            catch (InvalidOperationException)
            {
                Log("  Good!  Invalid operation exception caught for accessing Current from empty collection");
            }
        }

        private void TestIndexOf()
        {
            Log("Testing IList.IndexOf( object )...");
            PopulateList();

            for (int i = 0; i < _list.Count; i++)
            {
                int position = _list.IndexOf(_objects[i]);
                if (position != i || failOnPurpose)
                {
                    AddFailure("IList.IndexOf failed");
                    Log("*** Expected: {0} at position {1}", _objects[i], i);
                    Log("***   Actual: {0} at position {1}", _objects[i], position);
                }
            }

            Log("Testing Visual3DCollection.IndexOf( Visual3D )...");
            PopulateCollection();

            for (int i = 0; i < _collection.Count; i++)
            {
                int position = _collection.IndexOf(_objects[i]);
                if (position != i || failOnPurpose)
                {
                    AddFailure("IList.IndexOf failed");
                    Log("*** Expected: {0} at position {1}", _objects[i], i);
                    Log("***   Actual: {0} at position {1}", _objects[i], position);
                }
            }
        }

        private void TestIndexOf2()
        {
            Log("Testing IList.IndexOf( object ) with bad parameters...");

            SafeExecute(IndexOfWrongType);

            Log("Testing Visual3DCollection.IndexOf( Visual3D ) with bad parameters...");

            SafeExecute(IndexOfNullItem);
            SafeExecute(IndexOfNonExistentItem);
        }

        #region SafeExecutionBlocks for IndexOf

        private void IndexOfWrongType()
        {
            PopulateList();

            int index = _list.IndexOf(true);
            if (index != -1 || failOnPurpose)
            {
                AddFailure("IndexOf should return -1 when wrong type is passed in");
                Log("*** Actual: " + index);
            }
        }

        private void IndexOfNullItem()
        {
            PopulateCollection();
            int index = _collection.IndexOf(null);
            if (index != -1 || failOnPurpose)
            {
                AddFailure("IndexOf should return -1 when null is passed in");
                Log("*** Actual: " + index);
            }
        }

        private void IndexOfNonExistentItem()
        {
            _collection = NewVisual3DCollection();
            int index = _collection.IndexOf(_objects[0]);
            if (index != -1 || failOnPurpose)
            {
                AddFailure("IndexOf should return -1 when non existent item is passed in");
                Log("*** Actual: " + index);
            }

            PopulateCollection();
            _collection.RemoveAt(0);
            index = _collection.IndexOf(_objects[0]);
            if (index != -1 || failOnPurpose)
            {
                AddFailure("IndexOf should return -1 when non existent item is passed in");
                Log("*** Actual: " + index);
            }
        }

        #endregion

        private void TestInsert()
        {
            Log("Testing IList.Insert...");
            PopulateList();

            int count = _list.Count;
            ModelVisual3D visual = new ModelVisual3D();
            _list.Insert(0, visual);

            if (count + 1 != _list.Count ||
                !_list[0].Equals(visual) || failOnPurpose)
            {
                AddFailure("IList.Insert failed");
                Log("*** Expected: count = {0}, first object = {1}", count + 1, visual);
                Log("***   Actual: count = {0}, first object = {1}", _list.Count, _list[0]);
            }

            PopulateList();
            visual = new ModelVisual3D();
            _list.Insert(count - 1, visual);

            if (count + 1 != _list.Count ||
                !_list[_list.Count - 2].Equals(visual) || failOnPurpose)
            {
                AddFailure("IList.Insert failed");
                Log("*** Expected: count = {0}, 2nd to last object = {1}", count + 1, visual);
                Log("***   Actual: count = {0}, 2nd to last object = {1}", _list.Count, _list[_list.Count - 2]);
            }

            Log("Testing Visual3DCollection.Insert...");
            PopulateCollection();

            count = _collection.Count;
            visual = new ModelVisual3D();
            _collection.Insert(0, visual);

            if (count + 1 != _collection.Count ||
                !_collection[0].Equals(visual) || failOnPurpose)
            {
                AddFailure("Insert failed");
                Log("*** Expected: count = {0}, first object = {1}", count + 1, visual);
                Log("***   Actual: count = {0}, first object = {1}", _collection.Count, _collection[0]);
            }

            PopulateCollection();
            count = _collection.Count;
            visual = new ModelVisual3D();
            _collection.Insert(count - 1, visual);

            if (count + 1 != _collection.Count ||
                !_collection[count - 1].Equals(visual) || failOnPurpose)
            {
                AddFailure("Insert failed");
                Log("*** Expected: count = {0}, 2nd to last object = {1}", count + 1, visual);
                Log("***   Actual: count = {0}, 2nd to last object = {1}", _collection.Count, _collection[count - 1]);
            }
        }

        private void TestInsert2()
        {
            Log("Testing IList.Insert with bad parameters...");
            PopulateList();

            Try(ListInsertNull, typeof(ArgumentException));
            Try(ListInsertNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(ListInsertLargeIndex, typeof(ArgumentOutOfRangeException));

            Log("Testing Visual3DCollection.Insert with bad parameters...");
            PopulateCollection();

            Try(CollectionInsertNull, typeof(ArgumentException));
            Try(CollectionInsertNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(CollectionInsertLargeIndex, typeof(ArgumentOutOfRangeException));
        }

        #region ExceptionThrowers for Insert

        private void ListInsertNull()
        {
            _list.Insert(0, null);
        }

        private void ListInsertNegativeIndex()
        {
            _list.Insert(-1, _extraObjects[0]);
        }

        private void ListInsertLargeIndex()
        {
            _list.Insert(_objects.Count + 1, _extraObjects[0]);
        }


        private void CollectionInsertNull()
        {
            _collection.Insert(0, null);
        }

        private void CollectionInsertNegativeIndex()
        {
            _collection.Insert(-1, new ModelVisual3D());
        }

        private void CollectionInsertLargeIndex()
        {
            _collection.Insert(_objects.Count + 1, new ModelVisual3D());
        }

        #endregion

        private void TestIsFixedSize()
        {
            Log("Testing IsFixedSize...");
            PopulateList();

            bool theirAnswer = _list.IsFixedSize;
            if (theirAnswer != false || failOnPurpose)
            {
                AddFailure("A growing collection should not have a fixed size");
            }
        }

        private void TestIsReadOnly()
        {
            Log("Testing ICollection<Visual3D>.IsReadOnly...");
            PopulateCollection();

            bool theirAnswer = ((ICollection<Visual3D>)_collection).IsReadOnly;
            if (theirAnswer != false || failOnPurpose)
            {
                AddFailure("A growing collection should not be read-only");
            }

            Log("Testing IList.IsReadOnly...");
            PopulateList();

            theirAnswer = _list.IsReadOnly;
            if (theirAnswer != false || failOnPurpose)
            {
                AddFailure("A growing collection should not be read-only");
            }
        }

        private void TestIsSynchronized()
        {
            Log("Testing IsSynchronized...");
            PopulateList();

            // Collections should always be synchronized because they are only accessible
            //   from the thread they were created on.
            if (!_list.IsSynchronized)
            {
                AddFailure("IList.IsSynchronized failed");
                Log("*** Expected: true");
                Log("***   Actual: {0}", _list.IsSynchronized);
            }
        }

        private void TestItem()
        {
            Log("Testing IList.get/set_Item...");
            PopulateList();

            _list[0] = _extraObjects[0];
            if (!_list[0].Equals(_extraObjects[0]) || failOnPurpose)
            {
                AddFailure("IList.get/set_Item failed");
                Log("*** Expected: item[0] = {0}", _extraObjects[0]);
                Log("***   Actual: item[0] = {0}", _list[0]);
            }

            Log("Testing Visual3DCollection.get/set_Item...");
            PopulateCollection();

            _collection[0] = _extraObjects[0];
            if (!_collection[0].Equals(_extraObjects[0]) || failOnPurpose)
            {
                AddFailure("Visual3DCollection.get/set_Item failed");
                Log("*** Expected: item[0] = {0}", _extraObjects[0]);
                Log("***   Actual: item[0] = {0}", _collection[0]);
            }
        }

        private void TestItem2()
        {
            Log("Testing IList.get/set_Item with bad parameters...");
            PopulateList();

            Try(ListGetItemNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(ListGetItemIndexTooLarge, typeof(ArgumentOutOfRangeException));
            Try(ListSetItemNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(ListSetItemIndexTooLarge, typeof(ArgumentOutOfRangeException));
            Try(ListSetItemNull, typeof(ArgumentException));
            Try(ListSetItemWrongType, typeof(ArgumentException));

            Log("Testing Visual3DCollection.get/set_Item with bad parameters...");
            PopulateCollection();

            Try(CollectionGetItemNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(CollectionGetItemIndexTooLarge, typeof(ArgumentOutOfRangeException));
            Try(CollectionSetItemNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(CollectionSetItemIndexTooLarge, typeof(ArgumentOutOfRangeException));
            Try(CollectionSetItemNull, typeof(ArgumentException));
            Try(CollectionSetItemDuplicate, typeof(ArgumentException));
        }

        #region ExceptionThrowers for get/set_Item

        private void ListGetItemNegativeIndex()
        {
            object item = _list[-1];
        }

        private void ListGetItemIndexTooLarge()
        {
            object item = _list[_objects.Count];
        }

        private void ListSetItemNegativeIndex()
        {
            _list[-1] = _objects[0];
        }

        private void ListSetItemIndexTooLarge()
        {
            _list[_objects.Count] = _objects[0];
        }

        private void ListSetItemNull()
        {
            _list[0] = null;
        }

        private void ListSetItemWrongType()
        {
            _list[0] = true;
        }

        private void CollectionGetItemNegativeIndex()
        {
            Visual3D item = _collection[-1];
        }

        private void CollectionGetItemIndexTooLarge()
        {
            Visual3D item = _collection[_objects.Count];
        }

        private void CollectionSetItemNegativeIndex()
        {
            _collection[-1] = (Visual3D)_objects[0];
        }

        private void CollectionSetItemIndexTooLarge()
        {
            _collection[_objects.Count] = (Visual3D)_objects[0];
        }

        private void CollectionSetItemNull()
        {
            _collection[0] = (Visual3D)null;
        }

        private void CollectionSetItemDuplicate()
        {
            _collection[0] = _collection[1];
        }

        #endregion

        private void TestRemove()
        {
            Log("Testing IList.Remove...");
            PopulateList();

            int count = _list.Count;
            _list.Remove(_objects[0]);
            if (count - 1 != _list.Count || _list.IndexOf(_objects[0]) != -1 || failOnPurpose)
            {
                AddFailure("IList.Remove Failed");
                Log("*** Expected: count = {0} and object should not be found", count - 1);
                if (count - 1 == _list.Count)
                {
                    Log("***   Actual: count = {0} and object was not removed", _list.Count);
                }
                else
                {
                    Log("***   Actual: count = {0}", _list.Count);
                }
            }

            Log("Testing Visual3DCollection.Remove...");
            PopulateCollection();

            _collection.Remove(_objects[3]);

            if (_collection.Count != _objects.Count - 1 || _collection.IndexOf(_objects[3]) != -1 ||
                failOnPurpose)
            {
                AddFailure("Remove failed");
                Log("*** Expected: Count = {0}", _objects.Count - 1);
                Log("***   Actual: Count = {0}", _collection.Count);
            }
        }

        private void TestRemove2()
        {
            Log("Testing IList.Remove with bad parameters...");

            SafeExecute(ListRemoveNull);
            SafeExecute(ListRemoveWrongType);
            SafeExecute(ListRemoveNonExisting);
            SafeExecute(RemoveFromEmptyList);

            Log("Testing Visual3DCollection.Remove with bad parameters...");

            SafeExecute(CollectionRemoveNull);
            SafeExecute(CollectionRemoveNonExisting);
            SafeExecute(RemoveFromEmptyCollection);
        }

        #region SafeExecutionBlocks for Remove

        private void ListRemoveNull()
        {
            PopulateList();
            _list.Remove(null);
        }

        private void ListRemoveWrongType()
        {
            PopulateList();
            _list.Remove(true);
        }

        private void ListRemoveNonExisting()
        {
            PopulateList();
            _list.Remove(_objects[0]);
            _list.Remove(_objects[0]);
        }

        private void RemoveFromEmptyList()
        {
            _list = NewVisual3DCollection();
            _list.Remove(_objects[0]);
            if (_list.Count != 0)
            {
                AddFailure("IList.Remove failed with empty list");
                Log("*** A list with {0} elements is not empty", _list.Count);
            }
        }

        private void CollectionRemoveNull()
        {
            PopulateCollection();
            _collection.Remove(null);
        }

        private void CollectionRemoveNonExisting()
        {
            PopulateCollection();
            _collection.Remove((Visual3D)_objects[0]);
            _collection.Remove((Visual3D)_objects[0]);
        }

        private void RemoveFromEmptyCollection()
        {
            _collection = NewVisual3DCollection();
            _collection.Remove((Visual3D)_objects[0]);
            if (_collection.Count != 0)
            {
                AddFailure("Visual3DCollection.Remove failed with empty collection");
                Log("*** A collection with {0} elements is not empty", _collection.Count);
            }
        }

        #endregion

        private void TestRemoveAt()
        {
            Log("Testing RemoveAt...");
            PopulateList();

            object[] myList = new object[_list.Count];

            _list.CopyTo(myList, 0);
            _list.RemoveAt(0);
            myList[0] = null;

            VerifyRemoveAt(myList);

            PopulateList();
            _list.CopyTo(myList, 0);
            _list.RemoveAt(_list.Count - 1);
            myList[myList.Length - 1] = null;

            VerifyRemoveAt(myList);
        }

        private void VerifyRemoveAt(object[] myList)
        {
            int myIterator = 0;
            int theirIterator = 0;
            while (theirIterator < _list.Count)
            {
                if (myList[myIterator] == null)
                {
                    myIterator++;
                }
                if (!_list[theirIterator].Equals(myList[myIterator]) || failOnPurpose)
                {
                    AddFailure("RemoveAt Failed");
                    Log("*** Expected: object #{0} = {1}", theirIterator, myList[myIterator]);
                    Log("***   Actual: object #{0} = {1}", theirIterator, _list[theirIterator]);
                    break;
                }
                myIterator++;
                theirIterator++;
            }
        }

        private void TestRemoveAt2()
        {
            Log("Testing Visual3DCollection.RemoveAt with bad parameters...");

            Try(RemoveAtNegativeIndex, typeof(ArgumentOutOfRangeException));
            Try(RemoveAtLargeIndex, typeof(ArgumentOutOfRangeException));
            Try(RemoveAtZeroIndexEmpty, typeof(ArgumentOutOfRangeException));
        }

        #region ExceptionThrowers for RemoveAt

        private void RemoveAtNegativeIndex()
        {
            PopulateCollection();
            _collection.RemoveAt(-1);
        }

        private void RemoveAtLargeIndex()
        {
            PopulateCollection();
            _collection.RemoveAt(_objects.Count);
        }

        private void RemoveAtZeroIndexEmpty()
        {
            _collection = NewVisual3DCollection();
            _collection.RemoveAt(0);
        }

        #endregion

        private void TestSyncRoot()
        {
            Log("Covering SyncRoot...");
            PopulateList();

            // SyncRoot returns the owner of the Visual3DCollection and I can't access it (without reflection)
            // This isn't a high priority, so just use it and forget about it.
            Log("IList.SyncRoot returns " + _list.SyncRoot);
        }

        private Visual3DCollection _collection;
        private IList _list;
        private List<Visual3D> _objects;
        private List<Visual3D> _extraObjects;
        private ArrayList _bogusCollection;
    }
}
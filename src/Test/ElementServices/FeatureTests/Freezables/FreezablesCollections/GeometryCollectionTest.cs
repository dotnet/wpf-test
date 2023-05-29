// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   FreezableCollection Test
 
 *
 ************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{

    internal class       GeometryCollectionTest   :   FreezableCollectionTest<Geometry>
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        internal GeometryCollectionTest() 
        {
            _bogusCollection = new List<object>();
            collection.Add(new LineGeometry());
            collection.Add(new EllipseGeometry());
            _objects = new List<Geometry>();
            _objects.Add(new RectangleGeometry());
            _objects.Add(new PathGeometry());
            _objects.Add(Geometry.Empty);

            _bogusCollection.Add(10);
            _bogusCollection.Add(new TranslateTransform());
            _bogusCollection.Add(new Point3D());
            _bogusCollection.Add(null);
            _bogusCollection.Add(new Point());
            _bogusCollection.Add(new Vector3D());
            _bogusCollection.Add(new Matrix3D());
            _bogusCollection.Add(true);
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        internal override Microsoft.Test.ElementServices.Freezables.Utils.Result Perform()
        {
            TestDefaultConstructor();
            TestConstructorWith(464);
            FreezableCollection<Geometry> coll = PopulateCollection();
            TestConstructorWith(coll);
            TestConstructorWith(new NonCollectionEnumerable());
            TestConstructorWith(new FakeEnumerable());
            TestAdd(_objects);
            TestAddBadType(_bogusCollection);
            TestAddFromAnotherThread();
            TestAddToFrozenCollection();
            TestChangedEvent(_objects);
            TestClear();
            TestContains();
            TestCopyTo();
            TestCount();
            TestEnumerator();
            TestGetAsFrozen();
            TestGetCurrentValueAsFrozen();
            TestIndexOf();
            TestInsert();
            TestIsReadOnly();
            TestItem();
            TestRemove();
            TestRemoveAt();
     
            return result;
        }
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        internal override FreezableCollection<Geometry> PopulateCollection()
        {
            FreezableCollection<Geometry> coll = new FreezableCollection<Geometry>();
            coll.Add(new RectangleGeometry());
            coll.Add(new PathGeometry());
            coll.Add(Geometry.Empty);
            return coll;
        }
        //---------------------------------------------
        private List<object> _bogusCollection;
 //       private FreezableCollection<Geometry> geometries;
        private List<Geometry> _objects;
    }
    #region NonCollectionEnumerable

    public class NonCollectionEnumerable : IEnumerable<Geometry>
    {
        public IEnumerator<Geometry> GetEnumerator()
        {
            return new Enumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator();
        }

        public class Enumerator : IEnumerator<Geometry>
        {
            public Geometry Current
            {
                get { return null; }
            }
            public void Dispose()
            {
            }
            public bool MoveNext()
            {
                return false;
            }
            public void Reset()
            {
            }
            object IEnumerator.Current
            {
                get { return null; }
            }
        }
    }

    #endregion
    #region FakeEnumerable

    public class FakeEnumerable : IEnumerable<Geometry>, ICollection<Geometry>
    {
        public void Add(Geometry p)
        {
        }
        public void Clear()
        {
        }
        public bool Contains(Geometry p)
        {
            return false;
        }
        public void CopyTo(Geometry[] points, int index)
        {
        }
        public bool Remove(Geometry p)
        {
            return false;
        }
        public int Count
        {
            get { return 0; }
        }
        public bool IsReadOnly
        {
            get { return true; }
        }
        public IEnumerator<Geometry> GetEnumerator()
        {
            return new Enumerator();
        }
        IEnumerator IEnumerable.GetEnumerator()
        {
            return new Enumerator();
        }

        private class Enumerator : IEnumerator<Geometry>
        {
            public Geometry Current
            {
                get { return null; }
            }
            public void Dispose()
            {
            }
            public bool MoveNext()
            {
                return false;
            }
            public void Reset()
            {
            }
            object IEnumerator.Current
            {
                get { return null; }
            }
        }
    }

    #endregion


}

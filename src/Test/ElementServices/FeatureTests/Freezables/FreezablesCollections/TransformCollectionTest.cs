// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   Freezableinternal Test
 
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
    //--------------------------------------------------------------

    internal class       TransformCollectionTest   :   FreezableCollectionTest<Transform>
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        internal TransformCollectionTest() 
        {
            _bogusCollection = new List<object>();
            collection.Add(new TranslateTransform());
            collection.Add(new RotateTransform());
            _objects = new List<Transform>();
            _objects.Add(new TranslateTransform());
            _objects.Add(new RotateTransform());
       
            _bogusCollection.Add(10);
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
            FreezableCollection<Transform> coll = PopulateCollection();
            TestConstructorWith(coll);
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
        internal override FreezableCollection<Transform> PopulateCollection()
        {
            FreezableCollection<Transform> coll = new FreezableCollection<Transform>();
            coll.Add(new TranslateTransform());
            coll.Add(new RotateTransform());
            return coll;
        }
        //---------------------------------------------
        private List<object> _bogusCollection;
        private List<Transform> _objects;
    }
 }

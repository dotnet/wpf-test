// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
 *
 *   Copyright (c) Microsoft Corporation, 2006
 *
 *   Program:   BrusheCollection Test
 
 *
 ************************************************************/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Controls;
using System.Windows.Data;

using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.ElementServices.Freezables.Utils;


namespace Microsoft.Test.ElementServices.Freezables
{

    internal class       BrushCollectionTest   :   FreezableCollectionTest<Brush>
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -
        internal BrushCollectionTest() 
        {
            _bogusCollection = new List<object>();
            collection.Add(new SolidColorBrush());
            collection.Add(new LinearGradientBrush());
            _objects = new List<Brush>();
            _objects.Add(new SolidColorBrush());
            _objects.Add(new LinearGradientBrush());
       
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
            FreezableCollection<Brush> coll = PopulateCollection();
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
            TestDataBinding();
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
        internal override FreezableCollection<Brush> PopulateCollection()
        {
            FreezableCollection<Brush> coll = new FreezableCollection<Brush>();
            coll.Add(new SolidColorBrush());
            coll.Add(new LinearGradientBrush());
            return coll;
        }
        //-------------------------------------------
        internal override void TestDataBinding()
        {
            GlobalLog.LogStatus("Testing TestDataBinding for Brush...");
            // First do basic binding outside of a collection
            Binding binding = new Binding();
            binding.Path = new PropertyPath("Height");
            Button button = new Button();
            button.Height = 23;
            button.DataContext = button;

            SolidColorBrush scb = new SolidColorBrush();
            button.Tag = scb;
            BindingOperations.SetBinding(scb, Brush.OpacityProperty, binding);
            if (scb.Opacity != 23)
            {
                result.passed = false;
                result.failures.Add("TestBinding fail for Brush. Expected opacity = 23, Actual: " + scb.Opacity);
                GlobalLog.LogStatus("TestBinding fail for Brush. Expected opacity = 23, Actual: " + scb.Opacity);
            }
            Binding binding2 = new Binding();
            binding2.Path = new PropertyPath("Width");
            Button button2 = new Button();
            button2.Width = 345;
            button2.DataContext = button2;
            collection = PopulateCollection();
            button2.Tag = collection;
            foreach (DependencyObject d in collection)
            {
                BindingOperations.SetBinding(d, Brush.OpacityProperty, binding2);
                Brush brush = (Brush)d;
                if (brush.Opacity != 345)
                {
                    result.passed = false;
                    result.failures.Add("TestBinding fail for Brush. Expected opacity = 345, Actual: " + brush.Opacity);
                    GlobalLog.LogStatus("TestBinding fail for Brush. Expected opacity = 345, Actual: " + brush.Opacity);
                }
                // reset
                brush.Opacity = 1;
            }
        }

        //---------------------------------------------
        private List<object> _bogusCollection;
        private List<Brush> _objects;
    }
 }

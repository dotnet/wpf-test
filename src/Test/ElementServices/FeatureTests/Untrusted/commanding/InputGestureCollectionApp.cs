// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Threading;
using System.Windows.Threading;

using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted.Utilities;
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify general behavior of InputGestureCollection.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify general behavior of InputGestureCollection.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding\InputGestureCollection")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class InputGestureCollectionApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new InputGestureCollectionApp();
            Debug.Assert(app != null, "App does not exist!");
            CoreLogger.LogStatus("App object: " + app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        public override object DoSetup(object sender)
        {
            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="arg">Not used.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object arg)
        {
            //
            // Set up Sample command.
            //
            CoreLogger.LogStatus("Constructing Sample command...");
            RoutedCommand sampleCommand = new RoutedCommand("Sample", this.GetType(), null);

            //
            // InputGesture's - reused for multiple checks here.
            //
            CoreLogger.LogStatus("Constructing InputGesture's...");

            InputGesture inputGestureA = new KeyGesture(Key.F2, ModifierKeys.Control);
            InputGesture inputGestureB = new MouseGesture(MouseAction.RightDoubleClick, ModifierKeys.Control);

            //
            // Oracle used to verify InputGestureCollection.
            //
            List<InputGesture> oracle = new List<InputGesture>();

            //
            // InputGestureCollection(IList)
            //
            CoreLogger.LogStatus("Verifying new InputGestureCollection(IList)...");

            oracle.Add(inputGestureA);
            oracle.Add(inputGestureB);

            InputGestureCollection inputGestures = new InputGestureCollection(oracle);

            _VerifyList(oracle, inputGestures);

            oracle.Clear();

            //
            // InputGestureCollection()
            //
            CoreLogger.LogStatus("Verifying new InputGestureCollection()...");

            inputGestures = new InputGestureCollection();
            IList ilist = inputGestures;

            //
            // Add/Remove
            // 
            CoreLogger.LogStatus("Verifying Add/Remove...");

            // strongly-typed
            inputGestures.Add(inputGestureA);
            oracle.Add(inputGestureA);
            inputGestures.Add(inputGestureB);
            oracle.Add(inputGestureB);

            _VerifyList(oracle, inputGestures);

            inputGestures.Remove(inputGestureA);
            oracle.Remove(inputGestureA);
            inputGestures.Remove(inputGestureB);
            oracle.Remove(inputGestureB);

            _VerifyList(oracle, inputGestures);


            // IList
            ilist.Add(inputGestureA);
            oracle.Add(inputGestureA);
            ilist.Add(inputGestureB);
            oracle.Add(inputGestureB);

            _VerifyList(oracle, ilist);

            ilist.Remove(inputGestureA);
            oracle.Remove(inputGestureA);
            ilist.Remove(inputGestureB);
            oracle.Remove(inputGestureB);

            _VerifyList(oracle, ilist);

            //
            // Insert/RemoveAt 
            //
            CoreLogger.LogStatus("Verifying Insert/RemoveAt...");

            // strongly-typed
            inputGestures.Insert(0, inputGestureA);
            oracle.Insert(0, inputGestureA);
            _VerifyList(oracle, inputGestures);

            inputGestures.Insert(0, inputGestureB);
            oracle.Insert(0, inputGestureB);
            _VerifyList(oracle, inputGestures);

            inputGestures.RemoveAt(1);
            oracle.RemoveAt(1);
            _VerifyList(oracle, inputGestures);

            inputGestures.RemoveAt(0);
            oracle.RemoveAt(0);
            _VerifyList(oracle, inputGestures);

            // IList
            ilist.Insert(0, inputGestureA);
            oracle.Insert(0, inputGestureA);
            _VerifyList(oracle, ilist);

            ilist.Insert(0, inputGestureB);
            oracle.Insert(0, inputGestureB);
            _VerifyList(oracle, ilist);

            ilist.RemoveAt(1);
            oracle.RemoveAt(1);
            _VerifyList(oracle, ilist);

            ilist.RemoveAt(0);
            oracle.RemoveAt(0);
            _VerifyList(oracle, ilist);

            //
            // CopyTo
            //
            CoreLogger.LogStatus("Verifying CopyTo...");

            inputGestures.Add(inputGestureA);
            oracle.Add(inputGestureA);
            inputGestures.Add(inputGestureB);
            oracle.Add(inputGestureB);

            // strongly-typed
            InputGesture[] array = new InputGesture[3];
            InputGesture[] arrayOracle = new InputGesture[3];
            inputGestures.CopyTo(array, 1);
            oracle.CopyTo(arrayOracle, 1);

            if (array[1] != arrayOracle[1])
            {
                throw new Microsoft.Test.TestValidationException("CopyTo(...) did not copy the expected InputGesture.");
            }

            // ICollection
            ICollection coll = inputGestures;
            array = new InputGesture[3];
            coll.CopyTo(array, 1);

            if (array[1] != arrayOracle[1])
            {
                throw new Microsoft.Test.TestValidationException("CopyTo(...) did not copy the expected InputGesture.");
            }

            //
            // Clear
            //
            CoreLogger.LogStatus("Verifying Clear...");

            inputGestures.Clear();
            oracle.Clear();
            _VerifyList(oracle, inputGestures);

            // Call again to verify we can clear an empty collection.
            inputGestures.Clear();
            oracle.Clear();
            _VerifyList(oracle, inputGestures);

            //
            // set this[int] indexer
            //
            CoreLogger.LogStatus("Verifying this[int] setter...");

            // Initialize with B first and A second.
            inputGestures.Add(inputGestureB);
            oracle.Add(inputGestureB);
            inputGestures.Add(inputGestureA);
            oracle.Add(inputGestureA);

            // Switch A and B.
            inputGestures[0] = inputGestureA;
            oracle[0] = inputGestureA;
            inputGestures[1] = inputGestureB;
            oracle[1] = inputGestureB;

            _VerifyList(oracle, inputGestures);

            // Switch A and B again using IList.
            ilist[0] = inputGestureB;
            oracle[0] = inputGestureB;
            ilist[1] = inputGestureA;
            oracle[1] = inputGestureA;

            _VerifyList(oracle, inputGestures);

            inputGestures.Clear();

            //
            // AddRange(ICollection collection)
            //
            CoreLogger.LogStatus("Verifying AddRange()...");

            inputGestures.AddRange(oracle);

            _VerifyList(oracle, inputGestures);

            //
            // GetEnumerator()
            //
            CoreLogger.LogStatus("Verifying GetEnumerator()...");

            int i = 0;
            foreach (InputGesture value in inputGestures)
            {
                InputGesture expected_value = oracle[i++];

                if (!_IsEqual(expected_value, value))
                {
                    throw new Microsoft.Test.TestValidationException("GetEnumerator() item with index '" + i.ToString() + "' is not the expected value.");
                }
            }

            //
            // IsFizedSize, IsReadOnly, IsSynchronized, SyncRoot
            // Seal()
            //
            CoreLogger.LogStatus("Verifying IsFizedSize, IsReadOnly, IsSynchronized, and SyncRoot...");

            if (inputGestures.IsReadOnly || inputGestures.IsFixedSize || inputGestures.IsSynchronized)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection's IsReadOnly or IsFixedSize or IsSynchronized is true. Should be false until Seal() is called.");
            }

            object syncRoot = inputGestures.SyncRoot;

            if (syncRoot == null)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection.SyncRoot is null before Seal() is called.");
            }

            // Seal
            inputGestures.Seal();

            if (!inputGestures.IsReadOnly || !inputGestures.IsFixedSize)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection's IsReadOnly or IsFixedSize is false. Should be true after Seal() is called.");
            }
            if (inputGestures.IsSynchronized)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection's IsSynchronized is true. Should be false even after Seal() is called.");
            }

            syncRoot = inputGestures.SyncRoot;

            if (syncRoot == null)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection.SyncRoot is null after Seal() is called.");
            }

            CoreLogger.LogStatus("Verifying sealed collection...");
            _VerifySealed(inputGestures, "Add", inputGestureA);
            _VerifySealed(inputGestures, "AddRange", oracle);
            _VerifySealed(inputGestures, "Insert", 0, inputGestureA);
            _VerifySealed(inputGestures, "Remove", inputGestureA);
            _VerifySealed(inputGestures, "RemoveAt", 0);
            _VerifySealed(inputGestures, "Clear");

            base.DoExecute(arg);

            return null;
        }

        private void _VerifySealed(InputGestureCollection inputGestures, string methodName, params object[] parameters)
        {
            InternalObject internalObj = new InternalObject(inputGestures);
            Exception caughtException = null;

            try
            {
                internalObj.InvokeMethod(methodName, parameters);
            }
            catch (Exception ex)
            {
                caughtException = ex.InnerException;
            }

            if (caughtException == null)
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException exception was not thrown when " + methodName + "() was called on a sealed InputGestureCollection.");
            }

            if (!(caughtException is NotSupportedException))
            {
                throw new Microsoft.Test.TestValidationException("NotSupportedException exception was not thrown when " + methodName + "() was called on a sealed InputGestureCollection. \r\nException:" + caughtException.ToString());
            }
        }

        // Checks the entire contents of the actual list against the oracle list.
        private void _VerifyList(List<InputGesture> oracle, IList ilist)
        {
            InputGestureCollection inputGestureCollection = (InputGestureCollection)ilist;
            int count = inputGestureCollection.Count;
            int ilist_count = ilist.Count;

            // Count property
            // Check that the collection's count is the same as the oracle's count.
            if (count != oracle.Count || ilist_count != oracle.Count)
            {
                throw new Microsoft.Test.TestValidationException("InputGestureCollection.Count is not the expected value. Expected: " + oracle.Count + ", Actual: " + count + ", Actual(IList): " + ilist_count + ".");
            }

            // get this[int] indexer
            // IndexOf method
            // Contains
            // Check that each slot in the collection has the same value as the oracle's slots.
            for (int i = 0; i < count; i++)
            {
                InputGesture value = inputGestureCollection[i];
                InputGesture ilist_value = (InputGesture)ilist[i];
                InputGesture expected_value = oracle[i];

                if (!_IsEqual(expected_value, value) || !_IsEqual(expected_value, ilist_value))
                {
                    throw new Microsoft.Test.TestValidationException("Collection item with index '" + i.ToString() + "' is not the expected value.");
                }

                int indexof = inputGestureCollection.IndexOf(value);
                int ilist_indexof = ilist.IndexOf(ilist_value);
                int expected_indexof = oracle.IndexOf(expected_value);

                if (indexof != expected_indexof || ilist_indexof != expected_indexof)
                {
                    throw new Microsoft.Test.TestValidationException("IndexOf(inputGesture) returned an unexpected value. Expected: " + expected_indexof + ", Actual: " + indexof + ", Actual(IList): " + ilist_indexof + ".");
                }

                if (!inputGestureCollection.Contains(value) || !ilist.Contains(ilist_value))
                {
                    throw new Microsoft.Test.TestValidationException("Contains(inputGesture) returned false for a gesture the collection should contain.");
                }
            }
        }

        // Checks if two values are the same.
        private bool _IsEqual(InputGesture expectedValue, InputGesture actualValue)
        {
            bool same = false;

            if (actualValue == null)
            {
                same = expectedValue == null;
            }
            else
            {
                same = actualValue.Equals(expectedValue);
            }

            return same;
        }

        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender)
        {
            this.TestPassed = true;

            CoreLogger.LogStatus("Validation complete!");
            return null;
        }

    }
}

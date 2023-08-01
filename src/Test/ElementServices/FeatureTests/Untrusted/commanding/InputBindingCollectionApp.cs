// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
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
using Microsoft.Test.Win32;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify general behavior of InputBindingCollection.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify general behavior of InputBindingCollection.")]
    [TestCasePriority("2")]
    [TestCaseArea(@"Commanding\CoreCommanding\InputBindingCollection")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    public class InputBindingCollectionApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest()
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new InputBindingCollectionApp();
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
            // InputBinding's - reused for multiple checks here.
            //
            CoreLogger.LogStatus("Constructing InputBinding's...");
            InputBinding inputBindingA = new KeyBinding(sampleCommand, new KeyGesture(Key.A, ModifierKeys.Control));
            InputBinding inputBindingB = new MouseBinding(sampleCommand, new MouseGesture(MouseAction.LeftClick, ModifierKeys.Control));

            //
            // Oracle used to verify InputBindingCollection.
            //
            List<InputBinding> oracle = new List<InputBinding>();

            //
            // InputBindingCollection(IList)
            //
            CoreLogger.LogStatus("Verifying new InputBindingCollection(IList)...");

            oracle.Add(inputBindingA);
            oracle.Add(inputBindingB);

            InputBindingCollection inputBindings = new InputBindingCollection(oracle);

            _VerifyList(oracle, inputBindings);

            oracle.Clear();

            //
            // InputBindingCollection()
            //
            CoreLogger.LogStatus("Verifying new InputBindingCollection()...");

            inputBindings = new InputBindingCollection();
            IList ilist = inputBindings;

            //
            // Add/Remove
            // 
            CoreLogger.LogStatus("Verifying Add/Remove...");

            // strongly-typed
            inputBindings.Add(inputBindingA);
            oracle.Add(inputBindingA);
            inputBindings.Add(inputBindingB);
            oracle.Add(inputBindingB);

            _VerifyList(oracle, inputBindings);

            inputBindings.Remove(inputBindingA);
            oracle.Remove(inputBindingA);
            inputBindings.Remove(inputBindingB);
            oracle.Remove(inputBindingB);

            _VerifyList(oracle, inputBindings);


            // IList
            ilist.Add(inputBindingA);
            oracle.Add(inputBindingA);
            ilist.Add(inputBindingB);
            oracle.Add(inputBindingB);

            _VerifyList(oracle, ilist);

            ilist.Remove(inputBindingA);
            oracle.Remove(inputBindingA);
            ilist.Remove(inputBindingB);
            oracle.Remove(inputBindingB);

            _VerifyList(oracle, ilist);

            //
            // Insert/RemoveAt 
            //
            CoreLogger.LogStatus("Verifying Insert/RemoveAt...");

            // strongly-typed
            inputBindings.Insert(0, inputBindingA);
            oracle.Insert(0, inputBindingA);
            _VerifyList(oracle, inputBindings);

            inputBindings.Insert(0, inputBindingB);
            oracle.Insert(0, inputBindingB);
            _VerifyList(oracle, inputBindings);

            inputBindings.RemoveAt(1);
            oracle.RemoveAt(1);
            _VerifyList(oracle, inputBindings);

            inputBindings.RemoveAt(0);
            oracle.RemoveAt(0);
            _VerifyList(oracle, inputBindings);

            // IList
            ilist.Insert(0, inputBindingA);
            oracle.Insert(0, inputBindingA);
            _VerifyList(oracle, ilist);

            ilist.Insert(0, inputBindingB);
            oracle.Insert(0, inputBindingB);
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

            inputBindings.Add(inputBindingA);
            oracle.Add(inputBindingA);
            inputBindings.Add(inputBindingB);
            oracle.Add(inputBindingB);

            // strongly-typed
            InputBinding[] array = new InputBinding[3];
            InputBinding[] arrayOracle = new InputBinding[3];
            inputBindings.CopyTo(array, 1);
            oracle.CopyTo(arrayOracle, 1);

            if (array[1] != arrayOracle[1])
            {
                throw new Microsoft.Test.TestValidationException("CopyTo(...) did not copy the expected InputBinding.");
            }

            // ICollection
            ICollection coll = inputBindings;
            array = new InputBinding[3];
            coll.CopyTo(array, 1);

            if (array[1] != arrayOracle[1])
            {
                throw new Microsoft.Test.TestValidationException("CopyTo(...) did not copy the expected InputBinding.");
            }

            //
            // Clear
            //
            CoreLogger.LogStatus("Verifying Clear...");

            inputBindings.Clear();
            oracle.Clear();
            _VerifyList(oracle, inputBindings);

            // Call again to verify we can clear an empty collection.
            inputBindings.Clear();
            oracle.Clear();
            _VerifyList(oracle, inputBindings);

            //
            // set this[int] indexer
            //
            CoreLogger.LogStatus("Verifying this[int] setter...");

            // Initialize with B first and A second.
            inputBindings.Add(inputBindingB);
            oracle.Add(inputBindingB);
            inputBindings.Add(inputBindingA);
            oracle.Add(inputBindingA);

            // Switch A and B.
            inputBindings[0] = inputBindingA;
            oracle[0] = inputBindingA;
            inputBindings[1] = inputBindingB;
            oracle[1] = inputBindingB;

            _VerifyList(oracle, inputBindings);

            // Switch A and B again using IList.
            ilist[0] = inputBindingB;
            oracle[0] = inputBindingB;
            ilist[1] = inputBindingA;
            oracle[1] = inputBindingA;

            _VerifyList(oracle, inputBindings);

            inputBindings.Clear();

            //
            // AddRange(ICollection collection)
            //
            CoreLogger.LogStatus("Verifying AddRange()...");

            inputBindings.AddRange(oracle);

            _VerifyList(oracle, inputBindings);

            //
            // GetEnumerator()
            //
            CoreLogger.LogStatus("Verifying GetEnumerator()...");

            int i = 0;
            foreach (InputBinding value in inputBindings)
            {
                InputBinding expected_value = oracle[i++];

                if (!_IsEqual(expected_value, value))
                {
                    throw new Microsoft.Test.TestValidationException("GetEnumerator() item with index '" + i.ToString() + "' is not the expected value.");
                }
            }

            //
            // IsFizedSize, IsReadOnly, IsSynchronized, SyncRoot
            //
            CoreLogger.LogStatus("Verifying IsFizedSize, IsReadOnly, IsSynchronized, and SyncRoot...");

            if (inputBindings.IsReadOnly || inputBindings.IsFixedSize || inputBindings.IsSynchronized)
            {
                throw new Microsoft.Test.TestValidationException("InputBindingCollection's IsReadOnly or IsFixedSize or IsSynchronized is true. Should always be false.");
            }

            object syncRoot = inputBindings.SyncRoot;

            if (syncRoot != inputBindings)
            {
                throw new Microsoft.Test.TestValidationException("InputBindingCollection.SyncRoot is not the InputBindingCollection itself.");
            }

            base.DoExecute(arg);

            return null;
        }

        // Checks the entire contents of the actual list against the oracle list.
        private void _VerifyList(List<InputBinding> oracle, IList ilist)
        {
            InputBindingCollection inputBindingCollection = (InputBindingCollection)ilist;
            int count = inputBindingCollection.Count;
            int ilist_count = ilist.Count;

            // Count property
            // Check that the collection's count is the same as the oracle's count.
            if (count != oracle.Count || ilist_count != oracle.Count)
            {
                throw new Microsoft.Test.TestValidationException("InputBindingCollection.Count is not the expected value. Expected: " + oracle.Count + ", Actual: " + count + ", Actual(IList): " + ilist_count + ".");
            }

            // get this[int] indexer
            // IndexOf method
            // Contains
            // Check that each slot in the collection has the same value as the oracle's slots.
            for (int i = 0; i < count; i++)
            {
                InputBinding value = inputBindingCollection[i];
                InputBinding ilist_value = (InputBinding)ilist[i];
                InputBinding expected_value = oracle[i];

                if (!_IsEqual(expected_value, value) || !_IsEqual(expected_value, ilist_value))
                {
                    throw new Microsoft.Test.TestValidationException("Collection item with index '" + i.ToString() + "' is not the expected value.");
                }

                int indexof = inputBindingCollection.IndexOf(value);
                int ilist_indexof = ilist.IndexOf(ilist_value);
                int expected_indexof = oracle.IndexOf(expected_value);

                if (indexof != expected_indexof || ilist_indexof != expected_indexof)
                {
                    throw new Microsoft.Test.TestValidationException("IndexOf(inputBinding) returned an unexpected value. Expected: " + expected_indexof + ", Actual: " + indexof + ", Actual(IList): " + ilist_indexof + ".");
                }

                if (!inputBindingCollection.Contains(value) || !ilist.Contains(ilist_value))
                {
                    throw new Microsoft.Test.TestValidationException("Contains(inputBinding) returned false for a binding the collection should contain.");
                }
            }
        }

        // Checks if two values are the same.
        private bool _IsEqual(InputBinding expectedValue, InputBinding actualValue)
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

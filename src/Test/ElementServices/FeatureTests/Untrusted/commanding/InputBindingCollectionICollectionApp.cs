// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.CoreInput.Common.Controls;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Win32;


namespace Avalon.Test.CoreUI.Commanding
{
    /******************************************************************************
    * CLASS:          InputBindingCollectionICollectionApp
    ******************************************************************************/
    /// <summary>
    /// Verify InputBindingCollection ICollection APIs work as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of unit tests for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    /// <
    [Test(0, "Commanding.Collections", TestCaseSecurityLevel.FullTrust, "InputBindingCollectionICollectionApp")]
    public class InputBindingCollectionICollectionApp : TestApp
    {
        #region Private Data
        private static RoutedCommand s_sampleCommand = null;
        private object _oldSyncRoot;
        private object _newSyncRoot;
        private ICollection _itemList;
        private IEnumerable _itemEnumerableList;
        private Object _arbitraryObject = new Object();
        private InputBinding _firstItem;
        private object[] _objectArray;
        private InputBinding[] _itemArray;
        private bool _isArgNullExceptionThrown = false;
        private bool _isArgExceptionThrown = false;
        private bool _isArgExceptionWrongTypeThrown = false;
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          InputBindingCollectionICollectionApp Constructor
        ******************************************************************************/
        public InputBindingCollectionICollectionApp() :base()
        {
            GlobalLog.LogStatus("In InputBindingCollectionICollectionApp constructor");
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            GlobalLog.LogStatus("Running app...");
            this.RunTestApp();
            GlobalLog.LogStatus("App run!");

            //Any test failure will be caught by an Assert during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public and Protected Members
        /******************************************************************************
        * Function:          DoSetup
        ******************************************************************************/
        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            GlobalLog.LogStatus("Constructing window....");
            
            InputBindingCollection igc = new InputBindingCollection();
            _itemList = igc as ICollection;
            _itemEnumerableList = igc as IEnumerable;

            return null;
        }

        /******************************************************************************
        * Function:          DoExecute
        ******************************************************************************/
        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender) {
            _oldSyncRoot = _itemList.SyncRoot;

            GlobalLog.LogStatus("Adding Binding to collection...");
            _firstItem = new KeyBinding(SampleCommand, new KeyGesture(Key.Apps));
            ((InputBindingCollection)(_itemList)).Add(_firstItem);

            _newSyncRoot = _itemList.SyncRoot;


            GlobalLog.LogStatus("Synchronized? (shouldn't be)? " + (_itemList.IsSynchronized));
            this.Assert(!_itemList.IsSynchronized, "Whoops, we are unexpectedly synchronized!");

            GlobalLog.LogStatus("SyncRoot pre-Binding-add? (should be)? " + (_oldSyncRoot != null));
            this.Assert(_oldSyncRoot != null, "Whoops, we have unexpected null syncroot pre-Binding-add!");

            GlobalLog.LogStatus("SyncRoot post-Binding-add? (should be)? " + (_newSyncRoot != null));
            this.Assert(_newSyncRoot != null, "Whoops, we have unexpected null syncroot post-Binding-add!");

            GlobalLog.LogStatus("Checking to see whether Items are copied (null)...");
            _itemArray = null;
            try
            {
                _itemList.CopyTo(_itemArray, 0);
            }
            catch (ArgumentNullException)
            {
                _isArgNullExceptionThrown = true;
            }

            GlobalLog.LogStatus("Checking to see whether Items are copied (array too small)...");
            _itemArray = new InputBinding[0];
            try
            {
                _itemList.CopyTo(_itemArray, 0);
            }
            catch (ArgumentException)
            {
                _isArgExceptionThrown = true;
            }

            GlobalLog.LogStatus("Checking to see whether Items are copied (wrong type)...");
            _objectArray = new String[1];
            try
            {
                _itemList.CopyTo(_objectArray, 0);
            }
            // 
            catch (Exception)
            {
                _isArgExceptionWrongTypeThrown = true;
            }

            GlobalLog.LogStatus("Checking to see whether Items are copied (parent type and correct length)...");
            _objectArray = new object[1];
            _itemList.CopyTo(_objectArray, 0);

            GlobalLog.LogStatus("Checking to see whether Items are copied (correct type and length)...");
            _itemArray = new InputBinding[1];
            _itemList.CopyTo(_itemArray, 0);

            GlobalLog.LogStatus("Exception thrown copying to null? (should be) " + (_isArgNullExceptionThrown));
            this.Assert(_isArgNullExceptionThrown, "Whoops, expected exception not thrown!");

            GlobalLog.LogStatus("Exception thrown copying to smaller array? (should be) " + (_isArgExceptionThrown));
            this.Assert(_isArgExceptionThrown, "Whoops, expected exception not thrown!");

            GlobalLog.LogStatus("Exception thrown copying to wrong type array? (should be) " + (_isArgExceptionWrongTypeThrown));
            this.Assert(_isArgExceptionWrongTypeThrown, "Whoops, expected exception not thrown!");

            GlobalLog.LogStatus("Object array still null? (shouldn't be) " + (_objectArray != null));
            this.Assert(_objectArray != null, "Whoops, array not copied!");

            GlobalLog.LogStatus("Object array count: (should be 1) " + (_objectArray.Length));
            this.Assert(_objectArray.Length == 1, "Whoops, not all array element copied!");

            GlobalLog.LogStatus("Item array still null? (shouldn't be) " + (_itemArray != null));
            this.Assert(_itemArray != null, "Whoops, array not copied!");

            GlobalLog.LogStatus("Item array count: (should be 1) " + (_itemArray.Length));
            this.Assert(_itemArray.Length == 1, "Whoops, not all array element copied!");

            base.DoExecute(sender);
            return null;
        }

        /******************************************************************************
        * Function:          DoValidate
        ******************************************************************************/
        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.BeginVariation("InputBindingCollectionICollectionApp");
            GlobalLog.LogStatus("Validating...");

            IEnumerator enumList = _itemEnumerableList.GetEnumerator();
            GlobalLog.LogStatus("Enumerator exists? (should be) " + (enumList != null));
            this.Assert((enumList != null), "Whoops, enumerator can't be found!");

            this.TestPassed = true;
            GlobalLog.LogStatus("Setting log result to " + this.TestPassed);
            
            GlobalLog.LogStatus("Validation complete!");
            CoreLogger.LogTestResult(this.TestPassed, "Passed if event found.  Failed if not.");
            CoreLogger.EndVariation();

            return null;
        }

        /// <summary>
        /// Sample command belonging to this class.
        /// </summary>
        public static RoutedCommand SampleCommand
        {
            get
            {
                if (s_sampleCommand == null)
                {
                    s_sampleCommand = new RoutedCommand("Sample", typeof(InputBindingCollectionICollectionApp), null);
                }
                return s_sampleCommand;
            }
        }
        #endregion
    }
}

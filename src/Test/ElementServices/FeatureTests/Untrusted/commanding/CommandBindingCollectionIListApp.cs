// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;

using System.Collections;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Discovery;
using Microsoft.Test;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Commanding
{
    /// <summary>
    /// Verify CommandBindingCollection IList APIs work as expected.
    /// </summary>
    /// <description>
    /// This is part of a collection of scenarios for commanding.
    /// </description>
    /// <author>Microsoft</author>
 
    [CoreTestsLoader(CoreTestsTestType.ClassBase)]
    [TestCaseTitle(@"Verify CommandBindingCollection IList APIs work as expected.")]
    [TestCasePriority("1")]
    [TestCaseArea(@"Commanding\Collections")]
    [TestCaseMethod("LaunchTest")]
    [TestCaseDisabled("0")]
    [TestCaseSecurityLevel(TestCaseSecurityLevel.FullTrust)]
    public class CommandBindingCollectionIListApp : TestApp
    {
        /// <summary>
        /// Launch our test.
        /// </summary>
        public static void LaunchTest() 
        {
            CoreLogger.LogStatus("Creating app object...");
            TestApp app = new CommandBindingCollectionIListApp();
            Debug.Assert( app!=null, "App does not exist!");
            CoreLogger.LogStatus("App object: "+app.ToString());

            CoreLogger.LogStatus("Running app...");
            app.RunTestApp();
            CoreLogger.LogStatus("App run!");
        }

        /// <summary>
        /// Setup the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoSetup(object sender) 
        {
            CoreLogger.LogStatus("Constructing binding collection....");
            
            CommandBindingCollection ibc = new CommandBindingCollection();

            _firstItem = new CommandBinding(SampleCommand);
            _secondItem = new CommandBinding(SampleCommand);
            ibc.Add(_firstItem);

            _itemList = ibc as IList;

            return null;
        }

        /// <summary>
        /// Execute stuff.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        protected override object DoExecute(object sender) {

            CoreLogger.LogStatus("Checking list properties...");
            CoreLogger.LogStatus("Item list readonly,fixedsize? " + _itemList.IsReadOnly + "," + _itemList.IsFixedSize);
            this.Assert(!_itemList.IsReadOnly, "Whoops, Item list is ReadOnly! (shouldn't be)");
            this.Assert(!_itemList.IsFixedSize, "Whoops, Item list is FixedSize! (shouldn't be)");
            this.Assert(_itemList[0] is CommandBinding, "Whoops, not a command Item! (should be)");

            CoreLogger.LogStatus("Checking to see whether Items are contained in list...");
            _containedItemGood = _itemList.Contains(_firstItem);
            _containedItemBad = _itemList.Contains(_secondItem);
            CoreLogger.LogStatus("Item list containment: first,second? " + _containedItemGood + "," + _containedItemBad);
            this.Assert(_containedItemGood && !_containedItemBad, "Whoops, first Item should be contained, second shouldn't be!");

            CoreLogger.LogStatus("Checking to see whether Item can be indexed in list...");
            _idxItemGood = _itemList.IndexOf(_firstItem);
            _idxItemBad = _itemList.IndexOf(_secondItem);
            CoreLogger.LogStatus("Item list indexing: first,second? " + _idxItemGood + "," + _idxItemBad);
            this.Assert((_idxItemGood == 0) && (_idxItemBad == -1), "Whoops, first Item should be 0, second should be -1!");

            CoreLogger.LogStatus("Replacing first list item with plain object...");
            try
            {
                _itemList[0] = _arbitraryObject;
            }
            catch (NotSupportedException)
            {
                _isExceptionThrown = true;
            }
            CoreLogger.LogStatus("Was exception thrown? " + _isExceptionThrown);
            this.Assert(_isExceptionThrown, "Whoops, we expected an exception!");

            CoreLogger.LogStatus("Clearing out list...");
            _itemList.Clear();
            ICollection ibcPostClear = _itemList as ICollection;
            CoreLogger.LogStatus("Binding list count: " + ibcPostClear.Count);
            this.Assert(ibcPostClear.Count == 0, "Whoops, count not correct! (should be 0)");

            CoreLogger.LogStatus("Inserting Item into list between two existing Items...");
            _itemList.Clear();
            _itemList.Add(_firstItem);
            _itemList.Add(_firstItem);
            _itemList.Add(_secondItem);
            _itemList.Insert(1, _secondItem);
            ICollection ibcPostInsert = _itemList as ICollection;
            CoreLogger.LogStatus("Binding list count: " + ibcPostInsert.Count);
            this.Assert(ibcPostInsert.Count == 4, "Whoops, count not correct! (should be 4)");

            CoreLogger.LogStatus("Verifying proper contents of list post-insertion...");
            this.Assert(((CommandBinding)_itemList[0] == _firstItem), "Item at Index 0 not correct!");
            this.Assert(((CommandBinding)_itemList[1] == _secondItem), "Item at Index 1 not correct!");
            this.Assert(((CommandBinding)_itemList[2] == _firstItem), "Item at Index 2 not correct!");
            this.Assert(((CommandBinding)_itemList[3] == _secondItem), "Item at Index 3 not correct!");

            CoreLogger.LogStatus("Inserting invalid Item into list between two existing Items...");
            try
            {
                _itemList.Insert(1, _arbitraryObject);
            }
            catch (NotSupportedException)
            {
                _isInvalidInsertExceptionThrown = true;
            }
            CoreLogger.LogStatus("Exception thrown (should be)? " + _isInvalidInsertExceptionThrown);
            this.Assert(_isInvalidInsertExceptionThrown, "Whoops, expected insert exception not thrown!");

            CoreLogger.LogStatus("Removing Item from list...");
            _itemList.Remove(_secondItem);
            
            ICollection ibcPostRemove = _itemList as ICollection;
            CoreLogger.LogStatus("Binding list count: " + ibcPostRemove.Count);
            this.Assert(ibcPostRemove.Count == 3, "Whoops, count not correct! (should be 3)");

            CoreLogger.LogStatus("Verifying proper contents of list post-removal...");
            this.Assert(((CommandBinding)_itemList[0] == _firstItem), "Item at Index 0 not correct!");
            this.Assert(((CommandBinding)_itemList[1] == _firstItem), "Item at Index 1 not correct!");
            this.Assert(((CommandBinding)_itemList[2] == _secondItem), "Item at Index 2 not correct!");

            CoreLogger.LogStatus("Removing Item from list...");
            _itemList.RemoveAt(1);

            base.DoExecute(sender);
            return null;
        }


        /// <summary>
        /// Validate the test.
        /// </summary>
        /// <param name="sender">App sending the callback.</param>
        /// <returns>Null object.</returns>
        public override object DoValidate(object sender) 
        {
            CoreLogger.LogStatus("Validating...");

            ICollection ibcPostRemoveAt = _itemList as ICollection;
            this.Assert(ibcPostRemoveAt != null, "Whoops, item list wasn't a collection (should be)");

            CoreLogger.LogStatus("Binding list count: " + ibcPostRemoveAt.Count);
            this.Assert(ibcPostRemoveAt.Count == 2, "Whoops, count not correct! (should be 2)");

            CoreLogger.LogStatus("Verifying proper contents of list post-removal-at...");
            this.Assert(((CommandBinding)_itemList[0] == _firstItem), "Item at Index 0 not correct!");
            this.Assert(((CommandBinding)_itemList[1] == _secondItem), "Item at Index 1 not correct!");

            this.TestPassed = true;
            CoreLogger.LogStatus("Setting log result to " + this.TestPassed);
            
            CoreLogger.LogStatus("Validation complete!");
            
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
                    s_sampleCommand = new RoutedCommand("Sample", typeof(CommandBindingCollectionIListApp), null);
                }
                return s_sampleCommand;
            }
        }
        private static RoutedCommand s_sampleCommand = null;

        private IList _itemList;

        private Object _arbitraryObject = new Object();

        private CommandBinding _firstItem;
        private CommandBinding _secondItem;

        private bool _containedItemGood;
        private bool _containedItemBad;

        private int _idxItemGood;
        private int _idxItemBad;

        private bool _isExceptionThrown = false;
        private bool _isInvalidInsertExceptionThrown = false;
    }
  
}

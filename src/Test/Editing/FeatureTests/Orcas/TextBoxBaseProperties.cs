// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for TextBoxBase properties introduced in Orcas.

namespace Test.Uis.TextEditing
{
    #region Using directives

    using System;
    using System.Collections.Generic;
    using System.Text;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion

    // DISABLEDUNSTABLETEST:
    // TestName: UndoLimitTest
    // Area: Editing SubArea: UndoRedo
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    /// <summary>
    /// Tests UndoLimit property in TextBox and RichTextBox
    /// </summary>
    [Test(2, "UndoRedo", "UndoLimitTest", MethodParameters = "/TestCaseType=UndoLimitTest", Keywords = "Setup_SanitySuite", Disabled = true)]
    public class UndoLimitTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Combination starts here.</summary>
        protected override void DoRunCombination()
        {
            _tbb = (TextBoxBase)_editableType.CreateInstance();
            _tbb.AcceptsReturn = true;
            _tbbWrapper = new UIElementWrapper(_tbb);

            _tbb.IsUndoEnabled = _isUndoEnabledTestValue;

            //type "x{ENTER} couple of times so that we have undo units created for each operation.
            _typedContent = TextUtils.RepeatString("x{ENTER}", TestUndoLimitValue);
            _expectedTypedContent = TextUtils.RepeatString("x\r\n", TestUndoLimitValue);
            //
            _undoActionString = TextUtils.RepeatString("^z", TestUndoLimitValue);
            _expectedContentAfterUndo = TextUtils.RepeatString("x\r\n", TestUndoLimitValue / 2);
            _redoActionString = TextUtils.RepeatString("^y", TestUndoLimitValue);

            Verifier.Verify(_tbb.UndoLimit == DefaultUndoLimitValue,
                "Verifying the default value of the UndoLimit property", false);
            Verifier.Verify((_tbb.CanUndo == false) && (_tbb.CanRedo == false),
                "Verifying that CanUndo & CanRedo is false initially", false);

            TestElement = _tbb;
            QueueDelegate(SetTestLimitValue);
        }

        private void SetTestLimitValue()
        {
            Verifier.Verify(_tbb.UndoLimit == DefaultUndoLimitValue,
                "Verifying the default value of the UndoLimit property after added to the tree", false);

            _tbb.UndoLimit = TestUndoLimitValue;
            Log("Assigning [" + TestUndoLimitValue + "] to UndoLimit property");
            Verifier.Verify(_tbb.UndoLimit == TestUndoLimitValue,
                "Verifying value of UndoLimit property to the assigned value", false);

            _tbb.Focus();
            QueueDelegate(DoType);
        }

        private void DoType()
        {
            KeyboardInput.TypeString(_typedContent);
            QueueDelegate(DoUndo);
        }

        private void DoUndo()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";

            }
            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying initial state - content after before performing undo command: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            KeyboardInput.TypeString(_undoActionString);
            QueueDelegate(CheckUndoAtTestValue);
        }

        private void CheckUndoAtTestValue()
        {
            if (_isUndoEnabledTestValue)
            {
                _expectedContent = _expectedContentAfterUndo;
            }
            else
            {
                _expectedContent = _expectedTypedContent;
            }

            //extra newline for the last paragraph in RTB
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                "Verifying content after performing undo command: ActualContent [" + _tbbWrapper.Text +
                "] ExpectedContent [" + _expectedContent + "]", true);

            //perform undo after limit is reached
            KeyboardInput.TypeString("^z");

            QueueDelegate(CheckUndoAtTestValueAfterLimit);
        }

        private void CheckUndoAtTestValueAfterLimit()
        {
            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                "Verifying content after performing undo command with limit reached: " +
                "ActualContent [" + _tbbWrapper.Text + "] ExpectedContent [" + _expectedContent + "]", true);

            //perform redo all the way back            
            KeyboardInput.TypeString(_redoActionString);
            QueueDelegate(CheckRedoAtTestValue);
        }

        private void CheckRedoAtTestValue()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";

            }
            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing redo command: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            //Perform undo again to make sure things are working after performing redo
            KeyboardInput.TypeString(_undoActionString);
            QueueDelegate(CheckUndoAtTestValueAgain);
        }

        private void CheckUndoAtTestValueAgain()
        {
            if (_isUndoEnabledTestValue)
            {
                _expectedContent = _expectedContentAfterUndo;
            }
            else
            {
                _expectedContent = _expectedTypedContent;
            }
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing undo command again: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            SetMaxIntValue();
        }

        private void SetMaxIntValue()
        {
            //Testing at Int32.MaxValue   
            ClearBox();
            Log("Assigning Int32.MaxValue[" + Int32.MaxValue + "] to UndoLimit property");
            _tbb.UndoLimit = Int32.MaxValue;
            Verifier.Verify(_tbb.UndoLimit == Int32.MaxValue,
                "Verifying value of UndoLimit property to the assigned value", false);
            Verifier.Verify((_tbb.CanUndo == false) && (_tbb.CanRedo == false),
                "Verifying that CanUndo & CanRedo is false after UndoLimit property is set to Int.Max", true);

            Log("Type and perform undo with UndoLimit at Int32.MaxValue");
            KeyboardInput.TypeString(_typedContent + _undoActionString);

            QueueDelegate(CheckContentAtMaxIntAfterUndo);
        }

        private void CheckContentAtMaxIntAfterUndo()
        {
            if (_isUndoEnabledTestValue)
            {
                _expectedContent = _expectedContentAfterUndo;
            }
            else
            {
                _expectedContent = _expectedTypedContent;
            }
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing undo command with UndoLimit at Int32.MaxValue: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            KeyboardInput.TypeString(_redoActionString);
            QueueDelegate(CheckContentAtMaxIntAfterRedo);
        }

        private void CheckContentAtMaxIntAfterRedo()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing redo command with UndoLimit at Int32.MaxValue: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            SetMinusOneValue();
        }

        private void SetMinusOneValue()
        {
            //Testing with special value of -1 (infinite undostack)
            ClearBox();
            Log("Assigning [-1] to UndoLimit property");
            _tbb.UndoLimit = -1;
            Verifier.Verify(_tbb.UndoLimit == -1,
                "Verifying value of UndoLimit property to the assigned value", false);
            Verifier.Verify((_tbb.CanUndo == false) && (_tbb.CanRedo == false),
                "Verifying that CanUndo & CanRedo is false after UndoLimit property is set to -1", true);

            Log("Type and perform undo with UndoLimit at -1");
            KeyboardInput.TypeString(_typedContent + _undoActionString + _undoActionString);

            QueueDelegate(CheckContentAtMinusOneAfterUndo);
        }

        private void CheckContentAtMinusOneAfterUndo()
        {
            if (_isUndoEnabledTestValue)
            {
                _expectedContent = string.Empty;
            }
            else
            {
                _expectedContent = _expectedTypedContent;
            }
            if ((_tbb is RichTextBox) && (_expectedContent != string.Empty))
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing undo command with UndoLimit at -1: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            KeyboardInput.TypeString(_redoActionString + _redoActionString);
            QueueDelegate(CheckContentAtMinusOneAfterRedo);
        }

        private void CheckContentAtMinusOneAfterRedo()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing redo command with UndoLimit at -1: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            SetZeroValue();
        }

        private void SetZeroValue()
        {
            //Testing with special value of 0 (no undo)
            ClearBox();
            Log("Assigning [0] to UndoLimit property");
            _tbb.UndoLimit = 0;
            Verifier.Verify(_tbb.UndoLimit == 0,
                "Verifying value of UndoLimit property to the assigned value", false);
            Verifier.Verify((_tbb.CanUndo == false) && (_tbb.CanRedo == false),
                "Verifying that CanUndo & CanRedo is false after UndoLimit property is set to 0", true);

            Log("Type and perform undo with UndoLimit at 0");
            KeyboardInput.TypeString(_typedContent + _undoActionString);

            QueueDelegate(CheckContentAfterUndoAtZero);
        }

        private void CheckContentAfterUndoAtZero()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing undo command with UndoLimit at 0: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            KeyboardInput.TypeString(_redoActionString);
            QueueDelegate(CheckContentAfterRedoAtZero);
        }

        private void CheckContentAfterRedoAtZero()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing redo command with UndoLimit at 0: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            ChangeIsUndoEnabledValue();
        }

        private void ChangeIsUndoEnabledValue()
        {
            //Change the value of IsUndoEnabled
            ClearBox();
            _tbb.ClearValue(TextBoxBase.UndoLimitProperty);
            if (_isUndoEnabledTestValue)
            {
                _tbb.IsUndoEnabled = false;
                Verifier.Verify(_tbb.IsUndoEnabled == false, "Verifying that IsUndoEnabled is changed after assignment", false);
                Log("Type + Undo after IsUndoEnabled is changed from Enabled to Disabled");
            }
            else
            {
                _tbb.IsUndoEnabled = true;
                Verifier.Verify(_tbb.IsUndoEnabled == true, "Verifying that IsUndoEnabled is changed after assignment", false);
                Log("Type + Undo after IsUndoEnabled is changed from Disabled to Enabled");
            }

            KeyboardInput.TypeString(_typedContent + _undoActionString);
            QueueDelegate(CheckUndoAfterIsUndoEnabledChanged);
        }

        private void CheckUndoAfterIsUndoEnabledChanged()
        {
            if (_tbb.IsUndoEnabled)
            {
                _expectedContent = _expectedContentAfterUndo;
            }
            else
            {
                _expectedContent = _expectedTypedContent;
            }
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing undo command with IsUndoEnabled value changed: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            KeyboardInput.TypeString(_redoActionString);
            QueueDelegate(CheckRedoAfterIsUndoEnabledChanged);
        }

        private void CheckRedoAfterIsUndoEnabledChanged()
        {
            _expectedContent = _expectedTypedContent;
            if (_tbb is RichTextBox)
            {
                _expectedContent = _expectedContent + "\r\n";
            }

            Verifier.Verify(_tbbWrapper.Text == _expectedContent,
                    "Verifying content after performing redo command with IsUndoEnabled value changed: " +
                    "Actual [" + _tbbWrapper.Text + "] Expected [" + _expectedContent + "]", true);

            QueueDelegate(DoNegativeTesting);
        }

        private void DoNegativeTesting()
        {
            //set the value back to the test value
            _tbb.IsUndoEnabled = _isUndoEnabledTestValue;

            //Negative testing with -ve value
            try
            {
                _tbb.UndoLimit = -10;
                Verifier.Verify(false, "UndoLimit has accepted -ve value of 10", false);
            }
            catch (System.ArgumentException)
            {
                Log("Exception thrown as expected when -ve value is assigned to UndoLimit");
            }

            //Assigning value inside change block
            _tbb.ClearValue(TextBoxBase.UndoLimitProperty);
            using (_tbb.DeclareChangeBlock())
            {
                try
                {
                    _tbb.UndoLimit = 100;
                    if (_isUndoEnabledTestValue)
                    {
                        Verifier.Verify(false, "UndoLimit value is changed inside a DeclareChangeBlock", false);
                    }
                }
                catch (System.InvalidOperationException)
                {
                    Log("Exception thrown as expected when value is changed inside a DeclareChangeBlock");
                }
            }

            _tbb.BeginChange();
            try
            {
                _tbb.UndoLimit = 1000;
                if (_isUndoEnabledTestValue)
                {
                    Verifier.Verify(false, "UndoLimit value is changed inside a DeclareChangeBlock", false);
                }
            }
            catch (System.InvalidOperationException)
            {
                Log("Exception thrown as expected when value is changed inside a DeclareChangeBlock");
            }
            _tbb.EndChange();
            NextCombination();
        }

        private void ClearBox()
        {
            if (_tbb is TextBox)
            {
                ((TextBox)_tbb).Clear();
            }
            else if (_tbb is RichTextBox)
            {
                ((RichTextBox)_tbb).Document.Blocks.Clear();
            }
        }

        #endregion

        #region Private fields

        private TextEditableType _editableType = null;
        private bool _isUndoEnabledTestValue = true;
        private TextBoxBase _tbb;
        private UIElementWrapper _tbbWrapper;
        private const int DefaultUndoLimitValue = -1;
        //Has to be even number, otherwise expectedContent calculation logic will go wrong.
        private const int TestUndoLimitValue = 4;
        private string _typedContent,_expectedTypedContent,_expectedContent,_expectedContentAfterUndo;
        private string _undoActionString,_redoActionString;

        #endregion
    }
}
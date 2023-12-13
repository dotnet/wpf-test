// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for undo/redo functionality.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Editing/UndoRedoTests.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that Undo/Redo actions work with all keyboard input.
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem("28,22"), TestTactics("325")]
    public class UndoRedoTypingTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads a combination and determines whether it should run.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            return result;
        }

        /// <summary>Runs the test case.</summary>
        protected override void DoRunCombination()
        {
            _undoStack = new Stack<string>();
            _redoStack = new Stack<string>();

            _control = (Control) _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(_control);
            TestElement = _control;
            _initialContent = GetContent();

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(SetupInitialStacks);
        }

        private void SetupInitialStacks()
        {
            int totalItemCount;
            int setupItemCount;

            // Total item count is:
            // + Items that will remain in undo.
            // + Items 'over' undo.
            // + Items moving 'over' into redo stack.
            totalItemCount = _undoStackCount + _redoStackCount + _redoStackCount;
            setupItemCount = _undoStack.Count + _redoStack.Count * 2;
            if (setupItemCount < totalItemCount)
            {
                if (setupItemCount < _undoStackCount + _redoStackCount)
                {
                    _undoStack.Push(GetContent());
                    KeyboardInput.TypeString("a{END}");
                    QueueDelegate(SetupInitialStacks);
                }
                else
                {
                    _undoStack.Pop();
                    _redoStack.Push(GetContent());
                    KeyboardInput.TypeString("^z");
                    QueueDelegate(SetupInitialStacks);
                }
            }
            else
            {
                QueueDelegate(DoKeyboardEditing);
            }
        }

        private void DoKeyboardEditing()
        {
            if (_editingData != null)
            {
                // Capture current state and perform the editing action.
                _editingState = _editingData.CaptureBeforeEditing(_wrapper);
                _undoStack.Push(GetContent());
                if (!_editingData.IsNavigationAction)
                {
                    _redoStack.Clear();
                }
                _editingData.PerformAction(_wrapper, CheckEditing);
            }
            else
            {
                CheckEditing();
            }
        }

        private void CheckEditing()
        {
            if (_editingData != null)
            {
                _editingData.VerifyEditing(_editingState);
            }

            _contentAfterEditing = GetContent();
            _pendingOperationCount = _operationCount;

            QueueDelegate(PerformUndo);
        }

        private void PerformUndo()
        {
            // Capture state for redo, then perform undo.
            if (_undoStack.Count > 0)
            {
                _redoStack.Push(GetContent());
            }
            KeyboardInput.TypeString("^z");
            QueueDelegate(CheckUndo);
        }

        private void CheckUndo()
        {
            string currentContent;
            string expectedContent;

            // Verify that the action returns the control
            // to the previous state.
            currentContent = GetContent();
            if (_editableType.IsPassword)
            {
                expectedContent = _contentAfterEditing;
            }
            else if (_undoStack.Count == 0)
            {
                Log("No change expected.");
                expectedContent = _initialContent;
            }
            else
            {
                expectedContent = _undoStack.Pop();
            }
            Verifier.Verify(currentContent == expectedContent,
                "Expected content [" + expectedContent + "] matches current [" +
                currentContent + "]", true);

            _pendingOperationCount--;
            if (_pendingOperationCount > 0)
            {
                QueueDelegate(PerformUndo);
            }
            else
            {
                _pendingOperationCount = _operationCount;
                QueueDelegate(PerformRedo);
            }
        }

        private void PerformRedo()
        {
            // Perform Redo.
            if (_redoStack.Count > 0)
            {
                _undoStack.Push(GetContent());
                _contentBeforeVoidRedo = null;
            }
            else
            {
                _contentBeforeVoidRedo = GetContent();
            }
            KeyboardInput.TypeString("^y");
            QueueDelegate(CheckRedo);
        }

        private void CheckRedo()
        {
            string currentContent;
            string modifiedContent;

            if (_editableType.IsPassword)
            {
                modifiedContent = _contentAfterEditing;
            }
            else if (_redoStack.Count == 0)
            {
                Log("No change expected.");
                modifiedContent = _contentBeforeVoidRedo;
            }
            else
            {
                modifiedContent = _redoStack.Pop();
            }
            currentContent = GetContent();
            Verifier.Verify(modifiedContent == currentContent,
                "Modified content [" + modifiedContent + "] matches current [" +
                currentContent + "]", true);

            _pendingOperationCount--;
            if (_pendingOperationCount > 0)
            {
                QueueDelegate(PerformRedo);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private string GetContent()
        {
            if (_wrapper.IsElementRichText)
            {
                return _wrapper.XamlText;
            }
            else
            {
                return _wrapper.Text;
            }
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Editing state before operation.</summary>
        private KeyboardEditingState _editingState;

        /// <summary>Control being edited.</summary>
        private Control _control;

        /// <summary>Wrapper for control begin edited.</summary>
        private UIElementWrapper _wrapper;

        /// <summary>Modeled undo stack.</summary>
        private Stack<string> _undoStack;

        /// <summary>Modeled redo stack.</summary>
        private Stack<string> _redoStack;

        /// <summary>Content that the control begins with.</summary>
        /// <remarks>
        /// This is what we compare with when we undo and there were no
        /// undo units.
        /// </remarks>
        private string _initialContent;

        /// <summary>Content that the control had immediately after the editing action.</summary>
        private string _contentAfterEditing ="";

        /// <summary>Content that the control has before void redo.</summary>
        /// <remarks>
        /// This is what we compare with when we redo and there was nothing
        /// on the redo stack.
        /// </remarks>
        private string _contentBeforeVoidRedo ="";

        /// <summary>Number of pending operations for undo or redo.</summary>
        private int _pendingOperationCount;

        /// <summary>Type of editable control being tested.</summary>
        private TextEditableType _editableType = null;

        /// <summary>Data about editing operation performed.</summary>
        private KeyboardEditingData _editingData = null;

        /// <summary>Number of items on the undo stack.</summary>
        private int _undoStackCount = 0;

        /// <summary>Number of items on the redo stack.</summary>
        private int _redoStackCount = 0;

        /// <summary>Number of operations to be performed.</summary>
        private int _operationCount = 0;

        #endregion Private fields.
    }
}

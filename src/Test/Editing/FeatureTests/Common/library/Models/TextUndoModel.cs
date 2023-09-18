// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a model for undo/redo operations.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Common/Library/Models/TextUndoModel.cs $")]

namespace Test.Uis.Data
{
    #region Namespaces.

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Threading;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>Model for undo/redo operations.</summary>
    public class TextUndoModel
    {
        #region Constructors.

        /// <summary>
        /// Initializes a new TextUndoModel instance.
        /// </summary>
        /// <param name="wrapper">Wrapper for control being used.</param>
        public TextUndoModel(UIElementWrapper wrapper)
        {
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }
            this._wrapper = wrapper;
            this._undoStack = new Stack<string>();
            this._redoStack = new Stack<string>();
        }

        #endregion Constructors.


        #region Public methods.

        /// <summary>Captures the current state in an undo unit.</summary>
        public void CaptureUndo()
        {
            this._undoStack.Push(CurrentState);
            this._redoStack.Clear();
        }

        /// <summary>Captures the current state in an undo unit.</summary>
        public void PerformRedo()
        {
            this._undoStack.Push(this._redoStack.Pop());
        }

        /// <summary>Captures the current state in an undo unit.</summary>
        public void PerformUndo()
        {
            this._redoStack.Push(this._undoStack.Pop());
        }

        /// <summary>Captures the current state in an undo unit.</summary>
        public void VerifyRedo()
        {
            Verifier.VerifyText("current state matches top of undo stack after redo",
                this._undoStack.Peek(), CurrentState, false);
        }

        /// <summary>Captures the current state in an undo unit.</summary>
        public void VerifyUndo()
        {
            Verifier.VerifyText("current state matches top of undo stack after undo",
                this._undoStack.Peek(), CurrentState, false);
        }

        /// <summary>
        /// Schedules a series of undo and redo operations, up to
        /// the specified number.
        /// </summary>
        /// <example>The following code shows how to use this method.<code>...
        /// class MyTest {
        ///   private void Run() {
        ///     KeyboardInput.TypeString("foo");
        ///     QueueDelegate(CheckUndoAfterTyping);
        ///   }
        ///   private void CheckUndoAfterTyping) {
        ///     MyUndoModel.CaptureUndo(); // Called after every undo unit is generated.
        ///     MyUndoModel.VerifyUndoOperations(3, MyNextMethod);
        ///   }
        /// }
        /// </code></example>
        public void VerifyUndoOperations(int maxUndoDepth, SimpleHandler callback)
        {
            if (maxUndoDepth < 0)
            {
                throw new ArgumentException("Maximum undo depth must be positive.", "maxUndoDepth");
            }

            new UndoWithChecksHelper(this, maxUndoDepth, callback).Start();
        }

        #endregion Public methods.


        #region Public properties.

        /// <summary>Whether a Redo operation can be performed.</summary>
        public bool CanRedo
        {
            get { return this._redoStack.Count > 0; }
        }

        /// <summary>Whether an Undo operation can be performed.</summary>
        public bool CanUndo
        {
            get { return this._undoStack.Count > 0; }
        }

        /// <summary>Current state to be preserved.</summary>
        public string CurrentState
        {
            get
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
        }

        #endregion Public properties.


        #region Private fields.

        /// <summary>Stack with redo contents.</summary>
        private Stack<string> _redoStack;

        /// <summary>Stack with undo contents.</summary>
        private Stack<string> _undoStack;

        /// <summary>Wrapper around control being tested.</summary>
        private UIElementWrapper _wrapper;

        #endregion Private fields.


        #region Inner types.

        /// <summary>
        /// Encapsulates a request to perform undo / redo operations, verifying
        /// each step.
        /// </summary>
        class UndoWithChecksHelper
        {
            internal UndoWithChecksHelper(TextUndoModel model, int maxUndoDepth, SimpleHandler callback)
            {
                this._model = model;
                this._callback = callback;
                this._undoOperationsPending = maxUndoDepth;
                this._isUndoing = true;
            }

            internal void Start()
            {
                QueueHelper.Current.QueueDelegate(PerformStep);
            }

            private void PerformStep()
            {
                if (_isUndoing)
                {
                    if (_undoOperationsPending == 0 || !_model.CanUndo)
                    {
                        _isUndoing = false;
                    }
                    else
                    {
                        _undoOperationsPending--;
                        _redoOperationsPending++;
                        KeyboardInput.TypeString("^z");
                        QueueHelper.Current.QueueDelegate(AfterUndo);
                        return;
                    }
                }
                if (!_isUndoing)
                {
                    if (_redoOperationsPending == 0)
                    {
                        QueueHelper.Current.QueueDelegate(_callback);
                    }
                    else
                    {
                        _redoOperationsPending--;
                        KeyboardInput.TypeString("^y");
                        QueueHelper.Current.QueueDelegate(AfterRedo);
                    }
                }
            }

            private void AfterRedo()
            {
                _model.PerformRedo();
                _model.VerifyRedo();
                Logger.Current.Log("Redo operation succeeded.");
                PerformStep();
            }

            private void AfterUndo()
            {
                _model.PerformUndo();
                _model.VerifyUndo();
                Logger.Current.Log("Undo operation succeeded.");
                PerformStep();
            }

            private bool _isUndoing;
            private int _undoOperationsPending;
            private int _redoOperationsPending;
            private SimpleHandler _callback;
            private TextUndoModel _model;
        }

        #endregion Inner types.
    }
}

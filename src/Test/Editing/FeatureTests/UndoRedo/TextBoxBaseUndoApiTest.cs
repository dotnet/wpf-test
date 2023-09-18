// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


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
    
    /// <summary>Programmatic actions that may generate undo/redo units.</summary>
    public enum ProgramUndoRedoActions
    {
        /// <summary>Text property.</summary>
        TextProperty,
        
        /// <summary>SelectedText property.</summary>
        SelectedTextProperty,
        
        /// <summary>AppendText method.</summary>
        AppendTextMethod,

        /// <summary>Clear method.</summary>
        ClearMethod,

        /// <summary>ClearValue method, using the Text property.</summary>
        ClearValueMethod,

        /// <summary>Cut method.</summary>
        CutMethod,

        /// <summary>Paste method.</summary>
        PasteMethod,

        /// <summary>IValueProvider.SetValue method.</summary>
        IValueProviderSetValueMethod,

        /// <summary>Updating a binding source.</summary>
        BindingSourceUpdate,

        /// <summary>Animating the property.</summary>
        AnimationUpdate,

        /// <summary>Updating the style, triggering a Text update.</summary>
        StyleUpdate,
    }

    /// <summary>Provides coverage for the undo/redo API in TextBoxBase.</summary>
    /// <remarks>
    /// Mini test-matrix:
    /// - What the initial value of a change is
    ///   - empty / non-empty
    /// - What the final value of a change is
    ///   - empty / non-empty
    /// - How the value is updated
    ///   - Text property
    ///   - SelectedText property
    ///   - AppendText method
    ///   - Clear method
    ///   - ClearValue method
    ///   - Cut method
    ///   - Paste method
    ///   - IValueProvider.SetValue method
    ///   - Binding source updated
    ///   - Animation updated
    ///   - Style updated
    /// - Whether action occurs in a change block
    ///   - true / false
    /// - Initial IsUndoEnabled value
    ///   - true / false
    /// - Final IsUndoEnabled value
    ///   - true / false
    /// - IsUndoEnabled changing
    ///   - before change / after change
    ///
    /// Test actions:
    /// - Set up the initial value of Text for the desired effect.
    /// - Set up the initial value of IsUndoEnabled for the desired effect.
    /// - (optionally) Start a change block.
    /// - Update the content.
    /// - Set up the initial value of IsUndoEnabled for the desired effect.
    /// - (optionally) Change IsUndoEnabled.
    /// - (optionally) End the change block.
    ///
    /// What to verify:
    /// - whether CanUndo and CanRedo are true/false as expected
    /// - whether Undo and Redo go to the expected states
    /// - whether Undo and Redo return the expected boolean values
    /// </remarks>
    [TestOwner("Microsoft"), TestBugs("766,767,768,769, 770"), TestTactics("372"), TestWorkItem("")]
    public class TextBoxBaseUndoApiTest : ManagedCombinatorialTestCase
    {
        #region Main flow.
        
        /// <summary>Reads a combination and determines whether it should run.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            result = result && typeof(TextBoxBase).IsAssignableFrom(this._editableType.Type);
            result = result && !(
                typeof(RichTextBox).IsAssignableFrom(this._editableType.Type) &&
                _undoRedoAction == ProgramUndoRedoActions.ClearValueMethod
                );
                
            result = result && !(
                typeof(RichTextBox).IsAssignableFrom(this._editableType.Type) &&
                _undoRedoAction == ProgramUndoRedoActions.ClearMethod
                );
                
            result = result && _finalIsUndoEnabled != false;
            
            result = result && !(
                _finalIsEmpty == true &&
                _undoRedoAction == ProgramUndoRedoActions.AppendTextMethod
                );

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            // Prepare the control to be tested.
            _control = (TextBoxBase)_editableType.CreateInstance();
            _control.AcceptsReturn = true;

            _wrapper = new UIElementWrapper(_control);
            _undoModel = new TextUndoModel(_wrapper);
            _undoModel.CaptureUndo();

            TestElement = _control;
            QueueDelegate(StartInteraction);
        }

        private void StartInteraction()
        {
            Log("Verifying that new controls cannot undo or redo...");
            Verifier.Verify(_control.CanUndo == false, "CanUndo is false", false);
            Verifier.Verify(_control.CanRedo == false, "CanRedo is false", false);
            
            SetupControl();
            UpdateContent();
            VerifyActionResults();
        }

        private void SetupControl()
        {
            if (!_initialIsEmpty)
            {
                _control.IsUndoEnabled = false;
                _wrapper.Text = SampleText;
                _control.IsUndoEnabled = true;
            }
            
            _control.IsUndoEnabled = _initialIsUndoEnabled;
            
            if (_useChangeBlock)
            {
                _control.BeginChange();
            }
        }
        
        private void UpdateContent()
        {
            string targetText;
            
            targetText = _finalIsEmpty? "" : FinalSampleText;
            
            switch (_undoRedoAction)
            {
                case ProgramUndoRedoActions.TextProperty:
                    _wrapper.Text = targetText;
                    break;
        
                case ProgramUndoRedoActions.SelectedTextProperty:
                    _wrapper.SelectAll();
                    if (_control is TextBox)
                    {
                        ((TextBox)_control).SelectedText = targetText;
                    }
                    else if (_control is RichTextBox)
                    {
                        ((RichTextBox)_control).Selection.Text = targetText;
                    }
                    break;
        
                case ProgramUndoRedoActions.AppendTextMethod:
                    _control.AppendText(targetText);
                    break;

                case ProgramUndoRedoActions.ClearMethod:
                    if (_control is TextBox)
                    {
                        ((TextBox)_control).Clear();
                    }
                    else if (_control is RichTextBox)
                    {
                        ((RichTextBox)_control).Document = new FlowDocument();
                    }
                    break;

                case ProgramUndoRedoActions.ClearValueMethod:
                    _control.ClearValue(TextBox.TextProperty);
                    break;

                case ProgramUndoRedoActions.CutMethod:
                    _wrapper.SelectAll();
                    _control.Cut();
                    break;

                case ProgramUndoRedoActions.PasteMethod:
                    System.Windows.Clipboard.SetDataObject(targetText, false);
                    _control.Paste();
                    break;

                case ProgramUndoRedoActions.IValueProviderSetValueMethod:
                    break;

                case ProgramUndoRedoActions.BindingSourceUpdate:
                    break;

                case ProgramUndoRedoActions.AnimationUpdate:
                    break;

                case ProgramUndoRedoActions.StyleUpdate:
                    break;
            }
        }
        
        private void VerifyActionResults()
        {
            bool canUndoExpected;
            string log;
            
            if (_useChangeBlock)
            {
                if (_control.IsUndoEnabled)
                {
                    _control.EndChange();
                }
                else
                {
                    try
                    {
                        _control.EndChange();
                        throw new ApplicationException(
                            "Exception expected when IsUndoEnabled=False and EndChange is invoked.");
                    }
                    catch(InvalidOperationException)
                    {
                        // Exception thrown, as expected.
                    }
                }
            }
            _control.IsUndoEnabled = _finalIsUndoEnabled;

            if (!_initialIsUndoEnabled)
            {
                // If undo is disabled, it should always be false.
                canUndoExpected = false;
            }
            else if (_initialIsEmpty && _finalIsEmpty && _control is TextBox)
            {
                // In TextBox, not changing the Text property causes undo units
                // to not be generated.
                canUndoExpected = false; 
            }
            else if (_initialIsEmpty && _control is TextBox &&
                (_undoRedoAction == ProgramUndoRedoActions.ClearMethod ||
                 _undoRedoAction == ProgramUndoRedoActions.ClearValueMethod ||
                 _undoRedoAction == ProgramUndoRedoActions.CutMethod))
            {
                // Clearing value from empty won't generate any changes.
                canUndoExpected = false;
            }
            else if (_finalIsEmpty && _undoRedoAction == ProgramUndoRedoActions.PasteMethod)
            {
                // Empty pasting never generates an undo unit.
                canUndoExpected = false;
            }
            else if (_finalIsEmpty && _undoRedoAction == ProgramUndoRedoActions.AppendTextMethod)
            {
                // Empty appending never generates an undo unit.
                canUndoExpected = false;
            }
            else if (
                _undoRedoAction == ProgramUndoRedoActions.IValueProviderSetValueMethod ||
                _undoRedoAction == ProgramUndoRedoActions.BindingSourceUpdate ||
                _undoRedoAction == ProgramUndoRedoActions.AnimationUpdate ||
                _undoRedoAction == ProgramUndoRedoActions.StyleUpdate
                )
            {
                // Consider unimplemented changes
                canUndoExpected = false;
            }
            else
            {   
                canUndoExpected = true;
            }

            log = "Expected CanUndo to be " + canUndoExpected + 
                ", found to be " + _control.CanUndo;
            Verifier.Verify(_control.CanUndo == canUndoExpected, log, false);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.
        
        #region Private data.
        
        /// <summary>Control being tested.</summary>
        private TextBoxBase _control;

        /// <summary>Wrapper for control being tested.</summary>
        private UIElementWrapper _wrapper;
        
        private const string SampleText = "Sample text";
        private const string FinalSampleText = "Final text";
        private bool _initialIsUndoEnabled=false;
        private bool _initialIsEmpty=false;
        private bool _finalIsUndoEnabled=false;
        private bool _finalIsEmpty=false;
        private bool _useChangeBlock=false;
        private ProgramUndoRedoActions _undoRedoAction=0;

        /// <summary>EditableType of control tested.</summary>
        private TextEditableType _editableType=null;

        /// <summary>Model of undo/redo stacks.</summary>
        private TextUndoModel _undoModel;
        
        #endregion Private data.
    }

     /// <summary>Tests that nested change blocks work fine.</summary>
    [Test(0, "UndoRedo", "TestChangeBlocks", MethodParameters = "/TestCaseType=TestChangeBlocks")]
    [TestOwner("Microsoft"), TestTitle("TestChangeBlocks"), TestTactics("371"), TestLastUpdatedOn("June 06,2006"), TestWorkItem("40")]
    public class TestChangeBlocks : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            _rtb = _wrapper.Element as RichTextBox;
            TestElement = (FrameworkElement)_wrapper.Element;
            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(new Paragraph(new Run("Hey THERE !!!! ")));
            _rtb.FontSize = 20;
            QueueDelegate(TestChangeUndoRedoBlocks);
        }

        private void TestChangeUndoRedoBlocks()
        {
            _rtb.SelectAll();
            Brush _initialBrush = _rtb.Selection.GetPropertyValue(Paragraph.ForegroundProperty) as Brush;
            FontWeight _initialFontWeight = (FontWeight)_rtb.Selection.GetPropertyValue(Paragraph.FontWeightProperty);

            try
            {
                if (_commandCommentIndex != 1)
                {
                    Log("Perform BeginChange");
                    _rtb.BeginChange();//1
                }

                if (_commandCommentIndex != 2)
                {
                    Log("Perform ApplyForeground");
                    _rtb.Selection.ApplyPropertyValue(Paragraph.ForegroundProperty, Brushes.Red); //2
                }
                if (_commandCommentIndex == 3)
                {
                    Log("Perform Undo");
                    _rtb.Undo();// 3
                }
                if (_commandCommentIndex != 4)
                {
                    Log("Perform BeginChange");
                    _rtb.BeginChange(); // 4
                }
                if (_commandCommentIndex != 5)
                {
                    Log("Perform ApplyFontWeight");
                    _rtb.Selection.ApplyPropertyValue(Paragraph.FontWeightProperty, FontWeights.Bold); //5
                }
                if (_commandCommentIndex == 6)
                {
                    Log("Perform Undo");
                    _rtb.Undo(); //6
                }
                if (_commandCommentIndex != 7)
                {
                    Log("Perform EndChange");
                    _rtb.EndChange(); //7
                }
                if (_commandCommentIndex != 8)
                {
                    Log("Perform EndChange");
                    _rtb.EndChange(); //8
                }
                Log("Perform Undo");
                _rtb.Undo();
                switch (_commandCommentIndex)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                        {
                            throw new ApplicationException("InvalidOperation exception should be thrown when Statement #" +
                            _commandCommentIndex.ToString() + "is commented");
                        }

                    default:
                        {
                            Brush _afterUndoBrush = _rtb.Selection.GetPropertyValue(Paragraph.ForegroundProperty) as Brush;
                            FontWeight _afterUndoFontWeight = (FontWeight)_rtb.Selection.GetPropertyValue(Paragraph.FontWeightProperty);

                            Verifier.Verify(_afterUndoBrush == _initialBrush, "Expected Foreground to revert to original Expected [" + _initialBrush.ToString()+
                                "] Actual ["+ _afterUndoBrush.ToString() +"]", true);
                            Verifier.Verify(_afterUndoFontWeight.ToString() == _initialFontWeight.ToString(), "Expected FontWeight to revert to original Expected [" +_initialFontWeight.ToString()+
                                "] Actual ["+ _afterUndoFontWeight.ToString() +"]", true);

                        }
                        break;
                }
            }
            catch (Exception e)
            {
                switch (_commandCommentIndex)
                {
                    case 1:
                    case 3:
                    case 4:
                    case 6:
                    case 7:
                    case 8:
                        {
                            if (e is InvalidOperationException)
                            {                                Log("InvalidOperationException thrown as expected");
                            }
                            else
                            {
                                throw new ApplicationException("Invalid Operation exception not thrown when Statement #" + _commandCommentIndex.ToString() +
                                    "is commented");
                            }
                        }
                        break;

                    default: throw new ApplicationException(e.ToString() + " is thrown which is NOT expected when Statement #" +
                        _commandCommentIndex.ToString() + "is commented");
                }                
            }
            finally
            {
                if ((_commandCommentIndex == 3) || (_commandCommentIndex == 7) || (_commandCommentIndex == 8))
                {
                    _rtb.EndChange();
                }
                if (_commandCommentIndex == 6)
                {
                    _rtb.EndChange();
                    _rtb.EndChange();
                }
            }
            NextCombination();
        }

        #region data.

        private RichTextBox  _rtb = null;
        private UIElementWrapper _wrapper = null;
        private TextEditableType _editableType = null;
        private int _commandCommentIndex = 0; 

        #endregion data.
    }       
}

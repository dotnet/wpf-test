// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Intra-feature testing of Editing and AdvancedTypography features

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;    
    using System.Windows.Markup;    
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that Editing functions work with all advanced typography features.
    /// </summary>
    [Test(2, "RichEditing", "AdvancedTypographyTest", MethodParameters = "/TestCaseType:AdvancedTypographyTest", Timeout=800)]
    [TestOwner("Microsoft"), TestTactics("470"), TestBugs("")]
    public class AdvancedTypographyTest : CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {                                   
            return new Dimension[] {  
                new Dimension("EditControl", new TextEditableType[] {TextEditableType.GetByName("TextBox"), 
                    TextEditableType.GetByName("RichTextBox")}),
                new Dimension("TypographyFontData", TypographyFontData.Values),                
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _textEditableType = (TextEditableType)values["EditControl"];
            _typographyFontData = (TypographyFontData)values["TypographyFontData"];            

            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _editControl = (TextBoxBase)_textEditableType.CreateInstance();
            _wrapper = new UIElementWrapper(_editControl);

            TestElement = _editControl;

            _editControl.AcceptsReturn = true;
            if (_editControl is TextBox)
            {
                ((TextBox)_editControl).TextWrapping = TextWrapping.Wrap;
            }
            _editControl.FontSize = 16;

            if (_wrapper.IsPointerAllowedOnThis)
            {
                ((RichTextBox)_wrapper.Element).Document.Blocks.Clear();
                ((RichTextBox)_wrapper.Element).Document.Blocks.Add(_typographyFontData.GetSampleParagraph());               
            }
            else
            {
                _typographyFontData.ApplyToObject(_wrapper.Element);
                _wrapper.Text = _typographyFontData.TextContent;                
            }

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _editControl.Focus();
            _previousContents = _wrapper.Text;
            QueueDelegate(SetCaretToMiddle);
        }

        private void SetCaretToMiddle()
        {
            TextPointer midPointPointer;

            midPointPointer = _wrapper.Start.GetPositionAtOffset(_wrapper.Start.GetOffsetToPosition(_wrapper.End) / 2);
            midPointPointer = midPointPointer.GetInsertionPosition(LogicalDirection.Forward);            
            _wrapper.SelectionInstance.Select(midPointPointer, midPointPointer);            
                        
            QueueDelegate(DoNavigateAndSelection);
        }

        private void DoNavigateAndSelection()
        {
            Verifier.Verify((_wrapper.TextBeforeOrAfterCaret(LogicalDirection.Backward) != String.Empty) &&
                (_wrapper.TextBeforeOrAfterCaret(LogicalDirection.Forward) != String.Empty),
                "Verifying that caret is in the middle", true);

            KeyboardInput.TypeString("{LEFT 2}{RIGHT 2}^+{HOME}");
            QueueDelegate(DoDelete);
        }        

        private void DoDelete()
        {
            Verifier.Verify(!_wrapper.SelectionInstance.IsEmpty,
                   "Verifying that Selection is not empty", true);

            KeyboardInput.TypeString("{DELETE}");
            
            QueueDelegate(DoUndo);
        }

        private void DoUndo()
        {
            Verifier.Verify(_wrapper.Text != _previousContents,
                "Verifying that contents are deleted", true);

            KeyboardInput.TypeString("^z");
            
            QueueDelegate(DoNavigationAndDeletion);
        }

        private void DoNavigationAndDeletion()
        {
            Verifier.Verify(_wrapper.Text == _previousContents,
                "Verifying that contents are restored after Undo", true);

            KeyboardInput.TypeString("{RIGHT}{ENTER}{BACKSPACE 2}{DELETE 2}");

            QueueDelegate(VerifyContentsChanged);
        }

        private void VerifyContentsChanged()
        {
            Verifier.Verify(_wrapper.Text != _previousContents,
                "Verifying that contents are changed", true);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields

        private TextBoxBase _editControl;
        private UIElementWrapper _wrapper;
        private TextEditableType _textEditableType;
        private TypographyFontData _typographyFontData;
        private string _previousContents;
        
        #endregion Private fields
    }
}

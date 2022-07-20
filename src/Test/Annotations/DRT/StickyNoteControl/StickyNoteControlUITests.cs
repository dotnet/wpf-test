// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Windows;
using System.Windows.Media;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Interop;
using System.Runtime.InteropServices;

namespace DRT
{
    /// <summary>
    /// This is a test suite for
    /// 1. The interactions for the various UI parts: OKButton, IconThumb, TitleThumb,ResizeThumb
    /// 2. Verify the Events: TextChanged
    /// </summary>
    public sealed class StickyNoteControlUITests : DrtStickyNoteControlTestSuite
    {
        private RichTextBox         _richTextBox;
        private Button              _okButton;

        private Thumb               _titleThumb;
        private double              _oldWidth, _oldHeigth;

        private bool                _isOKButtonClicked;
        private bool                _isTextChanged;

        // The values for typing test
        private string TextString2 = "Hello World!\r\n";

        public StickyNoteControlUITests() : base("StickyNoteControlUITests")
        {
        }

        public override DrtTest[] PrepareTests()
        {
            DrtTest[] prepareStickyNoteControlTests = base.PrepareTests();

            List<DrtTest> tests = new List<DrtTest>();

            // Tests for creating StickyNoteControl via CAF api
            foreach ( DrtTest test in prepareStickyNoteControlTests )
            {
                tests.Add(test);
            }

            // Tests after the visual tree being set up.
            tests.Add(new DrtTest(IdentifyControls));
            tests.Add(new DrtTest(TypeTest));
            tests.Add(new DrtTest(VerifyTypeTest));
            tests.Add(new DrtTest(ClickOKButton));
            tests.Add(new DrtTest(VerifyOKButtonClicked));
            tests.Add(new DrtTest(ClickIconThumb));
            tests.Add(new DrtTest(VerifyIconThumbClicked));
            tests.Add(new DrtTest(DragTitleThumb));
            tests.Add(new DrtTest(VerifyTitleThumbDragged));
            tests.Add(new DrtTest(DragResizeThumb));
            tests.Add(new DrtTest(VerifyResizeThumbDragged));

            return tests.ToArray();
        }

        #region Tests
        private void IdentifyControls()
        {
            _okButton = StickyNote.Template.FindName(DrtStickyNoteControl.c_CloseButtonId, StickyNote) as Button;
            _okButton.AddHandler(ButtonBase.ClickEvent, new RoutedEventHandler(OnOKButtonClicked));

            _titleThumb = StickyNote.Template.FindName(DrtStickyNoteControl.c_TitleBarId, StickyNote) as Thumb;
            _richTextBox = StickyNote.Template.FindName(DrtStickyNoteControl.c_ContentId, StickyNote) as RichTextBox;

            if ( _titleThumb != null )
            {
                _titleThumb.AddHandler(Thumb.DragDeltaEvent, new DragDeltaEventHandler(OnDragDelta));
            }

            if ( _richTextBox != null )
            {
                _richTextBox.TextChanged += new TextChangedEventHandler(OnTextChanged);
            }
        }

        //
        // test for typing text in the TextBox
        //
        private void TypeTest()
        {
            _isTextChanged = false;
            _richTextBox.Document = new FlowDocument(new Paragraph(new Run()));
            DRT.MouseButtonUp();            // just in case someone left it down
            DRT.MoveMouse(StickyNote, 0.5, 0.5);
            DRT.ResumeAt(new DrtTest(MouseDown));
        }

        private void MouseDown()
        {
            DRT.MouseButtonDown();
            DRT.ResumeAt(new DrtTest(MouseUp));
        }
        
        private void MouseUp()
        {
            DRT.MouseButtonUp();
            DRT.ResumeAt(new DrtTest(TypeInput));
        }

        private void TypeInput()
        {
            DrtInput.KeyboardType("+hello +world+1");
        }

        private void VerifyTextChangedEvent()
        {
            DRT.AssertEqual(true, _isTextChanged, "VerifyTextChangedEvent: No TextChanged event fired!");
        }

        private void VerifyTypeTest()
        {
            TextRange textRange = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);

            IntPtr mainHwnd = ((HwndSource)PresentationSource.FromVisual(_richTextBox)).Handle;
            IntPtr activeHwnd = GetForegroundWindow();
            string activeWindowTitle = GetWindowTitle(activeHwnd);
            DRT.Assert(mainHwnd == activeHwnd, "Main DRT window " + mainHwnd.ToString("X") + " should be foreground but instead hwnd " + 
                          activeHwnd.ToString("X") + " (" + activeWindowTitle + ") was foreground.");

            DRT.Assert(_richTextBox == Keyboard.FocusedElement, "VerifyTypeTest: Focused Element is : " + Keyboard.FocusedElement);
            DRT.AssertEqual(TextString2, textRange.Text, "VerifyTypeTest: Types Text error!");
        }

        [DllImport("user32.dll", ExactSpelling = true, CharSet = CharSet.Auto)]
        private static extern IntPtr GetForegroundWindow();

        private static string GetWindowTitle(IntPtr hwnd)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder(500);
            MS.Win32.UnsafeNativeMethods.GetWindowText(new HandleRef(null, hwnd), sb, 500);
            return sb.ToString();
        }
        //
        // test for clicking the OK button.
        //
        private void ClickOKButton()
        {
            DRT.AssertEqual(true, StickyNoteProxy.IsExpanded, "ClickOKButton: IsExpanded should be true!");
            _isOKButtonClicked = false;

            UIElement target = _okButton;

            if ( _okButton != null )
            {
                DRT.MoveMouse(_okButton, 0.5, 0.5);
                DRT.ResumeAt(new DrtTest(MouseDownOnElement));
            }
        }

        private void MouseDownOnElement()
        {
            DRT.MouseButtonDown();
            DRT.ResumeAt(new DrtTest(MouseUpOnElement));
        }

        private void MouseUpOnElement()
        {
            DRT.MouseButtonUp();
        }

        private void VerifyOKButtonClicked()
        {
            if ( _isOKButtonClicked == false )
            {
                DRT.AssertEqual(true, _isOKButtonClicked, "VerifyOKButtonClicked: Clear Button hasn't been clicked");
            }
            else
            {
                DRT.AssertEqual(false, StickyNoteProxy.IsExpanded, "VerifyOKButtonClicked: ClickOKButton: IsExpanded should be false!");
            }
        }

        //
        // test for clicking the icon.
        //
        private void ClickIconThumb()
        {
            DRT.AssertEqual(false, StickyNoteProxy.IsExpanded, "ClickIconThumb: IsExpanded should be false!");

            UIElement target = StickyNote.Template.FindName(DrtStickyNoteControl.c_IconId, StickyNote) as Button;

            if (target != null)
            {
                // Because the icon thumb is collapsed until the mouse moves over it
                // we need to move relative to the StickyNoteControl.  We need to move                
                // over the anchor to get the icon to change to a button (hence the negative
                // y value since the small icon is placed below the text).
                DRT.MoveMouse(target, 0.25, 0.25);
                DRT.ResumeAt(new DrtTest(MouseDownOnElement));
            }
        }

        private void VerifyIconThumbClicked()
        {
            DRT.AssertEqual(true, StickyNoteProxy.IsExpanded, "VerifyIconThumbClicked: IsExpanded should be true!");
        }


        //
        // test for dragging the title thumb.
        //
        private void DragTitleThumb()
        {
            UIElement target = _titleThumb;

            if ( _titleThumb != null )
            {
                DRT.MoveMouse(_titleThumb, 0.5, 0.5);
                DRT.ResumeAt(new DrtTest(MouseButtonDownOnTitle));
            }
        }

        private void MouseButtonDownOnTitle()
        {
            DRT.MouseButtonDown();
            DRT.ResumeAt(new DrtTest(MouseMoveOnTitle));
        }

        private void MouseMoveOnTitle()
        {
            DRT.MoveMouse((UIElement)DRT.RootElement, 0.5, 0.5);
            DRT.ResumeAt(new DrtTest(MouseButtonUpOnTitle));
        }

        private void MouseButtonUpOnTitle()
        {
            DRT.MouseButtonUp();
        }
        
        private void VerifyTitleThumbDragged()
        {
            double top = (double)StickyNote.GetValue(Canvas.TopProperty);
            double left = (double)StickyNote.GetValue(Canvas.LeftProperty);
            double zeroLen = 0;

            DRT.Assert(zeroLen != top, "VerifyTitleThumbDragged: Top should not be 0!");
            DRT.Assert(zeroLen != left, "VerifyTitleThumbDragged: Top should not be 0!");
        }

        //
        // test for dragging the resize thumb.
        //
        private void DragResizeThumb()
        {
            UIElement target = StickyNote.Template.FindName(DrtStickyNoteControl.c_ResizeCornerId, StickyNote) as Thumb;

            if (target != null)
            {
                _oldWidth = StickyNote.Width;
                _oldHeigth = StickyNote.Height;
                DRT.MoveMouse(target, 0.5, 0.5);
                DRT.ResumeAt(new DrtTest(MouseButtonDownOnResize));
            }
        }
        
        private void MouseButtonDownOnResize()
        {
            DRT.MouseButtonDown();
            DRT.ResumeAt(new DrtTest(MouseMoveOnResize));
        }

        private void MouseMoveOnResize()
        {
                DRT.MoveMouse(StickyNote, 0.5, 0.5);
            DRT.ResumeAt(new DrtTest(MouseButtonUpOnResize));
        }

        private void MouseButtonUpOnResize()
        {
            DRT.MouseButtonUp();
        }

        private void VerifyResizeThumbDragged()
        {
            DRT.Assert(_oldWidth != StickyNote.Width, "VerifyResizeThumbDragged: Resize failed!");
            DRT.Assert(_oldHeigth != StickyNote.Height, "VerifyResizeThumbDragged: Resize failed!");
        }


        #endregion Tests

        #region Event Handlers
        // A class handler for ButtonBase.Click and MenuItem.Click events
        //      e       -   the event sender
        //      args    -   Event argument
        private void OnOKButtonClicked(object e, RoutedEventArgs args)
        {
            _isOKButtonClicked = true;
        }

        private void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            _isTextChanged = true;
        }

        // This method is called by the class _OnDragDelte handler.
        // It responds to the DragDelte events raised by the various thumb controls.
        //      args    -   The event arguments containing additional information about the event
        private void OnDragDelta(object sender, DragDeltaEventArgs args)
        {
            double xNew = Canvas.GetLeft(StickyNote) + (float)args.HorizontalChange;
            double yNew = Canvas.GetTop(StickyNote) + (float)args.VerticalChange;

            Canvas.SetLeft(StickyNote, xNew);
            Canvas.SetTop(StickyNote, yNew);
        }

        #endregion Event Handlers

    }
}

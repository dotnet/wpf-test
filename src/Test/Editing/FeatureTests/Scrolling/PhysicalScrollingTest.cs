// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Basic testing for textbox

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;    
    using System.Diagnostics;
    using System.Threading;  
  
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;            
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;    
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    /// <summary>
    /// Test if scrolling works. The test types a string that is long enough to scroll the box
    /// (TextBox, RichTextBox or PasswordBox)
    /// *** This case currently passes only in Classic theme ***
    /// </summary>
    [Test(0, "TextBoxBase", "PhysicalScrollingTest1", MethodParameters = "/TestCaseType:PhysicalScrollingTest /BoxType:TextBox")]
    [Test(0, "TextBoxBase", "PhysicalScrollingTest2", MethodParameters = "/TestCaseType:PhysicalScrollingTest /BoxType:RichTextBox")]
    [TestOwner("Microsoft"), TestTactics("492,493"), TestTitle("PhysicalScrollingTest"),
     TestArgument("BoxType", "Specify whether TextBox, RichTextBox or PasswordBox to be tested")]
    public class PhysicalScrollingTest : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            string boxType;
            UIElement element;
            StackPanel stackPanel;

            boxType = ConfigurationSettings.Current.GetArgument("BoxType");

            switch (boxType.ToLower(System.Globalization.CultureInfo.InvariantCulture))
            {
                case "textbox":
                    element = new TextBox();
                    break;
                case "richtextbox":
                    element = new RichTextBox();
                    break;
                case "passwordbox":
                    element = new PasswordBox();

                    // needed for PasswordBox testing
                    _referenceTextBox = new TextBox();
                    break;
                default:
                    throw new Exception("Unknown element type in argument: [" + boxType + "]");
            }
            
            element.SetValue(Control.FontFamilyProperty, new FontFamily("Courier New"));
            element.SetValue(Control.FontSizeProperty, 20.0);

            Log("Padding of the control: " + element.GetValue(Control.PaddingProperty));

            stackPanel = new StackPanel();
            stackPanel.Children.Add(element);
            if (boxType == "PasswordBox")
            {
                stackPanel.Children.Add(_referenceTextBox);
            }

            _elementWrapper = new UIElementWrapper(element);

            MainWindow.Content = stackPanel;
            MainWindow.Top = 32;

            QueueDelegate(TestHorizontalScrolling);
        }

        private void DuplicatePasswordBoxSettings(PasswordBox passwordBox,
            TextBox referenceTextBox,
            int numberOfChar)
        {
            string s;

            referenceTextBox.Height = passwordBox.Height;
            referenceTextBox.Width = passwordBox.Width;
            referenceTextBox.FontFamily = passwordBox.FontFamily;
            referenceTextBox.FontStyle = passwordBox.FontStyle;
            referenceTextBox.FontStretch = passwordBox.FontStretch;
            referenceTextBox.FontSize = passwordBox.FontSize;
            referenceTextBox.FontStyle = passwordBox.FontStyle;
            referenceTextBox.Padding = passwordBox.Padding;
            referenceTextBox.HorizontalAlignment = passwordBox.HorizontalAlignment;
            referenceTextBox.VerticalAlignment = passwordBox.VerticalAlignment;
            referenceTextBox.FlowDirection = passwordBox.FlowDirection;
            referenceTextBox.Margin = passwordBox.Margin;

            s = String.Empty;

            for (int i = 0; i < numberOfChar; i++)
            {
                s += passwordBox.PasswordChar;
            }

            referenceTextBox.Text = s;
        }

        private void TestHorizontalScrolling()
        {
            _elementWrapper.Element.SetValue(Control.WidthProperty, 400.0);

            if (_elementWrapper.Element is TextBox
                || _elementWrapper.Element is RichTextBox)
            {
                _elementWrapper.Element.SetValue(TextBox.TextWrappingProperty, TextWrapping.NoWrap);
            }
            else if (_elementWrapper.Element is PasswordBox)
            {
                DuplicatePasswordBoxSettings((PasswordBox)_elementWrapper.Element,
                    _referenceTextBox,
                    s_testString.Length);
            }

            MouseInput.MouseClick(_elementWrapper.Element);
            QueueDelegate(TypeKeyboardForHorizontalTest);
        }

        /// <summary>
        /// This function first sets the focus on the tested box, type a string (_testString)
        /// that is long enough to scroll a 200 pixel textbox (now the box should have scrolled
        /// and click at the left boundary where text is displayed in the textbox
        /// </summary>
        private void TypeKeyboardForHorizontalTest()
        {
            Log("Typing string to scroll horizontally...");

            // after the click point is calculated, we can do the typing
            KeyboardInput.TypeString(s_testString);
            
            QueueDelegate(ClickOnControlStartForHorizontal);
        }

        private void ClickOnControlStartForHorizontal()
        {
            ClickOnControlStart();
            QueueDelegate(VerifyResultForHorizontalScrolling);
        }

        /// <summary>
        /// This function verifies if scrolling does happen by comparing the
        /// start of the scrolled content to the expected string
        /// It also stores the HorizontalOffset value to be validated in the next step
        /// </summary>
        private void VerifyResultForHorizontalScrolling()
        {
            string expectedString;
            string message;

            Log("Verifying that control scrolled...");

            expectedString = s_testString.Substring(0, 6);

            _stringScrolledOffFromView = _elementWrapper.GetTextOutsideSelection(LogicalDirection.Backward);

            message = String.Format("String on caret left = {0}, expected string starts with {1}",
                _stringScrolledOffFromView,
                expectedString);

            Logger.Current.Log(message);

            // we can't exactly tell how many characters are scrolled
            // off from the view since this will be different
            // when different font and control settings
            if ((_elementWrapper.Element is RichTextBox) == false)
            {
                Verifier.Verify(_stringScrolledOffFromView.StartsWith(expectedString),
                    message);
            }
            
            _scrollingOffset = ((TextBoxBase)_elementWrapper.Element).HorizontalOffset;

            // The idea here is first to cache the string that scrolled out of view
            // in the the variable _stringScrolledOffFromView in the previous scrolling operation
            // Then the TextBox / RichTextBox (if it is testing PasswordBox the reference TextBox)
            // is cleared, and _stringScrolledOffFromView will be reinserted into the TextBox
            // The reason for this is to know the length of the characters that are scrolled
            // out of the view. This value is used to compare against the cached _scrollingOffset.
            // However, the comparison is not exact. It is because a chracter can be halfway scrolled
            // off the view, and thus a tolerance is applied. The tolerance is given to be around the width
            // of one character
            Logger.Current.Log("HorizontalOffset for typed content: " + _scrollingOffset);
            // if we are testing PasswordBox, we need to set the reference textbox to have the
            // same number of PasswordChar characters as _stringScrolledOffFromView
            // The reason for this is that we need to find out the length of that PasswordChar string
            // on a TextBox so that we can estimate the length of text that is scrolled out of view
            SetTextBoxToHaveText(_elementWrapper, _referenceTextBox, _stringScrolledOffFromView);

            // Now move on to verify HorizontalOffset property
            QueueDelegate(VerifyResultForHorizontalOffsetProperty);
        }

        private void VerifyResultForHorizontalOffsetProperty()
        {
            Rect rectFirstCharacter;
            Rect rectLastCharacter;
            double lengthOfText;
            double delta;
            double tolerance;

            // retrieve the first and last rect in the TextBox / RichTextBox or PasswordBox
            // (actually the referenceTextBox in this case)
            GetFirstAndLastRectOfTextBox(_elementWrapper,
                _referenceTextBox,
                out rectFirstCharacter,
                out rectLastCharacter);

            // get the display length of all the characters scrolled off the view
            lengthOfText = Math.Abs(rectLastCharacter.X - rectFirstCharacter.X)
                + rectLastCharacter.Width;

            // we need to compare _scrollingOffset with lengthOfText.
            // However they are notexactly equal (consider: a character which is halfway scrolled off the view)
            // so there's an error tolerance given to the delta.
            delta = Math.Abs(lengthOfText - _scrollingOffset);

            // tolerance is around average width of one character in the string scrolled out of view.
            tolerance = _elementWrapper.GetGlobalCharacterRectOfLastCharacter().Width;

            tolerance += Microsoft.Test.Display.Monitor.Dpi.x==96? (0):(tolerance*2.5);

            Logger.Current.Log("Distance between first and last characters: " + lengthOfText);
            Logger.Current.Log("Delta: " + delta);
            Logger.Current.Log("Tolerance allowed (one average character): " + tolerance);

            Verifier.Verify(delta <= tolerance, "Delta is below tolerance.");

            QueueDelegate(PrepareTextBoxForVerticalScrollingTest);
        }

        private void PrepareTextBoxForVerticalScrollingTest()
        {
            _elementWrapper.Clear();

            if (_elementWrapper.Element is PasswordBox)
            {
                _referenceTextBox.Clear();
                DuplicatePasswordBoxSettings((PasswordBox)_elementWrapper.Element,
                    _referenceTextBox,
                    0);
            }

            _elementWrapper.Element.SetValue(Control.WidthProperty, 100.0);
            _elementWrapper.Element.SetValue(Control.HeightProperty, 50.0);
            _elementWrapper.Element.SetValue(TextBox.TextWrappingProperty, TextWrapping.Wrap);

            QueueDelegate(TypeKeyboardForVerticalScrollingTest);
        }

        private void TypeKeyboardForVerticalScrollingTest()
        {
            Rect rect;

            // retrueve the upper left location to position
            // caret in an empty TextBox
            rect = GetLocationToPositionAtUpperLeft(_elementWrapper,
                _referenceTextBox);

            // type the tested string
            KeyboardInput.TypeString(s_testString);

            QueueDelegate(ClickOnStartForVertical);
        }

        private void ClickOnStartForVertical()
        {
            ClickOnControlStart();
            QueueDelegate(VerifyResultForVerticalScrolling);
        }

        private void VerifyResultForVerticalScrolling()
        {
            string expectedString;
            string message;

            message = String.Empty;
            expectedString = String.Empty;
            expectedString = s_testString.Substring(0, 6);

            // re-use _scrollingOffset here. From now on the value in _scrollingOffset is the
            // vertical scrolling offset            
            _scrollingOffset = ((TextBoxBase)_elementWrapper.Element).VerticalOffset;

            Logger.Current.Log("VerticalOffset: " + _scrollingOffset.ToString());

            // re-use _stringScrolledOffFromView here
            _stringScrolledOffFromView = _elementWrapper.GetTextOutsideSelection(LogicalDirection.Backward);

            message = String.Format("String on caret left = {0}, expected string starts with {1}",
                _stringScrolledOffFromView,
                expectedString);

            // we can't exactly tell how many characters are scrolled
            // off from the view since this will be different
            // when different font and control settings
            Verifier.Verify(_stringScrolledOffFromView.StartsWith(expectedString),
                message);

            // set the tested TextBox to be big enough so that vertical scrolling
            // doesn't happen when the text is set back
            _elementWrapper.Element.SetValue(Control.HeightProperty, 1000.0);

            // assign the textbox (or referenceTextBox in the case of PasswordBox
            // to have the text that was scrolled out of the view
            // the reason for this is that we need to calculate line information
            // of the text to verify if VerticalOffset is correct
            SetTextBoxToHaveText(_elementWrapper,
                _referenceTextBox,
                _stringScrolledOffFromView);

            QueueDelegate(VerifyResultForVerticalOffsetProperty);
        }

        /// <summary>
        /// This function compares the height of the all the lines
        /// in the textbox and the cached VerticalOffset value
        /// </summary>
        private void VerifyResultForVerticalOffsetProperty()
        {
            Rect rectFirstCharacter;
            Rect rectLastCharacter;
            double heightOfAllLines;
            double delta;
            double tolerance;
            int numberOfLines;

            numberOfLines = GetNumberOfLinesInTextBoxOrRichTextBox(_elementWrapper,
                _referenceTextBox);

            // retrieve the first and last rect in the TextBox / RichTextBox or PasswordBox
            // (actually the referenceTextBox in this case)
            GetFirstAndLastRectOfTextBox(_elementWrapper,
                _referenceTextBox,
                out rectFirstCharacter,
                out rectLastCharacter);

            heightOfAllLines = Math.Abs(rectLastCharacter.Height * numberOfLines);

            delta = Math.Abs(heightOfAllLines - _scrollingOffset - rectLastCharacter.Height);

            // tolerance is equal to the average height of the lines
            tolerance = heightOfAllLines / numberOfLines;
            tolerance += tolerance / 6;
            Logger.Current.Log("NumberOfLines: " + numberOfLines.ToString());
            Logger.Current.Log("heightOfAllLines: " + heightOfAllLines.ToString());
            Logger.Current.Log("Delta: " + delta.ToString());
            Logger.Current.Log("Tolerance allowed: " + tolerance.ToString());

            Verifier.Verify(delta <= tolerance, "VerticalOffset not correct.");

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        private static int GetNumberOfLinesInTextBoxOrRichTextBox(UIElementWrapper wrapper,
            TextBox referenceTextBox)
        {
            int numberOfLines;
            TextPointer start;

            numberOfLines = 0;

            if (wrapper.Element is PasswordBox)
            {
                numberOfLines = GetNumberOfLinesInTextBox(referenceTextBox);
            }
            else if (wrapper.Element is TextBox)
            {
                numberOfLines = GetNumberOfLinesInTextBox((TextBox)wrapper.Element);
            }
            else if (wrapper.Element is RichTextBox)
            {
                if (((RichTextBox)(wrapper.Element)).Document.ContentStart.CompareTo(((RichTextBox)(wrapper.Element)).Document.ContentEnd) != 0)
                {
                    start = ((RichTextBox)(wrapper.Element)).Document.ContentStart;

                    start = start.GetNextInsertionPosition(LogicalDirection.Forward); // move to valid insertion position.
                    //numberOfLines = start.MoveToLineBoundary(Int32.MaxValue);
                    start = start.GetLineStartPosition(Int32.MaxValue, out numberOfLines);
                    numberOfLines++; /* account for the first line */
                }
            }

            return numberOfLines;
        }

        private static int GetNumberOfLinesInTextBox(TextBox textBox)
        {
            string text;
            string subText;
            int numberOfLines;

            text = textBox.Text;
            numberOfLines = 0;

            // if text == null, we have a bug
            // if there's content in the textbox.
            while (text != String.Empty)
            {
                subText = textBox.GetLineText(numberOfLines++);

                // If this assert fails, there's a bug in GetLineText
                // which returns more text than what textBox.Text returns;
                Debug.Assert(text.Length >= subText.Length);

                text = text.Substring(subText.Length);
            }

            return numberOfLines;
        }

        private static Rect GetLocationToPositionAtUpperLeft(UIElementWrapper wrapper,
            TextBox referenceTextBox)
        {
            Rect location;

            // retrieve referenceCharacterRect
            // for TextBox and RichTextBox this is the position where it shows the first character
            // PasswordBox test needs to retrieve this info from reference TextBox (a plain textbox)
            // We need this info because we want to know where to click on the TextBox after
            // we type and scroll the box.
            if (wrapper.Element is PasswordBox)
            {
                location = referenceTextBox.GetRectFromCharacterIndex(0);
                OffsetRect(ElementUtils.GetScreenRelativeRect(wrapper.Element),
                    ref location);
            }
            else
            {
                location = wrapper.GetGlobalCharacterRect(0);
            }

            return location;
        }

        private static void ClickAtTheCenterOfRect(Rect location)
        {
            MouseInput.MouseClick((int)(location.X + location.Width / 2),
                (int)(location.Y + location.Height / 2));
        }

        private static void GetFirstAndLastRectOfTextBox(UIElementWrapper wrapper,
            TextBox referenceTextBox,
            out Rect firstRect,
            out Rect lastRect)
        {
            UIElementWrapper wrapperForReferenceTextBox;
            UIElementWrapper wrapperTemp;

            // if we are testing PasswordBox, the character rects are retrieved from the reference TextBox
            if (wrapper.Element is PasswordBox)
            {
                // create a UIElementWrapper for reference TextBox
                wrapperForReferenceTextBox = new UIElementWrapper(referenceTextBox);
                wrapperTemp = wrapperForReferenceTextBox;
            }
            // otherwise we can retrieve that info from the tested TextBox / RichTextBox directly.
            else
            {
                wrapperTemp = wrapper;
            }

            // get the rect for the first character
            firstRect = wrapperTemp.GetGlobalCharacterRect(0);

            // get the rect for the last character
            lastRect = wrapperTemp.GetGlobalCharacterRectOfLastCharacter();
        }

        /// <summary>
        /// This function sets either the tested textBox in the case
        /// of TextBox / RichTextBox, or the reference TextBox if we are
        /// testing PasswordBox to contain the string specified in the str
        /// argument
        /// </summary>
        /// <param name="wrapper"></param>
        /// <param name="referenceTextBox"></param>
        /// <param name="str"></param>
        private static void SetTextBoxToHaveText(UIElementWrapper wrapper,
            TextBox referenceTextBox,
            string str)
        {
            int length;
            // if we are testing PasswordBox, we need to set the reference textbox to have the
            // same number of PasswordChar characters as _stringScrolledOffFromView
            // The reason for this is that we need to find out the length of that PasswordChar string
            // on a TextBox so that we can estimate the length of text that is scrolled out of view
            if (wrapper.Element is PasswordBox)
            {
                referenceTextBox.Clear();
                length = str.Length;
                str = String.Empty;

                for (int i = 0; i < length; i++)
                {
                    str += ((PasswordBox)(wrapper.Element)).PasswordChar;
                }
                referenceTextBox.Text = str;
            }
            // otherwise, we can just re-use the TextBox / RichTextBox
            else
            {
                wrapper.Clear();
                wrapper.Text = str;
            }
        }

        /// <summary>
        /// Offset the rect with the value of offsetRect
        /// </summary>
        /// <param name="offsetRect"></param>
        /// <param name="rect"></param>
        private static void OffsetRect(Rect offsetRect, ref Rect rect)
        {
            rect.X += offsetRect.X;
            rect.Y += offsetRect.Y;
        }

        private void ClickOnControlStart()
        {
            Rect controlRect;

            Log("Clicking on first visible character of control...");
            controlRect = ElementUtils.GetScreenRelativeRect(_elementWrapper.Element);
            //for textbox we want just half char
            int width = (_elementWrapper.Element is TextBox) ? -8 : 8;
            MouseInput.MouseClick((int)controlRect.Left + 
                //16 corresponds to one char + half char
                (int)(16) +width+
               // (int)((Thickness)_elementWrapper.Element.GetValue(Control.BorderThicknessProperty)).Left+
                //To account for different padding in different themes
                (int)((Thickness)_elementWrapper.Element.GetValue(Control.PaddingProperty)).Left,
                (int)(controlRect.Top + controlRect.Bottom) / 2);
        }

        #region Private members

        private UIElementWrapper _elementWrapper = null;
        private TextBox _referenceTextBox = null;
        private string _stringScrolledOffFromView;
        private double _scrollingOffset;

        // test string should be long enough to scroll a 400 pixel textbox
        private static readonly string s_testString = "abcdefghijklmnopqrstuvwxyz0123456789~!@#$&*()X";

        #endregion
    }
}

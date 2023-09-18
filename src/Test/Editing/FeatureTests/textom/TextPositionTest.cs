// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit and functional testing for the TextPointer class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/textom/TextPositionTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using System.Windows.Threading;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Imaging;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the Background and Foreground properties are
    /// inherited correctly.
    /// </summary>
    [Test(1, "TextOM", "TextPointerRegression", MethodParameters = "/TestCaseType=TextPointerRegression")]
    [TestOwner("Microsoft"), TestTactics("555"), TestBugs("733,597,734, 599, 598"),TestLastUpdatedOn("Jan 25, 2007")]
    public class TextPointerRegression: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            ActionItemWrapper.SetMainXaml(
                "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                "<RichTextBox Name='TB1' Width='100px' FontSize='48px'><FlowDocument>" +
                "<Paragraph Background='Green' Foreground='White' FontFamily='Tahoma'>" +
                "a<Span FontWeight='Bold'>b</Span>c</Paragraph>" +
                "</FlowDocument></RichTextBox>" +
                "<RichTextBox Name='TB2' Width='100px' FontSize='48px'><FlowDocument>" +
                "<Paragraph Background='Green' Foreground='White' FontFamily='Tahoma'>" +
                "a<Span Background='Blue' Foreground='Red' FontWeight='Bold' FontFamily='Arial'>b</Span>c</Paragraph>" +
                "</FlowDocument></RichTextBox>" +
                "</DockPanel>");
            QueueDelegate(CheckProperties);
        }

        private void CheckProperties()
        {
            CheckProperties("TB1", null, (SolidColorBrush)Brushes.White, "Tahoma");
            CheckProperties("TB2", (SolidColorBrush) Brushes.Blue, (SolidColorBrush)Brushes.Red, "Arial");
            RegressionFor_Regression_Bug599();
            QueueDelegate(RegressionFor_Regression_Bug598);
        }

        /// <summary>Regression_Bug599 -  Calling new Run("Run", RichTextBox.Document.ContentStart) insert two Runs in the document.</summary>
        void RegressionFor_Regression_Bug599()
        {
            Paragraph para;
            RichTextBox richTextBox;
            int initialRuns, finalRuns; 
            richTextBox= new RichTextBox();
            para=richTextBox.Document.Blocks.FirstBlock as Paragraph; 
            initialRuns = para.Inlines.Count; 
            new Run("Run", richTextBox.Document.ContentStart);
            finalRuns = para.Inlines.Count;
            Verifier.Verify(finalRuns == initialRuns + 1, "Run count is not increased by 1, initial[" + initialRuns + "], final[" + finalRuns + "]");
        }

        /// <summary>Regression_Bug598 - Must perfrom undo more many times to restore the previous state if the TextContainer is modifyed by new Bold(new Run("run"), TextPointer) </summary>
        void RegressionFor_Regression_Bug598()
        {
            RichTextBox richTextBox;

            richTextBox = new RichTextBox();
            MainWindow.Content = richTextBox;
            QueueDelegate(ActionFor_Regression_Bug900);
        }
        
        void ActionFor_Regression_Bug900()
        {
            Paragraph para;
            int initialRuns, finalRuns;
            RichTextBox richTextBox;
            richTextBox = MainWindow.Content as RichTextBox; 
            para = richTextBox.Document.Blocks.FirstBlock as Paragraph;
            initialRuns = para.Inlines.Count;
            new Bold(new Run("Bold"), richTextBox.Document.ContentStart);
            finalRuns = para.Inlines.Count;
            Verifier.Verify(finalRuns == initialRuns + 1, "Run count is not increased by 1, initial[" + initialRuns + "], BoldCreated[" + finalRuns + "]");
            richTextBox.Undo();
            finalRuns = para.Inlines.Count;
            Verifier.Verify(finalRuns == initialRuns, "Run count is not increased by 1, initial[" + initialRuns + "], UndoBold[" + finalRuns + "]");
            
            //End of the Test Case. More regression can be added here.
            Logger.Current.ReportSuccess();
        }

        private void CheckProperties(string controlId, SolidColorBrush backgroundColor,
            SolidColorBrush foregroundColor, string fontFamilyName)
        {
            RichTextBox textbox;
            TextPointer navigator;
            bool foundInline;
            SolidColorBrush brush;
            FontFamily fontFamily;

            // Get the RichTextBox.
            textbox = (RichTextBox) ElementUtils.FindElement(MainWindow, controlId);
            Verifier.Verify(textbox != null, "RichTextBox found: " + controlId, true);

            Log("Creating a position in the Inline element...");
            navigator = textbox.Document.ContentStart;

            foundInline = false;
            while (!foundInline)
            {
                TextPointerContext nextSymbol;

                //Verifier.Verify(navigator.MoveToNextContextPosition(LogicalDirection.Forward),
                Verifier.Verify( (navigator = navigator.GetNextContextPosition(LogicalDirection.Forward)) != null,
                    "Pointer can move forward...", false);
                nextSymbol = navigator.GetPointerContext(LogicalDirection.Forward);
                if (nextSymbol == TextPointerContext.ElementStart)
                {
                    DependencyObject element;

                    Log("Looking for inline - next symbol is element start.");
                    element = navigator.GetAdjacentElement(LogicalDirection.Forward);
                    if (element == null)
                    {
                        // NOTE: this reproes Regression_Bug597.
                        throw new Exception("Adjacent element is null but was reported as element-start.");
                    }
                    if (typeof(Span).IsAssignableFrom(element.GetType()))
                    {
                        foundInline = true;
                    }
                }
            }

            navigator = navigator.GetNextContextPosition(LogicalDirection.Forward);
            Log("Element type inside element: " + navigator.Parent.GetType());

            // Query for the expected values.
            brush = (SolidColorBrush) navigator.Parent.GetValue(TextElement.ForegroundProperty);
            Verifier.Verify(brush != null, "Foreground value assigned.", true);
            Verifier.Verify(brush.Color == foregroundColor.Color, "Fore value " +
                brush.Color + " is as expected: " + foregroundColor.Color, true);

            brush = (SolidColorBrush) navigator.Parent.GetValue(TextElement.BackgroundProperty);
            if (backgroundColor == null)
            {
                Verifier.Verify(brush == null, "Background does not inherit.", true);
            }
            else
            {
                Verifier.Verify(brush != null, "Background value assigned.", true);
                Log("Background value: " + brush.Color);
                Verifier.Verify(brush.Color == backgroundColor.Color, "Background value " +
                    brush.Color + " is as expected: " + backgroundColor, true);
            }

            fontFamily = (FontFamily) navigator.Parent.GetValue(TextElement.FontFamilyProperty);
            Verifier.Verify(fontFamily != null, "FontFamily value assigned.", true);
            Verifier.Verify(fontFamily.Source == fontFamilyName, "FontFamly name value " +
                fontFamily.Source + " is as expected: " + fontFamilyName, true);
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that GetCharacterRect.
    /// </summary>
    /// <remarks>
    /// The signature for this method is Rect TextPointer.GetCharacterRect(LogicalDirection direction).
    /// Variables for this method include:
    /// - The value of the direction argument.
    /// - Whether there is content after.
    /// - What kind of content there is in that direction.
    /// - What kind of content there is in the opposide direction.
    ///
    /// Verifies:
    /// - Whether an exception is thrown.
    /// - Whether the rectangle is consistent with expectations.
    /// </remarks>
    [Test(2, "TextOM", "TextPointerGetCharacterRect", MethodParameters = "/TestCaseType=TextPointerGetCharacterRect")]
    [TestOwner("Microsoft"), TestBugs("645"), TestTactics("399"),TestLastUpdatedOn("Jan 25, 2007")]
    public class TextPointerGetCharacterRect: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _control = new RichTextBox();

            VerifyInvalidCalls();

            MainWindow.Content = _control;
            SetupLayoutMetrics();
            QueueDelegate(VerifyValidCalls);
        }

        private void VerifyInvalidCalls()
        {
            TextPointer pointer;

            Log("Verifying invalid calls to TextPointer.GetCharacterRect...");

            pointer = _control.Document.ContentStart;
            Verifier.Verify(!pointer.HasValidLayout,
                "TextPointers don't have a valid layout " +
                "before being part of a tree.", true);

            try
            {
                pointer.GetCharacterRect((LogicalDirection)0xF0);
                throw new ApplicationException(
                    "TextPointer.GetCharacterRect accepts invalid LogicalDirection values.");
            }
            catch(ArgumentException)
            {
                Log("TextPointer.GetCharacterRect rejects invalid LogicalDirection values.");
            }

            Rect result = pointer.GetCharacterRect(LogicalDirection.Forward);
            Verifier.Verify(result == Rect.Empty, "Verifying that empty Rect is returned if the pointer doesnt have valid layout", true);            
        }

        private void SetupLayoutMetrics()
        {
            // Hard-code all layout-affecting properties.
            _control.BorderThickness = new Thickness(5);
            _control.FontFamily = new FontFamily("Courier New");
            _control.FontSize = 30;
            _control.FontStyle = FontStyles.Normal;
            _control.FontWeight = FontWeights.Normal;
            _control.Margin = new Thickness(7);
            _control.Padding = new Thickness(11);
        }

        private void VerifyValidCalls()
        {
            Rect rect;
            Rect otherRect;
            TextPointer pointer;

            Log("\r\nVerifying valid calls to TextPointer.GetCharacterRect...\r\n");

            pointer = _control.Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);

            // Verify in forward direction.
            rect = pointer.GetCharacterRect(LogicalDirection.Forward);
            Log("Rectangle of pointer with no content: " + rect);
            Verifier.Verify(rect.Width == 0,
                "Rectangle on pointer with no content has zero width.", true);
            Verifier.Verify(
                rect.Height > _control.FontSize * 0.8 && rect.Height < _control.FontSize * 1.2,
                "Rectangle on pointer with no content has height close to font size.", true);
            Verifier.Verify(rect.Left > 15,
                "Rectangle on pointer with flushed left/right is relative to control, not text view.", true);
            Verifier.Verify(rect.Right > 15,
                "Rectangle on pointer with flushed left/right is relative to control, not text view.", true);
            Verifier.Verify(rect.Left < _control.ActualWidth / 2,
                "Caret is on correct side of document (rtl flips coordinate space).", true);

            // Verify in opposite direction.
            otherRect = pointer.GetCharacterRect(LogicalDirection.Backward);
            Verifier.Verify(rect == otherRect,
                "On empty content, logical direction makes no different.", true);

            if (_control.FlowDirection == FlowDirection.LeftToRight)
            {
                Log("Changing direction to right-to-left then top-to-bottom...");
                _control.FlowDirection = FlowDirection.RightToLeft;
                QueueDelegate(VerifyValidCalls);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Main flow.

        #region Helper methods.

        private RichTextBox _control;

        #endregion Helper methods.
    }

    /// <summary>
    /// Verifies the following API's of TextPointer
    /// a. DocumentStart
    /// b. DocumentEnd
    /// c. IsInSameDocument
    /// </summary>
    [Test(0, "TextOM", "TextPointerAPITest", MethodParameters = "/TestCaseType=TextPointerAPITest")]
    [TestOwner("Microsoft"), TestTactics("398"), TestBugs(""),  TestLastUpdatedOn("Jan 25, 2007")]
    public class TextPointerAPITest : CustomTestCase
    {
        #region Main flow
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 400;
            _rtb.Width = 400;
            _rtb.FontSize = 24;
            ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run("This is a RichTextBox"));
            _rtbWrapper = new UIElementWrapper(_rtb);
            StackPanel panel = new StackPanel();
            panel.Children.Add(_rtb);

            _textBlock = new TextBlock();
            _textBlockWrapper = new UIElementWrapper(_textBlock);
            _textBlock.Text = "This is a TextBlock";
            panel.Children.Add(_textBlock);

            _textFlow = new FlowDocumentScrollViewer();
            _textFlow.Document = new FlowDocument();
            _textFlowWrapper = new UIElementWrapper(_textFlow);
            _textFlowWrapper.XamlText = "<Paragraph><Span>This is a Inline inside Paragraph</Span></Paragraph>" +
                "<Paragraph><Bold>This is Bold inside a Paragraph</Bold></Paragraph>";
            panel.Children.Add(_textFlow);

            MainWindow.Content = panel;

            QueueDelegate(TestDocumentStart);
        }

        private void TestDocumentStart()
        {
            VerifyDocumentStart(_rtbWrapper);
            VerifyDocumentStart(_textBlockWrapper);
            VerifyDocumentStart(_textFlowWrapper);
            TestDocumentEnd();
        }

        private void TestDocumentEnd()
        {
            VerifyDocumentEnd(_rtbWrapper);
            VerifyDocumentEnd(_textBlockWrapper);
            VerifyDocumentEnd(_textFlowWrapper);
            TestIsInSameDocument();
        }

        private void TestIsInSameDocument()
        {
            VerifyIsInSameDocument(_rtbWrapper);
            VerifyIsInSameDocument(_textBlockWrapper);
            VerifyIsInSameDocument(_textFlowWrapper);

            Log("Calling TextPointer.ToString() :" + _rtbWrapper.Start.ToString());
            Logger.Current.ReportSuccess();
        }
        #endregion Main flow

        #region Helper methods.
        private void VerifyDocumentStart(UIElementWrapper wrapper)
        {
            TextPointer documentStart;

            TextPointer tp = wrapper.Start;
            tp = tp.GetPositionAtOffset(wrapper.Start.GetOffsetToPosition(wrapper.End) / 2);

            Log("Verifying DocumentStart on " + wrapper.Element.GetType().ToString());

            documentStart = tp.DocumentStart;
            Verifier.Verify(documentStart.GetOffsetToPosition(wrapper.Start) == 0,
                "Verifying that DocumentStart is pointing to same position as ContentStart -1", true);            

            documentStart = wrapper.Start.DocumentStart;
            Verifier.Verify(documentStart.GetOffsetToPosition(wrapper.Start) == 0,
                "Verifying that DocumentStart is pointing to same position as ContentStart -2", true);

            documentStart = wrapper.End.DocumentStart;
            Verifier.Verify(documentStart.GetOffsetToPosition(wrapper.Start) == 0,
                "Verifying that DocumentStart is pointing to same position as ContentStart -3", true);
        }

        private void VerifyDocumentEnd(UIElementWrapper wrapper)
        {
            TextPointer documentEnd;

            TextPointer tp = wrapper.Start;
            tp = tp.GetPositionAtOffset(wrapper.Start.GetOffsetToPosition(wrapper.End) / 2);

            documentEnd = tp.DocumentEnd;
            Verifier.Verify(documentEnd.GetOffsetToPosition(wrapper.End) == 0,
                "Verifying that DocumentEnd is pointing to same position as ContentEnd -1", true);            

            documentEnd = wrapper.Start.DocumentEnd;
            Verifier.Verify(documentEnd.GetOffsetToPosition(wrapper.End) == 0,
                "Verifying that DocumentEnd is pointing to same position as ContentEnd -2", true);

            documentEnd = wrapper.End.DocumentEnd;
            Verifier.Verify(documentEnd.GetOffsetToPosition(wrapper.End) == 0,
                "Verifying that DocumentEnd is pointing to same position as ContentEnd -3", true);
        }

        private void VerifyIsInSameDocument(UIElementWrapper wrapper)
        {
            TextPointer tp = wrapper.Start;
            tp = tp.GetPositionAtOffset(wrapper.Start.GetOffsetToPosition(wrapper.End) / 2);

            Verifier.Verify(tp.IsInSameDocument(wrapper.Start),
                "Verifying IsInSameDocument method with Start pointer", true);

            Verifier.Verify(tp.IsInSameDocument(wrapper.End),
                "Verifying IsInSameDocument method with End pointer", true);

            Verifier.Verify(tp.IsInSameDocument(tp),
                "Verifying IsInSameDocument method with the same pointer", true);

            try
            {
                tp.IsInSameDocument(null);
                throw new ApplicationException("IsInSameDocument accepts null as its parameter");
            }
            catch (ArgumentNullException)
            {
                Log("ArgumentNullException is thrown as expected");
            }
        }
        #endregion Helper methods.

        #region Members
        private RichTextBox _rtb;
        private TextBlock _textBlock;
        private FlowDocumentScrollViewer _textFlow;
        private UIElementWrapper _rtbWrapper,_textBlockWrapper,_textFlowWrapper;
        #endregion Members
    }    

    /// <summary>
    /// Verifies that isAtLineStart works correctly for all positions.
    /// </summary>
    [Test(2, "TextOM", "TextPointerIsAtLineStart", MethodParameters = "/TestCaseType=TextPointerIsAtLineStart")]
    [TestOwner("Microsoft"), TestBugs("744,744"), TestTactics("397"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextPointerIsAtLineStart : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            //Skip the long strings
            if (_testStringData.IsLong)
            {
                QueueDelegate(NextCombination);
                return;
            }

            if ((_testStringData.Value != null) && (_testStringData.Value.Length > 1024))
            {
                QueueDelegate(NextCombination);
                return;
            }

            if (_wrapper == null || _wrapper.Element.GetType() != _editableType.Type)
            {
                FrameworkElement control;

                control = _editableType.CreateInstance();
                _wrapper = new UIElementWrapper(control);
                _wrapper.Element.SetValue(TextBox.FontSizeProperty, (double)10);
                TestElement = control;
            }
            if (_wrapper.Element is TextBoxBase)
            {                
                ((TextBoxBase)_wrapper.Element).Width = 200;
            }
            if (_wrapper.Element is TextBox)
            {
                ((TextBox)_wrapper.Element).TextWrapping = _testWrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
                ((TextBox)_wrapper.Element).Text = _testStringData.Value;
            }
            _wrapper.Element.SetValue(TextBox.FontFamilyProperty,
                new FontFamily((_testFixedFont)? "Courier New" : "Arial"));
            QueueDelegate(CheckAfterLayout);
        }        

        private void CheckAfterLayout()
        {
            TextLayoutModel model;
            TextPointer cursor;
            int offset;

            model = new TextLayoutModel(_wrapper);
            cursor = _wrapper.Start;
            cursor = cursor.GetPositionAtOffset(0, LogicalDirection.Forward);
            offset = 0;            
            do
            {
                VerifyIsAtLineStart(cursor, model, offset);
                offset++;
            } while ( (cursor = cursor.GetPositionAtOffset(1)) != null);

            QueueDelegate(NextCombination);
        }

        private void VerifyIsAtLineStart(TextPointer pointer,
            TextLayoutModel model, int unitOffset)
        {
            bool isAtLineStart;
            bool knownAtStart;
            bool knownNotAtStart;
            string failureMessage;
            int knownLineStartIndex;
            int lineIndex;
            TextPointer pointerBackward;

            failureMessage = null;
            knownAtStart = knownNotAtStart = false;
            isAtLineStart = pointer.IsAtLineStartPosition;
            if (pointer.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.None)
            {
                knownAtStart = true;
            }

            knownLineStartIndex = 0;
            if ( (_wrapper.Element is TextBox) && (_testWrap) )
            {
                lineIndex = 0;
                while (knownLineStartIndex <= unitOffset)
                {
                    //Break if we are at the last index of the contents and if the contents are not empty
                    if ((_wrapper.Text.Length != 0) &&
                        (knownLineStartIndex == _wrapper.Text.Length) )
                    {
                        break;
                    }

                    if (knownLineStartIndex == unitOffset)                        
                    {
                        knownAtStart = true;
                        break;
                    }
                    else
                    {
                        if (lineIndex >= ((TextBox)_wrapper.Element).LineCount)
                        {                                                        
                            throw new ApplicationException("lineIndex crossing the TB.LineCount");
                        }
                        knownLineStartIndex += Math.Abs( ((TextBox)_wrapper.Element).GetLineLength(lineIndex) );                        
                    }
                    lineIndex++;
                }
            }

            if (knownAtStart && !isAtLineStart)
            {
                failureMessage = "Pointer at offset " + unitOffset +
                    " returns false for isAtLineStart, but it should return true.";
            }
            if (knownNotAtStart && isAtLineStart)
            {
                failureMessage = "Pointer at offset " + unitOffset +
                    " returns true for isAtLineStart, but it should return false.";
            }
            if (failureMessage != null)
            {
                TextTreeLogger.LogContainer("atline-failure", pointer,
                    pointer, "Failing Pointer");
                throw new Exception(failureMessage);
            }

            pointerBackward = pointer.GetPositionAtOffset(0, LogicalDirection.Backward);
            if (pointerBackward.GetOffsetToPosition(pointerBackward.DocumentStart) == 0)
            {
                Verifier.Verify(pointerBackward.IsAtLineStartPosition,
                    "Verifying that for a position at start of the document and Backward gravity " +
                    "IsAtLineStart returns true", false);
            }
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _wrapper;

        /// <summary>content used in the test</summary>
        private StringData _testStringData = null;

        /// <summary>Editable type used in the test</summary>
        private TextEditableType _editableType = null;

        /// <summary>Whether to set Wrap property to true</summary>
        private bool _testWrap = false;

        /// <summary>Whether to use a FixedFont</summary>
        private bool _testFixedFont = false;        

        #endregion Private fields.
    }

    /// <summary>
    /// Test TextPointer.DeleteTextInRun(int) methods.
    /// we will test: 
    ///     1. When pointer is not at Insertion Position, throw an exception.
    ///     2. negative parameter
    ///     3. Position Parameter
    ///     4. out of range parameter.
    ///     5. Make sure that it won't go out of run boundary.
    /// </summary>
    [Test(2, "TextOM", "DeleteTextInRunTest", MethodParameters = "/TestCaseType=DeleteTextInRunTest")]
    [TestOwner("Microsoft"), TestWorkItem("50"),TestBugs(""), TestTactics("396"), TestLastUpdatedOn("Jan 25, 2007")]
    public class DeleteTextInRunTest : ManagedCombinatorialTestCase
    {
        /// <summary>Start a combination </summary>
        protected override void DoRunCombination()
        {
            //We don't have to use a RichTextBox here.
            //use it so that we can see the text rendering.
            RichTextBox box; 
            box = new RichTextBox();
            TestElement = box;
            _wrapper = new UIElementWrapper(box);
            
            //We use two runs in a paragraph so that we can make sure it won't work cross Run boundary.
            Paragraph para= new Paragraph();
            Run r = new Run("ab");
            r.FontSize = 15;
            para.Inlines.Add(r);
            r = new Run("cd");
            r.FontSize = 10;
            para.Inlines.Add(r);
            box.Document.Blocks.Clear();
            box.Document.Blocks.Add(para);

            IList list = box.Document.Blocks as IList;

            //ignore the unreachable cases.
            if (_pointerOffset > _wrapper.Start.GetOffsetToPosition(_wrapper.End))
            {
                QueueDelegate(NextCombination);
            }
            _pointer = _wrapper.Start.GetPositionAtOffset(_pointerOffset);

            //Get the logicalDirection from the parameter. negative means Backward
            _deleteDirection = _deletingCount < 0 ? LogicalDirection.Backward : LogicalDirection.Forward;
            
            //if the position is not at an insertion positoin, an exception should be thrown.
            _excepton=(_pointer.IsAtInsertionPosition)? false : true;

            //Calculate the text to be deleted.
            if(!_excepton)
            {
                string str = _pointer.GetTextInRun(_deleteDirection);
                _deletedCount = (str.Length < Math.Abs(_deletingCount)) ? str.Length : Math.Abs(_deletingCount);
                _deletedText = (_deleteDirection == LogicalDirection.Forward) ? str.Substring(0, _deletedCount) : str.Substring(str.Length - _deletedCount, _deletedCount);
                
                //GetTextInRun should not cross the run boundary.
                Verifier.Verify(_deletedText.Length <= 2, "We should delete more than three characgters!");
            }

            _orginalText = _wrapper.Text;
            
            //Log status
            Log("Throw Exception[" + _excepton.ToString() + "]");
            Log("All text before perfrom deleting" + _orginalText + "]");
            Log("Expected deleted Text[" + _deletedText + "]");
          
            QueueDelegate(DeletText);
        }
        void DeletText()
        {
            try
            {
                _pointer.DeleteTextInRun(_deletingCount);
            }
            catch
            {
                //verify exceptoin
                Verifier.Verify(_excepton, "No Exception should be thrown!");
            }
            if(!_excepton)
            {
                //Verify deleting text.
                Log("All text after perfrom deleting" + _excepton.ToString() + "]");
                if (_deletedText != string.Empty)
                {
                    Verifier.Verify(!_wrapper.Text.Contains(_deletedText), "Deleted text is still in the Run!");
                }
                else
                {
                    Verifier.Verify(_wrapper.Text==_orginalText, "No textShould be deleted!");
                }
            }
            QueueDelegate(NextCombination);
        }

        int _deletingCount=0;
        int _pointerOffset=0;
        UIElementWrapper _wrapper; 
        int _deletedCount;
        bool _excepton; 
        TextPointer _pointer; 
        LogicalDirection _deleteDirection; 
        string _deletedText;
        string _orginalText;

        /// <summary>
        /// number of chars to be deleted.
        /// </summary>
        public static object[] DeletingValues = { -3, -2, -1, 0, 1, 2, 3 };

        /// <summary>
        /// specify the position at offset from the Document start .
        /// </summary>
        public static object[] OffsetValues ={ 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 }; 
    }

    /// <summary>
    /// Tests InsertLineBreak and InsertParagraphBreak API on TextPointer.
    /// Try's inserting at all positions from start to end in RichTextContentData.FullyPopulatedContent
    /// </summary>
    [Test(2, "TextOM", "InsertLineParagraphBreakTest", MethodParameters = "/TestCaseType=InsertLineParagraphBreakTest")]
    [TestOwner("Microsoft"), TestWorkItem("50"), TestTactics("395"), TestBugs("746, 747, 748, 749, 750"), TestLastUpdatedOn("Jan 25, 2007")]
    public class InsertLineParagraphBreakTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            Logger.Current.Log("Xaml tested: " + s_xaml);
            base.RunTestCase();
        }        

        /// <summary>Start a combination </summary>
        protected override void DoRunCombination()
        {
            s_wrapper = new UIElementWrapper(new RichTextBox());
            TestElement = s_wrapper.Element as FrameworkElement;
            XamlUtils.TextRange_SetXml(s_wrapper.TextRange, s_xaml);

            TextPointer pointer = s_wrapper.Start.GetPositionAtOffset(_pointerOffset);

            //rememer initial Paragraph count
            _originalParagraphCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.Paragraph));

            //rememer initial LineBreak count
            _originalLineBreakCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.LineBreak));

            //remember initial total count
            _originalTotalCount = s_wrapper.Start.GetOffsetToPosition(s_wrapper.End);

            if ((s_wrapper.IsTextPointerInsideTextElement(pointer, typeof(System.Windows.Documents.Hyperlink))) ||
                (s_wrapper.IsTextPointerInsideTextElement(pointer, typeof(System.Windows.Documents.InlineUIContainer))) )
            {
                _isPointerInsideUnbreakableElement = true;
            }
            else
            {
                _isPointerInsideUnbreakableElement = false;
            }

            QueueDelegate(InsertBreak);
        }

        private void InsertBreak()
        {
            try
            {
                TextPointer pointer = s_wrapper.Start.GetPositionAtOffset(_pointerOffset);
                if (_APITest == "InsertParagraphBreak")
                {
                    pointer.InsertParagraphBreak();
                }
                else if (_APITest == "InsertLineBreak")
                {
                    pointer.InsertLineBreak();
                }
               
            }
            catch (Exception)
            {
                _exception = true; 
            }

            if (_APITest == "InsertParagraphBreak")
            {
                if (_pointerOffset == 55)
                {
                    QueueDelegate(NextCombination);
                    return;
                }

                QueueDelegate(VerifyParagraphResult);
            }
            else if (_APITest == "InsertLineBreak")
            {
                QueueDelegate(VerifyLineBreakResult);
            }
            
        }

        private void VerifyParagraphResult()
        {
            int paragraphCount;
            
            if (!_isPointerInsideUnbreakableElement)
            {
                paragraphCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.Paragraph));
                Verifier.Verify(_originalParagraphCount < paragraphCount, 
                    "Paragraph count is wrong! Expected[>" + _originalParagraphCount + "], Actual[" + paragraphCount + "]");
            }
            else
            {
                Verifier.Verify(_exception, "Did not catch an exception!");
            }

            //Perform Undo and verify that state is back to original state
            ((RichTextBox)s_wrapper.Element).Undo();
            paragraphCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.Paragraph));
            Verifier.Verify(_originalParagraphCount == paragraphCount,
                    "Paragraph count is wrong after Undo! Expected[" + _originalParagraphCount + "], Actual[" + paragraphCount + "]");

            QueueDelegate(NextCombination);
        }

        private void VerifyLineBreakResult()
        {
            int lineBreakCount;            

            lineBreakCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.LineBreak));
            Verifier.Verify((_originalLineBreakCount +1) == lineBreakCount,
                    "LineBreak count is wrong! Expected[" + (_originalLineBreakCount + 1) + "], Actual[" + lineBreakCount + "]");

            //Perform Undo and verify that state is back to original state
            ((RichTextBox)s_wrapper.Element).Undo();
            lineBreakCount = ElementCount(s_wrapper.TextRange, typeof(System.Windows.Documents.LineBreak));
            Verifier.Verify(_originalLineBreakCount == lineBreakCount,
                    "LineBreak count is wrong after Undo! Expected[" + _originalLineBreakCount + "], Actual[" + lineBreakCount + "]");

            QueueDelegate(NextCombination);
        }

        private int ElementCount(TextRange range, Type ElementType)
        {
            int count = 0;
            TextPointer pointer = range.Start.GetPositionAtOffset(0);
            if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            {
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            while (pointer.GetOffsetToPosition(range.End) >= 0 && 
                pointer.GetOffsetToPosition(range.End.DocumentEnd) !=0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                    pointer.GetAdjacentElement(LogicalDirection.Forward).GetType() == ElementType)
                {
                    count++;
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return count;
        }

        /// <summary>Create combination.</summary>
        /// <returns></returns>
        public static object[] PointerOffsets()
        {
            object[] result;
            UIElementWrapper wrapper;
            int totalCount;

            wrapper = new UIElementWrapper(new RichTextBox());
            XamlUtils.TextRange_SetXml(wrapper.TextRange, s_xaml);
            
            totalCount = wrapper.Start.GetOffsetToPosition(wrapper.End);            
            result = new object[totalCount];
            for (int i = 0; i < totalCount; i++)
            {
                result[i] = i; 
            }
            return result;
        }

        #region Private fields

        /// <summary>Pointer offset from start which is used to test InsertParagraphBreak and InsertLineBreak</summary>
        private int _pointerOffset=0;

        /// <summary>API (InsertParagraphBreak or InsertLineBreak being tested</summary>
        private string _APITest=string.Empty;

        private static UIElementWrapper s_wrapper;
        private bool _exception;        
        private bool _isPointerInsideUnbreakableElement; 
        private int _originalParagraphCount, _originalLineBreakCount, _originalTotalCount;

        /// <summary>Add more content to the xaml if new scenarios are needed.</summary>
        static string s_xaml = "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            RichTextContentData.FullyPopulatedContent.Xaml +
            "</Section>";

        #endregion Private fields
    }

    /// <summary>Create run at a TextPointer. </summary>
    [Test(2, "TextOM", "InlineCreationAtPosition", MethodParameters = "/TestCaseType=InlineCreationAtPosition", Timeout = 300)]
    [TestOwner("Microsoft"), TestWorkItem("49"), TestTactics("394"), TestBugs("599, 745, 598"), TestLastUpdatedOn("Jan 25, 2007")]
    public class InlineCreationAtPosition : ManagedCombinatorialTestCase
    {
        private string _content=string.Empty;
        private UIElementWrapper _wrapper;
        private int _offset=0;
        private TextPointer _pointer, _insertStart, _insertEnd;
        private int _orginalRunCount;
        private Type _type;
        private TextElement _element; 
        private Inlines _elementType=0; 

        /// <summary>Start the test combination </summary>
        protected override void DoRunCombination()
        {
            Log("DoRunCombination ...");
            //System.Threading.Thread.Sleep(1000);
            _wrapper = new UIElementWrapper(new RichTextBox());
            TestElement = _wrapper.Element as FrameworkElement;
            XamlUtils.TextRange_SetXml(_wrapper.TextRange, s_xaml); 
            _type = ReflectionUtils.FindType("System.Windows.Documents." + _elementType.ToString());
            _orginalRunCount = TextPointerUtils.ElementCount(_wrapper.Start, _wrapper.End, _type); 
            QueueDelegate(InsertInlineElement);
        }

        void InsertInlineElement()
        {
            Log("InsertInlineElement ...");
            
            _pointer = _wrapper.Start.GetPositionAtOffset(_offset);
            
            //Find a range where the the TextElement will be position in.
            if (_pointer != null)
            {
                SetInsertionRange(_pointer);

                //Hyperlink, Floater, Figure are can't be stay in Hyperlink
                if (_wrapper.IsTextPointerInsideTextElement(_pointer, typeof(Hyperlink)) && (_elementType == Inlines.Figure ||
                    _elementType == Inlines.Floater ||
                    _elementType == Inlines.Hyperlink))
                {
                    QueueDelegate(NextCombination);
                    return;
                }
            }
            //Create a TextElement
            CreateElement(_pointer);           

            QueueDelegate(VerifyResult);
        }

        void SetInsertionRange(TextPointer pointer)
        {
            if (pointer.IsAtInsertionPosition)
            {
                TextPointer p = pointer.GetNextInsertionPosition(LogicalDirection.Forward);
                if (p == null || p.CompareTo(pointer) == 0)
                {
                    //At the end of the doucment (line).
                    _insertStart = pointer;
                    _insertEnd = pointer.DocumentEnd.GetPositionAtOffset(0);
                }
                else
                {
                    _insertStart = pointer.GetPositionAtOffset(-1);
                    _insertEnd = pointer.GetPositionAtOffset(1);
                }
            }
            else
            {
                SetInsertionRange( _pointer.GetInsertionPosition(LogicalDirection.Forward));
            }
        }

        void VerifyResult()
        {
            Log("VerifyResult ...");
            int dis1, dis2; 
            int actualRunCount = TextPointerUtils.ElementCount(_wrapper.Start, _wrapper.End, _type);

            ////Add Verification for Regression_Bug599
            if (_offset >= 0)
            {
                Verifier.Verify(actualRunCount > _orginalRunCount, "Wrong Run count in the document! Orignal[" + _orginalRunCount + "], Current[" + actualRunCount + "]");
           
                if (_insertStart == null)
                {
                    _insertStart = _pointer.DocumentStart;
                }
                dis1 = _insertStart.GetOffsetToPosition(_element.ContentStart);
                dis2 = _element.ContentEnd.GetOffsetToPosition(_insertEnd);

                //perfrom Undo
                ((TextBoxBase)_wrapper.Element).Undo();
                
                QueueDelegate(verifyUndo);
            }
            else
            {
                //if offset is -1, the inline is created however, it is not connected to the TextContainer since the TextPointer is null.
                Verifier.Verify(_element != null, "We expected element is created even though it is not hooked up to the TextContainer of RichTextBox!");
                Verifier.Verify(actualRunCount == _orginalRunCount, "Element count won't equal! Orignal[" + _orginalRunCount + "], Current[" + actualRunCount + "]");
                QueueDelegate(NextCombination);
            }
        }
     
        void verifyUndo()
        {
            //enable this following lines after Regression_Bug598 is fixed.
            //int actualRunCount = TextPointerUtils.ElementCount(_wrapper.TextRange, type);
            //Verifier.Verify(actualRunCount == _orginalRunCount, "Wrong Run count in the document after Undo! Orignal[" + _orginalRunCount + "], Current[" + actualRunCount + "]");
            QueueDelegate(NextCombination);
        }

        void CreateElement(TextPointer pointer)
        {
            //Note: Content=null, is valid.

            switch (_elementType)
            {
                case Inlines.Run:
                    if (_content == null)
                    {
                        _element=new Run(null, pointer);
                    }
                    else
                    {
                        _element = new Run(_content, pointer);
                    }
                    break;
                case Inlines.Bold:
                    if (_content == null)
                    {
                        _element = new Bold((Run)null, pointer);
                    }
                    else
                    {
                        _element = new Bold(new Run(_content), pointer);
                    }
                    break;
                case Inlines.Figure:
                    if (_content == null)
                    {
                        _element = new Figure((Paragraph)null, pointer);
                    }
                    else
                    {
                        _element = new Figure(new Paragraph(new Run(_content)), pointer);
                    }
                    break;
                case Inlines.Floater:
                    if (_content == null)
                    {
                        _element = new Floater((Paragraph)null, pointer);
                    }
                    else
                    {
                        _element = new Floater(new Paragraph(new Run(_content)), pointer);
                    }
                    break;
                case Inlines.Hyperlink:
                    if (_content == null)
                    {
                        _element = new Hyperlink((Run)null, pointer);
                    }
                    else
                    {
                        _element = new Hyperlink(new Run(_content), pointer);
                    }
                    break;
                case Inlines.InlineUIContainer:
                    if (_content == null)
                    {
                        _element = new InlineUIContainer((Button)null, pointer);
                    }
                    else
                    {
                        _element = new InlineUIContainer(new Button(), pointer);
                    }
                    break;
                case Inlines.Italic:
                    if (_content == null)
                    {
                        _element = new Italic((Run)null, pointer);
                    }
                    else
                    {
                        _element = new Italic(new Run(_content), pointer);
                    }
                    break;
                case Inlines.Span:
                    if (_content == null)
                    {
                        _element = new Span((Run)null, pointer);
                    }
                    else
                    {
                        _element = new Span(new Run(_content), pointer);
                    }
                    break;
                case Inlines.Underline:
                    if (_content == null)
                    {
                        _element = new Underline((Run)null, pointer);
                    }
                    else
                    {
                        _element = new Underline(new Run(_content), pointer);
                    }
                    break;
            }
        }

        /// <summary>Add more content to the xaml if new scenarios are needed.</summary>
        static string s_xaml = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
                        + "<Table><Table.Columns><TableColumn Width=\"160\" /></Table.Columns><TableRowGroup><TableRow><TableCell BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\"><Paragraph>abc<Hyperlink NavigateUri=\"link.xaml\">lk</Hyperlink></Paragraph></TableCell></TableRow></TableRowGroup></Table>"
                        + "</Section>";

        /// <summary>find all the offsets of TextPositions. </summary>
        /// <returns></returns>
        public static object[] Offsets
        {
            get
            {
                return TextPointerUtils.GetOffsets(s_xaml); 
            }
        }

        /// <summary>content used to create a run.</summary>
        public static object[] Contents
        {
            get
            {
                object[] results = {
                  null,
                  "run",
                };

                return results;
            }
        }

        /// <summary>return a array of textElement enum values </summary>
        public static object[] TextElements
        {
            get
            {
                object[] result = {
                    Inlines.Run,
                    Inlines.InlineUIContainer,
                    Inlines.Figure,
                    Inlines.Floater,
                    Inlines.Span,
                    Inlines.Hyperlink,
                    Inlines.Bold,
                    Inlines.Italic,
                    Inlines.Underline, 
                };
                return result; 
            }
        }
    }

    /// <summary>Define the Inline for test. </summary>
    public enum Inlines
    {
        /// <summary>Run</summary>
        Run,
        /// <summary>InlineUIContainer</summary>
        InlineUIContainer,
        /// <summary>Figure</summary>
        Figure,
        /// <summary>Floater</summary>
        Floater,
        /// <summary>Span</summary>
        Span,
        /// <summary>Hyperlink</summary>
        Hyperlink,
        /// <summary>Bold</summary>
        Bold,
        /// <summary>Italic</summary>
        Italic,
        /// <summary>Underline</summary>
        Underline, 
    }
    /// <summary>Test span create with two Pointers. </summary>
    [Test(1, "TextOM", "SpanCreationInRange", MethodParameters = "/TestCaseType=SpanCreationInRange", Timeout=600)]
    [TestOwner("Microsoft"), TestWorkItem("49"), TestTactics("393"), TestBugs("605, 604"), TestLastUpdatedOn("Jan 25, 2007")]
    public class SpanCreationInRange : ManagedCombinatorialTestCase
    {
        int _startOffset=0;
        int _endOffset=0;
        Inlines _elementType=0;
        UIElementWrapper _wrapper;
        Type _type;
        bool _exception;
        Span _element;
        int _orginalElements;
        string _orginalText;
        TextPointer _p1,_p2; 

        /// <summary> </summary>
        protected override void DoRunCombination()
        {
            Log("DoRunCombination ...");

            _wrapper = new UIElementWrapper(new RichTextBox());
            TestElement = _wrapper.Element as FrameworkElement;
            XamlUtils.TextRange_SetXml(_wrapper.TextRange, s_xaml);
            _type = ReflectionUtils.FindType("System.Windows.Documents." + _elementType.ToString());
            _orginalElements = TextPointerUtils.ElementCount(_wrapper.Start, _wrapper.End, _type);

            QueueDelegate(InsertSpanElement);
        }

        void InsertSpanElement()
        {
           bool bstart,  bend;
           int links; 
              
            _p1 = _wrapper.Start.GetPositionAtOffset(_startOffset);
            _p2 = _wrapper.Start.GetPositionAtOffset(_endOffset);
            
            //TextPointer must be at insertion position.
            if (_p1 != null && !_p1.IsAtInsertionPosition)
            {
                _p1 = _p1.GetInsertionPosition(_p1.LogicalDirection);
            }
            if (_p2 != null && !_p2.IsAtInsertionPosition)
            {
                _p2 = _p2.GetInsertionPosition(_p2.LogicalDirection);
            }

            if (_p1 != null && _p2 != null)
            {
                _orginalText = new TextRange(_p1, _p2).Text;
             
                //Note: if a ragne contains hyperlinks(not partial container), those hyperlinks will be removed at when new Hyperlink is created.
                if (_elementType == Inlines.Hyperlink)
                {
                    bstart = _wrapper.IsTextPointerInsideTextElement(_p1, typeof(Hyperlink));
                    bend = _wrapper.IsTextPointerInsideTextElement(_p2, typeof(Hyperlink));
                    
                    //find number of links.
                    links = TextPointerUtils.ElementCount(_p1, _p2, _type);
                    if (!bstart && !bend && links >= 0)
                    {
                        //we adjust the Element count here.
                        _orginalElements = _orginalElements - links;
                    }
                }
            }

            _exception = false;

            if (IsIllegalRangeForSpan(_p1, _p2))
            {
                _exception = true;
            }
            
            try
            {
                CreateElement(_p1, _p2);
            }
            catch 
            {
                Verifier.Verify(_exception, "Don't expect a exception!");
          
                QueueDelegate(NextCombination);
                return; 
            }
            QueueDelegate(VerifySpanCreation);
        }

        bool IsIllegalRangeForSpan(TextPointer start, TextPointer end)
        {
            bool bstart, bend, result=false;

            //span must be created in one Paragraph. No pointer should be null.
            if (start == null || end == null || start.Paragraph == null || end.Paragraph == null || start.Paragraph != end.Paragraph)
            {
                return true;
            }

            if (start.GetOffsetToPosition(end) < 0)
            {
                //Regression_Bug605 make it illegal to create a span with end is infront of the start, Expected an exception.
                result = true;
            }
            else
            {
                // no nested hyperlink. regression for Regression_Bug604
                bstart =_wrapper.IsTextPointerInsideTextElement(start, typeof(Hyperlink)); 
                bend = _wrapper.IsTextPointerInsideTextElement(end, typeof(Hyperlink));
                if (bstart && bstart)
                {
                    if (start.GetOffsetToPosition(end) == 0 && start.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        //if the two pointers are at the beginning of the link 
                        result =  false;
                    }
                    else
                    {
                        //if the two pointers are at the end of the link, it is illegal
                        result = true;
                    }
                }
                else if (bstart) 
                {
                    //When the start is in the link and the end of not:
                    //  return false: when the start is at then end of the link.
                    //  return true: when the start is not at the enod of the link.
                    if (start.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
                else if (bend)
                {
                    //when end is in the link and the start is not.
                    //  return false: if the end is at the beginning of the link.
                    //  return true: if the end os not at the end of the link.
                    if (end.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart)
                    {
                        result = false;
                    }
                    else
                    {
                        result = true;
                    }
                }
            }

            return result; 
        }

        void VerifySpanCreation()
        {
            string currentText;
            int currentElements;
            int expected = _orginalElements + 1;

            currentElements = TextPointerUtils.ElementCount(_wrapper.Start, _wrapper.End, _type) ;
            Verifier.Verify(currentElements == expected, "Element count is wrong! Expected[" + expected + "], Actual[" + currentElements + "]");
            Verifier.Verify(!_exception, "Don't expected an exception!");
            currentText = new TextRange(_p1, _p2).Text;
            Verifier.Verify(currentText == _orginalText, "Text between two pointer won't match! Orignal[" + _orginalText + "], current[" + currentText + "]");

            QueueDelegate(NextCombination);
        }

        void CreateElement(TextPointer p1, TextPointer p2)
        {
            switch (_elementType)
            {
                case Inlines.Bold:
                    _element = new Bold(p1, p2);
                    break;
                case Inlines.Hyperlink:
                    _element = new Hyperlink(p1, p2);
                    break;
                case Inlines.Italic:
                    _element = new Italic(p1, p2);
                    break;
                case Inlines.Span:
                    _element = new Span(p1, p2);
                    break;
                case Inlines.Underline:
                   _element = new Underline(p1, p2);
                    break;
            }
        }

        /// <summary></summary>
        public static object[] Offsets
        {
            get
            {
                return TextPointerUtils.GetOffsets(s_xaml);
            }
        }

        /// <summary>return a array of textElement enum values for all span Elements</summary>
        public static object[] SpanElements
        {
            get
            {
                object[] result = {
                    Inlines.Span,
                    Inlines.Hyperlink,
                    Inlines.Bold,
                    Inlines.Italic,
                    Inlines.Underline, 
                };
                return result;
            }
        }

        /// <summary>Add more content to the xaml if new scenarios are needed.</summary>
        static string s_xaml = "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">"
                        + "<Paragraph>c</Paragraph><Table><Table.Columns><TableColumn Width=\"160\" /></Table.Columns><TableRowGroup><TableRow><TableCell BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\"><Paragraph>d<Hyperlink NavigateUri=\"link.xaml\">lk</Hyperlink></Paragraph></TableCell></TableRow></TableRowGroup></Table>"
                        + "</Section>";
    }

    /// <summary>Test Utils of the TextPointer </summary>
    public class TextPointerUtils
    {
        /// <summary>
        /// Find element between two TextPointers. 
        /// Note: do not try to use TextRange since it will normalize the positions.
        /// </summary>
        /// <param name="p1">start pointer</param>
        /// <param name="p2">End pointer </param>
        /// <param name="ElementType"></param>
        /// <returns></returns>
        public static int ElementCount(TextPointer p1, TextPointer p2, Type ElementType)
        {
            int count = 0;
            TextPointer pointer = p1.GetPositionAtOffset(0);
            if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
            {
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            while (pointer.GetOffsetToPosition(p2) >= 0 &&
                pointer.GetOffsetToPosition(p2.DocumentEnd) != 0)
            {
                if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementEnd &&
                    pointer.GetAdjacentElement(LogicalDirection.Forward).GetType() == ElementType)
                {
                    count++;
                }
                pointer = pointer.GetNextContextPosition(LogicalDirection.Forward);
            }

            return count;
        }

        /// <summary>find all the offsets of TextPositions. </summary>
        /// <returns></returns>
        public static object[] GetOffsets(string xaml)
        {
            RichTextBox box;
            TextRange range;
            int offset;
            object[] results;

            box = new RichTextBox();
            range = new TextRange(box.Document.ContentEnd, box.Document.ContentStart);
            XamlUtils.TextRange_SetXml(range, xaml);

            offset = box.Document.ContentStart.GetOffsetToPosition(box.Document.ContentEnd);
            results = new object[offset + 1];
            for (int i = 0; i < offset; i++)
            {   
                results[i] = i;
            }
            results[offset] = -1;
            return results;
        }
    }
}

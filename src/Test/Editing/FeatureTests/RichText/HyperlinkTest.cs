// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 23 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/RichText/HyperlinkTest.cs $")]
namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.IO;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using System.Windows.Media.Imaging;

    #endregion Namespaces.

    /// <summary>
    /// Hyperlink editing functional testing
    /// </summary>
    [Test(1, "RichEditing", "HyperlinkTest1", MethodParameters = "/TestCaseType:HyperlinkTest")]
    [Test(1, "PartialTrust", TestCaseSecurityLevel.FullTrust, "HyperlinkTest2", MethodParameters = "/TestCaseType:HyperlinkTest /XbapName=EditingTestDeploy")]
    [TestOwner("Microsoft"), TestWorkItem("76"), TestTactics("488"), TestBugs("841,525,842,843,844,845,846,847,848, 178")]
    public class HyperlinkTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        ///<summary>Prepare RichTextBox content.</summary>
        protected override void DoRunCombination()
        {
            _control = (RichTextBox)_editableType.CreateInstance();
            _wrapper = new UIElementWrapper(_control);
            TestElement = _control;
            _control.Height = 150;
            string str = "<Paragraph>Hello Word.</Paragraph>";
            
            _wrapper.XamlText =
                "<Paragraph><Hyperlink/></Paragraph>" +
                "<Paragraph>Navigate to a <Hyperlink NavigateUri='#UIE'>"+_hyperlinkContent+"</Hyperlink>.</Paragraph>" +
                str +
                "<Paragraph>xxx<Hyperlink><Italic><Image Height='100' Width='100'" +
                " Source='pack://siteoforigin:,,,/test.png'" +
                "/>" +
                "yyy</Italic>zzz</Hyperlink>ooo</Paragraph>"+
                str + str + str + str +
                "<Paragraph><Button Foreground='Red' Name='UIE'>Button</Button></Paragraph>" +
                str + str + str + str + str;

            QueueDelegate(ClickInHyperlink);
        }

        /// <summary>
        /// Mouse inside Hyperlink. Verify there is no navigation in edit mode.
        /// Regression_Bug844 - Click on Hyperlink in RichTextBox doesn't navigate
        /// Regression_Bug846 - Hyperlink embedded in a rich text box doesnot work
        /// </summary>
        private void ClickInHyperlink()
        {
            Rect r = _wrapper.GetGlobalCharacterRect(_positionInHyperlink);
            MouseInput.MouseClick((int)r.X, (int)r.Y);

            QueueDelegate(VerifyClick);
        }
        ///<summary>Verify there is no navigation in edit mode.</summary>
        private void VerifyClick()
        {
            string temp = _wrapper.TextBeforeOrAfterCaret(LogicalDirection.Backward);
            Verifier.Verify(temp.Contains("Link"), "Expect to contain [Link]. Actual [" + temp + "]", true);

            QueueDelegate(EnterInHyperlink);
        }

        ///<summary>
        /// Hit enter in Hyperlink. Verify Hyperlink is not split.
        /// Regression_Bug843 - System.NullReferenceException @ System.Windows.Documents.Hyperlink.OnMouseEnter
        ///</summary>
        private void EnterInHyperlink()
        {   
            _enterParagraphBreakCommand.Execute(null, _control);
            Verifier.Verify(TextUtils.CountOccurencies(_wrapper.XamlText, "</Hyperlink>") == _tagCount,
                "Enter in Hyperlink should not split up Hyperlink. Expected Count[" + _tagCount.ToString()+
                "] Actual [" + TextUtils.CountOccurencies(_wrapper.XamlText, "</Hyperlink>").ToString()+"]\r\n ActualXaml["+
                _wrapper.XamlText+"]", true);

            QueueDelegate(CopyPasteHyperlink);
        }

        ///<summary>
        /// Copy part of Hyperlink and paste at end of Hyperlink. Verify Hyperlink pasted.
        /// Regression_Bug525 - When text is pasted at end of hyperlink an additional space is added
        /// Regression_Bug845 - Pasted hyperlink will not be underlined
        ///</summary>
        private void CopyPasteHyperlink()
        {
            _wrapper.Select(_positionInHyperlink, 3); // Select UIE
            _hyperlinkContent += _wrapper.SelectionInstance.Text; //Clipboard.GetText(); use GetText will fail in PT
            KeyboardInput.TypeString("^c");                                               
            QueueDelegate(PerformPaste);
        }

        private void PerformPaste()
        {
            _control.CaretPosition = _wrapper.Start.GetPositionAtOffset(_positionEndOfHyperlink);
            KeyboardInput.TypeString("^v");
            QueueDelegate(VerifyPaste);
        }

        private void VerifyPaste()
        {
            Verifier.Verify(_wrapper.XamlText.Contains(_hyperlinkContent)==false, "Paste at end doesnt merge Hyperlink content.", true);
            
            _control.CaretPosition = _wrapper.Start.GetPositionAtOffset(_positionEndOfHyperlink).GetNextInsertionPosition(LogicalDirection.Backward);

            Image simpleImage = new Image();
            simpleImage.Height = 60;
            simpleImage.Margin = new Thickness(5);

            // Create source.
            BitmapImage bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(@"test.png", UriKind.RelativeOrAbsolute);
            bi.EndInit();
            // Set the image source.
            simpleImage.Source = bi;


            ((Hyperlink)((Run)(_control.CaretPosition.Parent)).Parent).Inlines.Add(simpleImage);
            _control.Focus();
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(SelectAll);
        }

        private void SelectAll()
        {
            //_control.SelectAll();
            KeyboardInput.TypeString("+{DOWN 2}");
            KeyboardInput.TypeString(EditingCommandData.ToggleBold.KeyboardShortcut);
            QueueDelegate(VerifyBold);
        }

        private void VerifyBold()
        {
            Verifier.Verify(_wrapper.XamlText.Contains("Bold") == true, "Content should be Bold.", true);
            KeyboardInput.TypeString("^{HOME}{DOWN}{end}");
            QueueDelegate(DeleteHyperlink);
        }

        ///<summary>
        /// Hit Backspace to remove Hyperlink completely. Verify Hyperlink tags are removed.
        /// Regression_Bug841 - Editing: Hyperlink can't be deleted from TextContainer by using {Delete} or {Backspace} key
        ///</summary>
        private void DeleteHyperlink()
        {
            RoutedCommand BackspaceCommand = EditingCommands.Backspace;
            for (int i = 0; i < _hyperlinkContent.Length + 2; i++)
                BackspaceCommand.Execute(null, _control);

            _tagCount = _tagCount - 1; // Delete remove Hyperlink competely
            Verifier.Verify(TextUtils.CountOccurencies(_wrapper.XamlText, "</Hyperlink>") == _tagCount,
                "Hyperlink tags should be removed.", true);
            QueueDelegate(EnterInEmptyHyperlink);
        }

        /// <summary>
        /// Hit Enter in empty Hyperlink. Verify no assert and enter works. Then type. Verify new text doesn't have underline
        /// Regression_Bug842 - Editing: Invariant assert thrown when RichTextBox has hyperlink and Enter is typed in
        /// Regression_Bug848 - Hyperlink causes inconsistence of underline text when {Enter} and type
        /// </summary>
        private void EnterInEmptyHyperlink()
        {
            RoutedCommand MoveUpByLineCommand = EditingCommands.MoveUpByLine;
            MoveUpByLineCommand.Execute(null, _control);
            _enterParagraphBreakCommand.Execute(null, _control);
            Verifier.Verify(TextUtils.CountOccurencies(_wrapper.XamlText, "</Hyperlink>") == _tagCount,
                "Enter on empty hyperlink should not spil Hyperlink.", true);

            KeyboardInput.TypeString("ccc+{left 3}");
            QueueDelegate(VerifyTyping);
        }
        /// <summary>Verify new text doesn't have underline</summary>
        private void VerifyTyping()
        {
            Verifier.Verify(_wrapper.SelectionInstance.Text == "ccc", "Expect [ccc]. Actual [" + _wrapper.SelectionInstance.Text + "]", true);
            Verifier.Verify((TextUtils.CountOccurencies(_wrapper.XamlText, "TextDecoration")) == _tagCount,
                "Expected Tagcount [." + _tagCount.ToString() + " ] Actual [" + (TextUtils.CountOccurencies(_wrapper.XamlText, "TextDecoration")).ToString()+"]", true);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private RichTextBox _control;
        private UIElementWrapper _wrapper;
        private TextEditableType _editableType = null;
        private string _hyperlinkContent = "Link to UIElement";
        private int _positionInHyperlink = 31;
        private int _positionEndOfHyperlink = 40;
        private int _tagCount = 3;
        private RoutedCommand _enterParagraphBreakCommand = EditingCommands.EnterParagraphBreak;

        #endregion Private fields.
    }

    /// <summary>
    /// this case repro the bugs for Hyperlink
    /// </summary>
    [Test(1, "RichEditing", "HyperlinkRegressionTest", MethodParameters = "/TestCaseType=HyperlinkRegressionTest")]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("489"), TestBugs("190, 526, 191, 849, 192, 525, 194, 527, 850"), TestLastUpdatedOn("Jun 29, 2006 by Jerang")]
    public class HyperlinkRegressionTest : CustomTestCase
    {
        UIElementWrapper _wrapper;
        RichTextBox _richTextBox;
        Hyperlink _link;
        
        /// <summary>start the local test.</summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            _richTextBox.Document.Blocks.Clear();
            _link = new Hyperlink(new Run("Test Link"));
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));

            VerificationFor_Regression_Bug526();

            MainWindow.Content = _richTextBox;
            QueueDelegate(RegressionFor_Regression_Bug190);
        }

        private void RegressionFor_Regression_Bug190()
        {
            Log("Regression test for bug#Regression_Bug190 ...");
            _richTextBox.Focus();
            QueueDelegate(ActionFor_Regression_Bug190);
        }

        //Regression Regression_Bug190 - Editing: {enter} at the end of hyperlink creates a paragraph whose child inherit the Hyperlink's Foreground and TextDecorations properties.
        private void ActionFor_Regression_Bug190()
        {
            KeyboardInput.TypeString("{HOME}{ENTER}^{HOME}First Line^{END}{ENTER}Last Line");
            QueueDelegate(VerficationFor_Regression_Bug190);
        }

        private void VerficationFor_Regression_Bug190()
        {
            Paragraph para; 
            para = _richTextBox.Document.Blocks.FirstBlock as Paragraph; 
            Verifier.Verify(para.Inlines.Count == 1, "Only 1 inline should be in the first Paragraph! Actual[" + para.Inlines.Count + "]");
            Verifier.Verify(para.Foreground != _link.Foreground, "First paragraph && link has the same foreground[" + para.Foreground.ToString() + "]");
            Verifier.Verify(para.TextDecorations == null || para.TextDecorations.Count == 0, "No decoration expected in the first Paragraph!");

            para = _richTextBox.Document.Blocks.LastBlock as Paragraph; 
            Verifier.Verify(para.Inlines.Count == 1, "Only 1 inline should be in the last Paragraph! Actual[" + para.Inlines.Count + "]");
            Verifier.Verify(para.Foreground != _link.Foreground, "Last paragraph && link has the same foreground[" + para.Foreground.ToString() + "]");
            Verifier.Verify(para.TextDecorations == null || para.TextDecorations.Count == 0, "No decoration expected in the first Paragraph!");
            QueueDelegate(RegressionFor_Regression_Bug191);
        }

        //Regression_Bug191 - Editing: {Backspace} to delete the last character of a link results an empty Hyperlink.
        private void RegressionFor_Regression_Bug191()
        {
            Log("Regression test for Regression_Bug191 ...");
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            _richTextBox.Document.Blocks.Clear();
            _link = new Hyperlink(new Run("Link"));

            _richTextBox.Document.Blocks.Add(new Paragraph(_link));

            MainWindow.Content = _richTextBox;
            QueueDelegate(SetFocusFor_Regression_Bug191);
        }

        private void SetFocusFor_Regression_Bug191()
        {
            _richTextBox.Focus();
            QueueDelegate(ActionFor_Regression_Bug191);
        }

        private void ActionFor_Regression_Bug191()
        {
            Verifier.Verify(_wrapper.XamlText.Contains("Hyperlink"), "Hyperlink expected, actual[" + _wrapper.XamlText + "]");
            KeyboardInput.TypeString("^{HOME}{RIGHT}a{END}{BACKSPACE 5}");
            QueueDelegate(VerificationFor_Regression_Bug191);
        }

        private void VerificationFor_Regression_Bug191()
        {
            Verifier.Verify(!_wrapper.XamlText.Contains("Hyperlink"), "No Hyperlink expected, actual[" + _wrapper.XamlText + "]");
            QueueDelegate(RegressionFor_Regression_Bug192);
        }

        //regression test for Regression_Bug192
        private void RegressionFor_Regression_Bug192()
        {
            Log("Regression test for Regression_Bug192 ...");
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            _richTextBox.Document.Blocks.Clear();
            _link = new Hyperlink(new Run("Link"));
            _link.FontSize = 30;
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));
            
            MainWindow.Content = _richTextBox;
            QueueDelegate(SeFocusFor_Regression_Bug192);
        }

        private void SeFocusFor_Regression_Bug192()
        {
            _richTextBox.Focus();
            QueueDelegate(ActionFor_Regression_Bug192);
        }

        private void ActionFor_Regression_Bug192()
        {
            KeyboardInput.TypeString("{HOME}{ENTER}abc");
            QueueDelegate(VerificationFor_Regression_Bug192);
        }

        private void VerificationFor_Regression_Bug192()
        {
            double actualFont;
            Paragraph para;
            Hyperlink link; 
            
            para = _richTextBox.Document.Blocks.LastBlock as Paragraph;
            link = ((IList)para.Inlines)[1] as Hyperlink; 
            actualFont = (link.Inlines.FirstInline).FontSize;
            
            Verifier.Verify(actualFont == 30, "Expected fontsize[30], Actual[" + actualFont + "]");
            QueueDelegate(RegressionFor_Regression_Bug525);
        }

        //Regression_Bug193 - Copying entire Hyperlink does not paste it as a hyperlink, need special heristic for this scenario
        private void RegressionFor_Regression_Bug525()
        {
            Log("Regression test for Regression_Bug525 ...");
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            _richTextBox.Document.Blocks.Clear();
            _link = new Hyperlink(new Run("Link"));
            _link.FontSize = 30;
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));
            
            MainWindow.Content = _richTextBox;
            
            QueueDelegate(SeFocusFor_Regression_Bug525);
        }

        private void SeFocusFor_Regression_Bug525()
        {
            _richTextBox.Focus();
            QueueDelegate(ActionFor_Regression_Bug525);
        }

        private void ActionFor_Regression_Bug525()
        {
            KeyboardInput.TypeString("{HOME}+{RIGHT}^c{END}^V");
            QueueDelegate(VerificationFor_Regression_Bug525);
        }

        private void VerificationFor_Regression_Bug525()
        {
            Paragraph para;
            TextRange range;

            para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;
            range = new TextRange(para.ContentEnd, para.ContentStart);
           
            Verifier.Verify(!range.Text.EndsWith(" "), "Link is endded with space after paste.!");

            QueueDelegate(RegressionFor_Regression_Bug194);
        }
        
        //regression for Regression_Bug194 - paste an empty paragraph into Hyperlink will insert a space in the hyperlink        
        private void RegressionFor_Regression_Bug194()
        {
            Log("Regression test for Regression_Bug194 ...");
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            _richTextBox.Document.Blocks.Clear();
            _link = new Hyperlink(new Run("Link"));
            _link.FontSize = 30;
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));

            MainWindow.Content = _richTextBox;

            QueueDelegate(SetFocus_Regression_Bug194);
        }

        private void SetFocus_Regression_Bug194()
        {
            _wrapper.Element.Focus();
            QueueDelegate(ActionFor_Regression_Bug194);
        }

        private void ActionFor_Regression_Bug194()
        {
            KeyboardInput.TypeString("{END}+{RIGHT}^C{HOME}{RIGHT}^V");
            QueueDelegate(VerificationFor_Regression_Bug194);
        }

        private void VerificationFor_Regression_Bug194()
        {
            TextRange range = new TextRange(_link.ContentEnd, _link.ContentStart);
            Verifier.Verify(range.Text.Contains(" "), "Paste an empty Paragraph in Hyperlink result a space");
            QueueDelegate(RegressionFor_Regression_Bug195);
        }

        //Regression for Regression_Bug195 - Nested Hyperlink can be a created by adding a span (that containing a hyperlink) into a hyperlink's inline collection.
        private void RegressionFor_Regression_Bug195()
        {
            Log("Regression test for Regression_Bug195 ...");
            _richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(_richTextBox);
            Paragraph para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;
            para.Inlines.Add(new Hyperlink(new Run("link")));

            try
            {
                para = _richTextBox.Document.Blocks.FirstBlock as Paragraph;          
                ((Hyperlink)para.Inlines.FirstInline).Inlines.Add(new Span(new Hyperlink(new Run("NestedLink"))));
            }
            catch (InvalidOperationException e)
            {
                //This error message is logged.                
                //Regression_Bug527 - Confusing exception message for trying to insert a span that contains a Hyperlink into a Hyperlink.
                Log("Expected Exception message[" + e.Message + "]");                
            }

            QueueDelegate(RegressionFor_Regression_Bug850);
        }

        //verify Regression_Bug526
        private void VerificationFor_Regression_Bug526()
        {
            Log("Regression test for Regression_Bug526 ...");
            string xaml;
            TextPointer start;
            TextPointer end;
            start = _link.ContentStart;
            end = _link.ContentEnd;
            if (!start.IsAtInsertionPosition)
            {
                start = start.GetInsertionPosition(LogicalDirection.Forward);
            }
            if (!end.IsAtInsertionPosition)
            {
                end = end.GetInsertionPosition(LogicalDirection.Forward);
            }
            TextRange range = new TextRange(start, end);
            xaml = Test.Uis.Utils.XamlUtils.TextRange_GetXml(range);
            Verifier.Verify(xaml.Contains("Hyperlink"), "Hyperlink is not serialized, please check xaml[" + xaml + "]");
        }

        private void RegressionFor_Regression_Bug850()
        {
            Hyperlink imageHyperlink;
            Image image;

            _richTextBox = new RichTextBox();

            //Add a valid hyperlink without any embedded objects in it.
            _link = new Hyperlink();
            _link.Inlines.Add(new Run("This is a hyperlink"));
            _link.NavigateUri = new Uri("http://www.microsoft.com/");

            Verifier.Verify(File.Exists("colors.png"), "File colors.png doesnt exists", false);

            //create an image
            image = new Image();
            image.Height = image.Width = 50;
            System.Windows.Media.Imaging.BitmapImage bitmapImage = new System.Windows.Media.Imaging.BitmapImage();
            bitmapImage.BeginInit();
            bitmapImage.StreamSource = new FileStream("colors.png", FileMode.Open);
            bitmapImage.EndInit();
            image.Source = bitmapImage;

            //Add a valid hyperlink with an image in it.
            imageHyperlink = new Hyperlink();
            imageHyperlink.Inlines.Add(image);
            imageHyperlink.NavigateUri = new Uri("http://www.msn.com/");
            
            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));
            _richTextBox.Document.Blocks.Add(new Paragraph(imageHyperlink));

            QueueDelegate(Action1For_Regression_Bug850);           
        }

        private void Action1For_Regression_Bug850()
        {
            string rtfString;

            TextRange tr = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            rtfString = XamlUtils.GetRtfFromTextRange(tr);

            Verifier.Verify(rtfString.Contains("HYPERLINK") && rtfString.Contains("www.microsoft.com"),
                "Verifying that Rtf content has Hyperlink in it", true);

            Verifier.Verify(rtfString.Contains("HYPERLINK") && rtfString.Contains("www.msn.com"),
                "Verifying that Rtf content has image Hyperlink in it", true);

            //Add a hyperlink with only a (non-image) embedded object in it.
            _link = new Hyperlink();
            _link.Inlines.Add(new TextBlock(new Run("Embedded TextBlock inside Hyperlink")));
            _link.NavigateUri = new Uri("http://www.microsoft.com");

            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));

            QueueDelegate(Action2For_Regression_Bug850);
        }

        private void Action2For_Regression_Bug850()
        {
            string rtfString;

            TextRange tr = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            rtfString = XamlUtils.GetRtfFromTextRange(tr);

            Verifier.Verify((!rtfString.Contains("HYPERLINK")) && (!rtfString.Contains("www.microsoft.com")),
                "Verifying that Rtf content doesnt have Hyperlink in it", true);

            //Add a hyperlink with (non-image) embedded object along with some text in it.
            _link = new Hyperlink();
            _link.Inlines.Add(new Run("abc"));
            _link.Inlines.Add(new TextBlock(new Run("Embedded TextBlock inside Hyperlink")));
            _link.NavigateUri = new Uri("http://www.microsoft.com");

            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(new Paragraph(_link));

            QueueDelegate(Action3For_Regression_Bug850);
        }

        private void Action3For_Regression_Bug850()
        {
            string rtfString;

            TextRange tr = new TextRange(_richTextBox.Document.ContentStart, _richTextBox.Document.ContentEnd);
            rtfString = XamlUtils.GetRtfFromTextRange(tr);

            Verifier.Verify((!rtfString.Contains("HYPERLINK")) && (!rtfString.Contains("www.microsoft.com")),
                "Verifying that Rtf content doesnt have Hyperlink in it", true);

            //more test can be chained here.
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// The verify the invalid insertion positions is inforced in Hyperlink and InlineUIContainer. 
    /// Example of invalid positions: [Hyperlink][1][Run][2]click me![3][/Run][4][/Hyperlink]
    /// Selection.Start and Selection.End will not be at the above marked location when move caret around or call selection APIs.
    /// The verification checks the following:
    /// 1. caret or selection edges will not fall into the above location. 
    /// 2. Position between [p][l] and [/p][/l]are insertion position. positions specify to Selection.Select() will not be changed by nomalizaiton.
    /// 3. caret that is moved forward, backwoard by keyboard will not hit the above invalid position.
    /// </summary>
    [Test(0, "RichEditing", "HyperlinkCornerPositionTest", MethodParameters = "/TestCaseType=HyperlinkCornerPositionTest", Timeout=240)]
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("490"), TestBugs(""), TestLastUpdatedOn("May 1, 2006")]
    public class HyperlinkCornerPositionTest : ManagedCombinatorialTestCase
    {
        #region Main flow.
        /// <summary>
        /// start a new combination.
        /// </summary>
        protected override void DoRunCombination()
        {
            _wrapper = new UIElementWrapper(new RichTextBox());
            _wrapper.XamlText = _xamlSample;
            TestElement = _wrapper.Element as FrameworkElement;
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            _counter = 0;
            _wrapper.SelectionInstance.Select(_wrapper.Start, _wrapper.Start);
            QueueDelegate(MoveCaretForward);
        }

        /// <summary>
        /// loop to move caret forward to the end.
        /// </summary>
        void MoveCaretForward()
        {
            Log("Function - MoveCaretForward...");
            VerifySelection();
            Log("counter[" + _counter + "]");
            _counter++;
            if (_counter > _wrapper.Start.GetOffsetToPosition(_wrapper.End))
            {
                QueueDelegate(MoveCaretBackward);
            }

            KeyboardInput.TypeString("{Right}");
            QueueDelegate(MoveCaretForward);
        }

        /// <summary>
        /// loop to move caret backward to the start of the document.
        /// </summary>
        void MoveCaretBackward()
        {
            Log("Function - MoveCaretBackward...");
            VerifySelection();
            Log("counter[" + _counter + "]");
            _counter--;
            if (_counter < 0)
            {
                QueueDelegate(TestSelectionAPI);
            }

            KeyboardInput.TypeString("{Left}");
            QueueDelegate(MoveCaretBackward);

        }

        /// <summary>
        /// Test the selection API to make sure that the range is normalized correctly.
        /// </summary>
        void TestSelectionAPI()
        {
            Log("Function TestSelectionAPI...");
            TextPointer start = _wrapper.Start.GetPositionAtOffset(0);
            TextPointer end;
            bool bStart, bEnd,bStartInline, bEndInline; 

            while (start.GetOffsetToPosition(_wrapper.End) != 0)
            {
                Log("Start position offset[" + _wrapper.Start.GetOffsetToPosition(start).ToString() + "]");
                for (_counter = 0; _counter < start.GetOffsetToPosition(_wrapper.End); _counter++)
                {
                    
                    end = start.GetPositionAtOffset(_counter);
                    bStart =  VerifyInsertionPosition(start);
                    bEnd = VerifyInsertionPosition(end);
                    bStartInline = VerifyTheInlineUIContainer(start);
                    bEndInline = VerifyTheInlineUIContainer(end);
                    
                    Log("End position offset[" + _wrapper.Start.GetOffsetToPosition(end).ToString() + "]");
                    
                    _wrapper.SelectionInstance.Select(start, end);
                    
                    VerifySelection();
                    //Verify when position is at the beginning of the first run or at the end of the last run in a Hyperlink.
                    if (bStart)
                    {
                        Verifier.Verify(start.GetOffsetToPosition(_wrapper.SelectionInstance.Start) == 0, 
                            "Failed: Selection normalization is wrong when specified start is an insertion position");
                    }
                    if (bEnd)
                    {
                        Verifier.Verify(end.GetOffsetToPosition(_wrapper.SelectionInstance.End) == 0,
                            "Failed: Selection normalization is wrong when specified end is an insertion position");
                  
                    }

                    //Verify the InlineUIContainer
                    //When start or/and end is in side the InlineUIContainer, the selection.Start/end should move out.
                    if (bStartInline)
                    {
                        Verifier.Verify(!VerifyTheInlineUIContainer(_wrapper.SelectionInstance.Start), 
                            "When the specified start pointer is in InlineUIContainer, Seleciton range should be normalized it out of the container.");
                    }
                    if (bEndInline)
                    {
                        Verifier.Verify(!VerifyTheInlineUIContainer(_wrapper.SelectionInstance.End),
                           "When the specified end pointer is in InlineUIContainer, Seleciton range should be normalized it out of the container.");
                    }

                }
                start = start.GetPositionAtOffset(1);
            }
            QueueDelegate(NextCombination);
        }

        /// <summary>
        /// Verify if a pointer is inside an InlineUIContainer.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns>true if it is inside InlineUIContainer, false otherwise</returns>
        bool VerifyTheInlineUIContainer(TextPointer pointer)
        {
            DependencyObject forwardElement, backwardElement;
            forwardElement = pointer.GetAdjacentElement(LogicalDirection.Forward);
            backwardElement = pointer.GetAdjacentElement(LogicalDirection.Backward);
            if (forwardElement is InlineUIContainer && backwardElement is InlineUIContainer)
            {
                Verifier.Verify(pointer.IsAtInsertionPosition, "pointer inside InlineUIContainer should not be an insertion position!");
                return true; 
            }
  
            return false; 
        }

        /// <summary>
        /// check both the Selection.Start and Selection.End
        /// </summary>
        void VerifySelection()
        {
            Log("Function VerifySelection...");
            Log("Test Selection.Start..");
            VerifySelectionPointers(_wrapper.SelectionInstance.Start);
            Log("Test Selection.End...");
            VerifySelectionPointers(_wrapper.SelectionInstance.End);
        }

        /// <summary>
        /// The verify the invalid insertion position is. 
        /// Example of invalid positions: [Hyperlink][1][Run][2]click me![3][/Run][4][/Hyperlink]
         /// </summary>
        /// <param name="pointer"></param>
        void VerifySelectionPointers(TextPointer pointer)
        {
            //Selection.Start
            TextPointerContext forwardContext, backwardContext;
            DependencyObject forwardElement, backwardElement;
            bool b;
            forwardContext = pointer.GetPointerContext(LogicalDirection.Forward);
            backwardContext = pointer.GetPointerContext(LogicalDirection.Backward);

            forwardElement = pointer.GetAdjacentElement(LogicalDirection.Forward);
            backwardElement = pointer.GetAdjacentElement(LogicalDirection.Backward);

            Verifier.Verify(!(backwardElement is Hyperlink && backwardContext == TextPointerContext.ElementStart), 
                "Failed - Sice selection edge pointer is just after <Hyperlink> tag!");

            b = IsPositionAtFirstOrLastofHyperlink(pointer, LogicalDirection.Backward);
            Verifier.Verify(!b, "Failed - Since selection edge pointer is just after<Hyperlink> tag!"); 
            b =IsPositionAtFirstOrLastofHyperlink(pointer, LogicalDirection.Forward);
            Verifier.Verify(!b, "Failed - Since selection edge pointer is just before </Hyperlink> tag!"); 


            Verifier.Verify(!(forwardElement is Hyperlink && forwardContext == TextPointerContext.ElementEnd),
                "Failed - Sice selection edge pointer is just before </Hyperlink> tag!"); 
        }

        /// <summary>
        /// check to see if a pointer is at the first beginning of first run or at the end of last run inside a hyperlink.
        /// </summary>
        /// <param name="pointer"></param>
        /// <param name="direction"></param>
        /// <returns></returns>
        bool IsPositionAtFirstOrLastofHyperlink(TextPointer pointer, LogicalDirection direction)
        {
            int i; 
             TextPointer tempPointer;
             TextPointerContext contextforward, contextBackward;
             DependencyObject obj; 
             i = (direction == LogicalDirection.Forward)? 1:-1;
             tempPointer = pointer.GetPositionAtOffset(0) ;
             if(_wrapper.IsTextPointerInsideTextElement(pointer, typeof(Hyperlink)))
             {
                 contextforward = tempPointer.GetPointerContext(LogicalDirection.Forward);
                 contextBackward = tempPointer.GetPointerContext(LogicalDirection.Backward);
                obj = tempPointer.GetAdjacentElement(direction);
                while (!(obj is Hyperlink))
                {
                    //false if context is not the same or one of the context is test 
                    //for Example: 
                    //1. <Hyperlink><Run>[1]</Run><Run>a</Run></Hyperlink>
                    //2. <Hyperlink><Run>[1]t</Run><Run>a</Run></Hyperlink>
                    if (contextforward != contextBackward || contextforward == TextPointerContext.Text || contextBackward == TextPointerContext.Text)
                    {
                        return false;
                    }
                    tempPointer = tempPointer.GetPositionAtOffset(i);
                    contextforward = tempPointer.GetPointerContext(LogicalDirection.Forward);
                    contextBackward = tempPointer.GetPointerContext(LogicalDirection.Backward);
                    obj = tempPointer.GetAdjacentElement(direction);
                }
                return true; 
             }
             else
             {
                 return false; 
             }
        }
     
        /// <summary>
        /// check to see if a postion between [p] and [l] is a insertion positoin.
        /// the return value can be used for future verification after the selection range is normalized.
        /// </summary>
        /// <param name="pointer"></param>
        /// <returns></returns>
        bool VerifyInsertionPosition(TextPointer pointer)
        {
            Log("Function VerifyInsertionPosition...");
            TextPointerContext forwardContext, backwardContext;
            DependencyObject forwardElement, backwardElement;

            forwardContext = pointer.GetPointerContext(LogicalDirection.Forward);
            backwardContext = pointer.GetPointerContext(LogicalDirection.Backward);

            forwardElement = pointer.GetAdjacentElement(LogicalDirection.Forward);
            backwardElement = pointer.GetAdjacentElement(LogicalDirection.Backward);
            
            //check <p><l>
            if (backwardElement is Paragraph && backwardContext == TextPointerContext.ElementStart &&
                forwardElement is Hyperlink && forwardContext == TextPointerContext.ElementStart)
            {
                Verifier.Verify(pointer.IsAtInsertionPosition, "Failed: the position between <Paragraph><Hyperlink> should be a insertion position");
                return true; 
            }
            //check </p></l>
            else if (backwardElement is Hyperlink && backwardContext == TextPointerContext.ElementEnd && 
                forwardElement is Paragraph && forwardContext ==TextPointerContext.ElementEnd)
            {
                Verifier.Verify(pointer.IsAtInsertionPosition, "Failed: the position between </Hyperlink></Paragraph> should be a insertion position");
                return true; 
            }

            return false; 
        }
     
        /// <summary>
        /// Sample xaml containing Hyperlink for test.
        /// more xaml senarios can be added here.
        /// </summary>
        public static string [] XamlSamples
        {
            get
            {
                string[] results = new string[] {
                    "<Paragraph><Hyperlink /></Paragraph>",
                    "<Paragraph><Hyperlink>abc</Hyperlink></Paragraph>",
                    "<Paragraph><Hyperlink></Hyperlink><Hyperlink>abc</Hyperlink></Paragraph>",
                    "<Paragraph><Hyperlink></Hyperlink><Hyperlink>abc</Hyperlink><Hyperlink></Hyperlink></Paragraph>",
                    "<Paragraph><Hyperlink>d</Hyperlink><Hyperlink>ab<InlineUIContainer><Button></Button></InlineUIContainer>c</Hyperlink><Hyperlink>e</Hyperlink></Paragraph>",
                };
             
                return results;
            }
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _wrapper;
        private int _counter;
        private string _xamlSample=string.Empty;

        #endregion Private fields.
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************
*Scenarios:
*   Case 1: Double click to select a word
*       a) word containing many chars
*       b) word containing one char
*       c) word at the beginning of a pargraph
*       d) word at the end of the a pargraph.
*       5) word seperated by Tab, etc
*   Case 2: Caret navigating for word:
*       a) Ctrl-left/right moves caret word by words
*       b) ctrl-shift-left/right selects/deselect word by words.
*       c) ctrl-shift-left/right extend/contract selection by a word.
*       d) Ctrl-shift-left/right extend/contract selection cross lines.
*       e) Shift - left/right move care between word
*       f) shift - up/done (not defined)
*   case 3: Ctrl-shift-mouse click to perfrom selection
*       a): 3
*   
*********************************************************************/

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;    
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// this class contains code for testing the passwordbox.
    ///</summary>
    [Test(0, "RichEditing", "WordEditingBVTTest", MethodParameters = "/TestCaseType=WordEditingBVTTest")]
    [TestOwner("Microsoft"), TestTactics("689")]
    public class WordEditingBVTTest : RichEditingBase
    {
        /// <summary>Content in the text control</summary>
        protected string Text = null;
        /// <summary>String typed into the text control </summary>
        protected string TypedString = null;
        
        /// <summary>Initialize the test </summary>
        public override void Init()
        {
            base.Init();
            TextControlWraper.Text = "";
            QueueDelegate(SetUpBVTCommonContent);

        }
        /// <summary> </summary>
        void SetUpBVTCommonContent()
        {
            if (null == TypedString || null == Text)
            {
                Text = "This is a test.\r\nThose are flowers\r\nWe will be better.\r\n";
                TypedString = "This is a test.{enter}Those are flowers{enter}We will be better.";
            }
            KeyboardInput.TypeString(TypedString);
        }
        /// <summary> </summary>
        void EndACase()
        {
            CheckRichedEditingResults();
            QueueHelper.Current.QueueDelegate(EndTest);
        }

        #region case - Mouse double click the first word.
        /// <summary>
        /// BVT_MouseDoubleClickTheFirstWord_
        /// </summary>
        public void BVT_MouseDoubleClickTheFirstWord_CaseStart()
        {
            EnterFunction("BVT_MouseDoubleClickTheFirstWord_CaseStart");
            MouseClicksOnACharacter(TextControlWraper, 2, CharRectLocations.Center, 2);
            SetExpectedValues(Text, "This ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - Mouse click on a word with one character
        /// <summary>
        /// BVT_MouseDoubleClickAWordWithOneCharacter
        /// </summary>
        public void BVT_MouseDoubleClickAWordWithOneCharacter_CaseStart()
        {
            EnterFuction("BVT_MouseDoubleClickAWordWithOneCharacter_CaseStart");
            MouseClicksOnACharacter(TextControlWraper, 10, CharRectLocations.Center, 2);
            SetExpectedValues(Text, "a ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - Mouseclick on a space to select a word
        /// <summary>
        /// BVT_MouseDoubleClickASpaceWithOneCharacter
        /// </summary>
        public void BVT_MouseDoubleClickASpaceWithOneCharacter_CaseStart()
        {
            EnterFuction("BVT_MouseDoubleClickASpaceWithOneCharacter_CaseStart");
            MouseClicksOnACharacter(TextControlWraper, 7, CharRectLocations.Center, 2);
            SetExpectedValues(Text, "is ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - Mouse double click to select that last word of a line.
        /// <summary>
        /// BVT_MouseDoubleClickAtTheLastWord
        /// </summary>
        public void BVT_MouseDoubleClickAtTheLastWord_CaseStart()
        {
            EnterFuction("BVT_MouseDoubleClickAtTheLastWord_CaseStart");
            MouseClicksOnACharacter(TextControlWraper, 33, CharRectLocations.Center, 2);
            SetExpectedValues(Text, "flowers", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - Mouse click on the middle of the first character of a word.
        /// <summary>
        /// MouseDoubleClickOnMiddleOfTheFirstCharOfAWord
        /// </summary>
        public void BVT_MouseDoubleClickOnMiddleOfTheFirstCharOfAWord_CaseStart()
        {
            EnterFuction("BVT_MouseDoubleClickOnMiddleOfTheFirstCharOfAWord_CaseStart");
            MouseClicksOnACharacter(TextControlWraper, 27, CharRectLocations.Center, 2);
            SetExpectedValues(Text, "are ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion
        #region case - Shift + Ctrl + Right to select the first word when caret is initially at home.
        /// <summary>
        /// BVT_Shift_Ctrl_RightToSelectFirstWord
        ///</summary>
        public void BVT_Shift_Ctrl_RightToSelectFirstWord_CaseStart()
        {
            EnterFunction("BVT_Shift_Ctrl_RightToSelectFirstWord_CaseStart");
            KeyboardInput.TypeString("^{home}+^{RIGHT}");
            SetExpectedValues(Text, "This ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - Shift + Ctrl + Right to extend the selection cross pargraph
        /// <summary>
        /// Shift + Ctrl + Right to extend the selection cross pargraph
        /// </summary>
        public void BVT_Shift_Ctrl_RightToExTendSelectionCrossParagraph_CaseStart()
        {
            EnterFunction("BVT_Shift_Ctrl_RightToExTendSelectionCrossParagraph_CaseStart");
            KeyboardInput.TypeString("^{HOME}+^{Right 7}");
            SetExpectedValues(Text, "This is a test.\r\nThose ", 2, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion  
        #region case - Shift + Ctrl + Left to ContractSelection
        /// <summary>
        /// Shift + Ctrl + Left to ContractSelection
        /// </summary>
        public void BVT_Shift_Ctrl_LeftToContractSelection_CaseStart()
        {
            EnterFunction("BVT_Shift_Ctrl_LeftToContractSelection_CaseStart");
            KeyboardInput.TypeString("^{HOME}+^{Right 7}+^{Left 4}");
            SetExpectedValues(Text, "This is a ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);

        }
        #endregion 
        #region case - Shift + Ctrl + Left to select words, then Shift + Ctrl + Right to remove the selection on words
        /// <summary>
        /// Shift + Ctrl + Left to select words, then Shift + Ctrl + Right to remove the selection on words
        /// </summary>
        public void BVT_Shift_Ctrl_LeftToSelect_RightToDeSelect_Wrod_UndoDevelopment()
        {
            EnterFunction("BVT_Shift_Ctrl_LeftToSelect_RightToDeSelect_Wrod");
            KeyboardInput.TypeString("^{HOME}{END}{LEFT 2}+^{Left 4}+^{Right}");
            CheckRichedEditingResults(Text, "is a tes", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }

        #endregion 
        #region case - Shift + Ctrl + Right to Delete selection and then continue to make selection cross pargraph.
        /// <summary>
        ///  Shift + Ctrl + Right to Delete selection and then continue to make selection cross pargraph.
        /// </summary>
        public void BVT_Shift_Ctrl_RightToDeleteAndmakeSelectionOnWord_CaseStart()
        {
            EnterFunction("BVT_Shift_Ctrl_RightToDeleteAndmakeSelectionOnWord_CaseStart");
            KeyboardInput.TypeString("^{HOME}^{RIGHT}+^{Left}+^{Right 7}");
            SetExpectedValues(Text, "is a test.\r\nThose ", 2, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion
        #region case - shift + Left move caret word by word corss Paragraphs
        /// <summary>
        ///  shift + Left move caret word by word corss Paragraphs
        /// </summary>
        public void BVT_Shift_LeftToMoveCaretWordByWrod_CaseStart()
        {
            EnterFunction("BVT_Shift_LeftToMoveCaretWordByWrod_CaseStart");
            KeyboardInput.TypeString("^{END}^{Left 7}+^{RIGHT}");
            SetExpectedValues(Text, "flowers", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion 
        #region case - shift + Right move caret word by word corss Paragraphs
        /// <summary>
        /// shift + Right move caret word by word corss Paragraphs
        /// </summary>
        void BVT_Shift_RightToMoveCaretWordByWrod_CaseStart()
        {
            EnterFunction("BVT_Shift_RightToMoveCaretWordByWrod_CaseStart");
            KeyboardInput.TypeString("^{HOME}^{Right 7}+^{RIGHT}");
            SetExpectedValues(Text, "are ", 0, TextControlWraper);
            EndFunction();
            QueueHelper.Current.QueueDelegate(EndACase);
        }
        #endregion
        #region Regression case - Regression_Bug543 - Not able to selection part of a word using mouse
        /// <summary>Regression_Bug543</summary>
        public void Regression_Bug543_CaseStart()
        {
            EnterFunction("Regression_Bug543_CaseStart");
            EndFunction();
            QueueDelegate(Regression_Bug543_DoSelectionWithMouse);
        }

        void Regression_Bug543_DoSelectionWithMouse()
        {
            EnterFunction("Regression_Bug543_DoSelectionWithMouse");
            KeyboardInput.TypeString("{Left}");
            Point p1 = FindLocation(TextControlWraper, 1, CharRectLocations.Left);
            Point p2 = FindLocation(TextControlWraper, 14, CharRectLocations.Right);
            MouseInput.MouseDown((int)p1.X, (int)p1.Y);
            MouseInput.MouseDragInOtherThread(p1, p2, false, new TimeSpan(0, 0, 0, 2), new SimpleHandler(Regression_Bug543_DoMouseUp),System.Windows.Threading.Dispatcher.CurrentDispatcher);
        }
        void Regression_Bug543_DoMouseUp()
        {
            EnterFunction("Regression_Bug543_DoMouseUp");
            Point p1 = FindLocation(TextControlWraper, 13, CharRectLocations.Right);
            MouseInput.MouseMove((int)p1.X, (int)p1.Y);
            MouseInput.MouseUp();
            QueueDelegate(Regression_Bug543_Done);
        }

        void Regression_Bug543_Done()
        {
            EnterFunction("Regression_Bug543_Done");
            CheckRichedEditingResults(Text, "This is a te", 0, TextControlWraper);
            QueueDelegate(EndTest);
            EndFunction();
        }

        #endregion 
    }

    /// <summary>
    /// Test Shift+Up for RichTextBox
    /// </summary>
    [Test(0, "RichEditing", "RTBSelectionReproRegression_Bug669", MethodParameters = "/TestCaseType=RTBSelectionReproRegression_Bug669")]
    [TestOwner("Microsoft"), TestTitle("Shift+Up Test for RichTextBox"), 
    TestBugs("Regression_Bug669"),TestTactics("688")]
    public class RTBSelectionReproRegression_Bug669 : CustomTestCase
    {
        Control _testRTB;
        UIElementWrapper _testRTBWrapper;
        string _expStr = "this is test 1\r\nthis is test 2\r\nthis is test 3\r\nthis is test 4";

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetUpTestCase();
            QueueDelegate(new SimpleHandler(TypeInput));
        }

        /// <summary>Sets up the test case</summary>
        private void SetUpTestCase()
        {            
            _testRTB = new RichTextBox();
            MainWindow.Content = _testRTB;
            _testRTB.Width = MainWindow.Width / 2;
            _testRTB.Height = MainWindow.Height / 2;
            _testRTB.SetValue(TextElement.FontFamilyProperty, new FontFamily("Palatino Linotype"));
            _testRTB.SetValue(TextElement.FontSizeProperty, 30.0);            
            _testRTBWrapper = new Test.Uis.Wrappers.UIElementWrapper(_testRTB);
            ReflectionUtils.SetProperty(_testRTB, "AcceptsReturn", true);
        }

        /// <summary>Types the input string.</summary>
        private void TypeInput()
        {
            MouseInput.MouseClick((UIElement)_testRTB);
            KeyboardInput.TypeString("this is test 1{ENTER}this is test 2{ENTER}this is test 3{ENTER}this is test 4");
            QueueDelegate(new SimpleHandler(DoSelection));
        }

        /// <summary>Does the selection with Shift+UP</summary>
        private void DoSelection()
        {
            KeyboardInput.TypeString("+{UP 4}");
            QueueDelegate(new SimpleHandler(VerifyShiftUpSelection));
        }

        /// <summary>Verifies that the selection string</summary>
        private void VerifyShiftUpSelection()
        {
            string actualSelectionStr = _testRTBWrapper.GetSelectedText(false, false);
            Log("Expected selected text [" + _expStr + "]");
            Log("Actual selected text [" + actualSelectionStr + "]");
            Verifier.Verify(actualSelectionStr == _expStr, "Shift+Up selection didnt happen as expected", true);
            Logger.Current.ReportSuccess();
        }
    }
}

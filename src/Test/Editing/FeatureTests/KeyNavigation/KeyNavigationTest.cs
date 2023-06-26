// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Basic testing for textbox

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/KeyNavigation/KeyNavigationTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    /// <summary>
    /// Runs a navigation case by loading a page, clicking on an element,
    /// typing, and then verifying the caret position.
    /// </summary>
    [Test(3, "Editor", "KeyNavigationTest", MethodParameters = "/TestCaseType:KeyNavigationTest /xml:regressionxml.xml /TestName:Regression_Bug283")]
    [TestOwner("Microsoft"), TestTitle ("KeyNavigationTest"), TestTactics("346"),
     TestArgument("XamlPage", "File name of page to start from (opt.)"),
     TestArgument("AcceptsReturn", "AcceptsReturn value for text box with Name=TextBox1"),
     TestArgument("KeystrokeString", "Keys to type"),
     TestArgument("ExpectedStringOnCaretLeft", "Expected string to the left of the caret after typing."),
     TestArgument("ExpectedStringOnCaretRight", "Expected string to the right of the caret after typing.")]
    public class KeyNavigationTest : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            ConfigurationSettings.Current.DumpingHashtableValues();

            string xamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            if (!String.IsNullOrEmpty(xamlString))
            {
                ActionItemWrapper.SetMainXaml(xamlString);
            }
            else
            {
                TextBox textbox = new TextBox();

                textbox.Name = "TextBox1";

                textbox.AcceptsReturn = ConfigurationSettings.Current.GetArgumentAsBool ("AcceptsReturn");

                MainWindow.Content = textbox;

                textbox.Width = 400;
            }
            QueueDelegate(OnAdded);
        }

        private void OnAdded()
        {
            UIElement element = ElementUtils.FindElement(MainWindow, "TextBox1");

            if (element == null)
            {
                throw new ApplicationException("Cannot find Element named TextBox1");
            }

            this._elementWrapper = new UIElementWrapper(element);

            MouseInput.MouseClick(this._elementWrapper.Element);
            QueueDelegate(InjectKeys);
        }

        private void InjectKeys()
        {
            string keystrokeString = ConfigurationSettings.Current.GetArgument("KeystrokeString");
            KeyboardInput.TypeString(keystrokeString);
            QueueDelegate(OnInput);
        }

        private void OnInput()
        {
            // TO-DO: need visual verification and verification of text in element
            string expectedStringOnLeft;
            string expectedStringOnRight;
            string output;
            string actualString;

            expectedStringOnLeft = ConfigurationSettings.Current.GetArgument ("ExpectedStringOnCaretLeft");
            expectedStringOnRight = ConfigurationSettings.Current.GetArgument ("ExpectedStringOnCaretRight");

            actualString = this._elementWrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            output = String.Format ("Text on left hand side of caret [{0}]. Expected value [{1}]",
                actualString, expectedStringOnLeft);
            Verifier.Verify(expectedStringOnLeft == actualString, output, true);

            actualString = this._elementWrapper.GetTextOutsideSelection(LogicalDirection.Forward);
            output = String.Format ("Text on right hand side of caret [{0}]. Expected value [{1}]",
                actualString, expectedStringOnRight);
            Verifier.Verify(expectedStringOnRight  == actualString, output, true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private UIElementWrapper _elementWrapper = null;

        #endregion Private fields.
    }

    /// <summary>
    /// Runs all interesting navigation test cases.
    /// </summary>
    [Test(1, "Editor", "CombinatorialNavigationTest_0.0", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=0.0 /StopOnFailure=true", Timeout = 600, Keywords = "Setup_SanitySuite")]
    [Test(1, "Editor", "CombinatorialNavigationTest_0.1", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=0.1 /StopOnFailure=true", Timeout = 600, Keywords = "Setup_SanitySuite")]
    [Test(1, "Editor", "CombinatorialNavigationTest_0.2", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=0.2 /StopOnFailure=true", Timeout = 600, Keywords = "Setup_SanitySuite")]
    [Test(2, "Editor", "CombinatorialNavigationTest_1.0", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=1.0 /StopOnFailure=true", Timeout = 600)]
    [Test(2, "Editor", "CombinatorialNavigationTest_1.1", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=1.1 /StopOnFailure=true", Timeout = 600)]
    [Test(2, "Editor", "CombinatorialNavigationTest_2.1.0", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=2.1.0 /StopOnFailure=true", Timeout = 600, Disabled=true)]
    [Test(2, "Editor", "CombinatorialNavigationTest_2.1.1", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=2.1.1 /StopOnFailure=true", Timeout = 600, Disabled=true)]
    [Test(2, "Editor", "CombinatorialNavigationTest_2.1.2", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=2.1.2 /StopOnFailure=true", Timeout = 600, Disabled=true)]
    [Test(2, "Editor", "CombinatorialNavigationTest3", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=2.2 /StopOnFailure=true", Timeout = 600, Disabled=true)]
    [Test(3, "Editor", "CombinatorialNavigationTest4", MethodParameters = "/TestCaseType=CombinatorialNavigationTest /Pri=3 /StopOnFailure=true", Timeout = 600, Disabled=true)]
    [TestOwner("Microsoft"), TestWorkItem("29,30"), TestTactics("347,348,349"),
     TestBugs("861,680,862,863,864")]
    public class CombinatorialNavigationTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // For rich-text only commands, skip the combination
            // unless we expect something to happen.
            if (this._editingData.IsRichTextOnly)
            {
                result = result && this._editableType.SupportsParagraphs;
            }

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            // Some test cases are failing when combinations are skipped. Commenting it out.
            //if (_wrapper == null || (_wrapper.Element.GetType() != EditableType.Type))
            //{
                FrameworkElement control;

                control = _editableType.CreateInstance();
                _wrapper = new UIElementWrapper(control);
                TestElement = control;
            //}
            if (_wrapper.Element is TextBox)
            {
                ((TextBox)_wrapper.Element).TextWrapping = _wrap ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
            _wrapper.Text = (_stringData.Value == null)? "" : _stringData.Value;
            _wrapper.Element.SetValue(TextBox.FontSizeProperty, (double)10);
            QueueDelegate(PrepareControl);
        }

        private void PrepareControl()
        {
            _wrapper.Element.Focus();
            if (_selectionData.Select(_wrapper))
            {
                QueueDelegate(PerformNavigation);
            }
            else
            {
                _selectionData.PrepareForSelection(_wrapper);

                QueueDelegate(PerformSelection);
            }
        }

        private void PerformSelection()
        {
            if (!_selectionData.Select(_wrapper))
            {
                Log("Unable to prepare and perform selection.");
                // Uncomment this line to detect unimplemented selections.
                // throw new Exception("Unable to prepare and perform selection.");
                QueueDelegate(NextCombination);
            }
            else
            {
                QueueDelegate(PerformNavigation);
            } 
        }

        private void PerformNavigation()
        {
           _editingState = _editingData.CaptureBeforeEditing(_wrapper);
           _editingData.PerformAction(_wrapper, VerifyNavigation, _preferCommands);
        }

        private void VerifyNavigation()
        {
            if (!IsVerifyskipped())
            {
                _editingData.VerifyEditing(_editingState);
            }
            QueueDelegate(NextCombination);
        }

        /// <summary>
        /// skip some verification for the following reason:
        /// 1. Test verfication need to be fixed.
        /// </summary>
        /// <returns></returns>
        bool IsVerifyskipped()
        {
            if ((_editingData.TestValue == KeyboardEditingTestValue.PageUp || 
                _editingData.TestValue == KeyboardEditingTestValue.PageDown ||
                _editingData.TestValue == KeyboardEditingTestValue.PageDownShift ||
                _editingData.TestValue == KeyboardEditingTestValue.PageUpShift) && TestElement is TextBox)
            {
                return true;
            }
            else if ((_editingData.TestValue == KeyboardEditingTestValue.PasteCommandKeys ||
                _editingData.TestValue == KeyboardEditingTestValue.CutCommandKeys) && 
                TestElement is TextBoxBase)
            {
                return true;
            }

            return false;

        }

        #endregion Main flow.

        #region Private fields.

        private KeyboardEditingState _editingState;
        private UIElementWrapper _wrapper;

        private TextSelectionData _selectionData=null;
        private KeyboardEditingData _editingData=null;
        private TextEditableType _editableType=null;
        private bool _preferCommands=false;
        private StringData _stringData=null;
        private bool _wrap=false;

        #endregion Private fields.
    }

    /// <summary>
    /// Basic Typing/Editing test in different foreign locales
    /// </summary>
    /// <remarks>
    /// Some editing operations has generated bugs more often and those are targeted in this test.
    /// Ex: typing, Backspace, Backspace on space, Enter, Delete
    /// </remarks>
    [Test(2, "Editor", "TypingInDifferentLocales_Arabic", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Arabic(SaudiArabia)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Chinese", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Chinese", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_English", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=English(UnitedStates)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_German", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=German", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Hebrew", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Hebrew", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Korean", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Korean", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_French", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=French", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Japanese", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Japanese", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Spanish", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=Spanish(Argentina)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales_Thai", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=0 /LocaleData=ThaiKedmanee", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Arabic", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Arabic(SaudiArabia)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Chinese", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Chinese", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_English", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=English(UnitedStates)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_German", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=German", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Hebrew", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Hebrew", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Korean", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Korean", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_French", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=French", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Japanese", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Japanese", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Spanish", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=Spanish(Argentina)", Timeout = 500)]
    [Test(2, "Editor", "TypingInDifferentLocales1_Thai", MethodParameters = "/TestCaseType:TypingInDifferentLocales /Pri=1 /LocaleData=ThaiKedmanee", Timeout = 500)]        
    [TestOwner("Microsoft"), TestWorkItem("31"), TestTactics("350"), TestBugs("498,499,500")]
    public class TypingInDifferentLocales : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if (_localeData == InputLocaleData.JapaneseMsIme2002)
            {
                QueueDelegate(NextCombination);
                return;
            }

            // Prepare the control to be tested.
            _editingControl = (TextBoxBase)_editableType.CreateInstance();
            _editingControl.AcceptsReturn = true;
            _editingControl.FontSize = _testFontSize;

            _wrapper = new UIElementWrapper(_editingControl);

            TestElement = _editingControl;
            QueueDelegate(GetFocus);
        }

        private void GetFocus()
        {
            _editingControl.Focus();
            QueueDelegate(ChangeLocale);
        }

        private void ChangeLocale()
        {
            KeyboardInput.SetActiveInputLocale(_localeData.Identifier);
            QueueDelegate(TypingStep);
        }

        private void TypingStep()
        {
            KeyboardInput.TypeString("This is a test{BACKSPACE 6}");
            QueueDelegate(EnterStep);
        }

        private void EnterStep()
        {
            KeyboardInput.TypeString("{ENTER 2}This is still a test{HOME}{UP}");
            QueueDelegate(DeleteStep);
        }

        private void DeleteStep()
        {
            KeyboardInput.TypeString("{DELETE 5}");

            _currentIndex = 0;
            QueueDelegate(PerformKeyboardEditingAction);
        }

        private void PerformKeyboardEditingAction()
        {
            if (_currentIndex < _keyboardEditingData.Length)
            {
                _keyboardEditingData[_currentIndex].PerformAction(_wrapper, PerformKeyboardEditingAction);
                _currentIndex++;
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        #endregion Main flow.

        #region Private data.

        /// <summary>EditableType of control tested.</summary>
        private TextEditableType _editableType=null;

        /// <summary>Locale information tested</summary>
        private InputLocaleData _localeData=null;

        /// <summary>FontSize tested</summary>
        private double _testFontSize=0;

        /// <summary>Editing control being tested</summary>
        TextBoxBase _editingControl;

        /// <summary>Wrapper for control being tested.</summary>
        private UIElementWrapper _wrapper;

        private KeyboardEditingData[] _keyboardEditingData = KeyboardEditingData.NavigationValues;
        private int _currentIndex;

        #endregion Private data.
    }

    /// <summary>
    /// Verifies that editing in Overtype mode works as expected.
    /// </summary>
    /// <remarks>
    /// The following execution modes are expected:
    /// Pri-1: EditingTest.exe /TestCaseType=InsertEditingCase
    /// </remarks>
    [TestOwner("Microsoft"), TestWorkItem("32"), TestTactics("351"), TestBugs("501,502,503")]
    public class InsertEditingCase : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Tests a specific combination.</summary>
        protected override void DoRunCombination()
        {
            FrameworkElement element;

            element = _editableType.CreateInstance();
            this._wrapper = new UIElementWrapper(element);
            TestElement = element;

            _toggleBackPending = _toggleBackInsertion;

            QueueDelegate(DoInsertPress);
        }

        private void DoInsertPress()
        {
            MouseInput.MouseClick(TestElement);
            KeyboardInput.TypeString("{INS}");

            if (_toggleBackPending)
            {
                _toggleBackPending = false;
                QueueDelegate(DoInsertPress);
            }
            else
            {
                QueueDelegate(PrepareForSelection);
            }
        }

        private void PrepareForSelection()
        {
            _selectionData.PrepareForSelection(_wrapper);
            QueueDelegate(PerformTyping);
        }

        private void PerformTyping()
        {
            if (_selectionData.Select(_wrapper))
            {
                QueueDelegate(new SimpleHandler(delegate {
                    _state = _editingData.CaptureBeforeEditing(_wrapper);
                    _editingData.PerformAction(_wrapper, AfterPerformAction, true);
                }));
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private void AfterPerformAction()
        {
            _editingData.VerifyEditing(this._state);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private data.

        private UIElementWrapper _wrapper;

        private bool _toggleBackPending;

        private KeyboardEditingState _state;

        /// <summary>Whether the Insert key is pressed twice to return to regular typing.</summary>
        private bool _toggleBackInsertion=false;

        private TextEditableType _editableType=null;

        private TextSelectionData _selectionData=null;

        private KeyboardEditingData _editingData=null;

        #endregion Private data.
    }
    /// <summary>
    /// Verify the Delete, Backspace, home, End keys.
    /// This is a pri-1 cases
    /// </summary>
    [Test(2, "Editor", "DeleteBackspaceHomeEnd_TB", MethodParameters = "/TestCaseType=DeleteBackspaceHomeEnd /Pri=1 /EditableType=TextBox", Timeout = 400)]
    [Test(2, "Editor", "DeleteBackspaceHomeEnd_RTB", MethodParameters = "/TestCaseType=DeleteBackspaceHomeEnd /Pri=1 /EditableType=RichTextBox", Timeout = 400)]
    [Test(0, "Editor", "DeleteBackspaceHomeEnd_PB", MethodParameters = "/TestCaseType=DeleteBackspaceHomeEnd /Pri=1 /EditableType=PasswordBox", Timeout = 400)]    
    [TestOwner("Microsoft"), TestWorkItem("33, 34"), TestTactics("352"), TestBugs("504, 505")]
    public class DeleteBackspaceHomeEnd : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            return base.DoReadCombination(values);
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            //Create control
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());

            TestElement = (FrameworkElement)_wrapper.Element;

            //Enable AcceptsReturn property.
            if (_wrapper.Element is TextBoxBase)
            {
                ((TextBoxBase)_wrapper.Element).AcceptsReturn = true;
            }

            QueueDelegate(PrepareControl);
        }

        private void PrepareControl()
        {
            //Set Focus
            _wrapper.Element.Focus();
           QueueDelegate(PerformSelection);

        }

        private void PerformSelection()
        {
            //Set content and perfrom selection
            KeyboardInput.TypeString("a{Enter 2}def^{HOME}{Right " + _selectionStartIndex + "}+{RIGHT " + _selectionLength + "}");

            QueueDelegate(PerformNavigation);

        }

        private void PerformNavigation()
        {
            //Capture the initial state
            _editingState = _keyboardData.CaptureBeforeEditing(_wrapper);

            //Perfrom keyboard action
            _keyboardData.PerformAction(_wrapper, VerifyNavigation, false);
        }

        private void VerifyNavigation()
        {
            //do verification.
            _keyboardData.VerifyEditing(_editingState);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private KeyboardEditingState _editingState;
        private UIElementWrapper _wrapper;

        private int _selectionStartIndex=0;
        private int _selectionLength=0;
        private KeyboardEditingData _keyboardData=null;
        private TextEditableType _editableType=null;

        #endregion Private fields.
    }

    /// <summary>
    /// Verify the Delete, Backspace, home, End keys.
    /// This is a pri-1 cases
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics(""), TestBugs("")]
    public class TabKeyTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            //Create control
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());

            TestElement = (FrameworkElement)_wrapper.Element;

            //set AcceptsTab propety
            if (_wrapper.Element is TextBoxBase)
            {
                ((TextBoxBase)_wrapper.Element).AcceptsTab = _acceptsTab;
            }

            QueueDelegate(PrepareControl);
        }

        private void PrepareControl()
        {
            //Set Focus
            _wrapper.Element.Focus();
            QueueDelegate(PerformSelection);
        }

        private void PerformSelection()
        {
            //Set content and perfrom selection
            KeyboardInput.TypeString( _initialContent + "^{HOME}{Right " + _selectionStartIndex + "}+{RIGHT " + _selectionLength + "}");

            QueueDelegate(PerformNavigation);
        }

        private void PerformNavigation()
        {
            //Capture the initial state
            _editingState = _keyboardData.CaptureBeforeEditing(_wrapper);

            //Perfrom keyboard action
            _keyboardData.PerformAction(_wrapper, VerifyNavigation, false);
        }

        private void VerifyNavigation()
        {
            _keyboardData.VerifyEditing(_editingState);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private KeyboardEditingState _editingState;
        private UIElementWrapper _wrapper;

        private TextEditableType _editableType=null;
        private string _initialContent=string.Empty;
        private bool _acceptsTab=false;
        private KeyboardEditingData _keyboardData=null;
        private int _selectionStartIndex=0;
        private int _selectionLength=0;

        #endregion Private fields.
    }

    /// <summary>
    /// Add some static case to cover the keyboard Navigation inside Table.
    /// this case is temporary. After all verification is implmented in KeyboardEditingData
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem("35"), TestTactics("353"), TestBugs("")]
    public class KeyNavigationInTable : RichEditingBase
    {
        int _tables;
        int _rows;
        int _cells;
        string _xaml = "<Paragraph>x</Paragraph>" +
                "<Table xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><TableColumn /><TableColumn/><TableRowGroup>" +
                "<TableRow><TableCell  BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\" ><Paragraph>ab</Paragraph></TableCell>" +
                "<TableCell  BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\"><Paragraph>cd</Paragraph></TableCell></TableRow>" +
                "<TableRow><TableCell BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\" ><Paragraph>ef</Paragraph></TableCell>" +
                "<TableCell BorderThickness=\"1,1,1,1\" BorderBrush=\"Black\" ><Paragraph>gh</Paragraph></TableCell></TableRow>" +
                "</TableRowGroup></Table>" +
                "<Paragraph>y</Paragraph>";

        /// <summary>
        /// Basic key navigation in table
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "BasicNavigationInTable")]
        public void BasicNavigationInTable()
        {
            EnterFuction("BasicNavigationInTable");

            _tables = 1;
            _rows = 2;
            _cells = 4;

            TestDataArayList = new ArrayList();

            //Navigate into a table
            TestDataArayList.Add(new RichEditingData("Down key navigate into a table", _xaml, "^{HOME}{DOWN}+{RIGHT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "a", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Down key navigate into a table", _xaml, "^{HOME}{Right 2}+{RIGHT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "a", 0, 6, true, 0));

            TestDataArayList.Add(new RichEditingData("BackSpace in a Table", _xaml, "^{HOME}{Right 3}{Backspace 5}", "x\r\nb\tcd\r\nef\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Delete in a Table", _xaml, "^{HOME}{Right 3}{Delete 5}", "x\r\na\tcd\r\nef\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Right Key cross table", _xaml, "^{HOME}{Right 5}{Delete}", "x\r\nab\td\r\nef\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Left Key cross table", _xaml, "^{END}{Left 6}{Backspace}", "x\r\nab\tcd\r\ne\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Up Key cross rows", _xaml, "^{END}{LEFT}{UP}{Delete}", "x\r\nab\tcd\r\nf\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Down key cross rows", _xaml, "^{HOME}{DOWN 2}{Delete}", "x\r\nab\tcd\r\nf\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Try to type when caret is at the end of a row", _xaml, "^{END}{LEFT 2}z", "x\r\nab\tcd\r\nef\tgh\r\nzy\r\n", "", 0, 6, true, 0));

            //Try to delete a tabe from top and bottom
            TestDataArayList.Add(new RichEditingData("Try to delete table from be top", _xaml, "^{HOME}{RIGHT}{DELETE}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Try to Backspace the table from the bottom", _xaml, "^{END}{LEFT}{BackSpace}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "", 0, 6, true, 0));

            //Select a cell:
            TestDataArayList.Add(new RichEditingData("Select cell+{RIGHT}", _xaml, "^{HOME}{RIGHT 2}+{RIGHT 3}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\t", 1, 6, true, 0));

            //Selection cross cell:
            TestDataArayList.Add(new RichEditingData("Selection cross Cells+{RIGHT}", _xaml, "^{HOME}{RIGHT 2}+{RIGHT 4}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\tcd\r\n", 2, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross cells+{LEFT}", _xaml, "^{END}{LEFT 3}+{LEFT 3}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ef\tgh\r\n", 2, 6, true, 0));

            //Selection cross row
            TestDataArayList.Add(new RichEditingData("Select cross cells+{RIGHT}", _xaml, "^{HOME}{RIGHT 7}+{RIGHT 2}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\tcd\r\nef\tgh\r\n", 4, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{Down}", _xaml, "^{HOME}+{Down 2}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "x\r\nab\tcd\r\nef\tgh\r\n", 5, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{Down}", _xaml, "^{HOME}{RIGHT 2}+{Down}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\tef\t", 2, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{Down}", _xaml, "^{HOME}{RIGHT 2}+{Down 2}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\tcd\r\nef\tgh\r\n", 4, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{up}", _xaml, "^{END}+{UP 2}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "ab\tcd\r\nef\tgh\r\ny", 5, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{UP 2}", _xaml, "^{END}{LEFT 3}+{UP}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "cd\r\ngh\r\n", 2, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Select cross row+{UP}", _xaml, "^{END}{LEFT 3}+{UP 2}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "\r\nab\tcd\r\nef\tgh\r\n", 5, 6, true, 0));

            //PageUp/Down
            TestDataArayList.Add(new RichEditingData("{PGUP} cross table", _xaml, "^{END}{PGUP}+{LEFT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "x", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("{PGUP} start from table", _xaml, "^{END}{LEFT, 3}{PGUP}+{LEFT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "x", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("{PGDN} cross table", _xaml, "^{HOME}{PGDN}+{RIGHT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "y", 0, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("{PGDN} start from table", _xaml, "^{HOME}{RIGHT, 3}{PGDN}+{RIGHT}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "y", 0, 6, true, 0));

            SetInitValue(_xaml);
            QueueDelegate(TableNavigationExecution);
            EndFunction();
        }

        /// <summary>
        /// Test creating a row
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "Create a row")]
        public void CreateARow()
        {
            _tables = 1;
            _rows = 3;
            _cells = 6;
            //Create a row.
            TestDataArayList.Add(new RichEditingData("Caret a row", _xaml, "^{END}{LEFT 2}{ENTER}", "x\r\nab\tcd\r\nef\tgh\r\n\t\r\ny\r\n", "", 0, 8, true, 0));
            SetInitValue(_xaml);
            QueueDelegate(TableNavigationExecution);
        }

        /// <summary>
        /// Delete a row using BackSpace or Delete when caret is at the end of a row.
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "Delete a row")]
        public void DeleteaRow()
        {
            _tables = 1;
            _rows = 1;
            _cells = 2;
            //Delete a row when caret is at the end of a row.
            TestDataArayList.Add(new RichEditingData("Backspace a table row", _xaml, "^{HOME}{RIGHT 8}{Backspace}", "x\r\nef\tgh\r\ny\r\n", "", 0, 4, true, 0));
            TestDataArayList.Add(new RichEditingData("Delete table row", _xaml, "^{HOME}{RIGHT 8}{Delete}", "x\r\nab\tcd\r\ny\r\n", "", 0, 4, true, 0));

            SetInitValue(_xaml);
            QueueDelegate(TableNavigationExecution);
        }

        /// <summary>
        /// Test Tab Key
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "BasicNavigationInTable")]
        public void TabKeyInTable()
        {
            EnterFunction("TabKeyInTable");
            _tables = 1;
            _rows = 2;
            _cells = 4;

            TestDataArayList = new ArrayList();
            TestDataArayList.Add(new RichEditingData("{Tab} start from table", _xaml, "^{HOME}{RIGHT 3}{TAB 5}", "x\r\nab\tcd\r\nef\tgh\r\ny\r\n", "gh", 0, 6, true, 0));
            SetInitValue(_xaml);
            QueueDelegate(TableNavigationExecution);
            EndFunction();
        }

        /// <summary>Perfrom keyboard actions</summary>
        void TableNavigationExecution()
        {
            EnterFuction("TableNavigationExecution");
            ((TextBoxBase)TextControlWraper.Element).AcceptsTab = true;
            if (TestDataArayList.Count != 0)
            {
                RichEditingData rd = ((RichEditingData)TestDataArayList[0]);
                MyLogger.Log("\r\n\r\n" + base.RepeatString("*", 30) + "Start a new Combination - " + rd.CaseDescription + base.RepeatString("*", 30));
                MyLogger.Log(CurrentFunction + " - InitialXml: [" + rd.InitialXaml + "]");
                MyLogger.Log(CurrentFunction + " - Selected Text: [" + rd.FinalSelectedText + "]");
                MyLogger.Log(CurrentFunction + " - Selected Paragraph Count: [" + rd.FinalSelectedParagraphCount + "]");
                MyLogger.Log(CurrentFunction + " - All Xml Text: [" + rd.FinalXmlText + "]");
                MyLogger.Log(CurrentFunction + " - All Paragraph Count: [" + rd.FinalAllParagraphCount + "]");
                MyLogger.Log(CurrentFunction + " - We reset init value: [" + rd.Reset + "]");
                MyLogger.Log(CurrentFunction + " - Case Priority: [" + rd.Priority + "]");

                KeyboardInput.TypeString(((RichEditingData)TestDataArayList[0]).KeyboardInputString);
                QueueDelegate(TableNavigationExecution_CheckResult);
            }
            else
                QueueDelegate(EndTest);
            EndFunction();
        }

        /// <summary>Check the result after keyboard actions.</summary>
        void TableNavigationExecution_CheckResult()
        {
            EnterFuction("TableNavigationExecution_CheckResult");
            RichEditingData rd = ((RichEditingData)TestDataArayList[0]);
            CheckTableStructure();
            CheckRichedEditingResults(rd.FinalXmlText, rd.FinalSelectedText, rd.FinalSelectedParagraphCount, rd.FinalAllParagraphCount, TextControlWraper);
            TestDataArayList.RemoveAt(0);
            if (rd.Reset && TestDataArayList.Count > 0)
            {
                SetInitValue(((RichEditingData)TestDataArayList[0]).InitialXaml);
            }

            QueueDelegate(RichEditingDataKeyBoardExecution);
            EndFunction();
        }

        /// <summary>
        /// Verify number of tables, rows, and cells
        /// </summary>
        void CheckTableStructure()
        {
            EnterFunction("CheckTableStructure");
            int tables, rows, cells;
            string xml = TextControlWraper.XamlText;
            tables = Occurency(xml, "</Table>");
            rows = Occurency(xml, "</TableRow>");
            cells = Occurency(xml, "</TableCell>");
            if (!(tables == _tables && rows == _rows && cells == _cells))
            {
                pass = false;
                MyLogger.Log("Table structure is not expected!");
                MyLogger.Log("Expected Tables[" + _tables + "], Actual Tables[" + tables + "]");
                MyLogger.Log("Expected Rows[" + _rows + "], Actual Rows[" + rows + "]");
                MyLogger.Log("Expected Rells[" + _cells + "], Actual Cells[" + cells + "]");
            }
            EndFunction();
        }
    }

    /// <summary>
    /// Executes all keyboard actions on all interesting selections
    /// around a table.
    /// </summary>
    [Test(0, "TableEditing", "TableSelectionsTest1", MethodParameters = "/TestCaseType=TableSelectionsTest /Pri=0", Timeout = 240, Keywords = "MicroSuite")]
    [Test(3, "TableEditing", "TableSelectionsTest2", MethodParameters = "/TestCaseType=TableSelectionsTest /EditingAction=EditingValues", Timeout = 900)]
    [Test(3, "TableEditing", "TableSelectionsTest3", MethodParameters = "/TestCaseType=TableSelectionsTest /EditingAction=NavigationValues", Timeout = 600)]
    [TestOwner("Microsoft"), TestWorkItem("36"), TestTactics("354,355,356"),
    TestBugs("506,507,83,508,84,509")]
    public class TableSelectionsTest: ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            result = result &&
                this._editingData.TestValue != KeyboardEditingTestValue.IncreaseIndentationCommandKeys &&
                this._editingData.TestValue != KeyboardEditingTestValue.DecreaseIndentationCommandKeys &&
                this._editingData.TestValue != KeyboardEditingTestValue.Tab &&
                this._editingData.TestValue != KeyboardEditingTestValue.TabShift;

            result = result && !(
                _sampleTableStyle == TableStyle.MissingParagraphs && !_hasTrailingParagraph
                ) && ! (
                _sampleTableStyle == TableStyle.MissingParagraphs && !_hasLeadingParagraph
                );
            result = result && _hasTrailingParagraph;

            // Some selections cannot be made in certain table styles.
            result = result &&
                !(_selectionData == TableSelection.InFirstCell && this._sampleTableStyle == TableStyle.MissingCells) &&
                !(_selectionData == TableSelection.InFirstCell && this._sampleTableStyle == TableStyle.MissingRows);
            // Cannot span cells without cells.
            result = result &&
                !((_sampleTableStyle == TableStyle.MissingCells ||
                   _sampleTableStyle == TableStyle.MissingRows) &&
                  (_selectionData == TableSelection.SpanCellsInFirstRow ||
                   _selectionData == TableSelection.SpanCellsInFirstColumn ||
                   _selectionData == TableSelection.SpanCellsInLastRow));

            // Some selections cannot be made in certain paragraph configurations.
            result = result &&
                !(_selectionData == TableSelection.AfterTable && !this._hasTrailingParagraph);

            return result;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            bool multipleCells;
            bool multipleRows;

            if (_control == null)
            {
                _control = new RichTextBox();
                _wrapper = new UIElementWrapper(_control);
                TestElement = (FrameworkElement)_wrapper.Element;
                ((TextBoxBase)TestElement).AcceptsTab = true;
            }

            // Create a table to fit the selection.
            _control.Document.Blocks.Clear();
            if (_hasLeadingParagraph)
            {
                _control.Document.Blocks.Add(new Paragraph(new Run(TextScript.Latin.Sample)));
            }
            multipleCells = multipleRows =
                this._selectionData == TableSelection.SpanCellsInFirstRow ||
                this._selectionData == TableSelection.SpanCellsInLastRow ||
                this._selectionData == TableSelection.SpanCellsInFirstColumn;
            CreateBlocksForStyle(_sampleTableStyle, _control.Document.Blocks, multipleRows, multipleCells);
            if (_hasTrailingParagraph)
            {
                _control.Document.Blocks.Add(new Paragraph(new Run(TextScript.Hebrew.Sample)));
            }

            QueueDelegate(AfterRender);
        }

        private void AfterRender()
        {
            _wrapper.Element.Focus();
            PerformSelection(_selectionData, _wrapper);
            QueueDelegate(AfterSelection);
        }

        private void AfterSelection()
        {
            _editingState = _editingData.CaptureBeforeEditing(_wrapper);
            _editingData.PerformAction(_wrapper, VerifyAction, _preferCommands);
        }

        private void VerifyAction()
        {
            // Look in the verification table and see what verifications should take place.
            // EditingData.VerifyEditing(_editingState);
            FireVerifications();
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Verifies that Bold character formatting was set across
        /// a whole-table selection.
        /// </summary>
        private void CheckCharacterFormatting()
        {
            TextPointer cursor;

            // Verifier that every Run in the table is now Bold.
            cursor = _wrapper.SelectionInstance.Start;
            while (cursor.CompareTo(_wrapper.SelectionInstance.End) < 0)
            {
                Run run;

                run = cursor.Parent as Run;
                if (run != null)
                {
                    Verifier.VerifyValue("Run.FontWeight", FontWeights.Bold, run.FontWeight);
                }
                cursor = cursor.GetNextContextPosition(LogicalDirection.Forward);
            }
        }

        private static void CreateBlocksForStyle(TableStyle style,
            BlockCollection blocks, bool multipleRows, bool multipleCells)
        {
            Table table;
            TableRowGroup rows;

            table = new Table();
            rows = new TableRowGroup();

            // The cases logically fall through from FullTable,
            // across the styles that have missing parts.
            switch (style)
            {
                case TableStyle.FullTable:
                case TableStyle.NestedTable:
                case TableStyle.MissingText:
                case TableStyle.MissingParagraphs:
                case TableStyle.MissingCells:
                    rows.Rows.Add(CreateRowForStyle(style, multipleCells));
                    if (multipleRows)
                    {
                        rows.Rows.Add(CreateRowForStyle(style, multipleCells));
                    }
                    table.RowGroups.Add(rows);
                    goto case TableStyle.MissingRows;
                case TableStyle.MissingRows:
                    blocks.Add(table);
                    break;
            }
        }

        private static TableRow CreateRowForStyle(TableStyle style, bool multipleCells)
        {
            TableRow row;

            row = new TableRow();

            switch (style)
            {
                case TableStyle.FullTable:
                case TableStyle.NestedTable:
                case TableStyle.MissingText:
                case TableStyle.MissingParagraphs:
                    row.Cells.Add(CreateCellForStyle(style));
                    if (multipleCells)
                    {
                        row.Cells.Add(CreateCellForStyle(style));
                    }
                    goto case TableStyle.MissingCells;
                case TableStyle.MissingCells:
                    break;
                default:
                    throw new ArgumentException("Cannot create row for style " + style);
            }
            
            return row;
        }

        private static TableCell CreateCellForStyle(TableStyle style)
        {
            TableCell cell;
            Paragraph paragraph;

            cell = new TableCell();
            paragraph = new Paragraph();

            switch (style)
            {
                case TableStyle.FullTable:
                    paragraph.Inlines.Clear();
                    paragraph.Inlines.Add(new Run(TextScript.Arabic.Sample));
                    goto case TableStyle.MissingText;
                case TableStyle.NestedTable:
                    paragraph.Inlines.Clear();
                    paragraph.Inlines.Add(new Run(TextScript.Katakana.Sample));
                    cell.Blocks.Add(paragraph);
                    Table nestedTable = new Table();
                    TableRowGroup nestedRows = new TableRowGroup();
                    TableRow nestedRow = new TableRow();
                    TableCell nestedCell = new TableCell(new Paragraph(new Run(TextScript.Thaana.Sample)));
                    nestedTable.RowGroups.Add(nestedRows);
                    nestedRows.Rows.Add(nestedRow);
                    nestedRow.Cells.Add(nestedCell);
                    cell.Blocks.Add(nestedTable);
                    goto case TableStyle.MissingParagraphs;
                case TableStyle.MissingText:
                    cell.Blocks.Add(paragraph);
                    goto case TableStyle.MissingParagraphs;
                case TableStyle.MissingParagraphs:
                    break;
                default:
                    throw new ArgumentException("Cannot call CreateCellForStyle with style " + style);
            }
            return cell;
        }

        private static void PerformSelection(TableSelection selection,
            UIElementWrapper wrapper)
        {
            Table table;
            TextPointer pointer;

            table = wrapper.FindTable();
            switch (selection)
            {
                case TableSelection.AfterTable:
                    pointer = table.ContentEnd.GetNextInsertionPosition(LogicalDirection.Forward);
                    wrapper.SelectionInstance.Select(pointer, pointer);
                    break;
                case TableSelection.AtEndOfLastRow:
                    wrapper.SelectionInstance.Select(table.ContentEnd, table.ContentEnd);
                    break;
                case TableSelection.InFirstCell:
                    pointer = table.RowGroups[0].Rows[0].Cells[0].ContentStart;
                    wrapper.SelectionInstance.Select(pointer, pointer);
                    break;
                case TableSelection.SpanCellsInFirstColumn:
                    // Cannot make a segmented selection programmatically through
                    // the TextSelection API - using commands instead.
                    pointer = table.RowGroups[0].Rows[0].Cells[0].ContentStart;
                    wrapper.SelectionInstance.Select(pointer, pointer);
                    EditingCommands.SelectDownByParagraph.Execute(null, wrapper.Element);
                    EditingCommands.SelectDownByLine.Execute(null, wrapper.Element);
                    break;
                case TableSelection.SpanCellsInFirstRow:
                    PerformSelectionInRow(selection, wrapper, table.RowGroups[0].Rows[0]);
                    break;
                case TableSelection.SpanCellsInLastRow:
                    PerformSelectionInRow(selection, wrapper, table.RowGroups[0].Rows[table.RowGroups[0].Rows.Count - 1]);
                    break;
                case TableSelection.WholeTable:
                    wrapper.SelectionInstance.Select(table.ContentStart, table.ContentEnd);
                    break;
            }
        }

        private static void PerformSelectionInRow(TableSelection selection,
            UIElementWrapper wrapper, TableRow row)
        {
            TextPointer start;
            TextPointer end;
            if (row == null)
            {
                throw new ArgumentNullException("row");
            }
            
            start = row.Cells[0].ContentStart;
            end = row.Cells[row.Cells.Count - 1].ContentEnd;
            wrapper.SelectionInstance.Select(start, end);
        }

        #endregion Helper methods.

        #region Verification table support.

        /// <summary>Instance to flag 'any' matches in the verification table.</summary>
        private static object s_any = new object();

        /// <summary>Verification entries field.</summary>
        /// <remarks>
        /// This member is initialized on-demand by the VerificationEntries
        /// property; all other callers should use VerificationEntries.
        /// </remarks>
        private VerificationEntry[] _verificationEntries;

        /// <summary>Ordered name of values in verification entries.</summary>
        /// <remarks>
        /// This member is initialized on-demand by the VerificationValueNames
        /// property; all other callers should use VerificationValueNames.
        /// </remarks>
        private string[] _verificationValueNames;

        /// <summary>Table of verification entries.</summary>
        private VerificationEntry[] VerificationEntries
        {
            get
            {
                if (_verificationEntries == null)
                {
                    _verificationEntries = new VerificationEntry[] {
                        new VerificationEntry(CheckCharacterFormatting, TableSelection.WholeTable,
                            KeyboardEditingData.GetValue(KeyboardEditingTestValue.BoldCommandKeys), s_any),
                    };
                }
                return _verificationEntries;
            }
        }

        /// <summary>Ordered name of values in verification entries.</summary>
        private string[] VerificationValueNames
        {
            get
            {
                if (_verificationValueNames == null)
                {
                    _verificationValueNames = new string[] {
                        "SelectionData", "EditingData", "PreferCommands"
                    };
                }
                return _verificationValueNames;
            }
        }

        /// <summary>
        /// Fires verification callbacks as per the VerificationEntries table.
        /// </summary>
        private void FireVerifications()
        {
            int valueCount;
            object[] currentValues;

            valueCount = VerificationValueNames.Length;
            currentValues = new object[valueCount];

            for (int i = 0; i < valueCount; i++)
            {
                currentValues[i] = ReflectionUtils.GetField(this, VerificationValueNames[i]);
            }

            foreach (VerificationEntry entry in VerificationEntries)
            {
                bool allMatch;

                allMatch = true;
                for (int i = 0; i < valueCount; i++)
                {
                    if (entry._filters[i] == s_any)
                    {
                        continue;
                    }
                    if (!Verifier.AreValuesEqual(entry._filters[i], currentValues[i]))
                    {
                        allMatch = false;
                        break;
                    }
                }

                if (allMatch)
                {
                    entry._callback();
                }
            }
        }

        #endregion Verification table support.

        #region Private fields.

        /// <summary>State of the control before the action.</summary>
        private KeyboardEditingState _editingState;
        /// <summary>Wrapper around the control.</summary>
        private UIElementWrapper _wrapper;
        /// <summary>Control being tested.</summary>
        private RichTextBox _control;

        /// <summary>Table-specific selection to set.</summary>
        private TableSelection _selectionData=0;
        
        /// <summary>Keyboard editing action to perform.</summary>
        private KeyboardEditingData _editingData = null;
        
        /// <summary>Whether commands are prefered to keyboard execution.</summary>
        private bool _preferCommands = false;

        /// <summary>Whether the document has a paragraph after the table.</summary>
        private bool _hasTrailingParagraph=false;

        /// <summary>Whether the document has a paragraph before the table.</summary>
        private bool _hasLeadingParagraph=false;

        /// <summary>Style of table to create.</summary>
        private TableStyle _sampleTableStyle = 0;

        #endregion Private fields.

        #region Inner types.

        /// <summary>Entry in verification table.</summary>
        public class VerificationEntry
        {
            /// <summary>Initializes a new VerificationEntry</summary>
            public VerificationEntry(SimpleHandler callback, params object[] filters)
            {
                this._callback = callback;
                this._filters = filters;
            }

            internal SimpleHandler _callback;
            internal object[] _filters;
        }

        /// <summary>Interesting table selections.</summary>
        public enum TableSelection
        {
//            BeforeTable,
            /// <summary>Selection is placed inside the first cell.</summary>
            InFirstCell,
            /// <summary>Selection spans multiple cells in the first row of the table.</summary>
            SpanCellsInFirstRow,
            /// <summary>Selection spans multiple cells in the last row of the table.</summary>
            SpanCellsInLastRow,
            /// <summary>Selection spans multiple cells in the first column of the table.</summary>
            SpanCellsInFirstColumn,
//            AtEndOfFirstRow,
//            AtEndOfMiddleRow,
            /// <summary>Selection is empty, after the last row.</summary>
            AtEndOfLastRow,
//            InLastCell,
            /// <summary>Selection is placed after the table.</summary>
            AfterTable,
            /// <summary>Selection spans the whole table.</summary>
            WholeTable,
        }

        /// <summary>Styles of table to create.</summary>
        public enum TableStyle
        {
            /// <summary>A table without any rows.</summary>
            MissingRows,
            /// <summary>A table with rows but no cells.</summary>
            MissingCells,
            /// <summary>A table with cells but no paragraphs.</summary>
            MissingParagraphs,
            /// <summary>A table with paragraphs but no text within.</summary>
            MissingText,
            /// <summary>A table with rows, cells, paragraphs and text.</summary>
            FullTable,
            /// <summary>A full table with a nested table within a cell.</summary>
            NestedTable,
            // MergedCells,
        }

        #endregion Inner types.
    }
    
    /// <summary>
    /// Verify the Delete, Backspace, home, End keys.
    /// This is a pri-1 cases
    /// </summary>
    [Test(2, "Editor", "EnterKeyInRichTextBoxTest", MethodParameters = "/TestCaseType=EnterKeyInRichTextBoxTest /Pri=1", Timeout = 240)]
    [TestOwner("Microsoft"), TestWorkItem("37"), TestTactics("357"), TestBugs("")]
    public class EnterKeyInRichTextBoxTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>override to filter out no RichTextBox type</summary>
        /// <param name="values"></param>
        /// <returns></returns>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (!_editableType.Type.Name.Contains("RichTextBox"))
            {
                result = false;
            }
            return result; 
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            //Create control
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());

            TestElement = (FrameworkElement)_wrapper.Element;
            _wrapper.Text = _content;

            QueueDelegate(PrepareControl);
        }

        private void PrepareControl()
        {
            //Set Focus
            _wrapper.Element.Focus();
            QueueDelegate(PerformSelection);
        }

        private void PerformSelection()
        {
            //Set content and perfrom selection
            KeyboardInput.TypeString( "^{HOME}{ENTER}{Right " + _selectionStartIndex + "}+{RIGHT " + _selectionLength + "}");

            QueueDelegate(PerformNavigation);
        }

        private void PerformNavigation()
        {
            //Capture the initial state
            _editingState = _keyboardData.CaptureBeforeEditing(_wrapper);

            //Perfrom keyboard action
            _keyboardData.PerformAction(_wrapper, VerifyNavigation, false);
        }

        private void VerifyNavigation()
        {
            _keyboardData.VerifyEditing(_editingState);
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Private fields.

        private KeyboardEditingState _editingState;
        private UIElementWrapper _wrapper;

        private string _content=string.Empty;
        private TextEditableType _editableType=null;
        private KeyboardEditingData _keyboardData=null;
        private int _selectionStartIndex=0;
        private int _selectionLength=0;

        #endregion Private fields.
    }
}

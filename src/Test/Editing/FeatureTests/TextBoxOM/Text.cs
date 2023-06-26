// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for text-related APIs for TextBox.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Text.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;

    using Drawing = System.Drawing;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;        
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    #region enums.

    /// <summary> Input data</summary>
    enum InputStringDataChoices
    {
        Empty,
        Multi,
    }

    /// <summary> API called </summary>
    enum FunctionChoices
    {
        GetCharIndexFromLineIndex,
        GetLineIndexFromCharIndex,
        InvalidCalls,
    }
    #endregion enums.

    /// <summary>
    /// Verifies that the AppendText, Clear and Text
    /// members work correctly.
    ///
    /// Verifies that constraints on input text are enforced.
    /// </summary>
    [Test(0, "TextBox", "TextBoxText", MethodParameters = "/TestCaseType=TextBoxText")]
    [TestOwner("Microsoft"), TestTactics("526"), TestBugs("584,724,585"), TestWorkItem("84")]
    public class TextBoxText: TextBoxTestCase
    {
        #region Test case data.        
        /// <summary>Combinatorial engine for values.</summary>
        private CombinatorialEngine _combinatorials;
        
        /// <summary>Strings to be tested.</summary>
        private string TestText
        {
            get { return ConfigurationSettings.Current.GetArgument("TestText"); }
        }        
        #endregion Test case data.
        
        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for combinations.
            
            dimensions = new Dimension[] {
                new Dimension("AcceptsReturn", new object[] { true, false }),
                new Dimension("AcceptsTab", new object[] { true, false }),
            };
            _combinatorials = CombinatorialEngine.FromDimensions(dimensions);
            
            ConfigurationSettings.Current.SetArgument("TestText", "!AD:index=0");            
            QueueDelegate(RunNextTestCase);
        }
        
        private void RunNextTestCase()
        {
            Hashtable combinationValues;   // Values for combination.
            
            combinationValues = new Hashtable();
            if (_combinatorials.Next(combinationValues))
            {
                TestTextBox.Clear();
                TestTextBox.AcceptsReturn = (bool)combinationValues["AcceptsReturn"];
                TestTextBox.AcceptsTab = (bool)combinationValues["AcceptsTab"];
            }
            else
            {
                Logger.Current.ReportSuccess();
                return;
            }
            
            VerifyInvalidCalls();
            VerifyValidCalls();
            
            QueueDelegate(TestRegression_Bug584);
        }
        
        #region Regression_Bug584.        
        private void TestRegression_Bug584()
        {
            TextBox box;    // Scratch TextBox control.
            
            Log("Verifying bug #Regression_Bug584");

            Log("Setting text to null in scratch TextBox...");
            box = new TextBox();
            box.Text = null;

            Verifier.Verify(box.Text == String.Empty,
                "TextBox.Text is empty", true);

            QueueDelegate(TestCharacterCasingInvalidValues);
        }        
        #endregion Regression_Bug584.
        
        #region CharacterCasing invalid value testing        
        private void TestCharacterCasingInvalidValues()
        {
            // Coverage for Regression_Bug585.
            try
            {
                TestTextBox.CharacterCasing = (CharacterCasing)(-1);
                throw new ApplicationException("CharacterCasing accepts -1 value.");
            }
            catch(SystemException)
            {
                Log("Exception caught as expected.");
            }

            try
            {
                TestTextBox.CharacterCasing = (CharacterCasing)123;
                throw new ApplicationException("CharacterCasing accepts 123 value.");
            }
            catch(SystemException)
            {
                Log("Exception caught as expected.");
            }

            try
            {
                TestTextBox.SetValue(TextBox.CharacterCasingProperty, (CharacterCasing)123);
                throw new ApplicationException("CharacterCasing accepts 123 value through SetValue.");
            }
            catch(SystemException)
            {
                Log("Exception caught as expected.");
            }
            
            TestTextBox.Clear();            
            QueueDelegate(RunNextTestCase);
        }
        #endregion CharacterCasing invalid value testing.

        #endregion Main flow.

        #region Verifications.
        private void VerifyInvalidCalls()
        {
            // The members tested by this test-case are meant for Mort
            // and they should never throw exceptions. There are no
            // invalid arguments that can be tested in this case.
        }

        private void VerifyValidCalls()
        {
            Log("Verifying no-op AppendText calls...");
            TestTextBox.Text = TestText;
            VerifyText(TestTextBox, TestText);
            TestTextBox.AppendText(null);
            VerifyText(TestTextBox, TestText);
            TestTextBox.AppendText(String.Empty);
            VerifyText(TestTextBox, TestText);

            Log("Verifying Clear calls...");
            TestTextBox.Text = TestText;
            VerifyText(TestTextBox, TestText);
            TestTextBox.Clear();
            VerifyText(TestTextBox, String.Empty);
            TestTextBox.AppendText(String.Empty);
            VerifyText(TestTextBox, String.Empty);

            Log("Verifying repeat Clear calls...");
            TestTextBox.Clear();
            VerifyText(TestTextBox, String.Empty);

            Log("Verifying AppendText calls...");
            TestTextBox.Clear();
            TestTextBox.AppendText(TestText);
            TestTextBox.SelectAll();
            VerifyText(TestTextBox, TestText);
            VerifySelectedText(TestTextBox, TestText);
            TestTextBox.AppendText(TestText);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the AppendText, Clear and Text members work correctly
    /// regarless of the selection position and fire a TextChanged event unless
    /// AppendText uses an empty string and Clear was already empty.    
    /// </summary>
    [Test(0, "TextBox", "TextBoxTextManipulation", MethodParameters = "/TestCaseType=TextBoxTextManipulation")]
    [TestOwner("Microsoft"), TestTactics("525"), TestBugs(""), TestWorkItem("84")]
    public class TextBoxTextManipulation : CustomTestCase
    {
        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupCombinatorialEngine();
            QueueDelegate(NextCombination);
        }

        private void NextCombination()
        {
            if (!GetNextCombination())
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                _textBox = new TextBox();
                _wrapper = new UIElementWrapper(_textBox);
                MainWindow.Content = _textBox;
                
                QueueDelegate(BeforePrepare);
            }
        }

        private void BeforePrepare()
        {
            _wrapper.Element.Focus();
            _selectionData.PrepareForSelection(_wrapper);
            QueueDelegate(AfterPrepare);
        }

        private void AfterPrepare()
        {
            if (_selectionData.Select(_wrapper))
            {
                Log("Adjusted selection for: " + _selectionData.TestValue);
                _textBox.TextChanged += new TextChangedEventHandler(_textBox_TextChanged);
                _isEventFired = 0;
                _textBeforeAction = _textBox.Text;
                if (_testAction == "AppendText")
                {
                    QueueDelegate(AppendTextTest);
                }
                else if (_testAction == "Clear")
                {
                    QueueDelegate(ClearTest);
                }
                else if (_testAction == "Text")
                {
                    QueueDelegate(TextTest);
                }
            }
            else
            {
                Log("Unable to select: " + _selectionData.TestValue);
                QueueDelegate(NextCombination);
            }
        }

        private void AppendTextTest()
        {            
            _textBox.AppendText(_sampleText);
            Verifier.Verify(_textBox.Text == _textBeforeAction + _sampleText,
                "Verifying contents after AppendText()", false);
            if ((_sampleText == null) || (_sampleText == string.Empty))
            {
                Verifier.Verify(_isEventFired==0, "Verifying that TextChanged event didnt fire when append string is null or empty", false);
            }
            else
            {
                Verifier.Verify(_isEventFired==1, "Verifying that TextChanged event fired for AppendText", false);
            }

            QueueDelegate(NextCombination);
        }

        private void ClearTest()
        {            
            _textBox.Clear();
            Verifier.Verify(_textBox.Text == string.Empty,
                "Verifying contents after Clear()", false);
            if (_textBeforeAction == string.Empty)
            {
                Verifier.Verify(_isEventFired==0, "Verifying that TextChanged event didnt fire for Clear when it is already empty", false);
            }
            else
            {
                Verifier.Verify(_isEventFired==1, "Verifying that TextChanged event fired for Clear", false);
            }

            QueueDelegate(NextCombination);
        }

        private void TextTest()
        {            
            _textBox.Text=_sampleText;
            if (_sampleText == null)
            {
                _sampleText = string.Empty; //assigning empty string to _sampleText before using it to verification.
            }
            Verifier.Verify(_textBox.Text == _sampleText,
                "Verifying contents after setting Text", false);
            if (_textBeforeAction == _sampleText)
            {
                Verifier.Verify(_isEventFired==0, "Verifying that TextChanged event didnt fire when setting Text with same contents", false);
            }
            else
            {
                Verifier.Verify(_isEventFired==1, "Verifying that TextChanged event fired when setting Text", false);
            }

            QueueDelegate(NextCombination);
        }

        void _textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            _isEventFired += 1;
        }
        #endregion Main flow.

        #region Helper methods.
        private bool GetNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (_engine.Next(values))
            {
                _selectionData = (TextSelectionData)values["TextSelection"];
                _sampleText = (string)values["TextToAppend"];
                _testAction = (string)values["TestAction"];
                Log("Testing [" + _testAction + "] with selection [" + _selectionData + "] with sample text [" + _sampleText + "]");                
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupCombinatorialEngine()
        {
            object[] sampleTextValues = new object[] { null, 
                "abc",
                StringData.WrappingLine.Value,
                StringData.SurrogatePair.Value,
                StringData.MixedScripts.Value,
                };

            _engine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TextSelection", TextSelectionData.Values),
                new Dimension("SampleText", sampleTextValues),
                new Dimension("TestAction", new object[] {"AppendText", "Clear", "Text"})
                });
        }
        #endregion Helper methods.

        #region Private fields.
        /// <summary>Combinatorial engine driving test.</summary>
        private CombinatorialEngine _engine;

        private TextBox _textBox;

        /// <summary>Wrapper around instance being edited.</summary>
        private UIElementWrapper _wrapper;

        private TextSelectionData _selectionData;
        private string _sampleText;
        private string _testAction;

        private string _textBeforeAction;
        private int _isEventFired;
        #endregion Private fields.        
    }

    /// <summary>
    /// Verifies that pressing the Return key will insert a line
    /// break in the text.
    /// </summary>
    [Test(0, "TextBox", "TextBoxTextEnter", MethodParameters = "/TestCaseType=TextBoxTextEnter")]
    [TestOwner("Microsoft"), TestTactics("524")]
    public class TextBoxTextEnter: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            UIElement control;
            TextBox box;
            
            control = TestControl;
            box = control as TextBox;
            if (box != null)
            {
                box.AcceptsReturn = true;
                SetTextBoxProperties(box);
            }
            MouseInput.MouseClick(control);
            KeyboardInput.TypeString("{ENTER}");
            QueueDelegate(CheckText);
        }

        private void CheckText()
        {
            VerifyBreak();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyBreak()
        {
            Log("Verifying there is a break in the text...");

            string text = TestWrapper.Text;
            Log("Text: [" + text + "]");

            int breakIndex = text.IndexOf(Environment.NewLine);
            Log("Environment.NewLine index: " + breakIndex);

            Verifier.Verify(breakIndex != -1, "Break found", true);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that setting the text property before the TextBox is
    /// shown dows not make it show up selected.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("523"), TestBugs("641")]
    [Test(3, "TextBox", "TextBoxTextReproRegression_Bug641", MethodParameters = "/TestCaseType=TextBoxTextReproRegression_Bug641")]
    public class TextBoxTextReproRegression_Bug641: CustomTestCase
    {
        #region Private fields.

        private const string Content = "some content";
        private TextBox _box;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _box = new TextBox();

            Log("Setting text to some content...");
            _box.Text = Content;

            Verifier.Verify(_box.Text == Content,
                "TextBox.Text has set content", true);

            MainWindow.Content = _box;
            QueueDelegate(AfterDisplay);
        }

        private void AfterDisplay()
        {
            Log("After display, TextBox selection length is " +
                _box.SelectionLength);

            Verifier.Verify(_box.SelectionLength == 0,
                "There is no selection by default.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that different properties can be set through property triggers.
    /// </summary>
    /// <remarks>
    /// Mini-test-matrix:
    /// - Property being set.
    /// - Property trigger that sets the property.
    /// - Original value of property.
    /// - Value set in trigger.
    /// - Whether the value is modified before / after / during / never.
    ///   - Whether the value is modified programmatically or by the user.
    /// 
    /// Things to verify:
    /// - Before the property is set, the trigger-specific value does not apply.
    /// - When the property has the required value, the trigger-specific value applies.
    /// - When the trigger no longer applies, the trigger-specific value is reverted if applicable.
    /// 
    /// </remarks>
    [Test(2, "TextBox", "TextBoxTriggers", MethodParameters = "/TestCaseType:TextBoxTriggers")]
    [TestOwner("Microsoft"), TestTactics("522"), TestBugs("642")]
    public class TextBoxTriggers : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs the current combination.</summary>
        protected override void DoRunCombination()
        {
            // Move the mouse away from the future control area.
            MoveMouseOutside();
            
            // Setup style and control on window.
            SetupTestElement();

            QueueDelegate(AfterLayout);
        }

        private void AfterLayout()
        {
            // Verify initial property value.
            Verifier.VerifyValue("TextBox.Text", _initialValue, _control.Text);

            // Move the mouse over the control to trigger a change.
            MouseInput.MouseMove(_control);

            QueueDelegate(AfterMouseOver);
        }

        private void AfterMouseOver()
        {
            // Verify the property value has changed.
            Verifier.VerifyValue("TextBox.Text", _triggerValue, _control.Text);

            // If requested to do so, modify the text value.
            if (_interactiveChange)
            {
                _control.Focus();
                KeyboardInput.TypeString(Spaces);
            }

            QueueDelegate(AfterChange);
        }

        private void AfterChange()
        {
            // Move the mouse away from the control.
            MoveMouseOutside();

            QueueDelegate(AfterMouseOut);
        }

        private void AfterMouseOut()
        {
            string expectedText;

            // Verify the property value has changed only if it was not modified.
            if (_interactiveChange)
            {
                // Behavior change in 4.0 (Look at Part1 Regression_Bug600)
                // In 3.0/3.5, typing in TextBox caused the trigger to stop working because typing caused 
                // a local value to hide the trigger
                // In 4.0, this is fixed. So typing in TextBox will not invalidate the trigger.
                expectedText = _initialValue;
            }
            else
            {
                expectedText = _initialValue;                
            }            

            Verifier.VerifyValue("TextBox.Text", expectedText, _control.Text);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        private void MoveMouseOutside()
        {
            MouseInput.MouseMove(5, 5);
        }

        private void SetupTestElement()
        {
            Canvas parent;      // Parent of control.
            TextBox control;    // Control being tested.
            Style style;        // Style being built.
            Trigger trigger;    // Trigger set up for property change.

            // Create the control and its style.
            control = new TextBox();
            style = new Style(control.GetType());
            trigger = new Trigger();

            control.Height = 30d;
            control.Foreground = Brushes.Red;
            control.FontSize = 20d;
            trigger.Property = TextBox.IsMouseOverProperty;
            trigger.Value = true;
            trigger.Setters.Add(new Setter(TextBox.TextProperty, _triggerValue));
            style.Triggers.Add(trigger);
            if (_initialValue != "")
            {
                style.Setters.Add(new Setter(TextBox.TextProperty, _initialValue));
            }

            // Create the control and its parent.
            parent = new Canvas();
            parent.Children.Add(control);
            parent.Resources.Add(control.GetType(), style);

            // Place everything on the test window.
            TestElement = parent;

            _control = control;
        }

        #endregion Helper methods.

        #region Private fields.

        private TextBox _control;

        private bool _interactiveChange=false;
        private string _initialValue=string.Empty;
        private const string Spaces = "   ";
        private string _triggerValue = TextScript.Katakana.Sample;

        #endregion Private fields.
    }

    /// <summary> Tests for GetLineIndexFromCharIndex and GetCharIndexFromLineIndex </summary>
    [Test(0, "TextBox", "TextBoxGetLineCharIndex", MethodParameters = "/TestCaseType=TextBoxGetLineCharIndex")]
    [TestOwner("Microsoft"), TestTactics("521"), TestWorkItem("83")]
    public class TextBoxGetLineCharIndex : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is TextBox)
            {
                InitializeTextBox();
                Verifier.Verify(_textBox.GetLineIndexFromCharacterIndex(-1) == -1,"GetLineIndexFromCharacterIndex(-1) before attaching to tree returns -1", true) ;
                Verifier.Verify(_textBox.GetCharacterIndexFromLineIndex(-1) == -1, "GetCharacterIndexFromLineIndex(-1) before attaching to tree returns -1", true);
                TestElement = _element;
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>focuses on the control element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ProgramController();
        }

        /// <summary>ProgramController</summary>
        private void ProgramController()
        {
            switch (_functionNameSwitch)
            {
                case FunctionChoices.GetCharIndexFromLineIndex:
                    VerifyGetCharIndexFromLineIndex();
                    break;

                case FunctionChoices.GetLineIndexFromCharIndex:
                    VerifyGetLineIndexFromCharIndex();
                    break;

                case FunctionChoices.InvalidCalls:
                    VerifyInvalidCalls();
                    break;

                default:
                    break;
            }
            NextCombination();
        }

        /// <summary>VerifyGetCharIndexFromLineIndex</summary>
        private void VerifyGetCharIndexFromLineIndex()
        {
            int _lineIndices = (_inputStringSwitch == InputStringDataChoices.Multi) ? 1 : 0;
            for (int i = 0; i <= _lineIndices; i++)
            {
                int _charIndex = _textBox.GetCharacterIndexFromLineIndex(i);
                int _expectedIndex = (i == 0) ? 0 : 4; //AB->2 \R\N->2 
                Verifier.Verify(_charIndex == _expectedIndex, "GetCharIndexFromLineIndex for Line [" + i.ToString() +
                    "] is Actual [" + _charIndex.ToString() + "] Expected [" + _expectedIndex.ToString() + "]", true);
            }
        }

        /// <summary>VerifyGetLineIndexFromCharIndex</summary>
        private void VerifyGetLineIndexFromCharIndex()
        {
            int _charIndices = (_inputStringSwitch == InputStringDataChoices.Multi) ? (_firstLine.Length + 2 + _secLine.Length+ 1):0; //+1 for EOD symbol
            for (int i = 0; i < _charIndices; i++)
            {
                int _lineIndex = _textBox.GetLineIndexFromCharacterIndex(i);
                int _expectedIndex;

                // Behavior change in 4.0 (Look at Part1 Regression_Bug72)
                // In 4.0 with TextBox content [ab\r\ncd], GetLineLindexFromCharacterIndex() returns 2 starting from char index 3
                // In 3.0/3.5 with TextBox content [ab\r\ncd], GetLineLindexFromCharacterIndex() returns 2 starting from char index 2
                _expectedIndex = (i <= (_firstLine.Length + 1)) ? 0 : 1;
                Verifier.Verify(_lineIndex == _expectedIndex, "GetLineIndexFromCharacterIndex for CharIndex [" + i.ToString() +
                    "] is Actual [" + _lineIndex.ToString() + "] Expected [" + _expectedIndex.ToString() + "]", true);
            }
        }

        /// <summary>VerifyInvalidCalls</summary>
        private void VerifyInvalidCalls()
        {
            try
            {
                _textBox.GetLineIndexFromCharacterIndex(-1);
                throw new ApplicationException("GetLineIndexFromCharacterIndex(-1) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetLineIndexFromCharacterIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetLineIndexFromCharacterIndex(_firstLine.Length + 2 + _secLine.Length+ 2);
                throw new ApplicationException("GetLineIndexFromCharacterIndex(7) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetLineIndexFromCharacterIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetCharacterIndexFromLineIndex(-1);
                throw new ApplicationException("GetCharacterIndexFromLineIndex(-1) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetCharacterIndexFromLineIndex(-1) throws ArgumentOutOfRangeException as Expected");
            }
            try
            {
                _textBox.GetCharacterIndexFromLineIndex(2);
                throw new ApplicationException("GetCharacterIndexFromLineIndex(2) Doesnt throw ArgumentOutOfRangeException");
            }
            catch (ArgumentOutOfRangeException)
            {
                Log("GetCharacterIndexFromLineIndex(2) throws ArgumentOutOfRangeException as Expected");
            }
        }

        #region Helpers.

        /// <summary>Initialize TextBox</summary>
        private void InitializeTextBox()
        {
            _textBox = _element as TextBox;
            _textBox.FontWeight = FontWeights.Bold;
            _textBox.FontSize = 40;
            _textBox.FlowDirection = (_flowDirectionProperty == true) ? (FlowDirection.LeftToRight) : (FlowDirection.RightToLeft);
            _textBox.Text = (_inputStringSwitch == InputStringDataChoices.Multi)?( _firstLine + Environment.NewLine + _secLine):"";
            _controlWrapper = new UIElementWrapper(_element);
        }

        #endregion Helpers.

        #region data.

        private TextBox _textBox;
        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private InputStringDataChoices _inputStringSwitch = 0;
        private FunctionChoices _functionNameSwitch = 0;

        private bool _flowDirectionProperty = false; //true == LTR false == RTL
        private string _firstLine = "ab";
        private string _secLine = "cd";

        #endregion data.
    }
}

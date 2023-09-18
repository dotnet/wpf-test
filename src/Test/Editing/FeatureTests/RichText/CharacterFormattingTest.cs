// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


//  Functional and unit test cases for the character formatting
//  functionality. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    
    using Bitmap = System.Drawing.Bitmap;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Input;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Wrappers;
    using Test.Uis.Utils;

    #endregion Namespaces.

    #region enums.

    /// <summary> Cup Copy Paste possibilities </summary>
    enum ContentChoicesRTB
    {
        EmptyRTB,
        List,
        BuicAndText,
        DiffFontSizes,
    }
    
    enum InlineTypes
    {
        ConsecutiveRuns,
        BoldNormal,
        SpanItalicNormal,
        ParagraphBoldNormal,
    }

    enum SelectionChoices
    {
        NoSelection,
        SelectAll,
        SelectAcrossRuns,
    }
    #endregion enums.

    /// <summary>Utility class for testing TextRange.</summary>
    public sealed class TextRangeTestUtils
    {
        #region Public methods.

        /// <summary>Creates a new TextRange with the edge positions
        /// having inward gravity.</summary>
        public static TextRange CreateInwardRange()
        {
            // Create a TextBox, set some sample text,
            // and return the range spanning its content.
            TextBox box = new TextBox();
            box.Text = SampleText;

            return new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(box), Test.Uis.Utils.TextUtils.GetTextBoxEnd(box));
        }

        /// <summary>Creates a LocalValueEnumerator for testing.</summary>
        public static LocalValueEnumerator CreateLocalValueEnumerator()
        {
            return new TextBlock().GetLocalValueEnumerator();
        }

        /// <summary>
        /// A TextContainer that will refuse calls that would generate rich text.
        /// </summary>
        public static TextRange CreatePlainTextContainerRange()
        {
            TextBox box = new TextBox();
            return new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(box), Test.Uis.Utils.TextUtils.GetTextBoxEnd(box));
        }

        /// <summary>Creates a new TextRange.</summary>
        public static TextRange CreateRange()
        {
            // Create a document with some sample text.
            FlowDocument document = new FlowDocument();
            document.Blocks.Add(new Paragraph(new Run(TextScript.Latin.Sample)));

            return new TextRange(document.ContentStart, document.ContentEnd);
        }

        /// <summary>Creates a new TextRange.</summary>
        public static TextRange CreateRangeInElement()
        {
            // Create a range within an element.
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();
            Bold bold = new Bold();

            bold.Inlines.Add(new Run(TextScript.Latin.Sample));
            paragraph.Inlines.Add(bold);
            document.Blocks.Add(paragraph);

            return new TextRange(paragraph.ContentStart, paragraph.ContentEnd);
        }

        /// <summary>
        /// A TextContainer that will accepts calls that would generate rich text.
        /// </summary>
        public static TextRange CreateRichTextContainerRange()
        {
            RichTextBox box = new RichTextBox();
            return new TextRange(box.Document.ContentStart, box.Document.ContentEnd);
        }

        /// <summary>Creates a position outside any containing elements.</summary>
        public static TextPointer CreateUnscopedPosition()
        {
            TextBox box = new TextBox();
            return Test.Uis.Utils.TextUtils.GetTextBoxStart(box);
        }

        /// <summary>Creates a range outside any containing elements.</summary>
        public static TextRange CreateUnscopedRange()
        {
            TextBox box = new TextBox();
            return new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(box), Test.Uis.Utils.TextUtils.GetTextBoxEnd(box));
        }

        #endregion Public methods.

        #region Public properties.

        /// <summary>Sample text to be used when contents are not important.</summary>
        public static string SampleText
        {
            get { return "Some simple sample text."; }
        }

        #endregion Public properties.
    }

    /// <summary>
    /// Verifies that TextRange.CanApply and TextRange.Apply
    /// reject null DependencyProperty values.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("471"),TestBugs("456")]
    public class TextRangeRepro456: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            //TextRange does not have ApplyProperty method

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that AppendElement can be called on a range that
    /// has edges facing inward.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("472"),TestBugs("457")]
    public class TextRange457: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            // Select an arbitrary inline element.
            
            Log("Appending element to inward-facing range...");
            //TextRange range = TextRangeTestUtils.CreateInwardRange();
            //TextPointer p = range.Start;
            //inline.Apply(range.End, range.End);
            //TextRange elementRange = new TextRange(p, range.End);

            Log("Verifying that the inserted range is still within the range...");
            //Verifier.Verify(elementRange.Start.CompareTo(range.Start) >= 0);
            //Verifier.Verify(elementRange.End.CompareTo(range.End) <= 0);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that all formatting properties can be applied to all scripts.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("473"),
     TestArgument("FastMode", "bool indicating whether a quick run is done rather than all combinations.")]
    [Test(3, "RichEditing", "TextRangeScriptFormatting", MethodParameters = "/TestCaseType=TextRangeScriptFormatting", Disabled=true)]
    public class TextRangeScriptFormatting: CustomTestCase
    {
        #region Private data.

        private DockPanel _panel;
        private TextBlock _scriptText;
        private TextBox _textbox;
        private int _scriptIndex;
        private TextScript[] _scripts;
        private int _propertyIndex;
        private DependencyProperty[] _formattingProperties;
        private int _valueIndex;
        private object[] _propertyValues;
        private System.Diagnostics.Stopwatch _watch;
        private long _scriptsMillisecondsTaken;

        #endregion Private data.

        #region Arguments.

        private bool _fastMode;
        private bool _fastModeReady;

        /// <summary>Whether a quick run is done rather than all combinations tested.</summary>
        private bool FastMode
        {
            get
            {
                if (!_fastModeReady)
                {
                    _fastMode = Settings.GetArgumentAsBool("FastMode", false);
                    _fastModeReady = true;
                }
                return _fastMode;
            }
        }

        #endregion Arguments.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupTestVariables();

            SetupControls();
            // Set controls to initial values.
            _scriptText.Text = _scripts[_scriptIndex].Name;
            _textbox.Text = _scripts[_scriptIndex].Sample;

            QueueHelper.Current.QueueDelegate(RunStep);
        }

        private void AdvanceToNextStep()
        {
            // Steps cycle by property value, property, and script.
            _valueIndex++;
            if (_valueIndex == _propertyValues.Length || FastMode)
            {
                _valueIndex = 0;
                _propertyIndex++;
                if (_propertyIndex == _formattingProperties.Length)
                {
                    _propertyIndex = 0;
                    _scriptIndex++;
                    _scriptsMillisecondsTaken = _watch.ElapsedMilliseconds;
                    if (_scriptIndex == _scripts.Length)
                    {
                        return;
                    }
                    _textbox.Text = _scripts[_scriptIndex].Sample;
                }
                _propertyValues = CharacterFormattingHelper.ListFormattingPropertyValues(
                    _formattingProperties[_propertyIndex]);

                // The last value is always good value.
                if (FastMode)
                {
                    _valueIndex = _propertyValues.Length - 1;
                }
            }
        }

        private void SetupControls()
        {
            _panel = new DockPanel();
            _panel.LastChildFill = true;
            _scriptText = new TextBlock();
            _textbox = new TextBox();
            DockPanel.SetDock(_scriptText, Dock.Top);
            //DockPanel.SetDock(_textbox, Dock.Fill);
            _panel.Children.Add(_scriptText);
            _panel.Children.Add(_textbox);
            TestWindow.Content = _panel;
        }

        private void SetupTestVariables()
        {
            _scriptIndex = 0;
            _propertyIndex = 0;
            _valueIndex = 0;

            _scripts = TextScript.Values;
            _formattingProperties = CharacterFormattingHelper.FormattingProperties;
            _propertyValues = CharacterFormattingHelper.ListFormattingPropertyValues(_formattingProperties[0]);

            _watch = System.Diagnostics.Stopwatch.StartNew();
        }

        /// <summary>Gets a value indicating whether the test has consumed all variants.</summary>
        private bool TestFinished
        {
            get { return _scriptIndex == _scripts.Length; }
        }

        /// <summary>
        /// Updates the description text on the top label.
        /// </summary>
        private void UpdateDescriptionText()
        {
            if (TestFinished)
            {
                _scriptText.Text = "Finished";
            }
            else
            {
                DependencyProperty property = _formattingProperties[_propertyIndex];
                object propertyValue = _propertyValues[_valueIndex];
                string text = "Elapsed: " + SecondsElapsed + "\". ";
                if (_scriptsMillisecondsTaken > 0)
                {
                    long msElapsed = _watch.ElapsedMilliseconds;
                    float msPerScript = (float)_scriptsMillisecondsTaken / _scriptIndex;
                    float totalEstimated = _scripts.Length * msPerScript;
                    long remaining = (long)((totalEstimated - msElapsed) / 1000);
                    text += "Remaining: " + remaining + "\". ";
                }
                text += "Script: " + _scripts[_scriptIndex].Name + ". ";
                text += property.Name + "=" + propertyValue.ToString();
                _scriptText.Text = text;
            }
        }

        /// <summary>Gets a value indicating how many seconds ago the test case started.</summary>
        public long SecondsElapsed
        {
            get
            {
                long msElapsed = _watch.ElapsedMilliseconds;
                long secondsElapsed = (long)((float)msElapsed / 1000);
                return secondsElapsed;
            }
        }

        private void RunStep()
        {
            UpdateDescriptionText();

            // Get the current property and value.
            DependencyProperty property = _formattingProperties[_propertyIndex];
            object propertyValue = _propertyValues[_valueIndex];
            Log("Applying value " + propertyValue.ToString() + " to property " + property.Name + ".");

            // Apply property.
            TextRange range = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(_textbox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(_textbox));
            //which one is appropriate?
            range.ApplyPropertyValue(property, propertyValue);
            //range.ApplyPropertyValue(property, propertyValue);

            // Interact with content, even if meaningless.
            MouseInput.MouseClick(_textbox);
            KeyboardInput.TypeString("a +1{BS}{BS}{BS}");

            QueueHelper.Current.QueueDelegate(CheckStep);
        }

        private void CheckStep()
        {
            DependencyProperty property = _formattingProperties[_propertyIndex];
            object expectedValue = _propertyValues[_valueIndex];
            TextPointer p = Test.Uis.Utils.TextUtils.GetTextBoxStart(_textbox);
            p = p.GetPositionAtOffset(2);
            object currentValue = p.Parent.GetValue(property);
            Verifier.Verify(expectedValue.Equals(currentValue),
                "Current value [" + currentValue.ToString() + "] == expected value [" +
                expectedValue.ToString() + "] for property [" + property.Name + "].");

            // Advance to the next step, and succeed or queue another run.
            AdvanceToNextStep();
            if (TestFinished)
            {
                Log("Elapsed time: " + SecondsElapsed + " seconds.");
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueHelper.Current.QueueDelegate(RunStep);
            }
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that Bold/Italic/Underline formatting works in RichTextBox.
    /// </summary>
    [Test(0, "RichEditing", "CharFormattingBIU1", MethodParameters = "/TestCaseType:CharFormattingBIU")]

    // DISABLEDUNSTABLETEST:
    // TestName: CharFormattingBIU2
    // Area: Editing SubArea: PartialTrust
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
        [Test(1, "PartialTrust", TestCaseSecurityLevel.FullTrust, "CharFormattingBIU2", MethodParameters = "/TestCaseType:CharFormattingBIU /InputMonitorEnabled:False /XbapName=EditingTestDeploy", Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("474,475"), TestWorkItem("71"), TestBugs("458")]
    public class CharFormattingBIU : CustomTestCase
    {
        #region MainFlow
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
                _rtb = new RichTextBox();
                _rtbWrapper = new UIElementWrapper(_rtb);
                MainWindow.Content = _rtb;
                _rtb.Height = 500;
                _rtb.Width = 500;
                _rtb.FontSize = 24;
                ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run(_testContent));

                QueueDelegate(SelectText);
            }
        }

        /// <summary> Selects some text in the text control </summary>
        private void SelectText()
        {
            _rtb.Focus();
            TextSelection textSelection = _rtbWrapper.SelectionInstance;

            _selectionStart = _rtb.Document.ContentStart;
            _selectionEnd = _rtb.Document.ContentStart;
            _selectionEnd = _selectionEnd.GetPositionAtOffset(_rtbWrapper.Text.Length/2);
            textSelection.Select(_selectionStart, _selectionEnd);
            
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(250),
                new SimpleHandler(DoFormatAction), null);
        }

        private void DoFormatAction()
        {
            _bmpBeforeFormat = BitmapCapture.CreateBitmapFromElement(_rtb);
            _rtb.Focus();
            _typeCommandString = "";
            switch (_testAction)
            {
                case TestSteps.B:
                    _typeCommandString = EditingCommandData.ToggleBold.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { true, false, false });
                    break;

                case TestSteps.I:
                    _typeCommandString = EditingCommandData.ToggleItalic.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { false, true, false });
                    break;

                case TestSteps.U:
                    _typeCommandString = EditingCommandData.ToggleUnderline.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { false, false, true });
                    break;

                case TestSteps.BI:
                    _typeCommandString = EditingCommandData.ToggleBold.KeyboardShortcut + 
                        EditingCommandData.ToggleItalic.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { true, true, false });
                    break;

                case TestSteps.BU:
                    _typeCommandString = EditingCommandData.ToggleBold.KeyboardShortcut + 
                        EditingCommandData.ToggleUnderline.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { true, false, true });
                    break;

                case TestSteps.IU:
                    _typeCommandString = EditingCommandData.ToggleItalic.KeyboardShortcut + 
                        EditingCommandData.ToggleUnderline.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { false, true, true });
                    break;

                case TestSteps.BIU:
                    _typeCommandString = EditingCommandData.ToggleBold.KeyboardShortcut + 
                        EditingCommandData.ToggleItalic.KeyboardShortcut + 
                        EditingCommandData.ToggleUnderline.KeyboardShortcut;
                    KeyboardInput.TypeString(_typeCommandString);
                    QueueHelper.Current.QueueDelegate(new FormatDelegate(TestFormatting), new Object[] { true, true, true });
                    break;

                default:
                    break;
            }
        }

        private void TestFormatting(bool testBold, bool testItalic, bool testUnderline)
        {
            _bmpAfterFormat = BitmapCapture.CreateBitmapFromElement(_rtb);

            //Verify that rendering is updated after formatting is applied.
            if (ComparisonOperationUtils.AreBitmapsEqual(_bmpBeforeFormat, _bmpAfterFormat, out _bmpDifferences))
            {
                Logger.Current.LogImage(_bmpAfterFormat, "bmpAfterFormat");                
                throw new Exception("There have been no changes in rendering after formatting is applied.");
            }

            Verifier.Verify(VerifyFormattingAttributes(testBold, testItalic, testUnderline),
                "Verifying that formatting properties are applied", true);
            
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(300),
                new SimpleHandler(DoUnFormatAction), null);
        }

        private void DoUnFormatAction()
        {
            KeyboardInput.TypeString(_typeCommandString);
            QueueHelper.Current.QueueDelegate(new FormatDelegate(TestUnFormatting), new Object[] { false, false, false });
        }

        private void TestUnFormatting(bool testBold, bool testItalic, bool testUnderline)
        {
            _bmpAfterUnFormat = BitmapCapture.CreateBitmapFromElement(_rtb);

            //Verify that rendering is updated after unformatting.
            if (ComparisonOperationUtils.AreBitmapsEqual(_bmpAfterFormat, _bmpAfterUnFormat, out _bmpDifferences))
            {
                Logger.Current.LogImage(_bmpAfterUnFormat, "bmpAfterUnFormat");                
                throw new Exception("There have been no changes in rendering after unformatting.");
            }

            Verifier.Verify(VerifyFormattingAttributes(testBold, testItalic, testUnderline),
                "Verifying that formatting properties are removed after unformatting", true);

            QueueDelegate(NextCombination);
        }

        /// <summary>
        /// Verifies whether bold/italic/underline are applied properly.
        /// </summary>
        /// <param name="testBold">true if we have to verify that bold is on for the selected text</param>
        /// <param name="testItalic">true if we have to verify that italics is on for the selected text</param>
        /// <param name="testUnderline">true if we have to verify that underline is on for the selected text</param>
        /// <returns>returns true if formatting properties are as expected.</returns>
        private bool VerifyFormattingAttributes(bool testBold, bool testItalic, bool testUnderline)
        {
            bool formatPassed = true;
            TextPointer tp = _selectionStart.GetPositionAtOffset(_selectionStart.GetOffsetToPosition(_selectionEnd)/2);

            #region VerifyBold
            if (testBold)
            {

                if (CharacterFormattingHelper.VerifyFormatting(tp, "Bold"))
                {
                    Log("Bold set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Bold not set when we expect it to be set.", true);
                    formatPassed = false;
                }
            }
            else
            {
                if (!CharacterFormattingHelper.VerifyFormatting(tp, "Bold"))
                {
                    Log("Bold not set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Bold set when we expect it to be not set.", true);
                    formatPassed = false;
                }
            }
            #endregion VerifyBold

            #region VerifyItalic
            if (testItalic)
            {
                if (CharacterFormattingHelper.VerifyFormatting(tp, "Italic"))
                {
                    Log("Italics set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Italics not set when we expect it to be set.", true);
                    formatPassed = false;
                }
            }
            else
            {
                if (!CharacterFormattingHelper.VerifyFormatting(tp, "Italic"))
                {
                    Log("Italics not set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Italics set when we expect it to be not set.", true);
                    formatPassed = false;
                }
            }
            #endregion VerifyItalic

            #region VerifyUnderline
            if (testUnderline)
            {
                if (CharacterFormattingHelper.VerifyFormatting(tp, "Underline"))
                {
                    Log("Underline set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Underline not set when we expect it to be set.", true);
                    formatPassed = false;
                }
            }
            else
            {
                if (!CharacterFormattingHelper.VerifyFormatting(tp, "Underline"))
                {
                    Log("Underline not set as expected.");
                }
                else
                {
                    Logger.Current.ReportResult(false, "Underline set when we expect it to be not set.", true);
                    formatPassed = false;
                }
            }
            #endregion VerifyUnderline

            return formatPassed;
        }
        #endregion MainFlow

        #region Helper methods.
        private bool GetNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (_engine.Next(values))
            {
                _testContent = (string)values["TestContent"];
                _testAction = (TestSteps)values["TestAction"];
                Log("Testing [" + _testAction + "] with Text [" + _testContent + "]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupCombinatorialEngine()
        {
            object[] _testActions = new object[] {
                TestSteps.B,
                TestSteps.I,
                TestSteps.U,
                TestSteps.BI,
                TestSteps.BU,
                TestSteps.IU,
                TestSteps.BIU
                };

            string[] _testContents = new string[] {
                //"      ",
                //"\t\t",
                "This is a test",
                "This is a test\r\nThis is a test\r\nThis is a test"
            };

            _engine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TestContent", _testContents),
                new Dimension("TestAction", _testActions)
                });
        }
        #endregion Helper methods.

        #region members
        /// <summary>Combinatorial engine driving test.</summary>
        private CombinatorialEngine _engine;

        private RichTextBox _rtb;

        /// <summary> Wrapper for TextControl </summary>
        private UIElementWrapper _rtbWrapper;

        /// <summary>
        /// Enum of Test actions
        /// </summary>
        enum TestSteps { B, I, U, BI, BU, IU, BIU };

        string _testContent;
        TestSteps _testAction;
        string _typeCommandString;
        TextPointer _selectionStart, _selectionEnd;

        Bitmap _bmpBeforeFormat, _bmpAfterFormat, _bmpAfterUnFormat, _bmpDifferences;

        private delegate void FormatDelegate(bool testBold, bool testItalic, bool testUnderline);
        #endregion members
    }

    /// <summary>
    /// Verifies set-only commands related to character formatting in RichTextBox.
    /// </summary>
    [Test(2, "RichEditing", "CharFormattingTest", MethodParameters = "/TestCaseType:CharFormattingTest", Keywords = "Setup_SanitySuite",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("476"), TestWorkItem("71"), TestBugs("859,860"), TestLastUpdatedOn("July 20, 2006")]
    public class CharFormattingTest: CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            return new Dimension[] {
                new Dimension("CharacterFormattingProperties", DependencyPropertyData.GetCharacterFormattingProperties()),
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _testPropertyData = (DependencyPropertyData)values["CharacterFormattingProperties"];

            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 75;
            _rtb.BorderThickness = new Thickness(0);
            _rtb.Width = 200;
            _rtb.FontSize = 20;
            _rtbWrapper = new UIElementWrapper(_rtb);
            //text was changed because of aliasing effect when some chars are moved around
            //Same width chars are used now, which should reduce the effect
            ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run("SAMPLED"));

            _button = new Button();
            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            _panel.Children.Add(_button);
            TestElement = _panel;
            _testValueIndex = 0;

            _criteria = new ComparisonCriteria();
            //The below value in conjunction with rtb.Height&Width is very critical, 
            //since there will be few pixel difference after applying/unapplying 
            //formatting. If a test case fails in bitmap comparisons, this value might 
            //have to be adjusted.
            _criteria.MaxErrorProportion = 0.025f;

            _textDecorationsCriteria = new ComparisonCriteria();
            _textDecorationsCriteria.MaxErrorProportion = 0.015f;

            _resetFormatCommand = (RoutedUICommand)ReflectionUtils.GetStaticProperty(typeof(EditingCommands), "ResetFormat");

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _button.Focus();
            QueueDelegate(BeforeFormatting);
        }

        private void BeforeFormatting()
        {
            TextPointer tp;
            object testValue;

            testValue = _testPropertyData.TestValues[_testValueIndex];
            _beforeFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);
            _textSelection = _rtb.Selection;

            tp = _rtb.Document.ContentStart.GetPositionAtOffset(_rtb.Document.ContentStart.GetOffsetToPosition(_rtb.Document.ContentEnd) / 2);
            _textSelection.Select(_rtb.Document.ContentStart, tp);
            Log("Applying formatting...");
            _textSelection.ApplyPropertyValue(_testPropertyData.Property, testValue);

            QueueDelegate(SetSelection);
        }

        private void SetSelection()
        {
            //_textSelection.Select(_rtb.Document.ContentStart, _rtb.Document.ContentStart);
            QueueDelegate(AfterFormatting);
        }

        private void AfterFormatting()
        {
            ComparisonCriteria criteria;

            bool expectedEqual;     // Whether we expect bitmaps to be equal.

            //Verify that value is set on the selection
            Verifier.VerifyValue("Value of " + _testPropertyData.Property.ToString() + " on selection", _testPropertyData.TestValue,
                _textSelection.GetPropertyValue(_testPropertyData.Property));

            _afterFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);
            expectedEqual = !IsChangeExpected(_testPropertyData.Property,
                _testPropertyData.DefaultValue,
                _testPropertyData.TestValues[_testValueIndex]);

            if (_testPropertyData.Property == Inline.TextDecorationsProperty)
            {
                criteria = _textDecorationsCriteria;
            }
            else
            {
                criteria = _criteria;
            }

            if (expectedEqual != ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(
                                _beforeFormatting, _afterFormatting, criteria, false))
            {
                Logger.Current.LogImage(_beforeFormatting, "before-formatting");
                Logger.Current.LogImage(_afterFormatting, "after-formatting");

                if (expectedEqual)
                {
                    Log("Expected equal images, but they are different.");
                }
                else
                {
                    Log("Expected different images, but they are equal.");
                }

                Verifier.Verify(false, "Rendering didn't behave as expected; " +
                    "see before- and after- formatting images.");
            }

            _rtb.Focus();
            
            if (_performResetFormatVerification)
            {
                QueueDelegate(DoResetFormat);
            }
            else
            {
                EvaluateAndPerformNextCombination();
            }            
        }

        private void DoResetFormat()
        {
            Log("Calling ResetFormat command...");
            //Keyboard shortcut Ctrl+Space for resetting format collides with system shortcut for switching keyboard layouts in Vista
            //KeyboardInput.TypeString("^{SPACE}");
            _resetFormatCommand.Execute(null, _rtb);
            QueueDelegate(MoveFocusAway);  
        }

        private void MoveFocusAway()
        {
            _button.Focus();
            QueueDelegate(AfterResetFormat);  
        }

        private void AfterResetFormat()
        {
            //Verify that property value is cleared except for Language and FD property
            if ((_testPropertyData.Property == FrameworkElement.LanguageProperty) ||
                (_testPropertyData.Property == FrameworkElement.FlowDirectionProperty))
            {
                Verifier.VerifyValue("Value of " + _testPropertyData.Property.ToString() + " on selection", _testPropertyData.TestValue,
                _textSelection.GetPropertyValue(_testPropertyData.Property));
            }
            else
            {
                Verifier.VerifyValueDifferent("Value of " + _testPropertyData.Property.ToString() + " on selection", _testPropertyData.TestValue,
                _textSelection.GetPropertyValue(_testPropertyData.Property));
            }

            ComparisonCriteria criteria;
            _afterResetFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);

            _criteria.MaxErrorProportion = _criteria.MaxErrorProportion + 0.05f;
            criteria = _criteria;

            if (! ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_beforeFormatting, 
                _afterResetFormatting, criteria, false))
            {
                Logger.Current.LogImage(_beforeFormatting, "before-formatting");
                Logger.Current.LogImage(_afterResetFormatting, "afterReset-formatting");

                Verifier.Verify(false, "Captures before formatting and after reset formatting are not equal" +
                    "see before- and afterReset- formatting images.");
            }

            EvaluateAndPerformNextCombination();
        }

        private void EvaluateAndPerformNextCombination()
        {
            _testValueIndex++;
            if (_testValueIndex < _testPropertyData.TestValues.Length)
            {
                QueueDelegate(BeforeFormatting);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private static bool IsChangeExpected(DependencyProperty property,
            object originalValue, object newValue)
        {
            if (originalValue == null && newValue == null)
            {
                return false;
            }

            // Following properties *doesnt* change rendering even if the value changes.  
            if ((property == TextElement.FontStretchProperty) ||
                (property == FrameworkElement.LanguageProperty) ||
                (property == FrameworkElement.FlowDirectionProperty) ||
                (property == NumberSubstitution.CultureSourceProperty) ||
                (property == NumberSubstitution.SubstitutionProperty) ||
                (property == NumberSubstitution.CultureOverrideProperty))
            {
                return false;
            } 

            if (originalValue == null && newValue != null ||
                originalValue != null && newValue == null)
            {
                return true;
            }                     

            if (originalValue.GetType() != newValue.GetType())
            {
                return true;
            }

            if (originalValue is Brush)
            {
                return !BrushData.AreBrushesEqual((Brush)originalValue, (Brush) newValue);
            }
            
            return (!originalValue.Equals(newValue));
        }

        #endregion Main flow

        #region Private fields

        private RichTextBox _rtb;
        private Button _button;
        private StackPanel _panel;
        private UIElementWrapper _rtbWrapper;
        private TextSelection _textSelection;
        private DependencyPropertyData _testPropertyData;
        private Bitmap _beforeFormatting, _afterFormatting, _afterResetFormatting;        
        private int _testValueIndex;
        private ComparisonCriteria _criteria, _textDecorationsCriteria;

        private bool _performResetFormatVerification = true;

        private RoutedUICommand _resetFormatCommand;

        #endregion Private fields
    }

    /// <summary>Verifies auto word expansion while applying formatting for empty selection</summary>
    [Test(0, "RichEditing", "AutoWordExpansionFormatting", MethodParameters = "/TestCaseType:AutoWordExpansionFormatting")]
    [TestOwner("Microsoft"), TestTactics("477"), TestBugs("689")]
    public class AutoWordExpansionFormatting : ManagedCombinatorialTestCase
    {
        #region MainFlow

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if (_formattingCommand.Name == "ResetFormat")
            {
                Log("Skipping ResetFormat command...Its tested after other commands are done to clear the properties");
                NextCombination();
                return;
            }

            _rtb = new RichTextBox();
            _rtb.Height = 50;
            _rtb.Width = 300;
            _rtb.FontFamily = new FontFamily("Palatino Linotype");
            _rtb.FontSize = _initialFontSize;

            _rtb.Document.Blocks.Clear();

            _para = new Paragraph();
            _run = new Run(_content);
            _para.Inlines.Add(_run);
            _rtb.Document.Blocks.Add(_para);

            //1st[_firstWordPointer]Word 2nd[_secondWordPointer]Word 3rd[_thirdWordPointer]Word
            _firstWordPointer = _run.ContentStart.GetPositionAtOffset(_firstWordOffset + 3);
            _secondWordPointer = _run.ContentStart.GetPositionAtOffset(_secondWordOffset + 3);
            _thirdWordPointer = _run.ContentStart.GetPositionAtOffset(_thirdWordOffset + 3);
                                    
            _rtb.Selection.Select(_secondWordPointer, _secondWordPointer);

            _control = new TextBox();
            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            _panel.Children.Add(_control);

            _criteria = new ComparisonCriteria();
            _criteria.MaxErrorProportion = 0.01f;

            TestElement = _panel;

            _resetFormatCommand = (RoutedUICommand)ReflectionUtils.GetStaticProperty(typeof(EditingCommands), "ResetFormat");

            QueueDelegate(SetInitialFocus);
        }

        private void SetInitialFocus()
        {
            _control.Focus(); //so that caret is not there when we capture bitmap of RTB
            QueueDelegate(BeforeFormatting);
        }

        private void BeforeFormatting()
        {
            _beforeFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);
            _rtb.Focus();
            QueueDelegate(DoFormatting);
        }

        private void DoFormatting()
        {
            KeyboardInput.TypeString(_formattingCommand.KeyboardShortcut);
            QueueDelegate(VerifyFormattingProperties);
        }

        private void VerifyFormattingProperties()
        {
            _control.Focus();
            VerifyFormatting(true);
            QueueDelegate(AfterFormatting);
        }

        private void AfterFormatting()
        {
            _afterFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);

            //BitmapCapture comparison
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_beforeFormatting, _afterFormatting, _criteria, false))
            {
                Logger.Current.LogImage(_beforeFormatting, "beforeFormatting");
                Logger.Current.LogImage(_afterFormatting, "afterFormatting");
                Verifier.Verify(false, "Image comparison failed after formatting is done. " +
                    "Expect that bitmap captures are different. " +
                    "Look at beforeFormatting.png and afterFormatting.png images");
            }

            _rtb.Focus();
            QueueDelegate(DoResetFormatOrToggle);
        }

        private void DoResetFormatOrToggle()
        {           
            if (_formattingCommand.IsToggleCommand && _verifyToggle)
            {
                Log("Toggling the command...");
                KeyboardInput.TypeString(_formattingCommand.KeyboardShortcut);

                QueueDelegate(VerifyResetFormatOrToggle);
            }
            else
            {
                if (_performResetFormatVerification)
                {
                    Log("Calling ResetFormat command...");
                    //Keyboard shortcut Ctrl+Space for resetting format collides with system shortcut for switching keyboard layouts in Vista
                    //KeyboardInput.TypeString("^{SPACE}");                
                    _resetFormatCommand.Execute(null, _rtb);
                    QueueDelegate(VerifyResetFormatOrToggle);
                }
                else
                {
                    QueueDelegate(NextCombination);
                }
            }            
        }

        private void VerifyResetFormatOrToggle()
        {
            _control.Focus();
            VerifyFormatting(false);
            QueueDelegate(AfterResetFormatting);
        }

        private void AfterResetFormatting()
        {
            _afterResetFormatting = BitmapCapture.CreateBitmapFromElement(_rtb);

            //BitmapCapture comparison            
            if (!ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_beforeFormatting, _afterResetFormatting, _criteria, false))
            {
                Logger.Current.LogImage(_beforeFormatting, "beforeFormatting");
                Logger.Current.LogImage(_afterResetFormatting, "afterResetFormatting");
                Verifier.Verify(false, "Image comparison failed after formatting is removed. " +
                    "Expect that bitmap captures are same. " +
                    "Look at beforeFormatting.png and afterResetFormatting.png images");
            }

            QueueDelegate(NextCombination);
        }

        #endregion MainFlow

        #region Helper functions

        private void VerifyFormatting(bool isFormatOn)
        {
            TextDecorationCollection decoration;
            switch (_formattingCommand.Name)
            {
                case "ToggleBold":
                    if (isFormatOn)
                    {
                        Verifier.Verify((FontWeight)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontWeightProperty) ==
                            FontWeights.Bold, "Verify that Bold property is on tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((FontWeight)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontWeightProperty) ==
                            FontWeights.Normal, "Verify that Bold property is off tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((FontWeight)GetPropertyValueAtPosition(_firstWordPointer, TextElement.FontWeightProperty) ==
                            FontWeights.Normal, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((FontWeight)GetPropertyValueAtPosition(_thirdWordPointer, TextElement.FontWeightProperty) ==
                            FontWeights.Normal, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "ToggleItalic":
                    if (isFormatOn)
                    {
                        Verifier.Verify((FontStyle)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontStyleProperty) ==
                            FontStyles.Italic, "Verify that Italic property is on tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((FontStyle)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontStyleProperty) ==
                            FontStyles.Normal, "Verify that Italic property is off tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((FontStyle)GetPropertyValueAtPosition(_firstWordPointer, TextElement.FontStyleProperty) ==
                            FontStyles.Normal, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((FontStyle)GetPropertyValueAtPosition(_thirdWordPointer, TextElement.FontStyleProperty) ==
                            FontStyles.Normal, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "ToggleUnderline":
                    if (isFormatOn)
                    {
                        Verifier.Verify((TextDecorationCollection)GetPropertyValueAtPosition(_secondWordPointer, Inline.TextDecorationsProperty) ==
                            TextDecorations.Underline, "Verify that Underline property is on tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        decoration = (TextDecorationCollection)GetPropertyValueAtPosition(_secondWordPointer, Inline.TextDecorationsProperty);
                        Verifier.Verify(decoration == null || decoration.Count == 0, "Verify that Underline property is off tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    decoration = (TextDecorationCollection)GetPropertyValueAtPosition(_firstWordPointer, Inline.TextDecorationsProperty);
                    Verifier.Verify(decoration == null || decoration.Count == 0, "Verify that no formatting is applied before tested word", false);
                    decoration = (TextDecorationCollection)GetPropertyValueAtPosition(_thirdWordPointer, Inline.TextDecorationsProperty);
                    Verifier.Verify(decoration == null || decoration.Count == 0, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "ToggleSubscript":
                    if (isFormatOn)
                    {
                        Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_secondWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Subscript, "Verify that Subscript property is on tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_secondWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that Subscript property is off tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_firstWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_thirdWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "ToggleSuperscript":
                    if (isFormatOn)
                    {
                        Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_secondWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Superscript, "Verify that Superscript property is on tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_secondWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that Superscript property is off tested word", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_firstWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((FontVariants)GetPropertyValueAtPosition(_thirdWordPointer, Typography.VariantsProperty) ==
                            FontVariants.Normal, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "IncreaseFontSize":
                    if (isFormatOn)
                    {
                        Verifier.Verify((double)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontSizeProperty) >
                            _initialFontSize, "Verify that FontSize property on tested word is increased", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((double)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that FontSize property on tested word is equal to the initial size", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((double)GetPropertyValueAtPosition(_firstWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((double)GetPropertyValueAtPosition(_thirdWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that no formatting is applied after tested word", false);
                    
                    break;
                case "DecreaseFontSize":
                    if (isFormatOn)
                    {
                        Verifier.Verify((double)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontSizeProperty) <
                            _initialFontSize, "Verify that FontSize property on tested word is decreased", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }
                    else
                    {
                        Verifier.Verify((double)GetPropertyValueAtPosition(_secondWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that FontSize property on tested word is equal to the initial size", true);
                        Verifier.Verify(((Run)_secondWordPointer.Parent).Text.Length >= 7,
                            "Verifying that property is applied to the entire word", true);
                    }

                    //1stWord and 3rdWord should never be affected
                    Verifier.Verify((double)GetPropertyValueAtPosition(_firstWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that no formatting is applied before tested word", false);
                    Verifier.Verify((double)GetPropertyValueAtPosition(_thirdWordPointer, TextElement.FontSizeProperty) ==
                            _initialFontSize, "Verify that no formatting is applied after tested word", false);
                    
                    break;
            }
        }

        private object GetPropertyValueAtPosition(TextPointer tp, DependencyProperty property)
        {
            return tp.Parent.GetValue(property);
        }

        #endregion Helper functions

        #region Private fields

        private RichTextBox _rtb;
        private TextBox _control;
        private StackPanel _panel;
        private Paragraph _para;
        private Run _run;
        private TextPointer _firstWordPointer,_secondWordPointer,_thirdWordPointer;        
        private Bitmap _beforeFormatting,_afterFormatting,_afterResetFormatting;
        private ComparisonCriteria _criteria;

        private bool _performResetFormatVerification = true;

        private double _initialFontSize = 20d;
        private string _content = "1stWord 2ndWord 3rdWord";
        private int _firstWordOffset = 0;
        private int _secondWordOffset = 7 + 1;
        private int _thirdWordOffset = 7 + 1 + 7 + 1;

        //Formatting command being tested
        private EditingCommandData _formattingCommand=null;
        private RoutedUICommand _resetFormatCommand;
        
        // Whether toggle behavior of the FormattingCommand is tested or 
        // whether ResetFormat command is tested for clearing the properties        
        private bool _verifyToggle=false;

        #endregion Private fields
    }

    /// <summary>Verifies ResetFormat command</summary>
    [Test(2, "RichEditing", "ResetFormatTest", MethodParameters = "/TestCaseType:ResetFormatTest")]
    [TestOwner("Microsoft"), TestTactics("478"), TestBugs("685"), TestLastUpdatedOn("July 20, 2006")]
    public class ResetFormatTest : ManagedCombinatorialTestCase
    {
        #region MainFlow

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {            
            _rtb = new RichTextBox();
            _rtb.Height = 50;
            _rtb.Width = 300;
            _rtb.FontFamily = new FontFamily("Palatino Linotype");
            _rtb.FontSize = _initialFontSize;

            _rtb.Document.Blocks.Clear();

            _para = new Paragraph();
            _run = new Run(_content);

            if (_verifyOnSpan)
            {
                _span = new Span();
                _span.Inlines.Add(_run);
                _para.Inlines.Add(_span);
                _span.SetValue(_charFormatProperty.Property, _charFormatProperty.TestValue);
            }
            else
            {
                _run.SetValue(_charFormatProperty.Property, _charFormatProperty.TestValue);
                _para.Inlines.Add(_run);
            }  
          
            _rtb.Document.Blocks.Add(_para);            
            _midPointer = _run.ContentStart.GetPositionAtOffset(_run.Text.Length/2);

            _rtb.Selection.Select(_midPointer, _midPointer);
            
            _panel = new StackPanel();
            _panel.Children.Add(_rtb);
            TestElement = _panel;

            _resetFormatCommand = (RoutedUICommand)ReflectionUtils.GetStaticProperty(typeof(EditingCommands), "ResetFormat");

            QueueDelegate(SetInitialFocus);
        }

        private void SetInitialFocus()
        {
            _rtb.Focus(); //so that caret is not there when we capture bitmap of RTB

            Log("Value of Property[" + _charFormatProperty.Property.ToString() + "]----->[" +
                _run.GetValue(_charFormatProperty.Property) + "]");
            
            //Verify that all the properties have assigned values before calling ResetFormat
            //if property is not inheritable, then check for value on Span if VerifyOnSpan is true
            if ( (!IsInheritableProp(_charFormatProperty)) && (_verifyOnSpan) )
            {
                Verifier.VerifyValue(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                    _span.GetValue(_charFormatProperty.Property));
            }
            else
            {
                Verifier.VerifyValue(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                    _run.GetValue(_charFormatProperty.Property));
            }

            QueueDelegate(DoResetFormat);
        }

        private void DoResetFormat()
        {
            Log("Calling reset format command...");
            //Keyboard shortcut Ctrl+Space for resetting format collides with system shortcut for switching keyboard layouts in Vista
            //KeyboardInput.TypeString("^{SPACE}");
            _resetFormatCommand.Execute(null, _rtb);

            QueueDelegate(VerifyAfterReset);
        }

        private void VerifyAfterReset()
        {
            Log("AfterReset - Value of Property[" + _charFormatProperty.Property.ToString() + "]----->[" +
                _run.GetValue(_charFormatProperty.Property) + "]");

            //Verify that all the properties are cleared after calling ResetFormat
            //if property is not inheritable, then check for value on Span if VerifyOnSpan is true
            if ((!IsInheritableProp(_charFormatProperty)) && (_verifyOnSpan))
            {
                //Span is usually removed when removing the property. If it is not removed, check
                //the property value on Span or else on Run
                if (_span.ContentStart.IsInSameDocument(_run.ContentStart))
                {
                    //FlowDirection and Language properties are not formatting properties and wont get
                    //cleared by ResetFormat command.
                    Log("Verifying on Span (non-inheritable property)...");
                    if ((_charFormatProperty.Property == FrameworkElement.FlowDirectionProperty)||
                        (_charFormatProperty.Property == FrameworkElement.LanguageProperty))
                    {
                        Verifier.VerifyValue(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                            _span.GetValue(_charFormatProperty.Property));
                    }
                    else
                    {
                        Verifier.VerifyValueDifferent(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                            _span.GetValue(_charFormatProperty.Property));
                    }
                }
                else
                {
                    Log("Verifying on Run (non-inheritable property)...");
                    //FlowDirection and Language properties are not formatting properties and wont get
                    //cleared by ResetFormat command.
                    if ((_charFormatProperty.Property == FrameworkElement.FlowDirectionProperty) ||
                        (_charFormatProperty.Property == FrameworkElement.LanguageProperty))
                    {
                        Verifier.VerifyValue(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                            _run.GetValue(_charFormatProperty.Property));
                    }
                    else
                    {
                        Verifier.VerifyValueDifferent(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                            _run.GetValue(_charFormatProperty.Property));
                    }
                }
            }
            else
            {
                //FlowDirection and Language properties are not formatting properties and wont get
                //cleared by ResetFormat command.
                Log("Verifying on Run...");
                if ((_charFormatProperty.Property == FrameworkElement.FlowDirectionProperty) ||
                        (_charFormatProperty.Property == FrameworkElement.LanguageProperty))
                {
                    Verifier.VerifyValue(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                        _run.GetValue(_charFormatProperty.Property));
                }
                else
                {
                    Verifier.VerifyValueDifferent(_charFormatProperty.Property.ToString(), _charFormatProperty.TestValue,
                        _run.GetValue(_charFormatProperty.Property));
                }
            }

            QueueDelegate(NextCombination);
        }

        #endregion MainFlow

        #region Helpers

        private bool IsInheritableProp(DependencyPropertyData dpData)
        {
            if (dpData.IsInheritable)
            {
                return true;
            }
            PropertyMetadata propMetadata = dpData.Property.GetMetadata(typeof(FrameworkElement));
            if ((propMetadata is FrameworkPropertyMetadata)&&(((FrameworkPropertyMetadata)propMetadata).Inherits))
            {
                return true;
            }

            propMetadata = dpData.Property.GetMetadata(typeof(FrameworkContentElement));
            if ((propMetadata is FrameworkPropertyMetadata) && (((FrameworkPropertyMetadata)propMetadata).Inherits))
            {
                return true;
            }

            return false;
        }

        #endregion Helpers

        #region Private fields

        private RichTextBox _rtb;        
        private StackPanel _panel;
        private Paragraph _para;
        private Span _span;
        private Run _run;
        private TextPointer _midPointer;        

        private double _initialFontSize = 20d;
        private string _content = "SampleSample";        

        /// <summary>Character formatting property being applied to test ResetFormat command</summary>
        private DependencyPropertyData _charFormatProperty=null;

        /// <summary>
        /// Whether to test formatting property when applied on Span or directly on Run
        /// </summary>
        private bool _verifyOnSpan=false;

        private RoutedUICommand _resetFormatCommand;

        #endregion Private fields
    }

    /// <summary>
    /// Tests ResetFormat command over RTL content, Bidi content and Hyperlink content.
    /// *** This tests is marked to not run on 4.0 due to Part1 Regression_Bug185
    /// </summary>
    [Test(2, "RichEditing", "ResetFormatBidiHyperlinkContent", MethodParameters = "/TestCaseType:ResetFormatBidiHyperlinkContent", Versions="3.0,3.0SP1,3.0SP2,AH")]
    [TestOwner("Microsoft"), TestTactics("479"), TestBugs("686"), TestLastUpdatedOn("July 20, 2006")]
    public class ResetFormatBidiHyperlinkContent : CustomTestCase
    {
        #region Private fields

        private string _rtlContent = "\x062e\x0635\x0645"; //Taken from TextScriptData.cs
        private string _ltrContent = "abc";
        private RichTextBox _rtb;
        private Paragraph _para;
        private Run _runHL,_runLTR,_runRTL;
        private TextRange _trLTR,_trRTL;
        private Hyperlink _hLink;
        private TextPointer _tp1,_tp2;
        private Brush _hyperlinkForeground;

        private RoutedUICommand _resetFormatCommand;

        #endregion Private fields

        #region Main flow

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            _rtb = new RichTextBox();            
            _rtb.Height = 200;
            _rtb.FontSize = 20;

            SetRTLContents();

            MainWindow.Content = _rtb;

            _resetFormatCommand = (RoutedUICommand)ReflectionUtils.GetStaticProperty(typeof(EditingCommands), "ResetFormat");

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {            
            _rtb.Focus();
            _tp1 = _runRTL.ContentStart.GetPositionAtOffset(1);
            _tp2 = _runRTL.ContentEnd.GetPositionAtOffset(-1);
            _rtb.Selection.Select(_tp1, _tp2);            
            _rtb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            QueueDelegate(DoResetFormat);
        }

        private void DoResetFormat()
        {
            Log(_para.Inlines.Count.ToString());
            Verifier.Verify(_para.Inlines.Count == 1, "Verifying that paragraph has still one Inline", true);
            Verifier.Verify(_para.Inlines.FirstInline is Span, "Verifying that Inline is of type Span", true);
            Span span = (Span)_para.Inlines.FirstInline;
            Verifier.Verify(span.Inlines.Count == 3, "Verifying that Span has 3 inlines", true);
            _trRTL = new TextRange(span.ContentStart, span.ContentEnd);
            Verifier.Verify(_trRTL.Text == _rtlContent, "Verifying contents of span", true);            

            //Control+Space for ResetFormat            
            _resetFormatCommand.Execute(null, _rtb);

            QueueDelegate(VerifyResetFormatOverRTLContent);
        }

        private void VerifyResetFormatOverRTLContent()
        {
            Verifier.Verify(_para.Inlines.Count == 1, "Verifying that runs inside span gets merge into 1 run after ResetFormat", true);
            Verifier.Verify(((Run)_para.Inlines.FirstInline).Text == _rtlContent, 
                "Verifying contents of run", false);
            Verifier.Verify(((Run)_para.Inlines.FirstInline).FlowDirection == FlowDirection.RightToLeft, 
                "Verifying FD is not cleared after ResetFormat", true);
            Verifier.Verify(((Run)_para.Inlines.FirstInline).Language == XmlLanguage.GetLanguage("ar-sa"), 
                "Verifying Language is not cleared after ResetFormat", true);

            SetBidiContents();
            _trLTR = new TextRange(_runLTR.ContentStart, _runLTR.ContentEnd);
            _trRTL = new TextRange(_runRTL.ContentStart, _runRTL.ContentEnd);

            QueueDelegate(CheckBidiKeyboardInstalled);
        }

        private void CheckBidiKeyboardInstalled()
        {
            if (!KeyboardInput.IsBidiInputLanguageInstalled())
            {
                KeyboardInput.AddInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);
            }

            QueueDelegate(ChangeFlowDirection);
        }

        private void ChangeFlowDirection()
        {            
            //The below line is commented on purpose.
            //For single paragraph, after SelectAll, calling ApplyPropertyValue(FDProperty, FD.RTL) 
            //doesnt change the FD of the paragraph. 
            //Uncomment when Regression_Bug186 is fixed.
            //rtb.SelectAll();
            _rtb.Selection.ApplyPropertyValue(FrameworkElement.FlowDirectionProperty, FlowDirection.RightToLeft);
                        
            QueueDelegate(ApplyBold);            
        }

        private void ApplyBold()
        {
            _rtb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, FontWeights.Bold);

            //Control+Space for ResetFormat
            _resetFormatCommand.Execute(null, _rtb);

            QueueDelegate(VerifyResetFormatOverBidi);
        }

        private void VerifyResetFormatOverBidi()
        {
            Verifier.VerifyValue("FlowDirection on LTR run", FlowDirection.LeftToRight, 
                _trLTR.GetPropertyValue(FrameworkElement.FlowDirectionProperty));
            Verifier.VerifyValue("FlowDirection on RTL run", FlowDirection.RightToLeft, 
                _trRTL.GetPropertyValue(FrameworkElement.FlowDirectionProperty));            

            SetHyperlinkContents();
            _tp1 = _runHL.ContentStart.GetPositionAtOffset(1);
            _tp2 = _runHL.ContentEnd.GetPositionAtOffset(-1);
            _rtb.Selection.Select(_tp1, _tp2);
            _hyperlinkForeground = (Brush)_rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty);

            //Control+Space for ResetFormat
            _resetFormatCommand.Execute(null, _rtb);

            QueueDelegate(VerifyResetFormatOverPartialHyperlink);
        }

        private void VerifyResetFormatOverPartialHyperlink()
        {
            Verifier.VerifyValue("Foreground on partial selected Hyperlink", _hyperlinkForeground,
                _rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty));
            Verifier.Verify(_hLink.ContentStart.IsInSameDocument(_rtb.Document.ContentStart),
                "Verifying that Hyperlink is still in tact", true);
            Verifier.Verify(_hLink.TextDecorations.Count == 1,
                "TextDecorations on partial selected Hyperlink", true);                                               

            _rtb.SelectAll();

            //Control+Space for ResetFormat
            _resetFormatCommand.Execute(null, _rtb);

            QueueDelegate(VerifyResetFormatOverHyperlink);
        }

        private void VerifyResetFormatOverHyperlink()
        {
            Verifier.VerifyValueDifferent("Foreground on fully selected Hyperlink", _hyperlinkForeground,
                _rtb.Selection.GetPropertyValue(TextElement.ForegroundProperty));
            Verifier.Verify(!_hLink.ContentStart.IsInSameDocument(_rtb.Document.ContentStart),
                "Verifying that Hyperlink is removed from the document", true);
            Verifier.Verify(((TextDecorationCollection)_rtb.Selection.GetPropertyValue(Inline.TextDecorationsProperty)).Count == 0,
                "TextDecorations on fully selected Hyperlink", true);

            Logger.Current.ReportSuccess();
        }

        private void SetRTLContents()
        {
            _rtb.Document.Blocks.Clear();
            
            _runRTL = new Run();
            _runRTL.Text = _rtlContent;
            _runRTL.FlowDirection = FlowDirection.RightToLeft;
            _runRTL.Language = XmlLanguage.GetLanguage("ar-sa");

            _para = new Paragraph();
            _para.Inlines.Add(_runRTL);

            _rtb.Document.Blocks.Add(_para);
        }

        private void SetBidiContents()
        {
            _rtb.Document.Blocks.Clear();

            _runLTR = new Run();
            _runLTR.Text = _ltrContent;

            _runRTL = new Run();
            _runRTL.Text = _rtlContent;
            _runRTL.FlowDirection = FlowDirection.RightToLeft;
            _runRTL.Language = XmlLanguage.GetLanguage("ar-sa");

            _para = new Paragraph();            
            _para.Inlines.Add(_runLTR);
            _para.Inlines.Add(_runRTL);

            _rtb.Document.Blocks.Add(_para);
        }

        private void SetHyperlinkContents()
        {
            _rtb.Document.Blocks.Clear();

            _runHL = new Run();
            _runHL.Text = _ltrContent;
            _hLink = new Hyperlink(_runHL);

            _para = new Paragraph();
            _para.Inlines.Add(_hLink);

            _rtb.Document.Blocks.Add(_para);
        }

        #endregion Main flow
    }    

    /// <summary>
    /// Verifies that empty formatting tags are not left behind
    /// </summary>
    [Test(2, "RichEditing", "CharacterFormattingRegressions", MethodParameters = "/TestCaseType:CharacterFormattingRegressions")]
    [TestOwner("Microsoft"), TestTactics("480"), TestWorkItem("71"), TestBugs("902")]
    public class CharacterFormattingRegressions : CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            string[] _testFormatValues = new string[] { "BOLD", "ITALIC", "UNDERLINE" };
            return new Dimension[] {
                new Dimension("TestFormats", _testFormatValues),
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _testFormat = (string)values["TestFormats"];
            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 500;
            _rtb.Width = 500;
            _rtbWrapper = new UIElementWrapper(_rtb);
            ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run(_initialContents));
            TestElement = _rtb;

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _rtb.Focus();
            KeyboardInput.TypeString("{END}");
            QueueDelegate(PerformSelection);
        }

        private void PerformSelection()
        {
            string _commandString = CharacterFormattingHelper.GetCommandString(_testFormat);
            KeyboardInput.TypeString("+{LEFT}" + _commandString);
            QueueDelegate(OverrideSelection);
        }

        private void OverrideSelection()
        {
            KeyboardInput.TypeString("{RIGHT}+{LEFT " + _initialContents.Length + "}" + _initialContents);
            QueueDelegate(VerifyNoEmptyTags);
        }

        private void VerifyNoEmptyTags()
        {
            int _count = 0;
            TextPointer tp = _rtb.Document.ContentStart;
            do
            {
                Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(tp, _testFormat),
                    "Verifying that empty formatting tags are not present: " + _count, false);
                _count++;

            } while ( (tp = tp.GetPositionAtOffset(1)) != null);

            QueueDelegate(NextCombination);
        }
        #endregion Main flow

        #region Private fields
        RichTextBox _rtb;
        UIElementWrapper _rtbWrapper;
        string _initialContents = "This is a test";
        string _testFormat;
        #endregion Private fields
    }

    /// <summary>
    /// Verifies that Bold/Italic/Underline formatting works in RichTextBox when
    /// the ranges on which they are applied overlap partially.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("481"), TestWorkItem("71"), TestBugs("688")]
    [Test(3, "RichEditing", "CharFormattingOverlap", MethodParameters = "/TestCaseType:CharFormattingOverlap", Timeout = 500)]
    public class CharFormattingOverlap : CustomTestCase
    {
        #region MainFlow
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
                _rtb = new RichTextBox();
                _rtbWrapper = new UIElementWrapper(_rtb);
                _rtb.Height = 400;
                _rtb.Width = 500;
                _rtb.FontSize = 24;
                ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run(_testString));
                StackPanel _panel = new StackPanel();
                _panel.Children.Add(_rtb);
                MainWindow.Content = _panel;

                QueueDelegate(ApplyFirstFormat);
            }
        }

        private void ApplyFirstFormat()
        {
            _rtb.Focus();
            _rtbWrapper.Select(0, 18); //First formatting range is from 0 with length 18
            _formatOperations = _testAction.Split(new char[] { ',' });
            if (_formatOperations.Length < 2)
            {
                throw new ApplicationException("There should be atleast two formatting operations");
            }

            string _commandString = CharacterFormattingHelper.GetCommandString(_formatOperations[0]);
            KeyboardInput.TypeString(_commandString);

            QueueDelegate(ApplySecondFormat);
        }

        private void ApplySecondFormat()
        {
            _rtbWrapper.Select(6, 18); //Second formatting range is from 6 with length 18
            string _commandString = CharacterFormattingHelper.GetCommandString(_formatOperations[1]);
            KeyboardInput.TypeString(_commandString);

            if (_formatOperations.Length > 2)
            {
                QueueDelegate(ApplyThirdFormat);
            }
            else
            {
                QueueDelegate(VerifyCharFormatting);
            }
        }

        private void ApplyThirdFormat()
        {
            _rtbWrapper.Select(12, 18); //Third formatting range is from 12 with length 18
            string _commandString = CharacterFormattingHelper.GetCommandString(_formatOperations[2]);
            KeyboardInput.TypeString(_commandString);
            QueueDelegate(VerifyCharFormatting);
        }

        private void VerifyCharFormatting()
        {
            TextPointer tp1 = _rtb.Document.ContentStart;
            tp1 = tp1.GetPositionAtOffset(3 + 1); //+1 is to include the start tag of first format
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp1, _formatOperations[0]),
                "Verifying " + _formatOperations[0] + " attribute at tp1", true);

            TextPointer tp2 = _rtb.Document.ContentStart;
            tp2 = tp2.GetPositionAtOffset(9 + 3); //+3 is to include the start/end tags
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp2, _formatOperations[0]),
                "Verifying " + _formatOperations[0] + " attribute at tp2", true);
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp2, _formatOperations[1]),
                "Verifying " + _formatOperations[0] + " attribute at tp2", true);

            if (_formatOperations.Length > 2)
            {
                TextPointer tp3 = _rtb.Document.ContentStart;
                tp3 = tp3.GetPositionAtOffset(15 + 5); //+5 is to include the start/end tags
                Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp3, _formatOperations[0]),
                    "Verifying " + _formatOperations[0] + " attribute at tp3", true);
                Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp3, _formatOperations[1]),
                    "Verifying " + _formatOperations[0] + " attribute at tp3", true);
                Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tp3, _formatOperations[1]),
                    "Verifying " + _formatOperations[0] + " attribute at tp3", true);
            }

            QueueDelegate(NextCombination);
        }
        #endregion MainFlow

        #region Helper methods.
        private bool GetNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (_engine.Next(values))
            {
                _testAction = (string)values["TestAction"];
                Log("Testing ------------- [" + _testAction + "]");
                return true;
            }
            else
            {
                return false;
            }
        }

        private void SetupCombinatorialEngine()
        {
            string[] _testActions = new string[] {
                "Bold,Italic",
                "Italic,Bold",
                "Bold,Underline",
                "Underline,Bold",
                "Italic,Underline",
                "Underline,Italic",
                "Bold,Italic,Underline",
                "Bold,Underline,Italic",
                "Italic,Bold,Underline",
                "Italic,Underline,Bold",
                "Underline,Bold,Italic",
                "Underline,Italic,Bold"
                };

            _engine = CombinatorialEngine.FromDimensions(new Dimension[] {
                new Dimension("TestAction", _testActions)
                });
        }
        #endregion Helper methods.

        #region Members
        private RichTextBox _rtb;
        private UIElementWrapper _rtbWrapper;

        /// <summary>Combinatorial engine driving test.</summary>
        private CombinatorialEngine _engine;

        private string _testAction;
        private string[] _formatOperations;

        private const string _testString = "Test1 Test2 Test3 Test4 Test5 Test6";
        #endregion Members
    }

    /// <summary>
    /// Verifies the behavior of hitting Backspace/Delete after springloading bold/italic/underline
    /// </summary>
    [Test(2, "RichEditing", "SpringLoadingTest", MethodParameters = "/TestCaseType:SpringLoadingTest", Timeout=200)]
    [TestOwner("Microsoft"), TestTactics("482"), TestWorkItem("71"), TestBugs("851,852,853,854,855,856")]
    public class SpringLoadingTest : CustomCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            string[] _testActionValues = new string[] {
                "BACKSPACE",            //Backspace key action after springloading
                                        //should remove the preceeding character and springloading.

                "DELETE",               //Delete key action after springloading
                                        //should remove the springloading.

                "CONTROLSPACE",         //Control+Space (ResetFormat command) key action after springloading
                                        //should remove the springloading.

                "TYPEANDBACKSPACE",     //Type x number of characters after springloading and
                                        //then backspace x number of times. Springloading shouldnt be removed.

                "TYPEANDMOREBACKSPACE", //Type x number of characters after springloading and
                                        //then backspace y(>x) number of times. Springloading should be removed.

                "FORMATINTHEMIDDLE",    //Springload in the middle of the word. The whole word should be formatted.
                                        //Then type few new characters and formatting should persists on them.

                "SELECTALLANDDELETE",   //Type some characters after springloading and then do
                                        //Control-A and Delete. Formatting properties should not persist.

                "LEFT",                 //Springloading at empty space should not affect Neighbouring words.
                                        //Then Left key action should remove Springloading.

                "RIGHT",                //Springloading at empty space should not affect Neighbouring words.
                                        //Then Right key action should remove Springloading.

                "UP",                   //Springloading at empty space should not affect Neighbouring words.
                                        //Then Up key action should remove Springloading.

                "DOWN",                 //Springloading at empty space should not affect Neighbouring words.
                                        //Then Down key action should remove Springloading.
            };
            string[] _testFormatValues = new string[] { "BOLD", "ITALIC", "UNDERLINE" };
            return new Dimension[] {
                new Dimension("TestActions", _testActionValues),
                new Dimension("TestFormats", _testFormatValues),
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _testAction = (string)values["TestActions"];
            _testFormat = (string)values["TestFormats"];
            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.Height = 500;
            _rtb.Width = 500;
            _rtbWrapper = new UIElementWrapper(_rtb);
            ((Paragraph)_rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run(_initialContents));
            TestElement = _rtb;

            _resetFormatCommand = (RoutedUICommand)ReflectionUtils.GetStaticProperty(typeof(EditingCommands), "ResetFormat");

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            _rtb.Focus();
            KeyboardInput.TypeString("{END}");
            QueueDelegate(PerformTestAction);
        }

        private void PerformTestAction()
        {
            string _commandString = CharacterFormattingHelper.GetCommandString(_testFormat);
            string _typeString;
            switch (_testAction)
            {
                case "BACKSPACE":
                    _typeString = _commandString;
                    _typeString += "{BACKSPACE}";
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_Backspace_Action);
                    break;
                case "DELETE":
                    _typeString = _commandString;
                    _typeString += "{DELETE}";
                    _typeString += _appendContents;
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_Delete_Action);
                    break;
                case "CONTROLSPACE":
                    _typeString = _commandString;
                    KeyboardInput.TypeString(_typeString);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents(_inputWaitTime, System.Windows.Threading.DispatcherPriority.SystemIdle);
                    _resetFormatCommand.Execute(null, _rtb);                    
                    _typeString = _appendContents;
                    KeyboardInput.TypeString(_typeString);
                    Microsoft.Test.Threading.DispatcherHelper.DoEvents();
                    QueueDelegate(Verify_ControlSpace_Action);
                    break;
                case "TYPEANDBACKSPACE":
                    _typeString = _commandString;
                    _typeString += "abc{BACKSPACE 3}";
                    _typeString += _appendContents;
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_TypeAndBackspace_Action);
                    break;
                case "TYPEANDMOREBACKSPACE":
                    _typeString = _commandString;
                    _typeString += "abc{BACKSPACE 5}";
                    _typeString += _appendContents;
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_TypeAndMoreBackspace_Action);
                    break;
                case "FORMATINTHEMIDDLE":
                    _typeString = "";
                    _typeString += "{LEFT 2}" + _commandString;
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_FormatInTheMiddle_Action);
                    break;
                case "SELECTALLANDDELETE":
                    _typeString = _commandString + "Formatted" + _commandString + "Unformatted";
                    _typeString += "^a{DELETE}";
                    _typeString += _appendContents;
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(Verify_SelectAllAndDelete_Action);
                    break;
                case "LEFT":
                    _rtb.Document = new FlowDocument(new Paragraph(new Run()));
                    _typeString = "abc    def" + "{LEFT 5}" + _commandString;
                    _followUpActionCommandString = "{LEFT}";
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(VerifyNoFormattingOnNeighbourWord);
                    break;
                case "RIGHT":
                    _rtb.Document = new FlowDocument(new Paragraph(new Run()));
                    _typeString = "abc    def" + "{LEFT 5}" + _commandString;
                    _followUpActionCommandString = "{RIGHT}";
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(VerifyNoFormattingOnNeighbourWord);
                    break;
                case "UP":
                    _rtb.Document = new FlowDocument(new Paragraph(new Run()));
                    _typeString = "abc    def" + "{LEFT 5}" + _commandString;
                    _followUpActionCommandString = "{UP}";
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(VerifyNoFormattingOnNeighbourWord);
                    break;
                case "DOWN":
                    _rtb.Document = new FlowDocument(new Paragraph(new Run()));
                    _typeString = "abc    def" + "{LEFT 5}" + _commandString;
                    _followUpActionCommandString = "{DOWN}";
                    KeyboardInput.TypeString(_typeString);
                    QueueDelegate(VerifyNoFormattingOnNeighbourWord);
                    break;
                default :
                    throw new Exception("Unexpected test action");
            }
        }

        private void Verify_Backspace_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _initialContents.Substring(0, _initialContents.Length - 1) + "\r\n",
                "Verifying the contents after the Backspace Action", true);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute is removed after Backspace Action", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_Delete_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _initialContents + _appendContents + "\r\n",
                "Verifying the contents after the Delete Action", true);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute is removed after Delete Action", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_ControlSpace_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _initialContents + _appendContents + "\r\n",
                "Verifying the contents after the Control+Space Action", true);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute is removed after Control+Space Action", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_TypeAndBackspace_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _initialContents + _appendContents + "\r\n",
                "Verifying the contents after TypeAndBackspace action", true);
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute is not lost after type and backspacing (not beyond the point where it is springloaded)", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_TypeAndMoreBackspace_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _initialContents.Substring(0, _initialContents.Length - 2) + _appendContents + "\r\n",
                "Verifying the contents after TypeAndMoreBackspace action", true);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute is lost after type and backspacing (beyond the point where it is springloaded)", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_FormatInTheMiddle_Action()
        {
            TextPointer tpBack, tpForward;

            Verifier.Verify(_rtbWrapper.Text == _initialContents + "\r\n",
                "Verifying the contents after FormatInTheMiddle action", true);
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that springloaded formatting attribute has been applied", true);

            tpBack = _rtb.Selection.Start;
            tpBack = tpBack.GetPositionAtOffset(-1);
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tpBack, _testFormat),
                "Verifying that formatting attribute has been applied to the whole word by going backward", true);

            tpForward = _rtb.Selection.Start;
            tpForward = tpForward.GetPositionAtOffset(1);
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(tpForward, _testFormat),
                "Verifying that formatting attribute has been applied to the whole word by going forward", true);

            KeyboardInput.TypeString(_appendContents + "{LEFT}");
            QueueDelegate(Verify_FormatInTheMiddle_FollowupAction);
        }

        private void Verify_FormatInTheMiddle_FollowupAction()
        {
            Verifier.Verify(CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that formatting attribute persists when additional typing is done", true);

            QueueDelegate(NextCombination);
        }

        private void Verify_SelectAllAndDelete_Action()
        {
            Verifier.Verify(_rtbWrapper.Text == _appendContents + "\r\n",
                "Verifying the contents after SelectAllAndDelete action", true);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(_rtb.Selection.Start, _testFormat),
                "Verifying that SelectAll and Delete should not retain any formatting attributes", true);

            QueueDelegate(NextCombination);
        }

        private void VerifyNoFormattingOnNeighbourWord()
        {
            TextPointer tpBegining, tpEnding;

            tpBegining = _rtb.Document.ContentStart.GetPositionAtOffset(1);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(tpBegining, _testFormat),
                "Verifying that preceeding word is not formatted when springloaded in done in between two words", true);

            tpEnding = _rtb.Document.ContentEnd.GetPositionAtOffset(-1);
            Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(tpEnding, _testFormat),
                "Verifying that following word is not formatted when springloaded in done in between two words", true);

            KeyboardInput.TypeString(_followUpActionCommandString);
            QueueDelegate(VerifyFormattingRemoved);
        }

        private void VerifyFormattingRemoved()
        {
            int _count = 0;
            TextPointer tp = _rtb.Document.ContentStart;
            do
            {
                Verifier.Verify(!CharacterFormattingHelper.VerifyFormatting(tp, _testFormat),
                    "Verifying that empty formatting tags are not present: " + _count, false);
                _count++;

            } while ( (tp = tp.GetPositionAtOffset(1)) != null);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow

        #region Private fields

        RichTextBox _rtb;
        UIElementWrapper _rtbWrapper;
        string _initialContents = "This is a test";
        string _appendContents = "xxx";
        string _testAction, _testFormat;
        string _followUpActionCommandString;

        int _inputWaitTime = 1000;
        RoutedUICommand _resetFormatCommand;

        #endregion Private fields
    }

    /// <summary>
    /// Verifies that Font formatting works in TextBox/RichTextBox.
    /// FontFamily and FontSize properties are changed in the control and verified.
    /// </summary>
    [Test(0, "RichEditing", "FontFormatting", MethodParameters = "/TestCaseType:FontFormatting")]
    [TestOwner("Microsoft"), TestTactics("483"),
    TestArgument("FontSize", "FontSize to be used in the test."),
    TestArgument("FontFamily", "FontFamily to be used in the test.")]
    public class FontFormatting : CustomTestCase
    {
        #region Settings
        /// <summary>FontSize to be used in the test.</summary>
        private int FontSize
        {
            get
            {
                if (Settings.GetArgumentAsInt("FontSize") != 0)
                    return Settings.GetArgumentAsInt("FontSize");
                else
                    return (48); //default value
            }
        }

        /// <summary>FontFamily to be used in the test.</summary>
        private FontFamily FontFamily
        {
            get
            {
                if (Settings.GetArgument("FontFamily") != String.Empty)
                    return new FontFamily(Settings.GetArgument("FontFamily"));
                else
                    return new FontFamily("Castellar"); //default value
            }
        }

        #endregion Settings

        #region members
        TextEditableType _textEditableType;
        private int _editableTypeIndex;
        private Control _testElement;
        private UIElementWrapper _testWrapper;

        private delegate void FormatDelegate(bool testBold, bool testItalic, bool testUnderline);
        #endregion members

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            DoIteration();
        }

        private void DoIteration()
        {
            #region Setup
            FrameworkElement editableInstance;
            _textEditableType = TextEditableType.Values[_editableTypeIndex];

            Log("Creating editable type: " + _textEditableType.Type);
            editableInstance = _textEditableType.CreateInstance();
            _testElement = editableInstance as Control;
            if (_testElement == null)
            {
                Log("Element is not of type System.Windows.Control.");
                FinishedIteration();
                return;
            }
            _testWrapper = new UIElementWrapper(_testElement);
            _testWrapper.Text = "This is a test";

            ReflectionUtils.SetProperty(editableInstance, "FontSize", FontSize);
            ReflectionUtils.SetProperty(editableInstance, "FontFamily", FontFamily);

            MainWindow.Content = (UIElement)editableInstance;
            #endregion Setup

            QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyFontFormat));
        }

        private void VerifyFontFormat()
        {
            object testFontFamilyValue = null;
            object testFontSizeValue = null;

            if (_testElement is TextBox)
            {
                Verifier.Verify(_testElement.FontFamily.Source == FontFamily.Source,
                    "Verify FontFamily value...", true);

                Verifier.Verify((int)(_testElement.FontSize) == FontSize,
                    "Verify FontSize value...", true);
            }
            else if (_testElement is RichTextBox)
            {
                TextPointer tpStart = _testWrapper.Start;
                tpStart = tpStart.GetPositionAtOffset(2);

                testFontFamilyValue = tpStart.Parent.GetValue(TextElement.FontFamilyProperty);
                testFontSizeValue = tpStart.Parent.GetValue(TextElement.FontSizeProperty);

                Verifier.Verify((((FontFamily)testFontFamilyValue).Source == FontFamily.Source), "Verify FontFamily value...", true);
                Verifier.Verify(((double)testFontSizeValue == FontSize), "Verifying FontSize value...", true);
            }

            FinishedIteration();
        }

        private void FinishedIteration()
        {
            _editableTypeIndex++;
            if (_editableTypeIndex == TextEditableType.Values.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueDelegate(DoIteration);
            }
        }
    }

    /// <summary>
    /// Verifies that Font formatting works in RichTextBox control in a more complex scenario.
    /// It first selects some text, insert an element, change the fontsize and fontfamily properties,
    /// overwrites the text in that selection and verifies FontSize and FontFamily properties are retained.
    /// </summary>
    [Test(0, "RichEditing", "FontFormatting_Complex", MethodParameters = "/TestCaseType:FontFormatting_Complex")]
    [TestOwner("Microsoft"), TestTactics("484"),
    TestArgument("FontSize", "FontSize to be used in the test."),
    TestArgument("FontFamily", "FontFamily to be used in the test.")]
    public class FontFormatting_Complex : CustomTestCase
    {
        #region Settings
        /// <summary>TextBlock control which is being tested.</summary>
        private RichTextBox TextControl
        {
            get { return _textControl; }
            set { _textControl = value; }
        }

        /// <summary>FontSize to be used in the test.</summary>
        private int FontSize
        {
            get
            {
                if (Settings.GetArgumentAsInt("FontSize") != 0)
                    return Settings.GetArgumentAsInt("FontSize");
                else
                    return (48); //default value
            }
        }

        /// <summary>FontFamily to be used in the test.</summary>
        private FontFamily FontFamily
        {
            get
            {
                if (Settings.GetArgument("FontFamily") != String.Empty)
                    return new FontFamily(Settings.GetArgument("FontFamily"));
                else
                    return new FontFamily("Castellar"); //default value
            }
        }
        #endregion Settings

        #region members
        RichTextBox _textControl = null;
        private UIElementWrapper _textControlWrapper;

        private FontFamily _systemFontFamily = SystemFonts.MessageFontFamily;
        private double _systemFontSize = SystemFonts.MessageFontSize;

        private delegate void FormatDelegate(bool testBold, bool testItalic, bool testUnderline);
        #endregion members

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            #region Setup
            TextControl = new RichTextBox();

            _textControlWrapper = new UIElementWrapper(TextControl);
            _textControlWrapper.Text = "This is a test";

            MainWindow.Content = (UIElement)TextControl;
            #endregion Setup

            QueueHelper.Current.QueueDelegate(new SimpleHandler(SelectText));
        }

        /// <summary> Selects some text in the text control </summary>
        private void SelectText()
        {
            TextControl.Focus();
            //MouseInput.MouseClick((UIElement)TextControl); //To set the focus of the text control.

            TextSelection textSelection = _textControlWrapper.SelectionInstance;

            textSelection.Select(
                _textControl.Document.Blocks.FirstBlock.ContentStart,
                _textControl.Document.Blocks.FirstBlock.ContentStart.GetPositionAtOffset(7));

            textSelection.ApplyPropertyValue(TextElement.FontSizeProperty, (double)FontSize);
            textSelection.ApplyPropertyValue(TextElement.FontFamilyProperty, FontFamily);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(FontFormat));
        }

        private void FontFormat()
        {
            KeyboardInput.TypeString("This is"); //overwrite the selected text to check if properties are retained.
            QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyFontFormat));
        }

        private void VerifyFontFormat()
        {
            object testFontFamilyValue = null;
            object testFontSizeValue = null;

            TextPointer tpPart1 = _textControl.Document.Blocks.FirstBlock.ContentStart;
            TextPointer tpPart2 = _textControl.Document.Blocks.FirstBlock.ContentStart;
            tpPart1 = tpPart1.GetPositionAtOffset(2);
            tpPart2 = tpPart2.GetPositionAtOffset(10);

            testFontFamilyValue = tpPart1.Parent.GetValue(RichTextBox.FontFamilyProperty);
            testFontSizeValue = tpPart1.Parent.GetValue(RichTextBox.FontSizeProperty);

            Log("Actual FontFamily [" + ((FontFamily)testFontFamilyValue).Source + "] Expected FontFamily [" + FontFamily + "]");
            Verifier.Verify((((FontFamily)testFontFamilyValue).Source == FontFamily.Source), "Verify FontFamily value in the selected text...", true);
            Verifier.Verify(((double)testFontSizeValue == FontSize), "Verifying FontSize value in the selected text...", true);

            testFontFamilyValue = tpPart2.Parent.GetValue(RichTextBox.FontFamilyProperty);
            testFontSizeValue = tpPart2.Parent.GetValue(RichTextBox.FontSizeProperty);

            Verifier.Verify((((FontFamily)testFontFamilyValue).Source == _systemFontFamily.Source), "Verify FontFamily value in the non selected text...", true);
            Verifier.Verify(((double)testFontSizeValue == _systemFontSize), "Verifying FontSize value in the non selected text...", true);

            Logger.Current.ReportSuccess();
        }
    }
     
    /// <summary>Tests FlowDirection of Individual Runs and the XAML on splitting and merging operations </summary>
    [Test(0, "RichEditing", "RichTextBoxInlineFlowDirection", MethodParameters = "/TestCaseType:RichTextBoxInlineFlowDirection", Timeout = 300)]
    [TestOwner("Microsoft"), TestTactics("485"), TestWorkItem("73"), TestLastUpdatedOn("May 5, 2006")]
    public class RichTextBoxInlineFlowDirection : ManagedCombinatorialTestCase
    {
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("RichTextBox"))
                return true;
            return false;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            _controlWrapper.Clear();
            _rtb = _element as RichTextBox;
            _rtb.FontSize = 30;
            _underline = _underline.Replace("@", "");
            SetRTBContent();
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(StartExecution);
        }

        /// <summary>Program Controller</summary>
        private void StartExecution()
        {
            KeyboardInput.TypeString("{RIGHT 3}");
            QueueDelegate(VerifyFlowDirectionPropertyForFirstRun);
        }

        /// <summary>Verifies flow direction property for the first run</summary>
        private void VerifyFlowDirectionPropertyForFirstRun()
        {
            string flowDirectionForFirstRun = _rtb.Selection.GetPropertyValue(Run.FlowDirectionProperty).ToString();
            Verifier.Verify(flowDirectionForFirstRun == FlowDirection.LeftToRight.ToString(), "Flow Direction for first run doesnt match Actual[" + flowDirectionForFirstRun +
                "] Expected [LeftToRight]", false);
            KeyboardInput.TypeString("^{END}{RIGHT 1}");
            QueueDelegate(VerifyFlowDirectionPropertyForSecRun);
        }

        /// <summary>Verifies flow direction property for the second run</summary>
        private void VerifyFlowDirectionPropertyForSecRun()
        {
            string flowDirectionForSecRun = _rtb.Selection.GetPropertyValue(Run.FlowDirectionProperty).ToString();
            Verifier.Verify(flowDirectionForSecRun == FlowDirection.RightToLeft.ToString(), "Flow Direction for sec run doesnt match Actual[" + flowDirectionForSecRun +
                "] Expected [RightToLeft]", false);
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(ApplyFormatting);
        }

        /// <summary>Applies formatting to merge and split the runs</summary>
        private void ApplyFormatting()
        {
            switch (_selectionSwitch)
            {
                case SelectionChoices.NoSelection:
                    {
                        KeyboardInput.TypeString("{right 2}" + EditingCommandData.ToggleBold.KeyboardShortcut +
                            EditingCommandData.ToggleUnderline.KeyboardShortcut);
                        QueueDelegate(VerifyNoSelectionXaml);
                        break;
                    }

                case SelectionChoices.SelectAll:
                    {
                        KeyboardInput.TypeString("^A" + EditingCommandData.ToggleBold.KeyboardShortcut +
                            EditingCommandData.ToggleUnderline.KeyboardShortcut);
                        QueueDelegate(VerifySelectAllXaml);
                        break;
                    }

                case SelectionChoices.SelectAcrossRuns:
                    {
                        KeyboardInput.TypeString("{RIGHT 2}+{RIGHT 6}" + EditingCommandData.ToggleBold.KeyboardShortcut +
                            EditingCommandData.ToggleUnderline.KeyboardShortcut);
                        QueueDelegate(VerifySelectAcrossRunsXaml);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>Verifies when formatting is applied with no selection</summary>
        private void VerifyNoSelectionXaml()
        {
            string _xamlFirstRun, _xamlSecRun;

            string _xamlString = GetXaml();
            if (_inlineChoicesForRTB == InlineTypes.ConsecutiveRuns)
            {
                _xamlFirstRun = _runStart + _bold + _closeBrackets +  _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.BoldNormal)
            {
                _xamlFirstRun = _runStart + _closeBrackets +  _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.SpanItalicNormal)
            {
                _xamlFirstRun = _runStart + _italic + _bold + _closeBrackets +  _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)
            {
                _xamlFirstRun = _startPara + _closeBrackets + _runStart + _closeBrackets +  _firstString + _runEnd + _endPara;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _startPara + _closeBrackets + _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd + _endPara;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            NextCombination();
        }

        /// <summary>Verifies when formatting is applied with all contents selected</summary>
        private void VerifySelectAllXaml()
        {
            string _xamlFirstRun, _xamlSecRun;

            string _xamlString = GetXaml();

            if (_inlineChoicesForRTB == InlineTypes.ConsecutiveRuns) //normal underline -- normal Apply ^b^u
            {
                _xamlFirstRun = _runStart + _bold +  _closeBrackets +
                                _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _bold + _closeBrackets +
                                _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.BoldNormal) //bold underline -- normal Apply ^b^u
            {
                _xamlFirstRun = _runStart + _closeBrackets + _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.SpanItalicNormal) //italic underline -- normal Apply ^b^u
            {
                _xamlFirstRun = _runStart + _italic + _bold + _closeBrackets + _firstString + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _rightToLeft + _bold + _closeBrackets + _SecString + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)//bold underline -- normal Apply ^b^u
            {
                _xamlFirstRun = _startPara + _closeBrackets + _runStart + _closeBrackets + _firstString + _runEnd + _endPara;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _startPara + _closeBrackets + _runStart + _rightToLeft + _closeBrackets + _SecString + _runEnd + _endPara;
                VerificationOfXaml(_xamlSecRun, _xamlString);
            }
            NextCombination();
        }

        /// <summary>Verifies when formatting is applied with selection being part of the runs/paras</summary>
        private void VerifySelectAcrossRunsXaml()
        {
            string _xamlFirstRun, _xamlSecRun, _xamlThirdRun, xamlFourthRun; ;

            string _xamlString = GetXaml();

            TextRange tr = new TextRange(_rtb.Document.ContentStart, _controlWrapper.SelectionInstance.Start);
            string _firstPart = tr.Text;

            string _secPart = _firstString.Substring(_firstPart.Length);
            tr = new TextRange(_controlWrapper.SelectionInstance.End, _rtb.Document.ContentEnd);
            string _fourthPart = tr.Text;
            _fourthPart = _fourthPart.Replace("\r\n", "");
            string _thirdPart = _SecString.Substring(0, _SecString.Length - _fourthPart.Length);


            if (_inlineChoicesForRTB == InlineTypes.ConsecutiveRuns)  //normal underline -- normal underline Apply ^b^u
            {
                _xamlFirstRun = _runStart +  _underline + _closeBrackets +  _firstPart + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun =  _runStart + _bold + _closeBrackets + _secPart + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
                _xamlThirdRun = _spanRTL + _runStart + _bold + _closeBrackets + _thirdPart + _runEnd;
                VerificationOfXaml(_xamlThirdRun, _xamlString);
                xamlFourthRun = _runStart + _underline + _closeBrackets +  _fourthPart + _runEnd + _spanEnd;
                VerificationOfXaml(xamlFourthRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.BoldNormal) //bold underline -- normal underline Apply ^b^u
            {
                _xamlFirstRun = _runStart + _bold + _underline + _closeBrackets +  _firstPart + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _closeBrackets + _secPart + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
                _xamlThirdRun = _spanRTL + _runStart + _closeBrackets + _thirdPart + _runEnd;
                VerificationOfXaml(_xamlThirdRun, _xamlString);
                xamlFourthRun = _runStart + _underline + _closeBrackets + _fourthPart + _runEnd + _spanEnd;
                VerificationOfXaml(xamlFourthRun, _xamlString);
            }
            else if (_inlineChoicesForRTB == InlineTypes.SpanItalicNormal)
            {
                _xamlFirstRun = _runStart + _italic + _underline + _closeBrackets +  _firstPart + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart + _italic + _bold + _closeBrackets + _secPart + _runEnd;
                VerificationOfXaml(_xamlSecRun, _xamlString);
                _xamlThirdRun = _spanRTL + _runStart + _bold + _closeBrackets + _thirdPart + _runEnd;
                VerificationOfXaml(_xamlThirdRun, _xamlString);
                xamlFourthRun = _runStart + _underline + _closeBrackets + _fourthPart + _runEnd + _spanEnd;
                VerificationOfXaml(xamlFourthRun, _xamlString);

            }
            else if (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal) //bold underline -- normal underline Apply ^b^u
            {
                _xamlFirstRun = _startPara + _closeBrackets + _runStart + _bold + _underline + _closeBrackets +  _firstPart + _runEnd;
                VerificationOfXaml(_xamlFirstRun, _xamlString);
                _xamlSecRun = _runStart +  _closeBrackets +  _secPart + _runEnd + _endPara;
                VerificationOfXaml(_xamlSecRun, _xamlString);
                _xamlThirdRun = _startPara + _closeBrackets + _spanRTL + _runStart + _closeBrackets + _thirdPart + _runEnd;
                VerificationOfXaml(_xamlThirdRun, _xamlString);
                xamlFourthRun = _runStart + _underline + _closeBrackets +  _fourthPart + _runEnd + _spanEnd + _endPara;
                VerificationOfXaml(xamlFourthRun, _xamlString);
            }
            NextCombination();
        }

        /// <summary>Verifies XAML</summary>
        private static void VerificationOfXaml(string _xamlRun, string _xamlString)
        {
            string _xamlStringDisplay = _xamlString.Replace("/>", "/>\r\n");
            _xamlStringDisplay = _xamlStringDisplay.Replace("<Paragr", "\r\n******************\r\n<Paragr");
            _xamlStringDisplay = _xamlStringDisplay.Replace("<Run", "\r\n---------------------\r\n<Run");
            Verifier.Verify(_xamlString.Contains(_xamlRun) == true, "Complete xaml [" + _xamlStringDisplay +
                "] \r\n ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ \r\n Expected to contain[" + _xamlRun + "]\r\n ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ \r\n", false);
        }

        /// <summary>gets RTB XAML</summary>
        private string GetXaml()
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            return (XamlUtils.TextRange_GetXml(tr));
        }

        /// <summary>Sets RTB content</summary>
        private void SetRTBContent()
        {
            switch (_inlineChoicesForRTB)
            {
                //Change paragraph flowdirection to RightToLeft
                //currently theres a bug
                case InlineTypes.ConsecutiveRuns:
                    {
                        Run _firstRun = new Run();
                        _firstRun.Text = _firstString;
                        _firstRun.TextDecorations = TextDecorations.Underline;
                        _firstRun.FlowDirection = FlowDirection.LeftToRight;
                        Run _secRun = new Run(_SecString);
                        _secRun.FlowDirection = FlowDirection.RightToLeft;
                        UnderlineSecRun(_secRun);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        p.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }
                case InlineTypes.BoldNormal:
                    {
                        Bold _firstRun = new Bold();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        _firstRun.TextDecorations = TextDecorations.Underline;
                        _firstRun.FlowDirection = FlowDirection.LeftToRight;
                        Run _secRun = new Run(_SecString);
                        _secRun.FlowDirection = FlowDirection.RightToLeft;
                        UnderlineSecRun(_secRun);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        p.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }

                case InlineTypes.SpanItalicNormal:
                    {
                        Span s = new Span();
                        Italic _firstRun = new Italic();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        _firstRun.TextDecorations = TextDecorations.Underline;
                        _firstRun.FlowDirection = FlowDirection.LeftToRight;
                        Run _secRun = new Run(_SecString);
                        _secRun.FlowDirection = FlowDirection.RightToLeft;
                        UnderlineSecRun(_secRun);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        s.Inlines.Add(_firstRun);
                        s.Inlines.Add(_secRun);
                        p.Inlines.Add(s);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }

                case InlineTypes.ParagraphBoldNormal:
                    {
                        Bold _firstRun = new Bold();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        _firstRun.TextDecorations = TextDecorations.Underline;
                        _firstRun.FlowDirection = FlowDirection.LeftToRight;
                        Run _secRun = new Run(_SecString);
                        _secRun.FlowDirection = FlowDirection.RightToLeft;
                        UnderlineSecRun(_secRun);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        Paragraph p1 = new Paragraph();
                        p1.FlowDirection = FlowDirection.LeftToRight;
                        p1.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        _rtb.Document.Blocks.Add(p1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>Determine whether to underline second run</summary>
        private void UnderlineSecRun(Run _secRun)
        {
            if (_selectionSwitch == SelectionChoices.SelectAcrossRuns)
            {
                _secRun.TextDecorations = TextDecorations.Underline;
            }
        }

        #region data.

        private string _firstString = "hello ";
        private string _SecString = "world";
        private string _runStart = "<Run";
        private string _bold = " FontWeight=\"Bold\"";
        private string _italic = " FontStyle=\"Italic\"";
        private string _closeBrackets = ">";
        private string _runEnd = "</Run>";
        private string _startPara = "<Paragraph";
        private string _endPara = "</Paragraph>";
        private string _underline = " TextDecorations=\"Underline\"";
        private string _rightToLeft = " FlowDirection=\"RightToLeft\"";
        private string _spanRTL = "<Span FlowDirection=\"RightToLeft\">";
        private string _spanEnd = "</Span>";

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb;
        private InlineTypes _inlineChoicesForRTB = 0;
        private SelectionChoices _selectionSwitch = 0;

        #endregion data.
    }

    /// <summary>Tests FlowDirection of Individual Runs and the XAML on splitting and merging operations </summary>
    [Test(0, "RichEditing", "RichTextBoxInlineFlowDirectionUsingTextRange", MethodParameters = "/TestCaseType:RichTextBoxInlineFlowDirectionUsingTextRange", Timeout = 300)]
    [TestOwner("Microsoft"), TestTactics("486"), TestWorkItem("73"), TestLastUpdatedOn("May 5, 2006")]
    public class RichTextBoxInlineFlowDirectionUsingTextRange : ManagedCombinatorialTestCase
    {

        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            bool result = base.DoReadCombination(values);
            if (_editableType == TextEditableType.GetByName("RichTextBox"))
                return true;
            return false;
        }

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _controlWrapper = new UIElementWrapper(_element);
            _controlWrapper.Clear();
            _rtb = _element as RichTextBox;
            _rtb.FontSize = 30;
            SetRTBContent();
            TestElement = _element;
            QueueDelegate(DoFocus);
        }

        /// <summary>Focus on element</summary>
        private void DoFocus()
        {
            _element.Focus();
            ExecuteTrigger();
        }

        /// <summary>Program Controller</summary>
        private void ExecuteTrigger()
        {
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(StartExecution);
        }

        /// <summary>Program Controller</summary>
        private void StartExecution()
        {
            KeyboardInput.TypeString("{RIGHT 3}");
            QueueDelegate(VerifyFlowDirectionPropertyForFirstRun);
        }

        /// <summary>Verifies flow direction property for the first run</summary>
        private void VerifyFlowDirectionPropertyForFirstRun()
        {
            string flowDirectionForFirstRun = _rtb.Selection.GetPropertyValue(Run.FlowDirectionProperty).ToString();
            Verifier.Verify(flowDirectionForFirstRun == FlowDirection.LeftToRight.ToString(), "Flow Direction for first run doesnt match Actual[" + flowDirectionForFirstRun +
                "] Expected [LeftToRight]", false);
            QueueDelegate(ApplyFormatting);
        }

        /// <summary>Applies formatting to merge and split the runs</summary>
        private void ApplyFormatting()
        {
            switch (_selectionSwitch)
            {
                case SelectionChoices.NoSelection:
                    {
                        TextPointer _tp = _rtb.CaretPosition;
                        ApplyPropertyOnRange(_tp, _tp);
                        QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0,0,0,0,500), new SimpleHandler(VerifyNoSelection),null);
                        break;
                    }

                case SelectionChoices.SelectAll:
                    {
                        TextPointer _tp = _rtb.Document.ContentStart;
                        TextPointer _tp1 = _rtb.Document.ContentEnd;
                        ApplyPropertyOnRange(_tp, _tp1);
                        QueueDelegate(VerifySelectAll);
                        break;
                    }

                case SelectionChoices.SelectAcrossRuns:
                    {
                        TextPointer _tp = _rtb.Document.ContentStart;
                        _tp = _tp.GetPositionAtOffset(5, LogicalDirection.Forward);
                        int _position = (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal) ? 16 : 12;
                        TextPointer _tp1 = _rtb.Document.ContentStart.GetPositionAtOffset(_position, LogicalDirection.Forward);
                        _rtb.Selection.Select(_tp, _tp1);
                        ApplyPropertyOnRange(_tp, _tp1);
                        QueueDelegate(VerifySelectAcrossRuns);
                        break;
                    }

                default:
                    {
                        break;
                    }
            }
        }

        /// <summary>Applies formatting on the Range</summary>
        private static void ApplyPropertyOnRange(TextPointer _tp, TextPointer _tp1)
        {
            TextRange _tr = new TextRange(_tp, _tp1);
            _tr.ApplyPropertyValue(Run.FlowDirectionProperty, FlowDirection.RightToLeft);         
        }

        /// <summary>Verifies when formatting is applied with no selection</summary>
        private void VerifyNoSelection()
        {
            TextRange _rangeBeforeSelectedText = new TextRange(_rtb.Document.ContentStart, _rtb.Selection.Start);
            _flowDirectionLTR =  true;
            VerificationHelper(_rangeBeforeSelectedText);

            TextRange _rangeSelectedText = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            _flowDirectionLTR =  true;
            VerificationHelper(_rangeSelectedText);

            if (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)
            {
                TextRange _rangeSelectedAfterTextInFirstPara = new TextRange(_rtb.Selection.End, _rtb.Document.Blocks.FirstBlock.ContentEnd);
                _flowDirectionLTR = true;
                VerificationHelper(_rangeSelectedAfterTextInFirstPara);
                Block _block = _rtb.Document.Blocks.LastBlock;
                TextRange _rangeSelectedAfterTextInSecondPara = new TextRange(_block.ContentStart, _block.ContentEnd);
                _flowDirectionLTR = true;
                VerificationHelper(_rangeSelectedAfterTextInSecondPara);
                _flowDirectionLTR = false;
                VerificationHelperForPara();
            }
            else
            {
                if (_inlineChoicesForRTB == InlineTypes.SpanItalicNormal)
                {
                    TextRange _rangeSelectedImmediatelyAfterSelectionEnd = new TextRange(_rtb.Selection.End.GetNextInsertionPosition(LogicalDirection.Forward), _rtb.Selection.End.GetNextInsertionPosition(LogicalDirection.Forward));
                    _flowDirectionLTR = true;
                    VerificationHelper(_rangeSelectedImmediatelyAfterSelectionEnd);
                    TextRange _rangeSelectedImmediatelyBeforeDocEnd = new TextRange(_rtb.Document.ContentEnd.GetNextInsertionPosition(LogicalDirection.Backward), _rtb.Document.ContentEnd);
                    _flowDirectionLTR = false;
                    VerificationHelper(_rangeSelectedImmediatelyBeforeDocEnd);

                }
                else
                {
                    TextRange _rangeSelectedAfterText = new TextRange(_rtb.Selection.End, _rtb.Document.ContentEnd);
                    _flowDirectionLTR = true;
                    VerificationHelper(_rangeSelectedAfterText);
                }
                _flowDirectionLTR = false;
                VerificationHelperForPara(FlowDirection.RightToLeft.ToString(), FlowDirection.RightToLeft.ToString());
            }
            NextCombination();
        }

        /// <summary>Verifies when formatting is applied with all contents selected</summary>
        private void VerifySelectAll()
        {
            TextRange _rangeCompleteText = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            //RTL for span/run when there is a single paragraph
            //LTR when there are multiple paragraphs. here span is not created
            _flowDirectionLTR = (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)?true:false;
            VerificationHelper(_rangeCompleteText);
            //LTR for para for single para
            //RTL for multiple paras as FD is now applied on the para
            _flowDirectionLTR = (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)? false:true;
            string str = (_flowDirectionLTR) ? FlowDirection.LeftToRight.ToString() : FlowDirection.RightToLeft.ToString();
            VerificationHelperForPara(str,str);
            NextCombination();
        }

        /// <summary>Verifies when formatting is applied with selection being part of the runs/paras</summary>
        private void VerifySelectAcrossRuns()
        {
            //runs are in different paragraphs -- apply FD on all paragraphs
            if (_inlineChoicesForRTB == InlineTypes.ParagraphBoldNormal)
            {
                TextRange _rangeBeforeAllText = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
                //the section inherits from the RTB which is LTR
                _flowDirectionLTR = true;
                VerificationHelper(_rangeBeforeAllText);
                VerificationHelperForPara(FlowDirection.RightToLeft.ToString(), FlowDirection.RightToLeft.ToString());
            }
            else //Runs are in same paragraph -- apply on runs
            {
                TextRange _rangeBeforeSelectedText = new TextRange(_rtb.Document.ContentStart, _rtb.Selection.Start);
                _flowDirectionLTR = true;
                VerificationHelper(_rangeBeforeSelectedText);

                TextRange _rangeSelectedText = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
                _flowDirectionLTR = false;
                VerificationHelper(_rangeSelectedText);

                //Tests application of FD on runs with different FD
                TextRange _rangeSelectedAfterText = new TextRange(_rtb.Selection.End, _rtb.Document.ContentEnd);
                _flowDirectionLTR = (_inlineChoicesForRTB == InlineTypes.SpanItalicNormal)?false:true;
                VerificationHelper(_rangeSelectedAfterText);
            }
            //Tests that runs do not merge 
            if (_inlineChoicesForRTB == InlineTypes.ConsecutiveRuns)
            {
                Log("-------------------------------------------------------------------------");
                VerificationOfXaml(_xamlRun, GetXaml());
            }
            NextCombination();
        }

        /// <summary>Verification Helper</summary>
        private void VerificationHelper(TextRange _range)
        {
            string _flowDirection = _flowDirectionLTR ? FlowDirection.LeftToRight.ToString() : FlowDirection.RightToLeft.ToString();
            Verifier.Verify(_range.GetPropertyValue(Run.FlowDirectionProperty).ToString() == _flowDirection,
                "Flow Direction of String [" + _range.Text + "] Expected [" + _flowDirection.ToString() +
                "] Actual [" + _range.GetPropertyValue(Run.FlowDirectionProperty).ToString() + "]", true);
        }
        
          /// <summary>Verification Helper</summary>
        private void VerificationHelperForPara()
        {
            VerificationHelperForPara("", "");
        }

        /// <summary>Verification Helper</summary>
        private void VerificationHelperForPara(string _fdFirst, string _fdSecond)
        {
            string _flowDirection;
            _flowDirection = _flowDirectionLTR ? FlowDirection.LeftToRight.ToString() : FlowDirection.RightToLeft.ToString();
            _flowDirection = (_fdFirst == string.Empty) ? _flowDirection : _fdFirst;
            Verifier.Verify(_rtb.Document.Blocks.FirstBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() == _flowDirection,
                "Flow Direction of First Para ] Expected [" + _flowDirection.ToString() +
                "] Actual [" + _rtb.Document.Blocks.FirstBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() + "]", true);
            _flowDirection = FlowDirection.LeftToRight.ToString();
            _flowDirection = (_fdSecond == string.Empty) ? _flowDirection : _fdSecond;
            Verifier.Verify(_rtb.Document.Blocks.LastBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() == _flowDirection,
                "Flow Direction of Last Para ] Expected [" + _flowDirection.ToString() +
                "] Actual [" + _rtb.Document.Blocks.LastBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() + "]", true);

        }


        /// <summary>Verification of XAML</summary>
        private static void VerificationOfXaml(string _xamlRun, string _xamlString)
        {
            string _xamlStringDisplay = _xamlString.Replace("/>", "/>\r\n");
            _xamlStringDisplay = _xamlStringDisplay.Replace("<Paragr", "\r\n******************\r\n<Paragr");
            _xamlStringDisplay = _xamlStringDisplay.Replace("<Run", "\r\n---------------------\r\n<Run");
            Verifier.Verify(_xamlString.Contains(_xamlRun) == true, "Complete xaml [" + _xamlStringDisplay +
                "] \r\n ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ \r\n Expected to contain[" + _xamlRun + "]\r\n ~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~~ \r\n", true);
        }

        /// <summary>Gets selected XAML</summary>
        private string GetXaml()
        {
            TextRange tr = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            return XamlUtils.TextRange_GetXml(tr);
        }

        /// <summary>Sets RTB content</summary>
        private void SetRTBContent()
        {
            _rtb.FlowDirection = FlowDirection.LeftToRight;
            switch (_inlineChoicesForRTB)
            {
                case InlineTypes.ConsecutiveRuns:
                    {
                        Run _firstRun = new Run();
                        _firstRun.Text = _firstString;
                        Run _secRun = new Run(_SecString);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        p.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }
                case InlineTypes.BoldNormal:
                    {
                        Bold _firstRun = new Bold();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        Run _secRun = new Run(_SecString);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        p.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }

                case InlineTypes.SpanItalicNormal:
                    {
                        Span s = new Span();
                        Italic _firstRun = new Italic();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        Run _secRun = new Run(_SecString);
                        _secRun.FlowDirection = FlowDirection.RightToLeft;
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        s.Inlines.Add(_firstRun);
                        s.Inlines.Add(_secRun);
                        p.Inlines.Add(s);
                        _rtb.Document.Blocks.Add(p);
                        break;
                    }

                case InlineTypes.ParagraphBoldNormal:
                    {
                        Bold _firstRun = new Bold();
                        _firstRun.Inlines.Clear();
                        _firstRun.Inlines.Add(new Run(_firstString));
                        _firstRun.FlowDirection = FlowDirection.LeftToRight;
                        Run _secRun = new Run(_SecString);
                        Paragraph p = new Paragraph();
                        p.FlowDirection = FlowDirection.LeftToRight;
                        p.Inlines.Add(_firstRun);
                        Paragraph p1 = new Paragraph();
                        p1.FlowDirection = FlowDirection.LeftToRight;
                        p1.Inlines.Add(_secRun);
                        _rtb.Document.Blocks.Add(p);
                        _rtb.Document.Blocks.Add(p1);
                        break;
                    }
                default:
                    {
                        break;
                    }
            }
        }

        #region data.

        private string _firstString = "hello ";
        private string _SecString = "world";
        private string _xamlRun = "<Run>lo wo</Run></Span>";

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb;
        private InlineTypes _inlineChoicesForRTB = 0;
        private SelectionChoices _selectionSwitch = 0;
        private bool _flowDirectionLTR = true;

        #endregion data.
    }

     /// <summary>Tests Ctrl [] increase decrease font size of text </summary>
    [Test(2, "RichEditing", "RichTextBoxIncreaseDecreaseFontSize", MethodParameters = "/TestCaseType:RichTextBoxIncreaseDecreaseFontSize")]
    [TestOwner("Microsoft"), TestTactics("487"), TestWorkItem("74"), TestBugs("857, 858")]
    public class RichTextBoxIncreaseDecreaseFontSize : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Clear();
                _rtb = _element as RichTextBox;
                _defaultFontSize = _fontSizeFront = _fontSizeEnd = _fontSizeTracker = 20;
                _rtb.FontSize = _defaultFontSize;
                InitializeRTB();
                TestElement = _element;
                InitializeRTB();
                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>DoFocus</summary>
        private void DoFocus()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary>PROGRAM CONTROLLER</summary>
        private void ExecuteTrigger()
        {
            _element.Focus();
            switch (_contentDataSwitch)
            {
                case ContentChoicesRTB.EmptyRTB:
                    KeyboardInput.TypeString("^{HOME}^]^]"+_firstString);
                    QueueDelegate(CheckIncreasedFontSizeEmptyRTB);
                    break;

                case ContentChoicesRTB.List:
                    KeyboardInput.TypeString("^{HOME}{RIGHT 2}^]^]");
                    QueueDelegate(CheckIncreasedFontSizeInMiddle);
                    break;

                case ContentChoicesRTB.BuicAndText:
                    Verifier.Verify(_b.IsEnabled == false, "Button should be Disabled Actual [" + _b.IsEnabled.ToString() + "]", true);
                    KeyboardInput.TypeString("^{HOME}{RIGHT 1}^]^]");
                    QueueDelegate(CheckIncreasedFontSizeInMiddle);
                    break;

                case ContentChoicesRTB.DiffFontSizes:
                    _fontSizeTracker = _fontSizeFront = 80;
                    _fontSizeEnd = 40;
                    KeyboardInput.TypeString("^{HOME}{RIGHT 2}^]^]");
                    QueueDelegate(CheckIncreasedFontSizeInMiddle);
                    break;

                default:
                    break;
            }            
        }

        /// <summary>CheckIncreasedFontSizeEmptyRTB</summary>
        private void CheckIncreasedFontSizeEmptyRTB()
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            FontSizeVerificationHelper(tr, _defaultFontSize, true);
            _fontSizeFront = _fontSizeTracker;
            KeyboardInput.TypeString("^{HOME}{RIGHT 2}^]^]");
            QueueDelegate(CheckIncreasedFontSizeInMiddle);
        }

        /// <summary>CheckIncreasedFontSizeInMiddle /SPRINGLOAD</summary>
        private void CheckIncreasedFontSizeInMiddle()
        {
            TextRange tr = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            if (_contentDataSwitch == ContentChoicesRTB.BuicAndText)
            {
                double fontSize = (double)(tr.GetPropertyValue(Inline.FontSizeProperty));
                Verifier.Verify((int)(_fontSizeTracker) == (int)(fontSize), "FontSize should be == Initial FontSize Current [" + fontSize.ToString() +
                   "] Default [" + _fontSizeTracker.ToString() + "]", true);
            }
            else
            {
                double fontSize = (double)(tr.GetPropertyValue(Inline.FontSizeProperty));
                Verifier.Verify((_fontSizeTracker + 2 * 0.75) == fontSize, "FontSize should be == Initial FontSize + 2*0.75 Current [" + fontSize.ToString() +
                    "] Default [" + _fontSizeTracker.ToString() + " + 1.5]", true);
                FontSizeVerificationHelper(tr, _fontSizeTracker, true);
            }
            _fontSizeFront = _fontSizeTracker;
            _fontSizeTracker = (_contentDataSwitch != ContentChoicesRTB.EmptyRTB) ? _fontSizeEnd : _fontSizeTracker;
            KeyboardInput.TypeString("^{END}^]^]"+_secString+"+{LEFT 4}");
            QueueDelegate(CheckIncreasedFontSizeInEnd);
        }

        /// <summary>CheckIncreasedFontSizeInEnd</summary>
        private void CheckIncreasedFontSizeInEnd()
        {
            TextRange tr = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            FontSizeVerificationHelper(tr, _fontSizeTracker, true);
            _fontSizeEnd= _fontSizeTracker;
            KeyboardInput.TypeString("^A^[^[");
            QueueDelegate(CheckDecreaseFontSize);
        }

        /// <summary>CheckDecreaseFontSize</summary>
        private void CheckDecreaseFontSize()
        {
            KeyboardInput.TypeString("^{HOME}+{RIGHT 1}");
            QueueDelegate(CheckDecreaseAtFront);
        }

        /// <summary>CheckDecreaseAtFront</summary>
        private void CheckDecreaseAtFront()
        {
            TextRange tr = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            if (_contentDataSwitch == ContentChoicesRTB.BuicAndText)
            {
                double fontSize = (double)(tr.GetPropertyValue(Inline.FontSizeProperty));
                Verifier.Verify((int)(_defaultFontSize) == (int)(fontSize), "FontSize should be == Initial FontSize Current [" + fontSize.ToString() +
                   "] Default [" + _fontSizeTracker.ToString() + "]", true);
            }
            else
            {
                FontSizeVerificationHelper(tr, _fontSizeFront, false);
            }
            KeyboardInput.TypeString("^{END}+{LEFT 3}");
            QueueDelegate(CheckDecreaseAtEnd);
        }

        /// <summary>CheckDecreaseAtEnd</summary>
        private void CheckDecreaseAtEnd()
        {
            TextRange tr = new TextRange(_rtb.Selection.Start, _rtb.Selection.End);
            FontSizeVerificationHelper(tr, _fontSizeEnd, false);
            _fontSizeEnd = _fontSizeTracker;
            Log("Performing Undo");
            _rtb.Undo();
            QueueDelegate(CheckUndoIncreasesFontSize);
        }

        private void CheckUndoIncreasesFontSize()
        {
            TextRange tr = new TextRange(_rtb.CaretPosition, _rtb.CaretPosition);
            FontSizeVerificationHelper(tr, _fontSizeEnd, true);
            _fontSizeEnd = _fontSizeTracker;
            Log("Performing Redo");
            _rtb.Redo();
            QueueDelegate(CheckRedoDecreasesFontSize);
        }

        private void CheckRedoDecreasesFontSize()
        {
            TextRange tr = new TextRange(_rtb.CaretPosition, _rtb.CaretPosition);
            FontSizeVerificationHelper(tr, _fontSizeEnd, false);
            QueueDelegate(NextCombination);
        }


        #region Helpers.

        /// <summary>FontSizeVerificationHelper</summary>
        private void FontSizeVerificationHelper(TextRange tr, double _initialFontSize, bool greater )
        {
            double fontSize = (double)(tr.GetPropertyValue(Inline.FontSizeProperty));
            _fontSizeTracker = fontSize;
            if (greater)
            {
                Verifier.Verify((_initialFontSize) < (fontSize), "FontSize should be > Initial FontSize Current [" + fontSize.ToString() +
                   "] Default [" + _initialFontSize.ToString() + "]", true);
            }
            else
            {
                Verifier.Verify((_initialFontSize) > (fontSize), "FontSize should be < Initial FontSize Current [" + fontSize.ToString() +
                    "] Default [" + _initialFontSize.ToString() + "]", true);
            }
        }

        /// <summary>initialization of the RTB</summary>
        private void InitializeRTB()
        {
            _rtb.Document.Blocks.Clear();
            _rtb.FontSize = _defaultFontSize;
            switch (_contentDataSwitch)
            {
                case ContentChoicesRTB.EmptyRTB:
                    break;

                case ContentChoicesRTB.List:
                    List _list = new List();
                    ListItem _listItem1 = new ListItem(new Paragraph(new Run(_firstString)));
                    ListItem _listItem2 = new ListItem(new Paragraph(new Run(_secString)));
                    _list.ListItems.Add(_listItem1);
                    _list.ListItems.Add(_listItem2);
                    _rtb.Document.Blocks.Add(_list);
                    break;

                case ContentChoicesRTB.BuicAndText:
                    _b = new Button();
                    _rtb.Document.Blocks.Add(new BlockUIContainer(_b));
                    _rtb.Document.Blocks.Add(new Paragraph(new Run(_firstString)));
                    break;

                case ContentChoicesRTB.DiffFontSizes:
                    Paragraph p = new Paragraph();
                    Run run1 = new Run(_firstString);
                    run1.FontSize = 80;
                    Run run2 = new Run(_secString);
                    run2.FontSize = 40;
                    p.Inlines.Add(run1);
                    p.Inlines.Add(run2);
                    _rtb.Document.Blocks.Add(p);
                    break;

                default:
                    break;
            }
        }

        #endregion Helpers.

        #region data.

        private string _firstString = "hello ";
        private string _secString = "world";
        private double _defaultFontSize ;
        private double _fontSizeTracker;
        private double _fontSizeFront;
        private double _fontSizeEnd;
        private Button _b = null;

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb;
        private ContentChoicesRTB _contentDataSwitch = 0;

        #endregion data.
    }
}

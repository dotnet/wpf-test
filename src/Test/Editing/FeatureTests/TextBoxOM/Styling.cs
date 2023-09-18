// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for styling the TextBox control.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Styling.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Drawing;
    using System.Reflection;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Controls.Primitives;
    using System.Windows.Input;
    using System.Windows.Media;

    using Brushes = System.Windows.Media.Brushes;
    using Point = System.Windows.Point;

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

    /// <summary>
    /// Verifies that TextBox properties are not affected by styling,
    /// measuring and rendering stages.
    /// </summary>
    [Test(0, "TextBox", "TextBoxContentPropsStable", MethodParameters = "/TestCaseType=TextBoxContentPropsStable")]
    [TestOwner("Microsoft"), TestTactics("532"), TestWorkItem("89"),
   TestBugs("735,736,737,738,739,740,741,742,743,578"), TestLastUpdatedOn("July 10, 2006")]
    public class TextBoxContentPropsStable: ManagedCombinatorialTestCase
    {
        #region Private data.

        // Non-default values. text must be a number, otherwise
        // it gets wiped away when IsNumberOnly gets set.
        private const int selectionStart = 2;
        private const int selectionLength = 6;

        #endregion Private data.

        #region Main flow.

        /// <summary>Reads a combination and determines whether it should run.</summary>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);
            if (result)
            {
                // Filter out custom subclasses from parent-inherited sources.
                // The test library doesn't handle this way of style-setting well.
                result = (this._propertyValueSource == PropertyValueSource.ParentInherited)?
                    !this._editableType.IsSubClass : true;
            }

            return result;
        }

        /// <summary>Runs the test case.</summary>
        protected override void DoRunCombination()
        {
            _testDefaults = this._propertyValueSource == PropertyValueSource.PropertyDefault;
            _testElement = _editableType.CreateInstance();
            _parent = new Canvas();
            _wrapper = new UIElementWrapper(_testElement);

            _beforeParenting = true;            

            if (_testDefaults)
            {
                Log("Verifying standalone values...");
                VerifyValues();
            }
            else
            {
                Log("Customizing and verifying values...");
                SetCustomValues();
                VerifyValues();
            }

            Log("Parenting and verifying values...");            
            _parent.Children.Add(_testElement);                        
            TestElement = _parent;
            _beforeParenting = false;
            VerifyValues();

            QueueDelegate(AfterLayoutPass);
        }

        private void AfterLayoutPass()
        {
            Log("Verifying values after a layout pass...");
            VerifyValues();

            //TestStyledBox();
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Verifications.

        private void SetCustomValues()
        {
            DependencyPropertyData[] properties;

            properties = DependencyPropertyData.GetDPDataForControl(_testElement.GetType());
            switch (this._propertyValueSource)
            {
                case PropertyValueSource.LocalValueSet:
                    // Specific repro: set some properties to themselves.
                    if (_testElement is TextBoxBase)
                    {
                        ((TextBoxBase)_testElement).ScrollToHorizontalOffset(((TextBoxBase)_testElement).HorizontalOffset);
                        ((TextBoxBase)_testElement).ScrollToVerticalOffset(((TextBoxBase)_testElement).VerticalOffset);
                    }
                    foreach (DependencyPropertyData property in properties)
                    {                        
                        _testElement.SetValue(property.Property, property.TestValue);
                    }
                    break;
                case PropertyValueSource.ParentInherited:
                    foreach (DependencyPropertyData property in properties)
                    {
                        if (property.IsInheritable)
                        {
                            _parent.SetValue(property.Property, property.TestValue);
                        }
                    }
                    break;
                case PropertyValueSource.StyleSet:
                    Style style;
                    style = new Style(_testElement.GetType());
                    foreach (DependencyPropertyData property in properties)
                    {
                        Log(property.ToString()+"------"+ property.TestValue.ToString() +"=====");
                        style.Setters.Add(new Setter(property.Property, property.TestValue));
                    }
                    _testElement.Style = style;
                    break;
                case PropertyValueSource.PropertyDefault:
                default:
                    throw new InvalidOperationException(
                        "SetCustomValues cannot apply from source " + this._propertyValueSource);
            }
        }

        private void VerifyCustomValues()
        {
            DependencyPropertyData[] properties;

            if (this._propertyValueSource == PropertyValueSource.ParentInherited &&
                _testElement.Parent == null)
            {
                Log("Element has not been parented yet - skipping verification.");
                return;
            }

            properties = DependencyPropertyData.GetDPDataForControl(_testElement.GetType());
            foreach (DependencyPropertyData property in properties)
            {
                // We only verify inheritable properties when the value was
                // set from the parent and it's not overriden in the
                // control style.
                if (this._propertyValueSource == PropertyValueSource.ParentInherited)
                {
                    PropertyValueSource actualSource;

                    if (!property.IsInheritable)
                    {
                        Log("Skipping non-inheritable property: " + property.Property.Name);
                        continue;
                    }

                    actualSource = DependencyPropertyData.GetPropertyValueSource(_testElement, property.Property);
                    if (actualSource == PropertyValueSource.StyleSet ||
                        actualSource == PropertyValueSource.LocalValueSet)
                    {
                        Log("Skipping property set in element style/locally: " + property.Property.Name);
                        continue;
                    }
                }                

                //Special case - Triggers on theme style like one for IsEnabled for Foreground property
                if (property.Property == TextBox.ForegroundProperty &&
                    this._propertyValueSource == PropertyValueSource.StyleSet)
                {
                    if (!_testElement.IsEnabled)
                    {
                        //For RichTextBox, visual triggers for IsEnabled=false state kicks in 
                        //before getting parented bcoz internally in its constructor it adds a document
                        //as a LogicalChild. Look at Regression_Bug578 for more information.
                        if(_testElement is RichTextBox)
                        {
                            bool result;                            
                            result = (((SolidColorBrush)_testElement.GetValue(property.Property)).Color == System.Windows.SystemColors.GrayTextBrush.Color); //Classic & Luna theme

                            Verifier.Verify(result, "Control Disabled: VerifyCustomValue [Foreground] property." +
                                "Actual [" + _testElement.GetValue(property.Property) + "]", true);

                            continue;
                        }
                        else //for TB/PB, the visual triggers get kicked in only after getting parented.
                        {
                            if(!_beforeParenting)
                            {
                                bool result;
                                result = (((SolidColorBrush)_testElement.GetValue(property.Property)).Color == System.Windows.SystemColors.GrayTextBrush.Color); //Classic & Luna theme

                                Verifier.Verify(result, "VerifyCustomValue [Foreground] property." +
                                    "Actual [" + _testElement.GetValue(property.Property) + "]", true);

                                continue;
                            }
                        }                        
                    }
                }

                //NOTE: We dont have to handle the special case of Triggers for Background property
                //because it is not inheritable property like Foreground. Look at Regression_Bug578 for more info.
                Verifier.VerifyValue(property.Property.Name,
                        property.TestValue, _testElement.GetValue(property.Property));
            }
        }

        /// <summary>Verifies that _testElement has default values on all its properties.</summary>
        private void VerifyDefaultValues()
        {
            DependencyPropertyData[] properties;

            properties = DependencyPropertyData.GetDPDataForControl(_testElement.GetType());
            foreach (DependencyPropertyData property in properties)
            {
                PropertyValueSource source;
                
                if (property.Property == TextBox.LanguageProperty)
                {
                    continue;
                }

                source = DependencyPropertyData.GetPropertyValueSource(_testElement, property.Property);

                // When the control has a parent, it may be inheriting property
                // values, so comparison to control defaults no longer apply.
                if (source == PropertyValueSource.ParentInherited)
                {
                    Log("Skipping " + property.Property.Name + ", as the source " +
                        "of the value is " + source + ", not the default.");
                    continue;
                }

                // Test subclasses will set some properties in a custom style
                // or just locally. We don't keep this information in the
                // common library.
                if (_editableType.IsSubClass &&
                    source == PropertyValueSource.LocalValueSet || source == PropertyValueSource.StyleSet)
                {
                    Log("Skipping " + property.Property.Name + ", as the source " +
                        "of the value is " + source + ", and this is a known testing sub-class.");
                    continue;
                }

                //Value of HorizontalScrollBarVisibility depends on TextWrapping.
                //Special case handling for this case.
                if (property.Property.Name == "HorizontalScrollBarVisibility")
                {
                    //the default value is hidden not disabled since NoWrap no longer works
                    if (_testElement is RichTextBox)
                    {
                        Verifier.VerifyValue(property.Property.Name,
                        ScrollBarVisibility.Hidden, _testElement.GetValue(property.Property));
                    }
                    if (_testElement is TextBox)
                    {
                        if (((TextBox)_testElement).TextWrapping != TextWrapping.NoWrap)
                        {
                            Verifier.VerifyValue(property.Property.Name,
                            ScrollBarVisibility.Disabled, _testElement.GetValue(property.Property));
                        }
                        else
                        {
                            Verifier.VerifyValue(property.Property.Name,
                            property.DefaultValue, _testElement.GetValue(property.Property));
                        }
                    }
                    continue;
                }

                //When Element is not yet added to the visual tree, some of the properties dont get initialized.
                //Special case handling for them. Look at Regression_Bug579.
                if (_testElement.Parent == null)
                {
                    //Just continue for these cases as values for them are not initialized.
                    if ( ((_editableType.IsPassword) && (property.Property.Name == "Background")) ||
                        ((_editableType.SupportsParagraphs) && (property.Property.Name == "MinHeight")) )
                    {
                        continue;
                    }
                }
                else
                {
                    //For all other properties, do the verification.
                    Verifier.VerifyValue(property.Property.Name,
                        property.DefaultValue, _testElement.GetValue(property.Property));
                }
            }
        }

        private void VerifyValues()
        {
            if (_testDefaults)
            {
                VerifyDefaultValues();
            }
            else
            {
                VerifyCustomValues();
            }
        }

        #endregion Verifications.

        #region Private fields.

        /// <summary>Parent with element under test.</summary>
        private Canvas _parent;

        /// <summary>Whether defaults or custom values should be tested.</summary>
        private bool _testDefaults;

        /// <summary>
        /// Some property values depend on whether they are parented or not 
        /// and hence we need this variable
        /// </summary>
        private bool _beforeParenting;        

        /// <summary>Element under test.</summary>
        private FrameworkElement _testElement;

        /// <summary>Wrapper for element under test.</summary>
        private UIElementWrapper _wrapper;

        /// <summary>Type of control being tested.</summary>
        private TextEditableType _editableType=null;

        /// <summary>Source from which values will be applied.</summary>
        private PropertyValueSource _propertyValueSource=0;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that a TextBox can be created without a border.
    /// </summary>
    [Test(3, "Controls", "TextBoxBorderless", MethodParameters = "/TestCaseType=TextBoxBorderless")]
    [TestOwner("Microsoft"), TestTactics("531"), TestBugs("720,721,722,723")]
    public class TextBoxBorderless: CustomTestCase
    {
        #region Private data.

        private TextBox _textbox;
        private Bitmap _initialBorder;
        private Bitmap _noBorder;
        private Bitmap _newBackground;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Creating TextBox...");
            _textbox = new TextBox();

            MainWindow.Content = _textbox;
            MainWindow.Background = Brushes.Yellow;

            QueueDelegate(TakeSnapshot);
        }

        /// <summary>Take a snapshot of the TextBox with regular border.</summary>
        private void TakeSnapshot()
        {
            Log("Capturing TextBox picture...");
            _initialBorder = BitmapCapture.CreateBitmapFromElement(_textbox);

            Log("Removing TextBox border...");
            _textbox.BorderThickness = new Thickness(0);

            QueueDelegate(AfterNoBorder);
        }

        /// <summary>Verify that the TextBox has changed.</summary>
        private void AfterNoBorder()
        {
            Log("Capturing new TextBox picture...");
            _noBorder = BitmapCapture.CreateBitmapFromElement(_textbox);


            Log("Changing TextBox background...");
            _textbox.Background = Brushes.Yellow;

            QueueDelegate(AfterNewBackground);
        }

        /// <summary>Verify that the TextBox has changed.</summary>
        private void AfterNewBackground()
        {
            Log("Capturing new TextBox picture...");
            _newBackground = BitmapCapture.CreateBitmapFromElement(_textbox);

            Log("Verifying image has changed...");
            ActionItemWrapper.VerifyBitmapsDifferent(_initialBorder, _newBackground);
            ActionItemWrapper.VerifyBitmapsDifferent(_noBorder, _newBackground);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the TextBox can be re-styled and still works
    /// as expected.
    /// </summary>
    [Test(3, "Controls", "TextBoxReStyling", MethodParameters = "/TestCaseType=TextBoxReStyling")]
    [TestOwner("Microsoft"), TestTactics("530"), TestBugs("643,126"),
     TestSpec("TextBox"), TestWorkItem("87, 77")]
    public class TextBoxReStyling: CustomTestCase
    {
        #region Private data.
        struct TestCaseData
        {
            internal string testDescription;
            internal Type textBoxContentType;
            internal TestCaseData(string description, Type tbContentType)
            {
                testDescription = description;
                textBoxContentType = tbContentType;
            }

            internal static TestCaseData[] Cases = new TestCaseData[] {
                new TestCaseData("Default Style", null),
                new TestCaseData("Style - ScrollViewer", typeof(ScrollViewer)),
                new TestCaseData("Invalid Style", typeof(Button))
            };
        }

        private int _caseIndex;
        private TextBox _textbox;
        private bool _focusExpected;
        #endregion Private data.

        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _caseIndex = 0;
            SetupCase();
        }

        private void SetupCase()
        {
            string description;
            Type testTBContentType;
            Style parsedStyle;

            description = TestCaseData.Cases[_caseIndex].testDescription;
            testTBContentType = TestCaseData.Cases[_caseIndex].textBoxContentType;

            Log("Setting up test case " + _caseIndex);
            Log(TestCaseData.Cases[_caseIndex].testDescription);

            if (description == "Default Style")
            {
                Log("Creating TextBox with default style...");
                _textbox = new TextBox();
                TestWindow.Content = _textbox;
                _textbox.AcceptsReturn = true;
                _focusExpected = false;
            }
            else if (description == "Style - ScrollViewer")
            {
                Log("Creating TextBox with its content " + description);
                parsedStyle = GetStyleForEditableType(typeof(TextBox), testTBContentType);
                _textbox.Style = parsedStyle;
                _focusExpected = true;
            }
            else
            {
                Log("Creating TextBox with its content " + description);
                parsedStyle = GetStyleForEditableType(typeof(TextBox), testTBContentType);
                _textbox.Style = parsedStyle;
                try
                {
                    _textbox.UpdateLayout();
                    throw new ApplicationException("Invalid style is accepted.");
                }
                catch (System.NotSupportedException e)
                {
                    Log("Exception is thrown as expected: " + e.ToString());
                }
                catch (Exception e)
                {
                    Log("Exception thrown: " + e.ToString());
                    throw new ApplicationException("Unexpected exception type is thrown when we " +
                        "expect NotSupportedException to be thrown");
                }
                _textbox.ClearValue(TextBox.StyleProperty);
                _focusExpected = true;
            }
            QueueDelegate(CheckFocus);
        }

        private void CheckFocus()
        {
            if (_focusExpected)
            {
                Log("Checking for focus...");
                Verifier.Verify(_textbox.IsKeyboardFocused, "TextBox has focus.", true);
            }
            else
            {
                MouseInput.MouseClick(_textbox);
            }

            Log("Clearing TextBox...");
            _textbox.Clear();
            Verifier.Verify(_textbox.Text == "",
                "TextBox.Clear empties text.", true);

            KeyboardInput.TypeString("asdf{ENTER}");
            QueueDelegate(CheckTyping);
        }

        private void CheckTyping()
        {
            Log("TextBox.Text: [" + _textbox.Text + "]");
            Verifier.Verify(_textbox.Text != "", "TextBox accepts input.", true);
            Verifier.Verify(_textbox.Text == "asdf\r\n", "TextBox accepts enter.", true);

            _caseIndex++;
            System.Diagnostics.Debug.Assert(_caseIndex <= TestCaseData.Cases.Length);
            if (_caseIndex == TestCaseData.Cases.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueDelegate(SetupCase);
            }
        }

        private Style GetStyleForEditableType(Type editableType, Type contentType)
        {
            Style newStyle;
            FrameworkElementFactory tbBorder, tbContent;
            ControlTemplate template;

            if ( !(typeof(TextBoxBase).IsAssignableFrom(editableType)) )
            {
                throw new Exception("Only works for TextBoxBase");
            }

            newStyle = new Style(editableType);

            tbBorder = new FrameworkElementFactory(typeof(Border));
            tbBorder.SetValue(Border.BorderBrushProperty, Brushes.Blue);
            tbBorder.SetValue(Border.BorderThicknessProperty, new Thickness(2d));

            tbContent =new FrameworkElementFactory(contentType, "PART_ContentHost");

            template = new ControlTemplate(editableType);
            template.VisualTree = tbContent;

            if(editableType == typeof(RichTextBox))
            {
                newStyle.Setters.Add(new Setter(RichTextBox.TemplateProperty, template));
            }
            else
            {
                newStyle.Setters.Add(new Setter(TextBox.TemplateProperty, template));
            }

            return newStyle;
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the TextBox can be re-styled and still works
    /// as expected.
    /// </summary>
    /// <remarks>
    /// Mini-test-matrix:
    /// - What control is being re-styled.
    /// - Whether the style has no tagged PART_ContentHost, a tagged
    ///   ScrollViewer, a tagged Border, or something else tagged.
    /// - Whether the control is invalid (transitioned from scratch),
    ///   invalid (transitioned from valid) or valid (transition from invalid).
    /// What is verified:
    /// - General API usage and input. This could be expanded considerably.
    /// </remarks>
    [Test(2, "Controls", "TextInvalidStyleCase", MethodParameters = "/TestCaseType:TextInvalidStyleCase")]
    [TestOwner("Microsoft"), TestTactics("527"), TestBugs("64"), TestLastUpdatedOn("Aug 9, 2006")]
    public class TextInvalidStyleCase: CustomTestCase
    {       
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel panel = new StackPanel();

            _rtb = new RichTextBox();
            _pb = new PasswordBox();
            _tb = new TextBox();
            _tb.Height = 50;
            _tb.Width = 100;
            _rtb.Height = 100;
            _rtb.Width = 200;
            _pb.Height = 25;
            _pb.Width = 50;

            Log("Setting invalid styles on controls...");
            _rtb.Style = CreateStyleForEditableType(typeof(RichTextBox), null);
            _pb.Style = CreateStyleForEditableType(typeof(PasswordBox), null);
            _tb.Style = CreateStyleForEditableType(typeof(TextBox), null);

            panel.Children.Add(_tb);
            panel.Children.Add(_rtb);
            panel.Children.Add(_pb);
            MainWindow.Content = panel;

            QueueDelegate(DoMouseClickTB);
        }

        //Do Mouse LeftClick and RightClick actions. Coverage for 
        private void DoMouseClickTB()
        {
            MouseInput.MouseClick(_tb);
            QueueDelegate(DoMouseClickRTB);
        }

        private void DoMouseClickRTB()
        {            
            MouseInput.MouseClick(_rtb);
            QueueDelegate(DoMouseClickPB);
        }

        private void DoMouseClickPB()
        {
            MouseInput.MouseClick(_pb);
            QueueDelegate(DoRightClickTB);
        }

        private void DoRightClickTB()
        {
            MouseInput.RightMouseClick(_tb);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(DoRightClickRTB), null);
        }

        private void DoRightClickRTB()
        {            
            MouseInput.RightMouseClick(_rtb);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(DoRightClickPB), null);
        }

        private void DoRightClickPB()
        {            
            MouseInput.RightMouseClick(_pb);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(AfterLayout), null);
        }

        private void AfterLayout()
        {
            Log("Testing TextBox...");
            TestTextBox(_tb);
            Log("Done TextBox...");
            Log("Testing TextBoxBase...");
            TestTextBoxBase(_tb);
            Log("Done TextBoxBase...");
            Log("Testing RichTextBox...");
            TestRichTextBox(_rtb);
            Log("Done RichTextBox...");
            Log("Testing TextBoxBase...");
            TestTextBoxBase(_rtb);
            Log("Done TextBoxBase...");
            Log("Testing PasswordBox...");
            TestPasswordBox(_pb);
            Log("Done PasswordBox...");

            //When invalid style is set on controls, the control is disabled and content is not displayed
            //we do not guarantee the triggerring of editingCommands in this state
            //as such we do not test the editing/ApplicationCommands when the style is invalid
            if (_withProperStyle)
            {
                Log("Testing TextBox...");
                TestControl(_tb);
                Log("Testing RichTextBox...");
                TestControl(_rtb);
                Log("Testing PasswordBox...");
                TestControl(_pb);
            }

            if (_verifyDisabled == false)
            {
                Log("Setting valid styles on controls...");

                _rtb.Style = CreateStyleForEditableType(typeof(RichTextBox), typeof(ScrollViewer));
                _pb.Style = CreateStyleForEditableType(typeof(PasswordBox), typeof(ScrollViewer));
                _tb.Style = CreateStyleForEditableType(typeof(TextBox), typeof(ScrollViewer));
                _withProperStyle = true;
                // Disable verification after having changed all defaults.
                _verifyDisabled = true;
                
                DoMouseClickTB();
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Main flow.

        #region Helper methods.

        private void TestRichTextBox(RichTextBox rtb)
        {
            VerifyEquals("GetPositionFromPoint", rtb.GetPositionFromPoint(new Point(100, 100), false), null);
            rtb.AutoWordSelection = false;
            VerifyEquals("AutoWordSelection", rtb.AutoWordSelection, false);
            rtb.CaretPosition = rtb.Document.ContentStart;
            rtb.Selection.Select(rtb.Document.ContentStart, rtb.Document.ContentEnd);
        }

        private void TestPasswordBox(PasswordBox pb)
        {
            pb.Clear();
            pb.ApplyTemplate();
            pb.Paste();
            pb.SelectAll();
            pb.Password = "abcdefg";
            pb.MaxLength = 4;
            pb.PasswordChar = 'k';
        }

        private void TestTextBox(TextBox tb)
        {
            tb.Clear();
            VerifyEquals("Text", tb.Text, "");
            VerifyEquals("GetCharacterIndexFromLineIndex", tb.GetCharacterIndexFromLineIndex(0), -1);
            VerifyEquals("GetFirstVisibleLineIndex", tb.GetFirstVisibleLineIndex(), -1);
            VerifyEquals("GetLastVisibleLineIndex", tb.GetLastVisibleLineIndex(), -1);
            VerifyEquals("GetLineIndexFromCharacterIndex", tb.GetLineIndexFromCharacterIndex(0), -1);
            VerifyEquals("GetLineLength", tb.GetLineLength(0), -1);
            VerifyEquals("GetLineText", tb.GetLineText(0), null);
            VerifyEquals("GetRectFromCharacterIndex", tb.GetRectFromCharacterIndex(0), Rect.Empty);
            tb.ScrollToLine(0);
            tb.Select(0, 2);
            tb.Text = "f";
            tb.CaretIndex = 2;
            VerifyEquals("CaretIndex", tb.CaretIndex, 1);
            tb.CharacterCasing = CharacterCasing.Upper;
            VerifyEquals("LineCount", tb.LineCount, -1);
            VerifyEquals("MaxLength", tb.MaxLength, 0);
            VerifyEquals("MaxLines", tb.MaxLines, Int32.MaxValue);
            VerifyEquals("MinLines", tb.MinLines, 1);
            tb.MaxLines = 4;
            tb.MinLines = 2;
            VerifyEquals("SelectedText", tb.SelectedText, "");
            VerifyEquals("SelectionLength", tb.SelectionLength, 0);
            VerifyEquals("SelectionStart", tb.SelectionStart, 1);
            VerifyEquals("TextAlignment", tb.TextAlignment, TextAlignment.Left);
        }

        private void TestTextBoxBase(TextBoxBase tb)
        {
            KeyboardDevice keyboard;
            TextCompositionEventArgs textArgs;

            tb.AppendText("text");

            tb.BeginChange();
            tb.SelectAll();
            tb.Cut();
            tb.EndChange();
            using (tb.DeclareChangeBlock())
            {
                tb.SelectAll();
                tb.Paste();
            }
            tb.ApplyTemplate();
            tb.LineDown(); tb.LineLeft(); tb.LineRight(); tb.LineUp();
            tb.LockCurrentUndoUnit();
            tb.PageDown(); tb.PageLeft(); tb.PageRight(); tb.PageUp();
            tb.Undo();
            tb.Redo();
            tb.Redo();
            tb.ScrollToHome();
            tb.ScrollToEnd();
            tb.AcceptsReturn = true;
            VerifyEquals("AcceptsReturn", tb.AcceptsReturn, true);
            tb.AcceptsTab = true;
            VerifyEquals("AcceptsTab", tb.AcceptsTab, true);
            VerifyEquals("CanRedo", tb.CanRedo, false);
            VerifyEquals("CanUndo", tb.CanUndo, false);
            VerifyEquals("ExtentHeight", tb.ExtentHeight, 0d);
            VerifyEquals("ExtentWidth", tb.ExtentWidth, 0d);
            VerifyEquals("HorizontalOffset", tb.HorizontalOffset, 0d);
            tb.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            VerifyEquals("VerticalScrollBarVisibility", tb.VerticalScrollBarVisibility, ScrollBarVisibility.Visible);
            VerifyEquals("VerticalOffset", tb.VerticalOffset, 0d);
            VerifyEquals("IsReadOnly", tb.IsReadOnly, false);
            tb.IsReadOnly = true;
            VerifyEquals("IsReadOnly", tb.IsReadOnly, true);
            tb.IsReadOnly = false;
            tb.SpellCheck.IsEnabled = true;
            tb.IsUndoEnabled = false;
            tb.IsUndoEnabled = true;
            if (tb is TextBox)
            {
                ((TextBox)tb).TextWrapping = TextWrapping.Wrap;
            }

            tb.Focus();
            keyboard = InputManager.Current.PrimaryKeyboardDevice;
            textArgs = new TextCompositionEventArgs(keyboard,
                new TextComposition(InputManager.Current, tb, " "));
            textArgs.RoutedEvent = TextCompositionManager.TextInputEvent;

            InputManager.Current.ProcessInput(textArgs);
        }

        private void TestControl(Control control)
        {
            TestControlCommands(control, typeof(EditingCommands), true);
            TestControlCommands(control, typeof(EditingCommands), false);
            TestControlCommands(control, typeof(ApplicationCommands), true);
            TestControlCommands(control, typeof(ApplicationCommands), false);
        }

        private void TestControlCommands(Control control, Type type, bool withContent)
        {
            Log("content :" + withContent.ToString());
            if (withContent)
            {
                if (control is TextBox) ((TextBox)control).Text = "foo";
                if (control is PasswordBox) ((PasswordBox)control).Password = "foo";
                if (control is RichTextBox)
                {
                    FlowDocument fd = new FlowDocument();
                    fd.Blocks.Add(new Paragraph(new Run("foo")));
                    ((RichTextBox)control).Document = fd;
                }

                if (control is TextBoxBase) ((TextBoxBase)control).SelectAll();
                if (control is PasswordBox) ((PasswordBox)control).SelectAll();
            }
            else
            {
                if (control is TextBox) ((TextBox)control).Clear();
                if (control is PasswordBox) ((PasswordBox)control).Clear();
                if (control is RichTextBox) ((RichTextBox)control).Document = new FlowDocument();
            }
            foreach (PropertyInfo property in type.GetProperties())
            {
                // Skip any non-static properties, as we don't have an instance.
                if (!property.GetGetMethod().IsStatic)
                {
                    continue;
                }

                // Get the property and see if it's a valid RoutedCommand.
                RoutedCommand command = property.GetValue(null, null) as RoutedCommand;

                if (command != null)
                {
                    command.Execute(null, control);
                }
            }
        }

        private void VerifyEquals(string member, object current, object expected)
        {
            if (_verifyDisabled) return;
            if (current == expected) return;
            if (current == null)
                throw new Exception("Current value for " + member + " is null but expected non-null.");
            if (current is double && double.IsNaN((double)current) != double.IsNaN((double)expected))
                throw new Exception("Current value for " + member + " is NaN but expected a number (or vice-versa).");
            if (!current.Equals(expected))
                throw new Exception("Current value for " + member + " is " + current + " but expected " + expected + ".");
        }

        private Style CreateStyleForEditableType(Type editableType, Type contentType)
        {
            Style newStyle;
            FrameworkElementFactory tbBorder;
            ControlTemplate template;

            newStyle = new Style(editableType);

            tbBorder = new FrameworkElementFactory(typeof(Border));
            tbBorder.SetValue(Border.BorderBrushProperty, Brushes.Blue);
            tbBorder.SetValue(Border.BorderThicknessProperty, new Thickness(2d));

            if (contentType != null)
            {
                FrameworkElementFactory tbContent = new FrameworkElementFactory(contentType, "PART_ContentHost");

                tbBorder.AppendChild(tbContent);
            }

            template = new ControlTemplate(editableType);
            template.VisualTree = tbBorder;

            newStyle.Setters.Add(new Setter(Control.TemplateProperty, template));

            return newStyle;
        }

        #endregion Helper methods.

        #region Private fields.

        private bool _withProperStyle = false;
        private RichTextBox _rtb;
        private PasswordBox _pb;
        private TextBox _tb;
        private bool _verifyDisabled;

        #endregion Private fields.
    }

    /// <summary> Compares default visual values of all controls  </summary>
    [Test(3, "Controls", "MatchDefaultValuesOfControls", MethodParameters = "/TestCaseType:MatchDefaultValuesOfControls")]
    [TestOwner("Microsoft"), TestTactics("528"), TestBugs("644"), TestWorkItem("86")]
    public class MatchDefaultValuesOfControls : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel panel = new StackPanel();

            _pb = new PasswordBox();
            _rtb = new RichTextBox();

            _tb = new TextBox();

            Log("Set default values...");

            panel.Children.Add(_tb);
            panel.Children.Add(_rtb);
            panel.Children.Add(_pb);
            MainWindow.Content = panel;

            QueueDelegate(AfterLayout);
        }

        private void AfterLayout()
        {
            Log("Testing TextBox...");
            TestValues();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Helper methods.

        private void TestValues()
        {
            VerifyEquals("Background RTB/PB", _rtb.Background, _pb.Background);
            VerifyEquals("Background RTB/TB", _rtb.Background, _tb.Background);
            VerifyEquals("BorderBrush RTB/PB", _rtb.BorderBrush, _pb.BorderBrush);
            VerifyEquals("BorderBrush RTB/TB", _rtb.BorderBrush, _tb.BorderBrush);
            VerifyEquals("BorderThickness RTB/PB", _rtb.BorderThickness, _pb.BorderThickness);
            VerifyEquals("BorderThickness RTB/TB", _rtb.BorderThickness, _tb.BorderThickness);
            VerifyEquals("FlowDirection RTB/PB", _rtb.FlowDirection, _pb.FlowDirection);
            VerifyEquals("FlowDirection RTB/TB", _rtb.FlowDirection, _tb.FlowDirection);
            VerifyEquals("FontFamily RTB/TB", _rtb.FontFamily, _tb.FontFamily);
            VerifyEquals("FontSize RTB/PB", _rtb.FontSize, _pb.FontSize);
            VerifyEquals("FontSize RTB/TB", _rtb.FontSize, _tb.FontSize);
            VerifyEquals("Foreground RTB/PB", _rtb.Foreground, _pb.Foreground);
            VerifyEquals("Foreground RTB/TB", _rtb.Foreground, _tb.Foreground);
            VerifyEquals("IsEnabled RTB/PB", _rtb.IsEnabled, _pb.IsEnabled);
            VerifyEquals("IsEnabled RTB/TB", _rtb.IsEnabled, _tb.IsEnabled);
            VerifyEquals("FontStyle RTB/PB", _rtb.FontStyle, _pb.FontStyle);
            VerifyEquals("FontStyle RTB/TB", _rtb.FontStyle, _tb.FontStyle);
        }

        private void VerifyEquals(string member, object current, object expected)
        {
            if (current == expected) return;
            if (current == null)
                throw new Exception("Current value for " + member + " is null but expected non-null.");
            if (member == "Background RTB/PB")
            {
                current = current.ToString();
                expected = expected.ToString();
            }

            if (!current.Equals(expected))
                throw new Exception("Current value for " + member + " is " + current + " and " + expected + ". They should be same.");
        }

        #endregion Helper methods.

        #region Private fields.

        private RichTextBox _rtb;
        private PasswordBox _pb;
        private TextBox _tb;

        #endregion Private fields.
    }

     /// <summary>
    /// Tests Apply template works properly
    /// </summary>
    [Test(0, "Controls", "TestApplyTemplate", MethodParameters = "/TestCaseType:TestApplyTemplate")]
    [TestOwner("Microsoft"), TestTactics("529"), TestWorkItem("85"), TestLastUpdatedOn("July 11,2006")]
    public class TestApplyTemplate : ManagedCombinatorialTestCase
    {

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            Clipboard.Clear();
            _wrapper = new UIElementWrapper(_editableType.CreateInstance());
            _element = _wrapper.Element;
            TestElement = (FrameworkElement)_wrapper.Element;
            QueueDelegate(ApplyTemplate);
        }

        private void ApplyTemplate()
        {
            Type _contentType = (_validStyle == true) ? typeof(ScrollViewer) : null;

            Style _style = CreateStyleForEditableType(_element.GetType() ,_contentType);
            TestElement.Style = (_emptyStyle == true)? null:_style;
            _appliedTemplate = TestElement.ApplyTemplate();
            QueueDelegate(VerifyApplyTemplate);
        }

        private void VerifyApplyTemplate()
        {
            Verifier.Verify(((_emptyStyle == true)? (_appliedTemplate == false):(_appliedTemplate == true)),"EmptyStyle ["+
                _emptyStyle.ToString() + "] When EmptyStyle==FALSE ApplyTemplate == FALSE Actual [" + _appliedTemplate.ToString() + "]", true);
            QueueDelegate(NextCombination);
        }

        #region Helper.

        private Style CreateStyleForEditableType(Type editableType, Type contentType)
        {
            Style newStyle;
            FrameworkElementFactory tbBorder;
            ControlTemplate template;

            newStyle = new Style(editableType);

            tbBorder = new FrameworkElementFactory(typeof(Border));
            tbBorder.SetValue(Border.BorderBrushProperty, Brushes.Blue);
            tbBorder.SetValue(Border.BorderThicknessProperty, new Thickness(2d));
            tbBorder.SetValue(Border.BackgroundProperty, Brushes.Yellow);

            if (contentType != null)
            {
                FrameworkElementFactory tbContent = new FrameworkElementFactory(contentType, "PART_ContentHost");

                tbBorder.AppendChild(tbContent);
            }

            template = new ControlTemplate(editableType);
            template.VisualTree = tbBorder;

            newStyle.Setters.Add(new Setter(Control.TemplateProperty, template));

            return newStyle;
        }

        #endregion Helper.

        #region data.

        private UIElement _element = null;
        private UIElementWrapper _wrapper = null;
        private TextEditableType _editableType = null;

        private bool _validStyle = false;
        private bool _emptyStyle = false;
        private bool _appliedTemplate = false;

        #endregion data.
    }
}

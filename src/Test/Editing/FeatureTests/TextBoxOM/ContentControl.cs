// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test cases for TextBox that relate to its ContentControl superclass.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/ContentControl.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;    
    using System.Collections;
    using System.ComponentModel;
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Markup;    // for XmlLanguage
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
    
    using Bitmap = System.Drawing.Bitmap;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the Content property on the TextBox behaves
    /// appropriately.
    /// </summary>
    /// <remarks>
    /// Until TextBox subclasses ContentControl, this test case is modified
    /// to use the Text property instead.
    /// </remarks>
    [Test(0, "TextBox", "TextBoxContent", MethodParameters = "/TestCaseType=TextBoxContent /ValueForContent=sample")]
    [TestOwner("Microsoft"),TestTactics("653"), TestBugs("664"),
     TestArgument("ValueForContent", "Value for the Text property")]
    public class TextBoxContent: CustomTestCase
    {
        #region Settings.

        /// <summary>
        /// Value to be used in the Content test case.
        /// </summary>
        private string ValueForContent
        {
            get
            {
                string result = ConfigurationSettings.Current
                    .GetArgument("ValueForContent");
                return (result == "null")? null : result;
            }
        }

        #endregion Settings.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TextBox box;              // Control created.
            IEnumerator children;     // Children of control.
            UIElementWrapper wrapper; // Wrapper for operations.

            box = new TextBox();
            MainWindow.Content = box;

            Log("Setting content to value: " + ValueForContent);
            wrapper = new UIElementWrapper(box);
            wrapper.Text = ValueForContent;
            Verifier.Verify(ValueForContent == (string) wrapper.Text,
                "Value in box matches: " + wrapper.Text, true);

            Log("Verifying range contents...");
            children = LogicalTreeHelper.GetChildren(box).GetEnumerator();
            while (children.MoveNext())
            {
                Log("Children content: " + children.Current);
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the TextBox has the appropriate defaults.
    /// </summary>
    [Test(0, "TextBox", "TextBoxDefaults", MethodParameters = "/TestCaseType=TextBoxDefaults")]
    [TestOwner("Microsoft"),TestTactics("651,652"), TestBugs("799,800,801,802,803"),
     TestArgument("CheckBeforeTree", "Whether the defaults should also be " +
        "checked before the control is added to the UI tree (defaults to false)")]
    [Test(3, "TextBox", "TextBoxDefaults", MethodParameters = "/TestCaseType=TextBoxDefaults /CheckBeforeTree=True")]
    public class TextBoxDefaults: CustomTestCase
    {
        #region Settings.

        private bool CheckBeforeTree
        {
            get { return Settings.GetArgumentAsBool("CheckBeforeTree", false); }
        }

        #endregion Settings.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Creating TextBox outside of the UI tree...");
            _textbox = new TextBox();

            if (CheckBeforeTree)
            {
                VerifyDefaultValues(_textbox);
            }

            MainWindow.Content = _textbox;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckCreated));
        }

        private void CheckCreated()
        {
            Log("TextBox found in tree...");
            VerifyDefaultValues(_textbox);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyDefaultValues(TextBox box)
        {
            //
            // A TextBox initially has the following default values,
            // for a single-line textbox UX:
            //
            // - AcceptsReturn  = false
            // - AcceptsTab     = false
            // - Wrap           = false
            //
            Verifier.Verify(box.AcceptsReturn == false,
                "TextBox.AcceptsReturn == false", true);
            Verifier.Verify(box.AcceptsTab == false,
                "TextBox.AcceptsTab == false", true);
            Verifier.Verify(box.TextWrapping == TextWrapping.NoWrap,
                "TextBox.TextWrapping == TextWrapping.NoWrap", true);

            // Verify that no filtering takes place.
            Verifier.Verify(box.CharacterCasing == CharacterCasing.Normal,
                "TextBox.CharacterCasing == CharacterCasing.Normal", true);
            Verifier.Verify(box.MaxLength == 0,
                "TextBox.MaxLength == 0", true);
        }

        #endregion Verifications.

        #region Private fields.

        private TextBox _textbox;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the TextBox renders differently when different
    /// values are applied to the following properties:
    /// FontSize, FontStyle, FontWeight.
    /// </summary>
    [Test(0, "TextBox", "TextBoxFontProperties", MethodParameters = "/TestCaseType=TextBoxFontProperties /Text=abcd")]
    [TestOwner("Microsoft"),TestTactics("650"),TestBugs("804,712"),
     TestArgument("Text", "Text to set on TextBox")]
    public class TextBoxFontProperties: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            Log("Using text: " + Text);
            TestTextBox.FontSize = 12f * (96.0 / 72.0);
            TestTextBox.FontStyle = FontStyles.Normal;
            TestTextBox.FontWeight = FontWeights.Normal;
            TestTextBox.Text = Text;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CaptureInitial));
        }

        private void CaptureInitial()
        {
            if (TestTextBox.Text != Text)
            {
                Log("TextBox.Text=[" + TestTextBox.Text + "]");
                throw new Exception("Text has not been set.");
            }
            Log("Capturing initial rendering...");
            _lastBitmap = BitmapCapture.CreateBitmapFromElement(TestControl);
            TestTextBox.FontSize = 24f * (96.0 / 72.0);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontSizeChanged));
        }

        private void CheckFontSizeChanged()
        {
            Log("Verifying that FontSize produces a change...");
            _lastBitmap = CheckChanged();
            TestTextBox.FontStyle = FontStyles.Italic;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontStyleChanged));
        }

        private void CheckFontStyleChanged()
        {
            Log("Verifying that FontStyle produces a change...");
            _lastBitmap = CheckChanged();
            TestTextBox.FontWeight = FontWeights.Bold;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckFontWeightChanged));
        }

        private void CheckFontWeightChanged()
        {
            Log("Verifying that FontStyle produces a change...");
            CheckChanged();

            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("   ");

            QueueHelper.Current.QueueDelegate(new SimpleHandler(CheckEdited));
        }

        private void CheckEdited()
        {
            Verifier.Verify(TestTextBox.Text != Text, "Text has changed", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Verifies that the bitmap has changed.
        /// </summary>
        /// <returns>A new bitmap for the currently rendered control.</returns>
        private Bitmap CheckChanged()
        {
            Bitmap current = BitmapCapture.CreateBitmapFromElement(TestControl);
            if (ComparisonOperationUtils.AreBitmapsEqualUsingCriteria(_lastBitmap,current,null,true))
            {
                throw new Exception("There have been no changes in the control rendering.");
            }
            return current;
        }

        #endregion Verifications.

        #region Private fields.

        private Bitmap _lastBitmap;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the TextBox is still usable after changing
    /// its parent panel.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("649"), TestBugs("665")]
    [Test(2, "TextBox", "TextBoxChangeParent", MethodParameters = "/TestCaseType=TextBoxChangeParent")]
    public class TextBoxChangeParent: CustomTestCase
    {
        #region Private data.

        /// <summary>Data for test cases.</summary>
        internal struct TestCaseData
        {
            /// <summary>Type of control to instantiate.</summary>
            internal Type ControlType;

            /// <summary>Initializes a new TestCaseData instance.</summary>
            /// <param name="controlType">Type of control to instantiate.</param>
            internal TestCaseData(Type controlType)
            {
                this.ControlType = controlType;
            }
        }

        /// <summary>Cases to run..</summary>
        private TestCaseData[] _cases = new TestCaseData[] {
            new TestCaseData(typeof(TextBox))
        };

        /// <summary>Index of currently executing case.</summary>
        private int _caseIndex;

        /// <summary>Wrapper for tested control.</summary>
        private UIElementWrapper _wrapper;

        /// <summary>Control being tested.</summary>
        private Control _control;

        /// <summary>Initial panel.</summary>
        private Canvas _leftCanvas;

        /// <summary>Reparenting panel.</summary>
        private Canvas _rightCanvas;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetupWindow();
        }

        /// <summary>Sets up the window and start the test case flow.</summary>
        private void SetupWindow()
        {
            DockPanel topPanel;     // Top panel.

            System.Diagnostics.Debug.Assert(_caseIndex < _cases.Length);

            Log("Runnig case for type " + _cases[_caseIndex].ControlType);

            topPanel = new DockPanel();
            topPanel.Background = Brushes.Black;

            _leftCanvas = new Canvas();
            _leftCanvas.Background = Brushes.Blue;
            _leftCanvas.Width = 100;

            _rightCanvas = new Canvas();
            _rightCanvas.Background = Brushes.Green;
            _rightCanvas.Width = 100;

            topPanel.Children.Add(_leftCanvas);
            topPanel.Children.Add(_rightCanvas);

            // Create the control on the left canvas.
            _control = (Control)
                ReflectionUtils.CreateInstance(_cases[_caseIndex].ControlType);
            _wrapper = new UIElementWrapper(_control);
            _leftCanvas.Children.Add(_control);

            TestWindow.Content = topPanel;

            QueueDelegate(EditControl);
        }

        /// <summary>Queued input to edit control.</summary>
        private void EditControl()
        {
            MouseInput.MouseClick(_control);
            KeyboardInput.TypeString("111");
            QueueDelegate(ReparentControl);
        }

        /// <summary>Verified edited control and reparents it.</summary>
        private void ReparentControl()
        {
            // First, verify that the control has been edited.
            Log("Control text: [" + _wrapper.Text + "]");
            Verifier.Verify(_wrapper.Text == "111", "Text matches expected string.");

            // Reparent the control.
            _leftCanvas.Children.Remove(_control);
            _rightCanvas.Children.Add(_control);

            Log("Control text: [" + _wrapper.Text + "]");
            Verifier.Verify(_wrapper.Text == "111", "Text matches expected string.");

            QueueDelegate(EditReparentedControl);
        }

        /// <summary>Queues editing operations on reparented control.</summary>
        private void EditReparentedControl()
        {
            MouseInput.MouseClick(_control);
            KeyboardInput.TypeString("222");
            QueueDelegate(CheckReparentedControl);
        }

        /// <summary>Checks that the reparented control was edited.</summary>
        private void CheckReparentedControl()
        {
            // Verify that the control has been edited.
            Log("Control text: [" + _wrapper.Text + "]");
            Verifier.Verify(_wrapper.Text.Contains("222"), "Text contains typed string.");

            _caseIndex++;
            System.Diagnostics.Debug.Assert(_caseIndex <= _cases.Length);
            if (_caseIndex < _cases.Length)
            {
                QueueDelegate(SetupWindow);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Main flow.
    }

    /// <summary>
    /// Input tests for TextBox for CharacterCasing property
    /// </summary>
    [Test(0, "TextBox", "TestCharacterCasing", MethodParameters = "/TestCaseType:TestCharacterCasing")]
    [TestOwner("Microsoft"), TestTactics("648"), TestBugs("666"), TestWorkItem("115")]
    public class TestCharacterCasing : CustomTestCase
    {
        #region Private Members
        private TextBox _testTB;
        private StackPanel _panel;

        private string _testString = "abc123;('iIJK')";
        private string _testStringLocaleSpecific;
        private string _expLowerString,_expUpperString;

        /// <summary>Combinatorial engine for values.</summary>
        private CombinatorialEngine _combinatorials;
        private TextEditableType _textEditableType;
        private string _locale;
        private const string TurkishLocale = "0000041f";
        object[] _localeData = new object[] { TurkishLocale };

        #endregion

        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for combinations.
            dimensions = new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),
                new Dimension("InputLocaleData", _localeData)
            };
            _combinatorials = CombinatorialEngine.FromDimensions(dimensions);
            
            _panel = new StackPanel();
            MainWindow.Content = _panel;
            QueueDelegate(RunNextCase);
        }

        private void RunNextCase()
        {
            Hashtable combinationValues;   // Values for combination.

            combinationValues = new Hashtable();
            if (_combinatorials.Next(combinationValues))
            {
                _textEditableType = (TextEditableType)combinationValues["TextEditableType"];
                _locale = (string)combinationValues["InputLocaleData"];
                if (_textEditableType.Type == typeof(TextBox) ||
                    _textEditableType.Type.IsSubclassOf(typeof(TextBox)))
                {
                    _testTB = (TextBox)_textEditableType.CreateInstance();
                    _testTB.Height = 400;
                    _testTB.Width = 400;
                    _panel.Children.Clear();
                    _panel.Children.Add(_testTB);
                    KeyboardInput.SetActiveInputLocale(InputLocaleData.EnglishUS.Identifier);
                    Log("---- TextEditableType: " + _textEditableType.XamlName +
                    "---- Locale: " + _locale);
                    QueueDelegate(TestDefaultState);
                }
                else
                {
                    // skip for RichTextBox/RichTextBoxSubClass/PasswordBox
                    RunNextCase();
                }
            }
            else
            {
                Logger.Current.ReportSuccess();
                return;
            }
        }

        private void TestDefaultState()
        {
            Verifier.Verify(_testTB.CharacterCasing == CharacterCasing.Normal,
                "Verify that the default setting for CharacterCasing is normal", true);
            _testTB.Focus();
            QueueDelegate(TestCharacterCasingWithDefaultState);
        }

        private void TestCharacterCasingWithDefaultState()
        {
            KeyboardInput.TypeString(_testString);
            QueueDelegate(VerifyCharacterCasingWithDefaultState);
        }

        private void VerifyCharacterCasingWithDefaultState()
        {
            Verifier.Verify(_testTB.Text == _testString,
                "Verify textbox contents with CharacterCasing setting as Normal", true);
            _testTB.Clear();
            _testTB.CharacterCasing = CharacterCasing.Lower;
            KeyboardInput.TypeString(_testString);
            QueueDelegate(TestCharacterCasingLower);
        }

        private void TestCharacterCasingLower()
        {
            _expLowerString = _testString.ToLower(new CultureInfo(Int32.Parse(
                InputLocaleData.EnglishUS.Identifier, NumberStyles.AllowHexSpecifier)));
            Verifier.Verify(_testTB.Text == _expLowerString,
                "Verify textbox contents with CharacterCasing setting as Lower", true);
            _testTB.Clear();
            _testTB.CharacterCasing = CharacterCasing.Upper;
            KeyboardInput.TypeString(_testString);
            QueueDelegate(TestCharacterCasingUpper);
        }

        private void TestCharacterCasingUpper()
        {
            _expUpperString = _testString.ToUpper(new CultureInfo(Int32.Parse(
                InputLocaleData.EnglishUS.Identifier, NumberStyles.AllowHexSpecifier)));
            Verifier.Verify(_testTB.Text == _expUpperString,
                "Verify textbox contents with CharacterCasing setting as Upper", true);
            _testTB.Clear();

            //Coverage for 
            _testTB.CharacterCasing = CharacterCasing.Normal;
            KeyboardInput.SetActiveInputLocale(_locale);
            KeyboardInput.TypeString(_testString);
            QueueDelegate(GetLocaleSpecificTestString);
        }

        private void GetLocaleSpecificTestString()
        {
            _testStringLocaleSpecific = _testTB.Text;
            CultureInfo culture = new CultureInfo(Int32.Parse(
                InputLocaleData.EnglishUS.Identifier, NumberStyles.AllowHexSpecifier));
            _testTB.Clear();
            _testTB.Language = XmlLanguage.GetLanguage(culture.IetfLanguageTag); 
            _testTB.CharacterCasing = CharacterCasing.Lower;
            KeyboardInput.TypeString(_testString);
            QueueDelegate(TestCharacterCasingLowerTurkish);
        }

        private void TestCharacterCasingLowerTurkish()
        {
            _expLowerString = _testStringLocaleSpecific.ToLower(new CultureInfo(Int32.Parse(
                _locale, NumberStyles.AllowHexSpecifier)));
            Verifier.Verify(_testTB.Text == _expLowerString,
                "Verify textbox contents with CharacterCasing setting as Lower with locale=" + _locale, true);
            _testTB.Clear();
            _testTB.CharacterCasing = CharacterCasing.Upper;
            KeyboardInput.TypeString(_testString);
            QueueDelegate(TestCharacterCasingUpperTurkish);
        }

        private void TestCharacterCasingUpperTurkish()
        {
            _expUpperString = _testStringLocaleSpecific.ToUpper(new CultureInfo(Int32.Parse(
                _locale, NumberStyles.AllowHexSpecifier)));
            Verifier.Verify(_testTB.Text == _expUpperString,
                "Verify textbox contents with CharacterCasing setting as Upper with locale=" + _locale, true);
            _testTB.Clear();
            QueueDelegate(RunNextCase);
        }
        #endregion
    }
}

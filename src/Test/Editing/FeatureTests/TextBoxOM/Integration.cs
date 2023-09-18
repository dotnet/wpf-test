// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Intra-featuer and inter-feature integration test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/Integration.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;
    using System.Collections;
    using System.Drawing;
    using System.Globalization;
    using System.IO;
    using System.Text;

    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
    using System.Windows.Media.Animation;
    
    using DrawingColor = System.Drawing.Color;
    using MediaColor = System.Windows.Media.Color;
    using MediaColors = System.Windows.Media.Colors;

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
    /// Verifies that the TextBox works in a number of integration
    /// scenarios.
    /// </summary>
    [Test(0, "TextBox", "TextBoxIntegration", MethodParameters = "/TestCaseType=TextBoxIntegration")]
    [TestOwner("Microsoft"), TestTactics("586,587"),
     TestBugs("691,692,693,426,573,694,695,696,697,698,572"),
     TestArgument("Content", "Text to use for mixed editing, at least 3 character"),
     TestArgument("RunAccessibility", "Whether to run accesibility tests (false by default)"),
     TestArgument("RunComponenModel", "Whether to run ComponentModel tests (false by default)"),
     TestArgument("RunEDocuments", "Whether to run EDocument tests (false by default)"),
     TestArgument("RunMIL", "Whether to run MIL tests (false by default)")]
    public class TextBoxIntegration : TextBoxTestCase
    {
        #region Settings.

        /// <summary>Whether to accessibility integration tests.</summary>
        public bool RunAccessibility
        {
            get { return Settings.GetArgumentAsBool("RunAccessibility"); }
        }

        /// <summary>Whether to ComponentModel integration tests.</summary>
        public bool RunComponentModel
        {
            get { return Settings.GetArgumentAsBool("RunComponentModel"); }
        }

        /// <summary>Whether to EDocuments integration tests.</summary>
        public bool RunEDocuments
        {
            get { return Settings.GetArgumentAsBool("RunEDocuments"); }
        }

        /// <summary>Whether to MIL integration tests.</summary>
        public bool RunMIL
        {
            get { return Settings.GetArgumentAsBool("RunMIL"); }
        }

        /// <summary>TextBlock to use for mixed editing, at least 3 character.</summary>
        public string Content
        {
            get { return "Sample text."; }
        }

        private const string EnglishText = "abcd";
        private const string HebrewText = "\u05e9\u05e0";

        #endregion Settings.

        #region Sequence control.

        /// <summary>Index of executing step from the steps array.</summary>
        private int _stepIndex;

        /// <summary>Sequence of steps to be executed.</summary>
        private SimpleHandler[] _steps;

        /// <summary>Sets up the sequence of steps to be executed.</summary>
        private void SetupSequenceSteps()
        {
            ArrayList stepList = new ArrayList();
            if (RunAccessibility)
            {
                stepList.Add(new SimpleHandler(TestAccessibility));
            }
            if (RunComponentModel)
            {
                stepList.Add(new SimpleHandler(TestComponentModel));
            }
            stepList.Add(new SimpleHandler(TestConnectedData));
            stepList.Add(new SimpleHandler(TestCore));
            if (RunEDocuments)
            {
                stepList.Add(new SimpleHandler(TestEDocuments));
            }
            stepList.Add(new SimpleHandler(TestLayout));
            if (RunMIL)
            {
                stepList.Add(new SimpleHandler(TestMIL));
            }
            _steps = (SimpleHandler[])stepList.ToArray(typeof(SimpleHandler));
            if (_steps.Length == 0)
            {
                throw new Exception("There are no steps to be executed.");
            }
        }

        /// <summary>Starts running the step sequence.</summary>
        private void StartSequence()
        {
            if (_steps == null)
            {
                SetupSequenceSteps();
            }
            _stepIndex = 0;
            QueueHelper.Current.QueueDelegate(_steps[_stepIndex]);
        }

        /// <summary>
        /// Runs the next step in the step sequence, or finishes successfully.
        /// </summary>
        private void ContinueSequence()
        {
            System.Diagnostics.Debug.Assert(_stepIndex >= 0);
            _stepIndex++;
            if (_stepIndex == _steps.Length)
            {
                Logger.Current.ReportSuccess();
            }
            else
            {
                QueueHelper.Current.QueueDelegate(_steps[_stepIndex]);
            }
        }

        #endregion Sequence control.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);
            MouseInput.MouseClick(TestTextBox);
            StartSequence();
        }

        private void TestAccessibility()
        {
            // There is no commanding pattern in the current version,
            // but it should be tested here if and when it is
            // added.
            
            // Note that automation clients cannot run in the SEE.
            Log("Testing Accessibility integration...");
            ContinueSequence();
        }

        #region ComponentModel integration testing.


        private FrameworkElement[] _elements;
        private int _elementIndex;
        private ElementPosition _elementPosition;

        enum ElementPosition
        {
            Standalone, StartOfMixed, MiddleOfMixed, EndOfMixed,
            First = Standalone, Last = EndOfMixed
        }

        private void CreateTestElements()
        {
            Border border = new Border();
            TextBlock borderText = new TextBlock();
            borderText.Text="sample text";
            border.Child = borderText;

            Button button = new Button();
            button.Content = "Button contents";

            CheckBox checkbox = new CheckBox();
            checkbox.Content = "Checkbox contents";

            ComboBox combobox = new ComboBox();
            ComboBoxItem comboItem1 = new ComboBoxItem();
            comboItem1.Content = "Combo Item 1";
            ComboBoxItem comboItem2 = new ComboBoxItem();
            comboItem2.Content = "Combo Item 2";
            combobox.Items.Add(comboItem1);
            combobox.Items.Add(comboItem2);

            Slider slider = new Slider();
            slider.Minimum = 10;
            slider.Value = 40;
            slider.Maximum = 100;

            Hyperlink hyperlink = new Hyperlink();
            hyperlink.Inlines.Clear();
            hyperlink.Inlines.Add(new Run("http://www.microsoft.com/"));

            //Image image = new Image();
            //image.Source = "image.png";

            ListBox listbox = new ListBox();
            listbox.Items.Add("List item 1");
            listbox.Items.Add("List item 2");

            ListBox radioButtonList = new ListBox();
            radioButtonList.Resources.Add(typeof(ListBoxItem), CreateRadioButtonListStyle());
            radioButtonList.BorderBrush = System.Windows.Media.Brushes.Transparent;
            KeyboardNavigation.SetDirectionalNavigation(radioButtonList, KeyboardNavigationMode.Cycle);

            ListBoxItem radioButton1 = new ListBoxItem();
            radioButton1.Content = "Radio Button 1.";
            ListBoxItem radioButton2 = new ListBoxItem();
            radioButton2.Content = "Radio Button 2.";
            radioButtonList.Items.Add(radioButton1);
            radioButtonList.Items.Add(radioButton2);

            TextBlock text = new TextBlock();
            text.Text="Standalong text control.";

            TextBox textbox = new TextBox();
            textbox.Text = "Internal text box.";

            _elements = new FrameworkElement[]
                { border, button, checkbox, combobox, slider, listbox, radioButtonList, text, textbox };
        }

        /// <summary>
        /// Return Style for ListBoxItem that will have similar
        /// functionality as RadioButton. RadioButtonList is gone
        /// and ListBox with styling is used as a replacement
        /// </summary>
        /// <returns></returns>
        private Style CreateRadioButtonListStyle()
        {

            FrameworkElementFactory radioButton = new FrameworkElementFactory(typeof(RadioButton), "RadioButton");
            radioButton.SetValue(FrameworkElement.FocusableProperty, false);
            radioButton.SetValue(ContentControl.ContentProperty, new TemplateBindingExtension(ContentPresenter.ContentProperty));

            Binding isChecked = new Binding("IsSelected");
            isChecked.Mode = BindingMode.TwoWay;
            isChecked.RelativeSource = RelativeSource.TemplatedParent;
            radioButton.SetBinding(RadioButton.IsCheckedProperty, isChecked);

            Style radioButtonListStyle = new Style(typeof(ListBoxItem));
            ControlTemplate template = new ControlTemplate(typeof(ListBoxItem));
            template.VisualTree = radioButton;
            radioButtonListStyle.Setters.Add(new Setter(Control.TemplateProperty, template));

            return radioButtonListStyle;
        }

        private void TestComponentModel()
        {
            Log("Testing Component Model integration...");
            CreateTestElements();
            _elementIndex = 0;
            _elementPosition = ElementPosition.First;
            QueueHelper.Current.QueueDelegate(TestComponent);
        }

        private void TestComponent()
        {
            TestTextBox.Clear();
            FrameworkElement element = _elements[_elementIndex];
            // Prepare the range in which to insert element.
            TextRange range = null;
            TextPointer nav = null;

            switch (_elementPosition)
            {
                case ElementPosition.Standalone:
                    range = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox));
                    break;
                case ElementPosition.StartOfMixed:
                    TestTextBox.Text = Content;
                    range = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox));
                    break;
                case ElementPosition.MiddleOfMixed:
                    TestTextBox.Text = Content;
                    nav = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
                    for (int i = 0; i < 2; i++)
                    {
                        nav = nav.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                    range = new TextRange(nav, nav);
                    break;
                case ElementPosition.EndOfMixed:
                    TestTextBox.Text = Content;
                    nav = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
                    for (int i = 0; i < Text.Length; i++)
                    {
                        nav = nav.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                    range = new TextRange(nav, nav);
                    break;
            }
            System.Diagnostics.Debug.Assert(range != null);
            Test.Uis.Utils.ReflectionUtils.InvokeInstanceMethod(range, "InsertTextElement", new object[] {new InlineUIContainer(element) });
            //range.End.InsertTextElement(new InlineUIContainer(element));
            if (_elementPosition == ElementPosition.Last)
            {
                _elementIndex++;
                if (_elementIndex == _elements.Length)
                {
                    ContinueSequence();
                }
                else
                {
                    _elementPosition = ElementPosition.First;
                    QueueHelper.Current.QueueDelegate(TestComponent);
                }
            }
            else
            {
                _elementPosition = (ElementPosition)
                    ((int)_elementPosition) + 1;
                QueueHelper.Current.QueueDelegate(TestComponent);
            }
        }

        #endregion ComponentModel integration testing.

        #region ConnectedData integration testing.

        /// <summary>Helper object for data binding.</summary>
        public class TestDataObject
        {
            private string _name;
            private int _height;
            private int _width;
            private CharacterCasing _characterCasing;

            /// <summary>Name.</summary>
            public string Name { get { return _name; } set { _name = value; } }
            /// <summary>Height.</summary>
            public int Height { get { return _height; } set { _height = value; } }
            /// <summary>Height.</summary>
            public int Width { get { return _width; } set { _width = value; } }
            /// <summary>CharacterCasing.</summary>
            public CharacterCasing CharacterCasing { get { return _characterCasing; } set { _characterCasing = value; } }
        }

        /// <summary>Integer-to-Length transformer helper.</summary>
        public class IntegerToLengthTransformer : IValueConverter
        {
            /// <summary>Transforms the specified object.</summary>
            public object Convert(object o, Type type, object parameter, CultureInfo culture)
            {
                if (o == null)
                {
                    throw new ArgumentNullException("o");
                }
                if (type == null)
                {
                    throw new ArgumentNullException("type");
                }
                if (!(o is int))
                {
                    throw new ArgumentException("o should be an int", "o");
                }
                return (double)((int)o);
            }

            /// <summary>Performs the inverse transform from the specified object.</summary>
            public object ConvertBack(object o, Type type, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException("ConvertBack not implemented.");
            }
        }

        private TestDataObject _dataObject;

        private void TestConnectedData()
        {
            Binding bind;  // Binding instance used to specify bindings.

            Log("Testing ConnectedData integration...");

            _dataObject = new TestDataObject();
            _dataObject.Name = "Test Name";
            _dataObject.Height = 42;
            _dataObject.Width = 60;
            _dataObject.CharacterCasing = CharacterCasing.Upper;

            bind = new Binding("Name");
            bind.Mode = BindingMode.TwoWay;
            bind.UpdateSourceTrigger = UpdateSourceTrigger.PropertyChanged;
            bind.Source = _dataObject;
            TestTextBox.SetBinding(TextBox.TextProperty, bind);

            bind = new Binding("Height");
            bind.Mode = BindingMode.TwoWay;
            bind.Source = _dataObject;
            bind.Converter = new IntegerToLengthTransformer();
            TestTextBox.SetBinding(TextBox.HeightProperty, bind);

            bind = new Binding("Width");
            bind.Mode = BindingMode.OneWay;
            bind.Source = _dataObject;
            TestTextBox.SetBinding(TextBox.WidthProperty, bind);

            bind = new Binding("CharacterCasing");
            bind.Mode = BindingMode.TwoWay;
            bind.Source = _dataObject;
            TestTextBox.SetBinding(TextBox.CharacterCasingProperty, bind);

            QueueHelper.Current.QueueDelegate(CheckProperties);
        }

        private void CheckProperties()
        {
            Log("Control / Object - Text: " + TestTextBox.Text + " / " + _dataObject.Name);
            Log("Control / Object - Height: " + TestTextBox.Height + " / " + _dataObject.Height);
            Log("Control / Object - Width:  " + TestTextBox.Width + " / " + _dataObject.Width);
            Log("Control / Object - CharacterCasing: " + TestTextBox.CharacterCasing + " / " + _dataObject.CharacterCasing);

            Verifier.Verify(TestTextBox.Text == _dataObject.Name, "Text properties match.");
            Verifier.Verify(TestTextBox.Height == _dataObject.Height, "Height properties match.");
            Verifier.Verify(TestTextBox.Width == _dataObject.Width, "Width properties match.");
            Verifier.Verify(TestTextBox.CharacterCasing == _dataObject.CharacterCasing, "CharacterCasing properties match.");

            TestTextBox.CharacterCasing = CharacterCasing.Normal;
            ContinueSequence();
        }

        #endregion ConnectedData integration testing.

        #region Core integration testing.

        private string[] _inputLocalesForCore = new string[] {
            "00000401", // Arabic (Saudi Arabia) - Arabic (101)
            "00000409", // English (United States) - US
            "0000040d", // Hebrew - Hebrew
        };

        private string[] _inputLocaleNamesForCore = new string[] {
            "Arabic (Saudi Arabia) - Arabic (101)",
            "English (United States) - US",
            "Hebrew - Hebrew"
        };

        private int _inputLocalesForCoreIndex = 0;

        /// <summary>
        /// TextBlock expected in Core test after 'ab12' has been typed
        /// for all input locales. Note that Arabic will produce
        /// three characters for the 'ab' input sequence.
        /// </summary>
        private const string coreExpectedText = "\u0634\u0644\x0627" +
            "12" + "ab" + "12" + "\u05e9\u05e0" + "12";

        private void SetFixedWidthFont()
        {
            TestTextBox.FontFamily = new System.Windows.Media.FontFamily("Lucida Console");
            TestTextBox.FontSize = 24;
            // Character width is now 12.05 pixels in normal display.
        }

        private void TestCore()
        {
            Log("Testing Core integration...");
            TestTextBox.Clear();
            if (System.Windows.Forms.SystemInformation.MidEastEnabled)
            {
                QueueDelegate(TypeForLocale);
            }
            else
            {
                Log("This machine has no support for mid-east languages.");
                Log("Skipping Core typing tests.");
                ContinueSequence();
            }
        }

        private void TypeForLocale()
        {
            Log("Current text: [" + TestTextBox.Text + "] length=" + TestTextBox.Text.Length);
            Log("Current text (C-style): [" +
                TextUtils.ConvertToSingleLineAnsi(TestTextBox.Text) + "]");
            if (_inputLocalesForCoreIndex >= _inputLocalesForCore.Length)
            {
                Log("Expected text: [" + coreExpectedText + "] length=" + coreExpectedText.Length);
                Log("Expected text (C-style): [" +
                    TextUtils.ConvertToSingleLineAnsi(coreExpectedText) + "]");
                Verifier.Verify(coreExpectedText == TestTextBox.Text);
                CoreCheckNoPlainElements();
                SetFixedWidthFont();
                QueueDelegate(CoreCheckSerialization);
            }
            else
            {
                string inputLocale; // Hex representation of input locale.

                // These should always match.
                System.Diagnostics.Debug.Assert(
                    _inputLocalesForCore.Length == _inputLocaleNamesForCore.Length);

                // Log out locale being evaluated.
                inputLocale = _inputLocalesForCore[_inputLocalesForCoreIndex];
                Log("Typing for input locale: " + inputLocale + " (" +
                    _inputLocaleNamesForCore[_inputLocalesForCoreIndex] + ")");
                KeyboardInput.SetActiveInputLocale(inputLocale);
                KeyboardInput.TypeString("ab12");

                _inputLocalesForCoreIndex++;
                QueueDelegate(TypeForLocale);
            }
        }

        /// <summary>Checks that there are no elements in plain text.</summary>
        private void CoreCheckNoPlainElements()
        {
            TextPointer navigator;      // Navigator to find elements.
            TextPointerContext symbol;  // Symbol being read.

            Log("Verifying that there are no inline elements in " +
                "a plain text box (Regression_Bug572 inserts them " +
                "incorrectly for locale identification)");
            navigator = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            do
            {
                symbol = navigator.GetPointerContext(LogicalDirection.Forward);
                Verifier.Verify(symbol != TextPointerContext.ElementStart);
            } while ( (navigator = navigator.GetNextContextPosition(LogicalDirection.Forward)) != null);

            Log("No inline elements found.");
        }

        private void CoreCheckSerialization()
        {
            string serializedText;      // Serialized version of object.
            object parsedObject;        // Object returned from parser.
            TextBox parsedBox;          // Referenced to newly parsed TextBox.
            RichTextBox editablePanel;

            Log("Serializing TextBox...");
            serializedText = XamlWriter.Save(TestTextBox);
            Log("Serialized text:" + Environment.NewLine + serializedText + Environment.NewLine);

            Log("Parsing text...");
            parsedObject = XamlReader.Load(new Test.Uis.IO.StringStream(serializedText));
            Log("Parsed object: " + parsedObject);

            // Verify properties.
            parsedBox = (TextBox)parsedObject;
            Log("Text: " + parsedBox.Text);
            Verifier.Verify(parsedBox.Text == TestTextBox.Text);

            // Verifying that CommandLinks are not serialized.
            Log("Serializing FlowDocumentScrollViewer...");
            editablePanel = new RichTextBox();
            serializedText = XamlWriter.Save(editablePanel);
            Log("Serialized text:" + Environment.NewLine + serializedText + Environment.NewLine);

            Log("Parsing text...");
            parsedObject = XamlReader.Load(new Test.Uis.IO.StringStream(serializedText));
            Log("Parsed object: " + parsedObject);

            Log("Verifying serialized text...");
            Verifier.Verify(serializedText.IndexOf("CommandLinks") == -1,
                "There are no references to CommandLinks in serialized text.", true);
            Verifier.Verify(serializedText.IndexOf("TextEditorInstance") == -1,
                "There are no references to TextEditorInstance in serialized text.", true);

            ContinueSequence();
        }

        #endregion Core integration testing.

        #region EDocuments integration testing.

        private Type[] _edocsTypes = new Type[] {
            typeof(Block), typeof(Bold), /*typeof(Heading),*/typeof(Inline),
            typeof(Italic), /*typeof(Note),*/typeof(Paragraph), typeof(Span),
            typeof(Underline)
        };

        private int _edocsTypeIndex;

        private void TestEDocuments()
        {
            Log("Testing EDocuments integration...");

            TestTextBox.Clear();
            _edocsTypeIndex = 0;
            EDocumentsCreateElement();
        }

        private void EDocumentsCreateElement()
        {
            if (_edocsTypeIndex == _edocsTypes.Length)
            {
                ContinueSequence();
            }
            else
            {
                Type elementType = _edocsTypes[_edocsTypeIndex];
                System.Diagnostics.Debug.Assert(elementType != null);

                Log("Creating " + elementType.Name + "...");
                TestTextBox.Text = Content;

                bool constructWithPositions = elementType.IsSubclassOf(typeof(Inline));
                if (constructWithPositions)
                {
                    object[] args = new object[] { Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox) };
                    Activator.CreateInstance(elementType, args);
                }
                else
                {
                    TextRange range = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox));
                    TextElement elementObject = (TextElement)ReflectionUtils.CreateInstance(elementType);
                    Test.Uis.Utils.ReflectionUtils.InvokeInstanceMethod(range, "InsertTextElement", new object[] { elementObject });
          
                    //range.End.InsertTextElement(elementObject);
                    //--range.AppendElement(elementType);
                }

                // Verify that we don't break when interacting with text box - simplistic.
                MouseInput.MouseClick(TestTextBox);
                KeyboardInput.TypeString(Content);

                _edocsTypeIndex++;

                QueueHelper.Current.QueueDelegate(EDocumentsCreateElement);
            }
        }

        #endregion EDocuments integration testing.

        #region Layout integration testing.

        private TextBox _layoutTextBox;
        private Canvas _layoutCanvas;
        private DockPanel _layoutDockPanel;
        private StackPanel _layoutFlowPanel;
        private Grid _layoutGrid;
        private FlowDocumentScrollViewer _layoutTextPanel;
        private Decorator _layoutTransformDecorator;
        private delegate void MyHandler(SimpleHandler handler);

        private void TestLayout()
        {
            Log("Testing Layout integration...");

            TestTextBox.Clear();
            _layoutTextBox = TestTextBox;
            Panel parentPanel = (Panel)_layoutTextBox.Parent;
            parentPanel.Children.Remove(_layoutTextBox);

            LayoutCanvasSetup();
        }

        private void LayoutCanvasSetup()
        {
            Log("Laying out TextBox in Canvas...");
            _layoutCanvas = new Canvas();
            Canvas.SetTop(_layoutTextBox, 80);
            _layoutCanvas.Children.Add(_layoutTextBox);
            MainWindow.Content = _layoutCanvas;
            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectInput), new object[] { new SimpleHandler(LayoutDockPanelSetup) });            
        }

        private void LayoutInjectInput(SimpleHandler handler)
        {
            MouseInput.MouseClick(_layoutTextBox);            

            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectKeyboardInput), new object[] { new SimpleHandler(handler) });                        
        }

        private void LayoutInjectKeyboardInput(SimpleHandler handler)
        {            
            KeyboardInput.TypeString(Content);

            QueueHelper.Current.QueueDelegate(handler);
        }

        private void LayoutDockPanelSetup()
        {
            _layoutCanvas.Children.Remove(_layoutTextBox);

            Log("Laying out TextBox in DockPanel...");
            _layoutDockPanel = new DockPanel();
            DockPanel.SetDock(_layoutTextBox, Dock.Bottom);
            _layoutDockPanel.Children.Add(_layoutTextBox);
            MainWindow.Content = _layoutDockPanel;

            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectInput), new object[] { new SimpleHandler(LayoutFlowPanelSetup) });            
        }

        private void LayoutFlowPanelSetup()
        {
            _layoutDockPanel.Children.Remove(_layoutTextBox);

            Log("Laying out TextBox in StackPanel...");
            _layoutFlowPanel = new StackPanel();
            _layoutFlowPanel.Children.Add(new Button());
            _layoutFlowPanel.Children.Add(_layoutTextBox);
            _layoutFlowPanel.Children.Add(new Button());
            MainWindow.Content = _layoutFlowPanel;

            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectInput), new object[] { new SimpleHandler(LayoutGridSetup) });            
        }

        private void LayoutGridSetup()
        {
            Button button;  // Reference to buttons added.

            _layoutFlowPanel.Children.Remove(_layoutTextBox);

            Log("Laying out TextBox in Grid...");
            _layoutGrid = new Grid();
            _layoutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _layoutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _layoutGrid.ColumnDefinitions.Add(new ColumnDefinition());
            _layoutGrid.RowDefinitions.Add(new RowDefinition());
            _layoutGrid.RowDefinitions.Add(new RowDefinition());
            _layoutGrid.RowDefinitions.Add(new RowDefinition());

            button = new Button();
            button.Content = "button";
            Grid.SetRow(button, 0);
            Grid.SetColumn(button, 0);
            Grid.SetColumnSpan(button, 3);
            _layoutGrid.Children.Add(button);

            button = new Button();
            button.Content = "other button";
            Grid.SetRow(button, 1);
            Grid.SetColumn(button, 1);
            Grid.SetColumnSpan(button, 2);
            Grid.SetRowSpan(button, 2);
            _layoutGrid.Children.Add(button);

            Grid.SetRow(_layoutTextBox, 1);
            Grid.SetColumn(_layoutTextBox, 0);
            Grid.SetRowSpan(_layoutTextBox, 2);
            _layoutGrid.Children.Add(_layoutTextBox);

            MainWindow.Content = _layoutGrid;

            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectInput), new object[] { new SimpleHandler(LayoutTextPanelSetup) });            
        }

        private void LayoutTextPanelSetup()
        {
            Run runText = new Run();
            InlineUIContainer inlineUIContainer = new InlineUIContainer();

            _layoutGrid.Children.Remove(_layoutTextBox);

            Log("Laying out TextBox in FlowDocumentScrollViewer...");
            _layoutTextPanel = new FlowDocumentScrollViewer();
            _layoutTextPanel.Document = new FlowDocument(new Paragraph());
            //layoutTextPanel.ColumnCount = 3;
            string longContent = Content + Content + Content + Content;
            longContent = longContent +
                "\u0634\u0644" + "12" + "\u05e9\u05e0" + "12" + longContent + longContent;
            runText.Text += longContent;
            runText.Text += Environment.NewLine;
            runText.Text += longContent;
            runText.Text += Environment.NewLine + Environment.NewLine;
            ((Paragraph)_layoutTextPanel.Document.Blocks.FirstBlock).Inlines.Add(runText);
            inlineUIContainer.Child = _layoutTextBox;
            ((Paragraph)_layoutTextPanel.Document.Blocks.FirstBlock).Inlines.Add(inlineUIContainer);

            MainWindow.Content = _layoutTextPanel;

            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectInput), new object[] { new SimpleHandler(LayoutTransformDecoratorSetup) });            
        }

        private void LayoutTransformDecoratorSetup()
        {
            new TextRange(_layoutTextPanel.Document.ContentStart, _layoutTextPanel.Document.ContentEnd).Text = "";

            // The .Clear call above should clear the parent on the text box.
            // Regression_Bug573 was resolved "won't fix".  If you don't really care if the logical parent is null or not, remove the following:

            //Disconnect layoutTextBox from InlineUIContainer which was done in LayoutTextPanelSetup
            DependencyObject parent = _layoutTextBox.Parent;
            ((InlineUIContainer)parent).Child = null;

            ContentControl tempLogicalParent = new ContentControl();
            tempLogicalParent.Content = _layoutTextBox;
            tempLogicalParent.Content = null; // This will remove the layoutTextBox and set its parent to null.

            //PravirG - don't need this then because of the line above
            ((ContainerVisual)VisualTreeHelper.GetParent(_layoutTextBox)).Children.Remove(_layoutTextBox);

            Log("Laying out TextBox in TransformDecorator in Canvas....");
            _layoutTransformDecorator = new Decorator();
            _layoutCanvas.Children.Add(_layoutTransformDecorator);
            _layoutTransformDecorator.Child = _layoutTextBox;

            TransformCollection transformCollection = new TransformCollection();
            transformCollection.Add(new ScaleTransform(1, 1.5));
            transformCollection.Add(new TranslateTransform(160, 0));
            transformCollection.Add(new RotateTransform(45, 80, 80));
            TransformGroup group = new TransformGroup();
            group.Children = transformCollection;
            _layoutTransformDecorator.LayoutTransform = group;

            MainWindow.Content = _layoutCanvas;


            QueueHelper.Current.QueueDelegate(new MyHandler(LayoutInjectKeyboardInput), new object[] { new SimpleHandler(LayoutPassed) });                        
        }        

        private void LayoutPassed()
        {
            Log("All tested layout containers passed.");

            Log("Restoring TextBox...");
            _layoutTransformDecorator.Child = null;
            _layoutCanvas.Children.Remove(_layoutTransformDecorator);
            _layoutCanvas.Children.Add(_layoutTextBox);
            ContinueSequence();
        }

        #endregion Layout integration testing.

            #region MIL integration testing.

        private Canvas _milCanvas;
        private Decorator _milCompoundDecorator;
        private Decorator _milRotateDecorator;
        private Decorator _milScaleDecorator;
        private Decorator _milSkewDecorator;
        private Decorator _milTranslateDecorator;
        private Decorator[] _milDecorators;
        private int _milDecoratorIndex;
        private string[] _milDecoratorDescriptions;

        private const int OneSecondInMs = 1000;

        private void TestMIL()
        {
            TransformCollection transformCollection;    // Holder of TextBox transforms.
            TextBox t;                                  // Reference to tested box.

            Log("Testing MIL integration...");

            Log("Creating objects required for MIL integration testing...");
            _milCanvas = new Canvas();
            _milCompoundDecorator = new Decorator();
            transformCollection = new TransformCollection();
            transformCollection.Add(new ScaleTransform(1, 1.5));
            transformCollection.Add(new TranslateTransform(30, 0));
            transformCollection.Add(new RotateTransform(45, 80, 80));
            TransformGroup group = new TransformGroup();
            group.Children = transformCollection;
            _milCompoundDecorator.LayoutTransform = group;

            _milRotateDecorator = new Decorator();
            _milRotateDecorator.LayoutTransform = new RotateTransform(-45, 0, 0);

            _milScaleDecorator = new Decorator();
            _milScaleDecorator.LayoutTransform = new ScaleTransform(1.5, 0);

            _milSkewDecorator = new Decorator();
            _milSkewDecorator.LayoutTransform = new SkewTransform(10, -10);

            _milTranslateDecorator = new Decorator();
            _milTranslateDecorator.LayoutTransform = new TranslateTransform(80, 0);

            _milDecorators = new Decorator[] {
                _milCompoundDecorator, _milRotateDecorator, _milScaleDecorator,
                _milSkewDecorator, _milTranslateDecorator };
            _milDecoratorDescriptions = new string[] {
                "compound decorator", "rotate decorator", "scale decorator",
                "skew decorator", "translate decorator" };

            Log("Reseting the TextBox to a known position...");
            Canvas.SetTop(TestTextBox, 100);
            Canvas.SetLeft(TestTextBox, 100);

            t = TestTextBox;
            ((Panel)VisualTreeHelper.GetParent(t)).Children.Remove(t);
            _milCanvas.Children.Add(t);
            MainWindow.Content = _milCanvas;

            Log("Testing background brush...");
            t.Background = new LinearGradientBrush(Colors.Black, Colors.LightYellow, 0);
            t.Text = "";
            QueueHelper.Current.QueueDelegate(VerifyBackgroundBrush);
        }

        private void VerifyBackgroundBrush()
        {
            Log("Verifying bitmap...");
            Bitmap bitmap = BitmapCapture.CreateBitmapFromElement(TestTextBox);
            System.Drawing.Color color = bitmap.GetPixel(4, 20);
            float brightness = color.GetBrightness();
            Log("Brightness of color on left: " + brightness);
            Verifier.Verify(brightness < 0.5, "Brightness is closer to black (0) than to white (1)", true);

            color = bitmap.GetPixel(bitmap.Width - 4, 20);
            brightness = color.GetBrightness();
            Log("Brightness of color on right: " + brightness);
            Verifier.Verify(brightness > 0.5, "Brightness is closer to white (1) than to black (0)", true);

            Log("Testing foreground brush...");
            TestTextBox.Background = System.Windows.Media.Brushes.White;
            TestTextBox.Foreground = new LinearGradientBrush(Colors.Black, Colors.Yellow, 0);

            TestTextBox.Text = EnglishText;

            QueueHelper.Current.QueueDelegate(VerifyForegroundBrush);
        }

        private void VerifyForegroundBrush()
        {
            // Verify that there is a single, continuous gradient, by checking that the
            // brightness is always increasing from left to right.
            Log("Verifying bitmap...");
            Bitmap bitmap = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            // The value of y is known to intersect glyphs
            float lightSkyBlueHue = System.Drawing.Color.LightSkyBlue.GetHue();
            const int y = 11;
            int lastBrightnessFound = -1;
            float lastBrightness = 0;
            float lastPassBrightness = 0;
            bool oneErrorAllowed = false;
            System.Drawing.Color color;
            for (int x = 0; x < bitmap.Width; x++)
            {
                color = bitmap.GetPixel(x, y);
                float brightness = color.GetBrightness();
                // Ignore close-to-white pixels.
                if (brightness > 0.80)
                {
                    continue;
                }
                // Ignore light bluish pixels , as these are yellow-on-white artifacts.
                if (color.GetHue() - lightSkyBlueHue * 0.10 >= lightSkyBlueHue &&
                    color.GetHue() + lightSkyBlueHue * 0.10 <= lightSkyBlueHue)
                {
                    continue;
                }

                if (brightness >= lastBrightness)
                {
                    oneErrorAllowed = true;
                    lastPassBrightness = lastBrightness;
                    lastBrightness = brightness;
                    lastBrightnessFound = x;
                }
                else if (brightness >= lastPassBrightness)
                {
                    oneErrorAllowed = true;
                    lastBrightness = brightness;
                    lastBrightnessFound = x;
                }
                else
                {
                    if (oneErrorAllowed)
                    {
                        oneErrorAllowed = false;
                    }
                    else
                    {
                        Logger.Current.LogImage(bitmap, "foreground");
                        throw new Exception("Brightness seems to decrease " +
                            "from left to right (near column " + x + ", row " + y + ") - " +
                            "value " + brightness + " less than " + lastBrightness + " at column " + lastBrightnessFound);
                    }
                }
            }
            if (lastBrightness == 0)
            {
                Logger.Current.LogImage(bitmap, "foreground");
                throw new Exception("All exceptions were found - probably a white line was selected.");
            }

            Log("Brightness is increasing from left to right.");

            QueueHelper.Current.QueueDelegate(VerifyTransformations);
        }

        private void VerifyTransformations()
        {
            TextBox t;  // TextBox being tested.

            // After all decorators are tested, move on to animations.
            if (_milDecoratorIndex >= _milDecorators.Length)
            {
                QueueHelper.Current.QueueDelegate(VerifyAnimations);
            }
            else
            {
                Log("Testing transformation decorator: " +
                    _milDecoratorDescriptions[_milDecoratorIndex]);
                t = TestTextBox;
                if (_milDecoratorIndex == 0)
                {
                    // Shrink the font so the text fits without scrolling.
                    t.Text = EnglishText + HebrewText;
                    t.FontSize = 18;
                    t.Foreground = System.Windows.Media.Brushes.Black;
                    _milCanvas.Children.Remove(t);
                }
                else
                {
                    // We verify that some selection exists. Because we also
                    // use right-to-left, the selection rules are more complicated
                    // and that text may not actually be selected. Three characters
                    // should be left-to-right per settings, however.
                    Log("Verifying that transformed text was selected: " + t.SelectedText);
                    Verifier.Verify(t.SelectionLength >= 3,
                        "Selection is at least three characters long", true);
                    _milDecorators[_milDecoratorIndex - 1].Child = null;
                }
                _milCanvas.Children.Add(_milDecorators[_milDecoratorIndex]);
                _milDecorators[_milDecoratorIndex].Child = t;

                _milDecoratorIndex++;

                QueueHelper.Current.QueueDelegate(MILSelectTransformed);
            }
        }

        private void MILSelectTransformed()
        {
            Rect startRect;                     // Start of selection.
            System.Windows.Point startPoint;    // Start of selection point.
            Rect endRect;                       // End of selection.
            System.Windows.Point endPoint;      // End of selection point.

            Log("Selecting text on transformed TextBox.");
            Log(VisualLogger.DescribeVisualTree(TestTextBox));

            startRect = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox).GetCharacterRect(LogicalDirection.Forward);
            endRect = Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox).GetCharacterRect(LogicalDirection.Backward);

            startPoint = new System.Windows.Point(
                startRect.X + startRect.Width / 2, startRect.Y + startRect.Height / 2);
            endPoint = new System.Windows.Point(
                endRect.X + endRect.Width / 2, endRect.Y + endRect.Height / 2);

            startPoint = ElementUtils.GetScreenRelativePoint(TestTextBox, startPoint);
            endPoint = ElementUtils.GetScreenRelativePoint(TestTextBox, endPoint);

            MouseInput.MouseDragPressed(startPoint, endPoint);

            QueueHelper.Current.QueueDelegate(VerifyTransformations);
        }

        private SolidColorBrush CreateAnimatedBrush(MediaColor startColor,
            MediaColor endColor, int milliseconds, FillBehavior fill)
        {
            SolidColorBrush result;     // Brush created.
            ColorAnimation animation;   // Animation for brush color.
            TimeSpan timeDuration;          // Duration for animation.

            timeDuration = TimeSpan.FromMilliseconds(milliseconds);
            animation = new ColorAnimation(startColor, endColor, new Duration(timeDuration), fill);

            result = new SolidColorBrush(startColor);
            result.BeginAnimation(SolidColorBrush.ColorProperty, animation);
            return result;
        }

        private void VerifyAnimations()
        {
            SolidColorBrush foreground;

            foreground = CreateAnimatedBrush(MediaColors.Black, MediaColors.White,
                OneSecondInMs, FillBehavior.HoldEnd);
            TestTextBox.Foreground = foreground;

            ContinueSequence();
        }

        #endregion MIL integration testing.

        #endregion Main flow.
    }

    /// <summary>
    /// Regression coverage for Regression_Bug574. Performs the following operations on 
    /// TextBox and RichTextBox with its width animating.
    /// 1. MouseClick to get focus
    /// 2. Type some content
    /// 3. MouseClick on button to get focus away
    /// 4. MouseClick to get focus back
    /// 5. Type some more contents
    /// 6. Verify that contents typed are in TB/RTB
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("26"), TestBugs("574")]
    public class TBBAnimationTest : CombinedTestCase
    {
        #region Private fields

        private UIElementWrapper _wrapper;
        private Button _focusAwayButton;
        private string _typeContent1 = "This UIElement has its width animating.";
        private string _typeContent2 = "The animation is forever.";

        private string _xamlForRTB = @"<WrapPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            @"<WrapPanel.Triggers>" +
            @"<EventTrigger RoutedEvent='FrameworkElement.Loaded'>" +
            @"<EventTrigger.Actions>" +
            @"<BeginStoryboard>" +
            @"<Storyboard>" +
            @"<DoubleAnimation " +
            @"Storyboard.TargetProperty='(RichTextBox.Width)' " +
            @"Storyboard.TargetName='RTB' From='200' To='210' " +
            @"BeginTime='0:0:0' " + 
            @"Duration='0:0:10' " +
            @"AutoReverse='True' " +
            @"RepeatBehavior='Forever'/>" +
            @"</Storyboard>" +
            @"</BeginStoryboard>" +
            @"</EventTrigger.Actions>" +
            @"</EventTrigger>" +
            @"</WrapPanel.Triggers>" +
            @"<RichTextBox Name='RTB' Margin='20' Height='200' Width='200' Background='LightYellow' MinWidth='0'></RichTextBox>" +
            @"<Button Name='TestButton'>Focus away button</Button>" +
            @"</WrapPanel>";

        private string _xamlForTB = @"<WrapPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            @"<WrapPanel.Triggers>" +
            @"<EventTrigger RoutedEvent='FrameworkElement.Loaded'>" +
            @"<EventTrigger.Actions>" +
            @"<BeginStoryboard>" +
            @"<Storyboard>" +
            @"<DoubleAnimation " +
            @"Storyboard.TargetProperty='(TextBox.Width)' " +
            @"Storyboard.TargetName='TB' From='200' To='210' " +
            @"BeginTime='0:0:0' " +
            @"Duration='0:0:10' " +
            @"AutoReverse='True' " +
            @"RepeatBehavior='Forever'/>" +
            @"</Storyboard>" +
            @"</BeginStoryboard>" +
            @"</EventTrigger.Actions>" +
            @"</EventTrigger>" +
            @"</WrapPanel.Triggers>" +
            @"<TextBox Name='TB' Margin='20' Height='200' Width='200' Background='LightBlue' MinWidth='0'></TextBox>" +
            @"<Button Name='TestButton'>Focus away button</Button>" +
            @"</WrapPanel>";

        #endregion Private fields

        /// <summary>start test case</summary>
        public override void RunTestCase()
        {
            //Start the case for TextBox
            MainWindow.Content = (WrapPanel)XamlUtils.ParseToObject(_xamlForTB);
            _wrapper = new UIElementWrapper((UIElement)ElementUtils.FindElement(MainWindow, "TB"));            

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            MouseInput.MouseClick(_wrapper.Element);
            QueueDelegate(DoTyping);
        }

        private void DoTyping()
        {
            KeyboardInput.TypeString(_typeContent1);
            QueueDelegate(DoFocusAway);
        }

        private void DoFocusAway()
        {
            _focusAwayButton = (Button)ElementUtils.FindElement(MainWindow, "TestButton");
            MouseInput.MouseClick(_focusAwayButton);
            QueueDelegate(DoFocusBack);
        }

        private void DoFocusBack()
        {
            MouseInput.MouseClick(_wrapper.Element);
            QueueDelegate(DoTypingAgain);
        }

        private void DoTypingAgain()
        {
            KeyboardInput.TypeString("{END}" + _typeContent2);
            QueueDelegate(VerifyTyping);
        }

        private void VerifyTyping()
        {
            string totalContent = _typeContent1 + _typeContent2;

            Verifier.Verify(_wrapper.Text.Contains(totalContent),
                "Verifying the typed content in TextBox/RichTextBox", true);

            if (_wrapper.Element is RichTextBox)
            {
                EndTest();
            }
            else
            {
                //Start the case for RichTextBox
                MainWindow.Content = (WrapPanel)XamlUtils.ParseToObject(_xamlForRTB);
                _wrapper = new UIElementWrapper((UIElement)ElementUtils.FindElement(MainWindow, "RTB"));                

                QueueDelegate(DoFocus);
            }
        }        
    }
}

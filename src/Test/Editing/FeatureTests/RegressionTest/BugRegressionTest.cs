// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Regression test Regression_Bug119
    /// </summary>
    [Test(3, "Editor", "Regression_Bug119", MethodParameters = "/TestCaseType:Regression_Bug119 /xml:regressionxml.xml")]
    [TestOwner("Microsoft"), TestTactics("441"), TestTitle("Regression_Bug119 Regression Test")]
    public class Regression_Bug119 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            string mainXamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            ActionItemWrapper.SetMainXaml(mainXamlString);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {

            this._textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;

            if (this._textBox == null)
            {
                throw new Exception("Cannot find [TextBox1]");
            }

            Rect rect = ElementUtils.GetScreenRelativeRect(this._textBox);


            Point startPoint = new Point(rect.Right + 100, rect.Bottom + 100);
            Point endPoint = new Point(rect.Right - 100, rect.Bottom - 100);

            MouseInput.MouseDown(startPoint);
            MouseInput.MouseDrag(startPoint, endPoint);
            MouseInput.MouseUp();

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMouseInput));
        }

        private void OnMouseInput()
        {
            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;

    }

    /// <summary>
    /// Regression_Bug291
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug291 Regression Test")]
    public class Regression_Bug291 : ActionDrivenTest
    {
        /// <summary>
        /// This method tries to simulate rightdrag selecting text in textbox
        /// and this is called by the ActionManager directly
        /// WARNING: This code should only be used in Regression_Bug291 regression test
        /// no other part of the test code should call this.
        /// </summary>
        /// <param name="wrapper">UIElementWrapper instance to operate on</param>
        public static void SelectAllCharactersByRightMouseButton(object wrapper)
        {
            if (wrapper == null || wrapper as UIElementWrapper == null)
            {
                throw new ArgumentException("wrapper is not an instance of type UIElementWrapper");
            }

            UIElementWrapper elementWrapper = wrapper as UIElementWrapper;

            Rect rect = elementWrapper.GetGlobalCharacterRect(0);
            Rect rect1 = elementWrapper.GetGlobalCharacterRectOfLastCharacter();

            Point startPoint = new Point(MathUtils.GetSmallestPossibleIntegerValueWithinTheRect(rect), rect.Top + rect.Height / 2); ;

            Point endPoint = new Point(Math.Ceiling(rect1.Right), rect1.Top + rect1.Height / 2);
            MouseInput.RightMouseDown(startPoint);
            MouseInput.MouseDrag(startPoint, endPoint);
            MouseInput.RightMouseUp();
        }
    }

    /// <summary>
    /// Regression_Bug120
    /// </summary>
    [Test(1, "TextOM", "Regression_Bug120", MethodParameters = "/TestCaseType=Regression_Bug120 /xml:regressionxml.xml")]
    [TestOwner("Microsoft"), TestTitle("Regression_Bug120 Regression Test"), TestTactics("442")]
    public class Regression_Bug120 : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TextRange textRange;
            string stringInInline;
            Paragraph paragraph = new Paragraph();
            Run inlineElement = new Run();

            this._textBox = new RichTextBox();
            MainWindow.Content = this._textBox;

            textRange = new TextRange(this._textBox.Document.ContentStart, this._textBox.Document.ContentEnd);
            this._textBox.Document.Blocks.Clear();
            this._textBox.Document.Blocks.Add(paragraph);

            paragraph.Inlines.Add(inlineElement);
            this._inLineTextRange = new TextRange(inlineElement.ContentStart, inlineElement.ContentEnd);
            //--this._inLineTextRange = textRange.AppendElement(typeof(Inline));

            stringInInline = ConfigurationSettings.Current.GetArgument("StringInInline");

            this._inLineTextRange.End.InsertTextInRun(stringInInline);
            //--this._inLineTextRange.AppendText(stringInInline);

            QueueDelegate(SetElementProperty1);
        }

        private void SetElementProperty1()
        {
            this._inLineTextRange.ApplyPropertyValue(TextBlock.FontSizeProperty, 40.0);
            QueueDelegate(SetElementProperty2);
        }

        private void SetElementProperty2()
        {
            this._inLineTextRange.ApplyPropertyValue(TextBlock.ForegroundProperty, Brushes.Red);
            QueueDelegate(ClearElementValue1);
        }

        private void ClearElementValue1()
        {
            this._inLineTextRange.ApplyPropertyValue(TextBlock.FontSizeProperty, 11.0);
            //--this._inLineTextRange.ClearProperty(TextBlock.FontSizeProperty);
            QueueDelegate(ClearElementValue2);
        }

        private void ClearElementValue2()
        {
            this._inLineTextRange.ApplyPropertyValue(TextBlock.FontSizeProperty, 11.0);
            this._inLineTextRange.ApplyPropertyValue(TextBlock.ForegroundProperty, Brushes.Black);
            //--this._inLineTextRange.ClearProperty(TextBlock.ForegroundProperty);
            //--this._inLineTextRange.ClearProperty(TextBlock.FontSizeProperty);
            QueueDelegate(OnTestDone);
        }

        private void OnTestDone()
        {
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private data.

        private RichTextBox _textBox = null;

        private TextRange _inLineTextRange = null;

        #endregion Private data.
    }

    /// <summary>
    /// Regression_Bug293
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug293 Regression Test"), TestTactics("443")]
    public class Regression_Bug293 : CustomActionDrivenTest
    {
        /// <summary>
        /// ActionDrivenTestRunTestCase
        /// </summary>
        protected override void ActionDrivenTestRunTestCase()
        {
            s_window = base.MainWindow;
        }

        /// <summary>
        /// HookUpEventHandler
        /// </summary>
        public static void HookUpEventHandler()
        {
            s_textBox = ElementUtils.FindElement(s_window, "TextBox1") as TextBox;

            if (s_textBox == null)
            {
                throw new Exception("Cannot find [TextBox1] in the xaml page");
            }

            s_textBox.AddHandler(Keyboard.KeyDownEvent, new KeyEventHandler(OnKeyDown), true);
        }

        private static void OnKeyDown(object sender, KeyEventArgs args)
        {
            if (args.Key == System.Windows.Input.Key.A)
            {
                args.Handled = true;
            }
        }

        private static TextBox s_textBox = null;

        private static Window s_window;
    }

    /// <summary>
    /// Regression_Bug121
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug121 Regression Test"), TestTactics("444")]
    [Test(3, "Selection", "Regression_Bug121", MethodParameters = "/TestCaseType:Regression_Bug121 /xml:regressionxml.xml")]
    public class Regression_Bug121 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            string mainXamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            ActionItemWrapper.SetMainXaml(mainXamlString);
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            this._cachedElement = base.MainWindow.Content as FrameworkElement;

            base.MainWindow.Content = null;
            QueueDelegate(ReloadWindowContent);
        }

        private void ReloadWindowContent()
        {
            base.MainWindow.Content = this._cachedElement;
            QueueDelegate(OnReloadWindowContent);
        }

        private void OnReloadWindowContent()
        {
            this._textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;

            Verifier.Verify(this._textBox != null, "[TextBox1] can be found");

            MouseInput.MouseClick(this._textBox);

            this._strInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");

            KeyboardInput.TypeString(this._strInTextBox);
            QueueDelegate(OnInputDone);
        }

        private void OnInputDone()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(this._textBox);
            TextPointer textboxStart = Test.Uis.Utils.TextUtils.GetTextBoxStart(this._textBox);
            TextPointer textboxEnd = Test.Uis.Utils.TextUtils.GetTextBoxEnd(this._textBox);
            textSelection.Select(textboxStart, textboxEnd);

            Log("Expected selection: [" + this._strInTextBox + "]");
            Log("Actual selection:   [" + textSelection.Text + "]");
            Verifier.Verify(textSelection.Text == this._strInTextBox,
                "TextContainer.Text retrieve correct text");
            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;

        private FrameworkElement _cachedElement = null;

        private string _strInTextBox = String.Empty;
    }

    /// <summary>
    /// Regression_Bug302
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug302 Regression Test"), TestTactics("445")]
    [Test(3, "TextBox", "Regression_Bug302", MethodParameters = "/TestCaseType:Regression_Bug302 /xml:regressionxml.xml")]
    public class Regression_Bug302 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            this._stringInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");

            Window window = base.MainWindow;
            this._textbox = new TextBox();

            //
            // we need calling Addhandler (instead of TextBox.MouseButtonDown / Up += 
            // since mouse button down / up are handled before they get to here.
            // 
            this._textbox.AddHandler(Mouse.MouseUpEvent, new MouseButtonEventHandler(OnMouseLeftButtonUp), true);
            this._textbox.AddHandler(Mouse.MouseDownEvent, new MouseButtonEventHandler(OnMouseLeftButtonDown), true);

            this._textbox.MouseMove += new MouseEventHandler(OnMouseMove);
            this._textbox.KeyDown += new KeyEventHandler(OnKeyDown);
            this._textbox.KeyUp += new KeyEventHandler(OnKeyUp);

            window.Content = this._textbox;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnUICreated));
        }

        private void OnMouseMove(object sender, MouseEventArgs args)
        {
            VerifyObjectAsType(sender, typeof(TextBox));

            //
            // We can't accurate keep track of mouse move count
            // 
            this._mouseMoved = true;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton != MouseButton.Left) return;

            VerifyObjectAsType(sender, typeof(TextBox));
            this._mouseLeftButtonUpCount++;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs args)
        {
            if (args.ChangedButton != MouseButton.Left) return;

            VerifyObjectAsType(sender, typeof(TextBox));
            this._mouseLeftButtonDownCount++;
        }

        private void OnKeyDown(object sender, KeyEventArgs args)
        {
            VerifyObjectAsType(sender, typeof(TextBox));
            this._keyDownCount++;
        }

        private void OnKeyUp(object sender, KeyEventArgs args)
        {
            VerifyObjectAsType(sender, typeof(TextBox));
            this._keyUpCount++;
        }

        private void VerifyObjectAsType(object sender, Type type)
        {
            Verifier.Verify(sender != null, "[sender] is not null");

            Type typeOfObject = sender.GetType();

            string message = String.Format("[sender] is an instance of type or subclass [{0}]", type.Name);

            Verifier.Verify(typeOfObject.IsSubclassOf(type) || typeOfObject.Equals(type), message);
        }

        private void OnUICreated()
        {
            MouseInput.MouseClick(this._textbox);

            KeyboardInput.TypeString(this._stringInTextBox);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnTestComplete));
        }

        private void OnTestComplete()
        {
            Verifier.Verify(this._mouseMoved, "Mouse move is detected");

            string message = String.Format("MouseLeftButtonUpCount is correct. Count [{0}]",
                   this._mouseLeftButtonUpCount.ToString());

            Verifier.Verify(this._mouseLeftButtonUpCount == 1, message, true);

            message = String.Format("MouseLeftButtonDownCount is correct. Count [{0}]",
                   this._mouseLeftButtonDownCount.ToString());

            Verifier.Verify(this._mouseLeftButtonDownCount == 1, message, true);

            message = String.Format("KeyUpCount is correct. Count [{0}]",
                   this._keyUpCount.ToString());

            Verifier.Verify(this._keyUpCount == this._stringInTextBox.Length, message, true);

            string stringWithOutSpace = this._stringInTextBox.Replace(" ", "");
            message = String.Format("KeyDownCount is correct. Count [{0}]",
                   this._keyDownCount.ToString());

            Verifier.Verify(this._keyDownCount == stringWithOutSpace.Length, message, true);

            Logger.Current.ReportSuccess();
        }

        private TextBox _textbox;

        private bool _mouseMoved = false;
        private int _mouseLeftButtonUpCount = 0;
        private int _mouseLeftButtonDownCount = 0;
        private int _keyUpCount = 0;
        private int _keyDownCount = 0;

        private string _stringInTextBox;
    }

    /// <summary>
    /// Regression_Bug439
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug439 Regression Test"), TestTactics("446")]
    [Test(3, "Selection", "Regression_Bug439", MethodParameters = "/TestCaseType:Regression_Bug439")]
    public class Regression_Bug439 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            this._textBox = new TextBox();

            base.MainWindow.Content = this._textBox;

            this._textBox.Text = "Test string";

            this._textBox.SelectAll();

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(this._textBox);
            textSelection.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnApplyProperty));
        }

        private void OnApplyProperty()
        {
            Verifier.Verify(this._textBox.Foreground != Brushes.Red, "Setting Foreground property on the text changes that property on TextBox");

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug431
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug431 Regression Test"), TestTactics("447")]
    [Test(3, "Selection", "Regression_Bug431", MethodParameters = "/TestCaseType:Regression_Bug431 /xml:regressionxml.xml")]
    public class Regression_Bug431 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            StackPanel root = new StackPanel();

            this._textBox = new TextBox();

            MainWindow.Content = new StackPanel();

            ((IAddChild)MainWindow.Content).AddChild(this._textBox);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnControlsRendered));

        }


        private void OnControlsRendered()
        {
            MouseInput.MouseClick(this._textBox);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMouseInput));

        }

        private void OnMouseInput()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(this._textBox);
            textSelection.Text = String.Empty;

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug432
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug432 Regression Test"), TestTactics("448")]
    [Test(3, "TextBox", "Regression_Bug432", MethodParameters = "/TestCaseType:Regression_Bug432")]
    public class Regression_Bug432 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            TextBox textbox = new TextBox();

            IAddChild addChild = textbox as IAddChild;

            string message = String.Format("TextBox implements IAddChild");
            Verifier.Verify(addChild != null, message, true);

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Regression_Bug433
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug433 Regression Test"), TestTactics("449")]
    [Test(3, "TextBox", "Regression_Bug433", MethodParameters = "/TestCaseType:Regression_Bug433 /xml:regressionxml.xml")]
    public class Regression_Bug433 : CustomTestCase
    {
        /// <summary>
        /// Entry Point
        /// </summary>
        public override void RunTestCase()
        {
            DockPanel dockPanel = new DockPanel();

            MainWindow.Content = dockPanel;

            this._textbox = new TextBox();

            dockPanel.Children.Add(this._textbox);

            MainWindow.Height = 200;

            MainWindow.Width = 200;

            this._textbox.Height = 20;
            //this._textbox.Width = new Length(100, UnitType.Percent);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(DoUICreate));
        }

        private void DoUICreate()
        {
            this._textboxOriginalWidth = this._textbox.ActualWidth;

            double originalWindowWidth = base.MainWindow.Width;

            int resizeDelta = ConfigurationSettings.Current.GetArgumentAsInt("ResizeDelta");

            this._resizeDelta = resizeDelta;

            base.MainWindow.Width = (originalWindowWidth + this._resizeDelta);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnWindowResize));
        }

        private void OnWindowResize()
        {
            double finalWidth = (this._textboxOriginalWidth + this._resizeDelta);

            Verifier.Verify((this._textbox.ActualWidth).Equals(finalWidth), "Width of TextBox is changed");
            Logger.Current.ReportSuccess();
        }

        private TextBox _textbox = null;
        private double _textboxOriginalWidth;
        private double _resizeDelta;
    }

    /// <summary>
    /// Regression_Bug265
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug265 Regression Test")]
    public class Regression_Bug265 : CustomTestCase
    {
        #region Main flow.

        /// <summary>
        /// Entry point
        /// </summary>
        public override void RunTestCase()
        {
            this._elementWrapper = new UIElementWrapper(new TextBox());
            MainWindow.Content = (FrameworkElement)this._elementWrapper.Element;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnAdded));
        }

        private void OnAdded()
        {
            TextBox textbox = this._elementWrapper.Element as TextBox;

            if (textbox != null)
            {
                textbox.AppendText(ConfigurationSettings.Current.GetArgument("TestString"));
            }

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnTextAppended));
        }

        private void OnTextAppended()
        {
            int characterOffsetFromTextPosition = this._elementWrapper.Start.GetOffsetToPosition(this._elementWrapper.Start);

            int characterOffsetFromTextNavigator = this._elementWrapper.Start.GetOffsetToPosition(this._elementWrapper.Start);

            string message = String.Format("CharacterOffset(TextPosition) returns the same value as CharacterOffset(TextNavigator)");
            Verifier.Verify(characterOffsetFromTextPosition == characterOffsetFromTextNavigator,
                message,
                true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        private UIElementWrapper _elementWrapper = null;
    }

    /// <summary>
    /// Regression_Bug434
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug434 Regression Test"), TestTactics("450")]
    [Test(3, "RichTextBox", "Regression_Bug434", MethodParameters = "/TestCaseType:Regression_Bug434 /xml:regressionxml.xml")]
    public class Regression_Bug434 : CustomTestCase
    {
        /// <summary>
        /// Entry Point
        /// </summary>
        public override void RunTestCase()
        {
            this._richTextBox = new RichTextBox();
            base.MainWindow.Content = this._richTextBox;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            Paragraph paragraph = new Paragraph();

            this._richTextBox.Document.Blocks.Clear();
            this._richTextBox.Document.Blocks.Add(paragraph);

            TextSelection textSelection = this._richTextBox.Selection;

            paragraph.Inlines.Add("Test");

            paragraph.Inlines.Add(new Button());
            paragraph.Inlines.Add(new CheckBox());
            paragraph.Inlines.Add(new ListBox());
            QueueDelegate(OnWalkTextTree);
        }

        private void OnWalkTextTree()
        {
            TextPointer tn;

            tn = this._richTextBox.Document.Blocks.FirstBlock.ContentStart;
            tn = tn.GetInsertionPosition(LogicalDirection.Forward);
            tn = tn.GetNextContextPosition(LogicalDirection.Forward); //move over the text contents

            //To skip </R><InlineUIContainer>
            tn = tn.GetPositionAtOffset(2);
            VerifyObject(tn, typeof(Button));
            tn = tn.GetNextContextPosition(LogicalDirection.Forward);

            //To skip </InlineUIContainer><InlineUIContainer>
            tn = tn.GetPositionAtOffset(2);
            VerifyObject(tn, typeof(CheckBox));
            tn = tn.GetNextContextPosition(LogicalDirection.Forward);

            //To skip </InlineUIContainer><InlineUIContainer>
            tn = tn.GetPositionAtOffset(2);
            VerifyObject(tn, typeof(ListBox));

            Logger.Current.ReportSuccess();
        }

        private void VerifyObject(TextPointer tn, Type type)
        {
            TextPointerContext textSymbolType;
            string message1;
            object obj;

            textSymbolType = tn.GetPointerContext(LogicalDirection.Forward);

            Verifier.Verify(textSymbolType == TextPointerContext.EmbeddedElement,
                "TextPointer.GetPointerContext returns non-embedded object");

            obj = tn.GetAdjacentElement(LogicalDirection.Forward);

            message1 = String.Format("TextPointer,GetEmbeddedObject returns object of type [{0}], expected of type [{1}]",
                obj.GetType().ToString(),
                type.ToString());

            Verifier.Verify(obj.GetType().Equals(type));
        }

        private RichTextBox _richTextBox = null;
    }

    /// <summary>
    /// Regression_Bug345
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug345 Regression Test"), WindowlessTest(true), TestTactics("451")]
    [Test(3, "TextBox", "Regression_Bug345", MethodParameters = "/TestCaseType:Regression_Bug345 /xml:regressionxml.xml")]
    public class Regression_Bug345 : CustomTestCase
    {
        /// <summary>
        /// Entry point
        /// </summary>
        public override void RunTestCase()
        {
            FlowDocumentScrollViewer container = new FlowDocumentScrollViewer();
            container.Document = new FlowDocument(new Paragraph(new Run()));
            TextPointer startPointer = ((Paragraph)container.Document.Blocks.FirstBlock).Inlines.FirstInline.ContentStart;
            TextPointer endPointer = ((Paragraph)container.Document.Blocks.FirstBlock).Inlines.FirstInline.ContentEnd;

            string strInTextTree = ConfigurationSettings.Current.GetArgument("TestString");

            TextRange textRange = new TextRange(startPointer, endPointer);

            textRange.End.InsertTextInRun(strInTextTree);

            int textPositionStartGetTextLength = startPointer.GetTextRunLength(LogicalDirection.Forward);

            int textPositionEndGetTextLength = endPointer.GetTextRunLength(LogicalDirection.Backward);

            int textNavigatorStartGetTextLength = startPointer.GetTextRunLength(LogicalDirection.Forward);
            int textNavigatorEndGetTextLength = endPointer.GetTextRunLength(LogicalDirection.Backward);

            string message = String.Format("TextPointer.GetTextLength(Forward) returns [{0}], TextPointer.GetTextLength(Forward) returns [{1}]",
                textPositionStartGetTextLength.ToString(),
                textNavigatorStartGetTextLength.ToString());

            Verifier.Verify(textPositionStartGetTextLength == textNavigatorStartGetTextLength, message);

            message = String.Format("TextPointer.GetTextRunLength(Backward) returns [{0}], TextPointer.GetTextRunLength(Backward) returns [{1}]",
                textPositionEndGetTextLength.ToString(),
                textNavigatorEndGetTextLength.ToString());

            Verifier.Verify(textPositionEndGetTextLength == textNavigatorEndGetTextLength, message);

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Regression_Bug435
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug435 Regression Test")]
    public class Regression_Bug435 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            TextBlock text1 = new TextBlock();
            Bold bold1 = new Bold();

            TextBlock text2 = new TextBlock();
            Bold bold2 = new Bold();

            bold1.Inlines.Clear();
            bold1.Inlines.Add(new Run("Hello"));
            ((IAddChild)text1).AddChild(bold1);
            base.MainWindow.Content = text1;

            ((IAddChild)text2).AddChild(bold2);
            bold2.Inlines.Clear();
            bold2.Inlines.Add(new Run("Hello"));
            base.MainWindow.Content = text2;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Regression_Bug298
    /// </summary>
    [Test(3, "TextOM", "Regression_Bug298", MethodParameters = "/TestCaseType=Regression_Bug298 /xml:regressionxml.xml")]
    [TestOwner("Microsoft"), TestTitle("Regression_Bug298 Regression Test"), TestTactics("452"), TestLastUpdatedOn("Jan. 25, 2007")]
    public class Regression_Bug298 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            string mainXamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            ActionItemWrapper.SetMainXaml(mainXamlString);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            this._textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;

            if (this._textBox == null)
            {
                throw new Exception("Cannot find [TextBox1]");
            }

            //-TextContainer textContainer = this._textBox.Selection.TextContainer;

            TextRange textRange = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(this._textBox),
                                                Test.Uis.Utils.TextUtils.GetTextBoxEnd(this._textBox));
            //--TextRange textRange = new TextRange(textContainer.Start, textContainer.End);

            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, Brushes.Red);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnSetForegroundRed));
        }

        private void OnSetForegroundRed()
        {
            //--TextContainer textContainer = this._textBox.Selection.TextContainer;

            TextRange textRange = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(this._textBox),
                                                Test.Uis.Utils.TextUtils.GetTextBoxEnd(this._textBox));
            //--TextRange textRange = new TextRange(textContainer.Start, textContainer.End);

            textRange.ApplyPropertyValue(TextElement.ForegroundProperty, null);

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug122
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug122 Regression Test"), TestTactics("453")]
    public class Regression_Bug122 : ActionDrivenTest
    {
        /// <summary>
        /// ActionDrivenTestRunTestCase
        /// </summary>
        protected override void ActionDrivenTestRunTestCase()
        {
            s_testWindow = base.MainWindow;
        }

        /// <summary>
        /// TestCheckBoxChecked
        /// </summary>
        /// <param name="id"></param>
        /// <param name="isChecked"></param>
        public static void TestCheckBoxChecked(string id, bool isChecked)
        {
            CheckBox checkBox = ElementUtils.FindElement(s_testWindow, id) as CheckBox;

            if (checkBox == null)
            {
                throw new ApplicationException("Cannot find [CheckBox1]");
            }

            string message = String.Format("CheckBox is checked [{0}]", checkBox.IsChecked.ToString());

            Verifier.Verify(checkBox.IsChecked == isChecked, message);
        }

        private static Window s_testWindow;
    }

    /// <summary>
    /// MyTextElement
    /// </summary>
    public class MyTextElement : Bold
    {
        /// <summary>
        /// MyTextElement
        /// </summary>
        public MyTextElement()
            : base()
        {
            Loaded += new RoutedEventHandler(OnLoaded);
        }

        private void OnLoaded(object sender, RoutedEventArgs args)
        {
            Regression_Bug123.LoadedInvoked(this);
        }
    }

    /// <summary>
    /// Regression_Bug123
    /// </summary>
    [Test(1, "TextOM", "Regression_Bug123", MethodParameters = "/TestCaseType=Regression_Bug123")]
    [TestOwner("Microsoft"), TestTitle("Regression_Bug123 Regression Test"), TestTactics("454"), TestLastUpdatedOn("Jan 25, 2007")]
    public class Regression_Bug123 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            FlowDocument document = new FlowDocument();
            Paragraph paragraph = new Paragraph();

            document = new FlowDocument();
            document.Blocks.Add(paragraph);

            // Capture Xaml to create the element.
            TextRange textRange = new TextRange(document.ContentStart, document.ContentEnd);
            XamlUtils.TextRange_SetXml(textRange, CustomXaml);

            Logger.Current.ReportSuccess();
        }

        internal static void LoadedInvoked(MyTextElement element)
        {
            Verifier.VerifyValue("validated element", CustomFontSize, element.FontSize);
        }

        private const string MicrosoftTestingXmlns = "clr-namespace:Test.Uis.TextEditing;assembly=EditingTest";
        private const string CustomXaml =
            "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xmlns:t=\"" + MicrosoftTestingXmlns + "\">" +
            "<Paragraph><t:MyTextElement FontSize='128'>foo</t:MyTextElement></Paragraph></Section>";

        private const double CustomFontSize = 128d;
    }

    /// <summary>
    /// Regression_Bug294
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug294 Regression Test")]
    public class Regression_Bug294 : CustomTestCase
    {
        /// <summary>
        /// Entry Point
        /// </summary>
        public override void RunTestCase()
        {
            string mainXamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            ActionItemWrapper.SetMainXaml(mainXamlString);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            this._textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;

            if (this._textBox == null)
            {
                throw new Exception("Cannot find [TextBox1]");
            }

            MouseInput.MouseClick(this._textBox);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMouseClick));
        }

        private void OnMouseClick()
        {
            TextPointer textNavigator;
            TextPointerContext textSymbolType;

            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(this._textBox);

            textNavigator = textSelection.Start;

            textSymbolType = textNavigator.GetPointerContext(LogicalDirection.Backward);

            // this should be adjacent to the bold element.            
            Verifier.Verify(textSymbolType == TextPointerContext.ElementEnd,
                "Caret is after a TextElement");

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug437
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug437 Regression Test"), TestTactics("455")]
    [Test(3, "TextBox", "Regression_Bug437", MethodParameters = "/TestCaseType:Regression_Bug437")]
    public class Regression_Bug437 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            this._textBox = new TextBox();
            base.MainWindow.Content = this._textBox;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            MouseInput.MouseClick(this._textBox);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMouseInput));
        }

        private void OnMouseInput()
        {
            Verifier.Verify(this._textBox.IsKeyboardFocused == true, "TextBox.IsKeyboardFocused is true");
            Verifier.Verify(Keyboard.FocusedElement == this._textBox, "Keyboard.FocusedElement returns the expected TextBox.");
            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug299
    /// </summary>
    [Test(0, "TextOM", "Regression_Bug298", MethodParameters = "/TestCaseType=Regression_Bug299 /xml:regressionxml.xml")]
    [TestOwner("Microsoft"), TestTitle("Regression_Bug299 Regression Test"), TestTactics("456"), TestLastUpdatedOn("Jan 25, 2007")]
    public class Regression_Bug299 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            string mainXamlString = ConfigurationSettings.Current.GetArgument("MainXaml");

            ActionItemWrapper.SetMainXaml(mainXamlString);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            this._textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;

            if (this._textBox == null)
            {
                throw new ApplicationException("Cannot find [TextBox1]");
            }

            TextRange textRange = new TextRange(Test.Uis.Utils.TextUtils.GetTextBoxStart(this._textBox), Test.Uis.Utils.TextUtils.GetTextBoxEnd(this._textBox));

            textRange.Text = "Some Text";

            textRange.ApplyPropertyValue(Paragraph.TextDecorationsProperty, null);

            string str = XamlUtils.TextRange_GetXml(textRange);

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }

    /// <summary>
    /// Regression_Bug438
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("Regression_Bug438 Regression Test")]
    public class Regression_Bug438 : CustomTestCase
    {
        /// <summary>
        /// entry point
        /// </summary>
        public override void RunTestCase()
        {
            QueueHelper.Current.QueueDelegate(new SimpleHandler(StartTest));
        }

        private void StartTest()
        {
            _textBox = new TextBox();
            base.MainWindow.Content = _textBox;

            QueueHelper.Current.QueueDelegate(new SimpleHandler(DoInput));
        }

        private void DoInput()
        {
            MouseInput.MouseClick(_textBox);

            KeyboardInput.TypeString("This is a test");

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnVerification));
        }

        private void OnVerification()
        {
            string message = String.Format("TextBox text = {0}, expected {1}",
                _textBox.Text, "This is a test");

            Verifier.Verify(_textBox.Text == "This is a test", message);

            Logger.Current.ReportSuccess();
        }

        private TextBox _textBox = null;
    }
}

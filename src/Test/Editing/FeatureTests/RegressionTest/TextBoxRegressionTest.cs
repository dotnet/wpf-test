// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression Tests.

namespace Test.Uis.Regressions
{
    #region Namespaces.
    using System;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Shapes;
    using System.Windows.Automation;

    using System.Xml.XPath;
    using System.Windows.Navigation;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using MTI = Microsoft.Test.Input;
    using Microsoft.Test.Imaging;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the regression bugs doesnt repro.
    /// Test1: BugRepro441    
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("458"), TestBugs("441")]
    [Test(2, "TextBox", "BugRepro441", MethodParameters = "/TestCaseType=BugRepro441")]
    public class BugRepro441 : CustomTestCase
    {
        const string inputXaml = "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Background='white'>" +
            "<Menu DockPanel.Dock='Top'>" +
            "<MenuItem Name='TestMenuItem'>" +
            "<MenuItem.Header>" +
            "<TextBlock>This MenuItem</TextBlock>" +
            "</MenuItem.Header>" +
            "<MenuItem>" +
            "<MenuItem.Header>" +
            "<TextBox Name='TestTB' Height='50' Width='150'>abc</TextBox>" +
            "</MenuItem.Header>" +
            "</MenuItem>" +
            "</MenuItem>" +
            "</Menu>" +
            "<TextBox Name='OtherTB' DockPanel.Dock='Left'>This is a test</TextBox>" +
            "</DockPanel>";       
 
        UIElement _testUIElement;
        TextBox _testTB;
        MenuItem _testMenuItem;        
        bool _doTyping;

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Test1();
        }

        private void Test1()
        {
            _doTyping = false;
            ActionItemWrapper.SetMainXaml(inputXaml);
            QueueDelegate(ClickMenu);
        }

        private void ClickMenu()
        {
            _testMenuItem = ElementUtils.FindElement(MainWindow, "TestMenuItem") as MenuItem;
            MouseInput.MouseClick(_testMenuItem);            
            QueueHelper.Current.QueueDelayedDelegate(new System.TimeSpan(0,0,0,2), new SimpleHandler(ClickTB), null);
        }

        private void ClickTB()
        {
            _testTB = ElementUtils.FindElement(MainWindow, "TestTB") as TextBox;
            _testUIElement = ElementUtils.FindElement(MainWindow, "OtherTB") as UIElement;
            MouseInput.MouseClick(_testTB);

            if (_doTyping)
            {
                KeyboardInput.TypeString("This is a test");
                QueueDelegate(VerifyTypedString);
            }
            else
            {
                _doTyping = true;
                QueueDelegate(ClickTB2);                
            }
        }

        private void ClickTB2()
        {
            Verifier.Verify(_testTB.Text == "abc", "PreTestAction: Check the textbox contents.", true);
            MouseInput.MouseClick(_testUIElement);
            QueueDelegate(ClickMenu);
        }

        private void VerifyTypedString()
        {
            Verifier.Verify(_testTB.Text == "abcThis is a test", "Testing Regression_Bug441", true);
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that the regression bugs doesnt repro.
    /// Test1: Regression_Bug125 (Typing Alt+013 key sequence in TextBox)
    /// Test2a: Regression_Bug129 (Changing the style of textbox inside the TextChanged event handler)
    /// Test2b: Regression_Bug126 (Changing the style of textbox inside the textchanged event handler and then typing in it)
    /// Test3: Regression_Bug127 (Test Home/End keys in RightToLeftTopToBottom configuration)
    /// Test4: Regression_Bug128
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("459"), TestBugs("125, 442, 129, 126, 127, 128, 442")]
    [Test(2, "TextBox", "TestTBRegressions", MethodParameters = "/TestCaseType=TestTBRegressions")]
    public class TestTBRegressions : CustomTestCase
    {        
        TextBox _testTB;
        DockPanel _dPanel;
        bool _styleChanged;
        string _expectedText = "\r\r";
        int _count = 0;

        string _inputString = "ABCDEFGHIJ";
        UIElementWrapper _testTBWrapper;

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Log("Verifying that Regression_Bug125 doesnt regress");
            _testTB = new TextBox();
            _testTB.AcceptsReturn = true;
            MainWindow.Content = _testTB;
            QueueDelegate(DoInputAction);
        }

        private void DoInputAction()
        {
            Log("Typing ALT+013 key sequence on a textbox");
            MouseInput.MouseClick(_testTB);

            MTI.Input.SendKeyboardInput(Key.LeftAlt, true);

            MTI.Input.SendKeyboardInput(Key.NumPad0, true);
            MTI.Input.SendKeyboardInput(Key.NumPad0, false);

            MTI.Input.SendKeyboardInput(Key.NumPad1, true);
            MTI.Input.SendKeyboardInput(Key.NumPad1, false);

            MTI.Input.SendKeyboardInput(Key.NumPad3, true);
            MTI.Input.SendKeyboardInput(Key.NumPad3, false);

            MTI.Input.SendKeyboardInput(Key.LeftAlt, false);
            _count++;

            if (_count < 2)
            {
                QueueDelegate(DoInputAction);
            }
            else
            {
                QueueDelegate(VerifyResult);
            }            
        }

        private void VerifyResult()
        {
            Log("Text in TextBox: [" + _testTB.Text + "]");
            Log("Expected text: [" + _expectedText + "]");
            Log("Skipping verifying steps for Regression_Bug125 due to Regression_Bug442.");
            
            TestRegression_Bug129();
        }

        private void TestRegression_Bug129()
        {
            Log("Verifying that Regression_Bug129 and Regression_Bug126 doesnt regress");

            _testTB = new TextBox();
            _testTB.Width = 400;
            _testTB.Height = 400;

            _testTB.TextChanged += delegate
            {
                if (_styleChanged || _testTB.Text != "foo")
                {
                    return;
                }
                _styleChanged = true;

                FrameworkElementFactory textContent;
                Style style;

                style = new Style();
                textContent = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
                textContent.SetValue(System.Windows.Controls.TextBlock.FocusableProperty, true);
                ControlTemplate template = new ControlTemplate(typeof(TextBox));
                template.VisualTree = textContent;
                style.Setters.Add(new Setter(TextBox.TemplateProperty, template));

                _testTB.Style = style;
            };

            _dPanel = new DockPanel();
            _dPanel.Children.Add(_testTB);
            MainWindow.Content = _dPanel;
            QueueDelegate(TypeFoo);
        }

        private void TypeFoo()
        {
            MouseInput.MouseClick(_testTB);
            KeyboardInput.TypeString("foo");
            QueueDelegate(TypeAbc);
        }

        private void TypeAbc()
        {
            KeyboardInput.TypeString("abc");
            QueueDelegate(VerifyTBContents);
        }

        private void VerifyTBContents()
        {
            Verifier.Verify(_testTB.Text == "fooabc", "Verifying the contents after text is changed", true);
            Log("Regression_Bug129 and Regression_Bug126 didnt repro");
            TestRegression_Bug127();
        }

        private void TestRegression_Bug127()
        {
            //setting xaml which has a TB with RightToLeft
            string xamlString = "<StackPanel Background='white' xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            "<TextBox Name='TBRegression_Bug127' Margin ='10, 10, 10, 10' Height='50' Width='200' FlowDirection='RightToLeft'></TextBox>" +
            "</StackPanel>";

            Log("Verifying that Regression_Bug127 doesnt regress");
            ActionItemWrapper.SetMainXaml(xamlString);
            QueueDelegate(TypeInputString);
        }

        private void TypeInputString()
        {
            _testTB = ElementUtils.FindElement(MainWindow, "TBRegression_Bug127") as TextBox;
            MouseInput.MouseClick(_testTB);
            KeyboardInput.TypeString(_inputString);
            KeyboardInput.TypeString("{END}");
            QueueDelegate(TestEndAction);
        }

        private void TestEndAction()
        {
            _testTBWrapper = new UIElementWrapper(_testTB);
            Verifier.Verify(_testTBWrapper.GetTextOutsideSelection(LogicalDirection.Backward) == _inputString,
                "Verifying the contents after END key is pressed", true);
            KeyboardInput.TypeString("{HOME}");
            QueueDelegate(TestHomeAction);
        }

        private void TestHomeAction()
        {
            Verifier.Verify(_testTBWrapper.GetTextOutsideSelection(LogicalDirection.Forward) == _inputString,
                "Verying the contents after HOME key is pressed", true);
            Log("Regression_Bug127 didnt repro");
            TestRegression_Bug128();
        }

        private void TestRegression_Bug128()
        {
            Log("Verifying that Regression_Bug128 doesnt repro");
            string xamlString = "<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' Background='Black' Width='800' Height='600'>" +
                "<TextBlock Foreground='White' FontFamily='Pristina' FontSize='270pt' Canvas.Left='336' Canvas.Top='120'>1</TextBlock>" +
                "<StackPanel Canvas.Left='680' Canvas.Top='540'>" +
                "<TextBlock Foreground='White'>Name:</TextBlock>" +
                "<TextBox Name='TBRegression_Bug128' Foreground='White' Background='Black' Height='20' Width='100'></TextBox>" +
                "</StackPanel></Canvas>";
            ActionItemWrapper.SetMainXaml(xamlString);
            MainWindow.Height = 650;
            MainWindow.Width = 850;
            QueueDelegate(DoTabbing);
        }

        private void DoTabbing()
        {
            _testTB = ElementUtils.FindElement(MainWindow, "TBRegression_Bug128") as TextBox;
            KeyboardInput.TypeString("{TAB 4}");
            MouseInput.MouseClick(_testTB);
            KeyboardInput.TypeString("This is a test");
            QueueDelegate(VerifyTextInTextBox);
        }

        private void VerifyTextInTextBox()
        {
            Verifier.Verify(_testTB.Text == "This is a test", "Verifying the contents of TB", true);
            Log("Regression_Bug128 didnt repro");
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>Verifies that Regression_Bug130 doesnt repro</summary>
    [TestOwner("Microsoft"), TestTactics("460"), TestBugs("Regression_Bug130")]
    public class BugReproRegression_Bug130 : CustomTestCase
    {
        Canvas _outerCanvas,_innerCanvas,_canvasPanel;
        TextBox _testTB;
        CheckBox _checker;
        TextBlock _textLabel;
        int _counter = 0;

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Log("Verifying that Regression_Bug130 doesnt repro");

            _outerCanvas = new Canvas();
            //outerCanvas.Width = new Length(100, UnitType.Percent);
            //outerCanvas.Height = new Length(100, UnitType.Percent);

            _canvasPanel = new Canvas();
            _canvasPanel.Name = "orangeone";
            _canvasPanel.Background = System.Windows.Media.Brushes.Orange;
            _canvasPanel.MouseLeave += new System.Windows.Input.MouseEventHandler(canvasPanel_MouseLeave);
            _canvasPanel.Width = 600;
            _canvasPanel.Height = 200;
            _canvasPanel.SetValue(Canvas.TopProperty, 50d);
            _canvasPanel.SetValue(Canvas.LeftProperty, 50d);
            _outerCanvas.Children.Add(_canvasPanel);

            _innerCanvas = new Canvas();
            _innerCanvas.Name = "blueone";
            _innerCanvas.SetValue(Canvas.TopProperty, 40d);
            _innerCanvas.SetValue(Canvas.LeftProperty, 40d);
            _innerCanvas.Width = 100;
            _innerCanvas.Height = 100;
            _innerCanvas.Background = System.Windows.Media.Brushes.Blue;
            _canvasPanel.Children.Add(_innerCanvas);

            _testTB = new TextBox();
            _testTB.Name = "tb";
            _testTB.SetValue(Canvas.TopProperty, 250d);
            _testTB.SetValue(Canvas.LeftProperty, 40d);
            _testTB.Width = 400;
            _testTB.Height = 300;
            _testTB.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _outerCanvas.Children.Add(_testTB);

            _checker = new CheckBox();
            _checker.Name = "cb";
            _checker.SetValue(Canvas.TopProperty, 560d);
            _checker.SetValue(Canvas.LeftProperty, 40d);
            _outerCanvas.Children.Add(_checker);

            _textLabel = new TextBlock();
            _textLabel.SetValue(Canvas.TopProperty, 560d);
            _textLabel.SetValue(Canvas.LeftProperty, 60d);
            _textLabel.Text = "AutoScrollTextBox";
            _outerCanvas.Children.Add(_textLabel);

            MainWindow.Content = _outerCanvas;
            QueueDelegate(TestStep1);
        }

        private void canvasPanel_MouseLeave(object sender, System.Windows.Input.MouseEventArgs e)
        {
            _counter++;
            Log("MouseLeave event counter: [" + _counter + "]");
            if ((bool)_checker.IsChecked)
            {
                _testTB.ScrollToEnd();
            }
        }

        private void TestStep1()
        {
            MouseInput.MouseClick(_canvasPanel);
            MouseInput.MouseClick(_innerCanvas);
            QueueDelegate(TestStep2);
        }

        private void TestStep2()
        {
            Verifier.Verify(_counter == 1, "Verifying MouseLeave event counter value at step2", true);
            MouseInput.MouseClick(_canvasPanel);
            QueueDelegate(TestStep3);
        }

        private void TestStep3()
        {
            Verifier.Verify(_counter == 2, "Verifying MouseLeave event counter value at step3", true);
            _checker.IsChecked = true;
            MouseInput.MouseClick(_innerCanvas);
            QueueDelegate(TestStep4);
        }

        private void TestStep4()
        {
            Verifier.Verify(_counter == 3, "Verifying MouseLeave event counter value at step4", true);
            _canvasPanel.MouseLeave -= new System.Windows.Input.MouseEventHandler(canvasPanel_MouseLeave);
            Log("Regression_Bug130 didnt repro");
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that Regression_Bug131 and Regression_Bug132 (position of scrollbars in textbox) doesnt repro
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("461"), TestBugs("131, 132")]
    public class TextBoxScrollBarPosition : CustomTestCase
    {
        TextBox _testTB;
        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Log("Verifying that Regression_Bug131 and Regression_Bug132 dont repro");

            StackPanel fPanel = new StackPanel();
            _testTB = new TextBox();
            _testTB.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            _testTB.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            _testTB.Width = 400;
            _testTB.Height = 200;
            _testTB.Text = "This is a test for testing scrollbar positioning in textbox";
            fPanel.Children.Add(_testTB);
            MainWindow.Content = fPanel;
            QueueDelegate(VerifyScrollBarPosition);
        }

        /// <summary>
        /// Gets the visuals of the HorizontalScrollBar, VerticalScrollBar and TextBox from
        /// the VisualTree and compares their positioning.
        /// </summary>
        private void VerifyScrollBarPosition()
        {
            const string parentPath = ".//ScrollViewer/Grid/";
            Visual v;
            Rect hScrollBarRect, vScrollBarRect, textBoxRect;

            v = GetVisual(parentPath + "HorizontalScrollBar");
            UIElement hScrollBar = v as UIElement;
            hScrollBarRect = ElementUtils.GetScreenRelativeRect(hScrollBar);

            v = GetVisual(parentPath + "VerticalScrollBar");
            UIElement vScrollBar = v as UIElement;
            vScrollBarRect = ElementUtils.GetScreenRelativeRect(vScrollBar);

            v = GetVisual(".//TextBox");
            UIElement textBox = v as UIElement;
            textBoxRect = ElementUtils.GetScreenRelativeRect(textBox);

            Log("HorizontalScrollBar.Bottom [" + hScrollBarRect.Bottom + "]");
            Log("VerticalScrollBar.Right [" + vScrollBarRect.Right + "]");
            Log("HorizontalScrollBar.Top [" + hScrollBarRect.Top + "]");
            Log("VerticalScrollBar.Left [" + vScrollBarRect.Left + "]");
            Log("TextBox.Bottom [" + textBoxRect.Bottom + "]");
            Log("TextBox.Bottom [" + textBoxRect.Right + "]");

            //Currently there is a difference of 2 pixels between the boundary of
            //scrollbars and textbox. Verification is done for it. The purpose of
            //this test case is to catch regressions.
            Verifier.Verify(hScrollBarRect.Bottom + 2 == textBoxRect.Bottom,
                "Verifying the positioning of HorizontalScrollBar", true);

            Verifier.Verify(vScrollBarRect.Right + 2 == textBoxRect.Right,
                "Verifying the positioning of VerticalScrollBar", true);

            Verifier.Verify(hScrollBarRect.Top == vScrollBarRect.Bottom,
                            "Verifying the top/bottom positioning of VerticalScrollBar compared to HorizontalScrollBar", true);

            Verifier.Verify(hScrollBarRect.Right == vScrollBarRect.Left,
                            "Verifying the left/right positioning of VerticalScrollBar compared to HorizontalScrollBar", true);

            Log("Regression_Bug131 & Regression_Bug132 didnt repro");
            Logger.Current.ReportSuccess();
        }

        private Visual GetVisual(string xpath)
        {
            Visual[] v = XPathNavigatorUtils.ListVisuals(MainWindow, xpath);
            if (v.Length == 0)
            {
                Log("Unable to find xpath" + xpath);
                Log("Visual tree for main window follows.");
                Log(VisualLogger.DescribeVisualTree(MainWindow));
                throw new Exception("Unable to find element for path: " + xpath);
            }
            return v[0];
        }
    }

    /// <summary>Verifies that Regression_Bug133 doesnt repro</summary>
    [TestOwner("Microsoft"), TestTactics("462"), TestBugs("133")]
    public class Regression_Bug133 : CustomTestCase
    {        
        private ContentPresenter _textParent = null;
        private TextBox _textBox = null;
        private Button _clearTextButton,_addTextButton,_toggleVisibilityButton;       

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {                        
            MainWindow.Background = Brushes.LightBlue;

            DockPanel dockPanel = new DockPanel();
            //dockPanel.Width = new Length(100, UnitType.Percent);
            //dockPanel.Height = new Length(100, UnitType.Percent);
            MainWindow.Content = dockPanel;

            _clearTextButton = new Button();
            _clearTextButton.Background = Brushes.LightGray;
            _clearTextButton.Content = "Clear Text";            
            _clearTextButton.Click += new RoutedEventHandler(ClearTextButton_Click);
            dockPanel.Children.Add(_clearTextButton);

            _addTextButton = new Button();
            _addTextButton.Background = Brushes.LightGray;
            _addTextButton.Content = "Add Text";
            _addTextButton.Click += new RoutedEventHandler(AddTextButton_Click);
            dockPanel.Children.Add(_addTextButton);

            _toggleVisibilityButton = new Button();
            _toggleVisibilityButton.Background = Brushes.LightGray;
            _toggleVisibilityButton.Content = "Toggle Visibility";
            _toggleVisibilityButton.Click += new RoutedEventHandler(toggleVisibilityButton_Click);
            dockPanel.Children.Add(_toggleVisibilityButton);

            _textParent = new ContentPresenter();
            _textParent.Width = 150;
            _textParent.Height = 100;
            dockPanel.Children.Add(_textParent);

            _textBox = new TextBox();
            _textBox.Width = 100;
            _textBox.Height = 50;
            _textParent.Content = _textBox;

            QueueDelegate(AddText_Action1);
        }

        private void AddText_Action1()
        {
            MouseInput.MouseClick(_addTextButton);
            QueueDelegate(ToggleVisibility_Action1);
        }

        private void ToggleVisibility_Action1()
        {
            Verifier.Verify(_textBox.Text == "Hello", "Verifying TextBox contents after AddText_Action", true);           
            MouseInput.MouseClick(_toggleVisibilityButton);            
            QueueDelegate(ClearText_Action1);            
        }

        private void ClearText_Action1()
        {
            Verifier.Verify(_textBox.Text == "Hello", "Verifying TextBox contents after removing from tree", true);
            MouseInput.MouseClick(_clearTextButton);
            QueueDelegate(ToggleVisibility_Action2);
        }

        private void ToggleVisibility_Action2()
        {
            Verifier.Verify(_textBox.Text == "", "Verifying TextBox contents after ClearText_Action", true);
            MouseInput.MouseClick(_toggleVisibilityButton);
            QueueDelegate(AddText_Action2);
        }

        private void AddText_Action2()
        {
            Verifier.Verify(_textBox.Text == "", "Verifying TextBox contents after re-adding to tree", true);
            MouseInput.MouseClick(_addTextButton);
            QueueDelegate(VerifyAddText);
        }

        private void VerifyAddText()
        {
            Verifier.Verify(_textBox.Text == "Hello", "Verifying TextBox contents after AddText_Action2", true);
            MouseInput.MouseClick(_clearTextButton);
            QueueDelegate(VerifyClearText);
        }

        private void VerifyClearText()
        {
            Verifier.Verify(_textBox.Text == "", "Verifying TextBox contents after ClearText_Action2", true);
            Logger.Current.ReportSuccess();
        }

        private void ClearTextButton_Click(object sender, RoutedEventArgs e)
        {
            this._textBox.Text = "";
        }

        private void AddTextButton_Click(object sender, RoutedEventArgs e)
        {
            this._textBox.Text = "Hello";
        }

        private void toggleVisibilityButton_Click(object sender, RoutedEventArgs e)
        {
            if (_textParent.Content is DockPanel)
            {
                _textParent.Content = _textBox;
            }
            else
            {
                _textParent.Content = new DockPanel();
            }
        }
    }
}

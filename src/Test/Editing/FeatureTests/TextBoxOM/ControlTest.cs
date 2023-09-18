// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Test cases for TextBox members declared in its Control superclass.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/ControlTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;        
    using System.Collections;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Globalization;
    using System.Reflection;
    using System.Threading;
    using System.Xml;
    using System.Xml.XPath;
    using System.Xml.Xsl;

    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Text;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.IO;
    using Test.Uis.Management;        
    using Test.Uis.TestTypes;    
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    using Bitmap = System.Drawing.Bitmap;      

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the control behaves like a FrameworkElement
    /// correctly.
    /// </summary>
    [Test(0, "TextBox", "TextBoxFrameworkElement", MethodParameters = "/TestCaseType:TextBoxFrameworkElement")]
    [TestOwner("Microsoft"), TestTactics("646"), TestBugs("665,630,562")]
    public class TextBoxFrameworkElement: CustomTestCase
    {
        #region Private fields.

        private FrameworkElement _testElement;
        private UIElementWrapper _testWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            DoIteration();
        }

        private void DoIteration()
        {
            _textEditableType = TextEditableType.Values[_editableTypeIndex];

            Log("Creating editable type: " + _textEditableType.Type);
            _testElement = _textEditableType.CreateInstance();
            _testWrapper = new UIElementWrapper(_testElement);

            QueueDelegate(TestParent);
        }

        private void TestParent()
        {
            Verifier.Verify(_testElement.Parent == null,
                "Element has a null parent to begin with.", true);

            Log("Placing element in window...");
            MainWindow.Content = _testElement;
            QueueDelegate(TestParentInWindow);
        }

        private void TestParentInWindow()
        {
            Verifier.Verify(_testElement.Parent == MainWindow,
                "Element is parented to window.", true);

            Log("Checking that element is editable...");
            MouseInput.MouseClick(_testElement);
            KeyboardInput.TypeString("123");
            QueueDelegate(TestParentEditable);
        }

        private void TestParentEditable()
        {
            DockPanel newParent;

            Verifier.Verify((_testWrapper.Text == "123") || (_testWrapper.Text == "123\r\n"),
                "Text in element is as expected.", true);

            Log("Checking that element can be reparented...");
            newParent = new DockPanel();
            MainWindow.Content = null;
            newParent.Children.Add(_testElement);
            MainWindow.Content = newParent;
            QueueDelegate(TestParentReparented);
        }

        private void TestParentReparented()
        {
            Verifier.Verify(_testElement.Parent != null,
                "Parent is still assigned.", true);
            Verifier.Verify(_testElement.Parent != MainWindow,
                "Parent is no longer window.", true);

            Log("Checking that element is still editable...");
            _testWrapper.Text = "";
            MouseInput.MouseClick(_testElement);
            KeyboardInput.TypeString("321");
            QueueDelegate(TestParentReparentedEditable);
        }

        private void TestParentReparentedEditable()
        {
            Style newStyle;

            Verifier.Verify((_testWrapper.Text == "321") || (_testWrapper.Text == "321\r\n"),
                "Text in element is as expected.", true);

            Log("Testing that styles can be changed and content preserved...");
            if (_textEditableType.SupportsParagraphs)
            {
                _testWrapper.XamlText = "<Paragraph>Content <Button>button content</Button></Paragraph>";
            }
            else
            {
                _testWrapper.Text = "new content";
            }
            newStyle = new Style(_textEditableType.Type);
            newStyle.Setters.Add (new Setter(Control.BackgroundProperty, Brushes.Red));
            _testElement.Style = newStyle;
            QueueDelegate(MoveMouseOverControl);
        }

        private void MoveMouseOverControl()
        {
            Log("Moving mouse over the control for verifying Regression_Bug562");
            ActionItemWrapper.MouseElementRelative(_testElement, "move 20 20");
            QueueDelegate(TestStyleContents);
        }

        private void TestStyleContents()
        {
            string xamlContent;
            if (_textEditableType.SupportsParagraphs)
            {
                xamlContent = _testWrapper.XamlText;
                Verifier.Verify(xamlContent.IndexOf("Paragraph") != -1,
                    "Paragraph preserved after re-styling.", true);

                Button button = TextOMUtils.EmbeddedObjectsInRange(_testWrapper.TextRange)[0] as Button;

                Verifier.Verify(button != null && button.Content.ToString().Contains("button content"),
                    "Button content preserved after re-styling.", true);
            }
            else
            {
                Verifier.Verify(_testWrapper.Text == "new content",
                    "Content preserved after re-styling.", true);
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

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the control behaves like a Control correctly.
    /// </summary>
    [Test(0, "TextBox", "TextBoxControlTest", MethodParameters = "/TestCaseType:TextBoxControlTest")]
    [TestOwner("Microsoft"), TestTactics("645"), TestBugs("661")]
    public class TextBoxControlTest: CustomTestCase
    {
        #region Private fields.

        private Control _testElement;
        private UIElementWrapper _testWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex;
        private Canvas _container;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            DoIteration();
        }

        private void DoIteration()
        {
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

            Log("Placing element in window...");
            _container = new Canvas();
            _testElement.MinWidth = 80;
            _container.Children.Add(_testElement);
            MainWindow.Content = _container;

            QueueDelegate(TestContextMenu);
        }

        #region Context menu.

        private void TestContextMenu()
        {
            ContextMenu contextMenu;    // Context menu to assign.
            MenuItem menuItem;          // First menu item in menu.

            menuItem = new MenuItem();
            menuItem.Header = "First item.";

            contextMenu = new ContextMenu();
            contextMenu.Items.Add(menuItem);

            _testElement.ContextMenu = contextMenu;
            _testElement.ContextMenuOpening += ContextMenuOpeningHandler;

	    ActionItemWrapper.MouseElementRelative(_testElement,
                "click left 10 10");
            ActionItemWrapper.MouseElementRelative(_testElement,
                "click right 10 10");

            QueueDelegate(TestContextMenuShown);
        }

        private void ContextMenuOpeningHandler(object sender,
            ContextMenuEventArgs e)
        {
            MenuItem newItem;   // Item to add to context menu.

            Log("ContextMenuOpeningHandler invoked.");
            Log("Sender: " + sender);
            Verifier.Verify(sender == _testElement,
                "Sender is the element being tested.", true);

            ContextMenu cm = _testElement.ContextMenu;
            Log("Context menu item count: " + cm.Items.Count);
            Verifier.Verify(cm.Items.Count == 1,
                "There is a single item in the context menu.", true);

            Log("Adding a new item...");
            newItem = new MenuItem();
            newItem.Header = "Second item.";
            cm.Items.Add(newItem);
        }

        private void TestContextMenuShown()
        {
            ContextMenu contextMenu;    // Context menu modified before.

            contextMenu = _testElement.ContextMenu;

            Log("Item count in context menu: " + contextMenu.Items.Count);
            Verifier.Verify(contextMenu.Items.Count == 2,
                "There are now two items in the context menu.", true);

            ActionItemWrapper.MouseElementRelative(_testElement,
                "click left 8 10");
            QueueDelegate(TestContextMenuDismissed);
        }

        private void TestContextMenuDismissed()
        {
            TestSizeChanged();

        }

        #endregion Context menu.

        #region Size changed event.

        private System.Windows.Size _initialSize;
        private System.Windows.Size _newChangedSize;
        private bool _sizeChangedFired;

        private void SizeChanged(object sender, SizeChangedEventArgs e)
        {
            Log("SizeChanged event fired.");
            Log("Previous size: " + e.PreviousSize.ToString());
            Log("New size:      " + e.NewSize.ToString());

            Verifier.Verify(sender == _testElement,
                "Editable instance changed size.", true);
            Verifier.Verify(e.PreviousSize == _initialSize,
                "Previous size matches initial size.", true);
            //In RichTextBox size wont increase due to wrapping.
            if (!(_testElement is RichTextBox))
            {
                Verifier.Verify(e.PreviousSize.Width < e.NewSize.Width,
                    "Verifying that size.Width has increased", true);
            }

            _sizeChangedFired = true;
            _newChangedSize = e.NewSize;

            _testElement.SizeChanged -= SizeChanged;
        }

        private void TestSizeChanged()
        {
            _initialSize = new Size(
                _testElement.ActualWidth, _testElement.ActualHeight);
            Log("Initial size: " + _initialSize);

            // Type some and let the control grow.
            _sizeChangedFired = false;
            _testElement.SizeChanged += SizeChanged;
            KeyboardInput.TypeString(new string('-', 30));            
            QueueDelegate(CheckSizeChanged);
        }

        private void CheckSizeChanged()
        {
            //testElement.SizeChanged -= SizeChanged;
            Verifier.Verify(_sizeChangedFired, "SizeChanged event was fired.", true);
            FinishedIteration();
        }

        #endregion Size changed event.

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


        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the ToolTip can be set on the TextBox and
    /// it indicates it's visible. Also looks for repro #Regression_Bug662,
    /// verifying that a TextBox with a ToolTip assigned will
    /// reject tab characters if its AcceptsTab property
    /// is set to false.
    /// </summary>
    [Test(0, "TextBox", "TextBoxToolTipTab", MethodParameters = "/TestCaseType=TextBoxToolTipTab", Timeout = 300)]
    [TestOwner("Microsoft"), TestTactics("644"), TestBugs("662")]
    public class TextBoxToolTipTab: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Creating ToolTip..");
            _tooltip = new ToolTip();
            _tooltip.Content = "ToolTip Content";

            Log("Adding ToolTip to control...");
            Control control = (Control) TestControl;
            control.ToolTip = _tooltip;

            Log("Moving mouse over control...");
            MouseInput.MouseMove(control);

            QueueDelegate(AfterMove);
        }

        private void AfterMove()
        {
            Log("Waiting for the tooltip to appear...");
            Verifier.Verify(_tooltip.IsOpen == false,
                "ToolTip is not yet visible", true);

            TimeSpan delay = TimeSpan.FromMilliseconds(2000);

            QueueHelper.Current.QueueDelayedDelegate(
                delay, new SimpleHandler(AfterToolTip), new object[] {});
        }

        private void AfterToolTip()
        {
            Log("Verifying that ToolTip is visible...");
            Log("ToolTip visibility: " + _tooltip.IsOpen);
            Verifier.Verify(_tooltip.IsOpen, "ToolTip is visible", true);

            Log("Pressing TAB key...");
            TestTextBox.AcceptsTab = false;
            KeyboardInput.TypeString("{TAB}");

            QueueDelegate(AfterTab);
        }

        private void AfterTab()
        {
            Log("Verifying that TAB key was ignored by text editor...");
            Log("Text: [" + TestTextBox.Text + "]");
            Verifier.Verify(TestTextBox.Text.Length == 0, "Text is empty", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private ToolTip _tooltip;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that GotKeyboardFocus and LostKeyboardFocus events are fired
    /// when a TextBox is clicked, and that the IsKeyboardFocused
    /// and IsKeyboardFocusWithin properties show the expected values.
    /// </summary>
    [Test(3, "TextBox", "TextBoxUIElementGotLostKeyboardFocus", MethodParameters = "/TestCaseType=TextBoxUIElementGotLostKeyboardFocus")]
    [TestOwner("Microsoft"), TestTactics("643"), TestBugs("786,787")]
    public class TextBoxUIElementGotLostKeyboardFocus: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            SetTextBoxProperties(TestTextBox);

            //TestTextBox.Height = new Length(100, UnitType.Percent);
            //TestTextBoxAlt.Height = new Length(100, UnitType.Percent);
            TestTextBox.Focus();
            QueueDelegate(DoInput);
        }

        private void DoInput()
        {
            Log("Clicking on an alternate text box...");

            TestTextBox.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(BoxLostKeyboardFocus);
            TestTextBox.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(BoxGotKeyboardFocus);

            TestTextBoxAlt.LostKeyboardFocus += new KeyboardFocusChangedEventHandler(AltBoxLostKeyboardFocus);
            TestTextBoxAlt.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(AltBoxGotKeyboardFocus);

            MouseInput.MouseClick(TestTextBoxAlt);
            QueueHelper.Current.QueueDelegate(AfterClickAlt);
        }

        private void AfterClickAlt()
        {
            Log("Verifying that the right events fired...");
            CheckFired("Main lost focus", _mainLostKeyboardFocus, true);
            CheckFired("Main got focus", _mainGotKeyboardFocus, false);
            CheckFired("Alt lost focus", _altLostKeyboardFocus, false);
            CheckFired("Alt got focus", _altGotKeyboardFocus, true);

            VerifyFocused(TestTextBoxAlt);

            // Reset all state.
            _mainLostKeyboardFocus = _mainGotKeyboardFocus = _altLostKeyboardFocus = _altGotKeyboardFocus = false;

            Log("Clicking on the main text box...");
            MouseInput.MouseClick(TestTextBox);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(AfterClickMain));
        }

        private void AfterClickMain()
        {
            Log("Verifying that the right events fired...");
            CheckFired("Main lost focus", _mainLostKeyboardFocus, false);
            CheckFired("Main got focus", _mainGotKeyboardFocus, true);
            CheckFired("Alt lost focus", _altLostKeyboardFocus, true);
            CheckFired("Alt got focus", _altGotKeyboardFocus, false);

            VerifyFocused(TestTextBox);

            Log("Verifying that if Focusable=False, then focus is not changed...");
            TestTextBoxAlt.Focusable = false;
            MouseInput.MouseClick(TestTextBoxAlt);
            QueueHelper.Current.QueueDelegate(AfterClickUnfocusableAlt);
        }

        private void AfterClickUnfocusableAlt()
        {
            Log("Verifying that focus is still on old TextBox...");
            Verifier.Verify(TestTextBox.IsKeyboardFocused == true,
                "First text box still has focus.", true);
            Verifier.Verify(TestTextBoxAlt.IsKeyboardFocused == false,
                "Second text box does not have focus.", true);
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Handlers.

        private void BoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            _mainLostKeyboardFocus = true;
        }

        private void BoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            _mainGotKeyboardFocus = true;
        }

        private void AltBoxLostKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            _altLostKeyboardFocus = true;
        }

        private void AltBoxGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            _altGotKeyboardFocus = true;
        }

        #endregion Handlers.

        #region Verifications.

        private void CheckFired(string description, bool value, bool expected)
        {
            Log(description + ": " + value + " (expected " + expected + ")");
            Verifier.Verify(value == expected);
        }

        private void VerifyFocused(Control control)
        {
            System.Diagnostics.Debug.Assert(control != null);

            Verifier.Verify(control.IsKeyboardFocusWithin,
                "IsKeyboardFocusWithin is true as expected: " + control.IsKeyboardFocusWithin, true);
            Verifier.Verify(control.IsKeyboardFocused,
                "IsKeyboardFocused is true as expected: " + control.IsKeyboardFocused, true);
            Verifier.Verify(control == Keyboard.FocusedElement,
                "Control is Keyboard.FocusedElement.", true);
        }

        #endregion Verifications.

        #region Private fields.

        private bool _mainLostKeyboardFocus;
        private bool _mainGotKeyboardFocus;
        private bool _altLostKeyboardFocus;
        private bool _altGotKeyboardFocus;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that a TextBox can be focused by clicking a button.
    /// This is a very common user scenario.
    /// </summary>
    [Test(0, "TextBox", "TextBoxUIElementFocusFromButton", MethodParameters = "/TestCaseType=TextBoxUIElementFocusFromButton")]
    [TestOwner("Microsoft"), TestTactics("642"),
     TestBugs("906,907,908,909,910,911")]
    public class TextBoxUIElementFocusFromButton: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            CreateWindowContent();
            MainWindow.UpdateLayout();

            Log("Wrapping editable element...");
            _testWrapper = new UIElementWrapper(_textbox);

            Log("Clicking on button...");
            MouseInput.MouseClick(_button);
        }

        private void CreateWindowContent()
        {
           StackPanel topPanel; // Top-level element.

            Log("Creating window content (TextBox and Button)...");

            _button = new Button();
            _button.Content = "Click me";
            _button.Click += ButtonClick;

            _textbox = new TextBox();

            topPanel = new StackPanel();
            topPanel.Children.Add(_button);
            topPanel.Children.Add(_textbox);

            MainWindow.Content = topPanel;
        }

        private void ButtonClick(object sender, RoutedEventArgs args)
        {
            Log("Button clicked.");
            _textBeforeType = _testWrapper.Text;

            Log("Setting focus on test control...");
            _textbox.Focus();

            Log("Typing on test control...");
            KeyboardInput.TypeString("   ");

            QueueDelegate(AfterType);
        }

        private void AfterType()
        {
            Log("Typing has finished.");

            Log("Text before typing: [" + _textBeforeType + "]");
            Log("Text after typing: [" + _testWrapper.Text + "]");
            Verifier.Verify(_testWrapper.Text != _textBeforeType,
                "Text has changed", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        private string _textBeforeType;
        private TextBox _textbox;
        private Button _button;
        private UIElementWrapper _testWrapper;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the cursor changes when it is on the whitespace
    /// area of a TextBox, and that the control lays out correctly
    /// in a DockPanel.
    ///
    /// Also verifies that the scrollbars change the caret to an arrow.
    /// </summary>
    [Test(0, "TextBox", "TextBoxCursor", MethodParameters = "/TestCaseType=TextBoxCursor")]
    [TestOwner("Microsoft"), TestTactics("641"), TestBugs("793,794,795,796,797,563,798")]
    public class TextBoxCursor: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Setting the main XAML to a button and a TextBox...");
            ActionItemWrapper.SetMainXaml(
                "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                "<Button DockPanel.Dock='Left' Name='Button'>Button</Button>" +
                "<TextBox Name='TB' /></DockPanel>");
            QueueDelegate(MoveOverButton);
        }

        private void MoveOverButton()
        {
            Log("Finding references to controls..");
            _button = (Button) ElementUtils.FindElement(TestWindow, "Button");
            if (_button == null)
            {
                throw new Exception("Unable to find button.");
            }
            _textControl = (Control) ElementUtils.FindElement(TestWindow, "TB");
            if (_textControl == null)
            {
                throw new Exception("Unable to find text editing control.");
            }
            _textWrapper = new UIElementWrapper(_textControl);

            Log("Moving mouse over the button...");
            ActionItemWrapper.MouseElementRelative(_button, "click left 10 10");
            QueueDelegate(MoveOverTextBox);
        }

        private void MoveOverTextBox()
        {
            Log("Getting cursor for button...");
            _buttonCursor = Win32.SafeGetCursor();

            Log("Moving mouse over the TextBox...");
            ((TextBox)_textControl).Text = "Test for cursor type while selecting text using mouse";
            ActionItemWrapper.MouseElementRelative(_textControl, "move 20 20");

            QueueDelegate(ClickOverTextBox);
        }

        private void ClickOverTextBox()
        {
            Log("Getting cursor when mouse is over an out-focus textbox");
            _outFocusTextBoxCursor = Win32.SafeGetCursor();

            Log("Clicking mouse over the TextBox...");
            ActionItemWrapper.MouseElementRelative(_textControl, "click left 10 10");
            QueueDelegate(CheckCursors);
        }

        private void CheckCursors()
        {
            string longString;

            Log("Getting cursor for textbox...");
            _textCursor = Win32.SafeGetCursor();

            Verifier.Verify(_textCursor != _buttonCursor, "Cursor has changed.", true);
            Verifier.Verify(_textCursor == _outFocusTextBoxCursor, "Cursor should not be different between in-focus and out-focus TB.", true);

            Log("Setting text on TextBox to a very long string and selecting all...");
            longString = new string('-', 1024 * 4);
            _textWrapper.Text = longString;
            _textWrapper.Select(0, longString.Length - 1);

            Log("Moving mouse to ensure that cursor is on selection...");
            MouseInput.MouseMove(20, 20);
            //Moving the mouse so that it is on top of the selection made.
            ActionItemWrapper.MouseElementRelative(_textControl, "move 20 8");

            QueueDelegate(CheckSelectionCursor);
        }

        private void CheckSelectionCursor()
        {
            Win32.HCURSOR selectionCursor;

            Log("Getting cursor for selection...");
            selectionCursor = Win32.SafeGetCursor();

            Verifier.Verify(selectionCursor == _buttonCursor,
                "Cursor on selection matches cursor on button.", true);

            MouseInput.MouseMove(_textControl);
            QueueDelegate(CheckSelectionBelowCursor);
        }

        private void CheckSelectionBelowCursor()
        {
            //Repro for Regression_Bug563
            Win32.HCURSOR selectionBelowCursor;

            Log("Getting cursor when below a selection...");
            selectionBelowCursor = Win32.SafeGetCursor();

            Verifier.Verify(selectionBelowCursor != _buttonCursor,
                "Cursor below the selection should not be of type Arrow.", true);

            Log("Setting the main XAML to a TextBox in different panels...");
            ActionItemWrapper.SetMainXaml(
                "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' LastChildFill='False'>" +
                "  <TextBox Name='TB1' VerticalAlignment='Top' />" +
                "  <TextBox  Name='TB2' />" +
                "</DockPanel>");
            QueueDelegate(CheckLayouts);
        }

        private void CheckLayouts()
        {
            const string ScrollBarXPathQuery = "//ScrollBar[2]";
            TextBox canvasBox;
            TextBox dockBox;
            Visual[] queryResult;
            UIElement scrollbar;

            canvasBox = (TextBox) ElementUtils.FindElement(MainWindow, "TB1");
            dockBox = (TextBox) ElementUtils.FindElement(MainWindow, "TB2");

            Log("Canvas TextBox computed height: " + canvasBox.ActualHeight);
            Log("Dock TextBox computed height: " + dockBox.ActualHeight);
            Verifier.Verify(dockBox.ActualHeight > canvasBox.ActualHeight,
                "Dock with fill is larger than Canvas.", true);

            Log("Displaying scroll bar and clicking on it...");
            canvasBox.Height = 200;
            canvasBox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
            canvasBox.UpdateLayout();
            queryResult = XPathNavigatorUtils.ListVisuals(canvasBox, ScrollBarXPathQuery);
            if (queryResult.Length == 0)
            {
                throw new Exception("Cannot find scroll bar in " + queryResult);
            }
            scrollbar = (UIElement) queryResult[0];
            MouseInput.MouseClick(scrollbar);
            QueueDelegate(CheckScrollBarCursor);
        }

        private void CheckScrollBarCursor()
        {
            Log("Verifying that cursor is regular arrow...");
            Win32.HCURSOR scrollbarCursor;
            scrollbarCursor = Win32.SafeGetCursor();

            Verifier.Verify(scrollbarCursor == _buttonCursor,
                "Scrollbar cursor matches button.", true);

            QueueDelegate(SetContextMenu);
        }

        private void SetContextMenu()
        {
            Log("Verifying that cursor over a ContextMenu is of type Arrow");
            Log("Setting the main XAML to a button and a TextBox...");
            ActionItemWrapper.SetMainXaml(
                "<DockPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                "<Button DockPanel.Dock='Left' Name='Button'>Button</Button>" +
                "<TextBox  Name='TB' /></DockPanel>");

            QueueDelegate(RightClickOnTextBox);
        }

        private void RightClickOnTextBox()
        {
            _textControl = (Control)ElementUtils.FindElement(TestWindow, "TB");
            if (_textControl == null)
            {
                throw new Exception("Unable to find text editing control.");
            }

            ((TextBox)_textControl).Text = "This is a test for cursor type over ContextMenu";

            _textControl.ContextMenu = new ContextMenu();
            _textControl.ContextMenu.Name = "tbContextMenu";
            _textControl.ContextMenu.Items.Add("test one");
            _textControl.ContextMenu.Items.Add("test two");
            _textControl.ContextMenu.Items.Add("test three");

            MouseInput.MouseClick(_textControl);
            ActionItemWrapper.MouseElementRelative(_textControl, "move 10 10");
            QueueDelegate(PressShiftF10);
        }

        private void PressShiftF10()
        {
            KeyboardInput.TypeString("+{F10}");

            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 0, 2),
                new SimpleHandler(MoveOverContextMenu), new Object[]{});
        }

        private void MoveOverContextMenu()
        {
            ActionItemWrapper.MouseElementRelative(_textControl.ContextMenu, "move 10 10");

            Win32.HCURSOR contextMenuCursor;
            contextMenuCursor = Win32.SafeGetCursor();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private fields.

        /// <summary>Cursor displayed when mouse is over button.</summary>
        private Win32.HCURSOR _buttonCursor;
        /// <summary>Cursor displayed when mouse is over text.</summary>
        private Win32.HCURSOR _textCursor;
        /// <summary>Cursor displayed when mouse is over out-focus textbox.</summary>
        private Win32.HCURSOR _outFocusTextBoxCursor;
        /// <summary>Tested button.</summary>
        private Button _button;
        /// <summary>Tested TextBox (possibly other control).</summary>
        private Control _textControl;
        /// <summary>Text wrapper for TextBox.</summary>
        private UIElementWrapper _textWrapper;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that major visual aspects of text editable
    /// controls are represented properly with UI Automation.
    /// </summary>
    [Test(2, "TextBox", "TextBoxVisualAutomation", MethodParameters = "/TestCaseType=TextBoxVisualAutomation")]
    [TestOwner("Microsoft"), TestTactics("639"), TestBugs("788,789,790,791,792,24"), TestWorkItem("122")]
    public class TextBoxVisualAutomation: CustomTestCase
    {
        #region Private fields.

        /// <summary>Engine generating dimensions.</summary>
        private CombinatorialEngine _engine;

        /// <summary>Editable type being tested.</summary>
        private TextEditableType _textEditableType;

        /// <summary>Element being tested.</summary>
        private FrameworkElement _testElement;

        /// <summary>Wrapper for element being tested.</summary>
        private UIElementWrapper _testWrapper;

        /// <summary>Whether to use default values in element.</summary>
        private bool _useDefaults;

        /// <summary>A control different from the tested element.</summary>
        private Control _otherControl;

        /// <summary>Original system metrics.</summary>
        private SystemMetrics _savedMetrics;

        /// <summary>Non-standard system metrics.</summary>
        private SystemMetrics _customMetrics = new SystemMetrics(0x0000FF00, 0x00FFFF00);

        /// <summary>Thread used to get the AutomationElement.FocusedElement</summary>
        private Thread _aeThread; 

        private AutomationElement _focusedElement;
        private AutomationPropertyData[] _propertyDataArray;
        private int _testIndex;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Dimension[] dimensions; // Dimensions for engine.

            // Set up engine with dimensions.
            dimensions = new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.Values),                
                new Dimension("UseDefaults", new object[] { true, false }),
            };

            _engine = CombinatorialEngine.FromDimensions(dimensions);
            TestCombination();
        }

        private void TestCombination()
        {
            Hashtable combination;  // Values in a given combination.

            combination = new Hashtable();
            if (_engine.Next(combination))
            {
                Canvas topPanel;

                _textEditableType = (TextEditableType) combination["TextEditableType"];
                _useDefaults = (bool) combination["UseDefaults"];

                Log("******* Text editable type: " + _textEditableType.Type + " *******");

                _testElement = _textEditableType.CreateInstance();
                _testWrapper = new UIElementWrapper(_testElement);

                if (!_useDefaults)
                {
                    SetCustomValues();
                }

                topPanel = new Canvas();
                Canvas.SetTop(_testElement, 10);
                Canvas.SetLeft(_testElement, 10);
                _testElement.Width = 100;
                _testElement.Height = 100;

                _otherControl = new Button();
                Canvas.SetTop(_testElement, 10);
                Canvas.SetLeft(_testElement, 140);

                topPanel.Children.Add(_testElement);
                topPanel.Children.Add(_otherControl);                

                MainWindow.Content = topPanel;
                QueueDelegate(DoFocus);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        private void DoFocus()
        {
            _testElement.Focus();
            QueueDelegate(GetFocusedElement);
        }

        private void GetFocusedElement()
        {
            _aeThread = new Thread(new ThreadStart(GetAutomationElementFocusedElement));
            _aeThread.SetApartmentState(System.Threading.ApartmentState.STA);
            _aeThread.Start();
        }

        private void GetAutomationElementFocusedElement()
        {
            try
            {
                _focusedElement = AutomationElement.FocusedElement;                
            }
            catch (Exception e)
            {
                Log("Unable to get the AutomationElement.FocusedElement. Exception thrown: " + e.ToString());
                throw;
            }            

            QueueHelper helper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);            
            helper.QueueDelegate(new SimpleHandler(TestAutomationProperties));
        }        

        private void TestAutomationProperties()
        {
            _testIndex = 0;
            _propertyDataArray = AutomationPropertyData.Values;

            if (_focusedElement == null)
            {
                throw new Exception("Unable to obtain focused element.");
            }

            VerifyAutomationProperties();
        }

        private void VerifyAutomationProperties()
        {
            // There appears to be no way to get the native element
            // from the automation element. If we got the wrong object,
            // the control type property check will fail below.

            if (_testIndex < _propertyDataArray.Length)
            {
                _propertyDataArray[_testIndex].VerifyCurrentValue(_focusedElement, _testWrapper,
                    new SimpleHandler(VerifyAutomationProperties), true);
                _testIndex++;
            }
            else
            {
                _otherControl.Focus();

                //If test control is not a subclass type test visuals with system metrics
                if (!_textEditableType.IsSubClass)
                {
                    QueueDelegate(TestVisuals);
                }
                else
                {
                    QueueDelegate(TestCombination);
                }
            }
        }

        private void TestVisuals()
        {
            CheckAccessibleVisuals();

            Log("Changing system metrics and re-verifying...");
            SaveSystemMetrics();
            ChangeSystemMetrics(_customMetrics);
            QueueDelegate(TestChangedVisuals);
        }

        private void TestChangedVisuals()
        {
            Log("Testing changed visuals...");
            try
            {
                CheckAccessibleVisuals();
            }
            finally
            {
                ChangeSystemMetrics(_savedMetrics);
            }

            QueueDelegate(TestCombination);
        }

        private void CheckAccessibleVisuals()
        {
            foreach(AccessibleVisualData data in AccessibleVisualData.Values)
            {
                string description;

                Log("Checking value for: " + data.Name);

                //Regression_Bug24: Skip the check for foreground property in Classic theme
                if ( (string.Compare(Win32.SafeGetCurrentThemeName(), "", true, CultureInfo.InvariantCulture) == 0)&&
                     (data.DependencyProperty == Control.ForegroundProperty))
                {
                    Log("Skipping testing foreground property in classic theme");
                    continue;
                }

                description = data.MatchesSystemValue(_testWrapper);
                if (description.Length > 0)
                {
                    throw new Exception("Visual does not match system value: " + description);                    
                }
            }
        }

        private void ChangeSystemMetrics(SystemMetrics values)
        {
            // Change the current theme. This is a workaround
            // that lets the testing infrastructure know that
            // the visual settings need to be restored if the
            // test case crashes.
            if (values == null)
            {
                throw new ArgumentNullException("values");
            }

            // Change some specific metrics.        
            //Win32.SafeSetSysColors(2,
            //    new int[] { Win32.COLOR_WINDOW, Win32.COLOR_BTNTEXT },
            //    new uint[] { values.WindowColor, values.WindowTextColor });

            // Change some specific metrics for non-Classic theme
            Win32.SafeSetSysColors(2,
                new int[] { Win32.COLOR_WINDOW, Win32.COLOR_BTNTEXT },
                new uint[] { values.WindowColor, values.WindowTextColor });                        
        }

        private void SaveSystemMetrics()
        {
            _savedMetrics = new SystemMetrics(
                Win32.SafeGetSysColor(Win32.COLOR_WINDOW),
                Win32.SafeGetSysColor(Win32.COLOR_WINDOWTEXT));
        }

        private void SetCustomValues()
        {
            TextBox textbox;

            textbox = _testElement as TextBox;
            if (textbox != null)
            {
                textbox.IsReadOnly = true;
                textbox.Text = StringData.MixedScripts.Value;
                textbox.HorizontalScrollBarVisibility = ScrollBarVisibility.Visible;
                textbox.VerticalScrollBarVisibility = ScrollBarVisibility.Visible;
            }
        }

        #endregion Main flow.

        #region Inner types.

        /// <summary>Interesting system metrics.</summary>
        class SystemMetrics
        {
            internal uint WindowColor;
            internal uint WindowTextColor;

            internal SystemMetrics(uint windowColor, uint windowTextColor)
            {
                this.WindowColor = windowColor;
                this.WindowTextColor = windowTextColor;
            }
        }

        #endregion Inner types.
    }

    /// <summary>
    /// Verifies that editing controls can be loaded from localized
    /// resources correctly.
    /// </summary>
    /// <remarks>
    /// All editable controls are tested in one 'go', and all scenarios
    /// are tested in separate methods - no combinatorial engine required.
    /// </remarks>
    [Test(3, "TextBox", "TextBoxLocalizationTest", MethodParameters = "/TestCaseType=TextBoxLocalizationTest", Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("640"), TestBugs("659"), TestWorkItem("121")]
    public class TextBoxLocalizationTest : CustomTestCase
    {
        #region Private fields.

        private const bool UseUserOverrideFalse = false;

        private const string EnglishUSContent = "English - US content";
        private const string FrenchContent = "French - neutral content";
        private const string FrenchFranceContent = "French - France content";
        private const string GenericContent = "Generic Content";
        private const string SpanishContent = "Spanish - neutral content";
        private const string SpanishArgentinaContent = "Spanish - Argentina content";
        private const string TurkishContent = "Turkish - neutral content";
        private const string TurkishTurkeyContent = "Turkish - Turkey content";

        private CultureInfo _englishUSCulture = new CultureInfo("en-us", UseUserOverrideFalse);
        private CultureInfo _frenchCulture = new CultureInfo("fr", UseUserOverrideFalse);
        private CultureInfo _frenchFranceCulture = new CultureInfo("fr-FR", UseUserOverrideFalse);
        private CultureInfo _spanishCulture = new CultureInfo("es", UseUserOverrideFalse);
        private CultureInfo _spanishArgentinaCulture = new CultureInfo("es-AR", UseUserOverrideFalse);
        private CultureInfo _turkishCulture = new CultureInfo("tr", UseUserOverrideFalse);
        private CultureInfo _turkishTurkeyCulture = new CultureInfo("tr-TR", UseUserOverrideFalse);

        private List<string> _filesToDelete = new List<string>();

        private const string OutputFileName = "WTC.Uis.TextBoxLocalizationTest.txt";

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            try
            {
                TestSystemMatchLoading();
                TestSystemMismatchLoading();
                TestFallbackLoading();
            }
            finally
            {
                DeleteBuildFiles();
            }
            Logger.Current.ReportSuccess();
        }

        /// <summary>
        /// Verifies that given multiple localized resources, the one
        /// matching the system UI is used.
        /// </summary>
        private void TestSystemMatchLoading()
        {
            CultureInfo alternativeCulture;
            CultureInfo currentCulture;
            string alternativeContent;
            string currentContent;

            currentCulture = CultureInfo.CurrentUICulture;
            alternativeCulture = GetDifferentCulture(currentCulture);
            currentContent = GetContentForCulture(currentCulture);
            alternativeContent = GetContentForCulture(alternativeCulture);

            Log("Testing that UI culture " + currentCulture.Name + " is loaded when " +
                alternativeCulture.Name + " is also available.");

            BuildProject(currentCulture, currentContent);

            VerifyLoadedContent(currentCulture);

            DeleteBuildFiles();
        }

        /// <summary>
        /// Verifies that given multiple localized resources, when
        /// none matches the system.
        /// </summary>
        private void TestSystemMismatchLoading()
        {
            CultureInfo alternativeCulture;
            string alternativeContent;

            alternativeCulture = GetDifferentCulture(CultureInfo.CurrentUICulture);
            alternativeContent = GetContentForCulture(alternativeCulture);

            Log("Testing that culture " + alternativeCulture.Name + " works even " +
                "when it's the only culture.");

            BuildProject(alternativeCulture, alternativeContent);

            DeleteBuildFiles();
        }

        /// <summary>
        /// Verifies that given multiple localized resources, when
        /// there is no exact match, loading falls back to more
        /// acceptable resources.
        /// </summary>
        private void TestFallbackLoading()
        {
            CultureInfo fallbackCulture;
            string fallbackContent;

            fallbackCulture = CultureInfo.CurrentUICulture.Parent;
            if (fallbackCulture == null)
            {
                Log("The current UI culture [" + CultureInfo.CurrentUICulture.Name +
                    "] has no parent - cannot test fallback.");
                return;
            }
            if (fallbackCulture == CultureInfo.InvariantCulture)
            {
                Log("The fallback culture is the invariant culture - not interesting.");
                return;
            }

            Log("Testing fallback from " + CultureInfo.CurrentUICulture.Name +
                " to " + fallbackCulture.Name);
            fallbackContent = GetContentForCulture(fallbackCulture);

            BuildProject(fallbackCulture, fallbackContent);

            VerifyLoadedContent(fallbackCulture);

            DeleteBuildFiles();
        }

        /// <summary>Deletes files used by building processes.</summary>
        private void DeleteBuildFiles()
        {
            foreach (string fileToDelete in _filesToDelete)
            {
                Log("Deleting " + fileToDelete + "...");
                if (System.IO.Directory.Exists(fileToDelete))
                {
                    System.IO.Directory.Delete(fileToDelete, true);
                }
                else if (System.IO.File.Exists(fileToDelete))
                {
                    System.IO.File.Delete(fileToDelete);
                }
            }
            _filesToDelete.Clear();
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Builds a project with resources for the specified culture.
        /// </summary>
        /// <param name="culture">Culture to build resources for.</param>
        /// <param name="content">Content in resulting application.</param>
        /// <remarks>
        /// The project built has two controls, a TextBox and a RichTextBox.
        /// When loaded, the built program will write these values out
        /// to a file and exit.
        /// </remarks>
        private void BuildProject(CultureInfo culture, string content)
        {
            throw new System.NotImplementedException("Build-dependent tests are currently disabled in Arrowhead!");
            //string applicationXml;      // Application contents.
            //string pageXml;             // Page contents.
            //string projectXml;          // Project contents.
            //string projectXmlFileName;  // File name of project.
            //// Build executor helper.
            //Microsoft.Test.MSBuildEngine.MSBuildProjExecutor executor;
            //// Code that runs when the Initialized event is fired in the app.
            //const string InitializedCode =
            //    "private void PageInitialized(object sender, EventArgs e) {\r\n" +
            //    " const string fileName = \"" + OutputFileName + "\";\r\n" +
            //    " using (System.IO.StreamWriter writer = new System.IO.StreamWriter(fileName, false, System.Text.Encoding.Unicode)) {\r\n" +
            //    "  writer.WriteLine(textBox.Text);\r\n" +
            //    "  TextRange tr = new TextRange(richTextBox.Document.ContentStart, richTextBox.Document.ContentEnd);\r\n" +
            //    "  System.IO.MemoryStream mStream = new System.IO.MemoryStream();\r\n" +
            //    "  tr.Save(mStream, DataFormats.Xaml);\r\n" +
            //    "  mStream.Seek(0, System.IO.SeekOrigin.Begin);\r\n" +
            //    "  System.IO.StreamReader sReader = new System.IO.StreamReader(mStream);\r\n" +
            //    "  writer.WriteLine(sReader.ReadToEnd());\r\n" +
            //    "  System.Windows.Application.Current.Shutdown();\r\n" +
            //    " }\r\n" +
            //    "}";

            //if (culture == null)
            //{
            //    throw new ArgumentNullException("culture");
            //}
            //if (content == null)
            //{
            //    throw new ArgumentNullException("content");
            //}

            //projectXmlFileName = "MyProj.csproj";
            //applicationXml = "<Application " +
            //    "xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            //    "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' " +
            //    //"x:Guid='38e614cf-8828-44c3-b4ca-98f1c3f800b1' " +
            //    "x:Uid='Application_1' " +
            //    //"x:MaxUid='1' " +
            //    "StartupUri='MyPage.xaml' />";
            //pageXml = "<Page xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' \r\n" +
            //    "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' \r\n" +
            //    "x:Class='MyPage_Code' \r\n" +
            //    "x:Uid='Page_1' \r\n" +
            //    "Initialized='PageInitialized'> \r\n" +
            //    "<x:Code><![CDATA[ " + InitializedCode + " ]]></x:Code>\r\n" +
            //    "<Canvas Background='#440077' " +
            //    //"x:Guid='6c74995c-9dbe-460b-8b59-90ad44226f2e' " +
            //    "x:Uid='Canvas_1' " +
            //    //"x:MaxUid='524'" +
            //    "> \r\n" +
            //    "   <TextBox Name='textBox' x:Uid='textBox'>" + content + "</TextBox> \r\n" +
            //    "   <RichTextBox Name='richTextBox' x:Uid='richTextBox'> \r\n" +
            //    "       <FlowDocument x:Uid='FlowDocument_1'> \r\n" +
            //    "           <Paragraph x:Uid='Paragraph_1'>" + content + "<Bold x:Uid='Bold_1'>" + content + "</Bold>" + content + "</Paragraph> \r\n" +
            //    "       </FlowDocument> \r\n" +
            //    "   </RichTextBox> \r\n" +
            //    "</Canvas> \r\n" +
            //    "</Page>";
            //projectXml = "<Project DefaultTargets='Build' xmlns='http://schemas.microsoft.com/developer/msbuild/2003'> \r\n" +
            //    "<PropertyGroup> \r\n" +
            //    "   <RootNamespace>Microsoft.Test.MyProj</RootNamespace> \r\n" +
            //    "   <AssemblyName>MyProj</AssemblyName> \r\n" +
            //    "   <TargetType>winexe</TargetType> \r\n" +
            //    "   <Optimize>true</Optimize> \r\n" +
            //    "   <Configuration>Release</Configuration> \r\n" +
            //    "   <OutputPath>bin\\$(Configuration)\\</OutputPath> \r\n" +
            //    "   <UICulture>" + culture.Name + "</UICulture> \r\n" +
            //    "   <Debug>true</Debug> \r\n" +
            //    //"<SignManifests>true</SignManifests> \r\n" +
            //    //"<ManifestKeyFile>ClickOnceTest.pfx</ManifestKeyFile> \r\n" +
            //    //"<ManifestCertificateThumbprint>cd582af19e477ae94a53102e0453e71b3c592a80</ManifestCertificateThumbprint> \r\n" +
            //    "</PropertyGroup> \r\n" +
            //    @"<Import Project='$(MSBuildBinPath)\Microsoft.CSharp.targets' />" + " \r\n " +
            //    @"<Import Project='$(MSBuildBinPath)\Microsoft.WinFX.targets' />" + " \r\n " +
            //    "<ItemGroup> \r\n" +
            //    "   <ApplicationDefinition Include='app.xaml'><Localizable>False</Localizable></ApplicationDefinition> \r\n" +
            //    "   <Page Include='MyPage.xaml'><Localizable>True</Localizable></Page> \r\n" +
            //    "   <None Include=\"ClickOnceTest.pfx\" /> \r\n" +
            //    "</ItemGroup> \r\n" +
            //    "<ItemGroup> \r\n" +
            //    "   <Reference Include='System' /> \r\n" +
            //    "   <Reference Include='System.Xml' /> \r\n" +
            //    "   <Reference Include='System.Data' /> \r\n" +
            //    "   <Reference Include='WindowsBase' /> \r\n" +
            //    "   <Reference Include='PresentationCore' /> \r\n" +
            //    "   <Reference Include='PresentationFramework' /> \r\n" +
            //    "   <Reference Include='UIAutomationClient' /> \r\n" +
            //    "   <Reference Include='UIAutomationTypes' /> \r\n" +
            //    "</ItemGroup> \r\n" +
            //    "</Project> \r\n";

            //TextFileUtils.SaveToFile(projectXml, projectXmlFileName);
            //TextFileUtils.SaveToFile(applicationXml, "app.xaml");
            //TextFileUtils.SaveToFile(pageXml, "MyPage.xaml");

            //_filesToDelete.Add(projectXmlFileName);
            //_filesToDelete.Add("app.xaml");
            //_filesToDelete.Add("MyPage.xaml");
            //_filesToDelete.Add("bin");
            //_filesToDelete.Add("obj");
            //_filesToDelete.Add(OutputFileName);

            //Log("Building project " + projectXmlFileName + " for culture " + culture.Name);
            //executor = new Microsoft.Test.MSBuildEngine.MSBuildProjExecutor();
            //try
            //{
            //    if (!executor.Build(projectXmlFileName))
            //    {
            //        throw new Exception("Unable to build files.");
            //    }
            //}
            //catch (Exception)
            //{
            //    Log("Unable to build project file " + projectXmlFileName +
            //        ":\r\n" + projectXml);
            //    throw;
            //}
        }

        /// <summary>
        /// Counts the number of occurences of searchFor in the searchIn string.
        /// </summary>
        /// <param name="searchFor">Value to search for.</param>
        /// <param name="searchIn">String to search in.</param>
        /// <returns>The number of occurences of searchFor in searchIn.</returns>
        private static int CountOccurences(string searchFor, string searchIn)
        {
            int index;
            int result;

            result = 0;
            index = 0;
            while ((index < searchIn.Length &&
                (index = searchIn.IndexOf(searchFor, index)) != -1))
            {
                result++;
                index += searchFor.Length;
            }
            return result;
        }

        /// <summary>Gets a content string for the specified culture.</summary>
        /// <param name="culture">Culture info to get string for.</param>
        /// <returns>A content string for the specified culture.</returns>
        private string GetContentForCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            if (culture.Name == _englishUSCulture.Name)
            {
                return EnglishUSContent;
            }
            else if (culture.Name == _frenchCulture.Name)
            {
                return FrenchContent;
            }
            else if (culture.Name == _frenchFranceCulture.Name)
            {
                return FrenchFranceContent;
            }
            else if (culture.Name == _spanishCulture.Name)
            {
                return SpanishContent;
            }
            else if (culture.Name == _spanishArgentinaCulture.Name)
            {
                return SpanishArgentinaContent;
            }
            else if (culture.Name == _turkishCulture.Name)
            {
                return TurkishContent;
            }
            else if (culture.Name == _turkishTurkeyCulture.Name)
            {
                return TurkishTurkeyContent;
            }
            else
            {
                return GenericContent;
            }
        }

        /// <summary>Returns a CultureInfo different from the specified culture.</summary>
        /// <param name="culture">Culture different from the returned one.</param>
        /// <returns>A CultureInfo different from the specified culture.</returns>
        private CultureInfo GetDifferentCulture(CultureInfo culture)
        {
            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            if (culture.Name == _turkishTurkeyCulture.Name)
            {
                return _frenchFranceCulture;
            }
            else
            {
                return _turkishTurkeyCulture;
            }
        }

        /// <summary>
        /// Verifies that running the built application loads the content
        /// for the specified culture.
        /// </summary>
        /// <param name="culture">Culture expected to match.</param>
        private void VerifyLoadedContent(CultureInfo culture)
        {
            string standardOutput;
            string standardError;
            string outputContents;
            string expectedContent;
            int delimiterIndex;

            if (culture == null)
            {
                throw new ArgumentNullException("culture");
            }

            Log(@"Launching bin\release\MyProj.exe");
            ProcessUtils.RunProcess(@"bin\Release\MyProj.exe",
                null, 1000 * 30, out standardOutput, out standardError);

            if (standardOutput != "" || standardError != "")
            {
                Log("Child process output: " + standardOutput);
                Log("Child process error: " + standardError);
            }
            else
            {
                Log("No standard output or error contents written by application.");
            }

            expectedContent = GetContentForCulture(culture);
            outputContents = TextFileUtils.LoadFromFile(OutputFileName);

            Log("Expected content fragment: " + expectedContent);
            Log("Output contents:\r\n" + outputContents);

            delimiterIndex = outputContents.IndexOf("\r\n");
            if (delimiterIndex == -1)
            {
                throw new Exception("Unable to find newline in output content." +
                    "Plain text and rich text output should be delimited " +
                    "by a \\r\\n pair.");
            }
            if (CountOccurences(expectedContent, outputContents.Substring(0, delimiterIndex)) != 1)
            {
                throw new Exception("Unable to find expected content in plain" +
                    "text output: " + expectedContent);
            }
            if (CountOccurences(expectedContent, outputContents.Substring(delimiterIndex)) != 3)
            {
                throw new Exception("Unable to find three expected occurences of " +
                    "content in rich text output: " + expectedContent);
            }
            Log("Correct content found in plain and rich text.");
        }

        #endregion Helper methods.
    }

    /// <summary>
    /// Verifies that all properties are stable before the control is
    /// added to a default tree, after addition and after rendering.
    /// This test is performed for TextBox and RichTextBox.
    /// ExtentHeight and ExtentWidth after rendering will be tested separately.
    /// </summary>
    [Test(0, "TextBoxBase", "EditControlPropsTest", MethodParameters = "/TestCaseType:EditControlPropsTest")]
    [TestOwner("Microsoft"), TestTactics("638"), TestBugs("566, 785, 564"), TestWorkItem("77")]
    public class EditControlPropsTest : CustomTestCase
    {
        #region Private fields.

        private Control _testElement;
        private UIElementWrapper _testElementWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex;
        private StackPanel _container;        

        Hashtable _expTextBoxBasePropValues = new Hashtable();
        Hashtable _expTextBoxPropValues = new Hashtable();
        Hashtable _expRichTextBoxPropValues = new Hashtable();

        #endregion Private fields.

        #region Setup

        private void CreateTestElement()
        {
            FrameworkElement editableInstance;
            _textEditableType = TextEditableType.Values[_editableTypeIndex];
            editableInstance = _textEditableType.CreateInstance();
            _testElement = editableInstance as Control;
            if (_testElement == null || _testElement is PasswordBox)
            {
                Log("Element is not of type System.Windows.Control.");
                _testElement = null;
                FinishedIteration();
                return;
            }
            _testElement.Height = 200;
            _testElement.Width = 100;
            _testElement.FontSize = 24;
            _testElement.FontFamily = new FontFamily("Tahoma");

            _testElementWrapper = new UIElementWrapper(_testElement);
        }

        private void AddToTree()
        {
            //Adding the testElement to the tree
            //testElement.Parent = null;
            _container.Children.Clear();
            _container.Children.Add(_testElement);

            //_container.Parent = null;
            MainWindow.Content = null;
            MainWindow.Content = _container;            
        }

        /// <summary>
        /// Assigns the expected default values to a HashTable for all properties declared in TextBoxBase
        /// Later the HashTable will be used for verification.
        /// </summary>
        private void TextBoxBasePropDefault()
        {
            //expTextBoxBasePropValues["Wrap"] = false;
            _expTextBoxBasePropValues["IsReadOnly"] = false;
            _expTextBoxBasePropValues["AcceptsTab"] = false;
            _expTextBoxBasePropValues["SpellCheck.IsEnabled"] = false;
            //expTextBoxBasePropValues["HorizontalScrollBarVisibility"] = ScrollBarVisibility.Hidden;
            _expTextBoxBasePropValues["VerticalScrollBarVisibility"] = ScrollBarVisibility.Hidden;
            _expTextBoxBasePropValues["ExtentWidth"] = 0.0;
            _expTextBoxBasePropValues["ExtentHeight"] = 0.0;
            _expTextBoxBasePropValues["ViewportWidth"] = 0.0;
            _expTextBoxBasePropValues["ViewportHeight"] = 0.0;
            _expTextBoxBasePropValues["HorizontalOffset"] = 0.0;
            _expTextBoxBasePropValues["VerticalOffset"] = 0.0;
        }

        /// <summary>
        /// This function should be called before verifying properties of TextBoxBase after the control is rendered.
        /// Expected values for properties whose values change after rendering are set in this function.
        /// </summary>
        private void TextBoxBasePropAfterRender()
        {
            //For the following properties, it is enough if we check that the value is not 0.0 after
            //the control is rendered. For this purpose we flag it to -1.0
            _expTextBoxBasePropValues["ExtentWidth"] = -1.0;
            _expTextBoxBasePropValues["ExtentHeight"] = -1.0;
            _expTextBoxBasePropValues["ViewportWidth"] = -1.0;
            _expTextBoxBasePropValues["ViewportHeight"] = -1.0;

            //In classic theme the values of the ViewportWidth = 100.0 - 12.0(borders) - 16.0(scrollbar)
            //ViewportHeight = 200 - 6.0(borders) - 16.0(scrollbar)
        }

        /// <summary>
        /// Assigns the expected default values to a HashTable for all properties declared in TextBox
        /// Later the HashTable will be used for verification.
        /// </summary>
        private void TextBoxPropDefault()
        {
            _expTextBoxPropValues["AcceptsReturn"] = false;
            _expTextBoxPropValues["MinLines"] = 1;
            _expTextBoxPropValues["MaxLines"] = Int32.MaxValue;
            _expTextBoxPropValues["HorizontalScrollBarVisibility"] = ScrollBarVisibility.Hidden;
            _expTextBoxPropValues["Text"] = "Test";
            _expTextBoxPropValues["CharacterCasing"] = System.Windows.Controls.CharacterCasing.Normal;
            _expTextBoxPropValues["MaxLength"] = 0;
            _expTextBoxPropValues["SelectedText"] = "";
            _expTextBoxPropValues["SelectionLength"] = 0;
            _expTextBoxPropValues["SelectionStart"] = 0;
        }

        /// <summary>
        /// Assigns the expected default values to a HashTable for all properties declared in RichTextBox.
        /// Later the HashTable will be used for verification.
        /// </summary>
        private void RichTextBoxPropDefault()
        {
            _expRichTextBoxPropValues["AcceptsReturn"] = true;
            _expRichTextBoxPropValues["HorizontalScrollBarVisibility"] = ScrollBarVisibility.Hidden;
        }

        /// <summary>
        /// Assigns Custom Values (set)-I for properties declared in TextBoxBase on the test control.
        /// Also populates the HashTable with the assigned values for verification which will done later.
        /// </summary>
        private void AssignTextBoxBasePropCustom1()
        {
            //setting expected values
            TextBoxBasePropDefault();            
            _expTextBoxBasePropValues["IsReadOnly"] = false;
            _expTextBoxBasePropValues["AcceptsTab"] = true;
            _expTextBoxBasePropValues["SpellCheck.IsEnabled"] = true;            
            _expTextBoxBasePropValues["VerticalScrollBarVisibility"] = ScrollBarVisibility.Visible;
            _expTextBoxBasePropValues["HorizontalOffset"] = 5.0;
            _expTextBoxBasePropValues["VerticalOffset"] = 5.0;

            //setting property values on the control
            _testElementWrapper.Text = "This \r\n is a \r\n test \r\n for \r\n EditControls \r\n (textbox \r\n & richtextbox) \r\n properties";
            ReflectionUtils.SetProperty(_testElement, "IsReadOnly", false);
            ReflectionUtils.SetProperty(_testElement, "AcceptsTab", true);
            ReflectionUtils.SetProperty(((TextBoxBase)(_testElement)).SpellCheck, "IsEnabled", true);
            ReflectionUtils.SetProperty(_testElement, "VerticalScrollBarVisibility", ScrollBarVisibility.Visible);            
            ((TextBoxBase)_testElement).ScrollToHorizontalOffset(5d);            
            ((TextBoxBase)_testElement).ScrollToVerticalOffset(5d);            
        }

        /// <summary>
        /// Assigns Custom Values (set)-II for properties declared in TextBoxBase on the test control.
        /// Also populates the HashTable with the assigned values for verification which will done later.
        /// </summary>
        private void AssignTextBoxBasePropCustom2()
        {
            //setting expected values
            TextBoxBasePropDefault();            
            _expTextBoxBasePropValues["IsReadOnly"] = true;
            _expTextBoxBasePropValues["AcceptsTab"] = false;
            _expTextBoxBasePropValues["SpellCheck.IsEnabled"] = false;            
            _expTextBoxBasePropValues["VerticalScrollBarVisibility"] = ScrollBarVisibility.Visible;
            _expTextBoxBasePropValues["HorizontalOffset"] = 0.0;
            _expTextBoxBasePropValues["VerticalOffset"] = 0.0;

            //setting property values on the control            
            ReflectionUtils.SetProperty(_testElement, "IsReadOnly", true);
            ReflectionUtils.SetProperty(_testElement, "AcceptsTab", false);
            ReflectionUtils.SetProperty(((TextBoxBase)_testElement).SpellCheck, "IsEnabled", false);            
            ReflectionUtils.SetProperty(_testElement, "VerticalScrollBarVisibility", ScrollBarVisibility.Visible);            
            ((TextBoxBase)_testElement).ScrollToHorizontalOffset(0d);            
            ((TextBoxBase)_testElement).ScrollToVerticalOffset(0d);

            _testElementWrapper.Text = "Text";
        }

        /// <summary>
        /// Assigns Custom Values (set)-I for properties declared in TextBox on the test control.
        /// Also populates the HashTable with the assigned values for verification which will done later.
        /// </summary>
        private void AssignTextBoxPropCustom1()
        {
            //setting expected values
            TextBoxPropDefault();
            _expTextBoxPropValues["AcceptsReturn"] = true;
            _expTextBoxPropValues["MinLines"] = 2;
            _expTextBoxPropValues["MaxLines"] = 5;
            _expTextBoxPropValues["Text"] = "This is a test";
            _expTextBoxPropValues["CharacterCasing"] = System.Windows.Controls.CharacterCasing.Lower;
            _expTextBoxPropValues["TextWrapping"] = TextWrapping.Wrap;
            _expTextBoxPropValues["HorizontalScrollBarVisibility"] = ScrollBarVisibility.Disabled;

            //setting property values on the control
            //custom values
            ReflectionUtils.SetProperty(_testElement, "AcceptsReturn", true);
            ReflectionUtils.SetProperty(_testElement, "MinLines", 2);
            ReflectionUtils.SetProperty(_testElement, "MaxLines", 5);
            ReflectionUtils.SetProperty(_testElement, "Text", "This is a test");
            ReflectionUtils.SetProperty(_testElement, "CharacterCasing", System.Windows.Controls.CharacterCasing.Lower);
            ReflectionUtils.SetProperty(_testElement, "TextWrapping", TextWrapping.Wrap);
            //ReflectionUtils.SetProperty(testElement, "HorizontalScrollBarVisibility", ScrollBarVisibility.Disabled);

            //default values
            ReflectionUtils.SetProperty(_testElement, "MaxLength", 0);
            ReflectionUtils.SetProperty(_testElement, "SelectedText", "");
            ReflectionUtils.SetProperty(_testElement, "SelectionLength", 0);
            ReflectionUtils.SetProperty(_testElement, "SelectionStart", 0);
        }

        /// <summary>
        /// Assigns Custom Values (set)-II for properties declared in TextBox on the test control.
        /// Also populates the HashTable with the assigned values for verification which will done later.
        /// </summary>
        private void AssignTextBoxPropCustom2()
        {
            //setting expected values
            TextBoxPropDefault();
            _expTextBoxPropValues["Text"] = "12345";
            _expTextBoxPropValues["MaxLength"] = 7;
            //expTextBoxPropValues["SelectedText"] = "345";
            _expTextBoxPropValues["SelectionLength"] = 3;
            _expTextBoxPropValues["SelectionStart"] = 2;

            //setting property values on the control
            //custom values
            ReflectionUtils.SetProperty(_testElement, "MaxLength", 7);
            ReflectionUtils.SetProperty(_testElement, "Text", "12345");
            ReflectionUtils.SetProperty(_testElement, "SelectionLength", 3);
            ReflectionUtils.SetProperty(_testElement, "SelectionStart", 2);
            //ReflectionUtils.SetProperty(testElement, "SelectedText", "");

            //default values
            ReflectionUtils.SetProperty(_testElement, "AcceptsReturn", false);
            ReflectionUtils.SetProperty(_testElement, "MinLines", 1);
            ReflectionUtils.SetProperty(_testElement, "MaxLines", Int32.MaxValue);
            ReflectionUtils.SetProperty(_testElement, "CharacterCasing", System.Windows.Controls.CharacterCasing.Normal);

            _expTextBoxPropValues["SelectedText"] = "345";
        }

        /// <summary>
        /// Assigns Custom Values for properties declared in RichTextBox on the test control.
        /// Also populates the HashTable with the assigned values for verification which will done later.
        /// </summary>
        private void AssignRichTextBoxPropCustom()
        {
            //setting expected values
            RichTextBoxPropDefault();
            _expRichTextBoxPropValues["AcceptsReturn"] = false;            
            _expRichTextBoxPropValues["HorizontalScrollBarVisibility"] = ScrollBarVisibility.Visible;

            //setting property values on the control
            ReflectionUtils.SetProperty(_testElement, "AcceptsReturn", false);            
            ReflectionUtils.SetProperty(_testElement, "HorizontalScrollBarVisibility", ScrollBarVisibility.Visible);
        }

        #endregion Setup

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _container = new StackPanel();
            _container.Orientation = Orientation.Horizontal;

            DoIteration();
        }

        private void DoIteration()
        {            
            CreateTestElement();
            if (_testElement == null)
            {
                return;
            }            
            TextBoxBasePropDefault();
            TextBoxPropDefault();
            RichTextBoxPropDefault();

            Log("Creating editable type: " + _textEditableType.Type);
            _testElementWrapper.Text = "Test";

            //Verify default properties
            Log("Verifying default properties before adding to tree");
            VerifyAllProperties(false);

            AddToTree();
            Log("Verifying default properties after adding to tree");
            VerifyAllProperties(false);

            QueueDelegate(VerifyDefaultProps);
        }

        private void VerifyDefaultProps()
        {
            TextBoxBasePropAfterRender();            
            Log("Verifying default properties after rendering");
            VerifyAllProperties(true);

            //Verify with custom values-I for properties declared on TextBoxBase
            CreateTestElement();            
            AssignTextBoxBasePropCustom1();
            AddToTree();
            Log("Verifying with custom values-I for TextBoxBase after adding to tree");
            VerifyTextBoxBaseProperties(false);

            QueueDelegate(WorkAroundForRegression_Bug564);
            QueueDelegate(VerifyCustomProps1TextBoxBase);            
        }


        //This is a work around for Regression_Bug564: Calling ScrollToHorizontalOffset(x) works only if
        //TextBox/RichTextBox is already rendered.
        private void WorkAroundForRegression_Bug564()
        {
            if (_testElement is TextBoxBase)
            {
                ((TextBoxBase)_testElement).ScrollToVerticalOffset((double)_expTextBoxBasePropValues["VerticalOffset"]);
                ((TextBoxBase)_testElement).ScrollToHorizontalOffset((double)_expTextBoxBasePropValues["HorizontalOffset"]);
            }            
        }

        private void VerifyCustomProps1TextBoxBase()
        {
            TextBoxBasePropAfterRender();            
            Log("Verifying with custom values-I for TextBoxBase after rendering");
            VerifyTextBoxBaseProperties(true);

            //Verfiy with custom values-II for properties declared on TextBoxBase
            CreateTestElement();            
            AssignTextBoxBasePropCustom2();
            AddToTree();
            Log("Verifying with custom values-II for TextBoxBase after adding to tree");
            VerifyTextBoxBaseProperties(false);

            QueueDelegate(WorkAroundForRegression_Bug564);
            QueueDelegate(VerifyCustomProps2TextBoxBase);
        }

        private void VerifyCustomProps2TextBoxBase()
        {
            TextBoxBasePropAfterRender();            
            Log("Verifying with custom values-II for TextBoxBase after rendering");
            VerifyTextBoxBaseProperties(true);

            //Verify with custom values for properties declared on TextBox
            if (_textEditableType.Type == typeof(TextBox))
            {
                //custom values-I
                CreateTestElement();
                AssignTextBoxPropCustom1();
                AddToTree();
                Log("Verifying with custom values-I for TextBox after adding to tree");
                VerifyTextBoxProperties();

                QueueDelegate(VerifyCustomProps1TextBox);
            }
            //Verify with custom values for properties declared on RichTextBox
            else if (_textEditableType.Type == typeof(RichTextBox))
            {
                //custom values
                CreateTestElement();
                AssignRichTextBoxPropCustom();
                AddToTree();
                Log("Verifying with custom values for RichTextBox after adding to tree");
                VerifyRichTextBoxProperties();

                QueueDelegate(VerifyCustomPropsRichTextBox);
            }
            else
            {
                Log("Control should be Textbox or RichTextBox for this test");
                FinishedIteration();
            }
        }

        private void VerifyCustomProps1TextBox()
        {
            Log("Verifying with custom values-I for TextBox after rendering");
            VerifyTextBoxProperties();

            //custom values-II
            CreateTestElement();
            AssignTextBoxPropCustom2();
            AddToTree();
            Log("Verifying with custom values-II for TextBox after adding to tree");
            VerifyTextBoxProperties();

            QueueDelegate(VerifyCustomProps2TextBox);
        }

        private void VerifyCustomProps2TextBox()
        {
            Log("Verifying with custom values-II for TextBox after rendering");
            VerifyTextBoxProperties();

            FinishedIteration();
        }

        private void VerifyCustomPropsRichTextBox()
        {
            Log("Verifying with custom values for RichTextBox after rendering");
            VerifyRichTextBoxProperties();

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

        #endregion Main flow

        #region Verifiers
        /// <summary>
        /// Verifies all properties of the edit control
        /// </summary>
        private void VerifyAllProperties(bool isControlRendered)
        {
            Log("**************** Verifying all properties ************************");
            VerifyTextBoxBaseProperties(isControlRendered);

            if (_textEditableType.Type == typeof(TextBox))
            {
                VerifyTextBoxProperties();
            }

            if (_textEditableType.Type == typeof(RichTextBox))
            {
                VerifyRichTextBoxProperties();
            }
            Log("******************************************************************");
        }

        /// <summary>
        /// Verifies the properties of the control which are declared in TextBoxBase
        /// </summary>
        private void VerifyTextBoxBaseProperties(bool isControlRendered)
        {
            object actualValue, expectedValue;
            string logString;

            //Commented due to Regression_Bug566
            #region Compare Wrap property
            /*
            actualValue = ReflectionUtils.GetProperty(testElement, "Wrap");
            expectedValue = expTextBoxBasePropValues["Wrap"];
            logString = "Verifying " + "Wrap" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() +
                "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify((bool)actualValue == (bool)expectedValue, logString, true);
            */
            #endregion

            #region Compare IsReadOnly property
            actualValue = ReflectionUtils.GetProperty(_testElement, "IsReadOnly");
            expectedValue = _expTextBoxBasePropValues["IsReadOnly"];
            logString = "Verifying " + "IsReadOnly" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() +
                "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify((bool)actualValue == (bool)expectedValue, logString, true);
            #endregion

            #region Compare AcceptsTab property
            actualValue = ReflectionUtils.GetProperty(_testElement, "AcceptsTab");
            expectedValue = _expTextBoxBasePropValues["AcceptsTab"];
            logString = "Verifying " + "AcceptsTab" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() +
                "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify((bool)actualValue == (bool)expectedValue, logString, true);
            #endregion

            #region Compare SpellCheck.IsEnabled property
            actualValue = ReflectionUtils.GetProperty(_testElement, "SpellCheck");
            actualValue = ((System.Windows.Controls.SpellCheck)actualValue).IsEnabled;
            expectedValue = _expTextBoxBasePropValues["SpellCheck.IsEnabled"];
            logString = "Verifying " + "SpellCheck.IsEnabled" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() +
                "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify((bool)actualValue == (bool)expectedValue, logString, true);
            #endregion                        

            #region Compare VerticalScrollBarVisibility property
            actualValue = ReflectionUtils.GetProperty(_testElement, "VerticalScrollBarVisibility");
            expectedValue = _expTextBoxBasePropValues["VerticalScrollBarVisibility"];
            logString = "Verifying " + "VerticalScrollBarVisibility" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((ScrollBarVisibility)actualValue).ToString() +
                "] ExpectedValue [" + ((ScrollBarVisibility)expectedValue).ToString() + "]";
            Verifier.Verify((ScrollBarVisibility)actualValue == (ScrollBarVisibility)expectedValue, logString, true);
            #endregion

            #region Compare ExtentWidth property
            expectedValue = _expTextBoxBasePropValues["ExtentWidth"];
            actualValue = ReflectionUtils.GetProperty(_testElement, "ExtentWidth");
            //ExtentHeight after rendering will be tested separately (diff test case).
            //expectedValue is set to -1.0 when testing the value after rendering, so that we can skip it.
            if (((double)expectedValue) != -1.0)
            {
                logString = "Verifying " + "ExtentWidth" + " (TextBoxBase)Property: ";
                logString += "ActualValue [" + ((double)actualValue) +
                    "] ExpectedValue [" + ((double)expectedValue) + "]";
                Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            }
            else
            {
                logString = "Verifying that " + "ExtentWidth" + " (TextBoxBase)Property" +
                    " is != 0.0 ";
                logString += "ActualValue [" + ((double)actualValue) + "]";
                Verifier.Verify((double)actualValue != 0.0, logString, true);
            }
            #endregion

            #region Compare ExtentHeight property
            expectedValue = _expTextBoxBasePropValues["ExtentHeight"];
            //ExtentWidth after rendering will be tested separately (diff test case).
            //expectedValue is set to -1.0 when testing the value after rendering, so that we can skip it.
            if (((double)expectedValue) != -1.0)
            {
                actualValue = ReflectionUtils.GetProperty(_testElement, "ExtentHeight");
                logString = "Verifying " + "ExtentHeight" + " (TextBoxBase)Property: ";
                logString += "ActualValue [" + ((double)actualValue) +
                    "] ExpectedValue [" + ((double)expectedValue) + "]";
                Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            }
            else
            {
                logString = "Verifying that " + "ExtentHeight" + " (TextBoxBase)Property" +
                    " is != 0.0 ";
                logString += "ActualValue [" + ((double)actualValue) + "]";
                Verifier.Verify((double)actualValue != 0.0, logString, true);
            }
            #endregion

            #region Compare ViewportWidth property
            actualValue = ReflectionUtils.GetProperty(_testElement, "ViewportWidth");
            expectedValue = _expTextBoxBasePropValues["ViewportWidth"];
            //If the control is not yet rendered
            if (((double)expectedValue) != -1.0)
            {
                logString = "Verifying " + "ViewportWidth" + " (TextBoxBase)Property: ";
                logString += "ActualValue [" + ((double)actualValue) +
                    "] ExpectedValue [" + ((double)expectedValue) + "]";
                Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            }
            else //If the control is rendered, it is enough to check that the property value is != 0.0
            {
                logString = "Verifying that " + "ViewportWidth" + " (TextBoxBase)Property" +
                    " is != 0.0 ";
                logString += "ActualValue [" + ((double)actualValue) + "]";
                Verifier.Verify((double)actualValue != 0.0, logString, true);
            }
            #endregion

            #region Compare ViewportHeight property
            actualValue = ReflectionUtils.GetProperty(_testElement, "ViewportHeight");
            expectedValue = _expTextBoxBasePropValues["ViewportHeight"];
            //If the control is not yet rendered
            if (((double)expectedValue) != -1.0)
            {
                logString = "Verifying " + "ViewportHeight" + " (TextBoxBase)Property: ";
                logString += "ActualValue [" + ((double)actualValue) +
                    "] ExpectedValue [" + ((double)expectedValue) + "]";
                Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            }
            else //If the control is rendered, it is enough to check that the property value is != 0.0
            {
                logString = "Verifying that " + "ViewportHeight" + " (TextBoxBase)Property" +
                    " is != 0.0 ";
                logString += "ActualValue [" + ((double)actualValue) + "]";
                Verifier.Verify((double)actualValue != 0.0, logString, true);
            }
            #endregion

            //Commented due to Regression_Bug566
            #region Compare HorizontalOffset property
            /*
            actualValue = ReflectionUtils.GetProperty(testElement, "HorizontalOffset");
            expectedValue = expTextBoxBasePropValues["HorizontalOffset"];
            logString = "Verifying " + "HorizontalOffset" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((double)actualValue) +
                "] ExpectedValue [" + ((double)expectedValue) + "]";
            Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            */
            #endregion

            #region Compare "VerticalOffset" property
            actualValue = ReflectionUtils.GetProperty(_testElement, "VerticalOffset");            
            if (isControlRendered)
            {
                expectedValue = _expTextBoxBasePropValues["VerticalOffset"];
            }
            else
            {
                expectedValue = 0.0;
            }
            logString = "Verifying " + "VerticalOffset" + " (TextBoxBase)Property: ";
            logString += "ActualValue [" + ((double)actualValue) +
                "] ExpectedValue [" + ((double)expectedValue) + "]";
            Verifier.Verify((double)actualValue == (double)expectedValue, logString, true);
            #endregion
        }

        /// <summary>
        /// Verifies the properties of the control which are declared in TextBox
        /// </summary>
        private void VerifyTextBoxProperties()
        {
            object actualValue, expectedValue;
            string logString;

            #region Compare AcceptsReturn property
            actualValue = ReflectionUtils.GetProperty(_testElement, "AcceptsReturn");
            expectedValue = _expTextBoxPropValues["AcceptsReturn"];
            logString = "Verifying " + "AcceptsReturn" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() + "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify(((bool)actualValue) == ((bool)expectedValue), logString, true);
            #endregion

            #region Compare HorizontalScrollBarVisibility property
            actualValue = ReflectionUtils.GetProperty(_testElement, "HorizontalScrollBarVisibility");
            expectedValue = _expTextBoxPropValues["HorizontalScrollBarVisibility"];
            logString = "Verifying " + "HorizontalScrollBarVisibility" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((ScrollBarVisibility)actualValue).ToString() +
                "] ExpectedValue [" + ((ScrollBarVisibility)expectedValue).ToString() + "]";
            Verifier.Verify((ScrollBarVisibility)actualValue == (ScrollBarVisibility)expectedValue, logString, true);
            #endregion

            #region Compare MinLines property
            actualValue = ReflectionUtils.GetProperty(_testElement, "MinLines");
            expectedValue = _expTextBoxPropValues["MinLines"];
            logString = "Verifying " + "MinLines" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((int)actualValue) + "] ExpectedValue [" + ((int)expectedValue) + "]";
            Verifier.Verify(((int)actualValue) == ((int)expectedValue), logString, true);
            #endregion

            #region Compare MaxLines property
            actualValue = ReflectionUtils.GetProperty(_testElement, "MaxLines");
            expectedValue = _expTextBoxPropValues["MaxLines"];
            logString = "Verifying " + "MaxLines" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((int)actualValue) + "] ExpectedValue [" + ((int)expectedValue) + "]";
            Verifier.Verify(((int)actualValue) == ((int)expectedValue), logString, true);
            #endregion

            #region Compare Text property
            actualValue = ReflectionUtils.GetProperty(_testElement, "Text");
            expectedValue = _expTextBoxPropValues["Text"];
            logString = "Verifying " + "Text" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((string)actualValue) + "] ExpectedValue [" + ((string)expectedValue) + "]";
            Verifier.Verify(((string)actualValue) == ((string)expectedValue), logString, true);
            #endregion

            #region Compare CharacterCasing property
            actualValue = ReflectionUtils.GetProperty(_testElement, "CharacterCasing");
            expectedValue = _expTextBoxPropValues["CharacterCasing"];
            logString = "Verifying " + "CharacterCasing" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((System.Windows.Controls.CharacterCasing)actualValue).ToString() +
                "] ExpectedValue [" + ((System.Windows.Controls.CharacterCasing)expectedValue).ToString() + "]";
            Verifier.Verify(((System.Windows.Controls.CharacterCasing)actualValue) ==
                ((System.Windows.Controls.CharacterCasing)expectedValue), logString, true);
            #endregion

            #region Compare MaxLength property
            actualValue = ReflectionUtils.GetProperty(_testElement, "MaxLength");
            expectedValue = _expTextBoxPropValues["MaxLength"];
            logString = "Verifying " + "MaxLength" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((int)actualValue) + "] ExpectedValue [" + ((int)expectedValue) + "]";
            Verifier.Verify(((int)actualValue) == ((int)expectedValue), logString, true);
            #endregion

            #region Compare SelectedText property
            actualValue = ReflectionUtils.GetProperty(_testElement, "SelectedText");
            expectedValue = _expTextBoxPropValues["SelectedText"];
            logString = "Verifying " + "SelectedText" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((string)actualValue) + "] ExpectedValue [" + ((string)expectedValue) + "]";
            Verifier.Verify(((string)actualValue) == ((string)expectedValue), logString, true);
            #endregion

            #region Compare SelectionLength property
            actualValue = ReflectionUtils.GetProperty(_testElement, "SelectionLength");
            expectedValue = _expTextBoxPropValues["SelectionLength"];
            logString = "Verifying " + "SelectionLength" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((int)actualValue) + "] ExpectedValue [" + ((int)expectedValue) + "]";
            Verifier.Verify(((int)actualValue) == ((int)expectedValue), logString, true);
            #endregion

            #region Compare SelectionStart property
            actualValue = ReflectionUtils.GetProperty(_testElement, "SelectionStart");
            expectedValue = _expTextBoxPropValues["SelectionStart"];
            logString = "Verifying " + "SelectionStart" + " (TextBox)Property: ";
            logString += "ActualValue [" + ((int)actualValue) + "] ExpectedValue [" + ((int)expectedValue) + "]";
            Verifier.Verify(((int)actualValue) == ((int)expectedValue), logString, true);
            #endregion
        }

        /// <summary>
        /// Verifies the properties of the control which are declared in RichTextBox
        /// </summary>
        private void VerifyRichTextBoxProperties()
        {
            object actualValue, expectedValue;
            string logString;

            #region Compare AcceptsReturn property
            actualValue = ReflectionUtils.GetProperty(_testElement, "AcceptsReturn");
            expectedValue = _expRichTextBoxPropValues["AcceptsReturn"];
            logString = "Verifying " + "AcceptsReturn" + " (RichTextBox)Property: ";
            logString += "ActualValue [" + ((bool)actualValue).ToString() + "] ExpectedValue [" + ((bool)expectedValue).ToString() + "]";
            Verifier.Verify(((bool)actualValue) == ((bool)expectedValue), logString, true);
            #endregion

            #region Compare HorizontalScrollBarVisibility property
            actualValue = ReflectionUtils.GetProperty(_testElement, "HorizontalScrollBarVisibility");
            expectedValue = _expRichTextBoxPropValues["HorizontalScrollBarVisibility"];
            logString = "Verifying " + "HorizontalScrollBarVisibility" + " (RichTextBox)Property: ";
            logString += "ActualValue [" + ((ScrollBarVisibility)actualValue).ToString() +
                "] ExpectedValue [" + ((ScrollBarVisibility)expectedValue).ToString() + "]";
            Verifier.Verify((ScrollBarVisibility)actualValue == (ScrollBarVisibility)expectedValue, logString, true);
            #endregion
        }

        #endregion
    }

    /// <summary>
    /// ApplyTemplate will be called multiple times, and the properties on the TextBox will be verified
    /// to ensure that they survive a style change and visual tree regeneration
    /// (Verification will be done by checking the property values).
    /// </summary>
    [Test(0, "TextBox", "EditControlApplyTemplateTest", MethodParameters = "/TestCaseType:EditControlApplyTemplateTest")]
    [TestOwner("Microsoft"), TestTactics("635"), TestWorkItem("77"), TestBugs("657")]
    public class EditControlApplyTemplateTest : CustomTestCase
    {
        #region Private Members
        private Control _testElement;
        private UIElementWrapper _testElementWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex=0;
        private DockPanel _container;
        #endregion

        #region Setup
        private void CreateTestElement()
        {
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

            //setting properties for testElement
            _testElement.Height = 200;
            _testElement.Width = 200;
            _testElement.FontSize = 24;
            _testElement.Background = Brushes.Red;
            _testElement.AllowDrop = false;

            if (!(_testElement is PasswordBox))
            {
                ReflectionUtils.SetProperty(_testElement, "AcceptsTab", true);
            }

            if (_textEditableType.Type == typeof(TextBox))
            {
                ReflectionUtils.SetProperty(_testElement, "AcceptsReturn", true);
                ReflectionUtils.SetProperty(_testElement, "MaxLength", 100);
                ReflectionUtils.SetProperty(_testElement, "MaxLines", 5);
            }
            else if (_testElement is RichTextBox)
            {
                ReflectionUtils.SetProperty(_testElement, "AcceptsReturn", false);
            }

            _testElementWrapper = new UIElementWrapper(_testElement);
        }

        private void AddToTree()
        {
            //Adding the testElement to the tree
            //testElement.Parent = null;
            _container.Children.Clear();
            _container.Children.Add(_testElement);

            //_container.Parent = null;
            MainWindow.Content = null;
            MainWindow.Content = _container;
        }
        #endregion

        #region Main flow.
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _container = new DockPanel();
            DoIteration();
        }

        private void DoIteration()
        {
            if (TextEditableType.Values[_editableTypeIndex].IsPassword)
            {
                FinishedIteration();
                return;
            }

            CreateTestElement();
            AddToTree();

            Style testElementStyle = new Style(_textEditableType.Type);
            ControlTemplate testElementControlTemplate = new ControlTemplate(_textEditableType.Type);
            FrameworkElementFactory innerElement = new FrameworkElementFactory(typeof(ScrollViewer),"PART_ContentHost");
            innerElement.SetValue(ScrollViewer.BackgroundProperty, Brushes.LightBlue);            
            testElementControlTemplate.VisualTree = innerElement;
            testElementStyle.Setters.Add(new Setter(Control.TemplateProperty, testElementControlTemplate));

            _testElement.Style = testElementStyle;
            _testElement.ApplyTemplate();

            VerifyTestElement();

            FinishedIteration();
        }

        private void VerifyTestElement()
        {
            Verifier.Verify(_testElement.Height == 200,
                "Verifying the Height property", true);

            Verifier.Verify(_testElement.Width == 200,
                "Verifying the Width property", true);

            Verifier.Verify(_testElement.FontSize == 24,
                "Verifying the FontSize property", true);

            Verifier.Verify(_testElement.Background == Brushes.Red,
                "Verifying the Background property", true);

            Verifier.Verify(_testElement.AllowDrop == false,
                "Verifying the AllowDrop property", true);

            if (!(_testElement is PasswordBox))
            {
                Verifier.Verify((bool)ReflectionUtils.GetProperty(_testElement, "AcceptsTab") == true,
                    "Verifyign the AcceptsTab property", true);
            }

            if (_textEditableType.Type == typeof(TextBox))
            {
                Verifier.Verify((bool)ReflectionUtils.GetProperty(_testElement, "AcceptsReturn") == true,
                    "Verifying the AcceptsReturn property", true);

                Verifier.Verify((int)ReflectionUtils.GetProperty(_testElement, "MaxLength") == 100,
                    "Verifying the MaxLength property", true);

                Verifier.Verify((int)ReflectionUtils.GetProperty(_testElement, "MaxLines") == 5,
                    "Verifying the MaxLines property", true);
            }
            else if (!(_testElement is PasswordBox))
            {
                Verifier.Verify((bool)ReflectionUtils.GetProperty(_testElement, "AcceptsReturn") == false,
                    "Verifying the AcceptsReturn property", true);
            }
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
        #endregion
    }

    /// <summary>
    /// Verify the default TextBox style to ensure that all rendering properties are aliased
    /// somewhere unless they inherit (Background, BorderBrush, BorderThickness, FlowDirection,
    /// FontFamily, FontSize, FontStretch, FontStyle, FontWeight, Foreground, HorizontalAlignment,
    /// TextDecorations, TextTrimmings).
    /// ******************This test currently only runs in Classic theme*********************
    /// </summary>
    [Test(2, "TextBox", "EditControlPropAliasTest", MethodParameters = "/TestCaseType:EditControlPropAliasTest")]
    [TestOwner("Microsoft"), TestTactics("636"), TestWorkItem("77"), TestBugs("658"), TestLastUpdatedOn("May 21, 2006")]
    public class EditControlPropAliasTest : CustomTestCase
    {
        #region Private Members

        private Control _testElement;
        private UIElementWrapper _testElementWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex = 0;
        private DockPanel _container;
        private DependencyPropertyData[] _dpDataArray;

        #endregion

        #region Test case setup.

        private void CreateTestElement()
        {
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

            _dpDataArray =GetVisualProperties(_textEditableType.Type);
            _testElement.Height = 200;
            _testElement.Width = 200;
            _testElementWrapper = new UIElementWrapper(_testElement);
        }

        private void AddToTree()
        {
            //Adding the testElement to the tree
            //testElement.Parent = null;
            _container.Children.Clear();
            _container.Children.Add(_testElement);

            //_container.Parent = null;
            MainWindow.Content = null;
            MainWindow.Content = _container;
        }

        #endregion Test case setup.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Current theme: [" + Win32.SafeGetCurrentThemeName() + "]");
            if (string.Compare(Win32.SafeGetCurrentThemeName(), "Luna", true,
                                    System.Globalization.CultureInfo.InvariantCulture) == 0)
            {
                Log("Skipping further testing since this test case is theme dependant and fails in Luna");
                Logger.Current.ReportSuccess();
                return;
            }
            _container = new DockPanel();
            DoIteration();
        }

        private void DoIteration()
        {
            CreateTestElement();
            AddToTree();

            QueueDelegate(CheckWithDefaultValues);
        }

        private void CheckWithDefaultValues()
        {
            //Verifying default values only for built in types, not for custom types.
            if (!_textEditableType.IsSubClass)
            {
                Log("Verifying property aliasing with default values");
                VerifyPropertyAliasing(false);
            }

            Log("Assigning custom values for the interesting rendering DependencyProperties");

            //Wrap has to be false to test HorizontalScrollBarVisibilityProperty
            if ((_testElement is TextBox))
            {
                ((TextBox)_testElement).TextWrapping = TextWrapping.NoWrap;                
            }
            foreach (DependencyPropertyData dpData in _dpDataArray)
            {
                _testElement.SetValue(dpData.Property, dpData.TestValue);
            }

            QueueDelegate(CheckWithCustomValues);
        }

        private void CheckWithCustomValues()
        {
            Log("Verifying property aliasing with custom values");
            VerifyPropertyAliasing(true);
            FinishedIteration();
        }

        private void VerifyPropertyAliasing(bool withCustomValues)
        {
            bool aliasedPropertyFound;
            Visual tbVisual = (Visual)_testElement;
            object dpFoundInVisual = null;

            if (_testElement is PasswordBox && !withCustomValues)
            {
                return;
            }

            foreach (DependencyPropertyData dpData in _dpDataArray)
            {
                aliasedPropertyFound = false;
                int count = VisualTreeHelper.GetChildrenCount(tbVisual);

                for (int i = 0; i < count; i++)
                {
                    // Common base class for Visual and Visual3D is DependencyObject
                    DependencyObject childVisual = VisualTreeHelper.GetChild(tbVisual, i);

                    dpFoundInVisual = dpData.FindElementForProperty(childVisual, withCustomValues ? dpData.TestValue : dpData.DefaultValue);
                    if (dpFoundInVisual != null)
                    {
                        aliasedPropertyFound = true;
                        Log(dpData.Property.ToString() + "---Aliased To--->" + dpFoundInVisual.GetType().ToString());
                        break;
                    }
                }
                Verifier.Verify(aliasedPropertyFound, "Verifying that " + dpData.Property +
                    " is aliased to one of its children:", false);
            }
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

        /// <summary>
        /// Get the interesting visual DependencyProperties
        /// </summary>
        /// <param name="elementType">Type of the element for which visual DependencyProperties are needed</param>
        /// <returns>DependencyPropertyData array</returns>
        public static DependencyPropertyData[] GetVisualProperties(Type elementType)
        {
            DependencyPropertyData[] dpDataArray;
            int expectedPaddingThickness = 1;

            OperatingSystem os = Environment.OSVersion; Version ver = os.Version;
            //Visual tree will be change according the widows theme. 
            //The default Padding property of ScrollViewer is �0,0,0,0� on the Win8 OS
            //Update the expect value on Win8 OS         
            if (ver.Major > 6 || ((6 == ver.Major) && ver.Minor > 1))
            {
                expectedPaddingThickness = 0;
            }

            if (elementType == null)
            {
                throw new ArgumentNullException("elementType");
            }

            if (typeof(TextBox).IsAssignableFrom(elementType))
            {
                dpDataArray = new DependencyPropertyData[] {
                   DependencyPropertyData.CreateDPData(TextBox.BackgroundProperty, Brushes.White, BrushData.GradientBrush.Brush),
                    DependencyPropertyData.CreateDPData(TextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Auto),
                    DependencyPropertyData.CreateDPData(TextBox.PaddingProperty, new Thickness(expectedPaddingThickness), new Thickness(8,2,8,2)),
                    DependencyPropertyData.CreateDPData(TextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Auto)
                };
            }
            else if (typeof(RichTextBox).IsAssignableFrom(elementType))
            {
                dpDataArray = new DependencyPropertyData[] {
                    DependencyPropertyData.CreateDPData(RichTextBox.BackgroundProperty, Brushes.White, Brushes.LightGreen),
                    DependencyPropertyData.CreateDPData(RichTextBox.HorizontalScrollBarVisibilityProperty, ScrollBarVisibility.Disabled, ScrollBarVisibility.Auto),
                    DependencyPropertyData.CreateDPData(RichTextBox.PaddingProperty, new Thickness(expectedPaddingThickness), new Thickness(6,2,6,2)),
                    DependencyPropertyData.CreateDPData(RichTextBox.VerticalScrollBarVisibilityProperty, ScrollBarVisibility.Hidden, ScrollBarVisibility.Auto)
                };
            }
            else if (typeof(PasswordBox).IsAssignableFrom(elementType))
            {
                dpDataArray = new DependencyPropertyData[] {
                    DependencyPropertyData.CreateDPData(TextBox.BackgroundProperty, Brushes.White, BrushData.GradientBrush.Brush),
                    DependencyPropertyData.CreateDPData(TextBox.PaddingProperty, new Thickness(expectedPaddingThickness), new Thickness(8,2,8,2)),
                };
            }
            else
            {
                throw new ApplicationException("Use either TextBox or RichTextBox");
            }
            return dpDataArray;
        }

        #endregion
    }

    /// <summary>
    /// CommandBindings property will be tested by adding new commands.
    /// </summary>
    [Test(0, "TextBox", "EditControlCommandBindingsTest", MethodParameters = "/TestCaseType:EditControlCommandBindingsTest")]
    [TestOwner("Microsoft"), TestTactics("637"), TestWorkItem("77")]
    public class EditControlCommandBindingsTest : CustomTestCase
    {
        #region Private Members
        private Control _testElement;
        private UIElementWrapper _testElementWrapper;
        private TextEditableType _textEditableType;
        private int _editableTypeIndex = 0;
        private DockPanel _container;
        #endregion

        #region Setup
        private void CreateTestElement()
        {
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
            _testElement.Height = 200;
            _testElement.Width = 200;
            _testElement.FontSize = 24;

            _testElementWrapper = new UIElementWrapper(_testElement);
        }

        private void AddToTree()
        {
            //Adding the testElement to the tree
            //testElement.Parent = null;
            _container.Children.Clear();
            _container.Children.Add(_testElement);

            //_container.Parent = null;
            MainWindow.Content = null;
            MainWindow.Content = _container;
        }

        #endregion

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _container = new DockPanel();
            DoIteration();
        }

        private void DoIteration()
        {
            CreateTestElement();
            AddToTree();

            //Add a custom Command.
            Log("Adding custom command - a no-op cut command");
            CommandBindingCollection commandCol = _testElement.CommandBindings;
            Verifier.Verify(commandCol.Count == 0, "Verifying that intially, CommandBindings is emtpy", true);
            CommandBinding myCmdBinding = new CommandBinding(ApplicationCommands.Cut, new ExecutedRoutedEventHandler(ReplaceCutByNoop));
            commandCol.Add(myCmdBinding);

            _testElementWrapper.Text = "CommandBindings property test";
            QueueDelegate(RunCutCommand);
        }

        private void RunCutCommand()
        {
            MouseInput.MouseClick(_testElement);
            KeyboardInput.TypeString("^a^x");
            QueueDelegate(VerifyContents);
        }

        private void VerifyContents()
        {
            Verifier.Verify(_testElementWrapper.Text.Contains("CommandBindings property test"),
                "Verifying contents after no-op cut operation", true);
            FinishedIteration();
        }

        private void ReplaceCutByNoop(object sender, ExecutedRoutedEventArgs e)
        {
            Log("Cut command called");
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
        #endregion
    }

    /// <summary> Test class for evaluating value patterns and textpatterns </summary>
    [Test(2, "TextBox", "TextBoxValuePattern", MethodParameters = "/TestCaseType:TextBoxValuePattern")]
    [TestOwner("Microsoft"), TestTactics("634"), TestWorkItem("119"), TestBugs("780, 781")]
    public class TextBoxValuePattern : ManagedCombinatorialTestCase
    {
        /// <summary> filter for combinations read</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            //PASSWORD BOX IS NOT SUPPORTED. REMOVE WHEN IT IS SUPPORTED
            bool result = base.DoReadCombination(values);
            _autoThread = null;
            _autoThread1 = null;
            _functionCount = 0;
            _modifyFunc = false;
            //if (EditableType == TextEditableType.GetByName("PasswordBox"))
            //    return false;
            return true;
        }
        
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _element.Height = 200;
            _ControlWrapper = new UIElementWrapper(_element);
            _ControlWrapper.Clear();
            MainWindow.Content = _element;
            MainWindow.Title = "TextPattern Test App";
            QueueDelegate(DoFocus);
        }

        /// <summary>focuses on the control element</summary>
        private void DoFocus()
        {
            _element.Focus();
          // MouseInput.MouseClick(_element);
           QueueDelegate( FindElement);
        }

        private void FindElement()
        {
            //_ControlWrapper.Clear();
            _autoThread = new Thread(new ThreadStart(GetAutomationElementFocusedElement));
            _autoThread.SetApartmentState(System.Threading.ApartmentState.STA);
            _autoThread.Start();
        }

        private void GetAutomationElementFocusedElement()
        {
            try
            {
                _automationElement = AutomationElement.FocusedElement;
                _className = _automationElement.Current.ClassName;
                _isPatternAvailable = (AutomationUtils.IsTextPatternAvailable(_automationElement));
                _automationElement.TryGetCurrentPattern(TextPattern.Pattern, out _textPatternObject);
                _automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out _valuePatternObject);

            }
            catch (Exception e)
            {
                Log("Unable to get the AutomationElement.FocusedElement. Exception thrown: " + e.ToString());
                throw;
            }

            QueueHelper helper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);
            helper.QueueDelegate(new SimpleHandler(GetDocumentTextUsingTextPatterns));
            
        }        


        /// <summary>GetDocumentTextUsingTextPatterns Main</summary>
        private void GetDocumentTextUsingTextPatterns()
        {
            if (_element is PasswordBox)
            {
                _functionCount++;
                ((PasswordBox)_element).Password = _helloWorld;
                QueueDelegate(SetDocumentTextUsingValuePatterns);

            }
            else
            {
                Log("\r\n ~~~~~~~~~~~~~~~~~~~~~GETTING TEXT~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                GetDocumentTextUsingTextPatternsHelper();
            }
        }

        /// <summary>SetDocumentTextUsingValuePatterns</summary>
        private void SetDocumentTextUsingValuePatterns()
        {
            Log("\r\n ~~~~~~~~~~~~~~~~~~~~~SETTING TEXT~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
            SetDocumentTextUsingPatternsHelper();
        }

        /// <summary>GetSelectedTextUsingTextPattern Main</summary>
        private void GetSelectedTextUsingTextPattern()
        {
            if (_element is PasswordBox)
            {
                _functionCount++;
                ((PasswordBox)_element).Password = _helloWorld;
               QueueDelegate( GetModifiedTextUsingTextPattern);
                
            }
            else
            {
                Log("\r\n ~~~~~~~~~~~~~~~~~~~~~GETTING Selected TEXT~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                _ControlWrapper.Text = "hello world";
                _element.Focus();
                KeyboardInput.TypeString("^{HOME}{RIGHT 2}+{RIGHT 3}");
                QueueDelegate(GetSelectedTextUsingTextPatternSub);
            }
        }

        /// <summary>GetSelectedTextUsingTextPatternSub</summary>
        private void GetSelectedTextUsingTextPatternSub()
        {
           GetSelectedTextUsingTextPatternHelper();
        }

        /// <summary>GetModifiedTextUsingTextPattern Main</summary>
        public void GetModifiedTextUsingTextPattern()
        {
            if (_element is PasswordBox)
            {
                _functionCount++;
                ((PasswordBox)_element).Password = _helloWorld;
                QueueDelegate(NextCombination);

            }
            else
            {
                Log("\r\n ~~~~~~~~~~~~~~~~~~~~~GETTING Modified TEXT~~~~~~~~~~~~~~~~~~~~~~~~~\r\n");
                _element.Focus();

                _initialString = _initialString.TrimEnd();
                KeyboardInput.TypeString("^{HOME}+{RIGHT 4}{DELETE}");
                QueueDelegate(GetModifiedTextUsingTextPatternSub);
            }
        }

        /// <summary>GetModifiedTextUsingTextPatternSub</summary>
        private void GetModifiedTextUsingTextPatternSub()
        {
            GetModifiedTextUsingTextPatternHelper();
        }

        #region Helperfuntions.

        /// <summary>Gets and verifies text within control</summary>
        public void GetDocumentTextUsingTextPatternsHelper()
        {
            _ControlWrapper.Text = "hello world";
            Log("Get document text using TextPattern.");

            if (_automationElement != null)
            {
                Log("    Target element: ClassName:[" + _className +"]\r\n");
                Log("    TextPattern:[" + _isPatternAvailable.ToString() + "]\r\n");
                NextStage();
            }
            else
            {
                throw new ApplicationException("Cannot get Automation element");
            }
        }

        private void NextStage()
        {
            _functionCount++;
            if (_functionCount == 2)
            {
                _ControlWrapper.Clear();

            }
            _autoThread1 = new Thread(new ThreadStart(CallFunctions));
            _autoThread1.SetApartmentState(System.Threading.ApartmentState.STA);
            _autoThread1.Start();
        }

        private void CallFunctions()
        {
                switch (_functionCount)
                {
                    case 1:
                        if (_modifyFunc == true)
                        {
                            GetUIelementText(out _finalString);
                        }
                        else
                        {
                            GetUIelementText(out _initialText);
                        }
                        Log("\r\n**** Get Text successful *****\r\n");
                        break;

                    case 2:
                        SetTextHelper();
                        Log("\r\n**** Set Text successful ****\r\n");
                        break;

                    case 3:
                        GetSelectedText();
                        Log("\r\n**** Get Selected Text successful ****r\n");
                        break;

                    case 4:
                        break;

                    default:
                        break;
                }
               VerificationParts();
                
            
        }

        private void VerificationParts()
        {
            QueueHelper helper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);

            switch (_functionCount)
            {
                case 1:
                    if (_modifyFunc == false)
                    {
                        helper.QueueDelegate(new SimpleHandler(VerifyGetDocumentTextUsingTextPattern));
                    }
                    else
                    {
                        helper.QueueDelegate(new SimpleHandler(VerifyStringOnModification));
                    }
                    break;

                case 2:
                    helper.QueueDelegate(new SimpleHandler(VerifySet));

                    break;

                case 3:
                    helper.QueueDelegate(new SimpleHandler(VerifySelectedText));
                    break;

                case 4:
                    break;

                default:
                    break;
            }
        }

        /// <summary>Gets text within control</summary>
        public void GetUIelementText(out string text)
        {
            text = "";
            
            TextPattern textPattern = (TextPattern)_textPatternObject;

            if (_element is PasswordBox)
            {
                Log("Getting Password");
                ValuePattern val = (ValuePattern)_valuePatternObject;
                text = val.Current.Value;
                Log("Password=[" + text + "]");
            }
            else
            if (textPattern != null)
            {
                TextPatternRange textRange = textPattern.DocumentRange;
                if (textRange != null)
                {
                    Log("Getting GetUIelementText");
                    text = textRange.GetText(3000);
                    text = text.TrimEnd();
                    Log("Document Text=[" + text + "]");
                }
            }
            else
            {
                throw new ApplicationException("FAIL textPattern");
            }
        }

        /// <summary>Sets and verifies text within control</summary>
        public void SetDocumentTextUsingPatternsHelper()
        {
            if (_element is TextBox || _element is PasswordBox )
            {
                _ControlWrapper.Clear();
                Log("Set document text using TextPattern.");


                if (_automationElement != null)
                {
                    Log("    Target element: ClassName:[" + _className + "]\r\n");
                    Log("    TextPattern:[" + _isPatternAvailable.ToString() + "]\r\n");
                    NextStage();
                }
            }
            else
            {
                _functionCount++;
                GetSelectedTextUsingTextPattern();
            }
        }

        private void SetTextHelper()
        {
            string text = _setText;
            ValuePattern valuePattern = (ValuePattern)_valuePatternObject;
            TextPattern tp = (TextPattern)_textPatternObject;
            if (valuePattern != null)
            {
                valuePattern.SetValue(text);
            }
            else
            {
                throw new ApplicationException("----------FAIL VALUE PATTERN--------");
            }
        }

        /// <summary>Gets and verifies selected text within control</summary>
        public void GetSelectedTextUsingTextPatternHelper()
        {
            Log("Get selected document text using TextPattern.");
            if (_automationElement != null)
            {
                Log("    Target element: ClassName:[" + _className + "]\r\n");
                Log("    TextPattern:[" + _isPatternAvailable.ToString() + "]\r\n");
                NextStage();
            }
            else
            {
                throw new ApplicationException("Cannot get Automation element");
            }
        }

        private void GetSelectedText()
        {
            string text = "";

            TextPattern textPattern = (TextPattern)_textPatternObject;
            if (textPattern != null)
            {
                TextPatternRange[] textRanges = textPattern.GetSelection();
                if (textRanges != null && textRanges.Length != 0)
                {
                    text = textRanges[0].GetText(3000);
                    text = text.TrimEnd();
                    Log("Document Text=[" + text + "]");
                }
            }
            _selectedText = text;
        }

        /// <summary>Gets and verifies modified text within control</summary>
        public void GetModifiedTextUsingTextPatternHelper()
        {
            _initialString = _initialText;
            _functionCount = 0;
            _modifyFunc = true;
            NextStage();            
        }

        private void VerifyStringOnModification()
        {
            _finalString = _finalString.TrimEnd();
            VerifyGetModifiedTextUsingTextPattern();
        }
 
        /// <summary>Verfies text within control</summary>
        public void VerifyGetDocumentTextUsingTextPattern()
        {
            string text = _initialText;
           
            if (_element is PasswordBox)
            {
                Verifier.Verify(text.Length == _helloWorld.Length, "Text should be the same length. Length  Expected [" + _helloWorld.Length + "] actual[" + text.Length, true);
                Verifier.Verify(text == _helloWorld, "Text should be the same. Text  Expected [" + text + "] actual[hello world]", true);
            }
            else
            {
                //THERE ARE POSSIBILITIES OF STRINGS BEING SAME LENGTH BUT DIFF VALUES
                Verifier.Verify(text.Length == _helloWorld.Length, "Text should be the same length. Length Expected [" + _helloWorld.Length + "11] actual[" + text.Length, true);
                Verifier.Verify(text == _helloWorld, "Text should be the same. Text  Expected [" + text + "] actual[hello world]", true);
            }
            Log("\r\n**** Verify Retrieved Text Completed ****\r\n");
            QueueDelegate(SetDocumentTextUsingValuePatterns);
        }

        /// <summary>verifies modified text within control</summary>
        private void VerifyGetModifiedTextUsingTextPattern()
        {
            if (_element is PasswordBox )
            {
                Verifier.Verify(_finalString == _oworld, "Strings should be equal.Expected [o world] Actual" + _finalString, true);
                Verifier.Verify(_finalString.Length == _oworld.Length, "Text should be same length. Length  Expected [" + _oworld.Length + "] actual[" + _finalString.Length, true);
            }
            else
            {
                Verifier.Verify(_finalString == _oworld, "Strings should be equal.Expected [o world] Actual" + _finalString, true);

                Verifier.Verify(_finalString.Length == _oworld.Length, "Text should be same length. Length  Expected [" + _oworld.Length + "] actual[" + _finalString.Length, true);
            }
            Log("\r\n**** Verify Get Modified Text Completed ****\r\n\r\n\r\n\r\n\r\n\r\n\r\n");
            QueueDelegate(NextCombination);
        }

        /// <summary>verifies selected text within control</summary>
        private void VerifySelectedText()
        {
            String text = _selectedText;
            if (_element is PasswordBox)
            {
                Verifier.Verify(text.Length == _llo.Length, "Text should be the same length. Length Expected [" + _llo.Length + "] actual[" + text.Length, true);
            }
            else
            {
                Verifier.Verify(text.Length == _llo.Length, "Text should be the same length. Length Expected [" + _llo.Length + "] actual[" + text.Length, true);
                Verifier.Verify(text == _llo, "Text should be the same. Expected [" + text + "] actual[llo]", true);
            }
            Log("\r\n**** Verifying Get Selected Text Completed ****\r\n");
            QueueDelegate(GetModifiedTextUsingTextPattern);
        }

        /// <summary>verifies text within control</summary>
        private void VerifySet()
        {
            string text = _setText;
            string temp = _ControlWrapper.Text.TrimEnd();
           
            Verifier.Verify(text == temp, "Text should not be equal. Text Expected [" + text + "]Actual:[" + temp +"]", true);
            Verifier.Verify(text.Length == temp.Length, "Text length should be equal. Length Expected [" + text.Length.ToString() + "]Actual:[" + temp.Length.ToString() +"]", true);
            Log("\r\n**** Verifying Setting Text Completed ****\r\n");
            QueueDelegate(GetSelectedTextUsingTextPattern);
        }

        #endregion Helperfuntions.

        #region private data.

        private string _initialString = "";
        private string _helloWorld = "hello world";
        private string _llo = "llo";
        private string _oworld = "o world";
        private string _finalString = "";

        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _ControlWrapper;
        private AutomationElement _automationElement;
        private Thread _autoThread = null;
        private Thread _autoThread1 = null;
        private string _className = "";
        private bool _isPatternAvailable = false;
        private string _initialText="";
        private int _functionCount = 0;
        private string _setText = "once again";
        private object _valuePatternObject = null;
        private object _textPatternObject = null;
        private string _selectedText = "";
        private bool _modifyFunc = false;

        #endregion private data.
    }


     /// <summary> Tests value pattern on readonly property </summary>
    [Test(0, "TextBox", "IsReadOnlyValuePattern", MethodParameters = "/TestCaseType:IsReadOnlyValuePattern")]
    [TestOwner("Microsoft"), TestTactics("633"), TestWorkItem("120")]
    public class IsReadOnlyValuePattern : ManagedCombinatorialTestCase
    {

        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            _element.Height = 200;
            _ControlWrapper = new UIElementWrapper(_element);
            _ControlWrapper.Clear();
            TestElement = _element;
            MainWindow.Title = "TextPattern Test App";
            QueueDelegate(DoFocus);
        }

        /// <summary>focuses ans sets readonly property on the control element</summary>
        private void DoFocus()
        {
            _element.Focus();
            if (_element is PasswordBox)
            {  } //cannot set ReadOnly property
            else
            {
                ((TextBoxBase)_element).IsReadOnly = true;
            }
            QueueDelegate(FindElement);
        }

        /// <summary>find the control element</summary>
        private void FindElement()
        {
                _autoThread = new Thread(new ThreadStart(GetAutomationElementFocusedElement));
                _autoThread.SetApartmentState(System.Threading.ApartmentState.STA);
                _autoThread.Start();
        }

        /// <summary>GetAutomationElementFocusedElement</summary>
        private void GetAutomationElementFocusedElement()
        {
            try
            {
                _automationElement = AutomationElement.FocusedElement;
                _className = _automationElement.Current.ClassName;
                Log("inside thread");
                if (_element is RichTextBox)
                { }
                else
                {
                    _automationElement.TryGetCurrentPattern(ValuePattern.Pattern, out _valuePatternObject);
                    ValuePattern valuePattern = (ValuePattern)_valuePatternObject;
                    if (_element is PasswordBox)
                    {
                        Verifier.Verify(valuePattern.Current.IsReadOnly == false, "ReadOnly on PasswordBox Expected[False] Actual [" +
                            valuePattern.Current.IsReadOnly.ToString() + "]", true);
                    }
                    else
                    {
                        Verifier.Verify(valuePattern.Current.IsReadOnly == true, "ReadOnly on TextBox Expected[true] Actual [" +
                              valuePattern.Current.IsReadOnly.ToString() + "]", true);
                    }
                }                
            }
            catch (Exception e)
            {
                Log("Unable to get the AutomationElement.FocusedElement. Exception thrown: " + e.ToString());
                throw;
            }

            QueueHelper helper = new QueueHelper(  GlobalCachedObjects.Current.MainDispatcher);
            helper.QueueDelayedDelegate(new TimeSpan(0,0,2), new SimpleHandler(GotoNextCombination),null);
        }

        private void GotoNextCombination()
        {
            NextCombination();
        }

        #region private data.

        private FrameworkElement _element;
        private TextEditableType _editableType = null;
        private UIElementWrapper _ControlWrapper;
        private AutomationElement _automationElement;
        private Thread _autoThread = null;
        private string _className = "";
        private object _valuePatternObject = null;

        #endregion private data.
    }
}

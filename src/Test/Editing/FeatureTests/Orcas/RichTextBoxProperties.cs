// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides test cases for RichTextBox properties introduced in Orcas.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.IO;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Logging;

    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    #endregion Namespaces.

    /// <summary>
    /// This test case tests special functionality for Hyperlinks by RichTextBox when 
    /// hosted inside it like Tooltip and ContextMenu
    /// </summary>
    [Test(1, "RichTextBox", "HyperlinkInRichTextBoxTest", MethodParameters = "/TestCaseType:HyperlinkInRichTextBoxTest")]
    [TestOwner("Microsoft"), TestLastUpdatedOn("Microsoft 8th, 2007")]
    public class HyperlinkInRichTextBoxTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>starts the combination</summary>
        protected override void DoRunCombination()
        {
            _panel = new StackPanel();
            _rtb = (RichTextBox)_editableType.CreateInstance();
            _rtb.IsDocumentEnabled = true;
            _rtbWrapper = new UIElementWrapper(_rtb);
            _otherRTB = new RichTextBox();
            _toolTipOpeningEventCount = _toolTipClosingEventCount = _menuItemExecuteCount = 0;

            _run = new Run("Hyperlink in RichTextBox: ");

            _link = new Hyperlink();
            _link.NavigateUri = new Uri("http://www.live.com");
            _link.Inlines.Add(new Run("live"));
            _link.ToolTip = toolTipContent;
            _link.ToolTipOpening += new ToolTipEventHandler(link_ToolTipOpening);
            _link.ToolTipClosing += new ToolTipEventHandler(link_ToolTipClosing);

            _contextMenu = new ContextMenu();
            _item1 = new MenuItem();
            _item1.Header = "Dummy";
            _item1.Click += new RoutedEventHandler(item1_Click);
            _contextMenu.Items.Add(_item1);
            _link.ContextMenu = _contextMenu;

            _para = new Paragraph();
            _para.Inlines.Add(_run);
            _para.Inlines.Add(_link);
            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(_para);

            _panel.Children.Add(_rtb);
            _panel.Children.Add(_otherRTB);

            TestElement = _panel;
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _rtb.Focus();
            QueueDelegate(DoMouseOverLink);
        }

        private void DoMouseOverLink()
        {
            TextPointer tp = _link.ContentStart.GetPositionAtOffset(_link.ContentStart.GetOffsetToPosition(_link.ContentEnd) / 2);
            _linkRect = _rtbWrapper.GetGlobalCharacterRect(tp);
            _awayRect = _rtbWrapper.GetGlobalCharacterRect(0);

            Log("Mouse over the hyperlink to test if ToolTip comes up");
            MouseInput.MouseMove((int)_linkRect.Left, (int)(_linkRect.Top + (_linkRect.Height / 2)));
            //Have some delay in delegating next action because typically tooltip animates in and out.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new SimpleHandler(DoMouseAway), null);
        }

        private void DoMouseAway()
        {
            Log("Mouse away from the hyperlink to test if ToolTip goes away");
            MouseInput.MouseMove((int)_awayRect.Left, (int)(_awayRect.Top + (_awayRect.Height / 2)));
            QueueDelegate(CheckEventCount);
        }

        private void CheckEventCount()
        {
            Verifier.Verify(_toolTipOpeningEventCount == 1, "Verifying ToolTip opened", true);
            Verifier.Verify(_toolTipClosingEventCount == 1, "Verifying ToolTip closed", true);

            Log("Perform mouse right-click on the hyperlink to bring up ContextMenu");
            MouseInput.MouseMove((int)_linkRect.Left, (int)(_linkRect.Top + (_linkRect.Height / 2)));
            MouseInput.RightMouseDown();
            MouseInput.RightMouseUp();

            QueueDelegate(ContextMenuThroughMouse);
        }

        private void ContextMenuThroughMouse()
        {
            QueueDelegate(InvokeMenuItem);
            QueueDelegate(VerifyContextMenuThroughMouse);
        }

        private void VerifyContextMenuThroughMouse()
        {
            Verifier.Verify(_menuItemExecuteCount == 1, "Verifying context menu shows up through mouse right-click", true);

            Log("Perform Shift+F10 on the hyperlink to bring up ContextMenu");
            //caret (empty selection) should already be in the hyperlink by above mouse right-click operation
            KeyboardInput.TypeString("+{F10}");
            QueueDelegate(ContextMenuThroughKeyboard);
        }

        private void ContextMenuThroughKeyboard()
        {
            QueueDelegate(InvokeMenuItem);
            QueueDelegate(VerifyContextMenuThroughKeyboard);
        }

        private void VerifyContextMenuThroughKeyboard()
        {
            Verifier.Verify(_menuItemExecuteCount == 2, "Verifying context menu shows up through keyboard activation", true);

            Log("Copying the content from 1st RTB to 2nd RTB");
            _rtb.SelectAll();
            _rtb.Copy();
            _otherRTB.Document.Blocks.Clear();
            _otherRTB.Paste();

            VerifyToolTipSerialization();
            NextCombination();
        }

        private void VerifyToolTipSerialization()
        {
            Paragraph testParagraph = (Paragraph)_otherRTB.Document.Blocks.FirstBlock;
            Verifier.Verify(testParagraph != null, "Verifying the Paragraph got pasted", false);
            //Hyperlink should be the last inline in the paragraph
            Hyperlink testLink = (Hyperlink)testParagraph.Inlines.LastInline;
            Verifier.Verify(testLink != null, "Verifying the Hyperlink got pasted", false);

            Verifier.Verify((string)testLink.ToolTip == toolTipContent,
                "Verifying ToolTip of Hyperlink in RichTextBox is serialized", true);
            Verifier.Verify(testLink.ContextMenu == null,
                "Making sure ContextMenu of Hyperlink is not serialized", true);
        }

        #endregion

        #region Helpers and EventHandlers

        private void InvokeMenuItem()
        {
            //To invoke the MenuItem in ContextMenu (after ContextMenu already shows up)
            KeyboardInput.TypeString("{DOWN}{ENTER}");
        }

        private void link_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _toolTipClosingEventCount++;
        }

        private void link_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _toolTipOpeningEventCount++;
        }

        private void item1_Click(object sender, RoutedEventArgs e)
        {
            _menuItemExecuteCount++;
        }

        #endregion

        #region Private fields

        private TextEditableType _editableType = null;
        private RichTextBox _rtb,_otherRTB;
        private UIElementWrapper _rtbWrapper;
        private StackPanel _panel;

        private Hyperlink _link;
        private Rect _linkRect,_awayRect;
        private Paragraph _para;
        private Run _run;
        private ContextMenu _contextMenu;
        private MenuItem _item1;
        private const string toolTipContent = "Control+Click to navigate";

        private int _toolTipOpeningEventCount,_toolTipClosingEventCount,_menuItemExecuteCount;

        #endregion
    }

    /// <summary>
    /// Verifies the functionality of IsDocumentEnabled property on the RichTextBox.
    /// This test case checks if click event goes through embedded objects like Button, 
    /// CheckBox, TextBlock and Hyperlink TextElement in RichTextBox. It also checks for 
    /// hand cursor when mouse is over hyperlink and control key is pressed. The above 
    /// checks are repeated after setting IsDocumentEnabled to false.
    /// </summary>
    [Test(0, "RichTextBox", "RichTextBoxIsDocumentEnabled", MethodParameters = "/TestCaseType:RichTextBoxIsDocumentEnabled")]
    [TestOwner("Microsoft"), TestLastUpdatedOn("2-20-2007")]
    public class RichTextBoxIsDocumentEnabled : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Runs the test case.</summary>
        protected override void DoRunCombination()
        {
            Initialize();            
            QueueDelegate(ExecuteTrigger);
        }

        /// <summary>Program logic entry.</summary>
        private void ExecuteTrigger()
        {            
            Verifier.Verify(_link.IsEnabled == false, "Link should be disabled by Default", true);
            Verifier.Verify(_button.IsEnabled == false, "Button should be disabled by Default", true);
            Verifier.Verify(_check.IsEnabled == false, "CheckBox should be disabled by Default", true);
            Verifier.Verify(_rtb.IsDocumentEnabled == false, "IsDocumentEnabled should be disabled by Default", true);

            _rtb.IsDocumentEnabled = true;
            QueueDelegate(CheckAfterEnablingDocument);
        }

        /// <summary>CheckAfterEnablingDocument.</summary>
        private void CheckAfterEnablingDocument()
        {
            GlobalLog.LogDebug("******* Document Enabled");

            Verifier.Verify(_link.IsEnabled == true, "Link should be enabled", true);
            Verifier.Verify(_button.IsEnabled == true, "Button should be enabled", true);
            Verifier.Verify(_check.IsEnabled == true, "CheckBox should be enabled", true);
            Verifier.Verify(_rtb.IsDocumentEnabled == true, "IsDocumentEnabled should be enabled", true);

            TextPointer linkMidPointTP = _link.ContentStart.GetPositionAtOffset(_link.ContentStart.GetOffsetToPosition(_link.ContentEnd) / 2);
            _linkClickRect = _wrapper.GetGlobalCharacterRect(linkMidPointTP);
            
            DoMouseMove_Enabled();
        }

        private void DoMouseMove_Enabled()
        {
            Log("Moving mouse over hyperlink");
            MouseInput.MouseMove((int)_linkClickRect.Left, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));
                       
            QueueDelegate(PressControlKey_Enabled);
        }

        private void PressControlKey_Enabled()
        {            
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on enabled hyperlink: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Log("Pressing Control Key");
            KeyboardInput.PressVirtualKey(Win32.VK_LCONTROL);

            QueueDelegate(DoLightMouseMoveRight_Enabled);
        }

        private void DoLightMouseMoveRight_Enabled()
        {
            //Do a light mouse move after control key is pressed so that cursor is updated.
            MouseInput.MouseMove((int)_linkClickRect.Left + 1, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));
            QueueDelegate(DoMouseClick_Enabled);
        }

        private void DoMouseClick_Enabled()
        {
            Verifier.Verify(_currCursor == Cursors.Hand,
                "Verifying cursor on enabled hyperlink after control key is pressed: Expected [" + Cursors.Hand.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Log("Clicking mouse over hyperlink");
            MouseInput.MouseClick();

            QueueDelegate(ReleaseControlKey_Enabled);
        }

        private void ReleaseControlKey_Enabled()
        {
            Log("Releasing Control Key");
            KeyboardInput.ReleaseVirtualKey(Win32.VK_LCONTROL);

            QueueDelegate(DoLightMouseMoveLeft_Enabled);
        }

        private void DoLightMouseMoveLeft_Enabled()
        {
            //Do a light mouse move after control key is released so that cursor is updated.
            MouseInput.MouseMove((int)_linkClickRect.Left - 1, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));
            QueueDelegate(DoClickButton_Enabled);
        }

        private void DoClickButton_Enabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on enabled hyperlink after control key is released: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_button);
            QueueDelegate(DoClickCheckBox_Enabled);
        }

        private void DoClickCheckBox_Enabled()
        {
            Verifier.Verify(_currCursor == Cursors.Arrow,
                "Verifying cursor on enabled Button: Expected [" + Cursors.Arrow.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_check);
            QueueDelegate(DoClickTextBlock_Enabled);
        }

        private void DoClickTextBlock_Enabled()
        {
            Verifier.Verify(_currCursor == Cursors.Arrow,
                "Verifying cursor on enabled CheckBox: Expected [" + Cursors.Arrow.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_tbl);
            QueueDelegate(CheckAfterClicking);
        }

        /// <summary>CheckAfterClicking.</summary>
        private void CheckAfterClicking()
        {
            Verifier.Verify(_currCursor == Cursors.Hand,
                "Verifying cursor on enabled TextBlock: Expected [" + Cursors.Hand.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Verifier.Verify(_check.IsChecked == true, "CheckBox should be checked", true);
            Verifier.Verify(_buttonClick == 1, "Button should be Clicked Once", true);
            Verifier.Verify(_checkBoxClick == 1, "CheckBox should be Clicked Once", true);
            Verifier.Verify(_linkClick == 2, "Hyperlink.RequestNavigateEvent should trigger twice", true);

            GlobalLog.LogDebug("******* Document Disabled ********");
            _rtb.IsDocumentEnabled = false;

            DoMouseMove_Disabled();
        }

        private void DoMouseMove_Disabled()
        {
            Log("Moving mouse over hyperlink");
            MouseInput.MouseMove((int)_linkClickRect.Left, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));

            QueueDelegate(PressControlKey_Disabled);
        }

        private void PressControlKey_Disabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled hyperlink: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Log("Pressing Control Key");
            KeyboardInput.PressVirtualKey(Win32.VK_LCONTROL);

            QueueDelegate(DoLightMouseMoveRight_Disabled);
        }

        private void DoLightMouseMoveRight_Disabled()
        {
            //Do a light mouse move after control key is pressed so that cursor is updated.
            MouseInput.MouseMove((int)_linkClickRect.Left + 1, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));
            QueueDelegate(DoMouseClick_Disabled);
        }

        private void DoMouseClick_Disabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled hyperlink after control key is pressed: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Log("Clicking mouse over hyperlink");
            MouseInput.MouseClick();

            QueueDelegate(ReleaseControlKey_Disabled);
        }

        private void ReleaseControlKey_Disabled()
        {
            Log("Releasing Control Key");
            KeyboardInput.ReleaseVirtualKey(Win32.VK_LCONTROL);

            QueueDelegate(DoLightMouseMoveLeft_Disabled);
        }

        private void DoLightMouseMoveLeft_Disabled()
        {
            //Do a light mouse move after control key is released so that cursor is updated.
            MouseInput.MouseMove((int)_linkClickRect.Left - 1, (int)(_linkClickRect.Top + (_linkClickRect.Height / 2)));
            QueueDelegate(DoClickButton_Disabled);
        }

        private void DoClickButton_Disabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled hyperlink after control key is released: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_button);
            QueueDelegate(DoClickCheckBox_Disabled);
        }

        private void DoClickCheckBox_Disabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled Button: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_check);
            QueueDelegate(DoClickTextBlock_Disabled);
        }

        private void DoClickTextBlock_Disabled()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled CheckBox: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            MouseInput.MouseClick(_tbl);
            QueueDelegate(CheckAfterDisabling);
        }

        /// <summary>CheckAfterDisabling.</summary>
        private void CheckAfterDisabling()
        {
            Verifier.Verify(_currCursor == Cursors.IBeam,
                "Verifying cursor on disabled TextBlock: Expected [" + Cursors.IBeam.ToString() +
                "] Actual [" + _currCursor.ToString() + "]", true);

            Verifier.Verify(_link.IsEnabled == false, "Link should be disabled after Disabling", true);
            Verifier.Verify(_button.IsEnabled == false, "Button should be disabled after Disabling", true);
            Verifier.Verify(_check.IsEnabled == false, "CheckBox should be disabled after Disabling", true);
            Verifier.Verify(_rtb.IsDocumentEnabled == false, "IsDocumentEnabled should be disabled after Disabling", true);

            Verifier.Verify(_check.IsChecked == true, "CheckBox should not respond to clicking", true);
            Verifier.Verify(_buttonClick == 1, "Button should not respond to clicking", true);
            Verifier.Verify(_checkBoxClick == 1, "CheckBox should not respond to clicking", true);
            Verifier.Verify(_linkClick == 2, "Link should not respond to clicking", true);
            
            NextCombination();            
        }

        #endregion

        #region Helpers and EventHandlers

        /// <summary>Initialization helper.</summary>
        private void Initialize()
        {
            _buttonClick = _checkBoxClick = _linkClick = 0;

            //Move mouse to (0,0) as init state
            MouseInput.MouseMove(0, 0);
            Microsoft.Test.Threading.DispatcherHelper.DoEvents(1000);

            _rtb = (RichTextBox)_editableType.CreateInstance();
            
            _rtb.AddHandler(Mouse.QueryCursorEvent, new QueryCursorEventHandler(rtb_QueryCursor), true);
            //The below doesnt work, hence using the AddHandler way as above.
            //_rtb.QueryCursor += new QueryCursorEventHandler(_rtb_QueryCursor);
            _wrapper = new UIElementWrapper(_rtb);

            _button = new Button();
            _button.Content = "Click";
            _button.Click += new RoutedEventHandler(button_Click);

            _check = new CheckBox();
            _check.Checked += new RoutedEventHandler(check_Checked);

            _tbl = new TextBlock();
            _linkInTextBlock = new Hyperlink();
            _linkInTextBlock.NavigateUri = new Uri("http://www.microsoft.com");
            _linkInTextBlock.Inlines.Add(new Run("LINK"));
            _tbl.Inlines.Add(_linkInTextBlock);

            Paragraph p1 = new Paragraph();
            p1.Inlines.Add(new Run("This is a link "));
            _link = new Hyperlink();
            _link.NavigateUri = new Uri("http://www.live.com");
            _link.Inlines.Add(new Run("LIVE"));
            p1.Inlines.Add(_link);

            Paragraph p2 = new Paragraph();
            p2.Inlines.Add(new InlineUIContainer(_button));
            p2.Inlines.Add(_tbl);

            _rtb.Document.Blocks.Clear();
            _rtb.Document.Blocks.Add(p1);
            _rtb.Document.Blocks.Add(p2);
            _rtb.Document.Blocks.Add(new BlockUIContainer(_check));
            _rtb.AddHandler(Hyperlink.RequestNavigateEvent, new System.Windows.Navigation.RequestNavigateEventHandler(RequestNavigatEvent));

            MainWindow.Content = _rtb;            
        }

        void rtb_QueryCursor(object sender, QueryCursorEventArgs e)
        {
            //update the local cursor state variable used for verification.
            _currCursor = e.Cursor;
        }

        /// <summary>RTB link handler.</summary>
        void RequestNavigatEvent(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            _linkClick++;
        }

        /// <summary>CheckBox event handler.</summary>
        void check_Checked(object sender, RoutedEventArgs e)
        {
            _checkBoxClick++;
        }

        /// <summary>button click handler.</summary>
        void button_Click(object sender, RoutedEventArgs e)
        {
            _buttonClick++;
        }

        #endregion

        #region Private fields

        private TextEditableType _editableType = null;
        private TextBlock _tbl = null;
        private RichTextBox _rtb;
        private UIElementWrapper _wrapper;
        private Button _button;
        private CheckBox _check;
        private int _buttonClick,_checkBoxClick,_linkClick;
        private Hyperlink _link,_linkInTextBlock;
        private Rect _linkClickRect;
        private Cursor _currCursor;

        #endregion
    }
}
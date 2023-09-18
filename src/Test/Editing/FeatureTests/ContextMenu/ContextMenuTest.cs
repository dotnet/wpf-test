// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional and unit test cases on ContextMenu of Editing controls

using System;
using System.Collections;
using System.Globalization;
using System.Threading;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Imaging;

using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.Utils;
using Test.Uis.Wrappers;
using Microsoft.Test.Input; 

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 1 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/ContextMenu/ContextMenuTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region enums.

    enum InputChoices
    {
        Programmatical,
        Xaml,
    }

    enum ToolTipType
    {
        Default,
        Programmatical,
        Xaml,
    }

    enum TooltipTypingScenarios
    {
        OpenContextMenu,
        Typing,
    }

    #endregion enums.
    
    /// <summary>
    /// This class intend to test ContextMenu clipboard access in partial trust
    /// ContextMenu is opened by +{F10}, {ALT}e, or RightClick on selection
    /// </summary>
    [Test(2, "Controls", "ContextMenuTest", MethodParameters = "/TestCaseType:ContextMenuTest /Pri:1", Timeout=500)]
    [TestOwner("Microsoft"), TestBugs("393,394,395"), TestTactics("1,2"), TestWorkItem("1"),TestLastUpdatedOn("Jan 25, 2007")]
    public class ContextMenuTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            s_wrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)s_wrapper.Element;
            s_contextMenuString = _contextMenuOpenWith;
            QueueHelper.Current.QueueDelegate(new QueueHelperCallback(OnNextState), new object[] { 0 });
        }

        // Runs the next action.
        private void OnNextState(int state)
        {
            // Do the callback.
            ((Callback)s_states[state])(this);

            if (state + 1 < s_states.Length)
            {
                // Schedule the next callback.
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new QueueHelperCallback(OnNextState), new object[] { state + 1 });
            }
            else
            {
                // Run the same matrix for next control (PWB, RTB, TB, TBSubClass, RTBSubClass).
                QueueDelegate(NextCombination);
            }
        }

        //Mouse click in control to gain focus
        private static void MouseClick(ContextMenuTest thisTest)
        {
            MouseInput.MouseClick(s_wrapper.Element);
        }

        //Type ab then select ab
        private static void StartTest(ContextMenuTest thisTest)
        {
            KeyboardInput.TypeString("ab{Home}+{RIGHT 2}");
        }

        private static void VerifySelection(ContextMenuTest thisTest)
        {
            VerifyResult("ab", "ab");
        }

        //Open context menu with +{F10}, or RightClick on selection
        private static void OpenContextMenu(ContextMenuTest thisTest)
        {
            if (s_contextMenuString == "RightClick")
            {
                //Find left edge of the selection, if empty selection use caret positon
                //add 1 pixel to x cordinate then do right click
                TextSelection ts;
                Rect r;
                System.Windows.Point result;
                ts = s_wrapper.SelectionInstance;
                r = ts.Start.GetCharacterRect(ts.Start.LogicalDirection);
                result = new System.Windows.Point(r.Right, r.Top + r.Height / 2);
                result = ElementUtils.GetScreenRelativePoint(s_wrapper.Element, result);

                MouseInput.RightMouseDown((int)result.X + 1, (int)result.Y);
                MouseInput.RightMouseUp();
            }
            else if (s_contextMenuString=="{ALT}e")
            {
                KeyboardInput.PressOrReleaseOneKey("{ALT}", true);
                KeyboardInput.PressOrReleaseOneKey("e", true);
                KeyboardInput.PressOrReleaseOneKey("e", false);
                KeyboardInput.PressOrReleaseOneKey("{ALT}", false);               
            }
            else
            {
                KeyboardInput.TypeString(s_contextMenuString);
            }
        }        

        private static void ChooseTForCut(ContextMenuTest thisTest)
        {
            KeyboardInput.TypeString(EditingCommandData.GetAccessKeyForCommand(ApplicationCommands.Cut, CultureInfo.CurrentUICulture));
        }

        private static void VerifyCutWithT(ContextMenuTest thisTest)
        {
            VerifyResult("", "");
            KeyboardInput.TypeString("a");
        }

        private static void ChoosePForPaste(ContextMenuTest thisTest)
        {
            KeyboardInput.TypeString(EditingCommandData.GetAccessKeyForCommand(ApplicationCommands.Paste, CultureInfo.CurrentUICulture));
        }

        private static void VerifyPasteWithP(ContextMenuTest thisTest)
        {
            VerifyResult("", "aab");
            KeyboardInput.TypeString("+{LEFT}");
        }

        private static void ChooseCForCopy(ContextMenuTest thisTest)
        {
            VerifyResult("b", "aab");
            KeyboardInput.TypeString(EditingCommandData.GetAccessKeyForCommand(ApplicationCommands.Copy, CultureInfo.CurrentUICulture));
        }

        private static void VerifyCopyWithC(ContextMenuTest thisTest)
        {
            VerifyResult("b", "aab");
            KeyboardInput.TypeString("{END}");
        }

        private static void ArrowDownToPaste(ContextMenuTest thisTest)
        {
            if (s_contextMenuString == "{ALT}e")
            {
                KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            }
            else
            {
                KeyboardInput.TypeString("{DOWN 3}{ENTER}");
            }            
        }

        private static void VerifyArrowPaste(ContextMenuTest thisTest)
        {
            VerifyResult("", "aabb");
            KeyboardInput.TypeString("+{LEFT 2}");
        }

        private static void ArrowDownToCut(ContextMenuTest thisTest)
        {
            if (s_contextMenuString == "{ALT}e")
            {
                KeyboardInput.TypeString("{ENTER}");
            }
            else
            {
                KeyboardInput.TypeString("{DOWN}{ENTER}");
            }            
        }

        //Ctrl+v to paste
        private static void VerifyArrowCut(ContextMenuTest thisTest)
        {
            VerifyResult("", "aa");
            KeyboardInput.TypeString("x^v");
        }

        private static void VerifyControlV(ContextMenuTest thisTest)
        {
            VerifyResult("", "aaxbb");
            KeyboardInput.TypeString("+{LEFT 3}");
        }

        private static void ArrowDownToCopy(ContextMenuTest thisTest)
        {
            if (s_contextMenuString == "{ALT}e")
            {
                KeyboardInput.TypeString("{DOWN}{ENTER}");
            }
            else
            {
                KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            }            
        }

        private static void VerifyArrowCopy(ContextMenuTest thisTest)
        {
            VerifyResult("xbb", "aaxbb");
        }

        private static void ArrowDown3ToPaste(ContextMenuTest thisTest)
        {
            if (s_contextMenuString == "{ALT}e")
            {
                KeyboardInput.TypeString("{DOWN 2}{ENTER}");
            }
            else
            {
                KeyboardInput.TypeString("{DOWN 3}{ENTER}");
            }            
        }

        private static void Shift4Left(ContextMenuTest thisTest)
        {
            KeyboardInput.TypeString("+{LEFT 2}");
        }

        //Verify the last action then run next combination
        private static void VerifyLastAction(ContextMenuTest thisTest)
        {
            VerifyResult("bb", "aaxbb");
        }        

        //Verify the content of a TextEditableType.
        private static void VerifyResult(string selectedText, string AllText)
        {
            if (s_wrapper.Element is RichTextBox)
            {
                AllText += "\r\n";
            }
            Verifier.Verify(s_wrapper.Text == AllText, "After action, expect Text[" +
                AllText + "], Actual Text[" + s_wrapper.Text + "]", true);

            Verifier.Verify(s_wrapper.SelectionInstance.Text == selectedText,
                "After action, expect selected text[" + selectedText +
                "]\nActual selected text[" + s_wrapper.SelectionInstance.Text + "]", true);
        }

        #region Delegates

        // Delegate for QueueHelper callbacks.
        private delegate void QueueHelperCallback(int state);

        // Delegate for OnNextState callbacks.
        private delegate void Callback(ContextMenuTest thisTest);

        #endregion Delegates

        #region Private members

        // Array of actions to run from OnNextState.
        private static readonly Callback[] s_states =
        {
            new Callback(MouseClick),
            new Callback(StartTest),
            new Callback(VerifySelection),
            new Callback(OpenContextMenu),            
            new Callback(ChooseTForCut),
            new Callback(VerifyCutWithT),
            new Callback(OpenContextMenu),
            new Callback(ChoosePForPaste),
            new Callback(VerifyPasteWithP),
            new Callback(OpenContextMenu),
            new Callback(ChooseCForCopy),
            new Callback(VerifyCopyWithC),
            new Callback(OpenContextMenu),
            new Callback(ArrowDownToPaste),
            new Callback(VerifyArrowPaste),
            new Callback(OpenContextMenu),
            new Callback(ArrowDownToCut),
            new Callback(VerifyArrowCut),
            new Callback(VerifyControlV),
            new Callback(OpenContextMenu),
            new Callback(ArrowDownToCopy),
            new Callback(VerifyArrowCopy),
            new Callback(OpenContextMenu),
            new Callback(ArrowDown3ToPaste),
            new Callback(Shift4Left),
            new Callback(VerifyLastAction),
        };

        private static UIElementWrapper s_wrapper;
        private static string s_contextMenuString;
        private TextEditableType _editableType = null;
        private string _contextMenuOpenWith = string.Empty;

        #endregion Private members
    }    
    
    /// <summary>
    /// Verify that the ContextMenu can be overridden through XAML and programmatically. 
    /// Verify that the ContextMenu can be modified right before it is shown by adding, removing, and disabling existing items.
    /// </summary>
    [Test(2, "Controls", "CustomContextMenuTest", MethodParameters = "/TestCaseType:CustomContextMenuTest", Timeout=120)]
    [TestOwner("Microsoft"), TestTactics("3, 4"), TestWorkItem("2, 3"),TestLastUpdatedOn("Jan 25, 2007")]
    public class CustomContextMenuTest : ManagedCombinatorialTestCase
    {
        #region Override Members.

        /// <summary>entry point override </summary>
        protected override void DoRunCombination()
        {
            _controlWrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_controlWrapper.Element;
            _element = TestElement;
            _countCloseContextMenu = _countOpenContextMenu = _countSecondItemFocused = 0;
            QueueDelegate(OpenContextMenu);
        }

        #endregion

        #region Private Members.

        private void OpenContextMenu()
        {
            _element.ContextMenu = null;
            _element.Focus();
            _controlWrapper.Text = _sampleString;
            Clipboard.SetDataObject(_sampleString);

            //Mouse Click used insted of Shift F10 because the latter activates the menu of the Window.
            MouseInput.RightMouseClick(_element);
            OpenContextMenuThroughKeyboard(1);
            QueueDelegate(SelectPasteOption);
        }

        private void OpenContextMenuThroughKeyboard(int triggerCount)
        {
            if (_countOpenContextMenu != triggerCount)
            {
                Input.SendKeyboardInput(Key.RightShift, true);
                Input.SendKeyboardInput(Key.F10, true);
                Input.SendKeyboardInput(Key.RightShift, false);
                Input.SendKeyboardInput(Key.F10, false);
            }
        }

        private void SelectPasteOption()
        {
            KeyboardInput.TypeString("{DOWN 3}{ENTER}{ESC}");
            QueueDelegate(VerifyWhetherContextMenuOpened);
        }

        private void VerifyWhetherContextMenuOpened()
        {
            string _controlText = _controlWrapper.Text;
            _controlText = _controlText.Replace("\r\n", "");
            Verifier.Verify(_controlText == _sampleString , "Expected [" + _sampleString + "] Actual[" + _controlText + "]", true);
            QueueDelegate(AssignContextMenu);
        }

        private void AssignContextMenu() 
        {
            switch (_inputChoiceSwitch)
            {
                case InputChoices.Programmatical:
                    _element.ContextMenu = CreateContextMenuProgrammatically();
                    break;

                case InputChoices.Xaml:
                    _element.ContextMenu = CreateContextMenuThroughXaml();
                    break;
            }

            _element.ContextMenu.Opened += new RoutedEventHandler(ContextMenu_Opened);
            _element.ContextMenu.Closed += new RoutedEventHandler(ContextMenu_Closed);

            ((MenuItem)(_element.ContextMenu.Items.GetItemAt(1))).GotFocus += new RoutedEventHandler(CustomContextMenuTest_GotFocus);

            _element.Focus();
            QueueDelegate(AddItems);
        }

        /// <summary>AddItems to context menu and bring it up.</summary>
        private void AddItems()
        {
            MenuItem m1;
            m1 = new MenuItem();
            m1.Header = "New Item added";
            _element.ContextMenu.Items.Add(m1);
            KeyboardInput.TypeString(_contextMenuOpenedWith);

            OpenContextMenuThroughKeyboard(1);
            QueueDelegate(MouseClick);
        }

        /// <summary>Click control. Menu should disappear.</summary>
        private void MouseClick()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(StartTypingText);
        }

        /// <summary>Type sme text - This should surely make the menu to disappear.</summary>
        private void StartTypingText()
        {
            KeyboardInput.TypeString("hello");
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0,0,2), new SimpleHandler( VerifyIncreasedCountOfItems));
        }

        /// <summary>Verify that the number of items have increased.Open closed events =1</summary>
        private void VerifyIncreasedCountOfItems()
        {
            Verifier.Verify(_element.ContextMenu.Items.Count == _itemsCountAfterAdding, "Expected number of Item ["+_itemsCountAfterAdding.ToString()+"] Actual[" +
                _element.ContextMenu.Items.Count.ToString() + "]", true);

            VerifyClosingOpeningEventCountHelper(1);
            MouseInput.MouseMove(new Point(400, 0));
            QueueDelegate(DeleteAndDisableItems);
        }

        /// <summary>Delete last item and disable first item. Bring up the context menu</summary>
        private void DeleteAndDisableItems()
        {
            ((MenuItem)(_element.ContextMenu.Items.GetItemAt(0))).IsEnabled = false;
            _element.ContextMenu.Items.RemoveAt(_itemsCountAfterAdding-1);

            KeyboardInput.TypeString(_contextMenuOpenedWith + "{DOWN}");
            if (_countOpenContextMenu != 2)
            {
                OpenContextMenuThroughKeyboard(2);
                Input.SendKeyboardInput(Key.Down, true);
                Input.SendKeyboardInput(Key.Down, false);
            }
            QueueDelegate(VerifySelectedItemIsSecondItem);
        }

        /// <summary>Verify that Down key selects second item since first is disabled.</summary>
        private void VerifySelectedItemIsSecondItem()
        {
            Verifier.Verify(_countSecondItemFocused == 1, "First position disabled. Current position Expected [1] Actual [" +
                _countSecondItemFocused.ToString() + "]", true);

            MouseInput.MouseClick(_element);
            QueueDelegate(MakeMenuDisapper);
        }

        /// <summary>Type some text so that context menu disappears.</summary>
        private void MakeMenuDisapper()
        {
            KeyboardInput.TypeString("hello");
            QueueDelegate(VerifyDeletionAndDisabling);
        }

        /// <summary>Verify the deletion and open closed event counts.</summary>
        private void VerifyDeletionAndDisabling()
        {
            Verifier.Verify(_element.ContextMenu.Items.Count == _itemsCountAfterDeletion, "Expected number of Item After Deletion[" + 
                _itemsCountAfterDeletion.ToString() +  "] Actual[" + _element.ContextMenu.Items.Count.ToString() + "]", true);
            Verifier.Verify(((MenuItem)(_element.ContextMenu.Items.GetItemAt(0))).IsEnabled == false, "Expected First item to be disabled Expected [False] Actual[" +
                 ((MenuItem)(_element.ContextMenu.Items.GetItemAt(0))).IsEnabled.ToString() + "]", true);

            VerifyClosingOpeningEventCountHelper(2);
            QueueDelegate(ClearValueContextMenu);
            
        }

        /// <summary>ClearValueContextMenu.</summary>
        private void ClearValueContextMenu()
        {
            _controlWrapper.Text = "";
            Clipboard.SetDataObject(_sampleString);

            _element.ClearValue(FrameworkElement.ContextMenuProperty);
            KeyboardInput.TypeString(_contextMenuOpenedWith );
            QueueDelegate(SelectPasteOptionOnDefaultContextMenu);
        }

        /// <summary>SelectPasteOptionOnDefaultContextMenu.</summary>
        private void SelectPasteOptionOnDefaultContextMenu()
        {
            KeyboardInput.TypeString("{DOWN 3}{ENTER}{ESC}");
            QueueDelegate(VerifyDefaultMenuRestored);
        }

        /// <summary>VerifyDefaultMenuRestored.</summary>
        private void VerifyDefaultMenuRestored()
        {
            string _controlText = _controlWrapper.Text;
            _controlText = _controlText.Replace("\r\n", "");
            Verifier.Verify(_controlText == _sampleString, "Expected [" + _sampleString + "] Actual[" + _controlText + "]", true);
            QueueDelegate(NextCombination);
        }

        #endregion

        #region Internal Members.

        /// <summary>Create context menu programmatically.</summary>
        internal ContextMenu CreateContextMenuProgrammatically()
        {
            MenuItem m1,m2,m3,m4;
            _contextMenu = new ContextMenu();
            m1 = new MenuItem();
            m1.Header = "File";
            m2 = new MenuItem();
            m2.Header = "Save";
            m3 = new MenuItem();
            m3.Header = "SaveAs";
            m4 = new MenuItem();
            m4.Header = "Recent Files";
            _contextMenu.Items.Add(m1);
            _contextMenu.Items.Add(m2);
            _contextMenu.Items.Add(m3);
            _contextMenu.Items.Add(m4);
            return _contextMenu;

        }

        /// <summary>Create context menu through XAML.</summary>
        internal ContextMenu CreateContextMenuThroughXaml()
        {
            string _contextMenuXamlString = "<ContextMenu xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                                               "<MenuItem Header=\"File\"/>"+
                                               "<MenuItem Header=\"Save\"/>"+
                                               "<MenuItem Header=\"SaveAs\"/>"+
                                               "<MenuItem Header=\"Recent Files\"/>"+
                                            "</ContextMenu>";
            _contextMenu = Test.Uis.Utils.XamlUtils.ParseToObject(_contextMenuXamlString) as ContextMenu;
            return _contextMenu;
        }

        /// <summary>Verification helper.</summary>
        internal void VerifyClosingOpeningEventCountHelper(int expected)
        {
            Verifier.Verify((_countCloseContextMenu == expected) && (_countCloseContextMenu == expected), "Opened count. Expected [" +
                expected.ToString() + "] Actual[" + _countOpenContextMenu.ToString() + "] Close count. Expected [" + 
                expected.ToString() + "] Actual [" + _countCloseContextMenu.ToString() + "]", true);
        }

        /// <summary>Closed event for context menu.</summary>
        internal void ContextMenu_Closed(object sender, RoutedEventArgs e)
        {
            _countCloseContextMenu++;
        }

        /// <summary>Opening event for context menu.</summary>
        internal void ContextMenu_Opened(object sender, RoutedEventArgs e)
        {
            _countOpenContextMenu++;
        }

        /// <summary>Focuse event for the second menu item.</summary>
        internal void CustomContextMenuTest_GotFocus(object sender, RoutedEventArgs e)
        {
            _countSecondItemFocused++;
        }

        #endregion Helpers.

        #region data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private InputChoices _inputChoiceSwitch = 0;

        private string _contextMenuOpenedWith="";
        private ContextMenu _contextMenu = null;
        private int _countOpenContextMenu = 0;
        private int _countCloseContextMenu = 0;
        private int _countSecondItemFocused = 0;
        private int _itemsCountAfterAdding = 5;
        private int _itemsCountAfterDeletion = 4;
        private string _sampleString = "hello world";

        #endregion data.
    }
        
    /// <summary>
    /// Verify that tooltip and typing don't go together
    /// Verifies the operation for context menu - Tooltip set programmatically and through XAML
    /// </summary>
    [Test(2, "Controls", "ToolTipTypingTest", MethodParameters = "/TestCaseType:ToolTipTypingTest", Timeout = 520)]
    [TestOwner("Microsoft"), TestTactics("5"), TestWorkItem("2")]
    public class ToolTipTypingTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _controlWrapper = new UIElementWrapper(_editableType.CreateInstance());
            TestElement = (FrameworkElement)_controlWrapper.Element;
            _element = TestElement;

            _countToolTipOpening = _countToolTipClosing = _countToolTipOpened = _countToolTipClosed = 0;
            _element.Height = 50;
            MainWindow.Focus();
            MoveOutOfUIElement();
            QueueDelegate(CreateToolTip);
        }

        /// <summary>Runs a specific combination.</summary>
        private void CreateToolTip()
        {
            _element.ToolTipClosing += new ToolTipEventHandler(_element_ToolTipClosing);
            _element.ToolTipOpening += new ToolTipEventHandler(_element_ToolTipOpening);
            switch (_toolTipTypeSwitch)
            {
                case ToolTipType.Default:
                    _element.ToolTip = _defaultTooltip;
                    break;

                case ToolTipType.Programmatical:
                    _customToolTip = new ToolTip();
                    _customToolTip.Content = _defaultTooltip;
                    _customToolTip.Opened += new RoutedEventHandler(_customToolTip_Opened);
                    _customToolTip.Closed += new RoutedEventHandler(_customToolTip_Closed);
                    _element.ToolTip = _customToolTip;
                    break;

                case ToolTipType.Xaml:
                    string _toolTipXamlString = "<ToolTip xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                                                    _defaultTooltip + "</ToolTip>";
                    _customToolTip = Test.Uis.Utils.XamlUtils.ParseToObject(_toolTipXamlString) as ToolTip;
                    _customToolTip.Opened += new RoutedEventHandler(_customToolTip_Opened);
                    _customToolTip.Closed += new RoutedEventHandler(_customToolTip_Closed);
                    _element.ToolTip = _customToolTip;
                    break;
            }

            _element.Focus();
            MouseInput.MouseMove(ElementUtils.GetScreenRelativeCenter(_element));
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 3), new SimpleHandler(ExecuteTrigger));
            //QueueDelegate(SleepUntilTooltipAppears);
        }

        /// <summary>Controller for tooltip scenarios.</summary>
        private void ExecuteTrigger()
        {
            switch (_tooltipTypingScenariosSwitch)
            {
                case TooltipTypingScenarios.Typing:
                    KeyboardInput.TypeString("hello");
                    break;

                case TooltipTypingScenarios.OpenContextMenu:
                    KeyboardInput.TypeString("+{F10}");
                    break;
            }
            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 2), new SimpleHandler(VerifyThatToolTipDisappeared));
//            QueueDelegate(WaitForToolTipToClose);
        }

        /// <summary>Verify Tooltip disappears.</summary>            
        private void VerifyThatToolTipDisappeared()
        {
            int _openingCount =1;
            int _closingCount = 0; //0 because it was intercepted ..If tooltip vanishes on its own, then closing event is fired.

            VerifyingEventClosingAndOpening(_openingCount,_closingCount);
            KeyboardInput.TypeString("{ESC}123");
            MoveOutOfUIElement();
            QueueDelegate( NextCombination);
        }

        #region Helpers.

        /// <summary>ToolTip Closing event.</summary>
        void _element_ToolTipClosing(object sender, ToolTipEventArgs e)
        {
            _countToolTipClosing++;
        }

        /// <summary>ToolTip Opening event.</summary>
        void _element_ToolTipOpening(object sender, ToolTipEventArgs e)
        {
            _countToolTipOpening++;
        }

        /// <summary>ToolTip Closing event.</summary>
        void _customToolTip_Closed(object sender, RoutedEventArgs e)
        {
            _countToolTipClosed++;
        }

        /// <summary>ToolTip Opening event.</summary>
        void _customToolTip_Opened(object sender, RoutedEventArgs e)
        {
            _countToolTipOpened++;
        }

        /// <summary>Move Mouse out of UIElement.</summary>
        private void MoveOutOfUIElement()
        {
            MouseInput.MouseClick(MainWindow);
            MouseInput.MouseMove(800, 0);
        }

        /// <summary>Helper for verification of opening and closing events.</summary>
        private void VerifyingEventClosingAndOpening(int _openingCount, int _closingCount)
        {
            if (_toolTipTypeSwitch == ToolTipType.Default)
            {
                Verifier.Verify((_countToolTipOpening == _openingCount), "Count of Opening event Expected [" + _openingCount.ToString() +
                    "] Actual [" + _countToolTipOpening.ToString() + "]" , true);
            }
            else
            {
                _closingCount++;
                Verifier.Verify((_countToolTipClosed == _closingCount) && (_countToolTipOpened == _openingCount), "Count of Opening event Expected [" + _openingCount.ToString() +
                    "] Actual [" + _countToolTipOpened.ToString() + "] \r\n Count of Closing events. Expected [" + _closingCount.ToString() +
                    "] Actual [" + _countToolTipClosed.ToString() + "]", true);
            }
        }
        #endregion Helpers.

        #region data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private ToolTipType _toolTipTypeSwitch = 0;
        private TooltipTypingScenarios _tooltipTypingScenariosSwitch = 0;

        private int _countToolTipOpening = 0;
        private int _countToolTipClosing = 0;
        private int _countToolTipOpened = 0;
        private int _countToolTipClosed = 0;
        private string _defaultTooltip = "this is a tooltip";
        private ToolTip _customToolTip ;

        #endregion data.
    }    
}

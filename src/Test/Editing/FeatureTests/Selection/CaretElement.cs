// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the CaretElement type.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/Selection/CaretElement.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Drawing;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Microsoft.Test.Threading;
    
    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;    

    using DrawingPoint = System.Drawing.Point;
    using Point = System.Windows.Point;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that a caret caret always snap to device pixel, 
    /// no matter what property values are assigned to the TextBox/RichTextBox/PasswordBox.
    /// </summary>
    [TestOwner("Microsoft"), TestWorkItem(""), TestTactics("494"), TestBugs("675"), TestLastUpdatedOn("June 8, 2006")]
    public class TestCaretSnapsToDevicePixels : ManagedCombinatorialTestCase
    {
        #region Private fields

        private TextEditableType _editableType=null;
        private bool _settingOnControl=false;
        private double _paddingValue=0;

        private Control _testControl;
        private UIElementWrapper _wrapper;
        private CaretVerifier _caretVerifier;
        private Bitmap _caretBitmap;
        
        #endregion Private fields

        #region Main flow

        /// <summary>Starts running the combination.</summary>
        protected override void DoRunCombination()
        {
            _testControl = (Control)_editableType.CreateInstance();
            _testControl.FontSize = 24;
            _testControl.Height = 100;
            _testControl.Width = 200;

            _wrapper = new UIElementWrapper(_testControl);
            _caretVerifier = new CaretVerifier(_testControl);

            if (_settingOnControl)
            {
                _testControl.SnapsToDevicePixels = true;
            }
            else
            {
                Verifier.Verify(_testControl.SnapsToDevicePixels == false,
                    "Verifying that default property value of SnapsToDevicePixels on the testControl is false", false);
            }

            _testControl.Padding = new Thickness(_paddingValue);

            _wrapper.Text = "          "; //space characters (10), to capture the caret clean
            if (_testControl is PasswordBox)
            {
                ((PasswordBox)_testControl).PasswordChar = ' ';
            }
            TestElement = _testControl;
            
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _testControl.Focus();
            MouseInput.MouseMove(0, 0); //Move mouse away from the picture.
            QueueDelegate(SetCaretInTheMiddle);
        }

        private void SetCaretInTheMiddle()
        {
            //Verify CaretElement's SnapsToDevicePixels property value
            Adorner caretElement = _wrapper.CaretElement;
            UIElement caretSubElement = (UIElement)ReflectionUtils.GetField(caretElement, "_caretElement");            
            Verifier.Verify(caretSubElement.SnapsToDevicePixels,
                "Verifying that caret always has SnapsToDevicePixels set to true", true);

            KeyboardInput.TypeString("{RIGHT 5}"); //move right by half number of space characters initialized above.

            _caretVerifier.CaptureCaret(TestCaretWidthAndColor);            
        }

        private void TestCaretWidthAndColor()
        {
            //verify caret width
            _caretBitmap = _caretVerifier.CaretCaptureResult;
            Logger.Current.LogImage(_caretBitmap, "caretBitmap");
            if (_caretBitmap.Width == 2)
            {
                Verifier.Verify(true, "Caret width equals 2 as expected", true);
            }
            else
            {
                Verifier.Verify(false,
                    "Verifying Caret width. Expected[2] Actual[" + _caretBitmap.Width + "]", true);
                Log("Look at caretBitmap-" + CombinationNumber + ".png");                
            }
            
            //verify pixel color across the two pixels
            Log("Verfifying caret pixel color...");
            for (int i = 0; i < _caretBitmap.Height; i++)
            {
                Verifier.Verify(_caretBitmap.GetPixel(0, i) == _caretBitmap.GetPixel(1, i),
                    "Verifying pixel colors between first and second pixel at " + i + " row", false);
            }

            QueueDelegate(NextCombination);
        }

        #endregion Main flow
    }

    /// <summary>
    /// Verifies that a caret element can be created and visible in the following scenarios:
    /// a) in multiple windows in a single thread.
    /// b) in a window of a different thread.    
    /// To make caret render testing easy, foreground is set to red and black pixels are counted inside the caret rect.
    /// </summary>
    [Test(2, "Selection", "TestCaretVisibility", MethodParameters = "/TestCaseType=TestCaretVisibility", Timeout=120,Disabled=true)]    
    [TestOwner("Microsoft"), TestTactics("495"), TestBugs("819,820,821,822,544"), TestWorkItem("77,78"), TestLastUpdatedOn("May 16, 2006")]
    public class TestCaretVisibility : ManagedCombinatorialTestCase
    {
        #region Private data.

        /// <summary>Editable type used in the test</summary>
        private TextEditableType _editableType =null;

        /// <summary>Text sample to used in the test</summary>
        private string _textSample = string.Empty;

        private double _testFontSize = 24;
        private int _delayTimeForKeyboardLocaleChange = 500;
        private int _delayTimeForWindowFocusChange = 500;

        #region -------Data for first window in Primary thread-------

        /// <summary>Main control in the primary thread</summary>
        private Control _mainControl;

        /// <summary>Dispatcher for the primary thread</summary>
        private Dispatcher _primaryDispatcher;

        /// <summary>Queue helper for primary queue.</summary>
        private QueueHelper _primaryQueueHelper;

        private UIElementWrapper _wrapper;
        private CaretVerifier _caretVerifier;
        private int _selectionStart;

        #endregion

        #region -------Data for second window in Primary thread-------

        /// <summary>Second control in the primary thread</summary>
        private Control _secondControl;

        /// <summary>Second window in the primary thread</summary>
        private Window _secondWindow;

        private UIElementWrapper _secondWrapper;
        private CaretVerifier _secondCaretVerifier;
        private int _secondSelectionStart;

        #endregion

        #region -------Data for first window in Other (Secondary) thread-------

        /// <summary>Dispatcher of the other thread</summary>
        private Dispatcher _otherDispatcher;

        /// <summary>Queue helper for other thread.</summary>
        private QueueHelper _otherQueueHelper;

        /// <summary>Main control in the other thread.</summary>
        private Control _otherMainControl;

        /// <summary>Other thread on which Caret is tested</summary>
        private Thread _otherThread;

        /// <summary>Window in the other thread.</summary>
        private Window _otherWindow;

        private UIElementWrapper _otherWrapper;
        private CaretVerifier _otherCaretVerifier;

        #endregion

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {            
            _mainControl = (Control)_editableType.CreateInstance();
            SetControlProperties(_mainControl);            
            _wrapper = new UIElementWrapper(_mainControl);
            _caretVerifier = new CaretVerifier(_mainControl);
            
            TestElement = _mainControl;            
            
            MainWindow.Top = 0d;
            MainWindow.Left = 0d;
            MainWindow.Title = "First window in the primary thread";
            
            QueueDelegate(DoAfterFocus);
        }

        private void DoAfterFocus()
        {            
            _mainControl.Focus();
            DispatcherHelper.DoEvents();

            Log("Typing in First-PrimaryThread control");
            KeyboardInput.TypeString(_textSample);
            DispatcherHelper.DoEvents();

            Log("Verifying that caret is rendered in the First-PrimaryThread control...");
            VerifyCaretRender(_caretVerifier);

            QueueDelegate(CreateSecondWindow);
        }        

        private void CreateSecondWindow()
        {
            DispatcherHelper.DoEvents();
            //store the state of the selectionstart of the first Edit control in the primary thread.
            _selectionStart = _wrapper.SelectionStart;

            Verifier.Verify(_wrapper.Text.Contains(_textSample), "Verifying the contents of the First-PrimaryThread control", true);            

            _secondControl = (Control)_editableType.CreateInstance();
            SetControlProperties(_secondControl);            
            _secondWrapper = new UIElementWrapper(_secondControl);
            _secondCaretVerifier = new CaretVerifier(_secondControl);

            _secondWindow = new Window();
            _secondWindow.Top = 75d;
            _secondWindow.Left = 125d;
            _secondWindow.Content = _secondControl;
            _secondWindow.Title = "Second window in the primary thread";
            _secondWindow.Show();

            QueueDelegate(DoAfterFocusOnSecondWindow);
        }

        private void DoAfterFocusOnSecondWindow()
        {
            Verifier.Verify(_wrapper.SelectionStart == _selectionStart,
                "Verifying that selection didnt move when focus is lost for the first edit control", false);

            _secondControl.Focus();
            DispatcherHelper.DoEvents();

            Log("Typing in Second-PrimaryThread control...");
            KeyboardInput.TypeString(_textSample);
            DispatcherHelper.DoEvents();

            Log("Verifying that caret is blinking in the Second-PrimaryThread control...");
            VerifyCaretRender(_secondCaretVerifier);

            QueueDelegate(CreateOtherThread);            
        }

        private void CreateOtherThread()
        {
            //storing the state of selectionstart of the second Edit control
            _secondSelectionStart = _secondWrapper.SelectionStart;

            Verifier.Verify(_secondWrapper.Text.Contains(_textSample), "Verifying the contents of the Second-PrimaryThread", true);

            Log("Caret elements are created as a side-effect of having a selection (may happen on click).");

            // Save the primary context and a helper for it.
            _primaryDispatcher = Dispatcher.CurrentDispatcher;
            _primaryQueueHelper = new QueueHelper(_primaryDispatcher);

            // Disable input tracking.
            InputMonitorManager.Current.IsEnabled = false;

            Log("Starting up a new thread to work in a different context...");
            _otherThread = new Thread(OtherThreadStart);
            _otherThread.SetApartmentState(ApartmentState.STA);
            _otherThread.Start();
        }

        private void SuccessOnOtherThread()
        {
            Log("Processing success message from Secondary thread for " + _editableType.XamlName + "...");

            Verifier.Verify(_secondWrapper.SelectionStart == _secondSelectionStart,
                "Verifying that selection didnt move in second edit control when it gains focus " +
                "after window in different thread is closed(Regression_Bug544)", false);

            _secondWindow.Close();

            Verifier.Verify(_wrapper.SelectionStart == _selectionStart,
                "Verifying that selection didnt move in first edit control when it gains focus " +
                "after window in different thread is closed(Regression_Bug544)", false);

            QueueDelegate(NextCombination);
        }

        private void SetControlProperties(Control control)
        {
            control.Height = 50d;
            control.Width = 175d;
            control.FontSize = _testFontSize;
            control.Foreground = System.Windows.Media.Brushes.LightCyan;
            control.HorizontalAlignment = HorizontalAlignment.Left;
            control.VerticalAlignment = VerticalAlignment.Top;
        }

        private void VerifyCaretRender(CaretVerifier caretVerifier)
        {
            // Global state of caret is not affected by this operation.
            caretVerifier.StopCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Bitmap controlSnapshot = BitmapCapture.CreateBitmapFromElement(caretVerifier.Element);

            // Global state of caret is not affected by this operation.
            caretVerifier.StartCaretBlinking();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            Rect caretRect = caretVerifier.GetExpectedCaretRect();
            // Scale rect to Bitmap's DPI
            caretRect = BitmapUtils.AdjustBitmapSubAreaForDpi(controlSnapshot, caretRect);
            Bitmap caretSnapshot = BitmapUtils.CreateSubBitmap(controlSnapshot, caretRect);

            // Converting to black & white will make the caret pure black (for < 4.0 framework) and 
            // converts the light foreground text to white making the verification easy.
            caretSnapshot = BitmapUtils.ColorToBlackWhite(caretSnapshot);
            int blackPixelsInCaretSnapshot = BitmapUtils.CountColoredPixels(caretSnapshot, System.Windows.Media.Colors.Black);
            Log("Number of black pixels in the caret snapshot: " + blackPixelsInCaretSnapshot);
            int expectedBlackPixelsInCaretSnapshot = (3 * (int)_testFontSize) / 4;
            // Number of black pixels in the caret snapshot is expected to be atleast of 3/4th of FontSize height
            if (blackPixelsInCaretSnapshot < expectedBlackPixelsInCaretSnapshot)
            {
                Logger.Current.LogImage(caretSnapshot, "caretSnapShot");
                Logger.Current.LogImage(controlSnapshot, "controlSnapShot");
                Log("Expected number of black pixels in the caret snapshot: " + expectedBlackPixelsInCaretSnapshot);
                Verifier.Verify(false, "Verifying that we have expected # of black pixels in the caretSnapshot", true);
            }
        }

        #region Non-primary thread flow.

        private void OtherThreadStart()
        {
            Log("Starting a different context...");
            _otherDispatcher = Dispatcher.CurrentDispatcher;

            _otherDispatcher.BeginInvoke(DispatcherPriority.Background, new DispatcherOperationCallback(OtherContextStart), null);

            Dispatcher.Run();

            Log("Non-primary work finished.");
            Log("Posting success declaration to primary thread...");
            _primaryQueueHelper.QueueDelegate(SuccessOnOtherThread);
        }

        private object OtherContextStart(object o)
        {
            Log("Creating non-primary window and TextBox...");
            
            _otherMainControl = (Control)_editableType.CreateInstance();
            SetControlProperties(_otherMainControl);            
            _otherWrapper = new UIElementWrapper(_otherMainControl);
            _otherCaretVerifier = new CaretVerifier(_otherMainControl);

            _otherWindow = new Window();
            _otherWindow.Top = 150d;
            _otherWindow.Left = 250d;
            _otherWindow.Title = "Window in the secondary thread";
            _otherWindow.Content = _otherMainControl;
            _otherWindow.Show();

            _otherQueueHelper = new QueueHelper(_otherDispatcher);
            _otherQueueHelper.QueueDelegate(DoAfterFocusOnOtherThread);

            return null;
        }

        private void DoAfterFocusOnOtherThread()
        {
            _otherMainControl.Focus();
            DispatcherHelper.DoEvents();
            
            Log("Typing in the secondary thread...");

            //This is being done in a different thread. Change the locale to english before typing.
            //If this is not done, for example it will fail in BVT run on Japanese OS. 
            KeyboardInput.SetActiveInputLocale(InputLocaleData.EnglishUS.Identifier);
            DispatcherHelper.DoEvents(_delayTimeForKeyboardLocaleChange);            
            KeyboardInput.TypeString(_textSample);
            DispatcherHelper.DoEvents();

            Log("Verifying that caret is blinking in the secondary thread...");
            VerifyCaretRender(_otherCaretVerifier);

            QueueDelegate(OtherMainControlCheckText);
        }

        private void OtherMainControlCheckText()
        {
            Log("Actual contents [" + _otherWrapper.Text + "] Expecting that it contains [" + _textSample + "]");            
            Verifier.Verify(_otherWrapper.Text.Contains(_textSample), "Verifying the contents of the control in secondary thread", true);
            
            KeyboardInput.SetActiveInputLocale(InputLocaleData.ArabicSaudiArabia.Identifier);
            DispatcherHelper.DoEvents(_delayTimeForKeyboardLocaleChange);
            
            //Performing Alt+Tab to move the focus to a different window which has a
            //different active locale. Repro for 
            KeyboardInput.TypeString("%{TAB}");
            DispatcherHelper.DoEvents(_delayTimeForWindowFocusChange);

            KeyboardInput.TypeString("%{TAB}");
            DispatcherHelper.DoEvents(_delayTimeForWindowFocusChange);

            KeyboardInput.SetActiveInputLocale(InputLocaleData.EnglishUS.Identifier);
            DispatcherHelper.DoEvents(_delayTimeForKeyboardLocaleChange);

            _otherQueueHelper.QueueDelegate(CloseSecondThread);
        }

        private void CloseSecondThread()
        {
            _otherWindow.Close();
            _otherDispatcher.BeginInvokeShutdown(DispatcherPriority.Normal);
        }

        #endregion Non-primary thread flow.

        #endregion Main flow.
    }

    /// <summary>
    /// Caret tests for regression bugs.
    /// Coverage for Regression_Bug615,Regression_Bug616 test that caret is rendered and blinking even (a)if the control
    /// is removed and re-added from the tree, (b) if the control is disabled and re-enabled,
    /// (c)if other control in the tree is disabled.
    /// </summary>
    [Test(2, "Selection", "CaretRegressionTesting", MethodParameters = "/TestCaseType=CaretRegressionTesting")]
    [TestOwner("Microsoft"), TestTactics("497"), TestBugs("615,616,810"), TestWorkItem("78")]    
    public class CaretRegressionTesting : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _mainControl = (Control)_editableType.CreateInstance();
            _mainControl.Height = _mainControl.Width = 100d;
            _mainControl.FontSize = 24;
            _mainControl.Padding = new Thickness(3);
            _mainWrapper = new UIElementWrapper(_mainControl);
            if (_textSample != string.Empty)
            {
                _mainWrapper.Text = _textSample;
            }
            _mainCaretVerifier = new CaretVerifier(_mainControl);

            _otherControl = new TextBox();
            _otherControl.Text = "other control";
            _otherControl.Height = _otherControl.Width = 100d;

            _panel = new StackPanel();
            _panel.Children.Add(_mainControl);
            _panel.Children.Add(_otherControl);

            TestElement = _panel;

            QueueDelegate(TestCaretInitialState);
        }

        private void TestCaretInitialState()
        {
            _mainControl.Focus();

            Log("Verifying caret blinking in the initial state");            
            _mainCaretVerifier.VerifyCaretBlinking(DisableOtherControl, true);
        }

        private void DisableOtherControl()
        {
            Log("Disabling the other control...");
            _otherControl.IsEnabled = false;
            KeyboardInput.TypeString("A");
            DispatcherHelper.DoEvents();
            //QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(0), new SimpleHandler(TestCaretSecondState), null);
            TestCaretSecondState();
        }

        private void TestCaretSecondState()
        {
            Log("Verifying caret blinking after the other control is disabled");            
            _mainCaretVerifier.VerifyCaretBlinking(RemoveMainControlFromTree, true);
        }

        private void RemoveMainControlFromTree()
        {
            _otherControl.IsEnabled = true;

            Log("Removing the main Control from the Tree...");
            _panel.Children.Remove(_mainControl);

            QueueDelegate(ReAddMainControlToTree);
        }

        private void ReAddMainControlToTree()
        {
            Log("Re-adding the main Control to the Tree...");
            _panel.Children.Add(_mainControl);
            _mainControl.Focus();

            Log("Verifying caret blinking after it is removed and re-added to the tree");            
            _mainCaretVerifier.VerifyCaretBlinking(DisableMainControl, true);
        }

        private void DisableMainControl()
        {
            Log("Disabling the main control...");
            _mainControl.IsEnabled = false;

            QueueDelegate(ReEnableMainControl);
        }

        private void ReEnableMainControl()
        {
            Log("Enabling the main control back...");
            _mainControl.IsEnabled = true;
            _mainControl.Focus();

            Log("Verifying caret blinking after the control has be disabled and re-enabled");            
            _mainCaretVerifier.VerifyCaretBlinking(NextCombination, true);
        }

        #endregion Main flow.

        #region Private fields

        /// <summary>Editable type to be used in the test</summary>
        private TextEditableType _editableType = null;

        /// <summary>Text sample to be used in the test</summary>
        private string _textSample = "";

        private Control _mainControl;
        private UIElementWrapper _mainWrapper;
        private CaretVerifier _mainCaretVerifier;
        private TextBox _otherControl;
        private StackPanel _panel;

        #endregion Private fields
    }

    /// <summary>
    /// Test that the caret is visible and not blinking when ContextMenu is displayed.
    /// </summary>
    [Test(2, "Selection", "TestCaretWithContextMenu", MethodParameters = "/TestCaseType=TestCaretWithContextMenu")]
    [TestOwner("Microsoft"), TestTactics("498"), TestBugs("676"), TestWorkItem("78")]    
    public class TestCaretWithContextMenu : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _testControl = (Control)_editableType.CreateInstance();
            _testControl.Height = _testControl.Width = 200d;
            _testControl.FontSize = 24;
            _wrapper = new UIElementWrapper(_testControl);
            _wrapper.Text = _textSample;
            _caretVerifier = new CaretVerifier(_testControl);

            TestElement = _testControl;

            QueueDelegate(FocusControl);
        }

        private void FocusControl()
        {
            Log("Focusing the control");
            _testControl.Focus();

            QueueDelegate(BringUpContextMenu);
        }

        private void BringUpContextMenu()
        {
            Log("Bringing up the context menu");
            string typeString = "{RIGHT " + _wrapper.Text.Length / 2 + "}+{F10}";
            KeyboardInput.TypeString(typeString);

            QueueDelegate(VerifyCaretVisibility);
        }

        private void VerifyCaretVisibility()
        {
            Log("Verifying that caret is visible");            
            _caretVerifier.VerifyCaretRendered(NextCombination, true);
        }

        #endregion Main flow

        #region Private data

        /// <summary>Editable type to be used in the test</summary>
        private TextEditableType _editableType = null;

        private Control _testControl;
        private UIElementWrapper _wrapper;
        private CaretVerifier _caretVerifier;
        private string _textSample = StringData.LatinScriptData.Value;

        #endregion Private data
    }

    /// <summary>
    /// Test that the caret is visible around the edges of a Table, TableCell, TableRow, BUIC, IUIC
    /// </summary>
    [Test(1, "Selection", "TestCaretAtBoundaries", MethodParameters = "/TestCaseType=TestCaretAtBoundaries /AroundElementName=BUIC",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("499,500,501"), TestBugs("509,677"), TestLastUpdatedOn("July 28, 2006")]
    public class TestCaretAtBoundaries : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _rtb = new RichTextBox();
            _rtb.FontSize = 34;
            _rtb.Width = 200;
            _rtb.Height = 150;
            _rtb.Document.Blocks.Clear();
            

            _wrapper = new UIElementWrapper(_rtb);
            _caretVerifier = new CaretVerifier(_rtb);

            switch (_aroundElementName)
            {
                case "Table":
                    _tRow1 = new TableRow();
                    _tRow1.Cells.Add(CreateTableCell());
                    _tRow1.Cells.Add(CreateTableCell());
                    _tRow2 = new TableRow();
                    _tRow2.Cells.Add(CreateTableCell());
                    _tRow2.Cells.Add(CreateTableCell());
                    _tRow3 = new TableRow();
                    _tRow3.Cells.Add(CreateTableCell());
                    _tRow3.Cells.Add(CreateTableCell());

                    _trg = new TableRowGroup();
                    _trg.Rows.Add(_tRow1);
                    _trg.Rows.Add(_tRow2);
                    _trg.Rows.Add(_tRow3);

                    _table = new Table();
                    _table.RowGroups.Add(_trg);
                    _table.Columns.Add(CreateTableColumn());
                    _table.Columns.Add(CreateTableColumn());
                    _table.Columns.Add(CreateTableColumn());

                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }
                    _rtb.Document.Blocks.Add(_table);
                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }

                    _numOfCaretPositions = numOfPositionsInTable;
                    break;

                case "BUIC":
                    _button = new Button();
                    _button.Content = "BUIC";
                    _buic = new BlockUIContainer(_button);

                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }
                    _rtb.Document.Blocks.Add(_buic);
                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }

                    _numOfCaretPositions = numOfPositionsInBUIC;
                    break;

                case "IUIC":
                    _button = new Button();
                    _button.Content = "IUIC";
                    _iuic = new InlineUIContainer(_button);

                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }
                    _rtb.Document.Blocks.Add(new Paragraph(_iuic));
                    if (_isSurroundedByPara)
                    {
                        _rtb.Document.Blocks.Add(new Paragraph());
                    }

                    _numOfCaretPositions = numOfPositionsInIUIC;
                    break;
            }
            
            TestElement = _rtb;            
            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {            
            _rtb.Focus();
            QueueDelegate(SetInitialPosition);
        }

        private void SetInitialPosition()
        {
            KeyboardInput.TypeString("^{HOME}");            
            _currentPosition = 0;
            _currCaretRect = _prevCaretRect = Rect.Empty;
            DispatcherHelper.DoEvents();
            Log("Verifying Caret at position " + _currentPosition);
            _caretVerifier.CaptureCaret(VerifyCaretAndMove);
        }

        private void VerifyCaretAndMove()
        {
            _caretBitmap = _caretVerifier.CaretCaptureResult;
            if ((_caretBitmap.Height == 0) && (_caretBitmap.Width == 0))
            {
                Verifier.Verify(false, "CaretCapture failed to get a caret", true);
            }

            //This should remove any red tinges (from the TableCell border) 
            //if exists from caretBitmap and leaves pure caret pixels.
            _caretBitmap = _caretVerifier.ConvertNonBlackPixelsToWhite(_caretBitmap);
            //Convert to BlackWhite and then count # of black pixels
            _caretBitmapBW = BitmapUtils.ColorToBlackWhite(_caretBitmap);

            _prevCaretRect = _currCaretRect;            
            _currCaretRect = _caretVerifier.CaretCaptureRect;

            //Verifying Caret bitmap
            //With fontsize of 14, we should be able to atleast 10 black pixels, caret width being atleast 2.
            if (BitmapUtils.CountColoredPixels(_caretBitmapBW, System.Windows.Media.Colors.Black) > 10)
            {
                Log("Verified that caretBitmap has Black pixels");
            }
            else
            {
                Logger.Current.LogImage(_caretBitmap, "caretCaptureBitmap_" + CombinationNumber.ToString() + "_" + _currentPosition.ToString());
                Logger.Current.LogImage(_caretBitmapBW, "caretCaptureBitmapBW_" + CombinationNumber.ToString() + "_" + _currentPosition.ToString());
                Verifier.Verify(false, "caretBitmap didnt have black pixels at position " + _currentPosition.ToString(), true);
            }

            //Verify Caret position when prev state is established.
            if ((!_prevCaretRect.IsEmpty)&&(!_currCaretRect.IsEmpty))
            {
                Verifier.Verify((_currCaretRect.Top >= _prevCaretRect.Top)||
                    (_currCaretRect.Left >= _prevCaretRect.Left),
                        "Verifying that caret moves", true);                
            }

            if (_currentPosition < _numOfCaretPositions)
            {
                _currentPosition++;
                KeyboardInput.TypeString("{RIGHT}");
                DispatcherHelper.DoEvents();
                _caretVerifier.CaptureCaret(VerifyCaretAndMove);
            }
            else
            {
                NextCombination();
            }
        }

        private TableCell CreateTableCell()
        {
            TableCell cell = new TableCell();
            cell.BorderBrush = System.Windows.Media.Brushes.Red;
            cell.BorderThickness = new Thickness(1);
            return cell;
        }

        private TableColumn CreateTableColumn()
        {
            TableColumn column = new TableColumn();
            if (_isCellWidthSet)
            {
                column.Width = new GridLength(50);
            }
            return column;
        }

        #endregion Main flow

        #region Private data
        
        private RichTextBox _rtb;
        private Table _table;
        private TableRowGroup _trg;
        private TableRow _tRow1,_tRow2,_tRow3;
        private BlockUIContainer _buic;
        private InlineUIContainer _iuic;
        private Button _button;
        private UIElementWrapper _wrapper;
        private CaretVerifier _caretVerifier;

        private const int numOfPositionsInTable = 10;
        private const int numOfPositionsInBUIC = 4;
        private const int numOfPositionsInIUIC = 4;
        private int _numOfCaretPositions;
        private int _currentPosition;
        private Bitmap _caretBitmap,_caretBitmapBW;
        private Rect _currCaretRect,_prevCaretRect;

        private bool _isSurroundedByPara=false;
        private bool _isCellWidthSet=false;
        private string _aroundElementName=string.Empty;

        #endregion Private data
    }

    /// <summary>
    /// Verifies that a caret element is rendered as expected.
    /// </summary>
    [Test(2, "Selection", "CaretElementRender", MethodParameters = "/TestCaseType=CaretElementRender")]
    [TestOwner("Microsoft"), TestBugs("815,816,817,818"), TestTactics("496")]
    public class CaretElementRender : ManagedCombinatorialTestCase
    {
        #region Private data.

        /// <summary>Editable type used in the test</summary>
        private TextEditableType _editableType = null;

        private Control _editControl;
        private UIElementWrapper _wrapper;        

        /// <summary>Bounding box of the last caret found.</summary>
        private Rectangle _lastCaretRectangle;

        /// <summary>Caret width to restore.</summary>
        private int _originalWidth;

        /// <summary>Element on which we test caret movement.</summary>
        private FrameworkElement _caretMovementElement;

        /// <summary>Whether caret movement testing has started.</summary>
        private bool _caretAdvanceStarted;

        /// <summary>Rectangle for caret at start position.</summary>
        private Rect _startCaretRectangle;

        /// <summary>Bounding box of the last caret found while moving.</summary>
        private Rect _lastCaretMoved;

        /// <summary>Rectangle for caret at end position.</summary>
        private Rect _endCaretRectangle;        

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _editControl = (Control)_editableType.CreateInstance();
            _editControl.Height = 100d;
            _editControl.Width = 200d;            

            TestElement = _editControl;
            _wrapper = new UIElementWrapper(_editControl);

            //State initialization
            _caretAdvanceStarted = false;

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            MouseInput.MouseClick(_editControl);
            KeyboardInput.TypeString("^{HOME}");
            _editControl.Focus();
            QueueDelegate(DoType);
        }
        
        private void DoType()
        {            
            KeyboardInput.TypeString("  ");

            // Restore the caret width to the system default if requested.
            if (Settings.GetArgument("RestoreWidth") != "")
            {
                Log("Restoring width to 1 pixel.");
                Win32.SetCaretWidthInPixels(1, true);
                Log("Restored width: " + Win32.GetCaretWidthInPixels());
            }

            QueueDelegate(TestClicked);            
        }

        private void TestClicked()
        {
            VerifyCaretRendered(true);            
            QueueDelegate(TestCaretScrolled);            
        }

        private void TestCaretScrolled()
        {
            Log("Verifying that caret is still rendered " +
                "when it forces scrolling...");
            KeyboardInput.TypeString("          ");
            
            QueueDelegate(CheckCaretScrolled);            
        }

        private void CheckCaretScrolled()
        {
            VerifyCaretRendered(true);
            KeyboardInput.TypeString("^a{DEL}");
            QueueDelegate(TestCaretMovement);
        }

        private void TestCaretMovement()
        {
            if (_editableType.SupportsParagraphs)
            {
                _wrapper.XamlText = "English <Bold>Text</Bold>.";
            }
            else
            {
                _wrapper.Text = "English Text";
            }

            _caretMovementElement = _editControl;

            QueueDelegate(MoveToStart);            
        }

        /// <summary>Moves caret to start of caret movement element.</summary>
        private void MoveToStart()
        {            
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(AdvanceAndCheckChange);
        }

        /// <summary>Advances caret and queues checking.</summary>
        private void AdvanceAndCheckChange()
        {            
            _lastCaretMoved = GetCaretRect(_caretMovementElement);
            if (!_caretAdvanceStarted)
            {
                _startCaretRectangle = _lastCaretMoved;
            }

            KeyboardInput.TypeString("{RIGHT}");
            QueueDelegate(CheckCaretAdvanced);
        }

        /// <summary>
        /// Checks that caret changed and continues or starts testing start
        /// and end 'jumping' (HOME and END).
        /// </summary>
        private void CheckCaretAdvanced()
        {
            bool isFinished;            // Whether this is the last position.
            Rect advancedRectangle;     // Rectangle caret advanced into.
            
            advancedRectangle = GetCaretRect(_caretMovementElement);
            Log("Previous caret left: " + _lastCaretMoved.Left);
            Log("Current caret left: " + advancedRectangle.Left);
            Verifier.Verify(advancedRectangle.Left > _lastCaretMoved.Left,
                "Caret advanced to the right.", true);

            isFinished = _wrapper.SelectionInstance.End.CompareTo(new TextRange(_wrapper.End, _wrapper.End).Start) == 0;
            if (isFinished)
            {
                // Save rectangle for jump-to-end verification.
                _endCaretRectangle = advancedRectangle;

                Log("Jumping to start...");
                KeyboardInput.TypeString("{HOME}");
                QueueDelegate(CheckJumpedToStart);
            }
            else
            {
                _caretAdvanceStarted = true;
                QueueDelegate(AdvanceAndCheckChange);
            }
        }

        private void CheckJumpedToStart()
        {
            Rect startRectangle;
           
            startRectangle = GetCaretRect(_caretMovementElement);
            Log("Original start rectangle: " + _startCaretRectangle);
            Log("Current start rectangle: " + startRectangle);
            Verifier.Verify(_startCaretRectangle.Left == startRectangle.Left,
                "Caret returned to the original position.", true);

            Log("Jumping to end...");
            KeyboardInput.TypeString("{END}");
            QueueDelegate(CheckJumpedToEnd);
        }

        private void CheckJumpedToEnd()
        {
            Rect endRectangle;
            
            endRectangle = GetCaretRect(_caretMovementElement);
            Log("Original end rectangle: " + _endCaretRectangle);
            Log("Current end rectangle: " + endRectangle);
            Verifier.Verify(_endCaretRectangle.Left == endRectangle.Left,
                "Caret returned to the original position.", true);

            QueueDelegate(TestCaretReshown);
        }        

        private void TestCaretReshown()
        {            
            Log("Testing that Ctrl+Shift+Home discards horizontal position...");
            _wrapper.Clear();

            if (_editControl is TextBoxBase)
            {
                ((TextBoxBase)_editControl).AcceptsReturn = true;
            }

            KeyboardInput.TypeString("***{ENTER}****{ENTER}***^+{HOME}{DOWN}");
            
            QueueDelegate(TestCaretHorizontalDiscarded);
        }

        private void TestCaretHorizontalDiscarded()
        {
            bool caretFound;
            Rectangle caretRect;

            Log("Verifying that caret is on left-hand side of control...");
            caretFound = FindCaret(_editControl, out caretRect);
            Log("Caret found: " + caretFound);
            Log("Caret left position: " + caretRect.Left);
            Verifier.Verify(caretRect.Left < (_editControl.ActualWidth / 3),
                "Caret is on left-hand side of control.", true);

            QueueDelegate(ChangeCaretWidth);
        }

        private void ChangeCaretWidth()
        {
            int caretWidth;

            // Change the caret width.
            _originalWidth = Win32.GetCaretWidthInPixels();
            Log("Width of caret bounds: " + _lastCaretRectangle.Width);
            Log("System metric width:   " + _originalWidth);

            caretWidth = _originalWidth * 3;
            Log("Setting caret width *= 3...");
            Win32.SetCaretWidthInPixels(caretWidth, true);
            Log("New caret with: " + Win32.GetCaretWidthInPixels());

            // Ensure that the caret will be rendered.
            KeyboardInput.TypeString(" ");

            QueueDelegate(TestLargerWidth);
        }

        private void TestLargerWidth()
        {
            Rectangle caretBounds;
            double minBound;
            double maxBound;

            // Restore the width before anyting else might fail.
            Log("Restoring width: " + _originalWidth);
            Win32.SetCaretWidthInPixels(_originalWidth, true);

            caretBounds = VerifyCaretRendered(true);
            Logger.Current.Log("Width of caret: " + caretBounds.Width);
            Logger.Current.Log("Expected width: " + _originalWidth * 3);
            minBound = _originalWidth * 2;
            maxBound = _originalWidth * 4;
            Logger.Current.Log("Verification bounds: [" + minBound + "; "
                + maxBound + "]");
            Verifier.Verify(minBound >= caretBounds.Width &&
                caretBounds.Width <= maxBound, "Width is in acceptable bounds.",
                true);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Verifications.

        /// <summary>
        /// Looks for a caret in the specified element.
        /// </summary>
        /// <param name='element'>Element to look for caret in.</param>
        /// <param name='caretRect'>
        /// After execution, the rectangle bounding the caret.
        /// </param>
        /// <returns>true if the caret was found, false otherwise.</returns>
        private bool FindCaret(UIElement element, out Rectangle caretRect)
        {
            Bitmap elementBitmap;       // Bitmap of element 'as is'.
            Bitmap borderless;          // Bitmap of element without border.
            Bitmap bw;                  // Bitmap of element without border, in black and white.
            Bitmap lineBitmap;          // Bitmap of line with caret.
            Rectangle[] lines;          // Rectangles for lines.
            Rectangle arbitraryLine;    // Rectangle for an arbitrary line.
            int arbitraryLineHeight;    // Height for an arbitrary line.

            System.Diagnostics.Debug.Assert(element != null);

            caretRect = Rectangle.Empty;

            using (elementBitmap = BitmapCapture.CreateBitmapFromElement(element))
            using (borderless = BitmapUtils.CreateBorderlessBitmap(elementBitmap, 3))
            //using (bw = BitmapUtils.ColorToBlackWhite(borderless))
            using (bw = BitmapUtils.ColorToBlackWhite(borderless,200))
            {                
                bool found; // Whether the caret was found.

                lines = BitmapUtils.GetTextLines(bw);
                if (lines.Length == 0)
                {
                    return false;
                }

                Log("Line with caret: " + lines[0].ToString());
                if (lines[0].Width == 0 || lines[0].Height == 0)
                {
                    Log("Line has zero width or height.");
                    Log("Using a line arbitrarily smaller than borderless area.");
                    arbitraryLineHeight = (int)
                        ((double)element.GetValue(TextElement.FontSizeProperty));
                    arbitraryLine = lines[0];
                    arbitraryLine.Width = borderless.Width - 2;
                    arbitraryLine.Height = arbitraryLineHeight;
                    lineBitmap = BitmapUtils.CreateSubBitmap(bw, arbitraryLine);
                }
                else
                {
                    lineBitmap = BitmapUtils.CreateSubBitmap(bw, lines[0]);
                }

                // Look for the caret in the line and fix the offset.
                found = BitmapUtils.GetTextCaret(lineBitmap, out caretRect);
                if (found)
                {
                    caretRect.Offset(lines[0].Left, lines[0].Top);
                }
                return found;
            }
        }

        /// <summary>
        /// Verifies whether the caret is being rendered.
        /// </summary>
        /// <param name='caretExpected'>
        /// Whether the caret should be being rendered.
        /// </param>
        private Rectangle VerifyCaretRendered(bool caretExpected)
        {
            Bitmap boxBitmap;
            Rectangle caretRectangle;
            bool caretFound;

            caretFound = FindCaret(_editControl, out caretRectangle);            
            if (caretFound != caretExpected)
            {
                boxBitmap = BitmapCapture.CreateBitmapFromElement(_editControl);
                if (caretFound)
                {
                    boxBitmap = BitmapUtils.HighlightRectangle(boxBitmap, caretRectangle);
                }
                Logger.Current.LogImage(boxBitmap, "caret");
            }
            Log("Caret expected:  " + caretExpected);
            Log("Caret found:     " + caretFound);
            Log("Caret rectangle: " + caretRectangle.ToString());
            Verifier.Verify(caretExpected == caretFound);
            return caretRectangle;
        }

        private Rect GetCaretRectangle(UIElement editedElement)
        {
            Adorner caretElement;
            GeneralTransform caretTransform;
            Rect result;

            caretElement = new UIElementWrapper(editedElement).CaretElement;
            if (caretElement == null)
            {
                throw new ApplicationException("Element has no caret: " + editedElement);
            }
            caretTransform = caretElement.GetDesiredTransform(null);            

            result = new Rect(new Point(0, 0), caretElement.RenderSize);
            result = caretTransform.TransformBounds(result);
            return result;
        }

        private Rect GetCaretRect(UIElement editedElement)
        {
            Adorner caretElement;
            Win32.POINT point;
            bool result;

            caretElement = new UIElementWrapper(editedElement).CaretElement;
            if (caretElement == null)
            {
                throw new ApplicationException("Element has no caret: " + editedElement);
            }

            result = Win32.GetCaretPos(out point);
            if (result)
            {
                return new Rect(new Point((double)point.x, (double)point.y), caretElement.RenderSize);
            }
            else
            {
                return Rect.Empty;
            }
        }

        #endregion Verifications.       
    }

    /// <summary>
    /// Verifies that a caret renders according to system settings.
    /// </summary>
    [Test(2, "Selection", "CaretSystemSettingsTest", MethodParameters = "/TestCaseType=CaretSystemSettingsTest")]
    [TestOwner("Microsoft"), TestWorkItem("79"), TestTactics("503"), TestBugs("")]    
    public class CaretSystemSettingsTest: ManagedCombinatorialTestCase
    {
        #region Private data.

        /// <summary>Editable type used in the test</summary>
        private TextEditableType _editableType = null;

        /// <summary>Edit control on which caret rendering is tested.</summary>
        private Control _editControl;

        private UIElementWrapper _wrapper;
        private CaretVerifier _caretVerifier;
        
        /// <summary>Caret width to restore.</summary>
        private int _originalWidth;

        /// <summary>Caret blinktime to restore.</summary>
        private int _originalBlinkTime;

        private int _caretBitmapWidth;

        #endregion Private data.

        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _editControl = (Control)_editableType.CreateInstance();
            _editControl.Height = 200d;
            _editControl.Width = 600d;            

            TestElement = _editControl;
            if (TestElement is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _wrapper = new UIElementWrapper(_editControl);
                _caretVerifier = new CaretVerifier(_editControl);
                ((Control)TestElement).FontSize = 20;
                ((Control)TestElement).Foreground = System.Windows.Media.Brushes.White;
                ((Control)TestElement).Background = System.Windows.Media.Brushes.White;
                ((Control)TestElement).BorderThickness = new Thickness(0);

                //Restore the original caret state on Application.Exit. 
                //In case the test case fails in the middle, Application.Exit will
                //take care of restoring the state.
                _originalWidth = Win32.GetCaretWidthInPixels();
                _originalBlinkTime = Win32.GetCaretBlinkTime();
                Application.Current.Exit += new ExitEventHandler(Current_Exit);

                QueueDelegate(DoFocus);
            }
            
        }

        void Current_Exit(object sender, ExitEventArgs e)
        {
            Win32.SetCaretWidthInPixels(_originalWidth, true);
            Win32.SetCaretBlinkTime(_originalBlinkTime);
        }

        private void DoFocus()
        {
            MouseInput.MouseClick(_editControl);
            KeyboardInput.TypeString("^{HOME}");
            _editControl.Focus();
            DispatcherHelper.DoEvents();
            //Verify caret rendered in default state            
            _caretVerifier.VerifyCaretRendered(CaptureOriginalCaretWidth, true);                       
        }

        private void CaptureOriginalCaretWidth()
        {
            _caretVerifier.CaptureCaret(ChangeCaretWidth);                       
        }

        private void ChangeCaretWidth()
        {
            int testCaretWidth;

            _caretBitmapWidth = _caretVerifier.CaretCaptureResult.Width;
            Log("Original Caret Width:      " + _originalWidth);            

            //Setting a larger caret width            
            testCaretWidth = _originalWidth * 10;
            Log("Setting caret width to:" + testCaretWidth);
            Win32.SetCaretWidthInPixels(testCaretWidth, true);
            Log("New caret with: " + Win32.GetCaretWidthInPixels());
            
            KeyboardInput.TypeString(StringData.LatinScriptData.Value);
            DispatcherHelper.DoEvents();
            _caretVerifier.CaptureCaret(TestLargerCaretWidth);
        }

        private void TestLargerCaretWidth()
        {            
            int minBound;
            int maxBound;          

            minBound = _originalWidth * 5;
            maxBound = _originalWidth * 15;

            Log("Width of caret: " + _caretVerifier.CaretCaptureResult.Width);
            Log("Expected width: " + _originalWidth * 10);
            Log("Verification bounds: [" + minBound + "; " + maxBound + "\\"+ _caretBitmapWidth*10 +"]");
            Logger.Current.LogImage(_caretVerifier.CaretCaptureResult, "LargeCaret");


            if (_caretVerifier.CaretCaptureResult.Width * _caretVerifier.CaretCaptureResult.HorizontalResolution / 96.0f >= maxBound)
            {
                Verifier.Verify((minBound <= (_caretVerifier.CaretCaptureResult.Width * _caretVerifier.CaretCaptureResult.HorizontalResolution / 96.0f)) &&
                   (_caretVerifier.CaretCaptureResult.Width <= _caretBitmapWidth * 10), "Width is in Acceptable bounds. Actual" +
                   (_caretVerifier.CaretCaptureResult.Width ),
                    true);
            }
            else
            {
                Verifier.Verify((minBound <= (_caretVerifier.CaretCaptureResult.Width * _caretVerifier.CaretCaptureResult.HorizontalResolution / 96.0f)), "Width is in acceptable bounds. Actual" +
                   (_caretVerifier.CaretCaptureResult.Width * _caretVerifier.CaretCaptureResult.HorizontalResolution / 96.0f),
                    true);
            }
            // Restore the width before anything else might fail.
            Log("Restoring width: " + _originalWidth);
            Win32.SetCaretWidthInPixels(_originalWidth, true);
            DispatcherHelper.DoEvents();                    
            _caretVerifier.VerifyCaretBlinking(IncreaseCaretBlinkTime, true);
        }

        private void IncreaseCaretBlinkTime()
        {
            int testCaretBlinkTime;
            bool success;
            
            Log("Original Caret blink time: " + _originalBlinkTime);            

            testCaretBlinkTime = _originalBlinkTime * 4;
            Log("Increasing caret blink time to:" + testCaretBlinkTime);
            success = Win32.SetCaretBlinkTime(testCaretBlinkTime);
            Log("Increasing caret blink time operation success: " + success);
            Log("Increased caret blink time to: " + Win32.GetCaretBlinkTime());
            
            //Workaround for Regression_Bug167. Type something so that a new caret is created which
            //will pick up the new caret blink time.
            KeyboardInput.TypeString("x");
            DispatcherHelper.DoEvents();
            _caretVerifier.VerifyCaretBlinking(DecreaseCaretBlinkTime, true);
        }

        private void DecreaseCaretBlinkTime()
        {
            bool success;

            Log("Decreasing caret blink time to: " + _originalBlinkTime/2);
            success = Win32.SetCaretBlinkTime(_originalBlinkTime/2);
            Log("Decreasing caret blink time operation success: " + success);
            Log("Decreased caret blink time to: " + Win32.GetCaretBlinkTime());

            //Workaround for Regression_Bug167. Type something so that a new caret is created which
            //will pick up the new caret blink time.
            KeyboardInput.TypeString("{BACKSPACE}");
            DispatcherHelper.DoEvents();
            _caretVerifier.VerifyCaretBlinking(RestoreCaretBlinkTime, true);
        }        

        private void RestoreCaretBlinkTime()
        {
            bool success;

            Log("Restoring caret blink time to: " + _originalBlinkTime);
            success = Win32.SetCaretBlinkTime(_originalBlinkTime);
            Log("Restoring caret blink time operation success: " + success);
            Log("Restored caret blink time to: " + Win32.GetCaretBlinkTime());
            KeyboardInput.TypeString("{BACKSPACE}");
            DispatcherHelper.DoEvents();
            QueueDelegate(NextCombination);
        }

        #endregion Main flow.               
    }

    /// <summary>
    /// Verifies that the caret is rendered no matter what the scrollbar
    /// configuration is.
    /// </summary>
    [Test(2, "Selection", "CaretReproRegression_Bug678", MethodParameters = "/TestCaseType=CaretReproRegression_Bug678")]
    [TestOwner("Microsoft"), TestBugs("678"), TestTactics("502")]
    public class CaretReproRegression_Bug678: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            object[] visibilities;
            Dimension[] dimensions;

            Log("Setting up CombinatorialEngine...");
            visibilities = new object[] {
                ScrollBarVisibility.Auto,
                ScrollBarVisibility.Hidden,
                ScrollBarVisibility.Visible
            };
            dimensions = new Dimension[] {
                new Dimension("VerticalVisibility", visibilities),
                new Dimension("HorizontalVisibility", visibilities),
                new Dimension("TextEditableType", TextEditableType.Values),
            };

            _combinatorial = CombinatorialEngine.FromDimensions(dimensions);

            QueueDelegate(NextCombination);
        }

        private void NextCombination()
        {
            UIElement element;
            TextBoxBase baseBox;

            if (!ReadNextCombination())
            {
                Log("All combinations completed successfully.");
                Logger.Current.ReportSuccess();
                return;
            }

            element = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(element);
            baseBox = element as TextBoxBase;
            if (baseBox != null)
            {
                baseBox.HorizontalScrollBarVisibility = _horizontalVisibility;
                baseBox.VerticalScrollBarVisibility = _verticalVisibility;
            }

            SetupWindow();
            QueueDelegate(FocusAway);
        }

        private void FocusAway()
        {
            Log("Clicking on other control...");
            MouseInput.MouseClick(_otherControl);

            QueueDelegate(CaptureFocusAway);
        }

        private void CaptureFocusAway()
        {
            Log("Capturing bitmap with focus away...");
            _beforeClick = BitmapCapture.CreateBitmapFromElement(_wrapper.Element);

            MouseInput.MouseClick(_wrapper.Element);

            QueueDelegate(CaptureAfterClicked);
        }

        private void CaptureAfterClicked()
        {
            Log("Capturing bitmap with focus set...");
            _afterClick = BitmapCapture.CreateBitmapFromElement(_wrapper.Element);

            Log("Looking for differences before and after click...");
            ActionItemWrapper.VerifyBitmapsDifferent(_beforeClick, _afterClick);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Reads the values for the next combination into
        /// the test case fields.
        /// </summary>
        private bool ReadNextCombination()
        {
            Hashtable values;

            values = new Hashtable();
            if (!_combinatorial.Next(values))
            {
                return false;
            }

            _verticalVisibility = (ScrollBarVisibility) values["VerticalVisibility"];
            _horizontalVisibility = (ScrollBarVisibility) values["HorizontalVisibility"];
            _editableType = (TextEditableType) values["TextEditableType"];

            Log("TextEditableType:     " + _editableType);
            Log("HorizontalVisibility: " + _horizontalVisibility);
            Log("VerticalVisibility:   " + _verticalVisibility);

            return true;
        }

        private void SetupWindow()
        {
            DockPanel topPanel;

            topPanel = new DockPanel();
            _otherControl = new TextBox();
            _otherControl.Width = 50;

            topPanel.Children.Add(_otherControl);
            topPanel.Children.Add(_wrapper.Element);

            MainWindow.Content = topPanel;
        }

        #endregion Helper methods.

        #region Private data.

        private Bitmap _afterClick;
        private Bitmap _beforeClick;
        private CombinatorialEngine _combinatorial;
        private ScrollBarVisibility _verticalVisibility;
        private ScrollBarVisibility _horizontalVisibility;
        private TextEditableType _editableType;
        private UIElementWrapper _wrapper;
        private Control _otherControl;

        #endregion Private data.
    }

    /// <summary>
    /// Verifies that the caret is rendered in a new line following
    /// an Enter key.
    /// </summary>
    [Test(2, "Selection", "CaretReproRegression_Bug679", MethodParameters = "/TestCaseType=CaretReproRegression_Bug679")]
    [TestOwner("Microsoft"), TestTactics("504"), TestBugs("679")]
    public class CaretReproRegression_Bug679: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Setting properties on TextBox...");
            SetTextBoxProperties(TestTextBox);
            TestTextBox.AcceptsReturn = true;
            TestTextBox.FontSize = 96;
            QueueDelegate(AfterAvailable);
        }

        private void AfterAvailable()
        {
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("a");
            QueueDelegate(AfterFirstLineContent);
        }

        private void AfterFirstLineContent()
        {
            Log("Capturing bitmap...");
            _beforeEnter = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            Log("Pressing Enter...");
            KeyboardInput.TypeString("{ENTER}");

            QueueDelegate(AfterEnter);
        }

        private void AfterEnter()
        {
            Log("Verifying that Enter was processed...");
            Log("TextBox text: [" + TestTextBox.Text + "]");
            Verifier.Verify(TestTextBox.Text.IndexOf("\r\n") != -1,
                "TextBox test has a newline.", true);

            Log("Capturing bitmap...");
            _afterEnter = BitmapCapture.CreateBitmapFromElement(TestTextBox);

            ActionItemWrapper.VerifyBitmapsDifferent(_beforeEnter, _afterEnter);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Private data.

        private Bitmap _afterEnter;
        private Bitmap _beforeEnter;

        #endregion Private data.
    }

    /// <summary>
    /// Verifies that the caret accounts for character formatting
    /// regarding font sizes and italics attribute.
    /// </summary>
    /// <PSTask BugID="903" />
    [TestOwner("Microsoft"), TestTactics("505"), TestBugs("168")]
    public class CaretCharacterFormatting: TextBoxTestCase
    {
        #region Private fields.

        private double _smallCaretHeight;
        private double _largeCaretHeight;
        private double _hugeCaretHeight;

        private double _smallFontSize = 8;
        private double _largeFontSize = 10;
        private double _hugeFontSize = 1024;

        #endregion Private fields.

        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            MouseInput.MouseClick(TestTextBox);
            QueueDelegate(CheckSizes);
        }

        private void CheckSizes()
        {
            Log("Verifying that the caret is bigger when the default font is bigger...");
            TestTextBox.FontSize = _smallFontSize;
            _smallCaretHeight = GetUpdatedCaretHeight();
            Log("Small font size: " + _smallCaretHeight);

            TestTextBox.FontSize = _largeFontSize;
            _largeCaretHeight = GetUpdatedCaretHeight();
            Log("Large font size: " + _largeCaretHeight);

            Verifier.Verify(_largeCaretHeight > _smallCaretHeight,
                "Larger font size produces a large caret.", true);

            // Verify that caret remains bigger when typing in bigger font.
            Log("Verifying that the caret remains big as text is typed...");
            KeyboardInput.TypeString("Wj");
            QueueDelegate(CheckCaretRemainsWhenTyping);
        }

        private void CheckCaretRemainsWhenTyping()
        {
            double currentCaretHeight = GetUpdatedCaretHeight();
            Log("Current caret size: " + currentCaretHeight);
            Verifier.Verify(currentCaretHeight == _largeCaretHeight,
                "Caret size remains with same size when typing.", true);

            Log("Verifying that caret works when font is larger than textbox...");
            TestTextBox.FontSize = _hugeFontSize;
            _hugeCaretHeight = GetUpdatedCaretHeight();

            Verifier.Verify(_hugeCaretHeight > _largeCaretHeight,
                "Caret can grow larger than TextBox.", true);

            Log("Verifying that the caret takes the size of the next char to be typed...");
            Log("Setting text to '+++ +++', where last three addition symbols are larger.");
            double startCaretHeight;
            double endCaretHeight;
            double middleCaretHeight;
            //--TextContainer container = TestTextBox.StartPosition.TextContainer;
            TestTextBox.FontSize = _smallFontSize;
            TestTextBox.Text = "+++ +++";
            TextPointer textboxStart = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            TextPointer textboxEnd = Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox);
            TextPointer tempPointer = textboxEnd.GetPositionAtOffset(-3);
            tempPointer = tempPointer.GetPositionAtOffset(0, LogicalDirection.Forward);
            TextRange range = new TextRange(tempPointer, textboxEnd);
            //--TextRange range = new TextRange(
            //--    container.End.CreatePosition(-3, LogicalDirection.Forward), container.End);
            range.ApplyPropertyValue(TextBox.FontSizeProperty, _largeFontSize);

            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(TestTextBox);
            textSelection.Select(textboxStart, textboxStart);
            //--TestTextBox.Selection.MoveToPosition(TestTextBox.StartPosition);
            startCaretHeight = GetUpdatedCaretHeight();
            textSelection.Select(textboxEnd, textboxEnd);
            //--TestTextBox.Selection.MoveToPosition(TestTextBox.EndPosition);
            endCaretHeight = GetUpdatedCaretHeight();
            tempPointer = textboxEnd.GetPositionAtOffset(-3);
            textSelection.Select(tempPointer, tempPointer);
            //--TestTextBox.Selection.MoveToPosition(
            //--    TestTextBox.EndPosition.CreatePosition(-3, LogicalDirection.Forward));
            middleCaretHeight = GetUpdatedCaretHeight();

            Log("Caret height at start: " + startCaretHeight);
            Log("Caret height at middle: " + middleCaretHeight);
            Log("Caret height at end: " + endCaretHeight);
            Verifier.Verify(startCaretHeight < endCaretHeight,
                "Caret height at start is less than at end.", true);
            Verifier.Verify(middleCaretHeight - 0.5 <= endCaretHeight && middleCaretHeight + 0.5 >= endCaretHeight,
                "Caret height in middle is equal to end (expected in LtR).", true);

            Log("Verifying that caret does not grow when there is an embedded element...");
            TestTextBox.Clear();
            TestTextBox.Text = "abc";
            TextPointer p = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            p = p.GetPositionAtOffset(1);
            p = p.GetPositionAtOffset(0, LogicalDirection.Backward);
            Button button = new Button();
            button.Content = "button";
            button.Height = 48;
            // p.InsertUIElement(button);
            textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(TestTextBox);
            textSelection.Select(p,p);
            //--TestTextBox.Selection.MoveToPosition(p);
            double objectCaretHeight = GetUpdatedCaretHeight();
            Log("Caret height next to object: " + objectCaretHeight);

            QueueDelegate(TestRegression_Bug168Setup);
        }

        #region Regression_Bug168.

        private void TestRegression_Bug168Setup()
        {
            Log("Verifying that Regression_Bug168 does not repro...");

            TestRegression_Bug168SetRichContent();
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("{HOME}");

            QueueDelegate(TestRegression_Bug168ReadLargeCaret);
        }

        private void TestRegression_Bug168SetRichContent()
        {
            TextPointer containerStart;
            Run inlineElement;
            Paragraph paraElement;

            // <Paragraph><Inline FontSize="48pt">Text</Inline> </Paragraph>
            inlineElement = new Run("Text");
            inlineElement.FontSize = 48;
            paraElement = new Paragraph(inlineElement);

            TestTextBox.Clear();

            containerStart = Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox);
            //containerStart.InsertTextElement(paraElement);
            //Not sure if we can get the collection from the TextBox. So use the reflection the do this
            Test.Uis.Utils.ReflectionUtils.InvokeInstanceMethod(containerStart, "InsertTextElement", new object[] { paraElement });  
        }

        private void TestRegression_Bug168ReadLargeCaret()
        {
            Log("Verifying the large-sized caret...");
            _largeCaretHeight = GetUpdatedCaretHeight();
            Log("Large caret size: " + _largeCaretHeight);

            KeyboardInput.TypeString("{END}");
            QueueDelegate(TestRegression_Bug168ReadSmallCaret);
        }

        private void TestRegression_Bug168ReadSmallCaret()
        {
            Log("Verifying the small-sized caret...");
            _smallCaretHeight = GetUpdatedCaretHeight();
            Log("Small caret height: " + _smallCaretHeight);
            Verifier.Verify(_largeCaretHeight > _smallCaretHeight,
                "Large caret is larger than small caret.");

            KeyboardInput.TypeString("{LEFT}{RIGHT}");
            QueueDelegate(TestRegression_Bug168TestCaretUnchanged);
        }

        private void TestRegression_Bug168TestCaretUnchanged()
        {
            Log("Verifying the small-sized caret remains small...");
            _smallCaretHeight = GetUpdatedCaretHeight();
            Log("Small caret height: " + _smallCaretHeight);
            Verifier.Verify(_largeCaretHeight > _smallCaretHeight,
                "Large caret is larger than small caret.");

            Log("Regression_Bug168 does not repro.");

            QueueDelegate(TestRegression_Bug169Setup);
        }

        #endregion Regression_Bug168.

        #region Regression_Bug169

        private void TestRegression_Bug169Setup()
        {
            //--TextContainer container;
            Button testButton = new Button();
            testButton.Content = "ClickMe";

            Log("Verifying that Regression_Bug169 does not repro...");
            TestTextBox.Clear();

            //--container = TestTextBox.StartPosition.TextContainer;
            // <Button>ClickMe</Button>This is a test

            //Test.Uis.Utils.TextUtils.GetTextBoxStart(TestTextBox).InsertUIElement(testButton);
            //Test.Uis.Utils.TextUtils.GetTextBoxEnd(TestTextBox).InsertText("This is a test");
            //--container.InsertEmbeddedObject(container.Start, testButton);
            //--container.InsertText(container.End, "This is a test");
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("{HOME}");
            QueueDelegate(TestRegression_Bug169ReadLargeCaret);
        }

        private void TestRegression_Bug169ReadLargeCaret()
        {
            Log("Verifying the large-sized caret...");
            _largeCaretHeight = GetUpdatedCaretHeight();
            Log("Large caret size: " + _largeCaretHeight);

            KeyboardInput.TypeString("{END} ");
            QueueDelegate(TestRegression_Bug169ReadSmallCaret);
        }

        private void TestRegression_Bug169ReadSmallCaret()
        {
            Log("Verifying the small-sized caret...");
            _smallCaretHeight = GetUpdatedCaretHeight();
            Log("Small caret height: " + _smallCaretHeight);
            Verifier.Verify(_largeCaretHeight > _smallCaretHeight,
                "Large caret is larger than small caret.");

            Log("Regression_Bug169 does not repro.");

            QueueDelegate(TestBugRegression_Bug170);
        }

        #endregion Regression_Bug169

        #region Regression_Bug170.

        private void TestBugRegression_Bug170()
        {
            Log("Verifying that Regression_Bug170 does not repro...");
            Log("Caret should be present and visible after deleting all text.");

            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("^a{DEL}");

            QueueDelegate(TestBugRegression_Bug170CheckCaret);
        }

        private void TestBugRegression_Bug170CheckCaret()
        {
            UIElementWrapper wrapper;   // Wrapper to extract caret from.
            Adorner caretElement;       // Caret element extracted.

            wrapper = new UIElementWrapper(TestTextBox);
            caretElement = wrapper.CaretElement;
            Verifier.Verify(caretElement.RenderSize.Width > 0,
                "Caret element exists and has non-zero size.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Regression_Bug170.

        #endregion Main flow.

        #region Helper methods.

        private double GetUpdatedCaretHeight()
        {
            return TestWrapper.GetUpdatedCaretRenderSize().Height;
        }

        #endregion Helper methods.
    }

    /// <summary>
    /// Verifies that the caret gets positioned correctly when the
    /// mouse clicks in different positions.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("123")]
    public class CaretMousePosition: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            TextEditableType editableType;
            string xaml;
            string attributes;
            string content;

            editableType = TextEditableType.Values[_textTypeIndex];
            attributes = "Name='TB' Width='120px' FontSize='14pt' " +
				"TextBox.Wrap='True'";
            content = TextUtils.RepeatString("m ", 128);
            if (editableType.SupportsParagraphs)
            {
                content = "<Paragraph>" + content + "</Paragraph>";
            }
            xaml = "<Canvas xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
                editableType.GetEditableXaml(attributes, content) +
                "</Canvas>";

            ActionItemWrapper.SetMainXaml(xaml);
            _testedControl = (FrameworkElement) ElementUtils.FindElement(TestWindow, "TB");
            _testedWrapper = new UIElementWrapper(this._testedControl);

            // Workaround Regression_Bug1.
            _testedWrapper.SelectionInstance.Select(_testedWrapper.Start, _testedWrapper.Start);
            //--_testedWrapper.SelectionInstance.MoveToPosition(
            //--    _testedWrapper.TextContainer.Start);

            QueueDelegate(TestRendered);
        }

        private void TestRendered()
        {
            // Click on the second line (first line might be indented
            // and causes problems when clicking to the right of it).
            _lineRect = _testedWrapper.GetControlRelativeLineBounds(1);
            ActionItemWrapper.MouseElementRelative(_testedControl,
                "click left " + (int)(_lineRect.Left + 10) +
                " " + (int)(_lineRect.Top + 4));

            QueueDelegate(TestClickedLine);
        }

        private void TestClickedLine()
        {
            // Get the caret position.
            _topLeftLineCaret = GetCaretTopLeft(_testedControl);
            Log("Top-left caret in line: " + _topLeftLineCaret);

            // Click at the end of the line.
            ActionItemWrapper.MouseElementRelative(_testedControl,
                "click left " + (int)(_lineRect.Right) +
                " " + (int)(_lineRect.Top + 4));

            QueueDelegate(TestClickedEndLine);
        }

        private void TestClickedEndLine()
        {
            // Verify that horizontal position changed but vertical position
            // remains the same.
            _topLeftEndLineCaret = GetCaretTopLeft(_testedControl);
            Log("Top-left caret in end of line: " + _topLeftEndLineCaret);

            Verifier.Verify(_topLeftLineCaret.X < _topLeftEndLineCaret.X,
                "End of line is to the right.", true);
            Verifier.Verify(_topLeftLineCaret.Y == _topLeftEndLineCaret.Y,
                "End of line is at same vertical position.", true);

            _textTypeIndex++;
            System.Diagnostics.Debug.Assert(
                _textTypeIndex <= TextEditableType.Values.Length);
            if (_textTypeIndex < TextEditableType.Values.Length)
            {
                QueueDelegate(RunTestCase);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Finds a CaretElement in the specified array, if any.
        /// </summary>
        /// <param name="adorners">Adorners to search, possibly null.</param>
        /// <returns>The first CaretEle</returns>
        public static Adorner FindCaretElement(Adorner[] adorners)
        {
            Adorner adorner;
            string adornerTypeName;

            if (adorners == null)
            {
                return null;
            }

            for (int i = 0; i < adorners.Length; i++)
            {
                adorner = adorners[i];
                if (adorner == null)
                {
                    continue;
                }
                adornerTypeName = adorner.GetType().ToString();
                if (adornerTypeName.ToString().IndexOf("CaretElement") > 1)
                {
                    return adorner;
                }
            }
            return null;
        }

        /// <summary>
        /// Gets the first caret element found associated with
        /// the specified element or its chidlren.
        /// </summary>
        /// <param name="element">Element to retrieve caret element for.</param>
        /// <returns>The first caret element found, null if none are present.</returns>
        /// <exception cref="System.InvalidOperationException">Thrown when element has no adorner decorator.</exception>
        public static Adorner GetCaretElement(UIElement element)
        {
            Adorner result;
            Adorner[] elementAdorners;
            AdornerLayer elementAdornerLayer;
            UIElement childElement;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            elementAdornerLayer = AdornerLayer.GetAdornerLayer(element);
            if (elementAdornerLayer == null)
            {
                throw new InvalidOperationException("Element has no adorner layer.");
            }

            elementAdorners = elementAdornerLayer.GetAdorners(element);
            result = FindCaretElement(elementAdorners);
            if (result == null)
            {
                int count = VisualTreeHelper.GetChildrenCount(element);
                for(int i = 0; i < count; i++)
                {
                    // Common base class for Visual and Visual3D is DependencyObject
                    DependencyObject visual = VisualTreeHelper.GetChild(element,i);

                    childElement = visual as UIElement;
                    if (childElement != null)
                    {
                        result = GetCaretElement(childElement);
                        if (result != null)
                        {
                            return result;
                        }
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// Gets the caret rectangle of the first caret element
        /// associated with the specified element, relative to
        /// the element itself.
        /// </summary>
        /// <param name="element">Element to retrieve caret rectangle for.</param>
        public static Point GetCaretTopLeft(UIElement element)
        {
            Adorner caretElement;
            Matrix transformMatrix;
            Transform caretTransform;
            Transform adornedToElementTransform;
            UIElement adornedElement;

            if (element == null)
            {
                throw new ArgumentNullException("element");
            }

            caretElement = GetCaretElement(element);
            if (caretElement == null)
            {
                return new Point(0, 0);
            }

            adornedElement = caretElement.AdornedElement;
            if (element == adornedElement)
            {
                adornedToElementTransform = null;
            }
            else
            {
                System.Windows.Media.Matrix m;
                System.Windows.Media.GeneralTransform gt = adornedElement.TransformToAncestor(element);
                System.Windows.Media.Transform t = gt as System.Windows.Media.Transform;
                if(t==null)
                {
	                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case");
                }
                m = t.Value;
                adornedToElementTransform = new MatrixTransform(m);
            }
            System.Windows.Media.GeneralTransform gt2 = caretElement.GetDesiredTransform(adornedToElementTransform);
            caretTransform = gt2 as System.Windows.Media.Transform;
            if (caretTransform == null)
            {
                throw new System.ApplicationException("//TODO: Handle GeneralTransform Case");
            }

            transformMatrix = caretTransform.Value;

            return new Point(transformMatrix.OffsetX, transformMatrix.OffsetY);
        }

        #endregion Helper methods.

        #region Private fields.

        private Point _topLeftLineCaret;
        private Point _topLeftEndLineCaret;
        private Rect _lineRect;
        private FrameworkElement _testedControl;
        private UIElementWrapper _testedWrapper;
        private int _textTypeIndex;

        #endregion Private fields.
    }

    /// <summary>
    /// Verify caret formatting attributes on TextBox, RichTextBox in various states
    /// </summary>
    [Test(2, "Selection", "CaretFormattingAttributes1", MethodParameters = "/TestCaseType=CaretFormattingAttributes /StopOnFailure=true", Timeout = 300)]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "CaretFormattingAttributes2", MethodParameters = "/TestCaseType=CaretFormattingAttributes  /StopOnFailure=true /XbapName=EditingTestDeploy", Timeout = 240, Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("506,507"), TestBugs("680")]
    public class CaretFormattingAttributes : CustomCombinatorialTestCase
    {
        #region Private fields

        private TextEditableType _textEditableType;
        private double _testFontSize;
        private string _testContents;
        private bool _testItalic;
        private int _testPosition;

        private StackPanel _panel;
        private Control _testControl;
        private UIElementWrapper _testControlWrapper;
        private CaretVerifier _caretVerifier;         

        #endregion Private fields

        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            Dimension[] dimensions; // Dimensions for combinations.
            string[] contents;
            object[] testPosition;

            contents = new string[] {
                String.Empty,
                "              ",
                "xxxx xx x xxxx",
                "<Paragraph><Run>xxxx </Run><Italic FontSize='24'>xxxx </Italic><Bold FontSize='32'>xxxx</Bold></Paragraph>",
            };

            testPosition = new object[] {2};

            dimensions = new Dimension[] {
                new Dimension("TextEditableType", TextEditableType.PlatformTypes),
                new Dimension("TestFontSize", new object[] {-1.0, 24.0}),
                new Dimension("TestContents", contents),
                new Dimension("TestItalic", new object[] {false, true}),
                new Dimension("TestPosition", testPosition),
            };

            return dimensions;
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _textEditableType = (TextEditableType)values["TextEditableType"];
            _testFontSize = (double)values["TestFontSize"];
            _testContents = (string)values["TestContents"];            
            _testItalic = (bool)values["TestItalic"];
            _testPosition = (int)values["TestPosition"];

            if ((_textEditableType.Type != typeof(RichTextBox)) &&
                (_testContents.Contains("</")))
            {
                //This scenario is for RichTextBox only testing.
                return false;
            }

            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            _panel = new StackPanel();
            TestElement = _panel;

            _testControl = (Control)_textEditableType.CreateInstance();
            
            // Fix the FontFamily to avoid the variation across different OS/Locale
            _testControl.FontFamily = new System.Windows.Media.FontFamily("Tahoma");
            // Fix the foreground to light color so that it doesnt interfere with caret during verification
            _testControl.Foreground = System.Windows.Media.Brushes.LightCyan;
            _testControlWrapper = new UIElementWrapper(_testControl);
            _caretVerifier = new CaretVerifier(_testControl);
            _testControl.Height = 200;
            _testControl.Width = 300;

            _panel.Children.Clear();
            _panel.Children.Add(_testControl);

            SetUpTestCase();
        }

        /// <summary>Sets up the testcontrol based on the _testData.</summary>
        private void SetUpTestCase()
        {
            if (_testFontSize != -1)
            {
                _testControl.FontSize = _testFontSize;
            }

            if (_testItalic)
            {
                _testControl.FontStyle = System.Windows.FontStyles.Italic;
            }

            if (_testControl is TextBox)
            {
                ((TextBox)_testControl).AcceptsReturn = true;
                ((TextBox)_testControl).Text = _testContents;
            }
            else if (_testControl is RichTextBox)
            {
                RichTextBox rtb;
                rtb = _testControl as RichTextBox;
                if (_testContents.Contains("</"))
                {
                    _testControlWrapper.XamlText = _testContents;
                }
                else
                {
                    ((Paragraph)rtb.Document.Blocks.FirstBlock).Inlines.Add(new Run(_testContents));
                }
            }

            QueueDelegate(ClickAction);
        }

        private void ClickAction()
        {
            MouseInput.MouseClick(_testControl);
            QueueDelegate(MoveAction);
        }

        private void MoveAction()
        {
            string typeString = "{END}{LEFT " + _testPosition + "}";
            KeyboardInput.TypeString(typeString);
            QueueDelegate(TestCaret);
        }

        /// <summary>Tests whether caret is rendered.</summary>
        private void TestCaret()
        {
            bool isItalic;
            isItalic = (_testControl is RichTextBox) &&
                ((System.Windows.FontStyle)((RichTextBox)_testControl).Selection.GetPropertyValue(TextElement.FontStyleProperty) == System.Windows.FontStyles.Italic);

            Bitmap caretSnapshot = _caretVerifier.GetCaretRectBitmap();
            // Converting to black & white will make the caret pure black (for < 4.0 framework) and 
            // converts the light foreground text to white making the verification easy.
            caretSnapshot = BitmapUtils.ColorToBlackWhite(caretSnapshot);

            if (isItalic)
            {                                
                BitmapUtils.CheckCaretItalic(caretSnapshot);
            }
            else
            {      
                //For TextBox and PasswordBox, even when FontStyle is set to Italic on them, normal caret will be shown.
                TestCaretRender(caretSnapshot);
            }
            TestCaretHeight(caretSnapshot);
        }        

        private void TestCaretRender(Bitmap caretSnapshot)
        {
            int blackPixelsInCaretSnapshot = BitmapUtils.CountColoredPixels(caretSnapshot, System.Windows.Media.Colors.Black);
            Log("Number of black pixels in the caret snapshot: " + blackPixelsInCaretSnapshot);

            double actualFontSize = GetActualFontSize();
            int expectedBlackPixelsInCaretSnapshot = (3 * (int)actualFontSize) / 4;
            // Number of black pixels in the caret snapshot is expected to be atleast of 3/4th of font size
            if (blackPixelsInCaretSnapshot < expectedBlackPixelsInCaretSnapshot)
            {
                Logger.Current.LogImage(caretSnapshot, "caretSnapShot");                
                Log("Expected number of black pixels in the caret snapshot: " + expectedBlackPixelsInCaretSnapshot);
                Verifier.Verify(false, "Verifying that we have expected # of black pixels in the caretSnapshot", true);
            }
        }

        private double GetActualFontSize()
        {
            double actualFontSize = 0;
            if (_testControl is RichTextBox)
            {
                actualFontSize = (double)((RichTextBox)_testControl).Selection.GetPropertyValue(TextElement.FontSizeProperty);
            }
            else
            {
                actualFontSize = ((Control)_testControl).FontSize;
            }
            return actualFontSize;
        }

        private void TestCaretHeight(Bitmap caretSnapshot)
        {            
            double actualFontSize = GetActualFontSize();            
            float xFactor, yFactor;
            UIElementWrapper.HighDpiScaleFactors(out xFactor, out yFactor);
            double adjustedFontSize = actualFontSize * yFactor;
            double caretBitmapHeight = caretSnapshot.Height;

            string message = "Caret Height: [" + caretBitmapHeight + "] FontSize: [" + actualFontSize + "] AdjustedFontSize(To Dpi) [" + adjustedFontSize + "]";
            if ((caretBitmapHeight < (3*adjustedFontSize)/4) || 
                (caretBitmapHeight > (2 * adjustedFontSize)))
            {
                Logger.Current.ReportResult(false, message, false);
            }

            QueueDelegate(NextCombination);
        }
    }

    /// <summary>Add coverage for Regression_Bug171</summary>
    [Test(2, "Selection", "ReproRegression_Bug171", MethodParameters = "/TestCaseType=ReproRegression_Bug171")]
    [TestOwner("Microsoft"), TestTactics("508"), TestBugs("171, 682, 485")]
    public class ReproRegression_Bug171 : CustomTestCase
    {
        private RichTextBox _rtb1;
        private UIElementWrapper _rtb1Wrapper;
        private CaretVerifier _caretVerifier;
        private string _expectedRightString;

        ///<summary>Setup for the test case</summary>
        public void SetUp()
        {
            _expectedRightString = "ght Nine Ten\r\n";

            Canvas canvasMain = new Canvas();
            canvasMain.Height = 600;
            canvasMain.Width = 600;

            Grid canvas1 = new Grid();
            Canvas.SetTop(canvas1, 100);
            Canvas.SetLeft(canvas1, 50);
            canvas1.Width = 250;
            canvas1.Height = 400;
            canvas1.ClipToBounds = true;
            ScrollViewer sv1 = new ScrollViewer();

            _rtb1 = new RichTextBox();
            _rtb1.Width = 250;
            _rtb1.Height = 400;
            _rtb1.FontSize = 72;

            // Set the FontFamily in .NET 3.5 to ensure the richtextbox dispaly the expected test xaml.
#if TESTBUILD_CLR20
            Log("Set the FontFamily when Test .NET 3.5");
            rtb1.FontFamily = new System.Windows.Media.FontFamily("Arial"); 
#endif
            _rtb1Wrapper = new UIElementWrapper(_rtb1);           
            /*
            Moving the below 3 lines to a separate function SetUpRTB() because, we
            are not able to access SelectionInstance of RTB before it is rendered.
            */
            //rtb1Wrapper.XamlText = "<Paragraph>One Two Three Four Five Six Seven Eight Nine Ten</Paragraph>";
            //TextSelection selection = (TextSelection)(ReflectionUtils.GetProperty(rtb1, "Selection"));
            //selection.MoveToPosition(selection.TextContainer.Start);

            sv1.Content = _rtb1;
            canvas1.Children.Add(sv1);
            canvasMain.Children.Add(canvas1);

            MainWindow.Content = canvasMain;
        }

        ///<summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            SetUp();
            QueueDelegate(new SimpleHandler(SetUpRTB));
        }

        ///<summary>Assigns text to RTB</summary>
        private void SetUpRTB()
        {
            _rtb1Wrapper.XamlText = "<Paragraph>One Two Three Four Five Six Seven Eight Nine Ten</Paragraph>";
            TextSelection selection = _rtb1.Selection;
            selection.Select(selection.Start, selection.Start);
            QueueDelegate(ClickAction1);
        }

        ///<summary>Performs the click action</summary>
        public void ClickAction1()
        {
            MouseInput.MouseClick(_rtb1);
            KeyboardInput.TypeString("{DOWN 7}{UP 2}{LEFT 2}{RIGHT 1}");
            QueueDelegate(VerifyScrollAndCaretVisibility);
        }

        ///<summary>Verifies that text is scrolled and test is visible</summary>
        public void VerifyScrollAndCaretVisibility()
        {
            Log("Verifying that the text has scrolled...");
            string rightString = _rtb1Wrapper.GetTextOutsideSelection(LogicalDirection.Forward);
            Log("Expected text to the right of the cursor: [" + _expectedRightString + "]");
            Log("Actual text to the right of the cursor: [" + rightString + "]");
            Verifier.Verify(rightString == _expectedRightString, "Verify text to the right of the cursor", true);

            Log("Verifying that caret is still visible...");
            _caretVerifier = new CaretVerifier(_rtb1);            
            _caretVerifier.VerifyCaretRendered(LogResult, true);
        }

        private void LogResult()
        {
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Bidi Caret Test
    /// This test might fail if XP/Server/Tablet machine in the lab have "Advanced 
    /// Text Services" turned off. To turn it on go to Run->intl.cpl->Languages->
    /// Details->Advanced->Uncheck "Turn off advanced text services"
    /// </summary>
    [Test(2, "Selection", "TestCaretBiDi", MethodParameters = "/TestCaseType:TestCaretBiDi",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("509"), TestBugs("680,681"), TestWorkItem("136"), TestLastUpdatedOn("May 16, 2006")]
    public class TestCaretBiDi : ManagedCombinatorialTestCase
    {
        #region Private Members

        private TextEditableType _editableType=null;
        private string _testInputLocale=string.Empty;

        private Control _testElement;
        private UIElementWrapper _testWrapper;
        private CaretVerifier _caretVerifier;
        private StackPanel _panel;

        #endregion

        #region Main flow                

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            if (_editableType.IsPassword)
            {
                Log("Skipping combination");
                QueueDelegate(NextCombination);
                return;
            }

            _panel = new StackPanel();
            TestElement = _panel;

            _testElement = (Control)_editableType.CreateInstance();
            _testWrapper = new UIElementWrapper(_testElement);
            _caretVerifier = new CaretVerifier(_testElement);
            _testElement.Height = 300;
            _testElement.Width = 300;
            _testElement.FontSize = 32;
            _panel.Children.Clear();
            _panel.Children.Add(_testElement);            

            QueueDelegate(SetFocus);
        }

        private void SetFocus()
        {
            KeyboardInput.SetActiveInputLocale(InputLocaleData.EnglishUS.Identifier);
            _testElement.Focus();
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(VerifyNormalCaret), null);
        }

        private void VerifyNormalCaret()
        {
            Log("Verifying Normal Caret Rendering");            
            _caretVerifier.VerifyCaretRendered(SetBiDiCaret, true);
        }

        /// <summary>Set test locale to capture test caret</summary>
        private void SetBiDiCaret()
        {
            KeyboardInput.TypeString("^a{DELETE} {BACKSPACE}"); //Cleaning up before setting BiDi
            KeyboardInput.SetActiveInputLocale(_testInputLocale);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(VerifyBiDiCaret), null);
        }

        private void VerifyBiDiCaret()
        {
            Log("Verifying rendering of BiDi caret");            
            _caretVerifier.VerifyCaretBiDi(TypeInBiDi, true);
        }

        /// <summary>Type after locale is changed before capturing for caret</summary>
        private void TypeInBiDi()
        {
            KeyboardInput.TypeString("xxxxx{LEFT}");            
            _caretVerifier.VerifyCaretBiDi(NextCombination, true);
        }

        #endregion Main flow
    }
}

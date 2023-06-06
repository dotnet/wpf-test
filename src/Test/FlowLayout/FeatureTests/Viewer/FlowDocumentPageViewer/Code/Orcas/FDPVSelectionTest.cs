// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Globalization;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Threading;
using System.Windows.Markup;
using System.Windows.Navigation;
using System.Windows.Input;

using Microsoft.Test.Logging;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Testing selection feature in FlowDocumentPageViewer.   
    /// </summary>
    [Test(0, "Viewer.FlowDocumentPageViewer", "ViewerSelectionTest_Orcas", MethodName = "Run")]
    public class FDPVSelectionTest_Orcas : AvalonTest
    {
        #region Test case members
        
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private TextRange _beforeSelection;
        private TextRange _afterSelection;       
        private FrameworkElement _fe;
        private int _viewerType;

        [DllImport("user32.dll")]
        static extern void keybd_event(ushort bVk, ushort bScan, uint dwFlags, IntPtr dwExtraInfo);

        #endregion

        #region Constructor      

        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", 0)]
        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", 1)]
        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", 2)]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", 0)]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", 1)]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", 2)]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", 0)]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", 1)]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", 2)]  

        public FDPVSelectionTest_Orcas(string xamlFile, string testValue, int viewerType)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            this._viewerType = viewerType;

            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        #endregion

        #region Test Steps
       
        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult.Pass;</returns>
        TestResult Initialize()
        {                 
            _navWin = new NavigationWindow();
           
            Status("Initialize");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));

            switch (_viewerType)
            {
                case 0:
                    TestLog.Current.LogStatus("Testing public Selection property on FLowDocumentPageViewer...");
                    _fe = new FlowDocumentPageViewer();
                    ((FlowDocumentPageViewer)_fe).Document = _fd;
                    break;
                case 1:
                    TestLog.Current.LogStatus("Testing public Selection property on FLowDocumentScrollViewer...");
                    _fe = new FlowDocumentScrollViewer();
                    ((FlowDocumentScrollViewer)_fe).Document = _fd;
                    break;
                case 2:
                    TestLog.Current.LogStatus("Testing public Selection property on FLowDocumentReader...");
                    _fe = new FlowDocumentReader();
                    ((FlowDocumentReader)_fe).Document = _fd;
                    break;
            }
           
            _navWin.Content = _fe;
            _navWin.Topmost = true;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            //When running under Test Center, need to bring focus to the test Window (from the console window of the test harness)
            Microsoft.Test.Input.Input.MoveToAndClick(_navWin);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _navWin.Close();
            return TestResult.Pass;
        }

        /// <summary>
        /// RunTests: runs the tests where we test the Find functionality.
        /// </summary>
        /// <returns>TestResult</returns>
        TestResult RunTest()
        {
            _beforeSelection = new TextRange(GetSelectionFromViewer().Start, GetSelectionFromViewer().End);
            
            bool testPerformed = true;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "dragmousetoselectcontent":
                    DragMouseToSelectContent();
                    break;
                case "selectandnavigate":
                    DragMouseToSelectContent();
                    break;
                case "usekeyboardtoselectcontent":
                    _fe.Focus();
                    if (Keyboard.IsKeyToggled(Key.NumLock))
                    {
                        Status("Disabling NumLock...");
                        const ushort VK_NUMLOCK = 0x90;
                        const ushort BSCAN = 0x45;
                        const uint KEYEVENTF_EXTENDEDKEY = 0x1;
                        const uint KEYEVENTF_KEYUP = 0x2;

                        keybd_event(VK_NUMLOCK, BSCAN, KEYEVENTF_EXTENDEDKEY, IntPtr.Zero);
                        keybd_event(VK_NUMLOCK, BSCAN, KEYEVENTF_KEYUP, IntPtr.Zero);
                    }
                    UserInput.MouseLeftDown(_fe, 5, 5);
                    UserInput.MouseLeftUp(_fe, 5, 5);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    WaitFor(500);
                    MakeSelection();
                    break;
                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    testPerformed = false;
                    break;
            }

            if (testPerformed)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }

        private void DragMouseToSelectContent()
        {            
            _fe.Focus();
            UserInput.MouseButton(_fe, 3, 3, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(500);
            MouseLeftDown();
        }

        private void MouseLeftDown()
        {
            Status("MouseLeftButtonDown at the top left corner of FlowDocumentPageViewer");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);

            //Move mouse till the bottom right of the control.
            UserInput.MouseButton(_fe, 800, 600, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(500);
            MouseLeftUp();
        }

        private void MouseLeftUp()
        {
            
            Status("Move mouse in to see highlight");
            UserInput.MouseButton(_fe, 700, 500, "Move");
            Status("MouseLeftButtonUp at bottom right corner of the control");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
        }

        private void MakeSelection()
        {
            Status("Pressing shift key...");
            InputW.PressShift();
            for (int i = 0; i < 100; i++)
            {
                InputW.KeyboardType("+{RIGHT}");
            }
            Status("Releasing shift key...");
            InputW.ReleaseShift();
            //process input queue
            WaitForPriority(DispatcherPriority.SystemIdle);
            WaitFor(500);
        }
        
        /// <summary>
        /// VerifyTest: after running the test this verifies the test.
        /// </summary>
        /// <returns>TestResult</returns>        
        TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TextSelection selection = GetSelectionFromViewer();
            _afterSelection = new TextRange(selection.Start, selection.End);
            bool testVerified = true;
            switch (_inputString.ToLower(CultureInfo.InvariantCulture))
            {
                case "dragmousetoselectcontent":
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    TestVerification();
                    break;
                case "selectandnavigate":                    
                    if (0 == String.Compare(_beforeSelection.Text.Trim(), _afterSelection.Text.Trim(), StringComparison.InvariantCulture))
                    {
                        LogComment("Test case failed");
                        Log.Result = TestResult.Fail;
                    }
                    for (int i = 0; i < 3; i++)
                    {
                        NavigationCommands.NextPage.Execute(null, _fe);
                    }

                    //get selection after navigation; selection should not change
                    selection = GetSelectionFromViewer();
                            
                    TextRange afterNavigation = new TextRange(selection.Start, selection.End);
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    if (0 == String.Compare(afterNavigation.Text.Trim(), _afterSelection.Text.Trim(), StringComparison.InvariantCulture))
                    {
                        LogComment("Test case Passed. ");
                        Log.Result = TestResult.Pass;
                    }
                    else
                    {
                        LogComment("Test case failed");
                        Log.LogEvidence("Text After Selection: " + _afterSelection.Text);
                        Log.LogEvidence("Text After Navigation: " + afterNavigation.Text);
                        Log.Result = TestResult.Fail;
                    }
                    break;
                case "usekeyboardtoselectcontent":
                    WaitForPriority(DispatcherPriority.ApplicationIdle);
                    TestVerification();
                    break;
                default:
                    Status("Error !!! VerifyTestCases: Unexpected failure to match the argument. ");
                    testVerified = false;
                    break;
            }

            if (testVerified)
            {
                if (Log.Result == TestResult.Pass)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            else
            {
                return Log.Result = TestResult.Fail;
            }            
        }

        private TextSelection GetSelectionFromViewer()
        {
            TextSelection selection = null;
            switch (_viewerType)
            {
                case 0:
                    selection = ((FlowDocumentPageViewer)_fe).Selection;
                    break;
                case 1:
                    selection = ((FlowDocumentScrollViewer)_fe).Selection;
                    break;
                case 2:
                    selection = ((FlowDocumentReader)_fe).Selection;
                    break;
            }
            return selection;
        }

        private void TestVerification()
        {            
            if (0 != String.CompareOrdinal(_beforeSelection.Text.Trim(), _afterSelection.Text.Trim()))
            {
                LogComment("TestCase Passed. ");
                Log.Result = TestResult.Pass; 
            }
            else
            {
                LogComment("Test case failed");
                Log.LogEvidence("Text Before Selection: " + _beforeSelection.Text);
                Log.LogEvidence("Text After Selection: " + _afterSelection.Text);
                Log.Result = TestResult.Fail; 
            }
        }
        #endregion
    }
}

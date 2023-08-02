// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Globalization;
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
    [Test(0, "Viewer.FlowDocumentPageViewer", "FDPVSelectionTest", MethodName = "Run")]
    public class FDPVSelectionTest : AvalonTest
    {
        private FlowDocumentPageViewer _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private TextRange _beforeSelection;
        private TextRange _afterSelection;
        private FrameworkElement _fe;

        [DllImport("user32.dll")]
        public static extern void keybd_event(ushort bVk, ushort bScan, uint dwFlags, IntPtr dwExtraInfo);

        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent")]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate")]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent")]
        public FDPVSelectionTest(string xamlFile, string testValue)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentPageViewer and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            _navWin = new NavigationWindow();

            Status("Initialize");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
            _viewer = new FlowDocumentPageViewer();
            _viewer.Document = _fd;
            _navWin.Content = _viewer;

            _fe = _viewer;
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
        private TestResult RunTest()
        {
            TextSelection selection = TextEditorW.GetTextSelection(_viewer);
            _beforeSelection = new TextRange(selection.Start, selection.End);

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
                    MakeSelection();
                    break;
                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private void DragMouseToSelectContent()
        {
            _fe.Focus();
            UserInput.MouseButton(_fe, 3, 3, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(300);
            MouseLeftDown();
        }

        private void MouseLeftDown()
        {
            Status("MouseLeftButtonDown at the top left corner of FlowDocumentPageViewer");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);

            //Move mouse till the bottom right of the control.
            UserInput.MouseButton(_fe, 800, 600, "Move");
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            WaitFor(300);
            MouseLeftUp();
        }

        private void MouseLeftUp()
        {
            Status("Move mouse in to see highlight");
            UserInput.MouseButton(_fe, 700, 500, "Move");
            Status("MouseLeftButtonUp at bottom right corner of the control");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);
            WaitFor(300);
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
            WaitFor(300);
        }

        /// <summary>
        /// VerifyTest: after running the test this verifies the test.
        /// </summary>
        /// <returns>TestResult</returns>        
        private TestResult VerifyTest()
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            TextSelection selection = TextEditorW.GetTextSelection(_viewer);
            _afterSelection = new TextRange(selection.Start, selection.End);

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
                        NavigationCommands.NextPage.Execute(null, _viewer);
                    }
                    //get selection after navigation; selection should not change
                    selection = TextEditorW.GetTextSelection(_viewer);
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
                    return TestResult.Fail;
            }

            return Log.Result = TestResult.Pass;
        }

        public void TestVerification()
        {
            if (0 != String.Compare(_beforeSelection.Text.Trim(), _afterSelection.Text.Trim(), StringComparison.InvariantCulture))
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
    }

}

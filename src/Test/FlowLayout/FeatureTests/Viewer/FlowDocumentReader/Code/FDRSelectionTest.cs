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
    /// Testing Selection feature in FlowDocumentReader.    
    /// </summary>
    [Test(0, "Viewer.FlowDocumentReader", "SelectionTest", MethodName = "Run")]
    public class SelectionTest : AvalonTest
    {
        private TextRange _beforeSelection;
        private TextRange _afterSelection;
        private FlowDocumentReader _viewer;
        private FlowDocument _fd;
        private NavigationWindow _navWin;
        private string _inputXaml;
        private string _inputString = "";
        private string _viewingMode = "";
        private FrameworkElement _fe;
        [DllImport("user32.dll")]
        public static extern void keybd_event(ushort bVk, ushort bScan, uint dwFlags, IntPtr dwExtraInfo);

        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", "Page")]
        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", "TwoPage")]
        [Variation("FlowDocumentSample1x.xaml", "DragMouseToSelectContent", "Scroll")]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", "Page")]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", "TwoPage")]
        [Variation("FlowDocumentSample1x.xaml", "SelectAndNavigate", "Scroll")]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", "Page")]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", "TwoPage")]
        [Variation("FlowDocumentSample1x.xaml", "UseKeyBoardToSelectContent", "Scroll")]
        public SelectionTest(string xamlFile, string testValue, string testViewMode)
            : base()
        {
            _inputXaml = xamlFile;
            _inputString = testValue;
            _viewingMode = testViewMode;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Initialize: Get the FlowDocument and add it to the FlowDocumentReader and then set the FlowDocumentViewer viewing mode.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            //Hide the parent window, so it won't overlay the Animation window.            
            _navWin = new NavigationWindow();

            Status("Initialize");
            _fd = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
            _viewer = new FlowDocumentReader();
            _viewer.Document = _fd;
            _navWin.Content = _viewer;
            _navWin.Topmost = true;

            Status("Setting the FlowDocumentReader viewmode");

            switch (_viewingMode.ToLower(CultureInfo.InvariantCulture))
            {
                case "page":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.Page;
                    break;
                case "twopage":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.TwoPage;
                    break;
                case "scroll":
                    _viewer.ViewingMode = FlowDocumentReaderViewingMode.Scroll;
                    break;
                default:
                    Status("Error !!! SettingViewMode: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }
            _fe = _viewer;
            _navWin.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

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
            TextSelection selection = GetSelectionFromReader(_viewer);
            if (selection != null)
            {
                _beforeSelection = new TextRange(selection.Start, selection.End);
            }
            else
            {
                _beforeSelection = new TextRange(_viewer.Document.ContentStart, _viewer.Document.ContentStart);
            }

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
            Status("MouseLeftButtonDown at the top left corner of FlowDocumentReader");
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
            TextSelection selection = GetSelectionFromReader(_viewer);

            if (selection == null)
            {
                Status("Expected selection did not take place");
                return TestResult.Fail;
            }
            else
            {
                _afterSelection = new TextRange(selection.Start, selection.End);

                switch (_inputString.ToLower(CultureInfo.InvariantCulture))
                {
                    case "dragmousetoselectcontent":
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                        Verification();
                        break;
                    case "selectandnavigate":
                        for (int i = 0; i < 3; i++)
                        {
                            NavigationCommands.NextPage.Execute(null, _viewer);
                        }
                        WaitForPriority(DispatcherPriority.ApplicationIdle);
                        //Get selection again and make sure it has not changed after navigation
                        selection = GetSelectionFromReader(_viewer);
                        TextRange afterNavigation = new TextRange(selection.Start, selection.End);
                        if (0 == string.Compare(_afterSelection.Text.Trim(), afterNavigation.Text.Trim(), StringComparison.InvariantCulture))
                        {
                            LogComment("TestCase Passed. ");
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
                        Verification();
                        break;
                    default:
                        Status("Error !!! VerifyTestCases: Unexpected failure to match the argument. ");
                        return TestResult.Fail;
                }
            }

            return TestResult.Pass;
        }

        private void Verification()
        {
            if (0 != string.Compare(_beforeSelection.Text.Trim(), _afterSelection.Text.Trim(), StringComparison.InvariantCulture))
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

        private TextSelection GetSelectionFromReader(FlowDocumentReader reader)
        {
            Type viewerType = typeof(FlowDocumentReader).Assembly.GetType("MS.Internal.Documents.ReaderPageViewer", true);
            FrameworkElement viewer = LayoutUtility.GetChildFromVisualTree(reader, viewerType) as FrameworkElement;
            if (viewer == null)
            {
                viewerType = typeof(FlowDocumentReader).Assembly.GetType("MS.Internal.Documents.ReaderTwoPageViewer", true);
                viewer = LayoutUtility.GetChildFromVisualTree(reader, viewerType) as FrameworkElement;
                if (viewer == null)
                {
                    viewerType = typeof(FlowDocumentReader).Assembly.GetType("MS.Internal.Documents.ReaderScrollViewer", true);
                    viewer = LayoutUtility.GetChildFromVisualTree(reader, viewerType) as FrameworkElement;
                }
            }

            if (viewer != null)
            {
                return TextEditorW.GetTextSelection(viewer);
            }
            else
            {
                LogComment("Cannot obtain selection state because cannot find Page, Two Page or Scroll viewer embedded inside reader");
                LogComment("Trying to obtain selection state directly from reader");
                return TextEditorW.GetTextSelection(reader);
            }
        }
    }
}
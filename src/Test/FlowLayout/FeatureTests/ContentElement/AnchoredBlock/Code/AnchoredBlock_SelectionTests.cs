// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/////////////////////////////////////////////////////////////////////////////////////////////////////////////
// Description : Test Selection in Floaters and Figures..  
//               
//               This loads a xaml with Floaters and Figures that have selectable content (Paragraph)
//               Tests are then run to verify that selection works as expected within and through the AnchoredBlock.
//                                                
// Verification: Basic API validation.  
// Created by:  Microsoft
////////////////////////////////////////////////////////////////////////////////////////////////////////////// 

using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Threading;
using System.Windows.Markup;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Layout;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;

namespace Microsoft.Test.FlowLayout
{
    /// <summary>   
    /// Test selection in anchored block.   
    /// </summary>
    [Test(0, "AnchoredBlock", "AnchoredBlockSelectionTest", MethodName = "Run")]
    public class AnchoredBlocksSelectionTest : AvalonTest
    {
        private FlowDocumentPageViewer _singlePageViewer;
        private FlowDocument _document;
        private Window _w;
        private string _inputXaml;
        private int _inputID;
        private string _expectedSelectionText;
        private TextSelection _ts;
        private TextRange _tr;

        [Variation("AnchoredBlock_SelectionTestContent.xaml", 1)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 2, Keywords = "MicroSuite")]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 3, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 4)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 5, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 6)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 7, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 8, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 9, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 10, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 11, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 12, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 13, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 14, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 15, Priority = 2)]
        [Variation("AnchoredBlock_SelectionTestContent.xaml", 16, Priority = 2)]
        public AnchoredBlocksSelectionTest(string xamlFile, int testID)
            : base()
        {
            _inputXaml = xamlFile;
            _inputID = testID;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTests);
            RunSteps += new TestStep(VerifyTest);
        }

        /// <summary>
        /// Initialize: Initialize the content
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult Initialize()
        {
            Status("Initialize");
            _w = new Window();
            _singlePageViewer = new FlowDocumentPageViewer();
            _document = (FlowDocument)XamlReader.Load(File.OpenRead(_inputXaml));
            _singlePageViewer.Document = _document;

            ((DynamicDocumentPaginator)(_singlePageViewer.Document.DocumentPaginator)).PaginationCompleted += new EventHandler(paginator_PaginationCompleted);
            _w.Content = _singlePageViewer;
            if (_w.Content is FrameworkElement)
            {
                // Setting Window.Content size to ensure same size of root element over all themes.  
                // Different themes have different sized window chrome which will cause property dump 
                // and vscan failures even though the rest of the content is the same.
                // 784x564 is the content size of a 800x600 window in Aero theme.
                ((FrameworkElement)_w.Content).Height = 564;
                ((FrameworkElement)_w.Content).Width = 784;
            }
            _w.Left = 0;
            _w.Top = 0;
            _w.Topmost = true;
            _w.Resources.MergedDictionaries.Add(GenericStyles.LoadAllStyles());
            _w.SizeToContent = SizeToContent.WidthAndHeight;
            _w.Show();
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w.Close();
            return TestResult.Pass;
        }

        private void paginator_PaginationCompleted(object sender, EventArgs e)
        {
            if (_singlePageViewer.Document.DocumentPaginator.IsPageCountValid)
            {
                _singlePageViewer.NextPage(); //Get to the second page              
            }
            else
            {
                LogComment("**PageCount was invalid after Pagination completed.");
            }
        }

        /// <summary>
        /// RunTests: Runs a set of tests based on the input variation.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RunTests()
        {
            _singlePageViewer.Focus();

            _ts = TextEditorW.GetTextSelection(_singlePageViewer);
            _tr = (TextRange)_ts;

            Figure fig = LogicalTreeHelper.FindLogicalNode(_document, "testFigure") as Figure;
            Floater flt = LogicalTreeHelper.FindLogicalNode(_document, "testFloater") as Floater;

            switch (_inputID)
            {
                case 1:
                    Status("Running: Programatically select text content of a Floater test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Floater.";
                    ProgSelectAnchoredBlock(flt);
                    break;

                case 2:
                    Status("Running: Programatically select text content of a Figure test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Figure.";
                    ProgSelectAnchoredBlock(fig);
                    break;

                case 3:
                    Status("Running: Drag Mouse to select text content of a Floater test...");
                    _expectedSelectionText = "Paragraph content contained";
                    DragMouseToSelectAnchoredBlock(60, 110, 75, 153);
                    break;

                case 4:
                    Status("Running: Drag Mouse to select text content of a Figure test...");
                    _expectedSelectionText = "contained in a Figure";
                    DragMouseToSelectAnchoredBlock(600, 300, 620, 340);
                    break;

                case 5:
                    Status("Running: Use Keyboard to select text content of a Floater test...");
                    _expectedSelectionText = "aragra";
                    UseKeyBoardToSelectAnchoredBlock(60, 110, 6, "RIGHT");
                    break;

                case 6:
                    Status("Running: Use Keyboard to select text content of a Figure test...");
                    _expectedSelectionText = "aine";
                    UseKeyBoardToSelectAnchoredBlock(660, 300, 4, "LEFT");
                    break;

                case 7:
                    Status("Running: Drag Mouse to select content inside of a Floater to outside of a Floater test...");
                    _expectedSelectionText = "a Paragraph outside the Floater \r\nThis is Paragraph content contained in a Floater.";
                    DragMouseToSelectAnchoredBlock(60, 110, 80, 30);
                    break;

                case 8:
                    Status("Running: Drag Mouse to select content inside of a Figure to outside of a Figure test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Figure.\r\nThis is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the";
                    DragMouseToSelectAnchoredBlock(600, 300, 620, 380);
                    break;

                case 9:
                    Status("Running: Drag Mouse to select content outside of a Floater to inside of a Floater test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Floater.\r\nThis is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored";
                    DragMouseToSelectAnchoredBlock(90, 260, 80, 160);
                    break;

                case 10:
                    Status("Running: Drag Mouse to select content inside of a Figure to outside of a Figure test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Figure.\r\nThis is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored";
                    DragMouseToSelectAnchoredBlock(630, 240, 650, 160);
                    break;

                case 11:
                    Status("Running: Drag Mouse to select content through a Floater test...");
                    _expectedSelectionText = "a Paragraph outside the Floater \r\nThis is Paragraph content contained in a Floater.\r\nThis is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored";
                    DragMouseToSelectAnchoredBlock(80, 20, 90, 260);
                    break;

                case 12:
                    Status("Running: Drag Mouse to select content through a Figure test...");
                    _expectedSelectionText = "Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content surrounding the Anchored Blocks. This is content";
                    DragMouseToSelectAnchoredBlock(620, 390, 620, 120);
                    break;

                case 13:
                    Status("Running: Use Keyboard (RIGHT Arrow) to select content inside of a Floater to outside of a Floater test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Floater.\r\nThis i";
                    UseKeyBoardToSelectAnchoredBlock(60, 190, 14, "RIGHT");
                    break;

                case 14:
                    Status("Running: Use Keyboard (LEFT Arrow) to select content inside of a Figure to outside of a Figure test...");
                    _expectedSelectionText = "ure \r\nThis is Paragraph content contained in a Figure.";
                    UseKeyBoardToSelectAnchoredBlock(660, 200, 9, "LEFT");
                    break;

                case 15:
                    Status("Running: Use Keyboard (DOWN Arrow) to select content inside of a Floater to outside of a Floater test...");
                    _expectedSelectionText = "This is Paragraph content contained in a Floater.\r\nThis is content";
                    UseKeyBoardToSelectAnchoredBlock(60, 190, 3, "DOWN");
                    break;

                case 16:
                    Status("Running: Use Keyboard (UP Arrow) to select content inside of a Figure to outside of a Figure test...");
                    _expectedSelectionText = "tent surrounding the Anchored Blocks.\r\nThis is a Paragraph outside the Figure \r\nThis is Paragraph content contained in a Figure.";
                    UseKeyBoardToSelectAnchoredBlock(660, 200, 3, "UP");
                    break;
                default:
                    Status("Error !!! SettingTestCases: Unexpected failure to match the argument. ");
                    return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        /// <summary>
        /// VerifyTest: Verifies the test.
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult VerifyTest()
        {
            WaitFor(1000);
            if (String.Compare(_tr.Text.Trim(), _expectedSelectionText.Trim(), StringComparison.InvariantCulture) != 0)
            {
                LogComment("Selected text is wrong!");
                LogComment("Expected: " + _expectedSelectionText);
                LogComment("Instead got: " + _tr.Text.Trim());
                return TestResult.Fail;
            }
            else
            {
                LogComment("Selected text was as expected.");
                return TestResult.Pass;
            }
        }

        private void ProgSelectAnchoredBlock(AnchoredBlock block)
        {
            TextPointer tp1 = block.Blocks.FirstBlock.ContentStart;
            TextPointer tp2 = block.Blocks.FirstBlock.ContentEnd;
            _ts.Select(tp1, tp2);
            Status("Selected text: " + _tr.Text);
        }

        private void DragMouseToSelectAnchoredBlock(int startX, int startY, int endX, int endY)
        {
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Moving mouse to the start of the Selection...");
            UserInput.MouseButton(_singlePageViewer, startX, startY, "Move");

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("MouseLeftButtonDown...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Move Mouse within the Anchored Block...");
            UserInput.MouseButton(_singlePageViewer, endX, endY, "Move");

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("MouseLeftButtonUp...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Selected text: " + _tr.Text);
        }

        [DllImport("user32.dll")]
        private static extern void keybd_event(ushort bVk, ushort bScan, uint dwFlags, IntPtr dwExtraInfo);

        private void UseKeyBoardToSelectAnchoredBlock(int startX, int startY, int count, string direction)
        {
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
            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Moving mouse to the start of the Selection...");
            UserInput.MouseButton(_singlePageViewer, startX, startY, "Move");

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("MouseLeftButtonDown...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftDown);

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("MouseLeftButtonUp...");
            Microsoft.Test.Input.Input.SendMouseInput(0, 0, 0, SendMouseInputFlags.LeftUp);

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Pressing shift key...");
            InputW.PressShift();

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Pressing the " + direction + " arrow key " + count.ToString() + " times...");
            for (int i = 0; i < count; i++)
            {
                Status("Pressing " + direction + "...");
                InputW.KeyboardType("+{" + direction + "}");
            }

            WaitForPriority(DispatcherPriority.ApplicationIdle);
            Status("Releasing shift key...");
            InputW.ReleaseShift();
        }
    }
}

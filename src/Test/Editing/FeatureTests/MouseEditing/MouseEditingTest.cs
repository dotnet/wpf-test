// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Tests the TextBox/RichTextBox mouse editing interaction.

using System;
using System.IO;
using SysDrawing = System.Drawing;

using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Display;
using Microsoft.Test.Imaging;
using Microsoft.Test.Input;

using DataTransfer;
using Test.Uis.Data;
using Test.Uis.Loggers;
using Test.Uis.Management;
using Test.Uis.TestTypes;
using Test.Uis.TextEditing;
using Test.Uis.Utils;
using Test.Uis.Wrappers;

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 1 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/MouseEditing/MouseEditingTest.cs $")]

namespace Test.Uis.TextEditing
{
    /// <summary>Tests the TextBox/RichTextBox mouse click actions.</summary>
    [Test(0, "Editor", "MouseClickTest1", MethodParameters = "/TestCaseType:MouseClickTest /Priority:0")]
    [Test(0, "Editor", "MouseClickTest2", MethodParameters = "/TestCaseType:MouseClickTest /EditableBox1:TextBox /Priority:0")]
    [Test(2, "Editor", "MouseClickTest3", MethodParameters = "/TestCaseType:MouseClickTest /Priority:1")]
    [TestOwner("Microsoft"), TestTitle("MouseClickTest"), TestTactics("409,410,419"), TestWorkItem("57"), TestBugs("91,92,467")]
    public class MouseClickTest : TestContainer
    {
        private UIElementWrapper _editBox1;              // Editable box1
        private TestCaseData _testCaseData;
        private int _testCaseIndex = 0;
        private int[] _testFailed;                       // Array of all test that failed
        private bool _isTestFailed;                      // To remember if test failed
        private string _failID;                                  // Name of test case that failed
        private int _endIndex;                           // End of test case index
        private string[] _xamlContentArray ={
            // Simple content for mouse click boundary on character (can be run in RichTextBox as well).
            "ABC",

            // 3 lines content for mouse click in front/end/btw 2 lines (can be run in RichTextBox as well).
            "ABC DEF.\r\n123 456 789 0.\r\nabc def ghi.",

            // Regression_Bug92 - click on an empty line
            "ABC DEF.\r\n\r\n\r\nabc def ghi.",

            // Regression_Bug91 (RichTextBox only) - Click beyond the text when an embedded object is at the end
            "ABC DEF.\r\n\r\n\r\nabc def ghi.<Button>Button</Button>",
        };

        struct TestCaseData
        {
            public string TestType;     // Type of test case. For now I only have mouse single click
            public int XamlString;      // Xaml string to be display
            public bool NewXaml;        // If NexXaml is true then load another specified xaml
            public int CharacterIndex;  // Character index in your content
            public CharacterPosition ClickLocation; // Location to mouse click on
            public string ExpectedStringOnLeft;     // Expect string on left of caret
            public string ExpectedStringOnRight;    // Expect string on right of caret
            public int Priority;        // Test case priority. By default is BVT (0)

            public TestCaseData(string testType, int xamlString, bool newXaml, int characterIndex, CharacterPosition clickLocation,
                string expectedStringOnLeft, string expectedStringOnRight, int priority)
            {
                this.TestType = testType;
                this.XamlString = xamlString;
                this.NewXaml = newXaml;
                this.CharacterIndex = characterIndex;
                this.ClickLocation = clickLocation;
                this.ExpectedStringOnLeft = expectedStringOnLeft;
                this.ExpectedStringOnRight = expectedStringOnRight;
                this.Priority = priority;
            }

            public static TestCaseData[] Cases = new TestCaseData[] {
                // Click on 11 position withing a character boundary
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.Center, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.CenterToLowerLeft, "A", "B",0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.CenterToUpperRight, "B", "C", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.LowerLeft, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.LowerMiddle, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.LowerRight, "B", "C", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.MiddleLeft, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.MiddleRight, "B", "C", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.UpperLeft, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.UpperMiddle, "A", "B", 0),
                new TestCaseData("MouseClick", 0, false, 1, CharacterPosition.UpperRight, "B", "C", 0),

                // Click in front of a line or paragraph
                new TestCaseData("MouseClick", 0, false, 0, CharacterPosition.UpperLeft, "", "A", 0),

                // Text content consists of 3 lines.
                //    a. At the end of the first line and on empty space far right of that line.
                new TestCaseData("MouseClick", 1, true, 8, CharacterPosition.MiddleRight, ".", "", 0),
                new TestCaseData("MouseClick", 1, false, 8, CharacterPosition.FarRightEndOfALine, ".", "", 0),
                //    b. Between boundary of 1st and 2nd line (the boundary point, above boundary point, below boundary point)
                new TestCaseData("MouseClick", 1, false, 6, CharacterPosition.LowerMiddle, "E", "F", 0),
                new TestCaseData("MouseClick", 1, false, 11, CharacterPosition.UpperMiddle, "2", "3", 0),
                //    c. At the end of the 2rd line and on empty space far right of that line.
                new TestCaseData("MouseClick", 1, false, 23, CharacterPosition.LowerRight, ".", "", 0),
                new TestCaseData("MouseClick", 1, false, 11, CharacterPosition.FarRightEndOfALine, ".", "", 0),
                //    d. At the end of the 3rd line and on empty space far right of that line.
                new TestCaseData("MouseClick", 1, false, 36, CharacterPosition.CenterToUpperRight, ".", "", 0),
                new TestCaseData("MouseClick", 1, true, 36, CharacterPosition.FarRightEndOfALine, ".", "", 0),
                //    e. Between boundary of 2nd and 3rd line.
                new TestCaseData("MouseClick", 1, false, 9, CharacterPosition.LowerMiddle, "1", "2", 0),
                new TestCaseData("MouseClick", 1, false, 34, CharacterPosition.UpperMiddle, "h", "i", 0),
                //    f. On white space at the bottom right of editbox.
                new TestCaseData("MouseClick", 1, false, 34, CharacterPosition.LowerRightOfControl, ".", "", 0),
                //    g. On white space at the bottom of editbox but under text.
                new TestCaseData("MouseClick", 1, false, 34, CharacterPosition.FarBelowText, "h", "i", 0),
                //    h. Infront of 2nd line.
                new TestCaseData("MouseClick", 1, false, 10 , CharacterPosition.FarLeftInfrontOfALine, "", "1", 0),
                //    i. Infront of 3rd line.
                new TestCaseData("MouseClick", 1, false, 25, CharacterPosition.FarLeftInfrontOfALine, "", "a", 0),

                #region Bugs regression
                // Click on empty line to set caret.
                new TestCaseData("MouseClick", 2, true, 10, CharacterPosition.Center, "", "", 1),   // Regression_Bug92
                // Click beyond the text when an embedded object is at the end
                new TestCaseData("MouseClick", 3, true, 20, CharacterPosition.FarRightEndOfALine, " ", "", 1), // Regression_Bug91, TC: 419
                #endregion Bugs regression
            };
        }

        /// <summary>Override RunTestCase</summary>
        public override void RunTestCase()
        {
            BuildWindow(500, 800, 24);  // editBox1 Height is 500 and Width is 800
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _endIndex = TestCaseData.Cases.Length;
            _testCaseData = TestCaseData.Cases[_testCaseIndex];
            _testFailed = new int[TestCaseData.Cases.Length];
            _isTestFailed = false;
            for (int i = 0; i < _testFailed.Length; i++)
                _testFailed[i] = 0;
            if (Settings.GetArgumentAsInt("CaseID") != 0)
            {
                _testCaseIndex = Settings.GetArgumentAsInt("CaseID");
                _endIndex = _testCaseIndex + 1;
            }
            QueueDelegate(LoadXaml);
        }

        private void LoadXaml()
        {
            // Default is loading the first xaml.
            _editBox1.XamlText = _xamlContentArray[_testCaseData.XamlString];
            RunCase();
        }

        private void RunCase()
        {
            if (_testCaseIndex < _endIndex)
            {
                _testCaseData = TestCaseData.Cases[_testCaseIndex];
                if (Settings.GetArgumentAsInt("Priority") == _testCaseData.Priority)
                    QueueDelegate(StartTest);
                else
                {
                    _testCaseIndex++;
                    QueueDelegate(RunCase);
                }
            }
            else
            {
                if (_isTestFailed)
                {
                    // Log case id that failed.
                    for (int i = 0; i < _testFailed.Length; i++)
                    {
                        if (_testFailed[i] == -1)
                            _failID += " " + i + ",";
                    }
                    Log("The following test cases have failed: [" + _failID + "]" +
                        " To re-run append /CaseID:<test case number> /LoadXaml:true.");
                    Logger.Current.ReportResult(false, "At lease one of test has failed.", false);
                }
                else
                    Logger.Current.ReportSuccess();
            }
        }

        private void StartTest()
        {
            _testCaseData = TestCaseData.Cases[_testCaseIndex];
            Log("*******Running new test case:[" + _testCaseIndex + "]*******");
            Log("Test Type :[" + _testCaseData.TestType + "]");
            Log("Test EditableBox1 is:[" + EditableBox1 + "]");

            // Load new xaml if NewXaml specify true in TestCaseData or LoadXaml is true in command line.
            if (_testCaseData.NewXaml || Settings.GetArgumentAsBool("LoadXaml"))
            {
                _editBox1.XamlText = _xamlContentArray[_testCaseData.XamlString];
            }

            QueueDelegate(TestAction);
        }

        private void TestAction()
        {
            Log("Text    : [" + _xamlContentArray[_testCaseData.XamlString] + "]");
            Log("Character index: [" + _testCaseData.CharacterIndex + "]");
            Log("Click location : [" + _testCaseData.ClickLocation + "]");

            ClickOnCharacter(_editBox1, _testCaseData.CharacterIndex, _testCaseData.ClickLocation);
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(VerifyResult));
        }

        private void VerifyResult()
        {
            VerifyTextOnCaretLeft(_testCaseData.ExpectedStringOnLeft);
            VerifyTextOnCaretRight(_testCaseData.ExpectedStringOnRight);
            // Run next test case
            _testCaseIndex++;
            RunCase();
        }

        private bool VerifyTextOnCaretLeft(string ExpectedString)
        {
            // Get string on left of caret
            string strOnLeft = _editBox1.GetTextOutsideSelection(LogicalDirection.Backward);

            // If string is not empty or null, get the last character in the string
            if (!String.IsNullOrEmpty(strOnLeft))
            {
                strOnLeft = strOnLeft.Substring(strOnLeft.Length - 1);
                // If the character is \r\n or \n set to empty
                if (strOnLeft == "\r\n" || strOnLeft == "\n")
                {
                    strOnLeft = "";
                }
            }

            string message = String.Format("String on caret left [{0}], expected [{1}]", strOnLeft, ExpectedString);

            if (strOnLeft != ExpectedString)
            {
                Logger.Current.ReportResult(false, message, true);
                _testFailed[_testCaseIndex] = -1;
                _isTestFailed = true;
            }
            return true;
        }

        private bool VerifyTextOnCaretRight(string ExpectedString)
        {
            // Get string on right of caret
            string strOnRight = _editBox1.GetTextOutsideSelection(LogicalDirection.Forward);

            // If string length is 2 or more  and string doesn't contain "\r\n" (for RTB), nor "\n" (for TB),
            // get the first character in the string.
            if (strOnRight.Length > 1 && !(strOnRight.IndexOf("\r\n") == 0 || strOnRight.IndexOf("\n") == 0))
            {
                strOnRight = strOnRight.Remove(1);
            }
            // If string is not empty or null and string contain "\r\n" or "\n", set string to empty.
            else if (!String.IsNullOrEmpty(strOnRight) && (strOnRight.IndexOf("\r\n") == 0 || strOnRight.IndexOf("\n") == 0))
            {
                strOnRight = "";
            }
            // else return strOnRight.

            string message = String.Format("String on caret right [{0}], expected [{1}]", strOnRight, ExpectedString);

            if (strOnRight != ExpectedString)
            {
                Logger.Current.ReportResult(false, message, true);
                _testFailed[_testCaseIndex] = -1;
                _isTestFailed = true;
            }
            return true;
        }

        #region Finding Character Position

        /// <summary>Enum for mouse click position in a character</summary>
        internal enum CharacterPosition
        {
            /// <summary>Upperleft of a character.</summary>
            UpperLeft,

            /// <summary>Middleleft of a character.</summary>
            MiddleLeft,

            /// <summary>Lowerleft of a character.</summary>
            LowerLeft,

            /// <summary>Uppermiddle of a character.</summary>
            UpperMiddle,

            /// <summary>Center of a character.</summary>
            Center,

            /// <summary>Lowermiddle of a character.</summary>
            LowerMiddle,

            /// <summary>Upperright of a character.</summary>
            UpperRight,

            /// <summary>Middleright of a character.</summary>
            MiddleRight,

            /// <summary>Lowerright of a character.</summary>
            LowerRight,

            /// <summary>Center to top right of a character (center of quardrant I).</summary>
            CenterToUpperRight,

            /// <summary>Center to bottom left of a character (center of quardrant III).</summary>
            CenterToLowerLeft,

            /// <summary>
            /// Empty white space and far right of a line.
            /// x position is TextBox or RichTextBox's width -2.
            /// Caret should insert at the end of that line.
            /// </summary>
            FarRightEndOfALine,

            /// <summary>
            /// Empty white space and bottom right of TextBox or RichTextBox.
            /// x and y position are TextBox or RichTextBox's width -2 and height-2.
            /// Caret should insert at the end of last line of text.
            /// </summary>
            LowerRightOfControl,

            /// <summary>
            /// On empty white space and far below text.
            /// y position is TextBox or RichTextBox's height -2.
            /// Caret should insert into text of last line.
            /// </summary>
            FarBelowText,

            /// <summary>
            /// Empty white space and far left of a line.
            /// x position is TextBox or RichTextBox's (width - (width-10))
            /// Caret should insert at the beining of that line.
            /// </summary>
            FarLeftInfrontOfALine,
        };

        /// <summary>Mouse click on character posoition.</summary>
        /// <param name="wrapper">UIElementWrapper to be clicked in</param>
        /// <param name="index">Character index in TextBox/RichTextBox</param>
        /// <param name="position">See CharacterPosition enum</param>
        internal void ClickOnCharacter(Test.Uis.Wrappers.UIElementWrapper wrapper, int index, CharacterPosition position)
        {
            Point p = FindCharacterPosition(wrapper, index, position);
            Input.MoveToAndClickSafe(p);
            //MouseInput.MouseClick((int)p.X, (int)p.Y);
        }

        /// <summary>Mouse click on character posoition.</summary>
        /// <param name="wrapper">UIElementWrapper to be clicked in</param>
        /// <param name="index">Character index in TextBox/RichTextBox</param>
        /// <param name="position">See CharacterPosition enum</param>
        /// <param name="clicks">Number of clicking</param>
        internal void ClickOnCharacter(Test.Uis.Wrappers.UIElementWrapper wrapper, int index, CharacterPosition position, int clicks)
        {
            Point p = FindCharacterPosition(wrapper, index, position);
            for (int i = 0; i < clicks; i++)
            {
                MouseInput.MouseClick((int)p.X, (int)p.Y);
            }
        }

        /// <summary>Find character position</summary>
        /// <param name="wrapper">UIElementWrapper to be used</param>
        /// <param name="index">Character index in TextBox/RichTextBox</param>
        /// <param name="position">See CharacterPosition enum</param>
        /// <returns>x,y point</returns>
        internal Point FindCharacterPosition(Test.Uis.Wrappers.UIElementWrapper wrapper, int index, CharacterPosition position)
        {
            int x = 0, y = 0;
            Rect rec = wrapper.GetGlobalCharacterRect(TextPointerFromCharacterIndex(wrapper, index));
            switch (position)
            {
                case CharacterPosition.UpperLeft:
                    x = (int)(rec.Left)+ ((Monitor.Dpi.x == 120) ? 1 : 0);
                    y = (int)(rec.Top + ((Monitor.Dpi.x == 120) ? 2 : 0));
                    break;
                case CharacterPosition.MiddleLeft:
                    x = (int)(rec.Left);
                    y = (int)(rec.Top + rec.Height / 2);
                    break;
                case CharacterPosition.LowerLeft:
                    x = (int)(rec.Left);
                    y = (int)(rec.Bottom);
                    break;
                case CharacterPosition.UpperMiddle:
                    x = (int)(rec.Left + rec.Width / 2);
                    y = (int)(rec.Top) + 1;
                    break;
                case CharacterPosition.Center:
                    x = (int)(rec.Left + ((rec.Width==0)?2:rec.Width) / 2);
                    y = (int)(rec.Top + rec.Height / 2);
                    break;
                case CharacterPosition.LowerMiddle:
                    x = (int)(rec.Left + rec.Width / 2);
                    y = (int)(rec.Bottom);
                    break;
                case CharacterPosition.UpperRight:
                    x = (int)(rec.Right);
                    y = (int)(rec.Top + ((Monitor.Dpi.x == 120) ? 2 : 0));
                    break;
                case CharacterPosition.MiddleRight:
                    x = (int)(rec.Right);
                    y = (int)(rec.Top + rec.Height / 2);
                    break;
                case CharacterPosition.LowerRight:
                    x = (int)(rec.Right);
                    y = (int)(rec.Bottom);
                    break;
                case CharacterPosition.CenterToUpperRight:
                    x = (int)(rec.Left + (rec.Width / 2) + ((rec.Width / 2) / 2));
                    y = (int)(rec.Top + (rec.Height / 2) - ((rec.Height / 2) / 2));
                    break;
                case CharacterPosition.CenterToLowerLeft:
                    x = (int)(rec.Left + ((rec.Width / 2) / 2));
                    y = (int)(rec.Top + (rec.Height / 2) + ((rec.Height / 2) / 2));
                    break;
                case CharacterPosition.FarRightEndOfALine:
                    x = (int)(((FrameworkElement)wrapper.Element).Width) - 50;
                    y = (int)(rec.Top + rec.Height / 2);
                    break;
                case CharacterPosition.LowerRightOfControl:
                    x = (int)(((FrameworkElement)wrapper.Element).Width) - 50;
                    y = (int)(((FrameworkElement)wrapper.Element).Height) - 4;
                    break;
                case CharacterPosition.FarBelowText:
                    x = (int)(rec.Left + rec.Width / 2);
                    y = (int)(((FrameworkElement)wrapper.Element).Height) - 4;
                    break;
                case CharacterPosition.FarLeftInfrontOfALine:
                    x = (int)(((FrameworkElement)wrapper.Element).Width - (((FrameworkElement)wrapper.Element).Width - 12)+2) + ((Monitor.Dpi.x==120)?2:0);
                    y = (int)(rec.Top + rec.Height / 2);
                    break;
            }
            return new Point(x, y);
        }

        /// <summary>
        /// Find the insertion pointer from index.
        /// /r/n or /n count as 1
        /// ex: a/r/nb => total character 3
        /// </summary>
        /// <param name="wrapper">UIElementWrapper to be used</param>
        /// <param name="index">index number of each character</param>
        /// <returns>TextPointer of that character index</returns>
        public TextPointer TextPointerFromCharacterIndex(UIElementWrapper wrapper, int index)
        {
            TextPointer pStart = wrapper.Start;

            // If the content contains <P> or any othere element, move the insertion position to a valid position.
            while (!(pStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement ||
                    pStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text))
            {
                pStart = pStart.GetNextInsertionPosition(LogicalDirection.Forward);
            }

            // Move the insertion positon to next valid pisiton as specify by character inxex.
            while (index > 0)
            {
                if (pStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.EmbeddedElement ||
                    pStart.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    index--;
                }

                pStart = pStart.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            return pStart;
        }
        #endregion Finding Character Position
    }

    /// <summary>
    /// Verifies that the selection object reflects the expected positions
    /// after a mouse selection operation.
    /// P0 - EditingTest.exe /TestCaseType:MouseDragSelectTest /Pri=0 ~ 00:19 (TextBox, RichTextBox with selected start and end)
    /// P1 - EditingTest.exe /TestCaseType:MouseDragSelectTest /Pri=1 ~ 00:10 (RichTextBox with Table)
    /// P2 - EditingTest.exe /TestCaseType:MouseDragSelectTest /Pri=2 ~ 02:49 (TextBox, RichTextBox)
    /// </summary>
    [Test(0, "Editor", "MouseDragSelectTest1", MethodParameters = "/TestCaseType:MouseDragSelectTest /Pri=0", Timeout = 300)]
    [Test(1, "Editor", "MouseDragSelectTest2", MethodParameters = "/TestCaseType:MouseDragSelectTest /Pri=1", Timeout = 300)]
    [Test(2, "Editor", "MouseDragSelectTest3", MethodParameters = "/TestCaseType:MouseDragSelectTest /Pri=2", Timeout = 600)]
    [TestOwner("Microsoft"), TestTitle("MouseDragSelectTest"), TestTactics("411,412,413"), TestWorkItem("58,59"), TestBugs("865,866")]
    public class MouseDragSelectTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        /// <param name="values">Values to read from.</param>
        /// <returns>Whether the given combination should be accepted.</returns>
        /// <remarks>Filters out equal position-to-position combinations.</remarks>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // Filter out case with related bug for RichTextBox
            if (TextEditableType.GetByName("RichTextBox") == _editableType)
            {
                if (result)
                {
                    result = (_startPosition.DocumentPosition != _endPosition.DocumentPosition);
                }

                result = result && _endPosition.DocumentPosition != DocumentPosition.StartOfDelimitedLine;

                result = result && _endPosition.DocumentPosition != DocumentPosition.StartOfWrappedLine;
                result = result && _startPosition.DocumentPosition != DocumentPosition.StartOfWrappedLine;

                result = result && _endPosition.DocumentPosition != DocumentPosition.EndOfWhitespace;

                result = result && _startPosition.DocumentPosition != DocumentPosition.EndOfDelimitedLine;
                result = result && _endPosition.DocumentPosition != DocumentPosition.StartOfDelimitedLine;
            }
            return result;
        }

        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(TestElement);
            ((Control)(_wrapper.Element)).Background = Brushes.Lavender;
            // *** Important parameters ***
            //If you change these test case might fail due to delicate positioning of mouse movements.
            ((Control)(_wrapper.Element)).Width = 150;
            ((Control)(_wrapper.Element)).Padding = new Thickness(0);
            ((Control)(_wrapper.Element)).BorderThickness = new Thickness(0);
            ((Control)(_wrapper.Element)).Margin = new Thickness(0);
            ((Control)(_wrapper.Element)).FontFamily = new FontFamily("Tahoma");
            ((Control)(_wrapper.Element)).FontSize = 18;
            MainWindow.SizeToContent = SizeToContent.Height;
            if (_wrapper.IsElementRichText)
            {
                _wrapper.XamlText =
                    "<Paragraph>First paragraph with a<LineBreak/> Line break</Paragraph>" +
                    "<Paragraph>Second paragraph.</Paragraph>" +

                    "<Table>" +
                    "<Table.Columns>" +
                    "<TableColumn Width='40' Background='red'/>" +
                    "<TableColumn Width='40' Background='yellow'/>" +
                    "</Table.Columns>" +
                    "<TableRowGroup>" +
                    "<TableRow>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell 1</Paragraph></TableCell>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell 2</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "<TableRow>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell a</Paragraph></TableCell>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell b</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "<TableRow>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell m</Paragraph></TableCell>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell n</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "</TableRowGroup>" +
                    "</Table>" +

                    "<Paragraph>Third paragraph.</Paragraph>" +

                    "<Table>" +
                    "<Table.Columns>" +
                    "<TableColumn Width='40' Background='red'/>" +
                    "<TableColumn Width='40' Background='yellow'/>" +
                    "</Table.Columns>" +
                    "<TableRowGroup>" +
                    "<TableRow>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell x</Paragraph></TableCell>" +
                    "<TableCell  BorderThickness='1,1,1,1' BorderBrush='Black'><Paragraph>Cell y</Paragraph></TableCell>" +
                    "</TableRow>" +
                    "</TableRowGroup>" +
                    "</Table>" +

                    "<Paragraph>Fourth paragraph.</Paragraph>";
            }
            else
            {
                ((TextBox)(_wrapper.Element)).TextWrapping = TextWrapping.Wrap;
                _wrapper.Text = "This is a test1. This is a test2. This is a test3. " +
                    "This is a test4. This is a test5.\r\nNew line1\r\nNew line2 \r\n New line3";
            }
            QueueDelegate(PerformSelection);
        }

        private void PerformSelection()
        {
            _start = _startPosition.FindAny(_wrapper);

            if (_start == null)
            {
                QueueDelegate(NextCombination);
                return;
            }
            if (_startToEnd)
            {
                _end = _endPosition.FindAfter(_start);
            }
            else
            {
                _end = _endPosition.FindBefore(_start);
            }
            if (_end == null)
            {
                QueueDelegate(NextCombination);
                return;
            }

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(100), new SimpleHandler(SingleMovement));
        }

        private void SingleMovement()
        {
            Point startPoint;
            Point endPoint;

            startPoint = MouseEditingTestHelper.FindPoint(_start, _wrapper);
            endPoint = MouseEditingTestHelper.FindPoint(_end, _wrapper);

            //Not performing the drag operation in DragInOtherThread option because, if the
            //dragEndPosition and dragStartPosition are in different lines and
            //dragEndPosition.X < dragStartPosition.X, the autoword selection withdraws due to the
            //trajectory of the mouse.
            MouseInput.MouseDragPressed(startPoint, endPoint);
            QueueDelegate(VerifySelection);
            //MouseInput.MouseDragInOtherThread(startPoint, endPoint, true, new TimeSpan(0, 0, 1), new SimpleHandler(VerifySelection), Dispatcher.CurrentDispatcher);
        }

        private void VerifySelection()
        {
            TextSelection selection;    // The current selection on the control.
            TextPointer movingPointer;  // Moving pointer in selection.
            TextPointer anchorPointer;  // Anchor pointer in selection.
            TextPointer expectedStart;  // Expected start position.
            TextPointer expectedEnd;    // Expecetd end position.

            if (_wrapper.IsTextPointerInsideTextElement(_start, typeof(Table)) ||
                _wrapper.IsTextPointerInsideTextElement(_end, typeof(Table)))
            {
                QueueDelegate(VerifyTableSelection);
            }
            else
            {
                selection = _wrapper.SelectionInstance;
                movingPointer = _wrapper.SelectionMovingPointer;
                anchorPointer = (movingPointer.CompareTo(selection.Start) == 0) ? selection.End : selection.Start;

                if (_wrapper.Element is TextBox)
                {
                    // No adjustment is needed in TextBox
                    // because AutoWordSelection is disabled
                    expectedStart = _start;
                    expectedEnd = _end;
                }
                else
                {
                    // If we cross a word boundary, then we need
                    // to extend the first edge towards the beginning
                    // and the last edge towards the end, to word
                    // boundaries.
                    if (ShouldExtendPointers(selection.Text))
                    {
                        if (_start.CompareTo(_end) < 0)
                        {
                            expectedStart = ExtendStartEdge(_start);
                            expectedEnd = ExtendEndEdge(_end);
                        }
                        else
                        {
                            expectedStart = ExtendEndEdge(_start);
                            expectedEnd = ExtendStartEdge(_end);
                        }
                    }
                    else // No need to adjust pointers
                    {
                        // With no adjustments, the start and end edges should
                        // just match the mouse movements.
                        expectedStart = _start;
                        expectedEnd = _end;
                    }
                }

                try
                {
                    Verifier.Verify(anchorPointer.CompareTo(expectedStart) == 0, "Start edges match.", true);
                    Verifier.Verify(movingPointer.CompareTo(expectedEnd) == 0, "End edges match.", true);
                }
                catch
                {
                    Test.Uis.Loggers.TextTreeLogger.LogContainer(
                        "mouse-selection", _start,
                        movingPointer, "Moving pointer",
                        anchorPointer, "Selection anchor",
                        selection.Start, "Selection start",
                        _start, "Mouse start",
                        selection.End, "Selection end",
                        _end, "Mouse end",
                        expectedStart, "Expected start",
                        expectedEnd, "Expected end");
                    throw;
                }
            }
            // Delay 500 milliseconds before reading next combination
            // to make sure previous verification is completed.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(NextCombination));
        }

        private void VerifyTableSelection()
        {
            TextSelection selection;    // The current selection on the control.
            TextPointer movingPointer;  // Moving pointer in selection.
            TextPointer anchorPointer;  // Anchor pointer in selection.
            TextPointer expectedStart;  // Expected start position.
            TextPointer expectedEnd;    // Expecetd end position.
            selection = _wrapper.SelectionInstance;
            anchorPointer = selection.Start;
            movingPointer = selection.End;

            // If both pointer (_start && _end) are in Table
            // and there is a cross cell then the whole row is selected
            if (_wrapper.IsTextPointerInsideTextElement(_start, typeof(Table)) &&
                _wrapper.IsTextPointerInsideTextElement(_end, typeof(Table)) &&
                _wrapper.GetSelectedText(true, false).Contains("</TableCell><TableCell>"))
            {
                expectedStart = _start;
                while(!(expectedStart.GetPointerContext(LogicalDirection.Backward) == TextPointerContext.ElementStart &&
                    expectedStart.GetAdjacentElement(LogicalDirection.Backward) is TableRow))
                {
                    expectedStart = expectedStart.GetPositionAtOffset(-1);
                }

                // Move fw to get to run
                while (!(expectedStart.GetAdjacentElement(LogicalDirection.Backward) is Run))
                {
                    expectedStart = expectedStart.GetPositionAtOffset(1);
                }

                expectedEnd = _end;
                while (!(expectedEnd.GetAdjacentElement(LogicalDirection.Forward) is TableRow))
                {
                    expectedEnd = expectedEnd.GetPositionAtOffset(1);
                }
            }
            else
            {
                expectedStart = _start;
                expectedEnd = _end;
            }

            try
            {
                Verifier.Verify(anchorPointer.CompareTo(expectedStart) == 0, "Start edges match.", true);
                Verifier.Verify(movingPointer.CompareTo(expectedEnd) == 0, "End edges match.", true);
            }
            catch
            {
                Test.Uis.Loggers.TextTreeLogger.LogContainer(
                    "mouse-selection-table", _start,
                    movingPointer, "Moving pointer",
                    anchorPointer, "Selection anchor",
                    selection.Start, "Selection start",
                    _start, "Mouse start",
                    selection.End, "Selection end",
                    _end, "Mouse end",
                    expectedStart, "Expected start",
                    expectedEnd, "Expected end");
                throw;
            }
        }

        #endregion Main flow.

        #region Helper methods.

        // Returns a pointer to where the start of selection would
        // extend, starting from a given pointer.
        private TextPointer ExtendEndEdge(TextPointer pointer)
        {
            TextPointer result;

            result = pointer;

            // For case End point is EndOfWrappedLine then return result
            if (_start.CompareTo(_end) < 0)
            {
                if (_endPosition.DocumentPosition == DocumentPosition.EndOfWrappedLine)
                {
                    return result;
                }
            }

            if (_start.CompareTo(_end) > 0)
            {
                if (_startPosition.DocumentPosition == DocumentPosition.EndOfWrappedLine)
                {
                    return result;
                }
            }

            if (_endPosition.DocumentPosition == DocumentPosition.EndOfDocument)
            {
                return _wrapper.End;
            }

            if (result.GetTextInRun(LogicalDirection.Forward) == "") return result;

            // While startwithspace is not true, keep move forward 1
            while (!(TextUtils.StartsWithWhitespace(result.GetTextInRun(LogicalDirection.Forward)) ||
                TextUtils.StartsWithPunctuation(result.GetTextInRun(LogicalDirection.Forward)) ||
                result.GetTextInRun(LogicalDirection.Forward) == ""))
            {
                result = result.GetPositionAtOffset(1);
            }

            // Move forward 1 more time to include the whtiespace
            if (TextUtils.StartsWithWhitespace(result.GetTextInRun(LogicalDirection.Forward)))
            {
                result = result.GetPositionAtOffset(1);
            }

            return result;
        }

        // Returns a pointer to where the end of selection would
        // extend, starting from a given pointer.
        private TextPointer ExtendStartEdge(TextPointer pointer)
        {
            TextPointer result;
            Rect beforeRect;
            Rect afterRect;
            bool extend;
            result = pointer;

            do
            {
                if (result.GetTextInRun(LogicalDirection.Backward) == "") return result;
                if (TextUtils.EndsWithPunctuation(result.GetTextInRun(LogicalDirection.Backward))) return result;
                if (TextUtils.EndsWithWhitespace(result.GetTextInRun(LogicalDirection.Backward))) return result;

                // If mouse select from right to left and
                // If _start.Top > _end.Bottom then extend.
                beforeRect = _start.GetCharacterRect(LogicalDirection.Backward);
                if (_endPosition.DocumentPosition == DocumentPosition.EndOfWrappedLine)
                {
                    afterRect = _end.GetCharacterRect(LogicalDirection.Backward);
                }
                else
                {
                    afterRect = _end.GetCharacterRect(LogicalDirection.Forward);
                }

                extend = beforeRect.Top > afterRect.Bottom;
                extend = extend && _start.CompareTo(_end) > 0;

                if (!extend &&
                    TextUtils.EndsWithWhitespace(result.GetTextInRun(LogicalDirection.Backward)))
                {
                    return result;
                }

                result = result.GetPositionAtOffset(-1);
            } while (result != null);

            return result;
        }

        // Returns a value false if the selection contian 1 or less whitespace and
        // selection doesn't contains punctuation character, true if all other cases.
        private bool ShouldExtendPointers(string selection)
        {
            if (TextUtils.GetWhitespaceCount(selection) == 1 &&
                (!TextUtils.StartsWithWhitespace(selection) &&
                !TextUtils.EndsWithWhitespace(selection)))
            {
                return true;
            }

            if (TextUtils.GetWhitespaceCount(selection) < 2 &&
                !TextUtils.ContainsPunctuation(selection))
            {
                return false;
            }
            return true;
        }

        #endregion Helper methods.

        #region Private fields.

        //Wrapper around control being tested.
        private UIElementWrapper _wrapper = null;

        //Start pointer for selection operation.
        private TextPointer _start;

        //End pointer for selection operation.
        private TextPointer _end;

        //Whether to select from start to end or in the opposite direction.
        private bool _startToEnd = false;

        //Position to start select operation from.
        private DocumentPositionData _startPosition = null;

        //Position to end select operation.
        private DocumentPositionData _endPosition = null;

        //Type of control to create.
        private TextEditableType _editableType = null ;

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies that the selection object reflects the expected positions
    /// after a mouse double click operation.
    /// P1 - EditingTest.exe /TestCaseType:MouseDoubleClickTest /Pri:1 (RichTextBox, TextBox)
    /// </summary>
    [Test(2, "Editor", "MouseDoubleClickTest", MethodParameters="/TestCaseType:MouseDoubleClickTest /Pri:1")]
    [TestOwner("Microsoft"), TestTitle("MouseDoubleClickTest"), TestTactics("414"), TestWorkItem("61"), TestBugs("867,868,96")]
    public class MouseDoubleClickTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Reads combination values.</summary>
        /// <param name="values">Values to read from.</param>
        /// <returns>Whether the given combination should be accepted.</returns>
        /// <remarks>Filters out equal position-to-position combinations.</remarks>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // Filter out case with related 
            if (TextEditableType.GetByName("RichTextBox") == _editableType)
            {
                result = result && _startPosition.DocumentPosition != DocumentPosition.EndOfDelimitedLine;
                result = result && _startPosition.DocumentPosition != DocumentPosition.StartOfWord;
                result = result && _startPosition.DocumentPosition != DocumentPosition.EndOfWhitespace;
            }

            result = result && _startPosition.DocumentPosition != DocumentPosition.EndOfDocument;
            return result;
        }

        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(TestElement);
            ((Control)(_wrapper.Element)).Width = 100;
            ((Control)(_wrapper.Element)).Background = Brushes.Lavender;

            if (_wrapper.IsElementRichText)
            {
                _wrapper.XamlText = "<Paragraph>First paragraph with a<LineBreak/> Line break'</Paragraph>" +
                    "<Paragraph>Second paragraph.</Paragraph>";
            }
            else
            {
                ((TextBox)(_wrapper.Element)).TextWrapping = TextWrapping.Wrap;
                _wrapper.Text = "This is a test1. This is a test2. This is a test3. " +
                    "This is a test4. This is a test5.\r\nNew line1\r\nNew line2 \r\n New line3'";
            }
            QueueDelegate(PerformSelection);
        }

        private void PerformSelection()
        {
            _start = _startPosition.FindAny(_wrapper);

            if (_start == null)
            {
                QueueDelegate(NextCombination);
                return;
            }

            QueueDelegate(DoubleMovement);
        }

        private void DoubleMovement()
        {
            Point startPoint;
            startPoint = MouseEditingTestHelper.FindPoint(_start, _wrapper);

            MouseInput.MouseClick(startPoint);
            MouseInput.MouseClick(startPoint);

            QueueDelegate(VerifySelection);
        }

        private void VerifySelection()
        {
            TextSelection selection;    // The current selection on the control.
            TextPointer expectedLeft;
            TextPointer expectedRight;

            selection = _wrapper.SelectionInstance;
            expectedLeft = ExtendStartEdge(_start, selection.Text);
            expectedRight = ExtendEndEdge(_start, selection.Text); ;

            try
            {
                Verifier.Verify(expectedLeft.CompareTo(selection.Start) == 0, "Left edges match.", true);
                Verifier.Verify(expectedRight.CompareTo(selection.End) == 0, "Right edges match.", true);
            }
            catch
            {
                Test.Uis.Loggers.TextTreeLogger.LogContainer(
                    "mouse-double-click", _start,
                    expectedLeft, "Expected Left",
                    selection.Start, "Actual Left",
                    expectedRight, "Expected Right",
                    selection.End, "Actual Right");
                throw;
            }

            // Delay 500 milliseconds before reading next combination
            // to make sure previous verification is completed.
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromMilliseconds(500), new SimpleHandler(NextCombination));
        }

        #region Private Helper methods.
        /// <summary>
        /// Returns a pointer to where the end of selection would
        /// extend, starting from a given pointer.
        /// </summary>
        private TextPointer ExtendEndEdge(TextPointer pointer, string selection)
        {
            TextPointer result;

            result = pointer;

            // While startwithspace & startwithpunctuation are not true , keep move forward 1
            while (!TextUtils.StartsWithWhitespace(result.GetTextInRun(LogicalDirection.Forward)) &&
                !TextUtils.StartsWithPunctuation(result.GetTextInRun(LogicalDirection.Forward)))
            {
                // Break if we hit a hard dilimited
                if (result.GetTextInRun(LogicalDirection.Forward) == "") break;
                result = result.GetPositionAtOffset(1);
            }

            // Move forward to include whtiespace
            while (result.GetTextInRun(LogicalDirection.Forward).StartsWith(" "))
            {
                result = result.GetPositionAtOffset(1);
            }

            // Move forward to include \r\n
            if (result.GetTextInRun(LogicalDirection.Forward).StartsWith("\r\n") && selection == "\r\n")
            {
                result = result.GetPositionAtOffset(2);
            }

            return result;
        }

        /// <summary>
        /// Returns a pointer to where the start of selection would
        /// extend, starting from a given pointer.
        /// </summary>
        private TextPointer ExtendStartEdge(TextPointer pointer, string selection)
        {
            TextPointer result;
            result = pointer;

            do
            {
                if (result.GetTextInRun(LogicalDirection.Backward) == "") return result;

                if (result.GetTextInRun(LogicalDirection.Backward).EndsWith("\r\n") && selection == "\r\n ")
                {
                    result = result.GetPositionAtOffset(-2);
                    break;
                }

                if (!TextUtils.EndsWithText(result.GetTextInRun(LogicalDirection.Backward))) break;

                result = result.GetPositionAtOffset(-1);
            } while (result != null);

            return result;
        }
        #endregion Private Helper methods.

        #region Private fields.

        /// <summary>Wrapper around control being tested.</summary>
        private UIElementWrapper _wrapper;

        /// <summary>Start pointer for selection operation.</summary>
        private TextPointer _start;

        /// <summary>Position to start select operation from.</summary>
        private DocumentPositionData _startPosition = null;

        /// <summary>Type of control to create.</summary>
        private TextEditableType _editableType = null;

        #endregion Private fields.

        #endregion Main flow.
    }

    /// <summary>
    /// Verifies that the document scroll/not scroll after a mouse drag operation
    /// EditingTest.exe /TestCaseType:MouseDragToScroll /Pri:1
    /// </summary>
    [Test(2, "Editor", "MouseDragToScroll", MethodParameters="/TestCaseType:MouseDragToScroll /Pri:1", Timeout=300)]
    [TestOwner("Microsoft"), TestTitle("MouseDragToScroll"), TestTactics("415"), TestWorkItem("60"), TestBugs("97")]
    public class MouseDragToScroll : ManagedCombinatorialTestCase
    {
        #region Main flow.
        /// <summary>Reads combination values.</summary>
        /// <param name="values">Values to read from.</param>
        /// <returns>Whether the given combination should be accepted.</returns>
        /// <remarks>Filters out equal position-to-position combinations.</remarks>
        protected override bool DoReadCombination(System.Collections.Hashtable values)
        {
            bool result;

            result = base.DoReadCombination(values);

            // Filter out case with related bug for RichTextBox
            if (TextEditableType.GetByName("RichTextBox") == _editableType)
            {
                if (result)
                {
                    // Remove when Regression_Bug97 is fixed. 
                    result = (_isEnoughData != true) && (_scrollDirection != "Left");
                }
            }
            return result;
        }

        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            string textScripts = "";

            TestElement = _editableType.CreateInstance();
            _elementBox = (TextBoxBase)TestElement;
            _wrapper = new UIElementWrapper(TestElement);
            _elementBox.Width = _elementBox.Height = 200;
            _elementBox.FontSize = _elementBox.FontSize = 40;
            if (_elementBox is TextBox)
            {
                ((TextBox)_elementBox).TextWrapping = TextWrapping.NoWrap;
            }

            // Creating string data
            if (_isEnoughData)
            {
                for (int i = 0; i < 15; i++)
                {
                    textScripts += i;
                    for (int j = 0; j < 15; j++)
                    {
                        textScripts += TextScript.Latin.Sample;
                    }
                    if (i < 14)
                    {
                        textScripts += "\r\n";
                    }
                }
            }
            else
            {
                textScripts += "abc\r\ndef";
            }
            _wrapper.Text = textScripts;

            QueueDelegate(ScrollInitially);
        }

        private void ScrollInitially()
        {
            _initialOffsetValue = 40d * 3;
            _elementBox.ScrollToHorizontalOffset(_initialOffsetValue);
            _elementBox.ScrollToVerticalOffset(_initialOffsetValue);
            QueueDelegate(DoAction);
        }

        private void DoAction()
        {
            // rec is the center of test control
            _rect = ElementUtils.GetScreenRelativeRect(_wrapper.Element);
            _x = (int)(_rect.Left + _rect.Width / 2);
            _y = (int)(_rect.Top + _rect.Height / 2);

            int x1 =0,y1=0;

            switch (_scrollDirection)
            {
                case "Left":
                    x1 = (int)(_rect.Left - 10);
                    break;
                case "Right":
                    x1 = (int)(_rect.Right + 10);
                    break;
                case "Top":
                    y1 = (int)(_rect.Top - 10);
                    break;
                case "Down":
                    y1 = (int)(_rect.Bottom + 10);
                    break;
            }

            MouseInput.MouseDragInOtherThread(_x, _y, x1, y1, true, new TimeSpan(0, 0, 0, 0, 500), new SimpleHandler(VerifyScrolling), Dispatcher.CurrentDispatcher);
        }

        private void MouseUp()
        {
            MouseInput.MouseUp();
            QueueDelegate(VerifyScrolling);
        }

        private void VerifyScrolling()
        {
            Log("Initial Offset [" + _initialOffsetValue + "]");
            Log("Actual VerticalOffset   [" + _elementBox.VerticalOffset.ToString() + "]");
            Log("Actual HorizontalOffset [" + _elementBox.HorizontalOffset.ToString() + "]");
            if (_isEnoughData)
            {
                switch (_scrollDirection)
                {
                    case "Left":
                        Verifier.Verify(_elementBox.HorizontalOffset < _initialOffsetValue,
                            "HorizontalOffset should be less than initialOffsetValue");
                        break;
                    case "Right":
                        Verifier.Verify(_elementBox.HorizontalOffset > _initialOffsetValue,
                            "HorizontalOffset should be greater than initialOffsetValue");
                        break;
                    case "Top":
                        Verifier.Verify(_elementBox.VerticalOffset < _initialOffsetValue,
                            "VerticalOffset should be less than initialOffsetValue");
                        break;
                    case "Down":
                        Verifier.Verify(_elementBox.VerticalOffset > _initialOffsetValue,
                            "VerticalOffset should be less than initialOffsetValue");
                        break;
                }
            }
            else
            {
                Verifier.Verify(_elementBox.VerticalOffset == 0,
                    "VerticalOffset should be 0");
                Verifier.Verify(_elementBox.HorizontalOffset == 0,
                     "HorizontalOffset should be 0");
            }
            QueueDelegate(NextCombination);
        }
        #endregion Main flow.

        #region Private fields.
        private TextEditableType _editableType  = null;
        private double _initialOffsetValue;
        private bool _isEnoughData = false;  // To determind how much data should be loaded
        private Rect _rect;
        private string _scrollDirection ="";
        private int _x,_y; // x, y coordinate of test control
        private UIElementWrapper _wrapper;
        private TextBoxBase _elementBox;
        #endregion Private fields.
    }

    /// <summary>
    /// Helper methods for mouse editing test.
    /// </summary>
    public static class MouseEditingTestHelper
    {
        /// <summary>
        /// Find Point from a TextPointer
        /// </summary>
        public static Point FindPoint(TextPointer pointer, UIElementWrapper _wrapper)
        {
            Rect rectangle;
            Point result;
            double x;

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            rectangle = pointer.GetCharacterRect(pointer.LogicalDirection);

            if (pointer.LogicalDirection == LogicalDirection.Backward)
            {
                x = rectangle.Right;
            }
            else
            {
                x = rectangle.Left;
            }

            result = new Point(x, rectangle.Top + rectangle.Height / 2);
            return ElementUtils.GetScreenRelativePoint(_wrapper.Element, result);
        }
    }

    /// <summary>
    /// Verifies that the whole paragraph are selected when do mouse triple clicks.
    /// </summary>
    [Test(2, "Editor", "MouseTripleClicksTest1", MethodParameters = "/TestCaseType:MouseTripleClicksTest /Pri=0  /StopOnFailure=true", Timeout = 300)]
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "MouseTripleClicksTest2", MethodParameters = "/TestCaseType:MouseTripleClicksTest /Pri=0 /XbapName=EditingTestDeploy", Timeout = 300,Disabled=true)]
    [Test(2, "Editor", "MouseTripleClicksTest3", MethodParameters = "/TestCaseType:MouseTripleClicksTest /Pri=1  /StopOnFailure=true", Timeout = 300)]
    [TestOwner("Microsoft"), TestTitle("MouseTripleClicksTest"), TestTactics("416,417"), TestWorkItem("62"), TestBugs("869,890")]
    public class MouseTripleClicksTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            _wrapper = new UIElementWrapper(TestElement);
            if (_wrapper.Element is TextBox)
                _wrapper.Text = "Simple text.";
            else
                _wrapper.XamlText = _contentData;
            ((Control)(TestElement)).FontSize = 20;
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            switch (_clickPosition)
            {
                case "Front": // Infront of text
                    if (_wrapper.Element is TextBox)
                    {
                        _point = MouseEditingTestHelper.FindPoint(_wrapper.Start, _wrapper);
                        _point = new Point(_point.X + 1, _point.Y + 1);
                    }
                    else
                        _point = MouseEditingTestHelper.FindPoint(_wrapper.Start.GetNextContextPosition(LogicalDirection.Forward), _wrapper);
                    break;
                case "End": // End of text
                    _point = MouseEditingTestHelper.FindPoint(_wrapper.End, _wrapper);
                    break;
                case "OnWord": // On first word
                    TextPointer tp = _wrapper.Start;
                    tp = tp.GetPositionAtOffset(7, LogicalDirection.Forward);
                    _point = MouseEditingTestHelper.FindPoint(tp, _wrapper);
                    break;
            }
            TripleClick(_point);
            QueueDelegate(VerifySelection);
        }

        private void TripleClick(Point point)
        {
            //Using a much lower level call since the case occasionally fails because of delay in clicks
            int i = 3;
            while (i > 0)
            {
                Input.SendMouseInput(point.X, point.Y, 0, SendMouseInputFlags.LeftDown | SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
                Input.SendMouseInput(point.X, point.Y, 0, SendMouseInputFlags.LeftUp | SendMouseInputFlags.Move | SendMouseInputFlags.Absolute);
                i--;
            }
            //MouseInput.MouseClick(point);
            //MouseInput.MouseClick(point);
            //MouseInput.MouseClick(point);
        }

        private void VerifySelection()
        {
            string _actual = _wrapper.SelectionInstance.Text;
            _actual += (_wrapper.Element is TextBox) ? "" : "\r\n";

            string _expect = _wrapper.Text;
            if (_wrapper.Element is TextBox)
                _expect = (_clickPosition == "Front") ? "Simple " : (_clickPosition == "End")?"." : "text";

            Verifier.Verify(_expect == _actual, "Selection matched." +
                "\nExpect [" + _expect + "\r\n" + "]\nActual [" + _actual + "]");
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new SimpleHandler(NextCombination));
        }

        #endregion Main flow.

        #region Private fields.
        private UIElementWrapper _wrapper = null;
        private Point _point;
        private TextEditableType _editableType = null;
        private string _contentData = string.Empty;
        private string _clickPosition = string.Empty;
        /// <summary>Content for RichTextBox</summary>
        public static string[] _rtbContent = new string[] {
            "<Paragraph Background='yellow'>an_Image_<Image Height='100' Width='100'/>"+
            //"Source='"+Path.Combine(Path.GetPathRoot(System.Environment.SystemDirectory), "work")+"\\test.png' />"+
            "_control.</Paragraph>",

            "<Paragraph Background='red'>Button_<Button />_control.</Paragraph>",

            "<Paragraph Background='lightblue'>Linebreak_<LineBreak/>_element.</Paragraph>",

            "<Paragraph Background='pink'>simple text</Paragraph>",

            "<Paragraph Background='lavender'><Italic>Italic text</Italic></Paragraph>",

            "<List><ListItem><Paragraph>List item1</Paragraph></ListItem></List>",

            "<Table><TableRowGroup><TableRow>"+
                "<TableCell><Paragraph>text string 1.</Paragraph></TableCell>"+
            "</TableRow></TableRowGroup></Table>"
        };

        #endregion Private fields.
    }

    /// <summary>
    /// Verifies shift mouse click and double click select text correctly
    /// changing content in the mouse up handler doesnt cause a crash
    /// </summary>
    [Test(2, "Editor", "MouseSelectionTest", MethodParameters = "/TestCaseType=MouseSelectionTest", Timeout=120)]
    [TestOwner("Microsoft"), TestTitle("MouseSelectionTest"), TestTactics("418"), TestWorkItem("63, 64"), TestBugs("690")]
    public class MouseSelectionTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a combination for the test case.</summary>
        protected override void DoRunCombination()
        {
            TestElement = _editableType.CreateInstance();
            _element = TestElement;

            if (_element is PasswordBox)
            {
                NextCombination();
            }
            else
            {
                _controlWrapper = new UIElementWrapper(TestElement);
                _controlWrapper.Text = "Hello World";
                ((Control)_controlWrapper.Element).FontSize = 30;
                QueueDelegate(DoFocus);
            }
        }

        private void DoFocus()
        {
            _element.Focus();
            QueueDelegate(StartShiftClickWithinWord);
        }

        #region ShiftClickWithinWord.

        private void StartShiftClickWithinWord()
        {
            Log("\r\n************** Shift click to right of caret *************\r\n");
            KeyboardInput.TypeString("^{HOME}{RIGHT 3}");
            QueueDelegate(PressShiftForClickOnce);
        }

        private  void PressShiftForClickOnce()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRight);
        }

        private void MouseClickOnceOnRight()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p,2);
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnce);
        }

        private void ReleaseShiftAfterClickOnce()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyRightPostionSelected);
        }

        private void VerifyRightPostionSelected()
        {
            Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo", "Expected SelectedText [lo] Actual [" +
                _controlWrapper.SelectionInstance.Text + "]", true);
            QueueDelegate(StartShiftClickAcrossWordsRTL);
        }

        #endregion ShiftClickWithinWord.

        #region AcrossWordsRTL.

        private void StartShiftClickAcrossWordsRTL()
        {
            Log("\r\n**************RTL  Shift click to right across words*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.RightToLeft;
            KeyboardInput.TypeString("^{HOME}{LEFT 3}");
            QueueDelegate(PressShiftForClickOnceAcrossWordsRTL);
        }

        private void PressShiftForClickOnceAcrossWordsRTL()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForRTL);
        }

        private void MouseClickOnceOnRightForRTL()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, 5);
            Rect r1 = ElementUtils.GetScreenRelativeRect(_element);
            p.X = r1.Right - p.X;
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceAcrossWordsRTL);
        }

        private void ReleaseShiftAfterClickOnceAcrossWordsRTL()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyRightPostionSelectedAcrossWordsRTL);
        }

        private void VerifyRightPostionSelectedAcrossWordsRTL()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains( "lo W"), "Expected SelectedText to Contain [lo W] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello")==false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                      _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("World") == false), "Expected SelectedText NOT to Contain [World] Actual [" +
                      _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "Hello Wo", "Expected SelectedText [Hello Wo] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickAcrossWordsLTR);
        }

        #endregion AcrossWordsRTL.

        #region AcrossWordsLTR.

        private void StartShiftClickAcrossWordsLTR()
        {
            Log("\r\n**************LTR  Shift click to right across words*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}{RIGHT 3}");
            QueueDelegate(PressShiftForClickOnceAcrossWordsLTR);
        }

        private void PressShiftForClickOnceAcrossWordsLTR()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForLTR);
        }

        private void MouseClickOnceOnRightForLTR()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, 5);
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceAcrossWordsLTR);
        }

        private void ReleaseShiftAfterClickOnceAcrossWordsLTR()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyRightPostionSelectedAcrossWordsLTR);
        }

        private void VerifyRightPostionSelectedAcrossWordsLTR()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo Wo", "Expected SelectedText [lo Wo] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "Hello Wo", "Expected SelectedText [Hello Wo] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickExpandSelectionLTR);
        }

        #endregion AcrossWordsLTR.

        #region ExpandSelectionOnRightLTR.

        private void StartShiftClickExpandSelectionLTR()
        {
            Log("\r\n**************LTR Partial Selection of first word, Shift click to right*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}+{RIGHT 3}");
            QueueDelegate(PressShiftForClickOnceExpandSelectionLTR);
        }

        private void PressShiftForClickOnceExpandSelectionLTR()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForExpandSelectionLTR);
        }

        private void MouseClickOnceOnRightForExpandSelectionLTR()
        {
            Point p = new Point();
            if (_element is TextBox)
            {
                //8 because caretIndex return the starting index of selection
                GetPointFromRequiredPosition(ref p, 8);
            }
            else
            {
                GetPointFromRequiredPosition(ref p, 5);
            }
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceExpandSelectionLTR);
        }

        private void ReleaseShiftAfterClickOnceExpandSelectionLTR()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyExpandSelectionLTR);
        }

        private void VerifyExpandSelectionLTR()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text == "Hello Wo", "Expected SelectedText [Hello Wo] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "Hello Wo", "Expected SelectedText [Hello Wo] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickExpandSelectionOnLeftLTR);
        }

        #endregion ExpandSelectionOnRightLTR.

        #region ExpandSelectionOnLeftLTR.

        private void StartShiftClickExpandSelectionOnLeftLTR()
        {
            Log("\r\n**************LTR Partial Selection of first word, Shift click to LEFT*************\r\n");
            //_element.Focus();
            KeyboardInput.TypeString("^{END}{LEFT 2}+{LEFT 2}");
            QueueDelegate(PressShiftForClickOnceExpandSelectionOnLeftLTR);
        }

        private void PressShiftForClickOnceExpandSelectionOnLeftLTR()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForExpandSelectionOnLeftLTR);
        }

        private void MouseClickOnceOnRightForExpandSelectionOnLeftLTR()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, -4);

            //if (_element is TextBox)
            //{
            //    //8 because caretIndex return the starting index of selection
            //    GetPointFromRequiredPosition(ref p, -4);
            //}
            //else
            //{
            //    GetPointFromRequiredPosition(ref p, -4);
            //}
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceExpandSelectionOnLeftLTR);
        }

        private void ReleaseShiftAfterClickOnceExpandSelectionOnLeftLTR()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyExpandSelectionOnLeftLTR);
        }

        private void VerifyExpandSelectionOnLeftLTR()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo Wor", "Expected SelectedText [lo Wor] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo World", "Expected SelectedText [lo World] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickExpandSelectionRTL);
        }

        #endregion ExpandSelectionOnLeftLTR.

        #region ExpandSelectionOnRightRTL.

        private void StartShiftClickExpandSelectionRTL()
        {
            Log("\r\n**************RTL Partial Selection of first word, Shift click to right*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.RightToLeft;
            KeyboardInput.TypeString("^{HOME}{LEFT 2}+{LEFT 2}");
            QueueDelegate(PressShiftForClickOnceExpandSelectionRTL);
        }

        private void PressShiftForClickOnceExpandSelectionRTL()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForExpandSelectionRTL);
        }

        private void MouseClickOnceOnRightForExpandSelectionRTL()
        {
            Point p = new Point();
            if (_element is TextBox)
            {
                //8 because caretIndex return the starting index of selection
                GetPointFromRequiredPosition(ref p, 6);
            }
            else
            {
                GetPointFromRequiredPosition(ref p, 4);
            }
            Rect r1 = ElementUtils.GetScreenRelativeRect(_element);
            p.X = r1.Right - p.X;
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceExpandSelectionRTL);
        }

        private void ReleaseShiftAfterClickOnceExpandSelectionRTL()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyExpandSelectionRTL);
        }

        private void VerifyExpandSelectionRTL()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("llo W"), "Expected SelectedText to contain[llo W] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("World") == false), "Expected SelectedText NOT to Contain [World] Actual [" +
                     _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello") == false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                     _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "Hello Wo", "Expected SelectedText [Hello Wo] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickExpandSelectionOnLeftRTL);
        }

        #endregion ExpandSelectionOnRightRTL.

        #region ExpandSelectionOnLeftRTL.

        private void StartShiftClickExpandSelectionOnLeftRTL()
        {
            Log("\r\n**************RTL Partial Selection of first word, Shift click to left*************\r\n");
            //_element.Focus();
            KeyboardInput.TypeString("^{END}{RIGHT 2}+{RIGHT 2}");
            QueueDelegate(PressShiftForClickOnceExpandSelectionOnLeftRTL);
        }

        private void PressShiftForClickOnceExpandSelectionOnLeftRTL()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForExpandSelectionOnLeftRTL);
        }

        private void MouseClickOnceOnRightForExpandSelectionOnLeftRTL()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, -4);

            Rect r1 = ElementUtils.GetScreenRelativeRect(_element);
            p.X = r1.Right - p.X;

            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceExpandSelectionOnLeftRTL);
        }

        private void ReleaseShiftAfterClickOnceExpandSelectionOnLeftRTL()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyExpandSelectionOnLeftRTL);
        }

        private void VerifyExpandSelectionOnLeftRTL()
        {
            if (_element is TextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("lo Wor"), "Expected SelectedText to contain [lo Wor] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("World") == false), "Expected SelectedText NOT to Contain [World] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello") == false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                     _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                //Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo World", "Expected SelectedText [lo World] Actual [" +
                //    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartShiftClickBacktrackSelectionLTR);
        }

        #endregion ExpandSelectionOnLeftRTL.

        #region BacktrackSelectionLTR.

        private void StartShiftClickBacktrackSelectionLTR()
        {
            Log("\r\n**************RTL Selection spans 2 words, Shift click in middle of first word *************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}+{END}+{LEFT 3}");
            QueueDelegate(PressShiftForClickOnceBacktrackSelectionLTR);
        }

        private void PressShiftForClickOnceBacktrackSelectionLTR()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForBacktrackSelectionLTR);
        }

        private void MouseClickOnceOnRightForBacktrackSelectionLTR()
        {
            Point p = new Point();
            if (_element is TextBox)
            {
                GetPointFromRequiredPosition(ref p, 3);
            }
            else
            {
                GetPointFromRequiredPosition(ref p, -5);
            }

            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceBacktrackSelectionLTR);
        }

        private void ReleaseShiftAfterClickOnceBacktrackSelectionLTR()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyBacktrackSelectionLTR);
        }

        private void VerifyBacktrackSelectionLTR()
        {
            Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("Hel"), "Expected SelectedText to contain [Hel] Actual [" +
                                _controlWrapper.SelectionInstance.Text + "]", true);
            Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("World") == false), "Expected SelectedText NOT to Contain [World] Actual [" +
                _controlWrapper.SelectionInstance.Text + "]", true);
            Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello") == false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                 _controlWrapper.SelectionInstance.Text + "]", true);
            QueueDelegate(StartShiftClickBacktrackSelectionRTL);
        }

        #endregion BacktrackSelectionLTR.

        #region BacktrackSelectionRTL.

        private void StartShiftClickBacktrackSelectionRTL()
        {
            Log("\r\n**************LTR Selection spans 2 words, Shift click in middle of first word *************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.RightToLeft;
            KeyboardInput.TypeString("^{HOME}+{END}+{right 3}");
            QueueDelegate(PressShiftForClickOnceBacktrackSelectionRTL);
        }

        private void PressShiftForClickOnceBacktrackSelectionRTL()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickOnceOnRightForBacktrackSelectionRTL);
        }

        private void MouseClickOnceOnRightForBacktrackSelectionRTL()
        {
            Point p = new Point();
            if (_element is TextBox)
            {
                GetPointFromRequiredPosition(ref p, 3);
            }
            else
            {
                GetPointFromRequiredPosition(ref p, -6);
            }
            Rect r1 = ElementUtils.GetScreenRelativeRect(_element);
            p.X = r1.Right - p.X;
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickOnceBacktrackSelectionRTL);
        }

        private void ReleaseShiftAfterClickOnceBacktrackSelectionRTL()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyBacktrackSelectionRTL);
        }

        private void VerifyBacktrackSelectionRTL()
        {
            Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("He"), "Expected SelectedText to contain [He] Actual [" +
                                _controlWrapper.SelectionInstance.Text + "]", true);
            Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("lo World") == false), "Expected SelectedText NOT to Contain [lo World] Actual [" +
                _controlWrapper.SelectionInstance.Text + "]", true);
            Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello") == false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                 _controlWrapper.SelectionInstance.Text + "]", true);
            QueueDelegate(StartShiftDoubleClickWithinWord);
        }

        #endregion BacktrackSelectionRTL.

        #region ShiftDoubleClickWithinWord.

        private void StartShiftDoubleClickWithinWord()
        {
            Log("\r\n**************Caret in middle of Word, Shift Double Click on Right of caret *************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}{RIGHT 3}");
            QueueDelegate(PressShiftForClickTwice);
        }

        private void PressShiftForClickTwice()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseClickTwiceOnRight);
        }

        private void MouseClickTwiceOnRight()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, 2);
            MouseInput.MouseClick(p);
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterClickTwice);
        }

        private void ReleaseShiftAfterClickTwice()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyRightPostionSelectedAfterClickTwice);
        }

        private void VerifyRightPostionSelectedAfterClickTwice()
        {
            Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo", "Expected SelectedText [lo] Actual [" +
                _controlWrapper.SelectionInstance.Text + "]", true);
            QueueDelegate(StartAcrossWordsShiftDoubleClickRTL);
        }

        #endregion ShiftDoubleClickWithinWord.

        #region AcrossWordsShiftDoubleClickRTL.

        private void StartAcrossWordsShiftDoubleClickRTL()
        {
            Log("\r\n**************RTL  Shift double click to right across words*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.RightToLeft;
            KeyboardInput.TypeString("^{HOME}{LEFT 3}");
            QueueDelegate(PressShiftForDoubleClickRTL);
        }

        private void PressShiftForDoubleClickRTL()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseDoubleClickOnRightForRTL);
        }

        private void MouseDoubleClickOnRightForRTL()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, 5);
            Rect r1 = ElementUtils.GetScreenRelativeRect(_element);
            p.X = r1.Right - p.X;
            MouseInput.MouseClick(p);
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterDoubleClickRTL);
        }

        private void ReleaseShiftAfterDoubleClickRTL()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyWordsAfterShiftDoubleClickRTL);
        }

        private void VerifyWordsAfterShiftDoubleClickRTL()
        {
            if (_element is RichTextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("Hello World"), "Expected SelectedText to Contain [Hello World] Actual [" +
                        _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("lo W"), "Expected SelectedText to Contain [lo W] Actual [" +
                     _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("Hello") == false), "Expected SelectedText NOT to Contain [Hello] Actual [" +
                      _controlWrapper.SelectionInstance.Text + "]", true);
                Verifier.Verify((_controlWrapper.SelectionInstance.Text.Contains("World") == false), "Expected SelectedText NOT to Contain [World] Actual [" +
                      _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(StartAcrossWordsShiftDoubleClickLTR);
        }

        #endregion AcrossWordsShiftDoubleClickRTL.

        #region AcrossWordsShiftDoubleClickLTR.

        private void StartAcrossWordsShiftDoubleClickLTR()
        {
            Log("\r\n**************LTR  Shift double  click to right across words*************\r\n");
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}{RIGHT 3}");
            QueueDelegate(PressShiftForDoubleClickLTR);
        }

        private void PressShiftForDoubleClickLTR()
        {
            KeyboardInput.PressVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(MouseDoubleClickOnRightForLTR);
        }

        private void MouseDoubleClickOnRightForLTR()
        {
            Point p = new Point();
            GetPointFromRequiredPosition(ref p, 5);
            MouseInput.MouseClick(p);
            MouseInput.MouseClick(p);
            QueueDelegate(ReleaseShiftAfterDoubleClickLTR);
        }

        private void ReleaseShiftAfterDoubleClickLTR()
        {
            KeyboardInput.ReleaseVirtualKey(Win32.VK_SHIFT);
            QueueDelegate(VerifyWordsAfterShiftDoubleClickLTR);
        }

        private void VerifyWordsAfterShiftDoubleClickLTR()
        {
            if (_element is RichTextBox)
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text.Contains("Hello World"), "Expected SelectedText to Contain [Hello World] Actual [" +
                        _controlWrapper.SelectionInstance.Text + "]", true);
            }
            else
            {
                Verifier.Verify(_controlWrapper.SelectionInstance.Text == "lo Wo", "Expected SelectedText [lo Wo] Actual [" +
                    _controlWrapper.SelectionInstance.Text + "]", true);
            }
            QueueDelegate(RegisterMouseHandler);
        }

        #endregion AcrossWordsShiftDoubleClickLTR.

        #region ChangingContentOnMouseUpEvent.

        private void RegisterMouseHandler()
        {
            Log("\r\n**************LTR  Shift double  click to right across words*************\r\n");
            _element.PreviewMouseUp += new MouseButtonEventHandler(_element_PreviewMouseUp);
            //_element.Focus();
            ((TextBoxBase)_element).FlowDirection = FlowDirection.LeftToRight;
            KeyboardInput.TypeString("^{HOME}{RIGHT 3}");
            QueueDelegate(MouseClickToTriggerUPEvent);
        }

        void _element_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            _controlWrapper.Text = _updatedString;
        }

        private void MouseClickToTriggerUPEvent()
        {
            MouseInput.MouseClick(_element);
            QueueDelegate(VerifyTextChangedInHandler);

        }

        private void VerifyTextChangedInHandler()
        {
            string str = _controlWrapper.Text;
            str = str.Replace("\r\n","");
            Verifier.Verify(str == _updatedString, "Expected ["+ _updatedString +"] Actual ["+ str+"]",true);
            QueueDelegate(NextCombination);
        }

        #endregion ChangingContentOnMouseUpEvent.

        #region Helpers.

        private void GetPointFromRequiredPosition(ref Point p,  int directionIndex)
        {
            if (_element is TextBox)
            {
                int index = ((TextBox)_element).CaretIndex;
                Rect _rect = _controlWrapper.GetGlobalCharacterRect(index + directionIndex);
                p.X = _rect.X;
                p.Y = _rect.Bottom;
            }
            else
            {
                TextPointer tp = ((RichTextBox)_element).CaretPosition;
                bool _forwardDirection = (directionIndex > 0) ? true : false;
                directionIndex = Math.Abs(directionIndex);
                for (int i = 0; i < directionIndex; i++)
                {
                    LogicalDirection ld = (_forwardDirection == true) ? LogicalDirection.Forward : LogicalDirection.Backward;
                    tp = tp.GetNextInsertionPosition(ld);
                }
                Rect _rect = _controlWrapper.GetGlobalCharacterRect(tp);
                Log(_rect.Left.ToString());
                p.X = _rect.X;
                p.Y = _rect.Bottom;
            }
        }

        #endregion Helpers.

        #region data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;

        private string _updatedString = "text changed";

        #endregion data.
    }

    /// <summary>
    /// Test selection using MouseClick + MouseWheel scroll.
    /// This test case is blocked by Regression_Bug98 which is right now scheduled for V2
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("420"), TestBugs("468,98")]
    public class MouseWheelScrollSelectionTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Starts the combination</summary>
        protected override void DoRunCombination()
        {
            _tbb = (TextBoxBase)_editableType.CreateInstance();
            _tbb.Height = 100;
            _tbb.Width = 150;
            _tbb.AcceptsReturn = true;

            _wrapper = new UIElementWrapper(_tbb);
            _wrapper.Text = "            ";

            TestElement = _tbb;

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _tbb.Focus();
            _tbb.SelectAll();

            QueueDelegate(GetSelectionForegroundColor);
        }

        private void GetSelectionForegroundColor()
        {
            SysDrawing.Color selectionDrawingColor;
            TextPointer tp = _wrapper.Start.GetPositionAtOffset(_wrapper.Text.Length / 2);
            Rect rect = tp.GetCharacterRect(LogicalDirection.Forward);
            _elementBitmap = BitmapCapture.CreateBitmapFromElement(_tbb);
            _selectionCaptureRect = new Rect(rect.X, rect.Y + rect.Height / 2, 1, 1);
            _selectionColorBitmap = BitmapUtils.CreateSubBitmap(_elementBitmap, _selectionCaptureRect);

            Logger.Current.LogImage(_selectionColorBitmap, "SelectionColorBitmap");
            selectionDrawingColor = _selectionColorBitmap.GetPixel(0, 0);
            _selectionColor = Color.FromArgb(selectionDrawingColor.A, selectionDrawingColor.R,
                selectionDrawingColor.G, selectionDrawingColor.B);
            Log("Selection Color: " + _selectionColor.ToString());

            //fill content
            _wrapper.Text = TextUtils.RepeatString("This is a test for MouseClick+MouseWheelScroll selection\r\n", 25);

            //dismiss the current selection
            MouseInput.MouseClick(_tbb);

            QueueDelegate(LeftClickDown);
        }

        private void LeftClickDown()
        {
            Log("MouseDown...");
            MouseInput.MouseDown();
            QueueDelegate(MouseScrollWheel1);
        }

        private void MouseScrollWheel1()
        {
            Log("MouseWheelScroll 1st time...");
            //-ve value scrolls the wheel down towards the user
            MouseInput.MouseWheel(-10);
            QueueDelegate(MouseScrollWheel2);
        }

        private void MouseScrollWheel2()
        {
            Log("MouseWheelScroll 2nd time...");
            MouseInput.MouseWheel(-10);
            QueueDelegate(LeftClickUp);
        }

        private void LeftClickUp()
        {
            Log("MouseUp...");
            MouseInput.MouseUp();

            QueueDelegate(TestSelectionVisible);
        }

        private void TestSelectionVisible()
        {
            int selectionColorPixelCount, totalPixelCount;

            Verifier.Verify(!_wrapper.SelectionInstance.IsEmpty, "Selection should be non-empty", true);

            _testSelectionBitmap = BitmapCapture.CreateBitmapFromElement(_tbb);
            Logger.Current.LogImage(_testSelectionBitmap, "TestSelectionColorBitmap");

            selectionColorPixelCount = BitmapUtils.CountColoredPixels(_testSelectionBitmap, _selectionColor);
            totalPixelCount = _testSelectionBitmap.Height * _testSelectionBitmap.Width;
            Log("Pixel count with expected selection color: " + selectionColorPixelCount);
            Log("White pixel count: " + BitmapUtils.CountColoredPixels(_testSelectionBitmap, Brushes.White.Color));
            Log("Black pixel count: " + BitmapUtils.CountColoredPixels(_testSelectionBitmap, Brushes.Black.Color));
            Log("Total pixel count: " + totalPixelCount);

            Verifier.Verify(selectionColorPixelCount > (0.10 * totalPixelCount),
                "selection color pixel count should be atleast 10% of the bitmap capture", true);

            QueueDelegate(NextCombination);
        }

        #endregion Main flow

        #region Private fields

        private TextBoxBase _tbb;
        private UIElementWrapper _wrapper;

        /// <summary>TextEditableType being tested</summary>
        private TextEditableType _editableType=null;

        private SysDrawing.Bitmap _elementBitmap,_selectionColorBitmap,_testSelectionBitmap;
        private Rect _selectionCaptureRect;
        private Color _selectionColor;

        #endregion Private fields
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional testing of TextPattern for TextBox.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 17 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/Interactive/TreeNavigator.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Threading;
    using System.Windows;
    using System.Windows.Automation;
    using System.Windows.Automation.Text;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;
    using Test.Uis.Data;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using AutoText = System.Windows.Automation.Text;
    using System.Diagnostics;

    #endregion Namespaces.

    /// <summary>Base class for TextPattern test cases. Has some useful functions.</summary>
    public abstract class TextPatternTestBase : CustomTestCase
    {
        /// <summary>
        /// Gets the TextPattern object of a text control with specified Name among
        /// descendants of the RootElement.
        /// </summary>
        /// <param name="controlID">Name to look for</param>
        /// <returns>TextPattern instance.</returns>
        public TextPattern GetTextPattern(string controlID)
        {
            AutomationElement autoElement;  // Automation element found.
            AutomationElement root;         // Automation to start search from.
            TextPattern testTextPattern;    // Text pattern to return.
            PropertyCondition condition;    // Control Name condition.
            if (controlID == null)
            {
                throw new ArgumentNullException("controlID");
            }

            Process currentProcess = Process.GetCurrentProcess();
            if (currentProcess == null)
            {
                throw new Exception("Process EditingTest not found");
            }
            if (currentProcess.MainWindowHandle != IntPtr.Zero)
            {
                root = AutomationElement.FromHandle(currentProcess.MainWindowHandle);
            }
            else
            {
                root = AutomationElement.RootElement;
            }
            condition = new PropertyCondition(AutomationElement.AutomationIdProperty, controlID);
            autoElement = root.FindFirst(TreeScope.Descendants, condition);
            object patternObject;
            autoElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObject);
            testTextPattern = (TextPattern) patternObject;

            return testTextPattern;
        }

        /// <summary>Gets the TextPattern object of a text control.</summary>
        /// <param name="control">text control for which TextPattern object is needed.</param>
        /// <returns>TextPattern object</returns>
        public TextPattern GetTextPattern(UIElement control)
        {
            AutomationElement autoElement;
            TextPattern testTextPattern;

            if (control == null)
            {
                throw new ArgumentNullException("control");
            }

            autoElement = AutomationUtils.GetAutomationElement(control);
            object patternObject;
            autoElement.TryGetCurrentPattern(TextPattern.Pattern, out patternObject);
            testTextPattern = (TextPattern) patternObject;

            return testTextPattern;
        }

        /// <summary>Verifies the text in TextPattern</summary>
        /// <param name="tp">TextPattern object to be tested</param>
        /// <param name="expText">Expected test</param>
        /// <returns>true if text is same as expected</returns>
        public bool VerifyTextInTextPattern(TextPattern tp, string expText)
        {
            System.Windows.Automation.Text.TextPatternRange docRange = tp.DocumentRange;
            string actualText = docRange.GetText(expText.Length);
            Log("Text from TextPattern: [" + actualText + "]");
            Log("Expected text: [" + expText + "]");
            if (actualText != expText)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>Verifies some properties of TextPattern</summary>
        /// <param name="tp">TextPattern to test</param>
        /// <param name="supportsTextSelection">Expected value for SupportTextSelection property</param>
        /// <param name="isReadOnly">Expected value for IsReadOnly property</param>
        /// <param name="isPasswordField">Expected value for IsPasswordField property</param>
        /// <param name="nativeObjectModel">If true then NativeObjectModel property shouldnt return null</param>
        /// <returns>Returns true if the properties have right value</returns>
        public bool VerifyPropInTextPattern(TextPattern tp, bool supportsTextSelection, bool isReadOnly,
            bool isPasswordField, bool nativeObjectModel)
        {
            if (tp.SupportedTextSelection == SupportedTextSelection.None)
            {
                Log("Expected SupportsTextSelection property value: [" + supportsTextSelection + "]");
                Log("Actual SupportsTextSelection property value: [" + tp.SupportedTextSelection + "]");
                return false;
            }
            // These properties moved to AutomationElement
            /* if (tp.IsReadOnly != isReadOnly)
            {
                Log("Expected IsReadOnly property value: [" + isReadOnly + "]");
                Log("Actual IsReadOnly property value: [" + tp.IsReadOnly + "]");
                return false;
            }
            if (tp.IsPasswordField != isPasswordField)
            {
                Log("Expected IsReadOnly property value: [" + isPasswordField + "]");
                Log("Actual IsReadOnly property value: [" + tp.IsPasswordField + "]");
                return false;
            } */
            /*
            if (nativeObjectModel)
            {
                if (tp.NativeObjectModel == null)
                {
                    Log("NativeObjectModel property value is null when it is not expected");
                    return false;
                }
            }
            */
            return true;
        }
    }

    /// <summary>
    /// Runs a scenario type test case on TextPattern. This test tries to get the TextPattern object
    /// of a TextBox which is in a different process
    /// </summary>
    [Test(2, "Controls", "TextPatternTestScenario", MethodParameters = "/TestCaseType:TextPatternTestScenario", Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("377"), TestTitle("TextPatternTestScenario")]
    public class TextPatternTestScenario : TextPatternTestBase
    {
        TextPattern _testTextPattern;
        System.Diagnostics.Process _secondProcess;
        const string controlID1 = "TextBox1";
        const string controlID2 = "TextBox2";
        const string controlText1 = "This is TextBox1";
        const string controlText2 = "This is a multiple-line text box: TextBox2" + "\r\n" +
                                "This is a multiple-line text box: TextBox2";
        const int waitSeconds = 5;
        bool _testFail = false;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _secondProcess = LaunchControlApp("TextBoxApp");
            QueueHelper.Current.QueueDelayedDelegate(new System.TimeSpan(0, 0, 0, waitSeconds),
                new SimpleHandler(Step1), new object[] {});
        }

        /// <summary>Step1: Gets the textpattern object and verifies whether it has the right text.</summary>
        private void Step1()
        {
            Log("Trying to get TextPattern for TextBox1...");
            _testTextPattern = GetTextPattern(controlID1);
            VerifyTextPattern(_testTextPattern, controlText1, true, false, false, true);

            Log("Trying to get TextPattern for TextBox1...");
            _testTextPattern = GetTextPattern(controlID2);
            VerifyTextPattern(_testTextPattern, controlText2, true, false, false, true);

            Log("Waiting for the second process to exit");
            _secondProcess.WaitForExit();
            /*bool exitPass = secondProcess.WaitForExit(2*waitSeconds);
            if (!exitPass)
            {
                Log("ControlApp didnt finish");
                if (!secondProcess.HasExited)
                {
                    Log("Killing the ControlApp");
                    //ShutDownControlApp();
                    secondProcess.Kill();
                }
            }*/
            if (_testFail)
            {
                Logger.Current.ReportResult(false, "Test failed", false);
            }
            else if (!Logger.Current.ProcessLog("TextPatternTest.txt"))
            {
                Logger.Current.ReportResult(false, "Test failed: ControlApp failed", false);
            }
            else
            {
                Logger.Current.ReportSuccess();
            }
        }

        /// <summary>Cleanup function. We need to kill the 2nd process.</summary>
        private void ShutDownControlApp()
        {
            _secondProcess.Kill();
        }

        /// <summary>Verifies that the TextPattern has the right properties</summary>
        /// <param name="tp">TextPattern object to be tested</param>
        /// <param name="expText">Expected test</param>
        /// <param name="supportsTextSelection">Expected SupportsTextSelection property value</param>
        /// <param name="isReadOnly">Expected IsReadOnly property value</param>
        /// <param name="isPasswordField">Expected IsPasswordField property value</param>
        /// <param name="nativeObjectModel">Expected NativeObjectModel property value</param>
        private void VerifyTextPattern(TextPattern tp, string expText, bool supportsTextSelection, bool isReadOnly,
            bool isPasswordField, bool nativeObjectModel)
        {
            if (VerifyTextInTextPattern(tp, expText)==false)
            {
                _testFail = true;
                ShutDownControlApp();
                Logger.Current.ReportResult(false, "Text from TextPattern didnt match the actual text in TextBox", false);
            }

            if (VerifyPropInTextPattern(tp, supportsTextSelection, isReadOnly, isPasswordField, nativeObjectModel) == false)
            {
                _testFail = true;
                ShutDownControlApp();
                Logger.Current.ReportResult(false, "TextPattern didnt have the right property values.", false);
            }
        }

        /// <summary>Launches a separate process which has two Textbox.</summary>
        /// <param name="testCaseType">TestCaseType argument for the main exe.</param>
        /// <returns>process launched.</returns>
        private static System.Diagnostics.Process LaunchControlApp(string testCaseType)
        {
            string processArguments;
            System.Diagnostics.Process process;
            const string processFileName = "EditingTest.exe";            

            if (testCaseType == null)
            {
                throw new ArgumentNullException("testCaseType");
            }

            if (testCaseType.Length == 0)
            {
                throw new ArgumentException("Type name cannot be blank.", "testCaseType");
            }

            // Launch the process.
            Logger.Current.Log("Preparing and loading second app with TextBox...");            
            processArguments = "/TestCaseType=" + testCaseType;
            Logger.Current.Log("Calling [" + processFileName + " " + processArguments + "]");
            process = System.Diagnostics.Process.Start(processFileName, processArguments);

            // Let piper monitor process so it can be cleaned up if process hang.
            //Piper is not there anymore, we need to see if the new testframe work has anything for this
            //Test.Uis.Utils.ConfigurationSettings.Current.AutomationFramework.MonitorProcess(process);
            
            return process;
        }
    }

    /// <summary>Runs an app which has two textboxes.</summary>
    [TestOwner("Microsoft"), TestTitle("TextBoxApp")]
    [Test(2, "TextBox", "TextBoxApp", MethodParameters = "/TestCaseType:TextBoxApp")]
    public class TextBoxApp : CustomTestCase
    {
        DockPanel _panel;
        TextBox _tb1,_tb2;
        const string textBoxID1 = "TextBox1";
        const string textBoxID2 = "TextBox2";
        const string textBoxText1 = "This is TextBox1";
        const string textBoxText2 = "This is a multiple-line text box: TextBox2" + "\r\n" +
                                "This is a multiple-line text box: TextBox2";
        const int waitSeconds = 10;

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            //Create and open log file for recording log
            if (Test.Uis.IO.TextFileUtils.Exists("TextPatternTest.txt"))
                Test.Uis.IO.TextFileUtils.Delete("TextPatternTest.txt");

            Logger.Current.LogToFile("TextPatternTest.txt");

            _panel = new DockPanel();
            _tb1 = new TextBox();
            _tb2 = new TextBox();
            _tb1.Name = textBoxID1;
            _tb2.Name = textBoxID2;
            _tb1.Text = textBoxText1;
            _tb2.Text = textBoxText2;

            _panel.Children.Add(_tb1);
            _panel.Children.Add(_tb2);

            MainWindow.Width = 400;
            MainWindow.Height = 400;
            MainWindow.Content = _panel;

            Log("Launched TextBoxApp...");

            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 0, waitSeconds),
                new SimpleHandler(EndTestCase), new object[] {});
        }

        /// <summary>Ends the test case</summary>
        private void EndTestCase()
        {
            Log("Closing TextBoxApp");
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that changes in the TextOM are reflected in the associated
    /// TextPattern, and that every API in the TextPattern and TextPatternRange
    /// classes returns a valid result for every control.
    /// </summary>
    [TestOwner("Microsoft"), TestBugs("566,758,759,760,761"), TestWorkItem("45")]
    [Test(2, "TextPattern", "TextPatternInteractionTest", MethodParameters = "/TestCaseType:TextPatternInteractionTest")]
    public class TextPatternInteractionTest: TextPatternTestBase
    {
        #region Main flow.

        /// <summary>Automation element for control under test.</summary>
        private AutomationElement _automationElement;

        /// <summary>Array of types to be tested.</summary>
        private static Type[] s_typesToTest = new Type[] {
            typeof(TextBox), typeof(RichTextBox)
        };

        /// <summary>Index of type being tested.</summary>
        private int _typeToTestIndex;

        /// <summary>Instance being tested.</summary>
        private UIElement _element;

        /// <summary>Wrapper around instance being tested.</summary>
        private UIElementWrapper _wrapper;

        // <summary>Pattern interface for tested element.</summary>
        private TextPattern _pattern;

        private object _patternObject;

        private string _wrapperSelectedText;
        private string _wrapperText;
        private UIElement _child;                        // Child element to get range for.
        private bool _hasRichText = false;
        private Point _childPoint;                       // Screen-relative point falling within child.
        private Point _contentPoint;     // Point with content in control.
        private TextPointer _cursor; // Pointer to move through the content.
        private string _wrapString = "Sample phrase to repeat. Sample phrase to repeat. Sample phrase to repeat. Sample phrase to repeat. Sample phrase to repeat.";
        private string _wrapEnabledVisibleText;
        private delegate void SingleArgDelegateHandler(object o);

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _typeToTestIndex = 0;
            QueueDelegate(TestType);
        }

        /// <summary>Tests the currently indexed type.</summary>
        /// <remarks>
        /// Finishes the test successfully if all types have been tested.
        /// </remarks>
        private void TestType()
        {
            Type type;  // Type to be tested.

            if (_typeToTestIndex >= s_typesToTest.Length)
            {
                Logger.Current.ReportSuccess();
                return;
            }

            type = s_typesToTest[_typeToTestIndex];
            Log("Test Iteration " + _typeToTestIndex + " - " + type);

            _element = (UIElement) Activator.CreateInstance(type);
            _wrapper = new UIElementWrapper(_element);
            MainWindow.Content = _element;
            SetupElementContent();

            QueueDelegate(AfterRender);
        }

        /// <summary>Sets up interesting content for the element.</summary>
        private void SetupElementContent()
        {
            _wrapper.Wrap = true;
            if (_wrapper.IsElementRichText)
            {
                // 
                _wrapper.XamlText =
                    "<Paragraph>" + _wrapString + "</Paragraph>" +
                    "<Paragraph Foreground='Red'>red</Paragraph>" +
                    "<Paragraph>  <Bold>bold <Italic>italic</Italic></Bold></Paragraph>" +
                    "<List><ListItem><Paragraph>item</Paragraph></ListItem></List>" +
                    "<Paragraph TextAlignment='Center'><Bold><TextBox Name='InnerTextBox'>My text box</TextBox></Bold></Paragraph>" +
                    "<Paragraph>" + _wrapString + "</Paragraph>";
            }
            else
            {
                _wrapper.Text = TextScript.Hebrew.Sample + "\r\n" +
                    StringData.MixedScripts.Value;
            }
        }

        private void AfterRender()
        {
            _element.Focus();
            QueueDelegate(TestPatternProperties);
        }

        /// <summary>Verifies the properties on the text pattern..</summary>
        private void TestPatternProperties()
        {
            _element.Focus();
            if (!_element.IsKeyboardFocused)
            {
                throw new Exception("Element has not received focus.");
            }

            CallFunctionOnThread(GetAutomationElementFocusedElement, VerifyTextPatternProperties);
        }

        private void VerifyTextPatternProperties()
        {
            // Get the Automation element and the TextPattern for it.
            if (_automationElement == null)
            {
                throw new Exception("AutomationElement.FocusedElement is null " +
                    " - expected tested control.");
            }
            
            Log("VerifyTextPatternProperties");
            
            _pattern = _patternObject as TextPattern;
            if (_pattern == null)
            {
                throw new Exception("TextPattern not available for tested control: " +
                    _automationElement.Current.ClassName);
            }

            // Methods:
            // - GetSelection
            // - GetVisibleRange
            // - RangeFromChild
            // - RangeFromPoint

            // Properties:
            // - DocumentRange
            // - SupportsTextSelection
            _wrapper.SelectAll();
            _wrapperSelectedText = _wrapper.GetSelectedText(false, false);
            CallFunctionOnThread(TestGetSelection, TestGetVisibleRange);
        }

        private void TestGetSelection()
        {
            Log("Testing TextPattern.GetSelection...");

            Log("Text selection supported: " + _pattern.SupportedTextSelection);
            if (_pattern.SupportedTextSelection != SupportedTextSelection.None)
            {
                VerifySelection(_pattern);
            }
        }

        private void TestGetVisibleRange()
        {
            // The control setup should take care of this.
            Verifier.Verify(_wrapper.Wrap, "Wrap is enabled.", false);
            _wrapperText = _wrapper.Text;
            CallFunctionOnThread(VerifyVisibleRange, TestRangeFromChild);
        }

        private void VerifyVisibleRange()
        {
            string contentPrefix;                   // First characters in content.
            string visiblePrefix;                   // First characters in visible content.
            AutoText.TextPatternRange visibleRange; // Range of visible content.

            Log("Testing TextPattern.GetVisibleRange...");
            AutoText.TextPatternRange[] visibleRanges = _pattern.GetVisibleRanges();
            if (visibleRanges == null || visibleRanges.Length == 0)
            {
                throw new Exception("VisibleRange unavailable.");
            }
            visibleRange = visibleRanges[0];
            _wrapEnabledVisibleText = visibleRange.GetText(Int32.MaxValue);

            contentPrefix = _wrapperText.Substring(0, 3);
            visiblePrefix = _wrapEnabledVisibleText.Substring(0, 3);

            //




            Verifier.Verify(visibleRange.TextPattern == _pattern,
                "Visible range has original pattern as TextPattern.", true);
        }

        private void TestRangeFromChild()
        {
            if (_wrapper.IsElementRichText)
            {
                _hasRichText = true;
                Log("Looking for inner text box...");
                _child = TextUtils.FindElementInText(_wrapper.Start, "InnerTextBox");
                _childPoint = ElementUtils.GetScreenRelativePoint(_child, new Point(2, 2));
                if (_child == null)
                {
                    throw new Exception("Unable to get inner text box from control.");
                }
            }
            CallFunctionOnThread(VerifyTextRangeFromChild, TestRangeFromPoint);
        }

        private void VerifyTextRangeFromChild()
        {
            bool exceptionThrown;

            Log("Verifying that unknown children don't cause problems.");
            exceptionThrown = true;
            try
            {
                _pattern.RangeFromChild(_automationElement);
                exceptionThrown = false;
            }
            catch (InvalidOperationException ex)
            {
                Log(ex.ToString());
            }

            if (!exceptionThrown)
            {
                throw new ApplicationException(
                    "TextPattern accepts itself as child element.");
            }

            if (_hasRichText)
            {
                // AutoText.TextPatternRange childRange;   // Range with embedded object.
                AutomationElement childElement;         // AutomationElement for child.
                
                childElement = AutomationElement.FromPoint(_childPoint);
                if (childElement == null)
                {
                    throw new Exception("Unable to get inner text box from point.");
                }

                TextPatternRange childRange = _pattern.RangeFromChild(childElement);
                if (childRange == null)
                {
                    throw new Exception("Unable to get range for inner child.");
                }
            }
        }

        private void TestRangeFromPoint()
        {
            _contentPoint = ElementUtils.GetScreenRelativePoint(
                            _wrapper.Element, new Point(10, 10));
            CallFunctionOnThread(VerifyTextRangeFromPoint, TestAttributes);
        }

        private void VerifyTextRangeFromPoint()
        {
            bool exceptionThrown;   // Whether an exception has been thrown.
            AutoText.TextPatternRange range;    // Range with content.
            int rangeLength;                    // Length of text range.

            Log("Verifying RangeFromPoint for invalid point (-1,-1)...");
            exceptionThrown = false;
            try
            {
                _pattern.RangeFromPoint(new Point(-1, -1));
            }
            catch(Exception)
            {
                exceptionThrown = true;
            }
            Verifier.Verify(exceptionThrown, "Exception thrown as expected.", true);

            
            Log("Querying for range at point: " + _contentPoint);
            range = _pattern.RangeFromPoint(_contentPoint);
            Verifier.Verify(range != null, "Range is not null.", true);

            rangeLength = range.GetText(-1).Length;
            Verifier.Verify(rangeLength == 0,
                "Range length [" + rangeLength + "] is of a single character.", true);
        }

        private void TestAttributes()
        {
            Log("Verifying attributes at each position...");
            if (_wrapper.IsElementRichText)
            {
                _cursor = _wrapper.Start;
                CallFunctionOnThread(VerifyAttributesForRichText, TestRanges);
            }
            else
            {
                CallFunctionOnThread(VerifyAttributesForPlainText, TestRanges);
            }
        }

        private void TestRanges()
        {
            // Get a valid range on the document.
            // Clone the range and move to a different location.
            // Verify Clone() and Move worked by comparing ranges.

            // Verify ExpandToEnclosingUnit.
            // Verify that FindAttribute can find a bold range.
            // Verify that FindText can find a string in the text in a single element.
            // Verify that FindText can find a string in the text across formatting elements.
            // Verify that FindText cannot find a string in the text across block elements.
            // Verify all values on the range, and verify that GetAttributeValue matches those.
            // Verify that GetBoundingRectangles returns the bounding lines for the rendered lines.
            // Verify that GetEnclosingElement works correctly in an element, ouside of an element, and between elements.
            // Verify that GetText returns the correct plain text representation.
            // Verify that MoveEndPoint moves a single edge, for cases where the end is in the middle of a run of text, between elements, and at the end of the container.
            // Verify that a range can be scrolled into view, for vertical and horizontal spans, for partial ranges, and for too-large ranges. Verify that a range that is in view causes no scrolling.
            // Verify that an arbitrary range can be selected.

            QueueDelegate(ElementTested);
        }

        private void ElementTested()
        {
            _typeToTestIndex++;
            QueueDelegate(TestType);
        }

        #endregion Main flow.

        #region Helper methods.

        private void CallFunctionOnThread(SimpleHandler functionDelegate, SimpleHandler functionCallback)
        {
            SimpleHandler[] functionHandlers = new SimpleHandler[2] { functionDelegate, functionCallback };
            Thread autoThread = new Thread(new ParameterizedThreadStart(StartThread));
            autoThread.SetApartmentState(System.Threading.ApartmentState.STA);
            autoThread.Start(functionHandlers as object);
        }

        private void StartThread(object arg)
        {
            SimpleHandler[] functionHandlers = arg as SimpleHandler[];
            functionHandlers[0]();//actual function
            QueueHelper helper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);
            helper.QueueDelegate(functionHandlers[1]);//callback function
        }

        private void GetAutomationElementFocusedElement()
        {
            try
            {
                _automationElement = AutomationElement.FocusedElement;
            }
            catch (Exception e)
            {
                Log("Unable to get the AutomationElement.FocusedElement. Exception thrown: " + e.ToString());
                throw;
            }
            _automationElement.TryGetCurrentPattern(TextPattern.Pattern, out _patternObject);
        }  

        /// <summary>
        /// Verify that all positions match the attributes for the control.
        /// </summary>
        private void VerifyAttributesForPlainText()
        {
            AutoText.TextPatternRange range;
            range = _pattern.DocumentRange;
            Verifier.Verify(range.GetText(Int32.MaxValue).Equals(_wrapperText),"Text should match", false);
            //Range.GetAttributeValues API doesnt exist anymore
        }

        /// <summary>
        /// Verify that all positions match the attributes at each position
        /// on the rich text control.
        /// </summary>
        private void VerifyAttributesForRichText()
        {
            AutoText.TextPatternRange range;
            range = _pattern.DocumentRange;

            Log("Collapsing text to start...");
            // 
            range.MoveEndpointByUnit(AutoText.TextPatternRangeEndpoint.End,
                AutoText.TextUnit.Character, Int32.MinValue + 1);
            VerifyEmptyRange(range);

            /*
            while (cursor.GetPointerContext(LogicalDirection.Forward) != TextPointerContext.None)
            {
                foreach (AutomationTextAttributeData data in AutomationTextAttributeData.Values)
                {
                    data.VerifyPointerValue(range, cursor);
                }

                Verifier.Verify((cursor = cursor.GetNextContextPosition(LogicalDirection.Forward)) != null,
                    "Cursor moved forward.", false);
                // Update range.
            }
             */
        }
       
        /// <summary>Verifies that the specified range is empty.</summary>
        private void VerifyEmptyRange(AutoText.TextPatternRange range)
        {
            string text;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            text = range.GetText(-1);
            if (text.Length > 0)
            {
                throw new Exception("Expected empty range, found text [" +
                    text + "]");
            }
        }

        /// <summary>Verifies that the selection in the given pattern and wrapper match.</summary>
        private void VerifySelection(TextPattern pattern)
        {
            AutoText.TextPatternRange selectionRange;
            string selectedText;

            AutoText.TextPatternRange [] selectionRanges = pattern.GetSelection();
            if (selectionRanges == null || selectionRanges.Length == 0)
            {
                throw new Exception("Selection range is unavailable.");
            }
            selectionRange = selectionRanges[0];

            selectedText = selectionRange.GetText(int.MaxValue);
            if (_wrapperSelectedText.EndsWith("\r\n"))
            {
                _wrapperSelectedText = _wrapperSelectedText.Substring(0, _wrapperSelectedText.Length - 2);
            }
            Verifier.Verify(selectedText == _wrapperSelectedText,
                "Automation selected text [" + selectedText +
                "] matches element selected text [" + _wrapperSelectedText +
                "].", true);
        }

        #endregion Helper methods.
    }
}

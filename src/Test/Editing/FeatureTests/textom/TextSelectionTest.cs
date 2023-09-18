// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API for TextSelection class

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Runtime.Remoting;
    using System.Threading; using System.Windows.Threading;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Base class for TextSelection test cases.
    /// </summary>
    public abstract class TextSelectionTestBase : CustomTestCase
    {
        #region Constructors.
        
        /// <summary>Initializes a new TextSelectionTestBase instance.</summary>
        public TextSelectionTestBase() : base()
        {
            string xamlPage = ConfigurationSettings.Current.GetArgument("XamlPage");

            this._useXaml = !String.IsNullOrEmpty(xamlPage);
            if (this._useXaml)
            {
                base.StartupPage = xamlPage;
            }
        }
        
        #endregion Constructors.
        
        #region Public methods.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase ()
        {
            TextBox textBox = null;

            if (this._useXaml)
            {
                textBox = ElementUtils.FindElement(MainWindow, "TextBox1") as TextBox;
            }
            else
            {
                textBox = new TextBox();
                MainWindow.Content = textBox;
            }

            this._elementWrapper = new UIElementWrapper(textBox);

            QueueHelper.Current.QueueDelayedDelegate (new System.TimeSpan (0, 0, 0, 3), new SimpleHandler(OnAdded), new object[] {});
        }
        
        #endregion Public methods.
        
        #region Protected methods.
        
        /// <summary>
        /// Emulates to deprecated TextSelection.InputText method.
        /// </summary>
        /// <param name='selection'>Selection to insert text into.</param>
        /// <param name='textToInput'>Text to be inserted.</param>
        protected static void DoInputText(TextSelection selection, string textToInput)
        {
            if (selection == null)
            {
                throw new ArgumentNullException("selection");
            }
            if (textToInput == null)
            {
                throw new ArgumentNullException("textToInput");
            }
            
            selection.Text = textToInput;
            selection = selection.End;
        }

        /// <summary>Tests the Selection object.</summary>
        protected abstract void TestTextSelectionProperties ();

        #endregion Protected methods.
        
        #region Protected data.

        /// <summary>TextSelection object to be tested.</summary>
        protected TextSelection Selection
        {
            get { return this._textSelection; }
        }

        /// <summary>UIElementWrapper for the element being tested.</summary>
        protected UIElementWrapper Wrapper
        {
            get { return _elementWrapper; }
        }
        
        #endregion Protected data.

        #region Private methods.

        private void OnAdded ()
        {
            MouseInput.MouseClick(this._elementWrapper.Element);
            QueueDelegate(PreTestTextSelectionProperties);
        }

        private void PreTestTextSelectionProperties ()
        {
            this._textSelection = this._elementWrapper.SelectionInstance;

            Verifier.Verify (this._textSelection != null, "TextSelection can be obtained from the element", true);

            TestTextSelectionProperties ();
        }
        
        #endregion Private methods.
        
        #region Private data.

        private TextSelection _textSelection = null;

        private UIElementWrapper _elementWrapper = null;
        private bool _useXaml = false;
        
        #endregion Private data.
    }

    /// <summary>
    /// Verifies that the TextSelection.AppendString method works as expected.
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectionAppendString")]
    public sealed class TextSelectionAppendString : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string appendString;            // String to be appended.
            string inputTextString;         // Initial string.
            int selectionStartDisplacement; // Displacement for start of selection.
            int selectionEndDisplacement;   // Displacement for end of selection.
            TextContainer textContainer;    // Container being tested.
            TextPointer nvStart;          // Start navigator to move to.
            TextPointer nvEnd;            // End navigator to move to.
            
            inputTextString = Settings.GetArgument("InitialTextString");
            selectionStartDisplacement = Settings.GetArgumentAsInt("SelectionStartDisplacement");
            selectionEndDisplacement = Settings.GetArgumentAsInt("SelectionEndDisplacement");

            DoInputText(Selection, inputTextString);
            
            textContainer = base.Selection.Start.TextContainer;
            nvStart = textContainer.Start.CreateNavigator();
            nvEnd = textContainer.Start.CreateNavigator();

            nvStart.MoveByDistance(selectionStartDisplacement);
            nvEnd.MoveByDistance(selectionEndDisplacement);
            base.Selection.MoveToPositions(nvStart, nvEnd);

            appendString = ConfigurationSettings.Current.GetArgument ("AppendString");

            //
            // make the test call
            //
            base.Selection.AppendText(appendString);

            QueueDelegate(OnAppend);
        }

        private void OnAppend()
        {
            // TO-DO: need visual verification and verification of text in element

            string expectedFinalSelectedString = ConfigurationSettings.Current.GetArgument("ExpectedFinalSelectedString");
            string finalSelectedString = base.Wrapper.GetSelectedText(false, false);

            string output = String.Format ("Final selected text [{0}], expected final selected text [{1}]", 
                   finalSelectedString,
                   expectedFinalSelectedString);

            Verifier.Verify(finalSelectedString == expectedFinalSelectedString, output, true);
            Logger.Current.ReportSuccess ();
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.Start and TextSelection.End properties
    /// work as expected.
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectiongetStartEnd")]
    public sealed class TextSelectiongetStartEnd : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string keystrokeString = ConfigurationSettings.Current.GetArgument ("KeystrokeString");

            KeyboardInput.TypeString (keystrokeString);
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnInputDone));
        }

        private void OnInputDone ()
        {
            string backwardString = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            string forwardString = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Forward);

            string backwardExpectedString = ConfigurationSettings.Current.GetArgument ("BackwardExpectedString");
            string forwardwardExpectedString = ConfigurationSettings.Current.GetArgument ("ForwardExpectedString");

            string output = String.Format ("Backward string[{0}], Expected [{1}]", backwardString, backwardExpectedString);

            Verifier.Verify (backwardExpectedString == backwardString, output, true);

            output = String.Format ("Forward string[{0}], Expected [{1}]", forwardString, forwardwardExpectedString);

            Verifier.Verify (forwardwardExpectedString == forwardString, output, true);

            Logger.Current.ReportSuccess ();
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.Text property returns the
    /// expected value
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectiongetText")]
    public sealed class TextSelectiongetText : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string keystrokeString = ConfigurationSettings.Current.GetArgument ("KeystrokeString");

            KeyboardInput.TypeString (keystrokeString);
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnInputDone));
        }

        private void OnInputDone()
        {
            string text = base.Selection.Text;

            string expectedText = ConfigurationSettings.Current.GetArgument ("ExpectedString");

            string output = String.Format ("String in element [{0}], Expected [{1}]", text, expectedText);

            Verifier.Verify (text == expectedText, output, true);
            Logger.Current.ReportSuccess ();
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.Text property can be
    /// set to a given string.
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectionsetText")]
    public sealed class TextSelectionsetText : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string keystrokeString = ConfigurationSettings.Current.GetArgument ("KeystrokeString");

            KeyboardInput.TypeString (keystrokeString);
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnInputDone));
        }

        private void OnInputDone ()
        {
            string setTextString = ConfigurationSettings.Current.GetArgument ("SetTextString");

            base.Selection.Text = setTextString;

            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnSetText));
        }

        private void OnSetText()
        {
            string expectedText = ConfigurationSettings.Current.GetArgument ("ExpectedString");

            string output = String.Format ("String in element [{0}], Expected [{1}]", base.Wrapper.GetElementText (), expectedText);

            Verifier.Verify (base.Wrapper.GetElementText () == expectedText, output, true);

            Logger.Current.ReportSuccess ();
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.DeleteContent method works as expected.
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectionDeleteContent")]
    public sealed class TextSelectionDeleteContent : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string keystrokeString = ConfigurationSettings.Current.GetArgument ("KeystrokeString");

            KeyboardInput.TypeString (keystrokeString);
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnInputDone));
        }

        private void OnInputDone ()
        {
            base.Selection.DeleteContent ();
            QueueHelper.Current.QueueDelegate (new SimpleHandler (OnTextSelectionDelete));
        }

        private void OnTextSelectionDelete ()
        {
            string expectedText = ConfigurationSettings.Current.GetArgument ("ExpectedString");
            string output = String.Format ("String in element [{0}], Expected [{1}]", base.Wrapper.GetElementText (), expectedText);

            Verifier.Verify (base.Wrapper.GetElementText () == expectedText, output, true);
            Verifier.Verify (String.IsNullOrEmpty (base.Selection.Text), "TextSelection is empty", true);
            Logger.Current.ReportSuccess ();
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.Move event is fired as expected.
    /// </summary>
    [TestOwner ("Microsoft"), TestTitle ("TextSelectionMove")]
    public sealed class TextSelectionMove : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties ()
        {
            string inputTextString = ConfigurationSettings.Current.GetArgument ("InputTextString");

            DoInputText(Selection, inputTextString);

            int moveIndex = ConfigurationSettings.Current.GetArgumentAsInt("MoveIndex");

            int realMoveIndex = 0;

            LogicalDirection direction = LogicalDirection.Forward;

            if (moveIndex < 0)
            {
                direction = LogicalDirection.Backward;
            }
            moveIndex = Math.Abs(moveIndex);

            for (int i = 0; i < moveIndex; i++)
            {
                if (base.Selection.Move(direction))
                {
                    realMoveIndex++;
                }
                else
                {
                    break;
                }
            }

            string output = String.Format("TextPointer should move [{0}], now it moves only [{1}]", moveIndex.ToString(), realMoveIndex.ToString());

            Verifier.Verify(realMoveIndex == moveIndex, output, true);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnTextSelectionMove));
        }

        private void OnTextSelectionMove ()
        {
            string backwardString = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            string forwardString = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Forward);

            string backwardExpectedText = ConfigurationSettings.Current.GetArgument ("BackwardExpectedString");
            string forwardExpectedText = ConfigurationSettings.Current.GetArgument ("ForwardExpectedString");

            string output = String.Format ("Backward string in element [{0}], Expected [{1}]", backwardString, backwardExpectedText);

            Verifier.Verify (backwardString == backwardExpectedText, output, true);

            output = String.Format ("Forward string in element [{0}], Expected [{1}]", forwardString, forwardExpectedText);

            Verifier.Verify (forwardString == forwardExpectedText, output, true);

            Logger.Current.ReportSuccess ();
        }
    }
    
    /// <summary>
    /// Verifies that clicking a TextBox fires a single Moved event.
    /// </summary>
    [TestOwner("Microsoft"),TestTactics("385"),TestBugs("647")]
    public class TextSelectionReproRegression_Bug647: CustomTestCase
    {
        #region Main flow.
        
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _textbox = new TextBox();
            _textbox.Selection.Moved += delegate
            {
                _eventCount++;
            };
            
            Log("Attaching TextBox to tree...");
            MainWindow.Content = _textbox;
            Log("Event count: " + _eventCount);
            
            Log("Setting text...");
            _textbox.Text = "text";
            Log("Event count: " + _eventCount);
            
            QueueHelper.Current.QueueDelegate(ClickTextBox);
        }
        
        private void ClickTextBox()
        {
            Log("TextBox rendered...");
            Log("Event count: " + _eventCount);
            _eventCountBeforeClick = _eventCount;
            MouseInput.MouseClick(_textbox);
            QueueHelper.Current.QueueDelegate(CountEvents);
        }
        
        private void CountEvents()
        {
            Log("TextBox clicked...");
            Log("Event count: " + _eventCount);
            Verifier.Verify(_eventCount ==  _eventCountBeforeClick + 1,
                "Clicking fires a single move event.", true);
            Logger.Current.ReportSuccess();
        }
        
        #endregion Main flow.
        
        #region Private data.
        
        private TextBox _textbox;
        private int _eventCount;
        private int _eventCountBeforeClick;
        
        #endregion Private data.
    }

    /// <summary>
    /// Verifies that the TextSelection can be moved to the specified
    /// position.
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("TextSelectionMoveToPosition")]
    public sealed class TextSelectionMoveToPosition : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties()
        {
            string inputTextString = ConfigurationSettings.Current.GetArgument("InputTextString");

            DoInputText(Selection, inputTextString);

            QueueDelegate(DoMoveToPosition);
        }

        private void DoMoveToPosition()
        {
            TextContainer textContainer = base.Selection.Start.TextContainer;

            int textPositionOffset = ConfigurationSettings.Current.GetArgumentAsInt("TextPointerOffset");
 
            string directionStr = ConfigurationSettings.Current.GetArgument("Direction");
            
            LogicalDirection logicalDirection = LogicalDirection.Forward;

            switch(directionStr)
            {
                case "Backward":
                    logicalDirection = LogicalDirection.Backward;
                    break;
                default:
                    logicalDirection = LogicalDirection.Forward;
                    break;
            }

            TextPointer textPosition = textContainer.Start.CreateNavigator();

            textPosition.MoveByDistance(textPositionOffset);

            //
            // make the test call here
            // remember we can make logicalDirection == LogicalDirection.Forward
            // to test the MoveToPosition overload without logicalDirection param
            //

            base.Selection.MoveToPosition(textPosition, logicalDirection);

            string keystrokeString = ConfigurationSettings.Current.GetArgument("KeystrokeString");

            KeyboardInput.TypeString (keystrokeString);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnKeystroke));

        }

        private void OnKeystroke()
        {
            string expectedSelectedString = ConfigurationSettings.Current.GetArgument("ExpectedSelectedString");

            string selectedString = base.Wrapper.GetSelectedText(false, false);

            string message = String.Format("Selected string [{0}], expected selected string [{1}]", selectedString, expectedSelectedString);

            Verifier.Verify(expectedSelectedString == selectedString, message, true);

            Logger.Current.ReportSuccess();

        }
    }

    /// <summary>
    /// Verifies that the TextSelection can be moved to given positions.
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("TextSelectionMoveToPositions")]
    public sealed class TextSelectionMoveToPositions : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties()
        {
            string inputTextString = ConfigurationSettings.Current.GetArgument("InputTextString");

            DoInputText(Selection, inputTextString);
            QueueDelegate(DoMoveToPositions);
        }


        private void DoMoveToPositions()
        {            
            TextContainer textContainer = base.Selection.Start.TextContainer;
            
            int startTextPointerOffset = ConfigurationSettings.Current.GetArgumentAsInt("StartTextPointerOffset");

            int endTextPointerOffset = ConfigurationSettings.Current.GetArgumentAsInt("EndTextPointerOffset");

            TextPointer startTextPointer = textContainer.Start.CreateNavigator();
            TextPointer endTextPointer = textContainer.Start.CreateNavigator();

            startTextPointer.MoveByDistance(startTextPointerOffset);
            endTextPointer.MoveByDistance(endTextPointerOffset);

            //
            // make the test call here
            //

            base.Selection.MoveToPositions(startTextPointer, endTextPointer);

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMoveToPositons));
        }

        private void OnMoveToPositons()
        {
            string expectedSelectedString = ConfigurationSettings.Current.GetArgument("ExpectedSelectedString");
            string selectedString = base.Wrapper.GetSelectedText(false, false);
            string message = String.Format("Selected string [{0}], expected selected string [{1}]", selectedString, expectedSelectedString);

            Verifier.Verify(expectedSelectedString == selectedString, message, true);
            Logger.Current.ReportSuccess();             
        }
    }

    /// <summary>
    /// Verifies that the TextSelection.MoveToRange method works as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("TextSelectionMoveToRange")]
    public sealed class TextSelectionMoveToRange : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties()
        {
            string stringInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");

            DoInputText(Selection, stringInTextBox);

            QueueDelegate(DoMoveToRange);
        }

        private void DoMoveToRange()
        {
            int textRangeStartOffset = ConfigurationSettings.Current.GetArgumentAsInt("StartTextPointerOffset");
            int textRangeEndOffset = ConfigurationSettings.Current.GetArgumentAsInt("EndTextPointerOffset");

            TextPointer textPositionStart = base.Selection.TextContainer.Start.CreateNavigator();
            TextPointer textPositionEnd = base.Selection.TextContainer.Start.CreateNavigator();

            for (int i = 0; i < textRangeStartOffset; i++)
            {
                textPositionStart.MoveToNextCharacter(LogicalDirection.Forward);
            }

            for (int i = 0; i < textRangeEndOffset; i++)
            {
                textPositionEnd.MoveToNextCharacter(LogicalDirection.Forward);
            }

            bool useNullTextRange = ConfigurationSettings.Current.GetArgumentAsBool("UseNullTextRange");
            
            TextRange textRange = new TextRange(textPositionStart, textPositionEnd);

            // make the test call
            if (useNullTextRange)
            {
                base.Selection.MoveToRange(null);
            }
            else 
            {
                base.Selection.MoveToRange(textRange);
            }

            QueueHelper.Current.QueueDelegate(new SimpleHandler(OnMoveToRange));
        }

        private void OnMoveToRange()
        {
            string expectedStrOnCaretLeft = ConfigurationSettings.Current.GetArgument("ExpectedStrOnCaretLeft");
            string expectedStrOnCaretRight = ConfigurationSettings.Current.GetArgument("ExpectedStrOnCaretRight");

            string strOnCaretLeft = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Backward);
            string strOnCaretRight = base.Wrapper.GetTextOutsideSelection(LogicalDirection.Forward);

            string expectedSelectedString = ConfigurationSettings.Current.GetArgument("ExpectedSelectedString");
            string selectedString = base.Wrapper.GetSelectedText(false, false);

            string message = String.Format("String on caret left [{0}], expected string on caret left [{1}]", strOnCaretLeft, expectedStrOnCaretLeft);
            Verifier.Verify(expectedStrOnCaretLeft == strOnCaretLeft, message);

            message = String.Format("String on caret right [{0}], expected string on caret right [{1}]", strOnCaretRight, expectedStrOnCaretRight);
            Verifier.Verify(expectedStrOnCaretRight == strOnCaretRight, message);

            message = String.Format("Selected string [{0}], expected selected string [{1}]", selectedString, expectedSelectedString);
            Verifier.Verify(expectedSelectedString == selectedString);

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// This test case prepares the TextSelection for AppendEmbeddedObject call
    /// Start and End of TextSelection is repositioned. Element can be specified
    /// in testxml. Mouse down event handler is attached to the new element
    /// and test if that event works. If no mouse down event is received the test
    /// will not exit.
    /// </summary>
    [TestOwner("Microsoft"), TestTitle("TextSelectionAppendEmbeddedObject")]
    public sealed class TextSelectionAppendEmbeddedObject : TextSelectionTestBase
    {
        /// <summary>Runs the test case.</summary>
        protected override void TestTextSelectionProperties()
        {
            string stringInTextBox = ConfigurationSettings.Current.GetArgument("StringInTextBox");

            DoInputText(Selection, stringInTextBox);
            QueueDelegate(DoAppendEmbeddedObject);
        }

        private void DoAppendEmbeddedObject()
        {
            int textRangeStartOffset = ConfigurationSettings.Current.GetArgumentAsInt("StartTextPointerOffset");
            int textRangeEndOffset = ConfigurationSettings.Current.GetArgumentAsInt("EndTextPointerOffset");
            TextPointer textPositionStart = base.Selection.TextContainer.Start.CreateNavigator();
            TextPointer textPositionEnd = base.Selection.TextContainer.Start.CreateNavigator();

            for (int i = 0; i < textRangeStartOffset; i++)
            {
                textPositionStart.MoveToNextCharacter(LogicalDirection.Forward);
            }

            for (int i = 0; i < textRangeEndOffset; i++)
            {
                textPositionEnd.MoveToNextCharacter(LogicalDirection.Forward);
            }

            // move selection to the desired location
            base.Selection.MoveToPositions(textPositionStart, textPositionEnd);

            TextBox textBox = new TextBox();

            string assemblyName = ConfigurationSettings.Current.GetArgument("AssemblyName");

            string typeName = ConfigurationSettings.Current.GetArgument("TypeName");

            ObjectHandle handle = Activator.CreateInstance(assemblyName, typeName);

            string message = String.Format("Object [{0}.[1]] can be created", assemblyName, typeName);
            Verifier.Verify(handle != null, message);

            this._element = handle.Unwrap() as UIElement;

            message = String.Format("Object [{0}.[1]] is UIElement", assemblyName, typeName);
            Verifier.Verify(this._element  != null, message);


            this._element.SetValue(FrameworkElement.HeightProperty,200d);
            this._element.SetValue(FrameworkElement.WidthProperty, 200d);

            this._element.GotKeyboardFocus += new KeyboardFocusChangedEventHandler(OnGotKeyboardFocus);

            //
            // This is the test call
            // 
            
            base.Selection.AppendEmbeddedObject(this._element);
            QueueHelper.Current.QueueDelegate(new SimpleHandler(DoMouseClick));
        }

        private void DoMouseClick()
        {
            MouseInput.MouseClick(this._element);
        }

        private void OnGotKeyboardFocus(object sender, KeyboardFocusChangedEventArgs args)
        {
            //
            // upon receiving this event we will exit, or
            // else the process will hang and we need to investigate
            //
            Logger.Current.ReportSuccess();
        }

        private UIElement _element;        
    }
}

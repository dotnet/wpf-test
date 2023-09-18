// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for spell checking functionality in TextBox.

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using Drawing = System.Drawing;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Proofing;
    using System.Reflection;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// ***********This test case is marked Deleted in Tactics. Tests in file spellerTests.cs has coverage for this functionality.****************
    /// Verifies that the SpellCheck.IsEnabled property can
    /// be read and written whether the TextBox is added to
    /// a visual tree or not.
    /// </summary>
    /// <remarks>
    /// There will often be mismatches with the proofing library. To
    /// build the library from source depot, run the following
    /// commands.<code>
    /// sdx enlist enduser
    /// cd enduser\nlg\src\avalon\proofing
    /// bcz
    /// </code></remarks>
    [TestOwner("Microsoft"), TestTactics("537")]
    public class TextBoxIsSpellCheckEnabled: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("NOTE: this may fail until ProofingService.dll is set up correctly.");

            VerifyValidCalls(TestTextBox);

            Log("Repeating verification with a new stand-alone TextBox...");
            VerifyValidCalls(new TextBox());

            VerifyValidOnAdd();

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyValidCalls(TextBox box)
        {
            Log("Setting TextBox properties...");
            SetTextBoxProperties(box);

            Log("Verifying that spell checking is off by default...");
            Verifier.Verify(!box.SpellCheck.IsEnabled,
                "Spell checking is off by default", true);

            Log("Verifying that spell checking can be enabled and disabled...");
            box.SpellCheck.IsEnabled = true;
            Verifier.Verify(box.SpellCheck.IsEnabled,
                "Spell checking can be enabled", true);

            box.SpellCheck.IsEnabled = false;
            Verifier.Verify(!box.SpellCheck.IsEnabled,
                "Spell checking can be disable", true);
        }

        private void VerifyValidOnAdd()
        {
            Log("Verifying that the property is not lost when added to window...");
            TextBox box = new TextBox();
            box.SpellCheck.IsEnabled = true;
            WinPanel.Children.Add(box);
            WinPanel.UpdateLayout();
            Verifier.Verify(box.SpellCheck.IsEnabled,
                "Spell checking is enabled as expected", true);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Utility functions for testing the spell checker.
    /// </summary>
    sealed class SpellCheckUtils
    {
        #region Constructors.

        /// <summary>Hidden constructor.</summary>
        private SpellCheckUtils() { }

        #endregion Constructors.

        #region Internal methods.

        /// <summary>
        /// Gets the TextEditor editing the specified TextBox.
        /// </summary>
        /// <param name="textbox">TextBox from which to take the editor.</param>
        /// <returns>The TextEditor associated.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        internal static object GetTextEditor(TextBox textbox)
        {
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }
            
            return ReflectionUtils.GetField(textbox, "_editor");
        }

        /// <summary>
        /// Checks whether the position for a given TextBox has any
        /// speller suggestions.
        /// </summary>
        /// <param name="textbox">TextBox containing position.</param>
        /// <param name="position">Position to be tested.</param>
        /// <returns>true if any suggestions are present, false otherwise.</returns>
        internal static bool HasSuggestion(TextBox textbox, TextPointer position)
        {
            if (textbox == null)
            {
                throw new ArgumentNullException("textbox");
            }
            if (position == null)
            {
                throw new ArgumentNullException("position");
            }
            object textEditor = GetTextEditor(textbox);
            /* SpellerService */
            object speller =
                ReflectionUtils.GetField(textEditor, "_speller");
            if (speller == null)
            {
                throw new InvalidOperationException("TextBox has no speller.");
            }

            /* SuggestionSet */
            object suggestionSet = ReflectionUtils.InvokePropertyOrMethod(null,
                "GetErrorAtPosition", new object[] { speller, position, true /*visibleOnly*/ },
                InvokeType.InstanceMethod);
            if (suggestionSet == null)
            {
                return false;
            }

            object suggestions = ReflectionUtils.InvokePropertyOrMethod(null,
                "Suggestions", new object[] { suggestionSet }, InvokeType.GetInstanceProperty);
            if (suggestions == null)
            {
                return false;
            }

            int count = (int) ReflectionUtils.InvokePropertyOrMethod(null,
                "Count", new object[] { suggestions }, InvokeType.GetInstanceProperty);
            return count > 0;
        }

        #endregion Internal methods.
    }

    /// <summary>
    /// ***********This test case is marked Deleted in Tactics. Testcase 534 has coverage for this functionality.****************
    /// Verifies that the spell checker ignores embedded objects
    /// but will consider them as word breaks instead.
    /// Note that the bug is from the NLG database.
    /// </summary>
    /// <PSTask BugID="639" />
    [TestOwner("Microsoft"), TestTactics("535"), TestBugs("640")]
    public class TextBoxSpellerIgnoresEmbeddedObjects: TextBoxTestCase
    {
        #region Main flow.

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public override void RunTestCase()
        {
            TestTextBox.SpellCheck.IsEnabled = true;
            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("before");
            QueueHelper.Current.QueueDelegate(InsertObject);
        }

        private void InsertObject()
        {
            Log("Inserting Button and continuing with typing...");

            // Create an arbitrary embedded object.
            Button embeddedObject = new Button();
            embeddedObject.Content = "aqswde";

            // Insert the object into the TextBox.
            TextPointer position = TestTextBox.EndPosition;
            //position.InsertUIElement(embeddedObject);

            // Continue typing.
            KeyboardInput.TypeString("{END}after ");

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2),
                new SimpleHandler(CheckNoSuggestions));
        }

        private void CheckNoSuggestions()
        {
            Log("Verifying that there are no suggestions...");

            // Create positions to inspect for suggestions.
            TextPointer nearStart = TestTextBox.StartPosition;
            nearStart = nearStart.GetPositionAtOffset(1);
            nearStart = nearStart.GetPositionAtOffset(0, LogicalDirection.Forward);
            TextPointer nearEnd = TestTextBox.EndPosition;
            nearEnd = nearEnd.GetPositionAtOffset(-1);
            nearEnd = nearEnd.GetPositionAtOffset(0, LogicalDirection.Forward);            

            // Verify that no suggestions are present.
            Verifier.Verify(!SpellCheckUtils.HasSuggestion(TestTextBox, nearStart),
                "There is no suggestion near the start of the text.", true);
            Verifier.Verify(!SpellCheckUtils.HasSuggestion(TestTextBox, nearEnd),
                "There is no suggestion near the end of the text.", true);

            Log("Typing string to cause a suggestion near the end.");
            KeyboardInput.TypeString("{BS}kwx ");

            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2),
                new SimpleHandler(CheckSuggestionAtEnd));
        }

        private void CheckSuggestionAtEnd()
        {
            Log("Verifying that there is one suggestion near the end...");

            TextPointer nearEnd = TestTextBox.EndPosition;
            nearEnd = nearEnd.GetPositionAtOffset(-2);
            nearEnd = nearEnd.GetPositionAtOffset(0, LogicalDirection.Forward);            
            Verifier.Verify(SpellCheckUtils.HasSuggestion(TestTextBox, nearEnd),
                "There is a suggestion near the end of the text.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

    /// <summary>
    /// ***********This test case is marked Deleted in Tactics. Testcase 534 has coverage for this functionality.****************
    /// Verifies that the spell checker considers block elements as
    /// word breakers, but inlines are ignored.
    /// Note that the bug is from the NLG database.
    /// </summary>
    /// <PSTask BugID="639" />
    [TestOwner("Microsoft"), TestTactics("533"), TestBugs("640")]
    public class TextBoxSpellerElements: TextBoxTestCase
    {
        #region Main flow.

        const string TextRangeXaml =
            @"<FlowDocument xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
            @"<!--StartFragment-->" +
            @"Ant<Bold>Ant</Bold><LineBreak />Ant" +
            @"<!--EndFragment--></FlowDocument>";

        const string TextRangeXamlDescription = "XAML description:\r\n" +
            @" A n t . A n d / . / A n t " + "\r\n" +
            @"0 1 2 3 4 5 6 7 8 9 A B C D E F";

        const int FirstWordDistance = 2;
        const int SecondWordDistance = 6;
        const int ThirdWordDistance = 11;

        /// <summary>
        /// Runs the test case.
        /// </summary>
        public override void RunTestCase()
        {
            Log("Loading XAML into TextBox...");
            Log(TextRangeXaml);
            Log(TextRangeXamlDescription);
            TextRange range = new TextRange(TestTextBox.StartPosition, TestTextBox.EndPosition);
            range.Select(range.End, range.End);
            range.Xml = TextRangeXaml;
            //--range.AppendXaml(TextRangeXaml);

            TestTextBox.SpellCheck.IsEnabled = true;

            MouseInput.MouseClick(TestTextBox);
            KeyboardInput.TypeString("{END} ");

            QueueHelper.Current.QueueDelegate(CheckSuggestions);
        }

        private TextPointer PosAt(int distanceFromStart)
        {
            TextPointer pointer = TestTextBox.StartPosition;
            pointer = pointer.GetPositionAtOffset(distanceFromStart);
            pointer = pointer.GetPositionAtOffset(0, LogicalDirection.Forward);
            return pointer;            
        }

        private void CheckSuggestions()
        {
            Log("Checking that there are suggetions in the first word, " +
                "the second word, and no suggestions in the standalone word.");
            Verifier.Verify(
                SpellCheckUtils.HasSuggestion(TestTextBox, PosAt(FirstWordDistance)),
                "There are suggestions for the first word.", true);
            Verifier.Verify(
                SpellCheckUtils.HasSuggestion(TestTextBox, PosAt(SecondWordDistance)),
                "There are suggestions for the second word.", true);
            Verifier.Verify(
                !SpellCheckUtils.HasSuggestion(TestTextBox, PosAt(ThirdWordDistance)),
                "There are no suggestions for the third standalone word.", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }

	/*
    /// <summary>
    /// This test is not enabled at this time since we dont know the story of
    /// proofingservice.dll on xp.
    /// Verifies that highlights are added when spellcheck is enabled at run-time.
    /// Verifies that highlights are removed when spellcheck is disabled at run-time.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("123"), TestBugs("718,719")]
    public class TextBoxSpellerHighlights : CustomTestCase
    {
        FlowPanel fPanel;
        TextBox testTB;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            fPanel = new FlowPanel();
            testTB = new TextBox();
            testTB.Width = 500;
            testTB.Height = 500;            
            fPanel.Children.Add(testTB);
            MainWindow.Content = fPanel;
            QueueDelegate(TypeWhenNoSpellCheck);
        }

        private void TypeWhenNoSpellCheck()
        {
            MouseInput.MouseClick(testTB);
            KeyboardInput.TypeString("\"Hello worldd, goodby'e\" ");
            QueueDelegate(CheckForNoHighlights);
        }

        private void CheckForNoHighlights()
        {
            Log("Verifying that there are no highlights when spellcheck is not enabled");
            VerifyHighlights(false);
            testTB.SpellCheck.IsEnabled = true;
            QueueDelegate(CheckForHighlights);
        }

        private void CheckForHighlights()
        {
            Log("Verifying that highlights are shown when spellcheck is enabled");
            VerifyHighlights(true);
            testTB.SpellCheck.IsEnabled = false;
            QueueDelegate(CheckHighlightsRemoved);
        }

        private void CheckHighlightsRemoved()
        {
            Log("Verifying that highlights are removed when spellcheck is disabled");
            VerifyHighlights(false);
            Logger.Current.ReportSuccess();
        }

        private void VerifyHighlights(bool highlightsExists)
        {
            object boxHighlights;
            TextPointer tp1, tp2, tp3;

            // ["He<tp1/>llo wor<tp2/>ldd, good<tp3/>by'e" ]
            tp1 = testTB.StartPosition.CreatePosition(3, LogicalDirection.Forward);
            tp2 = testTB.StartPosition.CreatePosition(10, LogicalDirection.Forward);
            tp3 = testTB.StartPosition.CreatePosition(19, LogicalDirection.Forward);

            MethodInfo methodInfo = GetHighlightValueMethodInfo(testTB, out boxHighlights);

            Type spellerHighlightLayerWrapperType = typeof(FrameworkElement).Assembly.GetType("System.Windows.Documents.SpellerHighlightLayerWrapper", true);

            if (highlightsExists) //look for highlights at relevant places
            {
                if (methodInfo.Invoke(boxHighlights, new object[] { tp1, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) != DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<Hello> should not have any highlights");
                    Verifier.Verify(false, "<Hello> should not have any highlights", true);

                }

                if (methodInfo.Invoke(boxHighlights, new object[] { tp2, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) == DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<worldd> should be highlighted because it is wrongly spelled");
                    Verifier.Verify(false, "<worldd> should be highlighted because it is wrongly spelled", true);
                }

                if (methodInfo.Invoke(boxHighlights, new object[] { tp3, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) == DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<goodby'e> should be highlighted because it is wrongly typed");
                    Verifier.Verify(false, "<goodby'e> should be highlighted because it is wrongly typed", true);
                }
            }
            else
            {
                if (methodInfo.Invoke(boxHighlights, new object[] { tp1, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) != DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<Hello> should not have any highlights");
                    Verifier.Verify(false, "<Hello> should not have any highlights", true);
                }
                if (methodInfo.Invoke(boxHighlights, new object[] { tp2, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) != DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<worldd> should not have any highlights");
                    Verifier.Verify(false, "<worldd> should not have any highlights", true);
                }
                if (methodInfo.Invoke(boxHighlights, new object[] { tp3, LogicalDirection.Forward, spellerHighlightLayerWrapperType }) != DependencyProperty.UnsetValue)
                {
                    //throw new Exception("<goodby'e> should not have any highlights");
                    Verifier.Verify(false, "<goodby'e> should not have any highlights", true);
                }
            }
        }        

        /// <summary>
        /// Gets the GetHighlightValue's MethodInfo of the input TextBox using reflection.
        /// </summary>
        /// <param name="textBox">TextBox on which we have to get the GetHighlightValue MethodInfo</param>
        /// <param name="boxHighlights">out parameter for Highlights object</param>
        /// <returns>MethodInfo of GetHighlightValue method of the input textbox</returns>
        private MethodInfo GetHighlightValueMethodInfo(TextBox textBox, out object boxHighlights)
        {            
            Type textContainerType = typeof(TextContainer);
            BindingFlags bindingAttrib = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;

            PropertyInfo propertyInfo = textContainerType.GetProperty("Highlights", bindingAttrib);
            if (propertyInfo == null)
            {
                throw new Exception("Cannot access Hightlights property of a TextBox");
            }

            TextContainer boxContainer = textBox.StartPosition.TextContainer;
            boxHighlights = propertyInfo.GetValue(boxContainer, null);

            if (boxHighlights == null)
            {
                throw new Exception("Highlights of the TextBox is null");
            }

            Type highlightsType = boxHighlights.GetType();
            MethodInfo methodInfo = highlightsType.GetMethod("GetHighlightValue", bindingAttrib);

            return methodInfo;
        }
    }
	*/
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#if DISABLED_BY_TOM_BREAKING_CHANGE
#region Public Methods

        public EditPadSuite() : base("EditPad")
        {
        }

        public void LoadEditPad()
        {
            _editPad = new EditPad();

            _editPad.StartUp();

            _content = _editPad.RichTextBox.Content;

            // set drt window to upper left corner
            _editPad.Window.Left = new Length(0);
            _editPad.Window.Top = new Length(0);

        }

        public override DrtTest[] PrepareTests()
        {            
            LoadEditPad();

            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestBoldText),
                new DrtTest(ClickBoldButton),
                new DrtTest(VerifyBoldText),
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestItalicText),
                new DrtTest(ClickItalicButton),
                new DrtTest(VerifyItalicText),
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestUnderlineText),
                new DrtTest(ClickUnderlineButton),
                new DrtTest(VerifyUnderlineText),
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestOverlappingTextRange1),
                new DrtTest(ClickBoldButton),
                new DrtTest(ClearSelection),
                new DrtTest(TestOverlappingTextRange2),
                new DrtTest(ClickItalicButton),
                new DrtTest(VerifyOverlappingTextRange),
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestHeruistics),
                new DrtTest(ClickBoldButton),
                new DrtTest(DoHeruistics1),
                new DrtTest(VerifyHeruistics1),
                new DrtTest(ResetRichTextBoxContent),
                new DrtTest(TestSplitingParagraph),
                new DrtTest(VerifySplitingParagraph),
                new DrtTest(TestMergingParagraph),
                new DrtTest(VerifyMergingParagraph),
                new DrtTest(ResetTextTableContent),
                new DrtTest(TestTextTableTyping),
                new DrtTest(TestTextTableIntraCellSelection),
                new DrtTest(TestTextTableDeleteSelection),
                new DrtTest(TestTextTableSelectWholeTable)
            };
        }

#endregion Public Methods

#region Private Methods

#region Drt Manager
        
        // Outputs a message to Console in verose mode
        private void Trace(string message)
        {
            if (DRT.Verbose)
            {
                Console.Write(message);
            }
        }

        // Throws a drt failure exception if the given condition is not true.
        private void VerifyCondition(bool condition, string exceptionText)
        {
            if (!condition)
            {
                if (DRT.Verbose)
                {
                    Console.WriteLine(" - FAILED: " + exceptionText);
                }

                throw new ApplicationException(exceptionText);
            }
        }

        // Simulates Mouse Click on a UI Button by its name
        public void MouseClickButton(string buttonName)
        {
            Button button;

            button = LogicalTreeHelper.FindLogicalNode(_editPad.Window.Content, buttonName) as Button;
            if (button == null)
            {
                throw new ArgumentException("Button does not exist in EditPad frame: '" + buttonName + "'. Check EditPadFrame.xaml");
            }

            InputSimulator.MouseClick(button);
        }

        private int GetStart(string context)
        {
            int distance;

            distance = _testString.IndexOf(context);

            if (distance < 0)
            {
                throw new InvalidProgramException("Invalid context");
            }

            return distance;
        }

        private int GetEnd(string context)
        {
            return GetStart(context) + context.Length;
        }

#endregion Drt Manager

#region Test Cases

        // Sets a standard plain text into EditPad's RichTextBox
        private void ResetRichTextBoxContent()
        {
            (new TextRange(_editPad.RichTextBox.TextRange.Start, _editPad.RichTextBox.TextRange.End)).Text = _testString;
            ClearSelection();
        }

        // Clears selection. Nedded to avoid drag-drop effect instead of making selection
        private void ClearSelection()
        {
            _editPad.RichTextBox.Selection.MoveToPosition(_editPad.RichTextBox.TextRange.Start);
        }

        private void ClickBoldButton()
        {
            Debug.WriteLine("Clicking bold.");
            MouseClickButton("BoldButton");
        }

        private void ClickItalicButton()
        {
            MouseClickButton("ItalicButton");
        }

        private void ClickUnderlineButton()
        {
            MouseClickButton("UnderlineButton");
        }

        private void ClickCopyButton()
        {
            MouseClickButton("CopyButton");
        }

        private void ClickPasteButton()
        {
            MouseClickButton("PasteButton");
        }

        // Test Bold Text. This test selects the 6th character to the 16th character inclusive
        private void TestBoldText()
        {
            TextInputSimulator.MouseDrag(_content, GetStart("Welcome"), GetEnd("Welcome"));
        }

        private void VerifyBoldText()
        {
            FontWeight fontWeight;
            TextPosition beforeBoldStart;
            TextNavigator afterBoldStart;

            beforeBoldStart = _editPad.RichTextBox.TextRange.Start.CreatePosition(GetStart("Welcome"), LogicalDirection.Forward);

            // We should get back an Inline element, and the text is Bold
            VerifyCondition(beforeBoldStart.GetElementType(LogicalDirection.Forward) == typeof(Inline), "There is no element of type [Inline] next to this position");
            fontWeight = (FontWeight)beforeBoldStart.GetElementValue(LogicalDirection.Forward, Text.FontWeightProperty);
            VerifyCondition(fontWeight == FontWeights.Bold, "The text is not bold");

            // make sure the Inline element contains n characters
            afterBoldStart = beforeBoldStart.CreateNavigator();
            afterBoldStart.Move(LogicalDirection.Forward);
            VerifyCondition(afterBoldStart.GetTextRunLength(LogicalDirection.Forward) == "Welcome".Length, "Number of characters made bold is not correct");
        }

        // Test Italic Text. This test selects the 21th character to the 29th character inclusive
        private void TestItalicText()
        {
            TextInputSimulator.MouseDrag(_content, GetStart("EditPad"), GetEnd("EditPad"));
        }

        private void VerifyItalicText()
        {
            FontStyle style;
            TextNavigator textNavigator;
            TextPosition position = _editPad.RichTextBox.TextRange.Start.CreatePosition(GetStart("EditPad"), LogicalDirection.Forward);

            // We should get back an Inline element, and the text is Italic
            VerifyCondition(position.GetElementType(LogicalDirection.Forward) == typeof(Inline), "There is no element of type [Inline] next to this position");
            style = (FontStyle)position.GetElementValue(LogicalDirection.Forward, Text.FontStyleProperty);
            VerifyCondition(style == FontStyles.Italic, "The text is not italic");

            // make sure the Inline element contains n characters
            textNavigator = position.CreateNavigator();
            textNavigator.Move(LogicalDirection.Forward);
            VerifyCondition(textNavigator.GetTextRunLength(LogicalDirection.Forward) == "EditPad".Length, "Number of characters made italic is not correct");
        }

        // Test underline text. This test should underline 42th to 53th character 
        private void TestUnderlineText()
        {
            TextInputSimulator.MouseDrag(_content, GetStart("application"), GetEnd("application"));
        }

        private void VerifyUnderlineText()
        {
            TextNavigator textNavigator;
            TextDecorations textDecorations;
            TextPosition position = _editPad.RichTextBox.TextRange.Start.CreatePosition(GetStart("application"), LogicalDirection.Forward);

            // We should get back an Inline element, and the text is underline
            VerifyCondition(position.GetElementType(LogicalDirection.Forward) == typeof(Inline), "There is no element of type [Inline] next to this position");
            textDecorations = (TextDecorations)position.GetElementValue(LogicalDirection.Forward, Inline.TextDecorationsProperty);
            VerifyCondition(textDecorations != null && textDecorations.Count == 1 && textDecorations[0].Location == TextDecorationLocation.Underline), "The text is not underline");

            // make sure the Inline element contains n characters
            textNavigator = position.CreateNavigator();
            textNavigator.Move(LogicalDirection.Forward);
            VerifyCondition(textNavigator.GetTextRunLength(LogicalDirection.Forward) == "application".Length, "Number of characters made italic is not correct");
        }

        // Test overlapping TextRange
        // It first creates Bold character, and then do Italic with some overlapping 
        // on the bold text
        private void TestOverlappingTextRange1()
        {
            TextInputSimulator.MouseDrag(_content, GetStart("Welcome to EditPad"), GetEnd("Welcome to"));
        }

        private void TestOverlappingTextRange2()
        {
            TextInputSimulator.MouseDrag(_content, GetStart("to EditPad") + 1, GetEnd("to EditPad") + 2);
        }

        // We verify this by walking the content
        // we should see
        // <Character><ElementStart><Character><ElementStart><Character><ElementEnd><ElementEnd>
        // <ElementStart><Character><ElementEnd><Character><None>
        // Now the code only does validating this sequence, the code can be elaborated further to 
        // validate the element value / local values at each symbol
        private void VerifyOverlappingTextRange()
        {
            TextNavigator navigator;

            navigator = _editPad.RichTextBox.TextRange.Start.CreateNavigator();
            
            navigator.MoveByDistance(GetStart("Welcome"));
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementStart && navigator.GetElementType(LogicalDirection.Forward) == typeof(Inline) && FontWeights.Bold.Equals(navigator.GetElementValue(LogicalDirection.Forward, TextElement.FontWeightProperty)), "Expected Bolded Inline");
 
            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetTextInRun(LogicalDirection.Forward) == "Welcome ", "Bolded text is invalid");
            
            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementStart && navigator.GetElementType(LogicalDirection.Forward) == typeof(Inline) && FontStyles.Italic.Equals(navigator.GetElementValue(LogicalDirection.Forward, TextElement.FontStyleProperty)), "Expected Italicized Inline");
            
            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetTextInRun(LogicalDirection.Forward) == "to ", "Bolded+Italicized text is invalid");

            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementEnd, "End of inner Italicized Inline is expected");

            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementEnd, "End of Bold is expected");

            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementStart && navigator.GetElementType(LogicalDirection.Forward) == typeof(Inline) && FontStyles.Italic.Equals(navigator.GetElementValue(LogicalDirection.Forward, TextElement.FontStyleProperty)), "Expected Italicized Inline");
            
            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetTextInRun(LogicalDirection.Forward) == "EditPad ", "Italicized text is invalid");

            navigator.Move(LogicalDirection.Forward);
            VerifyCondition(navigator.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementEnd, "End of Italic is expected");
        }

        private void TestHeruistics()
        {
            TextInputSimulator.MouseDrag(_content, 10, 24);
        }

        private void DoHeruistics1()
        {
            // Click on the right half of the character before the chunk
            // of Bold text
            // The caret after this point should be positioned before
            // the bold text, but typing should not inject any bold characters
            TextInputSimulator.MouseClickOnRightHalfOfCharacter(_content, 9);
        }

        /// <summary>
        /// The caret should be positioned right before the bold text
        /// but it should still be outside bold (so one will not type bold text at that location)
        /// </summary>
        private void VerifyHeruistics1()
        {
            TextPosition selectionStart;
            FontWeight fontWeight;

            selectionStart = _editPad.RichTextBox.Selection.Start; 

            VerifyCondition(selectionStart.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementStart, "Selection should be before ElementStart");
            fontWeight = (FontWeight)selectionStart.GetElementValue(LogicalDirection.Forward, Text.FontWeightProperty);
            VerifyCondition(fontWeight == FontWeights.Bold, "Following Element is expected to have FontWeight=Bold");
        }

        private void DoHeruistics2()
        {
            // Click on the right half of the character at the end of the Bold Element
            // The caret after this point should be positioned after
            // the bold text, and further typing produces bold text
            TextInputSimulator.MouseClickOnLeftHalfOfCharacter(_content, 26);
        }

        /// <summary>
        /// The caret should be positioned right after the bold text
        /// it should be inside bold. one will type bold text at that location
        /// </summary>
        private void VerifyHeruistics2()
        {
            TextPosition selectionStart;
            FontWeight fontWeight;

            selectionStart = _editPad.RichTextBox.Selection.Start; 
            VerifyCondition(selectionStart.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementEnd, "It is not next to ElementEnd of an Inline element");

            fontWeight = (FontWeight)selectionStart.ReadLocalValue(Text.FontWeightProperty);
            VerifyCondition(fontWeight == FontWeights.Bold, "Current FontWeight is expected to be Bold");
        }

        private void TestCopyPaste1()
        {
            TextInputSimulator.MouseDrag(_content, 10, 24);
        }

        private void TestCopyPaste2()
        {
            TextInputSimulator.MouseClickBeforeCharacter(_content, 50);
        }

        // walk the content and we should be able to find 2 bold elements of the same size
        private void VerifyCopyPaste()
        {
            int numberOfElementStartSeen = 0;
            int numberOfElementEndSeen = 0;
            int textLengthForBold1 = 0;
            int textLengthForBold2 = 0;
            TextNavigator navigator = _editPad.RichTextBox.TextRange.Start.CreateNavigator();
            TextSymbolType symbolType = navigator.GetSymbolType(LogicalDirection.Forward);

            while (symbolType != TextSymbolType.None)
            {
                switch (symbolType)
                {
                    case TextSymbolType.ElementStart:
                        navigator.Move(LogicalDirection.Forward);
                        if (numberOfElementStartSeen == 0)
                        {
                            textLengthForBold1 = navigator.GetTextRunLength(LogicalDirection.Forward);
                        }
                        else
                        {
                            textLengthForBold2 = navigator.GetTextRunLength(LogicalDirection.Forward);
                        }

                        numberOfElementStartSeen++;
                        break;

                    case TextSymbolType.ElementEnd:
                        numberOfElementEndSeen++;
                        break;
                }
                navigator.Move(LogicalDirection.Forward);
                symbolType = navigator.GetSymbolType(LogicalDirection.Forward);
            }

            // There are two ElementStart and ElementEnd
            VerifyCondition(numberOfElementStartSeen == 2, "Cannot find 2 ElementStart");
            VerifyCondition(numberOfElementEndSeen == 2, "Cannot find 2 ElementEnd");

            // The second Element should be the same as the first one.
            VerifyCondition(textLengthForBold1 == textLengthForBold2, "Wrong content is pasted");
        }

        private void TestSplitingParagraph()
        {
            TextInputSimulator.MouseClickBeforeCharacter(_content, 40);
            InputSimulator.TypeString("{ENTER}");
        }

        /// <summary>
        /// Pressing enter in this RichTextBox will have two paragraphs. Verify this by walking through the content
        /// </summary>
        private void VerifySplitingParagraph()
        {
            TextNavigator navigator = _editPad.RichTextBox.TextRange.Start.CreateNavigator();
            TextSymbolType symbolType = navigator.GetSymbolType(LogicalDirection.Forward);
            Type typeOfElement1 = typeof(Inline);
            Type typeOfElement2 = typeof(Inline);
            int numberOfElementSeen = 0;

            while (symbolType != TextSymbolType.None)
            {
                switch (symbolType)
                {
                    case TextSymbolType.ElementStart:
                        if (numberOfElementSeen == 0)
                        {
                            typeOfElement1 = navigator.GetElementType(LogicalDirection.Forward);
                        }
                        else
                        {
                            typeOfElement2 = navigator.GetElementType(LogicalDirection.Forward);
                        }

                        numberOfElementSeen++;
                        break;
                }
                navigator.Move(LogicalDirection.Forward);
                symbolType = navigator.GetSymbolType(LogicalDirection.Forward);
            }

            VerifyCondition(numberOfElementSeen == 2, "Cannot find two Elements");
            VerifyCondition(typeOfElement1.Equals(typeof(Paragraph)), "Element 1 is not a paragraph");
            VerifyCondition(typeOfElement2.Equals(typeof(Paragraph)), "Element 2 is not a paragraph");
        }

        private void TestMergingParagraph()
        {
            InputSimulator.TypeString("{BACK}");
        }

        private void VerifyMergingParagraph()
        {
            TextNavigator navigator = _editPad.RichTextBox.TextRange.Start.CreateNavigator();
            TextSymbolType symbolType = navigator.GetSymbolType(LogicalDirection.Forward);
            Type typeOfElement = typeof(Inline);
            int numberOfElementSeen = 0;

            while (symbolType != TextSymbolType.None)
            {
                switch (symbolType)
                {
                    case TextSymbolType.ElementStart:
                        typeOfElement = navigator.GetElementType(LogicalDirection.Forward);
                        numberOfElementSeen++;
                        break;
                }
                navigator.Move(LogicalDirection.Forward);
                symbolType = navigator.GetSymbolType(LogicalDirection.Forward);
            }

            VerifyCondition(numberOfElementSeen == 1, "There should only be one Element");
            VerifyCondition(typeOfElement.Equals(typeof(Paragraph)), "Element is not a paragraph");
        }

        private void ResetTextTableContent()
        {
            Type textTableType = typeof(TextElement).Assembly.GetType("System.Windows.Documents.TextTable", false);
            TextElement textTable = Activator.CreateInstance(textTableType) as TextElement;

            _body = new Body();

            PropertyInfo bodyProperty = textTableType.GetProperty("Body", BindingFlags.Public | BindingFlags.Instance);
            bodyProperty.SetValue(textTable, _body, null);

            for (int r = 0; r < 3; r++)
            {
                Row row = new Row();
                _body.Rows.Add(row);

                for (int c = 0; c < 4; c++)
                {
                    Cell cell = new Cell();
                    cell.Text = "c" + r + "." + c;
                    cell.BorderThickness = new Thickness(new Length(1));
                    row.Cells.Add(cell);
                }
            }

            TextTree textTree = (TextTree)_editPad.RichTextBox.TextRange.TextContainer;

            textTree.InsertElement(textTree.Start, textTree.Start, textTable);
            textTree.InsertText(textTree.Start, "Top Text");
        }

        private void TestTextTableTyping()
        {
            TextInputSimulator.MouseClickOnLeftHalfOfCharacter(_content, 20);
            InputSimulator.TypeString("This is{ENTER}New Text{ENTER}In Table");
        }

        private void TestTextTableIntraCellSelection()
        {
            TextInputSimulator.MouseDrag(_content, 20, 25);
        }

        private void TestTextTableDeleteSelection()
        {
            InputSimulator.TypeString("{DELETE}");
        }

        private void TestTextTableSelectWholeTable()
        {
            TextInputSimulator.MouseDrag(_content, 0, 115);
        }

#endregion Test Cases

#endregion Private Methods
#region Private Delegates

        private delegate void TestCaseMethod();

#endregion Private Delegates
#region Private Classes

        private class TestCase
        {
            public TestCase(string name, TestCaseMethod method)
            {
                this.Name = name;
                this.Method = method;
            }

            public string Name;
            public TestCaseMethod Method;
        }

#endregion Private Classes
#region Private Fields

        // An instance of EditPad application
        private EditPad _editPad;

        // RIchTextBox content of the _editPad.
        private FrameworkElement _content;

        // Initial string used as a editable content
        private string _testString = "Quickly! Welcome to EditPad - a test application for rich text editing";

        // Cached table body
        private Body _body;

#endregion Private Fields

    }
}
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
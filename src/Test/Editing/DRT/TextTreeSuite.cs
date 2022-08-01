// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//TextContainer tests.
#if DISABLED_BY_TOM_BREAKING_CHANGE
#if NOT_YET
                CopyTests();
#endif // NOT_YET

            new DrtTest(EnumeratorTest),
            new DrtTest(TextElementTests),
        };
    }

        // Check the expected event sequence for inserting in InsertTests(true)
        // and AdvancePointerTests().  Note that these must be kept in sync with
        // changes to these tests.
        public static bool CheckArgs = false;
        public static int  CheckArgsIndex = 0;
        public static int[] CheckArgsNumbers = new int[] {
            0, 2, 0,    // InsertTests(true)
            1, 2, 0,
            3, 2, 0,
            5, 2, 0,
            4, 2, 0,
            3, 10, 0,   // AdvancePointerTests();
            7, 1, 0};           
            
        // Tree changed event handler
        public void MyTextChangedEventHandler(object sender, TextContainerChangedEventArgs args)
        {
            foreach (TextChangeSegment segment in args.Changes)
            {
                int index = segment.TextPosition.TextContainer.Start.GetOffsetToPosition(segment.TextPosition);

                if (CheckArgs && segment.TextChange != TextChange.ContentAffected)
                {
                    if (index != CheckArgsNumbers[CheckArgsIndex++])
                    {
                        throw new Exception("TextChanged args are wrong (0)");
                    }
                    if (segment.TextChange == TextChange.ContentAdded)
                    {
                        if (segment.SymbolCount != CheckArgsNumbers[CheckArgsIndex++] ||
                            0 != CheckArgsNumbers[CheckArgsIndex++])
                        {
                            throw new Exception("TextChanged args are wrong (1)");
                        }
                    }
                    else
                    {
                        if (segment.SymbolCount != CheckArgsNumbers[CheckArgsIndex++] ||
                            0 != CheckArgsNumbers[CheckArgsIndex++])
                        {
                            throw new Exception("TextChanged args are wrong (2)");
                        }
                    }
                }
            }
        }

        // Regression tests for previous bugs
        private void Bugs()
        {
            TextBox tb = new TextBox();

            TextFlow newPanel = new TextFlow();
            TextContainer newTree = (TextContainer)newPanel.TextRange.TextContainer;

            TextElement root = new Inline();
            TextElement nodeB = new Inline();
            TextElement nodeB1 = new Inline();

            // set their ids
            root.ID = "root";
            nodeB.ID = "nodeB";
            nodeB1.ID = "nodeB1";

            // now build the tree
            newTree.InsertElement(newTree.End, newTree.End, root);
            root.Append(nodeB);
            nodeB.Append(nodeB1);
            
            // create two text tree navigators & move into position
            TextNavigator ptr1 = newTree.Start.CreateNavigator();
            TextNavigator ptr2 = newTree.Start.CreateNavigator();

            newTree.MoveToElementEdge(ptr1, nodeB, ElementEdge.AfterStart);
            newTree.MoveToElementEdge(ptr2, nodeB, ElementEdge.BeforeEnd);

            // create the TextRange
            TextRange tr = new TextRange(ptr1, ptr2);  

            // Target: TextContainer.Delete
            tr.Text = String.Empty;

            tb.Text = "Some Text";
            TextPosition ptr3 = tb.EndPosition;
            int characterOffsetFromTextPosition = ptr3.GetOffsetToPosition(ptr3);
            int characterOffsetFromTextNavigator = ptr3.GetOffsetToPosition(ptr3.CreateNavigator());
            if (characterOffsetFromTextPosition != 0 ||
                characterOffsetFromTextNavigator != 0)
                throw new Exception("Bug regressed.");

                TextNavigator ptra = newTree.Start.CreateNavigator();
                newTree.MoveToElementEdge(ptra, root, ElementEdge.BeforeStart);
                TextNavigator ptrb = newTree.Start.CreateNavigator();
                newTree.MoveToElementEdge(ptrb, nodeB, ElementEdge.AfterEnd);

                TextRange tr1 = new TextRange(ptra, ptrb);

                tr1.Text = String.Empty;
                
                TextBox box = new TextBox();

                box.Text = "Hello, world!";
                TextNavigator n = box.StartPosition.CreateNavigator();
                n.MoveByDistance(3);
                new Bold(box.StartPosition, n);
                
                // Write it out.
                ConsoleWriteLine("TextBox XAML contents:");
                ConsoleWriteLine(
                  System.Windows.Markup.XamlWriter.Save(box));

                TextFlow newPanel0 = new TextFlow();
                Inline il = new Inline();
                ((IAddChild)newPanel0).AddChild(il);
                il.Append("text");

                TextNavigator tttn = newPanel0.TextRange.Start.CreateNavigator();
                ((TextContainer)newPanel0.TextRange.TextContainer).MoveToElementEdge(tttn, il, ElementEdge.AfterStart);
                if (tttn.GetSymbolType(LogicalDirection.Forward) != TextSymbolType.Character)
                    throw new Exception("Bug 848301 regressed 1.");
                if (((TextContainer)newPanel0.TextRange.TextContainer).GetElement(tttn, LogicalDirection.Backward) != il)
                    throw new Exception("Bug 848301 regressed 2.");

                // Verify TextElement.Text works
                if (il.Text != "text")
                    throw new Exception("Bad TextElement.Text");
                il.Text = "new text";
                if (il.Text != "new text")
                    throw new Exception("Bad TextElement.Text (2)");
       
           {
               TextFlow newPanel1 = new TextFlow();
               TextContainer newTree1 = (TextContainer)newPanel1.TextRange.TextContainer;

               TextElement root1 = new Inline();
               TextElement nodeB11 = new Inline();
           
               // set their ids
               root1.ID = "root";
               nodeB11.ID = "nodeB";
           
               // now build the tree
               ((IAddChild)newPanel1).AddChild(root1);
               UIElement testObject = new UIElement();
               root1.Append(testObject);
               root1.Append(nodeB11);

               TextNavigator ptr11 = newTree1.Start.CreateNavigator();
               newTree1.MoveToElementEdge(ptr11, root1, ElementEdge.AfterStart);
           
           
               // **** UNCOMMENT THIS LINE AND IT ALL WORKS ****
               //Console.WriteLine(ptr1.GetSymbolType(LogicalDirection.Forward));
           
               // Target: TextNavigator.GetEmbeddedObject(...)
               Object retrievedObject = ptr11.GetEmbeddedObject(LogicalDirection.Forward);
           
           }
        }

        // Test basic pointer operations on TextContainer
        private void BasicPointerTests()
        {
            // Create a tree 
            Text              uberRoot = new MyText(null);
            TextElement       theElement  = new MyTextElement();
            ((IAddChild)uberRoot).AddChild(theElement);
            TextContainer         theTree = theElement.TextContainer;
            TextPosition  ttp = theTree.Start;

            // Check default textposition properties and properties.  
            // They should all return something
            if (theTree.Start == null)
                throw new Exception("Start is null");
            if (theTree.End == null)
                throw new Exception("End is null");
            if (theTree != ttp.TextContainer)
                throw new Exception("Start not contained in tree");
            if (theTree.GetElement(ttp) != null)
                throw new Exception("GetElement on Start wrong");
            if (theTree != theTree.End.TextContainer)
                throw new Exception("End not contained in tree");
            if (theTree.End.Gravity != LogicalDirection.Forward ||
                ttp.Gravity != LogicalDirection.Backward)
                throw new Exception("Wrong start or end gravity");
            if (ttp.TextContainer != theTree || 
                theTree.End.TextContainer != theTree)
                throw new Exception("Wrong TextContainer");

            // Check index and position methods
            TextPosition tp = theTree.Start.CreatePosition(1, LogicalDirection.Backward);
            if (theTree.Start.GetOffsetToPosition(tp) != 1)
                throw new Exception("Start.CreatePosition or Start.GetOffsetToPosition error");
            if (tp.Gravity != LogicalDirection.Backward)
                throw new Exception("Wrong TextPosition gravity");

            // Add some text and move pointers around
            theTree.InsertText(theTree.Start, "1234567890");
            TextNavigator ttn = theTree.End.CreateNavigator();
            if (ttn.Gravity != LogicalDirection.Forward)
                throw new Exception("Wrong gravity on TextNavigator");
            ttn.MoveToPosition(theTree.Start.CreatePosition(5, LogicalDirection.Forward));
            ttn.SetGravity(LogicalDirection.Backward);
            if (theTree.Start.GetOffsetToPosition(ttn) != 5)
                throw new Exception("Start.GetOffsetToPosition error 1");
            ttn.MoveByDistance(3);
            if (theTree.Start.GetOffsetToPosition(ttn) != 8)
                throw new Exception("Start.GetOffsetToPosition error 2");
            ttn.MoveByDistance(-6);
            if (theTree.Start.GetOffsetToPosition(ttn) != 2)
                throw new Exception("Start.GetOffsetToPosition error 3");
            if (ttn.TextContainer != theTree)
                throw new Exception("Wrong text context");

            // Check immutable pointers and identity
            TextPosition txtpos1 = ttn.CreatePosition();
            TextPosition txtpos2 = ttn.CreatePosition();
            if (txtpos1 != txtpos2)
                throw new Exception("CreatePosition difference");

            // Check basic offsets
            long offset = theTree.Start.GetOffsetToPosition(theTree.End);
            if (offset != 12)
                throw new Exception("DistanceTo wrong 1.  Got " + offset);
            offset = theTree.End.GetOffsetToPosition(theTree.Start);
            if (offset != -12)
                throw new Exception("DistanceTo wrong 2.  Got " + offset);
            ttn.MoveByDistance(3);
            offset = theTree.End.GetOffsetToPosition(ttn);
            if (offset != -7)
                throw new Exception("DistanceTo wrong 3.  Got " + offset);

            // Test comparisons
            if (ttn >= theTree.End)
                throw new Exception("CompareTo 1 failed");
            if (theTree.End <= ttn)
                throw new Exception("CompareTo 2 failed");
            if (ttn >= theTree.End || theTree.Start >= ttn)
                throw new Exception("Comparison failed");
            
            
#if TEXTRANGES
            if (theTree.Contains(theTree.End))
                throw new Exception("End not contained in tree");
#endif // TEXTRANGES

            ConsoleWriteLine("\nBasicPointerTests");
            TextTreeDumperDumpSplay(theTree);
        }

        private void InsertTests()
        {
            RunInsertTests(true);
        }

        // Test insertion into TextContainer
        private TextTestControl RunInsertTests(bool dump)
        {
            if(dump)
            {
                ConsoleWriteLine("\nInsertTests");
                CheckArgs = true;
            }
            
            // Create a tree
            Text              uberRoot = new MyText(null);
            TextTestControl   panel  = new TextTestControl(uberRoot);
            TextContainer         theTree = panel.TextContainer;
            IAddChild        ipanel = panel as IAddChild;
            panel.AddChangeHandler(new TextContainerChangedEventHandler(MyTextChangedEventHandler));

            // Insert some elements using public apis
            MyTextElement mc1 = new MyTextElement();
            TextNavigator start = theTree.Start.CreateNavigator();
            TextNavigator end = theTree.End.CreateNavigator();
            MyTextElement mc2 = new MyTextElement();
            MyTextElement mc3 = new MyTextElement();
            MyTextElement mc4 = new MyTextElement();
            MyTextElement mc5 = new MyTextElement();
            IAddChild     imc4 = mc4 as IAddChild;
            IAddChild     imc5 = mc5 as IAddChild;

            ipanel.AddChild(mc4);
            imc4.AddChild(mc3);
            imc4.AddChild(mc5);
            imc4.AddChild(mc2);
            imc5.AddChild(mc1);

            TextNavigator ttn = theTree.Start.CreateNavigator();
            theTree.MoveToElementEdge(ttn, mc5, ElementEdge.BeforeEnd);
            if (theTree.GetElement(ttn) != mc5 ||
                theTree.Start.GetOffsetToPosition(ttn) != 6)
                throw new Exception("New TextNavigator positioned wrong");
            
            if (dump)
            {
                TextTreeDumperDumpSplay(theTree);
                CheckArgs = false;
            }

            return panel;
        }

        // Test extraction from text tree
        private void MoveExtractTests()
        {
        }

        // Test setting and getting text in TextContainer
        private void SetTextTests()
        {
            // Get a tree
            TextTestControl panel = RunInsertTests(false);
            TextContainer theTree = panel.TextContainer;

            ConsoleWriteLine("\nSetTextTests\nBefore:");
            TextTreeDumperDumpSplay(theTree);
            
            // Set up pointers and remove some elements
            TextNavigator start = theTree.Start.CreateNavigator(3);
            TextNavigator end = theTree.Start.CreateNavigator(7);
            end.SetGravity(LogicalDirection.Forward);
            theTree.DeleteContent(start, end);

            // Insert text in several steps, moving start pointer around
            start.MoveToPosition(theTree.Start.CreatePosition(5, LogicalDirection.Forward));
            theTree.InsertText(start,"lazy dog");
            theTree.InsertText(start,"The quick brown fox  the ");
            start.MoveByDistance(20);
            theTree.InsertText(start,"jumps over");

            // Move end pointer to start of text run and read text
            end.MoveToPosition(start);
            end.MoveByDistance(-20);
            TextNavigator copyOfEnd = end.CreateNavigator();
            string txt = copyOfEnd.GetTextInRun(LogicalDirection.Forward);
            if (txt != "The quick brown fox jumps over the lazy dog")
                throw new Exception ("SetText or GetText failure");
            TextPosition txtpos = copyOfEnd.CreatePosition();
            if (txtpos.GetSymbolType(LogicalDirection.Forward) != 
                TextSymbolType.Character ||
                txtpos.GetTextRunLength(LogicalDirection.Forward) != 43)
                throw new Exception ("Start.GetTextRunLength or GetSymbolType failure");
            if (txtpos.GetTextInRun(LogicalDirection.Forward) != 
                "The quick brown fox jumps over the lazy dog")
                throw new Exception ("Start.GetText failure");
            if (txtpos.GetOffsetToPosition(theTree.End) != 44)
                throw new Exception ("Start.GetOffsetToPosition failure");
            
            ConsoleWriteLine("\nText run=" + txt);
            
            ConsoleWriteLine("\nAfter:");
            TextTreeDumperDumpSplay(theTree);

            if (theTree.Start.GetOffsetToPosition(theTree.End) != 49)
                throw new Exception("Tree is unexpected size.");

            // Position the navigator and get raw character arrays
            start.MoveToPosition(theTree.Start.CreatePosition(4, LogicalDirection.Forward));
            char[] carr = new char[12];
            start.Move(LogicalDirection.Forward);
            // Range check on LogicalDirection
            bool gotargex = false;
            try { start.GetTextInRun((LogicalDirection)365 ,carr.Length,null,carr,0); }
            catch (ArgumentException e) {if (e.ParamName == "direction") gotargex = true;}
            if (!gotargex)
                throw new Exception("Failed to catch bad arg on GetChars");

            start.GetTextInRun(LogicalDirection.Forward,carr.Length,null,carr,0);
            ConsoleWrite("carr forward=");
            DumpChar(carr);
            start.MoveToPosition(theTree.Start.CreatePosition(20, LogicalDirection.Forward));
            carr = new char[30];
            for (int i=0; i<30; i++) carr[i]='-';
            start.GetTextInRun(LogicalDirection.Backward,carr.Length-6,null,carr,6);
            ConsoleWrite("carr backward=");
            DumpChar(carr);

            // Test a very large number of characters to see how multiple blocks
            // of text are handled.  Check sizes and characters stored.
            start.MoveToPosition(theTree.Start.CreatePosition(2, LogicalDirection.Forward));
            start.SetGravity(LogicalDirection.Backward);
            bool verbose = DRT.Verbose;
            DRT.Verbose = false;  // Don't spew changed events
            for (int i = 0; i < 10000; i++)
                theTree.InsertText(start,"0123456789");
            DRT.Verbose = verbose;
            if (start.GetTextInRun(LogicalDirection.Forward).Length != 100000)
                throw new Exception("GetText read wrong length");
            char[] bigchar = new char[100000];
            start.GetTextInRun(LogicalDirection.Forward, bigchar.Length, null, bigchar, 0);
            for (int j=0; j<10000; j+=10)
                for (int k=0; k<10; k++)
                    if (bigchar[j+k] != k+0x30)
                        throw new Exception("Wrong character in returned bigchar");
            start.MoveToPosition(theTree.Start.CreatePosition(100045, LogicalDirection.Forward));
            if (start.GetTextInRun(LogicalDirection.Backward) != 
                "The quick brown fox jumps over the lazy ")
                throw new Exception("Unexpected string in large tree");
        }

        // Test TextNavigator move operations
        private void AdvancedPointerTests()
        {
            ConsoleWriteLine("\nAdvancedPointerTests:");

            // Get a tree and pointer
            TextTestControl panel = RunInsertTests(false);
            TextContainer theTree = panel.TextContainer;

            CheckArgs = true;

            // Insert some text and a generic object
            TextNavigator tn = theTree.Start.CreateNavigator();
            tn.MoveToPosition(theTree.Start.CreatePosition(3, LogicalDirection.Forward));
            theTree.InsertText(tn,"some stuff");
            tn.MoveToPosition(theTree.Start.CreatePosition(7, LogicalDirection.Forward));

            TextTreeDumperDumpSplay(theTree);

            // Walk forward and then backward across tree
            ConsoleWriteLine("\nForwards:");
            tn.MoveToPosition(theTree.Start.CreatePosition(0, LogicalDirection.Forward));
            TextSymbolType[] runValues = new TextSymbolType[]{
                TextSymbolType.ElementStart,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementEnd,
                TextSymbolType.Character,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementEnd,
                TextSymbolType.None,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementStart,
                TextSymbolType.Character,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementStart,
                TextSymbolType.ElementStart,
                TextSymbolType.None};
                
            int[] textLengths = new int[]{10,10};
            int runValueIndex = 0;
            int textLengthIndex = 0;
            char[] characters = new char[10];
            do
            {
                ConsoleWrite("type=" + tn.GetSymbolType(LogicalDirection.Forward) + 
                                  " len=" + tn.GetTextRunLength(LogicalDirection.Forward));

                if (tn.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.Character &&
                    textLengths[textLengthIndex++] != tn.GetTextInRun(LogicalDirection.Forward,
                                      characters.Length, null, characters, 0))
                    throw new Exception("Wrong GetChars length");

                if (tn.GetSymbolType(LogicalDirection.Forward) != runValues[runValueIndex++])
                    throw new Exception("Wrong run value at index = " + runValueIndex);

                if (tn.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementStart)
                {
                    DependencyObject ooo = theTree.GetElement(tn, LogicalDirection.Forward);
                    ConsoleWrite(" " + ooo.GetHashCode());
                    if (ooo.GetType() != typeof(MyTextElement))
                    throw new Exception("GetElement(direction) got unexpected object");
                }

                if (tn.GetSymbolType(LogicalDirection.Forward) == TextSymbolType.ElementEnd)
                {  
                     DependencyObject deo = theTree.GetElement(tn);
                     ConsoleWrite(" " + deo.GetHashCode());
                     if (deo.GetType() != typeof(MyTextElement))
                        throw new Exception("GetElement got unexpected element");
                }
                ConsoleWriteLine("");
            } while (tn.Move(LogicalDirection.Forward));
            
            ConsoleWriteLine("\nBackwards:");
            do
            {
                ConsoleWriteLine("type=" + tn.GetSymbolType(LogicalDirection.Backward) + 
                                  " len=" + tn.GetTextRunLength(LogicalDirection.Backward));
                if (tn.GetSymbolType(LogicalDirection.Backward) == TextSymbolType.Character &&
                    textLengths[textLengthIndex++] != tn.GetTextInRun(LogicalDirection.Backward,
                                      characters.Length, null, characters, 0))
                    throw new Exception("Wrong GetChars length");

                if (tn.GetSymbolType(LogicalDirection.Backward) != runValues[runValueIndex++])
                    throw new Exception("Wrong run value at index = " + runValueIndex);

                if (tn.GetSymbolType(LogicalDirection.Backward) == TextSymbolType.ElementStart &&
                    theTree.GetElement(tn).GetType() != typeof(MyTextElement))
                    throw new Exception("GetElement got unexpected element");
            } while (tn.Move(LogicalDirection.Backward));

            CheckArgs = false;            
        }

        private void RangeTests()
        {
            int i;

            ConsoleWriteLine("\nRangeTests:");

            // Get a tree and pointer
            TextTestControl panel = RunInsertTests(false);
            TextContainer theTree = panel.TextContainer;

            // Create a range and check basic properties
            TextNavigator tpstart = theTree.Start.CreateNavigator();
            TextNavigator tpend = theTree.End.CreateNavigator();
            TextRange rng1 = new TextRange(tpstart, tpend);
            if (rng1.IsEmpty ||
                !rng1.IsMovable)
                throw new Exception("Unexpected TextRange basic property");
            if (!rng1.Contains(theTree.Start) ||
                !rng1.Contains(theTree.End))
                throw new Exception("Range doesn't contain endpoints");
            TextTreeDumperDumpSplay(theTree);

            // Move range endpoints and create an empty range.  
            tpstart.MoveToPosition(theTree.Start.CreatePosition(5, LogicalDirection.Forward));
            theTree.InsertText(tpstart,"Some Text Here");
            tpend.MoveByDistance(-10);
            rng1.MoveToPositions(tpstart, rng1.End);
            for (i=0; i<5; i++)
            {
                rng1.MoveEndToNextCharacter(LogicalDirection.Backward);
            }
            tpend.MoveByDistance(2);
            TextRange rng2 = new TextRange(tpend);
            if (rng2.Start != rng2.End)
                throw new Exception("range start and end different");
            if (!rng2.IsEmpty)
                throw new Exception("TextRange should be empty.");

            // Try various Move operations
            for (i=0; i<3; i++)
            {
                rng2.MoveStartToNextCharacter(LogicalDirection.Backward);
            }
            for (i=0; i<2; i++)
            {
                rng2.MoveEndToNextCharacter(LogicalDirection.Forward);
            }
            if (rng2.Text != "t Her")
                throw new Exception("Range move or Text get failed");
            rng2.Text = "ans go for brok";
            rng1.MoveToPositions(rng1.Start, theTree.End);
            if (rng1.Text != "Some Texans go for broke")
                throw new Exception("Range set Text failed");

            // Create a large number of textranges at various locations, then unposition
            // them.  This should work without asserts
            TextNavigator ttns = theTree.Start.CreateNavigator();
            ttns.MoveToPosition(theTree.Start.CreatePosition(11, LogicalDirection.Forward));
            theTree.InsertText(ttns, "A whole bunch more characters that I will remove later");
            TextNavigator ttne = theTree.End.CreateNavigator();
            int startcp = theTree.Start.GetOffsetToPosition(ttns);
            int endcp = theTree.Start.GetOffsetToPosition(ttne);
            ArrayList ranges = new ArrayList();
            ConsoleWriteLine("\nSplay before adding ranges");
            TextTreeDumperDumpSplay(theTree);
            for (int scp = startcp; scp <= endcp; scp++)
            {
                ttns.MoveToPosition(theTree.Start.CreatePosition(scp, LogicalDirection.Forward));
                for (int ecp = scp; ecp <= endcp; ecp++)
                {
                    ttne.MoveToPosition(theTree.Start.CreatePosition(ecp, LogicalDirection.Forward));
                    ranges.Add(new TextRange(ttns, ttne));
                }
            }

            // Remove some content to cause grid collapsing
            ttns.MoveToPosition(theTree.Start.CreatePosition(11, LogicalDirection.Forward));
            ttns.MoveToPosition(theTree.Start.CreatePosition(61, LogicalDirection.Forward));
            TextRange ttr = new TextRange(ttns, ttne);
            ttr.Text = String.Empty;
            // Unposition ranges.  If there are problems with range collapsing, we'll
            // assert here.
            ConsoleWriteLine("\nSplay after adding and removing " + ranges.Count + " ranges");
            TextTreeDumperDumpSplay(theTree);
        }

        // Tests high-level TextRange methods: RemoveBreaks, Clear, etc.
        private void TextOMRangeTests()
        {
            TextContainer tree;
            TextRange range;
            TextNavigator navigator;
            int i;

            TextSymbolType[] expectedSymbols = new TextSymbolType[]
            {
                TextSymbolType.ElementStart,
                TextSymbolType.ElementStart,
                TextSymbolType.Character,
                TextSymbolType.ElementEnd,
                TextSymbolType.ElementEnd,
                TextSymbolType.None,
            };

            tree = new TextContainer();
            range = new TextRange(tree.Start, tree.Start);

            // Add two paragraph breaks - note it will end up with THREE paragraphs!
            range.AppendBreak(typeof(Paragraph));
            range.AppendBreak(typeof(Paragraph));

            if (tree.Start.GetOffsetToPosition(tree.End) != 6)
                throw new Exception("Invalid initial three-paragraph structure!");

            range.MoveToPositions(tree.Start.CreatePosition(+1, LogicalDirection.Forward), tree.End.CreatePosition(-1, LogicalDirection.Forward));
            range.RemoveBreaks(typeof(Paragraph));

            if (tree.Start.GetOffsetToPosition(tree.End) != 2)
                throw new Exception("Breaks not removed!");

            // Create:
            // <Paragraph><Inline>Text.</Inline></Paragraph>
            navigator = tree.Start.CreateNavigator(+1);
            tree.InsertElement(navigator, navigator, typeof(Inline));
            navigator.Move(LogicalDirection.Forward);
            tree.InsertText(navigator, "Text.");

            // Insert break.
            // <Paragraph><Inline>Te</Inline></Paragraph><Paragraph><Inline>ext.</Inline></Paragraph>
            navigator.MoveByDistance(+2);
            range = new TextRange(navigator);
            range.AppendBreak(typeof(Paragraph));

            // Remove break.
            range.MoveToPositions(tree.Start.CreatePosition(+5, LogicalDirection.Forward), tree.Start.CreatePosition(+7, LogicalDirection.Forward));
            range.RemoveBreaks(typeof(Paragraph));

            navigator.MoveToPosition(tree.Start);

            for (i = 0; i < expectedSymbols.Length; i++)
            {
                if (navigator.GetSymbolType(LogicalDirection.Forward) != expectedSymbols[i])
                    throw new Exception("Bad symbol type (" + i + ")!");
                navigator.Move(LogicalDirection.Forward);
            }
        }

        // Test binary placement and searching of unembedded TextTreePositions
        private void BinaryPointerTests()
        {
            ConsoleWriteLine("\nBinaryPointerTests:");
            TextTestControl panel = RunInsertTests(false);
            TextContainer tree = panel.TextContainer;
            TextNavigator start = tree.Start.CreateNavigator(5);
            tree.InsertText(start, "this is some text");

            TextPosition p3  = tree.Start.CreatePosition(1, LogicalDirection.Forward);
            TextPosition p1  = tree.Start.CreatePosition(0, LogicalDirection.Backward);
            TextPosition p6  = tree.Start.CreatePosition(3, LogicalDirection.Forward);
            TextPosition p7  = tree.Start.CreatePosition(4, LogicalDirection.Forward);
            TextPosition p8  = tree.Start.CreatePosition(7, LogicalDirection.Forward);
            TextPosition p9  = tree.Start.CreatePosition(9, LogicalDirection.Forward);
            TextPosition p5  = tree.Start.CreatePosition(3, LogicalDirection.Backward);
            TextPosition p10 = tree.Start.CreatePosition(9, LogicalDirection.Backward);
            TextPosition p2  = tree.Start.CreatePosition(0, LogicalDirection.Forward);
            TextPosition p11  = tree.Start.CreatePosition(13, LogicalDirection.Forward);
            TextPosition p15  = tree.Start.CreatePosition(22, LogicalDirection.Forward);
            TextPosition p12  = tree.Start.CreatePosition(15, LogicalDirection.Forward);
            TextPosition p13  = tree.Start.CreatePosition(17, LogicalDirection.Forward);
            TextPosition p4  = tree.Start.CreatePosition(1, LogicalDirection.Backward);
            TextPosition p14  = tree.Start.CreatePosition(18, LogicalDirection.Forward);
            TextPosition p16  = tree.Start.CreatePosition(23, LogicalDirection.Forward);

            p4 = null;
            p6 = null;
            Thread.Sleep(100);
            GC.Collect();

            ConsoleWriteLine("\nAfter sleep - should be null TPs:");
            TextTreeDumperDumpTextPositions(tree);
            TextTreeDumperDumpSplay(tree);
            
            TextPosition p18a  = tree.Start.CreatePosition(3, LogicalDirection.Forward);
            TextPosition p17  = tree.Start.CreatePosition(1, LogicalDirection.Forward);
            TextPosition p18  = tree.Start.CreatePosition(3, LogicalDirection.Forward);
            TextPosition p19  = tree.Start.CreatePosition(2, LogicalDirection.Forward);
            
            ConsoleWriteLine("\nAdd new TPs, nulls should be gone:");
            TextTreeDumperDumpTextPositions(tree);
        }

        private void ControlsTests()
        {
            ConsoleWriteLine("\nControlsTests:");
            
            Inline a = new Inline();
            TextFlow b = new TextFlow();
            ((TextContainer)b.TextRange.TextContainer).InsertElement(b.TextRange.End, b.TextRange.End, a);
            a.Append("string");
            a.Append(new Inline());
            b.TextRange.AppendText("anotherString");

            if (a.TextContainer.Start.GetOffsetToPosition(b.TextRange.End) != 23)
                throw new Exception("Unexpected control tree size.");
            
            TextTreeDumperDumpSplay(a.TextContainer);
        }

#if NOT_YET
        private void CopyTests()
        {
            ConsoleWriteLine("\nCopyTests");
            
            // Create a tree
            Text              uberRoot = new MyText(null);
            TextTestControl   panel  = new TextTestControl(uberRoot);
            TextContainer         theTree = panel.TextRange.Start.TextContainer;
            IAddChild         ipanel = panel as IAddChild;
            panel.AddChangeHandler(new System.Windows.Documents.TextChangedEventHandler(TextTreeTest.MyTextChangedEventHandler));

            // Insert some elements using public apis
            //
            //   uberRoot -- mc1 -+- mc2
            //                    +- mc3 --- mc5
            //                    +- mc4
            //                    +- mc6
            //
            MyTextElement mc1 = new MyTextElement();
            TextNavigator start = new TextNavigator(theTree.Start);
            TextNavigator end = new TextNavigator(theTree.End);
            MyTextElement mc2 = new MyTextElement();
            MyTextElement mc3 = new MyTextElement();
            MyTextElement mc4 = new MyTextElement();
            MyTextElement mc5 = new MyTextElement();
            MyTextElement mc6 = new MyTextElement();
            IAddChild     imc1 = mc1 as IAddChild;
            IAddChild     imc3 = mc3 as IAddChild;

            ipanel.AddChild(mc1);
            imc1.AddChild(mc2);
            imc1.AddChild(mc3);
            imc1.AddChild(mc4);
            imc1.AddChild(mc6);
            imc3.AddChild(mc5);

            theTree.Copy(mc2.End, mc4.End, mc6.End, new ObjectCloneDelegate(CloneIt));
            ConsoleWriteLine("After Copy:");
            TextTreeDumperDumpSplay(theTree);
            //   uberRoot -- mc1 -+- mc2
            //                    +- mc3 --- mc5
            //                    +- mc4
            //                    +- mc6 -+- mc7
            //                            +- mc8 --- mc9
            //                            +- mc10

            theTree.Move(mc2.End, mc4.End, mc6.End, new ObjectCloneDelegate(CloneIt));
            ConsoleWriteLine("After Move:");
            TextTreeDumperDumpSplay(theTree);
            //   uberRoot -- mc1 -+- mc2
            //                    +- mc4
            //                    +- mc6 -+- mc7
            //                            +- mc8 --- mc9
            //                            +- mc10
            //                            +- mc11
            //                            +- mc3 --- mc5
            //                            +- mc12

        }
#endif // NOT_YET

        // Test TextTreeRangeContentEnumerator
        private void EnumeratorTest()
        {
            // Get a tree
            TextTestControl panel = RunInsertTests(false);
            IAddChild       ipanel = panel as IAddChild;
            TextContainer theTree = panel.TextContainer;

            ipanel.AddText("Some ");
            ipanel.AddText("Text");
            ipanel.AddChild(new MyTextElement());
            ipanel.AddChild(new TextTestControl(null));
            ipanel.AddText("More Text");

            ConsoleWriteLine("\nEnumeratorTest\n");
            TextTreeDumperDumpSplay(theTree);

            IEnumerator ienum = LogicalTreeHelper.GetChildren(panel).GetEnumerator();
            object curr;
            while (ienum.MoveNext())
            {
                curr = ienum.Current;
                ConsoleWriteLine(curr.ToString());
            }
            ienum.Reset();
            ConsoleWriteLine("\nReset enumerator");

            bool gotit = false;
            try
            {
                curr = ienum.Current;
            }
            catch (InvalidOperationException)
            {
                gotit = true;
            }
            if (!gotit)
                throw new Exception("MoveNext after end didn't throw exception");

            while (ienum.MoveNext())
            {
                ConsoleWriteLine(ienum.Current.ToString());
            }            
        }

        // Test the public non-abstract TextContainer/TextElement methods.
        private void TextElementTests()
        {
            TextNavigator navigator;
            TextElement element;
            TextContainer tree;

            element = new Inline();
            if (element.TextContainer == null)
                throw new Exception("Found null TextContainer on TextElement!");

            element = new Inline();
            element.Text = "test";
            if (element.TextContainer == null)
                throw new Exception("Found null TextContainer on TextElement! (2)");

            element = new Inline();
            element.Append("test");
            element.Text = String.Empty;

            element = new Inline();
            element.Append(new Inline());
            element.Text = String.Empty;

            element = new Inline();
            element.Append(new Button());
            element.Text = String.Empty;

            element = new Inline();
            element.Text = String.Empty;

            element = new Inline();
            if (!element.IsEmpty)
                throw new Exception("TextElement should be empty!");
            element.Text = "test";
            if (element.IsEmpty)
                throw new Exception("TextElement should not be empty!");

            tree = new TextContainer();
            if (tree.GetElement(tree.Start) != null)
                throw new Exception("Unexpected TextElement in tree!");
            if (tree.GetElement(tree.Start, LogicalDirection.Forward) != null)
                throw new Exception("Unexpected TextElement in tree!");
            tree.InsertElement(tree.Start, tree.Start, new Inline());
            navigator = tree.Start.CreateNavigator();
            if (tree.GetElement(navigator, LogicalDirection.Forward) == null)
                throw new Exception("Missing TextElement in tree! (1)");
            navigator.Move(LogicalDirection.Forward);
            if (tree.GetElement(navigator) == null)
                throw new Exception("Missing TextElement in tree! (2)");

            element = new Inline();
            element.Text = "test";
            element.TextContainer.InsertElement(element.Start, element.Start, new Inline());
            navigator = element.TextContainer.Start.CreateNavigator();
            element.TextContainer.MoveToElementEdge(navigator, element, ElementEdge.AfterEnd);
            tree.InsertElement(tree.Start, tree.Start, element);
            tree.ExtractElement(element);

            tree.InsertElement(tree.End, tree.End, new Inline());
            tree.InsertElement(tree.End, tree.End, new Inline());
            navigator = tree.Start.CreateNavigator(+2); // Between the two elements.
            tree.InsertElement(navigator, navigator, element);
            tree.ExtractElement(element);
        }
        // Clone helper
        private object CloneIt (object original)
        {
            return new MyTextElement();
        }

        // Dump an array of characters to the console
        private void DumpChar(char[] carr)
        {
            if (DRT.Verbose)
            {
                ConsoleWrite(">>");
                for (int i = 0; i<carr.Length; i++)
                    Console.Write(carr[i]);
                ConsoleWriteLine("<<");
            }
        }

        public void ConsoleWriteLine(string s)
        {
            if (DRT.Verbose)
                Console.WriteLine(s);
        }
        public void ConsoleWrite(string s)
        {
            if (DRT.Verbose)
                Console.Write(s);
        }
        public static void TextTreeDumperDumpSplay(object o)
        {
        }
        public static void TextTreeDumperDumpGrid(TextContainer a)
        {
        }
        public static void TextTreeDumperDumpTextPositions(TextContainer a)
        {
        }
    }

    /// <summary>
    ///  Test flags enum for parser testing
    /// </summary>
    [Flags]
    public enum TextTestFlags
    {
        /// <summary> </summary>
        one        = 1,
        /// <summary> </summary>
        two        = 2,
        /// <summary> </summary>
        four       = 4,
    }
        
    /// <summary>
    ///     Test control used for TextContainer drts.  The purpose of this class
    ///     is to expose some of the TextContainer apis for testing, and to provide some
    ///     properties used in parser testing.
    /// </summary>
    public class TextTestControl : Text
    {        
        static public int FieldOne = 111;
        static public double FieldTwo = 2.22;
        static public int PropertyThree
        {
            get { return 333; }
        }

        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaProperty = DependencyProperty.RegisterAttached(
            "Alpha", typeof(int), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty GreekAlphaProperty = DependencyProperty.RegisterAttached(
            "GreekAlpha", typeof(int), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty BackgroundDPProperty = DependencyProperty.RegisterAttached(
            "BackgroundDP", typeof(SolidColorBrush), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty ForegroundDPProperty = DependencyProperty.RegisterAttached(
            "ForegroundDP", typeof(SolidColorBrush), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with enums
        /// </summary>
        public static readonly DependencyProperty NumberEnumProperty = DependencyProperty.RegisterAttached(
            "NumberEnum", typeof(TextTestFlags), typeof(TextTestControl));
        /// <summary>
        ///     Test DependencyProperty with IList
        /// </summary>
        private static PropertyMetadata BetaPMD = new PropertyMetadata(null,true);
        public static readonly DependencyProperty BetaIListProperty = DependencyProperty.Register(
            "BetaIList", typeof(MyIList), typeof(TextTestControl), BetaPMD);

        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int Alpha
        {
            get { return (int)GetValue(AlphaProperty); }
            set { SetValue(AlphaProperty,value); }
        }

        ///<summary>
        ///  Test prop for culture
        ///</summary>
        [XmlLang]
        public CultureInfo MyCulture
        { 
            get { return _myCulture; }
            set { _myCulture = value; }
        }
        private CultureInfo _myCulture = null;

        /// <summary>
        ///     Test array property
        /// </summary>
        public MyText[] MyTextArrayProp
        {
            get { return _mtap; }
            set {_mtap = value; }
        }

        /// <summary>
        ///     Test DependencyProperty prop
        /// </summary>
        public SolidColorBrush BackgroundDP
        {
            get { return GetValue(BackgroundDPProperty) as SolidColorBrush; }
            set { SetValue(BackgroundDPProperty,value); }
        }

        /// <summary>
        ///  Test prop that is read only and implements IAddChild
        /// </summary>
        public IAddChild IAddChildCollection
        {
            get { return _iacc; }
        }

        /// <summary>
        ///  Test prop that is read only and implements IEnumerable
        /// </summary>
        public IEnumerable IEnumerableChildren
        {
            get { return _ienum; }
        }

        /// <summary>
        ///  Test prop that is read only and implements IList
        /// </summary>
        public MyIList BetaIList
        {
            get { return (MyIList)GetValue(BetaIListProperty); }
        }

        /// <summary>
        ///  Test prop that is read only and implements IDictionary
        /// </summary>
        public IDictionary IDict
        {
            get { return _idict; }
        }
        
        /// <summary>
        ///     Test DependencyProperty prop
        /// </summary>
        public SolidColorBrush ForegroundDP
        {
            get { return null; }
            set { }
        }

        /// <summary>
        ///     Test DependencyProperty attached prop 
        /// </summary>
        public static void SetGreekAlpha(DependencyObject target, int value)
        {
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static void SetNumberEnum(DependencyObject target, TextTestFlags value)
        {
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static MyIList GetBetaIListDependency(Object target)
        {
            return ((TextTestControl)target).BetaIList;
        }
        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControl() 
        {
             SetValue(BetaIListProperty, new MyIList(777));
        }
        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControl(DependencyObject root) : base(UIContext.CurrentContext)
        {
            _root = root;
            if (root != null)
                ((IAddChild)root).AddChild(this);
            SetValue(BetaIListProperty, new MyIList(777));
        }

        /// <summary>
        ///     TextContainer Change event test.  Used only for TextContainer drts.
        /// </summary>
        public void AddChangeHandler(TextContainerChangedEventHandler h)
        {
            this.TextContainer.Changed += h;
        }

        public TextContainer TextContainer
        {
            get { return (TextContainer)this.TextRange.TextContainer; }
        }
        
        private IACC             _iacc = new IACC();
        private IENUM            _ienum = new IENUM();
        private MyHashtable      _idict = new MyHashtable();
        private MyText[]         _mtap = null;
        private DependencyObject _root;
    }

    // Test FrameworkContentElement class
    public class FCETest : FrameworkContentElement, IAddChild
    {
        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaFCEProperty = DependencyProperty.Register(
            "AlphaFCE", typeof(int), typeof(FCETest));
        
        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int AlphaFCE
        {
            get { return (int)GetValue(AlphaFCEProperty); }
            set { SetValue(AlphaFCEProperty,value); }
        }

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
        }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }
    }
    
    // Test FrameworkContentElement class
    public class FCETestTwo : FrameworkContentElement, IAddChild
    {
        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty AlphaFCE2Property = DependencyProperty.Register(
            "AlphaFCE2", typeof(int), typeof(FCETestTwo));
        
        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int AlphaFCE2
        {
            get { return (int)GetValue(AlphaFCE2Property); }
            set { SetValue(AlphaFCE2Property,value); }
        }

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        ///<summary>
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
        }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }
    }
    
    // Test FrameworkElement class
    public class FETest : FrameworkElement, IAddChild
    {
        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        protected override Size MeasureOverride(Size constraint)
        {
            for (int i = 0; i < _children.Count; i++)
            {
                if (_children[i] is FrameworkContentElement)
                    ((FrameworkContentElement)_children[i]).EnsureLogical();
            }
            return constraint;
        }
        
        /// Called to Add the object as a Child.
        ///</summary>
        ///<param name="o">
        /// Object to add as a child
        ///</param>
        void IAddChild.AddChild(Object o)
        {
            _children.Add(o);
            AddLogicalChild(o);
        }
        ArrayList _children = new ArrayList();

        ///<summary>
        /// Called when text appears under the tag in markup
        ///</summary>
        ///<param name="s">
        /// Text to Add to the Object
        ///</param> 
        void IAddChild.AddText(string s)
        {
            _text = s;
        }
        string _text = null;

        /// <summary>
        ///     Returns children
        /// </summary>
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Content)]
        public ArrayList Kids
        {
            get { return _children; }
        }

        /// <summary>
        ///     Returns enumerator to logical children
        /// </summary>
        protected override IEnumerator LogicalChildren { get { return _children.GetEnumerator(); } }
     }

    /// <summary>
    ///     Test class that implements IDictionary.
    /// </summary>
    public class MyHashtable : Hashtable
    {
        public MyHashtable() : base ()
        {
        }

        public int MYTP
        {
            get { return _mytp; }
            set { _mytp = value; }
        }

        int _mytp;
    }
    
    /// <summary>
    ///     Test class that implements IList.
    /// </summary>
    public class MyIList : ArrayList
    {
        public MyIList() : base ()
        {
        }
        public MyIList(int i) : this ()
        {
            _myitp = i;
        }

        public int MYITP
        {
            get { return _myitp; }
            set { _myitp = value; }
        }

        int _myitp;
    }
    
    /// <summary>
    ///  Test class that implements IAddChild.
    /// </summary>
    public class IACC : IAddChild, IEnumerable
    {
        /// <summary> test </summary>
        public IACC()
        {
           _children = new ArrayList();
        }

        /// <summary> test </summary>
        public int SomeInt
        {
           get { return _int; }
           set { _int = value; }
        }

        /// <summary> test </summary>
        public void AddChild(object o)
        {
           _children.Add(o);
        }

        /// <summary> test </summary>
        public void AddText(string s)
        {
           _children.Add(s);
        }

        public IEnumerator GetEnumerator()
        {
           return _children.GetEnumerator();
        }

        private ArrayList _children;
        private int       _int; 
    }
        
    /// <summary>
    ///  Test class that implements IEnumerable
    /// </summary>
    public class IENUM : IEnumerable
    {
        /// <summary> test </summary>
        public IENUM()
        {
           _children = new ArrayList();
        }

        /// <summary> test </summary>
        public int SomeInt
        {
           get { return _int; }
           set { _int = value; }
        }

        public IEnumerator GetEnumerator()
        {
           return _children.GetEnumerator();
        }

        private ArrayList _children;
        private int       _int;        
    }
        
    /// <summary>
    ///     Test control used for Parser drts.  The sole purpose of this class
    ///     is to expose some DependencyProperties.
    /// </summary>
    public class TextTestControlSubclass : TextTestControl
    {        
        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControlSubclass(TextElement root) : base(root)
        {
        }

        /// <summary>
        ///     Test constructor.  Used only for TextContainer drts.
        /// </summary>
        public TextTestControlSubclass() : base(null)
        {
        }

        /// <summary>
        ///     Test DependencyProperty with default uri
        /// </summary>
        public static readonly DependencyProperty BetaProperty = DependencyProperty.RegisterAttached(
            "Beta", typeof(int), typeof(TextTestControlSubclass));
        /// <summary>
        ///     Test DependencyProperty with new uri
        /// </summary>
        public static readonly DependencyProperty GreekBetaProperty = DependencyProperty.RegisterAttached(
            "GreekBeta", typeof(int), typeof(TextTestControlSubclass));

        /// <summary>
        ///     Test DependencyProperty native prop
        /// </summary>
        public int Beta
        {
            get { return 1; }
            set { }
        }
        
        /// <summary>
        ///     Test DependencyProperty attached prop
        /// </summary>
        public static void SetGreekBeta(DependencyObject target, int value)
        {
        }
    }

    // Generic ContentElement to insert into TextContainer
    public class MyControl : FrameworkElement, IContentHost
    {
#region HitTest
        
        /// <summary>
        ///     Hit tests to the correct ContentElement 
        ///     within the ContentHost that the mouse 
        ///     is over
        /// </summary>
        /// <param name="p">
        ///     Mouse coordinates relative to 
        ///     the ContentHost
        /// </param>
        IInputElement IContentHost.InputHitTest(Point p)
        {
            return this;
        }
        
#endregion HitTest   

        /// <summary> testing </summary>
        public static void Whatever()
        {
        }
    }

    /// <summary> testing </summary>
    public class MyControlsSubclass : MyControl
    {
        /// <summary> testing </summary>
        public static new void Whatever()
        {
        }
    }

    public class MyTextElement : TextElement
    {
    }

    public class MyText : Text
    {
        public MyText() : this(null)
        {
        }

        public MyText(DependencyObject parent) : base(UIContext.CurrentContext)
        {
            if (parent != null)
                ((IAddChild)parent).AddChild(this);
        }
    }
}
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
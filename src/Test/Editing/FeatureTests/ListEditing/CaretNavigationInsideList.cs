// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/ListEditing/CareNavigationInsideList.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;

    using System.Windows.Controls;
    using System.Windows.Documents;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    using Test.Uis.Loggers; 
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;    
    using Test.Uis.Wrappers;
    
    #endregion Namespaces.
   
    /// <summary>Test Caret Navigation inside a list</summary>
    [Test(2, "ListEditing", "CaretNavigationInsideList", MethodParameters = "/TestCaseType=CaretNavigationInsideList", Timeout=300)]
    [TestOwner("Microsoft"), TestBugs("511"), TestTactics("401"), TestWorkItem("51, 52")]
    public class CaretNavigationInsideList : RichEditingBase
    {
        /// <summary>This case test the most important Keyboard action inside list. </summary>
        [TestCase(LocalCaseStatus.Ready, "Caret move, delet, tab, backspace in a List")]
        public void BasicNavigationInSideList()
        {
            EnterFunction("BasicNavigationInSideList");
            Dimension[] dimensions;
            dimensions = new Dimension[] {
                new Dimension("Action", new object[] {"{DELETE}", "{TAB}", "{BACKSPACE}", "{LEFT}", "{RIGHT}", "{UP}", "{DONW}" }),
                new Dimension("CareLocation", new object [] {"", "{RIGHT 1}", "{RIGHT 2}", "{RIGHT 3}", "{RIGHT 4}", "{RIGHT 5}", "{RIGHT 6}", "{RIGHT 7}", "{RIGHT 8}", "{RIGHT 9}", "{RIGHT 10}"}),                
                new Dimension("IsInsideTable", new object [] {false, true}),
            };

            _engine = CombinatorialEngine.FromDimensions(dimensions);
            QueueDelegate(BasicNavigationInSideList_LoopStart);

            EndFunction();
        }

        private void BasicNavigationInSideList_LoopStart()
        {
            EnterFuction("BasicNavigationInSideList_LoopStart");

            _values = new Hashtable();

            string _initialContent = "<List MarkerStyle=\"Disc\">" +
                             "<ListItem><Paragraph>a</Paragraph></ListItem>" +
                             "<ListItem><Paragraph>aaa</Paragraph></ListItem>" +
                             "<ListItem><Paragraph></Paragraph></ListItem>" +
                             "<ListItem><Paragraph></Paragraph>" +
                             "<List MarkerStyle=\"Decimal\"><ListItem><Paragraph></Paragraph></ListItem></List></ListItem>" +
                             "<ListItem><Paragraph>a</Paragraph></ListItem>" +
                             "</List>";

            if (!_engine.Next(_values))
            {
                QueueDelegate(EndTest);
                return;
            }

            MyLogger.Log("\r\n\r\n" + base.RepeatString("*", 30) + "Start a new Combination " + ++_combinationCounter + base.RepeatString("*", 30));

            if ((bool)_values["IsInsideTable"])
                SetInitValue("<Table><TableRowGroup><TableRow><TableCell>" + _initialContent + "</TableCell></TableRow></TableRowGroup></Table>");
            else
                SetInitValue(_initialContent);

            ((RichTextBox)(TextControlWraper.Element)).AcceptsTab = true;

            QueueDelegate(BasicNavigationInSideList_DoAction);

            EndFunction();
        }

        void BasicNavigationInSideList_DoAction()
        {
            EnterFunction("BasicNavigationInSideList_DoAction");

            _action = (string)_values["Action"];
            _careLocation = (string)_values["CareLocation"];
            _isInsideTable = (bool)_values["IsInsideTable"];

            MyLogger.Log(CurrentFunction + " - Action[" + _action + "]");
            MyLogger.Log(CurrentFunction + " - CareLocation[" + _careLocation + "]");
            MyLogger.Log(CurrentFunction + " - IsInsideTable[" + _values["IsInsideTable"] + "]");

            KeyboardInput.TypeString(_careLocation);

            if (_isInsideTable)
                KeyboardInput.TypeString("{RIGHT}");

            QueueDelegate(BasicNavigationInSideList_LastAction);

            EndFunction();
        }

        void BasicNavigationInSideList_LastAction()
        {
            EnterFunction("BasicNavigationInSideList_LastAction");

            _listCount = 2;
            _listItemCount = 6;
            _paragraphCount = 6;

            //Calcuate the expected value
            if (_action == "{BACKSPACE}")
            {
                if (TextControlWraper.ListLevel(TextControlWraper.SelectionInstance.Start) > 1 &&
                    TextControlWraper.IsCaretAtTheEndOfAList && TextControlWraper.IsCaretAtTheBeginningOfAList)
                {
                    _listCount--;
                }
                if (TextControlWraper.IsCaretAtTheBeginningOfAListItem)
                {
                    _listItemCount--;
                }
            }
            if (_action == "{DELETE}")
            {
                if (TextControlWraper.IsCaretFollowedBySubList)
                {
                    _listCount--;
                    _listItemCount--;
                    _paragraphCount--;
                }
                if (TextControlWraper.IsCaretAtTheEndOfAListItem && !TextControlWraper.IsCaretAtTheEndOfAList)
                {
                    _listItemCount--;
                    _paragraphCount--;
                }
                if (TextControlWraper.IsCaretAtTheEndOfAListItem && TextControlWraper.ListLevel(TextControlWraper.SelectionInstance.Start) > 1)
                {
                    _listItemCount--;
                    _paragraphCount--;
                }
            }
            if (_action == "{TAB}" && !(bool)_values["IsInsideTable"])
            {
                //for the first ListItem, Tab should not work.
                if (!((string)_values["CareLocation"] == "" || (string)_values["CareLocation"] == "{RIGHT 9}" || (string)_values["CareLocation"] == "{RIGHT 8}" || (string)_values["CareLocation"] == "{RIGHT 1}") &&
                    TextControlWraper.IsCaretAtTheBeginningOfAListItem
                    && !TextControlWraper.IsCaretFollowedBySubList)
                    _listCount++;
            }

            KeyboardInput.TypeString(_action);

            QueueDelegate(BasicActionsInSideList_OntoNextCase);

            EndFunction();
        }

        void BasicActionsInSideList_OntoNextCase()
        {
            TextRange range;
            string trimmedXaml;

            EnterFunction("BasicActionsInSideList_OntoNextCase");

            range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            trimmedXaml = XamlUtils.TextRange_GetXml(range);

            if (_isInsideTable)
            {
                trimmedXaml = trimmedXaml.Substring(trimmedXaml.IndexOf("<TableCell>", 0) + 11);
                trimmedXaml = trimmedXaml.Substring(0, trimmedXaml.IndexOf("</TableCell>"));
            }

            //evaluations 
            FailedIf(!(Occurency(trimmedXaml, "<List ") + Occurency(trimmedXaml, "<List>") == _listCount),
                "-Failed: expected num of List[" + _listCount.ToString() + "]" + "Check the xamle xaml for actual num: [" + trimmedXaml + "]");

            FailedIf(Occurency(trimmedXaml, "<ListItem") != _listItemCount,
                "- Failed: Number of ListItem expected[" + _listItemCount + "], Actual[" + Occurency(trimmedXaml, "<ListItem") + "]");

            FailedIf(Occurency(trimmedXaml, "<Paragraph") != _paragraphCount,
                "-Failed: ParagraphCount count expected[" + _paragraphCount + "]. Actual[" + Occurency(trimmedXaml, "<Paragraph") + "] Check the xaml[" + trimmedXaml + "]");

            if (!pass)
                EndTest();
            else
                QueueDelegate(BasicNavigationInSideList_LoopStart);

            EndFunction();
        }

        #region - private members

        /// <summary>Engine generating combinations to test.</summary>
        private CombinatorialEngine _engine;

        /// <summary>Hashtable to hold the values for each conbination.</summary>
        private Hashtable _values;

        /// <summary>counter for the combinations.</summary>
        private int _combinationCounter;

        /// <summary>Count of list in the document.</summary>
        int _listCount;

        /// <summary>Count of paragraph in the document.</summary>
        int _paragraphCount;

        /// <summary>Count of listItems in the doucment.</summary>
        int _listItemCount;

        /// <summary>The Keyboard Action to test for a combination.</summary>
        string _action;

        /// <summary>A keyboard string that tell where the caret is to be set.</summary>
        string _careLocation;

        /// <summary>Tells if a combination is runing in a table cell.</summary>
        bool _isInsideTable;

        #endregion
    }
    /// <summary>
    /// Test case for pefrom editing in List
    /// </summary>
    [Test(0, "ListEditing", "EditingInList", MethodParameters = "/TestCaseType=EditingInList")]
    [TestOwner("Microsoft"), TestBugs("512"), TestTactics("402"), TestWorkItem("51, 52")]
    public class EditingInList : ManagedCombinatorialTestCase
    {
        /// <summary>
        /// override method
        /// </summary>
        protected override void DoRunCombination()
        {
            TestElement = new RichTextBox();
            _wrapper = new UIElementWrapper(TestElement);
            _listData.SetupContent(_wrapper);
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(_listData.PerformAction);
            QueueDelegate(_listData.verifyResult);
            QueueDelegate(NextCombination);
        }

        ListEditingData _listData=null;
        UIElementWrapper _wrapper;  
    }

    /// <summary>Data class for ListEditing</summary>
    public class ListEditingData
    {
        string _xaml;
        string _action;
        int _selectionStart;
        int _selectionEnd;
        ListState _expectedState;
        ListState _originalState;
        UIElementWrapper _wrapper;

        /// <summary>
        /// constructor 
        /// </summary>
        /// <param name="xaml"></param>
        /// <param name="action"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="expectedState"></param>
        /// <param name="originalState"></param>
        public ListEditingData(string xaml, string action, int start, int end, ListState expectedState, ListState originalState)
        {
            _xaml = xaml;
            _action = action;
            _selectionStart = start;
            _selectionEnd = end;
            _expectedState = expectedState;
            _originalState = originalState;
        }

        /// <summary>
        /// return the Parameter values.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return "\r\nXaml:[" + _xaml + "]\r\n" +
                "Action:[" + _action + "]\r\n" +
                "SelectionStartIndex:[" + _selectionStart + "]\r\n" +
                "SelectionEndIndex:[" + _selectionEnd + "]\r\n";
        }
        /// <summary>
        /// Set up the content for testing.
        /// </summary>
        /// <param name="wrapper"></param>
        public void SetupContent(UIElementWrapper wrapper)
        {
            Logger.Current.Log("Set up the RichTextBox with content ...");
            _wrapper = wrapper;

            _wrapper.Clear();
            TextRange MyRange = new TextRange(_wrapper.Start, _wrapper.End);
            string xaml = XamlUtils.TextRange_GetXml(MyRange);
            xaml = xaml.Replace("<Paragraph><Run></Run></Paragraph>", _xaml);
            XamlUtils.TextRange_SetXml(MyRange, xaml);
            wrapper.SelectionInstance.Select(FindInsertionPointer(_selectionStart), FindInsertionPointer(_selectionEnd));
        }

        /// <summary>
        /// Call to perfrom the Action. 
        /// </summary>
        public void PerformAction()
        {
            Logger.Current.Log("Verify orginal state...");
            VerifyState(_originalState);

            Logger.Current.Log("PerformAction Keyboard Action ...");
            KeyboardInput.TypeString(_action);
        }

        /// <summary>
        /// Call to perfrom the verificaiton against the expected state.
        /// </summary>
        public void verifyResult()
        {
            VerifyState(_expectedState);
        }

        void VerifyState(ListState state)
        {
            Logger.Current.Log("Verify the state after action ...");
            IList aList = ((RichTextBox)_wrapper.Element).Document.Blocks;
            int count = ElementCount(aList, typeof(System.Windows.Documents.List));
            Verifier.Verify(state.ListCount == count, "List count won't mach! Expected[" + state.ListCount + "], Actual[" + count + "]");
            count = ElementCount(aList, typeof(System.Windows.Documents.ListItem));
            Verifier.Verify(state.ListItemCount == count, "ListItem count won't mach! Expected[" + state.ListItemCount + "], Actual[" + count + "]");
            count = ElementCount(aList, typeof(System.Windows.Documents.Paragraph));
            Verifier.Verify(state.ParagraphCount == count, "Paragraph count won't mach! Expected[" + state.ParagraphCount + "], Actual[" + count + "]");
        }

        TextPointer FindInsertionPointer(int index)
        {
            TextPointer p = _wrapper.Start;
            if (index < 0)
            {
                throw new ArgumentException("Index should not be negative!"); 
            }
            if (!p.IsAtInsertionPosition)
            {
                p = p.GetInsertionPosition(LogicalDirection.Forward);
            }

            while (index > 0)
            {
                p = p.GetNextInsertionPosition(LogicalDirection.Forward);
                index--; 
            }

            return p;
        }

        int ElementCount(IList alist, Type ElementType)
        {
            int count = 0;
            IList subList = null;

            if (alist != null)
            {
                for (int i = 0; i < alist.Count; i++)
                {
                    object obj = alist[i];
                    if (obj.GetType() == ElementType)
                    {
                        count++;
                    }
                    if (obj is Paragraph)
                    {
                        subList = ((Paragraph)obj).Inlines;
                    }
                    else if (obj is List)
                    {
                        subList = ((List)obj).ListItems;
                    }
                    else if (obj is ListItem)
                    {
                        subList = ((ListItem)obj).Blocks;
                    }
                    else if (obj is Span)
                    {
                        subList = ((Span)obj).Inlines;
                    }
                    count += ElementCount(subList, ElementType);
                }
            }
            return count;
        }

        /// <summary>Test case combinations</summary>
        public static ListEditingData[] BVTcases = 
        {
            //Delete:
            new ListEditingData("<List><ListItem><Paragraph></Paragraph></ListItem></List>", "{DELETE}", 0, 0, new ListState(1, 1, 1), new ListState(1, 1, 1)),
            new ListEditingData("<Paragraph></Paragraph><List><ListItem><Paragraph></Paragraph></ListItem></List>", "{DELETE}", 0, 0, new ListState(0, 0, 1), new ListState(1, 1, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem><ListItem><Paragraph>b</Paragraph></ListItem></List>", "{DELETE}", 1, 1, new ListState(1, 1, 1), new ListState(1, 2, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph><List><ListItem><Paragraph>b</Paragraph></ListItem></List></ListItem></List>", "{DELETE}", 1, 1, new ListState(1, 1, 1), new ListState(2, 2, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph><List><ListItem><Paragraph>b</Paragraph></ListItem></List></ListItem></List><Paragraph>c</Paragraph>", "{DELETE}", 3, 3, new ListState(2, 2, 2), new ListState(2, 2, 3)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem></List><List><ListItem><Paragraph>b</Paragraph></ListItem><ListItem><Paragraph>c</Paragraph></ListItem></List>", "{DELETE}", 1, 1, new ListState(1, 2, 2), new ListState(2, 3, 3)),
            //Add more scenarios.

            //BackSpace:
            new ListEditingData("<List><ListItem><Paragraph></Paragraph></ListItem></List>", "{BACKSPACE}", 0, 0, new ListState(0, 0, 1), new ListState(1, 1, 1)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem><ListItem><Paragraph>b</Paragraph></ListItem></List>", "{BACKSPACE}", 0, 0, new ListState(1, 1, 2), new ListState(1, 2, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem><ListItem><Paragraph>b</Paragraph></ListItem></List>", "{BACKSPACE}", 2, 2, new ListState(1, 1, 2), new ListState(1, 2, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph><List><ListItem><Paragraph>b</Paragraph></ListItem></List></ListItem></List>", "{BACKSPACE}", 2, 2, new ListState(1, 1, 2), new ListState(2, 2, 2)),
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem><ListItem><Paragraph>b</Paragraph></ListItem></List>", "{BACKSPACE 2}", 2, 2, new ListState(1, 1, 2), new ListState(1, 2, 2)),
            //when backspace to merge to sibling list, the first backspace delete the Item and the second backspace merge the pargraph to the previous list.
            new ListEditingData("<List><ListItem><Paragraph>a</Paragraph></ListItem></List><List><ListItem><Paragraph>b</Paragraph></ListItem><ListItem><Paragraph>c</Paragraph></ListItem></List>", "{BACKSPACE 2}", 2, 2, new ListState(1, 2, 2), new ListState(2, 3, 3)),
            //Add more scenarios
        };
    }

    /// <summary>Define the state of a List</summary>
    public class ListState
    {
        /// <summary>count the number of Lists</summary>
        public int ListCount;
        /// <summary>count the number of ListItems</summary>
        public int ListItemCount;
        /// <summary>count the number of Paragraphs</summary>
        public int ParagraphCount;
        
        /// <summary>
        /// constructor in set the parameters.
        /// </summary>
        /// <param name="listCount"></param>
        /// <param name="listItemCount"></param>
        /// <param name="paragraphCount"></param>
        public ListState(int listCount, int listItemCount, int paragraphCount)
        {
            ListCount = listCount;
            ListItemCount = listItemCount;
            ParagraphCount = paragraphCount;
        }
    }
}

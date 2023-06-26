// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/ListEditing/ListCreatingTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Globalization;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    #region Creating List using keyboard Shift_ctrl_L, Shift_ctrl_N
    /// <summary></summary>
    [Test(0, "ListEditing", "ListCreatingTest", MethodParameters = "/TestCaseType=ListCreatingTest", Timeout=240)]
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("403"), TestWorkItem("53")]
    public class ListCreatingTest : RichEditingBase
    { 
        /// <summary>creating simple list using keyboard.</summary>
        [TestCase(LocalCaseStatus.Ready, "Split paragraph")]
        public void CreatingListFromParagraphs()
        {
            EnterFunction("CreatingListFromParagraphs");
            Dimension[] dimensions;
            dimensions = new Dimension[] {
                new Dimension("Action", new object[] {"Ctrl_Shift_N", "Ctrl_Shift_L"}),
                new Dimension("SelectionStart", new object [] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}),
                new Dimension("SelectionLength", new object [] {0, 1, 2}),
                new Dimension("IsInsideTable", new object [] {true, false}),
            };
            _engine = CombinatorialEngine.FromDimensions(dimensions);
            QueueDelegate(CreatingListFromParagraphs_FlowBegings);
            EndFunction();
        }
        /// <summary>Retrivew the combination and perform initialization</summary>
        void CreatingListFromParagraphs_FlowBegings()
        {
            EnterFuction("CreatingListFromParagraphs_FlowBegings");
            _values = new Hashtable();
            _initialContent = "<Paragraph>ab</Paragraph><Paragraph>cd</Paragraph><Paragraph>ef</Paragraph><Paragraph>gh</Paragraph>";
        
            if (!_engine.Next(_values))
            {
                QueueDelegate(EndTest);
                return;
            }
            MyLogger.Log("\r\n\r\n" + base.RepeatString("*", 30) + "Start a new Combination" + base.RepeatString("*", 30));
            if ((bool)_values["IsInsideTable"])
                SetInitValue("<Table><TableRowGroup><TableRow><TableCell>" + _initialContent+ "</TableCell></TableRow></TableRowGroup></Table>");
            else
                SetInitValue(_initialContent);
            QueueDelegate(CreatingListFromParagraphs_DoCreating);
            EndFunction();
        }
        /// <summary>Calcuate the action string and expected value after action and then perfrom the keyboard action</summary>
        void CreatingListFromParagraphs_DoCreating()
        {
            EnterFunction("CreatingListFromParagraphs_DoCreating");
            int selectionStart = (int)_values["SelectionStart"];
            string action = (string)_values["Action"];
            int selectionLength = (int)_values["SelectionLength"];
            bool isInsideTable = (bool)_values["IsInsideTable"];

            MyLogger.Log(CurrentFunction + " - SelectionStart[" + selectionStart + "]");
            MyLogger.Log(CurrentFunction + " - SelectionLength[" + selectionLength + "]");
            MyLogger.Log(CurrentFunction + " - Action[" + action + "]");
            MyLogger.Log(CurrentFunction + " - IsInsideTable[" + _values["IsInsideTable"] + "]");
            if (isInsideTable)
                selectionStart++;
            string tempKeyStr = "^{home}{RIGHT " + selectionStart.ToString() + "}+{RIGHT " + selectionLength.ToString() + "}";
            tempKeyStr = (action == "Ctrl_Shift_N") ? tempKeyStr + "^+N" : tempKeyStr + "^+L";
            KeyboardInput.TypeString(tempKeyStr);

            QueueDelegate(CreatingListFromParagraphs_OnToNextCase);

            EndFunction();
        }
        /// <summary>This method evaluates the result and starts the next conbination </summary>
        void CreatingListFromParagraphs_OnToNextCase()
        {
            string[] contentArray = new string[4];
            string expectedString="";
            int selectionStart = (int)_values["SelectionStart"];
            int selectionLength = (int)_values["SelectionLength"];
            string action = (string)_values["Action"];
            bool isInsideTable = (bool)_values["IsInsideTable"];
            string mark =("Ctrl_Shift_N" == action)? "MarkerStyle=\"Decimal\"" : "MarkerStyle=\"Disc\""; 

            //the fomula of caulculating the number of ListItems expected.
            int listItems = (selectionLength + 2+ selectionStart %3)/ 3  ;
            listItems = (listItems == 0) ? 1 : listItems;

            TextRange range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            string trimmedXaml = XamlUtils.TextRange_GetXml(range);
            if (isInsideTable)
            {
                trimmedXaml = trimmedXaml.Substring(trimmedXaml.IndexOf("<TableCell>", 0) + 11);
                trimmedXaml = trimmedXaml.Substring(0, trimmedXaml.IndexOf("</TableCell>"));
            }

            contentArray[0] = "ab\r\n";
            contentArray[1] = "cd\r\n";
            contentArray[2] = "ef\r\n";
            contentArray[3] = "gh\r\n";
            int k, i; 
            for (i=0, k=0; i <=3; i++)
            {

                if (i >= ((selectionStart) / 3) && i < listItems + ((selectionStart) / 3))
                {
                    k++;
                    if (action == "Ctrl_Shift_N")
                    {
                        expectedString += k + ".\t" + contentArray[i];
                    }
                    else
                    {
                        expectedString += "\x2022\t" + contentArray[i];

                    }
                }
                else
                {
                    expectedString += contentArray[i];
                }
                
            }

            //evaluations 
            FailedIf(range.Text != expectedString, CurrentFunction + "- Failed: Text content in RichTextBox is incorrect. Expected["+ expectedString+ "], Actual:[" + range.Text + "]");
            FailedIf(Occurency(trimmedXaml, "<List ") + Occurency(trimmedXaml, "<List>")  != 1, "-Failed: Please check the xaml: [" + trimmedXaml + "]");
            FailedIf(Occurency(trimmedXaml, "<ListItem") != listItems, "- Failed: Number of ListItem expected[" + listItems + "], Actual[" + Occurency(trimmedXaml, "<ListItem") + "]");
            if (action == "Ctrl_Shift_N")
            {
                FailedIf(Occurency(trimmedXaml, "MarkerStyle=\"Decimal\"") != 1, " - Failed: markstyle is incorect, Expected[MarkerStyle=\"Decimal\"]. Check the xaml[" + trimmedXaml + "]");
            }
            else
            {
                //With 
                if (Microsoft.Test.Diagnostics.SystemInformation.WpfVersion == Microsoft.Test.Diagnostics.WpfVersions.Wpf30)
                {
                    FailedIf(Occurency(trimmedXaml, "MarkerStyle") != 0, " - Failed: markstyle is incorect, Expected no MarkStyle(default to Disc). Check the xaml[" + trimmedXaml + "]");
                }
            }
            if (!pass)
                EndTest();
            else
                QueueDelegate(CreatingListFromParagraphs_FlowBegings);
            EndFunction();
        }
        
        #region - private members

        /// <summary>Engine generating combinations to test.</summary>
        private CombinatorialEngine _engine;
        /// <summary>Hashtable to hold the values for each conbination.</summary>
        private Hashtable _values;
        /// <summary>Initial content</summary>
        string _initialContent;

        #endregion 
    }
    #endregion

    #region Enter key to create list/ListItem.
    /// <summary>Enter Key to create list/listitems</summary>
    [Test(0, "ListEditing", "ListItemCreatingTest", MethodParameters = "/TestCaseType=ListItemCreatingTest", Timeout=300)]
    [TestOwner("Microsoft"), TestTactics("404")]
    public class ListItemCreatingTest : RichEditingBase
    {
        /// <summary>ListItem creation using keyboard</summary>
        [TestCase(LocalCaseStatus.Ready, "Split paragraph")]
        public void EnterToCreatingListItems()
        {
            EnterFunction("CreatingListFromParagraphs");
            Dimension[] dimensions;
            dimensions = new Dimension[] {
                new Dimension("InsertionIndex", new object [] {0, 1, 2, 3, 4, 5, 6, 7, 8, 9}),
                //new Dimension("SelectionLength", new object [] {0, 1, 2}),
                new Dimension("Actions", new object [] {1, 2, 3}),
                new Dimension("IsInsideTable", new object [] {true, false}),
            };
            _engine = CombinatorialEngine.FromDimensions(dimensions);
            QueueDelegate(EnterToCreatingListItems_LoopBegings);
            EndFunction();
        }
        /// <summary></summary>
        void EnterToCreatingListItems_LoopBegings()
        {
            EnterFuction("CreatingListFromParagraphs_FlowBegings");
            _values = new Hashtable();
            string _initialContent = "<List>" +
                                "<ListItem><Paragraph><Run></Run></Paragraph></ListItem>" +
                                "<ListItem><Paragraph>abc</Paragraph></ListItem>" +
                                "<ListItem><Paragraph>de</Paragraph></ListItem>" +
                                "<ListItem><Paragraph>f</Paragraph></ListItem>" +
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
            QueueDelegate(EnterToCreatingListItems_DoCreating);
            EndFunction();
        }
        /// <summary>Calcuate the action string and expected value after action and then perfrom the keyboard action</summary>
        void EnterToCreatingListItems_DoCreating()
        {
            EnterFunction("CreatingListFromParagraphs_DoCreating");
            int InsertionIndex = (int)_values["InsertionIndex"];
            int Actions = (int)_values["Actions"];
            bool IsInsideTable = (bool)_values["IsInsideTable"];

            MyLogger.Log(CurrentFunction + " - InsertionIndex[" + InsertionIndex + "]");
            MyLogger.Log(CurrentFunction + " - Actions[" + Actions + "]");
            MyLogger.Log(CurrentFunction + " - IsInsideTable[" + _values["IsInsideTable"] + "]");
            if (IsInsideTable)
                InsertionIndex++;
            string tempKeyStr = "^{home}{RIGHT " + InsertionIndex.ToString() + "}{ENTER " + Actions.ToString() + "}";
            KeyboardInput.TypeString(tempKeyStr);

            QueueDelegate(EnterToCreatingListItems_OnToNextCase);

            EndFunction();
        }
        /// <summary>This method evaluates the result and starts the next conbination </summary>
        void EnterToCreatingListItems_OnToNextCase()
        {
            int InsertionIndex = (int)_values["InsertionIndex"];
            int Actions = (int)_values["Actions"];
            bool IsInsideTable = (bool)_values["IsInsideTable"];
            int ListCount, ListItemCount, ParagraphCount;
            if (Actions == 1)
            {
                if (InsertionIndex == 0)
                {
                    ParagraphCount = 4;
                    ListItemCount = 3;
                    ListCount = 1;
                }
                else
                {
                    ParagraphCount = ListItemCount = 5;
                    ListCount = 1;
                }
            }
            else
            {
                if (InsertionIndex == 0)
                {
                    ParagraphCount = 4 + Actions - 1;
                    ListItemCount = 3;
                    ListCount = 1;
                }
                else if (InsertionIndex == 9)
                {
                    ParagraphCount = 4 + Actions - 1;
                    ListItemCount = 4;
                    ListCount = 1;
                }
                else if (InsertionIndex == 4 || InsertionIndex == 7)
                {
                    ParagraphCount = 4 + Actions - 1;
                    ListItemCount = 4;
                    ListCount = 2;
                }
                else
                {
                    ParagraphCount = ListItemCount = 4 + Actions;
                    ListCount = 1;
                }
            }

            TextRange range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            string trimmedXaml = XamlUtils.TextRange_GetXml(range);
            if (IsInsideTable)
            {
                trimmedXaml = trimmedXaml.Substring(trimmedXaml.IndexOf("<TableCell>", 0) + 11);
                trimmedXaml = trimmedXaml.Substring(0, trimmedXaml.IndexOf("</TableCell>"));
            }
            //evaluations 
            FailedIf(!(Occurency(trimmedXaml, "<List >") == ListCount || Occurency(trimmedXaml, "<List>") == ListCount),
                "-Failed: expected num of List[" + ListCount.ToString() + "]" + "Check the xamle xaml for actual num: [" + trimmedXaml + "]");
            FailedIf(Occurency(trimmedXaml, "<ListItem") != ListItemCount,
                "- Failed: Number of ListItem expected[" + ListItemCount + "], Actual[" + Occurency(trimmedXaml, "<ListItem") + "]");
            FailedIf(Occurency(trimmedXaml, "<Paragraph") != ParagraphCount,
                "-Failed: Paragraph count expected[" + ParagraphCount + "]. Actual[" + Occurency(trimmedXaml, "<Paragraph") + "] Check the xaml[" + trimmedXaml + "]");
            if (!pass)
                EndTest();
            else
                QueueDelegate(EnterToCreatingListItems_LoopBegings);
            EndFunction();
        }

        #region - private members
        /// <summary>Engine generating combinations to test.</summary>
        private CombinatorialEngine _engine;
        /// <summary>Hashtable to hold the values for each conbination.</summary>
        private Hashtable _values;
        /// <summary>counter for the combinations</summary>
        private int _combinationCounter = 0;
        #endregion
    }
    #endregion

    /// <summary>Tests applying flowdirection on lists</summary>
    [Test(0, "ListEditing", "ListFlowDirection1", MethodParameters = "/TestCaseType:ListFlowDirection")]
    [Test(0, "PartialTrust", TestCaseSecurityLevel.FullTrust, "ListFlowDirection2", MethodParameters = "/TestCaseType:ListFlowDirection /InputMonitorEnabled:False /XbapName=EditingTestDeploy", Timeout = 200)]
    [TestOwner("Microsoft"), TestTactics("405,406"), TestWorkItem("54"), TestBugs("513, 514,515,516,517,518,519"), TestLastUpdatedOn("June 7, 2006")]
    public class ListFlowDirection : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {

            _element = _editableType.CreateInstance();
            if ((_element is RichTextBox)&&(KeyboardInput.IsBidiInputLanguageInstalled()))
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Clear();
                TestElement = _element;
                _rtb = _element as RichTextBox;
                _rtb.Document.Blocks.Clear();
                _rtb.Document.Blocks.Add(CreateListThroughXaml());

                if (_listVariationNumber == 0)
                {
                    TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
                    TextPointer tp = tr.Start;

                    for (int i = 0; i < 6; i++)
                    {
                        tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                    Paragraph p = tp.Paragraph;

                    Image simpleImage = new Image();
                    simpleImage.Width = 200;
                    simpleImage.Margin = new Thickness(5);

                    //Create source
                    System.Windows.Media.Imaging.BitmapImage bi = new System.Windows.Media.Imaging.BitmapImage();
                    //BitmapImage.UriSource must be in a BeginInit/EndInit block
                    bi.BeginInit();
                    bi.UriSource = new Uri(@"pack://siteoforigin:,,,/test.png");
                    bi.EndInit();
                    //set image source
                    simpleImage.Source = bi;

                    p.Inlines.Add(new InlineUIContainer(simpleImage));
                }

                _pointerArrayLength = 0;
                _selectedText = "";


                _firstRegionStart = null;
                _firstRegionEnd = null;
                _secRegionStart = null;
                _secRegionEnd = null;
                _thirdRegionStart = null;
                _thirdRegionEnd = null;
                _nodesCount = 0;

                for (int i = 0; i < 10; i++)
                {
                    _startRange[i] = null;
                    _endRange[i] = null;
                    _nodes[i] = 0;
                }

                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        private void DoFocus()
        {
            _rtb.Focus();
           
            KeyboardInput.TypeString("^{HOME}");
            QueueDelegate(SelectCharacters);
        }

        private void SelectCharacters()
        {
            SelectChars();

            QueueDelegate(ChangeFlowDirection);
        }

        private void ChangeFlowDirection()
        {
            string str = _rtb.Selection.Text;
            if ((str.IndexOf("\t", str.Length - 1) == (str.Length - 1)) && (str.Length >4))
            {
                str += "20";
            }
            if ((str.IndexOf(" ", str.Length - 1) == (str.Length - 1)) && (str.Length > 4))
            {
                str += "20";
            }
            str = str.Replace("�\t \r\n", "�\t20\r\n");
            str = str.Replace("�\t�\t", "20\r\n�\t");
            str = str.Replace("�\t", "");
            _selectedText = (str != "") ? (str) : _rtb.Selection.Start.GetTextInRun(LogicalDirection.Forward);
            if (_selectedText == "")
            {
                _selectedText = "20";
            }
            KeyboardEditingData[] data = KeyboardEditingData.GetValues(KeyboardEditingTestValue.ControlShiftRight);
            data[0].PerformAction(_controlWrapper, null, true);

            QueueDelegate(CheckFlowDirection);
        }

        private void CheckFlowDirection()
        {
            Log("\r\n SELECTED TEXT IS -----[" + _selectedText + "]---------------------------\r\n");
            SetNodes();
            InitializePointerArrays();
            SetRegionPointers();
            QueueDelegate( VerifyFlowOnFirstPart);            
        }

        private void VerifyFlowOnFirstPart()
        {
            if (_firstRegionStart != null)
            {
                _rtb.Selection.Select(_firstRegionStart, _firstRegionEnd);
                VerificationHelperLTR();
            }
            _rtb.Selection.Select(_secRegionStart, _secRegionEnd);
            QueueDelegate(VerifyFlowOnSecPart);
        }

        private void VerifyFlowOnSecPart()
        {
            _rtb.Selection.Select(_secRegionStart, _secRegionEnd);
            VerificationHelperRTL();
            if (_thirdRegionStart != null)
            {
                _rtb.Selection.Select(_thirdRegionStart, _thirdRegionEnd);
                QueueDelegate(VerifyFlowOnThirdPart);
            }
            else
            {
                QueueDelegate(NextCombination);
            }
        }

        private void VerifyFlowOnThirdPart()
        {
            VerificationHelperLTR();
            QueueDelegate(NextCombination);
        }

        private void VerificationHelperLTR()
        {
            Verifier.Verify(_rtb.Selection.GetPropertyValue(Paragraph.FlowDirectionProperty).ToString().Contains(FlowDirection.LeftToRight.ToString()),
            "Expected FD on Text ["+ _rtb.Selection.Text +"] is [" + FlowDirection.LeftToRight.ToString() + "] Actual [" +
            _rtb.Selection.GetPropertyValue(Paragraph.FlowDirectionProperty).ToString() + "]", true);
        }

        private void VerificationHelperRTL()
        {
            Paragraph _beginPara = _rtb.Selection.Start.Paragraph;
            Paragraph _endPara = _rtb.Selection.End.GetNextInsertionPosition(LogicalDirection.Backward).Paragraph;

            Verifier.Verify(_beginPara.FlowDirection.ToString().Contains(FlowDirection.RightToLeft.ToString()),
                "Expected FD on ParaBegin is ---[" + FlowDirection.RightToLeft.ToString() + "] Actual [" +
                _beginPara.FlowDirection.ToString() + "]", true);
            Verifier.Verify(_endPara.FlowDirection.ToString().Contains(FlowDirection.RightToLeft.ToString()),
                "Expected FD on ParaEnd is ---[" + FlowDirection.RightToLeft.ToString() + "] Actual [" +
                _endPara.FlowDirection.ToString() + "]", true);

            if ((_rtb.Selection.End.GetNextInsertionPosition(LogicalDirection.Backward).Paragraph.Inlines.FirstInline is InlineUIContainer)||
                (((Run)(_rtb.Selection.End.GetNextInsertionPosition(LogicalDirection.Backward).Paragraph.Inlines.FirstInline)).Text==""))
            {
            }
            else
            {
                Verifier.Verify(_rtb.Selection.GetPropertyValue(Block.FlowDirectionProperty).ToString().Contains(FlowDirection.LeftToRight.ToString()),
                    "Expected FD on selected Text is ---[" + FlowDirection.LeftToRight.ToString() + "] Actual [" +
                    _rtb.Selection.GetPropertyValue(Block.FlowDirectionProperty).ToString() + "]", true);
            }
            List temp =  ((List)((ListItem)((Paragraph)_rtb.Selection.Start.Paragraph).Parent).Parent);
            Verifier.Verify(temp.FlowDirection.ToString().Contains(FlowDirection.RightToLeft.ToString()),
                "Expected FD on List is *** [" + FlowDirection.RightToLeft.ToString() + "] Actual [" +
                temp.FlowDirection.ToString() + "]", true);

        }

        private void SelectChars()
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentStart);
            
            TextPointer _start =tr.Start;
            if (_startLevel == 2)
            {
                _start = _start.GetNextInsertionPosition(LogicalDirection.Forward);
                _start = _start.GetNextInsertionPosition(LogicalDirection.Forward);
                _start = _start.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            _rtb.CaretPosition = _start;
            TextPointer _end = _start;
            _end = GetPointerPosition(_end);
            _rtb.Selection.Select(_start, _end);
        }

        private TextPointer GetPointerPosition(TextPointer _end)
        {
            int count = 0;
            TextPointer _start = _end;
            for (int i = 0; i <= _numberOfCharsSelected; i++)
            {
                if (i > 0)
                {
                    if ((_start.GetTextInRun(LogicalDirection.Forward) == "") && (count == 0))
                    {
                        _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                        if (_listVariationNumber == 0)
                        {
                            _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                        }
                        count++;
                    }
                    else
                    {
                        _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                        
                        _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                        _end = (i > 1) ? _end.GetNextInsertionPosition(LogicalDirection.Forward) : _end;
                        _start = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                }
            }
            return _end;
        }

        /// <summary>Create context menu through XAML.</summary>
        private List CreateListThroughXaml()
        {            
            string _listXaml = _listVariations[_listVariationNumber];
            List _list = XamlUtils.ParseToObject(_listXaml) as List;
            return _list;
        }

        private void InitializePointerArrays()
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentStart);
            TextPointer _start = tr.Start;
            TextPointer _end;
            int i=0;

            _startRange[i] = _start;
            _end = _start.GetNextInsertionPosition(LogicalDirection.Forward);
            _end = _endRange[i] = _end.GetNextInsertionPosition(LogicalDirection.Forward);

            int count = 0;
            while (_end.GetNextInsertionPosition(LogicalDirection.Forward) != null)
            {
                i++;
                _start = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                if ((_start.GetTextInRun(LogicalDirection.Forward) == "")&&(count==0))
                {
                    _end = _start;
                    if (_listVariationNumber == 0)
                    {
                        _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                    }
                    count++;
                }
                else
                {
                    _end = _start.GetNextInsertionPosition(LogicalDirection.Forward);
                    _end = _end.GetNextInsertionPosition(LogicalDirection.Forward);
                }
                 _startRange[i] = _start;
                 _endRange[i] = _end;
            }
            i++;
            _pointerArrayLength = i;
        }

        private void SetNodes()
        {
            string str = _selectedText;
            str = str.Replace("\r\n","|");
            int index =0;
            int i=0;
            while (str.IndexOf("|", index) != -1)
            {
                int temp =index;
                index = str.IndexOf("|", index);
                string tempStr = str.Substring(temp, index - temp);
                _nodes[i] = Int32.Parse(tempStr);
                i++;

                index = index + 1;
            }
            int temp1 = index;
            string tempStr1 = str.Substring(temp1, str.Length - temp1);
            _nodes[i] = Int32.Parse(tempStr1);
            i++;
            _nodesCount = i;

        }

        private void SetRegionPointers()
        {
            int _regionStart = 100;
            int _regionEnd = 0;
            for (int i = 0; i < _nodesCount; i++)
            {
                int val = _nodes[i];
                int nodeNum = val / 10;
                int childNodeCount = val % 10;
                _regionStart = (nodeNum < _regionStart) ? nodeNum : _regionStart;

                int _possibleRegionEnd = nodeNum+ childNodeCount;
                _regionEnd = (_possibleRegionEnd > _regionEnd) ? _possibleRegionEnd : _regionEnd;
            }

            Log("\r\n########Selection Start Node[" + _regionStart.ToString() + "] Selection End Node[" + _regionEnd.ToString() + "]########\r\n");

            if (_regionStart > 0)
            {
                _firstRegionStart = _startRange[0];
                _firstRegionEnd = _endRange[_regionStart - 1];
            }
            _secRegionStart = _startRange[_regionStart];
            _secRegionEnd = _startRange[_regionEnd];
            if (_regionEnd < _pointerArrayLength)
            {
                _thirdRegionStart = _startRange[_regionEnd + 1];
                _thirdRegionEnd = _endRange[_pointerArrayLength - 1];
            }

        }

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private int _numberOfCharsSelected = 0;
        private int _listVariationNumber = 0;
        
        private string[] _listVariations = 
        {

    "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
        "<ListItem><Paragraph>02</Paragraph>"+
            "<List>"+
                "<ListItem><Paragraph>11</Paragraph>"+
                "</ListItem>"+
                "<ListItem><Paragraph></Paragraph></ListItem>"+
          "</List>"+
        "</ListItem>"+
        "<ListItem><Paragraph>30</Paragraph></ListItem>"+
     "<ListItem><Paragraph>40</Paragraph></ListItem>"+
    "</List>",
    
    "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
        "<ListItem><Paragraph>02</Paragraph>"+
	        "<List>"+
		        "<ListItem><Paragraph>11</Paragraph>"+
		        "</ListItem>"+
		        "<ListItem></ListItem>"+
		  "</List>"+
        "</ListItem>"+
        "<ListItem><Paragraph>30</Paragraph></ListItem>"+
	 "<ListItem><Paragraph>40</Paragraph></ListItem>"+
    "</List>",
    
    "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
        "<ListItem><Paragraph>03</Paragraph>"+
	        "<List>"+
		        "<ListItem><Paragraph>11</Paragraph>"+
			        "<List>"+
				        "<ListItem><Paragraph>20</Paragraph></ListItem>"+
			        "</List>"+
		        "</ListItem>"+
            //"</List>"+
            //"<List>"+
		        "<ListItem><Paragraph>30</Paragraph></ListItem>"+
		  "</List>"+
        "</ListItem>"+
        "<ListItem><Paragraph>40</Paragraph></ListItem>"+
    "</List>",

    "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>" +
        "<ListItem><Paragraph>02</Paragraph>"+
	        "<List>"+
		        "<ListItem><Paragraph>11</Paragraph>"+
			        "<List>"+
				        "<ListItem><Paragraph>20</Paragraph></ListItem>"+
			        "</List>"+
		        "</ListItem>"+
	        "</List>"+
        "</ListItem>"+
        "<ListItem><Paragraph>30</Paragraph></ListItem>"+
    "</List>",

};
        private RichTextBox _rtb = null;
        private int _startLevel = 0;
        private TextPointer[] _startRange = new TextPointer[10];
        private TextPointer[] _endRange = new TextPointer[10];
        private int _pointerArrayLength = 0;
        private string _selectedText = "";


        private TextPointer _firstRegionStart = null;
        private TextPointer _firstRegionEnd = null;
        private TextPointer _secRegionStart = null;
        private TextPointer _secRegionEnd = null;
        private TextPointer _thirdRegionStart = null;
        private TextPointer _thirdRegionEnd = null;
        private int[] _nodes = new int[10];
        private int _nodesCount = 0;

        #endregion Private Data.
    }

    /// <summary>Tests DeleteOperationsOnBUIC</summary>
    [Test(2, "RichEditing", "DeleteOperationsOnBUIC", MethodParameters = "/TestCaseType:DeleteOperationsOnBUIC")]
    [TestOwner("Microsoft"), TestTactics("407"), TestWorkItem("55"), TestBugs("520")]
    public class DeleteOperationsOnBUIC : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Clear();
                TestElement = _element;
                _rtb = _element as RichTextBox;
                _rtb.Document.Blocks.Clear();
                _rtb.Document.Blocks.Add(CreateContentThroughXaml());

                QueueDelegate(DoFocus);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>gO TO end of first line</summary>
        private void DoFocus()
        {
            _rtb.Focus();
            KeyboardInput.TypeString("^{HOME}{RIGHT}");
            QueueDelegate(CheckCaretPostionInFirstPara);
        }

        /// <summary>Check text on first line and perform delete</summary>
        private void CheckCaretPostionInFirstPara()
        {
            VerifyCaretInFirstPara();
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(CheckCaretPostionAfterFirstDelete);
        }

        /// <summary>Check text on first delete and perform second delete</summary>
        private void CheckCaretPostionAfterFirstDelete()
        {
            VerifyCaretAfterFirstDelete();
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(CheckCaretPostionAfterSecDelete);
        }

        /// <summary>Check text on sec delete and perform third delete</summary>
        private void CheckCaretPostionAfterSecDelete()
        {
            VerifyCaretAfterSecDelete();
            KeyboardInput.TypeString("{DELETE}");
            QueueDelegate(CheckCaretPostionAfterThirdDelete);
        }


        /// <summary>Check text on third delete </summary>
        private void CheckCaretPostionAfterThirdDelete()
        {
            VerifyCaretAfterThirdDelete();
            QueueDelegate(NextCombination);
        }

        #region Helpers.

        private void VerifyCaretInFirstPara()
        {
            VerifyInRun("", true);
            VerifyInRun("A", false);
            VerifyCaretInCorrectPara(_firstPara);
        }

        private void VerifyCaretAfterFirstDelete()
        {
            if (_contentVariationNumber == 3)
            {
                VerifyInRun("B", true);
                VerifyInRun("A", false);
                VerifyBlockCount(2);
                VerifyCaretInCorrectPara(_firstPara);
            }
            else
                if (_contentVariationNumber == 2)
                {
                    VerifyInRun("", true);
                    VerifyInRun("A", false);
                    VerifyBlockCount(2);
                    VerifyCaretInCorrectPara(_firstPara);
                }
                else
                {
                    VerifyInRun("", true);
                    VerifyInRun("A", false);
                    if (_contentVariationNumber != 0)
                    {
                        VerifyBlockCount(2);
                    }
                    VerifyCaretInCorrectPara(_firstPara);
                }
        }

        private void VerifyCaretAfterSecDelete()
        {
            if (_contentVariationNumber == 3)
            {
                VerifyInRun("", true);
                VerifyInRun("A", false);
                VerifyBlockCount(2);
                VerifyCaretInCorrectPara(_firstPara);
            }
            else
                if (_contentVariationNumber == 2)
                {
                    VerifyInRun("", true);
                    VerifyInRun("A", false);
                    VerifyBlockCount(1);
                    VerifyCaretInCorrectPara(_firstPara);
                }
                else
                {
                    VerifyInRun("", true);
                    VerifyInRun("", false);
                    if (_contentVariationNumber != 0)
                    {
                        VerifyBlockCount(2);
                    }
                    VerifyCaretInCorrectPara(_secPara);
                }
        }

        private void VerifyCaretAfterThirdDelete()
        {
            if (_contentVariationNumber == 3)
            {
                VerifyInRun("", true);
                VerifyInRun("", false);
                VerifyBlockCount(2);
                VerifyCaretInCorrectPara(_secPara);
            }
            else
                if (_contentVariationNumber == 2)
                {
                    VerifyInRun("", true);
                    VerifyInRun("", false);
                    VerifyBlockCount(1);
                    VerifyCaretInCorrectPara(_secPara);
                }
                else
                {
                    VerifyInRun("", true);
                    VerifyInRun("A", false);
                    if (_contentVariationNumber != 0)
                    {
                        VerifyBlockCount(2);
                    }
                    //This is actually 3 since there is an empty secondparagraph 
                    //so even though we pass 2 as paragraph number, the calculation 
                    //ensures that we are on the third para.
                    VerifyCaretInCorrectPara(_firstPara);
                }
        }

        private void VerifyInRun(string str, bool direction)
        {
            if (direction == true)
            {
                Verifier.Verify(_rtb.CaretPosition.GetTextInRun(LogicalDirection.Forward) == str, "string where Caret is present Expected[" + str +
                     "] Actual [" + _rtb.CaretPosition.GetTextInRun(LogicalDirection.Forward) + "]", true);
            }
            else
            {
                Verifier.Verify(_rtb.CaretPosition.GetTextInRun(LogicalDirection.Backward) == str, "string where Caret is present Expected[" + str +
                    "] Actual [" + _rtb.CaretPosition.GetTextInRun(LogicalDirection.Backward) + "]", true);
            }
        }

        private void VerifyCaretInCorrectPara(int paraNumber)
        {
            TextRange tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentStart);
            TextPointer tp = tr.Start;

            for (int i = 0; i < paraNumber; i++)
            {
                tp = tp.GetNextInsertionPosition(LogicalDirection.Forward);
            }
            Verifier.Verify(tp.Paragraph == _rtb.CaretPosition.Paragraph, "Caret Postion is not positioned on the correct paragraph #" +
                paraNumber.ToString(), false);
        }

        private void VerifyBlockCount(int count)
        {
            if (_contentVariationNumber == 3)
            {
                Section _section = _rtb.Document.Blocks.FirstBlock as Section;
                Verifier.Verify(_section.Blocks.Count == count, "Number of Blocks Expected [" + count.ToString() + "] Actual [" +
                    _section.Blocks.Count.ToString() + "]", true);
            }
            else
            {
                List _list = _rtb.Document.Blocks.FirstBlock as List;

                Verifier.Verify(_list.ListItems.Count == count, "Number of Blocks Expected [" + count.ToString() + "] Actual [" +
                   _list.ListItems.Count.ToString() + "]", true);
            }
        }
        

        private Block CreateContentThroughXaml()
        {

            string _contentXaml = _contentVariations[_contentVariationNumber];
            Block _content = XamlUtils.ParseToObject(_contentXaml) as Block;
            return _content as Block;
        }

        #endregion Helpers.


        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;

        private int _firstPara = 1;
        private int _secPara = 2;
        private int _contentVariationNumber = 0;
        private string[] _contentVariations = 
        {
            #region 1.
            "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<ListItem>"+
                  "<Paragraph>A</Paragraph>"+
                  "<List MarkerStyle=\"Disc\">"+
                    "<ListItem>"+
                      "<BlockUIContainer>"+
                        "<Button />"+
                      "</BlockUIContainer>"+
                      "<List MarkerStyle=\"Disc\">"+
                        "<ListItem>"+
                          "<BlockUIContainer>"+
                            "<Button />"+
                          "</BlockUIContainer>"+
                        "</ListItem>"+
                      "</List>"+
                    "</ListItem>"+
                  "</List>"+
                "</ListItem>"+
              "</List>",
            #endregion 1.

            #region 2.    
            "<List  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<ListItem>"+
                  "<Paragraph>A</Paragraph>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<BlockUIContainer>"+
                    "<Button />"+
                  "</BlockUIContainer>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<BlockUIContainer>"+
                    "<Button />"+
                  "</BlockUIContainer>"+
                "</ListItem>"+
            "</List>",
            #endregion 2.

            #region 3.
            "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<ListItem>"+
                  "<Paragraph>A</Paragraph>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<Paragraph></Paragraph>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<BlockUIContainer>"+
                    "<Button />"+
                  "</BlockUIContainer>"+
                "</ListItem>"+
            "</List>",
            #endregion 3.

            #region 4.
            "<Section xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<Paragraph>A</Paragraph>"+
                "<Paragraph>B</Paragraph>"+
                "<BlockUIContainer>"+
                  "<Button />"+
                "</BlockUIContainer>"+
            "</Section>",
            #endregion 4.

        };
        
        #endregion Private Data.
    }

     /// <summary>Tests tab /sift tab on lists</summary>
    [Test(0, "ListEditing", "TabOperationsOnList", MethodParameters = "/TestCaseType:TabOperationsOnList")]
    [TestOwner("Microsoft"), TestTactics("408"), TestWorkItem("56"), TestBugs("521")]
    public class TabOperationsOnList : ManagedCombinatorialTestCase
    {
        /// <summary>initialization of the run</summary>
        protected override void DoRunCombination()
        {
            _element = _editableType.CreateInstance();
            if (_element is RichTextBox)
            {
                _controlWrapper = new UIElementWrapper(_element);
                _controlWrapper.Clear();
                TestElement = _element;
                _rtb = _element as RichTextBox;
                _rtb.Document.Blocks.Clear();
                _rtb.AcceptsTab = true;
                _rtb.Document.Blocks.Add(CreateContentThroughXaml());
                QueueDelegate(PlaceCaretAtCtrlHome);
            }
            else
            {
                NextCombination();
            }
        }

        /// <summary>go to beginning of first line</summary>
        private void PlaceCaretAtCtrlHome()
        {
            _rtb.Focus();
            KeyboardInput.TypeString("^{HOME}{TAB}");
            QueueDelegate(VerifyTabDoesntWorkOnTopListItem);
        }

        /// <summary>VerifyTabDoesntWorkOnTopListItem</summary>
        private void VerifyTabDoesntWorkOnTopListItem()
        {
            VerifyBlockCount(2);
            KeyboardInput.TypeString("{HOME}+{TAB}");
            QueueDelegate(VerifyShiftTabWorksOnTopListItem);
        }

        /// <summary>VerifyShiftTabWorksOnTopListItem</summary>
        private void VerifyShiftTabWorksOnTopListItem()
        {
            Verifier.Verify(_rtb.Document.Blocks.Count == 2, "Expected Number of Blocks [2] Actual [" +
                _rtb.Document.Blocks.Count.ToString() + "]", true);
            try
            {
                if(_listVariationNumber ==1)
                {
                     Paragraph p = _rtb.Document.Blocks.FirstBlock as Paragraph;
                     p.AllowDrop = true;
                }
                else
                {
                    BlockUIContainer buic = _rtb.Document.Blocks.FirstBlock as BlockUIContainer;
                    buic.IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                if (_listVariationNumber == 1)
                {
                    Log("Expecting Paragraph as first element in RTB");
                }
                else
                {
                    Log("Expecting BUIC as first element in RTB");
                }
                throw new ApplicationException(e.ToString());
            }
            KeyboardInput.TypeString("^z{HOME}{DOWN}{TAB}");
            QueueDelegate(VerifyTabWorksOnSubItem);
        }

        /// <summary>VerifyTabWorksOnSubItem</summary>
        private void VerifyTabWorksOnSubItem()
        {
            VerifyBlockCount(1);
            VerifyListItemIsTabbed();
            KeyboardInput.TypeString("{HOME}+{TAB}");
            QueueDelegate(VerifyShiftTabWorksOnSubItem);
        }

        /// <summary>VerifyShiftTabWorksOnSubItem</summary>
        private void VerifyShiftTabWorksOnSubItem()
        {
            VerifyBlockCount(2);
            KeyboardInput.TypeString("{HOME}+{TAB}");
            QueueDelegate(VerifySecondShiftTabWorksOnSubItem);
        }

        /// <summary>VerifySecondShiftTabWorksOnSubItem</summary>
        private void VerifySecondShiftTabWorksOnSubItem()
        {
            Verifier.Verify(_rtb.Document.Blocks.Count == 2, "Expected Number of Blocks [2] Actual [" +
                 _rtb.Document.Blocks.Count.ToString() + "]", true);
            try
            {
                if (_listVariationNumber == 0)
                {
                    Paragraph p = _rtb.Document.Blocks.LastBlock as Paragraph;
                    p.AllowDrop = true;
                }
                else
                {
                    BlockUIContainer buic = _rtb.Document.Blocks.LastBlock as BlockUIContainer;
                    buic.IsEnabled = true;
                }
            }
            catch (Exception e)
            {
                if (_listVariationNumber == 0)
                {
                    Log("Expecting Paragraph as first element in RTB");
                }
                else
                {
                    Log("Expecting BUIC as first element in RTB");
                }
                throw new ApplicationException(e.ToString());
            }
            QueueDelegate(NextCombination);
        }

        #region Helpers.

        /// <summary>CreateContentThroughXaml</summary>
        private Block CreateContentThroughXaml()
        {
            string _contentXaml = _contentVariations[_listVariationNumber];
            Block _content = XamlUtils.ParseToObject(_contentXaml) as Block;
            return _content as Block;
        }

        /// <summary>VerifyBlockCount</summary>
        private void VerifyBlockCount(int count)
        {
            List _list = _rtb.Document.Blocks.FirstBlock as List;

            Verifier.Verify(_list.ListItems.Count == count, "Number of Blocks Expected [" + count.ToString() + "] Actual [" +
               _list.ListItems.Count.ToString() + "]", true);
        }

        /// <summary>VerifyListItemIsTabbed</summary>
        private void VerifyListItemIsTabbed()
        {
            List l = _rtb.Document.Blocks.FirstBlock as List;
            ListItem l1 = l.ListItems.FirstListItem;
            List sublist = l1.Blocks.LastBlock as List;
            if (_listVariationNumber == 0)
            {
                Paragraph para = sublist.ListItems.FirstListItem.Blocks.FirstBlock as Paragraph;
                Verifier.Verify(para == _rtb.CaretPosition.Paragraph, "Expected caret to be inside a para Actual [" + _rtb.CaretPosition.Paragraph.ToString() + "]", true);
            }
            else
            {
                BlockUIContainer buic = sublist.ListItems.FirstListItem.Blocks.FirstBlock as BlockUIContainer;
                Verifier.Verify(buic == _rtb.CaretPosition.Parent as BlockUIContainer, "Expected caret to be inside a buic Actual [" + _rtb.CaretPosition.Parent.ToString() + "]", true);
            }
        }

        #endregion Helpers.

        #region Private Data.

        private UIElementWrapper _controlWrapper;
        private TextEditableType _editableType = null;
        private FrameworkElement _element;
        private RichTextBox _rtb = null;

        private int _listVariationNumber = 0;
        private string[] _contentVariations = 
        {
            #region 1.
            "<List xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<ListItem>"+
                  "<BlockUIContainer>"+
                    "<Button />"+
                  "</BlockUIContainer>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<Paragraph>A</Paragraph>"+
                "</ListItem>"+
            "</List>",
            #endregion 1.

            #region 2.    
            "<List  xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'>"+
                "<ListItem>"+
                  "<Paragraph>A</Paragraph>"+
                "</ListItem>"+
                "<ListItem>"+
                  "<BlockUIContainer>"+
                    "<Button />"+
                  "</BlockUIContainer>"+
                "</ListItem>"+
            "</List>",
            #endregion 2.
        };

        #endregion Private Data.
    }

    /// <summary>
    /// Editing: {BackSpace}delete a listItem when caret is not at the beginning of the listItem.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("448"), TestLastUpdatedOn("Jun 28, 2006")]
    public class RegressionTest_448 : CombinedTestCase
    {
        UIElementWrapper _wrapper;
        string _text;
       
        /// <summary>start to run the test.</summary>
        public override void RunTestCase()
        {
            List list; 
            ListItem item;
            RichTextBox richTextBox;    
            _text = "a" + ((char)0x8001).ToString();
            list = new List();
            item = new ListItem(new Paragraph(new Run(_text)));
            list.ListItems.Clear();
            list.ListItems.Add(item);
            
 
            richTextBox = new RichTextBox();
            _wrapper = new UIElementWrapper(richTextBox);
            MainWindow.Content = richTextBox;
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(list);

            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(DoBackSpace);
        }

        void DoBackSpace()
        {
            Verifier.Verify(_wrapper.XamlText.Contains("<List"), "Failed - no list in document");
            KeyboardInput.TypeString("^{HOME}{END}{LEFT}{BACKSPACE}");
            QueueDelegate(CheckResult);
        }

        void CheckResult()
        {
            Verifier.Verify(_wrapper.XamlText.Contains("<List"), "Failed - no list in document");
            Verifier.Verify(!_wrapper.Text.Contains("a"), "Failed - character 'a' should be deleted from the listitem.");
            EndTest();
        }
    }

    /// <summary>
    /// Test +{TAB} for ListItems. We have a regression scenario here. 
    /// We should make this to be more complicated test for better coverage.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("522"), TestLastUpdatedOn("Jun 30, 2006")]
    public class RegressionTest_522 : CombinedTestCase
    {
        UIElementWrapper _wrapper;

        /// <summary>start to run the test.</summary>
        public override void RunTestCase()
        {
            RichTextBox richTextBox;
            richTextBox = new RichTextBox();
            richTextBox.AcceptsTab = true; 
            _wrapper = new UIElementWrapper(richTextBox);
            MainWindow.Content = richTextBox;
            richTextBox.Document.Blocks.Clear();
            richTextBox.Document.Blocks.Add(CreateNestedList());

            QueueDelegate(SetFocus);
        }

        List CreateNestedList()
        {
            List outerList, innerList;
            ListItem item; 
            innerList = new List(new ListItem(new Paragraph(new Run("c"))));
            innerList.ListItems.Add(new ListItem(new Paragraph(new Run("d"))));
            outerList = new List(new ListItem(new Paragraph(new Run("a"))));
            item = new ListItem(new Paragraph(new Run("b")));
            item.Blocks.Add(innerList);
            outerList.ListItems.Add(item);
            item.Blocks.Add(new Paragraph(new Run("e")));
            outerList.ListItems.Add(new ListItem(new Paragraph(new Run("f"))));
            innerList.MarkerStyle = TextMarkerStyle.Decimal;
            outerList.MarkerStyle = TextMarkerStyle.Decimal;
            return outerList;
        }

        void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(DoShiftTab);
        }

        void DoShiftTab()
        {
            KeyboardInput.TypeString("^{HOME}{DOWN 2}+{TAB}");
            QueueDelegate(CheckFirstShiftTab);
        }

        void CheckFirstShiftTab()
        {
            bool b = _wrapper.IsTextPointerInsideTextElement(_wrapper.SelectionInstance.Start, typeof(System.Windows.Documents.List));
            Verifier.Verify(b, "Failed - Caret should be inside a List");
            Verifier.Verify(_wrapper.ListLevel(_wrapper.SelectionInstance.Start) == 1, "Failed - After first shift-Tab, the list level should be one.");
            KeyboardInput.TypeString("+{TAB}");
            QueueDelegate(LastResultCheck);
        }

        void LastResultCheck()
        {
            bool b = _wrapper.IsTextPointerInsideTextElement(_wrapper.SelectionInstance.Start, typeof(System.Windows.Documents.List));
            Verifier.Verify(!b, "Failed - Caret should not inside a List");
            EndTest();
        }
    }

    /// <summary>
    /// Test +{TAB} for ListItems. We have a regression scenario here. 
    /// We should make this to be more complicated test for better coverage.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("450"), TestLastUpdatedOn("Aug 4, 2006")]
    public class RegressionTest_450 : CombinedTestCase
    {
        RichTextBox _richTextBox;
        private UIElementWrapper _wrapper;
        private List _level1List,_level2List,_level3List;
        private int _initialLevel;

        /// <summary>start to run the test.</summary>
        public override void RunTestCase()
        {            
            _richTextBox = new RichTextBox();
            _richTextBox.AcceptsTab = true;
            _wrapper = new UIElementWrapper(_richTextBox);
            MainWindow.Content = _richTextBox;
            _richTextBox.Document.Blocks.Clear();
            _richTextBox.Document.Blocks.Add(CreateNestedList());

            QueueDelegate(SetFocus);
        }

        private List CreateNestedList()
        {
            //<L1><LI><P>a</P><L2><LI><P>b</P><L3><LI><P>c</P></LI></L3></LI></L2><LI><P>d</P></LI></L1>
            _level1List = new List(new ListItem(new Paragraph(new Run("a"))));
            _level2List = new List(new ListItem(new Paragraph(new Run("b"))));
            _level3List = new List(new ListItem(new Paragraph(new Run("c"))));
            _level1List.MarkerStyle = _level2List.MarkerStyle = _level3List.MarkerStyle = TextMarkerStyle.Decimal;

            _level1List.ListItems.FirstListItem.Blocks.Add(_level2List);
            _level2List.ListItems.FirstListItem.Blocks.Add(_level3List);

            _level1List.ListItems.Add(new ListItem(new Paragraph(new Run("d"))));
            
            return _level1List;
        }

        private void SetFocus()
        {
            _wrapper.Element.Focus();
            QueueDelegate(DoSelection);
        }

        private void DoSelection()
        {
            TextPointer tp1, tp2;
            tp1 = _level2List.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
            tp2 = _level3List.ContentEnd.GetInsertionPosition(LogicalDirection.Backward);
            _richTextBox.Selection.Select(tp1, tp2);
            _initialLevel = _wrapper.ListLevel(_richTextBox.Selection.Start);
            Log("Initial level of List at selection start [" + _initialLevel.ToString() + "]");
            QueueDelegate(DoShiftTab);
        }

        private void DoShiftTab()
        {
            KeyboardInput.TypeString("+{TAB}");
            QueueDelegate(CheckShiftTab);
        }

        private void CheckShiftTab()
        {
            Verifier.Verify(_wrapper.ListLevel(_richTextBox.Selection.Start) == (_initialLevel - 1),
                "Verifying level of list after Shift+Tab. Level after Shift+Tab [" +
                _wrapper.ListLevel(_richTextBox.Selection.Start).ToString() + "]", true);
            
            EndTest();
        }        
    }

    /// <summary>
    /// Regression for Regression_Bug85 - Copy/Paste of listitems within a list had undesired behavior
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("85"), TestLastUpdatedOn("July 13, 2006")]
    public class RegressionTest_449 : CombinedTestCase
    {
        RichTextBox _richTextBox; 
        
        /// <summary>
        /// Start test 
        /// </summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            MainWindow.Content = _richTextBox;
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _richTextBox.Focus();
            QueueDelegate(PerformKeybaordAction);
        }

        void PerformKeybaordAction()
        {
            KeyboardInput.TypeString("a{ENTER}b+{LEFT 3}^+N^c{RIGHT}{ENTER}^v");
            QueueDelegate(VerifyResult);
        }

        void VerifyResult()
        {
            List list = _richTextBox.Document.Blocks.FirstBlock as List;
            Verifier.Verify(list.ListItems.Count == 4, "Failed - listItem expected[4], Actual[" + list.ListItems.Count + "]");
            EndTest();
        }
    }

    /// <summary>
    /// Regression for Regression_Bug86 - Second enter inside a ListItem with multiple paragraph should not bring the list item out of the list
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("25"), TestBugs("86, 523"), TestLastUpdatedOn("Aug 10, 2006")]
    public class RegressionTest_Regression_Bug86 : CombinedTestCase
    {
        RichTextBox _richTextBox;

        /// <summary>
        /// Start test 
        /// </summary>
        public override void RunTestCase()
        {
            _richTextBox = new RichTextBox();
            MainWindow.Content = _richTextBox;
            QueueDelegate(SetFocus);
        }

        void SetFocus()
        {
            _richTextBox.Focus();
            QueueDelegate(PerformKeybaordAction);
        }

        void PerformKeybaordAction()
        {
            KeyboardInput.TypeString("^+Na{ENTER}b{ENTER}{Backspace}c{ENTER}d{ENTER}{ENTER}");
            QueueDelegate(VerifyResult_Regression_Bug86);
        }

        void VerifyResult_Regression_Bug86()
        {
            //We need to verify that only two listItems, and the second listItem contains 3 paragraph.
            List list = _richTextBox.Document.Blocks.FirstBlock as List;
            Verifier.Verify(list.ListItems.Count == 2, "Failed - listItem expected[2], Actual[" + list.ListItems.Count + "]");
            ListItem item = list.ListItems.LastListItem as ListItem;
            Verifier.Verify(item.Blocks.Count == 3, "Failed - Paragraph count expected[3], Actual[" + item.Blocks.Count + "]");
            QueueDelegate(StartRegression_523);
            //EndTest();
        }

        void StartRegression_523()
        {
            _richTextBox.Document.Blocks.Clear();
            List list = new List();
            list.ListItems.Add(new ListItem(new Paragraph(new Run("Item1"))));
            list.ListItems.Add(new ListItem());
            list.ListItems.Add(new ListItem());
            list.ListItems.Add(new ListItem());
            list.ListItems.Add(new ListItem());
            _richTextBox.Document.Blocks.Add(list);
            _richTextBox.Focus();
            Verifier.Verify(list.ListItems.Count == 5, "Expected[5] Items, Actual" + list.ListItems.Count + "]");
            QueueDelegate(PerformDeleteOnEmptyItem);
        }

        void PerformDeleteOnEmptyItem()
        {
            KeyboardInput.TypeString("^{HOME}{DOWN}{DELETE}^{END}{UP}{BACKSPACE}");
            QueueDelegate(VerifyResult_523);
        }
        void VerifyResult_523()
        {
            List list = _richTextBox.Document.Blocks.FirstBlock as List; 
            Verifier.Verify(list.ListItems.Count == 3, "Expected[3] Items, Actual" + list.ListItems.Count + "]");
            EndTest();
        }
    }
}

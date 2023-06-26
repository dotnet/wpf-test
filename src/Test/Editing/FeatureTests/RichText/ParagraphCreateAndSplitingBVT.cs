// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Markup;
    using System.Windows.Media;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    
    #endregion Namespaces.

    /// <summary>This class contains BVT cases for Spliting/Creating Paragraph by user actions inside/outside the table</summary>
    [Test(0, "RichEditing", "ParagraphCreateAndSplitBVT", MethodParameters = "/TestCaseType=ParagraphCreateAndSplitBVT", Timeout = 360)]
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("696,695"), TestWorkItem("149, 148, 127")]
    public class ParagraphCreateAndSplitBVT : RichEditingBase
    {
        /// <summary>Use {Enter} to create/split paragraph</summary>
        [TestCase(LocalCaseStatus.Ready, "Split paragraph")]
        public void EnterToCreateAndSplit()
        {            
            EnterFunction("EnterToCreateAndSplit_Enter");
            Dimension[] dimensions;

            dimensions = new Dimension[] {
                new Dimension("SplitLocation", new object[] {"Beginning", "Middle", "End"}),
                new Dimension("SplitAction", new object [] {"x","xx", ""}),
                new Dimension("SplitCount", new object[] {1, 2}),
                new Dimension("Alignment", new object[] {System.Windows.HorizontalAlignment.Left, System.Windows.HorizontalAlignment.Center, System.Windows.HorizontalAlignment.Right}),
                new Dimension("FlowDirection", new object[] {System.Windows.FlowDirection.RightToLeft, System.Windows.FlowDirection.LeftToRight}),
                new Dimension("IsInsideTable", new object [] {true, false}),
            };
            _engine = CombinatorialEngine.FromDimensions(dimensions);
            QueueDelegate(EnterToCreateAndSplit_MainFlowBegins);
            EndFunction();
        }

        /// <summary>Retrivew the combination and perform initialization</summary>
        void EnterToCreateAndSplit_MainFlowBegins()
        {
            EnterFuction("EnterToCreateAndSplit_MainFlowBegins");
            _values = new Hashtable();
            if (!_engine.Next(_values))
            {
                QueueDelegate(EndTest);
                return;
            }
            MyLogger.Log("\r\n\r\n" + base.RepeatString("*", 30) +  "Start a new Combination" + base.RepeatString("*", 30));
            if((bool)_values["IsInsideTable"])
                SetInitValue("<Table BorderThickness=\"1,1,1,1\" BorderBrush=\"black\"><TableRowGroup><TableRow><TableCell><Paragraph></Paragraph></TableCell></TableRow></TableRowGroup></Table>");
            else
                SetInitValue("");
            
            //Set alignment
            ((Control)TextControlWraper.Element).HorizontalAlignment = (System.Windows.HorizontalAlignment)_values["Alignment"];
            //Set the flowDirection
            ((Control)TextControlWraper.Element).FlowDirection = (System.Windows.FlowDirection)_values["FlowDirection"];

            QueueDelegate(MouseClick);
           
            EndFunction();
        }

        /// <summary>
        /// Set Caret in the Table Cell
        /// </summary>
        void MouseClick()
        {
            if ((bool)_values["IsInsideTable"])
            {
                MouseInput.MouseClick(ElementUtils.GetScreenRelativePoint(TextControlWraper.Element, new Point(30, 30)));
            }
            QueueDelegate(EnterToCreateAndSplit_DoSplit); 
        }

        /// <summary>Calcuate the action string and expected value after action and then perfrom the keyboard action</summary>
        void EnterToCreateAndSplit_DoSplit()
        {
            EnterFuction("EnterToCreateAndSplit_DoSplit");
            string SplitLocation = (string)_values["SplitLocation"];
            string SplitAction = (string)_values["SplitAction"];
            int SplitCount = (int)_values["SplitCount"];
            bool table= (bool) _values["IsInsideTable"]; 

            MyLogger.Log(CurrentFunction + " - SplitLocation[" + SplitLocation + "]");
            MyLogger.Log(CurrentFunction + " - SplitAction[" + SplitAction + "]");
            MyLogger.Log(CurrentFunction + " - SplitCount[" + SplitCount + "]");
            MyLogger.Log(CurrentFunction + " - Alignment[" + _values["Alignment"].ToString() + "]");
            MyLogger.Log(CurrentFunction + " - FlowDirection[" + _values["FlowDirection"].ToString() + "]");
            MyLogger.Log(CurrentFunction + " - IsInsideTable[" + _values["IsInsideTable"].ToString() + "]");

            string TextString = null;
            string KeyBoardStr = null ;

            for (int i = 1; i <= SplitCount; i++)
            {
                KeyBoardStr = KeyBoardStr + "{ENTER}" + SplitAction;

                TextString = TextString + "\r\n" + SplitAction; 
            }

            if (SplitLocation == "Beginning")
            {
                KeyBoardStr = SplitAction + "{Home}" + KeyBoardStr;
                TextString =TextString + SplitAction; 
            }
            else if (SplitLocation == "Middle")
            {
                //Move caret the the middle. 
                if (SplitAction.Length > 0)
                {
                    if ((System.Windows.FlowDirection)_values["FlowDirection"] == FlowDirection.LeftToRight)
                    {
                        KeyBoardStr = SplitAction + "{Home}{Right}" + KeyBoardStr;
                    }
                    else
                    {
                        KeyBoardStr = SplitAction + "{Home}{Left}" + KeyBoardStr;
                    }
                }
                else
                {
                    KeyBoardStr = SplitAction + "{Home}" + KeyBoardStr;
                }

                if(SplitAction =="x")
                {
                    TextString = SplitAction + TextString;
                }
                else if(SplitAction == "xx")
                {
                    TextString = "x" + TextString + "x";
                }           
            }
            else
            {
                KeyBoardStr = SplitAction + "{End}" + KeyBoardStr;
                TextString = SplitAction + TextString;
            }

            TextString = TextString + "\r\n";
            int tempint = (int)_values["SplitCount"] + 1;
            _values.Remove("SplitCount");
            _values.Add("SplitCount", tempint);
            _values.Add("TextString", TextString);
            KeyboardInput.TypeString(KeyBoardStr);
           
            QueueDelegate(EnterToCreateAndSplit_OnToNextCase);
            EndFunction();
        }

        /// <summary>This method evaluates the result and starts the next conbination </summary>
        void EnterToCreateAndSplit_OnToNextCase()
        {
            EnterFuction("EnterToCreateAndSplit_OnToNextCase");
            Sleep(100);
            CheckRichedEditingResults((string)_values["TextString"], "", 0, (int)_values["SplitCount"], TextControlWraper);
           
            //if it is in table, we will check to see if the paragraphs are in the table.
            if ((bool)_values["IsInsideTable"] )
            {
                string trimmedXaml = TextControlWraper.XamlText;
                trimmedXaml = trimmedXaml.Substring(trimmedXaml.IndexOf("<TableCell>", 0) + 11 );
                trimmedXaml = trimmedXaml.Substring(0, trimmedXaml.IndexOf("</TableCell>"));
                if (Occurency(trimmedXaml, "<Paragraph") != (int)_values["SplitCount"])
                {
                    MyLogger.Log(CurrentFunction + " - Failed: Paragraphs in TableCell is wrong, Please check the xaml: [" + TextControlWraper.XamlText + "]");
                    pass = false;
                }
            }
            if (!pass)
            {
                QueueDelegate(EndTest);
            }
            else
                QueueDelegate(EnterToCreateAndSplit_MainFlowBegins);
            EndFunction();
        }

        /// <summary>OneActionParagraphEiting</summary>
        [TestCase(LocalCaseStatus.UnderDevelopment, "EnterOverSelection")]
        public void OtherWaysToSplitAndMerge()
        {
            EnterFuction("OtherWaysToSplitAndMerge");
            TestDataArayList = new ArrayList();
            //Enter over Selection
            TestDataArayList.Add(new RichEditingData("Enter over Selection", "", "abc+{Left 2}{enter}d", "a\r\nd\r\n", "", 0, 2, true, 0));
            TestDataArayList.Add(new RichEditingData("Enter over Selection", "", "abc^a{enter}d", "\r\nd\r\n", "", 0, 2, true, 0));
            TestDataArayList.Add(new RichEditingData("Enter over Selection", "", "ab{Enter}cd+{Left 3}{enter}", "ab\r\n\r\n", "", 0, 2, true, 0));
            TestDataArayList.Add(new RichEditingData("Enter over Selection", "", "ab{Enter}cd{left}+{Left 3}{Enter}", "a\r\nd\r\n", "", 0, 2, true, 0));
      
            SetInitValue("");
            QueueDelegate(RichEditingDataKeyBoardExecution);
            EndFunction();
        }
        /// <summary>Engine generating combinations to test.</summary>
        private CombinatorialEngine _engine;
        /// <summary>Hashtable to hold the values for each conbination.</summary>
        private Hashtable _values;
    }
}

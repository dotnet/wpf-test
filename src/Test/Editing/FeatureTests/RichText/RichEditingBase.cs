// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 21 $ $Source: //depot/private/WCP_dev_platform/Windowstest/client/wcptests/uis/Text/BVT/ExeTarget/EntryPoint.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Windows;
    using System.Collections;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;

    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// This class contains code for testing the PasswordBox
    /// and RichTextBox controls.
    /// </summary>
    public class RichEditingBase : CommonTestCase
    {
        /// <summary> </summary>
        protected Test.Uis.Wrappers.UIElementWrapper TextControlWraper;
        
        /// <summary>Remember the text content in a text control </summary>
        protected string Expected_TextContent = null;
        
        /// <summary>Remember the Selected text</summary>
        protected string Expected_SelectedText;
        
        /// <summary>Remember number of paragrahs in the selection </summary>
        protected int Expected_SelectedPargraphs = 0;
        
        /// <summary>Remeber the Wraper for the TextControl</summary>
        protected UIElementWrapper Expected_workWraper;

        /// <summary>Hold all the data objects for each test.</summary>
        protected ArrayList TestDataArayList;
        
        /// <summary></summary>
        protected bool IsEditingInsideTable = false;

        /// <summary>Initialize the test </summary>
        public override void Init()
        {
            EnterFuction("Init");
            RichTextBox richTextBox = new RichTextBox();
            MainWindow.Content = richTextBox;
            richTextBox.SetValue(TextElement.FontFamilyProperty, new FontFamily("Palatino Linotype"));
            richTextBox.SetValue(TextElement.FontSizeProperty, 30.0);
            richTextBox.Background = Brushes.Wheat;
	    MainWindow.UpdateLayout();
            TextControlWraper = new Test.Uis.Wrappers.UIElementWrapper(richTextBox);
            QueueDelegate(SetFocus);
            EndFunction();
        }

        private void SetFocus()
        {
            EnterFuction("SetFocus");
            RichTextBox rb = TextControlWraper.Element as RichTextBox;
            if(null != rb)
                rb.Focus();
            EndFunction();
        }

        /// <summary>
        /// SetInitValue for each test case.
        /// </summary>
        protected virtual void SetInitValue(string xamlstr)
        {
            if (!xamlstr.StartsWith("<"))
            {
                xamlstr = "<Paragraph>" + xamlstr + "</Paragraph>";
            }

            Init();            
            TextControlWraper.Clear();
            TextRange MyRange = new TextRange(TextControlWraper.Start,TextControlWraper.End);
            string xaml = XamlUtils.TextRange_GetXml(MyRange);                        
            xaml = xaml.Replace("<Paragraph><Run></Run></Paragraph>", xamlstr);
            XamlUtils.TextRange_SetXml(MyRange, xaml);            
        }

        /// <summary>
        /// Set the expected values
        /// </summary>
        /// <param name="Text"></param>
        /// <param name="SelectedText"></param>
        /// <param name="Pargraphs"></param>
        /// <param name="wraper"></param>
        protected void SetExpectedValues(string Text, string SelectedText, int Pargraphs, Test.Uis.Wrappers.UIElementWrapper wraper)
        {
            Expected_TextContent = Text;
            Expected_SelectedText = SelectedText;
            Expected_SelectedPargraphs = Pargraphs;
            Expected_workWraper = wraper;

        }
        
        /// <summary>
        /// Check the Text Control against the Expected values
        /// </summary>
        protected void CheckRichedEditingResults()
        {
            CheckRichedEditingResults(Expected_TextContent, Expected_SelectedText, Expected_SelectedPargraphs, Expected_workWraper);
        }
        /// <summary>
        /// CheckRichedEditingResults
        /// </summary>
        /// <param name="AllText"></param>
        /// <param name="selectedText"></param>
        /// <param name="selectedParagraphCount"></param>
        /// <param name="wrapper"></param>
        public void CheckRichedEditingResults(string AllText, string selectedText, int selectedParagraphCount, UIElementWrapper wrapper)
        {

            TextRange tRange = new TextRange(wrapper.Start, wrapper.End);
            CheckRichedEditingResults(AllText, selectedText, selectedParagraphCount, Occurency(XamlUtils.TextRange_GetXml(tRange), "<Paragraph"), wrapper);
        }
        /// <summary>
        /// CheckRichedEditingResults
        /// </summary>
        /// <param name="AllText"></param>
        /// <param name="selectedText"></param>
        /// <param name="selectedParagraphCount"></param>
        /// <param name="AllParagraphCount"></param>
        /// <param name="wrapper"></param>
        protected void CheckRichedEditingResults(string AllText, string selectedText, int selectedParagraphCount, int AllParagraphCount, UIElementWrapper wrapper)
        {
            string xaml;                // XAML serialization of all content.
            string realAllText;     // Plain text content.
            string realSelectedText;    // Plain text selection.
            int realSelectedParagraphCount;     // Number of paragraphs in Selecgted content.
            int realAllParagraphCount;      //All paragarph count.

            xaml = XamlUtils.TextRange_GetXml(wrapper.SelectionInstance);
            realAllText = wrapper.Text;
            realSelectedText = wrapper.SelectionInstance.Text;
            realSelectedParagraphCount = Occurency(xaml, "<Paragraph");
            TextRange tRange = new TextRange(wrapper.Start, wrapper.End);
            realAllParagraphCount = Occurency(XamlUtils.TextRange_GetXml(tRange), "<Paragraph") ;


            Sleep();

            if (AllText == null)
            {
                throw new ArgumentNullException("textContent");
            }
            if (selectedText == null)
            {
                throw new ArgumentNullException("selectedText");
            }
            if (wrapper == null)
            {
                throw new ArgumentNullException("wrapper");
            }

            //check text content
            if (realAllText != AllText)
            {
                Logger.Current.Log(CurrentFunction + " - Failed: text content does not match.");
                Logger.Current.Log("Expected text content: [" + AllText + "]");
                Logger.Current.Log("Current text content: [" + realAllText + "]");
                pass = false;
            }

            //check selection
            if (selectedText != realSelectedText)
            {
                Logger.Current.Log(CurrentFunction + " - Failed: selected content does not match.");
                Logger.Current.Log("Expected selection content: [" + selectedText + "]");
                Logger.Current.Log("Current selection content: [" + realSelectedText +"]");
                pass = false;
            }

            //check the selected paragraph
            if (realSelectedParagraphCount != selectedParagraphCount)
            {
                MyLogger.Log(CurrentFunction + " - Failed: we expected " + selectedParagraphCount +
                    " pair of Paragraph tag(s), but found " + realSelectedParagraphCount +
                    ". Please count them in the xaml: " + xaml);
                pass = false;
            }
            //check all the paragraph
            if (realAllParagraphCount != AllParagraphCount)
            {
                MyLogger.Log(CurrentFunction + " - Failed: we expected " + AllParagraphCount +
                    " pair of Paragraph tag(s), but found " + realAllParagraphCount +
                    ". Please count them in the xaml: " +XamlUtils.TextRange_GetXml(tRange));
                pass = false;
            }
            if (IsEditingInsideTable)
            {
                string trimmedXaml =XamlUtils.TextRange_GetXml(tRange);
                trimmedXaml = trimmedXaml.Substring(trimmedXaml.IndexOf("<TableCell>", 0) + 11);
                trimmedXaml = trimmedXaml.Substring(0, trimmedXaml.IndexOf("</TableCell>"));
                if (Occurency(trimmedXaml, "<Paragraph") != AllParagraphCount)
                {
                    MyLogger.Log(CurrentFunction + " - Failed: Paragraphs in TableCell is wrong, Please check the xaml: [" + TextControlWraper.XamlText + "]");
                    pass = false;
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Wraper"></param>
        /// <param name="Index"></param>
        /// <param name="location"></param>
        /// <param name="Clicks"></param>
        protected void MouseClicksOnACharacter(Test.Uis.Wrappers.UIElementWrapper Wraper, int Index, CharRectLocations location, int Clicks)
        {
            Point p=FindLocation(Wraper, Index, location);
            for (int i = 0; i < Clicks; i++)
            {
                MouseInput.MouseClick((int)p.X, (int)p.Y);
            }
        }
        /// <summary>
        /// FindLocation
        /// </summary>
        /// <param name="Wraper"></param>
        /// <param name="Index"></param>
        /// <param name="location"></param>
        /// <returns></returns>
        protected Point FindLocation(Test.Uis.Wrappers.UIElementWrapper Wraper, int Index, CharRectLocations location)
        {
            int ClickX = 0, ClickY = 0;
            TextPointer start = TextControlWraper.Start;
            Rect rec = TextControlWraper.GetGlobalCharacterRect(start, Index);
            switch (location)
            {
                case CharRectLocations.Left:
                    ClickX = ((int)(rec.Left + rec.Right) / 2) - 1;
                    ClickY = (int)((rec.Top + rec.Bottom) / 2);
                    break;
                case CharRectLocations.Right:
                    ClickX = ((int)(rec.Left + rec.Right) / 2) + 1;
                    ClickY = (int)((rec.Top + rec.Bottom) / 2);
                    break;
                case CharRectLocations.Top:
                    ClickX = ((int)(rec.Left + rec.Right) / 2);
                    ClickY = ((int)((rec.Top + rec.Bottom) / 2)) - 1;
                    break;
                case CharRectLocations.Botton:
                    ClickX = ((int)(rec.Left + rec.Right) / 2);
                    ClickY = ((int)((rec.Top + rec.Bottom) / 2)) + 1;
                    break;
                case CharRectLocations.LeftEdge:
                    ClickX = (int)rec.Left;
                    ClickY = (int)((rec.Top + rec.Bottom) / 2);
                    break;
                case CharRectLocations.RightEdge:
                    ClickX = (int)rec.Right;
                    ClickY = (int)((rec.Top + rec.Bottom) / 2);
                    break;
                case CharRectLocations.TopEdge:
                    ClickX = ((int)(rec.Left + rec.Right) / 2);
                    ClickY = (int)rec.Top;
                    break;
                case CharRectLocations.BottonEdge:
                    ClickX = ((int)(rec.Left + rec.Right) / 2);
                    ClickY = (int)rec.Bottom;
                    break;
                //center
                default:
                    ClickX = (int)(rec.Left + rec.Right) / 2;
                    ClickY = (int)((rec.Top + rec.Bottom) / 2);
                    break;
            }
            return new Point(ClickX, ClickY);
        }
        /// <summary>
        /// Start to run the cases.
        /// </summary>
        protected void StartCasesRunning()
        {
            int pri = ConfigurationSettings.Current.GetArgumentAsInt("Priority");
            int index = 0;
            while (index < TestDataArayList.Count)
            {
                if (((RichEditingData)TestDataArayList[index]).Priority != pri)
                {
                    TestDataArayList.RemoveAt(index);
                }
                else
                    index++;
            }
            if (TestDataArayList.Count > 0)
            {
                SetInitValue(((RichEditingData)TestDataArayList[0]).InitialXaml);
                QueueDelegate(RichEditingDataKeyBoardExecution);
            }
            else
            {
                QueueDelegate(EndTest);
            }

        }
        /// <summary>Find the insertion pointer from index</summary>
        /// <param name="wrapper"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        protected TextPointer PointerFromIndex(UIElementWrapper wrapper, int index)
        {
            TextPointer dStart = wrapper.Start;
            if (dStart.IsAtInsertionPosition)
                index--;
            while (index > 0)
            {
                dStart = dStart.GetNextInsertionPosition(LogicalDirection.Forward);
                index--;
            }
            return dStart;
        }

        #region those method loop execute cases that constructed using RichEditingData
        /// <summary>Perfrom keyboard actions</summary>
        protected void RichEditingDataKeyBoardExecution()
        {
            EnterFuction("RichEditingDataKeyBoardExecution");
            if (TestDataArayList.Count != 0)
            {                
                RichEditingData rd = ((RichEditingData)TestDataArayList[0]);
                MyLogger.Log("\r\n\r\n" + base.RepeatString("*", 30) + "Start a new Combination - " + rd.CaseDescription +  base.RepeatString("*", 30));
                MyLogger.Log(CurrentFunction + " - InitialXml: [" + rd.InitialXaml + "]");
                MyLogger.Log(CurrentFunction + " - Selected Text: [" + rd.FinalSelectedText + "]");
                MyLogger.Log(CurrentFunction + " - Selected Paragraph Count: [" + rd.FinalSelectedParagraphCount + "]");
                MyLogger.Log(CurrentFunction + " - All Xml Text: [" + rd.FinalXmlText + "]");
                MyLogger.Log(CurrentFunction + " - All Paragraph Count: [" + rd.FinalAllParagraphCount + "]");
                MyLogger.Log(CurrentFunction + " - We reset init value: [" + rd.Reset + "]");
                MyLogger.Log(CurrentFunction + " - Case Priority: [" + rd.Priority + "]");

                KeyboardInput.TypeString(((RichEditingData)TestDataArayList[0]).KeyboardInputString);
                QueueDelegate(RichEditingDataKeyBoardExecution_CheckResult);
            }
			else
				QueueDelegate(EndTest);
			EndFunction();
		}
        /// <summary>Check the result after keyboard actions.</summary>
        void RichEditingDataKeyBoardExecution_CheckResult()
        {
            EnterFuction("RichEditingDataKeyBoardExecution_CheckResult");
            RichEditingData rd = ((RichEditingData)TestDataArayList[0]);
            CheckRichedEditingResults(rd.FinalXmlText, rd.FinalSelectedText, rd.FinalSelectedParagraphCount, rd.FinalAllParagraphCount, TextControlWraper);
            TestDataArayList.RemoveAt(0);
            if (rd.Reset && TestDataArayList.Count > 0)
            {
                SetInitValue(((RichEditingData)TestDataArayList[0]).InitialXaml);
            }

            QueueDelegate(RichEditingDataKeyBoardExecution);
            EndFunction();
        }
        #endregion
    }
    #region interesting locations of each character
    /// <summary>CharRectLocations</summary>
    public enum CharRectLocations
    {
        /// <summary></summary>
        Left,
        /// <summary></summary>
        Right,
        /// <summary></summary>
        Top,
        /// <summary></summary>
        Botton,
        /// <summary></summary>
        Center,
        /// <summary></summary>
        LeftEdge,
        /// <summary></summary>
        RightEdge,
        /// <summary></summary>
        TopEdge,
        /// <summary></summary>
        BottonEdge,
    }
    #endregion

    # region struct for RichEditing input and result data
    /// <summary>
    /// RichEditingData
    /// </summary>
    public struct RichEditingData
    {
        /// <summary>Initial Xaml set in the RichTextBox</summary>
        public string InitialXaml;
        /// <summary>Xaml Text for the RichTextBox</summary>
        public string FinalXmlText;
        /// <summary>Selected Text for the RichTextBox</summary>
        public string FinalSelectedText;
        /// <summary>Number of Paragraphs in the selection</summary>
        public int FinalSelectedParagraphCount;
        /// <summary>Number of Paragraph in RichTextBox</summary>
        public int FinalAllParagraphCount;
        /// <summary>What to typing into RichTextBox</summary>
        public String KeyboardInputString;
        /// <summary>Description for each conbination</summary>
        public string CaseDescription; 
        /// <summary>Will the RichTextBox be reset</summary>
        public bool Reset;
        /// <summary>Case priority</summary>
        public int Priority;

	/// <summary>
	/// RichEditingData
	/// </summary>
	/// <param name="caseDescription"></param>
	/// <param name="initialXaml"></param>
	/// <param name="keybarodInputString"></param>
	/// <param name="xmlText"></param>
	/// <param name="selectedText"></param>
	/// <param name="selectedParagraphCount"></param>
	/// <param name="allParagraphCount"></param>
	/// <param name="reset"></param>
	/// <param name="priority"></param>
        public RichEditingData(string caseDescription, string initialXaml, string keybarodInputString, string xmlText, string selectedText, int selectedParagraphCount, int allParagraphCount, bool reset, int priority)
        {
            CaseDescription = caseDescription; 
            InitialXaml = initialXaml; 
            FinalXmlText = xmlText;
            FinalSelectedText = selectedText;
            FinalSelectedParagraphCount = selectedParagraphCount;
            FinalAllParagraphCount = allParagraphCount;
            KeyboardInputString = keybarodInputString;
            Reset = reset;
            Priority = priority; 
        }
    }
    #endregion 
}

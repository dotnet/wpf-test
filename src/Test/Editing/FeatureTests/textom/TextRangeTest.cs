// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Unit testing for public API of the TextRange class.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/textom/TextRangeTest.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Drawing;    
    using System.IO;
    using System.Threading;

    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;
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

    /// <summary>
    /// this class perform the inpu ttest the public API.
    /// </summary>
    [Test(0, "TextOM", "TextRangeAPITest", MethodParameters = "/TestCaseType=TextRangeAPITest")]
    [TestOwner("Microsoft"), TestBugs("646"), TestTactics("392"), TestWorkItem("48"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextRangeAPITest : RichEditingBase
    {
        int _rangeChangedCounter; 

        #region TextRange-constructor, Start, End, IsEmpty
        /// <summary>Test the Constructor of the TextRange</summary>
        [TestCase(LocalCaseStatus.Ready, "TextRange constructor test")]
        public void TextRangeConstructorTest()
        {
            SetInitValue("abecd");
            QueueDelegate(TextRangeConstructorTestInteral);
        }

        /// <summary>Start to test each scenarios </summary>
        void TextRangeConstructorTestInteral()
        {
            //Null values 
            TextRangeConstructorNullTest();

            //No Null values
            TextRangeConstructorNonNullTest();

            //TextPointer from different TextContainers
            TextRangeFromDifferentTextContainers();
            EndTest();
        }

        /// <summary>Test that TextRange can't be created from two TextPointers in different Containers</summary>
        void TextRangeFromDifferentTextContainers()
        {
            RichTextBox box1 = new RichTextBox();
            string errorMessage = "Should not be able create range from different containers!";

            try
            {
                new TextRange(TextControlWraper.Start, box1.Selection.End);
                throw new Exception(errorMessage);
            }
            catch (Exception e)
            {
                if(e.Message == errorMessage)
                {
                    Log(errorMessage);
                    pass = false; 
                }
            }
        }

        /// <summary> Create TextRanges from TextPointers in a RichTextBox.</summary>
        void TextRangeConstructorNonNullTest()
        {
            TextRange range;
            ArrayList textPointerList = GetAllTextPointers() ;
            
            for (int i = 0; i < textPointerList.Count; i++)
            {
                for(int j = 0; j<textPointerList.Count; j++)
                {
                    range = new TextRange((TextPointer)textPointerList[i], (TextPointer)textPointerList[j]);
                    Verifier.Verify(range != null, "TextRange should not be null!");

                    //Verify the Start and End Property. Make sure they are at insertion position.
                    Verifier.Verify(range.Start.IsAtInsertionPosition && range.End.IsAtInsertionPosition, "Failed. Either Range.Start or Range.End is not an Insertion position!");

                    //Verify the IsEmptyProperty.
                    if (range.Start.GetOffsetToPosition(range.End) == 0)
                    {
                        Verifier.Verify(range.IsEmpty, "Failed. When offset between Start and End is 0, IsEmpty should return true!");
                    }
                    else
                    {
                        Verifier.Verify(!range.IsEmpty, "Failed. When offset between Start and End is not 0, IsEmpty should return false!");
                    }
                }
            }
        }

        /// <summary>Test that Null value(s) for the TextRange constructor </summary>
        void TextRangeConstructorNullTest()
        {
            object [] parameters  = new object[2];
            int i, j; 
            parameters[0] = null;
            parameters[1] = TextControlWraper.Start; 
            for (i = 0; i < 2; i++)
            {
                for ( j = 0; j < 2; j++)
                {
                    try
                    {
                        new TextRange((TextPointer)parameters[i], (TextPointer)parameters[j]);
                        throw new Exception("No ArgumentNullException is thrown");
                    }
                    catch (Exception e)
                    {
                        if (!(e is ArgumentNullException) && !(parameters[i] != null && parameters[j] != null))
                        {
                            MyLogger.Log(e.Message);
                            pass = false;
                        }
                    }
                }
            }
        }

        /// <summary>Find all the TextPointers in a text tree </summary>
        /// <returns>an ArrayList</returns>
        ArrayList GetAllTextPointers()
        {
            TextPointer pointer = TextControlWraper.Start;
            ArrayList textPointerList = new ArrayList();
            do
            {
                textPointerList.Add(pointer);
                pointer = pointer.GetPositionAtOffset(1, LogicalDirection.Forward);

            } while (pointer.GetOffsetToPosition(pointer.DocumentEnd) > 0);

            return textPointerList;
        }

        #endregion 

        #region TextRange-Xml
        /// <summary>Test the XML property</summary>
        [TestCase(LocalCaseStatus.Ready, "TextRange.Xml test")]
        public void TestRangeXmlTest()
        {
            //Set a initial value to RichTextBox
            SetInitValue("abc");
            
            //Get_Xml
            QueueDelegate(TestGet_Xml);
        }

        /// <summary>Re</summary>
        void TestGet_Xml()
        {
            EnterFunction("TestGet_Xml");
            TextPointer p1; 
            TextRange range;
            string xml; 
            
            //Range span whole Document
            range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            xml = XamlUtils.TextRange_GetXml(range);
            Verifier.Verify(Occurency(xml, "<Paragraph><Run>abc</Run></Paragraph></Section>") == 1, "Failed: xml of the range should contain[<Paragraph><Run>abc</Run></Paragraph></Section>]. Please check the xml[" + xml + "]");
           
            //reverse the start, end TextPointer
            range = new TextRange(TextControlWraper.End, TextControlWraper.Start);
            xml =XamlUtils.TextRange_GetXml(range);
            Verifier.Verify(Occurency(xml, "<Paragraph><Run>abc</Run></Paragraph></Section>") == 1, "Failed: xml of the range should contain[<Paragraph><Run>abc</Run></Paragraph></Section>]. Please check the xml[" + xml + "]");
          
            //Empty Range from DocumentStart to DocumentStart
            range = new TextRange(TextControlWraper.Start, TextControlWraper.Start);
            xml = XamlUtils.TextRange_GetXml(range);
            Verifier.Verify(Occurency(xml, "<Run></Run></Span>")==1, "Failed: xml of the range should contain[<Run></Run></Span>] Please check the xml[" + xml + "]");
           
            //Empty range when TextPointer is a insertion point.
            p1 = TextControlWraper.Start.GetInsertionPosition(LogicalDirection.Forward);
            range = new TextRange(p1, p1);
            xml = XamlUtils.TextRange_GetXml(range);
            Verifier.Verify(Occurency(xml, "<Run></Run></Span>") == 1, "Failed: xml of the range should contain[<Run></Run></Span>] Please check the xml[" + xml + "]");
            
            //Set_Xml
            TestSet_Xml();
            EndFunction();
        }

        /// <summary>
        /// test to set xml to a TextRange
        /// we are not going to test the xaml schema.
        /// Invalid value such as null, "", "abc", will be caught by parser. 
        /// It seems that there is not way for Xml property to check all the invalid xaml string. Software developler should be very careful when use this API.
        /// </summary>
        void TestSet_Xml()
        {
            EnterFunction("TestSet_Xml");
            TextRange range;
            ArrayList validxmlList = new ArrayList();
            ArrayList invalidxmlList = new ArrayList();
            

            //valid xaml
            validxmlList.Add("<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">Test</Span>");
            validxmlList.Add("<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\">Test</Span>");
            validxmlList.Add("<Span xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Run>Test</Run></Span>");
            validxmlList.Add("<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph>Test</Paragraph></Section>");
            validxmlList.Add("<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run>Test</Run></Paragraph></Section>");

            //Invalid xaml
            invalidxmlList.Add(null);
            invalidxmlList.Add(string.Empty);
            invalidxmlList.Add("junck");
          
            //valid xaml
            range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            for (int i = 0; i < validxmlList.Count; i++)
            {
                XamlUtils.TextRange_SetXml(range, (string)validxmlList[i]);

                Verifier.Verify(TextControlWraper.Text == "Test\r\n", "Failed for set Xml! Expected text[Test\r\n], Actual[" + TextControlWraper.Text +"]");
                ((RichTextBox)TextControlWraper.Element).Undo();
                Verifier.Verify(TextControlWraper.Text == "abc\r\n", "Failed for Undo! Expected text[abc\r\n], Actual[" + TextControlWraper.Text + "]");
            }

            //invalid xaml
            for (int j= 0; j < invalidxmlList.Count; j++)
            {
                try
                {
                    XamlUtils.TextRange_SetXml(range,(string)invalidxmlList[j]);
                    throw new Exception("No Exception is caught!");
                }
                catch (Exception e)
                {
                    if (e.Message == "No Exception is caught!")
                    {

                        Log(e.Message);
                        pass = false;
                    }
                }
            }

            //end of this test.
            EndTest();
            EndFunction();
        }
        #endregion 

        #region TextRange-Text
        /// <summary>
        /// Set and get Text from TextRange.
        /// New line convertion: \r - \r, \r\n -> \r\n, \n -> \r\n
        /// null value is not accepted, the ArgumentNull exception is thrown from TextBoxBase. Do we consider this is fine?
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "TextRange.Text test")]
        public void TextRangeTextTest()
        {
            SetInitValue("abcd");
            QueueDelegate(InternalTextRangeTextTest);
        }
        void InternalTextRangeTextTest()
        {
            string[] testString ={ "", "x", "x\r\n", "x\r", "x\n", "x\n\n\r", "x<>&lt" };
            string[] expectedString ={ "", "x\r\n", "x\r\n", "x\r\r\n", "x\r\n","x\r\n\r\n\r\r\n", "x<>&lt\r\n" };
            TextRange range; 

            //Null value
            range = new TextRange(TextControlWraper.Start, TextControlWraper.End);
            try
            {
                range.Text = null;
                throw new Exception("Null value is not handled!");
            }
            catch (Exception e)
            {
                if (!(e is ArgumentNullException))
                {
                    Log(e.Message);
                    pass = false; 
                }
            }

            //Non-null values.
            for (int i = 0; i < testString.Length; i++)
            {
                range.Text = testString[i];
                TextRange tempRange = new TextRange(TextControlWraper.Start, TextControlWraper.End);
                Verifier.Verify(tempRange.Text == expectedString[i],
                    "Failed. TextRange.Text does not match the specified value! expected[" + expectedString[i] + "], returned[" + tempRange.Text + "]");
                
                //Make sure that special characters are serialized and parsed fine syntextacialy. 
               XamlUtils.TextRange_SetXml( tempRange, XamlUtils.TextRange_GetXml(tempRange));
            }
            EndTest();
        }
        #endregion

        #region TextRange Changed events Test.
        /// <summary>
        /// Test Changed event of TextRange
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "TextRange.Text test")]
        public void TextRangeChangedTest()
        {
            SetInitValue("abcd");
            QueueDelegate(InternalTextRangeChangedTest);
        }

        /// <summary>Test the Changed event </summary>
        private void InternalTextRangeChangedTest()
        {
            TextRange range;
            int expectedCounter;

            ArrayList textPointerList = GetAllTextPointers() ;
           
            for (int i = 0; i < textPointerList.Count; i++)
            {
                for (int j = 0; j < textPointerList.Count; j++)
                {
                    range = new TextRange((TextPointer)textPointerList[i], (TextPointer)textPointerList[j]);
                    range.Changed += new EventHandler(range_Changed);
                    expectedCounter = _rangeChangedCounter;

                    //Set Text to trigger the event
                    range.Text = "a";
                    expectedCounter++;
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                        "Failed: Expected changed event fire when set text! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");

                    //Set empty text value
                    range.Text = "";
                    expectedCounter++;
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                      "Failed: Expected changed event fire when set empty text to non-empty range! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");

                    //Set empty Text value again should not triggle the event to be fired.
                    range.Text = "";
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                     "Failed: dont Expected changed event fire! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");

                    //Set Non-TextElement xml
                    XamlUtils.TextRange_SetXml(range, "<Button xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"></Button>");
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                      "Failed: dont expected changed event fire when set non-TextElement xaml! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");

                    //Set xml with TextElement
                    XamlUtils.TextRange_SetXml(range, "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run>adsf</Run></Paragraph></Section>");
                    expectedCounter++;
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                      "Failed: dont expected changed event fire when set non-TextElement xaml! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");

                    range.Changed -= new EventHandler(range_Changed);
                    range.Text = "a";
                    range.Text = "";
                    XamlUtils.TextRange_SetXml(range, "<Section xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"><Paragraph><Run>adsf</Run></Paragraph></Section>");
                    Verifier.Verify(expectedCounter == _rangeChangedCounter,
                      "Failed: dont expected changed event fire when hanldler is removed! expected counter[" + expectedCounter + "], ActualCounter[" + _rangeChangedCounter + "]");
                }
            }

            //end test.
            QueueDelegate(EndTest);
        }

        /// <summary>
        /// TextRange.Chnaged event handler.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void range_Changed(object sender, EventArgs e)
        {
            _rangeChangedCounter++; 
        }
        #endregion

        #region TextRange Select() method test
        /// <summary>
        /// Test select method of TextRange
        /// </summary>
        [TestCase(LocalCaseStatus.Ready, "TextRange.Select() test")]
        public void TextRangeSelectTest()
        {
            SetInitValue("ab\r\ncd");
            QueueDelegate(InternalTextRangeSelectTest);
        }

        /// <summary>Implementation of testing Select method</summary>
        void InternalTextRangeSelectTest()
        {
            TextRangeSelectInvalidParameterTest();
            TextRangeSelectvalidParameterTest();
            EndTest();
        }

        /// <summary> Test the valid parameter of TextRange.Select(pointer1, pointer2)</summary>
        void TextRangeSelectvalidParameterTest()
        {
            ArrayList textPointerList = GetAllTextPointers();
            TextRange range = new TextRange((TextPointer)textPointerList[0], (TextPointer)textPointerList[0]);
            string text;


            for(int i=0; i<textPointerList.Count; i++)
            {
                for(int j=0; j<textPointerList.Count; j++)
                {
                    range.Select((TextPointer)textPointerList[i], (TextPointer)textPointerList[j]);
                    
                    //Start and End Pointers must be Insertion point
                    Verifier.Verify(range.Start.IsAtInsertionPosition, "Failed: TextRange.Start is not at insertion position after Select is called!");
                    Verifier.Verify(range.Start.IsAtInsertionPosition, "Failed: TextRange.End is not at insertion position after Select is called!");

                    //Verify the selected text
                    text = TextBetweenPointers((TextPointer)textPointerList[i], (TextPointer)textPointerList[j]);
                    Verifier.Verify(text == range.Text, "Failed TextRange.Text does not match! Expected[" + text + "]. Actual[" + range.Text + "]");
                }
            }
        }

        /// <summary>Find the text between Two Pointers</summary>
        /// <param name="p1"></param>
        /// <param name="p2"></param>
        /// <returns></returns>
        string TextBetweenPointers(TextPointer p1, TextPointer p2)
        {
            string result = "";
            string tempstr = "";
            TextPointer start;
            TextPointer end;
            TextPointer tempPointer; 
            Paragraph paragraph;
            int leadingIndex = -1; 
            int offset; 

            if (p1.GetOffsetToPosition(p2) >= 0)
            {
                start = p1.GetPositionAtOffset(0);
                end = p2.GetPositionAtOffset(0);
            }
            else
            {
                start = p2.GetPositionAtOffset(0);
                end = p1.GetPositionAtOffset(0);
            }

            if (!start.IsAtInsertionPosition)
            {
                start = start.GetInsertionPosition(LogicalDirection.Forward);
            }

            paragraph = start.Paragraph;           
            while (start.GetOffsetToPosition(end) > 0)
            {
                object element = start.Parent;
                if (element is Run)
                {
                    Run run = element as Run;
                    tempstr = run.Text;
                    if (leadingIndex < 0)
                    {
                        //the start pointer may be in the middle of a run.
                        tempPointer = paragraph.ContentStart;
                        if (!tempPointer.IsAtInsertionPosition)
                        {
                            tempPointer = tempPointer.GetInsertionPosition(LogicalDirection.Forward);
                        }
                        leadingIndex = tempPointer.GetOffsetToPosition(start);

                        tempstr = tempstr.Substring(leadingIndex);
                    }

                    //Find the offset between the start to the end.
                    offset = start.GetOffsetToPosition(end);
                    
                    //if offset < the length of the text, we are done.
                    if (tempstr.Length > offset)
                    {
                        result += tempstr.Substring(0, offset);
                        break; 
                    }

                    //go the the next run.
                    else
                    {
                        result += tempstr;
                        start = start.GetPositionAtOffset(tempstr.Length);
                        tempPointer = start; 

                        start = start.GetNextInsertionPosition(LogicalDirection.Forward);
                        
                        //Start Reaches the end of the document.
                        if (start == null )
                        {
                            if(tempPointer.GetOffsetToPosition(end) < 0)
                                result += "\r\n"; ;
                            break;
                        }
                        //cross Paragraph.
                        if (start.Paragraph != paragraph)
                        {
                            paragraph = start.Paragraph;
                            result += "\r\n"; 
                        }
                    }
                }
                else
                {
                }   
            }
            return result;
        }

        /// <summary>Test the Invalid parameter of TextRange.Select(Pointer1, Pointer2)</summary>
        void TextRangeSelectInvalidParameterTest()
        {
            TextRange range;
            RichTextBox box;
            ArrayList textPointerList = GetAllTextPointers();

            //Test Null values
            //Check the first Parameter.
            try
            {
                range = new TextRange(null, (TextPointer)textPointerList[0]);
                throw new Exception("Null value for the first Parameter should not be accepted!");
            }
            catch (Exception e)
            {
                if (!(e is ArgumentNullException))
                {
                    if (e.Message == "Null value for the first Parameter should not be accepted!")
                    {
                        Log("Null value for the first Parameter should not be accepted!");
                        pass = false;
                    }
                }
            }
            //Test the second parameter.
            try
            {
                range = new TextRange((TextPointer)textPointerList[0], null);
                throw new Exception("Null value for the secondParameter should not be accepted!");
            }
            catch (Exception e)
            {
                if (!(e is ArgumentNullException))
                {
                    if (e.Message == "Null value for the second Parameter should not be accepted!")
                    {
                        Log("Null value for the second Parameter should not be accepted!");
                        pass = false;
                    }
                }
            }
            //Test Parameter from different TextContainer.
            try
            {
                box = new RichTextBox();
                new TextRange(box.Selection.Start, (TextPointer)textPointerList[0]);
                throw new Exception("Should not able to select from different text Container!");
            }
            catch (Exception e)
            {
                if (e.Message == "Should not able to select from different text Container!")
                {
                    pass = false;
                    Log("Failed:" + "Should not able to select from different text Container!");
                }
            }
        }
        #endregion 
    }

    /// <summary>
    /// Verifies the TextRange Contains method    
    /// </summary>    
    [Test(0, "TextOM", "TextRangeContainsTest", MethodParameters = "/TestCaseType=TextRangeContainsTest")]
    [TestOwner("Microsoft"), TestTactics("391"), TestBugs("751, 603"), TestWorkItem("47"),TestLastUpdatedOn("Jan 25, 2007")]
    //[Test(1, "TextOM", TestCaseSecurityLevel.FullTrust, "TextRangeContainsTest", MethodName="StiEntryPoint", MethodParameters = "/TestCaseType:TextRangeContainsTest", TestParameters = "Class=EntryPointType", Timeout=200)]    
    public class TextRangeContainsTest : CustomTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            StackPanel _panel;

            _panel = new StackPanel();
            _rtb = new RichTextBox();
            
            _panel.Children.Add(_rtb);
            MainWindow.Content = _panel;

            QueueDelegate(TestTextRangeContains);
        }

        private void TestTextRangeContains()
        {
            Paragraph _para;
            TextPointer _rangeStart, _rangeEnd, _rangeMiddle;
            TextPointer _rangeStartReverseDir, _rangeEndReverseDir;
            RichTextBox _otherRTB;
            
            _para = (Paragraph)_rtb.Document.Blocks.FirstBlock;
            _para.Inlines.Clear();
            _para.Inlines.Add(new Run("This is RichTextBox"));

            _rangeEnd = _para.ContentEnd;
            _rangeStart = _para.ContentEnd;            
            _rangeStart = _rangeStart.GetPositionAtOffset(-(((Run)(_para.Inlines.FirstInline)).Text.Length / 2));

            _tr = new TextRange(_rangeStart, _rangeEnd);
            _otherRTB = new RichTextBox();

            try
            {
                _result = _tr.Contains(null);
                throw new ApplicationException("TextRange.Contains accepts null argument");
            }
            catch (ArgumentNullException)
            {
                Log("TextRange.Contains throws ArgumentNullException as expected");
            }

            try
            {
                _result = _tr.Contains(_otherRTB.Document.ContentStart);                                
                throw new ApplicationException("TextRange.Contains accepts TextPointers of other containers");
            }
            catch (ArgumentException)
            {
                Log("TextRange.Contains throws ArgumentException as expected when passing TextPointers of other containers");
            }

            _rangeMiddle = _rangeStart.GetPositionAtOffset(_rangeStart.GetOffsetToPosition(_rangeEnd) / 2);
            Verifier.Verify(_tr.Contains(_rangeMiddle), "Verifying that Contains() return true for pointers inside the range", true);

            Verifier.Verify(!_tr.Contains(_para.ContentStart), "Verifying that Contains() return false for pointers outside the range", true);

            #region Regression_Bug603

            Verifier.Verify(_tr.Contains(_tr.Start), "Verifying that Contains() return true for TextRange.Start", true);
            Verifier.Verify(_tr.Contains(_tr.End), "Verifying that Contains() return true for TextRange.End", true);
            
            _rangeStartReverseDir = _tr.Start;
            _rangeStartReverseDir = _rangeStartReverseDir.GetPositionAtOffset(0, (_tr.Start.LogicalDirection == LogicalDirection.Backward) ? LogicalDirection.Forward : LogicalDirection.Backward);
            Verifier.Verify(_tr.Contains(_rangeStartReverseDir), "Verifying that Contains() return true for TextRange.Start with reverse direction", true);

            _rangeEndReverseDir = _tr.End;
            _rangeEndReverseDir = _rangeEndReverseDir.GetPositionAtOffset(0, (_tr.End.LogicalDirection == LogicalDirection.Forward) ? LogicalDirection.Backward : LogicalDirection.Forward);
            Verifier.Verify(_tr.Contains(_rangeEndReverseDir), "Verifying that Contains() return true for TextRange.End with reverse direction", true);

            //collapsing the textrange
            _tr.Select(_tr.Start, _tr.Start);
            Verifier.Verify(_tr.Contains(_tr.Start), "Verifying that Contains() return true for TextRange.Start for empty TextRange", true);
            Verifier.Verify(_tr.Contains(_tr.End), "Verifying that Contains() return true for TextRange.End for empty TextRange", true);
            
            #endregion Regression_Bug603

            Logger.Current.ReportSuccess();
        }

        #region Private fields

        private RichTextBox _rtb;
        private TextRange _tr;
        private bool _result;

        #endregion Private fields
    }

    /// <summary>
    /// This case tests the basic Functionality of TR.Save/Load/CanSave/CanLoad methods
    /// Coverage for Regression_Bug606 comes from the RichTextContentData.FullyPopulatedContent which has BUIC with image
    /// </summary>
    [Test(2, "PartialTrust", TestCaseSecurityLevel.FullTrust, "TextRangeSaveLoadTest1", MethodParameters = "/TestCaseType:TextRangeSaveLoadTest /XbapName=EditingTestDeploy")]
    [Test(0, "TextOM", "TextRangeSaveLoadTest", MethodParameters = "/TestCaseType=TextRangeSaveLoadTest", Keywords = "MicroSuite")]
    [TestOwner("Microsoft"), TestTactics("390"), TestBugs("752, 606"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextRangeSaveLoadTest : ManagedCombinatorialTestCase
    {
        #region Main flow.

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {            
            string rtbXaml = "<RichTextBox xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation'><FlowDocument>" +
                _contentData.Xaml + "</FlowDocument></RichTextBox>";

            _rtb = (RichTextBox)XamlUtils.ParseToObject(rtbXaml);
            _wrapper = new UIElementWrapper(_rtb);

            _otherRTB = new RichTextBox();
            _otherWrapper = new UIElementWrapper(_otherRTB);

            StackPanel panel = new StackPanel();            
            panel.Children.Add(_rtb);
            panel.Children.Add(_otherRTB);
            TestElement = panel;            
            MainWindow.Height = 700.0;
            MainWindow.Title = "Editing Bvt - TextRangeSaveLoadTest";            

            QueueDelegate(PerformSave);
        }

        private void PerformSave()
        {
            _tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            _otherTR = new TextRange(_otherRTB.Document.ContentStart, _otherRTB.Document.ContentEnd);
            _otherTR.Text = string.Empty;

            if (!_testingCanSaveCanLoadDone)
            {
                DoInvalidTesting();
                DoCanSaveCanLoadTesting();
                _testingCanSaveCanLoadDone = true;
            }

            MemoryStream mStream = new MemoryStream();

            if ((!IsFullTrust()) &&
                ((_testDataFormat.DataFormatString == DataFormats.XamlPackage) || (_testDataFormat.DataFormatString == DataFormats.Rtf)))
            {
                Log("Trying to call TextRange.Save in PT for format (" + _testDataFormat + ")");
                
                try
                {
                    _tr.Save(mStream, _testDataFormat.DataFormatString);
                    throw new ApplicationException("TextRange.Save works for format (" + _testDataFormat + ") in PT");
                }
                catch (ArgumentException)
                {
                    Log("ArgumentException thrown as expected");
                    Verifier.Verify(mStream.Length == 0,
                    "Verifying that stream is empty for restricted formats in PT", true);
                }

                Log("Trying to call TextRange.Load in PT for format (" + _testDataFormat + ")");
                StreamWriter streamWriter = new StreamWriter(mStream);
                streamWriter.Write("Text");
                streamWriter.Flush();
                     
                try
                {
                    _otherTR.Load(mStream, _testDataFormat.DataFormatString);
                    throw new ApplicationException("TextRange.Save works for format (" + _testDataFormat + ") in PT");
                }
                catch (ArgumentException)
                {
                    Log("ArgumentException thrown as expected");
                    Verifier.Verify(_otherTR.IsEmpty,
                    "Verifying that nothing is loaded into TextRange for restricted formats in PT", true);
                }

                QueueDelegate(NextCombination);
            }
            else
            {
                Log("Calling TextRange.Save");                
                _tr.Save(mStream, _testDataFormat.DataFormatString);

                Log("Calling TextRange.Load");                
                _otherTR.Load(mStream, _testDataFormat.DataFormatString);

                QueueDelegate(VerifyTRSaveLoad);   
            }                     
        }

        private void VerifyTRSaveLoad()
        {
            string failReason;

            Verifier.Verify(_tr.Text == _otherTR.Text, "Verifying that TextRange.Text are equal for source and destination TextRange", true);

            if (_testDataFormat.DataFormatString == DataFormats.Text)
            {
                string text="";
                foreach (Block block in _otherRTB.Document.Blocks)
                {
                    if (block is Paragraph)
                    {
                        foreach (Inline inline in ((Paragraph)block).Inlines)
                        {
                            if (inline is Run)
                            {
                                text += ((Run)inline).Text;
                            }
                            else 
                            {
                                Verifier.Verify(false, "There is should be no non-Run Inlines " +
                            "because we are loading only text in to the destination TextRange", true);
                            }
                        }
                        text += "\r\n";
                    }
                    else
                    {
                        Verifier.Verify(false, "There is should be no non-paragraph blocks " +
                            "because we are loading only text in to the destination TextRange", true);
                    }                    
                }

                Log("Source TextRange.Text: [" + _tr.Text + "]");
                Log("Destination TextRange contents: [" + text + "]");
                Verifier.Verify(_tr.Text == text, "Verifying that only text contents are loaded", true);
            }

            if (_testDataFormat.DataFormatString == DataFormats.Xaml)
            {
                Log("Source TR: [" +   XamlUtils.TextRange_GetXml(_tr) + "]");
                Log("Destination TR: [" + XamlUtils.TextRange_GetXml(_otherTR) + "]");
                if (_contentData.ContainsUIElements)
                {
                    if (_contentData.Xaml.Contains("<Image"))
                    {
                        Verifier.Verify(TextOMUtils.EmbeddedObjectCountInRange(_otherTR)==0,
                            "Verifying that image didnt get transfered through Xaml DataFormat", true);
                    }
                }
                else
                {
                    bool result = TextTreeTestHelper.CompareTextRangeContents(_tr.Start, _tr.End, _otherTR.Start, _otherTR.End, out failReason);
                    if (!result)
                    {
                        Log("TextTreeTestHelper fail reason: " + failReason);
                    }

                    Verifier.Verify(result, "Verifying that the source TR and destination TR are equal for " +
                        _testDataFormat.DataFormatString + " format", true);
                }
            }

            if (_testDataFormat.DataFormatString == DataFormats.Rtf)
            {
                Log("Source TR: [" + XamlUtils.TextRange_GetXml(_tr) + "]");
                Log("Destination TR: [" + XamlUtils.TextRange_GetXml(_otherTR) + "]");
                bool result = TextTreeTestHelper.CompareTextRangeContents(_tr.Start, _tr.End, _otherTR.Start, _otherTR.End, out failReason);
                if (!result)
                {
                    Log("TextTreeTestHelper fail reason: " + failReason);
                }

                
                if (_contentData.Xaml.Contains("<Image"))
                {
                    VerifyImage();
                }

                Verifier.Verify(TextOMUtils.EmbeddedObjectCountInRange(_tr) ==
                    TextOMUtils.EmbeddedObjectCountInRange(_otherTR), "Verifying that embedded object count is the same", true);
            }

            if (_testDataFormat.DataFormatString == DataFormats.XamlPackage)
            {
                Log("Source TR: [" + XamlUtils.TextRange_GetXml(_tr) + "]");
                Log("Destination TR: [" + XamlUtils.TextRange_GetXml(_otherTR) + "]");
                bool result = TextTreeTestHelper.CompareTextRangeContents(_tr.Start, _tr.End, _otherTR.Start, _otherTR.End, out failReason);
                if (!result)
                {
                    Log("TextTreeTestHelper fail reason: " + failReason);
                }
                Verifier.Verify(result, "Verifying that the source TR and destination TR are equal for " +
                    _testDataFormat.DataFormatString + " format", true);

                if (_contentData.Xaml.Contains("<Image"))
                {
                    VerifyImage();
                }

                Verifier.Verify(TextOMUtils.EmbeddedObjectCountInRange(_tr) ==
                    TextOMUtils.EmbeddedObjectCountInRange(_otherTR), "Verifying that embedded object count is the same", true);
            }            

            QueueDelegate(NextCombination);
        }

        private void DoInvalidTesting()
        {
            MemoryStream stream = new MemoryStream();

            try
            {
                _tr.Save(null, DataFormats.Text);
                throw new ApplicationException("Save accepts Null stream");
            }
            catch (ArgumentNullException)
            {
                Log("Null exception is thrown as expected for Stream - Save API");
            }

            try
            {
                _tr.Save(stream, null);
                throw new ApplicationException("Save accepts Null dataformat");
            }
            catch (ArgumentNullException)
            {
                Log("Null exception is thrown as expected for DataFormat - Save API");
            }

            try
            {
                _tr.Load(null, DataFormats.Text);
                throw new ApplicationException("Load accepts Null stream");
            }
            catch (ArgumentNullException)
            {
                Log("Null exception is thrown as expected for Stream - Load API");
            }

            _tr.Save(stream, _testDataFormat.DataFormatString);
            stream.Seek(0, SeekOrigin.Begin);
            try
            {
                _tr.Load(stream, null);
                throw new ApplicationException("Load accepts Null dataformat");
            }
            catch (ArgumentNullException)
            {
                Log("Null exception is thrown as expected for DataFormat - Load API");
            }            
        }

        private void DoCanSaveCanLoadTesting()
        {
            DataFormatsData[] trUnSupportedFormats, trSupportedFormats;

            trUnSupportedFormats = DataFormatsData.TRUnSupportedValues;
            foreach (DataFormatsData format in trUnSupportedFormats)
            {
                Verifier.Verify(!_tr.CanSave(format.DataFormatString), "Verifying CanSave for unsupported format: " +
                    format.DataFormatString, true);
                Verifier.Verify(!_tr.CanLoad(format.DataFormatString), "Verifying CanLoad for unsupported format: " +
                    format.DataFormatString, true);
            }            

            trSupportedFormats = DataFormatsData.TRSupportedValues;
            foreach (DataFormatsData format in trSupportedFormats)
            {
                if ((!IsFullTrust()) &&
                ((format.DataFormatString == DataFormats.XamlPackage) || (format.DataFormatString == DataFormats.Rtf)))
                {
                    Verifier.Verify(!_tr.CanSave(format.DataFormatString), "Verifying CanSave for supported format: " +
                        format.DataFormatString + " in PT", true);
                    Verifier.Verify(!_tr.CanLoad(format.DataFormatString), "Verifying CanLoad for supported format: " +
                        format.DataFormatString + " in PT", true);
                }
                else
                {
                    Verifier.Verify(_tr.CanSave(format.DataFormatString), "Verifying CanSave for supported format: " +
                        format.DataFormatString, true);
                    Verifier.Verify(_tr.CanLoad(format.DataFormatString), "Verifying CanLoad for supported format: " +
                        format.DataFormatString, true);
                }
            }            
        }

        private void VerifyImage()
        {            
            Bitmap sourceImageCapture, destImageCapture;         

            ComparisonCriteria criteria;
            ComparisonOperation operation;
            ComparisonResult result;

            //Get source image capture            
            sourceImageCapture = GetImageCapture(_tr, _rtb);                        

            //Get dest image capture            
            destImageCapture = GetImageCapture(_otherTR, _otherRTB);

            Logger.Current.Log("Original SourceImage size: " + sourceImageCapture.Size.ToString());
            Logger.Current.Log("Original DestImage size: " + destImageCapture.Size.ToString());

            if (sourceImageCapture.Size != destImageCapture.Size)
            {
                if (destImageCapture.Height < sourceImageCapture.Height)
                {
                    Rect intersectionRect = new Rect(0, 0, (double)destImageCapture.Width, (double)destImageCapture.Height);
                    intersectionRect.Y = (double)(sourceImageCapture.Height - destImageCapture.Height) / 2;                    

                    sourceImageCapture = BitmapUtils.CreateSubBitmap(sourceImageCapture, intersectionRect);
                }

                if (destImageCapture.Width < sourceImageCapture.Width)
                {
                    Rect intersectionRect = new Rect(0, 0, (double)destImageCapture.Width, (double)destImageCapture.Height);
                    intersectionRect.X = (double)(sourceImageCapture.Width - destImageCapture.Width) / 2;

                    sourceImageCapture = BitmapUtils.CreateSubBitmap(sourceImageCapture, intersectionRect);
                }
            }
            Logger.Current.Log("Adjusted SourceImage size: " + sourceImageCapture.Size.ToString());
            Logger.Current.Log("Adjusted DestImage size: " + destImageCapture.Size.ToString());

            Logger.Current.LogImage(sourceImageCapture, "sourceImage");
            Logger.Current.LogImage(destImageCapture, "targetImage");
            
            //compare the images     
            Log("Comparing the Image Captures");
            criteria = new ComparisonCriteria();            
            criteria.MaxErrorProportion = 0.1f;

            operation = new ComparisonOperation();
            operation.Criteria = criteria;
            operation.MasterImage = sourceImageCapture;
            operation.SampleImage = destImageCapture;
            result = operation.Execute();

            if (!result.CriteriaMet)
            {
                Logger.Current.LogImage(sourceImageCapture, "sourceImage");
                Logger.Current.LogImage(destImageCapture, "targetImage");
                Verifier.Verify(false, "Failed, Image comparing failed!");
            }
        }

        private Bitmap GetImageCapture(TextRange tr, RichTextBox rtb)
        {            
            Bitmap elementCapture, imageCapture;
            Rect startRect, endRect, imageRect;

            startRect = endRect = Rect.Empty;

            //Find the InlineUIContainer's Start and End which contains the Image
            TextPointer tp = tr.Start.GetPositionAtOffset(0);
            while (tp.GetOffsetToPosition(tr.End) > 0)
            {
                if (tp.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.ElementStart)
                {
                    if (tp.GetAdjacentElement(LogicalDirection.Forward) is InlineUIContainer)
                    {
                        InlineUIContainer inlineContainer = (InlineUIContainer)tp.GetAdjacentElement(LogicalDirection.Forward);
                        if (inlineContainer.Child is System.Windows.Controls.Image)
                        {                            
                            startRect = inlineContainer.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                            endRect = inlineContainer.ContentEnd.GetCharacterRect(LogicalDirection.Backward);
                        }
                        break;
                    }
                    if (tp.GetAdjacentElement(LogicalDirection.Forward) is BlockUIContainer)
                    {
                        BlockUIContainer blockContainer = (BlockUIContainer)tp.GetAdjacentElement(LogicalDirection.Forward);
                        if (blockContainer.Child is System.Windows.Controls.Image)
                        {                            
                            startRect = blockContainer.ContentStart.GetCharacterRect(LogicalDirection.Forward);
                            endRect = blockContainer.ContentEnd.GetCharacterRect(LogicalDirection.Backward);
                        }
                        break;
                    }
                }
                tp = tp.GetNextContextPosition(LogicalDirection.Forward);
            }

            if (startRect != Rect.Empty)
            {
                imageRect = Rect.Union(startRect, endRect);                

                elementCapture = BitmapCapture.CreateBitmapFromElement(rtb);
                imageRect = BitmapUtils.AdjustBitmapSubAreaForDpi(elementCapture, imageRect);
                imageCapture = BitmapUtils.CreateSubBitmap(elementCapture, imageRect);

                return imageCapture;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Check to see if a app is a full trust application.
        /// </summary>
        /// <returns></returns>
        public static bool IsFullTrust()
        {
            try
            {
                new System.Security.Permissions.SecurityPermission(
                    System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)
                    .Demand();
                return true;
            }
            catch (System.Security.SecurityException)
            {
                return false;
            }
        }                

        #endregion Main flow.

        #region Private fields.

        private RichTextContentData _contentData=null;
        private DataFormatsData _testDataFormat=null;           
        private RichTextBox _rtb,_otherRTB;
        private UIElementWrapper _wrapper,_otherWrapper;
        private TextRange _tr,_otherTR;
        private bool _testingCanSaveCanLoadDone = false;

        #endregion Private fields.
    }    

    /// <summary>
    /// Verifies the TextRange/TextSelection ApplyPropertyValue method
    /// </summary>
    [Test(0, "TextOM", "TRApplyPropertyValueTest", MethodParameters = "/TestCaseType=TRApplyPropertyValueTest", Timeout=240)]
    [TestOwner("Microsoft"), TestTactics("389"), TestBugs("655"), TestWorkItem("46")]
    public class TRApplyPropertyValueTest : ManagedCombinatorialTestCase
    {
        #region Main flow

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            TextRange tr;
            TextPointer tp;

            _rtb = new RichTextBox();
            _rtbWrapper = new UIElementWrapper(_rtb);

            _rtb.Document.Blocks.Clear();
            tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            XamlUtils.SetXamlContent(tr, _xamlContent);
            
            if (_isSelectionEmpty)
            {
                tp = _rtb.Document.ContentStart.GetPositionAtOffset(_rtbWrapper.Text.Length / 2);
                _rtb.Selection.Select(tp, tp);
            }
            else
            {
                _rtb.Selection.Select(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            }

            TestElement = _rtb;

            QueueDelegate(DoFocus);
        }

        private void DoFocus()
        {
            _rtb.Focus();
            _rtb.Selection.ApplyPropertyValue(_formattingDPData.Property, _formattingDPData.TestValue);

            QueueDelegate(VerifyDPValue);
        }

        private void VerifyDPValue()
        {
            if ((_formattingDPData.Property.ToString() == "FlowDirection")&&(_isSelectionEmpty)&& (_xamlContent != string.Empty))
            {
                Verifier.Verify(_rtb.Selection.GetPropertyValue(_formattingDPData.Property).ToString() == FlowDirection.LeftToRight.ToString(),
                  "Verifying that property is applied to the selection Expected Value on span[" + FlowDirection.LeftToRight.ToString() +
                  "] Actual value [" + _rtb.Selection.GetPropertyValue(_formattingDPData.Property).ToString() + "]", true);
                Log("-----------------*****-----------------------");
                Log(XamlWriter.Save(_rtb));
                Log("-----------------*****-----------------------");
                Verifier.Verify(_rtb.Document.Blocks.FirstBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() == _formattingDPData.TestValue.ToString(),
                  "Verifying that property is applied to the selection Expected Value on Para[" + _formattingDPData.TestValue.ToString() +
                  "] Actual value [" + _rtb.Document.Blocks.FirstBlock.GetValue(Paragraph.FlowDirectionProperty).ToString() + "]", true);
            }
            else
            {
                Verifier.Verify(_rtb.Selection.GetPropertyValue(_formattingDPData.Property).ToString() == _formattingDPData.TestValue.ToString(),
                    "Verifying that property is applied to the selection Expected Value[" + _formattingDPData.TestValue.ToString() +
                    "] Actual value [" + _rtb.Selection.GetPropertyValue(_formattingDPData.Property).ToString() + "]", true);
            }
            if (!_invalidTestingDone)
            {
                DoInvalidTesting();
                _invalidTestingDone = true;
            }

            QueueDelegate(NextCombination);            
        }

        private void DoInvalidTesting()
        {
            try
            {
                _rtb.Selection.ApplyPropertyValue(null, true);
                throw new ApplicationException("ApplyPropertyValue accepted an null as argument for DP");
            }
            catch (ArgumentNullException)
            {
                Log("Argument exception thrown as expected when passing null property");
            }

            try
            {
                _rtb.Selection.ApplyPropertyValue(TextElement.FontWeightProperty, true);
                throw new ApplicationException("ApplyPropertyValue accepted an invalid value for DP");
            }
            catch (ArgumentException)
            {
                Log("Argument exception thrown as expected when passing invalid value to the property");
            }

            try
            {
                _rtb.Selection.ApplyPropertyValue(RichTextBox.AcceptsReturnProperty, true);
                throw new ApplicationException("ApplyPropertyValue accepted an non-formatting property as argument");
            }
            catch (ArgumentException)
            {
                Log("Argument exception thrown as expected when passing non-formatting property");
            }
        }

        #endregion Main flow

        #region Private fields

        /// <summary>Formatting DependencyProperty being used in the test</summary>
        private DependencyPropertyData _formattingDPData=null;
        /// <summary>Whether the selection is empty in this test</summary>
        private bool _isSelectionEmpty=false;
        /// <summary>XamlContent used in the test</summary>
        private string _xamlContent=string.Empty;

        private RichTextBox _rtb;
        private UIElementWrapper _rtbWrapper;
        private bool _invalidTestingDone = false;

        #endregion Private fields
    }

    /// <summary>
    /// Verifies the TextRange GetPropertyValue method    
    /// </summary>
    [Test(0, "TextOM", "TextRangeGetPropertyValueTest", MethodParameters = "/TestCaseType=TextRangeGetPropertyValueTest")]
    [TestOwner("Microsoft"), TestTactics("388"), TestBugs("609, 753"), TestWorkItem("46"), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextRangeGetPropertyValueTest : CustomCombinatorialTestCase
    {
        #region TestData
        private class TestData
        {
            private string _xamlContent;
            private DependencyProperty _testProperty;
            private object _expectedValue;

            /// <summary>
            /// Constructor for TestData
            /// </summary>
            /// <param name="xamlContent">xaml content for RichTextBox</param>
            /// <param name="testProperty">DependencyProperty to be tested</param>
            /// <param name="expectedValue">Expected value for the dependency property</param>
            public TestData(string xamlContent, DependencyProperty testProperty, object expectedValue)
            {
                _xamlContent = xamlContent;
                _testProperty = testProperty;
                _expectedValue = expectedValue;
            }

            /// <summary>Xaml content for RichTextBox</summary>
            public string XamlContent
            {
                get { return _xamlContent; }
            }

            /// <summary>DependencyProperty to be tested</summary>
            public DependencyProperty TestProperty
            {
                get { return _testProperty; }
            }

            /// <summary>Expected value for the dependency property</summary>
            public object ExpectedValue
            {
                get { return _expectedValue; }
            }

            /// <summary>ToString override</summary>
            /// <returns>String representation of the TestData</returns>
            public override string ToString()
            {
                string _output = "XamlContent[" + XamlContent + "]" + Environment.NewLine;
                _output += "TestProperty[" + TestProperty.ToString() + "]" + Environment.NewLine;
                _output += "ExpectedValue[" + ExpectedValue.ToString() + "]";
                return _output;
            }
        }
        #endregion TestData

        #region Main flow
        /// <summary>Gets the dimensions to combine.</summary>
        protected override Dimension[] DoGetDimensions()
        {
            SolidColorBrush expectedSCB;
            if (Win32.IsThemeActive())
            {
                expectedSCB = System.Windows.SystemColors.ControlTextBrush;
            }
            else
            {
                expectedSCB = System.Windows.SystemColors.WindowTextBrush;
            }

            
            TestData[] _testDataArray = new TestData[]{
                new TestData("", TextElement.ForegroundProperty, expectedSCB),
                new TestData("<Paragraph></Paragraph>", TextElement.FontSizeProperty, System.Windows.SystemFonts.MessageFontSize),
                new TestData("<Paragraph>This is a RichTextBox</Paragraph>", TextElement.FontWeightProperty, FontWeights.Normal),
                new TestData("<Italic>Italic</Italic>", TextElement.FontStyleProperty, FontStyles.Italic),
                new TestData("<Paragraph></Paragraph>", Paragraph.LineHeightProperty, Double.NaN), //repro for Regression_Bug609
                new TestData("<Span FontSize='12'>Inline1</Span><Span FontSize='24'>Inline2</Span>",
                TextElement.FontSizeProperty, DependencyProperty.UnsetValue),
            };            

            return new Dimension[] {          
                new Dimension("TestData", _testDataArray),                
            };
        }

        /// <summary>Reads combination values.</summary>
        protected override bool DoReadCombination(Hashtable values)
        {
            _testData = (TestData)values["TestData"];            
            return true;
        }

        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {            
            StackPanel _panel = new StackPanel();

            _rtb = new RichTextBox();
            _rtb.Height = 400;
            _rtb.Width = 400;
            _rtbWrapper = new UIElementWrapper(_rtb);
            
            if (!_invalidTestingDone)
            {
                DoInvalidTesting();
                _invalidTestingDone = true;
            }

            _rtbWrapper.XamlText = _testData.XamlContent;

            _panel.Children.Add(_rtb);            
            TestElement = _panel;
            
            QueueDelegate(TestGetPropertyValue);
        }

        private void TestGetPropertyValue()
        {
            object _propertyValue, _selectionPropertyValue;
            
            TextRange _tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            _rtb.Selection.Select(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);

            _propertyValue = _tr.GetPropertyValue(_testData.TestProperty);
            _selectionPropertyValue = _rtb.Selection.GetPropertyValue(_testData.TestProperty);
            if ( (_propertyValue.ToString() != _testData.ExpectedValue.ToString()) ||
                (_selectionPropertyValue.ToString() != _testData.ExpectedValue.ToString()) )
            {
                Log("Actual value   : " + _propertyValue);                
                Log("Expected value : " + _testData.ExpectedValue);
                Log("Actual value from selection :" + _selectionPropertyValue);
                Logger.Current.ReportResult(false, "Actual value didnt match with expected value", false);
            }

            _tr.ClearAllProperties();
            _rtb.Selection.ClearAllProperties();

            QueueDelegate(NextCombination);            
        }

        private void DoInvalidTesting()
        {
            TextRange _tr = new TextRange(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);
            _rtb.Selection.Select(_rtb.Document.ContentStart, _rtb.Document.ContentEnd);

            //Invalid testing for TextRange.GetPropertyValue()
            try
            {
                _tr.GetPropertyValue(null);
                throw new ApplicationException("GetPropertyValue accepts null parameter");
            }
            catch (ArgumentNullException)
            {
                Log("ArgumentNullException thrown as expected");
            }

            try
            {
                _tr.GetPropertyValue(TextBox.AcceptsTabProperty);
                throw new ApplicationException("GetPropertyValue accepts non character/block properties");
            }
            catch (ArgumentException)
            {
                Log("ArgumentException thrown as expected");
            }

            //Invalid testing for TextSelection.GetPropertyValue()
            try
            {
                _rtb.Selection.GetPropertyValue(null);
                throw new ApplicationException("TextSelection.GetPropertyValue accepts null parameter");
            }
            catch (ArgumentNullException)
            {
                Log("ArgumentNullException thrown as expected");
            }

            try
            {
                _rtb.Selection.GetPropertyValue(TextBox.AcceptsTabProperty);
                throw new ApplicationException("TextSelection.GetPropertyValue accepts non character/block properties");
            }
            catch (ArgumentException)
            {
                Log("ArgumentException thrown as expected");
            }
        }
        
        #endregion Main flow

        #region Private fields
        private TestData _testData;        
        
        RichTextBox _rtb;        
        UIElementWrapper _rtbWrapper;
        private bool _invalidTestingDone = false;
        #endregion Private fields
    }    

    /// <summary>
    /// Verifies that text range normalization works as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("123"), TestBugs("754,755")]
    public class TextRangeNormalization: CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _generator = new ContentGenerator(0);
            while (GenerateContent())
            {
                TestNormalizationInContent();
            }
            Logger.Current.ReportSuccess();
        }

        private void TestNormalizationInContent()
        {
            int positionCount;

            positionCount = TextUtils.CountContainerPositions(_contentStart);

            for (int startIndex = 0; startIndex < positionCount; startIndex++)
            {
                for (int endIndex = 0; endIndex < positionCount; endIndex++)
                {
                    TestRangeNormalization(startIndex, endIndex);
                }
            }
        }

        private void TestRangeNormalization(int startIndex, int endIndex)
        {
            TextPointer start;          // Start pointer  to normalize.
            TextPointer end;            // End pointer to normalize.
            TextPointer normalStart;    // Expected normalized start pointer.
            TextPointer normalEnd;      // Expected normalized end pointer.
            TextPointer originalStart;  // Clone of start pointer, before constructor.
            TextPointer originalEnd;    // Clone of end pointer, before constructor.
            TextRange range;            // Range performing the normalization.
            bool match;                 // Whether expected and actual ponters match.

            start = _contentStart;
            end = _contentStart;

            start = start.GetPositionAtOffset(startIndex);
            end = end.GetPositionAtOffset(endIndex);

            originalStart = start;
            originalEnd = end;

            Log("Creating range at indexes " + startIndex + " and " + endIndex +
                " (effectively " + TextUtils.GetDistanceFromStart(start) + " and " +
                TextUtils.GetDistanceFromStart(end) + ")");
            range = new TextRange(start, end);
            Verifier.Verify(start.CompareTo(originalStart) == 0,
                "Start was moved by TextRange constructor to " + TextUtils.GetDistanceFromStart(start), false);
            Verifier.Verify(end.CompareTo(originalEnd) == 0,
                "End was moved by TextRange constructor to " + TextUtils.GetDistanceFromStart(end), false);

            NormalizePointers(start, end, out normalStart, out normalEnd, false);

            match = range.Start.CompareTo(normalStart) == 0 && range.End.CompareTo(normalEnd) == 0;
            if (match)
            {
                Log("Range matches expected normalized pointers.");
            }
            else
            {
                Log("Range does not match expected normalized pointers.");
                TextTreeLogger.LogContainer("normalization", _contentStart,
                    range.Start, "Range Start",
                    range.End, "Range End",
                    start, "Initial Start",
                    end, "Initial End",
                    normalStart, "Expected Start",
                    normalEnd, "Expected End");
                throw new Exception("Range start/end match expected start/end. " +
                    "See normalization image for details.");
            }
        }

        #endregion Main flow.

        #region Helper methods.

        /// <summary>
        /// Generates content and initializes the _contentStart and
        /// _contentEnd fields.
        /// </summary>
        /// <returns>
        /// true if content was generated, false if all combinations have
        /// already been generated.
        /// </returns>
        private bool GenerateContent()
        {
            bool contentGenerated;  // Whether content has been generated.
            RichTextBox box;        // Control into which to create content.

            box = new RichTextBox();
            _contentStart = box.Document.ContentStart;
            _contentEnd = box.Document.ContentEnd;

            contentGenerated = _generator.AddContent(_contentEnd);
            if (contentGenerated)
            {
                Log("Generated content:\r\n[" + new UIElementWrapper(box).XamlText + "]");
            }

            return contentGenerated;
        }

        /// <summary>Checks whether the specified range is normalized.</summary>
        /// <param name="range">Range to check.</param>
        /// <returns>true if the range is normalized, false otherwise.</returns>
        private static bool IsNormalized(TextRange range)
        {
            TextPointer normalStart;
            TextPointer normalEnd;

            if (range == null)
            {
                throw new ArgumentNullException("range");
            }

            NormalizePointers(range.Start, range.End,
                out normalStart, out normalEnd, range is TextSelection);

            return (range.Start.CompareTo(normalStart) == 0 && (range.End.CompareTo(normalEnd) == 0));
        }

        /// <summary>
        /// Calculates what the normalize range would be for two pointers.
        /// </summary>
        /// <param name="start">Pointer to start.</param>
        /// <param name="end">Pointer to end.</param>
        /// <param name="normalStart">On return, normalized start pointer.</param>
        /// <param name="normalEnd">On return, normalized end pointer.</param>
        /// <param name="isSelection">Whether the range will be a TextSelection.</param>
        private static void NormalizePointers(TextPointer start,
            TextPointer end, out TextPointer normalStart,
            out TextPointer normalEnd, bool isSelection)
        {
            bool pointersFlipped;

            if (start == null)
            {
                throw new ArgumentNullException("start");
            }
            if (end == null)
            {
                throw new ArgumentNullException("end");
            }

            normalStart = start;
            normalEnd = end;
            if (normalStart.CompareTo(normalEnd) == 0)
            {
                normalStart = normalEnd;
            }

            // Fixed flipped pointers.
            pointersFlipped = normalStart.CompareTo(normalEnd) > 0;
            if (pointersFlipped)
            {
                TextPointer swap;

                swap = normalStart;
                normalStart = normalEnd;
                normalEnd = swap;
            }

            // Fix inside invalid combinations.
            NormalizeFromNewLine(normalEnd, false);
            NormalizeFromNewLine(normalStart, true);
            NormalizeFromSurrogatePair(normalEnd, false);
            NormalizeFromSurrogatePair(normalStart, true);

            // Normalize from combining characters. Prevent
            // crossing.
            NormalizeFromCombiningCharacters(normalEnd, false);
            if (normalEnd.CompareTo(normalStart) > 0)
            {
                NormalizeFromCombiningCharacters(normalStart, true);
            }

            // If the pointers crossed at some point, then
            // keep the one closest to the start.
            if (normalEnd.CompareTo(normalStart) > 0)
            {
                normalStart = normalEnd;
            }
        }

        #region Normalization helpers.

        private static bool IsHighSurrogate(char ch)
        {
            return ch >= 0xd800 && ch < 0xdc00;
        }

        private static bool IsLowSurrogate(char ch)
        {
            return ch >= 0xdc00 && ch < 0xe000;
        }

        private static bool IsCombiningCharacter(char ch)
        {
            // NOTE: these are only the Latin combining
            // diacritic marks - there are many more
            // combining characters.
            return ch >= 0x0300 && ch <= 0x362;
        }

        private static char GetCharacter(TextPointer pointer,
            LogicalDirection direction)
        {
            char[] character;

            if (pointer == null)
            {
                throw new ArgumentNullException("pointer");
            }

            character = new char[1];
            if (pointer.GetTextInRun(direction, character, 0, 1) != 1)
            {
                throw new Exception("Unable to get a character " +
                    "in direction: " + direction);
            }
            return character[0];
        }

        #endregion Normalization helpers.

        /// <summary>
        /// Normalizes the specified pointer outside of a combining
        /// character sequence if applicable.
        /// </summary>
        /// <param name="pointer">Pointer to normalize.</param>
        /// <param name="isStart">Whether it's a start pointer.</param>
        private static void NormalizeFromCombiningCharacters(TextPointer pointer,
            bool isStart)
        {
            LogicalDirection direction;
            int distance;

            direction = (isStart)? LogicalDirection.Forward : LogicalDirection.Backward;
            distance = (isStart)? 1 : -1;

            // We only normalize in some direction if we are in front
            // of a combining character.
            if (pointer.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text &&
                !IsCombiningCharacter(GetCharacter(pointer, LogicalDirection.Forward)))
            {
                return;
            }

            while (pointer.GetPointerContext(direction) == TextPointerContext.Text &&
                IsCombiningCharacter(GetCharacter(pointer, direction)))
            {
                pointer = pointer.GetPositionAtOffset(distance);
            }

            // After moving across combining characters, move beyond the
            // base character as well (if we're moving in that direction).
            if (direction == LogicalDirection.Backward &&
                pointer.GetPointerContext(direction) == TextPointerContext.Text)
            {
                System.Diagnostics.Debug.Assert(distance == -1);
                pointer = pointer.GetPositionAtOffset(distance);
            }
            return;
        }

        /// <summary>
        /// Normalizes the specified pointer outside of a \r\n combination
        /// if applicable.
        /// </summary>
        /// <param name="pointer">Pointer to normalize.</param>
        /// <param name="isStart">Whether it's a start pointer.</param>
        private static void NormalizeFromNewLine(TextPointer pointer, bool isStart)
        {
            string text;

            text = pointer.GetTextInRun(LogicalDirection.Backward);
            if (text.Length > 0 && text[text.Length-1] == '\r')
            {
                text = pointer.GetTextInRun(LogicalDirection.Forward);
                if (text.Length > 0 && text[0] == '\n')
                {
                    pointer = pointer.GetPositionAtOffset((isStart)? 1 : -1);
                }
            }
        }

        /// <summary>
        /// Normalizes the specified pointer outside of a surrogate pair
        /// if applicable.
        /// </summary>
        /// <param name="pointer">Pointer to normalize.</param>
        /// <param name="isStart">Whether it's a start pointer.</param>
        private static void NormalizeFromSurrogatePair(TextPointer pointer, bool isStart)
        {
            string text;

            text = pointer.GetTextInRun(LogicalDirection.Backward);
            if (text.Length > 0 && IsHighSurrogate(text[text.Length-1]))
            {
                text = pointer.GetTextInRun(LogicalDirection.Forward);
                if (text.Length > 0 && IsLowSurrogate(text[0]))
                {
                    pointer = pointer.GetPositionAtOffset((isStart)? 1 : -1);
                }
            }
        }

        #endregion Helper methods.

        #region Private data.

        private ContentGenerator _generator;
        private TextPointer _contentStart;
        private TextPointer _contentEnd;

        #endregion Private data.

        #region Inner types.

        /// <summary>Generates interesting content.</summary>
        class ContentGenerator
        {
            /// <summary>Kind of content to generate.</summary>
            enum ContentKind {
                EmptyString,
                Character,
                Space,
                NewLinePair,
                Tab,
                SurrogatePair,
                CombiningCharacter,
            };

            /// <summary>Depth of this generator.</summary>
            private int _level;
            /// <summary>Combinatorial engine for sequences.</summary>
            private CombinatorialEngine _engine;
            /// <summary>Contents of combination.</summary>
            private object[] _contents;

            /// <summary>Initializes a new content generator.</summary>
            /// <param name="level">Depth for this generator.</param>
            internal ContentGenerator(int level)
            {
                const int DimensionCount = 3;   // Number of dimensions.
                object[] contents;              // Contents in each dimension.
                Dimension[] dimensions;         // Dimensions to combine.

                contents = new object[] {
                    ContentKind.EmptyString,
                    ContentKind.Character,
                    ContentKind.Space,
                    ContentKind.NewLinePair,
                    ContentKind.Tab,
                    ContentKind.SurrogatePair,
                    ContentKind.CombiningCharacter,
                    (level > 3)? ContentKind.EmptyString : ContentKind.Character,
                };

                dimensions = new Dimension[DimensionCount];
                for (int i = 0; i < dimensions.Length; i++)
                {
                    dimensions[i] = new Dimension("Dimension-" + i, contents);
                }

                _engine = CombinatorialEngine.FromDimensions(dimensions);
                _contents = new object[DimensionCount];
                _level = level;
            }

            /// <summary>Adds content at the specified pointer.</summary>
            /// <param name="p">Pointer at which to add content.</param>
            /// <returns>Whether any content was added.</returns>
            internal bool AddContent(TextPointer p)
            {
                Hashtable values;

                values = new Hashtable();
                if (!_engine.Next(values))
                {
                    return false;
                }

                for (int i = 0; i < _contents.Length; i++)
                {
                    _contents[i] = values["Dimension-" + i];
                    if (_contents[i] is ContentKind)
                    {
                        ContentKind kind;

                        kind = (ContentKind)_contents[i];
                        switch (kind)
                        {
                            case ContentKind.EmptyString:
                                break;
                            case ContentKind.Character:
                                p.InsertTextInRun("a");
                                break;
                            case ContentKind.Space:
                                p.InsertTextInRun(" ");
                                break;
                            case ContentKind.NewLinePair:
                                p.InsertTextInRun("\r\n");
                                break;
                            case ContentKind.Tab:
                                p.InsertTextInRun("\t");
                                break;
                            case ContentKind.SurrogatePair:
                                p.InsertTextInRun("\xd869\xded6");
                                break;
                            case ContentKind.CombiningCharacter:
                                p.InsertTextInRun("\x0303");
                                break;
                        }
                    }
                }
                return true;
            }
        }

        #endregion Inner types.
    }


    /// <summary>
    /// The Changed event should fire on the range for each range reposition.
    /// 1. will use Selection and Caret as to test the scenario:
    /// 2. Selection.Select(p1, p2) - should always fire the event.
    /// 3. When Caret is moved - the event fires.
    /// 4. Selection Expanding - should fires the event.
    /// </summary>
    [Test(1, "TextOM", "TextRangeChangedEventTest1", MethodParameters = "/TestCaseType=TextRangeChangedEventTest /Pri:1", Timeout=240)]
    [Test(0, "TextOM", "TextRangeChangedEventTest", MethodParameters = "/TestCaseType=TextRangeChangedEventTest", Timeout = 210, Keywords = "Localization_Suite")]
    [TestOwner("Microsoft"), TestBugs("608, 607"), TestTactics("386, 387"), TestWorkItem(""), TestLastUpdatedOn("Jan 25, 2007")]
    public class TextRangeChangedEventTest : ManagedCombinatorialTestCase
    {
       /// <summary>
       /// Start to run the case
       /// </summary>
       protected override void DoRunCombination()
        {
            if (!(TestElement is RichTextBox) || _wrapper == null)
            {
                _wrapper = new UIElementWrapper(new RichTextBox());

                TestElement = _wrapper.Element as FrameworkElement;
                ((RichTextBox)_wrapper.Element).AcceptsTab = true;
            }
      
            QueueDelegate(SetFocus);
        }

        /// <summary>
        /// Set focus to the main editing control - RichTextBox
        /// </summary>
        void SetFocus()
        {
            _wrapper.Element.Focus();

            QueueDelegate(SetSelection);           
        }

        /// <summary>
        /// Set selection before perfrom action
        /// BVT cases use InitialDocument setting.
        /// p1 cases use SelectionData combinations.
        /// </summary>
        void SetSelection()
        {
            //P1 cases uses SelectionData which creates more combination.
            if (_selectionData != null)
            {
                _selectionData.PrepareForSelection(_wrapper);
                _selectionData.Select(_wrapper);
            }
            //BVT cases
            //1. empty document
            else if (_initialDocument == DocumentState.DocumentEmpty)
            {
                _wrapper.Text = "";
            }
            //BVT cases.
            //2. Caret in bettween of text.
            else if (_initialDocument == DocumentState.CaretInMiddle)
            {
                _wrapper.Text = "abcd";
                _wrapper.SelectionInstance.Select(_wrapper.Start.GetPositionAtOffset(3), _wrapper.Start.GetPositionAtOffset(3));
            }
            //BVT cases.
            //3. Selection container text.
            else if (_initialDocument == DocumentState.SelectionNonEmpty)
            {
                _wrapper.Text = "abcd";
                _wrapper.SelectionInstance.Select(_wrapper.Start.GetPositionAtOffset(3), _wrapper.Start.GetPositionAtOffset(5));
            }

            QueueDelegate(PerfromKeyboardAction);
        }

        /// <summary>
        /// Perfrom a Keyboard Action. 
        /// </summary>
        void PerfromKeyboardAction()
        {
            _initialState = CaptureSelectionState();
            
            //reset the Event firing counter
            _eventFireConter = 0; 

            //hookup the event handler. 
            _wrapper.SelectionInstance.Changed += new EventHandler(SelectionInstance_Changed);

            //Perfrom Action to change to selection. 
            KeyboardInput.TypeString(_action);

            QueueDelegate(DoVerification);
        }

       /// <summary>
       /// Catch the event firing 
       /// </summary>
       /// <param name="sender"></param>
       /// <param name="e"></param>
        void SelectionInstance_Changed(object sender, EventArgs e)
        {
            _eventFireConter++;
        }

        /// <summary>
        /// Verify if the changed event fires as expected.
        /// Any keyboard action that cause the Selection.Start or Selection.End to change should trigger the event to fires.
        /// </summary>
        void DoVerification()
        {
            _FinalState = CaptureSelectionState();
            
            if (_FinalState.EndOffset != _initialState.EndOffset || _FinalState.StartOffset != _initialState.StartOffset)
            {
                Log("Initial_StartOffset[" + _initialState.StartOffset + "]");
                Log("Initial_EndOffset[" + _initialState.EndOffset + "]");
                Log("Final_StartOffset[" + _FinalState.StartOffset + "]");
                Log("Final_EndOffset[" + _FinalState.EndOffset + "]");

                //the formating commands do change the offset of the selection, however, it won't triggle event.
                //Regression_Bug607 - Editing: TextRange.Changed event won't fire when a format( (^u, ^v, ^i)) is applied on a selection
                if (!(_action == "^b" || _action == "^u" || _action == "^i") )
                {

                    Verifier.Verify(_eventFireConter == 1,
                        "Failed - Changed event fire incorrectly. Expected fires[1], Actual fires[" + _eventFireConter.ToString() + "]");
                }
            }
            //Regression_Bug608, when this bug is fixed, remove the IsBlock() function call.
            //When the Selection is at the Boundary, Keyboard action will not change its location. 
            else if (!IsBlocked())
            {
                Verifier.Verify(_eventFireConter == 0,
                    "Failed - Changed event fire incorrectly. Expected fires[0], Actual fires[" + _eventFireConter.ToString() + "]");
            }

            //Re-Select using Selection.Select(p1, p2);
            int expectedCount = _eventFireConter + 1; 
            _wrapper.SelectionInstance.Select(_wrapper.SelectionInstance.Start, _wrapper.SelectionInstance.End);
            Verifier.Verify(expectedCount == _eventFireConter, 
                "Failed - on Selection.Select(p1, p2). Expected fires[" + expectedCount + "], Actual fires["+ _eventFireConter + "]");
            _wrapper.SelectionInstance.Changed -= new EventHandler(SelectionInstance_Changed);
            
            QueueDelegate(NextCombination);
        }

        /// <summary>
        /// Capture the selection status
        /// </summary>
        /// <returns></returns>
        SelectionState CaptureSelectionState()
        {
            return new SelectionState(_wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.Start), _wrapper.Start.GetOffsetToPosition(_wrapper.SelectionInstance.End));
        }

        /// <summary>
        /// When caret is at a boundary edge (for example, at an empty document), Some actions triggle the event to fire without changing the location of the selection.
        /// Regression_Bug608 - Editing: TextRange.Changed event fires for a Keyboard action that does not cause selection change.
        /// </summary>
        bool IsBlocked()
        {
            if (_action.Contains("DOWN") ||
                _action.Contains("UP") || 
                _action.Contains("HOME") ||
                _action.Contains("END")||
                _action.Contains("PGDN") || 
                _action.Contains("^") ||
                 _action.Contains("DELETE"))
            {
                return true;
            }
         
            return false;
        }
        
        public new static string[] Actions
        {
            get
            {
                string[] result = new string[]
                {
                    "a",
                    "{ENTER}",
                    "+{ENTER}",
                    "{DELETE}",
                    "^{DELETE}",
                    "{BACKSPACE}",
                    "+{BACKSPACE}",
                    "^{BACKSPACE}",
                    "{SPACE}",
                    "{TAB}",
                    "+{TAB}",
                    "{LEFT}",
                    "{RIGHT}",
                    "{UP}",
                    "{DOWN}",
                    "{HOME}",
                    "{END}",
                    "{PGUP}",
                    "{PGDN}",
                    "+{LEFT}",
                    "+{RIGHT}",
                    "+{UP}",
                    "+{DOWN}",
                    "+{HOME}",
                    "+{END}",
                    "+{PGUP}",
                    "+{PGDN}",
                    "^{LEFT}",
                    "^{RIGHT}",
                    "^{UP}",
                    "^{DOWN}",
                    "^{PGUP}",
                    "^{PGDN}",
                    "^{HOME}",
                    "^{END}",
                    "^+{LEFT}",
                    "^+{RIGHT}",
                    "^m",
                    "^+m",
                    "^z",
                    "^y",
                    "^b",
                    "^c",
                    "^u",
                    "^i",
                };
                return result;
            }
        }

        UIElementWrapper _wrapper;
        SelectionState _initialState;
        SelectionState _FinalState;
        int _eventFireConter = 0;
        DocumentState _initialDocument=0;
        TextSelectionData _selectionData=null;
        string _action = string.Empty;
    }

    /// <summary>
    /// remember the state of a selection
    /// </summary>
    internal class SelectionState 
    {
        int _startOffset;
        int _endOffset;

        /// <summary>
        /// constructor.
        /// </summary>
        /// <param name="start"></param>
        /// <param name="end"></param>
        public SelectionState(int start, int end)
        {
            _startOffset = start;
            _endOffset = end;  
        }

        /// <summary>
        /// Retrieve the Selection.Start offset to the Document.ContentStart
        /// </summary>
        public int StartOffset
        {
            get
            {
                return _startOffset; 
            }
        }

        /// <summary>
        /// Retrieve the Selection.End offset to the Document.ContentStart
        /// </summary>
        public int EndOffset
        {
            get
            {
                return _endOffset;
            }
        }
    }
    
    internal enum DocumentState
    {
        DocumentEmpty,
        CaretInMiddle,
        SelectionNonEmpty,
    }
}

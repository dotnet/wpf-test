// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/********************************************************************
 * This file helps the DEV's to run some BVT cases for regressions 
 * before they check in something. It assume that all the support files
 * are coppied in the running directory.
*********************************************************************/

namespace AvalonEditingTest
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Text;

    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;

    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Security.Permissions;
    using System.Threading;

    #endregion Namespaces.

    /// <summary>
    /// Developer BVT suite. 
    /// </summary>
    public class DevBvtSuite
    {
        /// <summary>
        /// TestCases to run
        /// </summary>
        private static string[] s_testCases = {           
            //CaseOwner=Microsoft
            "/TestCaseType=DragDropAPITest /DragDrop=Valid /CaseNo=188",
            "/TestCaseType=DragDropUIText /TestName=DragDropUIText-Canvas-CrossContainer-DragDrop /CaseNo=79 /CaseOwner=Microsoft" ,
            "/TestCaseType=PrevieweventDragDrop /Log=Yes /CaseNo=192 /CaseOwner=Microsoft",
            "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:23 /CaseNo=220 /CaseOwner=Microsoft",
            "/TestCaseType=CommandServiceClipboardQuery /CaseNo=68 /CaseOwner=Microsoft",
            "/TestCaseType=ActionDrivenTest /TestName=Commanding-ReproRegression_Bug396 /CaseNo=70 /CaseOwner=Microsoft",
            "/TestCaseType=EditCommandText /TestName=EditCommandText-Canvas-TextBox-AllCommand /CaseNo=71 /CaseOwner=Microsoft",
            "/TestCaseType=PlainTextCommandsTest /CaseNo=69 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectReproRegression_Bug360 /CaseNo=176 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectAPITest /caseIndex=1 /CaseNo=166 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectAPITest /caseIndex=3 /CaseNo=167 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectAPITest /caseIndex=4 /CaseNo=168 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectAPITest /caseIndex=5 /CaseNo=169 /CaseOwner=Microsoft",
            "/TestCaseType=DataObjectAPITest /caseIndex=8 /CaseNo=170 /CaseOwner=Microsoft",
            "/TestCaseType=ClipboardTest /TestText1=mytextstring.123@#$ /TestText2=abcd /Action=NoFlush /CaseNo=75807 /CaseOwner=Microsoft",
            "/TestCaseType=CutCopyPastePlanText /Xaml=CutCopyPastePlanText3.xaml /InputText=\"abc def\" /Action=SetClipboardDataUniCode /SelectText=\"abc\" /CrossApp=true /CaseNo=56 /CaseOwner=Microsoft",
            
            //CaseOwner=Microsoft
            "/TestCaseType=ParagraphCreateAndSplitBVT /CaseNo=696 /CaseOwner=Microsoft",
            "/TestCaseType=ParagraphEditingWithMouse /Case=MouseToSetCaretAndSelectText /CaseNo=690 /CaseOwner=Microsoft",
            //"/TestCaseType=UndoMultipleLanguagesTest /Case=UndoDeleteAndBackSpaceTest /data=!AD:index=0;length=10 /CaseNo=364",
            //"/TestCaseType=ParagraphEditingTestWithKeyboard /Case=RunAllCases CaseNo=692",
            //"/TestCaseType=ParagraphEditingWithMouse /Case=RunAllCases CaseNo=691",
            //"/TestCaseType=UndoEmbededObjectTest /Case=UndoSingleElement /ControlType=TextBox /CaseNo=91738",
            "/TestCaseType=UndoFormatingTest /Case=UndoFormating /Format=bold /CaseNo=366",

            // CaseOwner=Microsoft
            "/TestCaseType=TextBoxScrollScrollbars /TestName=TextBoxScrollScrollbars-Sizes /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxAcceptsTab /TestName=TextBoxAcceptsTab-False /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxRenderTyping /TextToType=sample /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxDefaults /CaseOwner=Microsoft",


            // CaseWoner=Microsoft
            "/TestCaseType:CharFormattingBIU /CaseNo=474 /CaseOwner=Microsoft",
            "/TestCaseType:FontFormatting /CaseNo=483",
            "/TestCaseType:SelectionByKeyboardTest /Priority:0 /CaseNo=671 /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxAddChild /CaseNo=589 /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxContentPropsStable /CaseNo=532 /CaseOwner=Microsoft",
            "/TestCaseType=TextControlContextCreation /CaseNo=647 /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxToolTipTab /CaseNo=644",
            "/TestCaseType=TextBoxUIElementFocusFromButton /CaseNo=642 /CaseOwner=Microsoft",
            "/TestCaseType=TextBoxCursor /CaseNo=641 /CaseOwner=Microsoft",
            "/TestCaseType:MaxMinLinesTest /CaseNo=585 /CaseOwner=Microsoft",
            "/TestCaseType:TextBoxHeightTest /CaseNo=584 /CaseOwner=Microsoft",
            "/TestCaseType:TextBoxFrameworkElement /CaseNo=646 /CaseOwner=Microsoft",
            "/TestCaseType:TextBoxControlTest /CaseNo=645 /CaseOwner=Microsoft",
            "/TestCaseType:TextBoxMaxLengthTest /CaseNo=621 /CaseOwner=Microsoft",
           
            // CaseOwner=Microsoft
        };
        
        /// <summary>
        /// Test entry function to run all the cases.
        /// </summary>
        [TestEntryPoint]
        public void RunTestCase()
        {
            const string Separator = "------------------------------------------------";

            string stdOut = "";
            string stdErr = "";
            const string executable = "EditingTest.exe";
            string argument=null;
            ArrayList caseResults = new ArrayList();
            StringBuilder failureLogs;

            failureLogs = new StringBuilder();
            failureLogs.AppendLine("Log began at " + DateTime.Now);
            caseResults.Add(Separator + "BVT suit failed cases" + Separator);
            for(int i = 0; i< s_testCases.Length; i++)
            {
                try
                {
                    argument = s_testCases[i];
                    Logger.Current.Log("");
                    Logger.Current.Log(Separator + " Start case #" + i.ToString() + " and there are " + (s_testCases.Length - i) + " Left." + Separator);
                    Logger.Current.Log("Command line: " + executable + " " + argument);
                    Logger.Current.Log("Case is runing ... ");
                    //time out after 30 second seems good for all current cases, we will specify this from case switch. 
                    ProcessUtils.RunProcess(executable, argument, 30 * 1000, out stdOut, out stdErr);
                }
                catch (Exception exception)
                {
                    Logger.Current.Log("Test has failed:   Rerun command line: " + executable + " " + argument);
                    caseResults.Add("Test has failed:   Rerun command line: " + executable + " " + argument);
                    failureLogs.AppendLine("Failure log for " + executable + " " + argument);
                    failureLogs.AppendLine("Exception when executing process.");
                    failureLogs.AppendLine(exception.ToString());
                }
                if (!stdOut.Contains("PASS - ") || stdErr != string.Empty)
                {
                    Logger.Current.Log("Test has failed:   Rerun command line: " + executable + " " + argument);
                    caseResults.Add("Test has failed:   Rerun command line: " + executable + " " + argument);
                    failureLogs.AppendLine("Failure log for " + executable + " " + argument);
                    failureLogs.AppendLine("Standard output stream:");
                    failureLogs.AppendLine(stdOut);
                    failureLogs.AppendLine("Standard error stream:");
                    failureLogs.AppendLine(stdErr);
                    failureLogs.AppendLine(Separator + Separator);
                }
                else
                {
                    Logger.Current.Log("Test has passed!   Rerun Command line: " + executable + " " + argument);
                }
            }
            Logger.Current.Log("\r\n\r\n");
            //print out all the failed cases and their rerun command line.
            for (int i = 0; i < caseResults.Count; i++)
            {
                Logger.Current.Log((string)(caseResults[i]));
            }
            if (caseResults.Count > 1)
            {
                Test.Uis.IO.TextFileUtils.SaveToFile(failureLogs.ToString(), "editing-bvt-log.txt");
                Logger.Current.Log("Failure logs available at editing-bvt-log.txt. Please check it for details or just rerun the case(s)!");
            }
        }
    }
}

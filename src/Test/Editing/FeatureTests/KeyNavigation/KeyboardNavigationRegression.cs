// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 20 $ $Source: //depot/vbl_wcp_avalon_dev/windowstest/client/wcptests/uis/Text/BVT/RichText/KeyboardNavigationRegression.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using System.Windows.Controls;
    using System.ComponentModel;
    using System.Windows.Markup;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Collections;
    using System.Windows;
    using System.Threading; using System.Windows.Threading;


    #endregion Namespaces.

    /// <summary>Regresion Test for Keyboard Navigation</summary>
    [TestOwner("Microsoft"), TestBugs("264, 263, 261, 262, 260, 496, 248, 250, 251, 252, 253, 254, 256, 257, 255, 258, 259, 497, 82"), TestTactics("400")]
    public class KeyboardNavigationRegression : RichEditingBase
    {        
        /// <summary>CaretNavigationForParagraph</summary>
        [TestCase(LocalCaseStatus.Ready, CasePriority.p1, "CaretNavigationForParagraph")]
        public void CaretNavigationForParagraph()
        {
            EnterFuction("OtherWaysToSplitAndMerge");
            TestDataArayList = new ArrayList();

            //Regression_Bug81 - Editing: Shift+{Down} will not able to select the end of the document.
            TestDataArayList.Add(new RichEditingData("Regression_Bug248, Regression_Bug81", "", "a{ENTER}{ENTER}b^a^b^{HOME}+{DOWN 5}", "a\r\n\r\nb\r\n", "a\r\n\r\nb\r\n", 3, 3, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug248", "", "Hello world{LEFT 5}+{LEFT 3}^{Right}+{Right}", "Hello world\r\n", "w", 0, 1, true, 0));

            //Regression_Bug82 - "+{END}" is not able to select the line end of the line is not the last listitem.
            //we should fix here when Regression_Bug82 is fixed.
            TestDataArayList.Add(new RichEditingData("Regression_Bug250", "", "ab{ENTER}cd^a^+n^{HOME}{RIGHT}+{END}^x", "1.\ta\r\n2.\tcd\r\n", "", 0, 2, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug251", "", "{ENTER 5}^+{HOME}", RepeatString("\r\n", 6), RepeatString("\r\n", 5), 5, 6, true, 0));
            TestDataArayList.Add(new RichEditingData("Regression_Bug251", "", "{ENTER 5}^+{HOME}^+{HOME}", RepeatString("\r\n", 6), RepeatString("\r\n", 5), 5, 6, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug252", "", "a{Enter}bc de{ENTER}fg hi{ENTER}j^{HOME}{DOWN}+{DOWN 2}^+n{RIGHT}+{UP 5}", "a\r\n1.\tbc de\r\n2.\tfg hi\r\nj\r\n", "a\r\n1.\tbc de\r\n2.\tfg hi\r\n", 3, 4, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug253", "", "a" + RepeatString("^+", 500) + "{DOWN}", "a\r\n", "",  0, 1, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug254 and Regression_Bug255", "", "a{ENTER}b^a^b{DELETE}^z", "a\r\nb\r\n", "a\r\nb\r\n",  2, 2, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug256", "", "ab^a^b{HOME}{RIGHT}{Enter}", "a\r\nb\r\n", "",  0, 2, true, 0));

            //<PageBreak> tag is no long exist, we use the BreakPageBefore property instead.
            TestDataArayList.Add(new RichEditingData("Regression_Bug257", "<Paragraph>ab</Paragraph><Paragraph BreakPageBefore=\"true\">cd</Paragraph>", "^{HOME}{END}{Down}+{LEFT}", "ab\r\ncd\r\n", "d", 0, 2, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug258", "", "a{ENTER}b{ENTER}c{ENTER}d{ENTER}e^a^+n^]^]^]^]^]^]", "1.\ta\r\n2.\tb\r\n3.\tc\r\n4.\td\r\n5.\te\r\n", "1.\ta\r\n2.\tb\r\n3.\tc\r\n4.\td\r\n5.\te\r\n", 5, 5, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug259", "", "one^a^+n{RIGHT}{ENTER}two{ENTER}three", "1.\tone\r\n2.\ttwo\r\n3.\tthree\r\n", "", 0, 3, true, 0));
            TestDataArayList.Add(new RichEditingData("Regression_Bug259", "", "one^a^+n{RIGHT}{ENTER}two{ENTER}three^a{DELETE}", "", "", 0, 0, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug260", "", "abc{left}+{left}^{BackSpace}+{Left}", "ac\r\n", "a", 0, 1, true, 0));
           
            TestDataArayList.Add(new RichEditingData("Regression_Bug261", "", "{Enter 5}{BackSpace}", "\r\n\r\n\r\n\r\n\r\n", "", 0, 5, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug262", "<Paragraph>a<Figure><Paragraph><Button /></Paragraph></Figure>b</Paragraph>", "^a", "a\r\n \r\nb\r\n", "a\r\n \r\nb\r\n", 2, 2, true, 0));

            TestDataArayList.Add(new RichEditingData("Regression_Bug263", "", "one{ENTER}Two^a^b^{HOME}{END}{DELETE}", "oneTwo\r\n", "", 0, 1, true, 0));
            
            TestDataArayList.Add(new RichEditingData("Regression_Bug264", "", "a{Enter}b{ENTER}c^{HOME}+{DOWN 2}", "a\r\nb\r\nc\r\n", "a\r\nb\r\n", 2, 3, true, 0));

            SetInitValue("");

            QueueDelegate(RichEditingDataKeyBoardExecution);

            EndFunction();
        }
    }
}

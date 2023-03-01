// Licensed to the .NET Foundation under one or more agreements. 
 // The .NET Foundation licenses this file to you under the MIT license. 
 // See the LICENSE file in the project root for more information. 

//  Description: Pri1s for annotating elements of type AnchoredBlock (e.g. Figures and Floaters).

using System;
using System.Windows;
using System.Drawing;
using Annotations.Test;
using Annotations.Test.Framework;
using System.Windows.Documents;
using Annotations.Test.Reflection;

namespace Avalon.Test.Annotations.Pri1s
{
	public class AnchoredBlockSuite : AAnchoredBlockSuite
	{
        #region AnchoredBlock Tests

        /// <summary>
        ///  Clear highlight at start of AnchoredBlock.
        /// </summary>
        [TestCase_Helper()]
        protected void highlight1_1()
        {
            CreateAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 0, PagePosition.End, 0));
            DeleteAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 1, PagePosition.Beginning, 25));
            VerifyAnnotation("\r\n" + GetText(AnchoredBlockSelection(PagePosition.Beginning, 25, PagePosition.End, 0)) + "\r\n");
            passTest("Verified clearing highlight in AnchoredBlock.");
        }
        protected void floater_highlight1_1() { highlight1_1(); }
        protected void figure_highlight1_1() { highlight1_1(); }

        /// <summary>
        ///  Clear highlight at end of AnchoredBlock.
        /// </summary>
        [TestCase_Helper()]
        protected void highlight1_2()
        {
            CreateAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 0, PagePosition.End, 0));
            DeleteAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 25, PagePosition.End, 0));
            VerifyAnnotation("\r\n" + GetText(AnchoredBlockSelection(PagePosition.Beginning, 1, PagePosition.Beginning, 25)) + "\r\n");
            passTest("Verified clearing highlight in AnchoredBlock.");
        }
        protected void floater_highlight1_2() { highlight1_2(); }
        protected void figure_highlight1_2() { highlight1_2(); }

        /// <summary>
        ///  Clear highlight at middle of AnchoredBlock.
        /// </summary>
        [TestCase_Helper()]
        protected void highlight1_3()
        {
            CreateAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 0, PagePosition.End, 0));
            DeleteAnnotation(AnchoredBlockSelection(PagePosition.Beginning, 10, PagePosition.End, -10));
            VerifyAnnotation(
                "\r\n" + 
                GetText(AnchoredBlockSelection(PagePosition.Beginning, 1, PagePosition.Beginning, 10)) + 
                GetText(AnchoredBlockSelection(PagePosition.End, 0, PagePosition.End, -10)) + 
                "\r\n");
            passTest("Verified clearing highlight in AnchoredBlock.");
        }
        protected void floater_highlight1_3() { highlight1_3(); }
        protected void figure_highlight1_3() { highlight1_3(); }

        #endregion
    }
}	


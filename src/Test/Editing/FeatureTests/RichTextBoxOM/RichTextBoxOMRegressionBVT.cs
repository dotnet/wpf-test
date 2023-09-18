// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 22 $ $Source: //depot/private/wcp_dev_platform/windowstest/client/wcptests/uis/Text/BVT/RichText/ParagraphEditingTestWithKeyboard.cs $")]

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
    using System.Threading;

    #endregion Namespaces.

    /// <summary></summary>
    [TestOwner("Microsoft"), TestBugs(""), TestTactics("")]
    public class RichTextBoxOMRegressionBVT : RichEditingBase
    {
        #region Regression case - Regression_Bug172: Animation: An animated transfom applied to a Text Element within a TextBox does not update the textBox size.
        /// <summary>Regression_Bug172 Animation: An animated transfom applied to a Text Element within a TextBox does not update the textBox size. This bug is filed for TextBox which is simplified. So we test it in RichTextBox.</summary>
        [TestCase(LocalCaseStatus.Ready, "Regression Test for Regression_Bug172")]
        public void Regression_Bug172()
        {
            EnterFunction("Regression_Bug172");

            Test.Uis.Wrappers.ActionItemWrapper.SetMainXaml("<Canvas xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" Background=\"#FF6666FF\" Height=\"100%\" Width=\"100%\" >" + 
                "<RichTextBox Canvas.Top=\"125\" Wrap=\"True\">" + 
                "<Decorator>" + 
                "<FlowDocumentScrollViewer Canvas.Top=\"350\" Canvas.Left=\"185\"><FlowDocument Foreground=\"Red\" FontSize=\"40\">TransformDecorator text</FlowDocument></FlowDocumentScrollViewer>" + 
                "<Decorator.LayoutTransform>" + 
                "<RotateTransform Angle=\"15\" Center=\"0,0\">" +
                "<RotateTransform.Angle>" + 
                "<DoubleAnimation Begin=\"3\" Restart=\"Always\" Duration=\"2\" To=\"90\" Fill=\"Hold\"/>" + 
                "</RotateTransform.Angle></RotateTransform>" + 
                "</Decorator.LayoutTransform></Decorator></RichTextBox></Canvas>");
            EndFunction();
            QueueDelegate(Regression_Bug172_WaitForAnimationStart);
        }

        void Regression_Bug172_WaitForAnimationStart()
        {
            EnterFunction("Regression_Bug172_WaitForAnimationStart");
            RichTextBox richBox = ((Canvas)MainWindow.Content).Children[0] as RichTextBox;
            double width = richBox.Width;
            double height = richBox.Height;
            if (width > height )
            {
                pass = false;
                MyLogger.Log(CurrentFunction + " - RichTextBox: Width[" + width.ToString() + "] Height[" + height.ToString() + "]");
                MyLogger.Log(CurrentFunction + " - Faild: it seems that the size of richTextBox does not accomodate to the animation, we expected the with is more than double of the height!!!");
            }
            //need to sleep for a while to waiting for the animation to start.
            Sleep(4500);
            QueueDelegate(Regression_Bug172_Done);
            EndFunction();
        }

        void Regression_Bug172_Done()
        {
            EnterFunction("BRegression_Bug172_Done");
            RichTextBox richBox = ((Canvas)MainWindow.Content).Children[0] as RichTextBox;
            double width = richBox.Width;
            double height = richBox.Height;
            if(width < height)
            {
                pass= false;
                MyLogger.Log(CurrentFunction + " - RichTextBox: Width[" + width.ToString() + "] Height[" + height.ToString() + "]");
                MyLogger.Log(CurrentFunction + " - Faild: it seems that the size of richTextBox does not accomodate to the animation, we expect the height is more than 3 times of the width!!!");
            }
            QueueDelegate(EndTest);
            EndFunction();
        }
        #endregion
    }
}

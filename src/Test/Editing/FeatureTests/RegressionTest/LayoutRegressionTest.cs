// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Regression Tests.

namespace Test.Uis.Regressions
{
    #region Namespaces.
    using System;
    using System.Threading; using System.Windows.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Media;
    using System.Windows.Input;
    using System.Windows.Documents;
    using System.Windows.Shapes;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Microsoft.Test.Imaging;
    using Test.Uis.Wrappers;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Verifies that the regression bugs doesnt repro.
    /// Test1: Repro for Regression_Bug124    
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("457"), TestBugs("124")]
    [Test(2, "TextBox", "LayoutRegressions", MethodParameters = "/TestCaseType=LayoutRegressions")]
    public class LayoutRegressions : CustomTestCase
    {
        const string inputXaml1 = "<StackPanel xmlns='http://schemas.microsoft.com/winfx/2006/xaml/presentation' " +
            "xmlns:x='http://schemas.microsoft.com/winfx/2006/xaml' Background='white' Orientation='Horizontal'>" +
            "<DockPanel DockPanel.Dock='Top' Height='100' Background='lightblue'>" +
            "<TextBox Name='TBOne' DockPanel.Dock='Top' Text='Textbox one'/>" +
            "<TextBox Name='TBTwo' DockPanel.Dock='Top' Text='Textbox two - Second text box appears on top of first.'/>" +
            "</DockPanel>" +
            "<DockPanel DockPanel.Dock='Top' Background='lightyellow'>" +
            "<TextBox Name='TBThree' DockPanel.Dock='Top' Text='Textbox three is so tall that the forth textbox doesnt even appear.'/>" +
            "<TextBox Name='TBFour' DockPanel.Dock='Top' Text='Textbox four - this textbox is positioned off screen.'/>" +
            "</DockPanel>" +
            "</StackPanel>";

        UIElement _testUIElement;
        UIElementWrapper _testWrapper1,_testWrapper2,_testWrapper3,_testWrapper4;        

        /// <summary>Runs the test</summary>
        public override void RunTestCase()
        {
            Log("Verify that Regression_Bug124 doesnt repro");
            Test1();
        }

        private void Test1()
        {
            ActionItemWrapper.SetMainXaml(inputXaml1);            
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            _testUIElement = ElementUtils.FindElement(MainWindow, "TBOne") as UIElement;
            _testWrapper1 = new UIElementWrapper(_testUIElement);
            _testUIElement = ElementUtils.FindElement(MainWindow, "TBTwo") as UIElement;
            _testWrapper2 = new UIElementWrapper(_testUIElement);
            _testUIElement = ElementUtils.FindElement(MainWindow, "TBThree") as UIElement;
            _testWrapper3 = new UIElementWrapper(_testUIElement);
            _testUIElement = ElementUtils.FindElement(MainWindow, "TBFour") as UIElement;
            _testWrapper4 = new UIElementWrapper(_testUIElement);
            VerifyTextBoxPositions();            
        }

        private void VerifyTextBoxPositions()
        {
            Rect tb1, tb2, tb3, tb4;
            tb1 = ElementUtils.GetClientRelativeRect(_testWrapper1.Element);
            tb2 = ElementUtils.GetClientRelativeRect(_testWrapper2.Element);
            tb3 = ElementUtils.GetClientRelativeRect(_testWrapper3.Element);
            tb4 = ElementUtils.GetClientRelativeRect(_testWrapper4.Element);

            Log("Verifying positions of the four TextBoxes");

            Verifier.Verify(tb1.BottomLeft.Y <= tb2.TopLeft.Y, 
                "TextBox2 should be below TextBox1", true);
            Verifier.Verify(tb3.BottomLeft.Y <= tb4.TopLeft.Y, 
                "TextBox4 should be below TextBox3", true);

            Verifier.Verify(tb1.BottomLeft.X == tb2.TopLeft.X, 
                "The X Co-ordinate of TextBox1 and TextBox2 should be equal", true);
            Verifier.Verify(tb3.BottomLeft.X == tb4.TopLeft.X, 
                "The X Co-ordinate of TextBox3 and TextBox4 should be equal", true);

            Verifier.Verify(tb2.BottomRight.X <= tb3.TopLeft.X,
                "TextBox2 should be to the left of TextBox3");

            Logger.Current.ReportSuccess();         
        }        
    }
}

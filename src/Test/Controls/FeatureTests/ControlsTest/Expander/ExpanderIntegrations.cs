

//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
// All classes in this file are test cases for integration test of Expander, all of them are related 
// to Expader; i
//---------------------------------------------------------------------------

#region Using directives

using System;
using System.Windows.Controls;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using System.Windows;
using System.Windows.Data;
using System.Collections;
using Avalon.Test.ComponentModel.UnitTests;
using Microsoft.Test.Input;

#endregion

namespace Avalon.Test.ComponentModel.IntegrationTests
{
    /// <summary>
    /// P1 class to test changing datasource for DataBinding ; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderChangeDataSource : IIntegrationTest
    {
        /// <summary>
        /// test changing datasource for DataBinding 
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;
            expander = LogicalTreeHelper.FindLogicalNode(element,"expander") as Expander;
            UIElement tb = LogicalTreeHelper.FindLogicalNode(element,"tb") as UIElement;
            UIElement tb1 = LogicalTreeHelper.FindLogicalNode(element,"tb1") as UIElement;

            //Set binding bwtween Expander and a TextBlock
            Binding bind = new Binding("Text");
            bind.Source = tb;
            expander.SetBinding(Expander.HeaderProperty, bind);

            //Change the Source from one TextBlock to the other
            bind.Source = tb1;
            QueueHelper.WaitTillQueueItemsProcessed();
            if (ControlTestHelper.ExecuteValidations(element, variation["Validations"]))
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderChangeDataSource: Failed in changed datasource !");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test binding content to TextBlock.TextContent ; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderDynamicBindToContent : IIntegrationTest
    {
        /// <summary>
        /// Testing binding content to TextBloxk.TextContent
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;

            //Get two Expanders
            expander = LogicalTreeHelper.FindLogicalNode(element, "expander") as Expander;
            TextBox tb = LogicalTreeHelper.FindLogicalNode(element,"tb") as TextBox;
            
            ControlTestHelper.ExecuteActions(element,variation);
            tb.Text = "---expand content---\r\n\r\n ...\r\n\r\n---end---";
            QueueHelper.WaitTillQueueItemsProcessed();
            if (ControlTestHelper.ExecuteValidations(element,variation["Validations"]))
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderDyanmicalBindToContent: Error databind to TextBloxk.TextContent !");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test binding one expander's ExpandDirection and IsExpanded to another ones'; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderDynamicBindToProperty : IIntegrationTest
    {
        /// <summary>
        /// Test binding one expander's ExpandDirection and IsExpanded to another ones'
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;
            expander = LogicalTreeHelper.FindLogicalNode(element, "expander") as Expander;
            Expander expander1 = LogicalTreeHelper.FindLogicalNode(element, "expander1") as Expander;

            // set binding between two Expanders (bind IsExpander and ExpandDirection of one Expander to the other)
            ControlTestHelper.ExecuteActions(element,variation);

            //Change the status of the source Expander and validate the status of the target one.
            expander.IsExpanded = !expander.IsExpanded;
            if (ControlTestHelper.ExecuteValidations(element, variation["Validations"]))
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderDyanmicalBindToProperty: Error happened in Property Binding between Expanders!");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test accessing Expander in panel by index; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderOperationByIndexInFlowPanel : IIntegrationTest
    {
        /// <summary>
        /// test accessing Expander in panel by index
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            Panel panel = testElement as Panel;
            expander = LogicalTreeHelper.FindLogicalNode(panel, "expander") as Expander;
            Expander expander1 = LogicalTreeHelper.FindLogicalNode(panel, "expander1") as Expander;


            //expander should be the first control and expander1 the second one.
            QueueHelper.WaitTillQueueItemsProcessed();
            if (panel.Children.IndexOf(expander) == 0 && panel.Children.IndexOf(expander1) == 1)
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderOperationByIndexInFlowPanel: Expander can not be accessed by index !");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test changing focus from one Expander to another one; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderChangeFocusToExpander : IIntegrationTest
    {
        /// <summary>
        /// test and validate changing focus from one Expander to another one
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;
            expander = LogicalTreeHelper.FindLogicalNode(element, "expander") as Expander;
            Expander expander1 = LogicalTreeHelper.FindLogicalNode(element, "expander1") as Expander;

            //make the first Expander focused, them enter Tab key to change focus from the first Expander to the second one.
            UserInput.KeyPress("Tab");
            UserInput.KeyPress("Tab");

            //After the second Expnder gettting focus, press Enter key to change its status.
            QueueHelper.WaitTillQueueItemsProcessed();
            if (expander1.IsKeyboardFocusWithin)
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderChangeFocusToExpander: Error Focus behavior between Expanders!");
            return TestResult.Fail;

        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test changing focus from Header to content; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderChangeFocusFromHeaderToContent : IIntegrationTest
    {
        /// <summary>
        /// test and validate changing focus from one Header to content
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;
            expander = LogicalTreeHelper.FindLogicalNode(element, "expander") as Expander;
            Expander expander1 = LogicalTreeHelper.FindLogicalNode(element, "expander1") as Expander;
            FrameworkElement content = expander.Content as FrameworkElement;

            //hl should get focus.
            UserInput.KeyPress("Tab");
            UserInput.KeyPress("Tab");
            QueueHelper.WaitTillQueueItemsProcessed();

            if (content.IsKeyboardFocused)
            {
                return TestResult.Pass;
            }
            GlobalLog.LogStatus("Fail: ExpanderChangeFocusFromHeaderToContent: Error focus behavior !");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }



    /// <summary>
    /// P1 class to test changing focus by Up,Down keys; To run this test case, one parameters should be provided: XTC file name
    /// for example : for example : /ExpanderTest.xtc 
    /// </summary>
    class ExpanderChangeItemsByUpDownKey : IIntegrationTest
    {
        /// <summary>
        /// test and validate changing focus by Up Down keys
        /// </summary>
        /// <param name="testElement"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object testElement, XmlElement variation)
        {
            FrameworkElement element = testElement as FrameworkElement;
            expander = LogicalTreeHelper.FindLogicalNode(element, "expander") as Expander;
            Expander expander1 = LogicalTreeHelper.FindLogicalNode(element, "expander1") as Expander;

            FrameworkElement element1 = LogicalTreeHelper.FindLogicalNode(expander,"control1") as FrameworkElement;
            FrameworkElement element2 = LogicalTreeHelper.FindLogicalNode(expander, "control2") as FrameworkElement;
            //Make the first focusable item in the first Expander's Content focused and change focus by Up, Down keys.

            UserInput.KeyPress("Tab");
            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();
            if (element1.IsKeyboardFocused)
            {
                UserInput.KeyPress("Down");
                QueueHelper.WaitTillQueueItemsProcessed();
                if (element2.IsKeyboardFocused)
                {
                    UserInput.KeyPress("Up");
                    QueueHelper.WaitTillQueueItemsProcessed();
                    if (expander.IsKeyboardFocused || expander.IsKeyboardFocusWithin)
                    {
                        return TestResult.Pass;
                    }
                }
            }
            GlobalLog.LogStatus("Fail: ExpanderChangeItemsByUpDownKey: Error focus behavor by automation");
            return TestResult.Fail;
        }



        //Expander instance variable
        private Expander expander = null;
    }

}

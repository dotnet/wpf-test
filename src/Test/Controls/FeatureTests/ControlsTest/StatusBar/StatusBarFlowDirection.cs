//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Drawing;
using System.Windows;
using System.Windows.Media;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Avalon.Test.ComponentModel.UnitTests;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class is used to test FlowDirection property for Statusbar
    /// Before use it, you should create .xtc file for it.
    /// Run Example: CMLoader.exe StatusBarFlowDirection000001
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarFlowDirection : IUnitTest
    {
        /// <summary>
        /// do test job
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("Test StatusBar FlowDirection Property starting... ");

            StatusBar statusbar;
            StatusBarItem item1, item2, item3;

            statusbar = (StatusBar)obj;

            item1 = new StatusBarItem();
            item1.Content = "ItemOne";
            statusbar.Items.Add(item1);

            item2 = new StatusBarItem();
            item2.Content = "ItemTwo";
            statusbar.Items.Add(item2);

            item3 = new StatusBarItem();
            item3.Content = "ItemThree";
            statusbar.Items.Add(item3);
            QueueHelper.WaitTillQueueItemsProcessed();

            //get each item's rect
            Rectangle rect1 = ImageUtility.GetScreenBoundingRectangle(item1);
            Rectangle rect2 = ImageUtility.GetScreenBoundingRectangle(item2);
            Rectangle rect3 = ImageUtility.GetScreenBoundingRectangle(item3);


            TestResult testResult = TestResult.Fail;
            //the test doss not cover TopToBottom or BottomToTop 
            switch (statusbar.FlowDirection)
            {
                case FlowDirection.LeftToRight:
                    {
                        if (rect1.X < rect2.X && rect2.X < rect3.X)
                        {
                            testResult = TestResult.Pass;
                        }
                        else
                        {
                            GlobalLog.LogStatus("Fail:Disorder of items");
                            testResult = TestResult.Fail;
                        }
                        break;
                    }
                
                case FlowDirection.RightToLeft:
                    {
                        if (rect1.X > rect2.X && rect2.X > rect3.X)
                        {
                            testResult = TestResult.Pass;
                        }
                        else
                        {
                            GlobalLog.LogStatus("Fail:Disorder of items");
                            testResult = TestResult.Fail;
                        }
                        break;
                    }


                default:
                    GlobalLog.LogStatus("Impossible value for FlowDirection");      
                    testResult = TestResult.Fail;
                    break;
            }

            //Restore
            statusbar.Items.Remove(item1);
            statusbar.Items.Remove(item2);
            statusbar.Items.Remove(item3);

            return testResult;
        }
    }
}

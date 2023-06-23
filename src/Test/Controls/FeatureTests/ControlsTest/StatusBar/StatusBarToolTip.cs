//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Windows;

using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;


namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// This class is used to test ToolTip for StatusBar
    /// Before using it, you should create .xtc file for it using GTO
    /// Run example: CMLoader.exe StatusBarToolTip000001.xtc
    /// Note: it depends on ToolTip implement, especially the length 
    /// of time the pointer must remain stationary within a Control region
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarToolTip : IUnitTest
    {
        /// <summary>
        /// do test job
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("StatusBar ToolTip test starting... ");
            
            StatusBar statusbar;
            statusbar = (StatusBar)obj;

            //create tooltip for statusbar
            ToolTip oldTip = null;
            ToolTip tp = new ToolTip();
            tp.Content = "StatusBar";

            if (statusbar.ToolTip != null)
            {
                oldTip = statusbar.ToolTip as ToolTip;
            }

            statusbar.ToolTip = tp;
            QueueHelper.WaitTillQueueItemsProcessed();

            //move mouse on it, wait to popup tooltip window
            GlobalLog.LogStatus("Move mouse into StatusBar");
            //get statusbar's center point coordinate
            int centerx = (int)(statusbar.ActualWidth / 2);
            int centery = (int)(statusbar.ActualHeight / 2);
            UserInput.MouseMove(statusbar, centerx, centery);
            
            //Get the InitialShowDelay, setting
            //a longer time than this is to make sure the ToolTip window can be popup.
            int waitMilliseconds = ToolTipService.GetInitialShowDelay(statusbar) + 200;
            QueueHelper.WaitTillTimeout(new TimeSpan(waitMilliseconds * 10000));

            TestResult testResult;
            //if statusbar.IsEnabled == true, so the tooltip should popup
            //then tp.IsOpen should be true, reverse is ok.
            if (statusbar.IsEnabled == tp.IsOpen)
            {
                testResult = TestResult.Pass;
            }
            else
            {
                GlobalLog.LogStatus("StatusBar' ToolTip fails because the statusbar's tooltip state" 
                        + "\n\rshould change according to StatusBar's IsEnabled property,"
                        + "\n\ralso maybe the mouse pointer remains stationary too short");
                testResult = TestResult.Fail;
            }

            //Restore
            statusbar.ToolTip = oldTip;
            
            return testResult;
        }
    }
}

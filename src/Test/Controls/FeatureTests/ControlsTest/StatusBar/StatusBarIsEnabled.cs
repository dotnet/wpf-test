//---------------------------------------------------------------------------
//
// Copyright (C) Microsoft Corporation.  All rights reserved.
//
//---------------------------------------------------------------------------

using System;
using System.Xml;
using System.Drawing;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Input;



namespace Avalon.Test.ComponentModel.UnitTests
{
    /// <summary>
    /// class StatusBarIsEnabled is used to test Statusbar's
    /// IsEnabled property.
    /// before use it, you should create .xtc file for it
    /// Run example: CMLoader.exe StatusBarIsEnabled00001.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarIsEnabled : IUnitTest
    {
        /// <summary>
        /// Do test
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            GlobalLog.LogStatus("StatusBar IsEnbaled property test starting... ");
            
            StatusBar statusbar;
            statusbar = (StatusBar)obj;
            
            //add a button item into StatusBar
            Button btn;
            btn = new Button();
            btn.Content = "Button";
            btn.Click += new System.Windows.RoutedEventHandler(btn_Click);
            statusbar.Items.Add(btn);
            QueueHelper.WaitTillQueueItemsProcessed();

            //Press enter key to fire button Click event
            btn.Focus();
            UserInput.KeyPress("Enter");
            QueueHelper.WaitTillQueueItemsProcessed();

            GlobalLog.LogStatus(statusbar.IsEnabled.ToString() + " " + m_bIsClicked.ToString());

            //Restore State
            statusbar.Items.Remove(btn);

            //if IsEnabled == true, then Button Click event could be fired
            //so m_bIsClicked would be set to true
            //if IsEnbled false, m_bIsClicked should be false
            if (statusbar.IsEnabled == m_bIsClicked)
            {
                return TestResult.Pass;
            }

            GlobalLog.LogStatus("Error State: Button Clicked dismatch the statusbar's state");
            return TestResult.Fail;
        }


        /// <summary>
        /// Button Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void btn_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            m_bIsClicked = true;
        }


        private bool m_bIsClicked = false;
    }
}

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
    /// This class is used to test ContextMenu for StatusBar
    /// when you use it, you should create .xtc file for it first.
    /// Run Example: CMLoader.exe StatusBarContextMenu000001.xtc
    /// </summary>
    [TargetType(typeof(StatusBar))]
    class StatusBarContextMenu : IUnitTest
    {
        /// <summary>
        /// do test job
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="variation"></param>
        /// <returns></returns>
        public TestResult Perform(object obj, XmlElement variation)
        {
            //initialize
            GlobalLog.LogStatus("StatusBar ContextMenu test starting... ");
            StatusBar statusbar;
            statusbar = (StatusBar)obj;
        
            //Add ContextMenu for StatusBar
            ContextMenu oldMenu = null;
            ContextMenu menu;
            menu = new ContextMenu();

            MenuItem item = new MenuItem();
            item.Header = "ContextMenu";
            item.Click += new RoutedEventHandler(menuitem_Click);
            menu.Items.Add(item);

            if (statusbar.ContextMenu != null)
            {
                oldMenu = statusbar.ContextMenu;
            }

            statusbar.ContextMenu = menu;
            QueueHelper.WaitTillQueueItemsProcessed();

            //Click Mouse rightbutton to fire ContextMenu
            UserInput.MouseRightClickCenter(statusbar);
            QueueHelper.WaitTillQueueItemsProcessed();
            // ContextMenu takes time to popup, so need to wait a second.
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));

            GlobalLog.LogStatus("StatusBar::IsEnabled = " + statusbar.IsEnabled);
            GlobalLog.LogStatus("ContextMenu::IsOpen = " + menu.IsOpen);

            TestResult testResult;
            
            if (statusbar.IsEnabled == false)
            {
                if (menu.IsOpen == false)
                {
                    testResult = TestResult.Pass;
                }
                else
                {
                    GlobalLog.LogStatus("Error State:StatusBar is disabled, so the ContextMenu should not be opened");
                    testResult = TestResult.Fail;
                }
            }
            else
            {
                if (menu.IsOpen == false)
                {
                    GlobalLog.LogStatus("Error State:StatusBar is enabled, so the ContextMenu should be opened");
                    testResult = TestResult.Fail;
                }
                else
                {
                    //click context menu item to fire click event
                    UserInput.MouseLeftClickCenter(item);
                    QueueHelper.WaitTillQueueItemsProcessed();

                    if (m_bClicked == true)
                    {
                        testResult = TestResult.Pass;
                    }
                    else
                    {
                        GlobalLog.LogStatus("Error State:The munuitem should be clicked.");
                        testResult = TestResult.Fail;
                    }
                }
            }

            //Restore state
            statusbar.ContextMenu = oldMenu;
           
            return testResult;
        }


        /// <summary>
        /// ContextMenu item Click event handler
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void menuitem_Click(object sender, RoutedEventArgs e)
        {
            GlobalLog.LogStatus("ContextMenu Item is clicked");
            m_bClicked = true;
        }

        private bool m_bClicked = false;
    }
}

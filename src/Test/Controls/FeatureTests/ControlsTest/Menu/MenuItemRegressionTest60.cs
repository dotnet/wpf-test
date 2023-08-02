using System;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;

namespace Avalon.Test.ComponentModel.UnitTests
{
    public class MenuItemRegressionTest60 : IUnitTest
    {
        public MenuItemRegressionTest60()
        {
        }

        /// <summary>
        /// IsSharedSizeScope on MenuItem does not work for the MenuItem.
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Menu menu = (Menu)obj;
            MenuItem menuHeader = menu.Items[0] as MenuItem;
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyPress("LeftAlt");
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();

            ColumnDefinition firstMenuItemColumnDefinition = FindMenuItemColumnDefinition((MenuItem)menuHeader.Items[0]);

            if (firstMenuItemColumnDefinition != null)
            {
                if (menuHeader.Items.Count > 0)
                {
                    for (int i = 1; i < menuHeader.Items.Count; i++)
                    {
                        if (menuHeader.Items[i] is Separator)
                        {
                            TestLog.Current.LogEvidence("Items[" + i.ToString() + "] is Separator.");
                        }
                        else
                        {
                            ColumnDefinition columnDefinition = FindMenuItemColumnDefinition((MenuItem)menuHeader.Items[i]);
                            if (!DoubleUtil.AreClose(firstMenuItemColumnDefinition.ActualWidth, columnDefinition.ActualWidth))
                            {
                                TestLog.Current.LogDebug("Fail: MenuItem " + i.ToString() + " ColumnDefinition actual width is not equal to the first MenuItem ColumnDefinition actual width.");
                                return TestResult.Fail;
                            }
                        }
                    }
                }
            }
            else
            {
                TestLog.Current.LogDebug("Fail: the first MenuItem ColumnDefinition is null.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private ColumnDefinition FindMenuItemColumnDefinition(MenuItem menuItem)
        {
            // In the theme visual style, two Grids for MenuItem is the max.
            // For Luna theme, we are looking for the first Grid because we use only one Grid to position MenuItem content.
            // For Aero theme, we are looking for the second Grid because we use two Grids, and we use the inner one to position MenuItem content.
            for (int i = 0; i < 2; i++)
            {
                Grid grid = VisualTreeUtils.FindPartByType(menuItem, typeof(Grid), i) as Grid; 
                if (grid != null && grid.ColumnDefinitions.Count > 1)
                {
                    return grid.ColumnDefinitions[0];
                }
            }
            return null;
        }
    }

}



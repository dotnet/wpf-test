using System;
using System.Windows;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;

namespace Avalon.Test.ComponentModel.UnitTests
{
    public class MenuItemRegressionTest61 : IUnitTest
    {
        public MenuItemRegressionTest61()
        {
        }

        /// <summary>
        /// Menu popup final rect should not resize when submenu opens.
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

            MenuItem menuItem = menuHeader.Items[0] as MenuItem;
            UIElement popupRoot = VisualTreeUtils.FindRootVisual(menuItem);
            Rect beforeOpenSubmenuFinalPopupRootRect = VisualTreeUtils.GetPreviousArrangeRect(popupRoot);

            MenuItem openSubmenu = menuItem.FindName("openSubmenu") as MenuItem;
            UserInput.MouseLeftClickCenter(openSubmenu);
            QueueHelper.WaitTillQueueItemsProcessed();

            Rect afterOpenSubmenuFinalPopupRootRect = VisualTreeUtils.GetPreviousArrangeRect(popupRoot);

            // Compare before and after open submenu popupRoot final rect width.
            if (DoubleUtil.AreClose(beforeOpenSubmenuFinalPopupRootRect.Width, afterOpenSubmenuFinalPopupRootRect.Width))
            {
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogDebug("Fail: before open submenu final rect width:" + beforeOpenSubmenuFinalPopupRootRect.Width.ToString() + " is not equal to after open submenu final rect width:" + afterOpenSubmenuFinalPopupRootRect.Width.ToString() + ".");
                return TestResult.Fail;
            }
        }

    }
}



using System;
using System.Windows.Controls;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;
using System.Windows.Media;
using System.Windows.Controls.Primitives;
using Microsoft.Test.RenderingVerification;
using System.Windows;

namespace Avalon.Test.ComponentModel.UnitTests
{
    public class ToolBarRegressionTest69 : IUnitTest
    {
        /// <summary>
        /// ToolBarTray drop down view gets clipped when it's partially covered.
        /// Customer repro markup:
        /// 1, A ToolBarTray has two ToolBars.
        /// Customer repro steps:
        /// 1, Move the second toolbar toward to left until "Help" button on the first toolbar disappears.
        /// 2, Mouse click on the first toolbar togglebutton to open the toolbar popup.
        /// 3, Validate the popup has enough room for it's content "Help" button.
        /// 4, Move the second toolbar toward to left until it covers "Format" button the first toolbar.
        /// 5, Mouse click on the first toolbar togglebutton to open the toolbar popup.
        /// 6, Validate the popup has enough room for it's content "Help" button plus "Format" button.
        /// </summary>
        /// <param name="toolBar"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            ToolBarTray toolBarTray = (ToolBarTray)obj;
            Button firstToolBarFormatButton = toolBarTray.FindName("firstToolBarFormatButton") as Button;
            Button firstToolBarHelpButton = toolBarTray.FindName("firstToolBarHelpButton") as Button;
            ToolBar firstToolBar = toolBarTray.FindName("firstToolBar") as ToolBar;
            ToolBar secondToolBar = toolBarTray.FindName("secondToolBar") as ToolBar;

            System.Drawing.Rectangle secondToolBarRect = ImageUtility.GetScreenBoundingRectangle(secondToolBar);
            ToggleButton firstToolBarToggleButton = VisualTreeUtils.FindPartByType(firstToolBar, typeof(ToggleButton), 0) as ToggleButton;
            Thumb secondToolBarThumb = VisualTreeUtils.FindPartByType(secondToolBar, typeof(Thumb), 0) as Thumb;
            QueueHelper.WaitTillQueueItemsProcessed();

            // Move the second toolbar toward to left until the last "Help" button on the first toolbar disappears.
            // Click on the first toolbar togglebutton to open the toolbar popup.
            int xCoordinate = secondToolBarRect.Left - Convert.ToInt32(firstToolBarHelpButton.ActualWidth / 2);
            int yCoordinate = (secondToolBarRect.Top + secondToolBarRect.Bottom) / 2;
            OpenToolBarPopup(firstToolBarToggleButton, secondToolBarThumb, xCoordinate, yCoordinate);

            if (!ValidateFinalPopupRootRect(firstToolBarHelpButton))
            {
                TestLog.Current.LogDebug("The first validation fail: the final toolbar popupRoot rect does not have enough room for popupPanel has the last button.");
                return TestResult.Fail;
            }

            // Click to close the first toolbar popup before we move the second toolbar.
            UserInput.MouseLeftClickCenter(firstToolBarToggleButton);
            QueueHelper.WaitTillQueueItemsProcessed();

            // Move the second toolbar toward to left until it covers the second last "Format" button on the first toolbar.
            // Click on the first toolbar togglebutton to open the toolbar popup.
            xCoordinate = secondToolBarRect.Left - Convert.ToInt32(firstToolBarFormatButton.ActualWidth + firstToolBarHelpButton.ActualWidth / 2);
            OpenToolBarPopup(firstToolBarToggleButton, secondToolBarThumb, xCoordinate, yCoordinate);

            if(!ValidateFinalPopupRootRect(firstToolBarHelpButton))
            {
                TestLog.Current.LogDebug("The second validation fail: the final toolbar popupRoot rect does not have enough room for popupPanel has the last two buttons.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        private void OpenToolBarPopup(ToggleButton firstToolBarToggleButton, Thumb secondToolBarThumb, int xCoordinate, int yCoordinate)
        {
            UserInput.MouseLeftDown(secondToolBarThumb);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseMove(xCoordinate, yCoordinate);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftUp(secondToolBarThumb);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseLeftClickCenter(firstToolBarToggleButton);
            QueueHelper.WaitTillQueueItemsProcessed();
        }

        private bool ValidateFinalPopupRootRect(Button firstToolBarHelpButton)
        {
            UIElement popupRoot = VisualTreeUtils.FindRootVisual(firstToolBarHelpButton);
            Rect finalPopupRootRect = VisualTreeUtils.GetPreviousArrangeRect(popupRoot);
            Panel popupPanel = System.Windows.Media.VisualTreeHelper.GetParent(firstToolBarHelpButton) as Panel;
            if (DoubleUtil.LessThan(finalPopupRootRect.Width, popupPanel.ActualWidth))
            {
                TestLog.Current.LogDebug("Fail: the final toolbar popupRoot rect width is less than popupPanel actual width.");
                TestLog.Current.LogDebug("Final toolbar popupRoot rect width is " + finalPopupRootRect.Width.ToString());
                TestLog.Current.LogDebug("PopupPanel actual width is " + popupPanel.ActualWidth.ToString());
                return false;
            }
            return true;
        }
    }
}



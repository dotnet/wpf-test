using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;
using Microsoft.Test.Layout;

namespace Avalon.Test.ComponentModel.UnitTests
{
    public class PopupRegressionTest64 : IUnitTest
    {
        public PopupRegressionTest64()
        {
        }

        /// <summary>
        /// Expression Blend Asset Library popup does not render correctly when resizing.
        /// Repro markup:
        /// 1, Put two grids inside of stackpanel that is the child of popup.
        /// 2, Set Grid.IsSharedSizeScope="true" on stackpanel to enable sharing size for column definition for Grids inside of the stackpanel.
        /// 3, Set the first ColumnDefinition SharedSizeGroup="First", the second ColumnDefinition SharedSizeGroup="Second" for each Grid.
        /// 4, Put more content on the second column of the second grid.
        /// 5, In test code, validate the final PopupRoot rect width size to popup panel width.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Panel popupPanel = panel.FindName("popupPanel") as Panel;
            CheckBox checkbox = panel.FindName("checkbox") as CheckBox;

            UserInput.MouseLeftClickCenter(checkbox);
            QueueHelper.WaitTillQueueItemsProcessed();

            // Get the popupRoot final rect.
            UIElement popupRoot = VisualTreeUtils.FindRootVisual(popupPanel);
            Rect finalPopupRootRect = VisualTreeUtils.GetPreviousArrangeRect(popupRoot);

            // Compare popupRoot final rect width with popup panel width.
            if (DoubleUtil.AreClose(finalPopupRootRect.Width, popupPanel.ActualWidth))
            {
                return TestResult.Pass;
            }
            else
            {
                TestLog.Current.LogDebug("Fail: popupRoot final rect width:" + finalPopupRootRect.Width.ToString() + " is not equal to popup panel width:" + popupPanel.ActualWidth.ToString() + ".");
                return TestResult.Fail;
            }
        }
    }
 
}



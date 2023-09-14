using System;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{
	/// <summary>
	/// Dump the entire Applications Visual Tree.
	/// </summary>
    public class WindowPositionResizeAction : IAction
    {
        /// <summary>
        /// Positions and resizes the main window.
        /// The XTC will look something like this:
        /// <Action Name="WindowPositionResizeAction">
        /// 	<Parameter Value="Always" />
        /// </Action>
        /// </summary>
        /// <param name="frmElement"></param>
        /// <param name="actionParams"></param>
        public void Do(System.Windows.FrameworkElement frmElement, params object[] actionParams)
        {
            double top = 0;
            double left = 0;
            double width = 800;
            double height = 480;

            if (actionParams.Length > 1)
            {
                top = Convert.ToDouble(actionParams[0], System.Globalization.CultureInfo.InvariantCulture);
                left = Convert.ToDouble(actionParams[1], System.Globalization.CultureInfo.InvariantCulture);
            }

            if (actionParams.Length > 3)
            {
                width = Convert.ToDouble(actionParams[2], System.Globalization.CultureInfo.InvariantCulture);
                height = Convert.ToDouble(actionParams[3], System.Globalization.CultureInfo.InvariantCulture);
            }
            TestLog.Current.LogStatus("top: [" + top.ToString() + "]");
            TestLog.Current.LogStatus("left: [" + left.ToString() + "]");
            TestLog.Current.LogStatus("width: [" + width.ToString() + "]");
            TestLog.Current.LogStatus("height: [" + height.ToString() + "]");

            System.Windows.Window window = WindowPositionResizeAction.GetParentWindow(frmElement);
            //System.Windows.Window window = System.Windows.Application.Current.MainWindow;

            if (window != null)
            {
                PositionResizeWindow(window, top, left, width, height);
            }
        }

        private void PositionResizeWindow(System.Windows.Window window, double top, double left, double width, double height)
        {
            window.Top = top;
            window.Left = left;
            window.Width = width;
            window.Height = height;
        }

        public static System.Windows.Window GetParentWindow(System.Windows.FrameworkElement frmElement)
        {
            System.Windows.Navigation.NavigationWindow navwin = new System.Windows.Navigation.NavigationWindow();
            
            System.Windows.Window window = null;
            if (frmElement == null)
                return window;
            if ((frmElement.GetType() == typeof(System.Windows.Window)) || (frmElement.GetType() == typeof(System.Windows.Navigation.NavigationWindow)))
            {
                window = (System.Windows.Window)frmElement;
                if (window != null)
                    return window;
            }
            return GetParentWindow((System.Windows.FrameworkElement)frmElement.Parent);
        }
    }

}

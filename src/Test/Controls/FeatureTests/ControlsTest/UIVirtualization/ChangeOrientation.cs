using System;
using System.Windows;
using System.Reflection;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Controls.Primitives;
using System.Windows.Media;
using System.Windows.Markup;
using System.Threading;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Avalon.Test.ComponentModel.Actions
{

    public class ChangeOrientation : IAction
    {
        /// <summary>
        /// Change the Orientation of the ListBox's VirtualizingStackPanel
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {

            if (actionParams.Length != 1)
            {
                throw new ArgumentException("ChangeOrientation action expects exactly one argument");
            }

            PropertyInfo info = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);
            VirtualizingStackPanel panel = (Panel)info.GetValue(frmElement, null) as VirtualizingStackPanel;

            if (actionParams[0].ToString() == "Vertical")
            {
                panel.Orientation = Orientation.Vertical;
            }
            else
            {
                if (actionParams[0].ToString() == "Horizontal")
                {
                    panel.Orientation = Orientation.Horizontal;
                }
                else
                {
                    throw new ArgumentException("Invalid value : " + actionParams[0].ToString() + " for  Orientation");
                }
            }


            QueueHelper.WaitTillQueueItemsProcessed();
        }

    }
}

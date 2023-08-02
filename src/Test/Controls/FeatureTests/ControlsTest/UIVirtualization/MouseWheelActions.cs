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
using System.Windows.Data;
using Avalon.Test.ComponentModel.DataSources;
using System.Collections.ObjectModel;

namespace Avalon.Test.ComponentModel.Actions
{

    public class MouseWheelRightAction : IAction
    {
        /// <summary>
        /// MouseWheel right
        /// Used to scroll a ListBox with ItemsPanelTemplate as VirtualizingStackPanel with Orientation Horizontal
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (frmElement == null)
            {
                throw new ArgumentException("frmElement null");
            }

            PropertyInfo info = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);
            VirtualizingStackPanel panel = (Panel)info.GetValue(frmElement, null) as VirtualizingStackPanel;

            panel.MouseWheelRight();

            QueueHelper.WaitTillQueueItemsProcessed();
        }

    }

    public class MouseWheelLeftAction : IAction
    {
        /// <summary>
        /// MouseWheel left
        /// Used to scroll a ListBox with ItemsPanelTemplate as VirtualizingStackPanel with Orientation Horizontal
        /// <param name="frmElement">Control to act upon.</param>
        public void Do(FrameworkElement frmElement, params object[] actionParams)
        {
            if (frmElement == null)
            {
                throw new ArgumentException("frmElement null");
            }

            PropertyInfo info = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);
            VirtualizingStackPanel panel = (Panel)info.GetValue(frmElement, null) as VirtualizingStackPanel;

            panel.MouseWheelLeft();

            QueueHelper.WaitTillQueueItemsProcessed();
        }

    }
}

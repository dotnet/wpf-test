using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Utilities
{
    /// <summary>
    /// ContextMenu Helper.
    /// Encapsulates ContextMenu helper methods.
    /// </summary>
    public static class ContextMenuHelper
    {
        public static void WaitForContextMenuOpening(FrameworkElement frameworkElement)
        {
            DispatcherFrame frame = new DispatcherFrame();
            frameworkElement.ContextMenuOpening += delegate(Object s, ContextMenuEventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }
        public static void WaitForContextMenuClosing(FrameworkElement frameworkElement)
        {
            DispatcherFrame frame = new DispatcherFrame();
            frameworkElement.ContextMenuClosing += delegate(Object s, ContextMenuEventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }
    }
}



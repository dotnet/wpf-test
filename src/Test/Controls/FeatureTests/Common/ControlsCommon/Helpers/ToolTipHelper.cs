using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.Reflection;

namespace Avalon.Test.ComponentModel.Utilities
{
    public static class ToolTipHelper
    {
        public static ToolTip FindToolTip()
        {
            Assembly assembly = Assembly.GetAssembly(typeof(FrameworkElement));
            Type type = assembly.GetType("System.Windows.Controls.PopupControlService");
            PropertyInfo popupControlServiceProperty = typeof(FrameworkElement).GetProperty("PopupControlService", BindingFlags.Static | BindingFlags.NonPublic);
            object foo = popupControlServiceProperty.GetValue(null, null);
            FieldInfo fieldInfo = type.GetField("_currentToolTip", BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfo.GetValue(foo) as ToolTip;
        }

        public static void WaitForToolTipOpening(FrameworkElement frameworkElement)
        {
            DispatcherFrame frame = new DispatcherFrame();
            frameworkElement.ToolTipOpening += delegate(Object s, ToolTipEventArgs e){ frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

        public static void WaitForToolTipClosing(FrameworkElement frameworkElement)
        {
            DispatcherFrame frame = new DispatcherFrame();
            frameworkElement.ToolTipClosing += delegate(Object s, ToolTipEventArgs e){ frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }
    }
}



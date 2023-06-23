using System;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;

namespace Microsoft.Test.Controls.Helpers
{
    public static class ComboBoxHelper
    {
        public static ToggleButton FindDropDownToggleButton(ComboBox combobox)
        {
            ToggleButton[] toggleButtons = (ToggleButton[])VisualTreeUtils.FindPartByType(combobox, typeof(ToggleButton)).ToArray(typeof(ToggleButton));

            for (int i = 0; i < toggleButtons.Length; i++)
            {
                if (toggleButtons[i].TemplatedParent == combobox)
                {
                    return toggleButtons[i];
                }
            }

            return null;
        }

        public static void WaitForDropDownOpened(ComboBox combobox)
        {
            DispatcherFrame frame = new DispatcherFrame();
            combobox.DropDownOpened += delegate(Object s, EventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

        public static void WaitForDropDownClosed(ComboBox combobox)
        {
            DispatcherFrame frame = new DispatcherFrame();
            combobox.DropDownClosed += delegate(Object s, EventArgs e) { frame.Continue = false; };
            Dispatcher.PushFrame(frame);
        }

    }
}



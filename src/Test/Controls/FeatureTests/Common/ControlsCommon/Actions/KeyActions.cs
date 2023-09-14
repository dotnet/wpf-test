using System;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;
using Microsoft.Test.Threading;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Actions
{
    public static class KeyActions
    {
        public static void PressKeyWithFocus(ButtonBase buttonBase, System.Windows.Input.Key key)
        {
            buttonBase.Focus();
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            UserInput.KeyPress(key.ToString());
            DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
        }
    }
}



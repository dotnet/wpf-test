using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;

using Microsoft.Test;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.Threading;

using WpfControlToolkit;

namespace Avalon.Test.ComponentModel.Actions
{
    public enum InfoTextBoxTestState
    {
        UT, // Unselected, HasText
        ST, // Selected< HasText
        UF, // Unselected, No Text
        SF, // Selected, No Text
    }

    public static class InfoTextBoxTestAction
    {
        public static bool ActionSetFocusTo(FrameworkElement TargetElement)
        {
            if (TargetElement == null)
            {
                throw new ArgumentNullException("TargetElement",
                    "Trouble with XTC file - \"TargetElement\" must be an instantiated InfoTextBox control.");
            }
            TargetElement.Focus();
            return true;
        }

        public static bool ActionClearText(InfoTextBox TargetElement)
        {
            if (TargetElement == null)
            {
                throw new ArgumentNullException("TargetElement",
                    "Trouble with XTC file - \"TargetElement\" must be an instantiated InfoTextBox control.");
            }
            TargetElement.Text = string.Empty;
            return true;
        }

        public static bool ActionAddText(InfoTextBox TargetElement, TextBox TextArg)
        {
            if (TargetElement == null)
            {
                throw new ArgumentNullException("TargetElement",
                    "Trouble with XTC file - \"TargetElement\" must be an instantiated InfoTextBox control.");
            }
            if (TextArg.Text == string.Empty)
            {
                throw new ArgumentException("Trouble with XTC file - \"TextArg\" TextBox must have non-empty text.", "TextArg");
            }
            TargetElement.Text = TextArg.Text;
            return true;
        }
    }
}

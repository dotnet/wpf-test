using System;
using System.Windows;
using System.Windows.Controls;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using System.Reflection;
using System.Windows.Controls.Primitives;
using Microsoft.Test.Input;
using System.Windows.Input;
using System.Collections.Generic;
using Microsoft.Test;
using Microsoft.Test.Threading;
using System.Windows.Threading;

namespace Avalon.Test.ComponentModel.Actions
{
    /// <summary>
    /// Encapsulates MenuBase controls actions.
    /// </summary>
    public static class ContextMenuActions
    {
        /// <summary>
        /// Test Exceptions
        /// </summary>
        /// <param name="contextMenu"></param>
        public static void TestExceptions(ContextMenu contextMenu)
        {
            string addContextMenuToPanelExceptionMessage = "'ContextMenu' cannot have a logical or visual parent.";
            ExceptionHelper.ExpectException(delegate()
            {
                StackPanel panel = new StackPanel();
                panel.Children.Add(contextMenu);
            }, new InvalidOperationException(addContextMenuToPanelExceptionMessage, new Exception()));
        }
    }
}



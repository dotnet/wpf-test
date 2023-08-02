using System;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using System.Windows;
using System.Collections.Generic;
using Avalon.Test.ComponentModel.Actions;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Test.Input;
using Microsoft.Test.Threading;
using System.Windows.Threading;
using Avalon.Test.ComponentModel.Utilities;

namespace Avalon.Test.ComponentModel
{
    /// <summary>
    /// Encapsulates MenuBase controls Closed event behavior test algorithm.
    /// </summary>
    public class ClosedEventTest : IMenuBaseTest
    {
        /// <summary>
        /// Run Closed event tests.
        /// </summary>
        /// <param name="menubase"></param>
        public void Run(MenuBase menubase)
        {
            if (menubase is ContextMenu)
            {
                // setup
                string eventName = "Closed";
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ContextMenu.ClosedEvent);
                routedEventArgs.Source = menubase;
                ((ContextMenu)menubase).IsOpen = true;
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();

                EventTriggerCallback pressEscapeCallback = delegate()
                {
                    UserInput.KeyPress(System.Windows.Input.Key.Escape.ToString());
                    QueueHelper.WaitTillTimeout(TimeSpan.FromSeconds(1));
                };
                eventTriggerCallbacks.Add(pressEscapeCallback);

                // run tests
                foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
                {
                    EventHelper.ExpectEvent<RoutedEventArgs>(eventTriggerCallback, menubase, eventName, routedEventArgs);
                }
            }
        }
    }
}

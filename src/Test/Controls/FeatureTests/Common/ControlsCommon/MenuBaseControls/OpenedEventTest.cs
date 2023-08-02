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
    /// Encapsulates MenuBase controls Opened event behavior test algorithm.
    /// </summary>
    public class OpenedEventTest : IMenuBaseTest
    {
        /// <summary>
        /// Run Opened event tests.
        /// </summary>
        /// <param name="menubase"></param>
        public void Run(MenuBase menubase)
        {
            if (menubase is ContextMenu)
            {
                string eventName = "Opened";
                Control placementTarget = new Control();
                placementTarget = ((ContextMenu)menubase).PlacementTarget as Control;
                if (placementTarget == null)
                {
                    throw new TestValidationException("PlacementTarget is null.");
                }
                placementTarget.Focus();
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                ((ContextMenu)menubase).IsOpen = false;
                QueueHelper.WaitTillTimeout(TimeSpan.FromSeconds(1));

                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ContextMenu.OpenedEvent);
                routedEventArgs.Source = menubase;
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();

                EventTriggerCallback ctrlShiftF10Callback = delegate()
                {
                    UserInput.KeyDown(System.Windows.Input.Key.LeftShift.ToString());
                    UserInput.KeyPress(System.Windows.Input.Key.F10.ToString());
                    UserInput.KeyUp(System.Windows.Input.Key.LeftShift.ToString());
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                };
                eventTriggerCallbacks.Add(ctrlShiftF10Callback);

                EventTriggerCallback mouseRightClickCallback = delegate()
                {
                    UserInput.MouseRightClickCenter(placementTarget);
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                };
                eventTriggerCallbacks.Add(mouseRightClickCallback);

                foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
                {
                    EventHelper.ExpectEvent<RoutedEventArgs>(eventTriggerCallback, menubase, eventName, routedEventArgs);
                    UserInput.KeyPress(System.Windows.Input.Key.Escape.ToString());
                    DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                }
            }
        }
    }
}

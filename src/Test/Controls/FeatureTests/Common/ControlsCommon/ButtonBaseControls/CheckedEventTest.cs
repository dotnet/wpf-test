using System;
using System.Windows.Controls.Primitives;
using Microsoft.Test;
using System.Windows;
using System.Collections.Generic;
using Avalon.Test.ComponentModel.Actions;
using System.Windows.Input;
using System.Windows.Controls;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel
{
    class CheckedEventTest : ToggleButtonValidation, IButtonBaseTest
    {
        public void Run(ButtonBase buttonBase)
        {
            if (buttonBase is ToggleButton)
            {
                initialIsCheckedState = ((ToggleButton)buttonBase).IsChecked;
                string eventName = "Checked";
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButton.CheckedEvent);
                routedEventArgs.Source = buttonBase;
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();

                if (initialIsCheckedState == false)
                {
                    EventTriggerCallback mouseLeftClickCallback = delegate()
                    {
                        UserInput.MouseLeftClickCenter(buttonBase);
                        QueueHelper.WaitTillQueueItemsProcessed();
                    };
                    eventTriggerCallbacks.Add(mouseLeftClickCallback);

                    EventTriggerCallback keyPressSpaceCallback = delegate()
                    {
                        KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Space);
                    };
                    eventTriggerCallbacks.Add(keyPressSpaceCallback);

                    if (buttonBase.GetType().Name.Equals("ToggleButton"))
                    {
                        EventTriggerCallback keyPressEnterCallback = delegate()
                        {
                            KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Enter);
                        };
                        eventTriggerCallbacks.Add(keyPressEnterCallback);
                    }

                    if (buttonBase.GetType().Name.Equals("CheckBox") && !((CheckBox)buttonBase).IsThreeState)
                    {
                        EventTriggerCallback keyPressAddCallback = delegate()
                        {
                            KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Add);
                        };
                        eventTriggerCallbacks.Add(keyPressAddCallback);
                    }
                }

                foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
                {
                    EventHelper.ExpectEvent<RoutedEventArgs>(eventTriggerCallback, buttonBase, eventName, routedEventArgs);
                    ValidateEndState((ToggleButton)buttonBase);
                }
            }
        }
    }
}

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

namespace Avalon.Test.ComponentModel
{
    class UncheckedEventTest : ToggleButtonValidation, IButtonBaseTest
    {
        public void Run(ButtonBase buttonBase)
        {
            if (buttonBase is ToggleButton)
            {
                bool isThreeState = ((ToggleButton)buttonBase).IsThreeState;
                initialIsCheckedState = ((ToggleButton)buttonBase).IsChecked;
                string eventName = "Unchecked";
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButton.UncheckedEvent);
                routedEventArgs.Source = buttonBase;
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();

                if (buttonBase.GetType().Name.Equals("CheckBox") && ((isThreeState == false && initialIsCheckedState == true)
                    || (isThreeState == true && initialIsCheckedState == null)))
                {
                    EventTriggerCallback mouseLeftClickCallback = delegate()
                    {
                        UserInput.MouseLeftClickCenter(buttonBase);
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                    };
                    eventTriggerCallbacks.Add(mouseLeftClickCallback);

                    if (!buttonBase.GetType().Name.Equals("CheckBox"))
                    {
                        EventTriggerCallback keyPressEnterCallback = delegate()
                        {
                            KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Enter);
                        };
                        eventTriggerCallbacks.Add(keyPressEnterCallback);
                    }

                    EventTriggerCallback keyPressSpaceCallback = delegate()
                    {
                        KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Space);
                    };
                    eventTriggerCallbacks.Add(keyPressSpaceCallback);

                    if (isThreeState == false && initialIsCheckedState == true)
                    {
                        EventTriggerCallback keyPressSubtractCallback = delegate()
                        {
                            KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Subtract);
                        };
                        eventTriggerCallbacks.Add(keyPressSubtractCallback);
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

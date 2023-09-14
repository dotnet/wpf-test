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
    class IndeterminateEventTest : ToggleButtonValidation, IButtonBaseTest
    {
        public void Run(ButtonBase buttonBase)
        {
            if (buttonBase is ToggleButton)
            {
                initialIsCheckedState = ((ToggleButton)buttonBase).IsChecked;
                bool isThreeState = ((ToggleButton)buttonBase).IsThreeState;
                string eventName = "Indeterminate";
                RoutedEventArgs routedEventArgs = new RoutedEventArgs(ToggleButton.IndeterminateEvent);
                routedEventArgs.Source = buttonBase;
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();

                if (buttonBase.GetType().Name.Equals("CheckBox") && isThreeState && initialIsCheckedState == true)
                {
                    EventTriggerCallback mouseLeftClickCallback = delegate()
                    {
                        UserInput.MouseLeftClickCenter(buttonBase);
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                    };
                    eventTriggerCallbacks.Add(mouseLeftClickCallback);

                    EventTriggerCallback keyPressSpaceCallback = delegate()
                    {
                        KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Space);
                    };
                    eventTriggerCallbacks.Add(keyPressSpaceCallback);
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

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
    public class ClickEventTest : ToggleButtonValidation, IButtonBaseTest
    {
        public void Run(ButtonBase buttonBase)
        {
            string eventName = "Click";
            RoutedEventArgs routedEventArgs = new RoutedEventArgs(ButtonBase.ClickEvent);
            routedEventArgs.Source = buttonBase;
            List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();
            if (buttonBase is ToggleButton)
            {
                initialIsCheckedState = ((ToggleButton)buttonBase).IsChecked;
            }

            EventTriggerCallback mouseLeftClickCallback = delegate()
            {
                UserInput.MouseLeftClickCenter(buttonBase);
                DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
            };
            eventTriggerCallbacks.Add(mouseLeftClickCallback);

            if (!buttonBase.GetType().Name.Equals("GridViewColumnHeader") &&
                !buttonBase.GetType().Name.Equals("DataGridColumnHeader") &&
                !buttonBase.GetType().Name.Equals("DataGridRowHeader"))
            {
                EventTriggerCallback keyPressSpaceCallback = delegate()
                {
                    KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Space);
                };
                eventTriggerCallbacks.Add(keyPressSpaceCallback);
            }

            if (buttonBase.GetType().Name.Equals("Button"))
            {
                EventTriggerCallback keyPressEnterCallback = delegate()
                {
                    KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Enter);
                };
                eventTriggerCallbacks.Add(keyPressEnterCallback);

                if (((Button)buttonBase).IsCancel)
                {
                    EventTriggerCallback testIsCancelCallback = delegate()
                    {
                        KeyActions.PressKeyWithFocus(buttonBase, System.Windows.Input.Key.Escape);
                    };
                    eventTriggerCallbacks.Add(testIsCancelCallback);
                }
                else if (((Button)buttonBase).IsDefault)
                {
                    EventTriggerCallback testIsDefaultCallback = delegate()
                    {
                        Panel panel = buttonBase.Parent as Panel;
                        if (panel == null)
                        {
                            throw new Exception("Button parent is not panel.");
                        }
                        TextBox textbox = new TextBox();
                        panel.Children.Add(textbox);
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

                        UserInput.MouseLeftClickCenter(textbox);
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);

                        UserInput.KeyPress(System.Windows.Input.Key.Enter.ToString());
                        DispatcherHelper.DoEvents(0, DispatcherPriority.ApplicationIdle);
                    };
                    eventTriggerCallbacks.Add(testIsDefaultCallback);
                }
            }

            foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
            {
                EventHelper.ExpectEvent<RoutedEventArgs>(eventTriggerCallback, buttonBase, eventName, routedEventArgs);
                if (buttonBase is ToggleButton)
                {
                    ValidateEndState((ToggleButton)buttonBase);
                }
            }
        }
    }
}

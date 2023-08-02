using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using Avalon.Test.ComponentModel;
using Microsoft.Test.Controls.Helpers;
using Microsoft.Test.Input;

namespace Microsoft.Test.Controls
{
    public class DropDownClosedEventTest : ISelectorTest
    {
        public void Run(Selector selector)
        {
            //open popup if it is closed.
            UserInput.KeyPress(System.Windows.Input.Key.F4.ToString());
            ComboBoxHelper.WaitForDropDownOpened((ComboBox)selector);

            string eventName = "DropDownClosed";
            List<bool> isEditables = new List<bool>() { false, true };
            foreach (bool isEditable in isEditables)
            {
                List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();
                if (selector is ComboBox)
                {
                    if (!((ComboBox)selector).IsEditable)
                    {
                        EventTriggerCallback mouseLeftClickCallback = delegate()
                        {
                            UserInput.MouseLeftClickCenter(ComboBoxHelper.FindDropDownToggleButton((ComboBox)selector));
                            ComboBoxHelper.WaitForDropDownClosed((ComboBox)selector);
                        };
                        eventTriggerCallbacks.Add(mouseLeftClickCallback);
                    }
                    EventTriggerCallback keyPressF4Callback = delegate()
                    {
                        selector.Focus();
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress(System.Windows.Input.Key.F4.ToString());
                        ComboBoxHelper.WaitForDropDownClosed((ComboBox)selector);
                    };
                    eventTriggerCallbacks.Add(keyPressF4Callback);
                    EventTriggerCallback keyPressAltDownCallback = delegate()
                    {
                        selector.Focus();
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyDown(System.Windows.Input.Key.LeftAlt.ToString());
                        UserInput.KeyPress(System.Windows.Input.Key.Down.ToString());
                        UserInput.KeyUp(System.Windows.Input.Key.LeftAlt.ToString());
                        ComboBoxHelper.WaitForDropDownClosed((ComboBox)selector);
                    };
                    eventTriggerCallbacks.Add(keyPressAltDownCallback);
                }

                foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
                {
                    EventHelper.ExpectEvent<EventArgs>(eventTriggerCallback, selector, eventName, new EventArgs());
                    selector.Focus();
                    QueueHelper.WaitTillQueueItemsProcessed();
                    UserInput.KeyPress(System.Windows.Input.Key.F4.ToString());
                    ComboBoxHelper.WaitForDropDownOpened((ComboBox)selector);
                }
            }
            UserInput.KeyPress(System.Windows.Input.Key.Escape.ToString());
            ComboBoxHelper.WaitForDropDownClosed((ComboBox)selector);
        }
    }
}

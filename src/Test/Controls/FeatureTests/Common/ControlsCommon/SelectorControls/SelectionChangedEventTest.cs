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
    public class SelectionChangedEventTest : ISelectorTest
    {
        public void Run(Selector selector)
        {
            string eventName = "SelectionChanged";
            List<EventTriggerCallback> eventTriggerCallbacks = new List<EventTriggerCallback>();
            List<ContentControl> addedItems = new List<ContentControl>();
            SelectionChangedEventArgs selectionChangedEventArgs = null;
            if (selector is ComboBox)
            {
                ComboBoxItem newItem = new ComboBoxItem();
                selector.Items.Add(newItem);
                addedItems.Add(newItem);
                selectionChangedEventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new List<ListBoxItem>(), addedItems);
                selectionChangedEventArgs.Source = selector;

                EventTriggerCallback mouseLeftClickCallback = delegate()
                {
                    UserInput.MouseLeftClickCenter(ComboBoxHelper.FindDropDownToggleButton((ComboBox)selector));
                    QueueHelper.WaitTillQueueItemsProcessed();
                    UserInput.KeyPress(System.Windows.Input.Key.Down.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                    UserInput.KeyPress(System.Windows.Input.Key.Enter.ToString());
                    QueueHelper.WaitTillQueueItemsProcessed();
                };
                eventTriggerCallbacks.Add(mouseLeftClickCallback);
            }
            else if (selector.GetType().Name.Equals("ListBox"))
            {
                ListBoxItem newItem = new ListBoxItem();
                newItem.Content = "Item1";
                selector.Items.Add(newItem);
                addedItems.Add(newItem);
                selectionChangedEventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new List<ListBoxItem>(), addedItems);
                selectionChangedEventArgs.Source = selector;
                QueueHelper.WaitTillQueueItemsProcessed();
                EventTriggerCallback mouseLeftClickCallback = delegate()
                {
                    UserInput.MouseLeftClickCenter(newItem);
                    QueueHelper.WaitTillQueueItemsProcessed();
                };
                eventTriggerCallbacks.Add(mouseLeftClickCallback);
            }
            else if (selector.GetType().Name.Equals("ListView"))
            {
                ListViewItem newItem = new ListViewItem();
                newItem.Content = "Item1";
                selector.Items.Add(newItem);
                addedItems.Add(newItem);
                selectionChangedEventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, new List<ListBoxItem>(), addedItems);
                selectionChangedEventArgs.Source = selector;
                QueueHelper.WaitTillQueueItemsProcessed();
                EventTriggerCallback mouseLeftClickCallback = delegate()
                {
                    UserInput.MouseLeftClickCenter(newItem);
                    QueueHelper.WaitTillQueueItemsProcessed();
                };
                eventTriggerCallbacks.Add(mouseLeftClickCallback);
            }
            else if (selector.GetType().Name.Equals("TabControl"))
            {
                TabItem removedItem = new TabItem();
                removedItem.Header = "Item1";
                selector.Items.Add(removedItem);
                List<ContentControl> removedItems = new List<ContentControl>();
                removedItems.Add(removedItem);
                TabItem addedItem = new TabItem();
                addedItem.Header = "Item2";
                selector.Items.Add(addedItem);
                addedItems.Add(addedItem);
                selectionChangedEventArgs = new SelectionChangedEventArgs(Selector.SelectionChangedEvent, removedItems, addedItems);
                selectionChangedEventArgs.Source = selector;
                QueueHelper.WaitTillQueueItemsProcessed();
                EventTriggerCallback mouseLeftClickCallback = delegate()
                {
                    UserInput.MouseLeftClickCenter(addedItem);
                    QueueHelper.WaitTillQueueItemsProcessed();
                };
                eventTriggerCallbacks.Add(mouseLeftClickCallback);
            }

            foreach (EventTriggerCallback eventTriggerCallback in eventTriggerCallbacks)
            {
                EventHelper.ExpectEvent<EventArgs>(eventTriggerCallback, selector, eventName, selectionChangedEventArgs);
            }
        }
    }
}

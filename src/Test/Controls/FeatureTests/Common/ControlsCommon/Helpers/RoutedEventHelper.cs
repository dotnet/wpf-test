using System;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// RoutedEventHelper
    /// </summary>
    public static class RoutedEventHelper
    {
        static bool isEventFired = false;

        /// <summary>
        /// Validate whether UIElement event fire or not
        /// </summary>
        /// <param name="source">An UIElement reference</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="action">An action</param>
        public static void ValidateEvent(UIElement source, string eventName, bool shouldEventFire, Action action)
        {
            EventInfo eventInfo = source.GetType().GetEvent(eventName);
            if (eventInfo == null)
            {
                throw new NullReferenceException("Fail: Could not find " + eventName + " event in object " + source.GetType().Name);
            }

            Type eventHandlerType = eventInfo.EventHandlerType;
            if (eventHandlerType == null)
            {
                throw new NullReferenceException("Fail: EventHandlerType is null.");
            }

            MethodInfo callbackMethodInfo = typeof(RoutedEventHelper).GetMethod("CallbackEventHandler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (callbackMethodInfo == null)
            {
                throw new NullReferenceException("Fail: MethodInfo is null.");
            }

            Delegate eventHandler = Delegate.CreateDelegate(eventHandlerType, callbackMethodInfo);
            if (eventHandler == null)
            {
                throw new NullReferenceException("Fail: eventHandler is null.");
            }

            RoutedEvent[] routedEvents = EventManager.GetRoutedEvents();
            RoutedEvent candidateRoutedEvent = null;

            foreach (RoutedEvent routedEvent in routedEvents)
            {
                if (routedEvent.Name.Equals(eventName) && (source.GetType().IsSubclassOf(routedEvent.OwnerType) || source.GetType() == routedEvent.OwnerType))
                {
                    candidateRoutedEvent = routedEvent;
                }
            }

            source.AddHandler(candidateRoutedEvent, eventHandler);
            action();
            source.RemoveHandler(candidateRoutedEvent, eventHandler);

            if (shouldEventFire != isEventFired)
            {
                throw new TestValidationException("Fail: the expected event fired " + shouldEventFire + " does not equal to the actual event fired " + isEventFired);
            }
        }

        /// <summary>
        /// Validate whether ItemsControl event fire or not
        /// </summary>
        /// <param name="source">An ItemsControl</param>
        /// <param name="itemIndex">Item index</param>
        /// <param name="eventName">Event name</param>
        /// <param name="shouldEventFire">Whether the event should fire or not</param>
        /// <param name="action">An action</param>
        public static void ValidateEventHandler(ItemsControl source, Type eventType, int attachEventIndex, string eventName, bool shouldEventFire, Action action)
        {
            if (source == null)
            {
                throw new NullReferenceException("Fail: the source is null.");
            }

            DependencyObject targetElement = source.ItemContainerGenerator.ContainerFromIndex(attachEventIndex);
            if (targetElement == null)
            {
                throw new NullReferenceException("Fail: the targetElement is null.");
            }

            MethodInfo callbackMethodInfo = typeof(RoutedEventHelper).GetMethod("CallbackEventHandler", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Static);
            if (callbackMethodInfo == null)
            {
                throw new NullReferenceException("Fail: the callbackMethodInfo is null.");
            }

            Delegate eventHandler = Delegate.CreateDelegate(typeof(RoutedEventHandler), callbackMethodInfo);
            if (eventHandler == null)
            {
                throw new NullReferenceException("Fail: the eventHandler is null.");
            }

            MethodInfo addEventHandler = eventType.GetMethod("Add" + eventName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (addEventHandler == null)
            {
                throw new NullReferenceException("Fail: the methodInfo is null.");
            }

            // Add event handler
            addEventHandler.Invoke(null, new object[] { targetElement, eventHandler });

            action();

            MethodInfo removeEventHandler = eventType.GetMethod("Remove" + eventName, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);
            if (removeEventHandler == null)
            {
                throw new NullReferenceException("Fail: the methodInfo is null.");
            }

            // Remove event handler
            removeEventHandler.Invoke(null, new object[] { targetElement, eventHandler });

            if (isEventFired != shouldEventFire)
            {
                throw new TestValidationException("Fail: actual event " + eventName + " fired " + isEventFired + " doesn't equal to the expected event fire " + shouldEventFire + ".");
            }

            // Reset isEventFired to false
            isEventFired = false;
        }

        static void CallbackEventHandler(object sender, EventArgs e)
        {
            isEventFired = true;
        }
    }
}

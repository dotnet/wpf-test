// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;

namespace RoutedEventHolder
{
    public class Service : DependencyObject
    {
        public static readonly RoutedEvent TestEvent = EventManager.RegisterRoutedEvent(
            "Test",
            RoutingStrategy.Bubble,
            typeof(RoutedEventHandler),
            typeof(Service)
        );

        public static void AddTestHandler( DependencyObject dependencyObject, RoutedEventHandler handler )
        {
            UIElement element = dependencyObject as UIElement;
            if (element != null)
            {
                element.AddHandler(TestEvent, handler);
            }
        }

        public static void RemoveTestHandler( DependencyObject dependencyObject, RoutedEventHandler handler )
        {
            UIElement element = dependencyObject as UIElement;
            if (element != null)
            {
                element.RemoveHandler(TestEvent, handler);
            }
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System.Windows;
using System.Windows.Controls;

namespace RegressionIssue122
{
    public class AttachedEventSource : Control
    {
        public static readonly RoutedEvent AttachEvent = EventManager.RegisterRoutedEvent("Attach", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(AttachedEventSource));

        public event RoutedEventHandler Attach
        {
            add { AddHandler(AttachEvent, value); }
            remove { RemoveHandler(AttachEvent, value); }
        }

    }
}

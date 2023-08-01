// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Microsoft.Test.Input;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// BaselineEventArgs
    /// </summary>
    public class BaselineEventArgs : RoutedEventArgs
    {
        private readonly Point _position;

        public BaselineEventArgs(RoutedEvent routedEvent, object source, Point position)
            : base(routedEvent, source)
        {
            this._position = position;
        }

        public Point Position
        {
            get
            {
                return this._position;
            }
        }
    }

    /// <summary>
    /// BaselineEventCollector
    /// </summary>
    public class BaselineEventCollector : EventCollector
    {
        public BaselineEventCollector(): base(true/*collectAllEvents*/)
        {
        }

        protected override void Collect(IList<EventParameters> list, object sender, RoutedEventArgs args)
        {
            BaselineEventArgs baselineArgs = args as BaselineEventArgs;
            Point position = baselineArgs == null ? new Point() : baselineArgs.Position;
            list.Add(new EventParameters(sender, args, position, args.RoutedEvent));
        }

        public void Collect(object sender, BaselineEventArgs args)
        {
            base.Collect(sender, args);
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
// Desc: Implements a non-blocking Flush() routine to ensure all messages in the queue are processed
//
//
// $Id:$ $Change:$
using System;
using System.Threading;
using System.Windows.Threading;

using System.Windows;
using System.Windows.Automation;

namespace Microsoft.Test.Animation
{

    /// <summary>
    /// Implements a Flush() routine to ensure all messages in the queue are processed
    /// This method is non-blocking.
    /// </summary>
    public class Queue
    {
        //Constructor is private
        private Queue()
        {
        }
        public static void Flush()
        {
            // this may not called within the context of an avalon app
            if (Dispatcher.CurrentDispatcher != null)
                Dispatcher.CurrentDispatcher.BeginInvoke(
                            DispatcherPriority.SystemIdle,
                            new DispatcherOperationCallback(QueueItemDelegate),
                            null);
        }
        private static object QueueItemDelegate(object arg)
        {
            // when in this method,
            // the queue is empty of all items with priority geater than DispatcherPriority.SystemIdle
            return null;
        }
    }

}

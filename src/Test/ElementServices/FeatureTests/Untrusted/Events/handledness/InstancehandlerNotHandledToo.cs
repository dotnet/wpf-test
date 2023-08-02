// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    ///<summary>
    /// Ensure the handled event will invoke other instance handlers, which is attached with HandledEventToo bit is true, for this event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Handledness
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  InstancehandlerNotHandledToo.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Bubble event</li>
    /// <li>Create a new instance of CustomControl, myControl</li>
    /// <li>Add two instance Listener, HandledEventToo is false</li>
    /// <li>Raise the Bubble event</li>
    /// <li>Ensure that Handler registered registered for CustomControl1 does not get invoked. </li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentHost", "InstancehandlerNotHandledToo")]
    public class InstancehandlerNotHandledToo : EventHelper
    {
        #region Constructor
        public InstancehandlerNotHandledToo()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            CoreLogger.LogStatus ("This is a BVT scenario for Handledness: Two handlers are all invoked althogh handled has been set to be true if HandledEventToo is true");

            // Local Varables
            // Create the objects to build a tree later and test Events
            CustomControl myControl = null;

            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create size of three object array to contain three CustomAvalonControl
            RouteTarget[] targets = new RouteTarget[2];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating custom controls"))
            {
                myControl = new CustomControl();
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for Bubble event"))
            {
                routedEvent = EventHelper.RoutedEvent1;
            }
            using (CoreLogger.AutoStatus ("add handlers"))
            {
                myControl.AddHandler (routedEvent, new CustomRoutedEventHandler (EventHandler1), false);
                myControl.AddHandler (routedEvent, new CustomRoutedEventHandler (EventHandler2), false);
            }

            using (CoreLogger.AutoStatus ("Raise the Bubble event"))
            {
                targets[0].Source = myControl;
                targets[0].Sender = myControl;
                targets[1].Source = myControl;
                targets[1].Sender = myControl;
                args = new CustomRoutedEventArgs (routedEvent, targets);
                myControl.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (1 != args.HandlersCalledCount)
                {
                    throw new Microsoft.Test.TestValidationException ("Incorrect HandlersCalledCount");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;

            // Exit Dispatcher
        }
        #endregion


        #region Public Members
        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler1 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("ClassHandler for Base class is running");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 0);
            args.Handled = true;
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler2 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("ClassHandler for Base class is running");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 1);
            args.Handled = true;
        }
        #endregion
    }
}

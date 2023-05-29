// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Events;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Verify the scenario a ContentHost has a visual child and a logical child. Raise event on logical child, visual child will not be affect. 
    /// <para />
    /// This is a BVT scenario for BuildRouteCore
    /// star=visual
    /// line=model
    ///     ch1
    ///      |  *                       
    ///      |   cc2
    ///     ce3 
    ///
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubblingEventRouting52.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Bubble event</li>
    /// <li>Create objects, as shown on the graph above, for the configuration</li>
    /// <li>Build the route</li>
    /// <li>Attach 1 instance handler for each element</li>
    /// <li>Raise the Bubble event</li>
    /// <li>Ensure Handlers are called in the correct order, and cc2 didn't get involved</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "BubblingEventRouting52")]
    public class BubblingEventRouting52 : EventHelper
    {
        #region Constructor
        public BubblingEventRouting52()
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
            CoreLogger.LogStatus ("This is a BVT scenario for a ContentHost has a visual child and a logical child");

            // Local Varables
            CustomContentHost ch1 = null;
            CustomControl cc2 = null;
            CustomContentElement ce3 = null;
            RoutedEvent routedEvent;

            // Create object array to contain three targets
            RouteTarget[] targets = new RouteTarget[9];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args = null;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating custom UIElements"))
            {
                ch1 = new CustomContentHost();
                cc2 = new CustomControl();
                ce3 = new CustomContentElement();
            }

            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                ch1.AppendChild (cc2);
                ch1.AppendModelChild (ce3);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = EventHelper.RoutedEvent1;
            }


            using (CoreLogger.AutoStatus ("Add event instance handlers"))
            {
                ch1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent2));
                ce3.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent1));
                cc2.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent));
            }

            using (CoreLogger.AutoStatus ("Raise the bubble event"))
            {
                targets[0].Sender = ce3;
                targets[0].Source = ce3;
                targets[1].Sender = ch1;
                targets[1].Source = ce3;
                args = new CustomRoutedEventArgs (routedEvent, targets);
                ce3.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (2 != args.HandlersCalledCount)
                {
                    throw new Microsoft.Test.TestValidationException ("Incorrect HandlersCalledCount");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 0);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent2 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent2");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 1);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent");
            args.HandlersCalledCount++;
        }
        #endregion
    }
}

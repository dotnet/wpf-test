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
    /// Tests calling EventRoute Add directly
    /// BuildRoute and InvokeHandler are internal. To keep coverage, keep this test case to use RaiseEvent.
    /// <para />
    /// This is a BVT scenario for attaching a bubble event to a simple customcontrol
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubblingEventRouteAdd.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handler for bubble event</li>
    /// <li>Raise the bubble event by calling EventRoute Add and InvokeHandlers</li>
    /// <li>Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "BubblingEventRouteAdd")]
    public class BubblingEventRouteAdd : EventHelper
    {
        #region Constructor
        public BubblingEventRouteAdd()
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
            CoreLogger.LogStatus("Attaching a bubble event to a simple customcontrol");
            CoreLogger.LogStatus("Tests attach bubbling event to customcontrol");

            // Local Varaibles
            // Create three CustomControl to build a tree later and test Events
            CustomControl control1=null;

            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create size of three object array to contain three CustomControls
            RouteTarget[] targets = new RouteTarget[1];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;



            using(CoreLogger.AutoStatus("Creating custom controls"))
            {
                control1 = new CustomControl();
            }

            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = BubblingEventRouteAdd.RoutedEvent1;
            }

            using(CoreLogger.AutoStatus("Add event handlers for bubble event"))
            {
                control1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
            }

            using(CoreLogger.AutoStatus("Raise the bubble event on control1"))
            {
                targets[0].Sender = control1;
                targets[0].Source = control1;
                args = new CustomRoutedEventArgs(routedEvent, targets);

                EventRoute route = new EventRoute(args.RoutedEvent);

                route.Add(control1, new CustomRoutedEventHandler(OnRoutedEvent1), false);

                args.Source=control1;
                control1.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                CoreLogger.LogStatus(args.HandlersCalledCount.ToString());
                if (args.HandlersCalledCount != 1)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
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
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }
        #endregion
    }
}



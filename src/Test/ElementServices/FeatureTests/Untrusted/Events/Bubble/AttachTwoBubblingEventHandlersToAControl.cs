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
    /// Tests Attaching Two Bubble EventHandlers to a CustomControl
    /// <para />
    /// This is a BVT scenario for attaching two bubble eventhandlers to a customcontrol.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AttachTwoBubblingEventHandlersToAControl.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble event: BuildRoute and InvokeHandlers</li>
    /// <li>Raise the bubble event: Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "AttachTwoBubblingEventHandlersToAControl")]
    public class AttachTwoBubblingEventHandlersToAControl : EventHelper
    {
        #region Constructor
        public AttachTwoBubblingEventHandlersToAControl()
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
            CoreLogger.LogStatus("Tests Attach Two Bubbling Event Handlers To A Control.");

            // Local Varaibles
            // Create one CustomControl to test Events
            CustomControl control1=null;
            
            // Create a routedEvent1 to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent1;
            
            // Create size of one object array to contain three CustomControls
            RouteTarget[] targets = new RouteTarget[2];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            using(CoreLogger.AutoStatus("Creating a custom control"))
            {
                control1 = new CustomControl();
            }
                
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent1 = AttachTwoBubblingEventHandlersToAControl.RoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add event handlers for bubble event"))
            {
                control1.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent1));
                control1.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent2));
            }
        
            using(CoreLogger.AutoStatus("Raise the bubble event: BuildRoute and InvokeHandlers"))
            {
                targets[0].Sender = control1;
                targets[0].Source = control1;
                targets[1].Sender = control1;
                targets[1].Source = control1;

                args = new CustomRoutedEventArgs(routedEvent1, targets);
                control1.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                if (args.HandlersCalledCount != 2)
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        } 
        #endregion


        #region Public Members
        /// <summary>
        /// OnRoutedEvent1 method: Event fire first.
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }

        /// <summary>
        /// OnRoutedEvent2 method: Event fire second
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent2(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent2");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 1);
        }
        #endregion
    }
}



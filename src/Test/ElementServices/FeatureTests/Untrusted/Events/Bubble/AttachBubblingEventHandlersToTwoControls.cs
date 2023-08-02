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
    /// Tests Attaching Bubble EventHandlers to two CustomControls and make sure events fire in order
    /// <para />
    /// This is a BVT scenario for a simple tree with control1 visual parent of controls2. CC2->CC1: CC1 has E1 and E2, CC2 has E3
    /// BuildRoute: E3->E1->E2
    /// Bubbling event fire order should be E3, E1, E2
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AttachBubblingEventHandlersToTwoControls.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble event: BuildRoute and InvokeHandlers</li>
    /// <li>Raise the bubble event: Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "AttachBubblingEventHandlersToTwoControls")]
    public class AttachBubblingEventHandlersToTwoControls : EventHelper
    {
        #region Constructor
        public AttachBubblingEventHandlersToTwoControls()
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
            CoreLogger.LogStatus("Tests Attaching Bubble EventHandlers to two CustomControls and make sure events fire in order.");

            // Local Varaibles
            // Create two CustomControl to build a tree later and test Events
            CustomControl control1=null;
            CustomControl control2=null;
            
            // Create a routedEvent1 to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent1;
            
            // Create size of three object array to contain three CustomControls
            RouteTarget[] senders = new RouteTarget[3];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;
            
            using(CoreLogger.AutoStatus("Creating two custom controls"))
            {
                control1 = new CustomControl();
                control2 = new CustomControl();
            }
                
            using(CoreLogger.AutoStatus("Construct tree"))
            {
                control1.AppendChild(control2);
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent1 = AttachBubblingEventHandlersToTwoControls.RoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add event handlers for bubble event on both controls"))
            {
                control1.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent1));
                control1.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent2));
                control2.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent3));
            }
        
            using(CoreLogger.AutoStatus("Raise the bubble event: BuildRoute and InvokeHandlers"))
            {
                senders[0].Source = control2;
                senders[0].Sender = control2;
                senders[1].Source = control2;
                senders[1].Sender = control1;
                senders[2].Source = control2;
                senders[2].Sender = control1;
                args = new CustomRoutedEventArgs(routedEvent1, senders);
                control2.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                if (args.HandlersCalledCount != 3)
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        } 
        #endregion


        #region Public Members
        /// <summary>
        /// OnRoutedEvent1 method: Event fire second.
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 1);
        }
    
        /// <summary>
        /// OnRoutedEvent2 method: Event fire last
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent2(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent2");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 2);
        }

        /// <summary>
        /// OnRoutedEvent2 method: Event fire first
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent3(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent3");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 0);
        }
        #endregion
    }
}
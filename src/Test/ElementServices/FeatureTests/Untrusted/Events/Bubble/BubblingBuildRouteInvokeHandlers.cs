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
    /// Tests calling BuildRoute and InvokeHandlers directly.
    /// BuildRoute and InvokeHandler are internal. To keep coverage, keep this test case to use RaiseEvent.
    /// <para />
    /// This is a BVT scenario for attaching bubble event to a simple tree with control1 visual parent of control2 visual parent of control3. CC3->CC2->CC1
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// Dev: Microsoft
    /// <para />
    /// <para />
    /// FileName:  BubblingBuildRouteInvokeHandlers.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble events by calling BuildRoute and InvokeHandlers directly</li>
    /// <li>Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "BubblingBuildRouteInvokeHandlers")]
    public class BubblingBuildRouteInvokeHandlers : EventHelper
    {
        #region Constructor
        public BubblingBuildRouteInvokeHandlers()
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
            CoreLogger.LogStatus("Simple tree with control1 visual parent of control2 visual parent of control3");
            CoreLogger.LogStatus("Tests calling BuildRoute and InvokeHandlers directly");

            // Local Varaibles
            // Create three CustomControl to build a tree later and test Events
            CustomControl control1=null;
            CustomControl control2=null;
            CustomControl control3=null;

            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create size of three object array to contain three CustomControls
            RouteTarget[] targets = new RouteTarget[3];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;



            using(CoreLogger.AutoStatus("Creating custom controls"))
            {
                control1 = new CustomControl();
                control2 = new CustomControl();
                control3 = new CustomControl();
            }

            using(CoreLogger.AutoStatus("Construct tree"))
            {
                control1.AppendChild(control2);
                control2.AppendChild(control3);
            }

            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = BubblingBuildRouteInvokeHandlers.RoutedEvent1;
            }

            using(CoreLogger.AutoStatus("Add event handlers for bubble event"))
            {
                control1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
                control2.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent2));
                control3.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent3));
            }

            using(CoreLogger.AutoStatus("Raise the bubble event on control1"))
            {
                targets[0].Sender = control3;
                targets[0].Source = control3;
                targets[1].Sender = control2;
                targets[1].Source = control3;
                targets[2].Sender = control1;
                targets[2].Source = control3;
                args = new CustomRoutedEventArgs(routedEvent, targets);

                EventRoute route = new EventRoute(args.RoutedEvent);
                args.Source=control3;
                control3.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                CoreLogger.LogStatus(args.HandlersCalledCount.ToString());
                if (args.HandlersCalledCount != 3)
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
            this.VerifyRoutedEvent(sender, args, 2);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent2(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent2");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 1);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent3(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent3");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }
        #endregion
    }
}



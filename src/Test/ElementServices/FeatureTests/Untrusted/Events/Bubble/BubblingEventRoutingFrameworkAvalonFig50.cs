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
    /// Tests Attaching Bubble EventHandler to Configuration Fig 50 to test bubbling
    /// <para />
    /// This is a BVT scenario for attaching Bubble event to the follow tree
    /// star=visual
    /// line=model
    ///   
    ///  
    ///     ch1
    ///     *  \
    ///    *    \
    ///   cc2  ce3
    ///    *    |
    ///    *    |
    ///    *    |
    ///  cc4   ce5
    ///    *    /
    ///     *  /
    ///     cc6
    ///
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubblingEventRoutingFrameworkAvalonFig50.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Bubble event</li>
    /// <li>Create objects for the configuration</li>
    /// <li>Build the route</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the Bubble event</li>
    /// <li>Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "BubblingEventRoutingFrameworkAvalonFig50")]
    public class BubblingEventRoutingFrameworkAvalonFig50 : EventHelper
    {
        #region Constructor
        public BubblingEventRoutingFrameworkAvalonFig50()
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
            CoreLogger.LogStatus ("This is a BVT scenario for bubble event for Fig 50");
            CoreLogger.LogStatus ("Tests attach bubbling event");

            // Local Varables
            // Create the objects to build a tree later and test Events
            CustomContentHost ch1 = null;
            CustomControl cc2 = null;
            CustomContentElement ce3 = null;
            CustomControl cc4 = null;
            CustomContentElement ce5 = null;
            CustomControl cc6 = null;

            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create size of three object array to contain three CustomAvalonControl
            RouteTarget[] targets = new RouteTarget[6];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating custom controls"))
            {
                ch1 = new CustomContentHost();
                ce3 = new CustomContentElement();
                ce5 = new CustomContentElement();
                cc2 = new CustomControl();
                cc4 = new CustomControl();
                cc6 = new CustomControl();
            }

            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                ch1.AppendModelChild (ce3);
                ch1.AppendChild (cc2);
                cc2.AppendChild (cc4);
                ce3.AppendModelChild (ce5);
                cc4.AppendChild (cc6);
                ce5.AppendModelChild (cc6);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = EventHelper.RoutedEvent1;
            }

            using (CoreLogger.AutoStatus ("Add event handlers for bubble event"))
            {
                ch1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent6));
                ce3.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent5));
                ce5.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent4));
                cc2.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent3));
                cc4.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent2));
                cc6.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent1));
            }

            using (CoreLogger.AutoStatus ("Raise the bubble event on control6"))
            {
                targets[0].Source = cc6;
                targets[0].Sender = cc6;
                targets[1].Source = cc4;
                targets[1].Sender = cc4;
                targets[2].Source = cc4;
                targets[2].Sender = cc2;
                targets[3].Source = cc6;
                targets[3].Sender = ce5;
                targets[4].Source = cc6;
                targets[4].Sender = ce3;
                targets[5].Source = cc6;
                targets[5].Sender = ch1;
                args = new CustomRoutedEventArgs (routedEvent, targets);
                cc6.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (6 != args.HandlersCalledCount)
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
        public void OnRoutedEvent3 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent3");
            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 2);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent4 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent4");
            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 3);
        }
        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent5 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent5");
            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 4);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent6 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent6");
            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 5);
        }
        #endregion
    }
}

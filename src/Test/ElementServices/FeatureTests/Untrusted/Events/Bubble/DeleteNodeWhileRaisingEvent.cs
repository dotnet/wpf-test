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
    /// Tests Attaching Bubble EventHandler to Configuration Fig 47 to test Bubbling
    /// <para />
    /// This is a BVT scenario for attaching bubble event to the follow tree
    /// star=visual
    /// line=model
    /// cc1   
    ///  |*
    ///  | ch2
    ///  |  *  \
    ///  | cc3  ce4
    ///  |   *  /
    ///  |   cc5
    ///  | *
    /// cc6
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubblingEventRoutingFrameworkAvalonFig48.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble event</li>
    /// <li>Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "DeleteNodeWhileRaisingEvent")]
    public class DeleteNodeWhileRaisingEvent : EventHelper
    {
        #region Constructor
        public DeleteNodeWhileRaisingEvent()
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
            CoreLogger.LogStatus ("This is a BVT scenario for bubble event for Fig 48");
            CoreLogger.LogStatus ("Tests attach bubbling event");

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
                _cc1 = new CustomControl();
                _cc2 = new CustomControl();
                _cc3 = new CustomControl();
                _cc4 = new CustomControl();
            }

            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                _cc1.AppendChild (_cc2);
                _cc2.AppendChild (_cc3);
                _cc3.AppendChild (_cc4);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = BubblingEventRoutingFrameworkAvalonFig42.RoutedEvent1;
            }

            using (CoreLogger.AutoStatus ("Add event handlers for bubble event"))
            {
                _cc1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent4));
                _cc2.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent3));
                _cc3.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent2));
                _cc4.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent1));
            }

            using (CoreLogger.AutoStatus ("Raise the bubble event on control3"))
            {
                targets[0].Source = _cc4;
                targets[0].Sender = _cc4;
                targets[1].Source = _cc4;
                targets[1].Sender = _cc3;
                targets[2].Source = _cc4;
                targets[2].Sender = _cc2;
                targets[3].Source = _cc4;
                targets[3].Sender = _cc1;



                args = new CustomRoutedEventArgs (routedEvent, targets);
                _cc4.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (4 != args.HandlersCalledCount)
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
            _cc2.RemoveChild (_cc3);
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

        CustomControl _cc1 = null;
        CustomControl _cc2 = null;
        CustomControl _cc3 = null;
        CustomControl _cc4 = null;
    }
    #endregion
}



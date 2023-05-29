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
    /// <summary>
    /// Tests Remove Tunnel EventHandler from CustomControls
    /// <para />
    /// This is a BVT scenario for removing tunnel event from a simple tree with control1 visual parent of control2 visual parent of control3. CC1->CC2->CC3
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\RemoveTunnelingEventHandler
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RemoveTunnelingEventHandler.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for tunnel event</li>
    /// <li>Add event handlers for tunnel event</li>
    /// <li>Raise the tunnel event</li>
    /// <li>Remmove event handlers for tunnel event</li>
    /// <li>Raise the tunnel event</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Tunnel", "RemoveTunnelingEventHandler")]
    public class RemoveTunnelingEventHandler : EventHelper
    {
        #region Constructor
        public RemoveTunnelingEventHandler()
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
            CoreLogger.LogStatus("Tests remove tunneling event from customcontrols");

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

            // Enter Dispatcher
            
            
            using(CoreLogger.AutoStatus("Creating custom controls "))
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
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for tunnel event"))
            {
                routedEvent = RemoveTunnelingEventHandler.PreviewRoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add event handlers for tunnel event"))
            {
                control1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent));
                control2.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent));
                control3.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent));
            }
            
            using(CoreLogger.AutoStatus("Raise the tunnel event"))
            {
                targets[0].Source = control3;
                targets[0].Sender = control1;
                targets[1].Source = control3;
                targets[1].Sender = control2;
                targets[2].Source = control3;
                targets[2].Sender = control3;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                control3.RaiseEvent(args);
            }
            
            using(CoreLogger.AutoStatus("Validation for preview event"))
            {
                if (args.HandlersCalledCount != 3)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }

            using(CoreLogger.AutoStatus("Remove event control1 handler for tunnel event"))
            {
                control1.RemoveHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent));
            }

            using(CoreLogger.AutoStatus("Raise the tunnel event"))
            {
                targets[0].Source = control3;
                targets[0].Sender = control2;
                targets[1].Source = control3;
                targets[1].Sender = control3;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                control3.RaiseEvent(args);

                if (args.HandlersCalledCount != 2)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }
            
            using(CoreLogger.AutoStatus("Remove event control2 handler for tunnel event"))
            {
                control2.RemoveHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent));
            }
            
            using(CoreLogger.AutoStatus("Raise the tunnel event"))
            {
                targets[0].Source = control3;
                targets[0].Sender = control3;

                args = new CustomRoutedEventArgs(routedEvent, targets);
                control3.RaiseEvent(args);            
            
                if (args.HandlersCalledCount != 1)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
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
        public void OnRoutedEvent(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args);
        }  
        #endregion
    }
}



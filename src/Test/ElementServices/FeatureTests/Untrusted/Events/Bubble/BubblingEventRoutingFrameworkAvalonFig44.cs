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
    /// Tests Attaching Bubble EventHandlers to CustomAvalonContentHost, CustomContentElement, CustomAvalonControl, CustomAvalonControl
    /// <para />
    /// This is a BVT scenario for attaching bubble event to a simple tree with contentHost1 visual parent of control4 visual parent of control3, 
    /// and contentHost1 model parent of contentElement2 model parent of control3. 
    /// star=visual
    /// line=model
    ///    CH1
    ///   *   \
    ///  *     \
    /// CC4   CE2
    ///  *     /
    ///   *   /
    ///    CC3
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Bubble
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubblingEventRoutingFrameworkAvalonFig44.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble event</li>
    /// <li>Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Bubble", "BubblingEventRoutingFrameworkAvalonFig44")]
    public class BubblingEventRoutingFrameworkAvalonFig44 : EventHelper
    {
        #region Constructor
        public BubblingEventRoutingFrameworkAvalonFig44()
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
            CoreLogger.LogStatus("This is a BVT scenario for attaching bubble event to a simple tree with contentHost1 visual parent of control4 visual parent of control3,");
            CoreLogger.LogStatus("and contentHost1 model parent of contentElement2 model parent of control3.");
            CoreLogger.LogStatus("Tests attach bubbling event to CustomAvalonContentHost, CustomContentElement, CustomAvalonControl, CustomAvalonControl. Then Raise Events.");

            // Local Varaibles
            // Create three CustomAvalonControl to build a tree later and test Events
            CustomAvalonContentHost contentHost1 = null;
            CustomContentElement contentElement2 = null;
            CustomAvalonControl control3 = null;
            CustomAvalonControl control4 = null;
            
            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;
            
            // Create size of three object array to contain three CustomFrameworkElements
            RouteTarget[] targets = new RouteTarget[4];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            
            using(CoreLogger.AutoStatus("Creating custom controls"))
            {
                contentHost1 = new CustomAvalonContentHost(context);
                contentElement2 = new CustomContentElement();
                control3 = new CustomAvalonControl(context);
                control4 = new CustomAvalonControl(context);
            }
                
            using(CoreLogger.AutoStatus("Construct tree"))
            {
                contentHost1.AppendModelChild(contentElement2);
                contentElement2.AppendModelChild(control3);
                contentHost1.AppendChild(control4);
                control4.AppendChild(control3);
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = BubblingEventRoutingFrameworkAvalonFig44.RoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add event handlers for bubble event"))
            {
                contentHost1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
                contentElement2.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent2));
                control4.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent3));
                control3.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent4));
            }
        
            using(CoreLogger.AutoStatus("Raise the bubble event on control3"))
            {

                targets[0].Source = control3;
                targets[0].Sender = control3;
                targets[1].Source = control4;
                targets[1].Sender = control4;
                targets[2].Source = control3;
                targets[2].Sender = contentElement2;
                targets[3].Source = control3;
                targets[3].Sender = contentHost1;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                control3.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                if (args.HandlersCalledCount != 4)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
            
            // Exit Dispatcher
        }    
        #endregion


        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 3);
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
            this.VerifyRoutedEvent(sender, args, 2);
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
            this.VerifyRoutedEvent(sender, args, 1);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent4(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent4");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }
    }
}


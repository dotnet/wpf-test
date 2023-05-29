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
    /// Eventing Handledness: 3 nodes each with 2 instance handlers, with HandledEventToo are true, false, 
    /// and mixed respectively. 
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Handledness
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  InstancehandlerMultipleNodes.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Bubble event</li>
    /// <li>Create 3 new instances of CustomControl, myControl1, myControl2, and myControl3</li>
    ///<li>Construct tree</li>
    /// <li>Add two instance Listeners to myControl1 HandledEventToo are true</li>
    /// <li>Add two instance Listeners to myControl2 HandledEventToo are false</li>
    /// <li>Add two instance Listeners to myControl1 HandledEventToo are mixed</li>
    /// <li>Raise the Bubble event and Ensure that the sequence and number of handler is correct</li>
    /// <li>set Handled in args to be true</li>
    /// <li>Raise the Bubble event and Ensure that the sequence and number of handler is correct</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentHost", "InstancehandlerMultipleNodes")]
    public class InstancehandlerMultipleNodes : EventHelper
    {
        #region Constructor
        public InstancehandlerMultipleNodes()
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
            CustomControl myControl1 = null;
            CustomControl myControl2 = null;
            CustomControl myControl3 = null;
            CustomRoutedEventHandler myHandler1 = null;
            CustomRoutedEventHandler myHandler2 = null;
            CustomRoutedEventHandler myHandler3 = null;
            CustomRoutedEventHandler myHandler4 = null;
            CustomRoutedEventHandler myHandler5 = null;
            CustomRoutedEventHandler myHandler6 = null;

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
                myControl1 = new CustomControl();
                myControl2 = new CustomControl();
                myControl3 = new CustomControl();
            }
            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                myControl1.AppendChild (myControl2);
                myControl2.AppendChild (myControl3);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for Bubble event"))
            {
                routedEvent = EventHelper.RoutedEvent1;
            }

            using (CoreLogger.AutoStatus ("add handlers"))
            {
                myHandler1 = new CustomRoutedEventHandler (EventHandler1);
                myHandler2 = new CustomRoutedEventHandler (EventHandler2);
                myHandler3 = new CustomRoutedEventHandler (EventHandler3);
                myHandler4 = new CustomRoutedEventHandler (EventHandler4);
                myHandler5 = new CustomRoutedEventHandler (EventHandler5);
                myHandler6 = new CustomRoutedEventHandler (EventHandler6);
                
                myControl1.AddHandler (routedEvent, myHandler5, false);
                myControl1.AddHandler (routedEvent, myHandler6, true);
                myControl2.AddHandler (routedEvent, myHandler3, true);
                myControl2.AddHandler (routedEvent, myHandler4, true);
                myControl3.AddHandler (routedEvent, myHandler1, false);
                myControl3.AddHandler (routedEvent, myHandler2, false);
            }

            using (CoreLogger.AutoStatus ("Raise the Bubble event"))
            {
                targets[0].Source = myControl3;
                targets[0].Sender = myControl3;
                targets[1].Source = myControl3;
                targets[1].Sender = myControl3;
                targets[2].Source = myControl3;
                targets[2].Sender = myControl2;
                targets[3].Source = myControl3;
                targets[3].Sender = myControl2;
                targets[4].Source = myControl3;
                targets[4].Sender = myControl1;
                targets[5].Source = myControl3;
                targets[5].Sender = myControl1;
                args = new CustomRoutedEventArgs (routedEvent, targets);
                myControl3.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (6 != args.HandlersCalledCount)
                {
                    throw new Microsoft.Test.TestValidationException ("Incorrect HandlersCalledCount");
                }
            }
            using (CoreLogger.AutoStatus ("set Handledness and reset CalledCount"))
            {
                args.Handled = true;
                args.HandlersCalledCount = 0;
            }
            using (CoreLogger.AutoStatus ("Remove and add handler to make sure correct sequence"))
            {
                myControl1.RemoveHandler (routedEvent, myHandler6);
                myControl2.RemoveHandler (routedEvent, myHandler3);
                myControl2.RemoveHandler (routedEvent, myHandler4);
            }

            using (CoreLogger.AutoStatus ("Add New handlers"))
            {
                myControl1.AddHandler (routedEvent, myHandler3, true);
                myControl2.AddHandler (routedEvent, myHandler1, true);
                myControl2.AddHandler (routedEvent, myHandler2, true);
            }


                
            using (CoreLogger.AutoStatus ("Raise event again"))
            {
                targets[0].Source = myControl3;
                targets[0].Sender = myControl2;
                targets[1].Source = myControl3;
                targets[1].Sender = myControl2;
                targets[2].Source = myControl3;
                targets[2].Sender = myControl1;
                myControl3.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (3 != args.HandlersCalledCount)
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
            CoreLogger.LogStatus ("Invoke order 1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 0);
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler2 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Invoke order 2");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 1);
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler3 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Invoke order 3");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 2);
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler4 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Invoke  order 5");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 3);
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler5 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Invoke the order 5");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 4);
        }

        /// <summary>
        /// Handler for Base Class
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void EventHandler6 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Invoke the order 6");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 5);
        }
        #endregion
    }
}

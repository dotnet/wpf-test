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
    /// All ContentElement Bubble Tunnel
    /// <para />
    /// This is a BVT scenario for attaching Tunnel event to the follow tree
    /// line=model
    ///     ce1 (CustomContentElement2 : CustomContentElement) 2 ClassHandlers attached
    ///      |  
    ///      |    
    ///     ce2 (CustomContentElement1 : CustomContentElement) one ClassHandler attached
    ///      |    
    ///      |  
    ///     ce3 (CustomContentElement1 : CustomContentElement) one ClassHandler attached
    ///      | 
    ///      | 
    ///     ce4 (CustomContentElement)
    ///      |
    ///      |
    ///     ce5 (CustomContentElement)
    ///
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  TunnelMultipleHandlerOnPureContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Tunnel event</li>
    /// <li>Register 1 class handler for CustomContentElement1 and 2 class handlers for CustomContentElement2</li>
    /// <li>Create objects, as shown on the graph above, for the configuration</li>
    /// <li>Build the route</li>
    ///
    /// <li>Attach 1 instance handler for ce4</li>
    /// <li>Attach 1 instance handler for ce2</li>
    /// <li>Raise the Tunnel event</li>
    /// <li>Ensure Handlers are called in the correct order: Tunnel down, last added last evoked, class handlers are invoked before instance handler</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "TunnelMultipleHandlerOnPureContentElement")]
    public class TunnelMultipleHandlerOnPureContentElement : EventHelper
    {
        #region Constructor
        public TunnelMultipleHandlerOnPureContentElement()
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
            CoreLogger.LogStatus ("This is a BVT scenario for ContentElement with multiple class handler and instance handler");
            CoreLogger.LogStatus ("Tests attach Tunneling event");

            // Local Varables
            // Create the objects to build a tree later and test Events
            CustomContentElement ce5 = null;
            CustomContentElement ce4 = null;
            CustomContentElement1 ce3 = null;
            CustomContentElement1 ce2 = null;
            CustomContentElement2 ce1 = null;

            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create size of three object array to contain three CustomAvalonControl
            RouteTarget[] targets = new RouteTarget[8];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating custom controls"))
            {
                ce5 = new CustomContentElement();
                ce4 = new CustomContentElement();
                ce3 = new CustomContentElement1 ();
                ce2 = new CustomContentElement1 ();
                ce1 = new CustomContentElement2 ();
            }

            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                ce1.AppendModelChild (ce2);
                ce2.AppendModelChild (ce3);
                ce3.AppendModelChild (ce4);
                ce4.AppendModelChild (ce5);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for Tunnel event"))
            {
                routedEvent = EventHelper.PreviewRoutedEvent1;
            }
            using (CoreLogger.AutoStatus ("Register ClassHandlers"))
            {
                EventManager.RegisterClassHandler (typeof(CustomContentElement1), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent));
                EventManager.RegisterClassHandler (typeof(CustomContentElement2), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent));
                EventManager.RegisterClassHandler (typeof(CustomContentElement2), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent));
            }

            using (CoreLogger.AutoStatus ("Add event instance handlers"))
            {
                ce4.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent8));
                ce2.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent6));
                ce1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent3));
                ce1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent4));
            }

            using (CoreLogger.AutoStatus ("Raise the Tunnel event"))
            {
                targets[0].Sender = ce1;
                targets[0].Source = ce5;
                targets[1].Sender = ce1;
                targets[1].Source = ce5;
                targets[2].Sender = ce1;
                targets[2].Source = ce5;
                targets[3].Sender = ce1;
                targets[3].Source = ce5;
                targets[4].Sender = ce2;
                targets[4].Source = ce5;
                targets[5].Sender = ce2;
                targets[5].Source = ce5;
                targets[6].Sender = ce3;
                targets[6].Source = ce5;
                targets[7].Sender = ce4;
                targets[7].Source = ce5;

                args = new CustomRoutedEventArgs (routedEvent, targets);
                ce5.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (8 != args.HandlersCalledCount)
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

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent7 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent7");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 6);
        }

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent8 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("OnRoutedEvent8");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent (sender, args, 7);
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

        private class CustomContentElement1 : CustomContentElement
        {
            public CustomContentElement1 () : base() { }
        }

        private class CustomContentElement2 : CustomContentElement
        {
            public CustomContentElement2 () : base() { }
        }
        #endregion
    }
}

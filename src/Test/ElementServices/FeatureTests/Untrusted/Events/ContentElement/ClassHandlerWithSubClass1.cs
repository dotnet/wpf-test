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
    /// Test the class eventhandler for subclass
    ///
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  ClassHandlerWithSubClass1.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Tunnel event</li>
    /// <li>Register 1 class handler for CustomContentElement1 and 1 class handlers for CustomContentElement2, where CustomContentElement2 is subclass of CustomContentElement1</li>
    /// <li>Create a object of class CustomContentElement</li>
    /// <li>Raise the Tunnel event</li>
    /// <li>Ensure Handlers are called in the correct order</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "ClassHandlerWithSubClass1")]
    public class ClassHandlerWithSubClass1 : EventHelper
    {
        #region Constructor
        public ClassHandlerWithSubClass1()
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
            CoreLogger.LogStatus ("This is a BVT scenario Test the class eventhandler for subclass");

            //Local variables
            CustomContentElement2 ce = null;
            RoutedEvent routedEvent = null;
            CustomRoutedEventArgs args = null;

            // Create size of three object array to contain three CustomAvalonControl
            RouteTarget[] targets = new RouteTarget[3];

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating element"))
            {
                ce = new CustomContentElement2 ();
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for Tunnel event"))
            {
                routedEvent = EventHelper.PreviewRoutedEvent1;
            }

            using (CoreLogger.AutoStatus ("Register ClassHandlers"))
            {
                EventManager.RegisterClassHandler (typeof(CustomContentElement1), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent2));
                EventManager.RegisterClassHandler (typeof(CustomContentElement2), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent1));
                EventManager.RegisterClassHandler (typeof(ContentElement), routedEvent, new CustomRoutedEventHandler (OnRoutedEvent3));
            }

            using (CoreLogger.AutoStatus ("Raise the Tunnel event"))
            {
                targets[0].Source = ce;
                targets[0].Sender = ce;
                targets[1].Source = ce;
                targets[1].Sender = ce;
                targets[2].Source = ce;
                targets[2].Sender = ce;

                args = new CustomRoutedEventArgs (routedEvent, targets);
                ce.RaiseEvent (args);
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
        private void OnRoutedEvent1 (object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus ("Running OnRoutedEvent");

            this.VerifyRoutedEvent (sender, args, 0);
        }

        private class CustomContentElement1 : CustomContentElement
        {
            public CustomContentElement1 () : base() { }
        }
        private class CustomContentElement2 : CustomContentElement1
        {
            public CustomContentElement2 () : base() { }
        }
        #endregion
    }
}

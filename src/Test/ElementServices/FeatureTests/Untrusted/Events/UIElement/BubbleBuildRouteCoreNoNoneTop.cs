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
    /// Verify an UIelement whose BuildRouteCore return a UIElement will not add the returned not effect the route if it has UIParant
    /// <para />
    /// This is a BVT scenario for BuildRouteCore, the tree:
    /// star=visual
    /// line=model
    ///   
    ///                                  cu4
    ///                                   *
    ///     cu1 (BuildRouteCore return)->cu5
    ///      *  
    ///      *    
    ///     cu2
    ///      * 
    ///      *
    ///     cu3 
    ///
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BubbleBuildRouteCoreNoNoneTop.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for Bubble event</li>
    /// <li>Create objects, as shown on the graph above, for the configuration</li>
    /// <li>Build the route</li>
    /// <li>Attach 1 instance handler for each element</li>
    /// <li>Raise the Bubble event</li>
    /// <li>Ensure Handlers are called in the correct order, and cu4 and cu5 get involved</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "BubbleBuildRouteCoreNoNoneTop")]
    public class BubbleBuildRouteCoreNoNoneTop : EventHelper
    {
        #region Constructor
        public BubbleBuildRouteCoreNoNoneTop()
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
            CoreLogger.LogStatus ("This is a BVT scenario for a line of element, ended with a ContentElement and beginning with a UIElement, with multiple class handler and instance handler");

            // Local Varables
            // Create the objects to build a tree later and test Events
            CustomUIElement cu2 = null;
            UIElementContinue cu1 = null;
            CustomUIElement cu3 = null;
            CustomUIElement cu4 = null;
            CustomUIElement cu5 = null;


            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent;

            // Create object array to contain three targets
            RouteTarget[] targets = new RouteTarget[9];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            using (CoreLogger.AutoStatus ("Creating custom UIElements"))
            {
                cu2 = new CustomUIElement ();
                cu3 = new CustomUIElement ();
                cu4 = new CustomUIElement ();
                cu5 = new CustomUIElement ();
                cu1 = new UIElementContinue (cu5);
            }

            using (CoreLogger.AutoStatus ("Construct tree"))
            {
                cu1.AppendChild (cu2);
                cu2.AppendChild (cu3);
                cu4.AppendChild (cu5);
            }

            using (CoreLogger.AutoStatus ("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = EventHelper.RoutedEvent1;
            }


            using (CoreLogger.AutoStatus ("Add event instance handlers"))
            {
                cu1.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent3));
                cu2.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent2));
                cu3.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent1));
                cu4.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent5));
                cu5.AddHandler (routedEvent, new CustomRoutedEventHandler (OnRoutedEvent4));
            }

            using (CoreLogger.AutoStatus ("Raise the bubble event"))
            {
                targets[0].Source = cu3;
                targets[0].Sender = cu3;
                targets[1].Source = cu3;
                targets[1].Sender = cu2;
                targets[2].Source = cu3;
                targets[2].Sender = cu1;
                targets[3].Source = cu3;
                targets[3].Sender = cu5;
                targets[4].Source = cu3;
                targets[4].Sender = cu4;

                args = new CustomRoutedEventArgs (routedEvent, targets);
                cu3.RaiseEvent (args);
            }

            using (CoreLogger.AutoStatus ("Validation for event"))
            {
                if (3 != args.HandlersCalledCount)
                {
                    throw new Microsoft.Test.TestValidationException ("Incorrect HandlersCalledCount, expected: 5, actually: " + args.HandlersCalledCount);
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
        #endregion


        #region Private Members
        private class UIElementContinue : CustomUIElement
        {
            public UIElementContinue (UIElement toContinue2 ):base()
            {
                this._toContinue2 = toContinue2;
            }


            protected override DependencyObject GetUIParentCore()
            {
                return _toContinue2;
            }

            UIElement _toContinue2;
        }
        #endregion
    }
}

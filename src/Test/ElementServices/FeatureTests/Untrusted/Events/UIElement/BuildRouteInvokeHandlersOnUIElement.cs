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
    /// Add instance handler for bubble event onto Control, build the route and invoke handlers
    /// BuildRoute and InvokeHandler are internal. To keep coverage, keep this test case to use RaiseEvent.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  BuileRouteInvokeHandlersOnUIElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new Control</li>
    /// <li>Add 3 handlers onto the Control, HandedEventToo=false</li>
    /// <li>Create a EventRoute and a RoutedEventArgs</li>
    /// <li>Check RoutedEventArgs has been set correctly</li>
    /// <li>Initialize the RoutedEventArgs with the RoutedEvent and the Control as source</li>
    /// <li>Build Route</li>
    /// <li>Check RoutedEventArgs again</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// <li>Check RoutedEventArgs again</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "BuileRouteInvokeHandlersOnUIElement")]
    public class BuileRouteInvokeHandlersOnUIElement : TestCase
    {
        #region Private Data
        private int _executedCount;
        #endregion


        #region Constructor
        public BuileRouteInvokeHandlersOnUIElement()
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
            CoreLogger.LogStatus ("This is a test case adding instance handlers onto UIElement");

            Dispatcher context = MainDispatcher;

            CustomControl myControl = null;
            RoutedEvent bubbleEvent = null;
            RoutedEventArgs myArgs = null;
            EventRoute myRoute = null;

            using (CoreLogger.AutoStatus ("Creating a new Control"))
            {
                myControl = new CustomControl();
            }

            using (CoreLogger.AutoStatus ("Creating a new Bubble Event Name"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomControl));
            }

            using (CoreLogger.AutoStatus ("Adding Handlers"))
            {
                myControl.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myControl.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myControl.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
            }

            using (CoreLogger.AutoStatus ("Building args and Route"))
            {
                myArgs  = new RoutedEventArgs (bubbleEvent, myControl);
                myRoute = new EventRoute (bubbleEvent);
            }

            using (CoreLogger.AutoStatus ("Check Args"))
            {
                if (false != myArgs.Handled)
                {
                    throw new Microsoft.Test.TestValidationException ("Handled should be false");
                }
                if (bubbleEvent != myArgs.RoutedEvent)
                {
                    throw new Microsoft.Test.TestValidationException ("Handled should be false");
                }
                if (myControl != myArgs.Source)
                {
                    throw new Microsoft.Test.TestValidationException ("not set source correctly");
                }
                if (myControl != myArgs.OriginalSource)
                {
                    throw new Microsoft.Test.TestValidationException ("not set Original source correctly");
                }
            }
            

            using (CoreLogger.AutoStatus ("Invoking Handlers"))
            {
                myControl.RaiseEvent(myArgs);
            }

            using (CoreLogger.AutoStatus ("Checking times handled"))
            {
                if (3 != _executedCount)
                    throw new Microsoft.Test.TestValidationException ("Should be handed only once, actually run: " + _executedCount.ToString () + "times");
            }
            using (CoreLogger.AutoStatus ("Checking handled args"))
            {
                if (myArgs.Source != myControl)
                    throw new Microsoft.Test.TestValidationException ("Args.Source has not been set correctly");

                if (myArgs.OriginalSource != myControl)
                    throw new Microsoft.Test.TestValidationException ("Args.OriginalSource has not been set correctly");

                if (myArgs.RoutedEvent != bubbleEvent)
                    throw new Microsoft.Test.TestValidationException ("Args.RoutedEvent has not been set correctly");

                if (myArgs.Handled != false)
                    throw new Microsoft.Test.TestValidationException ("Args.Handled has not been set correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Private Members
        private void MyHandler (object sender, RoutedEventArgs args)
        {
            _executedCount++;
        }
        #endregion
    }
}

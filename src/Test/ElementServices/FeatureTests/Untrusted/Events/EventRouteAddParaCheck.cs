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
    /// Checking Validation for EventRoute.Add()
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventRouteAddParaCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    ///     checking null Target EventRoute.Add
    ///     checking null handler for EventRoute.Add
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    
    [Test(0, "Events", "EventRouteAddParaCheck")]
    public class EventRouteAddParaCheck : TestCase
    {
        #region Private Data
            private void MyHandler (object sender, RoutedEventArgs args)
            {
            }
        #endregion

        #region Constructor
        public EventRouteAddParaCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case checking the exception for EventRoute.Add()");

            Dispatcher context = MainDispatcher;

            UIElement myUIElement = null;
            RoutedEvent myEvent = null;
            EventRoute myRoute = null;
            Exception ExpThrown = null;

            using (CoreLogger.AutoStatus("Create a UIElement, an UIElement and a RoutedEvent"))
            {
                myUIElement = new UIElement ();
                myEvent = EventManager.RegisterRoutedEvent ("ksdlki390973ssjfl ", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
            }

            using (CoreLogger.AutoStatus("Setup a route"))
            {
                myRoute = new EventRoute (myEvent);
            }

            using (CoreLogger.AutoStatus("Null source for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    myRoute.Add (null, new RoutedEventHandler (MyHandler), true);
                }
                catch (ArgumentNullException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Not check null Target for EventRoute.Add() correctly");
            }

            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    myRoute.Add (myUIElement, null, true);
                }
                catch (ArgumentNullException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Not check null args, handler, for EventRoute.Add correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

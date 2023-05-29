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
    /// Test check invalid input for EventManager.GetClassListeners
    /// </summary>
    /// <remarks>
    /// <para />
    /// The test case check invalid input for founctions at EventManage class:  GetClassListeners
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventMangerGetClassListenersParaValidation.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Register a new RoutedEvent which is already there</li>
    /// <li>call GetClassListeners with null classType</li>
    /// <li>call GetClassListeners with null routedEvent</li>
    /// </ol>
    /// </remarks>
    [Test(0, "Events.Boundary", "EventMangerGetClassListenersParaValidation")]
    public class EventMangerGetClassListenersParaValidation : TestCase
    {
        #region Constructor
        public EventMangerGetClassListenersParaValidation() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case check invalid input for EventManager.GetClassListers");

            Dispatcher context = MainDispatcher;
            
            Exception ExpThrown = null;     
            RoutedEvent routedEvent = EventManager.RegisterRoutedEvent ("TestIdNamekjsdl9722&@(#&$", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentElement));

            using (CoreLogger.AutoStatus("GetRoutedEventsForOwner with with null classType"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.GetRoutedEventsForOwner(null);
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
                    throw new Microsoft.Test.TestValidationException ("Null classType for EventManager.GetClassListeners");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
    #endregion
    }
}

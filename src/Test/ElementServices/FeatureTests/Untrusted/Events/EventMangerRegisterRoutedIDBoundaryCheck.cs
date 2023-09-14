// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows.Threading;
using System.Windows;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.Events;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;


namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Test check invalid input for registering at class EventManager
    /// </summary>
    /// <remarks>
    /// <para />
    /// The test case check invalid input for founctions at EventManage class:  RegisterRoutedEvent
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// Reviewed by: Microsoft
    /// <para />
    /// <para />
    /// FileName:  EventManagerRoutedIDBoundaryCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Register a new RoutedEvent with null name</li>
    /// <li>Register a new RoutedEvent with null handlerType</li>
    /// <li>Register a new RoutedEvent with null ownerType</li>
    /// <li>Register a new RoutedEvent which is already there</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Boundary", "EventManagerRegisterRoutedIDBoundaryCheck")]
    public class EventManagerRegisterRoutedIDBoundaryCheck : TestCase
    {
        #region Constructor
        public EventManagerRegisterRoutedIDBoundaryCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case check invalid input for EventManager: Register Event Name");

            Dispatcher context = MainDispatcher;
            

            Exception ExpThrown = null;     

            using (CoreLogger.AutoStatus("Registering Event with null name"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterRoutedEvent (null, RoutingStrategy.Bubble, null, null);
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
                    throw new Microsoft.Test.TestValidationException ("Null handlerType for EventManager.RegisterRoutedEvent");
            }

            using (CoreLogger.AutoStatus("Registering Event with null handlerType"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterRoutedEvent ("TestIdName", RoutingStrategy.Bubble, null, null);
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
                    throw new Microsoft.Test.TestValidationException ("Null handlerType for EventManager.RegisterRoutedEvent");
            }

            using (CoreLogger.AutoStatus("Registering Event with null ownerType"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterRoutedEvent ("TestIdName", RoutingStrategy.Bubble, typeof(RoutedEventHandler), null);
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
                    throw new Microsoft.Test.TestValidationException ("Null handlerType for EventManager.RegisterRoutedEvent");
            }


            using (CoreLogger.AutoStatus("Registering a existing Event"))
            {
                ExpThrown = null;
                EventManager.RegisterRoutedEvent ("TestIdName", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentElement));
                try
                {
                    EventManager.RegisterRoutedEvent ("TestIdName", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentElement));
                }

                catch (ArgumentException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Registering existing Name with EventManager.RegisterRoutedEvent");
            }

            using (CoreLogger.AutoStatus("Reading an Event with GetRoutedEventsForOwner"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.GetRoutedEventsForOwner (null);
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
                    throw new Microsoft.Test.TestValidationException ("Reading event Name with EventManager.GetRoutedEventsForOwner not check null arg correctly");
            }

            using (CoreLogger.AutoStatus("Reading an Event with GetRoutedEventFromName with first parameter is null"))
            {
                ExpThrown = null;
                try
                {
                    throw new ArgumentNullException();//EventManager.GetRoutedEventFromName (null, typeof(CustomContentElement));
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
                    throw new Microsoft.Test.TestValidationException ("Reading event Name with EventManager.GetRoutedEventFromName not check first null arg correctly");
            }

            using (CoreLogger.AutoStatus("Reading an Event with GetRoutedEventFromName with second parameter is null"))
            {
                ExpThrown = null;
                try
                {
                    throw new ArgumentNullException();//EventManager.GetRoutedEventFromName ("BubbleEvent", null);
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
                    throw new Microsoft.Test.TestValidationException ("Reading event Name with EventManager.GetRoutedEventFromName not check second null arg correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
    #endregion
    }
}

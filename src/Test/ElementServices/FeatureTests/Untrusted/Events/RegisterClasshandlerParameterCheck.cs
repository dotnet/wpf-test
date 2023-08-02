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
    /// Tests Checking invalid input for EventManager.RegisterClassHandler 
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a non-BVT scenario for Class handler
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RegisterClasshandlerParameterCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>add a class handler with null classtype</li>
    /// <li>add a class handler with null Event</li>
    /// <li>add a class handler with null handler</li>
    /// <li>add a class handler with non match handler type</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Boundary", "RegisterClasshandlerParameterCheck")]
    public class RegisterClasshandlerParameterCheck : TestCase
    {
        #region data
            /// <summary>
            /// MyContentElement
            /// </summary>
            /// <remarks>
            ///A subclass of ContentElement to avoid confliction 
            ///We can add class handler to a class, but we cannot remove it
            ///So, to avoid this scenrio to effect the rest of test cases, we use 
            ///a subclass.
            ///</remarks>
            private class MyContentElement : ContentElement
            {
                /// <summary>
                /// Constructor for MyContentElement
                /// </summary>
                /// <remarks>Just Pass it to base
                /// </remarks>
                public MyContentElement ():base()
                {
                }
            }
            private void MyHandler (object sender, RoutedEventArgs args) { }

            private delegate void MyRoutedEventHandler (object sender, RoutedEventArgs args);
        #endregion


        #region Constructor
        public RegisterClasshandlerParameterCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a BVT scenario for attaching bubble event to a content element, add two class handlers, call getRoutedEventIDsforOwner, call getRoutedEventIDFromName, invoke event");

            RoutedEvent bubbleEvent;

            using (CoreLogger.AutoStatus("Registering Event"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyContentElement));
            }
            
            Exception ExpThrown = null;

            using (CoreLogger.AutoStatus("Register with null class type"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterClassHandler (null, bubbleEvent, new RoutedEventHandler(MyHandler), true);
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
                    throw new Microsoft.Test.TestValidationException ("Registering class handler with first parameter is null doesn't throw expected exception");
            }

            using (CoreLogger.AutoStatus("Register with null event Name"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterClassHandler (typeof(MyContentElement), null, new RoutedEventHandler (MyHandler), true);
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
                    throw new Microsoft.Test.TestValidationException ("Registering class handler with second parameter is null doesn't throw expected exception");
            }

            using (CoreLogger.AutoStatus("Register with null handler type"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterClassHandler (typeof(MyContentElement), bubbleEvent, null, true);
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
                    throw new Microsoft.Test.TestValidationException ("Registering class handler with Thread parameter is null doesn't throw expected exception");
            }

            using (CoreLogger.AutoStatus("Register with unmatched handler type"))
            {
                ExpThrown = null;
                try
                {
                    EventManager.RegisterClassHandler (typeof(MyContentElement), bubbleEvent, new MyRoutedEventHandler (MyHandler), true);
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
                    throw new Microsoft.Test.TestValidationException ("Registering class handler with unmatched handler is null doesn't throw expected exception");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}


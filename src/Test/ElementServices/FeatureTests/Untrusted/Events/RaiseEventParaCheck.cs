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
    /// Raise Event with a none Event Name.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RaiseEventParaCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    ///     checking null args for RaiseEvent on ContentElement
    ///     checking null args for RaiseEvent on UIElement
    ///     checking the exception for setSource before Event is set. 
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Boundary", "RaiseEventParaCheck")]
    public class RaiseEventParaCheck : TestCase
    {
        #region Constructor
        public RaiseEventParaCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case checking the exception for creating route before set the Event Name");

            Dispatcher context = MainDispatcher;

            UIElement myUIElement = null;
            CustomContentElement myContentElement = null;
            using (CoreLogger.AutoStatus("Creating new objects"))
            {
                myUIElement = new UIElement ();
                myContentElement = new CustomContentElement();
            }

            Exception ExpThrown = null;

            using (CoreLogger.AutoStatus("Null args for content element"))
            {
                ExpThrown = null;

                RoutedEventArgs myArgs = new RoutedEventArgs ();

                try
                {
                    myContentElement.RaiseEvent (null);
                }
                catch (ArgumentNullException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Null args for UI Element"))
            {
                ExpThrown = null;

                RoutedEventArgs myArgs = new RoutedEventArgs ();

                try
                {
                    myUIElement.RaiseEvent (null);
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
                    throw new Microsoft.Test.TestValidationException ("Not check null args for RaiseEvent correctly");
            }
            using (CoreLogger.AutoStatus("Setting source before assign a event Name"))
            {
                ExpThrown = null;
                RoutedEventArgs myArgs = new RoutedEventArgs ();
                try
                {
                    myUIElement.RaiseEvent(myArgs);
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
                    throw new Microsoft.Test.TestValidationException ("Not correct exception for set Source");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion ExternalAPI
    }
}

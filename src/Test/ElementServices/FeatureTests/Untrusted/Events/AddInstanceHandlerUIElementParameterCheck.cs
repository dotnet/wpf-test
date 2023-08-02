// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Add instance handler for bubble event onto content element and raise event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AddInstanceHandlerUIElementParameterCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new Content Element</li>
    /// <li>Add 3 handlers onto the UI element, HandedEventToo=false</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Boundary", "AddInstanceHandlerUIElementParameterCheck")]
    public class AddInstanceHandlerUIElementParameterCheck : TestCase
    {
        #region Private Data
        private void MyHandler (object sender, RoutedEventArgs args) { }
        private delegate void MyRoutedEventHandler (object sender, RoutedEventArgs args);
        #endregion


        #region Constructor
        /******************************************************************************
        * Function:          AddInstanceHandlerContentElementParameterCheck Constructor
        ******************************************************************************/
        public AddInstanceHandlerUIElementParameterCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case adding instance handlers onto UIElement");

            Dispatcher context = MainDispatcher;

            UIElement myUIElement = null;
            RoutedEvent bubbleEvent = null;

            using (CoreLogger.AutoStatus("Creating a new UI element"))
            {
                myUIElement = new UIElement ();
            }

            using (CoreLogger.AutoStatus("Creating a new Bubble Event Name"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
            }

            Exception ExpThrown;

            using (CoreLogger.AutoStatus("Add handler with null Name args"))
            {
                ExpThrown = null;
                try
                {
                    myUIElement.AddHandler (null, null, false);
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
                    throw new Microsoft.Test.TestValidationException ("Not check first arg null for addHandler in UIElement correctly");
            }

            using (CoreLogger.AutoStatus("Add handler with null handler argument"))
            {
                ExpThrown = null;
                try
                {
                    myUIElement.AddHandler (bubbleEvent, null, false);
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
                    throw new Microsoft.Test.TestValidationException ("Not check second arg null for addHandler in UIElement correctly");
            }

            using (CoreLogger.AutoStatus("Check Handler type"))
            {
                ExpThrown = null;
                try
                {
                    myUIElement.AddHandler (bubbleEvent, new MyRoutedEventHandler (MyHandler), false);
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
                    throw new Microsoft.Test.TestValidationException ("Not check whether the handler type is correct correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion
    }
}

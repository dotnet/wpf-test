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
    /// Add instance handler for bubble event onto UI element and raise event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RaiseEventOnUIElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new UI Element</li>
    /// <li>Add 3 handlers onto the UI element, HandedEventToo=false</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// <li>Check args has been modified correctly</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "RaiseEventOnUIElement")]
    public class RaiseEventOnUIElement : TestCase
    {
        #region Private Data
        private int _executedCount;
        #endregion data


        #region Constructor
        public RaiseEventOnUIElement() :base(TestCaseType.ContextSupport)
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

            UIElement myUIElement = null;
            RoutedEvent bubbleEvent = null;
            RoutedEventArgs myArgs = null;  

            using (CoreLogger.AutoStatus ("Creating a new UI element"))
            {
                myUIElement = new UIElement ();
            }

            using (CoreLogger.AutoStatus ("Creating a new Bubble Event Name"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
            }

            using (CoreLogger.AutoStatus ("Adding Handlers"))
            {
                myUIElement.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myUIElement.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myUIElement.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
            }

            using (CoreLogger.AutoStatus ("Raising Event"))
            {
                myArgs = new RoutedEventArgs(bubbleEvent, myUIElement);
                myUIElement.RaiseEvent(myArgs);
            }

            using (CoreLogger.AutoStatus ("Checking handledness"))
            {
                if (3 != _executedCount)
                    throw new Microsoft.Test.TestValidationException ("Should be handed only once, actually run: " + _executedCount.ToString () + "times");
            }

            using (CoreLogger.AutoStatus ("Checking handled args"))
            {
                if (myArgs.Source != myUIElement)
                    throw new Microsoft.Test.TestValidationException ("Args.Source has not been set correctly");

                if (myArgs.OriginalSource != myUIElement)
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

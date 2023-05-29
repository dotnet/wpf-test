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
    /// Check the invalid inputs for RoutedEventArgs.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RoutedEventArgsParaCheck.cs
    /// <para />
    /// <ol>Scenarios covered:
    ///     Create an args
    ///     Source with null Event
    ///     Registe Event Name
    ///     RoutedEvent
    ///     Source
    ///     Read RoutedEvent, handled, Source, OriginalSource
    ///     Set handled
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
   [Test(0, "Events.Boundary", "RoutedEventArgsParaCheck")]
    public class RoutedEventArgsParaCheck : TestCase
    {
        #region Constructor
        public RoutedEventArgsParaCheck() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("This is a test case checking the exception for RoutedEventArgs");

            Dispatcher context = MainDispatcher;
            

            UIElement myUIElement = null;
            RoutedEventArgs myArgs = null;
            RoutedEvent myEvent = null;

            Exception ExpThrown = null;

            using (CoreLogger.AutoStatus("Create a args and a UIElement"))
            {
                myArgs = new RoutedEventArgs ();
                myUIElement = new UIElement ();
            }

            using (CoreLogger.AutoStatus("Null Event for Source?"))
            {
                ExpThrown = null;
                try
                {
                    myArgs.Source = myUIElement;
                }
                catch (InvalidOperationException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Not check null Event for RoutedEventArgs.Source correctly");
            }

            using (CoreLogger.AutoStatus("Setup event Name"))
            {
                myEvent = EventManager.RegisterRoutedEvent ("TestIdName77jjkdsajk", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(UIElement));
                myArgs.RoutedEvent = myEvent;
            }

            using (CoreLogger.AutoStatus("Source"))
            {
                myArgs.Source = myUIElement;
            }

            using (CoreLogger.AutoStatus("Reading out"))
            {
                bool myHandled = myArgs.Handled;
                Object mySource = myArgs.Source;
                Object myOriginalSource = myArgs.OriginalSource;
                RoutedEvent myID = myArgs.RoutedEvent;

                if (myHandled != false || mySource != myUIElement || myOriginalSource != myUIElement || myID != myEvent)
                    throw new Microsoft.Test.TestValidationException ("RoutedEventArgs read out wrong info");
            }
            using (CoreLogger.AutoStatus("Set handled"))
            {
                myArgs.Handled = true;
                if (true != myArgs.Handled)
                    throw new Microsoft.Test.TestValidationException ("Handled not set correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

        private void voidHandler (object sender, RoutedEventArgs args) { }
    }
}

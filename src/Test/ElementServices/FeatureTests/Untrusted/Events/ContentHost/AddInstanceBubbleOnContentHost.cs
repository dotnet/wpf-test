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
    /// Add instance handler for bubble event onto ContentHost and raise event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentHost
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AddInstanceBubbleOnContentHost.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new Content Host</li>
    /// <li>Add 3 handlers onto the content host, HandedEventToo=false</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentHost", "AddInstanceBubbleOnContentHost")]
    public class AddInstanceBubbleOnContentHost : TestCase
    {
        #region data
        private int _executedCount;
        #endregion data


        #region Constructor
        public AddInstanceBubbleOnContentHost() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a test case adding instance handlers onto ContentHost");

            Dispatcher context = MainDispatcher;
            

            CustomContentHost myContentHost = null;
            RoutedEvent bubbleEvent = null;

            using (CoreLogger.AutoStatus ("Creating a new content host"))
            {
                myContentHost = new CustomContentHost();
            }

            using (CoreLogger.AutoStatus ("Creating a new Bubble Event Name"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentHost));
            }

            using (CoreLogger.AutoStatus ("Adding Handlers"))
            {
                myContentHost.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myContentHost.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
                myContentHost.AddHandler (bubbleEvent, new RoutedEventHandler (MyHandler), false);
            }

            using (CoreLogger.AutoStatus ("Raising Event"))
            {
                myContentHost.RaiseEvent ( new RoutedEventArgs(bubbleEvent,myContentHost));
            }

            using (CoreLogger.AutoStatus ("Checking handledness"))
            {
                if (3 != _executedCount)
                    throw new Microsoft.Test.TestValidationException ("Should be handed only once, actually run: " + _executedCount.ToString () + "times");
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

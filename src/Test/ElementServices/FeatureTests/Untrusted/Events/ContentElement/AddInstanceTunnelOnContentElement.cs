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
    /// Add instance handler for tunnel event onto content element and Raise the event
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AddInstanceTunnelOnContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a RoutedEvent</li>
    /// <li>Create a new Content Element</li>
    /// <li>Add 3 handlers onto the content element, HandedEventToo=false</li>
    /// <li>Raise Event and check the times handle runs</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "AddInstanceTunnelOnContentElement")]
    public class AddInstanceTunnelOnContentElement : TestCase
    {
        #region Data
        private int _executedCount;
        #endregion data


        #region Constructor
        public AddInstanceTunnelOnContentElement() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a test case adding instance handlers for tunnel event onto ContentElement");

            Dispatcher context = MainDispatcher;
            

            CustomContentElement myContentElement = null;
            RoutedEvent tunnelEvent = null;

            using (CoreLogger.AutoStatus ("Creating a new content element"))
            {
                myContentElement = new CustomContentElement();
            }

            using (CoreLogger.AutoStatus ("Creating a new tunnel Event Name"))
            {
                tunnelEvent = EventManager.RegisterRoutedEvent ("TunnelRoutedEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CustomContentElement));
            }

            using (CoreLogger.AutoStatus ("Adding Handlers"))
            {
                myContentElement.AddHandler (tunnelEvent, new RoutedEventHandler (MyHandler), false);
                myContentElement.AddHandler (tunnelEvent, new RoutedEventHandler (MyHandler), false);
                myContentElement.AddHandler (tunnelEvent, new RoutedEventHandler (MyHandler), false);
            }

            using (CoreLogger.AutoStatus ("Raising Event"))
            {
                myContentElement.RaiseEvent (new RoutedEventArgs (tunnelEvent,myContentElement));
            }

            using (CoreLogger.AutoStatus ("Checking raised times"))
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

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
    /// Tests GetRoutedEvents() for UIElement
    /// <para />
    /// This is a BVT scenario Check GetRoutedEventsForOwner when there is not one
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  GetRoutedEventIDsForOwnerWhenNon.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEventIDs using GetRoutedEventsForOwner(), check whether the returned value is null</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.Boundary", "GetRoutedEventIDsForOwnerWhenNon")]
    public class GetRoutedEventIDsForOwnerWhenNon : TestCase
    {
        #region Constructor
        public GetRoutedEventIDsForOwnerWhenNon() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus("GetRoutedEventsForOwner for int");

            // Get Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            using (CoreLogger.AutoStatus("Fetch RoutedEvent When There is not"))
            {
                RoutedEvent[] routedEventIDs = EventManager.GetRoutedEventsForOwner (typeof(int));
                if (routedEventIDs != null)
                    throw new Microsoft.Test.TestValidationException ("RoutedEventIDFromName should return null by now");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }    
        #endregion
    }
}



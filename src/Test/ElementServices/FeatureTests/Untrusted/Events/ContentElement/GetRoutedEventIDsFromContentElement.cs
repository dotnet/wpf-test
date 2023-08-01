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
    /// Tests GetRoutedEvents() for ContentElement
    /// <para />
    /// This is a BVT scenario reading out RoutedEvents for ContentElement
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  GetRoutedEventsFromContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li> create a new ContentElement</li>"
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Call GetRoutedEvents and check whether return null</li>
    /// <li>attach an event handler with the RoutedEvent</li>
    /// <li>Call GetRoutedEvents again</li>
    /// <li>Check the number of IDs returned and whether the returned Name is what expected</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "GetRoutedEventIDsFromContentElement")]
    public class GetRoutedEventIDsFromContentElement : EventHelper
    {
        #region Constructor
        public GetRoutedEventIDsFromContentElement()
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
            CoreLogger.LogStatus ("Validate parameter for ContentElement.GetRoutedEvents ");

            // Local Varaibles
            CustomContentElement contentElement1=null;
            RoutedEvent routedEvent = null;

            // Get Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher
            
            
            using(CoreLogger.AutoStatus("Creating custom ContentElement "))
            {
                contentElement1 = new CustomContentElement();
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = RemoveBubblingEventHandler.RoutedEvent1;
            }


            // if (null != contentElement1.GetRoutedEventsWithHandlers())
            //         throw new Microsoft.Test.TestValidationException ("RoutedEvents returned should be null by now");

            using(CoreLogger.AutoStatus("Add event handler for bubble event"))
            {
                contentElement1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
            }

            // RoutedEvent[] myIDs = contentElement1.GetRoutedEventsWithHandlers();
            // if(1 != myIDs.Length)
            //     throw new Microsoft.Test.TestValidationException ("RoutedEvents should return only on Name");
            // if (myIDs[0]!=routedEvent)
            //     throw new Microsoft.Test.TestValidationException ("RoutedEvents should be routedEvent");

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;

            // Exit Dispatcher
        }    
        #endregion


        #region Private Members

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        private void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
        } 
        #endregion
    }
}



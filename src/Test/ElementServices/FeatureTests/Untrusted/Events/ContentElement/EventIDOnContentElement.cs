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
    /// Register and read Event for non ContentElement 
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a BVT scenario for varifying Registering and reading Event Name on ContentElement 
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventOnContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a ContentElement object</li>
    /// Register 2 Events: bubble and tunnel
    /// <li>call eventManager.GetRoutedEvents and compare result</li>
    /// <li>call eventManager.GetRoutedEventsforOwner and compare result</li>
    /// <li>call eventManager.GetRoutedEventFromName and compare result</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "EventIDOnContentElement")]
    public class EventIDOnContentElement : TestCase
    {
        #region data
        CustomContentElement _myContentElement;
        #endregion data

        #region Constructor
        public EventIDOnContentElement() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a BVT scenario for rigistering 2 event IDs, calling GetRoutedEvents, calling getRoutedEventsforOwner, and calling getRoutedEventFromName");

            RoutedEvent bubbleEvent = null;
            RoutedEvent tunnelEvent = null;

            Dispatcher context = MainDispatcher;

            using (CoreLogger.AutoStatus ("Creating new Object"))
            {
                _myContentElement = new CustomContentElement();
            }

            using (CoreLogger.AutoStatus ("Registering Event"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomContentElement));
                tunnelEvent = EventManager.RegisterRoutedEvent ("TunnelRoutedEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CustomContentElement));
            }

            using (CoreLogger.AutoStatus ("Reading event Name"))
            {
                RoutedEvent[] IDs = EventManager.GetRoutedEvents ();
                bool find1 = false, find2=false;
                RoutedEvent theEvent;

                for (int i = 0; i < IDs.Length; i++)
                {
                    if (IDs[i].Name.Equals ("BubbleRoutedEvent") && IDs[i].RoutingStrategy == RoutingStrategy.Bubble)
                    {
                        find1 = true;
                    }
                    if (IDs[i].Name.Equals ("TunnelRoutedEvent") && IDs[i].RoutingStrategy == RoutingStrategy.Tunnel)
                    {
                        find2 = true;
                    }
                }

                if (!find1 || !find2)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }

                find1 = find2 = false;

                IDs = EventManager.GetRoutedEventsForOwner (typeof(CustomContentElement));
                if (IDs != null)
                {
                    if(2 != IDs.Length)
                    {
                       throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                    }
                    for (int i = 0; i < IDs.Length; i++)
                    {
                        if (IDs[i].Name.Equals ("BubbleRoutedEvent") && IDs[i].RoutingStrategy == RoutingStrategy.Bubble)
                        {
                            find1 = true;
                        }

                        if (IDs[i].Name.Equals ("TunnelRoutedEvent") && IDs[i].RoutingStrategy == RoutingStrategy.Tunnel)
                        {
                           find2 = true;
                        }
                    }
                }

                if (!find1 || !find2)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }
    
                theEvent = bubbleEvent;

                if(theEvent == null)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }

                if (!(theEvent.Name.Equals ("BubbleRoutedEvent") && 
                    theEvent.RoutingStrategy == RoutingStrategy.Bubble))
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");

                theEvent = tunnelEvent;
                if (theEvent == null)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }

                if (!(theEvent.Name.Equals ("TunnelRoutedEvent") && theEvent.RoutingStrategy == RoutingStrategy.Tunnel))
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

    }
}



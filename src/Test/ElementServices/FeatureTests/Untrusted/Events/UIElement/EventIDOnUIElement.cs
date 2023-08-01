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
    /// Register and read Event for  CustomControl 
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a BVT scenario for varifying Registering and reading Event Name on  CustomControl type
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventOnUIElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a CustomControl object</li>
    /// Register 2 Events: bubble and tunnel
    /// <li>call eventManager.GetRoutedEvents and compare result</li>
    /// <li>call eventManager.GetRoutedEventsforOwner and compare result</li>
    /// <li>call eventManager.GetRoutedEventFromName and compare result</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "EventIDOnUIElement")]
    public class EventIDOnUIElement : TestCase
    {
        #region data
        CustomControl _myCustomControl = null;
        #endregion data

        #region Constructor
        public EventIDOnUIElement() :base(TestCaseType.ContextSupport)
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

            using (CoreLogger.AutoStatus ("Creating new CustomControl"))
            {
                _myCustomControl = new CustomControl();
            }

            using (CoreLogger.AutoStatus ("Registering Event"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(CustomControl));
                tunnelEvent = EventManager.RegisterRoutedEvent ("TunnelRoutedEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(CustomControl));
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

                IDs = EventManager.GetRoutedEventsForOwner (typeof(CustomControl));
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
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion 
    }
}



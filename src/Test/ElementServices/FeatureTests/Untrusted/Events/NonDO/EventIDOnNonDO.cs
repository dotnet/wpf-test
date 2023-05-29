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
    /// Register and read Event for non Dependency Object Type 
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a BVT scenario for varifying Registering and reading Event Name on non-dependency object type
    /// <para />
    /// Area: Events\NonDO
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  EventOnNonDO.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a Non dependency object type object</li>
    /// Register 2 Events
    /// <li>call eventManager.GetRoutedEvents and compare result</li>
    /// <li>call eventManager.GetRoutedEventsforOwner and compare result</li>
    /// <li>call eventManager.GetRoutedEventFromName and compare result</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.NonDO", "EventIDOnNonDO")]
    public class EventIDOnNonDO : TestCase
    {
        #region Private Data
        private MyObject _myObject;
        #endregion


        #region Constructor
        public EventIDOnNonDO() :base(TestCaseType.ContextSupport)
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
                _myObject = new MyObject ();
            }

            using (CoreLogger.AutoStatus ("Registering Event"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyObject));
                tunnelEvent = EventManager.RegisterRoutedEvent ("TunnelRoutedEvent", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MyObject));
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

                IDs = EventManager.GetRoutedEventsForOwner (typeof(MyObject));
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


        #region Private Members
        /// <summary>
        /// MyObject
        /// </summary>
        /// <remarks>
        ///A subclass of object to avoid confliction 
        ///We can add class handler to a class, but we cannot remove it
        ///So, to avoid this scenrio to effect the rest of test cases, we use 
        ///a subclass.
        ///</remarks>
        private class MyObject : Object
        {
            /// <summary>
            /// Constructor for MyObject
            /// </summary>
            /// <remarks>Just Pass it to base
            /// </remarks>
            public MyObject ():base()
            {
            }
        }
        #endregion
    }
}



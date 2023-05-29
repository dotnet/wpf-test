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
    /******************************************************************************
    * CLASS:          CheckContentElement4ClassHandler
    ******************************************************************************/
    /// <summary>
    /// Tests Attaching Bubble EventHandlers to a class inheriented from ContentElement, MyContentElement,
    /// add 2 class handlers, 
    /// register a event, 
    /// raise the event and check.
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a BVT scenario for varify raising/handle event on content elements/// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// Reviewed by: Microsoft
    /// <para />
    /// <para />
    /// FileName:  CheckContentElement4ClassHandler.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>new a content element</li>
    /// <li>add a class handler with handledEventsToo=true</li>
    /// <li>add the same handler for same event with handledEventsToo=true</li>
    /// <li>add the same handler for same event with handledEventsToo=false</li>
    /// <li>call eventManager.GetRoutedEvents</li>
    /// <li>call eventManager.GetRoutedEventsforOwner</li>
    /// <li>call eventManager.GetRoutedEventFromName</li>
    /// <li>Raise event</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "CheckContentElement4ClassHandler")]
    public class CheckContentElement4ClassHandler : TestCase
    {

        #region data
        private int _executedCount;
        MyContentElement _myContentElement;
        #endregion data


        #region Constructor
        public CheckContentElement4ClassHandler() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a BVT scenario for attaching bubble event to a content element, add two class handlers, call getRoutedEventsforOwner, call getRoutedEventFromName, invoke event");

            RoutedEvent BubbleEvent;

            Dispatcher context = MainDispatcher;

            using (CoreLogger.AutoStatus ("Creating new content element"))
            {
                _myContentElement = new MyContentElement ();
            }

            using (CoreLogger.AutoStatus ("Registering Event"))
            {
                BubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyContentElement));
            }

            using (CoreLogger.AutoStatus ("Registering 2 Class handlers"))
            {
                EventManager.RegisterClassHandler (typeof(MyContentElement), BubbleEvent, new RoutedEventHandler (OnRoutedEvent), true);
                EventManager.RegisterClassHandler (typeof(MyContentElement), BubbleEvent, new RoutedEventHandler (OnRoutedEvent), true);
                EventManager.RegisterClassHandler (typeof(MyContentElement), BubbleEvent, new RoutedEventHandler (OnRoutedEvent), false); 
            }

            using (CoreLogger.AutoStatus ("Reading event Name"))
            {
                RoutedEvent[] IDs = EventManager.GetRoutedEvents ();
                bool findIt = false;
                RoutedEvent theEvent;

                for (int i = 0; i < IDs.Length; i++)
                {
                    if (IDs[i].Name.Equals ("BubbleRoutedEvent") && IDs[i].RoutingStrategy == RoutingStrategy.Bubble)
                    {
                        findIt = true;

                        CoreLogger.LogStatus (IDs[i].OwnerType.FullName.ToString ());
                    }
                }

                if (!findIt)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }

            
                IDs = EventManager.GetRoutedEventsForOwner (typeof(MyContentElement));
                if (IDs != null)
                {
                    if(IDs.Length !=1)
                    {
                       throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                    }

                    if (IDs[0].Name.Equals ("BubbleRoutedEvent") && IDs[0].RoutingStrategy == RoutingStrategy.Bubble)
                        findIt = true;

                }

                if (!findIt)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }
    
                theEvent = BubbleEvent;

                if(theEvent == null)
                {
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
                }

                if (!(theEvent.Name.Equals ("BubbleRoutedEvent") && 
                    theEvent.RoutingStrategy == RoutingStrategy.Bubble))
                    throw new Microsoft.Test.TestValidationException ("Not Found Event Name");
            }

            using (CoreLogger.AutoStatus ("Raise Event"))
            {
                _myContentElement.RaiseEvent (new RoutedEventArgs (BubbleEvent, _myContentElement));
                if (ExecutedCount != 2)
                    CoreLogger.LogStatus ("Handler called: " + ExecutedCount);
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        ///<summary>
        ///The handler every time a handler is called: increase ExecutecdCount
        ///</summary>
        public void OnRoutedEvent (object sender, RoutedEventArgs args)
        {
            ExecutedCount++;
        }
        ///<summary>
        ///Property ExecutecdCount
        ///</summary>
        ///<remarks> An integer number to remember how many handlers are invoked
        ///</remarks>
        public int ExecutedCount
        {
            set
            {
                _executedCount = value;
            }
            get { return _executedCount; }
        }
        #endregion
    }

    /******************************************************************************
    * CLASS:          MyContentElement
    ******************************************************************************/
    /// <summary>
    /// MyContentElement
    /// </summary>
    /// <remarks>
    ///A subclass of ContentElement to avoid confliction 
    ///We can add class handler to a class, but we cannot remove it
    ///So, to avoid this scenrio to effect the rest of test cases, we use 
    ///a subclass.
    ///</remarks>
    public class MyContentElement : ContentElement
    {
        /// <summary>
        /// Constructor for MyContentElement
        /// </summary>
        /// <remarks>Just Pass it to base
        /// </remarks>

        public MyContentElement ():base()
        { 
        }
    }
}



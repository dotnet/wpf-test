// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Trusted;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Avalon.Test.CoreUI.Events
{
    /// <summary>
    /// Tests Attaching Bubble EventHandlers to a class inheriented from ContentElement, MyContentElement,
    /// </summary>
    /// <remarks>
    /// <para />
    /// This is a BVT scenario for Class handler
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// Dev: Microsoft
    /// <para />
    /// <para />
    /// FileName:  ContentElementClassHandler.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>new a content element</li>
    /// <li>add a class handler with handledEventsToo=true</li>
    /// <li>add the same handler for same event with handledEventsToo=true</li>
    /// <li>add the same handler for same event with handledEventsToo=true</li>
    /// <li>Raise event and check the times handler run</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "ContentElementClassHandler")]
    public class ContentElementClassHandler : TestCase
    {
        #region data
            private int _executedCount;
            MyContentElement _myContentElement;
            /// <summary>
            /// MyContentElement
            /// </summary>
            /// <remarks>
            ///A subclass of ContentElement to avoid confliction 
            ///We can add class handler to a class, but we cannot remove it
            ///So, to avoid this scenrio to effect the rest of test cases, we use 
            ///a subclass.
            ///</remarks>
            private class MyContentElement : ContentElement
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
        #endregion


        #region Constructor
        public ContentElementClassHandler() :base(TestCaseType.ContextSupport)
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

            RoutedEvent bubbleEvent;

            Dispatcher context = MainDispatcher;

            

            using (CoreLogger.AutoStatus ("Creating new content element"))
            {
                _myContentElement = new MyContentElement ();
            }

            
            using (CoreLogger.AutoStatus ("Registering Event"))
            {
                bubbleEvent = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyContentElement));
            }

            using (CoreLogger.AutoStatus ("Registering 2 Class handlers"))
            {
                EventManager.RegisterClassHandler (typeof(MyContentElement), bubbleEvent, new RoutedEventHandler (OnRoutedEvent), true);
                EventManager.RegisterClassHandler (typeof(MyContentElement), bubbleEvent, new RoutedEventHandler (OnRoutedEvent), true);
                EventManager.RegisterClassHandler (typeof(MyContentElement), bubbleEvent, new RoutedEventHandler (OnRoutedEvent), true); 
            }


            using (CoreLogger.AutoStatus ("Raise Event"))
            {
                _myContentElement.RaiseEvent (new RoutedEventArgs (bubbleEvent,_myContentElement));
                if (3 != ExecutedCount)
                    throw new Microsoft.Test.TestValidationException ("Incorrect number of class handlers: should be 3");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

        #region Operation
        ///<summary>
        ///The handler every time a handler is called: increase ExecutecdCount
        ///</summary>
        private void OnRoutedEvent (object sender, RoutedEventArgs args)
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

}



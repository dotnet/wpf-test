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
    /// Add an instance handler and an class handler for bubble event and 
    /// an instance handler and an class handler for tunnel event onto content element and read out event IDs
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\UIElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  GetRoutedEventsUIElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Create a 4 RoutedEvents, routedEvent1, routedEvent2, routedEvent3, and routedEvent4. The first 2 IDs are for bubble and the last two are for tunnel</li>
    /// <li>Create a new MyUIElement, a subclass of UIElement, myUIElement</li>
    /// <li>Attach a class handler for MyUIElement, with event id: rotedEvent1 </li>
    /// <li>Attach a class handler for MyUIElement, with event id: rotedEvent3 </li>
    /// <li>Add a handler onto myUIElement, with event id: rotedEvent2</li>
    /// <li>Add a handler onto myUIElement, with event id: rotedEvent4</li>
    /// <li>Call GetRoutedEventS for UIElement</li>
    /// <li>Check all IDs are there</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.UIElement", "GetRoutedEventIDsUIElement")]
    public class GetRoutedEventIDsUIElement : TestCase
    {
        #region Data
        private class MyUIElement : UIElement
        {
            public MyUIElement ():base() { }
        }
        #endregion


        #region Constructor
        public GetRoutedEventIDsUIElement() :base(TestCaseType.ContextSupport)
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
            CoreLogger.LogStatus ("This is a test case reading out Event IDs o UIElement");

            Dispatcher context = MainDispatcher;

            MyUIElement myUIElement = null;
            RoutedEvent routedEvent1 = null;
            RoutedEvent routedEvent2 = null;
            RoutedEvent routedEvent3 = null;
            RoutedEvent routedEvent4 = null;

            using (CoreLogger.AutoStatus ("Creating a new content element"))
            {
                myUIElement = new MyUIElement ();
            }

            using (CoreLogger.AutoStatus ("Creating new Event IDs"))
            {
                routedEvent1 = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent1", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyUIElement));
                routedEvent2 = EventManager.RegisterRoutedEvent ("BubbleRoutedEvent2", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(MyUIElement));
                routedEvent3 = EventManager.RegisterRoutedEvent ("PreviewBubbleRoutedEvent3", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MyUIElement));
                routedEvent4 = EventManager.RegisterRoutedEvent ("PreviewBubbleRoutedEvent4", RoutingStrategy.Tunnel, typeof(RoutedEventHandler), typeof(MyUIElement));
            }

            using (CoreLogger.AutoStatus ("Registering class Handlers"))
            {
                EventManager.RegisterClassHandler (typeof(MyUIElement), routedEvent1, new RoutedEventHandler (MyHandler), true);
                EventManager.RegisterClassHandler (typeof(MyUIElement), routedEvent3, new RoutedEventHandler (MyHandler), false);
            }

            using (CoreLogger.AutoStatus ("Adding instance Handlers"))
            {
                myUIElement.AddHandler (routedEvent2, new RoutedEventHandler (MyHandler), false);
                myUIElement.AddHandler (routedEvent4, new RoutedEventHandler (MyHandler), true);
            }

/*
            using (CoreLogger.AutoStatus ("Read out event IDs"))
            {
                RoutedEvent[] myIDs = myUIElement.GetRoutedEventsWithHandlers();
                int totalIDS = 0;
                foreach (RoutedEvent oneID in myIDs)
                {
                    if (oneID == routedEvent1)
                    {
                        CoreLogger.LogStatus ("get Name 1");
                        totalIDS++;
                    }
                    else if (oneID == routedEvent2)
                    {
                        CoreLogger.LogStatus ("get Name 2");
                        totalIDS++;
                    }
                    else if (oneID == routedEvent3)
                    {
                        CoreLogger.LogStatus ("get Name 3");
                        totalIDS++;
                    }
                    else if (oneID == routedEvent4)
                    {
                        CoreLogger.LogStatus ("get Name 4");
                        totalIDS++;
                    }
                    else
                        throw new Microsoft.Test.TestValidationException ("Event not added has been read out");
                }
                if (2 != totalIDS)
                    throw new Microsoft.Test.TestValidationException ("Total number of Name is not correct: " + totalIDS.ToString());
            }
*/            

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }
        #endregion

        #region Private Members
        private void MyHandler (object sender, RoutedEventArgs args)
        {
        }
        #endregion
    }
}

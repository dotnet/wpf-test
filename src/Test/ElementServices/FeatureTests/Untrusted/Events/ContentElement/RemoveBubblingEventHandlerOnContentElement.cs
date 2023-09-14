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
    /// Tests Remove Bubble EventHandler from ContentElement
    /// <para />
    /// This is a BVT scenario for removing bubble event from a simple tree with ContentElement
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RemoveBubblingEventHandlerOnContentElement.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Add event handlers for bubble event</li>
    /// <li>Raise the bubble event</li>
    /// <li>Handlers are called in the correct order</li>
    /// <li>Remove event handlers for bubble event</li>
    /// <li>Raise the bubble event</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events.ContentElement", "RemoveBubblingEventHandlerOnContentElement")]
    public class RemoveBubblingEventHandlerOnContentElement : EventHelper
    {
        #region Constructor
        public RemoveBubblingEventHandlerOnContentElement()
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
            CoreLogger.LogStatus("Simple tree with contentElement1 visual parent of contentElement2 visual parent of contentElement3");
            CoreLogger.LogStatus("Tests remove bubbling event from CustomContentElements");

            // Local Varaibles
            // Create three CustomContentElement to build a tree later and test Events
            CustomContentElement contentElement1=null;
            CustomContentElement contentElement2=null;
            CustomContentElement contentElement3=null;
            
            // Create a routedEvent to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent = null;
            

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args=null;

            // Get Dispatcher        
            Dispatcher context = MainDispatcher;
            // Create size of three object array to contain three CustomContentElements
            RouteTarget[] targets = new RouteTarget[3];

            // Enter Dispatcher
            
            
            using(CoreLogger.AutoStatus("Creating custom ContentElements "))
            {
                contentElement1 = new CustomContentElement();
                contentElement2 = new CustomContentElement();
                contentElement3 = new CustomContentElement();
            }
                
            using(CoreLogger.AutoStatus("Construct tree"))
            {
                contentElement1.AppendModelChild(contentElement2);
                contentElement2.AppendModelChild(contentElement3);
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = RemoveBubblingEventHandler.RoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add event handlers for bubble event"))
            {
                contentElement1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
                contentElement2.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent2));
                contentElement3.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent3));
            }
        
            using(CoreLogger.AutoStatus("Raise the bubble event on contentElement3"))
            {
                targets[0].Source = contentElement3;
                targets[0].Sender = contentElement3;
                targets[1].Source = contentElement3;
                targets[1].Sender = contentElement2;
                targets[2].Source = contentElement3;
                targets[2].Sender = contentElement1;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                contentElement3.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Validation for event"))
            {
                if (args.HandlersCalledCount != 3)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }
            
            using(CoreLogger.AutoStatus("Remove event contentElement3 handler for bubble event"))
            {
                contentElement1.RemoveHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
            }

            using(CoreLogger.AutoStatus("Raise the bubble event on contentElement3"))
            {
                targets[0].Sender = contentElement3;
                targets[0].Source = contentElement3;
                targets[1].Sender = contentElement2;
                targets[1].Source = contentElement3;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                contentElement3.RaiseEvent(args);

                if (args.HandlersCalledCount != 2)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }

            using(CoreLogger.AutoStatus("Remove event contentElement2 handler for bubble event"))
            {
                contentElement2.RemoveHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent2));
            }

            using(CoreLogger.AutoStatus("Raise the bubble event on contentElement3"))
            {
                targets[0].Source = contentElement3;
                targets[0].Sender = contentElement3;
                args = new CustomRoutedEventArgs(routedEvent, targets);
                contentElement3.RaiseEvent(args);

                if (args.HandlersCalledCount != 1)
                {
                    throw new Microsoft.Test.TestValidationException("Incorrect HandlersCalledCount");
                }
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        }    
        #endregion


        #region Public Members
        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 2);
        } 

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent2(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent2");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 1);
        } 

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent3(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent3");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 0);
        }
        #endregion
    }
}



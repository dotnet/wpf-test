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
    /// Tests Remove Bubble EventHandler from ContentElement
    /// <para />
    /// This is a BVT scenario for removing event handler from a ContentElement and check for invalid parameter
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\ContentElement
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RemoveHandlerOnContentElementParaValidation.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Remove event handler with invalid parameter and check</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
   
    [Test(0, "Events.Boundary", "RemoveHandlerOnContentElementParaValidation")]
    public class RemoveHandlerOnContentElementParaValidation : EventHelper
    {
        #region Constructor
        public RemoveHandlerOnContentElementParaValidation()
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
            CoreLogger.LogStatus("Validate parameter for COntentElement.Remove");

            // Local Varaibles
            CustomContentElement contentElement1=null;
            RoutedEvent routedEvent = null;

            // Get Dispatcher        
            Dispatcher context = MainDispatcher;

            
            using(CoreLogger.AutoStatus("Creating custom ContentElement "))
            {
                contentElement1 = new CustomContentElement();
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = RemoveBubblingEventHandler.RoutedEvent1;
            }

            using(CoreLogger.AutoStatus("Add event handler for bubble event"))
            {
                contentElement1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
            }
            Exception ExpThrown = null;
        
            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    contentElement1.RemoveHandler (null, new CustomRoutedEventHandler (OnRoutedEvent1));
                }
                catch (ArgumentNullException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Not check null RoutedEvent ContentElement.RemoveHandler correctly");
            }

            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    contentElement1.RemoveHandler (routedEvent, null);
                }
                catch (ArgumentNullException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("Not check null handler ContentElement.RemoveHandler correctly");
            }

            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    contentElement1.RemoveHandler (routedEvent, new AnotherEventHandler(OnRoutedEvent2));
                }
                catch (ArgumentException e)
                {
                    ExpThrown = e;
                }
                catch (Exception) { }
            }

            using (CoreLogger.AutoStatus("Validating the Argument exception is thrown"))
            {
                if (null == ExpThrown)
                    throw new Microsoft.Test.TestValidationException ("handler type is not same with the handler type RoutedEvent handling for ContentElement.RemoveHandler correctly");
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
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
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender and Source.
            this.VerifyRoutedEvent(sender, args, 2);
        } 

        /// <summary>
        /// Handler called
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        private void OnRoutedEvent2(object sender)
        {
        }
        private delegate void AnotherEventHandler (object sender);

        #endregion
    }
}



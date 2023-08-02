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
    /// Tests Remove Bubble EventHandler from UIElement
    /// <para />
    /// This is a BVT scenario for removing event handler from a UIElement and check for invalid parameter
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\Boundary
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  RemoveHandlerOnUIElementParaValidation.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble event</li>
    /// <li>Remove event handler with null RoutedEvent and check</li>
    /// <li>Remove event handler with null Handler and check</li>
    /// <li>Remove event handler with Handler type unmatch with handler type for the RoutedEvent and check</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
   
    [Test(0, "Events.Boundary", "RemoveHandlerOnUIElementParaValidation")]
    public class RemoveHandlerOnUIElementParaValidation : EventHelper
    {
        #region Constructor
        public RemoveHandlerOnUIElementParaValidation()
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
            CoreLogger.LogStatus("Validate parameter for UIElement.Remove");

            // Local Varaibles
            CustomUIElement uIElement1=null;
            RoutedEvent routedEvent = null;


            
            using(CoreLogger.AutoStatus("Creating custom UIElement "))
            {
                uIElement1 = new CustomUIElement();
            }
            
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble event"))
            {
                routedEvent = RemoveBubblingEventHandler.RoutedEvent1;
            }

            using(CoreLogger.AutoStatus("Add event handler for bubble event"))
            {
                uIElement1.AddHandler(routedEvent, new CustomRoutedEventHandler(OnRoutedEvent1));
            }
            Exception ExpThrown = null;
        
            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    uIElement1.RemoveHandler (null, new CustomRoutedEventHandler (OnRoutedEvent1));
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
                    throw new Microsoft.Test.TestValidationException ("Not check null RoutedEvent UIElement.RemoveHandler correctly");
            }

            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    uIElement1.RemoveHandler (routedEvent, null);
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
                    throw new Microsoft.Test.TestValidationException ("Not check null handler UIElement.RemoveHandler correctly");
            }

            using (CoreLogger.AutoStatus("Null args for EventRoute.Add"))
            {
                ExpThrown = null;
                try
                {
                    uIElement1.RemoveHandler (routedEvent, new AnotherEventHandler(OnRoutedEvent2));
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
                    throw new Microsoft.Test.TestValidationException ("handler type is not same with the handler type RoutedEvent handling for UIElement.RemoveHandler correctly");
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



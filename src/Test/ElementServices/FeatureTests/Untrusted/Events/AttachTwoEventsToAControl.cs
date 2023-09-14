// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Reflection;
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
    /******************************************************************************
    * CLASS:          AttachTwoEventsToAControl
    ******************************************************************************/
    /// <summary>
    /// Tests Attaching Bubble and Tunnel Events to a CustomControl
    /// <para />
    /// This is a BVT scenario for attaching bubble and tunnel event to a CustomControl and make sure events fire in order.
    /// </summary>
    /// <remarks>
    /// <para />
    /// Area: Events\AttachEvents
    /// <para />
    /// <para />
    /// <para />
    /// FileName:  AttachTwoEventsToAControl.cs
    /// <para />
    /// <ol>Scenarios covered:
    /// <li>Fetch RoutedEvent for bubble and tunnel event</li>
    /// <li>Add event handlers for bubble and tunnel event</li>
    /// <li>Raise the bubble and tunnel event</li>
    /// </ol>
    /// </remarks>
    /// <seealso cref="TestCaseType"/>
    [Test(0, "Events", "AttachTwoEventsToAControl")]
    public class AttachTwoEventsToAControl : EventHelper
    {
        #region Constructor
        public AttachTwoEventsToAControl()
        {
            RunSteps += new TestStep(StartTest);
        }
        #endregion

        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        TestResult StartTest()
        {
            CoreLogger.LogStatus("Tests Attach Two Bubbling Events To A Control.");

            // Local Varaibles
            // Create a CustomControl to test Events
            CustomControl control1=null;
            
            // Create routedEvent1 and previewroutedEvent1 to get EventManager.GetRoutedEventFromName
            RoutedEvent routedEvent1;
            RoutedEvent previewroutedEvent1;
            
            // Create size of one object array to contain the CustomControl
            RouteTarget[] targets = new RouteTarget[1];

            // Create a CustomRoutedEventArgs for later RaiseEvent use
            CustomRoutedEventArgs args;

            // Create Dispatcher        
            Dispatcher context = MainDispatcher;

            // Enter Dispatcher

            
            using(CoreLogger.AutoStatus("Creating a custom control"))
            {
                control1 = new CustomControl();
            }
                
            using(CoreLogger.AutoStatus("Fetch RoutedEvent for bubble and tunnel events"))
            {
                routedEvent1 = AttachTwoEventsToAControl.RoutedEvent1;
                previewroutedEvent1 = AttachTwoEventsToAControl.PreviewRoutedEvent1;
            }
            
            using(CoreLogger.AutoStatus("Add bubble and tunnle events to a control"))
            {
                control1.AddHandler(routedEvent1, new CustomRoutedEventHandler(OnRoutedEvent1));
                control1.AddHandler(previewroutedEvent1, new CustomRoutedEventHandler(OnPreviewRoutedEvent1));
            }
        
            using(CoreLogger.AutoStatus("Raise the bubble event"))
            {
                targets[0].Sender = control1;
                targets[0].Source = control1;
                args = new CustomRoutedEventArgs(routedEvent1, targets);
                control1.RaiseEvent(args);
            }

            using(CoreLogger.AutoStatus("Raise the tunnel event"))
            {
                args = new CustomRoutedEventArgs(previewroutedEvent1, targets);
                control1.RaiseEvent(args);
            }

            //Any test failures will be caught by throwing an Exception during verification.
            return TestResult.Pass;
        } 
   
        /******************************************************************************
        * Function:          OnRoutedEvent1
        ******************************************************************************/
        /// <summary>
        /// OnRoutedEvent1 method
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }

        /******************************************************************************
        * Function:          OnPreviewRoutedEvent1
        ******************************************************************************/
        /// <summary>
        /// OnPreviewRoutedEvent1 method
        /// </summary>
        /// <param name="sender">Pass the object to it</param>
        /// <param name="args">Pass the CustomRoutedEventArgs to it</param>
        public void OnPreviewRoutedEvent1(object sender, CustomRoutedEventArgs args)
        {
            CoreLogger.LogStatus("OnPreviewRoutedEvent1");

            // Verify sender, Source, and handler count.
            this.VerifyRoutedEvent(sender, args, 0);
        }
        #endregion
    }
}



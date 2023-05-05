// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Timeline container's BeginTime and Duration Properties
 */

//Instructions:
//  1. Create a Container with Begin = 100 , Duration = 1000
//  2. Attach Begun and Ended handlers to the Container
//  3. Create a TimeNode with Begin = 100 and Duration = 400
//  4. Attach Begun and Ended handlers to the TimeNode
//  5. Attach the TimeNode to Container
//  6. Create a Clock, associated with the Container
//  7. Start a TimeManager with Start=0 and Step = 100

//Warnings:
//  Any changes made in the output should be reflected in BeginDurationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ContainerBeginDuration :ITimeBVT
    {
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            tContainer.BeginTime     = TimeSpan.FromMilliseconds(100);
            tContainer.Duration      = new Duration(TimeSpan.FromMilliseconds(1000));
            tContainer.Name          = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            
            //Set properties directly on the Timeline.
            tNode.BeginTime     = TimeSpan.FromMilliseconds(100);
            tNode.Duration      = new Duration(TimeSpan.FromMilliseconds(400));
            tNode.Name            = "SimpleTimeNode";
            DEBUG.LOGSTATUS(" Set TimeNode Properties ");

            //Attach Handlers to the TimeNode
            AttachCurrentStateInvalidated( tNode );
            DEBUG.LOGSTATUS(" Attached EventHandlers to the TimeNode ");

            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            // Create a Clock, connected to the container.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 1100, 100, ref outString);
            
            return outString;
        }
    }
}

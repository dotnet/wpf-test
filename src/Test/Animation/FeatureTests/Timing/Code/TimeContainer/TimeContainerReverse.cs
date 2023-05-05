// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Timeline container's AutoReverse Property
 */

//Instructions:
//  1. Create a TimeNode with Begin = 0 and Duration = 250 
//  2. Attach event handlers to the TimeNode
//  3. Create a TimeContainer
//  4. Attach TimeNode to Container
//  5. Create a TimeManager with Start=0 and Step = 50
//  6. Test the CurrentGlobalSpeed property

//Warnings:
//  Any changes made in the output should be reflected in TimeContainerReverseExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TimeContainerReverse :ITimeBVT
    {
        Clock _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");

            tContainer.BeginTime        = TimeSpan.FromMilliseconds(0);
            tContainer.Duration         = new Duration(TimeSpan.FromMilliseconds(500));
            tContainer.AutoReverse      = true;
            tContainer.Name               = "Container";
            DEBUG.LOGSTATUS(" Set TimeContainer Properties ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );          
            // Set Properties to the TimeNode
            tNode.BeginTime             = TimeSpan.FromMilliseconds(0);
            tNode.Duration              = new Duration(TimeSpan.FromMilliseconds(250));
            tNode.Name                    = "TimeNode";
            DEBUG.LOGSTATUS(" Set TimeNode Properties ");
            
            //Attach Handler to the Child TimeNode
            AttachCurrentGlobalSpeedInvalidated( tNode );
            DEBUG.LOGSTATUS(" Attached EventHandler to the Timelines ");

            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            _tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 1000, 50, ref outString);
            
            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test a Timeline's BeginTime and Duration Properties
 */

//Instructions:
//  1. Create a Timeline with Begin = 100 and Duration = 500
//  2. Attach Begun and Ended handlers to the TimeNode
//  3. Create a Clock, associated with the Timeline
//  4. Create a TimeManager with Start=0 and Step = 100

//Warnings:
//  Any changes made in the output should be reflected BeginDurationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class BeginDuration :ITimeBVT
    {
        /*
         *  Function:   Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );

            //Set properties directly on the Timeline.
            tNode.BeginTime = TimeSpan.FromMilliseconds(100);
            tNode.Duration  = new System.Windows.Duration(TimeSpan.FromMilliseconds(500));
            tNode.Name        = "Timeline";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock,  tMan, 0, 1000, 100, ref outString);
            
            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting a Timeline's RepeatBehavior to a Timespan value (duration)
 */

//Instructions:
// 1. Create a Timeline with Begin=100, Duration=100,RepeatBehavior(Timespan value)
// 2. Check whether the Timeline completes at Tick=250

//Warnings:
//  Any changes made in the output should be reflected in RepeatDurationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class RepeatDuration :ITimeBVT
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
        
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime         = TimeSpan.FromMilliseconds(100);
            tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(100));
            tNode.RepeatBehavior    = new RepeatBehavior(TimeSpan.FromMilliseconds(150));
            tNode.Name                = "Timeline";            
            DEBUG.LOGSTATUS(" Set Timeline Properties ");
        
            //Attach Handlers to the TimeNode
            AttachCurrentGlobalSpeedInvalidated(tNode);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Timeline ");
        
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 300, 5, ref outString);
            
            return outString;
        }       
    
    }
}

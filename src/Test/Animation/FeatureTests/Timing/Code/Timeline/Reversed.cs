// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify event firing when AutoReverse = true
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 19 , and AutoReverse = true
//  2. Attach Begun, Ended, Reversed, Repeated handlers to the TimeNode
//  3. Create a TimeManager with Start=0 and Step = 5 and add the TimeNodes
//  4. Check each fired Event

//Warnings:
//  Any changes made in the output should be reflected in ReversedExpect.txt file

//Dependencies:
//  Automation Framework.dll, Timing\BVT\TimeNode\GlobalClasses.cs

//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Reversed :ITimeBVT
    {
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");
        
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime     = TimeSpan.FromMilliseconds(1);
            tNode.Duration      = new System.Windows.Duration(TimeSpan.FromMilliseconds(8));
            tNode.AutoReverse   = true;
            tNode.Name            = "Timeline";            
            DEBUG.LOGSTATUS(" Set Timeline Properties ");
        
            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            //HACK-HACK: CurrentTimeInvalidated must be associated to the -clock- to force a click.
            AttachAllHandlersTC(tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 20, 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }
    }
}

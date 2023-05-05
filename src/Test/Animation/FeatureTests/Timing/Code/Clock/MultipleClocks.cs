// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Create two Clocks and check event firing.
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 20 
//  2. Create two Clocks and bind events.
//  3. Tick a TimeManager with Start=0 and Step = 1

//Warnings:
//  Any changes made in the output should be reflected MultipleClocksExpect.txt file

//Pass Condition:
//  Verify events fire correctly.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class MultipleClocks :ITimeBVT
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
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created Timeline 1" );      
            // Set Properties to TimeNode 1
            tNode1.BeginTime    = TimeSpan.FromMilliseconds(0);
            tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(18));
            tNode1.Name           = "TimeNode1";
            DEBUG.LOGSTATUS(" Set Timeline 1 Properties ");

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created Timeline 2" );      
            // Set Properties to TimeNode 1
            tNode2.BeginTime    = TimeSpan.FromMilliseconds(0);
            tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(18));
            tNode2.Name           = "TimeNode2";
            DEBUG.LOGSTATUS(" Set Timeline 2 Properties ");

            // Create Clock 1, connected to the TimeNode 1.
            Clock tClock1 = tNode1.CreateClock();     
            DEBUG.ASSERT(tClock1 != null, "Cannot create Clock 1" , " Created Clock 1" );

            // Create Clock 2, connected to the TimeNode 2.
            Clock tClock2 = tNode2.CreateClock();     
            DEBUG.ASSERT(tClock2 != null, "Cannot create Clock 2" , " Created Clock 2" );

            //Attach Handlers to the Clocks
            AttachCurrentStateInvalidatedTC(tClock1);
            AttachCurrentStateInvalidatedTC(tClock2);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clocks ");
            
            int i = 0;
            this.CurrentTime = TimeSpan.Zero;
            tManager.Start();
            for( i = 0; i < 20 ; i += 1 )
            {
                tickNumber = i;
                outString += "---------- Processing time " + i + "\n";                         

                this.CurrentTime = TimeSpan.FromMilliseconds(i);
                tManager.Tick(); 
                
                outString += "-Clock1--------CurrentTime = " + tClock1.CurrentTime + "\n";
                outString += "-Clock2--------CurrentTime = " + tClock2.CurrentTime + "\n";
            }

            WriteAllEvents();
            
            return outString;
        }
    }
}

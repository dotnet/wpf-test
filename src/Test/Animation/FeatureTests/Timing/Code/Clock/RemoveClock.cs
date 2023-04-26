// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Clock can be removed via the RemoveRequested event
 */

//Instructions:
//  1. Create a Timeline
//  2. Create a Clock and connect it to the Timeline
//  3. Attach event handlers to the Clock
//  4. Create a TimeManager with Start=0 and Step = 100

//Pass Condition:
//   This test passes if events fired correctly.

//Pass Verification:
//   The output of this test should match the expected output in RemoveClockExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected RemoveClockExpect.txt file

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
    class RemoveClock :ITimeBVT
    {
        TimeManagerInternal _tMan;
        Clock _tClock;
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Clock and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";
            
            //Verify events by listing them at the end.
            eventsVerify = true;

            // Create a TimeManager
            _tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(_tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime       = TimeSpan.FromMilliseconds(0);
            tNode.Duration        = new Duration(TimeSpan.FromMilliseconds(10));
            tNode.Name            = "TimeNode";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = tNode.CreateClock();       
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Attach Handlers to the Clock
            AttachAllHandlersTC(_tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to the Clock ");
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, _tMan, 0, 11, 1, ref outString);

            WriteAllEvents();
               
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 7 )
            {
                _tClock.Controller.Remove();
            }
        }

        public override void OnRemoveRequested(object subject, EventArgs args)
        {    
            outString += "---------- RemoveRequested" + "\n";
            _tClock = null;
        }
        
        public override void OnProgress( Clock subject )
        {
            outString += "  CurrentProgress = " + subject.CurrentProgress + "\n";
        }
    }
}

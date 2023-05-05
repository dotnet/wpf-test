// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description: Verify the ClockController.SkipToFill() method
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 20 
//  2. Create a TimeManager with Start=0 and Step = 1 and add the TimeNode.
//  3. At tick=10 call the SkipToFill() method

//Warnings:
//  Any changes made in the output should be reflected ICSkipToFillExpect.txt file

//Pass Condition:
//  Verify at tick=15 the CurrentState of the clock.

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
    class ICSkipToFill :ITimeBVT
    {
        ParallelTimeline    _tNode;
        Clock               _tClock;
        
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *               Logs the results
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
            _tNode = new ParallelTimeline();
            DEBUG.ASSERT(_tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            _tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            _tNode.Duration          = new Duration(TimeSpan.FromMilliseconds(20));
            _tNode.Name              = "SimpleTimeNode";
            _tNode.FillBehavior      = FillBehavior.HoldEnd;
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            _tClock = _tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Attach Event Handlers to tContainer
            AttachCurrentStateInvalidatedTC( _tClock );
            AttachCurrentGlobalSpeedInvalidatedTC( _tClock );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 20 , 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }

        public override void OnProgress( Clock subject )
        {
            outString += "  " + _tNode.Name + ": CurrentGlobalSpeed = " + _tClock.CurrentGlobalSpeed + "\n";
        }
        
        public override void PostTick( int i )
        {
            if ( i == 10 )
            {
                DEBUG.LOGSTATUS(" Calling SkipToFill() on " + _tNode.Name );
                _tClock.Controller.SkipToFill(); 
            }

            outString += "  " + _tNode.Name + ": CurrentState = " + _tClock.CurrentState + "\n";
        }
        
    }
}

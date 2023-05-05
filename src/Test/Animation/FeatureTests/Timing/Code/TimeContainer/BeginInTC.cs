// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify InteractiveController.Begin() for a root Timeline with a child
 *                                            So, now testing BeginIn on the parent with a child.
 *                                        --- Also replacing InteractiveController with ClockController
 *                                        --- Also replacing BeginIn(0) with Begin()
 */

//Instructions:
//  1. Create a Timeline with Begin = Indefinite and Duration = 50
//  2. Attach Begun and Ended eventhandlers to the timeline
//  3. Create a Parent with Begin = 0
//  4. Attach eventHandlers to the Parent
//  5. Attach Timeline to the Parent
//  6. Create a TimeManager with Start=0 and Step = 1 and add the Parent
//  7. At Step = 2 call BeginIn(3) on the Parent

//Warnings:
//  Any changes made in the output should be reflected in BeginInTCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class BeginInTC :ITimeBVT
    {
        Clock  _tClock;
        /*
         *  Function:  Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");

            parent.BeginTime    = null;
            parent.Duration     = new Duration(TimeSpan.FromMilliseconds(10));
            parent.Name           = "Parent";
            DEBUG.LOGSTATUS(" Set Parent Properties ");

            // Create a Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Timeline" , " Created Timeline " );
            
            // Set Properties to the Timeline
            child.BeginTime     = TimeSpan.FromMilliseconds(0);
            child.Duration      = new Duration(TimeSpan.FromMilliseconds(10));
            child.Name            = "Child";
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            //Attach Timeline to the Parent
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached Timeline to the Parent ");

            // Create a Clock, connected to the container.
            _tClock = parent.CreateClock();        
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 16, 1, ref outString);
            
            return outString;
        }

        public override void PostTick(int i)
        {
            if ( i == 5 )
            {
                _tClock.Controller.Begin();
            }
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

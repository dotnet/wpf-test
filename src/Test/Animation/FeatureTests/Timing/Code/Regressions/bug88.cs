// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify applying Begin() before the scheduled Begin time


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (parent) and a child Timeline (child)
//  3. Bind the CurrentStateInvalidated event to both parent and child.
//  4. Create a Clock and associate it with the parent

//Pass Condition:
//   This test passes if the properties return correct values.

//Pass Verification:
//   The output of this test should match the expected output in bug88Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug88Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug88 : ITimeBVT
    {
        ClockGroup _clockParent;
        /*
         *  Function :  Test
         *  Arguments:  Framework
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

            // Create a container Timeline
            ParallelTimeline timelineParent = new ParallelTimeline();
            DEBUG.ASSERT(timelineParent != null, "Cannot create Parent", " Created Parent ");
            timelineParent.BeginTime            = TimeSpan.FromMilliseconds(10);
            timelineParent.Duration             = new Duration(TimeSpan.FromMilliseconds(20));
            timelineParent.Name                 = "Parent";

            // Create child Timeline
            ParallelTimeline timelineChild = new ParallelTimeline();
            DEBUG.ASSERT(timelineChild != null, "Cannot create Child" , " Created Child" );
            timelineChild.BeginTime             = TimeSpan.FromMilliseconds(5);
            timelineChild.Duration              = new Duration(TimeSpan.FromMilliseconds(5));
            timelineChild.FillBehavior          = FillBehavior.HoldEnd;
            timelineChild.Name                  = "Child";

            //Attach TimeNode1 to the Parent
            timelineParent.Children.Add(timelineChild);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Parent ");

            // Create a Clock, connected to the Timeline.
            _clockParent = timelineParent.CreateClock();       
            DEBUG.ASSERT(_clockParent != null, "Cannot create parent Clock" , " Created parent Clock " );
            //Attach Event Handlers to tClock
            AttachCurrentStateInvalidatedTC( _clockParent );

            Clock clockChild = _clockParent.Children[0];     
            DEBUG.ASSERT(clockChild != null, "Cannot create child Clock" , " Created child Clock " );
            //Attach Event Handlers to clockChild
            AttachCurrentStateInvalidatedTC( clockChild );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _clockParent, tManager, 0, 31, 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 5 )
            {
                _clockParent.Controller.Begin();
            }
        }
        
        public override void OnProgress( Clock subject )
        {
           outString += "-----------" + ((Clock)subject).Timeline.Name + ": CurrentState         = " + ((Clock)subject).CurrentState + "\n";
        }
    }
}

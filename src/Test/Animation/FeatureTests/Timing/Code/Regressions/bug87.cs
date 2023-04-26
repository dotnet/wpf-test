// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/*
 * Description:  Verify Progress when applying Begin() before its Child's scheduled Begin time


*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline (parent) and a child Timeline (child)
//  3. Create a Clock and associate it with the Timeline (parent)

//Pass Condition:
//   This test passes if the events fire appropriately.

//Pass Verification:
//   The output of this test should match the expected output in b.824079Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected in bug87Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug87 : ITimeBVT
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
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime            = TimeSpan.FromMilliseconds(0);
            parent.Duration             = new Duration(TimeSpan.FromMilliseconds(20));
            parent.Name                 = "Parent";

            // Create child Timeline
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            child.BeginTime         = TimeSpan.FromMilliseconds(5);
            child.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            child.FillBehavior      = FillBehavior.HoldEnd;
            child.Name              = "Child";

            //Attach TimeNode1 to the Parent
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNode to the Parent ");

            // Create a Clock, connected to the Timeline.
            _clockParent = parent.CreateClock();       
            DEBUG.ASSERT(_clockParent != null, "Cannot create parent Clock" , " Created parent Clock " );
            //Attach Event Handlers to tClock
            AttachCurrentStateInvalidatedTC( _clockParent );

            Clock clockChild = _clockParent.Children[0];     
            DEBUG.ASSERT(clockChild != null, "Cannot create child Clock" , " Created child Clock " );
            //Attach Event Handlers to clockChild
            AttachCurrentStateInvalidatedTC( clockChild );

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _clockParent, tManager, 0, 32, 1, ref outString);

            WriteAllEvents();
            
            return outString;
        }

        public override void PreTick( int i )
        {
            if ( i == 7 )
            {
                _clockParent.Controller.Begin();
            }
        }
    }
}

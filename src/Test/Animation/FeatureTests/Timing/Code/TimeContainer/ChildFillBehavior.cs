// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting a child's FillBehavior property

 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree
//  3. Create a Clock, associated with the topmost Timeline
//  4. Start the TimeManager


//Pass Verification:
//  The output of this test should match the expected output in ChildFillBehaviorExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected ChildFillBehaviorExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ChildFillBehavior :ITimeBVT
    {
        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            //Verify events by listing them at the end.
            eventsVerify = true;

            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.BeginTime         = TimeSpan.FromMilliseconds(0);
            tContainer.Duration          = new Duration(TimeSpan.FromMilliseconds(10));
            tContainer.FillBehavior      = FillBehavior.HoldEnd;
            tContainer.Name                = "Container";

            // Create child Timeline 1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            tNode1.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode1.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            tNode1.FillBehavior      = FillBehavior.HoldEnd;
            tNode1.Name                = "TimeNode1";

            // Create child Timeline 2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            tNode2.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode2.Duration          = new Duration(TimeSpan.FromMilliseconds(5));
            tNode2.FillBehavior      = FillBehavior.Stop;
            tNode2.Name                = "TimeNode2";

            //Attach TimeNodes to the Container
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();          
            DEBUG.ASSERT(tClock != null, "Cannot create Clock " , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Run the Timer
            TimeGenericWrappers.TESTSTATE( this, tClock, tManager, 0, 11, 1, ref outString);

            WriteAllEvents();

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

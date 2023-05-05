// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify a Timeline's FillBehavior.HoldEnd property when its child does not set it 
 */

//Instructions:
//  1. Create a TimeNode with Begin = 0 and Duration = 10.
//  2. Create a TimeContainer with Begin = 0 and Duration = 5. Set Fill.Hold property on the TimeContainer.
//  3. Attach TimeNode to the Container
//  4. Create a TimeManager with Start=0 and Step=1 and add the Container
//  5. Check the state of timing engine ( Progress, and CurrentState properties )

//Warnings:
//  Any changes made in the output should be reflected in ContainerFillHoldExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class ContainerFillHold : ITimeBVT
    {
        /*
         *  Function: Test
         *  Arguments: Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.BeginTime        = TimeSpan.FromMilliseconds(0);
            tContainer.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            tContainer.FillBehavior     = FillBehavior.HoldEnd;
            tContainer.Name             = "Container";
            
            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode.BeginTime             = TimeSpan.FromMilliseconds(0);
            tNode.Duration              = new Duration(TimeSpan.FromMilliseconds(5));
            tNode.Name                  = "TimeNode1";
            
            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tContainer.CreateClock();      
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);
            
            //Run the Timer         
            TimeGenericWrappers.TESTSTATE( this, tClock, tMan, 0, 12, 1, ref outString);
            
            return outString;   
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify setting and reading the Timeline's SpeedRatio property
  */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeContainer with Begin = 0, Duration = 800
//  3. Create a Timeline with Begin = 100, Duration = 800, and Speed = 2
//  4. Add the Timeline to the TimeContainer 
//  5. Add the TimeContainer to the TimeManager 

//Pass Condition:
//   This test passes if the Timeline finishes in half the time of its TimeContainer.

//Pass Verification:
//   The output of this test should match the expected output in SpeedExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in SpeedExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;


namespace Microsoft.Test.Animation
{
    class Speed : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            // Intialize output String
            outString = "";
               
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeContainer
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer" , " Created TimeContainer " );
            tContainer.BeginTime    = TimeSpan.FromMilliseconds(0);
            tContainer.Duration     = new System.Windows.Duration(TimeSpan.FromMilliseconds(800));

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            tNode.BeginTime     = TimeSpan.FromMilliseconds(0);
            tNode.Duration      = new System.Windows.Duration(TimeSpan.FromMilliseconds(800));
            tNode.SpeedRatio    = 2f;

            //Attach the Timeline to the TimeContainer          
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached Timeline to the TimeContainer ");

            // Create a Clock, connected to the tContainer.
            Clock tClock1 = tContainer.CreateClock();     
            DEBUG.ASSERT(tClock1 != null, "Cannot create Clock for tContainer" , " Created Clock for tContainer " );
            
            // Create a Clock, connected to the tNode.
            Clock tClock2 = tNode.CreateClock();      
            DEBUG.ASSERT(tClock2 != null, "Cannot create Clock for tNode" , " Created Clock for tNode" );
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clocks to force a click.
            AttachCurrentTimeInvalidatedTC(tClock1);
            AttachCurrentTimeInvalidatedTC(tClock2);

            //Read the TimeNode's SpeedRatio property.
            outString = "The SpeedRatio value is: " + tNode.SpeedRatio + "\n";
            
            //Run the Timer     
            tMan.Start();
                        
            for(int i = 0 ; i <= 1000; i += 100 )
            {               
                CurrentTime = TimeSpan.FromMilliseconds(i);
                tMan.Tick();

                outString += "----------------------------- " + i.ToString() + "\n";
                outString += "tContainer: " + tClock1.CurrentTime.ToString() + "\n";
                outString += "tNode:      " + tClock2.CurrentTime.ToString() + "\n";
            }
        
            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

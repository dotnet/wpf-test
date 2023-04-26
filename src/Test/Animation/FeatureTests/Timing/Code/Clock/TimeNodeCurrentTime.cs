// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading the Clock's CurrentTime property.

 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a TimeNode
//  3. Add the Timeline to the TimeManager 

//Pass Condition:
//   This test passes if CurrentTime returns integers at 100ms intervals starting at 0.

//Pass Verification:
//   The output of this test should match the expected output in TimeNodeCurrentTimeExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected in TimeNodeCurrentTimeExpect.txt file

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
    class TimeNodeCurrentTime : ITimeBVT
    {
        /*
         *  Function :  Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            tNode.BeginTime    = TimeSpan.FromMilliseconds(0);
            tNode.Duration = new Duration(TimeSpan.FromMilliseconds(1500));

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //HACK-HACK: CurrentTimeInvalidated must be associated to the clocks to force a click.
            AttachCurrentTimeInvalidatedTC(tClock);

            //Intialize output String
            outString = "";
            
            //Run the Timer     
            tMan.Start();
                        
            for(int i = 0 ; i <= 2000; i += 100 )
            {               
                CurrentTime = TimeSpan.FromMilliseconds(i);
                tMan.Tick();

                int currentTime = tClock.CurrentTime.HasValue ? (int)tClock.CurrentTime.Value.TotalMilliseconds : 0;
                outString += "CurrentTime: " + currentTime + "\n";
            }
        
            return outString;
        }       

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
// (c) Microsoft Corporation , 2002

/* 
 * Description: Verify the Parent of a Clock can be reached.
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 , Duration = 20 
//  2. Create two Clocks and bind the Begun and Ended events.
//  3. Tick a TimeManager with Start=0 and Step = 1

//Warnings:
//  Any changes made in the output should be reflected ParentExpect.txt file

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
    class Parent :ITimeBVT
    {
        private string _inString = "";
        
        /*
         *  Function:   Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created Timeline 1" );      
            tNode1.Name       = "TimeNode1";
            
            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created Timeline 2" );      
            tNode2.Name       = "TimeNode2";
            
            // Create TimeNode 3
            ParallelTimeline tNode3 = new ParallelTimeline();
            DEBUG.ASSERT(tNode3 != null, "Cannot create TimeNode 3" , " Created Timeline 3" );      
            tNode3.Name       = "TimeNode3";
            
            // Create TimeNode 4
            ParallelTimeline tNode4 = new ParallelTimeline();
            DEBUG.ASSERT(tNode4 != null, "Cannot create TimeNode 4" , " Created Timeline 4" );      
            tNode4.Name       = "TimeNode4";

            //Attach TimeNode2 to TimeNode1
            tNode1.Children.Add(tNode2);  //Parenting to tNode1.
            tNode1.Children.Add(tNode3);  //Parenting to tNode1.
            tNode3.Children.Add(tNode4);  //Parenting to tNode3.
            DEBUG.LOGSTATUS(" Attached TimeNode2 to TimeNode1 ");

            // Create Clock, connected to TimeNode 1.
            Clock tRoot = tNode1.CreateClock();       
            DEBUG.ASSERT(tRoot != null, "Cannot create Clock" , " Created Clock" );
            

            _inString = "--- Parent of root Clock is null? ---\t" + (tRoot.Parent == null).ToString() + "\n";
            
            ShowTree(tRoot);

            outString = _inString;

            return outString;
        }
          
        internal void ShowTree( Clock tClock )
        {        
            if (tClock != null)
            {
                if (tClock.Parent != null)
                {
                    _inString += "Parent of " + tClock.Timeline.Name + " = " + tClock.Parent.Timeline.Name + "\n";
                }

                ClockGroup tClockGroup = tClock as ClockGroup;

                if (tClockGroup != null)
                {
                    foreach ( Clock tClockChild in tClockGroup.Children )
                    {
                        ShowTree(tClockChild);
                    }
                }                    
            }
        }
    }
}

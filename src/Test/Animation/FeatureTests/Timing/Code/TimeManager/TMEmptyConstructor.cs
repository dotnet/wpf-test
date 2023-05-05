// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: This is to test the empty constructor of the TimeManager
  */

//Instructions:
//  1. Create a TimeManager using empty constructor
//  2. Attach a node with begin=0 and duration = Time.Indefinite
//  3. Start the TimeManager and give 10 ticks
//  4. Test the CurrentState property of the Clock

//Pass Verification:
//   The output of this test should match the expected output in TMEmptyConstructorExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TMEmptyConstructorExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class TMEmptyConstructor :ITimeBVT
    {
        Clock _tClock;
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager 
            TimeManagerInternal tManager = new TimeManagerInternal();
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            tNode.Name          = "TimeNode1";         
            tNode.BeginTime   = TimeSpan.FromMilliseconds(0);
            tNode.Duration    = System.Windows.Duration.Forever;

            // Create a Clock, connected to the Timeline.
            _tClock = tNode.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            tManager.Start();
            for(int i = 0 ; i <= 10; i++ )
            {               
                outString += "Processing time " + (int) i + " ms\n";
                DEBUG.LOGSTATUS("Processing time " + i);
                                
                tManager.Tick();    
                            
                outString += "--------------CurrentProgress = " + _tClock.CurrentProgress + "\n";
                outString += "--------------CurrentState    = " + _tClock.CurrentState + "\n";
            }
    
            return outString;
        }       
    }
}

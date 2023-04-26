// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//   

/* 
 * Description: Verify a child scheduled to begin during a Seek interval
 */

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline with begin=0 and duration=10 and attach to Manager
//  3. Start the TimeManager at timeline = 0
//  4. Seek the Timeline clock
//  5. Check the CurrentProgress, CurrentGlobalSpeed and CurrentState properties after each Tick

//Pass Verification:
//   The output of this test should match the expected output in ICSeekParent1Expect.txt

//Warnings:
//  Any changes made in the output should be reflected ICSeekParent1Expect.txt file

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
    class ICSeekParent1 :ITimeBVT
    {
        Clock               _tClock;
        
        /*
         *  Function:   Test
         *  Arguments:  Framework
         */
        public override string Test()
        {
            TimeManagerInternal tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(tManager != null, "Cannot create TimeManager" , " Created TimeManager ");
            
            // Create a parent TimeNode
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Timeline" , " Created Timeline " );
            parent.BeginTime   = TimeSpan.FromMilliseconds(0);
            parent.Duration    = new Duration(TimeSpan.FromMilliseconds(10));
            parent.Name        = "Parent";           
            
            // Create a child TimeNode
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Timeline" , " Created Timeline " );
            child.BeginTime   = TimeSpan.FromMilliseconds(4);
            child.Duration    = new Duration(TimeSpan.FromMilliseconds(4));
            child.Name        = "Child";           
            
            parent.Children.Add(child);
            
            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();     
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Intialize output String
            outString = "";
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tManager, 0, 12, 1, ref outString);
            
            return outString;
        }

        public override void PostTick( int i )
        {
            if ( i == 2 )
            {
                _tClock.Controller.Seek( TimeSpan.FromMilliseconds(8), TimeSeekOrigin.BeginTime );
            }
        }
        
        public override void OnProgress( Clock subject )
        {
            if ( ((Clock)subject).Timeline.Name == "Child" )
            {
                outString += "   CurrentProgress    = " + subject.CurrentProgress + "\n";
                outString += "   CurrentTime        = " + subject.CurrentTime + "\n";
                outString += "   CurrentState       = " + subject.CurrentState + "\n";
                outString += "   CurrentGlobalSpeed = " + subject.CurrentGlobalSpeed + "\n";
            }
        }
    }
}

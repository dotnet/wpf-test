// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify invoking Seek at the point a child reverses

*/

//Pass Verification:
//  The output of this test should match the expected output in bug82Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug82Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug82 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        ClockGroup            _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a Parent timeline
            ParallelTimeline parent = new ParallelTimeline();
            DEBUG.ASSERT(parent != null, "Cannot create Parent", " Created Parent ");
            parent.BeginTime            = TimeSpan.FromMilliseconds(0);
            parent.FillBehavior         = FillBehavior.Stop;
            parent.Name                 = "Parent";

            // Create Child 1
            ParallelTimeline child = new ParallelTimeline();
            DEBUG.ASSERT(child != null, "Cannot create Child" , " Created Child" );
            child.BeginTime             = TimeSpan.FromMilliseconds(0);
            child.Duration              = new Duration(TimeSpan.FromMilliseconds(5));
            child.AutoReverse           = true;
            child.FillBehavior          = FillBehavior.Stop;
            child.Name                  = "Child";

            //Attach TimeNodes to the Container
            parent.Children.Add(child);
            DEBUG.LOGSTATUS(" Attached TimeNodes to the Container ");

            // Create a Clock, connected to the Timeline.
            _tClock = parent.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            //Intialize output String
            outString = "";
            
            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 15, 1, ref outString);

            return outString;
        }
        
        public override void PostTick(int i)
        {
            if (i == 5)
            {
                _tClock.Controller.Seek(TimeSpan.FromMilliseconds(3),TimeSeekOrigin.BeginTime);
            }
        }
        
        public override void OnProgress( Clock subject )
        {    
            outString += "  " + ((Clock)subject).Timeline.Name + ": Progress = " + subject.CurrentProgress + "\n";
            outString += "  " + ((Clock)subject).Timeline.Name + ": State    = " + subject.CurrentState + "\n";
        }
    }
}

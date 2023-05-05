// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the property of Timeline within class ClockGroup

 */

//Instructions:
//  1. Create a new ClockGroup
//  2. Verify the property Timeline in class ClockGroup and Timeline's method CreateClock()

//Pass Condition:
//  Pass when clock starts.

//Pass Verification:
//  The output of this test should match the expected output in ClockGpTLExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected ClockGpTLExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class ClockGpTL : ITimeBVT
    {
        private Timeline _timeline;
        private Clock _tClock;

        public override string Test()
        {
            // Create a TimeManager
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create Timeline" , " Created Timeline " );
            tNode.BeginTime         = TimeSpan.FromMilliseconds(11);
            tNode.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            //tNode.Name                = "Timeline";
           
            // Create a ClockGroup
            ClockGroup clockGroup = ((TimelineGroup)tNode).CreateClock();
            DEBUG.ASSERT(clockGroup != null, "Cannot create ClockGroup", " Created TimelineGroup ");

            // Create a Clock, connected to the Timeline.
            _timeline = clockGroup.Timeline.Clone();
            _timeline.BeginTime = TimeSpan.FromMilliseconds(1);
            _timeline.Duration = new Duration(TimeSpan.FromMilliseconds(6));
            _tClock = _timeline.CreateClock();        
            //DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " ); 
          
            //Attach Handlers to the Timeline
            AttachCurrentStateInvalidatedTC(_tClock);
            DEBUG.LOGSTATUS(" Attached EventHandlers to TimeNode ");

            //Intialize output String
            outString = "";
           
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, _tClock, tMan, 0, 10, 1, ref outString);
            
            return outString;   
        }    
    }
}

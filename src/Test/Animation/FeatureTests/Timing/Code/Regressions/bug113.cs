// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description:  Verify setting BeginTime to null for a child Timeline

*/

//Instructions:
//  1. Create a TimeManager
//  2. Create a Timeline tree, with attached event handlers
//  3. Create a Clock, associated with the parent Timeline
//  4. Start the TimeManager

//Pass Condition:
//  No exception should occur.

//Pass Verification:
//  The output of this test should match the expected output in bug113Expect.txt.

//Warnings:
//  Any changes made in the output should be reflected bug113Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug113 :ITimeBVT
    {
        TimeManagerInternal   _tManager;
        Clock                 _tClock;

        /*
        *  Function:   Test
        *  Arguments:  Framework
        */
        public override string Test()
        {
            _tManager = EstablishTimeManager(this);
            DEBUG.ASSERT(_tManager != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a container Timeline
            ParallelTimeline tContainer = new ParallelTimeline();
            DEBUG.ASSERT(tContainer != null, "Cannot create TimeContainer", " Created TimeContainer ");
            tContainer.Name           = "Container";  //NOTE: no BeginTime or Duration is set.

            // Create a child Timeline1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode1" , " Created TimeNode1" );
            tNode1.BeginTime        = TimeSpan.FromMilliseconds(0);
            tNode1.Duration         = new Duration(TimeSpan.FromMilliseconds(10));
            tNode1.Name             = "TimeNode1";

            // Create a child Timeline2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode2" , " Created TimeNode2" );
            tNode2.BeginTime        = null;
            tNode2.Duration         = new Duration(TimeSpan.FromMilliseconds(4));
            tNode2.Name             = "TimeNode2";

            //Attach TimeNode to the Container
            tContainer.Children.Add(tNode1);
            DEBUG.LOGSTATUS(" Attached TimeNode1 to the Container ");
            tContainer.Children.Add(tNode2);
            DEBUG.LOGSTATUS(" Attached TimeNode2 to the Container ");

            // Create a Clock, connected to the container Timeline.
            _tClock = tContainer.CreateClock();          
            DEBUG.ASSERT(_tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(_tClock);

            //Intialize output String
            outString = "";

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, _tClock, _tManager, 0, 11, 1, ref outString);

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

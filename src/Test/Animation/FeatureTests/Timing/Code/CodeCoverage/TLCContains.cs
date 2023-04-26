// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the internal methods both IList.Contains within class TimelineCollection

 */

//Instructions:
//  1. Create a Time Container
//  2. Create time node with properties
//  3. Add time node as child to the Time Container
//  4. Verify existing child in the Time Container using IList.Contains method within TimelineCollection class

//Pass Condition:
//  Test passes when Time Container contains child time node.

//Pass Verification:
//  The output of this test should match the expected output in TLCContainsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TLCContainsExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Collections; 
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;

namespace Microsoft.Test.Animation
{
    class TLCContains :ITimeBVT
    {
        TimelineCollection _tc;

        /*
        *  Function:  Test
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
            tContainer.BeginTime  = TimeSpan.FromMilliseconds(0);
            tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(8));
            tContainer.Name         = "Container";

            // Create TimeNode 1
            ParallelTimeline tNode1 = new ParallelTimeline();
            DEBUG.ASSERT(tNode1 != null, "Cannot create TimeNode 1" , " Created TimeNode 1" );
            tNode1.BeginTime   = TimeSpan.FromMilliseconds(1);
            tNode1.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            tNode1.Name          = "Timeline1";

            // Create TimeNode 2
            ParallelTimeline tNode2 = new ParallelTimeline();
            DEBUG.ASSERT(tNode2 != null, "Cannot create TimeNode 2" , " Created TimeNode 2" );
            tNode2.BeginTime   = TimeSpan.FromMilliseconds(2);
            tNode2.Duration = new Duration(TimeSpan.FromMilliseconds(2));
            tNode2.Name          = "Timeline2";

            //Obtain a TimelineCollection for the Container
            _tc = tContainer.Children;
            DEBUG.ASSERT(_tc != null, "Cannot create TimelineCollection" , " Create TimelineCollection" );
            outString += "Count Before: " + _tc.Count + "\n";

            //Add a new time node to the TimelineCollection
            DEBUG.LOGSTATUS("Add a child time node." );
            tContainer.Children.Add(tNode1);
            tContainer.Children.Add(tNode2);

            outString += "Count After:  " + _tc.Count + "\n";

            DEBUG.LOGSTATUS("Test \'internal Contains\' method." );
            if (((IList)_tc).Contains(tNode2))  
            { 
                outString += "The last added child time node -> index = " + _tc.IndexOf(tNode2)  + "\n";
            } 

            // Create a Clock, connected to the container.
            Clock tClock = tContainer.CreateClock();          
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );

            AttachCurrentTimeInvalidatedTC(tClock);

            //Run the Timer               
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 10, 1, ref outString);

            return outString;
        }

        public override void OnCurrentTimeInvalidated(object subject, EventArgs args)
        {
        }
    }
}

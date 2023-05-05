// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the internal method IList.this{get;set} within class TimelineCollection

 */

//Instructions:
//  1. Create a Time Container
//  2. Create 2 time nodes with properties
//  3. Add a time node as child to the Time Container using IList.this{set}
//  4. Verify property of added time node using IList.this{get}

//Pass Condition:
//  Test passes when added string child exists.

//Pass Verification:
//  The output of this test should match the expected output in TLCThisExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected TLCThisExpect.txt file

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
    class TLCThis :ITimeBVT
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

            //Add time node to the TimelineCollection with public Add method
            _tc.Add(tNode2);
            DEBUG.LOGSTATUS("Add a time node to Time Container : " + _tc[0].Name);
            outString += "The name of the 1st added child time node :  " + _tc[0].Name + "\n";
            _tc.Add(tNode1);
            DEBUG.LOGSTATUS("Add a time node to Time Container : " + _tc[1].Name);
            outString += "The name of the 2nd added child time node :  " + _tc[1].Name + "\n";

            //Set (replace) timeline to the TimelineCollection
            DEBUG.LOGSTATUS("Test this_set method." );
            ((IList)_tc)[0] = tNode1;
            outString += "Setting (replacing) the 1st child time node : " + _tc[0].Name + "\n";          

            ((IList)_tc)[1] = tNode2;
            outString += "Setting (replacing) the 2nd child time node : " + _tc[1].Name + "\n";
            outString += "Count After setting :  " + _tc.Count + "\n";

            //Reading from the TimelineCollection
            DEBUG.LOGSTATUS("Test this_get method." );
            ParallelTimeline t = (ParallelTimeline)((IList)_tc)[0];  
            outString += "Reading the 1st child time node :  " + t.Name+ "\n";

            t = (ParallelTimeline)((IList)_tc)[1];  
            outString += "Reading the 2nd child timeline:  " + t.Name+ "\n";

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

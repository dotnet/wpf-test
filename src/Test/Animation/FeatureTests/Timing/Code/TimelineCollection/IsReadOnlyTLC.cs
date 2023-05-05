// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify reading a TimelineCollection's IsReadOnly property
 */

//Instructions:
//  1. Create a Timeline
//  2. Attach Begun and Ended handlers to the Container
//  3. Create a Clock, associated with the Container
//  4. Start the TimeManager

//Warnings:
//  Any changes made in the output should be reflected in IsReadOnlyTLCExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Collections;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class IsReadOnlyTLC :ITimeBVT
    {
        TimelineCollection _TLC;
        
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
            tContainer.Duration = new Duration(TimeSpan.FromMilliseconds(40));
            tContainer.Name         = "Container";

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created TimeNode " );
            tNode.BeginTime     = TimeSpan.FromMilliseconds(10);
            tNode.Duration = new Duration(TimeSpan.FromMilliseconds(30));
            tNode.FillBehavior  = FillBehavior.HoldEnd;
            tNode.Name            = "Timeline";
            
            //Attach TimeNode to the TimeContainer
            tContainer.Children.Add(tNode);
            DEBUG.LOGSTATUS(" Attached TimeNode to the TimeContainer ");

            _TLC = tContainer.Children;
               outString += "----------IsReadOnly: " + ((IList)_TLC).IsReadOnly + "\n";

            // Create a Clock, connected to the container.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
        
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 50, 10, ref outString);
               
               outString += "----------IsReadOnly: " + ((IList)_TLC).IsReadOnly + "\n";
            
            return outString;
        }
          public override void OnProgress( Clock subject )
          {
               outString += "----------IsReadOnly: " + ((IList)_TLC).IsReadOnly + "\n";
          }
    }
}

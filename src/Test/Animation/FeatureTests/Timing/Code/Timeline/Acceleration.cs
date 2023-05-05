// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 

/* 
 * Description: Verify the Timeline's AccelerationRatio Property
 *                                            Also, Acceleration is now called AccelerationRatio.
 */

//Instructions:
//  1. Create a Timeline with Begin = 0 and Duration = 100
//  2. Give an AccelerationRatio value of 0.4
//  3. Create a TimeManager with Start=0 and Step = 1 and add the TimeNode
//  4. Check the progress after each tick

//Warnings:
//  Any changes made in the output should be reflected AccelerationExpect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class Acceleration :ITimeBVT
    {
        /*
         *  Function:    Test
         *  Arguments:   Framework
         *  Description: Constructs a Timeline and Checks whether events are properly caught.
         *              Logs the results
         */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            // Create a TimeManager 
            TimeManagerInternal tMan = EstablishTimeManager(this);
            DEBUG.ASSERT(tMan != null, "Cannot create TimeManager" , " Created TimeManager ");

            // Create a TimeNode
            ParallelTimeline tNode = new ParallelTimeline();
            tNode.Name ="SimpleTimeNode";           
            DEBUG.ASSERT(tNode != null, "Cannot create TimeNode" , " Created Timeline " );
            
            // Set Properties to the TimeNode
            tNode.BeginTime         = TimeSpan.FromMilliseconds(0);
            tNode.Duration          = new System.Windows.Duration(TimeSpan.FromMilliseconds(100));
            tNode.AccelerationRatio = 0.4f;
            DEBUG.ASSERT( tNode.AccelerationRatio == 0.4f , "Acceleration is 0.4" , "Acceleration is not 0.4");
            DEBUG.LOGSTATUS(" Set Timeline Properties ");

            // Create a Clock, connected to the Timeline.
            Clock tClock = tNode.CreateClock();       
            DEBUG.ASSERT(tClock != null, "Cannot create Clock" , " Created Clock " );
            
            //Run the Timer         
            TimeGenericWrappers.EXECUTE( this, tClock, tMan, 0, 100, 1, ref outString);
            
            //Place 'Progress' in outString in order to force a call to LocalizeProgressValue().
            outString += "(Progress) Acceleration returns: " + ((Math.Round(tNode.AccelerationRatio * 10))/10).ToString() + "\n";
            outString += "(Progress) Deceleration returns: " + ((Math.Round(tNode.AccelerationRatio * 10))/10).ToString();

            return outString;
        }
    }
}

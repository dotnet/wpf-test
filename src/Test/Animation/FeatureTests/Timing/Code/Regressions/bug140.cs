// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify calling ClockCollection's CopyTo method with a null array

*/

//Instructions:
//     1. Create a new ClockCollection
//     2. Pass null to the ClockCollection

//Warnings:
//     Any changes made in the output should be reflected in bug140Expect.txt file

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs
//***************************************************************************************************
using System;
using System.Windows;
using System.Windows.Media.Animation;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Animation
{
    class bug140 :ITimeBVT
    {
          /*
           *  Function:  Test
           *  Arguments: Framework
           */
        public override string Test()
        {
            //Intialize output String
            outString = "";

            ParallelTimeline timeline = new ParallelTimeline();
            ClockGroup clock = timeline.CreateClock(); 
            ClockCollection CC = (ClockCollection)clock.Children;
            
            outString += "---Count: " + CC.Count + "\n";
            
            CC.CopyTo(null,0);
            
            outString += "---Count: " + CC.Count;


            return outString;
        }
    }
}

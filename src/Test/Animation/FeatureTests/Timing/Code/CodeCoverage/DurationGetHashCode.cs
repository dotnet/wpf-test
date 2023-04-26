// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the return value of GetHashCode method when an instance Duration has no TimeSpan with it.
 
 */

//Instructions:
//  1. Create new Duration without TimeSpan;
//  2. Verify the return value of GetHashCode method.

//Pass Condition:
//  All checks of the operators should pass.

//Pass Verification:
//  The output of this test should match the expected output in DurationOperatorsExpect.txt.

//Warnings:
//  Any changes made in the output should be reflected DurationOperatorsExpect.txt file

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
    class DurationGetHashCode :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            Duration Da = new Duration ();               
            
            //
            DEBUG.LOGSTATUS("Checking HashCode of a Duration without TimeSpan");
            outString += "HasTimeSpan method returns : " + Da.HasTimeSpan.ToString() + "\n";
            outString += "The Duration without TimeSpan returns the HashCode : " + Da.GetHashCode().ToString() + "\n";
            
            return outString;
        }
    }
}

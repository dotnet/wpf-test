// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the instance and static method Equals in Duration with different parameters

 */

//Instructions:
//  1. Create a Duration with an Object parameter
//  2. Verify the return value of Equal method (static)
//  3. Create a Duration with a non-Duration parameter
//  4. Verify the return value of Equal method (static)
//  5. Create two Durations with different TimeSpan values
//  6. Verify the return value of Equal method (instance)
//  7. Create two Durations with the same TimeSpan values
//  8. Verify the return value of Equal method (instance)


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
    class DurationEqual :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            Object obj = null;
            ParallelTimeline parallelTimeline = new ParallelTimeline();          
            
            Duration Da = new Duration (TimeSpan.FromMilliseconds(567));               
            Duration Db = new Duration (TimeSpan.FromMilliseconds(100));     
            Duration Dc = new Duration (TimeSpan.FromMilliseconds(567));     
            
            //To instance method Equals with Object parameter
            DEBUG.LOGSTATUS("Checking a null object as a parameter");
            outString += "Null object parameter returns : " + Da.Equals(obj).ToString()+ "\n";

            DEBUG.LOGSTATUS("Checking a non-Duration object as a parameter");
            outString += "Non-Duration object returns : " + Da.Equals(parallelTimeline).ToString()+ "\n";

        //To static method Equals
            DEBUG.LOGSTATUS("Checking the false Equal");
            Check( Da, Db, Duration.Equals(Da, Db), "Two Duration instances differ in their different values of TimeSpan contained");

            DEBUG.LOGSTATUS("Checking the true Equal");
            Check( Da, Dc, Duration.Equals(Da, Dc), "Two Duration instances equal in that the same values of TimeSpan contained");

            return outString;
        } 
    private void Check( Duration d1, Duration d2, bool condition, string message )
    {
        outString += message + " : "  + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
        outString += "The 1st Duration's Type = " + d1.ToString() + "\n";
        outString += "The 2nd Duration's Type = " + d2.ToString() + "\n";
        }
    }
}

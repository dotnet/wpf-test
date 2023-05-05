// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//*********************************************************************************************
// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Duration operators under different conditions

 
*/

//Instructions:
//  1. Create new Durations with different properties.
//  2. Verify the operators for the different Durations.

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
    class DurationOperators :ITimeBVT
    {
        /*
        *  Function : Test
        *  Arguments: Framework
        */
        public override string Test()
        {
            Duration Da = new Duration ();               
            Duration Db = new Duration (  TimeSpan.FromMilliseconds(100) );     

            Duration Dc = Duration.Automatic;               
            Duration Dd = Duration.Forever;               

            DEBUG.LOGSTATUS("Checking the + operators");
            Check( Da, Db, Da + Db == Duration.Automatic, "'+' operator  with one Duration without TimeSpan");

            DEBUG.LOGSTATUS("Checking the - operators");
            Check( Da, Db, Da - Db == Duration.Automatic, "-' operator  with one Duration without TimeSpan");

            DEBUG.LOGSTATUS("Checking the > operators");
            Check( Da, Db, Da > Db == false, "'>' operator with 1st Duration without TimeSpan and 2nd one with TimeSpan");
            Check( Dd, Dc, Dd > Dc == false, "'>' operator with both Duration without TimeSpan");

            DEBUG.LOGSTATUS("Checking the >= operators");
            Check( Da, Db, Da >= Db == false, "'>=' operator with 1st Duration without TimeSpan and 2nd one with TimeSpan");
            Check( Dd, Dc, Dd >= Dc == false, "'>=' operator with both Duration without TimeSpan");

            DEBUG.LOGSTATUS("Checking the < operators");       
            Check( Db, Da, Db < Da == false, "'<' operator with 1st Duration with TimeSpan and 2nd one without TimeSpan");
            Check( Dc, Dd, Dc < Dd == false, "'<' operator with both Duration without TimeSpan");

            DEBUG.LOGSTATUS("Checking the <= operators");
            Check( Db, Da, Db <= Da == false, "'<=' operator with 1st Duration with TimeSpan and 2nd one without TimeSpan");
            Check( Dc, Dd, Dc <= Dd == false, "'<=' operator with both Duration without TimeSpan");

            return outString;
        }

        private void Check( Duration d1, Duration d2, bool condition, string message )
        {
            outString += message + " : "  + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
            outString += "The 1st Duration's Type = " + d1.ToString() + ";\n";
            outString += "The 2nd Duration's Type = " + d2.ToString() + ".\n\n";
        }
    }
}

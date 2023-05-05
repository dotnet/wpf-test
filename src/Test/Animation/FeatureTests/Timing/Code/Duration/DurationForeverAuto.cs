// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

// Avalon Timing Verification Test Automation 
//  

/* 
 * Description: Verify the Duration methods using Duration.Forever and Duration.Automatic
*/

//Instructions:
//     1. Create Duration objects, assigned Duration.Forever and Duration.Automatic
//     2. Test different methods

//Warnings:
//     Any changes made in the output should be reflected DurationForeverAutoExpect.txt file

//Pass Condition:
//     The output from the program is compared with DurationForeverAutoExpect.txt file.

//Dependencies:
//  TestRuntime.dll, Timing\Common\GlobalClasses.cs

using System;
using System.Windows;
using System.Windows.Media.Animation;

using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;


namespace Microsoft.Test.Animation
{
     class DurationForeverAuto :ITimeBVT
     {
          /*
           *  Function : Test
           *  Arguments: Framework
           */
          public override string Test()
          {
               Duration dur1 = Duration.Forever;
               Duration dur2 = Duration.Automatic;
               Duration dur3 = new Duration (  TimeSpan.FromMilliseconds(512) );
               Duration dur4 = new Duration (  TimeSpan.FromMilliseconds(0) );
               Duration dur5 = Duration.Forever;
               Duration dur6 = Duration.Automatic;
               
               DEBUG.LOGSTATUS("Checking Add Method");
               Check( dur1.Add(dur3) == Duration.Forever   , " Add Method 1" );
               Check( dur3.Add(dur1) == Duration.Forever   , " Add Method 2" );
               Check( dur1.Add(dur2) == Duration.Automatic , " Add Method 3" );
               Check( dur2.Add(dur1) == Duration.Automatic , " Add Method 4" );
               Check( dur2.Add(dur3) == Duration.Automatic , " Add Method 5" );
               Check( dur3.Add(dur2) == Duration.Automatic , " Add Method 6" );
               Check( dur1.Add(dur5) == Duration.Forever   , " Add Method 7" );
               Check( dur2.Add(dur6) == Duration.Automatic , " Add Method 8" );

               DEBUG.LOGSTATUS("Checking Equals(duration) Method");
               Check(  dur1.Equals(dur1) == true     , " Equals Method 1");
               Check(  dur2.Equals(dur2) == true     , " Equals Method 2");
               Check(  dur1.Equals(dur5) == true     , " Equals Method 3");
               Check(  dur1.Equals(dur2) == false    , " Equals Method 4");
               Check(  dur2.Equals(dur1) == false    , " Equals Method 5");
               Check(  dur1.Equals(dur3) == false    , " Equals Method 6");
               Check(  dur3.Equals(dur1) == false    , " Equals Method 7");
               Check(  dur2.Equals(dur6) == true     , " Equals Method 8");
               Check(  dur2.Equals(dur3) == false    , " Equals Method 9");
               Check(  dur3.Equals(dur2) == false    , " Equals Method 10");
               Check(  dur2.Equals(dur4) == false    , " Equals Method 11");
               Check(  dur4.Equals(dur2) == false    , " Equals Method 12");

               DEBUG.LOGSTATUS("Checking Duration.Equals(duration,duration) Method");
               Check( (Equals(dur1,dur1) == true)   , " Equals(duration,duration) Method 1");
               Check( (Equals(dur2,dur2) == true)   , " Equals(duration,duration) Method 2");
               Check( (Equals(dur1,dur5) == true)   , " Equals(duration,duration) Method 3");
               Check( (Equals(dur1,dur2) == false)  , " Equals(duration,duration) Method 4");
               Check( (Equals(dur2,dur1) == false)  , " Equals(duration,duration) Method 5");
               Check( (Equals(dur1,dur3) == false)  , " Equals(duration,duration) Method 6");
               Check( (Equals(dur3,dur1) == false)  , " Equals(duration,duration) Method 7");
               Check( (Equals(dur2,dur6) == true)   , " Equals(duration,duration) Method 8");
               Check( (Equals(dur2,dur3) == false)  , " Equals(duration,duration) Method 9");
               Check( (Equals(dur3,dur2) == false)  , " Equals(duration,duration) Method 10");
               Check( (Equals(dur2,dur4) == false)  , " Equals(duration,duration) Method 11");
               Check( (Equals(dur4,dur2) == false)  , " Equals(duration,duration) Method 12");
               
               DEBUG.LOGSTATUS("Checking Equals(duration,duration) Method");
               Check( (Duration.Equals(dur1,dur1) == true)   , " Duration.Equals(duration,duration) Method 1");
               Check( (Duration.Equals(dur2,dur2) == true)   , " Duration.Equals(duration,duration) Method 2");
               Check( (Duration.Equals(dur1,dur5) == true)   , " Duration.Equals(duration,duration) Method 3");
               Check( (Duration.Equals(dur1,dur2) == false)  , " Duration.Equals(duration,duration) Method 4");
               Check( (Duration.Equals(dur2,dur1) == false)  , " Duration.Equals(duration,duration) Method 5");
               Check( (Duration.Equals(dur1,dur3) == false)  , " Duration.Equals(duration,duration) Method 6");
               Check( (Duration.Equals(dur3,dur1) == false)  , " Duration.Equals(duration,duration) Method 7");
               Check( (Duration.Equals(dur2,dur6) == true)   , " Duration.Equals(duration,duration) Method 8");
               Check( (Duration.Equals(dur2,dur3) == false)  , " Duration.Equals(duration,duration) Method 9");
               Check( (Duration.Equals(dur3,dur2) == false)  , " Duration.Equals(duration,duration) Method 10");
               Check( (Duration.Equals(dur2,dur4) == false)  , " Duration.Equals(duration,duration) Method 11");
               Check( (Duration.Equals(dur4,dur2) == false)  , " Duration.Equals(duration,duration) Method 12");

               DEBUG.LOGSTATUS("Checking Equals(object) Method");
               Duration nts7 = Duration.Forever;
               object dur7 = (object)nts7;
               Duration nts8 = Duration.Automatic;
               object dur8 = (object)nts8;
               Check( (dur1.Equals(dur7) == true)    , " Equals(object) Method 1");
               Check( (dur2.Equals(dur8) == true)    , " Equals(object) Method 2");
               Check( (dur3.Equals(dur7) == false)   , " Equals(object) Method 3");
               Check( (dur3.Equals(dur8) == false)   , " Equals(object) Method 4");
               
               DEBUG.LOGSTATUS("Checking GetHashCode Method");
               int hash1 = dur1.GetHashCode();
               bool hashResult1 = (hash1.GetType().ToString() == "System.Int32");
               Check( hashResult1 == true, " GetHashCode Method 1" );
               int hash2 = dur2.GetHashCode();
               bool hashResult2 = (hash2.GetType().ToString() == "System.Int32");
               Check( hashResult2 == true, " GetHashCode Method 2" );
               
               DEBUG.LOGSTATUS("Checking Subtract Method");
               Check( dur1.Subtract(dur1) == Duration.Automatic , " Subtract Method 1" );
               Check( dur2.Subtract(dur2) == Duration.Automatic , " Subtract Method 2" );
               Check( dur1.Subtract(dur5) == Duration.Automatic , " Subtract Method 3" );
               Check( dur1.Subtract(dur2) == Duration.Automatic , " Subtract Method 4" );
               Check( dur2.Subtract(dur1) == Duration.Automatic , " Subtract Method 5" );
               Check( dur1.Subtract(dur3) == Duration.Forever   , " Subtract Method 6" );
               Check( dur3.Subtract(dur1) == Duration.Automatic , " Subtract Method 7" );
               Check( dur2.Subtract(dur6) == Duration.Automatic , " Subtract Method 8" );
               Check( dur2.Subtract(dur3) == Duration.Automatic , " Subtract Method 9" );
               Check( dur3.Subtract(dur2) == Duration.Automatic , " Subtract Method 10" );
               Check( dur2.Subtract(dur4) == Duration.Automatic , " Subtract Method 11" );
               Check( dur4.Subtract(dur2) == Duration.Automatic , " Subtract Method 12" );
               
               DEBUG.LOGSTATUS("Checking ToString Method");
               Check( dur1.ToString() == "Forever",   " ToString Method 1" );
               Check( dur2.ToString() == "Automatic", " ToString Method 2" );
               
               DEBUG.LOGSTATUS("Checking Compare Method");
               Check( Duration.Compare(dur1,dur1) ==  0,  " Compare Method 1" );
               Check( Duration.Compare(dur2,dur2) ==  0,  " Compare Method 2" );
               Check( Duration.Compare(dur1,dur5) ==  0,  " Compare Method 3" );
               Check( Duration.Compare(dur1,dur2) ==  1,  " Compare Method 4" );
               Check( Duration.Compare(dur2,dur1) == -1,  " Compare Method 5" );
               Check( Duration.Compare(dur1,dur3) ==  1,  " Compare Method 6" );
               Check( Duration.Compare(dur3,dur1) == -1,  " Compare Method 7" );
               Check( Duration.Compare(dur2,dur6) ==  0,  " Compare Method 8" );
               Check( Duration.Compare(dur2,dur3) == -1,  " Compare Method 9" );
               Check( Duration.Compare(dur3,dur2) ==  1,  " Compare Method 10" );
               Check( Duration.Compare(dur2,dur4) == -1,  " Compare Method 11" );
               Check( Duration.Compare(dur4,dur2) ==  1,  " Compare Method 12" );

               DEBUG.LOGSTATUS("Checking > Method");
               Check( (dur1 > dur5) == false,  " > Method 1" );
               Check( (dur1 > dur2) == false,  " > Method 2" );
               Check( (dur2 > dur1) == false,  " > Method 3" );
               Check( (dur1 > dur3) == true,   " > Method 4" );
               Check( (dur3 > dur1) == false,  " > Method 5" );
               Check( (dur2 > dur6) == false,  " > Method 6" );
               Check( (dur2 > dur3) == false,  " > Method 7" );
               Check( (dur3 > dur2) == false,  " > Method 8" );
               Check( (dur2 > dur4) == false,  " > Method 9" );
               Check( (dur4 > dur2) == false,  " > Method 10" );

               DEBUG.LOGSTATUS("Checking >= Method");
               Check( (dur1 >= dur5) == true,   " >= Method 1" );
               Check( (dur1 >= dur2) == false,  " >= Method 2" );
               Check( (dur2 >= dur1) == false,  " >= Method 3" );
               Check( (dur1 >= dur3) == true,   " >= Method 4" );
               Check( (dur3 >= dur1) == false,  " >= Method 5" );
               Check( (dur2 >= dur6) == true,   " >= Method 6" );
               Check( (dur2 >= dur3) == false,  " >= Method 7" );
               Check( (dur3 >= dur2) == false,  " >= Method 8" );
               Check( (dur2 >= dur4) == false,  " >= Method 9" );
               Check( (dur4 >= dur2) == false,  " >= Method 10" );

               DEBUG.LOGSTATUS("Checking < Method");
               Check( (dur1 < dur5) == false,   " < Method 1" );
               Check( (dur1 < dur2) == false,   " < Method 2" );
               Check( (dur2 < dur1) == false,   " < Method 3" );
               Check( (dur1 < dur3) == false,   " < Method 4" );
               Check( (dur3 < dur1) == true,    " < Method 5" );
               Check( (dur2 < dur6) == false,   " < Method 6" );
               Check( (dur2 < dur3) == false,   " < Method 7" );
               Check( (dur3 < dur2) == false,   " < Method 8" );
               Check( (dur2 < dur4) == false,   " < Method 9" );
               Check( (dur4 < dur2) == false,   " < Method 10" );

               DEBUG.LOGSTATUS("Checking <= Method");
               Check( (dur1 <= dur5) == true,    " <= Method 1" );
               Check( (dur1 <= dur2) == false,   " <= Method 2" );
               Check( (dur2 <= dur1) == false,   " <= Method 3" );
               Check( (dur1 <= dur3) == false,   " <= Method 4" );
               Check( (dur3 <= dur1) == true,    " <= Method 5" );
               Check( (dur2 <= dur6) == true,    " <= Method 6" );
               Check( (dur2 <= dur3) == false,   " <= Method 7" );
               Check( (dur3 <= dur2) == false,   " <= Method 8" );
               Check( (dur2 <= dur4) == false,   " <= Method 9" );
               Check( (dur4 <= dur2) == false,   " <= Method 10" );

               DEBUG.LOGSTATUS("Checking == Method");
               Check( (dur1 == dur5) == true,    " == Method 1" );
               Check( (dur1 == dur2) == false,   " == Method 2" );
               Check( (dur2 == dur1) == false,   " == Method 3" );
               Check( (dur1 == dur3) == false,   " == Method 4" );
               Check( (dur3 == dur1) == false,   " == Method 5" );
               Check( (dur2 == dur6) == true,    " == Method 6" );
               Check( (dur2 == dur3) == false,   " == Method 7" );
               Check( (dur3 == dur2) == false,   " == Method 8" );
               Check( (dur2 == dur4) == false,   " == Method 9" );
               Check( (dur4 == dur2) == false,   " == Method 10" );
               
               DEBUG.LOGSTATUS("Checking + Method");
               Check( (dur1 + dur1) == Duration.Forever   , " + Method 1" );
               Check( (dur2 + dur2) == Duration.Automatic , " + Method 2" );
               Check( (dur1 + dur5) == Duration.Forever   , " + Method 3" );
               Check( (dur1 + dur2) == Duration.Automatic , " + Method 4" );
               Check( (dur2 + dur1) == Duration.Automatic , " + Method 5" );
               Check( (dur1 + dur3) == Duration.Forever   , " + Method 6" );
               Check( (dur3 + dur1) == Duration.Forever   , " + Method 7" );
               Check( (dur2 + dur6) == Duration.Automatic , " + Method 8" );
               Check( (dur2 + dur3) == Duration.Automatic , " + Method 9" );
               Check( (dur3 + dur2) == Duration.Automatic , " + Method 10" );
               Check( (dur2 + dur4) == Duration.Automatic , " + Method 11" );
               Check( (dur4 + dur2) == Duration.Automatic , " + Method 12" );
               
               DEBUG.LOGSTATUS("Checking - Method");
               Check( (dur1 - dur1) == Duration.Automatic , " - Method 1" );
               Check( (dur2 - dur2) == Duration.Automatic , " - Method 2" );
               Check( (dur1 - dur5) == Duration.Automatic , " - Method 3" );
               Check( (dur1 - dur2) == Duration.Automatic , " - Method 4" );
               Check( (dur2 - dur1) == Duration.Automatic , " - Method 5" );
               Check( (dur1 - dur3) == Duration.Forever   , " - Method 6" );
               Check( (dur3 - dur1) == Duration.Automatic , " - Method 7" );
               Check( (dur2 - dur6) == Duration.Automatic , " - Method 8" );
               Check( (dur2 - dur3) == Duration.Automatic , " - Method 9" );
               Check( (dur3 - dur2) == Duration.Automatic , " - Method 10" );
               Check( (dur2 - dur4) == Duration.Automatic , " - Method 11" );
               Check( (dur4 - dur2) == Duration.Automatic , " - Method 12" );

               DEBUG.LOGSTATUS("Checking Plus Method");
               Check( Duration.Plus(dur1) == dur1, " Plus Method 1" );
               Check( Duration.Plus(dur2) == dur2, " Plus Method 2" );


               return outString;
          }

          private void Check( bool condition, string message )
          {
               outString += message + " : " + ( condition ? "SUCCESS" : "FAILED" ) + "\n" ;
          }
     }
}

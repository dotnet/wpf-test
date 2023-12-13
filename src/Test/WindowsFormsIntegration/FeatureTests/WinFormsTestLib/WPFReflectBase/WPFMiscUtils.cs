// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using WFCTestLib.Log;

//
// Filename:    WPFMiscUtils.cs
// Description: Utility functions to bypass problems in WPFBase (until those problems are resolved)
//
namespace WPFReflectTools
{
    public class WPFMiscUtils
    {
        // These forms of IncCounters eventually call IncCounters(b) which calls LogLineNumber()
        // if b is false.  LogLineNumber access an internal log variable, which is wrong, so it throws
        // an Exception, knocking the entire Test Case down.
        // These versions bypass that behavior by testing the values first and then calling a "safe"
        // form of IncCounters if the test will fail.  Inefficient, but gets us past the exception.
        // Please remove these when someone fixes WPFReflectBase

        /// <summary>
        /// Safe replacement for ScenarioResult.IncCounters(expected, actual, comments, log)
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="expected"></param>
        /// <param name="actual"></param>
        /// <param name="comment"></param>
        /// <param name="log"></param>
        public static void IncCounters(ScenarioResult sr, object expected, object actual, string comments, Log log)
        {
            if (TestValues(expected, actual))
            {
                // only call this form if guaranteed to pass
                sr.IncCounters(expected, actual, comments, log);
            }
            else
            {
                // mimic what this was supposed to do, then fail safely
                log.WriteLine("FAIL: Expected = '{0}', Actual = '{1}'", expected.ToString(), actual.ToString());
                sr.IncCounters(new ScenarioResult(false, comments, log));
            }
        }

        /// <summary>
        /// Safe replacement for ScenarioResult.IncCounters(bool, comments)
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="log"></param>
        /// <param name="b"></param>
        /// <param name="comments"></param>
        public static void IncCounters(ScenarioResult sr, Log log, bool b, string comments)
        {
            if (b)
            {
                // only call this form if guaranteed to pass
                sr.IncCounters(b, comments, log);
            }
            else
            {
                log.WriteLine("FAIL: Comments = '{0}'", comments);
                sr.IncCounters(new ScenarioResult(false, comments, log));
            }
        }

        //
        // Returns true if expected.Equals(actual).  Checks for null values.
        // Code lifted from WFCTestlib.Log Scenario.cs to mimic dealing with null values
        //
        private static bool TestValues(object expected, object actual)
        {
            if (expected == null || actual == null)
            {
                if (expected == actual)
                    return true;
                else
                    return false;
            }

            return expected.Equals(actual);
        }
    }
}
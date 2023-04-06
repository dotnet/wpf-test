// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Windows.Test.Client.AppSec.BVT.ELCommon
{
    using System;
    using System.Collections;
    using Microsoft.Test;
    using Microsoft.Test.Logging;
        
    public class Logger
    {
        // Can't always predict whether a test will accidentally log a pass twice.
        // Prevent this by only doing the work once.
        public static Result CachedResult = Result.Fail;
        private static bool s_haveCachedAResult = false;
        private static bool s_haveInitialized = false;

        static private Hashtable s_hitCounter = new Hashtable();

        static public int RecordHit(string key)
        {
            if (TestLog.Current == null)
                Initialize();

            int val = 0;
            if (s_hitCounter[key] != null)
                val = (int)s_hitCounter[key];

            s_hitCounter[key] = ++val;
            Status("  [HITCOUNTER] Recording Event [" + key + "] --> HitCount=" + val.ToString());
            return val;
        }

        static public int GetHitCount(string key)
        {
            if (s_hitCounter[key] == null)
                return 0;
            else
                return (int)s_hitCounter[key];
        }
        
        static public void Initialize()
        {
            // Method left in place but modified to only work once for ease of conversion.
            // In the new Infra, if anything gets logged AFTER the result,
            // this code will create a new variation, which then also needs to get closed.
            // Eventual 
            if ((TestLog.Current == null) && !s_haveInitialized)
            {
                s_haveInitialized = true;
                new TestLog("WindowManagementTests");
            }
        }
        
        // Logger Wrappers
        static public void Status(string comment)
        {
            if (TestLog.Current == null)
            {
                Initialize();
            }
            // If it's still null here, that means that we're logging after the test has finished.  Which is fine...
            if (TestLog.Current == null)
            {
                LogManager.LogMessageDangerously(" *** " + comment);
            }
            else
            {
                TestLog.Current.LogEvidence(comment);
            }
        }

        static public void LogFail()
        {              
            LogFail(null);
        }
        
        static public void LogFail(string comment)
        {
            if ((TestLog.Current == null) && !s_haveInitialized)
            {
                Initialize();
            }
            if ((Log.Current != null) && Log.Current.CurrentVariation != null)
            {
                Log.Current.CurrentVariation.LogMessage("*** VALIDATION FAILED ***  " + comment);
            }
            else
            {
                LogManager.LogMessageDangerously(" *POST-RESULT-FAILURE* :" + comment);
            }
            cacheResult(Result.Fail);
        }

        static public void LogPass()
        {
            LogPass("*** TEST COMPLETED ***");
        }
        
        static public void LogPass(string comment)
        {
            if (TestLog.Current == null)
            {
                Initialize();
            }
            Log.Current.CurrentVariation.LogMessage(comment);
            cacheResult(Result.Pass);
        }

        static void cacheResult(Result result)
        {
            // Old behavior allowed unlimited invocations but 
            // there are places where pass is set after fail.  
            // This still needs to fail.
            if ((!s_haveCachedAResult) || (result == Result.Fail))
            {
                s_haveCachedAResult = true;
                CachedResult = result;
            }
        }

        static public void SetStage(string stage)
        {
            if (TestLog.Current == null)
            {
                Initialize();
            }
            Status(stage);
        }

        static public TestLog Current
        {
            get
            {
                if (TestLog.Current == null)
                {
                    Initialize();
                }
                return TestLog.Current;
            }
        }        
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Wpf.AppModel
{
    using System;
    using Microsoft.Test;
    using Microsoft.Test.Logging;
		
    public class Logger
    {
        private static Result s_cachedResult = Result.Fail;
        private static bool s_resultCached = false; // Needed as the above type is non-nullable

        private static void ensureActiveLog()
        {
            if (Log.Current.CurrentVariation == null)
            {
                Log.Current.CreateVariation("Variation");
            }
        }

        // Logger Wrappers
        static public void LogStatus(string comment)
        {
            ensureActiveLog();
            Log.Current.CurrentVariation.LogMessage(comment);
        }

        static public void LogFail()
        {              
            LogFail(null);
        }
        
        static public void LogFail(string comment)
        {
            s_resultCached = true;
            LogStatus("*** TEST FAILED ***  " + comment);            
            s_cachedResult = Result.Fail;
        }

        static public void LogPass()
        {
            LogPass("*** TEST PASSED ***");
        }
        
        static public void LogPass(string comment)
        {            
            if ((!s_resultCached) || (s_cachedResult != Result.Fail))
            {
                LogStatus(comment);
                s_cachedResult = Result.Pass;
            }
            else
            {
                LogStatus("Fail has already been logged, so ignoring call to pass test");
            }
            s_resultCached = true;
        }

        static public void LogIgnore(string comment)
        {
            LogStatus(comment);
            s_cachedResult = Result.Ignore;
            s_resultCached = true;
        }

        static public void CommitCachedResult()
        {
            Log.Current.CurrentVariation.LogMessage("Committing Cached Result: " + s_cachedResult);
            Log.Current.CurrentVariation.LogResult(s_cachedResult);
            Log.Current.CurrentVariation.Close();
        }
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// let's make things simplified
    /// </summary>
    public abstract class SimpleTestBase : StepsTest 
    {
        protected SimpleTestBase() 
        {
        }

        public TestResult PreInitialize()
        {
            TestResult result = TestResult.Pass;

            if (!MultiTouchVerifier.IsSupportedOS())
            {
                GlobalLog.LogStatus("Not supported OS");

                TestLog log = null;
                bool isLocalLog = false;

                if (TestLog.Current == null)
                {
                    GlobalLog.LogStatus("Not Current TestLog");

                    log = new TestLog(this.GetType().Name);
                    isLocalLog = true;
                }
                else
                {
                    log = TestLog.Current;
                }

                log.Result = TestResult.Ignore;
                result = log.Result;

                if (isLocalLog)
                {
                    log.Close();
                }
            }
            return result;
        }

    }
}

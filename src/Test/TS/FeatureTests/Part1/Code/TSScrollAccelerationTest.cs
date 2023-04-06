// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//****************************************************************** 
//* Purpose: Test TS Scroll Acceleration feature
//******************************************************************
using System;
using System.Windows;
using System.IO;
using System.Windows.Markup;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Threading;

namespace Microsoft.Test.TS
{
    public class TSScrollAccelerationTest
    {
        #region Methods
        
        public static void Run()
        {
            using (s_log = new TestLog("TS Scroll Acceleration Test"))
            {
                try
                {
                    PropertyBag parameters = DriverState.DriverParameters;

                    if (!string.IsNullOrEmpty(parameters["TestType"]))
                    {
                        s_testType = (parameters["TestType"]).Trim();
                        s_log.LogStatus(string.Format("Test Type: {0}.", s_testType));
                    }

                    ScrollAccelerationTestBase test = null;

                    switch (s_testType)
                    {
                        case "SimpleXamlBasedScrollTest":
                            test = new SimpleXamlBasedScrollTest(s_log, parameters);
                            break;

                        case "MultiScrollAreaTest":
                            test = new MultiScrollAreaTest(s_log, parameters);
                            break;

                        case "InteractionTest":
                            test = new InteractionTest(s_log, parameters);
                            break;

                        case "HitTest":
                            test = new HitTest(s_log, parameters);
                            break;

                        case "MultiMonitorTest":
                            test = new MultiMonitorTest(s_log, parameters);
                            break;

                        default:
                            throw new TestValidationException(string.Format("Invalid Test Type: {0}.", s_testType));
                    }

                    if (test != null)
                    {
                        test.RunTest();
                        s_log.Result = TestResult.Pass;
                    }
                }
                catch (Exception e)
                {
                    s_log.LogEvidence(string.Format("Got an exception: \n{0}", e.ToString()));
                    s_log.Result = TestResult.Fail;
                }
            }
        }

        #endregion

        #region Private Data

        private static TestLog s_log = null;
        private static string s_testType = string.Empty;
        
        #endregion
    }
}


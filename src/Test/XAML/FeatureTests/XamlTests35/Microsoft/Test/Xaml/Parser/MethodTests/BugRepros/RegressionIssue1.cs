// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Converters;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Xaml.Parser.MethodTests.BugRepros
{
    /// <summary>
    /// Repro for Regression Issue
    /// </summary>
    public class RegressionIssue1
    {
        /// <summary>
        /// Verifies that the CanConvertToString methos return false instead of
        /// throwing an exception.
        /// </summary>
        public void VerifyCanConvertToStringNoException()
        {
            int testVal = 10;   //object to be passed to the CanConvertToString Methods

            Int32RectValueSerializer int32RectValueSerializer = new Int32RectValueSerializer();
            PointValueSerializer pointValueSerializer = new PointValueSerializer();
            RectValueSerializer rectValueSerializer = new RectValueSerializer();
            SizeValueSerializer sizeValueSerializer = new SizeValueSerializer();
            VectorValueSerializer vectorValueSerializer = new VectorValueSerializer();

            if (int32RectValueSerializer.CanConvertToString(testVal, null))
            {
                GlobalLog.LogEvidence("Int32RectValueSerializer returned true when passed an int");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (pointValueSerializer.CanConvertToString(testVal, null))
            {
                GlobalLog.LogEvidence("PointValueSerializer returned true when passed an int");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (rectValueSerializer.CanConvertToString(testVal, null))
            {
                GlobalLog.LogEvidence("RectValueSerializer returned true when passed an int");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (sizeValueSerializer.CanConvertToString(testVal, null))
            {
                GlobalLog.LogEvidence("SizeValueSerializer returned true when passed an int");
                TestLog.Current.Result = TestResult.Fail;
            }

            if (vectorValueSerializer.CanConvertToString(testVal, null))
            {
                GlobalLog.LogEvidence("VectorValueSerializer returned true when passed an int");
                TestLog.Current.Result = TestResult.Fail;
            }
            TestLog.Current.Result = TestResult.Pass;
        }
    }
}

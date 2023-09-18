// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/******************************************************************* 
 * Purpose: Serialization cleanup for 2D
 ********************************************************************/
using System;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.Serialization;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    [Test(1, "Serialization", "Serialization 2D",
        Area = "2D",
        Description = @"Serialization 2D tests")]
    public class Serialization2D : StepsTest
    {

        /// <summary/>
        [Variation("ImageSource.xaml", SupportFiles = @"FeatureTests\2D\Dev10\Xamls\ImageSource.xaml,FeatureTests\2D\Dev10\Masters\winscii.png")]
        [Variation("RequestCachePolicy.xaml", SupportFiles = @"FeatureTests\2D\Dev10\Xamls\RequestCachePolicy.xaml,FeatureTests\2D\Dev10\Masters\winscii.png")]
        [Variation("Int32Rect.xaml", SupportFiles = @"FeatureTests\2D\Dev10\Xamls\Int32Rect.xaml")]
        [Variation("Brush.xaml", SupportFiles = @"FeatureTests\2D\Dev10\Xamls\Brush.xaml")]
        [Variation("InputBindings.xaml", SupportFiles = @"FeatureTests\2D\Dev10\Xamls\InputBindings.xaml")]
        public Serialization2D(string xamlFileIn)
        {
            _xamlFile = xamlFileIn;
            RunSteps += new TestStep(RoundTrip);
        }

        /// <summary>
        /// Attempt to roundtrip the xaml file
        /// </summary>
        /// <returns>TestResult</returns>
        private TestResult RoundTrip()
        {
            SerializationHelper helper = new SerializationHelper();
            TestResult result = TestResult.Fail;

            try
            {
                Log.LogStatus("Starting test for " + _xamlFile);
                helper.RoundTripTestFile(_xamlFile);
                Log.LogStatus("No exception for " + _xamlFile);

                result = TestResult.Pass;
            }
            catch (Exception e)
            {
                Log.LogEvidence("Unexpected Exception occurred:\n" + e.ToString());
            }

            return result;
        }

        private string _xamlFile;

    }
}


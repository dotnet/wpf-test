// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Graphics
{
    [Test(1, "VectorAngleBetween")]
    ///<summary>
    /// Tests for 2D Vector.AngleBetween
    ///</summary>
    class VectorAngleBetween : StepsTest
    {
        #region Variations
        [Variation( 1.0, 0.0, 0.0, 0.0, 0.0 ) ]     // invalid, degenerate vectors, sadly this is how it shipped
        [Variation( 1.0, 0.0, 1.0, 0.0, 0.0 ) ]     // same vector - 0 angle
        [Variation( 1.0, 0.0, 0.0, 1.0, 90.0 ) ]    // simple 90 degree case 
        [Variation( 0.0, 1.0, 1.0, 0.0, -90.0 ) ]   // mirror of simple 90 degree 
        [Variation( -1.0, 0.0, 0.0, 1.0, -90.0 ) ]  // rotated 90 degree  
        [Variation( 0.0, -1.0, -1.0, 0.0, -90.0 ) ] // rotated 90 degree  
        [Variation( 1.0, 0.0, 0.0, -1.0, -90.0 ) ]  // rotated 90 degree  
        [Variation( 1.0, 0.0, -1.0, 0.0, 180.0 ) ]  // basic 180 degree 
        [Variation( 1.0, 1.0, 0.0, 1.0, 45.0 ) ]    // different lengths 
        #endregion

        #region Constructor
        ///<summary>
        /// Copy our test variation data into the Settings struct
        ///</summary>
        public VectorAngleBetween(double x1In, double y1In, double x2In, double y2In, double angleIn)
        {
            _settings.X1 = x1In;
            _settings.Y1 = y1In;
            _settings.X2 = x2In;
            _settings.Y2 = y2In;
            _settings.ExpectedAngle = angleIn;

            RunSteps += new TestStep(Verify);
        }
        #endregion

        #region Test Step
        private TestResult Verify()
        {
            TestResult result = TestResult.Unknown;
            double angle = Vector.AngleBetween( new Vector(_settings.X1, _settings.Y1),
                                                new Vector(_settings.X2, _settings.Y2) );
      
            Log.LogEvidence("Expected angle " + _settings.ExpectedAngle + "+/-" + AllowableError);

            double difference = Math.Abs( _settings.ExpectedAngle - angle );
            
            if ( difference < AllowableError )
            {                
                Log.LogEvidence("Got acceptable angle " + angle);
                result = TestResult.Pass;
            }
            else
            {
                Log.LogEvidence("Got UNacceptable angle " + angle);
                result = TestResult.Fail;
            }

            return result;
        }
        #endregion

        #region Member variables
        private const double AllowableError = 0.00001; // allowable difference in value. There is no explicit accuracy contract.
        private TestSettings _settings; // test vector values
        #endregion

        #region internal declarations
        /// <summary>
        /// Each variation will have settings for the members of this struct,
        /// these are the test vectors for this test engine.
        /// </summary>
        internal struct TestSettings
        {
            public double   X1; // x ordinate of the first vector
            public double   Y1; // y ordinate of the first vector
            public double   X2; // x ordinate of the second vector
            public double   Y2; // y ordinate of the second vector
            public double   ExpectedAngle; // what the expected angle is, to within AllowableError
        }
        #endregion
    }
}

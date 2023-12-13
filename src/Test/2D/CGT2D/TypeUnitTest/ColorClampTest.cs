// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class ColorClampTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void    RunTheTest()
        {
            TestColorClamp();
        }

        private void TestColorClamp()
        {
            Log( "Testing Color.Clamp..." );

            TestColorClampWith( 0, 0, 0, 0 );
            TestColorClampWith( -1, -1, -1, -1 );
            TestColorClampWith( -13f, 1, -0.5f, 1 );
            TestColorClampWith( 0, -13f, 0, -0.1f );
            TestColorClampWith( 1, 1, 1, 1 );
            TestColorClampWith( float.MinValue, float.MinValue, float.MinValue, float.MinValue);
            TestColorClampWith( float.MaxValue, float.MaxValue, float.MaxValue, float.MaxValue);
            TestColorClampWith( float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity, float.NegativeInfinity);
            TestColorClampWith( float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity, float.PositiveInfinity);
        }

        private void TestColorClampWith( float a, float r, float g, float b  )
        {
            Color theirAnswer = Color.FromScRgb( a, r, g, b);
            theirAnswer.Clamp();

            Color myAnswer = Color.FromScRgb( a, r, g, b);

            // Clamp the color to 0 if it is lesser than 0 and to 1 if it is greater than 1
            myAnswer.ScA = (myAnswer.ScA < 0f) ? 0f : ((myAnswer.ScA > 1f) ? 1f : myAnswer.ScA);
            myAnswer.ScR = (myAnswer.ScR < 0f) ? 0f : ((myAnswer.ScR > 1f) ? 1f : myAnswer.ScR);
            myAnswer.ScG = (myAnswer.ScG < 0f) ? 0f : ((myAnswer.ScG > 1f) ? 1f : myAnswer.ScG);
            myAnswer.ScB = (myAnswer.ScB < 0f) ? 0f : ((myAnswer.ScB > 1f) ? 1f : myAnswer.ScB);

            if( !MathEx.Equals( theirAnswer, myAnswer ) || failOnPurpose )
            {
                AddFailure( "Color.Clamp( ) failed" );
                Log( "***Expected: Color = {0}, {1}, {2}, {3}", myAnswer.ScA, myAnswer.ScR, myAnswer.ScG, myAnswer.ScB );
                Log( "***Actual: Color = {0}, {1}, {2}, {3}", theirAnswer.ScA, theirAnswer.ScR, theirAnswer.ScG, theirAnswer.ScB );
            }
        }
    }
}

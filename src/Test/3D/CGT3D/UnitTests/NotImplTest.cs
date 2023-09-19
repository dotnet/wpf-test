// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Globalization;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;

#if !STANDALONE_BUILD
using TrustedMethodInfo = Microsoft.Test.Security.Wrappers.MethodInfoSW;
using TrustedType = Microsoft.Test.Security.Wrappers.TypeSW;
using Microsoft.Test.Graphics.Factories;

#else
using Microsoft.Test.Graphics.Factories;
using TrustedMethodInfo = System.Reflection.MethodInfo;
    using TrustedType = System.Type;
#endif
// Subnamespace "UnitTests" is required for this case to be picked up by /RunAll
namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary>
    /// The purpose of this test is to cover annoying
    /// methods that are not implemented.  We will
    /// know to implement tests for this stuff when
    /// this BVT fails.
    /// </summary>
    public class NotImplTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            CoverToString();
        }

        private void CoverToString()
        {
            // Camera
            CoverToStringWith(CameraFactory.MatrixDefaultOrthographic);
            CoverToStringWith(CameraFactory.OrthographicDefault);
            CoverToStringWith(CameraFactory.PerspectiveDefault);

            // Model3D
            CoverToStringWith(ModelFactory.MakeModel("SingleFrontFacingTriangle"));
#if SSL
            CoverToStringWith( ScreenSpaceLinesFactory.XyzAxes );
#endif
            CoverToStringWith(LightFactory.WhiteAmbient);
            CoverToStringWith(LightFactory.WhiteDirectionalNegZ);
            CoverToStringWith(LightFactory.WhitePoint);
            CoverToStringWith(LightFactory.WhiteSpot);
            CoverToStringWith(SceneFactory.Group);

            // Material
            CoverToStringWith(MaterialFactory.Default);
            CoverToStringWith(MaterialFactory.DefaultEmissive);
            CoverToStringWith(MaterialFactory.DefaultSpecular);
            CoverToStringWith(MaterialFactory.CompoundMaterial);

            // Transform
            CoverToStringWith(Const.rY90);
        }

        private void CoverToStringWith(object o)
        {
            string theirAnswer = o.ToString();
            TrustedType t = PT.Trust(o.GetType());
            string myAnswer = t.ToString();

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("{0}.ToString has changed!", t.Name);
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }

            theirAnswer = ((IFormattable)o).ToString("N", CultureInfo.CurrentCulture.NumberFormat);

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("{0}.ToString( string,IFormatProvider ) has changed!", t.Name);
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }

            theirAnswer = string.Empty;

            TrustedMethodInfo[] methods = t.GetMethods();
            foreach (TrustedMethodInfo method in methods)
            {
                if (method.Name == "ToString")
                {
                    if (method.GetParameters().Length == 1)
                    {
                        theirAnswer = (string)method.Invoke(o, new object[] { CultureInfo.CurrentCulture });
                    }
                }
            }

            if (theirAnswer != myAnswer || failOnPurpose)
            {
                AddFailure("{0}.ToString( IFormatProvider ) has changed!", t.Name);
                Log("*** Expected: {0}", myAnswer);
                Log("***   Actual: {0}", theirAnswer);
            }
        }
    }
}
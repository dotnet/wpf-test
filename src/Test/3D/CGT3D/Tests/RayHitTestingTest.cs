// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using Model3D.HitTest
    /// </summary>
    public class RayHitTestingTest : HitTestingTest
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            v.AssertExistenceOf("Origin", "Direction");
            Point3D origin = StringConverter.ToPoint3D(v["Origin"]);
            Vector3D direction = StringConverter.ToVector3D(v["Direction"]);
            ray = new Ray(origin, direction, RayOrigin.Visual3D);
        }

        /// <summary/>
        protected override void VerifyHitTesting()
        {
            RayHitTestParameters rayParameters = new RayHitTestParameters(ray.Origin, ray.Direction);
            if (rayParameters.Direction != ray.Direction || rayParameters.Origin != ray.Origin)
            {
                AddFailure("RayHitTestParameters' properties' get accessor failed");
                Log("*** Expected: Direction = {0}, Origin = {1}", ray.Direction, ray.Origin);
                Log("*** Actual:   Direction = {0}, Origin = {1}", rayParameters.Direction, rayParameters.Origin);
            }

            foreach (Visual3D visual3D in parameters.Visual3Ds)
            {
                VisualTreeHelper.HitTest(visual3D, filter, ProcessResults, rayParameters);
                // NOTE: I don't have to call this on the Children of visual3D
            }

            DoRayHitTesting(ray);
            VerifyHitTestResults();
        }

        private HitTestResultBehavior ProcessResults(HitTestResult result)
        {
            if (result is RayHitTestResult)
            {
                theirAnswers.Add(RayModelHitInfo.Create((RayHitTestResult)result));
            }
            else
            {
                throw new ApplicationException("Unexpected HitTestResult returned: " + result.GetType());
            }
            return HitTestResultBehavior.Continue;
        }

        /// <summary/>
        protected override bool OnlyVerifyClosest
        {
            get { return false; }
        }

        /// <summary/>
        protected Ray ray;
    }
}
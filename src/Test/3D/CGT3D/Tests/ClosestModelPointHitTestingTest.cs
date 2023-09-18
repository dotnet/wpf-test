// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using VisualTreeHelper.HitTest (i.e. from a 2D Point)
    /// This tests the API that has no callbacks and returns the closest model hit.
    /// </summary>
    public class ClosestModelPointHitTestingTest : PointHitTestingTest
    {
        /// <summary/>
        protected override void VerifyHitTesting()
        {
            // Set theirAnswer with Avalon's hit testing results
            HitTestClosestModel();

            // Set myAnswer with my hit testing results
            Ray ray = PointToRay();
            DoRayHitTesting(ray);

            // The ray hit testing fills "myAnswer" with all hit testing results.
            // We need to remove all but the closest one(s) (plural because of z tolerance)
            RemoveUnhitEntries();

            CompareHitTestResults();
        }

        private void RemoveUnhitEntries()
        {
            if (myAnswers.Count > 1)
            {
                myAnswers.Sort(RayModelHitInfo.Compare);

                // Leave any hits that have a distance close to the first one.
                // Because of numerical precision issues,
                //  we may have a different answer at entry 0 than they do.

                double dist = myAnswers[0].distanceToRayOrigin;
                for (int index = 1; index < myAnswers.Count; index++)
                {
                    if (MathEx.NotCloseEnough(dist, myAnswers[index].distanceToRayOrigin))
                    {
                        myAnswers.RemoveRange(index, myAnswers.Count - index);
                        break;
                    }
                }
            }
        }

        /// <summary/>
        protected override bool OnlyVerifyClosest
        {
            get { return true; }
        }
    }
}
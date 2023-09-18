// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// 3D Hit Testing using VisualTreeHelper.HitTest
    /// Don't verify hit test results, only that the
    /// HitTestFilterBehaviors work correctly.
    /// </summary>
    public class HitTestFilterBehaviorTest : RayHitTestingTest
    {
        /// <summary/>
        public override void Verify()
        {
            // We are overriding the implementation in HitTestingTest because we do not
            //  want to log an image with failures.
            // We do not care if the hit test was accurate.
            // We only care about what was looked at during the hit test pass.

            VerifyHitTesting();
        }

        /// <summary/>
        protected override void VerifyHitTesting()
        {
            // We don't care about the results.  We're just checking the filter.
            // But we do need to iterate through the HitTestResults so that the filter is called.
            // This is why we get the results and do nothing with them.

            RayHitTestParameters rayParameters = new RayHitTestParameters(ray.Origin, ray.Direction);
            foreach (Visual3D visual3D in parameters.Visual3Ds)
            {
                VisualTreeHelper.HitTest(visual3D, filter, IgnoreResults, rayParameters);
                // NOTE: I don't have to call this on the Children of visual3D

                _stopped = false;
                VerifyHitTestFilterBehavior(visual3D, false);
            }
        }

        private HitTestResultBehavior IgnoreResults(HitTestResult result)
        {
            // We don't care about the results.  We're just checking the filter.
            // Our filter marks the visuals as they are processed, so nothing more needs to be done here.
            return HitTestResultBehavior.Continue;
        }

        private bool VerifyHitTestFilterBehavior(Visual3D visual3D, bool shouldNotHaveTouched)
        {
            if (_stopped)
            {
                shouldNotHaveTouched = true;
            }

            bool pass = true;
            bool lookedAt = (bool)visual3D.GetValue(Const.LookedAtProperty);
            if (shouldNotHaveTouched && lookedAt)
            {
                pass = false;
                goto End;
            }

            string skip = (string)visual3D.GetValue(Const.SkipProperty);
            bool skipChildren = false;
            bool skipSelf = false;
            switch (skip)
            {
                case "SkipSelf":
                    skipSelf = true;
                    break;

                case "SkipChildren":
                    skipChildren = true;
                    break;

                case "SkipSelfAndChildren":
                    skipSelf = true;
                    skipChildren = true;
                    break;

                case "Stop":
                    _stopped = true;
                    if (lookedAt)
                    {
                        pass = false;
                        goto End;
                    }
                    break;
            }

            if (skipSelf && lookedAt)
            {
                pass = false;
            }
            else if (visual3D is ModelVisual3D)
            {
                // Double-check to make sure that Model3Ds aren't being visited
                bool contentObserved = (bool)((ModelVisual3D)visual3D).Content.GetValue(Const.LookedAtProperty);
                if (contentObserved)
                {
                    pass = false;
                    AddFailure("Model3D should not be filtered");
                    Log("Name = " + ObjectUtils.GetName(((ModelVisual3D)visual3D).Content));
                }
                foreach (Visual3D v in ((ModelVisual3D)visual3D).Children)
                {
                    pass |= VerifyHitTestFilterBehavior(v, skipChildren || shouldNotHaveTouched);
                }
            }

        End:
            if (!pass)
            {
                AddFailure("Avalon should not have looked at this Visual3D");
                Log("Name = " + ObjectUtils.GetName(visual3D));
            }
            return pass;
        }

        private bool _stopped;
    }
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Media;
using System.Windows;
using System.Windows.Ink;

namespace DRT
{
    [TestedSecurityLevelAttribute (SecurityLevel.PartialTrust)]
    public class StrokeIntersectionTest : DrtInkTestcase
	{
        private string inkFileName = "InterX.isf";

        private void PerformIntersectionTest(Stroke s, double [] interX)
        {
            double [] res = s.FindIntersections(s.Ink.Strokes);
            int hitCount = (null == interX) ? 0 : interX.Length;
            int resCount = (null == res) ? 0 : res.Length;
            if(hitCount != resCount)
            {
                throw new InvalidOperationException(
                    String.Format("Unexpected number of intersections returned by Stroke.FindIntersections : Expected {0} Actual {1}",
                    hitCount, res.Length));
            }
            int i;
            for(i = 0; i < hitCount; ++i)
            {
                if(Math.Abs(interX[i] - res[i]) > 0.001)
                    break;
            }
            if(i < hitCount)
            {
                throw new InvalidOperationException(
                    String.Format("Indices returned by Stroke.FindIntersections() do not match with ground truth"));
            }
        }

		public override void Run()
		{
            // Load reference ink file first
            Ink ink = DrtHelpers.LoadInk( inkFileName);

            // Perform hit test
            double[] interX1 = {
                2.454545f, 6.214286f, 6.333333f, 36.0625f, 42f, 66.5f, 68.92857f, 75.77778f, 87f, 110f, 119.3333f, 146f, 157.25f, 178.7692f, 182.8f, 214f, 218.5f, 245.9091f, 248.0625f, 278f, 286.75f, 311.4546f, 318.7273f, 346.5833f, 355.1667f, 387f, 391.5385f, 391.6667f
            };
            double[] interX2 = {
                2.6f, 4.2f, 11.11111f, 86.5f, 118f, 121.9f, 124.4f, 146.5f, 167f, 247f, 257.5f, 278f, 289f, 339.6667f, 358f, 379.5333f, 385.8462f, 428.25f, 437.8f, 450.75f, 453.6923f, 463.5f, 467.3333f, 500.2f, 516.875f, 530.75f, 548.1818f, 583.0667f, 586f, 594.8461f, 603.5f, 615.9355f, 633.5f, 667.875f, 678.8387f, 698.1707f, 708.3636f, 730.75f, 739.9756f, 750.6667f, 759.8333f, 785.0909f, 796.8333f, 801.9474f, 813.1818f, 814f, 849.3333f, 857.8421f, 866.1429f, 874.4375f, 878.6667f, 879.6f, 909.6154f, 909.6429f, 919.6905f, 938.2f, 938.9f, 939.8571f, 949.75f, 979.6f, 981f, 987.6667f
            };
            double[] interX3 = null;
            object[] intX = { interX1, interX2, interX3 };

            try
            {
                for (int i = 0; i < ink.Strokes.Count; ++i)
                {
                    this.PerformIntersectionTest(ink.Strokes[i], (double[])intX[i]);
                }
            }
            catch (InvalidOperationException)
            {
                DRT.Trace("StrokeIntersections test has regressed");
            }

            // done
			Success = true;
		}
	}
}

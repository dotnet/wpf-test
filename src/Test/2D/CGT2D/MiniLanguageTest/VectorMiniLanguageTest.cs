// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class VectorMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof(Vector);
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Vector a = new Vector(1, 2);
            Vector b = new Vector(5, 6);

            CompareVector(a, b, false);
            CompareVector(a, a, true);
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            Vector source = randomGenerator.CreateRandomVector(maxBounds);
            serialValue = randomGenerator.PadVector(source);

            Vector duplicate = (Vector)ConvertObject(serialValue, randomGenerator.IsClean);
            LogObjects(source, duplicate);
            CompareVector(source, duplicate, randomGenerator.IsClean);
        }

        private void CompareVector(Vector source, Vector duplicate, bool expectEqual)
        {
            if ((!MathEx.AreCloseEnough(source, duplicate)) ^ (!expectEqual))
            {
                LogProblemComparison(source, duplicate, expectEqual);
            }
        }

    }
}


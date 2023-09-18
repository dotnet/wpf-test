// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Shapes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class RectMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof(Rect);
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Rect a = new Rect(1, 2, 2, 2);
            Rect b = new Rect(5, 6, 2, 2);

            CompareRect(a, b, false);     //Size mismatch - fail
            CompareRect(a, a, true);      //identical Size - pass
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            Rect source = randomGenerator.CreateRandomRect(maxBounds);
            serialValue = randomGenerator.PadRect(source);
            Rect duplicate = (Rect)ConvertObject(serialValue, randomGenerator.IsClean);
            LogObjects(source, duplicate);
            CompareRect(source, duplicate, randomGenerator.IsClean);
        }

        private void CompareRect(Rect source, Rect duplicate, bool expectEqual)
        {
            if ((MathEx.NotCloseEnough(source, duplicate)) ^ (!expectEqual))
            {
                LogProblemComparison(source, duplicate, expectEqual);
            }
        }
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Microsoft.Test.Graphics.TestTypes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
     public class PointMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        public override void Init( Variation v )
        {
            base.Init( v );

            randomGenerator.paddingBugProbability = 0.5;
        }

        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof( Point );
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Point a = new Point( 1, 2 );
            Point b = new Point( 5, 6 );

            ComparePoint( a, b, false );
            ComparePoint( a, a, true );
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            Point source = randomGenerator.CreateRandomPoint( maxBounds );
            serialValue = randomGenerator.PadPoint( source );

            Point duplicate = (Point)ConvertObject( serialValue, randomGenerator.IsClean );

            LogObjects( source, duplicate );
            ComparePoint( source, duplicate, randomGenerator.IsClean );
        }

        private void ComparePoint( Point source, Point duplicate, bool expectEqual )
        {
            if ((!MathEx.AreCloseEnough( source, duplicate )) ^ (!expectEqual))
            {
                LogProblemComparison( source, duplicate, expectEqual );
            }
        }
    }
}


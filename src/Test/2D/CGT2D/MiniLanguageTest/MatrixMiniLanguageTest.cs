// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class MatrixMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof( Matrix );
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Matrix a = new Matrix( 1, 2, 2, 3, 4, 5 );
            Matrix b = new Matrix( 5, 6, 1, 2, 3, 4 );

            CompareMatrix( a, b, false );
            CompareMatrix( a, a, true );
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            Matrix source = randomGenerator.CreateRandomMatrix( maxBounds );
            serialValue = randomGenerator.PadMatrix( source );

            Matrix duplicate = (Matrix)ConvertObject( serialValue, randomGenerator.IsClean );

            LogObjects( source, duplicate );
            CompareMatrix( source, duplicate, randomGenerator.IsClean );
        }

        private void CompareMatrix( Matrix source, Matrix duplicate, bool expectEqual )
        {
            if ((!MathEx.AreCloseEnough( source, duplicate ) ^ (!expectEqual)))
            {
                LogProblemComparison( source, duplicate, expectEqual );
            }
        }

    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Shapes;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class SizeMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof( Size );
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Size a = new Size( 1, 2 );
            Size b = new Size( 5, 6 );

            CompareSize( a, b, false );
            CompareSize( a, a, true );
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            Size source = randomGenerator.CreateRandomSize( maxBounds );
            serialValue = randomGenerator.PadSize( source );
            Size duplicate = (Size)ConvertObject( serialValue, randomGenerator.IsClean );
            LogObjects( source, duplicate );
            CompareSize( source, duplicate, randomGenerator.IsClean );
        }

        private void CompareSize( Size source, Size duplicate, bool expectEqual )
        {
            if (
                (!(MathEx.AreCloseEnough( source.Height, duplicate.Height )
                && MathEx.AreCloseEnough( source.Width, duplicate.Width ))
                ^ (!expectEqual)))
            {
                LogProblemComparison( source, duplicate, expectEqual );
            }
        }

    }
}


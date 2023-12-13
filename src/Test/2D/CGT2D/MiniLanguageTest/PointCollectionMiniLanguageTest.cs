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
    public class PointCollectionMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        public override void Init( Variation v )
        {
            base.Init( v );
            _maxPoints = GetTestParameter( "maxPoints", 100 );
        }

        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof( PointCollection );
            }
        }

        /// <summary/>
        protected override void CheckTest()
        {
            PointCollection a = new PointCollection();
            a.Add( new Point( 1, 2 ) );

            PointCollection b = new PointCollection();
            b.Add( new Point( 3, 4 ) );

            PointCollection empty = new PointCollection();

            ComparePointCollection( a, a, true );
            ComparePointCollection( a, b, false );

            ComparePointCollection(a, empty, false);
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            PointCollection source = randomGenerator.CreateRandomPointCollection( _maxPoints, maxBounds );
            serialValue = randomGenerator.PadPointCollection( source );

            PointCollection duplicate = (PointCollection)ConvertObject( serialValue, randomGenerator.IsClean );

            LogObjects( source, duplicate );
            ComparePointCollection( source, duplicate, randomGenerator.IsClean );
        }

        private void ComparePointCollection( PointCollection source, PointCollection duplicate, bool expectEqual )
        {
            bool mismatch = false;
            if (source.Count == duplicate.Count)
            {
                for (int i = 0; i < source.Count; i++)
                {
                    if (!MathEx.AreCloseEnough( source[i], duplicate[i] ))
                        mismatch = true;
                }
            }
            else
            {
                mismatch = true;
            }

            if (mismatch ^ (!expectEqual))
            {
                LogProblemComparison( source, duplicate, expectEqual );
            }
        }

        private int _maxPoints;
    }
}


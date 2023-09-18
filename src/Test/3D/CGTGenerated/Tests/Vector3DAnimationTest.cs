// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for Vector3DAnimation (generated)
*
*   !!! This is a generated file. Do NOT edit it manually !!!
*
************************************************************/

using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes; using Microsoft.Test.Graphics;
using System.Windows.Media.Animation;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        Vector3DAnimationTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );


        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    RunTheTest()
        {
            TestFromTo();
            TestFromBy();
            TestKeyFrames();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            Seek( Clock clock, int milliseconds )
        {
            clock.Controller.SeekAlignedToLastTick( TimeSpan.FromMilliseconds( milliseconds ), TimeSeekOrigin.BeginTime );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        #region Interpolation Helpers

        private Vector3D Add( Vector3D obj1, Vector3D obj2 )
        {
            return new Vector3D( obj1.X + obj2.X, obj1.Y + obj2.Y, obj1.Z + obj2.Z );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Vector3D Scale( Vector3D obj, double s )
        {
            return new Vector3D( obj.X * s, obj.Y * s, obj.Z * s );
        }

        #endregion

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromTo()
        {
            Log( "Testing Vector3D From/To animations..." );

            TestFromToWith( new Vector3D( 0,0,0 ), new Vector3D( 10,10,10 ) );
            TestFromToWith( new Vector3D( -10,-10,-10 ), new Vector3D( 0,0,0 ) );
            TestFromToWith( new Vector3D( -10,-10,-10 ), new Vector3D( 20,20,20 ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromToWith( Vector3D from, Vector3D to )
        {
            Vector3DAnimation animation = new Vector3DAnimation();
            animation.From = from;
            animation.To = to;

            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Vector3D theirAnswer = (Vector3D)clock.GetCurrentValue( from, to );
                Vector3D myAnswer = InterpolateFromTo( from, to, n / (double)intervals );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "Vector3DAnimation From/To animations do not interpolate correctly" );
                    Log( "***     From: {0}", from );
                    Log( "***       To: {0}", to );

                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Vector3D InterpolateFromTo( Vector3D from, Vector3D to, double progress )
        {
            return Add( Scale( from, 1 - progress ), Scale( to, progress ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromBy()
        {
            Log( "Testing Vector3D From/By animations..." );

            TestFromByWith( new Vector3D( 1,1,1 ), new Vector3D( 1,1,1 ) );
            TestFromByWith( new Vector3D( 1,1,1 ), new Vector3D( -1,-1,-1 ) );
            TestFromByWith( new Vector3D( -1,-1,-1 ), new Vector3D( 1,1,1 ) );
            TestFromByWith( new Vector3D( -1,-1,-1 ), new Vector3D( -1,-1,-1 ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromByWith( Vector3D from, Vector3D by )
        {
            Vector3DAnimation animation = new Vector3DAnimation();
            animation.From = from;
            animation.By = by;

            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Vector3D theirAnswer = (Vector3D)clock.GetCurrentValue( from, from ); // 2nd parameter will be ignored
                Vector3D myAnswer = InterpolateFromBy( from, by, n / (double)intervals );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "Vector3DAnimation From/By animations do not interpolate correctly" );
                    Log( "***     From: {0}", from );
                    Log( "***       By: {0}", by );

                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Vector3D InterpolateFromBy( Vector3D from, Vector3D by, double progress )
        {
            return Add( from, Scale( by, progress ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestKeyFrames()
        {
            Log( "Testing Vector3D KeyFrame animations..." );

            TestKeyFramesWith( new Vector3D( -1,-1,-1 ), new Vector3D( 3,3,3 ), new Vector3D( 4,4,4 ) );
            TestKeyFramesWith( new Vector3D( 4,4,4 ), new Vector3D( 1,1,1 ), new Vector3D( -4,-4,-4 ) );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestKeyFramesWith( Vector3D frame1, Vector3D frame2, Vector3D frame3 )
        {
            Vector3DAnimationUsingKeyFrames animation = new Vector3DAnimationUsingKeyFrames();
            animation.KeyFrames.Add( new DiscreteVector3DKeyFrame( frame1, KeyTime.FromPercent( 0.0 ) ) );
            animation.KeyFrames.Add( new LinearVector3DKeyFrame( frame2, KeyTime.FromPercent( 0.6 ) ) );
            animation.KeyFrames.Add( new SplineVector3DKeyFrame( frame3, KeyTime.FromPercent( 1.0 ), new KeySpline( 0.25, 0.1, 0.75, 0.9 ) ) );


            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Vector3D theirAnswer = (Vector3D)clock.GetCurrentValue( frame1, frame3 );
                Vector3D myAnswer = InterpolateKeyFrames( animation.KeyFrames, n / (double)intervals );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "Vector3D KeyFrame animations do not interpolate correctly" );

                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Vector3D InterpolateKeyFrames( Vector3DKeyFrameCollection frames, double progress )
        {
            for ( int n = 0; n < frames.Count; n++ ) 
            {
                if ( frames[n].KeyTime.Percent == progress )
                {
                    return frames[n].Value;
                }
                if ( progress < frames[n].KeyTime.Percent )
                {
                    double previousFramePercent = ( n == 0 ) ? 0 : frames[n-1].KeyTime.Percent;
                    Vector3D previousFrameValue = ( n == 0 ) ? new Vector3D() : frames[n-1].Value;
                    double frameProgress = ( progress - previousFramePercent ) / ( frames[n].KeyTime.Percent - previousFramePercent );
                    return InterpolateKeyFrame( frames[n], frameProgress, previousFrameValue );
                }
            }
            return frames[frames.Count-1].Value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Vector3D InterpolateKeyFrame( Vector3DKeyFrame frame, double frameProgress, Vector3D previousValue )
        {
            if ( frame is DiscreteVector3DKeyFrame )
            {
                return previousValue;
            }
            if ( frame is LinearVector3DKeyFrame )
            {
                return InterpolateFromTo( previousValue, frame.Value, frameProgress );
            }
            if ( frame is SplineVector3DKeyFrame )
            {
                double progress = ((SplineVector3DKeyFrame)frame).KeySpline.GetSplineProgress( frameProgress );
                return InterpolateFromTo( previousValue, frame.Value, progress );
            }
            throw new NotSupportedException( "Unknown KeyFrame type" );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private const int       timespan = 1000;
        private const int       interval = 250;
        private const int       intervals = 4;
    }
}

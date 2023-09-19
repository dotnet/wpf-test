// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for QuaternionAnimation (generated)
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
using Microsoft.Test.Graphics.Factories;
using System.Windows.Media.Animation;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        QuaternionAnimationTest : CoreGraphicsTest
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

        private void            TestFromTo()
        {
            Log( "Testing Quaternion From/To animations..." );

            TestFromToWith( new Quaternion( 0,0.3826834323650898,0,0.9238795325112867 ), new Quaternion( 0,0.7071067811865476,0,-0.7071067811865475 ), true );
            TestFromToWith( new Quaternion( 0,0.3826834323650898,0,0.9238795325112867 ), new Quaternion( 0,0.7071067811865476,0,-0.7071067811865475 ), false );
            TestFromToWith( new Quaternion( -0.09229595564125724,0,0.09229595564125724,0.9914448613738104 ), new Quaternion( 0.5773502691896258,0.5773502691896258,-0.5773502691896258,0 ), true );
            TestFromToWith( new Quaternion( -0.09229595564125724,0,0.09229595564125724,0.9914448613738104 ), new Quaternion( 0.5773502691896258,0.5773502691896258,-0.5773502691896258,0 ), false );
            TestFromToWith( new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( -0.7071067811865475,0,0,0.7071067811865476 ), true );
            TestFromToWith( new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( -0.7071067811865475,0,0,0.7071067811865476 ), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromToWith( Quaternion from, Quaternion to, bool useShortestPath )
        {
            QuaternionAnimation animation = new QuaternionAnimation();
            animation.From = from;
            animation.To = to;
            animation.UseShortestPath = useShortestPath;
            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Quaternion theirAnswer = (Quaternion)clock.GetCurrentValue( from, to );
                Quaternion myAnswer = InterpolateFromTo( from, to, n / (double)intervals, useShortestPath );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "QuaternionAnimation From/To animations do not interpolate correctly" );
                    Log( "***     From: {0}", from );
                    Log( "***       To: {0}", to );
                    Log( "*** UseShortestPath: {0}", useShortestPath );
                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Quaternion InterpolateFromTo( Quaternion from, Quaternion to, double progress, bool useShortestPath )
        {
            return MathEx.Slerp( from, to, progress, useShortestPath );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromBy()
        {
            Log( "Testing Quaternion From/By animations..." );

            TestFromByWith( new Quaternion( 0,0.3826834323650898,0,0.9238795325112867 ), new Quaternion( 0,0.9238795325112867,0,-0.38268343236508967 ), true );
            TestFromByWith( new Quaternion( 0,0.3826834323650898,0,0.9238795325112867 ), new Quaternion( 0,0.9238795325112867,0,-0.38268343236508967 ), false );
            TestFromByWith( new Quaternion( -0.09229595564125724,0,0.09229595564125724,0.9914448613738104 ), new Quaternion( 0.6256980524354343,0.5724109576008407,-0.5191238627662471,-0.10657418966918719 ), true );
            TestFromByWith( new Quaternion( -0.09229595564125724,0,0.09229595564125724,0.9914448613738104 ), new Quaternion( 0.6256980524354343,0.5724109576008407,-0.5191238627662471,-0.10657418966918719 ), false );
            TestFromByWith( new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( -1,0,0,0 ), true );
            TestFromByWith( new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( -1,0,0,0 ), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestFromByWith( Quaternion from, Quaternion by, bool useShortestPath )
        {
            QuaternionAnimation animation = new QuaternionAnimation();
            animation.From = from;
            animation.By = by;
            animation.UseShortestPath = useShortestPath;
            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Quaternion theirAnswer = (Quaternion)clock.GetCurrentValue( from, from ); // 2nd parameter will be ignored
                Quaternion myAnswer = InterpolateFromBy( from, by, n / (double)intervals, useShortestPath );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "QuaternionAnimation From/By animations do not interpolate correctly" );
                    Log( "***     From: {0}", from );
                    Log( "***       By: {0}", by );
                    Log( "*** UseShortestPath: {0}", useShortestPath );
                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Quaternion InterpolateFromBy( Quaternion from, Quaternion by, double progress, bool useShortestPath )
        {
            return InterpolateFromTo( from, MathEx.Multiply( from, by ), progress, useShortestPath );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestKeyFrames()
        {
            Log( "Testing Quaternion KeyFrame animations..." );

            TestKeyFramesWith( new Quaternion( 0,0.7071067811865475,0,0.7071067811865476 ), new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( 0,0,0.7071067811865475,0.7071067811865476 ), true );
            TestKeyFramesWith( new Quaternion( 0,0.7071067811865475,0,0.7071067811865476 ), new Quaternion( 0.7071067811865475,0,0,0.7071067811865476 ), new Quaternion( 0,0,0.7071067811865475,0.7071067811865476 ), false );
            TestKeyFramesWith( new Quaternion( 0.3826834323650898,0,0,0.9238795325112867 ), new Quaternion( 0.9238795325112867,0,0,-0.3826834323650897 ), new Quaternion( -0.38268343236508967,0,0,-0.9238795325112869 ), true );
            TestKeyFramesWith( new Quaternion( 0.3826834323650898,0,0,0.9238795325112867 ), new Quaternion( 0.9238795325112867,0,0,-0.3826834323650897 ), new Quaternion( -0.38268343236508967,0,0,-0.9238795325112869 ), false );
            TestKeyFramesWith( new Quaternion( 0,0,0.3826834323650898,0.9238795325112867 ), new Quaternion( 0,0,0.3826834323650898,-0.9238795325112867 ), new Quaternion( 0,0,0.3826834323650898,0.9238795325112867 ), true );
            TestKeyFramesWith( new Quaternion( 0,0,0.3826834323650898,0.9238795325112867 ), new Quaternion( 0,0,0.3826834323650898,-0.9238795325112867 ), new Quaternion( 0,0,0.3826834323650898,0.9238795325112867 ), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestKeyFramesWith( Quaternion frame1, Quaternion frame2, Quaternion frame3, bool useShortestPath )
        {
            QuaternionAnimationUsingKeyFrames animation = new QuaternionAnimationUsingKeyFrames();
            animation.KeyFrames.Add( new DiscreteQuaternionKeyFrame( frame1, KeyTime.FromPercent( 0.0 ) ) );
            animation.KeyFrames.Add( new LinearQuaternionKeyFrame( frame2, KeyTime.FromPercent( 0.6 ) ) );
            animation.KeyFrames.Add( new SplineQuaternionKeyFrame( frame3, KeyTime.FromPercent( 1.0 ), new KeySpline( 0.25, 0.1, 0.75, 0.9 ) ) );
            ((LinearQuaternionKeyFrame)animation.KeyFrames[1]).UseShortestPath = useShortestPath;
            ((SplineQuaternionKeyFrame)animation.KeyFrames[2]).UseShortestPath = useShortestPath;
            animation.Duration = TimeSpan.FromMilliseconds( timespan );
            animation.FillBehavior = FillBehavior.HoldEnd;
            AnimationClock clock = animation.CreateClock();

            for ( int n = 0; n <= intervals; n++ )
            {
                Seek( clock, n * interval );
                Quaternion theirAnswer = (Quaternion)clock.GetCurrentValue( frame1, frame3 );
                Quaternion myAnswer = InterpolateKeyFrames( animation.KeyFrames, n / (double)intervals, useShortestPath );

                if ( MathEx.NotCloseEnough( theirAnswer, myAnswer ) || failOnPurpose )
                {
                    AddFailure( "Quaternion KeyFrame animations do not interpolate correctly" );
                    Log( "*** UseShortestPath: {0}", useShortestPath );
                    Log( "*** Progress: {0}", n / (double)intervals );
                    Log( "*** Expected: {0}", myAnswer );
                    Log( "***   Actual: {0}", theirAnswer );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Quaternion InterpolateKeyFrames( QuaternionKeyFrameCollection frames, double progress, bool useShortestPath )
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
                    Quaternion previousFrameValue = ( n == 0 ) ? new Quaternion() : frames[n-1].Value;
                    double frameProgress = ( progress - previousFramePercent ) / ( frames[n].KeyTime.Percent - previousFramePercent );
                    return InterpolateKeyFrame( frames[n], frameProgress, previousFrameValue, useShortestPath );
                }
            }
            return frames[frames.Count-1].Value;
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private Quaternion InterpolateKeyFrame( QuaternionKeyFrame frame, double frameProgress, Quaternion previousValue, bool useShortestPath )
        {
            if ( frame is DiscreteQuaternionKeyFrame )
            {
                return previousValue;
            }
            if ( frame is LinearQuaternionKeyFrame )
            {
                return InterpolateFromTo( previousValue, frame.Value, frameProgress, useShortestPath );
            }
            if ( frame is SplineQuaternionKeyFrame )
            {
                double progress = ((SplineQuaternionKeyFrame)frame).KeySpline.GetSplineProgress( frameProgress );
                return InterpolateFromTo( previousValue, frame.Value, progress, useShortestPath );
            }
            throw new NotSupportedException( "Unknown KeyFrame type" );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private const int       timespan = 1000;
        private const int       interval = 250;
        private const int       intervals = 4;
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class GlyphRunDrawingTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void RunTheTest()
        {
            if( priority > 0 )
            {
                RunTest2();
            }
            else
            {
                TestConstructor();
                TestClone();
                TestCloneCurrentValue();
                TestProperties();
            }
        }

        private void TestConstructor()
        {
            Log( "Testing Constructor..." );

            TestConstructorWith();

            TestConstructorWith( Brushes.Red, Const2D.glyphRun1 );
            TestConstructorWith( Const2D.linearGradientBrush1, Const2D.glyphRun1 );
            TestConstructorWith( Const2D.radialGradientBrush1, Const2D.glyphRun1 );
            TestConstructorWith( Const2D.imageBrush1, Const2D.glyphRun1 );
            TestConstructorWith( Const2D.drawingBrush1, Const2D.glyphRun1 );
            TestConstructorWith( Const2D.visualBrush1, Const2D.glyphRun1 );
        }

        private void TestConstructorWith()
        {
            GlyphRunDrawing theirAnswer = new GlyphRunDrawing();

            if( !ObjectUtils.Equals( theirAnswer.ForegroundBrush, null ) || !ObjectUtils.Equals( theirAnswer.GlyphRun, null ) || failOnPurpose )
            {
                AddFailure( "Constructor GlyphRunDrawing() failed" );
                Log( "*** Expected: GlyphRunDrawing.ForegroundBrush = {0}, GlyphRunDrawing.Glyphrun = {1}", null, null );
                Log( "*** Actual:   GlyphRunDrawing.ForegroundBrush = {0}, GlyphRunDrawing.Glyphrun = {1}", theirAnswer.ForegroundBrush, theirAnswer.GlyphRun );
            }
        }

        private void TestConstructorWith( Brush foregroundBrush, GlyphRun glyphRun )
        {
            GlyphRunDrawing theirAnswer = new GlyphRunDrawing( foregroundBrush, glyphRun );

            if( !ObjectUtils.Equals( theirAnswer.ForegroundBrush, foregroundBrush ) ||
                !ObjectUtils.Equals( theirAnswer.GlyphRun, glyphRun )               ||
                failOnPurpose )
            {
                AddFailure( "Constructor GlyphRunDrawing( Brush, GlyphRun ) failed" );
                Log( "***Expected: GlyphRunDrawing.ForegroundBrush = {0}, GlyphRunDrawing.GlyphRun = {1}", foregroundBrush, glyphRun );
                Log( "***Actual:   GlyphRunDrawing.ForegroundBrush = {0}, GlyphRunDrawing.GlyphRun = {1}", theirAnswer.ForegroundBrush, theirAnswer.GlyphRun );
                return;
            }
        }

        private void TestClone()
        {
            Log( "Testing Clone()..." );

            TestCloneWith( new GlyphRunDrawing() );
            TestCloneWith( new GlyphRunDrawing( Brushes.Red, Const2D.glyphRun1 ) );
            TestCloneWith( new GlyphRunDrawing( Const2D.linearGradientBrush1, Const2D.glyphRun1 ) );
            TestCloneWith( new GlyphRunDrawing( Const2D.radialGradientBrush1, Const2D.glyphRun1 ) );
            TestCloneWith( new GlyphRunDrawing( Const2D.imageBrush1, Const2D.glyphRun1 ) );
        }

        private void TestCloneWith( GlyphRunDrawing glyphRunDrawing )
        {
            GlyphRunDrawing theirAnswer = glyphRunDrawing.Clone();
            glyphRunDrawing.Freeze();
            theirAnswer.Freeze();

            if( !ObjectUtils.DeepEquals( theirAnswer, glyphRunDrawing ) || failOnPurpose )
            {
                AddFailure( "GlyphRunDrawing.Clone() failed" );
                Log( "***Expected: {0}", glyphRunDrawing );
                Log( "***Actual:   {0}", theirAnswer );
                return;
            }
        }

        private void TestCloneCurrentValue()
        {
            Log( "Testing CloneCurrentValue()..." );

            TestCloneCurrentValueWith( new GlyphRunDrawing() );
            TestCloneCurrentValueWith( new GlyphRunDrawing( new SolidColorBrush( Colors.Blue ), Const2D.glyphRun1 ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( Const2D.linearGradientBrush1, Const2D.glyphRun1 ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( Const2D.radialGradientBrush1, Const2D.glyphRun1 ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( Const2D.imageBrush1, Const2D.glyphRun1 ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( Const2D.drawingBrush1, Const2D.glyphRun1 ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( Const2D.visualBrush1, Const2D.glyphRun1 ) );
        }

        private void TestCloneCurrentValueWith( GlyphRunDrawing glyphRunDrawing )
        {
            GlyphRunDrawing theirAnswer = glyphRunDrawing.CloneCurrentValue();

            if( !ObjectUtils.DeepEquals( theirAnswer, glyphRunDrawing ) || failOnPurpose )
            {
                AddFailure( "CloneCurrentValue() failed" );
                Log( "***Expected: {0}", glyphRunDrawing );
                Log( "***Actual: {0}", theirAnswer );
            }
        }

        private void TestProperties()
        {
            Log( "Testing Properties ForegroundBrush, GlyphRun..." );

            TestPWith( Brushes.Red, "ForegroundBrush" );
            TestPWith( Const2D.linearGradientBrush1, "ForegroundBrush" );
            TestPWith( Const2D.radialGradientBrush1, "ForegroundBrush" );
            TestPWith( Const2D.imageBrush1, "ForegroundBrush" );
            TestPWith( Const2D.drawingBrush1, "ForegroundBrush" );
            TestPWith( Const2D.visualBrush1, "ForegroundBrush" );

            TestPWith( Const2D.glyphRun1, "GlyphRun" );
        }

        private void TestPWith( Brush value, string property )
        {
            GlyphRunDrawing glyphRunDrawing = new GlyphRunDrawing();
            Brush actual = SetTWith( ref glyphRunDrawing, value, property );
            if( !ObjectUtils.Equals( value, actual ) || failOnPurpose )
            {
                AddFailure( "set_" + property + " failed" );
                Log( "***Expected: {0}", value );
                Log( "***Actual:   {0}", actual );
            }
        }

        private Brush SetTWith( ref GlyphRunDrawing glyphRunDrawing, Brush value, string property )
        {
            switch ( property )
            {
                case "ForegroundBrush":     glyphRunDrawing.ForegroundBrush = value;      return glyphRunDrawing.ForegroundBrush;
            }
            throw new ApplicationException( "Invalid property: " + property + " cannot be set on GlyphRunDrawing" );
        }

        private void TestPWith( GlyphRun value, string property )
        {
            GlyphRunDrawing glyphRunDrawing = new GlyphRunDrawing();
            GlyphRun actual = SetTWith( ref glyphRunDrawing, value, property );
            if( !ObjectUtils.Equals( value, actual ) || failOnPurpose )
            {
                AddFailure( "set_" + property + " failed" );
                Log( "***Expected: {0}", value );
                Log( "***Actual:   {0}", actual );
            }
        }

        private GlyphRun SetTWith( ref GlyphRunDrawing glyphRunDrawing, GlyphRun value, string property )
        {
            switch ( property )
            {
                case "GlyphRun":     glyphRunDrawing.GlyphRun = value;      return glyphRunDrawing.GlyphRun;
            }
            throw new ApplicationException( "Invalid property: " + property + " cannot be set on GlyphRunDrawing" );
        }

        private void RunTest2()
        {
            TestConstructor2();
            TestClone2();
            TestCloneCurrentValue2();
            TestProperties2();
        }

        private void TestConstructor2()
        {
            Log( "P2 Testing Constructor..." );

            TestConstructorWith( null, null );
            TestConstructorWith( Brushes.Red, null );
            TestConstructorWith( null, Const2D.glyphRun1 );
        }

        private void TestClone2()
        {
            Log( "P2 Testing Clone()..." );

            TestCloneWith( new GlyphRunDrawing( null, null ) );
            TestCloneWith( new GlyphRunDrawing( Brushes.Red, null ) );
            TestCloneWith( new GlyphRunDrawing( null, Const2D.glyphRun1 ) );
        }

        private void TestCloneCurrentValue2()
        {
            Log( "P2 Testing CloneCurrentValue()..." );

            TestCloneCurrentValueWith( new GlyphRunDrawing( null, null ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( new SolidColorBrush( Colors.Red ), null ) );
            TestCloneCurrentValueWith( new GlyphRunDrawing( null, Const2D.glyphRun1 ) );
        }

        private void TestProperties2()
        {
            Log( "P2 Testing Properties ForegroundBrush, GlyphRun..." );

            TestPWith( (Brush)null, "ForegroundBrush" );
            TestPWith( (GlyphRun)null, "GlyphRun" );
        }
    }
}
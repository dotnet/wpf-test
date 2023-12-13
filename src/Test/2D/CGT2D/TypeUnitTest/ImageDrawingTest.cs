// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Globalization;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics.UnitTests
{
    /// <summary/>
    public class ImageDrawingTest : CoreGraphicsTest
    {
        /// <summary/>
        public override void    RunTheTest()
        {
            if( priority > 0 )
            {
                RunTest2();
            }
            else
            {
                TestConstructor();
                TestImageSource();
                TestRect();
            }
        }

        private void TestConstructor()
        {
            Log( "Testing Constructor ImageDrawing()..." );

            TestConstructorWith();

            Log( "Testing Constructor ImageDrawing( ImageSource, Rect )..." );

            TestConstructorWith( new BitmapImage( new Uri( "wood.bmp", UriKind.RelativeOrAbsolute ) ), new Rect() );
            TestConstructorWith( new BitmapImage( new Uri( "wood.bmp", UriKind.RelativeOrAbsolute ) ), new Rect( new Point( -1.5, -1.5 ), new Point( 1.5, 1.5 ) ) );
            TestConstructorWith( Const2D.drawingImage1, new Rect() );
            TestConstructorWith( Const2D.drawingImage1, new Rect( 10, 10, 10, 10 ) );
        }

        private void TestConstructorWith()
        {
            ImageDrawing theirAnswer = new ImageDrawing();
            if( theirAnswer.ImageSource != null || theirAnswer.Rect != Rect.Empty || failOnPurpose )
            {
                AddFailure( "Constructor ImageDrawing() failed" );
                Log( "*** Expected: ImageDrawing.ImageSource = {0}, ImageDrawing.Rect = {1}", null, Rect.Empty );
                Log( "*** Actual:   ImageDrawing.ImageSource = {0}, ImageDrawing.Rect = {1}", theirAnswer.ImageSource, theirAnswer.Rect );
            }
        }

        private void TestConstructorWith( ImageSource imageSource, Rect rect )
        {
            ImageDrawing theirAnswer = new ImageDrawing( imageSource, rect );

            if( !ObjectUtils.Equals( theirAnswer.Rect, rect )               ||
                !ObjectUtils.Equals( theirAnswer.ImageSource, imageSource ) ||
                failOnPurpose )
            {
                AddFailure( "Constructor ImageDrawing( ImageSource, Rect ) failed" );
                Log( "***Expected: ImageDrawing.ImageSource = {0}, ImageDrawing.Rect = {1}", imageSource, rect );
                Log( "***Actual:   ImageDrawing.ImageSource = {0}, ImageDrawing.Rect = {1}", theirAnswer.ImageSource, theirAnswer.Rect );
                return;
            }
        }

        private void TestImageSource()
        {
            Log( "Testing ImageSource Property..." );

            TestImageSourceWith( new BitmapImage( new Uri( "wood.bmp", UriKind.RelativeOrAbsolute ) ) );
            TestImageSourceWith( Const2D.drawingImage1 );
        }

        private void TestImageSourceWith( ImageSource imageSource )
        {
            ImageDrawing theirAnswer = new ImageDrawing();
            theirAnswer.ImageSource = imageSource;

            if( !ObjectUtils.Equals( theirAnswer.ImageSource, imageSource ) || failOnPurpose )
            {
                AddFailure( "get/set ImageSource Property failed" );
                Log( "***Expected: ImageDrawing.ImageSource = {0}", imageSource );
                Log( "***Actual:   ImageDrawing.ImageSource = {0}", theirAnswer.ImageSource );
                return;
            }
        }

        private void TestRect()
        {
            Log( "Testing Rect Property..." );

            TestRectWith( new Rect() ); // default Rect
            TestRectWith( new Rect( new Point( -1, -1 ), new Point( 1, 1 ) ) );
        }

        private void TestRectWith( Rect rect )
        {
            ImageDrawing theirAnswer = new ImageDrawing();

            theirAnswer.Rect = rect;

            if( !ObjectUtils.Equals( theirAnswer.Rect, rect ) || failOnPurpose )
            {
                AddFailure( "get/set Rect Property failed" );
                Log( "***Expected: ImageDrawing.Rect = {0}", rect );
                Log( "***Actual:   ImageDrawing.Rect = {0}", theirAnswer.Rect );
            }
        }

        private void RunTest2()
        {
            TestConstructor2();
            TestImageSource2();
            TestRect2();
        }

        private void TestConstructor2()
        {
            Log( "P2 Testing Constructor ImageDrawing( ImageSource, Rect )..." );

            TestConstructorWith( null, new Rect() ); // null ImageSource
            TestConstructorWith( new BitmapImage( new Uri( "wood.bmp", UriKind.RelativeOrAbsolute ) ), Rect.Empty ); // an Empty Rect
            TestConstructorWith( null, Rect.Empty ); // null ImageSource and an Empty Rect
        }

        private void TestImageSource2()
        {
            Log( "P2 Testing ImageSource Property..." );

            TestImageSourceWith( null ); // null ImageSource
        }

        private void TestRect2()
        {
            Log( "P2 Testing Rect Property..." );

            TestRectWith( Rect.Empty ); // an Empty Rect
            TestRectWith( new Rect( 0, 0, 0, 100 ) );  // Rect with zero width
            TestRectWith( new Rect( 0, 0, 100, 0 ) );  // Rect with zero height
        }
    }
}
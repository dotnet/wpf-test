// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for PointValueSerializer (generated)
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
using System.ComponentModel.Design.Serialization;
using System.Windows.Converters;
using System.Windows.Media.Converters;
using System.Windows.Media.Media3D.Converters;

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        PointValueSerializerTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _serializer = new PointValueSerializer();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    RunTheTest()
        {
            TestCanConvertToString();
            TestConvertToString();
            TestCanConvertFromString();
            TestConvertFromString();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertToString()
        {
            Log( "Testing CanConvertToString..." );

            TestCanConvertToStringWith( Const2D.p0, true );
            TestCanConvertToStringWith( Const2D.p10, true );
            TestCanConvertToStringWith( Const2D.pInf, true );
            TestCanConvertToStringWith( Const2D.pMaxMin, true );
            TestCanConvertToStringWith( Const2D.pNan, true );
            TestCanConvertToStringWith( Const2D.pEpsilon, true );
            TestCanConvertToStringWith( "0,0", false );
            TestCanConvertToStringWith( Const2D.v0, false );
            TestCanConvertToStringWith( Const2D.size1, false );
            TestCanConvertToStringWith( Const2D.pCollection, false );
            TestCanConvertToStringWith( Const.p0, false );
            TestCanConvertToStringWith( null, false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertToStringWith( object value, bool myAnswer )
        {
            bool theirAnswer = _serializer.CanConvertToString( value, null );
            if ( theirAnswer != myAnswer || failOnPurpose )
            {
                AddFailure( "CanConvertToString failed" );
                Log( "*** Object:   " + value );
                Log( "*** Expected: " + myAnswer );
                Log( "*** Actual:   " + theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToString()
        {
            Log( "Testing ConvertToString..." );

            TestConvertToStringWith( Const2D.p0, false );
            TestConvertToStringWith( Const2D.p10, false );
            TestConvertToStringWith( Const2D.pInf, false );
            TestConvertToStringWith( Const2D.pMaxMin, false );
            TestConvertToStringWith( Const2D.pNan, false );
            TestConvertToStringWith( Const2D.pEpsilon, false );
            TestConvertToStringWith( "0,0", true );
            TestConvertToStringWith( Const2D.v0, true );
            TestConvertToStringWith( Const2D.size1, true );
            TestConvertToStringWith( Const2D.pCollection, true );
            TestConvertToStringWith( Const.p0, true );
            TestConvertToStringWith( null, true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToStringWith( object value, bool shouldThrow )
        {
            try
            {
                object theirAnswer = _serializer.ConvertToString( value, null );

                // We do not verify the conversion here.  This should be done by the
                //  individual Point tests.
                // The only exception is converting to InstanceDescriptor.  This needs
                //  to be verified and will be done in the next update.

                if ( shouldThrow || failOnPurpose )
                {
                    AddFailure( "ConvertToString failed" );
                    Log( "Expected an exception" );
                    Log( "*** Value:       " + value );
                    Log( "*** TheirAnswer: " + theirAnswer );
                }
            }
            catch ( Exception ex )
            {
                if ( !shouldThrow || failOnPurpose )
                {
                    AddFailure( "ConvertToString failed" );
                    Log( "Unexpected exception thrown\n" + ex );
                    Log( "*** Value:         " + value );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertFromString()
        {
            Log( "Testing CanConvertFromString..." );

            TestCanConvertFromStringWith( "0,0", true );
            TestCanConvertFromStringWith( "Infinity,-Infinity", true );
            TestCanConvertFromStringWith( " -11.11 22.22 ", true );
            TestCanConvertFromStringWith( "NaN NaN" , true );
            TestCanConvertFromStringWith( "0", true );
            TestCanConvertFromStringWith( "0,0,0", true );
            TestCanConvertFromStringWith( "0,0,0,0", true );
            TestCanConvertFromStringWith( string.Empty, true );
            TestCanConvertFromStringWith( null, true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertFromStringWith( string value, bool canConvert )
        {
            bool theirAnswer = _serializer.CanConvertFromString( value, null );
            if ( theirAnswer != canConvert || failOnPurpose )
            {
                AddFailure( "CanConvertFromString failed" );
                Log( "*** String:   " + value );
                Log( "*** Expected: " + canConvert );
                Log( "*** Actual:   " + theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertFromString()
        {
            Log( "Testing ConvertFromString..." );

            TestConvertFromStringWith( "0,0", false );
            TestConvertFromStringWith( "Infinity,-Infinity", false );
            TestConvertFromStringWith( " -11.11 22.22 ", false );
            TestConvertFromStringWith( "NaN NaN" , false );
            TestConvertFromStringWith( "0", true );
            TestConvertFromStringWith( "0,0,0", true );
            TestConvertFromStringWith( "0,0,0,0", true );
            TestConvertFromStringWith( string.Empty, true );
            TestConvertFromStringWith( null, true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertFromStringWith( string value, bool shouldThrow )
        {
            try
            {
                // Do not localize the string because the ValueSerializers are culture-invariant
                object theirAnswer = _serializer.ConvertFromString( value, null );

                // We do not verify the conversion here.  This should be done by the
                //  individual Point tests.

                if ( shouldThrow || failOnPurpose )
                {
                    AddFailure( "ConvertFromString failed" );
                    Log( "Expected an exception" );
                    Log( "*** Value:       " + value );
                    Log( "*** TheirAnswer: " + theirAnswer );
                }
            }
            catch ( Exception ex )
            {
                // Not supported exception is thrown by ValueSerializer
                // Invalid operation exception is thrown by Point.Parse( string )
                // ArgumentException is thrown by Rect3D and Size3D parsers.
                // FormatException is thrown by mini-langrage parser when a malformed string is passed in

                if ( !shouldThrow ||
                    !( ex is NotSupportedException || ex is InvalidOperationException || ex is ArgumentException || ex is FormatException ) || 
                    failOnPurpose )
                {
                    AddFailure( "ConvertFromString failed" );
                    Log( "Unexpected exception thrown\n" + ex );
                    Log( "*** Value: " + value );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private PointValueSerializer _serializer;
    }
}

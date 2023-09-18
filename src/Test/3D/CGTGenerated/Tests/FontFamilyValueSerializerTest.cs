// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for FontFamilyValueSerializer (generated)
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

    public partial class        FontFamilyValueSerializerTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _serializer = new FontFamilyValueSerializer();
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

            TestCanConvertToStringWith( new FontFamily("Arial"), true );
            TestCanConvertToStringWith( new FontFamily("Microsoft Sans Serif"), true );
            TestCanConvertToStringWith( null, false );
            TestCanConvertToStringWith( "", false );
            TestCanConvertToStringWith( string.Empty, false );
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

            TestConvertToStringWith( new FontFamily("Arial"), false );
            TestConvertToStringWith( new FontFamily("Microsoft Sans Serif"), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToStringWith( object value, bool shouldThrow )
        {
            try
            {
                object theirAnswer = _serializer.ConvertToString( value, null );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontFamily tests.
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

            TestCanConvertFromStringWith( "Arial", true );
            TestCanConvertFromStringWith( "Microsoft Sans Serif", true );
            TestCanConvertFromStringWith( "12.0", true );
            TestCanConvertFromStringWith( "xxx", true );
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

            TestConvertFromStringWith( "Arial", false );
            TestConvertFromStringWith( "Microsoft Sans Serif", false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertFromStringWith( string value, bool shouldThrow )
        {
            try
            {
                // Do not localize the string because the ValueSerializers are culture-invariant
                object theirAnswer = _serializer.ConvertFromString( value, null );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontFamily tests.

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
                // Invalid operation exception is thrown by FontFamily.Parse( string )
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

        private FontFamilyValueSerializer _serializer;
    }
}
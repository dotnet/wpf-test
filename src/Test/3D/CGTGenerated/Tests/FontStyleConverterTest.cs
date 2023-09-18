// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for FontStyleConverter (generated)
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

//------------------------------------------------------------------

namespace                       Microsoft.Test.Graphics.Generated
{
    //--------------------------------------------------------------

    public partial class        FontStyleConverterTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _converter = new FontStyleConverter();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    RunTheTest()
        {
            TestCanConvertTo();
            TestCanConvertFrom();
            TestConvertTo();
            TestConvertFrom();
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertTo()
        {
            Log( "Testing CanConvertTo..." );

            TestCanConvertToWith( typeof( InstanceDescriptor ), true );
            TestCanConvertToWith( typeof( string ), true );
            TestCanConvertToWith( null, false );
            TestCanConvertToWith( typeof( bool ), false );
            TestCanConvertToWith( typeof( int ), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertToWith( Type t, bool canConvert )
        {
            bool theirAnswer = _converter.CanConvertTo( t );
            if ( theirAnswer != canConvert || failOnPurpose )
            {
                AddFailure( "CanConvertTo failed" );
                Log( "*** Type: " + t );
                Log( "*** Expected: " + canConvert );
                Log( "*** Actual:   " + theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertFrom()
        {
            Log( "Testing CanConvertFrom..." );

            TestCanConvertFromWith( typeof( string ), true );
            TestCanConvertFromWith( typeof( InstanceDescriptor ), false );
            TestCanConvertFromWith( null, false );
            TestCanConvertFromWith( typeof( bool ), false );
            TestCanConvertFromWith( typeof( int ), false );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestCanConvertFromWith( Type t, bool canConvert )
        {
            bool theirAnswer = _converter.CanConvertFrom( t );
            if ( theirAnswer != canConvert || failOnPurpose )
            {
                AddFailure( "CanConvertFrom failed" );
                Log( "*** Type: " + t );
                Log( "*** Expected: " + canConvert );
                Log( "*** Actual:   " + theirAnswer );
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertTo()
        {
            Log( "Testing ConvertTo..." );

            // Note that some of these pass in "false" (not expecting exception)
            //  but in reality they should be "true."  This is by design.
            //  The value will be changed in the test function.  It is much
            //  easier to do this at runtime instead of at CodeGen time.

            TestConvertToWith( FontStyles.Italic, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStyles.Oblique, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStyles.Normal, typeof( InstanceDescriptor ), false );
            TestConvertToWith( null, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStyles.Italic, typeof( string ), false );
            TestConvertToWith( FontStyles.Oblique, typeof( string ), false );
            TestConvertToWith( FontStyles.Normal, typeof( string ), false );
            TestConvertToWith( null, typeof( string ), false );
            TestConvertToWith( FontStyles.Italic, null, true );
            TestConvertToWith( FontStyles.Oblique, null, true );
            TestConvertToWith( FontStyles.Normal, null, true );
            TestConvertToWith( null, null, true );
            TestConvertToWith( FontStyles.Italic, typeof( bool ), true );
            TestConvertToWith( FontStyles.Oblique, typeof( bool ), true );
            TestConvertToWith( FontStyles.Normal, typeof( bool ), true );
            TestConvertToWith( null, typeof( bool ), true );
            TestConvertToWith( FontStyles.Italic, typeof( int ), true );
            TestConvertToWith( FontStyles.Oblique, typeof( int ), true );
            TestConvertToWith( FontStyles.Normal, typeof( int ), true );
            TestConvertToWith( null, typeof( int ), true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToWith( object value, Type destinationType, bool shouldThrow )
        {
            if ( !( value is FontStyle ) )
            {
                shouldThrow = destinationType != typeof( string );
            }

            try
            {
                object theirAnswer = _converter.ConvertTo( value, destinationType );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontStyle tests.
                // The only exception is converting to InstanceDescriptor.  This needs
                //  to be verified and will be done in the next update.

                if ( shouldThrow || failOnPurpose )
                {
                    AddFailure( "ConvertTo failed" );
                    Log( "Expected an exception" );
                    Log( "*** Value:          " + value );
                    Log( "*** ConvertToType:  " + destinationType );
                    Log( "*** TheirAnswer:   " + theirAnswer );
                }
            }
            catch ( Exception ex )
            {
                if ( !shouldThrow ||
                    ( destinationType != null && !( ex is NotSupportedException ) ) ||
                    ( destinationType == null && !( ex is ArgumentNullException ) ) ||
                    failOnPurpose )
                {
                    AddFailure( "ConvertTo failed" );
                    Log( "Unexpected exception thrown\n" + ex );
                    Log( "*** Value:         " + value );
                    Log( "*** ConvertToType: " + destinationType );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertFrom()
        {
            Log( "Testing ConvertFrom..." );

            TestConvertFromWith( "Normal", false );
            TestConvertFromWith( "Italic", false );
            TestConvertFromWith( "Oblique", false );
            TestConvertFromWith( null, true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertFromWith( object value, bool shouldThrow )
        {
            try
            {
                if ( value is string )
                {
                    value = MathEx.ToLocale( (string)value, CultureInfo.InvariantCulture );
                }
                object theirAnswer = _converter.ConvertFrom( value );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontStyle tests.

                if ( shouldThrow || failOnPurpose )
                {
                    AddFailure( "ConvertFrom failed" );
                    Log( "Expected an exception" );
                    Log( "*** Value: " + value );
                    Log( "*** TheirAnswer: " + theirAnswer );
                }
            }
            catch ( Exception ex )
            {
                // Not supported exception is thrown by TypeConverter
                // Invalid operation exception is thrown by FontStyle.Parse( string )

                if ( !shouldThrow ||
                    !( ex is NotSupportedException || ex is InvalidOperationException ) ||
                    failOnPurpose )
                {
                    AddFailure( "ConvertFrom failed" );
                    Log( "Unexpected exception thrown\n" + ex );
                    Log( "*** Value:         " + value );
                }
            }
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -


        private FontStyleConverter _converter;
    }
}

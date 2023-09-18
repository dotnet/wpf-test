// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for FontWeightConverter (generated)
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

    public partial class        FontWeightConverterTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _converter = new FontWeightConverter();
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

            TestConvertToWith( FontWeights.Thin, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.ExtraLight, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.UltraLight, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Light, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Normal, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Regular, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Medium, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.DemiBold, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.SemiBold, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Bold, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.ExtraBold, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.UltraBold, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Black, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Heavy, typeof( InstanceDescriptor ), false );
            TestConvertToWith( null, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontWeights.Thin, typeof( string ), false );
            TestConvertToWith( FontWeights.ExtraLight, typeof( string ), false );
            TestConvertToWith( FontWeights.UltraLight, typeof( string ), false );
            TestConvertToWith( FontWeights.Light, typeof( string ), false );
            TestConvertToWith( FontWeights.Normal, typeof( string ), false );
            TestConvertToWith( FontWeights.Regular, typeof( string ), false );
            TestConvertToWith( FontWeights.Medium, typeof( string ), false );
            TestConvertToWith( FontWeights.DemiBold, typeof( string ), false );
            TestConvertToWith( FontWeights.SemiBold, typeof( string ), false );
            TestConvertToWith( FontWeights.Bold, typeof( string ), false );
            TestConvertToWith( FontWeights.ExtraBold, typeof( string ), false );
            TestConvertToWith( FontWeights.UltraBold, typeof( string ), false );
            TestConvertToWith( FontWeights.Black, typeof( string ), false );
            TestConvertToWith( FontWeights.Heavy, typeof( string ), false );
            TestConvertToWith( null, typeof( string ), false );
            TestConvertToWith( FontWeights.Thin, null, true );
            TestConvertToWith( FontWeights.ExtraLight, null, true );
            TestConvertToWith( FontWeights.UltraLight, null, true );
            TestConvertToWith( FontWeights.Light, null, true );
            TestConvertToWith( FontWeights.Normal, null, true );
            TestConvertToWith( FontWeights.Regular, null, true );
            TestConvertToWith( FontWeights.Medium, null, true );
            TestConvertToWith( FontWeights.DemiBold, null, true );
            TestConvertToWith( FontWeights.SemiBold, null, true );
            TestConvertToWith( FontWeights.Bold, null, true );
            TestConvertToWith( FontWeights.ExtraBold, null, true );
            TestConvertToWith( FontWeights.UltraBold, null, true );
            TestConvertToWith( FontWeights.Black, null, true );
            TestConvertToWith( FontWeights.Heavy, null, true );
            TestConvertToWith( null, null, true );
            TestConvertToWith( FontWeights.Thin, typeof( bool ), true );
            TestConvertToWith( FontWeights.ExtraLight, typeof( bool ), true );
            TestConvertToWith( FontWeights.UltraLight, typeof( bool ), true );
            TestConvertToWith( FontWeights.Light, typeof( bool ), true );
            TestConvertToWith( FontWeights.Normal, typeof( bool ), true );
            TestConvertToWith( FontWeights.Regular, typeof( bool ), true );
            TestConvertToWith( FontWeights.Medium, typeof( bool ), true );
            TestConvertToWith( FontWeights.DemiBold, typeof( bool ), true );
            TestConvertToWith( FontWeights.SemiBold, typeof( bool ), true );
            TestConvertToWith( FontWeights.Bold, typeof( bool ), true );
            TestConvertToWith( FontWeights.ExtraBold, typeof( bool ), true );
            TestConvertToWith( FontWeights.UltraBold, typeof( bool ), true );
            TestConvertToWith( FontWeights.Black, typeof( bool ), true );
            TestConvertToWith( FontWeights.Heavy, typeof( bool ), true );
            TestConvertToWith( null, typeof( bool ), true );
            TestConvertToWith( FontWeights.Thin, typeof( int ), true );
            TestConvertToWith( FontWeights.ExtraLight, typeof( int ), true );
            TestConvertToWith( FontWeights.UltraLight, typeof( int ), true );
            TestConvertToWith( FontWeights.Light, typeof( int ), true );
            TestConvertToWith( FontWeights.Normal, typeof( int ), true );
            TestConvertToWith( FontWeights.Regular, typeof( int ), true );
            TestConvertToWith( FontWeights.Medium, typeof( int ), true );
            TestConvertToWith( FontWeights.DemiBold, typeof( int ), true );
            TestConvertToWith( FontWeights.SemiBold, typeof( int ), true );
            TestConvertToWith( FontWeights.Bold, typeof( int ), true );
            TestConvertToWith( FontWeights.ExtraBold, typeof( int ), true );
            TestConvertToWith( FontWeights.UltraBold, typeof( int ), true );
            TestConvertToWith( FontWeights.Black, typeof( int ), true );
            TestConvertToWith( FontWeights.Heavy, typeof( int ), true );
            TestConvertToWith( null, typeof( int ), true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToWith( object value, Type destinationType, bool shouldThrow )
        {
            if ( !( value is FontWeight ) )
            {
                shouldThrow = destinationType != typeof( string );
            }

            try
            {
                object theirAnswer = _converter.ConvertTo( value, destinationType );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontWeight tests.
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

            TestConvertFromWith( "Thin", false );
            TestConvertFromWith( "ExtraLight", false );
            TestConvertFromWith( "UltraLight", false );
            TestConvertFromWith( "Light", false );
            TestConvertFromWith( "Normal", false );
            TestConvertFromWith( "Regular", false );
            TestConvertFromWith( "Medium", false );
            TestConvertFromWith( "DemiBold", false );
            TestConvertFromWith( "SemiBold", false );
            TestConvertFromWith( "Bold", false );
            TestConvertFromWith( "ExtraBold", false );
            TestConvertFromWith( "UltraBold", false );
            TestConvertFromWith( "Black", false );
            TestConvertFromWith( "Heavy", false );
            TestConvertFromWith( "400", false );
            TestConvertFromWith( "123", false );
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
                //  individual FontWeight tests.

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
                // Invalid operation exception is thrown by FontWeight.Parse( string )

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


        private FontWeightConverter _converter;
    }
}

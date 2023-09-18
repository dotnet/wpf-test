// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/***********************************************************
*
*   Program:   Tests for FontStretchConverter (generated)
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

    public partial class        FontStretchConverterTest : CoreGraphicsTest
    {
        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        public override void    Init( Variation v )
        {
            base.Init( v );

            _converter = new FontStretchConverter();
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

            TestConvertToWith( FontStretches.UltraCondensed, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.ExtraCondensed, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.Condensed, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.SemiCondensed, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.Normal, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.Medium, typeof( InstanceDescriptor ), false );
            TestConvertToWith(  FontStretches.SemiExpanded, typeof( InstanceDescriptor ), false );
            TestConvertToWith(  FontStretches.ExtraExpanded, typeof( InstanceDescriptor ), false );
            TestConvertToWith(  FontStretches.UltraExpanded, typeof( InstanceDescriptor ), false );
            TestConvertToWith( null, typeof( InstanceDescriptor ), false );
            TestConvertToWith( FontStretches.UltraCondensed, typeof( string ), false );
            TestConvertToWith( FontStretches.ExtraCondensed, typeof( string ), false );
            TestConvertToWith( FontStretches.Condensed, typeof( string ), false );
            TestConvertToWith( FontStretches.SemiCondensed, typeof( string ), false );
            TestConvertToWith( FontStretches.Normal, typeof( string ), false );
            TestConvertToWith( FontStretches.Medium, typeof( string ), false );
            TestConvertToWith(  FontStretches.SemiExpanded, typeof( string ), false );
            TestConvertToWith(  FontStretches.ExtraExpanded, typeof( string ), false );
            TestConvertToWith(  FontStretches.UltraExpanded, typeof( string ), false );
            TestConvertToWith( null, typeof( string ), false );
            TestConvertToWith( FontStretches.UltraCondensed, null, true );
            TestConvertToWith( FontStretches.ExtraCondensed, null, true );
            TestConvertToWith( FontStretches.Condensed, null, true );
            TestConvertToWith( FontStretches.SemiCondensed, null, true );
            TestConvertToWith( FontStretches.Normal, null, true );
            TestConvertToWith( FontStretches.Medium, null, true );
            TestConvertToWith(  FontStretches.SemiExpanded, null, true );
            TestConvertToWith(  FontStretches.ExtraExpanded, null, true );
            TestConvertToWith(  FontStretches.UltraExpanded, null, true );
            TestConvertToWith( null, null, true );
            TestConvertToWith( FontStretches.UltraCondensed, typeof( bool ), true );
            TestConvertToWith( FontStretches.ExtraCondensed, typeof( bool ), true );
            TestConvertToWith( FontStretches.Condensed, typeof( bool ), true );
            TestConvertToWith( FontStretches.SemiCondensed, typeof( bool ), true );
            TestConvertToWith( FontStretches.Normal, typeof( bool ), true );
            TestConvertToWith( FontStretches.Medium, typeof( bool ), true );
            TestConvertToWith(  FontStretches.SemiExpanded, typeof( bool ), true );
            TestConvertToWith(  FontStretches.ExtraExpanded, typeof( bool ), true );
            TestConvertToWith(  FontStretches.UltraExpanded, typeof( bool ), true );
            TestConvertToWith( null, typeof( bool ), true );
            TestConvertToWith( FontStretches.UltraCondensed, typeof( int ), true );
            TestConvertToWith( FontStretches.ExtraCondensed, typeof( int ), true );
            TestConvertToWith( FontStretches.Condensed, typeof( int ), true );
            TestConvertToWith( FontStretches.SemiCondensed, typeof( int ), true );
            TestConvertToWith( FontStretches.Normal, typeof( int ), true );
            TestConvertToWith( FontStretches.Medium, typeof( int ), true );
            TestConvertToWith(  FontStretches.SemiExpanded, typeof( int ), true );
            TestConvertToWith(  FontStretches.ExtraExpanded, typeof( int ), true );
            TestConvertToWith(  FontStretches.UltraExpanded, typeof( int ), true );
            TestConvertToWith( null, typeof( int ), true );
        }

        // - - - - - - - - - - - - - - - - - - - - - - - - - - - - -

        private void            TestConvertToWith( object value, Type destinationType, bool shouldThrow )
        {
            if ( !( value is FontStretch ) )
            {
                shouldThrow = destinationType != typeof( string );
            }

            try
            {
                object theirAnswer = _converter.ConvertTo( value, destinationType );

                // We do not verify the conversion here.  This should be done by the
                //  individual FontStretch tests.
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

            TestConvertFromWith( "UltraCondensed", false );
            TestConvertFromWith( "ExtraCondensed", false );
            TestConvertFromWith( "Condensed", false );
            TestConvertFromWith( "SemiCondensed", false );
            TestConvertFromWith( "Normal", false );
            TestConvertFromWith( "Medium", false );
            TestConvertFromWith( "SemiExpanded", false );
            TestConvertFromWith( "ExtraExpanded", false );
            TestConvertFromWith( "UltraExpanded", false );
            TestConvertFromWith( "3", false );
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
                //  individual FontStretch tests.

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
                // Invalid operation exception is thrown by FontStretch.Parse( string )

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

        private FontStretchConverter _converter;
    }
}
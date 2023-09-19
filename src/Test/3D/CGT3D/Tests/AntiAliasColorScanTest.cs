// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Test Anti-Aliasing by color counting in the captured image
    /// </summary>
    public class AntiAliasColorScanTest : RenderingTest3D
    {
        /// <summary/>
        public override void Init( Variation v )
        {
            base.Init( v );

            if ( parameters.Model is GeometryModel3D )
            {
                GeometryModel3D gmodel = parameters.Model as GeometryModel3D;
                SolidColorBrush scb = GetSolidColorBrushFromMaterial( gmodel.Material );
                if ( scb != null )
                {
                    foregroundColor = scb.Color;
                }
                else
                {
                    throw new ApplicationException( "AntiAliasColorScanTest requires a solid color object with a solid color brackground." );
                }
                if ( parameters.Light is AmbientLight )
                {
                    AmbientLight al = parameters.Light as AmbientLight;
                    foregroundColor = ColorOperations.Modulate( foregroundColor, al.Color );
                }
                else
                {
                    throw new ApplicationException( "AntiAliasColorScanTest requires an AmbientLight." );
                }

                // Depending on tier, we want to assert that we DO have AA or that we DON'T have AA
                shouldHaveAntiAliasing = Const.IsVistaOrNewer && RenderCapability.Tier >= 0x00020000;

                // ... but we should override this from script too.
                if ( v[ "ShouldHaveAntiAliasing" ] != null )
                {
                    shouldHaveAntiAliasing = StringConverter.ToBool( v[ "ShouldHaveAntiAliasing" ] );
                }

            }
#if SSL
            else if ( parameters.Model is ScreenSpaceLines3D )
            {
                foregroundColor = ( parameters.Model as ScreenSpaceLines3D ).Color;
            }
#endif
            else
            {
                throw new ApplicationException(
                        "AntiAliasColorScanTest requires a solid color object with a solid color brackground." );
            }
            colors = new Dictionary<Color, int>();
        }

        /// <summary/>
        public override void Verify()
        {
            Color[,] screenCapture = GetScreenCapture();
            GatherColorsFromCapture( screenCapture );

            // print results:
            Log( "Harware Tier Found: 0x{0} ...", RenderCapability.Tier.ToString( "X8" ) );
            foreach ( Color k in colors.Keys )
            {
                Log( "  Color {1} found {0} times", colors[ k ], k );
            }

            // verify:
            Log( "" );
            Log( "Verifying ..." );
            Log( "Foreground = {0}  Background = {1}", foregroundColor, BackgroundColor );

            // Regardles of tier, both scenarios should have fg and bg colors
            bool foregroundFound = false;
            bool backgroundFound = false;
            foreach ( Color k in colors.Keys )
            {
                foregroundFound |= ColorOperations.AreWithinTolerance(
                        k, foregroundColor, RenderTolerance.DefaultColorTolerance );
                backgroundFound |= ColorOperations.AreWithinTolerance(
                        k, BackgroundColor, RenderTolerance.DefaultColorTolerance );
            }
            if ( !foregroundFound )
            {
                AddFailure( "Foreground color {0} NOT found in capture within {1} tolerance. ",
                        foregroundColor, RenderTolerance.DefaultColorTolerance );
            }
            if ( !backgroundFound )
            {
                AddFailure( "Background color {0} NOT found in capture within {1} tolerance. ",
                        BackgroundColor, RenderTolerance.DefaultColorTolerance );
            }

            if ( shouldHaveAntiAliasing )
            {
                // We exepct the colors to be more than the foreground and background asserted above
                if ( colors.Keys.Count > 2 )
                {
                    Log( "Anti-Aliasing is ENABLED!" );
                }
                else
                {
                    Log( "Anti-Aliasing is DISABLED!" );
                    AddFailure( "Only {0} different colors found in capture. ", colors.Keys.Count );
                }
            }
            else
            {
                // We only expect the colors to be the foreground and background
                if ( colors.Keys.Count == 2 )
                {
                    Log( "Anti-Aliasing is DISABLED!" );
                }
                else
                {
                    Log( "Anti-Aliasing is ENABLED!" );
                    AddFailure( "Only {0} different colors found in capture. ", colors.Keys.Count );
                }
            }

            // save image capture on failure
            if ( Failures > 0 )
            {
                PhotoConverter.SaveImageAs( screenCapture, logPrefix + "_Rendered.png" );
                LogImageSaved( "Captured image", logPrefix + "_Rendered.png" );
            }
        }

        /// <summary/>
        protected virtual void GatherColorsFromCapture( Color[,] capture )
        {
            int width = capture.GetLength( 0 );
            int height = capture.GetLength( 1 );
            for ( int y = 0; y < height; y++ )
            {
                for ( int x = 0; x < width; x++ )
                {
                    Color c = capture[ x, y ];
                    if ( colors.ContainsKey( c ) )
                    {
                        // color is already in the list, increment count
                        colors[ c ]++;
                    }
                    else
                    {
                        // new color, add it
                        colors.Add( c, 1 );
                    }
                }
            }
        }

        /// <summary/>
        protected SolidColorBrush GetSolidColorBrushFromMaterial( Material m )
        {
            if ( m is DiffuseMaterial )
            {
                DiffuseMaterial dm = m as DiffuseMaterial;
                return dm.Brush as SolidColorBrush;
            }
            else if ( m is EmissiveMaterial )
            {
                // Emissive needs to be blended with the background - ignore it.
            }
            else if ( m is SpecularMaterial )
            {
                // Specular needs directional/point light - ignore it.
            }
            else if ( m is MaterialGroup )
            {
                MaterialGroup mg = m as MaterialGroup;
                if ( mg.Children.Count == 1 )
                {
                    return GetSolidColorBrushFromMaterial( mg.Children[0] );
                }
            }
            return null;
        }

        /// <summary/>
        protected Color foregroundColor = Colors.Transparent;
        /// <summary/>
        protected Dictionary<Color, int> colors = null;
        /// <summary/>
        protected bool shouldHaveAntiAliasing;
    }
}


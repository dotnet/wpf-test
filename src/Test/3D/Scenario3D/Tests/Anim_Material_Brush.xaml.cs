// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class Anim_Material_Brush : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nParameters:" +
                    "\n   /Brush                One of SolidColorBrush, LinearGradientBrush," +
                    "\n                         RadialGradientBrush, ImageBrush, DrawingBrush," +
                    "\n                         VisualBrush. (default:SolidColorBrush)" +
                    "\n   /Material             One of DiffuseMaterial, SpecularMaterial, EmissiveMaterial," +
                    "\n                         MaterialGroup. (default:DiffuseMaterial)" +
                    "\n   /SpecularPower        Specular power for SpecularMaterial. (default:20)" +
                    "\n" +
                    "\nFlags:" +
                    "\n   /UseStandardBrush     use a brush with no animations. (default:false)" +
                    "\n   /UseCubeMesh          use a cube mesh instead of a sphere. (default:false)" +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            ParseParameters();
            BuildScene();
        }

        string _brush = "SolidColorBrush";
        string _material = "DiffuseMaterial";
        bool _useStandardBrush = false;
        bool _useCubeMesh = false;
        double _specularPower = 20;

        public void ParseParameters()
        {
            Application application = Application.Current as Application;
            if ( application.Properties[ "Brush" ] != null )
            {
                _brush = application.Properties[ "Brush" ] as string;
            }
            if ( application.Properties[ "Material" ] != null )
            {
                _material = application.Properties[ "Material" ] as string;
            }
            if ( application.Properties[ "SpecularPower" ] != null )
            {
                _specularPower = double.Parse( application.Properties[ "SpecularPower" ] as string );
            }
            if ( application.Properties[ "UseStandardBrush" ] != null )
            {
                _useStandardBrush = true;
            }
            if ( application.Properties[ "UseCubeMesh" ] != null )
            {
                _useCubeMesh = true;
            }
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Direction = new Vector3D( 0, -1, -1 );

            MeshGeometry3D mesh = _useCubeMesh ? MeshFactory.SimpleCubeMesh : MeshFactory.FullScreenMesh;

            Brush b = null;
            switch ( _brush )
            {
            case "SolidColorBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushSolidColor 
                            : BrushFactory.BrushSolidColorAnimated;
                break;
            case "LinearGradientBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushLinearGradient 
                            : BrushFactory.BrushLinearGradientAnimated;
                break;
            case "RadialGradientBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushRadialGradient 
                            : BrushFactory.BrushRadialGradientAnimated;
                break;
            case "DrawingBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushDrawing 
                            : BrushFactory.BrushDrawingAnimated;
                break;
            case "ImageBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushImage 
                            : BrushFactory.BrushImageAnimated;
                break;
            case "VisualBrush":
                b = ( _useStandardBrush ) ? 
                            BrushFactory.BrushVisual
                            : BrushFactory.BrushVisualAnimated;
                break;

            default:
                throw new ApplicationException( "Unexpected brush: " + _brush );
            }

            Material mat = null;
            switch ( _material )
            {
            case "SpecularMaterial":
                mat = new SpecularMaterial( b, _specularPower );
                break;
            case "EmissiveMaterial":
                mat = new EmissiveMaterial( b );
                break;
            case "DiffuseMaterial":
                mat = new DiffuseMaterial( b );
                break;
            case "MaterialGroup":
                MaterialGroup matg = new MaterialGroup();
                matg.Children.Add( new DiffuseMaterial( b ) );
                mat = matg;
                break;
            default:
                throw new ApplicationException( "Unexpected material: " + _material );
            }

            GeometryModel3D primitive = new GeometryModel3D( mesh, mat );
            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            VIEWPORT.Camera = CameraFactory.PerspectiveDefault;
        }
    }
}

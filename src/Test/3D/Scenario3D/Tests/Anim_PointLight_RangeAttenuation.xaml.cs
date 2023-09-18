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

// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    public partial class Anim_PointLight_RangeAttenuation : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /UseSpotLight              use a SpotLight instead of a PointLight." +
                    "\n   /TestRange                 test Range animations." +
                    "\n   /TestLinearAttenuation     test LinearAttenuation animations." +
                    "\n   /TestQuadraticAttenuation  test QuadraticAttenuation animations." +
                    "\n   /TestConstantAttenuation   test ConstantAttenuation animations." +
                    "\n   /Brush                     string using brush syntax for BVTs. (OPTIONAL)" +
                    "\n   /Material                  one of Diffuse, Specular, Emissive. (OPTIONAL)" +
                    "\n   /ReverseOrder              reverse the order panels are generated. (OPTIONAL)" +
                    "\n   /UseSphereMesh             use a sphere. (OPTIONAL)" +
                    "\n   /Scale=<sx,sy,sz>          Sets a scale vector to transform on the light. (OPTIONAL)" +
                    "\n   /RotationAngle=<ry>        Sets the angle to rotate the light on the y-axis. (OPTIONAL)" +
                    "\n   /Translate=<tx,ty,tz>      Sets a translate vector to transform the light. (OPTIONAL)" +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        string _brush = null;
        string _material = null;
        double _specularPower = 12;

        public void BuildScene()
        {
            Application application = Application.Current as Application;
            PointLightBase light;
            MeshGeometry3D mesh;

            if ( application.Properties[ "UseSpotLight" ] != null )
            {
                // using a spotlight
                light = new SpotLight();
                // small cone angle so we see the cone through the panes
                ( (SpotLight)light ).InnerConeAngle = 4.0;
                ( (SpotLight)light ).OuterConeAngle = 6.5;
                // perpendicular to all planes
                ( (SpotLight)light ).Direction = new Vector3D( 0, 0, -1 );
            }
            else
            {
                light = new PointLight();
                // mesh = MeshFactory.FullScreenMesh;
            }

            // highly tesselated grid so we see the cone
            mesh = MeshFactory.CreateFlatGridUV( 20, 20, 0, new Point( 0, 0 ), new Point( 1, 1 ) );

            // override mesh with sphere, so that interpenetration occurs
            if ( application.Properties[ "UseSphereMesh" ] != null )
            {
                mesh = MeshFactory.Sphere( 20, 20, 2.5 );
            }

            // common default parameters    
            light.Position = new Point3D( 0, 0, 0 );
            // Attenuation A^0+A^1+A^2 needs to be > 0
            light.ConstantAttenuation = 1;  // this is to prevent overflow, it can be overriden later
            light.LinearAttenuation = 0;
            light.QuadraticAttenuation = 0;
            light.Range = 200;

            Transform3DGroup tc = new Transform3DGroup();
            tc.Children = new Transform3DCollection( new Transform3D[] { Transform3D.Identity } );
            light.Transform = tc;

            if ( application.Properties[ "Scale" ] != null )
            {

                // Test for Scale
                Vector3D scaleVector = StringConverter.ToVector3D( application.Properties[ "Scale" ] as string );
                ScaleTransform3D scale = new ScaleTransform3D( scaleVector );
                tc.Children.Add( scale );
            }

            if ( application.Properties[ "RotationAngle" ] != null )
            {
                // Test for Rotation
                RotateTransform3D rotation = new RotateTransform3D();
                double angle = StringConverter.ToDouble( application.Properties[ "RotationAngle" ] as string );
                rotation.Rotation = new AxisAngleRotation3D( new Vector3D( 0, 1, 0 ), angle );
                tc.Children.Add( rotation );
            }

            if ( application.Properties[ "Translate" ] != null )
            {
                // Test for Translation
                Vector3D translateVector = StringConverter.ToVector3D( application.Properties[ "Translate" ] as string );
                TranslateTransform3D translate = new TranslateTransform3D( translateVector );
                tc.Children.Add( translate );
            }

            // Test for Range
            if ( application.Properties[ "TestRange" ] != null )
            {
                light.Range = 0;  // this will be the default, which should be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 5.0, 15.0, new Duration( TimeSpan.FromMilliseconds( 1000 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( PointLight.RangeProperty, da );
            }

            // Test for LinearAttenuation
            if ( application.Properties[ "TestLinearAttenuation" ] != null )
            {
                light.ConstantAttenuation = 0;
                light.LinearAttenuation = -1;  // this will be be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 0.01, 0.25, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( PointLight.LinearAttenuationProperty, da );
            }

            // Test for QuadraticAttenuation
            if ( application.Properties[ "TestQuadraticAttenuation" ] != null )
            {
                light.ConstantAttenuation = 0;
                light.QuadraticAttenuation = -1;  // this will be be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 0.001, 0.15, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( PointLight.QuadraticAttenuationProperty, da );
            }

            // Test for ConstantAttenuation
            if ( application.Properties[ "TestConstantAttenuation" ] != null )
            {
                light.ConstantAttenuation = -1;  // this will be be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 1.0, 3.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( PointLight.ConstantAttenuationProperty, da );
            }

            PerspectiveCamera camera = (PerspectiveCamera)CameraFactory.PerspectiveDefault;
            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 200;
            camera.Position = new Point3D( 4, 0, 0 );
            camera.LookDirection = new Vector3D( -14, 0, -30 );
            VIEWPORT.Camera = camera;

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );

            // Parse material parameters
            if ( application.Properties[ "Material" ] != null )
            {
                _material = application.Properties[ "Material" ] as string;
            }
            if ( application.Properties[ "Brush" ] != null )
            {
                _brush = application.Properties[ "Brush" ] as string;
            }
            
            Material mat = MaterialFactory.Default;
            if ( _material != null && _brush != null )
            {
                mat = MaterialFactory.MakeMaterial( _brush, _material, _specularPower );
            }

            if ( application.Properties[ "ReverseOrder" ] != null )
            {
                for ( int i = 9; i >= 0; i-- )
                {
                    GeometryModel3D primitive = new GeometryModel3D( mesh, mat );
                    primitive.Transform = new TranslateTransform3D( new Vector3D( 0, 0, i * 3.0 -30 ) );
                    mg.Children.Add( primitive );
                }
            }
            else
            {
                for ( int i = 0; i < 10; i++ )
                {
                    GeometryModel3D primitive = new GeometryModel3D( mesh, mat );
                    primitive.Transform = new TranslateTransform3D( new Vector3D( 0, 0, i * 3.0 -30 ) );
                    mg.Children.Add( primitive );
                }
            }

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }
    }
}
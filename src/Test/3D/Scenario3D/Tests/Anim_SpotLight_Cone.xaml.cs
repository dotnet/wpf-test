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
    public partial class Anim_SpotLight_Cone : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /TestInnerConeAngle   test InnerConeAngle animations." +
                    "\n   /TestOuterConeAngle   test OuterConeAngle animations." +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            BuildScene();
        }

        public void BuildScene()
        {
            Application application = Application.Current as Application;

            SpotLight light = new SpotLight();

            // constant parameters
            light.ConstantAttenuation = 1;
            light.LinearAttenuation = 0;
            light.QuadraticAttenuation = 0;
            light.Range = 100;
            light.Color = Colors.White;
            light.Position = new Point3D( 0, 0, 1.5 );
            light.Direction = new Vector3D( 0, 0, -1 );
            PerspectiveCamera camera = CameraFactory.PerspectiveDefault;
            camera.Position = new Point3D( 0, 0, 3 );
            VIEWPORT.Camera = camera;

            // defaults                
            light.InnerConeAngle = 25;
            light.OuterConeAngle = 45;

            // Test for InnerConeAngle
            if ( application.Properties[ "TestInnerConeAngle" ] != null )
            {
                light.InnerConeAngle = 0;  // this will be the default, which should be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 0.0, 35.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( SpotLight.InnerConeAngleProperty, da );
            }
            // Test for OuterConeAngle
            if ( application.Properties[ "TestOuterConeAngle" ] != null )
            {
                light.OuterConeAngle = 0;  // this will be the default, which should be overriden by the animation
                DoubleAnimation da = new DoubleAnimation( 30.0, 75.0, new Duration( TimeSpan.FromMilliseconds( 1500 ) ) );
                da.AutoReverse = true;
                da.RepeatBehavior = RepeatBehavior.Forever;
                light.BeginAnimation( SpotLight.OuterConeAngleProperty, da );
            }

            MeshGeometry3D mesh1 = MeshFactory.CreateFlatGridUV( 40, 40, 0, new Point( 0, 0 ), new Point( 1, 1 ) );
            Material mat1 = MaterialFactory.Default;
            GeometryModel3D primitive1 = new GeometryModel3D( mesh1, mat1 );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( primitive1 );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
        }
    }
}

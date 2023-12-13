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
using Microsoft.Test.Graphics.Factories;
using Microsoft.Test.Graphics.ReferenceRender;

namespace Microsoft.Test.Graphics
{
    public partial class Chain : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /ModelCount          (Int32)    Number of models in chain. [ default /ModelCount=20 ] " +
                    "\n   /CameraPosition      (Point3D)  Camera position. [ default /CameraPosition=4,10,30 ] " +
                    "\n   /CameraLookAtPoint   (Point3D)  Camera look at. [ default /CameraLookAtPoint=-10,0,0 ] " +
                    "\n   /TranslateVector     (Vector3D) Translate vector. [ default /TranslateVector=0,.5,1 ]" +
                    "\n   /RotateAxisVector    (Vector3D) Axis of rotation. [ default /RotateAxisVector=0,1,0 ]" +
                    "\n   /RotateAngle         (Double)   Angle of rotation. [ default /RotateAngle=-2.5 ]" +
                    "\n   /Mesh                (String)   Mesh to use. [ default /Mesh=SingleFrontFacingTriangle ]" +
                    "\n   /AnimateMaterial     (Flag)     Set this to use an animated material. [ default is flag not set ]" +
                    "\n   /AnimateCamera       (Flag)     Set this to use an animated camera. [ default is flag not set ]" +
                    "\n" +
                    "\nSample Usage:" +
                    "\n   Scenario3D.exe /scenario=Chain.baml /ModelCount=200 /TranslateVector=0,.02,.4" +
                    "\n   Scenario3D.exe /scenario=Chain.baml /ModelCount=100 /TranslateVector=0,.05,.5 /RotateAngle=-5.42 /Mesh=\"Sphere 10 15 .5\" " +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            RenderTolerance.TextureLookUpTolerance = .5;
            RenderTolerance.PixelToEdgeTolerance = .2;
            ParseParameters();
            BuildScene();
        }

        int _modelCount = 20;
        Point3D _cameraPosition = new Point3D( 4, 10, 30 );
        Point3D _cameraLookAtPoint = new Point3D( -10, 0, 0 );
        Vector3D _translateVector = new Vector3D( 0, .5, 1 );
        Vector3D _rotateAxisVector = new Vector3D( 0, 1, 0 );
        double _rotateAngle = -2.5;
        string _mesh = String.Empty;
        bool _animateMaterial = false;
        bool _animateCamera = false;

        public void ParseParameters()
        {
            Application application = Application.Current as Application;
            if ( application.Properties[ "ModelCount" ] != null )
            {
                _modelCount = StringConverter.ToInt( (string)application.Properties[ "ModelCount" ] );
            }
            if ( application.Properties[ "CameraPosition" ] != null )
            {
                _cameraPosition = StringConverter.ToPoint3D( (string)application.Properties[ "CameraPosition" ] );
            }
            if ( application.Properties[ "CameraLookAtPoint" ] != null )
            {
                _cameraLookAtPoint = StringConverter.ToPoint3D( (string)application.Properties[ "CameraLookAtPoint" ] );
            }
            if ( application.Properties[ "TranslateVector" ] != null )
            {
                _translateVector = StringConverter.ToVector3D( (string)application.Properties[ "TranslateVector" ] );
            }
            if ( application.Properties[ "RotateAxisVector" ] != null )
            {
                _rotateAxisVector = StringConverter.ToVector3D( (string)application.Properties[ "RotateAxisVector" ] );
            }
            if ( application.Properties[ "RotateAngle" ] != null )
            {
                _rotateAngle = StringConverter.ToDouble( (string)application.Properties[ "RotateAngle" ] );
            }
            if ( application.Properties[ "Mesh" ] != null )
            {
                _mesh = (string)application.Properties[ "Mesh" ];
            }
            if ( application.Properties[ "AnimateMaterial" ] != null )
            {
                _animateMaterial = true;
            }
            if ( application.Properties[ "AnimateCamera" ] != null )
            {
                _animateCamera = true;
            }
        }

        Model3D MakeModel( string meshString, Material material )
        {
            Model3D model = null;
            if ( meshString.StartsWith( "GroupSphere" ) )
            {
                // use sphereGroup ... 
                string[] parts = meshString.Split( new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries );
                model = SceneFactory.GroupSphere(
                        StringConverter.ToInt( parts[ 1 ] ),    // latitude
                        StringConverter.ToInt( parts[ 2 ] ),    // longitude
                        StringConverter.ToDouble( parts[ 3 ] ), // radius
                        material );
            }
            else
            {
                // use meshfactory or default
                MeshGeometry3D mesh = null;
                if ( meshString == String.Empty || meshString == null )
                {
                    mesh = MeshFactory.SingleFrontFacingTriangle;
                }
                else
                {
                    mesh = MeshFactory.MakeMesh( meshString );
                }
                model = new GeometryModel3D( mesh, material );
            }
            return model;
        }

        public void BuildScene()
        {
            DirectionalLight light = new DirectionalLight();
            light.Direction = _cameraLookAtPoint - _cameraPosition;

            Material mat = MaterialFactory.Default;
            if ( _animateMaterial )
            {
                mat = new DiffuseMaterial( BrushFactory.BrushSolidColorAnimated );
            }
            Model3D primitive1 = MakeModel( _mesh, mat );

            TranslateTransform3D translate = new TranslateTransform3D( _translateVector );
            RotateTransform3D rotate = new RotateTransform3D( new AxisAngleRotation3D( _rotateAxisVector, _rotateAngle ) );

            Transform3DGroup tx = new Transform3DGroup();
            tx.Children = new Transform3DCollection( new Transform3D[] { translate, rotate } );

            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add( light );
            mg.Children.Add( BuildModelChain( primitive1, tx, _modelCount ) );
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );

            // add camera
            ProjectionCamera camera = CameraFactory.PerspectiveDefault;
            camera.NearPlaneDistance = 1;
            camera.FarPlaneDistance = 200;
            camera.Position = _cameraPosition;
            camera.LookDirection = _cameraLookAtPoint - _cameraPosition;
            // add camera animation, if specified
            if ( _animateCamera )
            {
                Vector3D from = camera.LookDirection;
                from.Y -= 0.45;
                Vector3D to = camera.LookDirection;
                to.Y += 0.45;
                camera.LookDirection = new Point3D( 0, 0, 0 ) - camera.Position;

                Vector3DAnimation va = new Vector3DAnimation( from, to, TimeSpan.FromMilliseconds( 1500 ) );
                va.AutoReverse = true;
                va.RepeatBehavior = RepeatBehavior.Forever;
                camera.BeginAnimation( ProjectionCamera.LookDirectionProperty, va );
            }
            VIEWPORT.Camera = camera;
        }

        /// <summary>
        /// Creates a chain of models, each with the given relative transform
        /// </summary>
        public static Model3DGroup BuildModelChain( Model3D model, Transform3D transform, int count )
        {
            Model3DGroup group = new Model3DGroup();

            Model3DGroup last = group;
            for ( int i = 0; i < count; i++ )
            {
                Model3DGroup current = new Model3DGroup();
                // add child
                current.Children.Add( model );
                current.Transform = transform;
                // add to parent            
                last.Children.Add( current );
                // update chain
                last = current;
            }
            return group;
        }

    }
}

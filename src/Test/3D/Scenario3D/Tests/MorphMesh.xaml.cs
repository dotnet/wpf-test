// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.IO;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class MorphMesh : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nParameters:" +
                    "\n   /Image=                 (URI) Set to use as mesh material. Default is 'tahiti2.jpg'." +
                    "\n   /Video=                 (URI) If set, this replaces the image above with a video. Default is null." +
                    "\n   /HeightFieldURI=        (URI) Path to image file to use as mesh heightfield. Default is 'heightfield.bmp'" +
                    "\n   /AnimationFrames=       (Int) Number of cached animation frames for mesh morphing. Defult is 30." +
                    "\n   /RefractionIndex=       (Double) Index of refraction for UV mapping. Default is 0.1." +
                    "\n   /UseVisualBrush         (Flag) Set to add a VisualBrush on an EmissiveMaterial." +
                    "\n   /DisconnectVisualBrush  (Flag) Set to disconnect the VisualBrush from the main Visual Tree before using." +
                    "\n   /NoMeshAnimation        (Flag) Set to use a flat mesh without a morphing animation." +
                    "";
            }
        }

        string _videoURI = null;
        string _imageURI = "tahiti2.jpg";
        string _heightFieldURI = "heightfield.bmp";
        int _animationFrames = 30;
        double _refractIndex = 0.1;
        bool _useVisualBrush = false;
        bool _disconnectVisualBrush = false;
        bool _noMeshAnimation = false;

        public void OnLoaded(object sender, EventArgs args)
        {
            // Parse parameters
            Application app = Application.Current as Application;
            if (app.Properties["Video"] != null)
            {
                _videoURI = app.Properties["Video"] as string;
            }
            if (app.Properties["Image"] != null)
            {
                _imageURI = app.Properties["Image"] as string;
            }
            if (app.Properties["AnimationFrames"] != null)
            {
                _animationFrames = int.Parse(app.Properties["AnimationFrames"] as string);
            }
            if (app.Properties["HeightFieldURI"] != null)
            {
                _heightFieldURI = app.Properties["HeightFieldURI"] as string;
            }
            if (app.Properties["RefractionIndex"] != null)
            {
                _refractIndex = double.Parse(app.Properties["RefractionIndex"] as string);
            }
            if (app.Properties["UseVisualBrush"] != null)
            {
                _useVisualBrush = bool.Parse(app.Properties["UseVisualBrush"] as string);
            }
            if (app.Properties["DisconnectVisualBrush"] != null)
            {
                _disconnectVisualBrush = bool.Parse(app.Properties["DisconnectVisualBrush"] as string);
            }
            if (app.Properties["NoMeshAnimation"] != null)
            {
                _noMeshAnimation = bool.Parse(app.Properties["NoMeshAnimation"] as string);
            }

            BuildScene();
        }

        public void BuildScene()
        {
            // set camera
            ProjectionCamera camera = CameraFactory.PerspectiveDefault;

            // create material
            MaterialGroup mat = new MaterialGroup();
            if (_videoURI != null)
            {
                mat.Children.Add(MakeVideoMaterial(new Uri(_videoURI, UriKind.RelativeOrAbsolute)));
            }
            else
            {
                ImageBrush ib = new ImageBrush(new BitmapImage(new Uri(_imageURI, UriKind.RelativeOrAbsolute)));
                mat.Children.Add(new DiffuseMaterial(ib));
            }

            if (_useVisualBrush)
            {
                Border caption = CAPTION;
                VisualBrush vb = new VisualBrush(caption);

                mat.Children.Add(new EmissiveMaterial(vb));

                if (_disconnectVisualBrush)
                {
                    DockPanel host = VISUAL_BRUSH_HOST;
                    host.Children.Clear();
                }
            }
            else
            {
                DockPanel host = VISUAL_BRUSH_HOST;
                host.Children.Clear();
            }

            // create from and to meshes from images
            BitmapImage bmpA = new BitmapImage(new Uri(_heightFieldURI, UriKind.RelativeOrAbsolute));
            // 'B' is just 'A' rotated 90 degrees at the source
            BitmapImage bmpB = new BitmapImage();
            bmpB.BeginInit();
            bmpB.Rotation = Rotation.Rotate90;
            bmpB.UriSource = new Uri(_heightFieldURI, UriKind.RelativeOrAbsolute);
            bmpB.EndInit();
            MeshGeometry3D heightfieldA = MeshFactory.NormalizedHeightFieldFromBitmap(bmpA);
            MeshGeometry3D heightfieldB = MeshFactory.NormalizedHeightFieldFromBitmap(bmpB);

            // add refraction effect;
            Vector3D look = camera.LookDirection;
            MeshOperations.CalculateRefractedUV(heightfieldA, look, _refractIndex);
            MeshOperations.CalculateRefractedUV(heightfieldB, look, _refractIndex);

            // build geometry
            GeometryModel3D primitive = new GeometryModel3D(null, mat);

            if (_noMeshAnimation)
            {
                heightfieldA = MeshFactory.NormalizedHeightFieldFromBitmap(bmpA);
                primitive = new GeometryModel3D(heightfieldA, mat);
                primitive.Transform = new ScaleTransform3D(new Vector3D(3, 3, 1));
            }
            else
            {
                // create mesh animation
                MeshAnimation mesa = new MeshAnimation(heightfieldA, heightfieldB, _animationFrames);
                mesa.BeginTime = TimeSpan.FromMilliseconds(0);
                mesa.Duration = new Duration(TimeSpan.FromMilliseconds(1000));
                mesa.RepeatBehavior = RepeatBehavior.Forever;
                mesa.AutoReverse = true;
                primitive.BeginAnimation(GeometryModel3D.GeometryProperty, mesa);
                primitive.Transform = new ScaleTransform3D(new Vector3D(4, 4, 1));
            }
            // build scene
            Model3DGroup mg = new Model3DGroup();
            mg.Children.Add(new AmbientLight(Colors.DarkBlue));
            mg.Children.Add(new DirectionalLight(Colors.White, look));
            mg.Children.Add(primitive);
            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add(visual);
            VIEWPORT.Camera = camera;
        }

        public Material MakeVideoMaterial(Uri videoUri)
        {
            VideoDrawing vd = new VideoDrawing();
            MediaPlayer mp = new MediaPlayer();
            mp.Open(videoUri);
            mp.Play();

            vd.Player = mp;
            vd.Rect = new Rect(0, 0, 256, 256);

            DrawingBrush db = new DrawingBrush();
            db.Drawing = vd;

            return new DiffuseMaterial(db);
        }
    }
}

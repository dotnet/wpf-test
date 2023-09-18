// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using Microsoft.Test.Graphics.ReferenceRender;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class GlassBitmap : ScenarioUtility.IHelp
    {

        #region help

        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /Image               path to imagefile to load as window surface." +
                    "\n   /SecondImage         path to imagefile to load as window surface for morph target." +
                    "\n   /UseRadialGradient   use a RadialGradientBrush to render the image." +
                    "\n   /UseAmbientLight     use an AmbientLight instead of the default DirectionalLight." +
                    "\n   /UseFlatBackground   use a flat background with no readback for the topmost layer." +
                    "\n   /DownSampleFactor    (int) divide the texture size by this to blur it by bilinear filtering." +
                    "\n   /AnimateLight        animate the directional light and use AdvancedMaterial for specular." +
                    "\n   /AnimateMesh         animate the mesh geometry by changing the index of refraction of the material." +
                    "\n   /AnimationDuration   (int) milliseconds the animations take, default is 1200." +
                    "\n   /AnimationFrames     (int) number of frames to subdivide the mesh animation, default is 12." +
                    "";
            }
        }

        // defaults for parameters
        string _image = "heightfield.bmp";
        string _secondImage = null;
        bool _useRadialGradient = false;
        bool _useAmbientLight = false;
        bool _useFlatBackground = false;
        int _downSampleFactor = 1;
        bool _animateLight = false;
        bool _animateMesh = false;
        int _animationDuration = 1200;
        int _animationFrames = 12;

        void ParseParameters()
        {
            Application application = Application.Current as Application;
            if ( application.Properties[ "Image" ] != null )
            {
                _image = application.Properties[ "Image" ] as string;
            }
            if ( application.Properties[ "SecondImage" ] != null )
            {
                _secondImage = application.Properties[ "SecondImage" ] as string;
            }
            if ( application.Properties[ "UseRadialGradient" ] != null )
            {
                _useRadialGradient = true;
            }
            if ( application.Properties[ "UseAmbientLight" ] != null )
            {
                _useAmbientLight = true;
            }
            if ( application.Properties[ "UseFlatBackground" ] != null )
            {
                _useFlatBackground = true;
            }
            if ( application.Properties[ "DownSampleFactor" ] != null )
            {
                _downSampleFactor = int.Parse( application.Properties[ "DownSampleFactor" ] as string );
            }
            if ( application.Properties[ "AnimateLight" ] != null )
            {
                _animateLight = true;
            }
            if ( application.Properties[ "AnimateMesh" ] != null )
            {
                _animateMesh = true;
            }
            if ( application.Properties[ "AnimationDuration" ] != null )
            {
                _animationDuration = int.Parse( application.Properties[ "AnimationDuration" ] as string );
            }
            if ( application.Properties[ "AnimationFrames" ] != null )
            {
                _animationFrames = int.Parse( application.Properties[ "AnimationFrames" ] as string );
            }
        }

        #endregion

        #region event handlers

        void OnLoaded( object sender, EventArgs e )
        {
            RenderTolerance.PixelToEdgeTolerance = 0;

            Window w = Application.Current.Windows[ 0 ];
            _windowBounds = new Int32Rect( (int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height );
            w.ContentRendered += new EventHandler( MainWindow_ContentRendered );
            w.PreviewMouseLeftButtonDown += new MouseButtonEventHandler( MainWindow_MouseLeftButtonDown );
            w.PreviewMouseMove += new MouseEventHandler( MainWindow_MouseMove );
            w.WindowStyle = WindowStyle.None;
            w.ResizeMode = ResizeMode.NoResize;
            w.WindowState = WindowState.Minimized;

            ParseParameters();
        }

        void MainWindow_ContentRendered( object sender, EventArgs e )
        {
            Window w = Application.Current.MainWindow;
            _wholeBackground = ScenarioUtility.ScreenCapture.TakeScreenCapture( IntPtr.Zero );
            _currentBackground = new CroppedBitmap( _wholeBackground, _windowBounds );
            _bgPicture = new ImageBrush( _currentBackground );
            w.Background = _bgPicture;
            BuildScene();
            w.WindowState = WindowState.Normal;
        }

        void MainWindow_MouseLeftButtonDown( object sender, MouseButtonEventArgs e )
        {
            _startPosition = e.GetPosition( Application.Current.MainWindow );
        }

        void MainWindow_MouseMove( object sender, MouseEventArgs e )
        {
            if ( e.LeftButton == MouseButtonState.Pressed )
            {
                Window w = Application.Current.MainWindow;
                Point postion = e.GetPosition( w );
                w.Top = w.Top + postion.Y - _startPosition.Y;
                w.Left = w.Left + postion.X - _startPosition.X;
                Int32Rect rc = new Int32Rect( (int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height );
                _currentBackground = new CroppedBitmap( _wholeBackground, rc );
                _bgPicture.ImageSource = _currentBackground;
                if ( _downSampleFactor > 1 )
                {
                    BitmapSource foo = TextureGenerator.RenderBrushToImageData(
                            _bgPicture,
                            ( (BitmapSource)_bgPicture.ImageSource ).PixelWidth / _downSampleFactor,
                            ( (BitmapSource)_bgPicture.ImageSource ).PixelHeight / _downSampleFactor );
                    _bgPicture.ImageSource = foo;
                }
                if ( !_useAmbientLight )
                {
                    ( (DirectionalLight)_light ).Direction = new Vector3D(
                            w.Left / 600 - 1,
                            w.Top / 512 - 1,
                            -1 );
                }
            }
        }

        #endregion

        #region 3D utilities

        void BuildScene()
        {
            Model3DGroup mg = new Model3DGroup();

            if ( _useAmbientLight )
            {
                _light = new AmbientLight();
            }
            else
            {
                Window w = Application.Current.MainWindow;
                _light = new DirectionalLight();
                if ( !_useAmbientLight )
                {
                    ( (DirectionalLight)_light ).Direction = new Vector3D(
                                _windowBounds.X / 600 - 1,
                                _windowBounds.Y / 512 - 1,
                                -1 );
                }

                if ( _animateLight )
                {
                    Vector3DAnimation va = new Vector3DAnimation(
                            new Vector3D( -1, -1, -1 ),
                            new Vector3D( 1, 1, -1 ),
                            new Duration( TimeSpan.FromMilliseconds( _animationDuration ) ) );
                    va.AutoReverse = true;
                    va.RepeatBehavior = RepeatBehavior.Forever;
                    _light.BeginAnimation( DirectionalLight.DirectionProperty, va );
                }
            }
            mg.Children.Add( _light );

            Material mat = new DiffuseMaterial( _bgPicture );
            if ( !_useAmbientLight )
            {
                MaterialGroup matg = new MaterialGroup();
                matg.Children.Add( new DiffuseMaterial( Brushes.Black ) );
                matg.Children.Add( new EmissiveMaterial( _bgPicture ) );
                matg.Children.Add( new SpecularMaterial( Brushes.Gray, 50 ) );
                mat = matg;
            }

            Material backgroundMat = mat;
            if ( _useFlatBackground )
            {
                backgroundMat = MaterialFactory.Default;
            }
            GeometryModel3D primitive1 = new GeometryModel3D( MeshFactory.FullScreenMesh, backgroundMat );
            primitive1.Transform = new TranslateTransform3D( new Vector3D( 0, 0, 5 ) );
            mg.Children.Add( primitive1 );

            DiscreteAnimation disa = null;
            if ( _windowFrame == null )
            {
                BitmapSource id;
                BitmapSource idSecond;
                if ( _useRadialGradient )
                {
                    RadialGradientBrush rgb = new RadialGradientBrush( Colors.White, Colors.Black );
                    id = TextureGenerator.RenderBrushToImageData( rgb, 100, 100 );
                }
                else
                {
                    id = new BitmapImage(
                            new Uri( _image, UriKind.RelativeOrAbsolute ) );
                }
                _windowFrame = HeightFieldFromBitmapImage( id );

                if ( _animateMesh )
                {
                    double indexFrom = 0.0;
                    double indexTo = 1.0;
                    if ( _secondImage != null )
                    {
                        idSecond = new BitmapImage( new Uri( _secondImage, UriKind.RelativeOrAbsolute ) );
                        indexFrom = 1.0;
                    }
                    else
                    {
                        idSecond = id;
                    }
                    disa = new MeshAnimation(
                                HeightFieldFromBitmapImage( id, indexFrom ),
                                HeightFieldFromBitmapImage( idSecond, indexTo ),
                                _animationFrames );
                    disa.BeginTime = TimeSpan.FromMilliseconds( 0 );
                    disa.Duration = new Duration( TimeSpan.FromMilliseconds( _animationDuration ) );
                    disa.RepeatBehavior = RepeatBehavior.Forever;
                    disa.AutoReverse = true;
                }

            }
            GeometryModel3D primitive4 = new GeometryModel3D( _windowFrame, mat );
            if ( _animateMesh )
            {
                primitive4.BeginAnimation( GeometryModel3D.GeometryProperty, disa );
            }
            mg.Children.Add( primitive4 );

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            VIEWPORT.Camera = CameraFactory.OrthographicDefault;
        }

        MeshGeometry3D HeightFieldFromBitmapImage( BitmapSource image )
        {
            // use 1.0 which is about right for glass ...
            return HeightFieldFromBitmapImage( image, 1.0 );
        }

        MeshGeometry3D HeightFieldFromBitmapImage( BitmapSource image, double indexOfRefraction )
        {
            // extract pixel data, 4 bytes since one per channel ARGB
            byte[] pixels = new byte[ image.PixelWidth * image.PixelHeight * 4 ];
            image.CopyPixels( pixels, image.PixelWidth * 4, 0 );

            MeshGeometry3D mesh = new MeshGeometry3D();
            double ySize = (double)( image.PixelHeight -1 );
            double xSize = (double)( image.PixelWidth -1 );

            for ( int y = 0; y < image.PixelHeight; y++ )
            {
                for ( int x = 0; x < image.PixelWidth; x++ )
                {
                    int index = ( x + y * image.PixelHeight ) * 4;
                    byte r = pixels[ index + 1 ];
                    byte g = pixels[ index + 2 ];
                    byte b = pixels[ index + 3 ];
                    Point3D pos = new Point3D(
                            ( 2 * x / xSize ) - 1.0,
                            ( 2 * y / ySize ) - 1.0,
                            ( ( (int)r + (int)g + (int)b ) / 765.0 ) );
                    mesh.Positions.Add( pos );
                    mesh.TextureCoordinates.Add( new Point( x / xSize, y / ySize ) );
                }
            }
            AddGridTriangles( ref mesh, image.PixelWidth, image.PixelHeight );
            MeshOperations.GenerateNormals( mesh, false );

            // now do refraction
            Vector3D viewDirection = new Vector3D( 0, 0, -1 );
            MeshOperations.CalculateRefractedUV( mesh, viewDirection, indexOfRefraction );

            return mesh;
        }

        void AddGridTriangles( ref MeshGeometry3D mesh, int xSize, int zSize )
        {

            // Each grid cell needs 4 points, so we loop from 1 onward.
            // This way we are certain than z-1 and x-1 are valid indices.
            for ( int z = 1; z < zSize; z++ )
            {
                for ( int x = 1; x < xSize; x++ )
                {
                    // We need a reference to four adjacent points to make a grid cell.
                    // The cell consists of two triangles.
                    int indexCurrent = x + z * xSize;
                    int indexUp = x + ( z - 1 ) * xSize;
                    int indexLeft = ( x - 1 ) + z * xSize;
                    int indexUpLeft = ( x - 1 ) + ( z - 1 ) * xSize;

                    // I like to alternate the tesselation to form a /\/\ pattern, 
                    //    instead of a //// pattern or a \\\\ pattern for adjacent grid cells.
                    // Either one could be use instead.
                    if ( ( ( x + z ) % 2 ) == 0 )
                    {
                        mesh.TriangleIndices.Add( indexLeft );
                        mesh.TriangleIndices.Add( indexUp );
                        mesh.TriangleIndices.Add( indexCurrent );
                        mesh.TriangleIndices.Add( indexUpLeft );
                        mesh.TriangleIndices.Add( indexUp );
                        mesh.TriangleIndices.Add( indexLeft );
                    }
                    else
                    {
                        mesh.TriangleIndices.Add( indexUpLeft );
                        mesh.TriangleIndices.Add( indexUp );
                        mesh.TriangleIndices.Add( indexCurrent );
                        mesh.TriangleIndices.Add( indexLeft );
                        mesh.TriangleIndices.Add( indexUpLeft );
                        mesh.TriangleIndices.Add( indexCurrent );
                    }
                }
            }
        }

        #endregion

        #region fields
        Light _light;
        BitmapSource _wholeBackground;
        BitmapSource _currentBackground;
        ImageBrush _bgPicture;
        MeshGeometry3D _windowFrame = null;
        System.Windows.Point _startPosition;
        Int32Rect _windowBounds;
        #endregion

    }

}

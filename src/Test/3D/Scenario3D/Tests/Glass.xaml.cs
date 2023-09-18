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
using Microsoft.Test.Graphics.Factories;



// Namespace must be the same as what you set in project file
namespace Microsoft.Test.Graphics
{
    public partial class Glass : ScenarioUtility.IHelp
    {
        string ScenarioUtility.IHelp.Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /PercentCenter         % of the surface used as a center vs border (0.0 ... 1.0) ." +
                    "\n   /BorderHeight          height of the border surface." +
                    "\n   /Partitions            # of partitions for the 3D border." +
                    "\n   /ExposeChangeableLeak  set this to expose a changeable bug." +
                    "";
            }
        }

        public void OnLoaded( object sender, EventArgs args )
        {
            Window w = Application.Current.Windows[ 0 ];
            _windowBounds = new Int32Rect( (int)w.Left, (int)w.Top, (int)w.Width, (int)w.Height );
            w.ContentRendered += new EventHandler( MainWindow_ContentRendered );
            w.MouseLeftButtonDown += new MouseButtonEventHandler( MainWindow_MouseLeftButtonDown );
            w.PreviewMouseMove += new MouseEventHandler( MainWindow_MouseMove );
            w.WindowStyle = WindowStyle.None;
            w.ResizeMode = ResizeMode.NoResize;
            w.WindowState = WindowState.Minimized;

            Application application = Application.Current as Application;
            if ( application.Properties[ "ExposeChangeableLeak" ] != null )
            {
                _exposeChangeableLeak = bool.Parse( application.Properties[ "ExposeChangeableLeak" ] as string );
            }
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

        void MainWindow_MouseLeftButtonUp( object sender, MouseButtonEventArgs e )
        {
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
                if ( _exposeChangeableLeak )
                {
                    BuildScene();
                }
            }
        }

        public void BuildScene()
        {
            double percentCenter = 0.85;
            double borderHeight = 1;
            int partitions = 10;

            Application application = Application.Current as Application;
            if ( application.Properties[ "PercentCenter" ] != null )
            {
                percentCenter = double.Parse( application.Properties[ "PercentCenter" ] as string );
            }
            if ( application.Properties[ "BorderHeight" ] != null )
            {
                borderHeight = double.Parse( application.Properties[ "BorderHeight" ] as string );
            }
            if ( application.Properties[ "Partitions" ] != null )
            {
                partitions = int.Parse( application.Properties[ "Partitions" ] as string );
            }

            Model3DGroup mg = new Model3DGroup();

            mg.Children.Add( new DirectionalLight( Colors.White, new Vector3D( 0, 0, -1 ) ) );
            mg.Children.Add( new DirectionalLight( Colors.White, new Vector3D( 0, 0, 1 ) ) );

            Material mat = new DiffuseMaterial( _bgPicture );

            MeshGeometry3D flatMesh = MeshFactory.FullScreenMesh;
            GeometryModel3D primitive1 = new GeometryModel3D( flatMesh, mat );
            primitive1.Transform = new TranslateTransform3D( new Vector3D( 0, 0, 0 ) );
            mg.Children.Add( primitive1 );

            if ( _windowFrame == null )
            {
                Rect frame = new Rect(
                        new Point( -1 * percentCenter, -1 * percentCenter ),
                        new Point( 1 * percentCenter, 1 * percentCenter ) );
                _windowFrame = MeshFactory.CreateGlassFrame( frame, 1 - percentCenter, 1 - percentCenter * borderHeight, partitions );
                MeshOperations.CalculateRefractedUV( _windowFrame, new Vector3D( 0, 0, -1 ), 0.7 );
            }
            GeometryModel3D primitive4 = new GeometryModel3D( _windowFrame, mat );
            primitive4.Transform = new TranslateTransform3D( new Vector3D( 0, 0, 0 ) );
            mg.Children.Add( primitive4 );

            if ( _exposeChangeableLeak )
            {
                mg = mg.Clone();
            }

            ProjectionCamera camera = CameraFactory.OrthographicDefault;
            camera.Position = new Point3D( 0, 0, 5 );

            ModelVisual3D visual = new ModelVisual3D();
            visual.Content = mg;
            VIEWPORT.Children.Clear();
            VIEWPORT.Children.Add( visual );
            VIEWPORT.Camera = camera;
        }

        BitmapSource _wholeBackground;
        BitmapSource _currentBackground;
        ImageBrush _bgPicture;
        MeshGeometry3D _windowFrame = null;
        System.Windows.Point _startPosition;
        bool _exposeChangeableLeak = false;
        Int32Rect _windowBounds;
    }

}

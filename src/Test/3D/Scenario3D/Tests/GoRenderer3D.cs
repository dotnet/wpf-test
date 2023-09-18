// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Reflection;
using System.Security.Permissions;
using System.Threading;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public class GoRenderer3D
    {
        public GoRenderer3D( GoEngine engine )
            : this( engine, false )
        {
        }

        public GoRenderer3D( GoEngine engine, bool useCube )
        {
            this._engine = engine;

            _light = new DirectionalLight();
            _light.Direction = new Vector3D( 0, -1, .5 );

            MeshGeometry3D meshStone = MeshFactory.Sphere( 6, 12, .45 );

            MeshGeometry3D meshTable = MeshFactory.CreateFlatGrid( engine.XSize+1, engine.YSize+1, 0 );
            if ( useCube )
            {
                meshTable = MeshFactory.SimpleCubeMesh;
                meshTable.TextureCoordinates[ 4 ] = new Point( 0, 1 );
            }

            DiffuseMaterial white = new DiffuseMaterial( Brushes.AntiqueWhite );
            DiffuseMaterial black = new DiffuseMaterial( new SolidColorBrush( Color.FromRgb( 0x30, 0x30, 0x30 ) ) );
            DiffuseMaterial wood = new DiffuseMaterial( BrushDrawing );

            ScaleTransform3D crush = new ScaleTransform3D( new Vector3D( 1, .3, 1 ) );
            _whiteStone = new GeometryModel3D( meshStone, white );
            _whiteStone.Transform = crush;
            _blackStone = new GeometryModel3D( meshStone, black );
            _blackStone.Transform = crush;

            _table = new GeometryModel3D( meshTable, wood );
            _table.Transform = new TranslateTransform3D( -( engine.XSize ) * 0.5, 0, -( engine.YSize ) * 0.5 );
            if ( useCube )
            {
                _table.Transform = new ScaleTransform3D( ( engine.XSize ) * 0.5, 1, ( engine.YSize ) * 0.5 );
            }

            _pieces = new Model3DGroup();
            _pieces.Transform = new TranslateTransform3D( -( engine.XSize-1 ) * 0.5, 0, -( engine.YSize-1 ) * 0.5 );
            if ( useCube )
            {
                _pieces.Transform = new TranslateTransform3D( -( engine.XSize-1 ) * 0.5, 1, -( engine.YSize-1 ) * 0.5 );
            }
        }

        public Model3DGroup Render()
        {
            _board = new Model3DGroup();
            _board.Children.Add( _light );
            _board.Children.Add( _table );

            _pieces.Children.Clear();
            for ( int x=0; x < _engine.XSize; x++ )
            {
                for ( int y=0; y < _engine.YSize; y++ )
                {
                    if ( _engine.Board[ x, y ] != GoEngine.BoardContent.Empty )
                    {
                        Model3DGroup newPiece = new Model3DGroup();
                        newPiece.Transform = new TranslateTransform3D( new Vector3D( x, 0, y ) );
                        if ( _engine.Board[ x, y ] == GoEngine.BoardContent.BlackPiece )
                        {
                            newPiece.Children.Add( _blackStone );
                        }
                        else
                        {
                            newPiece.Children.Add( _whiteStone );
                        }
                        _pieces.Children.Add( newPiece );
                    }
                }
            }

            _board.Children.Add( _pieces );

            return _board;
        }

        public Brush BrushDrawing
        {
            get
            {
                DrawingGroup dg = new DrawingGroup();

                // draw background
                BitmapImage id = new BitmapImage( new Uri( "pack://siteoforigin:,,,/wood.bmp", UriKind.RelativeOrAbsolute ) );
                ImageBrush ib = new ImageBrush( id );
                double scale = 1.0/(double)Math.Max( _engine.XSize, _engine.YSize );
                dg.Children.Add( new GeometryDrawing(
                        ib, null,
                        new RectangleGeometry( new Rect( 0, 0, 1, 1 ) )
                        ) );
                // draw lines
                Pen blackPen = new Pen( Brushes.Black, .005 );
                for ( int x=0; x < _engine.XSize; x++ )
                {
                    dg.Children.Add( new GeometryDrawing(
                            null, blackPen,
                            new LineGeometry(
                                    new Point( ( .5 + x ) * scale, .5 * scale ),
                                    new Point( ( .5 + x )* scale, ( _engine.YSize -.5 )* scale ) )
                            ) );

                }

                for ( int y=0; y < _engine.YSize; y++ )
                {
                    dg.Children.Add( new GeometryDrawing(
                            null, blackPen,
                            new LineGeometry(
                                    new Point( .5 * scale, ( .5 + y )* scale ),
                                    new Point( ( _engine.XSize -.5 )* scale, ( .5 + y )* scale ) )
                            ) );
                }

                // draw center points
                if ( _engine.XSize == 19 && _engine.YSize == 19 )
                {
                    // classic GO Board
                    for ( int x=3; x < _engine.XSize; x+=6 )
                    {
                        for ( int y=3; y < _engine.YSize; y+=6 )
                        {
                            dg.Children.Add( new GeometryDrawing(
                                    Brushes.Black, null,
                                    new EllipseGeometry( new Point( ( .5 + x ) * scale, ( .5 + y )* scale ), .01, .01 )
                                    ) );
                        }
                    }
                }

                DrawingBrush db = new DrawingBrush( dg );
                db.ViewportUnits = BrushMappingMode.RelativeToBoundingBox;
                db.Stretch = Stretch.Fill;
                db.TileMode = TileMode.Tile;
                return db;
            }
        }

        Model3DGroup _board;
        Model3DGroup _pieces;
        DirectionalLight _light;
        GeometryModel3D _table;
        GeometryModel3D _whiteStone;
        GeometryModel3D _blackStone;
        GoEngine _engine;
    }
}
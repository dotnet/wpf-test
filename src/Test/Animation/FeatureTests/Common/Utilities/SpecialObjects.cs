// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

namespace Microsoft.Test.Animation
{
    using System;
    using System.Windows; 
    using System.Windows.Controls; 
    using System.Windows.Data; 
    using System.Windows.Media;
    using System.Windows.Media.Media3D;
    using System.Windows.Media.Animation;
    using System.Windows.Navigation;

    using Microsoft.Test.Logging;


    /******************************************************************************
    *******************************************************************************
    * CLASS:          SpecialObjects
    *******************************************************************************
    ******************************************************************************/
    /// <summary>
    /// Contains utility functions for creating objects to be animated.
    /// </summary>
    public static class SpecialObjects
    {
          
        /******************************************************************************
        * Function:          CreatePathFigureCollection
        ******************************************************************************/
        /// <summary>
        /// CreatePathFigureCollection: Create a PathFigure as part of a Collection.
        /// </summary>
        /// <returns>A PathFigureCollection</returns>
        public static PathFigureCollection CreatePathFigureCollection()
        {
            //Create a set of Segments: animation.
            LineSegment LS1 = new LineSegment();
            LS1.Point = new Point (40, 40);

            ArcSegment AS1 = new ArcSegment ();     
            AS1.Point = new Point (30, 60);
            AS1.Size = new Size (20, 20);

            LineSegment LS2 = new LineSegment();
            LS2.Point = new Point (20, 40);

            ArcSegment AS2 = new ArcSegment ();     
            AS2.Point = new Point (0, 30);
            AS2.Size = new Size (20, 20);

            LineSegment LS3 = new LineSegment();
            LS3.Point = new Point (20, 20);

            ArcSegment AS3 = new ArcSegment ();     
            AS3.Point = new Point (30, 0);
            AS3.Size = new Size (20, 20);

            LineSegment LS4 = new LineSegment();
            LS4.Point = new Point (40, 20);

            ArcSegment AS4 = new ArcSegment ();     
            AS4.Point = new Point (60, 30);
            AS4.Size = new Size (20, 20);

            //Add the above Segments to the PathFigure.
            PathFigure pathFigure = new PathFigure();
            pathFigure.StartPoint = new Point(60, 30);
            pathFigure.Segments.Add(LS1);
            pathFigure.Segments.Add(AS1);
            pathFigure.Segments.Add(LS2);
            pathFigure.Segments.Add(AS2);
            pathFigure.Segments.Add(LS3);
            pathFigure.Segments.Add(AS3);
            pathFigure.Segments.Add(LS4);
            pathFigure.Segments.Add(AS4);
            pathFigure.IsClosed = true;

            //Add the PathFigure to the PathFigureCollection.
            PathFigureCollection pathFigureCollection = new PathFigureCollection();
            pathFigureCollection.Add(pathFigure);

            return pathFigureCollection;
        }


        /******************************************************************************
        * Function:          Sphere
        ******************************************************************************/
        /// <summary>
        /// Create a spherical MeshGeometry3D.
        /// </summary>
        /// <returns>A new BeginStoryboard object</returns>
        public static MeshGeometry3D    Sphere( int latitude, int longitude, double radius )
        {
            MeshGeometry3D mesh = new MeshGeometry3D();

            double latTheta      = 0.0;
            double latDeltaTheta = Math.PI / latitude;
            double lonTheta      = 0.0;
            double lonDeltaTheta = 2.0 * Math.PI / longitude;

            Point3D origin = new Point3D( 0, 0, 0 );

            // Order of vertex creation:
            //  - For each latitude strip (y := [+radius,-radius] by -increment)
            //      - start at (-x,y,0)
            //      - For each longitude line (CCW about +y ... meaning +y points out of the paper)
            //          - generate vertex for latitude-longitude intersection

            // So if you have a 2x1 texture applied to this sphere:
            //      +---+---+
            //      | A | B |
            //      +---+---+
            // A camera pointing down -z with up = +y will see the "A" half of the texture.
            // "A" is considered to be the front of the sphere.

            for ( int lat = 0; lat <= latitude; lat++ )
            {
                double v = (double)lat / (double)latitude;
                double y = radius * Math.Cos( latTheta );
                double r = radius * Math.Sin( latTheta );

                if ( lat == latitude - 1 )
                {
                    latTheta = Math.PI;     // Close the gap in case of precision error
                }
                else
                {
                    latTheta += latDeltaTheta;
                }

                lonTheta = Math.PI;

                for ( int lon = 0; lon <= longitude; lon++ )
                {
                    double u  = (double)lon / (double)longitude;
                    double x = r * Math.Cos( lonTheta );
                    double z = r * Math.Sin( lonTheta );
                    if ( lon == longitude - 1 )
                    {
                        lonTheta = Math.PI;     // Close the gap in case of precision error
                    }
                    else
                    {
                        lonTheta -= lonDeltaTheta;
                    }

                    Point3D p = new Point3D( x,y,z );
                    Vector3D norm = p - origin;

                    mesh.Positions.Add( p );
                    mesh.Normals.Add( norm );
                    mesh.TextureCoordinates.Add( new Point( u,v ) );

                    if ( lat != 0 && lon != 0 )
                    {
                        // The loop just created the bottom right vertex (lat * (longitude + 1) + lon)
                        //  (the +1 comes because of the extra vertex on the seam)
                        // We only create panels when we're at the bottom-right vertex
                        //  (bottom-left, top-right, top-left have all been created by now)
                        //
                        //          +-----------+ x - (longitude + 1)
                        //          |           |
                        //          |           |
                        //      x-1 +-----------+ x

                        int bottomRight = lat * (longitude + 1) + lon;
                        int bottomLeft = bottomRight - 1;
                        int topRight = bottomRight - (longitude + 1);
                        int topLeft = topRight - 1;

                        // Wind counter-clockwise
                        mesh.TriangleIndices.Add( bottomLeft );
                        mesh.TriangleIndices.Add( topRight );
                        mesh.TriangleIndices.Add( topLeft );

                        mesh.TriangleIndices.Add( bottomRight );
                        mesh.TriangleIndices.Add( topRight );
                        mesh.TriangleIndices.Add( bottomLeft );
                    }
                }
            }

            return mesh;
        }
    }
}

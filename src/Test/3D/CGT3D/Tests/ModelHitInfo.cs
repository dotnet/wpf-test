// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    ///  Storage for hit test results
    /// </summary>
    public abstract class RayModelHitInfo
    {
        /// <summary/>
        public RayModelHitInfo()
        {
            alreadyVerified = false;
            screenPointHit = s_invalidScreenPointHit;
            distanceToRayOrigin = double.NaN;
            visualPointHit = Const.pNaN;
            modelHit = string.Empty;
            visualHit = string.Empty;
            baseVisualHit = string.Empty;
        }

        /// <summary/>
        public RayModelHitInfo(RayHitTestResult result)
        {
            alreadyVerified = false;
            screenPointHit = s_invalidScreenPointHit;
            distanceToRayOrigin = result.DistanceToRayOrigin;
            visualPointHit = result.PointHit;
            modelHit = ObjectUtils.GetName(result.ModelHit);
            visualHit = ObjectUtils.GetName(result.VisualHit);
            baseVisualHit = ObjectUtils.GetName(((HitTestResult)result).VisualHit);
        }

        /// <summary/>
        public static RayModelHitInfo Create(RayHitTestResult result)
        {
            if (result is RayMeshGeometry3DHitTestResult)
            {
                return new RayMeshHitInfo((RayMeshGeometry3DHitTestResult)result);
            }
#if SSL
            else if ( result is RayScreenSpaceLines3DHitTestResult )
            {
                return new RayLinesHitInfo( (RayScreenSpaceLines3DHitTestResult)result );
            }
#endif
            throw new NotSupportedException("Hit Info storage does not exist for: " + result.GetType());
        }

        /// <summary/>
        public static int Compare(RayModelHitInfo info1, RayModelHitInfo info2)
        {
            double d1 = info1.distanceToRayOrigin;
            double d2 = info2.distanceToRayOrigin;

            if (d1 < d2 && MathEx.NotCloseEnough(d1, d2))
            {
                return -1;
            }
            if (d1 > d2 && MathEx.NotCloseEnough(d1, d2))
            {
                return 1;
            }
            return 0;
        }

        /// <summary/>
        public bool HasValidScreenPointHit
        {
            get
            {
                return !(double.IsNaN(screenPointHit.X) || double.IsNaN(screenPointHit.Y));
            }
        }

        internal bool alreadyVerified;
        internal double distanceToRayOrigin;
        internal Point3D visualPointHit;
        internal string modelHit;
        internal string visualHit;
        internal string baseVisualHit;

        // For log output
        internal Point screenPointHit;
        private static Point s_invalidScreenPointHit = new Point(double.NaN, double.NaN);
    }

    /// <summary/>
    public sealed class RayMeshHitInfo : RayModelHitInfo
    {
        /// <summary/>
        public RayMeshHitInfo()
        {
            meshHit = string.Empty;
            modelPointHit = Const.pNaN;
            vertexIndex1 = -1;
            vertexIndex2 = -1;
            vertexIndex3 = -1;
            vertexWeight1 = -1;
            vertexWeight2 = -1;
            vertexWeight3 = -1;
        }

        /// <summary/>
        public RayMeshHitInfo(RayMeshGeometry3DHitTestResult result)
            : base(result)
        {
            meshHit = ObjectUtils.GetName(result.MeshHit);
            modelPointHit = ComputeModelPointHit(result);
            vertexIndex1 = result.VertexIndex1;
            vertexIndex2 = result.VertexIndex2;
            vertexIndex3 = result.VertexIndex3;
            vertexWeight1 = result.VertexWeight1;
            vertexWeight2 = result.VertexWeight2;
            vertexWeight3 = result.VertexWeight3;
        }

        private static Point3D ComputeModelPointHit(RayMeshGeometry3DHitTestResult result)
        {
            Point3D p1 = result.MeshHit.Positions[result.VertexIndex1];
            Point3D p2 = result.MeshHit.Positions[result.VertexIndex2];
            Point3D p3 = result.MeshHit.Positions[result.VertexIndex3];
            return MathEx.WeightedSum(p1, result.VertexWeight1, p2, result.VertexWeight2, p3, result.VertexWeight3);
        }

        internal string meshHit;
        internal Point3D modelPointHit;
        internal int vertexIndex1;
        internal int vertexIndex2;
        internal int vertexIndex3;
        internal double vertexWeight1;
        internal double vertexWeight2;
        internal double vertexWeight3;
    }


#if SSL
    public sealed class         RayLinesHitInfo             : RayModelHitInfo
    {


        public                  RayLinesHitInfo()
        {
            segmentIndex    = -1;
            segmentLocation = -1;
        }



        public                  RayLinesHitInfo( RayScreenSpaceLines3DHitTestResult result )
                                    : base( result )
        {
            segmentIndex    = result.LineSegmentIndex;
            segmentLocation = result.LineSegmentLocation;
        }



        internal int            segmentIndex;
        internal double         segmentLocation;
    }
#endif
}
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;

namespace Microsoft.Test.Graphics
{
    public class MeshAnimation : DiscreteAnimation
    {
        #region Constructors

        /// <Summary>
        /// Constructor requires an object array.
        /// </Summary>
        public MeshAnimation()
            : base()
        {
        }

        /// <Summary>
        /// Constructor requires an object array.
        /// </Summary>
        public MeshAnimation(MeshGeometry3D from, MeshGeometry3D to, int frames)
            : base()
        {
            // make sure these are the same size
            System.Diagnostics.Debug.Assert(from.Positions.Count == to.Positions.Count);

            Values = new object[frames];
            double delta = 1.0 / (frames - 1);

            for (int i = 0; i < frames; i++)
            {
                MeshGeometry3D current = new MeshGeometry3D();
                double toWeight = i * delta;
                double fromWeight = 1.0 - toWeight;

                // mesh positions
                for (int p = 0; p < from.Positions.Count; p++)
                {
                    current.Positions.Add(new Point3D(
                            toWeight * to.Positions[p].X + fromWeight * from.Positions[p].X,
                            toWeight * to.Positions[p].Y + fromWeight * from.Positions[p].Y,
                            toWeight * to.Positions[p].Z + fromWeight * from.Positions[p].Z));
                }
                // mesh normals
                for (int n = 0; n < from.Normals.Count; n++)
                {
                    current.Normals.Add(toWeight * to.Normals[n] + fromWeight * from.Normals[n]);
                }
                // mesh uvs
                for (int t = 0; t < from.TextureCoordinates.Count; t++)
                {
                    current.TextureCoordinates.Add(new Point(
                            toWeight * to.TextureCoordinates[t].X + fromWeight * from.TextureCoordinates[t].X,
                            toWeight * to.TextureCoordinates[t].Y + fromWeight * from.TextureCoordinates[t].Y));
                }
                // triangle indices
                for (int r = 0; r < from.TriangleIndices.Count; r++)
                {
                    // they should both be the same ...
                    current.TriangleIndices.Add(from.TriangleIndices[r]);
                }

                Values[i] = current;
            }
        }
        #endregion

        #region Freezable

        public new MeshAnimation Clone()
        {
            return (MeshAnimation)base.Clone();
        }

        protected override System.Windows.Freezable CreateInstanceCore()
        {
            return new MeshAnimation();
        }

        public new MeshAnimation GetAsFrozen()
        {
            return (MeshAnimation)base.GetAsFrozen();
        }

        #endregion

    }
}






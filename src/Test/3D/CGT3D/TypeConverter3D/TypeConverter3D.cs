// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Controls;
using System.Windows.Media.Media3D;

namespace Microsoft.Test.Graphics
{
    /// <summary>
    /// Contains every parseable 3D type available
    /// </summary>
    public class Type3D
    {
        /// <summary/>
        public Matrix3D Matrix3D { get { return _matrix3D; } set { _matrix3D = value; } }
        /// <summary/>
        public Point3D Point3D { get { return _point3D; } set { _point3D = value; } }
        /// <summary/>
        public Point4D Point4D { get { return _point4D; } set { _point4D = value; } }
        /// <summary/>
        public Quaternion Quaternion { get { return _quaternion; } set { _quaternion = value; } }
        /// <summary/>
        public Rect3D Rect3D { get { return _rect3D; } set { _rect3D = value; } }
        /// <summary/>
        public Size3D Size3D { get { return _size3D; } set { _size3D = value; } }
        /// <summary/>
        public Vector3D Vector3D { get { return _vector3D; } set { _vector3D = value; } }

        /// <summary/>
        public Point3DCollection Point3Ds { get { return _point3Ds; } set { _point3Ds = value; } }
        /// <summary/>
        public Vector3DCollection Vector3Ds { get { return _vector3Ds; } set { _vector3Ds = value; } }

        private Matrix3D _matrix3D;
        private Point3D _point3D;
        private Point4D _point4D;
        private Quaternion _quaternion;
        private Rect3D _rect3D;
        private Size3D _size3D;
        private Vector3D _vector3D;

        private Point3DCollection _point3Ds;
        private Vector3DCollection _vector3Ds;
    }

    /// <summary>
    /// A silly class that will allow our my special type to be set by the XAML parser.
    /// This way we can test the TypeConverters in a much simpler manner.
    /// </summary>
    public class TypeHolder : Canvas
    {
        /// <summary/>
        public Type3D Type3D { get { return _type; } set { _type = value; } }

        private Type3D _type;
    }
}
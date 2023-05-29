// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Media.Media3D;
using Microsoft.Test.Input.MultiTouch;

namespace Microsoft.Test.Input.MultiTouch.Tests
{
    /// <summary>
    /// Interaction logic for D3DTestWindow.xaml
    /// </summary>
    public partial class D3DTestWindow : Window
    {
        #region Constructor

        public D3DTestWindow()
        {
            InitializeComponent();

            BuildSolid();
        }

        #endregion 

        #region Helpers

        private void BuildSolid()
        {
            // Define 3D mesh object
            MeshGeometry3D mesh = new MeshGeometry3D();

            mesh.Positions.Add(new Point3D(-0.5, -0.5, 1));
            mesh.Positions.Add(new Point3D(0.5, -0.5, 1));
            mesh.Positions.Add(new Point3D(0.5, 0.5, 1));
            mesh.Positions.Add(new Point3D(-0.5, 0.5, 1));

            mesh.Positions.Add(new Point3D(-1, -1, -1));
            mesh.Positions.Add(new Point3D(1, -1, -1));
            mesh.Positions.Add(new Point3D(1, 1, -1));
            mesh.Positions.Add(new Point3D(-1, 1, -1));

            // Front face
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(0);

            // Back face
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(6);

            // Right face
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(2);

            // Top face
            mesh.TriangleIndices.Add(2);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(6);
            mesh.TriangleIndices.Add(7);

            // Bottom face
            mesh.TriangleIndices.Add(5);
            mesh.TriangleIndices.Add(1);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(5);

            // Right face
            mesh.TriangleIndices.Add(4);
            mesh.TriangleIndices.Add(0);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(3);
            mesh.TriangleIndices.Add(7);
            mesh.TriangleIndices.Add(4);

            // Geometry creation
            _mGeometry = new GeometryModel3D(mesh, new DiffuseMaterial(Brushes.YellowGreen));
            _mGeometry.Transform = new Transform3DGroup();
            group.Children.Add(_mGeometry);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, 5);
            _mGeometry.Transform = new Transform3DGroup();
        }

        private void Grid_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            camera.Position = new Point3D(camera.Position.X, camera.Position.Y, camera.Position.Z - e.Delta / 250D);
        }

        private void Grid_MouseMove(object sender, MouseEventArgs e)
        {
            if (_mDown)
            {
                Point pos = Mouse.GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - _mLastPos.X, dy = actualPos.Y - _mLastPos.Y;

                double mouseAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    mouseAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) mouseAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) mouseAngle += Math.PI;
                    else if (dx > 0 && dy < 0) mouseAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) mouseAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0) mouseAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = mouseAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group = _mGeometry.Transform as Transform3DGroup;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));

                _mLastPos = actualPos;
            }
        }

        private void Grid_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            _mDown = true;
            Point pos = Mouse.GetPosition(viewport);
            _mLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
        }

        private void Grid_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _mDown = false;
        }

        private void OnTouchDown(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchDown");

            _tDown = true;
            Point pos = e.GetTouchPoint(viewport).Position;
            _tLastPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
        }

        private void OnTouchMove(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchMove");

            if (_tDown)
            {
                Point pos = e.GetTouchPoint(viewport).Position; // GetPosition(viewport);
                Point actualPos = new Point(pos.X - viewport.ActualWidth / 2, viewport.ActualHeight / 2 - pos.Y);
                double dx = actualPos.X - _tLastPos.X, dy = actualPos.Y - _tLastPos.Y;

                double touchAngle = 0;
                if (dx != 0 && dy != 0)
                {
                    touchAngle = Math.Asin(Math.Abs(dy) / Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2)));
                    if (dx < 0 && dy > 0) touchAngle += Math.PI / 2;
                    else if (dx < 0 && dy < 0) touchAngle += Math.PI;
                    else if (dx > 0 && dy < 0) touchAngle += Math.PI * 1.5;
                }
                else if (dx == 0 && dy != 0) touchAngle = Math.Sign(dy) > 0 ? Math.PI / 2 : Math.PI * 1.5;
                else if (dx != 0 && dy == 0) touchAngle = Math.Sign(dx) > 0 ? 0 : Math.PI;

                double axisAngle = touchAngle + Math.PI / 2;

                Vector3D axis = new Vector3D(Math.Cos(axisAngle) * 4, Math.Sin(axisAngle) * 4, 0);

                double rotation = 0.01 * Math.Sqrt(Math.Pow(dx, 2) + Math.Pow(dy, 2));

                Transform3DGroup group = _mGeometry.Transform as Transform3DGroup;
                QuaternionRotation3D r = new QuaternionRotation3D(new Quaternion(axis, rotation * 180 / Math.PI));
                group.Children.Add(new RotateTransform3D(r));

                _tLastPos = actualPos;
            }
        }

        private void OnTouchUp(object sender, TouchEventArgs e)
        {
            AddWatchEvent("TouchUp");
            _tDown = false;
        }

        private void OnStylusSystemGesture(object sender, StylusSystemGestureEventArgs e)
        {
            AddWatchEvent(String.Format("Gesture ({0}) - {1}", e.StylusDevice.TabletDevice.Type, e.SystemGesture));
        }

        private void OnStylusDown(object sender, StylusDownEventArgs e)
        {
            AddWatchEvent(String.Format("StylusDown ({0}) ID={1}", e.StylusDevice.TabletDevice.Type, e.StylusDevice.Id));
        }

        private void OnStylusUp(object sender, StylusEventArgs e)
        {
            AddWatchEvent(String.Format("StylusUp ({0}) ID={1}", e.StylusDevice.TabletDevice.Type, e.StylusDevice.Id));
        }

        private void AddWatchEvent(string s)
        {
            if (WatchList.Items.Count >= MaxWatchEvents)
            {
                WatchList.Items.RemoveAt(0);
            }

            WatchList.Items.Add(new MakeStringUnique() { TheString = s });
        }

        private class MakeStringUnique
        {
            public string TheString 
            {
                get;
                set;
            }

            public override string ToString()
            {
                return TheString;
            }
        }

        #endregion

        #region Fields

        private GeometryModel3D _mGeometry;
        private bool _mDown;
        private Point _mLastPos;
        private bool _tDown;
        private Point _tLastPos;
        private const int MaxWatchEvents = 30;

        #endregion
    }
}

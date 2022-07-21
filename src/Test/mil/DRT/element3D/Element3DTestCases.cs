// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: Test cases for Element3D
//
//
//

using System;
using System.Diagnostics;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Data;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Markup;
using System.Windows.Media.Animation;
using System.Globalization;

using Microsoft.Samples;

namespace Tests
{
    public abstract class Element3DTestCase
    {
        // build some geometries
        static Element3DTestCase()
        {
            Point3DCollection positions = new Point3DCollection();
            positions.Add(new Point3D(-0.5,0.5,0));
            positions.Add(new Point3D(-0.5,-0.5,0));
            positions.Add(new Point3D(0.5,-0.5,0));
            positions.Add(new Point3D(0.5,0.5,0));
            positions.Freeze();

            PointCollection textureCoordinates = new PointCollection();
            textureCoordinates.Add(new Point(0,0));
            textureCoordinates.Add(new Point(0,1));
            textureCoordinates.Add(new Point(1,1));
            textureCoordinates.Add(new Point(1,0));
            
            Int32Collection triangleIndices = new Int32Collection();
            triangleIndices.Add(0);
            triangleIndices.Add(1);
            triangleIndices.Add(2);
            triangleIndices.Add(0);
            triangleIndices.Add(2);
            triangleIndices.Add(3);
            triangleIndices.Freeze();

            _planeMesh = new MeshGeometry3D();
            _planeMesh.Positions = positions;
            _planeMesh.TextureCoordinates = textureCoordinates;
            _planeMesh.TriangleIndices = triangleIndices;
        }
        
        public override abstract string ToString();

        public void BuildScene(Viewport3D visual)
        {
            visual.Children.Clear();

            BuildSceneCore(visual);
        }

        protected abstract void BuildSceneCore(Viewport3D visual);

        public virtual void DoBefore()
        {
        }

        public virtual void DoAfter()

        {
        }

        public bool Success
        {
            get 
            {
                return _success;
            }
        }

        protected static bool AreClose(Point3D p1, Point3D p2)
        {
            return (Math.Abs(p1.X - p2.X) < 0.00001 &&
                    Math.Abs(p1.Y - p2.Y) < 0.00001 &&
                    Math.Abs(p1.Z - p2.Z) < 0.00001);
        }

        protected static bool AreClose(Point p1, Point p2)
        {
            return (Math.Abs(p1.X - p2.X) < 0.00001 &&
                    Math.Abs(p1.Y - p2.Y) < 0.00001);
        }

        public static MeshGeometry3D _planeMesh = null;  
        protected bool _success = true;
        
    }

    public class Element3DTestSuite
    {
        public Element3DTestSuite()
        {
            _list = new List<Element3DTestCase>();
            _list.Add(new TestDefaults());
            _list.Add(new TestForceInheritance());
            _list.Add(new TestReverseInheritance());
            _list.Add(new TestEvents());
            _list.Add(new TestTransform3Ds());
            _list.Add(new TestOtherTransforms());
        }

        public Element3DTestCase this [ int index ]
        {
            get
            {
                return (Element3DTestCase) _list[index];
            }
        }

        public int Count
        {
            get
            {
                return _list.Count;
            }
        }

        List<Element3DTestCase> _list;
    }

    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestDefaults : Element3DTestCase
    {
        public override string ToString() { return "Defaults"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            ModelVisual3D mv3D = new ModelVisual3D();
            mv3D.Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            viewport.Children.Add(mv3D);

            // put some objects in the scene
            SphereUIElement3D sphereUIElement = new SphereUIElement3D();
            ModelUIElement3D sphereModelUIElement = new ModelUIElement3D();
            DiffuseMaterial diffMat = new DiffuseMaterial();
            diffMat.Brush = Brushes.White;
            sphereModelUIElement.Model = new GeometryModel3D(Sphere.Tessellate(10, 10, 0.5), diffMat);

            sphereUIElement.Transform = new TranslateTransform3D(new Vector3D(-2, 0, 0));            
            ContainerUIElement3D container = new ContainerUIElement3D();
            container.Transform = new TranslateTransform3D(new Vector3D(1, 0, 0));
            container.Children.Add(sphereUIElement);
            viewport.Children.Add(container);

            sphereModelUIElement.Transform = new TranslateTransform3D(new Vector3D(1, 0, -2));            
            viewport.Children.Add(sphereModelUIElement);            

            Viewport2DVisual3D viewport2DVisual3D = new Viewport2DVisual3D();
            viewport2DVisual3D.Transform = new TranslateTransform3D(new Vector3D(0, 0, 2));
            Button b = new Button();
            b.Content = "Hello";            
            viewport2DVisual3D.Visual = b;
            viewport2DVisual3D.Geometry = Element3DTestCase._planeMesh;            
            DiffuseMaterial m = new DiffuseMaterial();
            m.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            viewport2DVisual3D.Material = m;

            viewport.Children.Add(viewport2DVisual3D);
        }                
    }     
    
    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestForceInheritance : Element3DTestCase
    {
        public override string ToString() { return "Force Inheritance"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            ModelVisual3D mv3D = new ModelVisual3D();
            mv3D.Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            viewport.Children.Add(mv3D);

            // put some objects in the scene
            _sphereUIElement = new SphereUIElement3D();
            _container = new ContainerUIElement3D();

            _sphereUIElement.Transform = new TranslateTransform3D(new Vector3D(-2, 0, 0));
            _container.Transform = new TranslateTransform3D(new Vector3D(1, 0, 0));

            _container.Children.Add(_sphereUIElement);
            
            viewport.Children.Add(_container);
        }                

        public override void DoAfter()

        {
            _container.IsHitTestVisible = false;
            _sphereUIElement.IsHitTestVisible = true;

            // even though we set it to true, our parent should constrain us
            if (_sphereUIElement.IsHitTestVisible)
            {
                _success = false;
                return;
            }

            // now it should be true
            _container.IsHitTestVisible = true;
            if (!_sphereUIElement.IsHitTestVisible)
            {
                _success = false;
                return;
            }
        }  

        private SphereUIElement3D _sphereUIElement;
        private ContainerUIElement3D _container;
    }     

    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestReverseInheritance : Element3DTestCase
    {
        public override string ToString() { return "Reverse Inheritance"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            ModelVisual3D mv3D = new ModelVisual3D();
            mv3D.Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            viewport.Children.Add(mv3D);

            // put some objects in the scene
            _container = new ContainerUIElement3D();
            
            Viewport2DVisual3D viewport2DVisual3D = new Viewport2DVisual3D();
            viewport2DVisual3D.Transform = new TranslateTransform3D(new Vector3D(0, 0, 1));
            _button = new Button();
            _button.Content = "Hello";            
            viewport2DVisual3D.Visual = _button;
            viewport2DVisual3D.Geometry = Element3DTestCase._planeMesh;            
            DiffuseMaterial m = new DiffuseMaterial();
            m.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            viewport2DVisual3D.Material = m;

            _container.Children.Add(viewport2DVisual3D);
            viewport.Children.Add(_container);
        }                

        public override void DoBefore()
        {
            Rect buttonBounds = VisualTreeHelper.GetDescendantBounds(_button);
            Point p = new Point(buttonBounds.X + buttonBounds.Width / 2,
                                buttonBounds.Y + buttonBounds.Height / 2);

            Point screenPoint = _button.PointToScreen(p);

            InputHelper.MoveTo(screenPoint);
        }

        public override void DoAfter()

        {
            _success = _container.IsMouseOver;
        }  

        private ContainerUIElement3D _container;
        private Button _button;
    }     

    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestEvents : Element3DTestCase
    {
        public override string ToString() { return "Eventing"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            ModelVisual3D mv3D = new ModelVisual3D();
            mv3D.Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            viewport.Children.Add(mv3D);

            // put some objects in the scene
            _container = new ContainerUIElement3D();
            
            Viewport2DVisual3D viewport2DVisual3D = new Viewport2DVisual3D();
            viewport2DVisual3D.Transform = new TranslateTransform3D(new Vector3D(0, 0, 1));
            _button = new Button();
            _button.Content = "Hello";   
            _button.Click += ButtonClick;
            viewport2DVisual3D.Visual = _button;
            viewport2DVisual3D.Geometry = Element3DTestCase._planeMesh;            
            DiffuseMaterial m = new DiffuseMaterial();
            m.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            viewport2DVisual3D.Material = m;

            _container.Children.Add(viewport2DVisual3D);
            viewport.Children.Add(_container);
        }       

        protected void ButtonClick(object sender, RoutedEventArgs e)
        {
            _clicked = true;
        }

        public override void DoBefore()
        {
            Rect buttonBounds = VisualTreeHelper.GetDescendantBounds(_button);
            Point p = new Point(buttonBounds.X + buttonBounds.Width / 2,
                                buttonBounds.Y + buttonBounds.Height / 2);

            Point screenPoint = _button.PointToScreen(p);

            InputHelper.MoveToAndClick(screenPoint);
        }

        public override void DoAfter()

        {
            if (!_clicked)
            {
                _success = false;
            }
        }  

        private ContainerUIElement3D _container;
        private Button _button;
        private bool _clicked = false;
    }  

    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestTransform3Ds : Element3DTestCase
    {
        public override string ToString() { return "Test Transform3Ds"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            _rootMV3D = new ModelVisual3D();
            _subMV3D = new ModelVisual3D();
            _subSubMV3D = new ModelVisual3D();

            TranslateTransform3D tt3D = new TranslateTransform3D(500, 30, -40);
            RotateTransform3D rt3D = new RotateTransform3D(new AxisAngleRotation3D(new Vector3D(0.5, 0.5, 0.5), 30), 30, 20, 50);

            GeneralTransform3DGroup gt3Dgroup = new GeneralTransform3DGroup();
            gt3Dgroup.Children.Add(tt3D);
            gt3Dgroup.Children.Add(rt3D);
            _combinedTransform = gt3Dgroup;

            _subSubMV3D.Transform = tt3D;
            _subMV3D.Transform = rt3D;

            _subMV3D.Children.Add(_subSubMV3D);
            _rootMV3D.Children.Add(_subMV3D);            
            viewport.Children.Add(_rootMV3D);
        }                

        public override void DoAfter()

        {
            Point3D p3D1 = new Point3D(10, 10, 10);
            GeneralTransform3D t3D = _subSubMV3D.TransformToAncestor(_rootMV3D);
            Point3D p3D1xformed = t3D.Transform(p3D1);
            GeneralTransform3D t3D2 = _rootMV3D.TransformToDescendant(_subSubMV3D);
            Point3D p3D1original = t3D2.Transform(p3D1xformed);

            Point3D p3D1xformedExpected = _combinedTransform.Transform(p3D1);

            Point3D p3D2 = new Point3D(7, 3, 100);
            GeneralTransform3D t3Dp3D2 = _subMV3D.TransformToAncestor(_rootMV3D);
            Point3D p3D2xformed = t3Dp3D2.Transform(p3D2);
            GeneralTransform3D t3Dp3D22 = _rootMV3D.TransformToDescendant(_subMV3D);
            Point3D p3D2original = t3Dp3D22.Transform(p3D2xformed);

            if (!(AreClose(p3D1, p3D1original) && AreClose(p3D2, p3D2original) && AreClose(p3D1xformedExpected, p3D1xformed)))
            {
                _success = false;
            }
        }  

        private ModelVisual3D _rootMV3D;
        private ModelVisual3D _subMV3D;
        private ModelVisual3D _subSubMV3D;
        private GeneralTransform3D _combinedTransform;
    }     

    // test defaults just tests constructing the basic things and then
    // adding them to the viewport.  
    public class TestOtherTransforms : Element3DTestCase
    {
        public override string ToString() { return "TestOtherTransforms"; }

        protected override void BuildSceneCore(Viewport3D viewport)
        {
            _viewport = viewport;
            
            // lights and camera
            PerspectiveCamera camera = new PerspectiveCamera();
            camera.Position = new Point3D(0, 0, 6);
            viewport.Camera = camera;

            ModelVisual3D mv3D = new ModelVisual3D();
            mv3D.Content = new DirectionalLight(Colors.White, new Vector3D(0, 0, -1));
            viewport.Children.Add(mv3D);

            // put some objects in the scene
            _container = new ContainerUIElement3D();
            
            _viewport2DVisual3D = new Viewport2DVisual3D();
            _viewport2DVisual3D.Transform = new TranslateTransform3D(new Vector3D(0, 0, 1));
            _button = new Button();
            _button.Content = "Hello";   
            _viewport2DVisual3D.Visual = _button;
            _viewport2DVisual3D.Geometry = Element3DTestCase._planeMesh;            
            DiffuseMaterial m = new DiffuseMaterial();
            m.SetValue(Viewport2DVisual3D.IsVisualHostMaterialProperty, true);
            _viewport2DVisual3D.Material = m;

            _container.Children.Add(_viewport2DVisual3D);
            viewport.Children.Add(_container);
        }       
        
        public override void DoBefore()
        {
            Rect buttonBounds = VisualTreeHelper.GetDescendantBounds(_button);
            Point p = new Point(buttonBounds.X + buttonBounds.Width / 2,
                                buttonBounds.Y + buttonBounds.Height / 2);


            GeneralTransform2DTo3D gt2Dto3D = _button.TransformToAncestor(_viewport2DVisual3D);
            GeneralTransform gt = _button.TransformToAncestor(_viewport);
            GeneralTransform3DTo2D gt3Dto2D = _viewport2DVisual3D.TransformToAncestor(_viewport);

            Point finalPoint1 = gt.Transform(p);

            Point3D ptIn3D = gt2Dto3D.Transform(p);
            Point finalPoint2 = gt3Dto2D.Transform(ptIn3D);

            if (!AreClose(finalPoint1, finalPoint2))
            {
                _success = false;
            }
        }

        private Viewport2DVisual3D _viewport2DVisual3D;
        private ContainerUIElement3D _container;
        private Viewport3D _viewport;
        private Button _button;
    }  
}

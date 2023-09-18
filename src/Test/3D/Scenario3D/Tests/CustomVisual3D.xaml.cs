// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Windows;
using System.Threading;
using System.Windows.Threading;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Media.Animation;
using System.Windows.Navigation;
using System.Windows.Markup;
using Microsoft.Win32;
using Microsoft.Test.Graphics.Factories;

namespace Microsoft.Test.Graphics
{
    public partial class CustomVisual3DTest : ScenarioUtility.IHelp
    {
        private void OnInitialized(object sender, EventArgs args)
        {
            Application application = Application.Current as Application;

            if (application.Properties["WindowBackgroundColor"] != null)
            {
                Color background = StringConverter.ToColor((string)application.Properties["WindowBackgroundColor"]);
                this.Background = new SolidColorBrush(background);
            }
            if (application.Properties["DotDownViewport"] != null)
            {
                Storyboard s = (Storyboard)this.Resources["dotDownViewport"];

                // "Compose" means reset the values animated to their original state when the animation is done.
                // "true" means I want to be able to control this storyboard.
                s.Begin(this, HandoffBehavior.Compose, true);
            }
            if (application.Properties["DotDownVisual3D"] != null)
            {
                Storyboard s = (Storyboard)this.Resources["dotDownVisual3D"];
                s.Begin(this, HandoffBehavior.Compose, true);
            }
            if (application.Properties["ValueType"] != null)
            {
                Storyboard s = (Storyboard)this.Resources["valueType"];
                s.Begin(this, HandoffBehavior.Compose, true);
            }
            if (application.Properties["ReferenceType"] != null)
            {
                Storyboard s = (Storyboard)this.Resources["referenceType"];
                s.Begin(this, HandoffBehavior.Compose, true);
            }
            if (application.Properties["BeginAnimation"] != null)
            {
                TestBeginAnimation();
            }
            if (application.Properties["ApplyAnimationClock"] != null)
            {
                TestApplyAnimationClock();
            }
        }

        private void TestBeginAnimation()
        {
            Vector3DAnimation anim = new Vector3DAnimation();
            anim.To = new Vector3D(2, 2, 1);
            anim.From = new Vector3D(.5, .5, 1);
            anim.Duration = TimeSpan.FromSeconds(3);
            anim.AutoReverse = true;
            anim.RepeatBehavior = RepeatBehavior.Forever;

            plane.BeginAnimation(XYPlane.ScaleProperty, anim);
        }

        private void TestApplyAnimationClock()
        {
            Point3DAnimation anim = new Point3DAnimation();
            anim.To = new Point3D(-1, 0, 10);
            anim.From = new Point3D(2, 0, 6);
            anim.Duration = TimeSpan.FromSeconds(2);
            anim.AutoReverse = true;
            anim.RepeatBehavior = RepeatBehavior.Forever;

            light.ApplyAnimationClock(SceneLight.PositionProperty, anim.CreateClock());
        }

        public string Help
        {
            get
            {
                return
                    "\nFlags:" +
                    "\n   /DotDownViewport          Animate SceneLight.Position by dotting down from the Viewport3D" +
                    "\n   /DotDownVisual3D          Animate Brush.Color by dotting down from the Visual3D" +
                    "\n   /ValueType                Animate XYPlane.Scale by targeting XYPlane (no dotting down)" +
                    "\n   /ReferenceType            Animate XYPlane.Brush by targeting XYPlane (no dotting down)" +
                    "\n   /BeginAnimation           Animate XYPlane.Scale by calling BeginAnimation on Visual3D" +
                    "\n   /ApplyAnimationClock      Animate SceneLight.Position by calling ApplyAnimationClock on Visual3D" +
                    "";
            }
        }
    }

    public class XYPlane : ModelVisual3D
    {
        public XYPlane()
        {
            MeshGeometry3D mesh = MeshFactory.PlaneXY(new Point(-1, -1), new Point(1, 1), 0, 20, 20);
            _localMaterial = new DiffuseMaterial(Brush);

            Content = new GeometryModel3D(mesh, _localMaterial);

            _localTransform = new ScaleTransform3D(Scale);
            Content.Transform = _localTransform;
        }

        public Vector3D Scale
        {
            get { return (Vector3D)GetValue(ScaleProperty); }
            set { SetValue(ScaleProperty, value); }
        }

        public Brush Brush
        {
            get { return (Brush)GetValue(BrushProperty); }
            set { SetValue(BrushProperty, value); }
        }

        public static DependencyProperty ScaleProperty =
                DependencyProperty.Register(
                                        "Scale",
                                        typeof(Vector3D),
                                        typeof(XYPlane),
                                        new PropertyMetadata(new Vector3D(1, 1, 1), ScaleChanged));

        public static DependencyProperty BrushProperty =
                DependencyProperty.Register(
                                        "Brush",
                                        typeof(Brush),
                                        typeof(XYPlane),
                                        new PropertyMetadata(Brushes.White, BrushChanged));

        private static void ScaleChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                XYPlane dude = (XYPlane)o;
                Vector3D value = (Vector3D)args.NewValue;
                dude._localTransform.ScaleX = value.X;
                dude._localTransform.ScaleY = value.Y;
                dude._localTransform.ScaleZ = value.Z;
            }
        }

        private static void BrushChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                XYPlane dude = (XYPlane)o;
                dude._localMaterial.Brush = (Brush)args.NewValue;
            }
        }

        private ScaleTransform3D _localTransform;
        private DiffuseMaterial _localMaterial;
    }

    public class SceneLight : ModelVisual3D
    {

        public SceneLight()
        {
            _localLight = LightFactory.WhiteSpot;
            _localLight.Position = Position;
            _localLight.InnerConeAngle = 15;
            _localLight.OuterConeAngle = 30;
            Content = _localLight;
        }

        public Point3D Position
        {
            get { return (Point3D)GetValue(PositionProperty); }
            set { SetValue(PositionProperty, value); }
        }

        public static DependencyProperty PositionProperty =
                DependencyProperty.Register(
                                        "Position",
                                        typeof(Point3D),
                                        typeof(SceneLight),
                                        new PropertyMetadata(new Point3D(0, 0, 5), PositionChanged));

        private static void PositionChanged(DependencyObject o, DependencyPropertyChangedEventArgs args)
        {
            if (args.NewValue != args.OldValue)
            {
                SceneLight dude = (SceneLight)o;
                dude._localLight.Position = (Point3D)args.NewValue;
            }
        }

        private SpotLight _localLight;
    }

    public class BrushAnimation : ObjectAnimationBase
    {

        public BrushAnimation()
        {
            _brushes = new List<Brush>();
        }

        protected override void GetCurrentValueAsFrozenCore(Freezable source)
        {
            BrushAnimation a = (BrushAnimation)source;
            foreach (Brush brush in a._brushes)
            {
                Brush copy = brush.CloneCurrentValue();
                copy.Freeze();
                _brushes.Add(copy);
            }
            base.GetCurrentValueAsFrozenCore(source);
        }

        public List<Brush> Brushes
        {
            get { return _brushes; }
        }

        protected override Freezable CreateInstanceCore()
        {
            return new BrushAnimation();
        }

        protected override object GetCurrentValueCore(object defaultOriginValue, object defaultDestinationValue, System.Windows.Media.Animation.AnimationClock clock)
        {
            if (_brushes.Count == 0)
            {
                return null;
            }

            // return discrete value ...
            int index = (int)Math.Floor((double)(clock.CurrentProgress * (_brushes.Count)));
            if (index < _brushes.Count)
            {
                return _brushes[index];
            }
            return _brushes[_brushes.Count - 1];
        }

        private List<Brush> _brushes;
    }
}

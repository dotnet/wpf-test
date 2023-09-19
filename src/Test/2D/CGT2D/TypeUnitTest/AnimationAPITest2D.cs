// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Media.Animation;
using Microsoft.Test.Graphics.TestTypes;

using Microsoft.Test.Graphics.Factories;
using TrustedAssembly = System.Reflection.Assembly;
using TrustedType = System.Type;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class AnimationAPITest2D : AnimationAPITestBase
    {
        /// <summary/>
        public override void Init(Variation v)
        {
            base.Init(v);
            // Geometry needs Brush, Pen and Transform to render
            if (v["Geometry"] != null)
            {
                v.AssertExistenceOf("Pen", "Brush", "Transform");
                _pen = PenFactory.MakePen(v["Pen"]);
                _brush = BrushFactory.MakeBrush(v["Brush"]);
                _transform = Transform2DFactory.MakeTransform2D(v["Transform"]);
                _geometry = GeometryFactory.MakeGeometry(v["Geometry"]);
                _geometry.Transform = _transform;
                _objectType = ObjectType.Geometry;
                goto StartAnimation;
            }

            if (v["Shape"] != null)
            {
                _shape = ShapeFactory.MakeShape(v["Shape"]);
                _objectType = ObjectType.Shape;
                goto StartAnimation;
            }

            if (v["Drawing"] != null)
            {
                _drawing = DrawingFactory.MakeDrawing(v["Drawing"]);
                _objectType = ObjectType.Drawing;
                goto StartAnimation;
            }

            throw new System.ApplicationException("AnimationAPITest for 2D needs Geometry/Shape/Drawing to start");

        StartAnimation:
            PrepareAnimation(v["ClassName"], v["Property"], v["From"], v["To"], v["DefaultValue"]);
        }

        /// <summary/>
        protected override IAnimatable GetAnimatingObject(string property, string propertyOwner)
        {
            TrustedAssembly presentationCore = TrustedAssembly.GetAssembly(typeof(Animatable));
            TrustedAssembly presentationFramework = TrustedAssembly.GetAssembly(typeof(Shape));

            // I assume all the stuff we test with this framework should fall into System.Windows.Media or System.Windows.Shapes namespace
            TrustedType ownerType = presentationCore.GetType("System.Windows.Media." + propertyOwner);
            if (ownerType == null)
            {
                ownerType = presentationFramework.GetType("System.Windows.Shapes." + propertyOwner);
            }

            if (ownerType.IsSubclassOf(typeof(Geometry)))
            {
                return GetAnimatable(property, _geometry);
            }
            else if (ownerType.IsSubclassOf(typeof(Pen)))
            {
                return GetAnimatable(property, _pen);
            }
            else if (ownerType.IsSubclassOf(typeof(Brush)))
            {
                return GetAnimatable(property, _brush);
            }
            else if (ownerType.IsSubclassOf(typeof(Transform)))
            {
                return GetAnimatable(property, _transform);
            }
            else if (ownerType.IsSubclassOf(typeof(Drawing)))
            {
                return GetAnimatable(property, _drawing);
            }
            else if (ownerType.IsSubclassOf(typeof(Shape)))
            {
                return GetAnimatable(property, _shape);
            }
            else
            {
                throw new NotSupportedException("Can't do animations on " + propertyOwner);
            }
        }

        /// <summary/>
        public override Visual GetWindowContent()
        {
            Visual returnedVisual = null;
            DrawingVisual drawingVisual = null;
            switch (_objectType)
            {
                case ObjectType.Geometry:
                    drawingVisual = new DrawingVisual();
                    using (DrawingContext ctx = drawingVisual.RenderOpen())
                    {
                        ctx.DrawGeometry(_brush, _pen, _geometry);
                    }
                    returnedVisual = drawingVisual;
                    break;

                case ObjectType.Shape:
                    Canvas canvas = new Canvas();
                    canvas.Width = WindowWidth;
                    canvas.Height = WindowHeight;
                    canvas.Background = new SolidColorBrush(BackgroundColor);
                    canvas.Children.Add(_shape);
                    returnedVisual = canvas;
                    break;

                case ObjectType.Drawing:
                    drawingVisual = new DrawingVisual();
                    using (DrawingContext ctx = drawingVisual.RenderOpen())
                    {
                        ctx.DrawDrawing(_drawing);
                    }
                    returnedVisual = drawingVisual;
                    break;

                default:
                    throw new System.ApplicationException("Unsupported ObjectType, " + _objectType.ToString());
            }

            return returnedVisual;
        }

        private enum ObjectType
        {
            Shape,
            Geometry,
            Drawing,
        }

        private Pen _pen;
        private Geometry _geometry;
        private Transform _transform;
        private Brush _brush;
        private Shape _shape;
        private Drawing _drawing;
        private ObjectType _objectType;
    }
}

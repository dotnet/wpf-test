using System;

using System.Windows;

using System.Windows.Controls;

using System.Windows.Media;
using System.Collections.Generic;
using System.Diagnostics;

namespace WpfControlToolkit
{
    public class TreeMapPanel : Panel
    {
        public static readonly DependencyProperty AreaProperty =
    DependencyProperty.RegisterAttached("Area",
                                        typeof(double),
                                        typeof(TreeMapPanel),
                                        new FrameworkPropertyMetadata(1.0,
                                                                      FrameworkPropertyMetadataOptions.AffectsParentArrange));

        public static double GetArea(DependencyObject element)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            return (double)element.GetValue(AreaProperty);
        }

        public static void SetArea(DependencyObject element, double value)
        {
            if (element == null)
                throw new ArgumentNullException("element");
            element.SetValue(AreaProperty, value);
        }

        private const double tol = 1e-2;

        protected override Size ArrangeOverride(Size finalSize)
        {
            if (finalSize.Width < tol || finalSize.Height < tol)
                return finalSize;

            UIElementCollection children = InternalChildren;
            ComputeWeightMap(children);

            Rect strip = new Rect(finalSize);
            double remainingWeight = _totalWeight;

            int arranged = 0;
            while (arranged < children.Count)
            {
                double bestStripWeight = 0;
                double bestRatio = double.PositiveInfinity;

                int i;

                if (finalSize.Width < tol || finalSize.Height < tol)
                    return finalSize;

                if (strip.Width > strip.Height)
                {
                    double bestWidth = strip.Width;

                    // Arrange Vertically
                    for (i = arranged; i < children.Count; i++)
                    {
                        double stripWeight = bestStripWeight + GetWeight(i);
                        double ratio = double.PositiveInfinity;
                        double width = strip.Width * stripWeight / remainingWeight;

                        for (int j = arranged; j <= i; j++)
                        {
                            double height = strip.Height * GetWeight(j) / stripWeight;
                            ratio = Math.Min(ratio, height > width ? height / width : width / height);

                            if (ratio > bestRatio)
                                goto ArrangeVertical;
                        }
                        bestRatio = ratio;
                        bestWidth = width;
                        bestStripWeight = stripWeight;
                    }

                ArrangeVertical:
                    double y = strip.Y;
                    for (; arranged < i; arranged++)
                    {
                        UIElement child = GetChild(children, arranged);

                        double height = strip.Height * GetWeight(arranged) / bestStripWeight;
                        child.Arrange(new Rect(strip.X, y, bestWidth, height));
                        y += height;
                    }

                    strip.X = strip.X + bestWidth;
                    strip.Width = Math.Max(0.0, strip.Width - bestWidth);
                }
                else
                {
                    double bestHeight = strip.Height;

                    // Arrange Horizontally
                    for (i = arranged; i < children.Count; i++)
                    {
                        double stripWeight = bestStripWeight + GetWeight(i);
                        double ratio = double.PositiveInfinity;
                        double height = strip.Height * stripWeight / remainingWeight;

                        for (int j = arranged; j <= i; j++)
                        {
                            double width = strip.Width * GetWeight(j) / stripWeight;
                            ratio = Math.Min(ratio, height > width ? height / width : width / height);

                            if (ratio > bestRatio)
                                goto ArrangeHorizontal;
                        }
                        bestRatio = ratio;
                        bestHeight = height;
                        bestStripWeight = stripWeight;
                    }

                ArrangeHorizontal:
                    double x = strip.X;
                    for (; arranged < i; arranged++)
                    {
                        UIElement child = GetChild(children, arranged);

                        double width = strip.Width * GetWeight(arranged) / bestStripWeight;
                        child.Arrange(new Rect(x, strip.Y, width, bestHeight));
                        x += width;
                    }

                    strip.Y = strip.Y + bestHeight;
                    strip.Height = Math.Max(0.0, strip.Height - bestHeight);
                }
                remainingWeight -= bestStripWeight;
            }

            _weightMap = null;
            _weights = null;

            return finalSize;
        }

        private UIElement GetChild(UIElementCollection children, int index)
        {
            return children[_weightMap[index]];
        }

        private double GetWeight(int index)
        {
            return _weights[_weightMap[index]];
        }

        private void ComputeWeightMap(UIElementCollection children)
        {
            _totalWeight = 0;

            _weightMap = new int[InternalChildren.Count];
            _weights = new double[InternalChildren.Count];

            for (int i = 0; i < _weightMap.Length; i++)
            {
                _weightMap[i] = i;
                _weights[i] = GetArea(children[i]);
                _totalWeight += _weights[i];
            }

            Array.Sort<int>(_weightMap, new Comparison<int>(CompareWeights));
        }

        private int CompareWeights(int index1, int index2)
        {
            return _weights[index2].CompareTo(_weights[index1]);
        }

        private double _totalWeight;
        private int[] _weightMap;
        private double[] _weights;

    }
}

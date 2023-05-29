// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Text;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Media;
using System.Diagnostics;

namespace Microsoft.Test.Input.MultiTouch
{
    /// <summary>
    /// Random values generator
    /// </summary>
    public static class RandomGenerator
    {
        #region Fields

        private static Random s_random;
        private static int? s_seed;

        #endregion

        #region Seed and Random Properties

        /// <summary>
        /// Gets/Sets the seed for the random generator
        /// </summary>
        public static int Seed
        {
            get
            {
                if (RandomGenerator.s_seed == null)
                {
                    RandomGenerator.s_seed = Environment.TickCount;
                }
                return (int)RandomGenerator.s_seed;
            }
            set
            {
                RandomGenerator.s_seed = value;
                RandomGenerator.s_random = new Random(value);
            }
        }

        private static Random Random
        {
            get
            {
                if (RandomGenerator.s_random == null)
                {
                    RandomGenerator.s_random = new Random(Seed);
                }
                return RandomGenerator.s_random;
            }
        }

        #endregion

        #region Doubles

        /// <summary>
        /// Returns a random double between 0 and max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double GetDouble(double max)
        {
            return GetDouble(0, max);
        }

        /// <summary>
        /// Returns a random double between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static double GetDouble(double min, double max)
        {
            return Random.NextDouble() * (max - min) + min;
        }

        #endregion

        #region Int

        /// <summary>
        /// Returns a random int between 0 and max
        /// </summary>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetInt(int max)
        {
            return Random.Next(max);
        }

        /// <summary>
        /// Returns a random int between min and max
        /// </summary>
        /// <param name="min"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public static int GetInt(int min, int max)
        {
            return Random.Next(min, max);
        }

        #endregion

        #region Bool

        /// <summary>
        /// Returns a random bool.
        /// </summary>
        /// <returns></returns>
        public static bool GetBoolean()
        {
            return Random.Next(2) == 0 ? false : true;
        }

        #endregion 

        #region Enum

        /// <summary>
        /// Returns a random enum from the valid enum values
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public static T GetEnum<T>()
        {
            Array values = Enum.GetValues(typeof(T));
            return (T)values.GetValue(RandomGenerator.GetInt(values.Length));
        }

        #endregion

        #region String

        /// <summary>
        /// Generates a random string based on the given template and expected result length
        /// </summary>
        /// <param name="template"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string GetString(string template, int length)
        {
            if (string.IsNullOrEmpty(template))
            {
                if (length == 0)
                {
                    return template;
                }
                else
                {
                    return new string('?', length);
                }
            }

            if (length <= template.Length)
            {
                return template.Substring(length);
            }

            StringBuilder builder = new StringBuilder(length);
            while (true)
            {
                int required = length - builder.Length;
                if (required <= template.Length)
                {
                    builder.Append(template.Substring(required));
                    break;
                }

                builder.Append(template);
            }
            return builder.ToString();
        }

        #endregion

        #region Transformation

        /// <summary>
        /// Type of the transformation
        /// </summary>
        [Flags]
        public enum TransformKind
        {
            /// <summary>
            /// No transformation
            /// </summary>
            None = 0,

            /// <summary>
            /// Apply Rotate transform
            /// </summary>
            Rotate = 1,

            /// <summary>
            /// Apply Scale transform
            /// </summary>
            Scale = 2,

            /// <summary>
            /// Apply Skew transform
            /// </summary>
            Skew = 4,

            /// <summary>
            /// Apply Translate transform
            /// </summary>
            Translate = 8,

            /// <summary>
            /// Apply all transformations
            /// </summary>
            All = Rotate | Scale | Skew | Translate,
        }

        /// <summary>
        /// Transformation options
        /// </summary>
        public class TransformOptions
        {
            /// <summary>
            /// Transformation type
            /// </summary>
            public TransformKind TransformKind = TransformKind.None;

            /// <summary>
            /// Minimum rotation
            /// </summary>
            public double MinRotation = 0;

            /// <summary>
            /// Maximum rotation
            /// </summary>
            public double MaxRotation = 0;

            /// <summary>
            /// Minimum scale along X
            /// </summary>
            public double MinScaleX = 1;

            /// <summary>
            /// Maximum scale along X
            /// </summary>
            public double MaxScaleX = 1;

            /// <summary>
            /// Minimum scale along Y
            /// </summary>
            public double MinScaleY = 1;

            /// <summary>
            /// Maximum scale along X
            /// </summary>
            public double MaxScaleY = 1;

            /// <summary>
            /// Minimum skew along X
            /// </summary>
            public double MinSkewX = 0;

            /// <summary>
            /// Maximum skew along X
            /// </summary>
            public double MaxSkewX = 0;

            /// <summary>
            /// Minimum skew along Y
            /// </summary>
            public double MinSkewY = 0;

            /// <summary>
            /// Maximum skew along X
            /// </summary>
            public double MaxSkewY = 0;

            /// <summary>
            /// Minimum offset along X
            /// </summary>
            public double MinOffsetX = 0;

            /// <summary>
            /// Maximum offset along X
            /// </summary>
            public double MaxOffsetX = 0;

            /// <summary>
            /// Minimum offset along Y
            /// </summary>
            public double MinOffsetY = 0;

            /// <summary>
            /// Maximum offset along Y
            /// </summary>
            public double MaxOffsetY = 0;

            /// <summary>
            /// Heavy tranformation
            /// </summary>
            public static readonly TransformOptions HeavyTransform;

            /// <summary>
            /// Light tranformation
            /// </summary>
            public static readonly TransformOptions LightTransform;

            static TransformOptions()
            {
                HeavyTransform = new TransformOptions();
                HeavyTransform.TransformKind = TransformKind.All;
                HeavyTransform.MinRotation = 0;
                HeavyTransform.MaxRotation = 360;
                HeavyTransform.MinScaleX = 0.3;
                HeavyTransform.MaxScaleX = 3;
                HeavyTransform.MinScaleY = 0.3;
                HeavyTransform.MaxScaleY = 3;
                HeavyTransform.MinSkewX = 0;
                HeavyTransform.MaxSkewX = 45;
                HeavyTransform.MinSkewY = 0;
                HeavyTransform.MaxSkewY = 45;
                HeavyTransform.MinOffsetX = -50;
                HeavyTransform.MaxOffsetX = 50;
                HeavyTransform.MinOffsetY = -50;
                HeavyTransform.MaxOffsetY = 50;

                LightTransform = new TransformOptions();
                LightTransform.TransformKind = TransformKind.All;
                LightTransform.MinRotation = -25;
                LightTransform.MaxRotation = 25;
                LightTransform.MinScaleX = 0.9;
                LightTransform.MaxScaleX = 1.1;
                LightTransform.MinScaleY = 0.9;
                LightTransform.MaxScaleY = 1.1;
                LightTransform.MinSkewX = -5;
                LightTransform.MaxSkewX = 5;
                LightTransform.MinSkewY = -5;
                LightTransform.MaxSkewY = 5;
                LightTransform.MinOffsetX = -5;
                LightTransform.MaxOffsetX = 5;
                LightTransform.MinOffsetY = -5;
                LightTransform.MaxOffsetY = 5;
            }

        }

        /// <summary>
        /// Returns a random transformation based on the given options.
        /// </summary>
        /// <param name="options"></param>
        /// <returns></returns>
        public static Transform GetTransform(TransformOptions options)
        {
            Matrix total = new Matrix();

            if ((options.TransformKind & TransformKind.Rotate) != 0)
            {
                RotateTransform rotate = new RotateTransform(RandomGenerator.GetDouble(options.MinRotation, options.MaxRotation));
                total.Append(rotate.Value);
            }

            if ((options.TransformKind & TransformKind.Scale) != 0)
            {
                ScaleTransform scale = new ScaleTransform(RandomGenerator.GetDouble(options.MinScaleX, options.MaxScaleX),
                    RandomGenerator.GetDouble(options.MinScaleY, options.MaxScaleY));
                total.Append(scale.Value);
            }

            if ((options.TransformKind & TransformKind.Skew) != 0)
            {
                SkewTransform skew = new SkewTransform(RandomGenerator.GetDouble(options.MinSkewX, options.MaxSkewX),
                    RandomGenerator.GetDouble(options.MinSkewY, options.MaxSkewY));
                total.Append(skew.Value);
            }

            if ((options.TransformKind & TransformKind.Translate) != 0)
            {
                TranslateTransform translate = new TranslateTransform(RandomGenerator.GetDouble(options.MinOffsetX, options.MaxOffsetX),
                    RandomGenerator.GetDouble(options.MinOffsetY, options.MaxOffsetY));
                total.Append(translate.Value);
            }

            return new MatrixTransform(total);
        }

        #endregion

        #region Point

        /// <summary>
        /// Returns a random point with int coordinates between the given two.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <returns></returns>
        public static Point GetIntPoint(Point topLeft, Point bottomRight)
        {
            return new Point(GetInt((int)topLeft.X, (int)bottomRight.X),
                GetInt((int)topLeft.Y, (int)bottomRight.Y));
        }


        /// <summary>
        /// Returns a random point between the given two.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="bottomRight"></param>
        /// <returns></returns>
        public static Point GetPoint(Point topLeft, Point bottomRight)
        {
            return new Point(GetDouble(topLeft.X, bottomRight.X),
                GetDouble(topLeft.Y, bottomRight.Y));
        }

        #endregion 
    }
}

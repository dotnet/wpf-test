// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;
using System.ComponentModel;

using BindingFlags = System.Reflection.BindingFlags;
using Microsoft.Test.Graphics.TestTypes;
using Microsoft.Test.Graphics;

using TrustedPropertyInfo = System.Reflection.PropertyInfo;
using TrustedType = System.Type;

namespace Microsoft.Test.Graphics
{
    /// <summary/>
    public class BrushMiniLanguageTest : MiniLanguageTest
    {
        /// <summary/>
        protected override Type ClassType
        {
            get
            {
                return typeof(SolidColorBrush);
            }
        }

        /// <summary/>
        protected override void RunParseTest(bool isValid)
        {
            TupleColorBrushTest(isValid, true, true);
            TupleColorBrushTest(isValid, true, false);
            TupleColorBrushTest(isValid, false, true);
            TupleColorBrushTest(isValid, false, false);

            ScColorBrushTest(isValid, true);
            ScColorBrushTest(isValid, false);

            NamedColorBrushTest(isValid);
        }

        /// <summary/>
        protected override void CheckTest()
        {
            Color c = Color.FromRgb(50, 30, 20);

            SolidColorBrush a = PackBrush(c, false, true);
            String s = randomGenerator.PadTupleColor(c, false, true);

            CompareParsedObject(a, s, true);

            CompareParsedObject(a, "A color string with invalid tokens", false);
        }

        /// <summary/>
        protected override void ParseTestCore()
        {
            //generic Minilanguage parse test mechanism is not well suited
            //for the multiple format variations present with colors, and thus is not being used.
        }

        /// <summary/>
        protected void CompareParsedObject(Object source, String check, bool expectEqual)
        {
            try
            {
                Object duplicate = ConvertObject(check, expectEqual);

                LogObjects(source, duplicate);

                if (duplicate != null && !ObjectUtils.DeepEquals(source, duplicate))
                {
                    AddFailure("Color compare produced unexpected result- SRC:{0} DUP:{1}", source, duplicate);

                    LogParseString(check);
                }
            }
            catch (MiniLanguageTestTerminationException)
            {
            }
        }

        private void TupleColorBrushTest(bool isValid, bool hasAlpha, bool byteSized)
        {
            Log("Starting Color Tuple Test with Alpha Channel={0}, Byte Sized Channels={1}",
                hasAlpha, byteSized);

            randomGenerator.KeepClean = isValid;
            for (int i = 0; i < numIterations; i++)
            {
                randomGenerator.ResetCleanFlag();
                Color color = randomGenerator.CreateColor(hasAlpha, byteSized);

                serialValue = randomGenerator.PadTupleColor(color, hasAlpha, byteSized);

                SolidColorBrush source = PackBrush(color, hasAlpha, byteSized);

                CompareParsedObject(source, serialValue, randomGenerator.IsClean);
            }
        }

        private void ScColorBrushTest(bool isValid, bool hasAlpha)
        {
            Log("Starting scRGB color test, with alpha={0}", hasAlpha);

            randomGenerator.KeepClean = isValid;
            for (int i = 0; i < numIterations; i++)
            {
                randomGenerator.ResetCleanFlag();
                Color color = randomGenerator.CreateColor(hasAlpha, true);

                serialValue = randomGenerator.PadScColor(color, hasAlpha);

                SolidColorBrush source = PackBrush(color, hasAlpha, true);

                CompareParsedObject(source, serialValue, randomGenerator.IsClean);
            }
        }

        private void NamedColorBrushTest(bool isValid)
        {
            Log("Starting NamedColorBrushTest for all named colors.");

            randomGenerator.KeepClean = isValid;

            //Iterate through all the named brushes.
            TrustedType type = PT.Trust(typeof(Brushes));
            foreach (TrustedPropertyInfo field in type.GetProperties(BindingFlags.Static | BindingFlags.Public))
            {
                randomGenerator.ResetCleanFlag();
                SolidColorBrush source = (SolidColorBrush)ConvertObject(field.Name, randomGenerator.IsClean);

                serialValue = randomGenerator.PadString(field.Name);
                CompareParsedObject(source, serialValue, randomGenerator.IsClean);
            }
        }

        private void ProfileColorBrushTest(bool isValid)
        {
            randomGenerator.KeepClean = isValid;
            for (int i = 0; i < numIterations; i++)
            {
                Log("Starting ProfileColorBrushTest for Context Colors.");
                try
                {
                    BrushConverter bc = new BrushConverter();
                    SolidColorBrush z = (SolidColorBrush)bc.ConvertFromString("ContextColor file://C:/WINDOWS/system32/spool/drivers/color/is330.icm  0.50, 0.0, 0.5, 0.5 ");
                }
                catch (Exception e)
                {
                    Log(e.ToString());
                }
            }
        }

        private SolidColorBrush PackBrush(Color color, bool hasAlpha, bool isByte)
        {
            if (isByte)
            {
                //Data here is already prepared for placement in brush
            }
            else
            {
                if (hasAlpha)
                {
                    color.A = (byte)(color.A * 16 + color.A);
                }
                color.R = (byte)(color.R * 16 + color.R);
                color.G = (byte)(color.G * 16 + color.G);
                color.B = (byte)(color.B * 16 + color.B);
            }
            return new SolidColorBrush(color);
        }
    }
}

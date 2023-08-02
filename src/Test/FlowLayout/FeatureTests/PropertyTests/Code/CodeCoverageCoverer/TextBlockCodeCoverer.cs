// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Controls;
using System.Windows.Media;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - TextBlock", MethodName = "Run")]
    public class TextBlockCodeCoverer : AvalonTest
    {
        private DerivedTextBlock _tb;
        private Run _run1;
        private Window _w1;

        public TextBlockCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Initialize()
        {
            _w1 = new Window();
            UISetup(_w1, out _tb, out _run1);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();            
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_w1, _tb, _run1);
            FailMethods(_tb);
            GetProperties(_tb);
            SetProperties(_tb);
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out DerivedTextBlock tb, out Run run1)
        {
            TextBlock tb1 = new TextBlock(new Run());
            tb = new DerivedTextBlock();
            SimpleDecoratorForTextBlock dc = new SimpleDecoratorForTextBlock();
            ReflectionHelper rh = ReflectionHelper.WrapObject(tb);
            run1 = new Run("Loe dem o. Me and Kelis on Ducatis");
            tb.Inlines.Add(run1);
            tb.Inlines.Add(dc);
            w1.Content= tb;
            w1.Show();
        }

        private void FailMethods(DerivedTextBlock tb)
        {
            TestLog textBlockFailMethods = new TestLog("TextBLock Fail Methods");
            
            try
            {
                //This will return later and throw an Exception
                tb.CallGetRectanglesCore(new List());
            }
            catch (System.IO.InvalidDataException) { }

            try
            {
                // This will return later and throw an Exception
                tb.CallGetRectanglesCore(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                // Cover the null case. This is definitely going to throw
                TextBlock.GetBaselineOffset(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                //Hit Test Core with null point
                tb.CallHitTestCore(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (tb as IAddChild).AddChild(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (tb as IServiceProvider).GetService(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (tb as IAddChild).AddChild(new NullClass());
            }
            catch (ArgumentException) { }

            try
            {
                tb.OnRender(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                //This *MIGHT* throw an exception as the layout is invalid
                tb.Text = "Some Thing";
                tb.CallGetRectanglesCore(new List());
            }
            catch (System.IO.InvalidDataException) { }

            try
            {
                //This will throw an exception as the tb does not contain the new list
                tb.Text = "Some Thing2";
                CommonFunctionality.FlushDispatcher();
                tb.CallGetRectanglesCore(new List());
            }
            catch (System.IO.InvalidDataException) { }

            try
            {
                TextBlock tb1 = new TextBlock(null);
            }
            catch (ArgumentNullException) { }

            textBlockFailMethods.Result = TestResult.Pass;
            textBlockFailMethods.Close();
        }

        private void PassMethods(Window w1, DerivedTextBlock tb, Run run1)
        {
            TestLog textBlockPassMethods = new TestLog("TextBlock Pass Methods");
            
            // GetPositionFromPoint
            tb.GetPositionFromPoint(new Point(), false);

            // GetBaselineOffset
            TextBlock.GetBaselineOffset(tb);

            // calls GetLineProperties            
            tb.CallInputHitTestCore(new Point());

            // Hit Test Core with Point Inside                
            tb.CallHitTestCore(new PointHitTestParameters(new Point()));
            // Hit Test Core with Point Outside
            tb.CallHitTestCore(new PointHitTestParameters(new Point(-1, -1)));

            // OnTextDecorationsChanged
            tb.TextDecorations = new TextDecorationCollection();
            tb.TextDecorations.Add(new TextDecoration());

            // using the IContentHost methods
            ((IContentHost)tb).GetRectangles(run1);
            IEnumerator<IInputElement> enumerator = ((IContentHost)tb).HostedElements;

            // This will return earlier
            tb.CallGetRectanglesCore(run1);

            LineBreak lb = new LineBreak();
            (tb as IAddChild).AddChild(lb);
            CommonFunctionality.FlushDispatcher();
            tb.CallGetRectanglesCore(lb);

            Type textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextContainer");
            (tb as IServiceProvider).GetService(textViewType);
            textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.TextContainer");
            (tb as IServiceProvider).GetService(textViewType);

            TextBlock newTextBlock1 = new TextBlock(new Run("Some Content"));
            ReflectionHelper rh = ReflectionHelper.WrapObject(newTextBlock1);
            newTextBlock1.BaselineOffset = 10.0;
            newTextBlock1.ShouldSerializeBaselineOffset();
            newTextBlock1.ShouldSerializeText();
            newTextBlock1.ShouldSerializeInlines(null);

            textBlockPassMethods.Result = TestResult.Pass;
            textBlockPassMethods.Close();
        }

        private void GetProperties(DerivedTextBlock tb)
        {
            TextBlock.GetBaselineOffset(tb);
            TextBlock.GetFlowDirection(tb);
            TextBlock.GetFontFamily(tb);
            TextBlock.GetFontSize(tb);
            TextBlock.GetFontStretch(tb);
            TextBlock.GetFontStyle(tb);
            TextBlock.GetFontWeight(tb);
            TextBlock.GetForeground(tb);
            TextBlock.GetLineHeight(tb);
            TextBlock.GetTextAlignment(tb);
        }

        private void SetProperties(DerivedTextBlock tb)
        {
            // set_Properties
            tb.Background = new DrawingBrush();
            tb.BaselineOffset = 5.0;
            tb.FontFamily = new FontFamily("Tahoma");
            tb.FontStretch = FontStretches.Condensed;
            tb.FontSize = 12.0;
            tb.FontStyle = FontStyles.Normal;
            tb.FontWeight = FontWeights.Black;
            tb.TextAlignment = TextAlignment.Center;
            tb.TextDecorations = new TextDecorationCollection();
            tb.TextEffects = new TextEffectCollection();
            tb.Foreground = Brushes.Azure;
            tb.LineHeight = 10.0;

            TestLog textBlockSetProperties = new TestLog("TextBlock Set Properties");
            
            try
            {
                TextBlock.SetBaselineOffset(null, 5.0);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetBaselineOffset(tb, 5.0);

            try
            {
                TextBlock.SetFontFamily(null, new FontFamily("Tahoma"));
            }
            catch (ArgumentNullException) { }
            TextBlock.SetFontFamily(tb, new FontFamily("Tahoma"));

            try
            {
                TextBlock.SetFontSize(null, 10.0);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetFontSize(tb, 10.0);

            try
            {
                TextBlock.SetFontStretch(null, FontStretches.Condensed);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetFontStretch(tb, FontStretches.Condensed);

            try
            {
                TextBlock.SetFontStyle(null, FontStyles.Normal);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetFontStyle(tb, FontStyles.Normal);

            try
            {
                TextBlock.SetFontWeight(null, FontWeights.Normal);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetFontWeight(tb, FontWeights.Normal);

            try
            {
                TextBlock.SetForeground(null, tb.Foreground);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetForeground(tb, tb.Foreground);

            try
            {
                TextBlock.SetLineHeight(null, 10.0);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetLineHeight(tb, 10.0);

            try
            {
                TextBlock.SetTextAlignment(null, TextAlignment.Right);
            }
            catch (ArgumentNullException) { }
            TextBlock.SetTextAlignment(tb, TextAlignment.Right);

            try
            {
                (tb as IAddChild).AddText(null);
            }
            catch (ArgumentNullException) { }

            textBlockSetProperties.Result = TestResult.Pass;
            textBlockSetProperties.Close();

            tb.FontFamily = new FontFamily("Arial");
            tb.FontSize = 15.0;
            tb.Height = 100;
            tb.Width = 100;
            tb.Text = "Long Text So I can Wrap or get Ellipsis";
            tb.TextTrimming = TextTrimming.CharacterEllipsis;
            CommonFunctionality.FlushDispatcher();
            tb.ArrangeOverride(new Size(20, 20));

            DerivedTextBlock tb2 = new DerivedTextBlock();
            tb2.getHostedElementsCore();
        }
    }
}

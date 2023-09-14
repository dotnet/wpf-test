// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows;
using System.Windows.Markup;
using System.Windows.Documents;
using System.Windows.Media;
using System.Reflection;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - FlowDocument", MethodName = "Run")]
    public class FlowDocumentCodeCoverer : AvalonTest
    {
        private List _list1;
        private DerivedFlowDocument _fd;
        private SimpleDecoratorForFlowDocument _dc;
        private Window _w1;

        public FlowDocumentCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Initialize()
        {
            _w1 = new Window();
            UISetup(_w1, out _fd, out _list1, out _dc);

            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();           
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_fd, _list1, _dc);
            FailMethods(_fd);
            GetProperties(_fd);
            SetProperties(_fd);

            return TestResult.Pass;
        }

        private void UISetup(Window w1, out DerivedFlowDocument fd, out List list1, out SimpleDecoratorForFlowDocument dc)
        {
            fd = new DerivedFlowDocument();
            dc = new SimpleDecoratorForFlowDocument();
            Paragraph p = new Paragraph();
            p.Inlines.Add(dc);
            ReflectionHelper rh = ReflectionHelper.WrapObject(fd);

            list1 = new List();
            (fd as IAddChild).AddChild(list1);
            (fd as IAddChild).AddChild(p);
            w1.Content= fd;
            w1.Show();
        }

        private void PassMethods(DerivedFlowDocument fd, List list1, SimpleDecoratorForFlowDocument dc)
        {
            TestLog flowDocumentPassMethods = new TestLog("FlowDocument Pass Methods");

            ReflectionHelper rh = ReflectionHelper.WrapObject(fd);

            ((IDocumentPaginatorSource)fd).DocumentPaginator.CancelAsync(fd);
            ((DynamicDocumentPaginator)((IDocumentPaginatorSource)fd).DocumentPaginator).GetObjectPosition(list1);
            ((IDocumentPaginatorSource)fd).DocumentPaginator.GetPage(0);
            ((IDocumentPaginatorSource)fd).DocumentPaginator.GetPage(3);

            BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
            MethodInfo mInfo = fd.GetType().GetMethod("OnChildDesiredSizeChanged", flags, null, new Type[] { typeof(UIElement) }, null);
            rh.CallMethod(mInfo, new UIElement());

            fd.PagePadding = new Thickness(1, 1, 1, 1);
            fd.PageWidth = 926;
            fd.PageHeight = 658;

            try
            {
                Type textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextContainer");
                (fd as IServiceProvider).GetService(textViewType);
                textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.TextContainer");
                (fd as IServiceProvider).GetService(textViewType);
                (fd as IAddChild).AddChild(new FrameworkContentElement());

            }
            catch (System.ArgumentException) { }

            flowDocumentPassMethods.Result = TestResult.Pass;
            flowDocumentPassMethods.Close();
        }

        private void FailMethods(DerivedFlowDocument fd)
        {
            TestLog flowDocumentFailMethods = new TestLog("FlowDocument Fail Methods");
            
            DocumentPaginator dp = ((IDocumentPaginatorSource)fd).DocumentPaginator;

            dp.CancelAsync(null);

            try
            {
                // null exception
                ((DynamicDocumentPaginator)dp).GetObjectPosition(null);
            }
            catch (ArgumentNullException) { }


            // new List does not have same container as fd
            ((DynamicDocumentPaginator)dp).GetObjectPosition(new List());

            try
            {
                //exception on -1
                dp.GetPage(-1);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                // exception on -1
                dp.GetPageAsync(-1, fd);
            }
            catch (ArgumentOutOfRangeException) { }

            try
            {
                ((DynamicDocumentPaginator)dp).GetPageNumber(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                ((DynamicDocumentPaginator)dp).GetPageNumber(ContentPosition.Missing);
            }
            catch (ArgumentException) { }

            try
            {
                ((DynamicDocumentPaginator)dp).GetPageNumberAsync(null, fd);
            }
            catch (ArgumentNullException) { }

            try
            {
                // exception on null
                ((IServiceProvider)fd).GetService(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                //exception on null
                (fd as IAddChild).AddChild(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                //exception on null
                (fd as IAddChild).AddChild(new NullClass());
            }
            catch (ArgumentException) { }

            try
            {
                //exception on null
                (fd as IAddChild).AddText(null);
            }
            catch (ArgumentNullException) { }

            flowDocumentFailMethods.Result = TestResult.Pass;
            flowDocumentFailMethods.Close();
        }

        private void GetProperties(DerivedFlowDocument fd)
        {
            ReflectionHelper rh = ReflectionHelper.WrapObject(fd);
            double columnGap = fd.ColumnGap;
            Brush brush = fd.ColumnRuleBrush;
            double columnRuleWidth = fd.ColumnRuleWidth;
            double columnWidth = fd.ColumnWidth;
            FontStretch fontStretch = fd.FontStretch;
            bool isColWidthFlexible = fd.IsColumnWidthFlexible;
            double lineheight = fd.LineHeight;
            TextAlignment textAlignment = fd.TextAlignment;
            TextEffectCollection textEffects = fd.TextEffects;
            Typography typography = fd.Typography;
            rh.GetProperty("StructuralCache");
        }

        private void SetProperties(DerivedFlowDocument fd)
        {
            fd.TextEffects = new TextEffectCollection();
            CommonFunctionality.FlushDispatcher();
            fd.TextEffects = new TextEffectCollection();
            CommonFunctionality.FlushDispatcher();
            fd.TextEffects.Add(new TextEffect());

            fd.ColumnGap = 1.0;
            fd.ColumnRuleBrush = new DrawingBrush();
            fd.ColumnRuleWidth = 1.0;
            fd.ColumnWidth = 25.0;
            fd.FlowDirection = FlowDirection.LeftToRight;
            fd.FontFamily = new FontFamily("Arial");
            fd.FontSize = 14.0;
            fd.FontStretch = FontStretches.Normal;
            fd.FontStyle = FontStyles.Normal;
            fd.FontWeight = FontWeights.Normal;
            fd.Foreground = fd.Background;
            fd.IsOptimalParagraphEnabled = false;
            fd.IsColumnWidthFlexible = false;
            fd.LineHeight = 15.0;
            fd.MaxPageHeight = 100.0;
            fd.MaxPageWidth = 100.0;
            fd.MinPageHeight = 10.0;
            fd.MinPageWidth = 10.0;
            fd.PageHeight = 95.0;
            fd.PagePadding = new Thickness(1, 1, 1, 1);
            fd.PageWidth = 95.0;
            fd.TextAlignment = TextAlignment.Center;
            CommonFunctionality.FlushDispatcher();

            fd.PageHeight = Double.NaN;
            fd.PageWidth = Double.NaN;
            CommonFunctionality.FlushDispatcher();

            fd.MinPageHeight = 1000.0;
            fd.MinPageWidth = 1000.0;
            CommonFunctionality.FlushDispatcher();
        }
    }
}

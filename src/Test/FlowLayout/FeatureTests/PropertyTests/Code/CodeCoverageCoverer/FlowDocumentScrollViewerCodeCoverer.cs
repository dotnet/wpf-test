// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Windows.Documents;
using System.Windows;
using System.Reflection;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Controls;

using Microsoft.Test.TestTypes;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Layout
{
    [Test(2, "PropertyTests", "Code Coverage - FlowDocumentScrollViewer", MethodName = "Run")]
    public class FlowDocumentScrollViewerCodeCoverer : AvalonTest
    {
        private DerivedFlowDocumentScrollViewer _fdsv1;
        private Paragraph _p;
        private Window _w1;

        public FlowDocumentScrollViewerCodeCoverer()
        {
            CreateLog = false;
            InitializeSteps += new TestStep(Initialize);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(RunTest);
        }

        private TestResult Initialize()
        {
            _w1 = new Window();
            UISetup(_w1, out _fdsv1, out _p);
            return TestResult.Pass;
        }

        private TestResult CleanUp()
        {
            _w1.Close();            
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            PassMethods(_fdsv1, _p);
            FailMethods(_fdsv1);
            SetProperties(_fdsv1);
            return TestResult.Pass;
        }

        private void UISetup(Window w1, out DerivedFlowDocumentScrollViewer fdsv1, out Paragraph p)
        {
            fdsv1 = new DerivedFlowDocumentScrollViewer();
            p = new Paragraph();
            Figure f = new Figure();
            BlockUIContainer buc = new BlockUIContainer();
            buc.Child = new Button();
            f.Blocks.Add(buc);
            p.Inlines.Add(f);
            fdsv1.Document = new FlowDocument();
            fdsv1.Document.Blocks.Add(p);
            w1.Content= fdsv1;
            fdsv1.Document.FlowDirection = FlowDirection.RightToLeft;

            Span sp = new Span(new Run());
            w1.Show();
        }

        private void PassMethods(DerivedFlowDocumentScrollViewer fdsv1, Paragraph p)
        {
            TestLog flowDocumentScrollViewerPassMethods = new TestLog("FLowDocumentScrollViewer Pass Methods");

            ReflectionHelper rh = ReflectionHelper.WrapObject(fdsv1);
            BindingFlags flags = BindingFlags.Instance | BindingFlags.NonPublic;

            MethodInfo mInfo = fdsv1.GetType().GetMethod("GetPlainText", flags);
            rh.CallMethod(mInfo, null);

            List list1 = new List();

            CommonFunctionality.FlushDispatcher();

            fdsv1.CallHitTestCore(new PointHitTestParameters(new Point()));
            fdsv1.CallHitTestCore(new PointHitTestParameters(new Point(-1, -1)));

            Type textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextContainer");
            (fdsv1 as IServiceProvider).GetService(textViewType);
            textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.TextContainer");
            (fdsv1 as IServiceProvider).GetService(textViewType);
            textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Documents.ITextView");
            (fdsv1 as IServiceProvider).GetService(textViewType);
            textViewType = ReflectionHelper.GetTypeFromName("System.Windows.Controls.TextBlock");
            (fdsv1 as IServiceProvider).GetService(textViewType);

            p.KeepTogether = false;
            p.KeepWithNext = false;
            p.MinOrphanLines = p.MinOrphanLines;
            p.MinWidowLines = p.MinWidowLines;
            p.TextDecorations = TextDecorations.Underline;
            p.TextIndent = 10;

            flowDocumentScrollViewerPassMethods.Result = TestResult.Pass;
            flowDocumentScrollViewerPassMethods.Close();
        }

        private void FailMethods(DerivedFlowDocumentScrollViewer fdsv1)
        {
            TestLog flowDocumentScrollViewerFailMethods = new TestLog("FLowDocumentScrollViewer Fail Methods");
            
            ReflectionHelper rh = ReflectionHelper.WrapObject(fdsv1);

            fdsv1.CallHitTestCore(null);
            fdsv1.OnChildDesiredSizeChanged(null);
            fdsv1.ParentLayoutInvalidated(null);

            try
            {
                (fdsv1 as IServiceProvider).GetService(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (fdsv1 as IAddChild).AddChild(null);
            }
            catch (ArgumentNullException) { }

            try
            {
                (fdsv1 as IAddChild).AddChild(new NullClass());
            }
            catch (ArgumentException) { }

            (fdsv1 as IAddChild).AddText(null);

            Span sp = new Span(null);

            flowDocumentScrollViewerFailMethods.Result = TestResult.Pass;
            flowDocumentScrollViewerFailMethods.Close();
        }

        private void SetProperties(DerivedFlowDocumentScrollViewer fdsv1)
        {
            fdsv1.Document.LineHeight = 15.0;
            fdsv1.Document.TextAlignment = TextAlignment.Center;
        }
    }
}

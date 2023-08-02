// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.UtilityHelper;

using Microsoft.Test;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Threading;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshIntraResourceStyleReferenceTest
{
    /******************************************************************************
    * CLASS:          IntraResourceStyleReference
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "IntraResourceStyleReference", Disabled=true)]
    public class IntraResourceStyleReference : AvalonTest
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("ProgrammaticCV01")]
        [Variation("ProgrammaticCV02")]
        [Variation("ProgrammaticCV03")]
        [Variation("ProgrammaticCV04")]
        [Variation("ProgrammaticCV05")]
        [Variation("ProgrammaticCV06")]
        [Variation("ProgrammaticCV07")]
        [Variation("ProgrammaticCV08")]

        /******************************************************************************
        * Function:          IntraResourceStyleReference Constructor
        ******************************************************************************/
        public IntraResourceStyleReference(string arg)
        {
            _testName = arg;
            RunSteps += new TestStep(StartTest);
        }
        #endregion


        #region Test Steps
        /******************************************************************************
        * Function:          StartTest
        ******************************************************************************/
        /// <summary>
        /// Entry Method for the test case
        /// </summary>
        TestResult StartTest()
        {
            switch (_testName)
            {
                case "ProgrammaticCV01":
                    new ProgrammaticCV01().Run();
                    break;
                case "ProgrammaticCV02":
                    new ProgrammaticCV02().Run();
                    break;
                case "ProgrammaticCV03":
                    new ProgrammaticCV03().Run();
                    break;
                case "ProgrammaticCV04":
                    new ProgrammaticCV04().Run();
                    break;
                case "ProgrammaticCV05":
                    new ProgrammaticCV05().Run();
                    break;
                case "ProgrammaticCV06":
                    new ProgrammaticCV06().Run();
                    break;
                case "ProgrammaticCV07":
                    new ProgrammaticCV07().Run();
                    break;
                case "ProgrammaticCV08":
                    new ProgrammaticCV08().Run();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by Asserts.
            return TestResult.Pass;
        }
        #endregion


        #region Public Members
        /******************************************************************************
        * Function:          FindElementById
        ******************************************************************************/
        /// <summary>
        /// Finds the first element in the given scope with the specified id
        /// in the visual tree.
        /// </summary>
        /// <param name='scope'>Scope from which to begin search.</param>
        /// <param name='id'>Name of framework element.</param>
        /// <returns>The element found, null otherwise.</returns>
        /// <remarks>
        /// Note that this is not what the end-user would expect to use.
        /// However, it is a valid search operation, and it is required
        /// to work around a Window 

        public static FrameworkElement FindElementById(DependencyObject scope, string id)
        {
            if (scope == null)
            {
                throw new ArgumentNullException("scope");
            }

            FrameworkElement result = scope as FrameworkElement;
            if (result != null && result.Name == id)
                return result;

            int count = VisualTreeHelper.GetChildrenCount(scope);
            for(int i = 0; i < count; i++)
            {
                // Common base class for Visual and Visual3D is DependencyObject
                DependencyObject visual = VisualTreeHelper.GetChild(scope,i);

                result = FindElementById(visual, id);
                if (result != null)
                    return result;
            }

            // ContainerVisual's may have further Visual elements to examine
            ContainerVisual cv = scope as ContainerVisual;
            if (cv != null)
            {
                foreach (Visual visual in cv.Children)
                {
                    result = FindElementById(visual, id);
                    if (result != null)
                        return result;
                }
            }

            return null;
        }
        #endregion


        #region Private Classes
        /******************************************************************************
        * CLASS:          ProgrammaticCV01
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by control resource.
        /// </summary>
        private class ProgrammaticCV01 : Test
        {
            public ProgrammaticCV01() { }

            protected override void Setup()
            {
                Title = "Simple style, referenced by control resource.";

                StackPanel sp = new StackPanel();

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                sp.Resources["tpStyle"] = tpStyle;

                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                sp.Resources["tp"] = tp;
                tp.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");


                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                sp.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                Button btn = new Button();
                sp.Children.Add(btn);
                btn.Content = _tpExp = (FlowDocumentScrollViewer)btn.FindResource("tp");
            }

            protected override void Validate()
            {
                CV01ValidateState(_tpCtrl, _tpExp);
            }

            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV01ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = sp.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = sp.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("4. Button contains a text panel.");
            FlowDocumentScrollViewer tpExp = btn.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            CV01ValidateState(tpCtrl, tpExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV0[346]ValidateAll.
        /// </remarks>
        /// <param name="tpCtrl">Control text panel.</param>
        /// <param name="tpExp">Experimental text panel.</param>
        public static void CV01ValidateState(FlowDocumentScrollViewer tpCtrl, FlowDocumentScrollViewer tpExp)
        {
            Utilities.PrintStatus("Verify control text panel state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpCtrl.Foreground).Color == Colors.Red, "");
            Utilities.PrintStatus("2. TextBlock panel content.");
            Utilities.Assert((new TextRange(tpCtrl.Document.ContentStart, tpCtrl.Document.ContentEnd)).Text == "Control\r\n", "(control)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental text panel state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpExp.Foreground).Color == Colors.Red, "");
            Utilities.PrintStatus("2. TextBlock panel content.");

            string txtContent = "";
            int count = VisualTreeHelper.GetChildrenCount(tpExp);
            if (count>0)
            {
                // We must delve into the VisualTree
                TextBlock txtExp = (TextBlock)FindElementById(tpExp, "txtExperiment");
                if (txtExp != null)
                {
                    txtContent = txtExp.Text;
                }
            }
            else
            {
                txtContent = (new TextRange(tpExp.Document.ContentStart, tpExp.Document.ContentEnd)).Text;
            }

            Utilities.Assert(txtContent == "Experiment\r\n", "(experiment)");
            Utilities.PrintStatus("");
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV02
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by control resource in ancestral resource dictionary.
        /// </summary>
        private class ProgrammaticCV02 : Test
        {
            public ProgrammaticCV02() { }

            protected override void Setup()
            {
                Title = "Simple style, referenced by control resource in ancestral resource dictionary.";

                StackPanel spRoot = new StackPanel();
                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                tp.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");
                spRoot.Resources["tp"] = tp;

                // Child StackPanel #1

                StackPanel spChild1 = new StackPanel();
                spRoot.Children.Add(spChild1);

                Style style1 = new Style();
                style1.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spChild1.Resources["tpStyle"] = style1;

                // Control subject
                _tpCtrl1 = new FlowDocumentScrollViewer();
                _tpCtrl1.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild1.Children.Add(_tpCtrl1);
                _tpCtrl1.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                Button btn1 = new Button();
                spChild1.Children.Add(btn1);
                FlowDocumentScrollViewer tp1 = new FlowDocumentScrollViewer();
                tp1.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                tp1.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                btn1.Content = _tpExp1 = tp1;

                // Child StackPanel #2

                StackPanel spChild2 = new StackPanel();
                spRoot.Children.Add(spChild2);

                Style style2 = new Style();
                style2.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Blue));
                spChild2.Resources["tpStyle"] = style2;

                // Control subject
                _tpCtrl2 = new FlowDocumentScrollViewer();
                _tpCtrl2.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild2.Children.Add(_tpCtrl2);
                _tpCtrl2.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                Button btn2 = new Button();
                spChild2.Children.Add(btn2);
                FlowDocumentScrollViewer tp2 = new FlowDocumentScrollViewer();
                tp2.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                tp2.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                btn2.Content = _tpExp2 = tp2;
            }

            protected override void Validate()
            {
                CV02ValidateState(_tpCtrl1, _tpExp1, _tpCtrl2, _tpExp2);
            }

            private FlowDocumentScrollViewer _tpCtrl1, _tpCtrl2;
            private FlowDocumentScrollViewer _tpExp1, _tpExp2;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV02ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel spRoot = rootElem as StackPanel;
            Utilities.Assert(spRoot != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 2 child StackPanel's present.");
            Utilities.Assert(spRoot.Children.Count == 2, "2 children");
            StackPanel spChild1 = spRoot.Children[0] as StackPanel;
            Utilities.Assert(spChild1 != null, "Children[0] (child stack panel 1)");
            StackPanel spChild2 = spRoot.Children[1] as StackPanel;
            Utilities.Assert(spChild2 != null, "Children[1] (child stack panel 2)");
            Utilities.PrintStatus("");

            // Child StackPanel #1

            Utilities.PrintStatus("StackPanel #1:");
            Utilities.PrintStatus("3. 2 child elements present.");
            Utilities.Assert(spChild1.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("4. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl1 = spChild1.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl1 != null, "Children[0] (control text panel)");
            Button btn1 = spChild1.Children[1] as Button;
            Utilities.Assert(btn1 != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("5. Button contains a FlowDocumentScrollViewer.");
            FlowDocumentScrollViewer tpExp1 = btn1.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp1 != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            // Child StackPanel #2

            Utilities.PrintStatus("StackPanel #2:");
            Utilities.PrintStatus("6. 2 child elements present.");
            Utilities.Assert(spChild2.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("7. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl2 = spChild2.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl2 != null, "Children[0] (control text panel)");
            Button btn2 = spChild2.Children[1] as Button;
            Utilities.Assert(btn2 != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("8. Button contains a FlowDocumentScrollViewer.");
            FlowDocumentScrollViewer tpExp2 = btn2.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp2 != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            CV02ValidateState(tpCtrl1, tpExp1, tpCtrl2, tpExp2);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV05ValidateAll.
        /// </remarks>
        /// <param name="tpCtrl1"></param>
        /// <param name="tpExp1"></param>
        /// <param name="tpCtrl2"></param>
        /// <param name="tpExp2"></param>
        public static void CV02ValidateState(FlowDocumentScrollViewer tpCtrl1, FlowDocumentScrollViewer tpExp1, FlowDocumentScrollViewer tpCtrl2, FlowDocumentScrollViewer tpExp2)
        {
            Utilities.PrintStatus("StackPanel #1:");
            Utilities.PrintStatus("Verify control FlowDocumentScrollViewer state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpCtrl1.Document.Foreground).Color == Colors.Red, "");
            Utilities.PrintStatus("2. TextBlock panel content.");
            Utilities.Assert((new TextRange(tpCtrl1.Document.ContentStart, tpCtrl1.Document.ContentEnd)).Text == "Control\r\n", "(control)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental text panel state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpExp1.Document.Foreground).Color == Colors.Red, "");

            string txtContent = "";
            int count = VisualTreeHelper.GetChildrenCount(tpExp1);
            if (count>0)
            {
                // We must delve into the VisualTree
                TextBlock txtExp = (TextBlock)FindElementById(tpExp1, "txtExperiment");
                if (txtExp != null)
                {
                    txtContent = txtExp.Text;
                }
            }
            else
            {
                txtContent = (new TextRange(tpExp1.Document.ContentStart, tpExp1.Document.ContentEnd)).Text;
            }

            Utilities.PrintStatus("2. TextBlock panel content.");
            Utilities.Assert(txtContent == "Experiment\r\n", "(experiment)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("StackPanel #2:");
            Utilities.PrintStatus("Verify control text panel state.");
            Utilities.PrintStatus("1. Blue text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpCtrl2.Foreground).Color == Colors.Blue, "");
            Utilities.PrintStatus("2. TextBlock panel content.");
            Utilities.Assert((new TextRange(tpCtrl2.Document.ContentStart, tpCtrl2.Document.ContentEnd)).Text == "Control\r\n", "(control)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental text panel state.");
            Utilities.PrintStatus("1. Blue text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpExp2.Foreground).Color == Colors.Blue, "");

            txtContent = "";
            count = VisualTreeHelper.GetChildrenCount(tpExp2);
            if (count>0)
            {
                // We must delve into the VisualTree
                TextBlock txtExp = (TextBlock)FindElementById(tpExp2, "txtExperiment");
                if (txtExp != null)
                {
                    txtContent = txtExp.Text;
                }
            }
            else
            {
                txtContent = (new TextRange(tpExp2.Document.ContentStart, tpExp2.Document.ContentEnd)).Text;
            }

            Utilities.PrintStatus("2. TextBlock panel content.");
            Utilities.Assert(txtContent == "Experiment\r\n", "(experiment)");
            Utilities.PrintStatus("");
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV03
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by control resource in descendent resource dictionary.
        /// </summary>
        private class ProgrammaticCV03 : Test
        {
            public ProgrammaticCV03() { }

            protected override void Setup()
            {
                Title = "Simple style, referenced by control resource in descendent resource dictionary.";

                StackPanel spRoot = new StackPanel();

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spRoot.Resources["tpStyle"] = tpStyle;

                StackPanel spChild = new StackPanel();
                spRoot.Children.Add(spChild);
                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                spChild.Resources["tp"] = tp;
                tp.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                Button btn = new Button();
                spChild.Children.Add(btn);
                btn.Content = _tpExp = (FlowDocumentScrollViewer)btn.FindResource("tp");
            }

            protected override void Validate()
            {
                CV01ValidateState(_tpCtrl, _tpExp);
            }

            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV03ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel spRoot = rootElem as StackPanel;
            Utilities.Assert(spRoot != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 1 child StackPanel present.");
            Utilities.Assert(spRoot.Children.Count == 1, "");
            StackPanel spChild = spRoot.Children[0] as StackPanel;
            Utilities.Assert(spChild != null, "Children[0] (child stack panel)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Child StackPanel:");
            Utilities.PrintStatus("3. 2 child elements present.");
            Utilities.Assert(spChild.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("4. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = spChild.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = spChild.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("5. Button contains a text panel.");
            FlowDocumentScrollViewer tpExp = btn.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            CV01ValidateState(tpCtrl, tpExp);
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV04
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by style visual tree.
        /// </summary>
        private class ProgrammaticCV04 : TestApplication
        {
            public ProgrammaticCV04() { }

            protected override void Setup(StackPanel spRoot)
            {
                Title = "Simple style, referenced by style visual tree.";

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spRoot.Resources["tpStyle"] = tpStyle;

                Style btnStyle = new Style(typeof(Button));
                FrameworkElementFactory txtFactory = new FrameworkElementFactory(typeof(TextBlock));
                txtFactory.SetValue(FrameworkElement.NameProperty, "txtExperiment");
                txtFactory.SetValue(TextBlock.TextProperty, "Experiment");
                FrameworkElementFactory tpFactory = new FrameworkElementFactory(typeof(FlowDocument));
                tpFactory.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");
                tpFactory.SetValue(FrameworkElement.NameProperty, "tpExperiment");
                tpFactory.AppendChild(txtFactory);
                FrameworkElementFactory btnFactory = new FrameworkElementFactory(typeof(Button));
                btnFactory.AppendChild(tpFactory);
                ControlTemplate template = new ControlTemplate(typeof(Button));
                template.VisualTree = btnFactory;
                btnStyle.Setters.Add(new Setter(Button.TemplateProperty, template));
                spRoot.Resources["btnStyle"] = btnStyle;

                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spRoot.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                _btn = new Button();
                spRoot.Children.Add(_btn);
                _btn.SetResourceReference(FrameworkElement.StyleProperty, "btnStyle");
            }

            protected override void Validate()
            {
                // Experimental TP cannot be obtained until displayed because it is part of the styled
                // visual tree.
                _tpExp = CV04ValidateTPExperimental(_btn);

                CV01ValidateState(_tpCtrl, _tpExp);
            }

            private Button _btn;
            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Obtain the experimental text panel from its enclosing button.
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        public static FlowDocumentScrollViewer CV04ValidateTPExperimental(Button btn)
        {
            Utilities.PrintStatus("Obtain text panel from button visual tree.");
            FlowDocumentScrollViewer tpExp = FindElementById(btn, "tpExperiment") as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "");
            Utilities.PrintStatus("");

            return tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV04ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = sp.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = sp.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            FlowDocumentScrollViewer tpExp = CV04ValidateTPExperimental(btn);

            CV01ValidateState(tpCtrl, tpExp);
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV05
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by style visual tree in ancestral resource dictionary.
        /// </summary>
        private class ProgrammaticCV05 : TestApplication
        {
            public ProgrammaticCV05() { }

            protected override void Setup(StackPanel spRoot)
            {
                Title = "Simple style, referenced by style visual tree in ancestral resource dictionary.";

                Style btnStyle = new Style(typeof(Button));

                FrameworkElementFactory txtFactory = new FrameworkElementFactory(typeof(TextBlock));
                txtFactory.SetValue(FrameworkElement.NameProperty, "txtExperiment");
                txtFactory.SetValue(TextBlock.TextProperty, "Experiment");
                FrameworkElementFactory tpFactory = new FrameworkElementFactory(typeof(FlowDocument));
                tpFactory.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");
                tpFactory.SetValue(FrameworkElement.NameProperty, "tpExperiment");
                tpFactory.AppendChild(txtFactory);
                FrameworkElementFactory btnFactory = new FrameworkElementFactory(typeof(Button));
                btnFactory.AppendChild(tpFactory);
                ControlTemplate template = new ControlTemplate(typeof(Button));
                template.VisualTree = btnFactory;
                btnStyle.Setters.Add(new Setter(Button.TemplateProperty, template));
                spRoot.Resources["btnStyle"] = btnStyle;

                // Child StackPanel #1

                StackPanel spChild1 = new StackPanel();
                spRoot.Children.Add(spChild1);

                Style style1 = new Style();
                style1.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spChild1.Resources["tpStyle"] = style1;

                // Control subject
                _tpCtrl1 = new FlowDocumentScrollViewer();
                _tpCtrl1.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild1.Children.Add(_tpCtrl1);
                _tpCtrl1.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");


                // Experimental subject
                _btn1 = new Button();
                spChild1.Children.Add(_btn1);
                _btn1.SetResourceReference(FrameworkElement.StyleProperty, "btnStyle");

                // Child StackPanel #2

                StackPanel spChild2 = new StackPanel();
                spRoot.Children.Add(spChild2);

                Style style2 = new Style();
                style2.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Blue));
                spChild2.Resources["tpStyle"] = style2;

                // Control subject
                _tpCtrl2 = new FlowDocumentScrollViewer();
                _tpCtrl2.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild2.Children.Add(_tpCtrl2);
                _tpCtrl2.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                _btn2 = new Button();
                spChild2.Children.Add(_btn2);
                _btn2.SetResourceReference(FrameworkElement.StyleProperty, "btnStyle");
            }

            protected override void Validate()
            {
                // Experimental TPs cannot be obtained until displayed because it is part of the styled
                // visual tree.
                FlowDocumentScrollViewer[] tpExps = CV05ValidateTPExperimental(_btn1, _btn2);
                _tpExp1 = tpExps[0];
                _tpExp2 = tpExps[1];

                CV02ValidateState(_tpCtrl1, _tpExp1, _tpCtrl2, _tpExp2);
            }

            private Button _btn1, _btn2;
            private FlowDocumentScrollViewer _tpCtrl1, _tpCtrl2;
            private FlowDocumentScrollViewer _tpExp1, _tpExp2;
        }

        /// <summary>
        /// Obtain the experimental text panels from their enclosing buttons.
        /// </summary>
        /// <param name="btn1"></param>
        /// <param name="btn2"></param>
        /// <returns></returns>
        public static FlowDocumentScrollViewer[] CV05ValidateTPExperimental(Button btn1, Button btn2)
        {
            Utilities.PrintStatus("Obtain text panels from button visual trees.");

            Utilities.PrintStatus("Button #1:");
            Utilities.PrintStatus("Obtain text panel from button visual tree.");
            FlowDocumentScrollViewer tpExp1 = FindElementById(btn1, "tpExperiment") as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp1 != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Button #2:");
            Utilities.PrintStatus("Obtain FlowDocumentScrollViewer from button visual tree.");
            FlowDocumentScrollViewer tpExp2 = FindElementById(btn2, "tpExperiment") as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp2 != null, "");
            Utilities.PrintStatus("");

            return new FlowDocumentScrollViewer[] { tpExp1, tpExp2 };
        }


        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV05ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel spRoot = rootElem as StackPanel;
            Utilities.Assert(spRoot != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 2 child StackPanel's present.");
            Utilities.Assert(spRoot.Children.Count == 2, "2 children");
            StackPanel spChild1 = spRoot.Children[0] as StackPanel;
            Utilities.Assert(spChild1 != null, "Children[0] (child stack panel 1)");
            StackPanel spChild2 = spRoot.Children[1] as StackPanel;
            Utilities.Assert(spChild2 != null, "Children[1] (child stack panel 2)");
            Utilities.PrintStatus("");

            // Child StackPanel #1

            Utilities.PrintStatus("StackPanel #1:");
            Utilities.PrintStatus("3. 2 child elements present.");
            Utilities.Assert(spChild1.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("4. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl1 = spChild1.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl1 != null, "Children[0] (control FlowDocumentScrollViewer)");
            Button btn1 = spChild1.Children[1] as Button;
            Utilities.Assert(btn1 != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            // Child StackPanel #2

            Utilities.PrintStatus("StackPanel #2:");
            Utilities.PrintStatus("5. 2 child elements present.");
            Utilities.Assert(spChild2.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("6. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl2 = spChild2.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl2 != null, "Children[0] (control text panel)");
            Button btn2 = spChild2.Children[1] as Button;
            Utilities.Assert(btn2 != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            FlowDocumentScrollViewer[] tpExps = CV05ValidateTPExperimental(btn1, btn2);
            FlowDocumentScrollViewer tpExp1 = tpExps[0];
            FlowDocumentScrollViewer tpExp2 = tpExps[1];

            CV02ValidateState(tpCtrl1, tpExp1, tpCtrl2, tpExp2);
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV06
        ******************************************************************************/
        /// <summary>
        /// Simple style, referenced by style visual tree in descendent resource dictionary.
        /// </summary>
        private class ProgrammaticCV06 : TestApplication
        {
            public ProgrammaticCV06() { }

            protected override void Setup(StackPanel spRoot)
            {
                Title = "Simple style, referenced by style visual tree in descendent resource dictionary.";

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spRoot.Resources["tpStyle"] = tpStyle;

                StackPanel spChild = new StackPanel();
                spRoot.Children.Add(spChild);

                Style btnStyle = new Style(typeof(Button));
                FrameworkElementFactory txtFactory = new FrameworkElementFactory(typeof(TextBlock));
                txtFactory.SetValue(FrameworkElement.NameProperty, "txtExperiment");
                txtFactory.SetValue(TextBlock.TextProperty, "Experiment");
                FrameworkElementFactory tpFactory = new FrameworkElementFactory(typeof(FlowDocument));
                tpFactory.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");
                tpFactory.SetValue(FrameworkElement.NameProperty, "tpExperiment");
                tpFactory.AppendChild(txtFactory);
                FrameworkElementFactory btnFactory = new FrameworkElementFactory(typeof(Button));
                btnFactory.AppendChild(tpFactory);
                ControlTemplate template = new ControlTemplate(typeof(Button));
                template.VisualTree = btnFactory;
                btnStyle.Setters.Add(new Setter(Button.TemplateProperty, btnFactory));
                spChild.Resources["btnStyle"] = btnStyle;

                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyle");

                // Experimental subject
                _btn = new Button();
                spChild.Children.Add(_btn);
                _btn.SetResourceReference(FrameworkElement.StyleProperty, "btnStyle");
            }

            protected override void Validate()
            {
                // Experimental TP cannot be obtained until displayed because it is part of the styled
                // visual tree.
                _tpExp = CV06ValidateTPExperimental(_btn);

                CV01ValidateState(_tpCtrl, _tpExp);
            }

            private Button _btn;
            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Obtain the experimental text panel from its enclosing button.
        /// </summary>
        /// <param name="btn"></param>
        /// <returns></returns>
        public static FlowDocumentScrollViewer CV06ValidateTPExperimental(Button btn)
        {
            Utilities.PrintStatus("Obtain text panel from button visual tree.");
            FlowDocumentScrollViewer tpExp = FindElementById(btn, "tpExperiment") as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "");
            Utilities.PrintStatus("");

            return tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV06ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 1 child StackPanel present.");
            Utilities.Assert(sp.Children.Count == 1, "(one child)");
            StackPanel spChild = sp.Children[0] as StackPanel;
            Utilities.Assert(spChild != null, "(child is StackPanel)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Child StackPanel:");
            Utilities.PrintStatus("3. 2 child elements present.");
            Utilities.Assert(spChild.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("4. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = spChild.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = spChild.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            FlowDocumentScrollViewer tpExp = CV06ValidateTPExperimental(btn);

            CV01ValidateState(tpCtrl, tpExp);
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV07
        ******************************************************************************/
        /// <summary>
        /// Simple style, extended, referenced by control resource.
        /// </summary>
        private class ProgrammaticCV07 : Test
        {
            public ProgrammaticCV07() { }

            protected override void Setup()
            {
                Title = "Simple style, extended, referenced by control resource.";

                StackPanel sp = new StackPanel();

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                sp.Resources["tpStyle"] = tpStyle;

                Style tpStyleExt = new Style(typeof(FlowDocument), tpStyle);
                tpStyleExt.Setters.Add(new Setter(FlowDocument.BackgroundProperty, Brushes.Blue));
                sp.Resources["tpStyleExtension"] = tpStyleExt;
                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                tp.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyleExtension");
                sp.Resources["tp"] = tp;

                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                sp.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyleExtension");

                // Experimental subject
                Button btn = new Button();
                sp.Children.Add(btn);
                btn.Content = _tpExp = (FlowDocumentScrollViewer)btn.FindResource("tp");
            }

            protected override void Validate()
            {
                CV07ValidateState(_tpCtrl, _tpExp);
            }

            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV07ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = sp.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = sp.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. Button contains a text panel.");
            FlowDocumentScrollViewer tpExp = btn.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            CV07ValidateState(tpCtrl, tpExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV0[34]ValidateAll.
        /// </remarks>
        /// <param name="tpCtrl">Control text panel.</param>
        /// <param name="tpExp">Experimental text panel.</param>
        public static void CV07ValidateState(FlowDocumentScrollViewer tpCtrl, FlowDocumentScrollViewer tpExp)
        {
            Utilities.PrintStatus("Verify control text panel state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpCtrl.Document.Foreground).Color == Colors.Red, "");
            Utilities.PrintStatus("2. Blue text panel background.");
            Utilities.Assert(((SolidColorBrush)tpCtrl.Document.Background).Color == Colors.Blue, "");
            Utilities.PrintStatus("3. TextBlock panel content.");
            Utilities.Assert((new TextRange(tpCtrl.Document.ContentStart, tpCtrl.Document.ContentEnd)).Text == "Control", "(control)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental text panel state.");
            Utilities.PrintStatus("1. Red text panel foreground.");
            Utilities.Assert(((SolidColorBrush)tpExp.Document.Foreground).Color == Colors.Red, "");
            Utilities.PrintStatus("2. Blue text panel background.");
            Utilities.Assert(((SolidColorBrush)tpExp.Document.Background).Color == Colors.Blue, "");
            Utilities.PrintStatus("");
            Utilities.PrintStatus("3. TextBlock panel content.");
            Utilities.Assert((new TextRange(tpExp.Document.ContentStart, tpExp.Document.ContentEnd)).Text == "Experiment", "(experiment)");
        }

        /******************************************************************************
        * CLASS:          ProgrammaticCV08
        ******************************************************************************/
        /// <summary>
        /// Simple style, extended in descendent resource dictionary, referenced by control resource in descendent resource dictionary.
        /// </summary>
        private class ProgrammaticCV08 : Test
        {
            public ProgrammaticCV08() { }

            protected override void Setup()
            {
                Title = "Simple style, extended in descendent resource dictionary, referenced by control resource in descendent resource dictionary.";

                StackPanel spRoot = new StackPanel();

                Style tpStyle = new Style(typeof(FlowDocument));
                tpStyle.Setters.Add(new Setter(FlowDocument.ForegroundProperty, Brushes.Red));
                spRoot.Resources["tpStyle"] = tpStyle;

                StackPanel spChild = new StackPanel();
                spRoot.Children.Add(spChild);

                Style tpStyleExt = new Style(typeof(FlowDocument), (Style)spChild.FindResource("tpStyle"));
                tpStyleExt.Setters.Add(new Setter(FlowDocument.BackgroundProperty, Brushes.Blue));
                spChild.Resources["tpStyleExtension"] = tpStyleExt;
                FlowDocumentScrollViewer tp = new FlowDocumentScrollViewer();
                tp.Document = new FlowDocument(new Paragraph(new Run("Experiment")));
                tp.SetResourceReference(FrameworkElement.StyleProperty, "tpStyleExtension");
                spChild.Resources["tp"] = tp;

                // Control subject
                _tpCtrl = new FlowDocumentScrollViewer();
                _tpCtrl.Document = new FlowDocument(new Paragraph(new Run("Control")));
                spChild.Children.Add(_tpCtrl);
                _tpCtrl.Document.SetResourceReference(FrameworkElement.StyleProperty, "tpStyleExtension");

                // Experimental subject
                Button btn = new Button();
                spChild.Children.Add(btn);
                btn.Content = _tpExp = (FlowDocumentScrollViewer)btn.FindResource("tp");
            }

            protected override void Validate()
            {
                CV07ValidateState(_tpCtrl, _tpExp);
            }

            private FlowDocumentScrollViewer _tpCtrl;
            private FlowDocumentScrollViewer _tpExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV08ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("2. 1 child StackPanel present.");
            Utilities.Assert(sp.Children.Count == 1, "(one child)");
            StackPanel spChild = sp.Children[0] as StackPanel;
            Utilities.Assert(spChild != null, "(child is StackPanel)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Child StackPanel:");
            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(spChild.Children.Count == 2, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. 1 FlowDocumentScrollViewer and 1 Button.");
            FlowDocumentScrollViewer tpCtrl = spChild.Children[0] as FlowDocumentScrollViewer;
            Utilities.Assert(tpCtrl != null, "Children[0] (control text panel)");
            Button btn = spChild.Children[1] as Button;
            Utilities.Assert(btn != null, "Children[1] (button)");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("3. Button contains a text panel.");
            FlowDocumentScrollViewer tpExp = btn.Content as FlowDocumentScrollViewer;
            Utilities.Assert(tpExp != null, "Button.Content (experimental text panel)");
            Utilities.PrintStatus("");

            CV07ValidateState(tpCtrl, tpExp);
        }
        #endregion
    }
}

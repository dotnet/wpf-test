// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshFrameworkElementFactoryTest
{
    /******************************************************************************
    * CLASS:          FrameworkElementFactoryTest
    ******************************************************************************/
    [Test(0, "PropertyEngine.FrameworkElementFactory", TestCaseSecurityLevel.FullTrust, "FrameworkElementFactoryTest")]
    public class FrameworkElementFactoryTest : TestCase
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("TestMisc")]
        [Variation("TestConstructor")]
        [Variation("TestType")]
        [Variation("TestId")]
        [Variation("TestSetBinding")]
        [Variation("TestSetValue")]
        [Variation("TestAppendChild")]
        [Variation("TestSetResourceReference")]
        [Variation("TestEventHandlers")]

        /******************************************************************************
        * Function:          FrameworkElementFactoryTest Constructor
        ******************************************************************************/
        public FrameworkElementFactoryTest(string arg)
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
            TestFrameworkElementFactory test = new TestFrameworkElementFactory();

            switch (_testName)
            {
                case "TestMisc":
                    test.TestMisc();
                    break;
                case "TestConstructor":
                    test.TestConstructor();
                    break;
                case "TestType":
                    test.TestType();
                    break;
                case "TestId":
                    test.TestId();
                    break;
                case "TestSetBinding":
                    test.TestSetBinding();
                    break;
                case "TestSetValue":
                    test.TestSetValue();
                    break;
                case "TestAppendChild":
                    test.TestAppendChild();
                    break;
                case "TestSetResourceReference":
                    test.TestSetResourceReference();
                    break;
                case "TestEventHandlers":
                    test.TestEventHandlers();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by Asserts or Exceptions.
            return TestResult.Pass;
        }
        #endregion
    }


    /******************************************************************************
    * CLASS:          TestFrameworkElementFactory
    ******************************************************************************/
    public class TestFrameworkElementFactory
    {
        /******************************************************************************
        * Function:          TestMisc
        ******************************************************************************/
        /// <summary>
        /// Test FrameworkELementFactory type
        /// </summary>
        public void TestMisc()
        {
            Utilities.PrintTitle("Test FrameworkELementFactory type");

            Type typeFef = typeof(FrameworkElementFactory);

            Utilities.PrintStatus("FrameworkElementFactory derives directly from object");
            Utilities.Assert(typeFef.BaseType == typeof(object), "typeFef.BaseType == typeof(object)");
        }

        /******************************************************************************
        * Function:          TestConstructor
        ******************************************************************************/
        /// <summary>
        /// Test Constructors
        /// </summary>
        public void TestConstructor()
        {
            Utilities.PrintTitle("Test Constructors. Two overloads.");
            Utilities.PrintStatus("Overload 1");

            FrameworkElementFactory rectangle1 = new FrameworkElementFactory(typeof(Rectangle));
            FrameworkElementFactory rectangle2 = new FrameworkElementFactory((Type)null);

            Utilities.PrintStatus("Overload 2");

            FrameworkElementFactory rectangle3 = new FrameworkElementFactory(typeof(Rectangle), "rectangle3");
            FrameworkElementFactory rectangle4 = new FrameworkElementFactory(typeof(Rectangle), "rectangle3");
            FrameworkElementFactory rectangle5 = new FrameworkElementFactory(typeof(Rectangle), null);
            FrameworkElementFactory rectangle6 = new FrameworkElementFactory(null, "rectangle6");
            FrameworkElementFactory rectangle7 = new FrameworkElementFactory("rectangle7");

            //From White box, next code is the same as rectangle2
            FrameworkElementFactory rectangle8 = new FrameworkElementFactory(null, null);
        }

        /******************************************************************************
        * Function:          TestType
        ******************************************************************************/
        /// <summary>
        /// Test 'Type' property, Also: IsSealed Property
        /// </summary>
        public void TestType()
        {
            Utilities.PrintTitle("Test 'Type' property and 'IsSealed' Property");

            FrameworkElementFactory rectangle1 = new FrameworkElementFactory((Type)null);

            Utilities.Assert(rectangle1.Type == null, "rectangle1.Type == null");
            rectangle1.Type = typeof(System.Windows.Shapes.Polygon);
            Utilities.Assert(rectangle1.Type == typeof(Polygon), "rectangle1.Type == typeof(Polygon)");

            FrameworkElementFactory rectangle2 = new FrameworkElementFactory(typeof(Polyline));

            Utilities.Assert(rectangle2.Type == typeof(Polyline), "rectangle2.Type == typeof(Polyline)");
            rectangle2.Type = null;
            Utilities.Assert(rectangle2.Type == null, "rectangle2.Type == null");
            Utilities.Assert(rectangle1.IsSealed == false, "rectangle1.IsSealed == false");
            Utilities.PrintStatus("Negative Case: Cannot set Type when FrameworkElementFactory is sealed");

            Style style = new Style(typeof(Button));

            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = rectangle1;
            style.Setters.Add(new Setter(Button.TemplateProperty, template));

            CreateObject1(style);

            try
            {
                rectangle1.Type = typeof(System.Windows.Shapes.Polygon);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.Assert(rectangle1.IsSealed, "rectangle1.IsSealed");
        }

        private object CreateObject1(object val)
        {
            Style style = (Style)val;

            System.Diagnostics.Debug.Assert(style != null, "Need to pass style");

            Button fe = new Button();

            fe.Style = style;
            fe.ApplyTemplate();
            return fe;
        }


        /******************************************************************************
        * Function:          TestId
        ******************************************************************************/
        /// <summary>
        /// Test property Name. Its type is string. It is Read/Write
        /// Also test: IsSealed property
        /// </summary>
        public void TestId()
        {
            Utilities.PrintTitle("Test property 'Name'. Also Test property 'IsSealed'");

            FrameworkElementFactory rectange1 = new FrameworkElementFactory(typeof(Rectangle), "rect1");

            Utilities.Assert(rectange1.Name == "rect1", "rectange1.Name == \"rect1\"");
          


            try
            {
                rectange1.Name = "";
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            rectange1.Name = null;
            Utilities.Assert(rectange1.Name == null, "rectange1.Name == null");
            Utilities.Assert(rectange1.IsSealed == false, "rectange1.IsSealed == false");
        }

        private object CreateObject2(object arg)
        {
            Style style = (Style)arg;

            System.Diagnostics.Debug.Assert(style != null, "Needs Style");

            FrameworkContentElement fce = new FrameworkContentElement();

            fce.Style = style;
            return fce;
        }

        /******************************************************************************
        * Function:          TestAppendChild
        ******************************************************************************/
        /// <summary>
        /// Test method AppendChild (Add a factory child to this factory)
        /// Also test property Parent, FirstChild and NextSibling
        /// </summary>
        public void TestAppendChild()
        {
            Utilities.PrintTitle("Test method AppendChild. Also property Parent, FirstChild & NextSibling ");

            /*  Construct the following Data Structure (After rotate 90 degree)
            Rectangle1-----Rectangle21
                        |--Rectangle22-----Rectangle31
                        |--Rectangle23  |--Rectangle32
                                        |--Rectangle33
                                        |--Rectangle34
            */
            Utilities.PrintStatus("Positive Case");

            FrameworkElementFactory rectangle1 = new FrameworkElementFactory(typeof(Button), "Rectangle1");
            FrameworkElementFactory rectangle21 = new FrameworkElementFactory(typeof(Button), "Rectangle21");
            FrameworkElementFactory rectangle22 = new FrameworkElementFactory(typeof(Button), "Rectangle22");
            FrameworkElementFactory rectangle23 = new FrameworkElementFactory(typeof(Button), "Rectangle23");
            FrameworkElementFactory rectangle31 = new FrameworkElementFactory(typeof(Button), "Rectangle31");
            FrameworkElementFactory rectangle32 = new FrameworkElementFactory(typeof(Button), "Rectangle32");
            FrameworkElementFactory rectangle33 = new FrameworkElementFactory(typeof(Button), "Rectangle33");
            FrameworkElementFactory rectangle34 = new FrameworkElementFactory(typeof(Button), "Rectangle34");

            rectangle22.AppendChild(rectangle31);
            rectangle22.AppendChild(rectangle32);
            rectangle22.AppendChild(rectangle33);
            rectangle22.AppendChild(rectangle34);
            rectangle1.AppendChild(rectangle21);
            rectangle1.AppendChild(rectangle22);
            rectangle1.AppendChild(rectangle23);

            //Also Test 'Parent' read-only property
            Utilities.Assert(rectangle31.Parent == rectangle22, "rectangle31.Parent == rectangle22");
            Utilities.Assert(rectangle22.Parent == rectangle1, "rectangle22.Parent == rectangle1");
            Utilities.Assert(rectangle1.Parent == null, "rectangle1.Parent == null");

            //Also Test 'FirstChild' Read-Only property
            Utilities.Assert(rectangle21.FirstChild == null, "rectangle21.FirstChild == null");
            Utilities.Assert(rectangle22.FirstChild == rectangle31, "rectangle22.FirstChild == rectangle31");
            Utilities.Assert(rectangle1.FirstChild == rectangle21, "rectangle1.FirstChild == rectangle21");

            //Also Test 'NextSibling' Read-Only property
            Utilities.Assert(rectangle1.NextSibling == null, "rectangle1.NextSibling == null");
            Utilities.Assert(rectangle22.NextSibling == rectangle23, "rectangle22.NextSibling == rectangle23");
            Utilities.Assert(rectangle33.NextSibling == rectangle34, "rectangle33.NextSibling == rectangle34");
            Utilities.Assert(rectangle34.NextSibling == null, "rectangle34.NextSibling == null");
            Utilities.PrintStatus("Negative case 1: Child Already Parented");
            try
            {
                rectangle1.AppendChild(rectangle34);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative case 2: Child is null");
            try
            {
                rectangle1.AppendChild(null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative case 3: parent is sealed.");

            Style style = new Style();

            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = rectangle1;
            style.Setters.Add(new Setter(Button.TemplateProperty, template));

            CreateObject3(style);
            try
            {
                rectangle1.AppendChild(new FrameworkElementFactory(typeof(Rectangle), "RectCannotBeAddedNow"));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        private object CreateObject3(object arg)
        {
            Style style = (Style)arg;

            System.Diagnostics.Debug.Assert(style != null, "Needs Style");

            Button frameworkElement = new Button();

            //This will Seal Style
            frameworkElement.Style = style;
            return frameworkElement;
        }

        /******************************************************************************
        * Function:          TestSetValue
        ******************************************************************************/
        /// <summary>
        /// Test 'SetValue' method
        /// </summary>
        public void TestSetValue()
        {
            Utilities.PrintTitle("Test 'SetValue' method");
            Utilities.PrintStatus("Positive Test Cases");

            FrameworkElementFactory rectangle = new FrameworkElementFactory(typeof(Rectangle), "Rectangle");

            //non-freezable value
            rectangle.SetValue(DockPanel.DockProperty, Dock.Right);

            //Freezable value
            SolidColorBrush myBrush = new SolidColorBrush(Colors.Blue);

            rectangle.SetValue(Control.ForegroundProperty, myBrush);  //step to ensure code coverage

            //Set duplicate value
            rectangle.SetValue(DockPanel.DockProperty, Dock.Right);

            //Set Different Value
            rectangle.SetValue(DockPanel.DockProperty, Dock.Bottom);
            Utilities.PrintStatus("Negative Case 1: value invalid");

            //DockPanel.DockProperty does not validate property value.


            try
            {
                rectangle.SetValue(CheckBox.IsCheckedProperty, 99);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative case 2: dp is null");
            try
            {
                rectangle.SetValue(null, Dock.Top);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative Case 3: FrameworkElementFactory is sealed");


            Style style = new Style();
            ControlTemplate template = new ControlTemplate(typeof(Button));
            template.VisualTree = rectangle;
            style.Setters.Add(new Setter(Button.TemplateProperty, template));

            FrameworkElement fce = (FrameworkElement)CreateObject4(style);


            try
            {
                rectangle.SetValue(DockPanel.LastChildFillProperty, true);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        private object CreateObject4(object arg)
        {
            Style style = (Style)arg;

            System.Diagnostics.Debug.Assert(style != null, "Need Style");

            Button fce = new Button();

            fce.Style = style;
            fce.ApplyTemplate();
            return fce;
        }

        /******************************************************************************
        * Function:          TestEventHandlers
        ******************************************************************************/
        /// <summary>
        /// Test event handlers.
        /// </summary>
        public void TestEventHandlers()
        {
            Utilities.PrintTitle("Test event handlers.");
            Utilities.PrintStatus("Positive Case");

            // Create Loaded handler.
            int loadedHandlerCount = 0;
            Delegate handler = 
                (RoutedEventHandler)delegate(object sender, RoutedEventArgs args)
                {
                    loadedHandlerCount++;
                };

            // Test AddHandler twice -- second time call RemoveHandler.
            for (int i = 0; i < 2; i++)
            {
                Window window = new Window();

                // Build FrameworkElementFactory with event handler
                // Set it as template on Window.
                FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Canvas));

                fef.AddHandler(FrameworkElement.LoadedEvent, handler, true);
                fef.AddHandler(FrameworkElement.LoadedEvent, handler);

                if (i == 0)
                {
                    fef.RemoveHandler(FrameworkElement.LoadedEvent, handler);
                }

                ControlTemplate template = new ControlTemplate();
                template.VisualTree = fef;
                window.Template = template;
                
                // Open and close window -- inflates template.
                loadedHandlerCount = 0;
                window.Show();
                DispatcherHelper.DoEvents();
                window.Close();

                // Verify reference is found, effective value is correct.
                int expectedCount = i + 1;
                Utilities.Assert(loadedHandlerCount == expectedCount, "loadedHandlerCount == " + expectedCount);
            }
        }

        /******************************************************************************
        * Function:          TestSetResourceReference
        ******************************************************************************/
        /// <summary>
        /// Test 'SetResourceReference' method.
        /// </summary>
        public void TestSetResourceReference()
        {
            Utilities.PrintTitle("Test 'SetResourceReference' method.");
            Utilities.PrintStatus("Positive Case");

            // Build style and add to Control resources
            Style style = new Style();
            style.TargetType = typeof(Canvas);
            Setter setter = new Setter();
            setter.Property = Panel.BackgroundProperty;
            setter.Value = Brushes.Red;
            style.Setters.Add(setter);

            Control control = new Button();
            control.Resources.Add("resKey", style);

            // Build FrameworkElementFactory with resource reference.
            // Set it as template on Control.
            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Canvas));

            fef.SetResourceReference(FrameworkElement.StyleProperty, "resKey");

            ControlTemplate template = new ControlTemplate();
            template.VisualTree = fef;
            control.Template = template;

            control.ApplyTemplate();

            Canvas canvas = (Canvas)VisualTreeUtils.GetChild(control, 0);

            // Verify reference is found, effective value is correct.
            Utilities.Assert(canvas.Background == Brushes.Red, "canvas.Background == Brushes.Red");
        }

        /******************************************************************************
        * Function:          TestSetBinding
        ******************************************************************************/
        /// <summary>
        /// Test 'SetBinding' method.
        /// </summary>
        public void TestSetBinding()
        {
            Utilities.PrintTitle("Test 'SetBinding' method.");
            Utilities.PrintStatus("Positive Case");


            System.Windows.Data.Binding myBind = null;
            FrameworkElementFactory rectangle = null;

            myBind = new System.Windows.Data.Binding();
            myBind.XPath = "/Customer/Order[@OrderID=10]/Amount";
            rectangle = new FrameworkElementFactory(typeof(Rectangle), "rectangle");

            rectangle.SetBinding(DockPanel.DockProperty, myBind);
            Utilities.PrintStatus("Negative Test Scenario 1: bind is null");
            try
            {
                rectangle.SetBinding(DockPanel.DockProperty, null);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentException ex)  // causes ArgumentNullException to be ArgumentException
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative Test Scenario 2: dp is null");
            try
            {
                rectangle.SetBinding(null, myBind);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

            Utilities.PrintStatus("Negative Test Scenario 3: FrameworkElementFactory is sealed");


            Style style = new Style();
            ControlTemplate template = new ControlTemplate(typeof(MenuItem));
            template.VisualTree = rectangle;
            style.Setters.Add(new Setter(MenuItem.TemplateProperty, template));

            MenuItem mi = (MenuItem)CreateObject5(style);

            try
            {
                rectangle.SetBinding(MenuItem.AllowDropProperty, myBind);
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (InvalidOperationException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
        }

        private object CreateObject5(object arg)
        {
            Style style = (Style)arg;

            System.Diagnostics.Debug.Assert(style != null, "Need Style");

            MenuItem mi = new MenuItem();

            mi.Style = style;
            return mi;
        }

        /******************************************************************************
        * Function:          TestAliasProperty
        ******************************************************************************/
        /// <summary>
        /// Test 'AliasProperty' method
        /// </summary>
        public void TestAliasProperty()
        {
            Utilities.PrintTitle("Test 'AliasProperty' method");
            Utilities.PrintStatus("Positive Cases");

            FrameworkElementFactory fef = new FrameworkElementFactory(typeof(Button));

            fef.SetValue(DockPanel.DockProperty, new TemplateBindingExtension(DockPanel.DockProperty));
            fef.SetValue(Rectangle.FillProperty, new TemplateBindingExtension(Button.BackgroundProperty));

            //Curretnly there is no checking about two properties
            fef.SetValue(MenuItem.IsCheckedProperty, new TemplateBindingExtension(Button.ClickModeProperty));
            Utilities.PrintStatus("Negaive Test Case Scenario 1: sourceProperty is null");
            try
            {
                fef.SetValue(MenuItem.BorderThicknessProperty, new TemplateBindingExtension(null));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }
            Utilities.PrintStatus("Negative test scenario 2: destinationProperty is null");
            try
            {
                fef.SetValue(null, new TemplateBindingExtension(MenuItem.BorderBrushProperty));
                Utilities.ExpectedExceptionNotReceived();
            }
            catch (ArgumentNullException ex)
            {
                Utilities.ExpectedExceptionReceived(ex);
            }

        }

        private object CreateObject6(object arg)
        {
            Style style = (Style)arg;

            System.Diagnostics.Debug.Assert(style != null, "Need Style");

            System.Windows.Documents.Paragraph para = new System.Windows.Documents.Paragraph();

            //Seal it
            para.Style = style;
            return para;
        }
    }
}

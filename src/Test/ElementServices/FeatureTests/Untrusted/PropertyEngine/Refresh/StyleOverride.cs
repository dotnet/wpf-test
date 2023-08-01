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


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshStyleOverrideTest
{
    /******************************************************************************
    * CLASS:          StyleOverride
    ******************************************************************************/
    [Test(0, "PropertyEngine.Style", TestCaseSecurityLevel.FullTrust, "StyleOverride")]
    public class StyleOverride : AvalonTest
    {
        #region Private Data
        private string _testName = "";
        #endregion

        #region Constructor

        [Variation("StyleOverride01")]
        [Variation("StyleOverride02")]
        [Variation("StyleOverride03")]
        [Variation("StyleOverride04")]
        [Variation("StyleOverride05")]
        [Variation("StyleOverride06")]
        [Variation("StyleOverride07")]
        [Variation("StyleOverride08")]

        /******************************************************************************
        * Function:          StyleOverride Constructor
        ******************************************************************************/
        public StyleOverride(string arg)
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
                case "StyleOverride01":
                    new StyleOverride01().Run();
                    break;
                case "StyleOverride02":
                    new StyleOverride02().Run();
                    break;
                case "StyleOverride03":
                    new StyleOverride03().Run();
                    break;
                case "StyleOverride04":
                    new StyleOverride04().Run();
                    break;
                case "StyleOverride05":
                    new StyleOverride05().Run();
                    break;
                case "StyleOverride06":
                    new StyleOverride06().Run();
                    break;
                case "StyleOverride07":
                    new StyleOverride07().Run();
                    break;
                case "StyleOverride08":
                    new StyleOverride08().Run();
                    break;
                default:
                    throw new Microsoft.Test.TestValidationException("ERROR!!! Test case not found.");
            }

            //Any test failures will be caught by Asserts.
            return TestResult.Pass;
        }
        #endregion


        #region Private Classes
        /******************************************************************************
        * CLASS:          StyleOverride01
        ******************************************************************************/
        /// <summary>
        /// Anonymous style, override explicit style property.
        /// </summary>
        private class StyleOverride01 : Test
        {
            public StyleOverride01() { }

            protected override void Setup()
            {
                Title = "Anonymous style, override explicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();
                sp.Resources[typeof(Button)] = style;

                // Control.
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                sp.Children.Add(_btnCtrl);

                // Experiment.
                _btnExp = new Button();
                _btnExp.Focusable = false;
                _btnExp.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                CV01ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private Button _btnExp;
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

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. All child elements are buttons.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as Button;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            CV01ValidateState(btnCtrl, btnExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV05ValidateAll.
        /// </remarks>
        /// <param name="btnCtrl">Test control button.</param>
        /// <param name="btnExp">Test experiment button.</param>
        public static void CV01ValidateState(Button btnCtrl, Button btnExp)
        {
            Utilities.PrintStatus("Verify control button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnCtrl.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental button state.");
            Utilities.PrintStatus("1. Yellow button background.");
            Utilities.Assert(((SolidColorBrush)btnExp.Background).Color == Colors.Yellow, "");
            Utilities.PrintStatus("");
        }

        /******************************************************************************
        * CLASS:          StyleOverride02
        ******************************************************************************/
        /// <summary>
        /// Named style, override explicit style property.
        /// </summary>
        private class StyleOverride02 : Test
        {
            public StyleOverride02() { }

            protected override void Setup()
            {
                Title = "Named style, override explicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                _btnCtrl.Style = style;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new Button();
                _btnExp.Focusable = false;
                _btnExp.Style = style;
                _btnExp.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                CV02ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private Button _btnExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV02ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. All child elements are buttons.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as Button;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            CV02ValidateState(btnCtrl, btnExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV06ValidateAll.
        /// </remarks>
        /// <param name="btnCtrl">Test control button.</param>
        /// <param name="btnExp">Test experiment button.</param>
        private static void CV02ValidateState(Button btnCtrl, Button btnExp)
        {
            Utilities.PrintStatus("Verify control button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnCtrl.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental button state.");
            Utilities.PrintStatus("1. Yellow button background.");
            Utilities.Assert(((SolidColorBrush)btnExp.Background).Color == Colors.Yellow, "");
            Utilities.PrintStatus("");
        }

        /******************************************************************************
        * CLASS:          StyleOverride03
        ******************************************************************************/
        /// <summary>
        /// Anonymous style, override implicit style property.
        /// </summary>
        private class StyleOverride03 : Test
        {
            public StyleOverride03() { }

            protected override void Setup()
            {
                Title = "Anonymous style, override implicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();
                sp.Resources[typeof(Button)] = style;

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new Button();
                _btnExp.Focusable = false;
                _btnExp.Width = 100;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                CV03ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private Button _btnExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV03ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. All child elements are buttons.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as Button;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            CV03ValidateState(btnCtrl, btnExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV07ValidateAll.
        /// </remarks>
        /// <param name="btnCtrl">Test control button.</param>
        /// <param name="btnExp">Test experiment button.</param>
        public static void CV03ValidateState(Button btnCtrl, Button btnExp)
        {
            Utilities.PrintStatus("Verify control button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnCtrl.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnExp.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("2. 100px button width.");
            Utilities.Assert(btnExp.Width == 100, "");
            Utilities.PrintStatus("");
        }

        /******************************************************************************
        * CLASS:          StyleOverride04
        ******************************************************************************/
        /// <summary>
        /// Named style, override implicit style property.
        /// </summary>
        private class StyleOverride04 : Test
        {
            public StyleOverride04() { }

            protected override void Setup()
            {
                Title = "Named style, override implicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                _btnCtrl.Style = style;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new Button();
                _btnExp.Focusable = false;
                _btnExp.Style = style;
                _btnExp.Width = 100;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                CV04ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private Button _btnExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV04ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. All child elements are buttons.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as Button;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            CV04ValidateState(btnCtrl, btnExp);
        }

        /// <summary>
        /// Validate test state.
        /// </summary>
        /// <remarks>
        /// This validation is currently also used by CV08ValidateAll.
        /// </remarks>
        /// <param name="btnCtrl">Test control button.</param>
        /// <param name="btnExp">Test experiment button.</param>
        public static void CV04ValidateState(Button btnCtrl, Button btnExp)
        {
            Utilities.PrintStatus("Verify control button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnCtrl.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("");

            Utilities.PrintStatus("Verify experimental button state.");
            Utilities.PrintStatus("1. Red button background.");
            Utilities.Assert(((SolidColorBrush)btnExp.Background).Color == Colors.Red, "");
            Utilities.PrintStatus("2. 100px button width.");
            Utilities.Assert(btnExp.Width == 100, "");
            Utilities.PrintStatus("");
        }

        //
        // Derived Tests

        /******************************************************************************
        * CLASS:          StyleOverride05
        ******************************************************************************/
        /// <summary>
        /// BasedOn, anonymous style, override explicit property.
        /// </summary>
        private class StyleOverride05 : Test
        {
            public StyleOverride05() { }

            protected override void Setup()
            {
                Title = "BasedOn, anonymous style, override explicit property.";

                StackPanel sp = new StackPanel();

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));
                sp.Resources[typeof(Button)] = baseStyle;

                Style derivedStyle = new Style(typeof(FooButton), (Style)sp.Resources[typeof(Button)]);
                sp.Resources[typeof(FooButton)] = derivedStyle;

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new FooButton();
                _btnExp.Focusable = false;
                _btnExp.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV01
                CV01ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private FooButton _btnExp;
        }

        /// <summary>
        /// Validate application structure and test state.
        /// </summary>
        /// <param name="rootElem">Root element of application.</param>
        public static void CV05ValidateAll(UIElement rootElem)
        {
            Utilities.PrintStatus("Verify application structure.");

            Utilities.PrintStatus("1. Root element is a StackPanel.");
            StackPanel sp = rootElem as StackPanel;
            Utilities.Assert(sp != null, "");

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. 1 Button and 1 FooButton.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as FooButton;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            // Validate an identical state to CV01
            CV01ValidateState(btnCtrl, btnExp);
        }

        /******************************************************************************
        * CLASS:          StyleOverride06
        ******************************************************************************/
        /// <summary>
        /// BasedOn, named style, override implicit property.
        /// </summary>
        private class StyleOverride06 : Test
        {
            public StyleOverride06() { }

            protected override void Setup()
            {
                Title = "BasedOn, named style, override implicit property.";

                StackPanel sp = new StackPanel();

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                Style derivedStyle = new Style(typeof(FooButton), baseStyle);

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                _btnCtrl.Style = baseStyle;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new FooButton();
                _btnExp.Focusable = false;
                _btnExp.Style = derivedStyle;
                _btnExp.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV02
                CV02ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private FooButton _btnExp;
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

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. 1 Button and 1 FooButton.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as FooButton;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            // Validate an identical state to CV02
            CV02ValidateState(btnCtrl, btnExp);
        }

        /******************************************************************************
        * CLASS:          StyleOverride07
        ******************************************************************************/
        /// <summary>
        /// BasedOn, anonymous style, override explicit property.
        /// </summary>
        private class StyleOverride07 : Test
        {
            public StyleOverride07() { }

            protected override void Setup()
            {
                Title = "BasedOn, anonymous style, override explicit property.";

                StackPanel sp = new StackPanel();

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));
                sp.Resources[typeof(Button)] = baseStyle;

                Style derivedStyle = new Style(typeof(FooButton), (Style)sp.Resources[typeof(Button)]);
                sp.Resources[typeof(FooButton)] = derivedStyle;

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new FooButton();
                _btnExp.Focusable = false;
                _btnExp.Width = 100;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV03
                CV03ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private FooButton _btnExp;
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

            Utilities.PrintStatus("3. 1 Button and 1 FooButton.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as FooButton;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            // Validate an identical state to CV03
            CV03ValidateState(btnCtrl, btnExp);
        }

        /******************************************************************************
        * CLASS:          StyleOverride08
        ******************************************************************************/
        /// <summary>
        /// BasedOn, named style, override implicit property.
        /// </summary>
        private class StyleOverride08 : Test
        {
            public StyleOverride08() { }

            protected override void Setup()
            {
                Title = "BasedOn, named style, override implicit property.";

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                Style derivedStyle = new Style(typeof(FooButton), baseStyle);

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl = new Button();
                _btnCtrl.Focusable = false;
                _btnCtrl.Style = baseStyle;
                sp.Children.Add(_btnCtrl);

                // Experimental subject
                _btnExp = new FooButton();
                _btnExp.Focusable = false;
                _btnExp.Style = derivedStyle;
                _btnExp.Width = 100;
                sp.Children.Add(_btnExp);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV04
                CV04ValidateState(_btnCtrl, _btnExp);
            }

            private Button _btnCtrl;
            private FooButton _btnExp;
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

            Utilities.PrintStatus("2. 2 child elements present.");
            Utilities.Assert(sp.Children.Count == 2, "");

            Utilities.PrintStatus("3. 1 Button and 1 FooButton.");
            Button btnCtrl = sp.Children[0] as Button;
            Utilities.Assert(btnCtrl != null, "Children[1] (control button)");
            Button btnExp = sp.Children[1] as FooButton;
            Utilities.Assert(btnExp != null, "Children[2] (experiment button)");

            Utilities.PrintStatus("");

            // Validate an identical state to CV04
            CV04ValidateState(btnCtrl, btnExp);
        }
        #endregion
    }

    /// <summary>
    /// Subclass used to validate use of Style.BasedOn="{*typeof(...)}"
    /// </summary>
    public class FooButton : Button { }
}


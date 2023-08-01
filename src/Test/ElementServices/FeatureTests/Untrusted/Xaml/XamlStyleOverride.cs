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
using System.Windows.Threading;

using Avalon.Test.CoreUI;
using Avalon.Test.CoreUI.Common;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI.PropertyEngine;
using Avalon.Test.CoreUI.UtilityHelper;
using Microsoft.Test;


namespace Avalon.Test.CoreUI.PropertyEngine.RefreshStyleOverrideTest
{

    /// <summary>
    /// Subclass used to validate use of Style.BasedOn="{*typeof(...)}"
    /// </summary>
    public class FooButton : Button { }

    /// <summary>
    /// Style Override
    /// </summary>
    public class StyleOverride
    {
        /// <summary>
        /// Perform all correctness validation tests.
        /// </summary>
        public static void CorrectnessValidation()
        {
            new ProgrammaticCV01().Run();
            new ProgrammaticCV02().Run();
            new ProgrammaticCV03().Run();
            new ProgrammaticCV04().Run();
            new ProgrammaticCV05().Run();
            new ProgrammaticCV06().Run();
            new ProgrammaticCV07().Run();
            new ProgrammaticCV08().Run();
        }

        //
        // Override Tests

        /// <summary>
        /// Anonymous style, override explicit style property.
        /// </summary>
        private class ProgrammaticCV01 : Test
        {
            public ProgrammaticCV01() { }

            protected override void Setup()
            {
                Title = "Anonymous style, override explicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();
                sp.Resources[typeof(Button)] = style;

                // Control.
                _btnCtrl01 = new Button();
                _btnCtrl01.Focusable = false;
                sp.Children.Add(_btnCtrl01);

                // Experiment.
                _btnExp01 = new Button();
                _btnExp01.Focusable = false;
                _btnExp01.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp01);
            }

            protected override void Validate()
            {
                CV01ValidateState(_btnCtrl01, _btnExp01);
            }

            private Button _btnCtrl01;
            private Button _btnExp01;
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

        /// <summary>
        /// Named style, override explicit style property.
        /// </summary>
        private class ProgrammaticCV02 : Test
        {
            public ProgrammaticCV02() { }

            protected override void Setup()
            {
                Title = "Named style, override explicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl02 = new Button();
                _btnCtrl02.Focusable = false;
                _btnCtrl02.Style = style;
                sp.Children.Add(_btnCtrl02);

                // Experimental subject
                _btnExp02 = new Button();
                _btnExp02.Focusable = false;
                _btnExp02.Style = style;
                _btnExp02.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp02);
            }

            protected override void Validate()
            {
                CV02ValidateState(_btnCtrl02, _btnExp02);
            }

            private Button _btnCtrl02;
            private Button _btnExp02;
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

        /// <summary>
        /// Anonymous style, override implicit style property.
        /// </summary>
        private class ProgrammaticCV03 : Test
        {
            public ProgrammaticCV03() { }

            protected override void Setup()
            {
                Title = "Anonymous style, override implicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();
                sp.Resources[typeof(Button)] = style;

                // Control subject
                _btnCtrl03 = new Button();
                _btnCtrl03.Focusable = false;
                sp.Children.Add(_btnCtrl03);

                // Experimental subject
                _btnExp03 = new Button();
                _btnExp03.Focusable = false;
                _btnExp03.Width = 100;
                sp.Children.Add(_btnExp03);
            }

            protected override void Validate()
            {
                CV03ValidateState(_btnCtrl03, _btnExp03);
            }

            private Button _btnCtrl03;
            private Button _btnExp03;
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

        /// <summary>
        /// Named style, override implicit style property.
        /// </summary>
        private class ProgrammaticCV04 : Test
        {
            public ProgrammaticCV04() { }

            protected override void Setup()
            {
                Title = "Named style, override implicit style property.";

                Style style = new Style(typeof(Button));
                style.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl04 = new Button();
                _btnCtrl04.Focusable = false;
                _btnCtrl04.Style = style;
                sp.Children.Add(_btnCtrl04);

                // Experimental subject
                _btnExp04 = new Button();
                _btnExp04.Focusable = false;
                _btnExp04.Style = style;
                _btnExp04.Width = 100;
                sp.Children.Add(_btnExp04);
            }

            protected override void Validate()
            {
                CV04ValidateState(_btnCtrl04, _btnExp04);
            }

            private Button _btnCtrl04;
            private Button _btnExp04;
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

        /// <summary>
        /// BasedOn, anonymous style, override explicit property.
        /// </summary>
        private class ProgrammaticCV05 : Test
        {
            public ProgrammaticCV05() { }

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
                _btnCtrl05 = new Button();
                _btnCtrl05.Focusable = false;
                sp.Children.Add(_btnCtrl05);

                // Experimental subject
                _btnExp05 = new FooButton();
                _btnExp05.Focusable = false;
                _btnExp05.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp05);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV01
                CV01ValidateState(_btnCtrl05, _btnExp05);
            }

            private Button _btnCtrl05;
            private FooButton _btnExp05;
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

        /// <summary>
        /// BasedOn, named style, override implicit property.
        /// </summary>
        private class ProgrammaticCV06 : Test
        {
            public ProgrammaticCV06() { }

            protected override void Setup()
            {
                Title = "BasedOn, named style, override implicit property.";

                StackPanel sp = new StackPanel();

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                Style derivedStyle = new Style(typeof(FooButton), baseStyle);

                // Control subject
                _btnCtrl06 = new Button();
                _btnCtrl06.Focusable = false;
                _btnCtrl06.Style = baseStyle;
                sp.Children.Add(_btnCtrl06);

                // Experimental subject
                _btnExp06 = new FooButton();
                _btnExp06.Focusable = false;
                _btnExp06.Style = derivedStyle;
                _btnExp06.Background = Brushes.Yellow;
                sp.Children.Add(_btnExp06);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV02
                CV02ValidateState(_btnCtrl06, _btnExp06);
            }

            private Button _btnCtrl06;
            private FooButton _btnExp06;
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

        /// <summary>
        /// BasedOn, anonymous style, override explicit property.
        /// </summary>
        private class ProgrammaticCV07 : Test
        {
            public ProgrammaticCV07() { }

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
                _btnCtrl07 = new Button();
                _btnCtrl07.Focusable = false;
                sp.Children.Add(_btnCtrl07);

                // Experimental subject
                _btnExp07 = new FooButton();
                _btnExp07.Focusable = false;
                _btnExp07.Width = 100;
                sp.Children.Add(_btnExp07);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV03
                CV03ValidateState(_btnCtrl07, _btnExp07);
            }

            private Button _btnCtrl07;
            private FooButton _btnExp07;
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

        /// <summary>
        /// BasedOn, named style, override implicit property.
        /// </summary>
        private class ProgrammaticCV08 : Test
        {
            public ProgrammaticCV08() { }

            protected override void Setup()
            {
                Title = "BasedOn, named style, override implicit property.";

                Style baseStyle = new Style(typeof(Button));
                baseStyle.Setters.Add(new Setter(Button.BackgroundProperty, Brushes.Red));

                Style derivedStyle = new Style(typeof(FooButton), baseStyle);

                StackPanel sp = new StackPanel();

                // Control subject
                _btnCtrl08 = new Button();
                _btnCtrl08.Focusable = false;
                _btnCtrl08.Style = baseStyle;
                sp.Children.Add(_btnCtrl08);

                // Experimental subject
                _btnExp08 = new FooButton();
                _btnExp08.Focusable = false;
                _btnExp08.Style = derivedStyle;
                _btnExp08.Width = 100;
                sp.Children.Add(_btnExp08);
            }

            protected override void Validate()
            {
                // Validate an identical state to CV04
                CV04ValidateState(_btnCtrl08, _btnExp08);
            }

            private Button _btnCtrl08;
            private FooButton _btnExp08;
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
    }
}


// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

#define DEBUG  //Workaround - This allows Debug.Write to work.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Threading; 
using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Globalization;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.IO;
using System.Windows.Markup;
namespace Microsoft.Test.Globalization.MultiLangControl
{
    [Localizability(LocalizationCategory.None, Readability=Readability.Readable)]
    public class PanelFlow : Panel, IAddChild
    {
        public PanelFlow() : base()
        {
        }
        public Color Color = Color.FromRgb(0x00, 0xff, 0x00);
public Typeface Typeface = new Typeface("Arial");
        protected override void OnRender(DrawingContext ctx)
        {
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, 0),
                new Point(100, 100));
            ctx.DrawLine(
                new Pen(new SolidColorBrush(Color), 2.0f),
                new Point(0, 100),
                new Point(100, 0));
ctx.DrawText(
new System.Windows.Media.FormattedText("PanelFlow", System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 10, Brushes.Red),
new Point(10, 10));
        }
    }
    [Localizability(LocalizationCategory.Button, Readability=Readability.Readable, Modifiability=Modifiability.Unmodifiable)]
    public class MyButton : Button
    {
        public static readonly DependencyProperty MagicStringProperty = DependencyProperty.Register("MagicString", typeof(string), typeof(MyButton));

        static MyButton()
        {
            //the following four lines can be merged into one
            PropertyMetadata myMeta = new PropertyMetadata();

            myMeta.DefaultValue = "MyDefaultString";
            MagicStringProperty.OverrideMetadata(typeof(MyButton), myMeta);
        }

        [Localizability(LocalizationCategory.Text)]
        public string MagicString
        {
            get { return (string) GetValue(MagicStringProperty); }
            set { SetValue(MagicStringProperty, value); }
        }
    }
    [Localizability(LocalizationCategory.Text, Readability=Readability.Unreadable, Modifiability=Modifiability.Unmodifiable)]
    public class ClassDeriveFromObjectDirectly
    {
        public static readonly DependencyProperty NumberProperty = DependencyProperty.RegisterAttached("Number", typeof(int), typeof(ClassDeriveFromObjectDirectly), new PropertyMetadata(0));

        public static void SetNumber(DependencyObject d, int Number)
        {
            d.SetValue(NumberProperty, Number); //Boxing
        }

        public static int GetNumber(DependencyObject d)
        {
            return (int)d.GetValue(NumberProperty);  //Unboxing
        }
    }
    [Localizability(LocalizationCategory.Text, Readability=Readability.Readable)]
    public class MyClass : DependencyObject
    {
        static MyClass()
        {
            ClassDeriveFromObjectDirectly.NumberProperty.AddOwner(typeof(MyClass));

            PropertyMetadata myMeta = new PropertyMetadata();

            myMeta.DefaultValue = 100;
            ClassDeriveFromObjectDirectly.NumberProperty.OverrideMetadata(typeof(MyClass), myMeta);
        }

        //Provide CLR Accessors and local caching
        public int Number
        {
            get { return (int) GetValue(ClassDeriveFromObjectDirectly.NumberProperty); }
            set { SetValue(ClassDeriveFromObjectDirectly.NumberProperty, value); }
        }
    }
}

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
using System.Globalization;
using System.Threading; using System.Windows.Threading;
using System.Windows.Input;
using System.Windows.Media;
using System.Collections;
using System.ComponentModel;
using System.ComponentModel.Design.Serialization;
using System.Reflection;
using System.IO;
using System.Windows.Markup;
namespace Microsoft.Test.Globalization.MultiLangControl
{
    [Localizability(LocalizationCategory.None, Readability=Readability.Readable)]
    public class กรอบ : Panel, IAddChild
    {
        public กรอบ() : base()
        {
        }
        public Color Color = Color.FromRgb(0xff, 0xff, 0x00);
public Typeface Typeface = new Typeface("Microsoft Sans Serif");
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
string a = System.Globalization.CultureInfo.CurrentCulture.ToString();
ctx.DrawText(
new System.Windows.Media.FormattedText("กรอบ " + a, System.Globalization.CultureInfo.CurrentCulture, FlowDirection.LeftToRight, Typeface, 10, Brushes.Red),
new Point(10, 10));
        }
    }
    [Localizability(LocalizationCategory.Button, Readability=Readability.Readable, Modifiability=Modifiability.Unmodifiable)]
    public class ปุ่ม : Button
    {
        public static readonly DependencyProperty MagicStringProperty = DependencyProperty.Register("ประโยค", typeof(string), typeof(ปุ่ม));

        static ปุ่ม()
        {
            PropertyMetadata myMeta = new PropertyMetadata();

            myMeta.DefaultValue = "ประโยคของฉัน";
            MagicStringProperty.OverrideMetadata(typeof(ปุ่ม), myMeta);
        }

        [Localizability(LocalizationCategory.Text)]
        public string ประโยค
        {
            get { return (string) GetValue(MagicStringProperty); }
            set { SetValue(MagicStringProperty, value); }
        }
    }
    [Localizability(LocalizationCategory.Text, Readability=Readability.Unreadable, Modifiability=Modifiability.Unmodifiable)]
    public class คลาสที่มาจากวัตถุโดยตรง
    {
        public static readonly DependencyProperty NumberProperty = DependencyProperty.RegisterAttached("ตัวเลข", typeof(int), typeof(คลาสที่มาจากวัตถุโดยตรง), new PropertyMetadata(0));

        public static void SetNumber(DependencyObject d, int ตัวเลข)
        {
            d.SetValue(NumberProperty, ตัวเลข); //Boxing
        }

        public static int GetNumber(DependencyObject d)
        {
            return (int)d.GetValue(NumberProperty);  //Unboxing
        }
    }
    [Localizability(LocalizationCategory.Text, Readability=Readability.Readable)]
    public class คลาส : DependencyObject
    {
        static คลาส()
        {
            คลาสที่มาจากวัตถุโดยตรง.NumberProperty.AddOwner(typeof(คลาส));

            PropertyMetadata myMeta = new PropertyMetadata();

            myMeta.DefaultValue = 100;
            คลาสที่มาจากวัตถุโดยตรง.NumberProperty.OverrideMetadata(typeof(คลาส), myMeta);
        }

        //Provide CLR Accessors and local caching
        public int ตัวเลข
        {
            get { return (int) GetValue(คลาสที่มาจากวัตถุโดยตรง.NumberProperty); }
            set { SetValue(คลาสที่มาจากวัตถุโดยตรง.NumberProperty, value); }
        }
    }
}

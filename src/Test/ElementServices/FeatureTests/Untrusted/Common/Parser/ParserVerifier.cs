// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using Avalon.Test.CoreUI.Trusted;
using Avalon.Test.CoreUI;
using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using Avalon.Test.CoreUI.Common;
using Microsoft.Test.Serialization;
using Microsoft.Test.Serialization.CustomElements;
using Microsoft.Test.Integration;
using Avalon.Test.CoreUI.Serialization;
using Avalon.Test.CoreUI.IdTest;
using Microsoft.Test.Logging;

namespace Avalon.Test.CoreUI.Parser
{
    /// <summary>
    /// Holds verification routines for various XAML files.
    /// </summary>
    public class ParserVerifier
    {
        /// <summary>
        /// Default verification routine.  Does not real verification.
        /// </summary>
        public static void GenericVerify(UIElement uie, Hashtable IDedObjects)
        {
            CoreLogger.LogStatus("Inside ParserVerifier.GenericVerify()...");
        }

        /// <summary>
        /// Throws an exception with the given error message, if the condition is false.
        /// </summary>
        /// <param name="condition">Given condition</param>
        /// <param name="errorMesg">Error message for the exception to be thrown</param>
        private static void Assert(bool condition, String errorMesg)
        {
            if (!condition)
            {
                throw new Microsoft.Test.TestValidationException(errorMesg);
            }
        }

        /// <summary>
        /// Verify the properties of the button.
        /// </summary>
        public static void ClrPropertyVerify(UIElement uie)
        {
            String errorMesg = "CLR property verification failed";

            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");

            // Verify the foreground (Red), 
            Assert(Color.Equals((Button1.Foreground as SolidColorBrush).Color, Colors.Red), errorMesg);

            // background (Blue)    
            Assert(Color.Equals((Button1.Background as SolidColorBrush).Color, Colors.Blue), errorMesg);

            // and borderbrush (Green) properties.
            Assert(Color.Equals((Button1.BorderBrush as SolidColorBrush).Color, Colors.Green), errorMesg);
        }

        /// <summary>
        /// Verify the text in the button.
        /// </summary>
        public static void TextRecordVerify(UIElement uie)
        {
            String errorMesg = "Text record verification failed";

            ContentControl[] elements = new ContentControl[21];
            for (int i = 0; i < 21; i++)
            {
                elements[i] = (ContentControl)LogicalTreeHelper.FindLogicalNode(uie, "Element" + i.ToString());
            }
            
            ComboBox elem8ComboBox = elements[8].Content as ComboBox;
            ComboBox elem9ComboBox = elements[9].Content as ComboBox;
            ComboBox elem10ComboBox = elements[10].Content as ComboBox;

            ItemCollection elem8Children = elem8ComboBox.Items;
            ItemCollection elem9Children = elem9ComboBox.Items;
            ItemCollection elem10Children = elem10ComboBox.Items;

            /*
            


*/

            Assert(((String)elements[0].Content).Equals("उ नीहरू किन नेपाली मात्र बोल्न सक्दैनन् ?"), errorMesg);
            Assert(((String)elements[1].Content).Equals("な ぜ、みんな日本語を話してくれないのか？"), errorMesg);
            Assert(((String)elements[2].Content).Equals("ಅ ವರು ಕನ್ನಡ ಮಾತನಾಡಬಹುದಲ್ಲಾ?"), errorMesg);
            Assert(((String)elements[3].Content).Equals("ทำ ไมเขาถึงไม่พูดภาษาไทย"), errorMesg);
            Assert(((String)elements[4].Content).Equals("他 们为什么不说中文（中国）？"), errorMesg);
            Assert(((String)elements[5].Content).Equals("Hey \\n\\x Dude"), errorMesg);
            Assert(((String)elements[6].Content).Equals("Hey dear Dude"), errorMesg);
            Assert(((String)elements[7].Content).Equals("Hey Dude"), errorMesg);
            Assert(((String)elem8Children[0]).Equals("Hey Dude "), errorMesg);
            Assert(((String)elem9Children[1]).Equals(" Hey Dude"), errorMesg);
            Assert(((String)elem10Children[0]).Equals("Hey dear "), errorMesg);
            Assert(((String)elem10Children[2]).Equals(" Dude"), errorMesg);
            Assert(Color.Equals((((Button)elements[11]).Background as SolidColorBrush).Color, Colors.Red), errorMesg);
            Assert(((String)elements[12].Content).Equals("ทำไมเขาถึงไม่พูดภาษาไทย Hi 他们为什么不说中文（中国）？ Dude उनीहरू किन नेपाली मात्र बोल्न सक्दैनन् ?"), errorMesg);
            Assert(((String)elements[13].Content).Equals("Hey Dude"), errorMesg);
            Assert(((String)elements[15].Content).Equals("  Hey     Dude  "), errorMesg); //No verification for 14;
            Assert(((String)elements[16].Content).Equals("Hey" + "\n\t    Dude" + "\n\t"), errorMesg); // With XML, newline is always \n
            Assert(((String)elements[17].Content).Equals(" Hey Dude"), errorMesg);

            Assert(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[0] as String).Equals("Hello World "), errorMesg);
            Assert(((elements[18].GetValue(MyClass.MyChildrenProperty) as MyChildren).Children[2] as String).Equals(" Hi There"), errorMesg);

            Assert(Color.Equals((((Button)elements[19]).Background as SolidColorBrush).Color, Colors.Red), errorMesg);
            Assert(Color.Equals((((Button)elements[20]).Background as SolidColorBrush).Color, Colors.Green), errorMesg);

            // 

        }


        /// <summary>
        /// Verify complex properties.
        /// </summary>
        public static void ComplexPropertyVerify(UIElement uie)
        {
            String errorMesg = "Complex Property verification failed";

            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");

            // Verify Button0 properties
            Color Button0Color = (Button0.Background as SolidColorBrush).Color;
            Assert((Button0Color.A == 100) && (Button0Color.G == 255), errorMesg);

            // Verify Button1 properties
            GradientStopCollection Stops = (Button1.Background as LinearGradientBrush).GradientStops;
            Assert(Stops.Count == 3, errorMesg);

            Assert((Stops[0] as GradientStop).Offset == 0.2, errorMesg);
            Assert((Stops[1] as GradientStop).Offset == 0.4, errorMesg);
            Assert((Stops[2] as GradientStop).Offset == 0.6, errorMesg);
        }


        /// <summary>
        /// Verify custom properties and events.
        /// </summary>
        public static void CustomPropEventsVerify(UIElement uie)
        {
            String errorMesg = "Custom Properties and Events verification failed";

            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");
            Button Button4 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button4");
            Button Button5 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button5");
            Button Button7 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button7");

            Assert(((String)Button0.GetValue(MyClass.MyTransparencyProperty)).Equals("1.0"), errorMesg);
            Assert(((MyColor)Button0.GetValue(MyClass.MyColorProperty)).Color.Equals("Red"), errorMesg);

            Assert(((MyColor)Button1.GetValue(MyClass.MyColorProperty)).Color.Equals("Blue"), errorMesg);

            Assert(((MyColor)Button2.GetValue(MyClass.MyColorProperty)).Color.Equals("Yellow"), errorMesg);

            Assert(((MyColor)Button3.GetValue(MyClass.MyColorProperty)).Color.Equals("Green"), errorMesg);

            // Verify the array property 
            Brush[] brushes = (Brush[])Button4.GetValue(MyClass.MyBrushesProperty);
            Assert(Color.Equals((brushes[0] as SolidColorBrush).Color, Colors.Red), errorMesg);

            // Verify the array property
            brushes = (Brush[])Button5.GetValue(MyClass.MyBrushesProperty);
            Assert(Color.Equals((brushes[0] as SolidColorBrush).Color, Colors.Red), errorMesg);
            Assert(Color.Equals((brushes[1] as SolidColorBrush).Color, Colors.Green), errorMesg);

            // Verify the IDictionary property
            Hashtable table = (Hashtable)Button7.GetValue(MyClass.MyIDictProperty);
            Assert(Color.Equals((table[""] as SolidColorBrush).Color, Colors.Red), errorMesg);
            Assert(Color.Equals((table[typeof(Button)] as SolidColorBrush).Color, Colors.Green), errorMesg);
            Assert(Color.Equals((table["MyColor"] as SolidColorBrush).Color, Colors.Blue), errorMesg);
        }
        /// <summary>
        /// Verify custom properties.
        /// </summary>
        public static void CustomPropertyVerify(UIElement uie)
        {
            String errorMesg = "Custom Properties and Events verification failed";

            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");

            Assert(((String)Button0.GetValue(MyClass.MyTransparencyProperty)).Equals("1.0"), errorMesg);
            Assert(((MyColor)Button0.GetValue(MyClass.MyColorProperty)).Color.Equals("Red"), errorMesg);

            Assert(((MyColor)Button1.GetValue(MyClass.MyColorProperty)).Color.Equals("Blue"), errorMesg);

            Assert(((MyColor)Button2.GetValue(MyClass.MyColorProperty)).Color.Equals("Yellow"), errorMesg);
        }


        /// <summary>
        /// Verify alias properties in styles.
        /// </summary>
        public static void PropertyAliasInStyleVerify(UIElement uie)
        {
            String errorMesg = "Verification of aliased properties in styles failed";

            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");

            // Reach the elements in the visual tree for Button0.

            Border Button0Border = VisualTreeHelper.GetChild(Button0, 0) as Border;

            StackPanel Button0StackPanel = VisualTreeHelper.GetChild(Button0Border, 0) as StackPanel;

            // Verify the properties of Button0's visual tree elements
            Assert(Color.Equals((Button0Border.Background as SolidColorBrush).Color, Colors.Red), errorMesg);

            // 
            /*
            MouseUtility.MouseLeftButtonClickonElement(Button0, 1, 1, true);
            Assert(Color.Equals((Button0Border.Background as SolidColorBrush).Color, Colors.Blue), errorMesg);
            */

            Assert(Button0StackPanel.Height == 200, errorMesg);
            Assert(Button0StackPanel.Width == 200, errorMesg);

            // Reach the elements in the visual tree for Button1.            
            Border Button1Border = VisualTreeHelper.GetChild(Button1, 0) as Border;

            // Verify the properties of Button1's visual tree elements
            Assert(Color.Equals((Button1Border.Background as SolidColorBrush).Color, Colors.DarkRed), errorMesg);
            Assert(Color.Equals((Button1Border.BorderBrush as SolidColorBrush).Color, Colors.Yellow), errorMesg);
            Assert(Button1Border.BorderThickness.Left == 5, errorMesg);
        }

        /// <summary>
        /// Verify IList properties.
        /// </summary>
        public static void PropertyIListVerify(UIElement uie)
        {
            String errorMesg = "PropertyIList verification failed";

            ListBox ListBox0 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox0");
            ItemCollection Items = ListBox0.Items;

            Assert(Items.Count == 4, errorMesg);

            Assert((Items[0] as String).Equals("Hello 0 "), errorMesg);
            Assert(((Items[1] as ContentControl).Content as String).Equals("Hello 1"), errorMesg);
            Assert(((Items[2] as ContentControl).Content as String).Equals("Hello 2"), errorMesg);
            Assert(((Items[3] as ContentControl).Content as String).Equals("Hello 3"), errorMesg);
        }

        /// <summary>
        /// Verify the element tree for CompactDatabinding2.xaml.
        /// </summary>
        public static void CompactSyntaxVerify1(UIElement uie)
        {
            CoreLogger.LogStatus("Inside ParserVerifier.CompactSyntaxVerify1()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            Dock myDock = DockPanel.GetDock(myPanel);

            if (!myPanel.LastChildFill)
            {
                throw new Microsoft.Test.TestValidationException("LastChildFill should be true.");
            }

            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Should has only 1 child");
            }

            UIElement child = myChildren[0];

            TextBlock text = child as TextBlock;

            if (null == text)
            {
                throw new Microsoft.Test.TestValidationException("Should Have Text");
            }

            if (0 != String.Compare(text.Text.Trim(), "Test", true))
            {
                throw new Microsoft.Test.TestValidationException("Text.Text is >>" + text.Text.Trim() + "<<, should be Test");
            }

            //if (text.FontSize != 200)
            //{
            //  throw new Microsoft.Test.TestValidationException("Text.FontSize should be 200").
            //}
            if (text.FontStyle != System.Windows.FontStyles.Italic)
            {
                throw new Microsoft.Test.TestValidationException("Text.FontStyle is" + text.FontStyle + ", should be 2: Italic");
            }
        }
        /// <summary>
        /// Verifies x:Include tag is parsed and tree is created correctly.
        /// </summary>
        public static void VerifyIncludeTag2(UIElement root)
        {
            // Get StackPanel.
            StackPanel stackPanel = (StackPanel)LogicalTreeHelper.FindLogicalNode(root, "Panel1");
            if (null == stackPanel)
                CoreLogger.LogStatus("not found StackPanel.");
            else
                CoreLogger.LogStatus("found StackPanel.");

            //
            // Verify StackPanel's Background is a resource.
            //
            FrameworkElement fe = (FrameworkElement)root;
            if (null == fe)
                CoreLogger.LogStatus("Root is not a FrameworkElement.");
            else
                CoreLogger.LogStatus("Root is not a FrameworkElement.");

            Brush res = (Brush)fe.FindResource("GreenBrush");
            if (null == res)
                CoreLogger.LogStatus("GreenBrush not found.");
            else
                CoreLogger.LogStatus("GreenBrush found.");

            SolidColorBrush brush = (SolidColorBrush)stackPanel.Background;

            if (brush != res)
            {
                throw new Exception("Resource brush != stackPanel.Background brush.");
            }

            //
            // Verify StackPanel's Background is green and half-transparent.
            //
            Color color = brush.Color;

            if (color.A != 50)
                throw new Exception("SolidColorBrush.Color.A != 50");
            else if (color.G != 219)
                throw new Exception("SolidColorBrush.Color.G != 219");
            else if (color.B != 0)
                throw new Exception("SolidColorBrush.Color.B != 0");
            else if (color.R != 0)
                throw new Exception("SolidColorBrush.Color.R != 0");
        }
        /// <summary>
        /// 
        /// </summary>
        public static void LiteralSimpleVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside ParserVerifier.LiteralSimpleVerify()...");

            DockPanel myPanel = uie as DockPanel;

            if (null == myPanel)
            {
                throw new Microsoft.Test.TestValidationException("Should be DockPanel");
            }

            Dock myDock = DockPanel.GetDock(myPanel);
            UIElementCollection myChildren = myPanel.Children;

            if (myChildren.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Should has only 1 child");
            }

            UIElement child = myChildren[0];
            NodeWithLiteralContent nodeWithLiteral = child as NodeWithLiteralContent;

            if (null == nodeWithLiteral)
            {
                throw new Microsoft.Test.TestValidationException("Should Have NodeWithLiteralContent");
            }

            if (0 != String.Compare(nodeWithLiteral.LiteralString.Trim(), "test String", true))
            {
                throw new Microsoft.Test.TestValidationException("nodeWithLiteral.LiteralString is >>" + nodeWithLiteral.LiteralString.Trim() + "<<, should be Test");
            }
        }

        /// <summary>
        /// Verify x:Shared for resources. See DefSharedVerification.xaml
        /// </summary>
        public static void DefSharedVerify(UIElement uie)
        {
            String errorMesg = "x:Shared verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");

            // Verify that Button0 and Button1 get 2 different instances of SolidColorBrush, 
            // since GreenBrush is not shared 
            Assert(Button0.Background != Button1.Background, errorMesg);

            // Button2 and Button3 should get the same instance of SolidColorBrush,
            // since RedBrush is shared.
            Assert(Button2.Background == Button3.Background, errorMesg);
        }

        /// <summary>
        /// Verify that elements and text under an IEnumerable property 
        /// are added to the parent as children.
        /// See Petzold01_Verification.xaml
        /// </summary>
        public static void PropIEnumerableVerify(UIElement uie)
        {
            String errorMesg = "IEnumerable property verification failed";
            DockPanel DockPanel0 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel0");
            UIElementCollection children = DockPanel0.Children;

            // We cannot verify addition of text, because DockPanel.AddText(string) does nothing.
            Assert(((children[1] as Button).Content as string) == "dear", errorMesg);
        }

        /// <summary>
        /// Verify that properties set on a Style using Setters are set fine.
        /// </summary>
        /// <param name="uie"></param>
        public static void StyleSetterVerify(UIElement uie)
        {
            String errorMesg = "Style setter verification failed";
            ListBox ListBox0 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox0");
            ListBox ListBox1 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox1");
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");

            // Verify all the different properties on ListBox0            
            SelectionMode mode = (SelectionMode)ListBox0.GetValue(ListBox.SelectionModeProperty);
            Assert(mode == SelectionMode.Extended, errorMesg);

            SolidColorBrush background = ListBox0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Yellow), errorMesg);

            double fontSize = (double)ListBox0.GetValue(Control.FontSizeProperty);
            Assert(fontSize == 48.0, errorMesg);

            Dock dock = (Dock)ListBox0.GetValue(DockPanel.DockProperty);
            Assert(dock == Dock.Bottom, errorMesg);

            MyColor color = (MyColor)ListBox0.GetValue(MyClass.MyColorProperty);
            Assert(color.Color == "Yellow", errorMesg);

            FontStyle fontStyle = (FontStyle)ListBox0.GetValue(Control.FontStyleProperty);
            Assert(fontStyle == FontStyles.Italic, errorMesg);

            // Check one of ListBox1's properties
            color = (MyColor)ListBox1.GetValue(MyClass.MyColorProperty);
            Assert(color.Color == "Yellow", errorMesg);

            // Check Button0's background
            background = Button0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Red), errorMesg);

            // Verify Button1's properties, which it's supposed to get from the 
            // Open-ended style
            background = Button1.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Yellow), errorMesg);

            fontSize = (double)Button1.GetValue(Control.FontSizeProperty);
            Assert(fontSize == 48.0, errorMesg);

            dock = (Dock)Button1.GetValue(DockPanel.DockProperty);
            Assert(dock == Dock.Bottom, errorMesg);
        }

        /// <summary>
        /// SourceablePropertiesVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void SourceablePropertiesVerify(UIElement uie)
        {
            String errorMesg = "SourceableProperties verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");

            SolidColorBrush background = (SolidColorBrush)Button0.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Red), 
                errorMesg + " Expected background: Red. Actual background: " + background);
        }

        /// <summary>
        /// TypeExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void TypeExtensionVerify(UIElement uie)
        {
            String errorMesg = "TypeExtension verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            ListBox ListBox0 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox0");

            // Verify Background, Foreground, MyClass.MyColor and Content properties for Button0
            SolidColorBrush background = (SolidColorBrush)Button0.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Yellow), errorMesg);

            SolidColorBrush foreground = (SolidColorBrush)Button0.GetValue(Control.ForegroundProperty);
            Assert(Color.Equals(foreground.Color, Colors.Red), errorMesg);

            /* 


*/

            String[] content = (String[])Button0.GetValue(ContentControl.ContentProperty);
            Assert("Hello" == content[0], errorMesg);
            Assert("World" == content[1], errorMesg);

            // Verify Background for ListBox0. The SelectionMode property cannot be verified, 
            // since it't not assigned any particular value.
            background = ListBox0.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Blue), errorMesg);
        }



        /// <summary>
        /// LiteralAndNullExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void LiteralAndNullExtensionVerify(UIElement uie)
        {
            String errorMesg = "LiteralAndNullExtension verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");
            Button Button4 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button4");
            Button Button5 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button5");

            // Verify the Content property for Buttons 0 thru 4
            String content = (String)Button0.GetValue(ContentControl.ContentProperty);
            Assert(" {Foo}" == content, errorMesg + " for Button0.");

            content = (String)Button1.GetValue(ContentControl.ContentProperty);
            Assert("}" == content, errorMesg + " for Button1.");

            content = (String)Button2.GetValue(ContentControl.ContentProperty);
            Assert("{Foo}" == content, errorMesg + " for Button2.");

            content = (String)Button3.GetValue(ContentControl.ContentProperty);
            Assert("{" == content, errorMesg + " for Button3.");

            content = (String)Button4.GetValue(ContentControl.ContentProperty);
            Assert("{{}}" == content, errorMesg + " for Button4.");

            // Verify that Button5's background is set to Null
            SolidColorBrush background = (SolidColorBrush)Button5.GetValue(Control.BackgroundProperty);
            Assert(null == background, errorMesg + " for Button5.");
        }

        /// <summary>
        /// StaticExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void StaticExtensionVerify(UIElement uie)
        {
            String errorMesg = "StaticExtension verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");
            Button Button4 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button4");
            Button Button5 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button5");
  

            
            // Verify Button0's cursor
            Cursor cursor = (Cursor)Button0.GetValue(FrameworkElement.CursorProperty);
            Assert(cursor == Cursors.Pen, errorMesg);

            // Verify Button1's cursor
            cursor = (Cursor)Button1.GetValue(FrameworkElement.CursorProperty);
            Assert(cursor == Cursors.Pen, errorMesg);

            // Verify Button2 and Button3's content (using content tags and without)
            String content = (String)Button2.GetValue(ContentControl.ContentProperty);
            Assert("Hello world" == content, errorMesg);
            content = (String)Button4.GetValue(ContentControl.ContentProperty);
            Assert("Hello world" == content, errorMesg);

            content = (String)Button3.GetValue(ContentControl.ContentProperty);
            Assert("My motto" == content, errorMesg);
            content = (String)Button5.GetValue(ContentControl.ContentProperty);
            Assert("My motto" == content, errorMesg);


            // Verify ListBox0's SelectionMode
            ListBox ListBox0 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox0");
            SelectionMode mode = (SelectionMode)ListBox0.GetValue(ListBox.SelectionModeProperty);
            Assert(SelectionMode.Multiple == mode, errorMesg);
            ListBox ListBox1 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox1");
            mode = (SelectionMode)ListBox1.GetValue(ListBox.SelectionModeProperty);
            Assert(SelectionMode.Multiple == mode, errorMesg);
        }

        /// <summary>
        /// BindExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void BindExtensionVerify(UIElement uie)
        {
            String errorMesg = "BindExtension verification failed";

            String matchString = "Wak-a-doo!";
            TextBlock Text0 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "Text0");
            TextBlock Text1 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "Text1");

            CustomDockPanel CustomDockPanel0 = (CustomDockPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomDockPanel0");
            ResourceDictionary ResourcesCustom0 = CustomDockPanel0.Resources;

            TextBlock TextBlock2 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "TextBlock2");
            ComboBox ComboBox1 = (ComboBox)LogicalTreeHelper.FindLogicalNode(uie, "ComboBox1");

            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            TextBox TextBox1 = (TextBox)LogicalTreeHelper.FindLogicalNode(uie, "TextBox1");
            ListBox ListBox1 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox1");

            Assert(   ( (string) Button1.Content).Equals(matchString), errorMesg);

            // Verify the Foreground, FontSize and Text properties of Text0.
            // They are all databound
            SolidColorBrush foreground = (SolidColorBrush)Text0.GetValue(TextBlock.ForegroundProperty);
            Assert(Color.Equals(foreground.Color, Colors.Red), errorMesg);
            foreground = (SolidColorBrush)TextBlock2.GetValue(TextBlock.ForegroundProperty);
            Assert(Color.Equals(foreground.Color, Colors.Red), errorMesg);

            double fontsize = (double)Text0.GetValue(TextBlock.FontSizeProperty);
            Assert(fontsize == 200.0, errorMesg);
            fontsize = (double)TextBlock2.GetValue(TextBlock.FontSizeProperty);
            Assert(fontsize == 200.0, errorMesg);


            string textcontent = (string)Text0.GetValue(TextBlock.TextProperty);
            Assert("Hello World" == textcontent, errorMesg);
            textcontent = (string)TextBlock2.GetValue(TextBlock.TextProperty);
            Assert("Hello World" == textcontent, errorMesg);
            textcontent = (string)TextBox1.GetValue(TextBox.TextProperty);
            Assert(matchString == textcontent, errorMesg);
            textcontent = (string) (((ComboBoxItem) ComboBox1.Items[0]).Content);
            Assert(matchString == textcontent, errorMesg);
            textcontent = (string) (((ListBoxItem) ListBox1.Items[0]).Content);
            Assert(matchString == textcontent, errorMesg);


            // Verify Foreground of Text1. It's databound to Null XPath, 
            // thus should have default value
            foreground = (SolidColorBrush)Text1.GetValue(TextBlock.ForegroundProperty);
            Assert(Color.Equals(foreground.Color, Colors.Black), errorMesg);
            // Verify FontSize of Text1
            fontsize = (double)Text1.GetValue(TextBlock.FontSizeProperty);
            Assert(fontsize == 200.0, errorMesg);
        }



        private static void AssertInlineButtonHasString(object inline, string strContent, string errorMesg)
        {
            InlineUIContainer inlineUIContainer = (InlineUIContainer) inline;
            Button button = (Button) inlineUIContainer.Child;
            String buttonString = (String) button.Content;
            Assert(strContent.Equals(buttonString), errorMesg);
        }



        private static void AssertInlineRunHasString(object inline, string strContent, string errorMesg)
        {
            Run inlineRun = (Run)inline;
            String runText = (String) inlineRun.Text;
            Assert(strContent.Equals(runText), errorMesg);
        }

        private static void AssertContentControlHasString(object objContentControl, string strContent, string errorMesg)
        {
            ContentControl contentControl = (ContentControl) objContentControl;
            String contentString = (String)contentControl.Content;
            Assert(strContent.Equals(contentString), errorMesg);
        }

        private static void AssertContentControlHasButton(object objContentControl, string strContent, string errorMesg)
        {
            ContentControl contentControl = (ContentControl)objContentControl;
            Button contentButton = (Button)contentControl.Content;
            AssertContentControlHasString(contentButton, strContent, errorMesg); 
        }
            
        /// <summary>
        /// ResourceExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void ResourceExtensionVerify(UIElement uie)
        {

            String errorMesg = "ResourceExtension verification failed";

            CustomDockPanel CustomDockPanel0 = (CustomDockPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomDockPanel0");
            ResourceDictionary ResourcesCustom0 = CustomDockPanel0.Resources;
            
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");
            Button Button4 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button4");
            Button Button5 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button5");
            Button Button6 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button6");
            Button Button7 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button7");
            Button Button8 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button8");

            DockPanel DockPanel0 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel0");
            ResourceDictionary Resources0 = DockPanel0.Resources;
            // change a dynamic reference
            Resources0["string1"] = "string1mod";

            TextBlock TextBlock1 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "TextBlock1");
            ListBox ListBox1 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox1");

            DockPanel DockPanel1 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel1");
            ResourceDictionary Resources1 = DockPanel1.Resources;

            DockPanel DockPanel2 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel2");
            ResourceDictionary Resources2 = DockPanel2.Resources;
            ComboBox ComboBox3 = (ComboBox)LogicalTreeHelper.FindLogicalNode(uie, "ComboBox3");
            ListBox ListBox3 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox3");

            DockPanel DockPanel3 = (DockPanel)LogicalTreeHelper.FindLogicalNode(uie, "DockPanel3");
            ResourceDictionary Resources3 = DockPanel3.Resources;
            Button Button20 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button20");
            Button Button21 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button21");
            Button Button22 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button22");
            Button Button23 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button23");
            ListBox ListBox13 = (ListBox)LogicalTreeHelper.FindLogicalNode(uie, "ListBox13");


            Button ButtonResource12 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "ButtonResource1");
            TextBlock TextBlock2 = (TextBlock)LogicalTreeHelper.FindLogicalNode(uie, "TextBlock2");

            TextBox TextBox1 = (TextBox)LogicalTreeHelper.FindLogicalNode(uie, "TextBox1");
            TextBox TextBox2 = (TextBox)LogicalTreeHelper.FindLogicalNode(uie, "TextBox2");


            // checks listboxes with dynamic and static buttons and strings
            ListBoxItem ListBoxItem_imabutton6 = (ListBoxItem)LogicalTreeHelper.FindLogicalNode(uie, "ListBoxItem_imabutton6");
            Assert((ListBox1.Items.Count == 10), errorMesg);
            Assert((ListBoxItem_imabutton6.Content.GetType().ToString().Equals("System.Windows.Controls.Button")), errorMesg);
            Button ListBox1Button6 = (Button) ListBoxItem_imabutton6.Content;
            AssertContentControlHasButton(ListBox1.Items[0], "button4", errorMesg);
            AssertContentControlHasButton(ListBox1.Items[1], "button5", errorMesg);
            AssertContentControlHasButton(ListBox1.Items[2], "button6", errorMesg);
            // ensure that dynamic reference changed
            AssertContentControlHasString(ListBox1.Items[3], "string1mod", errorMesg);
            AssertContentControlHasString(ListBox1.Items[4], "string2", errorMesg);
            AssertContentControlHasString(ListBox1.Items[5], "string3", errorMesg);
            AssertContentControlHasString(ListBox1.Items[6], "string4", errorMesg);
            Assert(((string)ListBox1.Items[7]).Equals(" notintags "), errorMesg);
            Assert(((string)ListBox1.Items[8]).Equals("string5"), errorMesg);
            // ensure static reference didn't change
            Assert(((string)ListBox1.Items[9]).Equals("string1"), errorMesg);

            // test Textblock with static button and string resources
            // (Textblocks don't accept dynamic resources)
            Assert(ListBox1Button6.Content.ToString().Equals("button6"), errorMesg);
            Assert((TextBlock1.Inlines.Count == 7), errorMesg); // add check for 3 buttons and 2 whitespaces


            IEnumerator inlineEnum = TextBlock1.Inlines.GetEnumerator();

            int counter = 0;
            int buttonCounter = 0;
            while (inlineEnum.MoveNext()) // isn't there a way to index into an inlines collection?
            {
                switch (counter)
                {
                    case 0:
                    case 2:
                    case 4:
                        buttonCounter++;
                        AssertInlineButtonHasString(inlineEnum.Current, "button" + buttonCounter, errorMesg);
                        break;
                    case 1:
                    case 3:
                    case 5:
                        AssertInlineRunHasString(inlineEnum.Current, " ", errorMesg);
                        break;
                    case 6:
                        AssertInlineRunHasString(inlineEnum.Current, "string1", errorMesg);
                        break;
                    default:
                        throw new ApplicationException("Too many inlines in ResourceExtensionVerify()");
                }
                counter++;
            }


            Assert((DockPanel1.Children.Count == 3), errorMesg);
            Assert((DockPanel2.Children.Count == 2), errorMesg);
            Assert((DockPanel3.Children.Count == 9), errorMesg);



            // checks dynamic and static refs in textboxes
            Assert(TextBox1.Text == "wowww2", errorMesg);
            Assert(TextBox2.Text == "wowww2", errorMesg);


            SolidColorBrush button22Foreground = (SolidColorBrush)Button22.GetValue(Control.ForegroundProperty);
            Assert(Color.Equals(button22Foreground.Color, Colors.Brown), errorMesg);
            SolidColorBrush button23Foreground = (SolidColorBrush)Button23.GetValue(Control.ForegroundProperty);
            Assert(Color.Equals(button22Foreground.Color, Colors.Brown), errorMesg);

            // check button contents with dynamic and static resources (buttons and strings);
            Assert(Button21.Content.GetType().ToString().Equals("System.Windows.Controls.Button"), errorMesg);
            Assert(Button22.Content.GetType().ToString().Equals("System.Windows.Controls.Button"), errorMesg);
            Assert(Button23.Content.ToString().Equals("wowww1"), errorMesg);
            Assert(Button20.Content.ToString().Equals("wowww1"), errorMesg);

            // Verify Button0 and Button1's background property
            SolidColorBrush background = (SolidColorBrush)Button0.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Green), errorMesg);

            background = (SolidColorBrush)Button1.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Red), errorMesg);

            // Verify Button2's content
            String content = (String)Button2.GetValue(ContentControl.ContentProperty);
            Assert("Hello World" == content, errorMesg);



            // Verify Button5 thru 8's background property
            background = (SolidColorBrush)Button5.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Pink), errorMesg);

            background = (SolidColorBrush)Button6.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Pink), errorMesg);

            background = (SolidColorBrush)Button7.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Pink), errorMesg);

            background = (SolidColorBrush)Button8.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, Colors.Pink), errorMesg);
        }

        /// <summary>
        /// CustomMarkupExtensionVerify
        /// </summary>
        /// <param name="uie"></param>
        public static void CustomMarkupExtensionVerify(UIElement uie)
        {
            String errorMesg = "CustomMarkupExtension verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");
            Button Button4 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button4");
            Button Button5 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button5");
            Button Button6 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button6");
            Button Button7 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button7");
            Button Button8 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button8");
            Button Button9 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button9");
            Button Button10 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button10");
            Button Button11 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button11");
            Button Button12 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button12");
            Button Button13 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button13");

            // Buttons 0 thru 7 (except Button3) should have a background of Red+Blue
            SolidColorBrush background = (SolidColorBrush)Button0.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button1.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button2.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            // Button3 should have a background of Maroon+Navy+Aqua+Chocolate+Gold.
            background = (SolidColorBrush)Button3.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold})), errorMesg);

            background = (SolidColorBrush)Button4.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button5.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button6.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button7.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Red, Colors.Blue})), errorMesg);

            // Buttons 8 and 9 should have background Black + Blue
            background = (SolidColorBrush)Button8.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Blue})), errorMesg);

            background = (SolidColorBrush)Button9.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Blue})), errorMesg);

            // Buttons 10 and 11 should have Maroon + Navy + Green background
            background = (SolidColorBrush)Button10.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green})), errorMesg);

            background = (SolidColorBrush)Button11.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green})), errorMesg);

            // Button 12 should have Maroon + Navy + Aqua + Chocolate + Gold + Indigo
            SolidColorBrush brushContent = null;
            brushContent = (SolidColorBrush)Button12.Content;
            Assert(Color.Equals(brushContent.Color, MixColors(new Color[] { Colors.Maroon, Colors.Navy, Colors.Aqua, Colors.Chocolate, Colors.Gold, Colors.Indigo })), errorMesg);

            // Button 13 should have Maroon + Navy + Green
            brushContent = (SolidColorBrush)Button13.Content;
            Assert(Color.Equals(brushContent.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green})), errorMesg);                         
        }

        /// <summary>
        /// Verification routine for defArrayTest.xaml
        /// </summary>
        /// <param name="uie"></param>
        public static void ArrayExtensionVerify(UIElement uie)
        {
            String errorMesg = "ArrayExtension verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");

            // Verify the content property of Button
            Object[] contents = (Object[])Button0.GetValue(ContentControl.ContentProperty);
            Assert("Hello" == (contents[0] as TextBlock).Text, errorMesg);
            Assert("World" == (contents[1] as TextBlock).Text, errorMesg);
            Assert("foo" == (contents[2] as string), errorMesg);

            // Vefify the MyClass.MyBrushes attached property for Button
            //SolidColorBrush[] brushes = (SolidColorBrush[])Button0.GetValue(MyClass.MyBrushesProperty);
            //Assert(Color.Equals(brushes[0].Color, Colors.Red), errorMesg);
            //Assert(Color.Equals(brushes[1].Color, Colors.Green), errorMesg);
        }

        /// <summary>
        /// Verification routine for MarkupExtInStyle.xaml
        /// </summary>
        /// <param name="uie"></param>
        public static void MarkupExtInStyleVerify(UIElement uie)
        {
            String errorMesg = "MarkupExtInStyle verification failed";
            Button Button0 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button0");
            Button Button1 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button1");
            Button Button2 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button2");
            Button Button3 = (Button)LogicalTreeHelper.FindLogicalNode(uie, "Button3");

            // Verify properties for Button0
            SolidColorBrush background = (SolidColorBrush)Button0.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Red})), errorMesg);

            Double fontsize = (Double)Button0.GetValue(Control.FontSizeProperty);
            Assert(fontsize == 48.0, errorMesg);

            Dock dock = (Dock)Button0.GetValue(DockPanel.DockProperty);
            Assert(dock == Dock.Left, errorMesg);

            FontStyle fontstyle = (FontStyle)Button0.GetValue(Control.FontStyleProperty);
            Assert(fontstyle == FontStyles.Italic, errorMesg);

            String content = (String)Button0.GetValue(ContentControl.ContentProperty);
            Assert(content == "{Hello}", errorMesg);

            // Verify properties for Button1
            ControlTemplate template = (ControlTemplate)Button1.GetValue(Control.TemplateProperty);
            Canvas Canvas0 = (Canvas)template.FindName("Canvas0", Button1);
            background = (SolidColorBrush)Canvas0.GetValue(Panel.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Black, Colors.Red})), errorMesg);

            ContentPresenter CP0 = (ContentPresenter)template.FindName("CP0", Button1);
            content = (String)CP0.GetValue(ContentPresenter.ContentProperty);
            Assert(content == "{Hello}", errorMesg);

            // Verify properties for Button2
            SolidColorBrush foreground = (SolidColorBrush)Button2.GetValue(Control.ForegroundProperty);
            Assert(Color.Equals(foreground.Color, Colors.Blue), errorMesg);

            TextBlock[] contentArray = (TextBlock[])Button2.GetValue(ContentControl.ContentProperty);
            Assert("Text 0" == contentArray[0].Text, errorMesg);
            Assert("Text 1" == contentArray[1].Text, errorMesg);
            Assert("Text 2" == contentArray[2].Text, errorMesg);

            // Verify properties for Button3
            background = (SolidColorBrush)Button3.GetValue(Control.BackgroundProperty);
            Assert(Color.Equals(background.Color, MixColors(new Color[] {Colors.Maroon, Colors.Navy, Colors.Green, Colors.Indigo, Colors.Blue})), errorMesg);

            dock = (Dock)Button3.GetValue(DockPanel.DockProperty);
            Assert(dock == Dock.Left, errorMesg);
        }

        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest1Verify_NoSupportingAssemblies(UIElement uie)
        {
            StackPanel StackPanel0 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel0");
            StackPanel StackPanel1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel1");
            StackPanel StackPanel2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel2");
                        
            // Verify that all the StackPanels have no children
            VerifyNoChildren(StackPanel0);
            VerifyNoChildren(StackPanel1);
            VerifyNoChildren(StackPanel2);
            //VerifyNoChildren(StackPanel3);            
           
        }
        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest1Verify_V1(UIElement uie)
        {
            StackPanel StackPanel0 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel0");
            StackPanel StackPanel1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel1");
            StackPanel StackPanel2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel2");
            
            string transControlsNS = "Com.ControlStore.";
            string v1 = "ParserTestControlsV1";

            String errorMesg = "CompatReaderTest1 verification failed. V1 loaded, V2 not loaded.";

            //
            // Verify the tree for StackPanel0
            //
            // First child is a v1:TransButton with Background="Blue"
            // This child doesn't have any children
            FrameworkElement transButton1 = VerifyChildType(StackPanel0, 1, v1, transControlsNS + "TransButton");
            SolidColorBrush background = transButton1.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Blue), errorMesg);
            VerifyNoChildren(transButton1);

            // Second child should be a v1:TransButton, with no children
            FrameworkElement transButton2 = VerifyChildType(StackPanel0, 2, v1, transControlsNS + "TransButton");
            VerifyNoChildren(transButton2);

            // Third, fourth and fifth children should be v1:TransListBoxItems
            FrameworkElement transListBoxItem1 = VerifyChildType(StackPanel0, 3, v1, transControlsNS + "TransListBoxItem");
            FrameworkElement transListBoxItem2 = VerifyChildType(StackPanel0, 4, v1, transControlsNS + "TransListBoxItem");
            FrameworkElement transListBoxItem3 = VerifyChildType(StackPanel0, 5, v1, transControlsNS + "TransListBoxItem");

            // There should be no more children
            VerifyNoChild(StackPanel0, 6);

            //
            // Verify the tree for StackPanel1
            //
            // First child should be a v1:TransButton with no children
            FrameworkElement transButton = VerifyChildType(StackPanel1, 1, v1, transControlsNS + "TransButton");
            VerifyNoChildren(transButton);

            // Second child should be a v1:TransListBoxItem
            FrameworkElement transListBoxItem = VerifyChildType(StackPanel1, 2, v1, transControlsNS + "TransListBoxItem");

            // No More children
            VerifyNoChild(StackPanel1, 3);

            //
            // Vefiry the tree for StackPanel2
            //
            // First child should be a v1:TransButton
            transButton = VerifyChildType(StackPanel2, 1, v1, transControlsNS + "TransButton");

            // No more children
            VerifyNoChild(StackPanel2, 2);           
        }
        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest1Verify_V1V2(UIElement uie)
        {
            StackPanel StackPanel0 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel0");
            StackPanel StackPanel1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel1");
            StackPanel StackPanel2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel2");
           
            string transControlsNS = "Com.ControlStore.";
            string v1 = "ParserTestControlsV1";           
            String errorMesg = "CompatReaderTest1 verification failed. V1 loaded, V2 loaded, but V2 doesn't subsume V1.";
            string v2 = "ParserTestControlsV2";

            //
            // Verify the tree for StackPanel0
            //
            // First child is a v1:TransButton, which has a v2:TransButton child
            FrameworkElement transButton = VerifyChildType(StackPanel0, 1, v1, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v2, transControlsNS + "TransButton");

            // Second child should be a v2:TransButton, which has a v1:TransButton child
            transButton = VerifyChildType(StackPanel0, 2, v2, transControlsNS + "TransButton");
            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Blue), errorMesg);
            VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton");

            // Third child is a v2:TransListBox, with v1:TransListBoxItem children
            FrameworkElement transListBox = VerifyChildType(StackPanel0, 3, v2, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem");
            VerifyChildType(transListBox, 2, v1, transControlsNS + "TransListBoxItem");
            VerifyChildType(transListBox, 3, v1, transControlsNS + "TransListBoxItem");

            //
            // Verify the tree for StackPanel1
            //
            // First child should be a v2:TransButton, which has a v1:TransButton child
            transButton = VerifyChildType(StackPanel1, 1, v2, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton");

            // Second child is a v2:TransListBox, with a v1:TransListBoxItem child
            transListBox = VerifyChildType(StackPanel1, 2, v2, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem");

            //
            // Verify the tree for StackPanel2
            //
            // First child should be a v2:TransButton, which has a v1:TransButton child
            transButton = VerifyChildType(StackPanel2, 1, v2, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v1, transControlsNS + "TransButton");

            // Second child is a v2:TransListBox, with a v1:TransListBoxItem child
            transListBox = VerifyChildType(StackPanel2, 2, v2, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v1, transControlsNS + "TransListBoxItem");

            
        }
        /// <summary>
        /// Verification routine for CompatReaderTest1.xaml,
        /// which tests different aspects of Markup-compatibility attributes, such
        /// as mc:Ignorable and mc:ProcessContent
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest1Verify_Other(UIElement uie)
        {
            StackPanel StackPanel0 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel0");
            StackPanel StackPanel1 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel1");
            StackPanel StackPanel2 = (StackPanel)LogicalTreeHelper.FindLogicalNode(uie, "StackPanel2");
            
            string transControlsNS = "Com.ControlStore.";
            String errorMesg = "CompatReaderTest1 verification failed. V1 loaded, V2 loaded (and V2 subsumes V1), V3 loaded (and V3 subsumes V2).";
            string v3 = "ParserTestControlsV3SubsumingV2";

            //
            // Verify the tree for StackPanel0
            //
            // First child is a v3:TransButton, which has a v3:TransButton child
            FrameworkElement transButton = VerifyChildType(StackPanel0, 1, v3, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v3, transControlsNS + "TransButton");

            // Second child should be a v3:TransButton, which has a v3:TransButton child
            transButton = VerifyChildType(StackPanel0, 2, v3, transControlsNS + "TransButton");
            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Blue), errorMesg);
            VerifyChildType(transButton, 1, v3, transControlsNS + "TransButton");

            // Third child is a v3:TransListBox, with v3:TransListBoxItem children
            FrameworkElement transListBox = VerifyChildType(StackPanel0, 3, v3, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v3, transControlsNS + "TransListBoxItem");
            VerifyChildType(transListBox, 2, v3, transControlsNS + "TransListBoxItem");
            VerifyChildType(transListBox, 3, v3, transControlsNS + "TransListBoxItem");

            //
            // Verify the tree for StackPanel1
            //
            // First child should be a v3:TransButton, which has a v3:TransButton child
            transButton = VerifyChildType(StackPanel1, 1, v3, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v3, transControlsNS + "TransButton");

            // Second child is a v3:TransListBox, with a v3:TransListBoxItem child
            transListBox = VerifyChildType(StackPanel1, 2, v3, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v3, transControlsNS + "TransListBoxItem");

            //
            // Verify the tree for StackPanel2
            //
            // First child should be a v3:TransButton, which has a v3:TransButton child
            transButton = VerifyChildType(StackPanel2, 1, v3, transControlsNS + "TransButton");
            VerifyChildType(transButton, 1, v3, transControlsNS + "TransButton");

            // Second child is a v3:TransListBox, with a v3:TransListBoxItem child
            transListBox = VerifyChildType(StackPanel2, 2, v3, transControlsNS + "TransListBox");
            VerifyChildType(transListBox, 1, v3, transControlsNS + "TransListBoxItem");            
        }

        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest2Verify_NoSupportingAssemblies(UIElement uie)
        {
            CustomStackPanel CustomStackPanel0 = (CustomStackPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomStackPanel0");
            
            VerifyNoChildren(CustomStackPanel0);
            
        }
        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest2Verify_V1(UIElement uie)
        {
            CustomStackPanel CustomStackPanel0 = (CustomStackPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomStackPanel0");

            string transControlsNS = "Com.ControlStore.";
            string v1 = "ParserTestControlsV1";

            String errorMesg = "CompatReaderTest2 verification failed. V1 loaded, V2 not loaded.";
            // First child should be a v1:TransButton, with Content="Hello"
            // and no children except "Hello"
            FrameworkElement transButton = VerifyChildType(CustomStackPanel0, 1, v1, transControlsNS + "TransButton");
            string content = (string)transButton.GetValue(ContentControl.ContentProperty);
            Assert("Hello" == content, errorMesg);
            // "Hello" is also the first logical child. There shouldn't be any 
            // more logical children.
            VerifyNoChild(transButton, 2);

            // There should be no more children
            VerifyNoChild(CustomStackPanel0, 2);
           
        }
        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest2Verify_V1V2(UIElement uie)
        {
            CustomStackPanel CustomStackPanel0 = (CustomStackPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomStackPanel0");

            string transControlsNS = "Com.ControlStore.";          
            String errorMesg = "CompatReaderTest2 verification failed. V1 loaded, V2 loaded, but V2 doesn't subsume V1.";
            string v2 = "ParserTestControlsV2";

            // The only child should be v2:TransButton with a Red background
            // and Content="This is a v2 button"
            FrameworkElement transButton = VerifyChildType(CustomStackPanel0, 1, v2, transControlsNS + "TransButton");

            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Red), errorMesg);

            string content = (string)transButton.GetValue(ContentControl.ContentProperty);
            Assert("This is a v2 button" == content, errorMesg);

            // No more children
            VerifyNoChild(CustomStackPanel0, 2);            
        }
        /// <summary>
        /// Verification routine for CompatReaderTest2.xaml, 
        /// which tests different aspects of AlternateContent, a 
        /// Markup compatibility tag.
        /// </summary>
        /// <param name="uie"></param>
        public static void CompatReaderTest2Verify_Other(UIElement uie)
        {
            CustomStackPanel CustomStackPanel0 = (CustomStackPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomStackPanel0");

            string transControlsNS = "Com.ControlStore.";          
            String errorMesg = "CompatReaderTest2 verification failed. V1 loaded, V2 loaded, and V2 subsumes V1.";
            string v2 = "ParserTestControlsV2SubsumingV1";

            // The only child should be v2:TransButton with a Red background
            // and Content="This is a v2 button"
            FrameworkElement transButton = VerifyChildType(CustomStackPanel0, 1, v2, transControlsNS + "TransButton");

            SolidColorBrush background = transButton.GetValue(Control.BackgroundProperty) as SolidColorBrush;
            Assert(Color.Equals(background.Color, Colors.Red), errorMesg);

            string content = (string)transButton.GetValue(ContentControl.ContentProperty);
            Assert("This is a v2 button" == content, errorMesg);

            // No more children
            VerifyNoChild(CustomStackPanel0, 2);            
        }


        /// <summary>
        /// Verification routine for XmlnsCacheTest.xaml
        /// </summary>
        /// <param name="uie"></param>
        public static void XmlnsCacheTestVerify(UIElement uie)
        {
            CustomStackPanel CustomStackPanel0 = (CustomStackPanel)LogicalTreeHelper.FindLogicalNode(uie, "CustomStackPanel0");
            VerifyChildType(CustomStackPanel0, 1, "ParserTestControlsFixedSize", "Com.ControlStore.FixedSizeTransButton");
            VerifyNoChild(CustomStackPanel0, 2);
        }

        // Verify that given element doesn't have any logical children
        private static void VerifyNoChildren(FrameworkElement element)
        {
            VerifyNoChild(element, 1);
        }

        // Verify that 'n'th logical child doesn't exist for given element
        // i.e. given element has less than 'n' logical children
        // NOTE: 'n' starts with 1, and not with 0. 
        private static void VerifyNoChild(FrameworkElement element, int n)
        {
            String errorMesg = "VerifyNoChild failed. " + n + "th child exists for " + element.GetType().Name;
            IEnumerator children = LogicalTreeHelper.GetChildren(element).GetEnumerator();
            int i = 0;
            for (i = 0; i < n; i++)
            {
                if (!children.MoveNext()) break;
            }

            // We want to verify that element doesn't have n logical children.
            // If that's true, MoveNext() would return false somewhere in the above loop,
            // hence we would break out of the loop, so i wouldn't reach the value of n
            Assert(i < n, errorMesg);
        }

        // Verify that the assembly name and full type name of 'n'th logical child of
        // element match the assembly name and full type name provided.
        // NOTE: 'n' starts with 1, and not with 0. 
        private static FrameworkElement VerifyChildType(FrameworkElement element, int n, string assemblyName, string typeFullName)
        {
            String errorMesg = "VerifyChild failed for " + n + "th child of " + element.GetType().Name;
            IEnumerator children = LogicalTreeHelper.GetChildren(element).GetEnumerator();
            for (int i = 0; i < n; i++)
            {
                children.MoveNext();
            }
            object child = children.Current;
            Type type = child.GetType();
            // Verify the type name
            Assert(typeFullName == type.FullName, errorMesg);
            // Verify the assembly
            Assert(assemblyName == type.Assembly.GetName().Name, errorMesg);
            return (child as FrameworkElement);
        }

        /// <summary>
        /// Verification routine for XmlContent.xaml.
        /// See comments in XmlContent.xaml to learn what this is about.
        /// </summary>
        /// <param name="uie"></param>
        public static void XmlContentVerify(UIElement uie)
        {
            string errorMesg = "XmlContent verification failed.";
            XmlContentControl XmlContentControl0 = (XmlContentControl)LogicalTreeHelper.FindLogicalNode(uie, "XmlContentControl0");
            string content = XmlContentControl0.Content.ToString();
            string expectedContent = "<Foo xmlns=\"\">\n      <Bar />\n    </Foo>";
            Assert(content == expectedContent, errorMesg + @" Expected value                     
                of XmlContentControl0.Content.ToString(): " + expectedContent +
                "   Actual value: " + content);
        }

        /// <summary>
        /// Verification routine for Xaml content model (object content under objects).
        /// Look at framework\bvt\parser\ContentModel.cs for more info
        /// </summary>
        /// <param name="uie"></param>
        public static void ObjectContentUnderObjectsVerify(UIElement uie)
        {
            string errorMesg = "ObjectContentUnderObjects verification failed.";

            // Load the state persisted by the code that generated the Xaml
            ContentModelState _modelState = ContentModelState.Load() as ContentModelState;

            // Get info about how the tree should look like
            string Parent_object = _modelState.Parent_object;
            string[] Content_objects = _modelState.Content_objects;

            string Parent_assembly = "CoreTestsUntrusted";
            string Parent_ns = "Avalon.Test.CoreUI.Parser.";
            string Content_ns = "System.Windows.Controls.";

            // Verify that the expected parent object is the child of the root.
            CustomPage root = uie as CustomPage;
            VerifyChildType(root, 1, Parent_assembly, Parent_ns + Parent_object);

            // Get the parent object
            IEnumerator rootChildren = LogicalTreeHelper.GetChildren(root).GetEnumerator();
            rootChildren.MoveNext();
            object parent = rootChildren.Current;
            Type parentType = parent.GetType();

            // Get the children.
            ArrayList children = (ArrayList)parentType.InvokeMember("Children", BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                null, parent, null);
            // Verify the number and types of children 
            Assert(children.Count == Content_objects.Length, errorMesg + " " + Parent_object +
                " was expected to have " + Content_objects.Length + " children, but actually has "
                + children.Count + " children.");
            for (int i = 0; i < Content_objects.Length; i++)
            {
                string expectedTypeName = Content_ns + Content_objects[i];
                string actualTypeName = children[i].GetType().FullName;
                Assert(actualTypeName == expectedTypeName,
                    errorMesg + " " + i + "th child of " + Parent_object + " was expected to be of type "
                    + expectedTypeName + ", but actually was of type " + actualTypeName);
            }
        }

        /// <summary>
        /// Verification routine for Xaml content model (object content
        /// under compound properties).
        /// Look at framework\bvt\parser\ContentModel.cs for more info
        /// </summary>
        /// <param name="uie"></param>
        public static void ObjectContentUnderPropertiesVerify(UIElement uie)
        {
            // Load the state persisted by the code that generated the Xaml
            ContentModelState _modelState = ContentModelState.Load() as ContentModelState;

            // Get info about the state of the model. This determines how the tree 
            // should look like.
            string Parent_object = _modelState.Parent_object;
            string[] Content_objects = _modelState.Content_objects;
            bool isAttached = (bool)_modelState.Dictionary["isAttached"];
            string propertyName = _modelState.Dictionary["propertyName"] as string;
            string className = _modelState.Dictionary["className"] as string;
            string qualifiedPropertyName = _modelState.Dictionary["qualifiedPropertyName"] as string;
            string Type_of_property = _modelState.Dictionary["Type of property"] as string;
            string Object_tag_under_property = _modelState.Dictionary["Object tag under property"] as string;
            string Property_readability = _modelState.Dictionary["Property readability"] as string;
            string Stored_property_value = _modelState.Dictionary["Stored property value"] as string;

            // Decide whether it's a RO, Non-null return value, Explicit tag scenario.
            // If yes, it's a List-of-lists or Dictionary-of-dictionaries, and has to be
            // treated specially.
            bool RO_NonNull_Explicit_Scenario = false;
            if((Property_readability == "RO") && 
                (Stored_property_value == "NonNull") &&
                (Object_tag_under_property == "Explicit"))            
            {
                RO_NonNull_Explicit_Scenario = true;
            }

            string errorMesg = "ObjectContentUnderProperties verification failed. "
                    + "Object tag under property was: " + Object_tag_under_property;

            string Parent_assembly = "CoreTestsUntrusted";
            string Parent_ns = "Avalon.Test.CoreUI.Parser.";
            string Content_ns = "System.Windows.Controls.";

            // Verify that the expected parent object is the child of the root.
            CustomPage root = uie as CustomPage;
            VerifyChildType(root, 1, Parent_assembly, Parent_ns + Parent_object);

            // Get the parent object
            IEnumerator rootChildren = LogicalTreeHelper.GetChildren(root).GetEnumerator();
            rootChildren.MoveNext();
            object parent = rootChildren.Current;
            Type parentType = parent.GetType();

            // Get the property value.
            // For "normal" (non-attached) properties, get the property value directly 
            //   using reflection.
            // For attached properties, get it using the static Get method.
            object propertyValue = null;
            if (isAttached)
            {
                Type attacherType = null;
                if (className == "Custom_ClrProp_Attacher")
                {
                    attacherType = typeof(Custom_ClrProp_Attacher);
                }
                else if (className == "Custom_DP_Attacher")
                {
                    attacherType = typeof(Custom_DP_Attacher);
                }

                propertyValue = attacherType.InvokeMember("Get" + propertyName,
                    BindingFlags.Public | BindingFlags.Static | BindingFlags.InvokeMethod,
                    null, null, new object[1] { parent });
            }
            else
            {
                propertyValue = parentType.InvokeMember(propertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.GetProperty,
                    null, parent, null);
            }

            Assert(propertyValue != null, errorMesg + " Property " + qualifiedPropertyName
                + " on " + Parent_object + " has a null value.");

            // Populate the children list from the property value.
            ArrayList children = new ArrayList();
            switch (Type_of_property)
            {
                case "Singular":
                    children.Add(propertyValue);
                    break;

                // In all the following cases, the returned property value should be
                // of a type that implement IList. So we will use IList to get the values
                // from the collection.
                case "Array":
                case "IList":
                case "IListActualType":
                case "IListRestrictsTypes":
                    if (Type_of_property == "Array")
                    {
                        Assert(propertyValue is Array,
                            errorMesg + " Property " + qualifiedPropertyName + " on "
                            + Parent_object + " was expected to be an array, but it is of type "
                            + propertyValue.GetType());
                    }
                    IList propertyValueAsIList = propertyValue as IList;
                    if (RO_NonNull_Explicit_Scenario) // list-of-lists scenario
                    {
                        Assert(propertyValueAsIList.Count == 1,
                            errorMesg + " Property " + qualifiedPropertyName + " on "
                            + Parent_object + " was expected to contain only one child, since "
                            + " it's a list-of-lists scenario, but it has "
                            + propertyValueAsIList.Count + " children.");
                        // First child of the property value is going to contain the actual children.
                        propertyValueAsIList = propertyValueAsIList[0] as IList;
                    }
                    for (int i = 0; i < propertyValueAsIList.Count; i++)
                    {
                        children.Add(propertyValueAsIList[i]);
                    }
                    break;

                // In all the following cases, the returned property value should be
                // of a type that implement IDictionary. So we will use IDictionary to 
                // get the values from the collection.
                case "IDictionary":
                case "IDictionaryActualType":
                case "IDictionaryRestrictsTypes":
                    IDictionary propertyValueAsIDict = propertyValue as IDictionary;
                    if (RO_NonNull_Explicit_Scenario) // dictionary-of-dictionaries scenario
                    {
                        Assert(propertyValueAsIDict.Count == 1,
                            errorMesg + " Property " + qualifiedPropertyName + " on "
                            + Parent_object + " was expected to contain only one child, since "
                            + " it's a dictionary-of-dictionaries scenario, but it has "
                            + propertyValueAsIDict.Count + " children.");
                        // First child of the property value is going to contain the actual children.
                        propertyValueAsIDict = propertyValueAsIDict["key0"] as IDictionary;
                    }
                    for (int i = 0; i < propertyValueAsIDict.Count; i++)
                    {
                        children.Add(propertyValueAsIDict["key" + i]);
                    }
                    break;
            }

            // Verify the number and types of children 
            Assert(children.Count == Content_objects.Length,
                errorMesg + " Property " + qualifiedPropertyName + " on "
                + Parent_object + " was expected to have " + Content_objects.Length
                + " objects under it, but actually has " + children.Count
                + " objects under it.");

            for (int i = 0; i < Content_objects.Length; i++)
            {
                string expectedTypeName = Content_ns + Content_objects[i];
                string actualTypeName = children[i].GetType().FullName;
                Assert(actualTypeName == expectedTypeName,
                    errorMesg + " " + i + "th object under property "
                    + qualifiedPropertyName + " on "
                    + Parent_object + " was expected to be of type "
                    + expectedTypeName + ", but actually was of type " + actualTypeName);
            }
        }

        /// <summary>
        /// Verify routine for ItemsControlTest.xaml
        /// </summary>
        /// <param name="root"></param>
        public static void ItemsControlSerializationVerify(UIElement root)
        {

            CoreLogger.LogStatus("Inside ParserVerifier.ItemsControlTestVerify()...");

            CoreLogger.LogStatus("Verify that ItemsControl can be found.");
            ItemsControl control = (ItemsControl)IdTestBaseCase.FindElementWithId(root, "ItemsControl");
            VerifyElement.VerifyBool(null != control, true);

            CoreLogger.LogStatus("Verify that ItemsControl has 4 items.");
            ItemCollection items = control.Items;
            VerifyElement.VerifyInt(items.Count, 4);
        }

        /// <summary>
        /// Verify xaml ImmutableTypeVerification.xaml.
        /// </summary>
        /// <param name="root"></param>
        public static void ImmutableTypeVerify(UIElement root)
        {
            CoreLogger.LogStatus("Inside ParserVerifier.DoubleConverterVerify()...");
            CoreLogger.LogStatus("Verify that FrameworkElement can be found.");
            FrameworkElement fe = (FrameworkElement)IdTestBaseCase.FindElementWithId(root, "frameworkElement");
            VerifyElement.VerifyBool(null != fe, true);
            //Verify Double.
            VerifyElement.VerifyBool(Double.NaN.Equals(fe.Width), true);
            VerifyElement.VerifyBool(Double.NaN.Equals(fe.Height), true);

            Button button1 = (FrameworkElement)IdTestBaseCase.FindElementWithId(root, "button1") as Button;
            VerifyElement.VerifyBool(null != button1, true);
            VerifyElement.VerifyBool(Double.NaN.Equals(button1.Width), true);
            VerifyElement.VerifyBool(Double.NaN.Equals(button1.Height), true);

            Button button2 = (FrameworkElement)IdTestBaseCase.FindElementWithId(root, "button2") as Button;
            VerifyElement.VerifyBool(null != button2, true);
            VerifyElement.VerifyDouble(button2.Width, 5.0);
            VerifyElement.VerifyDouble(button2.Height, 6.0);

            //Verify string.
            string stringVal = (string)fe.GetValue(FrameworkElementWithimmutableProperties.StringDPProperty);
            VerifyElement.VerifyString(stringVal, "String value");
            stringVal = ((FrameworkElementWithimmutableProperties)fe).ClrString;
            VerifyElement.VerifyString(stringVal, "clr String value");

            //Verify Int32
            int intVal = (int)fe.GetValue(FrameworkElementWithimmutableProperties.Int32DPProperty);
            VerifyElement.VerifyInt(intVal, 32);
            intVal = ((FrameworkElementWithimmutableProperties)fe).ClrInt32;
            VerifyElement.VerifyInt(intVal, 322);
        }



        /// <summary>
        /// WSCollapseToEmptyVerify ensures that all string-content-capable uielements equals Null
        /// </summary>
        /// <param name="uie">logical tree to inspect</param>
        public static void WSCollapseToNullVerify(UIElement uie)
        {
            String testTargetString = null;

            String errorMesg = "WhiteSpaceCollapsing to empty string - verification failed";
            WSCollapseVerify(uie, testTargetString, errorMesg);
            WSCollapseGenericVerify(uie);
        }

        /// <summary>
        /// WSCollapseToAbcVerify ensures that all string-content-capable uielements equals "abc"
        /// </summary>
        /// <param name="uie">logical tree to inspect</param>
        public static void WSCollapseToAbcVerify(UIElement uie)
        {
            String testTargetString = "abc";
            String errorMesg = "WhiteSpaceCollapsing to abc string - verification failed: ";
            WSCollapseVerify(uie, testTargetString, errorMesg);
            WSCollapseGenericVerify(uie);
        }

        /// <summary>
        /// Verifier for correct parsing/serialization of ContentWrapperAttribute.
        /// </summary>
        public static void ContentWrapperVerify(UIElement uie)
        {
            CoreLogger.LogStatus("Inside ParserVerifier.ContentWrapperVerify()...");

            Custom_DO_With_GenericCollection_Properties customElement 
                = TreeHelper.FindNodeById(uie, "customElement") as Custom_DO_With_GenericCollection_Properties;

            if (customElement == null)
            {
                throw new Microsoft.Test.TestValidationException("Could not find custom element.");
            }

            if (customElement.CustomGenericCollection.Count != 1)
            {
                throw new Microsoft.Test.TestValidationException("Custom element does not contain exactly 1 child.");
            }

            if (!(customElement.CustomGenericCollection[0] is Run))
            {
                throw new Microsoft.Test.TestValidationException("Custom element does not contain a Run child.");
            }
        }

        /// <summary>
        /// WSCollapseCheck ensures that:
        /// 1. All pure WS strings are at most length `1.
        /// 2. All First and Last Siblings are left trimmed and right trimmed respectively if they contain text.
        /// 3. Siblings on both sides of a TrimSurroundingWhiteSpace-sibling are trimmed on the side adjacent 
        ///    to the TrimSurroundWhiteSpace-sibling
        /// 4. Text-containing siblings in a non-WhitespaceSignificantCollection are trimmed on both ends
        /// 
        /// WSCollapseCheck assumes that it is working with an xmlSpace=default setting. (NOT preserve)
        /// </summary>
        /// <param name="obj">logical tree to inspect</param>
        public static void WSCollapseGenericVerify(object obj)
        {
            Boolean sigCollectionAttrib = ContentPropertyHasAttribute(obj, "System.Windows.Markup.WhitespaceSignificantCollectionAttribute");
            IEnumerator logicalChildrenEnumerator = GetIEnumeratorForObject(obj);
            if (null != logicalChildrenEnumerator)
            {

                int count = 0;
                String childString = null;
                String prevString = null;
                char[] whitespaceTrimChars = " \t\n".ToCharArray();
                Boolean trimWSAttrib = false;
                Boolean prevTrimWSAttrib = false;
                int paddingForOutputAlignment = 50;

                while (logicalChildrenEnumerator.MoveNext())
                {
                    object child = logicalChildrenEnumerator.Current;
                    prevTrimWSAttrib = trimWSAttrib;
                    trimWSAttrib = HasAttribute(child, "System.Windows.Markup.TrimSurroundingWhitespaceAttribute");
                    prevString = childString;
                    childString = GetStringFromControl(child);

                    if (null != childString)
                    {
                        CoreLogger.LogStatus(child.GetType().ToString().PadRight(paddingForOutputAlignment) + "|" + childString + "|");
                        if (0 == count) // more rigorous test for first sibling
                        {
                            Assert(childString.Equals(childString.TrimStart(whitespaceTrimChars)),
                                    "Too much whitespace at start of first child! :" + childString);
                        }
                        EnsureWhitespaceCollapsedMiddleSiblings(childString, prevTrimWSAttrib, sigCollectionAttrib, whitespaceTrimChars);
                    }

                    if ((trimWSAttrib) && (null != prevString))
                    {
                        Assert(prevString.Equals(prevString.TrimEnd(whitespaceTrimChars)),
                                                    "TrimWS Attribute not enforced: " + prevString);
                    }
                    WSCollapseGenericVerify(child);
                    count++;
                }
                // more rigorous test for last sibling
                if (null != childString)
                {
                    Assert(childString.Equals(childString.TrimEnd(whitespaceTrimChars)),
                            "Too much whitespace at end of last child! :" + childString);
                }
            }
        }

        /// <summary>
        /// Verifies parser can handle markup extensions with a '.' in a property name 


        public static void MarkupExtDotPropertyNameVerify(UIElement uie)
        {
            CustomStackPanel root = TreeHelper.FindNodeById(uie, "Root") as CustomStackPanel;
            TextBox textBox = TreeHelper.FindNodeById(uie, "TB1") as TextBox;
            Binding bind = BindingOperations.GetBinding(textBox, TextBox.TextProperty);
            if (bind.NotifyOnTargetUpdated != true)
            {
                throw new Microsoft.Test.TestValidationException("NotifyOnTargetUpdated was false");
            }
            if (bind.NotifyOnSourceUpdated != false)
            {
                throw new Microsoft.Test.TestValidationException("NotifyOnSourceUpdated was true");
            }
            if (textBox.Text != root.Orientation.ToString())
            {
                GlobalLog.LogEvidence("TextBox.Text was incorrect");
                GlobalLog.LogEvidence("Expected: " + root.Orientation.ToString());
                GlobalLog.LogEvidence("Found: " + textBox.Text);
                throw new Microsoft.Test.TestValidationException("TextBox.Text was incorrect");
            }
        }

        /// <summary>
        /// Verifies handling of the NetFX/2007 Namespace
        /// </summary>
        /// <param name="uie"></param>
        public static void NetFX2007NSVerify(UIElement uie)
        {
            Grid grid1 = TreeHelper.FindNodeById(uie, "Grid1") as Grid;
            Grid grid2 = TreeHelper.FindNodeById(uie, "Grid2") as Grid;
            Grid grid3 = TreeHelper.FindNodeById(uie, "Grid3") as Grid;
            Grid grid4 = TreeHelper.FindNodeById(uie, "Grid4") as Grid;

            if ((((grid1.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox1")
            {
                throw new Microsoft.Test.TestValidationException("Did not find expected text: RichTextBox1");
            }
            if ((((grid2.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox2")
            {
                throw new Microsoft.Test.TestValidationException("Did not find expected text: RichTextBox2");
            }
            if ((((grid3.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox3")
            {
                throw new Microsoft.Test.TestValidationException("Did not find expected text: RichTextBox3");
            }
            if ((((grid4.Children[0] as RichTextBox).Document.Blocks.FirstBlock as Paragraph).Inlines.FirstInline as Run).Text != "RichTextBox4")
            {
                throw new Microsoft.Test.TestValidationException("Did not find expected text: RichTextBox4");
            }

        }


        /// <summary>
        /// EnsureWhitespaceCollapsedMiddleSiblings inspects the strCollapsed string
        /// to see if it meets xmlSpace=default Whitespace collapsing criteria.
        /// </summary>
        /// <param name="strCollapsed">String to be inspected for meeting criteria.  </param>
        /// <param name="prevSiblingHadTrimWS">Boolean that tells if previous sibling wants to trim this string.  </param>
        /// <param name="sigCollectionAttrib">Boolean that tells if whitespace is significant in strCollapsed's parent collection.  </param>
        /// <param name="whitespaceTrimChars">Char array that contains all acceptable characters to trim as whitespace.  </param>
        private static void EnsureWhitespaceCollapsedMiddleSiblings(String strCollapsed,
                                                                        Boolean prevSiblingHadTrimWS,
                                                                        Boolean sigCollectionAttrib,
                                                                        char[] whitespaceTrimChars)
        {
            // ensure that childUieString doesn't have more than a single whitespace on either side
            int strCollapsedLength = strCollapsed.Length;
            String strCollapsedTrimStart = strCollapsed.TrimStart(whitespaceTrimChars);
            String strCollapsedTrimEnd = strCollapsed.TrimEnd(whitespaceTrimChars);
            int strCollapsedTrimStartLength = strCollapsedTrimStart.Length;
            int strCollapsedTrimEndLength = strCollapsedTrimEnd.Length;
            // also takes care of condition: all purewhitespacestring.length == 1
            // how can we be sure we didn't trim too much?  can't ...
            // don't know if original string had 0 length string or not ...
            int tolerance = 1;
            if (!sigCollectionAttrib)
            {
                tolerance = 0; // whenever text abuts non-text in the collection, get trimming
            }
            Assert((strCollapsedLength - strCollapsedTrimStartLength) <= tolerance,
                    "Too much whitespace at start of string! :" + strCollapsed);
            Assert((strCollapsedLength - strCollapsedTrimEndLength) <= tolerance,
                    "Too much whitespace at end of string! :" + strCollapsed);


            if (prevSiblingHadTrimWS)// Add TrimWS checks
            {
                Assert(0 == (strCollapsedLength - strCollapsedTrimStartLength),
                        "Too much whitespace at start of first child! :" + strCollapsed);
            }
        }


        /// <summary>
        /// GetIEnumeratorForObject tries to return an enumerator for the argument element's children
        /// </summary>
        /// <param name="element">element that will be inspected for children that can be enumerated.  </param>
        private static IEnumerator GetIEnumeratorForObject(object element)
        {
            IEnumerable logicalChildrenEnumerable = null;
            IEnumerator logicalChildrenEnumerator = null;
            if (element is DependencyObject)
            {   
                logicalChildrenEnumerable = LogicalTreeHelper.GetChildren((DependencyObject) element);
                logicalChildrenEnumerator = logicalChildrenEnumerable.GetEnumerator();
            }
            return logicalChildrenEnumerator;
        }



        /// <summary>
        /// HasAttribute ensures that the element's type has the 
        /// attribute given by attributeName
        /// </summary>
        /// <param name="element">element that will have its type inspected.  </param>
        /// <param name="attributeName">string that represents the Attribute the element's type should have</param>
        private static Boolean HasAttribute(object element, string attributeName)
        {
            object[] elementAttributes = element.GetType().GetCustomAttributes(true);
            foreach (object attrib in elementAttributes)
            {
                if (attrib.GetType().ToString().Equals(attributeName))
                {
                    return true;
                }
            }
            return false;
        }


        /// <summary>
        /// ContentPropertyHasAttribute ensures that the element's contentProperty's type has the 
        /// attribute given by attributeName
        /// </summary>
        /// <param name="element">element whose contentProperty is to be inspected.  </param>
        /// <param name="attributeName">string that represents the Attribute contentProperty's type should have</param>
        private static Boolean ContentPropertyHasAttribute(object element, string attributeName)
        {
            object[] elementAttributes = element.GetType().GetCustomAttributes(true);
            String contentPropertyName = null;

            foreach (object attrib in elementAttributes)
            {
                if (attrib.GetType().ToString().Equals("System.Windows.Markup.ContentPropertyAttribute"))
                {
                    contentPropertyName = ((System.Windows.Markup.ContentPropertyAttribute)attrib).Name;
                }
            }
            if (null != contentPropertyName)
            {
                PropertyInfo pInfo = element.GetType().GetProperty(contentPropertyName);
                object[] contentPropertyAttibutes = pInfo.PropertyType.GetCustomAttributes(true);
                foreach (object contentAttrib in contentPropertyAttibutes)
                {
                    if (contentAttrib.GetType().ToString().Equals(attributeName))
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        /// <summary>
        /// WSCollapseVerify ensures that all string-content-capable uielements have text that equals testTargetString
        /// Only Checks one deep (really only set up for buttons in a CustomDockPanel for now).
        /// </summary>
        /// <param name="uie">logical tree to inspect</param>
        /// <param name="testTargetString">string that uielements with text-content should match</param>
        /// <param name="errorMesg">error message for when string in uielement doesn't match testTargetString \</param>
        private static void WSCollapseVerify(UIElement uie, String testTargetString, String errorMesg)
        {

            IEnumerable logicalChildrenEnumerable = LogicalTreeHelper.GetChildren(uie);
            IEnumerator logicalChildrenEnumerator = logicalChildrenEnumerable.GetEnumerator();
            while (logicalChildrenEnumerator.MoveNext())
            {
                object child = logicalChildrenEnumerator.Current;
                String childString = GetStringFromControl(child);

                Assert((((null == childString) && (null == testTargetString)) || (childString.Equals(testTargetString))),
                        errorMesg + childString + " not equals " + testTargetString);
               
            }
        }

        /// <summary>
        /// GetStringFromControl returns string content in a Control if possible.  Otherwise null.
        /// </summary>
        /// <param name="obj">control to return string from </param>
        private static String GetStringFromControl(object obj)
        {
            ContentControl objAsContentControl = obj as ContentControl;
            if (null != objAsContentControl)
            {
                return objAsContentControl.Content as String;
            }

            Run run = obj as Run;
            if (null != run)
            {
                return run.Text;
            }

            InlineUIContainer container = obj as InlineUIContainer;
            if (null != container)
            {
                return GetStringFromControl(container.Child);
            }

            return null;
        }

        private static Color MixColors(Color[] colors)
        {
            Color result = colors[0];
            for (int i = 1; i < colors.Length; i++)
            {
                result = result + colors[i];
            }

            if (result.ScA > 1.0f)
            { 
                result.ScA = 1.0f; 
            }

            return result;
        }
    }

}

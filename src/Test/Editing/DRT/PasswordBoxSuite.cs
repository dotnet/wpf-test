// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using System.Runtime.InteropServices;
using System.Threading;

using System.Collections;
using System.Security;
using System.Windows;
using System.Windows.Input;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;
using System.Windows.Media;
using System.Windows.Markup;
using System.Windows.Shapes;

namespace DRT
{
    internal class PasswordBoxSuite : DrtTestSuite
    {
        // Constructor.
        internal PasswordBoxSuite() : base("PasswordBox")
        {
        }

        private UIElement CreateTree()
        {
            Canvas canvas = new Canvas();
            _textbox = new PasswordBox();
            Canvas.SetLeft(_textbox, 5);
            Canvas.SetTop(_textbox, 5);
            _textbox.Width = 200;
            _textbox.Height = 200;

            canvas.Children.Add(_textbox);

            return canvas;
        }

        // Initialize tests.
        public override DrtTest[] PrepareTests()
        {
            DRT.Show(CreateTree());

            DRT.ShowRoot();
            // Return the lists of tests to run against the tree
            return new DrtTest[]{
                new DrtTest(ClickTextBox),
                new DrtTest(TestClear),
                new DrtTest(TestMaxLength),
                new DrtTest(TestMaxLengthLimited),
                new DrtTest(TestMaxLengthUnlimited),
                new DrtTest(TestPasteCommand),
                new DrtTest(TestPasteCommandCheck),
                new DrtTest(TestText),
                new DrtTest(TestHitTest),
                new DrtTest(TestStyleChange),
                new DrtTest(TestSelectionProperty),
            };
        }

        /// <summary>
        /// Checks that the specified condition is true.
        /// </summary>
        /// <param name='value'>Condition value.</param>
        /// <param name='description'>Description of evaluated condition</param>
        private void AssertCondition(bool value, string description)
        {
            if (!value)
            {
                string s = String.Format(
                    "Expected condition [{0}] does not hold", description);
                throw new Exception(s);
            }
        }

        /// <summary>
        /// Verifies that a given object has the expected value.
        /// </summary>
        /// <param name='value'>Actual value.</param>
        /// <param name='expected'>Expected value.</param>
        /// <param name='message'>An optional descriptive message.</param>
        private void AssertEqual(object value, object expected,
            string message)
        {
            bool condition = value.Equals(expected);
            string description = String.Format(
                "Expected [{0}] got [{1}] {2}", expected, value, message);
            if (!condition)
            {
                throw new Exception(description);
            }
        }

        /// <summary>
        /// Checks that the second parameter passed is greater than the first.
        /// </summary>
        /// <param name='lesserValue'>Value expected to be smaller.</param>
        /// <param name='greaterValue'>Value expected to be larger.</param>
        /// <param name='message'>An descriptive message of the condition tested.</param>
        private void AssertSecondGreater(double lesserValue, double greaterValue,
            string message)
        {
            if (lesserValue >= greaterValue)
            {
                string s = String.Format(
                    "Expected condition [{0} < {1}] does not hold {2}",
                    lesserValue, greaterValue, message);
                throw new Exception(s);
            }
        }

        /// <summary>
        /// Verifies that the previously SelectedText is same as clipbaord
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckClipboardText(string expectedText)
        {
            IDataObject dataObj = Clipboard.GetDataObject();
            if (dataObj != null)
            {
                object data = dataObj.GetData(DataFormats.Text);
                if (data != null && data is string)
                {
                    AssertEqual(data as string , expectedText, "Verifying ClipboardText");
                }
                else
                    AssertEqual(false , expectedText, "Verifying ClipboardText");
            }
        }

        /// <summary>
        /// Verifies that the Text property has the expected value.
        /// </summary>
        /// <param name='expectedText'>Expected value.</param>
        private void CheckText(string expectedText)
        {
            AssertEqual(GetPassword(), expectedText, "Verifying Text");
        }

        /// <summary>Tests the Clear method.</summary>
        private void TestClear()
        {
            SetPassword("");
            CheckText("");

            _textbox.Clear();
            CheckText("");

            _textbox.Clear();
            CheckText("");

            SetPassword("text");
            CheckText("text");
        }

        /// <summary>Tests the TextBox.MaxLength property.</summary>
        private void TestMaxLength()
        {
            _textbox.Clear();
            _textbox.MaxLength = 2;
            KeyboardType("abc");
        }

        private void TestMaxLengthLimited()
        {
            CheckText("ab");

            _textbox.MaxLength = 0;
            KeyboardType("abc");
        }

        private void TestMaxLengthUnlimited()
        {
            CheckText("ababc");
        }

        /// <summary>Tests the TextBox.Paste property.</summary>
        private void TestPasteCommand()
        {
            _textbox.Clear();
            SetPassword("what a good deal this is a test");
            _textbox.SelectAll();

            Clipboard.SetDataObject("bigdeal");
            _textbox.Paste();
        }

        /// <summary> Test the TextBox Commands result </summary>
        private void TestPasteCommandCheck()
        {
            CheckText("bigdeal");
        }

        /// <summary>Tests the TextBox.Text property.</summary>
        private void TestText()
        {
            _textbox.Clear();
            CheckText("");

            SetPassword("sample text");
            CheckText("sample text");
        }

        // Tests the GetRectFromTextPosition and GetTextPositionFromPoint APIs.
        private void TestHitTest()
        {
            // TextBox.GetRectFromTextPosition is interal...need to restore
            // this test when a cp based hittest method is ready.
#if DISABLED_BY_TOM_BREAKING_CHANGE
            Rect rect;
            OrientedTextPosition position;

            _textbox.Text = LongText;

            // Round trip the first char.

            _textbox.UpdateLayout();

            rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.StartPosition, LogicalDirection.Forward));
            position = _textbox.GetTextPositionFromPoint(new Point(rect.X + rect.Width / 4, rect.Y + rect.Height / 2), false);
            AssertEqual(position.TextPosition, _textbox.StartPosition, "Bad roundtrip from TestHitTest (1)");

            // Again, but this time snap from the left margin.

            rect = _textbox.GetRectangleFromTextPosition(new OrientedTextPosition(_textbox.StartPosition, LogicalDirection.Forward));
            position = _textbox.GetTextPositionFromPoint(new Point(rect.X - rect.Width / 4, rect.Y + rect.Height / 2), true);
            // The assert below could fail if anyone shrinks the current margins on our TextBox...
            AssertEqual(position.TextPosition, _textbox.StartPosition, "Bad roundtrip from TestHitTest (2)");
#endif // DISABLED_BY_TOM_BREAKING_CHANGE
        }

        // Tests that the TextBox's content is preserved if the style changes
        private void TestStyleChange()
        {
            String s = "Testing style change";
            SetPassword(s);

            // Change the style.  Same as default TextBox style, but yellow background
            Style ds = new Style(typeof(PasswordBox));
            Brush b1 = Brushes.Yellow;
            b1.Freeze();
            Brush b2 = Brushes.Gray;
            b2.Freeze();
            ds.Setters.Add (new Setter(PasswordBox.BackgroundProperty, b1));
            ds.Setters.Add (new Setter(Border.BorderBrushProperty, b2));
            ds.Setters.Add (new Setter(KeyboardNavigation.TabNavigationProperty, KeyboardNavigationMode.None));

            // default font (from system)
            ds.Setters.Add (new Setter(PasswordBox.ForegroundProperty, SystemColors.ControlTextBrush));
            ds.Setters.Add (new Setter(PasswordBox.FontFamilyProperty, SystemFonts.MessageFontFamily));
            ds.Setters.Add (new Setter(PasswordBox.FontSizeProperty, SystemFonts.MessageFontSize));
            ds.Setters.Add (new Setter(PasswordBox.FontStyleProperty, SystemFonts.MessageFontStyle));
            ds.Setters.Add (new Setter(PasswordBox.FontWeightProperty, SystemFonts.MessageFontWeight));
            ds.Setters.Add (new Setter(PasswordBox.HorizontalAlignmentProperty, System.Windows.HorizontalAlignment.Left));

            FrameworkElementFactory canvas = new FrameworkElementFactory(typeof(Canvas));
            canvas.SetValue(Control.ForegroundProperty, new TemplateBindingExtension(PasswordBox.ForegroundProperty));
            canvas.SetValue(Control.FontSizeProperty, new TemplateBindingExtension(PasswordBox.FontSizeProperty));
            canvas.SetValue(Control.FontFamilyProperty, new TemplateBindingExtension(PasswordBox.FontFamilyProperty));
            canvas.SetValue(PasswordBox.WidthProperty, new TemplateBindingExtension(PasswordBox.WidthProperty));
            canvas.SetValue(PasswordBox.HeightProperty, new TemplateBindingExtension(PasswordBox.HeightProperty));
            canvas.SetValue(Canvas.StyleProperty, null);

            FrameworkElementFactory border = new FrameworkElementFactory(typeof(Rectangle));
            border.SetValue(Shape.StrokeProperty, new TemplateBindingExtension(Border.BorderBrushProperty));
            border.SetValue(Shape.FillProperty, new TemplateBindingExtension(Border.BackgroundProperty));
            border.SetValue(Rectangle.RadiusXProperty, 4.0);
            border.SetValue(Rectangle.RadiusYProperty, 4.0);
            border.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory shadow = new FrameworkElementFactory(typeof(Rectangle));
            LinearGradientBrush b = new LinearGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x40, 0x00, 0x00, 0x00), 0.0));
            b.GradientStops.Add(new GradientStop(Colors.Transparent, 0.85));
            b.StartPoint = new Point(0, 0);
            b.EndPoint = new Point(0, 1);
            b.Freeze();
            shadow.SetValue(Shape.FillProperty, b);
            shadow.SetValue(Shape.StrokeProperty, Brushes.Transparent);
            shadow.SetValue(FrameworkElement.HeightProperty, 4d);
            shadow.SetValue(Rectangle.RadiusXProperty, 4.0);
            shadow.SetValue(Rectangle.RadiusYProperty, 4.0);
            shadow.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory shadowLeft = new FrameworkElementFactory(typeof(Rectangle));
            b = new LinearGradientBrush();
            b.GradientStops.Add(new GradientStop(Color.FromArgb(0x40, 0x00, 0x00, 0x00), 0.0));
            b.GradientStops.Add(new GradientStop(Colors.Transparent, 0.85));
            b.StartPoint = new Point(0.0, 0.0);
            b.EndPoint = new Point(1.0, 0.0);
            b.Freeze();
            shadowLeft.SetValue(Shape.FillProperty, b);
            shadowLeft.SetValue(Shape.StrokeProperty, Brushes.Transparent);
            shadowLeft.SetValue(FrameworkElement.HeightProperty, 4d);
            shadowLeft.SetValue(Rectangle.RadiusXProperty, 4.0);
            shadowLeft.SetValue(Rectangle.RadiusYProperty, 4.0);
            shadowLeft.SetValue(Rectangle.StyleProperty, null);

            FrameworkElementFactory sv = new FrameworkElementFactory(typeof(ScrollViewer), "PART_ContentHost");
            sv.SetValue(ScrollViewer.HorizontalScrollBarVisibilityProperty, new TemplateBindingExtension(ScrollViewer.HorizontalScrollBarVisibilityProperty));
            sv.SetValue(ScrollViewer.VerticalScrollBarVisibilityProperty, new TemplateBindingExtension(ScrollViewer.VerticalScrollBarVisibilityProperty));

            canvas.AppendChild(border);
            canvas.AppendChild(shadow);
            canvas.AppendChild(shadowLeft);
            canvas.AppendChild(sv);
            ControlTemplate template = new ControlTemplate(typeof(PasswordBox));
            template.VisualTree = canvas;
            ds.Setters.Add(new Setter(Control.TemplateProperty, template));

            _textbox.Style = ds;
            _textbox.ApplyTemplate();

            // Verify that content is intact
            AssertEqual(GetPassword(), s, "Plain text not intact after style change.");
        }

        private void TestSelectionProperty()
        {
            System.Reflection.PropertyInfo selectionProperty;
            Type type;

            type = typeof(PasswordBox);
            selectionProperty = type.GetProperty("Selection",
                System.Reflection.BindingFlags.Instance |
                System.Reflection.BindingFlags.Public |
                System.Reflection.BindingFlags.NonPublic);

            AssertCondition(selectionProperty != null,
                "Selection property is available through reflection from " +
                "PasswordBox type (testability requirement).");
        }

        /// <summary>Clicks on the TextBox control.</summary>
        private void ClickTextBox()
        {
            // Hard-coded to a point inside the client area. Correct
            // thing to do is map from client area to screen points,
            // but it requires more P/Invoke.
            const int x = 150;
            const int y = 150;
            DrtInput.ClickScreenPoint(x, y);
        }

        /// <summary>Emulates typing on the keyboard.</summary>
        /// <param name='text'>Text to type.</param>
        /// <remarks>
        /// Case is not respected - everything goes in lowercase.
        /// To get uppercase characters, add a "+" in front of the
        /// character. The original design had the "+" toggle the
        /// shift state, but by resetting it we make text string
        /// compatible with CLR's SendKeys.Send.
        /// <para />
        /// Eg, to type "Hello, WORLD!", pass "+hello, +W+O+R+L+D+1"
        /// <para />
        /// This method has not been globalized to keep it simple.
        /// Non-US keyboard may break this functionality.
        /// </remarks>
        private void KeyboardType(string text)
        {
            // System.Windows.Forms.SendKeys.SendWait(text);
            DrtInput.KeyboardType(text);
        }

        /// <summary>Text guaranteed to exceed one line.</summary>
        /// <remarks>
        /// Conceptually a string constant, but the compiler can have problems
        /// with the LongText and MultiPageText constants.
        /// </remarks>
        private string LongText
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(480);
                sb.Append("Text that is hopefully long enough to wrap ");
                sb.Append("no matter what the default width of the window ");
                sb.Append("is, and while we are taking variables into account, ");
                sb.Append("why not mention that even the screen resolution or ");
                sb.Append("the default font sizes might be enought to break ");
                sb.Append("this unless it's long enough - and by now I hope ");
                sb.Append("we all agree this is a reasonable length.");
                return sb.ToString();
            }
        }

        /// <summary>Text guaranteed to exceed one page.</summary>
        /// <remarks>
        /// Conceptually a string constant, but the compiler can have problems
        /// with the LongText and MultiPageText constants.
        /// </remarks>
        private string MultiPageText
        {
            get
            {
                System.Text.StringBuilder sb = new System.Text.StringBuilder(490 * 8);
                for (int i = 0; i < 8; i++)
                {
                    sb.Append(LongText);
                    sb.Append("\n\n\n");
                }
                return sb.ToString();
            }
        }

        private string GetPassword()
        {
            return _textbox.Password;
        }

        private void SetPassword(string text)
        {
            _textbox.Password = text;
        }

        private PasswordBox _textbox;
    }
}
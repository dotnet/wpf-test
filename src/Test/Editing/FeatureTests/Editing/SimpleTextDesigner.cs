// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Functional test cases for the SimpleTextDesigner class. 

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;
    
    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Globalization;
    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// Provides a base class for SimpleTextDesigner test cases.
    /// </summary>
    [TestArgument("DesignedTypeName",
        "Name of type of control to test designer on"),
     TestArgument("Text", "Text for control")]
    public abstract class SimpleTextDesignerTestCase: CustomTestCase
    {
        #region Configuration settings.

        /// <summary>
        /// Name of type of control to test designer on.
        /// </summary>
        public string DesignedTypeName
        {
            get
            {
                return Settings.GetArgument("DesignedTypeName", true);
            }
        }

        /// <summary>TextBlock for control.</summary>
        public string Text
        {
            get { return Settings.GetArgument("Text"); }
        }

        #endregion Configuration settings.

        #region Public methods.

        /// <summary>
        /// Creates an instance of the object to be designed.
        /// </summary>
        public object CreateDesignedObject()
        {
            Log("Creating object to be designed of type " + DesignedTypeName +
                "...");

            Type t = ReflectionUtils.FindType(DesignedTypeName);

            return ReflectionUtils.CreateInstance(t);
        }

        /// <summary>
        /// Sets up the control that the designer will be tested on.
        /// </summary>
        /// <param name="o">
        /// Designed control, usually created through CreateDesignedObject.
        /// </param>
        public void SetupDesignedControl(object o)
        {
            if (o == null)
                throw new ArgumentNullException("o");

            FrameworkElement uiel = (FrameworkElement)o;

            const string fontFamily = "Arial";
            const int fontSize = 13;

            MainWindow.Content = uiel;

            UIElementWrapper wrapper = new UIElementWrapper((UIElement)o);

            wrapper.Text = TextBlock;
            wrapper.FontFamily = new FontFamily(fontFamily);
            wrapper.FontSize = fontSize;
        }

        public Exception AcceptedException(string description)
        {
            return new ApplicationException(
                "SimpleTextDesigner call accepted " + description);
        }

        public void LogRejects(string description)
        {
            Log("SimpleTextDesigner call rejects " + description);
        }

        #endregion Public methods.
    }

    /// <summary>Verifies that the SimpleTextDesigner can be created.</summary>
    [TestOwner("Microsoft"), TestTactics("278")]
    public class SimpleTextDesignerCreation: SimpleTextDesignerTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Creating designer...");
            SimpleTextDesigner designer = new SimpleTextDesigner();

            Log("Verifying designer can be used for text selections...");
            Verifier.Verify(
                designer.SelectionType == typeof(TextSelection),
                "Selection is of type TextSelection", true);

            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>Verifies that HitTest works as expected.</summary>
    [TestOwner("Microsoft"),
        TestTactics(
        "249,250,251,252,253,254,255,256,257,99"),
        TestBugs("474,305"),
        TestArgument("ExpectedBackwardLength",
        "Expected length of text from start to position (-1 for unpositioned)"),
        TestArgument("PointX", "x-coordinate to test"),
        TestArgument("PointY", "y-coordinate to test"),
        TestArgument("SnapToText",
        "Boolean on whether position is snapped to text.")]
    public class SimpleTextDesignerHitTest: SimpleTextDesignerTestCase
    {
        #region Private data.

        private Visual _designedObject;

        private int ExpectedBackwardLength
        {
            get
            {
                return Settings.GetArgumentAsInt("ExpectedBackwardLength");
            }
        }

        private bool SnapToText
        {
            get
            {
                return Settings.GetArgumentAsBool("SnapToText");
            }
        }

        private Point Point
        {
            get
            {
                int x = Settings.GetArgumentAsInt("PointX");
                int y = Settings.GetArgumentAsInt("PointY");

                return new Point(x, y);
            }
        }

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            //designedObject = (Visual)CreateDesignedObject();
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalid();
            PrepareDesignedControl();
            QueueHelper.Current.QueueDelegate(new SimpleHandler(
                TestDesignedControl));
        }

        private void TestDesignedControl()
        {
            VerifyValid();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalid()
        {
            SimpleTextDesigner designer = new SimpleTextDesigner();

            Log("Verifying invalid calls to SimpleTextDesigner.HitTest...");
            try
            {
                designer.HitTest(null, new Point(0, 0), true);
                throw new ApplicationException("SimpleTextDesigner.HitTest accepted a null component");
            }
            catch (SystemException)
            {
                Log("SimpleTextDesigner.HitTest rejects null parent");
            }
            try
            {
                DependencyObject o = new DependencyObject();

                designer.HitTest(o, new Point(0, 0), true);
                throw new ApplicationException("SimpleTextDesigner.HitTest accepted a component that does not implement ITextParagraphResult");
            }
            catch (SystemException)
            {
                Log("SimpleTextDesigner.HitTest rejects components that do not implement ITextParagraphResult");
            }
        }

        private void VerifyValid()
        {
            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextPointer position = designer.HitTest(_designedObject, Point,
                SnapToText);

            if (ExpectedBackwardLength == -1)
            {
                Verifier.Verify(position == null,
                    "Position is unassigned as expected", true);
            }
            else
            {
                Verifier.Verify(position != null,
                    "Position is assigned as expected", true);
                Log(TextTreeLogger.Describe(position));

                string backward = position.GetTextInRun(LogicalDirection.Backward);
                string msg = String.Format(
                    "Backward text has length {0}, expected {1}",
                    backward.Length, ExpectedBackwardLength);

                Verifier.Verify(backward.Length == ExpectedBackwardLength, msg,
                    true);
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that HitTest does not crash with a control with no layout
    /// information.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("279"), TestBugs("475")]
    public class SimpleTextDesignerHitTestNoInfo: SimpleTextDesignerTestCase
    {
        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            const bool snapToText = true;
            TextBlock text = new TextBlock();
            SimpleTextDesigner designer = new SimpleTextDesigner();

            Log("Hit-testing with a TextBlock that has not been displayed...");
            designer.HitTest(text, new Point(0, 0), snapToText);
            Logger.Current.ReportSuccess();
        }
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner.GetRectFromTextPointer method
    /// works as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("280,282,281"),
        TestArgument("Position", "Position offset"),
        TestArgument("ExpectedRect",
        "Expected rectangle in left,top,width,height format")]
    public class SimpleTextDesignerGetRect: SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        private int Position
        {
            get { return Settings.GetArgumentAsInt("Position", true); }
        }

        private Rect ExpectedRect
        {
            get
            {
                string val = Settings.GetArgument("ExpectedRect", true);
                string errorMsg = String.Format(
                    "ExpectedRect argument [{0}] should be in format: " +
                    "[left,top,width,height]", val);
                string[] values = val.Split(',');

                if (values.Length != 4)
                    throw new Exception(errorMsg);

                Rect result = new Rect();

                result.X = float.Parse(values[0], NumberFormatInfo.InvariantInfo);
                result.Y = float.Parse(values[1], NumberFormatInfo.InvariantInfo);
                result.Width = float.Parse(values[2], NumberFormatInfo.InvariantInfo);
                result.Height = float.Parse(values[3], NumberFormatInfo.InvariantInfo);
                return result;
            }
        }

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalid();
            PrepareDesignedControl();
            QueueHelper.Current.QueueDelegate(new SimpleHandler(
                TestDesignedControl));
        }

        private void TestDesignedControl()
        {
            VerifyValid();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalid()
        {
            Log("Verifying invalid calls to SimpleTextDesigner.GetRectFromTextPointer...");

            SimpleTextDesigner designer = new SimpleTextDesigner();

            // Workaround: TextTree is internal, so we access it through
            // a control that exposes an instance of it.
            TextBlock text = new TextBlock();
            TextPointer position = text.TextRange.Start;

            try
            {
                designer.GetRectFromTextPointer(null, position);
                throw new ApplicationException("SimpleTextDesigner.GetRectFromTextPointer accepted a null component");
            }
            catch (SystemException)
            {
                Log("SimpleTextDesigner.GetRectFromTextPointer rejects null component");
            }
            try
            {
                designer.GetRectFromTextPointer(text, null);
                throw new ApplicationException("SimpleTextDesigner.GetRectFromTextPointer accepted a null position");
            }
            catch (SystemException)
            {
                Log("SimpleTextDesigner.GetRectFromTextPointer rejects null position");
            }
            try
            {
                TextBlock textB = new TextBlock();
                TextPointer positionB = textB.TextRange.Start;

                designer.GetRectFromTextPointer(text, positionB);
                throw new ApplicationException("SimpleTextDesigner.GetRectFromTextPointer accepted mixed component/position");
            }
            catch (SystemException)
            {
                Log("SimpleTextDesigner.GetRectFromTextPointer rejects mixed component/position");
            }
        }

        private void VerifyValid()
        {
            Log("Retrieving start position of control text...");

            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            TextArray array = wrapper.TextRange.TextContext;
            TextPointer position = array.GetPositionFromIndex(Position,
                LogicalDirection.Forward);

            Log(TextTreeLogger.Describe(position));

            SimpleTextDesigner designer = new SimpleTextDesigner();
            Rect r = designer.GetRectFromTextPointer(_designedObject, position);
            Log("Rectangle returned: " + r.ToString());
            Rect expected = ExpectedRect;
            Log("Rectangle expected: " + expected.ToString());

            // Log an image of the area.
            Drawing.Rectangle rectangle = new Drawing.Rectangle(
                (int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
            Drawing.Bitmap bitmap = BitmapCapture.CreateBitmapFromElement(
                _designedObject);
            bitmap = BitmapUtils.HighlightRectangle(bitmap, rectangle);
            Logger.Current.LogImage(bitmap, "rect");

            // Verify empty rectangles in a strict manner, and allow for some
            // floating-point or rendering error otherwise.
            if (expected.IsEmpty)
            {
                Verifier.Verify(r.IsEmpty);
            }
            else
            {
                const float errorMargin = 1;
                expected.Inflate(errorMargin, errorMargin);
                Verifier.Verify(expected.Contains(r),
                    "Result rectangle contained in expected rectangle.");
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner.AdvanceLine method
    /// works as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestBugs("476"),
     TestTactics("283,284,285,286,287"),
     TestArgument("ExpectedCount", "Expected count of lines moved"),
     TestArgument("ExpectedPosition", "Expected position after move (in chars)"),
     TestArgument("Limit", "Limit offset, -1 to specify none (in caret units)"),
     TestArgument("LineCount", "Numer of lines to advance"),
     TestArgument("Options", "TextMoveOptions for call"),
     TestArgument("Position", "Position offset (in caret units)")]
    public class SimpleTextDesignerAdvanceLine: SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        private int ExpectedCount
        {
            get { return Settings.GetArgumentAsInt("ExpectedCount", true); }
        }

        private int ExpectedPosition
        {
            get { return Settings.GetArgumentAsInt("ExpectedPosition", true); }
        }

        private int Limit
        {
            get { return Settings.GetArgumentAsInt("Limit", true); }
        }

        private int LineCount
        {
            get { return Settings.GetArgumentAsInt("LineCount", true); }
        }

        private TextMoveOptions Options
        {
            get
            {
                string s = Settings.GetArgument("Options");
                if (s == "") return 0;
                return (TextMoveOptions)
                    Enum.Parse(typeof(TextMoveOptions), s);
            }
        }

        private int Position
        {
            get { return Settings.GetArgumentAsInt("Position", true); }
        }

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            PrepareDesignedControl();
            QueueHelper.Current.QueueDelegate(
                new SimpleHandler(TestDesignedControl));
        }

        #endregion Main flow.

        #region Verifications.

        private void TestDesignedControl()
        {
            // Set up arguments for the call.
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            TextPointer start = wrapper.TextRange.Start;
            TextPointer position;
            position = wrapper.TextRange.TextContext.GetTextPointer(start);
            position.Move(TextUnits.CaretPosition, Position);
            Log("Start position details:" + Environment.NewLine +
                TextTreeLogger.Describe(position));

            TextPointer limit;
            if (Limit == -1)
            {
                limit = null;
            }
            else
            {
                TextPointer limitNavigator =
                    wrapper.TextRange.TextContext.GetTextPointer(start);
                limitNavigator.Move(TextUnits.CaretPosition, Limit);
                limit = limitNavigator;
                Log("Limit details:" + Environment.NewLine +
                    TextTreeLogger.Describe(limit));
            }

            SimpleTextDesigner designer = new SimpleTextDesigner();
            int result = designer.AdvanceLine(_designedObject, position,
                LineCount, Options, limit);

            int resultPosition =
                position.GetRunLength(LogicalDirection.Backward);
            Log("Expected result: " + ExpectedCount);
            Log("Expected position: " + ExpectedPosition);
            Log("Result: " + result);
            Log("Position: " + resultPosition);
            Log("Result position details:" + Environment.NewLine +
                TextTreeLogger.Describe(position));
            Verifier.Verify(ExpectedCount == result, "Line count matches.");
            Verifier.Verify(ExpectedPosition == resultPosition, "Position matches.");

            Logger.Current.ReportSuccess();
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner.AdvanceLine method
    /// rejects invalid input.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("288")]
    [Test(3, "Editing", "SimpleTextDesignerAdvanceLineInvalid", MethodParams = "/TestCaseType=SimpleTextDesignerAdvanceLineInvalid")]
    public class SimpleTextDesignerAdvanceLineInvalid:
        SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            PrepareDesignedControl();
            QueueHelper.Current.QueueDelegate(new
                SimpleHandler(TestDesignedControl));
        }

        #endregion Main flow.

        #region Verifications.

        private void TestDesignedControl()
        {
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            TextArray textArray = wrapper.TextRange.TextContext;
            SimpleTextDesigner designer = new SimpleTextDesigner();
            DependencyObject component = _designedObject;
            TextMoveOptions options = 0;
            TextPointer limit = null;
            int count = 1;

            TextPointer position =
                textArray.GetTextPointer(wrapper.TextRange.Start);
            TextPointer unpositioned =
                textArray.GetTextPointer(position);
            unpositioned.Unposition();

            TextBlock t = new TextBlock();
            TextPointer outOfTree = t.TextRange.TextContext.GetTextPointer(
                t.TextRange.Start);

            Log("Verifying invalid calls to SimpleTextDesigner.AdvanceLine...");
            try
            {
                designer.AdvanceLine(null, position, count, options, limit);
                throw AcceptedException("null component");
            }
            catch(SystemException)
            {
                LogRejects("null component");
            }
            try
            {
                designer.AdvanceLine(component, null, count, options, limit);
                throw AcceptedException("null position");
            }
            catch(SystemException)
            {
                LogRejects("null position");
            }
            try
            {
                designer.AdvanceLine(
                    component, unpositioned, count, options, limit);
                throw AcceptedException("unpositioned position");
            }
            catch(SystemException)
            {
                LogRejects("unpositioned position");
            }
            try
            {
                TextMoveOptions badOptions =
                    unchecked((TextMoveOptions) 0xFFFF);
                designer.AdvanceLine(
                    component, position, count, badOptions, limit);
                throw AcceptedException("bad options");
            }
            catch(SystemException)
            {
                LogRejects("bad options");
            }
            try
            {
                designer.AdvanceLine(
                    component, position, count, options, unpositioned);
                throw AcceptedException("unpositioned limit");
            }
            catch(SystemException)
            {
                LogRejects("unpositioned limit");
            }
            try
            {
                designer.AdvanceLine(
                    component, outOfTree, count, options, limit);
                throw AcceptedException("out of tree position");
            }
            catch(SystemException)
            {
                LogRejects("out of tree position");
            }
            try
            {
                designer.AdvanceLine(
                    component, position, count, options, outOfTree);
                throw AcceptedException("out of tree limit");
            }
            catch(SystemException)
            {
                LogRejects("out of tree limit");
            }

            Logger.Current.ReportSuccess();
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner.GetLineRange works
    /// as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("289,290,291,292"),
     TestArgument("ExpectedStart", "Expected start of line (in chars)(-1 for null)"),
     TestArgument("ExpectedEnd", "Expected end of line (in chars)"),
     TestArgument("Position", "Queried position (in chars)")]
    public class SimpleTextDesignerGetLineRange: SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        private int Position
        {
            get { return Settings.GetArgumentAsInt("Position", true); }
        }

        private int ExpectedStart
        {
            get { return Settings.GetArgumentAsInt("ExpectedStart", true); }
        }

        private int ExpectedEnd
        {
            get { return Settings.GetArgumentAsInt("ExpectedEnd", true); }
        }

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            PrepareDesignedControl();
            VerifyInvalid();

            // Verifies that the result is null if there is no view info.
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            TextPointer startPos = wrapper.TextRange.Start;
            SimpleTextDesigner designer = new SimpleTextDesigner();
            Verifier.Verify(
                null == designer.GetLineRange(_designedObject, startPos),
                "Line range is null if there is no view information.", true);

            QueueHelper.Current.QueueDelegate(
                new SimpleHandler(TestDesignedControl));
        }

        private void TestDesignedControl()
        {
            VerifyValid();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalid()
        {
            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextBlock component = new TextBlock();
            TextPointer position = component.TextRange.Start;
            try
            {
                designer.GetLineRange(null, position);
                throw AcceptedException("null component");
            }
            catch(SystemException)
            {
                LogRejects("null component");
            }
            try
            {
                designer.GetLineRange(component, null);
                throw AcceptedException("null position");
            }
            catch(SystemException)
            {
                LogRejects("null position");
            }
        }

        private void VerifyValid()
        {
            // Set up arguments for the call.
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            TextPointer startPos = wrapper.TextRange.Start;
            TextPointer position;
            position = wrapper.TextRange.TextContext.GetTextPointer(startPos);
            position.Move(TextUnits.Char, Position);
            Log("Start position details:" + Environment.NewLine +
                TextTreeLogger.Describe(position));

            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextRange result = designer.GetLineRange(_designedObject, position);
            if (ExpectedStart == -1)
            {
                Log("Expected null result");
                Verifier.Verify(result == null, "Result is null");
                return;
            }

            int start = result.Start.GetTextInRun(LogicalDirection.Backward).Length;
            int end = result.End.GetTextInRun(LogicalDirection.Backward).Length;

            Log("Result details:" + Environment.NewLine +
                TextTreeLogger.Describe(result));
            Log("Expected start: " + ExpectedStart);
            Log("Expected end: " + ExpectedEnd);
            Log("Resulting start: " + start);
            Log("Resulting end: " + end);

            Verifier.Verify(ExpectedStart == start, "Start position matches.");
            Verifier.Verify(ExpectedEnd == end, "End position matches.");
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner caret methods work as expected.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("293,294")]
    public class SimpleTextDesignerCaret: SimpleTextDesignerTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            VerifyInvalid();
            VerifyValid();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalid()
        {
            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextBlock component = new TextBlock();
            TextPointer position = component.TextRange.Start;
            TextPointer navigator =
                position.TextContext.GetTextPointer(position);

            Log("Verifying invalid calls to IsCaretPosition...");
            try
            {
                designer.IsCaretPosition(null, position);
                throw AcceptedException("null component");
            }
            catch(SystemException)
            {
                LogRejects("null component");
            }
            try
            {
                designer.IsCaretPosition(component, null);
                throw AcceptedException("null position");
            }
            catch(SystemException)
            {
                LogRejects("null position");
            }

            Log("Verifying invalid calls to MoveToInsertionPosition...");
            try
            {
                designer.MoveToInsertionPosition(null, navigator,
                    LogicalDirection.Backward);
                throw AcceptedException("null component");
            }
            catch(SystemException)
            {
                LogRejects("null component");
            }
            try
            {
                designer.MoveToInsertionPosition(component, null,
                    LogicalDirection.Backward);
                throw AcceptedException("null position");
            }
            catch(SystemException)
            {
                LogRejects("null position");
            }
        }

        private void VerifyValid()
        {
            const LogicalDirection direction = LogicalDirection.Forward;
            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextBlock component = new TextBlock();
            if (Text.Length > 0)
            {
                component.TextRange.Text = TextBlock;
            }
            TextPointer position = component.TextRange.Start;
            TextPointer navigator =
                position.TextContext.GetTextPointer(position);

            do
            {
                Verifier.Verify(
                    designer.IsCaretPosition(component, navigator),
                    "Position is a caret.", true);
                // Nothing to do - just make sure it does not throw.
                designer.MoveToInsertionPosition(component, navigator, direction);
            } while (navigator.Move(TextUnits.Char, 1) == 1);
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that the SimpleTextDesigner can insert paragraph breaks.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("295,296")]
    public class SimpleTextDesignerBreaks: SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            PrepareDesignedControl();
            VerifyInvalid();
            VerifyValid();
            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Verifications.

        private void VerifyInvalid()
        {
            SimpleTextDesigner designer = new SimpleTextDesigner();
            TextBlock component = new TextBlock();
            TextPointer position = component.TextRange.Start;

            Log("Verifying invalid calls to InsertParagraphBreak...");
            try
            {
                designer.InsertParagraphBreak(null, position);
                throw AcceptedException("null component");
            }
            catch(SystemException)
            {
                LogRejects("null component");
            }
            try
            {
                designer.InsertParagraphBreak(component, null);
                throw AcceptedException("null position");
            }
            catch(SystemException)
            {
                LogRejects("null position");
            }
        }

        private void VerifyValid()
        {
            const LogicalDirection direction = LogicalDirection.Backward;
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            SimpleTextDesigner designer = new SimpleTextDesigner();
            string originalText = TextBlock;
            string nl = Environment.NewLine;
            for (int i = 0; i <= originalText.Length; i++)
            {
                wrapper.Text = originalText;

                TextPointer start = wrapper.TextRange.Start;
                TextPointer nav = start.TextContext.GetTextPointer(start);
                nav.SetGravity(direction);
                nav.Move(TextUnits.Char, i);
                int charsToLeft = nav.GetTextInRun(direction).Length;

                Log("Inserting break at " + i);
                designer.InsertParagraphBreak(_designedObject, nav);

                Log("New text: [" +wrapper.Text + "]");
                Verifier.Verify(charsToLeft == nav.GetTextInRun(direction).Length,
                    "Gravity is honored", true);
                Verifier.Verify(wrapper.Text.Substring(i, nl.Length) == nl,
                    "Newline inserted", true);
            }
        }

        #endregion Verifications.
    }

    /// <summary>
    /// Verifies that SimpleTextDesigner can deal gracefully with
    /// text boxes without lines.
    /// </summary>
    [TestOwner("Microsoft"), TestTactics("297")]
    public class SimpleTextDesignerNullLines: SimpleTextDesignerTestCase
    {
        #region Private data.

        private UIElement _designedObject;

        #endregion Private data.

        #region Main flow.

        private void PrepareDesignedControl()
        {
            _designedObject = (UIElement)CreateDesignedObject();
            SetupDesignedControl(_designedObject);
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            Log("Setting up two lines of text...");
            PrepareDesignedControl();
            UIElementWrapper wrapper = new UIElementWrapper(_designedObject);
            wrapper.Text = Environment.NewLine;

            Log("Retrieving position before layout is available...");
            SimpleTextDesigner designer = new SimpleTextDesigner();
            Rect rect = designer.GetRectFromTextPointer(
                _designedObject, wrapper.TextRange.End);
            Log("Rectangle for last position: " + rect);
            Verifier.Verify(rect.IsEmpty, "Rectangle is empty", true);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.
    }
}

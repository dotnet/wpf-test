using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;
using Microsoft.Test.Diagnostics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Ink;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace VisualDiagnostics.FeatureTests.VisualTreeChange
{
    [Test(1, "VisualTreeChange")]
    public class VisualTreeChangeTest_NoEventsWithoutDebugger : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            if (Debugger.IsAttached)
            {
                // WARNING: this test will fail when running under a debugger.
                // Visual tree change notification is disable when debugger
                // is NOT attached and that is what we test.
                Debugger.Break();
                IsTrue(false, "TEST WILL FAIL WHEN RUNNING UNDER A DEBUGGER. Step thru slowly!");
            }

            int invokationCount = 0;
            EventHandler<VisualTreeChangeEventArgs> handler = delegate(object sender, VisualTreeChangeEventArgs args)
            {
                invokationCount++;
            };

            System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged += handler;
            Grid grid = new Grid();
            grid.Children.Add(new StackPanel());
            grid.Children.Add(new Button());
            grid.Children.Clear();
            System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged -= handler;
            IsTrue(invokationCount == 0, "Verify there are no event invokations");
        }
    }

    [Test(1, "VisualTreeChange")]
    public class VisualTreeChangeTest_2D : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(this.Window))
            {
                // Create Grid and add some content to it BEFORE adding grid to Window
                Grid grid = new Grid();
                Rectangle rectangle = new Rectangle();
                Line line = new Line() { X1 = 10, Y1 = 10, X2 = 50, Y2 = 50 };
                Ellipse ellipse = new Ellipse();
                grid.Children.Add(rectangle);
                grid.Children.Insert(0, line);
                grid.Children.Insert(1, ellipse);

                // Connect grid to Window
                recorder.Window.Content = grid;
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=Grid, child=Rectangle, index=0, Un-rooted",
                    "[1], Add, parent=Grid, child=Line, index=0, Un-rooted",
                    "[2], Add, parent=Grid, child=Ellipse, index=1, Un-rooted",
                    "[3], Add, parent=ContentPresenter, child=Grid, index=0, Rooted");

                // Add more content to grid
                StackPanel stackPanel = new StackPanel();
                grid.Children.Add(stackPanel);
                Border border = new Border();
                stackPanel.Children.Add(border);
                border.Child = new WrapPanel();
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=Grid, child=StackPanel, index=3, Rooted",
                    "[1], Add, parent=StackPanel, child=Border, index=0, Rooted",
                    "[2], Add, parent=Border, child=WrapPanel, index=0, Rooted");

                // Remove content and then add it back
                grid.Children.Remove(rectangle);
                grid.Children.Remove(ellipse);
                grid.Children.Remove(stackPanel);
                grid.Children.Add(stackPanel);
                grid.Children.Insert(0, ellipse);
                grid.Children.Insert(1, rectangle);
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=Grid, child=Rectangle, index=-1, Rooted",
                    "[1], Remove, parent=Grid, child=Ellipse, index=-1, Rooted",
                    "[2], Remove, parent=Grid, child=StackPanel, index=-1, Rooted",
                    "[3], Add, parent=Grid, child=StackPanel, index=1, Rooted",
                    "[4], Add, parent=Grid, child=Ellipse, index=0, Rooted",
                    "[5], Add, parent=Grid, child=Rectangle, index=1, Rooted");

                // Disconnect grid from window and then remove add content again
                recorder.Window.Content = new Canvas();
                recorder.UpdateLayout();
                recorder.Reset();

                grid.Children.Remove(stackPanel);
                grid.Children.Remove(line);
                grid.Children.Add(line);
                grid.Children.Add(stackPanel);
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=Grid, child=StackPanel, index=-1, Un-rooted",
                    "[1], Remove, parent=Grid, child=Line, index=-1, Un-rooted",
                    "[2], Add, parent=Grid, child=Line, index=2, Un-rooted",
                    "[3], Add, parent=Grid, child=StackPanel, index=3, Un-rooted");

                grid.Children.Clear();
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=Grid, child=Ellipse, index=-1, Un-rooted",
                    "[1], Remove, parent=Grid, child=Rectangle, index=-1, Un-rooted",
                    "[2], Remove, parent=Grid, child=Line, index=-1, Un-rooted",
                    "[3], Remove, parent=Grid, child=StackPanel, index=-1, Un-rooted");
            }
        }
    }

    [Test(1, "VisualTreeChange")]
    public class VisualTreeChangeTest_3D : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(this.Window))
            {
                // Add 3D children to 3D parent
                ContainerUIElement3D visual3D = new ContainerUIElement3D();
                Test3D_One child1 = new Test3D_One();
                Test3D_Two child2 = new Test3D_Two();
                Test3D_Three child3 = new Test3D_Three();
                visual3D.Children.Add(child1);
                visual3D.Children.Add(child2);
                visual3D.Children.Insert(0, child3);
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=ContainerUIElement3D, child=Test3D_One, index=0, Un-rooted",
                    "[1], Add, parent=ContainerUIElement3D, child=Test3D_Two, index=1, Un-rooted",
                    "[2], Add, parent=ContainerUIElement3D, child=Test3D_Three, index=0, Un-rooted");

                // Remove and add again
                visual3D.Children.Remove(child1);
                visual3D.Children.Remove(child2);
                visual3D.Children.Add(child1);
                visual3D.Children.Add(child2);
                visual3D.Children.Remove(child3);
                visual3D.Children.Insert(0, child3);
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=ContainerUIElement3D, child=Test3D_One, index=-1, Un-rooted",
                    "[1], Remove, parent=ContainerUIElement3D, child=Test3D_Two, index=-1, Un-rooted",
                    "[2], Add, parent=ContainerUIElement3D, child=Test3D_One, index=1, Un-rooted",
                    "[3], Add, parent=ContainerUIElement3D, child=Test3D_Two, index=2, Un-rooted",
                    "[4], Remove, parent=ContainerUIElement3D, child=Test3D_Three, index=-1, Un-rooted",
                    "[5], Add, parent=ContainerUIElement3D, child=Test3D_Three, index=0, Un-rooted");


                // Clear and add back
                visual3D.Children.Clear();
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=ContainerUIElement3D, child=Test3D_Two, index=-1, Un-rooted",
                    "[1], Remove, parent=ContainerUIElement3D, child=Test3D_One, index=-1, Un-rooted",
                    "[2], Remove, parent=ContainerUIElement3D, child=Test3D_Three, index=-1, Un-rooted");

                visual3D.Children.Add(child1);
                visual3D.Children.Add(child2);
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=ContainerUIElement3D, child=Test3D_One, index=0, Un-rooted",
                    "[1], Add, parent=ContainerUIElement3D, child=Test3D_Two, index=1, Un-rooted");

                // Add 3D content to 2D parent
                Viewport3DVisual visual2D = new Viewport3DVisual();
                AreEqual(visual2D is Visual, true, "Verify that Viewport3DVisual is NOT Visual3D");
                AreEqual(visual3D is Visual3D, true, "Verify that ContainerUIElement3D IS Visual3D");
                visual2D.Children.Add(visual3D);
                visual2D.Children.Remove(visual3D);
                visual2D.Children.Insert(0, visual3D);
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=Viewport3DVisual, child=ContainerUIElement3D, index=0, Un-rooted",
                    "[1], Remove, parent=Viewport3DVisual, child=ContainerUIElement3D, index=-1, Un-rooted",
                    "[2], Add, parent=Viewport3DVisual, child=ContainerUIElement3D, index=0, Un-rooted");

                visual2D.Children.Clear();
                VerifyResultsAndReset(recorder,
                    "[0], Remove, parent=Viewport3DVisual, child=ContainerUIElement3D, index=-1, Un-rooted");
            }
        }
    }

    [Test(1, "VisualTreeChange")]
    public class VisualTreeChangeTest_FixedControls : VisualTreeChangeTestBase
    {
        // Verifies that TextViewBox and InkPresenter provide correct child index when
        // they trigger ADD visual tree change.
        protected override void RunTest()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(this.Window))
            {
                // Create Grid and add some content to it
                Grid grid = new Grid();

                // Cteare TextViewBox. it is an internal class thus we have to use reflection.
                // Normally TextViewBox gets indirectly created by text box. However to minimize
                // the risk of regressions we'll create TextViewBox directly.
                TextBox text = new TextBox();
                object[] parameters = new object[] { text };

                Assembly assembly = typeof(TextBox).Assembly;
                Type textBoxViewType = assembly.GetType("System.Windows.Controls.TextBoxView");
                Type[] constructorTypes = new Type[] { assembly.GetType("System.Windows.Controls.ITextBoxViewHost") };
                ConstructorInfo constructor = textBoxViewType.GetConstructor(
                    BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                    Type.DefaultBinder, constructorTypes, null);

                FrameworkElement textBoxView = (FrameworkElement)constructor.Invoke(parameters);
                grid.Children.Add(textBoxView);

                // Create InkPresenter and add a child.
                InkPresenter inkPresenter = new InkPresenter();
                grid.Children.Add(inkPresenter);
                DrawingAttributes drawingAttributes = new DrawingAttributes();
                drawingAttributes.Color = Colors.Red;
                inkPresenter.AttachVisuals(new Canvas(), drawingAttributes);

                this.Window.Content = grid;
                this.Window.UpdateLayout();

                // What type of children TextViewBox produces does not matter. What matters is
                // that there is an ADD change with TextViewBox parent and non-negative index.
                string textBoxViewChange = recorder.Result.Find(r => r.Contains("Add, parent=TextBoxView,") && r.Contains("index=0"));
                bool isFound = !string.IsNullOrEmpty(textBoxViewChange);
                this.AreEqual(isFound, true, "Verify there is ADD TextViewBox change with valid index");

                // Similarly, there should be an ADD change for InkPresenter. Exact child type
                // does not matter but index should be non-negative.
                string inkPresenterChange = recorder.Result.Find(r => r.Contains("Add, parent=InkPresenter,") && r.Contains("index=0"));
                isFound = !string.IsNullOrEmpty(inkPresenterChange);
                this.AreEqual(isFound, true, "Verify there is ADD TextViewBox change with valid index");
            }
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_EnableInDevMode : VisualTreeChangeEnableTestBase
    {
        protected override Enabler GetEnabler()
        {
            return new DevModeEnabler(Enable, Disable);
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_EnableInEnvironment : VisualTreeChangeEnableTestBase
    {
        protected override Enabler GetEnabler()
        {
            return new EnvironmentEnabler("ENABLE_XAML_DIAGNOSTICS_VISUAL_TREE_NOTIFICATIONS", "1", Enable, Disable);
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_NullEnable : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            bool gotException = false;
            try
            {
                new NullEnabler(Enable, Disable);
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }

            this.AreEqual(gotException, true, "Verify EnableVisualTreeChanged throws in non-diagnostic scenario");
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_Disable : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            bool gotException = false;
            try
            {
                new DisableEnabler(Enable, Disable);
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }

            this.AreEqual(gotException, true, "Verify EnableVisualTreeChanged throws when diagnostics are disabled");
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_NoEventsWhenDisabled : VisualTreeChangeTestBase
    {
        // have to create this at ctor time, to get the 'disable' switch set before
        // any markup is loaded
        Enabler _enabler = new PrivateReflectionDisableEnabler();

        protected override void RunTest()
        {
            if (Debugger.IsAttached)
            {
                // WARNING: this test will fail when running under a debugger.
                // Visual tree change notification is disable when debugger
                // is NOT attached and that is what we test.
                Debugger.Break();
                IsTrue(false, "TEST WILL FAIL WHEN RUNNING UNDER A DEBUGGER. Step thru slowly!");
            }

            using (_enabler)
            {
                int invokationCount = 0;
                EventHandler<VisualTreeChangeEventArgs> handler = delegate(object sender, VisualTreeChangeEventArgs args)
                {
                    invokationCount++;
                };

                System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged += handler;
                Grid grid = new Grid();
                grid.Children.Add(new StackPanel());
                grid.Children.Add(new Button());
                grid.Children.Clear();
                System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged -= handler;
                IsTrue(invokationCount == 0, "Verify there are no event invokations");
            }
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_ReentrantChangesThrow : VisualTreeChangeReentrantTestBase
    {
        protected override bool ShouldThrow()
        {
            return true;
        }
    }

    [Test(1, "VisualTreeChange", Versions="4.7.1+")]
    public class VisualTreeChangeTest_ReentrantChangesWarn : VisualTreeChangeReentrantTestBase
    {
        protected override bool ShouldThrow()
        {
            return false;
        }
    }

    public class VisualTreeChangeTestBase : WindowTest
    {
        public VisualTreeChangeTestBase()
        {
            RunSteps += new TestStep(this.ExecuteTest);
        }

        private TestResult ExecuteTest()
        {
            try
            {
                this.RunTest();
            }
            catch
            {
                return TestResult.Fail;
            }

            return this.Log.Result;
        }

        protected virtual void RunTest()
        {
        }

        protected void VerifyResultsAndReset(VisualChangeRecorder recorder, params string[] expectedRecords)
        {
            recorder.UpdateLayout();

            this.AreEqual(recorder.Result.Count, expectedRecords.Length, "Verify actual/expected record count");
            int count = Math.Min(recorder.Result.Count, expectedRecords.Length);
            for (int i = 0; i < count; i++)
            {
                // Get normalized actual and expected records
                string actual = NormalizeRecord(recorder.Result[i]);
                string expected = NormalizeRecord(expectedRecords[i]);
                this.AreEqual(actual, expected, "Verify record " + i);
            }

            recorder.Reset();
        }

        protected static string NormalizeRecord(string record)
        {
            return record.Trim().Replace(" ", "").Replace(",", ", ");
        }

        protected void AreEqual(object actual, object expected, string message)
        {
            bool isOkay = object.Equals(actual, expected);
            string log = string.Format("{0}: {1}, actual='{2}', expected='{3}'",
                isOkay ? "PASS" : "FAILED", message, actual, expected);
            IsTrue(isOkay, message);
        }

        protected void IsTrue(bool value, string message)
        {
            this.Log.LogStatus(message);
            if (value == false)
            {
                this.Log.Result = TestResult.Fail;
                throw new ApplicationException(message);
            }
        }

        protected void Enable()
        {
            System.Windows.Diagnostics.VisualDiagnostics.EnableVisualTreeChanged();
        }

        protected void Disable()
        {
            System.Windows.Diagnostics.VisualDiagnostics.DisableVisualTreeChanged();
        }
    }

    public abstract class VisualTreeChangeEnableTestBase : VisualTreeChangeTestBase
    {
        protected override void RunTest()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(this.Window, GetEnabler()))
            {
                // Create Grid and add some content to it BEFORE adding grid to Window
                Grid grid = new Grid();
                Rectangle rectangle = new Rectangle();
                Line line = new Line() { X1 = 10, Y1 = 10, X2 = 50, Y2 = 50 };
                Ellipse ellipse = new Ellipse();
                grid.Children.Add(rectangle);
                grid.Children.Insert(0, line);
                grid.Children.Insert(1, ellipse);

                // Connect grid to Window
                recorder.Window.Content = grid;
                VerifyResultsAndReset(recorder,
                    "[0], Add, parent=Grid, child=Rectangle, index=0, Un-rooted",
                    "[1], Add, parent=Grid, child=Line, index=0, Un-rooted",
                    "[2], Add, parent=Grid, child=Ellipse, index=1, Un-rooted",
                    "[3], Add, parent=ContentPresenter, child=Grid, index=0, Rooted");
            }
        }

        protected abstract Enabler GetEnabler();
    }

    public abstract class VisualTreeChangeReentrantTestBase : VisualTreeChangeTestBase
    {
        bool _inVisualTreeChanged = false;

        protected override void RunTest()
        {
            using (new ReentrantEnabler(ShouldThrow()))
            {
                System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged += OnVisualTreeChanged;
                Grid grid = new Grid();
                grid.Children.Add(new Button());
                System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged -= OnVisualTreeChanged;
            }
        }

        void OnVisualTreeChanged(object sender, VisualTreeChangeEventArgs args)
        {
            // ignore re-entrant calls to this handler, so that test doesn't overflow the stack
            if (_inVisualTreeChanged)
                return;

            try
            {
                _inVisualTreeChanged = true;
                TryReentrantChanges();
            }
            finally
            {
                _inVisualTreeChanged = false;
            }
        }

        void TryReentrantChanges()
        {
            bool gotException;

            try
            {
                gotException = false;
                (new Grid()).Children.Add(new Button());
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }
            AreEqual(gotException, ShouldThrow(), "Reentrant change visual parent");

            try
            {
                gotException = false;
                Panel panel = new Grid();
                UIElementCollection list = new UIElementCollection(panel, panel);
                list.Add(new Button());
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }
            AreEqual(gotException, ShouldThrow(), "Reentrant change logical parent");

            try
            {
                gotException = false;
                (new FrameworkElement()).Name = "abc";
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }
            AreEqual(gotException, ShouldThrow(), "Reentrant change FE property");

            try
            {
                gotException = false;
                (new FrameworkContentElement()).Name = "abc";
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }
            AreEqual(gotException, ShouldThrow(), "Reentrant change FCE property");

            try
            {
                gotException = false;
                (new ResourceDictionary()).Add("abc", "abc");
            }
            catch (InvalidOperationException)
            {
                gotException = true;
            }
            AreEqual(gotException, ShouldThrow(), "Reentrant change resource dictionary");
        }

        protected abstract bool ShouldThrow();
    }

    // Helper class to record changes
    public class VisualChangeRecorder : IDisposable
    {
        public List<string> Result = new List<string>();
        public Window Window;
        private Enabler _enabler;

        public VisualChangeRecorder(Window window)
            : this(window, new VisualTreeChangedPrivateReflectionEnabler())
        {
        }

        public VisualChangeRecorder(Window window, Enabler enabler)
        {
            _enabler = enabler;

            this.Window = window;
            this.Window.Left = 10000;
            this.Window.Width = 400;
            this.Window.Height = 200;
            System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged += this.OnVisualtreeChange;
        }

        public void Dispose()
        {
            _enabler.Dispose();
            System.Windows.Diagnostics.VisualDiagnostics.VisualTreeChanged -= this.OnVisualtreeChange;
        }

        public void Reset()
        {
            this.Result.Clear();
        }

        public void UpdateLayout()
        {
            if (this.Window != null)
            {
                this.Window.UpdateLayout();
            }
        }

        public string ResultAsString
        {
            get
            {
                if (Result.Count == 0)
                {
                    return string.Empty;
                }

                StringBuilder builder = new StringBuilder();
                for (int i = 0; i < Result.Count - 1; i++)
                {
                    string value = Result[i];
                    builder.AppendFormat("\"{0}\",", value);
                    builder.AppendLine();
                }
                builder.AppendFormat("\"{0}\"", Result[Result.Count - 1]);

                return builder.ToString();
            }
        }

        private void OnVisualtreeChange(object sender, VisualTreeChangeEventArgs args)
        {
            // Record changes
            string record = FormatChange(args, this.Result.Count);
            this.Result.Add(record);
        }

        private bool IsRooted(VisualTreeChangeEventArgs change)
        {
            if (Window == null)
            {
                return false;
            }

            Visual visual = change.Parent as Visual;
            if (visual != null)
            {
                return visual.IsDescendantOf(Window);
            }

            Visual3D visual3D = change.Parent as Visual3D;
            if (visual != null)
            {
                return visual3D.IsDescendantOf(Window);
            }

            return false;
        }

        private string FormatChange(VisualTreeChangeEventArgs change, int index)
        {
            // Example:
            //  [1], Add, parent=Grid, child=Line, index=1, Un-rooted
            string result = string.Format(CultureInfo.InvariantCulture, "[{0}], {1}, parent={2}, child={3}, index={4}, {5}",
                index,
                change.ChangeType,
                change.Parent.GetType().Name,
                change.Child.GetType().Name,
                change.ChildIndex,
                IsRooted(change) ? "Rooted" : "Un-rooted");

            return result;
        }
    }

    public class VisualTreeChangedPrivateReflectionEnabler : Enabler
    {
        public VisualTreeChangedPrivateReflectionEnabler()
            : base(null, null)
        {
            SetSwitch("Switch.System.Windows.Diagnostics.DisableDiagnostics", true);
            SetPrivateField(typeof(System.Windows.Diagnostics.VisualDiagnostics), "s_isDebuggerCheckDisabledForTestPurposes", true);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestorePrivateField();
            RestoreSwitch();
        }
    }

    public class PrivateReflectionDisableEnabler : Enabler
    {
        public PrivateReflectionDisableEnabler(Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            SetSwitch("Switch.System.Windows.Diagnostics.DisableDiagnostics", true);
            SetPrivateField(typeof(System.Windows.Diagnostics.VisualDiagnostics), "s_isDebuggerCheckDisabledForTestPurposes", true);
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestorePrivateField();
            RestoreSwitch();
        }
    }

    public class ReentrantEnabler : Enabler
    {
        public ReentrantEnabler(bool shouldThrow, Action enable=null, Action disable=null)
            : base(enable, disable)
        {
            SetSwitch("Switch.System.Windows.Diagnostics.AllowChangesDuringVisualTreeChanged", !shouldThrow);
            SetEnvironmentVariable("ENABLE_XAML_DIAGNOSTICS_VISUAL_TREE_NOTIFICATIONS", "1");
            Enable();
        }

        public override void Dispose()
        {
            Disable();
            RestoreEnvironmentVariable();
            RestoreSwitch();
        }
    }

    public class Test3D_One : UIElement3D
    {
    }

    public class Test3D_Two : UIElement3D
    {
    }

    public class Test3D_Three : UIElement3D
    {
    }
}

// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//
//
//
// Description: DRT tests for visual tree change notification.
//

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Diagnostics;
using System.Windows.Media;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;

namespace DRT
{
    // ----------------------------------------------------------------------
    // Common functionality for layout test suites.
    // ----------------------------------------------------------------------
    public class DrtVisualTreeChangeSuite : DrtSuite<DrtVisualTreeChange>
    {
        // ------------------------------------------------------------------
        // Constructor.
        // ------------------------------------------------------------------
        public DrtVisualTreeChangeSuite() : base("DrtVisualTreeChange")
        {
            Contact = "Microsoft";
        }

        // ------------------------------------------------------------------
        // Initialize tests.
        // ------------------------------------------------------------------
        public override DrtTest[] PrepareTests()
        {
            return new DrtTest[]{
                           new DrtTest(Test_NoEventsWithoutDebugger),
                           new DrtTest(Test_2D),
                           new DrtTest(Test_3D),
                           null };
        }

        private void Test_NoEventsWithoutDebugger()
        {
            if (Debugger.IsAttached)
            {
                // WARNING: this test will fail when running under a debugger.
                // Visual tree change notification is disable when debugger
                // is NOT attached and that is what we test.
                Debugger.Break();
                Drt.Fail("TEST WILL FAIL WHEN RUNNING UNDER A DEBUGGER. Step thru slowly!");
            }

            int invokationCount = 0;
            EventHandler<VisualTreeChangeEventArgs> handler = delegate(object sender, VisualTreeChangeEventArgs args)
            {
                invokationCount++;
            };

            VisualDiagnostics.VisualTreeChanged += handler;

            // Connect/disconnet Visual's
            Grid grid = new Grid();
            grid.Children.Add(new StackPanel());
            grid.Children.Add(new Button());
            grid.Children.Clear();

            VisualDiagnostics.VisualTreeChanged -= handler;
            Drt.Assert(invokationCount == 0, "Verify there are no event invokations");
        }

        private void Test_3D()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(false))
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


        private void Test_2D()
        {
            using (VisualChangeRecorder recorder = new VisualChangeRecorder(true))
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

        private void VerifyResultsAndReset(VisualChangeRecorder recorder, params string[] expectedRecords)
        {
            recorder.UpdateLayout();

            AreEqual(recorder.Result.Count, expectedRecords.Length, "Verify actual/expected record count");
            int count = Math.Min(recorder.Result.Count, expectedRecords.Length);
            for (int i = 0; i < count; i++)
            {
                // Get normalized actual and expected records
                string actual = NormalizeRecord(recorder.Result[i]);
                string expected = NormalizeRecord(expectedRecords[i]);
                AreEqual(actual, expected, "Verify record " + i);
            }

            recorder.Reset();
        }

        private static string NormalizeRecord(string record)
        {
            return record.Trim().Replace(" ", "").Replace(",", ", ");
        }

        private void AreEqual(object actual, object expected, string message)
        {
            Drt.Assert(object.Equals(actual, expected), string.Format("String values are not equal. {0}", message));
        }

        // Helper class to record changes
        private class VisualChangeRecorder : IDisposable
        {
            public List<string> Result = new List<string>();
            public Window Window;
            private object originalValue;
            private FieldInfo overrideField;

            public VisualChangeRecorder(bool createWindow)
            {
                Type type = typeof(VisualDiagnostics);
                this.overrideField = type.GetField("s_isDebuggerCheckDisabledForTestPurposes", BindingFlags.NonPublic | BindingFlags.Static);
                this.originalValue = this.overrideField.GetValue(null);
                this.overrideField.SetValue(null, true);

                if (createWindow)
                {
                    Window = new Window();
                    Window.Left = 10000; // move it off screen
                    Window.Top = 10000; // move it off screen
                    Window.Width = 500;
                    Window.Height = 800;
                    Window.ShowActivated = false;
                    Window.Show();
                }

                VisualDiagnostics.VisualTreeChanged += this.OnVisualtreeChange;
            }

            public void Dispose()
            {
                this.overrideField.SetValue(null, this.originalValue);
                VisualDiagnostics.VisualTreeChanged -= this.OnVisualtreeChange;
                if (Window != null)
                {
                    Window.Close();
                }
            }

            public void Reset()
            {
                this.Result.Clear();
            }

            public void UpdateLayout()
            {
                if (Window != null)
                {
                    Window.UpdateLayout();
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


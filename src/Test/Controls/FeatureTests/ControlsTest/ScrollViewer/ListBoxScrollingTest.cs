using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// ListBoxScrollingTest
    /// </summary>
    public class ListBoxScrollingTest : XamlTest
    {
        public ListBoxScrollingTest(Dictionary<string, string> variation, string testInfo)
            : base(variation["XamlFileName"])
        {
            this.variation = variation;
            this.testInfo = testInfo;

            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(RunTest);
        }

        private Dictionary<string, string> variation;
        private string testInfo;
        private Panel panel;
        private ListBox control;

        private ICollectionView GetFamilyView()
        {
            DataSourceProvider provider = (DataSourceProvider)panel.FindResource("Family");
            int retryCount = 10;
            while (retryCount-- > 0 && ((provider == null) || (provider.Data == null)))
            {
                LogComment("Setup is waiting for DataSourceProvider, " + retryCount + " tries left...");
                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                Thread.Sleep(1000);
            }
            return CollectionViewSource.GetDefaultView(provider.Data);
        }

        private TestResult Setup()
        {
            Status("Setup");
            LogComment(testInfo);
            string controlName = variation["ControlName"];

            panel = (Panel)RootElement.FindName("panel");
            control = (ListBox)RootElement.FindName(controlName);

            string dataBindFilter = String.Empty;

            if (variation.TryGetValue("DataDindFilter", out dataBindFilter))
            {
                dataBindFilter = "Microsoft.Test.Controls." + dataBindFilter;
                LogComment(dataBindFilter);

                IDataBindFilter filter = Assembly.GetExecutingAssembly().CreateInstance(dataBindFilter) as IDataBindFilter;

                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                ICollectionView view = GetFamilyView();

                if (view == null)
                {
                    throw new TestValidationException("Fail: view is null.");
                }

                filter.Filter(view);
            }

            System.Windows.Window.GetWindow(control).FlowDirection = (FlowDirection)Enum.Parse(typeof(FlowDirection), variation["FlowDirection"]);

            ItemsControlModifier modifier = new ItemsControlModifier(control);
            modifier.Modify(Convert.ToBoolean(variation["IsVirtualizing"]), Convert.ToBoolean(variation["IsItemVirtualizing"]), (VirtualizationMode)Enum.Parse(typeof(VirtualizationMode), variation["VirtualizationMode"]));

            // Setup live or non-live scrolling
            ScrollViewer.SetIsDeferredScrollingEnabled(control, Convert.ToBoolean(variation["IsDeferredScrollingEnabled"]));
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            // Ensure the window has focus
            InputHelper.MouseClickWindowChrome(Window);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

            LogComment("Setup was successful");
            return TestResult.Pass;
        }

        private TestResult RunTest()
        {
            ScrollingMode scrollingMode;
            int firstTopItemInViewportIndex = 0;
            int numberOfItemsInViewport = 0;
            int focusedIndex = 0;
            int expectedItemInViewportIndex = -1;
            string eventName = variation["EventName"];
            bool shouldEventFire = false;
            Orientation orientation = (Orientation)Enum.Parse(typeof(Orientation), variation["Orientation"]);

            if (variation.ContainsKey("ShouldEventFire"))
            {
                shouldEventFire = Convert.ToBoolean(variation["ShouldEventFire"]);
            }

            using (ListBoxScrollingValidator validator = ListBoxScrollingValidator.GetValidator)
            {
                switch (variation["InputType"])
                {
                    case "Code":
                    case "Mouse":
                        scrollingMode = (ScrollingMode)Enum.Parse(typeof(ScrollingMode), variation["ScrollingMode"]);

                        switch (scrollingMode)
                        {
                            case ScrollingMode.LineDown:
                            case ScrollingMode.PageDown:
                                firstTopItemInViewportIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                numberOfItemsInViewport = ViewportHelper.GetNumberOfItemsInViewport(control, firstTopItemInViewportIndex);
                                int firstBottomItemNotInViewportIndex = ViewportHelper.GetFirstBottomItemNotInViewportIndex(control, firstTopItemInViewportIndex);

                                switch (scrollingMode)
                                {
                                    case ScrollingMode.LineDown:
                                        expectedItemInViewportIndex = firstBottomItemNotInViewportIndex;
                                        break;
                                    case ScrollingMode.PageDown:
                                        // We need to subtract 2 because convert numberOfItemsInViewport to index that need to subtract 1 and start from firstBottomItemNotInViewportIndex that subtract another 1
                                        expectedItemInViewportIndex = firstBottomItemNotInViewportIndex + numberOfItemsInViewport - 2;
                                        if (expectedItemInViewportIndex > control.Items.Count - 1)
                                        {
                                            expectedItemInViewportIndex = control.Items.Count - 1;
                                        }
                                        break;
                                }
                                break;
                            case ScrollingMode.LineUp:
                            case ScrollingMode.PageUp:
                                ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
                                scrollViewer.ScrollToEnd();
                                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                                firstTopItemInViewportIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                numberOfItemsInViewport = ViewportHelper.GetNumberOfItemsInViewport(control, firstTopItemInViewportIndex);
                                switch (scrollingMode)
                                {
                                    case ScrollingMode.LineUp:
                                        expectedItemInViewportIndex = firstTopItemInViewportIndex - 1;
                                        break;
                                    case ScrollingMode.PageUp:
                                        int lastBottomItemInViewportIndex = ViewportHelper.GetLastBottomItemInViewportIndex(control, firstTopItemInViewportIndex);
                                        // We subtract numberOfItemsInViewport from firstTopItemInViewportIndex because firstTopItemInViewportIndex won't be in viewport after pageup
                                        expectedItemInViewportIndex = firstTopItemInViewportIndex - numberOfItemsInViewport;
                                        if (expectedItemInViewportIndex < 0)
                                        {
                                            expectedItemInViewportIndex = 0;
                                        }
                                        break;
                                }
                                break;
                            default:
                                throw new NotSupportedException("Fail: unsupported ScrollingMode " + scrollingMode.ToString());
                        }

                        switch (variation["InputType"])
                        {
                            case "Code":
                                validator.Validate(control, orientation, (ScrollingMode)Enum.Parse(typeof(ScrollingMode), variation["ScrollingMode"]), expectedItemInViewportIndex, eventName, shouldEventFire);
                                break;
                            case "Mouse":
                                validator.Validate(control, orientation, (ScrollingMode)Enum.Parse(typeof(ScrollingMode), variation["ScrollingMode"]), expectedItemInViewportIndex, eventName, shouldEventFire, (MouseButton)Enum.Parse(typeof(MouseButton), variation["MouseButton"]));
                                break;
                        }
                        break;
                    case "MouseWheel":
                        int scrollAmount = Convert.ToInt32(variation["ScrollAmount"]);

                        switch (scrollAmount)
                        {
                            case -1:
                                firstTopItemInViewportIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                focusedIndex = ViewportHelper.GetLastBottomItemInViewportIndex(control, firstTopItemInViewportIndex);
                                expectedItemInViewportIndex = focusedIndex + 1;
                                break;
                            case 1:
                                ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
                                scrollViewer.ScrollToEnd();
                                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                                focusedIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                expectedItemInViewportIndex = focusedIndex - 1;
                                break;
                            default:
                                throw new NotSupportedException("Fail: unsupported scrollAmount " + scrollAmount.ToString());
                        }
                        validator.Validate(control, orientation, focusedIndex, expectedItemInViewportIndex, eventName, shouldEventFire, scrollAmount);
                        break;
                    case "Keyboard":
                        Key key = (Key)Enum.Parse(typeof(Key), variation["Key"]);

                        switch (key)
                        {
                            case System.Windows.Input.Key.Down:
                                focusedIndex = ViewportHelper.GetLastBottomItemInViewportIndex(control, ViewportHelper.GetFirstTopItemInViewportIndex(control));
                                expectedItemInViewportIndex = focusedIndex + 1;
                                break;
                            case System.Windows.Input.Key.PageDown:
                                focusedIndex = ViewportHelper.GetLastBottomItemInViewportIndex(control, ViewportHelper.GetFirstTopItemInViewportIndex(control));
                                numberOfItemsInViewport = ViewportHelper.GetNumberOfItemsInViewport(control, focusedIndex);
                                if (focusedIndex + numberOfItemsInViewport > control.Items.Count - 1)
                                {
                                    expectedItemInViewportIndex = control.Items.Count - 1;
                                }
                                else
                                {
                                    expectedItemInViewportIndex = focusedIndex + numberOfItemsInViewport;
                                }
                                break;
                            case System.Windows.Input.Key.Up:
                            case System.Windows.Input.Key.PageUp:
                                ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
                                scrollViewer.ScrollToEnd();

                                switch (key)
                                {
                                    case System.Windows.Input.Key.Up:
                                        focusedIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                        expectedItemInViewportIndex = focusedIndex + 1;
                                        break;
                                    case System.Windows.Input.Key.PageUp:
                                        focusedIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                        numberOfItemsInViewport = ViewportHelper.GetNumberOfItemsInViewport(control, focusedIndex);
                                        if (focusedIndex - numberOfItemsInViewport < 0)
                                        {
                                            expectedItemInViewportIndex = 0;
                                        }
                                        else
                                        {
                                            expectedItemInViewportIndex = focusedIndex - numberOfItemsInViewport;
                                        }
                                        break;
                                }
                                break;
                            default:
                                throw new NotSupportedException("Fail: unsupported key " + key.ToString());
                        }
                        validator.Validate(control, orientation, focusedIndex, expectedItemInViewportIndex, eventName, shouldEventFire, key);
                        break;
                    case "MouseDrag":
                        DragThumbScenario dragThumbScenario = (DragThumbScenario)Enum.Parse(typeof(DragThumbScenario), variation["DragThumbScenario"]);
                        DragThumbTo dragThumbTo = default(DragThumbTo);

                        switch (dragThumbScenario)
                        {
                            case DragThumbScenario.TopToBottom:
                                focusedIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                expectedItemInViewportIndex = control.Items.Count - 1;
                                dragThumbTo = DragThumbTo.Bottom;
                                break;
                            case DragThumbScenario.TopToMiddle:
                                focusedIndex = ViewportHelper.GetFirstTopItemInViewportIndex(control);
                                expectedItemInViewportIndex = control.Items.Count / 2;
                                dragThumbTo = DragThumbTo.Middle;
                                break;
                            case DragThumbScenario.BottomToTop:
                            case DragThumbScenario.BottomToMiddle:
                                ScrollViewer scrollViewer = VisualTreeHelper.GetVisualChild<ScrollViewer, ListBox>(control);
                                scrollViewer.ScrollToEnd();
                                DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                                focusedIndex = ViewportHelper.GetLastBottomItemInViewportIndex(control, ViewportHelper.GetFirstTopItemInViewportIndex(control));
                                switch (dragThumbScenario)
                                {
                                    case DragThumbScenario.BottomToTop:
                                        expectedItemInViewportIndex = 0;
                                        dragThumbTo = DragThumbTo.Top;
                                        break;
                                    case DragThumbScenario.BottomToMiddle:
                                        expectedItemInViewportIndex = control.Items.Count / 2;
                                        dragThumbTo = DragThumbTo.Middle;
                                        break;
                                }
                                break;
                        }
                        validator.Validate(control, orientation, focusedIndex, expectedItemInViewportIndex, eventName, dragThumbTo);
                        break;
                    default:
                        throw new NotSupportedException("Fail: unsupported input type " + variation["InputType"]);
                }
            }

            return TestResult.Pass;
        }
    }
}



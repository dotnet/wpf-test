using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Threading;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Graphics;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.TestTypes;
using System.Windows.Controls.Primitives;

namespace Microsoft.Test.Controls
{


    // Both Datepicker and Cleartype support are v4.0 features
    // So this code would break the 3.5 test build if included.
    // Ideally this should be factored out into a separate DLL.
#if !TESTBUILD_CLR20

    /// <summary>
    /// <description>
    /// Verify that 'esc' key is routed up the tree
    /// when a DataGrid does not make use of it for editing scenarios.
    /// </description>

    /// </summary>
    //[Test(0, "Integration", "ClearTypeIntegrationTest", SecurityLevel = TestCaseSecurityLevel.FullTrust)]
    public class ClearTypeIntegrationTest : XamlTest
    {
        private static readonly string XamlFile = @"ClearTypeIntegration.xaml";
        private Button debugButton;
        private ComboBox comboBox;
        private DatePicker datePicker;
        private Menu menu;
        private Button buttonWithContextMenu;
        private ToolBar toolBar;

        private static int rowToScan = 8;
        private static int UISettleTime = 1000; // ms

        #region Constructor

        public ClearTypeIntegrationTest()
            : base(XamlFile)
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);
            RunSteps += new TestStep(TestClearTypeIntegrationForComboBox);
            RunSteps += new TestStep(TestClearTypeIntegrationForDatePicker);
            RunSteps += new TestStep(TestClearTypeIntegrationForMenuItem);
            RunSteps += new TestStep(TestClearTypeIntegrationForToolBarComboBox);
            RunSteps += new TestStep(TestClearTypeIntegrationForToolBarMenuItem);
            RunSteps += new TestStep(TestClearTypeIntegrationForToolBarOverflowPopup);
            RunSteps += new TestStep(TestClearTypeIntegrationForContextMenu);
            RunSteps += new TestStep(TestClearTypeIntegrationForComboBoxScrolling);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup  
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public TestResult Setup()
        {
            Status("Setup specific for ClearTypeIntegrationTest");

            debugButton = (Button)RootElement.FindName("btn_Debug");
            Assert.AssertTrue("Unable to find btn_Debug from the resources", debugButton != null);

            comboBox = (ComboBox)RootElement.FindName("comboBox");
            Assert.AssertTrue("Unable to find comboBox from the resources", comboBox != null);           

            datePicker = (DatePicker)RootElement.FindName("datePicker");
            Assert.AssertTrue("Unable to find datePicker from the resources", datePicker != null);

            menu = (Menu)RootElement.FindName("menu");
            Assert.AssertTrue("Unable to find menu from the resources", menu != null);

            buttonWithContextMenu = (Button)RootElement.FindName("buttonWithContextMenu");
            Assert.AssertTrue("Unable to find buttonWithContextMenu from the resources", buttonWithContextMenu != null);

            toolBar = (ToolBar)RootElement.FindName("toolBar");
            Assert.AssertTrue("Unable to find toolBar from the resources", toolBar != null);

            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("Setup for ClearTypeIntegrationTest was successful");
            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            debugButton = null;
            comboBox = null;
            datePicker = null;
            menu = null;
            buttonWithContextMenu = null;
            toolBar = null;
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForComboBox()
        {
            Status("TestClearTypeIntegrationForComboBox");

            //set the text rendering option to cleartype
            TextOptions.SetTextRenderingMode(comboBox, TextRenderingMode.ClearType);
            UserInput.MouseLeftClickCenter(comboBox);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var content = comboBox.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("comboBox container must not be null", content != null);

            VerifyImage(content, TextRenderingMode.ClearType, "ComboBox");

            //set the text rendering option to aliased and check that it updates and re-renders the text in realtime
            TestTextRendering(comboBox, content, TextRenderingMode.Aliased);

            //now set it to Grayscale
            TestTextRendering(comboBox, content, TextRenderingMode.Grayscale);

            LogComment("TestClearTypeIntegrationForComboBox was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForDatePicker()
        {
            Status("TestClearTypeIntegrationForDatePicker");

            datePicker.IsDropDownOpen = true;
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var calendar = (Calendar)typeof(DatePicker).InvokeMember(
                "_calendar",
                BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField,
                null,
                datePicker,
                null);
            Assert.AssertTrue("calendar from DatePicker cannot be null", calendar != null);

            VerifyImage(calendar, TextRenderingMode.ClearType, "DatePicker", 42);

            datePicker.IsDropDownOpen = false;

            LogComment("TestClearTypeIntegrationForDatePicker was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForMenuItem()
        {
            Status("TestClearTypeIntegrationForMenuItem");

            UserInput.MouseLeftClickCenter(menu);
            QueueHelper.WaitTillQueueItemsProcessed();

            var menuItem = (MenuItem)menu.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("menuItem from Menu cannot be null", menuItem != null);

            UserInput.MouseLeftClickCenter(menuItem);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var subMenuItem = menuItem.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("subMenuItem from Menu cannot be null", subMenuItem != null);

            VerifyImage(subMenuItem, TextRenderingMode.ClearType, "MenuItem");

            //set the text rendering option to aliased and check that it updates and re-renders the text in realtime
            TestTextRendering(menu, subMenuItem, TextRenderingMode.Aliased);

            //now set it to Grayscale
            TestTextRendering(menu, subMenuItem, TextRenderingMode.Grayscale);

            UserInput.MouseLeftClickCenter(menu);
            QueueHelper.WaitTillQueueItemsProcessed();

            LogComment("TestClearTypeIntegrationForMenuItem was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForContextMenu()
        {
            Status("TestClearTypeIntegrationForContextMenu");

            UserInput.MouseLeftClickCenter(buttonWithContextMenu);
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.MouseRightClickCenter(buttonWithContextMenu);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var textBlock = VisualTreeUtils.GetVisualChild<TextBlock>(buttonWithContextMenu.ContextMenu);
            Assert.AssertTrue("textBlock must not be null", textBlock != null);

            VerifyImage(textBlock, TextRenderingMode.ClearType, "ContextMenu");

            //set the text rendering option to aliased and check that it updates and re-renders the text in realtime
            TestTextRendering(buttonWithContextMenu, textBlock, TextRenderingMode.Aliased);

            //now set it to Grayscale
            TestTextRendering(buttonWithContextMenu, textBlock, TextRenderingMode.Grayscale);

            LogComment("TestClearTypeIntegrationForContextMenu was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForToolBarComboBox()
        {
            Status("TestClearTypeIntegrationForToolBarComboBox");

            LogComment("verify the combobox in the toolbar");
            var toolBarComboBox = VisualTreeUtils.GetVisualChild<ComboBox>(toolBar);
            UserInput.MouseLeftClickCenter(toolBarComboBox);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var content = toolBarComboBox.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("comboBox container must not be null", content != null);

            VerifyImage(content, TextRenderingMode.ClearType, "ToolBarComboBox");

            LogComment("TestClearTypeIntegrationForToolBarComboBox was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForToolBarMenuItem()
        {
            Status("TestClearTypeIntegrationForToolBarMenuItem");

            LogComment("verify the combobox in the toolbar");
            var menu = VisualTreeUtils.GetVisualChild<Menu>(toolBar);

            UserInput.MouseLeftClickCenter(menu);
            QueueHelper.WaitTillQueueItemsProcessed();

            var menuItem = (MenuItem)menu.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("menuItem from Menu cannot be null", menuItem != null);

            UserInput.MouseLeftClickCenter(menuItem);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var subMenuItem = menuItem.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("subMenuItem from Menu cannot be null", subMenuItem != null);

            VerifyImage(subMenuItem, TextRenderingMode.ClearType, "ToolBarMenuItem");

            LogComment("TestClearTypeIntegrationForToolBarMenuItem was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForToolBarOverflowPopup()
        {
            Status("TestClearTypeIntegrationForToolBarOverflowPopup");

            toolBar.IsOverflowOpen = true;
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            var content = (TextBlock)toolBar.ItemContainerGenerator.ContainerFromIndex(toolBar.Items.Count - 1);
            VerifyImage(content, TextRenderingMode.ClearType, "ToolBarOverflow");

            toolBar.IsOverflowOpen = false;

            LogComment("TestClearTypeIntegrationForToolBarOverflowPopup was successful");
            return TestResult.Pass;
        }

        private TestResult TestClearTypeIntegrationForComboBoxScrolling()
        {
            Status("TestClearTypeIntegrationForComboBoxScrolling");

            //set the text rendering option to cleartype
            TextOptions.SetTextRenderingMode(comboBox, TextRenderingMode.ClearType);
            UserInput.MouseLeftClickCenter(comboBox);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            //find the scrollviewer and scroll down to the end
            var popup = VisualTreeUtils.GetVisualChild<Popup>(comboBox);
            Assert.AssertTrue("Unable to find popup from the resources", popup != null);
            var scrollViewer = VisualTreeUtils.GetVisualChild<ScrollViewer>(popup.Child);
            Assert.AssertTrue("Unable to find scrollViewer from the resources", scrollViewer != null);

            scrollViewer.ScrollToEnd();
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();

            // this is a specific container that is at the top of the view when scrolling down
            var content = comboBox.ItemContainerGenerator.ContainerFromIndex(13);
            Assert.AssertTrue("comboBox container must not be null", content != null);

            LogComment("Verifying ComboBoxItem: " + content);
            VerifyImage(content, TextRenderingMode.ClearType, "ComboBox");        

            LogComment("TestClearTypeIntegrationForComboBoxScrolling was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps

        private void TestTextRendering(DependencyObject parentElement, DependencyObject content, TextRenderingMode textRenderMode)
        {
            TextOptions.SetTextRenderingMode(parentElement, textRenderMode);
            WaitFor(UISettleTime);
            QueueHelper.WaitTillQueueItemsProcessed();
            VerifyImage(content as UIElement, textRenderMode, parentElement.GetType().ToString() + "-" + textRenderMode.ToString());
        }

        private void VerifyImage(DependencyObject content, TextRenderingMode textRenderType, string captureImageName)
        {
            VerifyImage(content, textRenderType, captureImageName, rowToScan);
        }

        private void VerifyImage(DependencyObject content, TextRenderingMode textRenderType, string captureImageName, int row)
        {
            var capture = ImageUtility.CaptureElement(content as UIElement);
            var clearTypeDetected = ClearTypeDetector.CheckForRedBlueShift(capture, row);
            bool clearTypeExpected = textRenderType == TextRenderingMode.ClearType ? true : false;

            //log the image  
            string saveFile = XamlFile + captureImageName + ".bmp";
            Status("Saving captured image to " + saveFile);
            ImageAdapter img = new ImageAdapter(capture);
            ImageUtility.ToImageFile(img, saveFile);
            Log.LogFile(saveFile);

            if (clearTypeDetected == clearTypeExpected)
            {
                Status("Results match expected behaviour - PASS");
            }
            else
            {
                Status("Results do not match expected behaviour - FAIL at rowToScan: " + row);

                throw new TestValidationException("ClearType verification failure.");
            }
        }

        private void Debug()
        {
            // To keep this thread busy, we'll have to push a frame.
            DispatcherFrame frame = new DispatcherFrame();

            debugButton.MouseLeftButtonDown += (sender, e) =>
            {
                frame.Continue = false;
            };

            Dispatcher.PushFrame(frame);
        }
    }

#endif

}

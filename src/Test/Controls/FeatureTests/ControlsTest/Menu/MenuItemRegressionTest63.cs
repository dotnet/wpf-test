using System.Windows;
using System.Windows.Controls;
using Avalon.Test.ComponentModel;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Discovery;
using Microsoft.Test.Input;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// Regression Test   Verify that MenuItem.IsSubmenuOpen DP does not lose it's trigger value source
    /// </description>

    /// </summary>
    [Test(0, "MenuItem", "MenuItemRegressionTest63", SecurityLevel = TestCaseSecurityLevel.FullTrust, Versions = "4.0+,4.0Client+")] // Bug fixed in 4.0 and not back-ported.
    public class MenuItemRegressionTest63 : RegressionXamlTest
    {
        private Menu menu;

        #region Constructor

        public MenuItemRegressionTest63()
            : base(@"MenuItemRegressionTest63.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            RunSteps += new TestStep(TestIsSubMenuOpenTrigger);
        }

        #endregion

        #region Test Steps

        /// <summary>
        /// Initial Setup
        /// </summary>
        /// <returns>true if all is fine; false otherwise.</returns>
        public override TestResult Setup()
        {
            Status("Setup specific for MenuItemRegressionTest63");

            menu = (Menu)RootElement.FindName("Menu");
            Assert.AssertTrue("Unable to find Menu from the resources", menu != null);

            QueueHelper.WaitTillQueueItemsProcessed();

            // Uncomment for ad-hoc debugging
            //base.Setup();

            LogComment("Setup for MenuItemRegressionTest63 was successful");
            return TestResult.Pass;
        }

        public override TestResult CleanUp()
        {
            menu = null;
            return base.CleanUp();
        }
        /// <summary>
        /// 1. click on the file menu item
        /// 2. mouse over menu item 1 => verify IsSubmenuOpen has a BaseValueSource == trigger
        /// 3. click the submenu item
        /// 4. Repeat steps 1 and 2 and verify that the BaseValueSource is still trigger
        /// </summary>
        private TestResult TestIsSubMenuOpenTrigger()
        {
            Status("TestIsSubMenuOpenTrigger");

            MenuItem fileMenuItem = (MenuItem)menu.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("fileMenuItem cannot be null for test to continue", fileMenuItem != null);

            LogComment(string.Format("1. click on the file menu item: {0}", fileMenuItem.Header.ToString()));
            fileMenuItem.IsSubmenuOpen = true;
            QueueHelper.WaitTillQueueItemsProcessed();
            this.WaitFor(1000);

            MenuItem subMenuItem = (MenuItem)fileMenuItem.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("subMenuItem cannot be null for test to continue", subMenuItem != null);

            LogComment(string.Format("2. mouse over the menu item 1: {0}", subMenuItem.Header.ToString()));
            UserInput.MouseMove(subMenuItem, 2, 2);
            QueueHelper.WaitTillQueueItemsProcessed();
            this.WaitFor(1000);

            LogComment("2a. verify IsSubmenuOpen BaseValueSource");
            ValueSource valueSource = DependencyPropertyHelper.GetValueSource(subMenuItem, MenuItem.IsSubmenuOpenProperty);
            Assert.AssertTrue(
                string.Format("expected subMenuItem.IsSubmenuOpen: {0}, actual: {1}", BaseValueSource.StyleTrigger, valueSource.BaseValueSource),
                valueSource.BaseValueSource == BaseValueSource.StyleTrigger);

            MenuItem subSubMenuItem = (MenuItem)subMenuItem.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("subSubMenuItem cannot be null for test to continue", subSubMenuItem != null);

            LogComment(string.Format("3. click the sub-submenu item: {0}", subSubMenuItem.Header.ToString()));
            UserInput.MouseLeftClickCenter(subSubMenuItem);
            QueueHelper.WaitTillQueueItemsProcessed();
            this.WaitFor(1000);

            LogComment(string.Format("4. click on the file menu item again", fileMenuItem.Header.ToString()));
            fileMenuItem.IsSubmenuOpen = true;
            QueueHelper.WaitTillQueueItemsProcessed();
            this.WaitFor(1000);

            subMenuItem = (MenuItem)fileMenuItem.ItemContainerGenerator.ContainerFromIndex(0);
            Assert.AssertTrue("subMenuItem cannot be null for test to continue", subMenuItem != null);

            LogComment(string.Format("4a. mouse over the menu item 1", subMenuItem.Header.ToString()));
            UserInput.MouseMove(subMenuItem, 2, 2);
            QueueHelper.WaitTillQueueItemsProcessed();
            this.WaitFor(1000);

            LogComment("4b. verify IsSubmenuOpen BaseValueSource has not changed");
            valueSource = DependencyPropertyHelper.GetValueSource(subMenuItem, MenuItem.IsSubmenuOpenProperty);
            Assert.AssertTrue(
                string.Format("expected subMenuItem.IsSubmenuOpen: {0}, actual: {1}", BaseValueSource.StyleTrigger, valueSource.BaseValueSource),
                valueSource.BaseValueSource == BaseValueSource.StyleTrigger);

            LogComment("TestIsSubMenuOpenTrigger was successful");
            return TestResult.Pass;
        }

        #endregion Test Steps
    }
}

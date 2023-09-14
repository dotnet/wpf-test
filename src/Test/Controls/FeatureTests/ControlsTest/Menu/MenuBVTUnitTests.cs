using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.RenderingVerification.Model.Analytical;
using Microsoft.Test.RenderingVerification;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(Menu))]
    public class MenuKeySelectMenuItemBVT : IUnitTest
    {
        public MenuKeySelectMenuItemBVT()
        {
        }

        /// <summary>
        /// Test press down arrow to highlight the second MenuItem
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Menu menu = (Menu)obj;
            MenuItem menuHeader = menu.Items[0] as MenuItem;
            MenuItem menuItem2 = menuHeader.Items[1] as MenuItem;

            UserInput.KeyPress("LeftAlt");
            QueueHelper.WaitTillQueueItemsProcessed();

            UserInput.KeyPress("Down");
            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!menuItem2.IsHighlighted)
            {
                TestLog.Current.LogDebug("Fail: MenuItem2 is not highlighted.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }
    }

    [TargetType(typeof(Menu))]
    public class MenuMouseClickMenuItemBVT : IUnitTest
    {
        public MenuMouseClickMenuItemBVT()
        {
        }

        /// <summary>
        /// Test mouse click menuHeader
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Menu menu = (Menu)obj;
            isClicked = false;

            MenuItem menuHeader = menu.Items[1] as MenuItem;
            menuHeader.Click += new RoutedEventHandler(OnClick);

            UserInput.MouseLeftClickCenter(menuHeader);
            QueueHelper.WaitTillQueueItemsProcessed();

            if (!isClicked)
            {
                TestLog.Current.LogDebug("Fail: Menu Click event does not fire.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        protected void OnClick(object e, RoutedEventArgs args)
        {
            isClicked = true;
        }

        //necessary because of testing Click event
        bool isClicked;
    }
    [TargetType(typeof(Menu))]
    public class MenuMouseClickSubMenuItemBVT : IUnitTest
    {
        public MenuMouseClickSubMenuItemBVT()
        {
        }

        /// <summary>
        /// Test mouse click menuHeader
        /// </summary>
        /// <param name="menu"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Menu menu = (Menu)obj;
            isClicked = false;

            MenuItem menuHeader = menu.Items[0] as MenuItem;
            TestLog.Current.LogEvidence("menuHeader.");

            MenuItem menuItem3 = menuHeader.Items[3] as MenuItem;
            TestLog.Current.LogEvidence("menuItem2.");

            MenuItem subMenuItem1 = menuItem3.Items[0] as MenuItem;
            TestLog.Current.LogEvidence("subMenuItem1.");

            subMenuItem1.AddHandler(MenuItem.ClickEvent, new RoutedEventHandler(OnClick));

            UserInput.KeyPress("LeftAlt");
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogEvidence("LeftAlt.");

            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();
            UserInput.KeyPress("Down");
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogEvidence("Down. Down. Down.");

            UserInput.KeyPress("Right");
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogEvidence("Right.");

            UserInput.KeyPress("Enter");
            QueueHelper.WaitTillQueueItemsProcessed();
            TestLog.Current.LogEvidence("Enter.");

            if (!isClicked)
            {
                TestLog.Current.LogDebug("Fail: subMenu Click event does not fire.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        protected void OnClick(object e, RoutedEventArgs args)
        {
            isClicked = true;
        }

        //necessary because of testing Click event
        bool isClicked;
    }
}



using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.IntegrationTests
{
    public static class OpenAndCloseMenu
    {
        /// <summary>
        /// Open and close menu.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public static bool RunTest(object obj, XmlElement variation, string key)
        {
            // Turn-off IME for these tests.
            InputMethod.Current.ImeState = InputMethodState.Off;

            if (BrowserInteropHelper.IsBrowserHosted)
            {
                GlobalLog.LogEvidence("+++ Run in Browser mode");
            }
            else
            {
                GlobalLog.LogEvidence("+++ Run in Standalone mode");
                Panel rootPanel = (Panel)obj;
                int numberOfControls = 0;

                ButtonFamilyTestHelper.AddButtonFamilyTestContent(rootPanel, variation, ref numberOfControls);

                for (int i = 0; i < numberOfControls; i++)
                {
                    Menu testObject = rootPanel.Children[i] as Menu;
                    QueueHelper.WaitTillQueueItemsProcessed();

                    if (testObject.IsEnabled && testObject.IsMainMenu)
                    {
                        MenuItem mh1 = testObject.Items[0] as MenuItem;
                        if (mh1.IsSubmenuOpen)
                        {
                            if (testObject.FlowDirection == FlowDirection.LeftToRight)
                            {
                                UserInput.MouseLeftDown(testObject, 5, 5);
                                UserInput.MouseLeftUp(testObject, 5, 5);
                            }
                            else
                            {
                                UserInput.MouseLeftDown(testObject, (int)testObject.ActualWidth - 5, 5);
                                UserInput.MouseLeftUp(testObject, (int)testObject.ActualWidth - 5, 5);
                            }
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }

                        mh1.SubmenuOpened += new RoutedEventHandler(mi_SubmenuOpened);
                        mh1.SubmenuClosed += new RoutedEventHandler(mi_SubmenuClosed);

                        switch (key)
                        {
                            case "AltDown":
                                UserInput.KeyPress("LeftAlt");
                                QueueHelper.WaitTillQueueItemsProcessed();
                                UserInput.KeyPress("Down");
                                break;
                            case "AltEnter":
                                UserInput.KeyPress("LeftAlt");
                                QueueHelper.WaitTillQueueItemsProcessed();
                                UserInput.KeyPress("Enter");
                                break;
                            case "F10Down":
                                UserInput.KeyPress("LeftAlt");
                                QueueHelper.WaitTillQueueItemsProcessed();
                                UserInput.KeyPress("Down");
                                break;
                            case "F10Enter":
                                UserInput.KeyPress("F10");
                                QueueHelper.WaitTillQueueItemsProcessed();
                                UserInput.KeyPress("Enter");
                                break;
                            case "Click":
                                if (testObject.FlowDirection == FlowDirection.LeftToRight)
                                {
                                    UserInput.MouseLeftDown(testObject, 5, 5);
                                    UserInput.MouseLeftUp(testObject, 5, 5);
                                }
                                else
                                {
                                    UserInput.MouseLeftDown(testObject, (int)testObject.ActualWidth - 5, 5);
                                    UserInput.MouseLeftUp(testObject, (int)testObject.ActualWidth - 5, 5);
                                }
                                break;
                            default:
                                GlobalLog.LogEvidence("UnKnown Test Case.");
                                return false;
                        }
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (key == "Click")
                        {
                            if (!mh1.IsSubmenuOpen || !menuItemSubmenuOpened)
                            {
                                GlobalLog.LogEvidence("After click to open MenuItem Header, it is not opened or SubmenuOpened event didn't fire.");
                                return false;
                            }
                        }
                        else
                        {
                            if (!mh1.IsHighlighted || !mh1.IsSubmenuOpen || !menuItemSubmenuOpened)
                            {
                                GlobalLog.LogEvidence("When try to open MenuItem Header, it is not Hightlighted or it is not opened or SubmenuOpened event didn't fire.");
                                return false;
                            }
                        }

                        UserInput.KeyPress("Escape");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        if (mh1.IsSubmenuOpen || !menuItemSubmenuClosed)
                        {
                            GlobalLog.LogEvidence("When try to close MenuItem, it is opened or SubmenuClosed event didn't fire.");
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        static void mi_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            menuItemSubmenuOpened = true;
        }

        static void mi_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            menuItemSubmenuClosed = true;
        }

        //nessary because of testing events
        static bool menuItemSubmenuClosed = false;
        static bool menuItemSubmenuOpened = false;
    }


    public class MenuAltDownIntegrationTest : IIntegrationTest
    {
        public MenuAltDownIntegrationTest()
        {
        }
        /// <summary>
        /// Test Alt+Down to open MenuItem andthen  Escape to close MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (OpenAndCloseMenu.RunTest(obj, variation, "AltDown"))
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }

    public class MenuAltEnterIntegrationTest : IIntegrationTest
    {
        public MenuAltEnterIntegrationTest()
        {
        }
        /// <summary>
        /// Test Alt+Enter to open MenuItem and then Escape to close MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (OpenAndCloseMenu.RunTest(obj, variation, "AltEnter"))
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }

    public class MenuF10DownIntegrationTest : IIntegrationTest
    {
        public MenuF10DownIntegrationTest()
        {
        }
        /// <summary>
        /// Test F10+Down to open MenuItem and then Escape to close MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (OpenAndCloseMenu.RunTest(obj, variation, "F10Down"))
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }
    public class MenuF10EnterIntegrationTest : IIntegrationTest
    {
        public MenuF10EnterIntegrationTest()
        {
        }
        /// <summary>
        /// Test F10+Enter to open MenuItem and then Escape to close MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (OpenAndCloseMenu.RunTest(obj, variation, "F10Enter"))
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }

    public class MenuItemHeaderClickIntegrationTest : IIntegrationTest
    {
        public MenuItemHeaderClickIntegrationTest()
        {
        }
        /// <summary>
        /// Test click on menuitem header to open MenuItem and then Escape to close MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (OpenAndCloseMenu.RunTest(obj, variation, "Click"))
                return TestResult.Pass;
            else
                return TestResult.Fail;
        }
    }

    public class MenuArrowKeyToOpenNextMenuHeaderIntegrationTest : IIntegrationTest
    {
        public MenuArrowKeyToOpenNextMenuHeaderIntegrationTest()
        {
        }
        /// <summary>
        /// Test press Arrow Key To Open Next MenuHeader.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                GlobalLog.LogEvidence("### Run in Browser mode");
            }
            else
            {
                GlobalLog.LogEvidence("### Run in Standalone mode");
                Panel rootPanel = (Panel)obj;
                int numberOfControls = 0;

                ButtonFamilyTestHelper.AddButtonFamilyTestContent(rootPanel, variation, ref numberOfControls);

                for (int i = 0; i < numberOfControls; i++)
                {
                    Menu testObject = rootPanel.Children[i] as Menu;
                    MenuItem mh2 = new MenuItem();
                    mh2.Header = "Header2";
                    testObject.Items.Add(mh2);
                    MenuItem mi2 = new MenuItem();
                    mi2.Header = "Item2";
                    mh2.Items.Add(mi2);

                    if (testObject.IsEnabled && testObject.IsMainMenu)
                    {
                        MenuItem mh1 = testObject.Items[0] as MenuItem;
                        mh1.SubmenuOpened += new RoutedEventHandler(mh1_SubmenuOpened);
                        mh1.SubmenuClosed += new RoutedEventHandler(mh1_SubmenuClosed);

                        mh2.SubmenuOpened += new RoutedEventHandler(mh2_SubmenuOpened);
                        mh2.SubmenuClosed += new RoutedEventHandler(mh2_SubmenuClosed);

                        UserInput.KeyPress("LeftAlt");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress("Down");
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (!mh1.IsHighlighted || !mh1.IsSubmenuOpen || !menuItem1SubmenuOpened)
                        {
                            GlobalLog.LogEvidence("After Press Alt+Down Key to open the first MenuItem, it is not Hightlighted or it is not opened or SubmenuOpened event didn't fire.");
                            return TestResult.Fail;
                        }

                        if (testObject.FlowDirection == FlowDirection.LeftToRight)
                        {
                            UserInput.KeyPress("Right");
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }
                        else if (testObject.FlowDirection == FlowDirection.RightToLeft)
                        {
                            UserInput.KeyPress("Left");
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }

                        if (mh1.IsSubmenuOpen || !menuItem1SubmenuClosed)
                        {
                            GlobalLog.LogEvidence("After press arrow key to close MenuItem, it is opened or SubmenuClosed event didn't fire.");
                            return TestResult.Fail;
                        }

                        if (!mh2.IsHighlighted || !mh2.IsSubmenuOpen || !menuItem2SubmenuOpened)
                        {
                            GlobalLog.LogEvidence("After press arrow key to open the second MenuItem, it is not Hightlighted or it is not opened or SubmenuOpened event didn't fire.");
                            return TestResult.Fail;
                        }
                        UserInput.KeyPress("Escape");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        if (mh2.IsSubmenuOpen || !menuItem2SubmenuClosed)
                        {
                            GlobalLog.LogEvidence("After press Escape key to close MenuItem, it is opened or SubmenuClosed event didn't fire.");
                            return TestResult.Fail;
                        }
                    }
                }
            }
            return TestResult.Pass;
        }

        void mh1_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuOpened = true;
        }

        void mh1_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuClosed = true;
        }
        void mh2_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            menuItem2SubmenuOpened = true;
        }

        void mh2_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            menuItem2SubmenuClosed = true;
        }

        //nessary because of testing events
        bool menuItem1SubmenuClosed = false;
        bool menuItem1SubmenuOpened = false;
        bool menuItem2SubmenuClosed = false;
        bool menuItem2SubmenuOpened = false;
    }
    public class MenuArrowKeyToOpenSubMenuItemIntegrationTest : IIntegrationTest
    {
        public MenuArrowKeyToOpenSubMenuItemIntegrationTest()
        {
        }
        /// <summary>
        /// Test press Arrow Key To Open sub menuitem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                GlobalLog.LogEvidence("### Run in Browser mode");
            }
            else
            {
                GlobalLog.LogEvidence("### Run in Standalone mode");
                Panel rootPanel = (Panel)obj;
                int numberOfControls = 0;

                ButtonFamilyTestHelper.AddButtonFamilyTestContent(rootPanel, variation, ref numberOfControls);

                for (int i = 0; i < numberOfControls; i++)
                {
                    Menu testObject = rootPanel.Children[i] as Menu;
                    MenuItem mh1 = testObject.Items[0] as MenuItem;
                    MenuItem mh2 = new MenuItem();
                    mh2.Header = "Header2";
                    testObject.Items.Add(mh2);
                    
                    MenuItem mi2 = new MenuItem();
                    mi2.Header = "Item2";
                    mh2.Items.Add(mi2);
                    
                    MenuItem mi12 = new MenuItem();
                    mi12.Header = "Item12";
                    mh1.Items.Add(mi12);

                    MenuItem mii1 = new MenuItem();
                    mii1.Header = "IItem1";
                    mi12.Items.Add(mii1);

                    if (testObject.IsEnabled && testObject.IsMainMenu)
                    {
                        mh1.SubmenuOpened += new RoutedEventHandler(mh1_SubmenuOpened);
                        mh1.SubmenuClosed += new RoutedEventHandler(mh1_SubmenuClosed);

                        mi12.SubmenuOpened += new RoutedEventHandler(mi12_SubmenuOpened);
                        mi12.SubmenuClosed += new RoutedEventHandler(mi12_SubmenuClosed);

                        UserInput.KeyPress("LeftAlt");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress("Down");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress("Down");
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (!mh1.IsHighlighted || !mh1.IsSubmenuOpen || !menuItem1SubmenuOpened)
                        {
                            GlobalLog.LogEvidence("After Press Alt+Down Key to open the first MenuItem, it is not Hightlighted or it is not opened or SubmenuOpened event didn't fire.");
                            return TestResult.Fail;
                        }

                        if (testObject.FlowDirection == FlowDirection.LeftToRight)
                        {
                            UserInput.KeyPress("Right");
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }
                        else if (testObject.FlowDirection == FlowDirection.RightToLeft)
                        {
                            UserInput.KeyPress("Left");
                            QueueHelper.WaitTillQueueItemsProcessed();
                        }

                        if (!mi12.IsSubmenuOpen || !mii1.IsHighlighted || !subMenuItem1SubmenuOpened)
                        {
                            GlobalLog.LogEvidence("After press arrow key to open the sub MenuItem, it is not opened or it is not hightlighted.");
                            return TestResult.Fail;
                        }

                        UserInput.KeyPress("Escape");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        if (mi12.IsSubmenuOpen || !subMenuItem1SubmenuClosed)
                        {
                            GlobalLog.LogEvidence("After press Escape key to close subMenuItem, it is opened or SubmenuClosed event didn't fire.");
                            return TestResult.Fail;
                        }
                        UserInput.KeyPress("Escape");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        if (mh1.IsSubmenuOpen || !menuItem1SubmenuClosed)
                        {
                            GlobalLog.LogEvidence("After press Escape key to close MenuHeader, it is opened or SubmenuClosed event didn't fire.");
                            return TestResult.Fail;
                        }
                    }
                }
            }
            return TestResult.Pass;
        }

        void mh1_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuOpened = true;
        }

        void mh1_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuClosed = true;
        }
        void mi12_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            subMenuItem1SubmenuOpened = true;
        }

        void mi12_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            subMenuItem1SubmenuClosed = true;
        }

        //nessary because of testing events
        bool menuItem1SubmenuClosed = false;
        bool menuItem1SubmenuOpened = false;
        bool subMenuItem1SubmenuClosed = false;
        bool subMenuItem1SubmenuOpened = false;
    }

    public class MenuDownArrowToSelectNextMenuItemIntegrationTest : IIntegrationTest
    {
        public MenuDownArrowToSelectNextMenuItemIntegrationTest()
        {
        }
        /// <summary>
        /// Test press Down Arrow Key To Select Next MenuItem.
        /// </summary>
        /// <param name="rootPanel"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            if (BrowserInteropHelper.IsBrowserHosted)
            {
                GlobalLog.LogEvidence("### Run in Browser mode");
            }
            else
            {
                GlobalLog.LogEvidence("### Run in Standalone mode");
                Panel rootPanel = (Panel)obj;
                int numberOfControls = 0;

                ButtonFamilyTestHelper.AddButtonFamilyTestContent(rootPanel, variation, ref numberOfControls);

                for (int i = 0; i < numberOfControls; i++)
                {
                    Menu testObject = rootPanel.Children[i] as Menu;

                    if (testObject.IsEnabled && testObject.IsMainMenu)
                    {
                        MenuItem mh1 = testObject.Items[0] as MenuItem;
                        MenuItem mi1 = mh1.Items[0] as MenuItem;
                        MenuItem mi2 = new MenuItem();
                        mi2.Header = "Item2";
                        mh1.Items.Add(mi2);
                        MenuItem mi3 = new MenuItem();
                        mi3.Header = "Item3";
                        mh1.Items.Add(mi3);

                        mh1.SubmenuOpened += new RoutedEventHandler(mh1_SubmenuOpened);
                        mh1.SubmenuClosed += new RoutedEventHandler(mh1_SubmenuClosed);

                        UserInput.KeyPress("LeftAlt");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        UserInput.KeyPress("Down");
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (!mh1.IsHighlighted || !mh1.IsSubmenuOpen || !menuItem1SubmenuOpened)
                        {
                            GlobalLog.LogEvidence("After Press Alt+Down Key to open the first MenuItem, it is not Hightlighted or it is not opened or SubmenuOpened event didn't fire.");
                            return TestResult.Fail;
                        }

                        UserInput.KeyPress("Down");
                        QueueHelper.WaitTillQueueItemsProcessed();

                        if (mi1.IsEnabled)
                        {
                            if (!mi2.IsHighlighted)
                            {
                                GlobalLog.LogEvidence("After press down arrow key to select the second MenuItem, it is not highlighted.");
                                return TestResult.Fail;
                            }
                        }
                        else
                        {
                            if (!mi3.IsHighlighted)
                            {
                                GlobalLog.LogEvidence("After press down arrow key to select the third MenuItem, it is not highlighted.");
                                return TestResult.Fail;
                            }
                        }

                        UserInput.KeyPress("Escape");
                        QueueHelper.WaitTillQueueItemsProcessed();
                        if (mh1.IsSubmenuOpen || !menuItem1SubmenuClosed)
                        {
                            GlobalLog.LogEvidence("After press Escape key to close MenuItem, it is opened or SubmenuClosed event didn't fire.");
                            return TestResult.Fail;
                        }
                    }
                }
            }
            return TestResult.Pass;
        }

        void mh1_SubmenuOpened(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuOpened = true;
        }

        void mh1_SubmenuClosed(object sender, RoutedEventArgs e)
        {
            menuItem1SubmenuClosed = true;
        }

        //nessary because of testing events
        bool menuItem1SubmenuClosed = false;
        bool menuItem1SubmenuOpened = false;
    }
}



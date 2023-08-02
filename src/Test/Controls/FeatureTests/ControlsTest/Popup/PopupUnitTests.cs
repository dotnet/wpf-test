using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Markup;
using System.Xml;
using Microsoft.Test.Logging;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.RenderingVerification;
using System.Security;
using Avalon.Test.ComponentModel.IntegrationTests;
using Avalon.Test.ComponentModel.Actions;


namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(Popup))]
    public class PopupPlacementCenterUnitTest : IUnitTest
    {
        public PopupPlacementCenterUnitTest()
        {
        }

        /// <summary>
        /// Test PopupPlacementCenter.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Popup popup = (Popup)obj;

            popup.IsOpen = true;
            popup.Placement = PlacementMode.Center;
            popup.StaysOpen = false;

            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));
            if (popup.Placement == PlacementMode.Center)
            {
                return TestResult.Pass;
            }
            else
            {
                return TestResult.Fail;
            }
        }
    }
 
    public class OpenPopupBeforeLoadedTest : IUnitTest
    {
        Popup popup = null;
        public OpenPopupBeforeLoadedTest()
        {
        }
        /// <summary>
        /// Test Open Popup Before Main page is Loaded.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            popup = obj as Popup;
            Panel panel = popup.Parent as Panel;

            Canvas canvas = new Canvas();
            Border border = new Border();
            Canvas.SetLeft(border, 100);
            Canvas.SetTop(border, 100);
            border.Width = 200;
            border.Height = 200;
            border.Background = new SolidColorBrush(Colors.LightBlue);

            Popup testObj = new Popup();
            TextBlock textblock = new TextBlock();
            textblock.Width = 100;
            textblock.Height = 100;
            textblock.Text = "OpenPopupBeforeLoaded";
            textblock.Background = new SolidColorBrush(Colors.Red);
            testObj.Child = textblock;

            try
            {
                testObj.IsOpen = true;
                panel.Children.Add(canvas);
                canvas.Children.Add(border);
                border.Child = testObj;
                return TestResult.Fail;
            }
            catch (SecurityException e)
            {
                TestLog.Current.LogEvidence(e);
                return TestResult.Pass;
            }
        }

    }
    [TargetType(typeof(Popup))]
    public class PopupExceptionTest : IUnitTest
    {
        Popup popup = null;
        public PopupExceptionTest()
        {
        }
        /// <summary>
        /// Test Popup Exception. Exception occurs open popup inside Closed event call back.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            popup = obj as Popup;
            try
            {
                popup.Closed += new EventHandler(popup_Closed);
                popup.IsOpen = true;
                popup.IsOpen = false;
                return TestResult.Fail;
            }
            catch (InvalidOperationException e)
            {
                TestLog.Current.LogEvidence(e);
                return TestResult.Pass;
            }
        }

        void popup_Closed(object sender, EventArgs e)
        {
            popup.IsOpen = true;
        }
    }

    public class PopupTransparencyHitTest : IUnitTest
    {
        bool invoked = false;
        public PopupTransparencyHitTest()
        {
        }
        /// <summary>
        /// Test Popup Transparency.
        /// </summary>
        /// <param name="popup"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            int numberOfControls = 1;

            ButtonFamilyTestHelper.AddButtonFamilyTestContent(panel, variation, ref numberOfControls);

            Button button = ((Canvas)panel.Children[0]).Children[0] as Button;
            Popup popup = ((Canvas)panel.Children[0]).Children[1] as Popup;
            CheckBox checkbox = ((Canvas)panel.Children[0]).Children[2] as CheckBox;

            // Action
            ControlMouseLeftClickAction performAction = new ControlMouseLeftClickAction();
            performAction.Do(checkbox);
            QueueHelper.WaitTillTimeout(new TimeSpan(0, 0, 0, 1));

            button.Click += new RoutedEventHandler(button_Click);

            // Action
            performAction.Do(button);
            QueueHelper.WaitTillQueueItemsProcessed();

            TestLog.Current.LogDebug("Popup AllowsTransparency = " + popup.AllowsTransparency.ToString());

            if (popup.AllowsTransparency)
            {
                if (invoked)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
            else
            {
                if (!invoked)
                {
                    return TestResult.Pass;
                }
                else
                {
                    return TestResult.Fail;
                }
            }
        }

        void button_Click(object sender, RoutedEventArgs e)
        {
            invoked = true;
        }

    }

}



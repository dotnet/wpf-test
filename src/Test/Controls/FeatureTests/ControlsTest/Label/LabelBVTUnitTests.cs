using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Xml;
using Avalon.Test.ComponentModel.Validations;
using Avalon.Test.ComponentModel.Actions;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;
using Microsoft.Test.Input;

namespace Avalon.Test.ComponentModel.UnitTests
{
    [TargetType(typeof(Label))]
    public class LabelTargetTextBoxBVT : IUnitTest
    {
        public LabelTargetTextBoxBVT()
        {
        }

        /// <summary>
        /// Test press access key to move focus from label to textbox
        /// </summary>
        /// <param name="label"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            TextBox textbox = panel.Children[0] as TextBox;
            Label label = panel.Children[1] as Label;
            label.Content = "_Press P To get to the TextBox:";
            label.Target = textbox;
            isGotKeyboardFocus = false;
            textbox.GotKeyboardFocus += new System.Windows.Input.KeyboardFocusChangedEventHandler(OnGotKeyboardFocus);

            UserInput.KeyDown("LeftAlt");
            UserInput.KeyDown("P");
            UserInput.KeyUp("P");
            UserInput.KeyUp("LeftAlt");

            QueueHelper.WaitTillQueueItemsProcessed();

            if (!isGotKeyboardFocus)
            {
                TestLog.Current.LogDebug("Fail: TextBox GotKeyboardeFocus event does not fire.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        protected void OnGotKeyboardFocus(object sender, System.Windows.Input.KeyboardFocusChangedEventArgs e)
        {
            isGotKeyboardFocus = true;
        }

        //necessary because of testing GotKeyboardFocus event
        bool isGotKeyboardFocus;
    }
    [TargetType(typeof(Label))]
    public class LabelTargetButtonBVT : IUnitTest
    {
        public LabelTargetButtonBVT()
        {
        }

        /// <summary>
        /// Test press label access key to invoke button
        /// </summary>
        /// <param name="label"></param>
        /// <param name="variation"></param>
        public TestResult Perform(object obj, XmlElement variation)
        {
            Panel panel = (Panel)obj;
            Label label = panel.Children[1] as Label;
            Button button = panel.Children[0] as Button;
            label.Content = "_Type T to invoke button";
            label.Target = button;
            isClicked = false;
            button.Click += new RoutedEventHandler(OnClick);

            UserInput.KeyDown("LeftAlt");
            UserInput.KeyDown("T");
            UserInput.KeyUp("T");
            UserInput.KeyUp("LeftAlt");

            QueueHelper.WaitTillQueueItemsProcessed();

            if (!isClicked)
            {
                TestLog.Current.LogDebug("Fail: Button click event does not fire.");
                return TestResult.Fail;
            }

            return TestResult.Pass;
        }

        protected void OnClick(object sender, RoutedEventArgs e)
        {
            isClicked = true;
        }

        //necessary because of testing Click event
        bool isClicked;
    }
}



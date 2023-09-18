// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

//  Provides a base class for TextBox-related test cases.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/Text/BVT/TextBoxOM/TextBoxTestCase.cs $")]

namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.Collections;
    using System.ComponentModel;

    using System.ComponentModel.Design;
    using Drawing = System.Drawing;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using System.Windows.Input;

    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Microsoft.Test.Imaging;
    using Test.Uis.Loggers;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;

    #endregion Namespaces.

    /// <summary>
    /// Provides a base class for TextBox-related test cases.
    /// </summary>
    public abstract class TextBoxTestCase: CustomTestCase
    {
        #region Configuration settings.

        /// <summary>Text for control.</summary>
        /// <remarks>
        /// May not be required - subtypes should declare a TestArgument
        /// attribute if they use it.
        /// </remarks>
        public string Text
        {
            get { return Settings.GetArgument("Text"); }
        }

        #endregion Configuration settings.

        #region Public properties.

        /// <summary>
        /// Provides access to the control being tested.
        /// </summary>
        public UIElement TestControl
        {
            get
            {
                UIElement result =
                    ElementUtils.FindElement(MainWindow, "TestControl1");
                if (result == null)
                {
                    throw new InvalidOperationException(
                        "No control with Name TestControl1 was found.");
                }
                return result;
            }
        }

        /// <summary>
        /// Provides access to an alternate control being tested.
        /// </summary>
        public UIElement TestControlAlt
        {
            get
            {
                UIElement result =
                    ElementUtils.FindElement(MainWindow, "TestControl2");
                if (result == null)
                {
                    throw new InvalidOperationException(
                        "No control with Name TestControl2 was found.");
                }
                return result;
            }
        }

        /// <summary>
        /// Provides access to a TextBox being tested. Use TestControl
        /// and TestWrapper unless there is a need for the control to be
        /// specifically a TextBox.
        /// </summary>
        public TextBox TestTextBox
        {
            get
            {
                return (TextBox) TestControl;
            }
        }

        /// <summary>
        /// Provides access to an alternate TextBox being tested. Use
        /// TestControlAlt and TestWrapperAlt unless there is a
        /// need for the control to be a TextBox.
        /// </summary>
        public TextBox TestTextBoxAlt
        {
            get { return (TextBox) TestControlAlt; }
        }

        /// <summary>
        /// Provides access to a wrapper around the tested control.
        /// </summary>
        public UIElementWrapper TestWrapper
        {
            get
            {
                return new UIElementWrapper(TestControl);
            }
        }

        /// <summary>
        /// Provides access to a wrapper around an alternate tested control.
        /// </summary>
        public UIElementWrapper TestWrapperAlt
        {
            get
            {
                return new UIElementWrapper(TestControlAlt);
            }
        }

        /// <summary>
        /// Provides access to a top-level Panel.
        /// </summary>
        public Panel WinPanel
        {
            get
            {
                if (MainWindow.Content == null)
                {
                    throw new InvalidOperationException(
                        "Main window requires content to access a panel.");
                }
                Panel result = MainWindow.Content as Panel;
                if (result == null)
                {
                    throw new InvalidOperationException(
                        "MainWindow.Content [" + MainWindow.Content + "] " +
                        "cannot be cast to a Panel type.");
                }
                return result;
            }
        }

        #endregion Public properties.

        #region Public methods.

        /// <summary>Creates a new TextBoxTestCase instance.</summary>
        public TextBoxTestCase()
        {
            StartupPage = "TextBox.xaml";
        }

        /// <summary>
        /// Creates an exception for the case when an API accepts an
        /// invalid operation.
        /// </summary>
        public Exception AcceptedException(string description)
        {
            return new ApplicationException(
                "TextBox call accepted " + description);
        }

        /// <summary>
        /// Logs that an API rejected an operation, typically because it was
        /// expected to do so.
        /// </summary>
        public void LogRejected(string description)
        {
            Log("Control call rejects " + description);
        }

        /// <summary>
        /// Sets properties on the specified TextBox if they are specified
        /// in the configuration settings.
        /// </summary>
        /// <param name='textBox'>TextBox to set values on.</param>
        /// <remarks>
        /// The following properties are set: AcceptsReturn, AcceptsTab,
        /// FontSize, HorizontalScrollBarVisibility, IsNumberOnly, MaxLength,
        /// VerticalScrollBarVisibility, Wrap.
        /// </remarks>
        public void SetTextBoxProperties(TextBox textBox)
        {
            string s;
            if (textBox == null)
            {
                throw new ArgumentNullException("textBox");
            }
            if (Settings.HasArgument("AcceptsReturn", out s))
            {
                textBox.AcceptsReturn = bool.Parse(s);
            }
            if (Settings.HasArgument("AcceptsTab", out s))
            {
                textBox.AcceptsTab = bool.Parse(s);
            }
            if (Settings.HasArgument("FontSize", out s))
            {
                textBox.FontSize = Int32.Parse(s);
            }
            if (Settings.HasArgument("FontFamily", out s))
            {
                textBox.FontFamily = new FontFamily(s);
            }
            if (Settings.HasArgument("HorizontalScrollBarVisibility", out s))
            {
                textBox.HorizontalScrollBarVisibility = (ScrollBarVisibility)
                    Enum.Parse(typeof(ScrollBarVisibility), s);
            }
            if (Settings.HasArgument("MaxLength", out s))
            {
                textBox.MaxLength = int.Parse(s);
            }
            if (Settings.HasArgument("VerticalScrollBarVisibility", out s))
            {
                textBox.VerticalScrollBarVisibility = (ScrollBarVisibility)
                    Enum.Parse(typeof(ScrollBarVisibility), s);
            }
            if (Settings.HasArgument("Wrap", out s))
            {
                textBox.TextWrapping = bool.Parse(s) ? TextWrapping.Wrap : TextWrapping.NoWrap;
            }
        }

        /// <summary>
        /// Verifies that the text for the given text box is as expected.
        /// </summary>
        public void VerifyText(TextBox box, string text)
        {
            string msg = String.Format(
                "Text in text box [{0}] is as expected [{1}]", box.Text, text);
            Verifier.Verify(box.Text == text, msg, true);

            if (text == null) text = String.Empty;

            string omText = String.Empty;
            bool contentPending = false;

            TextPointer n = Test.Uis.Utils.TextUtils.GetTextBoxStart(box);
            //--TextPointer n = box.StartPosition.CreateNavigator();
            while (n.CompareTo(Test.Uis.Utils.TextUtils.GetTextBoxEnd(box)) < 0 && !contentPending)
            {
                if (n.GetPointerContext(LogicalDirection.Forward) == TextPointerContext.Text)
                {
                    omText += n.GetTextInRun(LogicalDirection.Forward);
                }
                contentPending = ( (n = n.GetNextContextPosition(LogicalDirection.Forward)) == null) ? false : true;
            }
            Log("Text as calculated by OM: [" + omText + "]");
            Verifier.Verify(omText == text,
                msg + " - as checked by StartPosition/EndPosition", true);
        }

        /// <summary>
        /// Verifies that the selected text for the given text box is
        /// as expected.
        /// </summary>
        public void VerifySelectedText(TextBox box, string text)
        {
            string msg = String.Format(
                "Selected text in text box [{0}] is as expected [{1}]",
                box.SelectedText, text);
            Verifier.Verify(box.SelectedText == text, msg, true);
        }

        #endregion Public methods.

    }
}

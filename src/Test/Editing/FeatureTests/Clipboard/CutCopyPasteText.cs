// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

/*************************************************
 *  This codes test cut-paste, copy-paste, backspace, delete
 *  1. Create Canvas then add TextBox with textSring
 *  2. mouse click infront of textString
 *  3. For cut/paste : call textBox.select(); verify text selection; do cut (ctrl+x); verify clipboard dataobject; do paste (ctrl+v); verify text pasted.
 *  4. For copy/paste: call textBox.select(); verify text selection; do copy(ctrl+c); verify clipboard dataobject; do paste (ctrl+v); verify text pasted.
 *  5. For backspace : hit right arrow; hit backspace; verify char delete
 *  6. For delete    : hit right arrow; hit delete; verify char delete
 *  Command Line: exe.exe /TestCaseType=CutCopyPasteText /Action=cut, copy, backspace, delete, Regression_Bug282
 *
 * ************************************************/
 
namespace DataTransfer
{
    using System;
    using System.Windows;
    using System.Windows.Controls;

    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Markup;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.Utils;
    using Test.Uis.TestTypes;
    using AvalonTools.FrameworkUtils;

    /// <summary>
    /// Verifies that text can be cut, copied and pasted.
    /// </summary>
    [TestArgument("Action", "Action to test; one of cut,copy,backspace,delete,Regression_Bug282")]
    public class CutCopyPasteText : CustomTestCase
    {
        private Canvas _canvas;          //use for root element
        private TextBox _textBox;        //editable textbox
        private string _textString = "AbC dEf";      //content of textbox
        Rect _rc;    //find rectangle of a control
        System.Windows.Point _p1;      //first point for mouse click action

        private string Arg
        {
            get { return ConfigurationSettings.Current.GetArgument ("Action", true); }
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase ()
        {
            _canvas = new Canvas ();
            _canvas.Width = new Length (500);
            _canvas.Height = new Length (200);
            _textBox = new TextBox ();
            _textBox.Text = _textString;
            this.MainWindow.Content = _canvas;
            _canvas.Children.Add(_textBox);
            _textBox.Focusable = true;
            this.MainWindow.Show ();
            QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (StartTest));
        }

        void StartTest ()
        {
            _rc = ElementUtils.GetScreenRelativeRect(_textBox);
            _p1 = new Point(_rc.Left + 17, _rc.Top + 14);
            MouseInput.MouseClick (_p1);
            QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (DoCopyCutPasteText));
        }

        void DoCopyCutPasteText ()
        {
            switch (Arg)
            {
                case "cut":
                    _textBox.Select (0, 3);
                    Verifier.Verify(_textBox.GetSelectedText(false, false) == "AbC", "Fail to select text.");
                    KeyboardInput.PressVirtualKey(Test.Uis.Wrappers.Win32.VK_CONTROL);
                    KeyboardInput.TypeString("^x");
                    KeyboardInput.ReleaseVirtualKey(Test.Uis.Wrappers.Win32.VK_CONTROL);
                    QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (VerifyResult));                    
                    break;
                case "copy":
                    _textBox.Select (4, 7);
                    Verifier.Verify(_textBox.GetSelectedText(false, false) == "dEf", "Fail to select text.");
                    KeyboardInput.PressVirtualKey(Test.Uis.Wrappers.Win32.VK_CONTROL);
                    KeyboardInput.TypeString("^c");
                    KeyboardInput.ReleaseVirtualKey(Test.Uis.Wrappers.Win32.VK_CONTROL);
                    QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (VerifyResult));
                    break;
                case "backspace":
                    KeyboardInput.PressVirtualKey (VK_RIGHT); //hit right arrow
                    KeyboardInput.PressVirtualKey (VK_BACK); //hit backspace
                    QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (VerifyResult));
                    break;
                case "delete":
                    KeyboardInput.PressVirtualKey (VK_RIGHT); //hit right arrow
                    KeyboardInput.PressVirtualKey (VK_DELETE); //hit Delete
                    QueueHelper.Current.QueueDelegate (new Test.Uis.Utils.SimpleHandler (VerifyResult));
                    break;
                case "Regression_Bug282": //Copy command is premanently disabled when Copy is raised on an empty TextSelection
                    KeyboardInput.TypeString("^c");
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyResult));
                    break;
                default:
                    Logger.Current.Log("No action.");
                    break;
            }
        }

        void DoPaste ()
        {
            RoutedCommand PasteCommand = CommandLibrary.Paste;
            PasteCommand.Execute(_textBox);
            Verifier.Verify(_textBox.Text == _textString, "TextBox conent should contain the textString.", true);
            Logger.Current.ReportSuccess();
        }

        void VerifyResult ()
        {
            DataObject dataObj = Clipboard.GetDataObject ();
            object data = dataObj.GetData (DataFormats.Text);
            switch (Arg)
            {
                case "cut":
                    Verifier.Verify((string)data == "AbC", "Cut should set data to clipboard.", true);
                    Verifier.Verify(_textBox.Text != _textString, "TextBox content should be smaller when do cut.", true);
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoPaste));
                    break;
                case"copy":
                    Verifier.Verify((string)data == "dEf", "Copy should set data to clipboard.", true);
                    Verifier.Verify(_textBox.Text == _textString, "TextBox content should be the same when do copy.", true);
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoPaste));
                    break;
                case"backspace":
                    Verifier.Verify(_textBox.Text != _textString, "TextBox content should be smaller when do backspace.", true);
                    Logger.Current.ReportSuccess();
                    break;
                case"delete":
                    Verifier.Verify(_textBox.Text != _textString, "TextBox content should be smaller when do delete.", true);
                    Logger.Current.ReportSuccess();
                    break;
                case"Regression_Bug282":
                    Verifier.Verify(data == null, "Copy empty selection should not set data to clipboard.", true);
                    Verifier.Verify(_textBox.Text == _textString, "TextBox should remain the same.", true);
                    _textBox.Select(3, 6);
                    KeyboardInput.TypeString("^c");
                    QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(VerifyResultforRegression_Bug282));
                    break;
                default:
                    Logger.Current.Log("No action!");
                    break;
            }
        }

        void VerifyResultforRegression_Bug282()
        {
            DataObject dataObj = Clipboard.GetDataObject();
            object data = dataObj.GetData(DataFormats.Text);
            Verifier.Verify((string)data == " dEf", "clipboard should contain \" dEf\"", true);
            QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(DoPaste));
        }
        private const int VK_DELETE = 0x2E;
        private const int VK_BACK = 0x08;
        private const int VK_RIGHT = 0x27;
    }
}
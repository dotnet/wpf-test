// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.
    
    using System;
    using System.ComponentModel;
    using System.Reflection;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>
    /// UndoTypeingTest
    /// </summary>
    [Test(0, "UndoRedo", "UndoTypingTest", MethodParameters="/TestCaseType=UndoTypingTest /Case=SimpleTyping")]
    [TestOwner("Microsoft"), TestTitle("UndoTypingTest"), TestTactics("363"), TestLastUpdatedOn("Jan 25, 2007")]
    public class UndoTypingTest : CommonTestCase
    {
        /// <summary>TypedString</summary>
        string _typedString = "abc def ghi jkl mno pqr stu vwx yz1 234 567 890";

        /// <summary>HotKey</summary>
        string _hotKey = "\t";

        /// <summary>HK</summary> 
        string _HK = "enter";

        /// <summary> AfterFirstUndo</summary>
        string _afterFirstUndo;

        /// <summary>AfterSecondUndo </summary>
        string _afterSecondUndo = "";

        /// <summary>AfterTyping </summary>
        string _afterTyping;

        /// <summary>KeyStringInXAML</summary>
        string _keyStringInXAML = "";

        TextBox _textBox;

        /// <summary>Ini </summary>
        public override void Init()
        {
            EnterFuction("Init");
            _textBox = new TextBox();
            _textBox.AcceptsReturn = true;
            _textBox.Width = 300;
            _textBox.Height = 300;
            MainWindow.Content = _textBox;
            EndFunction();
        }
        #region case - SimpleTyping
        /// <summary>SimpleTyping</summary>
        [TestCase(LocalCaseStatus.Ready, "Test case for SimpleTyping")]
        public void SimpleTyping()
        {
            EnterFuction("SimpleTyping_CaseStart");
            _textBox.Text = "";

            //set the focus in the textbox
            MouseInput.MouseClick(_textBox);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(BeginSimpleTyping));
            EndFunction();
        }

        /// <summary>BeginSimpleTyping</summary>
        void BeginSimpleTyping()
        {
            EnterFuction("BeginSimpleTyping");
            Test.Uis.Utils.KeyboardInput.TypeString(_typedString);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterSimpleTyping));
            EndFunction();
        }

        /// <summary>AfterSimpleTyping</summary>
        void AfterSimpleTyping()
        {
            EnterFuction("AfterSimpleTyping");
            Verifier.Verify(_typedString == _textBox.Text, CurrentFunction + " - Text typed in should be: " + _typedString);

            //perfrom Undo
            Test.Uis.Utils.KeyboardInput.TypeString("^z");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterUndoSimpleTyping));
            EndFunction();
        }

        /// <summary>AfterUndoSimpleTyping</summary>
        void AfterUndoSimpleTyping()
        {
            Verifier.Verify("" == _textBox.Text || _textBox.Text == null, "After Undo, no text should be in the textbox!!!");

            //perfrom Redo
            Test.Uis.Utils.KeyboardInput.TypeString("^y");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterRedoSympleTyping));
        }

        /// <summary>AfterRedoSympleTyping</summary>
        void AfterRedoSympleTyping()
        {
            Verifier.Verify(_typedString == _textBox.Text, "Text typed in should be: " + _typedString);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(EndTest);

        }
        #endregion 
        
        #region case - HotKey
        /// <summary>UndoHotKey</summary>
        [TestCase(LocalCaseStatus.Ready, "Test case for UndoHotKey")]
        public void UndoHotKey()
        {
            _typedString = "abc";

            int strLength = _typedString.Length;

            _textBox.Text = "";
            _textBox.AcceptsReturn = true;
            _textBox.AcceptsTab = true;
            GetHotKeyValue();

            //set the focus in the textbox
            MouseInput.MouseClick(_textBox);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(BeginTypeHotKey));
        }

        /// <summary>GetHotKeyValue</summary>
        void GetHotKeyValue()
        {
            EnterFuction("GetHotKeyValue");
            switch (_HK)
            {
                case "enter": _hotKey = "\n";
                    _afterTyping = _typedString + "\r\n";
                    _afterFirstUndo = _typedString;
                    _HK = "backspace";
                    break;

                case "backspace": _hotKey = "{Backspace}";
                    _afterTyping = _typedString.Substring(0, _typedString.Length - 1);
                    _afterFirstUndo = _typedString;
                    _HK = "tab";
                    break;

                case "tab": _hotKey = "\tb{backspace}";
                    _afterTyping = _typedString + "\t";
                    _afterFirstUndo = _afterTyping + "b";
                    _HK = "delete";
                    break;

                case "delete": _hotKey = "{left}{delete}";
                    _afterTyping = _typedString.Substring(0, _typedString.Length - 1);
                    _afterFirstUndo = _typedString;
                    _afterSecondUndo = "";
                    _HK = "ctrl-x";
                    break;

                case "ctrl-x": _hotKey = "+{LEFT}^x";
                    _afterTyping = _typedString.Substring(0, _typedString.Length - 1);
                    _afterFirstUndo = _typedString;
                    _HK = "ctrl-v";
                    break;

                case "ctrl-v": _hotKey = "^v";
                    Clipboard.SetDataObject("def");
                    _afterTyping = _typedString + "def";
                    _afterFirstUndo = _typedString;
                    _HK = "";
                    break;

                default:
                    Verifier.Verify(false, CurrentFunction + " - the sepcified hot key is not supported!!! HotKey=" + _HK);
                    break;
            }
            MyLogger.Log("Now, perfrom Undoredo for Hotkey - " + _HK);
            EndFunction();
        }

        /// <summary>BeginTypeHotKey</summary>
        void BeginTypeHotKey()
        {
            EnterFuction("BeginTypeHotKey");
            Test.Uis.Utils.KeyboardInput.TypeString(_typedString + _hotKey);
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterTypeHotKey));
            EndFunction();
        }

        /// <summary>AfterTypeHotKey</summary>
        void AfterTypeHotKey()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection(_textBox);

            EnterFuction("AfterTypeHotKey");
            Verifier.Verify(_textBox.Text == _afterTyping, CurrentFunction + " - Wrong string is typed into the textbox!!! Expect:" + _afterTyping + " Actual: " + _textBox.Text);
            string str = XamlUtils.TextRange_GetXml(textSelection);
            Verifier.Verify(XamlUtils.TextRange_GetXml(textSelection).Contains(_keyStringInXAML), CurrentFunction + " - xaml does not container " + _keyStringInXAML + "xaml: " + XamlUtils.TextRange_GetXml(textSelection));
          
            //first Undo
            Test.Uis.Utils.KeyboardInput.TypeString("^z");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterFirstUndoTypeHotkey));
            EndFunction();
        }

        /// <summary>AfterFirstUndoTypeHotkey</summary>
        void AfterFirstUndoTypeHotkey()
        {
            EnterFuction("AfterFirstUndoTypeHotkey");
            Verifier.Verify(_textBox.Text == _afterFirstUndo, CurrentFunction + " - Wrong string is in textbox after first Undo!!! Expect: " + _afterFirstUndo + " Acual:" +_textBox.Text);
           
            //Second Undo
            Test.Uis.Utils.KeyboardInput.TypeString("^z");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterSecondUndoTypeHotkey));
            EndFunction();
        }

        /// <summary>AfterSecondUndoTypeHotkey</summary>
        void AfterSecondUndoTypeHotkey()
        {
            EnterFuction("AfterSecondUndoTypeHotkey");
            Verifier.Verify(_textBox.Text == _afterSecondUndo || _textBox.Text == null, CurrentFunction + " - After the second Undo, No text should be in the textbox!!! Expect: " + _afterSecondUndo + " Actual: " + _textBox.Text);
            
            //first Redo
            Test.Uis.Utils.KeyboardInput.TypeString("^y");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterFirstRedoTypeHotkey));
            EndFunction();
        }

        /// <summary>AfterFirstRedoTypeHotkey</summary>
        void AfterFirstRedoTypeHotkey()
        {
            EnterFuction("AfterFirstRedoTypeHotkey");
            Verifier.Verify(_textBox.Text == _afterFirstUndo, CurrentFunction + " - Wrong string is in textbox after first Redo!!! Expect: " + _afterFirstUndo + " Actual: " + _textBox.Text);
          
            //Second Redo
            Test.Uis.Utils.KeyboardInput.TypeString("^y");
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(AfterSecondRedoTypeHotkey));
            EndFunction();
        }

        /// <summary>AfterSecondRedoTypeHotkey</summary>
        void AfterSecondRedoTypeHotkey()
        {
            TextSelection textSelection = Test.Uis.Utils.TextUtils.GetTextBoxSelection (_textBox);

            EnterFuction("AfterSecondRedoTypeHotkey");
            Verifier.Verify(_textBox.Text == _afterTyping, CurrentFunction + " - Wrong string is typed into the textbox!!! Expect: " + _afterTyping + " Actual: " + _textBox.Text);
            Verifier.Verify(XamlUtils.TextRange_GetXml(textSelection).Contains(_keyStringInXAML), CurrentFunction + " - xaml does not container " + _keyStringInXAML + "xaml: " + XamlUtils.TextRange_GetXml(textSelection));
            if (_HK != string.Empty)
            {
                Sleep();
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(UndoHotKey);
            }
            else
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(EndTest);

            EndFunction();
        }
        #endregion  
    }
}

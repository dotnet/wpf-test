// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.


namespace Test.Uis.TextEditing
{
    #region Namespaces.

    using System;
    using System.ComponentModel;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Documents;
    using System.Windows.Media;
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.Utils;
    using Microsoft.Test;
    using Microsoft.Test.Discovery;

    #endregion Namespaces.

    /// <summary>The test support globalization </summary>
    [Test(0, "UndoRedo", "UndoMultipleLanguagesTest1", MethodParameters = "/TestCaseType=UndoMultipleLanguagesTest /Case=UndoDeleteAndBackSpaceTest /data=!AD:index=0", Timeout=300)]
    [Test(2, "UndoRedo", "UndoMultipleLanguagesTest", MethodParameters = "/TestCaseType=UndoMultipleLanguagesTest /Case=UndoDeleteAndBackSpaceTest /data=!AD:index=0;length=10", Timeout=500)]
    [TestOwner("Microsoft"), TestTitle("UndoMultipleLanguagesTest"), TestTactics("364"), TestLastUpdatedOn("Jan 25, 2007")]
    public class UndoMultipleLanguagesTest : CommonTestCase
    {
        /// <summary>The data field need to be specified from command line </summary>
        string _myString = string.Empty;

        TextBox _textBox;

        int _actionCounter = 0;

        bool _doDelete = true;

        /// <summary>set up for test </summary>
        public override void Init()
        {
            EnterFuction("Init");
            base.Init();
            _textBox = new TextBox();
            MainWindow.Content = _textBox;
            _textBox.Width = 500;
            _textBox.Height = 30;
            _textBox.Background = Brushes.Wheat;
            _myString = ConfigurationSettings.Current.GetArgument("data");
            Verifier.Verify(!(_myString == null || _myString == string.Empty), CurrentFunction + " - Failed: failed to get string from AutoData!!!");
            _textBox.Text = _myString;

            QueueDelegate(MyEndFunction);
        }

        void MyEndFunction()
        {
            _textBox.Focus();
            EndFunction();
        }

        /// <summary> this test undo/redo form delete and backspace hotkey with globalization. </summary>
        public void UndoDeleteAndBackSpaceTest()
        {
            EnterFuction("UndoDeleteAndBackSpaceTest");
            KeyboardInput.TypeString("{HOME}");
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformDeleteAndBackSpace));
        }

        /// <summary>Perfrom Delete or BackSpace </summary>
        void PerformDeleteAndBackSpace()
        {
            EnterFuction("PerformDeleteAndBackSpace");

            if (_textBox.Text.Length > 0)
            {
                if (_doDelete)
                    KeyboardInput.TypeString("{delete}");
                else
                KeyboardInput.TypeString("{backspace}");

                _actionCounter++;
                System.Threading.Thread.Sleep(20);
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformDeleteAndBackSpace));
            }
            else
            {
                EndFunction();
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(FinishedDeleteAndBackSpace));
            }
        }

        /// <summary>verify the result </summary>
        void FinishedDeleteAndBackSpace()
        {
            EnterFuction("FinishedDeleteAndBackSpace");
            Verifier.Verify(_textBox.Text == string.Empty, CurrentFunction + " - Failed: after deleting no text should be in the textbox!!! Actual: " + _textBox.Text);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformUndoDeleteAndBackSpace));
        }

        /// <summary>Perfrom Undo </summary>
        void PerformUndoDeleteAndBackSpace()
        {
            EnterFuction("PerformUndoDeleteAndBackSpace");
            KeyboardInput.TypeString("^z");
            _actionCounter--;
            System.Threading.Thread.Sleep(50);
            if (_actionCounter > 0)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformUndoDeleteAndBackSpace));
            }
            else
            {
                EndFunction();
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(FinishedUndoDeleteAndBackSpace));
            }
        }

        /// <summary>Verify the result after Undo </summary>
        void FinishedUndoDeleteAndBackSpace()
        {
            EnterFuction("FinishedUndoDeleteAndBackSpace");
            Verifier.Verify(_textBox.Text == _myString, CurrentFunction + " - Failed: after undo, Text in Textbox is wrong!!! Expected: " + _myString + " Actual:" + _textBox.Text);
            EndFunction();
            Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformRedoDeleteAndBackSpace));
        }

        /// <summary>Perfrom Redo</summary>
        void PerformRedoDeleteAndBackSpace()
        {
            EnterFuction("PerformRedoDeleteAndBackSpace");
            KeyboardInput.TypeString("^y");
            _actionCounter++;
            System.Threading.Thread.Sleep(50);
            if (_myString.Length > _actionCounter)
            {
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformRedoDeleteAndBackSpace));
            }
            else
            {
                EndFunction();
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(FinishedRedoDeleteAndBackSpace));
            }
        }
        /// <summary>Log the result </summary>
        void FinishedRedoDeleteAndBackSpace()
        {
            EnterFuction("FinishedRedoDeleteAndBackSpace");
            Verifier.Verify(_textBox.Text == string.Empty, CurrentFunction + " - Failed: after perfroming Undo/Redo, The text changed in the textbox!!! exptect: " + _myString + " Actual: " + _textBox.Text);
            if (_doDelete)
            {  
                _doDelete = false;
                _textBox.Text = _myString;
                KeyboardInput.TypeString("{End}");
                _actionCounter = 0;
                //start to run backspace
                Test.Uis.Utils.QueueHelper.Current.QueueDelegate(new Test.Uis.Utils.SimpleHandler(PerformDeleteAndBackSpace));
            }
            else
                MyLogger.ReportSuccess();
            EndFunction();
        }
    }
}

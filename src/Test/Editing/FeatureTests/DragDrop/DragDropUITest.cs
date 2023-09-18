// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

[assembly: Test.Uis.Management.VersionInformation("$Author: Microsoft $ $Change: 3 $ $Source: //depot/winmain_oob/wap_rtm/windowstest/client/wcptests/uis/forms/BVT/DataTransfer/DragDrop/DragDropUITest.cs $")]

/*************************************************
 *
 *  PrevieweventDragDrop - Contains custome controls dragdrop. See TC 192 for sample command line
 *  DragDropInternationalText - include RightToLeft and MaxLength TextBox. See TC 194 195 196
 *  DragDropRegressionBugs - Case for fixed bugs (see TC 193 for example) and other BVT cases include dd to cross Avalon and IE
 * ************************************************/

namespace DataTransfer
{
    #region Namespaces

    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.ComponentModel.Design;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using System.Text;
    using System.Threading;
    
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Markup;
    using System.Windows.Media;    
    using System.Windows.Threading;

    using Microsoft.Test;
    using Microsoft.Test.Discovery;
    using Microsoft.Test.Imaging;

    using Test.Uis.Data;    
    using Test.Uis.Loggers;
    using Test.Uis.Management;
    using Test.Uis.TestTypes;
    using Test.Uis.TextEditing;
    using Test.Uis.Utils;
    using Test.Uis.Wrappers;
    using Microsoft.Test.Threading;

    #endregion Namespaces

    #region PrevieweventDragDrop
    /// <summary>
    /// Testing Preview dragdrop events
    /// Handlers can be set for instances and classes of objects
    /// Preview fires on all the ancestor chain elements and can be canceled
    /// </summary>
    // DISABLEDUNSTABLETEST:
    // TestName:PrevieweventDragDrop
    // Area: Editing SubArea: DragDrop
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: findstr /snip DISABLEDUNSTABLETEST 
    [Test(2, "DragDrop", "PrevieweventDragDrop", MethodParameters = "/TestCaseType=PrevieweventDragDrop /Log=Yes",Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("192")]
    public class PrevieweventDragDrop : CustomTestCase
    {
        private Mycanvas _root = null;
        private MytextBox1 _textBox1 = null;
        private MytextBox2 _textBox2 = null;
        private Rect _rc1;                       //find rectangle of a 1st textbox1
        private Rect _rc2;                       //find rectangle of a 2nd textbox2
        private System.Windows.Point _p1;      //first point for top_left corner of textbox1
        private System.Windows.Point _p2;      //third point for top_left corner of textbox2
        private bool _textBox1_instance_Drop;
        private bool _textBox1_instance_PreviewDrop;
        private bool _textBox1_instance_PreviewQueryContinueDrag;
        private bool _textBox1_instance_PreviewDragEnter;
        private bool _textBox1_instance_PreviewDragLeave;
        private bool _textBox1_instance_PreviewDragOver;
        private bool _textBox1_instance_PreviewGiveFeedback;
        private bool _textBox2_instance_Drop;
        private bool _textBox2_instance_PreviewDrop;
        private bool _textBox2_instance_PreviewQueryContinueDrag;
        private bool _textBox2_instance_PreviewDragEnter;
        private bool _textBox2_instance_PreviewDragLeave;
        private bool _textBox2_instance_PreviewDragOver;
        private bool _textBox2_instance_PreviewGiveFeedback;
        private bool _mycanvas_instance_PreviewDrop;
        private bool _mycanvas_instance_Drop;

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            //Size and position the windows
            this.MainWindow.Width = this.MainWindow.Height = 500;
            _root = new Mycanvas();
            _root.Height = _root.Width = 400;
            _root.Background = Brushes.Red;
            _textBox1 = new MytextBox1();
            _textBox2 = new MytextBox2();
            _textBox1.Height = _textBox2.Height = 100;
            _textBox1.Width = _textBox2.Width = 200;
            Canvas.SetTop(_textBox2, 110);
            this.MainWindow.Content = _root;
            _root.Children.Add(_textBox1);
            _root.Children.Add(_textBox2);
            this.MainWindow.Show();
            _textBox1.Text = "abc defghijkl mno";
            _textBox1.Select(4, 9);
            Verifier.Verify(_textBox1.SelectedText == "defghijkl", "textBox1.Selection matchs. " + _textBox1.SelectedText);
            QueueDelegate(StartTest);
        }

        private void StartTest()
        {
            _textBox1.Focus();
            //Find point on textBox
            _rc1 = ElementUtils.GetScreenRelativeRect(_textBox1);
            _p1 = new System.Windows.Point(_rc1.Left + 60, _rc1.Top + 14);
            _rc2 = ElementUtils.GetScreenRelativeRect(_textBox2);
            _p2 = new System.Windows.Point(_rc2.Left + 20, _rc2.Top + 54);

            #region textbox1_AddHandler
            //Enter, Leave, Over will call when mouse move over register drop target
            //QueryContinue, GiveFeedback will call during a mouse drag on the source element only
            //Drop will call only on Drop source element
            _textBox1.PreviewQueryContinueDrag += new QueryContinueDragEventHandler(textBox1_PreviewQueryContinueDrag); //Tunnel PreviewQueryContinueDrag event
            _textBox1.PreviewGiveFeedback += new GiveFeedbackEventHandler(textBox1_PreviewGiveFeedback);           //Tunnel PreviewGiveFeedback event
            _textBox1.PreviewDragEnter += new DragEventHandler(textBox1_PreviewDragEnter);                 //Tunnel PreviewDragEnter event
            _textBox1.PreviewDragLeave += new DragEventHandler(textBox1_PreviewDragLeave);                 //Tunnel PreviewDragLeave event
            _textBox1.PreviewDragOver += new DragEventHandler(textBox1_PreviewDragOver);                   //Tunnel PreviewDragOver event
            _textBox1.PreviewDrop += new DragEventHandler(textBox1_PreviewDrop);                           //Tunnel PreviewDrop event
            _textBox1.AddHandler(DragDrop.DropEvent, new DragEventHandler(textBox1_Drop), true);         //Bubble Drop event
            #endregion textbox1_AddHandler
            #region textbox2_AddHandler
            _textBox2.PreviewQueryContinueDrag += new QueryContinueDragEventHandler(textBox2_PreviewQueryContinueDrag); //Tunnel PreviewQueryContinueDrag event
            _textBox2.PreviewGiveFeedback += new GiveFeedbackEventHandler(textBox2_PreviewGiveFeedback);           //Tunnel PreviewGiveFeedback event
            _textBox2.PreviewDragEnter += new DragEventHandler(textBox2_PreviewDragEnter);                 //Tunnel PreviewDragEnter event
            _textBox2.PreviewDragLeave += new DragEventHandler(textBox2_PreviewDragLeave);                 //Tunnel PreviewDragLeave event
            _textBox2.PreviewDragOver += new DragEventHandler(textBox2_PreviewDragOver);                   //Tunnel PreviewDragOver event
            _textBox2.PreviewDrop += new DragEventHandler(textBox2_PreviewDrop);                           //Tunnel PreviewDrop event
            _textBox2.AddHandler(DragDrop.DropEvent, new DragEventHandler(textBox2_Drop), true);         //Bubble Drop event
            #endregion textbox2_AddHandler
            #region root_AddHandler
            _root.PreviewDrop += new DragEventHandler(MyrootPreviewDrop); //tunnel
            _root.AddHandler(DragDrop.DropEvent, new DragEventHandler(MyrootOnDragDrop), true); //bubble
            #endregion root_AddHandler
            InputMonitorManager.Current.IsEnabled = false;
            QueueDelegate(InjectKeys);
        }

        private void InjectKeys()
        {
            MouseInput.MouseDragInOtherThread(_p1, _p2, true, TimeSpan.Zero,
                VerifyResult, Dispatcher.CurrentDispatcher);
        }

        private void VerifyResult()
        {
            //Verify valid events fired and invalid event not to fire.
            Verifier.Verify(_textBox1_instance_PreviewQueryContinueDrag, "1textBox1_instance_PreviewQueryContinueDrag is fired.", true);
            Verifier.Verify(_textBox1_instance_PreviewGiveFeedback, "2textBox1_instance_PreviewGiveFeedback is fired.", true);
            Verifier.Verify(_textBox1_instance_PreviewDragEnter, "3textBox1_instance_PreviewDragEnter is fired.", true);
            Verifier.Verify(_textBox1_instance_PreviewDragLeave, "4textBox1_instance_PreviewDragLeave is fired.", true);
            Verifier.Verify(_textBox1_instance_PreviewDragOver, "5textBox1_instance_PreviewDragOver is fired.", true);
            Verifier.Verify(!_textBox1_instance_Drop, "6textBox1_instance_Drop should not fire." + _textBox1_instance_Drop, true);
            Verifier.Verify(!_textBox1_instance_PreviewDrop, "7textBox1_instance_PreviewDrop should not fire." + _textBox1_instance_PreviewDrop, true);
            Verifier.Verify(!_textBox2_instance_PreviewQueryContinueDrag, "8textBox2_instance_PreviewQueryContinueDrag should not fire." + _textBox2_instance_PreviewQueryContinueDrag, true);
            Verifier.Verify(!_textBox2_instance_PreviewGiveFeedback, "9textBox2_instance_PreviewGiveFeedback should not fire." + _textBox2_instance_PreviewGiveFeedback, true);
            Verifier.Verify(_textBox2_instance_PreviewDragEnter, "10textBox2_instance_PreviewDragEnter is fired.", true);
            Verifier.Verify(_textBox2_instance_PreviewDragLeave, "11textBox2_instance_PreviewDragLeave is fired.", true);
            Verifier.Verify(_textBox2_instance_PreviewDragOver, "12textBox2_instance_PreviewDragOver is fired.", true);
            Verifier.Verify(_textBox2_instance_Drop, "13textBox2_instance_Drop should not fire.", true);
            Verifier.Verify(_textBox2_instance_PreviewDrop, "14textBox2_instance_PreviewDrop should not fire.", true);
            Verifier.Verify(_mycanvas_instance_PreviewDrop, "15mycanvas_instance_PreviewDrop is fired.", true);
            Verifier.Verify(_mycanvas_instance_Drop, "16mycanvas_instance_Drop is fired.", true);
            Verifier.Verify(_textBox1.mytextBox1_class_PreviewQueryContinueDrag, "17mytextBox1_class_PreviewQueryContinueDrag is fired.", true);
            Verifier.Verify(_textBox1.mytextBox1_class_PreviewGiveFeedback, "18mytextBox1_class_PreviewGiveFeedback is fired.", true);
            Verifier.Verify(_textBox1.mytextBox1_class_PreviewDragEnter, "19mytextBox1_class_PreviewDragEnter is fired.", true);
            Verifier.Verify(_textBox1.mytextBox1_class_PreviewDragLeave, "20mytextBox1_class_PreviewDragLeave is fired.", true);
            Verifier.Verify(_textBox1.mytextBox1_class_PreviewDragOver, "21mytextBox1_class_PreviewDragOver is fired.", true);
            Verifier.Verify(!_textBox1.mytextBox1_class_Drop, "22mytextBox1_class_Drop should not fire on source element." + _textBox1.mytextBox1_class_Drop, true);
            Verifier.Verify(!_textBox1.mytextBox1_class_PreviewDrop, "23mytextBox1_class_PreviewDrop should not fire." + _textBox1.mytextBox1_class_PreviewDrop, true);
            Verifier.Verify(!_textBox2.mytextBox2_class_PreviewQueryContinueDrag, "24mytextBox2_class_PreviewQueryContinueDrag should not fire." + _textBox2.mytextBox2_class_PreviewQueryContinueDrag, true);
            Verifier.Verify(!_textBox2.mytextBox2_class_PreviewGiveFeedback, "25mytextBox2_class_PreviewGiveFeedback should not fire." + _textBox2.mytextBox2_class_PreviewGiveFeedback, true);
            Verifier.Verify(_textBox2.mytextBox2_class_PreviewDragEnter, "26mytextBox2_class_PreviewDragEnter is fired.", true);
            Verifier.Verify(_textBox2.mytextBox2_class_PreviewDragLeave, "27mytextBox2_class_PreviewDragLeave is fired.", true);
            Verifier.Verify(_textBox2.mytextBox2_class_PreviewDragOver, "28mytextBox2_class_PreviewDragOver is fired.", true);
            //Regression_Bug342 fix so that the protected virtual method does get called. So OnDrop should be called when text is dropped in textBox.
            Verifier.Verify(_textBox2.mytextBox2_class_Drop, "29mytextBox2_class_Drop is fired.", true);
            Verifier.Verify(_textBox2.mytextBox2_class_PreviewDrop, "30mytextBox2_class_PreviewDrop is fired.", true);
            Verifier.Verify(_root.mycanvas_class_OnPreviewDrop, "31mycanvas_class_OnPreviewDrop is fired.", true);
            Verifier.Verify(_root.mycanvas_class_OnDragDrop, "32mycanvas_class_OnDragDrop should not fire. It's fire in TextEditor" + _root.mycanvas_class_OnDragDrop, true);
            if (ConfigurationSettings.Current.GetArgument("Log") == "Yes")
            {
                Logger.Current.ReportSuccess();
            }
        }

        #region textbox1
        private void textBox1_Drop(object sender, DragEventArgs e)
        {
            _textBox1_instance_Drop = true;
        }
        private void textBox1_PreviewDrop(object sender, DragEventArgs e)
        {
            _textBox1_instance_PreviewDrop = true;
        }
        private void textBox1_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            _textBox1_instance_PreviewQueryContinueDrag = true;
        }
        private void textBox1_PreviewDragEnter(object sender, DragEventArgs e)
        {
            _textBox1_instance_PreviewDragEnter = true;
        }
        private void textBox1_PreviewDragLeave(object sender, DragEventArgs e)
        {
            _textBox1_instance_PreviewDragLeave = true;
        }
        private void textBox1_PreviewDragOver(object sender, DragEventArgs e)
        {
            _textBox1_instance_PreviewDragOver = true;
        }
        private void textBox1_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            _textBox1_instance_PreviewGiveFeedback = true;
        }
        #endregion textbox1

        #region textbox2
        private void textBox2_Drop(object sender, DragEventArgs e)
        {
            _textBox2_instance_Drop = true;
        }
        private void textBox2_PreviewDrop(object sender, DragEventArgs e)
        {
            _textBox2_instance_PreviewDrop = true;
        }
        private void textBox2_PreviewQueryContinueDrag(object sender, QueryContinueDragEventArgs e)
        {
            _textBox2_instance_PreviewQueryContinueDrag = true;
        }
        private void textBox2_PreviewDragEnter(object sender, DragEventArgs e)
        {
            _textBox2_instance_PreviewDragEnter = true;
        }
        private void textBox2_PreviewDragLeave(object sender, DragEventArgs e)
        {
            _textBox2_instance_PreviewDragLeave = true;
        }
        private void textBox2_PreviewDragOver(object sender, DragEventArgs e)
        {
            _textBox2_instance_PreviewDragOver = true;
        }
        private void textBox2_PreviewGiveFeedback(object sender, GiveFeedbackEventArgs e)
        {
            _textBox2_instance_PreviewGiveFeedback = true;
        }
        #endregion textbox2

        #region root
        private void MyrootPreviewDrop(object sender, DragEventArgs e)
        {
            _mycanvas_instance_PreviewDrop = true;
        }
        private void MyrootOnDragDrop(object sender, DragEventArgs e)
        {
            _mycanvas_instance_Drop = true;
            QueueHelper.Current.QueueDelegate(new SimpleHandler(VerifyResult));
        }
        #endregion root
    }

    /// <summary>
    /// Creating my textbox and override dragdrop class events
    /// </summary>
    public class MytextBox1 : System.Windows.Controls.TextBox
    {
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewDrop;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewQueryContinueDrag;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewDragEnter;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewDragLeave;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewGiveFeedback;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_PreviewDragOver;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox1_class_Drop;

        /// <summary> override class event</summary>
        protected override void OnPreviewDrop(DragEventArgs e)
        {
            mytextBox1_class_PreviewDrop = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            mytextBox1_class_PreviewQueryContinueDrag = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            mytextBox1_class_PreviewDragEnter = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            mytextBox1_class_PreviewDragLeave = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewGiveFeedback(GiveFeedbackEventArgs e)
        {
            mytextBox1_class_PreviewGiveFeedback = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            mytextBox1_class_PreviewDragOver = true;
        }
        /// <summary> override class event</summary>
        protected override void OnDrop(DragEventArgs e)
        {
            mytextBox1_class_Drop = true; //Doesn't call here due to ondrop only handle on target element and in this case cases Bubble event is handler in TextEditor
        }
    }

    /// <summary>
    /// Creating my textbox and override dragdrop class events
    /// </summary>
    public class MytextBox2 : System.Windows.Controls.TextBox
    {
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewDrop;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewQueryContinueDrag;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewDragEnter;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewDragLeave;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewGiveFeedback;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_PreviewDragOver;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mytextBox2_class_Drop;
        /// <summary> override class event</summary>
        protected override void OnPreviewDrop(DragEventArgs e)
        {
            mytextBox2_class_PreviewDrop = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewQueryContinueDrag(QueryContinueDragEventArgs e)
        {
            mytextBox2_class_PreviewQueryContinueDrag = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragEnter(DragEventArgs e)
        {
            mytextBox2_class_PreviewDragEnter = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragLeave(DragEventArgs e)
        {
            mytextBox2_class_PreviewDragLeave = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewGiveFeedback(GiveFeedbackEventArgs e)
        {
            mytextBox2_class_PreviewGiveFeedback = true;
        }
        /// <summary> override class event</summary>
        protected override void OnPreviewDragOver(DragEventArgs e)
        {
            mytextBox2_class_PreviewDragOver = true;
        }
        /// <summary> override class event</summary>
        protected override void OnDrop(DragEventArgs e)
        {
            mytextBox2_class_Drop = true; //Get call in TextEditor instead of here
        }
    }

    /// <summary>
    /// Creating my canvas and override dragdrop class events
    /// </summary>
    public class Mycanvas : Canvas
    {
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mycanvas_class_OnPreviewDrop;
        /// <summary> initialize public bool for each dragdrop event to notify if they are called </summary>
        public bool mycanvas_class_OnDragDrop;
        /// <summary> override class event</summary>
        protected override void OnPreviewDrop(DragEventArgs e)
        {
            mycanvas_class_OnPreviewDrop = true;
        }
        /// <summary> override class event</summary>
        protected override void OnDrop(DragEventArgs e)
        {
            mycanvas_class_OnDragDrop = true; //get call in TextEditor instead of here
        }
    }
    #endregion PrevieweventDragDrop

    #region DragDropInternationalText
    /// <summary>
    /// Verify that DragDrop international scrip and dragdrop in RlTbTextBox works
    /// </summary>
    [Test(2, "DragDrop", "DragDropInternationalText1", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=InternationalScript",Disabled=true)]
    [Test(0, "DragDrop", "DragDropInternationalText2", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=AllowDropFalseTextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropInternationalText3", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=RlTbTextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropInternationalText4", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=DragDropInMaxLengthTextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropInternationalText5", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=DragOutOfReadyOnlyTextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropInternationalText6", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=DropIntoReadyOnlyTextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropInternationalText7", MethodParameters = "/TestCaseType=DragDropInternationalText /TextType=DragDropInMaxLengthTextBoxRegression_Bug343",Disabled=true)]
    [TestOwner("Microsoft"), TestTactics("194,197,198,199,196,200,201,202")]
    public class DragDropInternationalText : Test.Uis.TextEditing.TextBoxTestCase
    {
        private TextScript[] _scripts;
        private Rect _rc1;                     //find rectangle of a 1st textbox1
        private Rect _rc2;                     //find rectangle of a 2nd textbox2
        private System.Windows.Point _p1;      //first point for top_left corner of textbox1
        private System.Windows.Point _p2;      //third point for top_left corner of textbox2
        private string _expectString;//get this value to compare with end result

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            _scripts = TextScript.Values;
            TestTextBox.AcceptsReturn = true;
            TestTextBoxAlt.AcceptsReturn = true;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            MouseInput.MouseClick(TestTextBox); //mouse click in text to set focus
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            //Find point on textBox
            _rc1 = ElementUtils.GetScreenRelativeRect(TestTextBox);
            _p1 = new System.Windows.Point(_rc1.Left + 27, _rc1.Top + 14);
            _rc2 = ElementUtils.GetScreenRelativeRect(TestTextBoxAlt);
            _p2 = new System.Windows.Point(_rc2.Left + 20, _rc2.Top + 14);
            switch (ConfigurationSettings.Current.GetArgument("TextType"))
            {
                case "InternationalScript": //TC: 194
                    //Grabing all predefined internaltion scipt and output into left TextBox
                    for (int i = 0; i < _scripts.Length; i++)
                    {
                        TestTextBox.AppendText(_scripts[i].Sample);
                        Log("Script for line" + i + " is: " + _scripts[i].Name);
                        TestTextBox.AppendText("\n");
                    }
                    break;
                case "RlTbTextBox": //TC:196
                    TestTextBox.FlowDirection = FlowDirection.RightToLeft;
                    TestTextBoxAlt.FlowDirection = FlowDirection.RightToLeft;
                    TestTextBox.AppendText("abc def ghi jkl mno pqr stu vwz yz\n123 456\nxyz");
                    break;
                case "DragDropInMaxLengthTextBox": //TC:195
                    Log("TC:200 Regression_Bug344");
                    TestTextBox.AppendText("abc def");
                    TestTextBoxAlt.MaxLength = 3;
                    break;
                case "DragDropInMaxLengthTextBoxRegression_Bug343": //TC:201
                    TestTextBox.AppendText("abc def ghi");
                    TestTextBox.MaxLength = 11;
                    break;
                case "DragOutOfReadyOnlyTextBox": //Regression_Bug427
                    TestTextBox.AppendText("abc def");
                    TestTextBox.IsReadOnly = true;
                    break;
                case "DropIntoReadyOnlyTextBox": //Regression_Bug427
                    TestTextBox.AppendText("abc def");
                    TestTextBoxAlt.IsReadOnly = true;
                    break;
                case "AllowDropFalseTextBox":
                    TestTextBox.AppendText("abc def");
                    TestTextBoxAlt.AllowDrop = false;
                    break;
            }
            _expectString = TestTextBox.Text; //Get textBox string as expected string
            QueueDelegate(MakeSelection);
        }
        private void MakeSelection()
        {
            TestTextBox.Focus();
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            TestTextBox.SelectAll();
            QueueDelegate(DoDragDrop);
        }

        private void DoDragDrop()
        {
            Verifier.Verify(TestTextBox.SelectedText == _expectString, "Expect: [" + _expectString + "]\nActual. [" + TestTextBox.SelectedText + "]");
            if (ConfigurationSettings.Current.GetArgument("TextType") == "DragDropInMaxLengthTextBoxRegression_Bug343")
                TestTextBox.Select(2, 5);
            InputMonitorManager.Current.IsEnabled = false;
            QueueDelegate(InjectKeys);
        }

        private void InjectKeys()
        {
            if (ConfigurationSettings.Current.GetArgument("TextType") == "DragDropInMaxLengthTextBoxRegression_Bug343")
            {
                _p2.X = _p1.X + 80;
            }
            MouseInput.MouseDragInOtherThread(_p1, _p2, true, TimeSpan.Zero,
                VerifyResult, Dispatcher.CurrentDispatcher);
        }

        private void VerifyResult()
        {
            if (ConfigurationSettings.Current.GetArgument("TextType") == "DragDropInMaxLengthTextBox")
            {
                Log("Expect [abc]");
                Log("Actual [" + TestTextBoxAlt.Text + "]");
                Verifier.Verify(TestTextBoxAlt.SelectedText == "abc", "Dropped text matched.", true);
                Logger.Current.ReportSuccess();
            }
            else if (ConfigurationSettings.Current.GetArgument("TextType") == "DragOutOfReadyOnlyTextBox")
            {
                Verifier.Verify(TestTextBox.Text == "abc def", "Text remains in ReadOnly TextBox.", true);
                Verifier.Verify(TestTextBoxAlt.SelectedText == "abc def", "Dropped text matched.", true);
                Logger.Current.ReportSuccess();
            }
            else if (ConfigurationSettings.Current.GetArgument("TextType") == "DropIntoReadyOnlyTextBox" ||
                ConfigurationSettings.Current.GetArgument("TextType") == "AllowDropFalseTextBox")
            {
                Verifier.Verify(TestTextBox.SelectedText == "abc def", "Text remains selected.", true);
                Verifier.Verify(TestTextBoxAlt.Text == "", "Should not allow to drop into ReadyOnly TextBox.", true);
                Logger.Current.ReportSuccess();
            }
            else if (ConfigurationSettings.Current.GetArgument("TextType") == "DragDropInMaxLengthTextBoxRegression_Bug343")
            {
                Verifier.Verify(TestTextBox.SelectedText == "c def", "Expect selection [c def]\nActual [" + TestTextBox.SelectedText + "]");
                Verifier.Verify(TestTextBox.Text == "ab ghic def", "Expect text [ab ghic def]\nActual [" + TestTextBox.Text + "]");
                // do ReadOnly=true left arrow, delete, backspace, undo, redo, type uppercase, lowercase, symbold space, tab, insert, shift type
                TestTextBox.IsReadOnly = true;
                TestTextBox.AcceptsTab = true;
                KeyboardInput.TypeString("+{Left}+{Left}+{Left}+{Left}+{Left}{Delete}{Backspace}^z^yAa@ {Tab}{Insert}+x");
                QueueDelegate(VerifyReadOnly);
            }
            else
            {
                Verifier.Verify(TestTextBoxAlt.SelectedText == _expectString, "Drop text matched and selected.", true);
                Logger.Current.ReportSuccess();
            }
        }
        private void VerifyReadOnly()
        {
            Verifier.Verify(TestTextBox.SelectedText == "", "Selection should be destroyed by left arrow input.", true);
            Verifier.Verify(TestTextBox.Text == "ab ghic def", "Text remains the same in ready only TextBox.", true);
            Logger.Current.ReportSuccess();
        }
    }
    #endregion DragDropInternationalText

    #region DragDropRegressionBugs

    /// <summary>
    /// DragDrop regression bugs.
    /// </summary>
    [Test(2, "DragDrop", "DragDropRegressionBugs1", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:23",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs2", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:33 /EditableBox1:TextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs3", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:39 /EditableBox1:TextBox",Disabled=true)]
    [Test(0, "DragDrop", "DragDropRegressionBugs4", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:40 /EditableBox1:TextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs5", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:41 /EditableBox1:TextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs6", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:43 /DragAgain=True /EditableBox1=TextBox /EditableBox2=TextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs7", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:44 /EditableBox1=TextBox",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs8", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:0",Disabled=true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs9", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:1", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs10", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:2", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs11", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:3", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs12", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:4", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs13", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:5", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs14", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:6", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs15", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:7", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs16", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:8", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs17", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:9", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs18", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:10", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs19", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:11", Disabled=true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs48", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:12", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs49", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:13", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs50", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:15", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs51", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:16", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs52", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:17", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs53", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:18", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs54", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:19", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs20", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:20", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs55", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:21", Disabled = true)]
    [Test(3, "DragDrop", "DragDropRegressionBugs56", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:22", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs21", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:24", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs23", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:25", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs22", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:26", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs24", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:27", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs25", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:28", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs34", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:28 /EditableBox1:TextBox /EditableBox2:TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs35", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:31 /EditableBox1:TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs26", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:32", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs36", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:34 /EditableBox1:TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs27", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:35", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs28", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:36", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs29", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:37", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs30", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:38", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs31", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:39", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs32", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:40", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs33", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:41", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs37", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:42", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs38", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:45 /EditableBox1=TextBox /EditableBox2=TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs39", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:46 /EditableBox1=TextBox /EditableBox2=TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs40", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:47 /EditableBox1=TextBox", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs41", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:48", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs42", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:49", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs43", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:50", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs44", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:51", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs45", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:52", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs46", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:53", Disabled = true)]
    [Test(2, "DragDrop", "DragDropRegressionBugs47", MethodParameters = "/TestCaseType:DragDropRegressionBugs /ContainerType1:Canvas /CaseIndex:54", Disabled = true)]

    [TestOwner("Microsoft"), TestBugs("469,470"), TestLastUpdatedOn("Aug 15, 2006"), 
    TestTactics("193,203,204,205,206,207,208,209,210,211,212,213,214,215,216,217,218,219,220,221,222,223,224,225,226,227,228,229,230,231,232,233,234,235,236,237,238,239,240,241,242,243,244,245,246,247,248")]
    public class DragDropRegressionBugs : TestContainer
    {
        private UIElementWrapper _editBox1;
        private UIElementWrapper _editBox2;
        /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
        public TestCaseData testCaseData;
        private string _originalText;                // Original text in editbox1
        private string _originalSelectedText;        // Original selected text in editbox1
        private string _originalSelectedXamlText;    // Original selected text in Xaml format
        private string _dropptedSelectedXamlText;    // Droppted text is selected in Xaml format
        /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
        public int testCaseIndex;

        private Rect _rc1;                 //find rectangle of a 1st textbox1
        private Rect _rc2;                 //find rectangle of a 2nd textbox2
        private System.Windows.Point _p1;  //first point
        private System.Windows.Point _p2;  //third point
        private System.Diagnostics.Process _process;

        private bool _queryContinueDragEvent = false;    // Flag for drag/drop event.
        private bool _dragLeaveEvent = false;
        private bool _giveFeedbackEvent = false;
        private bool _dropEvent = false;
        private bool _dragEnterEvent = false;
        private bool _dragOverEvent = false;

        /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
        public enum TestCaseAction
        {
            /// <summary>Drag and Drop action</summary>
            DragAndDrop,
            /// <summary>Drag and Drop Copy action</summary>
            DragAndDropCopy,
            /// <summary>Drag and Drop Cancel action</summary>
            DragAndDropCancel,
            /// <summary>Drag and Drop on itself action</summary>
            DragAndDropOnSelf,
        }

        /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
        public enum TestCaseContainer
        {
            /// <summary>Drag and Drop in same container</summary>
            SameContainer,
            /// <summary>Drag and Drop in cross container</summary>
            CrossContainer,
            /// <summary>Drag and Drop in cross app</summary>
            CrossApp,
        }

        private static string LongTextString()
        {
            string longText = "\nWhat 2034 will bring: If I keep up my exercise schedule" +
                ", I stand a good chance of experiencing computers 30 years from now" +
                ". According to Moore's Law, computer power doubles every 18 months" +
                ", meaning that computers will be a million times more powerful by 2034" +
                ". According to Nielsen's Law of Internet bandwidth, connectivity to the " +
                "home grows by 50 percent per year; by 2034, we'll have 200,000 times more " +
                "bandwidth. That same year, I'll own a computer that runs at 3PHz CPU speed" +
                ", has a petabyte (a thousand terabytes) of memory, half an exabyte (a billion" +
                " gigabytes) of hard disk-equivalent storage and connects to the Internet with " +
                "a bandwidth of a quarter terabit (a trillion binary digits) per second.\nBy 2034" +
                ", we'll finally get decent computer displays, with a resolution of about 20,000 by 10,000 pixels.\n";
            StringBuilder sb = new StringBuilder(longText.Length);
            for (int i = 0; i < 6; i++)
            {
                sb.Append(longText);
            }
            return sb.ToString();
        }

        /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
        public struct TestCaseData
        {
            /// <summary>TestAction</summary>
            public TestCaseAction TestAction;
            /// <summary>TestContainer</summary>
            public TestCaseContainer TestContainer;
            /// <summary>TestString</summary>
            public string TestString;
            /// <summary>StartSelection</summary>
            public int StartSelection;
            /// <summary>EndSelection</summary>
            public int EndSelection;
            /// <summary>DragLocationX</summary>
            public int DragLocationX;
            /// <summary>DragLocationY</summary>
            public int DragLocationY;
            /// <summary>DropLocationX</summary>
            public int DropLocationX;
            /// <summary>DropLocationY</summary>
            public int DropLocationY;
            /// <summary>ExpectString</summary>
            public string ExpectString;
            /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
            public TestCaseData(TestCaseAction testAction, TestCaseContainer testContainer, string testString, int startSelection,
                                int endSelection, int dragLocationX, int dragLocationY, int dropLocationX, int dropLocationY, string expectString)
            {
                this.TestAction = testAction;
                this.TestContainer = testContainer;
                this.TestString = testString;
                this.StartSelection = startSelection;
                this.EndSelection = endSelection;
                this.DragLocationX = dragLocationX;
                this.DragLocationY = dragLocationY;
                this.DropLocationX = dropLocationX;
                this.DropLocationY = dropLocationY;
                this.ExpectString = expectString;
            }
            /// <summary>it is used to verify in dragdrop second app (DragDropRegressionBugsForSecondApp)</summary>
            public static TestCaseData[] Cases = new TestCaseData[] {
                // 0: [TC 193]: DragDrop bold text into Normal text [Regression_Bug33] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Bold>abc 123 456 xxx</Bold> yyy def www", 0, 17, 105, 14, 175, 14, ""),

                // 1: [TC 203]: DragDrop normal text into Bold text [Regression_Bug33] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc 123 456xxxyyy<Bold>www</Bold>", 0, 13, 75, 14, 220, 14, ""),

                // 2: [TC 204]: DragDrop part of text in Bullet List to middle of Bullet List [Regression_Bug34] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 6, 6, 110, 14, 130, 0, ""),

                // 3: [TC 205]: DragDrop part of text in Bullet List to begining of Bullet List [Regression_Bug34] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 6, 6, 115, 14, -70, 0, ""),

                // 4: [TC 206]: DragDrop part of text in Bullet List to end of Bullet List [Regression_Bug34] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 6, 6, 95, 14, 150, 0, ""),

                // 5: [TC 207]: DragDrop part of text in Number List to middle of Number List [Regression_Bug34] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List MarkerStyle=\"Decimal\"><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 5, 6, 125, 14, 70, 0, ""),

                // 6: [TC 208]: DragDrop part of text in Number List to begining of Number List [Regression_Bug34] 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List MarkerStyle=\"Decimal\"><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 6, 6, 125, 14, -70, 0, ""),

                // 7: [TC 209]: DragDrop part of text in Number List to end of Number List [Regression_Bug34]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<List><ListItem><Paragraph>abc xxxyyywww</Paragraph></ListItem></List>", 6, 6, 125, 14, 170, 0, ""),

                // 8: [TC 210]: DragDrop to begining of Paragraph [Regression_Bug35]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph>abc xxxyyywww</Paragraph>", 6, 8, 95, 14, -60, 0, ""),

                // 9: [TC 211]: DragDrop to end of Paragraph [Regression_Bug36]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph>abc xxxyyywww</Paragraph>", 6, 6, 95, 14, 185, 0, ""),

                // 10: [TC 212]: DragDrop RichText into second RichTextBox [Regression_Bug37]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "<Run FontWeight='Bold'>xxx</Run><Run FontWeight='Bold' TextDecorations='Underline'>yyy</Run>", 0, 10, 20, 10, 10, 10, ""),

                // 11: [TC 213]: DragDrop Text and Image into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <Image Source='" + System.IO.Path.Combine(System.IO.Path.GetPathRoot(System.Environment.SystemDirectory), "work") + "/w.gif' Height='60' Width='40' /> yyy", 3, 9, 50, 40, 10, 10, ""),                    

                // 12: [TC 258]: DragDrop Text and CheckBox into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <CheckBox>CheckBox</CheckBox> yyy", 3, 9, 35, 15, 10, 10, ""),

                // 13: [TC 259]: DragDrop Text and RadioButton into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <RadioButton IsChecked=\"True\">RadioButton</RadioButton> yyy", 3, 9, 35, 15, 10, 10, ""),

                // 14: [TC 260]: DragDrop Text and ListBox into RichTextBox2 - wait for Regression_Bug38 to be fixed
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <ListBox><ListBoxItem>Item1</ListBoxItem><ListBoxItem>Item2</ListBoxItem></ListBox> yyy", 3, 9, 30, 35, 10, 10, ""),

                // 15: [TC 261]: DragDrop Text and Hyperlink into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <Hyperlink>Hyperlink</Hyperlink> yyy", 3, 9, 35, 15, 10, 10, ""),

                // 16: [TC 262]: DragDrop Text and Table into RichTextBox2 - Regression_Bug39
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "<Paragraph>abcxxx</Paragraph><Table><Table.Columns><TableColumn Background=\"Red\" /></Table.Columns>"+
                    "<TableRowGroup><TableRow><TableCell><Paragraph>Table</Paragraph></TableCell></TableRow></TableRowGroup></Table><Paragraph> 123 456 789 yyy</Paragraph>",
                    3, 19, 35, 15, 10, 10, ""),

                // 17: [TC 263]: DragDrop Text and ComboBox into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <ComboBox><ComboBoxItem>Item1</ComboBoxItem></ComboBox> yyy",
                    3, 9, 30, 15, 10, 10, ""),

                // 18: [TC 264]: DragDrop Text and Ellipse into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <Ellipse Fill=\"Yellow\" Width=\"49px\" Height=\"49px\" /> yyy", 3, 9, 30, 50, 10, 10, ""),

                // 19: [TC 265]: DragDrop Text and Button into RichTextBox2
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx <Button>Button</Button> yyy", 3, 9, 30, 15, 10, 10, ""),

                // 20: [TC 214]: DragDrop text and all controls into RichTextBox2
                // Image, CheckBox, RadioButton, HyperLink, ComboBox, Button...
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abcxxx 1<CheckBox>CheckBox</CheckBox>2"+
                    "<RadioButton IsChecked='True'>RadioButton</RadioButton>3<Hyperlink>Hyperlink</Hyperlink>4"+
                    "<ComboBox><ComboBoxItem>Item1</ComboBoxItem></ComboBox>5"+
                    "6<Button>Button</Button>7 yyy<RadioButton>1</RadioButton><RadioButton>2</RadioButton>"+
                    "<ScrollViewer Width='50' Height='50'><Border Background='lightblue'><StackPanel Width='80' Height='80'><Border Background='yellow'><StackPanel Width='40' Height='40' /></Border></StackPanel></Border></ScrollViewer>"+
                    "<Slider Minimum='10' Maximum='200' SmallChange='2' LargeChange='10'/>"+
                    "<Slider Orientation='Vertical' Minimum='10' Maximum='200' SmallChange='2' LargeChange='10' Value='20'/>"+
                    "<Label>_Press P</Label>"+
                    "<ListBox><ListBoxItem>1</ListBoxItem><ListBoxItem>2</ListBoxItem></ListBox>"+
                    "<Menu><MenuItem><MenuItem.Header>_File</MenuItem.Header><MenuItem Header='New'></MenuItem><Separator /></MenuItem></Menu>"+
                    "<TabControl Height='50' Margin='2,2,2,2'><TabItem Header='Item1'><StackPanel Width='100'><Button>Button1</Button></StackPanel></TabItem></TabControl>"+
                    "<TextBox>TextBox</TextBox>"+
                    "<RichTextBox><FlowDocument><Paragraph>RichTextBox</Paragraph></FlowDocument></RichTextBox>"+
                    "<Canvas Width='100' Height='100' Background='Red'/>"+
                    "<DockPanel Width='100' Height='100' Background='Green'/>"+
                    "<FlowDocumentScrollViewer Width='100' Height='100'><FlowDocument Background='Blue'><Paragraph>abc</Paragraph></FlowDocument></FlowDocumentScrollViewer>"+
                    "<StackPanel Width='100' Height='100' Background='Yellow'><Button /></StackPanel>"+
                    "<TextBlock Background='Yellow'><Button />Text</TextBlock>",
                    3, 100, 30, 35, 100, 30, ""),

                // 21: [TC 266]: DragDrop lot of plan text and internationaltext in to another RichTextBox
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    StringData.MixedScripts.Value+LongTextString()+StringData.MixedScripts.Value+LongTextString(), 0, 10000, 30, 35, 10, 10, ""),

                // 22: [TC 267]: DragDrop Paragraph text and normal text into another RichTextBox Regression_Bug40
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "<Paragraph><Bold>abc def ghi</Bold></Paragraph><Paragraph> zzz.</Paragraph>", 2, 22, 30, 35, 10, 10, "abc def ghi\r\n zzz."),

                // 23: [TC 220]: DragDrop plain text into RichTextbox Regression_Bug41
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abc def ghi zzz.", 2, 5, 30, 15, 10, 15, ""),

                // 24: [TC 215]: DragDrop word to snap caret word by word Regression_Bug42
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "xxx wwwww yyy mmm zzz ", 16, 8, 220, 10, -145, 0, ""),

                // 25: [TC 217]: DragDrop word over button object to snap caret to right side
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc def ghi xxx <Button Height='80'>www yyy </Button>mmm zzzzzzzzzz ", 26, 11, 210, 18, -70, 0, ""),

                // 26: [TC 216]: DragDrop word over Embedded text to snap caret to left Regression_Bug43, Regression_Bug44
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "ghi jkl <TextBlock>TextEmbeded</TextBlock> zzz ", 0, 6, 25, 10, 100, 0, ""),

                // 27: [TC 218]: DragDrop plain text then press {Left},{Righ},{Up}or{Down} to destroy selection. Regression_Bug45
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc def ghi zzz.", 0, 6, 20, 13, 70, 35, ""),

                // 28: [TC 219, 229]: DragDrop should show a caret at the insertion point Regression_Bug46
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abc def ghi zzz.", 0, 8, 30, 15, 10, 15, ""),

                // 29: [TC 268]: DragDrop rich text back to original position causes the dropped text to lost selection Regression_Bug47
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Bold>abc </Bold>123 456 xxxyyy de ", 0, 6, 10, 13, 115, 0, "abc"),

                // 30: [TC 269]: DragDrop plan text back to original position 
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc 123 456 xxxyyy de ", 0, 6, 10, 13, 120, 0, "abc "),

                // 31: [TC 270, 230]: DragDrop in multilines text of RichTextBox and make sure caret position is correct Regression_Bug48
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "xxxwyyy abc 123456\n 456 def", 8, 10, 140, 13, -77, 2, ""),

                // 32: [TC 221]: DragDrop text to a bold/normal boundary, verify dropped text is selected and not bold Regression_Bug49
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph>ABCD<Run FontWeight=\"Bold\">EFG</Run></Paragraph>", 2, 2, 20, 13, 55, 15, ""),

                // 33: [TC 231, 271]: DragDrop text from begining and drop to end of a line
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc def ghi 123 end!", 0, 5, 13, 13, 240, 0, ""),

                // 34: [TC 232, 272]: DragDrop text from end and drop to begining of a line
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "abc def ghi 123 end!", 8, 10, 140, 13, -140, 0, ""),

                // 35: [TC 222]: Drag drop bold to U/I/B text
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    " <Bold>abc def</Bold> ghi <Bold><Italic><Underline>123  end!</Underline></Italic></Bold><Button Height='80'/>"
                    , 0, 10, 30, 15, 180, 0, ""),

                // 36: [TC 223]: Drag drop italic to U/I/B text
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Italic>abc def</Italic> ghi <Bold><Italic><Underline>123  end!</Underline></Italic></Bold>"
                    , 2, 5, 35, 13, 145, 15, ""),

                // 37: [TC 224]: Drag drop underline to U/I/B text : Regression_Bug50
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Underline>abc def</Underline> ghi <Underline>123  end!</Underline>"
                    , 2, 6, 35, 13, 142, 10, ""),

                // 38: [TC 225]: Drag drop U/I/B to U/I/B text Regression_Bug50
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Bold><Italic><Underline>abc def</Underline></Italic>"+
                    "</Bold>ghi <Bold><Italic><Underline>123  end!</Underline></Italic></Bold>"
                    , 6, 10, 37, 13, 125, 10, ""),

                // 39: [TC 233, 226]: Drag text from begining and drop copy to end of a line
                new TestCaseData(TestCaseAction.DragAndDropCopy, TestCaseContainer.SameContainer,
                    "abc def ghi 123 end!", 0, 3, 10, 13, 120, 0, ""),

                // 40: [TC 234, 227]: Drag text from begining and drop cancel to end of a line
                new TestCaseData(TestCaseAction.DragAndDropCancel, TestCaseContainer.SameContainer,
                    "abc def ghi 123 end!", 0, 3, 13, 13, 120, 0, ""),

                // 41: [TC 235, 228]: Drag text from begining and drop on self
                new TestCaseData(TestCaseAction.DragAndDropOnSelf, TestCaseContainer.SameContainer,
                    "abc def ghi 123 end!", 0, 5, 13, 13, 10, 0, ""),
                
                // 42: [TC 236]: Drag drop mix content to cross app RTB
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossApp,
                    "abc<Button>Hello</Button> def<Bold>Bold \r\n This is line2.</Bold>" , 0, 40, 10, 13, 450, 13,
                    "abc  defBold \n This is line2."),

                // 43: [TC 237]: DragDrop accross Textbox then drag back
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "abc def ghi zzz.", 0, 3, 9, 15, 10, 10, "abc"),

                // 44: [TC 238]: Drag drop text cross app
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossApp,
                    "abc def ghi" , 0, 3, 15, 15, 440, 15, "abc"),

                // 45: [TC 239]: TextBox: Drag copy cross textbox
                new TestCaseData(TestCaseAction.DragAndDropCopy, TestCaseContainer.CrossContainer,
                    "abc def ghi 123 end!", 0, 3, 10, 13, 10, 10, ""),

                // 46: [TC 240]: TextBox: Drag cancel cross textbox
                new TestCaseData(TestCaseAction.DragAndDropCancel, TestCaseContainer.CrossContainer,
                    "abc def ghi 123 end!", 0, 3, 10, 13, 10, 10, ""),

                // 47: [TC 241]: TextBox: Drag copy cross avalon
                new TestCaseData(TestCaseAction.DragAndDropCopy, TestCaseContainer.CrossApp,
                    "abc def ghi" , 0, 3, 10, 13, 450, 13, "abc"),

                // 48: [TC 242]: DragDrop selection in TableCell outside of table [Regression_Bug51]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Table><TableRowGroup><TableRow><TableCell><Paragraph>ab</Paragraph></TableCell></TableRow>"+
                    "</TableRowGroup></Table><Paragraph>cd</Paragraph>",
                    2, 6 ,15, 15, 25, 40, "ab"),

                // 49: [TC 243]: DragDrop selection include floater [Regression_Bug52, Regression_Bug53]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph>ab<Floater Background=\"LightBlue\"><Paragraph>cd</Paragraph></Floater>ef</Paragraph>",
                    0, 15, 18, 10, 55, 0, "bcde"),

                // 50: [TC 244]: DragDrop by mousedown on right half size of a big font character [Regression_Bug54]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.CrossContainer,
                    "<Paragraph FontSize=\"36.75\">W</Paragraph>",
                    2, 1, 33, 30, 10, 10, "W"),

                // 51: [TC 245]: DragDrop formated text into another paragraph with different formatted [Regression_Bug55]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph FontSize='20pt'>In a RichTextBox.</Paragraph>"+
		            "<Paragraph Foreground='Red' FontSize='10pt'>some more text.</Paragraph>",
                    7, 11, 80, 20, 10, 50, "RichTextBox"),

                // 52: [TC 246]: DragDrop paragraph 2 to in front of paragraph1 [Regression_Bug56]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
                    "<Paragraph>a</Paragraph>"+
		            "<Paragraph>MM</Paragraph>",
                    6, 3, 10, 65, -10, -45, "MMa"),

                // 53: [TC 247]: Drop text to the boundary of <Run> element [Regression_Bug56, Regression_Bug58]. Select efg drop infront of bcd
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
		            "<Paragraph><Span Background='red'>bcd efg hijk lm</Span></Paragraph>",
                    7, 12, 120, 15, -120, 15, "efg hijk lm"),

                // 54: [TC 248]: Drag drop by mouse down on right half side on chracter [Regression_Bug57]
                new TestCaseData(TestCaseAction.DragAndDrop, TestCaseContainer.SameContainer,
		            "<Paragraph>m<Run FontSize='30'>w</Run></Paragraph>",
                    5, 1, 35, 30, -25, 0, "w"),
            };
        }

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            BuildWindow();
            this.MainWindow.Width = 300;
            this.MainWindow.Height = 225;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();

            //Find element
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            _editBox2 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb2"));

            testCaseIndex = ConfigurationSettings.Current.GetArgumentAsInt("CaseIndex");
            testCaseData = TestCaseData.Cases[testCaseIndex];

            //Find point on textBox
            _rc1 = ElementUtils.GetScreenRelativeRect(_editBox1.Element);
            _p1 = new System.Windows.Point(_rc1.Left + testCaseData.DragLocationX, _rc1.Top + testCaseData.DragLocationY);
            if (testCaseData.TestContainer == TestCaseContainer.SameContainer ||
                testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                _p2.X = _p1.X + testCaseData.DropLocationX;
                _p2.Y = _p1.Y + testCaseData.DropLocationY;
            }
            else
            {
                _rc2 = ElementUtils.GetScreenRelativeRect(_editBox2.Element);
                _p2 = new System.Windows.Point(_rc2.Left, _rc2.Top);
                _p2.X = _p2.X + testCaseData.DropLocationX;
                _p2.Y = _p2.Y + testCaseData.DropLocationY;
            }
            _editBox1.Element.Focus();
            this.MainWindow.Topmost = true;
            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            StartTest();
        }

        private void StartTest()
        {
            if ((testCaseIndex == 0) ||
                (testCaseIndex == 1) || 
                (testCaseIndex == 7) ||
                (testCaseIndex == 9) ||
                (testCaseIndex == 33))
            {
                MainWindow.Width *= 2;
                ((Control)(_editBox1.Element)).Width *= 2;
                ((Control)(_editBox2.Element)).Width *= 2;
            }
            testCaseData = TestCaseData.Cases[testCaseIndex];   // Run the test case specified
            Log("-----" + testCaseIndex.ToString());
            if (_editBox1.Element is TextBox)
            {
                _editBox1.Text = testCaseData.TestString;
                ((TextBox)(_editBox1.Element)).FontSize = 20;                
            }
            else //RichTextBox
            {
                _editBox1.XamlText = testCaseData.TestString;
                if ((testCaseIndex == 20) ||
                    (testCaseIndex == 25) ||
                    (testCaseIndex == 12) ||
                    (testCaseIndex == 13) ||
                    (testCaseIndex == 17) ||
                    (testCaseIndex == 18) ||
                    (testCaseIndex == 19))
                {
                }
                else
                {
                    if (testCaseIndex == 28) //Testing drop caret
                    {
                        ((Control)_editBox2.Element).FontSize = 6 * (96.0 / 12.0);
                        ((RichTextBox)(_editBox1.Element)).FontSize = 20;
                        _editBox2.XamlText = "-------";
                        ReflectionUtils.InvokeInstanceMethod(_editBox2, "Select", new object[] { 3, 0 });
                    }
                    else
                    {
                        ((RichTextBox)(_editBox1.Element)).FontSize = 20;
                    }
                }
            }
            if (_editBox2.Element is TextBox)
            {
                if (testCaseIndex == 28) //Testing drop caret
                {
                    ((Control)_editBox2.Element).FontSize = 6 * (96.0 / 12.0);
                    _editBox2.Text = "-------";
                }
            }
            Log("*******Running new test case:[" + testCaseIndex + "]*******");
            Log("Test Action:[" + testCaseData.TestAction + "]");  // Log action of the test
            Log("Test Container1 is :[" + EditableBox1 + "-" + testCaseData.TestContainer + "]");
            Log("Text in editBox1 [" + _editBox1.Text + "]");
            if (testCaseData.TestContainer == TestCaseContainer.CrossContainer)
            {
                Log("Test Container2 is :[" + EditableBox2 + "]"); // Log container type for second box
                Log("Text in editBox2 [" + _editBox2.Text + "]");
            }

            Microsoft.Test.Threading.DispatcherHelper.DoEvents();
            InputMonitorManager.Current.IsEnabled = false;

            // Select text in editbox1
            ReflectionUtils.InvokeInstanceMethod(_editBox1, "Select",
                new object[] { testCaseData.StartSelection, testCaseData.EndSelection });
            // Get original text to compare after action
            _originalText = _editBox1.Text;
            _originalSelectedText = _editBox1.GetSelectedText(false, false);
            _originalSelectedXamlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);

            if (testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                // Launch second app
                if (_editBox1.Element is TextBox)
                {
                    _process = Avalon.Test.Win32.Interop.LaunchAProcess("EditingTest.exe",
                        "/TestCaseType=DragDropRegressionBugsForSecondApp /EditableBox1:TextBox /CaseIndex:" + testCaseIndex);
                }
                else
                {
                    _process = Avalon.Test.Win32.Interop.LaunchAProcess("EditingTest.exe",
                        "/TestCaseType=DragDropRegressionBugsForSecondApp /CaseIndex:" + testCaseIndex);
                }
                //Hard coding the delay for the second app causes bogus failures. 
                //We may think about using mutext, register flag to singal the state of the second app.
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(10), new SimpleHandler(StartAction));
            }
            else if (testCaseData.TestContainer == TestCaseContainer.CrossContainer)
            {
                _editBox1.Element.AddHandler(DragDrop.QueryContinueDragEvent, new QueryContinueDragEventHandler(VerifyQueryContinueDragEvent), true);
                _editBox1.Element.AddHandler(DragDrop.DragEnterEvent, new DragEventHandler(VerifyDragEnterEvent), true);
                _editBox1.Element.AddHandler(DragDrop.GiveFeedbackEvent, new GiveFeedbackEventHandler(VerifyGiveFeedbackEvent), true);
                _editBox2.Element.AddHandler(DragDrop.DropEvent, new DragEventHandler(VerifyDropEvent), true);
                _editBox1.Element.AddHandler(DragDrop.DragLeaveEvent, new DragEventHandler(VerifyDragLeaveEvent), true);
                _editBox2.Element.AddHandler(DragDrop.DragOverEvent, new DragEventHandler(VerifyDragOverEvent), true);
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(StartAction));
            }
            else
            {
                QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(2), new SimpleHandler(StartAction));
            }
        }

        #region Verify events

        private void VerifyQueryContinueDragEvent(object sender, QueryContinueDragEventArgs args)
        {
            _queryContinueDragEvent = true;
        }

        private void VerifyDragEnterEvent(object sender, DragEventArgs args)
        {
            _dragEnterEvent = true;
        }

        private void VerifyGiveFeedbackEvent(object sender, GiveFeedbackEventArgs args)
        {
            _giveFeedbackEvent = true;
        }

        private void VerifyDropEvent(object sender, DragEventArgs args)
        {
            _dropEvent = true;
        }

        private void VerifyDragLeaveEvent(object sender, DragEventArgs args)
        {
            _dragLeaveEvent = true;
        }

        private void VerifyDragOverEvent(object sender, DragEventArgs args)
        {
            _dragOverEvent = true;
        }

        #endregion Verify events

        private void StartAction()
        {
            if (testCaseData.TestAction == TestCaseAction.DragAndDropCopy)
            {
                Log("Pressing down control key...");
                KeyboardInput.PressVirtualKey(Win32.VK_CONTROL);
            }            
            MouseInput.MouseDown(_p1);            
            MouseInput.MouseDragInOtherThread(_p1, _p2, false, TimeSpan.Zero,
                OnMouseUp, Dispatcher.CurrentDispatcher);
        }

        private void OnMouseUp()
        {
            if (testCaseData.TestContainer == TestCaseContainer.CrossApp)
            {
                KeyboardInput.ReleaseVirtualKey(Win32.VK_CONTROL);                
                MouseInput.MouseUp();                
                
                QueueDelegate(VerifySecondApp);
            }
            else if (testCaseIndex == 28)  //Testing drop caret
            {
                MouseInput.MouseUp();                
                System.Drawing.Rectangle caretRect;
                Log("Verifying that caret is shown in editBox2...");
                FindCaret(_editBox2.Element, out caretRect);
                Verifier.Verify(caretRect.Height > 40
                    , "Expect caret height > 40. Actual caret height: " + caretRect.Height);
                Logger.Current.ReportSuccess();
            }
            else
            {
                if (testCaseData.TestAction == TestCaseAction.DragAndDropCancel)
                {
                    KeyboardInput.TypeString("{Esc}");
                    QueueDelegate(new SimpleHandler(VerifyDragAndDropCancel));
                }
                else
                {
                    MouseInput.MouseUp();                    

                    if (testCaseData.TestAction == TestCaseAction.DragAndDropOnSelf)
                    {
                        QueueDelegate(new SimpleHandler(VerifyDragAndDropOnSelf));
                    }
                    else if (testCaseData.TestAction == TestCaseAction.DragAndDropCopy)
                    {
                        Log("Releasing control key...");
                        KeyboardInput.ReleaseVirtualKey(Win32.VK_CONTROL);
                        QueueDelegate(new SimpleHandler(VerifyResult));
                    }
                    else
                        QueueDelegate(new SimpleHandler(VerifyResult));
                }
            }
        }

        private void VerifySecondApp()
        {
            //waiting for 10 second for the second app to exit.
            //this seems better than sleep and then checking Process.HasExit property.
            if (_process.WaitForExit(10000))
            {
                Verifier.Verify(Logger.Current.ProcessLog("DragDropRegressionBugsLog.txt"), "Content dropped in 2nd app.", true);
                Logger.Current.ReportSuccess();
            }
            else
                Logger.Current.ReportResult(false, "process has not exited.", false);
        }

        private void VerifyDragAndDropCancel()
        {
            // Verify text remains the same
            Verifier.Verify(_editBox1.Text == _originalText, "Make sure text remains after hit Esc."
                + "Expect text in editbox1 [" + _originalText + "]"
                + "Actual text in editbox1 after action [" + _editBox1.Text + "]");
            // Verify selection remians
            Verifier.Verify(_editBox1.GetSelectedText(false, false) == _originalSelectedText
                    , "Make sure selection remains after hit Esc."
                    + "Expect text selection [" + _originalSelectedText + "]"
                    + "Actual text selection after action [" + _editBox1.GetSelectedText(false, false) + "]");
            Logger.Current.ReportSuccess();
        }

        private void VerifyDragAndDropOnSelf()
        {
            // Verify text remains the same
            Verifier.Verify(_editBox1.Text == _originalText, "Make sure text remains after drop on self."
                + "Expect text in editbox1 [" + _originalText + "]"
                + "Actual text in editbox1 after action [" + _editBox1.Text + "]");
            // Verify selection got destroy
            Verifier.Verify(_editBox1.GetSelectedText(false, false) == ""
                    , "Make sure selection got destroy when drop on self"
                    + "Expect text selection []"
                    + "Actual text selection after action [" + _editBox1.GetSelectedText(false, false) + "]");
            Logger.Current.ReportSuccess();
        }

        private void VerifyResult()
        {
            // Verify text is rearranged after dragdrop
            Verifier.Verify(_editBox1.Text != _originalText, "Make sure text is rearranged after drop."
                + "Expect text in editbox1 [" + _originalText + "]"
                + "Actual text in editbox1 after action [" + _editBox1.Text + "]");

            // Verify Dropped text is selected
            if (testCaseData.TestContainer == TestCaseContainer.SameContainer)
            {
            
                string str = _editBox1.GetSelectedText(false, false);

                //When selection starts from the listitems. the items (number or bullet) count in the selection after the drop.
                //we should remove them for comparing.
                if (testCaseIndex == 3)
                {
                    //bullet has two chars
                    str = str.Substring(2);
                }
                if (testCaseIndex == 6)
                {
                    //number has three chars.
                    str = str.Substring(3);
                }
                Verifier.Verify(str == _originalSelectedText
                    , "Make sure dropped text selected and matched."
                    + "Expect text selection [" + _originalSelectedText + "]"
                    + "Actual text selection after action [" + str + "]");
                
                _dropptedSelectedXamlText = XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance);
                QueueDelegate(VerifyDragDropInSameContainer);
            }
            else
            {
                if (testCaseData.ExpectString == string.Empty)
                {
                    testCaseData.ExpectString = _editBox2.GetSelectedText(false, false);
                }
                Verifier.Verify(testCaseData.ExpectString == _originalSelectedText
                    , "Make sure dropped text selected and matched."
                    + "Expect text selection [" + _originalSelectedText + "]"
                    + "Actual text selection after action [" + testCaseData.ExpectString + "]");

                _dropptedSelectedXamlText = XamlUtils.TextRange_GetXml(_editBox2.SelectionInstance);
                QueueDelegate(VerifyDragDropInCrossContainer);
            }
        }

        private void VerifyDragDropInSameContainer()
        {
            // For drag drop with image or other control, xml:space="preserve" is added after declaring namespce
            if (_originalSelectedXamlText.Contains("xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""))
            {
                //xml:space="preserve" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                _originalSelectedXamlText = _originalSelectedXamlText.Replace(
                    "xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\"");
            }
            if (_dropptedSelectedXamlText.Contains("xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\""))
            {
                //xml:space="preserve" xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
                _dropptedSelectedXamlText = _originalSelectedXamlText.Replace(
                    "xml:space=\"preserve\" xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\"",
                    "xmlns=\"http://schemas.microsoft.com/winfx/2006/xaml/presentation\" xml:space=\"preserve\"");
            }

            // Verify caret is snap by word when drag drop word in RichTextBox
            if (testCaseIndex == 24)
            {
                Verifier.Verify(_editBox1.Text == "xxx mmm zzz wwwww yyy \r\n", "Drag Drop is snap word by word"
                    + "\nExpect text [xxx mmm zzz wwwww yyy \r\n]"
                    + "\nActual text [" + _editBox1.Text + "]", true);
            }
            // Verify caret is snap to left of text object
            else if (testCaseIndex == 25)
            {
                Verifier.Verify(_editBox1.Text == "abc def ghi xxx   zzzzzzzzzzmmm \r\n", "Drag Drop is snap to right side of Button."
                    + "\nExpect text [abc def ghi xxx   zzzzzzzzzzmmm \r\n]"
                    + "\nActual text [" + _editBox1.Text + "]", true);
            }
            // Verify caret is snap to left or right of embeded object.
            else if (testCaseIndex == 26)
            {
                Verifier.Verify(_editBox1.Text == "jkl ghi   zzz \r\n", "Caret snap to left when dropping on Text object."
                    + "\nExpect text [jkl ghi   zzz \r\n]"
                    + "\nActual text in TextBox1 [" + _editBox1.Text + "]");
            }
            else if (testCaseIndex == 27)
            {
                KeyboardInput.TypeString("{Right}");
            }
            // Verify caret is at correct positon when drag drop in multiline content 
            else if (testCaseIndex == 31)
            {
                Verifier.Verify(_editBox1.Text != "xxxwyyy abc 123456\n 456 def", "Regression_Bug48: caret was at incorrect position."
                    + "In RichTextBox, currently failed with Regression_Bug428."
                    + "\nExpect text [xxxwyyy abc 123456\n 456 def]"
                    + "\nActual text [" + _editBox1.Text + "]");
            }
            else if (testCaseIndex == 33)
            {
                Verifier.Verify(_editBox1.Text == "ef ghi 123 end!abc d", "Text should drop at the end."
                    + "\nExpect text [ef ghi 123 end!abc d]"
                    + "\nActual text in TextBox1 [" + _editBox1.Text + "]");
            }
            else if (testCaseIndex == 34)
            {
                Verifier.Verify(_editBox1.Text == "ghi 123 enabc def d!", "Text should drop at the begining point."
                    + "\nExpect text [ghi 123 enabc def d!]"
                    + "\nActual text in TextBox1 [" + _editBox1.Text + "]");
            }
            else if (testCaseIndex == 37)
            {
                Verifier.Verify(_dropptedSelectedXamlText.Contains("<Run TextDecorations=\"Underline\">abc d</Run>")
                    , "Dropped text selected and matched format."
                    + "\nExpect [<Run TextDecorations=\"Underline\">abc d</Run>]"
                    + "\n\nActual text selection after action in Xaml format [" + _dropptedSelectedXamlText + "]");                
            }
            else if (testCaseIndex == 38)
            {
                // In test 38, the dropped text contain Underline FontStyle="Italic" FontWeight="700">def</Underline>
                Verifier.Verify(_dropptedSelectedXamlText.Contains("<Run FontStyle=\"Italic\" FontWeight=\"Bold\" TextDecorations=\"Underline\">bc def</Run>")
                    , "Dropped text selected and matched format."
                    + "\nExpect [<Run FontStyle=\"Italic\" FontWeight=\"Bold\" TextDecorations=\"Underline\">bc def</Run>]"
                    + "\n\nActual text selection after action in Xaml format [" + _dropptedSelectedXamlText + "]");
            }
            else
            {
                // Verify Dropped text contains the Xml same content as before the action
                Verifier.Verify(_originalSelectedXamlText == _dropptedSelectedXamlText
                    , "Dropped text selected and matched format."
                    + "\nExpect text selection in Xaml format [" + _originalSelectedXamlText + "]"
                    + "\n\nActual text selection after action in Xaml format [" + _dropptedSelectedXamlText + "]");
            }

            if (Settings.GetArgumentAsBool("DragAgain"))
                QueueDelegate(DragAgain);
            else
                QueueDelegate(DoUndo);
        }
        
        private void VerifyDragDropInCrossContainer()
        {
            // Do undo then redo on Drop target => selection will remain after redo
            // Do undo on drag source => original content will remain select
            // Use TextRangeComparer to compare the 2 selection.
            if (Settings.GetArgumentAsBool("DragAgain"))
                QueueDelegate(new SimpleHandler(DragAgain));
            else
                QueueDelegate(new SimpleHandler(DoUndo));
        }
        
        private void DoUndo()
        {
            // Verify all events are fired for caseID 45
            if (testCaseIndex == 45)
            {
                Verifier.Verify(_queryContinueDragEvent, "QueryContinueDragEvent fired!", true);
                Verifier.Verify(_giveFeedbackEvent, "GiveFeedbackEvent fired!", true);
                Verifier.Verify(_dragEnterEvent, "DragEnterEvent fired!", true);
                Verifier.Verify(_dropEvent, "DropEvent fired!", true);
                Verifier.Verify(_dragLeaveEvent, "DragLeaveEvent is fired!", true);
                Verifier.Verify(_dragOverEvent, "DragOverEvent is fired!", true);
            }
            // Do undo.
            // If drag drop in samecontainer, undo/redo is in editbox1
            // If drag drop in croscontainer, undo/redo is in editbox2 only
            KeyboardInput.TypeString("^z");
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new SimpleHandler(VerifyUndo));
        }

        private void VerifyUndo()
        {
            // Verify selection is remain the same after Undo.
            if (testCaseData.TestContainer == TestCaseContainer.SameContainer)
            {
                Verifier.Verify(_editBox1.GetSelectedText(false, false) == _originalSelectedText, "Text should be selected and matched after Undo."
                    + "\nActual selection [" + _editBox1.GetSelectedText(false, false) + "]" + "\nExpect selection [" + _originalSelectedText + "]");

                // Verify undo move back text to original position.
                Verifier.Verify(_editBox1.Text == _originalText, "Text is moved back to original position after Undo."
                    + "\nActual text content [" + _editBox1.Text + "]" + "\nExpect text content [" + _originalText + "]");
            }
            // Verify there is no selection for crosscontainer
            else
            {
                if (_editBox2.Element is TextBox)
                {
                    Verifier.Verify(_editBox2.Text == "", "Undo should remove dropped text."
                        + "\nActual text [" + _editBox2.Text + "]" + "\nExpect text []");
                }
                else if (_editBox2.Element is RichTextBox)
                {
                    Verifier.Verify(_editBox2.Text == "\r\n", "Undo should remove dropped text."
                        + "\nActual text [" + _editBox2.Text + "]" + "\nExpect text []");
                }
            }

            // Do Redo.
            // If drag drop in samecontainer, undo/redo is in editbox1
            // If drag drop in croscontainer, undo/redo is in editbox2 only
            KeyboardInput.TypeString("^y");
            QueueHelper.Current.QueueDelayedDelegate(TimeSpan.FromSeconds(1), new SimpleHandler(VerifyRedo));
        }
        private void VerifyRedo()
        {
            if (testCaseData.TestContainer == TestCaseContainer.SameContainer)
            {
                string str = _editBox1.GetSelectedText(false, false);

                //When selection starts from the listitems. the items (number or bullet) count in the selection after the drop.
                //we should remove them for comparing.
                if (testCaseIndex == 3)
                {
                    //bullet has two chars
                    str = str.Substring(2);
                }
                if (testCaseIndex == 6)
                {
                    //number has three chars.
                    str = str.Substring(3);
                }
                Verifier.Verify( str== _originalSelectedText, "Text should be selected and matched after Redo."
                    + "\nActual selection [" + str + "]" + "\nExpect selection [" + _originalSelectedText + "]");

                // Verify text in editBox1 changed after Redo
                Verifier.Verify(_editBox1.Text != _originalText, "Text should be repositioned after Redo."
                    + "\nActual text content [" + _editBox1.Text + "]" + "\nExpect text content [" + _originalText + "]");
            }
            else
            {                
                if (testCaseIndex == 16)
                {
                    Verifier.Verify(_editBox2.XamlText.Contains("TableColumn Background=\"#FFFF0000\""),
                        "TableColumn should contain Background property." + _editBox2.XamlText);
                }

                // Do Undo in editbox1
                RoutedCommand undoCommand = ApplicationCommands.Undo;
                undoCommand.Execute(null, _editBox1.Element);

                // Verify undone content in editbox1 equal to selection in editbox2
                TextPointer tpStart1;
                TextPointer tpEnd1;
                TextPointer tpStart2;
                TextPointer tpEnd2;
                tpStart1 = _editBox1.SelectionInstance.Start;
                tpEnd1 = _editBox1.SelectionInstance.End;
                tpStart2 = _editBox2.SelectionInstance.Start;
                tpEnd2 = _editBox2.SelectionInstance.End;
                bool compareTextRangeContents;
                string unmatchedReason;
                int embeddedObjectCount, imageEmbeddedObjectCount;

                //Do TextTree comparison if there are no embeddedObjects or if the embeddedObjects are only of type Image
                embeddedObjectCount = TextOMUtils.EmbeddedObjectCountInRange(new TextRange(tpStart1, tpEnd1));
                imageEmbeddedObjectCount = TextOMUtils.EmbeddedObjectCountInRange(new TextRange(tpStart1, tpEnd1), typeof(Image));
                if ((embeddedObjectCount == 0) || ((embeddedObjectCount != 0) && (embeddedObjectCount == imageEmbeddedObjectCount)))
                {
                    compareTextRangeContents = TextTreeTestHelper.CompareTextRangeContents(
                    tpStart1, tpEnd1, tpStart2, tpEnd2, out unmatchedReason);
                    Verifier.Verify(compareTextRangeContents, "Drop content matched!" +
                        "r\nExpect [" + XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance) + "]\nActual [" + XamlUtils.TextRange_GetXml(_editBox2.SelectionInstance) + "]\r\n" + unmatchedReason);
                }
                else
                {
                    Log("embedded objects present...comparing only xaml");
                    Verifier.Verify(XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance)==XamlUtils.TextRange_GetXml(_editBox2.SelectionInstance),
                        "Drop Xaml content matched!" +
                        "\r\nExpect [" + XamlUtils.TextRange_GetXml(_editBox1.SelectionInstance) + "]\nActual [" + XamlUtils.TextRange_GetXml(_editBox2.SelectionInstance) + "]");
                }
            }
            Logger.Current.ReportSuccess();
        }
        
        #region DragAgain

        private void DragAgain()
        {
            QueueDelegate(StartDragAgainAction);
        }
        
        private void StartDragAgainAction()
        {
            System.Windows.Point p3 = _p1;
            p3.X = _p1.X - 5; //supposely drag back to original location
            p3.Y = _p1.Y;
            _p2.X = _p2.X + 5;
            MouseInput.MouseDragInOtherThread(_p2, p3, true, TimeSpan.Zero,
                VerifyDragAgainResult, Dispatcher.CurrentDispatcher);
        }

        private void VerifyDragAgainResult()
        {
            Verifier.Verify(_editBox1.Text == _originalText, "Expect [" + _originalText + "]\nActual [" + _editBox1.Text + "]");
            // Added: check for selection after redrop. - 
            Verifier.Verify(_editBox1.SelectionInstance.Text == testCaseData.ExpectString, "Drop text should be selected after drop again.\nActual [" +
                _editBox1.SelectionInstance.Text + "]\nExpected ["+testCaseData.ExpectString+"]");
            Logger.Current.ReportSuccess();
        }

        #endregion DragAgain
        /// <summary>This function is copied from CaretElement.cs
        /// Looks for a caret in the specified element.</summary>
        /// <param name='element'>Element to look for caret in.</param>
        /// <param name='caretRect'>After execution, the rectangle bounding the caret.</param>
        /// <returns>true if the caret was found, false otherwise.</returns>
        public bool FindCaret(UIElement element, out System.Drawing.Rectangle caretRect)
        {
            System.Drawing.Bitmap elementBitmap;
            System.Drawing.Bitmap borderless;
            System.Drawing.Bitmap bw;
            System.Drawing.Bitmap lineBitmap;
            System.Drawing.Rectangle[] lines;
            System.Drawing.Rectangle arbitraryLine;
            int arbitraryLineHeight;

            System.Diagnostics.Debug.Assert(element != null);

            caretRect = System.Drawing.Rectangle.Empty;

            using (elementBitmap = BitmapCapture.CreateBitmapFromElement(element))
            using (borderless = BitmapUtils.CreateBorderlessBitmap(elementBitmap, 3))
            using (bw = BitmapUtils.ColorToBlackWhite(borderless,200))
            {
                bw = BitmapUtils.CreateBitmapClipped(bw, new Thickness(0, 0, 0, 10),false);
                Logger.Current.LogImage(elementBitmap, "DragDropCaret219");
                lines = BitmapUtils.GetTextLines(bw);
                if (lines.Length == 0)
                {
                    return false;
                }
                else
                {
                    Log("Line with caret: " + lines[0].ToString());

                    if (lines[0].Width == 0 || lines[0].Height < 4)
                    {
                        Log("Line has zero width or height.");
                        Log("Using a line arbitrarily smaller than borderless area.");
                        arbitraryLineHeight = (int)
                            ((double)element.GetValue(TextElement.FontSizeProperty));
                        arbitraryLine = lines[0];
                        arbitraryLine.Width = borderless.Width - 2;
                        arbitraryLine.Height = arbitraryLineHeight;
                        Log("Line with arbitraryLine: " + arbitraryLine.ToString());
                        lineBitmap = BitmapUtils.CreateSubBitmap(bw, arbitraryLine);
                    }
                    else
                    {
                        lineBitmap = BitmapUtils.CreateSubBitmap(bw, lines[0]);
                    }

                    // Look for the caret in the line and fix the offset.
                    bool found = BitmapUtils.GetTextCaret(lineBitmap, out caretRect);
                    Logger.Current.LogImage(bw, "mycaretbitmap");
                    if (found)
                    {
                        caretRect.Offset(lines[0].Left, lines[0].Top);
                    }
                    return found;
                }
            }
        }
    }

    /// <summary>
    /// Second app for Testing drag drop - verifies that drag and drop works across application
    /// data-driven scenarios.
    /// </summary>
    public class DragDropRegressionBugsForSecondApp : TestContainer //CustomTestCase
    {
        private UIElementWrapper _editBox1;
        DragDropRegressionBugs _dragDropRegressionBugs;

        /// <summary>Runs the test case</summary>
        public override void RunTestCase()
        {
            //Create log file for logging on second app.
            if (Test.Uis.IO.TextFileUtils.Exists("DragDropRegressionBugsLog.txt"))
                Test.Uis.IO.TextFileUtils.Delete("DragDropRegressionBugsLog.txt");
            Logger.Current.LogToFile("DragDropRegressionBugsLog.txt");
            BuildWindow();
            this.MainWindow.Title = "DragDropRegressionBugsInCrossContainer Second App";
            this.MainWindow.Left = 300;
            QueueDelegate(StartTest);
        }

        /// <summary>Start test action</summary>
        private void StartTest()
        {
            _editBox1 = new UIElementWrapper(ElementUtils.FindElement(MainWindow, "tb1"));
            Log("Test Container2 is :[" + EditableBox1 + "]");
            _dragDropRegressionBugs = new DragDropRegressionBugs();
            _editBox1.Element.AddHandler(DragDrop.DropEvent, new DragEventHandler(Element_Drop), true);
        }

        void Element_Drop(object sender, DragEventArgs e)
        {
            int testIndex = ConfigurationSettings.Current.GetArgumentAsInt("CaseIndex");
            _dragDropRegressionBugs.testCaseData = DataTransfer.DragDropRegressionBugs.TestCaseData.Cases[testIndex];
            Verifier.Verify(_editBox1.SelectionInstance.Text == _dragDropRegressionBugs.testCaseData.ExpectString,
                "Expect [" + _dragDropRegressionBugs.testCaseData.ExpectString + "]\nActual [" + _editBox1.SelectionInstance.Text + "]", true);
            Logger.Current.ReportSuccess();
        }
    }
    #endregion DragDropRegressionBugs

    #region MouseMoveSlow
    //Make the mouse move slower
    /// <summary>Helper class to make the mouse move slowly.</summary>
    public class MyMouseMove
    {
        /// <summary>
        /// Creates a new MouseMoveSlow instance that will have moved the
        /// mouse on return.
        /// </summary>
        public void MouseMoveSlow(int x1, int y1, int x2, int y2)
        {
            double dx = x2 - x1;
            double dy = y2 - y1;
            double Dx = System.Math.Abs(dx);
            double Dy = System.Math.Abs(dy);
            int dragwidth = (int)((Dx > Dy) ? Dx : Dy);

            if (dragwidth != 0)
            {
                dx = dx / dragwidth;
                dy = dy / dragwidth;
                for (int i = 3; i <= dragwidth; i += 2)
                {
                    MouseInput.MouseMove(x1 + (int)(dx * i), y1 + (int)(dy * i));
                    Thread.Sleep(1);
                }

                //the last drag make sure that the mouse goes to the exact position
                MouseInput.MouseMove(x2, y2);
            }
        }
    }
    #endregion MouseMoveSlow

    /// <summary>
    /// Hook up DragLeave event handler and remove the characters from drag source
    /// </summary>
    // DISABLEDUNSTABLETEST:
    // TestName:DragLeaveEventTest
    // Area: Editing�� SubArea: DragDrop
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: findstr /snip DISABLEDUNSTABLETEST
    [Test(2, "DragDrop", "DragLeaveEventTest", MethodParameters = "/TestCaseType:DragLeaveEventTest", Timeout = 300,Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("273"), TestBugs(""), TestWorkItem("27")]
    public class DragLeaveEventTest : ManagedCombinatorialTestCase
    {
        /// <summary>Runs a specific combination.</summary>
        protected override void DoRunCombination()
        {
            StackPanel panel = new StackPanel();
            _tbSource = (TextBoxBase)_editableType.CreateInstance();
            _tbDest = (TextBoxBase)_editableType.CreateInstance();
            _tbSource.Height = _tbDest.Height = 200;
            _tbSource.Width = _tbDest.Width = 200;

            _wrapperSource = new UIElementWrapper(_tbSource);
            _wrapperDest = new UIElementWrapper(_tbDest);

            _wrapperSource.Text = contents;

            _myMouseMove = new MyMouseMove();

            panel.Children.Clear();
            panel.Children.Add(_tbSource);
            panel.Children.Add(_tbDest);

            _tbSource.SelectAll();
            _tbSource.PreviewDragLeave += new DragEventHandler(tbSource_PreviewDragLeave);

            TestElement = panel;
            QueueDelegate(GetFocus);
        }

        private void GetFocus()
        {
            _tbSource.Focus();
            QueueDelegate(AfterFocus);
        }

        private void AfterFocus()
        {
            TextPointer tpSource, tpDest;
            Rect startRect, endRect;

            tpSource = GetRealContentStart(_wrapperSource);
            tpSource = tpSource.GetPositionAtOffset(_wrapperSource.Text.Length / 2);
            startRect = tpSource.GetCharacterRect(LogicalDirection.Forward);
            _startPoint = new Point(startRect.Left + (startRect.Width / 2), startRect.Top + (startRect.Height / 2));
            _startPoint = ElementUtils.GetScreenRelativePoint(_tbSource, _startPoint);

            tpDest = GetRealContentStart(_wrapperDest);
            endRect = tpDest.GetCharacterRect(LogicalDirection.Forward);
            _endPoint = new Point(endRect.Left + (endRect.Width / 2) + _tbDest.Width / 2, endRect.Top + (endRect.Height / 2));
            _endPoint = ElementUtils.GetScreenRelativePoint(_tbDest, _endPoint);

            InputMonitorManager.Current.IsEnabled = false;
            QueueDelegate(new SimpleHandler(InjectKeys));
        }

        private void InjectKeys()
        {
            Log("Dragging contents from Source to Dest - From: " +
                _startPoint.ToString() + " To: " + _endPoint.ToString());

            this._inputSimulationThread = new Thread(new ThreadStart(InjectKeysProc));
            this._inputSimulationThread.SetApartmentState(System.Threading.ApartmentState.STA);
            this._inputSimulationThread.Start();
        }
        private void InjectKeysProc()
        {
            MouseInput.MouseDown(_startPoint);
            _myMouseMove.MouseMoveSlow((int)_startPoint.X, (int)_startPoint.Y, (int)_endPoint.X, (int)_endPoint.Y);

            // Queue an item to be executed in 3 seconds.
            QueueHelper helper = new QueueHelper(GlobalCachedObjects.Current.MainDispatcher);
            helper.QueueDelayedDelegate(TimeSpan.FromSeconds(3), new SimpleHandler(MouseUpAction));
        }

        private void MouseUpAction()
        {
            MouseInput.MouseUp();
        }

        private void LogResult()
        {
            //Changes made in DragLeave event handler shouldnt go through. 
            //Hence verifying with original contents.
            Verifier.Verify((_wrapperSource.Text == string.Empty) || (_wrapperSource.Text == "\r\n"),
                "Verifying source contents", true);
            Verifier.Verify((_wrapperDest.Text.Contains(contents)) && (!_wrapperDest.Text.Contains(appendedContents)),
                "Verifying destination contents", true);

            QueueDelegate(NextCombination);
        }

        void tbSource_PreviewDragLeave(object sender, DragEventArgs e)
        {
            Log("PreviewDragLeave event fired");

            Log("Modifying the contents of DragSource...");
            Log("Before modifying: " + e.Data.GetData(DataFormats.Text));
            e.Data.SetData(DataFormats.Text, contents + appendedContents);
            Log("After modifying: " + e.Data.GetData(DataFormats.Text));

            //Remove the event. Otherwise, since we are doing very slow mouse event, sometimes event gets fired multiple times.
            _tbSource.PreviewDragLeave -= tbSource_PreviewDragLeave;

            QueueHelper.Current.QueueDelayedDelegate(new TimeSpan(0, 0, 5), new SimpleHandler(LogResult), null);
        }

        private TextPointer GetRealContentStart(UIElementWrapper wrapper)
        {
            if (wrapper.Element is TextBox)
            {
                return wrapper.Start;
            }
            else if (wrapper.Element is RichTextBox)
            {                
                return ((RichTextBox)wrapper.Element).Document.ContentStart.GetInsertionPosition(LogicalDirection.Forward);
            }
            else
            {
                return null;
            }
        }

        private TextEditableType _editableType=null;
        private TextBoxBase _tbSource,_tbDest;
        private UIElementWrapper _wrapperSource,_wrapperDest;
        Point _startPoint,_endPoint;
        private Thread _inputSimulationThread = null;
        private MyMouseMove _myMouseMove = null; //For getting MouseMOveSlow        
        private const string contents = "This is a test";
        private const string appendedContents = "xxxxx";
    }

    /// <summary>
    /// Verifies that dragging into a PasswordBox is possible but dragging
    /// out isn't.
    /// </summary>
    // DISABLEDUNSTABLETEST:
    // TestName:PasswordDragDropTest
    // Area: Editing SubArea: DragDrop
    // Disable this case due to high fail rate, will enable after fix it.
    // to find all disabled tests in test tree, use: �findstr /snip DISABLEDUNSTABLETEST� 
    [Test(2, "DragDrop", "PasswordDragDropTest", MethodParameters = "/TestCaseType=PasswordDragDropTest",Disabled = true)]
    [TestOwner("Microsoft"), TestTactics("275"), TestBugs("471,472"), TestWorkItem("22")]
    public class PasswordDragDropTest : CustomTestCase
    {
        #region Main flow.

        /// <summary>Runs the test case.</summary>
        public override void RunTestCase()
        {
            CreateControls();
            QueueDelegate(DragToPasswordBox);
        }

        private void DragToPasswordBox()
        {
            Log("Dragging to PasswordBox...");
            PerformDrag(_textbox, _passwordbox, AfterDragToPasswordBox);
        }

        private void AfterDragToPasswordBox()
        {
            Verifier.VerifyText("PasswordBox text", _initialText, _passwordbox.Password, false);
            Verifier.VerifyText("TextBox text", "", _textbox.Text, false);

            Log("Dragging from PasswordBox...");
            PerformDrag(_passwordbox, _textbox, AfterDragFromPasswordBox);
        }

        private void AfterDragFromPasswordBox()
        {
            Log("Expecting no changes...");
            Verifier.VerifyText("PasswordBox text", _initialText, _passwordbox.Password, false);
            Verifier.VerifyText("TextBox text", "", _textbox.Text, false);

            Logger.Current.ReportSuccess();
        }

        #endregion Main flow.

        #region Helper methods.

        private void CreateControls()
        {
            _panel = new StackPanel();
            _textbox = new TextBox();
            _passwordbox = new PasswordBox();

            _initialText = Test.Uis.Data.StringData.MixedScripts.Value;
            _textbox.Text = _initialText;

            _panel.Children.Add(_textbox);
            _panel.Children.Add(_passwordbox);

            TestWindow.Content = _panel;
        }

        private void PerformDrag(Control source, Control target, SimpleHandler callback)
        {
            Point start;
            Point end;
            UIElementWrapper sourceWrapper;

            sourceWrapper = new UIElementWrapper(source);
            sourceWrapper.SelectAll();

            if (sourceWrapper.Element is TextBox)
            {
                start = sourceWrapper.GetGlobalCharacterRect(2, LogicalDirection.Backward).TopLeft;
                start.Offset(4, 4);
                _textBoxPoint = start;
            }
            else
            {
                start = _textBoxPoint;
                start.Y = ElementUtils.GetScreenRelativeCenter(source).Y;
            }

            end = ElementUtils.GetScreenRelativeCenter(target);

            MouseInput.MouseDragInOtherThread(start, end, true, TimeSpan.FromSeconds(3),
                callback, Dispatcher.CurrentDispatcher);
        }

        #endregion Helper methods.

        #region Private fields.

        private StackPanel _panel;
        private TextBox _textbox;
        private PasswordBox _passwordbox;
        private string _initialText;
        private System.Windows.Point _textBoxPoint;

        #endregion Private fields.
    }
}

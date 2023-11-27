// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Reflection;
using System.Windows;
using System.Drawing;
using SD = System.Drawing;
using SWC = System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using SWF = System.Windows.Forms;
using SWI = System.Windows.Input;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;

//
// Testcase:    WFModality
// Description: Verify the modality of WinForms windows launched from Avalon.
//
namespace WindowsFormsHostTests
{
    public class WFModality : WPFReflectBase
    {
        #region Testcase setup

        SWC.Button _avButton1;
        SWC.Button _avButton2;
        SWC.Button _avButton3;
        SWC.TextBox _avTextBox1;
        SWC.StackPanel _stackPanel;
        Edit _edit1;
        WinForm _winForm1;
        SD.Bitmap _bmp;

        public WFModality(string[] args) : base(args) { }

        protected override void InitTest(TParams p)
        {
            this.UseMITA = true;
            this.Title = "WFModality";
            this.Width = 400;
            this.Height = 400;
            this.Left = 0;
            this.Top = 210;

            _avButton1 = new SWC.Button();
            _avButton1.Name = "avButton1";
            _avButton1.Content = "Launch a Modal WF Form";
            _avButton1.Click += new RoutedEventHandler(avButton1_Click);

            _avButton2 = new SWC.Button();
            _avButton2.Name = "avButton2";
            _avButton2.Content = "Launch a Non Modal WF Form";
            _avButton2.Click += new RoutedEventHandler(avButton2_Click);

            _avButton3 = new SWC.Button();
            _avButton3.Name = "avButton3";
            _avButton3.Content = "Launch a Another Modal WF Form";
            _avButton3.Click += new RoutedEventHandler(avButton3_Click);

            _avTextBox1 = new SWC.TextBox();
            _avTextBox1.Name = "avTextBox1";

            _stackPanel = new SWC.StackPanel();
            _stackPanel.Children.Add(_avButton1);
            _stackPanel.Children.Add(_avButton2);
            _stackPanel.Children.Add(_avButton3);
            _stackPanel.Children.Add(_avTextBox1);
            this.Content = _stackPanel;

            base.InitTest(p);
        }

        void avButton3_Click(object sender, RoutedEventArgs e)
        {
            _winForm1 = new WinForm();
            _winForm1.Name = "WinForm2";
            _winForm1.Text = "WinForm2";
            _winForm1.Location = new System.Drawing.Point(300, 0);
            _winForm1.ShowDialog();
        }

        void avButton2_Click(object sender, RoutedEventArgs e)
        {
            _winForm1 = new WinForm();
            _winForm1.Show();
        }

        void avButton1_Click(object sender, RoutedEventArgs e)
        {
            _winForm1 = new WinForm();
            _winForm1.ShowDialog();
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios

        [Scenario("Launch a Modal WF Form from an AV Window.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "WFModality", "avButton1"))
            {
                return new ScenarioResult(false);
            }
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Verify that the form is really modal
            if (!GetEditControls(p, "WFModality", "avButton1"))
            {
                return new ScenarioResult(false);
            }
            bool modality = false;
            try
            {
                _edit1.SetFocus();
            }
            catch (Exception)
            {
                modality = true;
            }
            WPFMiscUtils.IncCounters(sr, p.log, modality, "Failed at Modality check. Window is not modal.");

            //Send some text to modal form and verify it is received
            bool interop = false;
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            UIObject uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm1"));
            UIObject uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfTextBox1"));
            _edit1 = new Edit(uiControl);
            try
            {
                _edit1.SetFocus();
                _edit1.SendKeys("Testing Interop");
                if (_edit1.Value == "Testing Interop")
                    interop = true;
            }
            catch (Exception)
            {
                interop = false;
            }
            WPFMiscUtils.IncCounters(sr, p.log, interop, "Failed at WindowsFormsInterop test.");

            Utilities.SleepDoEvents(40);
            //Close modal form, need to get WinForm window from within WFModality
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm1"));
            uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            _edit1 = new Edit(uiControl);

            _edit1.Click();
            Utilities.SleepDoEvents(20);
            //Verify that Modal WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            return sr;
        }

        [Scenario("Launch a Non-Modal WF Form from an AV Window.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "WFModality", "avButton2"))
            {
                return new ScenarioResult(false);
            }
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Non-Modal WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed Non-Modal window launch verification. Color=" + SD.SystemColors.Control +
                ". Percent match: " + BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Verify that the form is really non-modal
            if (!GetEditControls(p, "WFModality", "avButton2"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            Utilities.SleepDoEvents(120);
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) < 76,
                "Bitmap Failed at Non-Modal verification. Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Send some text to non-modal form and verify it is received
            bool interop = false;
            if (!GetEditControls(p, "WinForm1", "wfTextBox1"))
            {
                return new ScenarioResult(false);
            }
            try
            {
                _edit1.SetFocus();
                _edit1.SendKeys("Testing Interop");
                if (_edit1.Value == "Testing Interop")
                    interop = true;
            }
            catch (Exception)
            {
                interop = false;
            }
            WPFMiscUtils.IncCounters(sr, p.log, interop, "Failed at WindowsFormsInterop test.");
            Utilities.SleepDoEvents(40);

            //Close non-modal form
            if (!GetEditControls(p, "WinForm1", "wfButton1"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Non-Modal WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Closed verification. Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            return sr;
        }

        [Scenario("Launch a Modal WF Form from an AV Window that launches a Modal WF.")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            if (!GetEditControls(p, "WFModality", "avButton1"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Modal WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Try to launch second modal WF Form from parent, should throw exception
            if (!GetEditControls(p, "WFModality", "avTextBox1"))
            {
                return new ScenarioResult(false);
            }
            bool modality = false;
            try
            {
                _edit1.SetFocus();
                _edit1.SendKeys("Testing");
                if (_edit1.Value != "Testing")
                    modality = true;
            }
            catch (Exception)
            {
                modality = true;
            }
            WPFMiscUtils.IncCounters(sr, p.log, modality, "Failed at Modality check. Form is not modal.");

            //Launch second modal WF Form, need to get WinForm window from within WFModality
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            UIObject uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm1"));
            UIObject uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfButton2"));
            _edit1 = new Edit(uiControl);

            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Second Modal WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Close Second Modal form, need to get WinForm window from within WFModality
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm3"));
            uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            _edit1 = new Edit(uiControl);

            _edit1.Click();
            Utilities.SleepDoEvents(20);
            //Verify that Modal WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Close modal form, need to get WinForm window from within WFModality
            uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm1"));
            uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            _edit1 = new Edit(uiControl);

            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);
            //Verify that Modal WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            return sr;
        }

        [Scenario("Launch a Modal WF Form from an AV Window that launches a Non-Modal WF.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            //First, launch Non-Modal WF Form
            if (!GetEditControls(p, "WFModality", "avButton2"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Non-Modal WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Launch second (modal) WF Form
            if (!GetEditControls(p, "WFModality", "avButton3"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that second (modal) WF Form is launched.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) >= 60,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Verify modality of second (modal) WF Form, should throw exception if it is modal
            if (!GetEditControls(p, "WFModality", "avTextBox1"))
            {
                return new ScenarioResult(false);
            }
            bool modality = false;
            try
            {
                _edit1.SetFocus();
                _edit1.SendKeys("Testing");
                if (_edit1.Value != "Testing")
                    modality = true;
            }
            catch (Exception)
            {
                modality = true;
            }
            WPFMiscUtils.IncCounters(sr, p.log, modality, "Failed at Modality check. Form is not modal.");

            //Close modal form, need to get WinForm window from within WFModality
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("WFModality"));
            UIObject uiDialog = uiApp.Descendants.Find(UICondition.CreateFromId("WinForm2"));
            UIObject uiControl = uiDialog.Descendants.Find(UICondition.CreateFromId("wfButton1"));
            _edit1 = new Edit(uiControl);

            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that second (modal) WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(300, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            //Close non-modal form
            if (!GetEditControls(p, "WinForm1", "wfButton1"))
            {
                return new ScenarioResult(false);
            }
            _edit1.SetFocus();
            _edit1.Click();
            Utilities.SleepDoEvents(20);

            //Verify that Non-Modal WF Form has been closed.
            _bmp = Utilities.GetScreenBitmap(new Rectangle(0, 0, 300, 300));
            Utilities.SleepDoEvents(20);
            WPFMiscUtils.IncCounters(sr, p.log, BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) <= 40,
                "Bitmap Failed at Color=" + SD.SystemColors.Control + ". Percent match: " +
                BitmapsColorPercent(_bmp, 0, 0, 300, 300, SD.SystemColors.Control) + "%");

            return sr;
        }

        #endregion

        #region Helper Functions

        //Gets Mita wrapper controls from Avalon textbox and WinForm textbox, and passes them to _edit1 and 
        //_edit2 respectively.
        bool GetEditControls(TParams p, String window1, String control1)
        {
            UIObject uiApp = null;
            UIObject uiControl = null;
            try
            {
                uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName(window1));
                uiControl = uiApp.Descendants.Find(UICondition.CreateFromId(control1));
                _edit1 = new Edit(uiControl);

                return true;
            }
            catch (Exception ex)
            {
                p.log.WriteLine("Failed to Get Mita wrapper controls" + ex.ToString());
                return false;
            }
        }

        // BitmapsColorPercent
        // Starting at (x, y), look through the area of pixels in bmp specified by width and height 
        // for color match of specified color.
        // For every pixel that matches specified color, increment match counter.
        // Return percentage of matching pixels to total pixels.
        private static double BitmapsColorPercent(Bitmap bmp, int x, int y, int width, int height, Color color)
        {
            double total = 0;
            double match = 0;
            for (int col = x; col < x + width; col++)
            {
                for (int row = y; row < y + height; row++)
                {
                    if (Utilities.ColorsMatch(bmp.GetPixel(col, row), color))
                    {
                        match++;
                    }
                    total++;
                }
            }
            return match * 100 / total;
        }

        #endregion

        //class WinFormWindow creates a new WinForm window
        public class WinForm : SWF.Form
        {
            SWF.Button _wfButton1 = new SWF.Button();
            SWF.Button _wfButton2 = new SWF.Button();
            SWF.TextBox _wfTextBox1 = new SWF.TextBox();

            public WinForm()
            {
                this.SuspendLayout();
                // 
                // wfButton1
                // 
                this._wfButton1.Location = new System.Drawing.Point(95, 63);
                this._wfButton1.Name = "wfButton1";
                this._wfButton1.Text = "Close Window";
                this._wfButton1.AutoSize = true;
                this._wfButton1.Size = new System.Drawing.Size(100, 20);
                this._wfButton1.Click += new EventHandler(wfButton1_Click);
                // 
                // wfButton2
                // 
                this._wfButton2.Location = new System.Drawing.Point(95, 103);
                this._wfButton2.Name = "wfButton2";
                this._wfButton2.Text = "Launch WF Form";
                this._wfButton2.AutoSize = true;
                this._wfButton2.Size = new System.Drawing.Size(100, 20);
                this._wfButton2.Click += new EventHandler(wfButton2_Click);
                //
                // wfTextBox1
                //
                this._wfTextBox1.Name = "wfTextBox1";
                this._wfTextBox1.Location = new System.Drawing.Point(10, 150);
                this._wfTextBox1.Size = new System.Drawing.Size(250, 50);
                this._wfTextBox1.WordWrap = true;
                //
                // winform
                // 
                this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
                this.AutoScaleMode = SWF.AutoScaleMode.Font;
                this.ClientSize = new System.Drawing.Size(292, 266);
                this.Controls.Add(this._wfButton1);
                this.Controls.Add(this._wfButton2);
                this.Controls.Add(this._wfTextBox1);
                this.Name = "WinForm1";
                this.StartPosition = SWF.FormStartPosition.Manual;
                this.Location = new System.Drawing.Point(0, 0);
                this.Text = "WinForm1";
                this.ResumeLayout(false);
                this.PerformLayout();
            }
            void wfButton1_Click(object sender, EventArgs e)
            {
                this.Close();
            }
            void wfButton2_Click(object sender, EventArgs e)
            {
                WinForm winForm3 = new WinForm();
                winForm3.Name = "WinForm3";
                winForm3.Text = "WinForm3";
                winForm3.Location = new System.Drawing.Point(300, 0);
                winForm3.ShowDialog();
            }
        }
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Launch a Modal WF Form from an AV Window.
//@ Launch a non Modal WF From from an AV Window.
//@ Launch a Modal WF Form from an AV Window that launches a Modal WF.
//@ Launch a Modal WF Form from an AV Window that launches a Non-Modal WF.
// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms.Integration;

using MS.Internal.Mita.Foundation;
using MS.Internal.Mita.Foundation.Collections;
using MS.Internal.Mita.Foundation.Controls;
using MS.Internal.Mita.Foundation.Waiters;


namespace WindowsFormsHostTests
{
    public class DragDropBetween2WFH : WPFReflectBase  {

        #region Testcase setup

        DockPanel _dp = new DockPanel();
        WindowsFormsHost _wfh1 = new WindowsFormsHost();
        WindowsFormsHost _wfh2 = new WindowsFormsHost();

        System.Windows.Forms.TextBox _tb1 = new System.Windows.Forms.TextBox();
        System.Windows.Forms.TextBox _tb2 = new System.Windows.Forms.TextBox();
        System.Windows.Forms.DragDropEffects _ddEffect;

        TParams _tp;

        public DragDropBetween2WFH(string[] args) : base(args) { }

        protected override void InitTest(TParams p)
        {
            this.Width = 500;
            this.Height = 500;
            this.Topmost = true;

            _tp = p;

            _tb1.Name = "TB1";
            _tb2.Name = "TB2";
            _wfh1.Name = "WFH1";
            _wfh1.Width = 250;
            _wfh2.Name = "WFH2";
            _wfh2.Width = 250;

            _tb2.AllowDrop = true;
            _tb1.MouseDown += new System.Windows.Forms.MouseEventHandler(tb1_MouseDown);
            _tb2.DragEnter += new System.Windows.Forms.DragEventHandler(tb2_DragEnter);
            _tb2.DragDrop += new System.Windows.Forms.DragEventHandler(tb2_DragDrop);

            _wfh1.Child = _tb1;
            _wfh2.Child = _tb2;

            _dp.Children.Add(_wfh1);
            _dp.Children.Add(_wfh2);

            this.Content = _dp;
            

            this.Title = "DragDropBetween2WFH";
            UseMITA = true;
            base.InitTest(p);
        }

        public Edit _edit1;
        public Edit _edit2;

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            _tb1.Text = "THIS IS SOME TEXT";
            _tb2.Text = "";        

            return base.BeforeScenario(p, scenario);
        }    

        void tb2_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
        {
            _tp.log.WriteLine("TB2 DragDrop:");
            System.Windows.Forms.TextBox tb = (System.Windows.Forms.TextBox)sender;
            System.Windows.Forms.IDataObject data = e.Data;
            if (data.GetDataPresent("Text"))
            {
                tb.Text = (string)data.GetData("Text");
                _tp.log.WriteLine("Text Set: " + tb.Text);
            }
            else
            {
            _tp.log.WriteLine("Text Data Not Present!!!");
            }
        }

        void tb2_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
        {
            _tp.log.WriteLine("TB2 DragEnter:");
            System.Windows.Forms.IDataObject data = e.Data;
            if (data.GetDataPresent("Text"))
            {
                e.Effect = _ddEffect;
                _tp.log.WriteLine("Text Data Present");
            }
            else
            {
                _tp.log.WriteLine("Text Data Not Present!!!");
            }
        }

        void tb1_MouseDown(object sender, System.Windows.Forms.MouseEventArgs e)
        {
            System.Windows.Forms.TextBox tb = ((System.Windows.Forms.TextBox)sender);
            System.Windows.Forms.DragDropEffects dde = tb.DoDragDrop(tb.Text, System.Windows.Forms.DragDropEffects.All);
            _tp.log.WriteLine("Started DragDrop with text: " + tb.Text);
            if (dde == System.Windows.Forms.DragDropEffects.Move)
            {
                tb.Text = "";
            }
        
        }

        void GetEditControls()
        {
            UIObject uiApp = UIObject.Root.Children.Find(UICondition.CreateFromName("DragDropBetween2WFH"));
            BreadthFirstDescendantsNavigator bfTB1 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId("TB1"));
            BreadthFirstDescendantsNavigator bfTB2 = new BreadthFirstDescendantsNavigator(uiApp, UICondition.CreateFromId("TB2"));

            _edit1 = new Edit(bfTB1[0]);
            _edit2 = new Edit(bfTB2[0]);
        }

        public void DragAndDrop(UIObject dragSource, UIObject dropTarget)
        {
            using (PointerInput.Activate(Mouse.Instance))
            {
                System.Drawing.Rectangle sourceRect = dragSource.BoundingRectangle;
                System.Drawing.Point sourcePoint = new System.Drawing.Point
                (
                sourceRect.X + sourceRect.Height / 2,
                sourceRect.Y + sourceRect.Height / 2
                );

                Mouse.Instance.Move(sourcePoint);
                PointerInput.Press(PointerButtons.Primary);

                System.Drawing.Rectangle targetRect = dropTarget.BoundingRectangle;
                System.Drawing.Point targetPoint = new System.Drawing.Point 
                (
                targetRect.X + targetRect.Height / 2,
                targetRect.Y + targetRect.Height / 2
                );

                Mouse.Instance.Move(targetPoint);
                Mouse.Instance.Release(PointerButtons.Primary);
            }
        }
        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
            
        public ScenarioResult DragDropCopy(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            _ddEffect = System.Windows.Forms.DragDropEffects.Copy;
            
            GetEditControls();

            DragAndDrop(_edit1, _edit2);

            GetEditControls();

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(_edit2.Value), String.Format("DragDrop Copy not successful.  TB1={0}, TB2{1}", _edit1.Value, _edit2.Value));

            if(sr.FailCount > 0)
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 24);

            return sr;
        }

        public ScenarioResult DragDropNone(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            _ddEffect = System.Windows.Forms.DragDropEffects.None;
            GetEditControls();

            string str = _edit1.Value;

            DragAndDrop(_edit1, _edit2);

            GetEditControls();

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals(str) && _edit2.Value.Equals(""), String.Format("DragDrop None not successful.  TB1={0}, TB2{1}", _edit1.Value, _edit2.Value));
                
            return sr;
        }

        public ScenarioResult DragDropMove(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            _ddEffect = System.Windows.Forms.DragDropEffects.Move;
            GetEditControls();

            string str = _edit1.Value;

            DragAndDrop(_edit1, _edit2);

            GetEditControls();

            WPFMiscUtils.IncCounters(sr, p.log, _edit1.Value.Equals("") && _edit2.Value.Equals(str), String.Format("DragDrop Move not successful.  TB1={0}, TB2{1}", _edit1.Value, _edit2.Value));

            if (sr.FailCount > 0)
                p.log.LogKnownBug(BugDb.WindowsOSBugs, 24);

            return sr;
        }
        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ DragDropCopy()
//@ DragDropNone()
//@ DragDropMove()

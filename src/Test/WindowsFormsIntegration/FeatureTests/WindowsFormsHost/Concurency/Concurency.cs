// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
// See the LICENSE file in the project root for more information.

using System;
using WFCTestLib.Util;
using WFCTestLib.Log;
using ReflectTools;
using WPFReflectTools;
using System.ComponentModel;
using System.Windows.Controls;
using System.Windows;
using System.Windows.Forms.Integration;


// Testcase:    Concurency
// Description: Verify that concurency is maintained with a shared Datasource
namespace WindowsFormsHostTests
{
    public class Concurency : WPFReflectBase
    {
        #region Testcase setup
        public Concurency(string[] args) : base(args) { }

        // class vars
        private CountObj _co = new CountObj();
        private People _ppl;
        private ListBox _avLB;                           // used in Scenarios 2,3,4
        private System.Windows.Forms.ListBox _wfLB;      // used in Scenarios 2,3,4
        private System.Windows.Forms.ListBox _wfLB1;     // used in Scenarios 5,6,7
        private System.Windows.Forms.ListBox _wfLB2;     // used in Scenarios 5,6,7

        protected override void InitTest(TParams p)
        {
            // make window a bit smaller, insure is visible
            this.Width = 300;
            this.Height = 200;
            this.Topmost = true;
            this.Topmost = false;

            base.InitTest(p);
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("Verify that changing a bound value is represented in the WF control and the AV control.")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            
            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            // Avalon TextBox
            TextBox avTB = new TextBox();
            avTB.Name = "avTB";
            sp.Children.Add(avTB);

            // WinForms TextBox
            System.Windows.Forms.TextBox wfTB = new System.Windows.Forms.TextBox();
            wfTB.Name = "wfTB";

            // WindowsFormsHost
            WindowsFormsHost wfh = new WindowsFormsHost();
            wfh.Child = wfTB;
            sp.Children.Add(wfh);

            // Button
            Button btn = new Button();
            btn.Name = "avBTN";
            btn.Content = "Update";
            btn.Click += new RoutedEventHandler(btn_Click);
            sp.Children.Add(btn);

            // add data binding for Avalon
            avTB.DataContext = _co;
            avTB.SetBinding(TextBox.TextProperty, new System.Windows.Data.Binding("Count"));

            // add data binding for WF
            // bind "Text" property of TextBox to "Count" datamember of "co" datasource
            wfTB.DataBindings.Add(new System.Windows.Forms.Binding("Text", _co, "Count", true, System.Windows.Forms.DataSourceUpdateMode.OnPropertyChanged));

            // let system catch up
            WPFReflectBase.DoEvents();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Textbox: '{0}'", wfTB.Text);
            p.log.WriteLine("Avalon   Textbox: '{0}'", avTB.Text);

            // perform an update
            _co.Count++;

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Textbox: '{0}'", wfTB.Text);
            p.log.WriteLine("Avalon   Textbox: '{0}'", avTB.Text);

            // verify results
            string expText = "1";
            WPFMiscUtils.IncCounters(sr, expText, wfTB.Text, "WinForms TextBox Text not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expText, avTB.Text, "Avalon TextBox Text not as expected", p.log);

            return sr;
        }

        #region Helpers for Scenario 1

        void btn_Click(object sender, RoutedEventArgs e)
        {
            _co.Count++;
        }

        public class CountObj : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }

            int _count = 0;

            public int Count
            {
                get { return _count; }
                set
                {
                    this._count = value;
                    OnPropertyChanged("Count");
                }
            }
        }

        #endregion

        [Scenario("Verify that changing a bound value from a list is represented in the WF control and the AV control.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupAvalonWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // perform updates
            Perform_Update();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // verify results
            string expList = "Scott-56:Bill-81:Nate-106:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB), 
                "WinForms ListBox Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetAvalonListboxContents(),
                "Avalon ListBox Items not as expected", p.log);

            return sr;
        }

        [Scenario("Vefify that deleting a bound value from a list is represented in the WF control and the AV control.")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupAvalonWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // perform delete
            Perform_Delete();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // verify results
            string expList = "Scott-55:Bill-80:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB),
                "WinForms ListBox Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetAvalonListboxContents(),
                "Avalon ListBox Items not as expected", p.log);

            return sr;
        }

        [Scenario("Vefify that adding a bound value from a list is represented in the WF control and the AV control.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupAvalonWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // perform addition
            Perform_Add();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox: '{0}'", GetWinformsListboxContents(_wfLB));
            p.log.WriteLine("Avalon   Listbox: '{0}'", GetAvalonListboxContents());

            // verify results
            string expList = "Scott-55:Bill-80:Nate-105:Bob3-30:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB),
                "WinForms ListBox Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetAvalonListboxContents(),
                "Avalon ListBox Items not as expected", p.log);

            return sr;
        }

        [Scenario("Verify that changing a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupWinformsWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // perform updates
            Perform_Update();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // verify results
            string expList = "Scott-56:Bill-81:Nate-106:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB1),
                "WinForms ListBox 1 Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB2),
                "WinForms ListBox 2 Items not as expected", p.log);

            return sr;
        }

        [Scenario("Verify that deleting a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupWinformsWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // perform updates
            Perform_Delete();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // verify results
            string expList = "Scott-55:Bill-80:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB1),
                "WinForms ListBox 1 Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB2),
                "WinForms ListBox 2 Items not as expected", p.log);

            return sr;
        }

        [Scenario("Verify that adding a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.")]
        public ScenarioResult Scenario7(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            SetupWinformsWinformsListboxes();

            p.log.WriteLine("Before:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // perform updates
            Perform_Add();

            p.log.WriteLine("After:");
            p.log.WriteLine("Winforms Listbox 1: '{0}'", GetWinformsListboxContents(_wfLB1));
            p.log.WriteLine("Winforms Listbox 2: '{0}'", GetWinformsListboxContents(_wfLB2));

            // verify results
            string expList = "Scott-55:Bill-80:Nate-105:Bob3-30:";
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB1),
                "WinForms ListBox 1 Items not as expected", p.log);
            WPFMiscUtils.IncCounters(sr, expList, GetWinformsListboxContents(_wfLB2),
                "WinForms ListBox 2 Items not as expected", p.log);

            return sr;
        }

        #endregion

        #region Helpers for ListBox Scenarios 2,3,4,5,6,7

        private void SetupAvalonWinformsListboxes()
        {
            CreateListData();

            // DockPanel
            DockPanel dp = new DockPanel();
            this.Content = dp;

            // Avalon ListBox
            _avLB = new ListBox();
            _avLB.Name = "avLB";
            dp.Children.Add(_avLB);

            // WinForms ListBox
            _wfLB = new System.Windows.Forms.ListBox();
            _wfLB.Name = "wfLB";
            //wfLB.ScrollAlwaysVisible = true;

            // WindowsFormsHost
            WindowsFormsHost wfh = new WindowsFormsHost();
            wfh.Child = _wfLB;
            dp.Children.Add(wfh);

            // add data binding for WF
            // bind datasource of ListBox to "ppl" list object
            _wfLB.DataSource = _ppl;

            // add data binding for Avalon
            _avLB.ItemsSource = _ppl;

            // let system catch up
            WPFReflectBase.DoEvents();
        }

        private void SetupWinformsWinformsListboxes()
        {
            CreateListData();

            // DockPanel
            DockPanel dp = new DockPanel();
            this.Content = dp;

            // WinForms ListBox 1
            _wfLB1 = new System.Windows.Forms.ListBox();
            _wfLB1.Name = "wfLB1";
            //wfLB1.ScrollAlwaysVisible = true;

            // WindowsFormsHost
            WindowsFormsHost wfh1 = new WindowsFormsHost();
            wfh1.Child = _wfLB1;
            dp.Children.Add(wfh1);

            // WinForms ListBox 2
            _wfLB2 = new System.Windows.Forms.ListBox();
            _wfLB2.Name = "wfLB2";
            //wfLB2.ScrollAlwaysVisible = true;

            // WindowsFormsHost
            WindowsFormsHost wfh2 = new WindowsFormsHost();
            wfh2.Child = _wfLB2;
            dp.Children.Add(wfh2);

            // add data binding for WF
            // bind datasource of ListBox to "ppl" list object
            _wfLB1.DataSource = _ppl;
            _wfLB2.DataSource = _ppl;

            // let system catch up
            WPFReflectBase.DoEvents();
        }

        /// <summary>
        /// Helper function to create our test dataset
        /// </summary>
        private void CreateListData()
        {
            // add stuff to listboxes
            _ppl = new People();
            _ppl.Add(new Person("Scott", 55));
            _ppl.Add(new Person("Bill", 80));
            _ppl.Add(new Person("Nate", 105));
        }

        /// <summary>
        /// Helper function to create single string from WinForms listbox entries
        /// </summary>
        /// <returns></returns>
        string GetWinformsListboxContents(System.Windows.Forms.ListBox wfLB)
        {
            string list = "";
            foreach (Person per in wfLB.Items)
            {
                list += per.Name;
                list += "-";
                list += per.Speed;
                list += ":";
                //scenarioParams.log.WriteLine("WF: '{0}' '{1}'", per.Name, per.Speed);
            }
            return list;
        }

        /// <summary>
        /// Helper function to create single string from Avalon listbox entries
        /// </summary>
        /// <returns></returns>
        string GetAvalonListboxContents()
        {
            string list = "";
            foreach (Person per in _avLB.Items)
            {
                list += per.Name;
                list += "-";
                list += per.Speed;
                list += ":";
                //scenarioParams.log.WriteLine("AV: '{0}' '{1}'", per.Name, per.Speed);
            }
            return list;
        }

        /// <summary>
        /// Helper to update all rows of the dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Perform_Update()
        {
            foreach (Person p in _ppl)
            {
                p.Speed++;
            }
        }

        /// <summary>
        /// Helper to delete last item in dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Perform_Delete()
        {
            if (_ppl.Count != 0)
                _ppl.RemoveAt(_ppl.Count - 1);
        }

        /// <summary>
        /// Helper to add new item to dataset
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Perform_Add()
        {
            _ppl.Add(new Person("Bob" + _ppl.Count.ToString(), _ppl.Count * 10));
        }

        public class People : BindingList<Person> { }

        public class Person : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }

            string _name;
            public string Name
            {
                get { return _name; }
                set
                {
                    _name = value;
                    OnPropertyChanged("Name");
                }
            }

            int _speed;
            public int Speed
            {
                get { return _speed; }
                set
                {
                    _speed = value;
                    OnPropertyChanged("Speed");
                }
            }

            public override string ToString()
            {
                return _name + "-" + _speed;
            }

            public Person(string name, int speed)
            {
                this._name = name;
                this._speed = speed;
            }
        }

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ Verify that changing a bound value is represented in the WF control and the AV control.

//@ Verify that changing a bound value from a list is represented in the WF control and the AV control.

//@ Vefify that deleting a bound value from a list is represented in the WF control and the AV control.

//@ Vefify that adding a bound value from a list is represented in the WF control and the AV control.

//@ Verify that changing a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.

//@ Verify that deleting a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.

//@ Verify that adding a bound value in an WF control in one WFH is represented in the WF control hosted in another WFH.

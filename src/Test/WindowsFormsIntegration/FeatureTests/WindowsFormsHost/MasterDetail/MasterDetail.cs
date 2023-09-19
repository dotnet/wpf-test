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
using System.Windows.Data;


// Testcase:    MasterDetail
// Description: WF control represents the details of an Avalon Datasource
namespace WindowsFormsHostTests
{
    public class MasterDetail : WPFReflectBase
    {
        #region Testcase setup
        public MasterDetail(string[] args) : base(args) { }

        // class vars
        private Families _fams;
        private ListBox _avLB1;
        private System.Windows.Forms.TextBox _wfTB1;
        private System.Windows.Forms.DataGridView _wfDGV;
        private System.Windows.Forms.BindingSource _bsM;
        private System.Windows.Forms.BindingSource _bsD;
        private bool _debug = false;

        protected override void InitTest(TParams p)
        {
            // hacks to get window to show !!!
            this.Topmost = true;
            this.Topmost = false;
            this.WindowState = WindowState.Maximized;
            this.WindowState = WindowState.Normal;
            
            // set up our data set
            CreateListData();

            // make window smaller !!!
            this.Height = 300;

            base.InitTest(p);
        }

        protected override bool BeforeScenario(TParams p, System.Reflection.MethodInfo scenario)
        {
            bool b = base.BeforeScenario(p, scenario);

            // debug - run specific scenario !!!
            //if (scenario.Name != "Scenario3") { return false; }

            this.Title = currentScenario.Name;

            return b;
        }

        #endregion

        #region Families/Family Class

        public class Families : BindingList<Family> { }

        public class Family : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            protected void OnPropertyChanged(string propName)
            {
                if (this.PropertyChanged != null)
                    PropertyChanged(this, new PropertyChangedEventArgs(propName));
            }

            string _familyName;
            public string FamilyName
            {
                get { return _familyName; }
                set
                {
                    _familyName = value;
                    OnPropertyChanged("FamilyName");
                }
            }

            People _members;
            public People Members
            {
                get { return _members; }
                set
                {
                    _members = value;
                    OnPropertyChanged("Members");
                }
            }

            public override string ToString()
            {
                // debug !!!
                //scenarioParams.log.WriteLine("in ToString '{0}' '{1}'", name, age);
                return _familyName + " (" + _members.Count + " members)";
            }

            public Family(string familyName)
            {
                this._familyName = familyName;
                this._members = new People();
            }
        }

        #endregion

        #region People/Person Class

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

            int _age;
            public int Age
            {
                get { return _age; }
                set
                {
                    _age = value;
                    OnPropertyChanged("Age");
                }
            }

            public override string ToString()
            {
                // debug !!!
                //scenarioParams.log.WriteLine("in ToString '{0}' '{1}'", name, age);
                return _name + "-" + _age;
            }

            public Person(string name, int age)
            {
                this._name = name;
                this._age = age;
            }
        }

        #endregion

        //==========================================
        // Scenarios
        //==========================================
        #region Scenarios
        [Scenario("WF control as the details in a WFH with a DGV and other control (i.e. textbox, combo box, etc..)")]
        public ScenarioResult Scenario1(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();
            
            // interpret as:
            // master: main window, Avalon ListBox
            // details: main window, WF controls (DGV in WFH; TextBox in WFH)
            // (reverse of Scenario3)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            CreateAvalonControls(sp);
            CreateWinformsControls(sp);

            // set up master as ListBox
            _avLB1.ItemsSource = _bsM;
            FixCurrency();

            // set up detail
            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            _wfTB1.DataBindings.Add(b1);

            // detail as data grid
            _wfDGV.DataSource = _bsD;

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, _avLB1, _wfDGV, _wfTB1);

            return sr;
        }

        [Scenario("WF dialog with details DGV.")]
        public ScenarioResult Scenario2(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as:
            // master: main window, Avalon ListBox
            // details: new WinForms window, WF controls (DGV in WFH; TextBox in WFH)
            // (reverse of Scenario4)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            // setup main window with Avalon controls for master
            CreateAvalonControls(sp);

            // create Windows Forms window to be used as detail
            WindowsForms.Form1 Winform = new WindowsForms.Form1();
            Winform.Show();

            // set up master as ListBox
            _avLB1.ItemsSource = _bsM;
            FixCurrency();

            // set up detail
            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            //wfTB1.DataBindings.Add(b1);
            Winform.textBox1.DataBindings.Add(b1);

            // detail as data grid
            //wfDGV.DataSource = bsD;
            Winform.dataGridView1.DataSource = _bsD;

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, _avLB1, Winform.dataGridView1, Winform.textBox1);

            Winform.Close();

            return sr;
        }

        [Scenario("WF control as the master in a WFH with a DGV and other control (i.e. textbox, combo box, etc..)")]
        public ScenarioResult Scenario3(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as:
            // master: main window, WF control (DGV in WFH; TextBox in WFH)
            // details: main window, Avalon ListBox
            // (reverse of Scenario1)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            CreateWinformsControls(sp);
            CreateAvalonControls(sp);

            // set up master
            // master as data grid
            _wfDGV.DataSource = _bsM;

            // set up detail
            // detail as listbox
            _avLB1.ItemsSource = _bsD;
            //!!!avLB1.IsSynchronizedWithCurrentItem = true;

            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            _wfTB1.DataBindings.Add(b1);

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, _wfDGV, _avLB1, _wfTB1);

            return sr;
        }

        [Scenario("WF dialog with master data bound to a DGV or other control.")]
        public ScenarioResult Scenario4(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as:
            // master: new WinForms window, WF controls (DGV in WFH)
            // details: main window, Avalon ListBox
            // (reverse of Scenario2)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            // create Windows Forms window to be used as master
            WindowsForms.Form1 Winform = new WindowsForms.Form1();
            Winform.Show();

            // setup main window with Avalon controls for detail
            CreateAvalonControls(sp);

            // set up master
            Winform.dataGridView1.DataSource = _bsM;

            // set up detail
            // detail as listbox
            _avLB1.ItemsSource = _bsD;
            //!!!avLB1.IsSynchronizedWithCurrentItem = true;

            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            Winform.textBox1.DataBindings.Add(b1);

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, Winform.dataGridView1, _avLB1, Winform.textBox1);
            Winform.Close();

            return sr;
        }

        [Scenario("WF control as the master with AV details on another window.")]
        public ScenarioResult Scenario5(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as:
            // master: main window, WF control (DGV in WFH)
            // details: new Avalon window, AV ListBox
            // (reverse of Scenario6)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            // setup main window with Avalon controls for master
            CreateWinformsControls(sp);

            // create Avalon window to be used as details
            Window w = new Window();
            w.Title = "Second Avalon Window (Details)";
            StackPanel sp2 = new StackPanel();
            w.Content = sp2;
            w.Height = 300;
            CreateAvalonControls(sp2);
            w.Show();

            // set up master
            // master as data grid
            _wfDGV.DataSource = _bsM;

            // set up detail
            // detail as listbox
            _avLB1.ItemsSource = _bsD;
            //!!!avLB1.IsSynchronizedWithCurrentItem = true;

            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            _wfTB1.DataBindings.Add(b1);

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, _wfDGV, _avLB1, _wfTB1);
            w.Close();

            return sr;
        }

        [Scenario("WF control as the details with AV master on another window.")]
        public ScenarioResult Scenario6(TParams p)
        {
            ScenarioResult sr = new ScenarioResult();

            // interpret as:
            // master: new Avalon window, AV ListBox
            // details: main window, WF control (DGV in WFH)
            // (reverse of Scenario5)

            // StackPanel
            StackPanel sp = new StackPanel();
            this.Content = sp;

            // create Avalon window to be used for master
            Window w = new Window();
            w.Title = "Second Avalon Window (Master)";
            StackPanel sp2 = new StackPanel();
            w.Content = sp2;
            w.Height = 300;
            CreateAvalonControls(sp2);
            w.Show();

            // setup main window with Avalon controls for detail
            CreateWinformsControls(sp);

            // set up master as ListBox
            _avLB1.ItemsSource = _bsM;
            FixCurrency();

            // set up detail
            // detail as textbox with "currently selected master"
            System.Windows.Forms.Binding b1 = new System.Windows.Forms.Binding("Text", _bsM, "FamilyName");
            _wfTB1.DataBindings.Add(b1);

            // detail as data grid
            _wfDGV.DataSource = _bsD;

            //!!!Utilities.ActiveFreeze(currentScenario.Name);

            // check data binding to controls
            VerifyControlData(p, sr, _avLB1, _wfDGV, _wfTB1);

            w.Close();

            return sr;
        }

        #region Helpers

        /// <summary>
        /// Helper function that creates the data set that we will be playing with
        /// </summary>
        private void CreateListData()
        {
            Family f;

            // create dataset
            _fams = new Families();

            // add Families to data set
            f = new Family("Stooges");
            _fams.Add(f);
            f.Members.Add(new Person("Larry", 25));
            f.Members.Add(new Person("Moe", 26));
            f.Members.Add(new Person("Curly", 27));

            f = new Family("Peanuts");
            _fams.Add(f);
            f.Members.Add(new Person("Snoopy", 6));
            f.Members.Add(new Person("Charlie Brown", 11));
            f.Members.Add(new Person("Linus", 10));
            f.Members.Add(new Person("Woodstock", 5));
            f.Members.Add(new Person("Lucy", 13));
            f.Members.Add(new Person("Schroeder", 12));

            f = new Family("MASH");
            _fams.Add(f);
            f.Members.Add(new Person("Hawkeye", 30));
            f.Members.Add(new Person("Winchester", 31));
            f.Members.Add(new Person("Klinger", 32));
            f.Members.Add(new Person("Potter", 40));
            f.Members.Add(new Person("Radar", 29));

            f = new Family("Marx");
            _fams.Add(f);
            f.Members.Add(new Person("Harpo", 33));
            f.Members.Add(new Person("Zeppo", 34));
            f.Members.Add(new Person("Groucho", 35));
            f.Members.Add(new Person("Karl", 36));

            // set up binding sources to data
            _bsM = new System.Windows.Forms.BindingSource();
            _bsD = new System.Windows.Forms.BindingSource();
            _bsM.DataSource = _fams;
            _bsD.DataSource = _bsM;
            _bsD.DataMember = "Members";
        }

        // create structures to make verifing easier
        // (don't want to risk messing with currency when traversing data set while validating)
        private string[] _expFamilyName = { "Stooges", "Peanuts", "MASH", "Marx" };
        private int[] _expMemberCount = { 3, 6, 5, 4 };
        private string[] _expMemberList = { 
                "Larry:Moe:Curly:" ,
                "Snoopy:Charlie Brown:Linus:Woodstock:Lucy:Schroeder:",
                "Hawkeye:Winchester:Klinger:Potter:Radar:",
                "Harpo:Zeppo:Groucho:Karl:"
            };
        
        /// <summary>
        /// Helper function to bypass problem with master data list not being kept current !!!
        /// </summary>
        private void FixCurrency()
        {
            // keep data source in sync with current listbox item
            _avLB1.IsSynchronizedWithCurrentItem = true;

            // Here is the tricky part.  Due to current limitations, you have to give a little help
            // to the currency management aspect of the data models.
            // We need to create an event handler that will fire whenever the current item is changed
            // via the WPF ListBox and then force it to be in sync with the BindingSource.
            BindingListCollectionView cv = (BindingListCollectionView)CollectionViewSource.GetDefaultView(_bsM);
            cv.CurrentChanged += new EventHandler(cv_CurrentChanged);
        }

        /// <summary>
        /// ---- helper function that keeps data in sync with Avalon list !!!
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void cv_CurrentChanged(object sender, EventArgs e)
        {
            BindingListCollectionView cv = sender as BindingListCollectionView;
            _bsM.Position = cv.CurrentPosition;
        }

        /// <summary>
        /// Helper to create some Avalon controls and add to StackPanel
        /// </summary>
        /// <param name="dp"></param>
        private void CreateAvalonControls(StackPanel sp)
        {
            // Avalon ListBox
            _avLB1 = new ListBox();
            _avLB1.Name = "avLB1";
            sp.Children.Add(_avLB1);

            // Avalon TextBox
            TextBox avTB1;
            avTB1 = new TextBox();
            avTB1.Name = "avTB1";
            sp.Children.Add(avTB1);
        }

        /// <summary>
        /// Helper to create Winforms controls in Hosts and add to StackPanel
        /// </summary>
        /// <param name="dp"></param>
        private void CreateWinformsControls(StackPanel sp)
        {
            // horizontal stackpanel for label/textbox
            StackPanel sp2 = new StackPanel();
            sp2.Orientation = Orientation.Horizontal;
            sp.Children.Add(sp2);

            // Label
            TextBlock avLab = new TextBlock();
            avLab.Text = "Currently selected Family: ";
            sp2.Children.Add(avLab);

            // WinForms TextBox
            _wfTB1 = new System.Windows.Forms.TextBox();
            _wfTB1.Name = "wfTB";
            //wfLB1.ScrollAlwaysVisible = true;
            WindowsFormsHost wfh2 = new WindowsFormsHost();
            wfh2.Child = _wfTB1;
            sp2.Children.Add(wfh2);

            // WinForms DataGridView
            _wfDGV = new System.Windows.Forms.DataGridView();
            _wfDGV.Name = "wfDGV";
            WindowsFormsHost wfh1 = new WindowsFormsHost();
            wfh1.Child = _wfDGV;
            sp.Children.Add(wfh1);
        }

        /// <summary>
        /// Helper to verify that controls have proper data (Master = Avalon ListBox, Detail = DataGridView)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="LBmaster">Avalon Listbox used for master</param>
        /// <param name="DGVdetail">WinForms DataGridView used for detail</param>
        /// <param name="TBcursel">TextBox used for current selection</param>
        private void VerifyControlData(TParams p, ScenarioResult sr, ListBox LBmaster, System.Windows.Forms.DataGridView DGVdetail, System.Windows.Forms.TextBox TBcursel)
        {
            // iterate through all sets of data
            for (int i = 0; i < _fams.Count; i++)
            {
                // select item in master list (Avalon ListBox)
                p.log.WriteLine("Selecting item {0}", i);
                LBmaster.SelectedIndex = i;

                // process any events
                WPFReflectBase.DoEvents();
                System.Threading.Thread.Sleep(100);

                // check values
                int idxM = LBmaster.SelectedIndex;
                string curFam = TBcursel.Text;
                int cntDet = DGVdetail.RowCount;

                // currently selected master item should match loop index
                if (_debug) { p.log.WriteLine("Selected master is {0}", idxM); }
                WPFMiscUtils.IncCounters(sr, i, idxM, "Master list selected does not match loop index", p.log);

                // currently selected family name should match familyname corresponding to loop index
                if (_debug) { p.log.WriteLine("Current FamilyName is '{0}'", curFam); }
                WPFMiscUtils.IncCounters(sr, _expFamilyName[i], curFam, "Currently selected family is incorrect", p.log);

                // detail list count should match member count list corresponding to loop index
                if (_debug) { p.log.WriteLine("Number of detail rows '{0}'", cntDet); }
                WPFMiscUtils.IncCounters(sr, _expMemberCount[i], cntDet, "Detail list member count is incorrect", p.log);

                // look through rows in data grid, build into list
                string actList = "";
                foreach (System.Windows.Forms.DataGridViewRow row in DGVdetail.Rows)
                {
                    // for each row, look at second cell (want name, not age)
                    System.Windows.Forms.DataGridViewCell cell = row.Cells[0];
                    string val = cell.Value.ToString();
                    if (_debug) { p.log.WriteLine("  Row in Detail list '{0}'", val); }
                    actList += (val + ":");
                }

                // list of details should match list for loop index
                WPFMiscUtils.IncCounters(sr, _expMemberList[i], actList, "List of detail members is incorrect", p.log);

                if (_debug) { p.log.WriteLine(""); }
            }
        }

        /// <summary>
        /// Helper to verify that controls have proper data (Master = DataGridView, Detail = Avalon ListBox)
        /// </summary>
        /// <param name="p"></param>
        /// <param name="sr"></param>
        /// <param name="DGVmaster">WinForms DataGridView used for master</param>
        /// <param name="LBdetail">Avalon Listbox used for detail</param>
        /// <param name="TBcursel">TextBox used for current selection</param>
        private void VerifyControlData(TParams p, ScenarioResult sr, System.Windows.Forms.DataGridView DGVmaster, ListBox LBdetail, System.Windows.Forms.TextBox TBcursel)
        {
            // iterate through all sets of data
            for (int i = 0; i < _fams.Count; i++)
            {
                p.log.WriteLine("Selecting item {0}", i);
                // select item in master list (Winforms DataGridView)
                // (select row by setting first cell in desired row as current cell)
                DGVmaster.CurrentCell = DGVmaster.Rows[i].Cells[0];

                // process any events
                WPFReflectBase.DoEvents();
                System.Threading.Thread.Sleep(1000);

                // check values
                // get row index of currently selected row
                int idxM = DGVmaster.CurrentRow.Index;

                string curFam = TBcursel.Text;
                int cntDet = LBdetail.Items.Count;

                // currently selected master item should match loop index
                if (_debug) { p.log.WriteLine("Selected master is {0}", idxM); }
                WPFMiscUtils.IncCounters(sr, i, idxM, "Master list selected does not match loop index", p.log);

                // currently selected family name should match familyname corresponding to loop index
                if (_debug) { p.log.WriteLine("Current FamilyName is '{0}'", curFam); }
                WPFMiscUtils.IncCounters(sr, _expFamilyName[i], curFam, "Currently selected family is incorrect", p.log);

                // detail list count should match member count list corresponding to loop index
                if (_debug) { p.log.WriteLine("Number of detail rows '{0}'", cntDet); }
                WPFMiscUtils.IncCounters(sr, _expMemberCount[i], cntDet, "Detail list member count is incorrect", p.log);

                // look through rows in ListBox, build into list
                string actList = "";
                foreach ( Person item in LBdetail.Items)
                {
                    // each item in listbox is a Person record, we want the name field
                    string val = item.Name;
                    if (_debug) { p.log.WriteLine("  Row in Detail list '{0}'", val); }
                    actList += (val + ":");
                }

                // list of details should match list for loop index
                WPFMiscUtils.IncCounters(sr, _expMemberList[i], actList, "List of detail members is incorrect", p.log);

                if (_debug) { p.log.WriteLine(""); }
            }
        }

        #endregion

        #endregion
    }
}

// Keep these in sync by running the testcase locally through the driver whenever
// you add, remove, or rename scenarios.
//
// [Scenarios]
//@ WF control as the details in a WFH with a DGV and other control (i.e. textbox, combo box, etc..)

//@ WF dialog with details DGV.

//@ WF control as the master in a WFH with a DGV and other control (i.e. textbox, combo box, etc..)

//@ WF dialog with master data bound to a DGV or other control.

//@ WF control as the master with AV details on another window.

//@ WF control as the details with AV master on another window.

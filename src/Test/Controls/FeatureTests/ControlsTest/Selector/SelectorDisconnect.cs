using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;
using Microsoft.Test.Controls.Actions;
using Microsoft.Test.Discovery;
using Microsoft.Test.Logging;
using Microsoft.Test.TestTypes;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// <description>
    /// SelectorDisconnect
    /// </description>
    /// </summary>
    [Test(1, "Selection", "SelectorDisconnect", Versions="4.8+")]
    public class SelectorDisconnect : XamlTest
    {
        #region Private Members
        const string HoldForInteractionString = @"WPF_Test_Hold";

        ListBox _listbox;
        DataGrid _datagrid;
        RegressionTest88Model _model;

        Button _debug;
        DispatcherFrame _debugFrame;
        #endregion

        #region Public Members

        public SelectorDisconnect()
            : base(@"SelectorDisconnect.xaml")
        {
            InitializeSteps += new TestStep(Setup);
            CleanUpSteps += new TestStep(CleanUp);

            if (Environment.GetEnvironmentVariable(HoldForInteractionString) != null)
            {
                RunSteps += new TestStep(HoldForInteraction);
            }

            RunSteps += new TestStep(DisconnectInListBox);
            RunSteps += new TestStep(DisconnectInDataGrid);
        }

        public TestResult Setup()
        {
            Status("Setup");

            _model = new RegressionTest88Model(50);
            RootElement.DataContext = _model;

            WaitForPriority(DispatcherPriority.ApplicationIdle);

            _listbox = (ListBox)RootElement.FindName("listbox");
            if (_listbox == null)
            {
                throw new TestValidationException("ListBox is null");
            }

            _datagrid = (DataGrid)RootElement.FindName("datagrid");
            if (_datagrid == null)
            {
                throw new TestValidationException("DataGrid is null");
            }

            _debug = (Button)RootElement.FindName("debug");
            if (_debug == null)
            {
                throw new TestValidationException("Debug is null");
            }
            _debug.Click += OnDebugClick;

            LogComment("Setup was successful");

            return TestResult.Pass;
        }

        public TestResult CleanUp()
        {
            _listbox = null;
            _datagrid = null;
            return TestResult.Pass;
        }

        public TestResult HoldForInteraction()
        {
            // wait for user to click the Resume button,
            // used for interactive debugging
            ToggleInteraction();
            return TestResult.Pass;
        }

        public TestResult DisconnectInListBox()
        {
            return TestDisconnect(_listbox);
        }

        public TestResult DisconnectInDataGrid()
        {
            return TestDisconnect(_datagrid);
        }

        TestResult TestDisconnect(ItemsControl ic)
        {
            // scrolling the control will recycle its containers,
            // disconnecting the Selector (ComboBox) in the container from its
            // data source.  This should not change the SelectedValue, either
            // in the data model or in the display.  We verify this by scrolling
            // containers back into view and checking the displayed SelectedValue
            // (it should not be null)

            // start with fresh data
            _model.Reload(50);
            WaitForPriority(DispatcherPriority.ApplicationIdle);

            // get the scroll viewer
            ScrollViewer scroller = VisualTreeHelper.GetVisualChild<ScrollViewer>(ic);

            // scroll to the bottom, by line
            double oldOffset = scroller.VerticalOffset;
            while (true)
            {
                scroller.LineDown();
                WaitForPriority(DispatcherPriority.ApplicationIdle);

                double newOffset = scroller.VerticalOffset;
                if (newOffset <= oldOffset)
                    break;

                oldOffset = newOffset;
            }

            // now scroll to the top
            while (scroller.VerticalOffset > 0.0)
            {
                scroller.LineUp();
                WaitForPriority(DispatcherPriority.ApplicationIdle);
            }

            // check SelectedValue of each realized ComboBox
            bool foundNull = false;
            ItemContainerGenerator generator = ic.ItemContainerGenerator;
            for (int i=0; ; ++i)
            {
                DependencyObject container = generator.ContainerFromIndex(i);
                if (container == null)
                    break;
                ComboBox combobox = VisualTreeHelper.GetVisualChild<ComboBox>(container);
                if (combobox != null)
                {
                    if (combobox.SelectedValue == null)
                    {
                        foundNull = true;
                        LogComment(String.Format("SelectedValue is null for item {0}", i));
                    }
                }
            }

            if (!foundNull)
                return TestResult.Pass;

            return TestResult.Fail;
        }

        void OnDebugClick(object sender, EventArgs e)
        {
            ToggleInteraction();
        }

        void ToggleInteraction()
        {
            if (_debugFrame == null)
            {
                _debug.Content = "Resume";
                _debugFrame = new DispatcherFrame();
                Dispatcher.PushFrame(_debugFrame);
            }
            else
            {
                _debugFrame.Continue = false;
                _debugFrame = null;
                _debug.Content = "Debug";
            }
        }

        #endregion
    }

    public class RegressionTest88Model : INotifyPropertyChanged
    {
        public RegressionTest88Model(int size)
        {
            Reload(size);
        }

        public void Reload(int size)
        {
            ObservableCollection<RegressionTest88Item> data = new ObservableCollection<RegressionTest88Item>();
            for (int i=0; i<size; ++i)
            {
                data.Add(new RegressionTest88Item { ID = 1000+i });
            }
            Data = data;
        }

        ObservableCollection<RegressionTest88Item> _data;
        public ObservableCollection<RegressionTest88Item> Data
        {
            get { return _data; }
            private set { _data = value; OnPropertyChanged(nameof(Data)); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }

    public class RegressionTest88Item : INotifyPropertyChanged
    {
        static List<string> s_StatusValues;

        static RegressionTest88Item()
        {
            s_StatusValues = new List<string>();
            s_StatusValues.Add("None");
            s_StatusValues.Add("On Time");
            s_StatusValues.Add("Delayed");
            s_StatusValues.Add("Boarding");
            s_StatusValues.Add("Departed");
        }

        int _id;
        public int ID
        {
            get { return _id; }
            set { _id = value;  OnPropertyChanged(nameof(ID)); }
        }

        string _status = s_StatusValues[0];
        public string Status
        {
            get { return _status; }
            set { _status = value;  OnPropertyChanged(nameof(Status)); }
        }

        public List<string> StatusValues
        {
            get { return s_StatusValues; }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            var handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }
}


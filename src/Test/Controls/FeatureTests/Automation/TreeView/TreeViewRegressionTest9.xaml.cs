using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    public partial class TreeViewRegressionTest9 : Page
    {
        RegressionTest9Model _model = new RegressionTest9Model();

        public TreeViewRegressionTest9()
        {
            InitializeComponent();
            Dispatcher.UnhandledException += Dispatcher_UnhandledException;
            Reset();
            DataContext = _model;
        }

        private void Dispatcher_UnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            errorList.Items.Add(String.Format("{0} {1} {2} {3} {4} {5} threw {6}: {7}",
                cbDisable.IsChecked, cbTopDown.IsChecked,
                cb1stRemove.SelectedIndex, cb1stAdd.SelectedIndex,
                cb2ndRemove.SelectedIndex, cb2ndAdd.SelectedIndex,
                e.Exception.GetType().FullName, e.Exception.Message));

            // try to resume (this is optimistic)
            e.Handled = true;
        }

        void Reset()
        {
            tvGrid.IsEnabled = true;
            _model.Reset(2);
        }

        private void Reset(object sender, RoutedEventArgs e)
        {
            Reset();
        }

        private void Go(object sender, RoutedEventArgs e)
        {
            if (cbDisable.IsChecked == true)
            {
                tvGrid.IsEnabled = false;
            }

            _model.Go((cbTopDown.IsChecked == true), cb1stRemove.SelectedIndex, cb1stAdd.SelectedIndex, cb2ndRemove.SelectedIndex, cb2ndAdd.SelectedIndex);
        }

        // loop through all settings
        private void Loop(object sender, RoutedEventArgs e)
        {
            // reset to initial state
            cbDisable.IsChecked = false;
            cbTopDown.IsChecked = false;
            cb1stRemove.SelectedIndex = 0;
            cb1stAdd.SelectedIndex = 0;
            cb2ndRemove.SelectedIndex = 0;
            cb2ndAdd.SelectedIndex = 0;
            errorList.Items.Clear();

            // start the loop
            Reset();
            Dispatcher.BeginInvoke((Action)LoopBody, DispatcherPriority.ApplicationIdle);
        }

        private void LoopBody()
        {
            // run scenario with current settings
            Go(null, null);

            // wait for it...
            Dispatcher.BeginInvoke((Action)LoopAdvance, DispatcherPriority.ApplicationIdle);
        }

        // advance settings to the next configuration
        private void LoopAdvance()
        {
            if (cb2ndAdd.SelectedIndex < 2)
            {
                ++cb2ndAdd.SelectedIndex;
            }
            else
            {
                cb2ndAdd.SelectedIndex = 0;
                if (cb2ndRemove.SelectedIndex < 2)
                {
                    ++cb2ndRemove.SelectedIndex;
                }
                else
                {
                    cb2ndRemove.SelectedIndex = 0;
                    if (cb1stAdd.SelectedIndex < 2)
                    {
                        ++cb1stAdd.SelectedIndex;
                    }
                    else
                    {
                        cb1stAdd.SelectedIndex = 0;
                        if (cb1stRemove.SelectedIndex < 2)
                        {
                            ++cb1stRemove.SelectedIndex;
                        }
                        else
                        {
                            cb1stRemove.SelectedIndex = 0;
                            if (cbTopDown.IsChecked != true)
                            {
                                cbTopDown.IsChecked = true;
                            }
                            else
                            {
                                cbTopDown.IsChecked = false;
                                if (cbDisable.IsChecked != true)
                                {
                                    cbDisable.IsChecked = true;
                                }
                                else
                                {
                                    cbDisable.IsChecked = false;
                                    // we've cycled through all settings
                                    errorList.Items.Add(String.Format("Done: {0} errors", errorList.Items.Count));
                                    return;
                                }
                            }
                        }
                    }
                }
            }

            // continue the loop
            Reset();
            Dispatcher.BeginInvoke((Action)LoopBody, DispatcherPriority.ApplicationIdle);
        }
    }

    public class RegressionTest9Model : INotifyPropertyChanged
    {
        int _seq = 0;
        ObservableCollection<RegressionTest9MainItem> _mainItems = new ObservableCollection<RegressionTest9MainItem>();
        List<string> _removeActions = new List<string> { "None", "Remove", "Clear" };
        List<string> _addActions = new List<string> { "None", "Add", "Restore" };

        public ObservableCollection<RegressionTest9MainItem> MainItems { get { return _mainItems; } }
        public List<string> RemoveActions { get { return _removeActions; } }
        public List<string> AddActions { get { return _addActions; } }

        public void Reset(int n)
        {
            _seq = 0;
            RegressionTest9MainItem mainItem = new RegressionTest9MainItem(_seq++, 0);
            for (int i = 0; i < n; ++i)
            {
                RegressionTest9SubItem subItem = new RegressionTest9SubItem(_seq++, 0, i);
                mainItem.Add(subItem);
            }

            MainItems.Clear();
            MainItems.Add(mainItem);
        }

        public void Go(bool topDown, int remove1st, int add1st, int remove2nd, int add2nd)
        {
            RegressionTest9MainItem firstItem = (MainItems.Count > 0) ? MainItems[0] : null;

            if (topDown)
            {
                Do1st(firstItem, remove1st, add1st);
                Do2nd(firstItem, remove2nd, add2nd);
            }
            else
            {
                Do2nd(firstItem, remove2nd, add2nd);
                Do1st(firstItem, remove1st, add1st);
            }
        }

        void Do1st(RegressionTest9MainItem firstItem, int remove, int add)
        {
            switch (remove)
            {
                case 1: // Remove
                    if (firstItem != null)
                    {
                        MainItems.Remove(firstItem);
                    }
                    break;
                case 2: // Clear
                    MainItems.Clear();
                    break;
            }

            switch (add)
            {
                case 1: // Add
                    int index = _mainItems.Count;
                    RegressionTest9MainItem item = new RegressionTest9MainItem(_seq++, index);
                    item.Add(new RegressionTest9SubItem(_seq++, index, 0));
                    MainItems.Add(item);
                    break;
                case 2: // Restore
                    if (firstItem != null)
                    {
                        MainItems.Add(firstItem);
                    }
                    break;
            }
        }

        void Do2nd(RegressionTest9MainItem parent, int remove, int add)
        {
            if (parent == null) return;

            ObservableCollection<RegressionTest9SubItem> subItems = parent.SubItems;
            RegressionTest9SubItem firstItem = (subItems.Count > 0) ? subItems[0] : null;

            switch (remove)
            {
                case 1: // Remove
                    subItems.Remove(firstItem);
                    break;
                case 2: // Clear
                    subItems.Clear();
                    break;
            }

            switch (add)
            {
                case 1: // Add
                    int index = MainItems.IndexOf(parent);
                    int subIndex = subItems.Count;
                    subItems.Add(new RegressionTest9SubItem(_seq++, index, subIndex));
                    break;
                case 2: // Restore
                    if (firstItem != null)
                    {
                        subItems.Add(firstItem);
                    }
                    break;
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(name));
            }
        }
    }

    public class RegressionTest9MainItem
    {
        ObservableCollection<RegressionTest9SubItem> _subItems = new ObservableCollection<RegressionTest9SubItem>();

        public string Name { get; set; }
        public ObservableCollection<RegressionTest9SubItem> SubItems { get { return _subItems; } }

        public RegressionTest9MainItem(int seq, int index)
        {
            Name = String.Format("RegressionTest9MainItem {0} {1}", seq, index);
        }

        public void Add(RegressionTest9SubItem subItem)
        {
            _subItems.Add(subItem);
        }

        public void Clear()
        {
            _subItems.Clear();
        }

        public override string ToString()
        {
            return Name;
        }
    }

    public class RegressionTest9SubItem
    {
        public string Name { get; set; }

        public RegressionTest9SubItem(int seq, int index, int subindex)
        {
            Name = String.Format("RegressionTest9SubItem {0} {1}.{2} ", seq, index, subindex);
        }

        public override string ToString()
        {
            return Name;
        }
    }
}

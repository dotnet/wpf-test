using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Data;

namespace Microsoft.Test.Controls.DataSources
{
    public class TabGroups : ObservableCollection<TabGroup>
    {
        public TabGroups()
        {
            Add(new TabGroup("A"));
            Add(new TabGroup("B"));
        }
    }

    public class TabGroup : INotifyPropertyChanged
    {
        private string _name;
        private ObservableCollection<string> _children;
        private CollectionViewSource _source;
        private string _selectedElement;

        public TabGroup(string name)
        {
            _name = name;
            _children = new ObservableCollection<string>();

            for (int k = 0; k < 2; ++k)
            {
                _children.Add(String.Format("{0}.{1}", name, k + 1));
            }
        }

        public string Name { get { return _name; } }
        public ObservableCollection<string> Children { get { return _children; } }

        public string SelectedElement
        {
            get { return _selectedElement; }
            set
            {
                _selectedElement = value;
                OnPropertyChanged("SelectedElement");
            }
        }

        public ICollectionView VisibleChildren
        {
            get
            {
                if (_source == null)
                {
                    _source = new CollectionViewSource();
                    _source.Source = Children;
                }

                return _source.View;
            }
        }

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        private void OnPropertyChanged(string name)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(name));
            }
        }

        #endregion INotifyPropertyChanged
    }
}

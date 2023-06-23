using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Media;
using Avalon.Test.ComponentModel.Utilities;
using Microsoft.Test.Logging;

namespace Microsoft.Test.Controls.DataSources
{
    public class TreeNode : INotifyPropertyChanged
    {
        private bool isSelected;
        private bool isExpanded;

        public TreeNode(string name)
        {
            this.Name = name;
            this.Children = new ObservableCollection<TreeNode>();
        }

        public string Name
        {
            get;
            private set;
        }

        public ObservableCollection<TreeNode> Children
        {
            get;
            private set;
        }

        public bool IsSelected
        {
            get
            {
                return this.isSelected;
            }
            set
            {
                if (this.isSelected != value)
                {
                    this.isSelected = value;
                    NotifyPropertyChanged("IsSelected");
                }
            }
        }

        public bool IsExpanded
        {
            get
            {
                return this.isExpanded;
            }
            set
            {
                if (this.isExpanded != value)
                {
                    this.isExpanded = value;
                    NotifyPropertyChanged("IsExpanded");
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(string propertyName)
        {
            var eh = this.PropertyChanged;

            if (eh != null)
            {
                eh(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}

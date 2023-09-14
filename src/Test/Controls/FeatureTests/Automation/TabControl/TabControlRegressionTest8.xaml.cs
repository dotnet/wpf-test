using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Microsoft.Test.Controls
{
    public partial class TabControlRegressionTest8 : Page
    {
        WelcomePageViewModel _vmWelcome = new WelcomePageViewModel();

        public TabControlRegressionTest8()
        {
            InitializeComponent();
            rbWelcome.IsChecked = true;
        }

        private void SelectView(object sender, RoutedEventArgs e)
        {
            LaunchPageViewModel model;

            RadioButton rb = sender as RadioButton;
            switch (rb.Content as string)
            {
                case "Welcome":
                    model = MainContentControl.Content as LaunchPageViewModel;
                    if (model != null)
                    {
                        MainContentControl.IsEnabled = false;
                        model.Repopulate();
                    }
                    MainContentControl.Content = _vmWelcome;
                    break;
                case "Launch":
                    model = new LaunchPageViewModel();
                    model.Repopulate();
                    MainContentControl.Content = model;
                    break;
            }
        }
    }


    public partial class WelcomePageView : UserControl
    {
        public WelcomePageView()
        {
            InitializeComponent();
        }
    }

    public class WelcomePageViewModel : INotifyPropertyChanged
    {
        string _message = "Welcome";
        public string Message
        {
            get { return _message; }
            set { _message = value; OnPropertyChanged("Message"); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }


    public partial class LaunchPageView : UserControl
    {
        public LaunchPageView()
        {
            InitializeComponent();
        }
    }

    public class LaunchPageViewModel : INotifyPropertyChanged
    {
        FolderItem _root = new FolderItem() { IsExpanded = true };
        public FolderItem Root { get { return _root; } }

        public void Repopulate()
        {
            ObservableCollection<FileItem> files = _root.Files;

            files.Clear();
            files.Add(NewFolder("App_data", 2));
            files.Add(NewFolder("bin", 9));
            files.Add(NewFolder("Content", 8));
            files.Add(NewFolder("Images", 3));
            files.Add(NewFolder("Scripts", 11));
            for (int i=0; i<9; ++i)
            {
                files.Add(new FileItem { Name = "file" + i + ".ext" });
            }
        }

        FolderItem NewFolder(string name, int count)
        {
            FolderItem folder = new FolderItem { Name = name };
            for (int i=0; i<count; ++i)
            {
                folder.Files.Add(new FileItem { Name = "file" + i + ".ext" });
            }
            return folder;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }
    }


    public class FileItem: INotifyPropertyChanged
    {
        string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; OnPropertyChanged("Name"); }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string name)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
                handler(this, new PropertyChangedEventArgs(name));
        }

    }

    public class FolderItem : FileItem
    {
        ObservableCollection<FileItem> _files = new ObservableCollection<FileItem>();
        public ObservableCollection<FileItem> Files { get { return _files; } }

        bool _isExpanded;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set { _isExpanded = value;  OnPropertyChanged("IsExpanded"); }
        }
    }

}

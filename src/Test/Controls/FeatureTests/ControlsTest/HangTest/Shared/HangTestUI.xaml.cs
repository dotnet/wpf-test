using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Xml;

namespace HangTest
{
    /// <summary>
    /// Interaction logic for Page1.xaml
    /// </summary>
    public partial class HangTestUI : Page
    {
        public HangTestUI()
        {
            InitializeComponent();
        }

        public Grid MainGrid { get { return _mainGrid; } }
        public ItemsControl ErrorList { get { return _errorList; } }
        public FrameworkElement ResourceHolder { get { return _resourceHolder; } }

        private void Filename_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.DefaultExt = ".xml"; // Default file extension
            dlg.Filter = "XML files (.xml)|*.xml"; // Filter files by extension 

            Nullable<bool> result = dlg.ShowDialog();
            if (result == true)
            {
                string filename = dlg.FileName;
                _tbFilename.Text = filename;
                BindingOperations.GetBindingExpression(_tbFilename, TextBox.TextProperty).UpdateSource();
            }
        }
    }
}

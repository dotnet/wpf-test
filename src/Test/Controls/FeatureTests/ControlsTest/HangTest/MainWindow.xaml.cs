using System;
using System.Windows.Navigation;

namespace HangTest
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : NavigationWindow
    {
        Controller _controller;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void NavigationWindow_LoadCompleted(object sender, NavigationEventArgs e)
        {
            HangTestUI ui = this.Content as HangTestUI;
            _controller = new Controller(ui.MainGrid, ui.ErrorList, ui.ResourceHolder);
        }
    }
}

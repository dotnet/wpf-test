using System;
using System.Windows;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
            RibbonQATOverflowViewModel vm = new RibbonQATOverflowViewModel();
            DataContext = vm;
        }
    }
}

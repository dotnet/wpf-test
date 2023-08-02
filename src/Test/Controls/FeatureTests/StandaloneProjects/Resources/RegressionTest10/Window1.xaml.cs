using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// A standalone wpf project to ensure no exception occurs when using ThemeDictionaryExtension with a 3rd party windows theme
    /// 
    /// Note: The XamlParserException occurs on partial class Window1 constructor InitializeComponent method.  
    /// WPF window won’t show up when the exception happens, so I am hoping the test framework catches the exception if it happens.  
    /// </summary>
    public partial class Window1 : Window
    {
        public Window1()
        {
            InitializeComponent();
        }

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
            InputHelper.MouseClickCenter(result, MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }
    }
}

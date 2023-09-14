using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Threading;
using Microsoft.Test;
using Microsoft.Test.Logging;

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
            DataItem item = new DataItem();
            button = new Button() { Name = "toggleDock", Content = "Toggle Dock" };
            testButton.Click += new RoutedEventHandler(Button_Click);

            button.Click += delegate
            {
                Window window = HwndSource.FromVisual(button).RootVisual as Window;
                label.Content = null;
                if (window is Window1)
                {
                    Window floatingWindow = new Window();
                    floatingWindow.Owner = window;
                    floatingWindow.Content = item;
                    floatingWindow.Width = 200;
                    floatingWindow.Height = 200;
                    floatingWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
                    floatingWindow.Show();
                    floatingWindow.ContentRendered += delegate
                    {
                        InputHelper.MouseClickCenter(testButton, MouseButton.Left);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);

                        // We need to click one more time to invoke the button
                        Microsoft.Test.Input.Mouse.Click(Microsoft.Test.Input.MouseButton.Left);
                        DispatcherOperations.WaitFor(DispatcherPriority.ApplicationIdle);

                        if (!isClicked)
                        {
                            result.Content = "Fail: couldn't click 'Click me' button because main window doesn't have focus.";
                            return;
                        }

                        InputHelper.MouseClickCenter(result, MouseButton.Left);
                        DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
                    };
                }
                else
                {
                    window.Close();
                    label.Content = item;
                }
            };
            item.Content = new DataItem() { Content = button };
            label.Content = item;
        }

        private Button button;
        private bool isClicked = false;

        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            InputHelper.MouseClickCenter(button, MouseButton.Left);
            DispatcherOperations.WaitFor(DispatcherPriority.SystemIdle);
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            isClicked = true;
            GlobalLog.LogStatus("*** Button_Click is fired. ***");
        }
    }

    class DataItem : DependencyObject
    {
        public static readonly DependencyProperty ContentProperty =
            DependencyProperty.Register("Content", typeof(object), typeof(DataItem));
        public object Content
        {
            get { return (object)GetValue(ContentProperty); }
            set { SetValue(ContentProperty, value); }
        }
    }
}

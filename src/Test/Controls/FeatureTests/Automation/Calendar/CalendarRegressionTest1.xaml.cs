using System;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace Microsoft.Test.Controls
{
    /// <summary>
    /// Interaction logic for Calendar.xaml
    /// Regression Test 
    /// </summary>
    public partial class CalendarRegressionTest1 : Page
    {
        public CalendarRegressionTest1()
        {
            InitializeComponent();
        }

        // Ensure no exception occurs when create calendar and datepicker on separate STA threads
        private void RunTest_Click(object sender, RoutedEventArgs e)
        {
            ThreadStart CreateCalendarAndDatePickerOnWorkerThread = () =>
            {
                Window window = new Window();
                StackPanel panel = new StackPanel();
                Calendar calendar = new Calendar();
                DatePicker datepicker = new DatePicker();
                panel.Children.Add(calendar);
                panel.Children.Add(datepicker);
                window.Content = panel;
                window.ShowDialog();
            };

            TestOnSeparateSTAThreads(CreateCalendarAndDatePickerOnWorkerThread);

            InputHelper.MouseClickCenter(result, MouseButton.Left);
        }

        // We need two threads to repro the bugs. That's why we use two threads for regression test.
        private void TestOnSeparateSTAThreads(ThreadStart starter)
        {
            Thread thread1 = new Thread(starter);
            thread1.SetApartmentState(ApartmentState.STA);
            thread1.Start();

            // We need to wait for one second to create another thread
            DispatcherOperations.WaitFor(TimeSpan.FromSeconds(1));

            Thread thread2 = new Thread(starter);
            thread2.SetApartmentState(ApartmentState.STA);
            thread2.Start();

            // wait for one second to clean up the threads
            DispatcherOperations.WaitFor(TimeSpan.FromSeconds(1));

            // Clean up
            Cleanup(thread1);
            Cleanup(thread2);
        }

        void Cleanup(Thread thread)
        {
            Dispatcher dispatcher = Dispatcher.FromThread(thread);
            dispatcher.InvokeShutdown();
            thread.Join();
        }
    }
}
